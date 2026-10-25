extends Node3D
## 3D pohled na varnu – žije podle simulace (main.sim). Staví se z m.items (stejná data jako 2D a editor).
## Klávesa V = přepnout 2D/3D, X = průhledné nádoby, myš: levé tlačítko otáčí, pravé posouvá, kolečko přibližuje.
const K := 0.005          # 1 px plánu = 5 mm
const ZP := 0.42          # rovina potrubí (m před osou nádob)
var m
var sim
var active := false
var cam: Camera3D
var yaw := -0.25
var pitch := 0.2
var dist := 18.0
var target := Vector3(5, 2.5, 0)
var mode := 1   # 0 = plný nerez, 1 = řez (vidět dovnitř), 2 = sklo
var cut_shader: Shader
var floor_y := 1200.0
var W: Node3D
var inox: StandardMaterial3D
var steel_b: StandardMaterial3D
var steel_p: StandardMaterial3D
var vessels: Array = []
var tubes: Array = []
var valves: Array = []
var pumps: Array = []
var labels: Array = []
var steams: Array = []
var lamps: Array = []
var t := 0.0
var acc := 0.0
var steam_tex: GradientTexture2D
var pipe_shader: Shader
var liq_shader: Shader
var dirty := true
var keys_vel := Vector3.ZERO

const PIPE_SH := """
shader_type spatial;
uniform vec3 steel = vec3(0.72, 0.74, 0.78);
uniform vec3 liq = vec3(0.3, 0.6, 0.9);
uniform float fill = 0.0;
uniform float speed = 0.0;
void fragment() {
	float s = fract(UV.x * 4.0 - TIME * speed * 4.0);
	float dash = smoothstep(0.30, 0.45, s) * (1.0 - smoothstep(0.55, 0.70, s));
	float moving = step(0.001, abs(speed));
	vec3 c = mix(steel, liq, fill * 0.6);
	ALBEDO = c;
	METALLIC = 0.75; ROUGHNESS = 0.3;
	EMISSION = liq * fill * 0.22 + vec3(1.0) * dash * moving * fill * 0.55;
}
"""
const CUT_SH := """
shader_type spatial;
render_mode cull_disabled;
uniform vec4 albedo : source_color = vec4(0.8, 0.82, 0.85, 1.0);
uniform float metal = 0.85;
uniform float rough = 0.42;
uniform vec3 ctr = vec3(0.0);
uniform float rad = 1.0;
varying vec3 wpos;
void vertex() { wpos = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz; }
void fragment() {
	vec3 d = wpos - ctr;
	if (d.z > rad * 0.04 && abs(d.x) < rad * 0.80) discard;
	float k = 0.94 + 0.06 * sin(wpos.y * 260.0 + wpos.x * 3.0);   // jemný kartáčovaný vzor
	ALBEDO = albedo.rgb * k;
	if (!FRONT_FACING) ALBEDO *= 0.62;
	METALLIC = metal; ROUGHNESS = rough;
}
"""
const LIQ_SH := """
shader_type spatial;
render_mode cull_disabled;
uniform vec4 col : source_color = vec4(0.3, 0.6, 0.9, 0.9);
uniform float boil = 0.0;
uniform float stir = 0.0;
uniform float grain = 0.0;
uniform vec3 ctr = vec3(0.0);
uniform float rad = 1.0;
uniform float cut = 0.0;
varying float topface;
varying vec3 wpos;
float hh(vec3 p) { return fract(sin(dot(p, vec3(12.9898, 78.233, 37.719))) * 43758.5453); }
void vertex() {
	topface = step(0.6, NORMAL.y);
	float w = sin(VERTEX.x * 9.0 + TIME * (2.0 + boil * 9.0)) * cos(VERTEX.z * 7.0 + TIME * (1.7 + boil * 7.0));
	VERTEX.y += topface * w * (0.006 + 0.03 * boil + 0.012 * stir) * (1.0 - grain * 0.8);
	wpos = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
}
void fragment() {
	if (cut > 0.5) { vec3 d = wpos - ctr; if (d.z > rad * 0.04 && abs(d.x) < rad * 0.80) discard; }
	vec3 c = col.rgb;
	float a = col.a;
	if (grain > 0.5) {
		float n = hh(floor(wpos * 75.0)); float n2 = hh(floor(wpos * 22.0));
		c = c * (0.62 + 0.55 * n + 0.2 * n2);
		a = 1.0; ROUGHNESS = 0.92; SPECULAR = 0.15;
	} else { ROUGHNESS = 0.08; SPECULAR = 0.8; }
	ALBEDO = c; ALPHA = a;
	if (!FRONT_FACING) ALBEDO *= 0.8;
	METALLIC = 0.0;
	EMISSION = c * (0.10 + 0.25 * boil * topface) * (1.0 - grain * 0.8);
}
"""

func P(x: float, y: float, z: float = 0.0) -> Vector3: return Vector3(x * K, (floor_y - y) * K, z)

func _ready() -> void:
	inox = _mat(Color(0.86, 0.87, 0.9), 0.3, 1.0)
	steel_b = _mat(Color(0.82, 0.84, 0.87), 0.38, 1.0)
	steel_p = _mat(Color(0.9, 0.91, 0.93), 0.16, 1.0)
	pipe_shader = Shader.new(); pipe_shader.code = PIPE_SH
	liq_shader = Shader.new(); liq_shader.code = LIQ_SH
	cut_shader = Shader.new(); cut_shader.code = CUT_SH
	var g := Gradient.new(); g.colors = PackedColorArray([Color(1, 1, 1, 0.55), Color(1, 1, 1, 0.0)]); g.offsets = PackedFloat32Array([0.0, 1.0])
	steam_tex = GradientTexture2D.new(); steam_tex.gradient = g; steam_tex.fill = GradientTexture2D.FILL_RADIAL
	steam_tex.fill_from = Vector2(0.5, 0.5); steam_tex.fill_to = Vector2(1.0, 0.5); steam_tex.width = 64; steam_tex.height = 64
	cam = Camera3D.new(); cam.fov = 55; cam.far = 300; cam.near = 0.1; add_child(cam)
	cam.current = false
	visible = false

func set_active(on: bool) -> void:
	active = on; visible = on
	cam.current = on
	if on:
		var vp := get_viewport(); vp.msaa_3d = Viewport.MSAA_4X; vp.screen_space_aa = Viewport.SCREEN_SPACE_AA_FXAA
	if on:
		if dirty or W == null: build()
		_cam()
	if m != null: m.visible = not on

func _mat(c: Color, rough: float = 0.4, metal: float = 0.2) -> StandardMaterial3D:
	var s := StandardMaterial3D.new(); s.albedo_color = c; s.roughness = rough; s.metallic = metal; return s

