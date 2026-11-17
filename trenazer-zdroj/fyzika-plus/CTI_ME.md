# FYZIKA+ pro trenažér varny – obsah balíčku (verze 1.6, 17. 11. 2026)

| Soubor | Co to je |
|---|---|
| `FYZIKA_PLUS.md` | **Hlavní dokument:** co jsem v programu našel, reálná data z jiných varen a z literatury (se zdroji), co úprava mění, návrhy na další kroky, co změřit na Zajíci, výsledky ověření. Kapitola 7 = kapaliny a dílo (verze 1.1), kapitola 8 = stojící čerpadlo, ztráty podle hladiny a usazené dílo (verze 1.2) |
| `VYPOCTY.md` | **Vypočtené hodnoty místo odhadů:** ztráty izolovaných nádob, deskový chladič a další, s postupem výpočtu a porovnáním s knihami |
| `INTEGRACE.md` | Jak úpravu vložit do novější verze: automaticky skriptem, nebo ručně podle tabulky háčků (i zadání pro Clauda doma) |
| `aplikuj_fyzika_plus.py` | Skript, který úpravu vloží: `python aplikuj_fyzika_plus.py "<složka projektu>"` (napřed s `--zkouska`) |
| `fyzika_plus.gd` | Modul s veškerým počítáním (pára a plášť, bojler, přestup tepla, pH, barva, vířivka, trubky, samospád, stojící čerpadlo, ledová voda, odpar, ztráty podle hladiny, rychlost rmutu) |
| `fyzika_plus_vzhled.gd` | Kreslení kapalin na plánku: proudy padající obloukem, vlnky na hladině, usazené dílo (výpočet nemění) |
| `fyzika_plus_tests.gd` | 77 nových automatických kontrol (spustí se s `-- --tests`) |
| `proudy_pred_po.png` | Obrázek: sprcha v pánvi před úpravou kreslení a po ní |
| `usazene_dilo.png` | Obrázek: dílo v pánvi s míchadlem, 90 s a 5 min bez míchání |
| `reference-v70/` | Pro kontrolu: soubory v70 po vložení úpravy a rozdíl `zmeny-v70.diff` proti původní v70 |

Nejrychlejší cesta: stáhni složku (je i v repozitáři `vasil`, `trenazer-zdroj/fyzika-plus/`), spusť
`python aplikuj_fyzika_plus.py "D:\stazene\Trenazer-varny-aplikace" --zkouska`, přečti si zprávu, pak to samé bez
`--zkouska`. Nakonec spusť testy a automatickou várku.

Co je nového ve verzi 1.6 (17. 11.): místo odhadů jsou vypočtené hodnoty (ztráty izolovaných nádob 0,02 kW/K místo
0,25–0,35, deskový chladič 50 kW/K místo 25). Mladina pak jde na chladič s 95 °C a ohřátá voda do bojleru má 85 °C,
jak uvádějí knihy. Automat při chlazení řídí i ledovou vodu a velikost dílčích rmutů počítá z tepelné bilance
(30 % a 44 % díla místo poloviny a dvou třetin), takže dílo má po rmutech 63 a 76 °C. Postup je ve `VYPOCTY.md`; původní hodnoty nechá přepínač
`--bez-vypoctu`.

Verze 1.5 (17. 11. večer): kyslík v zákvasné mladině podle teploty a stupňovitosti (dřív natvrdo 8 mg/L), protokol `O2_mg_l`.

Verze 1.4 (17. 11. odpoledne): kalový kužel ve vířivce se rozpadá, když se konec stahuje moc rychle,
a kal jde do spilky. Automat na konci stahování zpomalí C3 a ubere ledovou. Protokol ukazuje kal ve spilce v mg/L.

Verze 1.3 (17. 11. ráno): otevřeným koncem trubky za stojícím čerpadlem vytéká nádoba do kanálu
(dřív stojící čerpadlo nepustilo nic). Jinak se nic nemění.

Co přidala verze 1.2 (16. 11. odpoledne):
- **Stojící čerpadlo je průchozí.** Otevřenou trasou přes vypnuté čerpadlo teče kapalina podle rozdílu hladin, i zpátky.
  Po čerpání se proto zavírá klapka; automat i automat scezování to dělají samy. Odpovídá to varně bez zpětných
  klapek za čerpadly, jak jsi potvrdil 16. 11. (parametr `pumpPass` = 1; hodnota 0 by to vypnula).
- **Ztráty tepla podle hladiny.** Poloprázdná pánev a vířivka ztrácejí míň tepla než plné (parametr `dryWallK`).
- **Usazené dílo na plánku.** Bez míchání se nahoře oddělí čirá kapalina a u dna zůstane vrstva zrna.

Verze 1.1 přidala: samospád podle průměru trubky, ubírání ledové vody klapkou, páru do bojleru stejným modelem jako
do pánve, bilanci energie při připalování, odpar z horké hladiny mimo var, hlídání rychlosti rmutu v potrubí, bod varu
u dna v panelu a kreslení proudů a vlnek. Kdo má vloženou starší verzi, spustí skript znovu a doplní se jen nové věci.
