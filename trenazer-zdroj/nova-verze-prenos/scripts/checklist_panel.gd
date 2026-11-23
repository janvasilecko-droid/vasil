class_name ChecklistPanel
extends PanelContainer
## DOČASNÝ checklist varny: obsluha jde bod za bodem, u každého kroku nastaví v aplikaci klapky / čerpadla
## (nebo cestu nahranou v Editoru cest), napíše popis a uloží stav. Ukládá se do checklist_uceni.json ve složce
## projektu – z něj se pak dělá kontrola kroku. Než je krok uložený, ukazuje panel návrh postupu (ChecklistNavrh)
## a živou kontrolu podle něj; tlačítkem Auto se várka podle návrhu projede sama.

const FILE := "user://checklist_uceni.json"
const LEGACY_FILE := "res://checklist_uceni.json"
const STEPS := [
	["Příprava", "CHECK KLAPEK, vše zavřeno", "Ledovka: aspoň půl tanku, cirkulace přes glykol", "Pustit vzduch, otevřít odvzdušnění u šoupěte", "Najet páru"],
	["Napouštění a vystírka", "Napustit vodu z bojleru a studenou (35–40 °C)", "Napouštět „do výpustě“ asi 20 hl", "Pustit míchadlo kotle", "Otevřít šoupě na slad (voda do šneku na čtvrtku)", "Check, vše ostatní zavřeno", "Pustit slad", "Zavřít slad a šoupě (ať nejde pára do šneku)", "Osprchovat kotel horkou vodou", "Vystírka 40 °C", "Zahřívat na 55 °C (bílkovinná prodleva)"],
	["Příprava scezovačky", "Připravit scezovačku: vypustit, odrazit mláto", "Čerpadlo bojleru 6–7, prohřát trubky", "Podrazit: voda do věnce a pod síta", "Zavřít, vypustit, sprcha, počkat na čistou vodu", "Zaplavit čerpadlo, napustit nad síta", "Kopyto nahoru, kopačka dolů"],
	["1. a 2. rmut", "Převod díla do SK, v kotli zůstane 1. rmut asi 10 hl", "Osprchovat kroužek a kotel", "1. rmut: 72 °C / 10 min", "1. rmut: var 10 min", "Vracení 1. rmutu, ideálně 67 °C po návratu", "2. rmut: 72 °C / 10 min, pak var", "Vracení 2. rmutu", "Spláchnout mláto z kopačky, protlačit trubky a pod síta", "20 min klid"],
	["Podrazit a filtrace", "Pustit vyslazovací čerpadlo, cestu jen „natrhnout“", "Míchat brzy (1–1,5 ot.), kopačkou nahoru a dolů", "Když je vidět hladina, pustit sprchu (78 °C)", "Asi 30 hl zastavit (u tyčky)", "Dopustit po hranu (předkvasná mladina)"],
	["Chmelovar", "Var 90 min, chmel podle rozpisu"],
	["Whirlpool, chlazení a spílání", "Propláchnout trubky k whirlpoolu", "Přečerpat do whirlpoolu", "Hadice na vratku, vymýt filtr", "Studená voda / ledovka připravená k chladiči", "Za filtrem ledový: hadici na tank", "Pustit ledovku přes čerpadlo do chladiče, výstup do bojleru", "Spílat, zakvašovací teplota 8,5 °C", "Protláčka do tanku, zavíčkovat"],
]
const STATE_TYPES := ["deco", "sw", "inl", "gauge", "dip", "spray"]  # další prvky s přepínačem (šoupě, vypínač, kohouty…)
const AUTO_SPEED := 180.0  # rychlost času při automatickém projetí várky
const C_OK := "#7CE08A"
const C_BAD := "#FF7A6B"
const C_INFO := "#E8C25A"

var main: Node
var plan: PlanData
var data: Dictionary = {}
var cur := 0
var list_box: VBoxContainer
var _scroll: ScrollContainer
var note: TextEdit
var info: Label
var title: Label
var progress: Label
var route_ob: OptionButton
var navrh_lbl: Label
var chk: RichTextLabel
var btn_auto: Button
var btn_rucne: Button
var flat: Array = []
var _rows: Array = []
var navrh: ChecklistNavrh
var auto := false  # várka se projíždí sama podle návrhu
var auto_log: Array = []  # [krok, čas simulace v s, stav nádob] po každém splněném kroku
var _last_clock := -1.0
var _ui_acc := 0.0
var _overlay: Node3D
var _overlay_key := ""


