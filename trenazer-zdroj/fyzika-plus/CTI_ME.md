# FYZIKA+ pro trenažér varny – obsah balíčku (verze 1.1, 16. 11. 2026)

| Soubor | Co to je |
|---|---|
| `FYZIKA_PLUS.md` | **Hlavní dokument:** co jsem v programu našel, reálná data z jiných varen a z literatury (se zdroji), co úprava mění, návrhy na další kroky, co změřit na Zajíci, výsledky ověření. Kapitola 7 = doplněk o kapalinách a díle (verze 1.1) |
| `INTEGRACE.md` | Jak úpravu vložit do novější verze: automaticky skriptem, nebo ručně podle tabulky háčků (i zadání pro Clauda doma) |
| `aplikuj_fyzika_plus.py` | Skript, který úpravu vloží: `python aplikuj_fyzika_plus.py "<složka projektu>"` (napřed s `--zkouska`) |
| `fyzika_plus.gd` | Modul s veškerým počítáním (pára a plášť, bojler, přestup tepla, pH, barva, vířivka, trubky, samospád, ledová voda, odpar, rychlost rmutu) |
| `fyzika_plus_vzhled.gd` | Kreslení kapalin na plánku: proudy padající obloukem, vlnky na hladině (výpočet nemění) |
| `fyzika_plus_tests.gd` | 56 nových automatických kontrol (spustí se s `-- --tests`) |
| `proudy_pred_po.png` | Obrázek: sprcha v pánvi před úpravou kreslení a po ní |
| `reference-v70/` | Pro kontrolu: soubory v70 po vložení úpravy a rozdíl `zmeny-v70.diff` proti původní v70 |

Nejrychlejší cesta: stáhni složku (je i v repozitáři `vasil`, `trenazer-zdroj/fyzika-plus/`), spusť
`python aplikuj_fyzika_plus.py "D:\stazene\Trenazer-varny-aplikace" --zkouska`, přečti si zprávu, pak to samé bez
`--zkouska`. Nakonec spusť testy a automatickou várku.

Co je nového proti verzi 1.0 (29. 10.): samospád podle průměru trubky, ubírání ledové vody klapkou, pára do bojleru
stejným modelem jako do pánve, bilance energie při připalování, odpar z horké hladiny mimo var, hlídání rychlosti rmutu
v potrubí, bod varu u dna v panelu a kreslení proudů a vlnek. Kdo má vloženou verzi 1.0, spustí skript znovu a doplní
se jen nové věci.
