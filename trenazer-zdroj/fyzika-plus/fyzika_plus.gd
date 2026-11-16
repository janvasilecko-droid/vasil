extends RefCounted
## FYZIKA+ – zpřesnění fyziky a chemie trenažéru varny (balíček z 29. 10. 2026).
## Všechno počítání je v tomhle souboru; sim.gd ho jen volá v místech označených „FYZIKA+“.
## Zdroje a odhady jsou popsané ve FYZIKA_PLUS.md. ODHAD = technický odhad, není změřený na varně Zajíc
## (každý parametr jde přepsat v kalibrace.json stejně jako ostatní parametry).
##
##  1) pára do pláště: ventil (Kv, charakteristika) + rovnováha tlaku v plášti + vzduch + zaplavení kondenzátem (stall)
##  2) přestup tepla na straně náplně: míchání (Chilton–Drew–Jebens) a přirozená konvekce bez míchadla
##  3) pH mladiny: pokles za varu, kyselina mléčná do mladiny (pufrační kapacita podle Kunzeho)
##  4) barva (Maillard): Arrhenius, pH, extrakt – i při dekokci, ve vířivce a během chlazení
##  5) vířivka: rychlost tangenciálního nátoku (max. 3,5 m/s), slabá rotace / tříštění lomu
##  6) chladnutí stojícího obsahu trubek podle průměru, oceli a součinitele prostupu
##  7) tepelná roztažnost vody podle Kella (místo lineárního 0,00052/K)

const VERZE := "FYZIKA+ 1.0 (2026-10-29)"

## výchozí hodnoty nových parametrů (stejné řádky jsou v CFG_DEFAULT; tady jako pojistka, kdyby v něm chyběly)
const DEF := {
	"stValveR": 30.0, "rvpKvs": 0.0, "mpKvs": 0.0, "stCondBack": 0.15, "jWaterKg": 150.0, "jDrainKgS": 0.6,
	"jAirTau": 20.0, "jAirInTau": 15.0, "jVacBreaker": 1.0, "hNatC": 0.28,
	"colK100": 1.2, "colEa": 77.0, "colPh": 0.7, "colMashK": 1.0,
	"phBoilDrop": 0.08, "phSparge": 0.05, "wortBuf": 26.6, "isoPh": 0.5,
	"wpInletMM": 0.0, "wpVmax": 3.5, "wpVmin": 1.0, "wpShearK": 0.1, "pipeU": 10.0,
}
## povolené rozsahy (pro CALIB_RANGE)
const RANGE := {
	"stValveR": [1.0, 50.0], "rvpKvs": [0.0, 200.0], "mpKvs": [0.0, 200.0], "stCondBack": [0.0, 2.0], "jWaterKg": [20.0, 1000.0], "jDrainKgS": [0.05, 5.0],
	"jAirTau": [2.0, 600.0], "jAirInTau": [2.0, 600.0], "jVacBreaker": [0.0, 1.0], "hNatC": [0.05, 1.5],
	"colK100": [0.0, 10.0], "colEa": [40.0, 150.0], "colPh": [0.0, 3.0], "colMashK": [0.0, 5.0],
	"phBoilDrop": [0.0, 0.4], "phSparge": [-0.2, 0.4], "wortBuf": [5.0, 80.0], "isoPh": [0.0, 2.0],
	"wpInletMM": [0.0, 150.0], "wpVmax": [1.0, 8.0], "wpVmin": [0.2, 5.0], "wpShearK": [0.0, 1.0], "pipeU": [0.5, 30.0],
}

static func cf(sim, k: String) -> float:
	return float(sim.S.cfg.get(k, DEF.get(k, 0.0)))

static func f2(x: float) -> String:
	return ("%.2f" % x).replace(".", ",")


# ======================================================================
# Voda a pára
# ======================================================================

## hustota vody [kg/m³], Kell (1975), 0–150 °C
static func water_rho(T: float) -> float:
	var t: float = clampf(T, 0.0, 150.0)
	var n: float = 999.83952 + t * (16.945176 + t * (-7.9870401e-3 + t * (-46.170461e-6 + t * (105.56302e-9 - t * 280.54253e-12))))
	return n / (1.0 + 16.879850e-3 * t)

## objem kapaliny při T vůči objemu při 20 °C (mladina se roztahuje prakticky stejně jako voda)
static func expansion(T: float) -> float:
	return water_rho(20.0) / water_rho(T)

