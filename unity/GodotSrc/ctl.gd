extends CanvasLayer

const Art = preload("res://art.gd")


var m
var S
var refs: = {}
var log_box: RichTextLabel
var state_lbl: RichTextLabel
var check_lbl: RichTextLabel
var t_acc: = 0.0
var last_log: = -1.0
var sv: SubViewport
var trend
var hist: = []
const HIST_DT: = 30.0
const HIST_N: = 360
const SERIES: = [["Pánev", "rvp", "ff9a3c"], ["SK", "sk", "f2c230"], ["Bojler", "hlt", "ff5a4a"], ["Vířivka", "wp", "4fd1c5"], ["Spilka", "spilka", "5aa9ff"]]
var changed: = false
var pend: = 1

const YEL: = Color("f2c230")
const DIM: = Color("9aa3ab")

func _ready() -> void :
    S = m.sim.S
    var th: = Theme.new()
    th.default_font = m.font_b;th.default_font_size = 14
    var sb: = StyleBoxFlat.new();sb.bg_color = Color("262d35");sb.set_corner_radius_all(6);sb.border_color = Color("3a444e");sb.set_border_width_all(1)
    sb.content_margin_left = 9;sb.content_margin_right = 9;sb.content_margin_top = 4;sb.content_margin_bottom = 4
    var sh: = sb.duplicate();sh.bg_color = Color("333c46");sh.border_color = YEL
    var sp: = sb.duplicate();sp.bg_color = YEL
    th.set_stylebox("normal", "Button", sb);th.set_stylebox("hover", "Button", sh);th.set_stylebox("pressed", "Button", sp)
    th.set_stylebox("hover_pressed", "Button", sp);th.set_stylebox("focus", "Button", StyleBoxEmpty.new())
    th.set_color("font_color", "Button", Color("e6ebef"));th.set_color("font_hover_color", "Button", Color.WHITE)
    th.set_color("font_pressed_color", "Button", Color("1b2026"));th.set_color("font_hover_pressed_color", "Button", Color("1b2026"))
    th.set_color("font_color", "Label", Color("d5dbe0"))
    var scb: = StyleBoxFlat.new();scb.bg_color = Color(0, 0, 0, 0);scb.content_margin_left = 6;scb.content_margin_right = 0
    var grb: = StyleBoxFlat.new();grb.bg_color = Color("36404a");grb.set_corner_radius_all(4)
    var grh: = grb.duplicate();grh.bg_color = Color("4a5662")
    var grp: = grb.duplicate();grp.bg_color = YEL
    th.set_stylebox("scroll", "VScrollBar", scb);th.set_stylebox("grabber", "VScrollBar", grb);th.set_stylebox("grabber_highlight", "VScrollBar", grh);th.set_stylebox("grabber_pressed", "VScrollBar", grp)
    var panel: = PanelContainer.new();panel.theme = th
    var psb: = StyleBoxFlat.new();psb.bg_color = Color("0e1216");psb.border_color = Color("2a323a");psb.border_width_left = 1
    psb.content_margin_left = 10;psb.content_margin_right = 8;psb.content_margin_top = 10;psb.content_margin_bottom = 10
    panel.add_theme_stylebox_override("panel", psb)
    var svc: = SubViewportContainer.new();svc.stretch = true
    svc.anchor_left = 1.0;svc.anchor_right = 1.0;svc.anchor_bottom = 1.0
    svc.offset_left = - m.PANEL_W;svc.offset_top = 52;svc.offset_right = 0;svc.offset_bottom = 0
    add_child(svc)
    sv = SubViewport.new();sv.transparent_bg = false;sv.render_target_update_mode = SubViewport.UPDATE_ONCE
    svc.add_child(sv)
    panel.set_anchors_preset(Control.PRESET_FULL_RECT);sv.add_child(panel)
    svc.gui_input.connect( func(_e): dirty())
    svc.mouse_exited.connect(dirty)
    svc.resized.connect(dirty)
    var sc: = ScrollContainer.new();sc.horizontal_scroll_mode = ScrollContainer.SCROLL_MODE_DISABLED;panel.add_child(sc)
    var box: = VBoxContainer.new();box.size_flags_horizontal = Control.SIZE_EXPAND_FILL;box.add_theme_constant_override("separation", 5);sc.add_child(box)

    _head(box, "ČAS")
    var r: = _row(box)
    refs.play = _btn(r, "Spustit", func(): m.running = not m.running;refresh())
    for s in [1, 10, 60, 180]:
        var b: = _btn(r, "x%d" % s, func(): m.speed = float(s);refresh());b.toggle_mode = true;refs["sp%d" % s] = b
    refs.time = _lbl(box, "")
    r = _row(box)
    _btn(r, "Uložit", func(): m.save_game();m.hud.toast("Várka uložena (ukládá se i automaticky a při zavření)"))
    var nb: = _btn(r, "Nová várka", Callable())
    nb.pressed.disconnect(nb.pressed.get_connections()[0].callable)
    nb.pressed.connect(m.new_game)
    refs.hall = _btn(r, "", func(): m.set_hall( not m.hall_dark);refresh())

    _head(box, "TREND TEPLOT")
    trend = Trend.new();trend.ctl = self;trend.custom_minimum_size = Vector2(0, 160);box.add_child(trend)

    _head(box, "AUTOMATICKÁ VÁRKA")
    r = _row(box)
    refs.auto = _btn(r, "", _toggle_auto)
    refs.autost = _lbl(box, "")
    refs.autost.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART;refs.autost.custom_minimum_size = Vector2(280, 0)
    _head(box, "ROZVADĚČ")
    r = _row(box)
    refs.cmain = _btn(r, "", func(): S.cab.main = not S.cab.main;refresh())
    refs.ckey = _btn(r, "", func(): S.cab.key = not S.cab.key;refresh())
    refs.cest = _btn(r, "", func(): S.cab.estop = not S.cab.estop;refresh())
    r = _row(box)
    refs.cint = _btn(r, "", func():
        S.cab.interlock = not S.cab.get("interlock", true)
        m.sim.logm("Ochrany čerpadel (interlock) %s." % ("zapnuty – čerpadlo nepůjde pustit do zavřené / prázdné trasy" if S.cab.interlock else "VYPNUTY – chyba obsluhy bude mít skutečný následek (zavzdušnění, tlaková špička)"), "info" if S.cab.interlock else "warn");refresh())
    _head(box, "JODOVÁ ZKOUŠKA")
    r = _row(box)
    _btn(r, "Dílo v pánvi", func(): m.open_iodine("rvp"))
    _btn(r, "Rmut / sladina v SK", func(): m.open_iodine("sk"))
    _lbl(box, "Barva ukáže, jak moc se škrob zcukřil.")
    _head(box, "KOTEL A PÁRA")
    r = _row(box)
    refs.boiler = _btn(r, "", func(): S.boiler.on = not S.boiler.on;refresh())

    _head(box, "PÁNEV (RMUTOMLADINOVÁ)")
    r = _row(box);_lbl(r, "Pára do pláště:")
    for p in [0, 25, 50, 75, 100]:
        var b2: = _btn(r, "%d" % p, func(): S.rvp.heat = p / 100.0;refresh());b2.toggle_mode = true;refs["h%d" % p] = b2
    r = _row(box)
    refs.stir = _btn(r, "", func(): S.rvp.stir = not S.rvp.stir;refresh())
    r = _row(box);_lbl(r, "Otáčky míchadla:")
    _btn(r, "-", func(): S.rvp.hz = maxf(10.0, S.rvp.hz - 5.0);refresh())
    refs.hz = _lbl(r, "")
    _btn(r, "+", func(): S.rvp.hz = minf(50.0, S.rvp.hz + 5.0);refresh())
    r = _row(box);_lbl(r, "Voda z bojleru:")
    _btn(r, "−", func(): S.rvpWater.target = maxf(S.rvpWater.added, S.rvpWater.target - 100.0);refresh())
    refs.wtarget = _lbl(r, "")
    _btn(r, "+", func(): S.rvpWater.target += 100.0;refresh())
    r = _row(box)
    refs.water = _btn(r, "", _water)

    refs.mix = _btn(r, "", func(): S.mixAuto = not S.mixAuto;refresh())
    r = _row(box);_lbl(r, "Směšovač na:")
    _btn(r, "−", func(): S.rvpWater.T = maxf(S.cool.waterT, S.rvpWater.T - 1.0);S.skWater.T = S.rvpWater.T;refresh())
    refs.mixT = _lbl(r, "")
    _btn(r, "+", func(): S.rvpWater.T = minf(85.0, S.rvpWater.T + 1.0);S.skWater.T = S.rvpWater.T;refresh())
    r = _row(box)
    refs.malt = _btn(r, "", _malt)
    r = _row(box)
    refs.gate = _btn(r, "", func(): S.malt.gate = not S.malt.gate;m.redraw();refresh())
    r = _row(box)
    refs.hatch = _btn(r, "", func(): S.sk["hatch"] = not S.sk.get("hatch", false);m.redraw();refresh())

    r = _row(box)
    refs.hop = _btn(r, "", func(): m.sim.hopPlanAdd();refresh())

    _head(box, "SCEZOVACÍ KÁĎ")
    r = _row(box)
    refs.sparge = _btn(r, "", func(): S.skWater.on = not S.skWater.on;refresh())
    _btn(r, "−", func(): S.skWater.flow = maxf(10.0, S.skWater.flow - 10.0);refresh())
    refs.sflow = _lbl(r, "")
    _btn(r, "+", func(): S.skWater.flow = minf(200.0, S.skWater.flow + 10.0);refresh())
    r = _row(box);_lbl(r, "pH rmutu:")
    refs.ph = _lbl(r, "")
    r = _row(box)
    refs.rake = _btn(r, "", func(): S.sk.rake = not S.sk.rake;refresh())
    _btn(r, "Zvednout", func(): S.sk.hydT = 0.0;refresh())
    _btn(r, "Střed", func(): S.sk.hydT = 0.4;refresh())
    _btn(r, "U síta", func(): S.sk.hydT = 1.0;refresh())
    refs.lauter = _btn(box, "", func(): m.sim.start_lautering();refresh())
    refs.lauter_auto = _btn(box, "", func():
        if m.sim.lauter_auto.state().on: m.sim.lauter_auto.stop("ruční převzetí")
        else: m.sim.lauter_auto.start()
        refresh())
    _lbl(box, "Automat scezování přebírá kapalinné klapky varny.")
    _btn(box, "Zvětšit průhledítko C2", func(): m.open_sight_c2())
    r = _row(box)
    _btn(r, "Hluboké kypření", func(): m.sim.kypr();refresh())
    _btn(r, "Vyhrabat mláto", func(): m.sim.emptySK();refresh())

    _head(box, "ČERPADLA")
    for pr in [["p1", 25.0, 50.0, 400.0], ["p2", 10.0, 10.0, 200.0], ["p3", 10.0, 20.0, 150.0]]:
        var key: String = pr[0]; var stp: float = pr[1]; var lo: float = pr[2]; var hi: float = pr[3]
        r = _row(box)
        refs[key] = _btn(r, "", func(): m.toggle_pump(key);refresh())
        _btn(r, "−", func(): S[key].flow = maxf(lo, S[key].flow - stp);refresh())
        refs[key + "f"] = _lbl(r, "")
        _btn(r, "+", func(): S[key].flow = minf(hi, S[key].flow + stp);refresh())
        refs[key + "s"] = _lbl(box, "")
        refs[key + "s"].add_theme_color_override("font_color", DIM)
    r = _row(box)
    refs.plv = _btn(r, "", func(): m.toggle_pump("plv");refresh())

    _head(box, "CESTY (nápověda)")
    for rt in [["Voda z bojleru → pánev", "hlt", "rvp", ["phw"], -1], ["Pánev → SK (C1)", "rvp", "sk", ["p1"], -1], ["SK → pánev (C1, rmut)", "sk", "rvp", ["p1"], -1], 
            ["SK → SK (C2, vorlauf)", "sk", "sk", ["p2"], 0], ["SK → pánev (C2, sladina)", "sk", "rvp", ["p2"], -1], ["Zpod sít → C2 → pánev", "sk", "rvp", ["p2"], -1, ["valve_murah5ei", "valve_muraglp3"]], 
            ["Pánev → vířivka (C1)", "rvp", "wp", ["p1"], -1], ["Vířivka → spilka (C3)", "wp", "spilka", ["p3"], -1], ["Horká voda → SK (sprchy)", "hlt", "sk", ["phw"], 0], 
            ["Bojler → podsítí SK", "hlt", "sk", ["phw"], 1], ["Voda z řadu → ledovka", "vodovod", "lv", [], -1], ["Voda z řadu → bojler", "vodovod", "hlt", [], -1]]:
        r = _row(box)
        var lb: = _lbl(r, rt[0]);lb.custom_minimum_size = Vector2(150, 0);lb.clip_text = true
        var a: Array = rt
        _btn(r, "Ukázat", func(): m.show_route(a[1], a[2], a[3], false, a[4], a[5] if a.size() > 5 else []))
        _btn(r, "Otevřít", func(): m.show_route(a[1], a[2], a[3], true, a[4], a[5] if a.size() > 5 else []))
    _head(box, "LEDOVKA")
    r = _row(box)
    refs.chill = _btn(r, "", func(): m.sim.lvChillSet( not S.lv.chill);refresh())

    _head(box, "RECEPT")
    r = _row(box)
    refs.recipe = _btn(r, "", func():
        var n: int = (m.sim.recipe_idx + 1) % m.sim.RECIPES.size()
        m.sim.recipe_idx = n
        var f = FileAccess.open("user://recept.txt", FileAccess.WRITE);f.store_string(str(n));f.close()
        m.hud.toast("Recept „%s“ se použije po stisku Nová várka." % m.sim.RECIPES[n].name);refresh())
    refs.recipe_info = _lbl(box, "");refs.recipe_info.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART;refs.recipe_info.custom_minimum_size = Vector2(280, 0)

    _head(box, "VODA A pH RMUTU")
    r = _row(box)
    refs.water_t = _btn(r, "", func():
        var W = m.sim.proc.WATERS; var i: = 0
        for k in W.size(): if W[k].name == S.water.get("name", ""): i = k
        S.water = W[(i + 1) % W.size()].duplicate();refresh())
    r = _row(box);_lbl(r, "Kyselina mléčná 80 %:")
    _btn(r, "−", func(): S.lactic = maxf(0.0, S.lactic - 250.0);refresh())
    refs.lactic = _lbl(r, "")
    _btn(r, "+", func(): S.lactic = minf(5000.0, S.lactic + 250.0);refresh())
    refs.ph_info = _lbl(box, "");refs.ph_info.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART;refs.ph_info.custom_minimum_size = Vector2(280, 0)

    _head(box, "SPILKA – KVAŠENÍ")
    r = _row(box)
    refs.fer = _btn(r, "", func():
        if S.fer.on: S.fer.on = false;m.sim.logm("Kvašení ukončeno – pivo jde na ležení.")
        else: m.sim.proc.fer_start()
        refresh())
    _btn(r, "Přeskočit 12 h", func():
        if S.fer.on: m.sim.proc.fer_skip(12.0)
        refresh())
    r = _row(box);_lbl(r, "Teplota spilky:")
    _btn(r, "−", func(): S.fer.set = maxf(4.0, S.fer.set - 0.5);refresh())
    refs.fer_set = _lbl(r, "")
    _btn(r, "+", func(): S.fer.set = minf(16.0, S.fer.set + 0.5);refresh())
    refs.fer_info = _lbl(box, "");refs.fer_info.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART;refs.fer_info.custom_minimum_size = Vector2(280, 0)

    _head(box, "CIP SANITACE")
    r = _row(box)
    refs.cip_t = _btn(r, "", func():
        var T = m.sim.proc.CIP_TARGETS; var i: = 0
        for k in T.size(): if T[k][0] == S.cip.target: i = k
        S.cip.target = T[(i + 1) % T.size()][0];refresh())
    refs.cip_m = _btn(r, "", func():
        var M = m.sim.proc.CIP_MEDIA; var i: = 0
        for k in M.size(): if M[k][0] == S.cip.medium: i = k
        S.cip.medium = M[(i + 1) % M.size()][0];S.cip.min = 0.0
        S.cip.set = {"voda": 50.0, "louh": 80.0, "kyselina": 50.0}[S.cip.medium];S.cip.conc = {"voda": 0.0, "louh": 1.5, "kyselina": 1.0}[S.cip.medium];refresh())
    r = _row(box)
    refs.cip_run = _btn(r, "", func():
        S.cip.run = not S.cip.run;S.cip.min = 0.0
        m.sim.logm("CIP %s: %s %s." % [m.sim.proc.cip_name(S.cip.target), S.cip.medium, "start" if S.cip.run else "stop"]);refresh())
    _btn(r, "−", func(): S.cip.set = maxf(20.0, S.cip.set - 5.0);refresh())
    refs.cip_set = _lbl(r, "")
    _btn(r, "+", func(): S.cip.set = minf(95.0, S.cip.set + 5.0);refresh())
    r = _row(box);_lbl(r, "Koncentrace:")
    _btn(r, "−", func(): S.cip.conc = maxf(0.0, snappedf(S.cip.get("conc", 1.5) - 0.25, 0.25));refresh())
    refs.cip_conc = _lbl(r, "")
    _btn(r, "+", func(): S.cip.conc = minf(5.0, snappedf(S.cip.get("conc", 1.5) + 0.25, 0.25));refresh())
    refs.cip_info = _lbl(box, "");refs.cip_info.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART;refs.cip_info.custom_minimum_size = Vector2(280, 0)

    _head(box, "NÁCVIK PORUCH A VÝSTRAHY")
    r = _row(box)
    refs.faults = _btn(r, "", func():
        S.faults.on = not S.faults.on;S.faults.next = 0.0;S.faults.nextMinor = 0.0
        m.sim.logm("Nácvik poruch %s – náhodné poruchy a provozní výstrahy se objeví během várky." % ("zapnut" if S.faults.on else "vypnut"));refresh())
    r = _row(box)
    _btn(r, "Odvzdušnit čerpadlo", func(): m.sim.proc.fault_fix("air");refresh())
    _btn(r, "Reset kotle", func(): m.sim.proc.fault_fix("steam");refresh())
    r = _row(box)
    _btn(r, "Vyčistit chladič", func(): m.sim.proc.fault_fix("plate");refresh())
    _btn(r, "Uvolnit klapku", func(): m.sim.proc.fault_fix("valve");refresh())
    r = _row(box)
    _btn(r, "Kalibrace teploměru pánve", func(): m.sim.proc.fault_fix("sensor");refresh())
    refs.faults_info = _lbl(box, "")

    _head(box, "HODNOCENÍ VÁRKY")
    refs.score = RichTextLabel.new();refs.score.bbcode_enabled = true;refs.score.fit_content = true;refs.score.scroll_active = false
    refs.score.add_theme_font_override("normal_font", m.font);refs.score.add_theme_font_override("bold_font", m.font_b);refs.score.add_theme_font_size_override("normal_font_size", 13);refs.score.add_theme_font_size_override("bold_font_size", 13)
    box.add_child(refs.score)

    _head(box, "POSTUP VÁRKY (dvourmutový)")
    r = _row(box)
    refs.next_step = _btn(r, "Potvrdit krok a pokračovat", func(): m.sim.training.confirm();refresh())
    _btn(r, "Proč?", func(): m.sim.training.state().why = not m.sim.training.state().get("why", false);refresh())
    refs.step_pause = _btn(r, "", func(): m.sim.training.state().pause = not m.sim.training.state().pause;refresh())
    check_lbl = RichTextLabel.new();check_lbl.bbcode_enabled = true;check_lbl.fit_content = true;check_lbl.scroll_active = false
    check_lbl.add_theme_font_override("normal_font", m.font);check_lbl.add_theme_font_size_override("normal_font_size", 13)
    box.add_child(check_lbl)
    _head(box, "STAV")
    state_lbl = RichTextLabel.new();state_lbl.bbcode_enabled = true;state_lbl.fit_content = true;state_lbl.scroll_active = false
    state_lbl.add_theme_font_override("normal_font", m.font);state_lbl.add_theme_font_override("bold_font", m.font_b);state_lbl.add_theme_font_size_override("normal_font_size", 13);state_lbl.add_theme_font_size_override("bold_font_size", 13)
    box.add_child(state_lbl)
    _head(box, "HLÁŠENÍ")
    log_box = RichTextLabel.new();log_box.bbcode_enabled = true;log_box.fit_content = true;log_box.scroll_active = false
    log_box.add_theme_font_override("normal_font", m.font);log_box.add_theme_font_size_override("normal_font_size", 13)
    box.add_child(log_box)
    _cardify(box)
    refresh()
    for a in OS.get_cmdline_user_args():
        if a.begins_with("--scroll="):
            await get_tree().process_frame;await get_tree().process_frame
            sc.scroll_vertical = int(a.substr(9));dirty()

