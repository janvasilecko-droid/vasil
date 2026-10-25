extends RefCounted



const STAINLESS: = [[0.0, "2c3237"], [0.07, "5f6870"], [0.18, "aeb7bf"], [0.27, "eef3f6"], [0.31, "ffffff"], [0.36, "dde3e8"], [0.52, "b3bcc4"], [0.72, "858e96"], [0.88, "5a636b"], [1.0, "2a3035"]]
const COPPER: = [[0.0, "4a230e"], [0.15, "8c4620"], [0.32, "f0b07a"], [0.45, "c47a42"], [0.75, "7e3d18"], [1.0, "3a1a08"]]
const CLAD: = [[0.0, "7d858c"], [0.16, "bcc3c9"], [0.3, "f1f4f6"], [0.34, "ffffff"], [0.42, "e6eaed"], [0.65, "c9cfd4"], [0.88, "98a0a7"], [1.0, "6f777e"]]
const NAVY: = [[0.0, "16233a"], [0.25, "2c4a78"], [0.4, "4f74a8"], [0.6, "2f4f80"], [1.0, "142035"]]
const PIPE_STEEL: = Color("a6b0b9")

static func grad(stops: Array, t: float) -> Color:
    t = clampf(t, 0.0, 1.0)
    for i in range(1, stops.size()):
        if t <= stops[i][0]:
            var a = stops[i - 1]; var b = stops[i]
            var k: float = (t - a[0]) / maxf(0.0001, b[0] - a[0])
            return Color(a[1]).lerp(Color(b[1]), k)
    return Color(stops[stops.size() - 1][1])


static func cyl_v(ci: CanvasItem, r: Rect2, stops: Array, n: = 24) -> void :
    for i in n:
        var t0: = float(i) / n; var t1: = float(i + 1) / n
        var x0: = r.position.x + r.size.x * t0; var x1: = r.position.x + r.size.x * t1
        var c0: = grad(stops, t0); var c1: = grad(stops, t1)
        ci.draw_polygon(PackedVector2Array([Vector2(x0, r.position.y), Vector2(x1, r.position.y), Vector2(x1, r.end.y), Vector2(x0, r.end.y)]), PackedColorArray([c0, c1, c1, c0]))


static func cyl_h(ci: CanvasItem, r: Rect2, stops: Array, n: = 20) -> void :
    for i in n:
        var t0: = float(i) / n; var t1: = float(i + 1) / n
        var y0: = r.position.y + r.size.y * t0; var y1: = r.position.y + r.size.y * t1
        var c0: = grad(stops, t0); var c1: = grad(stops, t1)
        ci.draw_polygon(PackedVector2Array([Vector2(r.position.x, y0), Vector2(r.end.x, y0), Vector2(r.end.x, y1), Vector2(r.position.x, y1)]), PackedColorArray([c0, c0, c1, c1]))


static func cone(ci: CanvasItem, cx: float, y: float, w: float, wb: float, h: float, stops: Array, n: = 20) -> void :
    for i in n:
        var t0: = float(i) / n; var t1: = float(i + 1) / n
        var a0: = cx - w * 0.5 + w * t0; var a1: = cx - w * 0.5 + w * t1
        var b0: = cx - wb * 0.5 + wb * t0; var b1: = cx - wb * 0.5 + wb * t1
        var c0: = grad(stops, t0).darkened(0.08); var c1: = grad(stops, t1).darkened(0.08)
        ci.draw_polygon(PackedVector2Array([Vector2(a0, y), Vector2(a1, y), Vector2(b1, y + h), Vector2(b0, y + h)]), PackedColorArray([c0, c1, c1, c0]))


static func dome(ci: CanvasItem, cx: float, y: float, w: float, h: float, stops: Array, n: = 24) -> void :
    for i in n:
        var t0: = float(i) / n; var t1: = float(i + 1) / n
        var x0: = cx - w * 0.5 + w * t0; var x1: = cx - w * 0.5 + w * t1
        var e0: = sqrt(maxf(0.0, 1.0 - pow(t0 * 2.0 - 1.0, 2))); var e1: = sqrt(maxf(0.0, 1.0 - pow(t1 * 2.0 - 1.0, 2)))
        var c0: = grad(stops, t0); var c1: = grad(stops, t1)
        ci.draw_polygon(PackedVector2Array([Vector2(x0, y), Vector2(x0, y - h * e0), Vector2(x1, y - h * e1), Vector2(x1, y)]), PackedColorArray([c0, c0.lightened(0.08), c1.lightened(0.08), c1]))


