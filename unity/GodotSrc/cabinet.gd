extends Node2D
const Art = preload("res://art.gd")





var m
var it
var sig: = ""
var sc: = 1.0
var fault_extra: = false

const CAB_W: = 770.0
const CAB_H: = 1130.0
const PX: = 200.0
const PW: = 570.0
const COLS: = [266.0, 353.0, 441.0, 528.0, 616.0, 703.0]
const ROW_Y: = [312.0, 452.0, 592.0, 732.0]

const ROWS: = [
    [["bxs", ["ČERPADLO", "C2", "SLADINA"], "sel", "black"], ["bxp", ["RYCHLOST", "C2"], "pot", ""], ["run", ["PROVOZ"], "lamp", "green"], ["blank", [""], "blank", ""], ["lmp", ["OSVĚTLENÍ", "MP"], "sel", "green"], ["lsk", ["OSVĚTLENÍ", "SK"], "sel", "green"]], 
    [["stirmp", ["MÍCHADLO", "MP"], "sel", "green"], ["rstA", ["RESET", "PORUCHY", "FMB"], "push", "blue"], ["potmp", ["RYCHLOST", "FMB"], "pot", ""], ["mpauto", ["OHŘEV MP", "MAN./AUTO"], "sel", "black"], ["mpheat", ["OHŘEV MP"], "sel", "green"], ["rez1", ["ČERPADLO", "LEDOVÁ V."], "sel", "green"]], 
    [["rake", ["MÍCHADLO", "SK"], "sel", "green"], ["rstB", ["RESET", "PORUCHY", "SK"], "push", "blue"], ["potsk", ["RYCHLOST", "SK"], "pot", ""], ["hyd", ["HYDRAULIKA", "SK"], "sel", "green"], ["hup", ["NAHORU", "SK"], "push", "green"], ["hdn", ["DOLŮ", "SK"], "push", "green"]], 
    [["pboil", ["BOILER", "ČERPADLO", "FMB"], "sel", "green"], ["rstC", ["RESET", "PORUCHY", "FMB"], "push", "blue"], ["potp", ["RYCHLOST", "FMB"], "pot", ""], ["hltheat", ["BOILER", "OHŘEV"], "sel", "black"], ["pvarna", ["VÁRNA", "ČERPADLO", "FMB"], "sel", "black"], ["rez2", ["ČERPADLO", "SPÍLACÍ"], "sel", "black"]], 
]
const DISP: = [["mp", "TEPLOTA MP"], ["sk", "TEPLOTA SK"], ["ml", "TEPLOTA MLADINA"], ["bo", "TEPLOTA BOILER"]]
const DISP_X: = [222.0, 357.0, 492.0, 627.0]
const DISP_Y: = 178.0


const FMS: = {"p1": {"min": 50.0, "max": 400.0, "step": 25.0}, "p2": {"min": 10.0, "max": 200.0, "step": 10.0}, "p3": {"min": 20.0, "max": 150.0, "step": 10.0}}

func setup(item) -> void :
    it = item
    sc = minf(float(it.get("w", CAB_W)) / CAB_W, float(it.get("h", CAB_H)) / CAB_H)

func S():
    return m.sim.S


func _live() -> bool:
    return m.sim.cab_live()

func _sel_on(k: String) -> bool:
    var s = S(); var c = s.cab
    match k:
        "lmp": return c.get("lmp", true)
        "lsk": return c.get("lsk", true)
        "stirmp": return s.rvp.stir
        "mpauto": return c.get("mpAuto", false)
        "mpheat": return c.get("mpHeatOn", false)
        "rake": return s.sk.rake
        "hyd": return c.get("hydOn", false)
        "hltheat": return s.hlt.auto
        "pboil": return s.rvpWater.on or s.skWater.on
        "pvarna": return s.p1.on
        "rez2": return s.p3.on
        "rez1": return s.lv.pump
        "bxs": return s.p2.on
    return false

func _pot_frac(k: String) -> float:
    var s = S()
    match k:
        "potmp": return clampf((s.rvp.hz - 10.0) / 40.0, 0.0, 1.0)
        "potsk": return clampf(s.cab.get("potsk", 0.5), 0.0, 1.0)
        "potp": return _fm_frac("p1")
        "bxp": return _fm_frac("p2")
    return 0.5

## potenciometry měničů mají stupnici 1–10 (jako na skutečném rozvaděči)
func _st(frac: float) -> int:
    return clampi(roundi(1.0 + 9.0 * frac), 1, 10)
func _stf(frac: float) -> String:
    return ("%.1f" % (1.0 + 9.0 * clampf(frac, 0.0, 1.0))).replace(".", ",")
func _frac_of(st: int) -> float:
    return (clampi(st, 1, 10) - 1) / 9.0

