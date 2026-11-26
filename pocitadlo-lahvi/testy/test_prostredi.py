"""Overuje, ze prostredi je pripravene: knihovny, fotky a soubor s pocty."""

import json
from pathlib import Path

import cv2
import numpy as np
import pytest

from pocitadlo import Lahev, najdi_lahve

FOTKY = Path(__file__).parent.parent / "fotky"
POCTY = json.loads((FOTKY / "pocty.json").read_text(encoding="utf-8"))
SOUBORY = sorted(p.name for p in FOTKY.glob("*.jpg"))


def test_fotky_existuji():
    assert len(SOUBORY) >= 4


@pytest.mark.parametrize("soubor", SOUBORY)
def test_fotka_jde_nacist_a_ma_zaznam_v_poctech(soubor):
    obrazek = cv2.imread(str(FOTKY / soubor))
    assert obrazek is not None and obrazek.ndim == 3
    assert soubor in POCTY


def test_rozhrani_detektoru():
    assert Lahev(1.0, 2.0).jako_dict() == {"x": 1.0, "y": 2.0, "druh": "pet", "jistota": 1.0}
    with pytest.raises(NotImplementedError):
        najdi_lahve(np.zeros((10, 10, 3), np.uint8))
