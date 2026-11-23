@echo off
chcp 65001 >nul
cd /d "%~dp0"
echo Spoustim celou varku podle checklistu (muze trvat nekolik minut)...
if exist "D:\stazene\Godot\Godot_v4.3-stable_win64.exe" (
  "D:\stazene\Godot\Godot_v4.3-stable_win64.exe" --path "%CD%" --log-file "%CD%\chkauto-vysledek.txt" -- --mode=chkauto --capture="%CD%\chkauto-snimek.png"
) else (
  echo Nenasel jsem D:\stazene\Godot\Godot_v4.3-stable_win64.exe
  pause
  exit /b 1
)
echo Hotovo. Vysledek je v souboru chkauto-vysledek.txt (radek CHKAUTO hotovo=...).
pause
