extends RefCounted
## Audit fyziky: bilance energie a hmoty, 2. zákon u výměníku, nezávislost na kroku, realistické chování lože při scezování a hydraulika čerpadel.

var T   # tests.gd (chk, near, fresh, bed)
var sim
var m

func run(t) -> void:
	T = t; sim = t.sim; m = t.m
	_energy()
	_plate()
	_hlt_boil()
	_lauter()
	_pumps()
	_protlacka()
	_process()
	_ui_regress()
	_practice_tests()
	_physics_lit_tests()
	_spec_tests()
	_steel_tests()
	_calib_tests()
	_ferm_cip_tests()
	_mains_tests()
	_reference_tests()
	_whirl_tests()
	_regrese_zadani()

func _open(a: String, b: String, pumps: Array, where := -1) -> void:
	var vv = sim.route_valves(a, b, pumps, where)
	if vv == null: return
	for id in vv:
		if m.outlet_open.has(id): m.outlet_open[id] = true
		elif sim.net.byId.has(id): sim.net.byId[id]["open"] = true
	sim.valve_ver += 1

# ---------- 1. termodynamika pánve ----------
func _energy() -> void:
	T.fresh(); var S = sim.S; var c = S.cfg
	var v = S.rvp; v.w = 2000.0; v.T = 20.0; v.Tsens = 20.0; v.stir = true; v.heat = 0.6
	var Ein := 0.0; var Eloss := 0.0
	var H0: float = (sim.cC(v) + sim.steelC(c.rvpSteel))*v.T
	for i in 1500:
		sim.sim_step(1.0)
		Ein += v.Pin*1.0; Eloss += c.rvpUA*(v.T - c.ambientT)*1.0
		if v.T > 85.0: break
	var H1: float = (sim.cC(v) + sim.steelC(c.rvpSteel))*v.T
	var err: float = absf((Ein - Eloss) - (H1 - H0))/maxf(1.0, Ein)
	T.chk("AUDIT energie: pára do pláště = ohřev vody a oceli + ztráty (1. zákon)", err < 0.02, "%.2f %% (dodáno %d MJ)" % [err*100.0, roundi(Ein/1000.0)])
	var rate: float = (v.T - 20.0)/(1500.0/60.0)
	T.chk("AUDIT ohřev 2000 L páru 60 %: realistická rychlost 0,5–3 K/min", rate > 0.5 and rate < 3.0, "%.2f K/min" % rate)
	# var: energie navíc jde do odparu
	v.w = 4500.0; v.T = sim.S.cfg.tbWater; v.heat = 0.85; var w0: float = v.w; var E2 := 0.0; var L2 := 0.0
	for i in 600:
		sim.sim_step(1.0); E2 += v.Pin; L2 += c.rvpUA*(v.T - c.ambientT)
	var evap: float = w0 - v.w
	var err2: float = absf((E2 - L2) - evap*2257.0)/maxf(1.0, E2)
	T.chk("AUDIT var: přebytek tepla = odpařená voda × 2257 kJ/kg", err2 < 0.03, "odpar %.0f L/h, chyba %.1f %%" % [evap*6.0, err2*100.0])
	var pct_h: float = evap*6.0/w0*100.0
	T.chk("AUDIT var plné pánve (4500 L, pára 85 %): odpar 4–10 % objemu za hodinu", pct_h > 4.0 and pct_h < 10.0, "%.1f %%/h" % pct_h)
	# nezávislost na časovém kroku
	var Ts := []
	for dt in [1.0, 5.0]:
		T.fresh(); var w = sim.S.rvp; w.w = 2000.0; w.T = 20.0; w.Tsens = 20.0; w.stir = true; w.heat = 0.6
		for i in int(1200.0/dt): sim.sim_step(dt)
		Ts.append(w.T)
	T.chk("AUDIT ohřev nezávisí na časovém kroku (1 s vs 5 s)", absf(Ts[0] - Ts[1]) < 0.3, "%.2f vs %.2f °C" % [Ts[0], Ts[1]])

# ---------- 2. deskový chladič: energie a 2. zákon ----------
func _plate() -> void:
	T.fresh(); var S = sim.S
	var pid := ""
	for it in m.items: if it.type == "vessel" and it.get("shape", "") == "plate": pid = it.id
	if pid == "": T.chk("AUDIT chladič existuje v plánku", false); return
	S.plates[pid] = {"kind": "water", "T": 12.0, "flow": 200.0}
	var Tout: float = sim.plate_cool(pid, 95.0, 150.0, 1.0/60.0)
	var Tc: float = S.plates[pid].Tcout
	var Qw: float = 150.0/60.0*1.04*4.0*(95.0 - Tout); var Qc: float = 200.0/60.0*4.19*(Tc - 12.0)
	T.chk("AUDIT chladič: teplo mladiny = teplo chladiva (1. zákon)", absf(Qw - Qc) < 0.01*Qw, "%.1f vs %.1f kW" % [Qw, Qc])
	T.chk("AUDIT chladič: mladina se neochladí pod vstup chladiva a chladivo neohřeje nad mladinu (2. zákon)", Tout >= 12.0 and Tc <= 95.0, "mladina %.1f °C, voda %.1f °C" % [Tout, Tc])

# ---------- 3. bojler: nad bodem varu se voda odpařuje, energie se neztrácí ----------
func _hlt_boil() -> void:
	T.fresh(); var S = sim.S; var h = S.hlt
	h.T = S.cfg.tbWater - 0.1; h.heat = 1.0; h.auto = false; var w0: float = h.w
	for i in 600: sim.sim_step(1.0)
	T.chk("AUDIT bojler: voda nepřekročí bod varu, přebytek tepla odpaří vodu", h.T <= S.cfg.tbWater + 0.01 and h.w < w0 - 1.0, "%.2f °C, odpar %.1f L/10 min" % [h.T, w0 - h.w])

# ---------- 4. scezování: odpor lože, podtlak pod sítem, utahování mláta, kypření ----------
func _draw_run(minutes: float, Q: float, rake: bool, rakeH := 0.4) -> Dictionary:
	var S = sim.S; var sk = S.sk
	sk.rake = rake; sk.rakeH = rakeH; sk.hydT = null
	var dp0 := -1.0; var dpmax := 0.0; var got := 0.0
	for i in int(minutes*6.0):
		sim.sim_step(10.0)
		sk.Qact = 0.0; sim.skQmax()
		var p = sim.skDraw(Q, 10.0/60.0)
		got += sim.liqVol(p)
		sim.skAddWater(sim.liqVol(p), 77.0, false)      # vyslazování drží hladinu
		if dp0 < 0: dp0 = sk.dp
		dpmax = maxf(dpmax, sk.dp)
	return {"dp0": dp0, "dp": sk.dp, "dpmax": dpmax, "c": sk.c, "Qmax": sk.Qmax, "got": got, "teig": sk.teig}

func _lauter() -> void:
	T.bed(); var sk = sim.S.sk; sim.skAddWater(800.0, 77.0, false); sk.settle = 30.0
	sim.skQmax(); var q0: float = sk.Qmax
	sk.Qact = 0.0; sim.skDraw(40.0, 0.05)
	T.chk("AUDIT lože: čerstvé lože při 40 L/min má Δp 15–90 mbar", sk.dp > 15.0 and sk.dp < 90.0, "%.0f mbar (Qmax %.0f L/min)" % [sk.dp, q0])
	# rozumný odběr s kypřením: Δp drží, mláto se neutáhne
	T.bed(); sim.skAddWater(800.0, 77.0, false); sim.S.sk.settle = 30.0
	var a := _draw_run(90.0, 40.0, true)
	T.chk("AUDIT scezování 40 L/min s mělkým kypřením 90 min: Δp pod 100 mbar, lože se neutáhne", a.dpmax < 100.0 and a.c < 0.5, "Δp %.0f→%.0f mbar, max %.0f, c %.2f" % [a.dp0, a.dp, a.dpmax, a.c])
	# stejné bez kypření: horní těsto a sedání lože zvyšují odpor
	T.bed(); sim.skAddWater(800.0, 77.0, false); sim.S.sk.settle = 30.0
	var b := _draw_run(90.0, 40.0, false)
	T.chk("AUDIT bez kypření roste Δp (horní těsto + sedání lože)", b.dp > a.dp*1.2, "%.0f vs %.0f mbar, těsto %.1f vs %.1f kg" % [b.dp, a.dp, b.teig, a.teig])
	# přetahování: lože se utáhne
	T.bed(); sim.skAddWater(800.0, 77.0, false); sim.S.sk.settle = 30.0
	var c := _draw_run(15.0, 150.0, false)
	T.chk("AUDIT přetahování 150 L/min: do 15 min Δp nad 150 mbar a propustnost spadne o víc než 40 %", c.dpmax > 150.0 and c.Qmax < q0*0.6, "Δp max %.0f mbar, Qmax %.0f→%.0f L/min" % [c.dpmax, q0, c.Qmax])
	T.chk("AUDIT přetahování: pod sítem vzniká podtlak (čerpadlo chce víc, než lože pustí)", sim.S.sk.vac > 0.0, "%.2f" % sim.S.sk.vac)
	# kypření utažené lože povolí
	# zadřené lože: zastavit odběr, hluboko nakypřit, pak pomalu znovu
	var sk2 = sim.S.sk; sk2.Qact = 0.0; sim.skQmax(); sim.skDraw(30.0, 0.05)
	var dp_tight: float = sk2.dp; var c_tight: float = sk2.c
	_draw_run(10.0, 0.0, true, 0.7); sim.kypr()
	var d := _draw_run(2.0, 30.0, true, 0.4)
	T.chk("AUDIT zadřené lože: zastavit, hluboko nakypřit → Δp při 30 L/min výrazně klesne", d.c < c_tight*0.5 and d.dp < dp_tight*0.6, "c %.2f→%.2f, Δp %.0f→%.0f mbar" % [c_tight, d.c, dp_tight, d.dp])
	# vyšší lože = větší odpor
	T.bed(); sim.skAddWater(800.0, 77.0, false); var s2 = sim.S.sk; sim.skQmax(); var qa: float = s2.Qmax
	s2.g *= 1.5; sim.skQmax()
	T.chk("AUDIT vyšší vrstva mláta (víc sladu) = menší propustnost", s2.Qmax < qa*0.8, "%.0f → %.0f L/min" % [qa, s2.Qmax])
	# chladná sladina teče pomaleji (viskozita)
	T.bed(); sim.skAddWater(800.0, 77.0, false); var s3 = sim.S.sk; sim.skQmax(); var qh: float = s3.Qmax
	s3.T = 50.0; sim.skQmax()
	T.chk("AUDIT chladnější lože (50 °C) teče pomaleji než při 77 °C", s3.Qmax < qh*0.85, "%.0f → %.0f L/min" % [qh, s3.Qmax])

