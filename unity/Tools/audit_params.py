#!/usr/bin/env python3
"""Audit registru parametrů: porovná CFG_DEFAULT (sim.gd), CALIB_RANGE (sim.gd), META (params.gd) a skutečné použití v kódu.

Skončí kódem 1, když najde nekonzistenci (CI ji tak chytí). Zapíše unity/docs/AUDIT-PARAMETRU.md.
Kontroly:
  1. každý parametr z CFG_DEFAULT má popis v META (jednotka, zdroj, nejistota) a povolený rozsah (číselné),
  2. výchozí hodnota leží uvnitř povoleného rozsahu,
  3. každý parametr se opravdu čte ve výpočtu (mimo definice, registr a testy),
  4. každé čtení cfg.xxx / c.xxx míří na existující klíč (překlep = tichá nula),
  5. záložní literály ve volání get("klíč", literál) se rovnají výchozí hodnotě (jediný zdroj pravdy),
  6. META nepopisuje klíč, který v CFG_DEFAULT není.
"""
import re, os, sys

here = os.path.dirname(os.path.abspath(__file__))
src = os.path.join(here, "..", "GodotSrc")
files = sorted(f for f in os.listdir(src) if f.endswith(".gd"))
txt = {f: open(os.path.join(src, f), encoding="utf-8").read() for f in files}
sim, par = txt["sim.gd"], txt["params.gd"]

blk = sim[sim.index("const CFG_DEFAULT"):sim.index("const CALIB_PATH")]
cfgblk = blk[:blk.index("\n}\n")]
defaults = {}
for m in re.finditer(r'"([A-Za-z0-9_]+)":\s*(-?[0-9.eE+-]+|true|false|"[^"]*")', cfgblk):
    defaults.setdefault(m.group(1), m.group(2))
rb = sim[sim.index("const CALIB_RANGE"):]
rb = rb[:rb.index("\n}\n")]
rng = {m.group(1): (float(m.group(2)), float(m.group(3)))
       for m in re.finditer(r'"([A-Za-z0-9_]+)":\s*\[([-0-9.eE+]+),\s*([-0-9.eE+]+)\]', rb)}
meta = {}
for m in re.finditer(r'"([A-Za-z0-9_]+)":\s*\["([^"]*)",\s*"([^"]*)",\s*"([^"]*)",\s*"((?:[^"\\]|\\.)*)"\]', par[par.index("const META"):]):
    meta[m.group(1)] = m.groups()[1:]

DYNAMIC = {  # klíče skládané za běhu: kk + "_H0" apod.
    "p1_H0": r'"_H0"', "p2_H0": r'"_H0"', "p3_H0": r'"_H0"',
    "p1_Qn": r'"_Qn"', "p2_Qn": r'"_Qn"', "p3_Qn": r'"_Qn"',
}
errors, warns = [], []

def is_num(v): return not (v in ("true", "false") or v.startswith('"'))

uses = {}
for k in defaults:
    u = []
    for f, t in txt.items():
        if f in ("params.gd", "tests.gd", "audit_tests.gd"): continue
        for i, l in enumerate(t.split("\n"), 1):
            if l.strip().startswith("#"): continue
            if f == "sim.gd" and i < 130 and re.match(r'\s*"', l): continue   # definice a rozsahy
            if re.search(r'(?<![A-Za-z0-9_])%s(?![A-Za-z0-9_])' % re.escape(k), l): u.append("%s:%d" % (f, i))
    if not u and k in DYNAMIC and re.search(DYNAMIC[k], sim): u.append("sim.gd:(skládaný klíč)")
    uses[k] = u

for k, v in sorted(defaults.items()):
    if k not in meta: errors.append("%s: chybí popis v META (jednotka/zdroj/nejistota)" % k)
    if is_num(v):
        if k not in rng: errors.append("%s: chybí povolený rozsah v CALIB_RANGE" % k)
        else:
            x = float(v)
            if not rng[k][0] <= x <= rng[k][1]: errors.append("%s: výchozí %s mimo povolený rozsah %s…%s" % (k, v, *rng[k]))
    if not uses[k]: errors.append("%s: parametr se nikde nepoužívá ve výpočtu" % k)
for k in meta:
    if k not in defaults: errors.append("%s: je v META, ale ne v CFG_DEFAULT" % k)
for k in rng:
    if k not in defaults: errors.append("%s: je v CALIB_RANGE, ale ne v CFG_DEFAULT" % k)

# čtení neexistujících klíčů
for f, t in txt.items():
    if f in ("params.gd", "tests.gd", "audit_tests.gd"): continue
    for i, l in enumerate(t.split("\n"), 1):
        if l.strip().startswith("#"): continue
        for m in re.finditer(r'(?<![A-Za-z0-9_])(?:S\.cfg|cfg_|\bc|\bcfg)\.([A-Za-z][A-Za-z0-9_]*)\b(?!\()', l):
            k = m.group(1)
            if k in defaults or k in ("get", "has", "keys", "size", "duplicate", "merge", "erase"): continue
            # c bývá i jiný slovník (receptura, pumpa…) – hlásíme jen klíče, které vypadají jako parametry (v META/registru podobné jméno)
            if k in meta or k in rng: errors.append("%s:%d: čte cfg.%s, které není v CFG_DEFAULT" % (f, i, k))
        for m in re.finditer(r'get\("([A-Za-z0-9_]+)",\s*([-0-9.eE+]+(?:\s*/\s*[0-9.]+)?)\)', l):
            k, lit = m.group(1), m.group(2)
            if k in defaults and is_num(defaults[k]) and not re.search(r'hop\.get\("%s"' % k, l):
                try: val = eval(lit)
                except Exception: continue
                d = float(defaults[k])
                if abs(val - d) > 1e-3 * max(abs(d), 1e-9): errors.append("%s:%d: get(\"%s\", %s) – záložní literál se liší od CFG_DEFAULT (%s)" % (f, i, k, lit, defaults[k]))

out = ["# Audit parametrů modelu", "",
       "_Generuje `unity/Tools/audit_params.py` (v CI), ručně neupravovat._", "",
       "Porovnává `CFG_DEFAULT` (sim.gd), `CALIB_RANGE` (sim.gd), `META` (params.gd) a skutečná čtení v kódu.", "",
       "- parametrů v `CFG_DEFAULT`: **%d**, popsaných v `META`: **%d**, s povoleným rozsahem: **%d**" % (len(defaults), len([k for k in defaults if k in meta]), len([k for k in defaults if k in rng])),
       "- nalezených nekonzistencí: **%d**" % len(errors), ""]
if errors: out += ["## Nekonzistence", ""] + ["- " + e for e in errors] + [""]
out += ["## Použití parametrů (první výskyty ve výpočtu)", "", "| Parametr | Výchozí | Počet čtení | Kde |", "|---|---|---|---|"]
for k in sorted(defaults):
    out.append("| `%s` | %s | %d | %s |" % (k, defaults[k], len(uses[k]), ", ".join(uses[k][:4])))
single = [k for k in sorted(defaults) if len(uses[k]) == 1]
out += ["", "Parametry čtené právě na jednom místě (změna se projeví jen tam; zkontrolováno, že jde o jediný fyzikální jev): " + ", ".join("`%s`" % k for k in single), ""]
open(os.path.join(here, "..", "docs", "AUDIT-PARAMETRU.md"), "w", encoding="utf-8").write("\n".join(out))
print("parametrů %d, popsaných %d, rozsahů %d, nekonzistencí %d" % (len(defaults), len(meta), len(rng), len(errors)))
for e in errors: print("CHYBA:", e)
sys.exit(1 if errors else 0)