## tlak sytých par vody [bar abs.] a teplota sytosti [°C] – IAPWS-IF97, oblast 4 (rovnice 30 a 31; přesnost parních tabulek,
## platí i pro podtlak; 0,01–373 °C)
const _N := [0.11670521452767e4, -0.72421316703206e6, -0.17073846940092e2, 0.12020824702470e5, -0.32325550322333e7,
	0.14915108613530e2, -0.48232657361591e4, 0.40511340542057e6, -0.23855557567849, 0.65017534844798e3]
static func psat(T: float) -> float:
	var Tk: float = clampf(T, 0.01, 373.0) + 273.15
	var th: float = Tk + _N[8] / (Tk - _N[9])
	var A: float = th * th + _N[0] * th + _N[1]
	var B: float = _N[2] * th * th + _N[3] * th + _N[4]
	var C: float = _N[5] * th * th + _N[6] * th + _N[7]
	return pow(2.0 * C / (-B + sqrt(B * B - 4.0 * A * C)), 4) * 10.0

static func tsat(p: float) -> float:
	var be: float = pow(clampf(p, 0.0061166, 220.0) / 10.0, 0.25)
	var E: float = be * be + _N[2] * be + _N[5]
	var F: float = _N[0] * be * be + _N[3] * be + _N[6]
	var G: float = _N[1] * be * be + _N[4] * be + _N[7]
	var D: float = 2.0 * G / (-F - sqrt(F * F - 4.0 * E * G))
	var Tk: float = (_N[9] + D - sqrt((_N[9] + D) * (_N[9] + D) - 4.0 * (_N[8] + _N[9] * D))) / 2.0
	return Tk - 273.15

## výparné teplo vody [kJ/kg] – tvar Watsonovy rovnice, exponent proložený hodnotami IAPWS-IF97 (20–160 °C, chyba < 0,1 %):
## 2453,5 (20 °C), 2256,5 (100 °C), 2147,7 (138,9 °C), 2081,9 (160 °C)
static func hfg(T: float) -> float:
	var t: float = clampf(T, 0.0, 370.0)
	return 2256.5 * pow((647.096 - (t + 273.15)) / (647.096 - 373.15), 0.325)


# ======================================================================
# 1) Pára do pláště: ventil × kondenzace, vzduch, kondenzát
# ======================================================================

## podíl Kvs při zdvihu x (0–1). R = rozsah regulace (rangeability): 1 = lineární ventil (ruční), 30–50 = ekviprocentní
## regulační ventil (Spirax Sarco doporučuje pro ohřev párou ekviprocentní). Ekviprocentní: Kv/Kvs = R^(x − 1);
## pod 5 % zdvihu lineárně do nuly (ventil těsně uzavře). Výchozí R 30 (ODHAD – typ ventilu u Vasila neznám).
static func kv_frac(x: float, R: float) -> float:
	var xx: float = clampf(x, 0.0, 1.0)
	if xx <= 0.0: return 0.0
	if R <= 1.01: return xx
	var x0: float = 0.05
	if xx < x0: return pow(R, x0 - 1.0) * xx / x0
	return pow(R, xx - 1.0)

## průtok syté páry ventilem [kg/s] – Spirax Sarco: ṁ = 12·Kv·P1·√(1 − 5,67·(0,42 − χ)²) [kg/h], χ = (P1 − P2)/P1,
## kritický (ucpaný) tok od χ = 0,42 (P2 pod 58 % P1): ṁ = 12·Kv·P1. Tlaky v bar abs.
static func steam_kgs(Kv: float, p1: float, p2: float) -> float:
	if Kv <= 0.0 or p1 <= 0.0 or p2 >= p1: return 0.0
	var chi: float = minf(0.42, (p1 - p2) / p1)
	return 12.0 * Kv * p1 * sqrt(maxf(0.0, 1.0 - 5.67 * (0.42 - chi) * (0.42 - chi))) / 3600.0