func setup(m: Node, p: PlanData) -> void:
	main = m
	plan = p
	for g in STEPS:
		for i in range(1, g.size()):
			flat.append({"grp": g[0], "t": g[i]})
	var sb := StyleBoxFlat.new()
	sb.bg_color = Color(0.08, 0.09, 0.11, 0.96)
	sb.border_color = Color(0.4, 0.75, 1.0, 0.7)
	sb.set_border_width_all(2)
	sb.set_corner_radius_all(8)
	sb.set_content_margin_all(10)
	add_theme_stylebox_override("panel", sb)
	# vpravo, přes celou výšku pod lištami – seznam kroků se roluje, editace kroku je vždy vidět dole
	anchor_left = 1.0
	anchor_right = 1.0
	anchor_top = 0.0
	anchor_bottom = 1.0
	offset_left = -470.0
	offset_right = -10.0
	offset_top = 96.0
	offset_bottom = -12.0
	var outer := VBoxContainer.new()
	outer.add_theme_constant_override("separation", 5)
	add_child(outer)
	var head := HBoxContainer.new()
	var t := Label.new()
	t.text = "CHECKLIST VARNY (dočasný, učení)"
	t.add_theme_color_override("font_color", Color(0.95, 0.8, 0.25))
	t.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	head.add_child(t)
	progress = Label.new()
	progress.add_theme_font_size_override("font_size", 12)
	progress.modulate = Color(0.75, 0.85, 0.95)
	head.add_child(progress)
	var cl := Button.new()
	cl.text = "✕"
	cl.focus_mode = Control.FOCUS_NONE
	cl.pressed.connect(func() -> void: visible = false)
	head.add_child(cl)
	outer.add_child(head)
	_scroll = ScrollContainer.new()
	_scroll.custom_minimum_size = Vector2(0, 120)
	_scroll.size_flags_vertical = Control.SIZE_EXPAND_FILL
	_scroll.horizontal_scroll_mode = ScrollContainer.SCROLL_MODE_DISABLED
	list_box = VBoxContainer.new()
	list_box.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	list_box.add_theme_constant_override("separation", 0)
	_scroll.add_child(list_box)
	outer.add_child(_scroll)
	outer.add_child(HSeparator.new())
	title = Label.new()
	title.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	title.add_theme_color_override("font_color", Color(0.6, 0.85, 1.0))
	outer.add_child(title)
	navrh_lbl = Label.new()
	navrh_lbl.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	navrh_lbl.custom_minimum_size = Vector2(440, 0)
	navrh_lbl.add_theme_font_size_override("font_size", 12)
	navrh_lbl.add_theme_color_override("font_color", Color(0.78, 0.84, 0.9))
	outer.add_child(navrh_lbl)
	chk = RichTextLabel.new()
	chk.bbcode_enabled = true
	chk.fit_content = true
	chk.scroll_active = false
	chk.custom_minimum_size = Vector2(440, 0)
	chk.add_theme_font_size_override("normal_font_size", 12)
	chk.add_theme_font_size_override("bold_font_size", 13)
	chk.mouse_filter = Control.MOUSE_FILTER_IGNORE
	outer.add_child(chk)
	var arow := HBoxContainer.new()
	arow.add_theme_constant_override("separation", 6)
	var bnv := Button.new()
	bnv.text = "Nastavit návrh"
	bnv.tooltip_text = "Nastaví klapky, čerpadla, šnek, míchadlo a ohřev tak, jak mají podle návrhu být na začátku kroku"
	bnv.focus_mode = Control.FOCUS_NONE
	bnv.pressed.connect(_apply_navrh)
	arow.add_child(bnv)
	btn_rucne = Button.new()
	btn_rucne.text = "Hotovo (ručně)"
	btn_rucne.tooltip_text = "Ruční krok bez měření (kypřidlo, hadice, filtr…): potvrdí, že je hotový"
	btn_rucne.focus_mode = Control.FOCUS_NONE
	btn_rucne.pressed.connect(func() -> void:
		navrh.manual_ok = true
		_refresh_check())
	arow.add_child(btn_rucne)
	outer.add_child(arow)
	var brow := HBoxContainer.new()
	brow.add_theme_constant_override("separation", 6)
	btn_auto = Button.new()
	btn_auto.text = "▶ Auto od tohoto kroku"
	btn_auto.tooltip_text = "Várka pokračuje sama podle návrhu od vybraného kroku (×%d, rychlost jde měnit klávesami 1–4)" % int(AUTO_SPEED)
	btn_auto.focus_mode = Control.FOCUS_NONE
	btn_auto.pressed.connect(func() -> void:
		if auto:
			_stop_auto(false)
		else:
			_start_auto(cur, false))
	brow.add_child(btn_auto)
	var bnew := Button.new()
	bnew.text = "▶▶ Nová várka automaticky"
	bnew.tooltip_text = "Začne novou várku a projede všech %d kroků podle návrhu" % flat.size()
	bnew.focus_mode = Control.FOCUS_NONE
	bnew.pressed.connect(func() -> void: _start_auto(0, true))
	brow.add_child(bnew)
	outer.add_child(brow)
	outer.add_child(HSeparator.new())
	var rrow := HBoxContainer.new()
	var rl := Label.new()
	rl.text = "Cesta kroku:"
	rrow.add_child(rl)
	route_ob = OptionButton.new()
	route_ob.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	route_ob.focus_mode = Control.FOCUS_NONE
	route_ob.tooltip_text = "Cesta nahraná v Editoru cest, která k tomuto kroku patří (nepovinné)"
	rrow.add_child(route_ob)
	outer.add_child(rrow)
	note = TextEdit.new()
	note.custom_minimum_size = Vector2(0, 84)
	note.wrap_mode = TextEdit.LINE_WRAPPING_BOUNDARY
	note.placeholder_text = "Tvůj popis kroku: kudy teče, které klapky a čerpadla, teploty, časy, kdy je krok splněný a co se má hlídat… (Esc = konec psaní)"
	outer.add_child(note)
	info = Label.new()
	info.add_theme_font_size_override("font_size", 12)
	info.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	info.custom_minimum_size = Vector2(440, 0)
	outer.add_child(info)
	var row := HBoxContainer.new()
	row.add_theme_constant_override("separation", 6)
	var b := Button.new()
	b.text = "Uložit stav"
	b.tooltip_text = "Uloží popis, cestu a aktuální stav klapek, čerpadel a nádob k tomuto kroku"
	b.focus_mode = Control.FOCUS_NONE
	b.pressed.connect(_save.bind(false))
	row.add_child(b)
	var bn := Button.new()
	bn.text = "Uložit a další →"
	bn.focus_mode = Control.FOCUS_NONE
	bn.pressed.connect(_save.bind(true))
	row.add_child(bn)
	var br := Button.new()
	br.text = "Obnovit uložený stav"
	br.tooltip_text = "Nastaví klapky a čerpadla tak, jak byly uložené u tohoto kroku"
	br.focus_mode = Control.FOCUS_NONE
	br.pressed.connect(_restore)
	row.add_child(br)
	outer.add_child(row)
	var nav := HBoxContainer.new()
	var bp := Button.new()
	bp.text = "← předchozí"
	bp.focus_mode = Control.FOCUS_NONE
	bp.pressed.connect(func() -> void: _pick(maxi(cur - 1, 0)))
	nav.add_child(bp)
	var bx := Button.new()
	bx.text = "další →"
	bx.focus_mode = Control.FOCUS_NONE
	bx.pressed.connect(func() -> void: _pick(mini(cur + 1, flat.size() - 1)))
	nav.add_child(bx)
	outer.add_child(nav)
	var sp = _sp()
	if sp != null:
		navrh = ChecklistNavrh.new(sp, plan)
	visible = false
	visibility_changed.connect(func() -> void:
		if visible:
			_refresh_routes()
			_select(cur)
		else:
			_update_overlay([], []))
	_load()
	_rebuild()
	_select(0)


