extends RefCounted
## Testy balíčku FYZIKA+ – spouští je tests.gd (řádek označený „FYZIKA+“), takže běží s `-- --tests`.
## Test háčku, který se do sim.gd nepodařilo vložit, se přeskočí (seznam vložených háčků je ve fyzika_plus_hooks.gd).

const FP = preload("res://fyzika_plus.gd")
const HOOKS = preload("res://fyzika_plus_hooks.gd")

var T
var sim

func run(t) -> void:
	T = t; sim = t.sim
	_props()
	_valve()
	if _has("H1"): _steam_sim()
	if _has("H2"): _film()
	_ph_color()
	if _has("H5"): _whirl()
	if _has("H6"): _pipe()
	if _has("H7"): _volume()
	if _has("H10"): _coil()
	if _has("H14"): _burn()
	if _has("H11"): _ice()
	if _has("H15"): _gravity()
	_geom()
	_evap()
	_mashv()
	T.fresh()

## čistý stav + otevřené klapky na parním potrubí (plánek s parními trubkami by jinak po fresh() neměl páru)
func _fs() -> void:
	T.fresh()
	var ch := false
	for k in sim.net.nodes:
		var n = sim.net.nodes[k]
		if n.valve == null: continue
		var steam := false
		for e in n.edges:
			if e.medium == "steam": steam = true
		if not steam: continue
		if T.m.outlet_open.has(n.valve): T.m.outlet_open[n.valve] = true
		elif sim.net.byId.has(n.valve): sim.net.byId[n.valve]["open"] = true
		ch = true
	if ch: sim.valve_ver += 1

## otevře klapky na trase (stejně jako testy programu); false = trasa v plánku není
func _open_route(a: String, b: String, pumps: Array, plate: String = "") -> bool:
	var ids = sim.route_valves(a, b, pumps, -1, plate)
	if ids == null: return false
	for id in ids:
		if T.m.outlet_open.has(id): T.m.outlet_open[id] = true
		elif sim.net.byId.has(id): sim.net.byId[id]["open"] = true
	sim.valve_ver += 1
	return true

func _has(h: String) -> bool:
	if h in HOOKS.APPLIED: return true
	T.chk("FP háček %s není vložený – test přeskočen (viz zpráva aplikuj_fyzika_plus.py)" % h, true)
	return false

# ---------- vlastnosti vody a páry ----------
func _props() -> void:
	T.chk("FP voda: hustota 20 °C 998,2 kg/m³, 100 °C 958,4 kg/m³ (Kell 1975)", absf(FP.water_rho(20.0) - 998.21) < 0.05 and absf(FP.water_rho(100.0) - 958.35) < 0.3, "%.2f / %.2f" % [FP.water_rho(20.0), FP.water_rho(100.0)])
	T.chk("FP voda: roztažnost 20→100 °C 4,16 %, 20→50 °C 1,03 % (lineární model dával 1,56 %)", absf(FP.expansion(100.0) - 1.0416) < 0.0006 and absf(FP.expansion(50.0) - 1.0103) < 0.0006, "%.4f / %.4f" % [FP.expansion(100.0), FP.expansion(50.0)])
	var ok := true; var det := ""
	for t_ in [20.0, 60.0, 99.0, 101.0, 139.0]:
		var back: float = FP.tsat(FP.psat(t_))
		if absf(back - t_) > 0.05:
			ok = false; det += "%.1f→%.2f " % [t_, back]
	T.chk("FP pára: tsat(psat(T)) = T i pod 1 bar (inverze)", ok, det)
	T.chk("FP pára (IAPWS-IF97): 1,01325 bar → 99,97 °C, 3,5 bar → 138,86 °C, 0,5 bar → 81,32 °C, 20 °C → 0,02339 bar", absf(FP.tsat(1.01325) - 99.974) < 0.02 and absf(FP.tsat(3.5) - 138.86) < 0.02 and absf(FP.tsat(0.5) - 81.32) < 0.02 and absf(FP.psat(20.0) - 0.02339) < 0.0001, "%.3f / %.3f / %.3f °C, %.5f bar" % [FP.tsat(1.01325), FP.tsat(3.5), FP.tsat(0.5), FP.psat(20.0)])
	var mono := true; var pv := 0.0
	for i in 200:
		var p_: float = FP.psat(float(i))
		if p_ <= pv: mono = false
		pv = p_
	T.chk("FP pára: tlak sytých par roste s teplotou plynule 0–199 °C", mono)
	T.chk("FP pára: výparné teplo podle IAPWS-IF97: 2453,5 kJ/kg při 20 °C, 2256,5 při 100 °C, 2147,7 při 138,86 °C (±3)", absf(FP.hfg(20.0) - 2453.5) < 3.0 and absf(FP.hfg(100.0) - 2256.5) < 3.0 and absf(FP.hfg(138.86) - 2147.7) < 3.0, "%.1f / %.1f / %.1f" % [FP.hfg(20.0), FP.hfg(100.0), FP.hfg(138.86)])

