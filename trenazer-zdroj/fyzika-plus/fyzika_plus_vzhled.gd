extends RefCounted
## FYZIKA+ vzhled – kapalina, která se na plánku chová jako kapalina (volitelné háčky V1 a V2 v main.gd).
##  V1: proudy do nádob padají po balistické dráze (sprcha stříká do oblouků, dopadá na stěnu a stéká po ní,
##      boční nátok vytéká obloukem) místo rovných čar
##  V2: hladina se v místě dopadu proudu vlní – kruhy se šíří od dopadu a slábnou; rmut je vazký, vlní se málo a krátce
## Nic z toho nemění výpočet (sim.gd), jen kreslení. Měřítko: plánek je schéma, 1 m ≈ 100 px (PX_PER_M).

const G_PX := 981.0          # tíhové zrychlení [px/s²] při 100 px/m
const V_SPRAY := 230.0       # výtoková rychlost ze sprchové hlavice [px/s] ≈ 2,3 m/s (ODHAD pro kreslení)
const V_SIDE := 160.0        # výtoková rychlost z bočního hrdla [px/s] ≈ 1,6 m/s (ODHAD pro kreslení)

## Balistická dráha kapky z bodu o rychlostí vel: konec na hladině (surf), na stěně (xl/xr) nebo po t_max.
## Vrací [body, kde_skoncila]: 0 = hladina, 1 = stěna.
static func _arc(o: Vector2, vel: Vector2, surf: float, xl: float, xr: float, top: float) -> Array:
	var pts := PackedVector2Array([o])
	var p := o; var v := vel; var end := 0
	var dt := 0.03
	for i in 80:
		v.y += G_PX * dt
		var q := p + v * dt
		if q.y < top:                      # strop nádoby: kapka se odrazí dolů
			q.y = top; v.y = absf(v.y) * 0.2
		if q.x <= xl or q.x >= xr:
			var dx: float = q.x - p.x
			var t: float = 0.0
			if absf(dx) > 1e-6: t = (xl - p.x) / dx if q.x <= xl else (xr - p.x) / dx
			q = p + (q - p) * clampf(t, 0.0, 1.0)
			if q.y < surf: pts.append(q)
			end = 1; p = q; break
		if q.y >= surf:
			var t2: float = (surf - p.y) / maxf(1e-6, q.y - p.y)
			q = p + (q - p) * clampf(t2, 0.0, 1.0)
			pts.append(q); p = q; end = 0; break
		pts.append(q); p = q
	return [pts, end]

## Náhrada za _stream_segs() v main.gd: stejné výstupní pole [začátek, konec, barva, šířka, šplíchnutí] jako dřív,
## jen proud je složený z krátkých úseků po balistické dráze. 6. prvek = síla dopadu na hladinu (pro vlnky, V2).
static func stream_segs(m) -> Array:
	var S = m.sim.S; var net = m.sim.net; var out := []
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
		var xl: float = v.x + 9.0; var xr: float = v.x + v.w - 9.0; var top: float = v.y + 6.0
		if a.spray != null:
			var sp = net.byId[a.spray]; var o := Vector2(sp.x, sp.y)
			# sprchová hlavice stříká do všech stran: část paprsků dopadne na stěnu a stéká po ní, část rovnou na hladinu
			for ang_d in [-30.0, 0.0, 30.0, 60.0, 90.0, 120.0, 150.0, 180.0, 210.0]:
				var ang: float = deg_to_rad(ang_d)
				var R: Array = _arc(o, Vector2(cos(ang), sin(ang)) * V_SPRAY, surf, xl, xr, top)
				var pts: PackedVector2Array = R[0]
				var step: int = maxi(1, int(ceil((pts.size() - 1) / 6.0)))
				var i := 0
				while i < pts.size() - 1:
					var j: int = mini(pts.size() - 1, i + step)
					var last: bool = j == pts.size() - 1
					out.append([_r(pts[i]), _r(pts[j]), col, 2.0, false, 0.4 if (last and R[1] == 0) else 0.0])
					i = j
				if R[1] == 1:
					var e: Vector2 = pts[pts.size() - 1]
					if e.y < surf - 4.0: out.append([_r(e), _r(Vector2(e.x, surf)), col, 1.4, false, 0.2])   # film stékající po stěně
		else:
			var x0 := clampf(float(n.x), v.x + 14, v.x + v.w - 14); var y1: float = maxf(n.y, v.y)
			var side: float = 0.0
			if float(n.x) <= v.x + 14.0 and y1 > v.y + 12.0: side = 1.0          # hrdlo v levé stěně
			elif float(n.x) >= v.x + v.w - 14.0 and y1 > v.y + 12.0: side = -1.0   # hrdlo v pravé stěně
			if side == 0.0 or surf - y1 < 12.0:
				out.append([Vector2(x0, y1), Vector2(x0, surf), col, 7.0, true, 1.0])
			else:
				var R2: Array = _arc(Vector2(x0, y1), Vector2(side * V_SIDE, 0.0), surf, xl, xr, top)
				var pts2: PackedVector2Array = R2[0]
				var step2: int = maxi(1, int(ceil((pts2.size() - 1) / 5.0)))
				var i2 := 0
				while i2 < pts2.size() - 1:
					var j2: int = mini(pts2.size() - 1, i2 + step2)
					var last2: bool = j2 == pts2.size() - 1
					out.append([_r(pts2[i2]), _r(pts2[j2]), col, 7.0, last2 and R2[1] == 0, 1.0 if (last2 and R2[1] == 0) else 0.0])
					i2 = j2
	return out

static func _r(p: Vector2) -> Vector2:
	return Vector2(roundf(p.x), roundf(p.y))

## Místa dopadu proudů na hladinu nádoby it: pole [x, síla]. Uloží se do fxs["fpimp"]; když něco dopadá,
## drží překreslování hladiny (stejným příznakem jako houpání).
static func impacts(m, it, fxs: Dictionary, lv: float) -> Array:
	var imp := []
	for s in stream_segs(m):
		if s.size() < 6 or float(s[5]) <= 0.0: continue
		var e: Vector2 = s[1]
		if e.x < it.x - 2.0 or e.x > it.x + it.w + 2.0: continue
		if absf(e.y - lv) > 14.0: continue
		imp.append([e.x, float(s[5])])
	if imp.size() > 0: fxs["slosh"] = maxf(float(fxs.get("slosh", 0.0)), 0.15)
	return imp

## Výška vlnky [px] v místě px: od každého dopadu se šíří tlumená vlna (kruhy na vodě).
## Voda: vlnová délka ~30 px, dosah ~110 px. Rmut je vazký: třetinová výška, krátký dosah, pomalejší.
static func ripple(fxs: Dictionary, px: float, tt: float, mash: bool) -> float:
	var imp = fxs.get("fpimp", null)
	if imp == null or not (imp is Array) or imp.is_empty(): return 0.0
	var A: float = 0.9 if mash else 2.6
	var L: float = 40.0 if mash else 110.0
	var kx: float = 0.16 if mash else 0.21
	var om: float = 5.0 if mash else 9.0
	var h := 0.0
	for q in imp:
		var d: float = absf(px - float(q[0]))
		h += A * float(q[1]) * exp(-d / L) * sin(kx * d - om * tt)
	return clampf(h, -5.0, 5.0)
