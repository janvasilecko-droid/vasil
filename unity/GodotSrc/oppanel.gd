extends PanelContainer
## Pevný ovládací pult vpravo: jen to, co obsluha opravdu ovládá – napájení, teploměry,
## ohřev, míchadla, čerpadla a frekvenční měniče (FM) se stupnicí 1–10. Velké, čitelné.

const YEL: = Color("f2c230")
const GREEN: = Color("2f9e55")
const RED: = Color("c8372b")

var m
var S
var rows: = []        # [callable] obnova textů a barev
var fb: Font
var fr: Font

func setup() -> void :
	S = m.sim.S;fb = m.font_b;fr = m.font
	anchor_left = 1.0;anchor_right = 1.0;anchor_bottom = 1.0;offset_top = 52;offset_bottom = 0
	var psb: = StyleBoxFlat.new();psb.bg_color = Color("0b0f13");psb.border_color = Color("2a323a");psb.border_width_left = 2
	psb.content_margin_left = 10;psb.content_margin_right = 6;psb.content_margin_top = 8;psb.content_margin_bottom = 8
	add_theme_stylebox_override("panel", psb)
	var sc: = ScrollContainer.new();sc.horizontal_scroll_mode = ScrollContainer.SCROLL_MODE_DISABLED;add_child(sc)
	var box: = VBoxContainer.new();box.size_flags_horizontal = Control.SIZE_EXPAND_FILL;box.add_theme_constant_override("separation", 8);sc.add_child(box)
	_build(box)
	refresh()

func fit() -> void : pass
func reset_view() -> void : pass

# ---------- stavební kostky ----------
func _card(box: Control, title: String) -> VBoxContainer:
	var p: = PanelContainer.new()
	var sb: = StyleBoxFlat.new();sb.bg_color = Color("151b21");sb.set_corner_radius_all(10);sb.border_color = Color("252e37");sb.set_border_width_all(1)
	sb.content_margin_left = 10;sb.content_margin_right = 10;sb.content_margin_top = 6;sb.content_margin_bottom = 10
	p.add_theme_stylebox_override("panel", sb);box.add_child(p)
	var v: = VBoxContainer.new();v.add_theme_constant_override("separation", 6);p.add_child(v)
	v.add_child(_lbl(title, 13, YEL))
	return v

func _lbl(t: String, size: int, col: Color, bold: = true) -> Label:
	var l: = Label.new();l.text = t;l.add_theme_font_override("font", fb if bold else fr)
	l.add_theme_font_size_override("font_size", size);l.add_theme_color_override("font_color", col)
	return l

func _sb(bg: Color, border: Color) -> StyleBoxFlat:
	var s: = StyleBoxFlat.new();s.bg_color = bg;s.set_corner_radius_all(8);s.border_color = border;s.set_border_width_all(1)
	s.content_margin_left = 8;s.content_margin_right = 8;s.content_margin_top = 6;s.content_margin_bottom = 6
	return s

func _btn(parent: Control, text: String, f: Callable, minw: = 0.0, h: = 40.0) -> Button:
	var b: = Button.new();b.text = text;b.focus_mode = Control.FOCUS_NONE;b.custom_minimum_size = Vector2(minw, h)
	b.add_theme_font_override("font", fb);b.add_theme_font_size_override("font_size", 15)
	_style(b, false)
	b.pressed.connect(func(): f.call();_click();refresh())
	parent.add_child(b);return b

func _style(b: Button, on: bool, col: = GREEN) -> void :
	var n: = _sb(col if on else Color("222a32"), col.lightened(0.25) if on else Color("37424d"))
	var hv: = _sb(col.lightened(0.12) if on else Color("2c3640"), YEL)
	b.add_theme_stylebox_override("normal", n);b.add_theme_stylebox_override("hover", hv);b.add_theme_stylebox_override("pressed", hv)
	b.add_theme_color_override("font_color", Color.WHITE if on else Color("c9d1d8"));b.add_theme_color_override("font_hover_color", Color.WHITE)

