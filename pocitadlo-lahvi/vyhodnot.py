#!/usr/bin/env python3
"""Pusti detektor na vsechny fotky ve slozce fotky/ a porovna vysledek se
skutecnymi pocty z fotky/pocty.json. Fotky s vyznacenymi lahvemi ulozi do vystup/.

Navratovy kod 0 jen kdyz pocty sedi presne na vsech fotkach se znamym poctem.
"""

import json
import sys
from pathlib import Path

import cv2

from pocitadlo import najdi_lahve
from pocitadlo.detektor import DRUHY

KOREN = Path(__file__).parent
FOTKY = KOREN / "fotky"
VYSTUP = KOREN / "vystup"
BARVY = {"pet": (0, 255, 0), "sklo": (255, 128, 0)}


def main() -> int:
    pocty = json.loads((FOTKY / "pocty.json").read_text(encoding="utf-8"))
    VYSTUP.mkdir(exist_ok=True)
    vse_sedi = True
    overeno = 0

    for soubor in sorted(FOTKY.glob("*.jpg")):
        obrazek = cv2.imread(str(soubor))
        if obrazek is None:
            print(f"{soubor.name}: fotku nejde nacist")
            vse_sedi = False
            continue
        try:
            lahve = najdi_lahve(obrazek)
        except NotImplementedError as chyba:
            print(f"{soubor.name}: {chyba}")
            return 2

        r = max(6, obrazek.shape[1] // 120)
        for i, lahev in enumerate(lahve, 1):
            stred = (round(lahev.x), round(lahev.y))
            cv2.circle(obrazek, stred, r, BARVY.get(lahev.druh, (0, 0, 255)), 3)
            cv2.putText(obrazek, str(i), (stred[0] + r, stred[1]), cv2.FONT_HERSHEY_SIMPLEX,
                        r / 20, (255, 255, 255), 2)
        cv2.imwrite(str(VYSTUP / soubor.name), obrazek)

        ocekavano = pocty.get(soubor.name, {})
        casti = []
        for druh in DRUHY:
            nalezeno = sum(1 for lahev in lahve if lahev.druh == druh)
            skutecne = ocekavano.get(druh)
            if skutecne is None:
                casti.append(f"{druh} {nalezeno} (skutecny pocet neznamy)")
            else:
                overeno += 1
                sedi = nalezeno == skutecne
                vse_sedi &= sedi
                casti.append(f"{druh} {nalezeno}/{skutecne} {'OK' if sedi else 'CHYBA'}")
        print(f"{soubor.name}: " + ", ".join(casti))

    if overeno == 0:
        print("Zadny skutecny pocet neni vyplnen, presnost nejde overit.")
        return 1
    return 0 if vse_sedi else 1


if __name__ == "__main__":
    sys.exit(main())
