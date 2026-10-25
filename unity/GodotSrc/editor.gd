extends Node2D
## Editor varny – plnohodnotný editor plánku: přidat, upravit, smazat, vytvořit jakýkoli díl.
## Nástroje: Výběr/posun (V), Trubka (T), Hadice (H), Spílací hadice, Spoj/rozpoj (J), Měřit (M), Cesta (C), Barva (B), Přidat díl.
## Výběr: klik = jeden díl, tažení po prázdnu = rámeček (Shift přidá), Ctrl+A vše na obrazovce.
## Klávesy: Del smazat, R otočit, Ctrl+D duplikovat, Ctrl+Z zpět, Ctrl+S uložit, šipky posun o 10 cm (Shift 1 m), Esc/E konec.
## Uloží se do user://plan_upravy.json (původní plán zůstává v aplikaci; „Obnovit původní“ ho vrátí).

const GRID: = 10.0
const PX_PER_M: = 100.0

## díly do palety: [klíč, popis, typ, vlastnosti]
const PARTS: = [
	["klapka", "⋈ Klapka", "valve", {"kind": "klapka", "label": "klapka"}],
	["kulovy", "◉ Kulový ventil", "valve", {"kind": "kulovy", "label": "kulový ventil"}],
	["skrt", "◐ Škrtidlo", "valve", {"kind": "skrt", "label": "škrtidlo"}],
	["belimo", "▣ Belimo", "valve", {"kind": "belimo", "label": "Belimo"}],
	["pruh", "▭ Průhledítko", "inl", {"k": "pruh", "label": "průhledítko"}],
	["zpk", "▷| Zpětná klapka", "inl", {"k": "zpk", "label": "zpětná klapka"}],
	["ysit", "Y Sítko", "inl", {"k": "ysit", "label": "Y-sítko"}],
	["red", "◁ Redukce", "inl", {"k": "red", "label": "redukce"}],
	["tep", "° Teploměr", "inl", {"k": "tep", "label": "teploměr"}],
	["man", "◔ Manometr", "inl", {"k": "man", "label": "manometr"}],
	["odk", "K Odvaděč kond.", "inl", {"k": "odk", "label": "odvaděč kondenzátu"}],
	["flow", "⇕ Rotametr", "flow", {"label": "rotametr"}],
	["pump", "⊙ Čerpadlo", "pump", {"label": "čerpadlo", "dir": 0, "bind": ""}],
	["fitT", "┳ T-kus", "fitting", {"ports": [180, 0, 90], "arm": 30, "label": "T-kus"}],
	["fitL", "┗ Koleno L", "fitting", {"ports": [270, 0], "arm": 30, "label": "koleno"}],
	["fitRL", "┛ Koleno ⅃", "fitting", {"ports": [270, 180], "arm": 30, "label": "koleno"}],
	["tank", "▯ Nádoba", "vessel", {"label": "nádoba", "shape": "tank", "w": 220, "h": 300}],
	["cone", "▽ Tank kónický", "vessel", {"label": "tank", "shape": "cone", "w": 220, "h": 440}],
	["horiz", "▭ Ležatý tank", "vessel", {"label": "ležatý tank", "shape": "horiz", "w": 420, "h": 200}],
	["platform", "▬ Podesta", "vessel", {"label": "podesta", "shape": "platform", "w": 400, "h": 16}],
	["spray", "✺ Sprcha", "spray", {"label": "sprcha"}],
	["nozzle", "⇣ Tryska", "nozzle", {"label": "tryska", "dir": 270}],
	["hopf", "▤ Chmelový filtr", "hopf", {"label": "chmelový filtr", "rot": 0}],
	["text", "T Popisek", "text", {"label": "popisek", "size": 14}],
]
const MEDIA: = [["mash", "rmut"], ["wort", "sladina"], ["hot", "horká"], ["water", "studená"], ["ice", "ledová"], ["steam", "pára"]]
const COLORS: = [["", "bez"], ["#e0452f", "červená"], ["#f08c1a", "oranžová"], ["#f2c230", "žlutá"], ["#2fbf5b", "zelená"], ["#2e7bd6", "modrá"], ["#9b59d0", "fialová"], ["#e84393", "růžová"]]
const ON_PIPE: = ["valve", "inl", "flow", "pump"]
const PROTECT: = ["vessel", "pump", "panel", "manifold"]
const VBINDS: = [["", "žádná (jen výkres)"], ["rvp", "pánev"], ["mp", "rmutovací pánev"], ["sk", "scezovací káď"], ["wp", "vířivka"], ["hlt", "bojler"], ["lv", "ledovka"], ["spilka", "spilka"], ["vodovod", "vodovod"], ["glyk", "glykol"]]
const PBINDS: = [["", "žádná (jen výkres)"], ["p1", "C1 rmutovací"], ["p2", "C2 sladina"], ["p3", "C3 mladina"], ["phw", "C4 horká voda"], ["plv", "C5 ledovka"]]
const SHAPES: = [["tank", "válcová"], ["cone", "kónická"], ["tall", "vysoká"], ["horiz", "ležatá"], ["lauter", "scezovací"], ["platform", "podesta"], ["plate", "deskový chladič"], ["chiller", "glykolový chladič"], ["inlet", "řad"]]
const VKINDS: = [["klapka", "klapka"], ["kulovy", "kulový ventil"], ["skrt", "škrtidlo"], ["belimo", "Belimo"]]
const IKINDS: = [["pruh", "průhledítko"], ["zpk", "zpětná klapka"], ["ysit", "Y-sítko"], ["red", "redukce"], ["tep", "teploměr"], ["man", "manometr"], ["odk", "odvaděč kondenzátu"], ["redp", "redukční ventil páry"], ["aer", "provzdušňovač"], ["tc", "čidlo"]]

var m
var on: = false
var tool: = "select"          # select | pipe | hose | pitch | joint | measure | route | paint | part
var part_key: = "klapka"
var sel = null                # {"t": "item"|"pt"|"pipe"|"size", "it": item, "i": index}
var multi: Array = []         # víc vybraných dílů (item nebo trubka)
var drag: = false
var drag_mode: = ""           # one | group | box | size
var drag_off: = Vector2.ZERO
var drag_start: = Vector2.ZERO
var drag_orig: = []
var box_a: = Vector2.ZERO
var draw_pts: = []            # kreslená trubka / hadice
var pitch_a = null            # začátek spílací hadice
var meas_a = null
var route_a = null
var route_hl: = []            # zvýrazněná cesta (seznam polyline)
var paint_col: = "#e0452f"
var hover_w: = Vector2.ZERO
var undo_stack: = []
var undo_key: = ""
var undo_t: = 0
var grave: = {}               # smazané díly (kvůli zpět – zachová vazby)
var del_arm: = 0
var dirty: = false
var ui: CanvasLayer
var info: Label
var props: VBoxContainer
var live_s
var live_i
var btns: = {}
var dn_new: = 50
var med_new: = "wort"
var fdlg: FileDialog
var dock_was: = false

func setup() -> void :
	z_index = 60
	ui = CanvasLayer.new();ui.layer = 3;ui.visible = false;m.add_child(ui)
	var p: = PanelContainer.new()
	var sb: = StyleBoxFlat.new();sb.bg_color = Color(0.05, 0.065, 0.08, 0.95);sb.set_corner_radius_all(10);sb.border_color = Color("f2c230");sb.set_border_width_all(2)
	sb.content_margin_left = 10;sb.content_margin_right = 10;sb.content_margin_top = 8;sb.content_margin_bottom = 8
	p.add_theme_stylebox_override("panel", sb);p.position = Vector2(12, 62);p.custom_minimum_size = Vector2(610, 0);ui.add_child(p)
	var th: = Theme.new();th.default_font_size = 13;p.theme = th
	var v: = VBoxContainer.new();v.add_theme_constant_override("separation", 5);p.add_child(v)
	var hb: = HBoxContainer.new();v.add_child(hb)
	var t: = Label.new();t.text = "✎ EDITOR VARNY";t.add_theme_color_override("font_color", Color("f2c230"));t.add_theme_font_override("font", m.font_b);t.add_theme_font_size_override("font_size", 15);t.size_flags_horizontal = Control.SIZE_EXPAND_FILL;hb.add_child(t)
	var more: = VBoxContainer.new();more.add_theme_constant_override("separation", 5)
	var cb: = _b(hb, "▴ sbalit", func(): pass)
	cb.pressed.connect(func():
		more.visible = not more.visible;info.visible = more.visible
		cb.text = "▴ sbalit" if more.visible else "▾ rozbalit")
	var r: = _row(v)
	for d in [["select", "⬚ Výběr (V)"], ["pipe", "╱ Trubka (T)"], ["hose", "∿ Nerez. hadice (H)"], ["pitch", "⤳ Spílací hadice"], ["joint", "● Spoj (J)"], ["measure", "⟷ Měřit (M)"], ["route", "➜ Cesta (C)"], ["paint", "▦ Barva (B)"]]:
		var b: = _b(r, d[1], _set_tool.bind(d[0]));b.toggle_mode = true;btns[d[0]] = b
	v.add_child(more)
	r = _row(more);_lab(r, "Do trubky:")
	for pd in PARTS:
		if pd[2] in ON_PIPE: _part_btn(r, pd)
	r = _row(more);_lab(r, "Díly:")
	for pd in PARTS:
		if not pd[2] in ON_PIPE: _part_btn(r, pd)
	r = _row(more);_lab(r, "Nová trubka:")
	for dn in [10, 15, 25, 40, 50, 75]:
		var bd: = _b(r, "DN%d" % dn, _set_dn.bind(dn));bd.toggle_mode = true;btns["dn%d" % dn] = bd
	_lab(r, "  teče:")
	for md in MEDIA:
		var bm: = _b(r, md[1], _set_med.bind(md[0]));bm.toggle_mode = true;btns["med_" + md[0]] = bm
	r = _row(more);_lab(r, "Barva cesty:")
	for c in COLORS:
		var bc: = _b(r, c[1], _set_paint.bind(c[0]));bc.toggle_mode = true;btns["col_" + c[0]] = bc
		if c[0] != "": bc.add_theme_color_override("font_color", Color(c[0]));bc.add_theme_color_override("font_pressed_color", Color(c[0]).lightened(0.3))
	props = VBoxContainer.new();props.add_theme_constant_override("separation", 4);v.add_child(props)
	r = _row(v)
	_b(r, "↶ Zpět (Ctrl+Z)", undo)
	_b(r, "✓ Uložit (Ctrl+S)", save)
	_b(r, "⟳ Uložit a načíst znovu", save_reload)
	_b(r, "⇪ Export", _export)
	_b(r, "⇩ Import", _import)
	_b(r, "Obnovit původní", reset_plan)
	_b(r, "✔ Hotovo (E)", toggle)
	info = Label.new();info.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART;info.custom_minimum_size = Vector2(590, 0)
	info.add_theme_color_override("font_color", Color("c9d1d8"));info.add_theme_font_size_override("font_size", 12);v.add_child(info)
	_set_dn(50);_set_med("wort");_set_paint("#e0452f");_set_tool("select")

