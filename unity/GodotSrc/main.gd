extends Node2D
const Art = preload("res://art.gd")
const Sim = preload("res://sim.gd")
const Ctl = preload("res://ctl.gd")
const Auto = preload("res://auto.gd")
const Sight = preload("res://sight.gd")
const Iodine = preload("res://iodine.gd")
const Cabinet = preload("res://cabinet.gd")
const LiquidFx = preload("res://liquidfx.gd")
var cabinet = null
var cab_hold: = ""
var iodine_win
var sight_win
var auto
const PANEL_W: = 280.0
var panel_on: = false
var dock_on: = true
var dock = null

## kolik místa vpravo zabírá panel a rozvaděč
func right_w() -> float:
    var w: = 0.0
    if ctl != null and panel_on: w += PANEL_W
    if dock != null and dock_on: w += dock_w()
    return w

func _load_ui() -> void :
    if FileAccess.file_exists("user://ui.json"):
        var d = JSON.parse_string(FileAccess.open("user://ui.json", FileAccess.READ).get_as_text())
        if d is Dictionary:
            panel_on = bool(d.get("panel", false));dock_on = true;dock_big = bool(d.get("big", false));hard = bool(d.get("hard", false))

func _save_ui() -> void :
    var f: = FileAccess.open("user://ui.json", FileAccess.WRITE)
    if f != null: f.store_string(JSON.stringify({"panel": panel_on, "dock2": dock_on, "big": dock_big, "hard": hard}))

func apply_ui() -> void :
    if flow_layer != null: flow_layer.visible = not hard
    if ctl != null: ctl.visible = panel_on;ctl.dirty()
    if dock != null:
        dock.visible = dock_on
        dock.offset_right = - (PANEL_W if (ctl != null and panel_on) else 0.0)
        dock.offset_left = dock.offset_right - dock_w()
        dock.fit()
    if hud != null: hud.ui_sync()

func toggle_panel() -> void :
    panel_on = not panel_on;_save_ui();apply_ui()
func toggle_dock() -> void :
    return
func dock_temp(on: bool) -> void :
    dock_on = on;apply_ui()

var dock_big: = false
## režim: soft = červená/zelená klapka + viditelné toky v trubkách; hard = modré klapky (rovnoběžně otevřeno), toky v trubkách nejsou vidět, hladinu je nutné změřit
var hard: = false
var calm_since: = {}
var calm_y: = {}
func toggle_hard() -> void :
    hard = not hard;_save_ui()
    if flow_layer != null: flow_layer.visible = not hard
    redraw();redraw_valves()
    if hud != null: hud.ui_sync();hud.toast("Režim HARD: modré klapky, toky v trubkách nejsou vidět, hladinu změř tlačítkem MĚŘ" if hard else "Režim SOFT: červená = zavřeno, zelená = otevřeno, toky jsou vidět")

## hladina je „klidná“, když se 15 s (herního času) nehnula a nemíchá se ani nevaří
func vessel_calm(v) -> bool:
    var b = v.get("bind"); var S = sim.S
    var busy: = false
    if b in ["rvp", "mp", "wp"]:
        busy = S[b].get("stir", false) or S[b].get("boiling", false) or (b == "rvp" and S.rvp.vort > 0.003)
    return (not busy) and (S.t - calm_since.get(v.id, -1e9) >= 15.0)

func hard_sig(v) -> String:
    if not hard: return "s"
    var o: = "h" + ("1" if vessel_calm(v) else "0")
    for g in items:
        if g.type == "gauge" and gauge_vessel(g) == v: o += str(g.get("_meas", 0))
        if g.type == "dip" and v.get("bind") == "rvp": o += str(g.get("_meas", 0))
    return o

func _calm_tick() -> void :
    var S = sim.S
    for v in items:
        if v.type != "vessel" or v.get("bind") == null: continue
        var y: float = level_y(v)
        var b = v.get("bind"); var busy: = false
        if b in ["rvp", "mp", "wp"]: busy = S[b].get("stir", false) or S[b].get("boiling", false) or (b == "rvp" and S.rvp.vort > 0.003)
        if busy or absf(y - calm_y.get(v.id, y)) > 0.25 or not calm_since.has(v.id): calm_since[v.id] = S.t
        calm_y[v.id] = y

func dip_geo(it) -> Dictionary:
    var k = vessel_by_bind("rvp"); var x: float = it.x; var y: float = it.y; var h: = float(it.get("h", 300))
    if k != null:
        h = k.h - minf(k.h * 0.12, 40.0);y = k.y;x = k.x + k.w + 34.0
    return {"x": x, "y": y, "h": h, "v": k}

func measure_dip(d) -> void :
    var G: Dictionary = dip_geo(d)
    if G.v == null: return
    if not vessel_calm(G.v):
        hud.toast("Hladina není klidná (míchadlo, vaření, plnění nebo čerpání). Zastav to a počkej cca 15 s, pak měř znovu.");return
    d["_meas"] = sim.hotV(sim.S.rvp) / 100.0
    hud.toast("Měrka kotle: hladina %s hl" % c1(d["_meas"]))
    redraw();update_live()

func measure_gauge(g) -> void :
    var v = gauge_vessel(g)
    if v == null: hud.toast("Měrka nemá nádobu.");return
    if g.get("bot", true) == false or g.get("top", true) == false:
        hud.toast("Měrka je odpojená kohouty – otevři horní i dolní kohout.");return
    if not vessel_calm(v):
        hud.toast("Hladina není klidná (míchadlo, vaření, plnění nebo čerpání). Zastav to a počkej cca 15 s, pak měř znovu.");return
    var G: Dictionary = gauge_geo(g)
    g["_meas"] = clampf(level_y(v), G.y1, G.y2);g["_meas_t"] = sim.S.t
    var fr: float = 10.0 * (G.y2 - g["_meas"]) / maxf(1.0, G.y2 - G.y1)
    hud.toast("Změřeno: hladina na %s z 10 dílků." % c1(fr))
    redraw();update_live()
## šířka rozvaděče vpravo: normálně na celou výšku okna, zvětšený až 60 % šířky
func dock_w() -> float:
    var vs: = vp_size()
    var free: float = vs.x - (PANEL_W if (ctl != null and panel_on) else 0.0)
    var h: = vs.y - 52.0
    if dock_big: return clampf(free * 0.5, 520.0, 900.0)
    return clampf(h * 0.34, 260.0, 380.0)

func toggle_dock_big() -> void :
    dock_big = not dock_big
    dock_on = true
    _save_ui();apply_ui()
    if dock != null: dock.reset_view()

## rozvaděč stále na očích: vlastní pohled do světa (sdílí world_2d), klikání předává rozvaděči
class CabDock extends SubViewportContainer:
    var m
    var sv: SubViewport
    var cam2: Camera2D
    var hold: = ""
    var lens: SubViewportContainer
    var lsv: SubViewport
    var lcam: Camera2D
    var lens_on: = false
    var lens_btn: Button
    ## lupa: po kliknutí do rozvaděče se nahoře/dole otevře zvětšený výřez kolem místa; v lupě se dá normálně ovládat
    func _lens_open(p: Vector2) -> void :
        if lens == null: return
        var w: = _to_world(p)
        lens.size = Vector2(maxf(250.0, size.x - 16.0), 250.0);lens.custom_minimum_size = lens.size
        lcam.make_current()
        var z: = cam2.zoom.x * 2.9
        lcam.position = w;lcam.zoom = Vector2(z, z)
        var ly: float = size.y - lens.size.y - 8.0 if p.y < size.y * 0.5 else 8.0
        lens.position = Vector2(8.0, ly);lens.visible = true;lens_on = true
        lens_btn.position = Vector2(lens.position.x + lens.size.x - 92.0, lens.position.y + 6.0);lens_btn.visible = true
    func _lens_close() -> void :
        lens_on = false
        if lens != null: lens.visible = false
        if lens_btn != null: lens_btn.visible = false
    var pot_in_lens: = false
    ## souřadnice události → svět: uvnitř lupy přes lupovou kameru, jinde přes kameru rozvaděče
    func _ew(p: Vector2) -> Vector2:
        var use_lens: bool = false
        if not pot.is_empty() or hold != "": use_lens = pot_in_lens
        else: use_lens = lens_on and Rect2(lens.position, lens.size).has_point(p)
        if use_lens: return lcam.position + (p - lens.position - lens.size * 0.5) / lcam.zoom.x
        return _to_world(p)
    func setup() -> void :
        stretch = true;mouse_filter = Control.MOUSE_FILTER_STOP
        anchor_left = 1.0;anchor_right = 1.0;anchor_bottom = 1.0;offset_top = 52;offset_bottom = 0
        sv = SubViewport.new();sv.world_2d = m.get_viewport().world_2d;sv.transparent_bg = false
        add_child(sv)
        cam2 = Camera2D.new();sv.add_child(cam2);cam2.enabled = true;cam2.make_current()
        resized.connect(reset_view)
        gui_input.connect(_on_input)
        # lupa: při najetí myší zvětší výřez rozvaděče kolem kurzoru (čitelné popisky)
        lens = SubViewportContainer.new();lens.stretch = true;lens.mouse_filter = Control.MOUSE_FILTER_IGNORE;lens.custom_minimum_size = Vector2(330, 170);lens.size = Vector2(330, 170);lens.visible = false
        lsv = SubViewport.new();lsv.world_2d = m.get_viewport().world_2d;lsv.transparent_bg = false;lens.add_child(lsv)
        lcam = Camera2D.new();lsv.add_child(lcam);lcam.enabled = true
        add_child(lens)
        var fr: = Panel.new();fr.mouse_filter = Control.MOUSE_FILTER_IGNORE;fr.set_anchors_preset(Control.PRESET_FULL_RECT)
        var fsb: = StyleBoxFlat.new();fsb.bg_color = Color(0, 0, 0, 0);fsb.border_color = Color("f2c230");fsb.set_border_width_all(3);fsb.set_corner_radius_all(8)
        fr.add_theme_stylebox_override("panel", fsb);lens.add_child(fr)
        lens_btn = Button.new();lens_btn.text = "✕ zavřít";lens_btn.focus_mode = Control.FOCUS_NONE;lens_btn.visible = false
        lens_btn.pressed.connect(_lens_close);add_child(lens_btn)
        m.get_viewport().size_changed.connect(func(): m.apply_ui())
    func rect() -> Rect2:
        var it = m.cabinet.it; var cs: float = m.cabinet.sc
        return Rect2(float(it.x) + 190.0 * cs, float(it.y) - 10.0, 580.0 * cs + 10.0, 1130.0 * cs + 20.0)
    var user: = false
    var panning: = false
    func base_zoom() -> float:
        var r: = rect()
        return minf(size.x / r.size.x, size.y / r.size.y) if not m.dock_big else size.x / r.size.x
    func reset_view() -> void :
        user = false;fit()
    func fit() -> void :
        if cam2 == null or m.cabinet == null or size.x < 10: return
        if user:
            _clamp();return
        var r: = rect(); var z: = base_zoom()
        cam2.zoom = Vector2(z, z)
        cam2.position = Vector2(r.get_center().x, r.position.y + size.y * 0.5 / z)
        _clamp()
    func _clamp() -> void :
        var r: = rect(); var z: float = cam2.zoom.x; var half: = size * 0.5 / z
        var lo: = r.position + half; var hi: = r.end - half
        cam2.position.x = r.get_center().x if lo.x > hi.x else clampf(cam2.position.x, lo.x, hi.x)
        cam2.position.y = (r.position.y + half.y) if lo.y > hi.y else clampf(cam2.position.y, lo.y, hi.y)
    func _zoom_at(f: float, p: Vector2) -> void :
        var w0: = _to_world(p)
        var z: = clampf(cam2.zoom.x * f, base_zoom(), base_zoom() * 3.5)
        cam2.zoom = Vector2(z, z)
        cam2.position += w0 - _to_world(p);user = true;_clamp()
    func _draw() -> void :
        draw_rect(Rect2(Vector2.ZERO, Vector2(2, size.y)), Color("2a323a"))
    func _to_world(p: Vector2) -> Vector2:
        return cam2.position + (p - size * 0.5) / cam2.zoom.x
    var pot: = {}
    func _pot_from(p: Vector2) -> void :
        var d: Vector2 = _ew(p) - pot.c
        var a: = rad_to_deg(atan2(d.x, - d.y))
        m.cabinet.set_pot(pot.k, (clampf(a, -135.0, 135.0) + 135.0) / 270.0);m.redraw()
    func _on_input(e: InputEvent) -> void :
        if e is InputEventMouseButton and e.button_index == MOUSE_BUTTON_LEFT and not e.pressed and not pot.is_empty():
            pot = {};accept_event();return
        if e is InputEventMouseButton and e.button_index == MOUSE_BUTTON_LEFT and e.pressed:
            var in_lens: bool = lens_on and Rect2(lens.position, lens.size).has_point(e.position)
            if not in_lens:
                _lens_open(e.position);accept_event();return
            pot_in_lens = true
            var ph: Dictionary = m.cabinet.pot_hit(_ew(e.position))
            if not ph.is_empty():
                pot = ph;_pot_from(e.position)
                if m.game != null: m.game.play("click")
                accept_event();return
        if e is InputEventMouseMotion and not pot.is_empty():
            _pot_from(e.position);accept_event();return
        if e is InputEventMouseButton and e.pressed and e.button_index in [MOUSE_BUTTON_WHEEL_UP, MOUSE_BUTTON_WHEEL_DOWN]:
            var up_: bool = e.button_index == MOUSE_BUTTON_WHEEL_UP
            var ph2: Dictionary = m.cabinet.pot_hit(_ew(e.position))
            if not ph2.is_empty() and not e.ctrl_pressed:
                m.cabinet.set_pot(ph2.k, m.cabinet.pot_value(ph2.k) + (1.0 if up_ else -1.0) * 0.2 / 9.0);m.redraw()
            elif e.ctrl_pressed: _zoom_at(1.15 if up_ else 1.0 / 1.15, e.position)
            else:
                cam2.position.y += (-60.0 if up_ else 60.0) / cam2.zoom.x;user = true;_clamp()
            accept_event();return
        if e is InputEventMouseButton and e.button_index == MOUSE_BUTTON_LEFT:
            var w: = _ew(e.position)
            if e.pressed:
                var hk: String = m.cabinet.hit(w)
                if hk == "hup" or hk == "hdn":
                    hold = hk;m.cabinet.act(hk, true)
                elif hk != "": m.cabinet.act(hk, true)
                m.redraw()
            elif hold != "":
                m.cabinet.act(hold, false);hold = ""
            accept_event()
        elif e is InputEventMouseButton:
            var mb: = e as InputEventMouseButton
            if mb.button_index in [MOUSE_BUTTON_RIGHT, MOUSE_BUTTON_MIDDLE]: panning = mb.pressed
            accept_event()
        elif e is InputEventMouseMotion and panning:
            cam2.position -= (e as InputEventMouseMotion).relative / cam2.zoom.x;user = true;_clamp();accept_event()
        elif e is InputEventMouseMotion and m.game != null:
            var hk: String = m.cabinet.hit(_ew(e.position))
            var isp: bool = not m.cabinet.pot_hit(_ew(e.position)).is_empty()
            mouse_default_cursor_shape = Control.CURSOR_DRAG if isp else (Control.CURSOR_POINTING_HAND if hk != "" else Control.CURSOR_ARROW)
            tooltip_text = "FM: táhni dokola, nebo kolečko myši (stupeň %s)" % m.cabinet._stf(m.cabinet.pot_value(m.cabinet.pot_hit(_ew(e.position)).k)) if isp else ""
var sim
var ctl
var flow_layer
var live_nodes: = []
var hl: = {}
var NOLIVE: = "--nolive" in OS.get_cmdline_user_args()
var NOFLOW: = "--noflow" in OS.get_cmdline_user_args()
var T_live: = 0
var T_flow: = 0
var T_sim: = 0
var N_sim: = 0
var NOSIM: = "--nosim" in OS.get_cmdline_user_args()
var dyn_t: = 0.0
var flow_t: = 0.0
var calm_t: = 0
var running: = false
var speed: = 10.0
var sim_acc: = 0.0



const PIPE_OUT: = Color("15191d")
const PIPE_MID: = Color("aab2ba")
const PIPE_IN: = Color("0c0f12")
const STEEL: = Color("8a939b")
const STEEL_D: = Color("23282e")
const DARK: = Color("2b3138")
const OPEN_C: = Color("2fbf5b")
const SHUT_C: = Color("d93a2c")
const ACCENT: = Color("f2c230")

var plan: = {}
var items: Array = []
var bounds: = Rect2()
var floor_y: = 0.0
var no_gpu_liquid: = false
var hall_dark: = true
var cam: Camera2D
var static_layer: StaticLayer
var item_layer: ItemLayer
var valve_layer: ItemLayer
var hud: Hud
var game
var mini
var updater
var font: Font
var font_b: Font


var zoom_target: = 0.4
var zoom_anchor: = Vector2.ZERO
var zoom_anim: = false
var dragging: = false
var drag_moved: = false
var drag_start: = Vector2.ZERO
var vel: = Vector2.ZERO
var last_move_ms: = 0

func _ready() -> void :
    if DisplayServer.get_name() != "headless": LiquidFx.prewarm()   # textury mláta, rmutu, kalu a pěny (0,3 s)
    var sf: = SystemFont.new();sf.font_names = PackedStringArray(["Bahnschrift", "Segoe UI", "Arial"]);font = sf
    var sfb: = SystemFont.new();sfb.font_names = PackedStringArray(["Bahnschrift SemiBold", "Bahnschrift", "Segoe UI Semibold", "Arial"]);sfb.font_weight = 600;font_b = sfb
    if FileAccess.file_exists("user://vzhled.txt"): hall_dark = FileAccess.open("user://vzhled.txt", FileAccess.READ).get_as_text().strip_edges() != "svetla"
    if "--light" in OS.get_cmdline_user_args(): hall_dark = false
    hall_dark = false
    RenderingServer.set_default_clear_color(Color.WHITE)
    DisplayServer.window_set_title("Trenažér varny Zajíc – nativní")
    _load_plan()
    _init_outlets()
    sim = Sim.new();sim.main = self
    sim.deposit_on = not ("--tests" in OS.get_cmdline_user_args())   # nános na dně SK (prokopnutí trhnutím) jen v ostrém běhu
    if FileAccess.file_exists("user://recept.txt"):
        sim.recipe_idx = int(FileAccess.open("user://recept.txt", FileAccess.READ).get_as_text())
    auto = Auto.new();auto.m = self;auto.sim = sim
    practice = preload("res://practice.gd").new();practice.m = self;practice.sim = sim
    var tb0: = Time.get_ticks_msec()
    sim.setup(items, plan.get("joints", []))
    print("síť: %d uzlů, %d trubek, %d ms" % [sim.net.nodes.size(), sim.net.edges.size(), Time.get_ticks_msec() - tb0])
    var restored: = false
    if not ("--simtest" in OS.get_cmdline_user_args() or "--demo" in OS.get_cmdline_user_args() or "--fresh" in OS.get_cmdline_user_args() or "--autotest" in OS.get_cmdline_user_args() or "--tests" in OS.get_cmdline_user_args()):
        restored = load_game()
    if "--savetest" in OS.get_cmdline_user_args():
        _savetest();get_tree().quit();return
    if "--tests" in OS.get_cmdline_user_args():
        save_path = "user://test_tmp.sav"; await _run_tests();return
    if "--autotest" in OS.get_cmdline_user_args():
        _autotest();get_tree().quit();return
    if "--simtest" in OS.get_cmdline_user_args():
        _simtest();get_tree().quit();return
    for a_ in OS.get_cmdline_user_args():
        if a_.begins_with("--diag="):
            var D = load(a_.substr(7)).new();D.m = self;D.run();get_tree().quit();return
    cam = Camera2D.new()
    add_child(cam)
    cam.make_current()
    updater = preload("res://updater.gd").new();updater.main = self;add_child(updater)
    hud = Hud.new();hud.m = self;add_child(hud)
    if DisplayServer.get_name() != "headless" and not ("--noaa" in OS.get_cmdline_user_args()): _make_aa()
    v3 = preload("res://view3d.gd").new();v3.m = self;add_child(v3)
    updater.status.connect( func(t: String, _k: String): hud.toast(t))
    updater.available.connect( func(v: int): hud.upd_btn.text = "⟳ Nová verze %d" % v;hud.upd_btn.add_theme_color_override("font_color", Color("f2c230")))
    if DisplayServer.get_name() != "headless" and OS.get_cmdline_user_args().size() == 0:
        get_tree().create_timer(4.0).timeout.connect( func(): updater.check(false))
    mini = Mini.new();mini.m = self;hud.add_child(mini);mini.setup()
    hud.toast("Připravuji plánek…")
    await get_tree().process_frame
    fit_all()
    if DisplayServer.get_name() != "headless": await _bake()

    flow_layer = FlowLayer.new();flow_layer.m = self;add_child(flow_layer);flow_layer.build();flow_layer.visible = not hard


    for it in items:
        if it.type == "vessel" and it.get("bind") in ["rvp", "mp", "sk", "wp", "hlt", "lv", "spilka"]: _live_node("vessel", it)
    for it in items:
        if it.type == "pump": _live_node("pump", it).position = Vector2(it.x, it.y)
    _live_node("streams", null);_live_node("leaks", null)
    for itp in items:
        if itp.type == "panel":
            cabinet = Cabinet.new();cabinet.m = self;cabinet.setup(itp);add_child(cabinet)
            cabinet.visibility_layer = 2;get_viewport().canvas_cull_mask = 0xFFFFFFFD
            break
    item_layer = ItemLayer.new();item_layer.m = self;item_layer.live_only = true;add_child(item_layer)
    valve_layer = ItemLayer.new();valve_layer.m = self;valve_layer.live_only = true;valve_layer.valves_only = true;add_child(valve_layer)
    for it in items:
        if it.type == "vessel" and it.get("bind") in ["rvp", "mp", "sk", "wp", "hlt", "lv", "spilka"]: _live_node("card", it)
    _load_ui();hard = hard or ("--hard" in OS.get_cmdline_user_args());apply_ui()
    if "--panel" in OS.get_cmdline_user_args(): panel_on = true
    if "--dockbig" in OS.get_cmdline_user_args(): dock_big = true
    if not ("--nopanel" in OS.get_cmdline_user_args()):
        ctl = Ctl.new();ctl.m = self;add_child(ctl)
    if cabinet != null:
        dock = CabDock.new();dock.m = self;hud.add_child(dock);dock.setup()
    game = preload("res://game.gd").new();game.m = self;add_child(game)
    var rota = preload("res://rotametr.gd").new();rota.m = self;game.add_child(rota)
    apply_ui()
    await get_tree().process_frame
    if dock != null: dock.fit()
    fit_all()
    for a0 in OS.get_cmdline_user_args():
        if a0.begins_with("--go="): hud._go(a0.substr(5))
        if a0.begins_with("--view="):
            var vv: = a0.substr(7).split(",");cam.position = Vector2(float(vv[0]), float(vv[1]));cam.zoom = Vector2(float(vv[2]), float(vv[2]))
    if "--editor" in OS.get_cmdline_user_args() or "--edtest" in OS.get_cmdline_user_args(): open_editor()
    if "--edtest" in OS.get_cmdline_user_args():
        editor.selftest();get_tree().quit();return
    for a1 in OS.get_cmdline_user_args():
        if a1.begins_with("--edsel=") and editor != null:
            for it1 in items:
                if str(it1.get("bind", "")) == a1.substr(8): editor.sel = {"t": "item", "it": it1};editor._props();editor.queue_redraw()
    if "--edclick" in OS.get_cmdline_user_args():
        if editor == null or not editor.on: open_editor()
        await editor.clicktest();get_tree().quit();return
    hud.toast("Rozjetá várka obnovena (%d:%02d)" % [int(sim.S.t / 3600), int(sim.S.t / 60) %60] if restored else "")
    if "--demo" in OS.get_cmdline_user_args():

        for id in sim.route_valves("hlt", "rvp", ["phw"]):
            if sim.net.byId.has(id): sim.net.byId[id]["open"] = true
            if outlet_open.has(id): outlet_open[id] = true
        sim.valve_ver += 1
        sim.S.rvpWater.on = true;sim.S.rvp.stir = true
        for i in 240: sim.sim_step(1.0)
        sim.S.malt.gate = true;sim.S.malt.on = true
        running = not ("--paused" in OS.get_cmdline_user_args());speed = 10.0
        for a in OS.get_cmdline_user_args():
            if a.begins_with("--speed="): speed = float(a.substr(8))
        flow_layer.refresh();update_live()
        if "--iodine" in OS.get_cmdline_user_args(): open_iodine("rvp")
        if "--pumpdemo" in OS.get_cmdline_user_args():
            for id in sim.route_valves("rvp", "sk", ["p1"]):
                if sim.net.byId.has(id): sim.net.byId[id]["open"] = true
            sim.valve_ver += 1;sim.S.cab.key = true;sim.S.p1.on = true
        if "--sight" in OS.get_cmdline_user_args():
            for it0 in items:
                if it0.type == "inl" and it0.get("k", "") == "pruh":
                    sight_win = Sight.new();sight_win.m = self;hud.add_child(sight_win);sight_win.open(it0);break

    if "--v3" in OS.get_cmdline_user_args():
        toggle_3d(true)
        for a in OS.get_cmdline_user_args():
            if a.begins_with("--yaw="): v3.yaw = float(a.substr(6))
            if a.begins_with("--pitch="): v3.pitch = float(a.substr(8))
            if a.begins_with("--dist="): v3.dist = float(a.substr(7))
            if a.begins_with("--look="): v3.look_at_item(a.substr(7), v3.dist)
            if a.begins_with("--tx="): v3.target.x = float(a.substr(5))
        v3._cam()
    for a in OS.get_cmdline_user_args():
        if a.begins_with("--rect="):
            var q = a.substr(7).split(",")
            go_to(Rect2(float(q[0]), float(q[1]), float(q[2]), float(q[3])))
        if a.begins_with("--go="):
            hud._go(a.substr(5))
    if "--bench" in OS.get_cmdline_user_args():
        await get_tree().create_timer(1.0).timeout
        var t0: = Time.get_ticks_usec(); var frames: = 0; var worst: = 0.0; var last: = t0
        while Time.get_ticks_usec() - t0 < 6000000:
            var el: = (Time.get_ticks_usec() - t0) / 1000000.0
            cam.position.x += 600.0 * get_process_delta_time() / cam.zoom.x * (1.0 if fmod(el, 2.0) < 1.0 else -1.0)
            if fmod(el, 3.0) < 0.05: zoom_at(1.6 if fmod(el, 6.0) < 3.0 else 0.62, vp_size() * 0.5)
            await get_tree().process_frame
            var now: = Time.get_ticks_usec();worst = maxf(worst, (now - last) / 1000.0);last = now;frames += 1
        print("TIMES live=%d ms flow=%d ms sim=%d ms (%d kroků)" % [T_live / 1000, T_flow / 1000, T_sim / 1000, N_sim])
        print("BENCH snímků/s=%.1f nejhorší snímek=%.1f ms" % [frames / ((Time.get_ticks_usec() - t0) / 1000000.0), worst])
        get_tree().quit()
    if "--hosetest" in OS.get_cmdline_user_args():

        await get_tree().create_timer(0.5).timeout
        var hs = null
        for it in items:
            if it.type == "pipe" and it.get("hose", false) and it.get("dn", 50) == 40: hs = it
        var a: = Vector2(hs.pts[0][0], hs.pts[0][1]); var b: Vector2 = outlets[0].front
        var to_s: = func(w: Vector2) -> Vector2: return (w - cam.position) * cam.zoom.x + vp_size() * 0.5
        var ev: = InputEventMouseButton.new();ev.button_index = MOUSE_BUTTON_LEFT;ev.pressed = true;ev.position = to_s.call(a);Input.parse_input_event(ev)
        for i in 11:
            var mv: = InputEventMouseMotion.new();mv.position = to_s.call(a.lerp(b, i / 10.0));Input.parse_input_event(mv)
            await get_tree().process_frame
        await get_tree().process_frame
        var up: = InputEventMouseButton.new();up.button_index = MOUSE_BUTTON_LEFT;up.pressed = false;up.position = to_s.call(b);Input.parse_input_event(up)
        await get_tree().process_frame
        print("HOSETEST konec=", hs.pts[0], " vývod=", b)
    for a in OS.get_cmdline_user_args():
        if a.begins_with("--shot="):
            await get_tree().create_timer(4.0).timeout
            get_viewport().get_texture().get_image().save_png(a.substr(7))
            get_tree().quit()



func _savetest() -> void :
    var S = sim.S
    S.rvpWater.on = true
    for id in sim.route_valves("hlt", "rvp", ["phw"]):
        if sim.net.byId.has(id): sim.net.byId[id]["open"] = true
        if outlet_open.has(id): outlet_open[id] = true
    sim.valve_ver += 1
    for i in 300: sim.sim_step(1.0)
    var before: = [S.t, sim.vol(S.rvp), S.rvp.T, S.hlt.w]
    save_game()
    sim.new_state();sim.build_net(items, plan.get("joints", []))
    var ok: = load_game()
    var S2 = sim.S
    print("SAVETEST načteno=%s před=%s po=%s" % [ok, str(before), str([S2.t, sim.vol(S2.rvp), S2.rvp.T, S2.hlt.w])])
    DirAccess.remove_absolute(ProjectSettings.globalize_path(save_path))



func _run_tests() -> void :
    var T = preload("res://tests.gd").new();T.m = self
    var failed: int = T.run()
    get_tree().quit(1 if failed > 0 else 0)

func _autotest() -> void :
    for a0 in OS.get_cmdline_user_args():
        if a0.begins_with("--recipe="): sim.recipe_idx = int(a0.substr(9))
    sim.new_state();sim.build_net(items, plan.get("joints", []))
    var S = sim.S;auto.S = S
    for rt in [["lv", "lv", ["plv"]], ["lv", "hlt", ["plv"]], ["lv", "tanky", ["plv"]], ["wp", "spilka", ["p3"]], ["vodovod", "spilka", []], ["vodovod", "lv", []]]:
        var vs = sim.route_valves(rt[0], rt[1], rt[2])
        print("TRASA ", rt, " -> ", vs, " uzly: ", sim.last_route.size())
    auto.start()
    var last: = -1; var t0: = Time.get_ticks_msec()
    var maxs: = 26 * 3600
    var adt: = 1.0
    for a in OS.get_cmdline_user_args():
        if a.begins_with("--autodt="): adt = float(a.substr(9))
    var lt: = 0.0
    while auto.on and S.t < maxs:
        auto.tick(adt);sim.sim_step(adt)
        if S.sk.mode == "lauter" and S.t - lt >= 300.0 and "--lauterlog" in OS.get_cmdline_user_args():
            lt = S.t
            print("  SK t=%.0f min Qmax %.0f Qact %.0f C2 %.0f dp %.0f mbar c %.2f ch %.2f teig %.1f kg lože %.0f cm hladina %.0f cm °P %.1f turb %.0f pánev %.0f L" % [S.t / 60.0, S.sk.Qmax, S.sk.Qact, S.p2.flow, S.sk.dp, S.sk.c, S.sk.ch, S.sk.get("teig", 0.0), sim.skBedH() * 100.0, sim.skHead() * 100.0, S.sk.outP, S.sk.turb, sim.vol(S.rvp)])
        if auto.i != last:
            last = auto.i
            var rr = S.rvp
            var mm = S.sk.mash if sim.vol(S.sk.mash) > 100 else rr
            var tl: float = mm.sg + mm.dx + mm.ld + mm.f
            print("   jod: kapalina %.2f %% škrobu (sg %.0f dx %.0f ld %.0f f %.0f), se zrny %.2f %%, T=%.1f" % [100.0 * mm.sg / maxf(1.0, tl), mm.sg, mm.dx, mm.ld, mm.f, 100.0 * (mm.sg + mm.su + mm.sc) / maxf(1.0, tl + mm.su + mm.sc), mm.T])
            print("   rmut: extrakt %.0f kg (zkv %.0f, dx %.0f, ld %.0f, ns %.0f) škrob %.0f (sg %.0f su %.0f sc %.0f) ni %.0f g %.0f | SK extrakt %.0f kg" % [sim.ext(rr), rr.f, rr.dx, rr.ld, rr.ns, rr.sg + rr.su + rr.sc, rr.sg, rr.su, rr.sc, rr.ni, rr.g, sim.ext(S.sk.mash) + S.sk.cells.reduce( func(a, c): return a + c.f + c.n, 0.0)])
            if str(auto.status).begins_with("Chmelovar"): print("BILANCE PŘED VAREM: ", sim.extract_inventory())
            print("[%5.1f min] %s | pánev %.0f L %.1f °C | SK %.0f L | WP %.0f L | spilka %.0f L | led. %.0f L %.1f °C" % [S.t / 60.0, auto.status, sim.vol(S.rvp), S.rvp.T, sim.skVolTotal(), sim.vol(S.wp), S.sp.L, S.lv.w, S.lv.T])
    print("KONEC: done=%s failed=\'%s\' čas=%.1f h, výpočet %d s" % [auto.done, auto.failed, S.t / 3600.0, (Time.get_ticks_msec() - t0) / 1000])
    var rep = sim.spReport()
    print("PROTOKOL: ", rep)
    print("BILANCE EXTRAKTU: ", sim.extract_inventory())
    if "--pipeext" in OS.get_cmdline_user_args():
        var lst: = []
        for e in sim.net.edges:
            var x: float = sim.ext(e.c) + e.c.su + e.c.sc + e.c.ni
            if x > 0.2: lst.append([x, e.get("pipe", e.get("id", "?")), sim.vol(e.c)])
        lst.sort_custom(func(a1, b1): return a1[0] > b1[0])
        for r in lst.slice(0, 15): print("  TRUBKA %.1f kg  %s  %.0f L" % r)
    print("vyteklo L: %.0f, vyrážka: %s" % [S.drainL + S.spillL, str(S.flags.knock)])
    for l in S.log.slice(0, 14): print("  [%s] %d:%02d %s" % [l.type, int(l.t / 3600), int(l.t / 60) %60, l.msg])

func _simtest() -> void :

    for s in ["Spustit 12", "▶ Spustit 13", "Spustit 14", "▶ Spustit 15", "■ 16"]:
        var t0: = Time.get_ticks_usec(); var w: = font_b.get_string_size(s, HORIZONTAL_ALIGNMENT_LEFT, -1, 14).x
        print("sazba \'%s\' %.1f ms (šířka %d)" % [s, (Time.get_ticks_usec() - t0) / 1000.0, w])
    var S = sim.S
    var pr: = func(lbl: String):
        var r = S.rvp
        print("%s t=%d min | pánev %.0f L %.1f °C %.2f °P škrob=%.1f | bojler %.0f L | SK %.0f L | únik %.0f L | toky=%s" % [lbl, S.t / 60, sim.vol(r), r.T, sim.plato(r), r.sg + r.su, S.hlt.w, sim.skVolTotal(), S.drainL + S.spillL, str(S.flows)])
    for rt in [["hlt", "rvp", ["phw"]], ["rvp", "sk", ["p1"]], ["sk", "rvp", ["p2"]], ["rvp", "wp", ["p1"]], ["wp", "spilka", ["p3"]], ["vodovod", "lv", []], ["hlt", "sk", ["phw"]]]:
        var vs = sim.route_valves(rt[0], rt[1], rt[2])
        var desc: = []
        if vs != null:
            for id in vs:
                var v = sim.net.byId.get(id)
                desc.append("%s(%s)" % [id, ("otevř" if sim.valve_open(id) else "ZAVŘ")])
        print("cesta %s→%s: %s" % [rt[0], rt[1], "NENÍ" if vs == null else ", ".join(PackedStringArray(desc))])

    for id in sim.route_valves("hlt", "rvp", ["phw"]):
        if sim.net.byId.has(id): sim.net.byId[id]["open"] = true
        if outlet_open.has(id): outlet_open[id] = true
    sim.valve_ver += 1
    S.rvpWater.on = true
    var t: = Time.get_ticks_usec()
    sim.prof_on = true;sim.prof = {}
    for i in 600: sim.sim_step(1.0)
    print("krok %.2f ms" % ((Time.get_ticks_usec() - t) / 600000.0))
    sim.prof_on = false
    var pa = sim.lp();pa.w = 5.0;pa.T = 40.0; var pb = sim.lp();pb.w = 7.0;pb.T = 40.1
    var t2: = Time.get_ticks_usec()
    for i in 10000: sim._similar(pa, pb)
    print("  mb_similar %.1f us" % ((Time.get_ticks_usec() - t2) / 10000.0));t2 = Time.get_ticks_usec()
    for i in 10000: sim.vol(pa)
    print("  mb_vol %.1f us" % ((Time.get_ticks_usec() - t2) / 10000.0));t2 = Time.get_ticks_usec()
    var ee = sim.net.edges[0]
    for i in 10000: sim.edge_push(ee, pa, true)
    print("  mb_push %.1f us (q %d)" % [(Time.get_ticks_usec() - t2) / 10000.0, ee.q.size()])
    for k in sim.prof: print("  %s %.2f ms" % [k, sim.prof[k] / 600000.0])
    pr.call("voda")
    S.rvp.stir = true;S.malt.gate = true;S.malt.on = true
    for i in 1200: sim.sim_step(1.0)
    pr.call("vystírání")
    S.rvp.heat = 0.6
    for i in 3600: sim.sim_step(1.0)
    pr.call("ohřev")
    S.p1.on = true
    for i in 1200: sim.sim_step(1.0)
    pr.call("C1")

    for id in sim.route_valves("rvp", "sk", ["p1"]):
        if sim.net.byId.has(id): sim.net.byId[id]["open"] = true
        if outlet_open.has(id): outlet_open[id] = true
    sim.valve_ver += 1
    S.rvp.heat = 0.0
    for i in 120: sim.sim_step(1.0)
    print("C1 nastaveno %d L/min, skutečně %.0f L/min, stav \'%s\' | SK %.0f L, pánev %.0f L, kopačka/lože %.0f cm" % [S.p1.flow, S.p1.q, S.p1.status, sim.skVolTotal(), sim.vol(S.rvp), sim.skBedH() * 100.0])

    for l in S.log.slice(0, 25): print("  [%s] %d:%02d %s" % [l.type, int(l.t / 3600), int(l.t / 60) %60, l.msg])

