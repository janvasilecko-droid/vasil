class_name SimPanel
extends PanelContainer
## Ovládání simulace varu. Průtoky se řídí stavem klapek a čerpadel v plánu (klik na klapku / čerpadlo),
## tlačítko trasy jen nastaví klapky a čerpadlo najednou (jako „nastavit cestu“ v původní aplikaci).

const PLAN_ID := {
	"hlt": "pp_hlt_murboadi_0",
	"rvp": "rvp",
	"sk": "sk",
	"wp": "wp",
	"spilka": "pp_spilka_murcpsky_0",
	"lv": "pp_lv_murbya65_0",
}
const SAVE_PATH := "user://brew_state.json"
const SPEEDS := [1.0, 10.0, 60.0, 180.0, 300.0, 3600.0]  # 300× a 3600× jako v původním trenažéru
## karta výuky s 31 kroky z původní aplikace – vypnutá: výuka a kontrola se postaví z checklistu obsluhy
## (panel Checklist → checklist_uceni.json), až budou body popsané a označené
const SHOW_TRAINING := false
## ovládání strojů (šnek na slad, kopačka, spílací hadice, protláčka, víko spilky) – tlačítka cest je nepřestavují
const MACHINE_KINDS := ["gate", "air", "vent", "screwwater", "screw", "rake", "rakelift", "hoof", "hose", "pushwater", "lid"]

var plan: PlanData
var plant: Node3D
var sim: BrewSim = BrewSim.new()
var time_scale: float = 60.0
var paused: bool = false
var resume_pending: bool = false
var _readout: Label
var _body: VBoxContainer
var _buttons: Dictionary = {}
var _orig_open: Dictionary = {}
var _acc: float = 0.0
var _save_acc: float = 0.0
var _route_cache: Dictionary = {}
var _flowing: Dictionary = {}
var _spray_state: Dictionary = {}
var force_sprays: bool = false  # jen pro snímky obrazovky
var _ctl_act: Dictionary = {}
var _ctl_prog: Label
var _pump_btns: Dictionary = {}
var pipe_fill: Dictionary = {}  # id trubky -> {"m": médium / "empty", "res": zbytky mláta 0..1}
var _route_pipes: Dictionary = {}  # trubky cesty podle jejího záznamu v plánu (pořadí po směru toku)
var _route_static: Dictionary = {}  # totéž, nemění se za běhu – podle něj se tok v síti přiřadí k cestě
var _flow_pipes: Dictionary = {}  # cesta -> trubky, kterými teď doopravdy teče (z hledání v síti)
var net  # NetFlow (net_flow.gd): síť potrubí z plánu – kam teče kapalina při otevřených klapkách a běžících čerpadlech
var _pq_cache: Dictionary = {}
var _want_cache: Dictionary = {}
var _want_sig: String = ""
var _want_tick: int = -100
var net_pump_curve: bool = false  # průtok čerpadla drží měnič (L/min); křivku a odpor potrubí už netlumí
var net_gravity: bool = true  # voda teče samospádem z vyšší hladiny přes otevřené klapky
var net_mode: bool = true  # false = stará logika (cesta teče, když má všechny své klapky otevřené)
var net_info: Array = []  # poslední výpočet sítě (pro testy a ladění)
var net_notes: Array = []  # čerpadla, která nesají nebo tlačí do zavřené klapky
var _net_struct: String = ""
var _net_tick: int = 0
var _pipe_vol: Dictionary = {}
var _front: Dictionary = {}
var _pipes_applied: Dictionary = {}
var _flow_applied: Dictionary = {}  # naposledy poslaný průtok do 3D (aby se neposílal každé okno)
var _flow_disp: Dictionary = {}     # zobrazený průtok trubky (vyhlazený, ať šipky neposkakují)
var _flow_target: Dictionary = {}   # cílový průtok z fyziky
var _vessel_inflow: Dictionary = {} # nádoba -> přítok v L/min (proud do hladiny)
var _vessel_inflow_mash: Dictionary = {} # nádoba -> přitéká rmut se šrotem (točí se kopačka v proudu)
var _clog_warned: bool = false
var _podraz_valves: Array = []
var _vstate: Dictionary = {}
var _close_ms: Dictionary = {}
var _man_req: Dictionary = {}
var _man_btns: Dictionary = {}
var _sp_spin: Dictionary = {}
var _mk_spin: Dictionary = {}
var _auto_ck: Dictionary = {}
var _hlt_slider: HSlider
var _rvp_slider: HSlider
var log_lines: Array = []
var training: Training
var _tr_card: PanelContainer
var _tr_step: Label
var _tr_title: Label
var _tr_text: Label
var _tr_why: Label
var _tr_state: Label
var _tr_text_box: VBoxContainer
var _tr_strip: HBoxContainer
var _tr_prev: Label
var _tr_err: Label
var _route_box: VBoxContainer
## skupiny cest v záložce CESTY: [nadpis, [klíče cest]]; ostatní cesty (z editoru) jdou do skupiny „Ostatní“
const ROUTE_GROUPS := [
	["Voda", ["voda_panev", "voda_rad", "lv_fill"]],
	["Rmutování a scezování", ["rmut_sk", "rmut_varna", "dilo", "sk_c2", "vorlauf", "podraz", "proplach", "vyslazovani"]],
	["Mladina a chlazení", ["varna_virivka", "virivka_chladic", "glykol", "ledovka"]],
]
var _route_order: Array = []
var _cur_route: String = ""
var _walk_lbl: Label
var _walk_info: Label
var _malt_lbl: Label
var _warned_once: Dictionary = {}
var _rake_id: String = ""  # kopačka ve scezovací kádi (prvek typu „rake“)


## kalibrace.json ve složce dat aplikace (user://): změřené hodnoty přepíšou parametry modelu, odmítnuté se vypíšou do protokolu
## (formát a parametry: BrewSim.CAL_PARAMS, příklad v reference/kalibrace_priklad.json)
const CAL_FILE := "user://kalibrace.json"
var cal_messages: Array = []


func _load_calibration() -> void:
	cal_messages.clear()
	if not FileAccess.file_exists(CAL_FILE):
		return
	var f := FileAccess.open(CAL_FILE, FileAccess.READ)
	var data: Variant = JSON.parse_string(f.get_as_text()) if f != null else null
	if typeof(data) != TYPE_DICTIONARY:
		cal_messages.append("Kalibrace: kalibrace.json není platný JSON objekt – přeskočeno.")
	else:
		cal_messages = sim.apply_calibration(data)
	for m in cal_messages:
		_log(str(m))


func setup(p: PlanData, pl: Node3D) -> void:
	plan = p
	plant = pl
	training = Training.new(sim)
	sim.load_routes(plan.raw.get("routes", {}))
	_load_calibration()
	_remember_valves()
	var sb := StyleBoxFlat.new()
	sb.bg_color = Color(0.08, 0.09, 0.11, 0.92)
	sb.set_corner_radius_all(6)
	sb.set_content_margin_all(10)
	add_theme_stylebox_override("panel", sb)
	offset_left = 16.0
	offset_top = 96.0
	offset_right = 452.0
	var root := VBoxContainer.new()
	root.add_theme_constant_override("separation", 6)
	add_child(root)
	var head := Button.new()
	head.focus_mode = Control.FOCUS_NONE
	head.text = "ROZVADĚČ A CESTY  ▸"
	head.flat = true
	head.add_theme_color_override("font_color", Color(0.95, 0.8, 0.25))
	head.pressed.connect(func() -> void:
		_body.visible = not _body.visible
		head.text = "ROZVADĚČ A CESTY  ▾" if _body.visible else "ROZVADĚČ A CESTY  ▸"
	)
	root.add_child(head)
	var outer := VBoxContainer.new()
	outer.add_theme_constant_override("separation", 5)
	root.add_child(outer)
	var tabs := TabContainer.new()
	tabs.custom_minimum_size = Vector2(0, 470)
	tabs.focus_mode = Control.FOCUS_NONE
	outer.add_child(tabs)
	_body = outer

	# záložka CESTY: průchod cestami jednu po druhé, seznam cest, zavřít vše
	_body = _tab(tabs, "CESTY")
	var walk := HBoxContainer.new()
	walk.add_child(_btn("◀", func() -> void: _walk_route(-1)))
	_walk_lbl = Label.new()
	_walk_lbl.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	_walk_lbl.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	_walk_lbl.text = "žádná cesta zvolena"
	walk.add_child(_walk_lbl)
	walk.add_child(_btn("▶", func() -> void: _walk_route(1)))
	_body.add_child(walk)
	_walk_info = Label.new()
	_walk_info.add_theme_font_size_override("font_size", 12)
	_walk_info.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	_walk_info.custom_minimum_size = Vector2(380, 0)
	_walk_info.modulate = Color(0.75, 0.85, 1.0)
	_body.add_child(_walk_info)
	var rowc := HBoxContainer.new()
	rowc.add_child(_btn("Zavřít vše (klapky i čerpadla)", close_all))
	_body.add_child(rowc)
	var sep := Label.new()
	sep.text = "Cesty (modrá = zvolená, zelená ● = teče):"
	sep.add_theme_font_size_override("font_size", 13)
	_body.add_child(sep)
	_route_box = VBoxContainer.new()
	_route_box.add_theme_constant_override("separation", 2)
	_body.add_child(_route_box)
	_build_route_buttons()
	var hint := Label.new()
	hint.text = "Klik na klapku ve scéně = otevřít/zavřít (zelená = otevřená, červená = zavřená)\nKlik na čerpadlo = start/stop"
	hint.add_theme_font_size_override("font_size", 12)
	hint.modulate = Color(0.7, 0.75, 0.8)
	_body.add_child(hint)

	# záložka TEPLOTY: ohřev, termostaty, rmutování, chmel
	_body = _tab(tabs, "TEPLOTY")
	var row2 := HBoxContainer.new()
	# dávky pro ~42 hl ležáku (~35 IBU): 5 kg hořkého na začátku, 2× 2 kg aromatického
	row2.add_child(_btn("Chmel 5 kg (α 5 %)", func() -> void:
		sim.add_hops(5000.0, 5.0)
		_log("Chmel 5 kg (α 5 %) do varny")))
	row2.add_child(_btn("Chmel 2 kg (α 12 %)", func() -> void:
		sim.add_hops(2000.0, 12.0)
		_log("Chmel 2 kg (α 12 %) do varny")))
	_body.add_child(row2)

	_hlt_slider = _slider("Ohřev bojleru (kW)", 300.0, "hlt")
	_rvp_slider = _slider("Ohřev varny (kW)", 600.0, "rvp")
	_build_control_section()

	# záložka STROJE: čerpadla, slad, propláchnutí, rozbočovač
	_body = _tab(tabs, "STROJE")
	_malt_lbl = Label.new()
	_malt_lbl.add_theme_font_size_override("font_size", 12)
	# slad se nesype tlačítkem, ale jako ve varně: vzduch → šoupě na slad → šnek (klikem ve scéně u násypky)
	_body.add_child(_malt_lbl)
	var rowf := HBoxContainer.new()
	rowf.add_child(_btn("Propláchnout potrubí pod síty → varna", _flush_pipes))
	_body.add_child(rowf)
	_build_pumps_section()
	_build_manifold_section()

	# záložka STAV: hladiny, teploty a protokol
	_body = _tab(tabs, "STAV")
	_readout = Label.new()
	_readout.add_theme_font_size_override("font_size", 14)
	_body.add_child(_readout)
	_body = outer
	_body.visible = true
	head.text = "ROZVADĚČ A CESTY  ▾"
	_init_pipes()
	if SHOW_TRAINING:
		_build_training_card.call_deferred()
	# měrky: měřítko podle kapacity nádoby a odečet na požádání
	plant.connect("dip_reading", _on_dip_reading)
	for it in plan.items:
		if str(it.get("type")) == "dip":
			var dvid := str(it.get("vid", ""))
			if sim.vessels.has(dvid):
				plant.call("set_dip_cap", str(it.id), float(sim.vessels[dvid]["cap"]))
	resume_pending = FileAccess.file_exists(SAVE_PATH)


## odečet z měrky: objem podle mokré části, zaokrouhlený na 5 l (víc z klacku odečíst nejde) a porovnaný se skutečností v protokolu
func _on_dip_reading(dip_id: String, vid: String, frac: float) -> void:
	var key := ""
	for k in PLAN_ID:
		if str(PLAN_ID[k]) == vid:
			key = str(k)
	if key == "" or not sim.vessels.has(key):
		return
	var cap: float = float(sim.vessels[key]["cap"])
	var read_hl: float = round(frac * 50.0)  # stupnice měrky: válec nádoby = 50 hl
	var read_l: float = frac * cap
	plant.call("set_dip_label", dip_id, "≈ %d hl" % int(read_hl))
	var vs: Dictionary = sim.vessels[key]
	var real_l: float = float(vs["v"]) + float(vs["malt"]) * (BrewSim.BED_L_PER_KG if key == "sk" else BrewSim.MALT_L_PER_KG)
	_log("Měrka %s: odečet %d hl na stupnici (v simulaci %.0f l, skutečně včetně šrotu %.0f l)" % [str(vs["name"]), int(read_hl), read_l, real_l])


func _spin(minv: float, maxv: float, val: float, stp: float, suffix: String, cb: Callable) -> SpinBox:
	var s := SpinBox.new()
	s.min_value = minv
	s.max_value = maxv
	s.step = stp
	s.value = val
	s.suffix = suffix
	s.custom_minimum_size = Vector2(96, 0)
	s.value_changed.connect(cb)
	return s