static func shadow(ci: CanvasItem, r: Rect2, off: = Vector2(10, 14), spread: = 14.0) -> void :
    for i in 5:
        var g: = spread * (5 - i) / 5.0
        ci.draw_rect(Rect2(r.position + off - Vector2(g, g) * 0.5, r.size + Vector2(g, g)), Color(0, 0, 0, 0.07))

static func ellipse_shadow(ci: CanvasItem, c: Vector2, rx: float, ry: float) -> void :
    for i in 5:
        var k: = 1.0 + i * 0.12
        var pts: = PackedVector2Array()
        for a in 24: pts.append(c + Vector2(cos(TAU * a / 24.0) * rx * k, sin(TAU * a / 24.0) * ry * k))
        ci.draw_colored_polygon(pts, Color(0, 0, 0, 0.06))

static  var _sb: = {}

static func rrect(ci: CanvasItem, r: Rect2, col: Color, rad: = 6.0) -> void :
    rrect_border(ci, r, col, col, 0, rad)

static func rrect_border(ci: CanvasItem, r: Rect2, col: Color, border: Color, bw: = 2, rad: = 6.0) -> void :
    var key: = "%s|%s|%d|%d" % [col.to_html(), border.to_html(), bw, int(rad)]
    var sb: StyleBoxFlat = _sb.get(key)
    if sb == null:
        sb = StyleBoxFlat.new();sb.bg_color = col;sb.border_color = border
        sb.set_border_width_all(bw);sb.set_corner_radius_all(int(rad));sb.anti_aliasing = true
        _sb[key] = sb
    ci.draw_style_box(sb, r)


static func pipe(ci: CanvasItem, pts: PackedVector2Array, w: float) -> void :
    ci.draw_polyline(pts, Color(0, 0, 0, 0.16), w + 9.0, true)
    ci.draw_polyline(pts, Color("1b2025"), w + 3.0, true)
    ci.draw_polyline(pts, PIPE_STEEL, w + 1.0, true)
    ci.draw_polyline(pts, Color("d9e0e5"), maxf(1.5, w * 0.45), true)
    ci.draw_polyline(pts, Color(1, 1, 1, 0.85), maxf(1.0, w * 0.14), true)
    if w > 8.0:
        ci.draw_polyline(pts, Color("2a3137"), w - 5.0, true)
        ci.draw_polyline(pts, Color("39424a"), maxf(1.0, w - 9.0), true)

## měkký stín pod nádobou / strojem na podlaze
static func floor_shadow(ci: CanvasItem, c: Vector2, rx: float, ry: float) -> void :
    for i in 6:
        var k: = 1.0 + i * 0.16
        var pts: = PackedVector2Array()
        for a in 32: pts.append(c + Vector2(cos(TAU * a / 32.0) * rx * k, sin(TAU * a / 32.0) * ry * k))
        ci.draw_colored_polygon(pts, Color(0.1, 0.12, 0.14, 0.07))

## kartáčovaný nerez: jemné svislé tahy přes plášť
static func brushed(ci: CanvasItem, r: Rect2) -> void :
    var x: = r.position.x + 3.0; var i: = 0
    while x < r.end.x - 2.0:
        var a: = 0.035 + 0.03 * fmod(i * 0.618, 1.0)
        ci.draw_line(Vector2(x, r.position.y + 2), Vector2(x, r.end.y - 2), Color(1, 1, 1, a) if i % 2 == 0 else Color(0, 0, 0, a * 0.8), 1.0)
        x += 3.0 + fmod(i * 1.7, 4.0);i += 1

static func text_c(ci: CanvasItem, f: Font, t: String, pos: Vector2, size: int, col: Color) -> void :
    var w: = f.get_string_size(t, HORIZONTAL_ALIGNMENT_LEFT, -1, size).x
    ci.draw_string(f, pos - Vector2(w * 0.5, 0), t, HORIZONTAL_ALIGNMENT_LEFT, -1, size, col)


static func pill(ci: CanvasItem, f: Font, t: String, pos: Vector2, size: = 13, fg: = Color("e6ebef"), bg: = Color(0.07, 0.09, 0.11, 0.78)) -> void :
    if t == "": return
    var w: = f.get_string_size(t, HORIZONTAL_ALIGNMENT_LEFT, -1, size).x
    rrect_border(ci, Rect2(pos.x - w * 0.5 - 8, pos.y - size - 3, w + 16, size + 9), bg, Color(1, 1, 1, 0.16), 1, (size + 9) * 0.5)
    ci.draw_string(f, Vector2(pos.x - w * 0.5, pos.y), t, HORIZONTAL_ALIGNMENT_LEFT, -1, size, fg)


