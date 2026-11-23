# Trenažér varny Zajíc – předávací dokument pro dalšího Clauda
> **STAV 2026-11-16**: vydáno 2D **v70**, 3D **v152** (pH ovlivňuje izomeraci chmele, `isoPh`; základ: aktuální zdroj z `unity/GodotSrc/` v tomto repu, ne ze zálohy v `trenazer-zdroj/*.tar.gz`, ta je starší). **Pracovat vždy z `unity/GodotSrc/`** (`git pull` nejdřív). Pozor: `unity/GodotSrc/updater.gd` je Unity varianta (UnityUpdate) a v Godotu se nezkompiluje – pro Godot pck použít čistý updater.gd (je v záloze `zdroj-2d-godot43.tar.gz`). 3D = stejné skripty + `view3d.gd/tscn` + project.godot (Godot 4.7.2, Forward+). Poslední změny: smyk míchadla (gel β-glukanu, `gelK` ODHAD, Kunze s. 241), usazování ve vířivce 20 min, konec vyslazování 1,0 °P.
> **Pozor (29. 10.):** Vasil má doma novější verzi, než je zdroj v kontejneru a v repu („stop, tady mám starou verzi“). Na staré kopii dál neladit a nic nevydávat; úpravy připravovat jako přenositelný balíček (viz FYZIKA+ níže).
> **Pozor (16. 11.):** Vasil často není u počítače (píše z telefonu). Práce musí jít dělat a převzít z jiného zařízení: výsledky dávat do repa `vasil` a do tohoto projektu, ne jen do konverzace. Chce, aby se pokračovalo automaticky, bez otázek.
Uživatel: Vasil (Pivovar Zajíc, varna 40 hl), komunikuje česky, neformálně, stručně.

## Pravidla od Vasila
- Česky, stručně, autonomně (žádné "zkopíruj si to"), nic nezměňovat bez zadání.
- Nikdy si nevymýšlet data; odhady v kódu/dokumentech označit "ODHAD" (viz PARAMETRY.md: MEASURED/MANUFACTURER/LITERATURE/ESTIMATED).
- Trasy, polohy ventilů a čerpadel neměnit, pokud je sám neupraví. Jeho editor plán (`user://plan_upravy.json`) nepřepisovat (jen cílené patche `_patch_splid`, `_patch_pitch`).
- 2D verze je hlavní (říká, že funguje líp); 3D je kopie na Godot 4.7.2.

## Kde je co
- **Zdroj**: repo `janvasilecko-droid/vasil`, `unity/GodotSrc/` (skripty v70) + kostry projektů v `trenazer-zdroj/zdroj-2d-godot43.tar.gz` a `zdroj-3d-godot47.tar.gz` (starší verze, ale s project.godot, ikonami, čistým updater.gd a 3D soubory). Kontejner se mezi relacemi maže.
- **Vydání**: GitHub repo `janvasilecko-droid/vasil` (větev main): `trenazer2d/trenazer2d.pck` + `version.json`, `trenazer3d/trenazer3d.pck` + `version.json` + `TrenazerVarny3D.zip` (první instalace). Program u Vasila stáhne novou verzi tlačítkem ⟳ Aktualizovat (updater.gd čte version.json).
- **Skripty**: `trenazer-zdroj/publish2d.sh N "poznámka"`, `publish3d.sh N "poznámka" [exe]` (exportují .pck a pushnou do repa; cesty P=, R=, G= upravit). Další verze: 2D **71**, 3D **153**.
- **Vasilův PC**: složka `D:\stazene\Trenazer-varny-aplikace` (Dell Precision 7530, 32 GB).

## Soubory (2D)
main.gd (~4400 řádků, UI/vykreslení/editor/rozvaděč), sim.gd (fyzika: rmutování, scezování Darcy+jerk, chmelovar, vířivka, chlazení, kvašení, CIP), cabinet.gd (rozvaděč, ráz míchadla), editor.gd (editor plánu, hadice DN10/DN15), sight.gd (průhledítko), rotametr.gd, auto.gd + lauter_auto.gd (automat), training.gd, tests.gd + audit_tests.gd, params.gd (zdroj/nejistota parametrů), liquidfx.gd (vzhled kapalin), plan.json (zapojení, flagy `_pitch2`).