# ---------- ventil a plášť (čisté funkce) ----------
func _valve() -> void:
	T.chk("FP ventil: charakteristika 0 → 0, 1 → 1, lineární (R = 1) → x", FP.kv_frac(0.0, 30.0) == 0.0 and absf(FP.kv_frac(1.0, 30.0) - 1.0) < 1e-9 and absf(FP.kv_frac(0.5, 1.0) - 0.5) < 1e-9)
	T.chk("FP ventil: ekviprocentní (R 30) Kv/Kvs = R^(zdvih − 1): 50 % → 18,3 %, 25 % → 7,8 %, pod 5 % lineárně k nule", absf(FP.kv_frac(0.5, 30.0) - 0.18257) < 0.0005 and absf(FP.kv_frac(0.25, 30.0) - 0.07792) < 0.0005 and FP.kv_frac(0.02, 30.0) < FP.kv_frac(0.05, 30.0), "%.4f / %.4f" % [FP.kv_frac(0.5, 30.0), FP.kv_frac(0.25, 30.0)])
	var crit: float = FP.steam_kgs(10.0, 4.0, 1.0)
	T.chk("FP ventil: kritický tok syté páry = 12·Kv·P1 kg/h (Spirax Sarco)", absf(crit * 3600.0 - 480.0) < 0.01, "%.1f kg/h" % (crit * 3600.0))
	T.chk("FP ventil: bez rozdílu tlaků nic neteče, s nižším tlakem za ventilem víc", FP.steam_kgs(10.0, 4.0, 4.0) == 0.0 and FP.steam_kgs(10.0, 4.0, 3.0) < FP.steam_kgs(10.0, 4.0, 2.0))
	var J: Dictionary = FP.jacket_solve(20.0, 3.513, 6.5, 60.0)
	var vflow: float = FP.steam_kgs(20.0, 3.513, J.p)
	T.chk("FP plášť: tlak v plášti je mezi tlakem par náplně a kotle; pára ventilem = zkondenzovaná pára", J.p > FP.psat(60.0) and J.p < 3.513 and absf(vflow - J.kgs) < 0.002 * maxf(0.01, J.kgs), "p %.2f bar abs, %.0f kg/h, %.0f kW" % [J.p, J.kgs * 3600.0, J.P])
	var Ja: Dictionary = FP.jacket_solve(20.0, 3.513, 6.5, 60.0, 0.25)
	T.chk("FP plášť: 25 % vzduchu sníží teplotu kondenzace o ~7 K (Dalton; Spirax: při 2 bar abs. o 8 K)", J.Tj - Ja.Tj > 4.0 and Ja.P < J.P, "%.1f → %.1f °C" % [J.Tj, Ja.Tj])
	var J0: Dictionary = FP.jacket_solve(20.0, 3.513, 0.0, 20.0)
	T.chk("FP plášť: prázdný plášť s otevřenou párou má tlak kotle (žádná kondenzace, žádný falešný podtlak)", absf(J0.p - 3.513) < 1e-6 and J0.P == 0.0, "%.2f bar abs" % J0.p)
	var Js: Dictionary = FP.jacket_solve(0.5, 3.513, 6.5, 30.0)
	T.chk("FP plášť: přiškrcená pára na studenou náplň → v plášti podtlak (pod 1 bar abs.)", Js.p < 1.0 and Js.P > 1.0, "%.2f bar abs, %.0f kW" % [Js.p, Js.P])