## otočné potenciometry: najít pod myší a nastavit plynule (táhnutím do kruhu)
func pot_hit(w: Vector2) -> Dictionary:
    if it == null: return {}
    var d: Vector2 = (w - Vector2(it.x, it.y)) / sc
    for ri in 4:
        for ci in 6:
            var def: Array = ROWS[ri][ci]
            if def[2] != "pot": continue
            var cc: = Vector2(COLS[ci], ROW_Y[ri] + 80.0)
            if d.distance_to(cc) < 40.0: return {"k": def[0], "c": Vector2(it.x, it.y) + cc * sc}
    return {}

func pot_value(k: String) -> float:
    return _pot_frac(k)

func set_pot(k: String, f: float) -> void :
    var s = S(); f = clampf(f, 0.0, 1.0)
    match k:
        "potmp": s.rvp.hz = 10.0 + 40.0 * f
        "potsk": s.cab.potsk = f
        "potp": s.p1.flow = roundf(FMS.p1.min + (FMS.p1.max - FMS.p1.min) * f)
        "bxp": s.p2.flow = roundf(FMS.p2.min + (FMS.p2.max - FMS.p2.min) * f)
    queue_redraw()

func _fm_frac(b: String) -> float:
    var F = FMS[b]
    return clampf((S()[b].flow - F.min) / (F.max - F.min), 0.0, 1.0)

func _status(k: String) -> String:
    var s = S()
    match k:
        "potmp": return "%s · %d Hz" % [_stf(_pot_frac("potmp")), roundi(s.rvp.hz)]
        "potsk": return "stupeň %s" % _stf(_pot_frac("potsk"))
        "potp": return "%s · %d L/min" % [_stf(_pot_frac("potp")), roundi(s.p1.flow)]
        "bxp": return "%s · %d L/min" % [_stf(_pot_frac("bxp")), roundi(s.p2.flow)]
        "stirmp": return ("%d Hz" % roundi(s.rvp.hz)) if s.rvp.stir else "stojí"
        "rake": return ("hloubka %d %%" % roundi(s.sk.rakeH * 100.0)) if s.sk.rake else "stojí"
        "hyd": return "zapnuta" if s.cab.get("hydOn", false) else "vypnuta"
        "pboil":
            var w = s.rvpWater
            return ("%d/%d L" % [roundi(w.added), roundi(w.target)]) if w.on else ("vyslazuje" if s.skWater.on else "stojí")
        "pvarna": return (s.p1.status if s.p1.status != "" else "běží") if s.p1.on else "stojí"
        "rez2": return (s.p3.status if s.p3.status != "" else "běží") if s.p3.on else "stojí"
        "rez1": return ("čerpá" if s.lv.w >= 300 else "ledovka prázdná") if s.lv.pump else "stojí"
        "bxs": return (s.p2.status if s.p2.status != "" else "běží") if s.p2.on else "stojí"
        "mpauto": return "AUTOMAT" if s.cab.get("mpAuto", false) else "ručně"
        "mpheat": return ("pára %d %%" % roundi(s.rvp.heat * 100.0)) if s.rvp.heat > 0 else "vyp"
        "hltheat": return ("termostat %d °C" % roundi(s.hlt.set)) if s.hlt.auto else "vyp"
    return ""

func _disp_text(k: String) -> String:
    var s = S(); var sim = m.sim
    if not s.cab.main: return ""
    var T = null; var V: = 0.0
    match k:
        "mp": T = s.rvp.Tsens;V = sim.vol(s.rvp)
        "sk": T = (s.sk.mash.T if (s.sk.g <= 1.0 and sim.vol(s.sk.mash) > 20.0) else s.sk.T);V = sim.skVolTotal()
        "ml": T = s.cool.Tout if s.cool.Tout != null else (s.wp.T if sim.vol(s.wp) > 20.0 else null);V = 100.0 if T != null else 0.0
        "bo": T = s.hlt.T;V = s.hlt.w
    if T == null or V < 20.0: return "---"
    return ("%d" % roundi(T)) if T >= 100.0 else ("%.1f" % T).replace(".", ",")

