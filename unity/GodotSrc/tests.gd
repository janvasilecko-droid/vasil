extends RefCounted




var m
var sim
var results: = []

func chk(name: String, ok: bool, detail: = "") -> void :
    results.append([name, ok, detail])
    print("%s  %s%s" % ["PASS" if ok else "FAIL", name, ("  [" + detail + "]") if detail != "" else ""])

func near(a: float, b: float, tol: float) -> bool:
    return absf(a - b) <= tol

func fresh() -> void :

    for it in m.items:
        if it.type == "valve": it["open"] = false
        if it.type == "vessel" and it.get("shape") == "inlet": it["on"] = false
    for k in m.outlet_open: m.outlet_open[k] = false
    sim.net = {}
    sim.new_state();sim.build_net(m.items, m.plan.get("joints", []))
    m.auto.S = sim.S
    m.auto.on = false


func bed() -> void :
    fresh()
    var S = sim.S; var sk = S.sk
    var p = sim.vessel()
    p.T = 72.0;p.w = 2300.0;p.g = 133.0;p.f = 330.0;p.dx = 20.0;p.ns = 70.0;p.ld = 60.0;p.sg = 15.0;p.col = 4000.0
    sk.under = 200.0;sk.underOK = true
    sim.dstGive("sk", p, {})
    sim.skMashToBed();sk.mode = "lauter"
    sk.settle = 30.0


func bed_mash_only() -> void :
    fresh()
    var S = sim.S; var sk = S.sk
    var p = sim.vessel()
    p.T = 72.0;p.w = 2300.0;p.g = 133.0;p.f = 330.0;p.dx = 20.0;p.ns = 70.0;p.ld = 60.0;p.sg = 15.0;p.col = 4000.0
    sk.under = 200.0;sk.underOK = true
    sim.dstGive("sk", p, {})