func _sp() -> Variant:
	return main.get("sim_panel")


func _rebuild() -> void:
	for c in list_box.get_children():
		list_box.remove_child(c)  # hned pryč z rozložení, jinak by posun seznamu počítal se starými řádky
		c.queue_free()
	_rows.clear()
	var idx := 0
	var last := ""
	for s in flat:
		if s.grp != last:
			last = s.grp
			var h := Label.new()
			h.text = str(s.grp).to_upper()
			h.add_theme_color_override("font_color", Color(0.95, 0.8, 0.25))
			h.add_theme_font_size_override("font_size", 12)
			list_box.add_child(h)
		var b := Button.new()
		b.alignment = HORIZONTAL_ALIGNMENT_LEFT
		b.flat = true
		b.focus_mode = Control.FOCUS_NONE
		b.clip_text = true
		b.text = ("✔ " if data.has(str(idx + 1)) else "○ ") + "%d. %s" % [idx + 1, s.t]
		b.pressed.connect(_pick.bind(idx))
		list_box.add_child(b)
		_rows.append(b)
		idx += 1
	progress.text = "uloženo %d / %d  " % [data.size(), flat.size()]


func _refresh_routes() -> void:
	route_ob.clear()
	route_ob.add_item("—  (bez cesty)")
	route_ob.set_item_metadata(0, "")
	var routes: Dictionary = plan.raw.get("routes", {})
	var keys: Array = routes.keys()
	keys.sort()
	for k in keys:
		var r: Dictionary = routes[k]
		var label := str(r.get("name", r.get("label", k)))
		route_ob.add_item(label if label == str(k) else "%s  (%s)" % [label, k])
		route_ob.set_item_metadata(route_ob.item_count - 1, str(k))


func _select_route(key: String) -> void:
	for i in route_ob.item_count:
		if str(route_ob.get_item_metadata(i)) == key:
			route_ob.select(i)
			return
	route_ob.select(0)


## výběr kroku obsluhou (seznam, předchozí / další): automatické projetí se tím zastaví
func _pick(i: int) -> void:
	if auto:
		_stop_auto(false)
	_select(i)