func _part_btn(r: Control, pd: Array) -> void :
	var bp: = _b(r, pd[1], _set_part.bind(pd[0]));bp.toggle_mode = true;btns["part_" + pd[0]] = bp

func _row(parent: Control) -> HFlowContainer:
	var r: = HFlowContainer.new();r.add_theme_constant_override("h_separation", 4);r.add_theme_constant_override("v_separation", 4);parent.add_child(r);return r

func _lab(parent: Control, text: String) -> Label:
	var l: = Label.new();l.text = text;l.add_theme_color_override("font_color", Color("8fa0ad"));parent.add_child(l);return l

func _b(parent: Control, text: String, f: Callable) -> Button:
	var b: = Button.new();b.text = text;b.focus_mode = Control.FOCUS_NONE;b.pressed.connect(f);parent.add_child(b);return b

const TOOLS: = ["select", "pipe", "hose", "pitch", "joint", "measure", "route", "paint"]

func _set_tool(t: String) -> void :
	tool = t;draw_pts.clear();pitch_a = null;meas_a = null;route_a = null
	if t != "route": route_hl.clear()
	for k in btns:
		if k in TOOLS: btns[k].set_pressed_no_signal(k == t)
		elif k.begins_with("part_"): btns[k].set_pressed_no_signal(t == "part" and k == "part_" + part_key)
	_hint();queue_redraw()

func _set_part(k: String) -> void :
	part_key = k;_set_tool("part")

func _set_dn(dn: int) -> void :
	dn_new = dn
	for d in [10, 15, 25, 40, 50, 75]: btns["dn%d" % d].set_pressed_no_signal(d == dn)

func _set_med(md: String) -> void :
	med_new = md
	for x in MEDIA: btns["med_" + x[0]].set_pressed_no_signal(x[0] == md)

func _set_paint(c: String) -> void :
	paint_col = c
	for x in COLORS: btns["col_" + x[0]].set_pressed_no_signal(x[0] == c)
	if tool != "paint": _set_tool("paint")

func _part_def(k: String) -> Array:
	for pd in PARTS:
		if pd[0] == k: return pd
	return PARTS[0]

func _hint() -> void :
	var h: = {"select": "Klik = vybrat a táhnout. Tažení po prázdnu = rámeček (Shift přidá k výběru). Bod trubky přetáhni, dvojklik na trubku přidá bod, pravým na bod ho smaže. Roh nádoby táhni = velikost. Šipky posun, Del smaže, R otočí, Ctrl+D kopie. Posun plánku: prostřední tlačítko / WASD, zoom kolečkem.",
		"pipe": "KRESLENÍ TRUBKY: klikej body (mřížka 10 cm, svisle/vodorovně; Alt = volně). Konec na jiné trubce nebo hrdle se napojí a trubka skončí. Jinak ukončí pravé tlačítko, Enter nebo klik na poslední bod. Esc zruší.",
		"hose": "NEREZOVÁ HADICE (opletená): klikni začátek (hrdlo katety / konec trubky) a konec – hadice se prověsí obloukem.",
		"pitch": "SPÍLACÍ HADICE: klikni začátek (výstup z chladiče / trubka), pak tank nebo spilku. Vznikne hadice s průhledítkem a T-kusem se dvěma klapkami (do tanku / výpust).",
		"joint": "SPOJ / ROZPOJ: klikni na místo, kde se trubky kříží nebo kde konec dosedá na jinou trubku – přepne propojení (žlutý bod = spojeno).",
		"measure": "MĚŘENÍ: klikni dva body – ukáže vzdálenost v metrech.",
		"route": "CESTA: klikni na první nádobu, pak na druhou – ukáže, kudy se dá potrubím projet a přes které klapky a čerpadlo. Klapky nemění.",
		"paint": "BARVA: klikni na trubku – obarví se proužkem zvolené barvy (pro přehled cest). Barva „bez“ proužek smaže.",
		"part": "PŘIDAT „%s“: " % str(_part_def(part_key)[1]) + ("klikni na trubku – díl se vloží do trubky ve směru trubky." if _part_def(part_key)[2] in ON_PIPE else "klikni do plánu. Vlastnosti (velikost, tvar, vazba na simulaci) nastavíš vpravo dole v panelu.")}
	info.text = h.get(tool, "") + ("\n● neuložené změny – Ctrl+S" if dirty else "")

func toggle() -> void :
	on = not on
	ui.visible = on;visible = on
	if on:
		m.running = false
		if m.tiles_root != null: m.tiles_root.visible = false
		if m.detail_spr != null: m.detail_spr.visible = false
		var lv: Array = m.make_plan_layers();live_s = lv[0];live_i = lv[1]
		if m.game != null: m.game.quest.visible = false
		# rozvaděč v editoru nepotřebuješ – víc místa na plánek
		dock_was = m.dock_on
		if m.dock_on: m.dock_temp(false)
		m.hud.toast("Editor: varna pozastavena. Úpravy se projeví hned, Ctrl+S je uloží.")
	else:
		if live_s != null: live_s.queue_free();live_i.queue_free();live_s = null
		if m.tiles_root != null: m.tiles_root.visible = true
		if m.game != null: m.game.quest.visible = true
		if dock_was and not m.dock_on: m.dock_temp(true)
		m.detail_rect = Rect2();m.detail_sig = ""
		if dirty: m.hud.toast("Pozor: změny nejsou uložené (✎ → Uložit). Platí do zavření programu.")
		m.compute_layout()
		m._bake()
	sel = null;multi.clear();draw_pts.clear();pitch_a = null;route_hl.clear();_props();_hint();queue_redraw()

# ---------- změny ----------
func _snap_undo(key: = "") -> void :
	# opakované drobné změny téhož pole (šipky, číselník) = jeden krok zpět
	var now: = Time.get_ticks_msec()
	if key != "" and key == undo_key and now - undo_t < 1500:
		undo_t = now;return
	undo_key = key;undo_t = now
	undo_stack.append(JSON.stringify({"items": _clean_items(), "joints": m.plan.get("joints", [])}))
	if undo_stack.size() > 100: undo_stack.pop_front()

func undo() -> void :
	if undo_stack.is_empty():
		m.hud.toast("Není co vracet.");return
	var d = JSON.parse_string(undo_stack.pop_back())
	undo_key = ""
	_restore(d.items, d.joints)
	sel = null;multi.clear();_changed();_props()

## obnoví seznam dílů a zachová původní slovníky (živé nádoby a čerpadla na ně odkazují)
func _restore(new_items: Array, joints: Array) -> void :
	var byid: = {}
	for it in m.items: byid[it.id] = it
	for k in grave: if not byid.has(k): byid[k] = grave[k]
	var out: = []
	for nd in new_items:
		var o = byid.get(nd.id)
		if o != null:
			o.clear();o.merge(nd);out.append(o)
		else: out.append(nd)
	for it in m.items:
		if not out.has(it): grave[it.id] = it
	m.items.clear();m.items.append_array(out);m.plan.items = m.items;m.plan.joints = joints

func _changed() -> void :
	dirty = true
	for v in m.items:
		if v.type == "valve": v["_ang"] = deg_to_rad(float(v.get("rot", 0)))
	for n in m.live_nodes:
		if n.it == null: continue
		n.visible = m.items.has(n.it)
		if n.kind == "pump": n.position = Vector2(float(n.it.x), float(n.it.y))
	m._init_outlets()
	m.sim.build_net(m.items, m.plan.get("joints", []))
	m.sim.valve_ver += 1
	if m.flow_layer != null: m.flow_layer.build()
	if live_s != null: live_s.queue_redraw();live_i.queue_redraw()
	m.redraw();_hint();queue_redraw()

func _live_redraw() -> void :
	for n in m.live_nodes:
		if n.it != null and n.kind == "pump": n.position = Vector2(float(n.it.x), float(n.it.y))
	if live_s != null: live_s.queue_redraw();live_i.queue_redraw()
	m.redraw();queue_redraw()

func _clean_items() -> Array:
	var its: = []
	for it in m.items:
		var c: Dictionary = it.duplicate(true)
		for k in c.keys():
			if str(k).begins_with("_") and k != "_on": c.erase(k)
		its.append(c)
	return its

