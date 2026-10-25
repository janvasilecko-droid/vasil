# Předání: doplnění fyziky a chemie simulátoru varny Zajíc

Datum: 25. 10. 2026 · větev `main` · testy: **Godot (přeložené) 235 / 0 chyb**, jádro 36 / 0, plná automatická várka bez chyb.
Podrobnosti rovnic: [FYZIKA-ROVNICE.md](FYZIKA-ROVNICE.md) · parametry: [KALIBRACE.md](KALIBRACE.md).

## 1. Skutečně nalezené nedostatky (po prověření kódu)

| # | Nedostatek | Důsledek | Stav |
|---|---|---|---|
| 1 | Voda z řadu = pevný limit průtoku (`mainsQ`) vynásobený „faktorem škrcení“; škrcení klapkou se počítalo dvakrát a tlak řadu nikde nebyl | průtok nereagoval na tlak, délku trasy ani na počet odběrů fyzikálně | **opraveno** – řad je zdroj tlaku, řeší se společná výška uzlu (`mains_solve`) |
| 2 | Reakce 1. řádu (nástřik, sacharifikace, α/β-amylasa, kvašení, diacetyl) počítané explicitním Eulerem `min(1, k·Δt)` | výsledek závisel na kroku, při velkém kroku zreagovalo vše naráz | **opraveno** – `1 − e^(−kΔt)`; test 600 × 1 s = 1 × 10 min na 1e-9 |
| 3 | CIP vypadalo jako ověřená čistota („hotovo ✔“, „Sanitace“) | uživatel by mohl považovat simulaci za důkaz hygieny | **opraveno** – hlášení „podmínky postupu splněny – SIMULOVANÝ výsledek“, mikrobiologie se nesimuluje |
| 4 | Využití chmele: konstanta 0,44 vepsaná v kódu | neviditelný odhad | **opraveno** – parametr `iboKeep` s rozsahem a popisem měření |
| 5 | Vířivka: usazování kalů skokem po 3 min víření (časovač) | nerealistické, neplynulé | **opraveno** – plynulá závislost na době víření a klidu |
| 6 | Dokumentace naznačovala rozpor výtěžností 80,7 % vs 62 % | zbytečné pochybnosti | **nebyla chyba** – VV = účinnost × e_lab/m_slad; doplněn test a vysvětlení |
| 7 | Chybí NPSH / kavitace čerpadel | nelze ověřit sání | **neřešeno** – nemáme křivky NPSHr (nevymýšlíme je) |
| 8 | Místní ztráty K = 0,4 na úsek, ráz L = 12 m, křivky čerpadel jsou odhady | hydraulika je řádově správná, ne přesná | **označeno ODHAD** v registru parametrů |

## 2. Změněné a nové soubory