func _emis(c: Color, e: float = 2.0) -> StandardMaterial3D:
	var s := StandardMaterial3D.new(); s.albedo_color = c; s.emission_enabled = true; s.emission = c; s.emission_energy_multiplier = e; return s

func _add(parent: Node3D, mesh: Mesh, pos: Vector3, mat: Material, rot: Vector3 = Vector3.ZERO) -> MeshInstance3D:
	var mi := MeshInstance3D.new(); mi.mesh = mesh; mi.position = pos; mi.rotation = rot; mi.material_override = mat; parent.add_child(mi); return mi

func _cyl(r: float, h: float, seg: int = 32, r2: float = -1.0) -> CylinderMesh:
	var c := CylinderMesh.new(); c.top_radius = r; c.bottom_radius = r if r2 < 0.0 else r2; c.height = h; c.radial_segments = seg; c.rings = 1; return c

func _box(s: Vector3) -> BoxMesh:
	var b := BoxMesh.new(); b.size = s; return b

# ───────────────────────── stavba scény ─────────────────────────
func build() -> void:
	if W != null: W.queue_free()
	W = Node3D.new(); add_child(W)
	vessels.clear(); tubes.clear(); valves.clear(); pumps.clear(); labels.clear(); steams.clear(); lamps.clear()
	sim = m.sim
	floor_y = 0.0
	for it in m.items:
		if it.type == "vessel" and it.get("shape") != "platform" and it.get("shape") != "inlet" and it.get("shape") != "chiller":
			floor_y = maxf(floor_y, float(it.y) + float(it.h))
	floor_y += 6.0
	_hall()
	for it in m.items:
		if it.type == "vessel": _vessel(it)
	_pipes()
	for it in m.items:
		match it.type:
			"valve": _valve(it)
			"pump": _pump(it)
			"panel": _panel(it)
			"deco": _deco(it)
			"flow": _flowmeter(it)
			"sw": pass
	dirty = false

func _pano() -> ImageTexture:
	var W_ := 512; var H_ := 256
	var im := Image.create(W_, H_, false, Image.FORMAT_RGBF)
	for y in H_:
		var f := float(y) / H_      # 0 strop … 1 podlaha
		for x in W_:
			var u := float(x) / W_
			var c := Color(0.16, 0.17, 0.19)
			if f < 0.30:   # strop s pásy svítidel
				c = Color(0.22, 0.23, 0.25)
				if (int(u * 24.0) % 3 == 0) and f > 0.10 and f < 0.18: c = Color(2.6, 2.5, 2.3)
			elif f < 0.52:  # stěna s okny
				c = Color(0.62, 0.6, 0.55)
				if (int(u * 16.0) % 2 == 0) and f > 0.34 and f < 0.44: c = Color(2.2, 2.4, 2.8)
			else:           # podlaha
				c = Color(0.28, 0.28, 0.3) * (1.0 - (f - 0.52))
			im.set_pixel(x, y, c)
	return ImageTexture.create_from_image(im)

func _hall() -> void:
	var e := Environment.new(); e.background_mode = Environment.BG_SKY
	var sk := Sky.new(); var psm := PanoramaSkyMaterial.new(); psm.panorama = _pano(); sk.sky_material = psm; sk.radiance_size = Sky.RADIANCE_SIZE_256; e.sky = sk
	e.ambient_light_source = Environment.AMBIENT_SOURCE_SKY; e.ambient_light_energy = 0.8; e.reflected_light_source = Environment.REFLECTION_SOURCE_SKY
	e.tonemap_mode = Environment.TONE_MAPPER_ACES; e.tonemap_exposure = 1.0
	e.glow_enabled = true; e.glow_intensity = 0.7; e.glow_bloom = 0.15; e.glow_hdr_threshold = 1.0
	e.fog_enabled = true; e.fog_light_color = Color(0.55, 0.6, 0.68); e.fog_density = 0.004
	var we := WorldEnvironment.new(); we.environment = e; W.add_child(we)
	var sun := DirectionalLight3D.new(); sun.rotation_degrees = Vector3(-55, -25, 0); sun.light_energy = 0.55; sun.shadow_enabled = true
	sun.directional_shadow_max_distance = 70; sun.light_color = Color(1.0, 0.95, 0.88); W.add_child(sun)
	var x0 := -12.0; var x1 := 46.0; var zc := 0.0; var depth := 30.0; var hgt := 8.0
	# podlaha – dlažba s lehkým leskem
	var img := Image.create(256, 256, false, Image.FORMAT_RGB8)
	for y in 256:
		for x in 256:
			var edge := (x % 128 < 3) or (y % 128 < 3)
			var n := 0.5 + 0.06 * sin(x * 0.37) * cos(y * 0.29) + 0.04 * sin(x * 1.3 + y * 0.7)
			var c := Color(0.47, 0.47, 0.5) * n if not edge else Color(0.22, 0.22, 0.24)
			img.set_pixel(x, y, c)
	img.generate_mipmaps()
	var ft := ImageTexture.create_from_image(img)
	var fm := _mat(Color(1, 1, 1), 0.32, 0.25); fm.albedo_texture = ft; fm.uv1_scale = Vector3((x1 - x0) / 2.5, depth / 2.5, 1.0); fm.texture_filter = BaseMaterial3D.TEXTURE_FILTER_LINEAR_WITH_MIPMAPS_ANISOTROPIC
	_add(W, _box(Vector3(x1 - x0, 0.2, depth)), Vector3((x0 + x1) * 0.5, -0.1, zc), fm)
	# zadní stěna – obklad
	var wi := Image.create(128, 128, false, Image.FORMAT_RGB8)
	for y in 128:
		for x in 128:
			var edge := (x % 64 < 2) or (y % 32 < 2)
			wi.set_pixel(x, y, Color(0.85, 0.85, 0.82) * (0.93 + 0.05 * sin(x * 0.9 + y * 0.4)) if not edge else Color(0.55, 0.55, 0.52))
	wi.generate_mipmaps()
	var wm := _mat(Color(1, 1, 1), 0.55, 0.0); wm.albedo_texture = ImageTexture.create_from_image(wi); wm.uv1_scale = Vector3((x1 - x0) / 2.0, 3.0, 1.0)
	_add(W, _box(Vector3(x1 - x0, 3.2, 0.3)), Vector3((x0 + x1) * 0.5, 1.6, zc - depth * 0.5), wm)
	var wall2 := _mat(Color(0.78, 0.76, 0.7), 0.85, 0.0)
	_add(W, _box(Vector3(x1 - x0, hgt - 3.2, 0.3)), Vector3((x0 + x1) * 0.5, 3.2 + (hgt - 3.2) * 0.5, zc - depth * 0.5), wall2)
	_add(W, _box(Vector3(0.3, hgt, depth)), Vector3(x0, hgt * 0.5, zc), wall2)
	_add(W, _box(Vector3(0.3, hgt, depth)), Vector3(x1, hgt * 0.5, zc), wall2)
	_add(W, _box(Vector3(x1 - x0, 0.3, depth)), Vector3((x0 + x1) * 0.5, hgt, zc), _mat(Color(0.7, 0.7, 0.68), 0.9, 0.0))
	# okna se světlem
	var win := _emis(Color(0.7, 0.82, 1.0), 1.6)
	for i in range(6):
		var wx := 2.0 + i * 8.0
		_add(W, _box(Vector3(3.2, 1.8, 0.05)), Vector3(wx, 5.0, zc - depth * 0.5 + 0.2), win)
		for s in [-1.0, 0.0, 1.0]: _add(W, _box(Vector3(0.06, 1.8, 0.07)), Vector3(wx + s * 1.0, 5.0, zc - depth * 0.5 + 0.22), _mat(Color(0.2, 0.2, 0.22), 0.5, 0.2))
	# střešní vazníky a lampy
	var beam := _mat(Color(0.25, 0.27, 0.3), 0.5, 0.6)
	for i in range(8):
		var bx := -4.0 + i * 7.0
		_add(W, _box(Vector3(0.3, 0.45, depth)), Vector3(bx, hgt - 0.4, zc), beam)
		var lamp := _add(W, _box(Vector3(1.4, 0.12, 0.35)), Vector3(bx, hgt - 1.0, 1.0), _emis(Color(1.0, 0.96, 0.85), 3.0))
		lamps.append(lamp)
		if i % 2 == 0:
			var l := OmniLight3D.new(); l.position = Vector3(bx, hgt - 1.4, 1.5); l.omni_range = 13; l.light_energy = 0.7; l.light_color = Color(1.0, 0.95, 0.85); l.shadow_enabled = false; W.add_child(l)
	# odtokový žlab v podlaze
	_add(W, _box(Vector3(x1 - x0 - 2.0, 0.02, 0.22)), Vector3((x0 + x1) * 0.5, 0.01, 2.6), _mat(Color(0.12, 0.12, 0.13), 0.4, 0.8))

