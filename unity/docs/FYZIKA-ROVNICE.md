# Fyzika a chemie simulátoru varny Zajíc – rovnice, předpoklady, ověření

Platí pro `GodotSrc/*.gd` (zdroj pravdy) a z něj generovaný C# v `Assets/Pivovar/GdGen`. Každá rovnice má: **tvar**, **proměnné/jednotky**, **druh** (ZÁKON = fyzikální zákon · EMPIRICKÝ = korelace z literatury · VLASTNÍ = vlastní aproximace modelu, ODHAD = konstanta nutná změřit), **předpoklady** a **test s nezávislou referencí** (`audit_tests.gd`, sekce „REF“ a „AUDIT“).

Pravidlo: čísla, která nejsou z uvedeného zdroje, jsou označena ODHAD a jsou v registru `params.gd` / [KALIBRACE.md](KALIBRACE.md) s nejistotou a návodem, jak je změřit. Nejsou zde vymyšlené citace ani křivky čerpadel.

## 0. Co se změnilo v tomto kole (před/po)

| Oblast | Dřív | Teď |
|---|---|---|
| Vodovodní řad | pevný limit průtoku `mainsQ` × „faktor škrcení“ (škrcení se počítalo dvakrát, tlak nebyl nikde) | **zdroj tlaku**: `H_zdroj(Q)=H_řad·(1−(Q/Q_max)²)`, paralelní větve řešeny společnou výškou uzlu, škrcení klapkou jen přes místní ztrátu K(p) (`mains_solve`) |
| Reakce 1. řádu (cukernatění, α/β-amylasa, SMM→DMS, kvašení, diacetyl) | explicitní Euler `min(1, k·Δt)` (chyba O((kΔt)²), při velkém kroku 100 % naráz) | přesné řešení `1−e^(−kΔt)` (`first_order`); výsledek nezávisí na délce kroku |
| Chmel IBU | konstanta 0,44 natvrdo v kódu | parametr `iboKeep` (ODHAD, rozsah 0,2–1,0) |
| Vířivka | skok po 3 min víření (časovač) | plynulá závislost `k = k_klid + (k_vír − k_klid)(1 − e^(−t_vír/τ))`, parametry `trubKstill`, `trubKwhirl`, `whirlTau` (ODHAD) |
| CIP | „hotovo ✔“ vypadalo jako ověřená čistota | hlášení a hodnocení říkají **„podmínky postupu splněny – SIMULOVANÝ výsledek“**; mikrobiologie se nesimuluje |
| Testy | 202 | **265** (z toho 30 z kontroly 25. 10.), 0 selhání |

## 0b. Kontrola parametrů a rizikových výpočtů (25. 10. 2026) – skutečné opravy

Podrobný přehled všech 111 parametrů (použití v kódu, výchozí hodnota, rozsah): [AUDIT-PARAMETRU.md](AUDIT-PARAMETRU.md), generuje `Tools/audit_params.py` (v CI; selže při nekonzistenci). Zdroj pravdy výchozích hodnot je jen `CFG_DEFAULT`.