## Testy (vždy před vydáním)
- `timeout 600 Godot_v4.3 --headless --path <projekt> -- --tests` → očekává "0 selhání" (v70: 269 kontrol).
- `... -- --autotest --autodt=2` → "done=true failed=''" (várka 10,7 h, bilance extraktu uzavřená).
- Testy nespouštět souběžně ve dvou kopiích projektu: stejný název projektu = stejné `user://`, test uložení pak náhodně selže. Chyba skriptu (parse error) nechá headless Godot viset – vždy `timeout`.
- Snímek obrazovky: `xvfb-run -a -s "-screen 0 1366x768x24" Godot --path . --rendering-driver opengl3 -- --demo --fresh --nopanel --rect=x,y,w,h --shot=soubor.png` (jde i pro 3D projekt v pohledu na plánek).
- Vlastní měření bez úprav programu: `-- --fresh --diag=res://muj.gd` (skript s `var m` a `func run()`).

## Co je hotovo (dřívější relace)
Lupa v rozvaděči; hydraulika i při pauze; otáčky míchadel; rotametr (ideál 20 l/min); pletená nerezová hadice; DN10/DN15; libovolné přiškrcování klapek (slider, Ctrl+kolečko, tažení); ráz po zastavení míchadla + RESET; trhání mláta klapkou (3–4 trhnutí smyje nános, pak trhá lože); realistické trubky s koleny/příchytkami; připnuté průhledítko se sladem; spilková hadice s průhledítkem/T/vývodem; víčko spilky (tlak při zavření); vířivka usazení 20 min (Kosař/Kunze); konec vyslazování 1,0 °P (Kosař s. 147/156).

## FYZIKA+ (verze 1.6 z 2026-11-17, NEVYDÁNO – Vasil si to vloží sám doma)
Vasil chtěl projít fyziku, chemii a mechaniku (kapaliny, přenos tepla), najít reálná data z jiných varen a připravit úpravu,
kterou si vloží do své **novější verze doma**. 16. 11. k tomu přidal: „ať je voda kapalina a chová se realisticky, i dílo“.
Hlavní dokument: `claude/FYZIKA_PLUS.md` v tomto projektu (kapitola 7 = doplněk 1.1, kapitola 8 = doplněk 1.2).
- **Kde to je:** repo `janvasilecko-droid/vasil`, složka `trenazer-zdroj/fyzika-plus/` + `trenazer-zdroj/fyzika-plus.zip`.
- **Co to je:** moduly `fyzika_plus.gd` (výpočet) a `fyzika_plus_vzhled.gd` (kreslení) + krátké háčky do sim.gd, ctl.gd, main.gd,
  auto.gd, lauter_auto.gd, tests.gd, audit_tests.gd a params.gd. Vkládá je `aplikuj_fyzika_plus.py "<projekt>" [--zkouska] [--bez-ui] [--bez-vzhledu]`
  podle textu, ne čísel řádků. Háček vloží, jen když místo najde přesně jednou (některé jen uvnitř dané funkce); skupiny všechno,
  nebo nic; zálohy `*.pred-fyzika-plus`; seznam vložených háčků zapíše do `fyzika_plus_hooks.gd`. Jde spustit opakovaně,
  i přes verzi 1.0 nebo 1.1 a přes ručně vloženou část háčku.
- **Obsah 1.0 (29. 10.):** pára do pláště přes ventil (Spirax; ekviprocentní R 30 ODHAD; Kvs dopočítané), tlak v plášti z rovnováhy
  ventil = kondenzace, vzduch (Dalton), zavzdušňovací ventil a podtlak (Kunze s. 290–291), stall kondenzátu (TLV);
  přestup na straně náplně (CDJ, viskozita, přirozená konvekce `hNatC` 0,28); pH mladiny (pokles za varu 0,08/h,
  tlačítko kyseliny mléčné do mladiny, pufr podle Kunzeho s. 229); barva Maillard (1,2 EBC/h, Ea 77 kJ/mol, pH, °P) ve všech
  nádobách; vířivka (nátok ≤ 3,5 m/s, Kunze s. 341); chladnutí trubek podle průměru; Kell; IAPWS-IF97 pro páru.