# ---------- pára v simulaci ----------
func _steam_sim() -> void:
	# A) plně otevřený ventil: výkon blízko dřívějšího modelu UA·smáčení·(T_páry − T)
	_fs(); var S = sim.S; var c = S.cfg
	var v = S.rvp; v.w = 2000.0; v.T = 60.0; v.Tsens = 60.0; v.Twall = 60.0; v.stir = true; v.hz = 35.0; v.heat = 1.0
	sim.sim_step(1.0); sim.sim_step(1.0)
	var wet: float = float(S._pd.rvp.get("wet", 1.0))
	var old_P: float = c.rvpUAst * wet * sim.film_factor(v) * (sim.tsat(S.boiler.p + 1.013) - v.T)
	T.chk("FP pára: plně otevřený ventil dá 85–100 % výkonu dřívějšího modelu (kalibrace rvpUAst platí dál)", v.Pin > 0.85 * old_P and v.Pin <= old_P * 1.001, "%.0f kW vs %.0f kW, plášť %.2f bar abs" % [v.Pin, old_P, float(v.get("jp", 0.0))])
	# B) ekviprocentní ventil: 25 % zdvihu = malý výkon (jemná regulace prodlev); plný výkon se měří zvlášť (bez vzduchu v plášti)
	var P_full: float = v.Pin
	_fs(); S = sim.S; c = S.cfg; c["stValveR"] = 30.0
	v = S.rvp; v.w = 2000.0; v.T = 60.0; v.Tsens = 60.0; v.Twall = 60.0; v.stir = true; v.hz = 35.0; v.heat = 0.25
	sim.sim_step(1.0); sim.sim_step(1.0)
	var P25: float = v.Pin
	T.chk("FP pára: ekviprocentní ventil (R 30) při 25 % zdvihu dá pod 15 % plného výkonu", P25 > 0.0 and P25 < 0.15 * P_full, "%.0f kW vs %.0f kW" % [P25, P_full])
	# C) stall: přiškrcená pára na chladný rmut → plášť pod protitlakem kondenzátu se zaplavuje; po otevření kondenzát odteče
	_fs(); S = sim.S; c = S.cfg
	v = S.rvp; v.w = 2500.0; v.T = 45.0; v.Tsens = 45.0; v.stir = true; v.hz = 35.0; v.heat = 0.2
	for i in 600: sim.sim_step(1.0)
	var fl1: float = float(v.get("jfl", 0.0)); var pj1: float = float(v.get("jp", 9.0))
	v.heat = 1.0
	for i in 300: sim.sim_step(1.0)
	var fl2: float = float(v.get("jfl", 0.0))
	T.chk("FP stall: přiškrcená pára (20 %) na chladnou náplň → tlak v plášti pod protitlakem kondenzátu, plášť se zaplavuje; po plném otevření kondenzát odteče", pj1 < 1.013 + float(c.get("stCondBack", 0.15)) and fl1 > 0.01 and fl2 < fl1 * 0.5, "p %.2f bar abs, zaplavení %.1f %% → %.1f %%" % [pj1, fl1 * 100.0, fl2 * 100.0])
	# D) podtlak po zavření páry: bez zavzdušňovacího ventilu varování, s ním vzduch v plášti
	_fs(); S = sim.S; c = S.cfg; c["jVacBreaker"] = 0.0
	v = S.rvp; v.w = 2500.0; v.T = 63.0; v.Tsens = 63.0; v.stir = true; v.hz = 35.0; v.heat = 1.0
	for i in 30: sim.sim_step(1.0)
	v.heat = 0.0
	for i in 30: sim.sim_step(1.0)
	var vac_msg: bool = S.log.any(func(l): return l.msg.contains("zborcení"))
	T.chk("FP podtlak: bez zavzdušňovacího ventilu spadne plášť po zavření páry pod 0,5 bar abs. a trenažér varuje (Kunze s. 290–291)", float(v.get("jp", 9.0)) < 0.5 and vac_msg, "%.2f bar abs" % float(v.get("jp", 9.0)))
	_fs(); S = sim.S
	v = S.rvp; v.w = 2500.0; v.T = 63.0; v.Tsens = 63.0; v.stir = true; v.hz = 35.0; v.heat = 1.0
	for i in 30: sim.sim_step(1.0)
	v.heat = 0.0
	for i in 120: sim.sim_step(1.0)
	T.chk("FP podtlak: se zavzdušňovacím ventilem nasaje plášť vzduch, tlak se vrátí na ~1 bar abs.", float(v.get("jy", 0.0)) > 0.5 and float(v.get("jp", 0.0)) > 0.98, "vzduch %.2f, %.2f bar abs" % [float(v.get("jy", 0.0)), float(v.get("jp", 0.0))])
	# E) zavzdušněný plášť topí nejdřív slabě, odvzdušňovač ho do pár minut vyčistí
	_fs(); S = sim.S
	v = S.rvp; v.w = 2500.0; v.T = 50.0; v.Tsens = 50.0; v.stir = true; v.hz = 35.0; v["jy"] = 1.0; v.heat = 1.0
	sim.sim_step(1.0); sim.sim_step(1.0)
	var P0: float = v.Pin
	for i in 180: sim.sim_step(1.0)
	T.chk("FP vzduch: zavzdušněný plášť topí nejdřív slabě, po odvzdušnění naplno", P0 < 0.3 * v.Pin and float(v.get("jy", 1.0)) < 0.02, "%.0f → %.0f kW, vzduch %.3f" % [P0, v.Pin, float(v.get("jy", 1.0))])
	# F) 1. zákon: energie páry = ohřev vody a oceli + ztráty (i s ventilem a pláštěm)
	_fs(); S = sim.S; c = S.cfg
	v = S.rvp; v.w = 2000.0; v.T = 20.0; v.Tsens = 20.0; v.stir = true; v.heat = 0.6
	var H0: float = (sim.cC(v) + sim.steelC(c.rvpSteel)) * v.T
	var Ein := 0.0; var Eloss := 0.0
	for i in 900:
		sim.sim_step(1.0)
		Ein += v.Pin; Eloss += c.rvpUA * (v.T - c.ambientT)
	var H1: float = (sim.cC(v) + sim.steelC(c.rvpSteel)) * v.T
	T.chk("FP pára: energie páry = ohřev náplně a oceli + ztráty (1. zákon, chyba < 2 %)", absf((Ein - Eloss) - (H1 - H0)) < 0.02 * maxf(1.0, Ein), "dodáno %.0f MJ" % (Ein / 1000.0))
	# G) zavřená parní trasa v plánku → pára nejde, kotel nic nevyrábí, plášť nic neukazuje
	_fs(); S = sim.S
	v = S.rvp; v.w = 2000.0; v.T = 60.0; v.stir = true; v.heat = 1.0
	S["steamOK"] = {"rvp": false, "mp": true, "hlt": true}
	var Pz: Dictionary = FP.steam_pd(sim, v, S.cfg.rvpUAst, S.cfg.rvpJacketL, 139.0)
	T.chk("FP pára: zavřená parní trasa = žádná pára ani zátěž kotle", Pz.P == 0.0 and float(v.get("jkgs", 1.0)) == 0.0)
	# H) poddimenzovaný odvaděč kondenzátu: plášť se zaplavuje i při přetlaku
	_fs(); S = sim.S; c = S.cfg; c["jDrainKgS"] = 0.05
	v = S.rvp; v.w = 2500.0; v.T = 60.0; v.Tsens = 60.0; v.stir = true; v.hz = 35.0; v.heat = 1.0
	for i in 300: sim.sim_step(1.0)
	T.chk("FP kondenzát: malý odvaděč (0,05 kg/s) nestíhá odvádět kondenzát ani při přetlaku → plášť se zaplavuje", float(v.get("jfl", 0.0)) > 0.1 and float(v.get("jp", 0.0)) > 1.013 + float(c.get("stCondBack", 0.15)), "zaplavení %.0f %%, %.2f bar abs" % [float(v.get("jfl", 0.0)) * 100.0, float(v.get("jp", 0.0))])

