extends Node
## Aktualizace přes internet: stáhne version.json z GitHubu, a když je novější, stáhne trenazer2d.pck a restartuje program.

const BASE: = "https://raw.githubusercontent.com/janvasilecko-droid/vasil/main/trenazer2d/"
var main: Node
var http: HTTPRequest
var stage: = ""
var remote: = {}
signal status(text: String, kind: String)
signal available(ver: int)

func _ready() -> void :
    http = HTTPRequest.new();http.timeout = 180.0;add_child(http)
    http.request_completed.connect(_done)

func current_ver() -> int:
    var f: = FileAccess.open("res://version.json", FileAccess.READ)
    if f == null: return 0
    var d = JSON.parse_string(f.get_as_text())
    return int(d.get("ver", 0)) if d is Dictionary else 0

func check(download: = true) -> void :
    if stage != "": return
    stage = "check_dl" if download else "check"
    if download: status.emit("Hledám novou verzi…", "info")
    var err: = http.request(BASE + "version.json?t=%d" % int(Time.get_unix_time_from_system()))
    if err != OK:
        stage = "";status.emit("Aktualizace: nejde se připojit (%d)." % err, "warn")

func _done(result: int, code: int, _h: PackedStringArray, body: PackedByteArray) -> void :
    var st: = stage;stage = ""
    if result != HTTPRequest.RESULT_SUCCESS or code != 200:
        http.download_file = ""
        if st != "check": status.emit("Aktualizace: server neodpověděl (chyba %d / HTTP %d) – zkontroluj internet." % [result, code], "warn")
        return
    if st.begins_with("check"):
        var d = JSON.parse_string(body.get_string_from_utf8())
        if not d is Dictionary:
            status.emit("Aktualizace: špatný soubor verze.", "warn");return
        remote = d
        var cv: = current_ver()
        if int(d.get("ver", 0)) <= cv:
            if st == "check_dl": status.emit("Máš nejnovější verzi (%d)." % cv, "info")
            return
        available.emit(int(d.ver))
        if st == "check":
            var dlg: = ConfirmationDialog.new();dlg.title = "Nová verze trenažéru"
            dlg.dialog_text = "Je k dispozici verze %d (máš %d).\n\n%s\n\nStáhnout a restartovat teď?" % [int(d.ver), cv, str(d.get("notes", ""))]
            dlg.dialog_autowrap = true
            dlg.ok_button_text = "Aktualizovat";dlg.cancel_button_text = "Později"
            dlg.confirmed.connect( func(): check(true))
            get_tree().root.add_child(dlg);dlg.popup_centered(Vector2i(560, 260))
            return
        status.emit("Stahuji verzi %d…" % int(d.ver), "info")
        stage = "pck";http.download_file = "user://update2d.pck.part"
        var err: = http.request(BASE + "trenazer2d.pck?v=%d" % int(d.ver))
        if err != OK:
            stage = "";http.download_file = "";status.emit("Stažení nejde spustit (%d)." % err, "warn")
    elif st == "pck":
        http.download_file = ""
        var f: = FileAccess.open("user://update2d.pck.part", FileAccess.READ)
        var size: int = f.get_length() if f != null else 0
        if f != null: f.close()
        if size < 1000 or (remote.has("size") and size != int(remote.size)):
            status.emit("Stažený soubor je neúplný (%d B) – zkus to znovu." % size, "warn");return
        var dir: = DirAccess.open("user://")
        if dir.file_exists("update2d.pck"): dir.remove("update2d.pck")
        dir.rename("update2d.pck.part", "update2d.pck")
        var vf: = FileAccess.open("user://update2d_ver.json", FileAccess.WRITE);vf.store_string(JSON.stringify(remote));vf.close()
        status.emit("Verze %d stažena – program se restartuje…" % int(remote.ver), "info")
        if main != null and main.has_method("save_game"): main.save_game()
        await get_tree().create_timer(1.2).timeout
        OS.set_restart_on_exit(true)
        get_tree().quit()
