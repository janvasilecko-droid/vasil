class_name BrewSim
extends RefCounted
const FyzikaPlus = preload("res://scripts/fyzika_plus.gd")  # FYZIKA+: tepelné ztráty a odpar
## Zjednodušená fyzika varu nad trasami z plan.json (objemy, teploty, extrakt, Darcyho filtrace).
## Nejde o přesný model pivovaru – je to základ, který lze kalibrovat na reálné várce.

const C_WATER := 4.186  # kJ/(kg·K)
const C_MALT := 1.7
const L_VAP := 2257.0  # kJ/kg
const AMBIENT := 20.0
const MAINS_T := 12.0
## měřítko: varna má max. 50 hl a vaří se 40–45 hl (dřív byl model v desetinách – 300 l)
const VOL_SCALE := 16.667  # násobek objemů, ploch a hmotností proti původnímu malému modelu
const FLOW_SCALE := 5.0  # průtoky čerpadel (L/min): 40 hl se přečerpá za pár minut, ne za hodiny
const BED_L_PER_KG := 1.1  # litrů lože mláta na kg sladu
const MALT_L_PER_KG := 0.7  # objem sladového šrotu v rmutu
const EXTRACT_YIELD := 0.78  # kg extraktu na kg sladu při plné přeměně
const XF_DEFAULT := {"f": 0.8, "dx": 0.1, "ld": 0.07, "ns": 0.03, "sg": 0.0}  # složení extraktu běžné mladiny (podíly)
const GEL_K := 0.07  # 1/min rychlost rozpouštění škrobu ze zrn nad ~62 °C (nastaveno na dosavadní rychlost zcukření)
const NI_SHARE := 0.22  # podíl extraktu ze sladu, který není škrob (cukry, bílkoviny) a rozpustí se hned
var enz_mean: float = 1.0  # enzymová síla zásypu (průměr podle sladů receptury: plzeňský 1, tmavé a karamelové míň)
var ph_a: float = 1.0  # vliv pH rmutu na α-amylázu (nastavuje rozvaděč podle vody a kyseliny)
var ph_b: float = 1.0  # totéž pro β-amylázu
const STEEL_C := 0.5  # kJ/(kg·K) nerez AISI 304 (PARAMETRY.md: cpSteel); hmotnosti ocelí nádob jsou ve vessels["…"]["steel"]

## ---------- kalibrace parametrů (kalibrace.json ve složce dat aplikace) ----------
## Hodnoty modelu, které jde přepsat změřenou hodnotou (podle tabulky parametrů původního trenažéru: PARAMETRY.md). Soubor má
## tvar {"UA1": {"value": 28, "source": "MEASURED"}, "tbWater": 99.3, …}; hodnoty mimo rozsah se odmítnou a vypíšou do hlášení.
const CAL_PARAMS := {
	"ambientT": {"var": "cal_ambient", "lo": 0.0, "hi": 40.0, "txt": "teplota ve varně (°C)"},
	"coolWaterT": {"var": "cal_mains_t", "lo": 4.0, "hi": 25.0, "txt": "teplota vody z řadu (°C)"},
	"tbWater": {"var": "cal_boil_t", "lo": 90.0, "hi": 105.0, "txt": "bod varu vody podle nadmořské výšky (°C)"},
	"extractYield": {"var": "cal_extract_yield", "lo": 0.5, "hi": 0.95, "txt": "kg extraktu na kg sladu při plné přeměně"},
	"gelK": {"var": "cal_gel_k", "lo": 0.005, "hi": 1.0, "txt": "rychlost rozpouštění škrobu (1/min)"},
	"boilerKW": {"var": "cal_boiler_kw", "lo": 50.0, "hi": 3000.0, "txt": "výkon parního kotle (kW)"},
	"boilerWaterKg": {"var": "cal_boiler_water_kg", "lo": 50.0, "hi": 20000.0, "txt": "voda v kotli (kg), určuje pokles tlaku"},
	"UA1": {"var": "cal_ua_water", "lo": 1.0, "hi": 200.0, "txt": "nepoužívá se – chladič je jednostupňový, platí UAice"},
	"UAice": {"var": "cal_ua_ice", "lo": 1.0, "hi": 400.0, "txt": "deskový chladič, ledová voda (kW/K)"},
}
const CAL_PUMPS := ["p1", "p2", "p3", "phw", "plv"]
var cal_ambient: float = AMBIENT
var cal_mains_t: float = MAINS_T
var cal_extract_yield: float = EXTRACT_YIELD
var cal_gel_k: float = GEL_K
var cal_boiler_kw: float = BOILER_KW
var cal_boiler_water_kg: float = BOILER_WATER_KG
var cal_ua_water: float = UA_WATER
var cal_ua_ice: float = UA_ICE
var cal_boil_t: float = BOIL_T
var pump_cal: Dictionary = {}  # bind čerpadla ("p1" …) -> {"H0": …, "Qn": …} přepsané kalibrací (p1_H0, p1_Qn …)


## vrátí hlášení o každé hodnotě: použito / odmítnuto / neznámý parametr
func apply_calibration(data: Dictionary) -> Array:
	var msgs: Array = []
	for key in data:
		var raw: Variant = data[key]
		var val: Variant = raw["value"] if typeof(raw) == TYPE_DICTIONARY and (raw as Dictionary).has("value") else raw
		var src: String = str((raw as Dictionary).get("source", "")) if typeof(raw) == TYPE_DICTIONARY else ""
		if typeof(val) != TYPE_FLOAT and typeof(val) != TYPE_INT:
			msgs.append("Kalibrace %s: hodnota není číslo – odmítnuto." % str(key))
			continue
		var v: float = float(val)
		var ks := str(key)
		var tail: String = (" (%s)" % src) if src != "" else ""
		if CAL_PARAMS.has(ks):
			var p: Dictionary = CAL_PARAMS[ks]
			if v < float(p["lo"]) or v > float(p["hi"]):
				msgs.append("Kalibrace %s = %s mimo rozsah %s–%s – odmítnuto." % [ks, str(v), str(p["lo"]), str(p["hi"])])
			else:
				set(str(p["var"]), v)
				msgs.append("Kalibrace %s = %s použito%s – %s." % [ks, str(v), tail, str(p["txt"])])
			continue
		var parts: PackedStringArray = ks.split("_")
		if parts.size() == 2 and CAL_PUMPS.has(parts[0]) and parts[1] in ["H0", "Qn"]:
			var lo := 5.0 if parts[1] == "H0" else 20.0
			var hi := 100.0 if parts[1] == "H0" else 2000.0
			if v < lo or v > hi:
				msgs.append("Kalibrace %s = %s mimo rozsah %s–%s – odmítnuto." % [ks, str(v), str(lo), str(hi)])
			else:
				var c: Dictionary = pump_cal.get(parts[0], {})
				c[parts[1]] = v
				pump_cal[parts[0]] = c
				msgs.append("Kalibrace křivky čerpadla %s: %s = %s použito%s." % [parts[0], parts[1], str(v), tail])
			continue
		msgs.append("Kalibrace: neznámý parametr %s – přeskočeno." % ks)
	return msgs

## trasa -> odkud, kam, čerpadlo, průtok (L/min)
const ROUTES := {
	"voda_panev": {"from": "hlt", "to": "rvp", "lpm": 60.0, "label": "Voda z bojleru do varny"},
	"voda_rad": {"from": "mains", "to": "rvp", "lpm": 35.0, "label": "Voda z řadu do varny"},
	"rmut_sk": {"from": "rvp", "to": "sk", "lpm": 80.0, "label": "Rmut do scezovací kádě"},
	"dilo": {"from": "sk", "to": "rvp", "lpm": 0.0, "label": "Odtah sladiny do varny (Darcy)"},
	"podraz": {"from": "hlt", "to": "sk", "lpm": 50.0, "label": "Podrážení tryskami pod síty (zelená)"},
	"vyslazovani": {"from": "hlt", "to": "sk", "lpm": 45.0, "label": "Vyslazování sprchami (červená)"},
	"varna_virivka": {"from": "rvp", "to": "wp", "lpm": 70.0, "label": "Varna do vířivky (čerpadlo C1)"},
	"virivka_chladic": {"from": "wp", "to": "spilka", "lpm": 40.0, "label": "Vířivka přes chladič do spilky (C3)"},
	"lv_fill": {"from": "mains", "to": "lv", "lpm": 40.0, "label": "Napuštění ledové vody"},
	"glykol": {"from": "", "to": "", "lpm": 0.0, "label": "Glykol chladí ledovou vodu"},
	"ledovka": {"from": "", "to": "", "lpm": 0.0, "label": "Ledová voda do deskového chladiče (C5)"},
}

## čerpadlo, které musí běžet, aby trasa tekla ("" = tlak z řadu)
const ROUTE_PUMP := {
	"voda_panev": "pump_murbmzxy",
	"voda_rad": "",
	"rmut_sk": "pump_mur92hh5",
	"dilo": "pump_mur92hh5",
	"podraz": "pump_murbmzxy",
	"vyslazovani": "pump_murbmzxy",
	"varna_virivka": "pump_mur92hh5",
	"virivka_chladic": "pump_muratmau",
	"lv_fill": "",
	"glykol": "pump_murcksta",
	"ledovka": "pump_murcksta",
}
var routes: Dictionary = {}  # platné trasy: výchozí ROUTES + úpravy z editoru cest (from, to, lpm, label, pump, medium)
var vessels: Dictionary = {}
var active: Dictionary = {}  # název trasy -> true
var blocked: Dictionary = {}  # trasa -> true, pokud má zavřenou klapku
var flow_scale: Dictionary = {}  # trasa -> podíl průtoku čerpadla (síť dělí průtok mezi víc cílů)
var heater_kw: Dictionary = {"hlt": 0.0, "rvp": 0.0}
var clock: float = 0.0
var k_darcy: float = 1.0

