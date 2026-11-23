extends PanelContainer
## Rozvaděč RT.20 vpravo – tmavý SCADA dispečink jako v původním trenažéru: displeje nádob se signálkami, parní kotel,
## pánev (pára do pláště, AUTO na teplotu, míchadlo, napouštění s předvolbou, slad, jódová zkouška, chmel), scezovací káď,
## čerpadla s frekvenčními měniči (nastavený a skutečný průtok, Hz, bar, NPSH, odvzdušnění, ucpávka), ledová voda
## a glykol, cesty, voda a pH, kvašení, sanitace CIP, nácvik poruch, spílání s vyhodnocením a hlášení.
## Ovládá tutéž simulaci, klapky a čerpadla jako levý panel a klikání ve scéně.

const NetFlowScript := preload("res://scripts/net_flow.gd")
const C_PANEL := Color(0.11, 0.13, 0.16, 1)
const C_LINE := Color(0.22, 0.25, 0.3)
const C_TXT := Color(0.86, 0.9, 0.95)
const C_DIM := Color(0.55, 0.6, 0.68)
const C_OK := Color(0.2, 0.9, 0.35)
const C_WARN := Color(1.0, 0.75, 0.15)
const C_BAD := Color(1.0, 0.25, 0.2)
const C_ACC := Color(0.25, 0.65, 1.0)
const C_OFF := Color(0.25, 0.25, 0.28)
const WIDTH := 418.0
const SAVE_PATH := "user://rt20_state.json"
const OPEN_SECS := ["RMUTOMLADINOVÁ PÁNEV", "ČERPADLA (frekvenční měniče)", "CESTY · KLAPKY", "HLÁŠENÍ"]
const MODES := ["Výuka (nápověda + ochrany)", "Samostatně (ochrany)", "Zkouška (bez nápovědy a ochran)"]
## produktová čerpadla s frekvenčními měniči podle "bind" v plánu (C5 ledová je v sekci ledové vody)
const PUMPS := [["p1", "rmutovací"], ["p2", "scezovací"], ["p3", "spílací"], ["phw", "horká voda"]]
## rozpis chmele pro ~42 hl ležáku jako v checklistu: minuta varu, kg, % alfa
const Recipes := preload("res://scripts/recipes.gd")
const WATER_DEFAULT := {"alk": 150.0, "ca": 60.0, "mg": 15.0}  # rozbor vody z řadu (mg/L) – NEZNÁMÝ, zadej skutečný
const CIP_TARGETS := {"rvp": "Rmutomladinová pánev", "sk": "Scezovací káď", "wp": "Vířivá káď", "plate": "Deskový chladič", "spilka": "Spilka"}
const CIP_MEDIA := ["louh", "voda", "kyselina"]
const FAULTS := ["air", "steam", "plate", "valve"]
const NPSH_R := 2.5  # m – potřebné NPSH odstředivého čerpadla
const DN_MIN := 50.0  # mm – trubky v plánu mají většinou výchozí DN 25; pro tlak a NPSH se počítá aspoň s DN 50 jako na varně 50 hl
const VNAME := {"hlt": "bojler", "rvp": "varna", "sk": "SK", "wp": "vířivka", "spilka": "spilka", "lv": "ledovka", "mains": "řad", "kanal": "kanál"}
const EMPTY := {"hlt": "bojler je prázdný", "rvp": "varna je prázdná", "sk": "SK je prázdná", "wp": "vířivka je prázdná", "spilka": "spilka je prázdná", "lv": "ledovka je prázdná"}

var main: Control
var sp: SimPanel
var sim: BrewSim
var plan: PlanData
var save_path: String = SAVE_PATH
var mode: int = 0  # 0 výuka, 1 samostatně, 2 zkouška
var w: Dictionary = {}  # ovládací prvky
var tiles: Dictionary = {}
var pumps_ui: Dictionary = {}  # id čerpadla -> {btn, spin, read, lamp, name}
var pump_ids: Array = []  # C1–C4
var lv_pump: String = ""  # C5
var reads: Dictionary = {}  # id čerpadla -> poslední stav pro rozvaděč (_pump_read)
var log_box: RichTextLabel
var _list: VBoxContainer
var _kind_btns: Array = []  # [tlačítko, typ, druh] – stroje z plánu
var _route_btns: Array = []  # [tlačítko, cesta]
var _route_keys: Array = []
var _route_sig: String = ""
var _held: Dictionary = {}  # cesty, které otevřel rozvaděč (napouštění, sprcha, glykol, souběžně…)
var _said: Dictionary = {}
var _timers: Dictionary = {}  # id čerpadla -> {dry, shut, last}
var _stir_ck: CheckButton
var _acc: float = 0.0
var _read_acc: float = 0.0
var _save_acc: float = 0.0
var _last_clock: float = 0.0
var _log_sig: String = ""
var _lid_was: bool = false
var _spent_auto: bool = false
var _secs: Array = []  # [tělo, nadpis, název] každé sekce
var scroll_pin: float = -1.0  # jen pro snímky obrazovky: drží posuv sekcí (0 = nahoře, 1 = dole)
# stav vrstvy rozvaděče – ukládá se do user://rt20_state.json spolu s časem várky
var water: Dictionary = WATER_DEFAULT.duplicate()
var lactic: float = 0.0  # mL kyseliny mléčné 80 % v rmutu
var fer: Dictionary = {}
var cip: Dictionary = {}
var faults: Dictionary = {"on": false, "next": 0.0, "active": []}
var errors: Array = []  # chyby a poruchy pro vyhodnocení
var step_hooks: Array = []  # funkce volané v každém kroku simulace s dt (s simulace)
var stuck: Dictionary = {}  # zadřená klapka {id, open}
var fill: Dictionary = {"on": false}  # napouštění varny s předvolbou objemu a teploty
var dose: Dictionary = {"on": false}  # sypání sladu s předvolbou dávky
var ibu_end: float = 0.0  # hořkost mladiny na konci chmelovaru
var fm_set: Dictionary = {}  # id čerpadla -> průtok nastavený na měniči (L/min), když ho uživatel změnil
var recipe: int = 0  # zvolená receptura (recipes.gd): zásyp, voda, rozpis chmele a cíle várky
var pitch: Dictionary = {"l": 0.0, "h": 0.0, "min": 99.0, "max": -99.0}  # zakvašovací teplota (vážený průměr, rozsah)


func setup(m: Control, p: SimPanel) -> void:
	main = m
	sp = p
	sim = p.sim
	plan = p.plan
	_fer_reset()
	_cip_reset()
	for spec in PUMPS:
		var pid := _pump_by_bind(str(spec[0]))
		if pid != "":
			pump_ids.append(pid)
	lv_pump = _pump_by_bind("plv")
	theme = _theme()
	add_theme_stylebox_override("panel", _sb(Color(0.06, 0.07, 0.09, 0.92), C_LINE, 8))
	anchor_left = 1.0
	anchor_right = 1.0
	anchor_bottom = 1.0
	offset_top = 96.0
	offset_bottom = -12.0
	_place(-10.0)
	_build()
	_last_clock = sim.clock
	_lid_was = sim.tank_lid
	_load()


func _pump_by_bind(bind: String) -> String:
	for it in plan.items:
		if str(it.get("type")) == "pump" and str(it.get("bind", "")) == bind:
			return str(it.id)
	return ""


func _place(right: float) -> void:
	offset_right = right
	offset_left = right - WIDTH


## vpravo u okraje; když je tam otevřený checklist, průhledítko nebo vlastnosti z editoru, posune se vedle nich
func _dock() -> void:
	var right := -10.0
	if main != null:
		for nm in ["checklist_panel", "_sight_panel", "props", "skrin"]:
			var p: Variant = main.get(nm)
			if p != null and is_instance_valid(p) and p is Control and (p as Control).visible and (p as Control).anchor_left >= 1.0:
				right = minf(right, (p as Control).offset_left - 10.0)
	if absf(offset_right - right) > 0.5:
		_place(right)


# ---------- vzhled (jako RT.20 v původním trenažéru) ----------
func _sb(col: Color, border: Color = C_LINE, rad: int = 6, bw: int = 1) -> StyleBoxFlat:
	var s := StyleBoxFlat.new()
	s.bg_color = col
	s.border_color = border
	s.set_border_width_all(bw)
	s.set_corner_radius_all(rad)
	s.content_margin_left = 7
	s.content_margin_right = 7
	s.content_margin_top = 4
	s.content_margin_bottom = 4
	return s


func _flat(col: Color) -> StyleBoxFlat:
	var s := StyleBoxFlat.new()
	s.bg_color = col
	s.set_corner_radius_all(2)
	return s


func _theme() -> Theme:
	var t := Theme.new()
	t.default_font_size = 13
	t.set_color("font_color", "Label", C_TXT)
	t.set_stylebox("normal", "Button", _sb(Color(0.16, 0.19, 0.23)))
	t.set_stylebox("hover", "Button", _sb(Color(0.2, 0.24, 0.3), C_ACC))
	t.set_stylebox("pressed", "Button", _sb(Color(0.1, 0.35, 0.2), C_OK))
	t.set_stylebox("hover_pressed", "Button", _sb(Color(0.12, 0.4, 0.24), C_OK))
	t.set_stylebox("disabled", "Button", _sb(Color(0.12, 0.13, 0.15)))
	t.set_stylebox("focus", "Button", _sb(Color(0, 0, 0, 0), Color(0, 0, 0, 0)))
	t.set_color("font_color", "Button", C_TXT)
	t.set_color("font_pressed_color", "Button", Color(0.75, 1, 0.8))
	t.set_color("font_hover_pressed_color", "Button", Color(0.75, 1, 0.8))
	t.set_stylebox("normal", "LineEdit", _sb(Color(0.05, 0.06, 0.08), C_LINE, 4))
	t.set_stylebox("focus", "LineEdit", _sb(Color(0.05, 0.06, 0.08), C_ACC, 4))
	t.set_color("font_color", "LineEdit", Color(0.6, 1, 0.7))
	t.set_stylebox("panel", "PanelContainer", _sb(C_PANEL, C_LINE, 8))
	t.set_stylebox("normal", "OptionButton", _sb(Color(0.16, 0.19, 0.23)))
	t.set_stylebox("hover", "OptionButton", _sb(Color(0.2, 0.24, 0.3), C_ACC))
	t.set_stylebox("pressed", "OptionButton", _sb(Color(0.16, 0.19, 0.23), C_ACC))
	t.set_stylebox("focus", "OptionButton", _sb(Color(0, 0, 0, 0), Color(0, 0, 0, 0)))
	t.set_color("font_color", "OptionButton", C_TXT)
	return t


func _btn(t: String, parent: Node) -> Button:
	var b := Button.new()
	b.text = t
	b.focus_mode = Control.FOCUS_NONE
	parent.add_child(b)
	return b


func _toggle(parent: Node, t: String, cb: Callable) -> Button:
	var b := _btn(t, parent)
	b.toggle_mode = true
	b.toggled.connect(cb)
	return b


func _row(parent: Node) -> HBoxContainer:
	var h := HBoxContainer.new()
	h.add_theme_constant_override("separation", 5)
	parent.add_child(h)
	return h


func _lamp(parent: Node) -> ColorRect:
	var c := ColorRect.new()
	c.custom_minimum_size = Vector2(12, 12)
	c.size_flags_vertical = Control.SIZE_SHRINK_CENTER
	c.color = C_OFF
	parent.add_child(c)
	return c


func _spin(parent: Node, lo: float, hi: float, step: float, val: float, suffix: String = "") -> SpinBox:
	var s := SpinBox.new()
	s.min_value = lo
	s.max_value = hi
	s.step = step
	s.value = val
	s.suffix = suffix
	s.custom_minimum_size.x = 100
	parent.add_child(s)
	return s


func _lbl(parent: Node, t: String = "", col: Color = C_TXT, size: int = 13) -> Label:
	var l := Label.new()
	l.text = t
	l.add_theme_color_override("font_color", col)
	l.add_theme_font_size_override("font_size", size)
	parent.add_child(l)
	return l


## popisek pod ovládáním: šedý, zalamuje se
func _info(parent: Node) -> Label:
	var l := _lbl(parent, "", C_DIM, 12)
	l.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	l.custom_minimum_size.x = 366
	return l


## sekce rozvaděče – klik na nadpis ji sbalí / rozbalí
func _sec(t: String) -> VBoxContainer:
	var pc := PanelContainer.new()
	_list.add_child(pc)
	var outer := VBoxContainer.new()
	outer.add_theme_constant_override("separation", 4)
	pc.add_child(outer)
	var hd := Button.new()
	hd.flat = true
	hd.alignment = HORIZONTAL_ALIGNMENT_LEFT
	hd.focus_mode = Control.FOCUS_NONE
	hd.add_theme_color_override("font_color", C_WARN)
	hd.add_theme_color_override("font_hover_color", Color(1, 0.9, 0.5))
	hd.add_theme_font_size_override("font_size", 13)
	outer.add_child(hd)
	var vb := VBoxContainer.new()
	vb.add_theme_constant_override("separation", 4)
	outer.add_child(vb)
	vb.visible = t in OPEN_SECS
	var upd := func() -> void: hd.text = ("▾ " if vb.visible else "▸ ") + t
	upd.call()
	hd.pressed.connect(func() -> void:
		vb.visible = not vb.visible
		upd.call())
	_secs.append([vb, hd, t])
	return vb


## rozbalí / sbalí všechny sekce (snímky obrazovky)
func expand_all(on: bool) -> void:
	for s in _secs:
		(s[0] as Control).visible = on
		(s[1] as Button).text = ("▾ " if on else "▸ ") + str(s[2])