# ---------- 5. čerpadla: křivka × odpor trasy ----------
func _pumps() -> void:
	T.fresh(); var S = sim.S
	_open("rvp", "sk", ["p1"])
	S.rvp.w = 2500.0; S.rvp.T = 60.0; S.rvp.stir = true
	S.p1.flow = 300.0; S.p1.on = true; S.p1.prime = 1.0
	sim.net_step(1.0)
	var q1: float = S.p1.q; var hz1: float = S.p1.get("hz", 0.0)
	T.chk("AUDIT C1 voda pánev→SK: měnič drží 300 L/min pod 50 Hz", q1 > 270.0 and hz1 > 10.0 and hz1 < 50.0, "%.0f L/min, %.0f Hz, %.1f bar" % [q1, hz1, S.p1.get("bar", 0.0)])
	S.p1.flow = 400.0; sim.net_step(1.0)
	T.chk("AUDIT C1 na 400 L/min potřebuje víc otáček", S.p1.get("hz", 0.0) > hz1, "%.0f Hz" % S.p1.get("hz", 0.0))
	# přiškrcení: na trase nastavíme klapce mezipolohu 15 %
	var vv = sim.route_valves("rvp", "sk", ["p1"])
	var thr = null
	for id in vv:
		var it = sim.net.byId.get(id)
		if it != null and it.type == "valve": thr = it; break
	if thr != null:
		thr["part"] = 15.0; S.p1.flow = 300.0; sim.net_step(1.0)
		T.chk("AUDIT přivřená klapka (15 %) sníží průtok, čerpadlo jede na plné otáčky", S.p1.q < q1*0.8, "%.0f L/min, %.0f Hz" % [S.p1.q, S.p1.get("hz", 0.0)])
		thr.erase("part")
	S.p1.on = false

# ---------- 6. protlačka: nic se nevyrobí z ničeho ----------
func _protlacka() -> void:
	T.fresh(); var S = sim.S
	var ids := {}; var ex0 := 0.0; var n := 0
	for e in sim.net.edges:
		if n >= 5: break
		var p = sim.lp(); p.w = e.cap*0.88; p.f = e.cap*0.12; p.T = 8.0
		e.c = p.duplicate(); e.q = [p]; ids[e.id] = true; ex0 += p.f; n += 1
	S["_castEdges"] = ids
	S.sp.L = 1000.0; S.sp.w = 880.0; S.sp.f = 120.0; S.sp.H = 8000.0
	var tot0: float = ex0 + S.sp.f + S.sp.n
	sim.push()
	var ex1 := 0.0
	for e in sim.net.edges: if ids.has(e.id): ex1 += sim.ext(e.c)
	T.chk("AUDIT protlačka přesune extrakt z potrubí do spilky (zachování hmoty)", absf(ex1 + S.sp.f + S.sp.n - tot0) < 1e-06 and ex1 < 1e-09, "%.3f kg v trubkách, spilka %.3f kg" % [ex1, S.sp.f + S.sp.n])

# ---------- 7. pH, kvašení, CIP ----------
func _process() -> void:
	T.fresh(); var S = sim.S; var P = sim.proc
	S.rvp.w = 2000.0; S.malt.added = 700.0
	S.water = P.WATERS[2].duplicate(); S.lactic = 0.0
	var ph_hard: float = P.mash_ph()
	S.water = P.WATERS[1].duplicate()
	var ph_soft: float = P.mash_ph()
	S.lactic = 1000.0
	var ph_acid: float = P.mash_ph()
	T.chk("AUDIT pH: tvrdá voda zvedá pH, měkká a kyselina mléčná snižují (Kolbach)", ph_hard > ph_soft and ph_acid < ph_soft and ph_hard > 5.8 and ph_soft < 5.8, "tvrdá %.2f, měkká %.2f, + kyselina %.2f" % [ph_hard, ph_soft, ph_acid])
	var f_bad: Vector2 = P.ph_factors(6.1); var f_ok: Vector2 = P.ph_factors(5.5)
	T.chk("AUDIT pH: amylázy jsou při pH 6,1 pomalejší než při 5,5", f_bad.x < f_ok.x and f_bad.y < f_ok.y, "β %.2f vs %.2f" % [f_bad.y, f_ok.y])
	# kvašení 12° mladiny při 9 °C
	T.fresh(); S = sim.S; P = sim.proc
	var e: float = 4000.0*1.048*0.12
	S.sp.L = 4000.0; S.sp.w = 4000.0*1.048 - e; S.sp.f = e*0.78; S.sp.n = e*0.22; S.sp.H = 9.0*4000.0
	S.fer.set = 9.0; P.fer_start()
	P.fer_skip(24.0*8.0)
	var ae: float = P.app_extract(); var abv: float = S.fer.E/10.0/0.79
	T.chk("AUDIT kvašení 12° při 9 °C za 8 dní: zdánlivý extrakt 2–4,5 °P, alkohol 4–5,5 % obj.", ae > 2.0 and ae < 4.5 and abv > 4.0 and abv < 5.5, "%.1f °P, %.1f %% obj., diacetyl %d ppb" % [ae, abv, roundi(S.fer.D)])
	# CIP
	T.fresh(); S = sim.S; P = sim.proc
	S.cip.target = "rvp"; S.cip.medium = "louh"; S.cip.set = 80.0; S.cip.T = 20.0; S.cip.run = true
	for i in 80: P.cip_step(60.0)
	T.chk("AUDIT CIP: louh 80 °C – ohřev stanice a 20 min nad 75 °C splní krok", S.cip.done.has("rvp:louh!"), "%.0f °C, %.0f min v pásmu" % [S.cip.T, S.cip.done.get("rvp:louh", 0.0)])
	S.cip.medium = "kyselina"; S.cip.set = 70.0; S.cip.T = 70.0; S.once.clear()
	P.cip_step(60.0)
	var warned := false
	for l in S.log: if "Kyselina nad 60" in l.msg or "kyselina nad 60" in l.msg: warned = true
	T.chk("AUDIT CIP: kyselina nad 60 °C vyvolá výstrahu a nepočítá se", warned and not S.cip.done.has("rvp:kyselina"), "")


# ---------- 8. regrese z provozu (hlášení Vasila 22. 10.) ----------
func _ui_regress() -> void:
	T.fresh(); var S = sim.S
	# přívod rmutu z C1 ústí do SK nad síto (ne do podsítí), ale pod hladinu
	var rt = sim.route_valves("rvp", "sk", ["p1"])
	var goal = sim.net.nodes.get(sim.last_goal) if sim.last_goal != null else null
	var under_ok := false; var low_ok := false
	if goal != null:
		for at in goal.att:
			if at.kind == "vessel" and at.bind == "sk": under_ok = not at.get("under", true);low_ok = at.low
	T.chk("PLÁN přívod rmutu (C1) do SK ústí nad síto, pod hladinu", rt != null and under_ok and low_ok, "uzel %s" % str(sim.last_goal))
	# staré uložení mělo u všech klapek "pct": 50 → klapky nesmí být přivřené na 50 %
	var kl = null
	for it in m.items:
		if it.type == "valve" and it.get("kind", "") != "skrt": kl = it; break
	kl["pct"] = 50
	m.save_game(); kl.erase("pct")
	var f := FileAccess.open(m.save_path, FileAccess.READ); var d = f.get_var(); f.close()
	d.items[kl.id]["pct"] = 50
	f = FileAccess.open(m.save_path, FileAccess.WRITE); f.store_var(d); f.close()
	m.load_game()
	var kl2 = null
	for it in m.items: if it.id == kl.id: kl2 = it
	T.chk("REGRESE staré uložení: klapka po načtení není přivřená na 50 %", kl2 != null and not kl2.has("pct") and not kl2.has("part"), str(kl2.get("pct", "-")) if kl2 != null else "?")
	# rozvaděč bez napětí: čerpadlo se nezapne a obsluha dostane důvod
	T.fresh(); S = sim.S
	S.cab.key = false; sim.sim_step(1.0)
	m.toggle_pump("p1")
	T.chk("REGRESE klíč VYP: C1 nejde pustit a hlášení řekne proč", not S.p1.on and S.log.size() > 0 and "klíč" in S.log[0].msg, S.log[0].msg if S.log.size() else "")
	S.cab.key = true; sim.sim_step(1.0)
	# nezavodněné čerpadlo se zaplavenou sací stranou ochrana pustí (zavodní se za chodu)
	S.rvp.w = 1500.0; S.p1.prime = 0.0
	_open("rvp", "sk", ["p1"])
	m.toggle_pump("p1")
	T.chk("REGRESE C1 se pustí i před plným zavodněním (sání zaplavené)", S.p1.on, S.log[0].msg if S.log.size() else "")
	for i in 30: sim.sim_step(1.0)
	T.chk("REGRESE C1 po pár sekundách čerpá", S.p1.q > 100.0, "%.0f L/min %s" % [S.p1.q, S.p1.status])
	S.p1.on = false


