extends RefCounted
## Provozní zásady z praxe – hlídá pořadí kroků obsluhy a vysvětluje, proč se to tak dělá.
## Porušení se počítá do hodnocení várky (S.practice). Fyzikální následky (nasucho, ráz, připálení…) řeší sim.gd samo;
## tady je hlavně postup a vysvětlení.

var m            # main
var sim

## stručný přehled do nápovědy (F1)
const RULES: = [
	["Spuštění čerpadla", "otevřít klapky PŘED čerpadlem → nechat zavodnit (natéct, odvzdušnit) → otevřít klapky ZA čerpadlem → pustit na nízké otáčky → přidávat postupně"],
	["Vypnutí čerpadla", "ubrat otáčky → VYPNOUT čerpadlo → zavřít klapku ZA čerpadlem → zavřít klapku PŘED čerpadlem (čerpadlo zůstane zavodněné)"],
	["Nikdy", "nezavírat klapky za chodu čerpadla (nasucho / tlačí do zavřené klapky), nepouštět čerpadlo nasucho"],
	["Pára", "nejdřív míchadlo, pak pára; páru otevírat pozvolna (odvodnit kondenzát), plášť musí být pod hladinou"],
	["Vystírání", "míchadlo běží dřív, než se pustí slad; voda 35–40 °C (nad 60 °C trpí β-amyláza)"],
	["Horká mladina", "u čerpání horké mladiny hlídat hladinu v sací nádobě – při nízké hladině kavituje"],
	["Chlazení", "nejdřív pustit chladivo (voda / ledová), pak mladinu; končit obráceně – nejdřív mladinu zastavit"],
	["Kotel", "nepouštět naplno všechny ohřevy naráz – tlak páry spadne a nic se neohřívá"],
]

const QN: = {"p1": 400.0, "p2": 150.0, "p3": 200.0}

func _S(): return sim.S

func _bad(rule: String, msg: String, why: String, fix: String) -> void :
	var S = _S()
	if not S.has("practice"): S.practice = {"n": 0, "rules": {}}
	S.practice.n += 1
	S.practice.rules[rule] = S.practice.rules.get(rule, 0) + 1
	sim.logm("ZÁSADA – %s %s Správně: %s" % [msg, "Proč: " + why + "." if why != "" else "", fix], "warn")
	if m != null and m.hud != null: m.hud.toast("⚠ " + msg)

func _tip(key: String, msg: String) -> void :
	sim.once(key, "TIP – " + msg, "info")

func _pump_name(b: String) -> String: return m.pump_name(b) if m != null else b.to_upper()

## sání a výtlak čerpadla (nádoby)
func _ends(b: String) -> Dictionary:
	var pis = sim.items_by_bind(b)
	if not pis.size(): return {}
	var pn = sim.net.pumpNodes.get(pis[0].id, {"in": [], "out": []})
	var src = null; var dst: = []
	for hh in sim.bfs(pn["in"]).hits:
		if hh.att.low and hh.att.bind in ["rvp", "mp", "sk", "wp", "hlt", "spilka", "lv"]:
			if src == null or sim._src_level(hh.att.bind) > sim._src_level(src): src = hh.att.bind
	for d in sim.bfs(pn.out).hits:
		if not d.att.bind in dst: dst.append(d.att.bind)
	return {"src": src, "dst": dst}

func _level_frac(b: String) -> float:
	var cap: float = 0.0
	match b:
		"rvp": cap = _S().cfg.get("rvpCap", 5000.0)
		"mp": cap = _S().cfg.get("mpCap", 5000.0)
		"wp": cap = _S().cfg.get("wpCap", 5000.0)
		"sk": cap = _S().cfg.skCap
		"hlt": cap = _S().cfg.hltCap
		"spilka": cap = _S().cfg.get("spCap", 5000.0)
		"lv": cap = _S().cfg.lvVol
	if cap <= 0.0: return 0.5
	return sim._src_level(b) / cap if b != "lv" else _S().lv.w / cap

func _temp(b: String) -> float:
	var S = _S()
	match b:
		"rvp", "mp", "wp": return S[b].T
		"hlt": return S.hlt.T
		"lv": return S.lv.T
		"sk": return S.sk.T
	return 20.0