# ---------- přestup na straně náplně ----------
func _film() -> void:
	T.fresh()
	var w = sim.vessel(); w.w = 2000.0; w.T = 60.0; w.stir = true; w.hz = 35.0
	T.chk("FP přestup: voda 60 °C, 35 Hz = referenční přestup (faktor 1)", absf(sim.film_factor(w) - 1.0) < 1e-6, "%.4f" % sim.film_factor(w))
	var cold = sim.vessel(); cold.w = 2000.0; cold.T = 20.0; cold.stir = true; cold.hz = 35.0
	var hot = sim.vessel(); hot.w = 2000.0; hot.T = 95.0; hot.stir = true; hot.hz = 35.0
	T.chk("FP přestup: studená voda (vyšší viskozita) předává hůř než horká", sim.film_hA(cold) < sim.film_hA(w) and sim.film_hA(hot) > sim.film_hA(w), "%.1f / %.1f / %.1f kW/K" % [sim.film_hA(cold), sim.film_hA(w), sim.film_hA(hot)])
	var nat = sim.vessel(); nat.w = 4000.0; nat.f = 480.0; nat.T = 80.0; nat.Twall = 110.0; nat.stir = false
	var mash = sim.vessel(); mash.w = 2000.0; mash.T = 63.0; mash.Twall = 93.0; mash.stir = false; mash.g = 150.0; mash.su = 300.0
	var href: float = sim.ha_ref() if sim.has_method("ha_ref") else 25.0
	var nst = nat.duplicate(); nst.stir = true; nst.hz = 35.0
	var r_nat: float = sim.film_factor(nat) / sim.film_factor(nst)
	var r_mash: float = sim.film_hA(mash) / href
	T.chk("FP přestup: nemíchaná řídká mladina má 45–75 % celkového prostupu míchané (učebnicově ~0,6), hustý rmut bez míchadla skoro nic", r_nat > 0.45 and r_nat < 0.75 and r_mash < 0.05, "mladina %.2f, rmut %.3f" % [r_nat, r_mash])
	_fs(); var S = sim.S
	var v = S.rvp; v.w = 3900.0; v.f = 480.0; v.T = 72.0; v.Tsens = 72.0; v.Twall = 72.0; v.stir = false; v.heat = 1.0
	for i in 600: sim.sim_step(1.0)
	var rate: float = (v.T - 72.0) / 10.0
	T.chk("FP přestup: nemíchaná mladina 43 hl s plnou párou se ohřívá 0,6–1,5 K/min (dřív 0,72; Kunze: moderní pánev k varu za ~15 min)", rate > 0.6 and rate < 1.5, "%.2f K/min" % rate)

# ---------- pH a barva (čisté funkce, nepotřebují háčky) ----------
func _ph_color() -> void:
	T.fresh(); var S = sim.S
	S.malt.added = 700.0; S["pH"] = 5.6
	var v = S.rvp; v.w = 3900.0; v.f = 480.0; v.T = 99.3
	var ph0: float = FP.wort_ph(sim)
	for i in 90:
		FP.boil_step(sim, v, 1.0); S.t += 60.0
	FP.boil_step(sim, v, 1.0)          # stejný okamžik podruhé (druhá vroucí pánev) se nepočítá
	var drop: float = ph0 - FP.wort_ph(sim)
	T.chk("FP pH: 90 min varu sníží pH mladiny o 0,12 (VLB 5,62 → 5,50; Kunze 5,5–5,6 → 5,4–5,5)", absf(drop - 0.12) < 0.02, "%.3f" % drop)
	var ph1: float = FP.wort_ph(sim)
	FP.acid_dose(sim, 530.0)
	var dacid: float = ph1 - FP.wort_ph(sim)
	T.chk("FP pH: 530 mL kyseliny mléčné 80 % na 700 kg sladu sníží pH mladiny o 0,30 (Kunze: 30 mL 0,8% roztoku na kg na 0,1 pH)", absf(dacid - 0.30) < 0.03, "%.3f" % dacid)
	T.chk("FP pH: mladina v pánvi má pH mladiny, rmut pH rmutu", absf(FP.vessel_ph(sim, v) - FP.wort_ph(sim)) < 1e-9 and absf(FP.vessel_ph(sim, sim.vessel()) - S.pH) < 1e-9)
	# barva
	T.fresh(); S = sim.S; S.malt.added = 700.0; S["pH"] = 5.5 - FP.cf(sim, "phSparge")
	var b1 = sim.vessel(); b1.w = 1000.0; b1.f = 136.4; b1.T = 100.0
	for i in 90: FP.color_step(sim, b1, 1.0)
	var e1: float = b1.col / sim.liqVol(b1)
	T.chk("FP barva: 90 min při 100 °C, pH 5,5, 12 °P vytvoří 1,6–2,0 EBC (VLB: 5,7 → 8,0 EBC i se zahuštěním odparem)", e1 > 1.6 and e1 < 2.0, "+%.2f EBC" % e1)
	S["pH"] = 5.2 - FP.cf(sim, "phSparge")
	var b2 = sim.vessel(); b2.w = 1000.0; b2.f = 136.4; b2.T = 100.0
	for i in 90: FP.color_step(sim, b2, 1.0)
	T.chk("FP barva: při pH 5,2 vzniká méně barvy (Kunze s. 286)", b2.col / sim.liqVol(b2) < e1 * 0.85, "+%.2f EBC" % (b2.col / sim.liqVol(b2)))
	S["pH"] = 5.5 - FP.cf(sim, "phSparge")
	var b3 = sim.vessel(); b3.w = 1000.0; b3.f = 136.4; b3.T = 95.0
	for i in 30: FP.color_step(sim, b3, 1.0)
	var b4 = sim.vessel(); b4.w = 1000.0; b4.f = 136.4; b4.T = 65.0
	for i in 60: FP.color_step(sim, b4, 1.0)
	var e3: float = b3.col / sim.liqVol(b3)
	T.chk("FP barva: ve vířivce (95 °C, 30 min) přibude 0,35–0,55 EBC, pod 70 °C nic", e3 > 0.35 and e3 < 0.55 and b4.col == 0.0, "+%.2f EBC" % e3)

