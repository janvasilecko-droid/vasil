extends CanvasLayer
## „Herní“ vrstva: úkol (quest tracker), oznámení, nápověda kláves, zvuky, vinětace,
## zvýraznění věci pod myší s popiskem a vlnka po kliknutí.

const Art = preload("res://art.gd")
const YEL: = Color("f2c230")

var m
var quest: PanelContainer
var q_step: Label
var q_title: Label
var q_hint: Label
var q_detail: Label
var q_bar: ProgressBar
var q_btn: Button
var q_body: VBoxContainer
var feed: VBoxContainer
var tip: PanelContainer
var tip_lbl: Label
var help: PanelContainer
var hover: HoverLayer
var last_log: = -1.0
var t_acc: = 0.0
var muted: = false
var snd: = {}
var hum: AudioStreamPlayer
var vign: ColorRect

func _ready() -> void :
	layer = 2
	#_vignette()
	_quest()
	feed = VBoxContainer.new();feed.add_theme_constant_override("separation", 6)
	feed.anchor_left = 0.5;feed.anchor_right = 0.5;feed.offset_left = -260;feed.offset_right = 260;feed.offset_top = 62
	feed.mouse_filter = Control.MOUSE_FILTER_IGNORE;add_child(feed)
	_tip();_help();_sounds()
	hover = HoverLayer.new();hover.g = self;hover.m = m;hover.z_index = 50;m.add_child(hover)
	if FileAccess.file_exists("user://zvuk.txt"): muted = FileAccess.open("user://zvuk.txt", FileAccess.READ).get_as_text().strip_edges() == "vyp"
	if m.sim.S.log.size(): last_log = m.sim.S.log[0].t
	get_tree().create_timer(1.5).timeout.connect(func(): notify("Ovládání jako ve hře: F1 = přehled kláves, Mezerník = čas, 1–4 = rychlost, Enter = potvrdit úkol", "info"))

# ---------- vinětace (tmavé okraje jako ve hře) ----------
func _vignette() -> void :
	var cl: = CanvasLayer.new();cl.layer = 0;m.add_child.call_deferred(cl)
	vign = ColorRect.new();vign.set_anchors_preset(Control.PRESET_FULL_RECT);vign.mouse_filter = Control.MOUSE_FILTER_IGNORE
	var sh: = Shader.new()
	sh.code = "shader_type canvas_item;\nvoid fragment(){ vec2 d = UV - vec2(0.5); d.x *= 1.25; float v = smoothstep(0.42, 0.9, length(d)); COLOR = vec4(0.0, 0.0, 0.0, v * 0.55); }"
	var mat: = ShaderMaterial.new();mat.shader = sh;vign.material = mat
	cl.add_child(vign)

func _card_sb(accent: Color = YEL) -> StyleBoxFlat:
	var sb: = StyleBoxFlat.new();sb.bg_color = Color(0.05, 0.065, 0.08, 0.9);sb.set_corner_radius_all(12)
	sb.border_color = Color(accent, 0.55);sb.set_border_width_all(1);sb.border_width_left = 4
	sb.shadow_color = Color(0, 0, 0, 0.45);sb.shadow_size = 10;sb.shadow_offset = Vector2(0, 4)
	sb.content_margin_left = 14;sb.content_margin_right = 14;sb.content_margin_top = 10;sb.content_margin_bottom = 12
	return sb

func _lbl(t: String, size: int, col: Color, bold: = true) -> Label:
	var l: = Label.new();l.text = t;l.add_theme_font_override("font", m.font_b if bold else m.font)
	l.add_theme_font_size_override("font_size", size);l.add_theme_color_override("font_color", col)
	return l

