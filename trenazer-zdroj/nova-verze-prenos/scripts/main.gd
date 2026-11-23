extends Control
## Sklo 3D + editor plan.json — E, LMB, RMB klapka, N nová trubka, Enter hotovo, Del smazat, Ctrl+S.

@onready var plant: Node3D = $SubViewportContainer/SubViewport/World/Plant3D
@onready var cam: Camera3D = $SubViewportContainer/SubViewport/World/Camera3D
@onready var vp: SubViewport = $SubViewportContainer/SubViewport
@onready var vpc: SubViewportContainer = $SubViewportContainer
@onready var status: Label = $UI/TopBar/Status
@onready var btn_editor: Button = $UI/TopBar/BtnEditor
@onready var btn_see: Button = $UI/TopBar/BtnSeeThrough
@onready var btn_new_pipe: Button = $UI/TopBar/BtnNewPipe
@onready var btn_save: Button = $UI/TopBar/BtnSave
@onready var info: Label = $UI/SideInfo

var plan: PlanData = PlanData.new()
var _panning: bool = false
var _pan_last: Vector2 = Vector2.ZERO
var _dragging: bool = false
var _capture_path: String = ""
var _capture_mode: String = "default"
var _capture_frame: int = 0
var _capture_wait: int = 14  # kolik snímků počkat před focením (měrka potřebuje několik sekund)
var _undo: Array = []
var _redo: Array = []
var _pre_snap: String = ""
var _plan_dirty: bool = false
var _autosave_acc: float = 0.0
var _drag_moved: bool = false
var _place_valve: bool = false
var _updating_props: bool = false
var btn_photo: Button
var btn_resume: Button
var btn_fresh: Button
var sim_panel: SimPanel
var _view_moved: bool = false
var _hose_drag: String = ""  # ovladač hadice, jejíž konec se právě tahá myší
var _hose_moved: bool = false
var _view_press: Vector2 = Vector2.ZERO
var _play_btn: Button
var _cabinet_btn: Button
var _cabinet_hide_btn: Button
var props: PanelContainer
var p_title: Label
var p_dn_row: HBoxContainer
var p_dn: SpinBox
var p_med_row: HBoxContainer
var p_med: OptionButton
var p_open: CheckBox
var p_label_row: HBoxContainer
var p_label: LineEdit
var p_btn_delpt: Button

const LOOKS := ["mash", "wort", "water", "steel"]

func _ready() -> void:
	vp.handle_input_locally = false
	vp.msaa_3d = Viewport.MSAA_4X  # vyhlazení hran trubek a nádob
	vp.screen_space_aa = Viewport.SCREEN_SPACE_AA_FXAA
	vp.use_taa = false
	vpc.mouse_filter = Control.MOUSE_FILTER_IGNORE
	var err: Error = plan.load_plan("res://plan.json")
	if err != OK:
		status.text = "Chyba načtení plan.json"
		return
	plant.call("setup", plan)
	_fit_camera()
	btn_editor.toggled.connect(_on_editor_toggled)
	btn_see.toggled.connect(_on_see_toggled)
	btn_new_pipe.pressed.connect(_on_new_pipe)
	btn_save.pressed.connect(_on_save)
	plant.connect("selection_changed", _on_sel)
	btn_see.button_pressed = true
	plant.call("set_see_through", true)
	_refresh_buttons()
	status.text = "E editor · N nová trubka · Enter hotovo · Del smazat · Ctrl+S"
	$UI/TopBar/Title.text = "Varna"
	_build_legend()
	_build_photo_button()
	_build_edit_buttons()
	_build_route_picker()
	sim_panel = SimPanel.new()
	sim_panel.setup(plan, plant)
	$UI.add_child(sim_panel)
	sim_panel.visible = false  # varna se ovládá přes rozvaděč RT.20; sim_panel zůstává jako výpočetní jádro
	_build_start_choice()
	_build_view_bar()
	checklist_panel = ChecklistPanel.new()
	checklist_panel.setup(self, plan)
	$UI.add_child(checklist_panel)
	route_editor = RouteEditor.new()
	route_editor.setup(self, plan, plant, sim_panel)
	$UI.add_child(route_editor)
	var rt = preload("res://scripts/rozvadec_rt20.gd").new()
	rt.setup(self, sim_panel)
	rozvadec = rt
	$UI.add_child(rozvadec)
	rozvadec.visible = false  # RT.20 běží skrytý jako řídicí vrstva; ovládá se přes skříň (rozvadec_skrin.gd)
	skrin = preload("res://scripts/rozvadec_skrin.gd").new()
	skrin.setup(self, rt)
	$UI.add_child(skrin)
	skrin.visible = false  # na přání uživatele ji nahrazuje rozvaděč přesně podle původní aplikace (rozvadec_puvodni.gd)
	var pv = preload("res://scripts/rozvadec_puvodni.gd").new()
	pv.setup(self, rt)
	puvodni = pv
	$UI.add_child(puvodni)
	_build_cabinet_hide_button()
	var op = preload("res://scripts/panel_puvodni.gd").new()
	op.setup(self, rt, pv)
	ovladani = op
	$UI.add_child(ovladani)
	protokol = preload("res://scripts/protokol_okno.gd").new()
	protokol.setup(sim_panel, rt)
	$UI.add_child(protokol)
	grafy = preload("res://scripts/grafy_okno.gd").new()
	grafy.setup(sim_panel, rt)
	$UI.add_child(grafy)
	var sl = preload("res://scripts/sladek_panel.gd").new()
	sl.setup(self, sim_panel)
	sladek = sl
	$UI.add_child(sladek)
	for c in $UI/TopBar.get_children():
		if c is Button:
			(c as Button).focus_mode = Control.FOCUS_NONE
	_build_props()
	_parse_capture_args()


func _build_photo_button() -> void:
	var b := Button.new()
	btn_photo = b
	b.toggle_mode = true
	b.text = "Foto render: VYP"
	b.toggled.connect(func(on: bool) -> void:
		if bool(plant.call("set_photo_mode", on)):
			b.text = "Foto render: ZAP" if on else "Foto render: VYP"
			status.text = "Foto render ukazuje stav při posledním renderu (Render foto.bat)" if on else "Živý 3D náhled"
		else:
			b.set_pressed_no_signal(false)
			status.text = "Chybí assets/render.png — spusť Render foto.bat"
	)
	$UI/TopBar.add_child(b)
	$UI/TopBar.move_child(b, btn_see.get_index() + 1)
	var hb := Button.new()
	hb.toggle_mode = true
	hb.text = "Hybrid (render + živé): VYP"
	hb.toggled.connect(func(on: bool) -> void:
		if bool(plant.call("set_hybrid", on)):
			hb.text = "Hybrid (render + živé): ZAP" if on else "Hybrid (render + živé): VYP"
			status.text = "Hybrid: pozadí je Blender render, kapaliny a trubky jsou živé" if on else "Živý 3D náhled"
		else:
			hb.set_pressed_no_signal(false)
			status.text = "Chybí assets/render_static.png — spusť Render pozadi.bat"
	)
	$UI/TopBar.add_child(hb)
	$UI/TopBar.move_child(hb, b.get_index() + 1)


func _build_start_choice() -> void:
	if not sim_panel.resume_pending:
		return
	btn_resume = Button.new()
	btn_resume.text = "Obnovit várku"
	btn_resume.tooltip_text = "Načte naposledy uložený stav simulace."
	btn_resume.pressed.connect(func() -> void:
		if sim_panel.load_state():
			btn_resume.visible = false
			btn_fresh.visible = false
			status.text = "Obnovena poslední uložená várka."
	)
	btn_fresh = Button.new()
	btn_fresh.text = "Začít naprázdno"
	btn_fresh.tooltip_text = "Zahodí běhový stav pro novou várku; plán potrubí zůstane zachovaný."
	btn_fresh.pressed.connect(func() -> void:
		sim_panel.new_batch()
		btn_resume.visible = false
		btn_fresh.visible = false
		status.text = "Nová prázdná várka: nádoby i potrubí jsou prázdné, klapky zavřené."
	)
	$UI/TopBar.add_child(btn_resume)
	$UI/TopBar.move_child(btn_resume, status.get_index())
	$UI/TopBar.add_child(btn_fresh)
	$UI/TopBar.move_child(btn_fresh, status.get_index())


const ROUTE_LABELS := {
	"vyslazovani": "Vyslazování (červená)",
	"podraz": "Proplach tryskami pod síty (zelená)",
	"rmut_sk": "Rmut do scezovací kádě",
	"varna_virivka": "Varna do vířivky (čerpadlo C1)",
	"virivka_chladic": "Vířivka přes chladič do spilky (čerpadlo C3)",
	"dilo": "Dílo",
	"voda_panev": "Voda do varny",
	"voda_rad": "Voda rad",
	"lv_fill": "Napuštění ledové vody",
	"ledovka": "Ledová voda",
	"glykol": "Glykol",
}


func _build_route_picker() -> void:
	var ob := OptionButton.new()
	_route_ob = ob
	refresh_route_picker()
	ob.item_selected.connect(func(i: int) -> void:
		var key: String = "" if i == 0 else str(ob.get_item_metadata(i))
		plant.call("show_route", key)
		status.text = "Cesta zvýrazněna, žluté čtverečky = klapky k otevření" if key != "" else "Cesta skryta"
	)
	$UI/TopBar.add_child(ob)
	$UI/TopBar.move_child(ob, btn_see.get_index() + 1)


## seznam cest v horní liště (volá i editor cest po uložení); vlastní cesty mají název z plánu
func refresh_route_picker() -> void:
	var ob := _route_ob
	ob.clear()
	ob.add_item("Cesta: žádná")
	var keys: Array = []
	for k in plant.call("route_names"):
		keys.append(str(k))
	keys.sort()
	var routes: Dictionary = plan.raw.get("routes", {})
	for k in keys:
		var lb: String = str(routes.get(k, {}).get("label", ROUTE_LABELS.get(k, k)))
		ob.add_item(lb)
		ob.set_item_metadata(ob.item_count - 1, k)