# ───────────────────────── nádoby ─────────────────────────
func _vessel(it: Dictionary) -> void:
	var w: float = it.w * K; var h: float = it.h * K; var cx: float = (it.x + it.w * 0.5) * K; var yb: float = (floor_y - it.y - it.h) * K
	var shape: String = it.get("shape", "tank"); var r: float = w * 0.5
	var zc: float = -r - 0.35
	var v := {"it": it, "shape": shape, "bind": it.get("bind"), "r": r, "h": h, "yb": yb, "cx": cx, "zc": zc, "liq": [], "shell": []}
	var shell_mat: Material = _shell_mat(v)
	match shape:
		"platform":
			var deck := _mat(Color(0.62, 0.64, 0.68), 0.35, 0.9)
			_add(W, _box(Vector3(w, 0.1, 5.2)), Vector3(cx, yb + 0.05, -2.0), deck)
			for i in range(0, 14): _add(W, _box(Vector3(w, 0.012, 0.03)), Vector3(cx, yb + 0.106, -4.5 + i * 0.4), steel_b)
			_add(W, _box(Vector3(w, 0.14, 0.05)), Vector3(cx, yb - 0.02, 0.58), steel_p)
			_add(W, _box(Vector3(w, 0.14, 0.05)), Vector3(cx, yb - 0.02, -4.58), steel_p)
			for s in range(0, 6):
				var px: float = cx - w * 0.5 + 0.25 + s * (w - 0.5) / 5.0
				for zz in [0.45, -4.45]: _add(W, _box(Vector3(0.1, yb, 0.1)), Vector3(px, yb * 0.5, zz), steel_b)
			# zábradlí
			for s in range(0, 11):
				var rx: float = cx - w * 0.5 + 0.1 + s * (w - 0.2) / 10.0
				_add(W, _cyl(0.02, 1.05, 8), Vector3(rx, yb + 0.62, 0.55), steel_p)
			for hh in [0.55, 1.1]: _add(W, _cyl(0.02, w, 8), Vector3(cx, yb + hh + 0.1, 0.55), steel_p, Vector3(0, 0, PI / 2))
			return
		"horiz":
			var rr: float = h * 0.5
			v.zc = -rr - 0.3; v.r = rr
			var sh := _add(W, _cyl(rr, w, 40), Vector3(cx, yb + rr, v.zc), shell_mat, Vector3(0, 0, PI / 2)); v.shell.append(sh)
			var lq := _add(W, _cyl(rr * 0.98, w * 0.98, 40), Vector3(cx, yb + rr, v.zc), _liq_mat(), Vector3(0, 0, PI / 2)); v.liq.append(lq)
			for s in [-1, 1]: _add(W, _box(Vector3(0.2, rr * 0.9, rr * 1.6)), Vector3(cx + s * w * 0.35, rr * 0.45, v.zc), inox)
		"plate":
			_add(W, _box(Vector3(w * 1.6, h, 0.55)), Vector3(cx, yb + h * 0.5, 0.0), _mat(Color(0.75, 0.2, 0.15), 0.4, 0.5))
			for i in range(14): _add(W, _box(Vector3(w * 1.5, h - 0.1, 0.012)), Vector3(cx, yb + h * 0.5, -0.28 + i * 0.04), _mat(Color(0.8, 0.82, 0.85), 0.3, 0.9))
			_label(Vector3(cx, yb + h + 0.35, 0.0), "chladič", null)
			return
		"chiller":
			_add(W, _box(Vector3(w, h, 1.1)), Vector3(cx, yb + h * 0.5, -0.4), _mat(Color(0.22, 0.48, 0.7), 0.45, 0.4))
			_add(W, _box(Vector3(w * 0.9, 0.2, 0.05)), Vector3(cx, yb + h * 0.7, 0.16), _emis(Color(0.3, 0.8, 1.0), 1.5))
			_label(Vector3(cx, yb + h + 0.35, -0.4), "glykol", null)
			return
		"inlet":
			_add(W, _cyl(0.12, 0.5, 16), Vector3(cx, yb + 0.25, 0.0), inox)
			_label(Vector3(cx, yb + 0.9, 0.0), "voda z řadu", null)
			return
		"cone", "tall", "lauter", "tank", _:
			_vessel_body(it, v, shell_mat)
	vessels.append(v)

