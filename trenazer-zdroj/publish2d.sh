#!/bin/bash
# Vydání aktualizace 2D trenažéru: ./publish2d.sh <verze> "<poznámka>"
# exportuje pck (Godot 4.3), nahraje do janvasilecko-droid/vasil/trenazer2d/ – program si ji stáhne tlačítkem ⟳ Aktualizovat
set -e
V=$1; NOTE=$2
P=/home/claude/godot/zajic2d; R=/home/claude/vasil; G=/tmp/g43/Godot_v4.3-stable_linux.x86_64
python3 - "$V" "$NOTE" <<'PY'
import json,sys,datetime
json.dump({"ver":int(sys.argv[1]),"date":datetime.date.today().isoformat(),"notes":sys.argv[2]},open('/home/claude/godot/zajic2d/version.json','w'),ensure_ascii=False)
PY
mkdir -p $R/trenazer2d
cd $P && timeout 300 $G --headless --path . --export-pack WinPack $R/trenazer2d/trenazer2d.pck >/dev/null 2>&1
SIZE=$(stat -c %s $R/trenazer2d/trenazer2d.pck)
python3 - "$SIZE" <<'PY'
import json,sys
d=json.load(open('/home/claude/godot/zajic2d/version.json')); d['size']=int(sys.argv[1])
json.dump(d,open('/home/claude/vasil/trenazer2d/version.json','w'),ensure_ascii=False)
PY
cp $R/trenazer2d/trenazer2d.pck "/tmp/gt/Trenazer varny.pck"
cd $R && git add trenazer2d && git commit -q -m "Trenažér varny 2D: verze $V – $NOTE

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
Claude-Session: https://claude.ai/code/session_01B2a3kEiSUZXSMLybLeAqpq" && git push -q origin HEAD:main && echo "vydáno $V ($SIZE B)"
