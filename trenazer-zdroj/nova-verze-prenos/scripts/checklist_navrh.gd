class_name ChecklistNavrh
extends RefCounted
## Návrh postupu ke 43 bodům checklistu varny (výchozí verze, než obsluha body popíše a označí): co se v kroku
## nastaví (cesty, šoupě a šnek, míchadlo, ohřev), kdy je krok splněný (kontrola) a co při automatickém projetí
## várky dělá obsluha. Uložený stav a popis kroku z checklist_uceni.json mají v panelu přednost.

## stroje mimo trasy (šnek na slad, kopačka, kopyto, výpusť SK): přepínání cest je nechává, jak jsou
const MACHINE := ["gate", "air", "vent", "screwwater", "screw", "rake", "rakelift", "hoof", "drain", "hose", "pushwater", "lid"]
const VARNA := ["voda_panev", "voda_rad", "rmut_sk", "rmut_varna", "dilo", "vorlauf", "sk_c2", "podraz", "vyslazovani", "varna_virivka", "virivka_chladic", "lv_fill"]
const BOIL_KW := 180.0  # pára při chmelovaru: přiměřený var (odpar asi 7 % za hodinu), ne naplno

## t = návrh postupu · zap / vyp = cesty spuštěné / zastavené na začátku kroku ("*" = všechny) · konec = cesty
## zastavené na konci kroku · konec_zap = cesta spuštěná během kroku · set / konec_set = stroje podle druhu na
## začátku / konci kroku (air, vent, gate, screwwater, screw, drain = výpusť SK, hoof = kopyto dole, rakelift =
## kopačka dole, rake = kopačka se točí, glykol = vypínač) · michadlo · ohrev = cíl termostatu (0 = pára vypnutá)
## · cil = cílová teplota ledové vody · pasmo = [od °C, do °C, s] souvislá prodleva · var = s varu s míchadlem
## · tekla = [cesta, s] jak dlouho má cesta v kroku téct · rucne = bez měření, potvrdí obsluha (cas = s simulace,
## za které ho potvrdí automatické projetí) · natrhnout = největší povolený průtok odběru ze SK (podíl, 1 = naplno),
## krok chce cestu „natrhnutou“ přivřenou klapkou (Ctrl + kolečko); automatické projetí přivře klapku na vorlaufu na 30 %
const N := [
	{"t": "Zavřít všechny klapky (i šoupě, vzduch, vodu do šneku a výpusť SK); čerpadla C1–C5, šnek i kopačka stojí, kopyto a kopačka nahoře, míchadlo vypnuté.", "vyp": ["*"], "set": {"air": false, "vent": false, "gate": false, "screwwater": false, "screw": false, "drain": false, "hoof": false, "rakelift": false, "rake": false, "hose": false, "vratka": false, "pushwater": false, "lid": false}, "michadlo": false},
	{"t": "Ledová voda: z řadu dopustit tank aspoň do poloviny (cesta Napuštění ledové vody), pak napouštění zavřít. Zapnout glykol a cirkulaci přes glykolový chladič čerpadlem C5, chladit na 3 °C.", "zap": ["lv_fill", "glykol"], "konec": ["lv_fill"], "set": {"glykol": true}, "cil": {"lv": 3.0}},
	{"t": "Otevřít tlakový vzduch (pohon šoupěte) a odvzdušnění u šoupěte.", "set": {"air": true, "vent": true}},
	{"t": "Najet páru na bojler: termostat bojleru AUTO, cíl 80 °C. Počkat, až má voda v bojleru aspoň 70 °C.", "ohrev": {"hlt": 80.0}},
	{"t": "Napouštět do kotle zároveň horkou vodu z bojleru (C4, cesta Voda z bojleru) a studenou z řadu; poměrem držet 35–40 °C.", "zap": ["voda_panev", "voda_rad"]},
	{"t": "Napouštět „do výpustě“, asi 20 hl vody 35–40 °C; pak obě cesty zavřít (C4 stojí).", "konec": ["voda_panev", "voda_rad"]},
	{"t": "Zapnout míchadlo kotle.", "michadlo": true},
	{"t": "Otevřít šoupě na slad a vodu do šneku z bojleru (kohout na čtvrtku). Bez tlakového vzduchu se šoupě neotevře.", "set": {"gate": true, "screwwater": true}},
	{"t": "Kontrola: ve varně je vše ostatní zavřené – žádná cesta varny neteče a C1–C4 stojí (glykol může běžet).", "vyp": ["voda_panev", "voda_rad", "rmut_sk", "rmut_varna", "dilo", "vorlauf", "sk_c2", "podraz", "vyslazovani", "varna_virivka", "virivka_chladic", "lv_fill"]},
	{"t": "Pustit šnek: celá dávka sladu (700 kg) se sype do míchané vody asi 10 minut.", "set": {"screw": true}},
	{"t": "Vypnout šnek, zavřít šoupě a vodu do šneku – ať nejde pára do šneku.", "set": {"screw": false, "gate": false, "screwwater": false}},
	{"t": "Osprchovat kotel: asi 30 s pustit horkou vodu z bojleru sprchami kotle (cesta Voda z bojleru), pak zavřít.", "zap": ["voda_panev"], "konec": ["voda_panev"], "tekla": ["voda_panev", 30.0]},
	{"t": "Vystírka: dílo s míchadlem na 40 °C, držet 10 min (38–42 °C).", "ohrev": {"rvp": 40.0}, "michadlo": true, "pasmo": [38.0, 42.0, 600.0]},
	{"t": "Ohřát dílo s míchadlem na 55 °C (bílkovinná prodleva); termostat drží 55 °C i během přípravy scezovačky.", "ohrev": {"rvp": 55.0}, "michadlo": true},
	{"t": "Scezovací káď: otevřít výpusť do kanálu a vypustit vodu z vyplachování; kopyto dolů a kopačkou vyhrnout zbytky mláta z minulé várky. Pak výpusť zavřít, kopyto nahoru a kopačku zastavit.", "set": {"drain": true, "hoof": true, "rake": true}, "konec_set": {"drain": false, "hoof": false, "rake": false}},
	{"t": "Čerpadlo bojleru C4 na 6–7: asi 30 s prohřát trubky horkou vodou do SK (sprchy, cesta Vyslazování).", "zap": ["vyslazovani"], "konec": ["vyslazovani"], "tekla": ["vyslazovani", 30.0]},
	{"t": "Podrazit: voda z bojleru do věnce a pod síta (cesta Podrážení), asi 40 s.", "zap": ["podraz"], "konec": ["podraz"], "tekla": ["podraz", 40.0]},
	{"t": "Zavřít podrážení, otevřít výpusť SK do kanálu, pustit sprchu (aspoň 1 min) a počkat, až z výpusti teče čistá voda. Pak sprchu i výpusť zavřít.", "vyp": ["podraz"], "zap": ["vyslazovani"], "konec": ["vyslazovani"], "set": {"drain": true}, "konec_set": {"drain": false}},
	{"t": "Zaplavit čerpadlo a napustit vodu nad síta (cesta Podrážení), asi 250 L v SK.", "zap": ["podraz"], "konec": ["podraz"]},
	{"t": "Kopyto nahoru, kopačku spustit dolů (hydraulický zdvih).", "set": {"hoof": false, "rakelift": true}},
	{"t": "Převést dílo do SK čerpadlem C1 (cesta Rmut do SK) s míchadlem; v kotli nechat 1. rmut asi 10 hl.", "zap": ["rmut_sk"], "konec": ["rmut_sk"], "michadlo": true},
	{"t": "Osprchovat kroužek a kotel krátce vodou z bojleru (asi 20 s), pak zavřít.", "zap": ["voda_panev"], "konec": ["voda_panev"], "tekla": ["voda_panev", 20.0]},
	{"t": "1. rmut: s míchadlem ohřát na 72 °C a držet 10 min (70–74 °C).", "ohrev": {"rvp": 72.0}, "michadlo": true, "pasmo": [70.0, 74.0, 600.0]},
	{"t": "1. rmut: přivést k varu a vařit 10 min s míchadlem.", "ohrev": {"rvp": 100.0}, "michadlo": true, "var": 600.0},
	{"t": "Vypnout páru a vrátit 1. rmut do SK (C1, cesta Rmut do SK) až do prázdna. Ideálně 67 °C po návratu.", "ohrev": {"rvp": 0.0}, "zap": ["rmut_sk"], "konec": ["rmut_sk"], "ideal": 67.0},
	{"t": "2. rmut: přečerpat asi 10 hl hustého díla ze SK do kotle (C1, cesta Rmut ze SK do varny), s míchadlem ohřát na 72 °C, držet 10 min, pak vařit 10 min.", "zap": ["rmut_varna"], "konec": ["rmut_varna"], "michadlo": true, "pasmo": [70.0, 74.0, 600.0], "var": 600.0},
	{"t": "Vypnout páru a vrátit 2. rmut do SK (C1, cesta Rmut do SK) až do prázdna – odrmutování, ideálně 76–78 °C.", "ohrev": {"rvp": 0.0}, "zap": ["rmut_sk"], "konec": ["rmut_sk"], "ideal": 77.0},
	{"t": "Spláchnout mláto z kopačky a protlačit trubky vodou pod síta (cesta Podrážení, asi 30 s).", "zap": ["podraz"], "konec": ["podraz"], "tekla": ["podraz", 30.0]},
	{"t": "20 min klid: do SK nic neteče ani se neodebírá a kopačka stojí – mláto se usadí do filtračního lože.", "vyp": ["rmut_sk", "rmut_varna", "dilo", "vorlauf", "sk_c2", "podraz", "vyslazovani"], "set": {"rake": false}, "michadlo": false},
	{"t": "Pustit vyslazovací čerpadlo C2, cestu jen „natrhnout“ – klapku na vorlaufu přivřít (Ctrl + kolečko nad klapkou, asi 30 %), pomalý začátek, ať se lože neutáhne. Nejdřív vorlauf: sladina jde přes průhledítko zpátky do SK, dokud není čirá (zákal pod 10 %); pak přepnout do kotle (cesta Dílo).", "zap": ["vorlauf"], "konec": ["vorlauf"], "konec_zap": ["dilo"], "natrhnout": 0.55},
	{"t": "Kopačku pustit brzy, asi na 1–1,5 otáčky, a jet s ní nahoru a dolů – nakypří lože, ať se neutáhne (chvíli zakalí sladinu). Pak ji zastavit a zvednout.", "set": {"rake": true}, "konec_set": {"rake": false, "rakelift": false}},
	{"t": "Až je nad mlátem vidět hladina (méně než asi 3 cm sladiny), pustit sprchu: vyslazovací voda 78 °C z bojleru (cesta Vyslazování).", "ohrev": {"hlt": 78.0}, "konec_zap": ["vyslazovani"]},
	{"t": "Asi při 30 hl v kotli (u tyčky) vyslazovací vodu zastavit.", "konec": ["vyslazovani"]},
	{"t": "Dopustit po hranu: nechat sladinu doběhnout do kotle, až nad mlátem nezbude kapalina; pak odtah zavřít (předkvasná mladina).", "konec": ["dilo"]},
	{"t": "Chmelovar 90 min: termostat kotle 100 °C, pára jen na přiměřený var (asi 180 kW), ať mladina nepřekypí. Chmel podle rozpisu: 5 kg (α 5 %) na začátku varu, 2 kg (α 12 %) ve 40. a 65. minutě. Po 90 min páru vypnout.", "ohrev": {"rvp": 100.0}, "konec_ohrev": {"rvp": 0.0}},
	{"t": "Propláchnout trubky k vířivce (ručně).", "rucne": true, "cas": 60.0},
	{"t": "Přečerpat mladinu do vířivé kádě (C1, cesta Varna do vířivky) až do prázdna; tangenciální nástřik roztočí vír.", "zap": ["varna_virivka"], "konec": ["varna_virivka"]},
	{"t": "Vířivka 20 min v klidu – kaly se usadí do kužele. Pak spílací hadici na vratku, otevřít klapku vratky a čerpadlem C3 (cesta Vířivka přes chladič) vymýt filtr a chladič: voda z potrubí a první mladina se vrací do vířivky.", "set": {"hose": false}, "konec_zap": ["virivka_chladic"], "konec_set": {"vratka": true}},
	{"t": "Ledová voda připravená k chladiči: v tanku aspoň tolik vody, kolik je mladiny, vychlazená na 4 °C; pustit ji čerpadlem C5 do deskového chladiče (cesta Ledová voda), ať je chladič studený.", "zap": ["ledovka"]},
	{"t": "Až za chladičem teče ledová mladina (9,5 °C a méně), přepnout spílací hadici z vratky na tank (spilku) a klapku vratky zavřít.", "konec_set": {"hose": true, "vratka": false}},
	{"t": "Ledová voda teče čerpadlem C5 do deskového chladiče, ohřátá voda odtéká do bojleru (cesta Ledová voda).", "zap": ["ledovka"]},
	{"t": "Spílat: mladina z vířivky čerpadlem C3 přes deskový chladič a hadici do spilky, zakvašovací teplota 8,5 °C. Když je vířivka prázdná, C3 i ledová voda běží dál – následuje protláčka.", "zap": ["virivka_chladic"]},
	{"t": "Protláčka: pustit vodu na protláčku (kohout a klapka na odbočce k sání C3); C3 dotlačí mladinu z chmelového filtru, chladiče a hadice do spilky. Jakmile z hadice teče voda, vodu zavřít, C3 i ledovou vodu zastavit a spilku zavíčkovat.", "set": {"pushwater": true}, "konec": ["virivka_chladic", "ledovka"], "konec_set": {"pushwater": false, "lid": true}},
]