func _select(i: int) -> void:
	cur = clampi(i, 0, flat.size() - 1)
	for r in _rows.size():
		(_rows[r] as Button).modulate = Color(1.0, 0.85, 0.3) if r == cur else Color(1, 1, 1)
	title.text = "Krok %d / %d: %s" % [cur + 1, flat.size(), flat[cur].t]
	var d: Dictionary = data.get(str(cur + 1), {})
	note.text = str(d.get("popis", ""))
	if route_ob.item_count == 0:
		_refresh_routes()
	_select_route(str(d.get("cesta", "")))
	if navrh != null:
		if not auto and navrh.step != cur:
			navrh.begin(cur)
		navrh_lbl.text = ("Návrh (zatím jen můj odhad – oprav ho svým popisem): " if not d.has("stav") else "Původní návrh: ") + navrh.text(cur)
		btn_rucne.disabled = not navrh.is_manual(cur)
	if not auto:
		info.text = ("Uloženo: " + _sum(d.get("stav", {}))) if d.has("stav") else "Zatím neuloženo. Nastav klapky a čerpadla (nebo vyber cestu z Editoru cest, nebo „Nastavit návrh“), napiš popis a ulož."
	_refresh_check()
	_scroll_to_cur.call_deferred()


## posune seznam na vybraný krok (až po přestavbě seznamu, řádek už musí být v seznamu)
func _scroll_to_cur() -> void:
	if cur < _rows.size() and is_instance_valid(_rows[cur]) and _scroll.is_ancestor_of(_rows[cur]):
		_scroll.ensure_control_visible(_rows[cur])


func _sum(st: Dictionary) -> String:
	var pumps: Array = []
	for pid in st.get("pumps_on", []):
		pumps.append(str(plan.get_item(str(pid)).get("label", pid)))
	var parts: Array = ["%d otevřených klapek" % int(st.get("open", []).size()), "čerpadla: " + (", ".join(pumps) if not pumps.is_empty() else "stojí")]
	if not st.get("flowing", []).is_empty():
		parts.append("teče: " + ", ".join(st.get("flowing", [])))
	var sim: Dictionary = st.get("sim", {})
	if sim.has("rvp"):
		parts.append("pánev %.0f L %.1f °C" % [float(sim["rvp"].get("L", 0.0)), float(sim["rvp"].get("T", 0.0))])
	return " · ".join(parts)


func _snapshot() -> Dictionary:
	var open_v: Array = []
	var pumps_on: Array = []
	var other: Dictionary = {}
	for it in plan.items:
		var t := str(it.get("type"))
		if t == "valve" and bool(it.get("open", false)):
			open_v.append(str(it.id))
		elif t == "pump" and bool(it.get("on", false)):
			pumps_on.append(str(it.id))
		elif t in STATE_TYPES:
			for k in ["open", "on", "closed"]:
				if it.has(k):
					other[str(it.id) + "." + k] = it[k]
	var st := {"open": open_v, "pumps_on": pumps_on, "other": other}
	var sp = _sp()
	if sp != null:
		sp._update_flows()  # které cesty teď opravdu tečou (podle klapek a čerpadel v tomto okamžiku)
		var sim_state := {}
		for k in sp.sim.vessels:
			var vs: Dictionary = sp.sim.vessels[k]
			sim_state[k] = {"L": snappedf(float(vs["v"]), 0.1), "T": snappedf(float(vs["t"]), 0.1), "slad_kg": snappedf(float(vs.get("malt", 0.0)), 0.1)}
		st["sim"] = sim_state
		var fl: Array = []
		for rn in sp._flowing:
			if bool(sp._flowing[rn]):
				fl.append(str(rn))
		st["flowing"] = fl
		st["ohrev_kw"] = sp.sim.heater_kw.duplicate()
		st["cil_C"] = sp.sim.setpoint.duplicate()
		st["auto"] = sp.sim.auto_mode.duplicate()
		st["michadlo"] = bool(sp.sim.stir.get("rvp", false))
		st["cas_min"] = int(float(sp.sim.clock) / 60.0)
	return st


func _save(advance: bool) -> void:
	var key := str(cur + 1)
	var cesta: String = str(route_ob.get_item_metadata(route_ob.selected)) if route_ob.selected >= 0 else ""
	data[key] = {"krok": flat[cur].t, "skupina": flat[cur].grp, "popis": note.text, "cesta": cesta, "stav": _snapshot()}
	var f := FileAccess.open(FILE, FileAccess.WRITE)
	if f == null:
		info.text = "Nepovedlo se zapsat %s (%s)" % [FILE, error_string(FileAccess.get_open_error())]
		return
	f.store_string(JSON.stringify(data, "\t"))
	f.close()
	note.release_focus()
	_rebuild()
	var saved := cur
	if advance and cur < flat.size() - 1:
		_select(cur + 1)
		info.text = "Krok %d uložen (%s). Pokračuj krokem %d." % [saved + 1, _sum(data[key].stav), cur + 1]
	else:
		_select(cur)
		info.text = "Uloženo: krok %d (%s)" % [saved + 1, _sum(data[key].stav)]


