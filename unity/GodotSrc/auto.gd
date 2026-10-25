extends RefCounted






var m
var sim
var S
var on: = false
var i: = -1
var t0: = 0.0
var held: = 0.0
var cool_tf: = -99.0   # vyhlazená teplota mladiny za chladičem (regulace C3)
const COOL_TAU: = 120.0   # s – vyhlazení teploty mladiny
const COOL_G: = 0.25      # l/min na °C a minutu – zesílení regulace C3
var T_set: = 0.0
var status: = ""
var failed: = ""
var done: = false
var mashV: = 0.0
var total_mash: = 0.0
var boil_hold: = 0.0
var clear_s: = 0.0
var keep: = {}
var ice_prep: = false
var pend: = {}


const STEPS: = [
    ["Kontrola: zavřít všechny klapky", 1, "check"], 
    ["Napouštím vodu do pánve (38 °C)", 60, "fill"], 
    ["Vystírám slad, míchadlo", 40, "malt"], 
    ["Ohřev na 50 °C a prodleva 15 min", 90, "r52"], 
    ["Podsítí SK: zaplavit vodou", 60, "under"], 
    ["1. rmut: polovinu díla do SK (C1)", 90, "d1split"], 
    ["1. rmut: 63 °C, 20 min", 120, "d1_63"], 
    ["1. rmut: 72 °C, 20 min", 120, "d1_72"], 
    ["1. rmut: var 10 min", 120, "d1_boil"], 
    ["1. rmut: zpět do SK", 90, "d1_ret"], 
    ["2. rmut: dvě třetiny díla ze SK do pánve", 90, "d2split"], 
    ["2. rmut: 63 °C, 20 min", 120, "d2_63"], 
    ["2. rmut: 72 °C, 20 min", 120, "d2_72"], 
    ["2. rmut: var 10 min", 120, "d2_boil"], 
    ["2. rmut: zpět do SK", 90, "d2_ret"], 
    ["Zcukření v SK (10 min)", 40, "conv"], 
    ["Odrmutovat: vytvořit filtrační lože", 1, "bed"], 
    ["Usazení lože 20 min", 25, "settle"], 
    ["Proplach pod síty proudem horké vody z bojleru (do kanálu)", 30, "flush"], 
    ["Vorlauf: sladina přes lože, než se vyčistí", 90, "vorlauf"], 
    ["Scezování 1: první sladina do pánve (~30 hl)", 150, "run1"], 
    ["Voda pod síta a promíchání mláta s ~1 hl vody", 30, "mix"], 
    ["Podržení mláta (klid)", 20, "hold"], 
    ["Dosladit do konečného objemu", 240, "sparge"], 
    ["Doplnit pánev horkou vodou na objem (poslední podíl pod 1 °P se nescezuje)", 30, "topup"], 
    ["Chmelovar", 300, "boil"], 
    ["Přečerpání do vířivky", 90, "wp"], 
    ["Usazení kalu ve vířivce", 90, "wpset"], 
    ["Ledovka: dočkat se vychlazení", 300, "ice"], 
    ["Chlazení a spilka (C3 + ledová voda)", 120, "cool"], 
    ["Protlačka a hotovo", 5, "push"], 
]

func start() -> void :
    S = sim.S;on = true;done = false;failed = "";i = 0;_begin()
    S.sp["lidOpen"] = true   # automat otevře víčko spilky
    m.running = true
    S.mixAuto = true
    sim.logm("AUTO-VÁRKA: start.", "info")

func stop(msg: = "") -> void :
    on = false;S.mixAuto = false;pend.clear()
    for p in ["p1", "p2", "p3"]: S[p].on = false
    S.rvpWater.on = false;S.skWater.on = false;S.malt.on = false;S.rvp.heat = 0.0
    if msg != "": sim.logm("AUTO-VÁRKA: " + msg, "warn")

func _log(t: String) -> void :
    sim.logm("AUTO: " + t)

func _id() -> String:
    return STEPS[i][2] if i >= 0 and i < STEPS.size() else ""


func _close_all() -> void :
    pend.clear()
    for pk in ["p1", "p2", "p3"]: S[pk].on = false
    for it in m.items:
        if it.type == "valve" and it.get("kind", "") != "zasl": it["open"] = keep.has(it.id)
    for k in m.outlet_open: m.outlet_open[k] = keep.has(k)
    _inlet(ice_prep and S.lv.w < 0.75 * S.cfg.lvVol)