# ---------- úkol ----------
func _quest() -> void :
	quest = PanelContainer.new();quest.add_theme_stylebox_override("panel", _card_sb())
	quest.position = Vector2(12, 60);quest.scale = Vector2(0.76, 0.76);quest.custom_minimum_size = Vector2(380, 0)
	add_child(quest)
	var v: = VBoxContainer.new();v.add_theme_constant_override("separation", 4);quest.add_child(v)
	var top: = HBoxContainer.new();v.add_child(top)
	var ic: = _lbl("◆ ÚKOL", 12, YEL);top.add_child(ic)
	q_step = _lbl("", 12, Color("8a939b"));q_step.size_flags_horizontal = Control.SIZE_EXPAND_FILL;q_step.horizontal_alignment = HORIZONTAL_ALIGNMENT_RIGHT;top.add_child(q_step)
	var hide: = Button.new();hide.text = "–";hide.flat = true;hide.focus_mode = Control.FOCUS_NONE;hide.add_theme_color_override("font_color", Color("8a939b"))
	hide.tooltip_text = "Sbalit / rozbalit úkol (Q)";hide.pressed.connect(toggle_quest);top.add_child(hide)
	q_title = _lbl("", 17, Color("f5f7f9"));q_title.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART;q_title.custom_minimum_size = Vector2(350, 0);v.add_child(q_title)
	q_body = VBoxContainer.new();q_body.add_theme_constant_override("separation", 6);v.add_child(q_body)
	q_hint = _lbl("", 13, Color("c9d1d8"), false);q_hint.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART;q_hint.custom_minimum_size = Vector2(350, 0);q_body.add_child(q_hint)
	q_detail = _lbl("", 12, YEL, false);q_detail.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART;q_detail.custom_minimum_size = Vector2(350, 0);q_body.add_child(q_detail)
	q_bar = ProgressBar.new();q_bar.show_percentage = false;q_bar.custom_minimum_size = Vector2(0, 6)
	var bg: = StyleBoxFlat.new();bg.bg_color = Color(1, 1, 1, 0.08);bg.set_corner_radius_all(3)
	var fg: = StyleBoxFlat.new();fg.bg_color = YEL;fg.set_corner_radius_all(3)
	q_bar.add_theme_stylebox_override("background", bg);q_bar.add_theme_stylebox_override("fill", fg);q_body.add_child(q_bar)
	q_btn = Button.new();q_btn.text = "✔  Potvrdit krok  (Enter)";q_btn.focus_mode = Control.FOCUS_NONE
	var bn: = StyleBoxFlat.new();bn.bg_color = Color("2f8f4e");bn.set_corner_radius_all(8);bn.content_margin_top = 6;bn.content_margin_bottom = 6
	var bh: = bn.duplicate();bh.bg_color = Color("39a85d")
	var bd: = bn.duplicate();bd.bg_color = Color(1, 1, 1, 0.06)
	q_btn.add_theme_stylebox_override("normal", bn);q_btn.add_theme_stylebox_override("hover", bh);q_btn.add_theme_stylebox_override("pressed", bh);q_btn.add_theme_stylebox_override("disabled", bd)
	q_btn.add_theme_font_override("font", m.font_b);q_btn.add_theme_color_override("font_color", Color.WHITE);q_btn.add_theme_color_override("font_disabled_color", Color("6b757f"))
	q_btn.pressed.connect(confirm);q_body.add_child(q_btn)
	if FileAccess.file_exists("user://ukol.txt") and FileAccess.open("user://ukol.txt", FileAccess.READ).get_as_text().strip_edges() == "sbaleno": q_body.visible = false

func toggle_quest() -> void :
	q_body.visible = not q_body.visible;quest.reset_size()
	var f: = FileAccess.open("user://ukol.txt", FileAccess.WRITE)
	if f != null: f.store_string("rozbaleno" if q_body.visible else "sbaleno")
	play("click")

func confirm() -> void :
	if m.sim.training.confirm():
		play("ok");notify("Krok splněn ✔", "good")
	else: play("deny")
	_refresh_quest()

func _refresh_quest() -> void :
	var tr = m.sim.training; var cur: Dictionary = tr.current(); var idx: int = tr.state().index
	var n: int = tr.STEPS.size()
	q_step.text = "krok %d / %d" % [mini(idx + 1, n), n]
	q_title.text = cur.title
	q_hint.text = cur.hint
	var det: String = str(cur.get("detail", "")).strip_edges()
	q_detail.text = det;q_detail.visible = det != ""
	q_bar.max_value = n;q_bar.value = idx
	q_btn.disabled = not cur.ready or cur.get("complete", false)
	q_btn.text = "✔  Potvrdit krok  (Enter)" if cur.ready else "… čeká na splnění podmínek"
	var sb: StyleBoxFlat = quest.get_theme_stylebox("panel")
	var acc: = Color("4fd18b") if cur.ready else YEL
	if sb.border_color != Color(acc, 0.55): sb.border_color = Color(acc, 0.55)

