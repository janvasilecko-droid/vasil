# FYZIKA+ v tomhle programu – co je přeneseno a co zbývá

Stav 17. 11. 2026. Balíček FYZIKA+ (repozitář `vasil`, `trenazer-zdroj/fyzika-plus/`) vznikl pro starší trenažér
(v70 / v152). Tenhle program má jinou stavbu (`scripts/sim.gd` nad trasami, `net_flow.gd`, checklist 43 kroků),
takže vkládací skript tu nejde použít. Přenáší se ručně po částech, každá část s testem v `tests/`.

Od 20. 11. večer je všechno v jediné složce `Trenazer varny` na ploše (chtěl jsi jen jednu). Původní soubory z doby před
FYZIKA+ jsou v `zaloha-pred-fyzika-plus/` (skripty s příponou `.txt`, plán z 17. 11. jako `plan_edited-17-11.json`).

**Oprava 20. 11.:** nová várka začíná s bojlerem napuštěným 4000 L studené vody (`sim_panel.new_batch`). Dřív byl prázdný,
žádná cesta ho nenapouštěla a automat stál na kroku 4. Množství je moje volba.

## Hotovo

### 1. část – tepelné ztráty a odpar (`scripts/fyzika_plus.gd`, test `tests/fyzika_plus_test.gd`)

- **Ztráty nádob podle výpočtu.** Dřív chladly všechny nádoby stejně (časová konstanta 12 h), u plné pánve to dělalo
  ~0,39 kW/K – jako holá nerez. Teď izolované nádoby: pánev, vířivka a scezovací káď 0,02 kW/K, bojler 0,015,
  ledovka a spilka 0,01. Do tepelné kapacity se počítá i šrot a ocel nádoby.
- **Při ohřevu se ztráty neodečítají podruhé** (`LOSS_KW_PER_K` v `_heat` už se nepoužívá).
- **Odpar z horké hladiny mimo var** (pánev, vířivka, scezovací káď): vzorec ASHRAE, zavřená nádoba s odtahem bere
  desetinu volné hladiny. Voda ubývá a nádoba se tím ochlazuje. Součet je v `sim.evap_idle_l`.
- Vířivka 38 hl z 98 °C má po 20 min 97,5 °C a po 80 min 96,2 °C (knihy: mladina přichází na chladič s 95–97 °C;
  dřív by spadla o 8 K).

### 2. část – deskový chladič jen s ledovou vodou (`sim.plate_cool`, testy `plate_test.gd`, `calibration_test.gd`)

- Podle tebe (17. 11.): voda z řadu mladinu přímo nechladí. Předchladí se cestou do ledovky, tam se okruhem přes glykol
  dochladí na 2 °C a do chladiče jde jen ledová voda. Stupeň „voda z řadu“ je proto pryč.
- Bez ledové vody (C5 stojí nebo je ledovka prázdná) se mladina nechladí vůbec; dřív ji voda z řadu srazila na ~20 °C.
- `UA_ICE` je 207 kW/K (dřív 76 za stupněm s vodou z řadu), aby při C3 200 L/min a ledové 3 °C vycházelo pořád
  ~8,5 °C. Ledová voda se vrací do bojleru s 89 °C (knihy: 80–88 °C).
- Kalibrace: platí `UAice`; `UA1` se přijme, ale nepoužívá.

### 3. část – 21. 11. (test `tests/fyzika_plus_test.gd`)

Program čerpá 5× rychleji než skutečnost (`FLOW_SCALE`). Všechno níže se proto počítá z průtoku ve skutečném měřítku
(průtok v programu / 5), takže meze z knih platí beze změny.

- **Chladnutí stojících trubek.** Voda 90 °C v trubce má po 1,5 h asi 46 °C (holá nerez Ø 50 mm). Dřív si trubky držely
  teplotu donekonečna.
- **Kalový kužel ve vířivce.** V posledních asi 240 L (výška kužele 4 cm) se kužel rozpadá, když hladina klesá rychleji než
  0,6 cm/min – to je 34 L/min ve skutečnosti, tedy 168 L/min v programu. C3 naplno (200 L/min v programu) je nad mezí:
  část usazeného kalu jde do spilky a program to ohlásí. Podíl kalu v kuželu (0,6 % objemu) a podíl strženého kalu
  (nejvýš 10 %) jsou ODHAD.
- **Automat u kužele zpomalí** odběr pod mez, takže při automatické várce kužel drží.
- **Řízení ledové vody (jen automat).** Automat pustí chladičem jen tolik ledové, aby mladina šla do spilky s 8,5 °C.
  Při stejném průtoku mladiny a ledové (200/200) to vychází naplno; projeví se, když mladina teče pomaleji nebo je
  ledová studenější. Ručně je průtok ledové dál 200 L/min.
- **Rychlost rmutu v potrubí.** Nad 1,5 m/s program varuje. C1 naplno (80 L/min ve skutečnosti) dělá v trubce Ø 50 mm
  0,68 m/s, takže varování přijde až při průtoku nad 177 L/min. Průměr 50 mm beru pro všechny trubky (sim průměry nezná).
- **Pára:** nepřenáší se, program má vlastní model tlaku kotle a rozdílu teplot (`_steam_factor`).

Změny v `sim.gd` jsou označené `FYZIKA+`. Původní soubory jsou v `zaloha-pred-fyzika-plus/`.

## Ověření (17.–20. 11., u mě bez grafiky)

- Testy v `tests/`: stejné výsledky jako původní verze, jen s jinými čísly tam, kde se to čeká (teploty, objem).
  Pět testů hlásí chyby už v původní verzi a stejně i tady: `flush`, `pump_flow`, `sight`, `skrin`, `valve_pct`.
- Celá várka v `train_test.gd`: dřív 3 440 L, 13,4 °P, 8,3 °C; teď 3 347 L, 13,7 °P, 8,1 °C (víc se odpaří, protože
  pánev ztrácí míň tepla). Chyb proti postupu 3 jako dřív.
- **Neověřeno: várka podle checklistu (`--mode=chkauto`, 43 kroků).** U mě bez grafiky neběží ani v původní verzi.
  Spusť `Zkouska cele varky.bat`; výsledek se zapíše do `chkauto-vysledek.txt`.

## Zbývá přenést

| Co | Kde v tomhle programu | Poznámka |
|---|---|---|
| Stojící čerpadlo průchozí, zpětný tok, výpust přes čerpadlo | `net_flow.gd` | zpětné klapky na Zajíci nejsou |
| pH mladiny za varu, barva podle teploty a pH | `recipes.gd` (`model_ebc`), `_heat` | tady je barva jen ze sladu |
| Nátok do vířivky (rychlost tangenciálního nástřiku) | `_settling` | |
| Kreslení: proudy obloukem, vlnky, usazené dílo | `plant_3d.gd`, shadery | jiný (3D) vzhled, dělá se zvlášť |

Dílčí rmuty: checklist má tvůj postup (asi 10 hl), velikost neměním. Jestli s menšími ztrátami vychází dílo po vrácení
rmutu teplejší, ukáže to celá várka.

Kyslík ve spilce tenhle program už má (`o2_spilka`).