# --- rozvaděč: termostaty, maximální výkony a rmutovací program ---
var setpoint: Dictionary = {"hlt": 78.0, "rvp": 65.0, "lv": 4.0}  # cílová teplota °C
var auto_mode: Dictionary = {"hlt": false, "rvp": false}  # termostat zapnut
var max_kw: Dictionary = {"hlt": 250.0, "rvp": 350.0}  # parní ohřev velkého zařízení
var prog: Array = [{"t": 63.0, "min": 40.0}, {"t": 72.0, "min": 20.0}, {"t": 78.0, "min": 5.0}]  # rmutovací prodlevy
var prog_run: bool = false
var prog_i: int = 0
var prog_hold_s: float = 0.0
var events: Array = []  # zprávy do protokolu (čte je panel)
var clog: float = 0.0  # zanesení potrubí pod síty mlátem 0..1 (snižuje průtok scezováním)
## zpoždění potrubí (jako push_path v původním net.gd): kapalina nejdřív naplní úseky, do cíle jde až přepad
var pipe_delay: bool = true
var pipe_caps: Dictionary = {}   # id trubky -> kapacita (L), nastaví sim_panel z plánu
var pipe_hold: Dictionary = {}   # id trubky -> {v, t, ext, malt, med}
var route_pipes: Dictionary = {} # id cesty -> [id trubek po směru toku], nastaví sim_panel při toku
const MALT_BATCH := 700.0  # dávka sladu na várku (kg) – tolik je na začátku v zásobníku nad šnekem
const MALT_FEED_KGPM := 70.0  # šnek nasype dávku asi za 10 minut
const SCREW_WATER_LPM := 15.0  # voda do šneku (z bojleru, kohout na čtvrtku) smáčí šrot cestou do pánve
var malt_stock: float = MALT_BATCH
# --- scezovací káď: výpusť do kanálu, kopačka (otáčení, hydraulický zdvih) a kopyto (vyhrnování mláta) ---
const SK_DRAIN_LPM := 300.0  # výpusť pod síty do kanálu samospádem (víc, než dají sprchy, ať káď při vyplachování neplní)
const RAKE_RPM := 1.0  # kopačka udělá jednu otáčku za minutu
const RAKE_TRAVEL_S := 20.0  # zdvih kopačky z dolní do horní polohy
const HOOF_KGPM := 60.0  # kopyto dole a kopačka v chodu vyhrnují mláto mlátovým otvorem (kg sladu za minutu)
const SPENT_RETAIN_L_PER_KG := 1.0  # sladina, kterou mláto zadrží a odnese s sebou
const SIEVE_FLOOD_L := 150.0  # tolik vody musí být v SK, aby byla síta pod vodou, až na ně dosedne rmut
var sk_drain_open: bool = false  # výpusť SK do kanálu otevřená
var rake_on: bool = false  # kopačka se otáčí
var rake_down: bool = false  # povel zdvihu kopačky: true = dolů, false = nahoru
var rake_target: float = -1.0  # cílová výška kopačky 0 (dole u sít) … 0,4 (střed, nože v loži) … 1 (nahoře); −1 = řídí rake_down
var hoof_down: bool = false  # kopyto dole (vyhrnování mláta)
var drain_lpm_now: float = 0.0  # co právě teče výpustí do kanálu (L/min)
var drain_l: float = 0.0  # celkem vypuštěno do kanálu (L)
var drain_ext: float = 0.0  # extrakt, který odtekl do kanálu (kg)
var spent_out: float = 0.0  # vyhrnuté mláto celkem (kg sladu včetně zbytků)
var _spent_batch: float = 0.0  # vyhrnuto při tomto vyhrnování
var spent_ext: float = 0.0  # extrakt, který odnesl s sebou vyhrnutý šrot (kg)
var malt_total: float = 0.0  # slad nasypaný od začátku várky (kg), základ pro výtěžnost
const O2_AIR_SAT := 14600.0  # ppb rozpuštěného O₂ ve vodě nasycené vzduchem při 0 °C (klesá s teplotou)
const O2_PICKUP := 0.25  # podíl nasycení, který mladina nabere cestou deskovým chladičem a hadicí
const O2_YEAST_TAU := 1800.0  # s, časová konstanta spotřeby O₂ kvasinkami po zakvašení
var o2_spilka: float = 0.0  # rozpuštěný kyslík ve spilce (ppb)
const CLUMP_FRAC := 0.04  # podíl sladu nasypaného do nemíchané vody, který zhrudkuje
const CLUMP_DRY := 0.5  # podíl hrudek, které se při přečerpání nerozpustí (suché jádro) a odejdou s mlátem
const CLUMP_REL_STIR_S := 300.0  # s, časová konstanta rozbití hrudek míchadlem (bez míchadla 3600 s)
const HOT_BREAK_PER_H := 0.012  # podíl extraktu, který se při hodinovém chmelovaru vysráží s bílkovinami (hot break)
const CONE_LOSS := 0.005  # podíl extraktu, který zůstane vázaný v kalovém kuželu usazené vířivky při čerpání
var dead_malt: float = 0.0  # kg sladu, jehož extrakt zůstal v suchých hrudkách (nerozpustí se)
var hot_break_ext: float = 0.0  # extrakt vysrážený v hot breaku při chmelovaru (kg)
var kal_ext: float = 0.0  # extrakt zachycený v kalu na dně vířivky (kg)
var evap_idle_l: float = 0.0  # FYZIKA+: voda odpařená z horké hladiny mimo var (L)
var yeast_pitched: bool = false  # kvasinky jsou v tanku (nastavuje rozvaděč při zakvašení); spotřebovávají O₂
var _said: Dictionary = {}  # jednorázová hlášení (klíč -> true), klíč se smaže, když pomine příčina
# --- spílání: spílací hadice na vratce (zpět do vířivky) nebo na tanku, voda stojící v chladiči a potrubí ---
const LINE_L := 60.0  # chmelový filtr, deskový chladič, potrubí a spílací hadice (L)
const PITCH_T := 8.5  # zakvašovací teplota (°C)
var hose_tank: bool = true  # spílací hadice na tanku (false = na vratce: mladina se vrací do vířivky)
var line_water: float = LINE_L  # voda po vyplachování stojí ve filtru, chladiči a hadici – vyteče jako první
var line_ext: float = 0.0  # extrakt mladiny, která zůstala v potrubí místo vody (dostane se do tanku protláčkou)
var cool_out_t: float = -1.0  # teplota mladiny za chladičem (-1 = neteče)
var water_to_tank: float = 0.0  # voda z potrubí, která natekla do spilky (L)
var _cold_return_s: float = 0.0  # jak dlouho už teče na vratku ledová mladina (s)
var _line_t: float = PITCH_T  # teplota mladiny, která zůstala v potrubí
# --- protláčka a víko spilky ---
var push_flow: bool = false  # protláčka: voda z řadu do sání C3 a C3 běží (nastavuje panel podle kohoutů)
var pushed_wort: float = 0.0  # mladina dotlačená protláčkou z potrubí do spilky (L)
var push_water_tank: float = 0.0  # voda z protláčky, která natekla do spilky (L)
var tank_lid: bool = false  # spilka zavíčkovaná
var _lid_was: bool = false
var _tank_inflow: bool = false  # v tomto kroku do spilky něco teče
# --- rozvaděč RT.20: parní kotel, porucha čerpadla, zanesený chladič ---
const BOILER_KW := 650.0  # jmenovitý výkon parního kotle (kW páry)
const STEAM_BAR := 2.5  # provozní přetlak páry (bar)
const PLATE_EFF := 0.94  # účinnost čistého deskového chladiče (zanesený chladič: plate_eff < PLATE_EFF zmenší UA)
const UA_WATER := 25.0  # kW/K 1. stupeň deskového chladiče (voda z řadu), z původního trenažéru (PARAMETRY.md: UA1)
const UA_ICE := 207.0  # FYZIKA+: jediný stupeň chladiče; 207 kW/K dává při C3 200 L/min, ledové 200 L/min a 3 °C z 95 °C na ~8,5 °C (dřív 76 za stupněm s vodou z řadu) # kW/K 2. stupeň (ledová voda); původně 65, tady nastaveno tak, aby při C3 200 L/min a C5 200 L/min vycházelo ~8,5 °C jako dosud
const ICE_LPM := 200.0  # průtok ledové vody čerpadlem C5 chladičem (L/min): stejný jako mladiny, ať ledovka (4200 L) vystačí na celou várku (3440 L)
var steam_on: bool = true  # parní kotel zapnutý – bez páry ohřev nejde
var steam_bar: float = STEAM_BAR  # nejvyšší tlak, který hořák udrží (porucha ho sníží)
const BOILER_WATER_KG := 1500.0  # voda v kotli: zásoba energie, která drží tlak (PARAMETRY.md: boilerWaterKg)
const BOILER_LOSS_KW := 15.0  # ztráta vypnutého kotle
var steam_p: float = STEAM_BAR  # skutečný přetlak páry v kotli (klesá při přetížení, roste zpět)
var boiler_load: float = 0.0  # zátěž kotle 0..1
var pump_fault: Dictionary = {}  # id čerpadla -> "air" (zavzdušněné) / "broken" (spálená ucpávka): běží, ale nečerpá
var plate_eff: float = PLATE_EFF  # zanesený chladič chladí hůř


func _init() -> void:
	reset()
	load_routes({})
	# kalibrace Darcy: při 75 °C, loži 15 cm a hladině 15 cm nad ložem má téct 15 L/min
	var mu75: float = viscosity(75.0)
	k_darcy = 15.0 * mu75 * 0.15 / (1050.0 * 9.81 * (0.15 + 0.075))


func reset() -> void:
	clock = 0.0
	steam_p = STEAM_BAR
	malt_stock = MALT_BATCH
	sk_drain_open = false
	rake_on = false
	rake_down = false
	hoof_down = false
	drain_lpm_now = 0.0
	drain_l = 0.0
	drain_ext = 0.0
	spent_out = 0.0
	_spent_batch = 0.0
	spent_ext = 0.0
	malt_total = 0.0
	o2_spilka = 0.0
	dead_malt = 0.0
	hot_break_ext = 0.0
	kal_ext = 0.0
	evap_idle_l = 0.0
	yeast_pitched = false
	_said.clear()
	line_water = 0.0
	line_ext = 0.0
	cool_out_t = -1.0
	water_to_tank = 0.0
	_cold_return_s = 0.0
	_line_t = PITCH_T
	pushed_wort = 0.0
	push_water_tank = 0.0
	active.clear()
	blocked.clear()
	flow_scale.clear()
	pipe_hold.clear()
	route_pipes.clear()
	heater_kw = {"hlt": 0.0, "rvp": 0.0}
	vessels = {
		"hlt": {"v": 0.0, "t": 15.0, "cap": 5000.0, "area": 8.3, "ext": 0.0, "malt": 0.0, "name": "Bojler (horká voda)", "steel": 600.0},
		"rvp": {"v": 0.0, "t": 15.0, "cap": 5000.0, "area": 15.0, "ext": 0.0, "malt": 0.0, "name": "Varna (rmutovací pánev)", "steel": 450.0},
		# nová várka začíná prázdná; kapalina vstupuje do nádoby až otevřenou cestou
		"sk": {"v": 0.0, "t": 15.0, "cap": 5000.0, "area": 5.8, "ext": 0.0, "malt": 0.0, "name": "Scezovací káď", "steel": 500.0, "bed": false, "turb": 1.0, "settle": 0.0,
			"res": 0.0, "fines": 0.0, "cloud": 0.0, "rake_h": 1.0, "pack": 0.0, "rev": 0.0},
		"wp": {"v": 0.0, "t": 15.0, "cap": 5000.0, "area": 15.0, "ext": 0.0, "malt": 0.0, "name": "Vířivka", "steel": 550.0},
		"spilka": {"v": 0.0, "t": 15.0, "cap": 5500.0, "area": 8.3, "ext": 0.0, "malt": 0.0, "name": "Spilka"},
		"lv": {"v": 0.0, "t": 14.0, "cap": 10000.0, "area": 20.0, "ext": 0.0, "malt": 0.0, "name": "Ledová voda"},
		# výpustě do kanálu z potrubní sítě: co do něj natéká, hned odteče (počítá se do drain_l)
		"kanal": {"v": 0.0, "t": 15.0, "cap": 1.0e9, "area": 1.0, "ext": 0.0, "malt": 0.0, "name": "Kanál"},
	}


## Trasy: výchozí definice z ROUTES/ROUTE_PUMP, přes ně se položí úpravy uložené v plánu z editoru cest
## (plan.raw["routes"][název] s klíči from, to, lpm, label, pump, medium). Nové trasy z editoru se přidají.
func load_routes(plan_routes: Dictionary) -> void:
	routes.clear()
	for rn in ROUTES:
		var r: Dictionary = (ROUTES[rn] as Dictionary).duplicate()
		r["pump"] = str(ROUTE_PUMP.get(rn, ""))
		routes[rn] = r
	for rn in plan_routes:
		var pr: Dictionary = plan_routes[rn]
		var r2: Dictionary = routes.get(rn, {"from": "", "to": "", "lpm": 40.0, "label": str(rn), "pump": ""})
		for k in ["from", "to", "label", "pump", "medium"]:
			if pr.has(k):
				r2[k] = str(pr[k])
		if pr.has("lpm"):
			r2["lpm"] = float(pr["lpm"])
		routes[rn] = r2


func route_pump(rn: String) -> String:
	return str(routes.get(rn, {}).get("pump", ""))


## dynamická viskozita vody v Pa·s (Vogelova rovnice)
func viscosity(t: float) -> float:
	return 2.414e-5 * pow(10.0, 247.8 / (t + 133.15))


func add_malt(kg: float) -> void:
	var rv: Dictionary = vessels["rvp"]
	_mix_viability(rv, kg, {"dA": 1.0, "dB": 1.0})  # čerstvý slad má plně živé enzymy
	rv["malt"] = float(rv["malt"]) + kg
	malt_total += kg


## smíchá živost enzymů (α, β) přisypaného sladu: ve výsledku je průměr podle hmotnosti sladu
func _mix_viability(dst: Dictionary, kg: float, src: Dictionary) -> void:
	var w0: float = float(dst["malt"])
	var tot: float = w0 + kg
	if tot <= 0.0:
		return
	for k in ["dA", "dB"]:
		dst[k] = (float(dst.get(k, 1.0)) * w0 + float(src.get(k, 1.0)) * kg) / tot


