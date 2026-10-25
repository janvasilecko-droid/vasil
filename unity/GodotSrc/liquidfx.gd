extends RefCounted
## Realistický vzhled kapalin a sypkých látek: mláto, rmut, kal, pěna, chmel.
## Textury se vyrobí procedurálně jednou (bezešvé dlaždice), pak se jen kreslí.
## Vzorem byly fotky: mláto v scezovací kádi (mokré, hnědé, plevy a drť), rmut (kaše s plevami),
## sladina (čirá zlatá), mladina s chmelem (tmavší, jantarová, pěna, zelenavé kousky chmele),
## hot break (bílé vločky bílkovin) a kal ve vířivce (světle hnědá krémová kupka uprostřed dna).

static var _tex: Dictionary = {}

static func h(i: float, s: float = 0.0) -> float:
    return fposmod(sin(i * 12.9898 + s * 78.233) * 43758.5453, 1.0)

## hodnotový šum, bezešvý po periodě per
static func _vn(x: float, y: float, per: int, seed: float) -> float:
    var xi: int = int(floor(x)); var yi: int = int(floor(y))
    var fx: float = x - xi; var fy: float = y - yi
    fx = fx * fx * (3.0 - 2.0 * fx);fy = fy * fy * (3.0 - 2.0 * fy)
    var x0: int = posmod(xi, per); var x1: int = posmod(xi + 1, per)
    var y0: int = posmod(yi, per); var y1: int = posmod(yi + 1, per)
    var a: float = h(x0 + y0 * 57.0, seed); var b: float = h(x1 + y0 * 57.0, seed)
    var c: float = h(x0 + y1 * 57.0, seed); var d: float = h(x1 + y1 * 57.0, seed)
    return lerpf(lerpf(a, b, fx), lerpf(c, d, fx), fy)

static func _fbm(x: float, y: float, per: int, seed: float) -> float:
    return _vn(x, y, per, seed) * 0.62 + _vn(x * 2.5, y * 2.5, per * 2, seed + 1.0) * 0.38

## vloček (plevy, drť) otisknutý do obrázku s přetečením přes okraj (bezešvé)
static func _stamp(img: Image, cx: float, cy: float, ang: float, len: float, wid: float, col: Color, shade: float) -> void:
    var W: int = img.get_width(); var H: int = img.get_height()
    var ca: float = cos(ang); var sa: float = sin(ang)
    var R: int = int(ceil(len * 0.5 + 2.0))
    for dy in range(-R, R + 1):
        for dx in range(-R, R + 1):
            var u: float = (dx * ca + dy * sa) / (len * 0.5)
            var v: float = (-dx * sa + dy * ca) / (wid * 0.5)
            var rr: float = u * u * 0.9 + v * v
            if rr > 1.0: continue
            var px: int = posmod(int(round(cx)) + dx, W); var py: int = posmod(int(round(cy)) + dy, H)
            # válcové nasvícení vločky: světlejší nahoře, tmavý okraj dole
            var lit: float = 1.0 + shade * (-v * 0.55) - 0.25 * rr * rr
            var c: Color = Color(col.r * lit, col.g * lit, col.b * lit, 1.0)
            img.set_pixel(px, py, c)

static func _mk(img: Image) -> ImageTexture:
    return ImageTexture.create_from_image(img)

## mokré mláto: plevy (světlé a tmavé), drť, tmavé mezery, lesklé body
static func tex_bed() -> ImageTexture:
    if _tex.has("bed"): return _tex["bed"]
    var W: int = 192; var H: int = 128
    var img: = Image.create(W, H, false, Image.FORMAT_RGBA8)
    for y in H:
        for x in W:
            var n: float = _fbm(x / 22.0, y / 22.0, 8, 3.0)
            var g: float = 0.34 + 0.34 * n + 0.10 * h(x + y * 211.0, 1.0)
            img.set_pixel(x, y, Color(g * 1.06, g * 0.94, g * 0.74, 1.0))
    var pal: = [Color(0.84, 0.72, 0.50), Color(0.72, 0.58, 0.38), Color(0.60, 0.47, 0.30), Color(0.46, 0.35, 0.21), Color(0.90, 0.82, 0.64), Color(0.38, 0.28, 0.17)]
    for i in 1560:
        var cx: float = h(i, 7.0) * W; var cy: float = h(i, 8.0) * H
        var ang: float = h(i, 9.0) * 3.14159 if h(i, 10.0) < 0.7 else (h(i, 9.0) - 0.5) * 0.9
        var ln: float = 3.6 + h(i, 11.0) * 5.4
        var k: float = h(i, 12.0)
        var pc: Color = pal[int(k * k * 5.99)]
        _stamp(img, cx, cy, ang, ln, ln * (0.40 + 0.18 * h(i, 13.0)), pc, 0.7)
    # drobná drť a lesk vlhkých povrchů
    for i in 520:
        var x: int = int(h(i, 21.0) * W); var y: int = int(h(i, 22.0) * H)
        var c: Color = img.get_pixel(x, y)
        var k: float = 1.25 if h(i, 23.0) < 0.5 else 0.7
        img.set_pixel(x, y, Color(minf(1.0, c.r * k), minf(1.0, c.g * k), minf(1.0, c.b * k), 1.0))
    for i in 260:
        var x2: int = int(h(i, 31.0) * W); var y2: int = int(h(i, 32.0) * H)
        img.set_pixel(x2, y2, Color(1, 0.97, 0.88, 1))
    _tex["bed"] = _mk(img)
    return _tex["bed"]

