# FYZIKA+ – vypočtené hodnoty místo odhadů (verze 1.6, 17. 11. 2026)

Tady jsou údaje, které jsem dřív nechával jako odhad a čekal na měření ze Zajíce. Teď jsou spočítané z fyziky
a z čísel v knihách. Není to měření: u každé hodnoty je napsané, z čeho vychází, a co by ji změnilo. Kde výpočet
nedává spolehlivé číslo, je to řečeno.

Předpoklady, které nejdou spočítat a beru je jako obvyklé u varny 40 hl:
- nádoby jsou izolované (80 mm minerální vlny pod oplechováním, ledovka 100 mm polyuretanu); Kosař s. 180 mluví
  o „dobře izolované“ vířivce,
- tvar nádob podle objemu (pánev výška = průměr 1,85 m; vířivka Ø 2,67 m; scezovací káď Ø 2,6 m podle `skArea`),
- okolí 20 °C.

## 1. Co se změnilo v programu (háček K1 a výchozí hodnoty balíčku)

| Parametr | Bylo (odhad) | Je (výpočet) | Jak jsem k tomu došel |
|---|---|---|---|
| `rvpUA` ztráty pánve | 0,25 kW/K | **0,02** | prostup izolací 0,53 W/(m²·K) × 16,2 m² × 1,2 (hrdla, nohy) + průlez 1,6 W/K + parník 7,5 W/K = 19 W/K |
| `mpUA` ztráty mladinové pánve | 0,3 | **0,022** | stejně, 60 hl |
| `wpUA` ztráty vířivky | 0,35 | **0,02** | 18,7 m² → 21 W/K |
| `skUA` ztráty scezovací kádě | 0,2 | **0,02** | Ø 2,6 m, výška 0,94 m → 21 W/K |
| `lvUA` ztráty ledovky | 0,15 | **0,01** | 26,7 m², 100 mm PUR: 0,27 W/(m²·K) × 1,2 = 9 W/K |
| `UA1` deskový chladič | 25 kW/K | **50** | návrhový bod z knih, viz níže |
| `dryWallK` suchá stěna proti smáčené | 0,5 | **0,95** | odpory: izolace 1,78 + vně 0,125 m²K/W; nad hladinou navíc ~0,1 → 1,905/2,005 |
| `evapOpen` odpar z hladiny mimo var | 0,1 | 0,1 (potvrzeno) | s vypočtenými ztrátami dává 0,1 teplotu mladiny na chladič 95–97 °C, jak uvádí Kosař s. 180 a Kunze s. 352; s 0,3 by to bylo 92 °C |
| `wpConeV` mez klesání hladiny u kužele | 0,6 cm/min | 0,6 (odvozeno z praxe) | běžné spílání 40 hl za 50–60 min (Kosař s. 181) dává ve vířivce s H : D 0,4–0,6 pokles 1,6–2,5 cm/min a na konci se má zpomalit; mez je tedy pod tím, beru asi třetinu |

`bojler` (`hltUA` 0,01) už vypočtené hodnotě odpovídal (vychází 0,015), neměnil jsem ho.

**Deskový chladič (`UA1`).** Mladina 75 L/min (40 hl za 50–60 min) z 97 na 8 °C je 463 kW. Ledová voda 1,5 °C
v poměru 1,08 : 1 (Kosař s. 186, Kunze s. 352: 1,05–1,1) se tím ohřeje na 83,3 °C – přesně do rozmezí 80–88 °C,
které knihy uvádějí. Střední rozdíl teplot je 9,7 K, takže chladič musí mít prostup 48 kW/K při 75 L/min, tj. 50 kW/K
při referenčních 80 L/min. S původními 25 kW/K by ohřátá voda měla jen kolem 60 °C.

**Ztráty nádob.** Původní hodnoty 0,25–0,35 kW/K odpovídaly holé nerezové nádobě (15–19 W/(m²·K)). Izolovaná nádoba
ztrácí vedením asi patnáctkrát míň. Většinu tepla ve vířivce pak bere ohřev samotné nádoby při napouštění
(550 kg oceli) a odpar do parníku, ne stěny.

**Velikost dílčích rmutů (háček A1).** Automat bral na první rmut pevně polovinu díla a na druhý dvě třetiny. To
vycházelo jen proto, že scezovací káď mezitím vychladla o 7 K. S izolovanými nádobami by dílo po prvním rmutu mělo
73 °C a po druhém 88 °C. Automat teď velikost rmutu počítá z tepelné bilance, aby po vrácení prvního rmutu mělo dílo
63 °C a po druhém 76 °C (Kosař s. 143: 62–64 °C a 75–78 °C). Vychází 30 % a 44 % díla – klasické třetiny. V bilanci
je i ocel kádě a 4,5 K, o které rmut cestou vychladne v potrubí. V simulaci pak vyjde 62,8 °C a 76,0 °C.

## 2. Co z toho vychází (automatická várka 12,5°, 700 kg)