func _shell_mat(v: Dictionary = {}) -> Material:
	if mode == 0: return inox
	if mode == 2:
		var s := _mat(Color(0.80, 0.84, 0.9, 0.20), 0.1, 0.35); s.transparency = BaseMaterial3D.TRANSPARENCY_ALPHA; s.cull_mode = BaseMaterial3D.CULL_DISABLED
		return s
	var sm := ShaderMaterial.new(); sm.shader = cut_shader
	if not v.is_empty(): sm.set_shader_parameter("ctr", Vector3(v.cx, 0.0, v.zc)); sm.set_shader_parameter("rad", v.r)
	return sm

func _liq_mat() -> ShaderMaterial:
	var sm := ShaderMaterial.new(); sm.shader = liq_shader; return sm

func _vessel_body(it: Dictionary, v: Dictionary, shell_mat: Material) -> void:
	var r: float = v.r; var h: float = v.h; var yb: float = v.yb; var cx: float = v.cx; var z: float = v.zc; var shape: String = v.shape
	var hc: float = h * 0.55 if (shape == "cone") else 0.0   # kuželové dno
	var hcyl: float = h * 0.9 - hc if shape != "lauter" else h * 0.8
	if shape == "cone":
		var cone := _add(W, _cyl(r, hc, 40, 0.09), Vector3(cx, yb + hc * 0.5, z), shell_mat); v.shell.append(cone)
		var lc := _add(W, _cyl(r * 0.98, hc, 40, 0.09), Vector3(cx, yb + hc * 0.5, z), _liq_mat()); v.liq.append(lc)
	var by: float = yb + hc
	var body := _add(W, _cyl(r, hcyl, 48), Vector3(cx, by + hcyl * 0.5, z), shell_mat); v.shell.append(body)
	var lb := _add(W, _cyl(r * 0.985, hcyl, 48), Vector3(cx, by + hcyl * 0.5, z), _liq_mat()); v.liq.append(lb)
	v["hc"] = hc; v["hcyl"] = hcyl; v["by"] = by
	var dome := SphereMesh.new(); dome.radius = r; dome.height = r * (0.55 if shape != "lauter" else 0.35); dome.is_hemisphere = true; dome.radial_segments = 48; dome.rings = 12
	var dm: Material = shell_mat
	_add(W, dome, Vector3(cx, by + hcyl, z), dm)
	# obruče, nohy, průlez
	var band: Material = _mat(Color(0.6, 0.62, 0.66), 0.35, 0.9) if mode != 1 else shell_mat
	for f in [0.2, 0.5, 0.85]: _add(W, _cyl(r * 1.012, 0.05, 48), Vector3(cx, by + hcyl * f, z), band)
	var legh: float = yb
	if shape != "cone":
		var bot := SphereMesh.new(); bot.radius = r; bot.height = r * 0.4; bot.is_hemisphere = true; bot.radial_segments = 48; bot.rings = 10
		_add(W, bot, Vector3(cx, by, z), dm, Vector3(PI, 0, 0))
		for a in [0.6, 2.5, 4.4]:
			_add(W, _cyl(0.05, 0.3, 12), Vector3(cx + cos(a) * r * 0.85, yb - 0.02 + 0.15 * 0.0, z + sin(a) * r * 0.85), steel_p)
			_add(W, _cyl(0.09, 0.02, 16), Vector3(cx + cos(a) * r * 0.85, yb - 0.15, z + sin(a) * r * 0.85), steel_b)
	else:
		for a in [0.8, 2.4, 3.9, 5.5]:
			_add(W, _cyl(0.06, hc * 0.9, 10), Vector3(cx + cos(a) * r * 0.9, yb + hc * 0.45 + 0.15, z + sin(a) * r * 0.9), inox, Vector3(0.0, 0, 0))
	_add(W, _cyl(0.26, 0.12, 24), Vector3(cx, by + hcyl * 0.55, z + (r + 0.04) * (1.0 if mode != 1 else -1.0)), _mat(Color(0.6, 0.62, 0.66), 0.35, 0.9), Vector3(PI / 2, 0, 0))
	# sklo stavoznaku na boku
	var gl := _add(W, _cyl(0.05, hcyl * 0.9, 12), Vector3(cx + r + 0.12, by + hcyl * 0.5, z + 0.0), _mat(Color(0.7, 0.8, 0.9, 0.35), 0.1, 0.1))
	(gl.material_override as StandardMaterial3D).transparency = BaseMaterial3D.TRANSPARENCY_ALPHA
	var gl_liq := _add(W, _cyl(0.035, 1.0, 10), Vector3(cx + r + 0.12, by, z), _liq_mat()); v["gl_liq"] = gl_liq
	# plášť (pára) a míchadlo u pánví
	if it.get("bind") in ["rvp", "mp", "wp"] and shape != "cone":
		var jmat := _mat(Color(0.55, 0.57, 0.62, 0.32), 0.3, 0.5); jmat.transparency = BaseMaterial3D.TRANSPARENCY_ALPHA
		var jk := _add(W, _cyl(r * 1.03, hcyl * 0.45, 48), Vector3(cx, by + hcyl * 0.3, z), jmat); v["jacket"] = jk
	if it.get("bind") in ["rvp", "mp"]:
		var shaft := _add(W, _cyl(0.04, hcyl * 0.9, 10), Vector3(cx, by + hcyl * 0.5 + 0.1, z), inox); v["shaft"] = shaft
		var rotor := Node3D.new(); rotor.position = Vector3(cx, by + 0.28, z); W.add_child(rotor); v["rotor"] = rotor
		for a in range(2):
			_add(rotor, _box(Vector3(r * 1.5, 0.06, 0.2)), Vector3(0, 0, 0), inox, Vector3(0, a * PI / 2 + 0.3, 0))
		_add(W, _cyl(0.2, 0.5, 20), Vector3(cx, by + hcyl + r * 0.5 + 0.28, z), _mat(Color(0.15, 0.35, 0.65), 0.45, 0.4))
	if it.get("bind") == "rvp":
		var st := _steam_emitter(Vector3(cx + r * 0.5, by + hcyl + r * 0.5, z)); v["steam"] = st
		_add(W, _cyl(0.17, 1.3, 16), Vector3(cx + r * 0.5, by + hcyl + r * 0.5 + 0.6, z), inox)
	# štítek
	var b = it.get("bind")
	v["label"] = _label(Vector3(cx, by + hcyl + r * 0.6 + 1.0, z), "", b)
	for lq in v.liq:
		var lsm := lq.material_override as ShaderMaterial
		lsm.set_shader_parameter("ctr", Vector3(cx, 0.0, z)); lsm.set_shader_parameter("rad", r); lsm.set_shader_parameter("cut", 1.0 if mode == 1 else 0.0)
	# vnitřek: hrabla a vypouštěcí síto u scezováku, míchadlo u pánví
	if shape == "lauter":
		var ds := _mat(Color(0.7, 0.72, 0.76), 0.4, 0.9)
		_add(W, _cyl(r * 0.97, 0.03, 48), Vector3(cx, by + 0.18, z), ds)
		var arms := Node3D.new(); arms.position = Vector3(cx, by + 0.4, z); W.add_child(arms); v["rotor"] = arms
		for a in range(2):
			var arm := Node3D.new(); arm.rotation.y = a * PI; arms.add_child(arm)
			_add(arm, _box(Vector3(r * 1.85, 0.05, 0.08)), Vector3.ZERO, steel_b)
			for k in range(1, 6):
				_add(arm, _box(Vector3(0.03, 0.34, 0.2)), Vector3(-r * 0.9 + k * r * 0.3, -0.1, 0.0), steel_p, Vector3(0, 0.0, 0.2))

