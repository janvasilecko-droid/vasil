#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
FYZIKA+ – vložení balíčku do projektu trenažéru varny (Godot).

Použití (Windows i Linux, stačí Python 3.8+ bez dalších knihoven):
    python aplikuj_fyzika_plus.py "D:\\stazene\\Trenazer-varny-aplikace"            # vloží všechno
    python aplikuj_fyzika_plus.py "D:\\...\\projekt" --zkouska                       # jen ukáže, co by udělal
    python aplikuj_fyzika_plus.py "D:\\...\\projekt" --bez-ui                        # bez tlačítka kyseliny a tlaku pláště v panelu

Co dělá:
  * zkopíruje fyzika_plus.gd, fyzika_plus_tests.gd a fyzika_plus_vzhled.gd do složky projektu (kde je sim.gd),
  * do sim.gd / params.gd / tests.gd / audit_tests.gd / ctl.gd / main.gd / auto.gd / lauter_auto.gd vloží krátké háčky – každý je hledaný podle
    textu (ne podle čísla řádku), takže snese i jinak upravenou verzi. Háček, jehož místo nenajde přesně jednou,
    PŘESKOČÍ a napíše proč – nic nerozbije napůl (skupina úprav se vloží celá, nebo vůbec),
  * před první změnou uloží zálohu každého souboru jako <soubor>.pred-fyzika-plus,
  * zapíše fyzika_plus_hooks.gd se seznamem vložených háčků (testy podle něj přeskočí to, co chybí),
  * jde spustit znovu: už vložené háčky pozná podle značky „FYZIKA+ Hx“ a nevkládá je podruhé.

