extends SceneTree
## Snímek okna Grafy z nasimulovaného varu (xvfb): Godot --path . --script res://tests/grafy_shot.gd
func _init() -> void:
	root.size = Vector2i(1200, 760)
	var g = load("res://scripts/grafy_okno.gd").new()
	var s = load("res://scripts/sim.gd").new()
	g.sim = s
	g.visible = true
	root.add_child(g)
	g.anchor_left = 0.5
	g.anchor_right = 0.5
	g.anchor_top = 0.5
	g.anchor_bottom = 0.5
	g.offset_left = -520.0
	g.offset_right = 520.0
	g.offset_top = -340.0
	g.offset_bottom = 340.0
	var box := VBoxContainer.new()
	g.add_child(box)
	g._info = Label.new()
	box.add_child(g._info)
	g._plot = Control.new()
	g._plot.custom_minimum_size = Vector2(1000, 560)
	g._plot.draw.connect(g._draw_plots)
	box.add_child(g._plot)
	g._reset()
	var rv: Dictionary = s.vessels["rvp"]
	rv["v"] = 4000.0
	rv["t"] = 70.0
	rv["ext"] = 480.0
	rv["hops"] = [{"g": 5000.0, "a": 10.0, "t0": 1800.0}]
	s.mash_ph = 5.45
	s.heater_kw["rvp"] = 400.0
	s.steam_on = true
	for i in 7000:
		g._on_step(1.0)
		s.step(1.0)
	await process_frame
	await process_frame
	g._process(1.0)
	await process_frame
	await process_frame
	root.get_texture().get_image().save_png("/tmp/grafy.png")
	quit()