func _toggle(parent: Control, name: String, get_on: Callable, f: Callable, col: = GREEN, minw: = 0.0) -> Button:
	var b: = _btn(parent, name, f, minw)
	b.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	rows.append(func():
		var on: bool = get_on.call()
		var t: String = "%s: %s" % [name, "ZAP" if on else "vyp"]
		if b.text != t: b.text = t
		if b.get_meta("on", null) != on:
			b.set_meta("on", on);_style(b, on, col))
	return b

func _click() -> void :
	if m.game != null: m.game.play("click")

func _cab(k: String) -> void :
	if m.cabinet != null: m.cabinet.act(k, true)
	m.redraw()

# ---------- frekvenční měnič: velké − / + a deset stupňů ----------
## frekvenční měnič jako na rozvaděči: otočný potenciometr se stupnicí 1–10 (táhni do kruhu, kolečko myši)
func _fm(parent: Control, get_st: Callable, set_st: Callable, info: Callable) -> void :
	var h: = HBoxContainer.new();h.add_theme_constant_override("separation", 10);parent.add_child(h)
	var k: = Knob.new();k.custom_minimum_size = Vector2(170, 170);k.font = fb;k.get_v = get_st;k.set_v = func(x): set_st.call(x);refresh()
	k.on_click = _click;h.add_child(k)
	var v: = VBoxContainer.new();v.alignment = BoxContainer.ALIGNMENT_CENTER;v.size_flags_horizontal = Control.SIZE_EXPAND_FILL;h.add_child(v)
	v.add_child(_lbl("STUPEŇ", 12, Color("9aa3ab")))
	var big: = _lbl("", 44, YEL);v.add_child(big)
	var lb: = _lbl("", 15, Color("eef2f5"), false);lb.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART;v.add_child(lb)
	var hint: = _lbl("táhni kolem knoflíku / kolečko", 11, Color("6b757f"), false);v.add_child(hint)
	rows.append(func():
		var cur: float = get_st.call()
		var t: String = ("%.1f" % cur).replace(".", ",")
		if big.text != t: big.text = t
		var t2: String = info.call()
		if lb.text != t2: lb.text = t2
		k.queue_redraw())