const USER_PLAN: = "user://plan_upravy.json"
var plan_user: = false
var editor = null
var v3 = null
var practice = null

## živé (nepečené) vrstvy plánku pro editor
func make_plan_layers() -> Array:
    var sl: = StaticLayer.new();sl.m = self;add_child(sl);move_child(sl, 1)
    var il: = ItemLayer.new();il.m = self;il.skip_valves = true;add_child(il);move_child(il, 2)
    return [sl, il]

func open_editor() -> void :
    if editor == null:
        editor = preload("res://editor.gd").new();editor.m = self;add_child(editor);editor.setup()
    editor.toggle()
## zapojení dna SK podle skutečnosti: výstupy ze sít → věnec → (klapka) → sladinové čerpadlo;
## 2 horní klapky v sérii na větvi proplachu věnce (hadice do věnce), 3. klapka → hadice do 4 trysek pod síty
func _patch_pitch(data: Dictionary) -> void :
    if int(data.get("_pitch2", 0)) >= 1: return
    var its: Array = data.get("items", [])
    var by: = {}
    for it in its: by[it.get("id", "")] = it
    if by.has("pipe_murcuixph"): by["pipe_murcuixph"]["pts"] = [[4986, 924], [5005, 996], [5053, 1048], [5100, 1070], [5130, 1082]]
    if by.has("fit_murcuixpp"): by["fit_murcuixpp"]["x"] = 5160;by["fit_murcuixpp"]["y"] = 1082
    if by.has("inl_murcuixps"):
        by["inl_murcuixps"]["x"] = 5076;by["inl_murcuixps"]["y"] = 1059;by["inl_murcuixps"]["rot"] = 25
    if by.has("pipe_murcuixpt"): by["pipe_murcuixpt"]["pts"] = [[5190, 1082], [5190, 1140], [5320, 1140]]
    if by.has("pipe_murcuixpv"): by["pipe_murcuixpv"]["pts"] = [[5160, 1112], [5160, 1195]]
    if by.has("valve_murcuixp2"):
        by["valve_murcuixp2"]["x"] = 5160;by["valve_murcuixp2"]["y"] = 1154;by["valve_murcuixp2"]["rot"] = 90
    if by.has("valve_murcuixp1"): its.erase(by["valve_murcuixp1"])   # přímá trubka do tanku je bez klapky (klapka je na tanku)
    data["_pitch2"] = 1

## na přání: odbočka k vířivce u rozbočovače (T-kus v x≈2540, y≈890; trubka k vířivce vede nahoru po x=2460) neměla klapku → horká voda do pánve tekla i do vířivky. Přidá se jedna zavřená klapka na odbočku, nic jiného se nemění
func _patch_wpvalve(data: Dictionary) -> void :
    if int(data.get("_wpv", 0)) >= 1: return
    var its: Array = data.get("items", [])
    var tgt = null
    for it in its:
        if it.get("id", "") == "pipe_murb0t3b": tgt = it
        if it.get("id", "") == "valve_wp_odbocka": return
    if tgt == null: return
    var pts: Array = tgt.get("pts", [])
    var ok: = false
    for i in pts.size() - 1:
        if is_equal_approx(float(pts[i][0]), 2460.0) and is_equal_approx(float(pts[i + 1][0]), 2460.0) and minf(float(pts[i][1]), float(pts[i + 1][1])) <= 470.0 and maxf(float(pts[i][1]), float(pts[i + 1][1])) >= 890.0: ok = true
    if not ok: return
    its.append({"id": "valve_wp_odbocka", "type": "valve", "kind": "klapka", "label": "klapka", "open": false, "rot": 90, "x": 2460, "y": 600, "_on": true})
    data["_wpv"] = 1

## na přání: volný konec trubky pod SK (x 1520, y 780) vedl do kanálu – napojí se na svislou trubku dolů (x 1518, y 800), kam patří
func _patch_skstub(data: Dictionary) -> void :
    if int(data.get("_skstub", 0)) >= 1: return
    for it in data.get("items", []):
        if it.get("id", "") == "pipe_ed1103256632":
            var pts: Array = it.get("pts", [])
            if pts.size() == 4 and int(pts[0][0]) == 1520 and int(pts[0][1]) == 780:
                it["pts"] = [[1518, 800], [1518, 760], [1440, 760], [1440, 800]]
                data["_skstub"] = 1
            return

## na přání (scezování má jít trubkami zpod SK sítem do čerpadla C2): v uživatelově plánu je spoj věnce a sací trubky C2 v x 1060 místo v rohu 1070,850 → věnec nebyl napojen na C2. Doplní se jeden spoj do rohu, nic se nepřesouvá
func _patch_skjoint(data: Dictionary) -> void :
    if int(data.get("_skj", 0)) >= 1: return
    var ring: = false; var suc: = false
    for it in data.get("items", []):
        var pts: Array = it.get("pts", [])
        if it.get("id", "") == "pipe_mur9keqg":
            for q in pts:
                if int(q[0]) == 1070 and int(q[1]) == 850: ring = true
        if it.get("id", "") == "pipe_mur9pa1c" and pts.size() > 1 and int(pts[0][0]) == 1070 and int(pts[0][1]) == 850: suc = true
    if not (ring and suc): return
    var js: Array = data.get("joints", [])
    for q in js:
        if int(q[0]) == 1070 and int(q[1]) == 850:
            data["_skj"] = 1
            return
    js.append([1070, 850])
    data["joints"] = js
    data["_skj"] = 1

func _patch_splid(data: Dictionary) -> void :
    var its: Array = data.get("items", [])
    for it in its:
        if it.get("k", "") == "splid": return
    its.append({"id": "deco_splid", "type": "deco", "k": "splid", "x": 5318, "y": 662})

func _patch_ring(data: Dictionary) -> void :
    if int(data.get("_ring", 0)) >= 1: return
    for it in data.get("items", []):
        if it.get("id", "") == "pipe_mur9keqg":
            it["ringmul"] = PI;data["_ring"] = 1   # věnec je na plánku rovně, ve výpočtu kruh: obvod = π × průměr (odhad z nakresleného průměru)

func _patch_sk(data: Dictionary) -> void :
    if int(data.get("_sk3", 0)) >= 1 or OS.get_environment("NOSK") != "": return
    var its: Array = data.get("items", [])
    var by: = {}
    for it in its: by[it.get("id", "")] = it
    for id in ["pipe_mur9i4w4", "pipe_mur9jylp", "pipe_mur9msnh", "valve_mur9o327", "valve_mur9o0mr", "valve_mur9ntsh"]:
        if not by.has(id): return
    for hid in ["pipe_mur9ht2r", "pipe_mur9i4w4", "pipe_mur9jylp", "pipe_mur9mxq9", "pipe_mur9n2hh", "pipe_mur9na1o"]:
        if by.has(hid): by[hid]["look"] = "hose"
    by["pipe_mur9jylp"]["pts"] = [[1180, 800], [1280, 800]]
    by["pipe_mur9i4w4"]["pts"] = [[1280, 800], [1590, 800], [1590, 890]]
    by["pipe_mur9msnh"]["pts"] = [[1390, 850], [1445, 850], [1445, 890]]
    by["pipe_mur9msnh"].erase("look")
    by["valve_mur9ntsh"]["x"] = 1590;by["valve_mur9ntsh"]["y"] = 852;by["valve_mur9ntsh"]["rot"] = 90
    by["valve_mur9o327"]["x"] = 1435;by["valve_mur9o327"]["y"] = 820;by["valve_mur9o327"]["rot"] = 0
    by["valve_mur9o0mr"]["x"] = 1485;by["valve_mur9o0mr"]["y"] = 820;by["valve_mur9o0mr"]["rot"] = 0
    if not by.has("pipe_sk_proplach"):
        its.append({"id": "pipe_sk_proplach", "type": "pipe", "medium": "mash", "dn": 25, "label": "", "look": "hose", "pts": [[1530, 890], [1530, 820], [1390, 820]]})
    else:
        by["pipe_sk_proplach"]["pts"] = [[1530, 890], [1530, 820], [1390, 820]]
    if not by.has("valve_sk_vystup"):
        its.append({"id": "valve_sk_vystup", "type": "valve", "kind": "klapka", "label": "klapka", "open": false, "rot": 0, "x": 1417, "y": 850, "_on": true})
    else:
        by["valve_sk_vystup"]["x"] = 1417;by["valve_sk_vystup"]["y"] = 850;by["valve_sk_vystup"]["rot"] = 0
    var js: Array = data.get("joints", [])
    var nj: Array = []
    var dropl: = [[1440, 890], [1440, 850], [1440, 830], [1390, 830], [1500, 890], [1570, 890], [1570, 830], [1445, 890], [1530, 890], [1590, 890], [1390, 820]]
    for q in js:
        var drop: bool = false
        for r in dropl:
            if int(q[0]) == r[0] and int(q[1]) == r[1]: drop = true
        if not drop: nj.append(q)
    for r in [[1445, 890], [1530, 890], [1590, 890], [1390, 820]]: nj.append([r[0], r[1]])
    data["joints"] = nj
    data["_sk3"] = 1

func _load_plan() -> void :
    var path: = "res://plan.json"
    plan_user = FileAccess.file_exists(USER_PLAN) and not ("--tests" in OS.get_cmdline_user_args() or "--autotest" in OS.get_cmdline_user_args())
    if plan_user: path = USER_PLAN
    var f: = FileAccess.open(path, FileAccess.READ)
    if f == null:
        push_error("Plánek nenalezen: " + path)
        return
    var data = JSON.parse_string(f.get_as_text())
    if typeof(data) != TYPE_DICTIONARY:
        push_error("Plánek nejde přečíst")
        return
    if not plan_user: _patch_sk(data);_patch_ring(data)   # uživatelův plán z editoru se nikdy nepřepisuje
    _patch_splid(data)   # víčko spilky (přidává jen novou věc)
    _patch_wpvalve(data)   # klapka na odbočce k vířivce (viz funkce)
    _patch_skjoint(data)   # spoj věnce pod sítem se sáním C2 (viz funkce)
    _patch_skstub(data)   # napojení volného konce trubky pod SK (viz funkce)
    _patch_pitch(data)   # jediná výjimka na přání: spílací hadice – průhledítko, T-kus, výpust (jen těch pár prvků)
    plan = data
    items = plan.get("items", [])
    for it0 in items:
        if it0.get("type", "") == "valve" and it0.has("part") and not can_throttle(it0): it0.erase("part")   # přiškrcení jen u 3 klapek
        if it0.get("id", "") == "pipe_murcuixph": it0["fixed"] = true   # spílací hadice je napevno (průhledítko + T-kus s klapkou), netahá se myší
    compute_layout()

## rozměry haly, podlaha, úhly klapek – po načtení i po úpravě v editoru
func compute_layout() -> void :
    var mn: = Vector2(1000000000.0, 1000000000.0)
    var mx: = Vector2(-1000000000.0, -1000000000.0)
    for it in items:
        if it.has("pts"):
            for p in it.pts:
                mn = mn.min(Vector2(p[0], p[1]));mx = mx.max(Vector2(p[0], p[1]))
        elif it.has("x"):
            var p0: = Vector2(it.x, it.get("y", 0))
            var p1: = p0 + Vector2(it.get("w", 0), it.get("h", 0))
            mn = mn.min(p0 - Vector2(60, 60));mx = mx.max(p1 + Vector2(60, 60))
    bounds = Rect2(mn, mx - mn).grow(80)
    floor_y = -1e9
    for it in items:
        if it.type == "vessel" and it.get("shape") in ["tank", "lauter", "cone", "tall", "horiz"]:
            floor_y = maxf(floor_y, float(it.y) + float(it.get("h", 0)) + 38.0)
        elif it.type == "pump": floor_y = maxf(floor_y, float(it.y) + 30.0)
    if floor_y < bounds.position.y: floor_y = bounds.end.y - 200.0

    for v in items:
        if v.type == "valve": v["_ang"] = deg_to_rad(float(v.get("rot", 0)))


## kde stojí nohy nádoby: na stojanu (y+h+38), nebo dotažené na podlahu, když je blízko
func foot_y(it) -> float:
    var yd: = float(it.y) + float(it.get("h", 0)) + 38.0
    var gap: = floor_y - yd
    if gap >= -4.0 and gap < 30.0: return floor_y - 1.0
    return yd

func vp_size() -> Vector2:
    return get_viewport().get_visible_rect().size

func screen_to_world(sp: Vector2) -> Vector2:
    return cam.position + (sp - vp_size() * 0.5) / cam.zoom.x

func fit_all() -> void :
    go_to(bounds)


var cam_anim: = false
var cam_to_p: = Vector2.ZERO
var cam_to_z: = 1.0
func go_to(rect: Rect2, smooth: = false) -> void :
    var s: = vp_size(); var pw: = right_w()
    var z: = clampf(minf((s.x - pw) / rect.size.x, (s.y - 60) / rect.size.y), 0.03, 4.0)
    var p: = rect.get_center() + Vector2(pw * 0.5, -30.0) / z
    zoom_anim = false;vel = Vector2.ZERO
    if smooth and game != null:
        cam_to_p = p;cam_to_z = z;cam_anim = true;return
    zoom_target = z;cam.zoom = Vector2(z, z);cam.position = p;cam_anim = false

func zoom_at(factor: float, screen_pos: Vector2) -> void :
    cam_anim = false
    zoom_target = clampf(zoom_target * factor, 0.03, 4.0)
    zoom_anchor = screen_pos
    zoom_anim = true

func _process(delta: float) -> void :
    if hud == null or cam == null: return

    if zoom_anim:
        var z0: = cam.zoom.x
        var z1: = z0 * pow(zoom_target / z0, minf(1.0, delta * 14.0))
        if absf(log(zoom_target / z1)) < 0.002:
            z1 = zoom_target;zoom_anim = false
        var w0: = screen_to_world(zoom_anchor)
        cam.zoom = Vector2(z1, z1)
        var w1: = screen_to_world(zoom_anchor)
        cam.position += w0 - w1

    if not dragging and vel.length() > 2.0:
        cam.position -= vel * delta / cam.zoom.x
        vel *= pow(0.02, delta)

    var k: = Vector2(float(Input.is_key_pressed(KEY_RIGHT) or Input.is_key_pressed(KEY_D)) - float(Input.is_key_pressed(KEY_LEFT) or Input.is_key_pressed(KEY_A)), float(Input.is_key_pressed(KEY_DOWN) or Input.is_key_pressed(KEY_S)) - float(Input.is_key_pressed(KEY_UP) or Input.is_key_pressed(KEY_W)))
    if k != Vector2.ZERO: cam_anim = false
    if cam_anim:
        var a: = minf(1.0, delta * 7.0)
        var zz: float = exp(lerpf(log(cam.zoom.x), log(cam_to_z), a))
        cam.zoom = Vector2(zz, zz);cam.position = cam.position.lerp(cam_to_p, a)
        if absf(log(zz / cam_to_z)) < 0.003 and cam.position.distance_to(cam_to_p) < 1.0:
            cam.zoom = Vector2(cam_to_z, cam_to_z);cam.position = cam_to_p;cam_anim = false
        zoom_target = cam.zoom.x
    if k != Vector2.ZERO:
        var sp: = 900.0 * (2.5 if Input.is_key_pressed(KEY_SHIFT) else 1.0)
        cam.position += k * sp * delta / cam.zoom.x
    hud.update_info()

    if not running and sim != null and sim.S.sk.hyd != 0.0:
        # hydraulika kopačky jde ovládat i při pauze (reálný čas, zdvih ~15 s)
        sim.S.sk.rakeH = clampf(sim.S.sk.rakeH + sim.S.sk.hyd * delta / 15.0, 0.0, 1.0)
    if running and sim != null and item_layer != null:
        var dts: = clampf(speed / 30.0, 0.25, 6.0)
        sim_acc += minf(delta, 0.1) * speed
        var n: = 0
        var _ts: = Time.get_ticks_usec()

        while sim_acc >= dts and n < 6 and Time.get_ticks_usec() - _ts < 8000:
            var _c: = Time.get_ticks_usec()
            if not NOSIM:
                auto.tick(dts);sim.sim_step(dts)
                if practice != null and not auto.on: practice.tick(dts)
            T_sim += Time.get_ticks_usec() - _c;N_sim += 1
            sim_acc -= dts;n += 1
        if sim_acc > dts * 6: sim_acc = 0.0
        dyn_t += delta
        if dyn_t >= (0.05 if speed < 60.0 else 0.1):
            dyn_t = 0.0
            var _a: = Time.get_ticks_usec(); if not NOLIVE: update_live()
            T_live += Time.get_ticks_usec() - _a
    elif zoom_anim:
        update_live()
    save_acc += delta
    if save_acc > 20.0 and running:
        save_acc = 0.0;save_game()
    if flow_layer != null:
        flow_t += delta
        if flow_t > (0.07 if speed < 60.0 else 0.12):
            flow_t = 0.0
            calm_t += 1
            if calm_t % 4 == 0: _calm_tick()
            var _b: = Time.get_ticks_usec(); if not NOFLOW: flow_layer.refresh()
            T_flow += Time.get_ticks_usec() - _b
    _detail_tick(delta)
    if cabinet != null and not running: cabinet.upd()


## vyhlazení hran celé scény (FXAA): Compatibility renderer ve 2D nemá MSAA, takže hrany polygonů a obdélníků by byly zubaté
const AA_SHADER: = """
shader_type canvas_item;
uniform sampler2D scr : hint_screen_texture, filter_linear, repeat_disable;
void fragment() {
	vec2 px = SCREEN_PIXEL_SIZE;
	vec2 uv = SCREEN_UV;
	vec3 L = vec3(0.299, 0.587, 0.114);
	vec3 cM = texture(scr, uv).rgb;
	vec3 cNW = texture(scr, uv + vec2(-1.0, -1.0) * px).rgb;
	vec3 cNE = texture(scr, uv + vec2(1.0, -1.0) * px).rgb;
	vec3 cSW = texture(scr, uv + vec2(-1.0, 1.0) * px).rgb;
	vec3 cSE = texture(scr, uv + vec2(1.0, 1.0) * px).rgb;
	float lM = dot(cM, L); float lNW = dot(cNW, L); float lNE = dot(cNE, L); float lSW = dot(cSW, L); float lSE = dot(cSE, L);
	float lMin = min(lM, min(min(lNW, lNE), min(lSW, lSE)));
	float lMax = max(lM, max(max(lNW, lNE), max(lSW, lSE)));
	if (lMax - lMin < max(0.04, lMax * 0.12)) { COLOR = vec4(cM, 1.0); }
	else {
		vec2 dir;
		dir.x = -((lNW + lNE) - (lSW + lSE));
		dir.y = ((lNW + lSW) - (lNE + lSE));
		float red = max((lNW + lNE + lSW + lSE) * 0.25 * 0.125, 1.0 / 128.0);
		float rcp = 1.0 / (min(abs(dir.x), abs(dir.y)) + red);
		dir = clamp(dir * rcp, vec2(-6.0), vec2(6.0)) * px;
		vec3 a = 0.5 * (texture(scr, uv + dir * (1.0 / 3.0 - 0.5)).rgb + texture(scr, uv + dir * (2.0 / 3.0 - 0.5)).rgb);
		vec3 b = a * 0.5 + 0.25 * (texture(scr, uv + dir * -0.5).rgb + texture(scr, uv + dir * 0.5).rgb);
		float lB = dot(b, L);
		COLOR = vec4((lB < lMin || lB > lMax) ? a : b, 1.0);
	}
}
"""
var aa_layer: CanvasLayer

func _make_aa() -> void :
    hud.layer = 3
    aa_layer = CanvasLayer.new();aa_layer.layer = 2;add_child(aa_layer)
    var sh: = Shader.new();sh.code = AA_SHADER
    var mat: = ShaderMaterial.new();mat.shader = sh
    var cr: = ColorRect.new();cr.material = mat;cr.mouse_filter = Control.MOUSE_FILTER_IGNORE
    aa_layer.add_child(cr);cr.set_anchors_preset(Control.PRESET_FULL_RECT)

func toggle_3d(on = null) -> void :
    if v3 == null: return
    var want: bool = (not v3.active) if on == null else bool(on)
    if want and editor != null and editor.on: return
    v3.set_active(want)
    if aa_layer != null: aa_layer.visible = not want
    hud.toast("3D pohled (V = zpět na 2D, X = průhledné nádoby)" if want else "2D pohled")

func _unhandled_input(e: InputEvent) -> void :
    if e is InputEventKey and e.pressed and not e.echo and e.keycode == KEY_V and not (editor != null and editor.on):
        toggle_3d();return
    if v3 != null and v3.active:
        if v3.handle(e): return
        if e is InputEventMouseButton or e is InputEventMouseMotion: return
    if editor != null and editor.on and editor.handle(e): return
    if e is InputEventMouseButton:
        var mb: = e as InputEventMouseButton
        if (mb.button_index == MOUSE_BUTTON_WHEEL_UP or mb.button_index == MOUSE_BUTTON_WHEEL_DOWN) and mb.pressed and mb.ctrl_pressed:
            _valve_pct(screen_to_world(mb.position), (10 if mb.shift_pressed else 2) * (1 if mb.button_index == MOUSE_BUTTON_WHEEL_UP else -1))
        elif mb.button_index == MOUSE_BUTTON_WHEEL_UP and mb.pressed:
            zoom_at(1.18, mb.position)
        elif mb.button_index == MOUSE_BUTTON_WHEEL_DOWN and mb.pressed:
            zoom_at(1.0 / 1.18, mb.position)
        elif mb.button_index == MOUSE_BUTTON_LEFT and (hose_drag != null or (mb.pressed and _hose_grab(mb.position))):
            if not mb.pressed: _hose_drop()
        elif mb.button_index in [MOUSE_BUTTON_LEFT, MOUSE_BUTTON_RIGHT, MOUSE_BUTTON_MIDDLE]:
            if mb.pressed:
                if mb.button_index == MOUSE_BUTTON_LEFT and cabinet != null:
                    var hk: String = cabinet.hit(screen_to_world(mb.position))
                    if hk == "hup" or hk == "hdn":
                        cab_hold = hk;cabinet.act(hk, true);return
                vdrag = null
                if mb.button_index == MOUSE_BUTTON_LEFT and not (editor != null and editor.on):
                    vdrag = _valve_at(screen_to_world(mb.position))
                    if vdrag != null and not can_throttle(vdrag): vdrag = null   # ostatní klapky se netáhnou: tažení posouvá pohled, klik otevře/zavře
                    if vdrag != null:
                        vdrag_pct = float(vdrag.get("pct", 50)) if vdrag.get("kind", "") == "skrt" else float(vdrag.get("part", 100 if vdrag.get("open", true) != false else 0))
                dragging = true;drag_moved = false;drag_start = mb.position;vel = Vector2.ZERO
            else:
                if cab_hold != "":
                    cabinet.act(cab_hold, false);cab_hold = "";dragging = false;return
                dragging = false
                if not drag_moved and mb.button_index == MOUSE_BUTTON_LEFT:
                    _tap(screen_to_world(mb.position))
                elif not drag_moved and mb.button_index == MOUSE_BUTTON_RIGHT:
                    if not _valve_slider(screen_to_world(mb.position), mb.position): _throttle_step(screen_to_world(mb.position), -10)
                if Time.get_ticks_msec() - last_move_ms > 80:
                    vel = Vector2.ZERO
    elif e is InputEventMouseMotion and hose_drag != null:
        var mpos: = (e as InputEventMouseMotion).position
        if not hose_drag.moved and mpos.distance_to(hose_drag.start) > 5.0: hose_drag.moved = true
        if hose_drag.moved:
            var it = hose_drag.pipe; var idx: int = hose_drag.idx
            var w: = screen_to_world(mpos)
            var o = outlet_at(w, 26.0 / minf(1.0, cam.zoom.x))
            if o != null and hose_at(o.front) in [null, [it, idx]]: w = o.front
            var other_i: int = it.pts.size() - 1 - idx
            var other: = Vector2(it.pts[other_i][0], it.pts[other_i][1])
            if idx == 0: hose_shape(it, w, other)
            else: hose_shape(it, other, w)
            redraw()
    elif e is InputEventMouseMotion and dragging:
        var mm: = e as InputEventMouseMotion
        if not drag_moved and mm.position.distance_to(drag_start) > 5.0:
            drag_moved = true
        if drag_moved and vdrag != null:
            if not vdrag.get("stuck", false):
                vdrag_pct = clampf(vdrag_pct + (mm.relative.x - mm.relative.y) * (0.17 if mm.shift_pressed else 0.5), 0.0, 100.0)
                var pi_: int = roundi(vdrag_pct)
                var cur: int = int(vdrag.get("pct", 50)) if vdrag.get("kind", "") == "skrt" else int(vdrag.get("part", 100 if vdrag.get("open", true) != false else 0))
                if pi_ != cur:
                    _set_valve_pct(vdrag, pi_)
                    hud.toast("%s: otevřeno %d %% (táhni vpravo/nahoru = víc, Shift = jemně)" % [vdrag.get("label", "klapka"), pi_])
            return
        if drag_moved:
            cam_anim = false
            cam.position -= mm.relative / cam.zoom.x
            var dt: = maxf(0.001, (Time.get_ticks_msec() - last_move_ms) / 1000.0)
            vel = vel.lerp(mm.relative / dt, 0.35)
            last_move_ms = Time.get_ticks_msec()
    elif e is InputEventMagnifyGesture:
        zoom_at((e as InputEventMagnifyGesture).factor, (e as InputEventMagnifyGesture).position)
    elif e is InputEventPanGesture:
        cam.position += (e as InputEventPanGesture).delta * 30.0 / cam.zoom.x
    elif e is InputEventKey and e.pressed:
        match (e as InputEventKey).keycode:
            KEY_PLUS, KEY_KP_ADD, KEY_EQUAL: zoom_at(1.25, vp_size() * 0.5)
            KEY_MINUS, KEY_KP_SUBTRACT: zoom_at(0.8, vp_size() * 0.5)
            KEY_HOME: fit_all()
            KEY_SPACE:
                running = not running;hud.toast("Čas várky: " + ("BĚŽÍ" if running else "POZASTAVEN"))
                if game != null: game.play("click")
            KEY_1, KEY_2, KEY_3, KEY_4:
                speed = [1.0, 10.0, 60.0, 180.0][(e as InputEventKey).keycode - KEY_1];running = true;hud.toast("Rychlost x%d" % int(speed))
            KEY_ENTER, KEY_KP_ENTER:
                if game != null: game.confirm()
            KEY_Q:
                if game != null: game.toggle_quest()
            KEY_TAB: toggle_panel()
            KEY_J: toggle_hard()
            KEY_R: toggle_dock()
            KEY_Z:
                if (e as InputEventKey).ctrl_pressed: undo()
                else: toggle_dock_big()
            KEY_N:
                if game != null: game.toggle_mute()
            KEY_F1, KEY_H:
                if game != null: game.toggle_help()
            KEY_ESCAPE:
                if game != null and game.help.visible: game.toggle_help()
            KEY_F5: hud._go("rvp")
            KEY_F6: hud._go("sk")
            KEY_F7: hud._go("wp")
            KEY_F8: hud._go("hlt")
            KEY_F9: hud._go("spilka")
            KEY_E: open_editor()
            KEY_M:
                if mini != null:
                    mini.visible = not mini.visible
                    hud.toast("Mapka: " + ("zobrazena" if mini.visible else "skrytá (M ji vrátí)"))


const SAVE_PATH: = "user://varka.sav"
const PLAN_REV: = 3
var save_path: = SAVE_PATH
var save_acc: = 0.0

func save_game() -> void :
    if sim == null or sim.net.is_empty(): return
    var iv: = {}
    for it in items:
        match it.type:
            "valve":
                iv[it.id] = {"open": it.get("open", true)}
                if it.get("kind", "") == "skrt": iv[it.id]["pct"] = it.get("pct", 50)
                if it.has("part"): iv[it.id]["part"] = it.part
            "pipe":
                if it.get("hose", false):
                    iv[it.id] = {"pts": it.pts}
                    if it.get("user", false): iv[it.id] = {"pts": it.pts, "user": true, "dn": it.get("dn", 40), "medium": it.get("medium", "water"), "id": it.id, "type": "pipe", "hose": true}
            "sw": iv[it.id] = {"on": it.get("on", false)}
            "vessel":
                if it.get("shape") == "inlet": iv[it.id] = {"on": it.get("on", false)}
    var ec: = {}
    for e in sim.net.edges: ec[e.id] = {"c": e.c, "q": e.q, "from": e.from}
    var f: = FileAccess.open(save_path, FileAccess.WRITE)
    if f == null: return
    f.store_var({"v": 1, "S": sim.S, "items": iv, "outlets": outlet_open, "edges": ec, "speed": speed, "auto": auto.to_dict(), "running": running, "plan_rev": PLAN_REV})
    f.close()


func load_game() -> bool:
    if not FileAccess.file_exists(save_path): return false
    var f: = FileAccess.open(save_path, FileAccess.READ)
    if f == null: return false
    var d = f.get_var();f.close()
    if typeof(d) != TYPE_DICTIONARY or d.get("v", 0) != 1 or typeof(d.get("S")) != TYPE_DICTIONARY: return false
    var iv: Dictionary = d.items
    for uid in iv:
        if String(uid).begins_with("hose_user_") and iv[uid].get("user", false):
            var have: = false
            for it0 in items:
                if it0.id == uid: have = true
            if not have: items.append(iv[uid].duplicate(true))
    for it in items:
        var s = iv.get(it.id)
        if s == null: continue
        for k in s:
            if k == "pts" and d.get("plan_rev", 0) != PLAN_REV and not it.get("user", false): continue
            if k == "pts" and it.get("fixed", false): continue   # pevná hadice (spilka) se ze zálohy nepřesouvá
            if k == "part" and it.get("type", "") == "valve" and not can_throttle(it): continue   # přiškrcení mají jen 3 klapky
            if k == "pct" and it.get("type", "") == "valve" and it.get("kind", "") != "skrt": continue   # staré uložení: 50 % u všech klapek nepatří
            it[k] = s[k]
    for k in d.outlets:
        if outlet_open.has(k): outlet_open[k] = d.outlets[k]
    var had_mode: bool = d.S.sk.has("mode")
    sim.S = d.S
    var fresh = Sim.new();fresh.main = self;fresh.new_state()
    _merge_defaults(sim.S, fresh.S)
    var old_mu: bool = not sim.S.cfg.has("skMuP")
    _merge_defaults(sim.S.cfg, fresh.S.cfg)
    # starší uložení: viskozita sladiny se počítá přesněji (skMuP 0,047) → propustnost překalibrovaná na stejný referenční Δp
    if old_mu and absf(float(sim.S.cfg.skPerm) - 1e-11) < 1e-13: sim.S.cfg.skPerm = 1.5e-11
    # verze 12: zadržení v mlátě kalibrováno na údaj Vasila (700 kg → 40 hl vyrážky, ~12,5 °P)
    if absf(float(sim.S.cfg.get("skRetentionLkg", 1.0)) - 1.56) < 1e-06: sim.S.cfg.skRetentionLkg = 1.0
    _fix_parts(sim.S)   # verze 15: nová složka aBs (stabilní β-amyláza)
    sim.calib_applied = sim.apply_calibration(sim.S.cfg)   # kalibrace platí i pro rozjetou várku
    if sim.calib_applied.size(): sim.logm("Kalibrace z kalibrace.json (i pro uloženou várku): %s" % ", ".join(PackedStringArray(sim.calib_applied)))
    if sim.calib_rejected.size(): sim.logm("Kalibrace – odmítnuto: %s" % "; ".join(PackedStringArray(sim.calib_rejected)), "warn")
    if not sim.S.cab.get("estop", false): sim.S.cab.key = true   # klíč START na rozvaděči už není

    # starší uložení: přepočítat kalibraci lože na nový model (propustnost ~10 darcy)
    if float(sim.S.cfg.get("skPerm", 1e-11)) < 5e-12: sim.S.cfg.skPerm = 1.0e-11
    if not had_mode and sim.S.sk.g > 1: sim.S.sk.mode = "lauter"
    sim.build_net(items, plan.get("joints", []))
    for e in sim.net.edges:
        var s2 = d.edges.get(e.id)
        if s2 != null:
            e.c = s2.c;e.from = s2.from
            for k0 in sim.lp():
                if not e.c.has(k0): e.c[k0] = 0.0
            if s2.has("q"): e.q = s2.q
            elif sim.vol(e.c) > 1e-09: e.q = [e.c.duplicate()]
            for b in e.q:
                for k in sim.lp():
                    if not b.has(k): b[k] = 0.0
            for k in sim.lp():
                if not e.c.has(k): e.c[k] = 0.0
    speed = d.get("speed", 10.0)
    auto.S = sim.S
    if d.has("auto"): auto.from_dict(d.auto)
    return true


func show_route(src: String, dst: String, pass2: Array, do_open: bool, where: = -1, extra: = []) -> void :
    var vs = sim.route_valves(src, dst, pass2, where)
    if vs == null:
        hud.toast("Cesta ve schématu neexistuje (chybí trubka nebo čerpadlo).");return
    for ex in extra:
        if not vs.has(ex) and sim.net.byId.has(ex): vs.append(ex)
    var need: = []
    for id in vs:
        var open: bool = sim.valve_open(id)
        if do_open and not open:
            if outlet_open.has(id): outlet_open[id] = true
            elif sim.net.byId.has(id):
                sim.net.byId[id]["open"] = true
                if sim.net.byId[id].get("kind", "") == "skrt" and int(sim.net.byId[id].get("pct", 50)) == 0: sim.net.byId[id]["pct"] = 100
        elif not open: need.append(id)
    hl.clear()
    if do_open and src == "vodovod":
        for it in items:
            if it.type == "vessel" and it.get("bind") == "vodovod" and it.get("shape") == "inlet": it["on"] = true
    if do_open:
        sim.valve_ver += 1;hud.toast("Cesta otevřena (%d klapek)%s. Zavřené klapky mimo cestu zůstaly zavřené." % [vs.size(), ", spoušť řadu puštěna" if src == "vodovod" else ""])
    else:
        for id in need: hl[id] = true
        hud.toast("Cesta: otevři %d klapek (žluté kroužky)." % need.size() if need.size() else "Cesta je celá otevřená.")
    redraw()


## doplní nové složky do všech „dávek“ kapaliny (nádoby, buňky, FIFO) ve starém uložení
func _fix_parts(x) -> void :
    if x is Dictionary:
        if x.has("aB") and not x.has("aBs"): x["aBs"] = 0.0
        for k in x: _fix_parts(x[k])
    elif x is Array:
        for y in x: _fix_parts(y)

func _merge_defaults(dst: Dictionary, src: Dictionary) -> void :
    for k in src:
        if not dst.has(k): dst[k] = src[k]
        elif typeof(dst[k]) == TYPE_DICTIONARY and typeof(src[k]) == TYPE_DICTIONARY: _merge_defaults(dst[k], src[k])

func open_iodine(src: String) -> void :
    if iodine_win != null and is_instance_valid(iodine_win): iodine_win.queue_free()
    iodine_win = Iodine.new();iodine_win.m = self;hud.add_child(iodine_win);iodine_win.open(src)

func open_sight_c2() -> void :
    for it0 in items:
        if it0.id == "inl_mur9u2hg":
            if sight_win != null and is_instance_valid(sight_win): sight_win.queue_free()
            sight_win = Sight.new();sight_win.m = self;hud.add_child(sight_win);sight_win.open(it0);return

func new_game() -> void :
    DirAccess.remove_absolute(ProjectSettings.globalize_path(save_path))
    get_tree().reload_current_scene()

func _notification(what: int) -> void :
    if what == NOTIFICATION_WM_CLOSE_REQUEST: save_game()

func _hose_grab(sp: Vector2) -> bool:
    var w: = screen_to_world(sp)
    var h = hose_at(w, 22.0 / minf(1.0, cam.zoom.x), true)
    if h == null: return false
    hose_drag = {"pipe": h[0], "idx": h[1], "start": sp, "moved": false, "orig": h[0].pts.duplicate(true)}
    vel = Vector2.ZERO
    return true

func _hose_drop() -> void :
    var d = hose_drag;hose_drag = null
    if not d.moved:
        _tap(screen_to_world(d.start));return
    var it = d.pipe; var idx: int = d.idx
    var end: = Vector2(it.pts[idx][0], it.pts[idx][1])
    var o = outlet_at(end, 2.0)
    if o != null:
        hud.toast("Hadice nasazena na vývod – klapka vývodu je %s." % ("otevřená" if outlet_open[o.key] else "ZAVŘENÁ (ťukni na ni)"))
    else:
        hud.toast("Konec hadice visí volně – co do ní poteče, vyteče na zem!")

    sim.build_net(items, plan.get("joints", []))
    if flow_layer != null: flow_layer.build()
    redraw()