## Rozvaděč: termostaty (cílová teplota, AUTO, max. výkon), rmutovací program a čerpadla
func _build_control_section() -> void:
	var title := Label.new()
	title.text = "— TEPLOTY A OHŘEV —"
	title.add_theme_color_override("font_color", Color(0.95, 0.8, 0.25))
	_body.add_child(title)
	for spec in [["hlt", "Bojler", 99.0], ["rvp", "Varna", 100.0], ["lv", "Ledová voda", 20.0]]:
		var key: String = spec[0]
		var row := HBoxContainer.new()
		row.add_theme_constant_override("separation", 4)
		var nm := Label.new()
		nm.text = str(spec[1])
		nm.custom_minimum_size = Vector2(76, 0)
		row.add_child(nm)
		var sps := _spin(0.0, float(spec[2]), float(sim.setpoint[key]), 0.5, " °C", func(v: float) -> void: sim.setpoint[key] = v)
		_sp_spin[key] = sps
		row.add_child(sps)
		if key != "lv":
			var ck := CheckButton.new()
			ck.text = "AUTO"
			ck.focus_mode = Control.FOCUS_NONE
			ck.button_pressed = bool(sim.auto_mode[key])
			ck.toggled.connect(func(on: bool) -> void:
				sim.auto_mode[key] = on
				if not on:
					sim.heater_kw[key] = 0.0
				_log("Termostat %s: %s" % [str(spec[1]), "zapnut" if on else "vypnut"]))
			row.add_child(ck)
			_auto_ck[key] = ck
			var mks := _spin(1.0, 1500.0, float(sim.max_kw[key]), 1.0, " kW", func(v: float) -> void: sim.max_kw[key] = v)
			_mk_spin[key] = mks
			row.add_child(mks)
		_body.add_child(row)
		var act := Label.new()
		act.add_theme_font_size_override("font_size", 13)
		_ctl_act[key] = act
		_body.add_child(act)

	var stir_ck := CheckButton.new()
	stir_ck.text = "Míchadlo varny (bez míchání se šrot usazuje)"
	stir_ck.focus_mode = Control.FOCUS_NONE
	stir_ck.button_pressed = bool(sim.stir.get("rvp", false))
	stir_ck.toggled.connect(func(on: bool) -> void:
		sim.stir["rvp"] = on
		_log("Míchadlo varny: %s" % ("zapnuto" if on else "vypnuto")))
	_body.add_child(stir_ck)
	var pr := Label.new()
	pr.text = "Rmutovací program (prodlevy):"
	_body.add_child(pr)
	for i in sim.prog.size():
		var prow := HBoxContainer.new()
		var idx := i
		var pl := Label.new()
		pl.text = "%d." % (i + 1)
		prow.add_child(pl)
		prow.add_child(_spin(20.0, 100.0, float(sim.prog[i]["t"]), 0.5, " °C", func(v: float) -> void: sim.prog[idx]["t"] = v))
		prow.add_child(_spin(0.0, 180.0, float(sim.prog[i]["min"]), 1.0, " min", func(v: float) -> void: sim.prog[idx]["min"] = v))
		_body.add_child(prow)
	var pb := HBoxContainer.new()
	pb.add_child(_btn("Spustit rmutování", func() -> void:
		sim.prog_run = true
		sim.prog_i = 0
		sim.prog_hold_s = 0.0
		_log("Spuštěn rmutovací program")))
	pb.add_child(_btn("Zastavit", func() -> void:
		sim.prog_run = false
		_log("Rmutovací program zastaven")))
	_body.add_child(pb)
	_ctl_prog = Label.new()
	_ctl_prog.add_theme_font_size_override("font_size", 13)
	_body.add_child(_ctl_prog)


## Čerpadla (záložka STROJE)
func _build_pumps_section() -> void:
	var pt := Label.new()
	pt.text = "Čerpadla (klik = start / stop):"
	_body.add_child(pt)
	var prow2 := HBoxContainer.new()
	for it in plan.items:
		if str(it.get("type")) == "pump":
			var pid := str(it.id)
			var pbt := _btn(str(it.get("label", pid)), func() -> void:
				plant.call("click_item", pid))
			_pump_btns[pid] = pbt
			prow2.add_child(pbt)
	_body.add_child(prow2)


func _update_controls() -> void:
	for key in _ctl_act:
		var vs: Dictionary = sim.vessels[key]
		var txt := "   teď %.1f °C → cíl %.1f °C" % [float(vs["t"]), float(sim.setpoint[key])]
		if key != "lv":
			txt += " · ohřev %.1f kW %s" % [float(sim.heater_kw.get(key, 0.0)), "(AUTO)" if bool(sim.auto_mode[key]) else "(ručně)"]
			if float(vs.get("boil", 0.0)) > 0.05:
				txt += " · VAR"
		(_ctl_act[key] as Label).text = txt
		var near: bool = absf(float(vs["t"]) - float(sim.setpoint[key])) < 0.6
		(_ctl_act[key] as Label).modulate = Color(0.6, 1.0, 0.6) if near else Color(1, 1, 1)
	if sim.prog_run:
		var st: Dictionary = sim.prog[sim.prog_i]
		_ctl_prog.text = "   běží %d/%d: %.0f °C, držení %.0f / %.0f min" % [sim.prog_i + 1, sim.prog.size(), float(st["t"]), sim.prog_hold_s / 60.0, float(st["min"])]
	else:
		_ctl_prog.text = "   program stojí"
	for key2 in _man_btns:
		var kk: Vector2i = key2
		(_man_btns[key2] as Button).set_pressed_no_signal(str(plant.call("manifold_jumper_id", kk.x, kk.y)) != "")
	for pid in _pump_btns:
		var on: bool = bool(plan.get_item(str(pid)).get("on", false))
		(_pump_btns[pid] as Button).modulate = Color(0.55, 1.0, 0.55) if on else Color(1, 1, 1)
	# ručně nastavené kW na posuvnících sledují skutečný ohřev (termostat je mění sám)
	_hlt_slider.set_value_no_signal(float(sim.heater_kw.get("hlt", 0.0)))
	_rvp_slider.set_value_no_signal(float(sim.heater_kw.get("rvp", 0.0)))


func _btn(text: String, cb: Callable) -> Button:
	var b := Button.new()
	b.text = text
	b.focus_mode = Control.FOCUS_NONE
	b.pressed.connect(cb)
	return b


func _slider(text: String, maxv: float, key: String) -> HSlider:
	var hb := HBoxContainer.new()
	var lb := Label.new()
	lb.text = text
	lb.custom_minimum_size = Vector2(150, 0)
	hb.add_child(lb)
	var sl := HSlider.new()
	sl.min_value = 0.0
	sl.max_value = maxv
	sl.step = 1.0
	sl.focus_mode = Control.FOCUS_NONE
	sl.custom_minimum_size = Vector2(150, 0)
	sl.value_changed.connect(func(v: float) -> void: sim.heater_kw[key] = v)
	hb.add_child(sl)
	_body.add_child(hb)
	return sl


func _log(text: String) -> void:
	var m: int = int(sim.clock / 60.0)
	log_lines.append("%d:%02d  %s" % [m / 60, m % 60, text])
	if log_lines.size() > 200:
		log_lines.pop_front()


# ---------- čas ----------
func set_speed(x: float) -> void:
	time_scale = x
	paused = false


func toggle_pause() -> void:
	paused = not paused


# ---------- stav klapek a čerpadel ----------
func _remember_valves() -> void:
	_orig_open.clear()
	for it in plan.items:
		var typ := str(it.get("type"))
		if typ == "valve":
			_orig_open[str(it.id)] = false
			if bool(it.get("open", false)):
				_set_item(it, "open", false)
		elif typ == "pump" and bool(it.get("on", false)):
			_set_item(it, "on", false)
		elif typ == "manifold":
			var ports: Dictionary = {}
			for i in int(it.get("cols", 4)):
				for side in ["L", "P"]:
					var key := "%s%d" % [side, i]
					ports[key] = false
			var previous: Dictionary = it.get("port_open", {})
			var needs_rebuild := false
			for key in ports:
				if not previous.has(key) or bool(previous[key]):
					needs_rebuild = true
			it["port_open"] = ports
			if needs_rebuild and plant != null:
				plant.call("rebuild_item", str(it.id))


func _set_item(it: Dictionary, key: String, val: bool) -> void:
	if bool(it.get(key, false)) != val:
		it[key] = val
		if key == "open" and not val:
			it.erase("pct")  # zavřená klapka nemá mezipolohu; po otevření je zase úplně otevřená
		if plant != null:
			plant.call("rebuild_item", str(it.id))


func close_all() -> void:
	for it in plan.items:
		var t := str(it.get("type"))
		if t == "valve":
			_set_item(it, "open", false)
		elif t == "pump":
			_set_item(it, "on", false)
	_set_manifold_ports("")
	_log("Všechny klapky zavřeny, čerpadla vypnuta")


## FYZIKA+ oprava: porty rozbočovače pro víc cest najednou (automat checklistu pouští např. vodu z bojleru i z řadu zároveň).
## Dřív je nastavovalo jen tlačítko jedné cesty, takže při automatu zůstal rozbočovač zavřený a voda do kotle nedotekla.
func set_manifold_ports_for(rns: Array) -> void:
	if plant == null:
		return
	var mp: Dictionary = plant.call("manifold_ports")
	if mp.is_empty():
		return
	var man: Dictionary = plan.get_item(str(mp["id"]))
	if man.is_empty():
		return
	if not man.has("port_open") or typeof(man["port_open"]) != TYPE_DICTIONARY:
		man["port_open"] = {}
	var desired: Dictionary = {}
	for rn in rns:
		var req: Array = _compute_man_req(str(rn))
		if req.size() == 2:
			desired["L%d" % int(req[0])] = true
			desired["P%d" % int(req[1])] = true
	var ports: Dictionary = man["port_open"]
	for i in int(man.get("cols", 4)):
		for side in ["L", "P"]:
			var key := "%s%d" % [side, i]
			var should_open := bool(desired.get(key, false))
			if bool(ports.get(key, true)) != should_open:
				plant.call("manifold_port_toggle", key)
			else:
				ports[key] = should_open


func _set_manifold_ports(rn: String) -> void:
	if plant == null:
		return
	var mp: Dictionary = plant.call("manifold_ports")
	if mp.is_empty():
		return
	var man: Dictionary = plan.get_item(str(mp["id"]))
	if man.is_empty():
		return
	if not man.has("port_open") or typeof(man["port_open"]) != TYPE_DICTIONARY:
		man["port_open"] = {}
	var desired: Dictionary = {}
	if rn != "":
		var req: Array = _compute_man_req(rn)
		if req.size() == 2:
			desired["L%d" % int(req[0])] = true
			desired["P%d" % int(req[1])] = true
	var ports: Dictionary = man["port_open"]
	for i in int(man.get("cols", 4)):
		for side in ["L", "P"]:
			var key := "%s%d" % [side, i]
			var should_open := bool(desired.get(key, false))
			if bool(ports.get(key, true)) != should_open:
				plant.call("manifold_port_toggle", key)
			else:
				ports[key] = should_open


## záložka v panelu: posuvná, s vlastním sloupcem
func _tab(tabs: TabContainer, title: String) -> VBoxContainer:
	var sc := ScrollContainer.new()
	sc.name = title
	sc.horizontal_scroll_mode = ScrollContainer.SCROLL_MODE_DISABLED
	tabs.add_child(sc)
	var vb := VBoxContainer.new()
	vb.add_theme_constant_override("separation", 5)
	vb.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	sc.add_child(vb)
	return vb


func _build_route_buttons() -> void:
	for c in _route_box.get_children():
		c.queue_free()
	_buttons.clear()
	_route_order.clear()
	var groups: Array = []
	for g in ROUTE_GROUPS:
		groups.append([g[0], []])
	groups.append(["Ostatní", []])
	for rn in sim.routes:
		if str(sim.routes[rn]["from"]) == "" and str(sim.routes[rn]["to"]) == "" and not (rn in ["glykol", "ledovka"]):
			continue  # rozpracovaná trasa z editoru bez nádob
		var gi: int = groups.size() - 1
		for i in ROUTE_GROUPS.size():
			if (ROUTE_GROUPS[i][1] as Array).has(rn):
				gi = i
		(groups[gi][1] as Array).append(rn)
	for i in ROUTE_GROUPS.size():  # v rámci skupiny pořadí podle ROUTE_GROUPS (zhruba pořadí várky)
		var ordered: Array = []
		for rn in ROUTE_GROUPS[i][1]:
			if (groups[i][1] as Array).has(rn):
				ordered.append(rn)
		groups[i][1] = ordered
	for g in groups:
		if (g[1] as Array).is_empty():
			continue
		var hl := Label.new()
		hl.text = "— %s —" % str(g[0])
		hl.add_theme_color_override("font_color", Color(0.95, 0.8, 0.25))
		hl.add_theme_font_size_override("font_size", 13)
		_route_box.add_child(hl)
		for rn in g[1]:
			var b := _btn(str(sim.routes[rn]["label"]), _set_route.bind(rn))
			b.alignment = HORIZONTAL_ALIGNMENT_LEFT
			_buttons[rn] = b
			_route_order.append(rn)
			_route_box.add_child(b)


## ◀ ▶ : nastaví předchozí / další cestu v pořadí
func _walk_route(dir: int) -> void:
	if _route_order.is_empty():
		return
	var i: int = _route_order.find(_cur_route)
	i = 0 if i < 0 and dir > 0 else (_route_order.size() - 1 if i < 0 else posmod(i + dir, _route_order.size()))
	_set_route(str(_route_order[i]))


func _item_name(vid: String) -> String:
	var it := plan.get_item(vid)
	var l := str(it.get("label", ""))
	return l if l != "" else vid.trim_prefix("valve_")