# ---------- oznámení ----------
func notify(t: String, kind: = "info") -> void :
	var col: Color = {"bad": Color("ff5a4a"), "warn": YEL, "good": Color("4fd18b")}.get(kind, Color("5aa9ff"))
	var p: = PanelContainer.new();p.add_theme_stylebox_override("panel", _card_sb(col));p.mouse_filter = Control.MOUSE_FILTER_IGNORE
	var h: = HBoxContainer.new();h.add_theme_constant_override("separation", 10);p.add_child(h)
	h.add_child(_lbl({"bad": "⚠", "warn": "!", "good": "✔"}.get(kind, "ℹ"), 18, col))
	var l: = _lbl(t, 14, Color("eef2f5"));l.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART;l.custom_minimum_size = Vector2(440, 0);h.add_child(l)
	feed.add_child(p);feed.move_child(p, 0)
	while feed.get_child_count() > 4: feed.get_child(feed.get_child_count() - 1).queue_free();feed.remove_child(feed.get_child(feed.get_child_count() - 1))
	p.modulate.a = 0.0;p.position.y -= 20
	var tw: = create_tween();tw.tween_property(p, "modulate:a", 1.0, 0.25)
	tw.tween_interval(5.0 + t.length() / 30.0);tw.tween_property(p, "modulate:a", 0.0, 0.6);tw.tween_callback(p.queue_free)

# ---------- popisek u myši ----------
func _tip() -> void :
	tip = PanelContainer.new();var sb: = _card_sb(Color("5aa9ff"));sb.content_margin_top = 6;sb.content_margin_bottom = 7;sb.content_margin_left = 10;sb.content_margin_right = 10
	tip.add_theme_stylebox_override("panel", sb);tip.mouse_filter = Control.MOUSE_FILTER_IGNORE;tip.visible = false
	tip_lbl = _lbl("", 13, Color("eef2f5"));tip.add_child(tip_lbl);add_child(tip)

func show_tip(t: String, sp: Vector2) -> void :
	if t == "":
		tip.visible = false;return
	if tip_lbl.text != t:
		tip_lbl.text = t;tip.reset_size()
	tip.visible = true
	var vs: Vector2 = get_viewport().get_visible_rect().size
	tip.position = Vector2(minf(sp.x + 18, vs.x - tip.size.x - 8), minf(sp.y + 20, vs.y - tip.size.y - 8))