func dirty() -> void :
    pend = 2

func _head(box: Control, t: String) -> void :
    var l: = Label.new();l.text = t;l.add_theme_color_override("font_color", YEL);l.add_theme_font_size_override("font_size", 13)
    l.set_meta("head", true)
    box.add_child(l)

## sekce panelu → sbalitelné karty (stav se pamatuje v user://panel.json)
const COLLAPSED0: = ["JODOVÁ ZKOUŠKA", "CESTY (nápověda)", "LEDOVKA", "RECEPT", "VODA A pH RMUTU", "SPILKA – KVAŠENÍ", "CIP SANITACE", "NÁCVIK PORUCH A VÝSTRAHY", "HODNOCENÍ VÁRKY"]
const PANEL_CFG: = "user://panel.json"
var collapsed: = {}
var sections: = {}

func _load_collapsed() -> void :
    collapsed.clear()
    var f: = FileAccess.open(PANEL_CFG, FileAccess.READ)
    var d = JSON.parse_string(f.get_as_text()) if f != null else null
    if typeof(d) == TYPE_ARRAY:
        for k in d: collapsed[str(k)] = true
    else:
        for k in COLLAPSED0: collapsed[k] = true

func _save_collapsed() -> void :
    var f: = FileAccess.open(PANEL_CFG, FileAccess.WRITE)
    if f != null: f.store_string(JSON.stringify(collapsed.keys()))

