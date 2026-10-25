# Audit parametrů modelu

_Generuje `unity/Tools/audit_params.py` (v CI), ručně neupravovat._

Porovnává `CFG_DEFAULT` (sim.gd), `CALIB_RANGE` (sim.gd), `META` (params.gd) a skutečná čtení v kódu.

- parametrů v `CFG_DEFAULT`: **111**, popsaných v `META`: **111**, s povoleným rozsahem: **111**
- nalezených nekonzistencí: **0**

## Použití parametrů (první výskyty ve výpočtu)

| Parametr | Výchozí | Počet čtení | Kde |
|---|---|---|---|
| `UA1` | 25.0 | 4 | proc.gd:166, proc.gd:167, proc.gd:186, sim.gd:1904 |
| `UA2` | 12.0 | 1 | sim.gd:1904 |
| `ambientT` | 20.0 | 8 | sim.gd:393, sim.gd:394, sim.gd:638, sim.gd:853 |
| `betaAct` | 4.5 | 1 | sim.gd:547 |
| `boilMin` | 90.0 | 8 | auto.gd:308, sim.gd:83, sim.gd:148, sim.gd:647 |
| `boilerKW` | 650.0 | 3 | sim.gd:843, sim.gd:848, sim.gd:849 |
| `boilerWaterKg` | 1500.0 | 1 | sim.gd:844 |
| `bore25` | 50.0 | 2 | sim.gd:2657, sim.gd:2722 |
| `bore40` | 50.0 | 2 | sim.gd:2657, sim.gd:2723 |
| `bore50` | 75.0 | 2 | sim.gd:2657, sim.gd:2724 |
| `chillerKW` | 45.0 | 2 | sim.gd:870, sim.gd:2115 |
| `cipAcidMin` | 0.5 | 3 | ctl.gd:546, proc.gd:109, proc.gd:110 |
| `cipAcidT` | 60.0 | 2 | proc.gd:109, proc.gd:111 |
| `cipHeat` | 1.2 | 1 | proc.gd:99 |
| `cipLyeMin` | 1.0 | 3 | ctl.gd:546, proc.gd:104, proc.gd:105 |
| `cipLyeT` | 75.0 | 2 | proc.gd:104, proc.gd:106 |
| `cipMin` | 20.0 | 1 | proc.gd:115 |
| `cipRinseMin` | 5.0 | 1 | proc.gd:115 |
| `coolWaterT` | 16.0 | 2 | sim.gd:165, sim.gd:167 |
| `cpSteel` | 0.50 | 1 | sim.gd:236 |
| `ferCool` | 0.25 | 1 | proc.gd:70 |
| `ferDiaK` | 0.012 | 1 | proc.gd:72 |
| `ferDiaRed` | 0.06 | 1 | proc.gd:74 |
| `ferDiaT` | 0.11 | 1 | proc.gd:72 |
| `ferHeat` | 0.6 | 1 | proc.gd:70 |
| `ferKsMu` | 8.0 | 1 | proc.gd:66 |
| `ferKsQ` | 6.0 | 1 | proc.gd:67 |
| `ferMu` | 0.05 | 1 | proc.gd:66 |
| `ferQ` | 0.2 | 1 | proc.gd:67 |
| `ferTa` | 0.095 | 1 | proc.gd:65 |
| `ferXmax` | 4.0 | 1 | proc.gd:66 |
| `hltCap` | 12000.0 | 8 | main.gd:1883, practice.gd:58, sim.gd:133, sim.gd:134 |
| `hltSet` | 83.0 | 1 | sim.gd:166 |
| `hltStart` | 11000.0 | 3 | sim.gd:133, sim.gd:134, sim.gd:166 |
| `hltSteel` | 600.0 | 1 | sim.gd:852 |
| `hltT0` | 83.0 | 1 | sim.gd:166 |
| `hltUA` | 0.01 | 1 | sim.gd:853 |
| `hltUAst` | 2.0 | 1 | sim.gd:836 |
| `iboKeep` | 0.44 | 1 | sim.gd:1239 |
| `kdAlpha63` | 0.022 | 1 | sim.gd:556 |
| `kdBeta63` | 0.13 | 1 | sim.gd:557 |
| `lvSet` | 2.0 | 2 | sim.gd:167, sim.gd:2115 |
| `lvStart` | 0.0 | 4 | sim.gd:135, sim.gd:136, sim.gd:167, sim.gd:168 |
| `lvT0` | 2.0 | 1 | sim.gd:167 |
| `lvUA` | 0.15 | 1 | sim.gd:859 |
| `lvVol` | 10000.0 | 17 | auto.gd:91, auto.gd:311, auto.gd:392, auto.gd:393 |
| `mainsBar` | 4.0 | 1 | sim.gd:2644 |
| `mainsQ` | 900.0 | 1 | sim.gd:2644 |
| `malt` | 680.0 | 48 | auto.gd:34, auto.gd:75, auto.gd:180, auto.gd:181 |
| `maltExtract` | 80.0 | 1 | sim.gd:243 |
| `maltMoist` | 4.5 | 3 | sim.gd:243, sim.gd:246, sim.gd:894 |
| `maltRate` | 40.0 | 1 | sim.gd:882 |
| `mashFilmK` | 4.6 | 1 | sim.gd:595 |
| `mill` | 1.0 | 2 | sim.gd:355, sim.gd:888 |
| `mpCap` | 6000.0 | 6 | main.gd:1882, practice.gd:55, sim.gd:966, sim.gd:989 |
| `mpDead` | 60.0 | 1 | sim.gd:679 |
| `mpJacketL` | 2500.0 | 1 | sim.gd:835 |
| `mpSteel` | 600.0 | 2 | sim.gd:621, sim.gd:735 |
| `mpUA` | 0.3 | 1 | sim.gd:620 |
| `mpUAst` | 4.5 | 1 | sim.gd:835 |
| `p1_H0` | 32.0 | 1 | sim.gd:(skládaný klíč) |
| `p1_Qn` | 400.0 | 1 | sim.gd:(skládaný klíč) |
| `p2_H0` | 22.0 | 1 | sim.gd:(skládaný klíč) |
| `p2_Qn` | 150.0 | 1 | sim.gd:(skládaný klíč) |
| `p3_H0` | 26.0 | 1 | sim.gd:(skládaný klíč) |
| `p3_Qn` | 200.0 | 1 | sim.gd:(skládaný klíč) |
| `pipeLenScale` | 0.4 | 3 | sim.gd:2650, sim.gd:2692, sim.gd:2717 |
| `pitchT` | 8.5 | 2 | proc.gd:218, sim.gd:148 |
| `pivLoad` | 10.0 | 1 | sim.gd:860 |
| `plateRefQ` | 80.0 | 1 | sim.gd:1909 |
| `preboilV` | 4430.0 | 4 | auto.gd:231, auto.gd:381, lauter_auto.gd:91, sim.gd:1047 |
| `rvpCap` | 5000.0 | 5 | practice.gd:54, sim.gd:966, sim.gd:989, sim.gd:2449 |
| `rvpDead` | 20.0 | 4 | auto.gd:296, sim.gd:679, training.gd:122, training.gd:150 |
| `rvpJacketL` | 2200.0 | 1 | sim.gd:835 |
| `rvpSteel` | 450.0 | 4 | sim.gd:621, sim.gd:735, sim.gd:898, sim.gd:2183 |
| `rvpUA` | 0.25 | 1 | sim.gd:620 |
| `rvpUAst` | 6.5 | 3 | sim.gd:587, sim.gd:604, sim.gd:835 |
| `skArea` | 5.3 | 6 | lauter_auto.gd:110, sim.gd:317, sim.gd:365, sim.gd:367 |
| `skBedLkg` | 2.7 | 1 | sim.gd:317 |
| `skCap` | 5000.0 | 7 | auto.gd:381, main.gd:1880, practice.gd:57, sim.gd:950 |
| `skChanK` | 0.02 | 1 | sim.gd:434 |
| `skCompK` | 0.05 | 1 | sim.gd:433 |
| `skContactUA` | 4.0 | 1 | sim.gd:389 |
| `skCreepK` | 0.0015 | 1 | sim.gd:432 |
| `skDpChannel` | 160.0 | 3 | sim.gd:137, sim.gd:138, sim.gd:431 |
| `skDpComp` | 30000.0 | 1 | sim.gd:363 |
| `skDpCompact` | 80.0 | 6 | sim.gd:137, sim.gd:138, sim.gd:431, sim.gd:454 |
| `skMuP` | 0.047 | 2 | main.gd:1126, sim.gd:351 |
| `skPerm` | 1.5e-11 | 6 | main.gd:1129, main.gd:1134, sim.gd:354, sim.gd:356 |
| `skRetentionLkg` | 1.0 | 2 | main.gd:1131, sim.gd:319 |
| `skSteel` | 500.0 | 3 | sim.gd:389, sim.gd:393, sim.gd:499 |
| `skSuction` | 20000.0 | 1 | sim.gd:375 |
| `skTeigK` | 0.03 | 1 | sim.gd:366 |
| `skTeigRho` | 350.0 | 1 | sim.gd:365 |
| `skUA` | 0.2 | 2 | sim.gd:393, sim.gd:394 |
| `spCap` | 6000.0 | 6 | main.gd:1884, practice.gd:59, sim.gd:717, sim.gd:718 |
| `spargeEndP` | 1.0 | 4 | auto.gd:303, auto.gd:306, lauter_auto.gd:91, training.gd:137 |
| `steamBar` | 2.5 | 8 | proc.gd:162, proc.gd:163, proc.gd:184, sim.gd:170 |
| `targetEBC` | 8.0 | 2 | proc.gd:215, sim.gd:147 |
| `targetIBU` | 32.0 | 3 | ctl.gd:593, proc.gd:214, sim.gd:147 |
| `targetP` | 12.0 | 2 | proc.gd:213, sim.gd:147 |
| `targetV` | 40.0 | 2 | proc.gd:212, sim.gd:1270 |
| `tbWater` | 99.3 | 4 | sim.gd:636, sim.gd:854, sim.gd:856, sim.gd:857 |
| `trubKstill` | 0.0111 | 1 | sim.gd:1017 |
| `trubKwhirl` | 0.2 | 1 | sim.gd:1017 |
| `waterFlow` | 250.0 | 3 | oppanel.gd:217, oppanel.gd:218, sim.gd:2149 |
| `whirlTau` | 1.5 | 1 | sim.gd:1016 |
| `wpCap` | 5000.0 | 4 | main.gd:1881, practice.gd:56, sim.gd:2450, sim.gd:2767 |
| `wpDead` | 30.0 | 2 | sim.gd:684, training.gd:153 |
| `wpSteel` | 550.0 | 2 | sim.gd:748, sim.gd:1006 |
| `wpUA` | 0.35 | 1 | sim.gd:1006 |

