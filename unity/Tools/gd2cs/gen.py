# Generátor C# z AST GDScriptu (gdparse.py). Výstup: třídy S_<soubor> postavené nad běhovou knihovnou Pivovar.Gd (Gv, Gd, GdObject, GdNode).
import sys, os, re, json
sys.path.insert(0, os.path.dirname(__file__))
import gdparse
from gdparse import parse_file, GdSyntax
from unity_patches import PATCHES


def apply_patches(stem, src):
    import unity_patches
    if stem == 'main': src = unity_patches.patch_main(src)
    for old, new in PATCHES.get(stem, []):
        if old not in src:
            raise GdSyntax('patch pro %s nenalezl text: %r' % (stem, old[:60]))
        src = src.replace(old, new, 1)
    return src


gdparse.PATCH_HOOK = apply_patches

CS_KEYWORDS = set('''abstract as base bool break byte case catch char checked class const continue decimal default delegate do double else enum event
explicit extern false finally fixed float for foreach goto if implicit in int interface internal is lock long namespace new null object operator out
override params private protected public readonly ref return sbyte sealed short sizeof stackalloc static string struct switch this throw true try typeof
uint ulong unchecked unsafe ushort using virtual void volatile while Gv Gd GdObject GdNode GdArray GdDict GdFn Gk'''.split())

ENGINE_OBJECT_BASES = {'RefCounted', 'Object', 'Resource'}

def mangle(n):
    return n + '_' if n in CS_KEYWORDS else n

def cs_str(s):
    out = ['"']
    for ch in s:
        o = ord(ch)
        if ch == '\\': out.append('\\\\')
        elif ch == '"': out.append('\\"')
        elif ch == '\n': out.append('\\n')
        elif ch == '\r': out.append('\\r')
        elif ch == '\t': out.append('\\t')
        elif o < 32 or o == 0x7f: out.append('\\u%04x' % o)
        else: out.append(ch)
    out.append('"')
    return ''.join(out)

# globální funkce implementované staticky v Gd (kontroluje se proti Gd*.cs)
def load_gd_statics(rt_dir):
    names = set()
    for fn in os.listdir(rt_dir):
        if fn.startswith('Gd') and fn.endswith('.cs'):
            txt = open(os.path.join(rt_dir, fn), encoding='utf-8').read()
            for m in re.finditer(r'public static Gv (\w+)\(', txt):
                names.add(m.group(1))
    return names

class Scope:
    def __init__(self, parent=None):
        self.parent = parent; self.vars = {}
    def lookup(self, n):
        s = self
        while s:
            if n in s.vars: return s.vars[n]
            s = s.parent
        return None

class FuncCtx:
    def __init__(self, static=False, ret=None):
        self.used = set(); self.static = static; self.ret = ret; self.tmp = 0
    def fresh(self, base):
        b = mangle(base)
        if b not in self.used:
            self.used.add(b); return b
        i = 2
        while '%s_%d' % (b, i) in self.used: i += 1
        n = '%s_%d' % (b, i); self.used.add(n); return n
    def temp(self):
        self.tmp += 1; return '_t%d' % self.tmp

class ClassInfo:
    def __init__(self, cls, cs_name, file_stem, outer=None):
        self.cls = cls; self.cs = cs_name; self.stem = file_stem; self.outer = outer
        self.members = {}   # name -> (kind, static, type)
        self.inner = {}     # name -> ClassInfo
        self.preloads = {}  # const name -> path
        self.base = None    # ClassInfo or None
        self.engine_base = None