## přepínač stroje z plánu (šoupě, vzduch, kopačka, kopyto, výpusť, hadice, víko) – stejný prvek jako klik ve scéně
func _kind_toggle(parent: Node, t: String, type: String, kind: String) -> Button:
	var b := _toggle(parent, t, func(on: bool) -> void: _set_kind(type, kind, on, ""))
	_kind_btns.append([b, type, kind])
	if sp._kind_item(type, kind).is_empty():
		b.disabled = true
		b.tooltip_text = "Prvek není v plánu"
	return b


## přepínač cesty: otevře její klapky a pustí čerpadlo, ostatní cesty nechá běžet
func _route_toggle(parent: Node, t: String, rn: String) -> Button:
	var b := _toggle(parent, t, func(on: bool) -> void:
		if on:
			_route_on(rn, true)
		else:
			_route_off(rn, true))
	_route_btns.append([b, rn])
	if not sim.routes.has(rn):
		b.disabled = true
	return b


func _tile(title: String, parent: Node) -> Dictionary:
	var pc := PanelContainer.new()
	pc.custom_minimum_size = Vector2(124, 66)
	pc.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	parent.add_child(pc)
	var vb := VBoxContainer.new()
	vb.add_theme_constant_override("separation", 0)
	pc.add_child(vb)
	var h := HBoxContainer.new()
	h.add_theme_constant_override("separation", 4)
	vb.add_child(h)
	var lamp := _lamp(h)
	_lbl(h, title, C_DIM, 10)
	var t := _lbl(vb, "--.- °C", Color(0.5, 1, 0.6), 17)
	var v := _lbl(vb, "", C_TXT, 11)
	v.clip_text = true
	var bar := ProgressBar.new()
	bar.max_value = 100.0
	bar.show_percentage = false
	bar.custom_minimum_size = Vector2(0, 5)
	bar.add_theme_stylebox_override("background", _flat(Color(0.05, 0.06, 0.08)))
	bar.add_theme_stylebox_override("fill", _flat(Color(0.25, 0.55, 0.85)))
	vb.add_child(bar)
	return {"t": t, "v": v, "lamp": lamp, "bar": bar}


# ---------- stavba panelu ----------
func _build() -> void:
	var root := VBoxContainer.new()
	root.add_theme_constant_override("separation", 6)
	add_child(root)
	var hb := HBoxContainer.new()
	hb.add_theme_constant_override("separation", 8)
	root.add_child(hb)
	_lbl(hb, "RT.20 · ROZVADĚČ", C_WARN, 16)
	w["clock"] = _lbl(hb, "", Color(0.5, 1, 0.65), 16)
	var spacer := Control.new()
	spacer.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	hb.add_child(spacer)
	var close := _btn("✕", hb)
	close.tooltip_text = "Skrýt rozvaděč (vrátí ho tlačítko Rozvaděč RT.20 nahoře)"
	close.pressed.connect(func() -> void: visible = false)
	var mr := _row(root)
	_lbl(mr, "Režim", C_DIM, 12)
	var md := OptionButton.new()
	md.focus_mode = Control.FOCUS_NONE
	md.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	for m in MODES:
		md.add_item(str(m))
	md.item_selected.connect(func(i: int) -> void:
		mode = i
		_log("Režim rozvaděče: " + str(MODES[i])))
	mr.add_child(md)
	w["mode"] = md
	# displeje nádob se signálkami
	var grid := GridContainer.new()
	grid.columns = 3
	grid.add_theme_constant_override("h_separation", 5)
	grid.add_theme_constant_override("v_separation", 5)
	root.add_child(grid)
	for t in [["rvp", "VARNA"], ["sk", "SCEZOVAČKA"], ["wp", "VÍŘIVKA"], ["hlt", "BOJLER"], ["lv", "LEDOVKA"], ["spilka", "SPILKA"]]:
		tiles[t[0]] = _tile(str(t[1]), grid)
	var sc := ScrollContainer.new()
	sc.horizontal_scroll_mode = ScrollContainer.SCROLL_MODE_DISABLED
	sc.size_flags_vertical = Control.SIZE_EXPAND_FILL
	root.add_child(sc)
	w["scroll"] = sc
	_list = VBoxContainer.new()
	_list.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	_list.add_theme_constant_override("separation", 6)
	sc.add_child(_list)
	_build_boiler()
	_build_kettle()
	_build_lauter()
	_build_pumps()
	_build_ice()
	_build_routes()
	_build_water()
	_build_ferment()
	_build_cip()
	_build_faults()
	_build_cast()
	_build_log()


func _build_boiler() -> void:
	var s := _sec("NAPÁJENÍ · KOTEL")
	var r := _row(s)
	w["lamp_boiler"] = _lamp(r)
	w["boiler"] = _toggle(r, "Parní kotel", func(on: bool) -> void:
		sim.steam_on = on
		_log("Parní kotel %s" % ("zapnut – pára jde do plášťů" if on else "vypnut – pára nejde, ohřev stojí")))
	w["steam"] = _lbl(r, "", C_DIM, 12)


func _build_kettle() -> void:
	var s := _sec("RMUTOMLADINOVÁ PÁNEV")
	var r := _row(s)
	_lbl(r, "Pára do pláště")
	var heat := HSlider.new()
	heat.min_value = 0.0
	heat.max_value = 100.0
	heat.step = 5.0
	heat.focus_mode = Control.FOCUS_NONE
	heat.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	heat.size_flags_vertical = Control.SIZE_SHRINK_CENTER
	heat.value_changed.connect(_on_heat)
	r.add_child(heat)
	w["heat"] = heat
	var hl := _lbl(r, "0 %")
	hl.custom_minimum_size.x = 96
	w["heat_l"] = hl
	r = _row(s)
	w["auto"] = _toggle(r, "Auto na", func(on: bool) -> void:
		sim.auto_mode["rvp"] = on
		if not on:
			sim.prog_run = false
			sim.heater_kw["rvp"] = 0.0
		_sync_left()
		_log("Termostat varny: %s" % (("AUTO na %.1f °C" % float(sim.setpoint["rvp"])) if on else "vypnut")))
	var setp := _spin(r, 20.0, 100.0, 0.5, float(sim.setpoint["rvp"]), " °C")
	setp.value_changed.connect(func(v: float) -> void:
		sim.setpoint["rvp"] = v
		_sync_left())
	w["set"] = setp
	w["stir"] = _toggle(r, "Míchadlo", func(on: bool) -> void:
		sim.stir["rvp"] = on
		_sync_left()
		_log("Míchadlo varny: %s" % ("zapnuto" if on else "vypnuto")))
	r = _row(s)
	_lbl(r, "Napustit")
	var fv := _spin(r, 100.0, 5000.0, 50.0, 2000.0, " L")
	var ft := _spin(r, 10.0, 85.0, 1.0, 38.0, " °C")
	ft.custom_minimum_size.x = 82
	w["fill"] = _toggle(r, "▶", func(on: bool) -> void:
		if on:
			_fill_start(fv.value, ft.value)
		else:
			_fill_stop("Napouštění varny zastaveno ručně."))
	r = _row(s)
	_lbl(r, "Slad")
	var mkg := _spin(r, 10.0, 1200.0, 5.0, BrewSim.MALT_BATCH, " kg")
	w["malt"] = _toggle(r, "Sypat", func(on: bool) -> void:
		if on:
			_dose_start(mkg.value)
		else:
			_dose_stop("Sypání sladu zastaveno ručně."))
	w["malt_l"] = _lbl(r, "", C_DIM, 12)
	r = _row(s)
	_kind_toggle(r, "Vzduch", "valve", "air")
	_kind_toggle(r, "Šoupě šneku", "valve", "gate")
	_kind_toggle(r, "Voda do šneku", "valve", "screwwater")
	r = _row(s)
	_btn("Jódová zkouška", r).pressed.connect(_iodine)
	_btn("Chmel: další dávka", r).pressed.connect(_hop_next)
	w["hop_info"] = _info(s)
	w["rvp_info"] = _info(s)


func _build_lauter() -> void:
	var s := _sec("SCEZOVACÍ KÁĎ")
	var r := _row(s)
	_kind_toggle(r, "Kopačka", "pump", "rake")
	_btn("▲ zvednout", r).pressed.connect(func() -> void: _set_kind("valve", "rakelift", false, "Kopačka jede nahoru."))
	_btn("▼ spustit", r).pressed.connect(func() -> void: _set_kind("valve", "rakelift", true, "Kopačka jede dolů do lože."))
	r = _row(s)
	_kind_toggle(r, "Kopyto dole", "valve", "hoof")
	_kind_toggle(r, "Výpusť do kanálu", "valve", "drain")
	r = _row(s)
	_btn("Hluboké kypření", r).pressed.connect(_deep_rake)
	_btn("Výhoz mláta", r).pressed.connect(_spent_out)
	r = _row(s)
	_lbl(r, "Vyslaz. voda")
	var st := _spin(r, 60.0, 85.0, 1.0, float(sim.setpoint["hlt"]), " °C")
	st.custom_minimum_size.x = 82
	st.tooltip_text = "Teplota vyslazovací vody = cíl termostatu bojleru"
	st.value_changed.connect(func(v: float) -> void:
		sim.setpoint["hlt"] = v
		_sync_left())
	w["sT"] = st
	var sf := _spin(r, 20.0, 480.0, 5.0, _route_lpm("vyslazovani"), " L/min")
	sf.tooltip_text = "Průtok sprchou (čerpadlo C4)"
	sf.value_changed.connect(func(v: float) -> void: _set_route_lpm("vyslazovani", v))
	w["sF"] = sf
	_route_toggle(r, "▶", "vyslazovani")
	w["sk_info"] = _info(s)


func _build_pumps() -> void:
	var s := _sec("ČERPADLA (frekvenční měniče)")
	for pid in pump_ids:
		var it: Dictionary = plan.get_item(str(pid))
		var nm := str(it.get("label", pid))
		var role := ""
		for spec in PUMPS:
			if str(spec[0]) == str(it.get("bind", "")):
				role = str(spec[1])
		var r := _row(s)
		var lamp := _lamp(r)
		var b := _toggle(r, "%s %s" % [nm, role], func(on: bool) -> void: _pump_toggle(str(pid), on))
		b.custom_minimum_size.x = 132
		b.alignment = HORIZONTAL_ALIGNMENT_LEFT
		var spn := _spin(r, 10.0, float(_curve(str(pid))["Qn"]) * 1.6, 5.0, _pump_set(str(pid)), " L/min")
		spn.custom_minimum_size.x = 112
		spn.tooltip_text = "Frekvenční měnič drží nastavený průtok (rotametr) – přidá otáčky až do 50 Hz"
		spn.value_changed.connect(func(v: float) -> void: _pump_set_flow(str(pid), v))
		var rd := _info(s)
		var r2 := _row(s)
		_btn("Odvzdušnit", r2).pressed.connect(func() -> void: _vent(str(pid)))
		_btn("Opravit ucpávku", r2).pressed.connect(func() -> void: _fix_seal(str(pid)))
		pumps_ui[pid] = {"btn": b, "spin": spn, "read": rd, "lamp": lamp, "name": nm}
	if pump_ids.is_empty():
		_info(s).text = "V plánu nejsou čerpadla C1–C4 (bind p1, p2, p3, phw)."


func _build_ice() -> void:
	var s := _sec("LEDOVÁ VODA · GLYKOL · ŘAD")
	var r := _row(s)
	w["lamp_lv"] = _lamp(r)
	w["lvp"] = _toggle(r, "C5 ledová", func(on: bool) -> void: _pump_toggle(lv_pump, on))
	_lbl(r, "cíl", C_DIM, 12)
	var lvs := _spin(r, 0.0, 20.0, 0.5, float(sim.setpoint["lv"]), " °C")
	lvs.value_changed.connect(func(v: float) -> void:
		sim.setpoint["lv"] = v
		_sync_left())
	w["lvset"] = lvs
	r = _row(s)
	w["glyk"] = _toggle(r, "Agregát glykol", _glycol)
	_route_toggle(r, "Ledová do chladiče", "ledovka")
	r = _row(s)
	_route_toggle(r, "Spoušť z řadu → ledovka", "lv_fill")
	w["lv_info"] = _info(s)


func _build_routes() -> void:
	var s := _sec("CESTY · KLAPKY")
	var r := _row(s)
	var ob := OptionButton.new()
	ob.focus_mode = Control.FOCUS_NONE
	ob.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	ob.fit_to_longest_item = false
	ob.clip_text = true
	r.add_child(ob)
	w["route"] = ob
	_fill_routes()
	r = _row(s)
	_btn("◀ předchozí", r).pressed.connect(func() -> void: _route_walk(-1))
	_btn("další ▶", r).pressed.connect(func() -> void: _route_walk(1))
	r = _row(s)
	_btn("Nastavit", r).pressed.connect(func() -> void: _route_set(false))
	_btn("+ souběžně", r).pressed.connect(func() -> void: _route_set(true))
	_btn("Zavřít vše", r).pressed.connect(_close_all)
	w["route_msg"] = _info(s)
	w["flowing"] = _info(s)


func _build_water() -> void:
	var s := _sec("VODA · pH RMUTU")
	var r := _row(s)
	_lbl(r, "Alkalita")
	var alk := _spin(r, 0.0, 400.0, 5.0, float(water["alk"]), " mg/L")
	alk.value_changed.connect(func(v: float) -> void: water["alk"] = v)
	r = _row(s)
	_lbl(r, "Ca")
	var ca := _spin(r, 0.0, 300.0, 5.0, float(water["ca"]), " mg/L")
	ca.value_changed.connect(func(v: float) -> void: water["ca"] = v)
	_lbl(r, "Mg")
	var mg := _spin(r, 0.0, 100.0, 1.0, float(water["mg"]), " mg/L")
	mg.value_changed.connect(func(v: float) -> void: water["mg"] = v)
	w["water_spins"] = [alk, ca, mg]
	r = _row(s)
	_lbl(r, "Kys. mléčná 80 %")
	var lac := _spin(r, 0.0, 3000.0, 50.0, 0.0, " mL")
	_btn("Přidat", r).pressed.connect(func() -> void:
		lactic += lac.value
		_log("Přidáno %d mL kyseliny mléčné 80 %% (celkem %d mL)." % [int(lac.value), int(lactic)]))
	w["ph"] = _info(s)


