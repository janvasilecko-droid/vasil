# FYZIKA+ – jak úpravu vložit do novější verze trenažéru

Balíček je udělaný tak, aby šel vložit do **jiné (novější) verze**, než na které vznikl (2D v70 / 3D v152).
Všechno počítání je v novém souboru `fyzika_plus.gd`. Do stávajících skriptů se vkládají jen krátké „háčky“
(1–3 řádky), které se hledají **podle textu**, ne podle čísel řádků.

## 1. Automaticky (doporučeno)

Potřebuješ Python 3.8 nebo novější, žádné další knihovny.

```
python aplikuj_fyzika_plus.py "D:\stazene\Trenazer-varny-aplikace" --zkouska   ← jen vypíše, co by udělal
python aplikuj_fyzika_plus.py "D:\stazene\Trenazer-varny-aplikace"             ← vloží
```
Přepínače: `--bez-ui` vynechá úpravy panelu (U1–U3), `--bez-vzhledu` vynechá kreslení kapalin (V1–V3).

Balíček je i v repozitáři `janvasilecko-droid/vasil`, složka `trenazer-zdroj/fyzika-plus/` (`git pull`), takže ho
nemusíš přenášet ručně.

**Už máš vloženou verzi 1.0 nebo 1.1?** Spusť nový skript na stejnou složku ještě jednou. Pozná, co už je vložené,
a doplní jen nové háčky a parametry verze 1.2.

Cesta je složka projektu, kde leží `project.godot` a `sim.gd`. Když zadáš nadřazenou složku, skript `sim.gd` najde sám.
Pro 3D verzi spusť skript ještě jednou se složkou 3D projektu, protože obě verze mají vlastní kopii skriptů.

Co skript dělá:
- zkopíruje `fyzika_plus.gd`, `fyzika_plus_tests.gd` a `fyzika_plus_vzhled.gd` do projektu;
- vloží háčky H0–H17, U1–U3, V1–V3 a R1 (tabulka níže). Od verze 1.2 sahá i do `auto.gd` a `lauter_auto.gd`
  (po jednom řádku: automat po kroku zavře klapky). **Háček, jehož místo nenajde přesně jednou, přeskočí a napíše proč.**
  Skupina úprav, která spolu souvisí, se vloží buď celá, nebo vůbec. Nic se tedy nerozbije napůl;
- před první změnou si každý soubor zazálohuje jako `<soubor>.pred-fyzika-plus`;
- zapíše `fyzika_plus_hooks.gd` se seznamem vložených háčků. Testy podle něj přeskočí, co chybí;
- jde spustit znovu: už vložené háčky pozná a podruhé je nevloží. Platí to i pro háček vložený ručně jen zčásti:
  hotovou část nechá a doplní zbytek.

Ověření po vložení (Godot 4.3 pro 2D, Godot 4.7 pro 3D):
```
Godot --headless --path "<projekt>" -- --tests       ← všechny testy včetně 64 nových „FP …“
Godot --headless --path "<projekt>" -- --autotest    ← celá automatická várka
```
Testy nespouštěj souběžně ve dvou kopiích projektu: mají stejný název, sdílejí `user://` (uložení várky)
a test „uložení: várka se načte“ pak náhodně selže.

**Vrácení:** přejmenuj `*.pred-fyzika-plus` zpět na původní názvy a smaž `fyzika_plus.gd`, `fyzika_plus_tests.gd`,
`fyzika_plus_vzhled.gd` a `fyzika_plus_hooks.gd`.

**Nastavení bez úprav kódu:** všechny nové parametry jdou přepsat v `kalibrace.json` jako ostatní, např.
`{"stValveR": 1.0, "rvpKvs": 40.0, "jVacBreaker": 0}`.

Hlavní nové parametry a výchozí hodnoty (celý seznam se zdroji je ve `fyzika_plus.gd`, konstanty `DEF` a `RANGE`):

