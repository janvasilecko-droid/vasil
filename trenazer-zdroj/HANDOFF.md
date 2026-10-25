# Trenažér varny Zajíc – předávací dokument pro dalšího Clauda
> **STAV 2026-10-25**: vydáno 2D **v69**, 3D **v151** (základ: aktuální zdroj z `unity/GodotSrc/` v tomto repu, ne ze zálohy v50 v `trenazer-zdroj/*.tar.gz`, ta je zastaralá). **Pracovat vždy z `unity/GodotSrc/`** (`git pull` nejdřív). Pozor: `unity/GodotSrc/updater.gd` je Unity varianta (UnityUpdate) a v Godotu se nezkompiluje – pro Godot pck použít čistý updater.gd (je v záloze `zdroj-2d-godot43.tar.gz`). 3D = stejné skripty + `view3d.gd/tscn` + project.godot (Godot 4.7.2, Forward+). Poslední změny: smyk míchadla (gel β-glukanu, `gelK` ODHAD, Kunze s. 241), usazování ve vířivce 20 min, konec vyslazování 1,0 °P.
Stav: 2026-10-24, 2D verze 50 (Godot 4.3), 3D verze 133 (Godot 4.7.2). Uživatel: Vasil (Pivovar Zajíc, varna 40 hl), komunikuje česky, neformálně, stručně.

## Pravidla od Vasila
- Česky, stručně, autonomně (žádné "zkopíruj si to"), nic nezměňovat bez zadání.
- Nikdy si nevymýšlet data; odhady v kódu/dokumentech označit "ODHAD" (viz PARAMETRY.md: MEASURED/MANUFACTURER/LITERATURE/ESTIMATED).
- Trasy, polohy ventilů a čerpadel neměnit, pokud je sám neupraví. Jeho editor plán (`user://plan_upravy.json`) nepřepisovat (jen cílené patche `_patch_splid`, `_patch_pitch`).
- 2D verze je hlavní (říká, že funguje líp); 3D je kopie na Godot 4.7.2.

## Kde je co
- **Zdroj (jen v kontejneru, nebyl v gitu!)**: `/home/claude/godot/zajic2d` (2D, Godot 4.3) a `/home/claude/godot/zajic47` (3D, 4.7.2; 2D se do něj kopíruje `cp` souborů .gd). Záloha zdrojů: `trenazer-zdroj/*.tar.gz` v tomto repu.
- **Vydání**: GitHub repo `janvasilecko-droid/vasil` (větev main): `trenazer2d/trenazer2d.pck` + `version.json`, `trenazer3d/trenazer3d.pck` + `version.json` + `TrenazerVarny3D.zip` (první instalace). Program u Vasila stáhne novou verzi tlačítkem ⟳ Aktualizovat (updater.gd čte version.json).
- **Skripty**: `tools/publish2d.sh N "poznámka"`, `tools/publish3d.sh N "poznámka" [exe]` (exportují .pck a pushnou do repa). Další verze: 2D **51**, 3D **134**.
- **Godot binárky**: `/tmp/g43/Godot_v4.3-stable_linux.x86_64`, `/tmp/g47/Godot_v4.7.2-stable_linux.x86_64` (v novém kontejneru stáhnout z GitHubu godotengine/godot-builds + export templates; preset `WinPack`).
- **Vasilův PC**: složka `D:\stazene\Trenazer-varny-aplikace` (Dell Precision 7530, 32 GB).

## Soubory (2D)
main.gd (~3600 řádků, UI/vykreslení/editor/rozvaděč), sim.gd (fyzika: rmutování, scezování Darcy+jerk, chmelovar, vířivka, chlazení, kvašení, CIP), cabinet.gd (rozvaděč, ráz míchadla), editor.gd (editor plánu, hadice DN10/DN15), sight.gd (průhledítko), rotametr.gd, auto.gd + lauter_auto.gd (automat), training.gd, tests.gd + audit_tests.gd, params.gd (zdroj/nejistota parametrů), plan.json (zapojení, flagy `_pitch2`).

## Testy (vždy před vydáním)
- `cd /home/claude/godot/zajic2d && timeout 280 /tmp/g43/Godot_v4.3-stable_linux.x86_64 --headless --path . -- --tests` → očekává "202 kontrol, 0 selhání".
- `... -- --autotest --autodt=2` → "done=true failed=''" (várka 10,1 h, výtěžnost ≈ 93 %, bilance extraktu uzavřená).
- Screenshot: `/tmp/gt/shot.sh name "args" secs` (dočasné debug argumenty vždy po použití odstranit).
- Po změně: `cp` .gd do zajic47, pak publish2d.sh + publish3d.sh.

