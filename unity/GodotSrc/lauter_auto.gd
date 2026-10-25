extends RefCounted





var simulation_ref: WeakRef
var sim:
    get: return simulation_ref.get_ref()
const SIGHT_ID: = "inl_mur9u2hg"
const CLEAR_TURB: = 15.0

func _init(simulation) -> void : simulation_ref = weakref(simulation)

func state() -> Dictionary:
    if not sim.S.has("lauterAuto"):
        sim.S.lauterAuto = {"on": false, "stage": "vypnut", "settle": 0.0, "clear": 0.0, "reason": "", "routeVersion": 0}
    return sim.S.lauterAuto

func route(dst: String) -> bool:
    var valves = sim.route_valves("sk", dst, ["p2"], 0 if dst == "sk" else -1)
    var water = sim.route_valves("hlt", "sk", ["phw"], 0)
    if valves == null or water == null: return false
    var steam_valves: = {}
    for node in sim.net.nodes.values():
        if node.valve == null: continue
        for edge in node.edges:
            if edge.medium == "steam": steam_valves[node.valve] = true
    for it in sim.net.byId.values():
        if it.type == "valve" and not steam_valves.has(it.id): it["open"] = false
        if it.type == "vessel" and it.get("shape") == "inlet": it["on"] = false
    for key in sim.main.outlet_open: sim.main.outlet_open[key] = false
    for key in valves + water:
        if sim.main.outlet_open.has(key): sim.main.outlet_open[key] = true
        elif sim.net.byId.has(key): sim.net.byId[key]["open"] = true
    sim.valve_ver += 1
    state().routeVersion = sim.valve_ver
    return true

func start() -> bool:
    var S = sim.S; var a = state()
    if S.sk.mode != "lauter" or S.sk.g < 1 or S.p1.on or S.p3.on or S.rvpWater.on:
        sim.logm("Automat scezování: nejprve odrmutuj (tlačítko Odrmutovat); zastav C1, C3 a napouštění pánve.", "warn");return false
    if sim.vol(S.rvp) > 100 or S.rvp.heat > 0:
        sim.logm("Automat scezování potřebuje prázdnou pánev s vypnutou párou.", "warn");return false
    if not route("sk"): sim.logm("Automat nemá cestu pro vorlauf a vyslazovací vodu.", "bad");return false
    a.on = true;a.stage = "usazení";a.settle = 0.0;a.clear = 0.0;a.reason = ""
    S.p2.on = false;S.skWater.on = false;S.sk.rake = false;S.sk.hydT = 0.4
    if S.sk.get("dep", 0.0) > 0.0:
        S.sk["dep"] = 0.0;sim.logm("Automat scezování prokopl síta (několik trhnutí klapkou) – nános ze dna je pryč.")
    sim.logm("Automat scezování přebírá kapalinné klapky: klid, vorlauf, odběr a vyslazování.")
    return true

func stop(reason: String) -> void :
    var a = state();a.on = false;a.stage = "zastaven";a.reason = reason
    sim.S.p2.on = false;sim.S.skWater.on = false
    sim.logm("Automat scezování zastaven: " + reason)

func update(seconds: float) -> void :
    var a = state()
    if not a.on: return
    var S = sim.S; var sk = S.sk
    if sim.valve_ver != a.get("routeVersion", sim.valve_ver) or sk.mode != "lauter" or sk.g < 1:
        stop("změna klapek nebo filtračního lože obsluhou");return
    if not sim.cab_live() or S.p1.on or S.p3.on or S.rvp.heat > 0:
        stop("zásah obsluhy / nepovolený souběh");return
    if S.hlt.w <= 150 or S.hlt.T < 76:
        stop("nedostatek horké vyslazovací vody");return
    if a.stage == "usazení":
        S.p2.on = false;S.skWater.on = false;sk.rake = false
        a.settle = 0.0 if S.skFillNow else a.settle + seconds
        if a.settle >= 1200:
            a.stage = "vorlauf";S.p2.flow = 25;S.p2.on = true
        return
    var sample = sim.sight_sample(sim.net.byId[SIGHT_ID])
    var actual: float = sample.get("flow", 0.0)
    if a.stage == "vorlauf":
        S.p2.flow = clampf(sk.Qmax * 0.5, 8.0, 25.0);S.p2.on = true
        var clear = sample.get("kind", "empty") == "wort" and actual > 0 and sample.get("turb", 999) < CLEAR_TURB and sk.vorlaufL >= 220
        a.clear = a.clear + seconds if clear else 0.0
        if a.clear >= 60:
            if not route("rvp"): stop("neprůchodná cesta do pánve");return
            a.stage = "odběr"
            state().routeVersion = sim.valve_ver
        return

    if sample.get("turb", 0) > 40 and actual > 0:
        if not route("sk"): stop("neprůchodný návrat zákalu");return
        a.stage = "vorlauf";a.clear = 0;S.skWater.on = false
        sim.logm("Zákal: automat vrací sladinu do SK a zpomaluje odběr.", "warn");return
    if sim.hotV(S.rvp) >= S.cfg.get("preboilV", 4430.0) or (sim.hotV(S.rvp) > 2000 and actual > 0 and sample.get("kind", "empty") != "empty" and sample.get("P", 99) <= S.cfg.spargeEndP):
        sim.logm("Konec: průhledítko %.2f °P, %s, %.0f L/min; podsítí %.2f °P" % [sample.get("P", -1), sample.get("kind", "empty"), actual, sk.outP])
        stop("cílový objem nebo slabé výstřelky");return
    if sim.tannin_risk() > 0.5:
        stop("rizikové pH / teplota vyslazování; ověř měřením");return
    if sk.Qmax < 10:
        stop("utáhnuté lože: nejprve ručně mělce nakypři a obnov vorlauf");return
    # odběr podle cílového tlakového rozdílu na loži ~50 mbar (Darcy: průtok úměrný Δp)
    S.p2.flow = clampf(sk.Qmax * 5000.0 / maxf(6000.0, sk.dpAvail), 15, 60);S.p2.on = true
    # kypření řízené tlakovým rozdílem na loži (jako automatika scezovací kádě): mělce při ~90 mbar, hlouběji a pomaleji nad 150 mbar
    if sk.dp > 150.0:
        S.p2.flow = maxf(10.0, S.p2.flow * 0.5);sk.rake = true;sk.hydT = 0.7
        sim.once("autorake2%d" % sim.tb(900), "Automat: Δp %d mbar – zpomaluji odběr a kypřím hlouběji." % roundi(sk.dp), "warn")
    elif sk.dp > 90.0:
        sk.rake = true;sk.hydT = 0.4
    elif sk.dp < 60.0 and sk.rake:
        sk.rake = false
    S.skWater.on = sim.skHead() < 0.1
    S.skWater.T = 77;S.mixAuto = true
    S.skWater.flow = clampf(sk.Qact + (0.1 - sim.skHead()) * S.cfg.skArea * 1000.0 / 2.0, 10, 100)
    if S.skWater.on: a.stage = "vyslazování"