| Parametr | Výchozí | Význam |
|---|---|---|
| `stValveR` | 30 (ODHAD) | charakteristika parního ventilu: 1 = lineární (ruční), 30–50 = ekviprocentní (regulační) |
| `rvpKvs`, `mpKvs` | 0 | Kvs parního ventilu pánve; 0 = dopočítat (plně otevřený ventil při 60 °C ubere 10 % tlaku) |
| `stCondBack` | 0,15 bar (ODHAD) | protitlak kondenzátu; pod ním kondenzát z pláště neodtéká (stall) |
| `jDrainKgS` | 0,6 kg/s (ODHAD) | kapacita odvaděče kondenzátu (plná od rozdílu tlaků 0,5 bar) |
| `jVacBreaker` | 1 | plášť má zavzdušňovací ventil (0 = nemá, po zavření páry podtlak) |
| `hNatC` | 0,28 | přestup bez míchadla (přirozená konvekce) vůči míchané referenci |
| `colK100`, `colEa`, `colPh` | 1,2 EBC/h; 77 kJ/mol; 0,7 | tvorba barvy při 100 °C, pH 5,5 a 12 °P |
| `phBoilDrop`, `phSparge`, `wortBuf` | 0,08/h; 0,05; 26,6 | pokles pH za varu, sladina proti rmutu, pufr mladiny |
| `wpInletMM` | 0 | průměr nátokového hrdla vířivky; 0 = jako trubka v plánku |
| `wpVmax`, `wpVmin` | 3,5 m/s; 1,0 m/s (ODHAD) | nad tím se lom tříští; pod tím se vířivka roztáčí slabě |
| `pipeU` | 10 W/(m²·K) | ztráty holé trubky do okolí |
| `hltKvs` | 0 | Kvs parního ventilu bojleru; 0 = dopočítat |
| `lvPumpQ` | 120 L/min (ODHAD) | průtok čerpadla ledové vody při otevřených klapkách (dřív pevně v kódu) |
| `rvpDiaM`, `mpDiaM`, `wpDiaM` | 0 | vnitřní průměr nádoby v metrech; 0 = odhad z objemu (pánev výška = průměr, vířivka 3 : 1) |
| `evapOpen` | 0,1 (ODHAD) | odpar z hladiny mimo var: 1 = otevřený průlez, 0,1 = zavřená nádoba s odtahem, 0 = vypnuto |
| `mashVmax` | 1,5 m/s | nejvyšší rychlost rmutu v potrubí (Kunze s. 903) |
| `pumpPass` | 1 | stojící čerpadlo je průchozí (samospád a zpětný tok). Na Zajíci zpětné klapky za čerpadly nejsou (potvrzeno 16. 11.), proto 1; 0 = jako dřív |
| `pumpPassK` | 5 | ztráta ve stojícím čerpadle v násobcích rychlostní výšky (GIW/KSB: 5 volně se protáčí, 10 stojí) |
| `dryWallK` | 0,5 (ODHAD) | ztráty suché stěny nádoby proti smáčené; 1 = ztráty nezávisí na hladině (jako dřív, dobře izolovaná nádoba) |

## 2. Háčky – co, kam a proč (pro ruční vložení nebo pro Clauda doma)

Odsazení: sim.gd v70 používá 4 mezery. Pokud tvoje verze používá tabulátory, skript to převede sám.
Při ručním vkládání drž odsazení okolních řádků.

