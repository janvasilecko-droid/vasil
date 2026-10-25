extends RefCounted
const Physics = preload("res://physics.gd")
const Training = preload("res://training.gd")
const LauterAuto = preload("res://lauter_auto.gd")
const Proc = preload("res://proc.gd")





const LIQ: = ["w", "sg", "dx", "ld", "f", "ns", "aA", "aB", "aBs", "smm", "dms", "alpha", "iso", "trubSus", "col", "fines", "air"]
const SOL: = ["g", "su", "sc", "ni"]
const CELLK: = ["w", "f", "n", "smm", "dms", "col"]

const CFG_DEFAULT: = {
    "rvpCap": 5000.0, "rvpUA": 0.25, "rvpSteel": 450.0, "rvpDead": 20.0, 
    "mpCap": 6000.0, "mpUA": 0.3, "mpSteel": 600.0, "mpDead": 60.0, 
    "wpCap": 5000.0, "wpDead": 30.0, "wpUA": 0.35, "wpSteel": 550.0, 
    "skCap": 5000.0, "skArea": 5.3, "skUA": 0.2, "skSteel": 500.0, "skRetentionLkg": 1.0, "spargeEndP": 1.0, "ambientT": 20.0, "skContactUA": 4.0, "skPerm": 1.5e-11, "skSuction": 20000.0, 
    "spCap": 6000.0, 
    "waterFlow": 250.0, "maltRate": 40.0, "mainsQ": 900.0, 
    "coolWaterT": 16.0, "UA1": 25.0, "UA2": 12.0, 
    "boilerKW": 650.0, "hltCap": 12000.0, "hltStart": 11000.0, "hltT0": 83.0, "hltSet": 83.0, 
    "lvVol": 10000.0, "lvStart": 0.0, "lvT0": 2.0, "lvSet": 2.0, "chillerKW": 45.0, "pivLoad": 10.0, 
    "tbWater": 99.3, 
    "malt": 680.0, "maltExtract": 80.0, "maltMoist": 4.5, "mill": 1.0, 
    "steamBar": 2.5, "rvpUAst": 6.5, "mpUAst": 4.5, "hltUAst": 2.0, "rvpJacketL": 2200.0, "mpJacketL": 2500.0, 
    "targetEBC": 8.0, "pitchT": 8.5, 
    "targetV": 40.0, "preboilV": 4430.0, "mashFilmK": 4.6, "boilerWaterKg": 1500.0, "plateRefQ": 80.0, "betaAct": 4.5, "kdAlpha63": 0.022, "kdBeta63": 0.13, "targetP": 12.0, "targetIBU": 32.0, "boilMin": 90.0, 
    "cipLyeMin": 1.0, "cipAcidMin": 0.5, 
    "cpSteel": 0.50, "hltSteel": 600.0, "hltUA": 0.01, "skMuP": 0.047, 
    "pipeLenScale": 0.4, "bore25": 50.0, "bore40": 50.0, "bore50": 75.0, 
    "p1_H0": 32.0, "p1_Qn": 400.0, "p2_H0": 22.0, "p2_Qn": 150.0, "p3_H0": 26.0, "p3_Qn": 200.0, 
    "skDpComp": 30000.0, "skTeigRho": 350.0, "skTeigK": 0.03, "skCreepK": 0.0015, "skDpCompact": 80.0, "skCompK": 0.05, "skDpChannel": 160.0, "skChanK": 0.02, 
}
const HOPS: = [
    {"id": "zpc", "name": "Žatecký poloraný červeňák", "a": 3.5}, 
    {"id": "sla", "name": "Sládek", "a": 6.5}, 
    {"id": "pre", "name": "Premiant", "a": 8.0}, 
    {"id": "kaz", "name": "Kazbek", "a": 5.5}, 
    {"id": "agn", "name": "Agnus", "a": 10.0}, 
]
const MALTS: = {
    "plz": {"name": "Plzeňský slad", "ext": 80.5, "ebc": 3.5, "enz": 1.0, "unf": 0.0}, 
    "mni": {"name": "Mnichovský slad", "ext": 79.0, "ebc": 15.0, "enz": 0.7, "unf": 0.0}, 
    "kar": {"name": "Karamelový slad", "ext": 75.0, "ebc": 120.0, "enz": 0.0, "unf": 0.55}, 
    "plzS": {"name": "Plzeňský slad Soufflet", "ext": 81.6, "ebc": 4.0, "enz": 1.0, "unf": 0.0, "moist": 4.5}, 
    "mniW": {"name": "Mnichovský slad Weyermann typ 1", "ext": 78.0, "ebc": 15.0, "enz": 0.7, "unf": 0.0, "moist": 4.5}, 
    "cpl": {"name": "Carapils Weyermann", "ext": 75.0, "ebc": 4.5, "enz": 0.0, "unf": 0.35, "moist": 7.0}, 
    "bar": {"name": "Barvicí slad", "ext": 70.0, "ebc": 1300.0, "enz": 0.0, "unf": 0.3}, 
    "vid": {"name": "Vídeňský slad", "ext": 79.5, "ebc": 8.0, "enz": 0.9, "unf": 0.0, "moist": 4.5}, 
}
const RECIPE: = {"id": "125", "name": "12,5° Světlá (Zajíc)", "P": 12.5, "IBU": 32.0, "EBC": 9.0, 
    "grist": {"plzS": 600.0, "cpl": 75.0, "mniW": 25.0}, "water": 2000.0, "boil": 80.0, "pitch": 8.5, 
    "hops": [{"min": 0.0, "hop": "sla", "kg": 4.2}, {"min": 40.0, "hop": "zpc", "kg": 3.0}, {"min": 65.0, "hop": "zpc", "kg": 3.0}]}
const RECIPES: = [RECIPE,
    {"id": "120", "name": "12° Světlá (rozpis Vasil)", "P": 12.0, "IBU": 36.0, "EBC": 9.0,
        "grist": {"plzS": 625.0, "cpl": 60.0, "vid": 50.0}, "water": 2000.0, "boil": 90.0, "pitch": 8.5,
        "hops": [{"min": 0.0, "hop": "zpc", "kg": 1.5}, {"min": 0.0, "hop": "sla", "kg": 2.28}, {"min": 45.0, "hop": "pre", "kg": 2.0}, {"min": 75.0, "hop": "zpc", "kg": 5.0}]},
    {"id": "100", "name": "10° Výčepní", "P": 10.0, "IBU": 25.0, "EBC": 7.0,
        "grist": {"plzS": 560.0, "cpl": 30.0}, "water": 1900.0, "boil": 80.0, "pitch": 8.5,
        "hops": [{"min": 0.0, "hop": "sla", "kg": 4.2}, {"min": 60.0, "hop": "zpc", "kg": 2.0}]},
]
const TSAT: = [[1.0, 100.0], [2.0, 120.2], [3.0, 133.5], [4.0, 143.6], [5.0, 151.8], [6.0, 158.8], [8.0, 170.4]]
const PIPE_L_PER_M: = 2.0
const PX_PER_M: = 100.0

var S = {}
var net = {}
var main
var training
var lauter_auto
var deposit_on: = false
var proc
var recipe_idx: = 0
var last_route = []
var last_goal = null
var valve_ver = 0
var _cache = {}
var _cache_ver = -1


func vessel() -> Dictionary:
    var v = {"T": 18.0, "heat": 0.0, "stir": false, "hz": 35.0, "burn": 0.0, "evapKgMin": 0.0, "boiling": false, "boilMin": 0.0, 
        "coag": 0.0, "trubSet": 0.0, "trubHB": 0.0, "whirlMin": 0.0, "foam": 0.0, "turbSum": 0.0, "turbVol": 0.0, 
        "grainBoiling": false, "grainBoilMin": 0.0, "settleMin": 0.0, "Tsens": 18.0, "Twall": 18.0, "whirlNow": false, "boilStartT": 0.0, "Pin": 0.0, "vort": 0.0, "virt": false}
    for k in LIQ: v[k] = 0.0
    for k in SOL: v[k] = 0.0
    return v

func cell() -> Dictionary:
    return {"w": 0.0, "f": 0.0, "n": 0.0, "smm": 0.0, "dms": 0.0, "col": 0.0}

## Kalibrace bez zásahu do kódu: user://kalibrace.json  { "klic": hodnota  nebo  {"value": x, "source": "MEASURED", ...} }
## Přepisuje jen známé klíče z CFG_DEFAULT; vrací seznam použitých klíčů.
const CALIB_PATH: = "user://kalibrace.json"
var calib_applied: = []
## povolené rozsahy (fyzikálně smysluplné) – hodnota mimo rozsah se odmítne a zapíše do calib_rejected
const CALIB_RANGE: = {
    "rvpCap": [1000.0, 10000.0], "mpCap": [1000.0, 10000.0], "wpCap": [1000.0, 10000.0], "skCap": [1000.0, 10000.0], "spCap": [1000.0, 20000.0],
    "rvpUA": [0.0, 3.0], "mpUA": [0.0, 3.0], "wpUA": [0.0, 3.0], "skUA": [0.0, 3.0], "hltUA": [0.0, 3.0],
    "rvpSteel": [0.0, 3000.0], "mpSteel": [0.0, 3000.0], "wpSteel": [0.0, 3000.0], "skSteel": [0.0, 3000.0], "hltSteel": [0.0, 5000.0], "cpSteel": [0.4, 0.6],
    "rvpUAst": [0.5, 30.0], "mpUAst": [0.5, 30.0], "hltUAst": [0.1, 20.0], "rvpJacketL": [200.0, 6000.0], "mpJacketL": [200.0, 6000.0],
    "skArea": [1.0, 20.0], "skPerm": [1e-13, 1e-9], "skDpComp": [1000.0, 200000.0], "skSuction": [0.0, 60000.0], "skRetentionLkg": [0.5, 3.0], "preboilV": [2000.0, 5000.0], "mashFilmK": [0.0, 30.0], "boilerWaterKg": [50.0, 20000.0], "plateRefQ": [10.0, 500.0], "betaAct": [0.1, 50.0], "kdAlpha63": [0.001, 1.0], "kdBeta63": [0.005, 3.0],
    "skDpCompact": [10.0, 500.0], "skDpChannel": [20.0, 800.0], "skMuP": [0.0, 0.1],
    "UA1": [1.0, 200.0], "UA2": [1.0, 200.0], "boilerKW": [50.0, 3000.0], "steamBar": [0.2, 8.0], "chillerKW": [5.0, 500.0],
    "hltCap": [1000.0, 30000.0], "lvVol": [1000.0, 30000.0], "tbWater": [95.0, 100.5], "ambientT": [-5.0, 40.0], "coolWaterT": [1.0, 30.0],
    "waterFlow": [10.0, 600.0], "mainsQ": [100.0, 2000.0], "maltRate": [5.0, 200.0], "pipeLenScale": [0.05, 3.0], "bore25": [15.0, 120.0], "bore40": [15.0, 120.0], "bore50": [15.0, 150.0],
    "p1_H0": [3.0, 80.0], "p2_H0": [3.0, 80.0], "p3_H0": [3.0, 80.0], "p1_Qn": [20.0, 1500.0], "p2_Qn": [20.0, 1500.0], "p3_Qn": [20.0, 1500.0],
    "maltExtract": [60.0, 85.0], "maltMoist": [2.0, 10.0], "cipLyeMin": [0.3, 5.0], "cipAcidMin": [0.1, 5.0],
}
var calib_rejected: = []
func apply_calibration(c: Dictionary) -> Array:
    var used: = [];calib_rejected = []
    if not FileAccess.file_exists(CALIB_PATH): return used
    var d = JSON.parse_string(FileAccess.open(CALIB_PATH, FileAccess.READ).get_as_text())
    if not d is Dictionary:
        calib_rejected.append("soubor není platný JSON");return used
    for k in d:
        if not CFG_DEFAULT.has(k):
            calib_rejected.append("%s (neznámý parametr)" % k);continue
        var v = d[k]
        if v is Dictionary: v = v.get("value", null)
        if not (v is float or v is int):
            calib_rejected.append("%s (není číslo)" % k);continue
        var r: Array = CALIB_RANGE.get(k, [-1e30, 1e30])
        if float(v) < r[0] or float(v) > r[1] or is_nan(float(v)):
            calib_rejected.append("%s = %s (mimo rozsah %s–%s)" % [k, str(v), str(r[0]), str(r[1])]);continue
        c[k] = float(v);used.append(k)
    return used

func new_state() -> void :
    var c: Dictionary = CFG_DEFAULT.duplicate()
    var R: Dictionary = RECIPES[clampi(recipe_idx, 0, RECIPES.size() - 1)]
    c.recipe = R.name
    c.grist = R.grist.duplicate(); var mt = 0.0
    for k in c.grist: mt += c.grist[k]
    c.malt = mt;c.targetP = R.P;c.targetIBU = R.IBU;c.targetEBC = R.EBC
    c.hopPlan = R.hops.duplicate(true);c.pitchT = R.pitch;c.water = R.water;c.boilMin = R.boil
    calib_applied = apply_calibration(c)
    S = {
        "cfg": c, "t": 0.0, 
        "rvp": vessel(), "mp": vessel(), "wp": vessel(), 
        "sk": {"mash": vessel(), "cells": [cell(), cell(), cell(), cell(), cell()], "T": 18.0, "Tw": 20.0, "g": 0.0, "lost": 0.0, "c": 0.0, 
            "rake": false, "rakeH": 1.0, "dig": 0.0, "hyd": 0.0, "hydT": null, "burst": 0.0, "mode": "mash", "iodLiquid": {}, "iodGrist": {}, "vorlaufL": 0.0, "initialExtract": 0.0, "fines": 0.0, "ch": 0.0, "air": 0.0, "dpAvail": 0.0, "filt": 0.0, "settle": 0.0, "filling": false, 
            "Qact": 0.0, "Qmax": 0.0, "dp": 0.0, "outP": 0.0, "outP0": 0.0, "turb": 0.0, "emptied": false, "hatch": false, "under": 0.0, "landed": false, 
            "underOK": false, "bedMade": false, "drawn": false, "warmLog": false, "lostExt": 0.0, "teig": 0.0, "dpPa": 0.0, "vac": 0.0}, 
        "sp": {"w": 0.0, "f": 0.0, "n": 0.0, "iso": 0.0, "alpha": 0.0, "dms": 0.0, "trub": 0.0, "col": 0.0, "H": 0.0, "L": 0.0, "O2": 0.0, "Tmax": -99.0, "Tmin": 99.0}, 
        "grist": c.grist, "hopPlan": c.hopPlan, "hopIdx": 0, 
        "p1": {"dry": 0.0, "dead": 0.0, "on": false, "flow": 300.0, "status": "", "q": 0.0, "prime": 0.0, "pt": 0.0, "flowing": false}, 
        "p2": {"dry": 0.0, "dead": 0.0, "on": false, "flow": 60.0, "status": "", "q": 0.0, "prime": 0.0, "pt": 0.0, "flowing": false}, 
        "p3": {"dry": 0.0, "dead": 0.0, "on": false, "flow": 70.0, "status": "", "q": 0.0, "prime": 0.0, "pt": 0.0, "flowing": false}, 
        "rvpWater": {"on": false, "T": 38.0, "target": c.water, "added": 0.0, "k": 1.0}, 
        "malt": {"on": false, "target": c.malt, "added": 0.0, "clump": 0.0, "gate": false}, 
        "rota": {"stick": 0.0}, "skWater": {"on": false, "T": 78.0, "flow": 60.0, "total": 0.0}, 
        "cool": {"waterT": c.coolWaterT, "ice": 40.0, "Tout": null, "T1": null, "Tw": c.coolWaterT}, 
        "hlt": {"w": c.hltStart, "T": c.hltT0, "set": c.hltSet, "auto": false, "heat": 0.0, "fill": false}, 
        "lv": {"w": c.lvStart, "T": c.lvT0 if c.lvStart > 0 else c.coolWaterT, "set": c.lvSet, "chill": c.lvStart > 0, "chillOn": false, 
            "varna": true, "piv": true, "pump": c.lvStart > 0, "reached": false, "glyk": null, "glykWhy": "", "_circ": false}, 
        "cab": {"main": true, "key": true, "estop": false, "dead": false, "hs": 0.0, "lmp": true, "lsk": true, "interlock": true, "mpAuto": false, "mpHeatOn": false, "hydOn": false, "potsk": 0.5, "mpSet": 50.0, "fault": false, "heatMem": 1.0}, 
        "boiler": {"on": true, "load": 0.0, "scale": 1.0, "hltScale": 1.0, "p": 2.5}, "clog": {"rvp": 0.0, "sk": 0.0}, "rek": true, "usedHot": 0.0, "aer": true, "mixAuto": false, 
        "hops": [], "log": [], "once": {}, "hist": [], 
        "flags": {"rest52": 0.0, "decCount": 0, "dmsPre": null, "knock": null, "boilStart": null, "decStart": 0, "r63": 0.0, "r72": 0.0, 
            "r72b": 0.0, "r63b": 0.0, "inSK": 0, "under": 0, "clearOK": 0, "full": 0, "wortBoil": 0.0, "whirlDone": 0, "castStart": null, "pushed": 0}, 
        "extIn": 0.0, "pH": 5.6, "tannin": 0.0, "usedWater": 0.0, "maxSparge": 0.0, "drainL": 0.0, "drainExt": 0.0, "spillL": 0.0, "turbSum": 0.0, "turbVol": 0.0, 
        "steamOK": {"rvp": true, "mp": true, "hlt": true}, "_pd": {}, "Tsteam": 20.0, 
        "plates": {}, "plateDuty": {}, "plateFlow": {}, "lastDuty": {}, "lastPlateFlow": {}, "flows": [], 
        "_skTop": 0, "_skLow": 0, "_rvpIn": 0, "skFillNow": false, "lvFillLog": false, 
        "leaks": [], 
        "streams": {}, 
        "flush": {"on": false, "L": 0.0, "target": 300.0, "vs": [], "rate": 100.0}, 
    }
    if proc != null: proc.init_state()

func VN(k: String) -> String:
    return {"rvp": "Pánev", "mp": "MP", "wp": "Vířivá káď", "sk": "Scezovací káď", "hlt": "Bojler", "lv": "Ledovka", "spilka": "Spilka"}.get(k, k)

func logm(msg: String, type: = "info") -> void :
    S.log.push_front({"t": S.t, "msg": msg, "type": type})
    if S.log.size() > 200: S.log.pop_back()

func once(key: String, msg: String, type: = "info") -> void :
    if not S.once.has(key):
        S.once[key] = 1;logm(msg, type)

func tb(x: float) -> int: return int(floor(S.t / x))


func ext(v) -> float: return v.f + v.dx + v.ld + v.ns + v.sg
func sol(v) -> float: return v.g + v.su + v.sc + v.ni
func liqVol(v) -> float: return v.w + ext(v) * 0.62
func hotV(v) -> float: return liqVol(v) * (1.0 + 0.00052 * maxf(0.0, v.T - 20.0))
func vol(v) -> float: return liqVol(v) + sol(v) * 0.65
func cC(v) -> float: return 4.19 * v.w + 1.5 * (ext(v) + sol(v))

func plato(v) -> float:
    var e: float = ext(v); var mass: float = e + v.w
    return 100.0 * e / mass if mass > 1e-09 else 0.0
func cellVol(c) -> float: return c.w + (c.f + c.n) * 0.62
func cellP(c) -> float:
    var e: float = c.f + c.n; var mass: float = e + c.w
    return 100.0 * e / mass if mass > 1e-09 else 0.0
func stirQ(v) -> float: return minf(1.0, v.hz / 25.0) if v.stir else 0.0
func tsat(pAbs: float) -> float:
    for i in range(1, TSAT.size()):
        if pAbs <= TSAT[i][0]:
            var a = TSAT[i - 1]; var b = TSAT[i]
            return a[1] + (b[1] - a[1]) * (maxf(a[0], pAbs) - a[0]) / (b[0] - a[0])
    return 170.0


func lp() -> Dictionary:
    return {"T": 18.0, "w": 0.0, "sg": 0.0, "dx": 0.0, "ld": 0.0, "f": 0.0, "ns": 0.0, "aA": 0.0, "aB": 0.0, "aBs": 0.0, "smm": 0.0, "dms": 0.0, 
        "alpha": 0.0, "iso": 0.0, "trubSus": 0.0, "col": 0.0, "fines": 0.0, "air": 0.0, "g": 0.0, "su": 0.0, "sc": 0.0, "ni": 0.0}

func takePart(src, L: float, solidFac: float) -> Dictionary:
    var V = vol(src); var p = lp();p.T = src.T
    if V <= 1e-06: return p
    var fr = minf(1.0, L / V); var fs = minf(1.0, fr * solidFac)
    for k in LIQ:
        p[k] = src[k] * fr;src[k] -= p[k]
    for k in SOL:
        p[k] = src[k] * fs;src[k] -= p[k]
    return p

## tepelná kapacita kovu nádoby [kJ/K] = hmotnost oceli [kg] × c_p oceli [kJ/(kg·K)] – jediné místo převodu
func steelC(kg: float) -> float: return kg * S.cfg.get("cpSteel", 0.50)

## smíchání přítoku p do nádoby dst; steel = HMOTNOST oceli nádoby [kg] (ocel má teplotu náplně)
## nerozpustná sušina sladu (pluchy) pro zadané kg sypání – stejné hodnoty MALTS jako výpočet extraktu
func grain_dry_insoluble(kg: float) -> float:
    var gt: = 0.0; var out: = 0.0
    for id in S.grist: gt += S.grist[id]
    if gt <= 0.0: return kg * (1.0 - S.cfg.maltMoist / 100.0) * (1.0 - S.cfg.maltExtract / 100.0)
    for id in S.grist:
        var M = MALTS.get(id, MALTS.plz); var k: float = kg * S.grist[id] / gt
        out += k * (1.0 - M.get("moist", S.cfg.maltMoist) / 100.0) * (1.0 - M.ext / 100.0)
    return out