func _ice_start() -> void :
    keep.clear()
    for pair in [["vodovod", "lv", []], ["lv", "lv", ["plv"]]]:
        var vs = sim.route_valves(pair[0], pair[1], pair[2])
        if vs == null: continue
        for id in vs: keep[id] = true
    ice_prep = true
    for it in m.items:
        if it.type == "sw": it["on"] = true
    S.lv.pump = true
    for id in keep:
        if m.outlet_open.has(id): m.outlet_open[id] = true
        elif sim.net.byId.has(id): sim.net.byId[id]["open"] = true
    _inlet(true)


func _pump(key: String) -> void :
    if S.rvp.stir == false and sim.vol(S.rvp) > 100.0 and sim.thick_factor(S.rvp) > 0.15 and (_id() in ["d1split", "d1_ret", "d2_ret"]): S.rvp.stir = true
    if S.sk.mode == "mash" and sim.vol(S.sk.mash) > 5.0 and sim.thick_factor(S.sk.mash) > 0.15: S.sk.rake = true
    pend[key] = true

func _pump_wait() -> void :
    for k in pend.keys():
        if S[k].prime >= 0.95:
            S[k].on = true;pend.erase(k)

func _route(a: String, b: String, pumps: Array, where: = -1, plate: = "") -> bool:
    var vs = sim.route_valves(a, b, pumps, where, plate)
    if vs == null:
        _fail("Cesta %s → %s ve schématu neexistuje." % [sim.VN(a), sim.VN(b)]);return false
    for id in vs:
        if m.outlet_open.has(id): m.outlet_open[id] = true
        elif sim.net.byId.has(id):
            sim.net.byId[id]["open"] = true
            if sim.net.byId[id].get("kind", "") == "skrt" and int(sim.net.byId[id].get("pct", 50)) == 0: sim.net.byId[id]["pct"] = 100
    sim.valve_ver += 1
    return true

func _plate_id() -> String:
    for it in m.items:
        if it.type == "vessel" and it.get("shape") == "plate": return it.id
    return ""

func ice_inlet_on() -> bool:
    for it in m.items:
        if it.type == "vessel" and it.get("shape") == "inlet": return it.get("on", false)
    return false

func _inlet(on_: bool) -> void :
    for it in m.items:
        if it.type == "vessel" and it.get("shape") == "inlet": it["on"] = on_
    sim.valve_ver += 1

func _fail(why: String) -> void :
    failed = why;status = "ZASEKLO SE: " + why
    sim.logm("AUTO-VÁRKA se zastavila – " + why, "bad")
    stop()


func _regulate() -> void :
    var r = S.rvp
    if T_set <= 0.0: return
    var e: float = T_set - r.Tsens
    r.heat = 0.0 if (r.w < 20 or e <= -0.3) else clampf(e / 3.0, 0.12, 1.0)

func _wait_min() -> float:
    return (S.t - t0) / 60.0

func _is_hold(id: String) -> bool:
    return id in ["r52", "d1_63", "d1_72", "d2_63", "d2_72"]

