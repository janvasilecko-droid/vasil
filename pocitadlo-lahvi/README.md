# Počítadlo lahví na paletě

Prostředí pro vývoj a ověřování detektoru, který z fotky palety shora spočítá
lahve. Samotná detekce zatím není implementovaná – je tu jen rozhraní, zkušební
fotky a měření přesnosti.

## Příprava

    ./priprav.sh

Vytvoří `.venv`, nainstaluje OpenCV, NumPy a pytest a pustí kontrolu prostředí.

## Co kde je

| Cesta | K čemu |
|---|---|
| `pocitadlo/detektor.py` | Rozhraní `najdi_lahve(obrazek) -> list[Lahev]`; sem přijde detekce |
| `fotky/` | Zkušební fotky palet |
| `fotky/pocty.json` | Skutečné počty spočítané ručně u palety (`null` = ještě nespočítáno) |
| `vyhodnot.py` | Pustí detektor na všechny fotky, porovná s `pocty.json`, do `vystup/` uloží fotky s vyznačenými lahvemi |
| `testy/` | Kontrola, že prostředí funguje |

## Postup

1. Ručně spočítat lahve na vyfocených paletách a doplnit `fotky/pocty.json`.
   Bez toho nejde poznat, jestli program počítá správně.
2. Implementovat `najdi_lahve` a ladit, dokud `vyhodnot.py` nehlásí u všech fotek OK.
3. Přidávat další fotky (jiné světlo, jiné druhy, plné i poloprázdné palety) s jejich počty.
4. Hotový detektor zapojit do aplikace přes stejné rozhraní: fotka dovnitř,
   seznam lahví `{x, y, druh, jistota}` ven. Lahve s nízkou jistotou si aplikace
   nechá potvrdit ťuknutím.