class Knob extends Control:
	var font: Font
	var get_v: Callable
	var set_v: Callable
	var on_click: Callable
	var drag: = false
	func _ready() -> void :
		mouse_filter = Control.MOUSE_FILTER_STOP;mouse_default_cursor_shape = Control.CURSOR_POINTING_HAND
	func _val_from(p: Vector2) -> float:
		var d: = p - size * 0.5
		var a: = rad_to_deg(atan2(d.x, - d.y))   # 0 = nahoře, ± 135 = krajní
		return clampf(1.0 + 9.0 * (clampf(a, -135.0, 135.0) + 135.0) / 270.0, 1.0, 10.0)
	func _gui_input(e: InputEvent) -> void :
		if e is InputEventMouseButton:
			if e.button_index == MOUSE_BUTTON_LEFT:
				drag = e.pressed
				if e.pressed: set_v.call(snappedf(_val_from(e.position), 0.1));on_click.call()
				accept_event()
			elif e.pressed and e.button_index in [MOUSE_BUTTON_WHEEL_UP, MOUSE_BUTTON_WHEEL_DOWN]:
				set_v.call(clampf(snappedf(get_v.call() + (0.2 if e.button_index == MOUSE_BUTTON_WHEEL_UP else -0.2), 0.1), 1.0, 10.0));accept_event()
		elif e is InputEventMouseMotion and drag:
			set_v.call(snappedf(_val_from(e.position), 0.1));accept_event()
	func _draw() -> void :
		var c: = size * 0.5; var R: = minf(size.x, size.y) * 0.5 - 4.0
		var r_b: = R * 0.70
		# deska + stupnice
		draw_circle(c + Vector2(2, 3), r_b + 3.0, Color(0, 0, 0, 0.35))
		draw_circle(c, r_b, Color("c3c7cc"));draw_arc(c, r_b, 0, TAU, 48, Color("5b6066"), 2.0, true)
		draw_arc(c, r_b - 4.0, 0, TAU, 48, Color(0.93, 0.94, 0.95, 0.7), 1.5, true)
		var val: float = get_v.call()
		var a0: = deg_to_rad(-135.0)
		# barevný oblouk do aktuální hodnoty
		var frac: = (val - 1.0) / 9.0
		draw_arc(c, r_b + 6.0, a0 - PI * 0.5, a0 - PI * 0.5 + deg_to_rad(270.0) * frac, 48, Color("f2c230"), 5.0, true)
		draw_arc(c, r_b + 6.0, a0 - PI * 0.5 + deg_to_rad(270.0) * frac, a0 - PI * 0.5 + deg_to_rad(270.0), 48, Color(1, 1, 1, 0.1), 5.0, true)
		for i in 10:
			var a: float = deg_to_rad(-135.0 + 30.0 * i)
			var dv: = Vector2(sin(a), - cos(a))
			draw_line(c + dv * (r_b - 10.0), c + dv * (r_b - 2.0), Color("2a2d30"), 2.0)
			var t: = str(i + 1); var fs: = 15
			var tw: float = font.get_string_size(t, HORIZONTAL_ALIGNMENT_LEFT, -1, fs).x
			draw_string(font, c + dv * (r_b + 20.0) + Vector2(- tw * 0.5, fs * 0.36), t, HORIZONTAL_ALIGNMENT_LEFT, -1, fs, Color("eef2f5") if absf(val - (i + 1)) < 0.5 else Color("8a939b"))
		# knoflík
		var rk: = r_b * 0.62
		draw_circle(c, rk, Color("1c1e21"));draw_arc(c, rk, 0, TAU, 40, Color.BLACK, 2.0, true)
		for i in 18:
			var ga: = TAU * i / 18.0
			draw_line(c + Vector2(cos(ga), sin(ga)) * (rk - 4.0), c + Vector2(cos(ga), sin(ga)) * rk, Color("33363a"), 2.0)
		var ang: float = deg_to_rad(-135.0 + 270.0 * frac)
		var dd: = Vector2(sin(ang), - cos(ang))
		draw_line(c + dd * 6.0, c + dd * (rk - 5.0), Color.WHITE, 4.0, true)
		var mt: = font.get_string_size("−", HORIZONTAL_ALIGNMENT_LEFT, -1, 18).x
		draw_string(font, c + Vector2(- r_b * 0.62 - mt * 0.5, r_b + 14.0), "−", HORIZONTAL_ALIGNMENT_LEFT, -1, 18, Color("8a939b"))
		draw_string(font, c + Vector2(r_b * 0.62 - mt * 0.5, r_b + 14.0), "+", HORIZONTAL_ALIGNMENT_LEFT, -1, 18, Color("8a939b"))

func _pump_st(b: String) -> float:
	var F = m.cabinet.FMS[b] if m.cabinet != null else {"min": 10.0, "max": 400.0}
	return 1.0 + 9.0 * clampf((S[b].flow - F.min) / (F.max - F.min), 0.0, 1.0)
func _pump_set(b: String, st: float) -> void :
	var F = m.cabinet.FMS[b] if m.cabinet != null else {"min": 10.0, "max": 400.0}
	S[b].flow = roundf(F.min + (F.max - F.min) * (clampf(st, 1.0, 10.0) - 1.0) / 9.0)

