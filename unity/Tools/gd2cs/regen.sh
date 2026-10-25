#!/bin/bash
# Přegeneruje C# z původních GDScriptů (zdroj: ../godot/zajic2d/zajic2d)
set -e
HERE=$(cd "$(dirname "$0")" && pwd)
SRC=${1:-$HERE/../../GodotSrc}
OUT=$HERE/../../Assets/Pivovar/GdGen
RT=$HERE/../../Assets/Pivovar/Core/Gd
rm -rf "$OUT"; mkdir -p "$OUT"
python3 -I "$HERE/gen.py" "$SRC" "$OUT" "$RT" physics params proc auto lauter_auto training practice sim tests audit_tests art main ctl cabinet liquidfx oppanel rotametr sight iodine editor game boot updater
rm -f "$OUT/_dyn_methods.json"