func _build_edit_buttons() -> void:
	var bar: HBoxContainer = $UI/TopBar
	var idx: int = btn_new_pipe.get_index() + 1
	for spec in [["Klapka (V)", "_on_place_valve"], ["Zpět", "_do_undo"], ["Vpřed", "_do_redo"]]:
		var b := Button.new()
		b.text = spec[0]
		b.pressed.connect(Callable(self, spec[1]))
		bar.add_child(b)
		bar.move_child(b, idx)
		idx += 1


func _row(parent: Control, text: String) -> HBoxContainer:
	var hb := HBoxContainer.new()
	var lb := Label.new()
	lb.text = text
	lb.custom_minimum_size = Vector2(120, 0)
	hb.add_child(lb)
	parent.add_child(hb)
	return hb


func _build_props() -> void:
	props = PanelContainer.new()
	var sb := StyleBoxFlat.new()
	sb.bg_color = Color(0.08, 0.09, 0.11, 0.9)
	sb.set_corner_radius_all(6)
	sb.set_content_margin_all(10)
	props.add_theme_stylebox_override("panel", sb)
	props.anchor_left = 1.0
	props.anchor_right = 1.0
	props.offset_left = -340.0
	props.offset_right = -16.0
	props.offset_top = 330.0
	var box := VBoxContainer.new()
	box.add_theme_constant_override("separation", 6)
	props.add_child(box)
	p_title = Label.new()
	p_title.add_theme_color_override("font_color", Color(0.95, 0.8, 0.25))
	box.add_child(p_title)
	p_dn_row = _row(box, "DN (tloušťka)")
	p_dn = SpinBox.new()
	p_dn.min_value = 10
	p_dn.max_value = 150
	p_dn.step = 5
	p_dn.value_changed.connect(_on_dn)
	p_dn_row.add_child(p_dn)
	p_med_row = _row(box, "Médium")
	p_med = OptionButton.new()
	for t in ["rmut (oranžová)", "sladina (žlutá)", "voda (modrá)", "nerez"]:
		p_med.add_item(t)
	p_med.item_selected.connect(_on_med)
	p_med_row.add_child(p_med)
	p_open = CheckBox.new()
	p_open.text = "Klapka otevřená"
	p_open.toggled.connect(_on_open)
	box.add_child(p_open)
	p_label_row = _row(box, "Popisek")
	p_label = LineEdit.new()
	p_label.custom_minimum_size = Vector2(170, 0)
	p_label.text_submitted.connect(_on_label)
	p_label_row.add_child(p_label)
	p_btn_delpt = Button.new()
	p_btn_delpt.text = "Smazat vybraný bod trubky"
	p_btn_delpt.pressed.connect(_on_del_point)
	box.add_child(p_btn_delpt)
	var b_dup := Button.new()
	b_dup.text = "Duplikovat (Ctrl+D)"
	b_dup.pressed.connect(_on_duplicate)
	box.add_child(b_dup)
	var b_del := Button.new()
	b_del.text = "Smazat prvek (Del)"
	b_del.pressed.connect(_delete_selected)
	box.add_child(b_del)
	var hint := Label.new()
	hint.text = "Shift+klik na trubku = nový bod\nCtrl+Z / Ctrl+Y zpět / vpřed"
	hint.add_theme_font_size_override("font_size", 13)
	hint.modulate = Color(0.75, 0.78, 0.82)
	box.add_child(hint)
	props.visible = false
	$UI.add_child(props)


func _selected() -> String:
	return str(plant.get("selected_id"))


func _update_props() -> void:
	var id := _selected()
	var it: Dictionary = plan.get_item(id) if id != "" else {}
	props.visible = not it.is_empty() and bool(plant.get("editor_mode"))
	if not props.visible:
		return
	_updating_props = true
	var typ := str(it.get("type"))
	p_title.text = "%s · %s" % [typ, id]
	p_dn_row.visible = typ == "pipe"
	p_med_row.visible = typ == "pipe"
	p_btn_delpt.visible = typ == "pipe"
	p_open.visible = typ == "valve"
	p_label_row.visible = it.has("label") or typ in ["valve", "pump", "vessel"]
	if typ == "pipe":
		p_dn.value = float(it.get("dn", 25))
		p_med.select(maxi(LOOKS.find(str(plant.call("_pipe_medium", it))), 0))
	if typ == "valve":
		p_open.button_pressed = bool(it.get("open", false))
	p_label.text = str(it.get("label", ""))
	_updating_props = false


func _on_dn(v: float) -> void:
	var id := _selected()
	if _updating_props or id == "":
		return
	_push_undo()
	plan.get_item(id)["dn"] = v
	plant.call("rebuild_item", id)


func _on_med(idx: int) -> void:
	var id := _selected()
	if _updating_props or id == "":
		return
	_push_undo()
	var it := plan.get_item(id)
	it["look"] = LOOKS[idx]
	if LOOKS[idx] in ["mash", "wort"]:
		it["medium"] = LOOKS[idx]
	plant.call("rebuild_item", id)


func _on_open(on: bool) -> void:
	var id := _selected()
	if _updating_props or id == "":
		return
	_push_undo()
	plan.get_item(id)["open"] = on
	plant.call("rebuild_item", id)


func _on_label(text: String) -> void:
	var id := _selected()
	if id == "":
		return
	_push_undo()
	plan.get_item(id)["label"] = text
	plant.call("rebuild_item", id)


func _on_del_point() -> void:
	var id := _selected()
	if id == "":
		return
	var snap := _snap()
	if bool(plant.call("delete_pipe_point", id)):
		_push_undo(snap)
		status.text = "Bod trubky smazán"
	else:
		status.text = "Vyber bod trubky (oranžový čtvereček); trubka musí mít aspoň 2 body"


func _on_duplicate() -> void:
	var id := _selected()
	if id == "":
		return
	var snap := _snap()
	var nid := str(plant.call("duplicate_item", id))
	if nid != "":
		_push_undo(snap)
		status.text = "Duplikováno: " + nid


func _on_place_valve() -> void:
	if not bool(plant.get("editor_mode")):
		btn_editor.button_pressed = true
	plant.call("cancel_draw_pipe")
	_place_valve = true
	status.text = "Nová klapka: klikni do plánu (Esc zrušit)"


func _snap() -> String:
	return JSON.stringify(plan.items)


func _push_undo(s: String = "") -> void:
	_undo.append(s if s != "" else _snap())
	if _undo.size() > 80:
		_undo.pop_front()
	_redo.clear()
	_plan_dirty = true
	btn_save.text = "Uložit*"


func _restore(s: String) -> void:
	var data: Variant = JSON.parse_string(s)
	if typeof(data) != TYPE_ARRAY:
		return
	plan.items = data
	plan.raw["items"] = data
	var ed: bool = bool(plant.get("editor_mode"))
	plant.call("setup", plan)
	plant.set("editor_mode", ed)
	plant.call("set_see_through", bool(plant.get("see_through_pipes")))
	plant.call("set_selected", "")


func _do_undo() -> void:
	if _undo.is_empty():
		status.text = "Není co vrátit"
		return
	_redo.append(_snap())
	_restore(_undo.pop_back())
	_plan_dirty = true
	btn_save.text = "Uložit*"
	status.text = "Zpět"


func _do_redo() -> void:
	if _redo.is_empty():
		status.text = "Není co vrátit vpřed"
		return
	_undo.append(_snap())
	_restore(_redo.pop_back())
	_plan_dirty = true
	btn_save.text = "Uložit*"
	status.text = "Vpřed"


func _build_legend() -> void:
	var panel := PanelContainer.new()
	panel.name = "Legenda"  # skříň rozvaděče ji posouvá vedle sebe (rozvadec_puvodni._push_help)
	var sb := StyleBoxFlat.new()
	sb.bg_color = Color(0.08, 0.09, 0.11, 0.88)
	sb.set_corner_radius_all(6)
	sb.set_content_margin_all(12)
	panel.add_theme_stylebox_override("panel", sb)
	panel.anchor_top = 1.0
	panel.anchor_bottom = 1.0
	panel.offset_left = 16.0
	panel.offset_top = -190.0
	panel.offset_bottom = -70.0
	panel.offset_right = 140.0
	var box := VBoxContainer.new()
	box.add_theme_constant_override("separation", 6)
	panel.add_child(box)
	for row in [["rmut", Color("c47a42")], ["sladina", Color("f2c230")], ["voda", Color("2f8fe0")]]:
		var hb := HBoxContainer.new()
		hb.add_theme_constant_override("separation", 10)
		var sw := ColorRect.new()
		sw.custom_minimum_size = Vector2(24, 24)
		sw.color = row[1]
		var lb := Label.new()
		lb.text = row[0]
		lb.add_theme_font_size_override("font_size", 20)
		hb.add_child(sw)
		hb.add_child(lb)
		box.add_child(hb)
	$UI.add_child(panel)


func _fit_camera() -> void:
	var focus := Rect2(-80, 350, 2600, 900)
	cam.projection = Camera3D.PROJECTION_ORTHOGONAL
	cam.size = maxf(focus.size.x, focus.size.y) * 0.62
	cam.near = 0.1
	cam.far = 20000.0
	cam.position = Vector3(focus.get_center().x, -focus.get_center().y, 1800)
	cam.rotation_degrees = Vector3(0, 0, 0)


func _refresh_buttons() -> void:
	var ed: bool = bool(plant.get("editor_mode"))
	var see: bool = bool(plant.get("see_through_pipes"))
	var drawing: bool = bool(plant.get("draw_mode"))
	btn_editor.text = "Editor: ZAP" if ed else "Editor: VYP"
	btn_see.text = "Průhled trubek: ZAP" if see else "Průhled trubek: VYP"
	btn_new_pipe.disabled = not ed
	btn_new_pipe.text = "Trubka… (klik)" if drawing else "Nová trubka (N)"


func _on_editor_toggled(on: bool) -> void:
	plant.set("editor_mode", on)
	if on and btn_photo != null and btn_photo.button_pressed:
		btn_photo.button_pressed = false
	if not on:
		plant.call("cancel_draw_pipe")
		_dragging = false
		plant.call("end_drag")
	plant.call("refresh_handles")
	_refresh_buttons()
	_update_props()
	if on:
		status.text = "Editor ZAP — N nová trubka · Del smaže celou trubku · Enter dokončí kreslení"
	else:
		status.text = "Editor VYP"