func _update_walk() -> void:
	if _walk_lbl == null:
		return
	if _cur_route == "" or not sim.routes.has(_cur_route):
		_walk_lbl.text = "žádná cesta zvolena"
		_walk_info.text = "Zvol cestu v seznamu nebo šipkami ◀ ▶ – nastaví se klapky a čerpadlo."
		return
	var i: int = _route_order.find(_cur_route)
	_walk_lbl.text = "%d / %d  %s" % [i + 1, _route_order.size(), str(sim.routes[_cur_route]["label"])]
	var names: Array = []
	for vid in route_valve_ids(_cur_route):
		names.append(_item_name(str(vid)))
	var pump: String = sim.route_pump(_cur_route)
	var pn := _item_name(pump) if pump != "" else "bez čerpadla (samospád)"
	_walk_info.text = "Čerpadlo: %s\nOtevřené klapky (%d): %s\nStav: %s" % [pn, names.size(), ", ".join(names), "● teče" if bool(_flowing.get(_cur_route, false)) else "zatím neteče (zkontroluj hladinu a rozbočovač)"]


## Po uložení trasy v editoru cest: znovu načíst definice tras, klapky, čerpadla a trubky
func reload_routes() -> void:
	sim.load_routes(plan.raw.get("routes", {}))
	_route_cache.clear()
	_man_req.clear()
	_init_pipes()
	_build_route_buttons()


func _set_route(rn: String) -> void:
	var want: Dictionary = {}
	for vid in route_valve_ids(rn):
		want[vid] = true
	var pump: String = sim.route_pump(rn)
	for it in plan.items:
		var t := str(it.get("type"))
		if MACHINE_KINDS.has(str(it.get("kind", ""))):
			continue  # šnek na slad a kopačka nepatří k cestám – tlačítko cesty je nechá, jak jsou
		if t == "valve":
			_set_item(it, "open", want.has(str(it.id)))
		elif t == "pump":
			_set_item(it, "on", str(it.id) == pump)
	_set_manifold_ports(rn)
	_cur_route = rn
	_update_walk()
	_log("Nastavena cesta: " + str(sim.routes[rn]["label"]))


func _dist_seg(p: Vector2, a: Vector2, b: Vector2) -> float:
	var ab := b - a
	var l2 := ab.length_squared()
	if l2 == 0.0:
		return p.distance_to(a)
	var t := clampf((p - a).dot(ab) / l2, 0.0, 1.0)
	return p.distance_to(a + ab * t)


func route_valve_ids(rname: String) -> Array:
	if _route_cache.has(rname):
		return _route_cache[rname]
	var res: Array = []
	var routes: Dictionary = plan.raw.get("routes", {})
	if not routes.has(rname):
		return res
	if routes[rname].has("valves"):
		# trasa označená v editoru cest: klapky přesně podle záznamu
		for vid in routes[rname]["valves"]:
			if str(plan.get_item(str(vid)).get("type", "")) == "valve":
				res.append(str(vid))
		_route_cache[rname] = res
		return res
	var segs: Array = routes[rname].get("segs", [])
	var rpipes: Array = routes[rname].get("pipes", [])
	for it in plan.items:
		if str(it.get("type")) != "valve":
			continue
		var vp := Vector2(float(it.x), float(it.y))
		var hit := false
		for s in segs:
			if _dist_seg(vp, Vector2(float(s[1]), float(s[2])), Vector2(float(s[3]), float(s[4]))) <= 8.0:
				hit = true
				break
		if not hit:
			for pid in rpipes:
				var pit: Dictionary = plan.get_item(str(pid))
				if pit.is_empty() or not pit.has("pts"):
					continue
				var on_pipe := false
				var pts: Array = pit["pts"]
				for i in pts.size() - 1:
					if _dist_seg(vp, Vector2(float(pts[i][0]), float(pts[i][1])), Vector2(float(pts[i + 1][0]), float(pts[i + 1][1]))) <= 6.0:
						on_pipe = true
						break
				if on_pipe:
					for s in segs:
						if vp.distance_to(Vector2(float(s[1]), float(s[2]))) <= 120.0 or vp.distance_to(Vector2(float(s[3]), float(s[4]))) <= 120.0:
							hit = true
							break
				if hit:
					break
		if hit:
			res.append(str(it.id))
	_route_cache[rname] = res
	return res


## trasa teče, když má otevřené všechny klapky a běží její čerpadlo
func _route_flows(rn: String) -> bool:
	var ids: Array = route_valve_ids(rn)
	if ids.is_empty():
		return false
	for vid in ids:
		if not bool(plan.get_item(str(vid)).get("open", false)):
			return false
	var pump: String = sim.route_pump(rn)
	if pump != "" and not bool(plan.get_item(pump).get("on", false)):
		return false
	return _manifold_ok(rn) and _hose_ok(rn)


## Trasa vedoucí přes rozbočovač teče jen když je mezi jejími porty nasazená propojka (trubka nebo hadice)
func _manifold_ok(rn: String) -> bool:
	if not _man_req.has(rn):
		_man_req[rn] = _compute_man_req(rn)
	var rq: Array = _man_req[rn]
	if rq.is_empty():
		return true
	return str(plant.call("manifold_jumper_id", int(rq[0]), int(rq[1]))) != ""


func _compute_man_req(rn: String) -> Array:
	var mp: Dictionary = plant.call("manifold_ports")
	if mp.is_empty():
		return []
	var routes: Dictionary = plan.raw.get("routes", {})
	if not routes.has(rn):
		return []
	var ys: Array = mp["ys"]
	var li := -1
	var rj := -1
	for s in routes[rn].get("segs", []):
		if not str(s[0]).begins_with("manifold_"):
			continue
		var x_lo := minf(float(s[1]), float(s[3]))
		var x_hi := maxf(float(s[1]), float(s[3]))
		var y := float(s[2])
		var idx := -1
		var best := 1e9
		for i in ys.size():
			if absf(float(ys[i]) - y) < best:
				best = absf(float(ys[i]) - y)
				idx = i
		if x_hi <= float(mp["xl"]) + 5.0:
			li = idx
		elif x_lo >= float(mp["xr"]) - 5.0:
			rj = idx
	if li >= 0 and rj >= 0:
		return [li, rj]
	return []


func _build_manifold_section() -> void:
	var mp: Dictionary = plant.call("manifold_ports")
	if mp.is_empty():
		return
	var n: int = (mp["ys"] as Array).size()
	var title := Label.new()
	title.text = "— ROZBOČOVAČ: PROPOJKY —"
	title.add_theme_color_override("font_color", Color(0.95, 0.8, 0.25))
	_body.add_child(title)
	var hint := Label.new()
	hint.text = "Klik = nasadit / sundat propojku z levého (L) do pravého (P) portu.\nStejné číslo = rovná trubka, jiné = hadice. Trasa přes rozbočovač teče jen s propojkou."
	hint.add_theme_font_size_override("font_size", 12)
	hint.modulate = Color(0.7, 0.75, 0.8)
	_body.add_child(hint)
	var grid := GridContainer.new()
	grid.columns = n + 1
	var corner := Label.new()
	corner.text = "L \\ P"
	grid.add_child(corner)
	for j in n:
		var hl := Label.new()
		hl.text = "P%d" % (j + 1)
		hl.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
		grid.add_child(hl)
	for i in n:
		var rl := Label.new()
		rl.text = "L%d" % (i + 1)
		grid.add_child(rl)
		for j in n:
			var b := Button.new()
			b.toggle_mode = true
			b.focus_mode = Control.FOCUS_NONE
			b.text = "—" if i != j else "═"
			var li := i
			var rj := j
			b.toggled.connect(func(_on: bool) -> void:
				var now: bool = plant.call("manifold_toggle", li, rj)
				b.set_pressed_no_signal(now)
				_log("Rozbočovač: propojka L%d→P%d %s" % [li + 1, rj + 1, "nasazena" if now else "sundána"]))
			_man_btns[Vector2i(i, j)] = b
			grid.add_child(b)
	_body.add_child(grid)


func _process(delta: float) -> void:
	if plan == null:
		return
	_update_flows()
	_sync_sk()
	if not paused:
		var dt: float = minf(delta, 0.1) * time_scale
		var n: int = maxi(int(ceil(dt / 2.0)), 1)
		for i in n:
			sim.step(dt / float(n))
			if SHOW_TRAINING:
				training.update(dt / float(n))
		_update_malt_feed(dt)
		_update_pipes(dt)
	_smooth_pipe_flow(delta)  # vzhled toku se dorovnává podle skutečného času, i při zrychlené simulaci
	for ev in sim.events:
		_log(str(ev))
	sim.events.clear()
	for ev2 in training.events:
		_log(str(ev2))
	training.events.clear()
	_update_visuals()
	_acc += delta
	if _acc > 0.25:
		_acc = 0.0
		_update_readout()
	_save_acc += delta
	if _save_acc > 20.0:
		_save_acc = 0.0
		save_state()


## které trasy teď tečou: podle stavu klapek a čerpadel v plánu se v síti potrubí najde, kam kapalina doopravdy teče
func _update_flows() -> void:
	var want: Dictionary = {}
	if net_mode:
		_net_check_structure()
		want = _net_wanted()
	for rn in sim.routes.keys():
		var r: Dictionary = sim.routes[rn]
		var f: bool
		if not net_mode or (str(r["from"]) == "" and not (rn in ["glykol", "ledovka"])):
			f = _route_flows(str(rn))  # cesta bez nádob (částečná cesta z editoru) a stará logika
		else:
			f = want.has(rn) and _hose_ok(str(rn))
		if f != bool(_flowing.get(rn, false)) and str(r["from"]) != "":
			_log(("Teče: " if f else "Zastaveno: ") + str(r["label"]))
		_flowing[rn] = f
		if f:
			sim.active[rn] = true
			sim.flow_scale[rn] = float(want.get(rn, {}).get("scale", 1.0))
			if want.has(rn):
				_flow_pipes[rn] = want[rn]["pipes"]
				sim.route_pipes[rn] = want[rn]["pipes"]  # zpoždění: sim._flow tlačí objem těmito trubkami
		else:
			sim.active.erase(rn)
			sim.flow_scale.erase(rn)
			_flow_pipes.erase(rn)
			sim.route_pipes.erase(rn)
		sim.blocked[rn] = false
		if bool(r.get("synthetic", false)) and not f:
			sim.routes.erase(rn)
			_flowing.erase(rn)
			_front.erase(rn)


func _build_net() -> void:
	var id_to_key: Dictionary = {}
	for k in PLAN_ID:
		id_to_key[str(PLAN_ID[k])] = k
	net = preload("res://scripts/net_flow.gd").new()
	net.build(plan, id_to_key)
	_net_struct = _net_struct_sig()


## tvar plánu (trubky, klapky, čerpadla, nádoby) – po změně v editoru se síť postaví znovu
func _net_struct_sig() -> String:
	var s := ""
	for it in plan.items:
		match str(it.get("type")):
			"pipe":
				s += str(it.id) + str(it.get("pts", []))
			"valve", "pump", "vessel", "manifold":
				s += "%s%s,%s,%s,%s;" % [str(it.id), str(it.get("x")), str(it.get("y")), str(it.get("w", "")), str(it.get("kind", ""))]
	return s


func _net_check_structure() -> void:
	_net_tick += 1
	if net == null:
		_build_net()
		return
	if _net_tick % 30 == 0 and _net_struct_sig() != _net_struct:
		_build_net()
		_init_pipes()


## výchozí průtok čerpadla (L/min): medián jeho definovaných cest
func _pump_lpm(pump: String) -> float:
	var l: Array = []
	for rn in sim.routes:
		var r: Dictionary = sim.routes[rn]
		if str(r.get("pump", "")) == pump and float(r.get("lpm", 0.0)) > 0.0 and not bool(r.get("synthetic", false)):
			l.append(float(r["lpm"]))
	if l.is_empty():
		return 40.0
	l.sort()
	return float(l[l.size() / 2])


## jaké cesty odpovídají toku src → dst: ledovka a glykol podle průchodu chladiči, jinak cesty se stejnou nádobou
## odkud a kam, stejným čerpadlem a pokrytím trubek; žádná taková? Vznikne dočasná cesta
func _net_names(src: String, dst: String, pump: String, pipes: Array, _plate: bool, chil: bool, loop_skip: bool = false) -> Array:
	if src == "lv" and dst == "lv" and chil:
		return ["glykol"]
	if src == "lv" and dst == "hlt" and pump != "":
		return ["ledovka"]
	var cand: Array = []
	for rn in sim.routes:
		var r: Dictionary = sim.routes[rn]
		if bool(r.get("synthetic", false)) or str(r["from"]) != src or str(r["to"]) != dst:
			continue
		if str(r.get("pump", "")) != pump:
			continue
		cand.append(rn)
	var covered: Array = []
	var best := ""
	var best_cov := -1.0
	for rn in cand:
		var st: Array = _route_static.get(rn, [])
		var hit := 0
		for pid in st:
			if pipes.has(pid):
				hit += 1
		var cov: float = float(hit) / float(maxi(st.size(), 1))
		if cov >= 0.6:
			covered.append(rn)
		if cov > best_cov:
			best_cov = cov
			best = str(rn)
	if not covered.is_empty():
		return covered
	if best != "":
		return [best]
	if loop_skip:
		return []  # čerpadlo vrací část toku do téže nádoby (okruh) – další cíl existuje, okruh se nepočítá zvlášť
	var nm := "net_%s_%s_%s" % [src, dst, pump]
	if not sim.routes.has(nm):
		var med := "water"
		if src in ["rvp", "sk", "wp", "spilka"]:
			med = "wort"
		var pl := ""
		for k in PLAN_ID:
			if k == src:
				pl = str(plan.get_item(str(PLAN_ID[k])).get("label", k))
		var pu: Dictionary = plan.get_item(pump)
		sim.routes[nm] = {"from": src, "to": dst, "lpm": _pump_lpm(pump) if pump != "" else 40.0, "pump": pump, "medium": med, "synthetic": true,
			"label": "%s: %s → %s" % [str(pu.get("label", "řad")), src, dst]}
	return [nm]