## Šnek na slad: otevřené šoupě a běžící šnek sypou slad ze zásobníku do pánve; voda do šneku (z bojleru)
## teče do pánve, i když šnek stojí. Vrací nasypané kg v tomto kroku.
func feed_malt(dt: float, gate_open: bool, screw_on: bool, water_open: bool) -> float:
	var kg := 0.0
	if gate_open and screw_on and malt_stock > 0.0:
		kg = minf(MALT_FEED_KGPM * dt / 60.0, malt_stock)
		malt_stock -= kg
		add_malt(kg)
		if not bool(stir.get("rvp", false)) and float(vessels["rvp"]["v"]) > 1.0:
			vessels["rvp"]["clump"] = float(vessels["rvp"].get("clump", 0.0)) + kg * CLUMP_FRAC  # do nemíchané vody padá slad do hrudek
	if water_open:
		var hl: Dictionary = vessels["hlt"]
		var q: float = minf(minf(SCREW_WATER_LPM * dt / 60.0, float(hl["v"])), _free_space("rvp"))
		if q > 0.0:
			hl["v"] = float(hl["v"]) - q
			_mix(vessels["rvp"], q, float(hl["t"]), 0.0)
	return kg


## Scezovací káď: hydraulický zdvih a otáčení kopačky, kopyto (vyhrnování mláta) a výpusť pod síty do kanálu
func _sk_machine(dt: float) -> void:
	var sk: Dictionary = vessels["sk"]
	var rake_to: float = rake_target if rake_target >= 0.0 else (0.0 if rake_down else 1.0)
	sk["rake_h"] = move_toward(float(sk.get("rake_h", 1.0)), clampf(rake_to, 0.0, 1.0), dt / RAKE_TRAVEL_S)
	var h: float = float(sk["rake_h"])
	var bed: bool = bool(sk.get("bed", false))
	if rake_on:
		sk["rev"] = float(sk.get("rev", 0.0)) + RAKE_RPM * dt / 60.0
		if bed and h < 0.5 and not hoof_down:
			# nože v loži: kypří utažené lože, ale zvíří jemné částice – sladina se na chvíli zakalí (vorlauf);
			# čím hlouběji (blíž k sítům), tím víc povolí a tím víc zakalí
			var depth: float = 1.0 - h / 0.5
			sk["pack"] = maxf(float(sk.get("pack", 0.0)) - dt / 90.0 * (0.4 + 0.6 * depth), 0.0)
			sk["turb"] = maxf(float(sk.get("turb", 0.0)), 0.2 + 0.4 * depth)
	# kopyto dole a kopačka v chodu: mláto (i zbytky z minulé várky) se vyhrnuje mlátovým otvorem ven
	var res: float = float(sk.get("res", 0.0))
	if hoof_down and rake_on and float(sk["malt"]) + res > 0.01:
		var kg: float = minf(HOOF_KGPM * dt / 60.0, float(sk["malt"]) + res)
		var from_res: float = minf(kg, res)
		sk["res"] = res - from_res
		var from_malt: float = kg - from_res
		spent_out += kg
		_spent_batch += kg
		if from_malt > 0.0:
			var m0: float = float(sk["malt"])
			var v0: float = float(sk["v"])
			var p0: float = plato("sk")
			# s mlátem odchází sladina v něm zadržená; rmut bez lože se vyhrnuje i s kapalinou
			var liq: float = minf(v0, from_malt * SPENT_RETAIN_L_PER_KG if bed else v0 * from_malt / m0)
			var ext_out: float = float(sk["ext"]) * liq / maxf(v0, 0.001)
			sk["malt"] = m0 - from_malt
			sk["v"] = v0 - liq
			sk["ext"] = float(sk["ext"]) - ext_out
			spent_ext += ext_out
			if p0 > 1.0:
				_say_once("kopyto_dilo", "Kopyto je dole a kopačka běží – vyhrnuješ ven dílo i se sladinou! Zastav kopačku a dej kopyto nahoru.")
		if float(sk["malt"]) + float(sk["res"]) <= 0.01:
			sk["malt"] = 0.0
			sk["res"] = 0.0
			if _spent_batch < 60.0:
				events.append("Zbytky mláta z minulé várky vyhrnuty (%.0f kg)" % _spent_batch)
			else:
				events.append("Mláto vyhrnuto (asi %.0f kg mokrého mláta)" % (_spent_batch * 1.3))
			_spent_batch = 0.0
	if not hoof_down:
		_said.erase("kopyto_dilo")
	# výpusť pod síty do kanálu (samospád)
	drain_lpm_now = 0.0
	if sk_drain_open:
		var v: float = float(sk["v"])
		var q: float = SK_DRAIN_LPM
		if bed:
			q = darcy_lpm()  # pod síty proteče jen to, co projde ložem
		elif float(sk["malt"]) > 1.0:
			q = SK_DRAIN_LPM * 0.5  # rmut sedá na síta, teče kalná sladina
		var dq: float = minf(q * dt / 60.0, v)
		if dq > 0.0:
			var p: float = plato("sk")
			var ext_out2: float = float(sk["ext"]) * dq / maxf(v, 0.001)
			if bed:
				ext_out2 = _sk_pull(sk, dq)  # výpusť pod síty bere ze spodní vrstvy lože
			sk["v"] = v - dq
			sk["ext"] = float(sk["ext"]) - ext_out2
			drain_lpm_now = dq * 60.0 / maxf(dt, 0.001)
			drain_l += dq
			drain_ext += ext_out2
			_flush_fines(sk, dq)
			if p > 0.5:
				_say_once("kanal_sladina", "Výpustí SK teče do kanálu sladina (%.1f °P)! Zavři výpusť." % p)
			if drain_turb() < 0.1:
				_say_once("cista", "Z výpusti SK už teče čistá voda.")
			if float(sk["v"]) < 0.5:
				_say_once("vypustena", "Scezovací káď je vypuštěná.")
	else:
		_said.erase("kanal_sladina")
	if drain_turb() > 0.3:
		_said.erase("cista")
	if float(sk["v"]) > 50.0:
		_said.erase("vypustena")


## zákal vody z výpusti SK 0..1 (kal z podsítí a zvířené částice); pod 0,1 teče čistá voda
func drain_turb() -> float:
	var sk: Dictionary = vessels["sk"]
	return clampf(float(sk.get("fines", 0.0)) * 1.5 + float(sk.get("cloud", 0.0)), 0.0, 1.0)


## voda tekoucí z podsítí ven (výpustí nebo čerpadlem) vyplachuje usazený kal
func _flush_fines(sk: Dictionary, liters: float) -> void:
	sk["fines"] = float(sk.get("fines", 0.0)) * exp(-liters / 500.0)
	sk["cloud"] = float(sk.get("cloud", 0.0)) * exp(-liters / 300.0)


func _say_once(key: String, text: String) -> void:
	if _said.has(key):
		return
	_said[key] = true
	events.append(text)


func plato(key: String) -> float:
	var vs: Dictionary = vessels[key]
	var v: float = float(vs["v"])
	var e: float = float(vs["ext"])
	if v + e <= 0.01:
		return 0.0
	return 100.0 * e / (v + e)


func starch_left() -> float:
	var m: float = float(vessels["rvp"]["malt"]) + float(vessels["sk"]["malt"])
	if m < 0.5:
		return 0.0
	var pot: float = m * cal_extract_yield
	var er: float = float(vessels["rvp"]["ext"])
	var es: float = float(vessels["sk"]["ext"])
	# škrob, který ještě není v roztoku (zrna) + rozpuštěný, ale nerozštěpený (nad 78 °C α-amyláza hyne a škrob zůstane)
	var unsol: float = maxf(pot - er - es, 0.0)
	var sg: float = er * float(_xf(vessels["rvp"])["sg"]) + es * float(_xf(vessels["sk"])["sg"])
	return clampf(100.0 * (unsol + sg) / pot, 0.0, 100.0)


## jodová zkouška: negativní, když zbývá méně než 5 % škrobu
func iodine_negative() -> bool:
	return starch_left() < 5.0


func bed_height(sk: Dictionary) -> float:
	return float(sk["malt"]) * BED_L_PER_KG / 1000.0 / float(sk["area"])


## průtok scezovacím ložem (L/min) podle Darcyho zákona
func darcy_lpm() -> float:
	var sk: Dictionary = vessels["sk"]
	if not bool(sk.get("bed", false)):
		return 0.0  # bez filtračního lože (SK ve stavu „rmut“) nic nescezuje
	var lb: float = bed_height(sk)
	if lb < 0.02 or float(sk["v"]) < 2.0:
		return 0.0
	var pores: float = float(sk["malt"]) * BED_L_PER_KG * 0.45
	var h_liq: float = maxf(float(sk["v"]) - pores, 0.0) / 1000.0 / float(sk["area"])
	var dp: float = 1050.0 * 9.81 * (h_liq + 0.5 * lb)
	var q: float = k_darcy * dp / (viscosity(float(sk["t"])) * lb)
	var fines: float = float(sk.get("fines", 0.0))
	var pack: float = clampf(float(sk.get("pack", 0.0)), 0.0, 1.0)  # utažené lože (bez kypření kopačkou)
	return minf(q * VOL_SCALE, 25.0 * VOL_SCALE) * (1.0 - clampf(clog, 0.0, 0.95)) * (1.0 - 0.7 * fines) * (1.0 - 0.6 * pack)


## ---------- stlačitelnost lože: tah čerpadla, utažení a trhání (podle sk_qmax / sk_draw původního trenažéru) ----------
## sk_demand_lpm = průtok, který chce vyslazovací čerpadlo C2 (měnič); 0 = bez omezení (čerpadlo bere, co lože pustí).
## Čerpadlo odebere nejvýš tolik, kolik lože propustí. Táhne-li nad ~70 % propustnosti, lože se utahuje (kvadraticky podle
## (poměr − 0,7)/0,3), nad 130 % to hlásí; pokles tlaku pod sítem dp = 20 + 180·Q/Qčisté·(1 + 2·utažení) mbar a nad prahem
## SK_DP_CHANNEL se lože trhá (kanálky): zákal a zrna jdou nahoru, dokud se tah nesníží.
const SK_DP_CHANNEL := 160.0  # mbar, práh vzniku kanálků (skDpChannel)
const SK_SUCTION_PA := 20000.0  # Pa, největší podtlak pod sítem, který C2 vytvoří (skSuction, PARAMETRY.md; vakuometr max. 200 mbar)
const SK_CREEP_K := 0.0015  # 1/min, pomalé sedání lože v klidu (skCreepK)
var sk_demand_lpm: float = 0.0


## škrcení odběru ze SK klapkou nebo křivkou čerpadla: největší flow_scale přes aktivní trasy, které ze SK berou (1 = nic neškrtí)
func _sk_throttle() -> float:
	var best := -1.0
	for rn in active:
		if str(routes.get(rn, {}).get("from", "")) == "sk":
			best = maxf(best, float(flow_scale.get(rn, 1.0)))
	return 1.0 if best < 0.0 else clampf(best, 0.0, 1.0)


func sk_draw_lpm() -> float:
	var cap: float = darcy_lpm()
	return minf(cap, sk_demand_lpm) if sk_demand_lpm > 0.0 else cap


