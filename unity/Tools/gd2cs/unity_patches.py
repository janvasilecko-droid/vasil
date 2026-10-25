# Úpravy původních GDScriptů pro běh v Unity (aplikují se na text před překladem).
# Každá úprava: (hledaný text, náhrada). Původní soubory se nemění.

BAKE_OLD_START = "func _bake() -> void :\n"

def _cut(src, start_marker, end_marker):
    a = src.index(start_marker); b = src.index(end_marker, a)
    return src[a:b]

PATCHES = {}

def make_main_patches():
    return []

# main.gd: v Unity se plánek kreslí přímo (vektorově), ne do dlaždic z podokna; doostřování odpadá.
def patch_main(src):
    bake = _cut(src, "func _bake() -> void :", "var detail_spr: Sprite2D = null")
    new_bake = (
        "func _bake() -> void :\n"
        "    if tiles_root: tiles_root.queue_free()\n"
        "    tiles_root = Node2D.new();add_child(tiles_root);move_child(tiles_root, 1)\n"
        "    var sl: = StaticLayer.new();sl.m = self;tiles_root.add_child(sl)\n"
        "    var il: = ItemLayer.new();il.m = self;il.skip_valves = true;tiles_root.add_child(il)\n\n"
    )
    src = src.replace(bake, new_bake, 1)
    dt = _cut(src, "func _detail_tick(delta: float) -> void :", "var detail_svs: = []")
    src = src.replace(dt, "func _detail_tick(delta: float) -> void :\n    return\n\n", 1)
    bd = _cut(src, "func _bake_detail(vr: Rect2, z: float) -> void :", "func mf_ports(mf) -> Array:")
    src = src.replace(bd, "func _bake_detail(vr: Rect2, z: float) -> void :\n    return\n\n", 1)
    return src
