extends RefCounted
## Registr parametrů modelu: jednotka, typ zdroje, nejistota, co je změří.
## Typy: MEASURED (změřeno na varně), MANUFACTURER (doklad / štítek / údaj provozovatele), LITERATURE,
## ESTIMATED (technický odhad), CALIBRATED (upraveno podle měření), UNKNOWN.
## Hodnoty jsou v sim.gd → CFG_DEFAULT; přepsat je jde souborem user://kalibrace.json (viz sim.apply_calibration).

const META: = {
	"rvpCap": ["L", "MANUFACTURER", "±5 %", "nominální objem pánve 50 hl (údaj provozovatele)"],
	"rvpSteel": ["kg oceli", "ESTIMATED", "±30 %", "hmotnost nádoby; kapacita = kg × cpSteel – z výkresu nebo štítku"],
	"cpSteel": ["kJ/(kg·K)", "LITERATURE", "0,46–0,51", "měrná tepelná kapacita austenitické nerezi (AISI 304); jediné místo převodu sim.steelC()"],
	"hltSteel": ["kg oceli", "ESTIMATED", "±50 %", "bojler: dříve natvrdo 300 kJ/K → 600 kg × 0,5"],
	"hltUA": ["kW/K", "ESTIMATED", "±100 %", "ztráty izolovaného bojleru – změřit chladnutí přes noc"],
	"mpSteel": ["kg oceli", "ESTIMATED", "±30 %", "mladinová pánev (v plánku nepoužitá)"], "wpSteel": ["kg oceli", "ESTIMATED", "±30 %", "vířivá káď"], "skSteel": ["kg oceli", "ESTIMATED", "±30 %", "scezovací káď včetně falešného dna"],
	"mpUA": ["kW/K", "ESTIMATED", "±50 %", "ztráty mladinové pánve"], "skUA": ["kW/K", "ESTIMATED", "±50 %", "ztráty SK"], "skContactUA": ["kW/K", "ESTIMATED", "±50 %", "přestup náplň ↔ stěna SK"],
	"mpUAst": ["kW/K", "ESTIMATED", "±40 %", "pára → náplň mladinové pánve"], "mpJacketL": ["L", "ESTIMATED", "±20 %", "smáčení pláště mladinové pánve"],
	"mpCap": ["L", "ESTIMATED", "±10 %", "mladinová pánev"], "mpDead": ["L", "ESTIMATED", "±50 %", "mrtvý objem"], "wpDead": ["L", "ESTIMATED", "±50 %", "kaly + mrtvý objem vířivky"],
	"skMuP": ["1/°P", "LITERATURE", "0,04–0,055", "μ_sladiny/μ_vody = e^(skMuP·°P); EBC kongresní sladina 8,6 % má ~1,4–1,7 mPa·s při 20 °C (≈1,5× vody)"],
	"skSuction": ["Pa", "ESTIMATED", "±50 %", "největší podtlak pod sítem, který C2 vytvoří – vakuometr pod sítem"],
	"spargeEndP": ["°P", "LITERATURE", "0,7–1 % (Kosa s. 147, 156)", "konec vyslazování (poslední podíl)"],
	"pivLoad": ["kW", "ESTIMATED", "±50 %", "tepelná zátěž okruhu ledové vody"],
	"hltStart": ["L", "ESTIMATED", "-", "počáteční náplň bojleru"], "hltT0": ["°C", "ESTIMATED", "-", "počáteční teplota bojleru"],
	"lvStart": ["L", "ESTIMATED", "-", "počáteční náplň ledovky"], "lvT0": ["°C", "ESTIMATED", "-", "počáteční teplota ledovky"],
	"bore25": ["mm", "ESTIMATED", "±10 %", "vnitřní průměr trubek DN25 z plánku (Vasil: DN50)"], "bore40": ["mm", "ESTIMATED", "±10 %", "vnitřní průměr DN40 z plánku (DN50)"], "bore50": ["mm", "ESTIMATED", "±10 %", "vnitřní průměr DN50 z plánku (silné DN75)"],
	"p1_H0": ["m", "ESTIMATED", "±40 %", "C1 výtlačná výška při nulovém průtoku – štítek/křivka"], "p1_Qn": ["L/min", "ESTIMATED", "±40 %", "C1 jmenovitý průtok"],
	"p2_H0": ["m", "ESTIMATED", "±40 %", "C2"], "p2_Qn": ["L/min", "ESTIMATED", "±40 %", "C2"], "p3_H0": ["m", "ESTIMATED", "±40 %", "C3"], "p3_Qn": ["L/min", "ESTIMATED", "±40 %", "C3"],
	"cipLyeMin": ["% NaOH", "ESTIMATED", "1–2 %", "minimální koncentrace louhu podle provozního předpisu – doplnit podle přípravku"],
	"cipAcidMin": ["%", "ESTIMATED", "0,5–1 %", "minimální koncentrace kyseliny – podle přípravku"],
	"rvpUA": ["kW/K", "ESTIMATED", "±50 %", "ztráty pánve do okolí – změřit chladnutí plné pánve bez páry"],
	"rvpUAst": ["kW/K", "ESTIMATED", "±40 %", "přestup pára → náplň; kalibrovat z rychlosti ohřevu (K/min) při známém tlaku páry"],
	"rvpJacketL": ["L", "ESTIMATED", "±20 %", "objem náplně, nad kterým je plášť celý smáčený"],
	"rvpDead": ["L", "ESTIMATED", "±50 %", "nevyčerpatelný zbytek ve výpusti"],
	"wpCap": ["L", "ESTIMATED", "±10 %", "objem vířivé kádě"],
	"wpUA": ["kW/K", "ESTIMATED", "±50 %", "ztráty vířivky"],
	"skCap": ["L", "ESTIMATED", "±10 %", "objem scezovací kádě"],
	"skArea": ["m²", "MANUFACTURER", "±3 %", "plocha falešného dna 5,3 m² (údaj provozovatele)"],
	"skRetentionLkg": ["L/kg sladu", "CALIBRATED", "0,8–1,6", "sladina zadržená v mlátě; průmyslové mláto má 20–25 % sušiny ≈ 0,8–1,0 L/kg; 1,0 sedí na údaj Vasila 700 kg → 40 hl / ~12,5 °P. Změřit: objem sladiny, který zůstane po scezení"],
	"skPerm": ["m²", "CALIBRATED (na referenční scénář, ne na měření)", "řád velikosti", "1,5e-11 ≈ 15 darcy; nastaveno tak, aby 40 L/min dalo Δp ~50 mbar; kalibrovat z Δp a průtoku C2"],
	"skDpComp": ["Pa", "ESTIMATED", "±50 %", "stlačitelnost koláče k = k₀ / (1 + Δp / skDpComp)"],
	"skTeigRho": ["kg/m³", "ESTIMATED", "±50 %", "hustota (sušina) horního těsta"],
	"skTeigK": ["-", "ESTIMATED", "×3", "propustnost těsta vůči mlátu"],
	"skCreepK": ["1/min", "ESTIMATED", "×3", "rychlost pomalého sedání lože"],
	"skDpCompact": ["mbar", "ESTIMATED", "60–120", "práh rychlého slehávání – kalibrovat z provozu (tlakoměr pod sítem)"],
	"skCompK": ["1/min", "ESTIMATED", "×3", "rychlost slehávání nad prahem"],
	"skDpChannel": ["mbar", "ESTIMATED", "120–250", "práh vzniku kanálků"],
	"skChanK": ["1/min", "ESTIMATED", "×3", "rychlost tvorby kanálků"],
	"spCap": ["L", "ESTIMATED", "±10 %", "objem spilky"],
	"waterFlow": ["L/min", "ESTIMATED", "±30 %", "průtok čerpadla horké vody C4 do pánve – změřit vodoměrem / časem napouštění"],
	"mainsQ": ["L/min", "ESTIMATED", "±40 %", "průtok při volném výtoku z řadu bez další trasy (kapacita přívodu) – změřit kbelíkem/vodoměrem u řadu"],
	"mainsBar": ["bar přetlaku", "ESTIMATED", "3–5 bar", "statický tlak vodovodního řadu v budově (manometr na přívodu při zavřených odběrech) – tlak pod zátěží se dopočte z přívodu a trasy"],
	"iboKeep": ["-", "ESTIMATED", "0,3–0,7", "podíl izo-α-kyselin z varu, který zůstane v hotové mladině a změří se jako IBU (ztráty na kalech, pěně, adsorpce na hořké zlomy) – kalibrovat analýzou IBU z várky"],
	"boilMin": ["min", "RECEPT", "podle receptu 60–120", "PROVOZNÍ NASTAVENÍ: délka varu mladiny, bere se z receptu (RECIPES.boil) a řídí časový plán chmelení a odpar; není to fyzikální konstanta. Ověření: technologický předpis varny"],
	"pitchT": ["°C", "RECEPT", "±0,5 °C", "CÍL RECEPTU: teplota mladiny při zakvašení; v modelu vstupuje jen do hodnocení várky (−25 bodů za 1 °C odchylky), chlazení neovlivňuje. Ověření: teploměr na výtoku z chladiče"],
	"targetEBC": ["EBC", "RECEPT", "±1 EBC", "CÍL RECEPTU: barva piva; porovnává se s barvou z podílů sladů (hodnocení), fyziku neovlivňuje. Ověření: kolorimetr EBC/ASBC"],
	"targetIBU": ["IBU", "RECEPT", "±2 IBU", "CÍL RECEPTU: hořkost; porovnává se s IBU z modelu (izo-α-kyseliny × iboKeep). Ověření: laboratorní IBU z hotové várky"],
	"targetP": ["°P", "RECEPT", "±0,2 °P", "CÍL RECEPTU: stupňovitost vyrážky; vstup kontroly receptu (potřebný slad) a hodnocení, fyziku neovlivňuje. Ověření: sacharometr / hustoměr, vyrážka při teplotě měření"],
	"lvUA": ["kW/K", "ESTIMATED", "±50 %", "ztráty tepla ledové vody do okolí při plné ledovce (Q = lvUA·(T_okolí − T)·plnost); dříve vepsáno v kódu. Ověření: nárůst teploty ledovky bez agregátu za noc"],
	"skBedLkg": ["L/kg sladu", "ESTIMATED", "±25 %", "objem vlhkého lože mláta na kg sladu (výška lože = slad·skBedLkg/skArea); dříve vepsáno v kódu. Ověření: změřit výšku lože při známém sypání"],
	"ferMu": ["1/h", "ESTIMATED", "×2", "KVAŠENÍ (vlastní fenomenologický model, neověřený měřením): maximální růst kvasnic při 12 °C. Ověření: počty buněk během kvašení"],
	"ferQ": ["g/(L·h)", "ESTIMATED", "×2", "KVAŠENÍ: rychlost příjmu extraktu na jednotku kvasnic (X) při 12 °C; ladí délku kvašení. Ověření: denní sacharometrie"],
	"ferKsMu": ["g/L", "ESTIMATED", "×3", "KVAŠENÍ: Monodova konstanta růstu"],
	"ferKsQ": ["g/L", "ESTIMATED", "×3", "KVAŠENÍ: Monodova konstanta příjmu extraktu"],
	"ferXmax": ["g/L", "ESTIMATED", "×2", "KVAŠENÍ: strop koncentrace kvasnic (ekvivalent suchých buněk)"],
	"ferTa": ["1/K", "ESTIMATED", "0,07–0,12", "KVAŠENÍ: teplotní faktor rychlosti e^(ferTa·(T−12))"],
	"ferHeat": ["kJ/g extraktu", "LITERATURE", "0,55–0,65", "KVAŠENÍ: reakční teplo zkvašeného extraktu (~600 kJ/kg); do T tanku jde dS·ferHeat/(4,1 kJ/(L·K))"],
	"ferCool": ["1/h", "ESTIMATED", "×3", "KVAŠENÍ: rychlost přiblížení teploty tanku k nastavené (chlazení pláště). Ověření: pokles teploty po zapnutí chlazení"],
	"ferDiaK": ["1/h", "ESTIMATED", "×3", "KVAŠENÍ: přeměna α-acetolaktátu na diacetyl při 12 °C"],
	"ferDiaT": ["1/K", "ESTIMATED", "0,08–0,14", "KVAŠENÍ: teplotní závislost přeměny na diacetyl"],
	"ferDiaRed": ["1/h", "ESTIMATED", "×3", "KVAŠENÍ: redukce diacetylu kvasnicemi při 12 °C"],
	"cipHeat": ["K/min", "ESTIMATED", "±50 %", "CIP: rychlost ohřevu roztoku stanicí. Ověření: stopky + teploměr na zpátečce"],
	"cipLyeT": ["°C", "ESTIMATED", "provozní předpis", "CIP: minimální teplota louhu pro platný krok postupu (hodnotí se POSTUP, ne ověřená čistota)"],
	"cipAcidT": ["°C", "ESTIMATED", "provozní předpis", "CIP: maximální teplota kyseliny (koroze nerezu)"],
	"cipMin": ["min", "ESTIMATED", "provozní předpis", "CIP: doba kontaktu louhu/kyseliny, po které se krok uzná"],
	"cipRinseMin": ["min", "ESTIMATED", "provozní předpis", "CIP: doba proplachu vodou"],
	"whirlTau": ["min", "ESTIMATED", "×2", "čas, za který se ve vířivce ustaví stabilní rotace (čajový efekt) – změřit odběrem vzorků kalu po různé době víření"],
	"trubKstill": ["1/min", "ESTIMATED", "×3", "rychlost usazování kalů bez víření (poločas ≈ 1 h) – změřit turbiditou/hmotností kalu v klidové nádobě"],
	"trubKwhirl": ["1/min", "ESTIMATED", "×2", "rychlost sběru kalů do kužele při plném víření (poločas ≈ 3,5 min) – změřit hmotností kalu a zákalem mladiny po usazení"],
	"maltRate": ["kg/min", "ESTIMATED", "±30 %", "výkon šrotovníku / vystírání"],
	"coolWaterT": ["°C", "ESTIMATED", "8–18 °C podle ročního období", "teplota vody z řadu"],
	"UA1": ["kW/K", "ESTIMATED", "±40 %", "1. stupeň deskového chladiče (voda) – změřit vstupy/výstupy a průtoky"],
	"UA2": ["kW/K", "ESTIMATED", "±40 %", "2. stupeň (ledová voda)"],
	"boilerKW": ["kW", "MANUFACTURER", "jmenovitý", "parní kotel 650 kW (údaj provozovatele) – skutečný výkon závisí na tlaku a odběru"],
	"steamBar": ["bar přetlaku", "MANUFACTURER", "±0,3 bar", "tlak páry 2,5 bar g; sytostní teplota se počítá z absolutního tlaku (p + 1,013)"],
	"hltCap": ["L", "MANUFACTURER", "±5 %", "bojler 120 hl"],
	"hltSet": ["°C", "MANUFACTURER", "-", "nastavení termostatu 83 °C"],
	"hltUAst": ["kW/K", "ESTIMATED", "±50 %", "ohřev bojleru párou"],
	"lvVol": ["L", "MANUFACTURER", "±5 %", "ledovka 100 hl"],
	"chillerKW": ["kW", "MANUFACTURER", "jmenovitý", "glykolový agregát 45 kW – skutečný výkon klesá s teplotou okolí"],
	"lvSet": ["°C", "MANUFACTURER", "-", "nastavení ledové vody 2 °C"],
	"tbWater": ["°C", "LITERATURE", "±0,3 °C", "bod varu vody pro nadmořskou výšku varny (99,3 °C ≈ 200 m n. m.) – upravit podle lokality"],
	"malt": ["kg", "MANUFACTURER", "-", "sypání podle receptu"],
	"maltExtract": ["% v sušině", "LITERATURE", "±2 %", "jen záloha pro neznámý slad – extrakt se počítá z tabulky MALTS (atest sladovny)"],
	"maltMoist": ["%", "LITERATURE", "±1 %", "jen záloha – vlhkost se bere z MALTS (atest)"],
	"mill": ["stupeň 1–3", "ESTIMATED", "-", "jemnost šrotu (1 = jemný → propustnost 0,6×, 3 = hrubý → 1,5×)"],
	"pipeLenScale": ["-", "ESTIMATED", "±50 %", "skutečná délka tras / délka v plánku (100 px = 1 m) – změřit trasy"],
	"ambientT": ["°C", "ESTIMATED", "±5 °C", "teplota ve varně"],
	"targetV": ["hl", "RECEPT", "-", "cílový objem várky (cíl, ne fyzikální parametr)"],
	"mashFilmK": ["-", "CALIBRATED (na referenční scénář, ne na měření)", "0–30", "zhoršení přestupu tepla do hustého rmutu: hA = hA_ref/(1 + K·hustota). Nastaveno tak, aby plná pánev rmutu hřála ~1 K/min; odkaz na patent v dřívější poznámce nebyl ověřen. Var mladiny tím ovlivněn není. Ověřit: změřit rychlost ohřevu rmutu (K/min) při známém tlaku páry"],
	"betaAct": ["-", "CALIBRATED (na referenční scénář, ne na měření)", "×2", "násobek rychlosti β-amylázy; nastaven tak, aby standardní dekokce dala zdánlivé prokvašení ~76 % – je to ladění na očekávaný výsledek, ne naměřená konstanta. Kinetika (teplota, pH, substrát, inaktivace) se přesto počítá, takže výsledek závisí na postupu. Ověřit: rozbor sladiny z várky (zdánlivý a konečný extrakt, zkvasitelnost)"],
	"kdAlpha63": ["1/min", "LITERATURE", "–", "inaktivace α-amylázy při 63 °C, Ea 83,7 kJ/mol (Marc et al. 1983, Biotechnol. Bioeng. 25:481) – exponent ze skenu, ověřit"],
	"kdBeta63": ["1/min", "LITERATURE", "–", "inaktivace β-amylázy při 63 °C, Ea 188 kJ/mol (Marc 1983); 13 % stabilní podíl 40× pomalejší (De Schepper et al. 2022)"],
	"plateRefQ": ["L/min", "ESTIMATED", "10–500", "průtok mladiny, při kterém platí UA1/UA2; prostup roste s průtokem ∝ Q^0,65 (korelace Nu–Re pro deskové výměníky)"],
	"boilerWaterKg": ["kg", "ESTIMATED", "50–20000", "zásoba syté vody v kotli – určuje, jak rychle klesá tlak při přetížení (dh'/dp ≈ 57 kJ/kg·bar); změřit/zjistit ze štítku kotle"],
	"preboilV": ["L (horká)", "STATED", "2000–5000", "plná pánev před varem; nastaveno tak, aby vyrážka byla 40 hl při ~7 %/h odparu (Vasil: 700 kg → 40 hl, 12,5 °P)"],
}