## rmut: kaše – jemná drť a mouka, plevy plavou v hustém těstě
static func tex_mash() -> ImageTexture:
    if _tex.has("mash"): return _tex["mash"]
    var W: int = 128; var H: int = 128
    var img: = Image.create(W, H, false, Image.FORMAT_RGBA8)
    for y in H:
        for x in W:
            var n: float = _fbm(x / 30.0, y / 30.0, 4, 11.0)
            var g: float = 0.80 + 0.22 * (n - 0.5) + 0.06 * h(x + y * 131.0, 2.0)
            img.set_pixel(x, y, Color(g * 1.03, g * 0.98, g * 0.90, 1.0))
    var pal: = [Color(0.70, 0.58, 0.38), Color(0.58, 0.46, 0.28), Color(0.95, 0.88, 0.70), Color(0.86, 0.76, 0.54)]
    for i in 320:
        var cx: float = h(i, 41.0) * W; var cy: float = h(i, 42.0) * H
        var ang: float = h(i, 43.0) * 3.14
        var ln: float = 4.0 + h(i, 44.0) * 6.0
        var k: int = int(h(i, 45.0) * 3.99)
        _stamp(img, cx, cy, ang, ln, ln * 0.38, pal[k], 0.5)
    _tex["mash"] = _mk(img)
    return _tex["mash"]

## kal / hot break: světlá krémově hnědá kupka s vločkami
static func tex_trub() -> ImageTexture:
    if _tex.has("trub"): return _tex["trub"]
    var W: int = 128; var H: int = 128
    var img: = Image.create(W, H, false, Image.FORMAT_RGBA8)
    for y in H:
        for x in W:
            var n: float = _fbm(x / 14.0, y / 14.0, 9, 51.0)
            var g: float = 0.78 + 0.34 * (n - 0.5) + 0.05 * h(x + y * 77.0, 5.0)
            img.set_pixel(x, y, Color(g * 1.04, g * 0.97, g * 0.82, 1.0))
    for i in 300:
        var cx: float = h(i, 61.0) * W; var cy: float = h(i, 62.0) * H
        var ang: float = h(i, 63.0) * 6.28
        var ln: float = 3.0 + h(i, 64.0) * 6.0
        var k: Color = Color(0.58, 0.55, 0.30) if h(i, 65.0) < 0.35 else Color(1.0, 0.97, 0.86)
        _stamp(img, cx, cy, ang, ln, ln * 0.7, k, 0.3)
    _tex["trub"] = _mk(img)
    return _tex["trub"]

## pěna: bublinky s okraji a odlesky
static func tex_foam() -> ImageTexture:
    if _tex.has("foam"): return _tex["foam"]
    var W: int = 192; var H: int = 96
    var img: = Image.create(W, H, false, Image.FORMAT_RGBA8)
    for y in H:
        for x in W:
            var n: float = _fbm(x / 18.0, y / 18.0, 10, 71.0)
            var g: float = 0.90 + 0.10 * n
            img.set_pixel(x, y, Color(g, g * 0.985, g * 0.93, 1.0))
    for i in 260:
        var cx: float = h(i, 81.0) * W; var cy: float = h(i, 82.0) * H
        var r: float = 3.0 + h(i, 83.0) * 7.5
        var shade: float = 0.80 + 0.18 * h(i, 84.0)
        var R: int = int(ceil(r)) + 1
        for dy in range(-R, R + 1):
            for dx in range(-R, R + 1):
                var d: float = sqrt(float(dx * dx + dy * dy))
                if d > r: continue
                var px: int = posmod(int(cx) + dx, W); var py: int = posmod(int(cy) + dy, H)
                var edge: float = smoothstep(r - 1.6, r, d)
                var hl: float = smoothstep(r * 0.55, 0.0, sqrt(float((dx + r * 0.35) * (dx + r * 0.35) + (dy + r * 0.35) * (dy + r * 0.35))))
                var base: float = shade * (1.0 - 0.34 * edge) + 0.22 * hl
                img.set_pixel(px, py, Color(minf(1.0, base * 1.02), minf(1.0, base), minf(1.0, base * 0.94), 1.0))
    _tex["foam"] = _mk(img)
    return _tex["foam"]