## výsledek hledání v síti jako {cesta: {scale, pipes}}
func _net_wanted() -> Dictionary:
	# výpočet se opakuje jen při změně klapek a čerpadel, jinak nejvýš každých 20 průchodů (hladiny se mění pomalu)
	var sig: String = net._signature()
	if sig == _want_sig and _net_tick - _want_tick < 20:
		return _want_cache
	_want_sig = sig
	_want_tick = _net_tick
	_want_cache = _net_wanted_calc()
	return _want_cache


## kapacita malých trysek vyslazování (L/min v měřítku nastavení čerpadla): NOZZLE_LPM na trysku (sprchy kind "vysl" v plánu).
## Při měniči C4 kolem 45–50 L/min tečou „akorát“, víc neprojde. 0 = plán žádné takové trysky nemá, bez omezení.
const NOZZLE_LPM := 25.0


func _nozzle_cap() -> float:
	var n := 0
	for it in plan.items:
		if str(it.get("type")) == "spray" and str(it.get("kind", "")) == "vysl":
			n += 1
	return float(n) * NOZZLE_LPM


func _net_wanted_calc() -> Dictionary:
	var res: Dictionary = net.solve(sim)
	var all_flows: Array = (res["flows"] as Array).duplicate()
	if net_gravity:
		var pumped: Dictionary = {}
		var busy: Dictionary = {}  # trubky, kterými právě tlačí čerpadlo nebo řad – samospád proti tlaku neteče
		for f0 in all_flows:
			for d0 in f0["dsts"]:
				pumped["%s>%s" % [f0["src"], d0["key"]]] = true
				for pid0 in net.pipes_of((f0["spath"] as Array) + (d0["edges"] as Array)):
					busy[pid0] = true
		for gf in net.solve_gravity(sim):
			var keep_d: Array = []
			for d1 in gf["dsts"]:
				if pumped.has("%s>%s" % [gf["src"], d1["key"]]):
					continue
				var clash := false
				for pid1 in net.pipes_of(d1["edges"]):
					if busy.has(pid1):
						clash = true
						break
				if not clash:
					keep_d.append(d1)
			if not keep_d.is_empty():
				gf["dsts"] = keep_d
				all_flows.append(gf)
	net_info = all_flows
	net_notes = res["notes"]
	var want: Dictionary = {}
	var pump_src: Dictionary = {}  # přípojky nádob, ze kterých právě sají čerpadla
	for fl0 in all_flows:
		if str(fl0["pump"]) != "":
			pump_src[int(fl0["src_k"])] = true
	for fl in all_flows:
		var groups: Dictionary = {}
		for d in fl["dsts"]:
			if str(fl["pump"]) == "" and pump_src.has(int(d["k"])):
				continue  # tlak z řadu nevtlačí vodu zpátky do přípojky, ze které čerpadlo zrovna saje
			var g: Dictionary = groups.get(d["key"], {"edges": [], "plate": false, "chil": false})
			(g["edges"] as Array).append_array(d["edges"])
			g["plate"] = bool(g["plate"]) or bool(d["plate"])
			g["chil"] = bool(g["chil"]) or bool(d["chil"])
			groups[d["key"]] = g
		var entries: Array = []
		for dk in groups:
			var g2: Dictionary = groups[dk]
			var all_edges: Array = (fl["spath"] as Array).duplicate()
			all_edges.append_array(g2["edges"])
			var pipes: Array = net.pipes_of(all_edges)
			var loop_skip: bool = str(dk) == str(fl["src"]) and groups.size() > 1
			for rn in _net_names(str(fl["src"]), str(dk), str(fl["pump"]), pipes, bool(g2["plate"]), bool(g2["chil"]), loop_skip):
				entries.append({"rn": rn, "pipes": pipes, "edges": all_edges})
		for en in entries:
			var cur: Dictionary = want.get(en["rn"], {})
			# čerpadlo má pevný průtok, který se dělí mezi cíle; řad tlačí stálým tlakem (~4 bar) všemi otevřenými větvemi
			var sc: float = 1.0 / float(entries.size()) if str(fl["pump"]) != "" else 1.0
			if str(fl["pump"]) != "":
				# měnič udává průtok (L/min) a čerpadlo ho opravdu přečerpá – neškrtí ho křivka ani délka potrubí.
				# Ubrat umí jen přivřená klapka na cestě (a kapacita trysek / propustnost lože, viz níže).
				sc *= net.path_throttle(en["edges"])
			elif str(fl["src"]) == "mains" and not bool(fl.get("gravity", false)):
				# tlak z řadu: projde trubkami, cestami i sprchami podle odporu (ne jen pevné lpm trasy)
				var setp_m: float = maxf(float(sim.routes.get(en["rn"], {}).get("lpm", 40.0)), 1.0)
				var ckm: String = "mains|%s|%s" % [str(en["rn"]), str(net._cache_sig)]
				var cm: Dictionary = _pq_cache.get(ckm, {})
				if cm.is_empty() or _net_tick - int(cm["t"]) > 40:
					cm = {"t": _net_tick, "q": float(net.mains_q(sim, fl, maxf(setp_m * 2.5, 80.0)))}
					_pq_cache[ckm] = cm
				sc *= clampf(float(cm["q"]) / setp_m, 0.05, 2.5)
				sc *= net.path_throttle(en["edges"])
			# přivřená klapka (mezipoloha po 10 %) na cestě: u čerpadla je odpor už nahoře, u řadu v mains_q;
			# samospád to má v solve_gravity. Zbývá cesta bez čerpadla (např. přepouštění klapkou).
			if not bool(fl.get("gravity", false)) and str(fl["src"]) != "mains" and str(fl["pump"]) == "":
				sc *= net.path_throttle(en["edges"])
			if str(en["rn"]) == "vyslazovani":  # malé sprchovací trysky: víc než jejich kapacita neprojde ani při vyšším nastavení měniče
				var noz_cap: float = _nozzle_cap()
				var setp_n: float = maxf(float(sim.routes.get(en["rn"], {}).get("lpm", 40.0)), 1.0)
				if noz_cap > 0.0 and setp_n * sc > noz_cap:
					sc = noz_cap / setp_n
					_warn_once("trysky_vysl", "Trysky vyslazování jsou malé: víc než asi %d L/min neprojde (snížený průtok čerpadla, tlak v potrubí roste)." % int(noz_cap))
			if bool(fl.get("gravity", false)):  # samospád: průtok podle spádu a průřezu, ne podle čerpadla
				var rl: float = maxf(float(sim.routes.get(en["rn"], {}).get("lpm", 40.0)), 1.0)
				sc = minf(1.5, float(fl["lpm"]) / float(entries.size()) / rl)
			if cur.is_empty():
				want[en["rn"]] = {"scale": sc, "pipes": en["pipes"]}
			else:
				cur["scale"] = minf(float(cur["scale"]), sc)
				for pid in en["pipes"]:
					if not (cur["pipes"] as Array).has(pid):
						(cur["pipes"] as Array).append(pid)
	return want


# ---------- obsah potrubí: plnění, zbytky mláta a proplach ----------
const ROUTE_FILL := {
	"voda_panev": "water", "voda_rad": "water", "podraz": "water", "vyslazovani": "water", "lv_fill": "water",
	"glykol": "water", "ledovka": "water", "varna_virivka": "wort", "virivka_chladic": "wort", "dilo": "wort", "rmut_sk": "mash",
}


## trubky každé trasy v pořadí po směru toku + jejich objem v litrech; na začátku jsou všechny prázdné
func _init_pipes() -> void:
	var routes: Dictionary = plan.raw.get("routes", {})
	var rx := RegEx.new()
	rx.compile("pipe_[A-Za-z0-9_]+")
	_route_pipes.clear()
	for rn in sim.routes:
		if not routes.has(rn):
			continue
		var ids: Array = []
		for e in routes[rn].get("edges", []):
			for m in rx.search_all(str(e)):
				var pid := m.get_string()
				if not ids.has(pid) and not plan.get_item(pid).is_empty():
					ids.append(pid)
		if ids.size() < 1:
			continue
		# směr toku: konec trasy blíž k cílové nádobě než k zdrojové -> pořadí je správně, jinak otočit
		var src_key: String = str(sim.routes[rn]["from"])
		var dst_key: String = str(sim.routes[rn]["to"])
		var sp := _vessel_center(src_key)
		var dp := _vessel_center(dst_key)
		if sp != Vector2.INF and dp != Vector2.INF:
			var first_pts: Array = plan.get_item(str(ids[0])).get("pts", [])
			var last_pts: Array = plan.get_item(str(ids[ids.size() - 1])).get("pts", [])
			if first_pts.size() > 0 and last_pts.size() > 0:
				var a := Vector2(float(first_pts[0][0]), float(first_pts[0][1]))
				var b := Vector2(float(last_pts[last_pts.size() - 1][0]), float(last_pts[last_pts.size() - 1][1]))
				if a.distance_to(dp) + b.distance_to(sp) < a.distance_to(sp) + b.distance_to(dp):
					ids.reverse()
		_route_pipes[rn] = ids
	_route_static = _route_pipes.duplicate(true)
	_pipe_vol.clear()
	pipe_fill.clear()
	_pipes_applied.clear()
	for it in plan.items:  # trubky se plní, kudy kapalina doopravdy teče – ne jen po nakreslených cestách
		if str(it.get("type")) != "pipe" or (it.get("pts", []) as Array).size() < 2:
			continue
		var pid := str(it.id)
		var pts: Array = it.get("pts", [])
		var len_cm := 0.0
		for i in pts.size() - 1:
			len_cm += Vector2(float(pts[i][0]), float(pts[i][1])).distance_to(Vector2(float(pts[i + 1][0]), float(pts[i + 1][1])))
		var r_m: float = float(it.get("dn", 25)) / 2000.0
		_pipe_vol[pid] = maxf(PI * r_m * r_m * len_cm / 100.0 * 1000.0, 0.3)
		pipe_fill[pid] = {"m": "empty", "res": 0.0}
	sim.init_pipe_caps(_pipe_vol)  # fyzický objem trubek pro zpoždění toku
	for pid in pipe_fill:
		_push_pipe(str(pid))


func _vessel_center(key: String) -> Vector2:
	if not PLAN_ID.has(key):
		return Vector2.INF
	var it: Dictionary = plan.get_item(str(PLAN_ID[key]))
	if it.is_empty():
		return Vector2.INF
	return Vector2(float(it.x) + float(it.get("w", 0)) * 0.5, float(it.y) + float(it.get("h", 0)))


func _push_pipe(pid: String) -> void:
	var st: Dictionary = pipe_fill[pid]
	var key := str(st["m"]) + "_" + str(int(float(st["res"]) * 20.0))
	if str(_pipes_applied.get(pid, "")) != key:
		_pipes_applied[pid] = key
		if plant != null:
			plant.call("set_pipe_fill", pid, str(st["m"]), float(st["res"]))