# ---------- nápověda kláves ----------
func _help() -> void :
	help = PanelContainer.new();help.add_theme_stylebox_override("panel", _card_sb());help.visible = false
	help.anchor_left = 0.5;help.anchor_top = 0.5;help.anchor_right = 0.5;help.anchor_bottom = 0.5
	help.offset_left = -300;help.offset_top = -340;help.offset_right = 300;help.offset_bottom = 340
	add_child(help)
	var v: = VBoxContainer.new();v.add_theme_constant_override("separation", 6);help.add_child(v)
	v.add_child(_lbl("OVLÁDÁNÍ", 18, YEL))
	var rows: = [["Mezerník", "spustit / pozastavit čas"], ["1  2  3  4", "rychlost x1 / x10 / x60 / x180"], ["Enter", "potvrdit krok úkolu"], ["Q", "sbalit / rozbalit úkol"], ["Alt nad klapkou", "ukáže, kam z ní teče"], ["Ctrl + Z", "vrátit poslední klapku / čerpadlo"],
		["Tab", "ovládací panel"], ["R", "rozvaděč vpravo"], ["FM na rozvaděči", "táhni dokola nebo kolečko"], ["Z", "zvětšit rozvaděč (kolečko nad ním přiblíží)"], ["M", "mapka"], ["E", "editor plánku: přidat, upravit, smazat díly (Ctrl+S uloží)"], ["W A S D / šipky", "posun pohledu (Shift rychleji)"],
		["kolečko", "přiblížit / oddálit"], ["táhnout myší", "posun pohledu"], ["klik", "klapka, čerpadlo, spoušť, vývod…"], ["Ctrl + kolečko", "klapka v mezipoloze"],
		["Home", "celá varna"], ["F5 … F9", "pánev, SK, vířivka, bojler, spilka"], ["N", "zvuk zapnout / vypnout"], ["F1 / H", "tato nápověda"]]
	var gr: = GridContainer.new();gr.columns = 2;gr.add_theme_constant_override("h_separation", 18);gr.add_theme_constant_override("v_separation", 4);v.add_child(gr)
	for r in rows:
		gr.add_child(_lbl(r[0], 13, Color("ffd95a")));gr.add_child(_lbl(r[1], 13, Color("d5dbe0"), false))
	v.add_child(_lbl("PROVOZNÍ ZÁSADY", 16, YEL))
	var gr2: = GridContainer.new();gr2.columns = 2;gr2.add_theme_constant_override("h_separation", 18);gr2.add_theme_constant_override("v_separation", 4);v.add_child(gr2)
	for r2 in preload("res://practice.gd").RULES:
		gr2.add_child(_lbl(r2[0], 12, Color("ffd95a")))
		var l2: Label = _lbl(r2[1], 12, Color("d5dbe0"), false);l2.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART;l2.custom_minimum_size = Vector2(380, 0);gr2.add_child(l2)
	v.add_child(_lbl("Esc zavře", 11, Color("6b757f"), false))

func toggle_help() -> void :
	help.visible = not help.visible;play("click")

# ---------- zvuky (generované, bez souborů) ----------
func _wav(samples: PackedFloat32Array, rate: = 22050, loop: = false) -> AudioStreamWAV:
	var data: = PackedByteArray();data.resize(samples.size() * 2)
	for i in samples.size(): data.encode_s16(i * 2, int(clampf(samples[i], -1.0, 1.0) * 30000.0))
	var w: = AudioStreamWAV.new();w.format = AudioStreamWAV.FORMAT_16_BITS;w.mix_rate = rate;w.data = data
	if loop:
		w.loop_mode = AudioStreamWAV.LOOP_FORWARD;w.loop_end = samples.size()
	return w

func _tone(f0: float, f1: float, dur: float, vol: float, noise: = 0.0, shape: = 1.0) -> PackedFloat32Array:
	var n: = int(22050 * dur); var o: = PackedFloat32Array();o.resize(n); var ph: = 0.0
	for i in n:
		var t: = float(i) / n
		ph += TAU * lerpf(f0, f1, t) / 22050.0
		var env: = pow(1.0 - t, shape) * minf(1.0, i / 120.0)
		o[i] = (sin(ph) + noise * randf_range(-1.0, 1.0)) * env * vol
	return o

func _sounds() -> void :
	var defs: = {"click": _tone(1800, 1200, 0.035, 0.25, 0.1, 2.0), "valve": _tone(140, 70, 0.16, 0.6, 0.35, 1.5), "start": _tone(120, 360, 0.45, 0.35, 0.05, 0.6),
		"stop": _tone(320, 90, 0.4, 0.3, 0.05, 0.8), "ok": _tone(660, 990, 0.18, 0.3, 0.0, 1.0), "deny": _tone(220, 180, 0.15, 0.3, 0.1, 1.0)}
	var al: = _tone(880, 880, 0.12, 0.3, 0.0, 0.2);al.append_array(PackedFloat32Array([0.0]).duplicate());var gap: = PackedFloat32Array();gap.resize(1500);al.append_array(gap);al.append_array(_tone(660, 660, 0.14, 0.3, 0.0, 0.2))
	defs["alarm"] = al
	for k in defs:
		var p: = AudioStreamPlayer.new();p.stream = _wav(defs[k]);add_child(p);snd[k] = p
	# bzučení čerpadel (smyčka 1 s, 50 Hz motor + harmonické + šum)
	var hs: = PackedFloat32Array();hs.resize(22050)
	for i in 22050:
		var t: = i / 22050.0
		hs[i] = 0.35 * sin(TAU * 100.0 * t) + 0.2 * sin(TAU * 50.0 * t) + 0.12 * sin(TAU * 300.0 * t) + 0.05 * randf_range(-1.0, 1.0)
	hum = AudioStreamPlayer.new();hum.stream = _wav(hs, 22050, true);hum.volume_db = -80.0;add_child(hum)