# ---------------------------------------------------------------------------------------------
# kreslení
# ---------------------------------------------------------------------------------------------

## texturovaný mnohoúhelník s barvou ve vrcholech; px_per_texel = měřítko textury v obraze
static func draw_tex_poly(ci: CanvasItem, pts: PackedVector2Array, cols: PackedColorArray, tex: Texture2D, off: Vector2, px_per_texel: float) -> void:
    var sz: Vector2 = tex.get_size() * px_per_texel
    var uv: = PackedVector2Array()
    for p in pts: uv.append((p + off) / sz)
    ci.draw_polygon(pts, cols, uv, tex)

## vločky plevy a drti pohybující se v kapalině (míchání, kypření, promíchání mláta s vodou)
static func draw_suspended(ci: CanvasItem, rect: Rect2, outline: PackedVector2Array, amount: float, t: float, speed: float, col: Color, seed: float, big: float = 1.0) -> void:
    if amount < 0.02: return
    var n: int = int(clampf(rect.get_area() / 420.0, 10.0, 140.0) * amount)
    for i in n:
        var u: float = h(i, seed); var v: float = h(i, seed + 1.0)
        # kruhový oběh (boční pohled na vír): x kmitá, y plave, hloubka mění velikost
        var ph: float = t * speed * (0.35 + 0.65 * h(i, seed + 2.0)) + h(i, seed + 3.0) * 6.28
        var cx: float = rect.position.x + rect.size.x * (0.5 + (u - 0.5) * 0.84 * cos(ph * 0.5 + u * 3.0))
        var cy: float = rect.position.y + rect.size.y * (v + 0.045 * sin(ph * 1.3))
        var p: = Vector2(cx, cy)
        if outline.size() > 2 and not Geometry2D.is_point_in_polygon(p, outline): continue
        var depth: float = 0.5 + 0.5 * sin(ph * 0.5 + u * 3.0)
        var ln: float = (3.0 + 4.0 * h(i, seed + 4.0)) * big * (0.7 + 0.5 * depth)
        var c: Color = col.lightened(0.18 * (h(i, seed + 5.0) - 0.4))
        c.a = (0.35 + 0.5 * depth) * minf(1.0, amount)
        var ang: float = ph * 1.7 + h(i, seed + 6.0) * 3.0
        var d: = Vector2(cos(ang), sin(ang)) * ln * 0.5
        ci.draw_line(p - d, p + d, c, maxf(1.3, ln * 0.33), true)

## bílé vločky hot breaku (sražené bílkoviny) – při varu stoupají a padají
static func draw_flocs(ci: CanvasItem, rect: Rect2, outline: PackedVector2Array, amount: float, t: float, seed: float) -> void:
    if amount < 0.02: return
    var n: int = int(clampf(rect.get_area() / 480.0, 10.0, 160.0) * amount)
    for i in n:
        var u: float = h(i, seed); var ph: float = t * (0.25 + 0.5 * h(i, seed + 1.0)) + h(i, seed + 2.0) * 6.28
        var cx: float = rect.position.x + rect.size.x * clampf(u + 0.05 * sin(ph * 1.7), 0.02, 0.98)
        var cy: float = rect.position.y + rect.size.y * (0.5 + 0.48 * sin(ph))
        var p: = Vector2(cx, cy)
        if outline.size() > 2 and not Geometry2D.is_point_in_polygon(p, outline): continue
        var r: float = 0.9 + 1.5 * h(i, seed + 3.0)
        var c: = Color(0.98, 0.91, 0.70, 0.45 + 0.3 * h(i, seed + 4.0))
        ci.draw_circle(p, r, c)
        ci.draw_circle(p + Vector2(r * 0.9, -r * 0.2), r * 0.6, Color(c, c.a * 0.8))
        ci.draw_circle(p + Vector2(-r * 0.8, r * 0.4), r * 0.5, Color(c, c.a * 0.7))
        ci.draw_circle(p + Vector2(r * 0.2, r * 0.9), r * 0.45, Color(c, c.a * 0.6))