func _begin() -> void :
    var s = STEPS[i]
    var id: String = s[2]
    status = "%d/%d  %s" % [i + 1, STEPS.size(), s[0]]
    t0 = S.t;held = 0.0;clear_s = 0.0;cool_tf = -99.0
    var c = S.cfg
    _log(s[0])
    if id in ["fill", "under", "d1split", "d1_ret", "d2split", "d2_ret", "conv", "settle", "flush", "vorlauf", "run1", "mix", "hold", "sparge", "topup", "wp", "cool"]: _close_all()
    match id:
        "check":
            _close_all();_ice_start()
        "fill":
            if not _route("hlt", "rvp", ["phw"]): return
            S.mixAuto = true;S.rvpWater.T = 38.0;S.rvpWater.k = 1.0
            S.rvpWater.target = S.rvpWater.added + c.water;S.rvpWater.on = true
        "malt":
            S.rvp.stir = true;S.malt.target = maxf(S.malt.added, c.malt);S.malt.gate = true;S.malt.on = true
        "r52": T_set = 50.0
        "under":
            T_set = 50.0
            if not _route("hlt", "sk", ["phw"], 1): return
            S.skWater.T = 78.0;S.skWater.on = true
        "d1split":
            S.skWater.on = false;T_set = 0.0;S.rvp.heat = 0.0
            if not _route("rvp", "sk", ["p1"]): return
            mashV = sim.vol(S.rvp);total_mash = mashV
            S.p1.flow = 250.0;_pump("p1")
        "d1_63", "d2_63": T_set = 63.0
        "d1_72", "d2_72": T_set = 72.0
        "d1_boil", "d2_boil": T_set = 0.0;S.rvp.stir = true;S.rvp.heat = 1.0
        "d1_ret", "d2_ret":
            T_set = 0.0;S.rvp.heat = 0.0
            if not _route("rvp", "sk", ["p1"]): return
            S.p1.flow = 250.0;_pump("p1")
        "d2split":
            if not _route("sk", "rvp", ["p1"]): return
            total_mash = sim.vol(S.rvp) + sim.vol(S.sk.mash)
            S.p1.flow = 250.0;_pump("p1")
        "conv": T_set = 0.0
        "bed":
            sim.start_lautering()
        "settle": pass
        "flush":
            # protlačka: horká voda z bojleru pod síta a ven do kanálu (nános ze dna se smyje proudem vody, ne trhnutím klapky)
            if not _route("hlt", "sk", ["phw"], 1): return
            var dv = sim.drain_valves("sk")
            if dv == null:
                # plán nemá výpust pod síty: proplach nejde udělat fyzicky, přeskočí se (a nános se pak smyje při scezování)
                sim.logm("Plán nemá pod síty SK výpust do kanálu – proplach (protlačka) se přeskakuje. Doplň odpadní trubku s klapkou ode dna pod sity.", "warn")
                S.flush = {"on": false, "L": 300.0, "target": 300.0, "vs": [], "rate": 100.0}
                S.sk["dep"] = 0.0
                return
            for vid in dv:
                if m.outlet_open.has(vid): m.outlet_open[vid] = true
                elif sim.net.byId.has(vid): sim.net.byId[vid]["open"] = true
            var wv = sim.route_valves("hlt", "sk", ["phw"], 1)
            var spr_id = ""
            var gnode = sim.net.nodes.get(sim.last_goal)
            if gnode != null:
                for ga in gnode.att:
                    if ga.kind == "vessel" and ga.spray != null: spr_id = str(ga.spray)
            print("FLUSH cesta: klapek bojler→pod síta %d %s, přes trysku: %s, výpust: %s" % [(wv.size() if wv != null else -1), str(wv), spr_id, str(dv)])
            S.flush = {"on": true, "L": 0.0, "target": 300.0, "vs": (wv if wv != null else []) + dv, "rate": 100.0}
            sim.valve_ver += 1
        "topup":
            S.p2.on = false;S.skWater.on = false
            var need: float = maxf(0.0, c.get("preboilV", 4430.0) - sim.hotV(S.rvp))
            keep["topup"] = need
            if need > 20.0:
                if not _route("hlt", "rvp", ["phw"]): return
                S.rvpWater.T = 78.0;S.rvpWater.k = 1.0
                S.rvpWater.target = S.rvpWater.added + need;S.rvpWater.on = true
        "vorlauf":
            if not _route("sk", "sk", ["p2"], 0): return
            S.p2.flow = 20.0;_pump("p2");S.sk.rake = false;S.sk.hydT = 0.4
        "run1":
            # první sladina bez vyslazování: sedí se do pánve, kopačka klidně
            S.p2.on = false;S.skWater.on = false
            if not _route("sk", "rvp", ["p2"]): return
            S.p2.flow = 40.0;_pump("p2")
        "mix":
            # voda pod síta, mláto se promíchá s cca 1 hl vody (čerpadlo stojí)
            S.p2.on = false
            if not _route("hlt", "sk", ["phw"], 1): return
            keep["mix0"] = S.skWater.total
            S.skWater.T = 77.0;S.skWater.flow = 50.0;S.skWater.on = true
            S.sk.rake = true;S.sk.hydT = 0.7
        "hold":
            S.skWater.on = false;S.p2.on = false;S.sk.rake = false
        "sparge":
            S.p2.on = false
            if not _route("sk", "rvp", ["p2"]): return
            if not _route("hlt", "sk", ["phw"], 0): return
            S.skWater.T = 77.0;S.skWater.flow = 60.0;S.p2.flow = 60.0;S.skWater.on = true;_pump("p2")
        "boil":
            S.p2.on = false;S.skWater.on = false;S.rvp.stir = false
            T_set = 0.0;S.rvp.heat = 0.85;boil_hold = 0.0
        "wp":
            S.rvp.heat = 0.0;T_set = 0.0
            if not _route("rvp", "wp", ["p1"]): return
            S.p1.flow = 300.0;_pump("p1")
        "wpset":
            S.p1.on = false
        "ice":
            pass
        "cool":
            if not _route("wp", "spilka", ["p3"]): return
            if not _route("lv", "hlt", ["plv"], -1, _plate_id()): return
            if not _route("lv", "lv", ["plv"]): return
            S.lv.pump = true
            S.p3.flow = 40.0;_pump("p3")   # začít pomalu: chladič je na začátku teplý, pomalý tok se vychladí líp
        "push":
            sim.push()
    m.redraw()