func _sig() -> String:
    var s = S(); var c = s.cab
    var a: = [c.main, c.key, c.estop, c.get("mpHeatOn", false), roundi(c.get("potsk", 0.5) * 90.0), c.get("lmp", true), c.get("lsk", true), c.get("mpAuto", false), c.get("hydOn", false), c.get("fault", false), roundi(c.get("potsk", 0.5) * 10.0), c.get("mpSet", 50), s.hlt.set, s.hlt.auto]
    a.append_array([s.rvp.stir, roundi(s.rvp.hz), roundi(s.rvp.heat * 100.0), s.sk.rake, roundi(s.sk.rakeH * 20.0), s.p1.on, s.p2.on, s.p3.on, s.lv.pump, s.rvpWater.on, s.skWater.on])
    a.append_array([roundi(s.p1.flow), roundi(s.p2.flow), roundi(s.p3.flow), s.p1.status, s.p2.status, s.p3.status, roundi(s.rvpWater.added / 20.0), s.lv.w >= 300])
    for d in DISP: a.append(_disp_text(d[0]))
    for fk in flash:
        if _flashing(fk) > 0.0: a.append("%s%d" % [fk, int(Time.get_ticks_msec() / 50)])
    return str(a)

func upd() -> void :
    var g: = _sig()
    if g != sig:
        sig = g;queue_redraw()


func _t(text: String, pos: Vector2, size: int, col: Color, bold: = true) -> void :
    Art.text_c(self, m.font_b if bold else m.font, text, pos, size, col)

func _bezel(c: Vector2, r: float) -> void :
    draw_circle(c, r, Color("c3c7cc"));draw_arc(c, r, 0, TAU, 28, Color("5b6066"), 1.6, true)
    draw_arc(c, r - 3.0, 0, TAU, 28, Color(0.93, 0.94, 0.95, 0.7), 1.0, true)

func _sticker(cx: float, top: float, lines: Array, sw: float) -> void :
    var lh: = 10.8; var hh: float = lines.size() * lh + 8.0
    Art.rrect_border(self, Rect2(cx - sw / 2.0, top, sw, hh), Color("f3f3f0"), Color("b9b9b3"), 1, 2.0)
    for i in lines.size():
        var l: String = lines[i]
        _t(l, Vector2(cx, top + 10.0 + i * lh), 7 if l.length() >= 9 else (8 if l.length() >= 7 else 9), Color("3a3d40"))

func _colors(n: String) -> Array:
    match n:
        "green": return [Color("1f6b43"), Color("0f3b27")]
        "blue": return [Color("2a55b8"), Color("17306c")]
        "red": return [Color("d8402e"), Color("7a1d14")]
    return [Color("26292d"), Color("111315")]

func _lamp(cx: float, cy: float, on: bool, kind: String) -> void :
    var c: = Vector2(cx, cy);_bezel(c, 24.0)
    var off: = Color("d9dcdf") if kind == "white" else (Color("1d4d36") if kind == "green" else Color("5a1d1d"))
    var onc: = Color("ffffff") if kind == "white" else (Color("46ff86") if kind == "green" else Color("ff4b3a"))
    if on: _glow(c, onc, 1.0)
    draw_circle(c, 19.0, onc if on else off);draw_arc(c, 19.0, 0, TAU, 24, Color("1b1e21"), 1.5, true)
    draw_circle(c + Vector2(-6, -7), 5.0, Color(1, 1, 1, 0.3))

## krátké podsvícení po stisku (zapnutí i vypnutí) – klíč → čas stisku [ms]
var flash: = {}
func _flashing(k: String) -> float:
    var t: int = flash.get(k, -100000)
    return clampf(1.0 - (Time.get_ticks_msec() - t) / 700.0, 0.0, 1.0)

func _glow(c: Vector2, col: Color, a: float) -> void :
    for i in 4: draw_circle(c, 40.0 - i * 4.0, Color(col, (0.07 + i * 0.03) * a))

func _push(cx: float, cy: float, col: String, pressed: = false, k: = "") -> void :
    var c: = Vector2(cx, cy); var cc: = _colors(col)
    var fl: = maxf(1.0 if pressed else 0.0, _flashing(k))
    if fl > 0.0:
        _glow(c, cc[0].lightened(0.5), fl)
        draw_rect(Rect2(cx - 28, cy - 80, 56, 112), Color(cc[0].lightened(0.5), 0.9 * fl), false, 3.0)
    _bezel(c, 24.0)
    draw_circle(c, 18.5, cc[0].lightened(0.45 * fl + (0.25 if pressed else 0.0)));draw_arc(c, 18.5, 0, TAU, 24, cc[1], 2.0, true)
    draw_circle(c + Vector2(-6, -7), 5.0, Color(1, 1, 1, 0.28))