## kousky chmele (zelenohnědé pelety a lístky) ve vařící mladině a v pěně
static func draw_hops(ci: CanvasItem, rect: Rect2, outline: PackedVector2Array, amount: float, t: float, seed: float, surface_y: float, calm: float) -> void:
    if amount < 0.02: return
    var n: int = int(clampf(rect.get_area() / 520.0, 8.0, 130.0) * amount)
    for i in n:
        var u: float = h(i, seed); var ph: float = t * (0.2 + 0.55 * h(i, seed + 1.0)) * (1.0 - 0.8 * calm) + h(i, seed + 2.0) * 6.28
        var cx: float = rect.position.x + rect.size.x * clampf(u + 0.04 * sin(ph * 1.3), 0.02, 0.98)
        var by: float = rect.position.y + rect.size.y * (0.5 + 0.45 * sin(ph))
        # klidná mladina: kousky chmele plavou u hladiny a na dně
        var cy: float = lerpf(by, surface_y + 3.0 + 6.0 * h(i, seed + 7.0), calm * 0.65)
        var p: = Vector2(cx, cy)
        if outline.size() > 2 and not Geometry2D.is_point_in_polygon(p, outline): continue
        var ln: float = 1.8 + 2.6 * h(i, seed + 3.0)
        var g: float = h(i, seed + 4.0)
        var c: Color = Color(0.40, 0.42, 0.12, 0.85) if g < 0.5 else (Color(0.52, 0.45, 0.14, 0.85) if g < 0.85 else Color(0.27, 0.24, 0.08, 0.85))
        var ang: float = h(i, seed + 5.0) * 3.14 + ph * 0.6
        var d: = Vector2(cos(ang), sin(ang)) * ln * 0.5
        ci.draw_line(p - d, p + d, c, 1.9, true)

## pěna při varu: tlustá vrstva bublin se zbarvením od chmelových pryskyřic
static func draw_foam(ci: CanvasItem, x0: float, x1: float, surf: PackedVector2Array, thick: float, hops: float, t: float) -> void:
    if thick < 1.0 or surf.size() < 2: return
    var tex: Texture2D = tex_foam()
    var top: = PackedVector2Array(); var bot: = PackedVector2Array()
    var cols_t: = PackedColorArray()
    for i in surf.size():
        var q: Vector2 = surf[i]
        var r: float = float(i) / float(surf.size() - 1)
        var bump: float = 0.55 + 0.45 * sin(r * 17.0 + t * 1.6) * sin(r * 7.0 - t * 0.9)
        top.append(Vector2(q.x, q.y - thick * bump + 2.0))
        bot.append(Vector2(q.x, q.y + 3.0))
    var poly: = PackedVector2Array()
    poly.append_array(top)
    for i in range(bot.size() - 1, -1, -1): poly.append(bot[i])
    var cols: = PackedColorArray()
    for p in poly:
        # od okraje k okraji lehce hnědne (pryskyřice u stěny), nahoře čistě bílá
        var edge: float = absf((p.x - (x0 + x1) * 0.5) / ((x1 - x0) * 0.5))
        var tint: float = clampf(hops * 0.55 + edge * 0.25, 0.0, 0.8)
        cols.append(Color(1.0, 0.98, 0.92).lerp(Color(0.86, 0.72, 0.42), tint))
    var polys: = [poly]
    # texturovaný mnohoúhelník musí být jednoduchý – okraj pěny je jednoduchá smyčka
    ci.draw_polygon(poly, cols, _uvs(poly, tex, Vector2(t * 4.0, 0.0), 0.9), tex)
    ci.draw_polyline(top, Color(1, 1, 1, 0.55), 1.4, true)

static func _uvs(pts: PackedVector2Array, tex: Texture2D, off: Vector2, px: float) -> PackedVector2Array:
    var sz: Vector2 = tex.get_size() * px
    var uv: = PackedVector2Array()
    for p in pts: uv.append((p + off) / sz)
    return uv