func addTo(dst, p, steel: float) -> void :
    var Cd = cC(dst) + steelC(steel); var Cp = cC(p)
    if Cd + Cp > 0: dst.T = (Cd * dst.T + Cp * p.T) / (Cd + Cp)
    for k in LIQ: dst[k] += p[k]
    for k in SOL: dst[k] += p[k]

func takeCell(c, L: float) -> Dictionary:
    var V = cellVol(c); var p = cell()
    if V <= 1e-09: return p
    var fr = minf(1.0, L / V)
    for k in CELLK:
        p[k] = c[k] * fr;c[k] -= p[k]
    return p

func mergeCell(c, p) -> void :
    for k in CELLK: c[k] += p[k]

func contentKind(c) -> String:
    var V = liqVol(c) + sol(c) * 0.65
    if V < 0.05: return "empty"
    if sol(c) + c.sg > 0.02 * V: return "mash"
    if ext(c) > 0.005 * V: return "wort"
    if c.T > 45: return "hot"
    if c.T < 6: return "ice"
    return "water"



func iodine(v, include_grist: = false):
    var st: float = v.sg
    if include_grist: st += v.su + v.sc
    var tot: float = st + v.dx + v.ld + v.f
    if tot < 1.0: return null
    var r = st / tot
    return "neg" if r < 0.01 else ("weak" if r < 0.04 else "pos")


func iodine_sample(source: String, grist: bool):
    if source == "rvp":
        return S.rvp if vol(S.rvp) > 30.0 else null
    if source == "sk":
        if S.sk.mode == "mash" and vol(S.sk.mash) > 30.0: return S.sk.mash
        var d = S.sk.iodGrist if grist else S.sk.iodLiquid
        if S.sk.g > 1.0 and d.size() > 0: return d
    return null


func iodine_info(v, include_grist: = false) -> Dictionary:
    var starch: float = v.sg + (v.su + v.sc if include_grist else 0.0)
    var total: float = starch + v.dx + v.ld + v.f + (0.0 if not include_grist else 0.0)
    if v.w < 1.0 and total < 0.5: return {"empty": true}
    total = maxf(total, 0.5)
    var s: float = starch / total
    var d: float = v.dx / total
    var ws: float = clampf(s * 14.0, 0.0, 1.0)
    var wd: float = clampf(d * 3.0, 0.0, 0.85) * (1.0 - ws)
    var col: Color = Color(0.8, 0.52, 0.12).lerp(Color(0.62, 0.2, 0.14), wd).lerp(Color(0.08, 0.05, 0.22), ws)
    var name: String
    if ws > 0.75: name = "modročerná – hodně nezcukřeného škrobu"
    elif ws > 0.4: name = "tmavě modrá – škrob (nezcukřeno)"
    elif ws > 0.15: name = "fialová – škrob a velké dextriny"
    elif ws > 0.04: name = "červenofialová – dextriny, skoro zcukřeno"
    elif wd > 0.4: name = "červenohnědá – erythrodextriny (zcukřeno částečně)"
    elif wd > 0.12: name = "oranžovohnědá – stopy dextrinů"
    else: name = "žlutohnědá (barva jódu) – ve vzorku není škrob ani dextriny (negativní)"
    return {"empty": false, "color": col, "name": name, "starch": s, "dextrin": d, "grist": include_grist}

func skMalt() -> float: return S.sk.g / 0.191
func skBedH() -> float: return skMalt() * 0.0027 / S.cfg.skArea

func skRet() -> float: return skMalt() * S.cfg.skRetentionLkg
func skC() -> float:
    var C: float = 1.4 * S.sk.g
    for c in S.sk.cells: C += 4.19 * c.w + 1.5 * (c.f + c.n)
    return C
func skHomog() -> void :
    var sk = S.sk; var tot = cell()
    for c in sk.cells: mergeCell(tot, c)
    var V = cellVol(tot); var cap = skRet() / 4.0; var rem = V
    var vols = [0.0, 0.0, 0.0, 0.0, 0.0]
    for i in range(4, 0, -1):
        vols[i] = minf(cap, rem);rem -= vols[i]
    vols[0] = rem
    for i in 5:
        var fr: float = vols[i] / V if V > 0 else 0.0
        for k in CELLK: sk.cells[i][k] = tot[k] * fr
func skAddMash(p) -> void :
    var sk = S.sk; var C0 = skC()
    var liqC: float = 4.19 * p.w + 1.5 * (ext(p) + sol(p))
    sk.T = (C0 * sk.T + liqC * p.T) / (C0 + liqC)
    sk.g += p.g;sk.lost += p.su + p.sc
    var top = sk.cells[0]
    top.w += p.w;top.f += p.f;top.n += p.sg + p.dx + p.ld + p.ns + p.ni;top.smm += p.smm;top.dms += p.dms;top.col += p.col
    skHomog()
func skVolTotal() -> float:
    var V = 0.0
    for c in S.sk.cells: V += cellVol(c)
    return V + skMalt() * 0.65 + vol(S.sk.mash)
## otáčky kopačky: stupeň 1–10 na potenciometru RYCHLOST SK; střed (5,5) = 1,5 ot/min (ODHAD, lineární závislost na stupni)
func rkRpm() -> float: return 1.5 * (1.0 + 9.0 * S.cab.get("potsk", 0.5)) / 5.5

func skViscosity(T: float, P: float) -> float:
    return 2.414e-05 * pow(10.0, 247.8 / (T + 133.15)) * exp(S.cfg.get("skMuP", 0.047) * P)


func skPerm() -> float:
    var perm: float = [0.6, 1.0, 1.5][clampi(roundi(S.cfg.mill) - 1, 0, 2)]
    return S.cfg.skPerm * perm / (1.0 + 2.5 * S.sk.c)


## hydraulický odpor lože [Pa·s/m³]: mláto (Darcy, stlačitelný koláč) + jemná horní vrstva „těsto“ (Oberteig)
## + kanálky obtékají lože. Stlačitelnost: propustnost klesá s tlakovým rozdílem na loži.
func skRes(mu: float) -> float:
    var sk = S.sk; var c = S.cfg
    var k_bed: float = skPerm() / (1.0 + maxf(0.0, sk.get("dpPa", 0.0)) / c.skDpComp)
    var L: float = maxf(0.05, skBedH())
    var Lt: float = sk.get("teig", 0.0) / (c.skArea * c.skTeigRho)
    var kt: float = c.skPerm * c.skTeigK
    return mu * (L / maxf(1e-16, k_bed) + Lt / kt) / pow(1.0 - 0.62 * sk.get("dep", 0.0), 2) / (c.skArea * (1.0 + 1.5 * sk.ch))

func skQmax() -> void :
    var sk = S.sk; var c = S.cfg
    var outP = cellP(sk.cells[4] if sk.cells[4].w > 1 else sk.cells[0])
    var mu: float = skViscosity(sk.T, outP)
    var h_over: float = cellVol(sk.cells[0]) / 1000.0 / c.skArea
    # hnací tlak = hladina nad mlátem + podtlak, který čerpadlo pod sítem ještě vytvoří
    var dpa: float = 9810.0 * h_over + c.skSuction
    sk.dpAvail = dpa
    var q: float = 60000.0 * dpa / skRes(mu)
    sk.Qmax = 400.0 if sk.g < 1 else q
    sk.outP0 = outP

func skThermalStep(seconds: float) -> void :
    var sk = S.sk; var c = S.cfg
    var capacities = [skC(), cC(sk.mash)]
    var total: float = capacities[0] + capacities[1]
    if total > 1e-09:
        for i in 2:
            if capacities[i] <= 1e-09: continue
            var temperature: float = sk.T if i == 0 else sk.mash.T
            var nx = Physics.exchange_bodies(temperature, capacities[i], sk.Tw, steelC(c.skSteel), c.skContactUA * capacities[i] / total, seconds)
            if i == 0: sk.T = nx[0]
            else: sk.mash.T = nx[0]
            sk.Tw = nx[1]
    sk.Tw = c.ambientT + (sk.Tw - c.ambientT) * exp( - c.skUA * seconds / steelC(c.skSteel))
    if skC() > 1.0: sk.T -= c.skUA * 0.3 * (sk.T - c.ambientT) * seconds / skC()
func skMashToBed() -> void :
    var sk = S.sk
    if vol(sk.mash) < 1: return
    var first = not sk.bedMade
    if first:
        sk.bedMade = true;sk["dep"] = 1.0 if deposit_on else 0.0   # nános jemných částic na dně pod síty – nutno prokopnout trhnutím klapky
        if not sk.underOK: sk.c += 0.18
    var cells = 0.0
    for x in sk.cells: cells += cellVol(x)
    if sk.g < 1 and cells < 5: sk.T = sk.mash.T
    var p = sk.mash;sk.mash = vessel()
    var tl_: float = p.sg + p.dx + p.ld + p.f
    sk.iodLiquid = {"sg": p.sg, "su": 0.0, "sc": 0.0, "dx": p.dx, "ld": p.ld, "f": p.f}
    sk.iodGrist = {"sg": p.sg, "su": p.su, "sc": p.sc, "dx": p.dx, "ld": p.ld, "f": p.f}
    skAddMash(p)
    if first:

        var fl: float = minf(sk.g * 0.2, 5.5 if sk.underOK else 9.0);sk.g -= fl;sk.fines += fl
        # při usazování dosedají nejjemnější částice nakonec – horní „těsto“ (asi 1 % sypání)
        var tg0: float = minf(sk.g * 0.1, 0.010 * skMalt());sk.g -= tg0;sk.teig = sk.get("teig", 0.0) + tg0
func skDraw(dem: float, m: float) -> Dictionary:
    skMashToBed()
    var sk = S.sk; var Q = minf(dem, sk.Qmax);sk.Qact += Q;sk.drawn = true
    var ratio: float = dem / maxf(1.0, sk.Qmax)
    var qs: float = sk.get("qSlow", 0.0)
    var surge: float = Q - qs
    var dts: float = m * 60.0
    sk["qSlow"] = qs + (Q - qs) * minf(1.0, dts / 8.0)
    sk["tearT"] = maxf(0.0, sk.get("tearT", 0.0) - dts)
    if sk.g > 1:
        # tlakový rozdíl na loži při skutečném průtoku (Darcy) – podle něj se mláto slehává
        var mu0: float = skViscosity(sk.T, sk.outP0)
        var dp_pa: float = Q / 60000.0 * skRes(mu0)
        sk.dpPa = dp_pa
        var dpm: float = dp_pa / 100.0
        var c = S.cfg
        var d1: float = c.skDpCompact; var d2: float = c.skDpChannel
        sk.c = minf(4.0, sk.c + m * c.skCreepK * dpm / d1)                       # pomalé sedání lože
        if dpm > d1: sk.c = minf(4.0, sk.c + m * c.skCompK * pow((dpm - d1) / d1, 2))   # stlačení nad prahem (výchozí 80 mbar, ODHAD)
        if dpm > d2: sk.ch = minf(0.5, sk.ch + c.skChanK * m * (dpm - d2) / d1)       # praskliny / kanálky (výchozí 160 mbar, ODHAD)
        sk.vac = clampf((dem - sk.Qmax) / maxf(1.0, sk.Qmax), 0.0, 1.0)
        if ratio > 1.05: once("stuckflow%d" % tb(900), "Čerpadlo táhne víc, než projde ložem – pod sítem vzniká podtlak (Δp %d mbar), mláto se utahuje. Zpomal C2 nebo nakypři." % roundi(dpm), "warn")
        elif dpm > 120.0: once("dphigh%d" % tb(900), "Tlakový rozdíl na loži %d mbar – lože se slehává. Zpomal odběr a mělce nakypři (kopačka na střed)." % roundi(dpm), "warn")
        # jemné částice se při filtraci ukládají nahoře (Oberteig) – berou se z jemné frakce mláta
        var tg: float = minf(sk.g * 0.05, 0.0012 * Q * m)
        sk.g -= tg;sk.teig = sk.get("teig", 0.0) + tg
    # TRHNUTÍ KLAPKOU – setrvačný ráz kapaliny v potrubí (Δp = ρ·L·Δv/Δt, Joukowského/inertní tlak):
    # skok průtoku ΔQ v potrubí DN50 délky L (ODHAD 12 m) za dobu otevření t (≈ krok simulace, 0,5–3 s).
    # Ráz utrhne nános na dně (jemné částice zalepené ve štěrbinách sít); pokud ráz + ustálený Δp přesáhne práh kanálků
    # stlačení lože (skDpCompact, ODHAD 80 mbar), trhá se samo lože.
    var dep: float = sk.get("dep", 0.0)
    if sk.g > 1 and surge > 3.0 and Q > 8.0 and sk.tearT <= 0.0:
        var dv: float = surge / 60000.0 / 0.00196
        var dpt: float = 1000.0 * 12.0 * dv / clampf(dts, 0.5, 3.0) / 100.0       # mbar
        if dpt > 10.0:
            sk.tearT = 10.0
            if dep > 0.02:
                sk["dep"] = maxf(0.0, dep - 0.55 * clampf((dpt - 10.0) / 30.0, 0.2, 1.5));sk.fines += 0.4 * clampf(dpt / 40.0, 0.3, 1.5)
                logm("Ráz klapkou (+%d mbar na sítech) utrhl část nánosu ze dna%s." % [roundi(dpt), (" – zbývá asi %d %%, trhni ještě" % roundi(sk.dep * 100.0)) if sk.dep > 0.05 else " – síta jsou volná, můžeš scezovat"], "info")
            elif sk.dpPa / 100.0 + dpt > S.cfg.skDpCompact:
                var st: float = clampf((sk.dpPa / 100.0 + dpt - S.cfg.skDpCompact) / S.cfg.skDpCompact + 0.3, 0.3, 1.5)
                sk.ch = minf(0.5, sk.ch + 0.15 * st);sk.c = minf(4.0, sk.c + 0.3 * st)
                var tr: float = minf(sk.g * 0.012 * st, 6.0);sk.g -= tr;sk.fines += tr
                sk.air = minf(1.0, sk.air + 0.1 * st)
                logm("TRHNUTÍ! Rychlé otevření klapky dalo ráz +%d mbar (celkem %d mbar nad prahem stlačení %d) – lože se utrhlo, vznikly kanálky a zákal. Klapku otevírej pomalu." % [roundi(dpt), roundi(sk.dpPa / 100.0 + dpt), roundi(S.cfg.skDpCompact)], "bad")
    var q = Q * m; var out: Dictionary
    if sk.g < 1: out = takeCell(sk.cells[0], q)
    else:

        out = takeCell(sk.cells[4], q * (1.0 - sk.ch))
        if sk.ch > 0.001: mergeCell(out, takeCell(sk.cells[0], q * sk.ch))
        var cap = skRet() / 4.0
        for i in range(4, 0, -1):
            var deficit = cap - cellVol(sk.cells[i])
            if deficit > 0: mergeCell(sk.cells[i], takeCell(sk.cells[i - 1], deficit))
        if cellVol(sk.cells[0]) < 3 and Q > 0:
            sk.c += 0.08 * m;sk.ch = minf(0.5, sk.ch + 0.05 * m);sk.air = minf(1.0, sk.air + 0.4 * m)
            once("drybed", "Hladina klesla pod mláto – do lože se tahá vzduch, lože se slehává a vznikají kanálky!", "bad")
    var Lout = cellVol(out)

    var fo: float = sk.fines * (1.0 - exp( - Lout / 250.0))
    sk.fines -= fo
    sk.turb = 3.0 + 25.0 * (sk.fines + fo * 0.5) * ((1.0 + (10.0 - sk.settle) / 10.0) if sk.settle < 10 else 1.0)
    var op = cellP(out);sk.outP = op if op > 0 else sk.outP0
    sk.filt += Lout

    var mu: float = skViscosity(sk.T, sk.outP)
    var dpn: float = Q / 60000.0 * skRes(mu)
    sk.dp = maxf(5.0, dpn / 100.0)
    var p = lp();p.T = sk.T;p.w = out.w;p.f = out.f;p.ns = out.n;p.smm = out.smm;p.dms = out.dms;p.col = out.col;p.fines = fo;p.air = sk.air * Lout * 0.02
    return p

func sanSpray(id) -> bool:
    if id == null or id == "": return false
    var sp = net.byId.get(id)
    if sp == null or sp.type != "spray": return false
    var sks = items_by_bind("sk")
    if not sks.size(): return false
    var v = sks[0]

    return sp.x >= v.x - 40 and sp.x <= v.x + v.w + 40 and absf(sp.x - (v.x + v.w / 2.0)) > v.w * 0.2
func skWallHeat(part) -> void :
    var L = liqVol(part)
    if L <= 1e-06: return
    var sk = S.sk; var Tw: float = sk.Tw; var C: float = steelC(S.cfg.skSteel)

    var Cw: float = cC(part)
    var Q = 0.6 * Cw * C / (Cw + C) * (part.T - Tw)
    sk.Tw = Tw + Q / C;part.T -= Q / Cw
    if sk.Tw >= 60 and not sk.warmLog:
        sk.warmLog = true;logm("Scezovací káď prohřátá – stěny %s °C. Rmut se v ní tolik neochladí." % fmt0(sk.Tw))
func skAddWater(L: float, T: float, low: = false) -> void :
    var sk = S.sk
    if skVolTotal() < 5 and vol(sk.mash) <= 5: sk.T = sk.Tw
    if low and sk.g < 1 and vol(sk.mash) <= 20:
        var C0 = skC(); var Cw = 4.19 * L
        sk.T = (C0 * sk.T + Cw * T) / (C0 + Cw);sk.cells[4].w += L;sk.under += L
        if sk.under >= 120: once("underok", "Podsítí zaplaveno vodou – síta jsou pod vodou, rmut dosedne měkce.")
        return
    if vol(sk.mash) > 20:
        var p = lp();p.w = L;p.T = T;addTo(sk.mash, p, 0.0);return
    var C1 = skC(); var Cw1 = 4.19 * L
    sk.T = (C1 * sk.T + Cw1 * T) / (C1 + Cw1);sk.cells[0].w += L


func react(v, m: float) -> void :
    if v.w < 1: return
    var T: float = v.T; var Vm3 = maxf(0.05, liqVol(v) / 1000.0)

    if v.ni > 0:
        var d: float = v.ni * minf(1.0, 0.08 * m * (0.5 + T / 60.0));v.ni -= d;v.f += 0.5 * d;v.ns += 0.5 * d

    if v.su > 0:
        var d2: float = v.su * minf(1.0, 0.3 * m / (1.0 + exp( - (T - 62.0) / 1.3)));v.su -= d2;v.sg += d2
    if v.sc > 0:
        var k = 0.08 if T >= 95 else (0.004 if T > 70 else 0.0)
        var d3: float = v.sc * minf(1.0, k * m);v.sc -= d3;v.sg += d3

    var phf: Vector2 = S.get("phF", Vector2.ONE)
    if v.aA > 0.01 and v.sg > 0:
        var r: float = 0.0015 * (v.aA / Vm3) * exp( - pow((T - 72.0) / 9.0, 2)) * m * phf.x
        var d4: float = v.sg * minf(1.0, r)
        var ldf: float = 0.1 + 0.2 * clampf((T - 60.0) / 15.0, 0.0, 1.0)
        v.sg -= d4;v.dx += (1.0 - ldf) * d4;v.ld += ldf * d4

    var aBt: float = v.aB + v.aBs
    if aBt > 0.01:
        var r2: float = 0.0016 * S.cfg.get("betaAct", 4.5) * (aBt / Vm3) * exp( - pow((T - 63.0) / 7.0, 2)) * m * phf.y
        var d5: float = v.dx * minf(1.0, r2); var d6: float = v.sg * minf(1.0, 0.25 * r2)
        v.dx -= d5;v.sg -= d6;v.f += d5 + d6

    # tepelná inaktivace amyláz – Arrhenius podle Marc et al. 1983 (konstanty fitované na rmutování):
    #   α: k(63 °C) = 0,022 /min, Ea 83,7 kJ/mol (poločas 32 min při 63 °C, 15 min při 72 °C)
    #   β: k(63 °C) = 0,13 /min,  Ea 188 kJ/mol (poločas 5 min při 63 °C, < 1 min při 72 °C)
    # β-amyláza ve dvou podílech (De Schepper et al. 2022): 87 % labilní, 13 % stabilní (v rmutu chráněná) – stabilní 40× pomalejší
    var iT: float = 1.0 / 336.15 - 1.0 / (T + 273.15)
    var kA: float = S.cfg.get("kdAlpha63", 0.022) * exp(10067.0 * iT)
    var kB: float = S.cfg.get("kdBeta63", 0.13) * exp(22612.0 * iT)
    v.aA *= exp(-minf(50.0, kA * m))
    v.aB *= exp(-minf(50.0, kB * m))
    v.aBs *= exp(-minf(50.0, kB / 40.0 * m))


    # SMM → DMS (Dickenson 1979, J. Inst. Brew. 85:235): k(100 °C) = 0,01824 /min při pH 5,2 a 0,0213 /min při pH 5,5,
    # poločas se zdvojnásobí na každých 6 °C méně (Ea ≈ 130 kJ/mol)
    var ph_d: float = clampf(float(S.get("pH", 5.5)), 5.0, 5.8)
    var k100: float = 0.01824 + (0.0213 - 0.01824) * (ph_d - 5.2) / 0.3
    var ksmm = maxf(0.0, k100) * pow(2.0, (T - 100.0) / 6.0) if T > 60 else 0.0
    var kd = 0.0
    if v.boiling: kd = v.evapKgMin / maxf(1.0, v.w) * 70.0
    elif T > 80: kd = 0.003
    if v.smm > 0 or v.dms > 0:
        var dn = Physics.chain(v.smm, v.dms, ksmm, kd, m)
        v.smm = dn[0];v.dms = dn[1]

    if v.alpha > 0 or v.iso > 0:
        var TK = T + 273.15
        var k1 = 790000000000.0 * exp(-11858.0 / TK); var k2 = 4100000000000.0 * exp(-12994.0 / TK)
        var hn = Physics.chain(v.alpha, v.iso, k1, k2, m)
        v.alpha = hn[0];v.iso = hn[1]




