extends Control
## Zvětšený rotametr scezování (vlevo): průtok sladiny čerpadlem SK.
## Ideál 20 (zadal Vasil, jednotka L/min = můj předpoklad). Zrna se lepí na plovák –
## čtení klesá, dokud se rotametr neproklepne (tempo zanášení = ODHAD).

const IDEAL: = 20.0
const MAXV: = 40.0
const YEL: = Color("f2c230")
var m
var btn: Button
var shown: = 0.0
var shake: = 0.0
var tapmsg: = 0.0

func _ready() -> void :
	mouse_filter = Control.MOUSE_FILTER_IGNORE
	btn = Button.new();btn.text = "PROKLEPAT";btn.focus_mode = Control.FOCUS_NONE
	btn.tooltip_text = "Poklepat na rotametr – uvolní zrna, co drží plovák (T)"
	btn.pressed.connect(tap);add_child(btn)

func tap() -> void :
	var S = m.sim.S
	if not S.has("rota"): S["rota"] = {"stick": 0.0}
	S.rota.stick = maxf(0.0, S.rota.stick - 1.0)
	shake = 0.5;tapmsg = 2.0
	m._say("Rotametr proklepnut – plovák je volný.")

func _process(delta: float) -> void :
	if m == null or m.sim == null: return
	var vs: Vector2 = get_viewport().get_visible_rect().size
	var h: float = clampf(vs.y - 400.0, 260.0, 360.0)
	size = Vector2(150, h);position = Vector2(12, vs.y - h - 80.0 if vs.y > 700 else 250.0)
	position.y = maxf(position.y, 250.0)
	btn.position = Vector2(10, h - 34);btn.size = Vector2(130, 28)
	var S = m.sim.S
	var q: float = S.p2.q if S.p2.on else 0.0
	var st: float = S.get("rota", {}).get("stick", 0.0)
	var target: float = q * (1.0 - 0.6 * clampf(st, 0.0, 1.0))
	shown = lerpf(shown, target, minf(1.0, delta * 3.0))
	shake = maxf(0.0, shake - delta);tapmsg = maxf(0.0, tapmsg - delta)
	queue_redraw()

func _unhandled_key_input(e: InputEvent) -> void :
	if e is InputEventKey and e.pressed and not e.echo and e.keycode == KEY_T and m.editor == null: tap()

func _draw() -> void :
	var f: Font = m.font
	var S = m.sim.S
	var st: float = S.get("rota", {}).get("stick", 0.0)
	draw_rect(Rect2(Vector2.ZERO, size), Color(0.07, 0.09, 0.11, 0.92), true)
	draw_rect(Rect2(Vector2.ZERO, size), YEL, false, 2.0)
	draw_string(f, Vector2(10, 18), "ROTAMETR SCEZOVÁNÍ", HORIZONTAL_ALIGNMENT_LEFT, -1, 12, YEL)
	var top: float = 34.0; var bot: float = size.y - 120.0
	var tx: float = 20.0; var tw: float = 56.0
	draw_rect(Rect2(tx, top, tw, bot - top), Color(0.55, 0.8, 0.95, 0.18), true)
	draw_rect(Rect2(tx, top, tw, bot - top), Color("c9d1d9"), false, 2.0)
	var yv: = func(v: float) -> float: return bot - (bot - top) * clampf(v / MAXV, 0.0, 1.0)
	draw_rect(Rect2(tx, yv.call(IDEAL + 2.0), tw, yv.call(IDEAL - 2.0) - yv.call(IDEAL + 2.0)), Color(0.2, 0.8, 0.35, 0.28), true)
	for v in range(0, 41, 5):
		var y: float = yv.call(float(v))
		draw_line(Vector2(tx + tw, y), Vector2(tx + tw + (14.0 if v % 10 == 0 else 8.0), y), Color("c9d1d9"), 1.5)
		if v % 10 == 0: draw_string(f, Vector2(tx + tw + 18, y + 4), str(v), HORIZONTAL_ALIGNMENT_LEFT, -1, 13, Color("e6edf3"))
	var yi: float = yv.call(IDEAL)
	draw_line(Vector2(tx - 8, yi), Vector2(tx + tw + 12, yi), Color("35d06a"), 3.0)
	draw_string(f, Vector2(tx + tw + 18, yi - 6), "ideál 20", HORIZONTAL_ALIGNMENT_LEFT, -1, 11, Color("35d06a"))
	# plovák
	var fy: float = yv.call(shown)
	if shake > 0.0: fy += sin(shake * 60.0) * 4.0 * shake * 2.0
	var cx: float = tx + tw * 0.5
	var col: = Color("e0452f") if st < 0.5 else Color("b58a3a")
	draw_colored_polygon(PackedVector2Array([Vector2(cx - 20, fy - 12), Vector2(cx + 20, fy - 12), Vector2(cx + 12, fy + 14), Vector2(cx - 12, fy + 14)]), col)
	draw_line(Vector2(cx - 20, fy - 12), Vector2(cx + 20, fy - 12), Color("f4f6f8"), 3.0)
	if st > 0.3: for k in int(st * 8.0) + 1: draw_circle(Vector2(cx - 16 + (k * 7) % 32, fy + 16 + (k * 5) % 6), 2.0, Color("8a6a3a"))
	# číslo
	var ry: float = bot + 16.0
	draw_rect(Rect2(10, ry, 130, 40), Color("0b0f12"), true);draw_rect(Rect2(10, ry, 130, 40), Color("444c54"), false, 1.0)
	var ok: bool = absf(shown - IDEAL) <= 2.0
	draw_string(f, Vector2(18, ry + 30), "%.1f" % shown, HORIZONTAL_ALIGNMENT_LEFT, -1, 28, Color("35d06a") if ok else (Color("ffb020") if shown > 1.0 else Color("8a939b")))
	draw_string(f, Vector2(104, ry + 30), "L/min", HORIZONTAL_ALIGNMENT_LEFT, -1, 12, Color("c9d1d9"))
	var msg: = ""
	if tapmsg > 0.0: msg = "proklepnuto ✔"
	elif st > 0.6: msg = "plovák se lepí – proklepat!"
	elif st > 0.3: msg = "zrna se usazují…"
	draw_string(f, Vector2(10, ry + 58), msg, HORIZONTAL_ALIGNMENT_LEFT, -1, 12, Color("ffb020"))
