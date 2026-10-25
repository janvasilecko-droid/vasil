# Parser GDScriptu (Godot 4) -> jednoduchý AST (n-tice). Pokrývá podmnožinu použitou ve Zajíc 2D.
import re, sys

class GdSyntax(Exception):
    pass

KEYWORDS = {'if','elif','else','for','while','match','break','continue','pass','return','class','class_name','extends','is','in','as','self',
            'signal','func','static','const','enum','var','await','and','or','not','true','false','null','preload','assert'}

TOKEN_RE = re.compile(r'''
  (?P<ws>[ \t]+)
 |(?P<comment>\#[^\n]*)
 |(?P<nl>\n)
 |(?P<cont>\\\n)
 |(?P<tstr>[&^r]?(?:"""(?:\\.|[^\\])*?"""|\'\'\'(?:\\.|[^\\])*?\'\'\'))
 |(?P<str>[&^r]?(?:"(?:\\.|[^"\\\n])*"|'(?:\\.|[^'\\\n])*'))
 |(?P<num>0[xX][0-9a-fA-F_]+|0[bB][01_]+|(?:\d[\d_]*\.(?!\.)[\d_]*|\.\d[\d_]*|\d[\d_]*)(?:[eE][+-]?\d+)?)
 |(?P<id>[^\W\d]\w*)
 |(?P<op>\*\*=|<<=|>>=|\*\*|<<|>>|==|!=|<=|>=|&&|\|\||->|\+=|-=|\*=|/=|%=|&=|\|=|\^=|:=|\.\.|[-+*/%&|^~!<>=.,:;()\[\]{}@$?])
''', re.X | re.S)

def unescape(s, raw=False):
    if raw: return s
    out = []; i = 0
    while i < len(s):
        c = s[i]
        if c == '\\' and i + 1 < len(s):
            d = s[i+1]; i += 2
            if d == 'n': out.append('\n')
            elif d == 't': out.append('\t')
            elif d == 'r': out.append('\r')
            elif d == '\\': out.append('\\')
            elif d == '"': out.append('"')
            elif d == "'": out.append("'")
            elif d == 'u':
                out.append(chr(int(s[i:i+4], 16))); i += 4
            elif d == 'U':
                out.append(chr(int(s[i:i+6], 16))); i += 6
            elif d == '\n': pass
            else: out.append('\\' + d)
        else:
            out.append(c); i += 1
    return ''.join(out)

def lambda_header(toks):
    """končí tokenová řada hlavičkou lambdy 'func [jm](...) [-> T] :' ?"""
    j = len(toks) - 1
    if not (toks[j][0] == 'OP' and toks[j][1] == ':'): return False
    k = j - 1
    # volitelný návratový typ
    m = k
    while m >= 0 and (toks[m][0] == 'ID' or (toks[m][0] == 'OP' and toks[m][1] in ('.', '[', ']'))): m -= 1
    if m >= 0 and toks[m][0] == 'OP' and toks[m][1] == '->': k = m - 1
    if not (k >= 0 and toks[k][0] == 'OP' and toks[k][1] == ')'): return False
    d = 0
    while k >= 0:
        if toks[k][0] == 'OP' and toks[k][1] == ')': d += 1
        elif toks[k][0] == 'OP' and toks[k][1] == '(':
            d -= 1
            if d == 0: break
        k -= 1
    if k < 1: return False
    p = k - 1
    if toks[p][0] == 'ID' and toks[p][1] != 'func' and p >= 1: p -= 1
    return toks[p][0] == 'ID' and toks[p][1] == 'func'