func _cardify(box: VBoxContainer) -> void :
    _load_collapsed()
    var csb: = StyleBoxFlat.new();csb.bg_color = Color("161c22");csb.set_corner_radius_all(8);csb.border_color = Color("262f38");csb.set_border_width_all(1)
    csb.content_margin_left = 10;csb.content_margin_right = 10;csb.content_margin_top = 4;csb.content_margin_bottom = 8
    var hsb: = StyleBoxFlat.new();hsb.bg_color = Color(0, 0, 0, 0);hsb.content_margin_left = 2;hsb.content_margin_right = 2;hsb.content_margin_top = 4;hsb.content_margin_bottom = 4
    var hsh: = hsb.duplicate();hsh.bg_color = Color(1, 1, 1, 0.04);hsh.set_corner_radius_all(5)
    var kids: = box.get_children()
    var content: VBoxContainer = null
    for c in kids:
        if c.has_meta("head"):
            var title: String = c.text
            box.remove_child(c);c.queue_free()
            var card: = PanelContainer.new();card.add_theme_stylebox_override("panel", csb);box.add_child(card)
            var v: = VBoxContainer.new();v.add_theme_constant_override("separation", 5);card.add_child(v)
            var hb: = Button.new();hb.alignment = HORIZONTAL_ALIGNMENT_LEFT;hb.focus_mode = Control.FOCUS_NONE
            for st in ["normal", "pressed", "hover_pressed", "disabled"]: hb.add_theme_stylebox_override(st, hsb)
            hb.add_theme_stylebox_override("hover", hsh)
            for fc in ["font_color", "font_pressed_color", "font_hover_pressed_color"]: hb.add_theme_color_override(fc, YEL)
            hb.add_theme_color_override("font_hover_color", Color("ffd95a"));hb.add_theme_font_size_override("font_size", 13)
            v.add_child(hb)
            content = VBoxContainer.new();content.add_theme_constant_override("separation", 5);v.add_child(content)
            sections[title] = [hb, content]
            hb.pressed.connect(_toggle_sec.bind(title))
            _apply_sec(title)
        elif content != null:
            box.remove_child(c);content.add_child(c)
    box.add_theme_constant_override("separation", 8)
    _fit_width(box, 266.0)
    if "--paneldebug" in OS.get_cmdline_user_args():
        await get_tree().process_frame
        print("PANEL box min %s, sv %s" % [box.get_combined_minimum_size(), sv.size])
        _dbg_wide(box, 0)