| # | Chyba | Oprava (soubor) | Regresní test |
|---|---|---|---|
| 1 | Kotel počítal odběr páry `UAst·(T_p − T)` **bez** zhoršení přestupu u husté náplně, zatímco nádoba přijala `× film_factor` → kotel „vyráběl“ až 2,2× víc, než se předalo (tlak páry a zatížení kotle nepravdivé) | `sim.gd sim_step`: `S._pd.rvp.P *= film_factor(rvp)` (totéž mp); `heat()` už film neaplikuje podruhé. `P_dodaný = UAst·h·wet·(T_p − T)·UA(film)/UA_ref` | REGRESE kotel: výroba = přijatá energie (< 1 %); přetížení: Σvýroba − Σodběr = Δ(buben) |
| 2 | Referenční přestup na straně náplně `HA_REF = 25 kW/K` byl pevný – při kalibrovaném `rvpUAst > 25` by odpor stěny vyšel záporný | `ha_ref() = max(25, 2·rvpUAst)`; odpor stěny+páry `1/gs = 1/UA_ref − 1/hA_ref` | REGRESE přestup tepla: voda při 35 Hz = faktor 1 i pro rvpUAst = 30 |
| 3 | Počáteční tlak páry 2,5 bar natvrdo (ignoroval `steamBar`) | `"boiler.p": c.steamBar` | REGRESE kotel: počáteční tlak = steamBar |
| 4 | Tabulka sytosti končila na 8 bar abs. (steamBar až 8 bar přetlaku = 9,0 bar abs.) a nad ní vracela 170 °C (pokles) | přidán bod 10 bar → 179,9 °C; za tabulkou poslední hodnota | REGRESE tabulka sytosti |
| 5 | Zásoba kapaliny v potrubí z jiné geometrie (`2 L/m·(DN/50)²`) než tlakové ztráty (`bore_mm`, `pipeLenScale`) | `pipe_hold_L = π/4·d²·L·pipeLenScale·1000`; spojené úseky jen při stejném DN | REGRESE potrubí: zásoba = π/4·d²·L·pipeLenScale; rychlost 400 L/min v DN50 = 1,5 m/s |
| 6 | `lvUA` (0,15 kW/K) a objem lože na kg sladu (2,7 L/kg) byly pevné literály mimo registr | nové parametry `lvUA`, `skBedLkg` (ODHAD, v registru) | REGRESE ledovka, REGRESE lože |
| 7 | Kvašení (μ, q, Ks, ferTa …) a CIP (ohřev stanice, teploty, časy) měly pevné literály | 23 nových parametrů `fer*`, `cip*` s META a rozsahy (**ODHAD** – model není fitován na měřený průběh) | REGRESE kvašení/CIP: změna parametru mění výsledek |
| 8 | Migrace starých uložení po načtení **přepsala** ručně kalibrovanou `skPerm` < 5e-12 na 1e-11 | migrace běží PŘED `apply_calibration` a vrací `CFG_DEFAULT.skPerm` (`main.gd`) | KALIBRACE skPerm 3e-12 přežije načtení |
| 9 | Kalibrace dovolila nesmyslné kombinace (hltStart > hltCap, lvStart > lvVol, práh slehávání ≥ práh kanálků) | křížové kontroly v `apply_calibration` | KALIBRACE vazba (2×) |
| 10 | Duplicitní výchozí hodnoty (`PUMPS` const vs `CFG_DEFAULT`; záložní literály `get("mpCap", 5000)` vs 6000, `skPerm` 1e-11 vs 1,5e-11, `trubKstill` 1/90 vs 0,0111) | jediný zdroj `CFG_DEFAULT`; `audit_params.py` hlídá záložní literály | `audit_params.py` v CI (job param-audit) |
| 11 | `boilMin`, `pitchT`, `targetEBC`, `targetIBU`, `targetP` bez popisu | META: zdroj RECEPT = provozní nastavení / cíl receptu, nikoliv fyzika (viz KALIBRACE.md) | registr: 111/111 popsaných |

Testy tohoto kola: rozsahy všech parametrů, extrémní hodnoty (každý z 111 parametrů na dolní i horní mezi → žádné NaN/∞), nulové průtoky deskového chladiče (mladina i chladivo), nezávislý ε-NTU, extrakt při rmutování (součet zůstává 300,0000 kg), citlivost na krok (0,5 vs 0,05 min: 287,7 vs 288,5 kg), `betaAct`, `kdBeta63`, teplotní optimum β-amylázy (podíl zkvasitelných 0,96 při 63 °C vs 0,54 při 72 °C – nic není nastaveno na cílové prokvašení).