- **Obsah 1.1 (16. 11.):** H10 pára do topného hadu bojleru stejným modelem; H11 průtok ledové vody podle přiškrcení klapek
  (`lvPumpQ`; teplota mladiny se v praxi řídí ledovou vodou, Kosař s. 186); H12 odpar z horké hladiny mimo var (ASHRAE,
  `evapOpen` 0,1 ODHAD); H13 rychlost rmutu v potrubí nad 1,5 m/s (Kunze s. 903) zhoršuje scezování; H14 připalování bez
  ztráty energie v bilanci (značka přes `set_meta`); H15 samospád podle hydrauliky trasy (`path_head` + výtoková ztráta);
  U3 řádky v panelu (chladič, bod varu u dna); V1/V2 kreslení proudů obloukem a vlnky na hladině.
- **Obsah 1.2 (16. 11. odpoledne, na pokyn „udělej to“):** H16 stojící odstředivé čerpadlo je průchozí oběma směry
  (`FP.idle_pumps` → `bfs(…, pass2)` se značkou `"~rev"`; ztráta K = 5, GIW/KSB; doběh 3 s skutečného času – `pump_grace`
  násobí rychlostí hry; čerpadlo v `auto.pend` se bere jako běžící); kvůli tomu automat v `_end_step()` volá `_close_all()`
  a H16b zavírá klapky po skončení automatu scezování (`FP.lauter_stop`). Vypnutí: `pumpPass` = 0. H17 ztráty tepla pánve
  a vířivky podle zaplnění (`FP.loss_k`, `dryWallK` 0,5 ODHAD; 1 = jako dřív). V3 kreslení usazeného díla (`S.clog`).
  **Vasil 16. 11.: zpětné klapky za čerpadly na Zajíci nejsou** → `pumpPass` = 1 platí (v params.gd označeno STATED).
  Jak jsou nádoby izolované nad hladinou, nevím (`dryWallK` je ODHAD) – zeptat se, až na to přijde řeč.
- **Obsah 1.3 (17. 11., na pokyn „pokračuj“ k nabídce):** H18 výpust do kanálu otevřeným koncem trubky i přes stojící
  čerpadlo (smyčka `for v in net.gravity:` dostane `bfs([s0], "liq", fp_idle)` a klíč mezipaměti se seznamem stojících
  čerpadel); `gravity_q` rozliší přeskok trasy přes čerpadlo (K·v²/2g) a přes deskový chladič (4·(Q/200)² m).
- **Obsah 1.4 (17. 11. odpoledne, „pokračuj“):** H19 rozpad kalového kužele ve vířivce při rychlém odtahu (`FP.wp_cone` ve
  `srcTake`, geometrie `cone_geo`/`cone_level`, `wpConeV` a `wpConeBreak` ODHAD, mezikruží 60 cm Kosař s. 181); automat
  v kroku chlazení omezí C3 (`auto_cone_flow`) a přes meta `fpIceK` (platí 5 s) ubere ledovou ve stejném poměru;
  protokol `kal_do_spilky_mg_L`. Kyslík (N6) záměrně neudělán – bez dat o kvašení by byl vymyšlený.
  Pozor v GDScriptu: `get_meta(klíč, null)` s null hází chybu, když klíč chybí – používat `has_meta`.
- **Obsah 1.5 (17. 11. večer, „pokračuj“):** H20 kyslík v zákvasné mladině podle teploty a stupňovitosti
  (`FP.o2_dose`; dřív natvrdo 8 mg/L, teď 8,43 mg/L při 7,8 °C a 12,8 °P – rozpustnost ve vodě podle tabulky APHA,
  vytěsnění extraktem `o2PlatoK` 0,017/°P ODHAD, účinnost provzdušnění `aerEff` 0,9 ODHAD); protokol `O2_mg_l`.
  Na výsledek várky to zatím nemá vliv – kyslík ve spilce nic neřídí, je to zpřesnění vstupu a ukazatel pro obsluhu.
  Zbytek N6 (vystírání shora, vliv na množení kvasnic) zůstává nevyřízený jako N6b: chybí rozlišení horního
  a spodního nátoku v plánku a čísla o kvašení.
