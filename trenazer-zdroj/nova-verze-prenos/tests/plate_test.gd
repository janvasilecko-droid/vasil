extends SceneTree
## Deskový chladič, jeden stupeň s ledovou vodou (sim.plate_cool): teplota za chladičem podle průtoku mladiny, ledové vody a zanesení.

var fails: int = 0


func check(name: String, ok: bool, detail: String = "") -> void:
	print("%s  %s %s" % ["OK  " if ok else "CHYBA", name, detail])
	if not ok:
		fails += 1


func _initialize() -> void:
	create_timer(60.0).timeout.connect(func() -> void: quit(1))
	var sim := BrewSim.new()
	var ref: Dictionary = sim.plate_cool(95.0, 200.0, true, 3.0)
	var t: float = float(ref["t_out"])
	check("referenční spílání: 95 °C → ~8,5 °C (C3 200 L/min, ledová 3 °C)", absf(t - 8.5) < 0.7, "(%.2f °C, ledová voda se vrací s %.1f °C, výkon %.0f kW)" % [t, float(ref["t_ice_out"]), float(ref["duty_kw"])])
	var slow: float = float(sim.plate_cool(95.0, 100.0, true, 3.0)["t_out"])
	var fast: float = float(sim.plate_cool(95.0, 400.0, true, 3.0)["t_out"])
	check("pomalejší průtok chladí víc, rychlejší míň", slow < t and t < fast, "(100 L/min %.1f · 200 %.1f · 400 %.1f °C)" % [slow, t, fast])
	var water: float = float(sim.plate_cool(95.0, 200.0, false, 3.0)["t_out"])
	check("bez ledové vody se mladina nechladí (voda z řadu do chladiče nejde)", absf(water - 95.0) < 1e-6, "(%.1f °C)" % water)
	sim.plate_eff = BrewSim.PLATE_EFF * 0.45
	var dirty: float = float(sim.plate_cool(95.0, 200.0, true, 3.0)["t_out"])
	check("zanesený chladič chladí hůř", dirty > t + 3.0, "(%.1f °C místo %.1f °C)" % [dirty, t])
	sim.plate_eff = BrewSim.PLATE_EFF
	var warm: float = float(sim.plate_cool(95.0, 200.0, true, 8.0)["t_out"])
	check("teplejší ledová voda = teplejší mladina", warm > t + 2.0, "(%.1f °C)" % warm)
	var cold_in: float = float(sim.plate_cool(40.0, 200.0, true, 3.0)["t_out"])
	check("chladnější vstup nikdy neohřeje mladinu", cold_in <= 40.0 and cold_in >= 3.0, "(%.1f °C)" % cold_in)
	print("VÝSLEDEK: %s (chyb %d)" % ["VŠE V POŘÁDKU" if fails == 0 else "SELHÁNÍ", fails])
	quit(1 if fails > 0 else 0)