## Trasa teče -> obsah trubek bere z sim.pipe_hold (zpoždění jako push_path); animace proudu podle L/min.
## Protékající voda vyplachuje zbytky mláta, nejrychleji při proplachu pod síty (podrážení + dílo směrem do varny).
func _update_pipes(dt: float) -> void:
	var flush_active: bool = bool(_flowing.get("podraz", false)) and bool(_flowing.get("dilo", false))
	var flow_now: Dictionary = {}
	for rn in _flow_pipes:
		if not bool(_flowing.get(rn, false)) or not sim.routes.has(rn):
			_front.erase(rn)
			continue
		var lpm: float = float(sim.routes[rn]["lpm"]) * BrewSim.FLOW_SCALE
		var skv: Dictionary = sim.vessels["sk"]
		var sk_mash: bool = str(sim.routes[rn]["from"]) == "sk" and not bool(skv.get("bed", false)) and float(skv["malt"]) > 1.0
		if rn == "dilo" and flush_active:
			lpm = float(sim.routes["podraz"]["lpm"]) * BrewSim.FLOW_SCALE * 0.8
		elif sk_mash:
			lpm = minf(lpm, 80.0 * BrewSim.FLOW_SCALE)  # rmut: co nastaví měnič C1, nejvýš plný výkon
		elif str(sim.routes[rn]["from"]) == "sk":
			lpm = sim.sk_draw_lpm()
		lpm *= float(sim.flow_scale.get(rn, 1.0))
		if lpm <= 0.0:
			continue
		var src_key: String = str(sim.routes[rn]["from"])
		if rn == "dilo" and flush_active:
			src_key = "hlt"
		if src_key != "" and src_key != "mains" and sim.vessels.has(src_key):
			var sv: Dictionary = sim.vessels[src_key]
			if float(sv["v"]) < 0.5 and float(sv["malt"]) < 1.0 and float(sim.pipe_ext_total()) < 0.01:
				# zdroj prázdný a v trubkách nic – neteče (obsah trubek jinak zůstává)
				var any_hold := false
				for fpid0 in _flow_pipes[rn]:
					var h0: Dictionary = sim.pipe_hold.get(str(fpid0), {})
					if float(h0.get("v", 0.0)) > 0.1:
						any_hold = true
						break
				if not any_hold:
					continue
		for fpid in _flow_pipes[rn]:
			flow_now[fpid] = maxf(float(flow_now.get(fpid, 0.0)), clampf(lpm / 60.0, 0.25, 1.3))
		var vol: float = lpm * dt / 60.0
		var res_in := 0.0
		if rn == "rmut_sk":
			var rv: Dictionary = sim.vessels["rvp"]
			if float(rv["malt"]) > 1.0:
				res_in = 1.0
		elif sk_mash and not (rn == "dilo" and flush_active):
			res_in = 1.0
		# vizuál z fyzického obsahu trubek (pipe_hold); bez zpoždění fallback na staré čelo
		if sim.pipe_delay:
			for pid in _flow_pipes[rn]:
				var hold: Dictionary = sim.pipe_hold.get(str(pid), {})
				var st: Dictionary = pipe_fill[pid]
				var hv: float = float(hold.get("v", 0.0)) + float(hold.get("malt", 0.0)) * BrewSim.MALT_L_PER_KG
				var cap: float = float(_pipe_vol.get(pid, 4.0))
				if hv > cap * 0.08:
					st["m"] = str(hold.get("med", "water"))
					if st["m"] == "empty":
						st["m"] = "water"
					if res_in > 0.0:
						st["res"] = maxf(float(st["res"]), res_in)
					else:
						var k := 0.6 if (rn == "dilo" and flush_active) else (1.3 if str(st["m"]) == "water" else 3.5)
						st["res"] = float(st["res"]) * exp(-vol / maxf(cap * k, 0.5))
						if float(st["res"]) < 0.04:
							st["res"] = 0.0
				else:
					# prázdná trubka – médium pryč, zbytky mláta mohou zůstat
					if str(st["m"]) != "empty":
						st["m"] = "empty"
				_push_pipe(str(pid))
		else:
			var med: String = str(sim.routes[rn].get("medium", ROUTE_FILL.get(rn, "wort" if str(sim.routes[rn]["from"]) in ["sk", "wp"] else "water")))
			if rn == "rmut_sk":
				var rv2: Dictionary = sim.vessels["rvp"]
				med = "mash" if float(rv2["malt"]) > 1.0 else ("wort" if float(rv2["ext"]) > 0.3 else "water")
			elif sk_mash and not (rn == "dilo" and flush_active):
				med = "mash"
			elif rn == "dilo":
				med = "water" if flush_active else "wort"
			_front[rn] = float(_front.get(rn, 0.0)) + vol
			var acc := 0.0
			for pid2 in _flow_pipes[rn]:
				var pv: float = float(_pipe_vol.get(pid2, 4.0))
				if float(_front[rn]) >= acc + pv * 0.5:
					var st2: Dictionary = pipe_fill[pid2]
					st2["m"] = med
					if res_in > 0.0:
						st2["res"] = maxf(float(st2["res"]), res_in)
					else:
						var k2 := 0.6 if (rn == "dilo" and flush_active) else (1.3 if med == "water" else 3.5)
						st2["res"] = float(st2["res"]) * exp(-vol / maxf(pv * k2, 0.5))
						if float(st2["res"]) < 0.04:
							st2["res"] = 0.0
					_push_pipe(str(pid2))
				acc += pv
	for fpid in pipe_fill:
		# cíl průtoku pro animaci; do 3D se posílá plynule v _smooth_pipe_flow
		_flow_target[str(fpid)] = float(flow_now.get(fpid, 0.0))
	_drain_standing_pipes(dt)
	var clog := 0.0
	for pid in _route_pipes.get("dilo", []):
		clog = maxf(clog, float(pipe_fill[pid]["res"]))
	if clog > 0.3 and not _clog_warned and bool(_flowing.get("dilo", false)) and not flush_active:
		_clog_warned = true
		_log("Potrubí pod síty je zanesené mlátem – propláchni ho vodou směrem do varny")
	if clog < 0.1:
		_clog_warned = false
	sim.clog = clog


## Plynulá animace toku: šipky v trubkách se rozjíždějí a dobíhají, nepřepínají skokem.
## Náběh je rychlý (~0,2 s), doběh pomalejší (~0,5 s) – voda v zavřené trubce ještě chvíli dotéká.
func _smooth_pipe_flow(real_dt: float) -> void:
	if plant == null:
		return
	var dt := clampf(real_dt, 0.0, 0.2)
	for pid in _flow_target:
		var tgt: float = float(_flow_target[pid])
		var cur: float = float(_flow_disp.get(pid, 0.0))
		if cur == tgt and cur == float(_flow_applied.get(pid, -9.0)):
			continue
		var tau: float = 0.22 if tgt > cur else 0.55
		var nv: float = cur + (tgt - cur) * (1.0 - exp(-dt / tau))
		if absf(nv - tgt) < 0.008:
			nv = tgt
		_flow_disp[pid] = nv
		if absf(nv - float(_flow_applied.get(pid, -9.0))) > 0.008:
			_flow_applied[pid] = nv
			plant.call("set_pipe_flow", str(pid), nv)


## Otevřený kanálový vývod vypustí i kapalinu, která zůstala v potrubí po zastavení zdroje.
func _drain_standing_pipes(dt: float) -> void:
	if dt <= 0.0 or net == null:
		return
	var active_pipes: Dictionary = {}
	for rn in _flow_pipes:
		if not bool(_flowing.get(rn, false)):
			continue
		for pid in _flow_pipes[rn]:
			active_pipes[str(pid)] = true
	var remaining := 2.0 * dt
	for pid_value in net.open_drain_pipe_ids():
		var pid := str(pid_value)
		if active_pipes.has(pid) or remaining <= 0.0 or not sim.pipe_hold.has(pid):
			continue
		var hold: Dictionary = sim.pipe_hold[pid]
		var held: float = float(hold.get("v", 0.0)) + float(hold.get("malt", 0.0)) * BrewSim.MALT_L_PER_KG
		if held <= 0.01:
			continue
		var slug: Dictionary = sim._take_pipe(hold, remaining)
		var drained: float = float(slug.get("v", 0.0)) + float(slug.get("malt", 0.0)) * BrewSim.MALT_L_PER_KG
		sim.drain_l += drained
		sim.drain_ext += float(slug.get("ext", 0.0))
		remaining -= drained
		var st: Dictionary = pipe_fill[pid]
		st["m"] = str(hold.get("med", "empty"))
		if float(hold.get("v", 0.0)) < 0.05 and float(hold.get("malt", 0.0)) < 0.05:
			st["m"] = "empty"
		_push_pipe(pid)
		_flow_target[pid] = 0.0  # tok v trubce ustane (animace doběhne v _smooth_pipe_flow)


## proplach: otevře klapky podrážení i díla a zapne čerpadla – voda jde z bojleru pod síta a odtud potrubím do varny
func _flush_pipes() -> void:
	var want: Dictionary = {}
	var pumps: Array = []
	var pr: Dictionary = (plan.raw.get("routes", {}) as Dictionary).get("proplach", {})
	if not pr.is_empty() and sim.routes.has("proplach"):
		# předvolba „proplach“ z plánu: klapky a čerpadla (C4 a C2) podle ní – voda z bojleru jde spodními linkami do sběrače pod síty a přes C2 do varny
		for vid in route_valve_ids("proplach"):
			want[vid] = true
		for pid in pr.get("pumps", [sim.route_pump("proplach")]):
			pumps.append(str(pid))
	else:
		for rn in ["podraz", "dilo"]:
			for vid in route_valve_ids(rn):
				want[vid] = true
		pumps = [sim.route_pump("podraz"), sim.route_pump("dilo")]
	for it in plan.items:
		var t := str(it.get("type"))
		if MACHINE_KINDS.has(str(it.get("kind", ""))):
			continue
		if t == "valve":
			_set_item(it, "open", want.has(str(it.id)))
		elif t == "pump":
			_set_item(it, "on", pumps.has(str(it.id)))
	_log("Proplach potrubí pod síty: voda z bojleru jde pod síta a potrubím do varny")


## sprchy stříkají, když teče trasa, která je napájí (kotel: napouštění vodou, scezovací káď: vysládání, vířivka: přečerpání)
func _update_sprays() -> void:
	for it in plan.items:
		if str(it.get("type")) != "spray":
			continue
		var sx := float(it.x)
		var sy := float(it.y)
		var routes: Array = []
		for v in plan.items:
			if str(v.get("type")) != "vessel" or v.get("bind") == null:
				continue
			if sx >= float(v.x) - 20.0 and sx <= float(v.x) + float(v.w) + 20.0 and sy >= float(v.y) - 80.0 and sy <= float(v.y) + float(v.h):
				match str(v["bind"]):
					"rvp":
						routes = ["voda_panev", "voda_rad"]
					"sk":
						routes = ["vyslazovani"]
					"wp":
						routes = ["varna_virivka"]
				break
		var on := force_sprays
		if str(it.get("kind", "")) == "san":
			routes = []  # sanitační sprchy (CIP) mají vlastní linku – při vyslazování nestříkají, jen když do nich teče
		for rn in routes:
			if bool(_flowing.get(rn, false)):
				on = true
		# sprcha se spustí sama, když do přívodní trubky teče voda
		if bool(plant.call("spray_fed", sx, sy)):
			on = true
		var sid := str(it.id)
		if _spray_state.get(sid, null) != on:
			_spray_state[sid] = on
			plant.call("set_spray", sid, on)


## Rychlé zavření a otevření klapky podrážení (do ~2,5 s) při běžícím čerpadle = vodní ráz, který utrhne kal ode dna
func _detect_kicks() -> void:
	if _podraz_valves.is_empty():
		_podraz_valves = route_valve_ids("podraz")
	var now := Time.get_ticks_msec()
	var pump_on := false
	var pid: String = str(sim.route_pump("podraz"))
	if pid != "":
		pump_on = bool(plan.get_item(pid).get("on", false))
	for vid in _podraz_valves:
		var open_now := bool(plan.get_item(str(vid)).get("open", false))
		if _vstate.has(vid) and bool(_vstate[vid]) != open_now:
			if not open_now:
				_close_ms[vid] = now
			elif now - int(_close_ms.get(vid, -99999)) < 2500 and pump_on and float(sim.vessels["hlt"]["v"]) > 1.0:
				sim.underlet_kick()
		_vstate[vid] = open_now


## usazené vrstvy a zákal: šrot ve varně, kal pod sítem, kal ve vířivce a ve spilce
func _update_sediment() -> void:
	var rv: Dictionary = sim.vessels["rvp"]
	plant.call("set_sediment", str(PLAN_ID["rvp"]), float(rv.get("settled", 0.0)) * minf(float(rv["malt"]) / 500.0, 1.0), 0.0)
	var sk: Dictionary = sim.vessels["sk"]
	plant.call("set_sediment", str(PLAN_ID["sk"]), float(sk.get("fines", 0.0)), float(sk.get("cloud", 0.0)))
	var wp: Dictionary = sim.vessels["wp"]
	var tr: float = float(wp.get("trub", 0.0))
	var ts: float = float(wp.get("tset", 0.0))
	plant.call("set_sediment", str(PLAN_ID["wp"]), tr * ts, tr * (1.0 - ts))
	plant.call("set_sediment", str(PLAN_ID["spilka"]), float(sim.vessels["spilka"].get("trub", 0.0)), 0.0)
	plant.call("set_spin", str(PLAN_ID["rvp"]), 1.1 if (bool(sim.stir.get("rvp", false)) and float(rv["v"]) > 1.0) else 0.0)
	# kopačka: otáčení a poloha zdvihu, proud z výpusti do kanálu
	if _rake_id == "":
		for it in plan.items:
			if str(it.get("type")) == "rake":
				_rake_id = str(it.id)
				break
	if _rake_id != "":
		plant.call("set_spin", _rake_id, 0.6 if sim.rake_on else 0.0)
		plant.call("set_rake_height", _rake_id, float(sk.get("rake_h", 1.0)))
	var drain: Dictionary = _kind_item("valve", "drain")
	if not drain.is_empty():
		plant.call("set_drain_flow", str(drain.id), sim.drain_lpm_now / BrewSim.SK_DRAIN_LPM, sim.drain_turb(), sim.plato("sk") > 0.5)
	# teploměr za chladičem na visačce spílací hadice
	var hose: Dictionary = _kind_item("valve", "hose")
	if not hose.is_empty():
		plant.call("set_aux_info", str(hose.id), ("za chladičem %.1f °C" % sim.cool_out_t) if sim.cool_out_t > -0.5 else "")


# ---------- průhledítka ----------
var inl_views: Dictionary = {}  # id průhledítka -> co je v něm právě vidět (3D sklo i zvětšené okno v main.gd)
var _inl_ids: Array = []
var _inl_sig: int = -1


## průhledítka ukazují, co jimi právě teče: sladinu ze SK podle stavu lože, rmut, mladinu, vodu
func _update_sight_glasses() -> void:
	if _inl_sig != plan.items.size():
		_inl_sig = plan.items.size()
		_inl_ids.clear()
		for it in plan.items:
			if str(it.get("type")) == "inl":
				_inl_ids.append(str(it.id))
	for iid in _inl_ids:
		var v := sight_view(str(plant.call("inl_pipe", iid)))
		inl_views[iid] = v
		plant.call("set_inl_view", iid, v)