func _sk_compaction(dt: float) -> void:
	var sk: Dictionary = vessels["sk"]
	var m: float = dt / 60.0
	var torn: float = float(sk.get("torn", 0.0))
	var cap: float = darcy_lpm()
	if bool(sk.get("bed", false)) and not sk_draw_active():
		sk["pack"] = minf(float(sk.get("pack", 0.0)) + (1.0 - float(sk.get("pack", 0.0))) * SK_CREEP_K * m, 1.0)  # lože v klidu pomalu sedá (skCreepK)
	if not bool(sk.get("bed", false)) or not sk_draw_active() or cap < 1.0:
		sk["torn"] = maxf(torn - 0.1 * m, 0.0)
		sk["dp"] = 20.0
		_said.erase("sk_pretazeni")
		return
	var demand: float = (sk_demand_lpm if sk_demand_lpm > 0.0 else 0.6 * cap) * _sk_throttle()  # bez nastaveného měniče ~60 % propustnosti; přivřená klapka za C2 tah zmenší
	var ratio: float = demand / cap
	var pack: float = clampf(float(sk.get("pack", 0.0)), 0.0, 1.0)
	if ratio > 0.7:
		sk["pack"] = minf(pack + 0.0075 * m * pow((minf(ratio, 2.5) - 0.7) / 0.3, 2.0), 1.0)
	if ratio > 1.3:
		_say_once("sk_pretazeni", "Čerpadlo táhne víc, než projde ložem – zpomal vyslazovací čerpadlo, jinak se lože utáhne.")
	elif ratio < 1.1:
		_said.erase("sk_pretazeni")
	var clean_cap: float = cap / maxf(1.0 - 0.6 * pack, 0.05)  # propustnost lože bez utažení
	var q: float = minf(cap, demand)
	var dp: float = minf(20.0 + 180.0 * (q / maxf(clean_cap, 1.0)) * (1.0 + 2.0 * pack), SK_SUCTION_PA / 100.0)  # víc podtlaku C2 nevytvoří (skSuction)
	sk["dp"] = dp
	if dp > SK_DP_CHANNEL:
		torn = minf(torn + (dp - SK_DP_CHANNEL) / 100.0 * 0.5 * m, 1.0)
		sk["turb"] = maxf(float(sk.get("turb", 0.0)), 0.3 + 0.5 * torn)  # kanálky pustí kal a zrna přímo k sítu
		if torn > 0.15:
			_say_once("sk_trhani", "Podtlak pod sítem je příliš velký (%.0f mbar) – lože se trhá, sladina je zakalená. Zpomal čerpadlo a nakypři lože." % dp)
	else:
		torn = maxf(torn - 0.1 * m, 0.0)
		if torn < 0.05:
			_said.erase("sk_trhani")
	sk["torn"] = torn


## ---------- scezovací lože po vrstvách (jako cells v původním trenažéru) ----------
## Kapalina v SK s ložem je rozdělená do 5 vrstev: [0] nad ložem (míchaná, sem teče sprcha) a [1]–[4] v pórech lože, každá
## s vlastním extraktem. Odběr jde ze spodní vrstvy a shora se doplňuje (pístový tok), takže první sladina je hustá a zředění
## sprchou dojde až s několika pórovými objemy. Součet vrstev se drží s vessels["sk"]["v"] a ["ext"] (ostatní kód je mění přímo).
const SK_CELLS := 5


func _sk_pores() -> float:
	return float(vessels["sk"]["malt"]) * BED_L_PER_KG * 0.45


func _sk_cells(sk: Dictionary) -> Array:
	var c: Array = sk.get("cells", [])
	if c.size() == SK_CELLS:
		return c
	var v: float = float(sk["v"])
	var e: float = float(sk["ext"])
	var cap: float = minf(_sk_pores() / 4.0, v / 4.0)
	var conc: float = e / maxf(v, 0.001)
	c = []
	for i in SK_CELLS:
		var w: float = cap if i > 0 else maxf(v - 4.0 * cap, 0.0)
		c.append({"w": w, "e": w * conc})
	sk["cells"] = c
	return c


## srovná vrstvy se součty nádoby: přidaná voda (sprcha, podrážení) jde do horní vrstvy, ubraná se bere shora,
## extrakt se přepočítá poměrně
func _sk_cells_sync() -> void:
	var sk: Dictionary = vessels["sk"]
	if not bool(sk.get("bed", false)) or float(sk["v"]) < 0.5:
		sk.erase("cells")
		return
	var c: Array = _sk_cells(sk)
	var sw := 0.0
	var se := 0.0
	for x in c:
		sw += float(x["w"])
		se += float(x["e"])
	var dw: float = float(sk["v"]) - sw
	if dw > 0.001:
		c[0]["w"] = float(c[0]["w"]) + dw
	elif dw < -0.001:
		var rem: float = -dw
		for i in SK_CELLS:
			var w: float = float(c[i]["w"])
			var take: float = minf(rem, w)
			if take > 0.0:
				c[i]["e"] = float(c[i]["e"]) * (1.0 - take / w)
				c[i]["w"] = w - take
				rem -= take
			if rem <= 0.0001:
				break
	se = 0.0
	for x in c:
		se += float(x["e"])
	var ext: float = float(sk["ext"])
	if se > 0.0 and absf(ext - se) > 0.0001:
		var k: float = ext / se
		for x in c:
			x["e"] = float(x["e"]) * k
	elif se <= 0.0 and ext > 0.0:
		c[0]["e"] = ext
	var last: Dictionary = c[SK_CELLS - 1]
	sk["outp"] = 100.0 * float(last["e"]) / maxf(float(last["w"]) + float(last["e"]), 0.001)  # °P sladiny, která teče ze spodní vrstvy


## Stav sladiny za ložem pro průhledítko a živý pohled (všechno 0..1, kromě ebc, °P a l/min):
## zakal = celkový zákal, kaly = jemné kaly z podsítí, zrna = kousky pluch a zrn (zvířené kopačkou, utažené lože), mlato = lože se
## trhá (hladina pod mlátem) nebo se vyhrnuje mláto, pack = utažení lože (zpomaluje průtok, kopačka ho povolí),
## ebc/barva = barva podle receptury a hustoty sladiny (první sladina tmavší), plato = °P ze spodní vrstvy, lpm = průtok ložem.
var recipe_ebc: float = 8.0  # barva mladiny receptury (EBC), nastavuje rozvaděč


func wort_view() -> Dictionary:
	var sk: Dictionary = vessels["sk"]
	var bed: bool = bool(sk.get("bed", false))
	var pores: float = _sk_pores()
	var turb: float = clampf(float(sk.get("turb", 0.0)), 0.0, 1.0)
	var fines: float = clampf(float(sk.get("fines", 0.0)), 0.0, 1.0)
	var cloud: float = clampf(float(sk.get("cloud", 0.0)), 0.0, 1.0)
	var pack: float = clampf(float(sk.get("pack", 0.0)), 0.0, 1.0)
	var plato_out: float = float(sk.get("outp", plato("sk")))
	var spent: bool = hoof_down and rake_on and float(sk["malt"]) + float(sk.get("res", 0.0)) > 0.01
	var dry: float = 0.0
	if bed and pores > 1.0:
		dry = clampf(1.0 - float(sk["v"]) / pores, 0.0, 1.0)  # hladina pod povrchem mláta: lože se trhá
	var torn: float = clampf(float(sk.get("torn", 0.0)), 0.0, 1.0)  # lože se trhá (kanálky při velkém podtlaku)
	var ebc: float = recipe_ebc * clampf(plato_out / 12.0, 0.15, 1.3) + 3.0 * turb
	return {
		"bed": bed,
		"zakal": clampf(maxf(maxf(turb, cloud), 0.7 * torn) + 0.3 * fines * turb, 0.0, 1.0),  # kal v podsítí kalí jen dokud lože nefiltruje čistě
		"kaly": clampf(0.5 * fines * maxf(turb, 0.1) + cloud, 0.0, 1.0),
		"zrna": clampf(maxf(turb - 0.25, 0.0) * (0.5 + 0.5 * pack) + (0.3 if rake_on and bed else 0.0) * turb + 0.6 * torn, 0.0, 1.0),
		"mlato": 1.0 if spent else clampf(maxf(dry * 2.0, 0.8 * torn), 0.0, 1.0),
		"trha": torn,
		"dp": float(sk.get("dp", 20.0)),
		"pack": pack,
		"ebc": ebc,
		"barva": ebc_color(ebc),
		"plato": plato_out,
		"lpm": sk_draw_lpm() if bed else 0.0,
	}


## orientační barva sladiny/piva podle EBC (3 = světle žlutá … 80 = tmavě hnědá)
static func ebc_color(ebc: float) -> Color:
	var pts := [[3.0, Color(0.96, 0.89, 0.55)], [8.0, Color(0.93, 0.74, 0.25)], [16.0, Color(0.82, 0.55, 0.14)], [32.0, Color(0.62, 0.32, 0.08)], [80.0, Color(0.20, 0.09, 0.04)]]
	if ebc <= float(pts[0][0]):
		return pts[0][1]
	for i in range(1, pts.size()):
		if ebc <= float(pts[i][0]):
			var t: float = (ebc - float(pts[i - 1][0])) / (float(pts[i][0]) - float(pts[i - 1][0]))
			return (pts[i - 1][1] as Color).lerp(pts[i][1], t)
	return pts[pts.size() - 1][1]


## odebere dq litrů ze spodní vrstvy lože a vrací extrakt (kg), který s nimi odešel; vrstvy se shora doplní
func _sk_pull(sk: Dictionary, dq: float) -> float:
	var c: Array = _sk_cells(sk)
	var cap: float = maxf(_sk_pores() / 4.0, 1.0)
	var out_e := 0.0
	var left: float = dq
	var guard := 0
	while left > 0.0001 and guard < 40:
		guard += 1
		var low: Dictionary = c[SK_CELLS - 1]
		var take: float = minf(left, float(low["w"]))
		if take > 0.0:
			var de: float = float(low["e"]) * take / float(low["w"])
			out_e += de
			low["e"] = float(low["e"]) - de
			low["w"] = float(low["w"]) - take
			left -= take
		var moved := false
		for i in range(SK_CELLS - 1, 0, -1):
			var deficit: float = cap - float(c[i]["w"])
			var above: Dictionary = c[i - 1]
			if deficit > 0.0 and float(above["w"]) > 0.0:
				var mv: float = minf(deficit, float(above["w"]))
				var fr: float = mv / float(above["w"])
				var me: float = float(above["e"]) * fr
				above["e"] = float(above["e"]) - me
				above["w"] = float(above["w"]) - mv
				c[i]["e"] = float(c[i]["e"]) + me
				c[i]["w"] = float(c[i]["w"]) + mv
				moved = true
		if take <= 0.0 and not moved:
			break
	return minf(out_e, float(sk["ext"]))


func step(dt: float) -> void:
	clock += dt
	_sk_cells_sync()
	cool_out_t = -1.0  # nastaví ho spílání, když teče
	_tank_inflow = false
	if not active.has("virivka_chladic"):
		_cold_return_s = 0.0
	for rn in active.keys():
		if blocked.get(rn, false) or pump_fault.has(route_pump(str(rn))):
			continue  # zavřená klapka, nebo čerpadlo zavzdušněné či s poruchou: točí se, ale nečerpá
		_flow(str(rn), dt)
	if push_flow and not pump_fault.has(route_pump("virivka_chladic")):
		_push_out(dt)
	_tank_lid()
	_sk_machine(dt)
	_thermostat(dt)
	_settling(dt)
	_sk_compaction(dt)
	_heat(dt)
	_mash_conversion(dt)
	_glycol(dt)
	if float(vessels["spilka"]["v"]) < 1.0:
		o2_spilka = 0.0
	elif yeast_pitched:
		o2_spilka *= exp(-dt / O2_YEAST_TAU)
	var sewer: Dictionary = vessels["kanal"]
	if float(sewer["v"]) > 0.0 or float(sewer["malt"]) > 0.0:
		drain_l += float(sewer["v"])
		drain_ext += float(sewer["ext"])
		if float(sewer["v"]) > 1.0:
			_say_once("kanal_sit", "Voda vytéká z otevřené výpusti do kanálu – zavři klapku.")
		sewer["v"] = 0.0
		sewer["ext"] = 0.0
		sewer["malt"] = 0.0
	elif _said.has("kanal_sit"):
		_said.erase("kanal_sit")
	# FYZIKA+: tepelné ztráty izolovaných nádob podle výpočtu a odpar z horké hladiny mimo var (scripts/fyzika_plus.gd).
	# Dřív: všechny nádoby stejně, časová konstanta 12 h (vs["t"] -= (t - okolí) * dt / 43200).
	evap_idle_l += FyzikaPlus.losses(self, dt)
	FyzikaPlus.pipe_cooling(self, dt)  # FYZIKA+: stojící obsah trubek chladne k teplotě haly


func _free_space(key: String) -> float:
	var vs: Dictionary = vessels[key]
	var used: float = float(vs["v"]) + float(vs["malt"]) * (BED_L_PER_KG if key == "sk" else MALT_L_PER_KG)
	return maxf(float(vs["cap"]) - used, 0.0)