class Gen:
    def __init__(self, src_dir, rt_dir):
        self.src_dir = src_dir
        self.gd_statics = load_gd_statics(rt_dir)
        self.files = {}      # stem -> ClassInfo
        self.warn = []
        self.dyn_methods = set()
        self.global_names = set()

    # ---------- příprava ----------
    def load(self, stems):
        for st in stems:
            cls = parse_file(os.path.join(self.src_dir, st + '.gd'))
            ci = ClassInfo(cls, 'S_' + re.sub(r'\W', '_', st), st)
            self.files[st] = ci
        for ci in list(self.files.values()):
            self.index(ci)
        for ci in list(self.files.values()):
            self.link(ci)

    def index(self, ci):
        for m in ci.cls['members']:
            kind = m[0]
            if kind == 'func':
                fn = m[1]; ci.members[fn['name']] = ('func', fn['static'], fn)
            elif kind in ('var', 'const'):
                st = m[1]; name = st[1]
                ci.members[name] = (kind, bool(st[5]) or kind == 'const', st)
                if kind == 'const' and st[3] is not None and st[3][0] == 'preload':
                    ci.preloads[name] = st[3][1]
            elif kind == 'prop':
                st = m[1]; ci.members[st[1]] = ('prop', False, m)
            elif kind == 'signal':
                ci.members[m[1]] = ('signal', False, None)
            elif kind == 'enum':
                nm, vals = m[1], m[2]
                for k, v in vals: ci.members[k] = ('enumval', True, v)
                if nm: ci.members[nm] = ('enum', True, dict(vals))
            elif kind == 'class':
                inner = ClassInfo(m[1], ci.cs + '_' + m[1]['name'], ci.stem, ci)
                ci.inner[m[1]['name']] = inner
                ci.members[m[1]['name']] = ('class', True, inner)
                self.index(inner)

    def link(self, ci):
        ext = ci.cls['extends']
        if ext:
            if ext[0] == 'path':
                stem = ext[1].replace('res://', '').rsplit('.', 1)[0]
                ci.base = self.files.get(stem)
            else:
                nm = ext[1]
                tgt = self.find_class(ci, nm)
                if tgt: ci.base = tgt
                else: ci.engine_base = nm
        for inner in ci.inner.values():
            inner.outer = ci
            self.link(inner)

    def find_class(self, ci, name):
        c = ci
        while c:
            if name in c.inner: return c.inner[name]
            c = c.outer
        # globální class_name
        for f in self.files.values():
            if f.cls.get('class_name') == name: return f
        return None

    def is_node_class(self, ci):
        c = ci
        while c:
            if c.engine_base and c.engine_base not in ENGINE_OBJECT_BASES: return True
            c = c.base
        return False

    def base_cs(self, ci):
        if ci.base: return ci.base.cs
        eb = ci.engine_base
        if eb in ENGINE_OBJECT_BASES or eb is None: return 'GdObject'
        return 'GdNode'

    def lookup_member(self, ci, name):
        c = ci
        while c:
            if name in c.members: return c, c.members[name]
            c = c.base
        return None, None

    def lookup_outer_const(self, ci, name):
        c = ci.outer
        while c:
            if name in c.members and c.members[name][0] in ('const', 'class', 'enum', 'enumval') or (name in c.members and c.members[name][0] == 'func' and c.members[name][1]):
                return c, c.members[name]
            c = c.outer
        return None, None

    # ---------- výstup ----------
    def gen_all(self):
        out = {}
        for stem, ci in self.files.items():
            out[stem] = self.gen_file(ci)
        return out

    def gen_file(self, ci):
        L = []
        L.append('// Vygenerováno z %s.gd překladačem gd2cs.py – needituj ručně.' % ci.stem)
        L.append('using System; using System.Collections.Generic;')
        L.append('namespace Pivovar.Gd.Gen {')
        self.gen_class(ci, L)
        L.append('}')
        return '\n'.join(L) + '\n'

    def gen_class(self, ci, L):
        cls = ci.cls
        for inner in ci.inner.values():
            self.gen_class(inner, L)
        base = self.base_cs(ci)
        L.append('public partial class %s : %s {' % (ci.cs, base))
        self.cur = ci
        members = cls['members']
        # pole
        statics_init = []; inst_init = []
        for m in members:
            kind = m[0]
            if kind == 'var':
                st = m[1]; name = mangle(st[1]); static = st[5]
                L.append('  public %sGv %s;' % ('static ' if static else '', name))
                (statics_init if static else inst_init).append(st)
            elif kind == 'const':
                st = m[1]
                L.append('  public static Gv %s;' % mangle(st[1]))
                statics_init.append(st)
            elif kind == 'signal':
                L.append('  public Gv %s = Gv.O(new GdSignal());' % mangle(m[1]))
            elif kind == 'prop':
                pass
        # statický konstruktor
        L.append('  static %s() {' % ci.cs)
        for st in statics_init:
            fc = FuncCtx(static=True)
            L.append('    ' + self.var_init_stmt(ci, st, fc, member=True, static=True))
        for m in members:
            if m[0] == 'enum':
                pass
        L.append('  }')
        # instanční konstruktor
        L.append('  public %s() {' % ci.cs)
        if ci.engine_base and base == 'GdNode': L.append('    TypeName = "%s";' % ci.engine_base)
        for st in inst_init:
            fc = FuncCtx(static=False)
            L.append('    ' + self.var_init_stmt(ci, st, fc, member=True, static=False))
        L.append('  }')
        # funkce
        for m in members:
            if m[0] == 'func':
                self.gen_func(ci, m[1], L)
            elif m[0] == 'prop':
                st, getter, setter = m[1], m[2], m[3]
                if getter is not None:
                    fn = {'name': 'get_' + st[1], 'params': [], 'ret': st[2], 'body': getter, 'static': False}
                    self.gen_func(ci, fn, L)
                if setter is not None:
                    fn = {'name': 'set_' + st[1], 'params': setter[0], 'ret': None, 'body': setter[1], 'static': False}
                    self.gen_func(ci, fn, L)
        # dispatch
        self.gen_dispatch(ci, L)
        L.append('}')

    def default_for(self, typ):
        t = typ or ''
        if t == 'float': return 'Gv.F(0.0)'
        if t == 'int': return 'Gv.I(0)'
        if t == 'bool': return 'Gv.B(false)'
        if t == 'String': return 'Gv.S("")'
        if t == 'Array' or t.startswith('Array[') or t.startswith('Packed'): return 'Gd.NewArray()'
        if t == 'Dictionary': return 'Gd.NewDict()'
        if t == 'Vector2': return 'Gv.Vec(0, 0)'
        if t == 'Color': return 'Gv.Col(0, 0, 0, 1)'
        return 'Gv.Null'

    def conv(self, typ, expr):
        if typ == 'float': return '(%s).AsF()' % expr
        if typ == 'int': return '(%s).AsI()' % expr
        return expr

    def var_init_stmt(self, ci, st, fc, member=False, static=False):
        kind, name, typ, init, infer, stat = st
        name_cs = mangle(name)
        if init is None:
            val = self.default_for(typ)
        else:
            sc = Scope()
            val = self.conv(typ, self.ex(init, ci, sc, fc))
        return '%s = %s;' % (name_cs, val)

    # ---------- funkce ----------
    def gen_func(self, ci, fn, L):
        fc = FuncCtx(static=fn['static'], ret=fn['ret'])
        sc = Scope()
        params_cs = []; pre = []
        for (pn, pt, pd) in fn['params']:
            cs = fc.fresh(pn); sc.vars[pn] = (cs, pt)
            params_cs.append('Gv %s = default(Gv)' % cs)
            if pd is not None:
                pre.append('if (%s.k == 0) %s = %s;' % (cs, cs, self.conv(pt, self.ex(pd, ci, sc, fc))))
            elif pt in ('float', 'int'):
                pre.append('%s = %s;' % (cs, self.conv(pt, cs)))
        # povinné parametry nemají default; C# vyžaduje, aby volitelné následovaly povinné -> všechny volitelné (zjednodušení)
        name = mangle(fn['name'])
        mods = 'public static ' if fn['static'] else 'public '
        L.append('  %sGv %s(%s) {' % (mods, name, ', '.join(params_cs)))
        for p in pre: L.append('    ' + p)
        body = self.block(fn['body'], ci, sc, fc, 2)
        L.extend(body)
        L.append('    return Gv.Null;')
        L.append('  }')

    # ---------- dispatch ----------
    def gen_dispatch(self, ci, L):
        cls = ci.cls
        has_base_obj = True
        # Get
        L.append('  public override Gv Get(string n) { switch (n) {')
        for m in cls['members']:
            if m[0] == 'var':
                st = m[1]; L.append('    case %s: return %s;' % (cs_str(st[1]), ('%s.%s' % (ci.cs, mangle(st[1]))) if st[5] else 'this.' + mangle(st[1])))
            elif m[0] == 'const':
                L.append('    case %s: return %s.%s;' % (cs_str(m[1][1]), ci.cs, mangle(m[1][1])))
            elif m[0] == 'signal':
                L.append('    case %s: return this.%s;' % (cs_str(m[1]), mangle(m[1])))
            elif m[0] == 'prop':
                if m[2] is not None: L.append('    case %s: return get_%s();' % (cs_str(m[1][1]), mangle(m[1][1])))
        L.append('    default: return base.Get(n); } }')
        L.append('  public override Gv Set(string n, Gv v) { switch (n) {')
        for m in cls['members']:
            if m[0] == 'var':
                st = m[1]
                if st[5]: L.append('    case %s: %s.%s = v; return v;' % (cs_str(st[1]), ci.cs, mangle(st[1])))
                else: L.append('    case %s: this.%s = v; return v;' % (cs_str(st[1]), mangle(st[1])))
            elif m[0] == 'prop':
                if m[3] is not None: L.append('    case %s: set_%s(v); return v;' % (cs_str(m[1][1]), mangle(m[1][1])))
        L.append('    default: return base.Set(n, v); } }')
        L.append('  public override Gv Invoke(string n, Gv[] a) { switch (n) {')
        for m in cls['members']:
            if m[0] == 'func':
                fn = m[1]
                np = len(fn['params'])
                args = ', '.join('Gd.A(a, %d)' % i for i in range(np))
                call = ('%s.%s(%s)' % (ci.cs, mangle(fn['name']), args)) if fn['static'] else ('%s(%s)' % (mangle(fn['name']), args))
                L.append('    case %s: return %s;' % (cs_str(fn['name']), call))
        L.append('    default: return base.Invoke(n, a); } }')
        # statické
        L.append('  public static Gv StaticGet(string n) { switch (n) {')
        for m in cls['members']:
            if m[0] == 'var' and m[1][5]: L.append('    case %s: return %s;' % (cs_str(m[1][1]), mangle(m[1][1])))
            elif m[0] == 'const': L.append('    case %s: return %s;' % (cs_str(m[1][1]), mangle(m[1][1])))
            elif m[0] == 'enum':
                for k, v in m[2]: L.append('    case %s: return Gv.I(%d);' % (cs_str(k), v))
            elif m[0] == 'class' and m[1].get('name') in ci.inner:
                L.append('    case %s: return Gd.Script(%s);' % (cs_str(m[1]['name']), cs_str(ci.inner[m[1]['name']].cs)))
        L.append('    default: return Gv.Null; } }')
        L.append('  public static Gv StaticCall(string n, Gv[] a) { switch (n) {')
        for m in cls['members']:
            if m[0] == 'func' and m[1]['static']:
                fn = m[1]; np = len(fn['params'])
                args = ', '.join('Gd.A(a, %d)' % i for i in range(np))
                L.append('    case %s: return %s(%s);' % (cs_str(fn['name']), mangle(fn['name']), args))
        L.append('    case "new": return New(a);')
        L.append('    default: return Gv.Null; } }')
        init = None
        for m in cls['members']:
            if m[0] == 'func' and m[1]['name'] == '_init': init = m[1]
        L.append('  public static Gv New(params Gv[] a) { var o = new %s(); ' % ci.cs + ('o._init(%s); ' % ', '.join('Gd.A(a, %d)' % i for i in range(len(init['params']))) if init else '') + 'return Gv.O(o); }')
        L.append('  public override string ScriptName { get { return %s; } }' % cs_str(ci.stem if ci.outer is None else ci.cs))

    # ---------- bloky a příkazy ----------
    def block(self, stmts, ci, sc, fc, ind):
        out = []
        for s in stmts:
            out.extend(self.stmt(s, ci, sc, fc, ind))
        return out

    def pad(self, ind): return '  ' * ind

    def stmt(self, s, ci, parent_sc, fc, ind):
        P = self.pad(ind); k = s[0]
        sc = parent_sc
        if k == 'var' or k == 'const':
            _, name, typ, init, infer, stat = s
            cs = fc.fresh(name)
            if init is None: val = self.default_for(typ)
            else: val = self.conv(typ, self.ex(init, ci, sc, fc))
            sc.vars[name] = (cs, typ)
            return [P + 'Gv %s = %s;' % (cs, val)]
        if k == 'expr':
            e = s[1]
            if e[0] != 'call': return [P + '_ = ' + self.ex(e, ci, sc, fc, stmt=True) + ';']
            return [P + self.ex(e, ci, sc, fc, stmt=True) + ';']
        if k == 'assign':
            return [P + self.assign(s, ci, sc, fc)]
        if k == 'return':
            if s[1] is None: return [P + 'return Gv.Null;']
            return [P + 'return %s;' % self.conv(fc.ret, self.ex(s[1], ci, sc, fc))]
        if k == 'pass': return [P + ';']
        if k == 'break': return [P + 'break;']
        if k == 'continue': return [P + 'continue;']
        if k == 'assert': return [P + ';']
        if k == 'if':
            out = []
            first = True
            for cond, body in s[1]:
                c = self.bx(cond, ci, sc, fc)
                out.append(P + ('if (%s) {' if first else '} else if (%s) {') % c)
                inner = Scope(sc)
                out.extend(self.block(body, ci, inner, fc, ind + 1))
                first = False
            if s[2] is not None:
                out.append(P + '} else {')
                out.extend(self.block(s[2], ci, Scope(sc), fc, ind + 1))
            out.append(P + '}')
            return out
        if k == 'while':
            out = [P + 'while (%s) {' % self.bx(s[1], ci, sc, fc)]
            out.extend(self.block(s[2], ci, Scope(sc), fc, ind + 1))
            out.append(P + '}')
            return out
        if k == 'for':
            _, name, it, body = s
            inner = Scope(sc)
            cs = fc.fresh(name); inner.vars[name] = (cs, None)
            if it[0] == 'call' and it[1][0] == 'name' and it[1][1] == 'range' and parent_sc.lookup('range') is None:
                itx = 'Gd.RangeIter(%s)' % ', '.join(self.ex(a, ci, sc, fc) for a in it[2])
            else:
                itx = '(%s).Iter()' % self.ex(it, ci, sc, fc)
            blk = self.block(body, ci, inner, fc, ind + 1)
            assigned = re.search(r'(?<![\w.])%s\s*([-+*/%%]|<<|>>|\||&|\^)?=(?!=)' % re.escape(cs), '\n'.join(blk)) is not None
            if assigned:
                out = [P + 'foreach (Gv %s__it in %s) {' % (cs, itx), P + '  Gv %s = %s__it;' % (cs, cs)]
            else:
                out = [P + 'foreach (Gv %s in %s) {' % (cs, itx)]
            out.extend(blk)
            out.append(P + '}')
            return out
        if k == 'match':
            _, e, cases = s
            tmp = fc.temp()
            out = [P + '{', P + '  Gv %s = %s;' % (tmp, self.ex(e, ci, sc, fc))]
            first = True
            for pats, body in cases:
                conds = []; binds = []
                anyp = False
                for p in pats:
                    if p[0] == 'any': anyp = True
                    elif p[0] == 'bind': anyp = True; binds.append(p[1])
                    else: conds.append('Gv.Eq(%s, %s)' % (tmp, self.ex(p[1], ci, sc, fc)))
                cond = 'true' if anyp else ' || '.join(conds)
                out.append(P + '  ' + ('if (%s) {' if first else 'else if (%s) {') % cond)
                inner = Scope(sc)
                for b in binds:
                    cs = fc.fresh(b); inner.vars[b] = (cs, None)
                    out.append(P + '    Gv %s = %s;' % (cs, tmp))
                out.extend(self.block(body, ci, inner, fc, ind + 2))
                out.append(P + '  }')
                first = False
            out.append(P + '}')
            return out
        raise GdSyntax('neznámý příkaz %r' % (k,))

    # ---------- přiřazení ----------
    def assign(self, s, ci, sc, fc):
        _, tgt, op, val = s
        v = self.ex(val, ci, sc, fc)
        if op != '=':
            bop = op[:-1]
            cur = self.ex(tgt, ci, sc, fc)
            v = self.binop(bop, cur, v)
        return self.store(tgt, v, ci, sc, fc)

    def binop(self, op, a, b):
        m = {'+': '+', '-': '-', '*': '*', '/': '/', '%': '%'}
        if op in m: return '(%s %s %s)' % (a, op, b)
        if op == '**': return 'Gv.Pow(%s, %s)' % (a, b)
        if op == '|': return 'Gv.BitOr(%s, %s)' % (a, b)
        if op == '&': return 'Gv.BitAnd(%s, %s)' % (a, b)
        if op == '^': return 'Gv.BitXor(%s, %s)' % (a, b)
        if op == '<<': return 'Gv.Shl(%s, %s)' % (a, b)
        if op == '>>': return 'Gv.Shr(%s, %s)' % (a, b)
        raise GdSyntax('operátor ' + op)

    def store(self, tgt, v, ci, sc, fc):
        k = tgt[0]
        if k == 'paren': return self.store(tgt[1], v, ci, sc, fc)
        if k == 'name':
            n = tgt[1]
            loc = sc.lookup(n)
            if loc:
                cs, typ = loc
                return '%s = %s;' % (cs, self.conv(typ, v))
            owner, info = self.lookup_member(ci, n)
            if info is not None:
                kind = info[0]
                if kind == 'var':
                    typ = info[2][2]
                    pre = (owner.cs + '.') if info[1] and owner is not ci else ''
                    return '%s%s = %s;' % (pre, mangle(n), self.conv(typ, v))
                if kind == 'prop':
                    return 'set_%s(%s);' % (mangle(n), v)
            oc, oi = self.lookup_outer_const(ci, n)
            if oi is not None and oi[0] == 'var':
                return '%s.%s = %s;' % (oc.cs, mangle(n), v)
            # vlastnost enginu (např. position, visible, text) na self
            return 'Self.S(%s, %s);' % (cs_str(n), v)
        if k == 'mem':
            obj, nm = tgt[1], tgt[2]
            if obj[0] == 'self':
                owner, info = self.lookup_member(ci, nm)
                if info is not None and info[0] == 'var': return 'this.%s = %s;' % (mangle(nm), v)
                return 'Self.S(%s, %s);' % (cs_str(nm), v)
            oexpr = self.ex(obj, ci, sc, fc)
            if obj[0] in ('name', 'mem', 'idx', 'paren') and self.assignable(obj, ci, sc):
                return self.store(obj, '%s.S(%s, %s)' % (oexpr, cs_str(nm), v), ci, sc, fc)
            return '%s.S(%s, %s);' % (oexpr, cs_str(nm), v)
        if k == 'idx':
            obj, ix = tgt[1], tgt[2]
            oexpr = self.ex(obj, ci, sc, fc)
            iexpr = self.ex(ix, ci, sc, fc)
            if obj[0] in ('name', 'mem', 'idx', 'paren') and self.assignable(obj, ci, sc):
                return self.store(obj, '%s.SI(%s, %s)' % (oexpr, iexpr, v), ci, sc, fc)
            return '%s.SI(%s, %s);' % (oexpr, iexpr, v)
        raise GdSyntax('neplatný cíl přiřazení %r' % (k,))

    def assignable(self, e, ci, sc):
        k = e[0]
        if k == 'paren': return self.assignable(e[1], ci, sc)
        if k == 'name':
            if sc.lookup(e[1]): return True
            owner, info = self.lookup_member(ci, e[1])
            if info is None and e[1][:1].islower() and self.is_node_class(ci): return True
            return info is not None and info[0] in ('var',)
        if k == 'mem': return e[1][0] == 'self' or self.assignable(e[1], ci, sc)
        if k == 'idx': return self.assignable(e[1], ci, sc)
        return False

    # ---------- výrazy ----------
    def bx(self, e, ci, sc, fc):
        """C# bool výraz"""
        k = e[0]
        if k == 'paren': return self.bx(e[1], ci, sc, fc)
        if k == 'and': return '(%s && %s)' % (self.bx(e[1], ci, sc, fc), self.bx(e[2], ci, sc, fc))
        if k == 'or': return '(%s || %s)' % (self.bx(e[1], ci, sc, fc), self.bx(e[2], ci, sc, fc))
        if k == 'not': return '!%s' % self.bx(e[1], ci, sc, fc)
        if k == 'bool': return 'true' if e[1] else 'false'
        if k == 'bin' and e[1] in ('==', '!=', '<', '>', '<=', '>='):
            a = self.ex(e[2], ci, sc, fc); b = self.ex(e[3], ci, sc, fc)
            fn = {'==': 'Eq', '!=': 'Ne', '<': 'Lt', '>': 'Gt', '<=': 'Le', '>=': 'Ge'}[e[1]]
            return 'Gv.%s(%s, %s)' % (fn, a, b)
        if k == 'in':
            r = 'Gv.In(%s, %s)' % (self.ex(e[1], ci, sc, fc), self.ex(e[2], ci, sc, fc))
            return '!' + r if e[3] else r
        if k == 'is':
            r = 'Gv.Is(%s, %s)' % (self.ex(e[1], ci, sc, fc), cs_str(e[2]))
            return '!' + r if e[3] else r
        return '(%s).T()' % self.ex(e, ci, sc, fc)

    def ex(self, e, ci, sc, fc, stmt=False):
        k = e[0]
        if k == 'paren': return '(%s)' % self.ex(e[1], ci, sc, fc)
        if k == 'num':
            v, isf = e[1], e[2]
            if isf:
                if v != v or v in (float('inf'), float('-inf')): return 'Gv.F(double.NaN)'
                r = repr(float(v)); 
                return 'Gv.F(%s)' % r
            return 'Gv.I(%dL)' % v
        if k == 'str':
            if e[2] == '&' or e[2] == '^': return 'Gv.S(%s)' % cs_str(e[1])
            return 'Gv.S(%s)' % cs_str(e[1])
        if k == 'bool': return 'Gv.B(%s)' % ('true' if e[1] else 'false')
        if k == 'null': return 'Gv.Null'
        if k == 'self': return 'Self'
        if k in ('and', 'or', 'not') or (k == 'bin' and e[1] in ('==', '!=', '<', '>', '<=', '>=')) or k in ('in', 'is'):
            return 'Gv.B(%s)' % self.bx(e, ci, sc, fc)
        if k == 'tern':
            return '(%s ? %s : %s)' % (self.bx(e[1], ci, sc, fc), self.ex(e[2], ci, sc, fc), self.ex(e[3], ci, sc, fc))
        if k == 'arr':
            return 'Gd.Arr(%s)' % ', '.join(self.ex(x, ci, sc, fc) for x in e[1])
        if k == 'dict':
            parts = []
            for kx, vx in e[1]:
                parts.append(self.ex(kx, ci, sc, fc)); parts.append(self.ex(vx, ci, sc, fc))
            return 'Gd.Dict(%s)' % ', '.join(parts)
        if k == 'un':
            if e[1] == '-': return '(-%s)' % self.ex(e[2], ci, sc, fc)
            if e[1] == '~': return 'Gv.BitNot(%s)' % self.ex(e[2], ci, sc, fc)
        if k == 'bin':
            return self.binop(e[1], self.ex(e[2], ci, sc, fc), self.ex(e[3], ci, sc, fc))
        if k == 'cast': return self.ex(e[1], ci, sc, fc)
        if k == 'await': return self.ex(e[1], ci, sc, fc)
        if k == 'preload':
            stem = e[1].replace('res://', '').rsplit('.', 1)[0]
            if e[1].endswith('.gd') and stem in self.files:
                return 'Gd.Script(%s)' % cs_str(stem)
            return 'Gd.Load(%s)' % cs_str(e[1])
        if k == 'getnode': return 'Self.C("get_node", Gv.S(%s))' % cs_str(e[1])
        if k == 'lambda': return self.lambda_(e, ci, sc, fc)
        if k == 'name': return self.name_expr(e[1], ci, sc, fc)
        if k == 'mem': return self.mem_expr(e, ci, sc, fc)
        if k == 'idx':
            return '%s.I(%s)' % (self.ex(e[1], ci, sc, fc), self.ex(e[2], ci, sc, fc))
        if k == 'call': return self.call_expr(e, ci, sc, fc)
        raise GdSyntax('neznámý výraz %r' % (k,))

    def lambda_(self, e, ci, sc, fc):
        _, params, body, ret = e
        inner = Scope(sc)
        pre = []
        for i, (pn, pt, pd) in enumerate(params):
            cs = fc.fresh(pn); inner.vars[pn] = (cs, pt)
            pre.append('Gv %s = Gd.A(_a, %d);' % (cs, i))
            if pd is not None:
                pre.append('if (%s.k == 0) %s = %s;' % (cs, cs, self.ex(pd, ci, inner, fc)))
        saved = fc.ret; fc.ret = ret
        stmts = self.block(body, ci, inner, fc, 0)
        fc.ret = saved
        return 'Gd.Fn((_a) => { %s %s return Gv.Null; })' % (' '.join(pre), ' '.join(stmts))

    def name_expr(self, n, ci, sc, fc):
        loc = sc.lookup(n)
        if loc: return loc[0]
        owner, info = self.lookup_member(ci, n)
        if info is not None:
            kind = info[0]
            if kind in ('var', 'const'):
                if owner is not ci and (info[1] or kind == 'const'): return '%s.%s' % (owner.cs, mangle(n))
                return mangle(n)
            if kind == 'prop': return 'get_%s()' % mangle(n)
            if kind == 'signal': return mangle(n)
            if kind == 'func': return 'Gd.Fn((_a) => %s(%s))' % (mangle(n), ', '.join('Gd.A(_a, %d)' % i for i in range(len(info[2]['params']))))
            if kind == 'enumval': return 'Gv.I(%d)' % info[2]
            if kind == 'class': return 'Gd.Script(%s)' % cs_str(info[2].cs)
            if kind == 'enum': return 'Gd.Null'
        oc, oi = self.lookup_outer_const(ci, n)
        if oi is not None:
            if oi[0] in ('const', 'var'): return '%s.%s' % (oc.cs, mangle(n))
            if oi[0] == 'enumval': return 'Gv.I(%d)' % oi[2]
            if oi[0] == 'class': return 'Gd.Script(%s)' % cs_str(oi[2].cs)
        if n in self.files or self.find_class(ci, n):
            return 'Gd.Script(%s)' % cs_str(n)
        if n[:1].islower() and not n.isupper() and self.is_node_class(ci):
            return 'Self.G(%s)' % cs_str(n)    # vlastnost zděděné třídy enginu (position, size, text …)
        self.global_names.add(n)
        return 'Gd.K(%s)' % cs_str(n)

    def script_ref(self, e, ci, sc):
        """pokud výraz označuje skript/třídu známou při překladu, vrátí ClassInfo"""
        if e[0] != 'name': return None
        n = e[1]
        if sc.lookup(n): return None
        owner, info = self.lookup_member(ci, n)
        if info is not None:
            if info[0] == 'const' and n in owner.preloads:
                stem = owner.preloads[n].replace('res://', '').rsplit('.', 1)[0]
                return self.files.get(stem)
            if info[0] == 'class': return info[2]
            return None
        c = ci
        while c.outer:
            c = c.outer
            if n in c.preloads:
                stem = c.preloads[n].replace('res://', '').rsplit('.', 1)[0]
                return self.files.get(stem)
            if n in c.inner: return c.inner[n]
        for st, f in self.files.items():
            if f.cls.get('class_name') == n: return f
        return None

    def mem_expr(self, e, ci, sc, fc):
        obj, nm = e[1], e[2]
        if obj[0] == 'self':
            owner, info = self.lookup_member(ci, nm)
            if info is not None and info[0] in ('var', 'const', 'signal'): return 'this.' + mangle(nm)
            return 'Self.G(%s)' % cs_str(nm)
        tgt = self.script_ref(obj, ci, sc)
        if tgt is not None:
            _, info = self.lookup_member(tgt, nm)
            if info is not None and info[0] in ('const', 'var'):
                return '%s.%s' % (self.owner_of(tgt, nm).cs, mangle(nm))
            if info is not None and info[0] == 'enumval': return 'Gv.I(%d)' % info[2]
            return '%s.StaticGet(%s)' % (tgt.cs, cs_str(nm))
        if obj[0] == 'name' and not sc.lookup(obj[1]) and self.lookup_member(ci, obj[1])[1] is None and self.lookup_outer_const(ci, obj[1])[1] is None and obj[1][:1].isupper():
            # engine konstanta / singleton: Vector2.ZERO, Control.PRESET_FULL_RECT
            return 'Gd.StaticGet(%s, %s)' % (cs_str(obj[1]), cs_str(nm))
        return '%s.G(%s)' % (self.ex(obj, ci, sc, fc), cs_str(nm))

    def owner_of(self, ci, name):
        o, _ = self.lookup_member(ci, name); return o

    def call_expr(self, e, ci, sc, fc):
        fn, args = e[1], e[2]
        a = [self.ex(x, ci, sc, fc) for x in args]
        if fn[0] == 'name':
            n = fn[1]
            if sc.lookup(n):
                return '%s.Call(%s)' % (sc.lookup(n)[0], ', '.join(a))   # callable ve lokální proměnné (nepřímé volání)
            owner, info = self.lookup_member(ci, n)
            if info is not None and info[0] == 'func':
                st = info[1]
                pre = (owner.cs + '.') if (st and owner is not ci) else ''
                return '%s%s(%s)' % (pre, mangle(n), ', '.join(a))
            if info is not None and info[0] in ('var', 'const', 'prop'):
                return '%s.Call(%s)' % (self.name_expr(n, ci, sc, fc), ', '.join(a))
            oc, oi = self.lookup_outer_const(ci, n)
            if oi is not None and oi[0] == 'func': return '%s.%s(%s)' % (oc.cs, mangle(n), ', '.join(a))
            if n in self.gd_statics:
                return 'Gd.%s(%s)' % (n, ', '.join(a))
            if n[:1].isupper():
                return 'Gd.Ctor(%s%s)' % (cs_str(n), ''.join(', ' + x for x in a))
            if info is None and self.is_engine_method(n):
                return 'Self.C(%s%s)' % (cs_str(n), ''.join(', ' + x for x in a))
            self.global_names.add(n)
            return 'Gd.Dyn(%s%s)' % (cs_str(n), ''.join(', ' + x for x in a))
        if fn[0] == 'mem':
            obj, nm = fn[1], fn[2]
            if obj[0] == 'self':
                owner, info = self.lookup_member(ci, nm)
                if info is not None and info[0] == 'func':
                    pre = (owner.cs + '.') if (info[1] and owner is not ci) else ''
                    return '%s%s(%s)' % (pre, mangle(nm), ', '.join(a))
                return 'Self.C(%s%s)' % (cs_str(nm), ''.join(', ' + x for x in a))
            tgt = self.script_ref(obj, ci, sc)
            if tgt is not None:
                if nm == 'new':
                    return '%s.New(%s)' % (tgt.cs, ', '.join(a))
                owner, info = self.lookup_member(tgt, nm)
                if info is not None and info[0] == 'func' and info[1]:
                    return '%s.%s(%s)' % (owner.cs, mangle(nm), ', '.join(a))
                return '%s.StaticCall(%s%s)' % (tgt.cs, cs_str(nm), ''.join(', ' + x for x in a))
            if obj[0] == 'name' and not sc.lookup(obj[1]) and self.lookup_member(ci, obj[1])[1] is None and self.lookup_outer_const(ci, obj[1])[1] is None and obj[1][:1].isupper():
                if nm == 'new':
                    return 'Gd.New(%s%s)' % (cs_str(obj[1]), ''.join(', ' + x for x in a))
                return 'Gd.Static(%s, %s%s)' % (cs_str(obj[1]), cs_str(nm), ''.join(', ' + x for x in a))
            self.dyn_methods.add(nm)
            return '%s.C(%s%s)' % (self.ex(obj, ci, sc, fc), cs_str(nm), ''.join(', ' + x for x in a))
        # volání výrazu (např. lambda přímo)
        return '(%s).Call(%s)' % (self.ex(fn, ci, sc, fc), ', '.join(a))

    ENGINE_METHODS = set('''add_child remove_child queue_free queue_redraw get_tree get_viewport get_node get_parent get_children get_child_count get_window
      draw_line draw_rect draw_circle draw_polygon draw_colored_polygon draw_polyline draw_arc draw_string draw_texture draw_texture_rect draw_set_transform
      draw_style_box draw_multiline draw_multiline_colors draw_primitive draw_string_outline draw_polyline_colors draw_ellipse draw_set_transform_matrix draw_texture_rect_region
      connect emit_signal set_process set_process_input set_process_unhandled_input is_inside_tree get_global_mouse_position get_local_mouse_position
      add_theme_stylebox_override add_theme_color_override add_theme_font_size_override add_theme_font_override add_theme_constant_override
      set_anchors_preset grab_focus release_focus accept_event set_meta get_meta has_meta call_deferred get_size get_rect show hide move_child
      set_anchor set_offset get_global_rect set_size set_position get_theme_default_font get_canvas_transform get_global_transform queue_sort set_physics_process
      create_timer create_tween play stop make_current set_deferred is_node_ready'''.split())

    def is_engine_method(self, n):
        return n in self.ENGINE_METHODS or n.startswith('draw_') or n.startswith('add_theme') or n.startswith('get_') or n.startswith('set_') or n.startswith('is_')