# ---------- vířivka ----------
func _whirl() -> void:
	T.fresh(); var S = sim.S; S.cfg["wpInletMM"] = 50.0     # hrdlo 50 mm (výchozí 0 = jako trubka DN50 v plánku)
	var p = sim.lp(); p.w = 300.0; p.f = 40.0; p.T = 97.0; p.trubSus = 3.0
	sim.dstGive("wp", p, {"src": "rvp", "m": 1.0})          # ~325 L/min do Ø 50 mm ≈ 2,8 m/s
	var ns_ok: float = float(S.wp.get("trubNS", 0.0)); var v_ok: float = float(S.wp.get("inletV", 0.0))
	var p2 = sim.lp(); p2.w = 600.0; p2.f = 80.0; p2.T = 97.0; p2.trubSus = 6.0
	sim.dstGive("wp", p2, {"src": "rvp", "m": 1.0})         # ~650 L/min ≈ 5,5 m/s
	var warn: bool = S.log.any(func(l): return l.msg.contains("3,5 m/s"))
	T.chk("FP vířivka: nátok do 3,5 m/s lom netříští, rychlejší ano (a trenažér varuje)", ns_ok == 0.0 and v_ok > 2.0 and v_ok < 3.5 and float(S.wp.get("trubNS", 0.0)) > 0.0 and warn, "%.1f → %.1f m/s, netříštěný kal %.2f kg" % [v_ok, float(S.wp.get("inletV", 0.0)), float(S.wp.get("trubNS", 0.0))])

# ---------- potrubí ----------
func _pipe() -> void:
	T.fresh()
	var fake = {"dn": 50.0, "cap": 10.0, "c": sim.vessel()}
	fake.c.w = 10.0
	var k1h: float = FP.pipe_cool_k(sim, fake, 3600.0)
	var Tend: float = 20.0 + 60.0 * (1.0 - k1h)
	T.chk("FP potrubí: plná trubka DN50 (Ø %d mm) za 1 h stání vychladne z 80 °C jen na 45–70 °C (dřív za 30 min skoro na okolí)" % roundi(sim.bore_mm(50.0)), Tend > 45.0 and Tend < 70.0, "%.1f °C" % Tend)

# ---------- objem horké kapaliny ----------
func _volume() -> void:
	var hv = sim.vessel(); hv.w = 1000.0; hv.T = 50.0
	T.chk("FP objem: kapalina při 50 °C má o 1,03 % větší objem než při 20 °C (Kell; lineárně bylo 1,56 %)", absf(sim.hotV(hv) / sim.liqVol(hv) - 1.0103) < 0.001, "%.4f" % (sim.hotV(hv) / sim.liqVol(hv)))

# ---------- topný had bojleru ----------
func _coil() -> void:
	_fs(); var S = sim.S; var c = S.cfg
	var h = S.hlt; h.w = 8000.0; h.T = 40.0; h.auto = false; h.heat = 1.0
	sim.sim_step(1.0); sim.sim_step(1.0)
	var old_P: float = c.hltUAst * (sim.tsat(S.boiler.p + 1.013) - h.T)
	var P: float = float(S._pd.hlt.P)
	T.chk("FP bojler: plně otevřená pára do topného hadu dá 85–100 % výkonu dřívějšího modelu (kalibrace hltUAst platí dál)", P > 0.85 * old_P and P <= old_P * 1.001, "%.0f kW vs %.0f kW, had %.2f bar abs" % [P, old_P, float(h.get("jp", 0.0))])
	# bilance: energie páry = ohřev vody a oceli + ztráty
	_fs(); S = sim.S; c = S.cfg
	h = S.hlt; h.w = 8000.0; h.T = 40.0; h.auto = false; h.heat = 1.0
	var C: float = 4.19 * h.w + sim.steelC(c.hltSteel)
	var T0: float = h.T; var Ein := 0.0; var Eloss := 0.0
	for i in 600:
		sim.sim_step(1.0)
		Ein += float(S._pd.hlt.P); Eloss += c.hltUA * (h.T - c.ambientT)
	T.chk("FP bojler: energie páry = ohřev vody a oceli + ztráty (chyba < 2 %)", absf((Ein - Eloss) - C * (h.T - T0)) < 0.02 * maxf(1.0, Ein), "dodáno %.0f MJ, +%.1f K" % [Ein / 1000.0, h.T - T0])
	_fs(); S = sim.S
	h = S.hlt; h.w = 8000.0; h.T = 40.0; h.heat = 1.0
	S["steamOK"] = {"rvp": true, "mp": true, "hlt": false}
	var Pz: Dictionary = FP.steam_coil(sim, h, S.cfg.hltUAst, 139.0)
	T.chk("FP bojler: zavřená parní trasa = žádná pára do hadu", Pz.P == 0.0)