## Tlak v plášti: ventil propustí právě tolik páry, kolik jí plášť zkondenzuje (UA·(T_sat(p_páry) − T) = ṁ·h_fg).
## y = podíl vzduchu v plášti (Dalton: tlak páry = celkový tlak × (1 − y), pára je chladnější).
## Průtok ventilem s rostoucím tlakem v plášti klesá, kondenzace roste → jediný průsečík, hledá se půlením.
## Vrací p (celkový tlak v plášti, bar abs.), ps (tlak páry), kgs (pára kg/s), P (kW), Tj (teplota kondenzace).
static func jacket_solve(Kv: float, p1: float, UA: float, T: float, y: float = 0.0) -> Dictionary:
	var yy: float = clampf(y, 0.0, 0.999)
	var plo: float = psat(T) / (1.0 - yy)                 # pod tímto tlakem pára na náplň nekondenzuje
	if Kv <= 0.0 or p1 <= 0.0:
		return {"p": plo, "ps": plo * (1.0 - yy), "kgs": 0.0, "P": 0.0, "Tj": T}
	if UA <= 1e-6 or p1 <= plo + 1e-4:
		return {"p": p1, "ps": p1 * (1.0 - yy), "kgs": 0.0, "P": 0.0, "Tj": T}   # pára nemá kde kondenzovat → plášť má tlak kotle
	var lo: float = plo; var hi: float = p1
	for i in 40:
		var mid: float = 0.5 * (lo + hi)
		var Tj: float = tsat(mid * (1.0 - yy))
		var mc: float = UA * maxf(0.0, Tj - T) / hfg(Tj)
		if steam_kgs(Kv, p1, mid) > mc: lo = mid
		else: hi = mid
	var p2: float = 0.5 * (lo + hi)
	var Tj2: float = tsat(p2 * (1.0 - yy))
	var P: float = UA * maxf(0.0, Tj2 - T)
	return {"p": p2, "ps": p2 * (1.0 - yy), "kgs": P / hfg(Tj2), "P": P, "Tj": Tj2}

## Kvs dopočítané z přestupu pláště: plně otevřený ventil při náplni 60 °C ztratí 10 % tlaku (Spirax: pod 10 % už
## regulace „plave“). Plně otevřený ventil tak dá skoro stejný výkon jako dřívější model (UA·(T_páry − T)).
static func kvs_auto(UA: float, steam_bar_g: float) -> float:
	var p1: float = maxf(0.3, steam_bar_g) + 1.013
	var Tj: float = tsat(0.9 * p1)
	var kgs: float = UA * maxf(1.0, Tj - 60.0) / hfg(Tj)
	return kgs * 3600.0 / (12.0 * p1 * sqrt(1.0 - 5.67 * 0.32 * 0.32))

static func _vn(sim, nm: String) -> String:
	return sim.VN(nm) if sim.has_method("VN") else nm