var sp: SimPanel
var plan: PlanData
var sim: BrewSim
var step: int = -1
var want: Dictionary = {}  # cesty, které mají podle návrhu téct
var paused: Dictionary = {}  # chtěné cesty, které obsluha na chvíli zastavila, protože by tlačily vodu jinam
var keep: Dictionary = {}  # klapky mimo trasy, které mají zůstat otevřené (klapka vratky spílací hadice)
var manual_ok: bool = false
var t_flow: Dictionary = {}  # cesta -> s, jak dlouho v tomto kroku tekla
var hold: float = 0.0  # souvislá prodleva v pásmu (po splnění zůstane)
var boil: float = 0.0  # var s míchadlem (s)
var quiet_max: float = 0.0  # nejdelší klid SK v kroku (s)
var wp_quiet_max: float = 0.0
var in_step: float = 0.0  # s simulace od začátku kroku
var phase: int = 0  # fáze kroku při automatickém projetí (2. rmut, kopačka)
var base: Dictionary = {}
var _quiet: float = 0.0
var _wp_quiet: float = 0.0


func _init(p_sp: SimPanel, p_plan: PlanData) -> void:
	sp = p_sp
	plan = p_plan
	sim = sp.sim


func is_manual(i: int) -> bool:
	return bool((N[i] as Dictionary).get("rucne", false))


func text(i: int) -> String:
	return str((N[i] as Dictionary)["t"])