# ---------- 9. doplněné kontroly podle zadání (bilance, definice výtěžnosti, kalibrace, determinismus) ----------
func _water_total() -> float:
	var S = sim.S; var w := 0.0
	for k in ["rvp", "mp", "wp"]: w += S[k].w
	w += S.sk.mash.w
	for c in S.sk.cells: w += c.w
	w += S.sp.w + S.hlt.w + S.lv.w
	for e in sim.net.edges: w += e.c.w
	return w

func _ext_total() -> float:
	var x: Dictionary = sim.extract_inventory()
	return x.celkem

func _spec_tests() -> void:
	# A. izolovaný přenos pánev → SK čerpadlem C1: voda i extrakt se zachovají (žádný odtok, var ani odpar)
	T.fresh(); var S = sim.S
	S.rvp.w = 2000.0; S.rvp.f = 200.0; S.rvp.T = 50.0; S.rvp.Tsens = 50.0
	S.boiler.on = false; S.hlt.auto = false
	_open("rvp", "sk", ["p1"]); S.p1.on = true; S.p1.prime = 1.0; S.p1.flow = 200.0
	var w0: float = _water_total(); var e0: float = S.rvp.f + S.rvp.dx + S.rvp.ld + S.rvp.ns
	for i in 240: sim.sim_step(1.0)
	S.p1.on = false
	for i in 30: sim.sim_step(1.0)
	var w1: float = _water_total() + S.drainL + S.get("evapTot", 0.0)
	var ex: Dictionary = sim.extract_inventory()
	var e1: float = ex.rvp + ex.sk + ex.skmash + ex.trubky + ex.vyteklo
	T.chk("ZADÁNÍ bilance vody: přečerpání 4 min (voda v nádobách + potrubí + odtok + odpar) se zachová", absf(w1 - w0) < 0.5, "%.2f → %.2f kg, přečerpáno do SK %.0f L" % [w0, w1, sim.skVolTotal()])
	T.chk("ZADÁNÍ bilance extraktu: při přečerpání extrakt nevzniká ani nemizí", absf(e1 - e0) < 0.05, "%.3f → %.3f kg" % [e0, e1])
	# B. definice výtěžnosti: provozní výpočet z objemu horké vyrážky odpovídá extraktu v modelu
	T.fresh(); S = sim.S
	S.malt.added = 700.0; S.extIn = 538.0
	var P := 12.5; var rho := 1.0 + 0.004 * P; var e := 500.0
	var V20 := (e / (P / 100.0)) / rho
	S.flags.knock = {"V": V20 / 0.96, "P": P, "e": e, "t": 0.0}
	var y: Dictionary = sim.yields()
	T.chk("ZADÁNÍ výtěžnost varny = extrakt ve vyrážce / slad", absf(y.VV_varna - 500.0 / 700.0 * 100.0) < 0.01, "%.2f %%" % y.VV_varna)
	T.chk("ZADÁNÍ výtěžnost z objemu horké vyrážky (×0,96) souhlasí s extraktem", absf(y.VV_cz - y.VV_varna) < 0.1, "%.2f vs %.2f %%" % [y.VV_cz, y.VV_varna])
	T.chk("ZADÁNÍ využití lab. extraktu = extrakt ve vyrážce / potenciál sladu", absf(y.ucinnost - 500.0 / 538.0 * 100.0) < 0.01, "%.1f %%" % y.ucinnost)
	var rc: String = sim.recipe_check(sim.RECIPES[0])
	T.chk("ZADÁNÍ kontrola receptu spočte potřebný slad z cílového objemu a °P", "kg sladu" in rc, rc)
	# C. kalibrace ze souboru přepíše parametr a nic jiného
	var f := FileAccess.open(sim.CALIB_PATH, FileAccess.WRITE); f.store_string("{\"rvpUAst\": {\"value\": 7.25, \"source\": \"CALIBRATED\"}, \"neznamy\": 3}"); f.close()
	sim.new_state()
	T.chk("ZADÁNÍ kalibrace.json přepíše parametr (rvpUAst 7,25) a ignoruje neznámé klíče", absf(sim.S.cfg.rvpUAst - 7.25) < 1e-6 and not sim.S.cfg.has("neznamy"), str(sim.calib_applied))
	DirAccess.remove_absolute(ProjectSettings.globalize_path(sim.CALIB_PATH))
	sim.new_state()
	T.chk("ZADÁNÍ bez kalibrace platí výchozí hodnoty", absf(sim.S.cfg.rvpUAst - sim.CFG_DEFAULT.rvpUAst) < 1e-6)
	# D. determinismus: stejný scénář dvakrát → stejný výsledek
	var res := []
	for run in 2:
		T.fresh(); S = sim.S
		S.rvp.w = 1500.0; S.rvp.T = 40.0; S.rvp.Tsens = 40.0; S.rvp.stir = true; S.rvp.heat = 0.7; S.boiler.on = true
		for i in 600: sim.sim_step(1.0)
		res.append([S.rvp.T, S.rvp.w, S.hlt.T])
	T.chk("ZADÁNÍ determinismus: stejný ohřev 10 min dvakrát = stejné teploty i hmotnosti", res[0] == res[1], str(res))
	# E. ohřev odpovídá výkonu: dodaná pára = ohřev vody + oceli + ztráty (kontrola řádu jako v zadání)
	T.fresh(); S = sim.S
	S.rvp.w = 2000.0; S.rvp.T = 40.0; S.rvp.Tsens = 40.0; S.rvp.stir = true; S.rvp.heat = 0.6; S.boiler.on = true
	sim.sim_step(1.0)
	var T0: float = S.rvp.T
	for i in 60: sim.sim_step(1.0)
	var dTdt: float = (S.rvp.T - T0)
	var P_need: float = 2000.0 * 4.19 * dTdt / 60.0
	T.chk("ZADÁNÍ ohřev 2000 L: výkon na vodu z rychlosti ohřevu je v řádu stovek kW (zadání: 2,1 K/min ≈ 293 kW)", P_need > 150.0 and P_need < 500.0, "%.2f K/min → %.0f kW jen na vodu" % [dTdt, P_need])


# ---------- 10. tepelná kapacita: kapalina + ocel (c_p oceli 0,50 kJ/(kg·K)) ----------
func _steel_tests() -> void:
	T.fresh(); var S = sim.S; var c = S.cfg
	var v = S.rvp; v.w = 1000.0; v.T = 20.0
	T.chk("OCEL kapacita kapaliny: 1000 kg vody = 4190 kJ/K", absf(sim.cC(v) - 4190.0) < 0.01, "%.1f kJ/K" % sim.cC(v))
	T.chk("OCEL příspěvek kovu = m · c_p (450 kg × 0,50 = 225 kJ/K)", absf(sim.steelC(450.0) - 225.0) < 1e-6, "%.1f kJ/K" % sim.steelC(450.0))
	# stejný ohřev s 0 a 450 kg oceli: rozdíl efektivní kapacity musí být ≈ 225 kJ/K, ne 450
	var Ce := []
	for st in [0.0, 450.0]:
		T.fresh(); S = sim.S; c = S.cfg; c.rvpSteel = st; c.rvpUA = 0.0
		v = S.rvp; v.w = 1000.0; v.T = 40.0; v.Tsens = 40.0; v.stir = true; v.heat = 0.5; S.boiler.on = true
		var E := 0.0
		for i in 300:
			sim.sim_step(1.0); E += v.Pin
		Ce.append(E / (v.T - 40.0))
	T.chk("OCEL ohřev s 450 kg oceli: navíc ≈ 225 kJ/K (ne 450 – kov se nepočítá dvakrát ani s c_p 1,0)", absf((Ce[1] - Ce[0]) - 225.0) < 8.0, "0 kg: %.0f, 450 kg: %.0f kJ/K → rozdíl %.0f" % [Ce[0], Ce[1], Ce[1] - Ce[0]])
	T.chk("OCEL bez oceli a ztrát: E/ΔT = kapacita vody (1000 kg → 4190 kJ/K)", absf(Ce[0] - 4190.0) < 30.0, "%.0f kJ/K" % Ce[0])
	# směšování: přítok do nádoby s ocelí – energie (voda + ocel) se zachová
	T.fresh(); S = sim.S; c = S.cfg
	v = S.rvp; v.w = 1000.0; v.T = 20.0
	var p = sim.lp(); p.w = 500.0; p.T = 80.0
	var Eb: float = sim.cC(v) * 20.0 + sim.steelC(c.rvpSteel) * 20.0 + sim.cC(p) * 80.0
	sim.addTo(v, p, c.rvpSteel)
	var Ea: float = (sim.cC(v) + sim.steelC(c.rvpSteel)) * v.T
	T.chk("OCEL směšování přítoku: entalpie vody + oceli se zachová", absf(Ea - Eb) < 1.0, "%.1f vs %.1f kJ, T = %.2f °C" % [Eb, Ea, v.T])


# ---------- 11. kalibrace: rozsahy, uložená várka, hydraulika z konfigurace ----------
func _wcal(txt: String) -> void:
	var f := FileAccess.open(sim.CALIB_PATH, FileAccess.WRITE); f.store_string(txt); f.close()
func _delcal() -> void:
	DirAccess.remove_absolute(ProjectSettings.globalize_path(sim.CALIB_PATH))

func _c1_flow() -> float:
	T.fresh(); var S = sim.S
	S.rvp.w = 2000.0; S.rvp.T = 40.0
	_open("rvp", "sk", ["p1"]); S.p1.on = true; S.p1.prime = 1.0; S.p1.flow = 400.0
	for i in 5: sim.net_step(1.0)
	var q: float = S.p1.get("hz", 0.0); S.p1.on = false
	return q