- **Obsah 1.6 (17. 11. večer, „vypočti ty údaje, jak by se to chovalo v reálu“; souběžně jiná relace vydala 1.5 s kyslíkem, sloučeno):** `VYPOCTY.md` v balíčku. K1 přepíše
  v `CFG_DEFAULT` původní odhady vypočtenými (rvpUA 0.25→0.02, mpUA 0.3→0.022, wpUA 0.35→0.02, skUA 0.2→0.02,
  lvUA 0.15→0.01, UA1 25→50) – jen když tam ještě jsou původní čísla; `dryWallK` 0,95. A1: automat počítá velikost
  dílčích rmutů z tepelné bilance (`FP.deco_frac`, cíle `decoT1` 63 a `decoT2` 76 °C, Kosař s. 143; bez toho by
  s malými ztrátami dílo mělo 73 a 88 °C). `autoIce`: v kroku chlazení `ice_flow` omezí ledovou na průtok pro `pitchT`
  (`ice_for_target`, stejné vztahy jako `plate_cool`) a `auto_cone_flow` přidává mladinu, dokud ledová stačí.
  Přepínač `--bez-vypoctu` vynechá K1 a A1. Předpoklad: nádoby jsou izolované (neověřeno u Vasila).
- **Ověřeno:** viz kapitoly 7.4, 8.3, 8.5–8.7 dokumentu (1.6: v70 2D i v152 3D 346 kontrol bez selhání, starší archivní
  verze 279, přechod z 1.0, 1.3 i 1.4; CRLF, tabulátory a ručně vložená půlka háčku zkoušené u 1.2/1.3; automatická várka 2D = 3D).
  Verze 1.0 a 1.1 prošly nezávislou kontrolou (druhý agent bez znalosti postupu), nálezy jsou opravené; 1.2–1.6 jen vlastní kontrolou a testy.
  **Na Vasilově novější verzi vyzkoušené není** (je jen na jeho PC; až ji nahraje do repa, ověřit tam).