func _throttle_step(w: Vector2, step: int) -> void :
    var best = null
    var bd: = maxf(18.0, minf(40.0, 26.0 / cam.zoom.x)) if cam != null else 18.0
    for it in items:
        if it.type == "valve" and it.get("kind", "") == "skrt":
            var d: = w.distance_to(Vector2(it.x, it.y))
            if d < bd:
                bd = d;best = it
    if best == null: return
    var pct: = int(best.get("pct", 50))
    if step > 0: pct = 0 if pct >= 100 else mini(100, pct + step)
    else: pct = maxi(0, pct + step)
    best["pct"] = pct;best["open"] = pct > 0
    if sim.net.has("byId") and sim.net.byId.has(best.id): sim.net.byId[best.id]["pct"] = pct;sim.net.byId[best.id]["open"] = pct > 0
    sim.valve_ver += 1
    redraw()
    hud.toast("%s: otevřeno %d %% (levé tlačítko +10 %%, pravé -10 %%)" % [best.get("label", "škrtidlo"), pct])

## Ctrl + kolečko na klapce: mezipoloha po 10 % (mění odpor trasy a průtok)
func _valve_pct(w: Vector2, step: int) -> void:
    var best = null
    var bd: = maxf(18.0, minf(40.0, 26.0 / cam.zoom.x)) if cam != null else 18.0
    for it in items:
        if it.type == "valve" and it.get("kind", "") in ["klapka", "belimo", "kulovy"]:
            var d: = w.distance_to(Vector2(it.x, it.y))
            if d < bd:
                bd = d;best = it
    if best == null:
        _throttle_step(w, step);return
    if best.get("stuck", false):
        hud.toast("%s je zadřená – nejde přestavit." % best.get("label", "Klapka"));return
    if not can_throttle(best):
        hud.toast("Tahle klapka je jen otevřeno / zavřeno (klik).");return
    var pct: int = int(best.get("part", 100 if best.get("open", true) != false else 0))
    _set_valve_pct(best, clampi(pct + step, 0, 100))
    hud.toast("%s: otevřeno %d %% (Ctrl + kolečko 2 %, Ctrl+Shift 10 %, pravé tlačítko = posuvník po 1 %)" % [best.get("label", "klapka"), int(best.get("part", 100 if best.get("open", true) != false else 0))])

func _set_valve_pct(best, pct: int) -> void:
    if best.get("kind", "") == "skrt":
        best["pct"] = pct;best["open"] = pct > 0
        if sim.net.has("byId") and sim.net.byId.has(best.id): sim.net.byId[best.id]["pct"] = pct;sim.net.byId[best.id]["open"] = pct > 0
        sim.valve_ver += 1;redraw_valves();return
    best["open"] = pct > 0
    if pct >= 100 or pct <= 0: best.erase("part")
    else: best["part"] = pct
    if sim.net.has("byId") and sim.net.byId.has(best.id):
        sim.net.byId[best.id]["open"] = pct > 0
        if pct >= 100 or pct <= 0: sim.net.byId[best.id].erase("part")
        else: sim.net.byId[best.id]["part"] = pct
    sim.valve_ver += 1
    redraw_valves()

## přiškrcovat (plynule 0–100 %) jdou jen tyto 3 klapky; všechny ostatní jsou jen otevřeno / zavřeno
const THROTTLE_IDS: = ["valve_muraqe36", "valve_murbq6e1", "valve_murciy3u"]   # škrtidlo scezování u SK, ventil u čerpadla C4, Belimo
func can_throttle(it) -> bool:
    return it != null and THROTTLE_IDS.has(str(it.get("id", "")))

var valve_pop: PanelContainer = null
var vdrag = null
var vdrag_pct: = 0.0
func _valve_at(w: Vector2):
    var best = null
    var bd: = maxf(14.0, minf(30.0, 20.0 / cam.zoom.x)) if cam != null else 14.0
    for it in items:
        if it.type == "valve" and it.get("kind", "") in ["klapka", "belimo", "kulovy", "skrt"]:
            var d: = w.distance_to(Vector2(it.x, it.y))
            if d < bd:
                bd = d;best = it
    return best
## pravé tlačítko na klapce: posuvník libovolného otevření 0–100 % (škrcení průtoku)
func _valve_slider(w: Vector2, spos: Vector2) -> bool:
    var best = null
    var bd: = maxf(18.0, minf(40.0, 26.0 / cam.zoom.x)) if cam != null else 18.0
    for it in items:
        if it.type == "valve" and it.get("kind", "") in ["klapka", "belimo", "kulovy", "skrt"]:
            var d: = w.distance_to(Vector2(it.x, it.y))
            if d < bd:
                bd = d;best = it
    if best == null: return false
    if not can_throttle(best): return false
    if best.get("stuck", false):
        hud.toast("%s je zadřená – nejde přestavit." % best.get("label", "Klapka"));return true
    if valve_pop != null and is_instance_valid(valve_pop): valve_pop.queue_free()
    var pc: = PanelContainer.new();valve_pop = pc
    var sb: = StyleBoxFlat.new();sb.bg_color = Color(0.07, 0.09, 0.11, 0.96);sb.border_color = Color("f2c230");sb.set_border_width_all(2);sb.set_corner_radius_all(8);sb.set_content_margin_all(10)
    pc.add_theme_stylebox_override("panel", sb)
    var v: = VBoxContainer.new();pc.add_child(v)
    var lb: = Label.new();v.add_child(lb)
    var sl: = HSlider.new();sl.min_value = 0;sl.max_value = 100;sl.step = 1;sl.custom_minimum_size = Vector2(220, 24)
    sl.value = int(best.get("pct", 50)) if best.get("kind", "") == "skrt" else int(best.get("part", 100 if best.get("open", true) != false else 0));v.add_child(sl)
    var lab0: String = str(best.get("label", "klapka"))
    lb.text = "%s – otevřeno %d %%" % [lab0, int(sl.value)]
    sl.value_changed.connect(func(x: float):
        _set_valve_pct(best, int(x));lb.text = "%s – otevřeno %d %%" % [lab0, int(x)])
    var cb: = Button.new();cb.text = "hotovo";cb.pressed.connect(func(): pc.queue_free());v.add_child(cb)
    hud.add_child(pc);sl.grab_focus()
    pc.position = (spos + Vector2(14, 14)).min(get_viewport().get_visible_rect().size - Vector2(250, 120))
    return true

func malt_gate_pos(it) -> Vector2:
    return Vector2(it.x + 36.0, it.y - 16.0)


func _say(t: String) -> void :
    if hud != null: hud.toast(t)

func _shift_hose(w: Vector2) -> bool:
    var zt: float = minf(1.0, cam.zoom.x) if cam != null else 1.0
    var hh = hose_at(w, 22.0 / zt, true)
    if hh != null:
        var hp = hh[0]
        if hp.get("user", false):
            var oe = outlet_at(Vector2(hp.pts[0][0], hp.pts[0][1]), 2.0)
            items.erase(hp)
            if oe != null: outlet_open[oe.key] = hose_at(oe.front) != null
            sim.build_net(items, plan.get("joints", []))
            if flow_layer != null: flow_layer.build()
            redraw();_say("Přidaná hadice odebrána.");return true
        return false
    var o = outlet_at(w, 24.0 / zt)
    if o == null: return false
    if hose_at(o.front) != null: _say("Na tomto vývodu už hadice je.");return true
    var n: = 1
    for it in items:
        if it.id.begins_with("hose_user_"): n = maxi(n, int(it.id.substr(10)) + 1)
    var h: = {"id": "hose_user_%d" % n, "type": "pipe", "hose": true, "dn": 40, "medium": "water", "user": true, "pts": []}
    hose_shape(h, o.front, o.front + Vector2(24.0, 150.0))
    items.append(h)
    outlet_open[o.key] = true
    sim.build_net(items, plan.get("joints", []))
    if flow_layer != null: flow_layer.build()
    redraw();_say("Přidána hadice na vývod katety (vývod otevřen). Volný konec přetáhni myší na jiný vývod; Shift + klepnutí na konec ji odebere.")
    return true

## kam se dá z klapky dostat otevřenými klapkami (pro Alt náhled): čáry trubek + názvy nádob
var _vnode: = {}
func flow_reach(vid: String) -> Array:
    if _vnode.is_empty():
        for k in sim.net.nodes:
            var n = sim.net.nodes[k]
            if n.valve != null: _vnode[n.valve] = k
    if not _vnode.has(vid) or not sim.valve_open(vid): return []
    var r = sim.bfs([_vnode[vid]])
    var out: = []; var seen: = {}
    for d in r.hits:
        for st in sim.path_to(r, d.k):
            if st.has("e") and not seen.has(st.e.id):
                seen[st.e.id] = true
                var pl: = PackedVector2Array()
                for p in st.e.pts: pl.append(Vector2(p.x, p.y) if p is Vector2 else Vector2(p[0], p[1]))
                out.append(pl)
    return out

func flow_targets(vid: String) -> String:
    if _vnode.is_empty(): flow_reach("")
    if not _vnode.has(vid) or not sim.valve_open(vid): return ""
    var r = sim.bfs([_vnode[vid]]); var names: = {}
    for d in r.hits: names[VNAME.get(d.att.bind, d.att.bind)] = true
    if r.leaks.size(): names["⚠ otevřený konec"] = true
    return ", ".join(PackedStringArray(names.keys()))
const VNAME: = {"rvp": "pánev", "sk": "SK", "wp": "vířivka", "hlt": "bojler", "lv": "ledovka", "spilka": "spilka", "vodovod": "řad", "tanky": "tanky", "mp": "mladinová pánev"}

## vrátit poslední zásah (Ctrl+Z)
var undo_stack: = []
func undo() -> void :
    if undo_stack.is_empty():
        hud.toast("Není co vrátit.");return
    var u: Dictionary = undo_stack.pop_back()
    match u.t:
        "valve":
            for it in items:
                if it.id == u.id:
                    it["open"] = u.open
                    if u.has("part"): it["part"] = u.part
                    else: it.erase("part")
            sim.valve_ver += 1;redraw_valves();hud.toast("Vráceno: %s je zase %s" % [u.get("label", "klapka"), "otevřená" if u.open else "zavřená"])
        "outlet":
            outlet_open[u.id] = u.open;sim.valve_ver += 1;redraw();hud.toast("Vráceno: vývod katety")
        "pump":
            toggle_pump(u.b);undo_stack.pop_back()
    if game != null: game.play("click")

## co je pod myší (pro zvýraznění a popisek) – nic nemění
func pick(w: Vector2) -> Dictionary:
    var S = sim.S
    var bd: = maxf(18.0, minf(40.0, 26.0 / cam.zoom.x))
    var best = null
    for it in items:
        if it.type == "valve":
            var d: = w.distance_to(Vector2(it.x, it.y))
            if d < bd: bd = d;best = it
    if best != null:
        var open: bool = sim.valve_open(best.id)
        var nm: String = best.get("label", "klapka")
        var t: String
        if best.get("kind", "") == "skrt": t = "%s · otevřeno %d %%\nlevé tlačítko +10 %%, pravé −10 %%" % [nm, int(best.get("pct", 50))]
        elif best.get("kind", "") == "zasl": t = "%s · %s" % [nm, "otevřená" if open else "zavřená"]
        else:
            if can_throttle(best): t = "%s · %s%s\nklik = %s · tažení myší = přiškrcení · pravé tl. = posuvník" % [nm, "OTEVŘENÁ" if open else "zavřená", (" %d %%" % int(best.part)) if best.has("part") else "", "zavřít" if open else "otevřít"]
            else: t = "%s · %s\nklik = %s" % [nm, "OTEVŘENÁ" if open else "zavřená", "zavřít" if open else "otevřít"]
            if open:
                var ft: String = flow_targets(best.id)
                if ft != "": t += "\nvede do: %s  (podrž Alt = ukázat trasu)" % ft
        if best.get("stuck", false): t += "\n⚠ zadřená"
        return {"key": best.id, "pos": Vector2(best.x, best.y), "r": 20.0, "text": t, "act": true, "col": Color("4fd18b") if open else Color("ff5a4a")}
    var o = outlet_at(w, 20.0)
    if o != null:
        return {"key": o.key, "pos": o.front, "r": 16.0, "text": "Vývod katety · %s\nklik = %s · hadici přetáhni myší" % ["otevřen" if outlet_open[o.key] else "zavřen", "zavřít" if outlet_open[o.key] else "otevřít"], "act": true}
    for it in items:
        if it.type == "pump" and w.distance_to(Vector2(it.x, it.y)) < 30:
            var b: String = it.get("bind", "")
            var on: bool = (S[b].on if b in ["p1", "p2", "p3"] else (S.lv.pump if b == "plv" else (S.rvpWater.on or S.skWater.on))) if b != "" else false
            var t2: String = "%s · %s" % [pump_name(b), "BĚŽÍ" if on else "stojí"]
            if b in ["p1", "p2", "p3"]:
                t2 += " · %d L/min" % roundi(S[b].q if on else S[b].flow)
                if on and S[b].status != "": t2 += "\n" + S[b].status
            t2 += "\nklik = " + ("zastavit" if on else "spustit")
            return {"key": "pump" + b, "pos": Vector2(it.x, it.y), "r": 32.0, "text": t2, "act": true, "col": Color("4fd18b") if on else Color("5aa9ff")}
    for it in items:
        if it.type == "vessel" and it.get("shape") == "inlet" and w.distance_to(Vector2(it.x + it.w * 0.5, it.y + it.h * 0.5)) < maxf(it.w, it.h) * 0.5 + 16.0:
            return {"key": "inlet", "pos": Vector2(it.x + it.w * 0.5, it.y + it.h * 0.5), "r": maxf(it.w, it.h) * 0.5 + 6.0, "text": "Spoušť vody z řadu · %s\nklik = %s" % ["PUŠTĚNA" if it.get("on", false) else "zavřená", "zavřít" if it.get("on", false) else "pustit"], "act": true}
        if it.type == "sw" and Rect2(it.x - 26, it.y - 34, 52, 68).has_point(w):
            return {"key": "sw" + str(it.get("id", "")), "rect": Rect2(it.x - 26, it.y - 34, 52, 68), "text": "%s · %s\nklik = přepnout" % [it.get("label", "spínač"), "ZAP" if it.get("on", false) else "vyp"], "act": true}
    for ith in items:
        if ith.type == "deco" and ith.get("k", "") == "splid" and Rect2(ith.x - 44.0, ith.y - 46.0, 88.0, 60.0).has_point(w):
            return {"key": "splid", "rect": Rect2(ith.x - 44.0, ith.y - 46.0, 88.0, 60.0), "text": "Víčko spilky · %s\nmusí být OTEVŘENÉ, jinak se tank tlakuje a mladina netěče\nklik = přepnout" % ("OTEVŘENÉ" if S.sp.get("lidOpen", false) else "zavřené"), "act": true, "col": Color("f2c230")}
        if ith.type == "deco" and ith.get("k", "") == "hatch" and Rect2(ith.x - 46.0, ith.y - 8.0, 92.0, 52.0).has_point(w):
            return {"key": "hatch", "rect": Rect2(ith.x - 46.0, ith.y - 8.0, 92.0, 52.0), "text": "Výhoz mláta · %s\nklik = přepnout" % ("OTEVŘEN" if S.sk.get("hatch", false) else "zavřen"), "act": true, "col": Color("f2c230")}
        if ith.type == "deco" and ith.get("k", "") == "conv" and w.distance_to(malt_gate_pos(ith)) < 30.0:
            return {"key": "gate", "pos": malt_gate_pos(ith), "r": 24.0, "text": "Šoupě násypky · %s\nklik = přepnout" % ("otevřeno" if S.malt.get("gate", false) else "zavřeno"), "act": true, "col": Color("f2c230")}
    if cabinet != null and cabinet.hit(w) != "": return {"key": "cab", "text": "", "act": true}
    for it in items:
        if it.type == "vessel" and it.get("bind") in ["rvp", "sk", "wp", "hlt", "lv", "spilka"] and Rect2(it.x, it.y, it.w, it.h).has_point(w):
            return {"key": "v" + it.bind, "rect": Rect2(it.x, it.y, it.w, it.h), "text": "%s\n%s" % [it.get("label", ""), vessel_text(it.bind)], "act": false, "col": Color(1, 1, 1, 0.5)}
    return {}

func _tap(w: Vector2) -> void :
    if game != null:
        game.hover.ripple(w)
        var pk: Dictionary = pick(w)
        if pk.get("act", false): game.play("valve" if pk.key.begins_with("valve") or pk.key.begins_with("manifold") else "click")
    if Input.is_key_pressed(KEY_SHIFT) and _shift_hose(w): return
    if false and cabinet != null:
        var ck: String = cabinet.hit(w)
        if ck != "":
            if ck != "hup" and ck != "hdn": cabinet.act(ck, true)
            return
    for ith in items:
        if ith.type == "deco" and ith.get("k", "") == "splid" and Rect2(ith.x - 44.0, ith.y - 46.0, 88.0, 60.0).has_point(w):
            sim.S.sp["lidOpen"] = not sim.S.sp.get("lidOpen", false)
            _say("Víčko spilky: %s" % ("OTEVŘENÉ – tank se netlakuje, mladina může téct dovnitř" if sim.S.sp.lidOpen else "zavřené – při plnění se tank bude tlakovat!"))
            redraw();return
        if ith.type == "deco" and ith.get("k", "") == "hatch" and Rect2(ith.x - 46.0, ith.y - 8.0, 92.0, 52.0).has_point(w):
            sim.S.sk["hatch"] = not sim.S.sk.get("hatch", false)
            _say("Výhoz mláta: %s" % ("OTEVŘEN – kapalina odtéká do kanálu, mláto vypadává (s kopačkou rychleji)" if sim.S.sk.hatch else "zavřen"))
            redraw();return
    for itg in items:
        if itg.type == "deco" and itg.get("k", "") == "conv" and w.distance_to(malt_gate_pos(itg)) < 30.0:
            sim.S.malt.gate = not sim.S.malt.get("gate", false)
            hud.toast("Šoupě násypky: %s" % ("otevřeno – slad padá, když je zapnuté šrotování" if sim.S.malt.gate else "zavřeno"))
            redraw();return
    if hard:
        for dm in items:
            if dm.type != "dip": continue
            var DM: Dictionary = dip_geo(dm)
            if Rect2(DM.x + 24.0, DM.y + DM.h * 0.5 - 13.0, 54.0, 26.0).has_point(w):
                measure_dip(dm);return
        for gm in items:
            if gm.type != "gauge": continue
            var GM: Dictionary = gauge_geo(gm)
            if Rect2(GM.x + 14.0, (GM.y1 + GM.y2) * 0.5 - 13.0, 54.0, 26.0).has_point(w):
                measure_gauge(gm);return
    for gk in items:
        if gk.type != "gauge": continue
        var GG: Dictionary = gauge_geo(gk)
        var spots: = {}
        if GG.free:
            spots = {"top": Vector2(GG.x, GG.y1 - 18.0), "bot": Vector2(GG.x, GG.y2 + 18.0)}
        else:
            var mx: float = (GG.cx + GG.x) / 2.0
            spots = {"top": Vector2(mx, GG.y1), "bot": Vector2(mx, GG.y2)}
        for key in spots:
            if w.distance_to(spots[key]) < 16.0:
                gk[key] = not (gk.get(key, true) != false)
                sim.valve_ver += 1
                hud.toast("Stavoznak: %s kohout %s" % ["horní" if key == "top" else "dolní", "otevřen" if gk.get(key, true) != false else "zavřen"])
                redraw();update_live();return
    for it0 in items:
        if it0.type == "inl" and it0.get("k", "") == "pruh" and w.distance_to(Vector2(it0.x, it0.y)) < 26.0:
            if sight_win != null and is_instance_valid(sight_win): sight_win.queue_free()
            sight_win = Sight.new();sight_win.m = self;hud.add_child(sight_win);sight_win.open(it0)
            return
    var o = outlet_at(w, 20.0)
    if o != null:
        undo_stack.append({"t": "outlet", "id": o.key, "open": outlet_open[o.key]})
        outlet_open[o.key] = not outlet_open[o.key]
        sim.valve_ver += 1
        redraw()
        hud.toast("Vývod katety: %s%s" % ["otevřen" if outlet_open[o.key] else "zavřen", "" if hose_at(o.front) != null else " (bez hadice – teklo by na zem)"])
        return
    var best = null
    var bd: = maxf(18.0, minf(40.0, 26.0 / cam.zoom.x)) if cam != null else 18.0
    for it in items:
        if it.type == "valve":
            var d: = w.distance_to(Vector2(it.x, it.y))
            if d < bd:
                bd = d;best = it
    if best != null and best.get("kind", "") != "zasl":
        if best.get("stuck", false):
            hud.toast("%s nejde přestavit – páka drhne, klapka je zadřená (Nácvik poruch → Uvolnit klapku)." % best.get("label", "Klapka"))
            sim.logm("%s je zadřená – nejde přestavit." % best.get("label", "Klapka"), "warn");return
        if best.get("kind", "") == "skrt":
            _throttle_step(Vector2(best.x, best.y), 10);return
        else:
            var ue: = {"t": "valve", "id": best.id, "open": best.get("open", true) != false, "label": best.get("label", "klapka")}
            if best.has("part"): ue["part"] = best.part
            undo_stack.append(ue)
            if undo_stack.size() > 50: undo_stack.pop_front()
            best["open"] = not (best.get("open", true) != false)
            best.erase("part")
            if sim.net.byId.has(best.id) and sim.net.byId[best.id] != best:
                sim.net.byId[best.id]["open"] = best.open;sim.net.byId[best.id].erase("part")
        sim.valve_ver += 1
        redraw_valves()
        hud.toast("%s: %s" % [best.get("label", "klapka"), "otevřena" if best.get("open", true) != false else "zavřena"])
        return

    for it in items:
        if it.type == "pump" and w.distance_to(Vector2(it.x, it.y)) < 30:
            toggle_pump(it.get("bind", ""));return

    for it in items:
        if it.type == "vessel" and it.get("shape") == "inlet" and w.distance_to(Vector2(it.x + it.w * 0.5, it.y + it.h * 0.5)) < maxf(it.w, it.h) * 0.5 + 16.0:
            it["on"] = not it.get("on", false)
            hud.toast("Voda z řadu: %s" % ("PUŠTĚNA" if it.on else "zavřena"));redraw();return
        if it.type == "sw" and Rect2(it.x - 26, it.y - 34, 52, 68).has_point(w):
            it["on"] = not it.get("on", false)
            hud.toast("%s: %s" % [it.get("label", "spínač"), "ZAPNUT" if it.on else "vypnut"]);redraw();return


func cab_why() -> String:
    var c = sim.S.cab
    if c.estop: return "je stisknuté NOUZOVÉ STOP (uvolni ho)"
    if not c.main: return "hlavní vypínač je vypnutý"
    if not c.key: return "klíč na rozvaděči je na VYP (otoč na START)"
    return ""

func toggle_pump(b: String) -> void :
    var S = sim.S
    var on_now: bool = S[b].on if b in ["p1", "p2", "p3"] else (S.lv.pump if b == "plv" else (S.rvpWater.on or S.skWater.on))
    if not on_now and not sim.cab_live():
        _say("%s nejde pustit – rozvaděč je bez napětí: %s. Rozvaděč najdeš vlevo (tlačítko Rozvaděč nahoře) nebo v panelu ROZVADĚČ." % [pump_name(b), cab_why()])
        sim.logm("%s nejde pustit – rozvaděč je bez napětí (%s)." % [pump_name(b), cab_why()], "warn");redraw();return
    match b:
        "p1", "p2", "p3":
            if not S[b].on and S.cab.get("interlock", true):
                var why: String = sim.pump_interlock(b)
                if why != "":
                    _say("OCHRANA %s: %s" % [pump_name(b), why]);sim.logm("Ochrana nepustila %s: %s" % [pump_name(b), why], "warn");redraw();return
            S[b].on = not S[b].on;_say("%s: %s" % [pump_name(b), "START" if S[b].on else "stop"])
            if practice != null:
                if S[b].on: practice.on_pump_start(b)
                else: practice.on_pump_stop(b)
            undo_stack.append({"t": "pump", "b": b})
            if game != null: game.play("start" if S[b].on else "stop")
            if S[b].on:
                var wn: String = sim.pump_warn(b)
                if wn != "": sim.logm("%s: %s" % [pump_name(b), wn], "warn")
        "plv":
            S.lv.pump = not S.lv.pump;_say("%s (ledová voda): %s" % [pump_name(b), "START" if S.lv.pump else "stop"])
            if practice != null and not S.lv.pump: practice.on_pump_stop("plv")
        "phw":
            if S.rvpWater.on or S.skWater.on:
                S.rvpWater.on = false;S.skWater.on = false;_say("Čerpadlo horké vody: stop")
            else:
                S.rvpWater.on = true;_say("Čerpadlo horké vody: napouští pánev (%d L) – objem a vyslazování nastavíš vpravo" % roundi(S.rvpWater.target - S.rvpWater.added))
        _:
            _say("Čerpadlo nemá v simulaci roli.")
    if not running: _say("Čas várky stojí – nic neteče. Stiskni SPUSTIT (vpravo nahoře v panelu).")
    redraw()

func pump_name(b: String) -> String:
    for it in items:
        if it.type == "pump" and it.get("bind") == b: return it.get("label", b.to_upper())
    return b.to_upper()



func pump_primed(it) -> bool:
    var b: String = it.get("bind", "")
    return b in ["p1", "p2", "p3"] and not sim.S[b].on and sim.S[b].prime >= 0.95

func pump_state(it) -> int:
    var S = sim.S; var b: String = it.get("bind", "")
    var on: = false
    match b:
        "p1", "p2", "p3": on = S[b].on
        "plv": on = S.lv.pump
        "phw": on = S.rvpWater.on or S.skWater.on or S.get("flush", {}).get("on", false)
    if not on: return 0
    if b in ["p1", "p2", "p3"] and S[b].prime < 0.95: return 3
    if b == "phw": return 2 if (S._rvpIn or S._skTop or S._skLow) else 1
    for f in S.flows:
        if f.pump == it.id: return 2
    return 1


const BAKE_SCALE: = 1.0
const TILE: = 2048
var tiles_root: Node2D

func set_hall(dark: bool) -> void :
    hall_dark = dark
    var f: = FileAccess.open("user://vzhled.txt", FileAccess.WRITE)
    if f != null: f.store_string("tmava" if dark else "svetla")
    RenderingServer.set_default_clear_color(Color.WHITE)
    if detail_spr != null: detail_spr.visible = false
    detail_rect = Rect2();detail_sig = ""
    hud.toast("Překresluji halu…")
    await _bake()
    hud.toast("Hala: " + ("tmavá" if dark else "světlá"))

func _bake() -> void :
    if tiles_root: tiles_root.queue_free()
    tiles_root = Node2D.new();add_child(tiles_root);move_child(tiles_root, 1)
    var px: = bounds.size * BAKE_SCALE
    var nx: = int(ceil(px.x / TILE)); var ny: = int(ceil(px.y / TILE))
    for ty in ny:
        for tx in nx:
            var tw: = mini(TILE, int(ceil(px.x)) - tx * TILE); var th: = mini(TILE, int(ceil(px.y)) - ty * TILE)
            if tw <= 0 or th <= 0: continue
            var origin: = bounds.position + Vector2(tx * TILE, ty * TILE) / BAKE_SCALE
            var sv: = SubViewport.new()
            sv.size = Vector2i(tw, th);sv.transparent_bg = false;sv.render_target_update_mode = SubViewport.UPDATE_ONCE
            var sl: = StaticLayer.new();sl.m = self;sv.add_child(sl)
            var il: = ItemLayer.new();il.m = self;il.skip_valves = true;sv.add_child(il)
            add_child(sv)
            sv.canvas_transform = Transform2D(0.0, Vector2(BAKE_SCALE, BAKE_SCALE), 0.0, - origin * BAKE_SCALE)
            await RenderingServer.frame_post_draw
            await RenderingServer.frame_post_draw
            var img: = sv.get_texture().get_image()
            img.generate_mipmaps()
            var spr: = Sprite2D.new()
            spr.texture = ImageTexture.create_from_image(img)
            spr.centered = false;spr.position = origin;spr.scale = Vector2.ONE / BAKE_SCALE
            spr.texture_filter = CanvasItem.TEXTURE_FILTER_LINEAR_WITH_MIPMAPS
            tiles_root.add_child(spr)
            sv.queue_free()

var detail_spr: Sprite2D = null
var detail_rect: = Rect2()
var detail_scale: = 1.0
var detail_busy: = false
var detail_idle: = 0.0
var detail_sig: = ""

func visible_world_rect() -> Rect2:
    var a: = screen_to_world(Vector2.ZERO); var b: = screen_to_world(vp_size())
    return Rect2(a, Vector2.ZERO).expand(b)



func _detail_tick(delta: float) -> void :
    if editor != null and editor.on: return
    if tiles_root == null or cam == null or tiles_root.get_child_count() == 0: return
    var z: float = cam.zoom.x
    var sig: = "%d,%d,%.3f" % [roundi(cam.position.x * 2.0), roundi(cam.position.y * 2.0), z]
    if sig == detail_sig: detail_idle += delta
    else:
        detail_idle = 0.0;detail_sig = sig
    if z < 1.1:
        if detail_spr != null: detail_spr.visible = false
        return
    if detail_busy or detail_idle < 0.35: return
    var vr: = visible_world_rect()
    var ok: bool = detail_spr != null and detail_spr.visible and detail_rect.encloses(vr) and detail_scale >= minf(z, 3.0) * 0.85
    if not ok: _bake_detail(vr, z)

var detail_svs: = []
var detail_cur: = 0
## doostření výřezu: bez čtení obrázku zpět do CPU (to dělalo trhání) – dvě trvalé SubViewport, střídají se
func _bake_detail(vr: Rect2, z: float) -> void :
    detail_busy = true
    var t_start: = Time.get_ticks_msec()
    var grow: = vr.grow_individual(vr.size.x * 0.25, vr.size.y * 0.25, vr.size.x * 0.25, vr.size.y * 0.25)
    var sc: float = clampf(z, 1.0, 3.0)
    while grow.size.x * sc > 3000.0 or grow.size.y * sc > 2000.0: sc *= 0.9
    if detail_svs.is_empty():
        for i in 2:
            var v: = SubViewport.new();v.transparent_bg = false;v.render_target_update_mode = SubViewport.UPDATE_DISABLED
            add_child(v)
            var sl0: = StaticLayer.new();sl0.m = self;v.add_child(sl0)
            var il0: = ItemLayer.new();il0.m = self;il0.skip_valves = true;v.add_child(il0)
            detail_svs.append(v)
    var back: = 1 - detail_cur
    var sv: SubViewport = detail_svs[back]
    sv.size = Vector2i(ceili(grow.size.x * sc), ceili(grow.size.y * sc))
    sv.canvas_transform = Transform2D(0.0, Vector2(sc, sc), 0.0, - grow.position * sc)
    var sl: StaticLayer = sv.get_child(0); sl.clip = grow; sl.queue_redraw()
    var il: ItemLayer = sv.get_child(1); il.clip = grow; il.queue_redraw()
    sv.render_target_update_mode = SubViewport.UPDATE_ONCE
    await RenderingServer.frame_post_draw
    await RenderingServer.frame_post_draw
    if detail_spr == null:
        detail_spr = Sprite2D.new();detail_spr.centered = false
        detail_spr.texture_filter = CanvasItem.TEXTURE_FILTER_LINEAR
        add_child(detail_spr);move_child(detail_spr, tiles_root.get_index() + 1)
    detail_spr.texture = sv.get_texture()
    detail_spr.position = grow.position;detail_spr.scale = Vector2.ONE / sc;detail_spr.visible = true
    detail_rect = grow;detail_scale = sc;detail_cur = back
    detail_busy = false
    if "--detaildebug" in OS.get_cmdline_user_args(): print("DETAIL doostřeno %dx%d px, měřítko %.2f, %d ms" % [sv.size.x, sv.size.y, sc, Time.get_ticks_msec() - t_start])

func mf_ports(mf) -> Array:
    var g: = float(mf.get("gap", 60)); var R: = int(mf.get("rows", 3)); var C: = int(mf.get("cols", 4))
    var side: String = mf.get("back", "top")
    var W: = C * g; var H: = R * g
    var x: float = mf.x; var y: float = mf.y
    var ctr: = Vector2(x + W * 0.5, y + H * 0.5); var rot: = deg_to_rad(float(mf.get("rot", 0)))
    var o: = []
    for r in R:
        for c in C:
            var nx: = x + g * 0.5 + c * g; var ny: = y + g * 0.5 + r * g
            var p: = Vector2.ZERO
            if side == "both":
                var t: = int(ceil(R / 2.0)); var up: = r < t; var cnt: = t if up else R - t; var j: = r if up else r - t
                p = Vector2(nx + (j - (cnt - 1) / 2.0) * minf(12.0, g / (cnt + 1)), y - 16 if up else y + H + 16)
            elif side == "top": p = Vector2(nx + (r - (R - 1) / 2.0) * minf(12.0, g / (R + 1)), y - 16)
            elif side == "bottom": p = Vector2(nx + (r - (R - 1) / 2.0) * minf(12.0, g / (R + 1)), y + H + 16)
            elif side == "left": p = Vector2(x - 16, ny + (c - (C - 1) / 2.0) * minf(12.0, g / (C + 1)))
            else: p = Vector2(x + W + 16, ny + (c - (C - 1) / 2.0) * minf(12.0, g / (C + 1)))
            var f: = (Vector2(nx, ny) - ctr).rotated(rot) + ctr
            var b: = (p - ctr).rotated(rot) + ctr
            o.append({"front": f.round(), "back": b.round(), "key": "%s_%d" % [mf.id, o.size()]})
    return o

var outlets: Array = []
var outlet_open: = {}
var hose_drag = null

var pipe_pts: = {}

func _init_outlets() -> void :
    outlets.clear()
    for q in items:
        if q.type == "pipe" and not q.get("hose", false):
            for p in q.pts: pipe_pts[Vector2i(int(p[0]), int(p[1]))] = true
    for it in items:
        if it.type == "manifold": outlets.append_array(mf_ports(it))
    for o in outlets:
        outlet_open[o.key] = hose_at(o.front) != null


func hose_at(p: Vector2, tol: = 6.0, grab: = false):
    for it in items:
        if it.type == "pipe" and it.get("hose", false) and not (grab and it.get("fixed", false)):   # pevná hadice je napojená na vývod, jen se netahá myší
            for idx in [0, it.pts.size() - 1]:
                if Vector2(it.pts[idx][0], it.pts[idx][1]).distance_to(p) <= tol: return [it, idx]
    return null

func outlet_at(p: Vector2, tol: float):
    var best = null; var bd: = tol
    for o in outlets:
        var d: float = o.front.distance_to(p)
        if d < bd: bd = d;best = o
    return best


func hose_shape(it, a: Vector2, b: Vector2) -> void :
    var L: = maxf(1.0, a.distance_to(b)); var mid: = (a + b) * 0.5
    var n: = Vector2( - (b.y - a.y), b.x - a.x) / L
    if n.y < 0: n = - n
    var bow: = minf(90.0, L * 0.35)
    var pts: = [a, mid + n * bow * 0.7 - (b - a) * 0.25, mid + n * bow, mid + n * bow * 0.7 + (b - a) * 0.25, b]
    var arr: = []
    for q in pts: arr.append([roundf(q.x), roundf(q.y)])
    it["pts"] = arr


func gauge_geo(it) -> Dictionary:
    var v = null
    for q in items:
        if q.id == it.get("vid", "-"): v = q
    if v == null:
        return {"x": float(it.x), "y1": float(it.y), "y2": float(it.y) + float(it.get("h", 200)), "cx": float(it.x), "free": true, "v": null}
    var right: bool = it.get("side", "R") != "L"
    return {"x": v.x + v.w + 22.0 if right else v.x - 22.0, "y1": v.y + minf(30.0, v.h * 0.1), "y2": v.y + v.h - minf(20.0, v.h * 0.06), "cx": (v.x + v.w) if right else float(v.x), "free": false, "v": v}


func gauge_vessel(it):
    var g: = gauge_geo(it)
    if not g.free: return g.v
    return sim.gauge_vessel_free(it)


func vgeo(it) -> Array:
    var shape: String = it.get("shape", "tank"); var bind = it.get("bind")
    if shape == "tank" and bind == "rvp": return [minf(it.h * 0.12, 40.0), 20.0]
    if shape == "cone" and bind != "wp": return [minf(it.h * 0.25, 60.0), 16.0]
    return [0.0, float(it.w)]


func vessel_inside(it) -> PackedVector2Array:
    var x: float = it.x + 4; var w: float = it.w - 8; var y: float = it.y + 2; var y0: float = it.y + it.h - 3
    if it.get("shape") == "horiz":
        var rr: float = it.h * 0.5; var pts: = PackedVector2Array()
        for a in 13:
            var ang: = - PI * 0.5 + PI * a / 12.0
            pts.append(Vector2(it.x + it.w - rr + cos(ang) * (rr - 3), it.y + rr + sin(ang) * (rr - 3)))
        for a in 13:
            var ang2: = PI * 0.5 + PI * a / 12.0
            pts.append(Vector2(it.x + rr + cos(ang2) * (rr - 3), it.y + rr + sin(ang2) * (rr - 3)))
        return pts
    var g: = vgeo(it); var c: float = g[0]; var bw: float = g[1]; var cx: float = it.x + it.w * 0.5
    if c > 0:
        return PackedVector2Array([Vector2(x, y), Vector2(x + w, y), Vector2(x + w, y0 - c), Vector2(cx + bw * 0.5 - 2, y0), Vector2(cx - bw * 0.5 + 2, y0), Vector2(x, y0 - c)])
    return PackedVector2Array([Vector2(x, y), Vector2(x + w, y), Vector2(x + w, y0), Vector2(x, y0)])


