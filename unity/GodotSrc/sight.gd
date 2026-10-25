extends Control




var m
var it
var sample: = {}
var acc: = 0.0
var t_anim: = 0.0
var parts: = []          # kousky sladu / slupky plynoucí sklem: {x, y, sz, rot, rs, k}
var rng: = RandomNumberGenerator.new()
var next_burst: = 6.0
var last_ch: = 0.0
var last_dep: = 0.0

func open(item) -> void :
    it = item
    mouse_filter = Control.MOUSE_FILTER_STOP
    size = Vector2(330, 450);position = Vector2(172, 226)
    refresh()

func _gui_input(e: InputEvent) -> void :
    if e is InputEventMouseButton and e.pressed and e.button_index == MOUSE_BUTTON_LEFT:
        if Rect2(size.x - 34, 4, 28, 28).has_point(e.position):
            queue_free()
        accept_event()

func _process(d: float) -> void :
    acc += d;t_anim += d
    _parts_step(d)
    if acc >= 0.25:
        acc = 0.0;refresh()
    queue_redraw()


func _burst(n: int, big: bool) -> void :
    for i in n:
        parts.append({"x": -106.0 if sample.get("from", "a") == "a" else 106.0, "y": rng.randf_range(-70.0, 70.0), "sz": rng.randf_range(4.0, 9.0 if big else 6.5), "rot": rng.randf() * TAU, "rs": rng.randf_range(-2.5, 2.5), "k": 0 if rng.randf() < 0.72 else 1, "v": rng.randf_range(0.7, 1.25)})

## drobné kousky sladu: při zákalu stále, v čiré sladině jen občas „něco utrhne“; ráz / kanálek v loži = dávka
func _parts_step(d: float) -> void :
    if m == null or m.sim == null or sample.get("empty", true) or sample.get("none", false): parts.clear();return
    var S = m.sim.S
    var flow: float = sample.get("flow", 0.0)
    var haze: float = clampf(sample.get("fines", 0.0) * 400.0, 0.0, 1.0)
    var dir: float = 1.0 if sample.get("from", "a") == "a" else -1.0
    var spd: float = flow / 60.0 * 90.0
    if flow > 0.5 and sample.get("kind", "") in ["wort", "mash"]:
        # plynulý přísun podle zákalu
        if rng.randf() < haze * 5.0 * d: _burst(1, false)
        # občasné utržení kousku z lože (i při čiré sladině)
        next_burst -= d
        if next_burst <= 0.0:
            next_burst = rng.randf_range(5.0, 16.0) / (1.0 + haze * 2.0)
            _burst(rng.randi_range(1, 3) + (rng.randi_range(2, 5) if haze > 0.3 else 0), false)
        # události v loži: trhnutí / kanálky / utržený nános
        var ch: float = S.sk.ch; var dep: float = S.sk.get("dep", 0.0)
        if ch > last_ch + 0.001: _burst(rng.randi_range(10, 22), true)
        if dep < last_dep - 0.01: _burst(rng.randi_range(8, 18), true)
        last_ch = ch;last_dep = dep
    for p in parts:
        if flow > 0.5: p.x += spd * dir * p.v * d
        else: p.y += 6.0 * d          # stojící kapalina: kousky pomalu klesají
        p.y += sin(t_anim * 3.0 + p.rot) * 6.0 * d
        p.rot += p.rs * d
    parts = parts.filter(func(p): return absf(p.x) < 112.0 and absf(p.y) < 112.0)
    if parts.size() > 90: parts = parts.slice(parts.size() - 90)

func take_sample(pos: Vector2) -> Dictionary:
    var sim = m.sim
    var best = null; var bd: = 40.0; var bt: = 0.0
    for e in sim.net.edges:
        var pts: Array = e.pts
        var tot: = 0.0
        for i in pts.size() - 1: tot += pts[i].distance_to(pts[i + 1])
        var run: = 0.0
        for i in pts.size() - 1:
            var a: Vector2 = pts[i]; var b: Vector2 = pts[i + 1]
            var c: = Geometry2D.get_closest_point_to_segment(pos, a, b)
            var dd: = c.distance_to(pos)
            if dd < bd:
                bd = dd;best = e;bt = (run + a.distance_to(c)) / maxf(1.0, tot)
            run += a.distance_to(b)
    if best == null: return {"none": true}
    var e = best
    var total: float = sim.vol(e.c)
    var x: float = bt * e.cap

    var lo: float = 0.0 if e.from == "a" else e.cap - total
    var rel: float = x - lo
    var found = null
    if rel >= 0.0 and rel <= total:
        var cum: = 0.0
        for b in e.q:
            var vb: float = sim.vol(b)
            if rel <= cum + vb + 1e-09:
                found = b;break
            cum += vb
    var s: = {"edge": e, "flow": e.flow, "from": e.from, "cap": e.cap, "filled": total}
    if found == null:
        s["empty"] = true;return s
    var V: float = maxf(1e-09, sim.vol(found))
    s["empty"] = false
    s["kind"] = sim.contentKind(found)
    s["T"] = found.T
    s["P"] = sim.plato(found)
    s["fines"] = found.fines / V
    s["air"] = found.air / V
    s["col"] = found.col / maxf(0.01, sim.liqVol(found))
    return s

