#!/usr/bin/env python3
"""Sestaví složku aktualizace pro větev unity-update:
   make_update_manifest.py <výstupní_složka> <PivovarCore.dll> <PivovarGdGen.dll> <commit> [requires]
Do <výstup>/files/ zkopíruje Managed/*.dll a StreamingAssets/godot (bez .meta) a napíše manifest.json."""
import hashlib, json, os, shutil, sys, time, subprocess
out, core, gdgen, commit = sys.argv[1:5]
requires = int(sys.argv[5]) if len(sys.argv) > 5 else 1
here = os.path.dirname(os.path.abspath(__file__))
sa = os.path.join(here, "..", "Assets", "StreamingAssets")
files = []
def add(src, rel):
    dst = os.path.join(out, "files", rel)
    os.makedirs(os.path.dirname(dst), exist_ok=True)
    shutil.copyfile(src, dst)
    h = hashlib.sha256(open(src, "rb").read()).hexdigest()
    files.append({"path": rel, "sha256": h, "size": os.path.getsize(src)})
add(core, "Managed/PivovarCore.dll")
add(gdgen, "Managed/PivovarGdGen.dll")
for root, _, names in os.walk(sa):
    for n in sorted(names):
        if n.endswith(".meta") or n == "build_stamp.txt": continue
        p = os.path.join(root, n)
        rel = "StreamingAssets/" + os.path.relpath(p, sa).replace(os.sep, "/")
        if not rel.startswith("StreamingAssets/godot/"): continue   # plan.json (3D) si Vasil upravuje sám – neaktualizuje se
        add(p, rel)
ver = int(time.strftime("%Y%m%d%H%M", time.gmtime()))
try:
    notes = subprocess.check_output(["git", "log", "-1", "--format=%s"], text=True).strip()
except Exception:
    notes = ""
m = {"ver": ver, "commit": commit, "notes": notes, "requires": requires, "files": sorted(files, key=lambda f: f["path"])}
os.makedirs(out, exist_ok=True)
json.dump(m, open(os.path.join(out, "manifest.json"), "w", encoding="utf-8"), ensure_ascii=False, indent=1)
print("manifest: ver", ver, "souborů", len(files), "celkem", sum(f["size"] for f in files), "B")