func level_y(it) -> float:
    var S = sim.S; var c = S.cfg; var b = it.get("bind"); var y0: float = it.y + it.h - 3; var H: float = it.h - 5
    var V: = 0.0; var cap: = 1.0
    match b:
        "rvp":
            var hl: float = sim.hotV(S.rvp) / 100.0; var cc: = minf(it.h * 0.12, 40.0)
            var px: float = cc * pow(maxf(0.0, hl) / 3.0, 1.0 / 3.0) if hl <= 3 else cc + (hl - 3.0) / 47.0 * (it.h - cc)
            return y0 - minf(px, H)
        "lv":
            var f: = clampf(S.lv.w / c.lvVol, 0.0, 1.0)

            var lo: = 0.0; var hi: = 1.0
            for i in 22:
                var mid: = (lo + hi) * 0.5; var th: = 2.0 * acos(1.0 - 2.0 * mid)
                if (th - sin(th)) / TAU < f: lo = mid
                else: hi = mid
            return it.y + it.h - 3 - lo * (it.h - 6)
        "sk": return y0 - minf(1.0, sim.skVolTotal() / c.skCap) * H
        "wp": V = sim.vol(S.wp);cap = c.wpCap
        "mp": V = sim.vol(S.mp);cap = c.mpCap
        "hlt": V = S.hlt.w;cap = c.hltCap
        "spilka": V = S.sp.L;cap = c.spCap
        _: return y0
    var g: = vgeo(it); var cn: float = g[0]
    var Vc: float = cap * (cn / 3.0) / ((it.h - cn) + cn / 3.0)
    var p2: = 0.0
    if cn > 0 and V <= Vc: p2 = cn * pow(maxf(0.0, V) / Vc, 1.0 / 3.0)
    else: p2 = cn + (V - Vc) / maxf(1.0, cap - Vc) * (it.h - cn)
    return y0 - clampf(p2, 0.0, H)

func wort_color(ebc: float) -> Color:
    var stops: = [[0.0, "f6e7a1"], [6.0, "efc463"], [12.0, "dc9a2e"], [25.0, "b5651d"], [50.0, "6e3412"], [100.0, "2e1206"]]
    for i in range(1, stops.size()):
        if ebc <= stops[i][0]:
            return Color(stops[i - 1][1]).lerp(Color(stops[i][1]), (ebc - stops[i - 1][0]) / (stops[i][0] - stops[i - 1][0]))
    return Color("2e1206")


func kind_color(kind: String, T: float, ebc: float) -> Color:
    match kind:
        "wort": return wort_color(ebc)
        "mash": return wort_color(ebc).lerp(Color("cdb68a"), 0.55)
        "ice": return Color("9fd8ff")
        "empty": return Color(0, 0, 0, 0)
    return Color("5aaee6").lerp(Color("e8743b"), clampf((T - 20.0) / 70.0, 0.0, 1.0))

func vessel_color(it) -> Color:
    var S = sim.S; var b = it.get("bind"); var col: Color
    match b:
        "rvp", "mp", "wp":
            var v = S[b];col = kind_color(sim.contentKind(v), v.T, v.col / maxf(1.0, sim.liqVol(v)))
            if sim.contentKind(v) == "wort" and (v.alpha + v.iso) > 0.5: col = col.lerp(Color("c4701c"), 0.14)   # mladina s chmelem je tmavší, jantarová
        "hlt": col = kind_color("water", S.hlt.T, 0.0)
        "lv": col = kind_color("ice" if S.lv.T < 6 else "water", S.lv.T, 0.0)
        "spilka": col = wort_color(S.sp.col / maxf(1.0, S.sp.L)) if S.sp.f + S.sp.n > 1 else kind_color("water", 20.0, 0.0)
        "sk":
            var c0 = S.sk.cells[0]
            if sim.vol(S.sk.mash) > 50: col = kind_color("mash", S.sk.mash.T, S.sk.mash.col / maxf(1.0, sim.liqVol(S.sk.mash)))
            elif c0.f + c0.n > 0.005 * maxf(1.0, c0.w): col = wort_color(c0.col / maxf(1.0, c0.w))
            else: col = kind_color("water", S.sk.T, 0.0)
        _: col = Color(0.4, 0.6, 0.8)
    col.a = 0.88
    return col

func c1(x: float) -> String: return ("%.1f" % x).replace(".", ",")


func vessel_text(b) -> String:
    var t: String = _vessel_text0(b)
    if hard:
        var re: = RegEx.new();re.compile(" · [0-9,]+ hl|[0-9,]+ hl · ")
        t = re.sub(t, "", true)
    return t

func _vessel_text0(b) -> String:
    var S = sim.S
    match b:
        "rvp", "mp", "wp":
            var v = S[b]; var V: float = sim.hotV(v) if b == "rvp" else sim.vol(v)
            if V < 5: return "prázdná"
            var t: = "%s °C · %s hl" % [c1(v.Tsens), c1(V / 100.0)]
            if v.Pin > 1.0 and v.Twall - v.T > 1.5: t += " (u pláště %s °C)" % c1(v.Twall)
            if V > 300: t += " · ρ %s kg/l" % ("%.3f" % sim.density(v)).replace(".", ",")
            if sim.ext(v) > 5: t += " · %s °P" % c1(sim.plato(v))
            if v.boiling: t += " · VŘE"
            if v.stir: t += " · míchá"
            return t
        "sk":
            var V2: float = sim.skVolTotal()
            if V2 < 5: return "prázdná · stěny %s °C" % c1(S.sk.Tw)
            var t2: = "%s °C · %s hl" % [c1(S.sk.T), c1(V2 / 100.0)]
            if S.sk.g > 1: t2 += " · lože %d cm" % roundi(sim.skBedH() * 100.0)
            if S.sk.drawn: t2 += " · výtok %s °P, zákal %d" % [c1(S.sk.outP), roundi(S.sk.turb)]
            return t2
        "hlt": return "%s °C · %s hl" % [c1(S.hlt.T), c1(S.hlt.w / 100.0)]
        "lv":
            if S.lv.w < 5: return "prázdná"
            return "%s °C · %s hl%s" % [c1(S.lv.T), c1(S.lv.w / 100.0), " · agregát chladí" if S.lv.chillOn else ""]
        "spilka":
            if S.sp.L < 5: return "prázdná"
            var e: float = S.sp.f + S.sp.n
            return "%s hl · %s °P · %s °C" % [c1(S.sp.L / 100.0), c1(100.0 * e / maxf(1.0, e + S.sp.w)), c1(S.sp.H / S.sp.L)]
    return ""


## data pro kartičku pod nádobou: velká teplota, objem, °P, příznaky
func vessel_card(b) -> Dictionary:
    var S = sim.S
    var d: = {"T": null, "lines": [], "flags": [], "empty": false}
    match b:
        "rvp", "mp", "wp":
            var v = S[b]; var V: float = sim.hotV(v) if b == "rvp" else sim.vol(v)
            if V < 5:
                d.empty = true;return d
            d.T = v.Tsens;d.lines.append("%s hl" % c1(V / 100.0))
            if sim.ext(v) > 5: d.lines.append("%s °P" % c1(sim.plato(v)))
            elif V > 300: d.lines.append("ρ %s" % ("%.3f" % sim.density(v)).replace(".", ","))
            if v.boiling: d.flags.append(["VŘE", Color("ff6b4a")])
            if v.stir: d.flags.append(["míchá", Color("5aa9ff")])
            if v.Pin > 1.0 and v.Twall - v.T > 1.5: d.flags.append(["plášť %d °C" % roundi(v.Twall), Color("ff9a3c")])
        "sk":
            var V2: float = sim.skVolTotal()
            if V2 < 5:
                d.empty = true;d.lines.append("stěny %s °C" % c1(S.sk.Tw));return d
            d.T = S.sk.T;d.lines.append("%s hl" % c1(V2 / 100.0))
            if S.sk.g > 1: d.lines.append("lože %d cm" % roundi(sim.skBedH() * 100.0))
            if S.sk.drawn: d.flags.append(["výtok %s °P" % c1(S.sk.outP), Color("f2c230")]);d.flags.append(["zákal %d" % roundi(S.sk.turb), Color("c9d1d8")])
            if S.sk.get("dp", 0.0) > 1.0 and S.sk.g > 1: d.flags.append(["Δp %d mbar" % roundi(S.sk.dp), Color("ff6b4a") if S.sk.dp > 150 else (Color("f2c230") if S.sk.dp > 80 else Color("4fd18b"))])
        "hlt":
            d.T = S.hlt.T;d.lines.append("%s hl" % c1(S.hlt.w / 100.0))
        "lv":
            if S.lv.w < 5:
                d.empty = true;return d
            d.T = S.lv.T;d.lines.append("%s hl" % c1(S.lv.w / 100.0))
            if S.lv.chillOn: d.flags.append(["agregát chladí", Color("5aa9ff")])
        "spilka":
            if S.sp.L < 5:
                d.empty = true;return d
            var e: float = S.sp.f + S.sp.n
            d.T = S.sp.H / S.sp.L;d.lines.append("%s hl" % c1(S.sp.L / 100.0));d.lines.append("%s °P" % c1(100.0 * e / maxf(1.0, e + S.sp.w)))
            if S.get("fer", {}).get("on", false): d.flags.append(["kvasí", Color("4fd18b")])
        _: return {}
    if hard:
        var keep: = []
        for l in d.lines:
            if not str(l).ends_with(" hl"): keep.append(l)
        d.lines = keep
    return d

static func temp_color(T: float) -> Color:
    if T < 15.0: return Color("5aa9ff")
    if T < 45.0: return Color("4fd1c5")
    if T < 66.0: return Color("9be15d")
    if T < 80.0: return Color("f2c230")
    if T < 95.0: return Color("ff9a3c")
    return Color("ff5a4a")

func _live_node(kind: String, it) -> LiveNode:
    var n: = LiveNode.new();n.m = self;n.live_only = true;n.kind = kind;n.it = it
    add_child(n);live_nodes.append(n);return n

func update_live() -> void :
    for n in live_nodes: n.upd()
    if cabinet != null: cabinet.upd()


func label_size() -> int:
    var z: float = cam.zoom.x
    return 14 if z >= 0.8 else (20 if z >= 0.5 else (28 if z >= 0.33 else 38))

func redraw() -> void :
    flow_t = 99.0
    if item_layer != null: item_layer.queue_redraw()
    if valve_layer != null: valve_layer.queue_redraw()
    update_live()


func redraw_valves() -> void :
    if valve_layer != null: valve_layer.queue_redraw()
    flow_t = 99.0

func vessel_by_bind(b: String):
    for it in items:
        if it.type == "vessel" and it.get("bind") == b:
            return it
    return null


class StaticLayer extends Node2D:
    var m
    var clip: = Rect2()
    func _seen(pts: PackedVector2Array) -> bool:
        if clip.size == Vector2.ZERO: return true
        var lo: = pts[0]; var hi: = pts[0]
        for p in pts:
            lo = Vector2(minf(lo.x, p.x), minf(lo.y, p.y));hi = Vector2(maxf(hi.x, p.x), maxf(hi.y, p.y))
        return clip.intersects(Rect2(lo, hi - lo).grow(30.0))

    func _pipe_layers(pts: PackedVector2Array, w: float, rounded: bool) -> void :
        _pipe_outline(pts, w, rounded);_pipe_fill(pts, w, rounded)

    var _pends: = {}
    var _occ: PackedVector2Array = PackedVector2Array()
    ## body, kde se potrubí potkává s jiným (konec/spoj) – tam se nezaoblují kolena
    func _pends_build() -> void :
        _pends.clear()
        _occ.clear()
        for it0 in m.items:
            if it0.type in ["valve", "pump", "fitting", "inl", "manifold"] and it0.has("x"): _occ.append(Vector2(it0.x, it0.get("y", 0)))
        for it in m.items:
            if it.type == "pipe" and not it.get("hose", false):
                var n: int = it.pts.size()
                for i in n:
                    var k: = Vector2i(roundi(it.pts[i][0]), roundi(it.pts[i][1]))
                    _pends[k] = _pends.get(k, 0) + 1
        for j in m.plan.get("joints", []): _pends[Vector2i(roundi(j[0]), roundi(j[1]))] = 9
    ## kolena jako lisované oblouky (R ≈ 1,5 × průměr), svary na tečných bodech
    func _fillet(pts: PackedVector2Array, w: float) -> PackedVector2Array:
        if pts.size() < 3: return pts
        var out: = PackedVector2Array([pts[0]])
        var R: float = maxf(8.0, w * 1.5)
        for i in range(1, pts.size() - 1):
            var P: Vector2 = pts[i]; var A: Vector2 = pts[i - 1]; var B: Vector2 = pts[i + 1]
            var d1: = (A - P); var d2: = (B - P)
            var l1: = d1.length(); var l2: = d2.length()
            if l1 < 1.0 or l2 < 1.0 or absf(d1.normalized().cross(d2.normalized())) < 0.2 or _pends.get(Vector2i(roundi(P.x), roundi(P.y)), 0) > 1:
                out.append(P);continue
            var r: float = minf(R, minf(l1, l2) * 0.5)
            var T1: Vector2 = P + d1 / l1 * r; var T2: Vector2 = P + d2 / l2 * r
            for k in 9:
                var t: = float(k) / 8.0
                out.append(T1.lerp(P, t).lerp(P.lerp(T2, t), t))
        out.append(pts[pts.size() - 1])
        return out

    func _pipe_outline(pts: PackedVector2Array, w: float, rounded: bool) -> void :
        pts = _fillet(pts, w)
        draw_polyline(pts, Color("1e2429"), w + 3.5, rounded)
    func _pipe_fill(pts: PackedVector2Array, w: float, rounded: bool) -> void :
        pts = _fillet(pts, w)
        # lesklá nerez (kartáčovaný válec)
        draw_polyline(pts, Color("7d8892"), w + 1.0, rounded)
        draw_polyline(pts, Color("a9b3bb"), maxf(1.5, w * 0.8), rounded)
        draw_polyline(pts, Color("cfd6dc"), maxf(1.3, w * 0.52), rounded)
        draw_polyline(pts, Color("f3f6f8"), maxf(1.0, w * 0.2), rounded)

    ## tri-clamp objímky na koncích trubek (světlé příruby + těsnění) a svary
    func _pipe_clamps(P) -> void :
        var pp: PackedVector2Array = P[0]; var w: float = P[1]
        if pp.size() < 2 or w < 5.0: return
        for k in 2:
            var e: Vector2 = pp[0] if k == 0 else pp[pp.size() - 1]
            var nb: Vector2 = pp[1] if k == 0 else pp[pp.size() - 2]
            var d: = (nb - e)
            if d.length() < 14.0: continue
            d = d.normalized();var nr: = Vector2( - d.y, d.x)
            var c: Vector2 = e + d * 8.0
            var hw: float = w * 0.5 + 3.5
            draw_line(c - nr * hw, c + nr * hw, Color("1e2429"), 7.0)
            draw_line(c - nr * (hw - 1.0), c + nr * (hw - 1.0), Color("c3ccd3"), 5.0)
            draw_line(c - nr * (hw - 1.0) - d * 1.2, c + nr * (hw - 1.0) - d * 1.2, Color("f2f5f7"), 1.2)
            draw_line(c - nr * (w * 0.5 + 1.0) + d * 6.0, c + nr * (w * 0.5 + 1.0) + d * 6.0, Color("5a646c"), 1.0)

    ## tri-clamp spoje na dlouhých rovných úsecích: dvě přivařené objímky, upínací pás a křídlová matice (jako ve 3D)
    func _pipe_joints(P) -> void :
        var pp: PackedVector2Array = P[0]; var w: float = P[1]
        if pp.size() < 2 or w < 8.0: return
        for i in pp.size() - 1:
            var a: Vector2 = pp[i]; var b: Vector2 = pp[i + 1]
            var L: float = a.distance_to(b)
            if L < 300.0: continue
            var d: = (b - a) / L; var nr: = Vector2( - d.y, d.x)
            var n: int = int(L / 280.0)
            for k in n:
                var c: Vector2 = a + d * (L * (k + 0.5) / n)
                var near: = false
                for q in _occ:
                    if q.distance_to(c) < 56.0: near = true;break
                if near: continue
                for e in [a, b]:
                    if e.distance_to(c) < 50.0: near = true
                if near: continue
                var hw: float = w * 0.5 + 3.0
                for fo in [-4.5, 4.5]:
                    draw_line(c + d * fo - nr * hw, c + d * fo + nr * hw, Color("1e2429"), 4.5)
                    draw_line(c + d * fo - nr * (hw - 0.8), c + d * fo + nr * (hw - 0.8), Color("b9c2ca"), 2.8)
                draw_line(c - nr * (hw + 1.0), c + nr * (hw + 1.0), Color("1e2429"), 6.0)
                draw_line(c - nr * hw, c + nr * hw, Color("8e98a2"), 4.2)
                draw_line(c - nr * hw - d * 1.0, c + nr * hw - d * 1.0, Color("e9eef1"), 1.2)
                var wn: Vector2 = c + nr * (hw + 3.5)
                draw_circle(wn, 3.6, Color("1e2429")); draw_circle(wn, 2.6, Color("aeb7bf")); draw_circle(wn - Vector2(0.7, 0.7), 1.0, Color("ffffff"))
                draw_line(wn, wn + nr * 2.5, Color("5a646c"), 2.0)

    func _joint_near(joints: Array, c: Vector2) -> bool:
        for j in joints:
            if absf(j[0] - c.x) < 2.5 and absf(j[1] - c.y) < 2.5: return true
        return false

    ## hala: obkladová stěna, sokl, epoxidová podlaha s kanálkem
    func _vgrad(r: Rect2, a: Color, b: Color) -> void :
        if r.size.x <= 0.0 or r.size.y <= 0.0: return
        draw_polygon(PackedVector2Array([r.position, Vector2(r.end.x, r.position.y), r.end, Vector2(r.position.x, r.end.y)]), PackedColorArray([a, a, b, b]))
    func _hall(B: Rect2, G: Rect2) -> void :
        draw_rect(B if clip.size == Vector2.ZERO else G, Color.WHITE)

    ## měkká eliptická skvrna (světlo, odlesk): střed barva c, okraj průhledný
    func _glow(c: Vector2, rx: float, ry: float, col: Color, n: = 40) -> void :
        var edge: = Color(col, 0.0)
        for i in n:
            var a0: = TAU * i / n; var a1: = TAU * (i + 1) / n
            draw_polygon(PackedVector2Array([c, c + Vector2(cos(a0) * rx, sin(a0) * ry), c + Vector2(cos(a1) * rx, sin(a1) * ry)]), PackedColorArray([col, edge, edge]))

    ## odraz v lesklé podlaze: svislé slábnutí, měkké boky
    func _reflect(x0: float, y0: float, w: float, h: float, c: Color, a: float) -> void :
        var e: = minf(w * 0.18, 24.0)
        var xs: = [x0, x0 + e, x0 + w - e, x0 + w]
        var at: = [0.0, a, a, 0.0]
        for i in 3:
            draw_polygon(PackedVector2Array([Vector2(xs[i], y0), Vector2(xs[i + 1], y0), Vector2(xs[i + 1], y0 + h), Vector2(xs[i], y0 + h)]), PackedColorArray([Color(c, at[i]), Color(c, at[i + 1]), Color(c, 0.0), Color(c, 0.0)]))

    func _lamp_items() -> Array:
        var out: = []
        for it in m.items:
            if it.type == "vessel" and it.get("shape") in ["tank", "lauter", "cone", "tall", "horiz"]: out.append(it)
        return out

    func _hall_dark(B: Rect2, G: Rect2) -> void :
        var R: Rect2 = B if clip.size == Vector2.ZERO else G
        var fy: float = m.floor_y
        var ty0: = fy - 300.0
        var ceil_y: = B.position.y + 30.0
        var wall_top: = Color("101317"); var wall_bot: = Color("2a3036")
        var k0: = clampf((R.position.y - B.position.y) / maxf(1.0, ty0 - B.position.y), 0.0, 1.0)
        var k1: = clampf((minf(R.end.y, ty0) - B.position.y) / maxf(1.0, ty0 - B.position.y), 0.0, 1.0)
        _vgrad(Rect2(R.position.x, R.position.y, R.size.x, minf(R.end.y, ty0) - R.position.y), wall_top.lerp(wall_bot, k0), wall_top.lerp(wall_bot, k1))
        # sendvičové panely
        var px: = floorf(R.position.x / 400.0) * 400.0
        while px < R.end.x:
            draw_line(Vector2(px, maxf(R.position.y, ceil_y)), Vector2(px, minf(R.end.y, ty0)), Color(0, 0, 0, 0.35), 2.0)
            draw_line(Vector2(px + 2, maxf(R.position.y, ceil_y)), Vector2(px + 2, minf(R.end.y, ty0)), Color(1, 1, 1, 0.035), 1.0)
            px += 400.0
        # obklad (tmavá glazura)
        if R.end.y > ty0 and R.position.y < fy:
            var tr: = Rect2(R.position.x, maxf(ty0, R.position.y), R.size.x, minf(fy, R.end.y) - maxf(ty0, R.position.y))
            _vgrad(tr, Color("343b41"), Color("2b3136"))
            var gy: = ty0 + 50.0
            while gy < fy:
                if gy > R.position.y - 2 and gy < R.end.y + 2:
                    draw_line(Vector2(R.position.x, gy), Vector2(R.end.x, gy), Color("1d2125"), 2.0)
                    draw_line(Vector2(R.position.x, gy + 2), Vector2(R.end.x, gy + 2), Color(1, 1, 1, 0.045), 1.0)
                gy += 50.0
            var row: = 0; var ry: = ty0
            while ry < fy:
                var off: = 50.0 if row % 2 == 1 else 0.0
                var gx: = floorf((R.position.x - off) / 100.0) * 100.0 + off
                while gx < R.end.x:
                    draw_line(Vector2(gx, maxf(ry, R.position.y)), Vector2(gx, minf(minf(ry + 50.0, fy), R.end.y)), Color("1d2125"), 2.0)
                    gx += 100.0
                ry += 50.0;row += 1
            draw_line(Vector2(R.position.x, ty0), Vector2(R.end.x, ty0), Color("596169"), 5.0)
            draw_line(Vector2(R.position.x, ty0 - 2), Vector2(R.end.x, ty0 - 2), Color(1, 1, 1, 0.18), 1.0)
        # strop: ocelový nosník
        if R.position.y < ceil_y + 10:
            _vgrad(Rect2(R.position.x, B.position.y, R.size.x, ceil_y - B.position.y), Color("07090b"), Color("15191d"))
            draw_line(Vector2(R.position.x, ceil_y), Vector2(R.end.x, ceil_y), Color("3a4148"), 3.0)
        # světla: kužel, skvrna na stěně
        var lamps: = _lamp_items()
        for it in lamps:
            var cx: float = it.x + it.w * 0.5; var ly: = ceil_y + 70.0
            var top_y: float = maxf(ly + 30.0, float(it.y) - 60.0)
            var warm: = Color(1.0, 0.87, 0.62)
            _glow(Vector2(cx, it.y + it.h * 0.45), it.w * 1.1, it.h * 0.8 + 140.0, Color(warm, 0.15), 48)
            draw_polygon(PackedVector2Array([Vector2(cx - 30, ly + 22), Vector2(cx + 30, ly + 22), Vector2(cx + it.w * 0.75 + 60, top_y + 140), Vector2(cx - it.w * 0.75 - 60, top_y + 140)]), PackedColorArray([Color(warm, 0.16), Color(warm, 0.16), Color(warm, 0.0), Color(warm, 0.0)]))
        # sokl a podlaha (lesklý tmavý epoxid)
        if R.end.y > fy - 16.0:
            _vgrad(Rect2(R.position.x, fy - 16.0, R.size.x, 16.0), Color("3f474e"), Color("1a1e22"))
            var fr: = Rect2(R.position.x, maxf(fy, R.position.y), R.size.x, R.end.y - maxf(fy, R.position.y))
            var kf0: = clampf((fr.position.y - fy) / maxf(1.0, B.end.y - fy), 0.0, 1.0); var kf1: = clampf((fr.end.y - fy) / maxf(1.0, B.end.y - fy), 0.0, 1.0)
            _vgrad(fr, Color("262b30").lerp(Color("0e1013"), kf0), Color("262b30").lerp(Color("0e1013"), kf1))
            draw_line(Vector2(R.position.x, fy), Vector2(R.end.x, fy), Color("0a0c0e"), 2.0)
            draw_line(Vector2(R.position.x, fy + 2), Vector2(R.end.x, fy + 2), Color(1, 1, 1, 0.10), 1.0)
            # odrazy nádob v podlaze + světlo na podlaze
            for it in lamps:
                var cx2: float = it.x + it.w * 0.5
                _glow(Vector2(cx2, fy + 70.0), it.w * 0.95, 55.0, Color(1.0, 0.86, 0.6, 0.10), 36)
                var rh: = minf(200.0, B.end.y - fy - 4.0)
                if rh > 10.0:
                    var on_floor: bool = m.foot_y(it) >= fy - 2.0
                    var refl: = []
                    if on_floor: refl.append([float(it.x) + 6.0, float(it.w) - 12.0, rh, 0.11])
                    else:
                        refl.append([float(it.x) - 22.0, 14.0, rh * 0.6, 0.16]);refl.append([float(it.x) + float(it.w) + 8.0, 14.0, rh * 0.6, 0.16])
                    for rf in refl: _reflect(rf[0], fy + 3.0, rf[1], rf[2], Color("aeb7bf"), rf[3])
            var fl: = fy + 110.0
            while fl < R.end.y:
                draw_line(Vector2(R.position.x, fl), Vector2(R.end.x, fl), Color(1, 1, 1, 0.025), 2.0)
                fl += 130.0
            var dy: = fy + 46.0
            if dy + 16.0 < B.end.y:
                draw_rect(Rect2(R.position.x, dy, R.size.x, 16.0), Color("2b3136"))
                draw_rect(Rect2(R.position.x, dy + 1.0, R.size.x, 2.0), Color("7c858d"))
                var sx: = floorf(R.position.x / 14.0) * 14.0
                while sx < R.end.x:
                    draw_rect(Rect2(sx, dy + 5.0, 8.0, 9.0), Color("0b0d0f"));sx += 14.0
        _stands(R, fy)
        # lampy navrch (aby je nepřekryl kužel)
        for it in lamps:
            var cx3: float = it.x + it.w * 0.5; var ly2: = ceil_y + 70.0
            draw_line(Vector2(cx3, ceil_y), Vector2(cx3, ly2), Color("2b3136"), 3.0)
            _glow(Vector2(cx3, ly2 + 24.0), 70.0, 40.0, Color(1.0, 0.9, 0.65, 0.35), 32)
            var shade: = PackedVector2Array([Vector2(cx3 - 8, ly2), Vector2(cx3 + 8, ly2), Vector2(cx3 + 36, ly2 + 22), Vector2(cx3 - 36, ly2 + 22)])
            draw_polygon(shade, PackedColorArray([Color("4a535b"), Color("4a535b"), Color("1b2025"), Color("2b3136")]))
            draw_line(Vector2(cx3 - 36, ly2 + 22), Vector2(cx3 + 36, ly2 + 22), Color("6c757d"), 2.0)
            draw_rect(Rect2(cx3 - 26, ly2 + 22, 52, 5), Color("fff1c8"))
        if clip.size == Vector2.ZERO: draw_rect(B, Color(0, 0, 0, 0.5), false, 6.0)

    func _hall_light(B: Rect2, G: Rect2) -> void :
        var R: Rect2 = B if clip.size == Vector2.ZERO else G
        var fy: float = m.floor_y
        var wall_top: = Color("c9d0d6"); var wall_bot: = Color("e3e8ec")
        var k0: = clampf((R.position.y - B.position.y) / maxf(1.0, fy - B.position.y), 0.0, 1.0)
        var k1: = clampf((minf(R.end.y, fy) - B.position.y) / maxf(1.0, fy - B.position.y), 0.0, 1.0)
        _vgrad(Rect2(R.position.x, R.position.y, R.size.x, minf(R.end.y, fy) - R.position.y), wall_top.lerp(wall_bot, k0), wall_top.lerp(wall_bot, k1))
        var px: = floorf(R.position.x / 400.0) * 400.0
        while px < R.end.x:
            draw_line(Vector2(px, R.position.y), Vector2(px, minf(R.end.y, fy - 300.0)), Color(0, 0, 0, 0.06), 2.0)
            draw_line(Vector2(px + 2, R.position.y), Vector2(px + 2, minf(R.end.y, fy - 300.0)), Color(1, 1, 1, 0.25), 1.0)
            px += 400.0
        var ty0: = fy - 300.0
        if R.end.y > ty0 and R.position.y < fy:
            _vgrad(Rect2(R.position.x, maxf(ty0, R.position.y), R.size.x, minf(fy, R.end.y) - maxf(ty0, R.position.y)), Color("eef1f3"), Color("e2e6e9"))
            draw_line(Vector2(R.position.x, ty0), Vector2(R.end.x, ty0), Color("9aa4ad"), 4.0)
            draw_line(Vector2(R.position.x, ty0 + 3), Vector2(R.end.x, ty0 + 3), Color(1, 1, 1, 0.7), 1.5)
            var gy: = ty0 + 50.0
            while gy < fy:
                if gy > R.position.y - 2 and gy < R.end.y + 2: draw_line(Vector2(R.position.x, gy), Vector2(R.end.x, gy), Color("cfd6dc"), 1.5)
                gy += 50.0
            var gx: = floorf(R.position.x / 50.0) * 50.0
            while gx < R.end.x:
                draw_line(Vector2(gx, maxf(ty0, R.position.y)), Vector2(gx, minf(fy, R.end.y)), Color("d6dce1"), 1.2)
                gx += 50.0
        if R.end.y > fy:
            _vgrad(Rect2(R.position.x, fy - 14.0, R.size.x, 14.0), Color("8e979f"), Color("5d666e"))
            var fr: = Rect2(R.position.x, maxf(fy, R.position.y), R.size.x, R.end.y - maxf(fy, R.position.y))
            var kf0: = clampf((fr.position.y - fy) / maxf(1.0, B.end.y - fy), 0.0, 1.0); var kf1: = clampf((fr.end.y - fy) / maxf(1.0, B.end.y - fy), 0.0, 1.0)
            _vgrad(fr, Color("a3abb0").lerp(Color("7a8389"), kf0), Color("a3abb0").lerp(Color("7a8389"), kf1))
            draw_line(Vector2(R.position.x, fy), Vector2(R.end.x, fy), Color("3b434a"), 2.0)
            draw_line(Vector2(R.position.x, fy + 2), Vector2(R.end.x, fy + 2), Color(1, 1, 1, 0.25), 1.0)
            var fx: = floorf(R.position.x / 200.0) * 200.0
            while fx < R.end.x + 200.0:
                draw_line(Vector2(fx, fy + 4), Vector2(fx - 60.0, B.end.y), Color(0, 0, 0, 0.06), 2.0)
                fx += 200.0
            var fl: = fy + 90.0
            while fl < R.end.y:
                draw_line(Vector2(R.position.x, fl), Vector2(R.end.x, fl), Color(0, 0, 0, 0.07), 2.0)
                fl += 120.0
            var dy: = fy + 46.0
            if dy + 16.0 < B.end.y:
                draw_rect(Rect2(R.position.x, dy, R.size.x, 16.0), Color("4d565e"))
                draw_rect(Rect2(R.position.x, dy + 1.0, R.size.x, 3.0), Color("c2cad1"))
                var sx: = floorf(R.position.x / 14.0) * 14.0
                while sx < R.end.x:
                    draw_rect(Rect2(sx, dy + 5.0, 8.0, 9.0), Color("262c31"));sx += 14.0
        _stands(R, fy)
        if clip.size == Vector2.ZERO: draw_rect(B, Color(0, 0, 0, 0.35), false, 6.0)

    ## ocelové stojany pod nádobami, které nestojí přímo na podlaze
    func _stands(R: Rect2, fy: float) -> void :
        for it in m.items:
            if it.type != "vessel" or not (it.get("shape") in ["tank", "lauter", "cone", "tall"]): continue
            var x: float = it.x; var w: float = it.w; var yd: float = float(it.y) + float(it.h) + 38.0
            if fy - yd < 30.0: continue
            if not R.intersects(Rect2(x - 60, yd - 20, w + 120, fy - yd + 40)): continue
            var cols: = [x - 22.0, x + w + 8.0]
            for cx in cols:
                draw_rect(Rect2(cx + 6, yd + 12, 14, fy - yd - 12), Color(0, 0, 0, 0.08))
                _hgrad(Rect2(cx, yd + 8, 14, fy - yd - 8), Color("6f7880"), Color("c3cbd2"), Color("5a636b"))
                draw_rect(Rect2(cx - 5, fy - 6, 24, 6), Color("3b434a"))
            var h: = fy - yd - 12.0
            if h > 60.0:
                var a0: = Vector2(cols[0] + 14, yd + 14); var a1: = Vector2(cols[1], fy - 10)
                var b0: = Vector2(cols[1], yd + 14); var b1: = Vector2(cols[0] + 14, fy - 10)
                draw_line(a0, a1, Color("7c858d"), 3.0, true);draw_line(b0, b1, Color("7c858d"), 3.0, true)
            draw_rect(Rect2(x - 30, yd + 6, w + 60, 8), Color(0, 0, 0, 0.12))
            _vgrad(Rect2(x - 30, yd - 2, w + 60, 10), Color("c9d1d8"), Color("6c757d"))
            draw_rect(Rect2(x - 30, yd - 2, w + 60, 10), Color("2a3137"), false, 1.0)
    func _hgrad(r: Rect2, a: Color, mid: Color, b: Color) -> void :
        var xm: = r.position.x + r.size.x * 0.35
        draw_polygon(PackedVector2Array([r.position, Vector2(xm, r.position.y), Vector2(xm, r.end.y), Vector2(r.position.x, r.end.y)]), PackedColorArray([a, mid, mid, a]))
        draw_polygon(PackedVector2Array([Vector2(xm, r.position.y), Vector2(r.end.x, r.position.y), r.end, Vector2(xm, r.end.y)]), PackedColorArray([mid, b, b, mid]))
    func _draw() -> void :
        var B: Rect2 = m.bounds
        var G: Rect2 = B if clip.size == Vector2.ZERO else B.intersection(clip.grow(60.0))

        _hall(B, G)

        for it in m.items:
            if it.type == "zone":
                var r: = Rect2(it.x, it.y, it.w, it.h)
                if not m.hall_dark:
                    draw_rect(r, Color(1, 1, 1, 0.07))
                    draw_rect(r, Color(0.35, 0.4, 0.45, 0.12), false, 2.0)
                var tw: float = m.font_b.get_string_size(it.label, HORIZONTAL_ALIGNMENT_LEFT, -1, 22).x + 36
                Art.rrect(self, Rect2(it.x + 14, it.y + 14, tw, 36), Color("f2c230"), 5.0)
                draw_string(m.font_b, Vector2(it.x + 32, it.y + 41), it.label, HORIZONTAL_ALIGNMENT_LEFT, -1, 22, Color("1b2026"))


        var pipes: = []
        var solo: = []
        for it in m.items:
            if it.type == "pipe" and it.pts.size() >= 2:
                if it.get("hose", false): continue
                var pts: = PackedVector2Array()
                for p in it.pts: pts.append(Vector2(p[0], p[1]))
                var dn: = float(it.get("dn", 50))
                var w: = clampf(dn * 0.24, 3.0, 6.0) if dn <= 32 else maxf(9.0, roundf(12.0 * dn / 50.0))
                if not _seen(pts): continue
                if it.get("solo", false): solo.append([pts, w])
                else: pipes.append([pts, w])
            elif it.type == "manifold":

                for q in m.mf_ports(it):
                    var mp_: = PackedVector2Array([q.back, q.front])
                    if _seen(mp_): solo.append([mp_, 12.0])
            elif it.type == "fitting":

                for q in m.sim.fit_ports(it):
                    var fp_: = PackedVector2Array([Vector2(it.x, it.y), q])
                    if _seen(fp_): solo.append([fp_, 12.0])
        for p in pipes + solo:
            var sh: = PackedVector2Array()
            for q in p[0]: sh.append(q + Vector2(4, 6))
            draw_polyline(sh, Color(0, 0, 0, 0.10), p[1] + 6.0, true)
        _pends_build()
        for p in pipes: _pipe_outline(p[0], p[1], true)
        for p in pipes: _pipe_fill(p[0], p[1], true)
        for p in pipes: _pipe_clamps(p)
        for p in pipes: _pipe_joints(p)
        var joints: Array = m.plan.get("joints", [])
        var jset: = {}
        for j in joints: jset[Vector2i(roundi(j[0]), roundi(j[1]))] = true
        var crosses: = []
        var dots: = {}
        for bi in range(1, pipes.size()):
            for ai in bi:
                var PA = pipes[ai]; var PB = pipes[bi]
                for jb in PB[0].size() - 1:
                    var b0: Vector2 = PB[0][jb]; var b1: Vector2 = PB[0][jb + 1]
                    var sv: = b1 - b0; var sl: = sv.length()
                    if sl < 1.0: continue
                    for ia in PA[0].size() - 1:
                        var a0: Vector2 = PA[0][ia]; var rv: Vector2 = PA[0][ia + 1] - a0
                        var den: = rv.x * sv.y - rv.y * sv.x
                        if absf(den) < 1e-09: continue
                        var t: = ((b0.x - a0.x) * sv.y - (b0.y - a0.y) * sv.x) / den
                        var uu: = ((b0.x - a0.x) * rv.y - (b0.y - a0.y) * rv.x) / den
                        if t <= 0.001 or t >= 0.999 or uu <= 0.001 or uu >= 0.999: continue
                        var c: = a0 + rv * t
                        if _joint_near(joints, c): dots[Vector2i(roundi(c.x), roundi(c.y))] = c;continue
                        crosses.append([c, sv / sl, (PA[1] + 6.0) / 2.0 + 4.0, PB[1]])

        for P in pipes:
            var pp: PackedVector2Array = P[0]
            if pp.size() < 4: continue
            for i in pp.size() - 1:
                for j in range(i + 2, pp.size() - 1):
                    var a0: Vector2 = pp[i]; var rv: Vector2 = pp[i + 1] - a0; var b0: Vector2 = pp[j]; var sv: Vector2 = pp[j + 1] - b0
                    var den: = rv.x * sv.y - rv.y * sv.x
                    if absf(den) < 1e-09: continue
                    var t: = ((b0.x - a0.x) * sv.y - (b0.y - a0.y) * sv.x) / den
                    var uu: = ((b0.x - a0.x) * rv.y - (b0.y - a0.y) * rv.x) / den
                    if t <= 0.001 or t >= 0.999 or uu <= 0.001 or uu >= 0.999: continue
                    var c: = a0 + rv * t
                    if _joint_near(joints, c): continue
                    var dv: = rv
                    if absf(sv.x) > absf(rv.x): dv = sv
                    crosses.append([c, dv.normalized(), P[1] / 2.0 + 7.0, P[1]])
        for x in crosses:
            var pa: Vector2 = x[0] - x[1] * x[2]; var pb: Vector2 = x[0] + x[1] * x[2]
            _pipe_layers(PackedVector2Array([pa, pb]), x[3], false)
        for p in solo: _pipe_layers(p[0], p[1], true)

        # barevný proužek cesty (editor → Barva)
        for itc in m.items:
            if itc.type == "pipe" and str(itc.get("color", "")) != "" and itc.pts.size() >= 2 and not itc.get("hose", false):
                var plc: = PackedVector2Array()
                for q in itc.pts: plc.append(Vector2(q[0], q[1]))
                if _seen(plc): draw_polyline(plc, Color(str(itc.color), 0.85), maxf(3.0, 4.0 * float(itc.get("dn", 50)) / 50.0), true)
        for c in dots.values():
            draw_circle(c, 10.5, Color("1e2429"));draw_circle(c, 9.0, Color("a9b3bb"));draw_circle(c, 6.5, Color("d7dde2"))
            draw_circle(c, 4.0, Color("f2c230"))
        for it in m.items:
            if it.type == "text":
                draw_string(m.font, Vector2(it.x, it.y), it.label, HORIZONTAL_ALIGNMENT_LEFT, -1, int(it.get("size", 12)), Color("aeb7bf") if m.hall_dark else Color("2b3138"))