func _calib_tests() -> void:
	_wcal('{"skArea": 0, "steamBar": -3, "rvpUAst": "x", "UA1": 30}')
	sim.new_state()
	T.chk("KALIBRACE nesmyslné hodnoty odmítnuty (skArea 0, steamBar −3, text), platná přijata (UA1 30)", absf(sim.S.cfg.skArea - 5.3) < 1e-6 and absf(sim.S.cfg.steamBar - 2.5) < 1e-6 and absf(sim.S.cfg.UA1 - 30.0) < 1e-6 and sim.calib_rejected.size() == 3, "; ".join(PackedStringArray(sim.calib_rejected)))
	_delcal(); sim.new_state()
	# kalibrace se uplatní i při načtení uložené várky
	T.fresh(); m.save_game()
	_wcal('{"rvpUAst": 8.0}')
	m.load_game()
	T.chk("KALIBRACE platí i pro načtenou uloženou várku", absf(sim.S.cfg.rvpUAst - 8.0) < 1e-6, "%.2f" % sim.S.cfg.rvpUAst)
	_delcal()
	# délka tras a křivka čerpadla z konfigurace mění pracovní bod
	var q0: float = _c1_flow()
	_wcal('{"pipeLenScale": 2.5}'); sim.new_state(); var q1: float = _c1_flow()
	_wcal('{"pipeLenScale": 3}'); var q3: float = _c1_flow()
	_wcal('{"pipeLenScale": 3, "p1_H0": 12}'); var q2: float = _c1_flow()
	_delcal()
	T.chk("KALIBRACE delší trasy (pipeLenScale 2,5) → měnič musí přidat otáčky pro stejných 400 L/min", q1 > q0 + 0.5, "%.1f → %.1f Hz" % [q0, q1])
	T.chk("KALIBRACE slabší čerpadlo (p1_H0 12 m) na odporové trase → vyšší otáčky pro stejný průtok", q2 > q3 + 0.5, "H0 32 m: %.1f Hz, H0 12 m: %.1f Hz" % [q3, q2])
	# viskozita: sladina 18 °P je hustší než voda
	var mu0: float = sim.skViscosity(75.0, 0.0); var mu18: float = sim.skViscosity(75.0, 18.0)
	T.chk("VISKOZITA sladina 18 °P / voda při 75 °C ≈ e^(0,047·18) = 2,3 (EBC kongresní sladina 8,6 % ≈ 1,4–1,7× vody)", absf(mu18 / mu0 - exp(0.047 * 18.0)) < 0.01, "%.2f×, μ(8,6 °P) = %.2f× vody" % [mu18 / mu0, sim.skViscosity(20.0, 8.6) / sim.skViscosity(20.0, 0.0)])
	# ledovka: teplota okolí ovlivňuje oteplování
	var dT := []
	for amb in [10.0, 30.0]:
		T.fresh(); var S = sim.S; S.cfg.ambientT = amb; S.lv.w = 8000.0; S.lv.T = 2.0; S.lv.chill = false
		for i in 3600: sim.sim_step(1.0)
		dT.append(S.lv.T - 2.0)
	T.chk("LEDOVKA ohřev od okolí závisí na ambientT (10 vs 30 °C)", dT[1] > dT[0] * 2.0, "+%.2f vs +%.2f K/h" % [dT[0], dT[1]])
	# mláto: nerozpustná sušina ze stejných dat jako extrakt
	T.fresh()
	var gi: float = sim.grain_dry_insoluble(700.0)
	T.chk("SLAD nerozpustná sušina + extrakt + voda = 700 kg sypání (stejné MALTS)", gi > 100.0 and gi < 160.0, "%.1f kg pluch" % gi)


func _ferm_cip_tests() -> void:
	T.fresh(); var S = sim.S
	S.sp.L = 4000.0; S.sp.w = 3500.0; S.sp.f = 400.0; S.sp.n = 80.0; S.sp.H = 4000.0 * 9.0
	sim.proc.fer_start()
	var f = S.fer; var S0: float = f.Sf
	for i in 96: sim.proc.fer_step(3600.0)
	var used: float = S0 - f.Sf
	var prod: float = f.E + f.get("CO2", 0.0) + f.get("XB", 0.0)
	T.chk("KVAŠENÍ bilance (Balling): spotřebovaný extrakt = etanol + CO₂ + biomasa", absf(prod - used) < 0.01 * maxf(1.0, used), "%.1f g/L → etanol %.1f + CO₂ %.1f + biomasa %.1f" % [used, f.E, f.get("CO2", 0.0), f.get("XB", 0.0)])
	T.fresh(); S = sim.S
	S.cip.target = "rvp"; S.cip.medium = "louh"; S.cip.set = 80.0; S.cip.T = 80.0; S.cip.conc = 0.5; S.cip.run = true
	for i in 30: sim.proc.cip_step(60.0)
	T.chk("CIP slabý louh (0,5 % NaOH) se nezapočítá ani při 80 °C 30 min", not S.cip.done.has("rvp:louh!"))
	S.cip.conc = 1.5; S.cip.done = {}
	for i in 25: sim.proc.cip_step(60.0)
	T.chk("CIP louh 1,5 % při 80 °C 25 min = splněno", S.cip.done.has("rvp:louh!"))


func _practice_tests() -> void:
	T.fresh(); var S = sim.S
	var pr = m.practice
	if pr == null:
		T.chk("ZÁSADY modul načten", false);return
	S.p1.prime = 0.0; S.p1.flow = 350.0
	pr.on_pump_start("p1")
	T.chk("ZÁSADY nezavodněné čerpadlo + rozjezd naplno = 2 porušení", int(S.practice.n) == 2, str(S.practice.rules))
	S.rvp.w = 1500.0; S.rvp.T = 40.0; S.rvp.heat = 0.0; pr.tick(1.0); S.rvp.heat = 1.0; S.rvp.stir = true; pr.tick(1.0)
	T.chk("ZÁSADY pára otevřená naráz je porušení", S.practice.rules.has("para_naraz"))
	S.rvp.g = 100.0; S.rvp.stir = false
	for i in 35: pr.tick(1.0)
	T.chk("ZÁSADY topení bez míchadla (rmut) je porušení", S.practice.rules.has("micha_para"))
	var n0: int = int(S.practice.n)
	for i in 120: pr.tick(1.0)
	T.chk("ZÁSADY stejné upozornění se neopakuje každou sekundu", int(S.practice.n) <= n0 + 1, "%d → %d" % [n0, int(S.practice.n)])
	S.p3.on = true; pr.on_pump_stop("plv")
	T.chk("ZÁSADY zastavení ledové vody před C3 je porušení", S.practice.rules.has("chlazeni_konec"))
	var card: Array = sim.proc.scorecard() if sim.get("proc") != null else []
	var found := false
	for r in card: if str(r[0]).begins_with("Provozní zásady"): found = true
	T.chk("ZÁSADY jsou v hodnocení várky", found or card.is_empty())

## fyzika podle literatury (rešerše 10/2026)
func _physics_lit_tests() -> void:
	T.fresh(); var S = sim.S
	# SMM: poločas při 100 °C a pH 5,2 ≈ 38 min (Dickenson 1979)
	var v = sim.lp(); v.w = 1000.0; v.T = 100.0; v.smm = 1.0; S.pH = 5.2; v["boiling"] = false; v["evapKgMin"] = 0.0
	for i in 38: sim.react(v, 1.0)
	T.chk("LIT SMM poločas při 100 °C, pH 5,2 ≈ 38 min (Dickenson)", absf(v.smm + v.dms - 1.0) < 0.6 and absf(v.smm - 0.5) < 0.05, "%.3f" % v.smm)
	v = sim.lp(); v.w = 1000.0; v.T = 94.0; v.smm = 1.0; v["boiling"] = false; v["evapKgMin"] = 0.0
	for i in 76: sim.react(v, 1.0)
	T.chk("LIT SMM při 94 °C: poločas 2× delší (každých −6 °C)", absf(v.smm - 0.5) < 0.05, "%.3f" % v.smm)
	# amylázy: α poločas 15 min při 72 °C, β labilní < 1 min při 72 °C (Marc 1983)
	v = sim.lp(); v.w = 1000.0; v.T = 72.0; v.aA = 1.0; v.aB = 0.87; v.aBs = 0.13; v["boiling"] = false; v["evapKgMin"] = 0.0
	for i in 15: sim.react(v, 1.0)
	T.chk("LIT α-amyláza při 72 °C: poločas ≈ 15 min (Marc)", absf(v.aA - 0.5) < 0.06, "%.3f" % v.aA)
	T.chk("LIT β-amyláza při 72 °C: labilní podíl zmizí, stabilní zůstane (De Schepper)", v.aB < 0.001 and v.aBs > 0.08, "aB %.4f aBs %.3f" % [v.aB, v.aBs])
	# kotel: velký odběr → tlak páry klesá, po odlehčení se vrátí
	T.fresh(); S = sim.S; S.cfg.boilerKW = 200.0
	S.rvp.w = 4000.0; S.rvp.T = 60.0; S.rvp.heat = 1.0; S.rvp.stir = true; S.hlt.heat = 1.0; S.hlt.auto = false; S.hlt.T = 40.0
	for i in 600: sim.sim_step(1.0)
	var p_low: float = S.boiler.p
	S.rvp.heat = 0.0; S.hlt.heat = 0.0
	for i in 600: sim.sim_step(1.0)
	T.chk("LIT kotel: při odběru nad výkon tlak páry klesá a po odlehčení se vrátí", p_low < S.cfg.steamBar - 0.3 and S.boiler.p > p_low + 0.2, "%.2f → %.2f bar" % [p_low, S.boiler.p])


# ---------- 12. vodovodní řad = zdroj tlaku (ne pevný limit průtoku) ----------
func _mains_outs(src) -> Array:
	var r = sim.bfs(sim.starts_of(src.id))
	var outs := []
	for d in r.hits:
		if d.att.bind != "vodovod": outs.append({"d": d, "path": sim.path_to(r, d.k)})
	for o in outs: o["f"] = sim.path_pct(o.path)
	return outs