## nastaví klapky a čerpadla podle uloženého stavu kroku (ostatní klapky zavře)
func _restore() -> void:
	var d: Dictionary = data.get(str(cur + 1), {})
	if not d.has("stav"):
		info.text = "Krok %d zatím nemá uložený stav." % (cur + 1)
		return
	var st: Dictionary = d["stav"]
	var sp = _sp()
	var open_v: Array = st.get("open", [])
	var pumps_on: Array = st.get("pumps_on", [])
	var other: Dictionary = st.get("other", {})
	for it in plan.items:
		var t := str(it.get("type"))
		var want := false
		var key := ""
		if t == "valve":
			key = "open"
			want = open_v.has(str(it.id))
		elif t == "pump":
			key = "on"
			want = pumps_on.has(str(it.id))
		else:
			for k in ["open", "on", "closed"]:
				var ok: String = str(it.id) + "." + str(k)
				if other.has(ok) and bool(it.get(k, false)) != bool(other[ok]):
					it[k] = bool(other[ok])
					main.plant.call("rebuild_item", str(it.id))
			continue
		if sp != null:
			sp._set_item(it, key, want)
		elif bool(it.get(key, false)) != want:
			it[key] = want
			main.plant.call("rebuild_item", str(it.id))
	info.text = "Obnoven stav kroku %d: %s" % [cur + 1, _sum(st)]


func _load() -> void:
	if not FileAccess.file_exists(FILE) and FileAccess.file_exists(LEGACY_FILE):
		var legacy := FileAccess.open(LEGACY_FILE, FileAccess.READ)
		if legacy == null:
			info.text = "Nepovedlo se přečíst starý checklist (%s)." % error_string(FileAccess.get_open_error())
			return
		var migrated := FileAccess.open(FILE, FileAccess.WRITE)
		if migrated == null:
			info.text = "Nepovedlo se přesunout checklist do uživatelských dat (%s)." % error_string(FileAccess.get_open_error())
			return
		migrated.store_string(legacy.get_as_text())
		migrated.close()
	if not FileAccess.file_exists(FILE):
		return
	var f := FileAccess.open(FILE, FileAccess.READ)
	if f == null:
		info.text = "Nepovedlo se přečíst checklist (%s)." % error_string(FileAccess.get_open_error())
		return
	var d: Variant = JSON.parse_string(f.get_as_text())
	if d is Dictionary:
		data = d
	else:
		info.text = "Checklist není platný JSON objekt."


# ---------- návrh, kontrola a automatické projetí ----------
func _apply_navrh() -> void:
	if navrh == null:
		return
	if auto:
		_stop_auto(false)
	navrh.apply_expected(cur)
	info.text = "Krok %d nastaven podle návrhu (klapky, čerpadla, šnek, míchadlo, ohřev). Stav nádob se nemění – ten vzniká průběhem várky." % (cur + 1)
	_refresh_check()


func _start_auto(from: int, new_batch: bool) -> void:
	var sp = _sp()
	if sp == null or navrh == null:
		return
	if new_batch:
		sp.new_batch()
		navrh.reset_controls()
		from = 0
	# FYZIKA+ oprava: automat spuštěný bez „Nové várky“ (tlačítko AUTOMATICKÁ VÁRKA po startu programu nebo pokračování
	# z uloženého stavu) měl prázdný bojler a stál na kroku 4. Před najetím páry na bojler ho proto napustí (4000 L, 80 °C).
	if from <= 3 and float(sp.sim.vessels["hlt"]["v"]) < 50.0:
		sp.sim.vessels["hlt"]["v"] = 4000.0
		sp.sim.vessels["hlt"]["t"] = 80.0
		sp._log("Bojler byl prázdný – napuštěn horkou vodou (4000 L, 80 °C).")
	auto = true
	sp.sim.ice_auto = true  # FYZIKA+: automat pouští chladičem jen tolik ledové vody, kolik je potřeba na zakvašovací teplotu
	auto_log.clear()
	_last_clock = float(sp.sim.clock)
	sp.set_speed(AUTO_SPEED)
	btn_auto.text = "■ Zastavit auto"
	note.release_focus()
	sp._log("Checklist: automatické projetí %s od kroku %d" % ["nové várky" if new_batch else "várky", from + 1])
	_enter_auto(from)


func _enter_auto(i: int) -> void:
	navrh.enter(i)
	_select(i)
	var sp = _sp()
	sp._log("Checklist %d: %s" % [i + 1, str(flat[i].t)])


func _stop_auto(done: bool) -> void:
	auto = false
	if _sp() != null:
		_sp().sim.ice_auto = false
	btn_auto.text = "▶ Auto od tohoto kroku"
	var sp = _sp()
	if done and sp != null:
		var s: BrewSim = sp.sim
		var spk: Dictionary = s.vessels["spilka"]
		var m: int = int(s.clock / 60.0)
		info.text = "Várka projetá podle návrhu za %d:%02d h: ve spilce %.0f L, %.1f °P, %.1f °C. Teď mi řekni, co v kterém kroku změnit." % [m / 60, m % 60, float(spk["v"]), s.plato("spilka"), float(spk["t"])]
		sp._log("Checklist: všech %d kroků projeto" % flat.size())
	elif sp != null:
		info.text = "Automatické projetí zastaveno u kroku %d." % (cur + 1)
		sp._log("Checklist: automatické projetí zastaveno u kroku %d" % (cur + 1))
	_refresh_check()