static func nameplate(ci: CanvasItem, f: Font, t: String, cx: float, y: float, size: = 16) -> void :
    var w: = f.get_string_size(t, HORIZONTAL_ALIGNMENT_LEFT, -1, size).x + 34
    var r: = Rect2(cx - w * 0.5, y, w, size + 14)
    rrect(ci, Rect2(r.position + Vector2(2, 3), r.size), Color(0, 0, 0, 0.25), 5.0)
    rrect_border(ci, r, Color("171c21"), Color("f2c230"), 2, 5.0)
    ci.draw_line(r.position + Vector2(6, 3), Vector2(r.end.x - 6, r.position.y + 3), Color(1, 1, 1, 0.12), 1.0)
    for sx in [r.position.x + 8.0, r.end.x - 8.0]:
        ci.draw_circle(Vector2(sx, r.position.y + r.size.y * 0.5), 2.2, Color("8a939b"))
        ci.draw_line(Vector2(sx - 1.4, r.position.y + r.size.y * 0.5), Vector2(sx + 1.4, r.position.y + r.size.y * 0.5), Color("3b434a"), 1.0)
    ci.draw_string(f, Vector2(cx - w * 0.5 + 17, y + size + 4), t, HORIZONTAL_ALIGNMENT_LEFT, -1, size, Color("f2c230"))


static func stadium(ci: CanvasItem, r: Rect2, stops: Array, n: = 56) -> void :
    var rr: = r.size.y * 0.5; var cy: = r.position.y + rr
    for i in n:
        var t0: = float(i) / n; var t1: = float(i + 1) / n
        var y0: = r.position.y + r.size.y * t0; var y1: = r.position.y + r.size.y * t1
        var dy: = absf((y0 + y1) * 0.5 - cy)
        var half: = sqrt(maxf(0.0, rr * rr - dy * dy))
        var xl: = r.position.x + rr - half; var xr: = r.end.x - rr + half
        var c0: = grad(stops, t0); var c1: = grad(stops, t1)
        ci.draw_polygon(PackedVector2Array([Vector2(xl, y0), Vector2(xr, y0), Vector2(xr, y1), Vector2(xl, y1)]), PackedColorArray([c0, c0, c1, c1]))
    var pts: = PackedVector2Array()
    for k in 25: pts.append(Vector2(r.end.x - rr, cy) + Vector2(cos( - PI * 0.5 + PI * k / 24.0), sin( - PI * 0.5 + PI * k / 24.0)) * rr)
    for k in 25: pts.append(Vector2(r.position.x + rr, cy) + Vector2(cos(PI * 0.5 + PI * k / 24.0), sin(PI * 0.5 + PI * k / 24.0)) * rr)
    pts.append(pts[0])
    ci.draw_polyline(pts, Color("0d1013"), 2.5, true)


## realistické sprity (vyrenderované z 3D, viz sprites.gd); mipmapy kvůli zmenšení
static var _tx: = {}
static func tex(n: String) -> Texture2D:
    if _tx.has(n): return _tx[n]
    var t: Texture2D = null
    var p: = "res://art/%s.png" % n
    if ResourceLoader.exists(p):
        var src: Texture2D = load(p)
        var im: = src.get_image()
        if im != null:
            im.generate_mipmaps()
            t = ImageTexture.create_from_image(im)
    _tx[n] = t
    return t

## sprite se středem v bodě pivot (v pixelech spritu), otočený a zmenšený na měřítko scale
static func sprite(ci: CanvasItem, n: String, p: Vector2, rot: float, scale: float, pivot: Vector2, flip: = false) -> bool:
    var t: = tex(n)
    if t == null: return false
    ci.texture_filter = CanvasItem.TEXTURE_FILTER_LINEAR_WITH_MIPMAPS
    ci.draw_set_transform(p, rot, Vector2(-scale if flip else scale, scale))
    ci.draw_texture(t, -pivot)
    ci.draw_set_transform(Vector2.ZERO, 0, Vector2.ONE)
    return true

static func tex_rect(ci: CanvasItem, n: String, r: Rect2, tint: = Color.WHITE) -> bool:
    var t: = tex(n)
    if t == null: return false
    ci.texture_filter = CanvasItem.TEXTURE_FILTER_LINEAR_WITH_MIPMAPS
    ci.draw_texture_rect(t, r, false, tint)
    return true
