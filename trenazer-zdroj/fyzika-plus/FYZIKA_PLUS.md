# FYZIKA+: realističtější fyzika a chemie trenažéru varny

Vypracováno 29. 10. 2026 na zdrojích 2D v70 / 3D v152 (`vasil/unity/GodotSrc`), **doplněno 16. 11. 2026 (verze 1.1:
kapaliny a dílo, kapitola 7; verze 1.2: stojící čerpadlo, ztráty podle hladiny, usazené dílo, kapitola 8)**.
Balíček je stavěný tak, aby šel vložit i do novější verze (viz INTEGRACE.md).
**Nic z toho není vydané** v ⟳ Aktualizovat. Balíček leží v repozitáři `vasil` ve složce `trenazer-zdroj/fyzika-plus/`.
Vzorce, jednotky a vkládací skript první verze prošla ještě nezávislá kontrola, její připomínky jsou zapracované.

> Upřímně: všechna čísla jsou z literatury nebo z jiných varen, ne z měření na Zajíci. Kde údaj chybí, je hodnota
> označená ODHAD a jde přepsat v `kalibrace.json`. Seznam věcí k naměření je v kapitole 5.

---

## 1. Co jsem v programu prošel a co jsem našel

Prošel jsem `sim.gd` (teplo, pára, var, reakce, potrubí, čerpadla, scezování, vířivka, chlazení), `proc.gd`
(pH, kvašení, CIP), `physics.gd`, `auto.gd`, testy a starší audity (22. 10.). Většina fyziky je v pořádku:
bilance energie a extraktu, ε-NTU chladiče, Darcy u lože, čerpadlo × odpor trasy, Malowicki a Dickenson.
Zbyla tahle slabá místa:

| # | Oblast | Co bylo | Proč to není reálné | Stav |
|---|---|---|---|---|
| 1 | **Pára** | výkon = UA × % ventilu × (T_páry − T) | Ventil neškrtí plochu, škrtí **průtok páry**. Tlak v plášti se ustaví podle rovnováhy ventil ↔ kondenzace. Při přiškrcení tlak v plášti spadne, u chladné náplně až k podtlaku, a kondenzát neodtéká („stall“). Na tohle upozorňoval už audit z 22. 10. | **opraveno (H1)** |
| 2 | **Přestup na straně náplně** | nemíchaná náplň = míchadlo na „4 Hz“; přestup nezávisel na teplotě | Řídká mladina se bez míchadla ohřívá přirozenou konvekcí, hustý rmut skoro vůbec, a proto se připaluje. Viskozita vody je při 20 °C 3,6× větší než při 100 °C, takže studená náplň předává hůř | **opraveno (H2)** |
| 3 | **pH** | jedno pH (rmutu) pro všechno, i pro vroucí mladinu | Za varu pH klesá (VLB 5,62 → 5,50 za 90 min), a izomerace chmele i SMM → DMS braly pH rmutu. Můj dřívější pokus s okyselením nemohl fungovat, protože `proc.step()` každý krok přepisuje `S.pH` | **opraveno (H3, H4)** |
| 4 | **Barva** | +2,5 EBC/h jen za varu, nezávisle na čemkoli | Maillardova reakce závisí na teplotě (Ea 77 kJ/mol), pH a extraktu. Barva přibývá i při **dekokci**, ve **vířivce** a během chlazení (VLB: vyražená 11,3 → 13,6 EBC, než se zchladí polovina) | **opraveno (H4)** |
| 5 | **Vířivka** | rychlost nátoku neměla vliv | Kunze: nátok max. 3,5 m/s, jinak se lom tříští; slabý nátok vířivku neroztočí | **doplněno (H5)** |
| 6 | **Trubky** | stojící obsah všech trubek chladl s τ = 30 min | Plná trubka DN50 z plánku (vnitřní průměr podle kalibrace `bore50` 75 mm) má podle konvekce, oceli a vody τ ≈ 2,3 h; tenčí trubky chladnou rychleji | **opraveno (H6)** |
| 7 | **Objem horké vody** | lineárně 0,00052/K | Při 100 °C to sedí (4,16 %), při 50 °C je to o 0,5 % víc (20 L na 40 hl) | **opraveno (H7, Kell 1975)** |
| 8 | Kyselina mléčná do rmutu | `11,8 mEq/mL` u „80 %“ | 11,8 platí pro 88% kyselinu, 80% má ~10,6 mEq/mL | jen upozornění, neměnil jsem |
| 9 | `film_factor()` | počítá vždy s `rvpUAst`, i pro mladinovou pánev | drobná chyba (MP v plánku nepoužitá) | FP.steam_pd bere správné UA |
| 10 | Připalování v `heat()` | výkon nádobě ubere 10 %, ale kotel už vyrobil 100 % | při připalování nesedí bilance energie (10 % zmizí) | **opraveno ve verzi 1.1 (H14)** |
| 11 | Testy | `%g` ve formátovacím řetězci | Godot `%g` nezná → 2× „unsupported format character“ v logu | jen kosmetika |
| 12 | Výchozí voda | alkalita 100 mg/L → pH rmutu 5,84 | O 0,2–0,3 víc než obvykle (Kosař s. 136: rmut 5,5–5,6; Kunze s. 286: sladina 5,5–5,6) → víc barvy, rychlejší DMS a izomerace | zadej rozbor vody v panelu |
| 13 | Ledová voda | čerpadlo ledové vody pevně 120 L/min | Kunze: ledová voda 1–2 °C se v chladiči ohřeje na 80–88 °C. To jde jen se seřízeným poměrem vody k mladině (~1,05–1,1 : 1) | **doplněno ve verzi 1.1 (H11)** |

---

## 2. Reálná data z jiných varen a z literatury

