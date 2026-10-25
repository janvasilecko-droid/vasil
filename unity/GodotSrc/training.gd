extends RefCounted



var simulation_ref: WeakRef
var sim:
    get: return simulation_ref.get_ref()
const STEPS: = [
    ["inspection", "Kontrola varny", "Prohlédni čistotu nádob, síta, hadice a klapky. Sanitaci nádob a chladiče proveď v sekci CIP (louh ≥ 75 °C, kyselina ≤ 60 °C), nebo potvrď vlastní kontrolu.", 0], 
    ["steam", "Připravit páru a horkou vodu", "Zapni rozvaděč a kotel. Bojler musí obsahovat nejméně 25 hl vody při alespoň 78 °C. Ledovku můžeš připravovat souběžně.", 0], 
    ["fill", "Napustit vystírací vodu", "Bojler → pánev přes C4. Zapni směšovač na 38 °C, napusť dávku z receptu a napouštění zastav. Samotný bojler dává horkou vodu, nikoli požadovanou směs.", 0], 
    ["malt", "Zapnout míchadlo a vystřít slad", "Míchadlo musí běžet. Postupně nasyp celou dávku 700 kg; udrž vystírku přibližně 35–45 °C.", 0], 
    ["rest52", "Ohřát dílo na 50 °C", "S míchadlem pozvolna ohřej a drž 48–52 °C po dobu 15 minut. Při překročení pásma se tato souvislá prodleva počítá znovu.", 15], 
    ["prime", "Prohřát SK a podrazit podsítí", "Přiveď horkou vodu pod síta: alespoň 120 L před příchodem mláta. Prohřej stěny SK nad 55 °C. Během přípravy drž hlavní dílo kolem 50 °C. Zavři výpustě; voda nesmí před odrmutováním utéct.", 0], 
    ["split1", "Oddělit první rmut", "C1 přečerpá polovinu díla do SK (bez filtračního lože – SK je ve stavu „rmut“), rmut k povaření zůstane v pánvi. Mimo používanou cestu klapky zavři. Postup: míchadlo na vysoké otáčky (hustý rmut se jinak usadí a ucpe spodní výpust), otevři klapky cesty (nejdřív před čerpadlem, nech čerpadlo zavodnit), potom za čerpadlem, teprve pak čerpadlo zapni. Na konci čerpadlo vypni, zavři klapku za ním a pak před ním.", 0], 
    ["rest63a", "První rmut: 63 °C", "V pánvi drž 61–65 °C s míchadlem 20 minut. Zbytek díla v SK nevař: musí v něm zůstat živé enzymy.", 20], 
    ["rest72a", "První rmut: 72 °C", "Ohřej na 70–74 °C a drž 20 minut s míchadlem.", 20], 
    ["boil1", "První rmut: povařit", "Přiveď rmut k varu a povař jej 10 minut. Míchadlo musí běžet, jinak se rmut připaluje.", 10], 
    ["return1", "Spojit první rmut", "Vypni páru. C1: pánev → SK. Přečerpej povařený podíl ke zbytku a C1 zastav; sleduj výslednou teplotu. Nepouštěj zatím scezovací C2. Postup: míchadlo na vysoké otáčky (hustý rmut se jinak usadí a ucpe spodní výpust), otevři klapky cesty (nejdřív před čerpadlem, nech čerpadlo zavodnit), potom za čerpadlem, teprve pak čerpadlo zapni. Na konci čerpadlo vypni, zavři klapku za ním a pak před ním.", 0], 
    ["split2", "Oddělit druhý rmut", "C1: SK → pánev, cesta pro dílo. Přečerpej dvě třetiny díla do pánve; C1 zastav. C2 je určené pro kapalinu pod síty. Postup: míchadlo na vysoké otáčky (hustý rmut se jinak usadí a ucpe spodní výpust), otevři klapky cesty (nejdřív před čerpadlem, nech čerpadlo zavodnit), potom za čerpadlem, teprve pak čerpadlo zapni. Na konci čerpadlo vypni, zavři klapku za ním a pak před ním.", 0], 
    ["rest63b", "Druhý rmut: 63 °C", "Drž 61–65 °C 20 minut s míchadlem. Pouhé čekání při jiné teplotě se za prodlevu nepočítá.", 20], 
    ["rest72b", "Druhý rmut: 72 °C", "S míchadlem drž 70–74 °C 20 minut. Nepřekračuj teplotu předčasně.", 20], 
    ["boil2", "Druhý rmut: povařit", "Povař druhý podíl 10 minut s míchadlem, potom páru vypni.", 10], 
    ["return2", "Spojit druhý rmut", "C1: pánev → SK. Po úplném vrácení druhého rmutu C1 zastav. Cílem je dílo přibližně 75–78 °C.", 0], 
    ["conversion", "Odrmutovací teplota a zcukření", "V SK drž dílo 74–78 °C 10 minut a ověř negativní jodovou zkoušku. Nezcukřený rmut po zničení enzymů nelze zachránit dalším ohřevem.", 10], 
    ["bed", "Odrmutovat: vytvořit filtrační lože", "Stiskni Odrmutovat a vytvořit lože. Vypni C1, C2 a kopačku; před prvním odběrem nech mláto usadit.", 0], 
    ["settle", "Nechat lože 20 minut v klidu", "Bez odběru, přítoku a kopačky nech lože vytvořit. Podsítí musí být zaplavené; během klidu nemá protékat voda do kanálu.", 20], 
    ["vorlauf", "Podrážení a vorlauf do SK", "C2: SK → SK. Začni malým odběrem kolem 20–30 L/min. Zvětši průhledítko: vracej proud, dokud není aspoň minutu čirý. Hladinu drž nad ložem.", 1], 
    ["collect", "Scezovat první sladinu do pánve", "C2: SK → pánev až po vyčiření. Udrž přibližně 5–15 cm kapaliny nad mlátem. Klesá-li hladina, začni rovnoměrně vyslazovat vodou 76–78 °C; silný odběr lože utáhne.", 0], 
    ["sparge", "Dokončit vyslazování", "Sleduj průhledítko, objem pánve a extrakt posledních výstřelků. Konči kolem 45 hl před varem nebo při poklesu pod 1,5 °P. Zastav C2 a vyslazovací vodu; další voda pivo pouze ředí.", 0], 
    ["hop0", "Začít chmelovar a první dávku", "Přiveď sladinu k varu, přidej první dávku podle receptu v 0. minutě. Na začátku hlídej pěnu a ubírej páru, aby mladina nepřekypěla.", 0], 
    ["hop40", "Druhá dávka chmele: 40. minuta", "Pokračuj v chmelovaru a přidej druhou dávku v 40. minutě skutečného varu, nikoli času od spuštění simulátoru.", 0], 
    ["hop65", "Třetí dávka chmele: 65. minuta", "Přidej třetí dávku v 65. minutě varu. Zastavený čas var nenahrazuje.", 0], 
    ["boil_end", "Dokončit chmelovar", "Dodrž dobu z receptu, poté vypni páru. Varem klesá objem a stoupá stupňovitost; odstraňuje se DMS.", 0], 
    ["whirlpool", "Přečerpat do vířivé kádě", "C1: pánev → vířivka. Po přečerpání C1 zastav. Proud vytvoří vír, který soustředí kaly. Klapky otvírej a zavírej ve stejném pořadí jako u rmutu: před čerpadlem, zavodnit, za čerpadlem, zapnout; na konci čerpadlo vypnout dřív než klapky.", 0], 
    ["whirl_rest", "Usadit kaly: 20 minut", "Vířivou káď nech v klidu bez přítoku a odběru. Připrav zatím průchozí chladicí okruh.", 20], 
    ["cooling", "Připravit chlazení a spilku", "Ledovka: nejméně 35 % objemu, voda do 4 °C. Pusť C5 a otevři stranu chladiva deskového chladiče. Zkontroluj čistotu spilky; její sanitace se musí provést mimo tento model.", 0], 
    ["cast", "Spílat a sledovat teplotu", "C3: vířivka → chladič → spilka. Průtok dolaď na přibližně 8,5 °C. Příliš rychlý proud se nestačí ochladit. Před nasátím kalů a vzduchu C3 zastav.", 0], 
    ["report", "Vyhodnotit várku", "Porovnej objem, °P, hořkost, DMS, zákal, ztráty a teplotu s receptem. Dokončený postup sám o sobě neznamená kvalitní pivo. Kvašení pak pokračuje ve spilce (sekce Spilka – kvašení, diacetyl).", 0], 
]

