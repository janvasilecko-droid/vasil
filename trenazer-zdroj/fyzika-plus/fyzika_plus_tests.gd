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
	T.fresh()

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
	T.fresh(); var S = sim.S; var c = S.cfg
	var v = S.rvp; v.w = 2000.0; v.T = 60.0; v.Tsens = 60.0; v.Twall = 60.0; v.stir = true; v.hz = 35.0; v.heat = 1.0
	sim.sim_step(1.0); sim.sim_step(1.0)
	var wet: float = float(S._pd.rvp.get("wet", 1.0))
	var old_P: float = c.rvpUAst * wet * sim.film_factor(v) * (sim.tsat(S.boiler.p + 1.013) - v.T)
	T.chk("FP pára: plně otevřený ventil dá 85–100 % výkonu dřívějšího modelu (kalibrace rvpUAst platí dál)", v.Pin > 0.85 * old_P and v.Pin <= old_P * 1.001, "%.0f kW vs %.0f kW, plášť %.2f bar abs" % [v.Pin, old_P, float(v.get("jp", 0.0))])
	# B) ekviprocentní ventil: 25 % zdvihu = malý výkon (jemná regulace prodlev); plný výkon se měří zvlášť (bez vzduchu v plášti)
	var P_full: float = v.Pin
	T.fresh(); S = sim.S; c = S.cfg; c["stValveR"] = 30.0
	v = S.rvp; v.w = 2000.0; v.T = 60.0; v.Tsens = 60.0; v.Twall = 60.0; v.stir = true; v.hz = 35.0; v.heat = 0.25
	sim.sim_step(1.0); sim.sim_step(1.0)
	var P25: float = v.Pin
	T.chk("FP pára: ekviprocentní ventil (R 30) při 25 % zdvihu dá pod 15 % plného výkonu", P25 > 0.0 and P25 < 0.15 * P_full, "%.0f kW vs %.0f kW" % [P25, P_full])
	# C) stall: přiškrcená pára na chladný rmut → plášť pod protitlakem kondenzátu se zaplavuje; po otevření kondenzát odteče
	T.fresh(); S = sim.S; c = S.cfg
	v = S.rvp; v.w = 2500.0; v.T = 45.0; v.Tsens = 45.0; v.stir = true; v.hz = 35.0; v.heat = 0.2
	for i in 600: sim.sim_step(1.0)
	var fl1: float = float(v.get("jfl", 0.0)); var pj1: float = float(v.get("jp", 9.0))
	v.heat = 1.0
	for i in 300: sim.sim_step(1.0)
	var fl2: float = float(v.get("jfl", 0.0))
	T.chk("FP stall: přiškrcená pára (20 %) na chladnou náplň → tlak v plášti pod protitlakem kondenzátu, plášť se zaplavuje; po plném otevření kondenzát odteče", pj1 < 1.013 + float(c.get("stCondBack", 0.15)) and fl1 > 0.01 and fl2 < fl1 * 0.5, "p %.2f bar abs, zaplavení %.1f %% → %.1f %%" % [pj1, fl1 * 100.0, fl2 * 100.0])
	# D) podtlak po zavření páry: bez zavzdušňovacího ventilu varování, s ním vzduch v plášti
	T.fresh(); S = sim.S; c = S.cfg; c["jVacBreaker"] = 0.0
	v = S.rvp; v.w = 2500.0; v.T = 63.0; v.Tsens = 63.0; v.stir = true; v.hz = 35.0; v.heat = 1.0
	for i in 30: sim.sim_step(1.0)
	v.heat = 0.0
	for i in 30: sim.sim_step(1.0)
	var vac_msg: bool = S.log.any(func(l): return l.msg.contains("zborcení"))
	T.chk("FP podtlak: bez zavzdušňovacího ventilu spadne plášť po zavření páry pod 0,5 bar abs. a trenažér varuje (Kunze s. 290–291)", float(v.get("jp", 9.0)) < 0.5 and vac_msg, "%.2f bar abs" % float(v.get("jp", 9.0)))
	T.fresh(); S = sim.S
	v = S.rvp; v.w = 2500.0; v.T = 63.0; v.Tsens = 63.0; v.stir = true; v.hz = 35.0; v.heat = 1.0
	for i in 30: sim.sim_step(1.0)
	v.heat = 0.0
	for i in 120: sim.sim_step(1.0)
	T.chk("FP podtlak: se zavzdušňovacím ventilem nasaje plášť vzduch, tlak se vrátí na ~1 bar abs.", float(v.get("jy", 0.0)) > 0.5 and float(v.get("jp", 0.0)) > 0.98, "vzduch %.2f, %.2f bar abs" % [float(v.get("jy", 0.0)), float(v.get("jp", 0.0))])
	# E) zavzdušněný plášť topí nejdřív slabě, odvzdušňovač ho do pár minut vyčistí
	T.fresh(); S = sim.S
	v = S.rvp; v.w = 2500.0; v.T = 50.0; v.Tsens = 50.0; v.stir = true; v.hz = 35.0; v["jy"] = 1.0; v.heat = 1.0
	sim.sim_step(1.0); sim.sim_step(1.0)
	var P0: float = v.Pin
	for i in 180: sim.sim_step(1.0)
	T.chk("FP vzduch: zavzdušněný plášť topí nejdřív slabě, po odvzdušnění naplno", P0 < 0.3 * v.Pin and float(v.get("jy", 1.0)) < 0.02, "%.0f → %.0f kW, vzduch %.3f" % [P0, v.Pin, float(v.get("jy", 1.0))])
	# F) 1. zákon: energie páry = ohřev vody a oceli + ztráty (i s ventilem a pláštěm)
	T.fresh(); S = sim.S; c = S.cfg
	v = S.rvp; v.w = 2000.0; v.T = 20.0; v.Tsens = 20.0; v.stir = true; v.heat = 0.6
	var H0: float = (sim.cC(v) + sim.steelC(c.rvpSteel)) * v.T
	var Ein := 0.0; var Eloss := 0.0
	for i in 900:
		sim.sim_step(1.0)
		Ein += v.Pin; Eloss += c.rvpUA * (v.T - c.ambientT)
	var H1: float = (sim.cC(v) + sim.steelC(c.rvpSteel)) * v.T
	T.chk("FP pára: energie páry = ohřev náplně a oceli + ztráty (1. zákon, chyba < 2 %)", absf((Ein - Eloss) - (H1 - H0)) < 0.02 * maxf(1.0, Ein), "dodáno %.0f MJ" % (Ein / 1000.0))
	# G) zavřená parní trasa v plánku → pára nejde, kotel nic nevyrábí, plášť nic neukazuje
	T.fresh(); S = sim.S
	v = S.rvp; v.w = 2000.0; v.T = 60.0; v.stir = true; v.heat = 1.0
	S["steamOK"] = {"rvp": false, "mp": true, "hlt": true}
	var Pz: Dictionary = FP.steam_pd(sim, v, S.cfg.rvpUAst, S.cfg.rvpJacketL, 139.0)
	T.chk("FP pára: zavřená parní trasa = žádná pára ani zátěž kotle", Pz.P == 0.0 and float(v.get("jkgs", 1.0)) == 0.0)
	# H) poddimenzovaný odvaděč kondenzátu: plášť se zaplavuje i při přetlaku
	T.fresh(); S = sim.S; c = S.cfg; c["jDrainKgS"] = 0.05
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
	T.fresh(); var S = sim.S
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
