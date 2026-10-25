# Pivovar Zajíc – Unity HDRP (první kolo: 3D varna podle 2D plánu)

Stavba 3D varny z `plan.json` (stejné rozmístění nádob a potrubí jako ve 2D verzi), HDRP materiály, hala, světla, post-processing, animovaný tok v potrubí a pára.

## Jak to rozjet (Unity 6.x, HDRP)
1. Unity Hub → **New project** → šablona **3D (HDRP)** (High Definition 3D), název např. `Pivovar`.
2. Do složky `Assets` nového projektu rozbal obsah balíčku (složky `Pivovar` a `StreamingAssets`).
3. Otevři libovolnou scénu (třeba `SampleScene`) a stiskni **Play**. Varna se postaví sama.

## Ovládání
pravé tlačítko = otáčení, Shift+pravé nebo prostřední = posun, kolečko = zoom, WASD/QE = jízda, **F** = tok v potrubí, **X** = řez / celé nádoby.

## Struktura
- `Assets/Pivovar/Core` – čistý C# bez Unity (JSON, plán varny). Testuje se na GitHubu (workflow *Unity core tests*).
- `Assets/Pivovar/Scripts` – Unity skripty (scéna, materiály HDRP, kamera, pára, tok v potrubí).
- `Assets/StreamingAssets/plan.json` – rozmístění z 2D verze.
- `CoreTests` – testy jádra (`dotnet run`).

## Stav
Hotovo: načtení plánu (testováno), nádoby s řezem, trubky s tri-clamp spoji, klapky, čerpadla, hala, světla, HDRP prostředí.
Zatím nenapojeno: simulace (hladiny, teploty, toky) – další krok je port `sim.gd` do C# s testy v CI.


## Aktualizace programu bez nového stahování
Tlačítko **⟳** v HUD (nebo automatická kontrola 4 s po startu) stáhne z GitHubu (větev `unity-update`) `manifest.json`, porovná SHA-256 souborů
v `Pivovar_Data` a stáhne **jen změněné** (`Managed/PivovarCore.dll`, `Managed/PivovarGdGen.dll`, `StreamingAssets/godot/**`).
Po zavření programu je dávka nahradí (záloha `.bak`, při chybě návrat) a program se spustí znovu. Protokol: `…/update/apply.log`.

Podmínky: Windows build s **Mono** (nastaví `Pivovar ▸ Sestavit .exe` automaticky), program ve složce, kam smí zapisovat.
Co aktualizace nemění: skripty ve scéně (vzhled, kamera, 3D varna) – ty vyžadují nové sestavení .exe; pak se zvýší `UpdateClient.ScriptsRev`.
Knihovny staví CI (`update-pack` v `unity-core.yml`) po zelených testech; manifest je vždy jediný commit ve větvi `unity-update`.