func _on_see_toggled(on: bool) -> void:
	plant.call("set_see_through", on)
	_refresh_buttons()


func _on_new_pipe() -> void:
	if not bool(plant.get("editor_mode")):
		btn_editor.button_pressed = true
	plant.call("start_draw_pipe")
	_dragging = false
	_refresh_buttons()
	status.text = "Nová trubka: LMB body · Enter hotovo (≥2) · Esc zrušit"


func _on_save() -> void:
	var err: Error = plan.save_plan()
	if err == OK:
		_plan_dirty = false
		_autosave_acc = 0.0
		btn_save.text = "Uložit plan"
		status.text = "Uloženo user://plan_edited.json"
	else:
		status.text = "Uložení plánu selhalo: " + error_string(err)


func _on_sel(id: String) -> void:
	_update_props()
	if id == "":
		info.text = "E editor\nN nová trubka\nEnter dokončit\nDel smazat celek\nRMB klapka\nCtrl+S uložit"
		return
	var it: Dictionary = plan.get_item(id)
	info.text = "Vybráno: %s\ntype=%s\n%s" % [id, it.get("type", "?"), JSON.stringify(it, "  ").left(500)]


func _over_chrome() -> bool:
	return get_viewport().get_mouse_position().y <= 88.0


## myš je nad otevřeným panelem (vlastnosti, rozvaděč, checklist, editor cest, průhledítko): klik a kolečko patří panelu, ne scéně
func _over_panel(pos: Vector2) -> bool:
	for p in [props, sim_panel, checklist_panel, route_editor, _sight_panel, rozvadec, skrin, skrin._lupa, sladek, puvodni, ovladani, protokol, grafy]:
		if p != null and is_instance_valid(p) and (p as Control).visible and (p as Control).get_global_rect().has_point(pos):
			return true
	return false


func _zoom(f: float, screen: Vector2) -> void:
	var before: Vector2 = plant.call("screen_to_plan", cam, screen)
	cam.size = clampf(cam.size * f, 120.0, 9500.0)
	var after: Vector2 = plant.call("screen_to_plan", cam, screen)
	cam.position += Vector3(before.x - after.x, -(before.y - after.y), 0)


## přesune kameru na střed (souřadnice plánu) tak, aby se vešla výška h
func _focus(center: Vector2, h: float, width: float = 0.0) -> void:
	var aspect: float = maxf(vpc.size.x / maxf(vpc.size.y, 1.0), 0.5)
	cam.size = maxf(h, width / aspect)
	cam.position = Vector3(center.x, -center.y, cam.position.z)


## Ctrl + kolečko nad klapkou: pootevřít / přivřít o 10 % jako v původní aplikaci („natrhnout“ cestu); mimo klapku se nic nestane.
func _valve_step(screen: Vector2, step: int) -> void:
	var hit: Dictionary = plant.call("pick_at_screen", cam, screen, _pick_tol())
	var id := str(hit.get("id", ""))
	var it: Dictionary = plan.get_item(id)
	if it.is_empty() or not bool(plant.call("can_throttle", it)):
		status.text = "Ctrl + kolečko nad klapkou ji pootevře nebo přivře po 10 %."
		return
	var pct: int = int(plant.call("set_valve_pct", id, int(plant.call("valve_pct", it)) + step))
	var nm := str(it.get("label", ""))
	var who := "Klapka" if nm == "" or nm.to_lower() == "klapka" else "Klapka " + nm
	status.text = "%s: %s (Ctrl + kolečko)" % [who, ("otevřeno %d %%" % pct) if pct > 0 else "zavřená"]


func _click_item(screen: Vector2) -> void:
	var hit: Dictionary = plant.call("pick_at_screen", cam, screen, _pick_tol())
	if hit.is_empty():
		return
	var id := str(hit.get("id", ""))
	# klapka na portu rozbočovače (id "manifold_…#L0")
	if "#" in id:
		var parts := id.split("#")
		if parts.size() == 2:
			var now: bool = bool(plant.call("manifold_port_toggle", str(parts[1])))
			status.text = "Rozbočovač %s: klapka %s" % [str(parts[1]), "OTEVŘENÁ" if now else "zavřená"]
			if sim_panel != null and sim_panel.net != null:
				sim_panel.net._cache_sig = ""  # hned přepočítat tok
			return
	var it: Dictionary = plan.get_item(id)
	if route_editor != null and route_editor.visible and str(it.get("type")) == "pipe":
		route_editor.on_pipe_clicked(id, plant.call("screen_to_plan", cam, screen))
		return
	match str(it.get("type")):
		"valve":
			plant.call("click_item", id)
			status.text = "Klapka %s: %s" % [str(it.get("label", id)), "OTEVŘENÁ" if bool(it.get("open", false)) else "zavřená"]
			if route_editor != null:
				route_editor.on_item_clicked(id)
		"pump":
			plant.call("click_item", id)
			status.text = "Čerpadlo %s: %s" % [str(it.get("label", id)), "BĚŽÍ" if bool(it.get("on", false)) else "stojí"]
			if route_editor != null:
				route_editor.on_item_clicked(id)
		"inl":
			_open_sight(id)
		"dip":
			plant.call("click_item", id)
			status.text = "Měrka: vložit do nádoby / odečíst (klik na mokrý klacek)"
		"gauge":
			plant.call("click_item", id)
			status.text = "Stavoznak: kohouty %s" % ("ZAVŘENÉ (hladina zůstává)" if bool(it.get("closed", false)) else "otevřené")
		"vessel":
			var c := Vector2(float(it.x) + float(it.get("w", 0)) * 0.5, float(it.y) + float(it.get("h", 0)) * 0.5)
			_focus(c, maxf(float(it.get("h", 300)) * 2.2, 700.0))


var checklist_panel: ChecklistPanel
var route_editor: RouteEditor
var rozvadec: PanelContainer  # rozvaděč RT.20 (rozvadec_rt20.gd), běží skrytý jako řídicí vrstva
var skrin: PanelContainer  # rozvaděč jako nerezová skříň vpravo (rozvadec_skrin.gd), přes něj se ovládá celá varna
var sladek: PanelContainer  # Digitální sládek – chat s pivovarským učitelem (sladek_panel.gd)
var puvodni: Control  # rozvaděč RT.20 přesně jako v původní aplikaci Trenažér varny (rozvadec_puvodni.gd), vlevo jako v hale
var grafy: PanelContainer  # živé grafy várky (grafy_okno.gd)
var protokol: PanelContainer  # varní protokol jako okno (protokol_okno.gd)
var ovladani: PanelContainer  # ovládací panel vpravo přesně jako v původní aplikaci (panel_puvodni.gd), nahrazuje detail RT.20
var _route_ob: OptionButton
var _sight_panel: PanelContainer
var _sight_cam: Camera3D
var _sight_id: String = ""
var _sight_title: Label
var _sight_phase: Label
var _sight_tip: Label
var _sight_nums: Label
var _sight_bars: Dictionary = {}  # klíč vzhledu -> [popis, ProgressBar, hodnota]
const SIGHT_W := 600
const SIGHT_TONES := {"ok": Color(0.45, 0.9, 0.45), "info": Color(0.75, 0.85, 1.0), "warn": Color(1.0, 0.75, 0.3), "bad": Color(1.0, 0.42, 0.35)}


## Průhledítko: po kliknutí se vedle skříně rozvaděče otevře velký živý pohled do skla (vlastní kamera, sklo vždy vodorovně) a pod ním,
## co je vidět: fáze sladiny, co dělat, průtok, °P, barva, zákal, kaly, zrna, mláto a utažení lože (sim_panel.sight_view)
func _open_sight(id: String) -> void:
	var it: Dictionary = plan.get_item(id)
	if it.is_empty():
		return
	_sight_id = id
	if _sight_panel == null:
		_build_sight_panel()
	_sight_panel.visible = true
	var fr: Dictionary = plant.call("inl_frame", id)
	if fr.is_empty():
		_sight_cam.position = Vector3(float(it.x), -float(it.y), 600.0)
		_sight_cam.rotation = Vector3.ZERO
		_sight_cam.size = 40.0
	else:
		var p: Vector3 = fr["pos"]
		_sight_cam.position = p + Vector3(0, 0, 600.0)
		_sight_cam.rotation = Vector3(0, 0, float(fr["rot"]))
		_sight_cam.size = 31.0 * float(fr["scale"])
	_update_sight()