func refresh() -> void :
    if it == null or m == null or m.sim == null: return
    sample = take_sample(Vector2(it.x, it.y))

func _draw() -> void :
    var f: Font = m.font; var fb: Font = m.font_b
    Art.rrect_border(self, Rect2(Vector2.ZERO, size), Color(0.07, 0.085, 0.1, 0.97), Color("f2c230"), 2, 8.0)
    draw_string(fb, Vector2(12, 22), "PRŮHLEDÍTKO", HORIZONTAL_ALIGNMENT_LEFT, -1, 15, Color("f2c230"))
    draw_string(f, Vector2(12, 38), str(it.get("label", "")), HORIZONTAL_ALIGNMENT_LEFT, 250, 11, Color("9aa3ab"))
    draw_string(fb, Vector2(size.x - 26, 22), "X", HORIZONTAL_ALIGNMENT_LEFT, -1, 18, Color("e6ebef"))
    var c: = Vector2(size.x * 0.5, 156); var R: = 106.0
    _frame(c, R)
    if sample.get("none", false):
        draw_circle(c, R, Color(0.15, 0.18, 0.2));draw_string(f, Vector2(14, size.y - 14), "Průhledítko není na potrubí.", HORIZONTAL_ALIGNMENT_LEFT, -1, 13, Color("ff6b5b"));return
    var flow: float = sample.get("flow", 0.0)
    if sample.get("empty", true):
        draw_circle(c, R, Color(0.45, 0.52, 0.58, 0.35))
        _glass(c, R)
        _lines(["Sklo je prázdné – v tomto místě potrubí nic není."], ["e6ebef"])
        _flow_arrow(c, R, flow);return
    var col: Color = m.kind_color(sample.kind, sample.T, sample.col)
    col.a = 1.0
    # kapalina: světlejší střed, tmavší okraj (tloušťka vrstvy kapaliny v tubusu)
    draw_circle(c, R, col.darkened(0.28))
    draw_circle(c, R * 0.86, col.darkened(0.12))
    draw_circle(c, R * 0.6, col)
    draw_circle(c, R * 0.3, col.lightened(0.10))
    var haze: float = clampf(sample.fines * 400.0, 0.0, 1.0)
    draw_circle(c, R, Color(0.93, 0.89, 0.78, haze * 0.7))
    var spd: float = flow / 60.0 * 90.0
    var dir: float = 1.0 if sample.from == "a" else -1.0
    # jemný zákal – drobné světlé částice proudí sklem
    var n: int = int(haze * 220.0)
    for i in n:
        var rx: float = fposmod(float(i) * 37.13 + t_anim * spd * dir, 2.0 * R) - R
        var ry: float = fposmod(float(i) * 91.7, 2.0 * R) - R
        if rx * rx + ry * ry < (R - 6.0) * (R - 6.0):
            draw_circle(c + Vector2(rx, ry), 1.0 + fposmod(float(i) * 0.37, 1.8), Color(0.98, 0.95, 0.85, 0.5))
    # kousky sladu: světlé slupky (zahnuté lupínky) a tmavší kousky zrna
    for p in parts:
        var pp: Vector2 = c + Vector2(p.x, p.y)
        if pp.distance_to(c) > R - 9.0: continue
        var dv: Vector2 = Vector2(cos(p.rot), sin(p.rot)) * float(p.sz)
        var nv: Vector2 = Vector2(-dv.y, dv.x) * 0.28
        if p.k == 0:
            draw_colored_polygon(PackedVector2Array([pp - dv, pp - nv * 0.6 + dv * 0.0, pp + dv, pp + nv * 0.6]), Color("d9be82"))
            draw_line(pp - dv, pp + dv, Color("8a6a3a"), 1.0, true)
        else:
            draw_circle(pp, p.sz * 0.42, Color("6f4f28"));draw_circle(pp - Vector2(1, 1), p.sz * 0.2, Color("a98350"))
    # bubliny
    var nb: int = int(clampf(sample.air * 2500.0, 0.0, 40.0))
    for i in nb:
        var bx: float = fposmod(float(i) * 53.7 + t_anim * spd * dir, 2.0 * R) - R
        var by: float = fposmod(float(i) * 29.3 - t_anim * 30.0, 2.0 * R) - R
        if bx * bx + by * by < (R - 8.0) * (R - 8.0):
            draw_arc(c + Vector2(bx, by), 3.0 + fposmod(float(i) * 0.7, 3.0), 0, TAU, 12, Color(1, 1, 1, 0.8), 1.5, true)
    # proudění: světlé pruhy ve směru toku
    if flow > 0.5:
        for i in 6:
            var sy: float = -R * 0.75 + i * R * 0.3
            var hw: float = sqrt(maxf(0.0, R * R * 0.9 - sy * sy))
            var sx: float = fposmod(t_anim * spd * dir * 1.6 + i * 41.0, 2.0 * hw) - hw
            draw_line(c + Vector2(sx - 12.0, sy), c + Vector2(sx + 12.0, sy), Color(1, 1, 1, 0.10), 2.0, true)
    _glass(c, R)
    _flow_arrow(c, R, flow)
    var lines: = []
    var cols: = []
    var kind_cz: String = {"wort": "sladina / mladina", "mash": "rmut (s pevnými částicemi)", "water": "voda", "hot": "horká voda", "ice": "ledová voda"}.get(sample.kind, sample.kind)
    lines.append("Kapalina: " + kind_cz);cols.append("e6ebef")
    lines.append("Teplota: %s °C   Extrakt: %s °P" % [m.c1(sample.T), m.c1(sample.P)]);cols.append("e6ebef")
    if flow > 0.5:
        lines.append("Průtok: %d L/min (%s)" % [roundi(flow), "a → b" if sample.from == "a" else "b → a"]);cols.append("7ee08a")
    else:
        lines.append("Kapalina STOJÍ.");cols.append("f2c230")
    var hz: = "čirá, jiskrná" if haze < 0.08 else ("lehce opalizující" if haze < 0.2 else ("zakalená" if haze < 0.5 else ("velmi zakalená" if haze < 0.75 else "kalná, slupky")))
    lines.append("Čirost: %s (%s g/l jemných částic)" % [hz, m.c1(sample.fines * 1000.0)]);cols.append("7ee08a" if haze < 0.2 else ("f2c230" if haze < 0.5 else "ff6b5b"))
    lines.append("Vzduch: %s" % ("bubliny (zavzdušnění, odhad)" if nb > 0 else "žádný"));cols.append("ff6b5b" if nb > 0 else "9aa3ab")
    if sample.kind == "wort" and flow > 0.5:
        lines.append("Vorlauf hotový, až je sladina čirá." if haze >= 0.2 else "Čirá – můžeš scezovat do pánve.");cols.append("9aa3ab" if haze >= 0.2 else "7ee08a")
    _lines(lines, cols)
    # stupnice čirosti
    var by0: = size.y - 26.0
    draw_rect(Rect2(14, by0, size.x - 28, 8), Color(0.15, 0.18, 0.2))
    var gr: = Color("3fbf5f").lerp(Color("e0a020"), clampf(haze * 2.0, 0.0, 1.0)).lerp(Color("e0452f"), clampf(haze * 2.0 - 1.0, 0.0, 1.0))
    draw_rect(Rect2(14, by0, (size.x - 28) * haze, 8), gr)
    draw_string(f, Vector2(14, by0 - 3), "zákal", HORIZONTAL_ALIGNMENT_LEFT, -1, 10, Color("9aa3ab"))