const HA_REF: = 25.0

func thick_factor(v) -> float:
    return clampf((sol(v) + v.sg) / maxf(1.0, vol(v)) * 6.0, 0.0, 1.0)

func film_hA(v) -> float:
    var hz: float = v.hz if v.stir else 4.0
    # hustý rmut předává teplo mnohem hůř než voda/mladina (viskózní film u stěny); kalibrace: plná pánev rmutu ~1 K/min (Weihenstephan)
    var visc: float = 1.0 / (1.0 + S.cfg.get("mashFilmK", 4.6) * thick_factor(v))
    var hA: float = HA_REF * pow(maxf(hz, 4.0) / 35.0, 0.67) * visc


    if v.get("boiling", false): hA = maxf(hA, 2.0 * HA_REF * visc)
    return hA


func film_factor(v) -> float:
    var ua_ref: float = S.cfg.rvpUAst
    var gs: float = 1.0 / maxf(1e-06, 1.0 / ua_ref - 1.0 / HA_REF)
    var ua: float = 1.0 / (1.0 / gs + 1.0 / film_hA(v))
    return ua / ua_ref


func density(v) -> float:
    var liq_mass: float = v.w + ext(v)
    var P: float = 100.0 * ext(v) / maxf(1e-09, liq_mass)
    var sg: float = 1.0 + P / (258.6 - 0.88 * P)
    var exp_f: float = 1.0 + 0.00052 * maxf(0.0, v.T - 20.0)
    var sol_mass: float = v.g + v.su + v.sc + v.ni
    var volume: float = liq_mass / (sg * 0.9982) * exp_f + sol_mass * 0.65 + v.sg * 0.0
    return (liq_mass + sol_mass) / maxf(1e-09, volume)
func heat(v, m: float, dt: float, name: String) -> void :
    var c = S.cfg; var pdv = S._pd.get(name, {"P": 0.0, "wet": 1.0})
    var UA: float = c.rvpUA if name == "rvp" else c.mpUA
    var steel: float = c.rvpSteel if name == "rvp" else c.mpSteel
    var Pin: float = pdv.P * S.boiler.scale * (1.0 if S.steamOK[name] else 0.0)


    var thick = thick_factor(v)
    var hA: float = film_hA(v)
    var ff: float = film_factor(v)
    Pin *= ff
    v.Twall = v.T + Pin / maxf(0.5, hA)
    if v.heat > 0 and pdv.wet <= 0 and S.boiler.on: once("dry%s%d" % [name, tb(600)], "%s: parní plášť není zaplavený – ohřev neběží, napusť víc." % VN(name), "warn")
    v.Pin = Pin

    if Pin > 0 and sol(v) + v.sg > 40 and v.Twall > 100.0:
        v.burn += m * clampf((v.Twall - 100.0) / 8.0, 0.0, 2.0)
        if v.burn > 1.5: once("burn" + name, "%s: rmut se připaluje u stěny (%s °C u pláště)! Zapni míchadlo nebo přidej otáčky, ubírej páru." % [VN(name), fmt0(v.Twall)], "bad")
        Pin *= 0.9
    if v.heat > 0 and not S.boiler.on: once("noboil%d" % tb(900), "Kotel je vypnutý – pára nejde.", "warn")
    var Tb: float = c.tbWater + 0.03 * plato(v)
    var C: float = cC(v) + steelC(steel)
    var netP: float = Pin - UA * (v.T - c.ambientT)
    var T: float = v.T + netP * dt / C
    v.evapKgMin = 0.0
    var wasBoil: bool = v.boiling
    if T >= Tb - 0.02 and netP > 0:
        var E: float = netP * dt - maxf(0.0, Tb - v.T) * C
        T = Tb
        if E > 0:
            var ev: float = minf(v.w, E / 2257.0);v.w -= ev;v.evapKgMin = ev / m;S.evapTot = S.get("evapTot", 0.0) + ev
        v.boiling = true;v.boilMin += m
    else: v.boiling = T >= Tb - 0.3 and Pin > 0
    if not wasBoil and v.boiling: v.boilStartT = S.t
    v.T = T

    if vol(v) < 5.0: v.Tsens = T
    else: v.Tsens += (T + (S.get("sensOff", 0.0) if name == "rvp" else 0.0) - v.Tsens) * (1.0 - exp( - dt / (20.0 + 70.0 * (1.0 - stirQ(v)))))


func drawHot(L: float) -> float:
    var h = S.hlt; var usable = maxf(0.0, h.w - 150.0); var hot = minf(L, usable)
    if hot < L: once("hltempty%d" % tb(1200), "Bojler je prázdný – horká voda neteče!", "bad")
    h.w -= hot;S.usedHot += hot
    return h.T if hot > 0.01 else S.cool.waterT
func drawWater(L: float, Tset: float) -> float:
    var cold: float = S.cool.waterT; var h = S.hlt; var usable = maxf(0.0, h.w - 150.0)
    var hot = 0.0; var T = cold
    if Tset > cold + 0.5:
        if h.T > Tset: hot = L * (Tset - cold) / (h.T - cold)
        else:
            hot = L
            if h.T < Tset - 1: once("hltcold%d" % tb(1200), "Horká voda má jen %s °C – požadovaných %s °C nedáš." % [fmt0(h.T), fmt0(Tset)], "warn")
        if hot > usable:
            hot = usable;once("hltempty%d" % tb(1200), "Tank horké vody je prázdný – teče jen studená voda!", "bad")
        T = (hot * h.T + (L - hot) * cold) / L if L > 0 else cold
    h.w -= hot;S.usedHot += hot
    return T


func srcTake(key: String, Lmin: float, m: float, pump: = "") -> Variant:
    var c = S.cfg; var L = Lmin * m
    if key == "rvp" or key == "mp":
        var v = S[key]; var dead: float = c.rvpDead if key == "rvp" else c.mpDead + v.trubSet * 6.0
        var LL = minf(L, maxf(0.0, vol(v) - dead))
        if LL <= 0.01: return null
        return takePart(v, LL, 1.0 if v.stir else 1.5)
    if key == "wp":
        var w = S.wp; var LL2 = minf(L, maxf(0.0, vol(w) - c.wpDead - w.trubSet * 6.0))
        if LL2 <= 0.01: return null
        return takePart(w, LL2, 1.0)
    if key == "sk" and vol(S.sk.mash) > 5 and pump != "p2":
        var LL3 = minf(L, vol(S.sk.mash) - 5.0)
        if LL3 <= 0.01: return null
        return takePart(S.sk.mash, LL3, 1.0)
    if key == "sk":
        if pump == "p2" or pump == "": skMashToBed()
        var tot = 0.0
        for x in S.sk.cells: tot += cellVol(x)
        if tot < 0.5: return null
        return skDraw(Lmin, m)
    if key == "hlt":
        var h = S.hlt; var LL4 = minf(L, maxf(0.0, h.w - 150.0))
        if LL4 <= 0.01: return null
        h.w -= LL4; var p = lp();p.w = LL4;p.T = h.T;return p
    if key == "spilka":
        var sp = S.sp
        if sp.L < 5: return null
        var LL5 = minf(L, sp.L); var fr: float = LL5 / sp.L; var q = lp()
        q.w = sp.w * fr;q.f = sp.f * fr;q.ns = sp.n * fr;q.iso = sp.iso * fr;q.dms = sp.dms * fr;q.col = sp.col * fr;q.T = sp.H / sp.L
        for k in ["w", "f", "n", "iso", "alpha", "dms", "trub", "col", "H", "L", "O2"]: sp[k] *= (1.0 - fr)
        return q
    if key == "lv":
        if S.lv.w < 300: return null
        var p2 = lp();p2.w = L;p2.T = S.lv.T;p2.virt = true;return p2
    return null

func spAdd(p) -> void :
    var sp = S.sp; var Tout: float = p.T;S.cool.Tout = Tout
    sp.w += p.w;sp.f += p.f;sp.n += p.sg + p.dx + p.ld + p.ns;sp.iso += p.iso;sp.alpha += p.alpha;sp.dms += p.dms;sp.trub += p.trubSus;sp.col += p.col
    var Lp = liqVol(p);sp.H += Lp * Tout;sp.L += Lp;sp.O2 += Lp * (8.0 if S.aer else 0.5)
    if sp.L > S.cfg.spCap:
        var o: float = sp.L - S.cfg.spCap; var fr: float = o / sp.L
        S.drainExt += (sp.f + sp.n) * fr
        for k in ["w", "f", "n", "iso", "alpha", "dms", "trub", "col", "H", "L", "O2"]: sp[k] *= (1.0 - fr)
        S.drainL += o;once("spover%d" % tb(600), "Spilka přetéká!", "bad")
    if ext(p) > 0.01:
        sp.Tmax = maxf(sp.Tmax, Tout);sp.Tmin = minf(sp.Tmin, Tout)
        if Tout > 14: once("hotcast%d" % tb(600), "Mladina jde do spilky při %s °C – pro spodní kvasnice moc teplá. Přidej ledovou vodu nebo zpomal." % fmt1(Tout), "warn")
        if Tout < 4: once("coldcast%d" % tb(600), "Mladina jde do spilky při %s °C – příliš studená, uber ledovou." % fmt1(Tout), "warn")
        if not S.once.has("castStart"):
            S.once.castStart = 1;S.flags.castStart = S.t;logm("Do spilky teče mladina, %s °C." % fmt1(Tout))
    elif p.w > 0.5 and sp.L > 100: once("spwater%d" % tb(300), "Do spilky teče voda.")

func dstGive(key: String, p, ctx: = {}) -> void :
    var c = S.cfg; var sk = S.sk; var mp = S.mp
    var Lp = liqVol(p)
    if Lp <= 1e-06 and sol(p) <= 1e-06: return
    if key == "rvp" or key == "mp":
        addTo(S[key], p, c.rvpSteel if key == "rvp" else c.mpSteel)
        S[key].trubSus += S[key].fines;S[key].fines = 0.0;S[key].air = 0.0
        if key == "mp" and ctx.get("src") == "mp":
            mp.whirlNow = true;mp.whirlMin += ctx.get("m", 0.0);mp.settleMin = 0.0
        if ctx.get("src") == "sk":
            var v = S[key]; var tu: float = ctx.get("turb", 0.0)
            v.turbSum += tu * Lp;v.turbVol += Lp;S.turbSum += tu * Lp;S.turbVol += Lp
            if sk.outP < 0.8 and sk.g > 1 and v.turbVol > 2000: once("lastrun", "Poslední podíl pod 0,8 °P – dál už jen ředíš a vyluhuješ třísloviny.", "warn")
        return
    if key == "wp":
        if S.flags.knock == null and ext(p) > 0.002 and ext(S.rvp) > 50:
            var r = S.rvp; var e = ext(r) + ext(p); var w: float = r.w + p.w
            S.flags.knock = {"V": hotV(r) + hotV(p), "P": 100.0 * e / maxf(1.0, e + w), "e": e, "t": S.t}
        addTo(S.wp, p, c.wpSteel);S.wp.whirlNow = true;S.wp.whirlMin += ctx.get("m", 0.0);S.wp.settleMin = 0.0
        return
    if key == "sk":
        if sk.mode == "lauter" and ctx.get("src", "") == "sk": sk.vorlaufL += Lp
        if p.fines > 0:
            # vrácený zákal se zachytí na povrchu lože (zvětší horní vrstvu), zbytek do mláta / ve vznosu
            sk.teig = sk.get("teig", 0.0) + p.fines * 0.5;sk.g += p.fines * 0.4;sk.fines += p.fines * 0.1;p.fines = 0.0
        if p.g + p.su + p.sc + p.ni > 0.01:
            if not sk.landed:
                sk.landed = true;sk.underOK = sk.under >= 120
                if not sk.underOK: once("underno", "Podsítí nebylo zaplavené – rmut dosedá na suchá síta, lože se utáhne a scezování půjde hůř.", "warn")
            if sk.g < 1:
                if vol(sk.mash) < 1: sk.mash.T = sk.Tw
                addTo(sk.mash, p, 0.0)
            else: skAddMash(p)
            sk.filling = true;S.skFillNow = true
        elif vol(sk.mash) > 1: addTo(sk.mash, p, 0.0)
        else:
            var C0 = skC(); var Cp = cC(p)
            sk.T = (C0 * sk.T + Cp * p.T) / (C0 + Cp)
            var t = sk.cells[0]
            t.w += p.w;t.f += p.f;t.n += p.sg + p.dx + p.ld + p.ns;t.smm += p.smm;t.dms += p.dms;t.col += p.col
        return
    if key == "spilka":
        spAdd(p);return
    if key == "hlt":
        var h = S.hlt
        var received = minf(p.w, maxf(0.0, c.hltCap - h.w))
        if received > 0:
            h.T = (h.w * h.T + received * p.T) / (h.w + received);h.w += received
        if p.w > received:
            S.drainL += p.w - received;once("hltfull%d" % tb(1200), "Tank horké vody je plný – přepad do kanálu.", "warn")
        return
    if key == "lv":
        var lv = S.lv
        if p.get("virt", false): return
        var L: float = p.w
        lv.T = p.T if lv.w < 1 else (lv.w * lv.T + L * p.T) / (lv.w + L);lv.w += L
        if not S.lvFillLog:
            S.lvFillLog = true;logm("Napouštím ledovku vodou z řadu %s °C – až naběhne hladina, pusť agregát (okruh) a nech ji nachladit." % fmt0(p.T))
        if lv.w > c.lvVol:
            var o: float = lv.w - c.lvVol;lv.w = c.lvVol;S.drainL += o
            once("lvover%d" % tb(600), "Ledovka je plná a přetéká přepadem – zavři napouštění.", "warn")
        return
    if key == "tanky": return
    S.drainL += Lp
    S.drainExt += ext(p) + p.su + p.sc + p.ni



func cab_live() -> bool:
    var c = S.cab
    return c.main and c.key and not c.estop

func cab_gate() -> void :
    var c = S.cab; var rv = S.rvp
    if not cab_live():
        if not c.dead:
            c.dead = true;logm("Rozvaděč bez napětí – všechno elektrické stojí (čerpadla, míchadlo, kopačka, ohřev).", "bad")
        S.p1.on = false;S.p2.on = false;S.p3.on = false;S.lv.pump = false;rv.stir = false;S.mp.stir = false
        S.sk.rake = false;S.sk.hyd = 0.0;S.rvpWater.on = false;S.skWater.on = false;S.malt.on = false
        if rv.heat > 0: c.hs = rv.heat;rv.heat = 0.0
        return
    if c.dead:
        c.dead = false;logm("Rozvaděč pod napětím.")
        if c.hs > 0: rv.heat = c.hs;c.hs = 0.0