const WHY: = {
    "inspection": "Zbytky mláta, kalů nebo louhu v nádobách a hadicích by znehodnotily várku (infekce, pachuti). Kontrola před várkou je levnější než vylitá várka.",
    "steam": "Bez páry se nic neohřeje a bez dost horké vody nejde vystírat ani vyslazovat. Bojler musí mít zásobu na celou várku (vystírka + podrážka + vyslazování).",
    "fill": "Vystírací teplota a poměr voda : slad určují, jak husté je dílo a jak rychle se rozpustí škrob. Příliš horká voda by hned zničila enzymy.",
    "malt": "Slad sypeme do míchané vody, aby se netvořily hrudky – v hrudce zůstane škrob, který se nezcukří (ztráta výtěžnosti, škrobový zákal).",
    "rest52": "Bílkovinná prodleva: proteázy štěpí bílkoviny na menší (pěna, výživa kvasinek), β-glukanázy snižují viskozitu – dílo se pak lépe scezuje.",
    "prime": "Zaplavené podsítí zabrání tomu, aby rmut dosedl na suchá síta a ucpal je; prohřátá káď rmut neochladí.",
    "split1": "Dekokce: část díla povaříme, tím zmazovatí škrob a vrácením zvýšíme teplotu celku bez přepalování enzymů ve zbytku.",
    "rest63a": "63 °C je optimum β-amylázy – vzniká zkvasitelná maltóza (vyšší prokvašení, sušší pivo).",
    "rest72a": "72 °C je optimum α-amylázy – štěpí škrob na dextriny, dokončí zcukření (jodová zkouška negativní), dává plnost.",
    "boil1": "Povaření rozruší škrobová zrna a tvrdé části sladu (lepší výtěžnost), vznikají melanoidiny (barva, chuť) – typické pro český ležák.",
    "return1": "Vrácením povařeného podílu zvedneme teplotu celého díla na další prodlevu – teplota vznikne z tepelné bilance obou částí.",
    "split2": "Druhý rmut: opět oddělíme hustou část, ve zbytku zůstávají živé enzymy pro zcukření.",
    "rest63b": "Druhá maltózová prodleva – další zkvasitelný cukr.",
    "rest72b": "Druhá zcukřovací prodleva – zbytkový škrob se rozštěpí.",
    "boil2": "Druhé povaření – opět lepší rozklad škrobu a barva.",
    "return2": "Spojením dosáhneme odrmutovací teploty ~76–78 °C: nižší viskozita sladiny = rychlejší scezování, enzymy se zastaví.",
    "conversion": "Negativní jodová zkouška = žádný škrob. Škrob v mladině dělá zákal a je to ztracený extrakt.",
    "bed": "Mláto (pluchy) tvoří přírodní filtr. Nejdřív se musí usadit do rovnoměrné vrstvy, jinak sladina teče kalná.",
    "settle": "Klidem se jemné částice usadí nahoře (těsto) a pluchy dole – vznikne filtrační lože.",
    "vorlauf": "První sladina nese jemné částice; vracíme ji na lože, dokud neteče čirá. Kalná sladina = horší chuť a stabilita piva.",
    "collect": "Předek (první sladina) je nejsilnější. Stahujeme pomalu – rychlý odběr vytvoří podtlak pod sítem, lože se stlačí a utáhne.",
    "sparge": "Vyslazování vymývá extrakt z mláta horkou vodou 76–78 °C. Při < 1,5 °P nebo vyšší teplotě/pH se už vyluhují třísloviny (svíravá chuť).",
    "hop0": "Var sterilizuje mladinu, sráží bílkoviny (lom), odpařuje DMS. Hořké α-kyseliny se izomerují jen varem – proto chmel na začátku dává hořkost.",
    "hop40": "Prostřední dávka dává hořkost i část chmelového aroma.",
    "hop65": "Pozdní dávka: kratší var = méně hořkosti, víc aromatických olejů zůstane v mladině.",
    "boil_end": "Dost dlouhý var (receptura) zajistí izomeraci, odpar DMS a vysrážení bílkovin. Krátký var = DMS (vařená zelenina) a slabá hořkost.",
    "whirlpool": "Tangenciální nátok roztočí mladinu, kaly se soustředí do kužele uprostřed (efekt čajových lístků).",
    "whirl_rest": "Klid nechá kaly usadit – čistá mladina pak jde na chladič bez kalů.",
    "cooling": "Ledová voda musí být připravená předem – chlazení musí stihnout celou mladinu na zákvasnou teplotu.",
    "cast": "Rychlé zchlazení na 8–9 °C chrání mladinu před infekcí a připraví ji pro spodní kvasnice; provzdušnění dodá kyslík pro množení.",
    "report": "Porovnáním s receptem se učíš, co v postupu ovlivnilo výsledek (objem, °P, hořkost, výtěžnost, ztráty).",
}