const PUMPS_META: = {
	"p1": ["H0 32 m, Qn 400 L/min", "ESTIMATED", "štítek a křivka čerpadla C1 chybí"],
	"p2": ["H0 22 m, Qn 150 L/min", "ESTIMATED", "štítek a křivka C2 chybí"],
	"p3": ["H0 26 m, Qn 200 L/min", "ESTIMATED", "štítek a křivka C3 chybí"],
	"DN": ["DN25/40 → Ø50 mm, DN50 → Ø75 mm vnitřní", "ESTIMATED", "vnitřní průměry podle Vasila (silné DN75, ostatní DN50) – ověřit tloušťku stěny"],
}

static func markdown(cfg: Dictionary) -> String:
	var out := "# Parametry modelu – zdroj a nejistota\n\nTypy: MEASURED (změřeno), MANUFACTURER (doklad / údaj provozovatele), LITERATURE, ESTIMATED (odhad), CALIBRATED, UNKNOWN.\nPřepsat hodnotu jde souborem `kalibrace.json` ve složce dat aplikace (stejná jako uložená várka), např. `{\"rvpUAst\": {\"value\": 7.2, \"source\": \"CALIBRATED\"}}`.\n\n| Parametr | Hodnota | Jednotka | Zdroj | Nejistota | Poznámka / jak změřit |\n|---|---|---|---|---|---|\n"
	var keys := META.keys(); keys.sort()
	for k in keys:
		var m: Array = META[k]
		out += "| %s | %s | %s | %s | %s | %s |\n" % [k, str(cfg.get(k, "?")), m[0], m[1], m[2], m[3]]
	out += "\n## Čerpadla a potrubí\n\n| Prvek | Hodnota | Zdroj | Poznámka |\n|---|---|---|---|\n"
	for k in PUMPS_META:
		var p: Array = PUMPS_META[k]
		out += "| %s | %s | %s | %s |\n" % [k, p[0], p[1], p[2]]
	var miss := []
	for k in cfg:
		if cfg[k] is float and not META.has(k): miss.append(k)
	if miss.size(): out += "\nBez popisu (ESTIMATED, doplnit): %s\n" % ", ".join(PackedStringArray(miss))
	return out