## smíchá dq litrů o teplotě t_in a extraktu ext_in (kg) do nádoby; src = nádoba, ze které extrakt je (kvůli složení)
func _mix(dst: Dictionary, dq: float, t_in: float, ext_in: float, src: Dictionary = {}, malt_in: float = 0.0) -> void:
	var v: float = float(dst["v"])
	# bilance tepla (dekokce): do teploty se počítá kapalina, šrot a ocel nádoby (ocel má teplotu obsahu), ne jen objem
	var cap_dst: float = v * C_WATER + float(dst.get("malt", 0.0)) * C_MALT + float(dst.get("steel", 0.0)) * STEEL_C
	var cap_in: float = dq * C_WATER + malt_in * C_MALT
	if cap_dst + cap_in > 0.0:
		dst["t"] = (cap_dst * float(dst["t"]) + cap_in * t_in) / (cap_dst + cap_in)
	dst["v"] = v + dq
	var e0: float = float(dst["ext"])
	if ext_in > 0.0 and not src.is_empty():
		var a: Dictionary = _xf(dst)
		var b: Dictionary = _xf(src)
		var tot: float = e0 + ext_in
		for k in XF_DEFAULT:
			a[k] = (float(a[k]) * e0 + float(b[k]) * ext_in) / tot
	dst["ext"] = e0 + ext_in


## kapacita a prázdný obsah trubky (volá sim_panel po _init_pipes)
func init_pipe_caps(caps: Dictionary) -> void:
	pipe_caps = caps
	for pid in pipe_caps:
		if not pipe_hold.has(pid):
			pipe_hold[pid] = {"v": 0.0, "t": cal_mains_t, "ext": 0.0, "malt": 0.0, "med": "empty"}


func _pipe_slot(pid: String) -> Dictionary:
	if not pipe_hold.has(pid):
		pipe_hold[pid] = {"v": 0.0, "t": cal_mains_t, "ext": 0.0, "malt": 0.0, "med": "empty"}
	return pipe_hold[pid]


func pipe_ext_total() -> float:
	var s := 0.0
	for pid in pipe_hold:
		s += float(pipe_hold[pid].get("ext", 0.0))
	return s


## smíchá slug do trubky (jako Sim.add_to na hraně)
func _mix_pipe(e: Dictionary, slug: Dictionary) -> void:
	var dv: float = float(slug.get("v", 0.0))
	var dm: float = float(slug.get("malt", 0.0))
	var de: float = float(slug.get("ext", 0.0))
	if dv <= 1e-9 and dm <= 1e-9 and de <= 1e-9:
		return
	var v0: float = float(e["v"])
	var m0: float = float(e["malt"])
	if v0 + dv > 1e-9:
		e["t"] = (v0 * float(e["t"]) + dv * float(slug.get("t", e["t"]))) / (v0 + dv)
	e["v"] = v0 + dv
	e["malt"] = m0 + dm
	e["ext"] = float(e["ext"]) + de
	var med := str(slug.get("med", "water"))
	if dm > 0.05 or med == "mash":
		e["med"] = "mash"
	elif de > 0.01 or med == "wort" or float(e["ext"]) > 0.05:
		e["med"] = "wort" if str(e["med"]) != "mash" else "mash"
	elif med != "empty":
		e["med"] = "water" if str(e["med"]) == "empty" else str(e["med"])


## odebere z trubky objem over (L včetně šrotu) jako slug – přepad dál po cestě
func _take_pipe(e: Dictionary, over: float) -> Dictionary:
	var total: float = float(e["v"]) + float(e["malt"]) * MALT_L_PER_KG
	if total <= 1e-6 or over <= 0.0:
		return {"v": 0.0, "t": float(e["t"]), "ext": 0.0, "malt": 0.0, "med": "empty"}
	var frac: float = clampf(over / total, 0.0, 1.0)
	var out := {
		"v": float(e["v"]) * frac,
		"t": float(e["t"]),
		"ext": float(e["ext"]) * frac,
		"malt": float(e["malt"]) * frac,
		"med": str(e["med"]),
	}
	e["v"] = float(e["v"]) * (1.0 - frac)
	e["ext"] = float(e["ext"]) * (1.0 - frac)
	e["malt"] = float(e["malt"]) * (1.0 - frac)
	if float(e["v"]) < 0.05 and float(e["malt"]) < 0.05:
		e["v"] = 0.0
		e["ext"] = 0.0
		e["malt"] = 0.0
		e["med"] = "empty"
	return out


## tlačí slug trubkami po směru; vrací přepad z poslední (ten jde do cílové nádoby)
func _push_through(pipes: Array, slug: Dictionary) -> Dictionary:
	var part: Dictionary = slug.duplicate()
	for pid in pipes:
		var e: Dictionary = _pipe_slot(str(pid))
		var cap: float = float(pipe_caps.get(str(pid), 4.0))
		_mix_pipe(e, part)
		var used: float = float(e["v"]) + float(e["malt"]) * MALT_L_PER_KG
		var over: float = used - cap
		if over > 0.05:
			part = _take_pipe(e, over)
		else:
			part = {"v": 0.0, "t": float(e["t"]), "ext": 0.0, "malt": 0.0, "med": "empty"}
	return part


## doručí slug do nádoby (s limitem místa); zbytek vrátí (nechá se v poslední trubce přes reverse push)
func _deliver_slug(dst_key: String, slug: Dictionary, src_for_xf: Dictionary = {}) -> Dictionary:
	var room: float = _free_space(dst_key)
	var need: float = float(slug.get("v", 0.0)) + float(slug.get("malt", 0.0)) * MALT_L_PER_KG
	if need <= 1e-9:
		return {"v": 0.0, "t": 15.0, "ext": 0.0, "malt": 0.0, "med": "empty"}
	var frac: float = 1.0 if need <= room else (room / need if room > 0.0 else 0.0)
	var liq: float = float(slug["v"]) * frac
	var malt: float = float(slug["malt"]) * frac
	var ext: float = float(slug["ext"]) * frac
	if liq > 1e-9 or malt > 1e-9:
		var dst: Dictionary = vessels[dst_key]
		if malt > 0.0 and not src_for_xf.is_empty():
			_mix_viability(dst, malt, src_for_xf)
		_mix(dst, liq, float(slug["t"]), ext, src_for_xf, malt)
		dst["malt"] = float(dst["malt"]) + malt
	return {
		"v": float(slug["v"]) * (1.0 - frac),
		"t": float(slug["t"]),
		"ext": float(slug["ext"]) * (1.0 - frac),
		"malt": float(slug["malt"]) * (1.0 - frac),
		"med": str(slug.get("med", "water")),
	}


func _flow(rn: String, dt: float) -> void:
	var r: Dictionary = routes.get(rn, {})
	if r.is_empty():
		return
	if rn == "glykol" or rn == "ledovka":
		return
	var src_key: String = str(r["from"])
	var dst_key: String = str(r["to"])
	if not vessels.has(dst_key) or (src_key != "mains" and not vessels.has(src_key)):
		return  # trasa bez platné nádoby (rozpracovaná v editoru cest)
	if src_key == dst_key:
		return  # oběh do téže nádoby (např. vorlauf SK → SK): objem se nemění, zákal řeší _settling
	var q_lpm: float = float(r["lpm"]) * FLOW_SCALE
	var sk_rmut: bool = src_key == "sk" and not bool(vessels["sk"].get("bed", false))  # SK jako nádoba na rmut: čerpadlo bere rmut i se šrotem
	if src_key == "sk":
		if sk_rmut:
			# rmut ze SK: čerpadlo přečerpá právě to, co nastaví měnič (cesta má lpm = nastavení / FLOW_SCALE),
			# ale víc než plný výkon C1 (80 v měřítku trasy) ne
			q_lpm = minf(q_lpm, 80.0 * FLOW_SCALE)
		else:
			q_lpm = sk_draw_lpm()  # sladina: co pustí lože, případně méně podle měniče C2
	var dq: float = q_lpm * float(flow_scale.get(rn, 1.0)) * dt / 60.0
	if dq <= 0.0:
		return
	if rn == "virivka_chladic":
		if not push_flow:  # při protláčce bere C3 vodu z řadu (_push_out), ne mladinu z vířivky
			_pitch(dq, dt)  # spílání: chladič, voda v potrubí, hadice na vratce nebo na tanku
		return
	var dst: Dictionary = vessels[dst_key]
	dq = minf(dq, _free_space(dst_key) + _route_pipe_free(rn))  # místo v cíli + volné úseky na cestě
	if dq <= 0.0:
		return
	var pipes: Array = route_pipes.get(rn, []) if pipe_delay else []
	# --- okamžitý režim (bez zpoždění) nebo cesta bez trubek ---
	if pipes.is_empty():
		_flow_instant(rn, src_key, dst_key, dq, sk_rmut)
		return
	# --- zpoždění: odebrat ze zdroje → tlačit trubkami → přepad do cíle ---
	var slug: Dictionary = _take_source(src_key, dq, sk_rmut)
	if float(slug.get("v", 0.0)) <= 1e-9 and float(slug.get("malt", 0.0)) <= 1e-9:
		return
	if dst_key == "sk" and float(slug.get("malt", 0.0)) > 0.0 and float(dst["malt"]) < 0.5 and float(dst["v"]) < SIEVE_FLOOD_L:
		dst["pack"] = maxf(float(dst.get("pack", 0.0)), 0.3)
		events.append("Síta nebyla pod vodou – rmut dosedá na suchá síta, lože bude utažené a scezování půjde pomaleji.")
	if rn == "varna_virivka" and float(slug.get("v", 0.0)) > 0.0:
		var src_tmp: Dictionary = vessels[src_key] if vessels.has(src_key) else {}
		dst["trub"] = minf(float(dst.get("trub", 0.0)) + float(slug["v"]) * 0.004 * clampf(float(src_tmp.get("boil_s", 0.0)) / 1800.0 + 0.3, 0.0, 1.0), 1.0)
	var overflow: Dictionary = _push_through(pipes, slug)
	var left: Dictionary = _deliver_slug(dst_key, overflow, vessels[src_key] if vessels.has(src_key) else {})
	# co se do cíle nevešlo, vrať do poslední trubky
	if float(left.get("v", 0.0)) > 0.05 or float(left.get("malt", 0.0)) > 0.05:
		_mix_pipe(_pipe_slot(str(pipes[pipes.size() - 1])), left)


## volný objem na trubkách cesty (L)
func _route_pipe_free(rn: String) -> float:
	var s := 0.0
	for pid in route_pipes.get(rn, []):
		var e: Dictionary = _pipe_slot(str(pid))
		var cap: float = float(pipe_caps.get(str(pid), 4.0))
		s += maxf(cap - float(e["v"]) - float(e["malt"]) * MALT_L_PER_KG, 0.0)
	return s


## odebere ze zdroje slug {v,t,ext,malt,med} (nebo vodu z řadu)
func _take_source(src_key: String, dq: float, sk_rmut: bool) -> Dictionary:
	if src_key == "mains":
		return {"v": dq, "t": cal_mains_t, "ext": 0.0, "malt": 0.0, "med": "water"}
	var src: Dictionary = vessels[src_key]
	if src_key == "rvp" or sk_rmut:
		var total: float = float(src["v"]) + float(src["malt"]) * MALT_L_PER_KG
		if total <= 0.5:
			return {"v": 0.0, "t": float(src["t"]), "ext": 0.0, "malt": 0.0, "med": "empty"}
		var frac: float = minf(dq / total, 1.0)
		var liq: float = float(src["v"]) * frac
		var malt: float = float(src["malt"]) * frac * (1.0 - 0.7 * float(src.get("settled", 0.0)))
		var ext: float = float(src["ext"]) * frac
		src["v"] = float(src["v"]) - liq
		src["malt"] = float(src["malt"]) - malt
		src["ext"] = float(src["ext"]) - ext
		if src_key == "sk" and float(src["malt"]) < 1.0:
			_flush_fines(src, liq)
		return {"v": liq, "t": float(src["t"]), "ext": ext, "malt": malt, "med": "mash" if malt > 0.05 else ("wort" if ext > 0.05 else "water")}
	dq = minf(dq, float(src["v"]))
	if dq <= 0.0:
		return {"v": 0.0, "t": float(src["t"]), "ext": 0.0, "malt": 0.0, "med": "empty"}
	var ext_out: float = float(src["ext"]) * dq / maxf(float(src["v"]), 0.001)
	if src_key == "sk" and bool(src.get("bed", false)):
		ext_out = _sk_pull(src, dq)
	src["v"] = float(src["v"]) - dq
	src["ext"] = float(src["ext"]) - ext_out
	return {"v": dq, "t": float(src["t"]), "ext": ext_out, "malt": 0.0, "med": "wort" if ext_out > 0.05 else "water"}