## plán bez pomocných klíčů (_ang apod.)
func _clean_plan() -> Dictionary:
	var out: Dictionary = m.plan.duplicate(false)
	out.items = _clean_items()
	return out

func save() -> bool:
	m.plan.items = m.items
	var f: = FileAccess.open(m.USER_PLAN, FileAccess.WRITE)
	if f == null:
		m.hud.toast("Plán nejde uložit.");return false
	f.store_string(JSON.stringify(_clean_plan(), "  "));f.close()
	dirty = false;m.plan_user = true;_hint()
	m.hud.toast("Plán varny uložen (%d dílů). Načte se i po restartu." % m.items.size())
	m.sim.logm("Editor: upravený plán varny uložen.")
	return true

## uloží a načte celý trenažér znovu – nové nádoby / čerpadla / vazby na simulaci se plně oživí
func save_reload() -> void :
	if not save(): return
	m.save_game()
	m.hud.toast("Plán uložen – načítám trenažér znovu…")
	await get_tree().create_timer(0.5).timeout
	get_tree().reload_current_scene()

func _export() -> void :
	_dialog(FileDialog.FILE_MODE_SAVE_FILE, func(path: String):
		var f: = FileAccess.open(path, FileAccess.WRITE)
		if f == null:
			m.hud.toast("Export se nepovedl.");return
		f.store_string(JSON.stringify(_clean_plan(), "  "));f.close()
		m.hud.toast("Plán vyexportován: " + path))

func _import() -> void :
	_dialog(FileDialog.FILE_MODE_OPEN_FILE, func(path: String):
		var f: = FileAccess.open(path, FileAccess.READ)
		if f == null:
			m.hud.toast("Soubor nejde otevřít.");return
		var d = JSON.parse_string(f.get_as_text())
		if not (d is Dictionary and d.has("items") and d.items is Array and d.items.size() > 5):
			m.hud.toast("Tohle není plán varny (chybí díly).");return
		_snap_undo()
		_restore(d.items, d.get("joints", []))
		sel = null;multi.clear();_changed();_props()
		m.hud.toast("Plán načten (%d dílů). „Uložit a načíst znovu“ ho uloží natrvalo." % m.items.size()))

func _dialog(mode: int, cb: Callable) -> void :
	if fdlg != null: fdlg.queue_free()
	fdlg = FileDialog.new();fdlg.access = FileDialog.ACCESS_FILESYSTEM;fdlg.file_mode = mode
	fdlg.filters = PackedStringArray(["*.json ; Plán varny"]);fdlg.current_file = "plan_varny.json"
	fdlg.use_native_dialog = true;fdlg.title = "Plán varny"
	fdlg.file_selected.connect(cb)
	ui.add_child(fdlg);fdlg.popup_centered(Vector2i(900, 600))

func reset_plan() -> void :
	if FileAccess.file_exists(m.USER_PLAN): DirAccess.remove_absolute(ProjectSettings.globalize_path(m.USER_PLAN))
	m.save_game()
	m.hud.toast("Vrácen původní plán – načítám znovu…")
	await get_tree().create_timer(0.6).timeout
	get_tree().reload_current_scene()

# ---------- pomocné ----------
func _z() -> float: return m.cam.zoom.x if m.cam != null else 1.0
func _snap(p: Vector2) -> Vector2: return (p / GRID).round() * GRID
func _uid(pre: String) -> String: return "%s_ed%d" % [pre, Time.get_ticks_usec()]
func _protected(it) -> bool: return it.type in PROTECT or str(it.get("bind", "")) != ""
func _is_pipe(it) -> bool: return it.type == "pipe"
func _name(it) -> String:
	var l: = str(it.get("label", ""))
	return l if l != "" else str(it.type)

func _pipes(with_hoses: = false) -> Array:
	return m.items.filter(func(it): return it.type == "pipe" and (with_hoses or not it.get("hose", false)) and it.pts.size() >= 2)

## nejbližší bod trubky (koncový nebo zlomový)
func _near_pt(w: Vector2, tol: float, with_hoses: = true):
	var best = null; var bd: = tol
	for p in _pipes(with_hoses):
		for i in p.pts.size():
			var d: = w.distance_to(Vector2(p.pts[i][0], p.pts[i][1]))
			if d < bd: bd = d;best = {"t": "pt", "it": p, "i": i}
	return best

## nejbližší místo na úseku trubky: {it, i (index úseku), q (bod na úseku), ang}
func _near_seg(w: Vector2, tol: float, with_hoses: = false):
	var best = null; var bd: = tol
	for p in _pipes(with_hoses):
		for i in p.pts.size() - 1:
			var a: = Vector2(p.pts[i][0], p.pts[i][1]); var b: = Vector2(p.pts[i + 1][0], p.pts[i + 1][1])
			var q: = Geometry2D.get_closest_point_to_segment(w, a, b)
			var d: = w.distance_to(q)
			if d < bd: bd = d;best = {"it": p, "i": i, "q": q, "ang": rad_to_deg((b - a).angle()), "a": a, "b": b}
	return best

func _vrect(it) -> Rect2: return Rect2(float(it.x), float(it.y), float(it.get("w", 100)), float(it.get("h", 100)))

func _near_item(w: Vector2, tol: float):
	var best = null; var bd: = tol; var vbest = null; var va: = 1e18
	for it in m.items:
		if not it.has("x") or it.type in ["zone", "pipe"]: continue
		if it.type == "vessel" and it.has("w"):
			var r: = _vrect(it)
			if r.grow(4.0).has_point(w) and r.get_area() < va: va = r.get_area();vbest = it
			continue
		if it.type == "panel" and it.has("w"):
			if _vrect(it).has_point(w) and vbest == null: vbest = it
			continue
		var c: = Vector2(it.x, it.y)
		if it.type == "text": c += Vector2(30, -6)
		var d: = w.distance_to(c)
		if d < bd: bd = d;best = it
	return best if best != null else vbest

## hrdla T-kusů, kolen a katety – i na ně se dá napojit trubka
func _ports() -> Array:
	var o: = []
	for f in m.items:
		if f.type == "fitting":
			for q in m.sim.fit_ports(f): o.append(q)
	for ol in m.outlets: o.append(ol.front)
	return o

## přichycení konce trubky: k hrdlu, k bodu jiné trubky, nebo na úsek (pak vznikne spoj – joint)
func _snap_pipe_pt(w: Vector2, alt: bool) -> Dictionary:
	var tol: = 14.0 / _z()
	for q in _ports():
		if w.distance_to(q) < tol: return {"p": q, "kind": "port"}
	var np = _near_pt(w, tol)
	if np != null: return {"p": Vector2(np.it.pts[np.i][0], np.it.pts[np.i][1]), "kind": "pt"}
	var ns = _near_seg(w, tol)
	if ns != null: return {"p": ns.q.round(), "kind": "seg"}
	return {"p": w if alt else _snap(w), "kind": "free"}

func _sel_items() -> Array:
	if multi.size(): return multi
	if sel != null: return [sel.it]
	return []

# ---------- vstup ----------
func handle(e: InputEvent) -> bool:
	if not on: return false
	var fo = get_viewport().gui_get_focus_owner()
	if e is InputEventKey and fo is LineEdit: return false
	if e is InputEventKey and e.pressed:
		var k: = e as InputEventKey
		if k.ctrl_pressed and k.keycode == KEY_Z: undo();return true
		if k.ctrl_pressed and k.keycode == KEY_S: save();return true
		if k.ctrl_pressed and k.keycode == KEY_D: _duplicate();return true
		if k.ctrl_pressed and k.keycode == KEY_A: _select_visible();return true
		if k.keycode in [KEY_LEFT, KEY_RIGHT, KEY_UP, KEY_DOWN] and _sel_items().size():
			var st: = 100.0 if k.shift_pressed else GRID
			var dv: Vector2 = {KEY_LEFT: Vector2(-st, 0), KEY_RIGHT: Vector2(st, 0), KEY_UP: Vector2(0, -st), KEY_DOWN: Vector2(0, st)}[k.keycode]
			_snap_undo("arrows");_move_items(_sel_items(), dv);_changed();_props();return true
		match k.keycode:
			KEY_ESCAPE:
				if draw_pts.size() or pitch_a != null or meas_a != null or route_a != null:
					draw_pts.clear();pitch_a = null;meas_a = null;route_a = null;queue_redraw()
				elif sel != null or multi.size():
					sel = null;multi.clear();_props();queue_redraw()
				else: toggle()
				return true
			KEY_E: toggle();return true
			KEY_DELETE, KEY_BACKSPACE: _delete();return true
			KEY_R: _rotate();return true
			KEY_ENTER, KEY_KP_ENTER: _finish_pipe();return true
			KEY_V: _set_tool("select");return true
			KEY_T: _set_tool("pipe");return true
			KEY_H: _set_tool("hose");return true
			KEY_J: _set_tool("joint");return true
			KEY_M: _set_tool("measure");return true
			KEY_C: _set_tool("route");return true
			KEY_B: _set_tool("paint");return true
			KEY_K: _set_part("klapka");return true
		return false
	if e is InputEventMouseMotion:
		hover_w = m.screen_to_world(e.position)
		if drag:
			_drag_to(hover_w, e.alt_pressed);return true
		if tool != "select": queue_redraw()
		return false
	if e is InputEventMouseButton:
		var mb: = e as InputEventMouseButton
		if mb.button_index in [MOUSE_BUTTON_WHEEL_UP, MOUSE_BUTTON_WHEEL_DOWN, MOUSE_BUTTON_MIDDLE]: return false
		var w: Vector2 = m.screen_to_world(mb.position)
		if fo != null: fo.release_focus()
		if mb.button_index == MOUSE_BUTTON_LEFT:
			if mb.pressed:
				if mb.double_click and tool == "select": _insert_point(w);return true
				match tool:
					"select": _press_select(w, mb.shift_pressed)
					"pipe": _pipe_click(w, mb.alt_pressed)
					"hose": _hose_click(w)
					"pitch": _pitch_click(w)
					"joint": _joint_click(w)
					"measure": _measure_click(w)
					"route": _route_click(w)
					"paint": _paint_click(w)
					"part": _add_part(w)
			else:
				if drag: _release()
			return true
		if mb.button_index == MOUSE_BUTTON_RIGHT and mb.pressed:
			if tool == "pipe" and draw_pts.size(): _finish_pipe();return true
			if tool != "select":
				_set_tool("select");return true
			var np = _near_pt(w, 12.0 / _z())
			if np != null and np.it.pts.size() > 2:
				_snap_undo();np.it.pts.remove_at(np.i);sel = null;_changed();_props()
			return true
	return false