func _steam_emitter(pos: Vector3) -> GPUParticles3D:
	var p := GPUParticles3D.new(); p.position = pos; p.amount = 60; p.lifetime = 3.2; p.emitting = false; p.visibility_aabb = AABB(Vector3(-4, -1, -4), Vector3(8, 10, 8))
	var pm := ParticleProcessMaterial.new(); pm.direction = Vector3(0, 1, 0); pm.spread = 14.0; pm.initial_velocity_min = 1.0; pm.initial_velocity_max = 1.8
	pm.gravity = Vector3(0.15, 0.25, 0.0); pm.scale_min = 0.5; pm.scale_max = 1.2
	var sc := Curve.new(); sc.add_point(Vector2(0, 0.35)); sc.add_point(Vector2(1, 2.2)); var sct := CurveTexture.new(); sct.curve = sc; pm.scale_curve = sct
	p.process_material = pm
	var q := QuadMesh.new(); q.size = Vector2(0.9, 0.9)
	var qm := StandardMaterial3D.new(); qm.shading_mode = BaseMaterial3D.SHADING_MODE_UNSHADED; qm.transparency = BaseMaterial3D.TRANSPARENCY_ALPHA
	qm.albedo_texture = steam_tex; qm.billboard_mode = BaseMaterial3D.BILLBOARD_PARTICLES; qm.vertex_color_use_as_albedo = true; qm.albedo_color = Color(1, 1, 1, 0.8)
	q.material = qm; p.draw_pass_1 = q
	W.add_child(p); steams.append(p); return p

func _label(pos: Vector3, txt: String, bind) -> Label3D:
	var l := Label3D.new(); l.text = txt; l.position = pos; l.billboard = BaseMaterial3D.BILLBOARD_ENABLED; l.font_size = 36; l.pixel_size = 0.0034
	l.outline_size = 10; l.outline_modulate = Color(0, 0, 0, 0.9); l.modulate = Color(1, 1, 1, 1); l.no_depth_test = true; l.render_priority = 5
	if m.font != null: l.font = m.font_b
	W.add_child(l); labels.append({"n": l, "bind": bind}); return l

# ───────────────────────── potrubí ─────────────────────────
func _tube(pts: Array, rad: float, mat: Material, flat_uv: bool = false) -> ArrayMesh:
	var verts := PackedVector3Array(); var norms := PackedVector3Array(); var uvs := PackedVector2Array(); var idx := PackedInt32Array()
	var seg := 10; var acc_len := 0.0
	for i in range(pts.size() - 1):
		var a: Vector3 = pts[i]; var b: Vector3 = pts[i + 1]
		var d := b - a; var L := d.length()
		if L < 0.001: continue
		var dn := d / L
		var up := Vector3.UP if absf(dn.dot(Vector3.UP)) < 0.95 else Vector3.RIGHT
		var s := dn.cross(up).normalized(); var u := s.cross(dn).normalized()
		var base := verts.size()
		for k in range(seg + 1):
			var ang := TAU * k / seg
			var n := s * cos(ang) + u * sin(ang)
			verts.append(a + n * rad); norms.append(n); uvs.append(Vector2(acc_len, float(k) / seg))
			verts.append(b + n * rad); norms.append(n); uvs.append(Vector2(acc_len + L, float(k) / seg))
		for k in range(seg):
			var i0 := base + k * 2
			idx.append_array([i0, i0 + 1, i0 + 2, i0 + 1, i0 + 3, i0 + 2])
		acc_len += L
	var arr := []; arr.resize(Mesh.ARRAY_MAX)
	arr[Mesh.ARRAY_VERTEX] = verts; arr[Mesh.ARRAY_NORMAL] = norms; arr[Mesh.ARRAY_TEX_UV] = uvs; arr[Mesh.ARRAY_INDEX] = idx
	var am := ArrayMesh.new()
	if verts.size() > 0: am.add_surface_from_arrays(Mesh.PRIMITIVE_TRIANGLES, arr)
	return am

func _vessel_hit(x: float, y: float) -> Dictionary:
	for v in vessels:
		var it: Dictionary = v.it
		if x > it.x - 8 and x < it.x + it.w + 8 and y > it.y - 8 and y < it.y + it.h + 8: return v
	return {}

func _pipes() -> void:
	var hose_ids := {}
	for it in m.items:
		if it.type == "pipe" and it.get("hose", false): hose_ids[it.id] = it
	for e in sim.net.edges:
		if hose_ids.has(e.pipe): continue
		var pts: Array = []
		for p in e.pts: pts.append(P(p.x, p.y, ZP))
		var rad: float = clampf(e.dn * 0.00056, 0.016, 0.06)
		var mat := ShaderMaterial.new(); mat.shader = pipe_shader
		var mesh := _tube(pts, rad, mat)
		var mi := MeshInstance3D.new(); mi.mesh = mesh; mi.material_override = mat; W.add_child(mi)
		# kolena
		for k in range(1, pts.size() - 1):
			var sp := SphereMesh.new(); sp.radius = rad * 1.02; sp.height = rad * 2.04; sp.radial_segments = 10; sp.rings = 5
			_add(W, sp, pts[k], mat)
		# nátrubky do nádoby
		for end_i in [0, pts.size() - 1]:
			var pp: Vector2 = e.pts[end_i]
			var vh := _vessel_hit(pp.x, pp.y)
			if vh.is_empty(): continue
			var zs: float = vh.zc + sqrt(maxf(0.02, vh.r * vh.r - pow(pp.x * K - vh.cx, 2))) if vh.shape != "horiz" else vh.zc + vh.r * 0.8
			var stub := [pts[end_i], Vector3(pts[end_i].x, pts[end_i].y, zs)]
			var smi := MeshInstance3D.new(); smi.mesh = _tube(stub, rad, mat); smi.material_override = mat; W.add_child(smi)
		tubes.append({"e": e, "mat": mat})
	for id in hose_ids:
		var it: Dictionary = hose_ids[id]
		var pts2: Array = []
		for p in it.pts: pts2.append(P(p[0], p[1], ZP + 0.12))
		var smi2 := MeshInstance3D.new(); smi2.mesh = _tube(pts2, 0.045, null); smi2.material_override = _mat(Color(0.12, 0.12, 0.14), 0.7, 0.0); W.add_child(smi2)