## kupka kalu uprostřed dna vířivky (kužel s měkkými boky)
static func draw_trub_cone(ci: CanvasItem, cx: float, floor_y: float, half_w: float, height: float, t: float) -> void:
    if height < 2.0: return
    var tex: Texture2D = tex_trub()
    var pts: = PackedVector2Array(); var cols: = PackedColorArray()
    var N: int = 18
    for i in N + 1:
        var u: float = float(i) / float(N) * 2.0 - 1.0
        # profil: úhel sypání ~35°, špička zaoblená
        var prof: float = pow(1.0 - absf(u), 0.75) * (1.0 + 0.04 * sin(u * 23.0))
        pts.append(Vector2(cx + u * half_w, floor_y - height * prof))
    pts.append(Vector2(cx + half_w, floor_y + 1.0));pts.append(Vector2(cx - half_w, floor_y + 1.0))
    for p in pts:
        var rel: float = clampf((floor_y - p.y) / maxf(1.0, height), 0.0, 1.0)
        var side: float = absf((p.x - cx) / maxf(1.0, half_w))
        var k: float = 0.88 + 0.22 * rel - 0.32 * side * side
        cols.append(Color(0.93 * k, 0.82 * k, 0.60 * k, 1.0))
    ci.draw_polygon(pts, cols, _uvs(pts, tex, Vector2.ZERO, 1.2), tex)
    # lesk na mokrém povrchu a stín u paty
    var rim: = PackedVector2Array()
    for i in range(0, N + 1): rim.append(pts[i] + Vector2(0, 1.5))
    ci.draw_polyline(rim, Color(1, 0.95, 0.80, 0.35), 1.5, true)
    ci.draw_line(Vector2(cx - half_w - 2, floor_y + 1), Vector2(cx + half_w + 2, floor_y + 1), Color(0, 0, 0, 0.3), 2.0)

## měkká kulatá skvrna (radiální gradient) pro páru
static func tex_puff() -> ImageTexture:
    if _tex.has("puff"): return _tex["puff"]
    var n: = 64
    var img: = Image.create(n, n, false, Image.FORMAT_RGBA8)
    for yy in n:
        for xx in n:
            var d: float = Vector2(xx - n * 0.5 + 0.5, yy - n * 0.5 + 0.5).length() / (n * 0.5)
            var a: float = clampf(1.0 - d, 0.0, 1.0)
            a = a * a * (3.0 - 2.0 * a)
            a *= 0.85 + 0.15 * _vn(xx / 9.0, yy / 9.0, 8, 3.0)
            img.set_pixel(xx, yy, Color(0.97, 0.98, 1.0, a))
    var t: = ImageTexture.create_from_image(img)
    _tex["puff"] = t
    return t

## předpřipravit všechny textury (při startu, ať to neškubne při prvním vykreslení)
static func prewarm() -> void:
    tex_bed();tex_mash();tex_trub();tex_foam();tex_puff()


