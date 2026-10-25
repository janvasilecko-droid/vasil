extends RefCounted
## Další procesní vrstvy trenažéru: chemie vody a pH rmutu, kvašení ve spilce (diacetyl), sanitace CIP,
## nácvik poruch a drobné provozní výstrahy, hodnocení várky. Všechno zjednodušené modely s přiznanými předpoklady.

var simulation_ref: WeakRef
var sim:
	get: return simulation_ref.get_ref()

func _init(simulation) -> void:
	simulation_ref = weakref(simulation)

const WATERS := [
	{"name": "řad (odhad – zadej rozbor)", "alk": 100.0, "ca": 50.0, "mg": 10.0},
	{"name": "měkká (plzeňská)", "alk": 15.0, "ca": 8.0, "mg": 2.0},
	{"name": "tvrdá", "alk": 280.0, "ca": 110.0, "mg": 25.0},
]

func init_state() -> void:
	var S = sim.S
	if not S.has("water"): S.water = WATERS[0].duplicate()
	if not S.has("lactic"): S.lactic = 0.0                 # mL kyseliny mléčné 80 % do rmutu
	if not S.has("fer"): S.fer = {"on": false, "set": 9.0, "T": 9.0, "X": 0.0, "S0": 0.0, "Sf": 0.0, "E": 0.0, "AL": 0.0, "D": 0.0, "t": 0.0, "pitch": 15.0, "P0": 0.0}
	if not S.has("cip"): S.cip = {"run": false, "target": "rvp", "medium": "louh", "set": 80.0, "T": 20.0, "min": 0.0, "done": {}, "conc": 1.5}
	if not S.has("faults"): S.faults = {"on": false, "next": 0.0, "active": [], "nextMinor": 0.0}
	if not S.has("sensOff"): S.sensOff = 0.0

# ---------- pH rmutu: Kolbach – zbytková alkalita, tmavé slady, kyselina mléčná ----------
func residual_alk(w: Dictionary) -> float:
	return w.alk - (w.ca * 0.714 + w.mg * 0.585)          # mg/L jako CaCO3

func mash_ph() -> float:
	var S = sim.S; var gt := 0.0; var dark := 0.0
	for id in S.grist:
		gt += S.grist[id]
		var M: Dictionary = sim.MALTS.get(id, sim.MALTS.plz)
		if M.ebc > 30: dark += S.grist[id] * clampf(M.ebc / 300.0, 0.3, 2.0)
		elif M.unf > 0.2: dark += S.grist[id] * 0.3
	var df: float = dark / maxf(1.0, gt)
	var V: float = maxf(500.0, sim.liqVol(S.rvp) + sim.liqVol(S.sk.mash))
	# kyselina mléčná 80 %: ~11,8 mEq/mL, pufrační kapacita rmutu ~35 mEq/(kg sladu·pH)
	var malt: float = maxf(100.0, S.malt.added)
	var ph: float = 5.75 - 0.35 * df + 0.0017 * residual_alk(S.water) - S.lactic * 11.8 / (35.0 * malt)
	return clampf(ph, 4.6, 6.4)

## aktivita amyláz podle pH (optimum β ≈ 5,4, α ≈ 5,6)
func ph_factors(ph: float) -> Vector2:
	return Vector2(exp(-pow((ph - 5.7) / 0.8, 2)), exp(-pow((ph - 5.5) / 0.7, 2)))

# ---------- kvašení: Monod + teplotní aktivita, α-acetolaktát → diacetyl → redukce kvasinkami ----------
func fer_start() -> void:
	var S = sim.S; var sp = S.sp; var f = S.fer
	if sp.L < 100:
		sim.logm("Kvašení: ve spilce není mladina.", "warn"); return
	f.on = true; f.T = sp.H / maxf(1.0, sp.L); f.X = f.pitch * 0.04
	f.S0 = sp.f / sp.L * 1000.0; f.Sf = f.S0; f.E = 0.0; f.AL = 0.0; f.D = 0.0; f.t = 0.0
	f.P0 = 100.0 * (sp.f + sp.n) / maxf(1.0, sp.f + sp.n + sp.w)
	sim.logm("Zakvašeno: %s hl, %s °C, %d mil. buněk/mL, spilka chlazená na %s °C." % [sim.fmt1(sp.L / 100.0), sim.fmt1(f.T), roundi(f.pitch), sim.fmt1(f.set)])