## starý okamžitý přesun (když zpoždění vypnuté nebo bez trubek)
func _flow_instant(rn: String, src_key: String, dst_key: String, dq: float, sk_rmut: bool) -> void:
	var dst: Dictionary = vessels[dst_key]
	dq = minf(dq, _free_space(dst_key))
	if dq <= 0.0:
		return
	if src_key == "mains":
		_mix(dst, dq, cal_mains_t, 0.0)
		return
	var src: Dictionary = vessels[src_key]
	if src_key == "rvp" or sk_rmut:
		var total: float = float(src["v"]) + float(src["malt"]) * MALT_L_PER_KG
		if total <= 0.5:
			return
		var frac: float = minf(dq / total, 1.0)
		var liq: float = float(src["v"]) * frac
		var malt: float = float(src["malt"]) * frac * (1.0 - 0.7 * float(src.get("settled", 0.0)))
		var ext: float = float(src["ext"]) * frac
		src["v"] = float(src["v"]) - liq
		src["malt"] = float(src["malt"]) - malt
		src["ext"] = float(src["ext"]) - ext
		if src_key == "sk" and float(src["malt"]) < 1.0:
			_flush_fines(src, liq)
		if dst_key == "sk" and malt > 0.0 and float(dst["malt"]) < 0.5 and float(dst["v"]) < SIEVE_FLOOD_L:
			dst["pack"] = maxf(float(dst.get("pack", 0.0)), 0.3)
			events.append("Síta nebyla pod vodou – rmut dosedá na suchá síta, lože bude utažené a scezování půjde pomaleji.")
		_mix_viability(dst, malt, src)
		_mix(dst, liq, float(src["t"]), ext, src, malt)
		dst["malt"] = float(dst["malt"]) + malt
		if rn == "varna_virivka":
			dst["trub"] = minf(float(dst.get("trub", 0.0)) + liq * 0.004 * clampf(float(src.get("boil_s", 0.0)) / 1800.0 + 0.3, 0.0, 1.0), 1.0)
		return
	dq = minf(dq, float(src["v"]))
	if dq <= 0.0:
		return
	var ext_out: float = float(src["ext"]) * dq / maxf(float(src["v"]), 0.001)
	if src_key == "sk" and bool(src.get("bed", false)):
		ext_out = _sk_pull(src, dq)
	src["v"] = float(src["v"]) - dq
	src["ext"] = float(src["ext"]) - ext_out
	_mix(dst, dq, float(src["t"]), ext_out, src)


## Deskový chladič (ε-NTU, protiproud), jeden stupeň: mladinu chladí jen ledová voda z C5 (UA_ICE, průtok ICE_LPM).
## FYZIKA+ (Vasil 17. 11. 2026): na Zajíci voda z řadu mladinu přímo nechladí – předchladí se cestou do ledovky, tam se
## okruhem přes glykol dochladí na 2 °C a teprve ledová voda jde do chladiče. Dřívější 1. stupeň „voda z řadu“ je pryč;
## bez ledové vody se mladina nechladí. Zanesený chladič (plate_eff) zmenší UA.
func plate_cool(t_in: float, q_lpm: float, ice: bool, t_ice: float) -> Dictionary:
	var out := {"t_out": t_in, "t_ice_out": t_ice, "duty_kw": 0.0}
	var cw: float = q_lpm / 60.0 * 1.04 * 4.0  # kW/K tepelná kapacita proudu mladiny
	if cw <= 0.0 or not ice:
		return out
	var foul: float = clampf(plate_eff / PLATE_EFF, 0.05, 1.0)
	var cc: float = ICE_LPM / 60.0 * 4.19
	var cmin: float = minf(cw, cc)
	var cr: float = cmin / maxf(cw, cc)
	var ntu: float = cal_ua_ice * foul / cmin
	var eff: float = ntu / (1.0 + ntu) if cr > 0.999 else (1.0 - exp(-ntu * (1.0 - cr))) / (1.0 - cr * exp(-ntu * (1.0 - cr)))
	var t2: float = minf(t_in - eff * cmin / cw * (t_in - t_ice), t_in)
	out["t_out"] = t2
	out["duty_kw"] = cw * (t_in - t2)
	# zpětné získání tepla: všechno teplo z mladiny odnáší ledová voda do bojleru (knihy: 80–88 °C), strop 95 °C
	out["t_ice_out"] = minf(t_ice + float(out["duty_kw"]) / cc, 95.0)
	return out


## Spílání (C3): mladina z vířivky přes chmelový filtr a deskový chladič do spílací hadice. Hadice na vratce vrací
## mladinu zpět do vířivky (vymyje se filtr a chladič, než za chladičem teče ledová), na tanku plní spilku.
## Z hadice vyteče nejdřív voda, která po vyplachování stojí ve filtru, chladiči a potrubí.
func _pitch(dq_max: float, dt: float) -> void:
	var wp: Dictionary = vessels["wp"]
	var dq: float = minf(dq_max, float(wp["v"]))
	if hose_tank:
		dq = minf(dq, _free_space("spilka"))
	if dq <= 0.0:
		return
	var t_in: float = float(wp["t"])
	# deskový chladič (ε-NTU, protiproud): chladí jen ledová voda z C5
	var lv: Dictionary = vessels["lv"]
	var ice: bool = active.get("ledovka", false) and float(lv["v"]) >= 1.0  # ledovka došla nebo C5 stojí – mladina se nechladí
	var plate: Dictionary = plate_cool(t_in, dq * 60.0 / maxf(dt, 0.001), ice, float(lv["t"]))
	var t_out: float = float(plate["t_out"])
	if ice:
		# ledová voda projde chladičem jednou a ohřátá (zpětné získání tepla) odtéká do bojleru
		var used: float = minf(ICE_LPM * dt / 60.0, float(lv["v"]))
		lv["v"] = float(lv["v"]) - used
		_mix(vessels["hlt"], minf(used, _free_space("hlt")), float(plate["t_ice_out"]), 0.0)
	cool_out_t = t_out
	_line_t = t_out
	var ext_out: float = float(wp["ext"]) * dq / maxf(float(wp["v"]), 0.001)
	# usazený kal v kuželu na dně váže část extraktu, která se čerpáním nedostane dál
	var cone: float = ext_out * CONE_LOSS * float(wp.get("tset", 0.0)) if hose_tank else 0.0
	wp["ext"] = float(wp["ext"]) - cone
	kal_ext += cone
	var cloudy: float = float(wp.get("trub", 0.0)) * (1.0 - float(wp.get("tset", 0.0)))
	wp["v"] = float(wp["v"]) - dq
	wp["ext"] = float(wp["ext"]) - ext_out
	# z hadice vyteče nejdřív voda stojící v potrubí, mladina zůstane v potrubí místo ní
	var water: float = minf(dq, line_water)
	line_water -= water
	var stay_ext: float = ext_out * water / dq
	line_ext += stay_ext
	if hose_tank:
		var sp: Dictionary = vessels["spilka"]
		_o2_in(sp, dq, t_out)
		_mix(sp, water, t_out, 0.0)
		_mix(sp, dq - water, t_out, ext_out - stay_ext, wp)
		_tank_inflow = true
		# čerpadlo odebírá z boku: kal ze zvířené části jde s mladinou do spilky
		sp["trub"] = minf(float(sp.get("trub", 0.0)) + dq * 0.02 * cloudy, 1.0)
		if cloudy > 0.35 and not _trub_warned:
			_trub_warned = true
			events.append("Mladina z vířivky je kalná – nechte vířivku usadit")
		elif cloudy < 0.1:
			_trub_warned = false
		water_to_tank += water
		if water_to_tank > 5.0:
			_say_once("voda_tank", "Do spilky natekla voda z chladiče a potrubí (%.0f L) – na začátku má být hadice na vratce, dokud z ní nejde mladina." % water_to_tank)
		if t_out > PITCH_T + 2.0 and dq > water:
			_say_once("tepla_tank", "Do spilky teče teplá mladina (%.1f °C) – na tank až ve chvíli, kdy je za chladičem ledová." % t_out)
		elif t_out <= PITCH_T + 1.0:
			_said.erase("tepla_tank")
		_cold_return_s = 0.0
	else:
		# vratka: mladina (a na začátku voda z potrubí) se vrací do vířivky
		_mix(wp, water, t_out, 0.0)
		_mix(wp, dq - water, t_out, ext_out - stay_ext, wp)
		if line_water <= 0.0:
			_say_once("vymyto", "Filtr a chladič jsou vymyté – z hadice na vratce teče mladina (%.1f °C)." % t_out)
			if t_out <= PITCH_T + 1.0:
				_cold_return_s += dt
				if _cold_return_s > 60.0:
					_say_once("ledova_vratka", "Za chladičem už teče ledová mladina (%.1f °C) – přepni hadici na tank." % t_out)
			else:
				_cold_return_s = 0.0


## O₂ ve spilce: vtékající mladina nabere O₂ podle teploty (studená víc), smíchá se s tím, co už ve spilce je
func _o2_in(sp: Dictionary, dq: float, t_in: float) -> void:
	var sat: float = O2_AIR_SAT * clampf(1.0 - t_in / 100.0, 0.2, 1.0) * clampf(1.0 - t_in / 160.0, 0.3, 1.0)
	var v0: float = float(sp["v"])
	o2_spilka = (o2_spilka * v0 + sat * O2_PICKUP * dq) / maxf(v0 + dq, 0.001)


## rozpuštěný kyslík v nádobě (ppb); zatím jen spilka, ostatní nádoby 0
func o2_ppb(key: String) -> float:
	return o2_spilka if key == "spilka" else 0.0


## výtěžnost: extrakt ve spilce a potrubí podle teoretického z nasypaného sladu (%)
func yield_pct() -> float:
	if malt_total <= 0.0:
		return 0.0
	var ext: float = float(vessels["spilka"]["ext"]) + line_ext + pipe_ext_total()
	return 100.0 * ext / (malt_total * cal_extract_yield)


## kg extraktu po složkách: kde skončil extrakt ze sladu. "teorie" je plná výtěžnost, "nezjisteno" rozdíl (ztráty,
## které model zatím nepočítá zvlášť: hot break, hrudky)
func losses() -> Dictionary:
	var theory: float = malt_total * cal_extract_yield
	var pipe_e: float = pipe_ext_total()
	var out := {
		"teorie": theory,
		"spilka": float(vessels["spilka"]["ext"]),
		"potrubi": line_ext + pipe_e,
		"kanal": drain_ext,
		"mlato": spent_ext,
		"hrudky": (dead_malt + float(vessels["rvp"].get("clump", 0.0))) * cal_extract_yield,
		"hot_break": hot_break_ext,
		"kal_virivka": kal_ext,
	}
	var in_vessels: float = 0.0
	for k in ["hlt", "rvp", "sk", "wp"]:
		in_vessels += float(vessels[k]["ext"])
	out["v_nadobach"] = in_vessels
	out["nezjisteno"] = theory - float(out["spilka"]) - float(out["potrubi"]) - drain_ext - spent_ext - in_vessels - float(out["hrudky"]) - hot_break_ext - kal_ext
	return out