func _auto_next() -> void:
	var sp = _sp()
	var s: BrewSim = sp.sim
	navrh.finish()
	var v: Dictionary = s.vessels
	auto_log.append([cur + 1, s.clock, "kotel %.0f L %.1f °C · SK %.0f L %.1f °C · bojler %.0f L %.1f °C · vířivka %.0f L · spilka %.0f L %.1f °C · ledovka %.0f L %.1f °C" % [float(v["rvp"]["v"]) + float(v["rvp"]["malt"]) * BrewSim.MALT_L_PER_KG, float(v["rvp"]["t"]), float(v["sk"]["v"]), float(v["sk"]["t"]), float(v["hlt"]["v"]), float(v["hlt"]["t"]), float(v["wp"]["v"]), float(v["spilka"]["v"]), float(v["spilka"]["t"]), float(v["lv"]["v"]), float(v["lv"]["t"])]])
	sp._log("Checklist %d: ✔ splněno" % (cur + 1))
	if cur >= flat.size() - 1:
		_stop_auto(true)
		return
	_enter_auto(cur + 1)


func _process(delta: float) -> void:
	if navrh == null:
		return
	var sp = _sp()
	if sp == null:
		return
	var clock: float = float(sp.sim.clock)
	var dt: float = clock - _last_clock if _last_clock >= 0.0 else 0.0
	_last_clock = clock
	if dt > 0.0:  # záporné = nová várka
		navrh.update(dt)
		if auto:
			navrh.operate(dt)
	if auto and bool(navrh.check()["ok"]):
		_auto_next()
	_ui_acc += delta
	if _ui_acc >= 0.25 and visible:
		_ui_acc = 0.0
		_refresh_check()


func _fmt(r: Array, as_info: bool = false) -> String:
	var blocking: bool = bool(r[2]) and not as_info
	var mark: String = "[color=%s]✔[/color]" % C_OK if bool(r[0]) else ("[color=%s]✗[/color]" % C_BAD if blocking else "[color=%s]•[/color]" % C_INFO)
	return mark + " " + str(r[1]).replace("[", "(").replace("]", ")")


## živá kontrola kroku: podle uloženého stavu obsluhy, a dokud není uložený, podle návrhu
func _refresh_check() -> void:
	if navrh == null or chk == null:
		return
	var d: Dictionary = data.get(str(cur + 1), {})
	var lines: Array = []
	var ok := true
	var c: Dictionary = navrh.check()
	if d.has("stav") and not auto:
		var cmp: Dictionary = _compare_saved(d)
		lines.append("[b]Kontrola podle uloženého stavu:[/b]")
		for r in cmp["rows"]:
			lines.append(_fmt(r))
			ok = ok and bool(r[0])
		_update_overlay(cmp["open"], cmp["close"])
		lines.append("[color=#8fa3b8]Podle návrhu (jen pro informaci):[/color]")
		for r in c["rows"]:
			lines.append(_fmt(r, true))
	else:
		_update_overlay([], [])
		lines.append("[b]Kontrola podle návrhu:[/b]")
		for r in c["rows"]:
			lines.append(_fmt(r))
		ok = bool(c["ok"])
	lines.append("[b][color=%s]✔ KROK SPLNĚN[/color][/b]" % C_OK if ok else "[color=%s]… krok zatím není splněný[/color]" % C_INFO)
	chk.text = "\n".join(lines)
	if auto:
		var sp = _sp()
		var m: int = int(float(sp.sim.clock) / 60.0)
		info.text = "AUTO: krok %d / %d · čas várky %d:%02d · rychlost ×%d (klávesy 1–4, mezera = pauza)" % [cur + 1, flat.size(), m / 60, m % 60, int(sp.time_scale)]


## porovnání se stavem uloženým obsluhou: které klapky otevřít / zavřít, která čerpadla zapnout / vypnout
func _compare_saved(d: Dictionary) -> Dictionary:
	var st: Dictionary = d["stav"]
	var open_v: Array = st.get("open", [])
	var pumps_on: Array = st.get("pumps_on", [])
	var need_open: Array = []
	var need_close: Array = []
	var p_on: Array = []
	var p_off: Array = []
	for it in plan.items:
		var t := str(it.get("type"))
		if t == "valve":
			var w: bool = open_v.has(str(it.id))
			if bool(it.get("open", false)) != w:
				if w:
					need_open.append(str(it.id))
				else:
					need_close.append(str(it.id))
		elif t == "pump":
			var wp: bool = pumps_on.has(str(it.id))
			if bool(it.get("on", false)) != wp:
				if wp:
					p_on.append(str(it.get("label", it.id)))
				else:
					p_off.append(str(it.get("label", it.id)))
	var rows: Array = []
	var v_ok: bool = need_open.is_empty() and need_close.is_empty()
	rows.append([v_ok, "Klapky jako v uloženém stavu" if v_ok else "Klapky: otevřít %d (ve scéně zeleně), zavřít %d (červeně)" % [need_open.size(), need_close.size()], true])
	var p_ok: bool = p_on.is_empty() and p_off.is_empty()
	var ptxt: Array = []
	if not p_on.is_empty():
		ptxt.append("zapnout " + ", ".join(p_on))
	if not p_off.is_empty():
		ptxt.append("vypnout " + ", ".join(p_off))
	rows.append([p_ok, "Čerpadla jako v uloženém stavu" if p_ok else "Čerpadla: " + "; ".join(ptxt), true])
	var cesta := str(d.get("cesta", ""))
	var sp = _sp()
	if cesta != "" and sp != null:
		var f: bool = bool(sp._flowing.get(cesta, false))
		var lbl: String = str(sp.sim.routes.get(cesta, {}).get("label", cesta))
		rows.append([f, ("Teče cesta: " if f else "Neteče cesta: ") + lbl, true])
	return {"rows": rows, "open": need_open, "close": need_close}