func _press_select(w: Vector2, shift: bool) -> void :
	drag_start = w
	# roh vybrané nádoby = změna velikosti
	if sel != null and sel.t == "item" and sel.it.type in ["vessel", "panel"] and sel.it.has("w"):
		var r: = _vrect(sel.it)
		if w.distance_to(r.end) < 14.0 / _z():
			_snap_undo();drag = true;drag_mode = "size";return
	var np = _near_pt(w, 10.0 / _z())
	var it = _near_item(w, 22.0 / _z())
	var ns = null
	if np == null and it == null: ns = _near_seg(w, 10.0 / _z(), true)
	var hit = it if it != null else (ns.it if ns != null else (np.it if np != null else null))
	# posun celé skupiny
	if hit != null and multi.has(hit) and not shift:
		_snap_undo();drag = true;drag_mode = "group";drag_orig = _orig(multi);return
	if shift:
		if hit != null:
			if sel != null and not multi.has(sel.it): multi.append(sel.it)
			sel = null
			if multi.has(hit): multi.erase(hit)
			else: multi.append(hit)
			_props();queue_redraw();return
		drag = true;drag_mode = "box";box_a = w;return
	multi.clear()
	if np != null and (it == null or w.distance_to(Vector2(np.it.pts[np.i][0], np.it.pts[np.i][1])) < w.distance_to(Vector2(it.x, it.y))):
		sel = np;_snap_undo();drag = true;drag_mode = "one";queue_redraw();_props();return
	if it != null:
		sel = {"t": "item", "it": it};_snap_undo();drag = true;drag_mode = "one";drag_off = Vector2(it.x, it.y) - w;queue_redraw();_props();return
	if ns != null:
		sel = {"t": "pipe", "it": ns.it};drag = true;drag_mode = "group";multi = [];drag_orig = _orig([ns.it]);_snap_undo();queue_redraw();_props();return
	sel = null;_props()
	drag = true;drag_mode = "box";box_a = w
	queue_redraw()

func _orig(list: Array) -> Array:
	var o: = []
	for it in list: o.append([it, it.pts.duplicate(true) if it.has("pts") else Vector2(float(it.get("x", 0)), float(it.get("y", 0)))])
	return o

func _move_items(list: Array, dv: Vector2) -> void :
	for it in list:
		if it.has("pts"):
			for q in it.pts:
				q[0] = float(q[0]) + dv.x;q[1] = float(q[1]) + dv.y
		elif it.has("x"):
			it.x = float(it.x) + dv.x;it.y = float(it.y) + dv.y

func _drag_to(w: Vector2, alt: bool) -> void :
	match drag_mode:
		"box":
			queue_redraw();return
		"size":
			var it = sel.it; var e2: = w if alt else _snap(w)
			it.w = maxf(20.0, e2.x - float(it.x));it.h = maxf(10.0, e2.y - float(it.y))
			_live_redraw();_props_soft();return
		"group":
			var dv: Vector2 = w - drag_start
			if not alt: dv = _snap(dv)
			for o in drag_orig:
				var it = o[0]
				if it.has("pts"):
					for i in it.pts.size(): it.pts[i] = [o[1][i][0] + dv.x, o[1][i][1] + dv.y]
				else:
					it.x = o[1].x + dv.x;it.y = o[1].y + dv.y
			_live_redraw();return
	if sel == null: return
	if sel.t == "pt":
		var own: = Vector2(sel.it.pts[sel.i][0], sel.it.pts[sel.i][1])
		var sp: Dictionary = _snap_pipe_pt(w, alt)
		var p: Vector2 = sp.p
		# nepřichytávat k sobě samé
		if sp.kind == "pt" and p == own: p = w if alt else _snap(w)
		sel.it.pts[sel.i] = [p.x, p.y]
	elif sel.t == "item":
		var it = sel.it; var p2: Vector2 = w + drag_off
		if it.type in ON_PIPE:
			# armatura a čerpadlo musí sedět na trubce
			var ns = _near_seg(p2, 24.0 / _z())
			if ns != null:
				p2 = ns.q.round()
				if it.type != "pump": it["rot"] = roundi(ns.ang)
			else: p2 = _snap(p2)
		elif not alt: p2 = _snap(p2)
		it.x = p2.x;it.y = p2.y
	_live_redraw();_props_soft()

func _release() -> void :
	drag = false
	if drag_mode == "box":
		var r: = Rect2(box_a, Vector2.ZERO).expand(hover_w)
		if r.size.length() > 6.0 / _z():
			for it in m.items:
				if it.type == "zone": continue
				var inside: = false
				if it.has("pts"):
					inside = it.pts.size() > 0
					for q in it.pts:
						if not r.has_point(Vector2(q[0], q[1])): inside = false;break
				elif it.type in ["vessel", "panel"] and it.has("w"): inside = r.encloses(_vrect(it))
				elif it.has("x"): inside = r.has_point(Vector2(it.x, it.y))
				if inside and not multi.has(it): multi.append(it)
			if multi.size() == 1:
				sel = {"t": "pipe" if multi[0].has("pts") else "item", "it": multi[0]};multi.clear()
			elif multi.size(): sel = null
		drag_mode = "";_props();queue_redraw();return
	drag_mode = ""
	_changed();_props()

func _select_visible() -> void :
	var r: Rect2 = m.visible_world_rect()
	multi.clear();sel = null
	for it in m.items:
		if it.type == "zone": continue
		var p: = Vector2(it.pts[0][0], it.pts[0][1]) if it.has("pts") and it.pts.size() else Vector2(float(it.get("x", -1e9)), float(it.get("y", -1e9)))
		if r.has_point(p): multi.append(it)
	_props();queue_redraw();m.hud.toast("Vybráno %d dílů na obrazovce." % multi.size())

func _insert_point(w: Vector2) -> void :
	var ns = _near_seg(w, 10.0 / _z(), true)
	if ns == null: return
	_snap_undo()
	var q: Vector2 = ns.q.round()
	ns.it.pts.insert(ns.i + 1, [q.x, q.y])
	sel = {"t": "pt", "it": ns.it, "i": ns.i + 1};multi.clear();_changed();_props()

# ---------- trubka ----------
func _pipe_click(w: Vector2, alt: bool) -> void :
	var sp: Dictionary = _snap_pipe_pt(w, alt)
	var p: Vector2 = sp.p
	if draw_pts.size() and not alt and sp.kind == "free":
		var last: Vector2 = draw_pts[draw_pts.size() - 1].p
		if absf(p.x - last.x) > absf(p.y - last.y): p.y = last.y
		else: p.x = last.x
	if draw_pts.size() and p.distance_to(draw_pts[draw_pts.size() - 1].p) < 6.0 / _z():
		_finish_pipe();return
	draw_pts.append({"p": p, "kind": sp.kind})
	if draw_pts.size() >= 2 and sp.kind != "free":
		_finish_pipe();return
	m.hud.toast("Bod %d – další klik pokračuje, pravé tlačítko / Enter trubku ukončí." % draw_pts.size())
	queue_redraw()

func _finish_pipe() -> void :
	if draw_pts.size() < 2:
		if draw_pts.size() == 1: m.hud.toast("Trubka potřebuje aspoň dva body.")
		draw_pts.clear();queue_redraw();return
	_snap_undo()
	var pts: = []
	for d in draw_pts: pts.append([d.p.x, d.p.y])
	var np: = {"id": _uid("pipe"), "type": "pipe", "dn": dn_new, "medium": med_new, "label": "", "pts": pts}
	m.items.append(np)
	var J: Array = m.plan.get("joints", [])
	for d in [draw_pts[0], draw_pts[draw_pts.size() - 1]]:
		if d.kind == "seg": J.append([d.p.x, d.p.y])
	m.plan.joints = J
	draw_pts.clear();_changed()
	sel = {"t": "pipe", "it": np};multi.clear();_props()
	m.hud.toast("Trubka přidána (DN%d). Konce na jiné trubce se napojily." % dn_new)

# ---------- hadice ----------
func _hose_pts(A: Vector2, B: Vector2) -> Array:
	var d: = B - A; var L: = d.length()
	var n: = Vector2(-d.y, d.x) / maxf(1.0, L)
	if n.y < 0: n = -n          # prověs dolů
	var bow: = minf(120.0, L * 0.18)
	var o: = []
	for i in 5:
		var t: = i / 4.0
		var q: = A.lerp(B, t) + n * bow * sin(PI * t)
		o.append([roundf(q.x), roundf(q.y)])
	return o