func _build_ferment() -> void:
	var s := _sec("SPILKA · KVAŠENÍ")
	var r := _row(s)
	_lbl(r, "Teplota kvašení")
	var fs := _spin(r, 4.0, 20.0, 0.5, float(fer["set"]), " °C")
	fs.value_changed.connect(func(v: float) -> void: fer["set"] = v)
	w["fset"] = fs
	_btn("Zakvasit", r).pressed.connect(_fer_start)
	r = _row(s)
	_lbl(r, "Kvasnice")
	var pt := _spin(r, 5.0, 30.0, 1.0, float(fer["pitch"]), " mil./mL")
	pt.custom_minimum_size.x = 120
	pt.value_changed.connect(func(v: float) -> void: fer["pitch"] = v)
	w["fpitch"] = pt
	w["fer"] = _info(s)


func _build_cip() -> void:
	var s := _sec("SANITACE CIP")
	var r := _row(s)
	var tg := OptionButton.new()
	tg.focus_mode = Control.FOCUS_NONE
	for k in CIP_TARGETS:
		tg.add_item(str(CIP_TARGETS[k]))
	r.add_child(tg)
	var md := OptionButton.new()
	md.focus_mode = Control.FOCUS_NONE
	for m in CIP_MEDIA:
		md.add_item(str(m))
	r.add_child(md)
	r = _row(s)
	_lbl(r, "Teplota")
	var cs := _spin(r, 10.0, 90.0, 1.0, float(cip["set"]), " °C")
	cs.value_changed.connect(func(v: float) -> void: cip["set"] = v)
	w["cipB"] = _toggle(r, "▶ Spustit", func(on: bool) -> void:
		cip["run"] = on
		if on:
			cip["target"] = str(CIP_TARGETS.keys()[tg.selected])
			cip["medium"] = str(CIP_MEDIA[md.selected])
			cip["set"] = cs.value
			cip["min"] = 0.0
			_said.erase("lyecold")
			_log("CIP: %s – %s, %d °C." % [str(CIP_TARGETS[cip["target"]]), str(cip["medium"]), int(cip["set"])])
		else:
			_log("CIP zastaven."))
	w["cip"] = _info(s)


func _build_faults() -> void:
	var s := _sec("NÁCVIK PORUCH")
	var r := _row(s)
	w["faults"] = _toggle(r, "Náhodné poruchy", func(on: bool) -> void:
		faults["on"] = on
		faults["next"] = 0.0
		_log("Náhodné poruchy " + ("zapnuty – sleduj průtoky, tlaky a teploty." if on else "vypnuty.")))
	r = _row(s)
	_btn("Reset kotle", r).pressed.connect(func() -> void: _fix("steam"))
	_btn("Vyčistit chladič", r).pressed.connect(func() -> void: _fix("plate"))
	_btn("Uvolnit klapku", r).pressed.connect(func() -> void: _fix("valve"))
	_info(s).text = "Porucha čerpadla: zavzdušnění → Odvzdušnit, spálená ucpávka → Opravit ucpávku (sekce Čerpadla)."


func _build_cast() -> void:
	var s := _sec("SPÍLÁNÍ · PROTOKOL")
	var r := _row(s)
	_kind_toggle(r, "Hadice na tanku", "valve", "hose")
	w["vratka"] = _toggle(r, "Klapka vratky", _set_vratka)
	_kind_toggle(r, "Víko spilky", "valve", "lid")
	r = _row(s)
	w["push"] = _toggle(r, "Protláčka (voda do sání C3)", _push)
	_btn("Vyhodnocení", r).pressed.connect(_report)
	w["cast"] = _info(s)
	w["report"] = _info(s)


func _build_log() -> void:
	var s := _sec("HLÁŠENÍ")
	log_box = RichTextLabel.new()
	log_box.bbcode_enabled = true
	log_box.scroll_following = false
	log_box.custom_minimum_size = Vector2(366, 210)
	log_box.add_theme_stylebox_override("normal", _sb(Color(0.04, 0.05, 0.06), C_LINE, 6))
	log_box.add_theme_font_size_override("normal_font_size", 12)
	s.add_child(log_box)
	var help := _info(s)
	help.add_theme_font_size_override("font_size", 11)
	help.text = "Klik na nadpis = sbalit / rozbalit · Tab = skrýt panely · ovládá stejné klapky a čerpadla jako levý panel a klik ve scéně"


# ---------- běh ----------
func _process(delta: float) -> void:
	if sim == null:
		return
	_dock()
	var now: float = sim.clock
	if now + 1.0 < _last_clock:
		_new_batch()  # Nová várka: čas simulace začal znovu
	var dt: float = now - _last_clock
	_last_clock = now
	_read_acc += delta
	if _read_acc >= 0.25:
		_read_acc = 0.0
		_update_reads()
	if dt > 0.0:
		_step(dt)
	_stuck_step()
	_acc += delta
	if _acc >= 0.25 and visible:
		_acc = 0.0
		_refresh()
	if scroll_pin >= 0.0:
		var sc: ScrollContainer = w["scroll"]
		sc.scroll_vertical = int(scroll_pin * sc.get_v_scroll_bar().max_value)
	_save_acc += delta
	if _save_acc > 20.0:
		_save_acc = 0.0
		save_state()


func _notification(what: int) -> void:
	if what == NOTIFICATION_WM_CLOSE_REQUEST and sim != null:
		save_state()


func _update_reads() -> void:
	for pid in pump_ids:
		reads[pid] = _pump_read(str(pid))
	if lv_pump != "":
		reads[lv_pump] = _pump_read(lv_pump)
	# měnič vyslazovacího čerpadla C2 = kolik chce odtáhnout ze scezovacího lože (z toho se lože utahuje nebo trhá)
	var p2: String = _pump_by_bind("p2")
	if p2 != "":
		sim.sk_demand_lpm = _pump_set(p2)
	# pH rmutu (voda, tmavé slady, kyselina mléčná) řídí aktivitu amyláz v simulaci rmutování
	var ph: float = _mash_ph()
	sim.mash_ph = ph
	sim.ph_a = exp(-pow((ph - 5.6) / 0.45, 2.0))
	sim.ph_b = exp(-pow((ph - 5.4) / 0.40, 2.0))


## modely rozvaděče v čase simulace (dt = s simulace)
func _step(dt: float) -> void:
	for h in step_hooks:  # např. měření prodlev pro okno protokolu (protokol_okno.gd)
		(h as Callable).call(dt)
	_fill_step()
	_dose_step()
	_guard(dt)
	_faults_step()
	_fer_step(dt)
	_cip_step(dt)
	_track()
	_spent_step()
	_boiler_warn()
	_ph_warn()
	if sim.tank_lid and not _lid_was and not bool(fer["on"]) and float(sim.vessels["spilka"]["v"]) > 100.0:
		_log("Spilka zavíčkovaná – zakvašuje se.")
		_fer_start()
	_lid_was = sim.tank_lid


func _new_batch() -> void:
	lactic = 0.0
	_fer_reset()
	_cip_reset()
	faults["active"] = []
	faults["next"] = 0.0
	errors.clear()
	stuck = {}
	sim.pump_fault.clear()
	sim.steam_bar = BrewSim.STEAM_BAR
	sim.steam_p = BrewSim.STEAM_BAR
	sim.plate_eff = BrewSim.PLATE_EFF
	ibu_end = 0.0
	sim.malt_stock = batch_kg()  # nová várka začíná se zásobou sladu zvolené receptury
	sim.enz_mean = Recipes.enz_mean(_recipe())
	sim.recipe_ebc = Recipes.model_ebc(_recipe())  # barva z použitého sladu (původní model), ne z tabulky
	pitch = {"l": 0.0, "h": 0.0, "min": 99.0, "max": -99.0}
	fill = {"on": false}
	dose = {"on": false}
	_held.clear()
	_timers.clear()
	_said.clear()
	_spent_auto = false
	_lid_was = sim.tank_lid
	if w.has("report"):
		(w["report"] as Label).text = ""


func _log(text: String) -> void:
	sp._log(text)


func _say_once(key: String, text: String) -> void:
	if _said.has(key):
		return
	_said[key] = true
	sp._log(text)


## hlášení nejvýš jednou za slot_s sekund simulace
func _say_slot(key: String, slot_s: float, text: String) -> void:
	_say_once("%s@%d" % [key, int(sim.clock / slot_s)], text)


## levý panel ukazuje totéž: termostaty, cíle a míchadlo
func _sync_left() -> void:
	for key in sp._auto_ck:
		(sp._auto_ck[key] as CheckButton).set_pressed_no_signal(bool(sim.auto_mode[key]))
	for key in sp._sp_spin:
		_spin_sync(sp._sp_spin[key], float(sim.setpoint[key]))
	if _stir_ck == null:
		_stir_ck = _find_stir(sp)
	if _stir_ck != null:
		_stir_ck.set_pressed_no_signal(bool(sim.stir.get("rvp", false)))


func _find_stir(n: Node) -> CheckButton:
	for c in n.get_children():
		if c is CheckButton and str((c as CheckButton).text).begins_with("Míchadlo"):
			return c
		var f := _find_stir(c)
		if f != null:
			return f
	return null


func _spin_sync(s: SpinBox, v: float) -> void:
	if s.is_inside_tree() and s.get_line_edit().has_focus():
		return  # obsluha zrovna píše číslo
	if absf(s.value - v) > 0.001:
		s.set_value_no_signal(v)


# ---------- cesty, klapky a stroje ----------
func _rlabel(rn: String) -> String:
	return str(sim.routes.get(rn, {}).get("label", rn))


func _route_open(rn: String) -> bool:
	var ids: Array = sp.route_valve_ids(rn)
	if ids.is_empty():
		return false
	for vid in ids:
		if not bool(plan.get_item(str(vid)).get("open", false)):
			return false
	var p := sim.route_pump(rn)
	return p == "" or bool(plan.get_item(p).get("on", false))


## otevře klapky cesty a pustí její čerpadlo (ostatní cesty nechá) – jako „+ souběžně“ v původním RT.20
func _route_on(rn: String, log_it: bool) -> void:
	_held[rn] = true
	for vid in sp.route_valve_ids(rn):
		var it: Dictionary = plan.get_item(str(vid))
		if not it.is_empty():
			sp._set_item(it, "open", true)
	var p := sim.route_pump(rn)
	if p != "" and not plan.get_item(p).is_empty():
		sp._set_item(plan.get_item(p), "on", true)
	if log_it:
		_log("Otevřena cesta: " + _rlabel(rn))


## zavře klapky cesty, které nepotřebuje jiná tekoucí nebo držená cesta; čerpadlo zastaví, když ho nic jiného nepotřebuje
func _route_off(rn: String, log_it: bool) -> void:
	_held.erase(rn)
	var keep: Dictionary = {}
	var pumps: Dictionary = {}
	for r2 in sim.routes:
		if str(r2) == rn or not (_held.has(r2) or bool(sp._flowing.get(r2, false))):
			continue
		for vid in sp.route_valve_ids(str(r2)):
			keep[str(vid)] = true
		var p2 := sim.route_pump(str(r2))
		if p2 != "":
			pumps[p2] = true
	for vid in sp.route_valve_ids(rn):
		var it: Dictionary = plan.get_item(str(vid))
		if not it.is_empty() and not keep.has(str(vid)):
			sp._set_item(it, "open", false)
	var p := sim.route_pump(rn)
	if p != "" and not pumps.has(p) and not plan.get_item(p).is_empty():
		sp._set_item(plan.get_item(p), "on", false)
	if log_it:
		_log("Zavřena cesta: " + _rlabel(rn))


func _route_lpm(rn: String) -> float:
	return float(sim.routes.get(rn, {}).get("lpm", 40.0)) * BrewSim.FLOW_SCALE


func _set_route_lpm(rn: String, v: float) -> void:
	if sim.routes.has(rn):
		sim.routes[rn]["lpm"] = v / BrewSim.FLOW_SCALE
		sp._pq_cache.clear()


func _routes_sig() -> String:
	var s := ""
	for rn in sim.routes:
		if not bool(sim.routes[rn].get("synthetic", false)):
			s += str(rn) + str(sim.routes[rn]["label"]) + ";"
	return s


func _fill_routes() -> void:
	var ob: OptionButton = w["route"]
	var keep := ""
	if ob.selected >= 0 and ob.selected < _route_keys.size():
		keep = str(_route_keys[ob.selected])
	ob.clear()
	_route_keys.clear()
	var groups: Array = []
	for g in SimPanel.ROUTE_GROUPS:
		groups.append([g[0], []])
	groups.append(["Ostatní", []])
	for rn in sim.routes:
		var r: Dictionary = sim.routes[rn]
		if bool(r.get("synthetic", false)):
			continue
		if str(r["from"]) == "" and str(r["to"]) == "" and not (rn in ["glykol", "ledovka"]):
			continue  # rozpracovaná trasa z editoru bez nádob
		var gi: int = groups.size() - 1
		for k in SimPanel.ROUTE_GROUPS.size():
			if (SimPanel.ROUTE_GROUPS[k][1] as Array).has(rn):
				gi = k
		(groups[gi][1] as Array).append(str(rn))
	for k in SimPanel.ROUTE_GROUPS.size():  # v rámci skupiny pořadí podle ROUTE_GROUPS (zhruba pořadí várky)
		var ordered: Array = []
		for rn in SimPanel.ROUTE_GROUPS[k][1]:
			if (groups[k][1] as Array).has(rn):
				ordered.append(rn)
		groups[k][1] = ordered
	for g in groups:  # nadpis skupiny je v seznamu nevolitelný oddělovač, v _route_keys má prázdný klíč
		if (g[1] as Array).is_empty():
			continue
		ob.add_separator(str(g[0]))
		_route_keys.append("")
		for rn in g[1]:
			_route_keys.append(str(rn))
			ob.add_item(str(sim.routes[rn]["label"]))
	var i := _route_keys.find(keep) if keep != "" else -1
	if i >= 0:
		ob.select(i)
	_route_sig = _routes_sig()