def lex(src, fname='?'):
    toks = []  # (kind, value, line)
    indents = [0]
    depth = 0
    frames = []    # víceřádkové lambdy uvnitř závorek: {'depth': d, 'ind': [šířky], 'closed': False}
    pos = 0; line = 1
    at_line_start = True
    n = len(src)
    while pos < n:
        if at_line_start and depth == 0:
            # měření odsazení
            m = re.match(r'[ \t]*', src[pos:])
            ws = m.group(0)
            width = sum(4 if ch == '\t' else 1 for ch in ws)
            rest = src[pos + len(ws):pos + len(ws) + 1]
            if rest in ('\n', '#', ''):
                # prázdný / komentářový řádek
                pos += len(ws)
                mm = re.match(r'#[^\n]*', src[pos:])
                if mm: pos += len(mm.group(0))
                if pos < n and src[pos] == '\n':
                    pos += 1; line += 1
                continue
            pos += len(ws)
            if width > indents[-1]:
                indents.append(width); toks.append(('INDENT', None, line))
            else:
                while width < indents[-1]:
                    indents.pop(); toks.append(('DEDENT', None, line))
                if width != indents[-1]:
                    raise GdSyntax('%s:%d: nekonzistentní odsazení' % (fname, line))
            at_line_start = False
            continue
        m = TOKEN_RE.match(src, pos)
        if not m:
            raise GdSyntax('%s:%d: neznámý znak %r' % (fname, line, src[pos]))
        kind = m.lastgroup; text = m.group(0)
        pos = m.end()
        if kind == 'ws' or kind == 'comment': continue
        if kind == 'cont':
            line += 1; continue
        if kind == 'nl':
            line += 1
            if depth == 0:
                if toks and toks[-1][0] not in ('NEWLINE', 'INDENT', 'DEDENT'):
                    toks.append(('NEWLINE', None, line - 1))
                at_line_start = True
            else:
                # další neprázdný řádek: šířka odsazení
                q = pos
                while True:
                    mm = re.match(r'[ \t]*', src[q:])
                    wd = sum(4 if ch == '\t' else 1 for ch in mm.group(0))
                    q2 = q + len(mm.group(0))
                    if q2 < n and src[q2] == '\n': q = q2 + 1; continue
                    if q2 < n and src[q2] == '#':
                        e2 = src.find('\n', q2)
                        if e2 < 0: break
                        q = e2 + 1; continue
                    break
                fr = frames[-1] if frames else None
                if fr is None and toks and toks[-1][0] == 'OP' and toks[-1][1] == ':' and lambda_header(toks):
                    fr = {'depth': depth, 'ind': [], 'closed': False}; frames.append(fr)
                if fr is not None and fr['depth'] == depth and not fr['closed']:
                    if toks[-1][0] not in ('NEWLINE', 'INDENT', 'DEDENT') and not (toks[-1][0] == 'OP' and toks[-1][1] == ':' and not fr['ind']):
                        toks.append(('NEWLINE', None, line - 1))
                    elif toks[-1][0] == 'OP' and toks[-1][1] == ':' and not fr['ind']:
                        toks.append(('NEWLINE', None, line - 1))
                    if not fr['ind']:
                        fr['ind'].append(wd); toks.append(('INDENT', None, line))
                    elif wd > fr['ind'][-1]:
                        fr['ind'].append(wd); toks.append(('INDENT', None, line))
                    else:
                        while fr['ind'] and wd < fr['ind'][-1]:
                            fr['ind'].pop(); toks.append(('DEDENT', None, line))
                        if not fr['ind']: fr['closed'] = True
                else:
                    toks.append(('BNL', (wd, depth), line))
            continue
        if kind in ('str', 'tstr'):
            pre = ''
            body = text
            if body[0] in '&^r': pre = body[0]; body = body[1:]
            q = body[:3] if kind == 'tstr' else body[:1]
            inner = body[len(q):-len(q)]
            line += text.count('\n')
            toks.append(('STR', (unescape(inner, pre == 'r'), pre), line))
            continue
        if kind == 'num':
            t = text.replace('_', '')
            if t[:2].lower() == '0x': toks.append(('NUM', (int(t, 16), False), line))
            elif t[:2].lower() == '0b': toks.append(('NUM', (int(t, 2), False), line))
            elif re.fullmatch(r'\d+', t): toks.append(('NUM', (int(t), False), line))
            else: toks.append(('NUM', (float(t), True), line))
            continue
        if kind == 'id':
            toks.append(('ID', text, line)); continue
        if kind == 'op':
            if text in '([{': depth += 1
            elif text in ')]}':
                if frames and frames[-1]['depth'] == depth:
                    fr = frames.pop()
                    while fr['ind']:
                        fr['ind'].pop(); toks.append(('DEDENT', None, line))
                depth -= 1
            elif text == ',' and frames and frames[-1]['depth'] == depth:
                fr = frames.pop()
                while fr['ind']:
                    fr['ind'].pop(); toks.append(('DEDENT', None, line))
            toks.append(('OP', text, line)); continue
    if toks and toks[-1][0] not in ('NEWLINE', 'DEDENT'):
        toks.append(('NEWLINE', None, line))
    while len(indents) > 1:
        indents.pop(); toks.append(('DEDENT', None, line))
    toks.append(('EOF', None, line))
    out = []; d = 0
    for tk in toks:
        out.append((tk[0], tk[1], tk[2], d))
        if tk[0] == 'OP':
            if tk[1] in '([{': d += 1
            elif tk[1] in ')]}': d -= 1
    return out


