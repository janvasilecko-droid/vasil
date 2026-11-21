extends RefCounted
## FYZIKA+ pro tento program (přenos z balíčku pro starší trenažér, 17. 11. 2026).
## 1. část: tepelné ztráty nádob podle výpočtu a odpar z horké hladiny mimo var.
## Všechno počítání je tady; sim.gd odsud volá jen losses(). Postup výpočtu čísel: VYPOCTY.md v balíčku fyzika-plus.

## Ztráty plné nádoby do okolí [kW/K]. Vypočteno pro izolované nádoby (80 mm minerální vlny, 0,53 W/(m²·K), přirážka 20 %
## na hrdla a nohy, průlez a parník): pánev, vířivka a scezovací káď ~0,02; bojler 0,015; ledovka a spilka 0,01.
## Dřív chladly všechny nádoby stejně rychle (časová konstanta 12 h), což u plné pánve dělalo ~0,39 kW/K – jako holá nerez.
const UA_FULL := {"hlt": 0.015, "rvp": 0.02, "sk": 0.02, "wp": 0.02, "spilka": 0.01, "lv": 0.01}
## suchá stěna (nad hladinou a víko) ztrácí proti smáčené: izolovaná nádoba 0,95 (odpor izolace 1,78 m²K/W + vně 0,125,
## uvnitř nad hladinou navíc ~0,1)
const DRY_WALL_K := 0.95
## Odpar z hladiny mimo var vůči volné hladině v klidném vzduchu (ASHRAE): 0,1 = zavřená nádoba s odtahem.
## S vypočtenými ztrátami dává 0,1 teplotu mladiny na chladič 95–97 °C (Kosař s. 180, Kunze s. 352).
const EVAP_OPEN := 0.1
## nádoby s parníkem / otevřeným odtahem, ze kterých se horká hladina odpařuje
const EVAP_VESSELS := ["rvp", "wp", "sk"]

## tlak syté vodní páry [kPa] (Antoine, 1–100 °C)
static func psat_kpa(t: float) -> float:
	return 0.13332 * pow(10.0, 8.07131 - 1730.63 / (233.426 + clampf(t, 0.0, 110.0)))

## odpar z volné hladiny v klidném vzduchu [kg/(m²·h)] (ASHRAE 1989, kap. 20): 0,089·(pw − pa)/hfg; vzduch v hale 50 % vlhkosti
static func evap_open(t: float, t_amb: float) -> float:
	var pw: float = psat_kpa(t); var pa: float = 0.5 * psat_kpa(t_amb)
	var hfg: float = 2501.0 - 2.36 * t
	return maxf(0.0, 0.089 * (pw - pa) / hfg * 3600.0)

## vnitřní průměr nádoby [m] odhadem z objemu: vířivka 3 : 1 (průměr : výška), ostatní výška = průměr
static func vessel_dia(key: String, cap_l: float) -> float:
	var m3: float = cap_l / 1000.0
	if key == "wp" or key == "sk":
		return pow(12.0 * m3 / PI, 1.0 / 3.0)
	return pow(4.0 * m3 / PI, 1.0 / 3.0)

## násobek ztrát podle zaplnění (1 = plná): smáčená stěna naplno, suchá × DRY_WALL_K
static func loss_k(key: String, cap_l: float, v_l: float) -> float:
	var d: float = vessel_dia(key, cap_l)
	var a: float = PI * d * d / 4.0
	var side: float = PI * d * (cap_l / 1000.0 / a)
	var z: float = clampf(v_l / maxf(cap_l, 1.0), 0.0, 1.0)
	return (a + z * side + DRY_WALL_K * ((1.0 - z) * side + a)) / (a + side + DRY_WALL_K * a)