var prof_on: = false
var prof: = {}
func sim_step(dt: float) -> void :
    cab_gate()
    var spd = S.sp
    if spd.get("lidOpen", false) or not spd.has("lidL0"):
        spd["lidL0"] = spd.L;spd["press"] = 0.0
    else:
        spd["lidL0"] = spd.lidL0 + (spd.L - spd.lidL0) * minf(1.0, dt / 900.0)   # těsnost víčka není dokonalá – přetlak pomalu povolí
        spd["press"] = 1013.0 * maxf(0.0, spd.L - spd.lidL0) / maxf(50.0, S.cfg.spCap - spd.L)
    var c = S.cfg; var m = dt / 60.0; var rvp = S.rvp; var mp = S.mp; var sk = S.sk; var h = S.hlt; var lv = S.lv

    if S.cab.get("mpAuto", false) and cab_live() and not main.auto.on:
        rvp.heat = clampf((S.cab.get("mpSet", 50.0) - rvp.Tsens) / 8.0, 0.0, 1.0) if S.cab.get("mpHeatOn", false) else 0.0

    if h.auto: h.heat = 1.0 if h.T < h.set - 0.5 else (0.0 if h.T > h.set + 0.2 else h.heat)
    # parní kotel jako buben se sytou vodou: tlak drží hořák; když odběr převýší výkon, voda v kotli se dopařuje ze své zásoby
    # a tlak klesá po křivce sytosti (dh'/dp ≈ 57 kJ/(kg·bar) při 2–3 bar) → chladnější pára → všechny ohřevy zpomalí
    var pb: float = float(S.boiler.get("p", c.steamBar))
    var Tst = tsat(pb + 1.013) if (S.boiler.on or pb > 0.05) else 20.0
    S.Tsteam = Tst
    S._pd = {"rvp": _pd(rvp, c.rvpUAst, c.rvpJacketL, Tst), "mp": _pd(mp, c.mpUAst, c.mpJacketL, Tst), 
        "hlt": {"P": c.hltUAst * h.heat * maxf(0.0, Tst - h.T), "wet": 1.0}}
    if not S.boiler.on and pb <= 0.05:
        for k_ in ["rvp", "mp", "hlt"]: S._pd[k_].P = 0.0
    var dem: float = S._pd.rvp.P + S._pd.mp.P + S._pd.hlt.P
    var Qb: float = 0.0
    if S.boiler.on: Qb = c.boilerKW if pb < c.steamBar - 0.02 else minf(c.boilerKW, dem)
    var Cst: float = maxf(50.0, c.get("boilerWaterKg", 1500.0)) * 57.0
    pb = clampf(pb + (Qb - dem) * dt / Cst, 0.0, c.steamBar)
    S.boiler.p = pb
    S.boiler.scale = 1.0;S.boiler.hltScale = 1.0
    S.boiler.load = Qb / c.boilerKW
    if dem > c.boilerKW * 1.02 and S.boiler.on: once("boilmax%d" % tb(1800), "Kotel nestačí: odběr %d kW, kotel dá max %d kW – tlak páry klesá (%s bar), ohřev všude zpomalí. Neohřívej pánev a bojler naráz naplno." % [roundi(dem), roundi(c.boilerKW), fmt1(pb)], "warn")
    if S.boiler.on and pb < c.steamBar - 1.0: once("lowp%d" % tb(900), "Tlak páry spadl na %s bar (nastaveno %s) – pára má jen %s °C, var bude slabý (DMS!). Uber jiné odběry a počkej, až kotel natlakuje." % [fmt1(pb), fmt1(c.steamBar), fmt0(Tst)], "bad")
    var Pin: float = S._pd.hlt.P * S.boiler.hltScale * (1.0 if S.steamOK.hlt else 0.0)
    var Ch: float = 4.19 * maxf(50.0, h.w) + steelC(c.hltSteel)
    h.T += (Pin - c.hltUA * (h.T - c.ambientT)) * dt / Ch
    if h.T > c.tbWater:
        # nad bodem varu se teplo spotřebuje na odpar (2257 kJ/kg), voda ubývá
        var ev: float = minf(h.w, (h.T - c.tbWater) * Ch / 2257.0);h.w -= ev;S.evapTot = S.get("evapTot", 0.0) + ev
        h.T = c.tbWater;once("hltboil%d" % tb(1800), "Horká voda v bojleru vře – pára uniká odvětráním (%s L/h). Ubírej páru." % fmt0(ev / maxf(1e-06, dt) * 3600.0), "warn")

    var Cl: float = maxf(200.0, lv.w) * 4.19; var Q: float = 0.15 * (c.ambientT - lv.T) * maxf(0.2, lv.w / c.lvVol)
    if lv.pump and lv.piv: Q += c.pivLoad
    if lv.chill:
        if lv.T > lv.set + 0.5: lv.chillOn = true
        elif lv.T < lv.set - 0.3: lv.chillOn = false
    else: lv.chillOn = false
    if lv.chill and lv.glyk == false:
        lv.chillOn = false
        if lv.w >= 0.52 * c.lvVol: once("noglyk%d" % tb(900), "Ledovka se nechladí: %s. Voda v ní má %s °C – na %s °C ji dostane jen okruh přes glykol." % [lv.glykWhy if lv.glykWhy != "" else "glykolový okruh není průchozí", fmt1(lv.T), fmt0(lv.set)], "bad")
    if lv.chillOn and lv.w < 0.35 * c.lvVol:
        lv.chillOn = false;once("lvlow%d" % tb(900), "Ledovka má jen %d hl – výparník agregátu není pod vodou, agregát stojí. Dopusť vodu." % roundi(lv.w / 100.0), "bad")
    if lv.chillOn: Q -= c.chillerKW
    lv.T = maxf(-0.5, lv.T + Q * dt / Cl)
    if lv.T <= lv.set + 0.5: lv.reached = true
    if lv.w >= 0.35 * c.lvVol and lv.T > lv.set + 3 and not lv.chill: once("lvwarm0%d" % tb(1800), "Ledovka má %s °C (voda z řadu) – pusť agregát (okruh), ať ji nachladí na %s °C." % [fmt1(lv.T), fmt0(lv.set)], "warn")
    elif lv.chillOn and lv.T > 5 and lv.reached: once("lvwarm%d" % tb(1800), "Ledová voda má %s °C – agregát nestíhá." % fmt1(lv.T), "warn")

    if S.malt.on:
        if rvp.w < 200:
            S.malt.on = false;logm("Nejdřív napusť do pánve vodu, pak teprve sypej slad.", "warn")
        elif not S.malt.get("gate", false):
            once("maltgate%d" % tb(120), "Šoupě násypky je zavřené – slad nepadá do pánve. Otevři šoupě (klepni na něj u násypky nebo tlačítko vpravo).", "warn")
        else:
            var kg: float = minf(c.maltRate * m, S.malt.target - S.malt.added)
            if kg > 0:
                var ratio: float = rvp.w / maxf(1.0, S.malt.added + kg)
                var cl = 0.15 - 0.14 * stirQ(rvp)
                if ratio < 2.5: cl += 0.05
                if stirQ(rvp) < 0.3: once("clump", "Vystíráš bez míchadla – tvoří se hrudky, část škrobu zůstane nezcukřená.", "warn")
                var coarse: float = [0.03, 0.08, 0.16][clampi(roundi(c.mill) - 1, 0, 2)]
                var p = lp();p.T = 18.0; var gt = 0.0
                for id in S.grist: gt += S.grist[id]
                gt = maxf(gt, 1.0)
                for id in S.grist:
                    var M = MALTS.get(id, MALTS.plz); var k: float = kg * S.grist[id] / gt
                    var dry: float = k * (1.0 - M.get("moist", c.maltMoist) / 100.0); var pot: float = dry * M.ext / 100.0
                    var unf: float = pot * M.unf; var st = (pot - unf) * 0.78; var ni = (pot - unf) * 0.22
                    S.extIn += pot;p.w += k - dry;p.g += dry - pot;p.ld += unf;p.su += st * (1.0 - cl - coarse);p.sc += st * (cl + coarse);p.ni += ni
                    p.aA += k * M.enz;p.aB += k * M.enz * 0.87;p.aBs += k * M.enz * 0.13;p.smm += k * 0.0027 * (1.0 if M.enz > 0 else 0.3);p.col += k * M.ebc * 10.0
                addTo(rvp, p, c.rvpSteel);S.malt.added += kg;S.malt.clump += cl * kg
            if S.malt.added >= S.malt.target - 0.01:
                S.malt.on = false
                logm("Vystírání hotovo: %d kg sladu, rmut %s °C." % [roundi(S.malt.added), fmt1(rvp.T)])
                if rvp.T > 60: logm("Vystírací teplota nad 60 °C – beta-amyláza už trpí, rmut bude méně zkvasitelný.", "warn")

    skQmax()
    if S.has("rota") and S.p2.on and S.p2.q > 1.0: S.rota.stick = minf(1.5, S.rota.stick + dt / 900.0)  # ODHAD: zanesení plováku zrny ~15 min
    sk.rakeH = clampf(sk.rakeH + sk.hyd * m / 0.25, 0.0, 1.0)
    if sk.dig > 0:
        sk.dig = maxf(0.0, sk.dig - dt);sk.rakeH = minf(1.0, sk.rakeH + dt / 8.0)
    if sk.hydT != null:
        var d: float = sk.hydT - sk.rakeH; var stp = m / 0.25
        if absf(d) <= stp:
            sk.rakeH = sk.hydT;sk.hydT = null
        else: sk.rakeH += signf(d) * stp

    var rk: float = m * rkRpm() / 1.5
    if sk.rake and sk.g > 1 and sk.rakeH >= 0.15:
        if sk.rakeH < 0.6:
            sk.c = maxf(0.0, sk.c - 0.03 * rk);sk.ch = maxf(0.0, sk.ch - 0.05 * rk)
            var rl: float = minf(sk.g, 0.01 * rk);sk.g -= rl;sk.fines += rl
            var tc: float = sk.get("teig", 0.0) * (1.0 - exp(-0.25 * rk));sk.teig -= tc;sk.g += tc * 0.9;sk.fines += tc * 0.1
        else:
            sk.c = maxf(0.0, sk.c - 0.05 * rk);sk.filt = maxf(0.0, sk.filt - 15.0 * rk)
            var rl2: float = minf(sk.g, 0.08 * rk);sk.g -= rl2;sk.fines += rl2
            var tc2: float = sk.get("teig", 0.0) * (1.0 - exp(-0.6 * rk));sk.teig -= tc2;sk.g += tc2 * 0.7;sk.fines += tc2 * 0.3
    sk.air = maxf(0.0, sk.air - 0.5 * m)

    var Tw_: float = sk.T; var ph_: float = S.pH; var dil: float = clampf((3.5 - sk.outP) / 3.0, 0.0, 1.0)
    var risk: float = 0.0
    if sk.drawn and sk.g > 1:
        risk = clampf(0.5 * clampf((ph_ - 5.7) / 0.4, 0.0, 1.0) + 0.3 * clampf((Tw_ - 76.0) / 8.0, 0.0, 1.0) + 0.2 * dil, 0.0, 1.0)
        if ph_ < 5.7 and Tw_ < 78.0: risk *= 0.3
    S.tannin += (risk - S.tannin) * minf(1.0, dt / 120.0)
    if S.tannin > 0.6: once("tannin%d" % tb(1800), "Riziko vyluhování tříslovin (odhad podle pH %s, teploty %s °C a zředění sladiny %s °P) – uber teplotu vody nebo pH, nebo zastav vyslazování." % [fmt1(ph_), fmt0(Tw_), fmt1(sk.outP)], "warn")
    sk.Qact = 0.0;sk.drawn = false;S.skFillNow = false
    mp.whirlNow = false;S.wp.whirlNow = false;S.cool.Tout = null;S.cool.T1 = null
    var _t0: = Time.get_ticks_usec()
    net_step(dt)
    if prof_on: prof["net"] = prof.get("net", 0) + Time.get_ticks_usec() - _t0;_t0 = Time.get_ticks_usec()
    if S.skWater.on:
        S.maxSparge = maxf(S.maxSparge, S.skWater.T)
    if not S.skFillNow and sk.filling:
        sk.filling = false;sk.settle = 0.0;sk.filt = 0.0
    if sk.filling: sk.settle = 0.0
    sk.settle += m
    if not sk.drawn:
        sk.dp = 20.0;sk.turb = 0.0;sk.dpPa = 0.0;sk.vac = 0.0
    if sk.Qmax < 15 and sk.g > 1 and S.p2.on: once("stuck%d" % tb(1800), "Scezovací káď je zadřená – nakypři mláto a zpomal.", "bad")
    if prof_on: prof["skmid"] = prof.get("skmid", 0) + Time.get_ticks_usec() - _t0;_t0 = Time.get_ticks_usec()
    skThermalStep(dt)
    if skVolTotal() > c.skCap:
        var ov0 = takeCell(sk.cells[0], skVolTotal() - c.skCap);S.drainL += cellVol(ov0);S.drainExt += ov0.f + ov0.n
        once("skover%d" % tb(600), "Scezovací káď přetéká!", "bad")
    if prof_on: prof["skth"] = prof.get("skth", 0) + Time.get_ticks_usec() - _t0;_t0 = Time.get_ticks_usec()

    for pair in [[rvp, "rvp"], [mp, "mp"]]:
        var v = pair[0]; var n: String = pair[1]
        heat(v, m, dt, n);react(v, m)
        if n == "rvp":

            var on: bool = v.stir and v.w > 200
            var rpm: float = v.hz * 0.8 if on else 0.0
            var kk = 0.8 if (sol(v) + v.sg > 20) else 1.0
            var om = rpm / 60.0 * TAU * 0.6 * kk
            var tgt = minf(0.15, om * om * 1.19 * 1.19 / (4.0 * 9.81))
            v.vort = v.vort + (tgt - v.vort) * minf(1.0, dt / 20.0)
        var cap: float = c.rvpCap if n == "rvp" else c.mpCap
        if vol(v) > cap:
            var ov = takePart(v, vol(v) - cap, 1.0);S.drainL += vol(ov);S.drainExt += ext(ov) + ov.su + ov.sc + ov.ni
            once("over%s%d" % [n, tb(600)], "%s přetéká!" % VN(n), "bad")

    for pair in [[rvp, "rvp"], [mp, "mp"]]:
        var v = pair[0]
        if v.boiling and ext(v) > 100 and v.g < 5:
            var pct: float = v.evapKgMin * 60.0 / maxf(1.0, v.w) * 100.0
            v.col += liqVol(v) * 2.5 / 60.0 * m
            v.coag += (pct / 7.0) * m / 60.0
            var form: float = 0.0006 * v.w * minf(1.0, v.coag)
            if form > v.trubHB:
                v.trubSus += form - v.trubHB;v.trubHB = form
            if S.flags.dmsPre == null:
                S.flags.dmsPre = (v.smm + v.dms) * 1000000.0 / maxf(1.0, liqVol(v));S.flags.boilStart = S.t


        if pair[1] != "":
            if v.boiling and ext(v) > 100 and v.g < 5:
                var fp: float = clampf(1.0 - 3.0 * v.coag, 0.15, 1.0)
                v.foam += 10.0 * v.evapKgMin * v.evapKgMin * fp * m
            v.foam -= v.foam * (1.0 - exp( - m / 1.0))
            var free: float = (c.rvpCap if pair[1] == "rvp" else c.mpCap) - hotV(v)
            if v.foam > free and v.foam > 1.0:
                var lost: float = minf(vol(v), (v.foam - maxf(0.0, free)) * 0.15)
                var bo = takePart(v, lost, 1.0);S.drainL += vol(bo);S.drainExt += ext(bo) + bo.su + bo.sc + bo.ni;v.foam = maxf(0.0, free)
                once("boilover%s%d" % [pair[1], tb(300)], "%s překypěla! Pěna je nad okrajem (%d L pěny). Uber páru, dokud nepadne pěna (lom), chmel přidávej při slabším varu." % [VN(pair[1]), roundi(v.foam + lost / 0.15)], "bad")

        if v.boiling and v.g > 20:
            if not v.grainBoiling:
                v.grainBoiling = true;v.grainBoilMin = 0.0
            v.grainBoilMin += m
        if v.grainBoiling and (v.g < 5 or v.T < 85):
            v.grainBoiling = false
            if v.grainBoilMin >= 5:
                S.flags.decCount += 1;logm("Rmut č. %d povařen %d min a spojen." % [S.flags.decCount, roundi(v.grainBoilMin)])

    var wv = S.wp
    if wv.w > 1:
        var Cw: float = cC(wv) + steelC(c.wpSteel);wv.T -= c.wpUA * (wv.T - c.ambientT) * dt / Cw;react(wv, m)
    var hm = sk.mash
    if hm.w > 1:
        react(hm, m)

    for v in [mp, S.wp]:
        if not v.whirlNow and not v.boiling and v.trubSus > 0:
            v.settleMin += m
            var k = 1.0 / 5.0 if v.whirlMin >= 3 else 1.0 / 90.0
            var fl: float = 0.1 * v.trubHB; var dd: float = maxf(0.0, v.trubSus - fl) * (1.0 - exp( - k * m))
            v.trubSus -= dd;v.trubSet += dd
    if prof_on: prof["heat"] = prof.get("heat", 0) + Time.get_ticks_usec() - _t0;_t0 = Time.get_ticks_usec()

    var decoOK = false
    for v in [sk.mash, mp]:
        if v.g > 20 and (v.aA > 0.05 or v.aB + v.aBs > 0.05): decoOK = true
    if rvp.T > 80 and not decoOK and (rvp.su + rvp.sg + rvp.sc) > 0.05 * (ext(rvp) + rvp.su + rvp.sc + 1.0):
        once("rvphot", "Pánev nad 80 °C s nezcukřeným škrobem – enzymy jsou zničené, škrob už nezcukří.", "bad")
    var f = S.flags; var gTot: float = grain_dry_insoluble(S.malt.added)
    var grainV = []
    for v in [rvp, mp]:
        if v.g > 20: grainV.append(v)
    for v in grainV:
        if v.T >= 49 and v.T <= 54 and S.malt.added > 100:
            f.rest52 += m;break
    var held: float = sk.mash.g
    if not f.decStart and gTot > 0 and ((rvp.g > 0.15 * gTot and (mp.g > 0.15 * gTot or held > 0.15 * gTot)) or (mp.g > 0.15 * gTot and held > 0.15 * gTot)): f.decStart = 1
    for v in grainV:
        if f.decStart and not f.decCount:
            if v.T >= 61 and v.T <= 65: f.r63 += m
            if v.T >= 70 and v.T <= 74: f.r72 += m
        if f.decCount >= 1 and v.T >= 70 and v.T <= 74: f.r72b += m
        if f.decCount >= 1 and v.T >= 61 and v.T <= 66: f.r63b += m
    if gTot > 0 and sk.g + held > 0.9 * gTot: f.inSK = 1
    if sk.under >= 120: f.under = 1
    if sk.drawn and sk.g > 1 and sk.turb < 20: f.clearOK = 1
    var wb = false
    for v in [rvp, mp]:
        if v.g < 5 and ext(v) > 200 and hotV(v) >= S.cfg.get("preboilV", 4430.0): f.full = 1
        if v.boiling and ext(v) > 100 and v.g < 5: wb = true
    if wb: f.wortBoil += m
    for v in [mp, S.wp]:
        if v.whirlMin >= 3 and v.settleMin >= 20: f.whirlDone = 1
    if prof_on: prof["checks"] = prof.get("checks", 0) + Time.get_ticks_usec() - _t0;_t0 = Time.get_ticks_usec()
    training.update(dt)
    lauter_auto.update(dt)
    if proc != null: proc.step(dt)
    if prof_on: prof["train"] = prof.get("train", 0) + Time.get_ticks_usec() - _t0
    S.t += dt

func _pd(v, UA: float, jL: float, Tst: float) -> Dictionary:
    var V = liqVol(v) + sol(v) * 0.65
    var wet = clampf((V - 100.0) / (jL - 100.0), 0.0, 1.0)
    return {"P": UA * v.heat * wet * maxf(0.0, Tst - v.T), "wet": wet}


func hopTarget():
    var best = null
    for v in [S.rvp, S.mp]:
        if v.g < 5 and ext(v) > 50:
            if best == null or (int(v.boiling) > int(best.boiling)) or (v.boiling == best.boiling and v.T > best.T): best = v
    if best != null: return best
    return S.mp if S.mp.w > S.rvp.w else S.rvp
func hopAdd(hopId: String, kg: float) -> void :
    var hp = null
    for x in HOPS:
        if x.id == hopId: hp = x
    if hp == null or kg <= 0: return
    var v = hopTarget()
    v.alpha += kg * hp.a * 10.0;v.trubSus += kg * 0.7

    if v.boiling: v.foam += minf(1.0, kg / 3.0) * clampf(1.0 - 3.0 * v.coag, 0.15, 1.0) * (150.0 + 60.0 * v.evapKgMin)
    S.hops.append({"t": S.t, "name": hp.name, "kg": kg, "a": hp.a, "boil": v.boiling, "T": v.T, "boilMin": v.boilMin})
    logm("Chmel: %s kg %s (%s %% alfa) při %s °C." % [fmt1(kg), hp.name, fmt1(hp.a), fmt0(v.T)])
func hopPlanNext():
    return S.hopPlan[S.hopIdx] if S.hopIdx < S.hopPlan.size() else null
func hopPlanAdd() -> void :
    var hp = hopPlanNext()
    if hp == null: return
    hopAdd(hp.hop, hp.kg);S.hopIdx += 1
func kypr() -> void :
    var kl: float = minf(S.sk.g, 3.2);S.sk.g -= kl;S.sk.c *= 0.35;S.sk.fines += kl;S.sk.filt = maxf(0.0, S.sk.filt - 100.0)
    var tk: float = S.sk.get("teig", 0.0) * 0.8;S.sk.teig -= tk;S.sk.g += tk * 0.7;S.sk.fines += tk * 0.3;S.sk.dpPa = 0.0
    logm("Hluboké kypření – lože povoleno, sladina se na chvíli zakalí. Vrať ji do vorlaufu.")
func emptySK() -> void :
    var sk = S.sk; var e = 0.0
    for c in sk.cells: e += c.f + c.n
    sk.lostExt += e
    logm("Mláto vyhrabáno (%d kg mokrého). V mlátě zůstalo %s kg extraktu." % [roundi(skMalt() * 0.26 / 0.2), fmt1(e)])
    sk.cells = [cell(), cell(), cell(), cell(), cell()];sk.g = 0.0;sk.c = 0.0;sk.burst = 0.0;sk.emptied = true;sk.dig = 45.0;sk.teig = 0.0;sk.dpPa = 0.0;sk.ch = 0.0;sk["dep"] = 0.0
    sk.under = 0.0;sk.landed = false;sk.underOK = false;sk.bedMade = false
func lvChillSet(on: bool) -> bool:
    var pct: float = S.lv.w / S.cfg.lvVol
    if on and pct < 0.35:
        logm("Okruh ledovky nejde pustit: nádrž je napuštěná jen na %d %% – nejdřív ji napusť z řadu (aspoň 35 %%)." % roundi(pct * 100.0), "warn")
        return false
    S.lv.chill = on
    logm("Agregát ledovky zapnut – okruh chladí vodu v nádrži." if on else "Agregát ledovky vypnut.")
    return true

func push() -> void :
    var sp = S.sp
    if sp.L < 10:
        logm("Protlačka: ve spilce ještě nic není.", "warn");return
    if S.flags.pushed: logm("Protlačka už proběhla – další voda jen ředí mladinu.", "warn")
    # skutečný obsah potrubí a chladiče na cestě do spilky se vytlačí vodou z řadu (nic se nevyrobí z ničeho)
    if "--lauterlog" in OS.get_cmdline_user_args(): print("INV před protlačkou: ", extract_inventory())
    var ids: Dictionary = S.get("_castEdges", {}); var moved: = 0.0; var ex: = 0.0
    var tout = S.cool.Tout if S.cool.Tout != null else 8.0
    for e in net.edges:
        if not ids.has(e.id) or vol(e.c) < 0.01: continue
        var part = lp()
        for k in LIQ: part[k] = e.c[k]
        for k in SOL: part[k] = e.c[k]
        part.T = minf(e.c.T, tout + 2.0)
        moved += vol(part);ex += ext(part)
        spAdd(part)
        var wfill = lp();wfill.w = e.cap;wfill.T = S.cool.waterT
        e.c = wfill.duplicate();e.q = [wfill]
    var wtail = lp();wtail.w = 8.0;wtail.T = S.cool.waterT;spAdd(wtail)   # rozhraní voda/mladina dojde do spilky
    if "--lauterlog" in OS.get_cmdline_user_args(): print("INV po protlačce: ", extract_inventory())
    S.flags.pushed = 1;logm("Protlačka: %s L mladiny (%s kg extraktu) z chladiče a potrubí dotlačeno vodou do tanku, trubky zůstaly plné vody." % [fmt0(moved), fmt1(ex)])


func gauge_vessel_free(g):
    var key: = "gv|%s|%s|%s" % [g.id, str(g.get("top", true)), str(g.get("bot", true))]
    var cached = _cached(key)
    if cached != null: return cached[0]
    var starts: = []
    var G: Dictionary = main.gauge_geo(g)
    var ports: = []
    if g.get("top", true) != false: ports.append(Vector2(G.x, G.y1 - 30.0))
    if g.get("bot", true) != false: ports.append(Vector2(G.x, G.y2 + 30.0))
    for pt in ports:
        for n in net.nodes.values():
            if n.edges.size() > 0 and Vector2(n.x, n.y).distance_to(pt) <= 14.0 and not starts.has(n.k): starts.append(n.k)

        for e in net.edges:
            var near: = false
            for i in e.pts.size() - 1:
                if Geometry2D.get_closest_point_to_segment(pt, e.pts[i], e.pts[i + 1]).distance_to(pt) <= 14.0: near = true;break
            if near:
                for k in [e.a, e.b]:
                    if not starts.has(k): starts.append(k)
    var found = null
    if starts.size():
        var r: = bfs(starts)
        for h in r.hits:
            if h.att.bind in ["rvp", "mp", "sk", "wp", "hlt", "lv", "spilka"]:
                found = net.byId[h.att.id];break
    _cache[key] = [found]
    return found
func skHead() -> float:
    return cellVol(S.sk.cells[0]) / (S.cfg.skArea * 1000.0)

func tannin_risk() -> float:
    return S.tannin

func lauter_efficiency() -> float:
    return 100.0 * ext(S.rvp) / maxf(1e-09, S.sk.initialExtract) if S.sk.initialExtract > 0 else 0.0


func start_lautering() -> void :
    if S.sk.mode == "lauter": return
    S.sk.mode = "lauter"
    skMashToBed();skQmax()
    S.sk.initialExtract = ext(S.rvp)
    for layer in S.sk.cells: S.sk.initialExtract += layer.f + layer.n
    for edge in net.get("edges", []): S.sk.initialExtract += ext(edge.c)
    logm("Odrmutováno: nech lože usadit, potom vracej první zakalenou sladinu do SK. Do pánve až čirá.")


func turbidity(p) -> float:
    var V: float = liqVol(p)
    if V <= 1e-08: return 0.0
    return 3.0 + 5600.0 * p.fines / V


func sight_sample(it) -> Dictionary:
    var pos = Vector2(it.x, it.y)
    var best = null; var bd: = 40.0; var bt: = 0.0
    for e in net.edges:
        var pts: Array = e.pts; var tot: = 0.0
        for i in pts.size() - 1: tot += pts[i].distance_to(pts[i + 1])
        var run: = 0.0
        for i in pts.size() - 1:
            var a: Vector2 = pts[i]; var b: Vector2 = pts[i + 1]
            var c: = Geometry2D.get_closest_point_to_segment(pos, a, b)
            var dd: = c.distance_to(pos)
            if dd < bd:
                bd = dd;best = e;bt = (run + a.distance_to(c)) / maxf(1.0, tot)
            run += a.distance_to(b)
    if best == null: return {"connected": false, "volume": 0.0, "flow": 0.0}
    var e = best
    var total: float = vol(e.c)
    var lo: float = 0.0 if e.from == "a" else e.cap - total
    var rel: float = bt * e.cap - lo
    var found = null
    if rel >= 0.0 and rel <= total:
        var cum: = 0.0
        for b in e.q:
            var vb: float = vol(b)
            if rel <= cum + vb + 1e-09:
                found = b;break
            cum += vb
    if found == null: return {"connected": true, "volume": 0.0, "flow": e.flow, "direction": 1.0 if e.from == "a" else -1.0}
    var V: float = maxf(1e-09, vol(found)); var LV: float = maxf(1e-09, liqVol(found))
    var fines_mg: float = found.fines * 1000000.0 / V
    return {"connected": true, "volume": vol(found), "flow": e.flow, "T": found.T, "P": plato(found), "turb": turbidity(found), 
        "fine": fines_mg, "husk": 0.15 * fines_mg, "direction": 1.0 if e.from == "a" else -1.0, "air": found.air / LV, 
        "EBC": found.col / maxf(0.01, LV), "kind": contentKind(found)}

