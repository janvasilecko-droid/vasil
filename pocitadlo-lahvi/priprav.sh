#!/usr/bin/env bash
# Pripravi prostredi: vytvori .venv a nainstaluje zavislosti.
set -euo pipefail
cd "$(dirname "$0")"
python3 -m venv .venv
.venv/bin/pip install --quiet --upgrade pip
.venv/bin/pip install --quiet -r requirements.txt
.venv/bin/python -m pytest -q testy
echo "Prostredi je pripravene. Vyhodnoceni: .venv/bin/python vyhodnot.py"