Parametry čtené právě na jednom místě (změna se projeví jen tam; zkontrolováno, že jde o jediný fyzikální jev): `UA2`, `betaAct`, `boilerWaterKg`, `cipHeat`, `cipMin`, `cipRinseMin`, `cpSteel`, `ferCool`, `ferDiaK`, `ferDiaRed`, `ferDiaT`, `ferHeat`, `ferKsMu`, `ferKsQ`, `ferMu`, `ferQ`, `ferTa`, `ferXmax`, `hltSet`, `hltSteel`, `hltT0`, `hltUA`, `hltUAst`, `iboKeep`, `kdAlpha63`, `kdBeta63`, `lvT0`, `lvUA`, `mainsBar`, `mainsQ`, `maltExtract`, `maltRate`, `mashFilmK`, `mpDead`, `mpJacketL`, `mpUA`, `mpUAst`, `p1_H0`, `p1_Qn`, `p2_H0`, `p2_Qn`, `p3_H0`, `p3_Qn`, `pivLoad`, `plateRefQ`, `rvpJacketL`, `rvpUA`, `skBedLkg`, `skChanK`, `skCompK`, `skContactUA`, `skCreepK`, `skDpComp`, `skSuction`, `skTeigK`, `skTeigRho`, `trubKstill`, `trubKwhirl`, `whirlTau`, `wpUA`