func _build_sight_panel() -> void:
	_sight_panel = PanelContainer.new()
	var sb := StyleBoxFlat.new()
	sb.bg_color = Color(0.08, 0.09, 0.11, 0.96)
	sb.set_corner_radius_all(8)
	sb.set_content_margin_all(10)
	_sight_panel.add_theme_stylebox_override("panel", sb)
	_sight_panel.offset_left = 12.0
	_sight_panel.offset_top = 96.0
	var box := VBoxContainer.new()
	box.add_theme_constant_override("separation", 6)
	_sight_panel.add_child(box)
	var head := HBoxContainer.new()
	_sight_title = Label.new()
	_sight_title.text = "PRŮHLEDÍTKO"
	_sight_title.add_theme_color_override("font_color", Color(0.95, 0.8, 0.25))
	_sight_title.add_theme_font_size_override("font_size", 17)
	_sight_title.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	head.add_child(_sight_title)
	var close := Button.new()
	close.text = "✕"
	close.focus_mode = Control.FOCUS_NONE
	close.pressed.connect(func() -> void:
		_sight_panel.visible = false
		_sight_id = "")
	head.add_child(close)
	box.add_child(head)
	var svp := SubViewport.new()
	svp.size = Vector2i(SIGHT_W, 330)
	svp.world_3d = vp.find_world_3d()
	svp.transparent_bg = false
	svp.msaa_3d = Viewport.MSAA_4X
	svp.render_target_update_mode = SubViewport.UPDATE_ALWAYS
	_sight_cam = Camera3D.new()
	_sight_cam.projection = Camera3D.PROJECTION_ORTHOGONAL
	_sight_cam.size = 40.0
	_sight_cam.near = 0.1
	_sight_cam.far = 20000.0
	svp.add_child(_sight_cam)
	_sight_cam.current = true
	_sight_panel.add_child(svp)
	var tr := TextureRect.new()
	tr.texture = svp.get_texture()
	tr.custom_minimum_size = Vector2(SIGHT_W, 330)
	tr.expand_mode = TextureRect.EXPAND_IGNORE_SIZE
	box.add_child(tr)
	_sight_phase = Label.new()
	_sight_phase.autowrap_mode = TextServer.AUTOWRAP_WORD
	_sight_phase.custom_minimum_size = Vector2(SIGHT_W, 0)
	_sight_phase.add_theme_font_size_override("font_size", 20)
	box.add_child(_sight_phase)
	_sight_tip = Label.new()
	_sight_tip.autowrap_mode = TextServer.AUTOWRAP_WORD
	_sight_tip.custom_minimum_size = Vector2(SIGHT_W, 0)
	_sight_tip.add_theme_color_override("font_color", Color(0.85, 0.88, 0.92))
	box.add_child(_sight_tip)
	_sight_nums = Label.new()
	_sight_nums.add_theme_font_size_override("font_size", 16)
	box.add_child(_sight_nums)
	var grid := GridContainer.new()
	grid.columns = 3
	grid.add_theme_constant_override("h_separation", 10)
	_sight_bars.clear()
	for row in [["zakal", "Zákal", Color(0.80, 0.62, 0.32)], ["kaly", "Jemné kaly", Color(0.52, 0.34, 0.16)], ["zrna", "Zrna a pluchy", Color(0.92, 0.80, 0.52)], ["mlato", "Mláto", Color(0.58, 0.40, 0.22)], ["pack", "Utažení lože", Color(0.55, 0.66, 0.82)]]:
		var nm := Label.new()
		nm.text = str(row[1])
		nm.custom_minimum_size = Vector2(130, 0)
		grid.add_child(nm)
		var pb := ProgressBar.new()
		pb.max_value = 100.0
		pb.show_percentage = false
		pb.custom_minimum_size = Vector2(380, 14)
		pb.size_flags_vertical = Control.SIZE_SHRINK_CENTER
		var fill := StyleBoxFlat.new()
		fill.bg_color = row[2]
		fill.set_corner_radius_all(3)
		pb.add_theme_stylebox_override("fill", fill)
		var bgs := StyleBoxFlat.new()
		bgs.bg_color = Color(0.18, 0.19, 0.22)
		bgs.set_corner_radius_all(3)
		pb.add_theme_stylebox_override("background", bgs)
		grid.add_child(pb)
		var val := Label.new()
		val.custom_minimum_size = Vector2(56, 0)
		val.horizontal_alignment = HORIZONTAL_ALIGNMENT_RIGHT
		grid.add_child(val)
		_sight_bars[row[0]] = [nm, pb, val]
	box.add_child(grid)
	$UI.add_child(_sight_panel)


## Okno průhledítka stojí mezi skříní rozvaděče (vlevo, x 10–610) a panely vpravo (ovládací panel, checklist, vlastnosti).
## Když je mezi nimi málo místa, zmenší se (nejvýš na 60 %); když se nevejde ani tak, zůstane vlevo nahoře a skříň mu uhne
## doprava sama (rozvadec_puvodni._dock).
func _place_sight_panel() -> void:
	var left := 12.0
	if puvodni != null and puvodni.visible:
		left = 10.0 + puvodni.size.x + 10.0  # domovská poloha skříně, ne ta, kam právě uhnula
	var right: float = get_viewport().get_visible_rect().size.x - 12.0
	for p in [ovladani, checklist_panel, props]:
		if p != null and is_instance_valid(p) and (p as Control).visible:
			var x: float = (p as Control).get_global_rect().position.x
			if x > left:
				right = minf(right, x - 10.0)
	var need: float = maxf(_sight_panel.get_combined_minimum_size().x, 1.0)
	var s: float = clampf((right - left) / need, 0.0, 1.0)
	if s < 0.6:
		left = 12.0
		s = 1.0
	if _sight_panel.position.x != left or _sight_panel.scale.x != s:
		_sight_panel.position = Vector2(left, 96.0)
		_sight_panel.scale = Vector2(s, s)


func _update_sight() -> void:
	if _sight_id == "" or _sight_panel == null or not _sight_panel.visible:
		return
	_place_sight_panel()
	var v: Dictionary = sim_panel.inl_views.get(_sight_id, {})
	if v.is_empty():
		v = sim_panel.sight_view(str(plant.call("inl_pipe", _sight_id)))
	_sight_title.text = "PRŮHLEDÍTKO" + (("  ·  " + str(v["cesta"])) if str(v.get("cesta", "")) != "" else "")
	_sight_phase.text = str(v.get("faze", ""))
	_sight_phase.add_theme_color_override("font_color", SIGHT_TONES.get(str(v.get("ton", "info")), Color.WHITE))
	_sight_tip.text = str(v.get("rada", ""))
	_sight_tip.visible = _sight_tip.text != ""
	var nums: Array = []
	nums.append(("průtok %.0f L/min" % float(v["lpm"])) if float(v.get("lpm", 0.0)) > 0.5 else "neteče")
	if v.has("plato"):
		nums.append("%.1f °P" % float(v["plato"]))
	if v.has("ebc"):
		nums.append("barva %.0f EBC" % float(v["ebc"]))
	if bool(v.get("sk", false)):
		nums.append("podtlak pod sítem %.0f mbar" % float(v.get("dp", 20.0)))
	_sight_nums.text = "   ·   ".join(nums)
	var liquid: bool = str(v.get("medium", "empty")) != "empty"
	for k in _sight_bars:
		var row: Array = _sight_bars[k]
		var show: bool = liquid and (k != "pack" or bool(v.get("sk", false)))
		for c in row:
			(c as Control).visible = show
		var x: float = clampf(float(v.get(k, 0.0)), 0.0, 1.0) * 100.0
		(row[1] as ProgressBar).value = x
		(row[2] as Label).text = "%.0f %%" % x


func _update_cabinet_button() -> void:
	if _cabinet_btn == null or puvodni == null:
		return
	_cabinet_btn.text = "Skrýt rozvaděč" if puvodni.visible else "Zobrazit rozvaděč"
	_cabinet_btn.tooltip_text = "Skrýt levý rozvaděč; jeho zobrazení uvolní místo pro zvětšený FM."


func _set_cabinet_visible(show: bool) -> void:
	if puvodni == null:
		return
	puvodni.visible = show
	_update_cabinet_button()


func _build_cabinet_hide_button() -> void:
	_cabinet_hide_btn = Button.new()
	_cabinet_hide_btn.text = "Skrýt ×"
	_cabinet_hide_btn.tooltip_text = "Skrýt levý rozvaděč. Zobrazíš ho tlačítkem v horní liště nebo klávesou H."
	_cabinet_hide_btn.focus_mode = Control.FOCUS_NONE
	_cabinet_hide_btn.position = Vector2(332.0, 7.0)
	_cabinet_hide_btn.custom_minimum_size = Vector2(88.0, 28.0)
	_cabinet_hide_btn.z_index = 10
	_cabinet_hide_btn.pressed.connect(func() -> void: _set_cabinet_visible(false))
	puvodni.add_child(_cabinet_hide_btn)


func _build_view_bar() -> void:
	var bg := ColorRect.new()
	bg.color = Color(0.08, 0.09, 0.11, 0.92)
	bg.anchor_right = 1.0
	bg.offset_top = 44.0
	bg.offset_bottom = 84.0
	$UI.add_child(bg)
	var bar := HBoxContainer.new()
	bar.anchor_right = 1.0
	bar.offset_left = 8.0
	bar.offset_top = 48.0
	bar.offset_bottom = 82.0
	bar.add_theme_constant_override("separation", 6)
	$UI.add_child(bar)
	var play := Button.new()
	play.focus_mode = Control.FOCUS_NONE
	play.text = "⏸"
	play.pressed.connect(func() -> void:
		sim_panel.toggle_pause()
		play.text = "▶" if sim_panel.paused else "⏸")
	bar.add_child(play)
	_play_btn = play
	for s in sim_panel.SPEEDS:
		var b := Button.new()
		b.focus_mode = Control.FOCUS_NONE
		b.text = "%d×" % int(s)
		b.pressed.connect(func() -> void:
			sim_panel.set_speed(s)
			play.text = "⏸"
			status.text = "Rychlost času ×%d" % int(s))
		bar.add_child(b)
	var clb := Button.new()
	clb.focus_mode = Control.FOCUS_NONE
	clb.text = "Checklist"
	clb.pressed.connect(func() -> void: checklist_panel.visible = not checklist_panel.visible)
	bar.add_child(clb)
	var rtb := Button.new()
	rtb.focus_mode = Control.FOCUS_NONE
	_cabinet_btn = rtb
	_update_cabinet_button()
	rtb.pressed.connect(func() -> void: _set_cabinet_visible(not puvodni.visible))
	bar.add_child(rtb)
	var rdb := Button.new()
	rdb.focus_mode = Control.FOCUS_NONE
	rdb.text = "Panel"
	rdb.tooltip_text = "Ovládací panel jako v původní aplikaci: čas, automat, rozvaděč, pánev, SK, čerpadla, cesty, recept, kvašení, CIP, poruchy"
	rdb.pressed.connect(func() -> void: ovladani.visible = not ovladani.visible)
	bar.add_child(rdb)
	var prb := Button.new()
	prb.focus_mode = Control.FOCUS_NONE
	prb.text = "Protokol"
	prb.tooltip_text = "Varní protokol: spilka, prodlevy s přesností ± 0,5 °C, CIP, chyby, hodnocení a zásoba sladu"
	prb.pressed.connect(func() -> void: protokol.visible = not protokol.visible)
	bar.add_child(prb)
	var grb := Button.new()
	grb.focus_mode = Control.FOCUS_NONE
	grb.text = "Grafy"
	grb.tooltip_text = "Živé grafy: teploty, °P, objem, hořkost, DMS, pH a barva mladiny v čase (export do CSV)"
	grb.pressed.connect(func() -> void: grafy.visible = not grafy.visible)
	bar.add_child(grb)
	var slb := Button.new()
	slb.focus_mode = Control.FOCUS_NONE
	slb.text = "Sládek"
	slb.tooltip_text = "Digitální sládek – zeptej se pivovarského učitele"
	slb.pressed.connect(func() -> void: sladek.visible = not sladek.visible)
	bar.add_child(slb)
	var reb := Button.new()
	reb.focus_mode = Control.FOCUS_NONE
	reb.text = "Editor cest"
	reb.pressed.connect(func() -> void:
		if route_editor.visible:
			route_editor.close()
		else:
			route_editor.open()
			status.text = "Editor cest: vyber cestu, nahrávej kliky na klapky a čerpadla, klik na trubku = úsek cesty")
	bar.add_child(reb)
	var rst := Button.new()
	rst.focus_mode = Control.FOCUS_NONE
	rst.text = "Nová várka"
	rst.pressed.connect(func() -> void:
		sim_panel.new_batch()
		status.text = "Nová várka od nuly")
	bar.add_child(rst)
	var sp := VSeparator.new()
	bar.add_child(sp)
	for spec in [["Celé", Vector2(4000, 820), 1700.0, 8800.0], ["Varna", Vector2(140, 585), 900.0, 0.0], ["Scezovačka", Vector2(1230, 605), 900.0, 0.0], ["Whirlpool", Vector2(2260, 865), 900.0, 0.0], ["Bojler", Vector2(4260, 800), 1000.0, 0.0], ["Ledovka", Vector2(7658, 1035), 800.0, 0.0], ["Spilka", Vector2(5318, 880), 800.0, 0.0]]:
		var cb := Button.new()
		cb.focus_mode = Control.FOCUS_NONE
		cb.text = spec[0]
		var c: Vector2 = spec[1]
		var h: float = spec[2]
		var w: float = spec[3]
		cb.pressed.connect(func() -> void: _focus(c, h, w))
		bar.add_child(cb)