## Náhrada za sim._pd(): výkon páry do pláště [kW] a smáčená část pláště.
## Stav pláště se drží v nádobě: jp (celkový tlak v plášti, bar abs.), jy (podíl vzduchu), jfl (zaplavení kondenzátem),
## jkgs (pára kg/s), jkv (ventil otevřený).
static func steam_pd(sim, v, UA_st: float, jL: float, _Tst: float) -> Dictionary:
	var S = sim.S; var c = S.cfg
	var nm: String = "rvp" if is_same(v, S.rvp) else ("mp" if is_same(v, S.mp) else "x")
	var V: float = sim.liqVol(v) + sim.sol(v) * 0.65
	var wet: float = clampf((V - 100.0) / maxf(1.0, jL - 100.0), 0.0, 1.0)
	var now: float = float(S.t)
	var dt: float = clampf(now - float(v.get("jt", now)), 0.0, 60.0)     # délka minulého kroku (sim_step ji sem nepředává)
	v["jt"] = now
	var pb: float = float(S.boiler.get("p", c.get("steamBar", 2.5)))
	var p1: float = (pb + 1.013) if (S.boiler.get("on", true) or pb > 0.05) else 0.0
	var kvs: float = cf(sim, nm + "Kvs") if nm != "x" else 0.0
	# ventil se nemění, když kotel ztratí tlak (porucha „pára“ přepíše steamBar) → navrhovat z původního tlaku
	if kvs <= 0.0: kvs = kvs_auto(UA_st, float(S.get("steamBar0", c.get("steamBar", 2.5))))
	var x: float = clampf(float(v.get("heat", 0.0)), 0.0, 1.0)
	var Kv: float = kvs * kv_frac(x, cf(sim, "stValveR"))
	# parní trasa v plánku zavřená (klapky) → do pláště pára nejde, kotel ji nevyrábí
	if not bool(S.get("steamOK", {}).get(nm, true)): Kv = 0.0
	var atm: float = 1.013
	var breaker: bool = cf(sim, "jVacBreaker") >= 0.5
	var pback: float = atm + cf(sim, "stCondBack")
	var M: float = maxf(1.0, cf(sim, "jWaterKg"))
	# 1) stav pláště se posune o uplynulý krok podle podmínek z minulého kroku
	var air: float = clampf(float(v.get("jy", 0.0)), 0.0, 1.0)
	var fl: float = clampf(float(v.get("jfl", 0.0)), 0.0, 0.95)
	if dt > 0.0 and v.has("jpt"):
		var pto: float = float(v.jpt); var kgo: float = float(v.get("jkgs", 0.0)); var kvo: bool = bool(v.get("jkv", false))
		if breaker and not kvo and pto < atm - 0.01:
			# pára zavřená, zbytek páry v plášti zkondenzoval → zavzdušňovací ventil pustí do pláště vzduch
			var tgt: float = clampf(1.0 - pto * (1.0 - air) / atm, 0.0, 1.0)     # tolik vzduchu, aby celkový tlak byl atmosférický
			air += (tgt - air) * (1.0 - exp(-dt / maxf(1.0, cf(sim, "jAirInTau"))))
		elif kvo and pto > atm + 0.02:
			# ventil otevřený a v plášti přetlak → pára vytlačuje vzduch odvzdušňovačem (tím rychleji, čím vyšší přetlak)
			air *= exp(-dt / maxf(0.5, cf(sim, "jAirTau") / clampf(pto - atm, 0.1, 2.0)))
		# kondenzát: přitéká, co se zkondenzovalo; odvaděč odvede víc, čím větší je rozdíl tlaků nad protitlakem
		var cap: float = cf(sim, "jDrainKgS") * sqrt(clampf((pto - pback) / 0.5, 0.0, 1.0))
		fl = clampf(fl + (kgo - cap) * dt / M, 0.0, 0.95)
	# 2) přestup: pára + stěna (gs, z kalibrovaného rvpUAst) v sérii s náplní (hA – míchání, viskozita, přirozená konvekce)
	var href: float = sim.ha_ref() if sim.has_method("ha_ref") else maxf(25.0, 2.0 * UA_st)
	var gs: float = 1.0 / maxf(1e-6, 1.0 / UA_st - 1.0 / href)
	var hA: float = sim.film_hA(v)
	var UA: float = wet * (1.0 - fl) / ((1.0 + 100.0 * air) / gs + 1.0 / maxf(1e-4, hA))   # 1 % vzduchu ≈ poloviční přestup na straně páry (ODHAD)
	# 3) tlak v plášti a výkon v tomto kroku
	var J: Dictionary = jacket_solve(Kv, p1, UA, float(v.T), air)
	var ptot: float = float(J.p)
	var kgs: float = float(J.kgs)
	var P: float = float(J.P)
	if breaker and Kv > 0.0 and p1 > atm + 0.05 and ptot < atm - 0.005:
		# přiškrcená pára: tlak by spadl pod atmosférický → zavzdušňovací ventil přimíchá tolik vzduchu,
		# aby plášť držel ~1 bar abs. a kondenzace se rovnala průtoku ventilem (rovnováha, nezávisí na délce kroku)
		var mv: float = steam_kgs(Kv, p1, atm)
		var lo: float = 0.0; var hi: float = 0.999
		for i in 40:
			var yv: float = 0.5 * (lo + hi)
			var Tjy: float = tsat(atm * (1.0 - yv))
			var UAy: float = wet * (1.0 - fl) / ((1.0 + 100.0 * yv) / gs + 1.0 / maxf(1e-4, hA))
			if UAy * maxf(0.0, Tjy - float(v.T)) / hfg(Tjy) > mv: lo = yv
			else: hi = yv
		air = 0.5 * (lo + hi)
		var Tje: float = tsat(atm * (1.0 - air))
		var UAe: float = wet * (1.0 - fl) / ((1.0 + 100.0 * air) / gs + 1.0 / maxf(1e-4, hA))
		P = UAe * maxf(0.0, Tje - float(v.T))
		kgs = P / hfg(Tje)
		ptot = atm
	# hlášení obsluze
	var x0: float = float(v.get("jx", x))
	v["jx"] = x
	if nm != "x":
		if fl > 0.25 and x - x0 > 0.3 and dt <= 5.0:
			sim.once("fphammer%s%d" % [nm, sim.tb(300)], "%s: RÁZ V PLÁŠTI – pára vpuštěná naráz do pláště zaplaveného kondenzátem (%d %%). Páru otevírej plynule a nech kondenzát nejdřív odtéct." % [_vn(sim, nm), roundi(fl * 100.0)], "bad")
		if kgs > 0.0 and ptot < pback and wet > 0.0:
			v["jst"] = float(v.get("jst", 0.0)) + dt
			if float(v.jst) > 120.0:
				sim.once("fpstall%s%d" % [nm, sim.tb(1800)], "%s: tlak v plášti %s bar abs. je pod protitlakem kondenzátu %s bar – kondenzát neodtéká (stall) a plášť se zaplavuje (%d %%). U přiškrcené páry na chladnou náplň je to běžné; pak páru otevírej pomalu, ať nevzniknou rázy." % [_vn(sim, nm), f2(ptot), f2(pback), roundi(fl * 100.0)], "warn")
		else: v["jst"] = 0.0
		if not breaker and ptot < 0.5 and float(v.get("jUsed", 0.0)) > 0.0 and Kv <= 0.0:
			sim.once("fpvac%s%d" % [nm, sim.tb(1800)], "%s: v plášti je podtlak %s bar abs. a plášť nemá zavzdušňovací ventil – po zavření páry otevři odvzdušnění pláště, jinak hrozí zborcení dna (Kunze s. 290–291)." % [_vn(sim, nm), f2(ptot)], "bad")
		if air > 0.3 and Kv > 0.0 and wet > 0.0:
			sim.once("fpair%s%d" % [nm, sim.tb(1800)], "%s: plášť je zavzdušněný – pára nejdřív vytlačí vzduch odvzdušňovačem, ohřev chvíli nejede naplno." % _vn(sim, nm), "info")
	if kgs > 0.0: v["jUsed"] = 1.0
	v["jy"] = air
	v["jfl"] = fl
	v["jp"] = ptot
	v["jpt"] = ptot
	v["jkgs"] = kgs
	v["jkv"] = Kv > 0.0
	return {"P": P, "wet": wet}

