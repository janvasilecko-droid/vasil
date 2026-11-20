extends SceneTree
## Kalibrace parametrů (sim.apply_calibration, kalibrace.json) a bilance tepla s ocelí nádob při přelévání rmutu (dekokce).

var fails: int = 0


func check(name: String, ok: bool, detail: String = "") -> void:
	print("%s  %s %s" % ["OK  " if ok else "CHYBA", name, detail])
	if not ok:
		fails += 1


func _initialize() -> void:
	create_timer(60.0).timeout.connect(func() -> void: quit(1))
	var sim := BrewSim.new()
	var t0: float = float(sim.plate_cool(95.0, 200.0, true, 3.0)["t_out"])
	var msgs: Array = sim.apply_calibration({
		"UAice": {"value": 60.0, "source": "MEASURED"},
		"tbWater": 99.3,
		"boilerKW": 900,
		"boilerWaterKg": -5,
		"coolWaterT": "abc",
		"neznamy": 1.0,
		"p3_H0": {"value": 50.0},
		"p3_Qn": 5000.0,
		"p9_H0": 30.0,
	})
	for m in msgs:
		print("   ", m)
	check("UAice z kalibrace se použije", is_equal_approx(sim.cal_ua_ice, 60.0))
	check("změněné UAice zhorší chlazení", float(sim.plate_cool(95.0, 200.0, true, 3.0)["t_out"]) > t0 + 0.5)
	check("bod varu 99,3 °C se použije", is_equal_approx(sim.cal_boil_t, 99.3))
	check("výkon kotle 900 kW se použije", is_equal_approx(sim.cal_boiler_kw, 900.0))
	check("záporná hmotnost vody v kotli se odmítne", is_equal_approx(sim.cal_boiler_water_kg, BrewSim.BOILER_WATER_KG))
	check("text místo čísla se odmítne", is_equal_approx(sim.cal_mains_t, BrewSim.MAINS_T))
	check("neznámý parametr se přeskočí s hlášením", msgs.any(func(m: String) -> bool: return m.contains("neznamy") and m.contains("neznámý")))
	check("křivka C3: H0 se použije, Qn mimo rozsah odmítnuto", is_equal_approx(float(sim.pump_cal.get("p3", {}).get("H0", 0.0)), 50.0) and not (sim.pump_cal.get("p3", {}) as Dictionary).has("Qn"))
	check("čerpadlo mimo seznam (p9) se přeskočí", not sim.pump_cal.has("p9"))
	# bod varu: nádoba se při ohřevu zastaví na kalibrované teplotě
	sim.vessels["rvp"]["v"] = 1000.0
	sim.vessels["rvp"]["t"] = 98.0
	sim.heater_kw["rvp"] = 330.0
	for i in 60:
		sim.step(2.0)
	check("var se ustálí na kalibrovaném bodu varu", absf(float(sim.vessels["rvp"]["t"]) - 99.3) < 0.1, "(%.2f °C)" % float(sim.vessels["rvp"]["t"]))

	# --- dekokce: bilance tepla s ocelí a šrotem ---
	var d := BrewSim.new()
	var rv: Dictionary = d.vessels["rvp"]
	rv["v"] = 1000.0
	rv["t"] = 50.0
	rv["malt"] = 300.0
	d._mix(rv, 500.0, 95.0, 0.0, {}, 100.0)
	var expect: float = (1000.0 * 4.186 + 300.0 * 1.7 + 450.0 * 0.5) * 50.0 + (500.0 * 4.186 + 100.0 * 1.7) * 95.0
	expect /= (1000.0 * 4.186 + 300.0 * 1.7 + 450.0 * 0.5) + (500.0 * 4.186 + 100.0 * 1.7)
	check("vrácený povařený díl: teplota podle bilance tepla (kapalina + šrot + ocel)", absf(float(rv["t"]) - expect) < 0.05, "(%.2f °C, čekáno %.2f; jen podle objemu by to bylo 65,0)" % [float(rv["t"]), expect])
	var e := BrewSim.new()
	var ev: Dictionary = e.vessels["rvp"]
	ev["v"] = 0.0
	e._mix(ev, 20.0, 79.0, 0.0)
	check("první litry do studené prázdné varny ocel ochladí", float(ev["t"]) < 60.0, "(20 L z 79 °C → %.1f °C)" % float(ev["t"]))
	e._mix(ev, 2000.0, 79.0, 0.0)
	check("a velký objem se od oceli skoro nezmění", float(ev["t"]) > 77.0, "(%.1f °C)" % float(ev["t"]))
	print("VÝSLEDEK: %s (chyb %d)" % ["VŠE V POŘÁDKU" if fails == 0 else "SELHÁNÍ", fails])
	quit(1 if fails > 0 else 0)