func _notification(what: int) -> void:
	if what == NOTIFICATION_WM_CLOSE_REQUEST and sim_panel != null:
		if _plan_dirty:
			_on_save()
		sim_panel.save_state()


func _vp_mouse() -> Vector2:
	var local: Vector2 = vpc.get_local_mouse_position()
	var cs: Vector2 = vpc.size
	if cs.x < 1.0 or cs.y < 1.0:
		return local
	return Vector2(local.x * float(vp.size.x) / cs.x, local.y * float(vp.size.y) / cs.y)


func _pick_tol() -> float:
	return clampf(cam.size * 0.018, 16.0, 48.0)


func _finish_draw() -> void:
	var snap := _snap()
	var id: String = str(plant.call("finish_draw_pipe", "mash", 50.0))
	_refresh_buttons()
	if id == "":
		status.text = "Trubka potřebuje aspoň 2 body (Enter až po 2. kliku)"
	else:
		_push_undo(snap)
		status.text = "Nová trubka: %s — LMB přesun / Del smazat / Ctrl+S" % id


func _input(event: InputEvent) -> void:
	var focus := get_viewport().gui_get_focus_owner()
	if focus is LineEdit or focus is SpinBox or focus is TextEdit:  # psaní do polí (i popis kroku v checklistu) nesmí spouštět zkratky
		if event is InputEventKey:
			if (event as InputEventKey).keycode == KEY_ESCAPE:
				focus.release_focus()
			return
		if event is InputEventMouseButton and (event as InputEventMouseButton).pressed and not (focus as Control).get_global_rect().has_point((event as InputEventMouseButton).position):
			focus.release_focus()  # klik mimo pole ukončí psaní a jde dál (do scény nebo panelu)
		elif not (event is InputEventMouseButton or event is InputEventMouseMotion):
			return
	if event.is_action_pressed("toggle_editor"):
		btn_editor.button_pressed = not btn_editor.button_pressed
		get_viewport().set_input_as_handled()
		return
	if event.is_action_pressed("toggle_see_through"):
		btn_see.button_pressed = not btn_see.button_pressed
		get_viewport().set_input_as_handled()
		return
	if event.is_action_pressed("save_plan"):
		_on_save()
		get_viewport().set_input_as_handled()
		return
	if event.is_action_pressed("new_pipe") and bool(plant.get("editor_mode")):
		_on_new_pipe()
		get_viewport().set_input_as_handled()
		return

	if event is InputEventKey and event.pressed and not event.echo:
		var k: InputEventKey = event
		if k.ctrl_pressed and bool(plant.get("editor_mode")):
			if k.keycode == KEY_Z and not k.shift_pressed:
				_do_undo()
				get_viewport().set_input_as_handled()
				return
			if k.keycode == KEY_Y or (k.keycode == KEY_Z and k.shift_pressed):
				_do_redo()
				get_viewport().set_input_as_handled()
				return
			if k.keycode == KEY_D:
				_on_duplicate()
				get_viewport().set_input_as_handled()
				return
		if k.keycode == KEY_V and bool(plant.get("editor_mode")) and not k.ctrl_pressed:
			_on_place_valve()
			get_viewport().set_input_as_handled()
			return
		# Enter dokončí kreslení trubky
		if (k.keycode == KEY_ENTER or k.keycode == KEY_KP_ENTER) and bool(plant.get("draw_mode")):
			_finish_draw()
			get_viewport().set_input_as_handled()
			return
		if k.keycode == KEY_ESCAPE and bool(plant.get("editor_mode")):
			if _place_valve:
				_place_valve = false
				status.text = "Vkládání klapky zrušeno"
			elif bool(plant.get("draw_mode")):
				plant.call("cancel_draw_pipe")
				_refresh_buttons()
				status.text = "Kreslení trubky zrušeno"
			else:
				plant.call("set_selected", "")
				_dragging = false
				plant.call("end_drag")
			get_viewport().set_input_as_handled()
			return
		# Del = celá položka (celá trubka), ne jen bod
		if (k.keycode == KEY_DELETE or k.keycode == KEY_BACKSPACE) and bool(plant.get("editor_mode")):
			if bool(plant.get("draw_mode")):
				plant.call("cancel_draw_pipe")
				_refresh_buttons()
				status.text = "Kreslení zrušeno"
			elif str(plant.get("selected_id")) != "":
				_delete_selected()
			get_viewport().set_input_as_handled()
			return
		if k.keycode == KEY_TAB:
			var show: bool = not (puvodni.visible or ovladani.visible)  # skříň vlevo a panel vpravo jako v původní aplikaci
			_set_cabinet_visible(show)
			ovladani.visible = show
			get_viewport().set_input_as_handled()
			return
		if k.keycode == KEY_H:
			_set_cabinet_visible(not puvodni.visible)
			get_viewport().set_input_as_handled()
			return
		if k.keycode == KEY_SPACE:
			sim_panel.toggle_pause()
			_play_btn.text = "▶" if sim_panel.paused else "⏸"
			get_viewport().set_input_as_handled()
			return
		if k.keycode >= KEY_1 and k.keycode < KEY_1 + sim_panel.SPEEDS.size() and not k.ctrl_pressed:
			sim_panel.set_speed(sim_panel.SPEEDS[k.keycode - KEY_1])
			_play_btn.text = "⏸"
			status.text = "Rychlost času ×%d" % int(sim_panel.time_scale)
			get_viewport().set_input_as_handled()
			return
		if k.keycode == KEY_F or k.keycode == KEY_HOME:
			_fit_camera()
			get_viewport().set_input_as_handled()
			return
		if k.keycode == KEY_N and bool(plant.get("editor_mode")) and not k.ctrl_pressed:
			_on_new_pipe()
			get_viewport().set_input_as_handled()
			return

	if _over_chrome():
		return
	if event is InputEventMouse and _over_panel((event as InputEventMouse).position):
		if event is InputEventMouseButton and not (event as InputEventMouseButton).pressed:
			_panning = false  # puštění myši nad panelem ukončí i posun pohledu začatý ve scéně
		return

	if event is InputEventMouseButton:
		var mb: InputEventMouseButton = event
		var screen: Vector2 = _vp_mouse()
		if mb.button_index in [MOUSE_BUTTON_WHEEL_UP, MOUSE_BUTTON_WHEEL_DOWN] and mb.pressed and mb.ctrl_pressed and not bool(plant.get("editor_mode")):
			_valve_step(screen, 10 if mb.button_index == MOUSE_BUTTON_WHEEL_UP else -10)
			get_viewport().set_input_as_handled()
		elif mb.button_index == MOUSE_BUTTON_WHEEL_UP and mb.pressed:
			_zoom(0.85, screen)
			get_viewport().set_input_as_handled()
		elif mb.button_index == MOUSE_BUTTON_WHEEL_DOWN and mb.pressed:
			_zoom(1.0 / 0.85, screen)
			get_viewport().set_input_as_handled()
		elif mb.button_index == MOUSE_BUTTON_MIDDLE:
			_panning = mb.pressed
			_pan_last = mb.position
			get_viewport().set_input_as_handled()
		elif mb.button_index == MOUSE_BUTTON_LEFT and not bool(plant.get("editor_mode")):
			# běžný režim jako v původní aplikaci: tažení = posun, klik = klapka / čerpadlo / nádoba
			if mb.pressed:
				# volný konec spílací hadice se dá chytit a přetáhnout na druhý vývod (tank / vratka), jako hadice na katetě v původní aplikaci
				var grab: String = str(plant.call("hose_grab_at", plant.call("screen_to_plan", cam, screen), clampf(cam.size * 0.035, 30.0, 150.0)))
				if grab != "":
					_hose_drag = grab
					_hose_moved = false
					_panning = false
					get_viewport().set_input_as_handled()
					return
				_panning = true
				_view_moved = false
				_view_press = mb.position
				_pan_last = mb.position
			elif _hose_drag != "":
				var drag_id := _hose_drag
				_hose_drag = ""
				var drop_pos: Vector2 = plant.call("screen_to_plan", cam, screen)
				if _hose_moved:
					status.text = str(plant.call("hose_drop", drag_id, drop_pos, clampf(cam.size * 0.2, 120.0, 600.0)))
				else:
					plant.call("hose_drop", drag_id, drop_pos, 0.0)  # jen klik: hadice se vrátí a klik přepne ovladač jako dřív
					_click_item(screen)
			else:
				_panning = false
				if not _view_moved:
					_click_item(screen)
			get_viewport().set_input_as_handled()
		elif mb.button_index == MOUSE_BUTTON_LEFT and bool(plant.get("editor_mode")) and mb.pressed:
			if bool(plant.get("draw_mode")):
				var world: Vector2 = plant.call("screen_to_plan", cam, screen)
				plant.call("add_draw_point", world)
				var n: int = int(plant.call("draw_point_count"))
				status.text = "Nová trubka: bodů %d — další LMB, Enter hotovo, Esc zrušit" % n
				get_viewport().set_input_as_handled()
				return
			if _place_valve:
				var snap_v := _snap()
				var vid: String = str(plant.call("add_valve_at", plant.call("screen_to_plan", cam, screen)))
				_push_undo(snap_v)
				_place_valve = false
				status.text = "Klapka %s vložena (RMB otevřít/zavřít, táhni pro přesun)" % vid
				get_viewport().set_input_as_handled()
				return
			var sel_id: String = str(plant.get("selected_id"))
			if mb.shift_pressed and sel_id != "" and str(plan.get_item(sel_id).get("type")) == "pipe":
				var snap_p := _snap()
				if bool(plant.call("add_pipe_point", sel_id, plant.call("screen_to_plan", cam, screen))):
					_push_undo(snap_p)
					status.text = "Bod přidán do trubky %s" % sel_id
				get_viewport().set_input_as_handled()
				return
			_pre_snap = _snap()
			_drag_moved = false
			plant.call("begin_drag", cam, screen, _pick_tol())
			_dragging = str(plant.get("selected_id")) != ""
			status.text = "Vybráno: %s (Del = smazat celek)" % str(plant.get("selected_id"))
			get_viewport().set_input_as_handled()
		elif mb.button_index == MOUSE_BUTTON_LEFT and bool(plant.get("editor_mode")) and not mb.pressed:
			if _dragging and _drag_moved:
				_push_undo(_pre_snap)
			_drag_moved = false
			plant.call("end_drag")
			_dragging = false
			get_viewport().set_input_as_handled()
		elif mb.button_index == MOUSE_BUTTON_RIGHT and mb.pressed and bool(plant.get("editor_mode")):
			if bool(plant.get("draw_mode")):
				plant.call("cancel_draw_pipe")
				_refresh_buttons()
				status.text = "Kreslení zrušeno"
			else:
				var snap_t := _snap()
				plant.call("begin_drag", cam, screen, _pick_tol())
				plant.call("end_drag")
				plant.call("toggle_selected_valve")
				if _snap() != snap_t:
					_push_undo(snap_t)
				var it: Dictionary = plan.get_item(str(plant.get("selected_id")))
				if str(it.get("type")) == "valve":
					status.text = "Klapka %s open=%s" % [it.get("id"), str(it.get("open"))]
			get_viewport().set_input_as_handled()
	elif event is InputEventMouseMotion:
		var mm: InputEventMouseMotion = event
		if _hose_drag != "":
			_hose_moved = true
			plant.call("hose_preview", _hose_drag, plant.call("screen_to_plan", cam, _vp_mouse()))
			get_viewport().set_input_as_handled()
		elif _panning:
			if mm.position.distance_to(_view_press) > 5.0:
				_view_moved = true
			var delta: Vector2 = mm.position - _pan_last
			var scale: float = cam.size / maxf(vpc.size.y, 1.0)
			cam.position += Vector3(-delta.x * scale, delta.y * scale, 0)
			_pan_last = mm.position
			get_viewport().set_input_as_handled()
		elif _dragging and bool(plant.get("editor_mode")) and Input.is_mouse_button_pressed(MOUSE_BUTTON_LEFT):
			_drag_moved = true
			plant.call("drag_to", cam, _vp_mouse())
			get_viewport().set_input_as_handled()