func _mains_tests() -> void:
	T.fresh(); var S = sim.S; var c = S.cfg
	var src = null
	for it in sim.items_by_bind("vodovod"):
		if it.get("shape") == "inlet": src = it
	if src == null:
		T.chk("AUDIT řad: zdroj vody z řadu existuje", false); return
	_open("vodovod", "lv", [])
	src["on"] = true
	var outs: Array = _mains_outs(src)
	var iLv := -1
	for i in outs.size():
		if outs[i].d.att.bind == "lv": iLv = i
	if iLv < 0 or outs[iLv].f <= 0.0:
		T.chk("AUDIT řad: cesta řad → ledovka je otevřená", false); return
	var Q1: float = sim.mains_solve(outs, src)[iLv]
	var Qmax: float = c.mainsQ; var Hm: float = c.mainsBar*1.0e5/(1000.0*9.81)
	# 1) rovnováha tlaků v uzlu: H zdroje při průtoku ΣQ = ztráta trasy + výška
	var mu: float = sim.skViscosity(S.cool.waterT, 0.0)
	var Qt := 0.0
	for q in sim.mains_solve(outs, src): Qt += q
	var Hs: float = Hm*(1.0 - pow(Qt/Qmax, 2))
	var path = outs[iLv].path
	var last = path[path.size() - 1]
	var ysrc: float = sim.net.nodes[sim.starts_of(src.id)[0]].y
	var hst: float = (ysrc - sim.net.nodes[last.to].y)/100.0
	var Hn: float = sim.path_head(path, Q1, 1.0, mu) + hst
	T.chk("AUDIT řad: tlak zdroje při odběru = ztráta trasy + výška (bilance tlaku v uzlu)", absf(Hs - Hn) < 0.02*maxf(1.0, Hs), "zdroj %.2f m, trasa %.2f m, Q %.0f L/min" % [Hs, Hn, Q1])
	T.chk("AUDIT řad: průtok je kladný a nepřekročí kapacitu přívodu", Q1 > 20.0 and Qt <= Qmax*1.001, "%.0f L/min z max %.0f" % [Q1, Qmax])
	# 2) vyšší tlak řadu → víc vody, ale podlinárně (turbulence: Q ~ √ΔH)
	c.mainsBar = 8.0
	var Q2: float = sim.mains_solve(outs, src)[iLv]
	c.mainsBar = 4.0
	T.chk("AUDIT řad: dvojnásobný tlak dá víc vody, ale méně než dvojnásobek", Q2 > Q1*1.05 and Q2 < Q1*1.99, "%.0f → %.0f L/min" % [Q1, Q2])
	# 3) přivřená klapka na trase = větší ztráta, menší průtok; plně zavřená = nula
	var vv = sim.route_valves("vodovod", "lv", [])
	var thr = null
	for id in vv:
		var it2 = sim.net.byId.get(id)
		if it2 != null and it2.type == "valve": thr = it2; break
	if thr != null:
		thr["part"] = 20.0
		var outs2: Array = _mains_outs(src)
		var Q3: float = 0.0
		for i in outs2.size(): if outs2[i].d.att.bind == "lv": Q3 = sim.mains_solve(outs2, src)[i]
		thr.erase("part")
		T.chk("AUDIT řad: přivřená klapka (20 %) sníží průtok, ale voda dál teče", Q3 < Q1*0.9 and Q3 > 0.0, "%.0f → %.0f L/min" % [Q1, Q3])
	# 4) nezávislost na pořadí/cachování: opakované řešení dává stejný výsledek
	var Q4: float = sim.mains_solve(outs, src)[iLv]
	T.chk("AUDIT řad: opakované řešení je deterministické", absf(Q4 - Q1) < 1e-9)
	# 5) zachování hmoty při plnění: přírůstek vody v ledovce nesmí překročit průtok × čas a musí se jím blížit
	S.lv.w = 1000.0
	var w0: float = S.lv.w
	for i in 30: sim.net_step(1.0)
	var dw: float = S.lv.w - w0
	var exp_: float = Q1/60.0*30.0
	T.chk("AUDIT řad: do ledovky za 30 s přiteče nejvýš průtok × čas a řádově tolik", dw > 0.6*exp_ and dw < 1.02*exp_, "%.0f L, řešení tlaku dává %.0f L" % [dw, exp_])
	src["on"] = false


# ---------- 13. referenční testy s NEZÁVISLÝM výpočtem (uzavřené vzorce z učebnic, ne kopie kódu) ----------
func _rel(a: float, b: float) -> float:
	return absf(a - b)/maxf(1e-12, absf(b))

func _mkv(Tc: float) -> Dictionary:
	var v = sim.vessel(); v.w = 1000.0; v.T = Tc; v.Tsens = Tc
	return v