func _hose_click(w: Vector2) -> void :
	var sp: Dictionary = _snap_pipe_pt(w, false)
	draw_pts.append({"p": sp.p, "kind": sp.kind})
	if draw_pts.size() < 2:
		m.hud.toast("Hadice: teď klikni na konec.");queue_redraw();return
	var A: Vector2 = draw_pts[0].p; var B: Vector2 = draw_pts[1].p
	draw_pts.clear()
	if A.distance_to(B) < 40.0:
		m.hud.toast("Hadice je moc krátká.");return
	_snap_undo()
	var h: = {"id": _uid("pipe"), "type": "pipe", "hose": true, "look": "hose", "dn": mini(dn_new, 50), "medium": med_new, "label": "nerezová hadice", "pts": _hose_pts(A, B)}
	m.items.append(h)
	_changed();sel = {"t": "pipe", "it": h};multi.clear();_props()
	m.hud.toast("Hadice přidána.")

# ---------- spílací hadice (průhledítko + T-kus se dvěma klapkami) – stejně jako ve webové aplikaci ----------
func _pitch_click(w: Vector2) -> void :
	var sp: Dictionary = _snap_pipe_pt(w, false)
	if pitch_a == null:
		pitch_a = sp.p;m.hud.toast("Spílací hadice: teď klikni na tank / spilku (konec hadice).");queue_redraw();return
	var A: Vector2 = pitch_a; var B: Vector2 = sp.p; pitch_a = null
	var d: = B - A; var L: = d.length()
	if L < 200.0:
		m.hud.toast("Moc krátké – konec dej dál (aspoň 2 m).");return
	_snap_undo()
	var u: = d / L; var nrm: = Vector2(-u.y, u.x)
	var ang: = rad_to_deg(u.angle()); var st: = str(Time.get_ticks_usec())
	var C: = (B - u * 110.0).round()
	var fit: = {"id": "fit_ed%sp" % st, "type": "fitting", "label": "T-kus spílací", "x": C.x, "y": C.y, "ports": [fposmod(ang + 180.0, 360.0), fposmod(ang, 360.0), fposmod(ang + 90.0, 360.0)], "arm": 30}
	m.items.append(fit)
	var fp: Array = m.sim.fit_ports(fit)
	var hp: Array = _hose_pts(A, fp[0])
	m.items.append({"id": "pipe_ed%sh" % st, "type": "pipe", "medium": "wort", "hose": true, "dn": 50, "label": "spílací hadice", "pts": hp})
	var s0: = Vector2(hp[2][0], hp[2][1]); var s1: = Vector2(hp[3][0], hp[3][1])
	m.items.append({"id": "inl_ed%ss" % st, "type": "inl", "k": "pruh", "label": "průhledítko", "x": roundf((s0.x + s1.x) / 2.0), "y": roundf((s0.y + s1.y) / 2.0), "rot": roundi(rad_to_deg((s1 - s0).angle()))})
	m.items.append({"id": "pipe_ed%st" % st, "type": "pipe", "medium": "wort", "dn": 50, "label": "", "pts": [[fp[1].x, fp[1].y], [B.x, B.y]]})
	var vt: Vector2 = ((fp[1] + B) / 2.0).round()
	m.items.append({"id": "valve_ed%s1" % st, "type": "valve", "kind": "klapka", "label": "klapka tank", "open": false, "x": vt.x, "y": vt.y, "rot": roundi(ang)})
	var E: Vector2 = (fp[2] + nrm * 90.0).round()
	m.items.append({"id": "pipe_ed%sv" % st, "type": "pipe", "medium": "wort", "dn": 50, "label": "výpust", "pts": [[fp[2].x, fp[2].y], [E.x, E.y]]})
	var vv: Vector2 = ((fp[2] + E) / 2.0).round()
	m.items.append({"id": "valve_ed%s2" % st, "type": "valve", "kind": "klapka", "label": "klapka výpust", "open": false, "x": vv.x, "y": vv.y, "rot": roundi(ang + 90.0)})
	if sp.kind == "seg":
		var J: Array = m.plan.get("joints", []);J.append([B.x, B.y]);m.plan.joints = J
	_changed();_set_tool("select")
	m.hud.toast("Spílací hadice s průhledítkem a T-kusem se dvěma klapkami (do tanku / výpust) přidána. Klapky jsou zavřené.")

# ---------- spoj / rozpoj ----------
func _joint_click(w: Vector2) -> void :
	var J: Array = m.plan.get("joints", [])
	var tol: = 12.0 / _z()
	for i in J.size():
		if w.distance_to(Vector2(J[i][0], J[i][1])) < tol:
			_snap_undo();J.remove_at(i);m.plan.joints = J;_changed()
			m.hud.toast("Rozpojeno – trubky tudy jen procházejí přes sebe.");return
	var best: = Vector2.INF; var bd: = tol
	var ps: Array = _pipes()
	for a in ps.size():
		for b in range(a + 1, ps.size()):
			var A = ps[a]; var B = ps[b]
			for i in A.pts.size() - 1:
				for j in B.pts.size() - 1:
					var x = Geometry2D.segment_intersects_segment(Vector2(A.pts[i][0], A.pts[i][1]), Vector2(A.pts[i + 1][0], A.pts[i + 1][1]), Vector2(B.pts[j][0], B.pts[j][1]), Vector2(B.pts[j + 1][0], B.pts[j + 1][1]))
					if x != null and w.distance_to(x) < bd: bd = w.distance_to(x);best = (x as Vector2).round()
	if best == Vector2.INF:
		var np = _near_pt(w, tol, false)
		if np != null: best = Vector2(np.it.pts[np.i][0], np.it.pts[np.i][1])
	if best == Vector2.INF:
		m.hud.toast("Tady se žádné trubky nekříží ani nedotýkají.");return
	_snap_undo();J.append([best.x, best.y]);m.plan.joints = J;_changed()
	m.hud.toast("Spojeno – kapalina tudy může přetékat mezi trubkami.")

# ---------- měření, cesta, barva ----------
func _measure_click(w: Vector2) -> void :
	var p: Vector2 = _snap_pipe_pt(w, false).p
	if meas_a == null:
		meas_a = p;m.hud.toast("Měření: klikni na druhý bod.");queue_redraw();return
	var d: Vector2 = p - meas_a
	m.hud.toast("Vzdálenost %.2f m (vodorovně %.2f m, svisle %.2f m)" % [d.length() / PX_PER_M, absf(d.x) / PX_PER_M, absf(d.y) / PX_PER_M])
	meas_a = null;queue_redraw()

func _vessel_at(w: Vector2):
	var best = null; var ba: = 1e18
	for it in m.items:
		if it.type == "vessel" and it.has("w") and str(it.get("bind", "")) != "" and _vrect(it).grow(8.0).has_point(w):
			if _vrect(it).get_area() < ba: ba = _vrect(it).get_area();best = it
	return best

func _route_click(w: Vector2) -> void :
	var v = _vessel_at(w)
	if v == null:
		m.hud.toast("Klikni na nádobu (pánev, SK, vířivka, bojler, spilka…).");return
	if route_a == null:
		route_a = v;route_hl.clear();m.hud.toast("Cesta z: %s – teď klikni na cílovou nádobu." % _name(v));queue_redraw();return
	var a = route_a; route_a = null
	var found = null; var via: = ""
	var r0 = m.sim.route_valves(a.bind, v.bind, [])
	if r0 != null: found = r0
	else:
		for pb in ["p1", "p2", "p3", "phw", "plv"]:
			var r1 = m.sim.route_valves(a.bind, v.bind, [pb])
			if r1 != null:
				found = r1;via = pb;break
	route_hl.clear()
	if found == null:
		m.hud.toast("Z %s do %s se potrubím nedostaneš (chybí spoj nebo trubka)." % [_name(a), _name(v)]);queue_redraw();return
	var keys: Array = m.sim.last_route
	for i in keys.size() - 1:
		var n = m.sim.net.nodes.get(keys[i])
		if n == null: continue
		var pl: = PackedVector2Array([Vector2(n.x, n.y), Vector2(m.sim.net.nodes[keys[i + 1]].x, m.sim.net.nodes[keys[i + 1]].y)])
		for e in n.edges:
			if (e.a == keys[i] and e.b == keys[i + 1]) or (e.b == keys[i] and e.a == keys[i + 1]):
				pl = PackedVector2Array(e.pts);break
		route_hl.append(pl)
	var pn: = ""
	if via != "":
		for it in m.items:
			if it.type == "pump" and it.get("bind", "") == via: pn = " přes čerpadlo %s" % _name(it)
	m.hud.toast("Cesta %s → %s%s: %d klapek na trase (zvýrazněno žlutě, klapky jsem neměnil)." % [_name(a), _name(v), pn, found.size()])
	queue_redraw()

func _paint_click(w: Vector2) -> void :
	var ns = _near_seg(w, 12.0 / _z(), true)
	if ns == null:
		m.hud.toast("Klikni na trubku.");return
	_snap_undo()
	if paint_col == "": ns.it.erase("color")
	else: ns.it["color"] = paint_col
	_changed()