func extract_inventory() -> Dictionary:
    var x = {}
    var pot = func(v): return ext(v) + v.su + v.sc + v.ni
    x.rvp = pot.call(S.rvp);x.mp = pot.call(S.mp);x.wp = pot.call(S.wp);x.skmash = pot.call(S.sk.mash)
    var sk = 0.0
    for c in S.sk.cells: sk += c.f + c.n
    x.sk = sk;x.sp = S.sp.f + S.sp.n
    var tr = 0.0
    for e in net.edges: tr += pot.call(e.c)
    x.trubky = tr;x.vyteklo = S.drainExt;x.mlato = S.sk.lostExt;x.skrob_v_lozi = S.sk.lost
    x.celkem = x.rvp + x.mp + x.wp + x.skmash + x.sk + x.sp + tr + S.drainExt + x.mlato + x.skrob_v_lozi;x.vystreno = S.extIn
    return x


func spReport():
    var sp = S.sp; var e: float = sp.f + sp.n
    if sp.L < 10: return null
    var P: float = 100.0 * e / (e + sp.w); var rho: float = 1.0 + 0.004 * P; var V20: float = (sp.w + e) / rho
    var rep: = {"V": V20 / 100.0, "P": P, "IBU": sp.iso * 1000.0 / V20 * 0.44, "T": sp.H / sp.L, "EBC": sp.col / maxf(1.0, sp.L), 
        "att": (sp.f / e) / 0.81 * 100.0 if e > 0 else 0.0, "dms": sp.dms * 1000000.0 / V20, "yield": e / maxf(1.0, S.malt.added) * 100.0}
    rep.merge(yields())
    return rep

## Výtěžnosti – přesné definice (všechno z extraktu, který model skutečně přenesl):
##  e_lab      = laboratorní (potenciální) extrakt sladu = Σ m_slad · (1 − vlhkost) · extrakt_v_sušině   [kg]
##  e_vyr      = extrakt ve vyrážené mladině (při vyrážce z pánve)                                     [kg]
##  e_sp       = extrakt ve spilce                                                                        [kg]
##  VV_varna   = e_vyr / m_slad · 100        – výtěžnost varny „na sladový šrot“ (česká praxe, horká mladina)
##  VV_cz      = V_hor · 0,96 · ρ20 · P / m_slad – totéž spočtené provozně z objemu horké vyrážky (kontrakce 0,96)
##  VV_spilka  = e_sp / m_slad · 100         – výtěžnost do spilky (po ztrátách ve vířivce, chladiči, potrubí)
##  ucinnost   = e_vyr / e_lab · 100         – využití laboratorního extraktu (rozdíl = ztráty v mlátě, vyslazení)
func yields() -> Dictionary:
    var m: float = maxf(1.0, S.malt.added)
    var out: = {"m_slad": m, "e_lab": S.extIn}
    var kn = S.flags.get("knock")
    if kn != null:
        var P: float = kn.P; var rho: float = 1.0 + 0.004 * P
        out["e_vyr"] = kn.e;out["VV_varna"] = kn.e / m * 100.0
        out["VV_cz"] = (kn.V * 0.96) * rho * P / 100.0 / m * 100.0
        out["ucinnost"] = kn.e / maxf(1.0, S.extIn) * 100.0
    var e_sp: float = S.sp.f + S.sp.n
    if e_sp > 1.0:
        out["e_sp"] = e_sp;out["VV_spilka"] = e_sp / m * 100.0
    return out

## Kontrola receptu: cíl = vyrážka (horká, jak se měří v pánvi) targetV hl × °P; ČR vzorec V·0,96·ρ·P
func recipe_check(R: Dictionary, vv: = 71.0) -> String:
    var m: = 0.0
    for k in R.get("grist", {}): m += float(R.grist[k])
    var P: float = R.get("P", 12.0); var V: float = S.cfg.get("targetV", 40.0) * 100.0
    var e_need: float = V * 0.96 * (1.0 + 0.004 * P) * P / 100.0
    var m_need: float = e_need / (vv / 100.0)
    return "Cíl vyrážka %s hl × %s °P = %d kg extraktu → při varní výtěžnosti %d %% je potřeba ≈ %d kg sladu (v receptu %d kg)." % [fmt1(V / 100.0), fmt1(P), roundi(e_need), roundi(vv), roundi(m_need), roundi(m)]

func fmt0(x: float) -> String: return "%d" % roundi(x)
func fmt1(x: float) -> String: return ("%.1f" % x).replace(".", ",")


func K(p: Vector2) -> String: return "%d,%d" % [roundi(p.x), roundi(p.y)]

func on_seg_t(p: Vector2, a: Vector2, b: Vector2, tolr: = 1.5) -> float:
    var d = b - a; var l2 = d.length_squared()
    if l2 < 1: return -1.0
    var cr = absf(d.x * (p.y - a.y) - d.y * (p.x - a.x)) / sqrt(l2)
    if cr > tolr: return -1.0
    var t = ((p.x - a.x) * d.x + (p.y - a.y) * d.y) / l2
    return t if (t > 1e-06 and t < 1.0 - 1e-06) else -1.0

func v_local(v, p: Vector2) -> Vector2:
    var r = deg_to_rad(float(v.get("rot", 0)))
    if r == 0: return p
    var c = Vector2(v.x + v.get("w", 0) / 2.0, v.y + v.get("h", 0) / 2.0)
    return (p - c).rotated( - r) + c

func fit_ports(f) -> Array:
    var A = float(f.get("arm", 30)); var o = []
    for a in f.get("ports", [0, 90]):
        var r = deg_to_rad(float(a));o.append(Vector2(f.x + cos(r) * A, f.y + sin(r) * A).round())
    return o

func _node(nodes: Dictionary, p: Vector2, sfx: String) -> Dictionary:
    var k = K(p) + sfx
    if not nodes.has(k):
        nodes[k] = {"k": k, "x": roundf(p.x), "y": roundf(p.y), "edges": [], "att": [], "valve": null, "end": 0, "noatt": false, "openEnd": false, "_tap": false, "_chil": "", "pside": null}
    return nodes[k]


func build_net(items: Array, joints: Array) -> void :
    var old: Dictionary = {}
    if net.has("edges"):
        for e in net.edges: old[e.id] = e
    var byId = {}
    for it in items: byId[it.id] = it
    var jv = []
    for q in joints: jv.append(Vector2(q[0], q[1]))
    var pipes = []
    for it in items:
        if it.type == "pipe" and it.pts.size() >= 2:
            var pts = []
            for q in it.pts: pts.append(Vector2(q[0], q[1]))
            pipes.append({"id": it.id, "ringmul": float(it.get("ringmul", 1.0)), "medium": it.get("medium", "wort"), "dn": float(it.get("dn", 50)), "pts": pts, "solo": it.get("solo", false), "off": it.get("off", null)})
    for mf in items:
        if mf.type == "manifold":
            var k = 0
            for q in main.mf_ports(mf):
                pipes.append({"id": "%s_b%d" % [mf.id, k], "medium": "water", "dn": 50.0, "pts": [q.back, q.front], "solo": true, "off": null});k += 1
    for f in items:
        if f.type == "fitting":
            var k2 = 0
            for q in fit_ports(f):
                pipes.append({"id": "%s_%d" % [f.id, k2], "medium": "water", "dn": 50.0, "pts": [Vector2(f.x, f.y), q], "solo": false, "off": null});k2 += 1
    var valves = []
    for it in items:
        if it.type == "valve": valves.append(it)
    var JS = {}
    for q in jv: JS[K(q)] = true
    for f in items:
        if f.type == "fitting":
            JS[K(Vector2(f.x, f.y))] = true
            for q in fit_ports(f): JS[K(q)] = true
    for o in main.outlets:
        JS[K(o.front)] = true;JS[K(o.back)] = true
    var offd = func(p, s: String) -> bool: return p.off != null and p.off.get(s, false)

    var cnt = {}
    for p in pipes:
        if p.solo or offd.call(p, "a") or offd.call(p, "b"): continue
        for q in [p.pts[0], p.pts[p.pts.size() - 1]]:
            cnt[K(q)] = cnt.get(K(q), 0) + 1
    for k in cnt:
        if cnt[k] >= 2: JS[k] = true
    var ends = []
    for p in pipes:
        if p.solo or offd.call(p, "a") or offd.call(p, "b"): continue
        var a0: Vector2 = p.pts[0]; var b0: Vector2 = p.pts[p.pts.size() - 1]
        if not JS.has(K(a0)) and not JS.has(K(b0)): continue
        ends.append({"pid": p.id, "pt": a0});ends.append({"pid": p.id, "pt": b0})
    var nodes = {}
    var edges = []
    var pumps = []; var inlets = []; var chillers = []; var vessels = []; var sprays = []
    for it in items:
        if it.type == "pump": pumps.append(it)
        elif it.type == "vessel":
            if it.get("shape") == "inlet": inlets.append(it)
            if it.get("shape") == "chiller" and not it.get("rot", 0): chillers.append(it)
            if it.get("shape") != "platform": vessels.append(it)
        elif it.type == "spray" or it.type == "nozzle": sprays.append(it)
    var cut = {}; var tapPts = {}; var tapPipes = {}; var chilPts = {}
    for p in pipes:
        var ref = [p.pts[0]]
        for i in p.pts.size() - 1:
            var a: Vector2 = p.pts[i]; var b: Vector2 = p.pts[i + 1]; var extra = []
            if not p.solo:
                for e in ends:
                    if e.pid == p.id or not JS.has(K(e.pt)): continue
                    var t = on_seg_t(e.pt, a, b, 5.0)
                    if t > 0: extra.append([t, e.pt])
            for q in jv:
                var t2 = on_seg_t(q, a, b)
                if t2 > 0: extra.append([t2, q])
            for v in valves:
                var t3 = on_seg_t(Vector2(v.x, v.y), a, b)
                if t3 > 0: extra.append([t3, Vector2(v.x, v.y)])
            var d = b - a; var l2 = d.length_squared()
            if l2 >= 1:

                for u in chillers:
                    var r = Rect2(u.x, u.y, u.w, u.h)
                    if r.has_point(a) or r.has_point(b): continue
                    var tt: float = ((u.x + u.w / 2.0 - a.x) * d.x + (u.y + u.h / 2.0 - a.y) * d.y) / l2
                    if tt <= 0.001 or tt >= 0.999: continue
                    var q: Vector2 = (a + d * tt).round()
                    if not r.has_point(q): continue
                    extra.append([tt, q]);chilPts[K(q)] = u.id

                for u in inlets:
                    var uc = Vector2(u.x + u.w / 2.0, u.y + u.h / 2.0)
                    var tt2: float = ((uc.x - a.x) * d.x + (uc.y - a.y) * d.y) / l2
                    if tt2 <= 0.001 or tt2 >= 0.999: continue
                    var fp = a + d * tt2
                    if fp.distance_to(uc) > 10: continue
                    var q2 = fp.round();extra.append([tt2, q2]);tapPts[K(q2)] = true

                var sl = sqrt(l2)
                if sl >= 56:
                    for u in pumps:
                        var t4: float = ((u.x - a.x) * d.x + (u.y - a.y) * d.y) / l2
                        if t4 * sl < 26 or (1 - t4) * sl < 26: continue
                        var fp2: Vector2 = a + d * t4
                        if fp2.distance_to(Vector2(u.x, u.y)) > 8: continue
                        var ud: Vector2 = d / sl; var q1: Vector2 = (fp2 - ud * 24).round(); var q3: Vector2 = (fp2 + ud * 24).round()
                        var rr = deg_to_rad(float(u.get("dir", 0))); var dot = cos(rr) * ud.x + sin(rr) * ud.y
                        var fwd = absf(dot) < 0.3 or dot > 0
                        extra.append([t4 - 24 / sl, q1]);extra.append([t4 + 24 / sl, q3])
                        cut[K(q1) + ">" + K(q3)] = {"id": u.id, "a": "in" if fwd else "out", "b": "out" if fwd else "in"}
            extra.sort_custom( func(x, y): return x[0] < y[0])
            for x in extra: ref.append(x[1])
            ref.append(b)
        var pts = []
        for i in ref.size():
            if i == 0 or K(ref[i]) != K(ref[i - 1]): pts.append(ref[i])
        var last = pts.size() - 1
        var sx: String = "#" + p.id
        var offA: bool = offd.call(p, "a"); var offB: bool = offd.call(p, "b")
        var nk = func(i: int) -> String:
            if i == 0 and offA: return "#offA" + p.id
            if i == last and offB: return "#offB" + p.id
            return "" if JS.has(K(pts[i])) else sx
        var n0 = _node(nodes, pts[0], nk.call(0));n0.end += 1
        if offA: n0.noatt = true
        var n1 = _node(nodes, pts[last], nk.call(last));n1.end += 1
        if offB: n1.noatt = true
        for i in last:
            var na = _node(nodes, pts[i], nk.call(i)); var nb = _node(nodes, pts[i + 1], nk.call(i + 1))
            if chilPts.has(K(pts[i])): na._chil = chilPts[K(pts[i])]
            if chilPts.has(K(pts[i + 1])): nb._chil = chilPts[K(pts[i + 1])]
            if tapPts.has(K(pts[i])):
                tapPipes[p.id] = true
                if not na._tap:
                    na._tap = true;na.end += 1
            if tapPts.has(K(pts[i + 1])):
                tapPipes[p.id] = true
                if not nb._tap:
                    nb._tap = true;nb.end += 1
            var ck: String = K(pts[i]) + ">" + K(pts[i + 1])
            if cut.has(ck):
                na.end += 1;nb.end += 1;na.pside = {"id": cut[ck].id, "side": cut[ck].a};nb.pside = {"id": cut[ck].id, "side": cut[ck].b}
                continue
            var ln: float = Vector2(nb.x, nb.y).distance_to(Vector2(na.x, na.y))
            if p.get("ringmul", 1.0) != 1.0 and absf(na.y - nb.y) < 1.0: ln *= p.ringmul
            var id: String = p.id + "|" + na.k + "|" + nb.k
            var o = old.get(id)
            var e = {"id": id, "pipe": p.id, "medium": p.medium, "a": na.k, "b": nb.k, "ax": na.x, "ay": na.y, "bx": nb.x, "by": nb.y, "len": ln, "dn": p.dn, 
                "cap": maxf(0.3, ln / PX_PER_M * PIPE_L_PER_M * pow(p.dn / 50.0, 2)), "c": o.c if o != null else lp(), "q": o.q if o != null else [], "from": o.from if o != null else "a", "flow": 0, "steam": false}
            edges.append(e);na.edges.append(e);nb.edges.append(e)
    for v in valves:
        var n = nodes.get(K(Vector2(v.x, v.y)))
        if n == null:
            for mm in nodes.values():
                if mm.x == roundf(v.x) and mm.y == roundf(v.y):
                    n = mm;break
        if n != null: n.valve = v.id

    for o in main.outlets:
        var n2 = nodes.get(K(o.front))
        if n2 != null: n2.valve = o.key
    var vesselNodes = {}; var plateNodes = {}; var pumpNodes = {}
    for n in nodes.values():
        if not n.end or n.noatt: continue
        var np = Vector2(n.x, n.y)
        if n.pside != null:
            n.att.append({"kind": "pump", "id": n.pside.id, "side": n.pside.side})
            if not pumpNodes.has(n.pside.id): pumpNodes[n.pside.id] = {"in": [], "out": []}
            pumpNodes[n.pside.id][n.pside.side].append(n.k);continue
        var best = null; var area = 1000000000000.0; var bl = Vector2.ZERO
        for v in vessels:
            var l = v_local(v, np)
            if l.x >= v.x - 14 and l.x <= v.x + v.w + 14 and l.y >= v.y - 14 and l.y <= v.y + v.h + 14:
                if v.w * v.h < area:
                    area = v.w * v.h;best = v;bl = l
        var viaSpray = null
        if best == null:

            for sp in sprays:
                if Vector2(sp.x, sp.y).distance_to(np) > 26: continue
                for v in vessels:
                    if v.get("shape") in ["plate", "inlet"]: continue
                    if sp.x >= v.x - 40 and sp.x <= v.x + v.w + 40 and sp.y >= v.y - 60 and sp.y <= v.y + v.h + 60:
                        if v.w * v.h < area:
                            area = v.w * v.h;best = v;viaSpray = sp.id
                            bl = Vector2(clampf(sp.x, v.x, v.x + v.w), (v.y + v.h) if sp.type == "nozzle" else v.y + 2)
                if best != null: break
        if best != null and viaSpray == null:
            for sp in sprays:
                if sp.type == "spray" and Vector2(sp.x, sp.y).distance_to(np) <= 26:
                    viaSpray = sp.id;break
        if best != null:
            if best.get("shape") == "plate":
                var side = "A" if ((bl.x < best.x + best.w / 2.0) != best.get("mirror", false)) else "B"
                n.att.append({"kind": "plate", "id": best.id, "side": side})
                if not plateNodes.has(best.id): plateNodes[best.id] = {"A": [], "B": []}
                plateNodes[best.id][side].append(n.k)
            else:
                var low: bool = bl.y > best.y + best.h * (0.45 if best.get("shape") == "horiz" else 0.6) or best.get("shape") in ["source", "drain", "box"]
                # scezovací káď: přípojka u dna může ústit pod síto (podsítí) nebo nad síto (síto 22 px nad dnem)
                var under: bool = low and (best.get("bind") != "sk" or bl.y > best.y + best.h - 22.0)
                n.att.append({"kind": "vessel", "id": best.id, "bind": best.get("bind", best.id), "low": low, "under": under, "spray": viaSpray})
                if not vesselNodes.has(best.id): vesselNodes[best.id] = []
                vesselNodes[best.id].append({"k": n.k, "low": low})
            continue
        for pu in pumps:
            var dd = np - Vector2(pu.x, pu.y)
            if dd.length() <= 26:
                var r2 = deg_to_rad(float(pu.get("dir", 0)))
                var side2 = "out" if dd.x * cos(r2) + dd.y * sin(r2) >= 0 else "in"
                n.att.append({"kind": "pump", "id": pu.id, "side": side2})
                if not pumpNodes.has(pu.id): pumpNodes[pu.id] = {"in": [], "out": []}
                pumpNodes[pu.id][side2].append(n.k);break


    for gi in items:
        if gi.type != "gauge": continue
        var G: Dictionary = main.gauge_geo(gi)
        if not G.free: continue
        for pr in [["t", Vector2(G.x, G.y1 - 30.0)], ["b", Vector2(G.x, G.y2 + 30.0)]]:
            for n in nodes.values():
                if Vector2(n.x, n.y).distance_to(pr[1]) <= 14.0:
                    n["gauge"] = true;n["gaugeId"] = gi.id;n["gaugeEnd"] = pr[0]
    for n in nodes.values():
        if n.att.size() or n.edges.size() != 1 or not n.end or n._tap or n.get("gauge", false): continue
        var np2 = Vector2(n.x, n.y); var touch = false
        for e in edges:
            if e == n.edges[0]: continue
            var a = Vector2(e.ax, e.ay); var b = Vector2(e.bx, e.by)
            if Geometry2D.get_closest_point_to_segment(np2, a, b).distance_to(np2) <= 8:
                touch = true;break
        if not touch:
            for pu in pumps:
                if np2.distance_to(Vector2(pu.x, pu.y)) <= 40:
                    touch = true;break
        if not touch:
            for v in vessels:
                if np2.x >= v.x - 30 and np2.x <= v.x + v.w + 30 and np2.y >= v.y - 30 and np2.y <= v.y + v.h + 30:
                    touch = true;break
        if not touch and tapPipes.has(n.edges[0].pipe): touch = true
        n.openEnd = not touch

    var eset = {}
    for e in edges:
        e.pts = [Vector2(e.ax, e.ay), Vector2(e.bx, e.by)];e.ids = [e.id];eset[e.id] = e
    for n in nodes.values():
        if n.edges.size() != 2 or n.att.size() or n.valve != null or n.end or n._tap or n._chil != "" or n.openEnd: continue
        var e1 = n.edges[0]; var e2 = n.edges[1]
        if e1 == e2 or e1.medium != e2.medium: continue

        var p1: Array = e1.pts.duplicate(); var u = e1.a
        if e1.a == n.k:
            p1.reverse();u = e1.b
        var p2: Array = e2.pts.duplicate(); var w = e2.b
        if e2.b == n.k:
            p2.reverse();w = e2.a
        if u == w: continue
        var nu = nodes[u]; var nw = nodes[w]
        var ids: Array = e1.ids + e2.ids
        var c2 = e1.c;addTo(c2, e2.c, 0.0)
        var q1: Array = e1.q.duplicate()
        if e1.a == n.k: q1.reverse()
        var q2: Array = e2.q.duplicate()
        if e2.b == n.k: q2.reverse()
        var ne = {"id": "~".join(PackedStringArray(ids)), "ids": ids, "pipe": e1.pipe, "medium": e1.medium, "a": u, "b": w, "ax": nu.x, "ay": nu.y, "bx": nw.x, "by": nw.y, 
            "len": e1.len + e2.len, "dn": e1.dn, "cap": e1.cap + e2.cap, "c": c2, "q": q1 + q2, "from": "a", "flow": 0, "steam": false, "pts": p1 + p2.slice(1)}
        eset.erase(e1.id);eset.erase(e2.id);eset[ne.id] = ne
        nu.edges.erase(e1);nu.edges.append(ne);nw.edges.erase(e2);nw.edges.append(ne)
        n.edges = []
    edges = eset.values()
    for e in edges:
        if old.has(e.id):
            e.c = old[e.id].c;e.q = old[e.id].q;e.from = old[e.id].from
    var hasSteam = false
    for p in pipes:
        if p.medium == "steam": hasSteam = true
    var byBind = {}; var sws = []; var gvs = []
    for i in items:
        var b = i.get("bind")
        if b != null and b != "":
            if not byBind.has(b): byBind[b] = []
            byBind[b].append(i)
        if i.type == "sw" and i.get("k", "") != "pump": sws.append(i)
        if i.type == "vessel" and i.get("bind") in ["rvp", "mp", "sk", "wp", "hlt", "lv"]: gvs.append(i)
    net = {"chilInline": chilPts.size() > 0, "nodes": nodes, "edges": edges, "byId": byId, "vesselNodes": vesselNodes, "plateNodes": plateNodes, "pumpNodes": pumpNodes, "hasSteam": hasSteam, 
        "byBind": byBind, "sws": sws, "gravity": gvs}
    valve_ver += 1