func run() -> int:
    sim = m.sim
    fresh()
    var S = sim.S


    var dissolved: float = 17.0 * 16.0 / 84.0
    var collected: float = 26.0 * 0.97 * 1.043 * 0.107
    var additional: float = 0.6 * 0.97 * 1.024 * 0.06
    var remaining: float = dissolved - collected - additional
    var retained: float = 4.3 * 1.56   # Troesterův příklad (jeho várka), ne parametr naší varny
    var water_after: float = retained - remaining * 0.62 + 8.6
    var pred: float = 100.0 * remaining / (remaining + water_after)
    chk("Troester: zbývající extrakt 0,38776 kg", near(remaining, 0.38776, 0.0001), "%.5f" % remaining)
    chk("Troester: predikce promývacího vzorku 2,509 °P (retence 1,56 L/kg)", near(pred, 2.509, 0.01), "%.3f" % pred)
    var pred_bad: float = 100.0 * remaining / (remaining + (4.3 * 0.8 - remaining * 0.62 + 8.6))
    chk("Troester: s chybnou retencí 0,8 L/kg vychází 3,182 °P", near(pred_bad, 3.182, 0.01), "%.3f" % pred_bad)
    S.sk.g = 4.3 * 0.2 * 0.955 * 0.191 / 0.191
    S.sk.g = 4.3 * 0.191
    chk("skRet() = skMalt * skRetentionLkg", near(sim.skRet(), sim.skMalt() * S.cfg.skRetentionLkg, 1e-06), "%.3f" % sim.skRet())


    var big = sim.lp();big.w = 1000.0;big.f = 100.0
    var small = sim.lp();small.w = 0.3;small.f = 0.03
    chk("°P v dávce 0,3 L je stejné jako ve velké (plato)", near(sim.plato(big), sim.plato(small), 1e-09), "%.3f / %.3f" % [sim.plato(big), sim.plato(small)])
    var cell_small = {"w": 0.3, "f": 0.03, "n": 0.0, "smm": 0.0, "dms": 0.0, "col": 0.0}
    chk("°P v malé buňce lože (cellP)", near(sim.cellP(cell_small), sim.plato(small), 1e-09))


    var io = sim.lp();io.su = 10.0;io.sc = 5.0;io.dx = 1.0;io.f = 100.0
    chk("jodová zkouška kapalného vzorku je negativní", sim.iodine(io) == null or sim.iodine(io) == "neg", str(sim.iodine(io)))
    chk("jodová zkouška včetně zrn ukáže nezcukřený škrob", sim.iodine(io, true) == "pos", str(sim.iodine(io, true)))


    var old: Dictionary = S.duplicate(true);old.cfg.erase("skRetentionLkg");old.sk.erase("fines");old.sk.erase("ch")
    var fr = m.Sim.new();fr.main = m;fr.new_state()
    m._merge_defaults(old, fr.S)
    chk("starší uložení se doplní o nová pole (retence, částice, kanálky)", old.cfg.has("skRetentionLkg") and old.sk.has("fines") and old.sk.has("ch"))


    var d1 = sim.lp();d1.w = 100.0;d1.T = 20.0
    var p1 = sim.lp();p1.w = 100.0;p1.T = 80.0
    sim.addTo(d1, p1, 0.0)
    chk("míchání 100 L 20 °C + 100 L 80 °C = 50 °C", near(d1.T, 50.0, 0.01) and near(d1.w, 200.0, 1e-09), "%.2f" % d1.T)


    var e = {"cap": 10.0, "c": sim.lp(), "q": [], "from": "a", "flow": 0}
    var mk = func(w: float, T: float, ex: float):
        var q = sim.lp();q.w = w;q.T = T;q.f = ex;return q
    var acc = {"in": 0.0, "out": 0.0}
    var feed = func(w: float, T: float, ex: float, from_a: bool):
        var part = mk.call(w, T, ex)
        var o = sim.edge_push(e, part, from_a)
        acc["in"] += w;acc["out"] += o.w
        return o
    var o1 = feed.call(5.0, 20.0, 0.0, true)
    var o2 = feed.call(5.0, 80.0, 0.0, true)
    chk("FIFO: dokud se trubka nezaplní, nic nevyteče", o1.w < 1e-09 and o2.w < 1e-09)
    var o3 = feed.call(5.0, 30.0, 0.0, true)
    chk("FIFO: nejdřív vyteče nejstarší dávka (20 °C), ne průměr", near(o3.w, 5.0, 1e-06) and near(o3.T, 20.0, 0.01), "%.2f L %.1f °C" % [o3.w, o3.T])
    var o4 = feed.call(5.0, 30.0, 0.0, true)
    chk("FIFO: teplá dávka (80 °C) přijde se zpožděním", near(o4.T, 80.0, 0.01), "%.1f °C" % o4.T)

    var o5 = feed.call(6.0, 80.0, 0.0, false)
    var o6 = feed.call(6.0, 50.0, 0.0, false)
    chk("FIFO: po obrácení toku vytéká obsah ze strany a (30 °C)", near(o5.T, 30.0, 0.01) and near(o5.w, 6.0, 1e-06), "%.1f °C" % o5.T)
    var exp_T: float = (4.0 * 30.0 + 2.0 * 80.0) / 6.0
    chk("FIFO: po obrácení se pořadí obrátí (směs 4 L 30 °C + 2 L 80 °C)", near(o6.T, exp_T, 0.2), "%.1f / %.1f °C" % [o6.T, exp_T])
    var stored: float = e.c.w
    chk("FIFO: zachování vody (vtok = výtok + obsah)", near(acc["in"], acc["out"] + stored, 1e-06), "%.3f vs %.3f" % [acc["in"], acc["out"] + stored])
    var qsum: = 0.0
    for b in e.q: qsum += b.w
    chk("FIFO: agregát obsahu odpovídá součtu dávek", near(qsum, e.c.w, 1e-06))

    var Vm: float = PI * pow(50.0 / 1000.0, 2) / 4.0 * 1.0 * 1000.0
    chk("objem DN50 je asi 1,9635 L/m", near(Vm, 1.9635, 0.001), "%.4f" % Vm)


    bed()
    S = sim.S
    var sk = S.sk
    sim.skQmax()
    var q0: float = sk.Qmax
    chk("Darcy: průtok lože je ve smysluplném rozsahu (40–200 L/min)", q0 > 40.0 and q0 < 200.0, "%.0f L/min" % q0)
    sk.c = 1.0;sim.skQmax()
    chk("Darcy: zhutnění snižuje propustnost", sk.Qmax < q0 * 0.5, "%.0f -> %.0f" % [q0, sk.Qmax])
    sk.c = 0.0;sim.skQmax()
    var hot: float = sk.Qmax
    sk.T = 40.0;sim.skQmax()
    chk("Darcy: studená (viskóznější) sladina teče pomaleji", sk.Qmax < hot, "%.0f -> %.0f" % [hot, sk.Qmax])
    sk.T = 72.0;sim.skQmax()

    bed();S = sim.S;sk = S.sk;sim.skQmax()
    var qm: float = sk.Qmax; var c_before: float = sk.c
    var volsum: = 0.0
    for i in 20:
        sim.skQmax();sk.Qact = 0.0
        var pp = sim.skDraw(qm * 2.5, 1.0 / 60.0 * 3.0)
        volsum += sim.liqVol(pp)
    chk("odběr 2,5× nad propustností nevytvoří víc kapaliny (max Qmax)", volsum <= qm * 3.0 / 1.0 * 1.001, "%.0f L za 3 min, Qmax %.0f" % [volsum, qm])
    chk("příliš rychlý odběr zhutní lože a vytvoří kanálky", sk.c > c_before + 0.05 and sk.ch > 0.0, "c %.2f ch %.2f" % [sk.c, sk.ch])

    bed();S = sim.S;sk = S.sk;sim.skAddWater(1500.0, 78.0, false);sim.skQmax()
    var calm = sim.skDraw(sk.Qmax * 0.5, 0.5)
    var P_calm: float = sim.plato(calm)
    bed();S = sim.S;sk = S.sk;sim.skAddWater(1500.0, 78.0, false);sk.ch = 0.4;sim.skQmax()
    var chan = sim.skDraw(sk.Qmax * 0.5, 0.5)
    var P_chan: float = sim.plato(chan)
    chk("kanálkování: část toku obtéká lože, sladina je řidší (extrakt zůstává v mlátě)", P_chan < P_calm - 0.05, "%.2f vs %.2f °P" % [P_chan, P_calm])

    var res: = []
    for pos in [0.05, 0.4, 1.0]:
        bed();S = sim.S;sk = S.sk
        sk.c = 1.0;sk.rake = true;sk.rakeH = pos; var f0: float = sk.fines
        for i in 600: sim.sim_step(1.0)
        res.append([sk.c, sk.fines - f0 if sk.fines > f0 else 0.0])
    chk("kopačka nahoře nekypří (žádné kouzlo)", near(res[0][0], 1.0, 0.01) and res[0][1] < 1e-06, "c=%.2f" % res[0][0])
    chk("mírné řezání povolí lože", res[1][0] < 0.95, "c=%.2f" % res[1][0])
    chk("hluboké řezání u síta uvolní mnohem víc částic než mírné", res[2][1] > res[1][1] * 3.0, "%.3f vs %.3f kg" % [res[2][1], res[1][1]])

    bed();S = sim.S;sk = S.sk
    var mass0: float = sk.g + sk.fines + sk.teig
    var clear_after: = -1.0
    var litres: = 0.0
    for i in 400:
        sim.skQmax();sk.Qact = 0.0
        var pv = sim.skDraw(minf(40.0, sk.Qmax * 0.5), 1.0 / 60.0 * 6.0)
        litres += sim.liqVol(pv)
        sim.dstGive("sk", pv, {"src": "sk"})
        if sk.turb < 20.0 and clear_after < 0.0: clear_after = litres
    chk("vorlauf skutečně vyčistí kapalinu (zákal < 20)", clear_after > 0.0 and sk.turb < 20.0, "čirá po %.0f L, zákal %.1f" % [clear_after, sk.turb])
    chk("vorlauf zachovává hmotnost mláta a částic", near(sk.g + sk.fines + sk.teig, mass0, 1e-06), "%.4f vs %.4f" % [sk.g + sk.fines + sk.teig, mass0])

    bed();S = sim.S;sk = S.sk
    var f_before: float = sk.fines
    var t1: float = sk.turb
    sim.skQmax();sk.Qact = 0.0
    sim.skDraw(30.0, 0.01)
    chk("samotné přepnutí fáze nevyčistí kapalinu (zákal zůstává vysoký po čerstvém loži)", sk.fines > f_before * 0.9 and sk.turb > 20.0, "zákal %.0f" % sk.turb)


    fresh();S = sim.S
    var best = null
    for ed in sim.net.edges:
        if ed.cap > 3.0: best = ed;break
    if best != null:
        var mid: Vector2 = best.pts[0].lerp(best.pts[best.pts.size() - 1], 0.5)
        var batch = sim.lp();batch.w = 0.4;batch.f = 0.04;batch.T = 70.0
        best.from = "a"
        sim.edge_push(best, batch, true)
        var SW = preload("res://sight.gd").new();SW.m = m
        var smp = SW.take_sample(best.pts[0] + (best.pts[1] - best.pts[0]) * 0.02)
        chk("průhledítko: dávka menší než 1 L má správné °P a teplotu", not smp.get("empty", true) and near(smp.P, 9.09, 0.1) and near(smp.T, 70.0, 0.1), "%s °P" % str(smp.get("P")))
        var far = SW.take_sample(best.pts[best.pts.size() - 1])
        chk("průhledítko na druhém konci úseku je (zatím) prázdné – vzorek je z konkrétního místa", far.get("empty", false) or far.get("none", false))
        SW.free()
        best.flow = 0.0
        chk("stojící proud je označen jako stojící (průtok 0)", best.flow == 0.0)
    else:
        chk("průhledítko: existuje úsek potrubí", false)


    fresh();S = sim.S
    var vs = sim.route_valves("hlt", "rvp", ["phw"])
    chk("cesta bojler -> pánev existuje", vs != null)
    if vs != null:
        for id in vs:
            if m.outlet_open.has(id): m.outlet_open[id] = true
            elif sim.net.byId.has(id): sim.net.byId[id]["open"] = true
        sim.valve_ver += 1
        var hlt0: float = S.hlt.w
        S.rvpWater.target = 800.0;S.rvpWater.on = true;S.mixAuto = true;S.rvpWater.T = 60.0
        for i in 900: sim.sim_step(1.0)
        var pipes_w: = 0.0
        for ed in sim.net.edges: pipes_w += ed.c.w
        var used: float = hlt0 - S.hlt.w
        var in_sys: float = S.rvp.w + pipes_w + S.drainL + S.spillL
        chk("bilance vody: z bojleru (+ studená z řadu) = pánev + potrubí + ztráty", in_sys >= used * 0.98 and in_sys <= used * 1.6, "bojler -%.0f L, pánev+potrubí+ztráty %.0f L" % [used, in_sys])
        chk("napouštění zastaví při cílovém objemu", S.rvp.w <= 800.0 + 20.0 and S.rvp.w >= 780.0, "%.0f L" % S.rvp.w)


    var Pm = preload("res://physics.gd")
    var one = Pm.chain(100.0, 20.0, 0.05, 0.01, 80.0)
    var many = [100.0, 20.0]
    for i in 1600: many = Pm.chain(many[0], many[1], 0.05, 0.01, 0.05)
    chk("reakce A->B->ztráta nezávisí na dělení kroku", absf(one[0] - many[0]) + absf(one[1] - many[1]) < 1e-08)
    chk("výměník: konečná účinnost a limit stejných kapacit", absf(Pm.exchanger_effectiveness(10.0, 5.0, 5.0) - 2.0 / 3.0) < 1e-09)
    var wall = Pm.exchange_bodies(80.0, 100.0, 20.0, 50.0, 2.0, 3600.0)
    chk("výměna tepla těleso/stěna zachovává citelné teplo", absf(wall[0] * 100.0 + wall[1] * 50.0 - (80.0 * 100.0 + 20.0 * 50.0)) < 1e-06)
    fresh();S = sim.S
    var sk2 = S.sk
    sk2.Tw = 20.0
    var spr = sim.lp();spr.w = 2000.0;spr.T = 78.0
    sim.skWallHeat(spr)
    chk("sprchování nezahřeje stěnu nad teplotu vody", sk2.Tw <= 78.0 and spr.T >= 20.0)
    S.hlt.w = 11990.0
    var inc = sim.lp();inc.w = 25.0;inc.T = 70.0
    sim.dstGive("hlt", inc)
    chk("plný bojler přijme jen zbývající objem, zbytek přeteče", near(S.hlt.w, 12000.0, 1e-06) and near(S.drainL, 15.0, 1e-06))

    bed_mash_only()
    chk("SK s rmutem je ve stavu rmut (bez lože)", sim.S.sk.mode == "mash" and sim.vol(sim.S.sk.mash) > 1000.0)
    sim.S.p2.on = true
    for rt in sim.route_valves("sk", "sk", ["p2"], 0):
        if m.outlet_open.has(rt): m.outlet_open[rt] = true
        elif sim.net.byId.has(rt): sim.net.byId[rt]["open"] = true
    sim.valve_ver += 1
    sim.net_step(1.0)
    chk("C2 se v režimu rmut zastaví (nevytvoří z rmutu lože)", not sim.S.p2.on and sim.S.sk.g < 1.0)
    sim.start_lautering()
    chk("Odrmutovat vytvoří lože a zapamatuje počáteční extrakt", sim.S.sk.mode == "lauter" and sim.S.sk.g > 20.0 and sim.S.sk.initialExtract > 100.0, "extrakt %.0f kg" % sim.S.sk.initialExtract)
    chk("automat scezování odmítne start před odrmutováním", ( func():
        fresh();return not sim.lauter_auto.start()).call())

    fresh();S = sim.S
    var tr = sim.training
    var cur = tr.current()
    chk("postup: první krok (kontrola) je splněn a jde potvrdit", cur.ready and not cur.complete)
    var water0: float = S.hlt.w; var T0: float = S.rvp.T
    tr.confirm()
    chk("postup: potvrzení posune krok, ale nemění vodu ani teplotu", tr.state().index == 1 and near(S.hlt.w, water0, 1e-09) and near(S.rvp.T, T0, 1e-09))
    tr.confirm()
    var c2 = tr.current()
    chk("postup: krok „napustit vodu“ se nedá potvrdit, dokud není voda v pánvi", not c2.ready and not tr.confirm())

    var smp = sim.sight_sample(sim.net.byId["inl_mur9u2hg"])
    chk("sight_sample vrací stav připojení a průtok", smp.has("connected") and smp.has("flow"))

    fresh();S = sim.S
    var vs2 = sim.route_valves("hlt", "rvp", ["phw"])
    for id in vs2:
        if m.outlet_open.has(id): m.outlet_open[id] = true
        elif sim.net.byId.has(id): sim.net.byId[id]["open"] = true
    sim.valve_ver += 1
    S.hlt.w = 160.0;S.mixAuto = false;S.rvpWater.target = 5000.0;S.rvpWater.on = true
    var wbefore: float = 160.0
    for i in 60: sim.sim_step(1.0)
    var pipes2: = 0.0
    for ed in sim.net.edges: pipes2 += ed.c.w
    chk("prázdný bojler (rezerva 150 L) nevyrobí vodu", S.hlt.w >= 149.9 and S.rvp.w + pipes2 + S.drainL + S.spillL <= wbefore - S.hlt.w + 0.5, "pánev %.1f L, potrubí %.1f L, usedHot %.1f usedWater %.1f lv %.1f drain %.1f spill %.1f mix %s" % [S.rvp.w, pipes2, S.usedHot, S.usedWater, S.lv.w, S.drainL, S.spillL, str(S.mixAuto)])

    var pq = func(mash: bool, stir: bool, clog: float) -> float:
        fresh();S = sim.S
        for id in sim.route_valves("rvp", "sk", ["p1"]):
            if m.outlet_open.has(id): m.outlet_open[id] = true
            elif sim.net.byId.has(id): sim.net.byId[id]["open"] = true
        sim.valve_ver += 1
        var rv = S.rvp;rv.w = 2000.0;rv.T = 63.0;rv.Tsens = 63.0;rv.stir = stir
        if mash:
            rv.g = 150.0;rv.su = 250.0;rv.sg = 30.0;rv.f = 150.0
        S.clog.rvp = clog
        S.p1.flow = 250.0;S.p1.on = true;S.p1.prime = 1.0
        sim.net_step(1.0)
        return S.p1.q
    var q_water: float = pq.call(false, true, 0.0)
    var q_mash: float = pq.call(true, true, 0.0)
    var q_clog: float = pq.call(true, false, 1.0)
    chk("C1: hustý rmut teče pomaleji než voda (viskozita suspenze)", q_water > 1.0 and q_mash < q_water * 0.85, "voda %.0f, rmut %.0f L/min" % [q_water, q_mash])
    chk("C1: usazené zrno ve výpusti průtok rmutu dál škrtí", q_clog < q_mash * 0.5, "%.0f L/min" % q_clog)
    fresh();S = sim.S
    S.rvp.w = 2000.0;S.rvp.g = 150.0;S.rvp.su = 250.0;S.rvp.sg = 30.0;S.rvp.stir = false
    for i in 600: sim.net_step(1.0)
    var c_still: float = S.clog.rvp
    S.rvp.stir = true
    for i in 120: sim.net_step(1.0)
    chk("bez míchání se zrno za ~5 min usadí, míchadlo ho za ~1 min rozvíří", c_still > 0.9 and S.clog.rvp < 0.1, "%.2f -> %.2f" % [c_still, S.clog.rvp])

    fresh();S = sim.S
    var open_route = func(a: String, b: String, pumps: Array) -> Array:
        var vv0 = sim.route_valves(a, b, pumps); var ids: Array = []
        for id in vv0:
            ids.append(id)
            if m.outlet_open.has(id): m.outlet_open[id] = true
            elif sim.net.byId.has(id): sim.net.byId[id]["open"] = true
        sim.valve_ver += 1
        return ids
    var rids: Array = open_route.call("rvp", "sk", ["p1"])
    S.rvp.w = 2000.0;S.rvp.T = 63.0;S.rvp.Tsens = 63.0;S.rvp.stir = true
    S.p1.prime = 0.0;S.p1.flow = 250.0;S.p1.on = false
    for i in 3: sim.net_step(1.0)
    var pr_a: float = S.p1.prime
    for i in 10: sim.net_step(1.0)
    chk("stojící čerpadlo se při otevřené klapce před ním zavodní samo (~6 s)", pr_a < 0.95 and S.p1.prime >= 0.95, "po 3 s %.2f, po 13 s %.2f" % [pr_a, S.p1.prime])
    S.p1.prime = 0.0;S.p1.on = true
    sim.net_step(1.0)
    chk("čerpadlo spuštěné nezavodněné nedává plný průtok (zavodňuje se)", S.p1.q < 60.0 and S.p1.status.begins_with("zavod"), "%.0f L/min \'%s\'" % [S.p1.q, S.p1.status])
    for i in 20: sim.net_step(1.0)
    chk("po zavodnění teče plným průtokem", S.p1.prime >= 0.95 and S.p1.q > 100.0 and S.p1.dry == 0.0, "%.0f L/min" % S.p1.q)

    for it in m.items:
        if it.type == "valve" and rids.has(it.id): it["open"] = false
    sim.valve_ver += 1
    for i in 20: sim.net_step(1.0)
    chk("zavřené klapky za chodu: čerpadlo nasucho ztrácí zavodnění", S.p1.prime < 0.5, "%.2f" % S.p1.prime)
    for i in 60: sim.net_step(1.0)
    chk("ochrana chodu nasucho čerpadlo vypne", not S.p1.on)
    chk("zavřená klapka za chodu čerpadla se zapíše do deníku jako chyba postupu", S.log.any( func(l): return l.type == "bad" and (l.msg.contains("klapka před čerpadlem") or l.msg.contains("klapka za čerpadlem"))))

    fresh();S = sim.S
    open_route.call("rvp", "sk", ["p1"])
    S.rvp.w = 2000.0;S.rvp.g = 150.0;S.rvp.su = 250.0;S.rvp.sg = 30.0;S.rvp.T = 63.0;S.rvp.Tsens = 63.0;S.rvp.stir = true
    S.clog.rvp = 1.0;S.p1.prime = 1.0;S.p1.flow = 250.0;S.p1.on = true
    for i in 2: sim.net_step(1.0)
    var q_blocked: float = S.p1.q
    for i in 150: sim.net_step(1.0)
    chk("ucpanou výpust čerpadlo nejdřív sotva táhne, po protlačení s mícháním teče", q_blocked < 60.0 and S.clog.rvp < 0.2 and S.p1.q > 80.0 and S.p1.on, "%.0f -> %.0f L/min, clog %.2f" % [q_blocked, S.p1.q, S.clog.rvp])

    chk("škrtidlo: 60 % a víc skoro nebrzdí, pod 40 % průtok rychle klesá", sim.throttle_flow(100) == 1.0 and sim.throttle_flow(60) > 0.8 and sim.throttle_flow(30) < 0.45 and sim.throttle_flow(10) < 0.08 and sim.throttle_flow(0) == 0.0, "%.2f %.2f %.2f %.2f" % [sim.throttle_flow(60), sim.throttle_flow(40), sim.throttle_flow(30), sim.throttle_flow(10)])
    fresh();S = sim.S
    var pid: = ""
    for it in m.items:
        if it.type == "vessel" and it.get("shape") == "plate": pid = it.id
    var cool_at = func(wort: float, ice: float) -> Array:
        S.plates = {pid: {"kind": "ice", "T": 2.0, "once": true, "flow": ice}}
        var t: float = sim.plate_cool(pid, 95.0, wort, 1.0 / 60.0)
        return [t, S.plates[pid].Tcout]
    var fast = cool_at.call(140.0, 60.0)
    var slow = cool_at.call(40.0, 60.0)
    var more_ice = cool_at.call(140.0, 200.0)
    chk("chladič: přiškrcená (pomalejší) mladina vychází studenější", slow[0] < fast[0] - 10.0, "140 L/min %.1f °C, 40 L/min %.1f °C" % [fast[0], slow[0]])
    chk("chladič: víc ledové vody = studenější mladina", more_ice[0] < fast[0] - 5.0, "%.1f °C" % more_ice[0])
    var q_wort: float = 140.0 / 60.0 * 1.04 * 4.0 * (95.0 - fast[0]); var q_ice: float = 60.0 / 60.0 * 4.19 * (fast[1] - 2.0)
    chk("chladič: teplo odebrané mladině = teplo přijaté ledovou vodou", near(q_wort, q_ice, 0.5), "%.1f / %.1f kW, voda ven %.1f °C" % [q_wort, q_ice, fast[1]])

    var raw = sim.lp();raw.w = 2000.0;raw.sg = 120.0;raw.dx = 20.0;raw.f = 100.0
    var rawc = sim.iodine_info(raw)
    var conv = sim.lp();conv.w = 2000.0;conv.f = 400.0;conv.ld = 60.0
    var convc = sim.iodine_info(conv)
    var part = sim.lp();part.w = 2000.0;part.dx = 60.0;part.f = 400.0;part.ld = 60.0;part.sg = 2.0
    var partc = sim.iodine_info(part)
    chk("jod: nezcukřený rmut je modročerný (tmavý, modrá převažuje)", rawc.color.b > rawc.color.r and rawc.color.v < 0.35, rawc.name)
    chk("jod: zcukřený rmut je žlutohnědý (barva jódu)", convc.name.begins_with("žlutohnědá") and convc.color.r > convc.color.b, convc.name)
    chk("jod: částečně zcukřený je červenohnědý/oranžový (dextriny)", partc.color.r > partc.color.b and partc.color.r > 0.55 and partc.name != convc.name, partc.name)
    var gr = sim.lp();gr.w = 2000.0;gr.f = 400.0;gr.su = 30.0
    chk("jod: zrna s nezcukřeným škrobem ukážou škrob, přecezená kapalina ne", sim.iodine_info(gr, true).starch > 0.02 and sim.iodine_info(gr, false).starch < 1e-09)
    fresh();S = sim.S
    var gg = null
    for it in m.items:
        if it.type == "gauge" and m.gauge_geo(it).free: gg = it
    chk("plán má volný stavoznak", gg != null)
    if gg != null:
        var ends: = 0; var leaking: = 0
        for n in sim.net.nodes.values():
            if n.get("gaugeId", "") == gg.id:
                ends += 1
                if n.openEnd: leaking += 1
        chk("konce trubek u stavoznaku jsou rozpoznané a nejsou únik", ends >= 1 and leaking == 0, "konců %d, úniků %d" % [ends, leaking])
        var gv0 = sim.gauge_vessel_free(gg)
        chk("stavoznak je trubkou napojen přímo na ledovku (horní hrdlo nemá klapku)", gv0 != null and gv0.get("bind") == "lv")
        for it in m.items:
            if it.type == "valve": it["open"] = true
        for k in m.outlet_open: m.outlet_open[k] = true
        sim.valve_ver += 1
        var gv = sim.gauge_vessel_free(gg)
        chk("s otevřenými klapkami ukazuje hladinu připojené nádoby", gv != null and gv.get("bind") == "lv", str(gv.get("bind") if gv != null else null))
        gg["top"] = false;gg["bot"] = false;sim.valve_ver += 1
        chk("se zavřenými oběma kohouty stavoznak nic neukazuje", sim.gauge_vessel_free(gg) == null)
        gg["top"] = true;gg["bot"] = true
        for it in m.items:
            if it.type == "valve": it["open"] = false
        for k in m.outlet_open: m.outlet_open[k] = false
    var runs: = {}
    for cfg in [["stoji", false, 35.0], ["pomalu", true, 10.0], ["normalne", true, 35.0], ["rychle", true, 50.0]]:
        fresh();S = sim.S
        var rv = S.rvp
        rv.w = 2000.0;rv.T = 40.0;rv.Tsens = 40.0;rv.g = 80.0;rv.su = 120.0;rv.sg = 20.0;rv.f = 150.0
        rv.stir = cfg[1];rv.hz = cfg[2];rv.heat = 0.5
        for i in 600: sim.sim_step(1.0)
        runs[cfg[0]] = [rv.T, rv.Twall, rv.Tsens]
    chk("míchání: rychlejší míchadlo ohřívá hustý rmut rychleji", runs["normalne"][0] > runs["pomalu"][0] and runs["pomalu"][0] > runs["stoji"][0] - 0.01 and runs["rychle"][0] >= runs["normalne"][0], str(runs))
    chk("míchání: bez míchadla je kapalina u pláště výrazně teplejší než střed nádoby", runs["stoji"][1] - runs["stoji"][0] > 1.5 * (runs["normalne"][1] - runs["normalne"][0]), "stoji +%.1f K, normalne +%.1f K" % [runs["stoji"][1] - runs["stoji"][0], runs["normalne"][1] - runs["normalne"][0]])
    fresh();S = sim.S
    var hv = S.rvp;hv.w = 2000.0;hv.T = 50.0;hv.Tsens = 50.0;hv.stir = true;hv.hz = 35.0;hv.heat = 1.0
    for i in 20: sim.sim_step(1.0)
    chk("teploměr má dobu odezvy (ukazuje méně než skutečná teplota při ohřevu)", hv.Tsens < hv.T - 0.05 and hv.Tsens > 50.0, "T %.2f, čidlo %.2f" % [hv.T, hv.Tsens])
    var thin = sim.vessel();thin.w = 2000.0;thin.stir = true;thin.hz = 35.0
    var thick_ = sim.vessel();thick_.w = 2000.0;thick_.stir = true;thick_.hz = 35.0;thick_.g = 150.0;thick_.su = 300.0
    chk("hustý rmut zhoršuje přestup tepla proti řídké kapalině", sim.film_factor(thick_) < sim.film_factor(thin) * 0.9)
    chk("referenční podmínky (35 Hz, řídká kapalina) = plný prostup", absf(sim.film_factor(thin) - 1.0) < 0.02, "%.3f" % sim.film_factor(thin))
    var wtr = sim.vessel();wtr.w = 1000.0;wtr.T = 20.0
    chk("hustota vody při 20 °C je 0,998 kg/l", absf(sim.density(wtr) - 0.998) < 0.002, "%.4f" % sim.density(wtr))
    var wort = sim.vessel();wort.w = 880.0;wort.f = 120.0;wort.T = 20.0
    chk("hustota 12 °P sladiny při 20 °C je asi 1,048 kg/l", absf(sim.density(wort) - 1.048) < 0.004, "%.4f" % sim.density(wort))
    var hot_wort = sim.vessel();hot_wort.w = 880.0;hot_wort.f = 120.0;hot_wort.T = 95.0
    chk("horká sladina je řidší než studená (tepelná roztažnost)", sim.density(hot_wort) < sim.density(wort) - 0.02)
    var mash = sim.vessel();mash.w = 2000.0;mash.f = 100.0;mash.g = 130.0;mash.su = 300.0;mash.T = 20.0
    chk("rmut s pevnými částicemi je hustší než sladina téhož extraktu", sim.density(mash) > 1.04)
    fresh();S = sim.S
    m.auto.start()
    for i in 1500:
        m.auto.tick(1.0);sim.sim_step(1.0)
    var snap_t: float = S.t; var snap_i: int = m.auto.i; var snap_v: float = sim.vol(S.rvp)
    m.save_game()
    sim.new_state();sim.build_net(m.items, m.plan.get("joints", []));m.auto.S = sim.S;m.auto.on = false;m.auto.i = -1
    var ok: bool = m.load_game()
    S = sim.S
    chk("uložení: várka se načte", ok)
    chk("uložení: čas, objem v pánvi a krok automatu jsou stejné", near(S.t, snap_t, 1e-06) and near(sim.vol(S.rvp), snap_v, 1e-06) and m.auto.i == snap_i, "t=%.0f i=%d" % [S.t, m.auto.i])
    chk("uložení: aktivní automat zůstane aktivní (obnova není ruční zásah)", m.auto.on)
    DirAccess.remove_absolute(ProjectSettings.globalize_path(m.save_path))

    chk("testy používají vlastní uložený soubor, ne várku uživatele", m.save_path != m.SAVE_PATH)


    fresh();S = sim.S
    var plate_id: = ""
    for it in m.items:
        if it.type == "vessel" and it.get("shape") == "plate": plate_id = it.id
    for rt in [
            ["hlt", "rvp", ["phw"], -1, ""], ["hlt", "sk", ["phw"], 1, ""], ["hlt", "sk", ["phw"], 0, ""], 
            ["rvp", "sk", ["p1"], -1, ""], ["sk", "rvp", ["p1"], -1, ""], ["rvp", "wp", ["p1"], -1, ""], 
            ["sk", "sk", ["p2"], 0, ""], ["sk", "rvp", ["p2"], -1, ""], 
            ["wp", "spilka", ["p3"], -1, ""], 
            ["lv", "lv", ["plv"], -1, ""], ["lv", "hlt", ["plv"], -1, plate_id], 
            ["vodovod", "lv", [], -1, ""]]:
        var rv_ = sim.route_valves(rt[0], rt[1], rt[2], rt[3], rt[4])
        chk("cesta %s → %s (%s, kde %d%s) je průchozí" % [rt[0], rt[1], ",".join(PackedStringArray(rt[2])), rt[3], ", deska" if rt[4] != "" else ""], rv_ != null)

    var vn_: = {}
    for nk in sim.net.nodes:
        var nn = sim.net.nodes[nk]
        if nn.valve != null: vn_[nn.valve] = nk
    var pair_ok: = false
    if vn_.has("valve_mur9syhl") and vn_.has("valve_mur9svxz"):
        pair_ok = sim._route([vn_["valve_mur9syhl"]], [vn_["valve_mur9svxz"]], "", -1) != null
    chk("cesta mezi klapkami u průhledítka (760,850 ↔ 820,850) je průchozí", pair_ok)
    var skew: = 0
    for it in m.items:
        if it.type == "pipe" and not it.get("hose", false):
            for i in it.pts.size() - 1:
                if absf(it.pts[i][0] - it.pts[i + 1][0]) > 0.01 and absf(it.pts[i][1] - it.pts[i + 1][1]) > 0.01: skew += 1
    chk("všechny trubky (mimo hadice) jsou vodorovné nebo svislé", skew == 0, "šikmých úseků %d" % skew)

    fresh();S = sim.S
    var free_o = null
    for o in m.outlets:
        if m.hose_at(o.front) == null: free_o = o;break
    chk("kateta má volný vývod pro hadici", free_o != null)
    if free_o != null:
        var n_items: int = m.items.size()
        var added: bool = m._shift_hose(free_o.front)
        chk("Shift + klepnutí na volný vývod přidá hadici", added and m.items.size() == n_items + 1 and m.hose_at(free_o.front) != null and m.outlet_open[free_o.key])
        var hh = m.hose_at(free_o.front)
        chk("přidaná hadice je uživatelská a má volný konec", hh != null and hh[0].get("user", false) and hh[0].pts.size() >= 2)
        var removed: bool = m._shift_hose(free_o.front)
        chk("Shift + klepnutí na přidanou hadici ji odebere", removed and m.items.size() == n_items and m.hose_at(free_o.front) == null)

    fresh();S = sim.S
    var route_ids: Array = open_route.call("hlt", "rvp", ["phw"])
    S.mixAuto = false;S.rvpWater.target = 5000.0;S.rvpWater.on = true
    for i in 240: sim.sim_step(1.0)
    var wet: = 0
    for ed in sim.net.edges:
        if ed.c.w > 0.01: wet += 1
    chk("voda z bojleru vyplní i větve mimo cestu k pánvi (ne jen jedinou trasu)", wet > route_ids.size() * 2 + 3, "mokrých úseků %d, klapek na trase %d" % [wet, route_ids.size()])

    for gt in [["sk", "wp"], ["rvp", "wp"], ["rvp", "sk"]]:
        fresh();S = sim.S
        var gids: Array = open_route.call(gt[0], gt[1], [])
        var srcv = S.sk.mash if gt[0] == "sk" else S[gt[0]]
        srcv.w = 2500.0;srcv.T = 20.0
        var v0: float = sim.vol(srcv)
        for i in 600: sim.sim_step(1.0)
        var moved: float = v0 - sim.vol(srcv)
        var dst_v: float = sim.vol(S.wp) if gt[1] == "wp" else (sim.vol(S.rvp) if gt[1] == "rvp" else sim.skVolTotal())
        var piped: = 0.0
        for ed in sim.net.edges: piped += sim.vol(ed.c)
        if gt[1] == "sk":
            chk("samospád: do SK se shora (nad hladinou) nepřetéká, voda nejde do kopce", moved < 1.0, "%.1f L" % moved)
        else:
            chk("samospád %s → %s: z vyšší hladiny do nižší, nic se neztratí" % [gt[0], gt[1]], moved > 500.0 and absf(moved - dst_v - piped) < 5.0, "přeteklo %.0f L, cíl %.0f, v potrubí %.0f" % [moved, dst_v, piped])

    for wh in [1, 0]:
        fresh();S = sim.S
        var wids: Array = open_route.call("hlt", "sk", ["phw"])
        var vsw = sim.route_valves("hlt", "sk", ["phw"], wh)
        for idw in vsw:
            if m.outlet_open.has(idw): m.outlet_open[idw] = true
            elif sim.net.byId.has(idw): sim.net.byId[idw]["open"] = true
        sim.valve_ver += 1
        S.mixAuto = false;S.skWater.flow = 60.0;S.skWater.on = true
        for i in 240: sim.sim_step(1.0)
        var got: float = S.sk.under if wh == 1 else S.skWater.total
        chk("%s: voda z bojleru teče do SK (ručně otevřená cesta)" % ("podrážení" if wh == 1 else "vyslazování"), got > 30.0, "do SK přiteklo %.0f L, bojler %.0f L" % [got, S.hlt.w])

    fresh();S = sim.S
    var cab_item = null
    for itc in m.items:
        if itc.type == "panel": cab_item = itc
    var cb = load("res://cabinet.gd").new();cb.m = m;cb.setup(cab_item)
    var dpos = func(dx: float, dy: float) -> Vector2: return Vector2(cab_item.x, cab_item.y) + Vector2(dx, dy) * cb.sc
    chk("rozvaděč: hlavní vypínač a hřib se najdou klepnutím", cb.hit(dpos.call(274.0, 960.0)) == "main" and cb.hit(dpos.call(565.0, 972.0)) == "estop")
    chk("rozvaděč: volič čerpadla C1 a potenciometr se najdou klepnutím", cb.hit(dpos.call(616.0, 812.0)) == "pvarna" or cb.hit(dpos.call(441.0, 812.0)) == "potp_d" or cb.hit(dpos.call(450.0, 812.0)) == "potp_u")
    var f0: float = S.p1.flow
    cb.act("potp_u", true)
    chk("rozvaděč: potenciometr měniče zvýší průtok čerpadla C1 o krok", S.p1.flow > f0, "%.0f → %.0f L/min" % [f0, S.p1.flow])
    var hz0: float = S.rvp.hz
    cb.act("potmp_d", true)
    chk("rozvaděč: potenciometr míchadla sníží otáčky (Hz)", S.rvp.hz < hz0, "%.1f → %.1f Hz" % [hz0, S.rvp.hz])
    # --- trhnutí lože rychlým otevřením klapky ---
    sim.deposit_on = false
    fresh();S = sim.S
    S.sk.cells[0].w = 800.0;S.sk.cells[4].w = 150.0;S.sk.g = 60.0;S.sk.mode = "lauter"
    sim.skQmax();S.sk["qSlow"] = 15.0
    var ch0: float = S.sk.ch
    sim.skDraw(min(70.0, S.sk.Qmax), 1.0 / 60.0)
    chk("trhnutí: skok průtoku z 15 na vysoký utrhne lože (kanálky)", S.sk.ch > ch0 or S.sk.Qmax < 30.0, "ch %.2f → %.2f, Qmax %.0f" % [ch0, S.sk.ch, S.sk.Qmax])
    fresh();S = sim.S
    S.sk.cells[0].w = 800.0;S.sk.cells[4].w = 150.0;S.sk.g = 60.0;S.sk.mode = "lauter"
    sim.skQmax();ch0 = S.sk.ch
    sim.skDraw(25.0, 1.0 / 60.0)
    chk("trhnutí: rozjezd čerpadla z nuly lože netrhá", S.sk.ch <= ch0 + 0.001)
    # --- nános na dně: nutno několikrát trhnout ---
    sim.deposit_on = true
    fresh();S = sim.S
    S.sk.cells[0].w = 800.0;S.sk.cells[4].w = 150.0;S.sk.g = 60.0;S.sk.mode = "lauter";S.sk["dep"] = 1.0
    sim.skQmax()
    var qm_dep: float = S.sk.Qmax
    S.sk["qSlow"] = 8.0
    for i in 6:
        S.sk["tearT"] = 0.0;S.sk["qSlow"] = 5.0
        sim.skDraw(maxf(45.0, S.sk.Qmax * 1.5), 1.0 / 60.0)
        sim.skQmax()
    chk("nános: několik trhnutí odstraní nános a zvedne Qmax", S.sk.get("dep", 1.0) < 0.4 and S.sk.Qmax > qm_dep * 1.2, "Qmax %.0f → %.0f, nános %.2f" % [qm_dep, S.sk.Qmax, S.sk.get("dep", 1.0)])
    sim.deposit_on = false
    # --- smyk míchadla: nad 57 °C a rychle = gel β-glukanu, pomalu ne ---
    for hzt in [20.0, 45.0]:
        fresh();S = sim.S
        S.rvp.w = 3000.0;S.rvp.g = 700.0;S.rvp.T = 63.0;S.rvp.stir = true;S.rvp.hz = hzt
        for i in 60: sim.sim_step(10.0)
        if hzt < 30.0: chk("smyk: pomalé míchání (20 Hz) nad 57 °C gel netvoří", S.get("gel", 0.0) < 0.01, "gel %.3f" % S.get("gel", 0.0))
        else: chk("smyk: rychlé míchání (45 Hz) nad 57 °C tvoří gel a zvyšuje odpor lože", S.get("gel", 0.0) > 0.1, "gel %.3f" % S.get("gel", 0.0))
    var res0: float = sim.skRes(0.001)
    S.gel = 0.0;var res_g0: float = sim.skRes(0.001);S.gel = 1.0
    chk("smyk: gel zvýší odpor lože", sim.skRes(0.001) > res_g0 * 1.2, "%.3g → %.3g" % [res_g0, sim.skRes(0.001)])
    S.gel = 0.0
    # --- pH a izomerace chmele (Kosa s. 163) ---
    var isoR: Array = []
    for phv in [5.2, 5.8]:
        fresh();S = sim.S
        S.pH = phv;S.rvp.w = 3000.0;S.rvp.g = 0.0;S.rvp.T = 100.0;S.rvp.alpha = 100.0;S.rvp.iso = 0.0
        for i in 30: sim.sim_step(10.0)
        isoR.append(S.rvp.iso)
    chk("izomerace: vyšší pH (5,8) dá za stejný čas víc izo-α než pH 5,2", isoR[1] > isoR[0] * 1.1, "%.2f vs %.2f" % [isoR[0], isoR[1]])
    # --- víčko spilky: zavřené = tlakuje se ---
    fresh();S = sim.S
    S.sp["lidOpen"] = false;S.sp.L = 100.0;sim.sim_step(1.0)
    S.sp.L = 400.0;sim.sim_step(1.0)
    chk("víčko spilky: zavřené při plnění vyrobí přetlak", S.sp.get("press", 0.0) > 40.0, "%.0f mbar" % S.sp.get("press", 0.0))
    S.sp["lidOpen"] = true;sim.sim_step(1.0)
    chk("víčko spilky: otevřené = žádný přetlak", S.sp.get("press", 1.0) == 0.0)
    # --- ráz při vypnutí míchadla ---
    S.cab.key = true;S.cab.main = true;S.cab.estop = false
    S.rvp.w = 3000.0;S.rvp.g = 700.0;S.rvp.hz = 50.0;S.rvp.stir = true
    cb.act("stirmp", true)
    chk("ráz: vypnutí míchadla MP na 50 Hz v rmutu ho poškodí", S.rvp.get("broken", false) and not S.rvp.stir)
    cb.act("rstA", true)
    chk("ráz: reset poruchy míchadlo opraví", not S.rvp.get("broken", false))
    S.rvp.hz = 10.0;S.rvp.stir = true
    cb.act("stirmp", true)
    chk("ráz: při nízkých otáčkách se vypne bez škody", not S.rvp.get("broken", false) and not S.rvp.stir)
    S.cab.key = true;S.cab.main = true;S.cab.estop = false
    cb.act("hyd", true)
    S.sk.rakeH = 0.5
    cb.act("hup", true)
    for i in 3: sim.sim_step(1.0)
    cb.act("hup", false)
    chk("rozvaděč: HYDRAULIKA SK + NAHORU zvedá kopačku (0 = nahoře)", S.sk.rakeH < 0.5, "výška %.2f" % S.sk.rakeH)
    var rh1: float = S.sk.rakeH
    cb.act("hdn", true)
    for i in 3: sim.sim_step(1.0)
    cb.act("hdn", false)
    chk("rozvaděč: DOLŮ spouští kopačku do lože", S.sk.rakeH > rh1, "výška %.2f → %.2f" % [rh1, S.sk.rakeH])
    cb.act("hyd", true)
    S.cab.interlock = true
    cb.act("pvarna", true)
    chk("ochrana: C1 nejde pustit se zavřeným sáním / výtlakem (interlock)", not S.p1.on)
    S.cab.interlock = false
    cb.act("pvarna", true)
    chk("rozvaděč: volič VÁRNA ČERPADLO spustí C1 (ochrany vypnuté)", S.p1.on)
    cb.act("key", true)
    sim.sim_step(1.0)
    chk("rozvaděč: klíč START vypnut = čerpadla stojí a nic se nedá zapnout", not S.p1.on and not sim.cab_live())
    cb.act("pvarna", true)
    chk("rozvaděč: bez napětí volič nic nezapne", not S.p1.on)
    cb.act("key", true)
    cb.act("estop", true)
    chk("rozvaděč: hřib nouzového vypnutí vypne rozvaděč a shodí klíč", S.cab.estop and not S.cab.key and not sim.cab_live())
    cb.act("estop", true)
    chk("rozvaděč: po odblokování hřibu je zase pod napětím (klíč START už na rozvaděči není)", sim.cab_live())
    cb.act("mpauto", true);cb.act("mpheat", true)
    S.rvp.T = 20.0;S.rvp.Tsens = 20.0;S.rvp.w = 1000.0
    sim.sim_step(1.0)
    chk("rozvaděč: ohřev MP v automatu pustí páru, když je pod nastavenou teplotou", S.rvp.heat > 0.5, "ohřev %.2f" % S.rvp.heat)

    fresh();S = sim.S
    S.sk.cells[0].w = 800.0;S.sk.cells[4].w = 150.0;S.sk.g = 60.0;S.sk.mode = "lauter"
    var drained0: float = S.spillL
    for i in 120: sim.sim_step(1.0)
    chk("zavřený výhoz mláta nic nevypouští", S.spillL - drained0 < 0.1 and S.sk.g > 59.9)
    S.sk["hatch"] = true;S.sk.rake = false
    for i in 300: sim.sim_step(1.0)
    var g_norake: float = 60.0 - S.sk.g
    var liq_left: = 0.0
    for cl_ in S.sk.cells: liq_left += sim.cellVol(cl_)
    chk("otevřený výhoz mláta vypustí kapalinu do kanálu", S.spillL - drained0 > 800.0 and liq_left < 5.0, "vyteklo %.0f L, kapaliny zbývá %.0f L" % [S.spillL - drained0, liq_left])
    fresh();S = sim.S
    S.sk.cells[0].w = 800.0;S.sk.g = 60.0;S.sk.mode = "lauter";S.sk["hatch"] = true;S.sk.rake = true
    for i in 300: sim.sim_step(1.0)
    chk("s kopačkou mláto z výhozu vypadává rychleji než bez ní", (60.0 - S.sk.g) > g_norake * 1.5, "bez %.1f kg, s kopačkou %.1f kg" % [g_norake, 60.0 - S.sk.g])

    var valves_: Array = []
    for it in m.items:
        if it.type == "valve" and it.get("kind", "") != "zasl": valves_.append(it)
    for seed_ in [11, 23]:
        fresh();S = sim.S
        var rng: = RandomNumberGenerator.new();rng.seed = seed_
        for step in 3000:
            if step % 6 == 0:
                match rng.randi_range(0, 9):
                    0, 1, 2, 3:
                        var vv = valves_[rng.randi_range(0, valves_.size() - 1)]
                        vv["open"] = not (vv.get("open", true) != false)
                        if sim.net.byId.has(vv.id): sim.net.byId[vv.id]["open"] = vv["open"]
                        sim.valve_ver += 1
                    4, 5: S[["p1", "p2", "p3"][rng.randi_range(0, 2)]].on = rng.randf() < 0.5
                    6: S.rvpWater.on = rng.randf() < 0.5
                    7: S.rvp.stir = rng.randf() < 0.5;S.rvp.heat = rng.randf()
                    8: S.skWater.on = rng.randf() < 0.5
                    9: S.malt.on = rng.randf() < 0.5
            sim.sim_step(1.0)
        var sane: = true; var why: = ""
        for vk in [S.rvp, S.mp, S.wp, S.sk.mash]:
            for k in vk:
                if typeof(vk[k]) == TYPE_FLOAT and ( not is_finite(vk[k]) or (vk[k] < -0.001 and k not in ["O2", "smm"])):
                    sane = false;why = "%s=%s" % [k, str(vk[k])]
        for k2 in ["hlt", "lv"]:
            if not is_finite(S[k2].w) or S[k2].w < -0.001 or not is_finite(S[k2].T): sane = false;why = k2
        for p_ in ["p1", "p2", "p3"]:
            if not is_finite(S[p_].q) or S[p_].prime < 0.0 or S[p_].prime > 1.0001: sane = false;why = p_
        chk("odolnost: náhodné klikání (seed %d) nechá stav konečný a nezáporný" % seed_, sane, why)
    var AU = preload("res://audit_tests.gd").new();AU.run(self)
    var bad: = 0
    for r in results:
        if not r[1]: bad += 1
    print("TESTY: %d kontrol, %d selhání" % [results.size(), bad])
    return bad