class Mini extends Control:
    var m
    var sc: = 1.0
    var org: = Vector2.ZERO
    var view: Control
    func setup() -> void :
        var B: Rect2 = m.bounds
        sc = minf(330.0 / B.size.x, 110.0 / B.size.y);org = B.position
        custom_minimum_size = (B.size * sc).ceil();size = custom_minimum_size
        mouse_filter = Control.MOUSE_FILTER_STOP
        anchor_top = 1.0;anchor_bottom = 1.0
        offset_left = 12;offset_right = 12 + size.x;offset_top = -14 - size.y;offset_bottom = -14
        clip_contents = true
        view = MiniView.new();view.m = m;view.mini = self;view.mouse_filter = Control.MOUSE_FILTER_IGNORE
        add_child(view);view.set_anchors_preset(Control.PRESET_FULL_RECT)
        queue_redraw()
    func w2m(p: Vector2) -> Vector2: return (p - org) * sc

    func _pipe_layers(pts: PackedVector2Array, w: float, rounded: bool) -> void :
        draw_polyline(pts, Color("0d1013"), w + 4.0, rounded)
        draw_polyline(pts, Art.PIPE_STEEL, w + 1.0, rounded)
        if w > 5.5: draw_polyline(pts, Color("eef2f5"), maxf(1.2, w * 0.2), rounded)
        if w > 8.0: draw_polyline(pts, Color("14181c"), w - 5.0, rounded)



    func _pipe_outline(pts: PackedVector2Array, w: float, rounded: bool) -> void :
        draw_polyline(pts, Color("0d1013"), w + 4.0, rounded)
    func _pipe_fill(pts: PackedVector2Array, w: float, rounded: bool) -> void :
        draw_polyline(pts, Art.PIPE_STEEL, w + 1.0, rounded)
        if w > 5.5: draw_polyline(pts, Color("eef2f5"), maxf(1.2, w * 0.2), rounded)
        if w > 8.0: draw_polyline(pts, Color("14181c"), w - 5.0, rounded)

    func _joint_near(joints: Array, c: Vector2) -> bool:
        for j in joints:
            if absf(j[0] - c.x) < 2.5 and absf(j[1] - c.y) < 2.5: return true
        return false
    func _draw() -> void :
        var B: Rect2 = m.bounds
        Art.rrect(self, Rect2(Vector2(3, 4), B.size * sc), Color(0, 0, 0, 0.3), 6.0)
        Art.rrect_border(self, Rect2(Vector2.ZERO, B.size * sc), Color(0.07, 0.085, 0.1, 0.8), Color(1, 1, 1, 0.18), 1, 6.0)
        var fy: float = (m.floor_y - B.position.y) * sc
        draw_rect(Rect2(0, fy, B.size.x * sc, B.size.y * sc - fy), Color(1, 1, 1, 0.05))
        for it in m.items:
            match it.type:
                "zone": draw_rect(Rect2(w2m(Vector2(it.x, it.y)), Vector2(it.w, it.h) * sc), Color(1, 1, 1, 0.06))
                "pipe":
                    if it.get("hose", false): continue
                    var pts: = PackedVector2Array()
                    for p in it.pts: pts.append(w2m(Vector2(p[0], p[1])))
                    if pts.size() >= 2: draw_polyline(pts, Color("7b858e"), 1.0)
                "vessel":
                    if it.get("shape") in ["platform", "inlet"]: continue
                    draw_rect(Rect2(w2m(Vector2(it.x, it.y)), Vector2(it.w, it.h) * sc), Color("c7cfd6"))
                "panel": draw_rect(Rect2(w2m(Vector2(it.x, it.y)), Vector2(it.get("w", 200), it.get("h", 200)) * sc), Color("59626a"))
    func _gui_input(e: InputEvent) -> void :
        if (e is InputEventMouseButton and e.pressed and e.button_index == MOUSE_BUTTON_LEFT) or (e is InputEventMouseMotion and (e.button_mask & MOUSE_BUTTON_MASK_LEFT) != 0):
            m.cam.position = e.position / sc + org - Vector2(m.right_w() * 0.5, -30.0) / m.cam.zoom.x
            m.vel = Vector2.ZERO;m.zoom_anim = false
            accept_event()

class MiniView extends Control:
    var m
    var mini
    var last: = Vector3.ZERO
    func _process(_d: float) -> void :
        var c: = Vector3(m.cam.position.x, m.cam.position.y, m.cam.zoom.x)
        if c != last:
            last = c;queue_redraw()
    func _draw() -> void :
        var s: Vector2 = m.vp_size(); var z: float = m.cam.zoom.x
        var avail: = Vector2(s.x - m.right_w(), s.y - 60.0) / z
        var ctr: Vector2 = m.cam.position - Vector2(m.right_w() * 0.5, -30.0) / z
        if size.x < 4.0 or size.y < 4.0: return
        var r: = Rect2(mini.w2m(ctr - avail * 0.5), avail * mini.sc).abs().intersection(Rect2(Vector2(1, 1), size - Vector2(2, 2)))
        if r.size.x <= 0.0 or r.size.y <= 0.0: return
        draw_rect(r, Color(0.95, 0.76, 0.19, 0.14))
        draw_rect(r, Color("f2c230"), false, 1.5)

const PIPE_LIQ: = """
shader_type canvas_item;
float h21(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float vn(vec2 p) {
	vec2 i = floor(p); vec2 f = fract(p); f = f * f * (3.0 - 2.0 * f);
	return mix(mix(h21(i), h21(i + vec2(1.0, 0.0)), f.x), mix(h21(i + vec2(0.0, 1.0)), h21(i + vec2(1.0, 1.0)), f.x), f.y);
}
void fragment() {
	float c = UV.y * 2.0 - 1.0;
	float d = sqrt(max(0.0, 1.0 - c * c));
	float spd = COLOR.a * 300.0;
	vec3 base = COLOR.rgb;
	base = mix(base, vec3(dot(base, vec3(0.299, 0.587, 0.114))), 0.10) * vec3(0.97, 0.95, 0.90);
	float x = UV.x - TIME * spd;
	vec3 deep = base * vec3(0.80, 0.64, 0.46);
	vec3 col = mix(base * vec3(1.10, 1.08, 1.05), deep, pow(d, 1.3));
	float st = vn(vec2(x * 0.045, c * 2.5 + 3.0));
	col *= 1.0 + step(0.5, spd) * (st - 0.5) * 0.10;
	vec2 g = vec2(x * 0.22, c * 5.0);
	vec2 gi = floor(g); vec2 gf = fract(g);
	float hh = h21(gi);
	vec2 cp = vec2(0.5 + 0.35 * sin(hh * 40.0), 0.5 + 0.35 * cos(hh * 30.0));
	float sp = smoothstep(0.16, 0.0, length(gf - cp)) * step(0.82, hh);
	col = mix(col, base * 1.35 + 0.06, sp * 0.5 * d);
	col *= mix(1.0, 0.62, smoothstep(0.70, 1.0, abs(c)));
	col += vec3(1.0, 0.97, 0.9) * exp(-pow((c + 0.46) / 0.10, 2.0)) * 0.38;
	col += vec3(0.8, 0.85, 0.95) * exp(-pow((c - 0.62) / 0.07, 2.0)) * 0.12;
	COLOR = vec4(col, 1.0);
}
"""

class FlowLayer extends Node2D:
    var m
    var mat: ShaderMaterial
    var tex: Texture2D
    var fill_mi: MeshInstance2D
    var dash_mi: MeshInstance2D
    var edges: = []
    var last_cols: = []
    var last_dash: = []
    func build() -> void :
        if mat == null:
            var sh: = Shader.new();sh.code = PIPE_LIQ
            mat = ShaderMaterial.new();mat.shader = sh
            fill_mi = MeshInstance2D.new();add_child(fill_mi)   # obrys (jednobarevný)
            dash_mi = MeshInstance2D.new();dash_mi.material = mat;add_child(dash_mi)   # tělo kapaliny (shader: válcový průřez, absorpce, lesk, částice)
        var hoses: = {}
        for it in m.items:
            if it.type == "pipe" and it.get("hose", false): hoses[it.id] = true
        edges.clear()
        for e in m.sim.net.edges:
            if hoses.has(e.pipe): continue
            var dn: float = e.dn
            var w: = 6.0 if dn <= 32 else maxf(9.0, roundf(12.0 * dn / 50.0))
            var wp: = clampf(dn * 0.24, 3.0, 6.0) if dn <= 32 else maxf(9.0, roundf(12.0 * dn / 50.0))   # šířka trubky jako ve statické vrstvě
            edges.append([e, maxf(3.0, w - 5.0), wp])
        last_cols = [];last_dash = []
        refresh()
    var prev_full: = {}   # minulé zaplnění trubky (pro směr: plní se / vyprazdňuje)
    var fmode: = {}       # +1 plní se od zdroje, -1 vyprazdňuje se (zbytek u cíle)
    var rng: = {}         # úsek trubky (podíl délky), kde je kapalina – čelo vody postupuje plynule
    func refresh() -> void :
        var sim = m.sim
        var cols: = []; var dsh: = []
        rng.clear()
        for E in edges:
            var e = E[0]
            var full: float = sim.vol(e.c) / e.cap
            if full < 0.02:
                cols.append(null);prev_full[e.id] = full;fmode[e.id] = 0;continue
            var pf: float = prev_full.get(e.id, full)
            if absf(full - pf) > 0.002:
                fmode[e.id] = 1 if full > pf else -1
                prev_full[e.id] = full
            var md: int = fmode.get(e.id, 0)
            var part: bool = e.flow != 0 and md != 0 and full < 0.97
            var col: Color = m.kind_color(sim.contentKind(e.c), e.c.T, e.c.col / maxf(0.01, sim.liqVol(e.c)))
            var s0: = 0.0; var s1: = 1.0
            if part:
                var f: = snappedf(clampf(full, 0.0, 1.0), 0.01)
                var at_a: bool = (e.from == "a") == (md > 0)
                if at_a: s1 = f
                else: s0 = 1.0 - f
                col.a = 1.0
            else:
                if full < 0.15:
                    cols.append(null);continue
                col.a = 1.0
            rng[e.id] = [s0, s1]
            cols.append([Color(snappedf(col.r, 0.03), snappedf(col.g, 0.03), snappedf(col.b, 0.03), 1.0), s0, s1, (_vpx(e) if e.flow != 0 else 0)])
        if cols != last_cols:
            last_cols = cols;_mesh_fill()

    func _sub(pts: Array, s0: float, s1: float) -> Array:
        if s0 <= 0.0 and s1 >= 1.0: return pts
        var tot: = 0.0
        for k in pts.size() - 1: tot += pts[k].distance_to(pts[k + 1])
        var a: = s0 * tot; var b: = s1 * tot; var o: = []; var acc: = 0.0
        for k in pts.size() - 1:
            var L: float = pts[k].distance_to(pts[k + 1])
            var lo: = maxf(a, acc); var hi: = minf(b, acc + L)
            if hi > lo and L > 0.0:
                var p0: Vector2 = pts[k].lerp(pts[k + 1], (lo - acc) / L); var p1: Vector2 = pts[k].lerp(pts[k + 1], (hi - acc) / L)
                if o.is_empty() or o[o.size() - 1] != p0: o.append(p0)
                o.append(p1)
            acc += L
        return o

    func _spd(e, w: float) -> int:
        var area: float = 0.00196 * pow(e.dn / 50.0, 2)
        var v_px: float = e.flow / 60000.0 / area * 100.0
        return clampi(roundi(v_px / (w * 4.0) * 2.0), 1, 24)
    ## kolena stejně zaoblená jako trubky (viz StaticLayer._fillet), aby voda sledovala vnitřek trubky
    func _fillet(pts: Array, w: float) -> Array:
        if pts.size() < 3: return pts
        var out: = [pts[0]]
        var R: float = maxf(8.0, w * 1.5)
        for i in range(1, pts.size() - 1):
            var P: Vector2 = pts[i]; var A: Vector2 = pts[i - 1]; var B: Vector2 = pts[i + 1]
            var d1: = (A - P); var d2: = (B - P)
            var l1: = d1.length(); var l2: = d2.length()
            if l1 < 1.0 or l2 < 1.0 or absf(d1.normalized().cross(d2.normalized())) < 0.2:
                out.append(P);continue
            var r: float = minf(R, minf(l1, l2) * 0.5)
            var T1: Vector2 = P + d1 / l1 * r; var T2: Vector2 = P + d2 / l2 * r
            for k in 9:
                var t: = float(k) / 8.0
                out.append(T1.lerp(P, t).lerp(P.lerp(T2, t), t))
        out.append(pts[pts.size() - 1])
        return out
    ## půlkruhová čepička na konci vodního pruhu (kulatý konec místo hranatého)
    func _cap(v: PackedVector2Array, uv: PackedVector2Array, c: PackedColorArray, p: Vector2, dir: Vector2, w: float, col: Color) -> void :
        var r: = w * 0.5; var base: = dir.angle()
        for k in 6:
            var a0: = base - PI * 0.5 + PI * k / 6.0; var a1: = base - PI * 0.5 + PI * (k + 1) / 6.0
            v.append_array([p, p + Vector2(cos(a0), sin(a0)) * r, p + Vector2(cos(a1), sin(a1)) * r])
            uv.append_array([Vector2(0.5, 0.5), Vector2(0.5, 0.5), Vector2(0.5, 0.5)])
            for i in 3: c.append(col)
    func _shift(pp: Array, off: Vector2) -> Array:
        var o: = []
        for q in pp: o.append(q + off)
        return o
    func _water_line(v: PackedVector2Array, uv: PackedVector2Array, c: PackedColorArray, pp: Array, w: float, col: Color) -> void :
        if pp.size() < 2: return
        for k in pp.size() - 1: _seg(v, uv, c, pp[k], pp[k + 1], w, col, 0.0, 1.0, 0.0, false)
        var d0: Vector2 = (pp[0] - pp[1]); var d1: Vector2 = (pp[pp.size() - 1] - pp[pp.size() - 2])
        if d0.length() > 0.01: _cap(v, uv, c, pp[0], d0.normalized(), w, col)
        if d1.length() > 0.01: _cap(v, uv, c, pp[pp.size() - 1], d1.normalized(), w, col)
    func _seg(v: PackedVector2Array, uv: PackedVector2Array, c: PackedColorArray, a: Vector2, b: Vector2, w: float, col: Color, u0: float, u1: float, yb: = 0.0, ext: = true) -> void :
        var d: = (b - a).normalized(); var n: = Vector2( - d.y, d.x) * w * 0.5; var ex: = d * w * 0.5 if ext else Vector2.ZERO
        var a2: = a - ex; var b2: = b + ex
        v.append_array([a2 + n, b2 + n, b2 - n, a2 + n, b2 - n, a2 - n])
        uv.append_array([Vector2(u0, yb + 0.1), Vector2(u1, yb + 0.1), Vector2(u1, yb + 0.9), Vector2(u0, yb + 0.1), Vector2(u1, yb + 0.9), Vector2(u0, yb + 0.9)])
        for i in 6: c.append(col)
    func _set_mesh(mi: MeshInstance2D, v: PackedVector2Array, uv: PackedVector2Array, c: PackedColorArray) -> void :
        if v.size() == 0:
            mi.mesh = null;return
        var arr: = [];arr.resize(Mesh.ARRAY_MAX)
        arr[Mesh.ARRAY_VERTEX] = v;arr[Mesh.ARRAY_TEX_UV] = uv;arr[Mesh.ARRAY_COLOR] = c
        var am: = ArrayMesh.new();am.add_surface_from_arrays(Mesh.PRIMITIVE_TRIANGLES, arr);mi.mesh = am
    ## rychlost proudu v px/s (kvantovaná na 10 px/s, ať se síť zbytečně nepřestavuje)
    func _vpx(e) -> int:
        var area: float = 0.00196 * pow(e.dn / 50.0, 2)
        var v_px: float = e.flow / 60000.0 / area * 100.0
        return clampi(int(roundf(v_px / 10.0)) * 10, 10, 300)
    func _trim(pts: Array, t0: float, t1: float) -> Array:
        var tot: = 0.0
        for k in pts.size() - 1: tot += pts[k].distance_to(pts[k + 1])
        if tot < t0 + t1 + 4.0: return pts
        return _sub(pts, t0 / tot, 1.0 - t1 / tot)
    ## pás podél lomené čáry: u = délka v px po směru proudu, v = 0…1 napříč (shader z toho dělá válcový průřez)
    func _strip(v: PackedVector2Array, uv: PackedVector2Array, c: PackedColorArray, pp: Array, w: float, col: Color, flat: bool) -> void :
        var n: int = pp.size()
        if n < 2: return
        var L: = []; var U: = []; var Rr: = []; var acc: = 0.0
        for i in n:
            var d0: = Vector2.ZERO; var d1: = Vector2.ZERO
            if i > 0: d0 = (pp[i] - pp[i - 1]).normalized()
            if i < n - 1: d1 = (pp[i + 1] - pp[i]).normalized()
            var dd: Vector2 = (d0 + d1)
            if dd.length() < 0.001: dd = d1 if d1 != Vector2.ZERO else d0
            dd = dd.normalized()
            var nn: = Vector2( - dd.y, dd.x)
            var k: float = 1.0
            if d0 != Vector2.ZERO and d1 != Vector2.ZERO: k = clampf(1.0 / maxf(0.35, dd.dot(d1)), 1.0, 1.6)
            L.append(pp[i] + nn * w * 0.5 * k);Rr.append(pp[i] - nn * w * 0.5 * k)
            if i > 0: acc += pp[i].distance_to(pp[i - 1])
            U.append(acc)
        for i in n - 1:
            v.append_array([L[i], L[i + 1], Rr[i + 1], L[i], Rr[i + 1], Rr[i]])
            uv.append_array([Vector2(U[i], 0.0), Vector2(U[i + 1], 0.0), Vector2(U[i + 1], 1.0), Vector2(U[i], 0.0), Vector2(U[i + 1], 1.0), Vector2(U[i], 1.0)])
            for q in 6: c.append(col)
    ## půlkruhová čepička (čelo kapaliny v částečně naplněné trubce)
    func _scap(v: PackedVector2Array, uv: PackedVector2Array, c: PackedColorArray, p: Vector2, dir: Vector2, w: float, col: Color, u: float) -> void :
        var r: = w * 0.5; var base: = dir.angle(); var nr: = Vector2( - dir.y, dir.x)
        for k in 8:
            var a0: = base - PI * 0.5 + PI * k / 8.0; var a1: = base - PI * 0.5 + PI * (k + 1) / 8.0
            var q0: = Vector2(cos(a0), sin(a0)) * r; var q1: = Vector2(cos(a1), sin(a1)) * r
            v.append_array([p, p + q0, p + q1])
            uv.append_array([Vector2(u, 0.5), Vector2(u, 0.5 + 0.5 * q0.dot(nr) / r), Vector2(u, 0.5 + 0.5 * q1.dot(nr) / r)])
            for i in 3: c.append(col)
    func _mesh_fill() -> void :
        var vo: = PackedVector2Array(); var uo: = PackedVector2Array(); var co: = PackedColorArray()
        var vb: = PackedVector2Array(); var ub: = PackedVector2Array(); var cb: = PackedColorArray()
        var nodes = m.sim.net.nodes
        for i in edges.size():
            var ci = last_cols[i]
            if ci == null: continue
            var e = edges[i][0]; var w: float = edges[i][1]
            var pts: Array = _sub(_fillet(e.pts, edges[i][2]), ci[1], ci[2]).duplicate()
            if pts.size() < 2: continue
            # na odbočkách (T-kus) se úsek zkrátí, aby se přes hlavní trubku nekreslil jeho průřez
            var ta: float = 0.0; var tb: float = 0.0
            if ci[1] <= 0.0 and nodes.has(e.a) and nodes[e.a].edges.size() >= 3: ta = w * 0.55
            if ci[2] >= 1.0 and nodes.has(e.b) and nodes[e.b].edges.size() >= 3: tb = w * 0.55
            pts = _trim(pts, ta, tb)
            var cap_a: bool = ci[1] > 0.0; var cap_b: bool = ci[2] < 1.0
            if e.flow != 0 and e.from == "b":
                pts.reverse();var tmp: = cap_a;cap_a = cap_b;cap_b = tmp
            var col: Color = ci[0]
            var oc: Color = col.darkened(0.30)
            _strip(vo, uo, co, pts, w, oc, true)
            var bc: = Color(col.r, col.g, col.b, clampf(float(ci[3]) / 300.0, 0.0, 1.0))
            _strip(vb, ub, cb, pts, w * 0.80, bc, false)
            var tot: = 0.0
            for k in pts.size() - 1: tot += pts[k].distance_to(pts[k + 1])
            if cap_a:
                var d0: Vector2 = (pts[0] - pts[1]).normalized()
                _scap(vo, uo, co, pts[0], d0, w, oc, 0.0);_scap(vb, ub, cb, pts[0], d0, w * 0.80, bc, 0.0)
            if cap_b:
                var d1: Vector2 = (pts[pts.size() - 1] - pts[pts.size() - 2]).normalized()
                _scap(vo, uo, co, pts[pts.size() - 1], d1, w, oc, tot);_scap(vb, ub, cb, pts[pts.size() - 1], d1, w * 0.80, bc, tot)
        _set_mesh(fill_mi, vo, uo, co)
        _set_mesh(dash_mi, vb, ub, cb)


class LiveNode extends ItemLayer:
    var anim_t: = 0.0
    var kind: = ""
    var it
    var sig: = ""
    var st: = 0
    var dash: Array = []
    func upd() -> void :
        var s: = _sig()
        if s == sig: return
        sig = s;queue_redraw()
        if kind == "streams" or kind == "leaks": _rebuild_dash()
    func _process(delta: float) -> void :
        if kind == "vessel":
            var bb = it.get("bind"); var SS = m.sim.S
            anim_t += delta
            if anim_t > 0.07 and bb in ["rvp", "mp", "wp", "sk"]:
                var fxs: Dictionary = fx.get(it.id, {})
                if float(fxs.get("stir", 0.0)) > 0.02 or float(fxs.get("susp", 0.0)) > 0.03 or float(fxs.get("slosh", 0.0)) > 0.1 or (bb != "sk" and (SS[bb].boiling or SS[bb].whirlNow)) or (bb == "sk" and float(SS.sk.turb) > 3.0 and m.sim.skVolTotal() > 50.0) or (bb != "sk" and m.sim.vol(SS[bb]) > 50.0 and (SS[bb].trubSus > 0.05 or (SS[bb].alpha + SS[bb].iso) > 0.5)):
                    anim_t = 0.0;queue_redraw()
            if bb == "rvp" and SS.rvp.stir:
                spin += delta * SS.rvp.hz * 0.8 * 3.0 / 60.0 * TAU;queue_redraw()
            elif bb == "sk" and SS.sk.rake:
                spin += delta * m.sim.rkRpm() * 6.0 / 60.0 * TAU;queue_redraw()
        if kind == "pump" and st != 0:
            var b: String = it.get("bind", "")
            var q: float = float(m.sim.S[b].get("q", 0.0)) if b in ["p1", "p2", "p3"] else 100.0
            var qn: float = {"p1": 400.0, "p2": 150.0, "p3": 200.0}.get(b, 200.0)
            spin += delta * ((4.0 + 12.0 * clampf(q / qn, 0.0, 1.5)) if st == 2 else 2.0)
            queue_redraw()
    func _sig() -> String:
        var S = m.sim.S
        match kind:
            "vessel":
                var b = it.get("bind"); var boil: = false
                if b in ["rvp", "mp", "wp"]: boil = S[b].boiling or (S[b].stir and m.sim.vol(S[b]) > 50)
                var ph: = int(Time.get_ticks_msec() / 100) %40 if boil else 0
                var vo: = roundi(S.rvp.vort * 1000.0) if b == "rvp" else 0
                return "%d|%s|%s|%d|%d|%d|%s" % [roundi(m.level_y(it) / 2.0), m.vessel_color(it).to_html(false), m.vessel_text(b), ph, m.label_size(), vo / 5, m.hard_sig(it) + (("|%d%s" % [roundi(S.sk.rakeH * 50.0), str(S.sk.rake)]) if b == "sk" else "")]
            "card":
                var dc: Dictionary = m.vessel_card(it.get("bind"))
                return "%s|%d|%d" % [str(dc), m.label_size(), roundi(m.level_y(it))]
            "pump":
                st = m.pump_state(it)
                return "%d|%d" % [st, 1 if m.pump_primed(it) else 0]
            "streams":
                var o: = []
                for s in _stream_segs(): o.append("%d,%d,%d,%d,%s" % [s[0].x, s[0].y, s[1].x, s[1].y, s[2].to_html()])
                return ";".join(PackedStringArray(o))
            "leaks":
                var ks: = {}
                for p in S.leaks: ks["%d,%d" % [p.x, p.y]] = true
                var arr: = ks.keys();arr.sort()
                return ";".join(PackedStringArray(arr))
        return ""
    func _draw() -> void :
        match kind:
            "vessel":
                _liquid(it);_gauge_levels(it);_internals(it)
            "card":
                _vstatus(it)
            "pump":
                if st == 0:
                    if m.pump_primed(it): draw_circle(Vector2(0, -34), 5.0, Color("2f8fe0"))
                    return
                var col: = Color("2fbf5b") if st == 2 else (Color("2f8fe0") if st == 3 else Color("f39a2b"))
                var tt: float = Time.get_ticks_msec() / 1000.0
                var pulse: float = 0.5 + 0.5 * sin(tt * 5.0)
                # jemná záře podle stavu (zelená = jede, modrá = zavodňuje)
                draw_circle(Vector2.ZERO, 34.0 + 3.0 * pulse, Color(col, 0.07 + 0.06 * pulse))
                # průhledné okénko na hlavě čerpadla: vidět točící se oběžné kolo
                draw_circle(Vector2.ZERO, 20.5, Color("0b0e11"))
                draw_circle(Vector2.ZERO, 19.0, Color(0.16, 0.19, 0.22))
                var nb: = 6
                for k in nb:
                    var a: float = spin + TAU * k / nb
                    # rozmazaná stopa lopatky (motion blur) – čím rychleji, tím delší
                    var trail: float = 0.9
                    for j in 7:
                        var f: float = float(j) / 6.0
                        draw_arc(Vector2.ZERO, 11.5, a - trail * (1.0 - f), a - trail * (1.0 - f) + trail / 6.0 + 0.02, 6, Color(0.78, 0.86, 0.93, 0.05 + 0.28 * f), 14.0, true)
                    var q0: = Vector2(cos(a), sin(a)); var q1: = Vector2(cos(a + 0.35), sin(a + 0.35))
                    draw_polyline(PackedVector2Array([q0 * 4.0, q1 * 11.0, Vector2(cos(a + 0.7), sin(a + 0.7)) * 18.0]), Color("e4eaee"), 2.6, true)
                draw_circle(Vector2.ZERO, 5.0, Color("9aa6af")); draw_circle(Vector2.ZERO, 3.2, Color("dfe6ea")); draw_circle(Vector2(-1, -1), 1.2, Color.WHITE)
                # sklíčko: odlesk + kroužek těsnění
                draw_arc(Vector2.ZERO, 20.0, 0, TAU, 40, Color("6c7780"), 2.0, true)
                draw_arc(Vector2.ZERO, 15.0, PI * 1.15, PI * 1.55, 12, Color(1, 1, 1, 0.45), 2.5, true)
                # LED stavu na motoru + šipka směru toku
                draw_circle(Vector2(30, -38), 3.6, Color("0d1013")); draw_circle(Vector2(30, -38), 2.6, col.lightened(0.15 + 0.2 * pulse))
                var b2: String = it.get("bind", "")
                var txt: = "BĚŽÍ"
                if st == 3: txt = "ZAVODŇUJE"
                elif st == 1: txt = "BĚŽÍ – BEZ PRŮTOKU"
                if st == 2 and b2 in ["p1", "p2", "p3"]: txt = "BĚŽÍ · %d L/min" % roundi(float(m.sim.S[b2].get("q", 0.0)))
                Art.pill(self, m.font_b, "%s %s" % [str(it.get("label", "")), txt], Vector2(0, -58), 13, col.lightened(0.2))
            "streams":
                for s in _stream_segs():
                    draw_line(s[0], s[1], Color(s[2], 0.28), s[3], true)
                    if s[4]: draw_arc(s[1], 11.0, PI, TAU, 12, Color(1, 1, 1, 0.6), 2.0, true)
            "leaks":
                var seen: = {}
                for p in m.sim.S.leaks:
                    var key: = "%d,%d" % [p.x, p.y]
                    if seen.has(key): continue
                    seen[key] = true
                    var pts: = PackedVector2Array()
                    for a in 20: pts.append(p + Vector2(0, 52) + Vector2(cos(TAU * a / 20.0) * 46, sin(TAU * a / 20.0) * 12))
                    draw_colored_polygon(pts, Color(0.35, 0.65, 0.9, 0.45))
                    draw_line(p, p + Vector2(0, 46), Color(0.35, 0.65, 0.9, 0.5), 5.0)
                    draw_arc(p, 22.0, 0, TAU, 24, Color("e0412f"), 3.0, true)
                    Art.pill(self, m.font_b, "VYTÉKÁ!", p + Vector2(0, -28), 13, Color("ff6b5b"))

    func _stream_segs() -> Array:
        var S = m.sim.S; var net = m.sim.net; var out: = []
        for k in S.streams:
            var n = net.nodes.get(k)
            if n == null: continue
            var a = null
            for at in n.att:
                if at.kind == "vessel": a = at
            if a == null or a.low: continue
            var v = net.byId[a.id]; var s = S.streams[k]
            var col: Color = m.kind_color(s.kind, s.T, 0.0)
            col = Color(snappedf(col.r, 0.05), snappedf(col.g, 0.05), snappedf(col.b, 0.05))
            var surf: float = roundf(m.level_y(v) / 4.0) * 4.0
            if a.spray != null:
                var sp = net.byId[a.spray]; var o: = Vector2(sp.x, sp.y)
                for j in 7:
                    var ang: = deg_to_rad(60.0 + j * 10.0)
                    out.append([o, Vector2(roundf(o.x + cos(ang) * (surf - o.y) * 0.6), surf), col, 2.0, false])
            else:
                var x0: = clampf(float(n.x), v.x + 14, v.x + v.w - 14); var y1: float = maxf(n.y, v.y)
                out.append([Vector2(x0, y1), Vector2(x0, surf), col, 7.0, true])
        return out
    func _rebuild_dash() -> void :
        for d in dash: d.queue_free()
        dash.clear()
        var fl = m.flow_layer
        var segs: = []
        if kind == "streams":
            for s in _stream_segs(): segs.append([s[0], s[1], s[2], 3.0 if s[3] < 5 else 5.0])
        else:
            var seen: = {}
            for p in m.sim.S.leaks:
                var key: = "%d,%d" % [p.x, p.y]
                if seen.has(key): continue
                seen[key] = true
                segs.append([p, p + Vector2(0, 46), Color(0.55, 0.8, 1.0), 5.0])
        for s in segs:
            var l: = Line2D.new();l.points = PackedVector2Array([s[0], s[1]]);l.width = s[3]
            l.texture = fl.tex;l.texture_mode = Line2D.LINE_TEXTURE_TILE;l.texture_repeat = CanvasItem.TEXTURE_REPEAT_ENABLED
            l.material = fl.mat;l.default_color = s[2].lightened(0.25)
            add_child(l);dash.append(l)