## Částice v kapalině (suspenze): plevy, drť, vločky hot breaku, kousky chmele, jemný kal.
## Pohybují se podle proudění: míchání/var = dvě protiběžné konvekční buňky (nahoru uprostřed, dolů u stěn),
## vířivka = oběh kolem osy a pád ke středu dna, klid = lehké plevy stoupají k hladině a těžká drť klesá ke dnu.
## st = stav (uchovává polohy), p = parametry: kind "mash"/"wort", stir 0..1, boil, whirl 0..1, a_husk, a_floc, a_hop, a_fine
static func particles(ci: CanvasItem, st: Dictionary, key: String, rect: Rect2, outline: PackedVector2Array, dt: float, tt: float, p: Dictionary) -> void:
    if rect.size.x < 8.0 or rect.size.y < 8.0: return
    var seed: float = float(p.get("seed", 1.0))
    var mash: bool = p.get("kind", "wort") == "mash"
    var n: int = int(clampf(rect.get_area() / (260.0 if mash else 340.0), 24.0, 260.0))
    var pos: PackedVector2Array = st.get(key, PackedVector2Array())
    var ph: PackedFloat32Array = st.get(key + "_ph", PackedFloat32Array())
    if pos.size() != n:
        pos = PackedVector2Array(); ph = PackedFloat32Array()
        for i in n:
            pos.append(Vector2(h(i, seed), h(i, seed + 1.0)))
            ph.append(h(i, seed + 2.0) * 6.2832)
    var stir: float = float(p.get("stir", 0.0)); var boil: bool = p.get("boil", false); var whirl: float = float(p.get("whirl", 0.0))
    var A: float = 46.0 * stir + (62.0 if boil else 0.0)
    var a_husk: float = float(p.get("a_husk", 0.0)); var a_floc: float = float(p.get("a_floc", 0.0))
    var a_hop: float = float(p.get("a_hop", 0.0)); var a_fine: float = float(p.get("a_fine", 0.0))
    var base: Color = p.get("col", Color(0.72, 0.58, 0.36))
    var W: float = rect.size.x; var Hh: float = rect.size.y
    for i in n:
        var c: float = h(i, seed + 3.0)
        var kind: int = 0   # 0 plevy, 1 drť, 2 vločka, 3 chmel, 4 kal
        var act: float = h(i, seed + 4.0)
        if mash:
            if c < 0.55: kind = 0
            elif c < 0.85: kind = 1
            else: kind = 4
            if act > a_husk: continue
        else:
            if c < 0.34: kind = 2
            elif c < 0.62: kind = 3
            else: kind = 4
            var amt: float = a_floc if kind == 2 else (a_hop if kind == 3 else a_fine)
            if act > amt: continue
        var q: Vector2 = pos[i]
        var b: float = -0.9 if kind == 0 else (0.55 if kind == 1 else (0.9 if kind == 3 and false else (0.35 if kind == 2 else 0.12)))
        if kind == 3: b = -0.25 + 0.5 * h(i, seed + 7.0)
        var u: float = -0.5 * sin(6.2832 * q.x) * cos(3.1416 * q.y)
        var vy: float = cos(6.2832 * q.x) * sin(3.1416 * q.y)
        var vx_px: float = A * u * 2.0 + (h(i, seed + 8.0) - 0.5) * 4.0 * (1.0 + 3.0 * stir)
        var vy_px: float = A * vy + b * (16.0 - 11.0 * clampf(stir + (1.0 if boil else 0.0), 0.0, 1.0))
        ph[i] += dt * (1.0 + 2.5 * stir + 2.0 * whirl)
        if whirl > 0.02:
            vx_px += whirl * 55.0 * cos(ph[i] * 1.3) * (0.4 + 0.6 * h(i, seed + 9.0))
            vx_px += (0.5 - q.x) * 38.0 * whirl
            vy_px += 11.0 * whirl
        q.x += vx_px * dt / W
        q.y += vy_px * dt / Hh
        q.x = clampf(q.x, 0.02, 0.98); q.y = clampf(q.y, 0.012, 0.985)
        pos[i] = q
        var sp: Vector2 = rect.position + Vector2(q.x * W, q.y * Hh)
        if outline.size() > 2 and not Geometry2D.is_point_in_polygon(sp, outline): continue
        var depth: float = 0.55 + 0.45 * sin(ph[i] * 0.8 + q.x * 5.0)
        var ang: float = ph[i] * (0.4 + 0.6 * stir) + h(i, seed + 5.0) * 3.14
        match kind:
            0:
                var ln: float = (3.2 + 3.0 * h(i, seed + 6.0)) * (0.75 + 0.35 * depth)
                var cc: Color = base.lerp(Color(0.92, 0.82, 0.58), 0.35 * h(i, seed + 10.0))
                cc = cc.darkened(0.30 * (1.0 - depth)); cc.a = 0.55 + 0.4 * depth
                var d: = Vector2(cos(ang), sin(ang)) * ln * 0.5
                ci.draw_line(sp - d, sp + d, cc, 1.5 + 0.9 * depth, true)
                ci.draw_line(sp - d + Vector2(0, -0.6), sp + d + Vector2(0, -0.6), cc.lightened(0.28), 0.8, true)
            1:
                var gc: Color = Color(0.90, 0.80, 0.58, 0.45 + 0.35 * depth)
                ci.draw_circle(sp, 0.9 + 0.7 * h(i, seed + 6.0), gc)
            2:
                var r: float = 0.9 + 1.4 * h(i, seed + 6.0)
                var fc: = Color(0.98, 0.92, 0.72, 0.40 + 0.35 * depth)
                ci.draw_circle(sp, r, fc)
                ci.draw_circle(sp + Vector2(r * 0.9, -r * 0.2), r * 0.6, Color(fc, fc.a * 0.8))
                ci.draw_circle(sp + Vector2(-r * 0.7, r * 0.5), r * 0.5, Color(fc, fc.a * 0.7))
            3:
                var g: float = h(i, seed + 11.0)
                var hc: Color = Color(0.40, 0.42, 0.12, 0.85) if g < 0.5 else (Color(0.52, 0.45, 0.14, 0.85) if g < 0.85 else Color(0.27, 0.24, 0.08, 0.85))
                var hl: float = 1.8 + 2.4 * h(i, seed + 6.0)
                var hd: = Vector2(cos(ang), sin(ang)) * hl * 0.5
                ci.draw_line(sp - hd, sp + hd, hc, 1.7, true)
            _:
                ci.draw_circle(sp, 0.7 + 0.5 * h(i, seed + 6.0), Color(base.r * 1.05, base.g * 1.0, base.b * 0.9, 0.30 + 0.25 * depth))
    st[key] = pos; st[key + "_ph"] = ph

