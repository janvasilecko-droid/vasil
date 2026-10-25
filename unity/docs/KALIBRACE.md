# Kalibrace modelu – přehled parametrů

Soubor je **generovaný** skriptem `unity/Tools/gen_calib_doc.py` z `sim.gd` (výchozí hodnoty, povolené rozsahy) a `params.gd` (jednotka, zdroj, nejistota, co a jak změřit). Neupravuj ručně.

Zdroj hodnoty: **MEASURED** změřeno na varně · **MANUFACTURER** údaj provozovatele / štítek · **LITERATURE** hodnota z literatury uvedená v kódu · **ESTIMATED** technický odhad, nutné změřit · **CALIBRATED** upraveno podle měření.

Kalibraci lze zadat bez zásahu do kódu v souboru `user://kalibrace.json` (klíč → hodnota); hodnota mimo povolený rozsah se odmítne.

Souhrn: MANUFACTURER 10, LITERATURE 8, CALIBRATED 3, ESTIMATED 64, CALIBRATED (na referenční scénář, ne na měření) 1, RECEPT 1, STATED 1; celkem 88 popsaných parametrů.

| Parametr | Výchozí | Jednotka | Zdroj | Nejistota / rozsah | Povolený rozsah | Co ovlivňuje / jak změřit |
|---|---|---|---|---|---|---|
| `boilerKW` | 650.0 | kW | MANUFACTURER | jmenovitý | 50.0 … 3000.0 | parní kotel 650 kW (údaj provozovatele) – skutečný výkon závisí na tlaku a odběru |
| `chillerKW` | 45.0 | kW | MANUFACTURER | jmenovitý | 5.0 … 500.0 | glykolový agregát 45 kW – skutečný výkon klesá s teplotou okolí |
| `hltCap` | 12000.0 | L | MANUFACTURER | ±5 % | 1000.0 … 30000.0 | bojler 120 hl |
| `hltSet` | 83.0 | °C | MANUFACTURER | - | – | nastavení termostatu 83 °C |
| `lvSet` | 2.0 | °C | MANUFACTURER | - | – | nastavení ledové vody 2 °C |
| `lvVol` | 10000.0 | L | MANUFACTURER | ±5 % | 1000.0 … 30000.0 | ledovka 100 hl |
| `malt` | 680.0 | kg | MANUFACTURER | - | – | sypání podle receptu |
| `rvpCap` | 5000.0 | L | MANUFACTURER | ±5 % | 1000.0 … 10000.0 | nominální objem pánve 50 hl (údaj provozovatele) |
| `skArea` | 5.3 | m² | MANUFACTURER | ±3 % | 1.0 … 20.0 | plocha falešného dna 5,3 m² (údaj provozovatele) |
| `steamBar` | 2.5 | bar přetlaku | MANUFACTURER | ±0,3 bar | 0.2 … 8.0 | tlak páry 2,5 bar g; sytostní teplota se počítá z absolutního tlaku (p + 1,013) |
| `cpSteel` | 0.50 | kJ/(kg·K) | LITERATURE | 0,46–0,51 | 0.4 … 0.6 | měrná tepelná kapacita austenitické nerezi (AISI 304); jediné místo převodu sim.steelC() |
| `kdAlpha63` | 0.022 | 1/min | LITERATURE | – | 0.001 … 1.0 | inaktivace α-amylázy při 63 °C, Ea 83,7 kJ/mol (Marc et al. 1983, Biotechnol. Bioeng. 25:481) – exponent ze skenu, ověřit |
| `kdBeta63` | 0.13 | 1/min | LITERATURE | – | 0.005 … 3.0 | inaktivace β-amylázy při 63 °C, Ea 188 kJ/mol (Marc 1983); 13 % stabilní podíl 40× pomalejší (De Schepper et al. 2022) |
| `maltExtract` | 80.0 | % v sušině | LITERATURE | ±2 % | 60.0 … 85.0 | jen záloha pro neznámý slad – extrakt se počítá z tabulky MALTS (atest sladovny) |
| `maltMoist` | 4.5 | % | LITERATURE | ±1 % | 2.0 … 10.0 | jen záloha – vlhkost se bere z MALTS (atest) |
| `skMuP` | 0.047 | 1/°P | LITERATURE | 0,04–0,055 | 0.0 … 0.1 | μ_sladiny/μ_vody = e^(skMuP·°P); EBC kongresní sladina 8,6 % má ~1,4–1,7 mPa·s při 20 °C (≈1,5× vody) |
| `spargeEndP` | 1.0 | °P | LITERATURE | 0,7–1 % (Kosa s. 147, 156) | – | konec vyslazování (poslední podíl) |
| `tbWater` | 99.3 | °C | LITERATURE | ±0,3 °C | 95.0 … 100.5 | bod varu vody pro nadmořskou výšku varny (99,3 °C ≈ 200 m n. m.) – upravit podle lokality |
| `betaAct` | 4.5 | - | CALIBRATED | 0,1–50 | 0.1 … 50.0 | rychlost štěpení β-amylázy; nastaveno na zdánlivé prokvašení ~76 % u standardní dekokce (typické pro český ležák – upřesnit vlastním rozborem) |
| `mashFilmK` | 4.6 | - | CALIBRATED | 0–30 | 0.0 … 30.0 | zhoršení přestupu tepla do hustého rmutu; nastaveno tak, aby plná pánev rmutu hřála ~1 K/min (Weihenstephan, US 10,982,178); var mladiny tím ovlivněn není |
| `skRetentionLkg` | 1.0 | L/kg sladu | CALIBRATED | 0,8–1,6 | 0.5 … 3.0 | sladina zadržená v mlátě; průmyslové mláto má 20–25 % sušiny ≈ 0,8–1,0 L/kg; 1,0 sedí na údaj Vasila 700 kg → 40 hl / ~12,5 °P. Změřit: objem sladiny, který zůstane po scezení |
| `UA1` | 25.0 | kW/K | ESTIMATED | ±40 % | 1.0 … 200.0 | 1. stupeň deskového chladiče (voda) – změřit vstupy/výstupy a průtoky |
| `UA2` | 12.0 | kW/K | ESTIMATED | ±40 % | 1.0 … 200.0 | 2. stupeň (ledová voda) |
| `ambientT` | 20.0 | °C | ESTIMATED | ±5 °C | -5.0 … 40.0 | teplota ve varně |
| `boilerWaterKg` | 1500.0 | kg | ESTIMATED | 50–20000 | 50.0 … 20000.0 | zásoba syté vody v kotli – určuje, jak rychle klesá tlak při přetížení (dh'/dp ≈ 57 kJ/kg·bar); změřit/zjistit ze štítku kotle |
| `bore25` | 50.0 | mm | ESTIMATED | ±10 % | 15.0 … 120.0 | vnitřní průměr trubek DN25 z plánku (Vasil: DN50) |
| `bore40` | 50.0 | mm | ESTIMATED | ±10 % | 15.0 … 120.0 | vnitřní průměr DN40 z plánku (DN50) |
| `bore50` | 75.0 | mm | ESTIMATED | ±10 % | 15.0 … 150.0 | vnitřní průměr DN50 z plánku (silné DN75) |
| `cipAcidMin` | 0.5 | % | ESTIMATED | 0,5–1 % | 0.1 … 5.0 | minimální koncentrace kyseliny – podle přípravku |
| `cipLyeMin` | 1.0 | % NaOH | ESTIMATED | 1–2 % | 0.3 … 5.0 | minimální koncentrace louhu podle provozního předpisu – doplnit podle přípravku |
| `coolWaterT` | 16.0 | °C | ESTIMATED | 8–18 °C podle ročního období | 1.0 … 30.0 | teplota vody z řadu |
| `hltStart` | 11000.0 | L | ESTIMATED | - | – | počáteční náplň bojleru |
| `hltSteel` | 600.0 | kg oceli | ESTIMATED | ±50 % | 0.0 … 5000.0 | bojler: dříve natvrdo 300 kJ/K → 600 kg × 0,5 |
| `hltT0` | 83.0 | °C | ESTIMATED | - | – | počáteční teplota bojleru |
| `hltUA` | 0.01 | kW/K | ESTIMATED | ±100 % | 0.0 … 3.0 | ztráty izolovaného bojleru – změřit chladnutí přes noc |
| `hltUAst` | 2.0 | kW/K | ESTIMATED | ±50 % | 0.1 … 20.0 | ohřev bojleru párou |
| `iboKeep` | 0.44 | - | ESTIMATED | 0,3–0,7 | 0.2 … 1.0 | podíl izo-α-kyselin z varu, který zůstane v hotové mladině a změří se jako IBU (ztráty na kalech, pěně, adsorpce na hořké zlomy) – kalibrovat analýzou IBU z várky |
| `lvStart` | 0.0 | L | ESTIMATED | - | – | počáteční náplň ledovky |
| `lvT0` | 2.0 | °C | ESTIMATED | - | – | počáteční teplota ledovky |
| `mainsBar` | 4.0 | bar přetlaku | ESTIMATED | 3–5 bar | 0.5 … 10.0 | statický tlak vodovodního řadu v budově (manometr na přívodu při zavřených odběrech) – tlak pod zátěží se dopočte z přívodu a trasy |
| `mainsQ` | 900.0 | L/min | ESTIMATED | ±40 % | 100.0 … 2000.0 | průtok při volném výtoku z řadu bez další trasy (kapacita přívodu) – změřit kbelíkem/vodoměrem u řadu |
| `maltRate` | 40.0 | kg/min | ESTIMATED | ±30 % | 5.0 … 200.0 | výkon šrotovníku / vystírání |
| `mill` | 1.0 | stupeň 1–3 | ESTIMATED | - | – | jemnost šrotu (1 = jemný → propustnost 0,6×, 3 = hrubý → 1,5×) |
| `mpCap` | 6000.0 | L | ESTIMATED | ±10 % | 1000.0 … 10000.0 | mladinová pánev |
| `mpDead` | 60.0 | L | ESTIMATED | ±50 % | – | mrtvý objem |
| `mpJacketL` | 2500.0 | L | ESTIMATED | ±20 % | 200.0 … 6000.0 | smáčení pláště mladinové pánve |
| `mpSteel` | 600.0 | kg oceli | ESTIMATED | ±30 % | 0.0 … 3000.0 | mladinová pánev (v plánku nepoužitá) |
| `mpUA` | 0.3 | kW/K | ESTIMATED | ±50 % | 0.0 … 3.0 | ztráty mladinové pánve |
| `mpUAst` | 4.5 | kW/K | ESTIMATED | ±40 % | 0.5 … 30.0 | pára → náplň mladinové pánve |
| `p1_H0` | 32.0 | m | ESTIMATED | ±40 % | 3.0 … 80.0 | C1 výtlačná výška při nulovém průtoku – štítek/křivka |
| `p1_Qn` | 400.0 | L/min | ESTIMATED | ±40 % | 20.0 … 1500.0 | C1 jmenovitý průtok |
| `p2_H0` | 22.0 | m | ESTIMATED | ±40 % | 3.0 … 80.0 | C2 |
| `p2_Qn` | 150.0 | L/min | ESTIMATED | ±40 % | 20.0 … 1500.0 | C2 |
| `p3_H0` | 26.0 | m | ESTIMATED | ±40 % | 3.0 … 80.0 | C3 |
| `p3_Qn` | 200.0 | L/min | ESTIMATED | ±40 % | 20.0 … 1500.0 | C3 |
| `pipeLenScale` | 0.4 | - | ESTIMATED | ±50 % | 0.05 … 3.0 | skutečná délka tras / délka v plánku (100 px = 1 m) – změřit trasy |
| `pivLoad` | 10.0 | kW | ESTIMATED | ±50 % | – | tepelná zátěž okruhu ledové vody |
| `plateRefQ` | 80.0 | L/min | ESTIMATED | 10–500 | 10.0 … 500.0 | průtok mladiny, při kterém platí UA1/UA2; prostup roste s průtokem ∝ Q^0,65 (korelace Nu–Re pro deskové výměníky) |
| `rvpDead` | 20.0 | L | ESTIMATED | ±50 % | – | nevyčerpatelný zbytek ve výpusti |
| `rvpJacketL` | 2200.0 | L | ESTIMATED | ±20 % | 200.0 … 6000.0 | objem náplně, nad kterým je plášť celý smáčený |
| `rvpSteel` | 450.0 | kg oceli | ESTIMATED | ±30 % | 0.0 … 3000.0 | hmotnost nádoby; kapacita = kg × cpSteel – z výkresu nebo štítku |
| `rvpUA` | 0.25 | kW/K | ESTIMATED | ±50 % | 0.0 … 3.0 | ztráty pánve do okolí – změřit chladnutí plné pánve bez páry |
| `rvpUAst` | 6.5 | kW/K | ESTIMATED | ±40 % | 0.5 … 30.0 | přestup pára → náplň; kalibrovat z rychlosti ohřevu (K/min) při známém tlaku páry |
| `skCap` | 5000.0 | L | ESTIMATED | ±10 % | 1000.0 … 10000.0 | objem scezovací kádě |
| `skChanK` | 0.02 | 1/min | ESTIMATED | ×3 | – | rychlost tvorby kanálků |
| `skCompK` | 0.05 | 1/min | ESTIMATED | ×3 | – | rychlost slehávání nad prahem |
| `skContactUA` | 4.0 | kW/K | ESTIMATED | ±50 % | – | přestup náplň ↔ stěna SK |
| `skCreepK` | 0.0015 | 1/min | ESTIMATED | ×3 | – | rychlost pomalého sedání lože |
| `skDpChannel` | 160.0 | mbar | ESTIMATED | 120–250 | 20.0 … 800.0 | práh vzniku kanálků |
| `skDpComp` | 30000.0 | Pa | ESTIMATED | ±50 % | 1000.0 … 200000.0 | stlačitelnost koláče k = k₀ / (1 + Δp / skDpComp) |
| `skDpCompact` | 80.0 | mbar | ESTIMATED | 60–120 | 10.0 … 500.0 | práh rychlého slehávání – kalibrovat z provozu (tlakoměr pod sítem) |
| `skSteel` | 500.0 | kg oceli | ESTIMATED | ±30 % | 0.0 … 3000.0 | scezovací káď včetně falešného dna |
| `skSuction` | 20000.0 | Pa | ESTIMATED | ±50 % | 0.0 … 60000.0 | největší podtlak pod sítem, který C2 vytvoří – vakuometr pod sítem |
| `skTeigK` | 0.03 | - | ESTIMATED | ×3 | – | propustnost těsta vůči mlátu |
| `skTeigRho` | 350.0 | kg/m³ | ESTIMATED | ±50 % | – | hustota (sušina) horního těsta |
| `skUA` | 0.2 | kW/K | ESTIMATED | ±50 % | 0.0 … 3.0 | ztráty SK |
| `spCap` | 6000.0 | L | ESTIMATED | ±10 % | 1000.0 … 20000.0 | objem spilky |
| `trubKstill` | 0.0111 | 1/min | ESTIMATED | ×3 | 0.001 … 0.2 | rychlost usazování kalů bez víření (poločas ≈ 1 h) – změřit turbiditou/hmotností kalu v klidové nádobě |
| `trubKwhirl` | 0.2 | 1/min | ESTIMATED | ×2 | 0.02 … 2.0 | rychlost sběru kalů do kužele při plném víření (poločas ≈ 3,5 min) – změřit hmotností kalu a zákalem mladiny po usazení |
| `waterFlow` | 250.0 | L/min | ESTIMATED | ±30 % | 10.0 … 600.0 | průtok čerpadla horké vody C4 do pánve – změřit vodoměrem / časem napouštění |
| `whirlTau` | 1.5 | min | ESTIMATED | ×2 | 0.1 … 10.0 | čas, za který se ve vířivce ustaví stabilní rotace (čajový efekt) – změřit odběrem vzorků kalu po různé době víření |
| `wpCap` | 5000.0 | L | ESTIMATED | ±10 % | 1000.0 … 10000.0 | objem vířivé kádě |
| `wpDead` | 30.0 | L | ESTIMATED | ±50 % | – | kaly + mrtvý objem vířivky |
| `wpSteel` | 550.0 | kg oceli | ESTIMATED | ±30 % | 0.0 … 3000.0 | vířivá káď |
| `wpUA` | 0.35 | kW/K | ESTIMATED | ±50 % | 0.0 … 3.0 | ztráty vířivky |
| `preboilV` | 4430.0 | L (horká) | STATED | 2000–5000 | 2000.0 … 5000.0 | plná pánev před varem; nastaveno tak, aby vyrážka byla 40 hl při ~7 %/h odparu (Vasil: 700 kg → 40 hl, 12,5 °P) |
| `skPerm` | 1.5e-11 | m² | CALIBRATED (na referenční scénář, ne na měření) | řád velikosti | 1e-13 … 1e-9 | 1,5e-11 ≈ 15 darcy; nastaveno tak, aby 40 L/min dalo Δp ~50 mbar; kalibrovat z Δp a průtoku C2 |
| `targetV` | 40.0 | hl | RECEPT | - | – | cílový objem várky (cíl, ne fyzikální parametr) |

## Parametry bez popisu v registru

Mají výchozí hodnotu v kódu, ale chybí jim jednotka/zdroj/nejistota – doplnit do `params.gd`:

`boilMin`=90.0, `pitchT`=8.5, `targetEBC`=8.0, `targetIBU`=32.0, `targetP`=12.0