func _sel(cx: float, cy: float, col: String, on: bool, k: = "") -> void :
    var c: = Vector2(cx, cy); var cc: = _colors(col)
    var fl: = _flashing(k)
    if fl > 0.0 and not on:
        # právě vypnuto: krátce blikne oranžově
        _glow(c, Color("ffb13b"), fl)
        draw_rect(Rect2(cx - 28, cy - 80, 56, 112), Color(1.0, 0.7, 0.25, 0.9 * fl), false, 3.0)
    if on:
        # zapnuto: podsvícený volič svítí zeleně
        for i in 4: draw_circle(c, 40.0 - i * 4.0, Color(0.2, 1.0, 0.35, 0.07 + i * 0.03))
        draw_rect(Rect2(cx - 28, cy - 80, 56, 112), Color(0.25, 1.0, 0.4, 0.9), false, 3.0)
        cc = [Color("2fe063"), Color("0f6b2a")]
    _bezel(c, 24.0)
    if on: draw_arc(c, 24.0, 0, TAU, 32, Color("4dff7c"), 3.0, true)
    draw_circle(c, 18.5, cc[0]);draw_arc(c, 18.5, 0, TAU, 24, cc[1], 2.0, true)
    var ang: float = deg_to_rad(35.0 if on else -35.0)

    var dir: = Vector2(sin(ang), cos(ang))
    draw_line(c + dir * 3.0, c + dir * 27.0, cc[1], 9.0, true)
    draw_line(c + dir * 3.0, c + dir * 27.0, Color(1, 1, 1, 0.18), 3.0, true)

func _key(cx: float, cy: float, on: bool) -> void :
    var c: = Vector2(cx, cy);_bezel(c, 24.0)
    draw_circle(c, 17.0, Color("2b2e32"));draw_arc(c, 17.0, 0, TAU, 24, Color("0e0f11"), 1.5, true)
    var ang: float = deg_to_rad(40.0 if on else -40.0)
    var d: = Vector2(sin(ang), cos(ang))
    draw_line(c - d * 8.0, c + d * 8.0, Color("0d0e10"), 6.0, true)
    draw_line(c + d * 6.0, c + d * 42.0, Color("9aa0a6"), 4.0, true)
    draw_arc(c + d * 44.0, 6.0, 0, TAU, 14, Color("9aa0a6"), 2.0, true)

func _pot(cx: float, cy: float, frac: float) -> void :
    var c: = Vector2(cx, cy);_bezel(c, 27.0)
    for i in 10:
        var a: float = deg_to_rad(-135.0 + 30.0 * i)
        var r1: = 21.0; var r2: = 26.0
        draw_line(c + Vector2(sin(a), - cos(a)) * r1, c + Vector2(sin(a), - cos(a)) * r2, Color("2a2d30"), 1.4)
        var tp: = c + Vector2(sin(a), - cos(a)) * 37.0; var ts: = str(i + 1)
        var tw: float = m.font_b.get_string_size(ts, HORIZONTAL_ALIGNMENT_LEFT, -1, 13).x
        draw_string_outline(m.font_b, tp + Vector2(- tw * 0.5, 5), ts, HORIZONTAL_ALIGNMENT_LEFT, -1, 13, 4, Color("0d0e10"))
        draw_string(m.font_b, tp + Vector2(- tw * 0.5, 5), ts, HORIZONTAL_ALIGNMENT_LEFT, -1, 13, Color("ffd95a"))
    draw_circle(c, 16.0, Color("1c1e21"));draw_arc(c, 16.0, 0, TAU, 20, Color.BLACK, 1.5, true)
    var ang: float = deg_to_rad(-135.0 + 270.0 * frac)
    var dd: = Vector2(sin(ang), - cos(ang))
    draw_line(c + dd * 3.0, c + dd * 15.0, Color.WHITE, 2.4, true)
    _t("−", c + Vector2(-22, 38), 13, Color("6a7077"));_t("+", c + Vector2(22, 38), 13, Color("6a7077"))

