extends SceneTree
## Grafy várky: vzorkování pH, IBU, barvy a DMS při varu a zápis CSV.
func _init() -> void:
	var g = load("res://scripts/grafy_okno.gd").new()
	var s = load("res://scripts/sim.gd").new()
	g.sim = s
	g._info = Label.new()
	g._reset()
	var rv: Dictionary = s.vessels["rvp"]
	rv["v"] = 4000.0
	rv["t"] = 100.0
	rv["ext"] = 480.0
	rv["boil_s"] = 0.0
	rv["hops"] = [{"g": 5000.0, "a": 10.0, "t0": 0.0}]
	s.mash_ph = 5.45
	s.heater_kw["rvp"] = 400.0
	for i in 5400:
		rv["boil_s"] = float(rv["boil_s"]) + 1.0
		s.clock += 1.0
		g._on_step(1.0)
		s.step(1.0)
	var n: int = g._ts.size()
	print("n=", n, " cols=", (g._cols["ph"] as PackedFloat32Array).size(), " clock=", s.clock)
	var ph: PackedFloat32Array = g._cols["ph"]
	var ebc: PackedFloat32Array = g._cols["ebc"]
	var ibu: PackedFloat32Array = g._cols["ibu"]
	var dms: PackedFloat32Array = g._cols["dms"]
	print("vzorků %d, pH %.2f→%.2f, EBC %.1f→%.1f, IBU %.0f, DMS %.0f" % [n, ph[0], ph[n - 1], ebc[0], ebc[n - 1], ibu[n - 1], dms[n - 1]])
	var ok: bool = n > 200 and ph[n - 1] < ph[0] and ebc[n - 1] > ebc[0] and ibu[n - 1] > 5.0 and dms[n - 1] < dms[0] * 0.2
	g._save_csv()
	ok = ok and FileAccess.file_exists("user://varna_prubeh.csv")
	print("VÝSLEDEK: ", "VŠE V POŘÁDKU" if ok else "CHYBA")
	quit()