## začátek sledování kroku: vynuluje časy a zapamatuje si výchozí objemy
func begin(i: int) -> void:
	step = i
	manual_ok = false
	t_flow.clear()
	hold = 0.0
	boil = 0.0
	quiet_max = 0.0
	wp_quiet_max = 0.0
	_quiet = 0.0
	_wp_quiet = 0.0
	in_step = 0.0
	phase = 0
	var sk: Dictionary = sim.vessels["sk"]
	var h: float = float(sk.get("rake_h", 1.0))
	base = {"rvp_v": _mash_l(), "wp_v": float(sim.vessels["wp"]["v"]), "clock": sim.clock, "rev": float(sk.get("rev", 0.0)), "h_min": h, "h_max": h}


# ---------- nastavení prvků ----------
func _fl(rn: String) -> bool:
	return bool(sp._flowing.get(rn, false))


func _lbl(rn: String) -> String:
	return str(sim.routes.get(rn, {}).get("label", rn))


## objem v kotli včetně šrotu (L)
func _mash_l() -> float:
	var rv: Dictionary = sim.vessels["rvp"]
	return float(rv["v"]) + float(rv["malt"]) * BrewSim.MALT_L_PER_KG


## výška sladiny nad mlátem v SK (cm)
func above_bed_cm() -> float:
	var sk: Dictionary = sim.vessels["sk"]
	var pores: float = float(sk["malt"]) * BrewSim.BED_L_PER_KG * 0.45
	return maxf(float(sk["v"]) - pores, 0.0) / 1000.0 / float(sk["area"]) * 100.0


## klapky a čerpadla tras: otevře klapky všech chtěných cest a pustí jejich čerpadla, ostatní zavře / zastaví
## (stroje – šnek, kopačka, kopyto, výpusť SK – nechá být)
func apply_routes() -> void:
	paused.clear()
	_apply_set()
	# potrubí je propojené doopravdy: když okruh ledovky (C5) běží zároveň s vodou z řadu nebo z bojleru, čerpadlo C5 tlačí vodu
	# i do kotle – obsluha ho na tu chvíli zastaví (okruh se vrátí, až cesty vody skončí)
	if want.has("glykol") and _has_stray():
		paused["glykol"] = true
		_apply_set()


## teče něco mimo plánované cesty (síť našla další cíl)?
func _has_stray() -> bool:
	for rn in sp._flowing:
		if bool(sp._flowing[rn]) and bool(sim.routes.get(rn, {}).get("synthetic", false)):
			return true
	return false


func _apply_set() -> void:
	var open_v: Dictionary = {}
	var pumps: Dictionary = {}
	for rn in want:
		if paused.has(rn):
			continue
		for vid in sp.route_valve_ids(str(rn)):
			open_v[str(vid)] = true
		var p: String = sim.route_pump(str(rn))
		if p != "":
			pumps[p] = true
	for it in plan.items:
		var t := str(it.get("type"))
		if str(it.get("kind", "")) in MACHINE or keep.has(str(it.id)):
			continue
		if t == "valve":
			sp._set_item(it, "open", open_v.has(str(it.id)))
		elif t == "pump":
			sp._set_item(it, "on", pumps.has(str(it.id)))
	var active: Array = []
	for rn in want:
		if not paused.has(rn):
			active.append(str(rn))
	sp.set_manifold_ports_for(active)  # FYZIKA+ oprava: rozbočovač otevřít pro všechny puštěné cesty
	sp._update_flows()


func _start(rn: String, log_it: bool = true) -> void:
	if not want.has(rn):
		want[rn] = true
		apply_routes()
		if log_it:
			_say("spuštěno: " + _lbl(rn))


func _stop(rn: String, log_it: bool = true) -> void:
	if want.has(rn):
		want.erase(rn)
		apply_routes()
		if log_it:
			_say("zastaveno: " + _lbl(rn))


## stroj podle druhu: pohon (šnek, kopačka) zapnout / vypnout, klapka (šoupě, vzduch, výpusť, kopyto, zdvih) otevřít / zavřít;
## hose = spílací hadice na tanku (false = na vratce), vratka = klapka vratky, kam se hadice vrací
func _set_kind(kind: String, on: bool) -> void:
	if kind == "glykol":
		for g in plan.items:
			if str(g.get("type")) == "sw":
				sp._set_item(g, "on", on)
		return
	if kind == "pushwater":  # voda na protláčku i klapky na cestě k sání C3 (pole via)
		var pw: Dictionary = sp._kind_item("valve", "pushwater")
		if pw.is_empty():
			return
		sp._set_item(pw, "open", on)
		for via in pw.get("via", []):
			if on:
				keep[str(via)] = true
			else:
				keep.erase(str(via))
			sp._set_item(plan.get_item(str(via)), "open", on)
		return
	if kind == "vratka":
		var vid := _vratka_id()
		if vid != "":
			if on:
				keep[vid] = true
			else:
				keep.erase(vid)
			sp._set_item(plan.get_item(vid), "open", on)
		return
	var it: Dictionary = sp._kind_item("pump", kind)
	if not it.is_empty():
		sp._set_item(it, "on", on)
		return
	it = sp._kind_item("valve", kind)
	if not it.is_empty():
		sp._set_item(it, "open", on)


## klapka vratky spílací hadice (pole vratka_valve u ovladače hadice), "" když v plánu není
func _vratka_id() -> String:
	for it in plan.items:
		if it.has("vratka_valve"):
			return str(it["vratka_valve"])
	return ""


## číselné pole simulace, které nemusí existovat (model hadice a chladiče se vyvíjí)
func _simf(field: String, def: float) -> float:
	var val: Variant = sim.get(field)
	return def if val == null else float(val)


## objem filtru, chladiče a hadice (L) – konstanta simulace LINE_L
func _line_l() -> float:
	return float(sim.get_script().get_script_constant_map().get("LINE_L", 60.0))


## protláčka hotová: potrubí je plné vody a do spilky se dotlačila mladina
func _pushed() -> bool:
	return _simf("line_water", 0.0) >= _line_l() - 0.5 and _simf("pushed_wort", 0.0) > 1.0


## teplota mladiny za chladičem (-1 = neteče); bez modelu chladiče podle toho, jestli teče ledová voda
func _cool_out() -> float:
	if sim.get("cool_out_t") == null:
		return 8.5 if _fl("ledovka") and _fl("virivka_chladic") else -1.0
	return float(sim.get("cool_out_t"))


## stav stroje podle druhu (prvek v plánu; simulace ho přebírá každý snímek)
func _kind_on(kind: String) -> bool:
	if kind == "vratka":
		return bool(plan.get_item(_vratka_id()).get("open", false))
	var it: Dictionary = sp._kind_item("pump", kind)
	if not it.is_empty():
		return bool(it.get("on", false))
	return bool(sp._kind_item("valve", kind).get("open", false))


func _has_kind(kind: String) -> bool:
	return not sp._kind_item("pump", kind).is_empty() or not sp._kind_item("valve", kind).is_empty()


## zákal vody z výpusti SK (0..1), pokud ho simulace počítá
func _drain_turb() -> float:
	return float(sim.call("drain_turb")) if sim.has_method("drain_turb") else 0.0


func _set_stir(on: bool) -> void:
	sim.stir["rvp"] = on
	for c in sp._body.get_children():
		if c is CheckButton and str((c as CheckButton).text).begins_with("Míchadlo"):
			(c as CheckButton).set_pressed_no_signal(on)


## termostat nádoby: cíl °C (AUTO), 0 = pára vypnutá
func _heat(key: String, c: float) -> void:
	if c <= 0.0:
		sim.auto_mode[key] = false
		sim.heater_kw[key] = 0.0
	else:
		sim.setpoint[key] = c
		sim.auto_mode[key] = true
		if sp._sp_spin.has(key):
			(sp._sp_spin[key] as SpinBox).set_value_no_signal(c)
	if sp._auto_ck.has(key):
		(sp._auto_ck[key] as CheckButton).set_pressed_no_signal(bool(sim.auto_mode[key]))