## odlesky světla v kapalině (kaustika) a lesk u stěn: kapalina působí průhledně, ne jako plná barva
static func draw_caustics(ci: CanvasItem, rect: Rect2, outline: PackedVector2Array, tt: float, strength: float, col: Color) -> void:
    if rect.size.y < 14.0 or strength < 0.02: return
    var nb: int = int(clampf(rect.size.y / 38.0, 2.0, 8.0))
    for k in nb:
        var yy: float = rect.position.y + rect.size.y * (0.10 + 0.8 * float(k) / float(nb)) + 4.0 * sin(tt * 0.6 + k * 2.1)
        var pts: = PackedVector2Array()
        var inside: bool = true
        for i in 14:
            var px: float = rect.position.x + rect.size.x * i / 13.0
            var py: float = yy + 3.5 * sin(px * 0.035 + tt * (0.9 + 0.13 * k) + k) + 2.0 * sin(px * 0.09 - tt * 1.3)
            var pt: = Vector2(px, py)
            if outline.size() > 2 and not Geometry2D.is_point_in_polygon(pt, outline): inside = false
            pts.append(pt)
        if not inside: continue
        var fade: float = 1.0 - float(k) / float(nb) * 0.7
        ci.draw_polyline(pts, Color(1.0, 0.96, 0.82, 0.07 * strength * fade), 6.0, true)
        ci.draw_polyline(pts, Color(1.0, 1.0, 0.92, 0.10 * strength * fade), 1.5, true)