func _finished() -> bool:
    var f = S.flags; var c = S.cfg; var r = S.rvp

    if _is_hold(_id()) and held <= 0.0 and r.Tsens > T_set + 2.0:
        sim.logm("AUTO: rmut má už %s °C (nad %s °C) – nechladí se, pokračuje se od této teploty." % [sim.fmt1(r.Tsens), sim.fmt0(T_set)])
        return true
    match _id():
        "check": return true
        "fill": return not S.rvpWater.on and S.rvpWater.added >= S.rvpWater.target - 5.0
        "malt": return S.malt.added >= S.malt.target - 1.0
        "r52": return held >= 15.0
        "under": return S.sk.under >= 150.0
        "d1split": return not S.p1.on and sim.vol(S.sk.mash) > 500.0
        "d1_63", "d2_63": return held >= 20.0
        "d1_72", "d2_72": return held >= 20.0
        "d1_boil", "d2_boil": return r.boiling and r.grainBoilMin >= 10.0
        "d1_ret", "d2_ret": return not S.p1.on and sim.vol(r) <= c.rvpDead + 12.0
        "d2split": return not S.p1.on and sim.vol(r) >= total_mash * 2.0 / 3.0 * 0.9
        "conv": return _wait_min() >= 10.0 and S.sk.mash.T >= 68.0
        "bed": return S.sk.mode == "lauter"
        "settle": return _wait_min() >= 20.0
        "flush": return S.flush.L >= S.flush.target - 0.5
        "vorlauf": return clear_s >= 60.0 and S.sk.vorlaufL >= 220.0
        "run1": return sim.liqVol(r) >= 3000.0 or sim.cellVol(S.sk.cells[0]) < 60.0 or sim.skVolTotal() <= sim.skRet() + 20.0 or (S.sk.drawn and S.sk.outP < c.spargeEndP)
        "mix": return S.skWater.total - float(keep.get("mix0", 0.0)) >= 100.0
        "hold": return _wait_min() >= 10.0
        "sparge": return f.full == 1 or (not S.skWater.on and S.skWater.total > 600.0 and sim.skVolTotal() <= sim.skRet() + 20.0) or (S.skWater.total >= 3000.0 and not S.p2.on) or (S.sk.drawn and S.sk.outP < c.spargeEndP and S.skWater.total > 600.0)
        "topup": return float(keep.get("topup", 0.0)) <= 20.0 or (not S.rvpWater.on and S.rvpWater.added >= S.rvpWater.target - 5.0)
        "boil": return f.wortBoil >= c.boilMin
        "wp": return not S.p1.on and sim.vol(S.wp) > 3000.0
        "wpset": return f.whirlDone == 1
        "ice": return S.lv.w >= 0.7 * c.lvVol and S.lv.T <= 5.0
        "cool": return not S.p3.on and S.sp.L > 3000.0
        "push": return f.pushed == 1
    return true

func _end_step() -> void :
    match _id():
        "d1split", "d2split", "d1_ret", "d2_ret", "wp": S.p1.on = false
        "flush": S.flush.on = false
        "run1": S.p2.on = false
        "mix": S.skWater.on = false;S.sk.rake = false
        "sparge": S.skWater.on = false;S.p2.on = false
        "boil", "d1_boil", "d2_boil": S.rvp.heat = 0.0;T_set = 0.0
        "malt": S.malt.gate = false
    if S.sk.mode == "mash": S.sk.rake = false
    i += 1
    if i >= STEPS.size():
        on = false;done = true;status = "AUTO-VÁRKA HOTOVA"
        sim.logm("AUTO-VÁRKA dokončena.", "info");stop();return
    _begin()