## ◀ ▶ : vybere předchozí / další cestu v pořadí skupin a rovnou ji nastaví (jen tuhle)
func _route_walk(dir: int) -> void:
	var ob: OptionButton = w["route"]
	var n := _route_keys.size()
	if n == 0:
		return
	var i: int = ob.selected if ob.selected >= 0 else (-1 if dir > 0 else n)
	for _k in n:
		i = posmod(i + dir, n)
		if str(_route_keys[i]) != "":
			ob.select(i)
			_route_set(false)
			return


func _route_set(add: bool) -> void:
	var ob: OptionButton = w["route"]
	if ob.selected < 0 or ob.selected >= _route_keys.size():
		return
	var rn: String = _route_keys[ob.selected]
	if rn == "":
		return
	if add:
		_route_on(rn, false)
		_log("Souběžně nastavena cesta: " + _rlabel(rn))
	else:
		_fill_stop("")
		_held.clear()
		sp._set_route(rn)  # jen tahle cesta: ostatní klapky zavře (stroje nechá)
	var pump := sim.route_pump(rn)
	w["route_sel"] = rn
	w["route_txt"] = "%s: %d klapek, %s" % [_rlabel(rn), sp.route_valve_ids(rn).size(), ("čerpadlo " + str(plan.get_item(pump).get("label", pump))) if pump != "" else "bez čerpadla (řad nebo samospád)"]


func _close_all() -> void:
	_fill_stop("")
	_dose_stop("")
	_held.clear()
	sp.close_all()


func _set_kind(type: String, kind: String, on: bool, msg: String) -> void:
	var it: Dictionary = sp._kind_item(type, kind)
	if it.is_empty():
		return
	var key := "on" if type == "pump" else "open"
	if bool(it.get(key, false)) == on:
		return
	sp._set_item(it, key, on)
	_log(msg if msg != "" else "%s: %s" % [str(it.get("label", kind)), _state_word(type, kind, on)])


func _state_word(type: String, kind: String, on: bool) -> String:
	if type == "pump":
		return "běží" if on else "stojí"
	match kind:
		"hoof":
			return "dole" if on else "nahoře"
		"rakelift":
			return "jede dolů" if on else "jede nahoru"
		"hose":
			return "na tanku" if on else "na vratce"
		"lid":
			return "zavřené" if on else "otevřené"
	return "otevřeno" if on else "zavřeno"


func _vratka_id() -> String:
	return str(sp._kind_item("valve", "hose").get("vratka_valve", ""))


func _set_vratka(on: bool) -> void:
	var vi: Dictionary = plan.get_item(_vratka_id())
	if vi.is_empty():
		_log("⚠ Klapka vratky není v plánu (u spílací hadice chybí vratka_valve).")
		return
	sp._set_item(vi, "open", on)
	_log("Klapka vratky %s." % ("otevřená" if on else "zavřená"))


## protláčka: kohout vody z řadu a klapky na odbočce k sání C3 (pole via u kohoutu)
func _push(on: bool) -> void:
	var pw: Dictionary = sp._kind_item("valve", "pushwater")
	if pw.is_empty():
		_log("⚠ Voda na protláčku není v plánu.")
		return
	sp._set_item(pw, "open", on)
	for via in pw.get("via", []):
		var vi: Dictionary = plan.get_item(str(via))
		if not vi.is_empty():
			sp._set_item(vi, "open", on)
	_log("Protláčka: voda z řadu do sání C3 %s" % ("otevřena – C3 musí běžet." if on else "zavřena."))
	var c3: String = sim.route_pump("virivka_chladic")
	if on and mode == 0 and c3 != "" and not bool(plan.get_item(c3).get("on", false)):
		_log("⚠ Pusť C3 – protláčka teče jen s běžícím čerpadlem.")


## agregát glykolu: vypínač u chladiče a okruh ledové vody přes glykolový chladič (C5)
func _glycol(on: bool) -> void:
	for it in plan.items:
		if str(it.get("type")) == "sw":
			sp._set_item(it, "on", on)
	if on:
		_route_on("glykol", false)
	else:
		_route_off("glykol", false)
	_log("Agregát glykolu %s" % ("zapnut – C5 žene ledovou vodu přes glykolový chladič." if on else "vypnut."))


# ---------- pánev: pára, napouštění, slad, jód, chmel ----------
func _on_heat(v: float) -> void:
	if sim.prog_run:
		sim.prog_run = false
		_log("Rmutovací program zastaven – pára ručně.")
	sim.auto_mode["rvp"] = false
	sim.heater_kw["rvp"] = v / 100.0 * maxf(float(sim.max_kw["rvp"]), 1.0)
	_sync_left()


## napouštění varny s předvolbou: studená z řadu a horká z bojleru (C4), horkou obsluha přivírá podle teploty v kotli
func _fill_start(v_add: float, t_set: float) -> void:
	var rv: Dictionary = sim.vessels["rvp"]
	var hl: Dictionary = sim.vessels["hlt"]
	var th: float = float(hl["t"])
	var want_hot: bool = t_set > sim.cal_mains_t + 1.0
	var hot: bool = want_hot and float(hl["v"]) > 50.0 and sim.routes.has("voda_panev")
	var cold: bool = (t_set < th - 1.0 or not hot) and sim.routes.has("voda_rad")
	if not hot and not cold:
		_log("⚠ Varna nemá cestu vody (Voda z bojleru / z řadu).")
		return
	if want_hot and not hot:
		_log("⚠ Bojler je prázdný – napouští se jen studená voda z řadu.")
	elif hot and t_set > th + 0.5:
		_log("⚠ Bojler má jen %.1f °C – voda %d °C nebude, zvyš teplotu bojleru." % [th, int(t_set)])
	fill = {"on": true, "target": float(rv["v"]) + v_add, "t": t_set, "mix": hot and cold, "hot": hot, "cold": cold, "hot_open": hot}
	if hot:
		_route_on("voda_panev", false)
	if cold:
		_route_on("voda_rad", false)
	var how := ("bojler %.0f °C + řad %.0f °C" % [th, sim.cal_mains_t]) if hot and cold else ("jen z bojleru" if hot else "jen z řadu")
	_log("Napouštění varny: %d L, %d °C (%s)." % [int(v_add), int(t_set), how])


func _fill_step() -> void:
	if not bool(fill.get("on", false)):
		return
	var rv: Dictionary = sim.vessels["rvp"]
	if float(rv["v"]) >= float(fill["target"]) - 0.5:
		_fill_stop("Varna: napuštěno %.0f L vody, %.1f °C." % [float(rv["v"]), float(rv["t"])])
		return
	# cesty zavřel někdo jiný (Zavřít vše, klik na klapku, jiná cesta) – napouštění končí
	if (bool(fill["cold"]) and not _route_open("voda_rad")) or (bool(fill["hot_open"]) and not _route_open("voda_panev")):
		_fill_stop("Napouštění varny přerušeno – cesta vody je zavřená.")
		return
	if bool(fill["mix"]) and float(rv["v"]) > 30.0:
		var t: float = float(rv["t"])
		var tt: float = float(fill["t"])
		if bool(fill["hot_open"]) and t > tt + 0.6:
			_route_off("voda_panev", false)
			fill["hot_open"] = false
		elif not bool(fill["hot_open"]) and t < tt - 0.6 and float(sim.vessels["hlt"]["v"]) > 50.0:
			_route_on("voda_panev", false)
			fill["hot_open"] = true
	if bool(fill["hot_open"]) and float(sim.vessels["hlt"]["v"]) < 20.0:
		_say_once("fill_hlt", "⚠ Bojler došel – horká voda neteče.")


func _fill_stop(msg: String) -> void:
	if not bool(fill.get("on", false)):
		return
	fill["on"] = false
	for rn in ["voda_panev", "voda_rad"]:
		if _held.has(rn):
			_route_off(rn, false)
	_said.erase("fill_hlt")
	if msg != "":
		_log(msg)


## sypání sladu s předvolbou: šnek běží, dokud nenasype dávku (slad padá jen přes otevřené šoupě se vzduchem)
func _dose_start(kg: float) -> void:
	var screw: Dictionary = sp._kind_item("pump", "screw")
	if screw.is_empty():
		_log("⚠ Šnek na slad není v plánu.")
		return
	if sim.malt_stock <= 0.5:
		_log("⚠ Zásobník sladu je prázdný.")
		return
	dose = {"on": true, "stock0": sim.malt_stock, "kg": kg}
	sp._set_item(screw, "on", true)
	_log("Sypání sladu: dávka %d kg (v zásobníku %d kg)." % [int(kg), int(sim.malt_stock)])
	if mode == 0 and not bool(sp._kind_item("valve", "gate").get("open", false)):
		_log("⚠ Šnek běží, ale šoupě je zavřené – pusť vzduch a otevři šoupě, jinak slad nepadá.")


func _dose_step() -> void:
	if not bool(dose.get("on", false)):
		return
	var screw: Dictionary = sp._kind_item("pump", "screw")
	if screw.is_empty() or not bool(screw.get("on", false)):
		dose["on"] = false  # šnek vypnul někdo jiný
		return
	var done: float = float(dose["stock0"]) - sim.malt_stock
	if done >= float(dose["kg"]) - 0.01 or sim.malt_stock <= 0.0:
		_dose_stop("Nasypáno %d kg sladu – šnek zastaven." % int(round(done)))


func _dose_stop(msg: String) -> void:
	if not bool(dose.get("on", false)):
		return
	dose["on"] = false
	var screw: Dictionary = sp._kind_item("pump", "screw")
	if not screw.is_empty():
		sp._set_item(screw, "on", false)
	if msg != "":
		_log(msg)


func _iodine() -> void:
	var m: float = float(sim.vessels["rvp"]["malt"]) + float(sim.vessels["sk"]["malt"])
	if m < 0.5:
		_log("Jódová zkouška: v nádobě není rmut – není z čeho dělat zkoušku.")
		return
	var s: float = sim.starch_left()
	var r := "žlutohnědá – škrob úplně zcukřený (jódově normální) ✔"
	if s >= 15.0:
		r = "modročerná – škrob nezcukřený! Drž 72 °C a zkoušku opakuj"
	elif s >= 5.0:
		r = "slabě fialová – ještě zbývá trochu škrobu, prodluž cukrotvornou prodlevu"
	_log("Jódová zkouška (talířek, kapka jódu): %s" % r)


## minuta chmelovaru (-1 = nevře)
func _boil_min() -> float:
	var rv: Dictionary = sim.vessels["rvp"]
	if float(rv.get("boil", 0.0)) <= 0.05:
		return -1.0
	return float(rv.get("boil_s", 0.0)) / 60.0


func _recipe() -> Dictionary:
	return Recipes.RECIPES[clampi(recipe, 0, Recipes.RECIPES.size() - 1)]


func _hop_plan() -> Array:
	return Recipes.hop_plan(_recipe())


## dávka sladu zvolené receptury (kg)
func batch_kg() -> float:
	return Recipes.total_kg(_recipe())


## výběr receptury: jen před začátkem vystírky (zásyp, voda a rozpis chmele se mění s ní)
func select_recipe(i: int) -> bool:
	i = clampi(i, 0, Recipes.RECIPES.size() - 1)
	if i == recipe:
		return true
	var started: bool = float(sim.vessels["rvp"]["malt"]) + float(sim.vessels["sk"]["malt"]) > 1.0 or sim.malt_stock < batch_kg() - 0.5
	if started and mode != 2:
		_log("⚠ Recepturu jde změnit jen před sypáním sladu (už se sypalo). Spusť Novou várku.")
		return false
	recipe = i
	var r: Dictionary = _recipe()
	sim.malt_stock = Recipes.total_kg(r)
	sim.enz_mean = Recipes.enz_mean(r)
	sim.recipe_ebc = Recipes.model_ebc(r)
	_log("Receptura: %s – %s; voda %d L; cíl %.1f °P, %d IBU, %d EBC." % [str(r["name"]), Recipes.grist_text(r), int(Recipes.water_l(r)), float(r["P"]), int(r["IBU"]), int(r["EBC"])])
	_log("Chmel: " + Recipes.hops_text(r))
	return true


func _hop_next() -> void:
	var rv: Dictionary = sim.vessels["rvp"]
	var i: int = (rv.get("hops", []) as Array).size()
	var plan_h: Array = _hop_plan()
	if i >= plan_h.size():
		_log("⚠ Všechny dávky chmele podle rozpisu už jsou v mladině.")
		return
	var h: Dictionary = plan_h[i]
	var bm := _boil_min()
	if bm < 0.0:
		_log("⚠ Pozor: mladina ještě nevře – chmel bez varu se neizomeruje.")
	elif absf(bm - float(h["min"])) > 5.0:
		_log("⚠ Dávka %d měla jít v %d. min varu, teď je %d. min." % [i + 1, int(h["min"]), int(bm)])
	sim.add_hops(float(h["kg"]) * 1000.0, float(h["a"]))
	_log("Chmel %d. dávka: %.1f kg (α %d %%) do varny." % [i + 1, float(h["kg"]), int(h["a"])])