**Co kontrola NEmění (a proč):** `chillerKW` zůstává konstantní kapacita (nezávisí na teplotě odpařování ani kondenzace – nutno změřit výkon na odpařovači); NPSH čerpadel se nemodeluje; konstanty `K = 0,4` na úsek a `0,191` (podíl pevných látek v mlátě) jsou ODHAD v kódu, ne v registru. Fermentace a CIP jsou fenomenologické/procedurální a o přesnosti nic netvrdí.

## A. Hmota a extrakt, výtěžnosti

* Sledované veličiny v nádobě: voda `w` [kg], extrakt `f+dx+ld+ns+sg` [kg], pevné `g+su+sc+ni` [kg]. Hustota a °Plato: `P = 100·E/(E+W)`, `ρ = 1 + 0,004·P` kg/L (VLASTNÍ, dobrá aproximace pro 5–20 °P).
* **Definice výtěžností** (`yields()`):
  * `e_lab = Σ m_slad·(1−vlhkost)·extrakt_sušiny` – laboratorní (potenciální) extrakt sladu [kg],
  * `e_vyr` – extrakt v mladině při vyrážce (z pánve), `VV_varna = e_vyr/m_slad·100` [kg/100 kg],
  * `ucinnost = e_vyr/e_lab·100` [%], `VV_cz = V·0,96·ρ·P/100/m_slad·100` (český výpočet z objemu).
  * Vztah: **VV_varna = účinnost × e_lab/m_slad**. Účinnost 80,7 % při e_lab/m = 0,764 dává VV = 61,7 kg/100 kg – oba údaje v dřívějších hlášeních jsou tedy konzistentní, nešlo o chybu (a Kunzeho 75–80 % je „účinnost“ vůči laboratornímu extraktu, ne VV).
* Bilance extraktu celé várky (`bilance`): Σ(pánev, MP, VK, SK, spilka, potrubí, vyteklo, mláto, škrob v loži) = vyráběný extrakt; autobrew hlásí rozdíl ≈ 0,01 kg z 538 kg.
* Testy: `REF výtěžnost`, `REF var: extrakt se zachová`, `AUDIT protlačka`, autobrew „BILANCE EXTRAKTU“.

## B. Hydraulika

1. **Ztráta v potrubí** (ZÁKON Darcy–Weisbach + EMPIRICKÝ koeficient tření): `H = (f·L/D + ΣK)·v²/(2g)` [m]. `f = 64/Re` pro Re < 2300, jinak Swamee–Jain `f = 0,25/[log10(ε/(3,7D)+5,74/Re^0,9)]²`, ε = 0,015 mm (ODHAD hladké nerezi). Místní ztráta K = 0,4 na úsek (ODHAD), klapka `K = 0,6·(1/p)^4` (VLASTNÍ, p = otevření 0,05–1). Průměr z DN: `bore25/40/50` (ODHAD), délky z plánku × `pipeLenScale` (ODHAD 0,4).
   Testy: `REF potrubí` – laminárně Hagen–Poiseuille (shoda < 2 %), turbulentně iterované Colebrook (shoda < 2 %, f = 0,0176), Q²-chování.
2. **Čerpadlo s měničem**: křivka `H(Q)=H0/(1+0,3·th) − A·Q²`, `A = 0,25·H0/Qn²` (VLASTNÍ parabola; **H0 a Qn jsou modelové, ne z typového štítku** – ODHAD ±40 %; `th` = zahuštění rmutu). Měnič drží zadaný průtok, frekvence z afinity `n ∝ √(H/H0)` (`hz = 50·√((H_sys+A·Q²)/H0)`). Když trasa neumožní, hledá se průsečík bisekcí; nad H0 neteče nic.
   Testy: `REF čerpadlo` (Q = √((H0−Hst)/A) = 734,8 L/min, hz = 27,24 Hz, žádný průtok nad H0), `AUDIT C1`.
