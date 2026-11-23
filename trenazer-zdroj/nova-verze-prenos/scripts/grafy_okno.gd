class_name GrafyOkno
extends PanelContainer
## Živé grafy várky: teploty nádob, °P, objem, hořkost (IBU) a DMS, pH sladiny a barva mladiny v čase.
## Vzorek se ukládá každých 20 s herního času (i se zavřeným oknem); tlačítko ULOŽIT CSV zapíše celý průběh do user://.
## pH a barva jsou obecné literární modely (FYZIKA+, ODHAD pro Zajíce), IBU je Tinseth, DMS poločas 20 min.

const FyzikaPlus := preload("res://scripts/fyzika_plus.gd")
const SAMPLE_S := 20.0
const MAX_PTS := 6000

var sp: SimPanel
var rt: Variant
var sim: BrewSim
var _plot: Control
var _info: Label
var _acc: float = 0.0
var _last_clock: float = -1.0
var _next_sample: float = 0.0
var _ts: PackedFloat32Array = PackedFloat32Array()
var _cols: Dictionary = {}  # název řady -> PackedFloat32Array (NAN = bez hodnoty)
var _hold: Dictionary = {}

## [nadpis, jednotka, [[řada, barva, popisek]...]]
const CHARTS := [
	["Teploty", "°C", [["t_rvp", Color(1.0, 0.45, 0.2), "varna"], ["t_sk", Color(0.95, 0.85, 0.3), "SK"], ["t_wp", Color(0.4, 0.8, 1.0), "vířivka"], ["t_spilka", Color(0.5, 1.0, 0.6), "spilka"]]],
	["Hustota", "°P", [["p_rvp", Color(1.0, 0.7, 0.2), "varna"], ["p_wp", Color(0.4, 0.8, 1.0), "vířivka"], ["p_spilka", Color(0.5, 1.0, 0.6), "spilka"]]],
	["Hořkost a DMS", "IBU / ppb", [["ibu", Color(0.6, 0.9, 0.3), "IBU"], ["dms", Color(0.9, 0.5, 0.9), "DMS ppb"]]],
	["pH sladiny", "pH", [["ph", Color(0.4, 0.8, 1.0), "pH"]]],
	["Barva mladiny", "EBC", [["ebc", Color(0.95, 0.65, 0.15), "EBC"]]],
	["Objem", "L", [["v_rvp", Color(1.0, 0.45, 0.2), "varna"], ["v_wp", Color(0.4, 0.8, 1.0), "vířivka"], ["v_spilka", Color(0.5, 1.0, 0.6), "spilka"]]],
]


func setup(p_sp: SimPanel, p_rt: Variant) -> void:
	sp = p_sp
	rt = p_rt
	sim = sp.sim
	if rt.has_method("_step") and not (rt.step_hooks as Array).has(_on_step):
		(rt.step_hooks as Array).append(_on_step)
	visible = false
	anchor_left = 0.5
	anchor_right = 0.5
	anchor_top = 0.5
	anchor_bottom = 0.5
	offset_left = -520.0
	offset_right = 520.0
	offset_top = -340.0
	offset_bottom = 340.0
	var sb := StyleBoxFlat.new()
	sb.bg_color = Color(0.08, 0.09, 0.11, 0.97)
	sb.set_corner_radius_all(8)
	sb.set_content_margin_all(12)
	add_theme_stylebox_override("panel", sb)
	var box := VBoxContainer.new()
	box.add_theme_constant_override("separation", 6)
	add_child(box)
	var head := HBoxContainer.new()
	var title := Label.new()
	title.text = "GRAFY VÁRKY"
	title.add_theme_color_override("font_color", Color(0.95, 0.8, 0.25))
	title.add_theme_font_size_override("font_size", 18)
	title.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	head.add_child(title)
	var csv := Button.new()
	csv.text = "Uložit CSV"
	csv.focus_mode = Control.FOCUS_NONE
	csv.pressed.connect(_save_csv)
	head.add_child(csv)
	var close := Button.new()
	close.text = "✕"
	close.focus_mode = Control.FOCUS_NONE
	close.pressed.connect(func() -> void: visible = false)
	head.add_child(close)
	box.add_child(head)
	_info = Label.new()
	_info.add_theme_font_size_override("font_size", 13)
	_info.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	box.add_child(_info)
	_plot = Control.new()
	_plot.size_flags_vertical = Control.SIZE_EXPAND_FILL
	_plot.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	_plot.custom_minimum_size = Vector2(1000, 560)
	_plot.draw.connect(_draw_plots)
	box.add_child(_plot)
	_reset()