| Veličina | Dřív (1.5) | Teď (1.6) | Knihy |
|---|---|---|---|
| Vířivka po 20 min klidu | 93,8 °C | 97,1 °C | – |
| Mladina na chladič (průměr) | 85,8 °C | 95,2 °C | 95–97 °C (Kosař s. 180), 95–98 °C (Kunze s. 352) |
| Ohřátá voda z chladiče (průměr) | 60,8 °C | 85,6 °C | 80–88 °C |
| Teplota do spilky | 5,4–10,0 °C | 8,5–10,0 °C | cíl 8,5 °C |
| Chlazení | 91 min | 81 min | 50–60 min u nových instalací |
| Dílo po 1. / 2. rmutu | 68,9 / 82,2 °C | 62,8 / 76,0 °C | 62–64 / 75–78 °C (Kosař s. 143) |
| Pára na varnu | 19,8 kWh/hl | 16,7 kWh/hl | 13,2 infuze (JRC), dekokce a vyšší odpar víc |
| Dosažitelné prokvašení | 76 % | 84 % | světlé ležáky 78–85 % (zdánlivé) |
| DMS | 16 ppb | 25 ppb | pod 60 ppb |
| Bojler na konci | 75,0 °C | 83,4 °C | – |

Chlazení trvá 81 min, protože čerpadlo ledové vody dává chladičem 60 L/min (`lvPumpQ` 120 L/min se dělí do dvou
větví). Na 55 min by bylo potřeba 74 L/min ledové přes chladič. Výkon čerpadla ledové je pořád odhad.

**Automat teď při chlazení řídí i ledovou vodu** (`autoIce`): pustí jí jen tolik, aby mladina šla do spilky s 8,5 °C,
a průtok mladiny přidává, dokud ledová stačí. S lepším chladičem by jinak první mladina šla do spilky s 2 °C.

## 3. Spočítáno, ale program jsem kvůli tomu neměnil

| Věc | Výsledek | Co z toho plyne |
|---|---|---|
| Výkon míchadla (návrh N8) | lopatky Ø 1,1 m, 17 ot/min při 20 Hz (obvod 1 m/s): 21 W; při 35 Hz 112 W | teplo od míchadla je zanedbatelné (0,04 K za hodinu), návrh N8 ruším |
| Vír u výpusti (N17) | potřebná hloubka nad výpustí Ø 75 mm: 0,30 m při 300 L/min, 0,11 m při 48 L/min (ANSI/HI 9.8) | u vířivky (C3 ~50 L/min) hrozí přisávání vzduchu až v posledních ~600 L, u pánve při vyrážení v posledních ~8 hl; zavedení potřebuje vědět, jestli máš ve dně lamač víru |
| Kavitace (N16) | mladina 97 °C: bez výšky hladiny je k dispozici jen 0,86 m; malé čerpadlo potřebuje 0,5–1,5 m | čerpadlo metr pod hladinou stačí; program chce 3 m, takže varuje dřív, než by musel |
| Odvodnění kalového kužele | Kozeny–Carman pro vločky 30–80 µm (Kosař s. 179) dává 30–220 cm/min | to je mnohem víc, než jak klesá hladina. Buď je kal o dva řády míň propustný (vločky se slehnou), nebo kužel rozpadá ztráta vztlaku (druhý důvod u Kunzeho s. 341). Výpočet tedy mez nedá, proto je `wpConeV` odvozená z praxe |
| Objem díla (N15) | slad 4,5 % vody; sušina 0,62–0,67 L/kg → 0,64–0,66 L/kg, knihy 0,7–0,8 | rozdíl dělá vzduch ve šrotu a v pluchách po vystření; po odvzdušnění model sedí |
| Plášť pánve | `rvpUAst` 6,5 kW/K: při prostupu 2 kW/(m²·K) je to 3,2 m² otopné plochy; odpar 8 %/h ze 44 hl potřebuje 5,6 kW/K | odpovídá, neměnil jsem. Celý rmut by na plnou páru hřál 1,5 K/min, v praxi se pára přivírá na 1 K/min (Kosař s. 140) |
| Kondenzát | 650 kW páry = 0,30 kg/s kondenzátu; `jDrainKgS` 0,6 je dvojnásobek | odpovídá běžnému dimenzování odvaděče |

## 4. Co spočítat nejde

- Typ parního ventilu (`stValveR`), výkon čerpadla ledové vody (`lvPumpQ`), průměry hrdel a nádob: to jsou vlastnosti
  tvojí varny, ne fyzika. Zůstávají jako odhad.
- Jestli jsou nádoby opravdu izolované. Kdyby nebyly, vrať původní hodnoty přepínačem `--bez-vypoctu` nebo
  v `kalibrace.json` (`rvpUA` 0,25, `wpUA` 0,35, `dryWallK` 0,5).
- Podíl kalu, který se při rozpadu kužele strhne (`wpConeBreak`).

Zdroje: Kosař, Procházka: Technologie výroby sladu a piva (s. 140, 179–182, 184, 186); Kunze: Technology Brewing and
Malting (s. 341, 352); ANSI/HI 9.8; EU JRC (spotřeba tepla varny). Jen čísla s odkazem na stranu.