# ---------- díly ----------
func _add_part(w: Vector2) -> void :
	var pd: Array = _part_def(part_key)
	var typ: String = pd[2]
	var it: Dictionary = (pd[3] as Dictionary).duplicate(true)
	it["type"] = typ;it["id"] = _uid({"valve": "valve", "inl": "inl", "flow": "flow", "fitting": "fit", "text": "text", "vessel": "vessel", "pump": "pump"}.get(typ, typ))
	if typ in ON_PIPE:
		var ns = _near_seg(w, 16.0 / _z())
		if ns == null:
			m.hud.toast("%s vlož přímo na trubku." % str(pd[1]).substr(2));return
		var q: Vector2 = ns.q.round()
		it.x = q.x;it.y = q.y
		if typ == "pump": it.dir = posmod(roundi(ns.ang / 90.0) * 90, 360)
		else: it.rot = roundi(ns.ang)
		if typ == "valve": it.open = false
	elif typ == "fitting":
		var sp: Dictionary = _snap_pipe_pt(w, false)
		var c: Vector2 = sp.p
		it.x = c.x;it.y = c.y
		# klik na konec trubky: natoč první hrdlo proti ní a posuň T-kus, ať hrdlo dosedne
		var np = _near_pt(w, 14.0 / _z(), false)
		if np != null and (np.i == 0 or np.i == np.it.pts.size() - 1):
			var o: = Vector2(np.it.pts[1 if np.i == 0 else np.i - 1][0], np.it.pts[1 if np.i == 0 else np.i - 1][1])
			var a0: = rad_to_deg((o - c).angle())
			var base: float = float(it.ports[0])
			var newp: = []
			for pa in it.ports: newp.append(fposmod(float(pa) - base + a0, 360.0))
			it.ports = newp
			var cc: = c - Vector2.from_angle(deg_to_rad(a0)) * float(it.arm)
			it.x = roundf(cc.x);it.y = roundf(cc.y)
	elif typ == "vessel":
		var c2: = _snap(w - Vector2(float(it.w), float(it.h)) * 0.5);it.x = c2.x;it.y = c2.y
	else:
		var c3: = _snap(w);it.x = c3.x;it.y = c3.y
	_snap_undo()
	m.items.append(it);sel = {"t": "item", "it": it};multi.clear();_changed();_props()
	if typ in ["vessel", "pump"]: m.hud.toast("%s přidáno. Vazbu na simulaci nastav v panelu, pak „Uložit a načíst znovu“." % _name(it))

func _duplicate() -> void :
	var list: Array = _sel_items()
	if list.is_empty(): return
	_snap_undo()
	var out: = []; var n: = 0
	for it in list:
		var c: Dictionary = it.duplicate(true);c.erase("_ang")
		c.id = _uid(str(it.type)) + str(n);n += 1
		if c.has("bind") and str(c.bind) != "": c.bind = ""     # kopie nemá vazbu na simulaci (jinak by byly dvě pánve)
		m.items.append(c);out.append(c)
	_move_items(out, Vector2(40, 40))
	if out.size() == 1:
		sel = {"t": "pipe" if out[0].has("pts") else "item", "it": out[0]};multi.clear()
	else:
		sel = null;multi = out
	_changed();_props()
	m.hud.toast("Zkopírováno %d dílů – přetáhni je na místo." % out.size())

func _delete() -> void :
	var list: Array = _sel_items()
	if list.is_empty(): return
	var prot: = list.filter(func(it): return _protected(it))
	if prot.size() and Time.get_ticks_msec() - del_arm > 3000:
		del_arm = Time.get_ticks_msec()
		m.hud.toast("Ve výběru je %s – má roli v simulaci. Opravdu smazat? Stiskni Del (Smazat) znovu." % _name(prot[0]));return
	del_arm = 0
	_snap_undo()
	for it in list:
		grave[it.id] = it;m.items.erase(it)
	sel = null;multi.clear();_changed();_props()
	m.hud.toast("Smazáno %d dílů (Ctrl+Z vrátí)." % list.size())

func _rot_item(it, deg: float) -> void :
	match it.type:
		"valve", "inl", "flow", "hopf": it.rot = posmod(int(it.get("rot", 0)) + int(deg), 360)
		"pump", "nozzle": it.dir = posmod(int(it.get("dir", 0)) + int(deg), 360)
		"fitting":
			var np: = []
			for pa in it.ports: np.append(fposmod(float(pa) + deg, 360.0))
			it.ports = np
		"vessel":
			if deg == 90.0 or deg == 270.0:
				var w0: = float(it.get("w", 100));it.w = it.get("h", 100);it.h = w0

func _rotate() -> void :
	if multi.size():
		_rotate_group();return
	if sel == null or sel.t != "item": return
	var it = sel.it
	_snap_undo()
	# klapku na trubce otočíme o 180° (zůstane ve směru trubky), ostatní o 90°
	_rot_item(it, 180.0 if it.type in ["valve", "inl", "flow", "pump"] else 90.0)
	_changed();_props()

func _rotate_group() -> void :
	var list: Array = multi
	var bb: = _bbox(list); var c: = bb.get_center().round()
	_snap_undo()
	var rp = func(q: Vector2) -> Vector2: return c + (q - c).rotated(PI * 0.5)
	for it in list:
		if it.has("pts"):
			for i in it.pts.size():
				var q: Vector2 = rp.call(Vector2(it.pts[i][0], it.pts[i][1])).round();it.pts[i] = [q.x, q.y]
		elif it.type in ["vessel", "panel"] and it.has("w"):
			var cen: Vector2 = rp.call(_vrect(it).get_center());_rot_item(it, 90.0)
			it.x = roundf(cen.x - float(it.w) / 2.0);it.y = roundf(cen.y - float(it.h) / 2.0)
		elif it.has("x"):
			var q2: Vector2 = rp.call(Vector2(it.x, it.y)).round();it.x = q2.x;it.y = q2.y;_rot_item(it, 90.0)
	var J: Array = m.plan.get("joints", [])
	for i in J.size():
		if bb.grow(2.0).has_point(Vector2(J[i][0], J[i][1])):
			var qj: Vector2 = rp.call(Vector2(J[i][0], J[i][1])).round();J[i] = [qj.x, qj.y]
	_changed();_props()

func _bbox(list: Array) -> Rect2:
	var r = null
	for it in list:
		var ri: Rect2
		if it.has("pts"):
			ri = Rect2(Vector2(it.pts[0][0], it.pts[0][1]), Vector2.ZERO)
			for q in it.pts: ri = ri.expand(Vector2(q[0], q[1]))
		elif it.type in ["vessel", "panel"] and it.has("w"): ri = _vrect(it)
		elif it.has("x"): ri = Rect2(Vector2(it.x, it.y), Vector2.ZERO)
		else: continue
		r = ri if r == null else r.merge(ri)
	return r if r != null else Rect2()

## zarovnání víc dílů: row = stejná výška středu, col = pod sebe, bottom = dna nádob, space = stejné rozestupy
func _align(mode: String) -> void :
	var list: Array = multi.filter(func(it): return not it.has("pts") and it.has("x"))
	if list.size() < 2:
		m.hud.toast("Vyber aspoň dva díly (ne trubky).");return
	_snap_undo()
	var cen = func(it) -> Vector2: return _vrect(it).get_center() if it.has("w") else Vector2(it.x, it.y)
	var ref: Vector2 = cen.call(list[0])
	match mode:
		"row":
			for it in list: it.y = float(it.y) + ref.y - cen.call(it).y
		"col":
			for it in list: it.x = float(it.x) + ref.x - cen.call(it).x
		"bottom":
			var by: float = float(list[0].y) + float(list[0].get("h", 0))
			for it in list: it.y = by - float(it.get("h", 0))
		"space":
			list.sort_custom(func(a, b): return cen.call(a).x < cen.call(b).x)
			var x0: float = cen.call(list[0]).x; var x1: float = cen.call(list[list.size() - 1]).x
			for i in list.size(): it_shift_x(list[i], x0 + (x1 - x0) * i / float(list.size() - 1) - cen.call(list[i]).x)
	for it in list:
		it.x = roundf(float(it.x));it.y = roundf(float(it.y))
	_changed();_props()

func it_shift_x(it, dx: float) -> void : it.x = float(it.x) + dx

# ---------- panel vlastností ----------
func _props_soft() -> void :
	# při tažení jen přepiš čísla polohy/velikosti, ať se panel nepřestavuje
	if sel == null or sel.t != "item": return
	for c in props.find_children("*", "SpinBox", true, false):
		var k: String = c.get_meta("key", "")
		if k != "" and sel.it.has(k): c.set_value_no_signal(float(sel.it[k]))