func _reset() -> void:
	_ts = PackedFloat32Array()
	_cols.clear()
	_hold.clear()
	for c in CHARTS:
		for s in c[2]:
			_cols[s[0]] = PackedFloat32Array()
	_next_sample = 0.0


func _on_step(_dt: float) -> void:
	var clock: float = sim.clock
	if _last_clock >= 0.0 and clock < _last_clock - 1.0:
		_reset()
	_last_clock = clock
	if clock < _next_sample:
		return
	_next_sample = clock + SAMPLE_S
	_sample(clock)


func _vessel_val(key: String, field: String) -> float:
	var vs: Dictionary = sim.vessels[key]
	if float(vs["v"]) < 5.0:
		return NAN
	return float(vs[field])


func _held(name: String, v: float) -> float:
	# po vyprázdnění nádoby zůstane poslední hodnota (sladina putuje dál)
	if is_nan(v):
		return float(_hold.get(name, NAN))
	_hold[name] = v
	return v


func _sample(clock: float) -> void:
	if _ts.size() >= MAX_PTS:
		return
	var rv: Dictionary = sim.vessels["rvp"]
	var boil_s: float = float(rv.get("boil_s", 0.0))
	var wort: bool = boil_s > 0.0 or float(rv["v"]) >= 5.0 and float(rv["malt"]) * sim.MALT_L_PER_KG <= 0.05 * float(rv["v"]) and float(rv["t"]) > 60.0
	var vals := {
		"t_rvp": _vessel_val("rvp", "t"), "t_sk": _vessel_val("sk", "t"), "t_wp": _vessel_val("wp", "t"), "t_spilka": _vessel_val("spilka", "t"),
		"p_rvp": sim.plato("rvp") if float(rv["v"]) >= 5.0 else NAN,
		"p_wp": sim.plato("wp") if float(sim.vessels["wp"]["v"]) >= 5.0 else NAN,
		"p_spilka": sim.plato("spilka") if float(sim.vessels["spilka"]["v"]) >= 5.0 else NAN,
		"v_rvp": float(rv["v"]), "v_wp": float(sim.vessels["wp"]["v"]), "v_spilka": float(sim.vessels["spilka"]["v"]),
		"ibu": _held("ibu", sim.ibu() if (float(rv["v"]) >= 5.0 and rv.has("hops")) else NAN),
		"dms": _held("dms", sim.dms_ppb() if boil_s > 0.0 else NAN),
		"ph": _held("ph", FyzikaPlus.wort_ph(sim.mash_ph, boil_s) if wort else (sim.mash_ph if float(rv["malt"]) > 1.0 else NAN)),
		"ebc": _held("ebc", sim.recipe_ebc + sim.wort_col if wort else NAN),
	}
	_ts.append(clock)
	for k in vals:
		var arr: PackedFloat32Array = _cols[k]
		arr.append(float(vals[k]))
		_cols[k] = arr


func _process(delta: float) -> void:
	if sim == null or not visible:
		return
	_acc += delta
	if _acc < 0.5:
		return
	_acc = 0.0
	var n: int = _ts.size()
	var txt := "Čas %s · vzorků %d. " % [_hms(sim.clock), n]
	if n > 0:
		for c in CHARTS:
			for s in c[2]:
				var a: PackedFloat32Array = _cols[s[0]]
				if not is_nan(a[n - 1]):
					txt += "%s %.1f · " % [s[2] if (c[0] as String).begins_with("p") or c[2].size() > 1 else c[0], a[n - 1]]
	_info.text = txt.trim_suffix(" · ")
	_plot.queue_redraw()


func _hms(s: float) -> String:
	return "%d:%02d" % [int(s / 3600.0), int(fmod(s, 3600.0) / 60.0)]