# ---------- připalování a bilance energie ----------
func _burn() -> void:
	_fs(); var S = sim.S; var c = S.cfg
	var v = S.rvp; v.w = 2000.0; v.g = 140.0; v.su = 420.0; v.T = 90.0; v.Tsens = 90.0; v.Twall = 90.0; v.stir = false; v.heat = 1.0
	var same := true; var burned := false; var Psum := 0.0; var Vsum := 0.0
	if sim.has_meta("fpH14"): sim.remove_meta("fpH14")
	for i in 60:
		sim.sim_step(1.0)
		Psum += float(S._pd.rvp.P); Vsum += v.Pin
		if absf(float(S._pd.rvp.P) - v.Pin) > 1e-6: same = false
		if sim.has_meta("fpH14"): burned = true
	T.chk("FP připalování: hustý rmut bez míchadla se připaluje a kotel dodá přesně tolik, kolik pánev přijme (dřív 10 % energie mizelo)", burned and same and v.burn > 0.0 and absf(Psum - Vsum) < 1e-3, "kotel %.1f MJ, pánev %.1f MJ, u stěny %.0f °C" % [Psum / 1000.0, Vsum / 1000.0, v.Twall])
	# nános zhorší prostup o 10 %: při plně otevřené páře klesne výkon o něco méně (tlak v plášti se zvedne)
	var href: float = sim.ha_ref() if sim.has_method("ha_ref") else maxf(25.0, 2.0 * float(c.rvpUAst))
	var gs: float = 1.0 / (1.0 / c.rvpUAst - 1.0 / href)
	var hA: float = sim.film_hA(v)
	var kvs: float = FP.kvs_auto(c.rvpUAst, c.steamBar)
	var R1: Dictionary = FP._jacket_eq(kvs, c.steamBar + 1.013, 1.0, 0.0, gs, hA, 90.0, 0.0, true, 1.0)
	var R2: Dictionary = FP._jacket_eq(kvs, c.steamBar + 1.013, 1.0, 0.0, gs, hA, 90.0, 0.0, true, 0.9)
	T.chk("FP připalování: nános (−10 % prostupu) sníží výkon o 5–10 % a tlak v plášti stoupne", R2.P < R1.P * 0.95 and R2.P > R1.P * 0.89 and R2.p >= R1.p, "%.0f → %.0f kW, %.2f → %.2f bar abs" % [R1.P, R2.P, R1.p, R2.p])
	# stejný okamžik dvakrát (druhé volání má nulový krok, stav pláště se neposune): se značkou H14 a bez ní
	sim.set_meta("fpH14", true)
	var P_with: float = float(FP.steam_pd(sim, v, c.rvpUAst, c.rvpJacketL, 0.0).P)
	sim.remove_meta("fpH14")
	var P_without: float = float(FP.steam_pd(sim, v, c.rvpUAst, c.rvpJacketL, 0.0).P)
	sim.set_meta("fpH14", true)
	T.chk("FP připalování: úbytek výkonu opravdu počítá pára (FP.steam_pd) – se značkou háčku H14 je výkon o 3–12 % nižší než bez ní", P_without > 0.0 and P_with < P_without * 0.97 and P_with > P_without * 0.88, "%.1f vs %.1f kW" % [P_with, P_without])

# ---------- ledová voda ----------
func _ice() -> void:
	T.fresh(); var S = sim.S
	var q_all: float = FP.ice_flow(sim, [], 1); var q_two: float = FP.ice_flow(sim, [], 2)
	var qp: float = FP.cf(sim, "lvPumpQ")
	T.chk("FP ledová voda: bez přiškrcení teče výkon čerpadla (lvPumpQ, výchozí 120 L/min), dvě větve si ho dělí", absf(q_all - qp) < 1e-6 and absf(q_two - qp / 2.0) < 1e-6, "%.0f / %.0f L/min" % [q_all, q_two])
	var key = null
	for k in sim.net.nodes:
		var n = sim.net.nodes[k]
		if n.valve != null and sim.net.byId.has(n.valve) and sim.net.byId[n.valve].get("kind", "") != "skrt":
			key = k; break
	if key == null:
		T.chk("FP ledová voda: v plánku není klapka – test škrcení přeskočen", true); return
	var vv = sim.net.byId[sim.net.nodes[key].valve]
	var had: bool = vv.has("part"); var old = vv.get("part", null)
	vv["part"] = 50.0
	var q_thr: float = FP.ice_flow(sim, [{"to": key}], 1)
	if had: vv["part"] = old
	else: vv.erase("part")
	T.chk("FP ledová voda: klapka přivřená na 50 % průtok ubere (stejná křivka škrcení jako jinde v plánku)", q_thr < qp * 0.99 and absf(q_thr - qp * sim.throttle_flow(50.0)) < 1e-6, "%.0f L/min" % q_thr)
	# v simulaci: ledová voda přes chladič do bojleru – poloviční výkon čerpadla = poloviční průtok
	var moved := []
	for q_set in [qp, qp / 2.0]:
		T.fresh(); S = sim.S; S.cfg["lvPumpQ"] = q_set
		var plate_id := ""
		for it in T.m.items:
			if it.type == "vessel" and it.get("shape") == "plate": plate_id = it.id
		if not _open_route("lv", "hlt", ["plv"], plate_id):
			T.chk("FP ledová voda: trasa ledovka → bojler v plánku není – test průtoku v simulaci přeskočen", true); return
		S.lv.w = 8000.0; S.lv.T = 2.0; S.lv.pump = true; S.hlt.w = 2000.0
		var w0: float = S.lv.w
		for i in 60: sim.sim_step(1.0)
		moved.append(w0 - S.lv.w)
	T.chk("FP ledová voda: v simulaci teče z ledovky do bojleru podle lvPumpQ (poloviční výkon čerpadla = poloviční průtok)", moved[0] > 5.0 and absf(moved[1] / moved[0] - 0.5) < 0.03, "%.0f a %.0f L za minutu" % [moved[0], moved[1]])