func _props() -> void :
	for c in props.get_children(): c.queue_free()
	if multi.size():
		var hb: = _row(props)
		var l: = _lab(hb, "Vybráno %d dílů:" % multi.size());l.add_theme_color_override("font_color", Color("f2c230"))
		_b(hb, "⇔ do řady", _align.bind("row"))
		_b(hb, "⇕ pod sebe", _align.bind("col"))
		_b(hb, "▁ dna stejně", _align.bind("bottom"))
		_b(hb, "↔ stejné rozestupy", _align.bind("space"))
		_b(hb, "↻ 90°", _rotate_group)
		_b(hb, "+ Kopie (Ctrl+D)", _duplicate)
		_b(hb, "✕ Smazat (Del)", _delete)
		return
	if sel == null: return
	var it = sel.it
	var hb2: = _row(props)
	var title: String = {"pipe": "Hadice" if it.get("hose", false) else "Trubka", "valve": "Armatura", "inl": "Díl v trubce", "flow": "Rotametr", "fitting": "Tvarovka", "text": "Popisek", "pump": "Čerpadlo", "vessel": "Nádoba", "spray": "Sprcha", "nozzle": "Tryska", "hopf": "Chmelový filtr", "panel": "Rozvaděč", "manifold": "Kateta", "gauge": "Stavoznak"}.get(it.type, str(it.type))
	var l2: = _lab(hb2, "%s · %s" % [title, str(it.id)]);l2.add_theme_color_override("font_color", Color("f2c230"))
	if it.type in ["valve", "pump", "fitting", "inl", "flow", "hopf", "nozzle", "vessel"]: _b(hb2, "⟳ Otočit (R)", _rotate)
	_b(hb2, "+ Kopie (Ctrl+D)", _duplicate)
	_b(hb2, "✕ Smazat (Del)", _delete)
	var g: = GridContainer.new();g.columns = 4;g.add_theme_constant_override("h_separation", 6);props.add_child(g)
	_ftext(g, it, "label", "popis")
	match it.type:
		"pipe":
			_fenum(g, it, "dn", "DN", [[10, "DN10"], [15, "DN15"], [25, "DN25"], [40, "DN40"], [50, "DN50"], [75, "DN75"]])
			_fenum(g, it, "medium", "teče", MEDIA)
			_fenum(g, it, "color", "barva", COLORS)
			_fbool(g, it, "hose", "hadice")
			_fbool(g, it, "solo", "nepropojovat s křížením")
			_lab(g, "délka");_lab(g, "%.2f m" % _plen(it))
		"valve":
			_fenum(g, it, "kind", "typ", VKINDS);_fbool(g, it, "open", "otevřená");_fnum(g, it, "rot", "úhel °", -360, 360, 1)
		"inl":
			_fenum(g, it, "k", "druh", IKINDS);_fnum(g, it, "rot", "úhel °", -360, 360, 1)
		"flow", "hopf":
			_fnum(g, it, "rot", "úhel °", -360, 360, 1)
		"fitting":
			_fnum(g, it, "arm", "rameno", 10, 120, 1)
			_ftext(g, it, "ports", "hrdla ° (čárkou)")
		"text":
			_fnum(g, it, "size", "písmo", 8, 80, 1)
		"vessel", "panel":
			_fnum(g, it, "w", "šířka", 10, 3000, 10);_fnum(g, it, "h", "výška", 10, 3000, 10)
			if it.type == "vessel":
				_fenum(g, it, "shape", "tvar", SHAPES)
				_fenum(g, it, "bind", "v simulaci", VBINDS)
		"pump":
			_fenum(g, it, "dir", "směr", [[0, "→"], [90, "↓"], [180, "←"], [270, "↑"]])
			_fenum(g, it, "bind", "v simulaci", PBINDS)
		"nozzle":
			_fenum(g, it, "dir", "směr", [[0, "→"], [90, "↓"], [180, "←"], [270, "↑"]])
	if it.has("x") and not it.has("pts"):
		_fnum(g, it, "x", "x (cm)", -100000, 100000, 10);_fnum(g, it, "y", "y (cm)", -100000, 100000, 10)

func _plen(it) -> float:
	var L: = 0.0
	for i in it.pts.size() - 1: L += Vector2(it.pts[i][0], it.pts[i][1]).distance_to(Vector2(it.pts[i + 1][0], it.pts[i + 1][1]))
	return L / PX_PER_M

func _set_field(it, k: String, val) -> void :
	_snap_undo(str(it.id) + ":" + k)
	if typeof(val) == TYPE_STRING and val == "" and k in ["color"]: it.erase(k)
	else: it[k] = val
	if k == "bind": m.hud.toast("Vazba na simulaci se plně projeví po „Uložit a načíst znovu“.")
	_changed()

func _ftext(g: GridContainer, it, k: String, title: String) -> void :
	_lab(g, title)
	var le: = LineEdit.new();le.custom_minimum_size = Vector2(170, 0)
	var cur = it.get(k, "")
	le.text = ",".join(PackedStringArray(cur.map(func(x): return str(int(x))))) if cur is Array else str(cur)
	le.text_submitted.connect(func(t: String):
		if k == "ports":
			var arr: = []
			for s in t.split(","):
				if s.strip_edges().is_valid_float(): arr.append(float(s))
			if arr.size() >= 2: _set_field(it, k, arr)
		else: _set_field(it, k, t)
		le.release_focus())
	le.focus_exited.connect(func():
		if k != "ports" and le.text != str(it.get(k, "")): _set_field(it, k, le.text))
	g.add_child(le)

func _fnum(g: GridContainer, it, k: String, title: String, mn: float, mx: float, step: float) -> void :
	_lab(g, title)
	var sp: = SpinBox.new();sp.min_value = mn;sp.max_value = mx;sp.step = step;sp.value = float(it.get(k, 0));sp.custom_minimum_size = Vector2(110, 0)
	sp.set_meta("key", k)
	sp.value_changed.connect(func(v: float): _set_field(it, k, v))
	g.add_child(sp)

func _fenum(g: GridContainer, it, k: String, title: String, opts: Array) -> void :
	_lab(g, title)
	var ob: = OptionButton.new();ob.focus_mode = Control.FOCUS_NONE
	var cur = it.get(k, opts[0][0])
	if cur == null: cur = ""
	var idx: = -1
	for i in opts.size():
		ob.add_item(str(opts[i][1]))
		if str(opts[i][0]) == str(cur) or (typeof(cur) == TYPE_FLOAT and typeof(opts[i][0]) == TYPE_INT and int(cur) == opts[i][0]): idx = i
	if idx < 0:
		ob.add_item("%s (vlastní)" % str(cur));idx = opts.size()
	ob.select(idx)
	ob.item_selected.connect(func(i: int):
		if i < opts.size(): _set_field(it, k, opts[i][0]))
	g.add_child(ob)

func _fbool(g: GridContainer, it, k: String, title: String) -> void :
	_lab(g, title)
	var cb: = CheckBox.new();cb.button_pressed = bool(it.get(k, false));cb.focus_mode = Control.FOCUS_NONE
	cb.toggled.connect(func(v: bool): _set_field(it, k, v))
	g.add_child(cb)

# ---------- kreslení ----------
var last_z: = 0.0
func _process(_d: float) -> void :
	# čáry a úchyty mají stálou velikost na obrazovce → při zoomu překreslit
	if on and m.cam != null and absf(m.cam.zoom.x - last_z) > 1e-4:
		last_z = m.cam.zoom.x;queue_redraw()

func _draw() -> void :
	if not on: return
	var z: = _z(); var hs: = 4.0 / z
	if z > 0.15:
		for p in _pipes(true):
			for q in p.pts: draw_rect(Rect2(Vector2(q[0], q[1]) - Vector2(hs, hs), Vector2(hs, hs) * 2.0), Color(0.35, 0.75, 1.0, 0.8), false, 1.2 / z)
		for q in _ports(): draw_arc(q, 6.0 / z, 0, TAU, 16, Color(0.3, 1.0, 0.6, 0.7), 1.5 / z)
		for j in m.plan.get("joints", []): draw_circle(Vector2(j[0], j[1]), 5.0 / z, Color(1, 0.8, 0.2, 0.85))
	var col: = Color("f2c230")
	for it in multi: _draw_sel(it, Color(0.3, 0.8, 1.0), z)
	if sel != null:
		if sel.t in ["pipe", "pt"]:
			_draw_sel(sel.it, col, z)
			if sel.t == "pt":
				var pp: = Vector2(sel.it.pts[sel.i][0], sel.it.pts[sel.i][1])
				draw_circle(pp, 8.0 / z, col)
		else:
			_draw_sel(sel.it, col, z)
			if sel.it.type in ["vessel", "panel"] and sel.it.has("w"):
				var e: = _vrect(sel.it).end
				draw_rect(Rect2(e - Vector2(7, 7) / z, Vector2(14, 14) / z), col)
	for pl in route_hl:
		draw_polyline(pl, Color(1, 0.85, 0.1, 0.45), 22.0, true);draw_polyline(pl, Color(1, 0.9, 0.2), 4.0, true)
	if drag and drag_mode == "box":
		var r: = Rect2(box_a, Vector2.ZERO).expand(hover_w)
		draw_rect(r, Color(0.3, 0.7, 1.0, 0.12));draw_rect(r, Color(0.4, 0.8, 1.0, 0.9), false, 1.5 / z)
	var cross: = Color("4fd18b")
	match tool:
		"pipe":
			var pl2: = PackedVector2Array()
			for d in draw_pts: pl2.append(d.p)
			var sp: Dictionary = _snap_pipe_pt(hover_w, Input.is_key_pressed(KEY_ALT))
			var hp: Vector2 = sp.p
			if draw_pts.size() and sp.kind == "free" and not Input.is_key_pressed(KEY_ALT):
				var last: Vector2 = draw_pts[draw_pts.size() - 1].p
				if absf(hp.x - last.x) > absf(hp.y - last.y): hp.y = last.y
				else: hp.x = last.x
			pl2.append(hp)
			if pl2.size() >= 2:
				draw_polyline(pl2, Color(0.4, 0.8, 1.0, 0.35), (clampf(dn_new * 0.24, 3.0, 6.0) if dn_new <= 32 else 12.0 * dn_new / 50.0) + 6.0 / z, true)
				draw_polyline(pl2, Color(0.6, 0.9, 1.0), 2.0 / z, true)
			for d in draw_pts: draw_circle(d.p, 5.0 / z, Color(0.6, 0.9, 1.0))
			_crosshair(hp, cross if sp.kind != "free" else Color(1, 1, 1, 0.9), z)
		"hose", "pitch":
			var sp2: Dictionary = _snap_pipe_pt(hover_w, false)
			var a = pitch_a if tool == "pitch" else (draw_pts[0].p if draw_pts.size() else null)
			if a != null:
				var hp2: Array = _hose_pts(a, sp2.p); var pl3: = PackedVector2Array()
				for q in hp2: pl3.append(Vector2(q[0], q[1]))
				draw_polyline(pl3, Color(0.85, 0.63, 0.23, 0.8), 8.0, true)
			_crosshair(sp2.p, cross if sp2.kind != "free" else Color(1, 1, 1, 0.9), z)
		"measure":
			var p3: Vector2 = _snap_pipe_pt(hover_w, false).p
			if meas_a != null:
				draw_line(meas_a, p3, Color(1, 1, 1, 0.9), 2.0 / z)
				draw_string(m.font_b, p3 + Vector2(12, -12) / z, "%.2f m" % (p3.distance_to(meas_a) / PX_PER_M), HORIZONTAL_ALIGNMENT_LEFT, -1, int(16.0 / z), Color.WHITE)
			_crosshair(p3, Color.WHITE, z)
		"route":
			if route_a != null: draw_rect(_vrect(route_a).grow(8.0 / z), Color(1, 0.85, 0.1), false, 3.0 / z)
			var hv = _vessel_at(hover_w)
			if hv != null: draw_rect(_vrect(hv).grow(4.0 / z), Color(1, 0.85, 0.1, 0.5), false, 2.0 / z)
		"paint":
			var ns2 = _near_seg(hover_w, 12.0 / z, true)
			if ns2 != null:
				var plc: = PackedVector2Array()
				for q in ns2.it.pts: plc.append(Vector2(q[0], q[1]))
				draw_polyline(plc, Color(paint_col) if paint_col != "" else Color(1, 1, 1, 0.5), 6.0 / z, true)
		"joint":
			_crosshair(hover_w, Color("f2c230"), z)
		"part":
			var pd: Array = _part_def(part_key)
			if pd[2] in ON_PIPE:
				var ns = _near_seg(hover_w, 16.0 / z)
				if ns != null: draw_circle(ns.q, 12.0 / z, Color(0.3, 1.0, 0.5, 0.6))
				else: _crosshair(hover_w, Color(1, 0.4, 0.3, 0.9), z)
			elif pd[2] == "vessel":
				var dd: Dictionary = pd[3]
				var vw: = float(dd.w); var vh: = float(dd.h)
				draw_rect(Rect2(_snap(hover_w - Vector2(vw, vh) * 0.5), Vector2(vw, vh)), Color(0.3, 1.0, 0.6, 0.6), false, 2.0 / z)
			else: _crosshair(_snap_pipe_pt(hover_w, false).p, cross, z)