func _dbg_wide(n: Node, d: int) -> void :
    for c in n.get_children():
        if c is Control and c.get_combined_minimum_size().x > 250.0:
            print("  ".repeat(d), c.get_class(), " ", c.get_combined_minimum_size(), " ", c.get("text") if c.get("text") != null else "")
            _dbg_wide(c, d + 1)

## nic v panelu nesmí být širší než sloupec – dlouhé popisky se zalomí, dlouhá tlačítka zkrátí
func _fit_width(n: Node, maxw: float) -> void :
    for c in n.get_children():
        if c is Label:
            if not (n is HFlowContainer):
                c.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART;c.custom_minimum_size.x = minf(c.custom_minimum_size.x, maxw)
        elif c is Button:
            if c.get_combined_minimum_size().x > maxw:
                c.clip_text = true;c.text_overrun_behavior = TextServer.OVERRUN_TRIM_ELLIPSIS;c.custom_minimum_size.x = maxw
        if c.get_child_count() > 0 and not (c is Button): _fit_width(c, maxw)

func _apply_sec(title: String) -> void :
    var sct: Array = sections[title]
    var shut: bool = collapsed.has(title)
    sct[0].text = ("▸  " if shut else "▾  ") + title
    sct[1].visible = not shut

func _toggle_sec(title: String) -> void :
    if collapsed.has(title): collapsed.erase(title)
    else: collapsed[title] = true
    _apply_sec(title);_save_collapsed();dirty()