| Veličina | Hodnota | Zdroj | Jak je to v modelu |
|---|---|---|---|
| Průtok syté páry ventilem | ṁ = 12·Kv·P1·√(1 − 5,67·(0,42 − χ)²) kg/h; kritický tok od P2 < 0,58·P1 | Spirax Sarco | FP.steam_kgs |
| Charakteristika parního ventilu | pro ohřev párou ekviprocentní; úbytek tlaku na ventilu ≥ 10 % | Spirax Sarco | Kv/Kvs = R^(zdvih − 1), R 30; Kvs dopočítané na 10 % |
| U pláště, nerez, míchaná voda | 850–1 700 W/(m²·K); vodné roztoky 450–1 140 | Spirax Sarco | rvpUAst 6,5 kW/K ≈ 5–6 m² × 1 100 (beze změny) |
| Stall | tlak v plášti pod protitlakem kondenzátu → kondenzát stojí, nerovnoměrný ohřev, rázy | TLV, Spirax Sarco | stCondBack, jWaterKg, jDrainKgS |
| Vzduch v páře | 2 bar abs. s 25 % vzduchu: manometr ukazuje 1 bar přetlaku (120 °C), pára má jen 112 °C; 1 mm vzduchu klade teplu odpor jako ~15 m mědi | Spirax Sarco | Daltonův zákon + odvzdušňovač jAirTau; ODHAD „1 % vzduchu = poloviční přestup na straně páry“ |
| Plášť pánve | na začátku varu vytlačit vzduch parou (1–4 odvzdušňovací trubky), na konci otevřít, jinak podtlak a zborcení dna; pára 2–3 bar; 2,5 m mladiny = +0,25 bar u dna, tam vře až při 106 °C | Kunze s. 290–291 | jVacBreaker, hlášení; bod varu u dna v panelu (U3) |
| Ohřev mladiny k varu | moderní pánev: na 100 °C za ~15 min | Kunze s. 299 | nemíchaná mladina 43 hl: 0,79 K/min (dřív 0,72) |
| Odpar | dnes 4–6 % celkem; dříve 8–12 %; 5–8 % objemu za hodinu | Kunze s. 299, EU JRC | 85 % páry u plné pánve: 8,8 %/h |
| pH za varu | nepřevařená 5,5–5,6 → vyražená 5,4–5,5; VLB 5,62 → 5,50 za 90 min | Kunze s. 286, VLB (Meyer, CBC 2015) | phBoilDrop 0,08/h |
| Okyselení mladiny | těsně před koncem varu na 5,1–5,2 (lepší lom, méně barvy, čistší hořkost, horší využití chmele) | Kunze s. 286 | tlačítko +100 mL 80% kyseliny mléčné |
| Dávka kyseliny | 30 mL 0,8% kyseliny mléčné na kg sladu → −0,1 pH mladiny (rmut 60 mL); příklad: 1 L nepřevařené mladiny z 5,54 na 5,20 = 0,15 g kyseliny | Kunze s. 229 | wortBuf 26,6 mEq/(kg·pH) → 530 mL 80 % na 700 kg = −0,30 pH |
| Barva za varu | 5,7 → 6,5 → 7,2 → 8,0 EBC za 0/30/60/90 min; nádoba plná 8,2 → vyražená 11,3 → po zchlazení poloviny 13,6 EBC | VLB (Meyer, CBC 2015) | colK100 1,2 EBC/h (bez zahuštění odparem, to model počítá zvlášť) |
| Ea tvorby barvy | 77 kJ/mol (65–100 °C), nad 100 °C 112 kJ/mol | Kroh et al., TU Berlín | colEa 77 |
| Barva v provozu | slad 3,9–4,2 → předek 6,3–6,6 → nepřevařená 8,8–9,0 → vyražená 8,8–10,9 → zákvasná 11,5–12,0 → pivo 7–8 EBC | Kunze s. 287 (obr. 3.67d) | odhad barvy piva = 0,65 × zákvasná |
| DMS za varu | DMS-P 230 → 82 µg/L, volný DMS 110 → 12 µg/L za 90 min | VLB | (Dickenson beze změny, jen pH mladiny) |
| Vířivka | nátok ≤ 3,5 m/s, co největší objemový tok, často stačí mnohem menší rychlost; klid 20–30 min; D : H 3 : 1 | Kunze s. 340–341 | wpVmax 3,5; wpVmin 1,0 (ODHAD); hrdlo = trubka z plánku |
| Horký kal | 0,2–0,4 % mladiny, částice 30–80 µm; 6,6–6,9 kg sušiny/100 hl u 12% | VLB | model ~2,4 kg lomu + chmel na 40 hl (sedí) |
| Chladič | jednostupňově ledová 1–2 °C → 80–88 °C, mladina 95–98 °C → zákvasná; ledová ~3 °C pod zákvasnou | Kunze s. 351–352 | průtok ledové vody jde přiškrtit (H11) |
| Kyslík po provzdušnění | 8–10 mg/L | VLB | návrh N6 |
| Ztráty povrchem | holá nerez ~15 W/(m²·K) při 0,5 m/s vzduchu; izolovaná ~0,8; holé víko pánve 9 m² ≈ 10 kW | inzonex (ISO 12241) | pipeU 10 (holá trubka bez průvanu) |
| Energie varny | infuze, 7,5 % odparu: rmutování 2,2 + ohřev k varu 3,4 + var 5,0 + CIP 0,3 + teplá voda 2,3 kWh/hl; 1 % odparu ≈ 0,67 kWh/hl | EU JRC | protokol ukáže kWh páry na hl |
| Mladina | c_p ≈ 4,0 kJ/(kg·K); otopná plocha vnějšího vařáku ~0,2 m²/hl | Briggs | c_p v modelu 3,87 při 12 °P (ponecháno) |
| Hustota vody | 998,2 (20 °C) … 958,4 (100 °C) kg/m³ | Kell 1975 | FP.water_rho |
| Sytá pára | tlak a teplota sytosti jako v parních tabulkách, i pro podtlak; výparné teplo 2 453,5 (20 °C), 2 256,5 (100 °C), 2 147,7 kJ/kg (138,9 °C) | IAPWS-IF97 | FP.psat, FP.tsat; FP.hfg proložené (chyba < 0,1 %) |

---

## 3. Co balíček mění (podrobně)

### P1. Pára do pláště: ventil, tlak v plášti, vzduch, kondenzát (`FP.steam_pd`)
- **Ventil:** Kv = Kvs × R^(zdvih − 1). Pod 5 % zdvihu klesá lineárně k nule.
  `stValveR` = 30 je ekviprocentní regulační ventil (ODHAD, typ ventilu u vás neznám), 1 je lineární ruční ventil.
  Kvs se bez údaje dopočítá tak, aby plně otevřený ventil při 60 °C ubral 10 % tlaku.
  Plně otevřená pára pak dá 96–98 % dřívějšího výkonu a kalibrace `rvpUAst` platí dál.
- **Tlak v plášti** se počítá z rovnováhy: pára ventilem (Spirax) = pára zkondenzovaná v plášti
  (UA·(T_sat − T)/h_fg). Výkon je tedy omezený buď ventilem (přiškrceno, studená náplň), nebo pláštěm
  (plně otevřeno, u varu).
- **Vzduch:** Daltonův zákon. Se vzduchem je pára v plášti chladnější a hůř předává teplo.
  Zavzdušňovací ventil (`jVacBreaker` = 1) drží při přiškrcené páře a po zavření páry ~1 bar abs.
  Pustí přitom do pláště vzduch, který pak po otevření páry vytlačí odvzdušňovač (`jAirTau`); ohřev chvíli nejede naplno.
  Bez zavzdušňovacího ventilu (`jVacBreaker` = 0) spadne plášť do podtlaku a trenažér varuje:
  „otevři odvzdušnění, hrozí zborcení dna“ (Kunze s. 290–291).
  Zjednodušení: na začátku várky je plášť bez vzduchu.