func _draw() -> void :
    if it == null or m == null or m.sim == null: return
    var s = S(); var c = s.cab; var live: = _live()
    draw_set_transform(Vector2(it.x, it.y), 0.0, Vector2(sc, sc))

    Art.shadow(self, Rect2(PX + 4.0, 6.0, PW, CAB_H), Vector2(14, 18), 22.0)
    Art.rrect_border(self, Rect2(PX, 0, PW, CAB_H), Color("c6c9cd"), Color("6b7076"), 3, 6.0)
    for i in 26:
        var sx: float = PX + 8.0 + i * 22.0 + (i * 37) %11
        draw_line(Vector2(sx, 6), Vector2(sx, CAB_H - 6), Color(1, 1, 1, 0.08) if i % 3 else Color(0.49, 0.51, 0.53, 0.09), 2.0 if i % 4 == 0 else 1.0)
    for sp_ in [[PX + 14.0, 14.0], [PX + PW - 14.0, 14.0], [PX + 14.0, CAB_H - 14.0], [PX + PW - 14.0, CAB_H - 14.0]]:
        draw_circle(Vector2(sp_[0], sp_[1]), 5.0, Color("9ea3a8"));draw_arc(Vector2(sp_[0], sp_[1]), 5.0, 0, TAU, 12, Color("5b6066"), 1.0)

    Art.rrect_border(self, Rect2(PX + 22, 28, 150, 62), Color("f6f6f3"), Color("2a2d30"), 2, 3.0)
    _t("RT.20", Vector2(PX + 97, 56), 30, Color("2a2d30"))
    _t("3/PEN AC 400/230V", Vector2(PX + 97, 78), 11, Color("2a2d30"))
    _t(String(it.get("label", "ROZVADĚČ VARNA")), Vector2(PX + PW - 90, 50), 13, Color("3a3d40"))

    for i in 4:
        var dx: float = DISP_X[i]; var k: String = DISP[i][0]
        Art.rrect_border(self, Rect2(dx, DISP_Y - 34, 122, 22), Color("f3f3f0"), Color("b9b9b3"), 1, 2.0)
        _t(DISP[i][1], Vector2(dx + 61, DISP_Y - 23), 10, Color("2a2d30"))
        Art.rrect_border(self, Rect2(dx, DISP_Y, 122, 70), Color("8a8e92"), Color("3a3d40"), 2, 5.0)
        draw_rect(Rect2(dx + 24, DISP_Y + 9, 70, 40), Color("0a0c0d"))
        var tx: = _disp_text(k) if c.main else ""
        draw_string(m.font_b, Vector2(dx + 26, DISP_Y + 43), tx, HORIZONTAL_ALIGNMENT_RIGHT, 66, 27, Color("ff4a38"))
        draw_circle(Vector2(dx + 12, DISP_Y + 18), 5.0, Color("6e7276"));draw_circle(Vector2(dx + 12, DISP_Y + 40), 5.0, Color("6e7276"))
        for ar in [[8.0, true], [28.0, false]]:
            Art.rrect_border(self, Rect2(dx + 100, DISP_Y + ar[0], 18, 16), Color("6e7276"), Color("3a3d40"), 1, 3.0)
            var cxa: float = dx + 109; var cya: float = DISP_Y + ar[0] + 8.0
            if ar[1]: draw_colored_polygon(PackedVector2Array([Vector2(cxa, cya - 5), Vector2(cxa + 5, cya + 5), Vector2(cxa - 5, cya + 5)]), Color("d6d9dc"))
            else: draw_colored_polygon(PackedVector2Array([Vector2(cxa, cya + 5), Vector2(cxa + 5, cya - 5), Vector2(cxa - 5, cya - 5)]), Color("d6d9dc"))
        _t("dixell", Vector2(dx + 67, DISP_Y + 65), 8, Color("d6d9dc"))
        if k == "mp" and c.main: _t("SP %d °C" % roundi(c.get("mpSet", 50)), Vector2(dx + 61, DISP_Y + 84), 10, Color("7a5a00"))
        if k == "bo" and c.main: _t("SP %d °C" % roundi(s.hlt.set), Vector2(dx + 61, DISP_Y + 84), 10, Color("7a5a00"))

    var fault: bool = c.get("fault", false)
    var pots_later: = []
    for ri in 4:
        var row: Array = ROWS[ri]
        for ci in 6:
            var def: Array = row[ci]; var k: String = def[0]; var ty: String = def[2]
            if ty == "blank": continue
            var X: float = COLS[ci]; var top: float = ROW_Y[ri]; var cy: = top + 80.0
            draw_rect(Rect2(X - 28, top, 56, 112), Color("14171a"))
            _sticker(X, top + 5.0, def[1], 50.0)
            match ty:
                "lamp":
                    var on: = false
                    match k:
                        "net": on = c.main
                        "run": on = live
                        "flt": on = c.estop or fault
                    _lamp(X, cy, on, def[3])
                "key": _key(X, cy, c.key)
                "push": _push(X, cy, def[3], (k == "hup" and s.sk.hyd < 0.0) or (k == "hdn" and s.sk.hyd > 0.0), k)
                "sel": _sel(X, cy, def[3], _sel_on(k), k)
                "pot": pots_later.append([X, cy, _pot_frac(k)])
            if ty in ["sel", "pot"] or k in ["hup", "hdn"]:
                var st: = _status(k)
                if st != "": _t(st, Vector2(X, top + 128.0), 10, Color("1d2126"))
    for pl in pots_later: _pot(pl[0], pl[1], pl[2])

    Art.rrect_border(self, Rect2(PX + 22, 890, 104, 20), Color("f2c230"), Color("8a6a00"), 1, 2.0)
    _t("HLAVNÍ VYPÍNAČ", Vector2(PX + 74, 900), 10, Color("2a2400"))
    Art.rrect_border(self, Rect2(PX + 30, 916, 88, 88), Color("18191b"), Color.BLACK, 2, 6.0)
    var mc: = Vector2(PX + 74, 960)
    draw_circle(mc, 34.0, Color("232527"));draw_arc(mc, 34.0, 0, TAU, 28, Color.BLACK, 2.0, true)
    draw_circle(mc, 27.0, Color("2c2f32"))
    var mang: float = deg_to_rad(90.0 if c.main else 0.0)
    var md: = Vector2( - sin(mang), - cos(mang))
    draw_line(mc + md * 8.0, mc + md * 28.0, Color("f1f1ef"), 6.0, true)
    _t("ZAP" if c.main else "VYP", Vector2(PX + 74, 1022), 9, Color("2fbf5b") if c.main else Color("a33a2f"))
    Art.rrect_border(self, Rect2(PX + 150, 900, 128, 124), Color("f3f3f0"), Color("b9b9b3"), 1, 4.0)
    Art.rrect(self, Rect2(PX + 150, 900, 128, 26), Color("d7332a"), 4.0)
    _t("VÝSTRAHA", Vector2(PX + 214, 913), 12, Color.WHITE)
    for i in 4:
        draw_rect(Rect2(PX + 158, 934 + i * 21, 112, 3), Color("8c8f93"));draw_rect(Rect2(PX + 158, 941 + i * 21, 88, 3), Color("b5b8bb"))
    Art.rrect_border(self, Rect2(PX + 290, 888, 150, 22), Color("f6f6f3"), Color("b9b9b3"), 1, 2.0)
    _t("NOUZOVÉ VYPNUTÍ", Vector2(PX + 365, 899), 11, Color("c1281d"))
    var ec: = Vector2(PX + 365, 972)
    draw_circle(ec, 52.0, Color("f0b323"));draw_arc(ec, 52.0, 0, TAU, 32, Color("8a6200"), 2.0, true)
    draw_circle(ec, 38.0, Color("e09a14"))
    draw_circle(ec, 30.0 if not c.estop else 26.0, Color("e03021") if not c.estop else Color("8f2016"));draw_arc(ec, 30.0, 0, TAU, 28, Color("7a130b"), 3.0, true)
    if not c.estop: draw_circle(ec + Vector2(-9, -9), 8.0, Color(1, 1, 1, 0.28))
    _t("EMERGENCY STOP", ec + Vector2(0, -34), 7, Color("7a5200"))
    Art.rrect_border(self, Rect2(PX + 456, 900, 104, 124), Color("f3f3f0"), Color("b9b9b3"), 1, 4.0)
    Art.rrect(self, Rect2(PX + 456, 900, 104, 34), Color("1f5e3b"), 4.0)
    _t("BEZPEČNOSTNÍ", Vector2(PX + 508, 910), 9, Color.WHITE);_t("INSTRUKCE", Vector2(PX + 508, 923), 9, Color.WHITE)
    for i in 3:
        draw_rect(Rect2(PX + 464, 946 + i * 26, 88, 3), Color("8c8f93"));draw_rect(Rect2(PX + 464, 953 + i * 26, 70, 3), Color("b5b8bb"))
    Art.rrect_border(self, Rect2(PX + 300, 1040, 132, 76), Color("f6f6f3"), Color("b9b9b3"), 1, 3.0)
    draw_colored_polygon(PackedVector2Array([Vector2(PX + 366, 1046), Vector2(PX + 394, 1092), Vector2(PX + 338, 1092)]), Color("f4d21e"))
    draw_rect(Rect2(PX + 300, 1096, 132, 20), Color("f4d21e"))
    _t("POZOR ELEKTRICKÉ ZAŘÍZENÍ", Vector2(PX + 366, 1106), 7, Color("1a1a1a"))

    draw_set_transform(Vector2.ZERO, 0.0, Vector2.ONE)