class ItemLayer extends Node2D:
    var fx: Dictionary = {}   # stav kreslení kapalin (vyhlazení, doznívání)
    var spin: = 0.0
    var m
    var live_only: = false
    var skip_valves: = false
    var valves_only: = false
    var clip: = Rect2()
    func _near(it) -> bool:
        if clip.size == Vector2.ZERO: return true
        if not it.has("x") or not it.has("y"): return false
        var w: float = maxf(float(it.get("w", 0)), float(it.get("len", 0)))
        return clip.intersects(Rect2(it.x - 90.0, it.y - 150.0, w + 180.0, float(it.get("h", 0)) + 300.0))
    func _draw() -> void :
        if valves_only:
            for it in m.items:
                if it.type == "valve": _valve(it)
            return
        if live_only:
            _live_static()
            return
        _baked()



    func _hatch_live(it) -> void :
        var is_open: bool = m.sim.S.sk.get("hatch", false)
        var p: = Vector2(it.x, it.y)
        var gcol: = Color("2fbf5b") if is_open else Color("e0412f")
        draw_circle(p + Vector2(0, 11), 24.0, Color(gcol, 0.18))
        if is_open:
            draw_colored_polygon(PackedVector2Array([p + Vector2(-32, 3), p + Vector2(32, 3), p + Vector2(26, 19), p + Vector2(-26, 19)]), Color("14181c"))
            draw_colored_polygon(PackedVector2Array([p + Vector2(-32, 3), p + Vector2(-48, 24), p + Vector2(-38, 28), p + Vector2(-26, 8)]), gcol)
        else:
            Art.rrect(self, Rect2(p - Vector2(32, -4), Vector2(64, 14)), gcol, 3.0)
        Art.pill(self, m.font_b, "výhoz mláta: %s" % ("OTEVŘEN" if is_open else "zavřen"), p + Vector2(112, 12), 11, gcol)

    func _splid_live(it) -> void :
        var open: bool = m.sim.S.sp.get("lidOpen", false)
        var pr: float = m.sim.S.sp.get("press", 0.0)
        var p: = Vector2(it.x, it.y)
        var gcol: = Color("2fbf5b") if open else (Color("e0412f") if pr > 40.0 else Color("f2c230"))
        # hrdlo (komín)
        Art.rrect_border(self, Rect2(p + Vector2(-30, -4), Vector2(60, 14)), Color("aab4bc"), Color("3b444c"), 2, 3.0)
        if open:
            draw_colored_polygon(PackedVector2Array([p + Vector2(-26, -2), p + Vector2(26, -2), p + Vector2(22, 6), p + Vector2(-22, 6)]), Color("14181c"))
            # víko odklopené na pantu vlevo
            var hp: = p + Vector2(-30, -4)
            draw_colored_polygon(PackedVector2Array([hp, hp + Vector2(-8, -44), hp + Vector2(2, -46), hp + Vector2(14, -2)]), Color("c3ccd3"))
            draw_polyline(PackedVector2Array([hp, hp + Vector2(-8, -44), hp + Vector2(2, -46), hp + Vector2(14, -2), hp]), Color("3b444c"), 2.0, true)
        else:
            Art.rrect_border(self, Rect2(p + Vector2(-34, -16), Vector2(68, 14)), Color("c3ccd3"), Color("3b444c"), 2, 4.0)
            draw_circle(p + Vector2(0, -9), 4.0, Color("59626a"))
        Art.pill(self, m.font_b, "víčko: %s%s" % ["OTEVŘENÉ" if open else "zavřené", ("  přetlak %d mbar!" % roundi(pr)) if (not open and pr > 5.0) else ""], p + Vector2(0, -62), 11, gcol)

    func _live_static() -> void :
        for it in m.items:
            if it.type == "deco" and it.get("k", "") == "hatch": _hatch_live(it)
            if it.type == "deco" and it.get("k", "") == "splid": _splid_live(it)
        for it in m.items:
            if it.type == "deco" and it.get("k", "") == "conv": _malt_gate(it)
        for it in m.items:
            match it.type:
                "dip": _dip(it)
                "gauge": _gauge_cocks(it)
                "sw": _sw(it)
                "vessel":
                    if it.get("shape") == "inlet": _inlet(it)
        for it in m.items:
            if it.type == "pipe" and it.get("hose", false): _hose(it)
            elif it.type == "pipe" and it.get("look", "") == "hose": _hose_vis(it)
        for o in m.outlets: _outlet(o)

    func _baked() -> void :

        for it in m.items:
            if it.type == "vessel" and it.get("shape") not in ["platform", "inlet"] and _near(it):
                var w: float = it.get("w", 100); var h: float = it.get("h", 100)
                if it.get("shape") in ["plate", "chiller"]: Art.ellipse_shadow(self, Vector2(it.x + w * 0.5 + 12, it.y + h + 26), w * 0.55, 18.0)
                else: Art.floor_shadow(self, Vector2(it.x + w * 0.5 + 10, m.foot_y(it) - 2.0), w * 0.52, 9.0)
        for it in m.items:
            if not _near(it): continue
            match it.type:
                "vessel":
                    if it.get("shape") != "inlet": _vessel(it)
                "pump": _pump(it)
                "spray": _spray(it)
                "nozzle": _nozzle(it)
                "panel": pass
                "manifold": _manifold(it)
                "gauge": _gauge(it)
                "deco": _deco(it)
                "rake": _rake(it)
                "fitting", "inl", "hopf", "flow": _small(it)
        if not skip_valves:
            for it in m.items:
                if it.type == "valve": _valve(it)


    func _vessel(it) -> void :
        var x: float = it.x; var y: float = it.y; var w: float = it.get("w", 100); var h: float = it.get("h", 100)
        var shape: String = it.get("shape", "tank"); var bind = it.get("bind")
        var cx: = x + w * 0.5
        match shape:
            "platform":

                draw_rect(Rect2(x, y, w, h), Color("3b434b"))
                var i: = 0.0
                while i < w:
                    draw_colored_polygon(PackedVector2Array([Vector2(x + i, y + h), Vector2(x + i + 12, y), Vector2(x + i + 24, y), Vector2(x + i + 12, y + h)]), Color("f2c230"))
                    i += 36.0
                draw_rect(Rect2(x, y, w, h), Color("15191d"), false, 2.0)
                Art.pill(self, m.font, it.label, Vector2(x + w - 50, y - 8), 12)
                return
            "inlet":
                Art.rrect_border(self, Rect2(x, y, w, h), Color("2457b8"), Color("0e2a5c"), 2, 6.0)
                Art.text_c(self, m.font_b, "💧 řad" if false else "ŘAD", Vector2(cx, y + h * 0.5 + 6), 14, Color.WHITE)
                Art.pill(self, m.font, it.label, Vector2(cx, y - 10), 12)
                return
            "plate":
                _plate(it);return
            "chiller":
                _chiller(it);return
        var stops: Array = Art.STAINLESS
        if bind == "hlt": stops = Art.CLAD
        elif shape == "horiz" or it.get("dark", false): stops = Art.NAVY

        if shape != "horiz":
            var fy_: float = m.foot_y(it)
            for lx in [x + w * 0.12, x + w * 0.88 - 14]:
                var ll_: = fy_ - 8.0 - (y + h - 10)
                Art.cyl_v(self, Rect2(lx, y + h - 10, 14, ll_), Art.STAINLESS, 6)
                draw_rect(Rect2(lx, y + h - 10, 14, ll_), Color("1e2429"), false, 1.0)
                draw_rect(Rect2(lx + 5, fy_ - 9, 4, 5), Color("5d666e"))
                Art.rrect(self, Rect2(lx - 5, fy_ - 5, 24, 5), Color("2b3138"), 2.0)
        else:
            for lx in [x + w * 0.2, x + w * 0.8 - 30]:
                var sp_: = PackedVector2Array([Vector2(lx, y + h * 0.7), Vector2(lx + 30, y + h * 0.7), Vector2(lx + 40, y + h + 30), Vector2(lx - 10, y + h + 30)])
                draw_polygon(sp_, PackedColorArray([Color("5d666e"), Color("3b434b"), Color("2b3138"), Color("4d555c")]))
                draw_rect(Rect2(lx - 14, y + h + 28, 58, 6), Color("2b3138"))
        if shape == "horiz":
            var rr: = h * 0.5
            if not Art.tex_rect(self, "vs_horiz", Rect2(x, y, w, h), Color(0.42, 0.58, 1.0)):
                Art.stadium(self, Rect2(x, y, w, h), stops)
                draw_line(Vector2(x + rr, y + h * 0.22), Vector2(x + w - rr, y + h * 0.22), Color(1, 1, 1, 0.22), h * 0.06)
            for bx in [x + w * 0.3, x + w * 0.7]: draw_line(Vector2(bx, y + 3), Vector2(bx, y + h - 3), Color(0, 0, 0, 0.3), 3.0)
            Art.nameplate(self, m.font_b, it.label, cx, y - 34, 16)
            return

        var cone_h: = 0.0
        var bottom_w: = w
        if shape == "tank" and bind == "rvp": cone_h = minf(h * 0.12, 40.0);bottom_w = 20.0
        elif shape == "cone" and bind != "wp": cone_h = minf(h * 0.25, 60.0);bottom_w = 16.0
        var body: = Rect2(x, y, w, h - cone_h)
        var tint: = Color(0.86, 0.9, 1.0) if bind == "hlt" else Color.WHITE
        if not Art.tex_rect(self, "vs_body", body, tint):
            Art.cyl_v(self, body, stops, 40)
            Art.brushed(self, body)
        draw_rect(Rect2(x + w * 0.73, y + 6, w * 0.035, body.size.y - 12), Color(1, 1, 1, 0.10))
        draw_rect(Rect2(x + w * 0.79, y + 6, w * 0.012, body.size.y - 12), Color(1, 1, 1, 0.08))
        if cone_h > 0.0:
            if not Art.tex_rect(self, "vs_cone", Rect2(x, y + h - cone_h, w, cone_h)):
                Art.cone(self, cx, y + h - cone_h, w, bottom_w, cone_h, stops)
            Art.cyl_v(self, Rect2(cx - bottom_w * 0.5 - 3, y + h - 2, bottom_w + 6, 8), Art.STAINLESS, 6)
        else:
            Art.cyl_v(self, Rect2(x, y + h - 10, w, 10), stops, 24)
            draw_rect(Rect2(x, y + h - 10, w, 10), Color(0, 0, 0, 0.22))

        for k in [0.08, 0.5, 0.92]:
            var by: float = y + (h - cone_h) * k
            draw_line(Vector2(x, by), Vector2(x + w, by), Color(0, 0, 0, 0.22), 3.0)
            draw_line(Vector2(x, by + 2), Vector2(x + w, by + 2), Color(1, 1, 1, 0.12), 1.0)

        var o: = PackedVector2Array([Vector2(x, y), Vector2(x, y + h - cone_h)])
        if cone_h > 0.0: o.append_array([Vector2(cx - bottom_w * 0.5, y + h), Vector2(cx + bottom_w * 0.5, y + h)])
        else: o.append_array([Vector2(x, y + h), Vector2(x + w, y + h)])
        o.append_array([Vector2(x + w, y + h - cone_h), Vector2(x + w, y)])
        draw_polyline(o, Color("0d1013"), 3.0, true)

        var lid_stops: Array = Art.COPPER if bind in ["rvp", "sk"] else stops
        if shape in ["tank", "lauter", "tall", "cone"]:
            var dh: = 14.0 if bind == "wp" else (34.0 if bind != "hlt" else 24.0)
            var cop: = Color(1.22, 0.78, 0.6) if bind in ["rvp", "sk"] else tint
            if not Art.tex_rect(self, "vs_dome", Rect2(cx - (w + 8) * 0.5, y - dh, w + 8, dh + 1), cop):
                Art.dome(self, cx, y, w + 8, dh, lid_stops)
            Art.cyl_v(self, Rect2(x - 6, y - 4, w + 12, 8), Art.STAINLESS, 24)
            draw_rect(Rect2(x - 6, y - 4, w + 12, 8), Color("1e2429"), false, 1.5)
            var bx_: = x + 4.0
            while bx_ < x + w:
                draw_circle(Vector2(bx_, y), 1.6, Color("3b434a"));bx_ += 22.0
        elif bind == "wp":
            draw_rect(Rect2(x - 4, y - 6, w + 8, 8), Art.grad(stops, 0.3).darkened(0.1))

        if bind in ["rvp", "sk", "wp", "hlt"]:
            var gx: = x + w - 26
            Art.rrect(self, Rect2(gx, y + 30, 12, (h - cone_h) - 60), Color(0.55, 0.78, 0.9, 0.35), 4.0)
            draw_rect(Rect2(gx, y + 30, 12, (h - cone_h) - 60), Color("0d1013"), false, 1.5)
        Art.nameplate(self, m.font_b, it.label, cx, y + 16, 16)
        _vdetails(it, x, y, w, h, cone_h, bottom_w)


    ## armatury na víku: CIP kulová hlavice, odvzdušnění s víčkem, průhledítko (stejné prvky jako ve 3D)
    func _lid_fit(px: float, py: float, kind: String) -> void :
        var dark: = Color("0d1013"); var bolt: = Color("3b434a")
        if kind == "cip":
            Art.rrect(self, Rect2(px - 15, py - 4, 30, 7), Color("8e98a2"), 2.0)
            Art.cyl_v(self, Rect2(px - 8, py - 24, 16, 22), Art.STAINLESS, 8)
            draw_rect(Rect2(px - 8, py - 24, 16, 22), dark, false, 1.2)
            Art.rrect(self, Rect2(px - 13, py - 29, 26, 6), Color("aeb7bf"), 2.0)
            for k in 4: draw_circle(Vector2(px - 9 + k * 6.0, py - 26), 1.2, bolt)
            Art.cyl_v(self, Rect2(px - 5, py - 37, 10, 8), Art.STAINLESS, 6)
        elif kind == "vent":
            Art.rrect(self, Rect2(px - 13, py - 4, 26, 7), Color("8e98a2"), 2.0)
            Art.cyl_v(self, Rect2(px - 7, py - 30, 14, 28), Art.STAINLESS, 8)
            draw_rect(Rect2(px - 7, py - 30, 14, 28), dark, false, 1.2)
            Art.cyl_v(self, Rect2(px - 11, py - 40, 22, 11), Art.STAINLESS, 10)
            draw_rect(Rect2(px - 11, py - 40, 22, 11), dark, false, 1.2)
            draw_circle(Vector2(px, py - 43), 6.0, Color("c7cfd6")); draw_arc(Vector2(px, py - 43), 6.0, 0, TAU, 16, dark, 1.2, true)
        elif kind == "sight":
            Art.rrect(self, Rect2(px - 22, py - 8, 44, 10), Color("8e98a2"), 3.0)
            Art.rrect(self, Rect2(px - 16, py - 13, 32, 6), Color(0.62, 0.78, 0.9, 0.55), 3.0)
            draw_line(Vector2(px - 11, py - 11), Vector2(px - 2, py - 11), Color(1, 1, 1, 0.7), 1.5)
            for k in 3: draw_circle(Vector2(px - 18 + k * 18.0, py - 3), 1.4, bolt)

    func _vdetails(it, x: float, y: float, w: float, h: float, cone_h: float, bottom_w: float) -> void :
        var bind = it.get("bind"); var shape: String = it.get("shape", "tank"); var cx: = x + w * 0.5
        var bh: float = h - cone_h
        var dark: = Color("0d1013"); var bolt: = Color("3b434a")
        # válcovaný lem víka: světlý a tmavý proužek
        if shape in ["tank", "lauter", "tall", "cone"]:
            draw_line(Vector2(x - 6, y - 3), Vector2(x + w + 6, y - 3), Color(1, 1, 1, 0.32), 1.5)
            draw_line(Vector2(x - 6, y + 3), Vector2(x + w + 6, y + 3), Color(0, 0, 0, 0.3), 1.5)
            if bind != "wp":
                var dh: float = 34.0 if bind != "hlt" else 24.0
                var rr: float = (w + 8) * 0.5
                for f in [[0.15, "cip"], [-0.30, "vent"], [-0.12, "sight"]]:
                    if bind == "lv" and f[1] == "cip": continue
                    var dx: float = w * float(f[0]); var k: float = clampf(absf(dx) / rr, 0.0, 0.98)
                    _lid_fit(cx + dx, y - dh * sqrt(1.0 - k * k) + 2.0, str(f[1]))
        # průlez na levém okraji pláště: příruba, šrouby, panty a madlo
        var ym: float = y + bh * 0.58
        var mr: float = minf(36.0, bh * 0.2)
        Art.cyl_h(self, Rect2(x - 14, ym - mr, 14, mr * 2.0), Art.STAINLESS, 10)
        draw_rect(Rect2(x - 14, ym - mr, 14, mr * 2.0), dark, false, 1.5)
        for k in 6: draw_circle(Vector2(x - 7, ym - mr + 6 + k * (mr * 2.0 - 12) / 5.0), 1.6, bolt)
        Art.rrect(self, Rect2(x - 22, ym - mr * 0.82, 9, mr * 1.64), Color("aeb7bf"), 3.0)
        draw_rect(Rect2(x - 22, ym - mr * 0.82, 9, mr * 1.64), dark, false, 1.2)
        Art.rrect(self, Rect2(x - 20, ym - mr - 9, 12, 9), Color("6c757d"), 2.0)
        draw_line(Vector2(x - 17, ym - 3), Vector2(x - 30, ym - 3), Color("2b3138"), 3.0, true)
        draw_circle(Vector2(x - 31, ym - 3), 3.0, Color("c7cfd6"))
        # čidlo teploty PT100: jímka, hlavice a kabel na pravém okraji
        var sy: float = y + bh * 0.30
        draw_line(Vector2(x + w, sy), Vector2(x + w + 14, sy), Color("8e98a2"), 5.0, true)
        Art.rrect(self, Rect2(x + w + 12, sy - 9, 14, 18), Color("20262b"), 3.0)
        draw_rect(Rect2(x + w + 12, sy - 9, 14, 18), Color("5d666e"), false, 1.2)
        draw_circle(Vector2(x + w + 19, sy - 3), 2.0, Color("c7cfd6"))
        var cab: = PackedVector2Array([Vector2(x + w + 19, sy + 9), Vector2(x + w + 19, sy + 22), Vector2(x + w + 11, sy + 34), Vector2(x + w + 11, sy + 50)])
        draw_polyline(cab, Color("15191d"), 2.0, true)
        # spodní výpust s tri-clamp spojem
        if cone_h > 0.0:
            var yb: float = y + h
            Art.cyl_v(self, Rect2(cx - 8, yb + 5, 16, 14), Art.STAINLESS, 6)
            for fy in [yb + 7.0, yb + 14.0]:
                Art.cyl_v(self, Rect2(cx - 14, fy, 28, 4), Art.STAINLESS, 8)
                draw_rect(Rect2(cx - 14, fy, 28, 4), dark, false, 1.0)
            draw_rect(Rect2(cx - 15, yb + 10.5, 30, 4), Color("8e98a2"))
            draw_circle(Vector2(cx + 15, yb + 12.5), 3.5, Color("c7cfd6")); draw_arc(Vector2(cx + 15, yb + 12.5), 3.5, 0, TAU, 12, dark, 1.0, true)
        # vzpěry nohou a šrouby patek
        if shape != "horiz":
            var fy_: float = m.foot_y(it)
            var l1: float = x + w * 0.12 + 7.0; var l2: float = x + w * 0.88 - 7.0
            var ytop: float = y + h - 10.0
            var ll: float = fy_ - 8.0 - ytop
            if ll > 50.0:
                var yb1: float = ytop + ll * 0.35; var yb2: float = ytop + ll * 0.80
                draw_line(Vector2(l1, yb2), Vector2(l2, yb2), Color("2b3138"), 4.0)
                draw_line(Vector2(l1, yb2 - 1.5), Vector2(l2, yb2 - 1.5), Color(1, 1, 1, 0.25), 1.0)
                draw_line(Vector2(l1, yb1), Vector2(l2, yb2), Color("2b3138"), 2.5, true)
                draw_line(Vector2(l2, yb1), Vector2(l1, yb2), Color("2b3138"), 2.5, true)
            for lx in [x + w * 0.12, x + w * 0.88 - 14]:
                for k in 2: draw_circle(Vector2(lx - 1 + k * 16.0, fy_ - 2.5), 1.3, Color("8e98a2"))

    func _plate(it) -> void :
        var x: float = it.x; var y: float = it.y; var w: float = it.w; var h: float = it.h
        Art.shadow(self, Rect2(x, y, w, h))
        draw_rect(Rect2(x - 6, y - 10, w + 12, 14), Color("2457b8"));draw_rect(Rect2(x - 6, y + h - 4, w + 12, 14), Color("2457b8"))
        Art.cyl_v(self, Rect2(x, y, w, h), Art.STAINLESS, 8)
        var yy: = y + 6.0
        while yy < y + h - 4:
            draw_line(Vector2(x + 3, yy), Vector2(x + w - 3, yy), Color(0, 0, 0, 0.35), 2.0);yy += 7.0
        draw_rect(Rect2(x, y, w, h), Color("0d1013"), false, 2.0)
        Art.pill(self, m.font, it.label, Vector2(x + w * 0.5, y + h + 30), 12)

    func _chiller(it) -> void :
        var r: = Rect2(it.x, it.y, it.w, it.h)
        Art.shadow(self, r)
        Art.rrect_border(self, r, Color("d9dee2"), Color("6c757d"), 3, 8.0)
        for k in 2:
            var c: = r.position + Vector2(r.size.x * (0.3 + 0.4 * k), r.size.y * 0.5)
            draw_circle(c, r.size.y * 0.32, Color("2b3138"));draw_arc(c, r.size.y * 0.32, 0, TAU, 32, Color("8a939b"), 3.0, true)
            for b in 4:
                var a: = TAU * b / 4.0 + 0.4
                draw_line(c, c + Vector2(cos(a), sin(a)) * r.size.y * 0.28, Color("aab2ba"), 6.0, true)
        Art.nameplate(self, m.font_b, it.label, r.position.x + r.size.x * 0.5, r.position.y - 34, 14)


    func _malt_gate(it) -> void :
        var is_open: bool = m.sim.S.malt.get("gate", false)
        var g: Vector2 = m.malt_gate_pos(it)
        var gcol: = Color("2fbf5b") if is_open else Color("e0412f")
        draw_circle(g, 20.0, Color(gcol, 0.22))
        Art.rrect(self, Rect2(g.x - 22, g.y - 7, 44, 14), Color("8e98a2"), 3.0)
        var goff: = 0.0 if not is_open else 30.0
        draw_rect(Rect2(g.x - 15 - goff, g.y - 4, 30, 8), gcol)
        draw_rect(Rect2(g.x - 15 - goff, g.y - 4, 30, 8), Color("0d1013"), false, 2.0)
        draw_line(Vector2(g.x - 15 - goff, g.y), Vector2(g.x - 25 - goff, g.y), Color("2b3138"), 3.0, true)
        draw_circle(Vector2(g.x - 28 - goff, g.y), 5.0, Color("c7cfd6"));draw_circle(Vector2(g.x - 28 - goff, g.y), 5.0, Color("0d1013"), false, 1.5)
        Art.pill(self, m.font_b, "šoupě: %s" % ("otevřeno" if is_open else "zavřeno"), g + Vector2(78, 0), 11, gcol)


    func _valve(it) -> void :
        var p: = Vector2(it.x, it.y)
        var kind: String = it.get("kind", "klapka")
        var open: bool = it.get("open", true) != false
        var rot: float = it.get("_ang", deg_to_rad(float(it.get("rot", 0))))
        var ax: = Vector2(cos(rot), sin(rot)); var nx: = Vector2( - ax.y, ax.x)
        var col: = Color("2fbf5b") if open else Color("e0412f")
        var pct: = 100
        if kind == "skrt":
            pct = int(it.get("pct", 50));col = Color("f39a2b") if pct > 0 else Color("e0412f")
        elif it.has("part") and open:
            pct = int(it.part);col = Color("f2c230")
        if it.get("stuck", false): col = col.darkened(0.35)


        var is_open: bool = open and (kind != "skrt" or pct >= 50)
        if Art.sprite(self, ("valveh_" if m.hard else "valve_") + ("open" if is_open else "closed"), p, rot, 0.1643, Vector2(434, 630)):
            if kind == "belimo":
                Art.rrect(self, Rect2(p - nx * 42 - Vector2(10, 8), Vector2(20, 16)), Color("f39a2b"), 3.0)
            if kind == "skrt" or (kind != "skrt" and it.has("part") and open):
                Art.pill(self, m.font_b, ("%d %%" % pct) if pct > 0 else "zavřeno", p + nx * 30 + Vector2(0, 6), 12, Color("f39a2b") if kind == "skrt" else Color("f2c230"))
            if m.hl.has(it.id) and not open: draw_arc(p, 25.0, 0, TAU, 28, Color("f2c230"), 4.0, true)
            return
        var t1: = PackedVector2Array([p - ax * 13 + nx * 9, p - ax * 13 - nx * 9, p])
        var t2: = PackedVector2Array([p + ax * 13 + nx * 9, p + ax * 13 - nx * 9, p])
        draw_colored_polygon(t1, col);draw_colored_polygon(t2, col)
        var o1: = t1.duplicate();o1.append(t1[0]); var o2: = t2.duplicate();o2.append(t2[0])
        draw_polyline(o1, Color("0d1013"), 2.0, true);draw_polyline(o2, Color("0d1013"), 2.0, true)

        draw_line(p, p - nx * 14, Color("2b3138"), 3.0, true)
        if kind == "kulovy":
            var la: = (ax if open else nx * -1.0)
            draw_line(p - nx * 14, p - nx * 14 + la * 18, Color("e0412f"), 5.0, true)
        elif kind == "belimo":
            Art.rrect(self, Rect2(p - nx * 26 - Vector2(10, 8), Vector2(20, 16)), Color("f39a2b"), 3.0)
        elif kind == "skrt":
            var ang: = rot - PI * 0.5 + PI * 0.5 * pct / 100.0
            draw_line(p - nx * 14, p - nx * 14 + Vector2(cos(ang), sin(ang)) * 18, Color("f39a2b"), 5.0, true)
            Art.pill(self, m.font_b, ("%d %%" % pct) if pct > 0 else "zavřeno", p + nx * 30 + Vector2(0, 6), 12, Color("f39a2b"))
        else:
            draw_circle(p - nx * 14, 4.0, Color("c7cfd6"))
        if kind != "skrt" and it.has("part") and open:
            Art.pill(self, m.font_b, "%d %%" % pct, p + nx * 30 + Vector2(0, 6), 12, Color("f2c230"))
        draw_circle(p, 3.0, Color("0d1013"))
        if m.hl.has(it.id) and not open: draw_arc(p, 25.0, 0, TAU, 28, Color("f2c230"), 4.0, true)


    ## hadice k tryskám pod síty: jen vzhled (v hydraulice zůstává trubka)
    func _hose_vis(it) -> void :
        var c: = Curve2D.new()
        var hp: Array = []
        for q in it.pts: hp.append(Vector2(q[0], q[1]))
        if hp.size() == 2:
            # krátká hadice mezi dvěma body: mírné S (jako na fotce zespodu)
            var a0: Vector2 = hp[0]; var b0: Vector2 = hp[1]; var dd: = b0 - a0
            var nn0: = Vector2( - dd.y, dd.x).normalized()
            var sg: float = 1.0 if int(a0.x) % 2 == 0 else -1.0
            hp = [a0, a0 + dd * 0.33 + nn0 * 5.0 * sg, a0 + dd * 0.67 - nn0 * 5.0 * sg, b0]
        var n: int = hp.size()
        for i in n:
            var p: Vector2 = hp[i]
            var t: = Vector2.ZERO
            if i > 0 and i < n - 1:
                t = (hp[i + 1] - hp[i - 1]) * 0.22
            c.add_point(p, - t, t)
        var pts: = c.tessellate(4, 3.0)
        draw_polyline(pts, Color(0, 0, 0, 0.16), 16.0, true)
        draw_polyline(pts, Color("3a434b"), 12.0, true)
        draw_polyline(pts, Color("aeb7bf"), 9.5, true)
        draw_polyline(pts, Color(1, 1, 1, 0.55), 2.6, true)
        # nerezový oplet: hustá křížová pletenina (světlé + tmavé vlákno) jako na fotce
        for i in range(1, pts.size()):
            var d: = pts[i] - pts[i - 1]
            var l: = d.length()
            if l < 0.01: continue
            var u: = d / l
            var nn: = Vector2( - u.y, u.x)
            var q: = 0.0
            while q < l:
                var pp: = pts[i - 1] + u * q
                draw_line(pp - nn * 4.6 - u * 2.2, pp + nn * 4.6 + u * 2.2, Color(0.12, 0.15, 0.18, 0.55), 1.1)
                draw_line(pp - nn * 4.6 + u * 2.2, pp + nn * 4.6 - u * 2.2, Color(1, 1, 1, 0.50), 1.1)
                q += 2.6
        draw_polyline(pts, Color(1, 1, 1, 0.35), 1.6, true)
        # lisované objímky na koncích
        for k in 2:
            var ee: Vector2 = hp[0] if k == 0 else hp[n - 1]
            var nb: Vector2 = pts[mini(3, pts.size() - 1)] if k == 0 else pts[maxi(0, pts.size() - 4)]
            var dr: = (nb - ee).normalized()
            var nr: = Vector2( - dr.y, dr.x)
            var e2: = ee + dr * 9.0
            draw_line(ee, e2, Color("3a434b"), 14.0);draw_line(ee, e2, Color("c3ccd3"), 11.5)
            draw_line(ee + nr * 3.0, e2 + nr * 3.0, Color(1, 1, 1, 0.7), 2.0)
            draw_line(ee + dr * 3.0 - nr * 6.0, ee + dr * 3.0 + nr * 6.0, Color("5a646c"), 1.2)
            draw_line(ee + dr * 6.0 - nr * 6.0, ee + dr * 6.0 + nr * 6.0, Color("5a646c"), 1.2)

    func _hose(it) -> void :
        var c: = Curve2D.new()
        var n: int = it.pts.size()
        for i in n:
            var p: = Vector2(it.pts[i][0], it.pts[i][1])
            var t: = Vector2.ZERO
            if i > 0 and i < n - 1:
                t = (Vector2(it.pts[i + 1][0], it.pts[i + 1][1]) - Vector2(it.pts[i - 1][0], it.pts[i - 1][1])) * 0.22
            c.add_point(p, - t, t)
        var pts: = c.tessellate(4, 3.0)
        var w: = 11.0 if float(it.get("dn", 50)) <= 40 else 13.0
        draw_polyline(pts, Color(0, 0, 0, 0.18), w + 10.0, true)
        draw_polyline(pts, Color("3a0c0a"), w + 4.0, true)
        draw_polyline(pts, Color("b3312a"), w, true)
        draw_polyline(pts, Color(1, 1, 1, 0.22), maxf(1.5, w * 0.25), true)

        for idx in [0, n - 1]:
            var e: = Vector2(it.pts[idx][0], it.pts[idx][1])
            var on = m.outlet_at(e, 2.0) != null
            if not on:
                on = m.pipe_pts.has(Vector2i(e.round()))
                if not on:
                    draw_arc(e, 16.0, 0, TAU, 24, Color("e0412f"), 3.0, true)
            draw_circle(e, 9.0, Color("6b4a10"));draw_circle(e, 7.0, Color("d8a13a"))


    func _outlet(o) -> void :
        var p: Vector2 = o.front
        var open: bool = m.outlet_open.get(o.key, false)
        var col: = Color("2fbf5b") if open else Color("e0412f")
        draw_circle(p, 13.0, Color("0d1013"));draw_circle(p, 11.0, Art.grad(Art.STAINLESS, 0.35))
        draw_arc(p, 11.0, 0, TAU, 24, col, 3.0, true)

        var la: = Vector2(0, -1) if open else Vector2(1, 0)
        draw_line(p, p + la * 22.0, Color("0d1013"), 7.0, true)
        draw_line(p, p + la * 22.0, col, 4.5, true)
        draw_circle(p, 4.0, Color("2b3138"))
        if m.hl.has(o.key) and not open: draw_arc(p, 22.0, 0, TAU, 28, Color("f2c230"), 4.0, true)


    func _pump(it) -> void :
        var p: = Vector2(it.x, it.y)
        var a: = deg_to_rad(float(it.get("dir", 0)))

        if Art.sprite(self, "pump_inline", p, 0.0, 0.1214, Vector2(700, 1056)):
            var f2: = Vector2(cos(a), sin(a))
            draw_colored_polygon(PackedVector2Array([p + f2 * 30 + Vector2(-f2.y, f2.x) * 5, p + f2 * 38, p + f2 * 30 - Vector2(-f2.y, f2.x) * 5]), Color(0.95, 0.76, 0.19, 0.9))
            Art.pill(self, m.font_b, it.get("label", ""), p + Vector2(0, 48), 13, Color("f2c230"))
            return
        draw_set_transform(p, a, Vector2.ONE)
        Art.cyl_h(self, Rect2(-26, -5, 52, 10), Art.STAINLESS, 8)
        draw_rect(Rect2(-26, -5, 52, 10), Color("1e2429"), false, 1.0)
        for fx in [-27.0, 21.0]:
            Art.cyl_h(self, Rect2(fx, -8, 6, 16), Art.STAINLESS, 6)
            draw_rect(Rect2(fx, -8, 6, 16), Color("1e2429"), false, 1.0)
        draw_set_transform(Vector2.ZERO, 0, Vector2.ONE)

        var mc: = p + Vector2(2, -31)
        # lucerna + spojka
        Art.cyl_v(self, Rect2(p.x - 7, p.y - 21, 14, 10), Art.STAINLESS, 6)
        draw_line(Vector2(p.x - 7, p.y - 16), Vector2(p.x + 7, p.y - 16), Color(0, 0, 0, 0.35), 1.0)
        # motor: žebrovaná kostra, kryt ventilátoru, svorkovnice
        var body: = Rect2(mc.x - 21, mc.y - 13, 44, 26)
        Art.cyl_h(self, body, Art.NAVY, 12)
        var fx2: = body.position.x + 4.0
        while fx2 < body.end.x - 2.0:
            draw_line(Vector2(fx2, body.position.y + 2), Vector2(fx2, body.end.y - 2), Color(0, 0, 0, 0.28), 1.5)
            draw_line(Vector2(fx2 + 1.5, body.position.y + 2), Vector2(fx2 + 1.5, body.end.y - 2), Color(1, 1, 1, 0.10), 1.0)
            fx2 += 5.0
        draw_rect(body, Color("161b20"), false, 1.5)
        var fan: = Rect2(body.position.x - 11, mc.y - 11, 12, 22)
        Art.cyl_h(self, fan, Art.NAVY, 8)
        draw_rect(fan, Color(0, 0, 0, 0.25))
        for gy in [-7.0, -3.5, 0.0, 3.5, 7.0]: draw_line(Vector2(fan.position.x + 2, mc.y + gy), Vector2(fan.end.x - 2, mc.y + gy), Color(0, 0, 0, 0.45), 1.0)
        draw_rect(fan, Color("161b20"), false, 1.5)
        var tb: = Rect2(mc.x - 4, body.position.y - 7, 14, 8)
        Art.cyl_h(self, tb, Art.NAVY, 4)
        draw_rect(tb, Color("161b20"), false, 1.0)
        draw_rect(Rect2(body.end.x, mc.y - 8, 3, 16), Color("39424a"))

        # spirální skříň: radiální lesk, hrdlo, šrouby
        var pts: = PackedVector2Array(); var cols: = PackedColorArray()
        for i in 33:
            var ang: = TAU * i / 32.0
            var rr: = 18.0 + 2.0 * (float(i) / 32.0)
            pts.append(p + Vector2(cos(ang), sin(ang)) * rr)
            cols.append(Art.grad(Art.STAINLESS, 0.5 + 0.45 * cos(ang + 2.3)))
        draw_polygon(pts, cols)
        draw_polyline(pts, Color("1e2429"), 2.0, true)
        for k in 8:
            var ba: = TAU * k / 8.0 + 0.2
            draw_circle(p + Vector2(cos(ba), sin(ba)) * 14.5, 1.6, Color("4a535b"))
        draw_circle(p, 8.0, Color("1e2429"));draw_circle(p, 6.8, Color("c9d1d8"));draw_circle(p + Vector2(-1.5, -1.5), 3.0, Color("eef3f6"))

        var f: = Vector2(cos(a), sin(a)); var n: = Vector2( - f.y, f.x)
        draw_colored_polygon(PackedVector2Array([p - f * 6 - n * 8, p + f * 9, p - f * 6 + n * 8]), Color("f2c230"))
        Art.pill(self, m.font_b, it.get("label", ""), p + Vector2(0, 48), 13, Color("f2c230"))
    func _spray(it) -> void :
        var p: = Vector2(it.x, it.y)
        draw_circle(p, 12.0, Color("0d1013"));draw_circle(p, 10.5, Color("c7cfd6"))
        for i in 8:
            var a: = TAU * i / 8.0
            draw_circle(p + Vector2(cos(a), sin(a)) * 6.0, 1.5, Color("2b3138"))
        draw_circle(p, 2.0, Color("2b3138"))

    func _nozzle(it) -> void :
        var p: = Vector2(it.x, it.y); var a: = deg_to_rad(float(it.get("dir", 0)))
        var f: = Vector2(cos(a), sin(a)); var n: = Vector2( - f.y, f.x)
        draw_colored_polygon(PackedVector2Array([p - n * 7, p + n * 7, p + f * 18 + n * 3, p + f * 18 - n * 3]), Color("c7cfd6"))
        draw_polyline(PackedVector2Array([p - n * 7, p + f * 18 - n * 3, p + f * 18 + n * 3, p + n * 7, p - n * 7]), Color("0d1013"), 1.5, true)


    func _panel(it) -> void :
        var r: = Rect2(it.x, it.y, it.get("w", 260), it.get("h", 320))
        Art.shadow(self, r, Vector2(16, 20), 24.0)
        Art.rrect_border(self, r, Color("b9c0c6"), Color("59626a"), 6, 10.0)
        draw_line(Vector2(r.position.x + r.size.x * 0.5, r.position.y + 90), Vector2(r.position.x + r.size.x * 0.5, r.end.y - 20), Color(0, 0, 0, 0.18), 3.0)
        Art.rrect(self, Rect2(r.position.x + 30, r.position.y + 24, r.size.x - 60, 56), Color("1b2026"), 6.0)
        Art.text_c(self, m.font_b, it.get("label", "ROZVADĚČ"), Vector2(r.position.x + r.size.x * 0.5, r.position.y + 62), 30, Color("f2c230"))
        var cols: = [Color("2fbf5b"), Color("e0412f"), Color("f2c230"), Color("2e7bd6"), Color("eef1f4"), Color("2fbf5b")]
        for row in 8:
            for col in 6:
                var c: = r.position + Vector2(95 + col * 140, 180 + row * 185)
                if c.x < r.end.x - 50 and c.y < r.end.y - 80:
                    Art.rrect_border(self, Rect2(c - Vector2(48, 50), Vector2(96, 130)), Color("eceff1"), Color("8a939b"), 2, 6.0)
                    draw_rect(Rect2(c - Vector2(36, 42), Vector2(72, 18)), Color("1b2026"))
                    draw_circle(c + Vector2(0, 30), 26.0, Color("2b3138"))
                    draw_circle(c + Vector2(0, 30), 21.0, cols[(row + col) %cols.size()])
                    draw_circle(c + Vector2(-7, 23), 6.0, Color(1, 1, 1, 0.35))

    func _manifold(it) -> void :
        var g: = float(it.get("gap", 60)); var cols: = int(it.get("cols", 4)); var rows: = int(it.get("rows", 3))

        var W: = cols * g; var H: = rows * g
        var ctr: = Vector2(it.x + W * 0.5, it.y + H * 0.5)
        var rot: = deg_to_rad(float(it.get("rot", 0)))
        var bw: = absf(cos(rot)) * W + absf(sin(rot)) * H; var bh: = absf(cos(rot)) * H + absf(sin(rot)) * W
        Art.shadow(self, Rect2(ctr - Vector2(bw, bh) * 0.5, Vector2(bw, bh)))
        draw_set_transform(ctr, rot, Vector2.ONE)
        var r: = Rect2( - W * 0.5, - H * 0.5, W, H)
        Art.rrect_border(self, r.grow(6), Color("1f252b"), Color("6c757d"), 2, 9.0)
        Art.rrect(self, r, Color("c9d0d6"), 6.0)
        for ry in rows:
            for cx in cols:
                var c: = Vector2( - W * 0.5 + g * 0.5 + cx * g, - H * 0.5 + g * 0.5 + ry * g)

                draw_circle(c, 21.0, Color("0d1013"));draw_circle(c, 19.0, Color("9aa3ab"))
                for b in 4:
                    var a: = TAU * b / 4.0 + PI * 0.25
                    draw_circle(c + Vector2(cos(a), sin(a)) * 15.0, 2.2, Color("3b434b"))
                draw_circle(c, 10.5, Color("15191d"))
        draw_set_transform(Vector2.ZERO, 0, Vector2.ONE)
        var top: = ctr.y - (absf(cos(rot)) * H + absf(sin(rot)) * W) * 0.5
        Art.pill(self, m.font_b, it.label, Vector2(ctr.x, top - 18), 13)


    func _gauge(it) -> void :
        var G: Dictionary = m.gauge_geo(it)
        var x: float = G.x; var y1: float = G.y1; var y2: float = G.y2; var h: = y2 - y1
        var tubeb: = Color("4b555e"); var dark: = Color("23282e")
        if G.free:
            draw_line(Vector2(x, y1 - 30), Vector2(x, y1 - 6), Color("8a939b"), 5.0);draw_line(Vector2(x, y2 + 6), Vector2(x, y2 + 30), Color("8a939b"), 5.0)
            Art.rrect_border(self, Rect2(x - 6, y1 - 6, 12, h + 12), Color(0.62, 0.83, 0.94, 0.28), tubeb, 2, 5.0)
            draw_circle(Vector2(x, y1 - 30), 4.5, Color("b9c2ca"));draw_circle(Vector2(x, y2 + 30), 4.5, Color("b9c2ca"))
            draw_arc(Vector2(x, y1 - 30), 4.5, 0, TAU, 14, dark, 1.5, true);draw_arc(Vector2(x, y2 + 30), 4.5, 0, TAU, 14, dark, 1.5, true)
        else:
            var cx: float = G.cx
            for yy in [y1, y2]: draw_line(Vector2(cx, yy), Vector2(x, yy), Color("8a939b"), 5.0)
            Art.rrect_border(self, Rect2(x - 6, y1 - 6, 12, h + 12), Color(0.62, 0.83, 0.94, 0.28), tubeb, 2, 5.0)
            Art.rrect_border(self, Rect2(x - 9, y1 - 12, 18, 7), Art.grad(Art.STAINLESS, 0.4), dark, 1, 2.0)
            Art.rrect_border(self, Rect2(x - 9, y2 + 5, 18, 7), Art.grad(Art.STAINLESS, 0.4), dark, 1, 2.0)
        for i in range(1, 10):
            var yy2: float = y1 + h * i / 10.0
            draw_line(Vector2(x + 6, yy2), Vector2(x + (10 if i % 5 else 14), yy2), Color("6c757d"), 1.2)


    func _gauge_cocks(it) -> void :
        var G: Dictionary = m.gauge_geo(it)
        var dark: = Color("23282e")
        var vt: bool = it.get("top", true) != false; var vb: bool = it.get("bot", true) != false
        if G.free:
            for pair in [[G.y1 - 18.0, vt], [G.y2 + 18.0, vb]]:
                var yy: float = pair[0]; var o: bool = pair[1]
                draw_circle(Vector2(G.x, yy), 7.0, dark);draw_circle(Vector2(G.x, yy), 6.0, Art.grad(Art.STAINLESS, 0.35))
                Art.rrect(self, Rect2(G.x - (2 if o else 6), yy - (6 if o else 2), 4 if o else 12, 12 if o else 4), Color("2fbf5b") if o else Color("d93a2c"), 2.0)
        else:
            var mx: float = (G.cx + G.x) / 2.0
            for pair in [[G.y1, vt], [G.y2, vb]]:
                var yy: float = pair[0]; var o: bool = pair[1]
                draw_circle(Vector2(mx, yy), 7.0, dark);draw_circle(Vector2(mx, yy), 6.0, Art.grad(Art.STAINLESS, 0.35))
                Art.rrect(self, Rect2(mx - 2, yy - (2 if o else 12), 12 if o else 4, 4 if o else 12), Color("2fbf5b") if o else Color("d93a2c"), 2.0)
    func _sw(it) -> void :
        var p: = Vector2(it.x, it.y); var on: bool = it.get("on", false)
        var r: = Rect2(p - Vector2(26, 34), Vector2(52, 68))
        Art.shadow(self, r, Vector2(6, 8), 10.0)
        Art.rrect_border(self, r, Color("e3e6e9"), Color("6c757d"), 2, 6.0)
        draw_circle(p + Vector2(0, -8), 12.0, Color("2b3138"))
        draw_line(p + Vector2(0, -8), p + Vector2(8 if on else -8, -22), Color("e0412f"), 5.0, true)
        draw_circle(p + Vector2(0, 20), 7.0, Color("2fbf5b") if on else Color("3a1a18"))
        Art.pill(self, m.font, it.get("label", ""), p + Vector2(0, 56), 11)

    func _deco(it) -> void :
        var p: = Vector2(it.x, it.y)
        match it.get("k", ""):
            "conv":
                var L: = float(it.get("len", 280))
                Art.cyl_h(self, Rect2(p, Vector2(L, 22)), Art.STAINLESS, 10)
                var xx: = p.x + 8
                while xx < p.x + L - 8:
                    draw_line(Vector2(xx, p.y + 2), Vector2(xx + 10, p.y + 20), Color(0, 0, 0, 0.35), 2.0);xx += 14.0
                draw_rect(Rect2(p, Vector2(L, 22)), Color("0d1013"), false, 2.0)
                Art.pill(self, m.font, it.get("label", ""), p + Vector2(L * 0.5, 36), 11)

                var hx: float = m.malt_gate_pos(it).x
                var fun: = PackedVector2Array([Vector2(hx - 44, p.y - 92), Vector2(hx + 44, p.y - 92), Vector2(hx + 13, p.y - 30), Vector2(hx - 13, p.y - 30)])
                draw_colored_polygon(fun, Color("c9d0d6"))
                draw_colored_polygon(PackedVector2Array([Vector2(hx - 44, p.y - 92), Vector2(hx - 20, p.y - 92), Vector2(hx - 5, p.y - 30), Vector2(hx - 13, p.y - 30)]), Color("eef1f4"))
                draw_polyline(PackedVector2Array([fun[0], fun[1], fun[2], fun[3], fun[0]]), Color("0d1013"), 2.0, true)
                draw_rect(Rect2(hx - 48, p.y - 98, 96, 8), Color("aab2ba"));draw_rect(Rect2(hx - 48, p.y - 98, 96, 8), Color("0d1013"), false, 2.0)
                draw_rect(Rect2(hx - 13, p.y - 30, 26, 30), Color("b4bcc4"));draw_rect(Rect2(hx - 13, p.y - 30, 26, 30), Color("0d1013"), false, 2.0)
                Art.pill(self, m.font, "násypka", Vector2(hx, p.y - 112), 11)
            "vent":
                Art.cyl_v(self, Rect2(p - Vector2(15, 70), Vector2(30, 70)), Art.STAINLESS, 8)
                draw_rect(Rect2(p - Vector2(15, 70), Vector2(30, 70)), Color("0d1013"), false, 2.0)
            "hatch":
                Art.rrect(self, Rect2(p - Vector2(36, -2), Vector2(72, 22)), Color("2b3138"), 4.0)
            "splid": pass
            _:
                draw_circle(p, 10.0, Color("c7cfd6"))

    ## vnitřek nádoby: míchadlo v kotli (rvp) a hrablo s kopačkou na SK – kreslí se přes kapalinu
    func _internals(it) -> void :
        var b = it.get("bind"); var S = m.sim.S
        var x: float = it.x; var w: float = it.w; var cx: float = x + w * 0.5
        var steel: = Color("aab4bc"); var dk: = Color("3b444c")
        if b == "rvp":
            var top: float = it.y - 4.0; var cc: float = minf(it.h * 0.12, 40.0)
            var bot: float = it.y + it.h - cc - 26.0
            var run: bool = S.rvp.stir and S.rvp.w > 200
            # převodovka + motor nad víkem
            Art.rrect_border(self, Rect2(cx - 26, top - 78, 52, 30), Color("2457b8"), dk, 2, 5.0)
            Art.rrect_border(self, Rect2(cx - 14, top - 50, 28, 34), Color("8d979f"), dk, 2, 3.0)
            for k in 4: draw_line(Vector2(cx - 22 + k * 14, top - 76), Vector2(cx - 22 + k * 14, top - 52), Color(0, 0, 0, 0.3), 2.0)
            # hřídel
            Art.cyl_v(self, Rect2(cx - 5, top - 16, 10, bot - top + 16), Art.STAINLESS, 6)
            var a: float = spin
            var yb: float = bot + 16.0
            var Rm: float = w * 0.40
            var sd: = Color("6c7680"); var sdk: = Color("2c3238")
            # náboj s přírubou na dně (jako na fotce)
            draw_colored_polygon(PackedVector2Array([Vector2(cx - 26, yb + 14), Vector2(cx + 26, yb + 14), Vector2(cx + 18, yb + 4), Vector2(cx - 18, yb + 4)]), Color("8d979f"))
            draw_polyline(PackedVector2Array([Vector2(cx - 26, yb + 14), Vector2(cx + 26, yb + 14), Vector2(cx + 18, yb + 4), Vector2(cx - 18, yb + 4), Vector2(cx - 26, yb + 14)]), sdk, 1.5, true)
            for k in 3: draw_circle(Vector2(cx - 16 + k * 16, yb + 10), 2.2, sdk)
            for arm in 2:
                var aa: float = a + arm * PI
                var R: float = Rm if arm == 0 else Rm * 0.9
                var cs: float = cos(aa); var sn: float = sin(aa)
                var back: bool = sn < 0.0
                var cl: Color = sd.darkened(0.3 if back else 0.0)
                var xe: float = cx + cs * R; var xp: float = cx + cs * R * 0.42
                # tenké rameno
                draw_line(Vector2(cx, yb), Vector2(xe, yb + 3), sdk, 8.0, true)
                draw_line(Vector2(cx, yb), Vector2(xe, yb + 3), cl, 5.5, true)
                # perforovaná lopatka (3 otvory) – větší na jedné straně
                var ph: float = 30.0 if arm == 0 else 38.0
                var pl: = PackedVector2Array([Vector2(xp, yb - ph * 0.45), Vector2(xe, yb - ph * 0.7), Vector2(xe, yb + ph * 0.3), Vector2(xp, yb + ph * 0.55)])
                draw_colored_polygon(pl, cl)
                draw_polyline(PackedVector2Array([pl[0], pl[1], pl[2], pl[3], pl[0]]), sdk, 1.3, true)
                for hi in 3:
                    var f: float = 0.35 + hi * 0.25
                    var hx: float = lerpf(xp, xe, f)
                    draw_circle(Vector2(hx, yb - ph * 0.1), 3.6 * (0.45 + 0.55 * absf(sn)), Color(0.05, 0.06, 0.07, 0.9))
            draw_circle(Vector2(cx, yb), 5.5, Color("20262b"));draw_circle(Vector2(cx, yb), 3.0, Color("9aa4ad"))
            # ohnuté oplachové rameno s kuličkovou tryskou (CIP)
            _cip_pipe(Vector2(x + w * 0.2, it.y + 8.0))
        elif b == "sk":
            var sieve: float = it.y + it.h - 3.0 - 22.0
            var rk: float = S.sk.rakeH
            var ry: float = sieve - 46.0 - (1.0 - rk) * 90.0
            var run2: bool = S.sk.rake
            var top2: float = it.y - 4.0
            # pohon: motor + převodovka nad víkem, hřídel se spojkou
            Art.rrect_border(self, Rect2(cx - 30, top2 - 74, 60, 34), Color("2457b8"), dk, 2, 5.0)
            Art.rrect_border(self, Rect2(cx - 16, top2 - 42, 32, 30), Color("8d979f"), dk, 2, 3.0)
            Art.cyl_v(self, Rect2(cx - 6, top2 - 12, 12, ry - top2 + 12), Art.STAINLESS, 6)
            Art.rrect_border(self, Rect2(cx - 9, ry - 14, 18, 20), Color("c3ccd3"), dk, 2, 3.0)
            var half: float = w * 0.5 - 26.0
            var ang: float = spin
            var bm: = Color("565f68"); var bmh: = Color("9aa4ad"); var bmd: = Color("1f252a")
            # příčná trubka s koncovkami na závěsu zdvihu (jako na fotce)
            Art.cyl_h(self, Rect2(cx - 38, ry - 36, 76, 12), Art.STAINLESS, 6)
            draw_rect(Rect2(cx - 38, ry - 36, 76, 12), bmd, false, 1.2)
            Art.rrect_border(self, Rect2(cx - 14, ry - 26, 28, 28), Color("39424a"), bmd, 1, 3.0)
            for arm in 2:
                var aa: float = ang + arm * PI
                var cs: float = cos(aa); var sn: float = sin(aa)
                var ex: float = cx + cs * half
                var front: bool = sn > 0.0
                var cc2: Color = bm if front else bm.darkened(0.3)
                # rameno z hranatého profilu
                draw_line(Vector2(cx, ry + 2), Vector2(ex, ry + 4), bmd, 17.0, true)
                draw_line(Vector2(cx, ry + 2), Vector2(ex, ry + 4), cc2, 14.0, true)
                draw_line(Vector2(cx, ry - 3), Vector2(ex, ry - 1), bmh if front else bmh.darkened(0.3), 3.0, true)
                # nože: svislé plechy s vroubkovanou spodní hranou
                var n: int = int(half / 46.0)
                for i in range(1, n):
                    var tx: float = cx + cs * (half * i / float(n))
                    var wd: float = 4.0 + 12.0 * absf(sn)
                    var kl: float = 42.0
                    var blade: = PackedVector2Array([Vector2(tx - wd, ry + 12), Vector2(tx + wd, ry + 12), Vector2(tx + wd, ry + kl)])
                    var zz: = PackedVector2Array([Vector2(tx - wd, ry + 12)])
                    var steps: int = 4
                    for q in steps + 1:
                        var qx: float = tx - wd + (2.0 * wd) * q / float(steps)
                        zz.append(Vector2(qx, ry + kl - (7.0 if q % 2 == 1 else 0.0)))
                    zz.append(Vector2(tx + wd, ry + 12))
                    draw_colored_polygon(zz, cc2.lightened(0.08))
                    draw_polyline(zz, bmd, 1.1, true)
                # kopačka: plech s nýty a otvory na konci ramene
                var sg: float = 1.0 if cs >= 0.0 else -1.0
                var pw: float = 8.0 + 26.0 * absf(sn)
                var pl: = PackedVector2Array([Vector2(ex - 4, ry + 8), Vector2(ex + 4 * sg, ry + 8), Vector2(ex + 4 * sg, ry + 58), Vector2(ex - sg * pw, ry + 66), Vector2(ex - sg * pw, ry + 30)])
                draw_colored_polygon(pl, cc2.darkened(0.05))
                draw_polyline(PackedVector2Array([pl[0], pl[1], pl[2], pl[3], pl[4], pl[0]]), bmd, 1.4, true)
                for hi in 2: draw_circle(Vector2(ex - sg * (4.0 + pw * 0.3 * (hi + 1)), ry + 40 + hi * 3), 3.0 * (0.4 + 0.6 * absf(sn)), bmd)
            draw_circle(Vector2(cx, ry + 2), 5.0, bmd)
            _cip_pipe(Vector2(x + w * 0.2, it.y + 8.0))

    ## ohnuté oplachové ramínko s kuličkovou tryskou
    func _cip_pipe(p: Vector2) -> void :
        var st: = Color("b9c2ca"); var d: = Color("1f252a")
        draw_line(p, p + Vector2(0, 26), d, 6.0);draw_line(p, p + Vector2(0, 26), st, 4.0)
        draw_arc(p + Vector2(10, 26), 10.0, PI, PI * 0.5, 10, d, 6.0, true);draw_arc(p + Vector2(10, 26), 10.0, PI, PI * 0.5, 10, st, 4.0, true)
        draw_line(p + Vector2(10, 36), p + Vector2(10, 44), d, 6.0);draw_line(p + Vector2(10, 36), p + Vector2(10, 44), st, 4.0)
        draw_circle(p + Vector2(10, 47), 5.0, d);draw_circle(p + Vector2(10, 47), 3.8, Color("dfe6ea"))

    func _rake(it) -> void :
        return
        var x: float = it.x; var y: float = it.y; var w: float = it.get("w", 300); var h: float = it.get("h", 280)
        Art.cyl_v(self, Rect2(x - 6, y, 12, h), Art.STAINLESS, 6)
        Art.cyl_h(self, Rect2(x - w * 0.5, y + h - 36, w, 10), Art.STAINLESS, 6)
        var k: = x - w * 0.5 + 12
        while k < x + w * 0.5 - 6:
            draw_colored_polygon(PackedVector2Array([Vector2(k, y + h - 26), Vector2(k + 6, y + h - 26), Vector2(k + 10, y + h - 8), Vector2(k - 4, y + h - 8)]), Color("aab2ba"))
            k += 30.0
        Art.rrect(self, Rect2(x - 22, y - 40, 44, 36), Color("2457b8"), 5.0)



    func _dip(it) -> void :
        var DG: Dictionary = m.dip_geo(it)
        var x: float = DG.x; var y: float = DG.y; var h: float = DG.h
        var wood: = [[0.0, "7a5228"], [0.35, "d9b07a"], [0.6, "c69355"], [1.0, "6b4a22"]]
        Art.cyl_v(self, Rect2(x - 7, y, 14, h), wood, 6)
        draw_rect(Rect2(x - 7, y, 14, h), Color(0, 0, 0, 0.5), false, 1.0)
        for hl in range(4, 51):
            var ty: = y + h - h * (hl - 3) / 47.0
            if hl in [10, 15, 19, 20, 21, 25, 30]:
                draw_line(Vector2(x - 7, ty), Vector2(x + 9, ty), Color("c0261b"), 2.5)
                draw_string(m.font_b, Vector2(x + 11, ty + 5), str(hl), HORIZONTAL_ALIGNMENT_LEFT, -1, 13, Color("c0261b"))
            elif hl == 40:
                draw_line(Vector2(x - 7, ty), Vector2(x + 9, ty), Color("c0261b"), 2.5)
                draw_string(m.font_b, Vector2(x + 11, ty + 6), "*", HORIZONTAL_ALIGNMENT_LEFT, -1, 18, Color("c0261b"))
            else:
                var big: = hl % 5 == 0
                draw_line(Vector2(x - 7, ty), Vector2(x - 7 + (10 if big else 6), ty), Color(0.1, 0.07, 0.04, 0.55), 1.0)
                if big: draw_string(m.font, Vector2(x + 11, ty + 4), str(hl), HORIZONTAL_ALIGNMENT_LEFT, -1, 10, Color(0.1, 0.07, 0.04, 0.55))


    var gpu_node = null
    func _gpu_body():
        if gpu_node == null:
            gpu_node = LiquidFx.Gpu.new();add_child(gpu_node)
        return gpu_node
    func _gpu_clear() -> void :
        if gpu_node != null and gpu_node.polys.size() > 0: gpu_node.set_polys([])
    func _liquid(it) -> void :
        var b = it.get("bind")
        if not (b in ["rvp", "mp", "sk", "wp", "hlt", "lv", "spilka"]): return
        var S = m.sim.S; var sim = m.sim
        var outline: PackedVector2Array = m.vessel_inside(it)
        var lv: float = m.level_y(it)
        var x: float = it.x; var w: float = it.w; var y0: float = it.y + it.h
        var col: Color = m.vessel_color(it)
        if lv >= y0 - 0.5 and b != "sk":
            _gpu_clear();return
        var tt: = Time.get_ticks_msec() / 1000.0
        var fxs: Dictionary = fx.get(it.id, {})
        var dtf: float = clampf(tt - float(fxs.get("t", tt)), 0.0, 0.25)
        fxs["t"] = tt

        var vort_px: = 0.0
        if b == "rvp": vort_px = S.rvp.vort * (it.h - minf(it.h * 0.12, 40.0)) / 1.06
        var below: = PackedVector2Array()
        var N: = 24
        var vv = S.get(b) if b in ["rvp", "mp", "wp"] else null
        var amp: = 0.0
        if vv != null: amp = 2.6 if vv.boiling else (1.4 if vv.stir else 0.0)
        # klid/pohyb: plynulý náběh a doznívání (kapalina se po zastavení míchání ještě chvíli houpe)
        var stir_t: = 0.0
        if vv != null: stir_t = 1.0 if (vv.stir or vv.boiling or vv.whirlNow) else 0.0
        elif b == "sk": stir_t = 1.0 if S.sk.rake else 0.0
        var stirK: float = float(fxs.get("stir", 0.0))
        stirK += (stir_t - stirK) * minf(1.0, dtf * (1.6 if stir_t > stirK else 0.45))
        fxs["stir"] = stirK
        # houpání hladiny: změna hladiny rozhoupe kapalinu, doznívá
        var lvp: float = float(fxs.get("lv", lv))
        var slosh: float = float(fxs.get("slosh", 0.0)) * exp( - dtf * 0.9) + absf(lv - lvp) * 0.7
        slosh = minf(slosh, 5.0)
        fxs["lv"] = lv;fxs["slosh"] = slosh
        var is_mash: = false
        if b == "sk": is_mash = sim.vol(S.sk.mash) > 50
        elif vv != null: is_mash = sim.contentKind(vv) == "mash"
        for i in N + 1:
            var px: = x - 20 + (w + 40) * i / float(N)
            var r: = clampf((px - (x + w * 0.5)) / (w * 0.5), -1.0, 1.0)
            var sl: float = slosh * sin(tt * 2.3 + r * 2.6) * r
            below.append(Vector2(px, lv - vort_px * (r * r - 0.5) + sl + amp * sin(px * 0.045 + tt * 3.1) + amp * 0.5 * sin(px * 0.11 - tt * 4.7)))
        below.append(Vector2(x + w + 20, y0 + 50));below.append(Vector2(x - 20, y0 + 50))
        var polys: = Geometry2D.intersect_polygons(outline, below)
        var mtex = null
        var uvoff: = Vector2.ZERO
        if is_mash:
            mtex = LiquidFx.tex_mash();texture_repeat = CanvasItem.TEXTURE_REPEAT_ENABLED
            uvoff = Vector2(tt * 9.0 * stirK, sin(tt * 0.7) * 3.0 * stirK)
            col = col.lightened(0.12)
        var gpu_ok: bool = b != "lv" and not is_mash and not m.no_gpu_liquid
        if gpu_ok:
            var turb_g: float = 0.0
            if b == "sk": turb_g = clampf(float(S.sk.turb) / 80.0, 0.0, 1.0)
            elif vv != null: turb_g = clampf(vv.trubSus / (maxf(1.0, sim.liqVol(vv)) * 0.006), 0.0, 1.0)
            _gpu_body().set_params(col, Rect2(x, it.y, w, it.h), lv, maxf(30.0, y0 - lv), turb_g, stirK, 1.0 if (vv != null and vv.boiling) else 0.0)
            _gpu_body().set_polys(polys)
        else:
            _gpu_clear()
            for p in polys:
                if b == "lv": _shade_liquid(p, it.y, it.h, col, true)
                else: _shade_liquid(p, x, w, col, false, lv, y0, mtex, uvoff)

        # zákal a částice v kapalině: suspenze se pohybuje s prouděním (míchání, var, vířivka), v klidu plevy stoupají a drť klesá
        var calm: float = 1.0 - stirK
        var hopped: = false
        var area: = Rect2(x + 8, lv + 4, w - 16, maxf(4.0, y0 - lv - 8))
        if vv != null and is_mash:
            LiquidFx.particles(self, fxs, "pp", area, outline, dtf, tt, {"kind": "mash", "stir": stirK, "boil": vv.boiling, "a_husk": 1.0, "seed": 11.0, "col": Color(0.70, 0.55, 0.32)})
        elif b == "sk" and is_mash:
            LiquidFx.particles(self, fxs, "pp", area, outline, dtf, tt, {"kind": "mash", "stir": stirK, "a_husk": 1.0, "seed": 11.0, "col": Color(0.70, 0.55, 0.32)})
        if b == "sk" and not is_mash:
            # první sladina před podražením (vracením do SK) je kalná – s ložem proplachuje jemné částice; po vorlaufu je čirá
            var tk: float = clampf(float(S.sk.turb) / 80.0, 0.0, 1.0)
            if tk > 0.03:
                for p in polys: draw_colored_polygon(p, Color(0.93, 0.85, 0.64, tk * 0.38))
                LiquidFx.particles(self, fxs, "pp", area, outline, dtf, tt, {"kind": "wort", "stir": maxf(stirK, 0.25 * tk), "a_fine": minf(1.0, tk * 1.2), "seed": 5.0, "col": col})
        if vv != null and not is_mash:
            var Vv: float = maxf(1.0, sim.liqVol(vv))
            var haze: float = clampf(vv.trubSus / (Vv * 0.006), 0.0, 1.0)
            if haze > 0.01:
                for p in polys: draw_colored_polygon(p, Color(0.93, 0.85, 0.64, haze * 0.30))
            hopped = (vv.alpha + vv.iso) > 0.5
            var hs_now: float = vv.alpha + vv.iso
            if fxs.has("hs") and hs_now - float(fxs["hs"]) > 0.5: fxs["hopt"] = tt
            fxs["hs"] = hs_now
            var wort: bool = sim.ext(vv) > 5.0
            var a_floc: float = clampf(vv.coag * 3.0, 0.0, 1.0) * (1.0 if (vv.boiling or vv.whirlNow) else 0.5) if vv.coag > 0.01 else 0.0
            var a_hop: float = clampf((vv.alpha + vv.iso) / 40.0, 0.25, 1.0) if (hopped and wort) else 0.0
            var a_fine: float = clampf(0.12 + haze * 0.8, 0.0, 1.0) if wort else 0.0
            if a_floc > 0.0 or a_hop > 0.0 or a_fine > 0.0:
                LiquidFx.particles(self, fxs, "pp", area, outline, dtf, tt, {"kind": "wort", "stir": stirK, "boil": vv.boiling, "whirl": vv.vort if vv.whirlNow else 0.0, "a_floc": a_floc, "a_hop": a_hop, "a_fine": a_fine, "seed": 5.0, "col": col})
            # kal ve vířivce: po usazení kupka uprostřed dna (roste s usazováním)
            if b in ["wp", "mp"] and not vv.whirlNow and not vv.boiling and vv.trubSet > 0.02:
                var pxL: float = (y0 - lv) / maxf(50.0, sim.vol(vv))
                var hpx: float = clampf(vv.trubSet * 6.0 * pxL * 5.0, 4.0, it.h * 0.17)
                LiquidFx.draw_trub_cone(self, x + w * 0.5, y0 - 2.0, clampf(hpx * 1.7, 12.0, w * 0.32), hpx, tt)
        # průhlednost a lom světla: odlesky v kapalině (čirá sladina/voda), u hustého rmutu jen slabé
        if b != "lv":
            LiquidFx.draw_caustics(self, area, outline, tt, 0.35 if is_mash else 1.0, col)

        if lv < y0 - 2:
            var surf: = PackedVector2Array()
            var back: = PackedVector2Array()
            for i in N + 1:
                var px2: = x + 6 + (w - 12) * i / float(N)
                var r2: = (px2 - (x + w * 0.5)) / (w * 0.5)
                var sl2: float = slosh * sin(tt * 2.3 + r2 * 2.6) * r2
                var yy: float = lv - vort_px * (r2 * r2 - 0.5) + sl2 + amp * sin(px2 * 0.045 + tt * 3.1) + amp * 0.5 * sin(px2 * 0.11 - tt * 4.7)
                surf.append(Vector2(px2, yy))
                # zadní okraj hladiny (pohled mírně shora): elipsa
                back.append(Vector2(px2, yy - 9.0 * sqrt(maxf(0.0, 1.0 - r2 * r2))))
            if Geometry2D.is_point_in_polygon(Vector2(x + w * 0.5, lv + 2), outline) or b == "lv":
                if b != "lv" and b != "sk" or (b == "sk" and not is_mash):
                    var lens: = PackedVector2Array(back)
                    for i in range(surf.size() - 1, -1, -1): lens.append(surf[i])
                    var lc: Color = col.lightened(0.30) if not is_mash else col.lightened(0.05)
                    lc.a = 0.55
                    var lp: = Geometry2D.intersect_polygons(outline, lens)
                    for q in lp: draw_colored_polygon(q, lc)
                var sheen: = PackedVector2Array()
                for q in surf: sheen.append(q + Vector2(0, 5))
                draw_polyline(sheen, Color(1, 1, 1, 0.10), 8.0, true)
                if vv != null:
                    var th: float = 0.0
                    if vv.boiling: th = 6.0 + minf(30.0, vv.foam * 0.05)
                    elif vv.foam > 3.0: th = 4.0 + minf(30.0, vv.foam * 0.05)
                    if th > 0.0: LiquidFx.draw_foam(self, x, x + w, surf, th, clampf((vv.alpha + vv.iso) / 60.0, 0.0, 1.0), tt)
                    if vv.boiling:
                        for i in 14:
                            var fxp: = x + 10 + fmod(i * 37.3, w - 20)
                            draw_circle(Vector2(fxp, lv - th * 0.5 + 2.0 * sin(tt * 2.0 + i)), 2.2 + fmod(i * 1.7, 2.5), Color(1, 0.98, 0.9, 0.55))
                    # chmel plovoucí na hladině a v pěně
                    if hopped and (vv.boiling or calm > 0.5):
                        for i in 22:
                            var hxp: float = x + 12 + fmod(i * 41.7, w - 24)
                            var hyp: float = lv - (th * (0.3 + 0.5 * LiquidFx.h(i, 2.0)) if vv.boiling else -2.0) + 1.0 * sin(tt * 1.4 + i)
                            var hl: float = 3.0 + 3.0 * LiquidFx.h(i, 3.0)
                            var ha: float = LiquidFx.h(i, 4.0) * 3.14
                            var hd: = Vector2(cos(ha), sin(ha)) * hl * 0.5
                            draw_line(Vector2(hxp, hyp) - hd, Vector2(hxp, hyp) + hd, Color(0.42, 0.47, 0.13, 0.9), 2.4, true)
                draw_polyline(surf, col.lightened(0.45), 2.5, true)
                draw_polyline(surf, Color(1, 1, 1, 0.55), 1.0, true)

        if vv != null and fxs.has("hopt"): _hop_fall(fxs, x, w, it.y + 26.0, lv, tt)
        if b == "sk": _grain_bed(it, fxs, tt, dtf, lv, outline, col)

        var v = S.get(b) if b in ["rvp", "mp", "wp"] else null
        if v != null and v.boiling:
            var t: = Time.get_ticks_msec() / 1000.0
            for i in 14:
                var bx: = x + 20 + fmod(i * 53.7, w - 40)
                var ph: = fmod(t * 1.7 + i * 0.37, 1.0)
                draw_circle(Vector2(bx, lv + 18 - ph * 22), 2.0 + 2.0 * ph, Color(1, 1, 1, 0.55 * (1.0 - ph)))
            var sb: = Vector2(x + w * 0.5, it.y - 20.0)
            for dv in m.items:
                if dv.type == "deco" and dv.get("k") == "vent" and dv.x > x - 40.0 and dv.x < x + w + 40.0 and dv.y < it.y + 40.0:
                    sb = Vector2(dv.x, dv.y - 70.0);break
            _steam(sb, 60.0, clampf(0.45 + 0.5 * minf(1.0, sim.vol(v) / 200.0), 0.3, 1.0), t, float(hash(it.id) % 50))
        fx[it.id] = fxs

    ## pelety chmele padají shora do mladiny: pád se zrychlením, na hladině kruhy od dopadu
    func _hop_fall(fxs: Dictionary, x: float, w: float, ytop: float, lv: float, tt: float) -> void :
        var e: float = tt - float(fxs["hopt"])
        if e > 4.5:
            fxs.erase("hopt");return
        var cx: float = x + w * 0.5
        for i in 34:
            var d0: float = LiquidFx.h(i, 7.0) * 1.6
            var te: float = e - d0
            if te < 0.0: continue
            var px: float = cx + (LiquidFx.h(i, 8.0) - 0.5) * w * 0.55 + 6.0 * sin(te * 3.0 + i)
            var py: float = ytop + 520.0 * te * te
            if py < lv - 2.0:
                var ang: float = te * (4.0 + 6.0 * LiquidFx.h(i, 9.0)) + i
                var dv: = Vector2(cos(ang), sin(ang)) * (2.6 + 1.6 * LiquidFx.h(i, 10.0))
                draw_line(Vector2(px, py) - dv, Vector2(px, py) + dv, Color(0.30, 0.38, 0.10, 0.95), 3.6, true)
                draw_line(Vector2(px, py) - dv * 0.6, Vector2(px, py) + dv * 0.6, Color(0.50, 0.58, 0.20, 0.9), 1.6, true)
            else:
                var tsp: float = te - sqrt(maxf(0.0, (lv - ytop) / 520.0))
                if tsp >= 0.0 and tsp < 0.7:
                    var k: float = tsp / 0.7
                    draw_arc(Vector2(px, lv + 1.0), 3.0 + 14.0 * k, 0.0, TAU, 20, Color(1, 0.97, 0.85, 0.55 * (1.0 - k)), 1.5, true)
                    if tsp < 0.25: draw_circle(Vector2(px, lv - 8.0 * tsp * (1.0 - tsp * 4.0) * 6.0), 1.6, Color(1, 0.97, 0.85, 0.7))

    ## měkká pára: stoupá z odtahu, rozšiřuje se a mizí (gradientní textura místo plochých koleček)
    func _steam(base: Vector2, wd: float, k: float, tt: float, sd: float) -> void :
        var tex: Texture2D = LiquidFx.tex_puff()
        for i in 12:
            var ph: float = fmod(tt * (0.16 + 0.06 * LiquidFx.h(i, sd)) + LiquidFx.h(i, sd + 1.0), 1.0)
            var sz: float = (50.0 + 170.0 * ph) * (0.7 + 0.6 * LiquidFx.h(i, sd + 2.0))
            var drift: float = 40.0 * ph * ph + 14.0 * sin(tt * 0.6 + i * 1.7) * ph
            var pos: = Vector2(base.x + (LiquidFx.h(i, sd + 3.0) - 0.5) * wd + drift, base.y - 230.0 * ph)
            var al: float = k * 0.46 * sin(clampf(ph, 0.0, 1.0) * PI) * (1.0 - 0.4 * ph)
            draw_texture_rect(tex, Rect2(pos - Vector2(sz, sz) * 0.5, Vector2(sz, sz)), false, Color(0.74, 0.79, 0.86, minf(0.6, al * 1.5)))

    ## mláto: filtrační lože z mokrých plev a drtě nad sítem. Vzhled podle fotek mláta ve scezovací kádi
    ## (tmavě hnědé, vlhké, zrnité, povrch zvlněný od kopačky). Výška lože plyne (kapalné chování), plevy ve vodě
    ## se při kypření a promíchání zvíří a pak se zase usadí.
    func _grain_bed(it, fxs: Dictionary, tt: float, dtf: float, lv: float, outline: PackedVector2Array, col: Color) -> void :
        var S = m.sim.S; var sim = m.sim
        var x: float = it.x; var w: float = it.w; var y0: float = it.y + it.h
        var sieve: = y0 - 22.0
        var tgt: float = sim.skBedH() / 0.943 * it.h
        var bed_px: float = float(fxs.get("bed", tgt))
        bed_px += (tgt - bed_px) * minf(1.0, dtf * 2.5)
        fxs["bed"] = bed_px
        if bed_px > 1:
            var top_y: float = sieve - bed_px
            var covered: = lv <= top_y
            var rake: = float(fxs.get("stir", 0.0))
            var tex: Texture2D = LiquidFx.tex_bed()
            texture_repeat = CanvasItem.TEXTURE_REPEAT_ENABLED
            # povrch lože: nerovný, od kopačky zvlněný (hřebeny se posouvají)
            var N2: = 40
            var prof: = PackedVector2Array()
            for i in N2 + 1:
                var px: float = x + 4.0 + (w - 8.0) * i / float(N2)
                var hh: float = 3.2 * (LiquidFx._vn(px / 17.0, 0.5, 64, 4.0) - 0.5) + 1.6 * (LiquidFx._vn(px / 6.0, 2.5, 128, 5.0) - 0.5)
                hh += rake * (3.4 * sin(px * 0.085 + tt * 1.3) + 1.6 * sin(px * 0.21 - tt * 2.1))
                prof.append(Vector2(px, top_y + hh))
            var poly: = PackedVector2Array(prof)
            poly.append(Vector2(x + w - 4.0, sieve));poly.append(Vector2(x + 4.0, sieve))
            var cols: = PackedColorArray()
            for p in poly:
                var rel: float = clampf((p.y - top_y) / maxf(1.0, bed_px), 0.0, 1.0)
                var side: float = absf((p.x - (x + w * 0.5)) / (w * 0.5))
                # válcové stínování + tmavnutí ke dnu (slisované vlhké mláto) + mokré/suché
                var k: float = (0.92 if covered else 1.12) - 0.30 * rel - 0.26 * side * side * side
                var c0: Color = Color("bc9c6c") if not covered else Color("9c7c4c")
                cols.append(Color(minf(1.0, c0.r * k * 1.18), minf(1.0, c0.g * k * 1.18), minf(1.0, c0.b * k * 1.18), 1.0))
            LiquidFx.draw_tex_poly(self, poly, cols, tex, Vector2(0.0, 0.0), 1.15)
            # mokrý lesk na hřebenech a kapky (bez kapaliny nad ložem je povrch vlhký a matný)
            draw_polyline(prof, Color(1.0, 0.92, 0.7, 0.30 if not covered else 0.18), 2.0, true)
            # pod hladinou je lože vidět přes sladinu
            if covered:
                var tint: Color = col
                tint.a = 0.22
                draw_colored_polygon(poly, tint)
            # kanálky a trhliny od vzduchu nebo nerovnoměrného odběru
            var ch: float = clampf(float(S.sk.get("c", 0.0)) * 3.0 + float(S.sk.get("air", 0.0)), 0.0, 1.0)
            if ch > 0.05:
                for i in int(3 + 8 * ch):
                    var cx: float = x + 14.0 + LiquidFx.h(i, 61.0) * (w - 28.0)
                    var cl: float = bed_px * (0.35 + 0.6 * LiquidFx.h(i, 62.0))
                    draw_polyline(PackedVector2Array([Vector2(cx, top_y + 2), Vector2(cx + 3 * sin(i), top_y + cl * 0.5), Vector2(cx - 2, top_y + cl)]), Color(0.1, 0.06, 0.02, 0.45), 1.2, true)
            if not covered: draw_line(Vector2(x + 4, top_y), Vector2(x + w - 4, top_y), Color("e0412f"), 2.0)
            # vířící plevy a drobná drť nad ložem při kypření / promíchání mláta s vodou
            var susp_t: = 0.0
            if covered and (S.sk.rake or S.skWater.on): susp_t = 1.0
            var susp: float = float(fxs.get("susp", 0.0))
            susp += (susp_t - susp) * minf(1.0, dtf * (0.9 if susp_t > susp else 0.18))
            fxs["susp"] = susp
            if susp > 0.03 and lv < top_y - 4.0:
                var zr: = Rect2(x + 8, lv + 3, w - 16, top_y - lv - 3)
                LiquidFx.particles(self, fxs, "sp", zr, outline, dtf, tt, {"kind": "mash", "stir": maxf(rake, 0.35 * susp), "a_husk": susp, "seed": 14.0, "col": Color(0.70, 0.55, 0.32)})
        var sx: = x + 4
        while sx < x + w - 4:
            draw_line(Vector2(sx, sieve), Vector2(minf(sx + 8, x + w - 4), sieve), Color("2b3138"), 3.0);sx += 12.0


    ## válcové stínování kapaliny (tmavé okraje, světlý pruh) – po pruzích s barvou ve vrcholech
    const LQ_T: = [0.0, 0.1, 0.24, 0.33, 0.5, 0.75, 0.9, 1.0]
    const LQ_F: = [-0.42, -0.2, 0.05, 0.16, 0.02, -0.1, -0.24, -0.45]
    func _lq_f(t: float) -> float:
        t = clampf(t, 0.0, 1.0)
        for i in range(1, LQ_T.size()):
            if t <= LQ_T[i]: return lerpf(LQ_F[i - 1], LQ_F[i], (t - LQ_T[i - 1]) / (LQ_T[i] - LQ_T[i - 1]))
        return LQ_F[LQ_F.size() - 1]
    func _lq_c(col: Color, f: float) -> Color:
        return col.lightened(f) if f > 0.0 else col.darkened( - f)
    func _shade_liquid(p: PackedVector2Array, a0: float, span: float, col: Color, horiz: bool, top: = 0.0, bot: = 0.0, tex: Texture2D = null, uvoff: = Vector2.ZERO) -> void :
        var dep: = maxf(1.0, bot - top)
        var lo: = Vector2(1e9, 1e9); var hi: = Vector2(-1e9, -1e9)
        for q in p:
            lo = lo.min(q);hi = hi.max(q)
        draw_colored_polygon(p, col.darkened(0.1))
        for i in LQ_T.size() - 1:
            var s0: float = a0 + span * LQ_T[i]; var s1: float = a0 + span * LQ_T[i + 1]
            var strip: PackedVector2Array
            if horiz: strip = PackedVector2Array([Vector2(lo.x - 2, s0), Vector2(hi.x + 2, s0), Vector2(hi.x + 2, s1), Vector2(lo.x - 2, s1)])
            else: strip = PackedVector2Array([Vector2(s0, lo.y - 2), Vector2(s1, lo.y - 2), Vector2(s1, hi.y + 2), Vector2(s0, hi.y + 2)])
            for q in Geometry2D.intersect_polygons(p, strip):
                if q.size() < 3 or absf(_area(q)) < 1.0: continue
                var cs: = PackedColorArray()
                for v in q:
                    var fv: = _lq_f(((v.y if horiz else v.x) - a0) / span)
                    if bot > top: fv -= 0.28 * clampf((v.y - top) / dep, 0.0, 1.0)
                    cs.append(_lq_c(col, fv))
                if tex != null: LiquidFx.draw_tex_poly(self, q, cs, tex, uvoff, 1.3)
                else: draw_polygon(q, cs)

    func _area(q: PackedVector2Array) -> float:
        var a: = 0.0
        for i in q.size(): a += q[i].cross(q[(i + 1) % q.size()])
        return a * 0.5

    func _gauge_levels(v) -> void :
        if m.hard and v.get("bind") == "rvp":
            for d in m.items:
                if d.type != "dip": continue
                var D: Dictionary = m.dip_geo(d)
                var calm2: bool = m.vessel_calm(v)
                var br2: = Rect2(D.x + 24.0, D.y + D.h * 0.5 - 13.0, 54.0, 26.0)
                Art.rrect_border(self, br2, Color("1d4f9c") if calm2 else Color("5a6169"), Color("0d1013"), 2, 6.0)
                draw_string(m.font_b, Vector2(br2.position.x + 8, br2.position.y + 18), "MĚŘ", HORIZONTAL_ALIGNMENT_LEFT, -1, 14, Color.WHITE if calm2 else Color("b4bcc3"))
                if d.has("_meas"):
                    var wy: float = D.y + D.h - D.h * (float(d["_meas"]) - 3.0) / 47.0
                    wy = clampf(wy, D.y, D.y + D.h)
                    draw_rect(Rect2(D.x - 7, wy, 14, D.y + D.h - wy), Color(0.08, 0.05, 0.02, 0.62))
                    draw_line(Vector2(D.x - 7, wy), Vector2(D.x + 7, wy), Color(0, 0, 0, 0.8), 1.5)
        for g in m.items:
            if g.type != "gauge": continue
            if m.gauge_vessel(g) != v: continue
            var G: Dictionary = m.gauge_geo(g)
            var ly: float = clampf(m.level_y(v), G.y1, G.y2)
            if m.hard:
                # skutečná hladina je vidět jen po změření (ztmavení); mezi měřeními zůstává stará hodnota
                var calm: bool = m.vessel_calm(v)
                var br: = Rect2(G.x + 14.0, (G.y1 + G.y2) * 0.5 - 13.0, 54.0, 26.0)
                Art.rrect_border(self, br, Color("1d4f9c") if calm else Color("5a6169"), Color("0d1013"), 2, 6.0)
                draw_string(m.font_b, Vector2(br.position.x + 8, br.position.y + 18), "MĚŘ", HORIZONTAL_ALIGNMENT_LEFT, -1, 14, Color.WHITE if calm else Color("b4bcc3"))
                if not g.has("_meas"): continue
                var my: float = g["_meas"]
                draw_rect(Rect2(G.x - 4, my, 8, G.y2 - my), Color(0.13, 0.12, 0.1, 0.9))
                draw_line(Vector2(G.x - 4, my), Vector2(G.x + 4, my), Color(0, 0, 0, 0.85), 1.5)
                continue
            if g.get("bot", true) == false:
                if not g.has("_frz"): g["_frz"] = ly
                ly = g["_frz"]
            else: g.erase("_frz")
            if ly >= G.y2 - 0.5: continue
            var col: Color = m.vessel_color(v)
            draw_rect(Rect2(G.x - 4, ly, 8, G.y2 - ly), Color(col, 0.92))
    func _vstatus(it) -> void :
        var b = it.get("bind")
        var d: Dictionary = m.vessel_card(b)
        if d.is_empty(): return
        var fs: = 14; var k: = 1.0
        var cx: float = it.x + it.w * 0.5
        var top: float = it.y + (40.0 if it.get("shape") == "horiz" else 54.0)
        var fb: Font = m.font_b
        var big: = 30; var sm: = 13; var tiny: = 11
        var ts: String = "prázdná" if d.empty else ("%s °C" % m.c1(d.T))
        var tcol: Color = Color("8a939b") if d.empty else m.temp_color(d.T)
        var bigs: = big if not d.empty else 18
        var tw: float = fb.get_string_size(ts, HORIZONTAL_ALIGNMENT_LEFT, -1, bigs).x
        var lw: = 0.0
        for l in d.lines: lw = maxf(lw, m.font.get_string_size(l, HORIZONTAL_ALIGNMENT_LEFT, -1, sm).x)
        var fw: = 0.0
        for fl in d.flags: fw += fb.get_string_size(fl[0], HORIZONTAL_ALIGNMENT_LEFT, -1, tiny).x + 18.0
        var pad: = 10.0
        var dial_r: = 0.0 if d.empty else 19.0
        var dial_w: = dial_r * 2.0 + 10.0 if dial_r > 0.0 else 0.0
        var W: float = maxf(dial_w + tw + (lw + 14.0 if lw > 0.0 else 0.0), fw) + pad * 2.0 + 4.0
        var Hc: float = maxf(bigs * 1.05 + 14.0, dial_r * 2.0 + 6.0) + pad + (tiny * 1.6 + 6.0 if d.flags.size() else 0.0)
        # karta se musí vejít do nádoby – jinak se zmenší
        var sc: = minf(m.label_size() / 14.0, it.w * 0.9 / W)
        draw_set_transform(Vector2(cx, top) * (1.0 - sc), 0.0, Vector2(sc, sc))
        var r: = Rect2(cx - W * 0.5, top, W, Hc)
        Art.rrect(self, Rect2(r.position + Vector2(3, 4), r.size), Color(0, 0, 0, 0.35), 10.0)
        Art.rrect_border(self, r, Color(0.045, 0.055, 0.068, 0.9), Color(tcol, 0.6), 2, 10.0)
        var bx: = r.position.x + pad
        if dial_r > 0.0:
            _dial_at(Vector2(bx + dial_r + 2.0, r.position.y + pad * 0.5 + maxf(bigs * 1.05 + 14.0, dial_r * 2.0 + 6.0) * 0.5), dial_r, d.T)
            bx += dial_w
        var by: = r.position.y + pad * 0.4 + bigs
        draw_string(fb, Vector2(bx, by), ts, HORIZONTAL_ALIGNMENT_LEFT, -1, bigs, tcol)
        var ly: = r.position.y + pad * 0.4 + sm * 1.15
        for l in d.lines:
            draw_string(m.font, Vector2(bx + tw + 14.0, ly), l, HORIZONTAL_ALIGNMENT_LEFT, -1, sm, Color("d5dbe0"));ly += sm * 1.3
        var fill: = clampf((it.y + it.h - m.level_y(it)) / maxf(1.0, it.h), 0.0, 1.0)
        var bar: = Rect2(bx, by + 6.0, r.end.x - pad - bx, 4.0)
        Art.rrect(self, bar, Color(1, 1, 1, 0.08), 2.0)
        if fill > 0.005 and not m.hard: Art.rrect(self, Rect2(bar.position, Vector2(bar.size.x * fill, bar.size.y)), Color(tcol, 0.85), 2.0)
        var fx: = r.position.x + pad; var fyy: = r.position.y + maxf(bigs * 1.05 + 14.0, dial_r * 2.0 + 6.0) + pad * 0.6
        for fl in d.flags:
            var fwi: float = fb.get_string_size(fl[0], HORIZONTAL_ALIGNMENT_LEFT, -1, tiny).x + 14.0
            Art.rrect(self, Rect2(fx, fyy, fwi, tiny * 1.6), Color(fl[1], 0.18), tiny * 0.8)
            draw_string(fb, Vector2(fx + 7.0, fyy + tiny * 1.18), fl[0], HORIZONTAL_ALIGNMENT_LEFT, -1, tiny, fl[1])
            fx += fwi + 4.0
        draw_set_transform(Vector2.ZERO, 0.0, Vector2.ONE)

    ## ručičkový teploměr (0–100 °C)
    func _dial_at(c: Vector2, R: float, T: float) -> void :
        draw_circle(c, R + 2.0, Color("0d1013"));draw_circle(c, R + 0.5, Color("aab3bb"));draw_circle(c, R - 1.5, Color("f4f6f8"))
        var a0: = deg_to_rad(135.0); var span: = deg_to_rad(270.0)
        draw_arc(c, R - 4.5, a0 + span * 0.78, a0 + span, 12, Color("ff5a4a"), 3.0, true)
        draw_arc(c, R - 4.5, a0 + span * 0.6, a0 + span * 0.78, 10, Color("f2c230"), 3.0, true)
        draw_arc(c, R - 4.5, a0, a0 + span * 0.6, 20, Color("4fa3ff"), 3.0, true)
        for i in 11:
            var a: = a0 + span * i / 10.0
            draw_line(c + Vector2(cos(a), sin(a)) * (R - 7.5), c + Vector2(cos(a), sin(a)) * (R - 3.0), Color("2b3138"), 1.0)
        var an: = a0 + span * clampf(T / 100.0, 0.0, 1.0)
        draw_line(c - Vector2(cos(an), sin(an)) * 3.0, c + Vector2(cos(an), sin(an)) * (R - 4.0), Color("c0261b"), 2.0, true)
        draw_circle(c, 2.4, Color("2b3138"))

    func _inlet(it) -> void :
        var on: bool = it.get("on", false)
        var cx: float = it.x + it.w * 0.5; var cy: float = it.y + it.h * 0.5
        var L2: float = maxf(it.w, it.h)
        var ang: = deg_to_rad(float(it.get("rot", 0)) + float(it.get("dir", 0)))
        draw_set_transform(Vector2(cx, cy), ang, Vector2.ONE)
        var col: = Color("2fbf5b") if on else Color("d93a2c")
        Art.cyl_h(self, Rect2( - L2 / 2.0, -7, L2, 14), Art.STAINLESS, 6)
        draw_rect(Rect2( - L2 / 2.0, -7, L2, 14), Color("23282e"), false, 1.5)
        draw_rect(Rect2( - L2 / 2.0 - 6, -11, 6, 22), Color("8a939b"))
        var wv: = PackedVector2Array()
        for i in 11: wv.append(Vector2( - L2 / 2.0 + 8 + i * 2.0, -1.0 + (-4.0 if i % 4 == 1 else (4.0 if i % 4 == 3 else 0.0)) * 0.6))
        draw_polyline(wv, Color("bfe6ff"), 2.0, true)
        var vx: float = L2 * 0.12
        draw_circle(Vector2(vx, 0), 13.0, Color("23282e"));draw_circle(Vector2(vx, 0), 11.5, Art.grad(Art.STAINLESS, 0.35))
        var dirv: = Vector2(1, 0).rotated(0.0 if on else - PI * 0.5)
        draw_line(Vector2(vx, 0) - dirv * 4.0, Vector2(vx, 0) + dirv * 30.0, Color("1b2026"), 10.0, true)
        draw_line(Vector2(vx, 0) - dirv * 4.0, Vector2(vx, 0) + dirv * 30.0, col, 7.0, true)
        draw_circle(Vector2(vx, 0), 4.0, Color("23282e"))
        draw_colored_polygon(PackedVector2Array([Vector2(L2 / 2.0 - 2, -9), Vector2(L2 / 2.0 + 12, 0), Vector2(L2 / 2.0 - 2, 9)]), Color("2e8bd6") if on else Color("6c757d"))
        draw_set_transform(Vector2.ZERO, 0, Vector2.ONE)
        var lab_y: float = maxf(it.y + it.h, cy + L2 / 2.0 + 6.0)
        Art.pill(self, m.font_b, "spoušť vody z řadu: " + ("PUŠTĚNO" if on else "ZAVŘENO (ťukni)"), Vector2(cx, lab_y + 24), 12, Color("7fc4ff") if on else Color("e6ebef"))


    func _small(it) -> void :
        var p: = Vector2(it.x, it.y); var r: float = deg_to_rad(float(it.get("rot", 0)))
        var dark: = Color("23282e"); var steel: = Art.grad(Art.STAINLESS, 0.4)
        if it.type == "inl":
            var k: String = it.get("k", "")
            draw_set_transform(p, r, Vector2.ONE)
            match k:
                "pruh":
                    _flange(-16);_flange(16)
                    Art.rrect_border(self, Rect2(-13, -10, 26, 20), Color(0.62, 0.83, 0.94, 0.45), dark, 1, 4.0)
                    draw_line(Vector2(-13, -10), Vector2(13, 10), dark, 1.0);draw_line(Vector2(-13, 10), Vector2(13, -10), dark, 1.0)
                "zpk":
                    _flange(-14);_flange(14);_body(24, 16)
                    draw_colored_polygon(PackedVector2Array([Vector2(-6, -6), Vector2(6, 0), Vector2(-6, 6)]), dark);draw_line(Vector2(7, -7), Vector2(7, 7), dark, 2.5)
                "ysit":
                    _flange(-16);_flange(16);_body(28, 14)
                    draw_colored_polygon(PackedVector2Array([Vector2(-4, 5), Vector2(8, 22), Vector2(14, 18), Vector2(2, 3)]), steel)
                "red":
                    draw_colored_polygon(PackedVector2Array([Vector2(-16, -9), Vector2(16, -5), Vector2(16, 5), Vector2(-16, 9)]), steel)
                    draw_polyline(PackedVector2Array([Vector2(-16, -9), Vector2(16, -5), Vector2(16, 5), Vector2(-16, 9), Vector2(-16, -9)]), dark, 1.5)
                "odk":
                    _flange(-15);_flange(15);_body(22, 22, Color("6c737b"));draw_string(m.font_b, Vector2(-4, 4), "K", HORIZONTAL_ALIGNMENT_LEFT, -1, 10, Color.WHITE)
                "redp":
                    _flange(-14);_flange(14)
                    draw_colored_polygon(PackedVector2Array([Vector2(-11, -9), Vector2(11, 9), Vector2(11, -9)]), steel)
                    draw_set_transform(p, 0.0, Vector2.ONE);draw_line(Vector2(0, -2), Vector2(0, -16), Color("8a939b"), 3.0)
                    draw_colored_polygon(PackedVector2Array([Vector2(-9, -16), Vector2(9, -16), Vector2(6, -26), Vector2(-6, -26)]), Color("d93a2c"))
                "aer":
                    _body(30, 12, Color("e8ecef"))
                    for bx in [-8, 0, 8]: draw_circle(Vector2(bx, -10 - (4 if bx == 0 else 0)), 2.2, Color("bfe6ff"))
                "tc":
                    Art.rrect_border(self, Rect2(-5, -12, 10, 24), Color("c9d1d9"), dark, 1, 2.0);draw_circle(Vector2(0, -15), 3.5, Color("8a939b"))
                "tep", "man", "pt", "hlad", "poj", "odv", "vzk", "vyp":
                    draw_set_transform(p, 0.0, Vector2.ONE)
                    _sensor(k)
                _:
                    _flange(-14);_flange(14);_body(24, 16)
            draw_set_transform(Vector2.ZERO, 0, Vector2.ONE)
            return
        if it.type == "flow":
            draw_set_transform(p, r, Vector2.ONE)
            _flange(-19);_flange(19)
            draw_set_transform(p, r + PI * 0.5, Vector2.ONE)
            Art.rrect_border(self, Rect2(-8, -20, 16, 40), Color("2a3b46"), Color("c9d1d9"), 1, 3.0)
            Art.rrect(self, Rect2(-5, -17, 10, 34), Color(0.62, 0.83, 0.94, 0.35), 2.0)
            for v in [-12, -6, 0, 6, 12]: draw_line(Vector2(-5, v), Vector2(-1, v), Color("c9d1d9"), 1.0)
            draw_colored_polygon(PackedVector2Array([Vector2(-4, 2), Vector2(4, 2), Vector2(0, 8)]), Color("e0452f"))
            draw_set_transform(Vector2.ZERO, 0, Vector2.ONE)
            Art.pill(self, m.font, it.get("label", ""), p + Vector2(0, -34), 11)
            return
        if it.type == "hopf":
            draw_set_transform(p, r, Vector2.ONE)
            _flange(-23);_flange(23)
            draw_set_transform(p, 0.0, Vector2.ONE)
            Art.rrect_border(self, Rect2(-20, -46, 40, 62), steel, dark, 2, 6.0)
            Art.rrect_border(self, Rect2(-23, -52, 46, 9), Color("8a939b"), dark, 1, 3.0)
            Art.rrect_border(self, Rect2(-6, -60, 12, 8), Color("6c737b"), dark, 1, 2.0)
            Art.rrect_border(self, Rect2(-13, -38, 26, 46), Color("2a3a22"), Color("6f8f3a"), 1, 3.0)
            for v in [-30, -22, -14, -6, 2]: draw_line(Vector2(-13, v), Vector2(13, v), Color("6f8f3a"), 0.8)
            for v in [-7, 0, 7]: draw_line(Vector2(v, -38), Vector2(v, 8), Color("6f8f3a"), 0.8)
            draw_circle(Vector2(0, -24), 5.0, Color("8fbf3f"));draw_circle(Vector2(0, -8), 4.5, Color("8fbf3f"))
            draw_rect(Rect2(-4, 16, 8, 8), Color("6c737b"))
            draw_set_transform(Vector2.ZERO, 0, Vector2.ONE)
            Art.pill(self, m.font, it.get("label", ""), p + Vector2(0, -68), 11)
            return
        draw_circle(p, 14.0, Color("0d1013"));draw_circle(p, 12.0, Color("c7cfd6"))
        draw_circle(p, 6.0, Color("6c757d"))

    func _flange(dx: float) -> void :
        draw_rect(Rect2(dx - 2.5, -10, 5, 20), Color("8a939b"));draw_rect(Rect2(dx - 2.5, -10, 5, 20), Color("23282e"), false, 1.0)

    func _body(w: float, h: float, col: = Color("c7cfd6")) -> void :
        Art.rrect_border(self, Rect2( - w / 2.0, - h / 2.0, w, h), col, Color("23282e"), 1, 3.0)


    func _sensor(k: String) -> void :
        var dark: = Color("23282e"); var grey: = Color("8a939b")
        match k:
            "tep", "man":
                draw_line(Vector2(0, 0), Vector2(0, -14), grey, 3.0);draw_circle(Vector2(0, -24), 11.0, dark);draw_circle(Vector2(0, -24), 9.5, Color.WHITE)
                draw_line(Vector2(0, -24), Vector2(5 if k == "tep" else -5, -30), Color("d93a2c") if k == "tep" else dark, 2.0)
                draw_string(m.font, Vector2(-7, -19), "°C" if k == "tep" else "bar", HORIZONTAL_ALIGNMENT_LEFT, -1, 7, dark)
            "pt":
                draw_line(Vector2(0, 4), Vector2(0, -18), grey, 3.0);Art.rrect_border(self, Rect2(-7, -30, 14, 12), Color("2e7bd6"), Color("0d2c52"), 1, 2.0)
                draw_string(m.font, Vector2(-5, -21), "PT", HORIZONTAL_ALIGNMENT_LEFT, -1, 7, Color.WHITE)
            "hlad":
                Art.rrect_border(self, Rect2(-5, -40, 10, 42), Color(0.62, 0.83, 0.94, 0.5), dark, 1, 3.0);draw_rect(Rect2(-3, -18, 6, 18), Color(0.18, 0.48, 0.84, 0.7))
            "poj":
                draw_line(Vector2(0, 0), Vector2(0, -10), grey, 3.0);Art.rrect_border(self, Rect2(-8, -18, 16, 8), Color("d93a2c"), Color("5a1410"), 1, 1.0);draw_line(Vector2(-4, -18), Vector2(-4, -28), Color("5a1410"), 2.0);draw_line(Vector2(4, -18), Vector2(4, -28), Color("5a1410"), 2.0)
            "odv":
                draw_line(Vector2(0, 0), Vector2(0, -12), grey, 3.0);draw_circle(Vector2(0, -17), 6.0, Art.grad(Art.STAINLESS, 0.4));draw_line(Vector2(0, -23), Vector2(0, -31), Color("8fd8ff"), 1.8)
            "vzk", "vyp":
                var down: bool = k == "vyp"
                draw_line(Vector2(0, 0), Vector2(0, 16 if down else -12), grey, 4.0);draw_circle(Vector2(0, 18 if down else -14), 5.0, Art.grad(Art.STAINLESS, 0.4))
                Art.rrect(self, Rect2(-2 if down else 0, 20 if down else -20, 4 if down else 12, 10 if down else 4), Color("d93a2c"), 2.0)