- **Kondenzát:** přitéká, co se zkondenzuje. Odvaděč odvede tím víc, čím víc je tlak v plášti nad protitlakem
  kondenzátu (`stCondBack` 0,15 bar ODHAD). Pod protitlakem neodtéká nic (stall) a plášť se zaplavuje;
  zaplavená část netopí. Poddimenzovaný odvaděč plášť zaplaví i při přetlaku. Pustíš-li páru naráz do zaplaveného
  pláště: hlášení **RÁZ V PLÁŠTI**.
- **Zavřená parní trasa** v plánku: pára nejde a kotel ji nevyrábí. Kotel dostává přesně ten výkon, který přijme plášť.
- Panel u řádku kotle ukazuje **tlak v plášti pánve** (přetlak, jako manometr), zaplavení kondenzátem a vzduch.

Výkon do 2 500 L vody podle zdvihu (spočteno modelem, Kvs dopočítané, pára 2,5 bar, míchadlo 35 Hz):

| Zdvih | 60 °C: dřív | lineární (R 1) | R 10 | **R 30 (výchozí)** | 95 °C: dřív | **R 30** |
|---|---|---|---|---|---|---|
| 25 % | 128 kW | 198 kW | 141 kW | **62 kW** (plášť 1,0 bar abs.) | 74 kW | **61 kW** (1,2 bar abs.) |
| 50 % | 256 kW | 386 kW | 250 kW | **144 kW** (1,0 bar abs.) | 147 kW | **142 kW** (1,8 bar abs.) |
| 75 % | 384 kW | 469 kW | 423 kW | **333 kW** (1,5 bar abs.) | 221 kW | **256 kW** (2,9 bar abs.) |
| 100 % | 511 kW | 490 kW | 490 kW | **490 kW** (3,2 bar abs.) | 295 kW | **289 kW** (3,4 bar abs.) |

Při 25–50 % zdvihu a náplni 60 °C je v plášti ~1 bar abs.: zavzdušňovací ventil drží atmosférický tlak, kondenzát
přitom neodtéká a plášť se pomalu zaplavuje (stall). U varu dává ekviprocentní ventil skoro stejný výkon jako
dřívější model. Lineární ventil dává většinu výkonu už v první polovině zdvihu.

### P2. Přestup tepla na straně náplně (`FP.film_hA`)
- **Míchání:** h ∝ N^⅔·μ^−⅓ (Chilton–Drew–Jebens). Reference je voda 60 °C při 35 Hz.
  Voda 20 °C předává o ~23 % hůř, vroucí voda o ~18 % líp; mladina 12,5 °P při 100 °C zhruba jako voda 60 °C.
- **Bez míchadla:** přirozená konvekce h ∝ (β·ΔT_stěna/ν)^⅓. Učebnicově (voda 60 °C, ΔT 30 K) je to ~0,46
  míchané hodnoty. Model má ale na straně náplně jen ~26 % celkového odporu, ve skutečnosti ~55 %.
  `hNatC` 0,28 proto vrací učebnicový poměr celkového prostupu bez míchání a s mícháním, ~0,6.
  Hustý rmut bez míchadla předá jen ~2 % (zdánlivá viskozita jako u čerpadel), takže se připaluje.
- Nemíchaná mladina 43 hl s plnou párou se ohřívá 0,79 K/min (dřív 0,72), s míchadlem 1,24 K/min (dřív 1,30).

### P3. pH mladiny (`FP.wort_ph`, `FP.boil_step`, `FP.acid_dose`)
- Sladina má pH rmutu + 0,05 (vyslazování, ODHAD). Za varu pH klesá o 0,08 za hodinu (VLB, Kunze).
- Tlačítko **Kyselina mléčná 80 % do mladiny +100 mL** funguje jen tehdy, když je v pánvi mladina.
  Pufr mladiny podle Kunzeho s. 229: 26,6 mEq na kg sladu a pH, takže 530 mL na 700 kg sníží pH o 0,3.
- pH mladiny teď řídí izomeraci chmele, SMM → DMS a barvu. Okyselení před koncem varu dá méně barvy a o něco
  menší hořkost. Okyselení na začátku varu zpomalí rozklad SMM, takže hrozí víc DMS.

### P4. Barva (`FP.color_step`)
- Přírůstek [EBC/h] = 1,2 × e^(−77 kJ/mol/R·(1/T − 1/373 K)) × e^(0,7·(pH − 5,5)) × °P/12. Pod 70 °C se barva netvoří.
  Zahuštění odparem přidává barvu zvlášť (barva je na litr).
- Barva se tvoří ve všech nádobách: při dekokci, ohřevu k varu, za varu, ve vířivce i během chlazení (mladina
  čeká ve vířivce). Při 95 °C je rychlost ~71 % rychlosti při 100 °C, při 80 °C ~25 %.
- Protokol ukáže i odhad barvy piva: 0,65 × zákvasná (Kunze s. 287: zákvasná 11,5–12 → pivo 7–8 EBC).

### P5. Vířivka (`FP.whirl_inlet`)
- Rychlost nátoku = průtok / průřez hrdla. `wpInletMM` 0 znamená stejný průměr jako trubka DN50 v plánku
  (75 mm podle `bore50`): 300 L/min ≈ 1,1 m/s. Když je hrdlo užší, zadej jeho průměr (50 mm → 2,5 m/s).
- Nad 3,5 m/s (Kunze) se část lomu roztříští a neusadí se; s odčerpanou mladinou ubývá.
  Trenažér varuje: „Zpomal čerpadlo“.
- Pod 1,0 m/s (ODHAD) se vířivka roztáčí slabě a kužel je horší.

### P6. Trubky (`FP.pipe_cool_k`)
- τ = (C_vody + C_oceli)/(U·π·D_vnější) na metr. Plná trubka DN50 z plánku (75 mm) za hodinu stání: 80 → 59 °C.
  Dřív za 30 min skoro na teplotu okolí.

### P7. Objem horké kapaliny podle Kella místo lineární roztažnosti.

---

## 4. Návrhy, které v balíčku nejsou (potřebují tvoje údaje nebo rozhodnutí)

Z původního seznamu jsou od verze 1.1 v balíčku: N1 (průtok ledové vody), N2 (odpar z hladiny), N7 (bod varu u dna),
N10 (pára pro bojler) a N14 (bilance při připalování), popis je v kapitole 7. Od verze 1.2 i N19 (stojící čerpadlo),
N20 (ztráty podle hladiny) a z N22 vrstva usazeného díla, popis je v kapitole 8. Zbývá:

| # | Návrh | Proč | Co potřebuju |
|---|---|---|---|
| N3 | Zanášení otopné plochy (připálení) během várky a jeho odstranění louhem (CIP ↔ přestup tepla) | špinavá pánev topí pomaleji, proto má CIP smysl. Číslo pro odpor nánosu rmutu jsem v literatuře nenašel | doba ohřevu čisté pánve a pánve po várce |
| N4 | pH rmutu během rmutování klesá o 0,15–0,30 (Kosař s. 137); pH výstřelků roste s alkalitou vyslazovací vody | třísloviny počítat z pH poslední sladiny, ne z pH rmutu | rozbor vody, pH poslední sladiny |
| N5 | Rozpustnost α-kyselin a ztráty izo-α na kalech místo pevného faktoru 0,44 | využití chmele klesá s dávkou | laboratorní IBU vyražené mladiny a piva |
| N6 | Kyslík: vystírání shora, čerpání, provzdušnění 8–10 mg/L podle teploty a °P | kyslík při vystírání = tmavší barva, horší stabilita | – |
| N8 | Výkon míchadla Np·ρ·N³·D⁵ (teplo do rmutu ~0,5–2 kW) | malý, ale měřitelný vliv při prodlevách | štítek motoru míchadla |
| N9 | Rozpad kužele kalu na konci stahování z vířivky (Kunze: na konci zpomalit) | kal v posledních hl | – |
| N11 | Hodnotit barvu proti odhadu barvy piva, ne proti zákvasné mladině | cíl receptu 9 EBC je nejspíš barva piva | potvrď, co znamená EBC v receptu |
| N12 | Kyselina mléčná do rmutu: 80 % = 10,6 mEq/mL (teď 11,8 = 88 %) | účinek je teď o 11 % silnější | jakou kyselinu používáte (80 / 88 %) |
| N13 | Poměr odporů pára/náplň (HA_REF): ve skutečnosti ~55 % na straně náplně, v modelu ~26 % | míchadlo, viskozita a teplota by pak působily víc; znamená to znovu nastavit `mashFilmK` | doby ohřevu rmutu s míchadlem a bez něj |
| N15 | Objem díla: slad přidá 0,7–0,8 L/kg (Kunze s. 235; Kosař s. 143: 0,7), v modelu 0,66 L/kg | dílo ze 700 kg je v modelu o 30–100 L menší (1–4 %) | výška hladiny po vystírce (měrka) |
| N16 | Kavitace podle NPSH čerpadla: malá sanitární čerpadla potřebují 0,8–1,4 m (Alfa Laval), model ubírá průtok už pod 3 m | u horké mladiny by čerpadlo chrastilo až ke konci stahování, ne dřív | typ čerpadel C1–C3 (štítek) |
| N17 | Konec stahování: u výpusti se tvoří vír a čerpadlo přisává vzduch. Potřebná hloubka nad výpustí S = D·(1 + 2,3·Fr) (ANSI/HI 9.8); 2 % vzduchu ubere 10 % průtoku, 4 % přes 40 % | „ke konci ubrat otáčky“ – teď čerpadlo táhne naplno až do dna | tvar dna a výpusti (rovné/klenuté, lamač víru?) |
| N18 | Vrstvení v nemíchaných nádobách: studená voda přitečená do bojleru zůstane dole, horká nahoře | bojler teď při dopouštění hned zchladne celý; ve skutečnosti dává shora dál horkou vodu | kde má bojler nátok, odběr a topný had |
| N21 | Zanášení deskového chladiče | Kosař s. 185: po třech várkách může být ohřátá voda o víc než 10 °C chladnější | jak často chladič čistíte |
| N22 | Vzhled, další kroky: houpání hladiny se skutečnou periodou (nádoba Ø 2,2 m: 1,6 s), vír u výpusti, pára nad hladinou podle odparu, hromádka šrotu při vystírání | navazuje na V1–V3 | napiš, co ti na plánku vadí nejvíc |
| N23 | Výpust do kanálu přes stojící čerpadlo a zpětný tok horním nátokem (násoska) | H16 počítá jen přetékání mezi nádobami spodními hrdly | – |
| N24 | Stěna nad hladinou jako samostatná hmota (po vypuštění chladne sama, při napouštění bere teplo náplni) | teď má ocel vždy teplotu náplně | hmotnost a izolace nádob |

---

## 5. Co změřit na Zajíci, aby to sedělo

1. **Parní ventil pánve:** typ (ruční / regulační), štítek (Kvs, DN), jestli má plášť zavzdušňovací a odvzdušňovací
   ventil a kam teče kondenzát (protitlak).
2. **Manometr na plášti** (pokud ho máte): tlak při 25 / 50 / 75 / 100 % páry na studenou a na horkou náplň.
   Porovnej s tabulkou v P1, z toho se pozná typ ventilu.
3. **Doby ohřevu:** 2 500 L rmutu 50 → 63 °C při známém otevření páry; plná pánev mladiny 72 °C → var
   s míchadlem a bez něj.
4. **Vířivka:** průměr nátokového hrdla, průtok C1 při vyrážení, teplota na začátku a na konci klidu.
5. **pH a barva:** pH sladiny před varem a vyražené mladiny, EBC vyražené a zákvasné mladiny a piva.
6. **Rozbor vody** (alkalita, Ca, Mg); výchozí odhad dává pH rmutu 5,84.
7. **Rozměry nádob:** vnitřní průměr pánve a vířivky (plocha hladiny, hloubka náplně) a tvar dna.
8. **Chladič:** průtok ledové vody a čím ho ubíráte (klapka, měnič), teplota ohřáté vody do bojleru, teplota mladiny
   na vstupu do chladiče na začátku a na konci chlazení.
9. **Vystírka:** výška hladiny (měrka) před sypáním a po něm – objem, který přidá slad.
10. **Čerpadla:** jsou za C1–C5 zpětné klapky? Když ano, nastav `pumpPass` na 0 (stojící čerpadlo pak nic nepustí).
11. **Izolace nádob:** je plášť pánve a vířivky izolovaný i nad hladinou a na víku? Podle toho se nastaví `dryWallK`
    (izolovaná 0,9–1, holá kolem 0,5). Nejlíp změřit chladnutí plné a poloprázdné nádoby za hodinu.

Hodnoty vlož do `kalibrace.json`, nebo mi je pošli a nastavím je.

---

## 6. Ověření verze 1.0 (stav 29. 10.; aktuální čísla verze 1.1 jsou v kapitole 7.4)

- **Testy na v70 / v152:** 2D (Godot 4.3) 305 kontrol, 0 selhání; 3D (Godot 4.7.2) 305 kontrol, 0 selhání.
  Z toho 36 nových „FP …“ kontrol (pára a plášť, přestup tepla, pH, barva, vířivka, trubky, objem)
  a 2 upravené staré (H9f, H9g): reference přestupu tepla se teď měří při 60 °C.
- **Starší verze:** skript prošel i na 2D v67 (304 kontrol, 0 selhání) a 3D v133 (238 kontrol, 0 selhání).
  Starší 3D násobí filmem v `heat()`, skript to pozná. Verze bez korekce pH izomerace (`isoPh`) ji dostane jako násobek
  a referenční test izomerace pak běží při pH 5,5. U v133 se 2 volitelné háčky testů přeskočí, protože tam ty testy ještě nejsou.