# ---------- scezovací káď ----------
func _deep_rake() -> void:
	if not bool(sim.vessels["sk"].get("bed", false)):
		_log("Hluboké kypření: v SK zatím není filtrační lože – není co kypřit.")
		return
	_set_kind("valve", "hoof", false, "")
	_set_kind("pump", "rake", true, "")
	_set_kind("valve", "rakelift", true, "")
	_log("Hluboké kypření: kopačka se otáčí a jede dolů do lože – lože povolí, sladina se na chvíli zakalí. Po nakypření kopačku zvedni (▲).")


func _spent_out() -> void:
	var sk: Dictionary = sim.vessels["sk"]
	if float(sk["malt"]) + float(sk.get("res", 0.0)) < 0.5:
		_log("Výhoz mláta: v SK není mláto.")
		return
	if sim.plato("sk") > 1.0 and mode != 2:
		_log("⚠ Výhoz mláta BLOKOVÁNO: v SK je ještě sladina (%.1f °P) – mláto se vyhrnuje až po scezení a vyslazení." % sim.plato("sk"))
		return
	_set_kind("valve", "hoof", true, "")
	_set_kind("pump", "rake", true, "")
	_spent_auto = true
	_log("Výhoz mláta: kopyto dole, kopačka se otáčí – mláto jde mlátovým otvorem ven.")


func _spent_step() -> void:
	if not _spent_auto:
		return
	var sk: Dictionary = sim.vessels["sk"]
	if not sim.hoof_down or not sim.rake_on:
		_spent_auto = false  # obsluha to převzala
	elif float(sk["malt"]) + float(sk.get("res", 0.0)) <= 0.01:
		_spent_auto = false
		_set_kind("pump", "rake", false, "")
		_set_kind("valve", "hoof", false, "")
		_log("Výhoz mláta hotový – kopačka zastavena, kopyto nahoře.")


# ---------- čerpadla s frekvenčními měniči ----------
func _curve(pid: String) -> Dictionary:
	var curves: Dictionary = NetFlowScript.PUMP_CURVES
	var bind := str(plan.get_item(pid).get("bind", ""))
	return (curves.get(bind, {"Qn": 200.0, "H0": 25.0}) as Dictionary).merged(sim.pump_cal.get(bind, {}), true)  # kalibrace.json přepíše H0 a Qn


## nastavený průtok čerpadla (L/min): medián jeho cest (bez odtahu ze SK, ten určuje lože)
func _pump_set(pid: String) -> float:
	if fm_set.has(pid):
		return float(fm_set[pid])
	var l: Array = []
	for rn in sim.routes:
		var r: Dictionary = sim.routes[rn]
		if str(r.get("pump", "")) == pid and str(r.get("from", "")) != "sk" and float(r.get("lpm", 0.0)) > 0.0 and not bool(r.get("synthetic", false)):
			l.append(float(r["lpm"]) * BrewSim.FLOW_SCALE)
	if l.is_empty():
		return float(_curve(pid)["Qn"]) * 0.75
	l.sort()
	return float(l[int(l.size() / 2.0)])


## frekvenční měnič: všechny cesty čerpadla poběží nastaveným průtokem (pokud ho křivka a potrubí dovolí)
func _pump_set_flow(pid: String, v: float) -> void:
	fm_set[pid] = v  # měnič si hodnotu pamatuje i pro čerpadlo bez vlastních tras (C2 u scezování)
	for rn in sim.routes:
		var r: Dictionary = sim.routes[rn]
		if str(r.get("pump", "")) == pid and not (rn in ["glykol", "ledovka"]):
			r["lpm"] = v / BrewSim.FLOW_SCALE
	sp._pq_cache.clear()
	sp._want_tick = -100


func _pump_toggle(pid: String, on: bool) -> void:
	var it: Dictionary = plan.get_item(pid)
	if it.is_empty():
		return
	var nm := str(it.get("label", pid))
	if on and mode != 2:
		var why := _interlock(pid)
		if why != "":
			_log("⚠ %s BLOKOVÁNO: %s" % [nm, why])
			_refresh_pump_buttons()
			return
	sp._set_item(it, "on", on)
	var set_txt := ""
	if on and pumps_ui.has(pid):
		set_txt = " (nastaveno %d L/min)" % int((pumps_ui[pid]["spin"] as SpinBox).value)
	_log("%s %s%s." % [nm, "start" if on else "stop", set_txt])


## blokování startu (Výuka, Samostatně): bez otevřeného sání s kapalinou nebo bez otevřeného výtlaku čerpadlo nepustí
func _interlock(pid: String) -> String:
	var net = sp.net
	if net == null:
		return ""
	if not net.pump_nodes.has(pid):
		return "čerpadlo není napojené na potrubí v plánu."
	if not _suction_flooded(pid):
		return "sání je zavřené nebo nádoba prázdná – čerpadlo by běželo nasucho (spálí ucpávku, nasaje vzduch)."
	var dst := false
	for h in net.bfs(net.pump_nodes[pid]["out"])["hits"]:
		if str(h["ves"]["key"]) != "mains":
			dst = true
	if not dst:
		return "výtlak je zavřený – tlak by vyletěl proti klapce (ráz v potrubí, přehřátí čerpadla)."
	return ""


## v sání je kapalina: otevřená cesta do nádoby pod hladinou s obsahem (nebo voda na protláčku u C3)
func _suction_flooded(pid: String) -> bool:
	var net = sp.net
	if pid == sim.route_pump("virivka_chladic") and bool(sp._kind_item("valve", "pushwater").get("open", false)):
		return true
	if net == null or not net.pump_nodes.has(pid):
		return false
	for h in net.bfs(net.pump_nodes[pid]["in"])["hits"]:
		if bool(h["ves"]["low"]) and float(net.vessel_avail(sim, str(h["ves"]["key"]))) > 5.0:
			return true
	return false


func _vent(pid: String) -> void:
	var nm := str(plan.get_item(pid).get("label", pid))
	if str(sim.pump_fault.get(pid, "")) != "air":
		_log("%s: z odvzdušňovacího ventilu teče kapalina – čerpadlo vzduch nemá." % nm)
		return
	if _suction_flooded(pid):
		sim.pump_fault.erase(pid)
		_resolve_air(pid)
		_log("%s odvzdušněno – čerpadlo znovu nasálo." % nm)
	else:
		_log("⚠ %s: odvzdušňovák jen syčí – sání je prázdné nebo zavřené, čerpadlo nemá co nasát." % nm)


func _fix_seal(pid: String) -> void:
	var nm := str(plan.get_item(pid).get("label", pid))
	if str(sim.pump_fault.get(pid, "")) != "broken":
		_log("%s: ucpávka je těsná, oprava není potřeba." % nm)
		return
	sim.pump_fault.erase(pid)
	_timers.erase(pid)
	sim.clock += 2.0 * 3600.0
	_last_clock = sim.clock  # dvě hodiny prostoje: kvašení ani CIP se kvůli tomu nepřepočítávají
	_log("⚠ %s: vyměněna ucpávka – várka stála 2 h." % nm)
	errors.append({"t": sim.clock, "msg": "%s: zničená ucpávka (chod nasucho nebo proti zavřenému výtlaku), 2 h prostoj" % nm})


## co trasa teď doopravdy přečerpá (L/min) – stejně jako simulace: rmut ze SK pevně, sladina ze SK podle lože
func _route_q(rn: String) -> float:
	var r: Dictionary = sim.routes.get(rn, {})
	if r.is_empty() or rn in ["glykol", "ledovka"]:
		return 0.0
	var src := str(r["from"])
	if sim.vessels.has(src):
		var sv: Dictionary = sim.vessels[src]
		if float(sv["v"]) < 0.5 and float(sv["malt"]) < 1.0:
			return 0.0
	var q: float = float(r["lpm"]) * BrewSim.FLOW_SCALE
	if src == "sk":
		if bool(sim.vessels["sk"].get("bed", false)):
			q = sim.sk_draw_lpm()  # sladina: kolik pustí lože (a méně podle měniče C2)
		else:
			q = minf(q, 80.0 * BrewSim.FLOW_SCALE)  # rmut: kolik nastaví měnič C1, nejvýš plný výkon
	return q * float(sim.flow_scale.get(rn, 1.0))


## stav čerpadla na rozvaděči: stojí / běží (pracovní bod) / nesaje / tlačí do zavřené klapky / porucha
func _pump_read(pid: String) -> Dictionary:
	var o := {"kind": "off", "q": 0.0, "hz": 0.0, "bar": 0.0, "npsh": 0.0, "txt": "stojí"}
	var it: Dictionary = plan.get_item(pid)
	if it.is_empty():
		o["kind"] = "none"
		o["txt"] = "není v plánu"
		return o
	if not bool(it.get("on", false)):
		return o
	var f := str(sim.pump_fault.get(pid, ""))
	if f == "broken":
		o["kind"] = "broken"
		o["txt"] = "PORUCHA – spálená ucpávka, čerpadlo nečerpá"
		return o
	if f == "air":
		o["kind"] = "air"
		o["hz"] = 50.0
		o["bar"] = 0.05
		o["txt"] = "ZAVZDUŠNĚNO – točí se, ale nečerpá, odvzdušni"
		return o
	if pid == sim.route_pump("virivka_chladic") and sim.push_flow:
		o["kind"] = "run"
		o["q"] = _route_lpm("virivka_chladic")
		o["txt"] = "protláčka: řad → chladič → hadice"
		return o
	var net = sp.net
	if net == null:
		o["kind"] = "run"
		o["txt"] = "běží"
		return o
	var cv := _curve(pid)
	for n in sp.net_notes:
		if str(n["pump"]) != pid:
			continue
		o["hz"] = 50.0
		match str(n["what"]):
			"zavřeno":
				o["kind"] = "shut"
				o["bar"] = 1000.0 * 9.81 * float(cv["H0"]) / 1.0e5  # uzavírací tlak při plných otáčkách
				o["txt"] = "tlačí do zavřené klapky – nic neteče, čerpadlo se hřeje"
			"nesaje":
				o["kind"] = "dry"
				o["txt"] = "nesaje – sání je zavřené, běží nasucho"
			_:
				o["kind"] = "dry"
				o["txt"] = "nesaje – sání nevede pod hladinu"
		return o
	var fl: Dictionary = {}
	for f2 in sp.net_info:
		if str(f2.get("pump", "")) == pid:
			fl = f2
			break
	if fl.is_empty():
		var wired: bool = net.pump_nodes.has(pid)
		o["kind"] = "run" if wired else "none"
		o["txt"] = "běží" if wired else "čerpadlo není napojené na potrubí v plánu"
		return o
	var src := str(fl["src"])
	if src != "mains" and float(net.vessel_avail(sim, src)) < 5.0:
		o["kind"] = "dry"
		o["hz"] = 50.0
		o["txt"] = "%s – čerpadlo běží naprázdno" % str(EMPTY.get(src, src + " je prázdná"))
		return o
	var names: Array = []
	for d in fl["dsts"]:
		var nm := str(VNAME.get(str(d["key"]), d["key"]))
		if not names.has(nm):
			names.append(nm)
	var q := 0.0
	for rn in sp._flowing:
		if bool(sp._flowing[rn]) and sim.route_pump(str(rn)) == pid:
			q += _route_q(str(rn))
	var hp := _hydraulics(fl, q)
	o["kind"] = "run"
	o["q"] = q
	o["hz"] = hp["hz"]
	o["bar"] = hp["bar"]
	o["npsh"] = hp["npsh"]
	o["txt"] = "%s → %s" % [str(VNAME.get(src, src)), ", ".join(names)]
	if float(hp["npsh"]) < NPSH_R and q > 1.0:
		o["txt"] = str(o["txt"]) + " · KAVITACE"
	elif bool(hp["full"]):
		o["txt"] = str(o["txt"]) + " · plné otáčky"
	return o


## pracovní bod čerpadla s frekvenčním měničem při průtoku q (L/min): otáčky, tlak na výtlaku a NPSH v sání.
## Odpor potrubí jako net_flow.pump_q: výška zdroj → nejvyšší cíl, tření podle délky a průměru, kolena a klapky.
func _hydraulics(fl: Dictionary, q: float) -> Dictionary:
	var net = sp.net
	var pid := str(fl["pump"])
	var cv := _curve(pid)
	var h0: float = float(cv["H0"])
	var ap: float = 0.3 * h0 / (float(cv["Qn"]) * float(cv["Qn"]))
	var sn: Dictionary = net.nodes[int(fl["src_k"])]
	var key := str(fl["src"])
	var hsrc: float = maxf(0.0, (float(sn["y"]) - float(net.level_y(sim, key))) / 100.0)
	var dz := -99.0
	var d_edges: Array = []
	for d in fl["dsts"]:
		dz = maxf(dz, (float(sn["y"]) - float(net.nodes[int(d["k"])]["y"])) / 100.0)
		d_edges.append_array(d["edges"])
	if dz < -98.0:
		dz = 0.0
	var vs: Dictionary = sim.vessels.get(key, {})
	var phi := 0.0
	var rho := 1000.0
	if not vs.is_empty() and float(vs["v"]) > 1.0:
		phi = clampf(float(vs.get("malt", 0.0)) * 0.65 / float(vs["v"]), 0.0, 0.55)
		rho = 1000.0 + 4.0 * sim.plato(key) + 300.0 * phi  # mladina je hustší, rmut se šrotem ještě víc
	var fr: float = 0.03 * exp(9.0 * phi * 0.6)  # hustý rmut se slupkami: větší tření
	var hs: float = _head_loss(net, fl["spath"], q, fr, 0.5)
	var hd: float = _head_loss(net, d_edges, q, fr, 1.0)
	var hsys: float = dz - hsrc + hs + hd
	var n: float = clampf(sqrt((maxf(0.0, hsys) + ap * q * q) / h0), 0.0, 1.0)
	var hp: float = h0 * n * n - ap * q * q
	var pit: Dictionary = plan.get_item(pid)
	var zs: float = (float(pit.get("y", sn["y"])) - float(sn["y"])) / 100.0  # čerpadlo pod hrdlem nádoby = zaplavené sání
	var t: float = float(vs.get("t", sim.cal_mains_t)) if not vs.is_empty() else sim.cal_mains_t
	var p_suct: float = rho * 9.81 * (hsrc + zs - hs)
	var npsh: float = (101325.0 - _pvap(t)) / (rho * 9.81) + hsrc + zs - hs
	return {"hz": 50.0 * n, "bar": maxf(0.0, (p_suct + rho * 9.81 * hp) / 1.0e5), "npsh": npsh, "full": n >= 0.999}