func _cil(key: String, c: float) -> void:
	sim.setpoint[key] = c
	if sp._sp_spin.has(key):
		(sp._sp_spin[key] as SpinBox).set_value_no_signal(c)


func _max_kw(key: String, kw: float) -> void:
	if absf(float(sim.max_kw[key]) - kw) < 0.5:
		return
	sim.max_kw[key] = kw
	if sp._mk_spin.has(key):
		(sp._mk_spin[key] as SpinBox).set_value_no_signal(kw)


func _say(msg: String) -> void:
	sp._log("Checklist %d: %s" % [step + 1, msg])


## výchozí stav ovládání pro novou várku (míchadlo, termostaty, výkony, rmutovací program)
func reset_controls() -> void:
	want.clear()
	keep.clear()
	sim.prog_run = false
	_set_stir(false)
	_heat("hlt", 0.0)
	_heat("rvp", 0.0)
	_cil("hlt", 78.0)
	_cil("rvp", 65.0)
	_cil("lv", 4.0)
	_max_kw("hlt", 250.0)
	_max_kw("rvp", 350.0)


func _apply_entry(n: Dictionary) -> void:
	for rn in n.get("vyp", []):
		if str(rn) == "*":
			want.clear()
		else:
			want.erase(str(rn))
	for rn in n.get("zap", []):
		want[str(rn)] = true
	apply_routes()
	var st: Dictionary = n.get("set", {})
	for k in st:
		_set_kind(str(k), bool(st[k]))
	if n.has("michadlo"):
		_set_stir(bool(n["michadlo"]))
	var h: Dictionary = n.get("ohrev", {})
	for k in h:
		_heat(str(k), float(h[k]))
	var c: Dictionary = n.get("cil", {})
	for k in c:
		_cil(str(k), float(c[k]))


## automatické projetí: vstup do kroku (nastaví, co krok mění oproti předchozímu)
func enter(i: int) -> void:
	begin(i)
	_apply_entry(N[i])


## automatické projetí: konec kroku (zastaví cesty a stroje kroku, vypne páru po chmelovaru)
func finish() -> void:
	var n: Dictionary = N[step]
	var changed := false
	for rn in n.get("konec", []):
		if want.has(str(rn)):
			want.erase(str(rn))
			changed = true
	for rn in n.get("konec_zap", []):
		if not want.has(str(rn)):
			want[str(rn)] = true
			changed = true
	if changed:
		apply_routes()
	var ks: Dictionary = n.get("konec_set", {})
	for k in ks:
		_set_kind(str(k), bool(ks[k]))
	var h: Dictionary = n.get("konec_ohrev", {})
	for k in h:
		_heat(str(k), float(h[k]))


## stav ovládání na začátku kroku i podle návrhu (projde všechny předchozí kroky)
func expected_at(i: int) -> Dictionary:
	var w: Dictionary = {}
	var st: Dictionary = {}
	var stir: Variant = null
	var heat: Dictionary = {}
	var cil: Dictionary = {}
	for k in range(i + 1):
		var n: Dictionary = N[k]
		for rn in n.get("vyp", []):
			if str(rn) == "*":
				w.clear()
			else:
				w.erase(str(rn))
		for rn in n.get("zap", []):
			w[str(rn)] = true
		var s: Dictionary = n.get("set", {})
		for kk in s:
			st[kk] = s[kk]
		if n.has("michadlo"):
			stir = n["michadlo"]
		var h: Dictionary = n.get("ohrev", {})
		for kk in h:
			heat[kk] = h[kk]
		var c: Dictionary = n.get("cil", {})
		for kk in c:
			cil[kk] = c[kk]
		if k < i:
			for rn in n.get("konec", []):
				w.erase(str(rn))
			for rn in n.get("konec_zap", []):
				w[str(rn)] = true
			var ks: Dictionary = n.get("konec_set", {})
			for kk in ks:
				st[kk] = ks[kk]
			var hk: Dictionary = n.get("konec_ohrev", {})
			for kk in hk:
				heat[kk] = hk[kk]
	return {"want": w, "set": st, "stir": stir, "heat": heat, "cil": cil}


## tlačítko „Nastavit návrh“: klapky, čerpadla, šnek, míchadlo a ohřev jako na začátku kroku i
func apply_expected(i: int) -> void:
	var e: Dictionary = expected_at(i)
	begin(i)
	want = e["want"]
	apply_routes()
	var st: Dictionary = e["set"]
	for k in st:
		_set_kind(str(k), bool(st[k]))
	if e["stir"] != null:
		_set_stir(bool(e["stir"]))
	var h: Dictionary = e["heat"]
	for k in h:
		_heat(str(k), float(h[k]))
	var c: Dictionary = e["cil"]
	for k in c:
		_cil(str(k), float(c[k]))


# ---------- průběh kroku ----------
## každý snímek (dt = s simulace): časy toku cest, prodlevy, var, klid SK a vířivky, výpusť SK
func update(dt: float) -> void:
	if step < 0:
		return
	in_step += dt
	for rn in sp._flowing:
		if bool(sp._flowing[rn]):
			t_flow[rn] = float(t_flow.get(rn, 0.0)) + dt
	var n: Dictionary = N[step]
	var rv: Dictionary = sim.vessels["rvp"]
	var stir: bool = bool(sim.stir.get("rvp", false))
	if n.has("pasmo"):
		var p: Array = n["pasmo"]
		if hold < float(p[2]):
			if stir and float(rv["v"]) > 100.0 and float(rv["t"]) >= float(p[0]) and float(rv["t"]) <= float(p[1]):
				hold += dt
			else:
				hold = 0.0
	if n.has("var") and boil < float(n["var"]):
		if float(rv.get("boil", 0.0)) > 0.05 and stir and float(rv["v"]) > 100.0:
			boil += dt
	_quiet = _quiet + dt if sim.sk_quiet() else 0.0
	quiet_max = maxf(quiet_max, _quiet)
	var wp: Dictionary = sim.vessels["wp"]
	_wp_quiet = _wp_quiet + dt if (float(wp["v"]) > 1.0 and not sim.active.has("varna_virivka") and not sim.active.has("virivka_chladic")) else 0.0
	wp_quiet_max = maxf(wp_quiet_max, _wp_quiet)
	if n.has("natrhnout") and (_fl("vorlauf") or _fl("dilo")) and sim._sk_throttle() <= float(n["natrhnout"]):
		base["natrh"] = true  # cesta byla aspoň chvíli natrhnutá přivřenou klapkou
	var h: float = float(sim.vessels["sk"].get("rake_h", 1.0))
	base["h_min"] = minf(float(base.get("h_min", h)), h)
	base["h_max"] = maxf(float(base.get("h_max", h)), h)