func fer_step(dt: float) -> void:
	var S = sim.S; var f = S.fer
	if not f.on: return
	var h: float = dt / 3600.0
	var a: float = exp(0.095 * (f.T - 12.0))
	var mu: float = 0.05 * a * f.Sf / (8.0 + f.Sf) * (1.0 - f.X / 4.0)
	var dS: float = minf(f.Sf, (0.2 * a * f.X * f.Sf / (6.0 + f.Sf)) * h)
	f.X += maxf(0.0, mu * f.X * h)
	f.Sf -= dS; f.E += dS / 2.0665; f.CO2 = f.get("CO2", 0.0) + dS * 0.9565 / 2.0665; f.XB = f.get("XB", 0.0) + dS * 0.11 / 2.0665   # Balling
	f.T += dS * 0.6 * 1000.0 / 4100.0 - (f.T - f.set) * minf(1.0, 0.25 * h)
	f.AL += mu * f.X * h * 0.9
	var k1: float = 0.012 * exp(0.11 * (f.T - 12.0))
	var conv: float = f.AL * minf(1.0, k1 * h); f.AL -= conv; f.D += conv * 1000.0
	f.D -= f.D * minf(1.0, 0.06 * a * minf(1.0, f.X / 1.5) * h)
	f.t += dt
	if f.D < 100 and f.Sf < 0.25 * f.S0: sim.once("dia_ok", "Diacetyl %d ppb – pod prahem chuti (≈ 100 ppb), hlavní kvašení končí. Můžeš chladit na ležení." % roundi(f.D))
	if f.Sf < 0.3 * f.S0 and f.D > 150 and f.set < 10: sim.once("dia_rest", "Diacetyl %d ppb je vysoko a kvašení končí – zvaž diacetylovou pauzu (12–14 °C na 1–2 dny)." % roundi(f.D), "warn")

func fer_skip(hours: float) -> void:
	for i in int(hours * 60.0): fer_step(60.0)

func app_extract() -> float:
	var f = sim.S.fer
	if f.P0 <= 0: return 0.0
	return f.P0 - (f.S0 - f.Sf) / 10.0 - 0.46 * f.E / 10.0

# ---------- CIP: teplota roztoku, čas, médium ----------
const CIP_TARGETS := [["rvp", "Pánev"], ["sk", "Scezovací káď"], ["wp", "Vířivá káď"], ["plate", "Deskový chladič"], ["spilka", "Spilka"]]
const CIP_MEDIA := [["voda", "proplach vodou"], ["louh", "louh (NaOH) ≥ 75 °C"], ["kyselina", "kyselina ≤ 60 °C"]]

func cip_name(t: String) -> String:
	for x in CIP_TARGETS: if x[0] == t: return x[1]
	return t

func cip_step(dt: float) -> void:
	var S = sim.S; var c = S.cip
	if not c.run: return
	var m: float = dt / 60.0
	c.T += clampf(c.set - c.T, -1.5 * m, 1.2 * m)      # CIP stanice ohřívá ~1,2 K/min (modelový předpoklad)
	c.min += m
	var ok := false
	match c.medium:
		"louh":
			ok = c.T >= 75.0 and c.get("conc", 1.5) >= sim.S.cfg.get("cipLyeMin", 1.0)
			if c.get("conc", 1.5) < sim.S.cfg.get("cipLyeMin", 1.0) and c.min > 2: sim.once("lyeweak%d" % sim.tb(600), "CIP: louh má jen %s %% NaOH – pod %s %% nerozpustí úsady. Doplň koncentrát." % [sim.fmt1(c.get("conc", 1.5)), sim.fmt1(sim.S.cfg.get("cipLyeMin", 1.0))], "warn")
			if c.T < 75.0 and c.min > 5: sim.once("lyecold%s%d" % [c.target, sim.tb(600)], "CIP: louh má jen %d °C – pod 75 °C nerozpustí bílkovinné úsady a pivní kámen." % roundi(c.T), "warn")
			if c.T > 90.0: sim.once("lyehot%d" % sim.tb(600), "CIP: louh nad 90 °C – zbytečně vysoká teplota, hrozí opaření a poškození těsnění.", "warn")
		"kyselina":
			ok = c.T >= 20.0 and c.T <= 60.0 and c.get("conc", 1.0) >= sim.S.cfg.get("cipAcidMin", 0.5)
			if c.get("conc", 1.0) < sim.S.cfg.get("cipAcidMin", 0.5) and c.min > 2: sim.once("acidweak%d" % sim.tb(600), "CIP: kyselina má jen %s %% – pod %s %% neodstraní pivní kámen." % [sim.fmt1(c.get("conc", 1.0)), sim.fmt1(sim.S.cfg.get("cipAcidMin", 0.5))], "warn")
			if c.T > 60.5: sim.once("acidhot%d" % sim.tb(600), "CIP: kyselina nad 60 °C – koroze nerezu a výpary. Uber teplotu.", "bad")
		"voda": ok = true
	var key: String = c.target + ":" + c.medium
	if ok: c.done[key] = c.done.get(key, 0.0) + m
	var need: float = 20.0 if c.medium != "voda" else 5.0
	if c.done.get(key, 0.0) >= need and not c.done.has(key + "!"):
		c.done[key + "!"] = 1
		var nxt: String = {"voda": "pokračuj louhem", "louh": "proplach vodou a kyselina", "kyselina": "proplach vodou, nádoba je čistá"}.get(c.medium, "")
		sim.logm("CIP %s: %s %d min hotovo ✔ – %s." % [cip_name(c.target), c.medium, roundi(need), nxt])