def main():
    src_dir = sys.argv[1]; out_dir = sys.argv[2]; rt_dir = sys.argv[3]
    stems = sys.argv[4:]
    g = Gen(src_dir, rt_dir)
    g.load(stems)
    files = g.gen_all()
    os.makedirs(out_dir, exist_ok=True)
    for stem, text in files.items():
        open(os.path.join(out_dir, 'S_%s.cs' % stem), 'w', encoding='utf-8').write(text)
    # registr skriptů
    L = ['// Vygenerováno – registr skriptů', 'using System; using System.Collections.Generic;', 'namespace Pivovar.Gd.Gen {', 'public static class GdRegistry {', '  public static void Init() {']
    def reg(ci):
        for inner in ci.inner.values(): reg(inner)
        key = ci.stem if ci.outer is None else ci.cs
        L.append('    Gd.Register(%s, %s.New, %s.StaticGet, %s.StaticCall);' % (cs_str(key), ci.cs, ci.cs, ci.cs))
    for ci in g.files.values(): reg(ci)
    L += ['  }', '}', '}']
    open(os.path.join(out_dir, 'GdRegistry.cs'), 'w', encoding='utf-8').write('\n'.join(L) + '\n')
    print('hotovo: %d souborů' % len(files))
    print('globální jména:', sorted(g.global_names))
    print('dyn metody:', len(g.dyn_methods))
    json.dump(sorted(g.dyn_methods), open(os.path.join(out_dir, '_dyn_methods.json'), 'w'))

if __name__ == '__main__':
    main()