## tlaková ztráta (m) v potrubí po hranách sítě: tření podle délky a nejužšího průměru, kolena 0,9, klapky 0,3, vstup/výstup k_end
func _head_loss(net, edge_ids: Array, q: float, fr: float, k_end: float) -> float:
	var len_m := 0.0
	var dn_mm := 200.0
	var kl := 0.0
	var last_dir := Vector2.ZERO
	var seen: Dictionary = {}
	for ei in edge_ids:
		if seen.has(ei):
			continue
		seen[ei] = true
		var e: Dictionary = net.edges[int(ei)]
		len_m += float(e["len"]) / 100.0
		dn_mm = minf(dn_mm, float(e["dn"]))
		var na: Dictionary = net.nodes[int(e["a"])]
		var nb: Dictionary = net.nodes[int(e["b"])]
		var dir := (Vector2(float(nb["x"]), float(nb["y"])) - Vector2(float(na["x"]), float(na["y"]))).normalized()
		if last_dir != Vector2.ZERO and absf(dir.dot(last_dir)) < 0.7:
			kl += 0.9
		last_dir = dir
		if str(na["valve"]) != "" or str(nb["valve"]) != "":
			kl += 0.3
	var dm: float = clampf(dn_mm, DN_MIN, 199.0) / 1000.0
	var v: float = q / 60000.0 / (PI * dm * dm / 4.0)
	return (fr * len_m / dm + kl + k_end) * v * v / 19.62


## tlak nasycených par vody (Pa) – Antoineova rovnice
func _pvap(t: float) -> float:
	return 133.322 * pow(10.0, 8.07131 - 1730.63 / (233.426 + clampf(t, 1.0, 100.0)))


## chod nasucho a proti zavřenému výtlaku: varování; v režimu Zkouška prázdná nádoba čerpadlo zavzdušní a chod
## nasucho (5 min) nebo proti zavřené klapce (15 min) zničí ucpávku
func _guard(dt: float) -> void:
	for pid in pump_ids:
		var kind := str(reads.get(pid, {}).get("kind", "off"))
		var nm := str(plan.get_item(str(pid)).get("label", pid))
		var tm: Dictionary = _timers.get(pid, {"dry": 0.0, "shut": 0.0, "last": "off"})
		_timers[pid] = tm
		if kind == "dry" or kind == "air":
			tm["dry"] = float(tm["dry"]) + dt
			if kind == "dry" and str(tm["last"]) == "run" and mode == 2 and not sim.pump_fault.has(pid):
				sim.pump_fault[pid] = "air"
				_log("⚠ %s: do sání šel vzduch – čerpadlo ztratilo nasátí a běží naprázdno. Vypni ho, napusť nádobu a odvzdušni." % nm)
				errors.append({"t": sim.clock, "msg": "%s nechané běžet do prázdné nádoby (zavzdušnění)" % nm})
			if float(tm["dry"]) > 60.0:
				_say_once("dry" + str(pid), "⚠ %s běží nasucho – mechanická ucpávka se bez kapaliny přehřívá! Vypni čerpadlo." % nm)
			if float(tm["dry"]) > 300.0 and mode == 2 and str(sim.pump_fault.get(pid, "")) != "broken":
				sim.pump_fault[pid] = "broken"
				_log("⚠ %s: chod nasucho zničil ucpávku – čerpadlo teče a nečerpá. Oprava trvá asi 2 h." % nm)
				errors.append({"t": sim.clock, "msg": "%s: chod nasucho zničil ucpávku" % nm})
		elif kind == "shut":
			tm["shut"] = float(tm["shut"]) + dt
			if float(tm["shut"]) > 120.0:
				_say_once("shut" + str(pid), "⚠ %s běží proti zavřenému výtlaku – kapalina v čerpadle se ohřívá, hrozí poškození ucpávky a ráz při otevření klapky." % nm)
			if float(tm["shut"]) > 900.0 and mode == 2 and str(sim.pump_fault.get(pid, "")) != "broken":
				sim.pump_fault[pid] = "broken"
				_log("⚠ %s: přehřáté čerpadlo – ucpávka netěsní, čerpadlo nečerpá." % nm)
				errors.append({"t": sim.clock, "msg": "%s: chod proti zavřenému výtlaku zničil ucpávku" % nm})
		else:
			tm["dry"] = 0.0
			tm["shut"] = 0.0
			_said.erase("dry" + str(pid))
			_said.erase("shut" + str(pid))
		tm["last"] = kind


# ---------- nácvik poruch ----------
func _faults_step() -> void:
	if not bool(faults["on"]):
		return
	if float(faults["next"]) <= 0.0:
		faults["next"] = sim.clock + randf_range(1800.0, 4800.0)
	if sim.clock < float(faults["next"]):
		return
	faults["next"] = sim.clock + randf_range(2400.0, 6000.0)
	var act: Array = faults["active"]
	match str(FAULTS[randi() % FAULTS.size()]):
		"air":
			var cand: Array = []
			for pid in pump_ids:
				var rd: Dictionary = reads.get(pid, {})
				if str(rd.get("kind", "")) == "run" and float(rd.get("q", 0.0)) > 5.0:
					cand.append(pid)
			if not cand.is_empty():
				var p: String = str(cand[randi() % cand.size()])
				sim.pump_fault[p] = "air"
				act.append({"k": "air", "p": p, "t": sim.clock})
				_log("⚠ %s: průtok náhle spadl, tlak na výtlaku skoro nulový…" % str(plan.get_item(p).get("label", p)))
		"steam":
			if sim.steam_bar >= BrewSim.STEAM_BAR - 0.01:
				sim.steam_bar = 1.0
				act.append({"k": "steam", "t": sim.clock})
				_log("⚠ Kotel: tlak páry klesá (teď %.1f bar)…" % sim.steam_p)
		"plate":
			if sim.plate_eff >= BrewSim.PLATE_EFF - 0.001:
				sim.plate_eff = 0.75  # zanesený chladič: za chladičem je tepleji, nic se nehlásí
				act.append({"k": "plate", "t": sim.clock})
		"valve":
			if stuck.is_empty():
				var vs: Array = []
				for it in plan.items:
					if str(it.get("type")) == "valve" and str(it.get("kind", "")) == "klapka":
						vs.append(it)
				if not vs.is_empty():
					var v: Dictionary = vs[randi() % vs.size()]
					stuck = {"id": str(v.id), "open": bool(v.get("open", false))}
					act.append({"k": "valve", "id": str(v.id), "t": sim.clock})


func _fix(k: String) -> void:
	var fixed := ""
	var act: Array = faults["active"]
	for f in act.duplicate():
		if str(f["k"]) != k:
			continue
		match k:
			"steam":
				sim.steam_bar = BrewSim.STEAM_BAR
				fixed = "Kotel: odkalení a reset hořáku – tlak páry zpět na %.1f bar." % sim.steam_bar
			"plate":
				sim.plate_eff = BrewSim.PLATE_EFF
				fixed = "Chladič rozebrán a vyčištěn (CIP) – přestup tepla obnoven."
			"valve":
				stuck = {}
				fixed = "Zadřená klapka uvolněna a promazána."
		act.erase(f)
		errors.append({"t": sim.clock, "msg": "Porucha vyřešena za %d min: %s" % [int((sim.clock - float(f["t"])) / 60.0), k]})
	if fixed == "":
		_log("⚠ Zásah bez účinku – tahle závada tu není. Zkontroluj znovu průtoky, tlaky a teploty.")
		errors.append({"t": sim.clock, "msg": "Špatná diagnóza poruchy (%s)" % k})
	else:
		_log(fixed)


func _resolve_air(pid: String) -> void:
	var act: Array = faults["active"]
	for f in act.duplicate():
		if str(f["k"]) == "air" and str(f.get("p", "")) == pid:
			act.erase(f)
			errors.append({"t": sim.clock, "msg": "Zavzdušněné %s vyřešeno za %d min" % [str(plan.get_item(pid).get("label", pid)), int((sim.clock - float(f["t"])) / 60.0)]})


## zadřená klapka se nedá přestavit (klik ve scéně ani cesta) – vrací se do polohy, ve které se zadřela
func _stuck_step() -> void:
	if stuck.is_empty():
		return
	var it: Dictionary = plan.get_item(str(stuck["id"]))
	if it.is_empty():
		stuck = {}
		return
	if bool(it.get("open", false)) != bool(stuck["open"]):
		sp._set_item(it, "open", bool(stuck["open"]))
		if mode == 0:
			_say_once("stuck", "⚠ Jedna klapka se nepřestavila – pohon hlásí chybu (zadřená? → Nácvik poruch: Uvolnit klapku).")


# ---------- kotel, pH, kvašení, CIP ----------
func _boiler_warn() -> void:
	var demand: float = float(sim.heater_kw.get("hlt", 0.0)) + float(sim.heater_kw.get("rvp", 0.0))
	if not sim.steam_on and demand > 0.5:
		_say_once("noboil", "⚠ Kotel je vypnutý – pára nejde, ohřev stojí.")
	else:
		_said.erase("noboil")
	if sim.steam_on and demand > sim.cal_boiler_kw * 1.02:
		_say_slot("boilmax", 1800.0, "⚠ Kotel nestačí (%d kW požadováno, %d kW max) – ohřev všude zpomalí." % [int(demand), int(sim.cal_boiler_kw)])


func _residual_alk() -> float:
	return float(water["alk"]) - (float(water["ca"]) * 0.714 + float(water["mg"]) * 0.585)


## pH rmutu (Kolbach: zbytková alkalita; světlý slad; zjednodušeno jako v původním trenažéru)
func _mash_ph() -> float:
	var rv: Dictionary = sim.vessels["rvp"]
	var sk: Dictionary = sim.vessels["sk"]
	var v := 0.0
	if float(rv["malt"]) > 1.0:
		v += float(rv["v"])
	if float(sk["malt"]) > 1.0:
		v += float(sk["v"])
	v = maxf(500.0, v)
	return clampf(5.75 - 0.35 * Recipes.dark_factor(_recipe()) + 0.0017 * _residual_alk() - 0.1 * (lactic / (0.05 * v)), 4.6, 6.4)


func _ph_warn() -> void:
	var m: float = float(sim.vessels["rvp"]["malt"]) + float(sim.vessels["sk"]["malt"])
	var ph := _mash_ph()
	if mode == 2 or m < 100.0 or (ph >= 5.2 and ph <= 5.8):
		if ph >= 5.2 and ph <= 5.8:
			_said.erase("ph")
		return
	_say_once("ph", "⚠ pH rmutu %.2f je mimo optimum 5,4–5,6 – ve skutečné várce by amylázy pracovaly pomaleji a zcukření by se protáhlo. Uprav vodu nebo přidej kyselinu mléčnou." % ph)


func _fer_reset() -> void:
	var st: float = float(fer.get("set", 9.0))
	var pt: float = float(fer.get("pitch", 15.0))
	fer = {"on": false, "set": st, "T": st, "X": 0.0, "S0": 0.0, "Sf": 0.0, "E": 0.0, "AL": 0.0, "D": 0.0, "t": 0.0, "pitch": pt, "P0": 0.0}


func _fer_start() -> void:
	if bool(fer["on"]):
		_log("Spilka už kvasí.")
		return
	var spv: Dictionary = sim.vessels["spilka"]
	var v: float = float(spv["v"])
	if v < 100.0:
		_log("⚠ Ve spilce je jen %d L – není co zakvasit." % int(v))
		return
	fer["on"] = true
	sim.yeast_pitched = true
	fer["T"] = float(spv["t"])
	fer["X"] = float(fer["pitch"]) * 0.04  # 15 mil. buněk/mL ≈ 0,6 g/L sušiny
	fer["S0"] = float(spv["ext"]) * sim.fermentable("spilka") / v * 1000.0  # zkvasitelný extrakt (g/L): podíl cukrů z rmutování (enzymy, pH, teploty prodlev)
	fer["Sf"] = fer["S0"]
	fer["E"] = 0.0
	fer["AL"] = 0.0
	fer["D"] = 0.0
	fer["t"] = 0.0
	fer["P0"] = sim.plato("spilka")
	_log("Zakvašeno: %.1f hl, %.1f °C, %.0f mil. buněk/mL. Spilka chlazená na %.1f °C." % [v / 100.0, float(fer["T"]), float(fer["pitch"]), float(fer["set"])])