func cip_status() -> String:
	var c = sim.S.cip; var parts := []
	for t in CIP_TARGETS:
		var st := ""
		for md in ["voda", "louh", "kyselina"]:
			if c.done.has(t[0] + ":" + md + "!"): st += {"voda": "V", "louh": "L", "kyselina": "K"}[md]
		if st != "": parts.append("%s %s" % [t[1], st])
	return ("hotovo: " + ", ".join(PackedStringArray(parts))) if parts.size() else "zatím nic nesanitováno"

# ---------- nácvik poruch a drobné provozní výstrahy ----------
const MINOR := [
	"Ucpávka C1 lehce kape – sleduj, při zvětšení vyměnit.",
	"Tlak vody z řadu kolísá (2,5–3,5 bar).",
	"Kondenzátní hrnec u pánve syčí – zkontroluj odvod kondenzátu.",
	"Teploměr pánve ukazuje o 1,5 °C víc než kontrolní teploměr – ověř kalibraci.",
	"Na podlaze u vířivky je kaluž – zkontroluj šroubení a sání C3.",
	"Ventilátor rozvaděče RT.20 je hlučný – zkontroluj filtr.",
	"Hladinoznak bojleru se zasekává – poklepni / vyčisti.",
]

func faults_step(dt: float) -> void:
	var S = sim.S; var F = S.faults
	if not F.on: return
	if F.next <= 0: F.next = S.t + randf_range(1800, 4800)
	if F.nextMinor <= 0: F.nextMinor = S.t + randf_range(900, 2400)
	if S.t >= F.nextMinor:
		F.nextMinor = S.t + randf_range(1500, 3600)
		var i: int = randi() % MINOR.size()
		sim.logm("ℹ " + MINOR[i], "warn")
		if i == 3: S.sensOff = 1.5
	if S.t < F.next: return
	F.next = S.t + randf_range(2400, 6000)
	var k: String = ["air", "steam", "plate", "valve"][randi() % 4]
	match k:
		"air":
			var cand := []
			for p in ["p1", "p2", "p3"]: if S[p].on and S[p].q > 5: cand.append(p)
			if cand.size():
				var p: String = cand[randi() % cand.size()]; S[p].airLock = true; S[p].prime = 0.05
				F.active.append({"k": "air", "p": p, "t": S.t}); sim.logm("⚠ %s: průtok náhle spadl, čerpadlo hučí naprázdno – zavzdušnilo se." % sim.main.pump_name(p), "bad")
		"steam":
			if not S.has("steamBar0"): S.steamBar0 = S.cfg.steamBar
			S.cfg.steamBar = 1.0; F.active.append({"k": "steam", "t": S.t})
			sim.logm("⚠ Kotel: tlak páry klesá na 1,0 bar – ohřev zpomalil.", "bad")
		"plate":
			if not S.has("UA10"): S.UA10 = S.cfg.UA1
			S.cfg.UA1 = S.UA10 * 0.45; F.active.append({"k": "plate", "t": S.t})
			sim.logm("⚠ Deskový chladič: mladina za chladičem je teplejší než obvykle při stejném průtoku…", "warn")
		"valve":
			var vs := []
			for v in sim.main.items: if v.type == "valve" and v.get("kind", "") == "klapka" and not v.get("stuck", false): vs.append(v)
			if vs.size():
				var v: Dictionary = vs[randi() % vs.size()]; v.stuck = true; v.stuckOpen = v.get("open", true) != false
				F.active.append({"k": "valve", "id": v.id, "t": S.t})