# ---------- geometrie a bod varu u dna ----------
func _geom() -> void:
	T.fresh(); var S = sim.S; var c = S.cfg
	c["rvpDiaM"] = 2.0
	var v = S.rvp; v.w = 4000.0; v.T = 99.0
	var d: float = FP.depth_m(sim, "rvp", v)
	var tb: float = FP.boil_T_bottom(sim, "rvp", v)
	var p_bot: float = FP.psat(c.tbWater) + 4000.0 * 9.81 / (PI * 1.0) / 1.0e5
	T.chk("FP hladina: 40 hl vody 99 °C v pánvi Ø 2 m = hloubka 1,33 m, u dna +0,125 bar → vře až při ~102,6 °C", absf(d - 1.328) < 0.02 and absf(tb - FP.tsat(p_bot)) < 1e-6 and tb > c.tbWater + 2.5 and tb < c.tbWater + 4.0, "%.2f m, %.1f °C (hladina %.1f °C)" % [d, tb, c.tbWater])
	T.chk("FP hladina: 2,5 m mladiny = +0,25 bar u dna → bod varu 106 °C (Kunze s. 290)", absf(FP.tsat(1.013 + 0.25) - 106.0) < 0.3, "%.1f °C" % FP.tsat(1.013 + 0.25))
	c["rvpDiaM"] = 0.0
	T.chk("FP hladina: bez zadaného průměru se bere odhad z objemu (pánev 50 hl: Ø ~1,85 m, vířivka 50 hl s D : H = 3 : 1: Ø ~2,67 m)", absf(FP.vessel_dia(sim, "rvp") - pow(4.0 * c.rvpCap / 1000.0 / PI, 1.0 / 3.0)) < 1e-6 and absf(FP.vessel_dia(sim, "wp") - pow(12.0 * c.wpCap / 1000.0 / PI, 1.0 / 3.0)) < 1e-6, "%.2f / %.2f m" % [FP.vessel_dia(sim, "rvp"), FP.vessel_dia(sim, "wp")])

# ---------- samospád ----------
func _gravity() -> void:
	T.fresh(); var S = sim.S
	var e = null
	for ed in sim.net.edges:
		if ed.medium != "steam" and float(ed.len) > 150.0 and float(ed.dn) > 40.0:
			e = ed; break
	if e == null:
		T.chk("FP samospád: v plánku není vhodný úsek trubky – test přeskočen", true); return
	var path = [{"e": e, "to": e.b}]
	var q1: float = FP.gravity_q(sim, path, 1.0, "hlt"); var q2: float = FP.gravity_q(sim, path, 2.0, "hlt")
	var hl: float = sim.path_head(path, q1, 1.0, sim.skViscosity(S.hlt.T, 0.0))
	var a_out: float = PI * pow(sim.bore_mm(float(e.dn)) / 1000.0, 2) / 4.0
	var v1: float = q1 / 60000.0 / a_out
	T.chk("FP samospád: ztráta trasy + výtoková rychlostní výška = rozdíl hladin; rychlost nepřekročí √(2gh) (Torricelli); dvojnásobný rozdíl dá ~1,4× průtok", absf(hl + v1 * v1 / 19.62 - 1.0) < 0.01 and v1 < sqrt(19.62) and q2 / q1 > 1.3 and q2 / q1 < 1.5, "%.0f → %.0f L/min, %.2f m/s (mez %.2f), ztráta trasy %.2f m" % [q1, q2, v1, sqrt(19.62), hl])
	var b0: float = S.cfg.bore50
	S.cfg["bore50"] = b0 * 0.6
	var qn: float = FP.gravity_q(sim, path, 1.0, "hlt")
	S.cfg["bore50"] = b0
	var v = S.rvp; v.w = 2000.0; v.g = 140.0; v.su = 420.0; v.T = 50.0
	var qm: float = FP.gravity_q(sim, path, 1.0, "rvp")
	v.g = 0.0; v.su = 0.0
	var qw: float = FP.gravity_q(sim, path, 1.0, "rvp")
	T.chk("FP samospád: užší trubkou teče míň a hustý rmut teče pomaleji než voda", qn < q1 * 0.5 and qm < qw, "Ø %d mm: %.0f L/min, Ø %d mm: %.0f L/min; rmut %.0f vs voda %.0f L/min" % [roundi(b0), q1, roundi(b0 * 0.6), qn, qm, qw])
	# v simulaci: z pánve do vířivky samospádem (dřív nejvýš ~156 L/min)
	T.fresh(); S = sim.S
	if not _open_route("rvp", "wp", []):
		T.chk("FP samospád: trasa pánev → vířivka v plánku není – test v simulaci přeskočen", true); return
	S.rvp.w = 2500.0; S.rvp.T = 20.0
	var v0: float = sim.vol(S.rvp)
	for i in 60: sim.sim_step(1.0)
	var mv: float = v0 - sim.vol(S.rvp)
	T.chk("FP samospád: v simulaci teče z pánve do vířivky podle hydrauliky trasy (víc než dřívějších ~156 L/min)", mv > 200.0, "%.0f L za první minutu" % mv)

