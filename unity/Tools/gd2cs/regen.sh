#!/bin/bash
# Přegeneruje C# z původních GDScriptů (zdroj: ../godot/zajic2d/zajic2d)
set -e
HERE=$(cd "$(dirname "$0")" && pwd)
SRC=${1:-/home/claude/godot/zajic2d/zajic2d}
OUT=$HERE/../../Assets/Pivovar/GdGen
RT=$HERE/../../Assets/Pivovar/Core/Gd
rm -rf "$OUT"; mkdir -p "$OUT"
python3 -I "$HERE/gen.py" "$SRC" "$OUT" "$RT" physics params proc auto lauter_auto training practice sim tests audit_tests art
rm -f "$OUT/_dyn_methods.json"