## Protláčka: voda z řadu jde sáním C3 přes chmelový filtr, chladič a spílací hadici a vytlačí mladinu, která v nich
## po vyprázdnění vířivky zůstala (na tanku do spilky, na vratce zpět do vířivky). Pak už z hadice teče jen voda.
func _push_out(dt: float) -> void:
	var q: float = float(routes.get("virivka_chladic", {}).get("lpm", 40.0)) * FLOW_SCALE * dt / 60.0
	if hose_tank:
		q = minf(q, _free_space("spilka"))
	if q <= 0.0:
		return
	var wp: Dictionary = vessels["wp"]
	if float(wp["v"]) > 50.0:
		_say_once("protlacka_virivka", "Vířivka ještě není prázdná (%.0f L) – protláčku pusť, až C3 vyprázdní vířivku." % float(wp["v"]))
	var wort_in_line: float = LINE_L - line_water
	var w_out: float = minf(q, wort_in_line)  # nejdřív vyteče mladina, potrubí se plní vodou
	var ext_w: float = line_ext * w_out / maxf(wort_in_line, 0.001)
	line_ext -= ext_w
	line_water += w_out
	var water_out: float = q - w_out
	var lv: Dictionary = vessels["lv"]
	var coolant: float = float(lv["t"]) if active.get("ledovka", false) and float(lv["v"]) >= 1.0 else cal_mains_t
	cool_out_t = maxf(coolant + 3.0, cal_mains_t - plate_eff * (cal_mains_t - coolant)) if water_out > 0.0 else _line_t
	var dst: Dictionary = vessels["spilka"] if hose_tank else wp
	_mix(dst, w_out, _line_t, ext_w, vessels["wp"])  # mladina z potrubí má složení mladiny z vířivky
	_mix(dst, water_out, cool_out_t, 0.0)
	if not hose_tank:
		if w_out > 0.0:
			_say_once("protlacka_vratka", "Protláčka jde na vratku – mladina z potrubí se vrací do vířivky, ne do tanku.")
		return
	_tank_inflow = true
	pushed_wort += w_out
	push_water_tank += water_out
	if line_water >= LINE_L - 0.01 and pushed_wort > 1.0:
		_say_once("protlacka", "Protláčka: z potrubí dotlačeno %.0f L mladiny – z hadice už teče voda, zavři vodu a zastav C3." % pushed_wort)
	if push_water_tank > 20.0:
		_say_once("protlacka_voda", "Protláčka běží dál – do spilky teče voda (%.0f L) a ředí mladinu." % push_water_tank)


## víko spilky: po zavíčkování hlášení se stavem spilky; zavírá se až po protláčce, když už do spilky nic neteče
func _tank_lid() -> void:
	if tank_lid and not _lid_was:
		var sp: Dictionary = vessels["spilka"]
		if float(sp["v"]) > 10.0:
			events.append("Spilka zavíčkovaná: %.0f L, %.1f °C, %.1f °P." % [float(sp["v"]), float(sp["t"]), plato("spilka")])
	_lid_was = tank_lid
	if tank_lid and _tank_inflow:
		_say_once("vicko_plneni", "Víko spilky je zavřené a do spilky teče – víko se zavírá až po protláčce.")
	elif not tank_lid:
		_said.erase("vicko_plneni")


## ---------- sedimentace a jemné kalové částice ----------
## rvp: šrot v rmutu se bez míchání usazuje ke dnu (Stokesův zákon ~ minuty), míchadlem se zvíří zpět.
## sk: jemné částice (kal) se usazují v podsítí a ucpávají síto a potrubí; jdou utrhnout jen rázem klapky při podrážení.
var stir: Dictionary = {"rvp": false}  # míchadlo varny zapnuto
var _trub_warned: bool = false


func _settling(dt: float) -> void:
	var rv: Dictionary = vessels["rvp"]
	if not rv.has("settled"):
		rv["settled"] = 0.0
	if float(rv["malt"]) > 0.5 and float(rv["v"]) > 1.0:
		if bool(stir.get("rvp", false)):
			rv["settled"] = maxf(float(rv["settled"]) - dt / 25.0, 0.0)  # zvíření
		else:
			rv["settled"] = float(rv["settled"]) + (1.0 - float(rv["settled"])) * dt / 260.0
	elif float(rv["malt"]) <= 0.5:
		rv["settled"] = 0.0
	var sk: Dictionary = vessels["sk"]
	if not sk.has("fines"):
		sk["fines"] = 0.0
		sk["cloud"] = 0.0
	if float(sk["malt"]) > 0.5 and float(sk["v"]) > 1.0:
		var carry: float = 0.0006 * dt * (sk_draw_lpm() / (25.0 * VOL_SCALE)) if sk_draw_active() else 0.0  # proud nese jemné částice k sítu
		sk["fines"] = minf(float(sk["fines"]) + carry + dt / 3000.0, 1.0)  # a v klidu se usazují
	# bez mláta kal v podsítí zůstává, dokud ho nevypláchne voda tekoucí ven (výpusť, odběr z podsítí)
	sk["cloud"] = float(sk["cloud"]) * (1.0 - minf(dt / 240.0, 1.0))
	# odrmutování: celé zcukřené dílo je v SK a nic neteče → mláto se během 20 min usadí do filtračního lože
	if not bool(sk.get("bed", false)) and float(sk["malt"]) >= 1.0 and float(sk["v"]) >= 1.0:
		var whole: bool = float(vessels["rvp"]["malt"]) < 0.05 * float(sk["malt"])
		if not sk_quiet():
			sk["settle"] = 0.0  # proud lože zvíří, usazování začíná znovu
		elif whole and iodine_negative():
			sk["settle"] = float(sk.get("settle", 0.0)) + dt / 1200.0
			if float(sk["settle"]) >= 1.0:
				sk["bed"] = true
				sk["turb"] = 1.0
				sk["rev"] = 0.0  # otáčky kopačky se počítají od začátku scezování
				events.append("Mláto se v klidu usadilo: v SK vzniklo filtrační lože (%.0f cm)" % (bed_height(sk) * 100.0))
	# zákal prvního průtoku lože: čistí se při průtoku, bez nátoku se lože v klidu usazuje
	if bool(sk.get("bed", false)):
		var pores: float = float(sk["malt"]) * BED_L_PER_KG * 0.45
		if sk_draw_active() and darcy_lpm() > 1.0:
			# vorlauf: sladina se vyčeří za ~8 min odběru, při silném tahu pomaleji (poměr tahu čerpadla k propustnosti lože)
			var tah: float = (sk_demand_lpm * _sk_throttle() / darcy_lpm()) if sk_demand_lpm > 0.0 and darcy_lpm() > 1.0 else 0.6 * _sk_throttle()
			sk["turb"] = maxf(float(sk.get("turb", 1.0)) - dt / 480.0 * clampf(1.4 - tah, 0.3, 1.0), 0.0)
			# odběr lože stlačuje – bez kypření kopačkou průtok postupně klesá
			sk["pack"] = minf(float(sk.get("pack", 0.0)) + dt / 3600.0, 1.0)
			if float(sk["v"]) < pores and plato("sk") > 2.0:
				# hladina pod povrchem mláta: lože nasává vzduch a slehává se
				sk["pack"] = minf(float(sk["pack"]) + dt / 300.0, 1.0)
				_say_once("sucho", "Hladina klesla pod povrch mláta – lože nasává vzduch a slehává se. Pusť sprchu (vyslazování).")
		if float(sk["v"]) > pores + 100.0:
			_said.erase("sucho")
		if float(sk["malt"]) < 1.0 or float(sk["v"]) < 1.0:
			sk["bed"] = false
	# vířivka: přítok kapaliny tangenciálně kal zvíří a stáhne do středu dna, v klidu se za ~15 min usadí
	var wp: Dictionary = vessels["wp"]
	if not wp.has("tset"):
		wp["tset"] = 0.0
		wp["trub"] = 0.0
	if active.get("varna_virivka", false):
		wp["tset"] = maxf(float(wp["tset"]) - dt / 60.0, 0.0)
	elif float(wp["v"]) > 1.0:
		wp["tset"] = float(wp["tset"]) + (1.0 - float(wp["tset"])) * minf(dt / 300.0, 1.0)
	elif float(wp["v"]) <= 1.0:
		wp["trub"] = 0.0
		wp["tset"] = 0.0


## SK je v klidu: nic do ní neteče ani se z ní neodebírá (podmínka pro usazení lože)
func sk_quiet() -> bool:
	for rn in active:
		var r: Dictionary = routes.get(rn, {})
		if str(r.get("from", "")) == "sk" or str(r.get("to", "")) == "sk":
			return false
	if rake_on and float(vessels["sk"]["malt"]) > 1.0:
		return false  # kopačka v chodu míchá dílo, mláto se neusadí
	return true


## z SK se právě odebírá (scezování do pánve nebo vorlauf zpět do SK)
func sk_draw_active() -> bool:
	for rn in active:
		if str(routes.get(rn, {}).get("from", "")) == "sk":
			return true
	return false


## ráz klapky při podrážení (rychlé zavření a otevření): vodní ráz utrhne část kalu ode dna a rozvíří ho
func underlet_kick() -> void:
	var sk: Dictionary = vessels["sk"]
	if not sk.has("fines"):
		return
	var released: float = float(sk["fines"]) * 0.5
	if released < 0.01:
		return
	sk["fines"] = float(sk["fines"]) - released
	sk["cloud"] = minf(float(sk["cloud"]) + released * 0.8, 1.0)
	events.append("Ráz klapky: kal se utrhl ode dna (v podsítí zbývá %.0f %%)" % (float(sk["fines"]) * 100.0))


## Termostat: ohřev spíná podle teploty a cíle (proporcionální s pásmem 8 K, při cíli ≥ 99 °C naplno až do varu)
func _thermostat(dt: float) -> void:
	if prog_run:
		auto_mode["rvp"] = true
		var step: Dictionary = prog[prog_i]
		setpoint["rvp"] = float(step["t"])
		var rv: Dictionary = vessels["rvp"]
		if float(rv["v"]) > 1.0 and float(rv["t"]) >= float(step["t"]) - 0.3:
			prog_hold_s += dt
			if prog_hold_s >= float(step["min"]) * 60.0:
				prog_hold_s = 0.0
				prog_i += 1
				if prog_i >= prog.size():
					prog_run = false
					prog_i = 0
					events.append("Rmutovací program dokončen")
				else:
					events.append("Rmutování: další prodleva %.0f °C / %.0f min" % [float(prog[prog_i]["t"]), float(prog[prog_i]["min"])])
	for key in ["hlt", "rvp"]:
		if not bool(auto_mode.get(key, false)):
			continue
		var t: float = float(vessels[key]["t"])
		var sp: float = float(setpoint[key])
		var kw: float = 0.0
		if sp >= 99.0:
			kw = float(max_kw[key])
		else:
			kw = clampf((sp - t) / 8.0, 0.0, 1.0) * float(max_kw[key])
		heater_kw[key] = kw


## bod varu ~99,6 °C (350 m n. m.), tepelná ztráta pláště a víka 0,012 kW/K
const BOIL_T := 99.6
const LOSS_KW_PER_K := 0.08  # ztráta velké nádoby (plášť a víko), kW na 1 K nad okolím
const BOIL_FULL_KW := 330.0  # nadbytek výkonu nad ztrátami, při kterém je var "naplno" (~10 % objemu za hodinu u 200 l)


## chmel do varny: g = hmotnost, alpha = obsah alfa-kyselin v %; čas varu se počítá od okamžiku přidání
func add_hops(g: float, alpha: float) -> void:
	var rvp: Dictionary = vessels["rvp"]
	if not rvp.has("hops"):
		rvp["hops"] = []
	(rvp["hops"] as Array).append({"g": g, "a": alpha, "t0": float(rvp.get("boil_s", 0.0))})


## měrná hmotnost z °Plato
func sg_from_plato(p: float) -> float:
	return 1.0 + p / (258.6 - p * 227.1 / 258.2)


## hořkost (Tinseth): využití chmele klesá se zahuštěním (bigness) a roste s dobou varu
func ibu() -> float:
	var rvp: Dictionary = vessels["rvp"]
	var v: float = float(rvp["v"])
	if v < 5.0 or not rvp.has("hops"):
		return 0.0
	var sg: float = sg_from_plato(plato("rvp"))
	var big: float = 1.65 * pow(0.000125, sg - 1.0)
	var total: float = 0.0
	for h in rvp["hops"]:
		var tb: float = maxf(float(rvp.get("boil_s", 0.0)) - float(h["t0"]), 0.0) / 60.0
		var util: float = big * (1.0 - exp(-0.04 * tb)) / 4.15
		total += util * (float(h["a"]) / 100.0 * float(h["g"]) * 1000.0 / v)
	return total