- **POZOR 17. 11.: Vasilova aktuální verze není v70, ale jiný program** – složka `C:\Users\Uzivatel\Desktop\Trenazer varny`
  (Godot 4.3, `scripts/sim.gd` nad trasami – třída BrewSim, `net_flow.gd`, `plant_3d.gd`, checklist 43 kroků, vlastní testy
  v `tests/` spouštěné `Godot --headless --path . -s tests/x_test.gd`, celá várka `-- --mode=chkauto`). Řekl: „dával jsi to do
  staré verze, vše implementuj do aktuální, je to složka na ploše“. Vkládací skript tam nejde použít (jiná stavba kódu),
  přenáší se ručně po částech. Rozpracováno v `trenazer-zdroj/nova-verze-prenos/` (CTI_ME.md tam): 1. část (ztráty
  izolovaných nádob + odpar mimo var) a 2. část (jednostupňový chladič) jsou od 20. 11. zapsané na jeho PC v kopii
  `Desktop\\Trenazer varny FYZIKA+` (původní složka beze změny; řekl „až to bude, ulož na plochu složku s tím programem“).
  **Od 20. 11. 23:10 je vše v jediné složce `Desktop\\Trenazer varny`** (kopie zrušena, zálohy v `zaloha-pred-fyzika-plus`);
  nová várka začíná s bojlerem 4000 L (jinak automat stál na kroku 4). Viz `nova-verze-prenos/CTI_ME.md`.
  **23. 11. 16:50: 3. část (commit 15a50fb) ZAPSÁNA NA PC** do `Desktop\\Trenazer varny` (soubory níže; předchozí stav v `zaloha-pred-fyzika-plus/cast2`,
  původní `checklist_panel.gd` v `zaloha-pred-fyzika-plus/scripts`). Prázdná složka `Trenazer varny FYZIKA+` na ploše zbyla (smazat ručně).
  Obsah: chladnutí stojících trubek, kalový kužel (mez 34 L/min skutečných = 168 v programu, automat u kužele zpomalí),
  řízení ledové vody v automatu (`sim.ice_auto`), varování na rychlost rmutu > 1,5 m/s; vše z průtoku programu ÷ 5.
  Zapsáno: `scripts/fyzika_plus.gd`, `scripts/sim.gd`, `scripts/checklist_panel.gd`, `tests/fyzika_plus_test.gd`,
  `FYZIKA-PLUS-PRENOS.md` (`plan_edited.json` nedotčen). ODHADY: podíl kalu 0,6 %,
  strhne se max. 10 % kalu, trubky Ø 50 mm. Celá várka podle checklistu (`--mode=chkauto`) NEOVĚŘENA – v kontejneru
  za 2 h nedoběhla (jen start programu trvá minuty); pustit na jeho PC přes `Zkouska cele varky.bat`.
  **23. 11. 18:15 oprava kroku 4 podruhé:** tlačítko AUTOMATICKÁ VÁRKA (`panel_puvodni.gd`) spouští `_start_auto(0, false)`,
  tedy bez `new_batch()`, takže bojler zůstal prázdný (displej TEPLOTA BOILER „---“). `checklist_panel._start_auto` teď
  bojler napustí (4000 L), když je prázdný a automat začíná od kroku ≤ 4. Zapsáno na PC, program restartován; jestli projde, neověřeno
  (okno trenažéru bylo na obrazovce zakryté jiným oknem, ovládání přes obrazovku nešlo dokončit).
  **23. 11. 18:35 – celá várka poprvé spuštěna na jeho PC** (`Zkouska cele varky.bat`; režim `chkauto` běží jen s `--capture=…`,
  bat opraven; výsledek v `chkauto-vysledek.txt`, trvá ~2 min). Opravy: (a) Vasil: „na páru se čekat nemusí“ → bojler na začátku
  várky 4000 L / 80 °C (`sim_panel.new_batch`, `checklist_panel._start_auto`); (b) automat checklistu neotvíral porty rozbočovače
  (`sim_panel.set_manifold_ports_for` volané z `checklist_navrh._apply_set`) – voda z bojleru + řadu netekla do kotle (krok 5).
  Stav: projde 25 ze 43 kroků, **stojí na kroku 26 (2. rmut ze SK do varny)**: cesta `rmut_varna` v plánu odkazuje na trubku
  `pipe_mur91to7`, kterou Vasil 20. 11. v editoru smazal a nakreslil novou `pipe_new_1795204313` (konec [630,680] zřejmě nenavazuje);
  sání C1 se ke SK dostane jen přes klapky cesty rmut_sk. Plán je jeho – neměnit bez pokynu (záloha plánu ze 17. 11. je
  v `zaloha-pred-fyzika-plus/plan_edited-17-11.json`). Pozor: testy v kontejneru čtou plán z `user://plan_edited.json`,
  jinak berou starý `plan.json` a výsledky sítě neodpovídají jeho PC.
  **Oprava diagnózy kroku 26 (23. 11. 18:40):** smazaná trubka příčinou není – s plánem ze 17. 11. stojí várka stejně (zkoušeno
  na jeho PC, pak vrácen jeho plán z 20. 11.). V obou plánech nemá C1 sání ze SK: s klapkami cesty `rmut_varna` čerpadlo „nesaje“,
  po otevření `valve_mur96jdk` + `valve_mur96n43` teče opačně (rvp → sk). Chybí tedy potrubí/klapky pro 2. rmut SK → C1 → kotel;
  nová trubka a klapka z 20. 11. je nejspíš jeho rozdělaná oprava. Potrubí nevymýšlet – musí říct/nakreslit, kudy 2. rmut na Zajíci teče.
  Přístup do `AppData\Roaming\Godot\app_userdata\Trenažér varny Zajíc — vzhled + editor` (odtud program čte plán) povolen v této relaci.
  **23. 11. 18:55 – CELÁ VÁRKA PROJDE (43/43 kroků, 6:57 h; spilka 3572 L, 12,3 °P, 8,5 °C, kal 26 %)** na jeho PC se všemi opravami.
  Poslední dvě příčiny: (1) nová trubka `pipe_new_1795204313` končila o 10 j. vedle trubky, na kterou navazuje ([630,680] místo
  [630,690]) – po jeho svolení ("ty to udelej") opraveno jedním číslem v `plan_edited.json` (záloha před úpravou:
  `zaloha-pred-fyzika-plus/plan_edited-pred-opravou-konce-trubky.json`); (2) cesta `virivka_chladic` nemá v plánu úsek přes
  rozbočovač, takže automat neotevřel porty L1/P1 → krok 38 stál; `sim_panel.ROUTE_MANIFOLD` je doplní (hodnoty z `net_fix_routes.gd`).
  Ověřit: kal 26 % ve spilce se zdá vysoký (train_test dával zákal 9 %) – není vyšetřeno, jestli to je kužel z FYZIKA+ nebo původní model.
  Zbývá přenést: stojící čerpadlo v `net_flow.gd`, pH a barva mladiny, nátok do vířivky, kreslení.
  Dřívější stav: 1. část **na jeho PC zapsaná nebyla** (spojení s PC
  během práce spadlo; `device_bash` tam nejde – málo místa na disku, jde jen stage/commit souborů). `chkauto` se
  v kontejneru nepodařilo dojet ani s původním kódem. Další části a pořadí: `FYZIKA-PLUS-PRENOS.md`. Chladič je tam
  dvoustupňový (řad + ledová), ale **Vasil 17. 11. potvrdil, že na Zajíci je jednostupňový**: voda jde do ledovky, tam ji
  vychladí glykol, a do deskového chladiče jde jen ledová voda. V aktuální verzi tedy zrušit stupeň s vodou z řadu.
  Upřesnění: voda z řadu se cestou do ledovky předchladí, ale to nestačí – v ledovce se pak okruhem přes glykolový chladič
  dochlazuje na 2 °C (stejně jako ve staré verzi: napouštění z řadu + cirkulace lv → C5 → glykol → lv).
