extends Node
## Start: když je stažená novější aktualizace (user://update2d.pck), načte ji místo vestavěné verze a pustí trenažér.

static func ver_of(path: String) -> int:
    if not FileAccess.file_exists(path): return 0
    var d = JSON.parse_string(FileAccess.open(path, FileAccess.READ).get_as_text())
    return int(d.get("ver", 0)) if d is Dictionary else 0

func _ready() -> void :
    var base: = ver_of("res://version.json")
    var upd: = ver_of("user://update2d_ver.json")
    if upd > base and FileAccess.file_exists("user://update2d.pck"):
        if not ProjectSettings.load_resource_pack("user://update2d.pck", true):
            push_warning("Aktualizaci se nepodařilo načíst – jede vestavěná verze.")
    print("Trenažér varny: verze %d (vestavěná %d, stažená %d)" % [ver_of("res://version.json"), base, upd])
    get_tree().change_scene_to_file.call_deferred("res://main.tscn")