func _reference_tests() -> void:
	T.fresh(); var S = sim.S; var c = S.cfg
	# --- A) přesné reakce 1. řádu: nezávislost na kroku a shoda s analytickým e^(-kt) ---
	var f5: float = sim.first_order(5.0, 1.0)
	T.chk("REF reakce 1. řádu: k = 5 /min, 1 min → 1 − e^−5 (dřív min(1, k·m) = 100 %)", absf(f5 - (1.0 - exp(-5.0))) < 1e-12 and f5 < 1.0, "%.5f" % f5)
	var rest := 1.0
	for i in 600: rest *= 1.0 - sim.first_order(0.3, 1.0/60.0)
	T.chk("REF reakce 1. řádu: 600 krátkých kroků = jeden dlouhý (přesné řešení)", absf((1.0 - rest) - sim.first_order(0.3, 10.0)) < 1e-9, "%.9f" % (1.0 - rest))
	# --- B) amylasa α při 63 °C: A(t) = A0·e^(−0,022·t) (Marc 1983, k(63 °C) z cfg) ---
	var va = _mkv(63.0); va.aA = 10.0
	var vb = _mkv(63.0); vb.aA = 10.0
	sim.react(va, 30.0)
	for i in 1800: sim.react(vb, 1.0/60.0)
	var Aref: float = 10.0*exp(-c.kdAlpha63*30.0)
	T.chk("REF α-amylasa 63 °C, 30 min: A = A0·e^(−k·t)", _rel(va.aA, Aref) < 1e-6, "%.4f vs %.4f" % [va.aA, Aref])
	T.chk("REF α-amylasa: krok 30 min a 1 s dávají totéž", _rel(va.aA, vb.aA) < 1e-6, "%.5f / %.5f" % [va.aA, vb.aA])
	# --- C) SMM → DMS při 100 °C, pH 5,2: SMM(t) = SMM0·e^(−0,01824·t) (Dickenson 1979) ---
	S["pH"] = 5.2
	var vs1 = _mkv(100.0); vs1.smm = 10.0
	var vs2 = _mkv(100.0); vs2.smm = 10.0
	sim.react(vs1, 60.0)
	for i in 3600: sim.react(vs2, 1.0/60.0)
	var Sref: float = 10.0*exp(-0.01824*60.0)
	T.chk("REF SMM při 100 °C, pH 5,2, 60 min: e^(−0,01824·t)", _rel(vs1.smm, Sref) < 2e-3, "%.4f vs %.4f" % [vs1.smm, Sref])
	T.chk("REF SMM: krok 60 min a 1 s dávají totéž", _rel(vs1.smm, vs2.smm) < 1e-6 and _rel(vs1.dms, vs2.dms) < 1e-6, "DMS %.5f / %.5f" % [vs1.dms, vs2.dms])
	T.chk("REF SMM → DMS: hmota se zachovává (SMM + DMS ≤ počáteční, zbytek se odpaří)", vs1.smm + vs1.dms <= 10.0 + 1e-9)
	# --- D) izomerizace α-kyselin (Malowicki & Shellhammer 2005): α(t) = α0·e^(−k1·t), iso = k1·α0·(e^(−k1 t) − e^(−k2 t))/(k2 − k1) ---
	var vh1 = _mkv(100.0); vh1.alpha = 100.0
	var vh2 = _mkv(100.0); vh2.alpha = 100.0
	sim.react(vh1, 60.0)
	for i in 3600: sim.react(vh2, 1.0/60.0)
	var TK: float = 373.15
	var k1: float = 7.9e11*exp(-11858.0/TK); var k2: float = 4.1e12*exp(-12994.0/TK)
	var isoRef: float = k1*100.0*(exp(-k1*60.0) - exp(-k2*60.0))/(k2 - k1)
	T.chk("REF chmel 100 °C, 60 min: izo-α podle analytického řešení dvou následných reakcí", _rel(vh1.iso, isoRef) < 1e-6, "%.3f vs %.3f" % [vh1.iso, isoRef])
	T.chk("REF chmel: α klesá jako e^(−k1·t) a krok nehraje roli", _rel(vh1.alpha, 100.0*exp(-k1*60.0)) < 1e-6 and _rel(vh1.iso, vh2.iso) < 1e-5, "iso %.4f / %.4f" % [vh1.iso, vh2.iso])
	T.chk("REF chmel: využití izomerizací za 60 min varu je v rozumných mezích 20–65 %", vh1.iso > 20.0 and vh1.iso < 65.0, "%.1f %% α → iso" % vh1.iso)
	# --- E) odpar a zahuštění: extrakt se při varu zachová, °Plato = 100·E/(E+W) ---
	T.fresh(); S = sim.S; c = S.cfg
	var v = S.rvp; v.w = 4000.0; v.f = 450.0; v.T = c.tbWater; v.Tsens = c.tbWater; v.stir = true; v.heat = 0.85
	var e0: float = sim.ext(v); var w0: float = v.w; var dr0: float = S.drainExt; var dl0: float = S.drainL
	for i in 900: sim.sim_step(1.0)
	var e1: float = sim.ext(v) + (S.drainExt - dr0)
	var P1: float = 100.0*sim.ext(v)/(sim.ext(v) + v.w)
	T.chk("REF var: extrakt (v nádobě + odtečený) se při odparu zachová", _rel(e1, e0) < 0.005, "%.2f → %.2f kg" % [e0, e1])
	T.chk("REF var: vody ubývá (odpar) a °P = 100·E/(E+W) roste", v.w < w0 - 20.0 and P1 > 100.0*e0/(e0 + w0) + 0.1, "voda %.0f → %.0f kg, %.2f °P" % [w0, v.w, P1])
	T.chk("REF var: plato() v modelu odpovídá 100·E/(E+W) (±0,3 °P)", absf(sim.plato(v) - P1) < 0.3, "%.2f vs %.2f" % [sim.plato(v), P1])
	# --- F) výtěžnost: VV (kg extraktu/100 kg sladu) = účinnost × laboratorní extrakt/hmotnost sladu ---
	T.fresh(); S = sim.S; c = S.cfg
	var mm := 1000.0; S.malt.added = mm
	var elab: float = mm*(1.0 - c.maltMoist/100.0)*c.maltExtract/100.0
	S.extIn = elab
	S.flags["knock"] = {"P": 12.0, "e": 0.807*elab, "V": 71.0}
	var Y: Dictionary = sim.yields()
	T.chk("REF výtěžnost: laboratorní extrakt = slad·(1−vlhkost)·extrakt sušiny", _rel(Y.e_lab, elab) < 1e-9, "%.1f kg" % Y.e_lab)
	T.chk("REF výtěžnost: VV_varna = účinnost × e_lab/m_slad (80,7 % účinnosti ≈ 61,7 kg/100 kg)", absf(Y.VV_varna - Y.ucinnost/100.0*Y.e_lab/Y.m_slad*100.0) < 1e-9 and absf(Y.VV_varna - 61.65) < 0.2, "VV %.2f, účinnost %.1f %%" % [Y.VV_varna, Y.ucinnost])
	# --- G) čerpadlo: pracovní bod = průsečík křivky čerpadla a charakteristiky soustavy ---
	var H0: float = c.get("p1_H0", 32.0); var Qn: float = c.get("p1_Qn", 400.0); var A: float = 0.25*H0/(Qn*Qn)
	var pp: Dictionary = sim.pump_point("p1", 5000.0, 5.0, [], [], 1.0, 0.001, 0.0)
	var Qref: float = sqrt((H0 - 5.0)/A)
	T.chk("REF čerpadlo: konstantní statická výška 5 m → Q = √((H0−Hst)/A) (průsečík s parabolou)", _rel(pp.Q, Qref) < 1e-3, "%.1f vs %.1f L/min" % [pp.Q, Qref])
	pp = sim.pump_point("p1", 300.0, 5.0, [], [], 1.0, 0.001, 0.0)
	var hzRef: float = 50.0*sqrt((5.0 + A*300.0*300.0)/H0)
	T.chk("REF čerpadlo: měnič drží 300 L/min při frekvenci dané afinitním zákonem H ~ n²", _rel(pp.Q, 300.0) < 1e-6 and _rel(pp.hz, hzRef) < 1e-3, "%.2f Hz vs %.2f Hz" % [pp.hz, hzRef])
	pp = sim.pump_point("p1", 300.0, H0 + 5.0, [], [], 1.0, 0.001, 0.0)
	T.chk("REF čerpadlo: výška nad uzavřenou výškou H0 → žádný průtok", pp.Q == 0.0, "%.1f L/min" % pp.Q)
	# --- H) třecí ztráta v potrubí: laminárně Hagen–Poiseuille, turbulentně Colebrook ---
	var D: float = sim.bore_mm(25.0)/1000.0; var Lm: float = 1000.0/100.0*c.pipeLenScale
	var path := [{"e": {"dn": 25.0, "len": 1000.0}}]
	var Ql: float = 5.0; var vl: float = Ql/60000.0/(PI*D*D/4.0)
	var Hlam: float = 128.0*0.001*Lm*(Ql/60000.0)/(PI*pow(D, 4.0)*1000.0*9.81) + 0.4*vl*vl/(2.0*9.81)
	var Hm1: float = sim.path_head(path, Ql, 1.0, 0.001)
	T.chk("REF potrubí: laminární proudění odpovídá Hagen–Poiseuille (+ místní ztráta)", _rel(Hm1, Hlam) < 0.02, "%.5f vs %.5f m" % [Hm1, Hlam])
	var Qt2: float = 500.0; var vt: float = Qt2/60000.0/(PI*D*D/4.0); var Ret: float = 1000.0*vt*D/0.001
	var fc: float = 0.02
	for i in 60: fc = pow(1.0/(-2.0*log(1.5e-05/(3.7*D) + 2.51/(Ret*sqrt(fc)))*0.4342944819), 2.0)
	var Hturb: float = (fc*Lm/D + 0.4)*vt*vt/(2.0*9.81)
	var Hm2: float = sim.path_head(path, Qt2, 1.0, 0.001)
	T.chk("REF potrubí: turbulence – Swamee–Jain v modelu souhlasí s Colebrookem do 2 %", _rel(Hm2, Hturb) < 0.02, "%.3f vs %.3f m, f=%.4f" % [Hm2, Hturb, fc])
	T.chk("REF potrubí: ztráta roste s průtokem (≈ Q²) a se zavřením klapky", sim.path_head(path, 2.0*Qt2, 1.0, 0.001) > 3.3*Hm2, "")
	# --- I) výměník: ε-NTU protiproud, nezávislý uzavřený vzorec ---
	var Pm = preload("res://physics.gd")
	var cases := [[2.0, 1.0, 0.5], [2.0, 4.0, 1.0], [1.0, 3.0, 3.0], [0.5, 2.0, 2.0]]
	var okE := true; var det := ""
	for cs in cases:
		var UA: float = cs[0]; var Ch: float = cs[1]; var Cc: float = cs[2]
		var cmin: float = minf(Ch, Cc); var cmax: float = maxf(Ch, Cc); var cr: float = cmin/cmax; var ntu: float = UA/cmin
		var eref: float = ntu/(1.0 + ntu) if absf(1.0 - cr) < 1e-9 else (1.0 - exp(-ntu*(1.0 - cr)))/(1.0 - cr*exp(-ntu*(1.0 - cr)))
		var em: float = Pm.exchanger_effectiveness(UA, Ch, Cc)
		if absf(em - eref) > 1e-9: okE = false
		det += "%.3f " % em
	T.chk("REF výměník: ε-NTU protiproud souhlasí s uzavřeným vzorcem (Cr<1 i Cr=1)", okE, det)
	var emax: float = Pm.exchanger_effectiveness(1000.0, 1.0, 2.0)
	T.chk("REF výměník: nekonečně velká plocha → ε → 1, nulová plocha → 0", emax > 0.999 and Pm.exchanger_effectiveness(0.0, 1.0, 2.0) == 0.0, "%.5f" % emax)
	# --- J) tepelná kapacita: ohřev bez ztrát stoupá lineárně – T = T0 + P·t/(m·c) ---
	T.fresh(); S = sim.S; c = S.cfg
	var vh = S.rvp; vh.w = 2000.0; vh.T = 20.0; vh.Tsens = 20.0; vh.stir = true; vh.heat = 0.5
	var Cth: float = sim.cC(vh) + sim.steelC(c.rvpSteel); var Ein := 0.0; var Lo := 0.0; var T0: float = vh.T
	for i in 300:
		sim.sim_step(1.0); Ein += vh.Pin; Lo += c.rvpUA*(vh.T - c.ambientT)
	var Tref: float = T0 + (Ein - Lo)/Cth
	T.chk("REF ohřev: T = T0 + (E_dodaná − E_ztracená)/C (C = voda + ocel, kJ/K)", absf(vh.T - Tref) < 0.6, "%.2f vs %.2f °C" % [vh.T, Tref])


# ---------- 14. vířivka: usazení kalů závisí na procesu (době víření a době klidu), ne na pevném časovači ----------
func _settle(whirl: float, minutes: float) -> float:
	T.fresh(); var S = sim.S
	var v = S.wp; v.w = 3800.0; v.T = 85.0; v.Tsens = 85.0
	v.trubSus = 10.0; v.trubHB = 10.0; v.whirlMin = whirl; v.whirlNow = false; v.boiling = false
	for i in int(minutes*60.0): sim.sim_step(1.0)
	return v.trubSet

func _whirl_tests() -> void:
	var c = sim.S.cfg
	var s0: float = _settle(0.0, 10.0)
	var s1: float = _settle(1.0, 10.0)
	var s3: float = _settle(3.0, 10.0)
	var s8: float = _settle(8.0, 10.0)
	T.chk("REF vířivka: čím déle se vířilo, tím víc kalu se za stejný klid usadí (plynule, bez prahu)", s0 < s1 and s1 < s3 and s3 < s8, "%.2f / %.2f / %.2f / %.2f kg" % [s0, s1, s3, s8])
	var kst: float = c.trubKstill; var kw: float = c.trubKwhirl
	var wi: float = 1.0 - exp(-8.0/c.whirlTau); var k8: float = kst + (kw - kst)*wi
	var ref8: float = 9.0*(1.0 - exp(-k8*10.0))
	T.chk("REF vířivka: usazená hmota = (kal − nesedimentující 10 %)·(1 − e^(−k·t))", absf(s8 - ref8) < 0.03*ref8 + 0.02, "%.3f vs %.3f kg" % [s8, ref8])
	var sl: float = _settle(8.0, 40.0)
	T.chk("REF vířivka: po dlouhém klidu se usadí všechen kal kromě podílu, který zůstane v suspenzi (10 %)", sl > 8.5 and sl <= 9.0 + 1e-6, "%.2f z 9,0 kg" % sl)


# ---------- 15. kontrola parametrů a nejrizikovějších výpočtů (25. 10.): každá oprava má svůj regresní test ----------
func _vsum(v) -> float: return v.sg + v.dx + v.ld + v.f