class Hud extends CanvasLayer:
    var m
    var info: Label
    var msg: Label
    var toast_box: PanelContainer
    var upd_btn: Button
    var warn_btn: Button
    var cab_btn: Button
    var hard_btn: Button
    var big_btn: Button
    var pan_btn: Button
    var play_btn: Button
    var sp_btns: = {}
    func ui_sync() -> void :
        if hard_btn != null: hard_btn.text = "HARD" if m.hard else "SOFT"
        if cab_btn != null: cab_btn.set_pressed_no_signal(m.dock_on)
        if pan_btn != null: pan_btn.set_pressed_no_signal(m.panel_on)
        if big_btn != null: big_btn.set_pressed_no_signal(m.dock_big and m.dock_on)
    var msg_t: = 0.0
    func _ready() -> void :
        var th: = Theme.new()
        th.default_font = m.font_b;th.default_font_size = 13
        var sb_n: = StyleBoxFlat.new();sb_n.bg_color = Color("262d35");sb_n.set_corner_radius_all(7);sb_n.content_margin_left = 8;sb_n.content_margin_right = 8;sb_n.content_margin_top = 6;sb_n.content_margin_bottom = 6;sb_n.border_color = Color("3a444e");sb_n.set_border_width_all(1)
        var sb_h: = sb_n.duplicate();sb_h.bg_color = Color("333c46");sb_h.border_color = Color("f2c230")
        var sb_p: = sb_n.duplicate();sb_p.bg_color = Color("f2c230")
        th.set_stylebox("normal", "Button", sb_n);th.set_stylebox("hover", "Button", sb_h);th.set_stylebox("pressed", "Button", sb_p);th.set_stylebox("focus", "Button", StyleBoxEmpty.new())
        th.set_color("font_color", "Button", Color("e6ebef"));th.set_color("font_hover_color", "Button", Color("ffffff"));th.set_color("font_pressed_color", "Button", Color("1b2026"))
        var top: = PanelContainer.new()
        var tsb: = StyleBoxFlat.new();tsb.bg_color = Color("0e1216");tsb.border_color = Color("f2c230");tsb.border_width_bottom = 2;tsb.content_margin_left = 14;tsb.content_margin_right = 14;tsb.content_margin_top = 8;tsb.content_margin_bottom = 8
        tsb.shadow_color = Color(0, 0, 0, 0.35);tsb.shadow_size = 6;tsb.shadow_offset = Vector2(0, 2)
        top.add_theme_stylebox_override("panel", tsb);top.theme = th
        top.anchor_right = 1.0;top.offset_right = 0
        add_child(top)
        var bar: = HBoxContainer.new();bar.add_theme_constant_override("separation", 5);top.add_child(bar)
        var logo: = Panel.new();logo.custom_minimum_size = Vector2(30, 30);logo.size_flags_vertical = Control.SIZE_SHRINK_CENTER
        var lsb: = StyleBoxFlat.new();lsb.bg_color = Color("f2c230");lsb.set_corner_radius_all(15);logo.add_theme_stylebox_override("panel", lsb);bar.add_child(logo)
        var ll: = Label.new();ll.text = "Z";ll.add_theme_color_override("font_color", Color("15191d"));ll.add_theme_font_size_override("font_size", 18)
        ll.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER;ll.vertical_alignment = VERTICAL_ALIGNMENT_CENTER;ll.set_anchors_preset(Control.PRESET_FULL_RECT);logo.add_child(ll)
        var tv: = VBoxContainer.new();tv.add_theme_constant_override("separation", -4);bar.add_child(tv)
        var title: = Label.new();title.text = "TRENAŽÉR VARNY";title.add_theme_color_override("font_color", Color("f2c230"));title.add_theme_font_size_override("font_size", 18);tv.add_child(title)
        var sub: = Label.new();sub.text = "Pivovar Zajíc · 40 hl";sub.add_theme_color_override("font_color", Color("7b858e"));sub.add_theme_font_size_override("font_size", 11);tv.add_child(sub)
        var sp: = Control.new();sp.custom_minimum_size = Vector2(18, 0);bar.add_child(sp)
        ## čas várky vždy po ruce (nezávisle na bočním panelu): Spustit / Pauza a rychlost
        play_btn = Button.new();play_btn.toggle_mode = true;play_btn.custom_minimum_size = Vector2(38, 0);play_btn.tooltip_text = "Spustit / pozastavit čas várky (mezerník)";bar.add_child(play_btn)
        play_btn.pressed.connect( func():
            m.running = not m.running
            if m.game != null: m.game.play("click")
            update_info())
        for spd in [1, 10, 60, 180]:
            var sb2: = Button.new();sb2.toggle_mode = true;sb2.text = "%d×" % spd;sb2.tooltip_text = "Rychlost času x%d (klávesa %d)" % [spd, [1, 10, 60, 180].find(spd) + 1];bar.add_child(sb2)
            sb2.pressed.connect( func():
                m.speed = float(spd);m.running = true
                update_info())
            sp_btns[spd] = sb2
        ## reset: začít celou várku úplně od nuly (smaže uloženou várku, klapky i nádrže se vrátí do výchozího stavu; plán a úpravy trubek zůstávají)
        var rst: = Button.new();rst.text = "↺ Reset";rst.tooltip_text = "Začít úplně od nuly: nová várka, všechno prázdné a klapky zavřené (tvoje úpravy plánu zůstanou)";bar.add_child(rst)
        var rdlg: = ConfirmationDialog.new();rdlg.title = "Reset várky";rdlg.dialog_text = "Opravdu začít úplně od nuly?\nUložená várka se smaže (úpravy trubek v editoru zůstanou).";rdlg.ok_button_text = "Ano, od nuly";rdlg.cancel_button_text = "Zrušit"
        rdlg.confirmed.connect( func(): m.new_game())
        add_child(rdlg)
        rst.pressed.connect( func(): rdlg.popup_centered())
        var sp2: = Control.new();sp2.custom_minimum_size = Vector2(6, 0);bar.add_child(sp2)
        for pair in [["Celé", ""], ["Varna", "rvp"], ["Scezovačka", "sk"], ["Whirlpool", "wp"], ["Bojler", "hlt"], ["Ledovka", "lv"], ["Spilka", "spilka"], ["Kateta", "kat"]]:
            var b: = Button.new();b.text = pair[0];bar.add_child(b)
            var bind: String = pair[1]
            b.pressed.connect( func(): _go(bind))
        var grow: = Control.new();grow.size_flags_horizontal = Control.SIZE_EXPAND_FILL;bar.add_child(grow)
        warn_btn = Button.new();warn_btn.visible = false;warn_btn.tooltip_text = "Klepni – ukážu rozvaděč"
        var wsb: = StyleBoxFlat.new();wsb.bg_color = Color("5a1712");wsb.border_color = Color("ff5a4a");wsb.set_border_width_all(1);wsb.set_corner_radius_all(7)
        wsb.content_margin_left = 10;wsb.content_margin_right = 10;wsb.content_margin_top = 6;wsb.content_margin_bottom = 6
        for st in ["normal", "hover", "pressed"]: warn_btn.add_theme_stylebox_override(st, wsb)
        warn_btn.add_theme_color_override("font_color", Color("ffd6d1"));warn_btn.add_theme_color_override("font_hover_color", Color.WHITE)
        warn_btn.pressed.connect( func():
            if not m.dock_on: m.toggle_dock()
            else: _go("rozv"))
        bar.add_child(warn_btn)
        cab_btn = Button.new();cab_btn.toggle_mode = true;cab_btn.text = "Rozvaděč";cab_btn.tooltip_text = "Rozvaděč RT.20 vpravo (R)";bar.add_child(cab_btn)
        cab_btn.visible = false
        big_btn = Button.new();big_btn.toggle_mode = true;big_btn.text = "⤢";big_btn.tooltip_text = "Zvětšit rozvaděč (Z). Kolečko posouvá, Ctrl+kolečko přibližuje, FM se točí táhnutím.";bar.add_child(big_btn)
        big_btn.pressed.connect( func(): m.toggle_dock_big())
        hard_btn = Button.new();hard_btn.text = "SOFT";hard_btn.tooltip_text = "Soft: červená/zelená klapka a viditelné toky. Hard: modré klapky, toky v trubkách nejsou vidět, hladina jen měřením (J)";bar.add_child(hard_btn)
        hard_btn.pressed.connect( func(): m.toggle_hard())
        pan_btn = Button.new();pan_btn.toggle_mode = true;pan_btn.text = "☰ Panel";pan_btn.tooltip_text = "Ovládací panel s nastavením, postupem a hlášeními";bar.add_child(pan_btn)
        pan_btn.pressed.connect( func(): m.toggle_panel())
        upd_btn = Button.new();upd_btn.text = "⟳ Aktualizovat";upd_btn.tooltip_text = "Stáhne nejnovější verzi trenažéru z internetu a restartuje ho";bar.add_child(upd_btn)
        upd_btn.pressed.connect( func(): m.updater.check(true))
        var edb: = Button.new();edb.text = "✎ Editor";edb.tooltip_text = "Úpravy trubek, klapek a dílů (E)";bar.add_child(edb)
        edb.pressed.connect( func(): m.open_editor())
        var b3: = Button.new();b3.text = "🎥 3D";b3.tooltip_text = "3D pohled na varnu (V)";bar.add_child(b3)
        b3.pressed.connect( func(): m.toggle_3d())
        var chip: = PanelContainer.new();chip.size_flags_vertical = Control.SIZE_SHRINK_CENTER
        var csb: = StyleBoxFlat.new();csb.bg_color = Color("1b2229");csb.set_corner_radius_all(14);csb.border_color = Color("2f3943");csb.set_border_width_all(1)
        csb.content_margin_left = 12;csb.content_margin_right = 12;csb.content_margin_top = 4;csb.content_margin_bottom = 4
        chip.add_theme_stylebox_override("panel", csb);bar.add_child(chip)
        info = Label.new();info.add_theme_color_override("font_color", Color("c9d1d8"));info.add_theme_font_size_override("font_size", 14);chip.add_child(info)
        toast_box = PanelContainer.new();toast_box.position = Vector2(16, 66);toast_box.visible = false
        var msb: = StyleBoxFlat.new();msb.bg_color = Color(0.07, 0.085, 0.1, 0.9);msb.set_corner_radius_all(10);msb.border_color = Color("f2c230");msb.border_width_left = 4
        msb.content_margin_left = 14;msb.content_margin_right = 14;msb.content_margin_top = 8;msb.content_margin_bottom = 8
        msb.shadow_color = Color(0, 0, 0, 0.3);msb.shadow_size = 8
        toast_box.add_theme_stylebox_override("panel", msb);add_child(toast_box)
        msg = Label.new();msg.add_theme_color_override("font_color", Color("f5f7f9"));msg.add_theme_font_override("font", m.font_b);msg.add_theme_font_size_override("font_size", 15);toast_box.add_child(msg)
    func _go(bind: String) -> void :
        if bind == "":
            m.go_to(m.bounds, true);return
        if bind == "rozv":
            if m.cabinet != null:
                var cs: float = m.cabinet.sc
                m.go_to(Rect2(m.cabinet.it.x - 30.0, m.cabinet.it.y, 770.0 * cs + 60.0, 1130.0 * cs).grow(40), true)
            return
        if bind == "kat":
            if m.outlets.size():
                var r: = Rect2(m.outlets[0].front, Vector2.ZERO)
                for o in m.outlets: r = r.expand(o.front)
                m.go_to(r.grow(220), true)
            return
        var v = m.vessel_by_bind(bind)
        if v != null:
            m.go_to(Rect2(v.x, v.y, v.get("w", 200), v.get("h", 200)).grow(260), true)
    func toast(t: String) -> void :
        msg.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
        msg.custom_minimum_size = Vector2(minf(m.font_b.get_string_size(t, HORIZONTAL_ALIGNMENT_LEFT, -1, 15).x + 6.0, 620.0), 0)
        msg.text = t;msg_t = 3.0 + t.length() / 40.0;toast_box.visible = t != "";toast_box.reset_size()
    func update_info() -> void :
        var t: float = m.sim.S.t
        var run: String = ("▶  x%d" % int(m.speed)) if m.running else "⏸  pauza"
        var fps: = ("   ·   %d fps" % Engine.get_frames_per_second()) if "--fps" in OS.get_cmdline_user_args() else ""
        info.text = "%s   ·   %d:%02d:%02d%s" % [run, int(t / 3600), int(t / 60) %60, int(t) %60, fps]
        info.tooltip_text = "zoom %d %%" % roundi(m.cam.zoom.x * 100.0)
        if play_btn != null:
            var pt: String = "⏸" if m.running else "▶"
            if play_btn.text != pt: play_btn.text = pt
            play_btn.set_pressed_no_signal(m.running)
            for k in sp_btns: sp_btns[k].set_pressed_no_signal(int(m.speed) == k)
        var dead: bool = not m.sim.cab_live()
        if warn_btn.visible != dead: warn_btn.visible = dead
        if dead:
            var wt: String = "⚠ Rozvaděč bez napětí – " + ("NOUZOVÉ STOP" if m.sim.S.cab.estop else ("hlavní vypínač" if not m.sim.S.cab.main else "klíč VYP"))
            if warn_btn.text != wt: warn_btn.text = wt
        if msg_t > 0.0:
            msg_t -= get_process_delta_time()
            if msg_t <= 0.0:
                msg.text = "";toast_box.visible = false
