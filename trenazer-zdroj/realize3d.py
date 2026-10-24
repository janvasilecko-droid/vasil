# Převod 2D plánku z webové aplikace na 3D trasy podle fotek varny:
# - trubky mezi nádobami a nad nimi vedou v ose nádob (z = 0), stoupačky vedle nádob
# - vodorovné trubky pod nádobami vedou za zadními nohami (z = -(0.55 r + 0.25))
# - úseky, které by na plánku prošly nádobou, jdou za ní u zdi (z = -(r + 0.3))
# - kde se mění hloubka, je koleno (svislý bod se stejným x,y)
import json, sys, math
src, dst = sys.argv[1], sys.argv[2]
d = json.load(open(src))
items = d['items']
V = []
for v in items:
    if v['type'] != 'vessel' or v.get('shape', '') in ('platform', 'plate', 'inlet', 'source', 'drain', 'box', 'chiller'): continue
    r = v['w']/200.0 if v.get('shape') != 'horiz' else v['h']/200.0
    V.append(dict(x0=v['x']+14, y0=v['y']+14, x1=v['x']+v['w']-14, y1=v['y']+v['h']-14, X0=v['x'], X1=v['x']+v['w'], Y1=v['y']+v['h'], r=r, horiz=v.get('shape') == 'horiz', id=v['id']))
def clip_len(a, b, R):
    # délka části úseku uvnitř obdélníku (Liang-Barsky)
    x0, y0 = a; dx, dy = b[0]-a[0], b[1]-a[1]; t0, t1 = 0.0, 1.0
    for p, q in ((-dx, x0-R['x0']), (dx, R['x1']-x0), (-dy, y0-R['y0']), (dy, R['y1']-y0)):
        if p == 0:
            if q < 0: return 0.0
        else:
            t = q/p
            if p < 0: t0 = max(t0, t)
            else: t1 = min(t1, t)
    if t1 <= t0: return 0.0
    return (t1-t0)*math.hypot(dx, dy)
def inside(p, R): return R['x0'] < p[0] < R['x1'] and R['y0'] < p[1] < R['y1']
def seg_z(a, b, is_first, is_last):
    zs = []
    for R in V:
        L = clip_len(a, b, R)
        if L < 20: continue
        # konec trubky v nádobě = hrdlo (jen kus u konce) – ne průchod
        ends_in = (is_first and inside(a, R)) or (is_last and inside(b, R))
        other_out = not (inside(a, R) and inside(b, R))
        if ends_in and other_out:
            # vstup zespodu / shora / zboku – v ose nádoby
            continue
        zs.append(-(R['r'] + 0.3))
    if zs: return round(min(zs), 2)
    # pod nádobou vodorovně
    if abs(b[1]-a[1]) < abs(b[0]-a[0]):
        y = (a[1]+b[1])/2.0; best = None
        for R in V:
            if R['horiz']: continue
            if y > R['Y1'] and min(a[0], b[0]) < R['X1'] and max(a[0], b[0]) > R['X0']:
                z = -(0.55*R['r'] + 0.25)
                best = z if best is None else min(best, z)
        if best is not None: return round(best, 2)
    return 0.0
pipes = [p for p in items if p['type'] == 'pipe' and not p.get('hose') and len(p.get('pts', [])) >= 2]
segz = {}
for p in pipes:
    pts = p['pts']; n = len(pts)
    segz[p['id']] = [seg_z(pts[i], pts[i+1], i == 0, i == n-2) for i in range(n-1)]
# kateta zezadu: hrdla v hloubce katety
mz = 0.0
for m in items:
    if m['type'] == 'manifold': m['z'] = mz
# spoje: kanonická hloubka (podle trubky, která spojem prochází)
def on_seg(q, a, b, tol=3.0):
    dx, dy = b[0]-a[0], b[1]-a[1]; l2 = dx*dx+dy*dy
    if l2 < 1: return -1
    t = ((q[0]-a[0])*dx + (q[1]-a[1])*dy)/l2
    if t <= 1e-6 or t >= 1-1e-6: return -1
    if abs(dx*(q[1]-a[1]) - dy*(q[0]-a[0]))/math.sqrt(l2) > tol: return -1
    return t
canon = {}
for j in d.get('joints', []):
    key = (round(j[0]), round(j[1])); z = None
    for p in pipes:
        for i in range(len(p['pts'])-1):
            if on_seg(j, p['pts'][i], p['pts'][i+1]) > 0: z = segz[p['id']][i]; break
        if z is not None: break
    if z is None:
        for p in pipes:
            for i, q in enumerate(p['pts']):
                if (round(q[0]), round(q[1])) == key:
                    z = segz[p['id']][min(i, len(p['pts'])-2)]; break
            if z is not None: break
    if z is not None: canon[key] = z
for m in items:
    if m['type'] == 'manifold':
        g = m.get('gap', 60); R_ = int(m.get('rows', 3)); C = int(m.get('cols', 4))
# nové body
for p in pipes:
    pts = p['pts']; zs = segz[p['id']]; out = []
    def add(q):
        if not out or out[-1] != q: out.append(q)
    for i, q in enumerate(pts):
        x, y = int(round(q[0])), int(round(q[1])); key = (x, y)
        zin = zs[i-1] if i > 0 else zs[0]; zout = zs[i] if i < len(zs) else zs[-1]
        zc = canon.get(key)
        if i == 0 and zc is not None and zc != zout: add([x, y, zc])
        add([x, y, zin])
        if zc is not None and 0 < i < len(pts)-1 and zc not in (zin, zout): add([x, y, zc])
        if zout != zin: add([x, y, zout])
        if i == len(pts)-1 and zc is not None and zc != zin: add([x, y, zc])
    p['pts'] = out
d['plan_ver'] = 5
d['real3d'] = True
json.dump(d, open(dst, 'w'), ensure_ascii=False)
import collections
c = collections.Counter(z for p in pipes for z in segz[p['id']])
print('hloubky úseků:', sorted(c.items()))