## zvýraznění ve scéně: zelený čtvereček = klapku otevřít, červený = zavřít
func _update_overlay(need_open: Array, need_close: Array) -> void:
	var key := ",".join(need_open) + "|" + ",".join(need_close)
	if key == _overlay_key:
		return
	_overlay_key = key
	if _overlay != null and is_instance_valid(_overlay):
		_overlay.queue_free()
	_overlay = null
	if need_open.is_empty() and need_close.is_empty():
		return
	var plant: Node3D = main.plant
	_overlay = Node3D.new()
	_overlay.name = "ChecklistOverlay"
	plant.add_child(_overlay)
	for pair in [[need_open, Color(0.3, 1.0, 0.4)], [need_close, Color(1.0, 0.3, 0.25)]]:
		var mat := StandardMaterial3D.new()
		mat.shading_mode = BaseMaterial3D.SHADING_MODE_UNSHADED
		mat.albedo_color = pair[1]
		mat.no_depth_test = true
		mat.render_priority = 12
		for vid in pair[0]:
			var it: Dictionary = plan.get_item(str(vid))
			if it.is_empty():
				continue
			var mi := MeshInstance3D.new()
			var bm := BoxMesh.new()
			bm.size = Vector3(30, 30, 4)
			mi.mesh = bm
			mi.material_override = mat
			mi.position = plant.call("plan_to_3d", float(it.x), float(it.y), 55.0)
			_overlay.add_child(mi)