| Háček | Soubor | Najdi | Udělej |
|---|---|---|---|
| **H0** | sim.gd | `const Physics = preload("res://physics.gd")` (případně `extends RefCounted`) | Pod něj přidej řádek `const FP = preload("res://fyzika_plus.gd")` |
| **H1** | sim.gd | hlavičku `func _pd(v, UA: float, jL: float, Tst: float) -> Dictionary:` | Hned pod ni jako první řádek těla dej `return FP.steam_pd(self, v, UA, jL, Tst)`. Starý kód pod ním už neproběhne (Godot jen upozorní na nedosažitelný kód) |
| H1 (2. část) | sim.gd | v `sim_step()`: `S._pd.rvp.P *= film_factor(rvp);S._pd.mp.P *= film_factor(mp)`; ve starších verzích v `heat()` řádek `Pin *= ff` | **Zakomentuj** (`#`). Film už je ve `FP.steam_pd`, jinak by se započítal dvakrát. **Bez této části H1 nevkládej** |
| **H2** | sim.gd | hlavičku `func film_hA(v) -> float:` | Jako první řádek těla dej `return FP.film_hA(self, v)` |
| **H3a** | sim.gd | v `react()`: `var ph_d: float = clampf(float(S.get("pH", 5.5)), 5.0, 5.8)` | Nahraď `float(S.get("pH", 5.5))` za `FP.vessel_ph(self, v)` |
| **H3b** | sim.gd | v `react()` řádek s `isoPh` a `S.get("pH", 5.5)` | Nahraď `float(S.get("pH", 5.5))` za `FP.vessel_ph(self, v)`. Když verze ještě nemá `isoPh`, místo toho na konec řádku `var k1 = 790000000000.0 * exp(-11858.0 / TK)` přidej ` * FP.iso_ph_factor(self, v)` |
| **H4** | sim.gd | začátek `react()`: `func react(v, m: float) -> void :` a pod ním `if v.w < 1: return` | Pod `if v.w < 1: return` přidej `FP.color_step(self, v, m)` |
| H4 (2. část) | sim.gd | v bloku varu `v.col += liqVol(v) * 2.5 / 60.0 * m` | Nahraď za `FP.boil_step(self, v, m)` (pevný přírůstek barvy zmizí, barvu počítá color_step). **Obě části H4 jen spolu** |
| **H5** | sim.gd | v `dstGive()` u „wp“: `S.wp.whirlMin += ctx.get("m", 0.0)` | Změň na `S.wp.whirlMin += ctx.get("m", 0.0) * FP.whirl_inlet(self, S.wp, p, ctx)` |
| H5 (2. část) | sim.gd | v usazování kalu: `var fl: float = 0.1 * v.trubHB` | Změň na `var fl: float = minf(v.trubSus, 0.1 * v.trubHB + FP.trub_ns(self, v))`. **Obě části H5 jen spolu** |
| **H6** | sim.gd | v `net_step()`: `e.c.T += (c.ambientT - e.c.T) * kcool` a `for b in e.q: b.T += (c.ambientT - b.T) * kcool` | První řádek změň na `var kc_fp: float = FP.pipe_cool_k(self, e, dt); e.c.T += (c.ambientT - e.c.T) * kc_fp`, ve druhém nahraď `kcool` za `kc_fp` |
| **H7** | sim.gd | `func hotV(v)…: return liqVol(v) * (1.0 + 0.00052 * maxf(0.0, v.T - 20.0))` | Změň na `return liqVol(v) * FP.expansion(v.T)` |
| H7b | sim.gd | v `density()`: `var exp_f: float = 1.0 + 0.00052 * maxf(0.0, v.T - 20.0)` | Změň na `var exp_f: float = FP.expansion(v.T)` |
| **H8** | sim.gd | `const CFG_DEFAULT: = {` a `const CALIB_RANGE: = {` | Hned za otevírací závorku vlož nové klíče s výchozími hodnotami a rozsahy (seznam je ve `fyzika_plus.gd`, konstanty `DEF` a `RANGE`). Klíče, které už v souboru jsou (např. `isoPh`), přeskoč |
| H8p | params.gd | `const META: = {` | Popisy nových parametrů (zdroj, nejistota), aby byly v PARAMETRY.md |
| **H9** | tests.gd | `var AU = preload("res://audit_tests.gd").new();AU.run(self)` | Pod něj přidej `var FPT = preload("res://fyzika_plus_tests.gd").new();FPT.run(self)` |
| H9f | tests.gd | referenční test přestupu tepla: `var thin = sim.vessel(); thin.w = 2000.0; thin.stir = true; thin.hz = 35.0` | Doplň `;thin.T = 60.0`. Referenční přestup teď platí pro vodu 60 °C, protože nově závisí na teplotě |
| H9g | audit_tests.gd | `var w = sim.vessel(); w.w = 2000.0; w.stir = true; w.hz = 35.0` | Doplň `; w.T = 60.0` (stejný důvod) |
| H9h | audit_tests.gd | `var vh1 = _mkv(100.0); vh1.alpha = 100.0…` (referenční test izomerace) | Nad řádek dej `sim.S["pH"] = 5.5` (v70 to už má). Při pH 5,5 je korekce izomerace neutrální, takže referenční hodnota testu platí dál |
| U1 | ctl.gd | `refs.hop = _btn(r, "", func(): m.sim.hopPlanAdd();refresh())` + v `refresh()` řádek `else: _t(refs.hop, "Chmel podle receptu přidán…` | Tlačítko „Kyselina mléčná 80 % do mladiny +100 mL (pH …)“ a jeho popisek |
| U2 | ctl.gd | `_t(refs.boiler, "Pára z kotle: …")` | Za text přidej `+ preload("res://fyzika_plus.gd").jacket_label(m.sim)`. Zobrazí tlak v plášti pánve, zaplavení kondenzátem a vzduch |
| R1 | sim.gd | `S.boiler.load = Qb / c.boilerKW` a `rep.merge(yields())` | Pod první dej `FP.energy_step(self, Qb, dt)`, pod druhý `FP.report_extra(self, rep)`. Protokol pak ukáže kWh páry na hl, pH mladiny a odhad barvy piva |
| **H10** | sim.gd | v `sim_step()`: `"hlt": {"P": c.hltUAst * h.heat * maxf(0.0, Tst - h.T), "wet": 1.0}` | Nahraď za `"hlt": FP.steam_coil(self, h, c.hltUAst, Tst)`. Pára do topného hadu bojleru stejným modelem jako plášť pánve |
| **H11** | sim.gd | v `net_step()`, blok čerpadla ledové vody (je v něm `var Lm = 120.0 / maxf(1.0, dests.size())`): `for d in dests:` a pod ním `var path = path_to(r, d.k)` | Pod řádek `var path = …` přidej `Lm = FP.ice_flow(self, path, dests.size())`. Průtok ledové vody pak respektuje přiškrcení klapek |
| **H12** | sim.gd | v `heat()` řádek `v.T = T`; v `sim_step()` u vířivky `…;react(wv, m)` | Pod `v.T = T` přidej `FP.surface_evap(self, v, name, dt)`; za `react(wv, m)` přidej `;FP.surface_evap(self, wv, "wp", dt)`. **Obě části jen spolu** |
| **H13** | sim.gd | v `net_step()`, smyčka čerpadel: `var part0 = srcTake(sb, Qf, m, key)` | Pod něj přidej `FP.transfer_check(self, key, sb, part0, Qf, sp_, m, dpaths)`. Hlídá rychlost rmutu v sání i výtlaku |
| **H14** | sim.gd | v `heat()` řádek `Pin *= 0.9` (připalování) | Nahraď za `set_meta("fpH14", true)`. Úbytek výkonu pak počítá `FP.steam_pd`, stejný pro kotel i pánev. **Jen s H1** |
| **H15** | sim.gd | v `_vessel_gravity()`: `var q: float = 120.0 * f * minf(1.3, sqrt(bhd / 1.05))`; ve smyčce výpustí `var Lmin: float = tot * o.f / outs.size(); var part = null` | První nahraď za `var q: float = FP.gravity_q(self, pa, bhd, gb)`, druhý za `var Lmin: float = FP.gravity_q(self, o.path, maxf(0.0, head_m + (net.nodes[o.lk.k].y - (v.y + v.h)) / PX_PER_M), b); var part = null`. Potřebuje funkce `path_head()` a `_src_fluid()` |
| U3 | ctl.gd | `lines.append("[b]Spilka:[/b] %s" % m.vessel_text("spilka"))` | Pod něj přidej `for fp_l in preload("res://fyzika_plus.gd").panel_lines(m.sim): lines.append(fp_l)`. Řádky o chladiči a bodu varu u dna |
| V1 | main.gd | hlavičku `func _stream_segs() -> Array:` | Jako první řádek těla dej `return preload("res://fyzika_plus_vzhled.gd").stream_segs(m)`. Proudy do nádob padají obloukem |
| V2 | main.gd | v `_liquid()`: řádek `fxs["lv"] = lv;fxs["slosh"] = slosh` a dvakrát výraz `amp * 0.5 * sin(px * 0.11 - tt * 4.7)` (podruhé s `px2`) | Pod první přidej `fxs["fpimp"] = preload("res://fyzika_plus_vzhled.gd").impacts(m, it, fxs, lv)`; za oba výrazy připiš `+ preload("res://fyzika_plus_vzhled.gd").ripple(fxs, px, tt, is_mash)` (podruhé s `px2`). **Jen s V1** |
| **H16** | sim.gd | v `bfs()`: `if pass2.has(pb.get("bind", "")) and a.side == "in":` a pod ním `for k2 in net.pumpNodes.get(a.id, {}).get("out", []):`; v `_vessel_gravity()`: `var gr = bfs(sts)` | První změň na `… and (a.side == "in" or pass2.has("~rev")):`, druhý na `….get("out" if a.side == "in" else "in", []):`, třetí na `var gr = bfs(sts, "liq", FP.idle_pumps(self))`. Stojícím čerpadlem pak teče kapalina samospádem oběma směry |
| H16 (2. část) | auto.gd | v `_end_step()` řádek `i += 1` | Nad něj přidej `if preload("res://fyzika_plus.gd").cf(sim, "pumpPass") >= 0.5: _close_all()`. Automat po každém kroku zavře klapky, jinak by to stojícím čerpadlem teklo dál nebo zpátky. **Obě části H16 jen spolu** |
| H16b | lauter_auto.gd | ve `stop()` řádek `sim.S.p2.on = false;sim.S.skWater.on = false` | Pod něj přidej `preload("res://fyzika_plus.gd").lauter_stop(sim)`. Automat scezování po skončení zavře klapky, které si otevřel. **Jen s H16** |
| **H17** | sim.gd | v `heat()`: `var UA: float = c.rvpUA if name == "rvp" else c.mpUA`; v `sim_step()` u vířivky `wv.T -= c.wpUA * (wv.T - c.ambientT) * dt / Cw` | Pod první přidej `UA *= FP.loss_k(self, name, v)`; ve druhém vlož za `c.wpUA` násobek `* FP.loss_k(self, "wp", wv)`. Ztráty tepla podle zaplnění nádoby. **Obě části jen spolu** |
| V3 | main.gd | v `_liquid()` dvojici řádků `if b != "lv":` a pod ním `LiquidFx.draw_caustics(…)` | Nad ni přidej `preload("res://fyzika_plus_vzhled.gd").settled_layer(self, m, it, b, fxs, outline, lv, y0, tt, is_mash)`. Kreslí usazené dílo |