- **Vkládací skript:** zvládá Windows (výpis v cp1250), soubory s CRLF i spuštění ze složky projektu.
  Po aktualizaci programu obnoví zálohy. Když místo nenajde jednoznačně, háček nevloží a napíše proč.
- **Automatická várka 12,5° (700 kg), v70 bez a s FYZIKA+:**

| Veličina | v70 | v70 + FYZIKA+ | Reálně (zdroj) |
|---|---|---|---|
| Objem ve spilce | 37,6 hl | 37,4 hl | – |
| Stupňovitost | 12,68 °P | 12,76 °P | – |
| IBU | 33,1 | 33,0 | cíl 32 |
| Barva vyražené mladiny | – | 11,6 EBC | 8,8–10,9 pro slad 3,9–4,2 EBC (Kunze s. 287); tvůj šrot má ~4,5 → ~9,7–12,0 |
| Barva ve spilce | 11,3 EBC | 12,8 EBC | zákvasná 11,5–12,0 (Kunze s. 287) → pro tvůj šrot ~12,6–13,2 |
| Odhad barvy piva | – | 8,3 EBC | cíl receptu 9 |
| DMS | 15,9 ppb | 16,4 ppb | < 60 ppb |
| pH vyražené mladiny | (pH rmutu 5,84) | 5,78 | 5,4–5,5 při běžné vodě |
| Pára na varnu | – | 20,6 kWh/hl | JRC: infuze 13,2 kWh/hl; + dekokce (+10–20 %) a vyšší odpar (+0,67 kWh/hl na 1 %) ≈ 18–19 |
| Výtěžnost varny | 73,8 % | 73,8 % | 75–80 % (Kunze s. 323) |
| Délka várky | 637 min | 643 min | – |

Barva teď sedí do rozmezí z provozu (přepočteného na tvůj šrot). Dřívější model byl spíš světlý, protože barva
přibývala jen za varu. Výchozí voda (pH rmutu 5,84) barvu zvyšuje: Maillardova část barvy vzniká při pH 5,84
o ~18 % rychleji než při pH 5,6.

---

## 7. Doplněk 16. 11. 2026 (verze 1.1): aby se voda chovala jako kapalina a dílo jako dílo

Prošel jsem znovu všechno, co se týká kapalin a díla: hladiny, toky trubkami, čerpadla, samospád, míchání,
usazování, odpar i to, jak se kapaliny na plánku kreslí. Porovnal jsem to s Kunzem, Kosařem a s podklady výrobců
(Spirax Sarco, Alfa Laval, ASHRAE, ANSI/HI). Co šlo udělat bez tvých údajů, je v balíčku. Zbytek je v návrzích (kapitola 4).

### 7.1 Co už v programu sedí s knihami (neměnil jsem)

| Věc | V programu | Knihy a data |
|---|---|---|
| Voda zadržená v mlátě | 1,0 L na kg sladu | 0,90–1,04 kg/kg (Kosař s. 158: mláto 120–130 kg na 100 kg sladu při 75–80 % vody; Kunze s. 807) |
| Viskozita sladiny 12 % při 20 °C | 1,76 mPa·s | 1,73–2,20 mPa·s (Kunze s. 223) |
| Měrné teplo díla | 3,53 kJ/(kg·K) | 3,6–3,75 (Krones, Briggs); slad 1,67 (model 1,62) |
| Teplota po vystírce | voda 38 °C + 700 kg sladu 18 °C → 35,0 °C | vzorec ABG dává pokles ~2,4 K, zbytek jsou ztráty pánve |
| Usazování díla bez míchadla | hustý díl klesá ke dnu, čerpadlo ho bere první, výpust se ucpává | Kunze s. 243–244, Kosař s. 143 (rychlost usazování knihy neuvádějí) |
| Míchadlo | varuje nad 1 m/s při teplotě nad 57 °C | do 2, nejvýš 3 m/s (Kunze s. 232); 2–3 m/s, pánev do 24 ot/min (Kosař s. 140) |
| Vír od míchadla | centimetry | při 24 ot/min centimetry (Rao 2009) |
| Volný prostor nad dílem | pánev 50 hl, dílo ~25 hl → 50 % | rmutovací pánev 40–50 % (Kosař s. 139) |
| Čerpadla | křivka čerpadla × odpor trasy, zavodnění, chod nasucho, kavitace | odpovídá Alfa Laval a Kunzemu s. 857 (prahy kavitace viz návrh N16) |

### 7.2 Co nesedělo nebo chybělo

| # | Co | Jak to bylo | Jak je to doopravdy | Stav |
|---|---|---|---|---|
| 1 | **Samospád** | 120 L/min na 1 m rozdílu hladin, ať je trubka jakákoli | Průtok určuje průměr a délka trubky a přiškrcení klapek. 40 hl z pánve do kanálu teklo 31 min, teď 5 min | **opraveno (H15)** |
| 2 | **Ledová voda** | pevně 120 L/min, nešla ubrat | Teplota mladiny do spilky se v praxi řídí průtokem ledové vody (Kosař s. 186, Kunze s. 350–352) | **doplněno (H11)** |
| 3 | **Pára do bojleru** | výkon = UA × % ventilu × rozdíl teplot | Stejný ventil a kondenzát jako u pánve; u hadu hrozí stall a rázy (Spirax Sarco) | **opraveno (H10)** |
| 4 | **Připalování** | pánev dostala o 10 % míň, kotel vyrobil 100 % | Energie nesmí mizet | **opraveno (H14)** |
| 5 | **Odpar mimo var** | voda ubývala jen za varu | Horká hladina se odpařuje i pod bodem varu; u zavřené nádoby málo | **doplněno (H12)** |
| 6 | **Rychlost rmutu v potrubí** | bez omezení | Do scezovací kádě pod 1,5 m/s (Kunze s. 903; Kosař s. 140: 1,2–1,5), jinak smyk zhorší scezování | **doplněno (H13)** |
| 7 | **Bod varu u dna** | všude stejný | 2,5 m mladiny = +0,25 bar, u dna vře až při 106 °C (Kunze s. 290) | **v panelu (U3)** |
| 8 | **Kreslení proudů** | rovné čáry ze sprchy, hladina na dopad nereaguje | Voda padá obloukem, dopadá na stěnu a stéká po ní; od dopadu se šíří vlnky | **vzhled (V1, V2)** |
| 9 | Objem díla | slad přidá 0,66 L/kg | 0,7–0,8 L/kg (Kunze s. 235, Kosař s. 143) | návrh N15 |
| 10 | Ohřev rmutu | celý rmut na plnou páru 1,46 K/min | 0,8–1 K/min celý rmut, 1,5–2 K/min dekokční díl (Kosař s. 140) | jen upozornění: v praxi se pára na rmut přivírá |
| 11 | Teplota ve vířivce | za 20 min klidu z 98,5 na 96,0 °C, za 80 min na 89 °C | na chladič přichází 95–98 °C (Kunze s. 352, Kosař s. 184) | `wpUA` je nejspíš vysoké – změřit; ztráty podle hladiny jsou ve verzi 1.2 (H17) |
| 12 | Chladič | ledové vody 1,3× víc než mladiny, do bojleru ~70 °C | 1,05–1,1× a 80–88 °C (Kosař s. 186, Kunze s. 352). To odpovídá `UA1` kolem 50 kW/K při 80 L/min, v modelu je 25 | jen upozornění: `UA1` je odhad – změřit |
| 13 | Kavitace | průtok klesá už pod NPSH 3 m | malá čerpadla potřebují 0,8–1,4 m (Alfa Laval) | návrh N16 |
| 14 | Konec stahování | čerpadlo táhne naplno až do dna | vír u výpusti, čerpadlo přisává vzduch | návrh N17 |
| 15 | Vrstvení v bojleru | celý obsah má jednu teplotu | studená voda zůstává dole | návrh N18 |
| 16 | Stojící čerpadlo | neprůchozí | odstředivé čerpadlo je po vypnutí průchozí | **opraveno ve verzi 1.2 (H16)** |