func hit(w: Vector2) -> String:
    if it == null: return ""
    var d: Vector2 = (w - Vector2(it.x, it.y)) / sc
    if d.x < 0.0 or d.y < 0.0 or d.x > CAB_W or d.y > CAB_H: return ""
    for ri in 4:
        for ci in 6:
            var def: Array = ROWS[ri][ci]; var ty: String = def[2]
            var cc: = Vector2(COLS[ci], ROW_Y[ri] + 80.0)
            if ty == "pot":
                if Rect2(cc.x - 30, cc.y - 30, 60, 60).has_point(d): return def[0] + ("_d" if d.x < cc.x else "_u")
            elif ty != "lamp" and ty != "blank" and d.distance_to(cc) < 27.0: return def[0]
    for i in [0, 3]:
        var dx: float = DISP_X[i]
        if Rect2(dx + 100, DISP_Y + 8, 18, 16).has_point(d): return ("spmp" if i == 0 else "spbo") + "_u"
        if Rect2(dx + 100, DISP_Y + 28, 18, 16).has_point(d): return ("spmp" if i == 0 else "spbo") + "_d"
    if Rect2(PX + 30, 916, 88, 88).has_point(d): return "main"
    if d.distance_to(Vector2(PX + 365, 972)) < 52.0: return "estop"
    return ""