# ---------- teploměr jako displej Dixell ----------
func _disp(parent: Control, title: String, val: Callable, sp_up: = "", sp_dn: = "", sp: = Callable()) -> void :
	var p: = PanelContainer.new();p.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	var sb: = _sb(Color("0d0f11"), Color("3a434b"));p.add_theme_stylebox_override("panel", sb);parent.add_child(p)
	var v: = VBoxContainer.new();v.add_theme_constant_override("separation", 0);p.add_child(v)
	v.add_child(_lbl(title, 11, Color("9aa3ab")))
	var h: = HBoxContainer.new();v.add_child(h)
	var big: = _lbl("---", 30, Color("ff3b2f"));big.size_flags_horizontal = Control.SIZE_EXPAND_FILL;h.add_child(big)
	var spl: Label = null
	if sp_up != "":
		var c: = VBoxContainer.new();c.add_theme_constant_override("separation", 2);h.add_child(c)
		var u: = _btn(c, "▲", func(): _cab(sp_up), 34, 22);u.add_theme_font_size_override("font_size", 11)
		var d: = _btn(c, "▼", func(): _cab(sp_dn), 34, 22);d.add_theme_font_size_override("font_size", 11)
		spl = _lbl("", 11, YEL);v.add_child(spl)
	rows.append(func():
		var t: String = val.call()
		if big.text != t: big.text = t
		if spl != null:
			var st: String = "nastaveno %d °C" % roundi(sp.call())
			if spl.text != st: spl.text = st)

func _row(parent: Control) -> HBoxContainer:
	var h: = HBoxContainer.new();h.add_theme_constant_override("separation", 6);parent.add_child(h);return h

func _status(parent: Control, f: Callable) -> void :
	var l: = _lbl("", 12, Color("9aa3ab"), false);l.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART;parent.add_child(l)
	rows.append(func():
		var t: String = f.call()
		if l.text != t: l.text = t
		l.visible = t != "")