## GPU kapalina: shader počítá absorpci světla s hloubkou (Beer–Lambert), válcové nasvícení, kaustiku, mikročástice (suspenzi)
## a rozptyl zákalu. Animuje se na grafické kartě (TIME), takže plyne plynule bez zatížení procesoru.
const LIQUID_SHADER := """
shader_type canvas_item;
uniform vec4 col_light : source_color = vec4(0.9, 0.7, 0.3, 1.0);
uniform vec4 col_deep : source_color = vec4(0.6, 0.35, 0.1, 1.0);
uniform vec2 box_pos = vec2(0.0);
uniform vec2 box_size = vec2(100.0);
uniform float surf_y = 0.0;
uniform float depth_px = 300.0;
uniform float turb = 0.0;
uniform float stir = 0.0;
uniform float boil = 0.0;
uniform float alpha = 0.9;
uniform float absorb = 2.2;
varying vec2 wp;
void vertex() { wp = VERTEX; }
float hash(vec2 p) { p = fract(p * vec2(123.34, 456.21)); p += dot(p, p + 45.32); return fract(p.x * p.y); }
vec2 hash2(vec2 p) { return vec2(hash(p), hash(p + 17.13)); }
float vnoise(vec2 p) {
    vec2 i = floor(p); vec2 f = fract(p); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash(i), hash(i + vec2(1, 0)), f.x), mix(hash(i + vec2(0, 1)), hash(i + vec2(1, 1)), f.x), f.y);
}
float ridge(vec2 p) { float n = vnoise(p) * 0.6 + vnoise(p * 2.1 + 7.3) * 0.4; return pow(1.0 - abs(n * 2.0 - 1.0), 7.0); }
float specks(vec2 p, float scale, float dens, float t, float flow) {
    vec2 q = p / scale;
    // proudění: konvekční buňky při míchání/varu, jinak pomalé stoupání/klesání
    q += vec2(sin(p.y * 0.021 + t * 0.7), cos(p.x * 0.017 - t * 0.6)) * (0.5 * flow);
    q.y += t * (0.04 + 0.5 * flow * sin(p.x * 0.013));
    vec2 id = floor(q); vec2 f = fract(q);
    vec2 r = hash2(id);
    if (r.x > dens) return 0.0;
    vec2 c = 0.5 + 0.3 * vec2(sin(t * (0.3 + r.y) + r.y * 6.28), cos(t * (0.4 + r.x) + r.x * 6.28));
    float d = length(f - c);
    return smoothstep(0.17, 0.0, d) * (0.5 + 0.5 * r.y);
}
void fragment() {
    float t = TIME;
    float depth = clamp((wp.y - surf_y) / max(depth_px, 1.0), 0.0, 1.0);
    float k = 1.0 - exp(-absorb * depth);
    vec3 c = mix(col_light.rgb, col_deep.rgb, k);
    float sx = clamp((wp.x - box_pos.x) / box_size.x, 0.0, 1.0);
    float edge = abs(sx * 2.0 - 1.0);
    float shade = 1.0 - 0.38 * pow(edge, 3.0) + 0.10 * exp(-pow((sx - 0.30) * 6.0, 2.0));
    c *= shade;
    // zákal: mléčný rozptyl světla
    c = mix(c, vec3(0.90, 0.80, 0.58) * shade, turb * 0.42);
    // kaustika (odlesky světla lomeného na hladině), slábne s hloubkou
    float fl = clamp(stir + boil, 0.0, 1.5);
    vec2 cp = wp * 0.036 + vec2(t * (0.05 + 0.10 * fl), t * 0.03);
    float ca = ridge(cp) * 0.7 + ridge(cp * 1.9 + vec2(-t * 0.07, 3.1)) * 0.5;
    c += vec3(1.0, 0.96, 0.85) * ca * 0.16 * (1.0 - 0.75 * k) * (1.0 - 0.6 * turb);
    // tenký světlý pás pod hladinou (lom světla, meniscus)
    float sd = wp.y - surf_y;
    c += vec3(1.0, 0.97, 0.88) * exp(-sd / 7.0) * 0.16;
    // mikročástice: dvě měřítka
    float flow = clamp(stir + boil * 1.2, 0.0, 1.5);
    float sp = specks(wp, 11.0, 0.20 + 0.55 * turb, t, flow) * 0.55 + specks(wp + 31.0, 19.0, 0.10 + 0.35 * turb, t * 0.8, flow) * 0.7;
    c = mix(c, vec3(1.0, 0.95, 0.78), clamp(sp, 0.0, 1.0) * (0.22 + 0.5 * turb));
    // bubliny při varu: světlé tečky stoupající ode dna
    if (boil > 0.05) {
        vec2 bq = vec2(wp.x / 14.0, wp.y / 18.0 + t * 1.4);
        vec2 bid = floor(bq); vec2 bf = fract(bq); vec2 br = hash2(bid);
        float bd = length(bf - (0.2 + 0.6 * br));
        float bub = smoothstep(0.16 + 0.08 * br.x, 0.05, bd) * step(br.y, 0.35 * boil);
        c += vec3(1.0) * bub * 0.25;
    }
    COLOR = vec4(c, alpha);
}
"""

class Gpu extends Node2D:
    var polys: Array = []
    var mat: ShaderMaterial
    func _init() -> void:
        var sh: = Shader.new();sh.code = LIQUID_SHADER
        mat = ShaderMaterial.new();mat.shader = sh
        material = mat
        show_behind_parent = true
    func set_params(col: Color, box: Rect2, surf_y: float, depth_px: float, turb: float, stir: float, boil: float) -> void:
        var cl: Color = col
        var cd: = Color(pow(col.r, 1.8) * 0.92, pow(col.g, 1.8) * 0.92, pow(col.b, 1.8) * 0.92, 1.0)
        mat.set_shader_parameter("col_light", Color(minf(1.0, cl.r * 1.12 + 0.03), minf(1.0, cl.g * 1.10 + 0.03), minf(1.0, cl.b * 1.06 + 0.02), 1.0))
        mat.set_shader_parameter("col_deep", cd)
        mat.set_shader_parameter("box_pos", box.position)
        mat.set_shader_parameter("box_size", box.size)
        mat.set_shader_parameter("surf_y", surf_y)
        mat.set_shader_parameter("depth_px", depth_px)
        mat.set_shader_parameter("turb", clampf(turb, 0.0, 1.0))
        mat.set_shader_parameter("stir", stir)
        mat.set_shader_parameter("boil", boil)
        mat.set_shader_parameter("alpha", col.a)
    func set_polys(p: Array) -> void:
        polys = p
        queue_redraw()
    func _draw() -> void:
        for q in polys:
            if q.size() >= 3: draw_colored_polygon(q, Color.WHITE)