func _row(box: Control) -> HFlowContainer:
    var r: = HFlowContainer.new();r.add_theme_constant_override("h_separation", 4);r.add_theme_constant_override("v_separation", 4);box.add_child(r);return r

func _btn(r: Control, t: String, f: Callable) -> Button:
    var b: = Button.new();b.text = t;b.pressed.connect(_on_btn.bind(f))
    r.add_child(b);return b

func _on_btn(f: Callable) -> void :
    f.call()
    m.redraw()

func _toggle_auto() -> void :
    if m.auto.on:
        m.auto.stop("zastavena obsluhou")
    else:
        m.auto.start()
    refresh()

func _water() -> void :
    if S.rvpWater.on: S.rvpWater.on = false
    else:
        if S.rvpWater.added >= S.rvpWater.target - 1: S.rvpWater.target = S.rvpWater.added + S.cfg.water
        S.rvpWater.on = true
    if S.rvpWater.on:
        if not m.running:
            m.running = true   # voda se pouští, aby tekla – čas se rozběhne sám
            m._say("Čas spuštěn, ať voda teče.")
        var vs = m.sim.route_valves("hlt", "rvp", ["phw"])
        if vs == null: m._say("Z bojleru do pánve ve schématu cesta není (chybí trubka nebo čerpadlo).")
        else:
            var zav: int = 0
            for id in vs:
                if not m.sim.valve_open(id): zav += 1
            if zav > 0: m._say("Voda z bojleru zatím nepoteče – na cestě je %d zavřených klapek. V panelu Cesty → „Voda z bojleru → pánev“: Ukázat (vidíš které) nebo Otevřít (otevře je)." % zav)
    refresh()

func _malt() -> void :
    if not S.malt.on and S.malt.added >= S.malt.target - 0.01: return
    S.malt.on = not S.malt.on;refresh()

func _lbl(r: Control, t: String) -> Label:
    var l: = Label.new();l.text = t;r.add_child(l);return l

func _t(c, s: String) -> void :
    if c.text != s:
        c.text = s;changed = true

func _p(b: Button, on: bool) -> void :
    if b.button_pressed != on:
        b.button_pressed = on;changed = true