func _delete_selected() -> void:
	var id: String = str(plant.get("selected_id"))
	var it: Dictionary = plan.get_item(id)
	if it.is_empty():
		return
	var typ: String = str(it.get("type"))
	if typ == "zone":
		status.text = "Zone nelze smazat"
		return
	# vždy celá položka (u pipe = celá trasa + všechny pts)
	var snap := _snap()
	var ok: bool = bool(plant.call("delete_item", id))
	if ok:
		_push_undo(snap)
	_dragging = false
	if ok:
		status.text = "Smazáno celek: %s (%s)" % [id, typ]
	else:
		status.text = "Smazání selhalo: " + id


## snímek průhledítka na scezovací lince ve fázi prvni / kalna / cira / trha / sucho / rmut / voda (simulace pak stojí),
## za dvojtečkou volitelně výška záběru hlavního pohledu (např. „cira:160“ = zblízka na sklo ve scéně)
func _sight_capture(arg: String) -> void:
	var parts := arg.split(":")
	var phase: String = parts[0]
	var zoom: float = float(parts[1]) if parts.size() > 1 else 420.0
	var sp: SimPanel = sim_panel
	var s: BrewSim = sp.sim
	var sk: Dictionary = s.vessels["sk"]
	sk["malt"] = 700.0
	sk["v"] = 3400.0
	sk["ext"] = 600.0
	sk["t"] = 76.0
	sk["bed"] = phase != "rmut"
	s.vessels["hlt"]["v"] = 4000.0
	s.vessels["hlt"]["t"] = 78.0
	s.vessels["rvp"]["v"] = 300.0
	sp.close_all()
	if phase == "voda":
		sp._flush_pipes()
	else:
		sp._set_route("vorlauf" if phase in ["prvni", "kalna"] else "dilo")
	for i in 160:
		sp._update_flows()
		s.step(0.5)
		sp._update_pipes(0.5)
	var st := {
		"prvni": {"turb": 1.0, "fines": 0.55, "cloud": 0.45, "pack": 0.1, "torn": 0.0, "outp": 17.0},
		"kalna": {"turb": 0.6, "fines": 0.2, "cloud": 0.05, "pack": 0.2, "torn": 0.0, "outp": 15.0},
		"cira": {"turb": 0.0, "fines": 0.0, "cloud": 0.0, "pack": 0.3, "torn": 0.0, "outp": 11.0},
		"trha": {"turb": 0.3, "fines": 0.1, "cloud": 0.0, "pack": 0.85, "torn": 0.6, "dp": 230.0, "outp": 6.0},
		"sucho": {"turb": 0.1, "fines": 0.05, "cloud": 0.0, "pack": 0.5, "torn": 0.0, "outp": 3.0},
	}
	for k in st.get(phase, {}):
		sk[k] = st[phase][k]
	if phase == "sucho":
		sk["v"] = 700.0 * BrewSim.BED_L_PER_KG * 0.45 * 0.8  # hladina pod povrchem mláta
	sp.paused = true
	_open_sight("inl_mur9u2hg")
	_focus(Vector2(790, 810), zoom)
	_capture_wait = 40