Závislosti: H1–H7, H10–H13, H15–H17, U1, U3 a R1 potřebují H0. U2 a H14 potřebují H1, H9f a H9g potřebují H2,
H9h potřebuje H3b, V2 potřebuje V1, H16b potřebuje H16. Na ostatních háčcích nic nezávisí.

**Co se s H16 změní v ovládání:** když čerpadlo stojí a trasa přes něj je otevřená, kapalina teče podle rozdílu hladin
(i zpátky). Po čerpání proto zavři klapku – máš na to 3 s skutečného času (při 10× rychlosti 30 s simulace), pak se
proud rozběhne a deník napíše „kapalina přetéká samospádem“. Oba automaty si klapky zavírají samy.

## 3. Pro Clauda doma (zadání)

> V projektu trenažéru varny (Godot) vlož balíček FYZIKA+ ze složky `fyzika-plus`.
> 1. Spusť `python aplikuj_fyzika_plus.py "<složka projektu>" --zkouska` a přečti si zprávu.
> 2. Když hlásí jen přeskočené volitelné háčky (·), spusť skript bez `--zkouska`.
>    Když hlásí přeskočený hlavní háček (✗), otevři INTEGRACE.md, najdi v sim.gd místo s tou funkcí podle smyslu
>    (verze se mohla změnit) a háček vlož ručně. Pak skript spusť znovu, aby zapsal `fyzika_plus_hooks.gd`.
> 3. Spusť testy (`-- --tests`) a automatickou várku (`-- --autotest`). Porovnej protokol s tabulkou
>    „Ověření“ ve FYZIKA_PLUS.md (kapitoly 6, 7.4 a 8.3): objem, °P, IBU, EBC, DMS mají vyjít podobně (EBC ve spilce
>    o ~1,5 výš, pára ~20 kWh/hl, odpar mimo var ~20 L). V deníku automatické várky nesmí být „přetéká samospádem“.
> 4. Nic jiného neměň. Pravidla projektu: žádná vymyšlená čísla (odhad označit ODHAD), nesahat na trasy,
>    klapky a polohy čerpadel ani na plán editoru. Text z knih Kosař a Kunze se do repozitáře nedává, jen čísla
>    s odkazem na stranu.