Vrácení: přejmenuj zálohy *.pred-fyzika-plus zpět na původní jména a smaž fyzika_plus*.gd.
"""
import os
import re
import shutil
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
MODULE_FILES = ["fyzika_plus.gd", "fyzika_plus_tests.gd", "fyzika_plus_vzhled.gd"]

# nové parametry: (klíč, výchozí hodnota, rozsah, META pro params.gd)
NEW_PARAMS = [
    ("stValveR", 30.0, (1.0, 50.0), ["-", "ESTIMATED", "1–50", "charakteristika parního ventilu: 1 = lineární (ruční ventil), 30–50 = ekviprocentní regulační ventil Kv/Kvs = R^(zdvih − 1) (Spirax Sarco doporučuje pro ohřev párou); výchozí 30 je ODHAD – nastav podle typu ventilu"]),
    ("rvpKvs", 0.0, (0.0, 200.0), ["m³/h", "ESTIMATED", "0 = dopočítat", "Kvs parního ventilu pánve (štítek ventilu); 0 = dopočítat z rvpUAst tak, aby plně otevřený ventil při 60 °C ubral 10 % tlaku"]),
    ("mpKvs", 0.0, (0.0, 200.0), ["m³/h", "ESTIMATED", "0 = dopočítat", "Kvs parního ventilu mladinové pánve"]),
    ("stCondBack", 0.15, (0.0, 2.0), ["bar (přetlak)", "ESTIMATED", "0–0,5", "protitlak kondenzátu za odvaděčem (výtlak do kondenzátní sítě, převýšení); pod ním kondenzát z pláště neodtéká – stall (TLV)"]),
    ("jWaterKg", 150.0, (20.0, 1000.0), ["kg", "ESTIMATED", "±50 %", "kolik kondenzátu se vejde do pláště pánve (objem pláště)"]),
    ("jDrainKgS", 0.6, (0.05, 5.0), ["kg/s", "ESTIMATED", "±50 %", "největší odvod kondenzátu odvaděčem"]),
    ("jAirTau", 20.0, (2.0, 600.0), ["s", "ESTIMATED", "±100 %", "jak rychle odvzdušňovač vytlačí vzduch z pláště (při přetlaku 1 bar); ruční odvzdušnění = delší"]),
    ("jAirInTau", 15.0, (2.0, 600.0), ["s", "ESTIMATED", "±100 %", "jak rychle zavzdušňovací ventil pustí vzduch do pláště při podtlaku"]),
    ("jVacBreaker", 1.0, (0.0, 1.0), ["0/1", "ESTIMATED", "-", "1 = plášť má zavzdušňovací (vakuový) ventil; 0 = nemá – po zavření páry vzniká podtlak (Kunze s. 290–291: odvzdušnit, jinak hrozí zborcení dna)"]),
    ("hNatC", 0.28, (0.05, 1.5), ["-", "LITERATURE (odvozeno)", "0,2–0,4", "přirozená konvekce bez míchadla v měřítku HA_REF: učebnicově h_přir/h_míchané ≈ 0,46 (Nu = 0,10·Ra^(1/3) vs 0,36·Re^(2/3)·Pr^(1/3), voda 60 °C, ΔT 30 K); 0,28 dává stejný poměr celkového prostupu (~0,6), protože model má na straně náplně jen ~26 % odporu místo ~55 %"]),
    ("colK100", 1.2, (0.0, 10.0), ["EBC/h", "LITERATURE (odvozeno)", "1,0–1,35", "tvorba barvy při 100 °C, pH 5,5, 12 °P (bez zahuštění odparem, to se počítá zvlášť); VLB (Meyer, CBC 2015): 5,7 → 8,0 EBC za 90 min varu, z toho 0,4–0,6 EBC zahuštění odparem"]),
    ("colEa", 77.0, (40.0, 150.0), ["kJ/mol", "LITERATURE", "77 (65–100 °C)", "aktivační energie tvorby barvy (Kroh et al., TU Berlín: 77 kJ/mol do 100 °C, 112 nad 100 °C)"]),
    ("colPh", 0.7, (0.0, 3.0), ["1/pH", "ESTIMATED", "0,5–2", "citlivost barvy na pH (Kunze s. 286: nižší pH = méně barvy); velikost ODHAD"]),
    ("colMashK", 1.0, (0.0, 5.0), ["-", "ESTIMATED", "1–2", "tvorba barvy v hustém rmutu (dekokce) proti mladině"]),
    ("phBoilDrop", 0.08, (0.0, 0.4), ["pH/h", "LITERATURE", "0,06–0,1", "pokles pH mladiny za varu: VLB 5,62 → 5,50 za 90 min; Kunze s. 286: 5,5–5,6 → 5,4–5,5"]),
    ("phSparge", 0.05, (-0.2, 0.4), ["pH", "ESTIMATED", "0–0,15", "o kolik má sladina vyšší pH než rmut (vyslazování vodou s alkalitou)"]),
    ("wortBuf", 26.6, (5.0, 80.0), ["mEq/(kg sladu·pH)", "LITERATURE", "±30 %", "pufrační kapacita mladiny: Kunze s. 229 – 30 mL 0,8% kyseliny mléčné na kg sladu sníží pH o 0,1"]),
    ("isoPh", 0.5, (0.0, 2.0), ["-", "ESTIMATED", "0–1", "relativní růst rychlosti izomerace α-kyselin na 1 jednotku pH (Kosař s. 163); velikost ODHAD"]),
    ("wpInletMM", 0.0, (0.0, 150.0), ["mm", "ESTIMATED", "-", "vnitřní průměr tangenciálního nátoku do vířivky; 0 = stejný jako trubka DN50 v plánku (bore50) – změřit hrdlo"]),
    ("wpVmax", 3.5, (1.0, 8.0), ["m/s", "LITERATURE", "3,5", "nejvyšší rychlost nátoku do vířivky, kdy se lom netříští (Kunze s. 341)"]),
    ("wpVmin", 1.0, (0.2, 5.0), ["m/s", "ESTIMATED", "±50 %", "pod touto rychlostí nátoku se vířivka roztáčí slabě (horší kužel); Kunze: často stačí mnohem menší rychlost než 3,5 m/s"]),
    ("wpShearK", 0.1, (0.0, 1.0), ["1/(m/s)", "ESTIMATED", "±100 %", "podíl lomu roztříštěného na 1 m/s nad wpVmax"]),
    ("pipeU", 10.0, (0.5, 30.0), ["W/(m²·K)", "LITERATURE", "1 (izolovaná) – 15 (holá, průvan)", "prostup tepla trubky do okolí; holá nerez ~8 konvekce + ~2 sálání (inzonex: holé nádoby ~15 při 0,5 m/s)"]),
    # --- verze 1.1 (16. 11. 2026) ---
    ("hltKvs", 0.0, (0.0, 200.0), ["m³/h", "ESTIMATED", "0 = dopočítat", "Kvs parního ventilu topného hadu bojleru (štítek ventilu); 0 = dopočítat z hltUAst"]),
    ("lvPumpQ", 120.0, (10.0, 600.0), ["L/min", "ESTIMATED", "±30 %", "průtok čerpadla ledové vody při plně otevřených klapkách (dřív pevně 120 L/min v kódu); přiškrcením klapky na trase se ubírá – změřit"]),
    ("rvpDiaM", 0.0, (0.0, 5.0), ["m", "ESTIMATED", "0 = odhad", "vnitřní průměr rmutomladinové pánve (plocha hladiny a hloubka náplně); 0 = odhad z objemu (výška = průměr) – změřit"]),
    ("mpDiaM", 0.0, (0.0, 5.0), ["m", "ESTIMATED", "0 = odhad", "vnitřní průměr mladinové pánve; 0 = odhad z objemu"]),
    ("wpDiaM", 0.0, (0.0, 6.0), ["m", "ESTIMATED", "0 = odhad", "vnitřní průměr vířivé kádě; 0 = odhad z objemu při průměr : výška mladiny = 3 : 1 (Kunze s. 340–341) – změřit"]),
    ("evapOpen", 0.1, (0.0, 3.0), ["-", "ESTIMATED", "0,05–1", "odpar z hladiny, když nádoba nevře, vůči volné hladině v klidném vzduchu (ASHRAE: při 90 °C ~10 kg/(m²·h)); 1 = otevřený průlez, 0,1 = zavřená nádoba s odtahem (ODHAD – knihy uvádějí jen, že mladina jde na chladič s 95–98 °C)"]),
    ("mashVmax", 1.5, (0.5, 4.0), ["m/s", "LITERATURE", "1,2–1,5", "nejvyšší rychlost rmutu v potrubí do scezovací kádě (Kunze s. 903: pod 1,5 m/s; Kosař s. 140: 1,2–1,5 m/s); nad ní smyk zhoršuje scezování"]),
    # --- verze 1.2 (16. 11. 2026) ---
    ("pumpPass", 1.0, (0.0, 1.0), ["0/1", "STATED", "-", "1 = stojící odstředivé čerpadlo je průchozí: otevřenou trasou teče kapalina samospádem dopředu i zpátky (spojené nádoby), proto se po čerpání zavírají klapky. Na Zajíci za čerpadly zpětné klapky nejsou (Vasil 16. 11. 2026), proto 1; 0 = stojící čerpadlo nic nepustí (dřívější chování, pro varnu se zpětnými klapkami)"]),
    ("pumpPassK", 5.0, (0.0, 50.0), ["-", "LITERATURE", "5–10", "ztráta ve stojícím čerpadle v násobcích rychlostní výšky: ~5 když se oběžné kolo volně protáčí, ~10 když stojí (GIW/KSB, údaj pro kalová čerpadla – pro čerpadla na varně ODHAD)"]),
    ("dryWallK", 0.5, (0.0, 1.0), ["-", "ESTIMATED", "0,4–1", "kolik tepla ztrácí suchá stěna nádoby (plášť nad hladinou a víko) proti smáčené: 1 = stejně (dobře izolovaná nádoba, ztráty nezávisí na hladině – dřívější chování), 0,5 = holá stěna. Podle toho poloprázdná pánev a vířivka ztrácejí míň než plné; rvpUA, mpUA a wpUA platí pro plnou nádobu"]),
]


class Hook:
    def __init__(self, hid, title, edits, optional=False, needs=None, probes=None):
        self.hid = hid
        self.title = title
        self.edits = edits          # list of (file, callable(text, styl) -> (nový text | None, důvod))
        self.optional = optional
        self.needs = needs or []
        self.probes = probes or []  # (soubor, text, min. počet) – podle nich se pozná už vložený háček


def indent_style(text):
    tabs = len(re.findall(r'^\t+\S', text, re.M))
    spaces = len(re.findall(r'^ {4}\S', text, re.M))
    return "\t" if tabs > spaces else "    "


def adapt(snippet, style):
    """Fragmenty jsou psané se 4 mezerami; když soubor používá tabulátory, převede odsazení."""
    if style == "    ":
        return snippet
    out = []
    for ln in snippet.split("\n"):
        m = re.match(r'^( {4})+', ln)
        if m:
            n = len(m.group(0)) // 4
            ln = "\t" * n + ln[len(m.group(0)):]
        out.append(ln)
    return "\n".join(out)


def sub_once(pattern, repl, flags=re.M, comment=None):
    """Vrátí funkci, která nahradí vzor, jen když se v textu najde přesně jednou.
    comment = poznámka „# FYZIKA+ …“; připíše se jen tehdy, když za nahrazeným místem už na řádku nic není
    (jinak by zakomentovala zbytek řádku)."""
    rx = re.compile(pattern, flags)

    def g(text, style):
        hits = list(rx.finditer(text))
        if len(hits) != 1:
            return None, "vzor nalezen %d× (čekám 1×): %s" % (len(hits), pattern[:90])
        h = hits[0]
        r = repl(h, style) if callable(repl) else h.expand(adapt(repl, style))
        rest = text[h.end():]
        nl = rest.find("\n")
        line_rest = rest if nl < 0 else rest[:nl]
        if comment and line_rest.strip() == "" and "#" not in r.split("\n")[-1]:
            r = r + "   " + comment
        return text[:h.start()] + r + text[h.end():], ""
    return g


def need(pattern, why, flags=re.M):
    """Podmínka: vzor musí v souboru být (jinak háček nevkládat) – nic nemění."""
    rx = re.compile(pattern, flags)

    def g(text, style):
        return (text, "") if rx.search(text) else (None, why)
    return g


def done_if(marker, fn):
    """Dílčí úprava, která už v textu je (pozná se podle marker), se bere jako hotová a nevkládá se podruhé –
    např. když někdo vložil půlku háčku ručně podle INTEGRACE.md."""
    def g(text, style):
        if marker in text:
            return text, ""
        return fn(text, style)
    return g


def within(func_name, fn):
    """Použije úpravu jen na tělo funkce func_name (od „func jméno(“ po další „func“), aby se vzor nechytil jinde."""
    rx = re.compile(r'^[ \t]*(?:static )?func ' + re.escape(func_name) + r'\(', re.M)
    nxt = re.compile(r'^[ \t]*(?:static )?func ', re.M)

    def g(text, style):
        ms = list(rx.finditer(text))
        if len(ms) != 1:
            return None, "funkce %s() nalezena %d× (čekám 1×)" % (func_name, len(ms))
        a = ms[0].start()
        nx = nxt.search(text, ms[0].end())
        b = nx.start() if nx else len(text)
        body, why = fn(text[a:b], style)
        if body is None:
            return None, "%s(): %s" % (func_name, why)
        return text[:a] + body + text[b:], ""
    return g


def first_of(*fns):
    def g(text, style):
        why = []
        for fn in fns:
            t, w = fn(text, style)
            if t is not None:
                return t, ""
            why.append(w)
        return None, " | ".join(why)
    return g


def dict_block(text, const_name):
    m = re.search(r'^const ' + const_name + r'\s*:?\s*=\s*\{[^\n]*\n', text, re.M)
    if not m:
        return None
    end = re.search(r'^\}', text[m.end():], re.M)
    if not end:
        return None
    return m.start(), m.end(), m.end() + end.start()


def add_dict_entries(const_name, make_line):
    def g(text, style):
        blk = dict_block(text, const_name)
        if blk is None:
            return None, "nenašel jsem const %s = {" % const_name
        s, body_start, body_end = blk
        body = text[body_start:body_end]
        missing = [p for p in NEW_PARAMS if ('"%s"' % p[0]) not in body]
        if not missing:
            return text, ""
        line = make_line(missing, style)
        return text[:body_start] + line + text[body_start:], ""
    return g


def cfg_line(missing, style):
    ind = "\t" if style == "\t" else "    "
    return ind + ", ".join('"%s": %s' % (k, repr(float(v))) for k, v, _, _ in missing) + ",   # FYZIKA+ H8\n"


def range_line(missing, style):
    ind = "\t" if style == "\t" else "    "
    return ind + ", ".join('"%s": [%s, %s]' % (k, repr(float(r[0])), repr(float(r[1]))) for k, _, r, _ in missing) + ",   # FYZIKA+ H8\n"


def meta_lines(missing, style):
    ind = "\t" if style == "\t" else "    "
    out = ""
    for k, _, _, meta in missing:
        out += ind + '"%s": [%s],   # FYZIKA+ H8\n' % (k, ", ".join('"%s"' % x.replace('"', "'") for x in meta))
    return out


# ---------------------------------------------------------------- háčky
def build_hooks():
    H = []
    # H0 – načtení modulu
    H.append(Hook("H0", "sim.gd: načtení modulu fyzika_plus.gd", probes=[("sim.gd", 'preload("res://fyzika_plus.gd")', 1)], edits=[
        ("sim.gd", first_of(
            sub_once(r'^(const Physics\s*:?=\s*preload\("res://physics\.gd"\)[^\n]*)$', r'\1' + '\nconst FP = preload("res://fyzika_plus.gd")   # FYZIKA+ H0'),
            sub_once(r'^(extends RefCounted[^\n]*)$', r'\1' + '\nconst FP = preload("res://fyzika_plus.gd")   # FYZIKA+ H0'),
        )),
    ]))
    # H1 – pára: ventil + plášť (náhrada _pd) a zrušení dvojího započtení filmu
    def pd_head(m, style):
        v, ua, jl, ts = m.group(1), m.group(2), m.group(4), m.group(6)
        return m.group(0) + "\n" + adapt("    return FP.steam_pd(self, %s, %s, %s, %s)   # FYZIKA+ H1 – ventil, tlak v plášti, vzduch, kondenzát; původní výpočet níže už neběží" % (v, ua, jl, ts), style)
    H.append(Hook("H1", "sim.gd: pára do pláště přes ventil a tlak v plášti (_pd)", probes=[("sim.gd", "FP.steam_pd(self", 1), ("sim.gd", "FYZIKA+ H1: film", 1)], edits=[
        ("sim.gd", done_if("FP.steam_pd(self", sub_once(r'^func _pd\((\w+), (\w+)(: float)?, (\w+)(: float)?, (\w+)(: float)?\)(\s*->\s*Dictionary)?\s*:[ \t]*$', pd_head))),
        ("sim.gd", done_if("FYZIKA+ H1: film", first_of(
            sub_once(r'^([ \t]*)(S\._pd\.rvp\.P \*= film_factor\(rvp\);\s*S\._pd\.mp\.P \*= film_factor\(mp\))[ \t]*$', r'\1#\2   # FYZIKA+ H1: film je už ve FP.steam_pd'),
            sub_once(r'^([ \t]*)(S\._pd\.rvp\.P \*= film_factor\(rvp\))[ \t]*\n([ \t]*)(S\._pd\.mp\.P \*= film_factor\(mp\))[ \t]*$', r'\1#\2   # FYZIKA+ H1: film je už ve FP.steam_pd\n\3#\4   # FYZIKA+ H1'),
            sub_once(r'^([ \t]*)(Pin \*= ff)[ \t]*$', r'\1#\2   # FYZIKA+ H1: film je už ve FP.steam_pd'),     # starší verze násobily filmem v heat()
        ))),
    ], needs=["H0"]))
    # H2 – přestup na straně náplně
    def film_head(m, style):
        return m.group(0) + "\n" + adapt("    return FP.film_hA(self, %s)   # FYZIKA+ H2 – míchání, viskozita, přirozená konvekce; původní výpočet níže už neběží" % m.group(1), style)
    H.append(Hook("H2", "sim.gd: přestup tepla náplň ↔ stěna (film_hA)", probes=[("sim.gd", "FP.film_hA(self", 1)], edits=[
        ("sim.gd", sub_once(r'^func film_hA\((\w+)\)(\s*->\s*float)?\s*:[ \t]*$', film_head)),
    ], needs=["H0"]))
    # H3 – pH mladiny v reakcích (DMS, izomerace)
    H.append(Hook("H3a", "sim.gd: pH mladiny pro SMM → DMS", probes=[("sim.gd", "clampf(FP.vessel_ph(self, v), 5.0, 5.8)", 1)], edits=[
        ("sim.gd", need(r'^func react\(v\b', "funkce react() nemá parametr v")),
        ("sim.gd", sub_once(r'(var ph_d: float = clampf\()float\(S\.get\("pH", 5\.5\)\)(, 5\.0, 5\.8\))', r'\1FP.vessel_ph(self, v)\2', comment="# FYZIKA+ H3a")),
    ], needs=["H0"]))
    def iso_line(text, style):
        lines = text.split("\n")
        idx = [i for i, ln in enumerate(lines) if "isoPh" in ln and 'S.get("pH", 5.5)' in ln and "CFG" not in ln]
        if len(idx) == 1:
            i = idx[0]
            lines[i] = lines[i].replace('float(S.get("pH", 5.5))', 'FP.vessel_ph(self, v)').replace('S.get("pH", 5.5)', 'FP.vessel_ph(self, v)') + "   # FYZIKA+ H3b"
            return "\n".join(lines), ""
        if re.search(r'cfg\.get\("isoPh"', text) or len(idx) > 1:
            return None, "izomerace už má korekci pH (isoPh), ale řádek se nedá jednoznačně najít – vlož ručně (INTEGRACE.md)"
        return sub_once(r'(var k1 = 790000000000\.0 \* exp\(-11858\.0 / TK\))(?! \* FP)', r'\1 * FP.iso_ph_factor(self, v)', comment="# FYZIKA+ H3b")(text, style)
    H.append(Hook("H3b", "sim.gd: pH mladiny pro izomeraci chmele", probes=[("sim.gd", ["FP.iso_ph_factor(self, v)", "FP.vessel_ph(self, v), 4.8, 6.0"], 1)], edits=[("sim.gd", iso_line)], needs=["H0"]))
    # H4 – barva kinetikou + pokles pH za varu
    H.append(Hook("H4", "sim.gd: barva podle teploty, pH a extraktu + pokles pH za varu", probes=[("sim.gd", "FP.color_step(self", 1), ("sim.gd", "FP.boil_step(self", 1)], edits=[
        ("sim.gd", done_if("FP.color_step(self", sub_once(r'^func react\((\w+), (\w+)(: float)?\)(?:\s*->\s*void)?\s*:[ \t]*\n([ \t]+)if \1\.w < 1: return[ \t]*\n', lambda m, st: m.group(0) + m.group(4) + "FP.color_step(self, %s, %s)   # FYZIKA+ H4 – barva (Maillard) ve všech nádobách\n" % (m.group(1), m.group(2))))),
        ("sim.gd", done_if("FP.boil_step(self", sub_once(r'^([ \t]*)v\.col \+= liqVol\(v\) \* 2\.5 / 60\.0 \* m[ \t]*$', r'\1FP.boil_step(self, v, m)   # FYZIKA+ H4 – pH mladiny za varu; barva se počítá ve FP.color_step (dřív: v.col += liqVol(v) * 2.5 / 60.0 * m)'))),
    ], needs=["H0"]))
    # H5 – vířivka
    H.append(Hook("H5", "sim.gd: rychlost nátoku do vířivky a tříštění lomu", probes=[("sim.gd", "FP.whirl_inlet(self", 1), ("sim.gd", "FP.trub_ns(self, v)", 1)], edits=[
        ("sim.gd", done_if("FP.whirl_inlet(self", sub_once(r'S\.wp\.whirlMin \+= ctx\.get\("m", 0\.0\)(?! \* FP)', 'S.wp.whirlMin += ctx.get("m", 0.0) * FP.whirl_inlet(self, S.wp, p, ctx)'))),
        ("sim.gd", done_if("FP.trub_ns(self, v)", sub_once(r'var fl: float = 0\.1 \* v\.trubHB(?![\w.]|\s*\+)', 'var fl: float = minf(v.trubSus, 0.1 * v.trubHB + FP.trub_ns(self, v))'))),
    ], needs=["H0"]))
    # H6 – chladnutí trubek
    H.append(Hook("H6", "sim.gd: chladnutí stojícího obsahu trubek podle průměru", probes=[("sim.gd", "FP.pipe_cool_k(self, e, dt)", 1), ("sim.gd", "* kc_fp", 2)], edits=[
        ("sim.gd", need(r'^func net_step\(dt\b', "funkce net_step() nemá parametr dt")),
        ("sim.gd", done_if("FP.pipe_cool_k(self, e, dt)", sub_once(r'e\.c\.T \+= \(c\.ambientT - e\.c\.T\) \* kcool\b', 'var kc_fp: float = FP.pipe_cool_k(self, e, dt); e.c.T += (c.ambientT - e.c.T) * kc_fp'))),
        ("sim.gd", done_if("b.T += (c.ambientT - b.T) * kc_fp", sub_once(r'for b in e\.q: b\.T \+= \(c\.ambientT - b\.T\) \* kcool\b', 'for b in e.q: b.T += (c.ambientT - b.T) * kc_fp'))),
    ], needs=["H0"]))
    # H7 – tepelná roztažnost
    H.append(Hook("H7", "sim.gd: objem horké kapaliny podle Kella (hotV)", probes=[("sim.gd", "liqVol(v) * FP.expansion(v.T)", 1)], edits=[
        ("sim.gd", sub_once(r'liqVol\(v\) \* \(1\.0 \+ 0\.00052 \* maxf\(0\.0, v\.T - 20\.0\)\)', 'liqVol(v) * FP.expansion(v.T)')),
    ], needs=["H0"]))
    H.append(Hook("H7b", "sim.gd: hustota horké kapaliny podle Kella (density)", probes=[("sim.gd", "var exp_f: float = FP.expansion(v.T)", 1)], edits=[
        ("sim.gd", sub_once(r'var exp_f: float = 1\.0 \+ 0\.00052 \* maxf\(0\.0, v\.T - 20\.0\)', 'var exp_f: float = FP.expansion(v.T)')),
    ], needs=["H0"]))
    # H8 – parametry
    H.append(Hook("H8", "sim.gd: nové parametry v CFG_DEFAULT a CALIB_RANGE (kalibrace.json je přijme)", edits=[
        ("sim.gd", add_dict_entries("CFG_DEFAULT", cfg_line)),
        ("sim.gd", add_dict_entries("CALIB_RANGE", range_line)),
    ]))
    H.append(Hook("H8p", "params.gd: popis nových parametrů (PARAMETRY.md)", edits=[
        ("params.gd", add_dict_entries("META", meta_lines)),
    ], optional=True))
    # H9 – testy
    H.append(Hook("H9", "tests.gd: spuštění testů FYZIKA+", probes=[("tests.gd", "fyzika_plus_tests.gd", 1)], edits=[
        ("tests.gd", first_of(
            sub_once(r'^([ \t]*)(var AU = preload\("res://audit_tests\.gd"\)\.new\(\);\s*AU\.run\(self\)[^\n]*)$', r'\1\2' + '\n' + r'\1' + 'var FPT = preload("res://fyzika_plus_tests.gd").new();FPT.run(self)   # FYZIKA+ H9'),
            sub_once(r'^([ \t]*)(print\("TESTY: [^\n]*)$', r'\1var FPT = preload("res://fyzika_plus_tests.gd").new();FPT.run(self)   # FYZIKA+ H9' + '\n' + r'\1\2'),
        )),
    ]))
    H.append(Hook("H9f", "tests.gd: referenční přestup tepla se měří při 60 °C (nová teplotní závislost)", probes=[("tests.gd", "thin.T = 60.0", 1)], edits=[
        ("tests.gd", sub_once(r'(var thin = sim\.vessel\(\);\s*thin\.w = 2000\.0;\s*thin\.stir = true;\s*thin\.hz = 35\.0)(?![\d.]|;thin\.T)', r'\1;thin.T = 60.0', comment="# FYZIKA+ H9f")),
    ], optional=True, needs=["H2"]))
    H.append(Hook("H9g", "audit_tests.gd: totéž v regresním testu přestupu tepla", probes=[("audit_tests.gd", "w.T = 60.0", 1)], edits=[
        ("audit_tests.gd", sub_once(r'(var w = sim\.vessel\(\); w\.w = 2000\.0; w\.stir = true; w\.hz = 35\.0)(?![\d.]|; w\.T)', r'\1; w.T = 60.0', comment="# FYZIKA+ H9g")),
    ], optional=True, needs=["H2"]))
    H.append(Hook("H9h", "audit_tests.gd: referenční test izomerace běží při pH 5,5 (kde je korekce pH neutrální)", probes=[("audit_tests.gd", ["FYZIKA+ H9h", "sim.S.pH = 5.5", 'sim.S["pH"] = 5.5'], 1)], edits=[
        ("audit_tests.gd", sub_once(r'^([ \t]*)(var vh1 = _mkv\(100\.0\); vh1\.alpha = 100\.0[^\n]*)$', r'\1sim.S["pH"] = 5.5   # FYZIKA+ H9h: referenční pH, při kterém je korekce izomerace neutrální' + '\n' + r'\1\2')),
    ], optional=True, needs=["H3b"]))
    # H10 – parní topný had bojleru stejným modelem jako plášť pánve
    H.append(Hook("H10", "sim.gd: pára do topného hadu bojleru přes ventil (stejný model jako plášť pánve)", probes=[("sim.gd", "FP.steam_coil(self", 1)], edits=[
        ("sim.gd", within("sim_step", sub_once(r'"hlt": \{"P": c\.hltUAst \* h\.heat \* maxf\(0\.0, Tst - h\.T\), "wet": 1\.0\}', '"hlt": FP.steam_coil(self, h, c.hltUAst, Tst)', comment="# FYZIKA+ H10"))),
    ], optional=True, needs=["H0"]))
    # H11 – průtok ledové vody podle přiškrcení klapek
    H.append(Hook("H11", "sim.gd: průtok ledové vody chladičem podle přiškrcení klapek (dřív pevně 120 L/min)", probes=[("sim.gd", "FP.ice_flow(self", 1)], edits=[
        ("sim.gd", need(r'var Lm = 120\.0 / maxf\(1\.0, dests\.size\(\)\)', "blok čerpadla ledové vody vypadá jinak (nenašel jsem var Lm = 120.0 / …)")),
        ("sim.gd", within("net_step", sub_once(r'^([ \t]*)for d in dests:[ \t]*\n([ \t]*)(var path = path_to\(r, d\.k\))[ \t]*$',
                            r'\1for d in dests:' + '\n' + r'\2\3' + '\n' + r'\2' + 'Lm = FP.ice_flow(self, path, dests.size())   # FYZIKA+ H11 – přiškrcení klapek ledové vody'))),
    ], optional=True, needs=["H0"]))
    # H14 – připalování: úbytek výkonu se počítá ve FP.steam_pd (kotel i nádoba vidí stejný výkon)
    H.append(Hook("H14", "sim.gd: připalování – úbytek výkonu vidí kotel i nádoba stejně (bilance energie)", probes=[("sim.gd", 'set_meta("fpH14", true)', 1)], edits=[
        ("sim.gd", need(r'^func heat\(v\b', "funkce heat() nemá parametr v")),
        ("sim.gd", within("heat", sub_once(r'^([ \t]*)Pin \*= 0\.9[ \t]*$', r'\1' + 'set_meta("fpH14", true)   # FYZIKA+ H14 – úbytek výkonu při připalování počítá FP.steam_pd, stejný pro kotel i nádobu (dřív: Pin *= 0.9)'))),
    ], optional=True, needs=["H1"]))
    # H12 – odpar z horké hladiny, když nádoba nevře (před varem, ve vířivce)
    H.append(Hook("H12", "sim.gd: odpar z horké hladiny před varem a ve vířivce (malý u zavřené nádoby, parametr evapOpen)", probes=[("sim.gd", "FP.surface_evap(self", 2)], edits=[
        ("sim.gd", need(r'^func heat\(v, m(: float)?, dt(: float)?, name\b', "funkce heat() má jiné parametry")),
        ("sim.gd", need(r'^func sim_step\(dt\b', "funkce sim_step() nemá parametr dt")),
        ("sim.gd", within("heat", done_if("FP.surface_evap(self, v, name, dt)", sub_once(r'^([ \t]*)(v\.T = T)[ \t]*$', r'\1\2' + '\n' + r'\1' + 'FP.surface_evap(self, v, name, dt)   # FYZIKA+ H12 – odpar z hladiny, když nevře')))),
        ("sim.gd", within("sim_step", done_if('FP.surface_evap(self, wv, "wp", dt)', sub_once(r'react\(wv, m\)(?!;FP)', 'react(wv, m);FP.surface_evap(self, wv, "wp", dt)', comment="# FYZIKA+ H12")))),
    ], optional=True, needs=["H0"]))
    # H13 – rychlost rmutu v potrubí (smyk)
    H.append(Hook("H13", "sim.gd: rychlost rmutu v potrubí nad 1,5 m/s zhoršuje scezování (Kunze s. 903)", probes=[("sim.gd", "FP.transfer_check(self", 1)], edits=[
        ("sim.gd", need(r'var sp_ = path_to\(', "smyčka čerpadel nemá sací trasu sp_")),
        ("sim.gd", need(r'var dpaths\b', "smyčka čerpadel nemá výtlačné trasy dpaths")),
        ("sim.gd", within("net_step", sub_once(r'^([ \t]*)(var part0 = srcTake\(sb, Qf, m, key\))[ \t]*$', r'\1\2' + '\n' + r'\1' + 'FP.transfer_check(self, key, sb, part0, Qf, sp_, m, dpaths)   # FYZIKA+ H13 – rychlost rmutu v potrubí'))),
    ], optional=True, needs=["H0"]))
    # H15 – samospád podle hydrauliky trasy (stejná tlaková ztráta jako u čerpadel)
    H.append(Hook("H15", "sim.gd: samospád podle průměru, délky a přiškrcení trasy (dřív pevně 120 L/min na 1 m)", probes=[("sim.gd", "FP.gravity_q(self", 2)], edits=[
        ("sim.gd", need(r'^func path_head\(', "verze nemá hydrauliku tras (path_head)")),
        ("sim.gd", need(r'^func _src_fluid\(', "verze nemá _src_fluid()")),
        ("sim.gd", need(r'var pa = path_to\(', "_vessel_gravity() nemá trasu pa")),
        ("sim.gd", need(r'"lk": lk\b', "výpusti do kanálu nemají otevřený konec o.lk")),
        ("sim.gd", need(r'var head_m\b', "výpusti do kanálu nemají výšku hladiny head_m")),
        ("sim.gd", within("_vessel_gravity", done_if("FP.gravity_q(self, pa, bhd, gb)", sub_once(r'var q: float = 120\.0 \* f \* minf\(1\.3, sqrt\(bhd / 1\.05\)\)', 'var q: float = FP.gravity_q(self, pa, bhd, gb)', comment="# FYZIKA+ H15 (dřív: 120.0 * f * minf(1.3, sqrt(bhd / 1.05)))")))),
        ("sim.gd", within("net_step", done_if("FP.gravity_q(self, o.path", sub_once(r'var Lmin: float = tot \* o\.f / outs\.size\(\); var part = null', 'var Lmin: float = FP.gravity_q(self, o.path, maxf(0.0, head_m + (net.nodes[o.lk.k].y - (v.y + v.h)) / PX_PER_M), b); var part = null', comment="# FYZIKA+ H15 (dřív: tot * o.f / outs.size())")))),
    ], optional=True, needs=["H0"]))
    # H16 – stojící odstředivé čerpadlo je průchozí (samospád a zpětný tok), automat po kroku zavírá klapky
    FPM = 'preload("res://fyzika_plus.gd")'
    H.append(Hook("H16", "sim.gd + auto.gd: stojícím čerpadlem teče kapalina samospádem (oběma směry); automat po každém kroku zavře klapky", probes=[("sim.gd", 'pass2.has("~rev")', 1), ("sim.gd", '"out" if a.side == "in" else "in"', 1), ("sim.gd", "FP.idle_pumps(self)", 1), ("auto.gd", '"pumpPass"', 1)], edits=[
        ("sim.gd", need(r'^func _vessel_gravity\(', "verze nemá samospád mezi nádobami (_vessel_gravity)")),
        ("auto.gd", need(r'^func _close_all\(', "automat nemá _close_all()")),
        ("auto.gd", need(r'^var pend\b', "automat nemá seznam spouštěných čerpadel (pend)")),
        ("sim.gd", within("bfs", done_if('pass2.has("~rev")', sub_once(r'if pass2\.has\(pb\.get\("bind", ""\)\) and a\.side == "in":', 'if pass2.has(pb.get("bind", "")) and (a.side == "in" or pass2.has("~rev")):', comment="# FYZIKA+ H16 – stojícím čerpadlem to teče oběma směry")))),
        ("sim.gd", within("bfs", done_if('"out" if a.side == "in" else "in"', sub_once(r'for k2 in net\.pumpNodes\.get\(a\.id, \{\}\)\.get\("out", \[\]\):', 'for k2 in net.pumpNodes.get(a.id, {}).get("out" if a.side == "in" else "in", []):', comment="# FYZIKA+ H16")))),
        ("sim.gd", within("_vessel_gravity", done_if("FP.idle_pumps(self)", sub_once(r'var gr = bfs\(sts\)', 'var gr = bfs(sts, "liq", FP.idle_pumps(self))', comment="# FYZIKA+ H16 – trasa samospádu smí vést přes stojící čerpadlo")))),
        ("auto.gd", within("_end_step", done_if('"pumpPass"', sub_once(r'^([ \t]*)i \+= 1[ \t]*$', r'\1if ' + FPM + '.cf(sim, "pumpPass") >= 0.5: _close_all()   # FYZIKA+ H16 – po kroku zavřít klapky: stojícím čerpadlem by to teklo samospádem dál nebo zpátky' + '\n' + r'\1i += 1')))),
    ], optional=True, needs=["H0"]))
    H.append(Hook("H16b", "lauter_auto.gd: automat scezování po skončení zavře klapky, které si otevřel", probes=[("lauter_auto.gd", ".lauter_stop(sim)", 1)], edits=[
        ("lauter_auto.gd", within("stop", sub_once(r'^([ \t]*)(sim\.S\.p2\.on = false;\s*sim\.S\.skWater\.on = false)[ \t]*$', r'\1\2' + '\n' + r'\1' + FPM + '.lauter_stop(sim)   # FYZIKA+ H16b – zavřít své klapky (stojícím C2 by pánev tekla zpátky pod síta)'))),
    ], optional=True, needs=["H16"]))
    # H17 – ztráty tepla podle smáčené plochy
    H.append(Hook("H17", "sim.gd: ztráty tepla pánve a vířivky podle zaplnění (poloprázdná nádoba ztrácí míň než plná)", probes=[("sim.gd", "FP.loss_k(self", 2)], edits=[
        ("sim.gd", need(r'^func heat\(v, m(: float)?, dt(: float)?, name\b', "funkce heat() má jiné parametry")),
        ("sim.gd", within("heat", done_if("FP.loss_k(self, name, v)", sub_once(r'^([ \t]*)(var UA: float = c\.rvpUA if name == "rvp" else c\.mpUA)[ \t]*$', r'\1\2' + '\n' + r'\1' + 'UA *= FP.loss_k(self, name, v)   # FYZIKA+ H17 – ztráty podle smáčené plochy (UA platí pro plnou nádobu)')))),
        ("sim.gd", within("sim_step", done_if('FP.loss_k(self, "wp", wv)', sub_once(r'wv\.T -= c\.wpUA \* \(wv\.T - c\.ambientT\) \* dt / Cw', 'wv.T -= c.wpUA * FP.loss_k(self, "wp", wv) * (wv.T - c.ambientT) * dt / Cw', comment="# FYZIKA+ H17")))),
    ], optional=True, needs=["H0"]))
    # U – panel (volitelné)
    H.append(Hook("U1", "ctl.gd: tlačítko kyseliny mléčné do mladiny", probes=[("ctl.gd", "fpAcid", 2)], edits=[
        ("ctl.gd", need(r'r = _row\(box\)', "ctl.gd nemá řádky r = _row(box)")),
        ("ctl.gd", done_if('refs["fpAcid"] = _btn', sub_once(r'^([ \t]*)(refs\.hop = _btn\(r, "", func\(\): m\.sim\.hopPlanAdd\(\);\s*refresh\(\)\)[^\n]*)$',
                            r'\1\2' + '\n' + r'\1' + 'r = _row(box)   # FYZIKA+ U1' + '\n' + r'\1' + 'refs["fpAcid"] = _btn(r, "", func(): preload("res://fyzika_plus.gd").acid_dose(m.sim, 100.0);refresh())'))),
        ("ctl.gd", done_if('refs.has("fpAcid")', sub_once(r'^([ \t]*)(else: _t\(refs\.hop, "Chmel podle receptu přidán \(%d dávky\)" % S\.hops\.size\(\)\)[^\n]*)$',
                            r'\1\2' + '\n' + r'\1' + 'if refs.has("fpAcid"): _t(refs.fpAcid, preload("res://fyzika_plus.gd").acid_label(m.sim))   # FYZIKA+ U1'))),
    ], optional=True, needs=["H0"]))
    H.append(Hook("U2", "ctl.gd: tlak v plášti pánve u řádku kotle", probes=[("ctl.gd", "jacket_label(m.sim)", 1)], edits=[
        ("ctl.gd", sub_once(r'_t\(refs\.boiler, "Pára z kotle: %s   \(zatížení %d %%\)" % \[onoff\(S\.boiler\.on\), roundi\(S\.boiler\.load \* 100\.0\)\]\)(?! \+)',
                            '_t(refs.boiler, ("Pára z kotle: %s   (zatížení %d %%)" % [onoff(S.boiler.on), roundi(S.boiler.load * 100.0)]) + preload("res://fyzika_plus.gd").jacket_label(m.sim))', comment="# FYZIKA+ U2")),
    ], optional=True, needs=["H1"]))
    H.append(Hook("U3", "ctl.gd: doplňkové řádky FYZIKA+ ve stavovém panelu (chladič, hladina)", probes=[("ctl.gd", "panel_lines(m.sim)", 1)], edits=[
        ("ctl.gd", sub_once(r'^([ \t]*)(lines\.append\("\[b\]Spilka:\[/b\] %s" % m\.vessel_text\("spilka"\)\))[ \t]*$',
                            r'\1\2' + '\n' + r'\1' + 'for fp_l in preload("res://fyzika_plus.gd").panel_lines(m.sim): lines.append(fp_l)   # FYZIKA+ U3')),
    ], optional=True, needs=["H0"]))
    # V – vzhled kapalin na plánku (volitelné, main.gd; nemění výpočet)
    VZ = 'preload("res://fyzika_plus_vzhled.gd")'
    def segs_head(m, style):
        return m.group(0) + "\n" + m.group(1) + (style if style == "\t" else "    ") + "return " + VZ + ".stream_segs(m)   # FYZIKA+ V1 – proudy po balistické dráze; původní výpočet níže už neběží"
    H.append(Hook("V1", "main.gd: proudy do nádob padají obloukem (sprcha, boční nátok) místo rovných čar", probes=[("main.gd", ".stream_segs(m)", 1)], edits=[
        ("main.gd", need(r'm\.kind_color\(', "main.gd nemá kind_color()")),
        ("main.gd", sub_once(r'^([ \t]+)func _stream_segs\(\)(\s*->\s*Array)?\s*:[ \t]*$', segs_head)),
    ], optional=True))
    H.append(Hook("V2", "main.gd: hladina se v místě dopadu proudu vlní (rmut míň než voda)", probes=[("main.gd", ".ripple(fxs, px, tt, is_mash)", 1), ("main.gd", ".ripple(fxs, px2, tt, is_mash)", 1), ("main.gd", ".impacts(m, it, fxs, lv)", 1)], edits=[
        ("main.gd", need(r'var is_mash\s*:?\s*=\s*false', "kreslení kapaliny nemá proměnnou is_mash")),
        ("main.gd", done_if(".impacts(m, it, fxs, lv)", sub_once(r'^([ \t]+)(fxs\["lv"\] = lv;\s*fxs\["slosh"\] = slosh)[ \t]*$', r'\1\2' + '\n' + r'\1' + 'fxs["fpimp"] = ' + VZ + '.impacts(m, it, fxs, lv)   # FYZIKA+ V2 – místa dopadu proudů na hladinu'))),
        ("main.gd", done_if(".ripple(fxs, px, tt, is_mash)", sub_once(r'amp \* 0\.5 \* sin\(px \* 0\.11 - tt \* 4\.7\)(?! \+ preload)', 'amp * 0.5 * sin(px * 0.11 - tt * 4.7) + ' + VZ + '.ripple(fxs, px, tt, is_mash)'))),
        ("main.gd", done_if(".ripple(fxs, px2, tt, is_mash)", sub_once(r'amp \* 0\.5 \* sin\(px2 \* 0\.11 - tt \* 4\.7\)(?! \+ preload)', 'amp * 0.5 * sin(px2 * 0.11 - tt * 4.7) + ' + VZ + '.ripple(fxs, px2, tt, is_mash)'))),
    ], optional=True, needs=["V1"]))
    H.append(Hook("V3", "main.gd: dílo bez míchání se usazuje – nahoře čirá kapalina, u dna vrstva zrna", probes=[("main.gd", ".settled_layer(", 1)], edits=[
        ("main.gd", need(r'var is_mash\s*:?\s*=\s*false', "kreslení kapaliny nemá proměnnou is_mash")),
        ("main.gd", need(r'var outline: PackedVector2Array = m\.vessel_inside\(it\)', "kreslení kapaliny nemá obrys nádoby (outline)")),
        ("main.gd", within("_liquid", sub_once(r'^([ \t]+)(if b != "lv":[ \t]*\n[ \t]+LiquidFx\.draw_caustics\()', r'\1' + VZ + '.settled_layer(self, m, it, b, fxs, outline, lv, y0, tt, is_mash)   # FYZIKA+ V3 – usazené dílo' + '\n' + r'\1\2'))),
    ], optional=True))
    # R – protokol (volitelné)
    H.append(Hook("R1", "sim.gd: spotřeba páry na hl, pH mladiny a odhad barvy piva v protokolu", probes=[("sim.gd", "FP.energy_step(", 1), ("sim.gd", "FP.report_extra(", 1)], edits=[
        ("sim.gd", need(r'^func sim_step\(dt\b', "funkce sim_step() nemá parametr dt")),
        ("sim.gd", done_if("FP.energy_step(", sub_once(r'^([ \t]*)(S\.boiler\.load = Qb / c\.boilerKW)[ \t]*$', r'\1\2' + '\n' + r'\1' + 'FP.energy_step(self, Qb, dt)   # FYZIKA+ R1'))),
        ("sim.gd", done_if("FP.report_extra(", sub_once(r'^([ \t]*)(rep\.merge\(yields\(\)\))[ \t]*$', r'\1\2' + '\n' + r'\1' + 'FP.report_extra(self, rep)   # FYZIKA+ R1'))),
    ], optional=True, needs=["H0"]))
    return H


def main(argv):
    try:
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")   # Windows (cp1250) by jinak spadl na znaku ✔
    except Exception:
        pass
    if len(argv) < 2 or argv[1] in ("-h", "--help"):
        print(__doc__)
        return 1
    proj = argv[1]
    dry = "--zkouska" in argv
    no_ui = "--bez-ui" in argv
    no_look = "--bez-vzhledu" in argv
    if os.path.isfile(proj):
        proj = os.path.dirname(proj)
    if not os.path.isfile(os.path.join(proj, "sim.gd")):
        for root, dirs, files in os.walk(proj):
            if "sim.gd" in files and "project.godot" in files:
                proj = root
                break
    if not os.path.isfile(os.path.join(proj, "sim.gd")):
        print("CHYBA: ve složce %s není sim.gd (zadej složku projektu trenažéru, kde je project.godot)." % proj)
        return 1
    print("Projekt: %s%s" % (proj, "   (ZKOUŠKA – nic se nezapíše)" if dry else ""))
    cache = {}
    orig = {}
    crlf = {}

    def get(fn):
        if fn not in cache:
            p = os.path.join(proj, fn)
            if not os.path.isfile(p):
                cache[fn] = None
            else:
                with open(p, "r", encoding="utf-8", newline="") as f:
                    raw = f.read()
                crlf[fn] = "\r\n" in raw
                cache[fn] = raw.replace("\r\n", "\n")
                orig[fn] = cache[fn]
        return cache[fn]

    # moduly nejdřív (kdyby kopírování selhalo, háčky se nevloží a projekt zůstane funkční)
    if not dry:
        for mf in MODULE_FILES:
            src_f = os.path.join(HERE, mf); dst_f = os.path.join(proj, mf)
            if os.path.exists(dst_f) and os.path.samefile(src_f, dst_f):
                continue
            shutil.copy2(src_f, dst_f)

    applied, skipped = [], []
    changed = set()
    for hk in build_hooks():
        if no_ui and hk.hid.startswith("U"):
            skipped.append((hk, "vynecháno (--bez-ui)"))
            continue
        if no_look and hk.hid.startswith("V"):
            skipped.append((hk, "vynecháno (--bez-vzhledu)"))
            continue
        miss = [n for n in hk.needs if n not in [a.hid for a in applied]]
        if miss:
            skipped.append((hk, "chybí předchozí háček %s" % ", ".join(miss)))
            continue
        def _cnt(fn, t):
            txt = get(fn)
            if txt is None:
                return -1
            return max(txt.count(x) for x in t) if isinstance(t, list) else txt.count(t)
        if hk.probes and all(_cnt(fn, t) >= n for fn, t, n in hk.probes):
            applied.append(hk)          # už vložené dřív
            continue
        new = {}
        why = ""
        for fn, fnc in hk.edits:
            txt = new.get(fn, get(fn))
            if txt is None:
                why = "soubor %s chybí" % fn
                break
            res, w = fnc(txt, indent_style(txt))
            if res is None:
                why = "%s: %s" % (fn, w)
                break
            new[fn] = res
        if why:
            skipped.append((hk, why))
            continue
        for fn, txt in new.items():
            if txt != cache[fn]:
                cache[fn] = txt
                changed.add(fn)
        applied.append(hk)

    if not dry:
        for fn in sorted(changed):
            p = os.path.join(proj, fn)
            bak = p + ".pred-fyzika-plus"
            # záloha = poslední verze BEZ FYZIKA+ (po aktualizaci programu se obnoví, stará by vracela starý kód)
            if "FYZIKA+" not in orig.get(fn, "") or not os.path.exists(bak):
                shutil.copy2(p, bak)
            out = cache[fn].replace("\n", "\r\n") if crlf.get(fn) else cache[fn]
            with open(p, "w", encoding="utf-8", newline="") as f:
                f.write(out)
        ids = [a.hid for a in applied]
        with open(os.path.join(proj, "fyzika_plus_hooks.gd"), "w", encoding="utf-8", newline="\n") as f:
            f.write("extends RefCounted\n## vygeneroval aplikuj_fyzika_plus.py – háčky FYZIKA+ vložené v tomhle projektu (testy podle toho přeskočí chybějící)\n")
            f.write("const APPLIED := [%s]\n" % ", ".join('"%s"' % i for i in sorted(set(ids))))

    print("\nVLOŽENO (%d):" % len(applied))
    for hk in applied:
        print("  ✔ %-4s %s" % (hk.hid, hk.title))
    if skipped:
        print("\nPŘESKOČENO (%d):" % len(skipped))
        for hk, why in skipped:
            print("  %s %-4s %s\n         důvod: %s" % ("·" if hk.optional else "✗", hk.hid, hk.title, why))
    core_missing = [hk.hid for hk, _ in skipped if not hk.optional]
    if not dry:
        print("\nZměněné soubory: %s" % (", ".join(sorted(changed)) if changed else "žádné"))
        print("Zálohy: <soubor>.pred-fyzika-plus   ·   Přidáno: %s, fyzika_plus_hooks.gd" % ", ".join(MODULE_FILES))
    if core_missing:
        print("\nPOZOR: nevložily se hlavní háčky %s – návod k ručnímu vložení je v INTEGRACE.md." % ", ".join(core_missing))
        return 2
    print("\nHotovo. Ověř: Godot --headless --path <projekt> -- --tests  (a --autotest).")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))