func tick(dt: float) -> void :
    if not on: return
    if i < 0 or i >= STEPS.size(): return
    var r = S.rvp; var f = S.flags; var c = S.cfg
    var id: = _id()
    _pump_wait()

    # nad 57 °C míchat pomaleji (Kunze s. 241, obvodová rychlost < 1 m/s ≈ 20 Hz), jinak vzniká gel z β-glukanu
    if r.stir and sim.thick_factor(r) > 0.1: r.hz = 20.0 if r.T > 57.0 else 35.0
    if T_set > 0:
        if _is_hold(id) and absf(r.Tsens - T_set) <= 1.5: held += dt / 60.0
        _regulate()

    if id == "d1split" and S.p1.on and sim.vol(r) <= mashV * 0.5: S.p1.on = false
    if id == "d2split" and S.p1.on and sim.vol(r) >= total_mash * 2.0 / 3.0: S.p1.on = false

    if id == "d1_boil" or id == "d2_boil":
        S.rvp.stir = true
        if r.boiling and r.grainBoilMin >= 10.0: r.heat = 0.0

    if id == "boil":
        if r.boiling:
            boil_hold += dt / 60.0
            r.heat = 0.55 if boil_hold < 15.0 else 0.85
            var nx = sim.hopPlanNext()
            if nx != null and boil_hold >= nx.min:
                r.heat = 0.3
                if r.evapKgMin < 4.0: sim.hopPlanAdd()
        else: r.heat = 0.85 if S.rvp.T > 90 else 1.0

    if id == "vorlauf":
        S.p2.flow = clampf(S.sk.Qmax * 0.5, 8.0, 25.0)
        var smp = sim.sight_sample(sim.net.byId["inl_mur9u2hg"])
        var clear = smp.get("kind", "empty") == "wort" and smp.get("flow", 0.0) > 0.0 and smp.get("turb", 999.0) < 15.0 and S.sk.turb < 20.0
        clear_s = clear_s + dt if clear else 0.0
    if id == "run1":
        S.p2.flow = clampf(S.sk.Qmax * 5500.0 / maxf(6000.0, S.sk.dpAvail), 15.0, 70.0)
        if S.sk.dp > 150.0: S.p2.flow = maxf(12.0, S.p2.flow * 0.5)
        S.skWater.on = false
        if S.sk.dig <= 0:
            if S.sk.dp > 120.0: S.sk.hydT = 0.7;S.sk.rake = true
            elif S.sk.dp > 70.0 or not S.sk.rake: S.sk.hydT = 0.4;S.sk.rake = true
    if id == "sparge":

        # odběr na cílový tlakový rozdíl na loži ~55 mbar (Darcy), kypření podle Δp
        S.p2.flow = clampf(S.sk.Qmax * 5500.0 / maxf(6000.0, S.sk.dpAvail), 15.0, 70.0)
        if S.sk.dp > 150.0: S.p2.flow = maxf(12.0, S.p2.flow * 0.5)
        S.skWater.flow = clampf(S.p2.flow * 0.95, 10.0, 100.0)
        # vyslazovací vodu zavřít včas: voda v pánvi + volná sladina nad ložem už stačí na plnou pánev → lože doběhne
        var free_sk: float = maxf(0.0, sim.skVolTotal() - sim.skRet())
        S.skWater.on = S.skWater.total < 3000.0 and sim.skVolTotal() < c.skCap * 0.97 and f.full != 1 and sim.hotV(S.rvp) + free_sk < c.get("preboilV", 4430.0)
        if S.sk.dig <= 0:
            if S.sk.dp > 120.0: S.sk.hydT = 0.7;S.sk.rake = true
            elif S.sk.dp > 70.0 or not S.sk.rake: S.sk.hydT = 0.4;S.sk.rake = true
    if id == "cool" and S.cool.Tout != null:

        # regulace teploty mladiny do spilky: vyhlazená teplota (plechový chladič + trubky mají zpoždění, rychlá regulace by kmitala 3 ↔ 23 °C)
        if cool_tf < -50.0: cool_tf = S.cool.Tout
        cool_tf += (S.cool.Tout - cool_tf) * minf(1.0, dt / COOL_TAU)
        S.p3.flow = clampf(S.p3.flow + (8.5 - cool_tf) * COOL_G * dt / 60.0, 30.0, 200.0)
    if ice_prep:
        if S.lv.w >= 0.35 * c.lvVol and not S.lv.chill: sim.lvChillSet(true)
        if S.lv.w >= 0.75 * c.lvVol and ice_inlet_on(): _inlet(false)
    if _finished():
        _end_step();return

    if _wait_min() > STEPS[i][1]:
        _fail(_why())

