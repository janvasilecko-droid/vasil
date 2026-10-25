#!/usr/bin/env python3
"""Vygeneruje unity/docs/KALIBRACE.md z GodotSrc/sim.gd (CFG_DEFAULT) a GodotSrc/params.gd (META)."""
import re, sys, os
here = os.path.dirname(os.path.abspath(__file__))
src = os.path.join(here, "..", "GodotSrc")
sim = open(os.path.join(src, "sim.gd"), encoding="utf-8").read()
par = open(os.path.join(src, "params.gd"), encoding="utf-8").read()
blk = sim[sim.index("const CFG_DEFAULT"):sim.index("const CALIB_PATH")]
blk = blk[:blk.index("\n}\n")] if "\n}\n" in blk else blk
defaults = {}
for m in re.finditer(r'"([A-Za-z0-9_]+)":\s*(-?[0-9.eE+-]+|true|false|"[^"]*")', blk):
    defaults.setdefault(m.group(1), m.group(2).strip('"'))
meta = {}
mblk = par[par.index("const META"):]
for m in re.finditer(r'"([A-Za-z0-9_]+)":\s*\["([^"]*)",\s*"([^"]*)",\s*"([^"]*)",\s*"((?:[^"\\]|\\.)*)"\]', mblk):
    meta[m.group(1)] = m.groups()[1:]
rng = {}
rb = sim[sim.index("const CALIB_RANGE"):]
rb = rb[:rb.index("\n}\n")]
for m in re.finditer(r'"([A-Za-z0-9_]+)":\s*\[([-0-9.eE+]+),\s*([-0-9.eE+]+)\]', rb):
    rng[m.group(1)] = (m.group(2), m.group(3))
order = {"MEASURED": 0, "MANUFACTURER": 1, "LITERATURE": 2, "CALIBRATED": 3, "CALIBRATED (na referenční scénář, ne na měření)": 3, "STATED": 3, "ESTIMATED": 4, "RECEPT": 6, "UNKNOWN": 7}
out = ["# Kalibrace modelu – přehled parametrů", "",
       "Soubor je **generovaný** skriptem `unity/Tools/gen_calib_doc.py` z `sim.gd` (výchozí hodnoty, povolené rozsahy) a `params.gd` (jednotka, zdroj, nejistota, co a jak změřit). Neupravuj ručně.", "",
       "Zdroj hodnoty: **MEASURED** změřeno na varně · **MANUFACTURER** údaj provozovatele / štítek · **LITERATURE** hodnota z literatury uvedená v kódu · **ESTIMATED** technický odhad, nutné změřit · **CALIBRATED** upraveno podle měření (u „na referenční scénář, ne na měření“ jde o ladění modelu na referenční údaj, ne o měření na této varně) · **RECEPT** cíl nebo provozní nastavení receptu, ne fyzikální veličina (jeho nejistota je tolerance receptu, ne chyba modelu) · **STATED** údaj sdělený provozovatelem bez dokladu.", "",
       "Kalibraci lze zadat bez zásahu do kódu v souboru `user://kalibrace.json` (klíč → hodnota); hodnota mimo povolený rozsah se odmítne.", ""]
stats = {}
for k, v in meta.items(): stats[v[1]] = stats.get(v[1], 0) + 1
out.append("Souhrn: " + ", ".join("%s %d" % (a, stats[a]) for a in sorted(stats, key=lambda x: order.get(x, 9))) + f"; celkem {len(meta)} popsaných parametrů.")
out.append("")
out += ["| Parametr | Výchozí | Jednotka | Zdroj | Nejistota / rozsah | Povolený rozsah | Co ovlivňuje / jak změřit |", "|---|---|---|---|---|---|---|"]
for k in sorted(meta, key=lambda x: (order.get(meta[x][1], 9), x)):
    u, s, unc, d = meta[k]
    r = "%s … %s" % rng[k] if k in rng else "–"
    out.append("| `%s` | %s | %s | %s | %s | %s | %s |" % (k, defaults.get(k, "–"), u, s, unc, r, d.replace("|", "/")))
miss = [k for k in defaults if k not in meta]
if miss:
    out += ["", "## Parametry bez popisu v registru", "", "Mají výchozí hodnotu v kódu, ale chybí jim jednotka/zdroj/nejistota – doplnit do `params.gd`:", ""]
    out.append(", ".join("`%s`=%s" % (k, defaults[k]) for k in sorted(miss)))
path = os.path.join(here, "..", "docs", "KALIBRACE.md")
open(path, "w", encoding="utf-8").write("\n".join(out) + "\n")
print("zapsáno", path, len(meta), "popsaných,", len(miss), "bez popisu")