# ---------- testy (spouští main.gd v režimu --mode=chkauto / chkprobe) ----------
## celá várka podle návrhu bez vykreslování; uloženou várku obsluhy (brew_state.json) vrátí, jak byla
func run_auto_test() -> void:
	var sp = _sp()
	var had: bool = FileAccess.file_exists(SimPanel.SAVE_PATH)
	var bak: String = FileAccess.get_file_as_string(SimPanel.SAVE_PATH) if had else ""
	var stop_at := -1  # --chkstop=N: zastavit uprostřed kroku N (snímek rozpracované kontroly)
	var show_pk := false
	var demo := false  # --chkdemo: k zastavenému kroku jen v paměti „uložit“ stav obsluhy s rozdílem (porovnání a zvýraznění)
	for a in OS.get_cmdline_user_args():
		if a.begins_with("--chkstop="):
			stop_at = int(a.substr(10)) - 1
		elif a == "--chkdemo":
			demo = true
		elif a == "--chkprotokol":  # po projetí várky otevřít okno Varní protokol (snímek)
			show_pk = true
		elif a.begins_with("--chkroute="):  # jen ukázat trasu ve scéně (bez várky), např. --chkroute=dilo
			visible = false
			main.plant.call("show_route", a.substr(11))
			main.call("_focus", Vector2(700.0, 880.0), 760.0)
			return
		elif a.begins_with("--chkpatch="):  # body trubek z JSON {id: [[x,y],…], "_focus": [x,y,h]} jen v paměti (plán se nemění)
			var pd: Variant = JSON.parse_string(FileAccess.get_file_as_string(a.substr(11)))
			if pd is Dictionary:
				for pid in pd:
					if str(pid) != "_focus" and not plan.get_item(str(pid)).is_empty():
						plan.get_item(str(pid))["pts"] = pd[pid]
						main.plant.call("rebuild_item", str(pid))
				var fc: Array = (pd as Dictionary).get("_focus", [])
				if fc.size() == 3:
					main.call("_focus", Vector2(float(fc[0]), float(fc[1])), float(fc[2]))
			visible = false
			return
		elif a.begins_with("--chkflow="):  # rotametr jen v paměti (plán se nemění): --chkflow=x,y[,teče]
			var xy: PackedStringArray = a.substr(10).split(",")
			var fit := {"id": "flow_test", "type": "flow", "label": "rotametr", "rot": 270, "x": float(xy[0]), "y": float(xy[1])}
			plan.items.append(fit)
			main.plant.call("_build_item", fit)
			if xy.size() > 2:
				sp.set_process(false)  # simulace by průtok hned přepsala
				var fpid := str((main.plant.get("_flows") as Dictionary)["flow_test"]["pipe"])
				main.plant.call("set_pipe_fill", fpid, "water", 0.0)
				main.plant.call("set_pipe_flow", fpid, 1.0)
			visible = false
			main.call("_focus", Vector2(float(xy[0]), float(xy[1])), 230.0)
			return
	_start_auto(0, true)
	sp._save_acc = -1.0e12
	sp.time_scale = 30.0
	var n := 0
	var t0 := Time.get_ticks_msec()
	var step_clock: float = sp.sim.clock
	var last_step := cur
	while auto and n < 60000:
		sp._process(0.1)
		_process(0.1)
		n += 1
		if stop_at >= 0 and cur == stop_at and navrh.in_step >= 60.0:
			break
		if cur != last_step:
			last_step = cur
			step_clock = sp.sim.clock
		elif sp.sim.clock - step_clock > 5.0 * 3600.0:
			print("CHKAUTO ZASEKNUTO v kroku %d (%s):" % [cur + 1, flat[cur].t])
			for r in navrh.check()["rows"]:
				print("   ", "OK " if r[0] else "-- ", r[1])
			break
	for e in auto_log:
		var mm: int = int(float(e[1]) / 60.0)
		print("CHKAUTO %2d. %-52s %d:%02d  %s" % [int(e[0]), str(flat[int(e[0]) - 1].t).left(52), mm / 60, mm % 60, str(e[2])])
	var s: BrewSim = sp.sim
	print("CHKAUTO hotovo=%s kroků=%d iterací=%d za %d ms; spilka %.0f L %.1f °P %.1f °C kal %.0f %%" % [str(not auto and auto_log.size() == flat.size()), auto_log.size(), n, Time.get_ticks_msec() - t0, float(s.vessels["spilka"]["v"]), s.plato("spilka"), float(s.vessels["spilka"]["t"]), float(s.vessels["spilka"].get("trub", 0.0)) * 100.0])
	for ln in sp.log_lines.slice(maxi(sp.log_lines.size() - 12, 0)):
		print("CHKLOG ", ln)
	if auto:
		_stop_auto(false)
	if show_pk and main.get("protokol") != null:
		var pk: Control = main.get("protokol")
		pk.visible = true
		visible = false
		print("PROTOKOL
", pk.call("_text"))
	if demo:
		var open_v: Array = []
		var pumps: Array = []
		var extra := ""
		for it in plan.items:
			if str(it.get("type")) == "valve":
				if bool(it.get("open", false)):
					open_v.append(str(it.id))
				elif extra == "" and str(it.get("kind", "")) == "klapka":
					extra = str(it.id)
			elif str(it.get("type")) == "pump" and bool(it.get("on", false)):
				pumps.append(str(it.id))
		var cesta: String = str(navrh.want.keys()[0]) if not navrh.want.is_empty() else ""
		data[str(cur + 1)] = {"krok": flat[cur].t, "skupina": flat[cur].grp, "popis": "TEST jen v paměti", "cesta": cesta, "stav": {"open": open_v, "pumps_on": pumps}}
		if not open_v.is_empty():
			sp._set_item(plan.get_item(str(open_v[0])), "open", false)  # obsluha zapomněla jednu klapku
		if extra != "":
			sp._set_item(plan.get_item(extra), "open", true)  # a jednu otevřela navíc
		_rebuild()
		_select(cur)
		print("CHKDEMO krok %d, cesta %s, overlay %s" % [cur + 1, cesta, str(_overlay != null)])
		var it0: Dictionary = plan.get_item(str(open_v[0])) if not open_v.is_empty() else {}
		if not it0.is_empty():
			main.call("_focus", Vector2(float(it0.x), float(it0.y)), 700.0)
	if stop_at >= 0:
		print("CHKSTOP krok %d: %s" % [cur + 1, chk.get_parsed_text().replace("\n", " | ")])
	sp._save_acc = -1.0e12
	if had:
		var f := FileAccess.open(SimPanel.SAVE_PATH, FileAccess.WRITE)
		if f != null:
			f.store_string(bak)
	else:
		DirAccess.remove_absolute(ProjectSettings.globalize_path(SimPanel.SAVE_PATH))


## klapky a čerpadla tras a které cesty tečou při společném nastavení několika cest
func probe_routes() -> void:
	var sp = _sp()
	for rn in sp.sim.routes:
		print("ROUTE %-16s pump=%-16s valves=%s" % [str(rn), sp.sim.route_pump(str(rn)), str(sp.route_valve_ids(str(rn)))])
	for combo in [["glykol"], ["lv_fill", "glykol"], ["glykol", "voda_panev", "voda_rad"], ["glykol", "voda_rad"], ["glykol", "voda_panev"], ["glykol", "vyslazovani"], ["glykol", "podraz"], ["glykol", "rmut_sk"], ["glykol", "dilo"], ["glykol", "dilo", "vyslazovani"], ["glykol", "varna_virivka"], ["glykol", "ledovka"], ["glykol", "ledovka", "virivka_chladic"]]:
		navrh.want.clear()
		for rn in combo:
			navrh.want[rn] = true
		navrh.apply_routes()
		var fl: Array = []
		for rn in sp._flowing:
			if bool(sp._flowing[rn]):
				fl.append(str(rn))
		print("COMBO %s -> teče %s" % [str(combo), str(fl)])
	navrh.want.clear()
	navrh.apply_routes()