func fault_fix(k: String) -> String:
	var S = sim.S; var F = S.faults; var msg := ""
	for f in F.active.duplicate():
		if f.k != k: continue
		match k:
			"air":
				S[f.p].airLock = false; msg = "%s odvzdušněno – čerpadlo znovu nasaje." % sim.main.pump_name(f.p)
			"steam":
				S.cfg.steamBar = S.get("steamBar0", 2.5); msg = "Kotel: odkalení a reset hořáku – tlak páry zpět na %s bar." % sim.fmt1(S.cfg.steamBar)
			"plate":
				S.cfg.UA1 = S.get("UA10", S.cfg.UA1); msg = "Chladič rozebrán a vyčištěn – přestup tepla obnoven."
			"valve":
				for v in sim.main.items: if v.id == f.id: v.erase("stuck")
				msg = "Zadřená klapka uvolněna a promazána."
		F.active.erase(f)
		S.faults.solved = S.faults.get("solved", 0) + 1
		sim.logm("✔ " + msg + " (za %d min)" % roundi((S.t - f.t) / 60.0))
	if k == "air":
		for p in ["p1", "p2", "p3"]:
			if S[p].get("airLock", false):
				S[p].airLock = false; msg = "%s odvzdušněno." % sim.main.pump_name(p); sim.logm("✔ " + msg)
	if k == "sensor" and S.sensOff != 0.0:
		S.sensOff = 0.0; msg = "Teploměr pánve zkalibrován."; sim.logm("✔ " + msg)
	return msg

func faults_text() -> String:
	var F = sim.S.faults
	if not F.on: return "vypnuto"
	var n: int = F.active.size()
	return "zapnuto · aktivní poruchy: %d · vyřešeno: %d" % [n, F.get("solved", 0)]

# ---------- hodnocení várky ----------
func scorecard() -> Array:
	var S = sim.S; var c = S.cfg; var rep = sim.spReport(); var out := []
	var sc := func(label: String, val: String, target: String, pts: float) -> void: out.append([label, val, target, clampf(pts, 0.0, 100.0)])
	if rep != null:
		sc.call("Objem mladiny", "%s hl" % sim.fmt1(rep.V), "%s hl" % sim.fmt1(c.targetV), 100.0 - absf(rep.V - c.targetV) / c.targetV * 400.0)
		sc.call("Stupňovitost", "%s °P" % sim.fmt1(rep.P), "%s °P" % sim.fmt1(c.targetP), 100.0 - absf(rep.P - c.targetP) * 60.0)
		sc.call("Hořkost", "%d IBU" % roundi(rep.IBU), "%d IBU" % roundi(c.targetIBU), 100.0 - absf(rep.IBU - c.targetIBU) * 6.0)
		sc.call("Barva", "%s EBC" % sim.fmt1(rep.EBC), "%s EBC" % sim.fmt1(c.targetEBC), 100.0 - absf(rep.EBC - c.targetEBC) * 12.0)
		sc.call("DMS", "%d ppb" % roundi(rep.dms), "< 60 ppb", 100.0 - maxf(0.0, rep.dms - 60.0) * 2.0)
		sc.call("Zákaly (průměr sladiny)", "%d" % roundi(S.turbSum / maxf(1.0, S.turbVol)), "< 20", 100.0 - maxf(0.0, S.turbSum / maxf(1.0, S.turbVol) - 20.0) * 3.0)
		sc.call("Teplota zakvašení", "%s °C" % sim.fmt1(rep.T), "%s °C" % sim.fmt1(c.pitchT), 100.0 - absf(rep.T - c.pitchT) * 25.0)
		sc.call("Výtěžnost do spilky (extrakt ve spilce / slad)", "%s %%" % sim.fmt1(rep.yield), "≥ 68 %", 100.0 - maxf(0.0, 68.0 - rep.yield) * 8.0)
		if rep.has("VV_varna"):
			sc.call("Výtěžnost varny (extrakt ve vyrážce / slad)", "%s %%  (provozně z objemu: %s %%)" % [sim.fmt1(rep.VV_varna), sim.fmt1(rep.VV_cz)], "≥ 72 %", 100.0 - maxf(0.0, 72.0 - rep.VV_varna) * 8.0)
			sc.call("Využití laboratorního extraktu", "%s %%" % sim.fmt1(rep.ucinnost), "≥ 92 %", 100.0 - maxf(0.0, 92.0 - rep.ucinnost) * 6.0)
	var lost: float = S.drainL + S.spillL
	sc.call("Ztráty do kanálu", "%d L" % roundi(lost), "0 L", 100.0 - lost / 10.0)
	var bad := 0
	for l in S.log: if l.type == "bad": bad += 1
	sc.call("Chyby obsluhy (červená hlášení)", "%d" % bad, "0", 100.0 - bad * 12.0)
	var pr: Dictionary = S.get("practice", {"n": 0, "rules": {}})
	sc.call("Provozní zásady (pořadí klapek, čerpadel, páry)", "%d porušení" % int(pr.n), "0", 100.0 - int(pr.n) * 10.0)
	sc.call("Prodleva 52 °C / 63 °C / 72 °C", "%d / %d / %d min" % [roundi(S.flags.rest52), roundi(S.flags.r63 + S.flags.r63b), roundi(S.flags.r72 + S.flags.r72b)], "15 / 40 / 40 min", 100.0 - absf(S.flags.rest52 - 15.0) * 3.0 - absf(S.flags.r63 + S.flags.r63b - 40.0) * 1.5 - absf(S.flags.r72 + S.flags.r72b - 40.0) * 1.5)
	var ra: Dictionary = S.get("restAcc", {"in": 0.0, "tot": 0.0, "dev": 0.0})
	if ra.tot > 60.0:
		var pin: float = 100.0 * ra["in"] / ra.tot
		sc.call("Přesnost prodlev (±0,5 °C)", "%d %% času, průměrná odchylka %s °C" % [roundi(pin), sim.fmt1(ra.dev / ra.tot)], "≥ 80 %", pin * 1.15)
	var cd: Dictionary = S.get("cip", {}).get("done", {})
	var lye_ok: = false
	for k in cd: if String(k).ends_with(":louh!"): lye_ok = true
	sc.call("Sanitace – horký louh ≥ 75 °C 20 min", "splněno" if lye_ok else "neprovedeno", "splněno", 100.0 if lye_ok else 0.0)
	var hrs: float = S.t / 3600.0
	sc.call("Časová efektivita várky", "%s h" % sim.fmt1(hrs), "≤ 10 h", 100.0 - maxf(0.0, hrs - 10.0) * 15.0 if rep != null else 50.0)
	return out