func _dead() -> void :
    var c = S().cab
    if not c.main: m._say("Rozvaděč je bez napětí – zapni HLAVNÍ VYPÍNAČ.")
    elif c.estop: m._say("Nouzové vypnutí je stisknuté – klepni na hřib – odblokuje se.")
    else: m._say("Rozvaděč neběží – otoč klíč START.")

func _log(t: String, ty: = "info") -> void :
    m.sim.logm(t, ty)

func _bind_step(b: String, dir: int) -> void :
    var s = S()
    if FMS.has(b):
        var F = FMS[b]
        var st: = _st(_fm_frac(b)) + dir
        s[b].flow = roundf(F.min + (F.max - F.min) * _frac_of(st))

## RÁZ: vypnout míchadlo při vysokých otáčkách = prudké zastavení (setrvačnost hřídele a lopat v hustém rmutu).
## Správně: nejdřív snížit otáčky potenciometrem, pak vypnout. Prahy a míra poškození jsou ODHAD.
func _stop_shock(which: String) -> void :
    var s = S(); var tgt = s.rvp if which == "mp" else s.sk
    var sev: float = 0.0
    if which == "mp":
        var load: float = 0.2
        if s.rvp.w > 200: load = clampf(0.4 + 0.6 * m.sim.thick_factor(s.rvp), 0.4, 1.0)
        sev = clampf((s.rvp.hz - 15.0) / 35.0, 0.0, 1.0) * load
    else:
        var pf: float = s.cab.get("potsk", 0.5)
        sev = clampf((pf - 0.25) / 0.75, 0.0, 1.0) * clampf(s.sk.rakeH * 1.2, 0.0, 1.0)
    if sev < 0.08: return
    tgt["wear"] = tgt.get("wear", 0.0) + sev * 0.5
    var what: String = "míchadla MP" if which == "mp" else "kopačky SK"
    if sev > 0.55 or tgt.wear >= 1.0:
        tgt["broken"] = true
        _log("RÁZ! Vypnutí %s za plných otáček – hřídel se prudce zastavila, ulomila se spojka/lopatka. Nejdřív sniž otáčky, pak vypínej. Oprava: RESET PORUCHY." % what, "bad")
        if m.hud != null: m.hud.toast("RÁZ – poškození %s!" % what)
    elif sev > 0.2:
        _log("Silný ráz při vypnutí %s (otáčky byly vysoké) – namáhá spojku a převodovku. Příště nejdřív sniž otáčky." % what, "warn")
    else:
        _log("Lehký ráz při vypnutí %s – radši nejdřív sniž otáčky." % what, "warn")