3. **Vodovodní řad** (nové): `H_řad = p_řad/(ρg)` (`mainsBar`, ODHAD 4 bar), `Q_max` (`mainsQ`, ODHAD 900 L/min = průtok při volném výtoku). Každá otevřená větev má `H_n = path_head_i(Q_i) + Δz_i`; uzel řeší `H_n = H_řad·(1−(ΣQ/Q_max)²)` půlením. Výsledek se pamatuje, dokud se nezmění síť/škrcení.
   Výstup při výchozích parametrech: 630 L/min do ledovky; při dvojnásobném tlaku 739 L/min (≈ √ΔH); při klapce 20 % otevřené 156 L/min.
   Testy: `AUDIT řad` (6 testů: tlaková bilance uzlu, mez Q_max, sublinearita, škrcení, determinismus, hmotová bilance plnění 315 L = 315 L).
4. **Ráz při trhnutí klapkou** (ZÁKON, inertní tlak): `Δp = ρ·L·Δv/Δt`, L = 12 m (ODHAD), Δt = krok 0,5–3 s. Je to horní odhad inertního tlaku, ne Allievi/Joukowského model s pružností potrubí.
5. **NPSH / kavitace**: v modelu není. Sání se kontroluje jen přes „lift“ (výška > H0) a nasátí vzduchu při nízké hladině. Pro doplnění by byla potřeba křivka NPSHr čerpadel – **nepřidáno, protože ji nemáme** (viz „Chybějící měření“).

## C. Ohřev, pára, var

* `C = c_voda·m_w + 1,5·(E+pevné) + m_ocel·c_ocel` [kJ/K] (`cC`, `steelC`; c_voda = 4,19, c_ocel = `cpSteel` 0,50 – LITERATURA).
* `dT/dt = (P_in − UA·(T − T_okolí))/C` (ZÁKON, bilance energie), `P_in` = pára přes film (`UAst`, `mashFilmK`, ODHAD), v hustém rmutu zhoršený film a připalování (`burn`) – VLASTNÍ.
* Bod varu `T_b = tbWater + 0,03·°P` [°C] (tbWater = 99,3 °C pro ~200 m n. m.; 0,03 K/°P je ODHAD pořadu desetin K při 12 °P).
* Odpar nad bodem varu: `m_odp = (P_net·Δt − C·(T_b − T))/2257` [kg] (ZÁKON, výparné teplo vody při 100 °C = 2257 kJ/kg). Hmota i energie se zachovávají.
* Testy: `AUDIT energie` (chyba < 2 % na 65 K), `REF ohřev` (T = T0 + (E_dodaná − E_ztracená)/C: 31,52 vs 31,52 °C), `AUDIT var` (přebytek tepla = odpar × 2257, 4–10 %/h), nezávislost na kroku 1 s vs 5 s.

## D. Rmutování a enzymy

* **Inaktivace amylas** (EMPIRICKÝ Arrhenius, Marc et al. 1983 – konstanty podle kódu, exponent ze skenu **ověřit**): `k = k63·exp[(Ea/R)(1/336,15 − 1/T)]`; α: k63 = 0,022 /min, Ea = 83,7 kJ/mol (Ea/R = 10067 K); β: 0,13 /min, 188 kJ/mol (22612 K); stabilní podíl β (13 %) 40× pomalejší (De Schepper 2022). Řešení `A = A0·e^(−k·t)` (přesné).
* **Aktivita α/β-amylasy**: rychlost = `k0·[enzym]/V·exp(−((T−T_opt)/σ)²)·faktor_pH` (Gaussovo teplotní optimum 72/9 °C a 63/7 °C – **VLASTNÍ aproximace**, tvar není z literatury); konverze nyní `1−exp(−r)`.
* **Sacharifikace škrobu**: gelatinace `su` (logistika kolem 62 °C, šířka 1,3 K – VLASTNÍ), `sc` hrubý/jílový škrob se rozpouští pomalu.
* **pH rmutu**: vlastní model chemie vody (`proc.gd`), ne titrační křivka sladu – ODHAD.
* Testy: `REF α-amylasa` (5,1685 vs 5,1685, krok 30 min = 1800 × 1 s), jednotlivé rmutovací testy v `tests.gd`.

