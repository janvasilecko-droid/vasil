# FYZIKA+ v tomhle programu – co je přeneseno a co zbývá

Stav 17. 11. 2026. Balíček FYZIKA+ (repozitář `vasil`, `trenazer-zdroj/fyzika-plus/`) vznikl pro starší trenažér
(v70 / v152). Tenhle program má jinou stavbu (`scripts/sim.gd` nad trasami, `net_flow.gd`, checklist 43 kroků),
takže vkládací skript tu nejde použít. Přenáší se ručně po částech, každá část s testem v `tests/`.

Upravená verze je ve složce `Trenazer varny FYZIKA+` na ploše; původní složka `Trenazer varny` zůstala beze změny.

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

Změny v `sim.gd` jsou označené `FYZIKA+`. Původní soubory jsou ve složce `Trenazer varny` na ploše (ta zůstala beze změny).

## Ověření (17.–20. 11., u mě bez grafiky)

- Testy v `tests/`: stejné výsledky jako původní verze, jen s jinými čísly tam, kde se to čeká (teploty, objem).
  Pět testů hlásí chyby už v původní verzi a stejně i tady: `flush`, `pump_flow`, `sight`, `skrin`, `valve_pct`.
- Celá várka v `train_test.gd`: dřív 3 440 L, 13,4 °P, 8,3 °C; teď 3 347 L, 13,7 °P, 8,1 °C (víc se odpaří, protože
  pánev ztrácí míň tepla). Chyb proti postupu 3 jako dřív.
- **Neověřeno: várka podle checklistu (`--mode=chkauto`, 43 kroků).** U mě bez grafiky neběží ani v původní verzi.
  Spusť `Zkouska cele varky.bat`; výsledek se zapíše do `chkauto-vysledek.txt`.

## Zbývá přenést (v tomhle pořadí)

| # | Co | Kde v tomhle programu | Poznámka |
|---|---|---|---|
| 2 | Pára: výkon podle rozdílu teplot pára – náplň, ventil a tlak v plášti | `_heat`, `_thermostat`, `max_kw` | teď má ohřev pevné kW bez ohledu na teplotu náplně |
| 3 | Řízení ledové vody podle teploty do spilky (jednostupňový chladič je hotový, 2. část) | `_pitch`, `ICE_LPM` | **Vasil 17. 11.: chladič je jednostupňový.** Voda jde do nádoby (ledovky), tam ji vychladí glykol, a teprve ledová voda jde do chladiče. Stupeň „voda z řadu“ (`UA_WATER`) tedy pryč, chladí jen ledová. Voda z řadu se cestou do ledovky předchladí a v ledovce se okruhem přes glykol dochladí na 2 °C |
| 4 | Velikost dílčích rmutů z tepelné bilance (63 a 76 °C po vrácení) | automat checklistu (`checklist_navrh.gd`) | s menšími ztrátami by pevné podíly přehřály dílo |
| 5 | Stojící čerpadlo průchozí, zpětný tok, výpust přes čerpadlo | `net_flow.gd` (`solve_gravity`, BFS) | zpětné klapky na Zajíci nejsou |
| 6 | Kalový kužel ve vířivce při rychlém stahování | `_settling`, `_pitch`, `CONE_LOSS` | |
| 7 | pH mladiny za varu, barva podle teploty a pH | `recipes.gd` (`model_ebc`), `_heat` | tady je barva jen ze sladu |
| 8 | Rychlost rmutu v potrubí, nátok do vířivky | `_flow`, `net_flow.gd` | |
| 9 | Chladnutí stojícího potrubí | `pipe_hold` | trubky tu teplotu mají, ale nechladnou |
| 10 | Kreslení: proudy obloukem, vlnky, usazené dílo | `plant_3d.gd`, shadery | jiný (3D) vzhled, dělá se zvlášť |

Kyslík ve spilce tenhle program už má (`o2_spilka`).