# ---------- události ----------
func on_pump_start(b: String) -> void :
	var S = _S()
	if not b in ["p1", "p2", "p3"]:
		if b == "plv": return
		return
	var q = S[b]
	var pn: String = _pump_name(b)
	if q.get("prime", 1.0) < 0.95 and not q.get("airLock", false):
		_bad("zavodneni", "%s jsi pustil nezavodněné." % pn, "odstředivé čerpadlo vzduch nenasaje – běží naprázdno, hučí a ničí se mechanická ucpávka",
			"otevři klapky před čerpadlem, počkej, až čerpadlo zavodní (modrá kapka u čerpadla), teprve pak ho pusť")
	if q.get("flow", 0.0) > 0.6 * QN.get(b, 200.0):
		_bad("rozjezd", "%s jsi rozjel rovnou na vysoké otáčky (%d L/min)." % [pn, roundi(q.flow)],
			"prudký rozjezd udělá ráz v potrubí, strhne vzduch; rmutu to láme zrno, u scezování utáhne lože",
			"pusť na nízké otáčky (FM 2–3) a přidávej postupně podle průtoku")
	var e: Dictionary = _ends(b)
	if e.is_empty(): return
	if e.src != null and _temp(e.src) > 90.0 and _level_frac(e.src) < 0.15:
		_bad("kavitace", "%s sání z %s: horká kapalina (%d °C) a nízká hladina." % [pn, sim.VN(e.src), roundi(_temp(e.src))],
			"vroucí kapalina má na sání skoro žádnou rezervu tlaku (NPSH) – čerpadlo kavituje: rachotí, nečerpá, ničí oběžné kolo",
			"čerpej horkou mladinu s dostatečnou hladinou, ke konci ubírej otáčky")
	for d in e.dst:
		if d in ["rvp", "mp", "sk", "wp", "hlt", "spilka"] and _level_frac(d) > 0.95:
			_bad("preplneni", "%s tlačí do nádoby, která je skoro plná (%s)." % [pn, d],
				"nádoba přeteče – ztráta a nebezpečí opaření", "zkontroluj objem v cílové nádobě dřív, než pustíš čerpadlo")
	if b == "p3" and "spilka" in e.dst:
		var inlet_on: = false
		for it in m.items:
			if it.type == "vessel" and it.get("shape") == "inlet" and it.get("on", false): inlet_on = true
		if not S.lv.pump and not inlet_on:
			_bad("chlazeni", "Mladinu (C3) jsi pustil do chladiče dřív než chladivo.",
				"bez vody / ledové vody jde mladina do spilky horká – zabije kvasnice, hrozí infekce a teplé zakvašení",
				"nejdřív pusť chladicí vodu a ledovou vodu (C5), pak C3 s mladinou")

func on_pump_stop(b: String) -> void :
	var S = _S()
	if b == "plv" and S.p3.on:
		_bad("chlazeni_konec", "Ledovou vodu jsi zastavil, zatímco C3 ještě žene mladinu chladičem.",
			"zbytek mladiny poteče do spilky nevychlazený", "na konci chlazení nejdřív zastav C3, pak ledovou vodu")
		return
	if not b in ["p1", "p2", "p3"]: return
	if sim.pump_interlock(b) == "":
		_tip("stoptip%s%d" % [b, sim.tb(1800)], "%s vypnuto. Teď zavři klapku ZA čerpadlem a pak PŘED ním – čerpadlo zůstane zavodněné a nic nepoteče samospádem zpátky." % _pump_name(b))

## volá main po přestavení klapky (vlastní ochrany „nasucho / proti zavřené“ hlídá sim.gd za chodu)
func on_valve(_v) -> void :
	pass

var _ph: = {}
var _nostir: = {}
func tick(dt: float) -> void :
	var S = _S()
	for name in ["rvp", "mp"]:
		var v = S[name]
		var h: float = float(v.get("heat", 0.0))
		var prev: float = _ph.get(name, h)
		if prev <= 0.05 and h >= 0.6 and S.boiler.on and sim.liqVol(v) > 100:
			_bad("para_naraz", "Páru do %s jsi otevřel naráz na %d %%." % [sim.VN(name), roundi(h * 100.0)],
				"v plášti a potrubí stojí kondenzát – prudce otevřená pára ho rozžene a udělá vodní ráz (rány, poškozené těsnění)",
				"otevři páru nejdřív na 10–20 %, nech odvodnit kondenzát a pak přidávej")
		_ph[name] = h
		var solids: float = float(v.get("g", 0.0)) + float(v.get("su", 0.0))
		if h > 0.05 and not v.get("stir", false) and solids > 5.0 and sim.liqVol(v) > 100:
			_nostir[name] = _nostir.get(name, 0.0) + dt
			if _nostir[name] > 30.0:
				_nostir[name] = -540.0
				_bad("micha_para", "%s: topíš párou a míchadlo stojí." % sim.VN(name),
					"stojící rmut se u horké stěny připaluje – připálenina zhorší chuť i přestup tepla a ohřev zpomalí",
					"nejdřív pusť míchadlo, pak otevři páru; při vypínání obráceně")
		elif _nostir.get(name, 0.0) > 0.0: _nostir[name] = 0.0
		elif _nostir.get(name, 0.0) < 0.0: _nostir[name] = minf(0.0, _nostir[name] + dt)
		if name == "rvp" and S.malt.get("on", false) and S.malt.get("gate", false) and not v.get("stir", false) and v.w > 50:
			if not S.get("_vystir_warn", false):
				S._vystir_warn = true
				_bad("vystirani", "Sypeš slad do pánve a míchadlo stojí.",
					"slad se nerozmíchá – vzniknou hrudky se suchým středem, ztratíš extrakt a enzymy se nedostanou ke škrobu",
					"pusť míchadlo dřív, než otevřeš šoupě násypky")