## Co je vidět v průhledítku na trubce pid. Vzhled pro shader (medium, liq, zakal, kaly, zrna, mlato, bubliny, flow,
## alpha0) a údaje pro okno: lpm, faze (co je vidět), klic (krátký kód fáze), rada (co dělat), ton (ok / info / warn / bad),
## cesta; u sladiny ze
## scezovací kádě sk = true a navíc plato, ebc, pack (utažení lože), dp (podtlak pod sítem, mbar), trha (trhání lože).
func sight_view(pid: String) -> Dictionary:
	var st: Dictionary = pipe_fill.get(pid, {"m": "empty", "res": 0.0})
	var m := str(st["m"])
	var res: float = clampf(float(st["res"]), 0.0, 1.0)
	var v := {"medium": m, "liq": Color(0.85, 0.93, 0.98), "zakal": 0.0, "kaly": 0.0, "zrna": 0.0, "mlato": 0.0,
		"bubliny": 0.0, "flow": 0.0, "alpha0": 0.92, "lpm": 0.0, "faze": "", "klic": "", "rada": "", "ton": "info", "cesta": "", "sk": false}
	if pid == "" or m == "empty":
		v["medium"] = "empty"
		v["klic"] = "prazdne"
		v["faze"] = "Prázdné – v potrubí nic není" if pid != "" else "Průhledítko nesedí na potrubí"
		return v
	# trasa, která touto trubkou právě teče (přednost má odběr ze SK)
	var rn := ""
	for r in _flowing:
		if not bool(_flowing[r]) or not sim.routes.has(r):
			continue
		var pipes: Array = _flow_pipes.get(r, []) if net_mode else _route_pipes.get(r, [])
		if pipes.has(pid) and (rn == "" or str(sim.routes[r]["from"]) == "sk"):
			rn = str(r)
	var src := ""
	var runs := "stojí v potrubí"
	if rn != "":
		src = str(sim.routes[rn]["from"])
		v["cesta"] = str(sim.routes[rn].get("label", rn))
		# průtok stejně jako v _update_pipes: proplach pod síty, rmut ze SK bez lože, odběr ložem (Darcy, měnič C2)
		var skm: Dictionary = sim.vessels["sk"]
		var lpm: float = float(sim.routes[rn]["lpm"]) * BrewSim.FLOW_SCALE
		if rn == "dilo" and bool(_flowing.get("podraz", false)):
			lpm = float(sim.routes["podraz"]["lpm"]) * BrewSim.FLOW_SCALE * 0.8
		elif src == "sk" and not bool(skm.get("bed", false)) and float(skm["malt"]) > 1.0:
			lpm = minf(lpm, 80.0 * BrewSim.FLOW_SCALE)  # rmut: co nastaví měnič C1, nejvýš plný výkon
		elif src == "sk":
			lpm = sim.sk_draw_lpm()
		lpm *= float(sim.flow_scale.get(rn, 1.0))  # dělení mezi cíle, křivka čerpadla, přivřená klapka – jako v sim._flow
		v["lpm"] = lpm
		v["flow"] = clampf(lpm / 60.0, 0.25, 1.3) if lpm > 0.0 else 0.0
		runs = "teče"
	var sk: Dictionary = sim.vessels["sk"]
	if src == "sk" and m != "water" and bool(sk.get("bed", false)):
		_sight_lauter(v, rn)
	elif m == "mash":
		v["medium"] = "mash"
		v["liq"] = Color(0.80, 0.56, 0.28)
		v["zakal"] = 1.0
		v["kaly"] = 0.8
		v["zrna"] = 0.85
		v["mlato"] = 0.45
		v["faze"] = "Rmut se šrotem – " + runs
		v["klic"] = "rmut"
	elif m == "wort":
		var key := src if src in ["rvp", "wp"] else ""
		var pl: float = sim.plato(key) if key != "" else 12.0
		var ebc: float = sim.recipe_ebc * clampf(pl / 12.0, 0.3, 1.5) * (1.15 if key != "" else 1.0)  # chmelovar barvu prohloubí
		v["liq"] = BrewSim.ebc_color(ebc)
		v["plato"] = pl
		v["ebc"] = ebc
		v["alpha0"] = 0.92
		if key == "rvp":
			v["zakal"] = 0.35
			v["kaly"] = 0.5
			v["bubliny"] = 0.2
			v["faze"] = "Horká mladina z varny – horké kaly (klky) a chmel"
			v["klic"] = "mladina_horka"
		elif key == "wp":
			var wp: Dictionary = sim.vessels["wp"]
			var trub: float = float(wp.get("trub", 0.0)) * (1.0 - float(wp.get("tset", 0.0)))
			v["zakal"] = clampf(0.15 + 0.8 * trub, 0.0, 1.0)
			v["kaly"] = clampf(0.25 + trub, 0.0, 1.0)
			if trub < 0.1:
				v["faze"] = "Zchlazená mladina – čirá, jemné studené kaly"
				v["klic"] = "mladina_chlazena"
				v["ton"] = "ok"
			else:
				v["faze"] = "Mladina s kalem z vířivky – kal se ještě neusadil"
				v["klic"] = "mladina_kal"
				v["rada"] = "Před spíláním nech mladinu ve vířivce v klidu usadit."
				v["ton"] = "warn"
		else:
			v["zakal"] = maxf(0.12, res)
			v["mlato"] = res * 0.8
			v["faze"] = "Sladina – " + runs
			v["klic"] = "sladina"
	else:
		v["medium"] = "water"
		v["alpha0"] = 0.85
		v["zakal"] = 0.6 * res
		v["zrna"] = res
		v["mlato"] = 0.8 * res
		v["bubliny"] = 0.1 if rn != "" else 0.0
		var hot: bool = src == "hlt" and float(sim.vessels["hlt"]["t"]) > 45.0
		if res > 0.05:
			v["faze"] = "Voda vynáší zbytky mláta (proplach)"
			v["klic"] = "proplach"
			v["rada"] = "Proplachuj, dokud voda v průhledítku není čistá."
			v["ton"] = "warn"
		else:
			v["faze"] = ("Horká voda z bojleru (%.0f °C) – " % float(sim.vessels["hlt"]["t"]) if hot else "Voda – čistá, ") + runs
			v["klic"] = "voda"
	return v


## sladina ze scezovací kádě: barva, zákal, kaly, zrna a mláto podle lože (sim.wort_view) a co z toho plyne pro obsluhu
func _sight_lauter(v: Dictionary, rn: String) -> void:
	var w: Dictionary = sim.wort_view()
	var sk: Dictionary = sim.vessels["sk"]
	var pores: float = float(sk["malt"]) * BrewSim.BED_L_PER_KG * 0.45
	var dry: float = clampf(1.0 - float(sk["v"]) / pores, 0.0, 1.0) if pores > 1.0 else 0.0
	for k in ["zakal", "kaly", "zrna", "mlato", "pack", "dp", "plato", "ebc", "trha", "lpm"]:
		v[k] = float(w.get(k, 0.0))
	v["sk"] = true
	v["medium"] = "wort"
	v["liq"] = w["barva"]
	v["alpha0"] = 0.93
	v["bubliny"] = clampf(dry * 1.5 + 0.3 * float(v["trha"]), 0.0, 1.0)  # lože nad hladinou nasává vzduch
	v["flow"] = clampf(float(v["lpm"]) / 60.0, 0.25, 1.3) if float(v["lpm"]) > 0.5 else 0.0
	var z: float = float(v["zakal"])
	var grain := ", občas zrno nebo plucha" if float(v["zrna"]) > 0.04 else ""
	var to: String = str(sim.routes[rn].get("to", ""))
	var back := "Vorlauf běží – vracej do SK, dokud se nevyčeří." if to == "sk" else ("Kalná sladina teče do pánve – vracej ji do SK (vorlauf)." if to == "rvp" else "Vracej ji zpět do SK (vorlauf), dokud se nevyčeří.")
	var fz := ""
	var rd := ""
	var ton := "warn"
	var key := ""
	if float(v["lpm"]) < 1.0:
		key = "stoji"
		fz = "Sladina stojí – lože nepropouští"
		rd = "Zkontroluj hladinu v SK a nakypři lože kopačkou."
	elif sim.hoof_down and sim.rake_on:
		key = "kopyto"
		fz = "Kopyto vyhrnuje mláto – sladina bere mláto"
		rd = "Při vyhrnování mláta sladinu neodebírej."
		ton = "bad"
	elif float(v["trha"]) > 0.15:
		key = "trha"
		fz = "Lože se trhá – kanálky pouštějí kal, zrna a mláto (podtlak %.0f mbar)" % float(v["dp"])
		rd = "Zpomal vyslazovací čerpadlo (C2), nech lože sednout, případně ho nakypři kopačkou."
		ton = "bad"
	elif dry > 0.03:
		key = "sucho"
		fz = "Hladina pod mlátem – lože nasává vzduch, sladina bere mláto"
		rd = "Pusť vyslazovací vodu, nebo zastav odběr, dokud hladina nevystoupí nad mláto."
		ton = "bad"
	elif float(v["mlato"]) > 0.3:
		key = "mlato"
		fz = "Poslední podíly – sladina už bere mláto"
		rd = "Ukonči scezování."
		ton = "bad"
	elif z > 0.75:
		key = "prvni"
		fz = "První sladina – tmavě hnědá, kalná, s jemnými kaly"
		rd = back
	elif z > 0.25:
		key = "kalna"
		fz = "Kalná sladina" + grain
		rd = back
	elif z > 0.1:
		key = "temer_cira"
		fz = "Téměř čirá" + grain
		rd = "Ještě chvíli vracet, pak přepnout scezování do pánve."
		ton = "info"
	elif float(v["plato"]) < 1.5:
		key = "vystrelky"
		fz = "Výstřelky – sladina je už řídká (%.1f °P)" % float(v["plato"])
		rd = "Konec scezování."
		ton = "info"
	else:
		key = "cira"
		fz = "Čirá sladina" + grain
		rd = "Je čirá – přepni z vorlaufu na scezování do pánve." if to == "sk" else ("Teče do pánve – v pořádku." if to == "rvp" else "Scezuj do pánve.")
		if float(v["pack"]) > 0.6:
			rd += " Lože se utahuje (průtok klesá) – nakypři ho kopačkou nebo zpomal čerpadlo."
		ton = "ok"
	v["faze"] = fz
	v["klic"] = key
	v["rada"] = rd
	v["ton"] = ton


## výpusť SK, kopačka, kopyto a spílací hadice: stav prvků v plánu (klik v zařízení) -> simulace
func _sync_sk() -> void:
	sim.sk_drain_open = bool(_kind_item("valve", "drain").get("open", false))
	sim.rake_on = bool(_kind_item("pump", "rake").get("on", false))
	sim.rake_down = bool(_kind_item("valve", "rakelift").get("open", false))
	sim.hoof_down = bool(_kind_item("valve", "hoof").get("open", false))
	var hose: Dictionary = _kind_item("valve", "hose")
	sim.hose_tank = hose.is_empty() or bool(hose.get("open", false))  # plán bez ovladače hadice: rovnou do tanku
	sim.tank_lid = bool(_kind_item("valve", "lid").get("open", false))
	# protláčka: kohout vody otevřený, klapky na cestě k sání C3 („via“) otevřené a C3 běží
	var push: Dictionary = _kind_item("valve", "pushwater")
	var push_on: bool = bool(push.get("open", false))
	if push_on:
		for vid in push.get("via", []):
			if not bool(plan.get_item(str(vid)).get("open", false)):
				push_on = false
				_warn_once("protlacka_via", "Voda na protláčku je otevřená, ale klapka na odbočce k sání C3 je zavřená – voda neteče.")
		var c3: String = sim.route_pump("virivka_chladic")
		if c3 != "" and not bool(plan.get_item(c3).get("on", false)):
			push_on = false
	if push_on or not bool(push.get("open", false)):
		_warned_once.erase("protlacka_via")
	sim.push_flow = push_on


## Spílání s hadicí na vratce teče jen přes otevřenou klapku vratky (jinak C3 tlačí proti zavřené klapce)
func _hose_ok(rn: String) -> bool:
	if rn != "virivka_chladic":
		return true
	var hose: Dictionary = _kind_item("valve", "hose")
	if hose.is_empty() or bool(hose.get("open", false)):
		return true
	var vv: Dictionary = plan.get_item(str(hose.get("vratka_valve", "")))
	if vv.is_empty() or bool(vv.get("open", false)):
		_warned_once.erase("vratka_zavrena")
		return true
	_warn_once("vratka_zavrena", "Spílací hadice je na vratce, ale klapka vratky je zavřená – mladina neteče. Otevři klapku vratky, nebo dej hadici na tank.")
	return false


func _update_visuals() -> void:
	_detect_kicks()
	_update_sediment()
	_update_sprays()
	_update_sight_glasses()
	# přítok do nádob (proud do hladiny): součet tras, které do nádoby právě tečou
	_vessel_inflow.clear()
	_vessel_inflow_mash.clear()
	for rn in sim.routes:
		if not bool(_flowing.get(rn, false)):
			continue
		var to_key := str(sim.routes[rn]["to"])
		if to_key == "" or not PLAN_ID.has(to_key):
			continue
		var lpm: float = float(sim.routes[rn]["lpm"]) * BrewSim.FLOW_SCALE * float(sim.flow_scale.get(rn, 1.0))
		_vessel_inflow[to_key] = float(_vessel_inflow.get(to_key, 0.0)) + maxf(lpm, 0.0)
		var src_key := str(sim.routes[rn]["from"])
		var mash: bool = str(sim.routes[rn].get("medium", "")) == "mash" or (src_key != "" and sim.vessels.has(src_key) and float(sim.vessels[src_key].get("malt", 0.0)) > 1.0)
		if mash:
			_vessel_inflow_mash[to_key] = true
	for key in PLAN_ID:
		var vs: Dictionary = sim.vessels[key]
		var cap: float = float(vs["cap"])
		var kind := "water"
		var frac: float = float(vs["v"]) / cap
		var bed: float = 0.0
		if key == "hlt":
			kind = "hot" if float(vs["t"]) > 45.0 else "water"
		elif key == "lv":
			kind = "water"
		elif key == "rvp":
			kind = "mash" if float(vs["malt"]) > 1.0 else ("wort" if float(vs["ext"]) > 0.3 else "water")
			frac = (float(vs["v"]) + float(vs["malt"]) * BrewSim.MALT_L_PER_KG) / cap
		elif key == "sk":
			var vb: float = (float(vs["malt"]) + float(vs.get("res", 0.0))) * BrewSim.BED_L_PER_KG
			bed = vb / cap
			frac = (float(vs["v"]) + 0.55 * vb) / cap
			kind = "wort" if float(vs["ext"]) > 0.3 or float(vs["malt"]) > 1.0 else "water"  # ráno voda z vyplachování
		elif key == "wp" or key == "spilka":
			kind = "mladina"
		plant.call("set_vessel_state", PLAN_ID[key], frac, kind, bed, float(vs.get("boil", 0.0)), float(vs.get("foam", 0.0)), float(vs["t"]))
		plant.call("set_vessel_inflow", PLAN_ID[key], float(_vessel_inflow.get(key, 0.0)), bool(_vessel_inflow_mash.get(key, false)))
		var v_total: float = float(vs["v"]) + (float(vs["malt"]) * BrewSim.BED_L_PER_KG if key == "sk" else 0.0)
		var txt := "prázdná" if v_total < 0.5 else "%.1f °C · %.1f hl" % [float(vs["t"]), v_total / 100.0]
		plant.call("set_vessel_info", PLAN_ID[key], txt)


