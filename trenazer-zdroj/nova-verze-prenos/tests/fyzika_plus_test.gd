extends SceneTree
## FYZIKA+ (scripts/fyzika_plus.gd): tepelné ztráty izolovaných nádob podle výpočtu a odpar z horké hladiny mimo var.

const FP := preload("res://scripts/fyzika_plus.gd")
var fails: int = 0


func check(name: String, ok: bool, detail: String = "") -> void:
	print("%s  %s %s" % ["OK  " if ok else "CHYBA", name, detail])
	if not ok:
		fails += 1


func _init() -> void:
	# odpar z volné hladiny (ASHRAE): při 90 °C ~10 kg/(m²·h), při 60 °C zhruba čtvrtina
	var e90: float = FP.evap_open(90.0, 20.0)
	var e60: float = FP.evap_open(60.0, 20.0)
	check("odpar z volné hladiny 90 °C ~10 kg/(m²·h), 60 °C čtvrtina", e90 > 9.0 and e90 < 11.5 and e60 > 2.0 and e60 < 3.5, "(%.1f / %.1f)" % [e90, e60])
	# ztráty podle zaplnění: plná 1, poloprázdná o málo míň (izolovaná nádoba)
	var k1: float = FP.loss_k("rvp", 5000.0, 5000.0)
	var k5: float = FP.loss_k("rvp", 5000.0, 2500.0)
	check("ztráty podle zaplnění: plná 1,00, z poloviny plná 0,97–1,00", absf(k1 - 1.0) < 1e-6 and k5 > 0.97 and k5 < 1.0, "(%.3f / %.3f)" % [k1, k5])
	# vířivka 38 hl, 98 °C: po 20 min klidu přes 96,5 °C, po 80 min přes 94 °C (knihy: na chladič 95–97 °C)
	var s := BrewSim.new()
	var wp: Dictionary = s.vessels["wp"]
	wp["v"] = 3800.0; wp["t"] = 98.0; wp["ext"] = 480.0
	for i in 1200:
		s.step(1.0)
	var t20: float = float(wp["t"])
	for i in 3600:
		s.step(1.0)
	var t80: float = float(wp["t"])
	check("vířivka 38 hl z 98 °C: po 20 min přes 96,5 °C, po 80 min 94–97 °C", t20 > 96.5 and t20 < 98.0 and t80 > 94.0 and t80 < 97.0, "(%.1f / %.1f °C, odpar %.1f L)" % [t20, t80, s.evap_idle_l])
	# energie: úbytek tepla = ztráty stěnami + výparné teplo odpařené vody (bilance na 2 %)
	var s2 := BrewSim.new()
	var rv: Dictionary = s2.vessels["rvp"]
	rv["v"] = 4000.0; rv["t"] = 90.0
	var c0: float = 4000.0 * s2.C_WATER + float(rv["steel"]) * s2.STEEL_C
	var q_wall := 0.0
	for i in 1800:
		q_wall += float(FP.UA_FULL["rvp"]) * FP.loss_k("rvp", 5000.0, float(rv["v"])) * (float(rv["t"]) - s2.cal_ambient)
		s2.step(1.0)
	var lost: float = c0 * (90.0 - float(rv["t"]))
	var want: float = q_wall + s2.evap_idle_l * s2.L_VAP
	check("pánev 40 hl 90 °C, 30 min: úbytek tepla = ztráty stěnami + výparné teplo", absf(lost - want) < 0.03 * want, "(%.0f / %.0f kJ, −%.2f K, odpar %.1f L)" % [lost, want, 90.0 - float(rv["t"]), s2.evap_idle_l])
	# bojler a ledovka se neodpařují (zavřené nádoby), jen pomalu ztrácejí
	var s3 := BrewSim.new()
	var h: Dictionary = s3.vessels["hlt"]
	h["v"] = 4500.0; h["t"] = 80.0
	for i in 3600:
		s3.step(1.0)
	check("bojler 45 hl 80 °C za hodinu ztratí pod 0,5 K a nic se neodpaří", 80.0 - float(h["t"]) < 0.5 and 80.0 - float(h["t"]) > 0.05 and absf(float(h["v"]) - 4500.0) < 1e-6, "(−%.2f K)" % (80.0 - float(h["t"])))
	# stojící trubka: 90 °C voda v trubce po 1,5 h zchladne asi o 63 % rozdílu k hale (τ 1,5 h); prázdná trubka se nemění
	var s4 := BrewSim.new()
	s4.pipe_hold["tp"] = {"v": 5.0, "t": 90.0, "ext": 0.0, "malt": 0.0, "med": "water"}
	s4.pipe_hold["te"] = {"v": 0.0, "t": 90.0, "ext": 0.0, "malt": 0.0, "med": "empty"}
	for i in 5400:
		s4.step(1.0)
	var tp: float = float(s4.pipe_hold["tp"]["t"])
	check("stojící trubka z 90 °C: po 1,5 h 44–48 °C, prázdná beze změny", tp > 44.0 and tp < 48.0 and absf(float(s4.pipe_hold["te"]["t"]) - 90.0) < 1e-6, "(%.1f °C)" % tp)
	print("VÝSLEDEK: %s (chyb %d)" % ["VŠE V POŘÁDKU" if fails == 0 else "CHYBY", fails])
	quit(1 if fails > 0 else 0)