### 7.3 Co verze 1.1 přidává

**P8. Samospád podle skutečné hydrauliky (H15, `FP.gravity_q`).** Průtok samospádem je ten, při kterém tlaková ztráta
trasy spotřebuje právě rozdíl hladin. Ztráta se počítá stejnou funkcí jako u čerpadel (průměr a délka trubek, místní
ztráty, přiškrcení klapek), s hustotou a viskozitou kapaliny. Hustý rmut teče pomaleji než voda, užší trubkou teče míň.
Příklad z modelu: úsek trubky Ø 75 mm při rozdílu hladin 1 m propustí 848 L/min (3,2 m/s), při 2 m 1 203 L/min;
Ø 45 mm jen 275 L/min. Z pánve do vířivky přeteče za první minutu 327 L (dřív nejvýš 156).
Na konci trasy se odečte i výtoková ztráta (v²/2g), takže rychlost nikdy nepřekročí Torricelliho √(2gh).
Platí pro přetékání mezi nádobami i pro výpust do kanálu.

**P9. Průtok ledové vody jde ubrat klapkou (H11, `FP.ice_flow`).** Čerpadlo ledové vody dává `lvPumpQ` (výchozí 120 L/min
jako dřív, teď jako parametr) a přivřená klapka na trase průtok ubere stejnou křivkou jako jinde v plánku. Když jde
mladina do spilky přechlazená, můžeš „ubrat ledovou“, jak ti trenažér radí. Automat dál řídí teplotu průtokem mladiny.
Panel ukazuje průtok mladiny a ledové vody, jejich poměr a teplotu ohřáté vody.

**P10. Pára do topného hadu bojleru (H10, `FP.steam_coil`).** Stejný model jako plášť pánve: ventil, tlak v hadu,
kondenzát, stall. Plně otevřená pára dá 94 % dřívějšího výkonu (186 místo 197 kW při 40 °C), takže kalibrace `hltUAst`
platí dál. Had pojme míň kondenzátu než plášť (v poměru prostupů – ODHAD).

**P11. Připalování a bilance energie (H14).** Nános při připalování zhorší prostup o 10 % (stejné číslo jako dřív),
ale počítá se v páře: kotel vyrobí přesně tolik, kolik pánev přijme. U přiškrcené páry výkon skoro neklesne,
protože tlak v plášti stoupne.

**P12. Odpar z horké hladiny, když nádoba nevře (H12, `FP.surface_evap`).** Vzorec ASHRAE pro volnou hladinu v klidném
vzduchu: při 90 °C asi 10 kg/(m²·h), při 60 °C asi 2,6. Parametr `evapOpen` říká, jak moc je hladina otevřená:
1 = otevřený průlez, výchozí 0,1 = zavřená nádoba s odtahem (ODHAD). Měřený odpar ve vířivce jsem v knihách nenašel;
uvádějí jen, že mladina přichází na chladič s 95–98 °C, takže odpar je malý. Vířivka 38 hl při 97 °C ztratí za 20 min
klidu 2,3 L (zavřená) nebo 21 L (otevřená). Za celou automatickou várku se mimo var odpaří 19 L. Plocha hladiny se
bere z průměru nádoby (`rvpDiaM`, `wpDiaM`; bez zadání odhad z objemu). Ochlazení odparem se přičítá ke ztrátám
`rvpUA` a `wpUA`. Až je budeš ladit podle měřeného chladnutí, nastav `evapOpen` na 0, jinak se odpar započte dvakrát.

**P13. Rychlost rmutu v potrubí (H13, `FP.transfer_check`).** Nad 1,5 m/s (`mashVmax`) trenažér varuje a smyk zhorší
scezování stejnou cestou jako příliš rychlé míchadlo. Bere se nejužší místo v sání i ve výtlaku. V trubce Ø 75 mm je to
až od 400 L/min, v Ø 50 mm už od 177 L/min.

**P14. Bod varu u dna (U3, `FP.boil_T_bottom`).** Panel u horké pánve ukáže hloubku náplně a teplotu, při které vře
u dna. 40 hl v pánvi Ø 2 m: hloubka 1,33 m, u dna vře až při 102,7 °C (u hladiny 99,3 °C). Na výpočet várky to nemá vliv.

**P15. Kapalina na plánku (V1 a V2, `fyzika_plus_vzhled.gd`, jen kreslení).**
- Sprchové hlavice stříkají do oblouků: část paprsků dopadne na stěnu a stéká po ní, část na hladinu. Boční nátok
  vytéká obloukem. Dřív to byly rovné čáry.
- Hladina se v místě dopadu vlní, kruhy se šíří a slábnou. Rmut je vazký: vlní se třetinovou výškou a jen blízko dopadu.
- Vložení jde vynechat přepínačem `--bez-vzhledu`. Srovnání před a po je na obrázku `proudy_pred_po.png` v balíčku.

### 7.4 Ověření verze 1.1

- **Testy:** 2D v70 (Godot 4.3) 325 kontrol, 0 selhání; 3D v152 (Godot 4.7.2) 325 kontrol, 0 selhání.
  Z toho 56 kontrol FYZIKA+ (20 nových ve verzi 1.1).
- **Starší verze** (archiv zdroje 2D a 3D v repozitáři, 202 původních kontrol): 258 kontrol, 0 selhání. Dva volitelné
  háčky testů se tam přeskočí, protože ty testy ve starší verzi nejsou.
- **Přechod z verze 1.0:** do projektu s první verzí balíčku nový skript doplnil jen nové háčky a parametry
  (325 kontrol, 0 selhání). Druhé spuštění už nic nezměnilo.
- **Vkládací skript:** vyzkoušený i na souborech s CRLF, s tabulátory a na projektu, kde byla část háčků vložená ručně
  (nic se nevložilo dvakrát).