func play(k: String) -> void :
	if muted or not snd.has(k): return
	snd[k].play()

func toggle_mute() -> void :
	muted = not muted
	var f: = FileAccess.open("user://zvuk.txt", FileAccess.WRITE)
	if f != null: f.store_string("vyp" if muted else "zap")
	m.hud.toast("Zvuk: " + ("vypnut (N)" if muted else "zapnut (N)"))

func _process(delta: float) -> void :
	t_acc += delta
	if t_acc > 0.25:
		t_acc = 0.0;_refresh_quest();_feed_log();_refresh_pult()
	var S = m.sim.S; var npumps: = 0
	for p in ["p1", "p2", "p3"]: if S[p].on and S[p].q > 1.0: npumps += 1
	if S.lv.pump: npumps += 1
	if S.rvpWater.on or S.skWater.on: npumps += 1
	var target: = -80.0 if (muted or npumps == 0 or not m.running) else (-26.0 + 2.5 * npumps)
	hum.volume_db = lerpf(hum.volume_db, target, minf(1.0, delta * 3.0))
	if hum.volume_db > -60.0 and not hum.playing: hum.play()
	elif hum.volume_db <= -78.0 and hum.playing: hum.stop()

func _feed_log() -> void :
	var L: Array = m.sim.S.log
	if L.is_empty(): return
	if L[0].t == last_log and L.size() > 0: return
	var news: = []
	for l in L:
		if l.t <= last_log: break
		news.append(l)
		if news.size() >= 3: break
	last_log = L[0].t
	news.reverse()
	for l in news:
		if l.type == "bad": notify(l.msg, "bad");play("alarm")
		elif l.type == "warn": notify(l.msg, "warn")

# ---------- zvýraznění pod myší ----------
class HoverLayer extends Node2D:
	var g
	var m
	var cur: = {}
	var ripples: = []
	var prev_key: = ""
	var prev_lines: = []
	func _process(delta: float) -> void :
		var vp: = get_viewport()
		var sp: Vector2 = vp.get_mouse_position()
		var over_gui: bool = vp.gui_get_hovered_control() != null
		if m.dock != null and m.dock.visible and m.dock.get_global_rect().has_point(sp): over_gui = true
		if g.quest != null and g.quest.get_global_rect().has_point(sp): over_gui = true
		var h: Dictionary = {} if over_gui or m.cam == null else m.pick(m.screen_to_world(sp))
		if h.get("key", "") != cur.get("key", ""):
			Input.set_default_cursor_shape(Input.CURSOR_POINTING_HAND if h.get("act", false) else Input.CURSOR_ARROW)
		cur = h
		# Alt nad klapkou: ukáž, kam z ní teče (otevřenými klapkami)
		var key: String = h.get("key", "") if Input.is_key_pressed(KEY_ALT) else ""
		if key != prev_key:
			prev_key = key;prev_lines = m.flow_reach(key) if key.begins_with("valve") else []
		g.show_tip(h.get("text", ""), sp)
		for r in ripples: r.t += delta
		ripples = ripples.filter(func(r): return r.t < 0.5)
		queue_redraw()
	func ripple(w: Vector2) -> void :
		ripples.append({"p": w, "t": 0.0})
	func _draw() -> void :
		var z: float = m.cam.zoom.x if m.cam != null else 1.0
		var tt: = Time.get_ticks_msec() / 1000.0
		if not cur.is_empty():
			var col: Color = cur.get("col", Color("5aa9ff"))
			var pul: = 0.5 + 0.5 * sin(tt * 6.0)
			if cur.has("rect"):
				var r: Rect2 = cur.rect
				draw_rect(r.grow(8.0 / z), Color(col, 0.10 + 0.08 * pul))
				draw_rect(r.grow(8.0 / z), Color(col, 0.6 + 0.3 * pul), false, 2.5 / z)
			elif cur.has("pos"):
				var R: float = cur.get("r", 22.0) + 4.0 * pul / z
				draw_circle(cur.pos, R, Color(col, 0.14))
				draw_arc(cur.pos, R, 0, TAU, 40, Color(col, 0.75 + 0.25 * pul), 3.0 / z, true)
		for pl in prev_lines:
			draw_polyline(pl, Color(0.35, 0.75, 1.0, 0.35), 14.0 / z, true)
			draw_polyline(pl, Color(0.6, 0.9, 1.0, 0.9), 3.0 / z, true)
		for rp in ripples:
			var k: float = rp.t / 0.5
			draw_arc(rp.p, (10.0 + 50.0 * k) / z, 0, TAU, 40, Color(1, 0.85, 0.3, 0.8 * (1.0 - k)), 3.0 / z, true)