## text pro panel: tlak v plášti pánve (přetlak, jak ho ukáže manometr)
static func jacket_label(sim) -> String:
	var v = sim.S.rvp
	if not v.has("jp"): return ""
	var bg: float = float(v.jp) - 1.013
	var t: String = "   plášť pánve %s bar" % sim.fmt1(bg)
	if float(v.get("jfl", 0.0)) > 0.05: t += ", kondenzát %d %%" % roundi(float(v.jfl) * 100.0)
	if float(v.get("jy", 0.0)) > 0.05: t += ", vzduch"
	return t


# ======================================================================
# 2) Přestup tepla na straně náplně
# ======================================================================

## Součinitel přestupu náplň ↔ stěna [kW/K] (náhrada za sim.film_hA).
## Míchaná náplň: Nu ∝ Re^(2/3)·Pr^(1/3) (Chilton–Drew–Jebens) → h ∝ N^(2/3)·μ^(−1/3); reference = voda 60 °C, 35 Hz.
## Bez míchadla: přirozená konvekce u svislé stěny Nu = 0,10·Ra^(1/3) → h ∝ (β·ΔT/ν)^(1/3). Učebnicově je to u vody
## 60 °C a ΔT 30 K ~0,46 míchané hodnoty (CDJ ~1 800 W/(m²·K), přirozená ~830). V modelu je ale strana náplně jen ~26 %
## celkového odporu (HA_REF 25 kW/K), ve skutečnosti ~55 %; hNatC 0,28 proto vrací učebnicový poměr CELKOVÉHO prostupu
## bez míchání / s mícháním ≈ 0,6. V hustém rmutu přirozená konvekce skoro nefunguje (viskozita) → připalování.
static func film_hA(sim, v) -> float:
	var c = sim.S.cfg
	var href: float = sim.ha_ref() if sim.has_method("ha_ref") else 25.0
	var th: float = sim.thick_factor(v)
	var visc: float = 1.0 / (1.0 + float(c.get("mashFilmK", 4.6)) * th)      # kalibrovaný vliv hustého rmutu (beze změny)
	var T: float = clampf(float(v.T), 1.0, 110.0)
	var mu_ref: float = sim.skViscosity(60.0, 0.0)
	var mu: float = maxf(1e-5, sim.skViscosity(T, sim.plato(v)))
	var hA_ag: float = 0.0
	if v.stir:
		hA_ag = href * pow(maxf(float(v.hz), 1.0) / 35.0, 0.67) * visc * pow(mu_ref / mu, 1.0 / 3.0)
	var dTw: float = clampf(float(v.get("Twall", T)) - T, 0.5, 80.0)
	var mu_eff: float = mu * (1.0 + 60.0 * th * th)                          # zdánlivá viskozita rmutu (stejně jako u čerpadel)
	var beta: float = maxf(5e-5, 3.85e-4 + (T - 40.0) * 6.08e-6)            # objemová roztažnost vody 40–100 °C [1/K]
	var beta_ref: float = 3.85e-4 + 20.0 * 6.08e-6
	var nat: float = pow(dTw / 30.0 * beta / beta_ref * mu_ref / mu_eff, 1.0 / 3.0)
	var hA_nat: float = href * cf(sim, "hNatC") * nat * clampf(1.0 - 0.7 * th, 0.1, 1.0)
	var hA: float = maxf(hA_ag, hA_nat)
	if v.get("boiling", false): hA = maxf(hA, 2.0 * href * visc)             # var: bublinky u stěny (beze změny)
	return maxf(0.02 * href * visc, hA)


