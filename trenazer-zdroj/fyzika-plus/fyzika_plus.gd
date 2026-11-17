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
## Doplněno 16. 11. 2026 (verze 1.1):
##  8) parní topný had bojleru stejným modelem ventilu a kondenzátu jako plášť pánve
##  9) připalování: úbytek výkonu vidí kotel i nádoba stejně (bilance energie sedí)
## 10) průtok ledové vody podle přiškrcení klapek (dřív pevně 120 L/min)
## 11) hladina a kapalina: odpar z horké hladiny před varem a ve vířivce, bod varu u dna podle hloubky
## Doplněno 16. 11. 2026 odpoledne (verze 1.2):
## 12) stojící odstředivé čerpadlo je průchozí (samospád a zpětný tok přes vypnuté čerpadlo)
## 13) ztráty tepla nádoby podle smáčené plochy (poloprázdná nádoba ztrácí míň než plná)
## Doplněno 17. 11. 2026 (verze 1.3):
## 14) výpust do kanálu otevřeným koncem trubky i přes stojící čerpadlo (H18)
## 15) rozpad kalového kužele na konci stahování z vířivky (H19)
## 16) verze 1.5: kyslík v zákvasné mladině podle teploty a stupňovitosti (H20)
## 17) verze 1.6: vypočtené hodnoty místo odhadů (ztráty izolovaných nádob, deskový chladič) – háček K1, VYPOCTY.md;
##     automat počítá velikost dílčích rmutů z tepelné bilance (A1) a při chlazení řídí ledovou vodu

const VERZE := "FYZIKA+ 1.6 (2026-11-17)"