func _init(simulation) -> void :
    simulation_ref = weakref(simulation)

func state() -> Dictionary:
    if not sim.S.has("training"):
        sim.S.training = {"index": 0, "elapsed": 0.0, "done": [], "baseline": {}, "pause": false, "notified": false}
    return sim.S.training

func enter() -> void :
    var data = state()
    data.elapsed = 0.0;data.notified = false;data.baseline = {}

func current() -> Dictionary:
    var data = state()
    if data.index >= STEPS.size(): return {"complete": true, "ready": true, "title": "Postup dokončen", "hint": "Zhodnoť výsledek várky podle protokolu.", "detail": ""}
    var step = STEPS[data.index]
    var S = sim.S; var r = S.rvp; var sk = S.sk
    var condition: = false
    var detail: = ""
    var skip: = false
    match step[0]:
        "inspection": condition = true
        "steam": condition = sim.cab_live() and S.boiler.on and S.hlt.w >= 2500 and S.hlt.T >= 78
        "fill":
            condition = S.rvpWater.added >= S.cfg.water * 0.98 and not S.rvpWater.on and r.T >= 33 and r.T <= 40
            detail = "Voda %.0f / %.0f L, %.1f °C" % [S.rvpWater.added, S.cfg.water, r.T]
        "malt": condition = S.malt.added >= S.malt.target - 1 and r.stir and r.T >= 33 and r.T <= 45
        "rest52": condition = r.g > 20 and r.stir and r.Tsens >= 48 and r.Tsens <= 52
        "prime":
            condition = sk.under >= 120 and sk.Tw >= 55 and not sk.landed
            detail = "Podsítí %.0f L, stěny %.1f °C (minimum 120 L / 55 °C)" % [sk.under, sk.Tw]
        "split1":
            condition = sk.landed and sim.vol(sk.mash) > 500 and not S.p1.on and sim.vol(r) < 0.6 * (sim.vol(r) + sim.vol(sk.mash)) and sim.vol(r) > 800
            detail = "Pánev %.0f L, SK %.0f L (polovina díla zůstává v pánvi)" % [sim.vol(r), sim.vol(sk.mash)]
        "split2":
            condition = sim.vol(r) > 500 and not S.p1.on and sim.vol(sk.mash) > 800 and sim.vol(r) >= 0.6 * (sim.vol(r) + sim.vol(sk.mash)) and sim.vol(r) <= 0.75 * (sim.vol(r) + sim.vol(sk.mash))
            detail = "Pánev %.0f L (dvě třetiny díla), SK %.0f L" % [sim.vol(r), sim.vol(sk.mash)]
        "rest63a", "rest63b":

            if not data.baseline.has("T0") and r.g > 20: data.baseline["T0"] = r.Tsens
            if data.baseline.get("T0", 0.0) > 65.0:
                skip = true;condition = true
                detail = "Rmut měl při převzetí %.1f °C – nad 63 °C se nechladí, pokračuj ohřevem na 72 °C." % data.baseline.T0
            else: condition = r.g > 20 and r.stir and r.Tsens >= 61 and r.Tsens <= 65
        "rest72a", "rest72b": condition = r.g > 20 and r.stir and r.Tsens >= 70 and r.Tsens <= 74
        "boil1", "boil2": condition = r.g > 20 and r.boiling and r.stir
        "return1", "return2": condition = sim.vol(r) <= S.cfg.rvpDead + 5 and sim.vol(sk.mash) > 1000 and not S.p1.on and r.heat == 0
        "conversion":
            condition = sim.vol(sk.mash) > 1000 and sk.mash.T >= 74 and sk.mash.T <= 78 and sim.iodine(sk.mash) == "neg"
            detail = "Dílo v SK %.1f °C; jodová zkouška %s" % [sk.mash.T, str(sim.iodine(sk.mash))]
        "bed": condition = sk.mode == "lauter" and sk.g > 20 and not S.p1.on and not S.p2.on and not sk.rake
        "settle": condition = sk.mode == "lauter" and not sk.rake and not sk.filling and not S.p1.on and not S.p2.on and not S.skWater.on
        "vorlauf":
            var sample = sim.sight_sample(sim.net.byId["inl_mur9u2hg"])
            var return_flow = false
            for flow in S.flows:
                if flow.src == "sk" and flow.dst.has("sk"): return_flow = true
            condition = return_flow and sk.vorlaufL >= 220 and sample.get("flow", 0.0) > 0 and sample.get("turb", 999.0) < 15 and sample.get("kind", "empty") == "wort"
            detail = "Vráceno %.0f L; průhledítko %.0f L/min, zákal %.0f" % [sk.vorlaufL, sample.get("flow", 0.0), sample.get("turb", 0.0)]
        "collect": condition = sim.hotV(r) >= 3000 and sk.mode == "lauter"
        "sparge":
            condition = (sim.hotV(r) >= 4500 or (sk.filt > 200 and sk.outP <= S.cfg.spargeEndP)) and not S.p2.on and not S.skWater.on
            detail = "Pánev %.0f L; poslední výstřelky %.1f °P" % [sim.hotV(r), sk.outP]
        "hop0", "hop40", "hop65":
            var idxs: Array = hop_group(step[0])
            condition = idxs.size() > 0
            for index in idxs:
                if S.hops.size() <= index:
                    condition = false;continue
                var hop = S.hops[index]; var wanted = S.hopPlan[index]
                if not (hop.boil and absf(hop.get("boilMin", -999.0) - wanted.min) <= 2 and absf(hop.kg - wanted.kg) <= 0.05): condition = false
            if idxs.is_empty(): skip = true;condition = true
            detail = "Skutečný var %.1f min; dávky %d / %d" % [r.boilMin, S.hops.size(), S.hopPlan.size()]
        "boil_end": condition = r.boilMin >= S.cfg.boilMin and r.heat == 0
        "whirlpool": condition = sim.vol(r) <= S.cfg.rvpDead + 5 and sim.vol(S.wp) > 2500 and not S.p1.on
        "whirl_rest": condition = S.wp.w > 1000 and not S.wp.whirlNow and not S.p1.on and not S.p3.on
        "cooling": condition = S.lv.w >= S.cfg.lvVol * 0.35 and S.lv.T <= 4 and S.lv.pump and not S.plates.is_empty()
        "cast": condition = S.sp.L > 2000 and not S.p3.on and sim.vol(S.wp) <= S.cfg.wpDead + S.wp.trubSet * 6 + 10
        "report": condition = sim.spReport() != null
    var ready: bool = condition and (skip or step[3] == 0 or data.elapsed >= float(step[3]) * 60.0)
    if step[3] > 0: detail += "\nSouvislá prodleva %.1f / %d min" % [data.elapsed / 60.0, step[3]]
    var title: String = step[1]; var hint: String = step[2].replace("700 kg", "%d kg" % roundi(S.malt.target))
    if step[0] in ["hop0", "hop40", "hop65"]:
        var g: Array = hop_group(step[0]); var names: = []
        for index in g:
            var hp = S.hopPlan[index]; var nm: String = hp.hop
            for h in sim.HOPS: if h.id == hp.hop: nm = h.name
            names.append("%s kg %s v %d. min" % [sim.fmt1(hp.kg), nm, roundi(hp.min)])
        title = "Chmel: " + (", ".join(PackedStringArray(names)) if names.size() else "(recept nemá tuto dávku)")
    return {"complete": false, "ready": ready, "condition": condition, "title": title, "hint": hint, "detail": detail, "id": step[0]}