func valve_open(id) -> bool:
    if main.outlet_open.has(id): return main.outlet_open[id]
    var v = net.byId.get(id)
    if v != null and v.get("stuck", false): return v.get("stuckOpen", true)   # zadřená klapka zůstává, jak byla
    return v == null or v.get("open", true) != false

func _cached(key: String):
    if _cache_ver != valve_ver:
        _cache.clear();_cache_ver = valve_ver
    return _cache.get(key)


func bfs(starts: Array, mode: = "liq", pass2: = []) -> Dictionary:
    var key = mode + "|" + ",".join(PackedStringArray(starts)) + "|" + ",".join(PackedStringArray(pass2))
    var cr = _cached(key)
    if cr != null: return cr
    var prev = {}; var q = []; var hits = []; var leaks = []
    for k in starts:
        if net.nodes.has(k):
            prev[k] = null;q.append(k)
    var qi = 0
    var steam = mode == "steam"
    while qi < q.size():
        var k = q[qi];qi += 1
        var n = net.nodes[k]
        if prev[k] == null and n.valve != null and not valve_open(n.valve): continue
        if prev[k] != null:
            var stop = false
            for a in n.att:
                if a.kind == "vessel":
                    if n._tap: continue
                    hits.append({"k": k, "att": a})
                    if a.spray != null: continue
                    stop = true
                elif a.kind == "pump":
                    if prev[k].get("pump", "") == a.id: continue
                    var pb = net.byId[a.id]
                    if pass2.has(pb.get("bind", "")) and a.side == "in":
                        for k2 in net.pumpNodes.get(a.id, {}).get("out", []):
                            if not prev.has(k2):
                                prev[k2] = {"k": k, "pump": a.id};q.append(k2)
                    stop = true
                elif a.kind == "plate":
                    for k2 in net.plateNodes.get(a.id, {}).get(a.side, []):
                        if not prev.has(k2):
                            prev[k2] = {"k": k, "plate": a.id, "side": a.side};q.append(k2)
            if stop: continue
            if not steam and n.openEnd: leaks.append({"k": k})
        for e in n.edges:
            if (e.medium == "steam") != steam: continue
            var k2 = e.b if e.a == k else e.a
            if prev.has(k2): continue
            var n2 = net.nodes[k2]
            if n2.valve != null and not valve_open(n2.valve): continue
            prev[k2] = {"k": k, "e": e};q.append(k2)
    var r = {"hits": hits, "prev": prev, "leaks": leaks, "paths": {}}
    _cache[key] = r
    return r



## klapky na cestě od nádoby dolů do otevřeného konce trubky (výpust do kanálu); null = taková cesta není
func drain_valves(src_bind: String) -> Variant:
    var vs_ = items_by_bind(src_bind)
    if not vs_.size(): return null
    var v = vs_[0]
    var goals = []
    for k in net.nodes:
        var n = net.nodes[k]
        if n.get("openEnd", false) and n.y >= v.y + v.h * 0.6 and n.x >= v.x - 200.0 and n.x <= v.x + v.w + 200.0: goals.append(k)
    if not goals.size(): return null
    return _route(starts_of(v.id, true), goals, "", -1)

## Proplach pod síty proudem horké vody z bojleru (protlačka): voda jde bojler → C4 → pod síta SK → výpust do kanálu.
## Teče jen když je celá cesta otevřená; bere vodu z bojleru, smývá nános ze dna pod síty (S.sk.dep) a teče do kanálu
func _flush_step(dt: float) -> void :
    var f = S.flush
    if not f.on: return
    for id in f.vs:
        if not valve_open(id):
            once("flushclosed%d" % tb(600), "Proplach pod síty neteče – na cestě je zavřená klapka.", "warn")
            return
    if not f.vs.size(): return
    var L: float = f.rate * dt / 60.0
    if S.hlt.w < L:
        once("flushempty", "Proplach: v bojleru dochází voda.", "bad");return
    S.hlt.w -= L;S.usedHot += L;S.usedWater += L;S.drainL += L;f.L += L
    S._skLow = 1
    var sk = S.sk
    if sk.get("dep", 0.0) > 0.0: sk["dep"] = maxf(0.0, sk.dep - L / f.target)
    if f.L >= f.target and not S.once.has("flushDone"):
        S.once["flushDone"] = 1
        logm("Proplach pod síty hotov: %d L horké vody z bojleru odtekly do kanálu, nános ze dna je smytý." % roundi(f.L), "info")

func route_valves(src_bind: String, dst_bind: String, pass2: = [], where: = -1, plate: = "") -> Variant:
    var start = []
    for v in items_by_bind(src_bind): start.append_array(starts_of(v.id, src_bind != "vodovod"))
    var all = [];last_route = []
    for pb in pass2:
        var pumps_ = items_by_bind(pb)
        if not pumps_.size(): return null
        var pn = net.pumpNodes.get(pumps_[0].id)
        if pn == null: return null
        var r1 = _route(start, pn. in , "", -1)
        if r1 == null: return null
        all.append_array(r1)
        start = pn.out
    if plate != "" and net.plateNodes.has(plate):

        var bn: Array = net.plateNodes[plate].B
        var r3 = _route(start, bn, "", -1)
        if r3 == null: return null
        all.append_array(r3)
        var arrive = last_goal
        start = []
        for k in bn:
            if k != arrive: start.append(k)
    var r2 = _route(start, [], dst_bind, where)
    if r2 == null: return null
    all.append_array(r2)
    return all


func _route(starts: Array, goal_keys: Array, dst_bind: String, where: = -1) -> Variant:
    var prev = {}; var q = []
    for k in starts:
        prev[k] = null;q.append(k)
    var qi = 0; var goal = null
    for k in starts:
        if goal_keys.has(k): goal = k
    while qi < q.size() and goal == null:
        var k = q[qi];qi += 1
        var n = net.nodes[k]
        if prev[k] != null:
            var stop = false
            if goal_keys.has(k):
                goal = k;break
            for a in n.att:
                if a.kind == "vessel" and not n._tap:
                    if dst_bind != "" and a.bind == dst_bind and (where < 0 or int(a.low) == where):
                        goal = k;break
                    if a.spray == null: stop = true
                elif a.kind == "pump": stop = true
                elif a.kind == "plate":
                    for k2 in net.plateNodes.get(a.id, {}).get(a.side, []):
                        if not prev.has(k2):
                            prev[k2] = {"k": k, "plate": a.id};q.append(k2)
            if goal != null or stop: continue
        for e in n.edges:
            if e.medium == "steam": continue
            var k2 = e.b if e.a == k else e.a
            if prev.has(k2): continue
            prev[k2] = {"k": k, "e": e};q.append(k2)
    if goal == null: return null
    var vs = []; var cur = goal;last_goal = goal
    while cur != null:
        last_route.append(cur)
        var n2 = net.nodes[cur]
        if n2.valve != null: vs.append(n2.valve)
        var p = prev.get(cur);cur = p.k if p != null else null
    vs.reverse()
    return vs

func path_to(r: Dictionary, k) -> Array:
    if r.paths.has(k): return r.paths[k]
    var prev: Dictionary = r.prev
    var out = []; var cur = k
    while prev.get(cur) != null:
        var p = prev[cur]
        if p.has("e"): out.append({"e": p.e, "to": cur})
        elif p.has("plate"): out.append({"plate": p.plate, "side": p.side})
        cur = p.k
    out.reverse()
    r.paths[k] = out
    return out

func path_len(path: Array) -> float:
    var L = 0.0
    for s in path:
        if s.has("e"): L += s.e.len
    return L


func path_pct(path: Array) -> float:
    var f = 1.0
    for s in path:
        if not s.has("to"): continue
        var n = net.nodes.get(s.to)
        if n == null or n.valve == null: continue
        var v = net.byId.get(n.valve)
        if v != null and v.get("kind", "") == "skrt": f = minf(f, throttle_flow(float(v.get("pct", 50))))
        elif v != null and v.has("part"): f = minf(f, throttle_flow(float(v.part)))
    return f




func throttle_flow(pct: float) -> float:
    var p: float = clampf(pct / 100.0, 0.0, 1.0)
    if p <= 0.0: return 0.0
    var K: float = p * p
    return 1.0 / sqrt(1.0 + 0.05 * (1.0 / (K * K) - 1.0))

func starts_of(vid: String, low_only: = false) -> Array:
    var o = []
    for x in net.vesselNodes.get(vid, []):
        if not low_only or x.low: o.append(x.k)
    return o

func items_by_bind(b: String) -> Array:
    return net.byBind.get(b, [])




func _similar(c, p) -> bool:
    if absf(c.T - p.T) > 0.4: return false

    var ec: float = c.f + c.dx + c.ld + c.ns + c.sg; var sc_: float = c.g + c.su + c.sc + c.ni
    var ep: float = p.f + p.dx + p.ld + p.ns + p.sg; var sp_: float = p.g + p.su + p.sc + p.ni
    var vc: float = c.w + ec * 0.62 + sc_ * 0.65; var vp: float = p.w + ep * 0.62 + sp_ * 0.65
    if vc < 1e-09 or vp < 1e-09: return true
    var ic: float = 1.0 / vc; var ip: float = 1.0 / vp
    return absf(c.w * ic - p.w * ip) < 0.002 and absf(ec * ic - ep * ip) < 0.0015 and absf(sc_ * ic - sp_ * ip) < 0.002 and absf(c.fines * ic - p.fines * ip) < 0.0004 and absf(c.aA * ic - p.aA * ip) < 0.0005


func _agg_sub(agg, b) -> void :
    var Ct: float = cC(agg); var Cb: float = cC(b)
    if Ct - Cb > 1e-09: agg.T = (Ct * agg.T - Cb * b.T) / (Ct - Cb)
    for k in LIQ: agg[k] = maxf(0.0, agg[k] - b[k])
    for k in SOL: agg[k] = maxf(0.0, agg[k] - b[k])

func edge_push(e, part, from_a: bool) -> Dictionary:
    var out = lp()
    if vol(part) <= 1e-12 and part.w <= 1e-12: return out
    var q: Array = e.q

    if q.size() == 1:
        var q0 = q[0]
        if absf(q0.T - part.T) <= 0.4:
            var ec = e.c
            if ec.w + (ec.f + ec.dx + ec.ld + ec.ns + ec.sg) * 0.62 + (ec.g + ec.su + ec.sc + ec.ni) * 0.65 >= e.cap - 1e-06 and _similar(q0, part): return part
    var bp = part.duplicate()
    var b0 = null
    if q.size(): b0 = q[0] if from_a else q[q.size() - 1]
    if b0 != null and _similar(b0, bp): addTo(b0, bp, 0.0)
    elif from_a: q.insert(0, bp)
    else: q.append(bp)

    if q.size() > 4:
        if from_a:
            addTo(q[1], q[0], 0.0);q.remove_at(0)
        else:
            addTo(q[q.size() - 2], q[q.size() - 1], 0.0);q.remove_at(q.size() - 1)
    addTo(e.c, part, 0.0)
    var over: float = vol(e.c) - e.cap
    while over > 1e-09 and q.size() > 0:
        var idx: int = q.size() - 1 if from_a else 0
        var b = q[idx]; var vb: float = vol(b)
        if vb <= over + 1e-12:
            q.remove_at(idx);_agg_sub(e.c, b);addTo(out, b, 0.0);over -= vb
        else:
            var tk = takePart(b, over, 1.0);_agg_sub(e.c, tk);addTo(out, tk, 0.0);over = 0.0
    if q.is_empty():
        var z = lp();z.T = e.c.T
        for k in z: e.c[k] = z[k]
    return out

func push_path(path: Array, part, Lmin: float, m: float) -> Dictionary:
    for st in path:
        if st.has("plate"):
            if st.side == "A": part.T = plate_cool(st.plate, part.T, Lmin, m)
            continue
        if not st.has("e"): continue
        var e = st.e
        if liqVol(part) <= 1e-09 and sol(part) <= 1e-09: break
        e.from = "a" if st.to == e.b else "b";e.flow = maxf(e.flow, Lmin)
        part = edge_push(e, part, st.to == e.b)
    return part

func _same(c, p, cap: float) -> bool:
    if absf(c.T - p.T) > 0.3: return false
    var vc = vol(c)
    if vc < cap - 1e-06: return false
    var vp = vol(p)
    if vp <= 1e-09: return false
    return absf(c.w / vc - p.w / vp) < 0.002 and absf(sol(c) / vc - sol(p) / vp) < 0.002 and absf(c.aA / vc - p.aA / vp) < 0.0005

func edges_only(path: Array) -> Array:
    var o = []
    for s in path:
        if s.has("e"): o.append(s)
    return o


func suction_path(path: Array) -> Array:
    var o = []
    for i in range(path.size() - 1, -1, -1):
        var s = path[i]
        if s.has("e"): o.append({"e": s.e, "to": s.e.b if s.to == s.e.a else s.e.a})
        else: o.append(s)
    return o

func plate_cool(id: String, Tin: float, Lmin: float, m: float) -> float:
    var it = net.byId[id]; var st = S.plates.get(id); var c = S.cfg
    S.plateFlow[id] = S.plateFlow.get(id, 0.0) + Lmin
    if st == null:
        once("nocool%s%d" % [id, tb(900)], "%s: na straně chladiva nic neteče – mladina se nechladí." % it.get("label", "Chladič"), "warn");return Tin
    var Cw = Lmin / 60.0 * 1.04 * 4.0
    if Cw <= 0: return Tin
    var UA0: float = c.UA2 if it.get("bind") == "ch2" else c.UA1
    var Tout: float
    var Qc: float = maxf(1e-06, st.get("flow", 0.0))
    # prostup tepla deskového výměníku roste s průtokem (Nu ∝ Re^0,65 na každé straně); UA1/UA2 platí při referenčním průtoku
    # mladiny plateRefQ L/min a chladiva 1,1× víc (běžný poměr voda : mladina 1,1–1,5 hl/hl, ABG Eng. Lecture 6)
    var qref: float = c.get("plateRefQ", 80.0)
    var UA: float = UA0 / (0.5 * pow(qref / maxf(1.0, Lmin), 0.65) + 0.5 * pow(qref * 1.1 / maxf(1.0, Qc), 0.65))


    var Cc: float = Qc / 60.0 * 4.19
    var Cmin: float = minf(Cw, Cc); var Cr: float = Cmin / maxf(Cw, Cc); var NTU: float = UA / Cmin
    var eff: float
    if Cr > 0.999: eff = NTU / (1.0 + NTU)
    else:
        var x: float = exp( - NTU * (1.0 - Cr));eff = (1.0 - x) / (1.0 - Cr * x)
    Tout = Tin - eff * Cmin * (Tin - st.T) / Cw
    st["Tcout"] = st.T + eff * Cmin * (Tin - st.T) / Cc
    var duty = Cw * (Tin - Tout);S.plateDuty[id] = S.plateDuty.get(id, 0.0) + duty
    if st.kind == "ice" and not st.get("once", false): S.lv.T += duty * m * 60.0 / (maxf(300.0, S.lv.w) * 4.19)
    if it.get("bind") == "ch1" or (S.cool.T1 == null and st.kind == "water"): S.cool.T1 = Tout
    return Tout

func leak_where(k) -> String:
    var n = net.nodes.get(k)
    if n == null: return ""
    var best = null; var bd = 1000000000.0
    for it in net.byId.values():
        if not (it.type == "vessel" or it.type == "pump") or it.get("label", "") == "" or it.get("shape") == "platform": continue
        var d = Vector2(it.x + it.get("w", 0) / 2.0 - n.x, it.y + it.get("h", 0) / 2.0 - n.y).length()
        if d < bd:
            bd = d;best = it
    return "u „%s“" % best.label if best != null else ""

func spill(r: Dictionary, lk: Dictionary, part, Lmin: float, m: float, what: String) -> void :
    var out = push_path(edges_only(path_to(r, lk.k)), part, Lmin, m)
    S.spillL += vol(out);S.drainExt += ext(out) + out.su + out.sc + out.ni
    var n = net.nodes[lk.k];S.leaks.append(Vector2(n.x, n.y))
    once("leak%s%d" % [lk.k, tb(600)], "%s vytéká z otevřeného konce trubky %s – zavři klapku!" % [what, leak_where(lk.k)], "bad")