* Změněno: `GodotSrc/sim.gd` (`mains_solve`, `first_order`, `iboKeep`, vířivka, cfg a rozsahy), `GodotSrc/proc.gd` (exponenciály, CIP texty), `GodotSrc/params.gd` (nové parametry), `GodotSrc/audit_tests.gd` (+33 testů), `GodotSrc/updater.gd` (aktualizace v Unity), `.github/workflows/unity-core.yml`, `CoreTests/Program.cs`, `CompileCheck/CompileCheck.csproj`, `Core/Gd/Gd.cs`, `Tools/gd2cs/regen.sh`.
* Nové: `docs/FYZIKA-ROVNICE.md`, `docs/KALIBRACE.md` (generovaný `Tools/gen_calib_doc.py`), `docs/PREDANI.md`, `Core/UpdateLogic.cs`, `Core/PivovarCore.asmdef`, `Tools/gd2cs/PivovarGdGen.asmdef`, `Scripts/GdUnity/UpdateClient.cs`, `Tools/make_update_manifest.py`, `UpdatePack/*.csproj`.
* Přegenerováno: `Assets/Pivovar/GdGen/*` (C# z GDScriptu).

## 3. Nové rovnice a modely

Řad jako zdroj tlaku `H = H_řad·(1 − (Q/Q_max)²)` s paralelními větvemi · přesné řešení reakcí 1. řádu · usazování kalů `k = k_klid + (k_vír − k_klid)(1 − e^(−t/τ))` · parametrizace využití chmele. Beze změny, ale nyní s nezávislým testem: Darcy–Weisbach + Swamee–Jain, křivka čerpadla + afinita, ε-NTU, Arrhenius amylas, Dickenson SMM→DMS, Malowicki izomerizace.

## 4. Parametry, které jsou stále odhad

64 z 88 popsaných parametrů je ODHAD (plný seznam s nejistotou a návodem na měření: KALIBRACE.md). Nové odhady přidané v tomto kole: `mainsBar` (3–5 bar), `iboKeep` (0,3–0,7), `whirlTau`, `trubKstill`, `trubKwhirl`.

## 5. Testy před a po

| | Před | Po |
|---|---|---|
| Godot testy (přeložené, CI) | 202 | **235** (+6 řad, +24 referenčních, +3 vířivka) |
| Jádro (bez Unity) | 23 | **36** (+13 aktualizace) |
| Plná várka (625 min simulace) | bez chyb, VV 62,1 kg/100 kg, účinnost 80,7 %, IBU ≈ 30,4 | beze změny v bilanci (extrakt 538 kg = 538 kg); IBU 30,4 / 30,8 |

Referenční testy porovnávají model s **nezávislým** výpočtem: Hagen–Poiseuille (laminárně), Colebrook (turbulentně), uzavřený ε-NTU, analytické `e^(−kt)`, analytické řešení následných reakcí, průsečík paraboly čerpadla, energetická bilance ohřevu (31,52 vs 31,52 °C), zachování extraktu při odparu (450,00 → 450,00 kg), zachování hmoty při plnění z řadu.

## 6. Limity

* Všechny výsledky jsou ověřeny testy a plnou automatickou várkou v CI, **ne měřením na skutečné varně**. Kde je konstanta ODHAD, výsledek je tak dobrý jako ten odhad.
* **Unity: překlad ověřen v CI** (dotnet proti UnityEngine.Modules), **běh v editoru Unity jsem nespouštěl** – vzhled kapalin, aktualizace i rozdělení do asmdef jsou zatím neviděné. První Play a první .exe sestavení je třeba zkontrolovat.
* Žádný model nepočítá: NPSH/kavitaci, mikrobiologii CIP, přesnou reologii rmutu, plynule se měnící měrné teplo, pěnu podle surfaktantů.

## 7. Co je nutné změřit (v pořadí vlivu)

1. Ohřev pánve: rychlost K/min při známém tlaku páry → `rvpUAst`, `mashFilmK`; chladnutí plné pánve → `rvpUA`.
2. Tlak a průtok řadu: manometr při zavřených odběrech (`mainsBar`), průtok volného výtoku (`mainsQ`).
3. Čerpadla C1–C3: štítek / křivka → `pX_H0`, `pX_Qn`; vnitřní průměry trubek → `bore25/40/50`.
4. Scezování: tlakoměr pod sítem (Δp při známém průtoku) → `skPerm`, `skDpCompact`, `skDpChannel`.
5. Chladič: 4 teploty + 2 průtoky → `UA1`, `UA2`, `plateRefQ`.
6. Chmel: laboratorní IBU z várky → `iboKeep`; DMS z várky.
7. Vířivka: hmotnost kalu a zákal po různé době víření → `trubKwhirl`, `whirlTau`.
8. Kvašení: denní sacharometrie, teplota, diacetyl → parametry Monod modelu.
9. Ověřit exponent Arrhenia amylas (`kdAlpha63`, `kdBeta63`) proti originálu Marc et al. 1983.