func onoff(b: bool) -> String: return "ZAP" if b else "vyp"
func c1(x: float) -> String: return ("%.1f" % x).replace(".", ",")

func _process(delta: float) -> void :
    if pend > 0:
        pend -= 1;sv.render_target_update_mode = SubViewport.UPDATE_ONCE
    t_acc += delta
    if t_acc > 0.6:
        t_acc = 0.0;refresh()

func _sample() -> void :
    var t: float = S.t
    if hist.size() and t < hist[hist.size() - 1][0] - 1.0: hist.clear()
    if hist.size() and t - hist[hist.size() - 1][0] < HIST_DT: return
    var row: = [t]
    for se in SERIES:
        var d: Dictionary = m.vessel_card(se[1])
        row.append(null if d.is_empty() or d.empty or d.T == null else float(d.T))
    hist.append(row)
    if hist.size() > HIST_N: hist.pop_front()
    if trend != null: trend.queue_redraw()
    changed = true

class Trend extends Control:
    var ctl
    func _draw() -> void :
        var r: = Rect2(Vector2.ZERO, size)
        Art.rrect(self, r, Color("0b0f13"), 6.0)
        var plot: = Rect2(28, 8, size.x - 36, size.y - 52)
        var f: Font = ctl.m.font
        for v in [0, 25, 50, 75, 100]:
            var y: float = plot.end.y - plot.size.y * v / 100.0
            draw_line(Vector2(plot.position.x, y), Vector2(plot.end.x, y), Color(1, 1, 1, 0.06 if v % 50 else 0.1), 1.0)
            draw_string(f, Vector2(2, y + 4), str(v), HORIZONTAL_ALIGNMENT_LEFT, -1, 10, Color("6b757f"))
        var H: Array = ctl.hist
        if H.size() >= 2:
            var t0: float = H[0][0]; var t1: float = maxf(H[H.size() - 1][0], t0 + 600.0)
            for si in SERIES.size():
                var col: = Color(SERIES[si][2])
                var seg: = PackedVector2Array()
                for row in H:
                    var v = row[si + 1]
                    if v == null:
                        if seg.size() >= 2: draw_polyline(seg, col, 2.0, true)
                        seg = PackedVector2Array();continue
                    seg.append(Vector2(plot.position.x + plot.size.x * (row[0] - t0) / (t1 - t0), plot.end.y - plot.size.y * clampf(v / 100.0, 0.0, 1.05)))
                if seg.size() >= 2: draw_polyline(seg, col, 2.0, true)
                if seg.size() >= 1: draw_circle(seg[seg.size() - 1], 3.0, col)
            var mins: = (t1 - t0) / 60.0
            draw_string(f, Vector2(plot.position.x, plot.end.y + 13), "před %d min" % roundi(mins), HORIZONTAL_ALIGNMENT_LEFT, -1, 10, Color("6b757f"))
            draw_string(f, Vector2(plot.end.x - 24, plot.end.y + 13), "teď", HORIZONTAL_ALIGNMENT_LEFT, -1, 10, Color("6b757f"))
        else:
            draw_string(f, Vector2(plot.position.x + 10, plot.position.y + plot.size.y * 0.5), "Graf se kreslí, jak běží čas várky…", HORIZONTAL_ALIGNMENT_LEFT, -1, 12, Color("6b757f"))
        # legenda s aktuální hodnotou
        var lx: = 6.0; var ly: = plot.end.y + 28.0
        var last: Array = H[H.size() - 1] if H.size() else []
        for si in SERIES.size():
            var v2 = last[si + 1] if last.size() else null
            var txt: String = SERIES[si][0] + (" %d°" % roundi(v2) if v2 != null else "")
            var col2: = Color(SERIES[si][2])
            var iw: = f.get_string_size(txt, HORIZONTAL_ALIGNMENT_LEFT, -1, 10).x + 20.0
            if lx + iw > size.x - 2.0:
                lx = 6.0;ly += 13.0
            draw_rect(Rect2(lx, ly - 7, 8, 3), col2)
            draw_string(f, Vector2(lx + 11, ly), txt, HORIZONTAL_ALIGNMENT_LEFT, -1, 10, Color("c9d1d8") if v2 != null else Color("5b646c"))
            lx += f.get_string_size(txt, HORIZONTAL_ALIGNMENT_LEFT, -1, 10).x + 20.0

