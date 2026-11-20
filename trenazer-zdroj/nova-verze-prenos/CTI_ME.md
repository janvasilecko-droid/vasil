# Přenos FYZIKA+ do aktuální verze trenažéru

Stav 20. 11. 2026. Aktuální verze je složka `Trenazer varny` na ploše Vasilova PC (zůstala beze změny).
**Upravená kopie je na ploše ve složce `Trenazer varny FYZIKA+`** (kopie bez 391MB videa v `reference/`), zapsáno 20. 11.

Hotovo (podrobně `FYZIKA-PLUS-PRENOS.md`):
1. tepelné ztráty izolovaných nádob podle výpočtu + odpar z horké hladiny mimo var (`scripts/fyzika_plus.gd`),
2. deskový chladič jednostupňový, jen ledová voda (`sim.plate_cool`, `UA_ICE` 207).

Soubory tady = to, co se na PC zapsalo: `scripts/fyzika_plus.gd`, `scripts/sim.gd` (rozdíl proti jeho verzi z 8. 11. v `sim.gd.diff`),
`tests/fyzika_plus_test.gd`, `tests/plate_test.gd`, `tests/calibration_test.gd`, `Zkouska cele varky.bat`.

Ověření: jeho testy v `tests/` dávají stejné výsledky jako původní (5 testů selhává už v původní verzi: flush, pump_flow,
sight, skrin, valve_pct); celá várka `train_test.gd` projde. `--mode=chkauto` (43 kroků) v kontejneru neběží (jedna
iterace trvá přes 1,7 s i v původním kódu) – má si ji pustit Vasil přes `Zkouska cele varky.bat`, výsledek bude
v `chkauto-vysledek.txt` v té složce.

Práce s jeho PC: `device_bash` funguje (20. 11.), složky jsou v `$HOME/mnt/Desktop/…`; soubory z kontejneru přes
`device_commit_files` z `/mnt/user-data/outputs/`. Testy se pouští `Godot --headless --path . -s tests/x_test.gd`.

**20. 11. večer – oprava automatu:** `sim.reset()` nechává bojler prázdný a žádný krok checklistu ho nenapouští, takže
automat stál na kroku 4 (bojler aspoň 70 °C); prázdná nádoba se nehřeje. Je to tak i v jeho původní složce. V kopii
FYZIKA+ je v `checklist_panel._start_auto` při nové várce bojler napuštěn 4000 L / 15 °C (moje volba, ne údaj od Vasila).
Zapsáno na PC, záloha `checklist_panel.gd.pred-opravou-bojleru`. Celý automat u mě neověřen (headless je příliš pomalý).

**20. 11. 23:10 – jedna složka (Vasil: „udělej jen jednu složku“):** všechno je teď přímo v `Desktop\Trenazer varny`;
kopie `Trenazer varny FYZIKA+` je vyprázdněná (prázdná složka možná zůstala, nešla odstranit). Původní soubory jsou
v `Trenazer varny\zaloha-pred-fyzika-plus\` (skripty jako `.txt`, je tam `.gdignore`). Oprava bojleru je místo
`checklist_panel.gd` v `sim_panel.new_batch` (4000 L / 15 °C, platí i pro ruční várku – Vasil odsouhlasil). Aktivní
`plan_edited.json` je ten z 20. 11. 20:52 (Vasil v kopii smazal trubku `pipe_mur91to7` a klapku `valve_muraghbe`,
přidal novou trubku a klapku, posunul `valve_mur96gfu`); plán ze 17. 11. je v záloze.