# ---------- obsah ----------
func _build(box: Control) -> void :
	var _c = S.cab
	var v: = _card(box, "NAPÁJENÍ ROZVADĚČE")
	var r: = _row(v)
	_toggle(r, "HLAVNÍ", func(): return S.cab.main, func(): _cab("main"))
	_toggle(r, "KLÍČ START", func(): return S.cab.key, func(): _cab("key"))
	var es: = _toggle(r, "NOUZ. STOP", func(): return S.cab.estop, func(): _cab("estop"), RED)
	es.tooltip_text = "Nouzové vypnutí – stiskni znovu pro odblokování, pak otoč klíč START"

	v = _card(box, "TEPLOTY")
	var g: = GridContainer.new();g.columns = 2;g.add_theme_constant_override("h_separation", 6);g.add_theme_constant_override("v_separation", 6);v.add_child(g)
	_disp(g, "PÁNEV (MP)", func(): return m.cabinet._disp_text("mp") if m.cabinet != null else "", "spmp_u", "spmp_d", func(): return S.cab.get("mpSet", 50))
	_disp(g, "BOJLER", func(): return m.cabinet._disp_text("bo") if m.cabinet != null else "", "spbo_u", "spbo_d", func(): return S.hlt.set)
	_disp(g, "SCEZOVACÍ KÁĎ", func(): return m.cabinet._disp_text("sk") if m.cabinet != null else "")
	_disp(g, "MLADINA (za chladičem)", func(): return m.cabinet._disp_text("ml") if m.cabinet != null else "")

	# --- nejdůležitější nahoře: rmutovací čerpadlo, čerpadlo boileru, ohřev ručně/automat ---
	v = _card(box, "RMUTOVACÍ ČERPADLO " + m.pump_name("p1"))
	_pump(v, "p1", "pvarna", true)

	v = _card(box, "ČERPADLO BOILERU " + m.pump_name("phw"))
	r = _row(v)
	var pb: = _toggle(r, "ČERPADLO BOILERU", func(): return S.rvpWater.on or S.skWater.on, func(): _cab("pboil"))
	pb.custom_minimum_size.y = 48;pb.add_theme_font_size_override("font_size", 18)
	_fm(v, func(): return 1.0 + 9.0 * clampf((S.cfg.waterFlow - 50.0) / 250.0, 0.0, 1.0), func(st): S.cfg.waterFlow = roundf(50.0 + 250.0 * (clampf(st, 1.0, 10.0) - 1.0) / 9.0),
		func(): return ("napouští pánev %d / %d L · %d L/min" % [roundi(S.rvpWater.added), roundi(S.rvpWater.target), roundi(S.cfg.waterFlow)]) if S.rvpWater.on else ("vyslazuje %d L/min" % roundi(S.skWater.flow) if S.skWater.on else "nastaveno %d L/min" % roundi(S.cfg.waterFlow)))

	v = _card(box, "OHŘEV PÁNVE – RUČNĚ / AUTOMAT")
	r = _row(v)
	var bm: = _btn(r, "RUČNĚ", func():
		if S.cab.get("mpAuto", false): _cab("mpauto"), 0, 48)
	var ba: = _btn(r, "AUTOMAT", func():
		if not S.cab.get("mpAuto", false): _cab("mpauto"), 0, 48)
	bm.size_flags_horizontal = Control.SIZE_EXPAND_FILL;ba.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	bm.add_theme_font_size_override("font_size", 18);ba.add_theme_font_size_override("font_size", 18)
	rows.append(func():
		var a: bool = S.cab.get("mpAuto", false)
		if bm.get_meta("on", null) != (not a): bm.set_meta("on", not a);_style(bm, not a, Color("2f6fb0"))
		if ba.get_meta("on", null) != a: ba.set_meta("on", a);_style(ba, a, Color("2f6fb0")))
	r = _row(v)
	var oh: = _toggle(r, "OHŘEV PÁNVE", func(): return S.cab.get("mpHeatOn", false), func(): _cab("mpheat"), Color("c46a1f"))
	oh.custom_minimum_size.y = 48;oh.add_theme_font_size_override("font_size", 18)
	var hr: = _row(v)
	var hl_: = _lbl("pára:", 14, Color("c9d1d8"), false);hr.add_child(hl_)
	var hb: = []
	for p in [0, 25, 50, 75, 100]:
		var pp: int = p
		var b: = _btn(hr, "%d %%" % p, func(): S.rvp.heat = pp / 100.0;S.cab.mpHeatOn = pp > 0;m.redraw(), 0, 40)
		b.size_flags_horizontal = Control.SIZE_EXPAND_FILL;hb.append([b, pp])
	var spr: = _row(v)
	spr.add_child(_lbl("automat drží:", 14, Color("c9d1d8"), false))
	var spl: = _lbl("", 26, YEL);spl.size_flags_horizontal = Control.SIZE_EXPAND_FILL;spl.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER;spr.add_child(spl)
	var su: = _btn(spr, "▲", func(): _cab("spmp_u"), 48, 40);var sd: = _btn(spr, "▼", func(): _cab("spmp_d"), 48, 40)
	_status(v, func(): return ("RUČNĚ: pára %d %% podle tlačítek." % roundi(S.rvp.heat * 100.0)) if not S.cab.get("mpAuto", false) else ("AUTOMAT: pustí páru, když je pánev pod %d °C (teď %s °C)." % [roundi(S.cab.get("mpSet", 50)), m.c1(S.rvp.Tsens)]))
	rows.append(func():
		var a: bool = S.cab.get("mpAuto", false)
		hr.visible = not a;spr.visible = a
		var t: String = "%d °C" % roundi(S.cab.get("mpSet", 50))
		if spl.text != t: spl.text = t
		for x in hb:
			var on: bool = roundi(S.rvp.heat * 100.0) == x[1]
			if x[0].get_meta("on", null) != on: x[0].set_meta("on", on);_style(x[0], on, Color("c46a1f")))

	v = _card(box, "KOTEL A BOJLER")
	r = _row(v)
	_toggle(r, "PÁRA Z KOTLE", func(): return S.boiler.on, func(): S.boiler.on = not S.boiler.on;m.redraw(), Color("c46a1f"))
	_toggle(r, "OHŘEV BOJLERU", func(): return S.hlt.auto, func(): _cab("hltheat"), Color("c46a1f"))
	_status(v, func(): return ("kotel zatížen %d %%" % roundi(S.boiler.load * 100.0)) if S.boiler.on else "")

	v = _card(box, "MÍCHADLO PÁNVE")
	r = _row(v)
	_toggle(r, "MÍCHADLO MP", func(): return S.rvp.stir, func(): _cab("stirmp"))
	_fm(v, func(): return 1.0 + 9.0 * clampf((S.rvp.hz - 10.0) / 40.0, 0.0, 1.0), func(st): S.rvp.hz = roundf(10.0 + 40.0 * (clampf(st, 1.0, 10.0) - 1.0) / 9.0);m.redraw(), func(): return "%d Hz" % roundi(S.rvp.hz))

	v = _card(box, "SCEZOVACÍ KÁĎ – KOPAČKA")
	r = _row(v)
	_toggle(r, "KOPAČKA", func(): return S.sk.rake, func(): _cab("rake"))
	_fm(v, func(): return 1.0 + 9.0 * S.cab.get("potsk", 0.5), func(st): S.cab.potsk = (clampf(st, 1.0, 10.0) - 1.0) / 9.0;m.redraw(), func(): return "otáčky kopačky")
	r = _row(v)
	_toggle(r, "HYDRAULIKA", func(): return S.cab.get("hydOn", false), func(): _cab("hyd"))
	var up: = Button.new();up.text = "▲ NAHORU";up.focus_mode = Control.FOCUS_NONE;up.custom_minimum_size = Vector2(0, 44);up.size_flags_horizontal = Control.SIZE_EXPAND_FILL;_style(up, false);up.add_theme_font_override("font", fb)
	var dn: = Button.new();dn.text = "▼ DOLŮ";dn.focus_mode = Control.FOCUS_NONE;dn.custom_minimum_size = Vector2(0, 44);dn.size_flags_horizontal = Control.SIZE_EXPAND_FILL;_style(dn, false);dn.add_theme_font_override("font", fb)
	up.button_down.connect(func(): m.cabinet.act("hup", true));up.button_up.connect(func(): m.cabinet.act("hup", false))
	dn.button_down.connect(func(): m.cabinet.act("hdn", true));dn.button_up.connect(func(): m.cabinet.act("hdn", false))
	var r2: = _row(v);r2.add_child(up);r2.add_child(dn)
	_status(v, func(): return "hloubka kopačky %d %% (drž tlačítko)" % roundi(S.sk.rakeH * 100.0))

	v = _card(box, "OSTATNÍ ČERPADLA")
	_pump(v, "p2", "bxs", true)
	_pump(v, "p3", "rez2", true)
	_sep(v)
	r = _row(v)
	_toggle(r, m.pump_name("plv") + " LEDOVÁ VODA", func(): return S.lv.pump, func(): _cab("rez1"))

func _sep(v: Control) -> void :
	var s: = ColorRect.new();s.color = Color(1, 1, 1, 0.06);s.custom_minimum_size = Vector2(0, 1);v.add_child(s)


func _pump(v: Control, b: String, cab_key: String, fm: bool) -> void :
	if v.get_child_count() > 1: _sep(v)
	var r: = _row(v)
	var pt: = _toggle(r, m.pump_name(b), func(): return S[b].on, func(): _cab(cab_key))
	pt.custom_minimum_size.y = 48;pt.add_theme_font_size_override("font_size", 18)
	if fm:
		_fm(v, func(): return _pump_st(b), func(st): _pump_set(b, st);m.redraw(), func(): return ("teče %d L/min (nastaveno %d)" % [roundi(S[b].q), roundi(S[b].flow)]) if S[b].on else ("nastaveno %d L/min" % roundi(S[b].flow)))
	_status(v, func(): return S[b].status if S[b].on else "")

var t_acc: = 0.0
func _process(delta: float) -> void :
	t_acc += delta
	if t_acc > 0.2 and visible:
		t_acc = 0.0;refresh()

func refresh() -> void :
	for f in rows: f.call()