class Parser:
    def __init__(self, toks, fname='?'):
        self.t = toks; self.i = 0; self.f = fname
        self.stop = []   # hloubky závorek, ve kterých se BNL nepřeskakuje (tělo víceřádkové lambdy)

    def norm(self):
        while self.t[self.i][0] == 'BNL' and not (self.stop and self.t[self.i][1][1] == self.stop[-1]):
            self.i += 1

    # ---------- základ ----------
    def peek(self, k=0):
        self.norm()
        j = self.i; n = 0
        while True:
            tk = self.t[min(j, len(self.t) - 1)]
            if tk[0] == 'BNL' and not (self.stop and tk[1][1] == self.stop[-1]):
                j += 1; continue
            if n == k: return tk
            n += 1; j += 1
    def kind(self): self.norm(); return self.t[self.i][0]
    def val(self): self.norm(); return self.t[self.i][1]
    def line(self): self.norm(); return self.t[self.i][2]
    def next(self):
        self.norm(); tk = self.t[self.i]; self.i += 1; return tk
    def err(self, msg):
        raise GdSyntax('%s:%d: %s (token %s %r)' % (self.f, self.line(), msg, self.kind(), self.val()))
    def is_op(self, v, k=0):
        tk = self.peek(k); return tk[0] == 'OP' and tk[1] == v
    def is_kw(self, v, k=0):
        tk = self.peek(k); return tk[0] == 'ID' and tk[1] == v
    def accept_op(self, v):
        if self.is_op(v): self.i += 1; return True
        return False
    def accept_kw(self, v):
        if self.is_kw(v): self.i += 1; return True
        return False
    def expect_op(self, v):
        if not self.accept_op(v): self.err('očekáván %r' % v)
    def expect_id(self):
        if self.kind() != 'ID': self.err('očekáván identifikátor')
        return self.next()[1]
    def skip_nl(self):
        while self.kind() == 'NEWLINE': self.i += 1

    # ---------- třída ----------
    def parse_class_body(self, name, base=None):
        cls = {'name': name, 'extends': base, 'members': [], 'class_name': None}
        while self.kind() not in ('EOF', 'DEDENT'):
            self.skip_nl()
            if self.kind() in ('EOF', 'DEDENT'): break
            self.parse_member(cls)
        return cls

    def parse_type(self):
        # Typ: Name, Name[Inner], Name.Sub
        if self.kind() != 'ID': self.err('očekáván typ')
        t = self.next()[1]
        while self.is_op('.') and self.peek(1)[0] == 'ID':
            self.next(); t += '.' + self.next()[1]
        if self.is_op('['):
            self.next(); depth = 1
            inner = []
            while depth:
                tk = self.next()
                if tk[0] == 'OP' and tk[1] == '[': depth += 1
                elif tk[0] == 'OP' and tk[1] == ']': depth -= 1
                if depth: inner.append(str(tk[1]))
            t += '[' + ''.join(inner) + ']'
        return t

    def parse_member(self, cls):
        static = False
        while self.is_op('@'):
            self.next(); self.expect_id()
            if self.accept_op('('):
                depth = 1
                while depth:
                    tk = self.next()
                    if tk[0] == 'OP' and tk[1] == '(': depth += 1
                    elif tk[0] == 'OP' and tk[1] == ')': depth -= 1
            self.skip_nl()
        if self.is_kw('extends'):
            self.next()
            if self.kind() == 'STR': cls['extends'] = ('path', self.next()[1][0])
            else: cls['extends'] = ('name', self.parse_type())
            self.end_stmt(); return
        if self.is_kw('class_name'):
            self.next(); cls['class_name'] = self.expect_id()
            if self.accept_kw('extends'):
                cls['extends'] = ('name', self.parse_type())
            self.end_stmt(); return
        if self.is_kw('static'):
            self.next(); static = True
        if self.is_kw('func'):
            fn = self.parse_func(static); cls['members'].append(('func', fn)); return
        if self.is_kw('class'):
            self.next(); nm = self.expect_id(); base = None
            if self.accept_kw('extends'):
                if self.kind() == 'STR': base = ('path', self.next()[1][0])
                else: base = ('name', self.parse_type())
            self.expect_op(':'); self.skip_nl()
            if self.kind() != 'INDENT': self.err('očekáváno odsazení třídy')
            self.next()
            inner = self.parse_class_body(nm, base)
            if self.kind() == 'DEDENT': self.next()
            cls['members'].append(('class', inner)); return
        if self.is_kw('var') or self.is_kw('const'):
            st = self.parse_var(static)
            if st[0] == 'var' and self.kind() == 'NEWLINE' and self.peek(1)[0] == 'INDENT':
                # vlastnost s get/set bloky
                self.next(); self.next(); getter = None; setter = None
                while self.kind() not in ('DEDENT', 'EOF'):
                    self.skip_nl()
                    nm = self.expect_id()
                    if nm == 'get':
                        self.expect_op(':'); getter = self.parse_block()
                    elif nm == 'set':
                        sp = self.parse_params() if self.is_op('(') else [('value', None, None)]
                        self.expect_op(':'); setter = (sp, self.parse_block())
                    else: self.err('neznámý blok vlastnosti')
                    self.skip_nl()
                if self.kind() == 'DEDENT': self.next()
                cls['members'].append(('prop', st, getter, setter)); return
            cls['members'].append(('var' if st[0] == 'var' else 'const', st)); self.end_stmt(); return
        if self.is_kw('signal'):
            self.next(); nm = self.expect_id()
            if self.accept_op('('):
                depth = 1
                while depth:
                    tk = self.next()
                    if tk[0] == 'OP' and tk[1] == '(': depth += 1
                    elif tk[0] == 'OP' and tk[1] == ')': depth -= 1
            cls['members'].append(('signal', nm)); self.end_stmt(); return
        if self.is_kw('enum'):
            self.next(); nm = None
            if self.kind() == 'ID': nm = self.next()[1]
            self.expect_op('{'); vals = []; cur = 0
            while not self.is_op('}'):
                k = self.expect_id()
                if self.accept_op('='):
                    e = self.expr(); cur = e[1] if e[0] == 'num' else cur
                vals.append((k, cur)); cur += 1
                if not self.accept_op(','): break
            self.expect_op('}')
            cls['members'].append(('enum', nm, vals)); self.end_stmt(); return
        self.err('neznámý člen třídy')

    def end_stmt(self):
        if self.accept_op(';'): return
        if self.kind() == 'NEWLINE': self.next(); return
        if self.kind() in ('DEDENT', 'EOF'): return
        self.err('očekáván konec příkazu')

    def parse_params(self):
        params = []
        self.expect_op('(')
        while not self.is_op(')'):
            nm = self.expect_id(); typ = None; default = None
            if self.accept_op(':'):
                if self.is_op('='): pass   # ': =' inference
                else: typ = self.parse_type()
            if self.accept_op(':='): default = self.expr()
            elif self.accept_op('='): default = self.expr()
            params.append((nm, typ, default))
            if not self.accept_op(','): break
        self.expect_op(')')
        return params

    def parse_func(self, static=False):
        self.next()  # func
        name = self.expect_id()
        params = self.parse_params()
        ret = None
        if self.accept_op('->'): ret = self.parse_type()
        self.expect_op(':')
        body = self.parse_block()
        return {'name': name, 'params': params, 'ret': ret, 'body': body, 'static': static}

    def parse_var(self, static=False):
        kw = self.next()[1]
        name = self.expect_id(); typ = None; init = None; infer = False
        if self.accept_op(':='): infer = True; init = self.expr()
        else:
            if self.accept_op(':'):
                if self.is_op('='):
                    self.next(); infer = True; init = self.expr()
                elif self.kind() == 'NEWLINE':
                    pass
                else:
                    typ = self.parse_type()
                    if self.accept_op('='): init = self.expr()
            elif self.accept_op('='): init = self.expr()
        # setter/getter bloky ('var x: set = ...') se nepoužívají
        return ('var' if kw == 'var' else 'const', name, typ, init, infer, static)

    # ---------- bloky ----------
    def parse_block(self):
        """po ':' – buď inline příkazy, nebo odsazený blok"""
        if self.kind() == 'NEWLINE':
            self.skip_nl()
            if self.kind() != 'INDENT': self.err('očekáván odsazený blok')
            self.next()
            stmts = []
            while self.kind() not in ('DEDENT', 'EOF'):
                self.skip_nl()
                if self.kind() in ('DEDENT', 'EOF'): break
                stmts.extend(self.parse_stmt_line())
            if self.kind() == 'DEDENT': self.next()
            return stmts
        # inline
        stmts = []
        stmts.extend(self.parse_simple_list())
        return stmts

    def parse_simple_list(self):
        out = [self.parse_simple()]
        while self.accept_op(';'):
            if self.kind() in ('NEWLINE', 'DEDENT', 'EOF'): break
            out.append(self.parse_simple())
        if self.kind() == 'NEWLINE': self.next()
        return out

    def parse_stmt_line(self):
        tk = self.peek()
        if tk[0] == 'ID':
            v = tk[1]
            if v == 'if': return [self.parse_if()]
            if v == 'for': return [self.parse_for()]
            if v == 'while': return [self.parse_while()]
            if v == 'match': return [self.parse_match()]
        return self.parse_simple_list()

    def parse_if(self):
        self.next()
        branches = []
        cond = self.expr(); self.expect_op(':')
        body = self.parse_block()
        branches.append((cond, body)); els = None
        while True:
            self.skip_nl()
            if self.is_kw('elif'):
                self.next(); c = self.expr(); self.expect_op(':'); b = self.parse_block(); branches.append((c, b))
            elif self.is_kw('else'):
                self.next(); self.expect_op(':'); els = self.parse_block(); break
            else: break
        return ('if', branches, els)

    def parse_for(self):
        self.next(); nm = self.expect_id()
        if self.accept_op(':'): self.parse_type()
        if not self.accept_kw('in'): self.err('očekáváno in')
        it = self.expr(); self.expect_op(':')
        return ('for', nm, it, self.parse_block())

    def parse_while(self):
        self.next(); c = self.expr(); self.expect_op(':')
        return ('while', c, self.parse_block())

    def parse_match(self):
        self.next(); e = self.expr(); self.expect_op(':'); self.skip_nl()
        if self.kind() != 'INDENT': self.err('očekáván blok match')
        self.next(); cases = []
        while self.kind() not in ('DEDENT', 'EOF'):
            self.skip_nl()
            if self.kind() in ('DEDENT', 'EOF'): break
            pats = []
            while True:
                if self.is_kw('var'):
                    self.next(); pats.append(('bind', self.expect_id()))
                elif self.is_op('_') or self.is_kw('_'):
                    self.next(); pats.append(('any',))
                else:
                    pats.append(('val', self.expr()))
                if not self.accept_op(','): break
            self.expect_op(':')
            cases.append((pats, self.parse_block()))
        if self.kind() == 'DEDENT': self.next()
        return ('match', e, cases)

    def parse_simple(self):
        tk = self.peek()
        if tk[0] == 'ID':
            v = tk[1]
            if v == 'if': return self.parse_if()
            if v == 'for': return self.parse_for()
            if v == 'while': return self.parse_while()
            if v == 'match': return self.parse_match()
            if v in ('var', 'const'):
                return self.parse_var()
            if v == 'return':
                self.next()
                if self.kind() in ('NEWLINE', 'DEDENT', 'EOF') or self.is_op(';') or self.is_op(')'):
                    return ('return', None)
                return ('return', self.expr())
            if v == 'break': self.next(); return ('break',)
            if v == 'continue': self.next(); return ('continue',)
            if v == 'pass': self.next(); return ('pass',)
            if v == 'assert':
                self.next(); self.expect_op('(')
                e = self.expr()
                while self.accept_op(','): self.expr()
                self.expect_op(')'); return ('assert', e)
        e = self.expr()
        for op in ('=', '+=', '-=', '*=', '/=', '%=', '|=', '&=', '^=', '<<=', '>>=', '**='):
            if self.is_op(op):
                self.next(); val = self.expr()
                return ('assign', e, op, val)
        return ('expr', e)

    # ---------- výrazy ----------
    def expr(self):
        return self.ternary()

    def ternary(self):
        e = self.or_()
        if self.is_kw('if'):
            self.next(); c = self.or_(); 
            if not self.accept_kw('else'): self.err('očekáváno else')
            f = self.ternary()
            return ('tern', c, e, f)
        return e

    def or_(self):
        e = self.and_()
        while self.is_kw('or') or self.is_op('||'):
            self.next(); r = self.and_(); e = ('or', e, r)
        return e

    def and_(self):
        e = self.not_()
        while self.is_kw('and') or self.is_op('&&'):
            self.next(); r = self.not_(); e = ('and', e, r)
        return e

    def not_(self):
        if self.is_kw('not') or self.is_op('!'):
            self.next(); return ('not', self.not_())
        return self.cmp_()

    def cmp_(self):
        e = self.bor()
        while True:
            tk = self.peek()
            if tk[0] == 'OP' and tk[1] in ('==', '!=', '<', '>', '<=', '>='):
                self.next(); r = self.bor(); e = ('bin', tk[1], e, r)
            elif tk[0] == 'ID' and tk[1] == 'in':
                self.next(); r = self.bor(); e = ('in', e, r, False)
            elif tk[0] == 'ID' and tk[1] == 'not' and self.is_kw('in', 1):
                self.next(); self.next(); r = self.bor(); e = ('in', e, r, True)
            elif tk[0] == 'ID' and tk[1] == 'is':
                self.next()
                neg = False
                if self.is_kw('not'): self.next(); neg = True
                t = self.parse_type(); e = ('is', e, t, neg)
            else: break
        return e

    def bor(self):
        e = self.bxor()
        while self.is_op('|'):
            self.next(); r = self.bxor(); e = ('bin', '|', e, r)
        return e
    def bxor(self):
        e = self.band()
        while self.is_op('^'):
            self.next(); r = self.band(); e = ('bin', '^', e, r)
        return e
    def band(self):
        e = self.shift()
        while self.is_op('&'):
            self.next(); r = self.shift(); e = ('bin', '&', e, r)
        return e
    def shift(self):
        e = self.add()
        while self.is_op('<<') or self.is_op('>>'):
            op = self.next()[1]; r = self.add(); e = ('bin', op, e, r)
        return e
    def add(self):
        e = self.mul()
        while self.is_op('+') or self.is_op('-'):
            op = self.next()[1]; r = self.mul(); e = ('bin', op, e, r)
        return e
    def mul(self):
        e = self.unary()
        while self.is_op('*') or self.is_op('/') or self.is_op('%'):
            op = self.next()[1]; r = self.unary(); e = ('bin', op, e, r)
        return e
    def unary(self):
        if self.is_op('-'):
            self.next(); e = self.unary()
            if e[0] == 'num': return ('num', -e[1], e[2])
            return ('un', '-', e)
        if self.is_op('+'): self.next(); return self.unary()
        if self.is_op('~'): self.next(); return ('un', '~', self.unary())
        return self.power()
    def power(self):
        e = self.as_()
        if self.is_op('**'):
            self.next(); r = self.unary(); e = ('bin', '**', e, r)
        return e
    def as_(self):
        e = self.await_()
        while self.is_kw('as'):
            self.next(); t = self.parse_type(); e = ('cast', e, t)
        return e
    def await_(self):
        if self.is_kw('await'):
            self.next(); return ('await', self.await_())
        return self.postfix()

    def postfix(self):
        e = self.primary()
        while True:
            if self.is_op('.'):
                self.next()
                nm = self.expect_id(); e = ('mem', e, nm)
            elif self.is_op('('):
                self.next(); args = self.call_args(); e = ('call', e, args)
            elif self.is_op('['):
                self.next(); idx = self.expr(); self.expect_op(']'); e = ('idx', e, idx)
            else: break
        return e

    def call_args(self):
        args = []
        while not self.is_op(')'):
            args.append(self.expr())
            if not self.accept_op(','): break
        self.expect_op(')')
        return args

    def primary(self):
        tk = self.peek()
        k, v = tk[0], tk[1]
        if k == 'NUM': self.next(); return ('num', v[0], v[1])
        if k == 'STR': self.next(); return ('str', v[0], v[1])
        if k == 'OP':
            if v == '(':
                self.next(); e = self.expr(); self.expect_op(')'); return ('paren', e)
            if v == '[':
                self.next(); items = []
                while not self.is_op(']'):
                    items.append(self.expr())
                    if not self.accept_op(','): break
                self.expect_op(']'); return ('arr', items)
            if v == '{':
                self.next(); items = []
                while not self.is_op('}'):
                    kx = self.expr()
                    if self.accept_op(':'): vx = self.expr()
                    elif self.accept_op('='):
                        vx = self.expr()
                        if kx[0] == 'name': kx = ('str', kx[1], '')
                    else: self.err('očekáváno : ve slovníku')
                    items.append((kx, vx))
                    if not self.accept_op(','): break
                self.expect_op('}'); return ('dict', items)
            if v == '$' or v == '%':
                self.next()
                if self.kind() == 'STR': path = self.next()[1][0]
                else:
                    path = self.expect_id()
                    while self.is_op('/'):
                        self.next(); path += '/' + self.expect_id()
                return ('getnode', path)
            if v == '.':
                # ".5"
                self.err('neočekávaná tečka')
            self.err('neočekávaný operátor')
        if k == 'ID':
            if v == 'true': self.next(); return ('bool', True)
            if v == 'false': self.next(); return ('bool', False)
            if v == 'null': self.next(); return ('null',)
            if v == 'self': self.next(); return ('self',)
            if v == 'func': return self.parse_lambda()
            if v == 'preload':
                self.next(); self.expect_op('('); s = self.next()[1][0]; self.expect_op(')'); return ('preload', s)
            if v == 'await': self.next(); return ('await', self.postfix())
            self.next(); return ('name', v)
        self.err('neočekávaný token ve výrazu')

    def parse_lambda(self):
        fdepth = self.t[self.i][3]
        self.next()  # func
        if self.kind() == 'ID' and not self.is_op('('): self.next()   # pojmenovaná lambda
        params = self.parse_params()
        ret = None
        if self.accept_op('->'): ret = self.parse_type()
        self.expect_op(':')
        if self.kind() == 'NEWLINE':
            body = self.parse_block()
        else:
            body = []
            while True:
                body.append(self.parse_simple_inline())
                if self.accept_op(';'): continue
                break
        return ('lambda', params, body, ret)

    def parse_simple_inline(self):
        # příkaz v inline lambdě (končí před ')' ',' ']' '}' nebo ';')
        tk = self.peek()
        if tk[0] == 'ID' and tk[1] == 'return':
            self.next()
            if self.is_op(')') or self.is_op(',') or self.is_op(';') or self.kind() in ('NEWLINE', 'DEDENT'):
                return ('return', None)
            return ('return', self.expr())
        e = self.expr()
        for op in ('=', '+=', '-=', '*=', '/=', '%='):
            if self.is_op(op):
                self.next(); val = self.expr(); return ('assign', e, op, val)
        return ('expr', e)


def parse_file(path):
    src = open(path, encoding='utf-8').read()
    toks = lex(src, path)
    p = Parser(toks, path)
    name = path.rsplit('/', 1)[-1].rsplit('.', 1)[0]
    cls = p.parse_class_body(name)
    if p.kind() != 'EOF': p.err('nedočteno do konce souboru')
    return cls

if __name__ == '__main__':
    import glob
    bad = 0
    for f in sorted(sys.argv[1:]):
        try:
            c = parse_file(f)
            print('OK  %-24s členů: %d' % (f.rsplit('/', 1)[-1], len(c['members'])))
        except GdSyntax as e:
            bad += 1; print('ERR', e)
    sys.exit(1 if bad else 0)
