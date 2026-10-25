extends Control



var m
var source: = "rvp"
var grist: = false
var info: = {}
var anim: = 0.0

func open(src: String) -> void :
    source = src;mouse_filter = Control.MOUSE_FILTER_STOP
    size = Vector2(640, 360);position = Vector2(80, 70)
    refresh()

func refresh() -> void :
    var smp = m.sim.iodine_sample(source, grist)
    info = {"empty": true} if smp == null else m.sim.iodine_info(smp, grist)
    if smp != null:
        info["T"] = smp.get("T", 20.0)
        var ig = m.sim.iodine_info(smp, true)
        info["hidden"] = ( not grist) and ( not ig.get("empty", true)) and ig.starch > 0.02 and info.get("starch", 0.0) < 0.02
    queue_redraw()

func _gui_input(e: InputEvent) -> void :
    if e is InputEventMouseButton and e.pressed and e.button_index == MOUSE_BUTTON_LEFT:
        var pos: Vector2 = e.position
        if Rect2(size.x - 40, 6, 32, 30).has_point(pos): queue_free()
        elif Rect2(14, 300, 230, 40).has_point(pos): grist = not grist;refresh()
        elif Rect2(260, 300, 120, 40).has_point(pos): source = "sk" if source == "rvp" else "rvp";refresh()
        accept_event()

func _draw() -> void :
    var f: Font = m.font; var fb: Font = m.font_b
    Art.rrect_border(self, Rect2(Vector2.ZERO, size), Color(0.07, 0.085, 0.1, 0.97), Color("f2c230"), 2, 8.0)
    draw_string(fb, Vector2(14, 28), "JODOVÁ ZKOUŠKA – %s" % ("dílo v pánvi" if source == "rvp" else "rmut / sladina v SK"), HORIZONTAL_ALIGNMENT_LEFT, -1, 17, Color("f2c230"))
    draw_string(fb, Vector2(size.x - 30, 27), "X", HORIZONTAL_ALIGNMENT_LEFT, -1, 18, Color("e6ebef"))

    var c: = Vector2(150, 160)
    draw_circle(c, 112.0, Color("8f9aa3"));draw_circle(c, 108.0, Color("f4f6f7"));draw_circle(c, 98.0, Color("e6eaed"))
    if info.get("empty", true):
        draw_string(f, Vector2(14, 350), "", HORIZONTAL_ALIGNMENT_LEFT, -1, 1, Color.WHITE)
        _text(["V nádobě není co vzorkovat.", "Zkoušku dělej s dílem po prodlevě nebo s rmutem v SK."], [Color("e6ebef"), Color("9aa3ab")])
    else:
        var col: Color = info.color

        draw_circle(c, 44.0, Color("e9d9a8").lerp(col, 0.25))
        var k: float = minf(1.0, anim)
        draw_circle(c, 36.0 * (0.4 + 0.6 * k), Color(col, 0.95))
        draw_arc(c, 36.0 * (0.4 + 0.6 * k), 0, TAU, 40, Color(0, 0, 0, 0.25), 2.0, true)
        draw_circle(c + Vector2(-12, -12), 6.0, Color(1, 1, 1, 0.25))
        _text(["Barva po kapce jódu:", info.name, 
            "Vzorek: %s, ochlazený na pokojovou teplotu." % ("přecezená kapalina" if not grist else "kapalina s rozetřenými zrny"), 
            "Škrob: %s · dextriny: %s" % [_lvl(info.starch), _lvl(info.dextrin * 0.4)], 
            "POZOR: v zrnech ještě zůstává škrob – kapalina je negativní, ale zrna ne. Zkus „se zrny“." if info.get("hidden", false) else ""], 
            [Color("9aa3ab"), Color("f2c230"), Color("c9d1d8"), Color("7b858e"), Color("ff6b5b")])
    Art.rrect_border(self, Rect2(14, 300, 230, 40), Color("262d35"), Color("3a444e"), 1, 6.0)
    draw_string(fb, Vector2(26, 326), "Vzorek: %s" % ("se zrny (rozetřená drť)" if grist else "přecezená kapalina"), HORIZONTAL_ALIGNMENT_LEFT, -1, 14, Color("e6ebef"))
    Art.rrect_border(self, Rect2(260, 300, 120, 40), Color("262d35"), Color("3a444e"), 1, 6.0)
    draw_string(fb, Vector2(274, 326), "Pánev / SK", HORIZONTAL_ALIGNMENT_LEFT, -1, 14, Color("e6ebef"))

func _lvl(x: float) -> String:
    return "žádný" if x < 0.002 else ("stopy" if x < 0.01 else ("málo" if x < 0.04 else ("hodně" if x < 0.15 else "velmi mnoho")))

func _text(lines: Array, cols: Array) -> void :
    var y: = 78.0
    for i in lines.size():
        draw_string(m.font, Vector2(280, y), lines[i], HORIZONTAL_ALIGNMENT_LEFT, 340, 13, cols[i])
        y += 40.0 if i == 1 else 24.0

func _process(d: float) -> void :
    anim += d * 1.4
    if anim < 1.2: queue_redraw()

const Art = preload("res://art.gd")
