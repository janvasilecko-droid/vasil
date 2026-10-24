#!/bin/bash
# Vydání aktualizace trenažéru: ./publish.sh <verze> "<poznámka>"
# exportuje jen pck (pár set kB), nahraje do janvasilecko-droid/vasil/trenazer/ – program si ji stáhne tlačítkem ⟳ Aktualizovat
set -e
V=$1; NOTE=$2
P=/home/claude/godot/trenazer-varny; R=/home/claude/vasil; G=/tmp/godot/Godot_v4.4.1-stable_linux.x86_64
python3 - "$V" "$NOTE" <<'PY'
import json,sys,datetime
json.dump({"ver":int(sys.argv[1]),"date":datetime.date.today().isoformat(),"notes":sys.argv[2]},open('/home/claude/godot/trenazer-varny/version.json','w'),ensure_ascii=False)
PY
cd $P && timeout 150 $G --headless --editor --quit --path . >/dev/null 2>&1 || true
timeout 300 $G --headless --path . --export-pack "Windows" $R/trenazer/trenazer.pck >/dev/null 2>&1
SIZE=$(stat -c %s $R/trenazer/trenazer.pck)
python3 - "$SIZE" <<'PY'
import json,sys
d=json.load(open('/home/claude/godot/trenazer-varny/version.json')); d['size']=int(sys.argv[1])
json.dump(d,open('/home/claude/vasil/trenazer/version.json','w'),ensure_ascii=False)
PY
cd $R && git add trenazer && git commit -q -m "Trenažér varny: verze $V – $NOTE

Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>
Claude-Session: https://claude.ai/code/session_01B2a3kEiSUZXSMLybLeAqpq" && git push -q origin HEAD:main && echo "vydáno $V ($SIZE B)"