## kvašení ve spilce (zjednodušený model: Monod + teplotní závislost, α-acetolaktát → diacetyl → redukce kvasinkami)
func _fer_step(dt: float) -> void:
	if not bool(fer["on"]):
		return
	var h: float = dt / 3600.0
	var tt: float = float(fer["T"])
	var x: float = float(fer["X"])
	var sf: float = float(fer["Sf"])
	var a: float = exp(0.095 * (tt - 12.0))  # aktivita kvasinek vůči 12 °C
	var mu: float = 0.035 * a * sf / (8.0 + sf) * (1.0 - x / 4.0)  # růst kvasinek /h (Xmax ≈ 4 g/L)
	var ds: float = minf(sf, (0.11 * a * x * sf / (6.0 + sf)) * h)  # g/L zkvašeného extraktu
	x += maxf(0.0, mu * x * h)
	sf -= ds
	fer["E"] = float(fer["E"]) + ds * 0.485
	tt += ds * 0.6 * 1000.0 / 4100.0 - (tt - float(fer["set"])) * minf(1.0, 0.25 * h)  # teplo z kvašení, chlazení pláštěm
	var al: float = float(fer["AL"]) + mu * x * h * 0.9
	var conv: float = al * minf(1.0, 0.012 * exp(0.11 * (tt - 12.0)) * h)
	al -= conv
	var dd: float = float(fer["D"]) + conv * 1000.0  # µg/L (ppb)
	dd -= dd * minf(1.0, 0.06 * a * minf(1.0, x / 1.5) * h)
	fer["T"] = tt
	fer["X"] = x
	fer["Sf"] = sf
	fer["AL"] = al
	fer["D"] = dd
	fer["t"] = float(fer["t"]) + dt
	sim.vessels["spilka"]["t"] = tt  # spilka chlazená na teplotu kvašení
	var s0: float = float(fer["S0"])
	if dd < 100.0 and sf < 0.25 * s0:
		_say_once("dia_ok", "Diacetyl %.0f ppb – pod prahem chuti (≈100 ppb), kvašení je u konce. Můžeš chladit na ležení." % dd)
	if sf < 0.3 * s0 and dd > 150.0 and float(fer["set"]) < 10.0:
		_say_once("dia_rest", "⚠ Diacetyl %.0f ppb je vysoko a kvašení končí – zvaž diacetylovou pauzu (zvýšit na 12–14 °C na 1–2 dny)." % dd)


## zdánlivý extrakt: ubývá cukr a alkohol snižuje hustotu (Balling)
func _app_extract() -> float:
	var real: float = float(fer["P0"]) - (float(fer["S0"]) - float(fer["Sf"])) / 10.0
	return real - 0.46 * float(fer["E"]) / 10.0


func _cip_reset() -> void:
	cip = {"run": false, "target": "rvp", "medium": "louh", "set": float(cip.get("set", 80.0)), "T": 20.0, "min": 0.0, "done": {}}


## sanitace CIP: teplota roztoku (stanice ohřívá asi 1,2 K/min), čas a médium – bez chemie koncentrací
func _cip_step(dt: float) -> void:
	if not bool(cip["run"]):
		return
	var m: float = dt / 60.0
	var t: float = float(cip["T"]) + clampf(float(cip["set"]) - float(cip["T"]), -1.5 * m, 1.2 * m)
	cip["T"] = t
	cip["min"] = float(cip["min"]) + m
	var med := str(cip["medium"])
	var tgt := str(cip["target"])
	var ok := true
	match med:
		"louh":
			ok = t >= 75.0
			if float(cip["set"]) < 75.0 and float(cip["min"]) > 5.0:
				_say_once("lyecold", "⚠ Louh je nastavený jen na %.0f °C – pod 75 °C nerozpouští bílkovinné úsady a pivní kámen spolehlivě." % float(cip["set"]))
		"kyselina":
			ok = t >= 20.0 and t <= 60.0
			if t > 60.5:
				_say_once("acidhot", "⚠ Kyselina nad 60 °C – riziko koroze nerezu a výparů. Uber teplotu.")
			elif t < 58.0:
				_said.erase("acidhot")
	var key := tgt + ":" + med
	var done: Dictionary = cip["done"]
	if ok:
		done[key] = float(done.get(key, 0.0)) + m
	if med == "louh" and float(done.get(key, 0.0)) >= 20.0 and not done.has(key + "!"):
		done[key + "!"] = 1
		_log("CIP %s: louh ≥ 75 °C 20 min hotovo ✔ – následuje proplach vodou a kyselina." % str(CIP_TARGETS.get(tgt, tgt)))


# ---------- vyhodnocení ----------
## hořkost na konci chmelovaru a zakvašovací teplota mladiny, která natekla do spilky
func _track() -> void:
	var rv: Dictionary = sim.vessels["rvp"]
	if float(rv["v"]) > 500.0 and float(rv.get("boil_s", 0.0)) > 60.0 and not sim.active.has("varna_virivka"):
		ibu_end = sim.ibu()
	var v: float = float(sim.vessels["spilka"]["v"])
	var dv: float = v - float(pitch.get("v_last", v))
	pitch["v_last"] = v
	if dv > 0.01 and sim.hose_tank and sim.cool_out_t > -0.5:
		pitch["l"] = float(pitch["l"]) + dv
		pitch["h"] = float(pitch["h"]) + dv * sim.cool_out_t
		pitch["min"] = minf(float(pitch["min"]), sim.cool_out_t)
		pitch["max"] = maxf(float(pitch["max"]), sim.cool_out_t)


func _report() -> void:
	var spv: Dictionary = sim.vessels["spilka"]
	var v: float = float(spv["v"])
	if v < 10.0:
		_log("⚠ Spilka je prázdná – zatím není co vyhodnotit.")
		return
	var malt: float = maxf(batch_kg() - sim.malt_stock, 1.0)
	var rcp: Dictionary = _recipe()
	var pl: float = float(pitch["l"])
	var t_avg: float = float(pitch["h"]) / pl if pl > 1.0 else float(spv["t"])
	var rng := (" (%.1f–%.1f)" % [float(pitch["min"]), float(pitch["max"])]) if pl > 1.0 else ""
	var txt := "VARNÍ PROTOKOL: %.1f hl, %.2f °P, %.0f IBU, zakvašení %.1f °C%s, výtěžnost %.1f %%, várka %.1f h" % [v / 100.0, sim.plato("spilka"), ibu_end, t_avg, rng, float(spv["ext"]) / malt * 100.0, sim.clock / 3600.0]
	var extra: Array = ["receptura %s (cíl %.1f °P, %d IBU, %d EBC)" % [str(rcp["name"]), float(rcp["P"]), int(rcp["IBU"]), int(rcp["EBC"])]]
	if sim.water_to_tank + sim.push_water_tank > 5.0:
		extra.append("voda z potrubí a protláčky ve spilce %.0f L" % (sim.water_to_tank + sim.push_water_tank))
	if sim.drain_ext > 0.5:
		extra.append("extrakt do kanálu %.1f kg" % sim.drain_ext)
	if not errors.is_empty():
		extra.append("chyby a poruchy %d" % errors.size())
	if not extra.is_empty():
		txt += " · " + ", ".join(extra)
	_log(txt)
	var lines: Array = [txt]
	for i in range(maxi(errors.size() - 5, 0), errors.size()):
		var mm: int = int(float(errors[i]["t"]) / 60.0)
		lines.append("  %d:%02d  %s" % [mm / 60, mm % 60, str(errors[i]["msg"])])
	(w["report"] as Label).text = "\n".join(lines)


# ---------- obnova displejů ----------
func _refresh() -> void:
	var sec: int = int(sim.clock)
	(w["clock"] as Label).text = "⏱ %d:%02d:%02d" % [sec / 3600, (sec / 60) % 60, sec % 60]
	var md: OptionButton = w["mode"]
	if md.selected != mode:
		md.select(mode)
	_sync_left()
	_refresh_tiles()
	_refresh_boiler()
	_refresh_kettle()
	_refresh_lauter()
	_refresh_pump_buttons()
	_refresh_ice()
	_refresh_routes()
	_refresh_water()
	_refresh_ferment()
	_refresh_cip()
	_refresh_cast()
	_refresh_log()
	for kb in _kind_btns:
		var it: Dictionary = sp._kind_item(str(kb[1]), str(kb[2]))
		(kb[0] as Button).set_pressed_no_signal(bool(it.get("on" if str(kb[1]) == "pump" else "open", false)))
	for rb in _route_btns:
		(rb[0] as Button).set_pressed_no_signal(_route_open(str(rb[1])))


func _p_txt(key: String) -> String:
	var p: float = sim.plato(key)
	return ("  %.1f °P" % p) if p > 0.5 else ""


func _set_tile(k: String, t: float, txt: String, frac: float, state: int) -> void:
	var tl: Dictionary = tiles[k]
	(tl["t"] as Label).text = "%.1f °C" % t
	(tl["v"] as Label).text = txt
	(tl["bar"] as ProgressBar).value = clampf(frac, 0.0, 1.0) * 100.0
	(tl["lamp"] as ColorRect).color = [C_OFF, C_OK, C_WARN, C_BAD][state]


func _refresh_tiles() -> void:
	var vs: Dictionary = sim.vessels
	var rv: Dictionary = vs["rvp"]
	var rvol: float = float(rv["v"]) + float(rv["malt"]) * BrewSim.MALT_L_PER_KG
	var boiling: bool = float(rv.get("boil", 0.0)) > 0.05
	var rst := 0
	if rvol > float(rv["cap"]) * 0.97:
		rst = 3
	elif boiling:
		rst = 2
	elif float(sim.heater_kw.get("rvp", 0.0)) > 0.5 and sim.steam_on:
		rst = 1
	_set_tile("rvp", float(rv["t"]), "%d L%s%s" % [int(rvol), _p_txt("rvp"), "  VAŘÍ" if boiling else ""], rvol / float(rv["cap"]), rst)
	var sk: Dictionary = vs["sk"]
	var skv: float = float(sk["v"]) + float(sk["malt"]) * BrewSim.BED_L_PER_KG
	var bed: bool = bool(sk.get("bed", false))
	var sst := 0
	if bed and float(sk.get("pack", 0.0)) > 0.6:
		sst = 3
	elif sim.sk_draw_active():
		sst = 1
	_set_tile("sk", float(sk["t"]), "%d L%s" % [int(skv), ("  lože %.0f cm" % (sim.bed_height(sk) * 100.0)) if bed else _p_txt("sk")], skv / float(sk["cap"]), sst)
	var wp: Dictionary = vs["wp"]
	var wv: float = float(wp["v"])
	_set_tile("wp", float(wp["t"]), "%d L%s" % [int(wv), _p_txt("wp")], wv / float(wp["cap"]), 3 if wv > float(wp["cap"]) * 0.97 else (1 if wv > 5.0 else 0))
	var hl: Dictionary = vs["hlt"]
	_set_tile("hlt", float(hl["t"]), "%d L" % int(hl["v"]), float(hl["v"]) / float(hl["cap"]), 2 if float(hl["v"]) < 1500.0 else 1)
	var lv: Dictionary = vs["lv"]
	_set_tile("lv", float(lv["t"]), "%d L" % int(lv["v"]), float(lv["v"]) / float(lv["cap"]), 2 if float(lv["t"]) > float(sim.setpoint["lv"]) + 3.0 else 1)
	var spv: Dictionary = vs["spilka"]
	_set_tile("spilka", float(spv["t"]), "%d L%s" % [int(spv["v"]), _p_txt("spilka")], float(spv["v"]) / float(spv["cap"]), 1 if float(spv["v"]) > 1.0 else 0)


func _refresh_boiler() -> void:
	(w["boiler"] as Button).set_pressed_no_signal(sim.steam_on)
	(w["lamp_boiler"] as ColorRect).color = C_OK if sim.steam_on else Color(0.3, 0.1, 0.1)
	var demand: float = float(sim.heater_kw.get("hlt", 0.0)) + float(sim.heater_kw.get("rvp", 0.0))
	if sim.steam_on:
		(w["steam"] as Label).text = "%.1f bar · %d °C · zátěž %d %% (%d kW)" % [sim.steam_p, int(sim.steam_t(sim.steam_p)), int(sim.boiler_load * 100.0), int(minf(demand, sim.cal_boiler_kw))]
	else:
		(w["steam"] as Label).text = "0,0 bar · pára nejde"


func _refresh_kettle() -> void:
	var mx: float = maxf(float(sim.max_kw["rvp"]), 1.0)
	var kw: float = float(sim.heater_kw.get("rvp", 0.0))
	var heat: HSlider = w["heat"]
	var drag: bool = heat.is_inside_tree() and Input.is_mouse_button_pressed(MOUSE_BUTTON_LEFT) and heat.get_global_rect().has_point(heat.get_global_mouse_position())
	if not drag:
		heat.set_value_no_signal(clampf(kw / mx * 100.0, 0.0, 100.0))
	(w["heat_l"] as Label).text = "%d %% · %d kW" % [int(round(kw / mx * 100.0)), int(kw)]
	(w["auto"] as Button).set_pressed_no_signal(bool(sim.auto_mode["rvp"]))
	_spin_sync(w["set"], float(sim.setpoint["rvp"]))
	(w["stir"] as Button).set_pressed_no_signal(bool(sim.stir.get("rvp", false)))
	(w["fill"] as Button).set_pressed_no_signal(bool(fill.get("on", false)))
	(w["malt"] as Button).set_pressed_no_signal(bool(dose.get("on", false)))
	(w["malt_l"] as Label).text = "zásobník %d kg" % int(sim.malt_stock)
	var rv: Dictionary = sim.vessels["rvp"]
	var bm := _boil_min()
	var n_hops: int = (rv.get("hops", []) as Array).size()
	var nx := "rozpis hotový"
	if n_hops < _hop_plan().size():
		var h: Dictionary = _hop_plan()[n_hops]
		nx = "%d. dávka %.1f kg α %d %% v %d. min" % [n_hops + 1, float(h["kg"]), int(h["a"]), int(h["min"])]
	(w["hop_info"] as Label).text = "Var: %s · chmel: %s · hořkost %.0f IBU · DMS %.0f ppb" % [("%d min" % int(bm)) if bm >= 0.0 else "nevře", nx, sim.ibu(), sim.dms_ppb()]
	var ptxt := "rmutovací program stojí"
	if sim.prog_run:
		var st: Dictionary = sim.prog[sim.prog_i]
		ptxt = "program %d/%d: %.0f °C, drží %.0f / %.0f min" % [sim.prog_i + 1, sim.prog.size(), float(st["t"]), sim.prog_hold_s / 60.0, float(st["min"])]
	var malt: float = float(rv["malt"]) + float(sim.vessels["sk"]["malt"])
	var starch := (" · škrob %.0f %%" % sim.starch_left()) if malt > 0.5 else ""
	var fill_txt := ""
	if bool(fill.get("on", false)):
		fill_txt = " · napouští se do %.0f L (%s)" % [float(fill["target"]), "horká + studená" if bool(fill["hot_open"]) and bool(fill["cold"]) else ("horká" if bool(fill["hot_open"]) else "studená")]
	(w["rvp_info"] as Label).text = "Termostat %s · %s%s%s" % [("AUTO %.1f °C" % float(sim.setpoint["rvp"])) if bool(sim.auto_mode["rvp"]) else "ručně", ptxt, starch, fill_txt]