func net_step(dt: float) -> void :
    var m = dt / 60.0; var c = S.cfg
    var _n0: = Time.get_ticks_usec()

    var kcool = minf(1.0, dt / 1800.0)
    for e in net.edges:
        if not e.flow and e.c.w > 0.01 and not e.steam:
            e.c.T += (c.ambientT - e.c.T) * kcool
            for b in e.q: b.T += (c.ambientT - b.T) * kcool
        e.flow = 0;e.steam = false
    S._skTop = 0;S._skLow = 0;S._rvpIn = 0;S.leaks = [];S.streams = {}
    _flush_step(dt)
    S.lastDuty = S.plateDuty;S.lastPlateFlow = S.plateFlow
    S.plates = {};S.plateDuty = {};S.plateFlow = {};S.flows = []
    if prof_on: prof["n_trubky"] = prof.get("n_trubky", 0) + Time.get_ticks_usec() - _n0;_n0 = Time.get_ticks_usec()

    S.steamOK = {"rvp": true, "mp": true, "hlt": true}
    var kotle = items_by_bind("kotel")
    if net.hasSteam and kotle.size():
        var st = []
        for k in kotle: st.append_array(starts_of(k.id))
        var r = bfs(st, "steam"); var reach = {}
        for hh in r.hits: reach[hh.att.bind] = hh
        for b in ["rvp", "mp", "hlt"]:
            var hasSt = false
            for v in items_by_bind(b):
                for x in net.vesselNodes.get(v.id, []):
                    for e in net.nodes[x.k].edges:
                        if e.medium == "steam": hasSt = true
            S.steamOK[b] = reach.has(b) or not hasSt
            var on: bool = (S.hlt.heat > 0) if b == "hlt" else (S[b].heat > 0)
            if reach.has(b) and on and S.boiler.on:
                for s in path_to(r, reach[b].k):
                    if s.has("e"): s.e.steam = true
    if prof_on: prof["n_para"] = prof.get("n_para", 0) + Time.get_ticks_usec() - _n0;_n0 = Time.get_ticks_usec()

    var plvs = items_by_bind("plv")
    if plvs.size():
        var plv = plvs[0]
        S.lv.varna = false;S.lv.piv = false;S.lv._circ = false
        var pn = net.pumpNodes.get(plv.id)
        if S.lv.pump and pn != null and S.lv.w < 300: once("plvdry2%d" % tb(600), "Ledovka je skoro prázdná – čerpadlo ledové by běželo nasucho, nečerpá.", "bad")
        if S.lv.pump and pn != null and S.lv.w >= 300:
            var rin = bfs(pn. in )
            var src = null
            for hh in rin.hits:
                if hh.att.bind == "lv" and hh.att.low:
                    src = hh;break
            if src != null:
                var r = bfs(pn.out)
                if net.chilInline:
                    var tg = []
                    for hh in r.hits:
                        if hh.att.bind == "lv": tg.append(hh.k)
                    for k in pn. in :
                        if r.prev.has(k): tg.append(k)
                    for k in tg:
                        var cur = k; var g = 0
                        while cur != null and g < 4000:
                            var nn = net.nodes.get(cur)
                            if nn != null and nn._chil != "":
                                S.lv._circ = true;break
                            var pv = r.prev.get(cur);cur = pv.k if pv != null else null;g += 1
                        if S.lv._circ: break
                var suction = suction_path(path_to(rin, src.k))
                var dests: Array = r.hits; var Lm = 120.0 / maxf(1.0, dests.size())
                for s in suction:
                    if s.has("e"):
                        var p = lp();p.w = 120.0 * m;p.T = S.lv.T;push_path([s], p, 120.0, m)
                for d in dests:
                    var path = path_to(r, d.k)
                    for s in path:
                        if s.has("plate") and s.side == "B":
                            S.plates[s.plate] = {"kind": "ice", "T": S.lv.T, "once": d.att.bind == "hlt", "flow": minf(Lm, S.lv.w / maxf(1e-06, m))};S.lv.varna = true
                    if d.att.bind == "tanky": S.lv.piv = true
                    if d.att.bind == "hlt":

                        var V = minf(Lm * m, S.lv.w); var du = 0.0
                        for s in path:
                            if s.has("plate") and s.side == "B": du = S.lastDuty.get(s.plate, 0.0)
                        var Tw = minf(95.0, S.lv.T + du / maxf(0.1, Lm / 60.0 * 4.19)); var room = maxf(0.0, c.hltCap - S.hlt.w); var Vin = minf(V, room)
                        S.lv.w -= V
                        if Vin > 0:
                            S.hlt.T = (S.hlt.T * S.hlt.w + Tw * Vin) / (S.hlt.w + Vin);S.hlt.w += Vin
                        if V > Vin + 1e-06:
                            S.drainL += V - Vin
                            once("hltfull%d" % tb(900), "Bojler je plný – ohřátá chladicí voda teče přepadem do kanálu.", "warn")
                    var p2 = lp();p2.w = Lm * m;p2.T = S.lv.T;push_path(edges_only(path), p2, Lm, m)
                var dl = []
                for d in dests: dl.append(d.att.bind)
                S.flows.append({"pump": plv.id, "src": "lv", "dst": dl})
            else: once("plvdry%d" % tb(900), "Čerpadlo ledové vody nemá sání z nádrže – zkontroluj klapky.", "warn")
    if prof_on: prof["n_ledova"] = prof.get("n_ledova", 0) + Time.get_ticks_usec() - _n0;_n0 = Time.get_ticks_usec()

    var gl = items_by_bind("glyk")
    if not gl.size(): S.lv.glyk = null
    else:
        var ok = false; var why = "glykolový chladič není propojený s ledovkou okruhem tam i zpět (dvě trubky) nebo je na cestě zavřená klapka"
        if net.chilInline:
            ok = S.lv._circ
            why = "ledovka necirkuluje přes glykolový chladič – nastav klapky tak, aby voda šla z čerpadla ledovky přes chladič glykolu a zpět" if S.lv.pump else "čerpadlo ledovky stojí – okruh přes glykolový chladič neběží"
        else:
            for g in gl:
                var st2 = starts_of(g.id)
                if not st2.size():
                    why = "glykolový chladič nemá připojenou žádnou trubku";continue
                var r2 = bfs(st2); var ks = {}
                for hh in r2.hits:
                    if hh.att.bind == "lv": ks[hh.k] = true
                if ks.size() >= 2 or (ks.size() >= 1 and st2.size() >= 2):
                    ok = true;break
        var sws = net.sws

        if sws.size():
            var on = false
            for i in sws:
                if i.get("on", false): on = true
            if on and not S.lv.chill: S.lv.chill = true
            if not on and S.lv.chill: S.lv.chill = false
            if ok and not on:
                ok = false;why = "spínač glykolu je vypnutý"
        S.lv.glyk = ok;S.lv.glykWhy = "" if ok else why
    if prof_on: prof["n_glykol"] = prof.get("n_glykol", 0) + Time.get_ticks_usec() - _n0;_n0 = Time.get_ticks_usec()

    for src in items_by_bind("vodovod"):
        if src.get("shape") == "inlet" and src.get("on", false) != true: continue
        var r = bfs(starts_of(src.id))
        var outs = []
        for d in r.hits:
            if d.att.bind != "vodovod": outs.append({"d": d, "path": path_to(r, d.k)})
        for lk in r.leaks: outs.append({"lk": lk, "path": path_to(r, lk.k)})
        var sw = 0.0; var nOpen = 0
        for o in outs:
            o.f = path_pct(o.path);o.w = o.f / sqrt(1.0 + path_len(o.path) / 3000.0);sw += o.w
            if o.f > 0: nOpen += 1
        if nOpen > 1: once("mainsplit%d" % tb(900), "Voda z řadu teče do %d míst naráz – tlak se dělí, do každého teče méně." % nOpen)
        for o in outs:
            if o.f <= 0: continue
            var mq: float = float(S.cfg.get("mainsQ", 900.0))
            var cap0: float = (0.9 if (o.has("d") and o.d.att.bind == "lv") else 0.8) * mq * o.f   # řad s dobrým tlakem: ~6–7 l/s do jednoho místa (na protlačky a proplach), dělí se mezi víc míst
            var q = minf(cap0, mq * o.w / maxf(1e-06, sw))
            if q < 0.3: continue
            if o.has("lk"):
                var pl = lp();pl.w = q * m;pl.T = S.cool.waterT;spill(r, o.lk, pl, q, m, "Voda z řadu");continue
            var d = o.d; var path: Array = o.path; var rate = q; var T: float = S.cool.waterT
            var plates = []
            for s in path:
                if s.has("plate") and s.side == "B": plates.append(s)
            for s in plates:
                S.plates[s.plate] = {"kind": "water", "T": S.cool.waterT}
                var pf: float = S.lastPlateFlow.get(s.plate, 0.0)
                if pf > 0: rate = maxf(rate, 1.2 * pf * o.f)
            for s in plates: S.plates[s.plate]["flow"] = rate
            for s in plates: T += S.lastDuty.get(s.plate, 0.0) / (rate / 60.0 * 4.19)
            var part = lp();part.w = rate * m;part.T = S.cool.waterT

            for s in path:
                if s.has("plate"):
                    part.T = T;continue
                var e = s.e;e.from = "a" if s.to == e.b else "b";e.flow = maxf(e.flow, rate)
                part = edge_push(e, part, s.to == e.b)
            var out = part
            if d.att.bind == "hlt": S.cool.Tw = T
            _stream(d, out)
            if d.att.bind == "lv":
                var fl = lp();fl.w = rate * m;fl.T = S.cool.waterT
                var viaChil = false
                for s in path:
                    if s.has("e") and net.nodes[s.to]._chil != "": viaChil = true
                if viaChil:
                    var gOn: bool = S.lv.chill
                    if gOn:
                        fl.T = maxf(c.lvSet, S.cool.waterT - c.chillerKW / (rate / 60.0 * 4.19))
                        once("lvprechill", "Voda z řadu teče do ledovky přes glykolový chladič – přitéká vychlazená na %s °C." % fmt1(fl.T))
                    else: once("lvprechill0%d" % tb(1800), "Voda z řadu teče do ledovky přes glykolový chladič, ale glykol je vypnutý – přitéká teplá z řadu. Zapni spínač glykolu.", "warn")
                dstGive("lv", fl, {"src": "vodovod"})
            else:
                if d.att.bind == "sk":
                    if sanSpray(d.att.spray): skWallHeat(out)
                    skAddWater(liqVol(out), out.T, d.att.get("under", d.att.low))
                else: dstGive(d.att.bind, out, {"src": "vodovod"})

        var me_m: float = _dead_due("main", m)
        if me_m > 0.0:
            var used_m: = {}
            for o in outs:
                if o.f > 0:
                    for s in o.path:
                        if s.has("e"): used_m[s.e.id] = true
            _dead_end_apply(_dead_end_plan(r, used_m, 80.0, me_m), S.cool.waterT)
    if prof_on: prof["n_vodovod"] = prof.get("n_vodovod", 0) + Time.get_ticks_usec() - _n0;_n0 = Time.get_ticks_usec()

    var wantR: bool = S.rvpWater.on; var wantS: bool = S.skWater.on
    if wantR or wantS:
        var st3 = []
        for hh in items_by_bind("hlt"): st3.append_array(starts_of(hh.id, true))
        var r = bfs(st3, "liq", ["phw"])
        var dests = []
        for d in r.hits:
            if d.att.bind != "hlt" and d.att.bind != "vodovod": dests.append(d)
        var hasR = false; var hasS = false
        for d in dests:
            if d.att.bind == "rvp": hasR = true
            if d.att.bind == "sk": hasS = true
        if wantR and not hasR: once("nowR%d" % tb(600), "Voda do pánve nemá cestu – otevři klapky od horké vody.", "warn")
        if wantS and not hasS: once("nowS%d" % tb(600), "Vyslazovací voda nemá cestu do SK – otevři klapky.", "warn")
        var wf: float = c.waterFlow * S.rvpWater.k
        var rate: float = (wf if wantR else 0.0) + (S.skWater.flow if wantS else 0.0)
        if wantR:
            var left: float = S.rvpWater.target - S.rvpWater.added
            if left < wf * m: rate -= wf - left / m
        var Tset: float = S.rvpWater.T if wantR else S.skWater.T
        if (dests.size() or r.leaks.size()) and rate > 0:

            var L = rate * m
            if not S.mixAuto: L = minf(L, maxf(0.0, S.hlt.w - 150.0))
            var T: float = drawWater(L, Tset) if S.mixAuto else drawHot(L)
            rate = L / m
            S.usedWater += L
            var ws = {}; var wsum = 0.0
            for d in dests:
                var pa = path_to(r, d.k); var w = path_pct(pa) / sqrt(1.0 + path_len(pa) / 3000.0);ws[d.k] = w;wsum += w
            for lk in r.leaks:
                var pa2 = path_to(r, lk.k); var w2 = path_pct(pa2) / sqrt(1.0 + path_len(pa2) / 3000.0);ws[lk.k] = w2;wsum += w2
            wsum = maxf(1e-06, wsum)
            for lk in r.leaks:
                var p = lp();p.w = L * ws[lk.k] / wsum;p.T = T
                if p.w > 1e-06: spill(r, lk, p, rate * ws[lk.k] / wsum, m, "Horká voda")
            for d in dests:
                var p2 = lp();p2.w = L * ws[d.k] / wsum;p2.T = T
                if p2.w <= 1e-06: continue
                var _pp: = Time.get_ticks_usec()
                var out = push_path(path_to(r, d.k), p2, rate, m)
                if prof_on: prof["n_h_push"] = prof.get("n_h_push", 0) + Time.get_ticks_usec() - _pp;_pp = Time.get_ticks_usec()
                var Lo = liqVol(out)
                _stream(d, out)
                if d.att.bind == "sk":
                    if d.att.get("under", d.att.low): S._skLow = 1
                    else: S._skTop = 1
                if d.att.bind == "rvp":
                    S._rvpIn = 1;S.rvpWater.added += Lo;addTo(S.rvp, out, c.rvpSteel)
                elif d.att.bind == "sk":
                    if sanSpray(d.att.spray): skWallHeat(out)
                    S.skWater.total += Lo;skAddWater(Lo, out.T, d.att.get("under", d.att.low))
                else:
                    dstGive(d.att.bind, out, {"src": "hlt"})
                    once("hwelse%s%d" % [d.att.bind, tb(600)], "Horká voda teče i do: %s – zavři klapku." % net.byId[d.att.id].get("label", d.att.bind), "warn")

            var used_h: = {}
            var me_h: float = _dead_due("hot", m)
            var hplan: Array = []
            if me_h > 0.0:
                for d in dests:
                    for s in path_to(r, d.k):
                        if s.has("e"): used_h[s.e.id] = true
                for lk in r.leaks:
                    for s in path_to(r, lk.k):
                        if s.has("e"): used_h[s.e.id] = true
                hplan = _dead_end_plan(r, used_h, 60.0, me_h)
            var Lh: = 0.0
            for x in hplan: Lh += x[1]
            if Lh > 0.0:
                var Lh2: float = Lh if S.mixAuto else minf(Lh, maxf(0.0, S.hlt.w - 150.0))
                if Lh2 > 0.01:
                    var fk: float = Lh2 / Lh
                    for x in hplan: x[1] *= fk
                    var Th: float = drawWater(Lh2, Tset) if S.mixAuto else drawHot(Lh2)
                    S.usedWater += Lh2
                    _dead_end_apply(hplan, Th)
        if wantR and S.rvpWater.added >= S.rvpWater.target - 0.5:
            S.rvpWater.on = false;logm("Pánev: napuštěno %d L vody." % roundi(S.rvpWater.added))
    if prof_on: prof["n_horka"] = prof.get("n_horka", 0) + Time.get_ticks_usec() - _n0;_n0 = Time.get_ticks_usec()


    if not S.has("clog"): S.clog = {"rvp": 0.0, "sk": 0.0}
    for cb in ["rvp", "sk"]:
        var mv = S.rvp if cb == "rvp" else S.sk.mash
        var mixed: bool = (S.rvp.stir and cab_live()) if cb == "rvp" else (S.sk.rake and cab_live())
        if vol(mv) < 5 or thick_factor(mv) < 0.15: S.clog[cb] = maxf(0.0, S.clog[cb] - dt / 60.0)
        elif mixed: S.clog[cb] = maxf(0.0, S.clog[cb] - dt / 60.0)
        else: S.clog[cb] = minf(1.0, S.clog[cb] + dt / 300.0)

    for key in ["p1", "p2", "p3"]:
        var q = S[key]
        if not q.on:
            q.status = "";q.flowing = false
            q.pt += dt
            if q.pt >= 2.0:
                _pump_prime_idle(key, q, q.pt);q.pt = 0.0
            continue
        var pis = items_by_bind(key)
        if not pis.size():
            once("nopump" + key, "%s není ve schématu." % key.to_upper(), "warn");q.status = "není ve schématu";continue
        var pumpIt = pis[0]; var pname: String = pumpIt.get("label", key.to_upper())
        if q.get("airLock", false):
            q.prime = minf(q.prime, 0.05);q.q = 0.0;q.status = "ZAVZDUŠNĚNO – hučí naprázdno, nečerpá (vypni, odvzdušni)";q.dry += dt
            if q.dry > 120.0:
                q.on = false;q.dry = 0.0;logm("Ochrana chodu nasucho vypnula %s – zavzdušněné čerpadlo běželo bez kapaliny." % pname, "bad")
            continue
        var pn = net.pumpNodes.get(pumpIt.id, {"in": [], "out": []})
        var rs = bfs(pn. in )
        var srcs = []
        for hh in rs.hits:
            if hh.att.low and hh.att.bind in ["rvp", "mp", "sk", "wp", "hlt", "spilka"]: srcs.append(hh)
        var src = null
        for hh in srcs:
            if _src_level(hh.att.bind) > 5:
                src = hh;break
        if src == null and srcs.size(): src = srcs[0]
        if src == null:
            q.status = "nesaje";once("nosrc%s%d" % [key, tb(300)], "%s nemá odkud sát – klapka před čerpadlem je zavřená nebo je nádoba prázdná. Čerpadlo nejdřív vypni, pak zavírej klapky; při chodu nasucho se čerpadlo odvzdušní a ucpávka se ničí." % pname, "warn")
            if q.get("flowing", false):
                q.flowing = false
                logm("%s: klapka před čerpadlem se zavřela za jeho chodu – čerpadlo nasává vzduch. Správně: nejdřív čerpadlo vypnout, pak zavřít klapku za ním a až potom před ním." % pname, "bad")
            q.prime = maxf(0.0, q.prime - dt / 15.0)
            q.dry += dt
            if q.dry > 60.0:
                q.on = false;q.dry = 0.0;logm("Ochrana chodu nasucho vypnula %s – běžela bez kapaliny (hrozilo poškození ucpávky)." % pname, "bad")
            continue
        var rd = bfs(pn.out)
        var dests2 = []
        for d in rd.hits:
            if d.att.bind != "vodovod": dests2.append(d)
        var lks: Array = rd.leaks
        if not dests2.size() and not lks.size():
            if q.get("flowing", false):
                q.flowing = false
                logm("%s: klapka za čerpadlem se zavřela za jeho chodu – čerpadlo tlačí do zavřeného potrubí (tlakový ráz, ohřev kapaliny). Správně: nejdřív čerpadlo vypnout, teprve potom zavírat klapky." % pname, "bad")
            q.status = "tlačí do zavřené klapky";once("nodst%s%d" % [key, tb(300)], "%s tlačí proti zavřené klapce za čerpadlem – nic neteče. Správné pořadí: čerpadlo vypnout, zavřít klapku za čerpadlem, pak před čerpadlem; spouštět naopak (otevřít před, zavodnit, otevřít za, zapnout)." % pname, "warn")
            q.dead += dt
            if q.dead > 90.0:
                q.on = false;q.dead = 0.0;logm("Tepelná ochrana vypnula %s – dlouho tlačila proti zavřené klapce (kapalina v čerpadle se ohřívala)." % pname, "bad")
            continue
        var sb: String = src.att.bind
        if key == "p2" and sb == "sk" and S.sk.mode == "mash" and (vol(S.sk.mash) > 5 or S.sk.g > 1):
            q.on = false;q.status = "Nejprve odrmutuj a vytvoř lože.";logm("C2 by z rmutu nenápadně udělalo filtrační lože. Nejprve dokonči rmutování a stiskni Odrmutovat a vytvořit lože.", "warn");continue
        var names = {}
        for d in dests2: names[net.byId[d.att.id].get("label", d.att.bind)] = true
        q.status = "%s » %s" % [net.byId[src.att.id].get("label", sb), " + ".join(PackedStringArray(names.keys()))]
        var ids = {}
        for d in dests2: ids[d.att.id] = true
        if ids.size() > 1: once("split%s%d" % [key, tb(600)], "%s: průtok se dělí do %d nádob." % [pname, ids.size()], "warn")
        # víčko spilky: zavřené = vzduch nemá kudy ven, tank se tlakuje (stlačitelný vzduch nad hladinou: p ≈ 1013·ΔV/V_vzduch, ODHAD)
        var to_sp: bool = false
        for d in dests2:
            if d.att.bind == "spilka": to_sp = true
        if to_sp and not S.sp.get("lidOpen", false):
            var pr: float = S.sp.get("press", 0.0)
            if pr > 40.0: once("splidw%d" % tb(300), "Spilka se tlakuje (%d mbar) – víčko je zavřené, vzduch nemá kudy ven! Otevři víčko na tanku." % roundi(pr), "warn")
            if pr > 120.0:
                q.status = "přetlak ve spilce – otevři víčko";once("splidx%d" % tb(300), "Přetlak ve spilce %d mbar – tank není tlaková nádoba, hrozí deformace. Čerpadlo stojí, otevři víčko." % roundi(pr), "bad")
                q.dead += dt
                if q.dead > 90.0: q.on = false;q.dead = 0.0;logm("Ochrana vypnula %s – přetlak ve spilce (zavřené víčko)." % pname, "bad")
                continue

        var sp_ = path_to(rs, src.k); var rp = 0.0; var nd = 0
        var dpaths: Array = []
        for d in dests2: dpaths.append(path_to(rd, d.k));nd += 1
        for lk in lks: dpaths.append(path_to(rd, lk.k));nd += 1
        # --- hydraulika: křivka odstředivého čerpadla s měničem × odpor trasy (Darcy–Weisbach, místní ztráty, výška) ---
        var pump_y: float = pumpIt.y
        var sv = net.byId[src.att.id]; var Tsrc = _src_temp(sb)
        var cap_s = _src_cap(sb)
        var lvl_frac: float = clampf(_src_level(sb) / maxf(1.0, cap_s), 0.0, 1.0)
        var lvl_m = lvl_frac * sv.h / PX_PER_M
        var head = (pump_y - (sv.y + sv.h)) / PX_PER_M + lvl_m
        var fl_v = _src_fluid(sb)
        var th0: float = thick_factor(fl_v) if fl_v.has("g") else 0.0
        var rho: float = density(fl_v) if fl_v.has("g") else 1.0
        var mu: float = skViscosity(Tsrc, plato(fl_v) if fl_v.has("g") else 0.0) * (1.0 + 60.0 * th0 * th0)
        var y_lvl: float = sv.y + sv.h - lvl_frac * sv.h
        var Hst: = -99.0
        for dp_ in dpaths:
            var last = dp_[dp_.size() - 1] if dp_.size() else null
            if last != null and last.has("to"): Hst = maxf(Hst, (y_lvl - net.nodes[last.to].y) / PX_PER_M)
        if Hst < -98: Hst = 0.0
        var HY: Dictionary = pump_point(key, q.flow, Hst, sp_, dpaths, rho, mu, th0)
        q.hz = HY.hz;q.bar = HY.bar;q.hyd = HY.why
        rp = HY.Q / maxf(1.0, q.flow)
        var Pv = pow(10.0, 8.07131 - 1730.63 / (233.426 + Tsrc)) * 0.00133322
        var npsha = (0.99 - Pv) * 10.2 / rho + head - HY.hs_loss
        q.npsh = npsha
        var cav_f = clampf((npsha - 0.8) / 2.2, 0.12, 1.0)
        if cav_f < 0.85: once("cav%s%d" % [key, tb(600)], "%s kavituje (sání z %s %s °C, výška %s m, NPSH %s m) – čerpadlo chrastí a nedává plný průtok. Zpomal ho, nech vychladnout nebo dolej nádobu." % [pname, VN(sb), fmt0(Tsrc), fmt1(head), fmt1(npsha)], "warn")
        if HY.why == "max": once("pmax%s%d" % [key, tb(900)], "%s jede na plné otáčky (50 Hz) a nastavených %d L/min přes tuhle trasu nedá – teče %d L/min (odpor trasy, výška, hustota)." % [pname, roundi(q.flow), roundi(HY.Q)], "warn")
        if HY.why == "lift": once("plift%s%d" % [key, tb(900)], "%s nepřekoná výškový rozdíl %s m – nic neteče." % [pname, fmt1(Hst)], "bad")
        rp *= cav_f


        var th: float = 0.0
        var cg: float = 0.0

        var mv_src = S.rvp if sb == "rvp" else (S.sk.mash if sb == "sk" and vol(S.sk.mash) > 5 else null)
        if mv_src != null:
            th = thick_factor(mv_src)
            rp /= 1.0 + 0.5 * th

            cg = S.clog.get(sb, 0.0) * th
            if cg > 0.05:
                rp *= 1.0 - 0.85 * cg
                if cg > 0.35: once("clog%s%d" % [sb, tb(600)], "%s: zrno se bez míchání usadilo nad výpustí a ucpává ji – rmut teče pomalu. Zapni %s a nech čerpadlo rmut protlačit (zrno se uvolní, voda ze rmutu pomůže)." % [VN(sb), "míchadlo" if sb == "rvp" else "kopačku"], "warn")


        if head >= 0.3 and cg < 0.75:
            q.prime = minf(1.0, q.prime + dt / (6.0 + 10.0 * th))
        elif cg >= 0.75:
            q.prime = maxf(0.0, q.prime - dt / 10.0)
        if head < 0.3 and q.prime < 0.95:
            q.status = "nezavodněno – hladina pod čerpadlem, nenasaje";once("prime%s%d" % [key, tb(600)], "%s se nezavodnilo: hladina v %s je pod čerpadlem (%s m), odstředivé čerpadlo samo nenasaje. Dolej nádobu nebo zavodni sání." % [pname, VN(sb), fmt1(head)], "warn")
            q.dry += dt
            if q.dry > 60.0:
                q.on = false;q.dry = 0.0;logm("Ochrana chodu nasucho vypnula %s – běželo nezavodněné." % pname, "bad")
            continue
        if q.prime < 0.95:
            once("startdry%s%d" % [key, tb(120)], "%s zapnuto dřív, než se zavodnilo těleso – točí se napůl na sucho. Nejdřív otevři klapku před čerpadlem a počkej, až naběhne modrá tečka u čerpadla, pak ho zapni." % pname, "warn")
            q.dry += dt * (1.0 - q.prime)
            q.status = "zavodňuje se (%d %%)" % roundi(q.prime * 100.0)
            if q.dry > 40.0:
                q.on = false;q.dry = 0.0;logm("Ochrana chodu nasucho vypnula %s – těleso čerpadla se nezavodnilo (otevři klapku před čerpadlem a nech čerpadlo zavodnit, než ho zapneš; ucpanou výpust nejdřív uvolni)." % pname, "bad")
                continue
        else:
            q.dry = 0.0
        var Qf: float = q.flow * rp * q.prime
        q.q = Qf
        if Qf >= 0.5: q.status += " · %d Hz · %s bar" % [roundi(q.get("hz", 0.0)), fmt1(q.get("bar", 0.0))]

        if Qf < 0.5:
            if q.prime >= 0.95: q.status = "přiškrceno – nic neteče"
            continue
        q.dead = 0.0;q.flowing = true

        if mv_src != null and S.clog.get(sb, 0.0) > 0.0:
            S.clog[sb] = maxf(0.0, S.clog[sb] - dt / 120.0)
        var part0 = srcTake(sb, Qf, m, key)
        if part0 == null:
            q.on = false;logm("%s zastaveno – %s je prázdná." % [pname, net.byId[src.att.id].get("label", sb)]);continue
        var turb: float = S.sk.turb
        var part = push_path(suction_path(sp_), part0, Qf, m)
        var nAll = dests2.size() + lks.size()
        if lks.size(): q.status += (" + " if dests2.size() else "") + "VYTÉKÁ z otevřené trubky"
        for i in nAll:
            var pp = part if i == nAll - 1 else takePart(part, vol(part) / (nAll - i), 1.0)
            if i >= dests2.size():
                var kd = contentKind(pp)
                spill(rd, lks[i - dests2.size()], pp, Qf / nAll, m, "%s: %s" % [pname, "dílo" if kd == "mash" else ("sladina / mladina" if kd == "wort" else "kapalina")]);continue
            var d = dests2[i]
            if d.att.bind == "spilka":
                var ce: Dictionary = S.get("_castEdges", {})
                for s in sp_: if s.has("e"): ce[s.e.id] = true
                for s in path_to(rd, d.k): if s.has("e"): ce[s.e.id] = true
                S["_castEdges"] = ce
            var out = push_path(path_to(rd, d.k), pp, Qf / nAll, m)
            _stream(d, out)
            dstGive(d.att.bind, out, {"src": sb, "turb": turb, "m": m})

        var used_p: = {}
        var me_p: float = _dead_due(key, m)
        var pplan: Array = []
        if me_p > 0.0:
            for d in dests2:
                for s in path_to(rd, d.k):
                    if s.has("e"): used_p[s.e.id] = true
            for lk in lks:
                for s in path_to(rd, lk.k):
                    if s.has("e"): used_p[s.e.id] = true
            pplan = _dead_end_plan(rd, used_p, Qf * 0.5, me_p)
        var Lp: = 0.0
        for x in pplan: Lp += x[1]
        if Lp > 0.01:
            var pfill = srcTake(sb, Lp / me_p, me_p, key)
            if pfill != null:
                for xi in pplan.size():
                    var x2 = pplan[xi]
                    var piece = pfill if xi == pplan.size() - 1 else takePart(pfill, minf(x2[1], vol(pfill)), 1.0)
                    if vol(piece) > 1e-06: edge_push(x2[0], piece, true)
        if sb == "mp":
            for d in dests2:
                if d.att.bind == "mp": S.mp.whirlNow = true
        var dl = []
        for d in dests2: dl.append(d.att.bind)
        S.flows.append({"pump": pumpIt.id, "src": sb, "dst": dl})
    if prof_on: prof["n_cerpadla"] = prof.get("n_cerpadla", 0) + Time.get_ticks_usec() - _n0;_n0 = Time.get_ticks_usec()

    for v in net.gravity:
        var b = v.get("bind")

        var gk: String = "gd|" + v.id
        var outs = _cached(gk)
        if outs == null:
            outs = []
            for s0 in starts_of(v.id, true):
                var r = bfs([s0])
                var lk = null
                for l in r.leaks:
                    if net.nodes[l.k].y >= v.y + v.h * 0.6:
                        lk = l;break
                if lk == null: continue
                var path = path_to(r, lk.k); var f = path_pct(path) / sqrt(1.0 + path_len(path) / 6000.0)
                if f > 0: outs.append({"path": edges_only(path), "f": f, "lk": lk})
            _cache[gk] = outs
        if not outs.size(): continue
        var fr: float
        match b:
            "sk": fr = skVolTotal() / c.skCap
            "hlt": fr = S.hlt.w / c.hltCap
            "lv": fr = S.lv.w / c.lvVol
            "rvp": fr = vol(S.rvp) / c.rvpCap
            "wp": fr = vol(S.wp) / c.wpCap
            _: fr = vol(S.mp) / c.mpCap


        var head_m = maxf(0.02, fr) * v.h / PX_PER_M
        var tot = minf(240.0, 120.0 + 30.0 * outs.size()) * minf(1.3, sqrt(head_m / 1.05))
        for o in outs:
            var Lmin: float = tot * o.f / outs.size(); var part = null
            if b == "lv":
                var L = minf(Lmin * m, maxf(0.0, S.lv.w))
                if L > 0.01:
                    S.lv.w -= L;part = lp();part.w = L;part.T = S.lv.T
            else: part = srcTake(b, Lmin, m, "p2" if b == "sk" else "drain")
            if part == null: continue
            var kd = contentKind(part)
            var out = push_path(o.path, part, Lmin, m);S.drainL += vol(out);S.drainExt += ext(out) + out.su + out.sc + out.ni
            var n = net.nodes[o.lk.k];S.leaks.append(Vector2(n.x, n.y))
            once("gdrain%s%d" % [b, tb(900)], "%s: %s vytéká spodem otevřenými klapkami do kanálu %s." % [v.get("label", b), "dílo" if kd == "mash" else ("sladina" if kd == "wort" else "voda"), leak_where(o.lk.k)], "warn")
    _vessel_gravity(m)
    skHatchStep(m)