## E. Scezování

* **Darcy**: `Q = A·k·Δp/(μ·L)` → odpor `R = μ·(L_lože/k_lože + L_těsta/k_těsta)/(1−0,62·dep)²/(A·(1+1,5·ch))` [Pa·s/m³]; `Q_max = Δp_dostupný/R`, kde `Δp = ρ·g·h_nad_ložem + skSuction`.
* **Stlačitelné lože** (VLASTNÍ): `k = k0/(1+Δp/skDpComp)`; slehávání nad prahem `skDpCompact` (80 mbar) a kanálky nad `skDpChannel` (160 mbar) – **prahy jsou ODHAD**, kalibrovat tlakoměrem pod sítem.
* **Viskozita sladiny**: voda `μ = 2,414·10⁻⁵·10^(247,8/(T+133,15))` Pa·s, sladina × `e^(0,047·°P)` (LITERATURA, rozsah 0,04–0,055).
* Konec vyslazování 0,7–1 °P (Kosa), zbytek extraktu v mlátě je držen jako `skRetentionLkg` (ODHAD).
* Testy: `AUDIT lože/scezování/přetahování/kypření` (Δp 15–90 mbar, slehávání, podtlak, propustnost), `ZADÁNÍ bilance vody/extraktu`.

## F. Chmelovar, IBU, DMS, kaly, pěna

* **Izomerizace α-kyselin** (EMPIRICKÝ, Malowicki & Shellhammer 2005 – konstanty podle kódu): `k1 = 7,9·10¹¹·e^(−11858/T)`, `k2 = 4,1·10¹²·e^(−12994/T)` [1/min, T v K]. Řešení následných reakcí `α → iso → degradace`: `α = α0·e^(−k1 t)`, `iso = k1·α0·(e^(−k1 t) − e^(−k2 t))/(k2 − k1)` (uzavřený vzorec, přesný pro libovolný krok).
  Při 100 °C po 60 min: 47,6 % α → iso.
* **IBU v mladině** = `iso[mg]·1000/V20 × iboKeep`, **iboKeep = 0,44 je ODHAD** (ztráty na kalech/pěně/adsorpce). Výsledné IBU autobrew: ≈ 30,4–30,8 (cíl 32). Kalibrovat laboratorním IBU z várky.
* **DMS** (Dickenson 1979): `k_SMM→DMS(100 °C) = 0,01824…0,0213 /min` (pH 5,2→5,5), `×2^((T−100)/6)`; odpar DMS při varu `k = výparnost/hmotnost × 70` (VLASTNÍ). Testy `REF SMM` (3,3474 vs 3,3474).
* **Kaly (hot break)**: `trubHB = 0,0006·m·min(1, coag)`, `coag` roste s intenzitou varu (VLASTNÍ, ODHAD).
* **Pěna / překypění**: tvorba `10·evap²·fp`, rozpad `e^(−t/1 min)`, překypění když pěna přesáhne volný objem (VLASTNÍ, ODHAD).
* Testy: `REF chmel` (3 testy), `REF var`.

## G. Vířivka

* Usazování kalu: `dTrub/dt = −k·(trubSus − 0,1·trubHB)`, `k = k_klid + (k_vír − k_klid)·(1 − e^(−t_vír/τ))`; k_klid = 1/90 min⁻¹, k_vír = 0,2 min⁻¹, τ = 1,5 min – **vše ODHAD** (fyzikální pozadí: Stokesova sedimentace + sekundární proudění „čajového hrnku“; model je fenomenologický).
* 10 % kalu zůstává v suspenzi (VLASTNÍ).
* Testy: `REF vířivka` (plynulost 0,95 / 5,79 / 7,43 / 7,77 kg pro 0 / 1 / 3 / 8 min víření; shoda s uzavřeným vzorcem 7,771 vs 7,771; po 40 min klidu 9,00 z 9,00 kg).

