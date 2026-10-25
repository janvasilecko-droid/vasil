// Vygenerováno z liquidfx.gd překladačem gd2cs.py – needituj ručně.
using System; using System.Collections.Generic;
namespace Pivovar.Gd.Gen {
public partial class S_liquidfx_Gpu : GdNode {
  public Gv polys;
  public Gv mat;
  static S_liquidfx_Gpu() {
  }
  public S_liquidfx_Gpu() {
    TypeName = "Node2D";
    polys = Gd.Arr();
    mat = Gv.Null;
  }
  public Gv _init() {
    Gv sh = Gd.New("Shader");
    sh = sh.S("code", S_liquidfx.LIQUID_SHADER);
    mat = Gd.New("ShaderMaterial");
    mat = mat.S("shader", sh);
    Self.S("material", mat);
    Self.S("show_behind_parent", Gv.B(true));
    return Gv.Null;
  }
  public Gv set_params(Gv col = default(Gv), Gv box = default(Gv), Gv surf_y = default(Gv), Gv depth_px = default(Gv), Gv turb = default(Gv), Gv stir = default(Gv), Gv boil = default(Gv)) {
    surf_y = (surf_y).AsF();
    depth_px = (depth_px).AsF();
    turb = (turb).AsF();
    stir = (stir).AsF();
    boil = (boil).AsF();
    Gv cl = col;
    Gv cd = Gd.Ctor("Color", (Gd.pow(col.G("r"), Gv.F(1.8)) * Gv.F(0.92)), (Gd.pow(col.G("g"), Gv.F(1.8)) * Gv.F(0.92)), (Gd.pow(col.G("b"), Gv.F(1.8)) * Gv.F(0.92)), Gv.F(1.0));
    mat.C("set_shader_parameter", Gv.S("col_light"), Gd.Ctor("Color", Gd.minf(Gv.F(1.0), ((cl.G("r") * Gv.F(1.12)) + Gv.F(0.03))), Gd.minf(Gv.F(1.0), ((cl.G("g") * Gv.F(1.1)) + Gv.F(0.03))), Gd.minf(Gv.F(1.0), ((cl.G("b") * Gv.F(1.06)) + Gv.F(0.02))), Gv.F(1.0)));
    mat.C("set_shader_parameter", Gv.S("col_deep"), cd);
    mat.C("set_shader_parameter", Gv.S("box_pos"), box.G("position"));
    mat.C("set_shader_parameter", Gv.S("box_size"), box.G("size"));
    mat.C("set_shader_parameter", Gv.S("surf_y"), surf_y);
    mat.C("set_shader_parameter", Gv.S("depth_px"), depth_px);
    mat.C("set_shader_parameter", Gv.S("turb"), Gd.clampf(turb, Gv.F(0.0), Gv.F(1.0)));
    mat.C("set_shader_parameter", Gv.S("stir"), stir);
    mat.C("set_shader_parameter", Gv.S("boil"), boil);
    mat.C("set_shader_parameter", Gv.S("alpha"), col.G("a"));
    return Gv.Null;
  }
  public Gv set_polys(Gv p = default(Gv)) {
    polys = p;
    Self.C("queue_redraw");
    return Gv.Null;
  }
  public Gv _draw() {
    foreach (Gv q in (polys).Iter()) {
      if (Gv.Ge(q.C("size"), Gv.I(3L))) {
        Self.C("draw_colored_polygon", q, Gd.StaticGet("Color", "WHITE"));
      }
    }
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "polys": return this.polys;
    case "mat": return this.mat;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "polys": this.polys = v; return v;
    case "mat": this.mat = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "_init": return _init();
    case "set_params": return set_params(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6));
    case "set_polys": return set_polys(Gd.A(a, 0));
    case "_draw": return _draw();
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_liquidfx_Gpu(); o._init(); return Gv.O(o); }
  public override string ScriptName { get { return "S_liquidfx_Gpu"; } }
}
public partial class S_liquidfx : GdObject {
  public static Gv _tex;
  public static Gv LIQUID_SHADER;
  static S_liquidfx() {
    _tex = Gd.Dict();
    LIQUID_SHADER = Gv.S("\nshader_type canvas_item;\nuniform vec4 col_light : source_color = vec4(0.9, 0.7, 0.3, 1.0);\nuniform vec4 col_deep : source_color = vec4(0.6, 0.35, 0.1, 1.0);\nuniform vec2 box_pos = vec2(0.0);\nuniform vec2 box_size = vec2(100.0);\nuniform float surf_y = 0.0;\nuniform float depth_px = 300.0;\nuniform float turb = 0.0;\nuniform float stir = 0.0;\nuniform float boil = 0.0;\nuniform float alpha = 0.9;\nuniform float absorb = 2.2;\nvarying vec2 wp;\nvoid vertex() { wp = VERTEX; }\nfloat hash(vec2 p) { p = fract(p * vec2(123.34, 456.21)); p += dot(p, p + 45.32); return fract(p.x * p.y); }\nvec2 hash2(vec2 p) { return vec2(hash(p), hash(p + 17.13)); }\nfloat vnoise(vec2 p) {\n    vec2 i = floor(p); vec2 f = fract(p); f = f * f * (3.0 - 2.0 * f);\n    return mix(mix(hash(i), hash(i + vec2(1, 0)), f.x), mix(hash(i + vec2(0, 1)), hash(i + vec2(1, 1)), f.x), f.y);\n}\nfloat ridge(vec2 p) { float n = vnoise(p) * 0.6 + vnoise(p * 2.1 + 7.3) * 0.4; return pow(1.0 - abs(n * 2.0 - 1.0), 7.0); }\nfloat specks(vec2 p, float scale, float dens, float t, float flow) {\n    vec2 q = p / scale;\n    // proudění: konvekční buňky při míchání/varu, jinak pomalé stoupání/klesání\n    q += vec2(sin(p.y * 0.021 + t * 0.7), cos(p.x * 0.017 - t * 0.6)) * (0.5 * flow);\n    q.y += t * (0.04 + 0.5 * flow * sin(p.x * 0.013));\n    vec2 id = floor(q); vec2 f = fract(q);\n    vec2 r = hash2(id);\n    if (r.x > dens) return 0.0;\n    vec2 c = 0.5 + 0.3 * vec2(sin(t * (0.3 + r.y) + r.y * 6.28), cos(t * (0.4 + r.x) + r.x * 6.28));\n    float d = length(f - c);\n    return smoothstep(0.17, 0.0, d) * (0.5 + 0.5 * r.y);\n}\nvoid fragment() {\n    float t = TIME;\n    float depth = clamp((wp.y - surf_y) / max(depth_px, 1.0), 0.0, 1.0);\n    float k = 1.0 - exp(-absorb * depth);\n    vec3 c = mix(col_light.rgb, col_deep.rgb, k);\n    float sx = clamp((wp.x - box_pos.x) / box_size.x, 0.0, 1.0);\n    float edge = abs(sx * 2.0 - 1.0);\n    float shade = 1.0 - 0.38 * pow(edge, 3.0) + 0.10 * exp(-pow((sx - 0.30) * 6.0, 2.0));\n    c *= shade;\n    // zákal: mléčný rozptyl světla\n    c = mix(c, vec3(0.90, 0.80, 0.58) * shade, turb * 0.42);\n    // kaustika (odlesky světla lomeného na hladině), slábne s hloubkou\n    float fl = clamp(stir + boil, 0.0, 1.5);\n    vec2 cp = wp * 0.036 + vec2(t * (0.05 + 0.10 * fl), t * 0.03);\n    float ca = ridge(cp) * 0.7 + ridge(cp * 1.9 + vec2(-t * 0.07, 3.1)) * 0.5;\n    c += vec3(1.0, 0.96, 0.85) * ca * 0.16 * (1.0 - 0.75 * k) * (1.0 - 0.6 * turb);\n    // tenký světlý pás pod hladinou (lom světla, meniscus)\n    float sd = wp.y - surf_y;\n    c += vec3(1.0, 0.97, 0.88) * exp(-sd / 7.0) * 0.16;\n    // mikročástice: dvě měřítka\n    float flow = clamp(stir + boil * 1.2, 0.0, 1.5);\n    float sp = specks(wp, 11.0, 0.20 + 0.55 * turb, t, flow) * 0.55 + specks(wp + 31.0, 19.0, 0.10 + 0.35 * turb, t * 0.8, flow) * 0.7;\n    c = mix(c, vec3(1.0, 0.95, 0.78), clamp(sp, 0.0, 1.0) * (0.22 + 0.5 * turb));\n    // bubliny při varu: světlé tečky stoupající ode dna\n    if (boil > 0.05) {\n        vec2 bq = vec2(wp.x / 14.0, wp.y / 18.0 + t * 1.4);\n        vec2 bid = floor(bq); vec2 bf = fract(bq); vec2 br = hash2(bid);\n        float bd = length(bf - (0.2 + 0.6 * br));\n        float bub = smoothstep(0.16 + 0.08 * br.x, 0.05, bd) * step(br.y, 0.35 * boil);\n        c += vec3(1.0) * bub * 0.25;\n    }\n    COLOR = vec4(c, alpha);\n}\n");
  }
  public S_liquidfx() {
  }
  public static Gv h(Gv i = default(Gv), Gv s = default(Gv)) {
    i = (i).AsF();
    if (s.k == 0) s = (Gv.F(0.0)).AsF();
    return (Gd.fposmod((Gd.sin(((i * Gv.F(12.9898)) + (s * Gv.F(78.233)))) * Gv.F(43758.5453)), Gv.F(1.0))).AsF();
    return Gv.Null;
  }
  public static Gv _vn(Gv x = default(Gv), Gv y = default(Gv), Gv per = default(Gv), Gv seed = default(Gv)) {
    x = (x).AsF();
    y = (y).AsF();
    per = (per).AsI();
    seed = (seed).AsF();
    Gv xi = (Gd.Dyn("int", Gd.floor(x))).AsI();
    Gv yi = (Gd.Dyn("int", Gd.floor(y))).AsI();
    Gv fx = ((x - xi)).AsF();
    Gv fy = ((y - yi)).AsF();
    fx = (((fx * fx) * ((Gv.F(3.0) - (Gv.F(2.0) * fx))))).AsF();
    fy = (((fy * fy) * ((Gv.F(3.0) - (Gv.F(2.0) * fy))))).AsF();
    Gv x0 = (Gd.posmod(xi, per)).AsI();
    Gv x1 = (Gd.posmod((xi + Gv.I(1L)), per)).AsI();
    Gv y0 = (Gd.posmod(yi, per)).AsI();
    Gv y1 = (Gd.posmod((yi + Gv.I(1L)), per)).AsI();
    Gv a = (h((x0 + (y0 * Gv.F(57.0))), seed)).AsF();
    Gv b = (h((x1 + (y0 * Gv.F(57.0))), seed)).AsF();
    Gv c = (h((x0 + (y1 * Gv.F(57.0))), seed)).AsF();
    Gv d = (h((x1 + (y1 * Gv.F(57.0))), seed)).AsF();
    return (Gd.lerpf(Gd.lerpf(a, b, fx), Gd.lerpf(c, d, fx), fy)).AsF();
    return Gv.Null;
  }
  public static Gv _fbm(Gv x = default(Gv), Gv y = default(Gv), Gv per = default(Gv), Gv seed = default(Gv)) {
    x = (x).AsF();
    y = (y).AsF();
    per = (per).AsI();
    seed = (seed).AsF();
    return (((_vn(x, y, per, seed) * Gv.F(0.62)) + (_vn((x * Gv.F(2.5)), (y * Gv.F(2.5)), (per * Gv.I(2L)), (seed + Gv.F(1.0))) * Gv.F(0.38)))).AsF();
    return Gv.Null;
  }
  public static Gv _stamp(Gv img = default(Gv), Gv cx = default(Gv), Gv cy = default(Gv), Gv ang = default(Gv), Gv len = default(Gv), Gv wid = default(Gv), Gv col = default(Gv), Gv shade = default(Gv)) {
    cx = (cx).AsF();
    cy = (cy).AsF();
    ang = (ang).AsF();
    len = (len).AsF();
    wid = (wid).AsF();
    shade = (shade).AsF();
    Gv W = (img.C("get_width")).AsI();
    Gv H = (img.C("get_height")).AsI();
    Gv ca = (Gd.cos(ang)).AsF();
    Gv sa = (Gd.sin(ang)).AsF();
    Gv R = (Gd.Dyn("int", Gd.ceil(((len * Gv.F(0.5)) + Gv.F(2.0))))).AsI();
    foreach (Gv dy in Gd.RangeIter((-R), (R + Gv.I(1L)))) {
      foreach (Gv dx in Gd.RangeIter((-R), (R + Gv.I(1L)))) {
        Gv u = (((((dx * ca) + (dy * sa))) / ((len * Gv.F(0.5))))).AsF();
        Gv v = ((((((-dx) * sa) + (dy * ca))) / ((wid * Gv.F(0.5))))).AsF();
        Gv rr = ((((u * u) * Gv.F(0.9)) + (v * v))).AsF();
        if (Gv.Gt(rr, Gv.F(1.0))) {
          continue;
        }
        Gv px = (Gd.posmod((Gd.Dyn("int", Gd.round(cx)) + dx), W)).AsI();
        Gv py = (Gd.posmod((Gd.Dyn("int", Gd.round(cy)) + dy), H)).AsI();
        Gv lit = (((Gv.F(1.0) + (shade * (((-v) * Gv.F(0.55))))) - ((Gv.F(0.25) * rr) * rr))).AsF();
        Gv c = Gd.Ctor("Color", (col.G("r") * lit), (col.G("g") * lit), (col.G("b") * lit), Gv.F(1.0));
        img.C("set_pixel", px, py, c);
      }
    }
    return Gv.Null;
  }
  public static Gv _mk(Gv img = default(Gv)) {
    return Gd.Static("ImageTexture", "create_from_image", img);
    return Gv.Null;
  }
  public static Gv tex_bed() {
    if ((_tex.C("has", Gv.S("bed"))).T()) {
      return _tex.I(Gv.S("bed"));
    }
    Gv W = (Gv.I(192L)).AsI();
    Gv H = (Gv.I(128L)).AsI();
    Gv img = Gd.Static("Image", "create", W, H, Gv.B(false), Gd.StaticGet("Image", "FORMAT_RGBA8"));
    foreach (Gv y in (H).Iter()) {
      foreach (Gv x in (W).Iter()) {
        Gv n = (_fbm((x / Gv.F(22.0)), (y / Gv.F(22.0)), Gv.I(8L), Gv.F(3.0))).AsF();
        Gv g = (((Gv.F(0.34) + (Gv.F(0.34) * n)) + (Gv.F(0.1) * h((x + (y * Gv.F(211.0))), Gv.F(1.0))))).AsF();
        img.C("set_pixel", x, y, Gd.Ctor("Color", (g * Gv.F(1.06)), (g * Gv.F(0.94)), (g * Gv.F(0.74)), Gv.F(1.0)));
      }
    }
    Gv pal = Gd.Arr(Gd.Ctor("Color", Gv.F(0.84), Gv.F(0.72), Gv.F(0.5)), Gd.Ctor("Color", Gv.F(0.72), Gv.F(0.58), Gv.F(0.38)), Gd.Ctor("Color", Gv.F(0.6), Gv.F(0.47), Gv.F(0.3)), Gd.Ctor("Color", Gv.F(0.46), Gv.F(0.35), Gv.F(0.21)), Gd.Ctor("Color", Gv.F(0.9), Gv.F(0.82), Gv.F(0.64)), Gd.Ctor("Color", Gv.F(0.38), Gv.F(0.28), Gv.F(0.17)));
    foreach (Gv i in (Gv.I(1560L)).Iter()) {
      Gv cx = ((h(i, Gv.F(7.0)) * W)).AsF();
      Gv cy = ((h(i, Gv.F(8.0)) * H)).AsF();
      Gv ang = ((Gv.Lt(h(i, Gv.F(10.0)), Gv.F(0.7)) ? (h(i, Gv.F(9.0)) * Gv.F(3.14159)) : (((h(i, Gv.F(9.0)) - Gv.F(0.5))) * Gv.F(0.9)))).AsF();
      Gv ln = ((Gv.F(3.6) + (h(i, Gv.F(11.0)) * Gv.F(5.4)))).AsF();
      Gv k = (h(i, Gv.F(12.0))).AsF();
      Gv pc = pal.I(Gd.Dyn("int", ((k * k) * Gv.F(5.99))));
      _stamp(img, cx, cy, ang, ln, (ln * ((Gv.F(0.4) + (Gv.F(0.18) * h(i, Gv.F(13.0)))))), pc, Gv.F(0.7));
    }
    foreach (Gv i_2 in (Gv.I(520L)).Iter()) {
      Gv x_2 = (Gd.Dyn("int", (h(i_2, Gv.F(21.0)) * W))).AsI();
      Gv y_2 = (Gd.Dyn("int", (h(i_2, Gv.F(22.0)) * H))).AsI();
      Gv c = img.C("get_pixel", x_2, y_2);
      Gv k_2 = ((Gv.Lt(h(i_2, Gv.F(23.0)), Gv.F(0.5)) ? Gv.F(1.25) : Gv.F(0.7))).AsF();
      img.C("set_pixel", x_2, y_2, Gd.Ctor("Color", Gd.minf(Gv.F(1.0), (c.G("r") * k_2)), Gd.minf(Gv.F(1.0), (c.G("g") * k_2)), Gd.minf(Gv.F(1.0), (c.G("b") * k_2)), Gv.F(1.0)));
    }
    foreach (Gv i_3 in (Gv.I(260L)).Iter()) {
      Gv x2 = (Gd.Dyn("int", (h(i_3, Gv.F(31.0)) * W))).AsI();
      Gv y2 = (Gd.Dyn("int", (h(i_3, Gv.F(32.0)) * H))).AsI();
      img.C("set_pixel", x2, y2, Gd.Ctor("Color", Gv.I(1L), Gv.F(0.97), Gv.F(0.88), Gv.I(1L)));
    }
    _tex = _tex.SI(Gv.S("bed"), _mk(img));
    return _tex.I(Gv.S("bed"));
    return Gv.Null;
  }
  public static Gv tex_mash() {
    if ((_tex.C("has", Gv.S("mash"))).T()) {
      return _tex.I(Gv.S("mash"));
    }
    Gv W = (Gv.I(128L)).AsI();
    Gv H = (Gv.I(128L)).AsI();
    Gv img = Gd.Static("Image", "create", W, H, Gv.B(false), Gd.StaticGet("Image", "FORMAT_RGBA8"));
    foreach (Gv y in (H).Iter()) {
      foreach (Gv x in (W).Iter()) {
        Gv n = (_fbm((x / Gv.F(30.0)), (y / Gv.F(30.0)), Gv.I(4L), Gv.F(11.0))).AsF();
        Gv g = (((Gv.F(0.8) + (Gv.F(0.22) * ((n - Gv.F(0.5))))) + (Gv.F(0.06) * h((x + (y * Gv.F(131.0))), Gv.F(2.0))))).AsF();
        img.C("set_pixel", x, y, Gd.Ctor("Color", (g * Gv.F(1.03)), (g * Gv.F(0.98)), (g * Gv.F(0.9)), Gv.F(1.0)));
      }
    }
    Gv pal = Gd.Arr(Gd.Ctor("Color", Gv.F(0.7), Gv.F(0.58), Gv.F(0.38)), Gd.Ctor("Color", Gv.F(0.58), Gv.F(0.46), Gv.F(0.28)), Gd.Ctor("Color", Gv.F(0.95), Gv.F(0.88), Gv.F(0.7)), Gd.Ctor("Color", Gv.F(0.86), Gv.F(0.76), Gv.F(0.54)));
    foreach (Gv i in (Gv.I(320L)).Iter()) {
      Gv cx = ((h(i, Gv.F(41.0)) * W)).AsF();
      Gv cy = ((h(i, Gv.F(42.0)) * H)).AsF();
      Gv ang = ((h(i, Gv.F(43.0)) * Gv.F(3.14))).AsF();
      Gv ln = ((Gv.F(4.0) + (h(i, Gv.F(44.0)) * Gv.F(6.0)))).AsF();
      Gv k = (Gd.Dyn("int", (h(i, Gv.F(45.0)) * Gv.F(3.99)))).AsI();
      _stamp(img, cx, cy, ang, ln, (ln * Gv.F(0.38)), pal.I(k), Gv.F(0.5));
    }
    _tex = _tex.SI(Gv.S("mash"), _mk(img));
    return _tex.I(Gv.S("mash"));
    return Gv.Null;
  }
  public static Gv tex_trub() {
    if ((_tex.C("has", Gv.S("trub"))).T()) {
      return _tex.I(Gv.S("trub"));
    }
    Gv W = (Gv.I(128L)).AsI();
    Gv H = (Gv.I(128L)).AsI();
    Gv img = Gd.Static("Image", "create", W, H, Gv.B(false), Gd.StaticGet("Image", "FORMAT_RGBA8"));
    foreach (Gv y in (H).Iter()) {
      foreach (Gv x in (W).Iter()) {
        Gv n = (_fbm((x / Gv.F(14.0)), (y / Gv.F(14.0)), Gv.I(9L), Gv.F(51.0))).AsF();
        Gv g = (((Gv.F(0.78) + (Gv.F(0.34) * ((n - Gv.F(0.5))))) + (Gv.F(0.05) * h((x + (y * Gv.F(77.0))), Gv.F(5.0))))).AsF();
        img.C("set_pixel", x, y, Gd.Ctor("Color", (g * Gv.F(1.04)), (g * Gv.F(0.97)), (g * Gv.F(0.82)), Gv.F(1.0)));
      }
    }
    foreach (Gv i in (Gv.I(300L)).Iter()) {
      Gv cx = ((h(i, Gv.F(61.0)) * W)).AsF();
      Gv cy = ((h(i, Gv.F(62.0)) * H)).AsF();
      Gv ang = ((h(i, Gv.F(63.0)) * Gv.F(6.28))).AsF();
      Gv ln = ((Gv.F(3.0) + (h(i, Gv.F(64.0)) * Gv.F(6.0)))).AsF();
      Gv k = (Gv.Lt(h(i, Gv.F(65.0)), Gv.F(0.35)) ? Gd.Ctor("Color", Gv.F(0.58), Gv.F(0.55), Gv.F(0.3)) : Gd.Ctor("Color", Gv.F(1.0), Gv.F(0.97), Gv.F(0.86)));
      _stamp(img, cx, cy, ang, ln, (ln * Gv.F(0.7)), k, Gv.F(0.3));
    }
    _tex = _tex.SI(Gv.S("trub"), _mk(img));
    return _tex.I(Gv.S("trub"));
    return Gv.Null;
  }
  public static Gv tex_foam() {
    if ((_tex.C("has", Gv.S("foam"))).T()) {
      return _tex.I(Gv.S("foam"));
    }
    Gv W = (Gv.I(192L)).AsI();
    Gv H = (Gv.I(96L)).AsI();
    Gv img = Gd.Static("Image", "create", W, H, Gv.B(false), Gd.StaticGet("Image", "FORMAT_RGBA8"));
    foreach (Gv y in (H).Iter()) {
      foreach (Gv x in (W).Iter()) {
        Gv n = (_fbm((x / Gv.F(18.0)), (y / Gv.F(18.0)), Gv.I(10L), Gv.F(71.0))).AsF();
        Gv g = ((Gv.F(0.9) + (Gv.F(0.1) * n))).AsF();
        img.C("set_pixel", x, y, Gd.Ctor("Color", g, (g * Gv.F(0.985)), (g * Gv.F(0.93)), Gv.F(1.0)));
      }
    }
    foreach (Gv i in (Gv.I(260L)).Iter()) {
      Gv cx = ((h(i, Gv.F(81.0)) * W)).AsF();
      Gv cy = ((h(i, Gv.F(82.0)) * H)).AsF();
      Gv r = ((Gv.F(3.0) + (h(i, Gv.F(83.0)) * Gv.F(7.5)))).AsF();
      Gv shade = ((Gv.F(0.8) + (Gv.F(0.18) * h(i, Gv.F(84.0))))).AsF();
      Gv R = ((Gd.Dyn("int", Gd.ceil(r)) + Gv.I(1L))).AsI();
      foreach (Gv dy in Gd.RangeIter((-R), (R + Gv.I(1L)))) {
        foreach (Gv dx in Gd.RangeIter((-R), (R + Gv.I(1L)))) {
          Gv d = (Gd.sqrt(Gd.Dyn("float", ((dx * dx) + (dy * dy))))).AsF();
          if (Gv.Gt(d, r)) {
            continue;
          }
          Gv px = (Gd.posmod((Gd.Dyn("int", cx) + dx), W)).AsI();
          Gv py = (Gd.posmod((Gd.Dyn("int", cy) + dy), H)).AsI();
          Gv edge = (Gd.smoothstep((r - Gv.F(1.6)), r, d)).AsF();
          Gv hl = (Gd.smoothstep((r * Gv.F(0.55)), Gv.F(0.0), Gd.sqrt(Gd.Dyn("float", ((((dx + (r * Gv.F(0.35)))) * ((dx + (r * Gv.F(0.35))))) + (((dy + (r * Gv.F(0.35)))) * ((dy + (r * Gv.F(0.35)))))))))).AsF();
          Gv base_ = (((shade * ((Gv.F(1.0) - (Gv.F(0.34) * edge)))) + (Gv.F(0.22) * hl))).AsF();
          img.C("set_pixel", px, py, Gd.Ctor("Color", Gd.minf(Gv.F(1.0), (base_ * Gv.F(1.02))), Gd.minf(Gv.F(1.0), base_), Gd.minf(Gv.F(1.0), (base_ * Gv.F(0.94))), Gv.F(1.0)));
        }
      }
    }
    _tex = _tex.SI(Gv.S("foam"), _mk(img));
    return _tex.I(Gv.S("foam"));
    return Gv.Null;
  }
  public static Gv draw_tex_poly(Gv ci = default(Gv), Gv pts = default(Gv), Gv cols = default(Gv), Gv tex = default(Gv), Gv off = default(Gv), Gv px_per_texel = default(Gv)) {
    px_per_texel = (px_per_texel).AsF();
    Gv sz = (tex.C("get_size") * px_per_texel);
    Gv uv = Gd.Ctor("PackedVector2Array");
    foreach (Gv p in (pts).Iter()) {
      uv.C("append", (((p + off)) / sz));
    }
    ci.C("draw_polygon", pts, cols, uv, tex);
    return Gv.Null;
  }
  public static Gv draw_suspended(Gv ci = default(Gv), Gv rect = default(Gv), Gv outline = default(Gv), Gv amount = default(Gv), Gv t = default(Gv), Gv speed = default(Gv), Gv col = default(Gv), Gv seed = default(Gv), Gv big = default(Gv)) {
    amount = (amount).AsF();
    t = (t).AsF();
    speed = (speed).AsF();
    seed = (seed).AsF();
    if (big.k == 0) big = (Gv.F(1.0)).AsF();
    if (Gv.Lt(amount, Gv.F(0.02))) {
      return Gv.Null;
    }
    Gv n = (Gd.Dyn("int", (Gd.clampf((rect.C("get_area") / Gv.F(420.0)), Gv.F(10.0), Gv.F(140.0)) * amount))).AsI();
    foreach (Gv i in (n).Iter()) {
      Gv u = (h(i, seed)).AsF();
      Gv v = (h(i, (seed + Gv.F(1.0)))).AsF();
      Gv ph = ((((t * speed) * ((Gv.F(0.35) + (Gv.F(0.65) * h(i, (seed + Gv.F(2.0))))))) + (h(i, (seed + Gv.F(3.0))) * Gv.F(6.28)))).AsF();
      Gv cx = ((rect.G("position").G("x") + (rect.G("size").G("x") * ((Gv.F(0.5) + ((((u - Gv.F(0.5))) * Gv.F(0.84)) * Gd.cos(((ph * Gv.F(0.5)) + (u * Gv.F(3.0)))))))))).AsF();
      Gv cy = ((rect.G("position").G("y") + (rect.G("size").G("y") * ((v + (Gv.F(0.045) * Gd.sin((ph * Gv.F(1.3))))))))).AsF();
      Gv p = Gd.Ctor("Vector2", cx, cy);
      if ((Gv.Gt(outline.C("size"), Gv.I(2L)) && !(Gd.Static("Geometry2D", "is_point_in_polygon", p, outline)).T())) {
        continue;
      }
      Gv depth = ((Gv.F(0.5) + (Gv.F(0.5) * Gd.sin(((ph * Gv.F(0.5)) + (u * Gv.F(3.0))))))).AsF();
      Gv ln = (((((Gv.F(3.0) + (Gv.F(4.0) * h(i, (seed + Gv.F(4.0)))))) * big) * ((Gv.F(0.7) + (Gv.F(0.5) * depth))))).AsF();
      Gv c = col.C("lightened", (Gv.F(0.18) * ((h(i, (seed + Gv.F(5.0))) - Gv.F(0.4)))));
      c = c.S("a", (((Gv.F(0.35) + (Gv.F(0.5) * depth))) * Gd.minf(Gv.F(1.0), amount)));
      Gv ang = (((ph * Gv.F(1.7)) + (h(i, (seed + Gv.F(6.0))) * Gv.F(3.0)))).AsF();
      Gv d = ((Gd.Ctor("Vector2", Gd.cos(ang), Gd.sin(ang)) * ln) * Gv.F(0.5));
      ci.C("draw_line", (p - d), (p + d), c, Gd.maxf(Gv.F(1.3), (ln * Gv.F(0.33))), Gv.B(true));
    }
    return Gv.Null;
  }
  public static Gv draw_flocs(Gv ci = default(Gv), Gv rect = default(Gv), Gv outline = default(Gv), Gv amount = default(Gv), Gv t = default(Gv), Gv seed = default(Gv)) {
    amount = (amount).AsF();
    t = (t).AsF();
    seed = (seed).AsF();
    if (Gv.Lt(amount, Gv.F(0.02))) {
      return Gv.Null;
    }
    Gv n = (Gd.Dyn("int", (Gd.clampf((rect.C("get_area") / Gv.F(480.0)), Gv.F(10.0), Gv.F(160.0)) * amount))).AsI();
    foreach (Gv i in (n).Iter()) {
      Gv u = (h(i, seed)).AsF();
      Gv ph = (((t * ((Gv.F(0.25) + (Gv.F(0.5) * h(i, (seed + Gv.F(1.0))))))) + (h(i, (seed + Gv.F(2.0))) * Gv.F(6.28)))).AsF();
      Gv cx = ((rect.G("position").G("x") + (rect.G("size").G("x") * Gd.clampf((u + (Gv.F(0.05) * Gd.sin((ph * Gv.F(1.7))))), Gv.F(0.02), Gv.F(0.98))))).AsF();
      Gv cy = ((rect.G("position").G("y") + (rect.G("size").G("y") * ((Gv.F(0.5) + (Gv.F(0.48) * Gd.sin(ph))))))).AsF();
      Gv p = Gd.Ctor("Vector2", cx, cy);
      if ((Gv.Gt(outline.C("size"), Gv.I(2L)) && !(Gd.Static("Geometry2D", "is_point_in_polygon", p, outline)).T())) {
        continue;
      }
      Gv r = ((Gv.F(0.9) + (Gv.F(1.5) * h(i, (seed + Gv.F(3.0)))))).AsF();
      Gv c = Gd.Ctor("Color", Gv.F(0.98), Gv.F(0.91), Gv.F(0.7), (Gv.F(0.45) + (Gv.F(0.3) * h(i, (seed + Gv.F(4.0))))));
      ci.C("draw_circle", p, r, c);
      ci.C("draw_circle", (p + Gd.Ctor("Vector2", (r * Gv.F(0.9)), ((-r) * Gv.F(0.2)))), (r * Gv.F(0.6)), Gd.Ctor("Color", c, (c.G("a") * Gv.F(0.8))));
      ci.C("draw_circle", (p + Gd.Ctor("Vector2", ((-r) * Gv.F(0.8)), (r * Gv.F(0.4)))), (r * Gv.F(0.5)), Gd.Ctor("Color", c, (c.G("a") * Gv.F(0.7))));
      ci.C("draw_circle", (p + Gd.Ctor("Vector2", (r * Gv.F(0.2)), (r * Gv.F(0.9)))), (r * Gv.F(0.45)), Gd.Ctor("Color", c, (c.G("a") * Gv.F(0.6))));
    }
    return Gv.Null;
  }
  public static Gv draw_hops(Gv ci = default(Gv), Gv rect = default(Gv), Gv outline = default(Gv), Gv amount = default(Gv), Gv t = default(Gv), Gv seed = default(Gv), Gv surface_y = default(Gv), Gv calm = default(Gv)) {
    amount = (amount).AsF();
    t = (t).AsF();
    seed = (seed).AsF();
    surface_y = (surface_y).AsF();
    calm = (calm).AsF();
    if (Gv.Lt(amount, Gv.F(0.02))) {
      return Gv.Null;
    }
    Gv n = (Gd.Dyn("int", (Gd.clampf((rect.C("get_area") / Gv.F(520.0)), Gv.F(8.0), Gv.F(130.0)) * amount))).AsI();
    foreach (Gv i in (n).Iter()) {
      Gv u = (h(i, seed)).AsF();
      Gv ph = ((((t * ((Gv.F(0.2) + (Gv.F(0.55) * h(i, (seed + Gv.F(1.0))))))) * ((Gv.F(1.0) - (Gv.F(0.8) * calm)))) + (h(i, (seed + Gv.F(2.0))) * Gv.F(6.28)))).AsF();
      Gv cx = ((rect.G("position").G("x") + (rect.G("size").G("x") * Gd.clampf((u + (Gv.F(0.04) * Gd.sin((ph * Gv.F(1.3))))), Gv.F(0.02), Gv.F(0.98))))).AsF();
      Gv by = ((rect.G("position").G("y") + (rect.G("size").G("y") * ((Gv.F(0.5) + (Gv.F(0.45) * Gd.sin(ph))))))).AsF();
      Gv cy = (Gd.lerpf(by, ((surface_y + Gv.F(3.0)) + (Gv.F(6.0) * h(i, (seed + Gv.F(7.0))))), (calm * Gv.F(0.65)))).AsF();
      Gv p = Gd.Ctor("Vector2", cx, cy);
      if ((Gv.Gt(outline.C("size"), Gv.I(2L)) && !(Gd.Static("Geometry2D", "is_point_in_polygon", p, outline)).T())) {
        continue;
      }
      Gv ln = ((Gv.F(1.8) + (Gv.F(2.6) * h(i, (seed + Gv.F(3.0)))))).AsF();
      Gv g = (h(i, (seed + Gv.F(4.0)))).AsF();
      Gv c = (Gv.Lt(g, Gv.F(0.5)) ? Gd.Ctor("Color", Gv.F(0.4), Gv.F(0.42), Gv.F(0.12), Gv.F(0.85)) : ((Gv.Lt(g, Gv.F(0.85)) ? Gd.Ctor("Color", Gv.F(0.52), Gv.F(0.45), Gv.F(0.14), Gv.F(0.85)) : Gd.Ctor("Color", Gv.F(0.27), Gv.F(0.24), Gv.F(0.08), Gv.F(0.85)))));
      Gv ang = (((h(i, (seed + Gv.F(5.0))) * Gv.F(3.14)) + (ph * Gv.F(0.6)))).AsF();
      Gv d = ((Gd.Ctor("Vector2", Gd.cos(ang), Gd.sin(ang)) * ln) * Gv.F(0.5));
      ci.C("draw_line", (p - d), (p + d), c, Gv.F(1.9), Gv.B(true));
    }
    return Gv.Null;
  }
  public static Gv draw_foam(Gv ci = default(Gv), Gv x0 = default(Gv), Gv x1 = default(Gv), Gv surf = default(Gv), Gv thick = default(Gv), Gv hops = default(Gv), Gv t = default(Gv)) {
    x0 = (x0).AsF();
    x1 = (x1).AsF();
    thick = (thick).AsF();
    hops = (hops).AsF();
    t = (t).AsF();
    if ((Gv.Lt(thick, Gv.F(1.0)) || Gv.Lt(surf.C("size"), Gv.I(2L)))) {
      return Gv.Null;
    }
    Gv tex = tex_foam();
    Gv top = Gd.Ctor("PackedVector2Array");
    Gv bot = Gd.Ctor("PackedVector2Array");
    Gv cols_t = Gd.Ctor("PackedColorArray");
    foreach (Gv i in (surf.C("size")).Iter()) {
      Gv q = surf.I(i);
      Gv r = ((Gd.Dyn("float", i) / Gd.Dyn("float", (surf.C("size") - Gv.I(1L))))).AsF();
      Gv bump = ((Gv.F(0.55) + ((Gv.F(0.45) * Gd.sin(((r * Gv.F(17.0)) + (t * Gv.F(1.6))))) * Gd.sin(((r * Gv.F(7.0)) - (t * Gv.F(0.9))))))).AsF();
      top.C("append", Gd.Ctor("Vector2", q.G("x"), ((q.G("y") - (thick * bump)) + Gv.F(2.0))));
      bot.C("append", Gd.Ctor("Vector2", q.G("x"), (q.G("y") + Gv.F(3.0))));
    }
    Gv poly = Gd.Ctor("PackedVector2Array");
    poly.C("append_array", top);
    foreach (Gv i_2 in Gd.RangeIter((bot.C("size") - Gv.I(1L)), Gv.I(-1L), Gv.I(-1L))) {
      poly.C("append", bot.I(i_2));
    }
    Gv cols = Gd.Ctor("PackedColorArray");
    foreach (Gv p in (poly).Iter()) {
      Gv edge = (Gd.absf((((p.G("x") - (((x0 + x1)) * Gv.F(0.5)))) / ((((x1 - x0)) * Gv.F(0.5)))))).AsF();
      Gv tint = (Gd.clampf(((hops * Gv.F(0.55)) + (edge * Gv.F(0.25))), Gv.F(0.0), Gv.F(0.8))).AsF();
      cols.C("append", Gd.Ctor("Color", Gv.F(1.0), Gv.F(0.98), Gv.F(0.92)).C("lerp", Gd.Ctor("Color", Gv.F(0.86), Gv.F(0.72), Gv.F(0.42)), tint));
    }
    Gv polys = Gd.Arr(poly);
    ci.C("draw_polygon", poly, cols, _uvs(poly, tex, Gd.Ctor("Vector2", (t * Gv.F(4.0)), Gv.F(0.0)), Gv.F(0.9)), tex);
    ci.C("draw_polyline", top, Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.55)), Gv.F(1.4), Gv.B(true));
    return Gv.Null;
  }
  public static Gv _uvs(Gv pts = default(Gv), Gv tex = default(Gv), Gv off = default(Gv), Gv px = default(Gv)) {
    px = (px).AsF();
    Gv sz = (tex.C("get_size") * px);
    Gv uv = Gd.Ctor("PackedVector2Array");
    foreach (Gv p in (pts).Iter()) {
      uv.C("append", (((p + off)) / sz));
    }
    return uv;
    return Gv.Null;
  }
  public static Gv draw_trub_cone(Gv ci = default(Gv), Gv cx = default(Gv), Gv floor_y = default(Gv), Gv half_w = default(Gv), Gv height = default(Gv), Gv t = default(Gv)) {
    cx = (cx).AsF();
    floor_y = (floor_y).AsF();
    half_w = (half_w).AsF();
    height = (height).AsF();
    t = (t).AsF();
    if (Gv.Lt(height, Gv.F(2.0))) {
      return Gv.Null;
    }
    Gv tex = tex_trub();
    Gv pts = Gd.Ctor("PackedVector2Array");
    Gv cols = Gd.Ctor("PackedColorArray");
    Gv N = (Gv.I(18L)).AsI();
    foreach (Gv i in ((N + Gv.I(1L))).Iter()) {
      Gv u = ((((Gd.Dyn("float", i) / Gd.Dyn("float", N)) * Gv.F(2.0)) - Gv.F(1.0))).AsF();
      Gv prof = ((Gd.pow((Gv.F(1.0) - Gd.absf(u)), Gv.F(0.75)) * ((Gv.F(1.0) + (Gv.F(0.04) * Gd.sin((u * Gv.F(23.0)))))))).AsF();
      pts.C("append", Gd.Ctor("Vector2", (cx + (u * half_w)), (floor_y - (height * prof))));
    }
    pts.C("append", Gd.Ctor("Vector2", (cx + half_w), (floor_y + Gv.F(1.0))));
    pts.C("append", Gd.Ctor("Vector2", (cx - half_w), (floor_y + Gv.F(1.0))));
    foreach (Gv p in (pts).Iter()) {
      Gv rel = (Gd.clampf((((floor_y - p.G("y"))) / Gd.maxf(Gv.F(1.0), height)), Gv.F(0.0), Gv.F(1.0))).AsF();
      Gv side = (Gd.absf((((p.G("x") - cx)) / Gd.maxf(Gv.F(1.0), half_w)))).AsF();
      Gv k = (((Gv.F(0.88) + (Gv.F(0.22) * rel)) - ((Gv.F(0.32) * side) * side))).AsF();
      cols.C("append", Gd.Ctor("Color", (Gv.F(0.93) * k), (Gv.F(0.82) * k), (Gv.F(0.6) * k), Gv.F(1.0)));
    }
    ci.C("draw_polygon", pts, cols, _uvs(pts, tex, Gd.StaticGet("Vector2", "ZERO"), Gv.F(1.2)), tex);
    Gv rim = Gd.Ctor("PackedVector2Array");
    foreach (Gv i_2 in Gd.RangeIter(Gv.I(0L), (N + Gv.I(1L)))) {
      rim.C("append", (pts.I(i_2) + Gd.Ctor("Vector2", Gv.I(0L), Gv.F(1.5))));
    }
    ci.C("draw_polyline", rim, Gd.Ctor("Color", Gv.I(1L), Gv.F(0.95), Gv.F(0.8), Gv.F(0.35)), Gv.F(1.5), Gv.B(true));
    ci.C("draw_line", Gd.Ctor("Vector2", ((cx - half_w) - Gv.I(2L)), (floor_y + Gv.I(1L))), Gd.Ctor("Vector2", ((cx + half_w) + Gv.I(2L)), (floor_y + Gv.I(1L))), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.3)), Gv.F(2.0));
    return Gv.Null;
  }
  public static Gv tex_puff() {
    if ((_tex.C("has", Gv.S("puff"))).T()) {
      return _tex.I(Gv.S("puff"));
    }
    Gv n = Gv.I(64L);
    Gv img = Gd.Static("Image", "create", n, n, Gv.B(false), Gd.StaticGet("Image", "FORMAT_RGBA8"));
    foreach (Gv yy in (n).Iter()) {
      foreach (Gv xx in (n).Iter()) {
        Gv d = ((Gd.Ctor("Vector2", ((xx - (n * Gv.F(0.5))) + Gv.F(0.5)), ((yy - (n * Gv.F(0.5))) + Gv.F(0.5))).C("length") / ((n * Gv.F(0.5))))).AsF();
        Gv a = (Gd.clampf((Gv.F(1.0) - d), Gv.F(0.0), Gv.F(1.0))).AsF();
        a = (((a * a) * ((Gv.F(3.0) - (Gv.F(2.0) * a))))).AsF();
        a = ((a * (Gv.F(0.85) + (Gv.F(0.15) * _vn((xx / Gv.F(9.0)), (yy / Gv.F(9.0)), Gv.I(8L), Gv.F(3.0)))))).AsF();
        img.C("set_pixel", xx, yy, Gd.Ctor("Color", Gv.F(0.97), Gv.F(0.98), Gv.F(1.0), a));
      }
    }
    Gv t = Gd.Static("ImageTexture", "create_from_image", img);
    _tex = _tex.SI(Gv.S("puff"), t);
    return t;
    return Gv.Null;
  }
  public static Gv prewarm() {
    tex_bed();
    tex_mash();
    tex_trub();
    tex_foam();
    tex_puff();
    return Gv.Null;
  }
  public static Gv particles(Gv ci = default(Gv), Gv st = default(Gv), Gv key = default(Gv), Gv rect = default(Gv), Gv outline = default(Gv), Gv dt = default(Gv), Gv tt = default(Gv), Gv p = default(Gv)) {
    dt = (dt).AsF();
    tt = (tt).AsF();
    if ((Gv.Lt(rect.G("size").G("x"), Gv.F(8.0)) || Gv.Lt(rect.G("size").G("y"), Gv.F(8.0)))) {
      return Gv.Null;
    }
    Gv seed = (Gd.Dyn("float", p.C("get", Gv.S("seed"), Gv.F(1.0)))).AsF();
    Gv mash = Gv.B(Gv.Eq(p.C("get", Gv.S("kind"), Gv.S("wort")), Gv.S("mash")));
    Gv n = (Gd.Dyn("int", Gd.clampf((rect.C("get_area") / (((mash).T() ? Gv.F(260.0) : Gv.F(340.0)))), Gv.F(24.0), Gv.F(260.0)))).AsI();
    Gv pos = st.C("get", key, Gd.Ctor("PackedVector2Array"));
    Gv ph = st.C("get", (key + Gv.S("_ph")), Gd.Ctor("PackedFloat32Array"));
    if (Gv.Ne(pos.C("size"), n)) {
      pos = Gd.Ctor("PackedVector2Array");
      ph = Gd.Ctor("PackedFloat32Array");
      foreach (Gv i in (n).Iter()) {
        pos.C("append", Gd.Ctor("Vector2", h(i, seed), h(i, (seed + Gv.F(1.0)))));
        ph.C("append", (h(i, (seed + Gv.F(2.0))) * Gv.F(6.2832)));
      }
    }
    Gv stir = (Gd.Dyn("float", p.C("get", Gv.S("stir"), Gv.F(0.0)))).AsF();
    Gv boil = p.C("get", Gv.S("boil"), Gv.B(false));
    Gv whirl = (Gd.Dyn("float", p.C("get", Gv.S("whirl"), Gv.F(0.0)))).AsF();
    Gv A = (((Gv.F(46.0) * stir) + (((boil).T() ? Gv.F(62.0) : Gv.F(0.0))))).AsF();
    Gv a_husk = (Gd.Dyn("float", p.C("get", Gv.S("a_husk"), Gv.F(0.0)))).AsF();
    Gv a_floc = (Gd.Dyn("float", p.C("get", Gv.S("a_floc"), Gv.F(0.0)))).AsF();
    Gv a_hop = (Gd.Dyn("float", p.C("get", Gv.S("a_hop"), Gv.F(0.0)))).AsF();
    Gv a_fine = (Gd.Dyn("float", p.C("get", Gv.S("a_fine"), Gv.F(0.0)))).AsF();
    Gv base_ = p.C("get", Gv.S("col"), Gd.Ctor("Color", Gv.F(0.72), Gv.F(0.58), Gv.F(0.36)));
    Gv W = (rect.G("size").G("x")).AsF();
    Gv Hh = (rect.G("size").G("y")).AsF();
    foreach (Gv i_2 in (n).Iter()) {
      Gv c = (h(i_2, (seed + Gv.F(3.0)))).AsF();
      Gv kind = (Gv.I(0L)).AsI();
      Gv act = (h(i_2, (seed + Gv.F(4.0)))).AsF();
      if ((mash).T()) {
        if (Gv.Lt(c, Gv.F(0.55))) {
          kind = (Gv.I(0L)).AsI();
        } else if (Gv.Lt(c, Gv.F(0.85))) {
          kind = (Gv.I(1L)).AsI();
        } else {
          kind = (Gv.I(4L)).AsI();
        }
        if (Gv.Gt(act, a_husk)) {
          continue;
        }
      } else {
        if (Gv.Lt(c, Gv.F(0.34))) {
          kind = (Gv.I(2L)).AsI();
        } else if (Gv.Lt(c, Gv.F(0.62))) {
          kind = (Gv.I(3L)).AsI();
        } else {
          kind = (Gv.I(4L)).AsI();
        }
        Gv amt = ((Gv.Eq(kind, Gv.I(2L)) ? a_floc : ((Gv.Eq(kind, Gv.I(3L)) ? a_hop : a_fine)))).AsF();
        if (Gv.Gt(act, amt)) {
          continue;
        }
      }
      Gv q = pos.I(i_2);
      Gv b = ((Gv.Eq(kind, Gv.I(0L)) ? Gv.F(-0.9) : ((Gv.Eq(kind, Gv.I(1L)) ? Gv.F(0.55) : (((Gv.Eq(kind, Gv.I(3L)) && false) ? Gv.F(0.9) : ((Gv.Eq(kind, Gv.I(2L)) ? Gv.F(0.35) : Gv.F(0.12))))))))).AsF();
      if (Gv.Eq(kind, Gv.I(3L))) {
        b = ((Gv.F(-0.25) + (Gv.F(0.5) * h(i_2, (seed + Gv.F(7.0)))))).AsF();
      }
      Gv u = (((Gv.F(-0.5) * Gd.sin((Gv.F(6.2832) * q.G("x")))) * Gd.cos((Gv.F(3.1416) * q.G("y"))))).AsF();
      Gv vy = ((Gd.cos((Gv.F(6.2832) * q.G("x"))) * Gd.sin((Gv.F(3.1416) * q.G("y"))))).AsF();
      Gv vx_px = ((((A * u) * Gv.F(2.0)) + ((((h(i_2, (seed + Gv.F(8.0))) - Gv.F(0.5))) * Gv.F(4.0)) * ((Gv.F(1.0) + (Gv.F(3.0) * stir)))))).AsF();
      Gv vy_px = (((A * vy) + (b * ((Gv.F(16.0) - (Gv.F(11.0) * Gd.clampf((stir + (((boil).T() ? Gv.F(1.0) : Gv.F(0.0)))), Gv.F(0.0), Gv.F(1.0)))))))).AsF();
      ph = ph.SI(i_2, (ph.I(i_2) + (dt * (((Gv.F(1.0) + (Gv.F(2.5) * stir)) + (Gv.F(2.0) * whirl))))));
      if (Gv.Gt(whirl, Gv.F(0.02))) {
        vx_px = ((vx_px + (((whirl * Gv.F(55.0)) * Gd.cos((ph.I(i_2) * Gv.F(1.3)))) * ((Gv.F(0.4) + (Gv.F(0.6) * h(i_2, (seed + Gv.F(9.0))))))))).AsF();
        vx_px = ((vx_px + ((((Gv.F(0.5) - q.G("x"))) * Gv.F(38.0)) * whirl))).AsF();
        vy_px = ((vy_px + (Gv.F(11.0) * whirl))).AsF();
      }
      q = q.S("x", (q.G("x") + ((vx_px * dt) / W)));
      q = q.S("y", (q.G("y") + ((vy_px * dt) / Hh)));
      q = q.S("x", Gd.clampf(q.G("x"), Gv.F(0.02), Gv.F(0.98)));
      q = q.S("y", Gd.clampf(q.G("y"), Gv.F(0.012), Gv.F(0.985)));
      pos = pos.SI(i_2, q);
      Gv sp = (rect.G("position") + Gd.Ctor("Vector2", (q.G("x") * W), (q.G("y") * Hh)));
      if ((Gv.Gt(outline.C("size"), Gv.I(2L)) && !(Gd.Static("Geometry2D", "is_point_in_polygon", sp, outline)).T())) {
        continue;
      }
      Gv depth = ((Gv.F(0.55) + (Gv.F(0.45) * Gd.sin(((ph.I(i_2) * Gv.F(0.8)) + (q.G("x") * Gv.F(5.0))))))).AsF();
      Gv ang = (((ph.I(i_2) * ((Gv.F(0.4) + (Gv.F(0.6) * stir)))) + (h(i_2, (seed + Gv.F(5.0))) * Gv.F(3.14)))).AsF();
      {
        Gv _t1 = kind;
        if (Gv.Eq(_t1, Gv.I(0L))) {
          Gv ln = ((((Gv.F(3.2) + (Gv.F(3.0) * h(i_2, (seed + Gv.F(6.0)))))) * ((Gv.F(0.75) + (Gv.F(0.35) * depth))))).AsF();
          Gv cc = base_.C("lerp", Gd.Ctor("Color", Gv.F(0.92), Gv.F(0.82), Gv.F(0.58)), (Gv.F(0.35) * h(i_2, (seed + Gv.F(10.0)))));
          cc = cc.C("darkened", (Gv.F(0.3) * ((Gv.F(1.0) - depth))));
          cc = cc.S("a", (Gv.F(0.55) + (Gv.F(0.4) * depth)));
          Gv d = ((Gd.Ctor("Vector2", Gd.cos(ang), Gd.sin(ang)) * ln) * Gv.F(0.5));
          ci.C("draw_line", (sp - d), (sp + d), cc, (Gv.F(1.5) + (Gv.F(0.9) * depth)), Gv.B(true));
          ci.C("draw_line", ((sp - d) + Gd.Ctor("Vector2", Gv.I(0L), Gv.F(-0.6))), ((sp + d) + Gd.Ctor("Vector2", Gv.I(0L), Gv.F(-0.6))), cc.C("lightened", Gv.F(0.28)), Gv.F(0.8), Gv.B(true));
        }
        else if (Gv.Eq(_t1, Gv.I(1L))) {
          Gv gc = Gd.Ctor("Color", Gv.F(0.9), Gv.F(0.8), Gv.F(0.58), (Gv.F(0.45) + (Gv.F(0.35) * depth)));
          ci.C("draw_circle", sp, (Gv.F(0.9) + (Gv.F(0.7) * h(i_2, (seed + Gv.F(6.0))))), gc);
        }
        else if (Gv.Eq(_t1, Gv.I(2L))) {
          Gv r = ((Gv.F(0.9) + (Gv.F(1.4) * h(i_2, (seed + Gv.F(6.0)))))).AsF();
          Gv fc = Gd.Ctor("Color", Gv.F(0.98), Gv.F(0.92), Gv.F(0.72), (Gv.F(0.4) + (Gv.F(0.35) * depth)));
          ci.C("draw_circle", sp, r, fc);
          ci.C("draw_circle", (sp + Gd.Ctor("Vector2", (r * Gv.F(0.9)), ((-r) * Gv.F(0.2)))), (r * Gv.F(0.6)), Gd.Ctor("Color", fc, (fc.G("a") * Gv.F(0.8))));
          ci.C("draw_circle", (sp + Gd.Ctor("Vector2", ((-r) * Gv.F(0.7)), (r * Gv.F(0.5)))), (r * Gv.F(0.5)), Gd.Ctor("Color", fc, (fc.G("a") * Gv.F(0.7))));
        }
        else if (Gv.Eq(_t1, Gv.I(3L))) {
          Gv g = (h(i_2, (seed + Gv.F(11.0)))).AsF();
          Gv hc = (Gv.Lt(g, Gv.F(0.5)) ? Gd.Ctor("Color", Gv.F(0.4), Gv.F(0.42), Gv.F(0.12), Gv.F(0.85)) : ((Gv.Lt(g, Gv.F(0.85)) ? Gd.Ctor("Color", Gv.F(0.52), Gv.F(0.45), Gv.F(0.14), Gv.F(0.85)) : Gd.Ctor("Color", Gv.F(0.27), Gv.F(0.24), Gv.F(0.08), Gv.F(0.85)))));
          Gv hl = ((Gv.F(1.8) + (Gv.F(2.4) * h(i_2, (seed + Gv.F(6.0)))))).AsF();
          Gv hd = ((Gd.Ctor("Vector2", Gd.cos(ang), Gd.sin(ang)) * hl) * Gv.F(0.5));
          ci.C("draw_line", (sp - hd), (sp + hd), hc, Gv.F(1.7), Gv.B(true));
        }
        else if (true) {
          ci.C("draw_circle", sp, (Gv.F(0.7) + (Gv.F(0.5) * h(i_2, (seed + Gv.F(6.0))))), Gd.Ctor("Color", (base_.G("r") * Gv.F(1.05)), (base_.G("g") * Gv.F(1.0)), (base_.G("b") * Gv.F(0.9)), (Gv.F(0.3) + (Gv.F(0.25) * depth))));
        }
      }
    }
    st = st.SI(key, pos);
    st = st.SI((key + Gv.S("_ph")), ph);
    return Gv.Null;
  }
  public static Gv draw_caustics(Gv ci = default(Gv), Gv rect = default(Gv), Gv outline = default(Gv), Gv tt = default(Gv), Gv strength = default(Gv), Gv col = default(Gv)) {
    tt = (tt).AsF();
    strength = (strength).AsF();
    if ((Gv.Lt(rect.G("size").G("y"), Gv.F(14.0)) || Gv.Lt(strength, Gv.F(0.02)))) {
      return Gv.Null;
    }
    Gv nb = (Gd.Dyn("int", Gd.clampf((rect.G("size").G("y") / Gv.F(38.0)), Gv.F(2.0), Gv.F(8.0)))).AsI();
    foreach (Gv k in (nb).Iter()) {
      Gv yy = (((rect.G("position").G("y") + (rect.G("size").G("y") * ((Gv.F(0.1) + ((Gv.F(0.8) * Gd.Dyn("float", k)) / Gd.Dyn("float", nb)))))) + (Gv.F(4.0) * Gd.sin(((tt * Gv.F(0.6)) + (k * Gv.F(2.1))))))).AsF();
      Gv pts = Gd.Ctor("PackedVector2Array");
      Gv inside = Gv.B(true);
      foreach (Gv i in (Gv.I(14L)).Iter()) {
        Gv px = ((rect.G("position").G("x") + ((rect.G("size").G("x") * i) / Gv.F(13.0)))).AsF();
        Gv py = (((yy + (Gv.F(3.5) * Gd.sin((((px * Gv.F(0.035)) + (tt * ((Gv.F(0.9) + (Gv.F(0.13) * k))))) + k)))) + (Gv.F(2.0) * Gd.sin(((px * Gv.F(0.09)) - (tt * Gv.F(1.3))))))).AsF();
        Gv pt = Gd.Ctor("Vector2", px, py);
        if ((Gv.Gt(outline.C("size"), Gv.I(2L)) && !(Gd.Static("Geometry2D", "is_point_in_polygon", pt, outline)).T())) {
          inside = Gv.B(false);
        }
        pts.C("append", pt);
      }
      if (!(inside).T()) {
        continue;
      }
      Gv fade = ((Gv.F(1.0) - ((Gd.Dyn("float", k) / Gd.Dyn("float", nb)) * Gv.F(0.7)))).AsF();
      ci.C("draw_polyline", pts, Gd.Ctor("Color", Gv.F(1.0), Gv.F(0.96), Gv.F(0.82), ((Gv.F(0.07) * strength) * fade)), Gv.F(6.0), Gv.B(true));
      ci.C("draw_polyline", pts, Gd.Ctor("Color", Gv.F(1.0), Gv.F(1.0), Gv.F(0.92), ((Gv.F(0.1) * strength) * fade)), Gv.F(1.5), Gv.B(true));
    }
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "_tex": return S_liquidfx._tex;
    case "LIQUID_SHADER": return S_liquidfx.LIQUID_SHADER;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "_tex": S_liquidfx._tex = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "h": return S_liquidfx.h(Gd.A(a, 0), Gd.A(a, 1));
    case "_vn": return S_liquidfx._vn(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "_fbm": return S_liquidfx._fbm(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "_stamp": return S_liquidfx._stamp(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6), Gd.A(a, 7));
    case "_mk": return S_liquidfx._mk(Gd.A(a, 0));
    case "tex_bed": return S_liquidfx.tex_bed();
    case "tex_mash": return S_liquidfx.tex_mash();
    case "tex_trub": return S_liquidfx.tex_trub();
    case "tex_foam": return S_liquidfx.tex_foam();
    case "draw_tex_poly": return S_liquidfx.draw_tex_poly(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5));
    case "draw_suspended": return S_liquidfx.draw_suspended(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6), Gd.A(a, 7), Gd.A(a, 8));
    case "draw_flocs": return S_liquidfx.draw_flocs(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5));
    case "draw_hops": return S_liquidfx.draw_hops(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6), Gd.A(a, 7));
    case "draw_foam": return S_liquidfx.draw_foam(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6));
    case "_uvs": return S_liquidfx._uvs(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "draw_trub_cone": return S_liquidfx.draw_trub_cone(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5));
    case "tex_puff": return S_liquidfx.tex_puff();
    case "prewarm": return S_liquidfx.prewarm();
    case "particles": return S_liquidfx.particles(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6), Gd.A(a, 7));
    case "draw_caustics": return S_liquidfx.draw_caustics(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5));
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    case "_tex": return _tex;
    case "LIQUID_SHADER": return LIQUID_SHADER;
    case "Gpu": return Gd.Script("S_liquidfx_Gpu");
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "h": return h(Gd.A(a, 0), Gd.A(a, 1));
    case "_vn": return _vn(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "_fbm": return _fbm(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "_stamp": return _stamp(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6), Gd.A(a, 7));
    case "_mk": return _mk(Gd.A(a, 0));
    case "tex_bed": return tex_bed();
    case "tex_mash": return tex_mash();
    case "tex_trub": return tex_trub();
    case "tex_foam": return tex_foam();
    case "draw_tex_poly": return draw_tex_poly(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5));
    case "draw_suspended": return draw_suspended(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6), Gd.A(a, 7), Gd.A(a, 8));
    case "draw_flocs": return draw_flocs(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5));
    case "draw_hops": return draw_hops(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6), Gd.A(a, 7));
    case "draw_foam": return draw_foam(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6));
    case "_uvs": return _uvs(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "draw_trub_cone": return draw_trub_cone(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5));
    case "tex_puff": return tex_puff();
    case "prewarm": return prewarm();
    case "particles": return particles(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6), Gd.A(a, 7));
    case "draw_caustics": return draw_caustics(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5));
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_liquidfx(); return Gv.O(o); }
  public override string ScriptName { get { return "liquidfx"; } }
}
}