func _why() -> String:
    var S_ = S
    match _id():
        "fill": return "Voda z bojleru do pánve neteče (napuštěno %d L). Zkontroluj cestu a čerpadlo horké vody. %s" % [roundi(S_.rvpWater.added), _recent()]
        "malt": return "Slad se nevystírá. %s" % _recent()
        "r52", "d1_63", "d1_72", "d2_63", "d2_72": return "Pánev nedosáhla/neudržela %s °C (je %s °C): pára nemá cestu do pláště nebo je málo vody. %s" % [sim.fmt0(T_set), sim.fmt1(S_.rvp.T), _recent()]
        "d1_boil", "d2_boil": return "Rmut nevře dost dlouho (%s °C, var %d min). %s" % [sim.fmt1(S_.rvp.T), roundi(S_.rvp.grainBoilMin), _recent()]
        "under": return "Voda pod síta SK neteče (podsítí %d L). %s" % [roundi(S_.sk.under), _recent()]
        "d1split", "d1_ret", "d2split", "d2_ret": return "Rmut mezi pánví a SK neteče (v pánvi %d L, v SK %d L). %s" % [roundi(sim.vol(S_.rvp)), roundi(sim.vol(S_.sk.mash)), _recent()]
        "flush": return "Proplach pod síty neteče (proteklo %d L). Zkontroluj cestu bojler → pod síta a výpust do kanálu. %s" % [roundi(S_.flush.L), _recent()]
        "run1": return "První sladina se nescezuje: v pánvi %d L. %s" % [roundi(sim.liqVol(S_.rvp)), _recent()]
        "mix": return "Voda pod síta neteče (přidáno %d L). %s" % [roundi(S_.skWater.total - float(keep.get("mix0", 0.0))), _recent()]
        "vorlauf": return "Vorlauf se nevyčistil (zákal %d). %s" % [roundi(S_.sk.turb), _recent()]
        "sparge": return "Sladina se nescezuje: v pánvi %d L. %s" % [roundi(sim.liqVol(S_.rvp)), _recent()]
        "topup": return "Horká voda z bojleru do pánve neteče (napuštěno %d L). %s" % [roundi(S_.rvpWater.added), _recent()]
        "boil": return "Mladina nevře dost dlouho (%d min). %s" % [roundi(S_.flags.wortBoil), _recent()]
        "wp": return "Do vířivky natekla jen %d L. %s" % [roundi(sim.vol(S_.wp)), _recent()]
        "ice": return "Ledovka se nenapustila/nenachladila (%d hl, %s °C). %s" % [roundi(S_.lv.w / 100.0), sim.fmt1(S_.lv.T), _recent()]
        "cool": return "Do spilky natekla jen %d L. %s" % [roundi(S_.sp.L), _recent()]
    return "Krok trvá příliš dlouho. " + _recent()


func _recent() -> String:
    var out: = []
    for l in S.log.slice(0, 12):
        if (l.type == "bad" or l.type == "warn") and not l.msg.begins_with("AUTO") and not out.has(l.msg): out.append(l.msg)
        if out.size() >= 2: break
    return " | ".join(PackedStringArray(out))


func to_dict() -> Dictionary:
    return {"sid": _id(), "on": on, "i": i, "t0": t0, "held": held, "T_set": T_set, "status": status, "failed": failed, "done": done, "mashV": mashV, "total_mash": total_mash, "boil_hold": boil_hold, "clear_s": clear_s, "keep": keep, "ice_prep": ice_prep}

func from_dict(d: Dictionary) -> void :
    on = d.get("on", false);i = d.get("i", -1)
    if d.has("sid"):   # krok podle názvu (kroky se mohou přidávat)
        for k in STEPS.size():
            if STEPS[k][2] == d.sid: i = k
    t0 = d.get("t0", 0.0);held = d.get("held", 0.0);T_set = d.get("T_set", 0.0)
    status = d.get("status", "");failed = d.get("failed", "");done = d.get("done", false);mashV = d.get("mashV", 0.0)
    total_mash = d.get("total_mash", 0.0);boil_hold = d.get("boil_hold", 0.0);clear_s = d.get("clear_s", 0.0)
    keep = d.get("keep", {});ice_prep = d.get("ice_prep", false)
    if i >= STEPS.size(): on = false
    S = sim.S
    if on: S.mixAuto = true