## Tepelné ztráty všech nádob za krok dt [s] a odpar z horké hladiny (volá sim.step místo dřívějšího jednotného chladnutí).
## Vrací odpařenou vodu [L] za krok (pro bilanci a testy).
static func losses(sim, dt: float) -> float:
	var evap_total := 0.0
	for k in sim.vessels:
		if not UA_FULL.has(k):
			continue
		var vs: Dictionary = sim.vessels[k]
		var v: float = float(vs["v"])
		if v <= 0.5:
			continue
		var t: float = float(vs["t"])
		var amb: float = sim.cal_ambient
		var c: float = v * sim.C_WATER + float(vs.get("malt", 0.0)) * sim.C_MALT + float(vs.get("steel", 0.0)) * sim.STEEL_C
		var cap_l: float = float(vs["cap"])
		var q_kj: float = float(UA_FULL[k]) * loss_k(k, cap_l, v) * (t - amb) * dt
		# odpar z hladiny: jen když nádoba nevře (za varu ho počítá _heat) a je teplejší než okolí
		if EVAP_VESSELS.has(k) and t > amb + 5.0 and float(vs.get("boil", 0.0)) < 0.05:
			var d: float = vessel_dia(k, cap_l)
			var m: float = minf(evap_open(t, amb) * EVAP_OPEN * (PI * d * d / 4.0) * dt / 3600.0, v - 0.5)
			if m > 0.0:
				vs["v"] = v - m
				q_kj += m * sim.L_VAP
				evap_total += m
		vs["t"] = t - q_kj / maxf(c, 1.0)
	return evap_total


## Chladnutí obsahu trubek: holá nerezová trubka Ø 50 mm plná vody, U ≈ 10 W/(m²·K) (volná konvekce + sálání),
## s ocelí trubky vychází časová konstanta asi 1,5 h (Ø 75 mm by měla 2,3 h). Sim průměry trubek nezná, bere se jedna
## hodnota pro všechny. Tekoucí trubky se plní novým obsahem, takže se to projeví jen u stojících.
const PIPE_TAU_S := 5400.0

static func pipe_cooling(sim, dt: float) -> void:
	var k: float = 1.0 - exp(-maxf(dt, 0.0) / PIPE_TAU_S)
	var amb: float = sim.cal_ambient
	for pid in sim.pipe_hold:
		var e: Dictionary = sim.pipe_hold[pid]
		if float(e.get("v", 0.0)) > 0.01:
			e["t"] = float(e["t"]) - (float(e["t"]) - amb) * k


## Kalový kužel ve vířivce. Tvar: kal po varu zabírá asi 0,6 % objemu mladiny (ODHAD), základna kužele nechává u stěny
## 60 cm bez kalu (Kosař s. 181), nejvýš 3/4 průměru dna. Kužel se rozpadá, když hladina v jeho výšce klesá rychleji než
## CONE_V (0,6 cm/min, odvozeno z praxe: běžné spílání 40 hl za 50–60 min se má u kužele zpomalit). Průtok q_real_lpm je
## ve skutečném měřítku (program čerpá FLOW_SCALE× rychleji, mez se tím přepočítá sama).
## Vrací podíl usazeného kalu, který se chová jako zvířený (0 = kužel drží, nejvýš CONE_BREAK – ODHAD).
const CONE_V := 0.006      # m/min
const CONE_BREAK := 0.1
const CONE_TRUB_FRAC := 0.006
const CONE_ANNULUS := 0.6  # m

static func cone_height(cap_l: float) -> float:
	var d: float = vessel_dia("wp", cap_l)
	var rc: float = clampf(d / 2.0 - CONE_ANNULUS, 0.1 * d, 0.375 * d)
	return 3.0 * (cap_l * 0.8 * CONE_TRUB_FRAC / 1000.0) / (PI * rc * rc)

static func cone_break(v_l: float, cap_l: float, q_real_lpm: float) -> float:
	var d: float = vessel_dia("wp", cap_l)
	var a: float = PI * d * d / 4.0
	if v_l / 1000.0 / a >= cone_height(cap_l) or q_real_lpm <= 0.0:
		return 0.0
	var u: float = q_real_lpm / 1000.0 / a
	return CONE_BREAK * clampf((u - CONE_V) / CONE_V, 0.0, 1.0)

## největší průtok z vířivky ve skutečném měřítku [L/min], při kterém kužel ještě drží
static func cone_safe_lpm(cap_l: float) -> float:
	var d: float = vessel_dia("wp", cap_l)
	return CONE_V * PI * d * d / 4.0 * 1000.0


## Rychlost v potrubí [m/s] při průtoku ve skutečném měřítku; sim průměry trubek nezná, bere se světlost 50 mm (v plánu
## jsou trubky DN 50–55). Rmut má téct pod 1,5 m/s (Kunze s. 903; Kosař s. 140: 1,2–1,5 m/s).
const PIPE_BORE_M := 0.05
const MASH_VMAX := 1.5

static func pipe_velocity(q_real_lpm: float) -> float:
	return q_real_lpm / 60000.0 / (PI * PIPE_BORE_M * PIPE_BORE_M / 4.0)