## DMS (ppb): začíná na 100 a při varu klesá s poločasem 20 min
func dms_ppb() -> float:
	return 100.0 * pow(0.5, float(vessels["rvp"].get("boil_s", 0.0)) / 1200.0)


## teplota syté páry (°C) při přetlaku bar (Antoineova rovnice vody)
func steam_t(bar: float) -> float:
	return 1810.94 / (8.14019 - log((bar + 1.013) * 750.06) / log(10.0)) - 244.485


## nižší tlak páry = menší rozdíl teplot pára – nádoba = menší přestup tepla (1,0 při provozním tlaku)
func _steam_factor(t: float) -> float:
	if steam_p >= STEAM_BAR - 0.001:
		return 1.0
	return clampf((steam_t(steam_p) - t) / maxf(steam_t(STEAM_BAR) - t, 1.0), 0.0, 1.0)


## tlak páry v kotli podle zásoby energie ve vodě kotle (BOILER_WATER_KG, 57 kJ/kg·bar): hořák dodává BOILER_KW, odběr
## ohřevů víc než to tlak sráží (a tím i přestup tepla), po ubrání zase roste až na steam_bar (poruchou snížený strop).
## Kotel je při startu teplý; po vypnutí tlak pomalu klesá ztrátami.
func _steam_pressure(used_kw: float, dt: float) -> void:
	var cap_kj: float = cal_boiler_water_kg * 57.0  # kJ na 1 bar
	if steam_on:
		var p0: float = steam_p
		steam_p = clampf(p0 + (cal_boiler_kw - used_kw) * dt / cap_kj, 0.0, STEAM_BAR)
		var cap_p: float = minf(steam_bar, STEAM_BAR)  # strop snížený poruchou hořáku: tlak klesá postupně (0,1 bar/min)
		if steam_p > cap_p:
			steam_p = maxf(cap_p, minf(p0, steam_p) - 0.1 * dt / 60.0)
	else:
		steam_p = maxf(steam_p - (used_kw + BOILER_LOSS_KW) * dt / cap_kj, 0.0)


func _heat(dt: float) -> void:
	# parní kotel: bez páry ohřev nejde; chtějí-li ohřevy víc, než kotel dá, dostanou poměrně méně
	var demand: float = float(heater_kw.get("hlt", 0.0)) + float(heater_kw.get("rvp", 0.0))
	# ohřevy berou páru, kolik chtějí; když kotel nestačí, klesá tlak (_steam_pressure) a s ním výkon všech ohřevů
	var supply: float = 1.0 if (steam_on and steam_p > 0.05) else 0.0
	var used_kw := 0.0
	boiler_load = minf(demand, cal_boiler_kw) / cal_boiler_kw if steam_on else 0.0
	for key in ["hlt", "rvp"]:
		var vs: Dictionary = vessels[key]
		if not vs.has("boil"):
			vs["boil"] = 0.0
			vs["foam"] = 0.0
			vs["evap_lph"] = 0.0
			vs["boil_s"] = 0.0
		var kw: float = float(heater_kw.get(key, 0.0)) * supply * _steam_factor(float(vs["t"]))
		if float(vs["v"]) < 1.0:
			vs["boil"] = 0.0
			vs["foam"] = 0.0
			vs["evap_lph"] = 0.0
			continue
		used_kw += kw
		var t: float = float(vs["t"])
		var cap: float = float(vs["v"]) * C_WATER + float(vs["malt"]) * C_MALT + float(vs.get("steel", 0.0)) * STEEL_C
		# čistý příkon = ohřev - ztráty do haly (bez ohřevu jen ztráty řeší _step pomalé chladnutí)
		var net_kw: float = kw  # FYZIKA+: ztráty do haly počítá FyzikaPlus.losses pro všechny nádoby stejně (dřív se tu při ohřevu odečítaly podruhé: LOSS_KW_PER_K)
		var heat: float = net_kw * dt
		var evap_l: float = 0.0
		if heat > 0.0 and t + heat / cap >= cal_boil_t:
			var need: float = (cal_boil_t - t) * cap
			evap_l = (heat - need) / L_VAP  # kg páry = litrů vody
			vs["t"] = cal_boil_t
		elif kw > 0.0 or t > cal_ambient:
			if kw > 0.0:
				vs["t"] = t + heat / cap
		vs["v"] = maxf(float(vs["v"]) - evap_l, 0.0)
		var excess_kw: float = evap_l * L_VAP / maxf(dt, 0.001)
		var target: float = clampf(excess_kw / BOIL_FULL_KW, 0.0, 1.0) if float(vs["t"]) >= cal_boil_t - 0.05 else 0.0
		vs["boil"] = lerpf(float(vs["boil"]), target, minf(dt / 20.0, 1.0))
		vs["evap_lph"] = lerpf(float(vs["evap_lph"]), evap_l / maxf(dt, 0.001) * 3600.0, minf(dt / 30.0, 1.0))
		if key != "rvp":
			continue
		# bílkovinný zlom: po začátku varu se tvoří pěna, ze které při plné nádobě a plném varu kypí přes okraj
		if float(vs["boil"]) > 0.05:
			if float(vs["malt"]) * MALT_L_PER_KG < 0.05 * float(vs["v"]):  # chmelovar = var sladiny (nanejvýš zbytky šrotu); povařování rmutu se nepočítá
				vs["boil_s"] = float(vs["boil_s"]) + dt
				if float(vs["boil"]) > 0.3:
					# hot break: bílkoviny se srážejí hlavně na začátku varu a berou s sebou kousek extraktu
					var hb: float = float(vs["ext"]) * HOT_BREAK_PER_H * dt / 3600.0 * exp(-float(vs["boil_s"]) / 3000.0) * 1.8
					vs["ext"] = float(vs["ext"]) - hb
					hot_break_ext += hb
			var p: float = plato("rvp")
			var foam_t: float = float(vs["boil"]) * clampf(p / 8.0, 0.0, 1.5) * 1.4 * exp(-float(vs["boil_s"]) / 900.0) + 0.12 * float(vs["boil"])
			vs["foam"] = lerpf(float(vs["foam"]), foam_t, minf(dt / 40.0, 1.0))
			if float(vs["foam"]) > 1.0 and float(vs["v"]) / float(vs["cap"]) > 0.8:
				vs["v"] = maxf(float(vs["v"]) - 0.6 * dt / 60.0, 0.0)  # překypění: ztráta mladiny
				vs["overflow"] = true
		else:
			vs["foam"] = lerpf(float(vs["foam"]), 0.0, minf(dt / 120.0, 1.0))
			if float(vs["t"]) < 90.0:
				vs["overflow"] = false
	_steam_pressure(used_kw, dt)


func _mash_conversion(dt: float) -> void:
	_mash_conversion_in("rvp", dt)
	if not bool(vessels["sk"].get("bed", false)):  # SK s rmutem před odrmutováním zcukřuje stejně jako varna
		_mash_conversion_in("sk", dt)


## Složení extraktu v nádobě (podíly na celkovém extraktu): sg = rozpuštěný nezcukřený škrob, dx = dextriny,
## ld = limitní dextriny, f = zkvasitelné cukry, ns = nezkvasitelné látky. Prázdné nebo nenastavené = běžná mladina.
func _xf(vs: Dictionary) -> Dictionary:
	var x: Dictionary = vs.get("xf", {})
	var sum := 0.0
	for k in XF_DEFAULT:
		sum += float(x.get(k, 0.0))
	if sum < 0.5:
		x = XF_DEFAULT.duplicate()
		vs["xf"] = x
	return x


## podíl zkvasitelných cukrů v extraktu nádoby (z toho se po zakvašení zkvasí cukr)
func fermentable(key: String) -> float:
	return float(_xf(vessels[key]).get("f", 0.8))


## Rmut: rozpouštění zrn (želatinace škrobu kolem 62 °C) a enzymy v kapalině – α-amyláza (optimum 72 °C) štěpí škrob na
## dextriny, β-amyláza (optimum 63 °C) z dextrinů dělá zkvasitelné cukry; obě tepelně hynou a závisí na pH rmutu.
## Rovnice podle původního trenažéru (sim.gd: react); rychlost rozpouštění je nastavena na dosavadní průběh (GEL_K).
func _mash_conversion_in(key: String, dt: float) -> void:
	var vs: Dictionary = vessels[key]
	var malt: float = float(vs["malt"])
	var m: float = dt / 60.0
	var locked: float = 0.0
	if key == "rvp":
		# hrudky: s přečerpaným sladem odejde i jejich část; suché jádro se už nerozpustí
		var clump: float = float(vs.get("clump", 0.0))
		var prev: float = float(vs.get("clump_m", malt))
		if malt < prev - 0.01 and prev > 0.0:
			var gone: float = clump * minf((prev - malt) / prev, 1.0)
			dead_malt += CLUMP_DRY * gone
			clump -= gone
		clump = minf(clump, malt)
		clump -= clump * minf(dt / (CLUMP_REL_STIR_S if bool(stir.get("rvp", false)) else 3600.0), 1.0)
		vs["clump"] = clump
		vs["clump_m"] = malt
		locked = clump
	if malt < 0.5 or float(vs["v"]) < 1.0:
		return
	var t: float = float(vs["t"])
	var ext: float = float(vs["ext"])
	var x: Dictionary = _xf(vs)
	var mass := {}
	for k in XF_DEFAULT:
		mass[k] = float(x.get(k, 0.0)) * ext
	# 1) rozpouštění: škrob ze zrn do roztoku, rychle rozpustné látky (NI_SHARE) rovnou jako cukry a nezkvasitelné
	var remaining: float = maxf((malt - locked - dead_malt) * cal_extract_yield - ext, 0.0)
	var da: float = remaining * minf(1.0, cal_gel_k * m / (1.0 + exp(-(t - 62.0) / 1.3)))
	mass["sg"] += da * (1.0 - NI_SHARE)
	mass["f"] += da * NI_SHARE * 0.35
	mass["ns"] += da * NI_SHARE * 0.65
	# 2) enzymy: aktivita na m³ kapaliny, vliv pH, tepelná inaktivace
	var vm3: float = maxf(0.05, float(vs["v"]) / 1000.0)
	var act: float = malt * enz_mean / vm3
	var via: float = float(vs.get("dA", 1.0))
	var vib: float = float(vs.get("dB", 1.0))
	var ra: float = 0.0015 * act * via * exp(-pow((t - 72.0) / 9.0, 2.0)) * m * ph_a
	var rb: float = 0.0016 * act * vib * exp(-pow((t - 63.0) / 7.0, 2.0)) * m * ph_b
	var d_a: float = mass["sg"] * minf(1.0, ra)
	mass["sg"] -= d_a
	mass["dx"] += 0.8 * d_a
	mass["ld"] += 0.2 * d_a
	var d1: float = mass["dx"] * minf(1.0, rb)
	var d2: float = mass["sg"] * minf(1.0, 0.25 * rb)
	mass["dx"] -= d1
	mass["sg"] -= d2
	mass["f"] += d1 + d2
	vs["dA"] = via * exp(-0.02 * exp((t - 76.0) / 2.2) * m)
	vs["dB"] = vib * exp(-0.02 * exp((t - 65.0) / 2.2) * m)
	var tot := 0.0
	for k in XF_DEFAULT:
		tot += float(mass[k])
	vs["ext"] = tot
	if tot > 0.0:
		for k in XF_DEFAULT:
			x[k] = float(mass[k]) / tot


func _glycol(dt: float) -> void:
	var lv: Dictionary = vessels["lv"]
	if active.get("glykol", false) and not blocked.get("glykol", false):
		if float(lv["t"]) > float(setpoint.get("lv", 1.0)):  # glykol chladí jen do nastavené teploty ledové vody
			lv["t"] = maxf(float(lv["t"]) - 0.6 * dt / 60.0, float(setpoint.get("lv", 1.0)))
	# ledová voda (C5) chladí jen v deskovém chladiči při spílání – viz _flow("virivka_chladic"); vířivka sama chlazená není
