# Přenos FYZIKA+ do aktuální verze trenažéru (složka „Trenazer varny“ na ploše)

Rozpracováno 17. 11. 2026. **Na Vasilův počítač to ještě zapsané není.**

- `scripts/fyzika_plus.gd` – nový soubor (1. část: tepelné ztráty a odpar), patří do `scripts/`.
- `scripts/sim.gd` – jeho `sim.gd` ze 17. 11. (změna souboru 1794164209848 ms) se čtyřmi úpravami; rozdíl je v `sim.gd.diff`.
  Kdyby mezitím `sim.gd` změnil, nepřepisovat – vložit jen ty čtyři úpravy z rozdílu.
- `tests/fyzika_plus_test.gd` – test 1. části (5 kontrol, tady prošel), patří do `tests/`.
- `FYZIKA-PLUS-PRENOS.md` – co je přeneseno a co zbývá, patří do kořene projektu.

Co není ověřeno: celá várka podle checklistu (`--mode=chkauto`, 43 kroků). V kontejneru bez grafiky ani za hodinu
neprošla prvním krokem, a to ani v původní verzi bez úprav – nejspíš potřebuje skutečné okno. Spustit na jeho PC:
`Godot --path "<projekt>" -- --mode=chkauto` a čekat `CHKAUTO hotovo=true kroků=43`; pak ostatní testy v `tests/`.
Před zápisem zazálohovat `scripts/sim.gd` do `zaloha-pred-fyzika-plus/`.