func _refresh_lauter() -> void:
	_spin_sync(w["sT"], float(sim.setpoint["hlt"]))
	_spin_sync(w["sF"], _route_lpm("vyslazovani"))
	var sk: Dictionary = sim.vessels["sk"]
	var bed: bool = bool(sk.get("bed", false))
	var pores: float = float(sk["malt"]) * BrewSim.BED_L_PER_KG * 0.45
	var txt := ""
	if bed:
		var lb: float = sim.bed_height(sk)
		var h_liq: float = maxf(float(sk["v"]) - pores, 0.0) / 1000.0 / float(sk["area"])
		txt = "Lože %.0f cm · odtok %.0f L/min · Δp pod sítem %.0f mbar · utažení %.0f %% · zákal %.0f %%" % [lb * 100.0, sim.darcy_lpm(), 1050.0 * 9.81 * (h_liq + 0.5 * lb) / 100.0, float(sk.get("pack", 0.0)) * 100.0, float(sk.get("turb", 0.0)) * 100.0]
	else:
		txt = "Bez filtračního lože (%s) · %.0f L" % ["rmut se usazuje" if float(sk["malt"]) > 1.0 else "voda", float(sk["v"])]
	var rh: float = float(sk.get("rake_h", 1.0))
	var rpos := "nahoře" if rh > 0.98 else ("dole" if rh < 0.02 else ("jede dolů" if sim.rake_down else "jede nahoru"))
	var dr := ("teče %.0f L/min" % sim.drain_lpm_now) if sim.drain_lpm_now > 0.5 else ("otevřená" if sim.sk_drain_open else "zavřená")
	txt += "\nKopačka %s, %s (výška %.0f %%) · kopyto %s · výpusť %s" % ["se otáčí" if sim.rake_on else "stojí", rpos, rh * 100.0, "DOLE" if sim.hoof_down else "nahoře", dr]
	var lbl: Label = w["sk_info"]
	lbl.text = txt
	var bad: bool = bed and (float(sk.get("pack", 0.0)) > 0.6 or (float(sk["v"]) < pores and sim.plato("sk") > 2.0))
	lbl.add_theme_color_override("font_color", C_BAD if bad else C_DIM)


func _refresh_pump_buttons() -> void:
	for pid in pumps_ui:
		var u: Dictionary = pumps_ui[pid]
		var rd: Dictionary = reads.get(pid, {})
		(u["btn"] as Button).set_pressed_no_signal(bool(plan.get_item(str(pid)).get("on", false)))
		var kind := str(rd.get("kind", "off"))
		var lamp := C_OFF
		var col := C_DIM
		var txt := "  stojí"
		if kind == "run":
			lamp = C_OK if float(rd["q"]) >= 1.0 else C_WARN
			if float(rd["hz"]) > 0.1:
				txt = "  %d L/min · %.0f Hz · %.2f bar · NPSH %.1f m · %s" % [int(rd["q"]), float(rd["hz"]), float(rd["bar"]), float(rd["npsh"]), str(rd["txt"])]
			else:
				txt = "  %d L/min · %s" % [int(rd["q"]), str(rd["txt"])]
		elif kind != "off":
			lamp = C_BAD
			col = C_BAD
			txt = "  %s · %.2f bar" % [str(rd.get("txt", "")), float(rd.get("bar", 0.0))]
		(u["lamp"] as ColorRect).color = lamp
		var rl: Label = u["read"]
		rl.text = txt
		rl.add_theme_color_override("font_color", col)
		_spin_sync(u["spin"], _pump_set(str(pid)))
	if w.has("lvp") and lv_pump != "":
		var on: bool = bool(plan.get_item(lv_pump).get("on", false))
		(w["lvp"] as Button).set_pressed_no_signal(on)
		var bad: bool = on and str(reads.get(lv_pump, {}).get("kind", "")) in ["dry", "shut"]
		(w["lamp_lv"] as ColorRect).color = C_BAD if bad else (C_OK if on else C_OFF)


func _refresh_ice() -> void:
	_spin_sync(w["lvset"], float(sim.setpoint["lv"]))
	var sw_on := false
	for it in plan.items:
		if str(it.get("type")) == "sw" and bool(it.get("on", false)):
			sw_on = true
	(w["glyk"] as Button).set_pressed_no_signal(sw_on)
	var lv: Dictionary = sim.vessels["lv"]
	var g := "neběží"
	if sim.active.has("glykol"):
		g = "chladí" if float(lv["t"]) > float(sim.setpoint["lv"]) + 0.05 else "drží teplotu"
	(w["lv_info"] as Label).text = "Ledovka %d L · %.1f °C (cíl %.1f) · okruh přes glykol %s · do chladiče %s%s" % [int(lv["v"]), float(lv["t"]), float(sim.setpoint["lv"]), g, "teče" if sim.active.has("ledovka") else "neteče", " · napouští se z řadu" if sim.active.has("lv_fill") else ""]


func _refresh_routes() -> void:
	if _routes_sig() != _route_sig:
		_fill_routes()
	var names: Array = []
	for rn in sp._flowing:
		if bool(sp._flowing[rn]):
			names.append(_rlabel(str(rn)))
	(w["flowing"] as Label).text = "Teče: " + (", ".join(names) if not names.is_empty() else "nic")
	var sel := str(w.get("route_sel", ""))
	if sel != "" and sim.routes.has(sel):
		var f: bool = bool(sp._flowing.get(sel, false))
		(w["route_msg"] as Label).text = "%s\nStav: %s" % [str(w.get("route_txt", "")), "● teče" if f else "neteče – zkontroluj hladinu, klapky a rozbočovač"]
	else:
		(w["route_msg"] as Label).text = "Nastavit = jen tahle cesta (ostatní klapky zavře) · + souběžně = přidá cestu k běžícím."


func _refresh_water() -> void:
	var spins: Array = w["water_spins"]
	var keys := ["alk", "ca", "mg"]
	for i in spins.size():
		_spin_sync(spins[i], float(water[keys[i]]))
	var ra := _residual_alk()
	var lbl: Label = w["ph"]
	var m: float = float(sim.vessels["rvp"]["malt"]) + float(sim.vessels["sk"]["malt"])
	if m > 1.0:
		var ph := _mash_ph()
		var fa: float = exp(-pow((ph - 5.6) / 0.45, 2.0))
		var fb: float = exp(-pow((ph - 5.4) / 0.40, 2.0))
		lbl.text = "RA %.0f mg/L CaCO₃ · pH rmutu ≈ %.2f (optimum 5,4–5,6) · aktivita α %d %% β %d %% · kyselina %d mL" % [ra, ph, int(fa * 100.0), int(fb * 100.0), int(lactic)]
		lbl.add_theme_color_override("font_color", C_WARN if ph < 5.2 or ph > 5.8 else C_DIM)
	else:
		lbl.text = "RA %.0f mg/L CaCO₃ · pH se spočítá po vystírce (model Kolbach – přibližný, na zcukření v simulaci nepůsobí)" % ra
		lbl.add_theme_color_override("font_color", C_DIM)


func _refresh_ferment() -> void:
	_spin_sync(w["fset"], float(fer["set"]))
	_spin_sync(w["fpitch"], float(fer["pitch"]))
	if bool(fer["on"]):
		(w["fer"] as Label).text = "Den %.1f · %.1f °C · zdánlivý extrakt %.1f °P · diacetyl %d ppb · kvasnice %.1f g/L" % [float(fer["t"]) / 86400.0, float(fer["T"]), _app_extract(), int(fer["D"]), float(fer["X"])]
	else:
		(w["fer"] as Label).text = "Nezakvašeno – zakvasí se po zavíčkování spilky (Víko spilky) nebo tlačítkem Zakvasit."


func _refresh_cip() -> void:
	(w["cipB"] as Button).set_pressed_no_signal(bool(cip["run"]))
	var t := "stojí"
	if bool(cip["run"]):
		t = "%s · %s · %.1f °C · %d min" % [str(CIP_TARGETS.get(str(cip["target"]), cip["target"])), str(cip["medium"]), float(cip["T"]), int(cip["min"])]
	var done: Array = []
	var dd: Dictionary = cip["done"]
	for k in dd:
		if str(k).ends_with("!"):
			continue
		var parts := str(k).split(":")
		if parts.size() == 2:
			done.append("%s %s %d min" % [str(CIP_TARGETS.get(parts[0], parts[0])), parts[1], int(dd[k])])
	if not done.is_empty():
		t += "\nHotovo: " + ", ".join(done)
	(w["cip"] as Label).text = t


func _refresh_cast() -> void:
	(w["push"] as Button).set_pressed_no_signal(bool(sp._kind_item("valve", "pushwater").get("open", false)))
	(w["vratka"] as Button).set_pressed_no_signal(bool(plan.get_item(_vratka_id()).get("open", false)))
	(w["faults"] as Button).set_pressed_no_signal(bool(faults["on"]))
	var spv: Dictionary = sim.vessels["spilka"]
	var co := ("%.1f °C" % sim.cool_out_t) if sim.cool_out_t > -0.5 else "neteče"
	(w["cast"] as Label).text = "Za chladičem %s · spilka %d L, %.1f °C%s · hadice %s · voda v potrubí %d L · protláčka dotlačila %d L" % [co, int(spv["v"]), float(spv["t"]), _p_txt("spilka"), "na tanku" if sim.hose_tank else "na vratce", int(sim.line_water), int(sim.pushed_wort)]


## hlášení: nejnovější nahoře, varování žlutě, poruchy a blokování červeně
func _refresh_log() -> void:
	var lines: Array = sp.log_lines
	var sig := "%d|%s" % [lines.size(), str(lines.back()) if not lines.is_empty() else ""]
	if sig == _log_sig:
		return
	_log_sig = sig
	var o := ""
	for i in range(lines.size() - 1, maxi(lines.size() - 41, 0) - 1, -1):
		var s := str(lines[i])
		var col := "#9fb3c8"
		if s.contains("BLOKOVÁNO") or s.contains("PORUCHA") or s.contains("zničil") or s.contains("PŘEKYPĚNÍ") or s.contains("přehřáté"):
			col = "#ff5a4f"
		elif s.contains("⚠") or s.contains("!"):
			col = "#ffc23d"
		var cut := s.find("  ")
		var head := s.substr(0, cut) if cut > 0 else ""
		var body := s.substr(cut + 2) if cut > 0 else s
		o += "[color=#5a6a7a]%s[/color] [color=%s]%s[/color]\n" % [head, col, body.replace("[", "[lb]")]
	log_box.text = o


# ---------- uložení ----------
func save_state() -> void:
	var d := {"clock": sim.clock, "mode": mode, "water": water, "lactic": lactic, "fer": fer, "cip": cip, "faults": faults, "errors": errors, "stuck": stuck,
		"steam_on": sim.steam_on, "steam_bar": sim.steam_bar, "steam_p": sim.steam_p, "plate_eff": sim.plate_eff, "pump_fault": sim.pump_fault, "ibu": ibu_end, "pitch": pitch, "recipe": recipe}
	var f := FileAccess.open(save_path, FileAccess.WRITE)
	if f != null:
		f.store_string(JSON.stringify(d))


func _load() -> void:
	if not FileAccess.file_exists(save_path):
		return
	var f := FileAccess.open(save_path, FileAccess.READ)
	if f == null:
		return
	var data: Variant = JSON.parse_string(f.get_as_text())
	if typeof(data) != TYPE_DICTIONARY:
		return
	var d: Dictionary = data
	mode = clampi(int(d.get("mode", 0)), 0, MODES.size() - 1)
	(w["mode"] as OptionButton).select(mode)
	var wd: Dictionary = d.get("water", {})
	for k in water:
		water[k] = float(wd.get(k, water[k]))
	recipe = clampi(int(d.get("recipe", 0)), 0, Recipes.RECIPES.size() - 1)
	sim.enz_mean = Recipes.enz_mean(_recipe())
	sim.recipe_ebc = Recipes.model_ebc(_recipe())  # barva z použitého sladu (původní model), ne z tabulky
	if absf(float(d.get("clock", -1.0e9)) - sim.clock) > 120.0:
		return  # uložený stav patří k jiné várce – kvašení, CIP a poruchy začínají znovu
	lactic = float(d.get("lactic", 0.0))
	var fd: Dictionary = d.get("fer", {})
	for k in fd:
		fer[k] = fd[k]
	var cd: Dictionary = d.get("cip", {})
	for k in cd:
		cip[k] = cd[k]
	var fl: Dictionary = d.get("faults", {})
	faults = {"on": bool(fl.get("on", false)), "next": float(fl.get("next", 0.0)), "active": fl.get("active", [])}
	errors = d.get("errors", [])
	stuck = d.get("stuck", {})
	sim.steam_on = bool(d.get("steam_on", true))
	sim.steam_bar = float(d.get("steam_bar", BrewSim.STEAM_BAR))
	sim.steam_p = float(d.get("steam_p", sim.steam_bar))
	sim.plate_eff = float(d.get("plate_eff", BrewSim.PLATE_EFF))
	sim.pump_fault = d.get("pump_fault", {})
	ibu_end = float(d.get("ibu", 0.0))
	var pd: Dictionary = d.get("pitch", {})
	for k in pd:
		pitch[k] = float(pd[k])
