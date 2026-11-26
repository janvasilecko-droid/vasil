"""Rozhrani detektoru lahvi. Samotna detekce zatim neni implementovana.

Smlouva, kterou bude pouzivat aplikace:
    vstup:  fotka palety shora jako pole BGR (tak ji nacte cv2.imread)
    vystup: seznam nalezenych lahvi, jedna polozka = jedna lahev
"""

from dataclasses import asdict, dataclass

import numpy as np

DRUHY = ("pet", "sklo")


@dataclass(frozen=True)
class Lahev:
    x: float  # stred vicka v pixelech fotky
    y: float
    druh: str = "pet"  # "pet" nebo "sklo"
    jistota: float = 1.0  # 0-1; nizka hodnota = aplikace si vyzada potvrzeni

    def jako_dict(self) -> dict:
        return asdict(self)


def najdi_lahve(obrazek: np.ndarray) -> list[Lahev]:
    """Vrati vsechny lahve nalezene na fotce."""
    raise NotImplementedError("Detekce lahvi jeste neni implementovana.")