# ---------- operátorský pult dole: nejčastější akce velkými dlaždicemi ----------
var pult: PanelContainer
var pult_sc: ScrollContainer
var tiles: = []
func _pult() -> void :
	pult = PanelContainer.new()
	var sb: = StyleBoxFlat.new();sb.bg_color = Color(0.04, 0.05, 0.06, 0.88);sb.set_corner_radius_all(12);sb.border_color = Color(1, 1, 1, 0.08);sb.set_border_width_all(1)
	sb.content_margin_left = 8;sb.content_margin_right = 8;sb.content_margin_top = 6;sb.content_margin_bottom = 6;sb.shadow_color = Color(0, 0, 0, 0.4);sb.shadow_size = 10
	pult.add_theme_stylebox_override("panel", sb)
	pult.anchor_top = 1.0;pult.anchor_bottom = 1.0;pult.offset_bottom = -12;pult.offset_top = -12 - 84
	add_child(pult)
	pult_sc = ScrollContainer.new();pult_sc.vertical_scroll_mode = ScrollContainer.SCROLL_MODE_DISABLED;pult.add_child(pult_sc)
	var h: = HBoxContainer.new();h.add_theme_constant_override("separation", 6);pult_sc.add_child(h)
	var S = m.sim.S
	_tile(h, "VODA → PÁNEV", func(): m.ctl._water() if m.ctl != null else null, func(d): S.rvpWater.target = maxf(S.rvpWater.added, S.rvpWater.target + 100.0 * d),
		func(): return [("%d/%d L" % [roundi(S.rvpWater.added), roundi(S.rvpWater.target)]), S.rvpWater.on])
	_tile(h, "ŠROT", func(): m.ctl._malt() if m.ctl != null else null, null,
		func(): return [("%d/%d kg" % [roundi(S.malt.added), roundi(S.malt.target)]), S.malt.on])
	_tile(h, "PÁRA PÁNEV", func(): S.rvp.heat = 0.0 if S.rvp.heat >= 0.99 else snappedf(S.rvp.heat + 0.25, 0.25), func(d): S.rvp.heat = clampf(snappedf(S.rvp.heat + 0.25 * d, 0.25), 0.0, 1.0),
		func(): return [("%d %%" % roundi(S.rvp.heat * 100.0)), S.rvp.heat > 0.0])
	_tile(h, "MÍCHADLO", func(): S.rvp.stir = not S.rvp.stir, func(d): S.rvp.hz = clampf(S.rvp.hz + 2.5 * d, 10.0, 50.0),
		func(): return [("%d Hz" % roundi(S.rvp.hz)) if S.rvp.stir else "stojí", S.rvp.stir])
	_tile(h, "KOPAČKA SK", func(): S.sk.rake = not S.sk.rake, func(d): S.sk.rakeH = clampf(S.sk.rakeH + 0.1 * d, 0.0, 1.0),
		func(): return [("hloubka %d %%" % roundi(S.sk.rakeH * 100.0)) if S.sk.rake else "stojí", S.sk.rake])
	_tile(h, "VYSLAZOVÁNÍ", func(): S.skWater.on = not S.skWater.on, func(d): S.skWater.flow = clampf(S.skWater.flow + 10.0 * d, 10.0, 200.0),
		func(): return [("%d L/min" % roundi(S.skWater.flow)), S.skWater.on])
	for p in [["p1", 25.0, 50.0, 400.0], ["p2", 10.0, 10.0, 200.0], ["p3", 10.0, 20.0, 150.0]]:
		var key: String = p[0]; var stp: float = p[1]; var lo: float = p[2]; var hi: float = p[3]
		_tile(h, m.pump_name(key), func(): m.toggle_pump(key), func(d): S[key].flow = clampf(S[key].flow + stp * d, lo, hi),
			func(): return [("%d L/min" % roundi(S[key].q if S[key].on else S[key].flow)), S[key].on])
	_tile(h, "LEDOVKA " + m.pump_name("plv"), func(): m.toggle_pump("plv"), null, func(): return [("%s °C" % m.c1(S.lv.T)), S.lv.pump])
	_tile(h, "PÁRA Z KOTLE", func(): S.boiler.on = not S.boiler.on, null, func(): return [("%d %%" % roundi(S.boiler.load * 100.0)) if S.boiler.on else "vyp", S.boiler.on])