# ---------- odpar z hladiny ----------
func _evap() -> void:
	var e90: float = FP.evap_open(90.0, 20.0); var e60: float = FP.evap_open(60.0, 20.0)
	T.chk("FP odpar: volná hladina v klidném vzduchu odpaří při 90 °C ~10 kg/(m²·h), při 60 °C zhruba čtvrtinu (ASHRAE)", e90 > 9.0 and e90 < 11.5 and e60 > 2.0 and e60 < 3.5, "%.1f / %.1f kg/(m²·h)" % [e90, e60])
	if not ("H12" in HOOKS.APPLIED):
		T.chk("FP háček H12 není vložený – test odparu ve vířivce přeskočen", true); return
	var res := []
	for k in [0.0, 0.1, 1.0]:
		T.fresh(); var S = sim.S; S.cfg["evapOpen"] = k
		var w = S.wp; w.w = 3345.0; w.f = 487.0; w.T = 97.0
		var w0: float = w.w; var ev0: float = float(S.get("evapTot", 0.0))
		for i in 1200: sim.sim_step(1.0)
		res.append([w0 - w.w, 97.0 - w.T, float(S.get("evapTot", 0.0)) - ev0])
	var ok: bool = res[0][0] < 1e-6 and res[1][0] > 1.0 and res[1][0] < 12.0 and res[2][0] > 5.0 * res[1][0] and res[1][1] > res[0][1] and res[2][1] > res[1][1] + 1.0
	var bal: bool = absf(res[1][0] - res[1][2]) < 1e-6 and absf(res[2][0] - res[2][2]) < 1e-6
	T.fresh(); var S5 = sim.S; S5.cfg["evapOpen"] = 1.0
	var kv = S5.rvp; kv.w = 3000.0; kv.T = 90.0; kv.Tsens = 90.0; kv.heat = 0.0
	var kw0: float = kv.w
	for i in 600: sim.sim_step(1.0)
	T.chk("FP odpar: i pánev s horkou vodou bez páry ztrácí vodu odparem z hladiny (otevřená, 90 °C, 10 min)", kw0 - kv.w > 1.0 and float(S5.get("evapPre", 0.0)) > 1.0, "%.1f L" % (kw0 - kv.w))
	T.chk("FP odpar: vířivka 38 hl, 97 °C, 20 min klidu – zavřená (0,1) ztratí pár litrů, s otevřeným průlezem (1) zhruba desetkrát víc a víc vychladne; odpařená voda je v bilanci vody", ok and bal, "zavřená %.1f L / −%.1f K, otevřená %.1f L / −%.1f K, bez odparu −%.1f K" % [res[1][0], res[1][1], res[2][0], res[2][1], res[0][1]])

# ---------- rychlost rmutu v potrubí ----------
func _mashv() -> void:
	T.fresh(); var S = sim.S
	var path = [{"e": {"dn": 50.0}, "to": "x"}]
	var mash = sim.lp(); mash.w = 100.0; mash.g = 12.0; mash.su = 30.0; mash.T = 63.0
	S["gel"] = 0.0
	FP.transfer_check(sim, "p1", "rvp", mash, 300.0, path, 1.0)
	var g_ok: float = float(S.gel); var v_ok: float = float(S.p1.get("vpipe", 0.0))
	FP.transfer_check(sim, "p1", "rvp", mash, 600.0, path, 1.0)
	var g_fast: float = float(S.gel); var v_fast: float = float(S.p1.get("vpipe", 0.0))
	var water = sim.lp(); water.w = 100.0; water.T = 80.0
	FP.transfer_check(sim, "p1", "hlt", water, 900.0, path, 1.0)
	var warn: bool = S.log.any(func(l): return l.msg.contains("rmut teče potrubím"))
	T.chk("FP rmut v potrubí: do 1,5 m/s bez následků, rychleji smyk zhorší scezování a trenažér varuje (Kunze s. 903); vody se to netýká", g_ok == 0.0 and v_ok < 1.5 and g_fast > 0.0 and v_fast > 1.5 and float(S.gel) == g_fast and warn, "Ø %d mm: 300 L/min = %.2f m/s, 600 L/min = %.2f m/s, gel %.3f" % [roundi(sim.bore_mm(50.0)), v_ok, v_fast, g_fast])
	# v simulaci: C1 čerpá rmut z pánve do scezovací kádě – háček zapisuje rychlost v potrubí
	T.fresh(); S = sim.S
	if not _open_route("rvp", "sk", ["p1"]):
		T.chk("FP rmut v potrubí: trasa pánev → SK v plánku není – test v simulaci přeskočen", true); return
	if not ("H13" in HOOKS.APPLIED):
		T.chk("FP háček H13 není vložený – test rychlosti rmutu v simulaci přeskočen", true); return
	S.rvp.w = 2000.0; S.rvp.g = 130.0; S.rvp.f = 380.0; S.rvp.T = 63.0; S.rvp.Tsens = 63.0; S.rvp.stir = true
	S.p1.flow = 250.0; S.p1.on = true; S.p1.prime = 1.0
	for i in 30: sim.sim_step(1.0)
	var vp: float = float(S.p1.get("vpipe", 0.0)); var qq: float = float(S.p1.get("q", 0.0))
	var d_ref: float = sim.bore_mm(50.0) / 1000.0
	T.chk("FP rmut v potrubí: v simulaci se u běžícího C1 počítá rychlost rmutu v potrubí (průtok / průřez nejužší trubky)", qq > 20.0 and vp > 0.0 and vp >= qq / 60000.0 / (PI * d_ref * d_ref / 4.0) * 0.5, "%.0f L/min → %.2f m/s" % [qq, vp])