func _parse_capture_args() -> void:
	for a in OS.get_cmdline_user_args():
		if a.begins_with("--capture="):
			_capture_path = a.substr("--capture=".length())
		elif a.begins_with("--mode="):
			_capture_mode = a.substr("--mode=".length())
	if _capture_path == "":
		return
	if _capture_mode.begins_with("focus:"):
		var fp := _capture_mode.substr(6).split(",")
		_focus(Vector2(float(fp[0]), float(fp[1])), float(fp[2]))
		_refresh_buttons()
		return
	if _capture_mode.begins_with("sight:"):
		_sight_capture(_capture_mode.substr(6))
		_refresh_buttons()
		return
	if _capture_mode == "valvepct":  # klapky u průhledítka: zavřená, 30 %, 70 %, otevřená (Ctrl + kolečko)
		_focus(Vector2(850, 800), 260.0)
		var vpcts := {"valve_mur9syhl": 0, "valve_mur9svxz": 0, "valve_muracwuj": 70, "valve_muraczea": 100}
		for vid in vpcts:
			plant.call("set_valve_pct", vid, vpcts[vid])
		var vnode: Node3D = (plant.get("_nodes") as Dictionary)["valve_mur9svxz"]
		for i in 3:  # stejně jako Ctrl + kolečko nahoru nad klapkou: výběr pod kurzorem a +10 %
			_valve_step(cam.unproject_position(vnode.global_position), 10)
			vnode = (plant.get("_nodes") as Dictionary)["valve_mur9svxz"]
		print("VALVEPCT ", status.text)
		sim_panel.paused = true
		_refresh_buttons()
		return
	match _capture_mode:
		"edittest":
			btn_editor.button_pressed = true
			var n0: int = plan.items.size()
			plant.call("set_selected", "pipe_mur8tme6")
			_on_dn(80.0)
			plant.call("add_valve_at", Vector2(600, 600))
			plant.call("add_pipe_point", "pipe_mur8tme6", Vector2(2270, 900))
			_on_duplicate()
			print("EDITTEST items ", n0, " -> ", plan.items.size(), " undo=", _undo.size())
			_do_undo()
			print("EDITTEST after undo ", plan.items.size(), " dn=", plan.get_item("pipe_mur8tme6").get("dn"))
		"route_red":
			plant.call("show_route", "vyslazovani")
		"route_green":
			plant.call("show_route", "podraz")
		"simtest":
			var sp: SimPanel = sim_panel
			var s: BrewSim = sp.sim
			var run := func(minutes: float) -> void:
				for i in int(minutes * 30.0):
					sp._update_flows()
					s.step(2.0)
			sp._set_route("voda_panev")
			run.call(2.0)
			print("SIM voda: hlt=%.0f rvp=%.0f teče=%s" % [s.vessels["hlt"]["v"], s.vessels["rvp"]["v"], sp._flowing["voda_panev"]])
			# ruční ovládání jako myší: zavřu jednu klapku trasy -> tok se zastaví
			var vids: Array = sp.route_valve_ids("voda_panev")
			plant.call("click_item", str(vids[0]))
			sp._update_flows()
			print("SIM po zavření klapky %s teče=%s" % [vids[0], sp._flowing["voda_panev"]])
			plant.call("click_item", str(vids[0]))
			sp._update_flows()
			print("SIM po otevření teče=%s" % sp._flowing["voda_panev"])
			plant.call("click_item", "pump_murbmzxy")
			sp._update_flows()
			print("SIM po vypnutí čerpadla teče=%s" % sp._flowing["voda_panev"])
			sp.close_all()
			s.add_malt(25.0)
			s.heater_kw["rvp"] = 40.0
			run.call(10.0)
			print("SIM rvp: %.1f L %.1f C" % [s.vessels["rvp"]["v"], s.vessels["rvp"]["t"]])
			s.heater_kw["rvp"] = 0.0
			sp._set_route("rmut_sk")
			run.call(4.0)
			sp._set_route("dilo")
			run.call(5.0)
			print("SIM dilo: sk %.0f L rvp %.0f L darcy %.0f" % [s.vessels["sk"]["v"], s.vessels["rvp"]["v"], s.darcy_lpm()])
			sp._set_route("varna_virivka")
			run.call(5.0)
			sp._set_route("virivka_chladic")
			run.call(6.0)
			print("SIM spilka: %.0f L %.1f C, wp %.0f L" % [s.vessels["spilka"]["v"], s.vessels["spilka"]["t"], s.vessels["wp"]["v"]])
			sp._update_visuals()
		"boil", "liquids", "valves", "water", "rake", "spray", "spraysk", "panel", "dip", "dipmid", "pipes", "lvview", "hltview", "inflow", "inflowmash":
			var sp3: SimPanel = sim_panel
			var s3: BrewSim = sp3.sim
			var vr: Dictionary = s3.vessels
			vr["hlt"]["v"] = 3700.0
			vr["hlt"]["t"] = 78.0
			vr["lv"]["v"] = 6700.0
			vr["lv"]["t"] = 4.0
			vr["sk"]["v"] = 1800.0
			vr["sk"]["malt"] = 400.0
			vr["sk"]["t"] = 75.0
			vr["sk"]["ext"] = 200.0
			vr["wp"]["v"] = 2500.0
			vr["wp"]["t"] = 92.0
			vr["wp"]["ext"] = 300.0
			if _capture_mode == "boil":
				vr["rvp"]["v"] = 3400.0
				vr["rvp"]["ext"] = 400.0
				vr["rvp"]["t"] = 99.0
				s3.heater_kw["rvp"] = 600.0
				s3.add_hops(800.0, 5.0)
				for i in 300:
					s3.step(2.0)
				vr["rvp"]["foam"] = maxf(float(vr["rvp"]["foam"]), 0.6)  # snímek v době bílkovinného zlomu
			elif _capture_mode == "water" or _capture_mode == "dip" or _capture_mode == "dipmid":
				vr["rvp"]["v"] = 3700.0
				vr["rvp"]["t"] = 20.0
			else:
				vr["rvp"]["v"] = 2700.0
				vr["rvp"]["malt"] = 400.0
				vr["rvp"]["t"] = 65.0
			sp3._update_visuals()
			if _capture_mode == "lvview":
				_focus(Vector2(7500, 1030), 800.0)
			elif _capture_mode == "hltview":
				_focus(Vector2(4260, 800), 900.0)
			elif _capture_mode == "pipes":
				sp3._set_route("voda_panev")
				sp3._update_flows()
				for i in 40:
					s3.step(2.0)
					sp3._update_pipes(2.0)
				sp3._update_pipes(2.0)
				_focus(Vector2(3560, 1050), 300.0)
			elif _capture_mode == "inflow" or _capture_mode == "inflowmash":
				# přítok do varny: otevřená cesta z bojleru (voda) nebo ze scezovací kádě (rmut se šrotem)
				sp3._set_route("rmut_varna" if _capture_mode == "inflowmash" else "voda_panev")
				sp3._update_flows()
				for i in 30:
					s3.step(2.0)
					sp3._update_pipes(2.0)
				sp3._update_visuals()
				_focus(Vector2(150, 640), 520.0)
			elif _capture_mode == "dip" or _capture_mode == "dipmid":
				plant.call("click_item", "dipk_mutkw07f")
				_capture_wait = 4600 if _capture_mode == "dip" else 3400
				_focus(Vector2(110, 560), 520.0)
			elif _capture_mode == "boil" or _capture_mode == "water":
				_focus(Vector2(140, 585), 560.0)
			elif _capture_mode == "rake":
				_focus(Vector2(1230, 560), 440.0)
			elif _capture_mode == "spray":
				sp3.force_sprays = true
				_focus(Vector2(150, 480), 300.0)
			elif _capture_mode == "spraysk":
				sp3.force_sprays = true
				_focus(Vector2(1230, 560), 420.0)  # scezovací káď: sprchy musí být uvnitř
			elif _capture_mode == "panel":
				s3.auto_mode["rvp"] = true
				s3.setpoint["rvp"] = 78.0
				s3.setpoint["hlt"] = 80.0
				sp3._update_readout()
				_focus(Vector2(-860, 840), 1800.0)
			elif _capture_mode == "valves":
				_focus(Vector2(1300, 800), 260.0)
		"hose", "hosetank":
			# spílací hadice na vratce (výchozí) / přepojená na tank
			if _capture_mode == "hosetank":
				plant.call("click_item", "valve_spilaci_hadice")
			_focus(Vector2(5010, 990), 420.0)
		"skdrain", "skrake":
			# scezovací káď: výpusť do kanálu (teče kalná voda) / kopačka dole v loži a otáčí se, kopyto dole
			var sp5: SimPanel = sim_panel
			var vs5: Dictionary = sp5.sim.vessels["sk"]
			if _capture_mode == "skdrain":
				vs5["v"] = 3000.0
				plant.call("click_item", str(sp5._kind_item("valve", "drain").get("id", "")))
				_focus(Vector2(1240, 1110), 380.0)
			else:
				vs5["v"] = 1800.0
				vs5["malt"] = 700.0
				vs5["ext"] = 300.0
				vs5["bed"] = true
				vs5["res"] = 0.0
				plant.call("click_item", "valve_kopacka_zdvih")
				plant.call("click_item", "pump_kopacka")
				plant.call("click_item", "valve_kopyto")
				_focus(Vector2(1330, 590), 520.0)
			_capture_wait = 90
		"pipetest":
			var sp4: SimPanel = sim_panel
			var s4: BrewSim = sp4.sim
			var run4 := func(minutes: float) -> void:
				for i in int(minutes * 30.0):
					sp4._update_flows()
					s4.step(2.0)
					sp4._update_pipes(2.0)
			var rep := func(tag: String) -> void:
				var cnt := {}
				for pid in sp4.pipe_fill:
					var mm: String = str(sp4.pipe_fill[pid]["m"])
					cnt[mm] = int(cnt.get(mm, 0)) + 1
				print("PIPE ", tag, " obsah=", cnt, " clog=%.2f darcy=%.1f" % [s4.clog, s4.darcy_lpm()])
			var v4: Dictionary = s4.vessels
			v4["rvp"]["v"] = 150.0
			v4["rvp"]["malt"] = 25.0
			v4["rvp"]["t"] = 65.0
			v4["rvp"]["ext"] = 10.0
			v4["hlt"]["v"] = 250.0
			v4["hlt"]["t"] = 78.0
			rep.call("start")
			sp4._set_route("rmut_sk")
			run4.call(1.0)
			rep.call("rmut_sk 1 min")
			run4.call(5.0)
			rep.call("rmut_sk 6 min")
			sp4.close_all()
			sp4._update_flows()
			run4.call(1.0)
			rep.call("zavřeno (trubky zůstávají plné)")
			sp4._set_route("dilo")
			run4.call(3.0)
			rep.call("dilo bez proplachu")
			sp4.close_all()
			sp4._flush_pipes()
			run4.call(2.0)
			rep.call("proplach 2 min")
			run4.call(4.0)
			rep.call("proplach 6 min")
			sp4.close_all()
			sp4._set_route("dilo")
			run4.call(3.0)
			rep.call("dilo po proplachu")
		"sedtest":
			var sp5: SimPanel = sim_panel
			var s5: BrewSim = sp5.sim
			var v5: Dictionary = s5.vessels
			var run5 := func(minutes: float) -> void:
				for i in int(minutes * 30.0):
					sp5._update_flows()
					s5.step(2.0)
					sp5._update_pipes(2.0)
			v5["rvp"]["v"] = 150.0
			v5["rvp"]["malt"] = 25.0
			v5["rvp"]["t"] = 65.0
			v5["rvp"]["ext"] = 10.0
			run5.call(10.0)
			print("SED rvp nemíchaný 10 min: usazeno=%.2f" % float(v5["rvp"]["settled"]))
			s5.stir["rvp"] = true
			run5.call(2.0)
			print("SED rvp míchané 2 min: usazeno=%.2f" % float(v5["rvp"]["settled"]))
			s5.stir["rvp"] = false
			run5.call(15.0)
			var m0: float = float(v5["rvp"]["malt"])
			sp5._set_route("rmut_sk")
			run5.call(1.0)
			print("SED přečerpání nemíchaného: malt ve varně %.1f -> %.1f kg, v sk %.1f kg" % [m0, float(v5["rvp"]["malt"]), float(v5["sk"]["malt"])])
			sp5.close_all()
			run5.call(20.0)
			print("SED sk kal po 20 min: fines=%.2f" % float(v5["sk"]["fines"]))
			s5.underlet_kick()
			print("SED po rázu klapky: fines=%.2f cloud=%.2f" % [float(v5["sk"]["fines"]), float(v5["sk"]["cloud"])])
			v5["rvp"]["boil_s"] = 3600.0
			v5["rvp"]["v"] = 200.0
			sp5._set_route("varna_virivka")
			run5.call(3.0)
			print("SED vířivka hned po přítoku: trub=%.2f usazeno=%.2f" % [float(v5["wp"]["trub"]), float(v5["wp"]["tset"])])
			sp5.close_all()
			run5.call(20.0)
			print("SED vířivka po 20 min klidu: usazeno=%.2f" % float(v5["wp"]["tset"]))
		"sedvis":
			var sp6: SimPanel = sim_panel
			var v6: Dictionary = sp6.sim.vessels
			v6["rvp"]["v"] = 160.0
			v6["rvp"]["malt"] = 25.0
			v6["rvp"]["settled"] = 0.9
			v6["sk"]["v"] = 110.0
			v6["sk"]["malt"] = 25.0
			v6["sk"]["fines"] = 0.8
			v6["sk"]["cloud"] = 0.4
			v6["wp"]["v"] = 150.0
			v6["wp"]["trub"] = 0.7
			v6["wp"]["tset"] = 1.0
			v6["spilka"]["v"] = 100.0
			v6["spilka"]["trub"] = 0.5
			sp6._update_visuals()
		"scaletest":
			var sp7: SimPanel = sim_panel
			var s7: BrewSim = sp7.sim
			var v7: Dictionary = s7.vessels
			var run7 := func(minutes: float) -> void:
				for i in int(minutes * 30.0):
					sp7._update_flows()
					s7.step(2.0)
					sp7._update_pipes(2.0)
			sp7._set_route("voda_panev")
			run7.call(15.0)
			print("SCALE napuštění varny 15 min: varna %.0f L (%.1f hl), bojler %.0f L" % [float(v7["rvp"]["v"]), float(v7["rvp"]["v"]) / 100.0, float(v7["hlt"]["v"])])
			sp7.close_all()
			s7.add_malt(400.0)
			s7.setpoint["rvp"] = 65.0
			s7.auto_mode["rvp"] = true
			run7.call(40.0)
			print("SCALE ohřev na 65 °C za 40 min: t=%.1f kW=%.0f" % [float(v7["rvp"]["t"]), float(s7.heater_kw["rvp"])])
			v7["rvp"]["malt"] = 0.0
			v7["rvp"]["ext"] = 700.0
			v7["rvp"]["v"] = 4600.0
			s7.setpoint["rvp"] = 100.0
			run7.call(60.0)
			print("SCALE var po 60 min: t=%.1f objem %.0f L (%.1f hl) odpar %.0f L/h (%.1f %%/h) pěna %.2f" % [float(v7["rvp"]["t"]), float(v7["rvp"]["v"]), float(v7["rvp"]["v"]) / 100.0, float(v7["rvp"]["evap_lph"]), float(v7["rvp"]["evap_lph"]) / float(v7["rvp"]["v"]) * 100.0, float(v7["rvp"]["foam"])])
		"zoomc2":
			_focus(Vector2(900, 1060), 380.0)
		"zoomwp":
			_focus(Vector2(2260, 880), 700.0)
		"zoomsk":
			_focus(Vector2(1230, 470), 520.0)
		"sightdemo":
			_open_sight("inl_mur9u2hg")
			_focus(Vector2(790, 810), 300.0)
		"rt20", "rt20stred", "rt20dole", "lupa":
			# rozvaděč RT.20: všechny sekce rozbalené, varna čerpá do vířivky (C1), bojler na AUTO 80 °C
			var sp9: SimPanel = sim_panel
			var v9: Dictionary = sp9.sim.vessels
			v9["rvp"]["v"] = 3000.0
			v9["rvp"]["t"] = 72.0
			v9["hlt"]["v"] = 4000.0
			v9["hlt"]["t"] = 79.0
			sp9.sim.auto_mode["hlt"] = true
			sp9.sim.setpoint["hlt"] = 80.0
			sp9.sim.setpoint["rvp"] = 72.0
			sp9._set_route("varna_virivka")
			rozvadec.call("expand_all", true)
			if _capture_mode == "lupa":
				skrin.lupa_open({"t": "fc", "pid": str(rozvadec.pump_ids[0])})
			if _capture_mode != "rt20" and _capture_mode != "lupa":
				rozvadec.set("scroll_pin", 1.0 if _capture_mode == "rt20dole" else 0.3)
			_capture_wait = 40
		"puvodni":
			# rozvaděč jako v původní aplikaci, přiblížený kolečkem na horní řady ovladačů
			puvodni.call_deferred("_zoom_at", Vector2(300.0, 300.0), 2.4)
		"ovladani", "ovladani2", "ovladani3":
			# ovládací panel jako v původní aplikaci: nahoře se sekcemi jako po spuštění, pak všechno rozbalené uprostřed a dole
			sim_panel.paused = true
			if _capture_mode != "ovladani":
				ovladani.call("expand_all", true)
				ovladani.set("scroll_pin", 0.35 if _capture_mode == "ovladani2" else 1.0)
			_capture_wait = 40
		"sladek":
			# Digitální sládek: panel otevřený, otázka se stavem várky; snímek chvíli po odpovědi (nejdéle asi 3 min)
			sim_panel.paused = true
			sladek.visible = true
			sladek.connect("answered", func() -> void: _capture_wait = _capture_frame + 30)
			sladek.call("_ask", "Co mám teď v trenažéru udělat a proč? Vycházej z kroku checklistu a ze stavu várky.", "Co mám teď dělat?")
			_capture_wait = 12000
		"chkopen":
			checklist_panel.visible = true
		"chkauto":
			# celá várka podle návrhu checklistu (bez zásahu do uložené várky a checklistu obsluhy)
			checklist_panel.visible = true
			checklist_panel.run_auto_test()
		"chkprobe":
			checklist_panel.probe_routes()
		"chktest":
			# test panelu checklistu: uloží krok 1 se zavřenými klapkami a krok 2 s cestou, obnoví stav, vypíše soubor
			# (nikdy nepřepíše skutečně nahraný checklist obsluhy)
			if FileAccess.file_exists(ChecklistPanel.FILE):
				print("CHKTEST přeskočen: checklist_uceni.json už existuje")
				return
			checklist_panel.visible = true
			sim_panel.close_all()
			checklist_panel.note.text = "TEST: výchozí stav, vše zavřeno"
			checklist_panel._save(true)
			sim_panel._set_route("voda_panev")
			checklist_panel._refresh_routes()
			checklist_panel._select_route("voda_panev")
			checklist_panel.note.text = "TEST: cesta bojler → pánev"
			checklist_panel._save(false)
			sim_panel.close_all()
			checklist_panel._restore()
			var opened := 0
			for it in plan.items:
				if str(it.get("type")) == "valve" and bool(it.get("open", false)):
					opened += 1
			print("CHKTEST uloženo kroků: ", checklist_panel.data.size(), " | krok 2 cesta: ", checklist_panel.data["2"]["cesta"], " | po obnovení otevřených klapek: ", opened, " | uloženo v kroku 2: ", checklist_panel.data["2"]["stav"]["open"].size())
		"routeed":
			# editor cest: nahraje postup cesty bojler → pánev (klapky, pak čerpadlo C4)
			route_editor.open()
			route_editor._refresh_list("voda_panev")
			route_editor._rec.button_pressed = true
			for vid in sim_panel.route_valve_ids("voda_panev"):
				plant.call("click_item", str(vid))
				route_editor.on_item_clicked(str(vid))
			plant.call("click_item", "pump_murbmzxy")
			route_editor.on_item_clicked("pump_murbmzxy")
		"zoomx":
			_focus(Vector2(510, 890), 330.0)
		"hybrid", "hybvar", "hybwp", "hybsk":
			plant.call("set_hybrid", true)
			var sp8: SimPanel = sim_panel
			var v8: Dictionary = sp8.sim.vessels
			v8["hlt"]["v"] = 3700.0
			v8["rvp"]["v"] = 3400.0
			v8["rvp"]["malt"] = 400.0
			v8["rvp"]["t"] = 65.0
			v8["sk"]["v"] = 1800.0
			v8["sk"]["malt"] = 400.0
			v8["wp"]["v"] = 2500.0
			v8["lv"]["v"] = 6700.0
			sp8._set_route("voda_panev")
			sp8._update_flows()
			for i in 40:
				sp8.sim.step(2.0)
				sp8._update_pipes(2.0)
			sp8._update_visuals()
			v8["wp"]["v"] = 2500.0
			if _capture_mode == "hybvar":
				_focus(Vector2(140, 585), 560.0)
			elif _capture_mode == "hybwp":
				_focus(Vector2(2260, 860), 700.0)
			elif _capture_mode == "hybsk":
				_focus(Vector2(1230, 600), 620.0)
		"photo":
			plant.call("set_photo_mode", true)
		"editor":
			btn_editor.button_pressed = true
			plant.set("editor_mode", true)
		"seethrough":
			btn_see.button_pressed = true
			plant.call("set_see_through", true)
		_:
			pass
	_refresh_buttons()