func _tile(parent: Control, title: String, click: Callable, wheel, val: Callable) -> void :
	var b: = Button.new();b.custom_minimum_size = Vector2(112, 70);b.focus_mode = Control.FOCUS_NONE;b.toggle_mode = false;b.clip_text = true
	var n: = StyleBoxFlat.new();n.bg_color = Color("161c22");n.set_corner_radius_all(9);n.border_color = Color("2a333c");n.set_border_width_all(1)
	var hv: = n.duplicate();hv.border_color = YEL;hv.bg_color = Color("1d252d")
	b.add_theme_stylebox_override("normal", n);b.add_theme_stylebox_override("hover", hv);b.add_theme_stylebox_override("pressed", hv)
	b.tooltip_text = title + "\nklik = zapnout / vypnout" + ("\nkolečko = nastavit hodnotu" if wheel != null else "")
	var v: = VBoxContainer.new();v.set_anchors_preset(Control.PRESET_FULL_RECT);v.offset_left = 8;v.offset_top = 6;v.offset_right = -8;v.offset_bottom = -6;v.mouse_filter = Control.MOUSE_FILTER_IGNORE
	v.add_theme_constant_override("separation", 2);b.add_child(v)
	var t: = _lbl(title, 10, Color("8a939b"));t.mouse_filter = Control.MOUSE_FILTER_IGNORE;t.clip_text = true;v.add_child(t)
	var vl: = _lbl("", 15, Color("eef2f5"));vl.mouse_filter = Control.MOUSE_FILTER_IGNORE;vl.clip_text = true;v.add_child(vl)
	var led: = ColorRect.new();led.custom_minimum_size = Vector2(0, 4);led.mouse_filter = Control.MOUSE_FILTER_IGNORE;v.add_child(led)
	b.pressed.connect(func(): click.call();play("click");m.redraw();_refresh_pult())
	if wheel != null:
		b.gui_input.connect(func(e):
			if e is InputEventMouseButton and e.pressed and e.button_index in [MOUSE_BUTTON_WHEEL_UP, MOUSE_BUTTON_WHEEL_DOWN]:
				wheel.call(1 if e.button_index == MOUSE_BUTTON_WHEEL_UP else -1);_refresh_pult();b.accept_event())
	parent.add_child(b)
	tiles.append({"b": b, "v": vl, "led": led, "val": val, "n": n})

func _refresh_pult() -> void :
	if pult == null: return
	var left: = 12.0
	pult.offset_left = left;pult.offset_right = - m.right_w() - 12.0
	pult.visible = pult_on
	if m.mini != null:
		var mh: float = m.mini.size.y; var base: = -14.0 - (96.0 if pult_on else 0.0)
		if m.mini.offset_bottom != base:
			m.mini.offset_bottom = base;m.mini.offset_top = base - mh
	for t in tiles:
		var r: Array = t.val.call()
		if t.v.text != r[0]: t.v.text = r[0]
		var on: bool = r[1]
		t.led.color = Color("4fd18b") if on else Color(1, 1, 1, 0.08)
		t.v.add_theme_color_override("font_color", Color("ffffff") if on else Color("aab3bb"))

var pult_on: = true
func toggle_pult() -> void :
	pult_on = not pult_on;_refresh_pult();play("click")