func _draw_plots() -> void:
	var font: Font = ThemeDB.fallback_font
	var w: float = _plot.size.x
	var h: float = _plot.size.y
	var cw: float = w / 2.0
	var ch: float = h / 3.0
	var tmax: float = maxf(float(_ts[_ts.size() - 1]) if _ts.size() > 0 else 0.0, 600.0)
	for i in CHARTS.size():
		var c: Array = CHARTS[i]
		var org := Vector2((i % 2) * cw + 46.0, (i / 2) * ch + 22.0)
		var sz := Vector2(cw - 60.0, ch - 46.0)
		_plot.draw_rect(Rect2(org, sz), Color(0.12, 0.13, 0.16), true)
		_plot.draw_string(font, org + Vector2(0, -6), "%s [%s]" % [c[0], c[1]], HORIZONTAL_ALIGNMENT_LEFT, -1, 13, Color(0.9, 0.9, 0.9))
		var lo: float = INF
		var hi: float = -INF
		for s in c[2]:
			for v in (_cols[s[0]] as PackedFloat32Array):
				if not is_nan(v):
					lo = minf(lo, v)
					hi = maxf(hi, v)
		if lo == INF:
			_plot.draw_string(font, org + Vector2(8, 20), "zatím bez dat", HORIZONTAL_ALIGNMENT_LEFT, -1, 12, Color(0.6, 0.6, 0.6))
			continue
		var pad: float = maxf((hi - lo) * 0.1, 0.05 if c[1] == "pH" else 1.0)
		lo -= pad
		hi += pad
		if c[1] != "°C" and c[1] != "pH" and lo < 0.0 and lo > -pad - 0.001:
			lo = 0.0
		for g in 5:
			var gy: float = org.y + sz.y * g / 4.0
			_plot.draw_line(Vector2(org.x, gy), Vector2(org.x + sz.x, gy), Color(1, 1, 1, 0.08))
			var val: float = hi - (hi - lo) * g / 4.0
			_plot.draw_string(font, Vector2(org.x - 44.0, gy + 4.0), "%.2f" % val if c[1] == "pH" else "%.0f" % val, HORIZONTAL_ALIGNMENT_RIGHT, 40, 10, Color(0.7, 0.7, 0.7))
		for g in 5:
			var gx: float = org.x + sz.x * g / 4.0
			_plot.draw_string(font, Vector2(gx - 14.0, org.y + sz.y + 13.0), _hms(tmax * g / 4.0), HORIZONTAL_ALIGNMENT_LEFT, -1, 10, Color(0.7, 0.7, 0.7))
		var lx: float = org.x + 6.0
		for s in c[2]:
			var a: PackedFloat32Array = _cols[s[0]]
			var prev := Vector2.INF
			for k in _ts.size():
				var v: float = a[k]
				if is_nan(v):
					prev = Vector2.INF
					continue
				var p := Vector2(org.x + sz.x * float(_ts[k]) / tmax, org.y + sz.y * (1.0 - (v - lo) / (hi - lo)))
				if prev != Vector2.INF:
					_plot.draw_line(prev, p, s[1], 1.6)
				prev = p
			_plot.draw_rect(Rect2(lx, org.y + sz.y + 18.0, 10, 4), s[1], true)
			_plot.draw_string(font, Vector2(lx + 14.0, org.y + sz.y + 23.0), s[2], HORIZONTAL_ALIGNMENT_LEFT, -1, 10, Color(0.8, 0.8, 0.8))
			lx += 80.0


func _save_csv() -> void:
	var path := "user://varna_prubeh.csv"
	var f := FileAccess.open(path, FileAccess.WRITE)
	if f == null:
		_info.text = "CSV se nepodařilo zapsat."
		return
	var names: Array = _cols.keys()
	f.store_line("cas_s;" + ";".join(names))
	for k in _ts.size():
		var row: Array = ["%.0f" % _ts[k]]
		for n in names:
			var v: float = (_cols[n] as PackedFloat32Array)[k]
			row.append("" if is_nan(v) else ("%.3f" % v).replace(".", ","))
		f.store_line(";".join(row))
	f.close()
	_info.text = "Průběh uložen: %s" % ProjectSettings.globalize_path(path)