## dávky chmele podle receptu: první (všechny v čase první dávky), prostřední, poslední
func hop_group(id: String) -> Array:
    var P: Array = sim.S.hopPlan; var n: int = P.size()
    if n == 0: return []
    var first: = []; var last: = []; var mid: = []
    for i in n:
        if P[i].min == P[0].min: first.append(i)
    if n - 1 > first[first.size() - 1]: last = [n - 1]
    for i in n:
        if not (i in first) and not (i in last): mid.append(i)
    return {"hop0": first, "hop40": mid, "hop65": last}[id]

func update(seconds: float) -> void :
    var data = state()
    if data.index >= STEPS.size(): return
    var view = current()
    if STEPS[data.index][3] > 0:
        data.elapsed = data.elapsed + seconds if view.condition else 0.0
    view = current()
    if view.ready and not data.notified:
        data.notified = true
        sim.logm("Krok připraven k potvrzení: " + view.title)
        if data.pause and sim.main != null: sim.main.running = false

func confirm() -> bool:
    var data = state(); var view = current()
    if view.complete or not view.ready: return false
    data.done.append({"id": view.id, "t": sim.S.t})
    sim.logm("Potvrzen krok: " + view.title)
    data.index += 1;enter()
    return true

func text() -> String:
    var data = state(); var view = current()
    var result = "[b]%d / %d: %s[/b]\n%s\n[color=#f2c230]%s[/color]" % [mini(data.index + 1, STEPS.size()), STEPS.size(), view.title, view.hint, view.detail]
    if data.get("why", false) and WHY.has(view.get("id", "")): result += "\n[color=#8fc7ff][b]Proč to děláme?[/b] %s[/color]" % WHY[view.id]
    if not view.complete:
        result += "\n[color=#7ee08a]Splněno – potvrď další krok.[/color]" if view.ready else "\n[color=#eeb772]Čeká na skutečné splnění podmínek.[/color]"
    return result


func list_text() -> String:
    var data = state(); var out: = []
    for i in STEPS.size():
        var mark = "+" if i < data.index else (">" if i == data.index else "·")
        var tt: String = STEPS[i][1]
        if STEPS[i][0] in ["hop0", "hop40", "hop65"]:
            var g: Array = hop_group(STEPS[i][0])
            if g.size(): tt = "Chmel v %s. min" % ", ".join(PackedStringArray(g.map(func(x): return str(roundi(sim.S.hopPlan[x].min)))))
        out.append("%s %d. %s" % [mark, i + 1, tt])
    return "\n".join(PackedStringArray(out))