- **Nevydávat** přes ⟳ Aktualizovat bez jeho souhlasu. Zbylé návrhy (N3–N24) a seznam měření jsou ve FYZIKA_PLUS.md.
- **Knihy – číslování stran:** Kunze (sken, OCR): tištěná strana = PDF − 1 do PDF 257, PDF + 2 pro 259–646, PDF + 3 od 647.
  Kosař–Procházka: PDF strana = tištěná. Autor je Kosař (v názvu souboru „Kosa“).
- **Obnova prostředí po restartu kontejneru:** Godot 4.3 z `github.com/godotengine/godot/releases/download/4.3-stable/Godot_v4.3-stable_linux.x86_64.zip`,
  4.7.2 z `github.com/godotengine/godot-builds/releases/download/4.7.2-stable/…`. Základ v70 2D = kostra z `zdroj-2d-godot43.tar.gz`
  + všechny `.gd` z `unity/GodotSrc/` kromě `updater.gd`; základ v152 3D = kostra z `zdroj-3d-godot47.tar.gz` + tytéž `.gd` kromě
  `updater.gd`, `boot.gd` a `view3d.gd` (ty jsou ve 3D jiné). Kontrola: v70 dává 269 kontrol, 0 selhání.
  Starší verze balíčku pro zkoušku přechodu: `git archive 43c8ced` (1.0), `a4db14a` (1.1), `4aade71` (1.2), `6a4b19b` (1.3) a `7bf4781` (1.4), složka `trenazer-zdroj/fyzika-plus`.

## Zdroje čísel
- `KOSA_KUNZE_PARAMETRY.md` (i v projektu Zajic na claude.ai): čísla z Kosař–Procházka (Technologie výroby sladu a piva) a Kunze (Technology Brewing and Malting, s. 255–356) se stránkami.
- Sim už sedí s knihami: tříslověny (76 °C, pH 5,7), DMS po 30 min varu, jodové barvy, prokvašení 76 %; z kontroly 16. 11. i voda zadržená v mlátě, viskozita sladiny, měrné teplo díla, teplota vystírky (viz FYZIKA_PLUS.md, 7.1).