# ───────────────────────── armatury ─────────────────────────
func _ring(parent: Node3D, pos: Vector3, rad: float, len: float, mat: Material, axis_x: bool = true) -> MeshInstance3D:
	return _add(parent, _cyl(rad, len, 24), pos, mat, Vector3(0, 0, PI / 2) if axis_x else Vector3.ZERO)

func _valve(it: Dictionary) -> void:
	var p := P(it.x, it.y, ZP)
	var n := Node3D.new(); n.position = p; n.rotation.z = deg_to_rad(-float(it.get("rot", 0))); W.add_child(n)
	var kind: String = it.get("kind", "klapka")
	var rp := 0.028
	# tělo klapky + dvě tri-clamp příruby se sevřením
	_ring(n, Vector3.ZERO, rp * 1.5, 0.085, steel_b)
	for s in [-1.0, 1.0]:
		_ring(n, Vector3(s * 0.052, 0, 0), rp * 1.95, 0.016, steel_p)
		_ring(n, Vector3(s * 0.075, 0, 0), rp * 1.7, 0.022, steel_p)
	# krk + převodový/ručkový nástavec
	var neck := _add(n, _cyl(0.026, 0.075, 16), Vector3(0, 0.082, 0), steel_p)
	var stem := Node3D.new(); stem.position = Vector3(0, 0.125, 0); n.add_child(stem)
	stem.rotation.z = -n.rotation.z  # držet páku svisle bez ohledu na natočení potrubí
	_add(stem, _cyl(0.028, 0.02, 16), Vector3.ZERO, steel_p)
	# páka – plochá nerezová s plastovým držadlem a barevným bodem stavu
	var hold := Node3D.new(); stem.add_child(hold)
	_add(hold, _box(Vector3(0.12, 0.008, 0.022)), Vector3(0.06, 0.012, 0), steel_p)
	_add(hold, _box(Vector3(0.07, 0.022, 0.03)), Vector3(0.155, 0.012, 0), _mat(Color(0.12, 0.13, 0.15), 0.6, 0.0))
	var dot := _add(hold, _cyl(0.007, 0.004, 10), Vector3(0.155, 0.025, 0), _emis(Color(0.2, 0.85, 0.3), 0.9))
	valves.append({"it": it, "lever": dot, "stem": stem, "hold": hold, "ang": 0.0, "kind": kind})

func _pump(it: Dictionary) -> void:
	var p := P(it.x, it.y, ZP)
	var n := Node3D.new(); n.position = p; W.add_child(n)
	# hlava čerpadla (voluta), sání do osy, výtlak nahoru
	_add(n, _cyl(0.135, 0.085, 36), Vector3(0, 0, 0.0), steel_b, Vector3(PI / 2, 0, 0))
	_add(n, _cyl(0.06, 0.07, 24), Vector3(0, 0, -0.085), steel_p, Vector3(PI / 2, 0, 0))
	_ring(n, Vector3(0, 0, -0.13), 0.052, 0.018, steel_p, false)
	_add(n, _cyl(0.04, 0.17, 20), Vector3(0, 0.15, 0.0), steel_p)
	_add(n, _cyl(0.052, 0.018, 24), Vector3(0, 0.245, 0.0), steel_p)
	# lucerna a motor s žebry v nerezovém plášti
	_add(n, _cyl(0.09, 0.09, 28), Vector3(0, 0, 0.08), steel_b, Vector3(PI / 2, 0, 0))
	_add(n, _cyl(0.105, 0.34, 36), Vector3(0, 0, 0.30), steel_b, Vector3(PI / 2, 0, 0))
	for i in range(9): _add(n, _cyl(0.113, 0.012, 36), Vector3(0, 0, 0.16 + i * 0.037), steel_p, Vector3(PI / 2, 0, 0))
	_add(n, _box(Vector3(0.11, 0.06, 0.1)), Vector3(0, 0.12, 0.30), steel_b)
	var fan := Node3D.new(); fan.position = Vector3(0, 0, 0.485); n.add_child(fan)
	_add(n, _cyl(0.1, 0.07, 24), Vector3(0, 0, 0.465), _mat(Color(0.13, 0.14, 0.16), 0.55, 0.3), Vector3(PI / 2, 0, 0))
	for i in range(8): _add(fan, _box(Vector3(0.15, 0.012, 0.01)), Vector3.ZERO, _mat(Color(0.06, 0.06, 0.07), 0.5, 0.2), Vector3(0, 0, i * PI / 8))
	# nohy a rám na podlahu
	var h := p.y
	var base_y := -0.19
	_add(n, _box(Vector3(0.3, 0.025, 0.66)), Vector3(0, base_y, 0.27), steel_b)
	for sx in [-1.0, 1.0]:
		for sz in [0.02, 0.5]:
			_add(n, _cyl(0.02, h + base_y, 10), Vector3(sx * 0.12, base_y - (h + base_y) * 0.5, sz), steel_p)
	var led := _add(n, _cyl(0.012, 0.01, 10), Vector3(0.0, 0.176, 0.30), _emis(Color(0.5, 0.1, 0.1), 0.4), Vector3(0, 0, 0))
	var lab := _label(p + Vector3(0, 0.55, 0.3), "", it.get("bind"))
	pumps.append({"it": it, "n": n, "fan": fan, "led": led, "lab": lab, "base": p})

func _flowmeter(it: Dictionary) -> void:
	var p := P(it.x, it.y, ZP)
	_add(W, _cyl(0.09, 0.4, 16), p, _mat(Color(0.7, 0.8, 0.9, 0.5), 0.1, 0.1))

func _deco(it: Dictionary) -> void:
	if it.get("k") == "vent":
		var p := P(it.x, it.y, ZP)
		_add(W, _cyl(0.18, 1.0, 16), p, inox)