## automatické projetí: co v kroku dělá obsluha (dopouštění, míchání vody, zastavení cest, chmel, pára)
func operate(_dt: float) -> void:
	if step < 0:
		return
	var n: Dictionary = N[step]
	var v: Dictionary = sim.vessels
	var rv: Dictionary = v["rvp"]
	if bool(n.get("rucne", false)) and not manual_ok and in_step >= float(n.get("cas", 30.0)):
		manual_ok = true
		_say("ruční krok potvrzen automaticky")
	if n.has("tekla"):
		var tk: Array = n["tekla"]
		if float(t_flow.get(str(tk[0]), 0.0)) >= float(tk[1]):
			_stop(str(tk[0]))
	match step:
		1:
			if float(v["lv"]["v"]) >= 0.5 * float(v["lv"]["cap"]) + 100.0:
				_stop("lv_fill")
		4, 5:
			# míchání: studená z řadu teče stále, horkou z bojleru obsluha přivírá podle teploty v kotli
			if float(rv["v"]) > 30.0:
				if float(rv["t"]) > 39.0:
					_stop("voda_panev", false)
				elif float(rv["t"]) < 36.5 and float(v["hlt"]["v"]) > 50.0:
					_start("voda_panev", false)
			if step == 5 and float(rv["v"]) >= 2000.0:
				_stop("voda_panev")
				_stop("voda_rad")
		17:
			# sprcha teče, dokud z výpusti nejde čistá voda (aspoň 1 min)
			if float(t_flow.get("vyslazovani", 0.0)) >= 60.0 and _drain_turb() < 0.1:
				_stop("vyslazovani")
		18:
			if float(v["sk"]["v"]) >= 250.0:
				_stop("podraz")
		20:
			if _mash_l() <= 1000.0:
				_stop("rmut_sk")
		24, 26:
			if _mash_l() <= 20.0:
				_stop("rmut_sk")
		25:
			if phase == 0 and _mash_l() >= 1000.0:
				_stop("rmut_varna")
				_heat("rvp", 72.0)
				phase = 1
			elif phase == 1 and hold >= 600.0:
				_heat("rvp", 100.0)
				_say("2. rmut: povařit")
				phase = 2
		29:
			# vorlauf s „natrhnutou“ klapkou, dokud sladina v průhledítku není čirá, pak přepnout do kotle
			if phase == 0 and not base.has("natrh_set") and _fl("vorlauf"):
				base["natrh_set"] = true
				_natrhnout_vorlauf(30)
			if phase == 0 and float(v["sk"].get("turb", 1.0)) <= 0.1 and t_flow.has("vorlauf"):
				_stop("vorlauf", false)
				_start("dilo", false)
				_say("sladina je čirá (zákal %.0f %%) – z vorlaufu přepnuto do kotle" % (float(v["sk"].get("turb", 0.0)) * 100.0))
				phase = 1
		30:
			# kopačka: dolů → nahoru → dolů a po ~1,3 otáčky zastavit a zvednout
			var dr: float = float(v["sk"].get("rev", 0.0)) - float(base.get("rev", 0.0))
			if phase == 0 and dr >= 0.4:
				_set_kind("rakelift", false)
				phase = 1
			elif phase == 1 and dr >= 0.8:
				_set_kind("rakelift", true)
				phase = 2
			elif phase == 2 and dr >= 1.3:
				_set_kind("rake", false)
				_set_kind("rakelift", false)
				phase = 3
				_say("kopačka %.1f otáčky, zastavena a zvednutá" % dr)
		31:
			if not want.has("vyslazovani") and above_bed_cm() <= 3.0 and float(v["hlt"]["v"]) > 100.0:
				_start("vyslazovani")
		32:
			if float(rv["v"]) >= 3000.0:
				_stop("vyslazovani")
			elif float(v["hlt"]["v"]) < 100.0 and want.has("vyslazovani"):
				_stop("vyslazovani", false)
				_say("v bojleru dochází voda – vyslazování zastaveno dřív")
		33:
			var sk: Dictionary = v["sk"]
			var pores: float = float(sk["malt"]) * BrewSim.BED_L_PER_KG * 0.45
			if float(sk["v"]) <= pores + 10.0 or float(rv["v"]) >= 4400.0:
				_stop("dilo")
		34:
			var bm: float = float(rv.get("boil_s", 0.0)) / 60.0
			var hops: Array = rv.get("hops", [])
			if hops.size() == 0 and float(rv.get("boil", 0.0)) > 0.05:
				sim.add_hops(5000.0, 5.0)
				_say("chmel 5 kg (α 5 %) na začátku varu")
			elif hops.size() == 1 and bm >= 40.0:
				sim.add_hops(2000.0, 12.0)
				_say("chmel 2 kg (α 12 %) ve 40. min")
			elif hops.size() == 2 and bm >= 65.0:
				sim.add_hops(2000.0, 12.0)
				_say("chmel 2 kg (α 12 %) v 65. min")
			if bm >= 90.0:
				if bool(sim.auto_mode["rvp"]):
					_heat("rvp", 0.0)
					_say("chmelovar 90 min hotov, pára vypnuta")
				_max_kw("rvp", 350.0)
			else:
				_max_kw("rvp", BOIL_KW if float(rv["t"]) >= 98.5 else 350.0)
		36:
			if _mash_l() <= 1.0:
				_stop("varna_virivka")
		37:
			# po 20 min klidu hadice na vratku a C3 vymyje filtr a chladič – mladina se vrací do vířivky
			if phase == 0 and wp_quiet_max >= 1200.0:
				_set_kind("hose", false)
				_set_kind("vratka", true)
				_start("virivka_chladic")
				phase = 1
		39:
			var co: float = _cool_out()
			if phase == 0 and co >= 0.0 and co <= 9.5:
				_set_kind("hose", true)
				_set_kind("vratka", false)
				_say("za chladičem %.1f °C – hadice přepnuta z vratky na tank" % co)
				phase = 1
		42:
			# protláčka: jakmile je mladina z potrubí venku (z hadice teče voda), vodu zavřít, C3 a ledovku zastavit, víko zavřít
			if phase == 0 and _pushed():
				_set_kind("pushwater", false)
				_stop("virivka_chladic", false)
				_stop("ledovka", false)
				_set_kind("lid", true)
				_say("protláčka hotová (%.0f L mladiny dotlačeno), C3 a ledová voda stojí, spilka zavíčkovaná" % _simf("pushed_wort", 0.0))
				phase = 1


# ---------- kontrola ----------
func _row(ok: bool, txt: String, blocking: bool = true) -> Array:
	return [ok, txt, blocking]


## obsluha „natrhne“ vorlauf: přivře klapku, která patří jen vorlaufu (po přepnutí na dílo se s cestou zavře a mezipoloha zmizí)
func _natrhnout_vorlauf(pct: int) -> void:
	var dilo: Array = sp.route_valve_ids("dilo")
	var pick := ""
	for vid in sp.route_valve_ids("vorlauf"):
		if bool(sp.plant.call("can_throttle", plan.get_item(str(vid)))):
			if pick == "":
				pick = str(vid)
			if not dilo.has(vid):
				pick = str(vid)
				break
	if pick != "":
		sp.plant.call("set_valve_pct", pick, pct)
		_say("vorlauf natrhnut: klapka na %d %%" % pct)


func _tekla_rows(rows: Array, rn: String, secs: float) -> void:
	var t: float = float(t_flow.get(rn, 0.0))
	rows.append(_row(t >= secs, "Cesta „%s“ tekla %.0f / %.0f s" % [_lbl(rn), minf(t, secs), secs]))
	rows.append(_row(not _fl(rn), ("Cesta „%s“ je zavřená" if not _fl(rn) else "Cesta „%s“ ještě teče") % _lbl(rn)))


func _pumps_on(skip: Array, skip_machines: bool = false) -> Array:
	var res: Array = []
	for it in plan.items:
		if str(it.get("type")) != "pump" or not bool(it.get("on", false)) or skip.has(str(it.id)):
			continue
		if skip_machines and str(it.get("kind", "")) in MACHINE:
			continue
		res.append(str(it.get("label", it.id)))
	return res


