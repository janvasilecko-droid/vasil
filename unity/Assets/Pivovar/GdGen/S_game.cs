// Vygenerováno z game.gd překladačem gd2cs.py – needituj ručně.
using System; using System.Collections.Generic;
namespace Pivovar.Gd.Gen {
public partial class S_game_HoverLayer : GdNode {
  public Gv g;
  public Gv m;
  public Gv cur;
  public Gv ripples;
  public Gv prev_key;
  public Gv prev_lines;
  static S_game_HoverLayer() {
  }
  public S_game_HoverLayer() {
    g = Gv.Null;
    m = Gv.Null;
    cur = Gd.Dict();
    ripples = Gd.Arr();
    prev_key = Gv.S("");
    prev_lines = Gd.Arr();
  }
  public Gv _process(Gv delta = default(Gv)) {
    delta = (delta).AsF();
    Gv vp = Self.C("get_viewport");
    Gv sp = vp.C("get_mouse_position");
    Gv over_gui = Gv.B(Gv.Ne(vp.C("gui_get_hovered_control"), Gv.Null));
    if (((Gv.Ne(m.G("dock"), Gv.Null) && (m.G("dock").G("visible")).T()) && (m.G("dock").C("get_global_rect").C("has_point", sp)).T())) {
      over_gui = Gv.B(true);
    }
    if ((Gv.Ne(g.G("quest"), Gv.Null) && (g.G("quest").C("get_global_rect").C("has_point", sp)).T())) {
      over_gui = Gv.B(true);
    }
    Gv h = (((over_gui).T() || Gv.Eq(m.G("cam"), Gv.Null)) ? Gd.Dict() : m.C("pick", m.C("screen_to_world", sp)));
    if (Gv.Ne(h.C("get", Gv.S("key"), Gv.S("")), cur.C("get", Gv.S("key"), Gv.S("")))) {
      Gd.Static("Input", "set_default_cursor_shape", ((h.C("get", Gv.S("act"), Gv.B(false))).T() ? Gd.StaticGet("Input", "CURSOR_POINTING_HAND") : Gd.StaticGet("Input", "CURSOR_ARROW")));
    }
    cur = h;
    Gv key = ((Gd.Static("Input", "is_key_pressed", Gd.K("KEY_ALT"))).T() ? h.C("get", Gv.S("key"), Gv.S("")) : Gv.S(""));
    if (Gv.Ne(key, prev_key)) {
      prev_key = key;
      prev_lines = ((key.C("begins_with", Gv.S("valve"))).T() ? m.C("flow_reach", key) : Gd.Arr());
    }
    g.C("show_tip", h.C("get", Gv.S("text"), Gv.S("")), sp);
    foreach (Gv r__it in (ripples).Iter()) {
      Gv r = r__it;
      r = r.S("t", (r.G("t") + delta));
    }
    ripples = ripples.C("filter", Gd.Fn((_a) => { Gv r_2 = Gd.A(_a, 0); return Gv.B(Gv.Lt(r_2.G("t"), Gv.F(0.5))); return Gv.Null; }));
    Self.C("queue_redraw");
    return Gv.Null;
  }
  public Gv ripple(Gv w = default(Gv)) {
    ripples.C("append", Gd.Dict(Gv.S("p"), w, Gv.S("t"), Gv.F(0.0)));
    return Gv.Null;
  }
  public Gv _draw() {
    Gv z = ((Gv.Ne(m.G("cam"), Gv.Null) ? m.G("cam").G("zoom").G("x") : Gv.F(1.0))).AsF();
    Gv tt = (Gd.Static("Time", "get_ticks_msec") / Gv.F(1000.0));
    if (!(cur.C("is_empty")).T()) {
      Gv col = cur.C("get", Gv.S("col"), Gd.Ctor("Color", Gv.S("5aa9ff")));
      Gv pul = (Gv.F(0.5) + (Gv.F(0.5) * Gd.sin((tt * Gv.F(6.0)))));
      if ((cur.C("has", Gv.S("rect"))).T()) {
        Gv r = cur.G("rect");
        Self.C("draw_rect", r.C("grow", (Gv.F(8.0) / z)), Gd.Ctor("Color", col, (Gv.F(0.1) + (Gv.F(0.08) * pul))));
        Self.C("draw_rect", r.C("grow", (Gv.F(8.0) / z)), Gd.Ctor("Color", col, (Gv.F(0.6) + (Gv.F(0.3) * pul))), Gv.B(false), (Gv.F(2.5) / z));
      } else if ((cur.C("has", Gv.S("pos"))).T()) {
        Gv R = ((cur.C("get", Gv.S("r"), Gv.F(22.0)) + ((Gv.F(4.0) * pul) / z))).AsF();
        Self.C("draw_circle", cur.G("pos"), R, Gd.Ctor("Color", col, Gv.F(0.14)));
        Self.C("draw_arc", cur.G("pos"), R, Gv.I(0L), Gd.K("TAU"), Gv.I(40L), Gd.Ctor("Color", col, (Gv.F(0.75) + (Gv.F(0.25) * pul))), (Gv.F(3.0) / z), Gv.B(true));
      }
    }
    foreach (Gv pl in (prev_lines).Iter()) {
      Self.C("draw_polyline", pl, Gd.Ctor("Color", Gv.F(0.35), Gv.F(0.75), Gv.F(1.0), Gv.F(0.35)), (Gv.F(14.0) / z), Gv.B(true));
      Self.C("draw_polyline", pl, Gd.Ctor("Color", Gv.F(0.6), Gv.F(0.9), Gv.F(1.0), Gv.F(0.9)), (Gv.F(3.0) / z), Gv.B(true));
    }
    foreach (Gv rp in (ripples).Iter()) {
      Gv k = ((rp.G("t") / Gv.F(0.5))).AsF();
      Self.C("draw_arc", rp.G("p"), (((Gv.F(10.0) + (Gv.F(50.0) * k))) / z), Gv.I(0L), Gd.K("TAU"), Gv.I(40L), Gd.Ctor("Color", Gv.I(1L), Gv.F(0.85), Gv.F(0.3), (Gv.F(0.8) * ((Gv.F(1.0) - k)))), (Gv.F(3.0) / z), Gv.B(true));
    }
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "g": return this.g;
    case "m": return this.m;
    case "cur": return this.cur;
    case "ripples": return this.ripples;
    case "prev_key": return this.prev_key;
    case "prev_lines": return this.prev_lines;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "g": this.g = v; return v;
    case "m": this.m = v; return v;
    case "cur": this.cur = v; return v;
    case "ripples": this.ripples = v; return v;
    case "prev_key": this.prev_key = v; return v;
    case "prev_lines": this.prev_lines = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "_process": return _process(Gd.A(a, 0));
    case "ripple": return ripple(Gd.A(a, 0));
    case "_draw": return _draw();
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_game_HoverLayer(); return Gv.O(o); }
  public override string ScriptName { get { return "S_game_HoverLayer"; } }
}
public partial class S_game : GdNode {
  public static Gv Art;
  public static Gv YEL;
  public Gv m;
  public Gv quest;
  public Gv q_step;
  public Gv q_title;
  public Gv q_hint;
  public Gv q_detail;
  public Gv q_bar;
  public Gv q_btn;
  public Gv q_body;
  public Gv feed;
  public Gv tip;
  public Gv tip_lbl;
  public Gv help;
  public Gv hover;
  public Gv last_log;
  public Gv t_acc;
  public Gv muted;
  public Gv snd;
  public Gv hum;
  public Gv vign;
  public Gv pult;
  public Gv pult_sc;
  public Gv tiles;
  public Gv pult_on;
  static S_game() {
    Art = Gd.Script("art");
    YEL = Gd.Ctor("Color", Gv.S("f2c230"));
  }
  public S_game() {
    m = Gv.Null;
    quest = Gv.Null;
    q_step = Gv.Null;
    q_title = Gv.Null;
    q_hint = Gv.Null;
    q_detail = Gv.Null;
    q_bar = Gv.Null;
    q_btn = Gv.Null;
    q_body = Gv.Null;
    feed = Gv.Null;
    tip = Gv.Null;
    tip_lbl = Gv.Null;
    help = Gv.Null;
    hover = Gv.Null;
    last_log = Gv.F(-1.0);
    t_acc = Gv.F(0.0);
    muted = Gv.B(false);
    snd = Gd.Dict();
    hum = Gv.Null;
    vign = Gv.Null;
    pult = Gv.Null;
    pult_sc = Gv.Null;
    tiles = Gd.Arr();
    pult_on = Gv.B(true);
  }
  public Gv _ready() {
    Self.S("layer", Gv.I(2L));
    _quest();
    feed = Gd.New("VBoxContainer");
    feed.C("add_theme_constant_override", Gv.S("separation"), Gv.I(6L));
    feed = feed.S("anchor_left", Gv.F(0.5));
    feed = feed.S("anchor_right", Gv.F(0.5));
    feed = feed.S("offset_left", Gv.I(-260L));
    feed = feed.S("offset_right", Gv.I(260L));
    feed = feed.S("offset_top", Gv.I(62L));
    feed = feed.S("mouse_filter", Gd.StaticGet("Control", "MOUSE_FILTER_IGNORE"));
    Self.C("add_child", feed);
    _tip();
    _help();
    _sounds();
    hover = S_game_HoverLayer.New();
    hover = hover.S("g", Self);
    hover = hover.S("m", m);
    hover = hover.S("z_index", Gv.I(50L));
    m.C("add_child", hover);
    if ((Gd.Static("FileAccess", "file_exists", Gv.S("user://zvuk.txt"))).T()) {
      muted = Gv.B(Gv.Eq(Gd.Static("FileAccess", "open", Gv.S("user://zvuk.txt"), Gd.StaticGet("FileAccess", "READ")).C("get_as_text").C("strip_edges"), Gv.S("vyp")));
    }
    if ((m.G("sim").G("S").G("log").C("size")).T()) {
      last_log = m.G("sim").G("S").G("log").I(Gv.I(0L)).G("t");
    }
    Self.C("get_tree").C("create_timer", Gv.F(1.5)).G("timeout").C("connect", Gd.Fn((_a) => {  notify(Gv.S("Ovládání jako ve hře: F1 = přehled kláves, Mezerník = čas, 1–4 = rychlost, Enter = potvrdit úkol"), Gv.S("info")); return Gv.Null; }));
    return Gv.Null;
  }
  public Gv _vignette() {
    Gv cl = Gd.New("CanvasLayer");
    cl = cl.S("layer", Gv.I(0L));
    m.G("add_child").C("call_deferred", cl);
    vign = Gd.New("ColorRect");
    vign.C("set_anchors_preset", Gd.StaticGet("Control", "PRESET_FULL_RECT"));
    vign = vign.S("mouse_filter", Gd.StaticGet("Control", "MOUSE_FILTER_IGNORE"));
    Gv sh = Gd.New("Shader");
    sh = sh.S("code", Gv.S("shader_type canvas_item;\nvoid fragment(){ vec2 d = UV - vec2(0.5); d.x *= 1.25; float v = smoothstep(0.42, 0.9, length(d)); COLOR = vec4(0.0, 0.0, 0.0, v * 0.55); }"));
    Gv mat = Gd.New("ShaderMaterial");
    mat = mat.S("shader", sh);
    vign = vign.S("material", mat);
    cl.C("add_child", vign);
    return Gv.Null;
  }
  public Gv _card_sb(Gv accent = default(Gv)) {
    if (accent.k == 0) accent = YEL;
    Gv sb = Gd.New("StyleBoxFlat");
    sb = sb.S("bg_color", Gd.Ctor("Color", Gv.F(0.05), Gv.F(0.065), Gv.F(0.08), Gv.F(0.9)));
    sb.C("set_corner_radius_all", Gv.I(12L));
    sb = sb.S("border_color", Gd.Ctor("Color", accent, Gv.F(0.55)));
    sb.C("set_border_width_all", Gv.I(1L));
    sb = sb.S("border_width_left", Gv.I(4L));
    sb = sb.S("shadow_color", Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.45)));
    sb = sb.S("shadow_size", Gv.I(10L));
    sb = sb.S("shadow_offset", Gd.Ctor("Vector2", Gv.I(0L), Gv.I(4L)));
    sb = sb.S("content_margin_left", Gv.I(14L));
    sb = sb.S("content_margin_right", Gv.I(14L));
    sb = sb.S("content_margin_top", Gv.I(10L));
    sb = sb.S("content_margin_bottom", Gv.I(12L));
    return sb;
    return Gv.Null;
  }
  public Gv _lbl(Gv t = default(Gv), Gv size = default(Gv), Gv col = default(Gv), Gv bold = default(Gv)) {
    size = (size).AsI();
    if (bold.k == 0) bold = Gv.B(true);
    Gv l = Gd.New("Label");
    l = l.S("text", t);
    l.C("add_theme_font_override", Gv.S("font"), ((bold).T() ? m.G("font_b") : m.G("font")));
    l.C("add_theme_font_size_override", Gv.S("font_size"), size);
    l.C("add_theme_color_override", Gv.S("font_color"), col);
    return l;
    return Gv.Null;
  }
  public Gv _quest() {
    quest = Gd.New("PanelContainer");
    quest.C("add_theme_stylebox_override", Gv.S("panel"), _card_sb());
    quest = quest.S("position", Gd.Ctor("Vector2", Gv.I(12L), Gv.I(60L)));
    quest = quest.S("scale", Gd.Ctor("Vector2", Gv.F(0.76), Gv.F(0.76)));
    quest = quest.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(380L), Gv.I(0L)));
    Self.C("add_child", quest);
    Gv v = Gd.New("VBoxContainer");
    v.C("add_theme_constant_override", Gv.S("separation"), Gv.I(4L));
    quest.C("add_child", v);
    Gv top = Gd.New("HBoxContainer");
    v.C("add_child", top);
    Gv ic = _lbl(Gv.S("◆ ÚKOL"), Gv.I(12L), YEL);
    top.C("add_child", ic);
    q_step = _lbl(Gv.S(""), Gv.I(12L), Gd.Ctor("Color", Gv.S("8a939b")));
    q_step = q_step.S("size_flags_horizontal", Gd.StaticGet("Control", "SIZE_EXPAND_FILL"));
    q_step = q_step.S("horizontal_alignment", Gd.K("HORIZONTAL_ALIGNMENT_RIGHT"));
    top.C("add_child", q_step);
    Gv hide = Gd.New("Button");
    hide = hide.S("text", Gv.S("–"));
    hide = hide.S("flat", Gv.B(true));
    hide = hide.S("focus_mode", Gd.StaticGet("Control", "FOCUS_NONE"));
    hide.C("add_theme_color_override", Gv.S("font_color"), Gd.Ctor("Color", Gv.S("8a939b")));
    hide = hide.S("tooltip_text", Gv.S("Sbalit / rozbalit úkol (Q)"));
    hide.G("pressed").C("connect", Gd.Fn((_a) => toggle_quest()));
    top.C("add_child", hide);
    q_title = _lbl(Gv.S(""), Gv.I(17L), Gd.Ctor("Color", Gv.S("f5f7f9")));
    q_title = q_title.S("autowrap_mode", Gd.StaticGet("TextServer", "AUTOWRAP_WORD_SMART"));
    q_title = q_title.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(350L), Gv.I(0L)));
    v.C("add_child", q_title);
    q_body = Gd.New("VBoxContainer");
    q_body.C("add_theme_constant_override", Gv.S("separation"), Gv.I(6L));
    v.C("add_child", q_body);
    q_hint = _lbl(Gv.S(""), Gv.I(13L), Gd.Ctor("Color", Gv.S("c9d1d8")), Gv.B(false));
    q_hint = q_hint.S("autowrap_mode", Gd.StaticGet("TextServer", "AUTOWRAP_WORD_SMART"));
    q_hint = q_hint.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(350L), Gv.I(0L)));
    q_body.C("add_child", q_hint);
    q_detail = _lbl(Gv.S(""), Gv.I(12L), YEL, Gv.B(false));
    q_detail = q_detail.S("autowrap_mode", Gd.StaticGet("TextServer", "AUTOWRAP_WORD_SMART"));
    q_detail = q_detail.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(350L), Gv.I(0L)));
    q_body.C("add_child", q_detail);
    q_bar = Gd.New("ProgressBar");
    q_bar = q_bar.S("show_percentage", Gv.B(false));
    q_bar = q_bar.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(0L), Gv.I(6L)));
    Gv bg = Gd.New("StyleBoxFlat");
    bg = bg.S("bg_color", Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.08)));
    bg.C("set_corner_radius_all", Gv.I(3L));
    Gv fg = Gd.New("StyleBoxFlat");
    fg = fg.S("bg_color", YEL);
    fg.C("set_corner_radius_all", Gv.I(3L));
    q_bar.C("add_theme_stylebox_override", Gv.S("background"), bg);
    q_bar.C("add_theme_stylebox_override", Gv.S("fill"), fg);
    q_body.C("add_child", q_bar);
    q_btn = Gd.New("Button");
    q_btn = q_btn.S("text", Gv.S("✔  Potvrdit krok  (Enter)"));
    q_btn = q_btn.S("focus_mode", Gd.StaticGet("Control", "FOCUS_NONE"));
    Gv bn = Gd.New("StyleBoxFlat");
    bn = bn.S("bg_color", Gd.Ctor("Color", Gv.S("2f8f4e")));
    bn.C("set_corner_radius_all", Gv.I(8L));
    bn = bn.S("content_margin_top", Gv.I(6L));
    bn = bn.S("content_margin_bottom", Gv.I(6L));
    Gv bh = bn.C("duplicate");
    bh = bh.S("bg_color", Gd.Ctor("Color", Gv.S("39a85d")));
    Gv bd = bn.C("duplicate");
    bd = bd.S("bg_color", Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.06)));
    q_btn.C("add_theme_stylebox_override", Gv.S("normal"), bn);
    q_btn.C("add_theme_stylebox_override", Gv.S("hover"), bh);
    q_btn.C("add_theme_stylebox_override", Gv.S("pressed"), bh);
    q_btn.C("add_theme_stylebox_override", Gv.S("disabled"), bd);
    q_btn.C("add_theme_font_override", Gv.S("font"), m.G("font_b"));
    q_btn.C("add_theme_color_override", Gv.S("font_color"), Gd.StaticGet("Color", "WHITE"));
    q_btn.C("add_theme_color_override", Gv.S("font_disabled_color"), Gd.Ctor("Color", Gv.S("6b757f")));
    q_btn.G("pressed").C("connect", Gd.Fn((_a) => confirm()));
    q_body.C("add_child", q_btn);
    if (((Gd.Static("FileAccess", "file_exists", Gv.S("user://ukol.txt"))).T() && Gv.Eq(Gd.Static("FileAccess", "open", Gv.S("user://ukol.txt"), Gd.StaticGet("FileAccess", "READ")).C("get_as_text").C("strip_edges"), Gv.S("sbaleno")))) {
      q_body = q_body.S("visible", Gv.B(false));
    }
    return Gv.Null;
  }
  public Gv toggle_quest() {
    q_body = q_body.S("visible", Gv.B(!(q_body.G("visible")).T()));
    quest.C("reset_size");
    Gv f = Gd.Static("FileAccess", "open", Gv.S("user://ukol.txt"), Gd.StaticGet("FileAccess", "WRITE"));
    if (Gv.Ne(f, Gv.Null)) {
      f.C("store_string", ((q_body.G("visible")).T() ? Gv.S("rozbaleno") : Gv.S("sbaleno")));
    }
    play(Gv.S("click"));
    return Gv.Null;
  }
  public Gv confirm() {
    if ((m.G("sim").G("training").C("confirm")).T()) {
      play(Gv.S("ok"));
      notify(Gv.S("Krok splněn ✔"), Gv.S("good"));
    } else {
      play(Gv.S("deny"));
    }
    _refresh_quest();
    return Gv.Null;
  }
  public Gv _refresh_quest() {
    Gv tr = m.G("sim").G("training");
    Gv cur = tr.C("current");
    Gv idx = (tr.C("state").G("index")).AsI();
    Gv n = (tr.G("STEPS").C("size")).AsI();
    q_step = q_step.S("text", (Gv.S("krok %d / %d") % Gd.Arr(Gd.mini((idx + Gv.I(1L)), n), n)));
    q_title = q_title.S("text", cur.G("title"));
    q_hint = q_hint.S("text", cur.G("hint"));
    Gv det = Gd.Dyn("str", cur.C("get", Gv.S("detail"), Gv.S(""))).C("strip_edges");
    q_detail = q_detail.S("text", det);
    q_detail = q_detail.S("visible", Gv.B(Gv.Ne(det, Gv.S(""))));
    q_bar = q_bar.S("max_value", n);
    q_bar = q_bar.S("value", idx);
    q_btn = q_btn.S("disabled", Gv.B((!(cur.G("ready")).T() || (cur.C("get", Gv.S("complete"), Gv.B(false))).T())));
    q_btn = q_btn.S("text", ((cur.G("ready")).T() ? Gv.S("✔  Potvrdit krok  (Enter)") : Gv.S("… čeká na splnění podmínek")));
    Gv sb = quest.C("get_theme_stylebox", Gv.S("panel"));
    Gv acc = ((cur.G("ready")).T() ? Gd.Ctor("Color", Gv.S("4fd18b")) : YEL);
    if (Gv.Ne(sb.G("border_color"), Gd.Ctor("Color", acc, Gv.F(0.55)))) {
      sb = sb.S("border_color", Gd.Ctor("Color", acc, Gv.F(0.55)));
    }
    return Gv.Null;
  }
  public Gv notify(Gv t = default(Gv), Gv kind = default(Gv)) {
    if (kind.k == 0) kind = Gv.S("info");
    Gv col = Gd.Dict(Gv.S("bad"), Gd.Ctor("Color", Gv.S("ff5a4a")), Gv.S("warn"), YEL, Gv.S("good"), Gd.Ctor("Color", Gv.S("4fd18b"))).C("get", kind, Gd.Ctor("Color", Gv.S("5aa9ff")));
    Gv p = Gd.New("PanelContainer");
    p.C("add_theme_stylebox_override", Gv.S("panel"), _card_sb(col));
    p = p.S("mouse_filter", Gd.StaticGet("Control", "MOUSE_FILTER_IGNORE"));
    Gv h = Gd.New("HBoxContainer");
    h.C("add_theme_constant_override", Gv.S("separation"), Gv.I(10L));
    p.C("add_child", h);
    h.C("add_child", _lbl(Gd.Dict(Gv.S("bad"), Gv.S("⚠"), Gv.S("warn"), Gv.S("!"), Gv.S("good"), Gv.S("✔")).C("get", kind, Gv.S("ℹ")), Gv.I(18L), col));
    Gv l = _lbl(t, Gv.I(14L), Gd.Ctor("Color", Gv.S("eef2f5")));
    l = l.S("autowrap_mode", Gd.StaticGet("TextServer", "AUTOWRAP_WORD_SMART"));
    l = l.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(440L), Gv.I(0L)));
    h.C("add_child", l);
    feed.C("add_child", p);
    feed.C("move_child", p, Gv.I(0L));
    while (Gv.Gt(feed.C("get_child_count"), Gv.I(4L))) {
      feed.C("get_child", (feed.C("get_child_count") - Gv.I(1L))).C("queue_free");
      feed.C("remove_child", feed.C("get_child", (feed.C("get_child_count") - Gv.I(1L))));
    }
    p = p.S("modulate", p.G("modulate").S("a", Gv.F(0.0)));
    p = p.S("position", p.G("position").S("y", (p.G("position").G("y") - Gv.I(20L))));
    Gv tw = Gd.Dyn("create_tween");
    tw.C("tween_property", p, Gv.S("modulate:a"), Gv.F(1.0), Gv.F(0.25));
    tw.C("tween_interval", (Gv.F(5.0) + (t.C("length") / Gv.F(30.0))));
    tw.C("tween_property", p, Gv.S("modulate:a"), Gv.F(0.0), Gv.F(0.6));
    tw.C("tween_callback", p.G("queue_free"));
    return Gv.Null;
  }
  public Gv _tip() {
    tip = Gd.New("PanelContainer");
    Gv sb = _card_sb(Gd.Ctor("Color", Gv.S("5aa9ff")));
    sb = sb.S("content_margin_top", Gv.I(6L));
    sb = sb.S("content_margin_bottom", Gv.I(7L));
    sb = sb.S("content_margin_left", Gv.I(10L));
    sb = sb.S("content_margin_right", Gv.I(10L));
    tip.C("add_theme_stylebox_override", Gv.S("panel"), sb);
    tip = tip.S("mouse_filter", Gd.StaticGet("Control", "MOUSE_FILTER_IGNORE"));
    tip = tip.S("visible", Gv.B(false));
    tip_lbl = _lbl(Gv.S(""), Gv.I(13L), Gd.Ctor("Color", Gv.S("eef2f5")));
    tip.C("add_child", tip_lbl);
    Self.C("add_child", tip);
    return Gv.Null;
  }
  public Gv show_tip(Gv t = default(Gv), Gv sp = default(Gv)) {
    if (Gv.Eq(t, Gv.S(""))) {
      tip = tip.S("visible", Gv.B(false));
      return Gv.Null;
    }
    if (Gv.Ne(tip_lbl.G("text"), t)) {
      tip_lbl = tip_lbl.S("text", t);
      tip.C("reset_size");
    }
    tip = tip.S("visible", Gv.B(true));
    Gv vs = Self.C("get_viewport").C("get_visible_rect").G("size");
    tip = tip.S("position", Gd.Ctor("Vector2", Gd.minf((sp.G("x") + Gv.I(18L)), ((vs.G("x") - tip.G("size").G("x")) - Gv.I(8L))), Gd.minf((sp.G("y") + Gv.I(20L)), ((vs.G("y") - tip.G("size").G("y")) - Gv.I(8L)))));
    return Gv.Null;
  }
  public Gv _help() {
    help = Gd.New("PanelContainer");
    help.C("add_theme_stylebox_override", Gv.S("panel"), _card_sb());
    help = help.S("visible", Gv.B(false));
    help = help.S("anchor_left", Gv.F(0.5));
    help = help.S("anchor_top", Gv.F(0.5));
    help = help.S("anchor_right", Gv.F(0.5));
    help = help.S("anchor_bottom", Gv.F(0.5));
    help = help.S("offset_left", Gv.I(-300L));
    help = help.S("offset_top", Gv.I(-340L));
    help = help.S("offset_right", Gv.I(300L));
    help = help.S("offset_bottom", Gv.I(340L));
    Self.C("add_child", help);
    Gv v = Gd.New("VBoxContainer");
    v.C("add_theme_constant_override", Gv.S("separation"), Gv.I(6L));
    help.C("add_child", v);
    v.C("add_child", _lbl(Gv.S("OVLÁDÁNÍ"), Gv.I(18L), YEL));
    Gv rows = Gd.Arr(Gd.Arr(Gv.S("Mezerník"), Gv.S("spustit / pozastavit čas")), Gd.Arr(Gv.S("1  2  3  4"), Gv.S("rychlost x1 / x10 / x60 / x180")), Gd.Arr(Gv.S("Enter"), Gv.S("potvrdit krok úkolu")), Gd.Arr(Gv.S("Q"), Gv.S("sbalit / rozbalit úkol")), Gd.Arr(Gv.S("Alt nad klapkou"), Gv.S("ukáže, kam z ní teče")), Gd.Arr(Gv.S("Ctrl + Z"), Gv.S("vrátit poslední klapku / čerpadlo")), Gd.Arr(Gv.S("Tab"), Gv.S("ovládací panel")), Gd.Arr(Gv.S("R"), Gv.S("rozvaděč vpravo")), Gd.Arr(Gv.S("FM na rozvaděči"), Gv.S("táhni dokola nebo kolečko")), Gd.Arr(Gv.S("Z"), Gv.S("zvětšit rozvaděč (kolečko nad ním přiblíží)")), Gd.Arr(Gv.S("M"), Gv.S("mapka")), Gd.Arr(Gv.S("E"), Gv.S("editor plánku: přidat, upravit, smazat díly (Ctrl+S uloží)")), Gd.Arr(Gv.S("W A S D / šipky"), Gv.S("posun pohledu (Shift rychleji)")), Gd.Arr(Gv.S("kolečko"), Gv.S("přiblížit / oddálit")), Gd.Arr(Gv.S("táhnout myší"), Gv.S("posun pohledu")), Gd.Arr(Gv.S("klik"), Gv.S("klapka, čerpadlo, spoušť, vývod…")), Gd.Arr(Gv.S("Ctrl + kolečko"), Gv.S("klapka v mezipoloze")), Gd.Arr(Gv.S("Home"), Gv.S("celá varna")), Gd.Arr(Gv.S("F5 … F9"), Gv.S("pánev, SK, vířivka, bojler, spilka")), Gd.Arr(Gv.S("N"), Gv.S("zvuk zapnout / vypnout")), Gd.Arr(Gv.S("F1 / H"), Gv.S("tato nápověda")));
    Gv gr = Gd.New("GridContainer");
    gr = gr.S("columns", Gv.I(2L));
    gr.C("add_theme_constant_override", Gv.S("h_separation"), Gv.I(18L));
    gr.C("add_theme_constant_override", Gv.S("v_separation"), Gv.I(4L));
    v.C("add_child", gr);
    foreach (Gv r in (rows).Iter()) {
      gr.C("add_child", _lbl(r.I(Gv.I(0L)), Gv.I(13L), Gd.Ctor("Color", Gv.S("ffd95a"))));
      gr.C("add_child", _lbl(r.I(Gv.I(1L)), Gv.I(13L), Gd.Ctor("Color", Gv.S("d5dbe0")), Gv.B(false)));
    }
    v.C("add_child", _lbl(Gv.S("PROVOZNÍ ZÁSADY"), Gv.I(16L), YEL));
    Gv gr2 = Gd.New("GridContainer");
    gr2 = gr2.S("columns", Gv.I(2L));
    gr2.C("add_theme_constant_override", Gv.S("h_separation"), Gv.I(18L));
    gr2.C("add_theme_constant_override", Gv.S("v_separation"), Gv.I(4L));
    v.C("add_child", gr2);
    foreach (Gv r2 in (Gd.Script("practice").G("RULES")).Iter()) {
      gr2.C("add_child", _lbl(r2.I(Gv.I(0L)), Gv.I(12L), Gd.Ctor("Color", Gv.S("ffd95a"))));
      Gv l2 = _lbl(r2.I(Gv.I(1L)), Gv.I(12L), Gd.Ctor("Color", Gv.S("d5dbe0")), Gv.B(false));
      l2 = l2.S("autowrap_mode", Gd.StaticGet("TextServer", "AUTOWRAP_WORD_SMART"));
      l2 = l2.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(380L), Gv.I(0L)));
      gr2.C("add_child", l2);
    }
    v.C("add_child", _lbl(Gv.S("Esc zavře"), Gv.I(11L), Gd.Ctor("Color", Gv.S("6b757f")), Gv.B(false)));
    return Gv.Null;
  }
  public Gv toggle_help() {
    help = help.S("visible", Gv.B(!(help.G("visible")).T()));
    play(Gv.S("click"));
    return Gv.Null;
  }
  public Gv _wav(Gv samples = default(Gv), Gv rate = default(Gv), Gv loop = default(Gv)) {
    if (rate.k == 0) rate = Gv.I(22050L);
    if (loop.k == 0) loop = Gv.B(false);
    Gv data = Gd.Ctor("PackedByteArray");
    data.C("resize", (samples.C("size") * Gv.I(2L)));
    foreach (Gv i in (samples.C("size")).Iter()) {
      data.C("encode_s16", (i * Gv.I(2L)), Gd.Dyn("int", (Gd.clampf(samples.I(i), Gv.F(-1.0), Gv.F(1.0)) * Gv.F(30000.0))));
    }
    Gv w = Gd.New("AudioStreamWAV");
    w = w.S("format", Gd.StaticGet("AudioStreamWAV", "FORMAT_16_BITS"));
    w = w.S("mix_rate", rate);
    w = w.S("data", data);
    if ((loop).T()) {
      w = w.S("loop_mode", Gd.StaticGet("AudioStreamWAV", "LOOP_FORWARD"));
      w = w.S("loop_end", samples.C("size"));
    }
    return w;
    return Gv.Null;
  }
  public Gv _tone(Gv f0 = default(Gv), Gv f1 = default(Gv), Gv dur = default(Gv), Gv vol = default(Gv), Gv noise = default(Gv), Gv shape = default(Gv)) {
    f0 = (f0).AsF();
    f1 = (f1).AsF();
    dur = (dur).AsF();
    vol = (vol).AsF();
    if (noise.k == 0) noise = Gv.F(0.0);
    if (shape.k == 0) shape = Gv.F(1.0);
    Gv n = Gd.Dyn("int", (Gv.I(22050L) * dur));
    Gv o = Gd.Ctor("PackedFloat32Array");
    o.C("resize", n);
    Gv ph = Gv.F(0.0);
    foreach (Gv i in (n).Iter()) {
      Gv t = (Gd.Dyn("float", i) / n);
      ph = (ph + ((Gd.K("TAU") * Gd.lerpf(f0, f1, t)) / Gv.F(22050.0)));
      Gv env = (Gd.pow((Gv.F(1.0) - t), shape) * Gd.minf(Gv.F(1.0), (i / Gv.F(120.0))));
      o = o.SI(i, ((((Gd.sin(ph) + (noise * Gd.randf_range(Gv.F(-1.0), Gv.F(1.0))))) * env) * vol));
    }
    return o;
    return Gv.Null;
  }
  public Gv _sounds() {
    Gv defs = Gd.Dict(Gv.S("click"), _tone(Gv.I(1800L), Gv.I(1200L), Gv.F(0.035), Gv.F(0.25), Gv.F(0.1), Gv.F(2.0)), Gv.S("valve"), _tone(Gv.I(140L), Gv.I(70L), Gv.F(0.16), Gv.F(0.6), Gv.F(0.35), Gv.F(1.5)), Gv.S("start"), _tone(Gv.I(120L), Gv.I(360L), Gv.F(0.45), Gv.F(0.35), Gv.F(0.05), Gv.F(0.6)), Gv.S("stop"), _tone(Gv.I(320L), Gv.I(90L), Gv.F(0.4), Gv.F(0.3), Gv.F(0.05), Gv.F(0.8)), Gv.S("ok"), _tone(Gv.I(660L), Gv.I(990L), Gv.F(0.18), Gv.F(0.3), Gv.F(0.0), Gv.F(1.0)), Gv.S("deny"), _tone(Gv.I(220L), Gv.I(180L), Gv.F(0.15), Gv.F(0.3), Gv.F(0.1), Gv.F(1.0)));
    Gv al = _tone(Gv.I(880L), Gv.I(880L), Gv.F(0.12), Gv.F(0.3), Gv.F(0.0), Gv.F(0.2));
    al.C("append_array", Gd.Ctor("PackedFloat32Array", Gd.Arr(Gv.F(0.0))).C("duplicate"));
    Gv gap = Gd.Ctor("PackedFloat32Array");
    gap.C("resize", Gv.I(1500L));
    al.C("append_array", gap);
    al.C("append_array", _tone(Gv.I(660L), Gv.I(660L), Gv.F(0.14), Gv.F(0.3), Gv.F(0.0), Gv.F(0.2)));
    defs = defs.SI(Gv.S("alarm"), al);
    foreach (Gv k in (defs).Iter()) {
      Gv p = Gd.New("AudioStreamPlayer");
      p = p.S("stream", _wav(defs.I(k)));
      Self.C("add_child", p);
      snd = snd.SI(k, p);
    }
    Gv hs = Gd.Ctor("PackedFloat32Array");
    hs.C("resize", Gv.I(22050L));
    foreach (Gv i in (Gv.I(22050L)).Iter()) {
      Gv t = (i / Gv.F(22050.0));
      hs = hs.SI(i, ((((Gv.F(0.35) * Gd.sin(((Gd.K("TAU") * Gv.F(100.0)) * t))) + (Gv.F(0.2) * Gd.sin(((Gd.K("TAU") * Gv.F(50.0)) * t)))) + (Gv.F(0.12) * Gd.sin(((Gd.K("TAU") * Gv.F(300.0)) * t)))) + (Gv.F(0.05) * Gd.randf_range(Gv.F(-1.0), Gv.F(1.0)))));
    }
    hum = Gd.New("AudioStreamPlayer");
    hum = hum.S("stream", _wav(hs, Gv.I(22050L), Gv.B(true)));
    hum = hum.S("volume_db", Gv.F(-80.0));
    Self.C("add_child", hum);
    return Gv.Null;
  }
  public Gv play(Gv k = default(Gv)) {
    if (((muted).T() || !(snd.C("has", k)).T())) {
      return Gv.Null;
    }
    snd.I(k).C("play");
    return Gv.Null;
  }
  public Gv toggle_mute() {
    muted = Gv.B(!(muted).T());
    Gv f = Gd.Static("FileAccess", "open", Gv.S("user://zvuk.txt"), Gd.StaticGet("FileAccess", "WRITE"));
    if (Gv.Ne(f, Gv.Null)) {
      f.C("store_string", ((muted).T() ? Gv.S("vyp") : Gv.S("zap")));
    }
    m.G("hud").C("toast", (Gv.S("Zvuk: ") + (((muted).T() ? Gv.S("vypnut (N)") : Gv.S("zapnut (N)")))));
    return Gv.Null;
  }
  public Gv _process(Gv delta = default(Gv)) {
    delta = (delta).AsF();
    t_acc = (t_acc + delta);
    if (Gv.Gt(t_acc, Gv.F(0.25))) {
      t_acc = Gv.F(0.0);
      _refresh_quest();
      _feed_log();
      _refresh_pult();
    }
    Gv S = m.G("sim").G("S");
    Gv npumps = Gv.I(0L);
    foreach (Gv p in (Gd.Arr(Gv.S("p1"), Gv.S("p2"), Gv.S("p3"))).Iter()) {
      if (((S.I(p).G("on")).T() && Gv.Gt(S.I(p).G("q"), Gv.F(1.0)))) {
        npumps = (npumps + Gv.I(1L));
      }
    }
    if ((S.G("lv").G("pump")).T()) {
      npumps = (npumps + Gv.I(1L));
    }
    if (((S.G("rvpWater").G("on")).T() || (S.G("skWater").G("on")).T())) {
      npumps = (npumps + Gv.I(1L));
    }
    Gv target = ((((muted).T() || Gv.Eq(npumps, Gv.I(0L))) || !(m.G("running")).T()) ? Gv.F(-80.0) : ((Gv.F(-26.0) + (Gv.F(2.5) * npumps))));
    hum = hum.S("volume_db", Gd.lerpf(hum.G("volume_db"), target, Gd.minf(Gv.F(1.0), (delta * Gv.F(3.0)))));
    if ((Gv.Gt(hum.G("volume_db"), Gv.F(-60.0)) && !(hum.G("playing")).T())) {
      hum.C("play");
    } else if ((Gv.Le(hum.G("volume_db"), Gv.F(-78.0)) && (hum.G("playing")).T())) {
      hum.C("stop");
    }
    return Gv.Null;
  }
  public Gv _feed_log() {
    Gv L = m.G("sim").G("S").G("log");
    if ((L.C("is_empty")).T()) {
      return Gv.Null;
    }
    if ((Gv.Eq(L.I(Gv.I(0L)).G("t"), last_log) && Gv.Gt(L.C("size"), Gv.I(0L)))) {
      return Gv.Null;
    }
    Gv news = Gd.Arr();
    foreach (Gv l in (L).Iter()) {
      if (Gv.Le(l.G("t"), last_log)) {
        break;
      }
      news.C("append", l);
      if (Gv.Ge(news.C("size"), Gv.I(3L))) {
        break;
      }
    }
    last_log = L.I(Gv.I(0L)).G("t");
    news.C("reverse");
    foreach (Gv l_2 in (news).Iter()) {
      if (Gv.Eq(l_2.G("type"), Gv.S("bad"))) {
        notify(l_2.G("msg"), Gv.S("bad"));
        play(Gv.S("alarm"));
      } else if (Gv.Eq(l_2.G("type"), Gv.S("warn"))) {
        notify(l_2.G("msg"), Gv.S("warn"));
      }
    }
    return Gv.Null;
  }
  public Gv _pult() {
    pult = Gd.New("PanelContainer");
    Gv sb = Gd.New("StyleBoxFlat");
    sb = sb.S("bg_color", Gd.Ctor("Color", Gv.F(0.04), Gv.F(0.05), Gv.F(0.06), Gv.F(0.88)));
    sb.C("set_corner_radius_all", Gv.I(12L));
    sb = sb.S("border_color", Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.08)));
    sb.C("set_border_width_all", Gv.I(1L));
    sb = sb.S("content_margin_left", Gv.I(8L));
    sb = sb.S("content_margin_right", Gv.I(8L));
    sb = sb.S("content_margin_top", Gv.I(6L));
    sb = sb.S("content_margin_bottom", Gv.I(6L));
    sb = sb.S("shadow_color", Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.4)));
    sb = sb.S("shadow_size", Gv.I(10L));
    pult.C("add_theme_stylebox_override", Gv.S("panel"), sb);
    pult = pult.S("anchor_top", Gv.F(1.0));
    pult = pult.S("anchor_bottom", Gv.F(1.0));
    pult = pult.S("offset_bottom", Gv.I(-12L));
    pult = pult.S("offset_top", (Gv.I(-12L) - Gv.I(84L)));
    Self.C("add_child", pult);
    pult_sc = Gd.New("ScrollContainer");
    pult_sc = pult_sc.S("vertical_scroll_mode", Gd.StaticGet("ScrollContainer", "SCROLL_MODE_DISABLED"));
    pult.C("add_child", pult_sc);
    Gv h = Gd.New("HBoxContainer");
    h.C("add_theme_constant_override", Gv.S("separation"), Gv.I(6L));
    pult_sc.C("add_child", h);
    Gv S = m.G("sim").G("S");
    _tile(h, Gv.S("VODA → PÁNEV"), Gd.Fn((_a) => {  (Gv.Ne(m.G("ctl"), Gv.Null) ? m.G("ctl").C("_water") : Gv.Null); return Gv.Null; }), Gd.Fn((_a) => { Gv d = Gd.A(_a, 0); S = S.S("rvpWater", S.G("rvpWater").S("target", Gd.maxf(S.G("rvpWater").G("added"), (S.G("rvpWater").G("target") + (Gv.F(100.0) * d))))); return Gv.Null; }), Gd.Fn((_a) => {  return Gd.Arr(((Gv.S("%d/%d L") % Gd.Arr(Gd.roundi(S.G("rvpWater").G("added")), Gd.roundi(S.G("rvpWater").G("target"))))), S.G("rvpWater").G("on")); return Gv.Null; }));
    _tile(h, Gv.S("ŠROT"), Gd.Fn((_a) => {  (Gv.Ne(m.G("ctl"), Gv.Null) ? m.G("ctl").C("_malt") : Gv.Null); return Gv.Null; }), Gv.Null, Gd.Fn((_a) => {  return Gd.Arr(((Gv.S("%d/%d kg") % Gd.Arr(Gd.roundi(S.G("malt").G("added")), Gd.roundi(S.G("malt").G("target"))))), S.G("malt").G("on")); return Gv.Null; }));
    _tile(h, Gv.S("PÁRA PÁNEV"), Gd.Fn((_a) => {  S = S.S("rvp", S.G("rvp").S("heat", (Gv.Ge(S.G("rvp").G("heat"), Gv.F(0.99)) ? Gv.F(0.0) : Gd.snappedf((S.G("rvp").G("heat") + Gv.F(0.25)), Gv.F(0.25))))); return Gv.Null; }), Gd.Fn((_a) => { Gv d_2 = Gd.A(_a, 0); S = S.S("rvp", S.G("rvp").S("heat", Gd.clampf(Gd.snappedf((S.G("rvp").G("heat") + (Gv.F(0.25) * d_2)), Gv.F(0.25)), Gv.F(0.0), Gv.F(1.0)))); return Gv.Null; }), Gd.Fn((_a) => {  return Gd.Arr(((Gv.S("%d %%") % Gd.roundi((S.G("rvp").G("heat") * Gv.F(100.0))))), Gv.B(Gv.Gt(S.G("rvp").G("heat"), Gv.F(0.0)))); return Gv.Null; }));
    _tile(h, Gv.S("MÍCHADLO"), Gd.Fn((_a) => {  S = S.S("rvp", S.G("rvp").S("stir", Gv.B(!(S.G("rvp").G("stir")).T()))); return Gv.Null; }), Gd.Fn((_a) => { Gv d_3 = Gd.A(_a, 0); S = S.S("rvp", S.G("rvp").S("hz", Gd.clampf((S.G("rvp").G("hz") + (Gv.F(2.5) * d_3)), Gv.F(10.0), Gv.F(50.0)))); return Gv.Null; }), Gd.Fn((_a) => {  return Gd.Arr(((S.G("rvp").G("stir")).T() ? ((Gv.S("%d Hz") % Gd.roundi(S.G("rvp").G("hz")))) : Gv.S("stojí")), S.G("rvp").G("stir")); return Gv.Null; }));
    _tile(h, Gv.S("KOPAČKA SK"), Gd.Fn((_a) => {  S = S.S("sk", S.G("sk").S("rake", Gv.B(!(S.G("sk").G("rake")).T()))); return Gv.Null; }), Gd.Fn((_a) => { Gv d_4 = Gd.A(_a, 0); S = S.S("sk", S.G("sk").S("rakeH", Gd.clampf((S.G("sk").G("rakeH") + (Gv.F(0.1) * d_4)), Gv.F(0.0), Gv.F(1.0)))); return Gv.Null; }), Gd.Fn((_a) => {  return Gd.Arr(((S.G("sk").G("rake")).T() ? ((Gv.S("hloubka %d %%") % Gd.roundi((S.G("sk").G("rakeH") * Gv.F(100.0))))) : Gv.S("stojí")), S.G("sk").G("rake")); return Gv.Null; }));
    _tile(h, Gv.S("VYSLAZOVÁNÍ"), Gd.Fn((_a) => {  S = S.S("skWater", S.G("skWater").S("on", Gv.B(!(S.G("skWater").G("on")).T()))); return Gv.Null; }), Gd.Fn((_a) => { Gv d_5 = Gd.A(_a, 0); S = S.S("skWater", S.G("skWater").S("flow", Gd.clampf((S.G("skWater").G("flow") + (Gv.F(10.0) * d_5)), Gv.F(10.0), Gv.F(200.0)))); return Gv.Null; }), Gd.Fn((_a) => {  return Gd.Arr(((Gv.S("%d L/min") % Gd.roundi(S.G("skWater").G("flow")))), S.G("skWater").G("on")); return Gv.Null; }));
    foreach (Gv p in (Gd.Arr(Gd.Arr(Gv.S("p1"), Gv.F(25.0), Gv.F(50.0), Gv.F(400.0)), Gd.Arr(Gv.S("p2"), Gv.F(10.0), Gv.F(10.0), Gv.F(200.0)), Gd.Arr(Gv.S("p3"), Gv.F(10.0), Gv.F(20.0), Gv.F(150.0)))).Iter()) {
      Gv key = p.I(Gv.I(0L));
      Gv stp = (p.I(Gv.I(1L))).AsF();
      Gv lo = (p.I(Gv.I(2L))).AsF();
      Gv hi = (p.I(Gv.I(3L))).AsF();
      _tile(h, m.C("pump_name", key), Gd.Fn((_a) => {  m.C("toggle_pump", key); return Gv.Null; }), Gd.Fn((_a) => { Gv d_6 = Gd.A(_a, 0); S = S.SI(key, S.I(key).S("flow", Gd.clampf((S.I(key).G("flow") + (stp * d_6)), lo, hi))); return Gv.Null; }), Gd.Fn((_a) => {  return Gd.Arr(((Gv.S("%d L/min") % Gd.roundi(((S.I(key).G("on")).T() ? S.I(key).G("q") : S.I(key).G("flow"))))), S.I(key).G("on")); return Gv.Null; }));
    }
    _tile(h, (Gv.S("LEDOVKA ") + m.C("pump_name", Gv.S("plv"))), Gd.Fn((_a) => {  m.C("toggle_pump", Gv.S("plv")); return Gv.Null; }), Gv.Null, Gd.Fn((_a) => {  return Gd.Arr(((Gv.S("%s °C") % m.C("c1", S.G("lv").G("T")))), S.G("lv").G("pump")); return Gv.Null; }));
    _tile(h, Gv.S("PÁRA Z KOTLE"), Gd.Fn((_a) => {  S = S.S("boiler", S.G("boiler").S("on", Gv.B(!(S.G("boiler").G("on")).T()))); return Gv.Null; }), Gv.Null, Gd.Fn((_a) => {  return Gd.Arr(((S.G("boiler").G("on")).T() ? ((Gv.S("%d %%") % Gd.roundi((S.G("boiler").G("load") * Gv.F(100.0))))) : Gv.S("vyp")), S.G("boiler").G("on")); return Gv.Null; }));
    return Gv.Null;
  }
  public Gv _tile(Gv parent = default(Gv), Gv title = default(Gv), Gv click = default(Gv), Gv wheel = default(Gv), Gv val = default(Gv)) {
    Gv b = Gd.New("Button");
    b = b.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(112L), Gv.I(70L)));
    b = b.S("focus_mode", Gd.StaticGet("Control", "FOCUS_NONE"));
    b = b.S("toggle_mode", Gv.B(false));
    b = b.S("clip_text", Gv.B(true));
    Gv n = Gd.New("StyleBoxFlat");
    n = n.S("bg_color", Gd.Ctor("Color", Gv.S("161c22")));
    n.C("set_corner_radius_all", Gv.I(9L));
    n = n.S("border_color", Gd.Ctor("Color", Gv.S("2a333c")));
    n.C("set_border_width_all", Gv.I(1L));
    Gv hv = n.C("duplicate");
    hv = hv.S("border_color", YEL);
    hv = hv.S("bg_color", Gd.Ctor("Color", Gv.S("1d252d")));
    b.C("add_theme_stylebox_override", Gv.S("normal"), n);
    b.C("add_theme_stylebox_override", Gv.S("hover"), hv);
    b.C("add_theme_stylebox_override", Gv.S("pressed"), hv);
    b = b.S("tooltip_text", ((title + Gv.S("\nklik = zapnout / vypnout")) + ((Gv.Ne(wheel, Gv.Null) ? Gv.S("\nkolečko = nastavit hodnotu") : Gv.S("")))));
    Gv v = Gd.New("VBoxContainer");
    v.C("set_anchors_preset", Gd.StaticGet("Control", "PRESET_FULL_RECT"));
    v = v.S("offset_left", Gv.I(8L));
    v = v.S("offset_top", Gv.I(6L));
    v = v.S("offset_right", Gv.I(-8L));
    v = v.S("offset_bottom", Gv.I(-6L));
    v = v.S("mouse_filter", Gd.StaticGet("Control", "MOUSE_FILTER_IGNORE"));
    v.C("add_theme_constant_override", Gv.S("separation"), Gv.I(2L));
    b.C("add_child", v);
    Gv t = _lbl(title, Gv.I(10L), Gd.Ctor("Color", Gv.S("8a939b")));
    t = t.S("mouse_filter", Gd.StaticGet("Control", "MOUSE_FILTER_IGNORE"));
    t = t.S("clip_text", Gv.B(true));
    v.C("add_child", t);
    Gv vl = _lbl(Gv.S(""), Gv.I(15L), Gd.Ctor("Color", Gv.S("eef2f5")));
    vl = vl.S("mouse_filter", Gd.StaticGet("Control", "MOUSE_FILTER_IGNORE"));
    vl = vl.S("clip_text", Gv.B(true));
    v.C("add_child", vl);
    Gv led = Gd.New("ColorRect");
    led = led.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(0L), Gv.I(4L)));
    led = led.S("mouse_filter", Gd.StaticGet("Control", "MOUSE_FILTER_IGNORE"));
    v.C("add_child", led);
    b.G("pressed").C("connect", Gd.Fn((_a) => {  click.C("call"); play(Gv.S("click")); m.C("redraw"); _refresh_pult(); return Gv.Null; }));
    if (Gv.Ne(wheel, Gv.Null)) {
      b.G("gui_input").C("connect", Gd.Fn((_a) => { Gv e = Gd.A(_a, 0); if (((Gv.Is(e, "InputEventMouseButton") && (e.G("pressed")).T()) && Gv.In(e.G("button_index"), Gd.Arr(Gd.K("MOUSE_BUTTON_WHEEL_UP"), Gd.K("MOUSE_BUTTON_WHEEL_DOWN"))))) {   wheel.C("call", (Gv.Eq(e.G("button_index"), Gd.K("MOUSE_BUTTON_WHEEL_UP")) ? Gv.I(1L) : Gv.I(-1L)));   _refresh_pult();   b.C("accept_event"); } return Gv.Null; }));
    }
    parent.C("add_child", b);
    tiles.C("append", Gd.Dict(Gv.S("b"), b, Gv.S("v"), vl, Gv.S("led"), led, Gv.S("val"), val, Gv.S("n"), n));
    return Gv.Null;
  }
  public Gv _refresh_pult() {
    if (Gv.Eq(pult, Gv.Null)) {
      return Gv.Null;
    }
    Gv left = Gv.F(12.0);
    pult = pult.S("offset_left", left);
    pult = pult.S("offset_right", ((-m.C("right_w")) - Gv.F(12.0)));
    pult = pult.S("visible", pult_on);
    if (Gv.Ne(m.G("mini"), Gv.Null)) {
      Gv mh = (m.G("mini").G("size").G("y")).AsF();
      Gv base_ = (Gv.F(-14.0) - (((pult_on).T() ? Gv.F(96.0) : Gv.F(0.0))));
      if (Gv.Ne(m.G("mini").G("offset_bottom"), base_)) {
        m = m.S("mini", m.G("mini").S("offset_bottom", base_));
        m = m.S("mini", m.G("mini").S("offset_top", (base_ - mh)));
      }
    }
    foreach (Gv t__it in (tiles).Iter()) {
      Gv t = t__it;
      Gv r = t.G("val").C("call");
      if (Gv.Ne(t.G("v").G("text"), r.I(Gv.I(0L)))) {
        t = t.S("v", t.G("v").S("text", r.I(Gv.I(0L))));
      }
      Gv on = r.I(Gv.I(1L));
      t = t.S("led", t.G("led").S("color", ((on).T() ? Gd.Ctor("Color", Gv.S("4fd18b")) : Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.08)))));
      t.G("v").C("add_theme_color_override", Gv.S("font_color"), ((on).T() ? Gd.Ctor("Color", Gv.S("ffffff")) : Gd.Ctor("Color", Gv.S("aab3bb"))));
    }
    return Gv.Null;
  }
  public Gv toggle_pult() {
    pult_on = Gv.B(!(pult_on).T());
    _refresh_pult();
    play(Gv.S("click"));
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "Art": return S_game.Art;
    case "YEL": return S_game.YEL;
    case "m": return this.m;
    case "quest": return this.quest;
    case "q_step": return this.q_step;
    case "q_title": return this.q_title;
    case "q_hint": return this.q_hint;
    case "q_detail": return this.q_detail;
    case "q_bar": return this.q_bar;
    case "q_btn": return this.q_btn;
    case "q_body": return this.q_body;
    case "feed": return this.feed;
    case "tip": return this.tip;
    case "tip_lbl": return this.tip_lbl;
    case "help": return this.help;
    case "hover": return this.hover;
    case "last_log": return this.last_log;
    case "t_acc": return this.t_acc;
    case "muted": return this.muted;
    case "snd": return this.snd;
    case "hum": return this.hum;
    case "vign": return this.vign;
    case "pult": return this.pult;
    case "pult_sc": return this.pult_sc;
    case "tiles": return this.tiles;
    case "pult_on": return this.pult_on;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "m": this.m = v; return v;
    case "quest": this.quest = v; return v;
    case "q_step": this.q_step = v; return v;
    case "q_title": this.q_title = v; return v;
    case "q_hint": this.q_hint = v; return v;
    case "q_detail": this.q_detail = v; return v;
    case "q_bar": this.q_bar = v; return v;
    case "q_btn": this.q_btn = v; return v;
    case "q_body": this.q_body = v; return v;
    case "feed": this.feed = v; return v;
    case "tip": this.tip = v; return v;
    case "tip_lbl": this.tip_lbl = v; return v;
    case "help": this.help = v; return v;
    case "hover": this.hover = v; return v;
    case "last_log": this.last_log = v; return v;
    case "t_acc": this.t_acc = v; return v;
    case "muted": this.muted = v; return v;
    case "snd": this.snd = v; return v;
    case "hum": this.hum = v; return v;
    case "vign": this.vign = v; return v;
    case "pult": this.pult = v; return v;
    case "pult_sc": this.pult_sc = v; return v;
    case "tiles": this.tiles = v; return v;
    case "pult_on": this.pult_on = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "_ready": return _ready();
    case "_vignette": return _vignette();
    case "_card_sb": return _card_sb(Gd.A(a, 0));
    case "_lbl": return _lbl(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "_quest": return _quest();
    case "toggle_quest": return toggle_quest();
    case "confirm": return confirm();
    case "_refresh_quest": return _refresh_quest();
    case "notify": return notify(Gd.A(a, 0), Gd.A(a, 1));
    case "_tip": return _tip();
    case "show_tip": return show_tip(Gd.A(a, 0), Gd.A(a, 1));
    case "_help": return _help();
    case "toggle_help": return toggle_help();
    case "_wav": return _wav(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "_tone": return _tone(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5));
    case "_sounds": return _sounds();
    case "play": return play(Gd.A(a, 0));
    case "toggle_mute": return toggle_mute();
    case "_process": return _process(Gd.A(a, 0));
    case "_feed_log": return _feed_log();
    case "_pult": return _pult();
    case "_tile": return _tile(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4));
    case "_refresh_pult": return _refresh_pult();
    case "toggle_pult": return toggle_pult();
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    case "Art": return Art;
    case "YEL": return YEL;
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_game(); return Gv.O(o); }
  public override string ScriptName { get { return "game"; } }
}
}