## výchozí hodnoty nových parametrů (stejné řádky jsou v CFG_DEFAULT; tady jako pojistka, kdyby v něm chyběly)
const DEF := {
	"stValveR": 30.0, "rvpKvs": 0.0, "mpKvs": 0.0, "stCondBack": 0.15, "jWaterKg": 150.0, "jDrainKgS": 0.6,
	"jAirTau": 20.0, "jAirInTau": 15.0, "jVacBreaker": 1.0, "hNatC": 0.28,
	"colK100": 1.2, "colEa": 77.0, "colPh": 0.7, "colMashK": 1.0,
	"phBoilDrop": 0.08, "phSparge": 0.05, "wortBuf": 26.6, "isoPh": 0.5,
	"wpInletMM": 0.0, "wpVmax": 3.5, "wpVmin": 1.0, "wpShearK": 0.1, "pipeU": 10.0,
	"hltKvs": 0.0, "lvPumpQ": 120.0, "rvpDiaM": 0.0, "mpDiaM": 0.0, "wpDiaM": 0.0, "evapOpen": 0.1, "mashVmax": 1.5,
	"pumpPass": 1.0, "pumpPassK": 5.0, "dryWallK": 0.95,
	"wpConeV": 0.006, "wpConeBreak": 0.1, "wpConeAnn": 0.6, "wpConeAuto": 1.0, "autoIce": 1.0, "decoT1": 63.0, "decoT2": 76.0,
	"aerEff": 0.9, "o2PlatoK": 0.017, "o2Rest": 0.5,
}
## povolené rozsahy (pro CALIB_RANGE)
const RANGE := {
	"stValveR": [1.0, 50.0], "rvpKvs": [0.0, 200.0], "mpKvs": [0.0, 200.0], "stCondBack": [0.0, 2.0], "jWaterKg": [20.0, 1000.0], "jDrainKgS": [0.05, 5.0],
	"jAirTau": [2.0, 600.0], "jAirInTau": [2.0, 600.0], "jVacBreaker": [0.0, 1.0], "hNatC": [0.05, 1.5],
	"colK100": [0.0, 10.0], "colEa": [40.0, 150.0], "colPh": [0.0, 3.0], "colMashK": [0.0, 5.0],
	"phBoilDrop": [0.0, 0.4], "phSparge": [-0.2, 0.4], "wortBuf": [5.0, 80.0], "isoPh": [0.0, 2.0],
	"wpInletMM": [0.0, 150.0], "wpVmax": [1.0, 8.0], "wpVmin": [0.2, 5.0], "wpShearK": [0.0, 1.0], "pipeU": [0.5, 30.0],
	"hltKvs": [0.0, 200.0], "lvPumpQ": [10.0, 600.0], "rvpDiaM": [0.0, 5.0], "mpDiaM": [0.0, 5.0], "wpDiaM": [0.0, 6.0], "evapOpen": [0.0, 3.0], "mashVmax": [0.5, 4.0],
	"pumpPass": [0.0, 1.0], "pumpPassK": [0.0, 50.0], "dryWallK": [0.0, 1.0],
	"wpConeV": [0.0005, 0.1], "wpConeBreak": [0.0, 1.0], "wpConeAnn": [0.0, 2.0], "wpConeAuto": [0.0, 1.0], "autoIce": [0.0, 1.0], "decoT1": [0.0, 80.0], "decoT2": [0.0, 80.0],
	"aerEff": [0.0, 1.5], "o2PlatoK": [0.0, 0.06], "o2Rest": [0.0, 5.0],
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
	var S = sim.S
	var nm: String = "rvp" if is_same(v, S.rvp) else ("mp" if is_same(v, S.mp) else "x")
	var V: float = sim.liqVol(v) + sim.sol(v) * 0.65
	var wet: float = clampf((V - 100.0) / maxf(1.0, jL - 100.0), 0.0, 1.0)
	# přestup: pára + stěna (gs, z kalibrovaného rvpUAst) v sérii s náplní (hA – míchání, viskozita, přirozená konvekce)
	var href: float = sim.ha_ref() if sim.has_method("ha_ref") else maxf(25.0, 2.0 * UA_st)
	var gs: float = 1.0 / maxf(1e-6, 1.0 / UA_st - 1.0 / href)
	var hA: float = sim.film_hA(v)
	# připalování hustého rmutu (H14): nános zhorší prostup o 10 % (stejná velikost jako dřív v heat()), ale tady,
	# takže kotel vyrobí přesně tolik páry, kolik nádoba přijme
	# značka, že heat() už výkon sám neubírá, je na objektu simulace (set_meta v háčku H14) – neukládá se s várkou
	var burn: bool = sim.has_meta("fpH14") and sim.sol(v) + float(v.get("sg", 0.0)) > 40.0
	return _steam_core(sim, v, nm, UA_st, clampf(float(v.get("heat", 0.0)), 0.0, 1.0), wet, gs, hA, float(v.T), maxf(1.0, cf(sim, "jWaterKg")), burn)

## Parní topný had bojleru (H10): stejný ventil, tlak, vzduch a kondenzát jako u pláště pánve. hltUAst je celkový prostup
## (pára → voda), had je pod vodou. Kondenzátu se do hadu vejde úměrně míň než do pláště pánve (podle poměru UA – ODHAD).
static func steam_coil(sim, h, UA_st: float, _Tst: float) -> Dictionary:
	var c = sim.S.cfg
	var M: float = maxf(5.0, cf(sim, "jWaterKg") * UA_st / maxf(0.1, float(c.get("rvpUAst", UA_st))))
	return _steam_core(sim, h, "hlt", UA_st, clampf(float(h.get("heat", 0.0)), 0.0, 1.0), 1.0, UA_st, 1e9, float(h.T), M, false)

## Rovnováha v plášti při daném stavu (bez změny stavu): vrací P [kW], kgs, p (celkový tlak) a air (podíl vzduchu).
## kf = násobek prostupu (nános při připalování).
static func _jacket_eq(Kv: float, p1: float, wet: float, fl: float, gs: float, hA: float, T: float, air0: float, breaker: bool, kf: float) -> Dictionary:
	var atm: float = 1.013
	var air: float = air0
	var UA: float = kf * wet * (1.0 - fl) / ((1.0 + 100.0 * air) / gs + 1.0 / maxf(1e-4, hA))   # 1 % vzduchu ≈ poloviční přestup na straně páry (ODHAD)
	var J: Dictionary = jacket_solve(Kv, p1, UA, T, air)
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
			var UAy: float = kf * wet * (1.0 - fl) / ((1.0 + 100.0 * yv) / gs + 1.0 / maxf(1e-4, hA))
			if UAy * maxf(0.0, Tjy - T) / hfg(Tjy) > mv: lo = yv
			else: hi = yv
		air = 0.5 * (lo + hi)
		var Tje: float = tsat(atm * (1.0 - air))
		var UAe: float = kf * wet * (1.0 - fl) / ((1.0 + 100.0 * air) / gs + 1.0 / maxf(1e-4, hA))
		P = UAe * maxf(0.0, Tje - T)
		kgs = P / hfg(Tje)
		ptot = atm
	return {"P": P, "kgs": kgs, "p": ptot, "air": air}

## Společné jádro pláště i topného hadu. st = slovník, ve kterém se drží stav (nádoba nebo S.hlt).
static func _steam_core(sim, st, nm: String, UA_st: float, x: float, wet: float, gs: float, hA: float, Tliq: float, M: float, burn: bool) -> Dictionary:
	var S = sim.S; var c = S.cfg
	var now: float = float(S.t)
	var dt: float = clampf(now - float(st.get("jt", now)), 0.0, 60.0)     # délka minulého kroku (sim_step ji sem nepředává)
	st["jt"] = now
	var pb: float = float(S.boiler.get("p", c.get("steamBar", 2.5)))
	var p1: float = (pb + 1.013) if (S.boiler.get("on", true) or pb > 0.05) else 0.0
	var kvs: float = cf(sim, nm + "Kvs") if nm != "x" else 0.0
	# ventil se nemění, když kotel ztratí tlak (porucha „pára“ přepíše steamBar) → navrhovat z původního tlaku
	if kvs <= 0.0: kvs = kvs_auto(UA_st, float(S.get("steamBar0", c.get("steamBar", 2.5))))
	var Kv: float = kvs * kv_frac(x, cf(sim, "stValveR"))
	# parní trasa v plánku zavřená (klapky) → do pláště pára nejde, kotel ji nevyrábí
	if not bool(S.get("steamOK", {}).get(nm, true)): Kv = 0.0
	var atm: float = 1.013
	var breaker: bool = cf(sim, "jVacBreaker") >= 0.5
	var pback: float = atm + cf(sim, "stCondBack")
	# 1) stav pláště se posune o uplynulý krok podle podmínek z minulého kroku
	var air: float = clampf(float(st.get("jy", 0.0)), 0.0, 1.0)
	var fl: float = clampf(float(st.get("jfl", 0.0)), 0.0, 0.95)
	if dt > 0.0 and st.has("jpt"):
		var pto: float = float(st.jpt); var kgo: float = float(st.get("jkgs", 0.0)); var kvo: bool = bool(st.get("jkv", false))
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
	# 2) tlak v plášti a výkon v tomto kroku
	var R: Dictionary = _jacket_eq(Kv, p1, wet, fl, gs, hA, Tliq, air, breaker, 1.0)
	if burn and float(R.P) > 0.0 and Tliq + float(R.P) / maxf(0.5, hA) > 100.0:
		R = _jacket_eq(Kv, p1, wet, fl, gs, hA, Tliq, air, breaker, 0.9)
	var ptot: float = float(R.p)
	var kgs: float = float(R.kgs)
	var P: float = float(R.P)
	air = float(R.air)
	# hlášení obsluze
	var x0: float = float(st.get("jx", x))
	st["jx"] = x
	if nm != "x":
		var kde: String = "topném hadu" if nm == "hlt" else "plášti"
		var kde_v: String = "TOPNÉM HADU" if nm == "hlt" else "PLÁŠTI"
		var co: String = "topný had" if nm == "hlt" else "plášť"
		var ceho: String = "topného hadu" if nm == "hlt" else "pláště"
		if fl > 0.25 and x - x0 > 0.3 and dt <= 5.0:
			sim.once("fphammer%s%d" % [nm, sim.tb(300)], "%s: RÁZ V %s – pára vpuštěná naráz do %s zaplaveného kondenzátem (%d %%). Páru otevírej plynule a nech kondenzát nejdřív odtéct." % [_vn(sim, nm), kde_v, ceho, roundi(fl * 100.0)], "bad")
		if kgs > 0.0 and ptot < pback and wet > 0.0:
			st["jst"] = float(st.get("jst", 0.0)) + dt
			if float(st.jst) > 120.0:
				sim.once("fpstall%s%d" % [nm, sim.tb(1800)], "%s: tlak v %s %s bar abs. je pod protitlakem kondenzátu %s bar – kondenzát neodtéká (stall) a %s se zaplavuje (%d %%). U přiškrcené páry na chladnou náplň je to běžné; pak páru otevírej pomalu, ať nevzniknou rázy." % [_vn(sim, nm), kde, f2(ptot), f2(pback), co, roundi(fl * 100.0)], "warn")
		else: st["jst"] = 0.0
		if not breaker and ptot < 0.5 and float(st.get("jUsed", 0.0)) > 0.0 and Kv <= 0.0 and nm != "hlt":
			sim.once("fpvac%s%d" % [nm, sim.tb(1800)], "%s: v plášti je podtlak %s bar abs. a plášť nemá zavzdušňovací ventil – po zavření páry otevři odvzdušnění pláště, jinak hrozí zborcení dna (Kunze s. 290–291)." % [_vn(sim, nm), f2(ptot)], "bad")
		if air > 0.3 and Kv > 0.0 and wet > 0.0 and nm != "hlt":
			sim.once("fpair%s%d" % [nm, sim.tb(1800)], "%s: plášť je zavzdušněný – pára nejdřív vytlačí vzduch odvzdušňovačem, ohřev chvíli nejede naplno." % _vn(sim, nm), "info")
	if kgs > 0.0: st["jUsed"] = 1.0
	st["jy"] = air
	st["jfl"] = fl
	st["jp"] = ptot
	st["jpt"] = ptot
	st["jkgs"] = kgs
	st["jkv"] = Kv > 0.0
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

## izomerace rychlejší při vyšším pH (Kosař s. 163); pro verze, které ještě nemají isoPh v react()
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
	var spd = sim.S.get("sp", null)
	if spd is Dictionary and float(spd.get("L", 0.0)) > 100.0:
		rep["kal_do_spilky_mg_L"] = float(spd.get("trub", 0.0)) * 1000000.0 / float(spd.L)
		rep["O2_mg_l"] = float(spd.get("O2", 0.0)) / float(spd.L)   # kyslík pro kvasnice (Kunze s. 353: 8–10 mg/L)
	var V_hl: float = float(rep.get("V", 0.0))
	if V_hl > 0.1:
		rep["kWh_pary_na_hl"] = float(sim.S.get("Esteam", 0.0)) / 3600.0 / V_hl
	rep["pH_mladiny"] = wort_ph(sim)
	if sim.S.has("evapPre"): rep["odpar_mimo_var_L"] = float(sim.S.evapPre)
	if rep.has("EBC"): rep["EBC_pivo_odhad"] = float(rep.EBC) * 0.65


# ======================================================================
# 8) Chlazení mladiny: průtok ledové vody (H11)
# ======================================================================

## Průtok ledové vody jednou větví [L/min]: výkon čerpadla ledové vody (lvPumpQ; dřív pevně 120 L/min) dělený počtem větví
## × přiškrcení klapek na trase (stejná funkce škrcení jako u ostatních tras v plánku). Větve se navzájem neovlivňují
## (plochá křivka čerpadla – zjednodušení). Obsluha tak může „ubrat ledovou“, když jde mladina do spilky přechlazená.
static func ice_flow(sim, path: Array, n_dest: int) -> float:
	var q: float = cf(sim, "lvPumpQ") / maxf(1.0, float(n_dest))
	if sim.has_method("path_pct"): q *= clampf(float(sim.path_pct(path)), 0.0, 1.0)
	sim.set_meta("fpIceNom", q)             # kolik ledové čerpadlo touhle větví dá naplno
	q = minf(q, auto_ice_cap(sim))          # automat v kroku chlazení řídí ledovou podle teploty do spilky
	return q


# ======================================================================
# 8b) Samospád podle hydrauliky trasy (H15)
# ======================================================================

## Průtok samospádem [L/min]: takový, při kterém tlaková ztráta trasy spotřebuje právě rozdíl hladin head_m.
## Ztráta trasy se počítá stejnou funkcí jako u čerpadel (sim.path_head: Darcy–Weisbach podle průměru a délky trubek,
## místní ztráty, přiškrcení klapek), s hustotou a viskozitou kapaliny ve zdrojové nádobě (hustý rmut teče pomaleji).
## K tomu výtoková ztráta na konci trasy (v²/2g). Dřív pevně 120 L/min při 1 m rozdílu bez ohledu na průměr trubky.
static func gravity_q(sim, path: Array, head_m: float, src_key) -> float:
	if head_m <= 0.0 or not sim.has_method("path_head"): return 0.0
	var rho: float = 1.0; var mu: float = sim.skViscosity(20.0, 0.0)
	if src_key is String and src_key in ["rvp", "mp", "wp", "sk", "hlt", "spilka", "lv"]:
		var T: float = float(sim.S.lv.T) if src_key == "lv" else float(sim._src_temp(src_key))
		var fl = sim._src_fluid(src_key) if sim.has_method("_src_fluid") else {}
		if fl is Dictionary and fl.has("g"):
			var th: float = sim.thick_factor(fl)
			rho = sim.density(fl)
			mu = sim.skViscosity(T, sim.plato(fl)) * (1.0 + 60.0 * th * th)      # zdánlivá viskozita rmutu jako u čerpadel
		else:
			mu = sim.skViscosity(T, 0.0)
	# na konci trasy kapalina vytéká rychlostí v – její rychlostní výška v²/2g se z rozdílu hladin spotřebuje taky
	# (bez ní by krátkou trubkou teklo víc, než dovolí Torricelli v = √(2gh))
	var d_out: float = 0.0
	for st in path:
		if st is Dictionary and st.has("e"): d_out = float(sim.bore_mm(float(st.e.dn))) / 1000.0
	if d_out <= 0.0: d_out = float(sim.bore_mm(50.0)) / 1000.0
	var a_out: float = PI * d_out * d_out / 4.0
	# stojící čerpadla na trase: každé přidá ztrátu K·v²/2g (trasa v místě čerpadla „přeskakuje“ mezi sáním a výtlakem).
	# Trasa výpusti do kanálu nese jen trubky (bez kroku chladiče): přeskok přes deskový chladič se pozná podle uzlu
	# a dostane ztrátu chladiče stejnou jako v path_head (4 m při 200 L/min).
	var pump_a: Array = []
	var plates_x := 0
	var prev_to = null
	for st2 in path:
		if not (st2 is Dictionary): continue
		if st2.has("plate") or not st2.has("e"):
			prev_to = null; continue
		if prev_to != null and st2.e.get("a") != prev_to and st2.e.get("b") != prev_to:
			var is_plate := false
			var nd = sim.net.nodes.get(prev_to) if sim.net is Dictionary and sim.net.has("nodes") else null
			if nd != null:
				var has_pump := false
				for at in nd.att:
					if at.kind == "pump": has_pump = true
					elif at.kind == "plate": is_plate = true
				if has_pump: is_plate = false
			if is_plate: plates_x += 1
			else:
				var dp: float = float(sim.bore_mm(float(st2.e.dn))) / 1000.0
				pump_a.append(PI * dp * dp / 4.0)
		prev_to = st2.get("to")
	var kp: float = cf(sim, "pumpPassK")
	var need: Callable = func(Q: float) -> float:
		var vo: float = Q / 60000.0 / a_out
		var h: float = float(sim.path_head(path, Q, rho, mu)) + vo * vo / (2.0 * 9.81)
		for ap in pump_a:
			var vp: float = Q / 60000.0 / float(ap)
			h += kp * vp * vp / (2.0 * 9.81)
		h += plates_x * 4.0 * pow(Q / 200.0, 2)
		return h
	var hi: float = 6000.0
	if float(need.call(hi)) <= head_m: return hi
	var lo: float = 0.0
	for i in 34:
		var mid: float = 0.5 * (lo + hi)
		if float(need.call(mid)) > head_m: hi = mid
		else: lo = mid
	return 0.5 * (lo + hi)


# ======================================================================
# 8c) Stojící odstředivé čerpadlo je průchozí (H16)
# ======================================================================

## Čerpadla, která právě stojí – hledání trasy pro samospád jimi smí projít, a to oběma směry (značka "~rev").
## Odstředivé čerpadlo nemá nic, co by po vypnutí zavřelo průtok: kapalina jím teče podle rozdílu hladin dopředu i zpátky.
## Proto se po vypnutí čerpadla zavírá klapka za ním a před ním. Ztráta v čerpadle: K ≈ 5 (oběžné kolo se volně protáčí),
## K ≈ 10 (zablokované) násobků rychlostní výšky (GIW/KSB) – parametr pumpPassK. pumpPass = 0 vrací dřívější chování.
## Na Zajíci za čerpadly zpětné klapky nejsou (Vasil 16. 11. 2026), výchozí pumpPass = 1 tomu odpovídá.
## Doběh: první 3 s po vypnutí se čerpadlo ještě bere jako neprůchozí (kolo dobíhá a drží tlak; ODHAD) – obsluha i automat
## tak stihnou zavřít klapku dřív, než se proud otočí. Stejně dlouho trvá, než se proud rozběhne po přestavení klapek
## (setrvačnost kapaliny v potrubí). Při zrychleném čase se ta doba násobí rychlostí – obsluha má vždycky 3 s skutečného
## času na zavření klapky (při 10× by jinak měla 0,3 s).
const PUMP_RUNDOWN_S := 3.0

## doba [s simulace], po kterou je čerpadlo po vypnutí (a trasa po přestavení klapek) ještě neprůchozí
static func pump_grace(sim) -> float:
	var g: float = PUMP_RUNDOWN_S
	var mn = sim.get("main")
	if mn != null and mn.get("running") == true and mn.get("speed") != null: g *= maxf(1.0, float(mn.speed))
	return g

static func idle_pumps(sim) -> Array:
	if cf(sim, "pumpPass") < 0.5: return []
	var S = sim.S; var out: Array = []
	# čerpadlo, které automat právě spouští (čeká na zavodnění), se nebere jako stojící – při ručním ovládání je v té chvíli už zapnuté
	var pend: Dictionary = {}
	var mn = sim.get("main")
	if mn != null and mn.get("auto") != null and bool(mn.auto.get("on")) and mn.auto.get("pend") is Dictionary: pend = mn.auto.pend
	var run: Dictionary = {}
	for k in ["p1", "p2", "p3"]:
		if S.has(k) and S[k] is Dictionary: run[k] = bool(S[k].get("on", false)) or pend.has(k)
	var hot: bool = false
	for k2 in ["rvpWater", "skWater", "flush"]:
		if S.has(k2) and S[k2] is Dictionary and bool(S[k2].get("on", false)): hot = true
	run["phw"] = hot
	if S.has("lv") and S.lv is Dictionary: run["plv"] = bool(S.lv.get("pump", false))
	var st: Dictionary = sim.get_meta("fpPumpOff", {})
	var now: float = float(S.get("t", 0.0))
	var grace: float = pump_grace(sim)
	var vv = sim.get("valve_ver")
	if vv != null:
		if not st.has("~vv") or st["~vv"] != vv or now < float(st.get("~vt", 0.0)):
			st["~vv"] = vv; st["~vt"] = now          # klapky se právě přestavily (nebo nový stav simulace)
		if now - float(st["~vt"]) < grace:
			for k0 in run:
				if run[k0]: st.erase(k0)
				elif not st.has(k0) or now < float(st[k0]): st[k0] = now
			sim.set_meta("fpPumpOff", st)
			return []
	for k in run:
		if run[k]:
			st.erase(k); continue
		if not st.has(k) or now < float(st[k]): st[k] = now          # poprvé viděno stát (nebo nový stav simulace)
		if now - float(st[k]) >= grace: out.append(k)
	sim.set_meta("fpPumpOff", st)
	if out.size() > 0: out.append("~rev")
	return out

## Automat scezování po skončení zavře kapalinové klapky, které si sám otevřel (H16b). Dřív je nechával otevřené, protože
## stojící čerpadlo nic nepustilo; teď by plná pánev tekla stojícím C2 zpátky pod síta. Když klapkami mezitím hýbala
## obsluha (jiná verze klapek, než si automat pamatuje), nesahá na nic.
static func lauter_stop(sim) -> void:
	if cf(sim, "pumpPass") < 0.5: return
	var a = sim.S.get("lauterAuto", null)
	if a == null or not (a is Dictionary) or int(a.get("routeVersion", -1)) != int(sim.valve_ver): return
	var steam := {}
	for node in sim.net.nodes.values():
		if node.valve == null: continue
		for edge in node.edges:
			if edge.medium == "steam": steam[node.valve] = true
	var n := 0
	for it in sim.net.byId.values():
		if it.type == "valve" and not steam.has(it.id) and it.get("kind", "") != "zasl" and bool(it.get("open", false)):
			it["open"] = false; n += 1
	var mn = sim.get("main")
	if mn != null and mn.get("outlet_open") is Dictionary:
		for key in mn.outlet_open:
			if mn.outlet_open[key]:
				mn.outlet_open[key] = false; n += 1
	if n > 0:
		sim.valve_ver += 1
		a["routeVersion"] = sim.valve_ver
		sim.logm("Automat scezování zavřel klapky na svých trasách (stojícím čerpadlem by kapalina tekla samospádem dál).")


# ======================================================================
# 8d) Ztráty tepla podle smáčené plochy (H17)
# ======================================================================

## Násobek ztrát nádoby do okolí podle zaplnění. Kalibrované UA (rvpUA, wpUA…) platí pro plnou nádobu.
## Smáčená stěna (dno a plášť pod hladinou) předává teplo naplno. Suchá stěna (plášť nad hladinou a víko) ho dostává jen
## od páry a vzduchu nad hladinou, takže ztrácí míň – dryWallK násobek smáčené stěny:
##   k = [A_dna + z·A_pláště + dryWallK·((1 − z)·A_pláště + A_víka)] / [A_dna + A_pláště + dryWallK·A_víka],  z = zaplnění
## dryWallK = 1: dobře izolovaná nádoba (odpor izolace je mnohem větší než odpor na vnitřní straně) – ztráty na hladině
## nezávisí, tj. dřívější chování. dryWallK ≈ 0,5: holá stěna (vnější odpor ~0,1 m²K/W, uvnitř nad hladinou zhruba stejný).
## Výchozí 0,95 je VYPOČTENO pro izolovanou nádobu (80 mm minerální vlny: odpor izolace 1,78 m²K/W, vně 0,125, uvnitř nad
## hladinou ~0,1 → suchá stěna 1,905/2,005 = 0,95 smáčené); viz VYPOCTY.md. Pro holou nádobu ~0,5.
## Geometrie z průměru nádoby (vessel_dia), válec s rovným dnem.
static func loss_k(sim, key: String, v) -> float:
	var dk: float = clampf(cf(sim, "dryWallK"), 0.0, 1.0)
	if dk >= 0.999: return 1.0
	var cap: float = float(sim.S.cfg.get(key + "Cap", 0.0))
	if cap <= 0.0: return 1.0
	var d: float = vessel_dia(sim, key)
	var A: float = PI * d * d / 4.0
	var Hfull: float = cap / 1000.0 / maxf(0.01, A)
	var side: float = PI * d * Hfull
	var z: float = clampf(float(sim.vol(v)) / cap, 0.0, 1.0)
	return (A + z * side + dk * ((1.0 - z) * side + A)) / (A + side + dk * A)


# ======================================================================
# 8e) Kalový kužel ve vířivce na konci stahování (H19)
# ======================================================================

## Tvar kužele: objem vlhkého kalu Vc = trubSet × 6 L (stejné číslo, jaké program bere jako mrtvý objem kalu), základna
## kužele nechá u stěny mezikruží bez kalu wpConeAnn (Kosař s. 181: aspoň 60 cm), nejvýš však 3/4 průměru dna.
## Vrací {A, rc, h, Vc} v m², m, m, m³.
static func cone_geo(sim) -> Dictionary:
	var w = sim.S.wp
	var D: float = vessel_dia(sim, "wp")
	var A: float = PI * D * D / 4.0
	var rc: float = clampf(D / 2.0 - cf(sim, "wpConeAnn"), 0.1 * D, 0.375 * D)
	var Vc: float = maxf(0.0, float(w.get("trubSet", 0.0))) * 6.0 / 1000.0
	var h: float = 3.0 * Vc / (PI * rc * rc) if Vc > 0.0 else 0.0
	return {"A": A, "rc": rc, "h": h, "Vc": Vc}

## Výška hladiny nad dnem [m] při objemu V [L]: vlhký kal je napitý mladinou (jeho mladina je v objemu vířivky), takže
## pod vrcholem kužele platí V = A·y + Vc·((h − y)/h)³ (část kužele nad hladinou zabírá místo).
static func cone_level(g: Dictionary, V_L: float) -> float:
	var V: float = maxf(0.0, V_L) / 1000.0
	var A: float = g.A; var h: float = g.h
	if h <= 1e-6 or V >= A * h: return V / A
	var lo := 0.0; var hi := h
	for i in 40:
		var y: float = (lo + hi) * 0.5
		var t: float = (h - y) / h
		if A * y + g.Vc * t * t * t < V: lo = y
		else: hi = y
	return (lo + hi) * 0.5

## volná plocha hladiny [m²] ve výšce y (kužel zabírá kruh o poloměru rc·(1 − y/h))
static func cone_free_area(g: Dictionary, y: float) -> float:
	if g.h <= 1e-6 or y >= g.h: return g.A
	var r: float = g.rc * (1.0 - y / g.h)
	return maxf(0.05 * g.A, g.A - PI * r * r)

## Odtah z vířivky (háček H19 ve srcTake): když hladina klesá podél kalového kužele rychleji, než z něj stačí vytékat
## zadržená mladina, kužel se rozpadá a kal jde s mladinou (Kunze s. 341). Kosař s. 181: rychlost spílání snižovat, když
## se hladina přiblíží vrcholu kužele; při pozvolném vytékání se strhávání kalu omezí. Za vířivkou má být průměrně
## pod 100 mg/L hrubého kalu (Kosař s. 184).
## Model (ODHAD): z části kužele, která se při tomto odběru vynoří, se uvolní podíl
##   f = wpConeBreak × clamp((u − u0)/u0, 0, 1),  u = rychlost klesání hladiny, u0 = wpConeV (jak rychle se kužel odvodňuje).
## part = to, co srcTake právě vzalo (L litrů při průtoku Lmin L/min).
static func wp_cone(sim, part, L: float, Lmin: float):
	if part == null or not (part is Dictionary): return part
	var w = sim.S.wp
	if float(w.get("trubSet", 0.0)) <= 1e-4 or L <= 0.0: return part
	var g: Dictionary = cone_geo(sim)
	if g.h <= 1e-6: return part
	var V1: float = float(sim.vol(w)); var V0: float = V1 + L
	var y0: float = cone_level(g, V0); var y1: float = cone_level(g, V1)
	if y1 >= g.h: return part
	var e0: float = pow(clampf((g.h - y0) / g.h, 0.0, 1.0), 3.0)
	var e1: float = pow(clampf((g.h - y1) / g.h, 0.0, 1.0), 3.0)
	if e1 <= e0 + 1e-9: return part
	var u: float = Lmin / 1000.0 / cone_free_area(g, (y0 + y1) * 0.5)        # m/min
	var u0: float = maxf(1e-5, cf(sim, "wpConeV"))
	var f: float = cf(sim, "wpConeBreak") * clampf((u - u0) / u0, 0.0, 1.0)
	if f <= 0.0: return part
	var rel: float = float(w.trubSet) * (e1 - e0) / maxf(1e-6, 1.0 - e0) * f
	rel = minf(rel, float(w.trubSet))
	w.trubSet -= rel
	part["trubSus"] = float(part.get("trubSus", 0.0)) + rel
	sim.S["coneLoss"] = float(sim.S.get("coneLoss", 0.0)) + rel
	if sim.has_method("once") and sim.has_method("tb"):
		sim.once("fpcone%d" % sim.tb(600), "Vířivá káď: hladina klesá podél kalového kužele rychleji (%s cm/min), než z něj stačí vytékat mladina – kužel se rozpadá a kal jde s mladinou. Na konci stahování zpomal čerpadlo (pod %s cm/min, teď by to bylo pod %d L/min)." % [f2(u * 100.0), f2(u0 * 100.0), int(wp_cone_cap(sim))], "warn")
	return part

## Nejvyšší průtok odtahu z vířivky [L/min], při kterém se kužel ještě nerozpadá (hladina klesá nejvýš wpConeV).
## Nad kuželem (s rezervou 20 % výšky) žádné omezení. Používá automat (krok chlazení), když wpConeAuto = 1.
static func wp_cone_cap(sim) -> float:
	var w = sim.S.wp
	if float(w.get("trubSet", 0.0)) <= 1e-4: return 1e9
	var g: Dictionary = cone_geo(sim)
	if g.h <= 1e-6: return 1e9
	var y: float = cone_level(g, float(sim.vol(w)))
	if y > g.h * 1.2: return 1e9
	return cf(sim, "wpConeV") * cone_free_area(g, minf(y, g.h)) * 1000.0 * 0.95

## Průtok ledové vody [L/min], při kterém mladina o průtoku Lw a teplotě Tin vyjde z deskového chladiče s teplotou Tgoal.
## Stejné vztahy jako plate_cool v sim.gd (ε-NTU protiproud, prostup roste s průtokem s exponentem 0,65).
static func ice_for_target(sim, Lw: float, Tin: float, Tc: float, Tgoal: float) -> float:
	var c = sim.S.cfg
	var UA0: float = float(c.get("UA1", 25.0)); var qref: float = float(c.get("plateRefQ", 80.0))
	var Cw: float = Lw / 60.0 * 1.04 * 4.0
	if Cw <= 0.0 or Tin <= Tgoal or Tgoal <= Tc: return 1e9
	var lo := 2.0; var hi := 600.0
	for i in 40:
		var Qc: float = (lo + hi) * 0.5
		var UA: float = UA0 / (0.5 * pow(qref / maxf(1.0, Lw), 0.65) + 0.5 * pow(qref * 1.1 / Qc, 0.65))
		var Cc: float = Qc / 60.0 * 4.19
		var Cmin: float = minf(Cw, Cc); var Cr: float = Cmin / maxf(Cw, Cc); var NTU: float = UA / Cmin
		var eff: float
		if Cr > 0.999: eff = NTU / (1.0 + NTU)
		else:
			var x: float = exp(-NTU * (1.0 - Cr)); eff = (1.0 - x) / (1.0 - Cr * x)
		var Tout: float = Tin - eff * Cmin * (Tin - Tc) / Cw
		if Tout > Tgoal: lo = Qc
		else: hi = Qc
	return (lo + hi) * 0.5

## automat v kroku chlazení (vrací průtok C3, který má nastavit):
##  – když řídí ledovou (autoIce) a ledové je dost, přidává mladinu o 3 L/min za minutu, dokud ledová nestačí jen tak tak
##    (92 % toho, co čerpadlo ledové dá) – várka se tak zchladí co nejrychleji, jak chladič a ledová dovolí;
##  – u kalového kužele zpomalí pod mez (wpConeAuto). Ledovou vodu k tomu ubírá ice_flow.
static func auto_cone_flow(sim, flow: float) -> float:
	var f: float = flow
	if cf(sim, "autoIce") >= 0.5 and sim.has_meta("fpIceNom"):
		var S = sim.S
		var now: float = float(S.get("t", 0.0))
		var t0: float = float(sim.get_meta("fpAutoT", now))
		var dts: float = clampf(now - t0, 0.0, 10.0)
		sim.set_meta("fpAutoT", now)
		var need: float = ice_for_target(sim, f, float(S.wp.T), float(S.lv.T), float(S.cfg.get("pitchT", 8.5)))
		if need < 0.92 * float(sim.get_meta("fpIceNom")): f += 3.0 * dts / 60.0
	if cf(sim, "wpConeAuto") >= 0.5: f = minf(f, wp_cone_cap(sim))
	return f

## Automat v kroku chlazení řídí i ledovou vodu (autoIce = 1): pustí jí jen tolik, aby mladina při nastaveném průtoku C3
## vyšla s teplotou pitchT (teplota do spilky se v praxi řídí ledovou vodou, Kosař s. 186).
## Vrací nejvyšší průtok ledové [L/min], nebo 1e9 (bez omezení).
static func auto_ice_cap(sim) -> float:
	if cf(sim, "autoIce") < 0.5: return 1e9
	var mn = sim.get("main")
	if mn == null or mn.get("auto") == null or not bool(mn.auto.get("on")) or not mn.auto.has_method("_id") or mn.auto._id() != "cool": return 1e9
	var S = sim.S
	if not (S.get("p3") is Dictionary) or not (S.get("wp") is Dictionary) or not (S.get("lv") is Dictionary): return 1e9
	var Lw: float = float(S.p3.get("flow", 0.0))
	if Lw <= 1.0: return 1e9
	return ice_for_target(sim, Lw, float(S.wp.T), float(S.lv.T), float(S.cfg.get("pitchT", 8.5)))


# ======================================================================
# 8f) Velikost dílčího rmutu z tepelné bilance (A1)
# ======================================================================

## Jak velký díl rmutu povařit, aby po vrácení mělo celé dílo cílovou teplotu (Kosař s. 143: po prvním rmutu 62–64 °C,
## po druhém odrmutovací teplota 75–78 °C). Bilance: x·Cm·Tv + (1 − x)·Cm·Tz + Cs·Ts = (Cm + Cs)·Tc, kde Tv je teplota
## vraceného rmutu (var − 4,5 K na cestu), Tz teplota zbytku díla, Cs a Ts tepelná kapacita a teplota oceli kádě.
## Automat dřív bral pevně polovinu a pak dvě třetiny – to sedělo jen se silně chladnoucími nádobami.
## n = 1 první rmut (celé dílo je v pánvi), n = 2 druhý rmut (dílo je ve scezovací kádi). old = původní podíl automatu,
## vrátí se, když je cílová teplota (decoT1/decoT2) nastavená na 0. Podíl se spočítá jednou na začátku kroku automatu.
static func deco_frac(sim, n: int, old: float) -> float:
	var Tc: float = cf(sim, "decoT1") if n == 1 else cf(sim, "decoT2")
	if Tc <= 0.0: return old
	var S = sim.S; var c = S.cfg
	var key := "%d" % n
	var mn = sim.get("main")
	if mn != null and mn.get("auto") != null and mn.auto.get("t0") != null: key = "%d|%s" % [n, str(mn.auto.t0)]
	if sim.has_meta("fpDeco"):
		var dm = sim.get_meta("fpDeco")
		if dm is Dictionary and dm.get("key", "") == key and mn != null: return float(dm.x)
	var v = S.rvp if n == 1 else S.sk.mash
	var Cm: float = float(sim.cC(v))
	if Cm < 100.0: return old
	var Tz: float = float(v.T)
	var Cs: float = float(c.get("skSteel", 500.0)) * float(c.get("cpSteel", 0.5))
	var Ts: float = float(S.sk.get("Tw", Tz)) if n == 1 else Tz
	var Tv: float = float(c.get("tbWater", 99.3)) - 4.5      # rmut cestou ohřívá studené potrubí a čerpadlo (v modelu ~4–5 K)
	if Tv - Tz < 5.0 or Tc <= Tz: return old
	var x: float = clampf(((Cm + Cs) * Tc - Cm * Tz - Cs * Ts) / (Cm * (Tv - Tz)), 0.15, 0.6)
	sim.set_meta("fpDeco", {"key": key, "x": x})
	if sim.has_method("logm") and mn != null and mn.get("auto") != null and bool(mn.auto.get("on")):
		sim.logm("AUTO: %s rmut – povařit %d %% díla, aby po vrácení mělo dílo %s °C (teď %s °C)." % ["první" if n == 1 else "druhý", roundi(x * 100.0), f2(Tc), f2(Tz)])
	return x


# ======================================================================
# 8g) Kyslík v zákvasné mladině (H20)
# ======================================================================

## Rozpustnost kyslíku ve vodě nasycené vzduchem při 1 atm [mg/L], 0–40 °C (standardní tabulka, APHA/Weiss).
## 0 °C 14,6 · 10 °C 11,3 · 20 °C 9,1 · 25 °C 8,3 mg/L.
static func o2_water(T: float) -> float:
	var t: float = clampf(T, 0.0, 40.0)
	return 14.62 - 0.3898 * t + 0.006969 * t * t - 0.00005897 * t * t * t

## Rozpustnost v mladině [mg/L]: rozpuštěný extrakt kyslík vytěsňuje. o2PlatoK 0,017/°P je ODHAD
## (ve 12% mladině je rozpustnost zhruba o pětinu nižší než ve vodě); knihy udávají jen výsledek 8–10 mg/L.
static func o2_sat(sim, T: float, P: float) -> float:
	return maxf(0.0, o2_water(T) * (1.0 - cf(sim, "o2PlatoK") * maxf(0.0, P)))

## Kolik kyslíku má mladina při spílání do spilky [mg/L]. Dřív to bylo natvrdo 8 mg/L při zapnutém provzdušnění
## bez ohledu na teplotu a stupňovitost. Vzduchem se plné nasycení nedosáhne (aerEff 0,9 ODHAD), čistým kyslíkem
## by to šlo výš. Kunze s. 353 a VLB: 8–10 mg/L u zákvasné mladiny; Kosař s. 188: 5–6 mg/L u flotace.
## Teplá mladina pobere kyslíku míň – model to teď ukáže.
static func o2_dose(sim, T: float, P: float) -> float:
	var rest: float = cf(sim, "o2Rest")
	if not bool(sim.S.get("aer", false)): return rest
	return maxf(rest, o2_sat(sim, T, P) * clampf(cf(sim, "aerEff"), 0.0, 1.5))

# ======================================================================
# 9) Geometrie nádob, hladina
# ======================================================================

## vnitřní průměr nádoby [m]: parametr <nádoba>DiaM; 0 = odhad z objemu (ODHAD): pánev výška = průměr,
## vířivka průměr : výška mladiny = 3 : 1 (Kunze s. 340–341). Plánek je schéma, skutečné rozměry v něm nejsou.
static func vessel_dia(sim, key: String) -> float:
	var d: float = cf(sim, key + "DiaM")
	if d > 0.05: return d
	var cap_m3: float = float(sim.S.cfg.get(key + "Cap", 5000.0)) / 1000.0
	if key == "wp": return pow(12.0 * cap_m3 / PI, 1.0 / 3.0)
	return pow(4.0 * cap_m3 / PI, 1.0 / 3.0)

## plocha hladiny [m²] a hloubka kapaliny [m] (válec s rovným dnem)
static func surface_m2(sim, key: String) -> float:
	var d: float = vessel_dia(sim, key)
	return PI * d * d / 4.0

## hmotnost náplně [kg]
static func mass_kg(sim, v) -> float:
	return float(v.w) + float(sim.ext(v)) + float(sim.sol(v))

static func depth_m(sim, key: String, v) -> float:
	var rho: float = float(sim.density(v)) if sim.has_method("density") else 1.0       # kg/L při teplotě náplně
	return mass_kg(sim, v) / maxf(0.5, rho) / 1000.0 / maxf(0.1, surface_m2(sim, key))

## Bod varu u dna [°C]: tlak u dna = tlak nad hladinou (z tbWater) + tíha náplně na plochu dna (m·g/A) → teplota sytosti
## (IAPWS-IF97) + zvýšení extraktem (stejné 0,03 K/°P jako v heat()). Kunze s. 290: 2,5 m mladiny = +0,25 bar u dna, tj. 106 °C.
## Pára v plášti musí být teplejší než tohle číslo, jinak se u dna bubliny netvoří.
static func boil_T_bottom(sim, key: String, v) -> float:
	var c = sim.S.cfg
	var p0: float = psat(float(c.get("tbWater", 99.3)))
	var p: float = p0 + mass_kg(sim, v) * 9.81 / maxf(0.1, surface_m2(sim, key)) / 1.0e5
	return tsat(p) + 0.03 * float(sim.plato(v))


# ======================================================================
# 9b) Odpar z horké hladiny před varem a ve vířivce (H12)
# ======================================================================

## Odpar z volné hladiny do klidného vzduchu [kg/(m²·h)] – ASHRAE Handbook 1989, kap. 20, rov. 1 (v SI):
## E = 0,089·(p_w − p_a)/h_fg [kg/(m²·s)], tlaky v kPa, h_fg v kJ/kg. Při 90 °C ~10 kg/(m²·h) ≈ 6 kW/m².
## Vzduch v hale 50 % relativní vlhkosti (ODHAD – u horké hladiny je vliv zanedbatelný).
static func evap_open(T: float, Tamb: float) -> float:
	var pw: float = psat(T) * 100.0
	var pa: float = 0.5 * psat(Tamb) * 100.0
	return maxf(0.0, 0.089 * (pw - pa) / hfg(T)) * 3600.0

## Krok odparu z hladiny nádoby, která nevře (var počítá heat() z bilance energie). evapOpen = jak moc je hladina otevřená:
## 1 = volná hladina (otevřený průlez), výchozí 0,1 = zavřená nádoba s odtahem (ODHAD). Knihy měřený odpar ani pokles teploty
## ve vířivce neuvádějí; uvádějí jen, že mladina přichází na chladič s 95–98 °C (Kunze s. 350, Kosař s. 184) – odpar je tedy malý.
## Odpařená voda ubude z náplně a výparné teplo ji ochladí. Vrací odpařené kg.
static func surface_evap(sim, v, key: String, dt: float) -> float:
	if dt <= 0.0 or bool(v.get("boiling", false)): return 0.0
	if float(v.w) < 50.0 or float(v.T) < 40.0: return 0.0
	var k: float = cf(sim, "evapOpen")
	if k <= 0.0: return 0.0
	var c = sim.S.cfg
	var ev: float = evap_open(float(v.T), float(c.get("ambientT", 20.0))) * k * surface_m2(sim, key) * dt / 3600.0
	ev = minf(ev, 0.01 * float(v.w))
	var C: float = sim.cC(v) + sim.steelC(float(c.get(key + "Steel", 0.0)))
	v.w -= ev
	v.T -= ev * hfg(float(v.T)) / maxf(1.0, C)
	sim.S["evapTot"] = float(sim.S.get("evapTot", 0.0)) + ev
	sim.S["evapPre"] = float(sim.S.get("evapPre", 0.0)) + ev
	return ev


# ======================================================================
# 9c) Rychlost rmutu v potrubí (H13)
# ======================================================================

## největší rychlost [m/s] na trase při průtoku Q [L/min] (nejužší trubka)
static func _path_vel(sim, path: Array, Q: float) -> float:
	var dmm: float = 1e9
	for st in path:
		if st is Dictionary and st.has("e"): dmm = minf(dmm, float(sim.bore_mm(float(st.e.dn))))
	if dmm > 1e8: return 0.0
	var d: float = dmm / 1000.0
	return Q / 60000.0 / (PI * d * d / 4.0)

## Kunze s. 903: rmut do scezovací kádě pod 1,5 m/s (Kosař s. 140: 1,2–1,5 m/s). Rychlejší proudění drtí pluchy a smykem
## uvolňuje β-glukany → vyšší viskozita sladiny a pomalejší scezování (Kunze s. 222: smyk zvedne viskozitu sladiny
## průměrně o 0,22, nejvýš o 0,93 mPa·s). Účinek jde do stejné veličiny S.gel jako smyk míchadla; míra je stejný ODHAD.
## path = sací trasa (teče jí celý průtok), dpaths = výtlačné trasy (průtok se mezi ně dělí); bere se nejužší místo.
static func transfer_check(sim, key: String, src: String, part, Qf: float, path: Array, m: float, dpaths: Array = []) -> void:
	if part == null or Qf <= 0.0 or m <= 0.0 or not sim.has_method("bore_mm"): return
	var vel: float = _path_vel(sim, path, Qf)
	var nd: float = maxf(1.0, float(dpaths.size()))
	for dp in dpaths:
		if dp is Array: vel = maxf(vel, _path_vel(sim, dp, Qf / nd))
	if vel <= 0.0: return
	var S = sim.S
	if S.has(key) and S[key] is Dictionary: S[key]["vpipe"] = vel
	if sim.contentKind(part) != "mash": return
	var vmax: float = cf(sim, "mashVmax")
	if vel <= vmax: return
	S["gel"] = minf(1.0, float(S.get("gel", 0.0)) + clampf((vel - vmax) / 1.0, 0.0, 1.5) * 0.02 * m)
	sim.once("fpmashv%s%d" % [key, sim.tb(600)], "%s: rmut teče potrubím %s m/s – víc než %s m/s (Kunze s. 903, Kosař s. 140). Pluchy se drtí a smykem se uvolňují β-glukany, scezování půjde hůř. Uber otáčky čerpadla." % [key.to_upper().replace("P", "C"), sim.fmt1(vel), sim.fmt1(vmax)], "warn")


# ======================================================================
# 10) Doplňkové řádky do stavového panelu (háček U3)
# ======================================================================

static func panel_lines(sim) -> Array:
	var out: Array = []
	var S = sim.S
	# deskový chladič: průtoky a teplota ohřáté vody
	var plates = S.get("plates", {})
	if plates is Dictionary:
		for id in plates:
			var st = plates[id]
			if not (st is Dictionary) or st.get("kind", "") != "ice": continue
			var qw: float = float(S.get("plateFlow", {}).get(id, 0.0))
			var qi: float = float(st.get("flow", 0.0))
			if qw < 1.0 or qi < 0.5: continue
			var t: String = "   chladič: mladina %d L/min · ledová voda %d L/min (%s : 1)" % [roundi(qw), roundi(qi), sim.fmt1(qi / qw)]
			if st.has("Tcout"): t += " · ohřátá voda %d °C" % roundi(float(st.Tcout))
			out.append(t)
	# pánev: bod varu u dna (hydrostatický tlak)
	for key in ["rvp", "mp"]:
		var v = S.get(key, null)
		if not (v is Dictionary) or sim.vol(v) < 300.0: continue
		if float(v.T) < 95.0 and not bool(v.get("boiling", false)): continue
		out.append("   %s: hloubka %s m → u dna vře až při %s °C" % [_vn(sim, key), sim.fmt1(depth_m(sim, key, v)), sim.fmt1(boil_T_bottom(sim, key, v))])
	return out
