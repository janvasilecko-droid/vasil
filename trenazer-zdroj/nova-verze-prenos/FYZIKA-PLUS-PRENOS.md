# FYZIKA+ v tomhle programu – co je přeneseno a co zbývá

Stav 17. 11. 2026. Balíček FYZIKA+ (repozitář `vasil`, `trenazer-zdroj/fyzika-plus/`) vznikl pro starší trenažér
(v70 / v152). Tenhle program má jinou stavbu (`scripts/sim.gd` nad trasami, `net_flow.gd`, checklist 43 kroků),
takže vkládací skript tu nejde použít. Přenáší se ručně po částech, každá část s testem v `tests/`.

Zálohy souborů před změnou: složka `zaloha-pred-fyzika-plus/`.

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

Změny v `sim.gd`: tři místa označená `FYZIKA+` (načtení modulu, volání `FyzikaPlus.losses` ve `step`, řádek v `_heat`).

## Zbývá přenést (v tomhle pořadí)

| # | Co | Kde v tomhle programu | Poznámka |
|---|---|---|---|
| 2 | Pára: výkon podle rozdílu teplot pára – náplň, ventil a tlak v plášti | `_heat`, `_thermostat`, `max_kw` | teď má ohřev pevné kW bez ohledu na teplotu náplně |
| 3 | Deskový chladič podle návrhového bodu z knih, řízení ledové vody | `plate_cool`, `_pitch`, `UA_WATER`, `UA_ICE`, `ICE_LPM` | **Vasil 17. 11.: chladič je jednostupňový.** Voda jde do nádoby (ledovky), tam ji vychladí glykol, a teprve ledová voda jde do chladiče. Stupeň „voda z řadu“ (`UA_WATER`) tedy pryč, chladí jen ledová. Voda z řadu se cestou do ledovky předchladí a v ledovce se okruhem přes glykol dochladí na 2 °C |
| 4 | Velikost dílčích rmutů z tepelné bilance (63 a 76 °C po vrácení) | automat checklistu (`checklist_navrh.gd`) | s menšími ztrátami by pevné podíly přehřály dílo |
| 5 | Stojící čerpadlo průchozí, zpětný tok, výpust přes čerpadlo | `net_flow.gd` (`solve_gravity`, BFS) | zpětné klapky na Zajíci nejsou |
| 6 | Kalový kužel ve vířivce při rychlém stahování | `_settling`, `_pitch`, `CONE_LOSS` | |
| 7 | pH mladiny za varu, barva podle teploty a pH | `recipes.gd` (`model_ebc`), `_heat` | tady je barva jen ze sladu |
| 8 | Rychlost rmutu v potrubí, nátok do vířivky | `_flow`, `net_flow.gd` | |
| 9 | Chladnutí stojícího potrubí | `pipe_hold` | trubky tu teplotu mají, ale nechladnou |
| 10 | Kreslení: proudy obloukem, vlnky, usazené dílo | `plant_3d.gd`, shadery | jiný (3D) vzhled, dělá se zvlášť |

Kyslík ve spilce tenhle program už má (`o2_spilka`).