func _process(dt: float) -> void:
	if _plan_dirty:
		_autosave_acc += dt
		if _autosave_acc >= 10.0:
			_on_save()
	if _sight_panel != null and _sight_panel.visible:
		_update_sight()
	# šipky = posun kamery
	var focus := get_viewport().gui_get_focus_owner()
	if not (focus is LineEdit or focus is SpinBox or focus is TextEdit):
		var dir := Vector2.ZERO
		if Input.is_key_pressed(KEY_LEFT):
			dir.x -= 1.0
		if Input.is_key_pressed(KEY_RIGHT):
			dir.x += 1.0
		if Input.is_key_pressed(KEY_UP):
			dir.y += 1.0
		if Input.is_key_pressed(KEY_DOWN):
			dir.y -= 1.0
		if dir != Vector2.ZERO:
			cam.position += Vector3(dir.x, dir.y, 0) * cam.size * 0.8 * dt
	if _capture_path == "":
		return
	_capture_frame += 1
	if _capture_frame == _capture_wait:
		RenderingServer.frame_post_draw.connect(_do_capture, CONNECT_ONE_SHOT)


func _do_capture() -> void:
	var img: Image = get_viewport().get_texture().get_image()
	if img != null:
		img.save_png(_capture_path)
		print("CAPTURE_OK ", _capture_path)
	else:
		print("CAPTURE_FAIL")
	get_tree().quit()