## text na displejích 3D rozvaděče a stav kontrolek čerpadel
func _push_panel() -> void:
	var hl: Dictionary = sim.vessels["hlt"]
	var rv: Dictionary = sim.vessels["rvp"]
	var wp: Dictionary = sim.vessels["wp"]
	var lv: Dictionary = sim.vessels["lv"]
	var sk: Dictionary = sim.vessels["spilka"]
	var rows: Array = [
		"BOJLER   %5.1f °C → %4.0f   %4.1f kW %s" % [float(hl["t"]), float(sim.setpoint["hlt"]), float(sim.heater_kw.get("hlt", 0.0)), "AUTO" if bool(sim.auto_mode["hlt"]) else "RUČ"],
		"VARNA   %5.1f °C → %4.0f   %4.1f kW %s%s" % [float(rv["t"]), float(sim.setpoint["rvp"]), float(sim.heater_kw.get("rvp", 0.0)), "AUTO" if bool(sim.auto_mode["rvp"]) else "RUČ", "  VAR" if float(rv.get("boil", 0.0)) > 0.05 else ""],
		"VÍŘIVKA %5.1f °C   %4.0f L" % [float(wp["t"]), float(wp["v"])],
		"LED. VODA %5.1f °C → %4.1f" % [float(lv["t"]), float(sim.setpoint["lv"])],
		"SPILKA  %5.1f °C   %4.0f L" % [float(sk["t"]), float(sk["v"])],
		("PROGRAM %d/%d %3.0f °C  %2.0f/%2.0f min" % [sim.prog_i + 1, sim.prog.size(), float(sim.prog[sim.prog_i]["t"]), sim.prog_hold_s / 60.0, float(sim.prog[sim.prog_i]["min"])]) if sim.prog_run else "PROGRAM STOJÍ",
	]
	var pumps_on: Dictionary = {}
	for it in plan.items:
		if str(it.get("type")) == "pump":
			pumps_on[str(it.id)] = bool(it.get("on", false))
	plant.call("set_panel_text", rows, pumps_on)


## Karta výuky: aktuální krok checklistu (31 kroků) s postupem, vysvětlením a průběžnou kontrolou.
## Nic se nepotvrzuje – splněný krok se sám odškrtne, chyby proti postupu se zapisují.
func _build_training_card() -> void:
	var host: Node = get_parent()
	if host == null:
		return
	_tr_card = PanelContainer.new()
	var sb := StyleBoxFlat.new()
	sb.bg_color = Color(0.07, 0.10, 0.09, 0.94)
	sb.border_color = Color(0.35, 0.78, 0.45, 0.9)
	sb.set_border_width_all(2)
	sb.set_corner_radius_all(8)
	sb.set_content_margin_all(10)
	_tr_card.add_theme_stylebox_override("panel", sb)
	_tr_card.offset_left = 470.0
	_tr_card.offset_top = 96.0
	_tr_card.offset_right = 1090.0
	_tr_card.mouse_filter = Control.MOUSE_FILTER_PASS
	var v := VBoxContainer.new()
	v.add_theme_constant_override("separation", 4)
	_tr_card.add_child(v)
	var head := HBoxContainer.new()
	var tag := Label.new()
	tag.text = "◆ VÝUKA · kontrola podle checklistu"
	tag.add_theme_font_size_override("font_size", 12)
	tag.add_theme_color_override("font_color", Color(0.95, 0.8, 0.25))
	head.add_child(tag)
	_tr_step = Label.new()
	_tr_step.add_theme_font_size_override("font_size", 12)
	_tr_step.modulate = Color(0.7, 0.75, 0.8)
	_tr_step.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	_tr_step.horizontal_alignment = HORIZONTAL_ALIGNMENT_RIGHT
	head.add_child(_tr_step)
	v.add_child(head)
	# pruh postupu: zelená = odškrtnuto, žlutá = právě probíhá, šedá = čeká
	_tr_strip = HBoxContainer.new()
	_tr_strip.add_theme_constant_override("separation", 2)
	for i in training.step_count():
		var cell := ColorRect.new()
		cell.custom_minimum_size = Vector2(17, 7)
		cell.mouse_filter = Control.MOUSE_FILTER_IGNORE
		_tr_strip.add_child(cell)
	v.add_child(_tr_strip)
	_tr_prev = Label.new()
	_tr_prev.add_theme_font_size_override("font_size", 12)
	_tr_prev.add_theme_color_override("font_color", Color(0.49, 0.88, 0.54))
	v.add_child(_tr_prev)
	_tr_title = Label.new()
	_tr_title.add_theme_font_size_override("font_size", 17)
	v.add_child(_tr_title)
	_tr_text_box = VBoxContainer.new()
	_tr_text_box.add_theme_constant_override("separation", 4)
	v.add_child(_tr_text_box)
	_tr_text = Label.new()
	_tr_text.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	_tr_text.custom_minimum_size = Vector2(590, 0)
	_tr_text.add_theme_font_size_override("font_size", 13)
	_tr_text_box.add_child(_tr_text)
	_tr_why = Label.new()
	_tr_why.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	_tr_why.custom_minimum_size = Vector2(590, 0)
	_tr_why.add_theme_font_size_override("font_size", 12)
	_tr_why.add_theme_color_override("font_color", Color(0.56, 0.78, 1.0))
	_tr_text_box.add_child(_tr_why)
	_tr_state = Label.new()
	_tr_state.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	_tr_state.custom_minimum_size = Vector2(590, 0)
	_tr_state.add_theme_font_size_override("font_size", 13)
	_tr_state.add_theme_color_override("font_color", Color(0.93, 0.72, 0.45))
	v.add_child(_tr_state)
	_tr_err = Label.new()
	_tr_err.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	_tr_err.custom_minimum_size = Vector2(590, 0)
	_tr_err.add_theme_font_size_override("font_size", 12)
	_tr_err.add_theme_color_override("font_color", Color(1.0, 0.48, 0.42))
	v.add_child(_tr_err)
	host.add_child(_tr_card)
	_update_training_card()


func _update_training_card() -> void:
	if _tr_card == null or training == null:
		return
	var n: int = training.step_count()
	_tr_step.text = "krok %d / %d" % [mini(training.index + 1, n), n]
	for i in n:
		var cell := _tr_strip.get_child(i) as ColorRect
		if training.finished or i < training.index:
			cell.color = Color(0.35, 0.78, 0.45)
		elif i == training.index:
			cell.color = Color(0.95, 0.8, 0.25)
		else:
			cell.color = Color(0.32, 0.34, 0.38)
	var errs: Array = training.mistakes
	_tr_err.text = "" if errs.is_empty() else "⚠ Chyby proti postupu: %d – poslední: %s" % [errs.size(), str(errs[errs.size() - 1])]
	if training.finished:
		_tr_prev.text = "✓ Všech %d kroků splněno" % n
		_tr_title.text = "Postup dokončen – vyhodnocení várky"
		_tr_text.text = training.result_text()
		_tr_why.text = "Porovnej výsledek s receptem: dokončený postup sám o sobě neznamená kvalitní pivo."
		_tr_state.text = ""
		return
	var st: Dictionary = training.current()
	_tr_prev.text = "" if training.index == 0 else "✓ " + str(training.STEPS[training.index - 1]["title"])
	_tr_title.text = "%d. %s" % [training.index + 1, str(st["title"])]
	_tr_text.text = str(st["text"])
	_tr_why.text = "Proč to děláme? " + str(st["why"])
	_tr_state.text = "⏳ Kontrola: " + str(training.check()["detail"])


func _update_readout() -> void:
	_update_controls()
	_update_training_card()
	_push_panel()
	if _malt_lbl != null:
		var gate: Dictionary = _kind_item("valve", "gate")
		var screw: Dictionary = _kind_item("pump", "screw")
		_malt_lbl.text = "Slad v zásobníku %.0f kg · šoupě %s · šnek %s" % [sim.malt_stock, "otevřené" if bool(gate.get("open", false)) else "zavřené", "běží" if bool(screw.get("on", false)) else "stojí"]
	for rn in _buttons:
		var b: Button = _buttons[rn]
		var f: bool = bool(_flowing.get(rn, false))
		b.text = ("● " if f else "   ") + str(sim.routes[rn]["label"])
		b.modulate = Color(0.6, 1.0, 0.6) if f else (Color(0.55, 0.75, 1.0) if rn == _cur_route else Color(1, 1, 1))
	_update_walk()
	var lines: Array = []
	var m: int = int(sim.clock / 60.0)
	lines.append("čas %d:%02d  (×%d%s)" % [m / 60, m % 60, int(time_scale), " pauza" if paused else ""])
	for key in ["hlt", "rvp", "sk", "wp", "spilka", "lv"]:
		var vs: Dictionary = sim.vessels[key]
		var extra := ""
		if key == "rvp" or key == "sk":
			extra = "  %.1f °P" % sim.plato(key)
			if float(vs["malt"]) > 0.5:
				extra += "  šrot %.0f kg" % float(vs["malt"])
		lines.append("%s: %.0f L · %.1f °C%s" % [str(vs["name"]), float(vs["v"]), float(vs["t"]), extra])
	if float(sim.vessels["rvp"]["malt"]) > 0.5 or float(sim.vessels["sk"]["malt"]) > 0.5:
		var sl: float = sim.starch_left()
		lines.append("jodová zkouška: %s (škrob %.0f %%)" % ["negativní" if sim.iodine_negative() else "pozitivní", sl])
	if sim.active.has("dilo"):
		lines.append("filtrace Darcy: %.0f L/min" % sim.darcy_lpm())
	var rv2: Dictionary = sim.vessels["rvp"]
	if float(rv2.get("settled", 0.0)) > 0.1 and float(rv2["malt"]) > 0.5:
		lines.append("šrot ve varně usazený %.0f %% – zapni míchadlo" % (float(rv2["settled"]) * 100.0))
	var sk2: Dictionary = sim.vessels["sk"]
	if float(sk2.get("fines", 0.0)) > 0.05 or float(sk2.get("cloud", 0.0)) > 0.05:
		var hint: String = "utrhni rázem klapky při podrážení" if float(sk2["malt"]) > 1.0 else "vypláchni výpustí do kanálu"
		lines.append("kal v podsítí %.0f %% · zvířený zákal %.0f %% (%s)" % [float(sk2.get("fines", 0.0)) * 100.0, float(sk2.get("cloud", 0.0)) * 100.0, hint])
	var rh: float = float(sk2.get("rake_h", 1.0))
	var rpos := "nahoře" if rh > 0.98 else ("dole" if rh < 0.02 else ("jede dolů" if sim.rake_down else "jede nahoru"))
	lines.append("kopačka %s, %s · kopyto %s%s" % ["se otáčí (%.1f ot.)" % float(sk2.get("rev", 0.0)) if sim.rake_on else "stojí", rpos, "DOLE" if sim.hoof_down else "nahoře", (" · na sítech zbytky mláta %.0f kg" % float(sk2["res"])) if float(sk2.get("res", 0.0)) > 0.5 else ""])
	if sim.sk_drain_open or sim.drain_l > 0.5:
		var dstate: String = ("teče %.0f L/min, voda %s" % [sim.drain_lpm_now, "čistá" if sim.drain_turb() < 0.1 else "kalná"]) if sim.drain_lpm_now > 0.5 else ("otevřená" if sim.sk_drain_open else "zavřená")
		lines.append("výpusť SK %s · do kanálu celkem %.0f L%s" % [dstate, sim.drain_l, (" (extrakt %.1f kg)" % sim.drain_ext) if sim.drain_ext > 0.05 else ""])
	if bool(sk2.get("bed", false)) and float(sk2.get("pack", 0.0)) > 0.1:
		lines.append("lože utažené %.0f %% – nakypři kopačkou (spustit do lože, nahoru a dolů)" % (float(sk2["pack"]) * 100.0))
	if not _kind_item("valve", "hose").is_empty() and (sim.cool_out_t > -0.5 or sim.line_water < BrewSim.LINE_L - 0.5):
		lines.append("spílací hadice %s · za chladičem %s · voda v potrubí %.0f L" % ["NA TANKU" if sim.hose_tank else "na vratce", ("%.1f °C" % sim.cool_out_t) if sim.cool_out_t > -0.5 else "neteče", sim.line_water])
	if sim.push_flow or sim.pushed_wort > 0.5:
		lines.append("protláčka %s · dotlačeno %.0f L mladiny · voda z protláčky ve spilce %.0f L" % ["BĚŽÍ" if sim.push_flow else "stojí", sim.pushed_wort, sim.push_water_tank])
	if float(sim.vessels["spilka"]["v"]) > 10.0:
		lines.append("víko spilky %s" % ("zavřené (zavíčkováno)" if sim.tank_lid else "otevřené"))
	var wp2: Dictionary = sim.vessels["wp"]
	if float(wp2.get("trub", 0.0)) > 0.05:
		lines.append("kal ve vířivce: usazeno %.0f %%" % (float(wp2.get("tset", 0.0)) * 100.0))
	if sim.clog > 0.05:
		lines.append("potrubí pod síty: zbytky mláta %.0f %% (průtok snížen)" % (sim.clog * 100.0))
	var rv: Dictionary = sim.vessels["rvp"]
	if float(rv.get("boil_s", 0.0)) > 1.0 or float(rv.get("boil", 0.0)) > 0.05:
		var bm: int = int(float(rv.get("boil_s", 0.0)) / 60.0)
		var pct: float = float(rv.get("evap_lph", 0.0)) / maxf(float(rv["v"]), 1.0) * 100.0
		lines.append("VAR %d min · odpar %.1f L/h (%.1f %%/h) · pěna %.0f %%" % [bm, float(rv.get("evap_lph", 0.0)), pct, float(rv.get("foam", 0.0)) * 100.0])
		lines.append("hořkost %.0f IBU · DMS %.0f ppb · %.1f °P" % [sim.ibu(), sim.dms_ppb(), sim.plato("rvp")])
		if bool(rv.get("overflow", false)):
			lines.append("⚠ PŘEKYPĚNÍ – sniž ohřev!")
	lines.append("— protokol —")
	for i in range(maxi(log_lines.size() - 6, 0), log_lines.size()):
		lines.append(str(log_lines[i]))
	_readout.text = "\n".join(lines)