# ======================================================================
# 3) pH mladiny
# ======================================================================

static func _wph(sim) -> Dictionary:
	if not sim.S.has("wph"): sim.S["wph"] = {}
	return sim.S.wph

static func _wph_base(sim, w: Dictionary) -> float:
	if not w.has("base"): w["base"] = float(sim.S.get("pH", 5.5)) + cf(sim, "phSparge")
	return float(w.base)

## pH sladiny/mladiny v pánvi: pH rmutu + vyslazování, pokles za varu (melanoidiny, srážení fosforečnanů vápníku;
## VLB: 5,62 → 5,50 za 90 min, Kunze s. 286: 5,5–5,6 → 5,4–5,5) a kyselina mléčná do mladiny.
static func wort_ph(sim) -> float:
	var S = sim.S; var w: Dictionary = _wph(sim)
	var base: float = float(w.base) if w.has("base") else float(S.get("pH", 5.5)) + cf(sim, "phSparge")
	var malt: float = maxf(100.0, float(S.malt.get("added", 0.0)))
	var ph: float = base - cf(sim, "phBoilDrop") * float(w.get("boilMin", 0.0)) / 60.0 - float(w.get("acid", 0.0)) / (cf(sim, "wortBuf") * malt)
	return clampf(ph, 4.0, 6.5)

## pH pro reakce v nádobě: mladina (bez zrna) má pH mladiny, rmut pH rmutu
static func vessel_ph(sim, v) -> float:
	if v.g < 5.0 and sim.ext(v) > 50.0 and float(sim.S.malt.get("added", 0.0)) > 1.0: return wort_ph(sim)
	return float(sim.S.get("pH", 5.5))

## izomerace rychlejší při vyšším pH (Kosa s. 163); pro verze, které ještě nemají isoPh v react()
static func iso_ph_factor(sim, v) -> float:
	return maxf(0.2, 1.0 + cf(sim, "isoPh") * (clampf(vessel_ph(sim, v), 4.8, 6.0) - 5.5))

## volá se z bloku varu (vroucí mladina) místo dřívějšího pevného přírůstku barvy
static func boil_step(sim, v, m: float) -> void:
	var w: Dictionary = _wph(sim)
	_wph_base(sim, w)
	if float(w.get("lastT", -1.0)) == float(sim.S.t): return      # dvě vroucí pánve v jednom kroku nepočítat dvakrát
	w["lastT"] = float(sim.S.t)
	w["boilMin"] = float(w.get("boilMin", 0.0)) + m