func skHatchStep(m: float) -> void :
    var sk = S.sk
    if not sk.get("hatch", false): return
    if skVolTotal() < 0.5 and sk.g < 0.01: return
    var Lq: float = 450.0 * m; var out_L: = 0.0
    if vol(sk.mash) > 0.5:
        var pm = takePart(sk.mash, Lq, 1.0)
        out_L += vol(pm);Lq -= vol(pm);S.drainExt += ext(pm) + pm.su + pm.sc + pm.ni
    for i in [0, 1, 2, 3, 4]:
        if Lq <= 0.001: break
        var tk = takeCell(sk.cells[i], Lq)
        var tv: float = cellVol(tk);S.drainExt += tk.f + tk.n
        out_L += tv;Lq -= tv
    var dg: = 0.0
    if sk.g > 0.01:
        dg = minf(sk.g, 6.0 * (4.0 if sk.rake else 1.0) * m)
        sk.g -= dg;sk.lostExt += 0.0
        if sk.g < 0.01: sk.g = 0.0
    if out_L > 0.0 or dg > 0.0:
        S.spillL += out_L
        var hs = items_by_bind("sk")
        if hs.size():
            for it in net.byId.values():
                if it.type == "deco" and it.get("k", "") == "hatch": S.leaks.append(Vector2(it.x, it.y + 24));break
        once("hatch%d" % tb(900), "Výhoz mláta je otevřený: kapalina odtéká do kanálu%s." % [", mláto vypadává" if sk.g > 0.01 or dg > 0.0 else ""], "warn")



func _vessel_gravity(m: float) -> void :
    for gb in ["rvp", "sk", "hlt", "wp", "spilka"]:
        var gvl = items_by_bind(gb)
        if not gvl.size() or _src_level(gb) < 8.0: continue
        var gv = gvl[0]
        var sts: Array = starts_of(gv.id, true)
        if not sts.size(): continue
        var oy: = -1000000000.0
        for sk_ in sts: oy = maxf(oy, net.nodes[sk_].y)
        var sy: float = main.level_y(gv)
        if sy >= oy: continue
        var gr = bfs(sts)
        var bh = null; var bhd: = 0.03
        for h in gr.hits:
            var db: String = h.att.bind
            if db == gb or db in ["lv", "vodovod", "mp"]: continue
            var dvl = items_by_bind(db)
            if not dvl.size(): continue
            var ref_y: float = minf(main.level_y(dvl[0]), net.nodes[h.k].y)
            var dh: float = (ref_y - sy) / PX_PER_M
            if dh > bhd: bhd = dh;bh = h
        if bh == null: continue
        var db2: String = bh.att.bind
        var dv2 = items_by_bind(db2)[0]
        var pa = path_to(gr, bh.k)
        var f: float = path_pct(pa) / sqrt(1.0 + path_len(pa) / 6000.0)
        if f <= 0.0: continue
        var q: float = 120.0 * f * minf(1.3, sqrt(bhd / 1.05))
        var th: = 0.0
        if gb == "rvp": th = thick_factor(S.rvp) * (1.0 - 0.85 * S.clog.get("rvp", 0.0))
        elif gb == "sk" and vol(S.sk.mash) > 5: th = thick_factor(S.sk.mash) * (1.0 - 0.85 * S.clog.get("sk", 0.0))
        q /= 1.0 + 0.5 * th
        var As: float = _src_cap(gb) / 1000.0 / maxf(0.5, gv.h / PX_PER_M)
        var Ad: float = _src_cap(db2) / 1000.0 / maxf(0.5, dv2.h / PX_PER_M)
        var v_eq: float = bhd * (As * Ad / (As + Ad)) * 1000.0
        var room: float = _src_cap(db2) - _src_level(db2)
        var Lg: float = minf(q * m, minf(v_eq * 0.6, room))
        if Lg < 0.02: continue
        var gp = srcTake(gb, Lg / m, m, "drain")
        if gp == null: continue
        var gout = push_path(pa, gp, Lg / m, m)
        _stream(bh, gout)
        dstGive(db2, gout, {"src": gb, "turb": S.sk.turb, "m": m})
        once("gravx%s%s%d" % [gb, db2, tb(600)], "%s → %s: kapalina přetéká samospádem otevřenými klapkami (hladina v %s je výš). Zavři klapku, jestli nemá téct." % [VN(gb), VN(db2), VN(gb)], "warn")



func _dead_end_plan(r: Dictionary, used: Dictionary, rate: float, m: float) -> Array:
    var out: = []
    for e in net.edges:
        if e.medium == "steam" or used.has(e.id): continue
        var a_in: bool = r.prev.has(e.a); var b_in: bool = r.prev.has(e.b)
        if not (a_in or b_in): continue
        if not (a_in and b_in):
            var other = net.nodes[e.b if a_in else e.a]
            if other.valve == null or valve_open(other.valve): continue
        if ext(e.c) > 0.05: continue
        var room: float = e.cap - vol(e.c)
        if room < 0.01: continue
        out.append([e, minf(room, rate * m)])
    return out

var _dead_acc: = {}

func _dead_due(key: String, m: float) -> float:
    var a: float = _dead_acc.get(key, 0.0) + m
    if a < 2.0 / 60.0:
        _dead_acc[key] = a;return 0.0
    _dead_acc[key] = 0.0
    return a

func _dead_end_apply(plan: Array, T: float) -> void :
    for x in plan:
        var part = lp();part.w = x[1];part.T = T
        edge_push(x[0], part, true)


## ochrana (interlock) před startem čerpadla: otevřené sání z nádoby s kapalinou, otevřený výtlak do nádoby, zavodněné těleso
func pump_interlock(key: String) -> String:
    var pis = items_by_bind(key)
    if not pis.size(): return ""
    var pn = net.pumpNodes.get(pis[0].id, {"in": [], "out": []})
    var rs = bfs(pn. in ); var src_ok: = false
    for hh in rs.hits:
        if hh.att.low and hh.att.bind in ["rvp", "mp", "sk", "wp", "hlt", "spilka"] and _src_level(hh.att.bind) > 5: src_ok = true
    if not src_ok: return "sání není otevřené z nádoby s kapalinou – čerpadlo by běželo nasucho a zavzdušnilo se. Otevři klapky před čerpadlem."
    var rd = bfs(pn.out); var dst_ok: = false
    for d in rd.hits:
        if d.att.bind != "vodovod": dst_ok = true
    if not dst_ok and rd.leaks.size() == 0: return "výtlak je zavřený – čerpadlo by tlačilo do zavřeného potrubí (tlaková špička, ohřev ucpávky). Otevři cestu za čerpadlem."
    return ""

## varování (čerpadlo se pustí, ale obsluha má vědět)
func pump_warn(key: String) -> String:
    var pis = items_by_bind(key)
    if not pis.size(): return ""
    var pn = net.pumpNodes.get(pis[0].id, {"in": [], "out": []})
    if bfs(pn.out).leaks.size(): return "výtlak vede i do otevřeného konce trubky – část poteče na zem."
    return ""

func _pump_prime_idle(key: String, q: Dictionary, el: float) -> void :
    if q.get("airLock", false): return
    var pis = items_by_bind(key)
    if not pis.size(): return
    var pumpIt = pis[0]
    var pn = net.pumpNodes.get(pumpIt.id, {"in": [], "out": []})
    var rs = bfs(pn. in )
    var src = null
    for hh in rs.hits:
        if hh.att.low and hh.att.bind in ["rvp", "mp", "sk", "wp", "hlt", "spilka"] and _src_level(hh.att.bind) > 5:
            src = hh;break
    if src == null: return
    var sb: String = src.att.bind; var sv = net.byId[src.att.id]
    var lvl_m = clampf(_src_level(sb) / maxf(1.0, _src_cap(sb)), 0.0, 1.0) * sv.h / PX_PER_M
    var head = (pumpIt.y - (sv.y + sv.h)) / PX_PER_M + lvl_m
    if head < 0.3: return
    var th: float = 0.0
    if sb == "rvp": th = thick_factor(S.rvp)
    elif sb == "sk" and vol(S.sk.mash) > 5: th = thick_factor(S.sk.mash)
    q.prime = minf(1.0, q.prime + el / (6.0 + 10.0 * th))

## kapalina v sací nádobě (pro hustotu a viskozitu)
func _src_fluid(b: String) -> Dictionary:
    match b:
        "rvp", "mp", "wp": return S[b]
        "sk": return S.sk.mash if vol(S.sk.mash) > 5 else {"w": 1000.0, "f": cellP(S.sk.cells[4]) * 10.0, "dx": 0.0, "ld": 0.0, "ns": 0.0, "sg": 0.0, "g": 0.0, "su": 0.0, "sc": 0.0, "ni": 0.0, "T": S.sk.T}
    return {}

## čerpadla: H₀ při 50 Hz [m], jmenovitý průtok [L/min] (modelové hodnoty, nejsou z typových štítků)
const PUMPS: = {"p1": {"H0": 32.0, "Qn": 400.0}, "p2": {"H0": 22.0, "Qn": 150.0}, "p3": {"H0": 26.0, "Qn": 200.0}}
const DN_REAL: = {25.0: 50.0, 40.0: 50.0, 50.0: 75.0}   # DN v plánku → skutečné (Vasil: silné DN75, ostatní DN50)

## tlaková ztráta trasy [m sloupce] při průtoku Q [L/min]
func path_head(path: Array, Q: float, rho: float, mu: float) -> float:
    if Q <= 0.0: return 0.0
    var H: = 0.0; var g: = 9.81; var sc: float = S.cfg.get("pipeLenScale", 0.4)
    for st in path:
        if st.has("plate"):
            H += 4.0 * pow(Q / 200.0, 2);continue
        if not st.has("e"): continue
        var e = st.e
        var D: float = bore_mm(float(e.dn)) / 1000.0
        var A: float = PI * D * D / 4.0; var v: float = Q / 60000.0 / A
        var Re: float = rho * 1000.0 * v * D / maxf(1e-05, mu)
        var fr: float = 64.0 / maxf(1.0, Re) if Re < 2300.0 else 0.25 / pow(log(1.5e-05 / (3.7 * D) + 5.74 / pow(Re, 0.9)) / log(10.0), 2)
        var Lm: float = e.len / PX_PER_M * sc
        var K: = 0.4
        if st.has("to"):
            var n = net.nodes.get(st.to)
            if n != null and n.valve != null:
                var vv = net.byId.get(n.valve)
                var p: float = 1.0
                if vv != null and vv.get("kind", "") == "skrt": p = clampf(float(vv.get("pct", 50)) / 100.0, 0.05, 1.0)
                elif vv != null and vv.has("part"): p = clampf(float(vv.part) / 100.0, 0.05, 1.0)
                K += 0.6 * pow(1.0 / p, 4)
        H += (fr * Lm / D + K) * v * v / (2.0 * g)
    return H

## vnitřní průměr potrubí [mm] podle DN z plánku – kalibrovatelné (bore25/40/50)
func bore_mm(dn: float) -> float:
    var c = S.cfg
    if dn <= 25.0: return c.get("bore25", 50.0)
    if dn <= 40.0: return c.get("bore40", 50.0)
    if dn <= 50.0: return c.get("bore50", 75.0)
    return DN_REAL.get(dn, dn)

## pracovní bod: měnič drží nastavený průtok až do 50 Hz; když trasa víc nedovolí, teče míň
func pump_point(key: String, Qset: float, Hst: float, sp_: Array, dpaths: Array, rho: float, mu: float, th: float) -> Dictionary:
    var cfg_ = S.cfg; var kk: String = key if key in ["p1", "p2", "p3"] else "p1"
    var P: Dictionary = {"H0": float(cfg_.get(kk + "_H0", PUMPS[kk].H0)), "Qn": float(cfg_.get(kk + "_Qn", PUMPS[kk].Qn))}
    var H0: float = P.H0 / (1.0 + 0.3 * th)          # hustý rmut: viskózní korekce výtlačné výšky
    var A: float = 0.25 * H0 / (P.Qn * P.Qn)
    var nd: int = maxi(1, dpaths.size())
    var sys = func(Q: float) -> float:
        var hd: = 0.0
        for dp_ in dpaths: hd = maxf(hd, path_head(dp_, Q / nd, rho, mu))
        return Hst + path_head(sp_, Q, rho, mu) + hd
    var Q: float = Qset; var why: = ""
    var Hs_: float = sys.call(Qset)
    if Hs_ > H0 - A * Qset * Qset:
        if sys.call(1.0) > H0:
            Q = 0.0;why = "lift"
        else:
            var lo: = 0.0; var hi: float = Qset
            for i in 30:
                var mid: float = (lo + hi) / 2.0
                if sys.call(mid) > H0 - A * mid * mid: hi = mid
                else: lo = mid
            Q = lo;why = "max"
        Hs_ = sys.call(Q)
    var hz: float = 50.0 * sqrt(clampf((Hs_ + A * Q * Q) / H0, 0.0, 1.0)) if Q > 0 else 0.0
    return {"Q": Q, "hz": hz, "H": Hs_, "bar": rho * 9.81 * maxf(0.0, Hs_) / 100.0, "why": why, "hs_loss": path_head(sp_, Q, rho, mu)}

func _src_temp(b: String) -> float: match b:
        "sk": return S.sk.T if vol(S.sk.mash) < 5 else S.sk.mash.T
        "hlt": return S.hlt.T
        "spilka": return S.sp.H / maxf(1.0, S.sp.L)
        _: return S[b].T

func _src_cap(b: String) -> float:
    var c = S.cfg
    match b:
        "sk": return c.skCap
        "hlt": return c.hltCap
        "spilka": return c.spCap
        "rvp": return c.rvpCap
        "wp": return c.wpCap
        _: return c.mpCap

func _src_level(b: String) -> float:
    match b:
        "sk": return skVolTotal()
        "hlt": return S.hlt.w
        "spilka": return S.sp.L
        _: return vol(S[b])


func _stream(d: Dictionary, p) -> void :
    var L = vol(p)
    if L <= 1e-06: return
    var k: String = d.k
    if S.streams.has(k): S.streams[k].L += L
    else: S.streams[k] = {"L": L, "kind": contentKind(p), "T": p.T}

func setup(items: Array, joints: Array) -> void :
    new_state()
    training = Training.new(self)
    lauter_auto = LauterAuto.new(self)
    proc = Proc.new(self);proc.init_state()
    build_net(items, joints)
    var R: Dictionary = RECIPES[clampi(recipe_idx, 0, RECIPES.size() - 1)]
    logm("Kontrola receptu: " + recipe_check(R))
    if calib_applied.size(): logm("Kalibrace z kalibrace.json: %s" % ", ".join(PackedStringArray(calib_applied)))
    if calib_rejected.size(): logm("Kalibrace – odmítnuto: %s" % "; ".join(PackedStringArray(calib_rejected)), "warn")