## Je krok splněný? {"ok": bool, "rows": [[splněno, text, blokuje], …]} – podle návrhu
func check() -> Dictionary:
	var rows: Array = []
	if step < 0:
		return {"ok": false, "rows": rows}
	var n: Dictionary = N[step]
	var v: Dictionary = sim.vessels
	var rv: Dictionary = v["rvp"]
	var sk: Dictionary = v["sk"]
	var hl: Dictionary = v["hlt"]
	var lv: Dictionary = v["lv"]
	var wp: Dictionary = v["wp"]
	var spk: Dictionary = v["spilka"]
	var stir: bool = bool(sim.stir.get("rvp", false))
	if bool(n.get("rucne", false)):
		rows.append(_row(manual_ok, "Potvrzeno ručně" if manual_ok else "Až to bude hotové, potvrď tlačítkem „Hotovo (ručně)“"))
	if n.has("tekla"):
		var tk: Array = n["tekla"]
		_tekla_rows(rows, str(tk[0]), float(tk[1]))
	if n.has("pasmo"):
		var p: Array = n["pasmo"]
		rows.append(_row(hold >= float(p[2]), "Prodleva %.1f / %.0f min v pásmu %.0f–%.0f °C (teď %.1f °C)" % [minf(hold, float(p[2])) / 60.0, float(p[2]) / 60.0, float(p[0]), float(p[1]), float(rv["t"])]))
	if n.has("var"):
		rows.append(_row(boil >= float(n["var"]), "Var s míchadlem %.1f / %.0f min (teď %.1f °C)" % [minf(boil, float(n["var"])) / 60.0, float(n["var"]) / 60.0, float(rv["t"])]))
	if n.has("michadlo") and bool(n["michadlo"]) and step != 20:
		rows.append(_row(stir, "Míchadlo kotle běží" if stir else "Míchadlo kotle stojí – zapni ho"))
	match step:
		0:
			var opened := 0
			for it in plan.items:
				if str(it.get("type")) == "valve" and bool(it.get("open", false)):
					opened += 1
			var pon: Array = _pumps_on([])
			rows.append(_row(opened == 0, "Všechny klapky zavřené" if opened == 0 else "Otevřených klapek: %d" % opened))
			rows.append(_row(pon.is_empty(), "Všechna čerpadla stojí" if pon.is_empty() else "Běží: " + ", ".join(pon)))
		1:
			var half: float = 0.5 * float(lv["cap"])
			rows.append(_row(float(lv["v"]) >= half - 1.0, "Ledová voda %.0f / %.0f L (aspoň půl tanku)" % [float(lv["v"]), half]))
			rows.append(_row(_fl("glykol"), "Cirkulace přes glykol běží (C5)" if _fl("glykol") else "Cirkulace přes glykol neběží"))
			rows.append(_row(not _fl("lv_fill"), "Napouštění ledovky zavřené" if not _fl("lv_fill") else "Ledovka se ještě napouští"))
			rows.append(_row(true, "Ledová voda %.1f °C → cíl %.1f °C" % [float(lv["t"]), float(sim.setpoint["lv"])], false))
		2:
			var air: bool = bool(sp._kind_item("valve", "air").get("open", false))
			var vent: bool = bool(sp._kind_item("valve", "vent").get("open", false))
			rows.append(_row(air, "Tlakový vzduch otevřený" if air else "Tlakový vzduch zavřený"))
			rows.append(_row(vent, "Odvzdušnění u šoupěte otevřené" if vent else "Odvzdušnění u šoupěte zavřené"))
		3:
			var on: bool = bool(sim.auto_mode["hlt"]) and float(sim.setpoint["hlt"]) >= 75.0
			rows.append(_row(on, "Pára na bojleru: AUTO, cíl %.0f °C" % float(sim.setpoint["hlt"]) if on else "Ohřev bojleru není zapnutý na AUTO s cílem aspoň 75 °C"))
			rows.append(_row(float(hl["t"]) >= 70.0, "Bojler %.1f °C (aspoň 70 °C), %.0f L" % [float(hl["t"]), float(hl["v"])]))
		4:
			var filling: bool = _fl("voda_panev") or _fl("voda_rad")
			rows.append(_row(float(rv["v"]) >= 200.0, "V kotli %.0f L vody" % float(rv["v"])))
			rows.append(_row(float(rv["v"]) >= 200.0 and float(rv["t"]) >= 35.0 and float(rv["t"]) <= 40.0, "Teplota v kotli %.1f °C (35–40 °C)" % float(rv["t"])))
			rows.append(_row(filling, "Napouští se (bojler %s, řad %s)" % ["teče" if _fl("voda_panev") else "zavřený", "teče" if _fl("voda_rad") else "zavřený"] if filling else "Nenapouští se"))
		5:
			var filling2: bool = _fl("voda_panev") or _fl("voda_rad")
			rows.append(_row(float(rv["v"]) >= 1900.0 and float(rv["v"]) <= 2200.0, "V kotli %.0f L (asi 20 hl)" % float(rv["v"])))
			rows.append(_row(float(rv["t"]) >= 35.0 and float(rv["t"]) <= 40.5, "Teplota %.1f °C (35–40 °C)" % float(rv["t"])))
			rows.append(_row(not filling2, "Napouštění zavřené" if not filling2 else "Ještě se napouští"))
		7:
			var gate: bool = bool(sp._kind_item("valve", "gate").get("open", false))
			var sw: bool = bool(sp._kind_item("valve", "screwwater").get("open", false))
			var air2: bool = bool(sp._kind_item("valve", "air").get("open", false))
			rows.append(_row(gate, "Šoupě na slad otevřené" if gate else "Šoupě na slad zavřené"))
			rows.append(_row(sw, "Voda do šneku otevřená" if sw else "Voda do šneku zavřená"))
			rows.append(_row(air2, "Tlakový vzduch otevřený (pohon šoupěte)" if air2 else "Není vzduch – šoupě se neotevře", false))
		8:
			var runs: Array = []
			for rn in VARNA:
				if _fl(str(rn)):
					runs.append(_lbl(str(rn)))
			var cooling: Dictionary = {}
			for rn in ["glykol", "ledovka"]:
				for vid in sp.route_valve_ids(rn):
					cooling[str(vid)] = true
			var extra := 0
			for it in plan.items:
				if str(it.get("type")) == "valve" and bool(it.get("open", false)) and not (str(it.get("kind", "")) in MACHINE) and not cooling.has(str(it.id)):
					extra += 1
			var pon2: Array = _pumps_on([sim.route_pump("glykol"), sim.route_pump("ledovka")], true)
			rows.append(_row(runs.is_empty(), "Žádná cesta varny neteče" if runs.is_empty() else "Teče: " + ", ".join(runs)))
			rows.append(_row(extra == 0, "Klapky varny zavřené" if extra == 0 else "Otevřených klapek varny: %d" % extra))
			rows.append(_row(pon2.is_empty(), "C1–C4 stojí" if pon2.is_empty() else "Běží: " + ", ".join(pon2)))
		9:
			var done: bool = float(rv["malt"]) >= BrewSim.MALT_BATCH - 10.0 or sim.malt_stock <= 1.0
			rows.append(_row(done, "Slad v kotli %.0f / %.0f kg (v zásobníku %.0f kg)" % [float(rv["malt"]), BrewSim.MALT_BATCH, sim.malt_stock]))
			rows.append(_row(stir, "Slad padá do míchané vody" if stir else "Míchadlo stojí – tvoří se hrudky"))
		10:
			var gate2: bool = bool(sp._kind_item("valve", "gate").get("open", false))
			var screw: bool = bool(sp._kind_item("pump", "screw").get("on", false))
			var sw2: bool = bool(sp._kind_item("valve", "screwwater").get("open", false))
			rows.append(_row(not screw, "Šnek stojí" if not screw else "Šnek ještě běží"))
			rows.append(_row(not gate2, "Šoupě zavřené" if not gate2 else "Šoupě je otevřené – pára by šla do šneku"))
			rows.append(_row(not sw2, "Voda do šneku zavřená" if not sw2 else "Voda do šneku ještě teče"))
		13:
			rows.append(_row(float(rv["t"]) >= 54.0, "Dílo %.1f °C (cíl 55 °C)" % float(rv["t"])))
			rows.append(_row(bool(sim.auto_mode["rvp"]) and absf(float(sim.setpoint["rvp"]) - 55.0) < 0.6, "Termostat kotle drží 55 °C", false))
		14:
			var res: float = float(sk.get("res", 0.0)) + float(sk["malt"])
			rows.append(_row(float(sk["v"]) < 20.0, "SK vypuštěná" if float(sk["v"]) < 20.0 else "V SK je %.0f L vody – otevřít výpusť do kanálu" % float(sk["v"]), _has_kind("drain")))
			rows.append(_row(res <= 0.5, "Zbytky mláta vyhrnuté" if res <= 0.5 else "Na sítech je %.0f kg mláta – kopyto dolů a pustit kopačku" % res, _has_kind("hoof")))
			rows.append(_row(true, "Výpusť %s · kopyto %s · kopačka %s" % ["otevřená" if _kind_on("drain") else "zavřená", "dole" if _kind_on("hoof") else "nahoře", "se točí" if _kind_on("rake") else "stojí"], false))
		16:
			rows.append(_row(true, "V SK je %.0f L vody %.1f °C" % [float(sk["v"]), float(sk["t"])], false))
		17:
			var t18: float = float(t_flow.get("vyslazovani", 0.0))
			var turb: float = _drain_turb()
			var hd: bool = _has_kind("drain")
			rows.append(_row(t18 >= 60.0, "Sprcha tekla %.0f / 60 s" % minf(t18, 60.0)))
			rows.append(_row(turb < 0.1, "Z výpusti teče čistá voda" if turb < 0.1 else "Z výpusti teče kalná voda (zákal %.0f %%)" % (turb * 100.0), hd))
			rows.append(_row(not _fl("vyslazovani"), "Sprcha zavřená" if not _fl("vyslazovani") else "Sprcha ještě teče"))
			rows.append(_row(float(sk["v"]) < 20.0, "SK vypuštěná" if float(sk["v"]) < 20.0 else "V SK je ještě %.0f L" % float(sk["v"]), hd))
		19:
			var rh: float = float(sk.get("rake_h", 1.0))
			var down: bool = _kind_on("rakelift")
			rows.append(_row(not _kind_on("hoof"), "Kopyto nahoře" if not _kind_on("hoof") else "Kopyto je dole", _has_kind("hoof")))
			rows.append(_row(down and rh < 0.05, "Kopačka dole" if down and rh < 0.05 else ("Kopačka jede dolů (%.0f %%)" % ((1.0 - rh) * 100.0) if down else "Kopačka je nahoře – spustit dolů"), _has_kind("rakelift")))
		18:
			rows.append(_row(float(sk["v"]) >= 250.0, "Voda nad síty: v SK %.0f L (asi 250 L)" % float(sk["v"])))
			rows.append(_row(not _fl("podraz"), "Podrážení zavřené" if not _fl("podraz") else "Podrážení ještě teče"))
		20:
			var m: float = _mash_l()
			rows.append(_row(m >= 900.0 and m <= 1150.0, "V kotli zůstal 1. rmut %.0f L (asi 10 hl)" % m))
			rows.append(_row(not _fl("rmut_sk"), "Převod zastavený" if not _fl("rmut_sk") else "Převod do SK běží"))
			rows.append(_row(float(sk["malt"]) > 100.0, "Dílo v SK: %.0f L, %.1f °C" % [float(sk["v"]) + float(sk["malt"]) * BrewSim.MALT_L_PER_KG, float(sk["t"])]))
		24, 26:
			var ideal: float = float(n.get("ideal", 67.0))
			var heat_off: bool = not bool(sim.auto_mode["rvp"]) and float(sim.heater_kw["rvp"]) < 1.0
			rows.append(_row(_mash_l() <= 30.0, "Kotel prázdný" if _mash_l() <= 30.0 else "V kotli ještě %.0f L rmutu" % _mash_l()))
			rows.append(_row(not _fl("rmut_sk"), "Převod zastavený" if not _fl("rmut_sk") else "Vracení do SK běží"))
			rows.append(_row(heat_off, "Pára na kotli vypnutá" if heat_off else "Pára na kotli je zapnutá"))
			rows.append(_row(absf(float(sk["t"]) - ideal) <= 2.0, "Dílo v SK %.1f °C (ideálně %.0f °C)" % [float(sk["t"]), ideal], false))
		25:
			var m2: float = _mash_l()
			var taken: bool = m2 >= 900.0 or phase > 0
			rows.append(_row(taken and not _fl("rmut_varna"), "2. rmut v kotli %.0f L (asi 10 hl)%s" % [m2, ", odběr ze SK ještě běží" if _fl("rmut_varna") else ""]))
		28:
			rows.append(_row(quiet_max >= 1200.0, "Klid v SK %.1f / 20 min" % (minf(quiet_max, 1200.0) / 60.0)))
			rows.append(_row(bool(sk.get("bed", false)), "Filtrační lože vytvořené (%.0f cm)" % (sim.bed_height(sk) * 100.0) if bool(sk.get("bed", false)) else "Mláto se usazuje (%.0f %%)" % (float(sk.get("settle", 0.0)) * 100.0)))
			rows.append(_row(sim.iodine_negative(), "Jodová zkouška negativní" if sim.iodine_negative() else "Jodová zkouška POZITIVNÍ (zbývá %.0f %% škrobu) – lože se neusadí" % sim.starch_left(), false))
			rows.append(_row(not _kind_on("rake"), "Kopačka stojí" if not _kind_on("rake") else "Kopačka se točí – mláto se neusadí", false))
		29:
			var turb2: float = float(sk.get("turb", 1.0))
			rows.append(_row(turb2 <= 0.1, ("Sladina v průhledítku čirá (zákal %.0f %%)" if turb2 <= 0.1 else "Vorlauf: sladina se čiří, zákal %.0f %%") % (turb2 * 100.0)))
			rows.append(_row(base.has("natrh"), "Cesta natrhnutá přivřenou klapkou (odběr ze SK nejvýš %.0f %%)" % (float(n["natrhnout"]) * 100.0) if base.has("natrh") else "Klapku na vorlaufu přivřít (Ctrl + kolečko, asi 30 %) – pomalý začátek, ať se lože neutáhne"))
			rows.append(_row(not _fl("vorlauf"), "Vorlauf zastavený" if not _fl("vorlauf") else "Vorlauf běží (zpět do SK)"))
			rows.append(_row(_fl("dilo"), "Odtah sladiny do kotle teče (C2)" if _fl("dilo") else "Odtah do kotle zatím neteče"))
			rows.append(_row(sim.darcy_lpm() > 0.0, "Průtok ložem %.0f L/min" % sim.darcy_lpm(), false))
		30:
			var dr2: float = float(sk.get("rev", 0.0)) - float(base.get("rev", 0.0))
			var moved: bool = float(base.get("h_max", 1.0)) - float(base.get("h_min", 1.0)) >= 0.5
			var hr: bool = _has_kind("rake")
			rows.append(_row(dr2 >= 1.0, "Kopačka udělala %.1f otáčky (1–1,5)" % dr2, hr))
			rows.append(_row(moved, "Kopačka jela nahoru i dolů" if moved else "S kopačkou jet nahoru a dolů (zdvih)", hr))
			rows.append(_row(not _kind_on("rake"), "Kopačka stojí" if not _kind_on("rake") else "Kopačka se točí", false))
			rows.append(_row(true, "Utažení lože %.0f %% · zákal sladiny %.0f %%" % [float(sk.get("pack", 0.0)) * 100.0, float(sk.get("turb", 0.0)) * 100.0], false))
		31:
			rows.append(_row(_fl("vyslazovani"), "Sprcha (vyslazovací voda) teče" if _fl("vyslazovani") else "Sprcha zatím neteče – nad mlátem %.1f cm sladiny" % above_bed_cm()))
			rows.append(_row(_fl("dilo"), "Odtah sladiny teče" if _fl("dilo") else "Odtah sladiny neteče"))
			rows.append(_row(float(hl["t"]) >= 76.0 and float(hl["t"]) <= 80.0, "Vyslazovací voda %.1f °C (78 °C), v bojleru %.0f L" % [float(hl["t"]), float(hl["v"])], false))
		32:
			rows.append(_row(float(rv["v"]) >= 2900.0, "V kotli %.0f L (asi 30 hl)" % float(rv["v"])))
			rows.append(_row(not _fl("vyslazovani"), "Vyslazovací voda zastavená" if not _fl("vyslazovani") else "Vyslazovací voda ještě teče"))
			rows.append(_row(float(hl["v"]) >= 100.0, "V bojleru %.0f L vody" % float(hl["v"]), false))
		33:
			rows.append(_row(above_bed_cm() <= 0.5, "Nad mlátem %.1f cm sladiny" % above_bed_cm()))
			rows.append(_row(not _fl("dilo"), "Odtah zavřený" if not _fl("dilo") else "Odtah ještě teče"))
			rows.append(_row(true, "Předkvasná mladina v kotli: %.0f L, %.1f °P" % [float(rv["v"]), sim.plato("rvp")], false))
		34:
			var bm2: float = float(rv.get("boil_s", 0.0)) / 60.0
			var hops2: Array = rv.get("hops", [])
			var off: bool = not bool(sim.auto_mode["rvp"]) and float(sim.heater_kw["rvp"]) < 1.0
			rows.append(_row(bm2 >= 90.0, "Chmelovar %.0f / 90 min" % minf(bm2, 90.0)))
			rows.append(_row(hops2.size() >= 1, "Chmel přidán: %d dávky (rozpis: 3)" % hops2.size()))
			rows.append(_row(bm2 >= 90.0 and off, ("Pára vypnutá" if bm2 >= 90.0 else "Pára je vypnutá před koncem varu") if off else "Po 90 min páru vypnout"))
			rows.append(_row(not bool(rv.get("overflow", false)), "%.1f °P · %.0f IBU · odpar %.0f L/h" % [sim.plato("rvp"), sim.ibu(), float(rv.get("evap_lph", 0.0))] if not bool(rv.get("overflow", false)) else "Mladina překypěla – uber páru", false))
		36:
			var b: float = maxf(float(base.get("rvp_v", 0.0)), 1.0)
			rows.append(_row(float(wp["v"]) >= 0.9 * b, "Ve vířivce %.0f L z %.0f L" % [float(wp["v"]), b]))
			rows.append(_row(not _fl("varna_virivka"), "Přečerpávání zastavené" if not _fl("varna_virivka") else "Přečerpávání běží"))
		37:
			var has_hose: bool = _has_kind("hose")
			var lw: float = _simf("line_water", 0.0)
			rows.append(_row(wp_quiet_max >= 1200.0, "Vířivka v klidu %.0f / 20 min (kaly usazené %.0f %%)" % [minf(wp_quiet_max, 1200.0) / 60.0, float(wp.get("tset", 0.0)) * 100.0]))
			rows.append(_row(not _kind_on("hose"), "Spílací hadice na vratce" if not _kind_on("hose") else "Spílací hadice je na tanku – dát na vratku", has_hose))
			rows.append(_row(_kind_on("vratka"), "Klapka vratky otevřená" if _kind_on("vratka") else "Klapka vratky zavřená", has_hose))
			rows.append(_row(lw <= 0.5 and t_flow.has("virivka_chladic"), "Filtr a chladič vymyté" if lw <= 0.5 and t_flow.has("virivka_chladic") else "Vymývání: v potrubí ještě %.0f L vody" % lw))
			rows.append(_row(_fl("virivka_chladic"), "C3 vrací mladinu přes chladič do vířivky" if _fl("virivka_chladic") else "C3 stojí", false))
		38:
			rows.append(_row(float(lv["v"]) >= float(wp["v"]), "Ledová voda %.0f L (mladiny ve vířivce %.0f L)" % [float(lv["v"]), float(wp["v"])]))
			rows.append(_row(float(lv["t"]) <= 4.5, "Ledová voda %.1f °C (do 4 °C)" % float(lv["t"])))
			rows.append(_row(_fl("ledovka"), "Ledová voda teče do chladiče (C5)" if _fl("ledovka") else "Ledová voda do chladiče neteče"))
		39:
			var co2: float = _cool_out()
			var on_tank: bool = _kind_on("hose")
			rows.append(_row(on_tank, "Spílací hadice na tanku" if on_tank else "Hadice ještě na vratce – přepnout, až za chladičem teče ledová mladina", _has_kind("hose")))
			rows.append(_row(not _kind_on("vratka"), "Klapka vratky zavřená" if not _kind_on("vratka") else "Klapka vratky otevřená", _has_kind("hose")))
			rows.append(_row(co2 >= 0.0 and co2 <= 9.5, ("Za chladičem %.1f °C" % co2) if co2 >= 0.0 else "Chladičem neteče mladina", false))
		40:
			rows.append(_row(_fl("ledovka"), "Ledová voda teče do chladiče (C5)" if _fl("ledovka") else "Ledová voda do chladiče neteče"))
		41:
			var bw: float = maxf(float(base.get("wp_v", 0.0)), 1.0)
			rows.append(_row(float(spk["v"]) >= 0.9 * bw, "Ve spilce %.0f L z %.0f L" % [float(spk["v"]), bw]))
			rows.append(_row(float(spk["v"]) > 10.0 and float(spk["t"]) >= 7.5 and float(spk["t"]) <= 9.5, "Zakvašovací teplota %.1f °C (8,5 °C)" % float(spk["t"])))
			rows.append(_row(float(wp["v"]) <= 1.0, "Vířivka prázdná" if float(wp["v"]) <= 1.0 else "Ve vířivce ještě %.0f L" % float(wp["v"])))
		42:
			var pushed: bool = _pushed()
			var water_tank: float = _simf("push_water_tank", 0.0)
			var c3_off: bool = not _fl("virivka_chladic") and not _fl("ledovka")
			rows.append(_row(pushed, ("Mladina z potrubí dotlačena do spilky (%.0f L)" % _simf("pushed_wort", 0.0)) if pushed else ("Protláčka: v potrubí ještě %.0f L mladiny" % maxf(_line_l() - _simf("line_water", 0.0), 0.0)), _has_kind("pushwater")))
			rows.append(_row(not _kind_on("pushwater"), "Voda na protláčku zavřená" if not _kind_on("pushwater") else "Voda na protláčku teče"))
			rows.append(_row(c3_off, "C3 a ledová voda stojí" if c3_off else "C3 nebo ledová voda ještě běží"))
			rows.append(_row(_kind_on("lid"), "Spilka zavíčkovaná" if _kind_on("lid") else "Víko spilky otevřené", _has_kind("lid")))
			rows.append(_row(water_tank <= 20.0, "Ve spilce %.0f L, %.1f °P, %.1f °C; voda z protláčky %.0f L" % [float(spk["v"]), sim.plato("spilka"), float(spk["t"]), water_tank], false))
	var ok := not rows.is_empty()
	for r in rows:
		if bool(r[2]) and not bool(r[0]):
			ok = false
	return {"ok": ok, "rows": rows}