- **Nezávislá kontrola:** doplněk prošel druhý agent, který neznal můj postup. Našel tři chyby, všechny jsou opravené:
  samospádu chyběla výtoková ztráta (krátkou trubkou teklo víc, než dovoluje Torricelli), jeden test by prošel i bez
  funkce, kterou měl hlídat, a skript uměl po ručním vložení půlky háčku vložit druhou půlku dvakrát.
- **Kreslení (V1, V2):** ověřeno na snímcích z programu ve 2D i ve 3D verzi, bez chyb skriptu. Oblouky proudů jsou na
  snímcích vidět; vlnky jsem v pohybu sledovat nemohl, jen na jednotlivých snímcích.
- **Na tvé novější verzi z domu to vyzkoušené není.**
- **Automatická várka 12,5° (700 kg)**, 2D i 3D dávají stejný protokol:

| Veličina | v70 | + FYZIKA+ 1.0 | + FYZIKA+ 1.1 |
|---|---|---|---|
| Objem ve spilce | 37,6 hl | 37,4 hl | 37,2 hl |
| Stupňovitost | 12,68 °P | 12,76 °P | 12,80 °P |
| IBU | 33,1 | 33,0 | 32,9 |
| Barva ve spilce | 11,3 EBC | 12,8 EBC | 12,8 EBC |
| DMS | 15,9 ppb | 16,4 ppb | 15,8 ppb |
| Pára na varnu | – | 20,6 kWh/hl | 20,3 kWh/hl |
| Odpar mimo var | – | – | 19 L |
| Výtěžnost varny | 73,8 % | 73,8 % | 73,8 % |
| Délka várky | 637 min | 643 min | 644 min |

Verze 1.1 várku skoro nemění: odpar mimo var ubere 19 L vody (o to je mladina nepatrně silnější) a bojler bere
o něco míň páry.

## 8. Doplněk 16. 11. 2026 odpoledne (verze 1.2): stojící čerpadlo, ztráty podle hladiny, usazené dílo

Tři věci z návrhů, které šly udělat bez tvých údajů. Každá jde vypnout parametrem.

### 8.1 Co verze 1.2 přidává

**P16. Stojící čerpadlo je průchozí (H16, H16b, `FP.idle_pumps`).** Odstředivé čerpadlo nemá nic, co by po vypnutí
zavřelo průtok. Když je trasa přes něj otevřená, kapalina teče podle rozdílu hladin dopředu i zpátky (spojené nádoby).
Dřív stojící čerpadlo trasu zavíralo jako klapka.
- Platí pro C1–C3, čerpadlo horké vody (C4) i ledové vody (C5). Průtok se počítá stejně jako ostatní samospád (P8)
  a čerpadlo přidá ztrátu K = 5 rychlostních výšek (`pumpPassK`; GIW/KSB uvádí 5 pro volně se protáčející kolo
  a 10 pro stojící, údaj je pro kalová čerpadla). Rychlost beru v trubce za čerpadlem (ODHAD, zdroj to neuvádí).
- Příklady z modelu: sací a výtlačná trubka C1 při rozdílu hladin 1 m propustí 449 L/min (bez ztráty v čerpadle by to
  bylo 865). Z plné pánve přeteče otevřenou trasou přes stojící C1 do scezovací kádě 993 L za 2 min. Když po scezování
  zůstanou klapky otevřené, steče za 10 min 1 212 L sladiny z pánve zpátky pod síta.
- **Doběh:** první 3 s po vypnutí čerpadlo ještě nic nepustí (kolo dobíhá; ODHAD) a stejně dlouho trvá, než se proud
  rozběhne po přestavení klapek. Při zrychleném čase se ta doba násobí rychlostí, takže máš vždycky 3 s skutečného času
  na zavření klapky. Pak deník napíše „kapalina přetéká samospádem otevřenými klapkami“.
- **Automat** po každém kroku zavře klapky (dřív je nechával otevřené do dalšího čerpání). **Automat scezování** po
  skončení zavře klapky, které si otevřel; když jsi mezitím klapkami hýbal ty, nesahá na nic.
- Vypnutí: `pumpPass` = 0 vrátí dřívější chování. **Nastav to, jestli máš za čerpadly zpětné klapky** – to nevím.
- Co to nepočítá: výpust do kanálu přes stojící čerpadlo a zpětný tok horním nátokem (návrh N23).

**P17. Ztráty tepla podle hladiny (H17, `FP.loss_k`).** Ztráty pánve a vířivky do okolí byly stejné, ať byla nádoba
plná nebo skoro prázdná. Teď se smáčená stěna (dno a plášť pod hladinou) počítá naplno a suchá stěna (plášť nad
hladinou a víko) násobkem `dryWallK`:

k = [dno + z·plášť + dryWallK·((1 − z)·plášť + víko)] / [dno + plášť + dryWallK·víko], z = zaplnění

- `rvpUA`, `mpUA` a `wpUA` dál platí pro plnou nádobu. Výchozí `dryWallK` 0,5 je ODHAD pro holou stěnu: `rvpUA`
  0,25 kW/K na ~16 m² povrchu pánve dává ~15 W/(m²·K), což odpovídá neizolované nádobě (inzonex). U dobře izolované
  nádoby rozhoduje odpor izolace a suchá stěna ztrácí skoro stejně jako smáčená: nastav 0,9–1. S hodnotou 1 je
  všechno jako dřív.
- Při výchozím tvaru (pánev výška = průměr, vířivka 3 : 1) vychází: pánev z poloviny plná 0,82, prázdná 0,64;
  vířivka z poloviny 0,88, s 38 hl 0,94.
- Vířivka 38 hl: za 20 min klidu z 98,5 na 96,2 °C (dřív 96,0), za 80 min na 89,6 °C (dřív 89,2). Zbytek 10 hl:
  za 20 min na 91,3 °C (dřív 90,0), za 80 min na 74,1 °C (dřív 70,3).
- Zjednodušení: ocel má pořád teplotu náplně (návrh N24). Bojler a scezovací káď jsem nechal být.

**P18. Usazené dílo na plánku (V3, jen kreslení).** Když se dílo nemíchá, zrno klesá: nahoře se oddělí čirá kapalina
a u dna zůstane tmavší vrstva. Kreslení bere stav z výpočtu – stejnou veličinu, podle které se bez míchání ucpává
výpust (usazeno asi za 5 min, mícháním se do minuty zvedne). Výška vrstvy je objem lože (`skBedLkg` × slad) proti
objemu díla; u vystírky 21 hl to jsou zhruba tři čtvrtiny výšky. Platí pro pánev i pro rmut ve scezovací kádi.
Obrázek `usazene_dilo.png` v balíčku ukazuje pánev s míchadlem, po 90 s a po 5 min klidu.

### 8.2 Co se změní při ovládání

- Po čerpání zavři klapku za čerpadlem (nebo před ním). Jinak se hladiny začnou vyrovnávat a deník to napíše.
- Na plánku je vidět, že automat po každém kroku zavírá klapky.
- V automatické várce přes stojící čerpadla neteče nic, protože automat klapky zavírá. Rozdíl ve výsledcích proti
  verzi 1.1 dělají jen ztráty podle hladiny (rychlejší ohřev dílčích rmutů, o 2 % míň páry).