## Otevřené / nápady (Vasil zatím neschválil)
- Okyselení mladiny na pH 5,1–5,2 před koncem varu (potřeba tlačítko dávkování kyseliny). → Je v balíčku FYZIKA+ (nevydáno).
- Odvzdušnění parního pláště a podtlak (potřebuje změnu zapojení páry). → FYZIKA+ to modeluje parametry (`jVacBreaker`, odvzdušňovač) bez změny zapojení.
- Nátok do vířivky ≤ 3,5 m/s (neznámý průměr vtoku – nevymýšlet). → FYZIKA+: `wpInletMM` 0 = průměr trubky z plánku; skutečné hrdlo změřit.
- Odhady k doptání: délka potrubí scezování 12 m, prahy 80/160 mbar (skDpCompact/skDpChannel), jednotka rotametru l/min; nově průměry nádob, tvar dna, průtok a škrcení ledové vody, typ čerpadel (NPSH).
- Dále z původního seznamu: přepěnění po chmelení, lom + zanášení deskového chladiče/zpětný proplach, měření (refraktometr, pH, jod co 5 min), využití chmele podle času/teploty, CIP sekvence, poruchy, bezpečnost (LOTO, horké povrchy, chemie).

## ÚKOL: pokračovat ve čtení Kunzeho (Wolfgang Kunze, Technology Brewing and Malting, 946 s., sken)
Vasil chce, aby se z obou knih (Kosař–Procházka i Kunze) vzalo **všechno, co může simulátor udělat realističtější** ("projdi celé knihy").
- **Stav (2026-10-25)**: Kunze má i celé OCR (946 stran, jen lokálně v kontejneru a v přílohách konverzace, ne v repu). Přečteno: rmutování (s. 234–250), provzdušnění, řízení, bezpečnost (354–361) a dříve kapitoly o varně: scezování (s. 255–272), chmelovar a odpar (281–291, 322–325), výtěžek (323–325), vířivka a separátory (341–344), chladič (350–356). Výstupy jsou v `KOSA_KUNZE_PARAMETRY.md`. Kosař–Procházka (327 s.) je projitá pro varnu (kap. 11–18, 20); kvašení (kap. 21) a zbytek ne.
- **Nepřečteno u Kunzeho**: 3.1 šrotování (s. 196–213), 3.4.2–3.4.3 konstrukce varny a postup chmelovaru (s. 289–322), 3.8 chlazení a provzdušnění (347–355), 3.10 řízení, **3.11 bezpečnost práce při výrobě mladiny (s. 359–366)**; kap. 1 suroviny (chmel, voda, kvasnice, s. 32–95), kap. 4 kvašení a filtrace (od s. 367) a další kapitoly (CIP, energie, laboratoř, automatizace).
- **Jak číst**: PDF je sken bez textu a je příliš velký pro Read (>100 MB). Postup: `pdftoppm -r 95 -f N -l M -png Kunze.pdf x` a pak Read na PNG stránky. OCR text ber jen jako pomůcku, čísla ověř proti obrázku stránky nebo aspoň proti číslu strany v OCR.
- **Autorská práva**: text knih (ani OCR) nedávat do repa ani do projektu, jen vlastní shrnutí čísel se stránkou, bez dlouhých citací.
- **Postup**: projít kapitoly, vypsat čísla a jevy do `KOSA_KUNZE_PARAMETRY.md`, porovnat se `sim.gd`/`params.gd`, navrhnout Vasilovi nové jevy a po jeho "ano" je implementovat s testem. Čísla nahrazující odhad přepsat v `params.gd` na LITERATURE se stránkou.

## 23. 11. večer – chemie varu (pH sladiny + Maillardova barva)
- `fyzika_plus.gd`: `wort_ph` (pH rmutu +0,05, −0,08/h varu), `color_rate_ebc_h` (1,2 EBC/h při 100 °C, pH 5,5, 12 °P; Arrhenius Ea 77 kJ/mol; ×exp(0,7(pH−5,5)); pod 70 °C nic), `color_step`, `beer_color_ebc` (0,65×). Koeficienty obecné literární (VLB/Kunze) = ODHAD pro Zajíce.
- `sim.gd`: `mash_ph`, `wort_col`; hák v `step`. `rozvadec_rt20.gd` předává `mash_ph`. `sim_panel.gd` ukazuje pH a barvu při varu.
- Test `fyzika_plus_test` 13 kontrol OK. Nasazeno na PC; celovarka po změně zatím neověřena na PC.