## Co je hotovo (poslední relace)
Lupa v rozvaděči; hydraulika i při pauze; otáčky míchadel; rotametr (ideál 20 l/min); pletená nerezová hadice; DN10/DN15; libovolné přiškrcování klapek (slider, Ctrl+kolečko, tažení); ráz po zastavení míchadla + RESET; trhání mláta klapkou (3–4 trhnutí smyje nános, pak trhá lože); realistické trubky s koleny/příchytkami; připnuté průhledítko se sladem; spilková hadice s průhledítkem/T/vývodem; víčko spilky (tlak při zavření); vířivka usazení 20 min (Kosa/Kunze); konec vyslazování 1,0 °P (Kosa s. 147/156).

## Zdroje čísel
- `KOSA_KUNZE_PARAMETRY.md` (i v projektu Zajic na claude.ai): čísla z Kosa–Procházka (Technologie výroby sladu a piva) a Kunze (Technology Brewing and Malting, s. 255–356) se stránkami. Kunzeho celý sken se OCR-uje do `kunze_ocr.txt` (viz níže).
- Sim už sedí s knihami: tříslověny (76 °C, pH 5,7), DMS po 30 min varu, jodové barvy, prokvašení 76 %.

## Otevřené / nápady (Vasil zatím neschválil)
- Okyselení mladiny na pH 5,1–5,2 před koncem varu (potřeba tlačítko dávkování kyseliny).
- Odvzdušnění parního pláště a podtlak (potřebuje změnu zapojení páry).
- Nátok do vířivky ≤ 3,5 m/s (neznámý průměr vtoku – nevymýšlet).
- Odhady k doptání: délka potrubí scezování 12 m, prahy 80/160 mbar (skDpCompact/skDpChannel), jednotka rotametru l/min.
- Dále z původního seznamu: přepěnění po chmelení, lom + zanášení deskového chladiče/zpětný proplach, měření (refraktometr, pH, jod co 5 min), využití chmele podle času/teploty, CIP sekvence, poruchy, bezpečnost (LOTO, horké povrchy, chemie).

## Pozn. k prostředí
Kontejner je dočasný – zdroj hned zálohovat do repa (viz `trenazer-zdroj/`). V novém kontejneru: rozbalit tar.gz, stáhnout Godot 4.3 a 4.7.2, upravit cesty ve skriptech (P=, R=, G=); R je klon repa `vasil`.

## ÚKOL: pokračovat ve čtení Kunzeho (Wolfgang Kunze, Technology Brewing and Malting, 946 s., sken)
Vasil chce, aby se z obou knih (Kosa–Procházka i Kunze) vzalo **všechno, co může simulátor udělat realističtější** ("projdi celé knihy").
- **Stav (2026-10-25)**: Kunze má i celé OCR (946 stran, jen lokálně v kontejneru, ne v repu). Přečteno: rmutování (s. 234–250), provzdušnění, řízení, bezpečnost (354–361) a dříve kapitoly o varně: scezování (s. 255–272), chmelovar a odpar (281–291, 322–325), výtěžek (323–325), vířivka a separátory (341–344), chladič (350–356). Výstupy jsou v `KOSA_KUNZE_PARAMETRY.md`. Kosa–Procházka (327 s.) je projitá pro varnu (kap. 11–18, 20); kvašení (kap. 21) a zbytek ne.
- **Nepřečteno u Kunzeho** (obsah na PDF stranách 7–11, kniha = PDF − 1 strana): 3.1 šrotování (s. 196–213), 3.4.2–3.4.3 konstrukce varny a postup chmelovaru (s. 289–322), 3.8 chlazení a provzdušnění (347–355), 3.10 řízení, **3.11 bezpečnost práce při výrobě mladiny (s. 359–366)**; kap. 1 suroviny (chmel, voda, kvasnice, s. 32–95), kap. 4 kvašení a filtrace (od s. 367) a další kapitoly (CIP, energie, laboratoř, automatizace).
- **Jak číst**: PDF je sken bez textu a je příliš velký pro Read (>100 MB). Postup: `pdftoppm -r 95 -f N -l M -png Kunze.pdf x` a pak Read na PNG stránky. Případně OCR: `kunze_ocr_resume.sh` (tesseract, ~10–20 s/stránka, přeskočí hotové). OCR text ber jen jako pomůcku, čísla ověř proti obrázku stránky.
- **Autorská práva**: text knih (ani OCR) nedávat do repa ani do projektu, jen vlastní shrnutí čísel se stránkou, bez dlouhých citací.
- **Postup**: projít kapitoly, vypsat čísla a jevy do `KOSA_KUNZE_PARAMETRY.md`, porovnat se `sim.gd`/`params.gd`, navrhnout Vasilovi nové jevy a po jeho "ano" je implementovat s testem. Čísla nahrazující odhad přepsat v `params.gd` na LITERATURE se stránkou.
- Soubor, který Vasil nahrál: `/mnt/user-data/uploads/pdfcoffee_com_wolfgang-kunze-technology-brewing-and-malting-2-pdf-free.pdf` (když chybí, požádat o nový upload).