func _panel(it: Dictionary) -> void:
	var w: float = it.w * K; var h: float = it.h * K; var cx: float = (it.x + it.w * 0.5) * K; var yb: float = 0.0
	h = 2.2; w = minf(w, 3.6)
	_add(W, _box(Vector3(w, h, 0.5)), Vector3(cx, yb + h * 0.5, -1.0), _mat(Color(0.55, 0.58, 0.62), 0.45, 0.3))
	_add(W, _box(Vector3(w * 0.9, h * 0.9, 0.03)), Vector3(cx, yb + h * 0.5, -0.74), _mat(Color(0.3, 0.32, 0.35), 0.5, 0.3))
	for r in range(5):
		for c in range(7):
			var on: bool = ((r * 7 + c * 3) % 5) == 0
			_add(W, _cyl(0.045, 0.03, 12), Vector3(cx - w * 0.38 + c * w * 0.125, yb + h * 0.85 - r * 0.3, -0.71), _emis(Color(0.2, 0.9, 0.3) if on else Color(0.12, 0.2, 0.14), 1.6 if on else 0.1), Vector3(PI / 2, 0, 0))
	_label(Vector3(cx, yb + h + 0.35, -1.0), "ROZVADĚČ", null)

# ───────────────────────── živý update ─────────────────────────
func _process(dt: float) -> void:
	if not active or m == null or sim == null: return
	t += dt; acc += dt
	_move_keys(dt)
	for v in valves:
		var it: Dictionary = v.it
		var open: bool = it.get("open", true) != false
		var pct: float = float(it.get("part", 100.0 if open else 0.0)) if it.get("kind") == "skrt" else (100.0 if open else 0.0)
		var tg: float = deg_to_rad(90.0 * (1.0 - pct / 100.0))
		v.ang = lerp_angle(v.ang, tg, minf(1.0, dt * 9.0))
		v.stem.rotation.z = -v.stem.get_parent().rotation.z
		v.hold.rotation.y = v.ang
		var lm := v.lever.material_override as StandardMaterial3D
		var c := Color(0.2, 0.85, 0.3) if pct > 0.5 else Color(0.9, 0.2, 0.15)
		if lm.albedo_color != c: lm.albedo_color = c; lm.emission = c
	var S = sim.S
	for p in pumps:
		var b: String = p.it.get("bind", "")
		var on: bool = b in ["p1", "p2", "p3"] and S[b].on
		var q: float = float(S[b].get("flow", 0.0)) if b in ["p1", "p2", "p3"] else 0.0
		var spd: float = (6.0 + q * 0.06) if on else 0.0
		p.fan.rotation.z += spd * dt * 2.0
		var lm2 := p.led.material_override as StandardMaterial3D
		var lc := Color(0.2, 1.0, 0.35) if on else Color(0.5, 0.1, 0.1)
		lm2.albedo_color = lc; lm2.emission = lc; lm2.emission_energy_multiplier = 3.0 if on else 0.4
		p.n.position = p.base + (Vector3(sin(t * 61.0), cos(t * 53.0), 0) * 0.0015 if on else Vector3.ZERO)
	if acc > 0.12:
		acc = 0.0
		_update_tubes(); _update_vessels(); _update_labels()
	for v in vessels:
		if v.has("rotor"):
			var it2: Dictionary = v.it; var b2 = it2.get("bind")
			var stir: bool = (b2 in ["rvp", "mp"] and S[b2].get("stir", false)) or (b2 == "sk" and S.sk.get("rake", false))
			if stir: v.rotor.rotation.y += dt * 2.2

func _update_tubes() -> void:
	for T in tubes:
		var e = T.e
		var full: float = clampf(sim.vol(e.c) / e.cap, 0.0, 1.0)
		var mat: ShaderMaterial = T.mat
		if full < 0.12:
			mat.set_shader_parameter("fill", 0.0); mat.set_shader_parameter("speed", 0.0); continue
		var col: Color = m.kind_color(sim.contentKind(e.c), e.c.T, e.c.col / maxf(0.01, sim.liqVol(e.c)))
		mat.set_shader_parameter("liq", Vector3(col.r, col.g, col.b)); mat.set_shader_parameter("fill", full)
		var sp := 0.0
		if e.flow != 0:
			sp = clampf(0.5 + absf(float(e.flow)) / 220.0, 0.5, 2.5) * (1.0 if e.from == "a" else -1.0)
		mat.set_shader_parameter("speed", sp)

func _update_vessels() -> void:
	var S = sim.S
	for v in vessels:
		var it: Dictionary = v.it
		var col: Color = _liq_col(it)
		var surf: float = m.level_y(it)
		var hpx: float = clampf((it.y + it.h - 3) - surf, 0.0, it.h)
		var lh: float = hpx * K
		var boil: float = 0.0; var stir: float = 0.0
		var b = it.get("bind")
		if b in ["rvp", "mp", "wp"]:
			boil = 1.0 if S[b].get("boiling", false) else 0.0
			stir = 1.0 if S[b].get("stir", false) else 0.0
		var gr := 0.0
		if b in ["rvp", "mp", "wp"] and sim.contentKind(S[b]) == "mash": gr = 1.0
		if b == "sk" and (sim.vol(S.sk.mash) > 50 or S.sk.g > 1.0): gr = 1.0
		for lq in v.liq:
			var sm := lq.material_override as ShaderMaterial
			sm.set_shader_parameter("col", Color(col.r, col.g, col.b, 0.9 if lh > 0.02 else 0.0))
			sm.set_shader_parameter("boil", boil); sm.set_shader_parameter("stir", stir)
			sm.set_shader_parameter("grain", gr)
		# nastavit výšku sloupce kapaliny
		if v.shape == "horiz":
			var rr: float = v.r; var f: float = clampf(lh / (rr * 2.0), 0.0, 1.0)
			v.liq[0].visible = f > 0.02
			v.liq[0].scale = Vector3(1.0, 1.0, maxf(0.02, 1.0))
		elif v.liq.size() > 0:
			var hc: float = v.get("hc", 0.0); var hcyl: float = v.get("hcyl", 1.0)
			var li: int = 0
			if hc > 0.0:
				var lcone: MeshInstance3D = v.liq[0]
				var hh: float = minf(lh, hc)
				lcone.visible = hh > 0.01
				var cm := lcone.mesh as CylinderMesh
				cm.height = maxf(0.01, hh); cm.top_radius = lerpf(0.09, v.r * 0.98, hh / hc) if hc > 0 else v.r
				lcone.position.y = v.yb + hh * 0.5
				li = 1
			var lb: MeshInstance3D = v.liq[li]
			var hh2: float = clampf(lh - hc, 0.0, hcyl)
			lb.visible = hh2 > 0.01
			(lb.mesh as CylinderMesh).height = maxf(0.01, hh2); lb.position.y = v.by + hh2 * 0.5
		if v.has("gl_liq"):
			var gl: MeshInstance3D = v.gl_liq
			var hg: float = clampf(lh - v.get("hc", 0.0), 0.0, v.get("hcyl", 1.0))
			gl.visible = hg > 0.01
			(gl.mesh as CylinderMesh).height = maxf(0.01, hg); gl.position.y = v.by + hg * 0.5
		if v.has("jacket"):
			var heat: float = float(S[b].get("heat", 0.0)) if b in ["rvp", "mp"] else 0.0
			var jm := v.jacket.material_override as StandardMaterial3D
			jm.emission_enabled = heat > 0.05; jm.emission = Color(1.0, 0.45, 0.1); jm.emission_energy_multiplier = heat * 1.6
		if v.has("steam"):
			var bl: bool = S.rvp.get("boiling", false) or (S.rvp.T > 92.0 and S.rvp.w > 100.0)
			(v.steam as GPUParticles3D).emitting = bl