# ---------- ukládání várky ----------
## první prvek plánu daného typu a druhu (šoupě, šnek, vzduch, …), {} když v plánu není
func _kind_item(type: String, kind: String) -> Dictionary:
	for it in plan.items:
		if str(it.get("type")) == type and str(it.get("kind", "")) == kind:
			return it
	return {}


## Šnek na slad ve scéně: šoupě je pneumatické (bez vzduchu se neotevře), šnek sype jen přes otevřené šoupě,
## voda do šneku teče do pánve. Hlídá chyby z checklistu: slad do nemíchané vody, pára do šneku, prázdný zásobník.
func _update_malt_feed(dt: float) -> void:
	var gate: Dictionary = _kind_item("valve", "gate")
	var screw: Dictionary = _kind_item("pump", "screw")
	if gate.is_empty() or screw.is_empty():
		return
	var air: Dictionary = _kind_item("valve", "air")
	var water: Dictionary = _kind_item("valve", "screwwater")
	var gate_open: bool = bool(gate.get("open", false))
	if gate_open and not air.is_empty() and not bool(air.get("open", false)):
		_set_item(gate, "open", false)  # pneumatický pohon bez tlakového vzduchu šoupě neotevře
		_log("Šoupě na slad se neotevřelo – není vzduch. Nejdřív pusť vzduch.")
		gate_open = false
	var screw_on: bool = bool(screw.get("on", false))
	var kg: float = sim.feed_malt(dt, gate_open, screw_on, bool(water.get("open", false)) if not water.is_empty() else false)
	var rv: Dictionary = sim.vessels["rvp"]
	if kg > 0.0 and not bool(sim.stir.get("rvp", false)):
		_warn_once("hrudky", "Slad padá do nemíchané vody – tvoří se hrudky. Zapni míchadlo kotle.")
	if gate_open and (float(rv["t"]) >= 85.0 or float(rv.get("boil", 0.0)) > 0.05):
		_warn_once("para_snek", "Pára jde přes otevřené šoupě do šneku – zavři slad a šoupě.")
	if screw_on and gate_open and sim.malt_stock <= 0.0:
		_warn_once("zasobnik", "Zásobník sladu je prázdný – vypni šnek a zavři šoupě.")
	if not gate_open:
		_warned_once.erase("para_snek")
	if not screw_on:
		_warned_once.erase("hrudky")
		_warned_once.erase("zasobnik")


func _warn_once(key: String, text: String) -> void:
	if _warned_once.has(key):
		return
	_warned_once[key] = true
	_log("⚠ " + text)


func save_state() -> void:
	# testy a skripty (SceneTree bez hlavní scény) uloženou várku uživatele nikdy nepřepisují
	if resume_pending or not is_inside_tree() or get_tree().current_scene == null:
		return
	var valves: Dictionary = {}
	var valve_pct: Dictionary = {}  # přivřené klapky (Ctrl + kolečko): id -> %
	var pumps: Dictionary = {}
	for it in plan.items:
		var t := str(it.get("type"))
		if t == "valve":
			valves[str(it.id)] = bool(it.get("open", false))
			if it.has("pct") and bool(it.get("open", false)):
				valve_pct[str(it.id)] = int(it["pct"])
		elif t == "pump":
			pumps[str(it.id)] = bool(it.get("on", false))
	var manifold_ports: Dictionary = {}
	for it in plan.items:
		if str(it.get("type")) == "manifold":
			manifold_ports[str(it.id)] = (it.get("port_open", {}) as Dictionary).duplicate()
	var data := {"vessels": sim.vessels, "clock": sim.clock, "heater": sim.heater_kw, "valves": valves, "valve_pct": valve_pct, "pumps": pumps, "manifold_ports": manifold_ports, "log": log_lines, "setpoint": sim.setpoint, "auto": sim.auto_mode, "maxkw": sim.max_kw, "prog": sim.prog, "pipes": pipe_fill, "pipe_hold": sim.pipe_hold, "tr": training.index, "trf": training.finished, "trm": training.mistakes, "malt_stock": sim.malt_stock, "drain_l": sim.drain_l, "drain_ext": sim.drain_ext, "spent_out": sim.spent_out, "spent_ext": sim.spent_ext, "malt_total": sim.malt_total, "o2_spilka": sim.o2_spilka, "dead_malt": sim.dead_malt, "hot_break_ext": sim.hot_break_ext, "kal_ext": sim.kal_ext, "yeast_pitched": sim.yeast_pitched, "line_water": sim.line_water, "line_ext": sim.line_ext, "water_to_tank": sim.water_to_tank, "pushed_wort": sim.pushed_wort, "push_water_tank": sim.push_water_tank, "ver": 3}
	var f := FileAccess.open(SAVE_PATH, FileAccess.WRITE)
	if f == null:
		_warn_once("save_state", "Uloženou várku se nepodařilo zapsat: " + error_string(FileAccess.get_open_error()))
		return
	f.store_string(JSON.stringify(data))
	var write_error: Error = f.get_error()
	f.close()
	if write_error != OK:
		_warn_once("save_state", "Uloženou várku se nepodařilo zapsat: " + error_string(write_error))


func load_state() -> bool:
	if not FileAccess.file_exists(SAVE_PATH):
		resume_pending = false
		return false
	var f := FileAccess.open(SAVE_PATH, FileAccess.READ)
	if f == null:
		_log("⚠ Nepodařilo se otevřít uloženou várku: " + error_string(FileAccess.get_open_error()))
		return false
	var data: Variant = JSON.parse_string(f.get_as_text())
	if typeof(data) != TYPE_DICTIONARY:
		_log("⚠ Uložená várka není platný JSON objekt.")
		return false
	var d: Dictionary = data
	if int(d.get("ver", 0)) < 2:
		_log("⚠ Uložená várka je ze staršího modelu a nebyla obnovena.")
		return false
	if d.has("vessels"):
		for k in d["vessels"]:
			if sim.vessels.has(k):
				for kk in d["vessels"][k]:
					sim.vessels[k][kk] = d["vessels"][k][kk]
		if not (d["vessels"].get("sk", {}) as Dictionary).has("res"):
			sim.vessels["sk"]["res"] = 0.0  # várka uložená před výpustí a kopačkou: bez ranních zbytků mláta
	sim.clock = float(d.get("clock", 0.0))
	sim.malt_stock = float(d.get("malt_stock", BrewSim.MALT_BATCH))
	sim.drain_l = float(d.get("drain_l", 0.0))
	sim.drain_ext = float(d.get("drain_ext", 0.0))
	sim.spent_out = float(d.get("spent_out", 0.0))
	sim.spent_ext = float(d.get("spent_ext", 0.0))
	sim.malt_total = float(d.get("malt_total", 0.0))
	sim.o2_spilka = float(d.get("o2_spilka", 0.0))
	sim.dead_malt = float(d.get("dead_malt", 0.0))
	sim.hot_break_ext = float(d.get("hot_break_ext", 0.0))
	sim.kal_ext = float(d.get("kal_ext", 0.0))
	sim.yeast_pitched = bool(d.get("yeast_pitched", false))
	sim.line_water = float(d.get("line_water", 0.0))
	sim.line_ext = float(d.get("line_ext", 0.0))
	sim.water_to_tank = float(d.get("water_to_tank", 0.0))
	sim.pushed_wort = float(d.get("pushed_wort", 0.0))
	sim.push_water_tank = float(d.get("push_water_tank", 0.0))
	for k in d.get("setpoint", {}):
		sim.setpoint[k] = float(d["setpoint"][k])
	for k in d.get("auto", {}):
		sim.auto_mode[k] = bool(d["auto"][k])
	for k in d.get("maxkw", {}):
		sim.max_kw[k] = float(d["maxkw"][k])
	if d.has("pipe_hold") and typeof(d["pipe_hold"]) == TYPE_DICTIONARY:
		for pid in d["pipe_hold"]:
			if pipe_fill.has(pid) and typeof(d["pipe_hold"][pid]) == TYPE_DICTIONARY:
				sim.pipe_hold[pid] = d["pipe_hold"][pid].duplicate(true)
				var hold: Dictionary = sim.pipe_hold[pid]
				pipe_fill[pid] = {"m": str(hold.get("med", "empty")), "res": float(pipe_fill.get(pid, {}).get("res", 0.0))}
				if float(hold.get("v", 0.0)) < 0.05 and float(hold.get("malt", 0.0)) < 0.05:
					pipe_fill[pid] = {"m": "empty", "res": float(pipe_fill[pid]["res"])}
				_push_pipe(str(pid))
	else:
		# Older saves only stored a visual pipe color, not the physical liquid volume.
		for pid in pipe_fill:
			pipe_fill[pid] = {"m": "empty", "res": float(pipe_fill[pid].get("res", 0.0))}
			sim.pipe_hold[pid] = {"v": 0.0, "t": sim.cal_mains_t, "ext": 0.0, "malt": 0.0, "med": "empty"}
			_push_pipe(str(pid))
	for k in _sp_spin:
		(_sp_spin[k] as SpinBox).set_value_no_signal(float(sim.setpoint[k]))
	for k in _mk_spin:
		(_mk_spin[k] as SpinBox).set_value_no_signal(float(sim.max_kw[k]))
		(_auto_ck[k] as CheckButton).set_pressed_no_signal(bool(sim.auto_mode[k]))
	for k in d.get("heater", {}):
		sim.heater_kw[k] = float(d["heater"][k])
	_hlt_slider.set_value_no_signal(float(sim.heater_kw.get("hlt", 0.0)))
	_rvp_slider.set_value_no_signal(float(sim.heater_kw.get("rvp", 0.0)))
	for it in plan.items:
		var id := str(it.id)
		if str(it.get("type")) == "valve" and d.get("valves", {}).has(id):
			_set_item(it, "open", bool(d["valves"][id]))
			var vp := int(d.get("valve_pct", {}).get(id, 100))
			if bool(it.get("open", false)) and int(plant.call("valve_pct", it)) != vp:
				plant.call("set_valve_pct", id, vp)
		elif str(it.get("type")) == "pump" and d.get("pumps", {}).has(id):
			_set_item(it, "on", bool(d["pumps"][id]))
		elif str(it.get("type")) == "manifold" and d.get("manifold_ports", {}).has(id):
			it["port_open"] = d["manifold_ports"][id].duplicate()
	sim._lid_was = bool(_kind_item("valve", "lid").get("open", false))  # po načtení nehlásit znovu zavíčkování
	log_lines = d.get("log", [])
	training.index = clampi(int(d.get("tr", 0)), 0, training.step_count() - 1)
	training.finished = bool(d.get("trf", false))
	training.mistakes = d.get("trm", [])
	training._enter()
	_log("Várka obnovena")
	resume_pending = false
	return true


func new_batch() -> void:
	resume_pending = false
	sim.reset()
	# FYZIKA+ oprava: nová várka začíná s bojlerem napuštěným a nahřátým (4000 L, 80 °C). Žádná cesta ho nenapouští
	# a prázdný se nehřeje, takže várka (ruční i automat, krok 4) nešla začít.
	sim.vessels["hlt"]["v"] = 4000.0
	sim.vessels["hlt"]["t"] = 80.0  # Vasil 23. 11.: na páru se čekat nemusí – bojler je na začátku várky už nahřátý
	sim.clog = 0.0
	_front.clear()
	sim.init_pipe_caps(_pipe_vol)
	for pid in pipe_fill:
		pipe_fill[pid] = {"m": "empty", "res": 0.0}
		_push_pipe(str(pid))
	_hlt_slider.set_value_no_signal(0.0)
	_rvp_slider.set_value_no_signal(0.0)
	for it in plan.items:
		var t := str(it.get("type"))
		if t == "valve":
			_set_item(it, "open", bool(_orig_open.get(str(it.id), false)))
		elif t == "pump":
			_set_item(it, "on", false)
	_set_manifold_ports("")
	log_lines.clear()
	training.reset()
	_log("Nová várka")
	save_state()