## kyselina mléčná 80 % (1,19 g/mL × 80 % / 90,08 g/mol ≈ 10,6 mmol/mL) do mladiny.
## Pufr mladiny: Kunze s. 229: 30 mL 0,8% roztoku na kg sladu sníží pH mladiny o 0,1 → 26,6 mEq/(kg sladu·pH)
## (tamtéž příklad: 1 L nepřevařené mladiny z pH 5,54 na 5,20 = 0,147 g kyseliny → ~4,8 mmol/(L·pH), tj. ~31 mEq/(kg·pH)).
static func acid_dose(sim, mL80: float) -> float:
	var S = sim.S
	var has_wort: bool = false
	for k in ["rvp", "mp"]:
		var v = S.get(k, null)
		if v is Dictionary and v.get("g", 0.0) < 5.0 and sim.ext(v) > 50.0: has_wort = true
	if not has_wort:
		sim.logm("V pánvi teď není mladina – kyselina mléčná do mladiny se dává za varu (nejlépe těsně před koncem). Do rmutu ji dávkuj v části Voda a pH.", "warn")
		return wort_ph(sim)
	var w: Dictionary = _wph(sim)
	_wph_base(sim, w)
	var before: float = wort_ph(sim)
	w["acid"] = float(w.get("acid", 0.0)) + maxf(0.0, mL80) * 10.6
	w["mL"] = float(w.get("mL", 0.0)) + maxf(0.0, mL80)
	var after: float = wort_ph(sim)
	sim.logm("Kyselina mléčná 80 %%: +%d mL do mladiny (celkem %d mL) – pH %s → %s. Kniha: okyselit těsně před koncem varu na 5,1–5,2 (lepší lom, méně barvy, čistší hořkost; o něco horší využití chmele)." % [roundi(mL80), roundi(float(w.mL)), f2(before), f2(after)])
	return after

static func acid_label(sim) -> String:
	var w: Dictionary = _wph(sim)
	return "Kyselina mléčná 80 %% do mladiny +100 mL   (pH mladiny %s, dáno %d mL)" % [f2(wort_ph(sim)), roundi(float(w.get("mL", 0.0)))]


# ======================================================================
# 4) Barva (Maillardovy reakce)
# ======================================================================

## přírůstek barvy [EBC/h] = colK100 · e^(−Ea/R·(1/T − 1/373 K)) · e^(colPh·(pH − 5,5)) · °P/12
## colK100 1,2 EBC/h: VLB (Meyer, CBC 2015) 5,7 → 8,0 EBC za 90 min varu (mladina 10,6 % extraktu, pH ~5,56). Odpar v pokusu
## není uveden; zahuštění odparem 6–10 % dá 0,4–0,6 EBC (model ho počítá zvlášť, barva je na litr), tvorba je tedy 1,7–1,9 EBC
## → colK100 1,0–1,35 podle odparu; Ea 77 kJ/mol pro 65–100 °C (Kroh, TU Berlín);
## nižší pH = méně barvy (Kunze s. 286). Pod 70 °C se barva netvoří. Běží ve všech nádobách (var, dekokce, vířivka).
static func color_step(sim, v, m: float) -> void:
	var T: float = float(v.T)
	if T < 70.0 or m <= 0.0: return
	if sim.ext(v) < 1.0: return
	var fT: float = exp(-cf(sim, "colEa") * 1000.0 / 8.314 * (1.0 / (T + 273.15) - 1.0 / 373.15))
	var fph: float = exp(cf(sim, "colPh") * (vessel_ph(sim, v) - 5.5))
	var fP: float = clampf(sim.plato(v) / 12.0, 0.0, 3.0)
	var k: float = cf(sim, "colK100") * fT * fph * fP
	if v.g > 5.0: k *= cf(sim, "colMashK")
	v.col += sim.liqVol(v) * k / 60.0 * m


# ======================================================================
# 5) Vířivka – tangenciální nátok
# ======================================================================