## H. Deskový chladič

* ε-NTU protiproud (ZÁKON, Kays & London): `ε = (1−e^(−NTU(1−Cr)))/(1−Cr·e^(−NTU(1−Cr)))`, `Cr = 1: ε = NTU/(1+NTU)`, `NTU = UA/C_min`; `T_vyst = T_vst − ε·C_min·(T_vst−T_chl)/C_mladina`.
* UA roste s průtokem jako `Nu ∝ Re^0,65` (EMPIRICKÝ exponent pro deskové výměníky, ODHAD pro tento typ), UA1/UA2 platí při `plateRefQ` (80 L/min mladiny) – **ODHAD ±40 %**, kalibrovat 4 teplotami + průtoky.
* Mladina: ρ = 1,04 kg/L, c = 4,0 kJ/(kg·K). Teplo odebrané mladině = teplo přijaté chladivem (test), mladina nemůže být chladnější než vstup chladiva (2. zákon).
* Testy: `AUDIT chladič` (1. a 2. zákon), `REF výměník` (4 kombinace Cr, ε → 1 pro nekonečnou plochu, ε = 0 pro nulovou).

## I. Kvašení, ležení

* Monod-typ: `μ = 0,05·a·S/(8+S)·(1−X/4)`, spotřeba `0,2·a·X·S/(6+S)`, teplotní faktor `a = e^(0,095(T−12))` (VLASTNÍ, ODHAD), poměr extrakt : etanol : CO₂ podle Ballingovy rovnice (2,0665 : 0,9565 …), exoterma 0,6 MJ/kg zkvašeného extraktu (ODHAD).
* Diacetyl: tvorba z α-acetolaktátu (k1 = 0,012·e^(0,11(T−12)) /h), redukce kvasnicemi – nyní přesná exponenciála (ODHAD).
* Nevalidováno proti měřenému průběhu kvašení – **nutno změřit** (denní sacharometrie + teplota + diacetyl).

## J. CIP

* Model sleduje jen **dodržení postupu**: teplota roztoku (ohřev stanicí 1,2 K/min – ODHAD), koncentrace louhu ≥ `cipLyeMin`, ≥ 75 °C po 20 min; kyselina ≤ 60 °C, ≥ `cipAcidMin`, 20 min; voda 5 min. Nic se nepočítá o mikrobiologii (log redukce, biofilm, mrtvé větve, rychlost proudění ≥ 1,5 m/s, pokrytí postřikem).
* Zobrazení: „postup splněn (simulace)“ a ve výsledcích „Postup CIP … (dodržení postupu, ne ověřená čistota)“. Skutečné ověření: ATP/otěr, vodivost a pH posledního proplachu, titrace.

## K. Nezávislost na časovém kroku

* Reakce 1. řádu: přesné řešení (test: 600 × 1 s = 1 × 10 min na 1e-9; α-amylasa, SMM, izo-α shodné na 6 platných míst).
* Ohřev/var: 1 s vs 5 s (rozdíl < 0,3 K), `AUDIT ohřev nezávisí na časovém kroku`.
* **Zbývá explicitní krok** (nepřesné pro velké Δt): film/Tsens relaxace (už exponenciála), `heat()` (lineární s krokem 1 s; při Δt ≥ 30 s by se mělo dělit), lože při velkém `dpPa`. Aplikace běží s Δt ≤ 1 s, zrychlení času se dělí na 1s kroky (`sim_step`).

## L. Čeho si je třeba všimnout (limity)

* Všechno stojí na konstantách označených ODHAD; po jejich změření se bude výstup posouvat. Seznam a poučení: [KALIBRACE.md](KALIBRACE.md).
* Číselné výsledky jsou ověřeny **testy s nezávislou referencí** a **autobrew**, ne měřením na skutečné varně.
* Unity vizuálně zatím nezkoušeno (kompilace v CI ano, běh v editoru ne).