## přesnost teplotních prodlev: podíl času v toleranci ±0,5 °C od cíle (52 / 63 / 72 °C) během kroků prodlev
const REST_T := {"rest52": 50.0, "rest63a": 63.0, "rest63b": 63.0, "rest72a": 72.0, "rest72b": 72.0}
func _rest_acc(dt: float) -> void:
	var S = sim.S
	if not S.has("restAcc"): S.restAcc = {"in": 0.0, "tot": 0.0, "dev": 0.0}
	var cur: Dictionary = sim.training.current()
	var id: String = cur.get("id", "")
	if not REST_T.has(id) or sim.vol(S.rvp) < 300: return
	var d: float = absf(S.rvp.Tsens - REST_T[id])
	S.restAcc.tot += dt; S.restAcc.dev += d * dt
	if d <= 0.5: S.restAcc["in"] += dt

func scorecard_text() -> String:
	var rows := scorecard(); var tot := 0.0; var lines := []
	for r in rows:
		tot += r[3]
		var col := "#7ee08a" if r[3] >= 80 else ("#f2c230" if r[3] >= 50 else "#ff6b5b")
		lines.append("[color=%s]%d[/color]  %s: %s [color=#7b858e](cíl %s)[/color]" % [col, roundi(r[3]), r[0], r[1], r[2]])
	var avg: float = tot / maxf(1, rows.size())
	var grade: String = "výborně" if avg >= 85 else ("dobře" if avg >= 70 else ("dostatečně" if avg >= 50 else "nedostatečně"))
	return "[b]Celkem %d / 100 – %s[/b]\n" % [roundi(avg), grade] + "\n".join(PackedStringArray(lines))

func step(dt: float) -> void:
	init_state()
	var S = sim.S
	if S.malt.added > 1:
		var ph: float = mash_ph()
		S.mashPH = ph; S.pH = ph; S.phF = ph_factors(ph)
		if (ph < 5.2 or ph > 5.8) and S.malt.added > 100 and S.sk.mode == "mash" and (S.rvp.g > 20 or S.sk.mash.g > 20): sim.once("ph%d" % sim.tb(1800), "pH rmutu %s je mimo optimum 5,4–5,6 – amylázy pracují pomaleji, zcukření se protáhne. Uprav vodu nebo kyselinu mléčnou." % sim.fmt1(ph), "warn")
	fer_step(dt); cip_step(dt); faults_step(dt); _rest_acc(dt)
	if S.sp.L > 100 and not S.fer.on and S.flags.get("pushed", 0) and S.p3.q < 1: fer_start()