func _draw_sel(it, col: Color, z: float) -> void :
	if it.has("pts"):
		var pl: = PackedVector2Array()
		for q in it.pts: pl.append(Vector2(q[0], q[1]))
		if pl.size() >= 2: draw_polyline(pl, Color(col, 0.35), 18.0 / z, true);draw_polyline(pl, col, 2.0 / z, true)
	elif it.type in ["vessel", "panel"] and it.has("w"): draw_rect(_vrect(it).grow(6.0 / z), col, false, 3.0 / z)
	elif it.type == "text": draw_rect(Rect2(float(it.x) - 4, float(it.y) - 18, 8.0 * str(it.get("label", "")).length() + 8, 24), col, false, 2.0 / z)
	elif it.has("x"): draw_arc(Vector2(it.x, it.y), 26.0 / z, 0, TAU, 32, col, 3.0 / z, true)

func _crosshair(p: Vector2, col: Color, z: float) -> void :
	var r: = 12.0 / z
	draw_circle(p, 6.0 / z, col)
	draw_line(p - Vector2(r * 2.0, 0), p + Vector2(r * 2.0, 0), Color(col, 0.6), 1.5 / z)
	draw_line(p - Vector2(0, r * 2.0), p + Vector2(0, r * 2.0), Color(col, 0.6), 1.5 / z)

## samotest (spouští --edtest)
func selftest() -> void :
	var ok: = true
	if m.cam != null: m.cam.zoom = Vector2.ONE
	var n0: int = m.items.size(); var e0: int = m.sim.net.edges.size(); var j0: int = m.plan.get("joints", []).size()
	var ps: Array = _pipes(); var p0 = ps[0]
	var a: = Vector2(p0.pts[0][0], p0.pts[0][1])
	draw_pts = [{"p": a, "kind": "pt"}, {"p": a + Vector2(0, 200), "kind": "free"}, {"p": a + Vector2(300, 200), "kind": "free"}]
	_finish_pipe()
	var e1: int = m.sim.net.edges.size()
	print("EDTEST trubka: položek %d→%d, hran %d→%d" % [n0, m.items.size(), e0, e1]);ok = ok and m.items.size() == n0 + 1 and e1 > e0
	part_key = "klapka";_add_part(a + Vector2(0, 100))
	var v = m.items[m.items.size() - 1]
	var onnet: bool = m.sim.net.byId.has(v.id)
	print("EDTEST klapka na síti: %s, rot %d" % [onnet, int(v.rot)]);ok = ok and onnet
	sel = {"t": "item", "it": v};_rotate()
	print("EDTEST otočení: rot %d" % int(v.rot))
	for k in ["pruh", "flow", "fitT", "text", "tank", "spray", "hopf"]:
		part_key = k;var nb: int = m.items.size();_add_part(a + Vector2(150, 200))
		print("EDTEST díl %s: %s" % [k, "OK" if m.items.size() == nb + 1 else "CHYBA"]);ok = ok and m.items.size() == nb + 1
	part_key = "pump";var npb: int = m.items.size();_add_part(a + Vector2(150, 200))
	print("EDTEST čerpadlo na trubce: %s" % ("OK" if m.items.size() == npb + 1 else "CHYBA"));ok = ok and m.items.size() == npb + 1
	var tk = null
	for it in m.items:
		if it.type == "vessel" and str(it.get("label", "")) == "nádoba": tk = it
	sel = {"t": "item", "it": tk};_set_field(tk, "w", 300.0);_set_field(tk, "shape", "cone")
	print("EDTEST vlastnosti nádoby: w %d tvar %s" % [int(tk.w), tk.shape]);ok = ok and int(tk.w) == 300 and tk.shape == "cone"
	var nh: int = m.items.size()
	_pitch_click(a + Vector2(600, 0));_pitch_click(a + Vector2(1100, 300))
	print("EDTEST spílací hadice: +%d dílů (má být 7)" % (m.items.size() - nh));ok = ok and m.items.size() - nh == 7
	multi = m.items.slice(nh, nh + 7);sel = null
	var bb0: = _bbox(multi);_move_items(multi, Vector2(100, 0));_rotate_group()
	print("EDTEST skupina: posun + otočení OK, rámeček %s" % str(_bbox(multi).size));
	var nd: int = m.items.size();_duplicate()
	print("EDTEST duplikace skupiny: +%d (má být 7)" % (m.items.size() - nd));ok = ok and m.items.size() == nd + 7
	_delete()
	print("EDTEST smazání skupiny: %d (má být %d)" % [m.items.size(), nd]);ok = ok and m.items.size() == nd
	draw_pts = [{"p": a + Vector2(-100, 100), "kind": "free"}, {"p": a + Vector2(100, 100), "kind": "free"}];_finish_pipe()
	_joint_click(a + Vector2(0, 100))
	print("EDTEST spoj na křížení: spojů %d → %d" % [j0, m.plan.joints.size()]);ok = ok and m.plan.joints.size() == j0 + 1
	# smazání nádoby s vazbou: napoprvé jen varuje, napodruhé smaže, zpět ji vrátí i s živým zobrazením
	var rvp = null
	for it in m.items:
		if it.get("bind", "") == "rvp": rvp = it
	var nr: int = m.items.size();sel = {"t": "item", "it": rvp};multi.clear();_delete()
	var warned: bool = m.items.size() == nr
	_delete()
	var gone: bool = not m.items.has(rvp)
	undo()
	print("EDTEST mazání pánve: varování %s, smazáno %s, zpět vráceno %s" % [warned, gone, m.items.has(rvp)]);ok = ok and warned and gone and m.items.has(rvp)
	_route_click(_vrect(rvp).get_center())
	var sk = null
	for it in m.items:
		if it.get("bind", "") == "sk": sk = it
	_route_click(_vrect(sk).get_center())
	print("EDTEST cesta pánev → SK: %d úseků" % route_hl.size());ok = ok and route_hl.size() > 0
	sel = null;multi.clear()
	while undo_stack.size(): undo()
	print("EDTEST zpět: položek %d (má být %d), hran %d (má být %d)" % [m.items.size(), n0, m.sim.net.edges.size(), e0]);ok = ok and m.items.size() == n0 and m.sim.net.edges.size() == e0
	print("EDTEST " + ("OK" if ok else "CHYBA"))

## test kliky myší přes celý vstup aplikace (--edclick)
func clicktest() -> void :
	m.cam.zoom = Vector2.ONE;m.cam.position = Vector2(700, 900)
	await get_tree().process_frame
	var vp: Vector2 = m.vp_size()
	var to_s = func(w: Vector2) -> Vector2: return (w - m.cam.get_screen_center_position()) * m.cam.zoom.x + vp * 0.5
	var n0: int = m.items.size()
	_set_tool("pipe")
	for w in [Vector2(420, 1060), Vector2(420, 1160), Vector2(620, 1160)]:
		for pr in [true, false]:
			var ev: = InputEventMouseButton.new();ev.button_index = MOUSE_BUTTON_LEFT;ev.pressed = pr;ev.position = to_s.call(w);ev.global_position = ev.position
			get_viewport().push_input(ev)
			await get_tree().process_frame
	var ev2: = InputEventMouseButton.new();ev2.button_index = MOUSE_BUTTON_RIGHT;ev2.pressed = true;ev2.position = to_s.call(Vector2(620, 1160))
	get_viewport().push_input(ev2)
	await get_tree().process_frame
	var last = m.items[m.items.size() - 1]
	print("EDCLICK trubka myší: položek %d→%d, body %s" % [n0, m.items.size(), str(last.get("pts", []))])
	print("EDCLICK " + ("OK" if m.items.size() == n0 + 1 and last.type == "pipe" else "CHYBA"))