## Volá se při přítoku do vířivky; vrací násobek doby víření (slabý nátok = slabá rotace = horší kužel).
## Kunze s. 341: nátok max. 3,5 m/s, co největší objemový tok, lom se nesmí tříštit (čerpadlo, kolena).
## Průměr hrdla wpInletMM; 0 = stejný jako trubka DN50 v plánku (bore50, u Vasila 75 mm).
static func whirl_inlet(sim, v, p, ctx: Dictionary) -> float:
	var m: float = float(ctx.get("m", 0.0))
	if m <= 0.0: return 1.0
	var Lp: float = sim.liqVol(p)
	if sim.vol(v) - Lp < 5.0:
		v["trubNS"] = 0.0; v["trubNSV"] = 0.0          # vířivka byla prázdná – nová várka
		var src = sim.S.get(String(ctx.get("src", "")), null)
		if src is Dictionary and src.has("col") and sim.ext(src) > 50.0 and sim.liqVol(src) > 50.0:
			sim.logm("Vyrážím do vířivky: mladina pH %s, barva %s EBC (v pánvi)." % [f2(wort_ph(sim)), sim.fmt1(src.col / sim.liqVol(src))])
	var dmm: float = cf(sim, "wpInletMM")
	if dmm <= 0.0: dmm = float(sim.bore_mm(50.0)) if sim.has_method("bore_mm") else 75.0
	var d: float = dmm / 1000.0
	var vel: float = Lp / m / 60000.0 / (PI * d * d / 4.0)
	v["inletV"] = vel
	var vmax: float = cf(sim, "wpVmax")
	if vel > vmax and p.trubSus > 0.0:
		var fr: float = clampf(cf(sim, "wpShearK") * (vel - vmax), 0.0, 1.0)
		var ns: float = trub_ns(sim, v)
		v["trubNS"] = minf(float(v.trubSus), ns + p.trubSus * fr)
		v["trubNSV"] = sim.liqVol(v)
		sim.once("fpwpfast%d" % sim.tb(600), "Nátok do vířivky má %s m/s – víc než %s m/s (Kunze s. 341): lom se tříští a kal se pak hůř usadí. Zpomal čerpadlo." % [sim.fmt1(vel), sim.fmt1(vmax)], "warn")
	elif float(v.get("trubNS", 0.0)) > 0.0:
		v["trubNS"] = trub_ns(sim, v); v["trubNSV"] = sim.liqVol(v)
	return clampf(vel / maxf(0.1, cf(sim, "wpVmin")), 0.25, 1.0)

## roztříštěný (neusaditelný) kal [kg] – ubývá úměrně s odčerpanou mladinou
static func trub_ns(sim, v) -> float:
	var ns: float = float(v.get("trubNS", 0.0))
	if ns <= 0.0: return 0.0
	var V0: float = float(v.get("trubNSV", 0.0))
	var L: float = sim.liqVol(v)
	return ns * (clampf(L / V0, 0.0, 1.0) if V0 > 1.0 else 1.0)


# ======================================================================
# 6) Chladnutí stojícího obsahu trubek
# ======================================================================

## podíl vyrovnání s okolím za dt [s]: τ = (C_kapaliny + C_oceli) / (U·π·D_vnější) na metr trubky (nezávisí na délce).
## U holé nerezové trubky ~10 W/(m²·K) (volná konvekce ~8 + sálání; holá nádoba ~15 W/(m²·K) při proudění vzduchu 0,5 m/s).
## DN50 (Ø 75 mm) plná vody: τ ≈ 2,3 h; Ø 50 mm: τ ≈ 1,5 h (dřív pevně 30 min pro všechny trubky).
static func pipe_cool_k(sim, e, dt: float) -> float:
	var bore: float = float(sim.bore_mm(float(e.get("dn", 50.0)))) if sim.has_method("bore_mm") else 50.0
	var D: float = bore / 1000.0
	var Do: float = D + 0.003
	var fill: float = clampf(sim.vol(e.c) / maxf(1e-6, float(e.get("cap", 1.0))), 0.05, 1.0)
	var C: float = 1000.0 * PI * D * D / 4.0 * 4.19 * fill + PI * Do * 0.0015 * 7900.0 * 0.5    # kJ/(K·m)
	var UA: float = cf(sim, "pipeU") * PI * Do / 1000.0                                            # kW/(K·m)
	return 1.0 - exp(-maxf(0.0, dt) * UA / maxf(1e-6, C))


# ======================================================================
# 7) Energie a doplňky protokolu (volitelné háčky)
# ======================================================================

static func energy_step(sim, Qb_kW: float, dt: float) -> void:
	sim.S["Esteam"] = float(sim.S.get("Esteam", 0.0)) + maxf(0.0, Qb_kW) * dt

## do protokolu: pára na hl, pH mladiny, odhad barvy piva (barva během kvašení klesá; Kunze s. 287: zákvasná 11,5–12 → pivo 7–8 EBC)
static func report_extra(sim, rep: Dictionary) -> void:
	var V_hl: float = float(rep.get("V", 0.0))
	if V_hl > 0.1:
		rep["kWh_pary_na_hl"] = float(sim.S.get("Esteam", 0.0)) / 3600.0 / V_hl
	rep["pH_mladiny"] = wort_ph(sim)
	if rep.has("EBC"): rep["EBC_pivo_odhad"] = float(rep.EBC) * 0.65
