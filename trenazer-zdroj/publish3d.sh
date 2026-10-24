#!/bin/bash
# Vydání 3D trenažéru (Godot 4.7.2, Forward+): ./publish3d.sh <verze> "<poznámka>" [exe]
# bez 'exe' nahraje jen pck (⟳ Aktualizovat); s 'exe' vytvoří i ZIP s programem pro první instalaci
set -e
V=$1; NOTE=$2; WITHEXE=$3
P=/home/claude/godot/zajic47; R=/home/claude/vasil; G=/tmp/g47/Godot_v4.7.2-stable_linux.x86_64
python3 - "$V" "$NOTE" <<'PY'
import json,sys,datetime
json.dump({"ver":int(sys.argv[1]),"date":datetime.date.today().isoformat(),"notes":sys.argv[2]},open('/home/claude/godot/zajic47/version.json','w'),ensure_ascii=False)
PY
mkdir -p $R/trenazer3d /tmp/out47
cd $P && timeout 300 $G --headless --path . --export-pack WinPack $R/trenazer3d/trenazer3d.pck >/dev/null 2>&1
SIZE=$(stat -c %s $R/trenazer3d/trenazer3d.pck)
python3 - "$SIZE" <<'PY'
import json,sys
d=json.load(open('/home/claude/godot/zajic47/version.json')); d['size']=int(sys.argv[1])
json.dump(d,open('/home/claude/vasil/trenazer3d/version.json','w'),ensure_ascii=False)
PY
if [ "$WITHEXE" = "exe" ]; then
  rm -rf /tmp/out47/*; timeout 300 $G --headless --path . --export-release WinPack /tmp/out47/TrenazerVarny3D.exe >/dev/null 2>&1
  cd /tmp/out47 && rm -f $R/trenazer3d/TrenazerVarny3D.zip && zip -q -9 $R/trenazer3d/TrenazerVarny3D.zip TrenazerVarny3D.exe TrenazerVarny3D.pck
fi
cd $R && git add trenazer3d && git commit -q -m "Trenažér varny 3D: verze $V – $NOTE

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
Claude-Session: https://claude.ai/code/session_01B2a3kEiSUZXSMLybLeAqpq" && git push -q origin HEAD:main && echo "vydáno $V ($SIZE B)"