func refresh() -> void :
    _sample()
    _t(refs.play, "PAUZA" if m.running else "SPUSTIT (čas stojí)")
    _t(refs.hall, "☀ Světlá hala" if m.hall_dark else "☾ Tmavá hala")
    for s in [1, 10, 60, 180]: _p(refs["sp%d" % s], int(m.speed) == s)
    var t: float = S.t
    _t(refs.time, "Čas várky %d:%02d:%02d   (rychlost x%d)" % [int(t / 3600), int(t / 60) %60, int(t) %60, int(m.speed)])
    _t(refs.auto, "STOP automatu" if m.auto.on else ("Spustit automatickou várku" if not m.auto.done else "Hotovo – spustit znovu"))
    _t(refs.autost, m.auto.status if m.auto.status != "" else "Automat sám otvírá cesty, pouští čerpadla, topí a přidává chmel. Když se něco zasekne, zastaví se a napíše proč.")
    _t(refs.cmain, "Hlavní: %s" % onoff(S.cab.main))
    _t(refs.ckey, "Klíč: %s" % ("START" if S.cab.key else "vyp"))
    _t(refs.cest, "Nouzové stop: %s" % ("STISKNUTO" if S.cab.estop else "ok"))
    _t(refs.cint, "Ochrany čerpadel (interlock): %s" % ("ZAP" if S.cab.get("interlock", true) else "VYP – zkouška"))
    _t(refs.boiler, "Pára z kotle: %s   (zatížení %d %%)" % [onoff(S.boiler.on), roundi(S.boiler.load * 100.0)])
    for p in [0, 25, 50, 75, 100]: _p(refs["h%d" % p], roundi(S.rvp.heat * 100.0) == p)
    _t(refs.hz, "%d Hz" % roundi(S.rvp.hz))
    _t(refs.stir, "Míchadlo: %s" % onoff(S.rvp.stir))
    _t(refs.wtarget, "%d L" % roundi(S.rvpWater.target))
    _t(refs.water, ("Stop  %d / %d L" % [roundi(S.rvpWater.added), roundi(S.rvpWater.target)]) if S.rvpWater.on else "Napouštět pánev")
    _t(refs.mix, "Směšovač: %s" % ("ZAP" if S.mixAuto else "vyp"))
    _t(refs.mixT, "%d °C" % roundi(S.rvpWater.T))
    if S.malt.added >= S.malt.target - 0.01: _t(refs.malt, "Slad vystřen (%d kg)" % roundi(S.malt.added))
    else: _t(refs.malt, ("Stop šrotu  %d / %d kg" % [roundi(S.malt.added), roundi(S.malt.target)]) if S.malt.on else "Šrotovat a vystírat (%d kg)" % roundi(S.malt.target - S.malt.added))
    _t(refs.gate, "Šoupě násypky: %s" % ("OTEVŘENO" if S.malt.gate else "zavřeno"))
    _t(refs.hatch, "Výhoz mláta: %s" % ("OTEVŘEN" if S.sk.get("hatch", false) else "zavřen"))
    var hp = m.sim.hopPlanNext()
    if hp != null:
        var nm: = ""
        for h in m.sim.HOPS:
            if h.id == hp.hop: nm = h.name
        _t(refs.hop, "Chmel: %s kg %s (%d. min varu)" % [c1(hp.kg), nm, roundi(hp.min)])
    else: _t(refs.hop, "Chmel podle receptu přidán (%d dávky)" % S.hops.size())
    _t(refs.sparge, ("Stop vyslazování" if S.skWater.on else "Vyslazovací voda"))
    _t(refs.ph, "%s" % c1(S.get("mashPH", S.pH)))
    var pr = m.sim.proc
    if pr != null and S.has("water"):
        var R = m.sim.RECIPES[m.sim.recipe_idx]
        _t(refs.recipe, "Recept: %s ▸" % R.name)
        var hs: = []
        for h in R.hops: hs.append("%s kg %s v %d." % [c1(h.kg), h.hop.to_upper(), roundi(h.min)])
        _t(refs.recipe_info, "%s °P · %d IBU · var %d min · sypání %s · chmel: %s%s" % [c1(R.P), roundi(R.IBU), roundi(R.boil), ", ".join(PackedStringArray(R.grist.keys().map(func(k): return "%s %d kg" % [k, roundi(R.grist[k])]))), "; ".join(PackedStringArray(hs)), "" if S.cfg.get("recipe", "") == R.name else "  (po stisku Nová várka)"])
        _t(refs.water_t, "Voda: %s ▸" % S.water.get("name", "vlastní"))
        _t(refs.lactic, "%d mL" % roundi(S.lactic))
        _t(refs.ph_info, "Alkalita %d, Ca %d, Mg %d mg/L → zbytková alkalita %d mg/L CaCO₃. pH rmutu %s (optimum 5,4–5,6). Aktivita α %d %%, β %d %%." % [roundi(S.water.alk), roundi(S.water.ca), roundi(S.water.mg), roundi(pr.residual_alk(S.water)), c1(S.get("mashPH", pr.mash_ph())), roundi(S.get("phF", Vector2.ONE).x * 100.0), roundi(S.get("phF", Vector2.ONE).y * 100.0)])
        var F = S.fer
        _t(refs.fer, "Ukončit kvašení" if F.on else "Zakvasit")
        _t(refs.fer_set, "%s °C" % c1(F.set))
        _t(refs.fer_info, ("Den %s · %s °C · zdánlivý extrakt %s °P · alkohol %s %% obj. · diacetyl %d ppb · kvasnice %s g/L" % [c1(F.t / 86400.0), c1(F.T), c1(pr.app_extract()), c1(F.E / 10.0 / 0.79), roundi(F.D), c1(F.X)]) if F.on or F.t > 0 else "Po protlačce se zakvasí samo, nebo stiskni Zakvasit. Přeskoč 12 h a sleduj diacetyl.")
        var C = S.cip
        _t(refs.cip_t, "Kde: %s ▸" % pr.cip_name(C.target))
        _t(refs.cip_m, "Čím: %s ▸" % C.medium)
        _t(refs.cip_run, "STOP CIP" if C.run else "Spustit CIP")
        _t(refs.cip_set, "%d °C" % roundi(C.set))
        _t(refs.cip_conc, "%s %%" % c1(C.get("conc", 1.5)))
        _t(refs.cip_info, ("Běží %d min, roztok %s °C · " % [roundi(C.min), c1(C.T)] if C.run else "") + pr.cip_status() + ". Louh ≥ 75 °C a ≥ %s %% NaOH 20 min, kyselina ≤ 60 °C a ≥ %s %% 20 min, mezi tím proplach vodou. Hodnotí se dodržení postupu, ne mikrobiologická čistota." % [c1(S.cfg.get("cipLyeMin", 1.0)), c1(S.cfg.get("cipAcidMin", 0.5))])
        _t(refs.faults, "Nácvik poruch: %s" % ("ZAP" if S.faults.on else "vyp"))
        _t(refs.faults_info, pr.faults_text())
        var sc: String = pr.scorecard_text()
        if refs.score.text != sc:
            refs.score.text = sc;changed = true
    _t(refs.sflow, "%d L/min" % roundi(S.skWater.flow))
    _t(refs.rake, "Kopačka: %s · hloubka %d %%" % [onoff(S.sk.rake), roundi(S.sk.rakeH * 100.0)])
    for key in ["p1", "p2", "p3"]:
        _t(refs[key], "%s %s" % [m.pump_name(key), "STOP" if S[key].on else "start"])
        _t(refs[key + "f"], "%d L/min" % roundi(S[key].flow))
        _t(refs[key + "s"], ("   " + S[key].status) if S[key].on and S[key].status != "" else "")
        var vis: bool = refs[key + "s"].text != ""
        if refs[key + "s"].visible != vis:
            refs[key + "s"].visible = vis;changed = true
    _t(refs.plv, "%s ledová voda %s" % [m.pump_name("plv"), "STOP" if S.lv.pump else "start"])
    _t(refs.chill, "Agregát (glykol): %s" % onoff(S.lv.chill))

    var sim = m.sim
    var lines: = []
    lines.append("[b]Pánev:[/b] %s" % m.vessel_text("rvp"))
    var io = sim.iodine(S.rvp)
    if io != null and S.rvp.g > 5: lines.append("   jodová zkouška: %s" % {"neg": "[color=#7ee08a]negativní (zcukřeno)[/color]", "weak": "[color=#f2c230]slabě modrá[/color]", "pos": "[color=#ff6b5b]modrá – škrob[/color]"}[io])
    lines.append("[b]Scezovací káď:[/b] %s" % m.vessel_text("sk"))
    if S.sk.g > 1:
        var sk_ = S.sk; var risk_: = "nízké" if S.tannin < 0.3 else ("střední" if S.tannin < 0.6 else "VYSOKÉ")
        var load: = roundi(100.0 * sk_.Qact / maxf(1.0, sk_.Qmax))
        lines.append("   lože: průtok %d z max %d L/min (%d %%), tlak %d mbar, zhutnění %d %%, kanálky %d %%, zákal %d, částic %s kg%s" % [roundi(sk_.Qact), roundi(sk_.Qmax), load, roundi(sk_.dp), roundi(sk_.c * 25.0), roundi(sk_.ch * 200.0), roundi(sk_.turb), c1(sk_.fines), " · VZDUCH" if sk_.air > 0.05 else ""])
        lines.append("   třísloviny (varovný index podle pH, teploty a zředění, ne koncentrace): %s" % risk_)
    lines.append("[b]Vířivá káď:[/b] %s" % m.vessel_text("wp"))
    lines.append("[b]Bojler:[/b] %s" % m.vessel_text("hlt"))
    lines.append("[b]Ledovka:[/b] %s" % m.vessel_text("lv"))
    lines.append("[b]Spilka:[/b] %s" % m.vessel_text("spilka"))

    var tr = sim.training
    var cs: String = tr.text() + "\n[color=#7b858e]" + tr.list_text() + "[/color]"
    if check_lbl.text != cs:
        check_lbl.text = cs;changed = true
    var tcur = tr.current()
    refs.next_step.disabled = ( not tcur.ready) or tcur.complete
    _t(refs.step_pause, "Pauza při splnění: " + onoff(tr.state().pause))
    var las = sim.lauter_auto.state()
    _t(refs.lauter_auto, ("Zastavit automat scezování: " if las.on else "Spustit automat scezování: ") + las.stage)
    _t(refs.lauter, "Odrmutovat a vytvořit lože" if S.sk.mode == "mash" else "Lože připraveno – scezování")
    refs.lauter.disabled = S.sk.mode == "lauter"
    var rep = sim.spReport()
    if rep != null:
        lines.append("[b]Protokol (spilka):[/b] %s hl · %s °P · hořkost %d IBU (cíl %d) · barva %s EBC · DMS %d ppb · zdánlivá zkvasitelnost %d %% · výtěžnost %s %%" % [c1(rep.V), c1(rep.P), roundi(rep.IBU), roundi(S.cfg.targetIBU), c1(rep.EBC), roundi(rep.dms), roundi(rep.att), c1(rep.yield )])
    if S.flags.knock != null:
        var yy: Dictionary = m.sim.yields()
        lines.append("[color=#9aa3ab]Vyrážka: %s hl horké při %s °P · výtěžnost varny %s %% · využití lab. extraktu %s %%[/color]" % [c1(S.flags.knock.V / 100.0), c1(S.flags.knock.P), c1(yy.get("VV_varna", 0.0)), c1(yy.get("ucinnost", 0.0))])
    var lost: float = S.drainL + S.spillL
    if lost > 1: lines.append("[color=#ff6b5b][b]Vyteklo / do kanálu:[/b] %d L[/color]" % roundi(lost))
    lines.append("[color=#9aa3ab]Voda z bojleru celkem %d L · pára %d °C[/color]" % [roundi(S.usedHot), roundi(S.Tsteam)])
    var st: = "\n".join(PackedStringArray(lines))
    if state_lbl.text != st:
        state_lbl.text = st;changed = true

    if S.log.size() and S.log[0].t != last_log or log_box.text == "":
        last_log = S.log[0].t if S.log.size() else -1.0
        var ll: = []
        for l in S.log.slice(0, 9):
            var c: = "#ff6b5b" if l.type == "bad" else ("#f2c230" if l.type == "warn" else "#c9d1d8")
            ll.append("[color=#7b858e]%d:%02d[/color] [color=%s]%s[/color]" % [int(l.t / 3600), int(l.t / 60) %60, c, l.msg])
        changed = true
        log_box.text = "\n".join(PackedStringArray(ll)) if ll.size() else "[color=#7b858e]Zatím nic. Otevři klapky, pusť vodu a spusť čas (Spustit).[/color]"
    if changed:
        changed = false;dirty()