func _mash(Tc: float, minutes: float, dt: float, betaAct := 4.5, kdB := 0.13) -> Dictionary:
	T.fresh(); var S = sim.S
	S.cfg.betaAct = betaAct; S.cfg.kdBeta63 = kdB
	var v = sim.vessel(); v.w = 2000.0; v.T = Tc; v.Tsens = Tc
	v.aA = 560.0; v.aB = 560.0*0.87; v.aBs = 560.0*0.13; v.sg = 300.0
	var s0: float = _vsum(v)
	var n: int = int(minutes/dt + 0.5)
	for i in n: sim.react(v, dt)
	return {"f": v.f, "dx": v.dx, "sum0": s0, "sum": _vsum(v), "aB": v.aB, "frac": v.f/maxf(1e-9, _vsum(v))}

func _fer_run(mods: Dictionary, hours: int) -> Dictionary:
	T.fresh(); var S = sim.S; var P = sim.proc
	for k in mods: S.cfg[k] = mods[k]
	var e: float = 4000.0*1.048*0.12
	S.sp.L = 4000.0; S.sp.w = 4000.0*1.048 - e; S.sp.f = e*0.78; S.sp.n = e*0.22; S.sp.H = 9.0*4000.0
	S.fer.set = 9.0; P.fer_start()
	var Tmax: float = S.fer.T
	for i in hours*60:
		P.fer_step(60.0); Tmax = maxf(Tmax, S.fer.T)
	return {"Sf": S.fer.Sf, "S0": S.fer.S0, "E": S.fer.E, "Tmax": Tmax, "X": S.fer.X, "ok": S.fer.Sf == S.fer.Sf and S.fer.T == S.fer.T}

func _fin(x: float) -> bool: return x == x and absf(x) < 1e9