### 8.3 Ověření verze 1.2

- **Testy:** 2D v70 (Godot 4.3) 333 kontrol, 0 selhání; 3D v152 (Godot 4.7.2) 333 kontrol, 0 selhání.
  Z toho 64 kontrol FYZIKA+ (8 nových: průchodnost stojícího čerpadla oběma směry, doběh, ztráta v čerpadle, tok
  v simulaci, zavírání klapek automatem, ztráty podle hladiny ve vzorci i v simulaci).
- **Starší verze** (archiv zdroje 2D a 3D): 266 kontrol, 0 selhání; vložily se i všechny nové háčky.
- **Přechod z verze 1.0 i 1.1:** skript doplnil jen nové háčky a parametry (333 kontrol, 0 selhání), druhé spuštění
  nic nezměnilo.
- **Vkládací skript:** znovu vyzkoušený na souborech s CRLF, s tabulátory a na projektu s ručně vloženou půlkou H16 a H17.
- **Automat scezování:** proběhl celý, žádný tok samospádem, po skončení zavřel klapky.
- **Automatická várka** s krokem 0,5 s, 2 s a 5 s: v deníku žádné „přetéká samospádem“.
- **Kreslení (V3):** ověřeno na snímcích z programu ve 2D i ve 3D verzi, bez chyb skriptu.
- Tuhle verzi už druhý agent nekontroloval. Prošel jsem ji sám a testy.
- **Na tvé novější verzi z domu to vyzkoušené není.**
- **Automatická várka 12,5° (700 kg)**, 2D i 3D dávají stejný protokol:

| Veličina | + FYZIKA+ 1.1 | + FYZIKA+ 1.2 |
|---|---|---|
| Objem ve spilce | 37,2 hl | 37,2 hl |
| Stupňovitost | 12,80 °P | 12,81 °P |
| IBU | 32,9 | 33,1 |
| Barva ve spilce | 12,8 EBC | 12,8 EBC |
| DMS | 15,8 ppb | 16,3 ppb |
| Pára na varnu | 20,3 kWh/hl | 19,8 kWh/hl |
| Odpar mimo var | 19 L | 19 L |
| Výtěžnost varny | 73,8 % | 73,8 % |
| Délka várky | 644 min | 637 min |

### 8.4 Co jsem cestou našel (neměnil jsem)

- Ráz při rozjezdu vorlaufu závisí na délce kroku simulace: s krokem 0,5 s automat „trhne“ ložem (+51 mbar), s krokem
  2 s ne. Je to v původním programu i bez balíčku.
- Automat u kroku „Proplach pod síty“ nejdřív otevře trasu z bojleru pod síta a teprve pak zjistí, že plánek nemá
  výpust do kanálu, a krok přeskočí. Voda při tom neteče (klapky se hned zavřou), jen to na plánku problikne.

## Zdroje

- Spirax Sarco – Control valve sizing for steam systems; Heating with coils and jackets; Air venting theory. https://spiraxsarco.com/learn-about-steam
- TLV – Stall phenomenon; Temperature problems caused by air. https://www.tlv.com/steam-info/steam-theory
- B. Meyer (VLB Berlin): Wort boiling, Craft Brewers Conference 2015. https://www.craftbrewersconference.com/wp-content/uploads/2015_presentations/W1440_Burghard_Meyer.pdf
- Kroh et al. (TU Berlín): Simplified kinetics and colour formation in sucrose solutions based on α-dicarbonyl compounds. https://depositonce.tu-berlin.de/items/13e09ab2-3369-4ced-86fc-e2eccb232d73
- EU JRC – Best environmental management practice, brewery energy. https://greenbestpractice.jrc.ec.europa.eu/print/pdf/node/237
- Briggs of Burton – Brewhouse heating (IBD 2017). https://briggsofburton.com/wp-content/uploads/technical-papers/IBD-DCCC-BHHeat-Jan-2017-Briggs.pdf
- Inzonex – Heat loss from brewery kettles, tuns & whirlpool. https://inzonex.co.uk/calc/articles/brewery-heat-loss-kettles-tuns/
- IAPWS-IF97 (průmyslová formulace vlastností vody a páry), oblast 4; G. S. Kell (1975): hustota vody 0–150 °C.

Doplněno ve verzi 1.1:
- Spirax Sarco – Heating with coils and jackets (topné hady, stall). https://www.spiraxsarco.com/learn-about-steam/steam-engineering-principles-and-heat-transfer/heating-with-coils-and-jackets
- ASHRAE Handbook 1989, kap. 20, rov. 1 (odpar z volné hladiny), podle https://www.aivc.org/sites/default/files/airbase_4059.pdf
- Alfa Laval Pump Handbook (NPSH, čerpání horkých kapalin). https://acuityprocess.com/wp-content/uploads/2019/06/Alfa_Laval_Pump_Handbook.pdf
- ANSI/HI 9.8 – minimální ponoření sání. https://datatool.pumps.org/pump-fundamentals/submergence ; vzduch v čerpadle: https://www.pumpsandsystems.com/guidelines-submergence-air-entrainment
- V. Scheer (Krones): Thermal process engineering in brewing, MBAA Ontario 2015 (měrné teplo díla a mladiny, deskový chladič). https://www.mbaa.com/districts/Ontario/Events/Documents/Thermal%20process%20engineering%20in%20brewing%20%20MBAA%20Ontario,%20January%202015.pdf
- American Brewers Guild – Brewhouse Calculations (teplota vystírky). https://www.abgbrew.com/abg_pdf/Brewhouse_Calculation.pdf
- F. T. Dodge (SwRI): The New Dynamic Behavior of Liquids in Moving Containers (houpání hladiny). https://www2.swri.org/www2/fluid-slosh/dodge-sp-106-update.pdf
- Rao et al. (2009): vír od míchadla, ScienceAsia 35:183. https://www.scienceasia.org/2009.35.n2/scias35_183.pdf
- Engineering Toolbox – výtok z nádoby (Torricelli). https://www.engineeringtoolbox.com/flow-liquid-water-tank-d_1753.html
- W. Kunze: Technology Brewing and Malting, 3. angl. vyd. (s. 222–223, 229, 232, 235, 243–244, 286–287, 290–291, 299, 323,
  340–341, 350–352, 807, 857, 903) a Kosař, Procházka: Technologie výroby sladu a piva (s. 136–137, 139–140, 143, 158, 163,
  184–186). Jen čísla s odkazem na stranu, bez textu knih.

Doplněno ve verzi 1.2:
- GIW Industries (KSB): Understanding the practice of pumping through (ztráta ve stojícím čerpadle, K ≈ 5 a 10). https://giw.ksb.com/blog/understanding-the-practice-of-pumping-through