## ocelová příruba s šrouby a těsněním
func _frame(c: Vector2, R: float) -> void :
    draw_circle(c, R + 16, Color("1b2025"))
    draw_circle(c, R + 14, Color("8e98a1"))
    draw_circle(c, R + 10, Color("c3ccd3"))
    draw_circle(c, R + 5, Color("3a434b"))
    draw_circle(c, R + 2, Color("111417"))
    for i in 8:
        var a: = TAU * i / 8.0 + 0.2
        var p: = c + Vector2(cos(a), sin(a)) * (R + 10.0)
        draw_circle(p, 3.2, Color("59626a"));draw_circle(p, 1.6, Color("aeb7bf"))

## sklo: odlesky a okrajové ztmavení
func _glass(c: Vector2, R: float) -> void :
    draw_arc(c, R - 1.0, 0, TAU, 64, Color(0, 0, 0, 0.35), 4.0, true)
    draw_arc(c, R - 5.0, PI * 1.05, PI * 1.55, 20, Color(1, 1, 1, 0.45), 4.0, true)
    draw_arc(c, R - 12.0, PI * 1.15, PI * 1.4, 12, Color(1, 1, 1, 0.25), 3.0, true)
    draw_circle(c + Vector2(R * 0.45, R * 0.5), R * 0.08, Color(1, 1, 1, 0.12))

func _lines(lines: Array, cols: Array) -> void :
    var y: = 318.0
    for i in lines.size():
        draw_string(m.font, Vector2(14, y), lines[i], HORIZONTAL_ALIGNMENT_LEFT, size.x - 28, 12, Color(cols[i]))
        y += 18.0

func _flow_arrow(c: Vector2, R: float, flow: float) -> void :
    if flow <= 0.5: return
    var up: bool = sample.get("from", "a") == "a"
    var p: = c + Vector2(0, R + 26)
    var dx: = 30.0 * (1.0 if up else -1.0)
    draw_line(p - Vector2(dx, 0), p + Vector2(dx, 0), Color("7ee08a"), 3.0, true)
    draw_colored_polygon(PackedVector2Array([p + Vector2(dx, 0), p + Vector2(dx * 0.6, -8), p + Vector2(dx * 0.6, 8)]), Color("7ee08a"))

const Art = preload("res://art.gd")