func act(k: String, down: bool) -> void :
    var s = S(); var c = s.cab; var sk = s.sk; var rv = s.rvp
    if down: flash[k.trim_suffix("_u").trim_suffix("_d")] = Time.get_ticks_msec()
    if k == "hup" or k == "hdn":
        if not down:
            sk.hyd = 0.0;return
        if not _live(): _dead();return
        if not c.get("hydOn", false): m._say("Hydraulika SK je vypnutá – zapni volič HYDRAULIKA SK.");return
        sk.hyd = -1.0 if k == "hup" else 1.0
        return
    if not down: return
    if k == "main":
        c.main = not c.main
        _log("Hlavní vypínač zapnut – rozvaděč pod napětím." if c.main else "Hlavní vypínač vypnut – rozvaděč bez napětí, všechno elektrické stojí.", "info" if c.main else "bad")
        m.redraw();return
    if k == "key":
        if not c.main: m._say("Nejdřív zapni HLAVNÍ VYPÍNAČ.");return
        if c.estop: m._say("Nouzové vypnutí je stisknuté – nejdřív ho odblokuj (klepni na hřib).");return
        c.key = not c.key
        _log("START: rozvaděč v provozu." if c.key else "Klíč START vypnut – rozvaděč v klidu, všechno elektrické stojí.", "info" if c.key else "warn")
        m.redraw();return
    if k == "estop":
        c.estop = not c.estop
        if c.estop:
            c.key = false;_log("NOUZOVÉ VYPNUTÍ! Všechno elektrické vypnuto. Odblokuj hřib (klepni na něj).", "bad")
        else:
            c.key = true;_log("Nouzové vypnutí odblokováno – rozvaděč zase v provozu.", "info")
        m.redraw();return
    if not _live(): _dead();return
    if not m.running: m._say("Čas stojí – nahoře stiskni SPUSTIT, ať se to rozběhne.")
    match k:
        "lmp": c.lmp = not c.get("lmp", true);_log("Osvětlení MP %s." % ("zapnuto" if c.lmp else "vypnuto"))
        "lsk": c.lsk = not c.get("lsk", true);_log("Osvětlení SK %s." % ("zapnuto" if c.lsk else "vypnuto"))
        "stirmp":
            if not rv.stir and rv.get("broken", false):
                _log("Míchadlo MP je poškozené po rázu – nejdřív oprava (RESET PORUCHY FMB = výměna spojky/lopatky).", "bad");return
            rv.stir = not rv.stir
            _log("Míchadlo MP %s." % (("zapnuto (%d Hz)" % roundi(rv.hz)) if rv.stir else "vypnuto"))
            if not rv.stir: _stop_shock("mp")
        "potmp_u", "potmp_d": rv.hz = roundf(10.0 + 40.0 * _frac_of(_st(_pot_frac("potmp")) + (1 if k == "potmp_u" else -1)))
        "mpauto":
            c.mpAuto = not c.get("mpAuto", false)
            _log(("Ohřev MP: AUTOMAT – drží %d °C (nastavíš šipkami na displeji TEPLOTA MP)." % roundi(c.get("mpSet", 50))) if c.mpAuto else "Ohřev MP: RUČNĚ – pára podle nastavení ohřevu vpravo.")
        "mpheat":
            c.mpHeatOn = not c.get("mpHeatOn", false)
            if c.mpHeatOn:
                if not c.get("mpAuto", false) and rv.heat <= 0.0: rv.heat = c.get("heatMem", 1.0)
            else:
                if rv.heat > 0.0: c["heatMem"] = rv.heat
                rv.heat = 0.0
            _log("Ohřev MP %s." % ("zapnut" if c.mpHeatOn else "vypnut"))
            if c.mpHeatOn and not s.boiler.on: m._say("Parní kotel je vypnutý – pára nejde (v panelu vpravo zapni kotel).")
        "spmp_u", "spmp_d": c.mpSet = clampf(c.get("mpSet", 50) + (1.0 if k == "spmp_u" else -1.0), 20.0, 105.0)
        "spbo_u", "spbo_d": s.hlt.set = clampf(s.hlt.set + (1.0 if k == "spbo_u" else -1.0), 40.0, 95.0)
        "rake":
            if not sk.rake and sk.get("broken", false):
                _log("Kopačka SK je poškozená po rázu – nejdřív oprava (RESET PORUCHY SK = výměna střižného kolíku).", "bad");return
            sk.rake = not sk.rake
            if not sk.rake: _stop_shock("sk")
            _log(("Míchadlo SK (kopačka) se točí, ale je nahoře – spusť ji hydraulikou do lože." if sk.rakeH < 0.4 else "Míchadlo SK (kopačka) běží.") if sk.rake else "Míchadlo SK (kopačka) zastaveno.")
        "potsk_u", "potsk_d": c.potsk = _frac_of(_st(_pot_frac("potsk")) + (1 if k == "potsk_u" else -1))
        "hyd":
            c.hydOn = not c.get("hydOn", false)
            if not c.hydOn: sk.hyd = 0.0
            _log("Hydraulika SK %s." % ("zapnuta" if c.hydOn else "vypnuta"))
        "pboil": m.toggle_pump("phw")
        "pvarna": m.toggle_pump("p1")
        "rez2": m.toggle_pump("p3")
        "rez1": m.toggle_pump("plv")
        "bxs": m.toggle_pump("p2")
        "potp_u", "potp_d": _bind_step("p1", 1 if k == "potp_u" else -1)
        "bxp_u", "bxp_d": _bind_step("p2", 1 if k == "bxp_u" else -1)
        "hltheat":
            s.hlt.auto = not s.hlt.auto
            _log("Ohřev boileru %s." % (("zapnut (termostat %d °C)" % roundi(s.hlt.set)) if s.hlt.auto else "vypnut"))
        "rstA", "rstB", "rstC":
            c.fault = false
            if k == "rstA" and rv.get("broken", false):
                rv["broken"] = false;rv["wear"] = 0.0;_log("Míchadlo MP opraveno (nová spojka / lopatka).")
            if k == "rstB" and sk.get("broken", false):
                sk["broken"] = false;sk["wear"] = 0.0;_log("Kopačka SK opravena (nový střižný kolík).")
            for pk in ["p1", "p2", "p3"]:
                s[pk].dry = 0.0;s[pk].dead = 0.0
            _log("Reset poruchy měniče.")
    m.redraw()