func _liq_col(it: Dictionary) -> Color:
	var col: Color = m.vessel_color(it)
	var b = it.get("bind"); var S = sim.S
	var kind := "water"
	match b:
		"rvp", "mp", "wp": kind = sim.contentKind(S[b])
		"sk": kind = "mash" if sim.vol(S.sk.mash) > 50 else "wort"
		"spilka": kind = "wort"
	if kind == "water" or kind == "ice":
		var T: float = S[b].T if b in ["rvp", "mp", "wp"] else (S.hlt.T if b == "hlt" else (S.lv.T if b == "lv" else 20.0))
		var c := Color.from_hsv(lerpf(0.57, 0.04, clampf((T - 20.0) / 75.0, 0.0, 1.0)), 0.45, 0.95)
		if kind == "ice": c = Color(0.65, 0.85, 1.0)
		return c
	return col

func _update_labels() -> void:
	var S = sim.S
	for L in labels:
		var b = L.bind
		if b == null: continue
		var n: Label3D = L.n
		if b in ["p1", "p2", "p3"]:
			var on: bool = S[b].on
			n.text = ("%s BĚŽÍ · %d L/min" % [m.pump_name(b), roundi(S[b].get("flow", 0.0))]) if on else m.pump_name(b)
			n.modulate = Color(0.5, 1.0, 0.6) if on else Color(0.8, 0.8, 0.85)
		else:
			n.text = m.vessel_text(b).replace(" · ", "\n")
			n.modulate = Color(1, 1, 1)

# ───────────────────────── ovládání ─────────────────────────
func _cam() -> void:
	cam.position = target + Vector3(sin(yaw) * cos(pitch), sin(pitch), cos(yaw) * cos(pitch)) * dist
	cam.look_at(target, Vector3.UP)

func _move_keys(dt: float) -> void:
	var f := float(Input.is_key_pressed(KEY_W) or Input.is_key_pressed(KEY_UP)) - float(Input.is_key_pressed(KEY_S) or Input.is_key_pressed(KEY_DOWN))
	var s := float(Input.is_key_pressed(KEY_D) or Input.is_key_pressed(KEY_RIGHT)) - float(Input.is_key_pressed(KEY_A) or Input.is_key_pressed(KEY_LEFT))
	var u := float(Input.is_key_pressed(KEY_PAGEUP)) - float(Input.is_key_pressed(KEY_PAGEDOWN))
	if f == 0.0 and s == 0.0 and u == 0.0: return
	var spd := dist * 0.8 * dt
	var fw := Vector3(-sin(yaw), 0, -cos(yaw)); var rt := Vector3(cos(yaw), 0, -sin(yaw))
	target += (fw * f + rt * s) * spd + Vector3(0, u * spd, 0)
	_cam()

func handle(e: InputEvent) -> bool:
	if not active: return false
	if e is InputEventMouseMotion:
		if e.button_mask & MOUSE_BUTTON_MASK_LEFT:
			yaw -= e.relative.x * 0.005; pitch = clampf(pitch + e.relative.y * 0.005, -0.05, 1.45); _cam(); return true
		if e.button_mask & (MOUSE_BUTTON_MASK_RIGHT | MOUSE_BUTTON_MASK_MIDDLE):
			var rt := cam.global_transform.basis.x; var up := cam.global_transform.basis.y
			target += (-rt * e.relative.x + up * e.relative.y) * dist * 0.0016; _cam(); return true
	elif e is InputEventMouseButton and e.button_index == MOUSE_BUTTON_LEFT:
		if e.pressed: press_pos = e.position
		elif e.position.distance_to(press_pos) < 5.0: _click(e.position); return true
	if e is InputEventMouseButton and e.pressed:
		if e.button_index == MOUSE_BUTTON_WHEEL_UP: dist = maxf(2.0, dist * 0.9); _cam(); return true
		if e.button_index == MOUSE_BUTTON_WHEEL_DOWN: dist = minf(80.0, dist * 1.1); _cam(); return true
	elif e is InputEventKey and e.pressed and not e.echo:
		if e.keycode == KEY_X:
			mode = (mode + 1) % 3; dirty = true; build(); _cam()
			m.hud.toast(["Nádoby: plný nerez", "Nádoby: řez (vidíš dovnitř)", "Nádoby: sklo"][mode]); return true
	return false

var press_pos := Vector2.ZERO
func _click(sp: Vector2) -> void:
	var best = null; var bd := 34.0
	for v in valves:
		var p: Vector3 = v.stem.get_parent().global_position
		if cam.is_position_behind(p): continue
		var d := cam.unproject_position(p).distance_to(sp)
		if d < bd: bd = d; best = v.it
	for pp in pumps:
		var p2: Vector3 = pp.n.global_position
		if cam.is_position_behind(p2): continue
		var d2 := cam.unproject_position(p2).distance_to(sp)
		if d2 < bd: bd = d2; best = pp.it
	if best != null: m._tap(Vector2(best.x, best.y)); return
	var o := cam.project_ray_origin(sp); var dir := cam.project_ray_normal(sp)
	if absf(dir.z) < 0.001: return
	var tt := (ZP - o.z) / dir.z
	if tt < 0: return
	var q := o + dir * tt
	m._tap(Vector2(q.x / K, floor_y - q.y / K))

func look_at_item(id: String, d: float = 9.0) -> void:
	for it in m.items:
		if it.id == id:
			target = P(it.x + it.get("w", 0) * 0.5, it.y + it.get("h", 0) * 0.5, 0.0); dist = d; _cam(); return