func _regrese_zadani() -> void:
	var Pm = preload("res://physics.gd")
	_delcal(); T.fresh(); var S = sim.S; var c = S.cfg

	# --- registr parametrů: každý klíč má rozsah, výchozí hodnota v rozsahu ---
	var bad := []
	for k in sim.CFG_DEFAULT:
		if not sim.CALIB_RANGE.has(k): bad.append(k + " (bez rozsahu)"); continue
		var r = sim.CALIB_RANGE[k]; var d: float = float(sim.CFG_DEFAULT[k])
		if d < r[0] or d > r[1]: bad.append("%s=%s mimo %s…%s" % [k, str(d), str(r[0]), str(r[1])])
	T.chk("REGRESE registr: všech %d parametrů má rozsah a výchozí hodnotu v něm" % sim.CFG_DEFAULT.size(), bad.is_empty(), ", ".join(PackedStringArray(bad)))

	# --- kotel: počáteční tlak = steamBar (dřív natvrdo 2,5), tabulka sytosti pokrývá 8 bar ---
	_wcal('{"steamBar": 4.0}'); sim.new_state()
	T.chk("REGRESE kotel: počáteční tlak páry = kalibrované steamBar (4 bar), ne natvrdo 2,5", absf(sim.S.boiler.p - 4.0) < 1e-9, "%.2f bar" % sim.S.boiler.p)
	_delcal(); sim.new_state()
	T.chk("REGRESE tabulka sytosti: 8 bar přetlaku (9,013 bar abs.) ≈ 175,4 °C a nad tabulkou teplota neklesá", absf(sim.tsat(9.013) - 175.4) < 1.0 and sim.tsat(12.0) >= sim.tsat(10.0), "%.1f °C, 12 bar abs. %.1f °C" % [sim.tsat(9.013), sim.tsat(12.0)])

	# --- film u husté náplně: voda = beze změny, hustý rmut zhorší přestup; celkové UA zůstane referenční ---
	T.fresh(); S = sim.S; c = S.cfg
	var w = sim.vessel(); w.w = 2000.0; w.stir = true; w.hz = 35.0
	var thick = sim.vessel(); thick.w = 1900.0; thick.g = 400.0; thick.sg = 300.0; thick.stir = true; thick.hz = 35.0
	var ff_w: float = sim.film_factor(w); var ff_t: float = sim.film_factor(thick)
	c.rvpUAst = 30.0
	var ff_w30: float = sim.film_factor(w)
	T.chk("REGRESE přestup tepla: voda při 35 Hz = referenční UA (faktor 1) i při kalibrovaném rvpUAst = 30 kW/K", absf(ff_w - 1.0) < 1e-9 and absf(ff_w30 - 1.0) < 1e-9, "%.6f / %.6f" % [ff_w, ff_w30])
	T.chk("REGRESE přestup tepla: hustý rmut předá jen 25–70 % výkonu vody (viskózní film)", ff_t > 0.25 and ff_t < 0.7, "%.2f" % ff_t)

	# --- bilance páry: výkon, který kotel vyrábí, = výkon, který nádoba skutečně přijala (dřív kotel počítal bez filmu) ---
	T.fresh(); S = sim.S; c = S.cfg
	var v = S.rvp; v.w = 1900.0; v.g = 400.0; v.sg = 300.0; v.T = 40.0; v.Tsens = 40.0; v.stir = true; v.heat = 0.1
	var Eb := 0.0; var Ed := 0.0
	for i in 300:
		sim.sim_step(1.0)
		Eb += S.boiler.load * c.boilerKW; Ed += S.rvp.Pin + S.mp.Pin
	T.chk("REGRESE kotel: vyrobená energie páry = energie přijatá hustým rmutem (chyba < 1 %)", Ed > 1000.0 and absf(Eb - Ed)/Ed < 0.01, "kotel %.0f MJ, nádoba %.0f MJ" % [Eb/1000.0, Ed/1000.0])
	T.chk("REGRESE kotel: při odběru pod výkonem drží tlak na steamBar", absf(S.boiler.p - c.steamBar) < 0.02, "%.2f bar" % S.boiler.p)
	# přetížení: obě pánve naplno → tlak klesá a rozdíl výroby a odběru je uložen v bubnu (Cst = boilerWaterKg·57 kJ/bar)
	T.fresh(); S = sim.S; c = S.cfg
	S.rvp.w = 2000.0; S.rvp.T = 20.0; S.rvp.Tsens = 20.0; S.rvp.stir = true; S.rvp.heat = 1.0
	S.mp.w = 2000.0; S.mp.T = 20.0; S.mp.Tsens = 20.0; S.mp.stir = true; S.mp.heat = 1.0
	var p0: float = S.boiler.p; var Eb2 := 0.0; var Ed2 := 0.0
	for i in 120:
		sim.sim_step(1.0)
		Eb2 += S.boiler.load * c.boilerKW; Ed2 += S.rvp.Pin + S.mp.Pin
	var Est: float = (S.boiler.p - p0) * c.boilerWaterKg * 57.0
	T.chk("REGRESE kotel přetížen: tlak klesne a Σvýroba − Σodběr = změna energie bubnu", S.boiler.p < p0 - 0.05 and absf((Eb2 - Ed2) - Est) < 0.03*Eb2, "p %.2f→%.2f bar, výroba %.0f MJ, odběr %.0f MJ, buben %.0f MJ" % [p0, S.boiler.p, Eb2/1000.0, Ed2/1000.0, Est/1000.0])

	# --- zásoba v potrubí: STEJNÁ geometrie jako hydraulika (π/4·d²·L·pipeLenScale) ---
	T.fresh(); S = sim.S; c = S.cfg
	var nchk := 0; var maxrel := 0.0
	for e in sim.net.edges:
		if float(e.cap) <= 0.31 or e.ids.size() != 1: continue
		var exp_cap: float = Pm.pipe_capacity(float(e.len)/sim.PX_PER_M*float(c.pipeLenScale), sim.bore_mm(float(e.dn)))
		maxrel = maxf(maxrel, absf(float(e.cap) - exp_cap)/maxf(1e-9, exp_cap)); nchk += 1
	T.chk("REGRESE potrubí: zásoba kapaliny v úsecích = π/4·d²·L·pipeLenScale (stejné bore a délka jako tlakové ztráty)", nchk > 5 and maxrel < 1e-9, "%d úseků, max. odchylka %.2g" % [nchk, maxrel])
	var vel: float = 400.0/60000.0 / (PI*pow(sim.bore_mm(50.0)/1000.0, 2.0)/4.0)
	T.chk("REGRESE potrubí: 400 L/min v DN50 má rychlost 0,5–3 m/s (kontrola vnitřního průměru bore50)", vel > 0.5 and vel < 3.0, "%.2f m/s při průměru %.0f mm" % [vel, sim.bore_mm(50.0)])

	# --- ledovka: ztráty do okolí z lvUA, lože z skBedLkg ---
	var dTs := []
	for ua in [0.0, 0.15]:
		T.fresh(); S = sim.S; S.cfg.ambientT = 30.0; S.cfg.lvUA = ua; S.lv.w = 8000.0; S.lv.T = 2.0; S.lv.chill = false; S.lv.pump = false
		for i in 3600: sim.sim_step(1.0)
		dTs.append(S.lv.T - 2.0)
	T.chk("REGRESE ledovka: lvUA = 0 → žádný přísun tepla z okolí, lvUA 0,15 kW/K → ohřev (dřív pevných 0,15 mimo kalibraci)", absf(dTs[0]) < 0.01 and dTs[1] > 0.05, "%.3f vs %.3f K/h" % [dTs[0], dTs[1]])
	T.fresh(); S = sim.S; S.sk.g = 100.0
	var h1: float = sim.skBedH(); S.cfg.skBedLkg = S.cfg.skBedLkg*0.5; var h2: float = sim.skBedH()
	T.chk("REGRESE lože: výška = slad × skBedLkg / plocha (dřív pevné 2,7 L/kg)", absf(h1/h2 - 2.0) < 1e-9 and h1 > 0.05, "%.3f m → %.3f m" % [h1, h2])

	# --- kalibrace: vazby mezi parametry a stará migrace skPerm už nepřepíše ruční hodnotu ---
	_wcal('{"hltStart": 20000, "hltCap": 5000}'); sim.new_state()
	T.chk("KALIBRACE vazba: hltStart nad hltCap se sníží na kapacitu a hlásí se", sim.S.cfg.hltStart <= sim.S.cfg.hltCap + 1e-9 and sim.calib_rejected.size() > 0, "%s / %s" % [str(sim.S.cfg.hltStart), str(sim.S.cfg.hltCap)])
	_wcal('{"skDpCompact": 300, "skDpChannel": 100}'); sim.new_state()
	T.chk("KALIBRACE vazba: práh slehávání nad prahem kanálků se vrátí na výchozí dvojici", sim.S.cfg.skDpCompact < sim.S.cfg.skDpChannel and sim.calib_rejected.size() > 0, "%s < %s" % [str(sim.S.cfg.skDpCompact), str(sim.S.cfg.skDpChannel)])
	_delcal(); T.fresh(); m.save_game()
	_wcal('{"skPerm": 3e-12}'); m.load_game()
	T.chk("KALIBRACE skPerm 3e-12 z kalibrace.json přežije načtení várky (dřív ji migrace starých uložení přepsala na 1e-11)", absf(sim.S.cfg.skPerm - 3e-12) < 1e-15, "%s" % str(sim.S.cfg.skPerm))
	_delcal(); T.fresh(); sim.S.cfg.skPerm = 1e-12; m.save_game(); m.load_game()
	T.chk("KALIBRACE staré uložení s nízkou propustností se přepočítá na výchozí skPerm", absf(sim.S.cfg.skPerm - sim.CFG_DEFAULT.skPerm) < 1e-15, "%s" % str(sim.S.cfg.skPerm))
	_delcal(); T.fresh()

	# --- deskový chladič: ε-NTU nezávisle, nulový průtok, UA1 ---
	T.fresh(); S = sim.S; c = S.cfg
	var pid := ""
	for it in m.items: if it.type == "vessel" and it.get("shape", "") == "plate": pid = it.id
	if pid != "":
		var ua0: float = c.UA2 if sim.net.byId[pid].get("bind") == "ch2" else c.UA1
		S.plates[pid] = {"kind": "water", "T": 16.0, "flow": c.plateRefQ*1.1}
		var Tout: float = sim.plate_cool(pid, 95.0, c.plateRefQ, 1.0/60.0)
		var Cw: float = c.plateRefQ/60.0*1.04*4.0; var Cc: float = c.plateRefQ*1.1/60.0*4.19
		var Cmin: float = minf(Cw, Cc); var Cr: float = Cmin/maxf(Cw, Cc); var ntu: float = ua0/Cmin
		var x: float = exp(-ntu*(1.0 - Cr)); var eff: float = (1.0 - x)/(1.0 - Cr*x)
		var Tref: float = 95.0 - eff*Cmin*(95.0 - 16.0)/Cw
		T.chk("REGRESE chladič: při referenčním průtoku plateRefQ a UA z kalibrace sedí výstup s nezávislým ε-NTU", absf(Tout - Tref) < 0.05, "%.2f vs %.2f °C (UA %.0f kW/K, ε %.3f)" % [Tout, Tref, ua0, eff])
		var T0: float = sim.plate_cool(pid, 95.0, 0.0, 1.0/60.0)
		T.chk("REGRESE chladič: nulový průtok mladiny → vrací vstupní teplotu, žádné NaN", absf(T0 - 95.0) < 1e-9)
		S.plates[pid].flow = 0.0
		var Tz: float = sim.plate_cool(pid, 95.0, 80.0, 1.0/60.0)
		T.chk("REGRESE chladič: nulový průtok chladiva → konečný výsledek, mladina se prakticky neochladí", _fin(Tz) and Tz > 90.0, "%.2f °C" % Tz)
		c[("UA2" if sim.net.byId[pid].get("bind") == "ch2" else "UA1")] = ua0*0.5
		S.plates[pid] = {"kind": "water", "T": 16.0, "flow": c.plateRefQ*1.1}
		var Tlow: float = sim.plate_cool(pid, 95.0, c.plateRefQ, 1.0/60.0)
		T.chk("REGRESE chladič: poloviční UA → teplejší výstup mladiny", Tlow > Tout + 0.5, "%.1f → %.1f °C" % [Tout, Tlow])

	# --- rmutování: bilance extraktu, krok, citlivost na parametry, teplotní optimum; nic není nastaveno na cílové prokvašení ---
	var m63 := _mash(63.0, 30.0, 0.5)
	T.chk("REGRESE rmutování: extrakt (škrob + dextriny + dlouhé + zkvasitelné) se při 30 min při 63 °C zachová", absf(m63.sum - m63.sum0) < 1e-6*m63.sum0, "%.4f vs %.4f kg" % [m63.sum, m63.sum0])
	var m63b := _mash(63.0, 30.0, 0.05)
	T.chk("REGRESE rmutování: zkvasitelné cukry nezávisí na kroku (0,5 vs 0,05 min) o víc než 5 %", absf(m63.f - m63b.f) < 0.05*maxf(1.0, m63b.f), "%.1f vs %.1f kg" % [m63.f, m63b.f])
	var mb1 := _mash(63.0, 5.0, 0.5)
	var mb2 := _mash(63.0, 5.0, 0.5, 9.0)
	T.chk("REGRESE rmutování: betaAct ×2 → po 5 min výrazně víc zkvasitelných cukrů (parametr opravdu působí)", mb2.f > mb1.f*1.15, "%.1f → %.1f kg" % [mb1.f, mb2.f])
	var mk1: Dictionary = _mash(63.0, 10.0, 0.5)
	var mk: Dictionary = _mash(63.0, 10.0, 0.5, 4.5, 0.52)
	T.chk("REGRESE rmutování: kdBeta63 ×4 (β-amyláza se ničí rychleji) → po 10 min míň zkvasitelných cukrů a míň zbylého enzymu", mk.f < mk1.f*0.97 and mk.aB < mk1.aB*0.1, "%.1f vs %.1f kg, aB %.2f vs %.2f" % [mk.f, mk1.f, mk.aB, mk1.aB])
	var m72 := _mash(72.0, 30.0, 0.5)
	T.chk("REGRESE rmutování: při 72 °C je podíl zkvasitelných cukrů nižší než při 63 °C (β-amyláza se tepelně ničí) – výsledek není nastaven na cíl", m72.frac < m63.frac*0.7, "%.2f vs %.2f" % [m72.frac, m63.frac])
	var m0 := _mash(63.0, 0.0, 0.5)
	T.chk("REGRESE rmutování: nulový čas = beze změny", m0.f == 0.0 and m0.sum == m0.sum0)

	# --- kvašení a CIP: parametry z konfigurace opravdu mění výsledek ---
	var f0: Dictionary = _fer_run({}, 48)
	var fq: Dictionary = _fer_run({"ferQ": 0.1}, 48)
	T.chk("REGRESE kvašení: ferQ ×0,5 → po 48 h zbývá víc extraktu (parametr působí)", fq.Sf > f0.Sf + 1.0 and f0.ok and fq.ok, "%.1f vs %.1f g/L" % [fq.Sf, f0.Sf])
	var fh: Dictionary = _fer_run({"ferHeat": 0.2}, 48)
	T.chk("REGRESE kvašení: menší reakční teplo (ferHeat 0,2 vs 0,6 kJ/g) → nižší nárůst teploty", (fh.Tmax - 9.0) < (f0.Tmax - 9.0) and f0.Tmax > 9.0, "+%.2f vs +%.2f K" % [fh.Tmax - 9.0, f0.Tmax - 9.0])
	T.fresh(); S = sim.S; S.cfg.cipMin = 5.0
	S.cip.target = "rvp"; S.cip.medium = "louh"; S.cip.set = 80.0; S.cip.T = 80.0; S.cip.conc = 1.5; S.cip.run = true
	for i in 6: sim.proc.cip_step(60.0)
	var quick: bool = S.cip.done.has("rvp:louh!")
	T.fresh(); S = sim.S; S.cfg.cipLyeT = 85.0
	S.cip.target = "rvp"; S.cip.medium = "louh"; S.cip.set = 80.0; S.cip.T = 80.0; S.cip.conc = 1.5; S.cip.run = true
	for i in 40: sim.proc.cip_step(60.0)
	T.chk("REGRESE CIP: cipMin = 5 min stačí na splnění; cipLyeT = 85 °C se při 80 °C nesplní ani za 40 min", quick and not S.cip.done.has("rvp:louh!"))

	# --- extrémní hodnoty: každý parametr na dolní i horní mez rozsahu, simulace zůstane konečná ---
	var offenders := []
	for k in sim.CALIB_RANGE:
		var r = sim.CALIB_RANGE[k]
		for side in 2:
			T.fresh(); S = sim.S; c = S.cfg
			c[k] = float(r[side])
			S.rvp.w = 2000.0; S.rvp.T = 40.0; S.rvp.Tsens = 40.0; S.rvp.stir = true; S.rvp.heat = 1.0; S.rvp.g = 200.0
			S.mp.w = 1500.0; S.mp.T = 90.0; S.mp.Tsens = 90.0; S.mp.heat = 1.0
			S.hlt.heat = 1.0; S.lv.chill = true
			for i in 60: sim.sim_step(5.0)
			sim.skQmax()
			var okv: bool = _fin(S.rvp.T) and _fin(S.mp.T) and _fin(S.hlt.T) and _fin(S.lv.T) and _fin(S.boiler.p) and _fin(S.hlt.w) and _fin(S.sk.Qmax)
			if not okv: offenders.append("%s=%s" % [k, str(r[side])])
	T.chk("REGRESE extrémní hodnoty: žádný z %d parametrů na mezi rozsahu nevyrobí NaN/∞ v teplotách, tlaku páry ani propustnosti" % sim.CALIB_RANGE.size(), offenders.is_empty(), ", ".join(PackedStringArray(offenders)))
	_delcal(); T.fresh()
