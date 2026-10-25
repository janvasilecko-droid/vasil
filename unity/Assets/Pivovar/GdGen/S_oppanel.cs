// Vygenerováno z oppanel.gd překladačem gd2cs.py – needituj ručně.
using System; using System.Collections.Generic;
namespace Pivovar.Gd.Gen {
public partial class S_oppanel_Knob : GdNode {
  public Gv font;
  public Gv get_v;
  public Gv set_v;
  public Gv on_click;
  public Gv drag;
  static S_oppanel_Knob() {
  }
  public S_oppanel_Knob() {
    TypeName = "Control";
    font = Gv.Null;
    get_v = Gv.Null;
    set_v = Gv.Null;
    on_click = Gv.Null;
    drag = Gv.B(false);
  }
  public Gv _ready() {
    Self.S("mouse_filter", Gd.StaticGet("Control", "MOUSE_FILTER_STOP"));
    Self.S("mouse_default_cursor_shape", Gd.StaticGet("Control", "CURSOR_POINTING_HAND"));
    return Gv.Null;
  }
  public Gv _val_from(Gv p = default(Gv)) {
    Gv d = (p - (Self.G("size") * Gv.F(0.5)));
    Gv a = Gd.rad_to_deg(Gd.atan2(d.G("x"), (-d.G("y"))));
    return (Gd.clampf((Gv.F(1.0) + ((Gv.F(9.0) * ((Gd.clampf(a, Gv.F(-135.0), Gv.F(135.0)) + Gv.F(135.0)))) / Gv.F(270.0))), Gv.F(1.0), Gv.F(10.0))).AsF();
    return Gv.Null;
  }
  public Gv _gui_input(Gv e = default(Gv)) {
    if (Gv.Is(e, "InputEventMouseButton")) {
      if (Gv.Eq(e.G("button_index"), Gd.K("MOUSE_BUTTON_LEFT"))) {
        drag = e.G("pressed");
        if ((e.G("pressed")).T()) {
          set_v.C("call", Gd.snappedf(_val_from(e.G("position")), Gv.F(0.1)));
          on_click.C("call");
        }
        Self.C("accept_event");
      } else if (((e.G("pressed")).T() && Gv.In(e.G("button_index"), Gd.Arr(Gd.K("MOUSE_BUTTON_WHEEL_UP"), Gd.K("MOUSE_BUTTON_WHEEL_DOWN"))))) {
        set_v.C("call", Gd.clampf(Gd.snappedf((get_v.C("call") + ((Gv.Eq(e.G("button_index"), Gd.K("MOUSE_BUTTON_WHEEL_UP")) ? Gv.F(0.2) : Gv.F(-0.2)))), Gv.F(0.1)), Gv.F(1.0), Gv.F(10.0)));
        Self.C("accept_event");
      }
    } else if ((Gv.Is(e, "InputEventMouseMotion") && (drag).T())) {
      set_v.C("call", Gd.snappedf(_val_from(e.G("position")), Gv.F(0.1)));
      Self.C("accept_event");
    }
    return Gv.Null;
  }
  public Gv _draw() {
    Gv c = (Self.G("size") * Gv.F(0.5));
    Gv R = ((Gd.minf(Self.G("size").G("x"), Self.G("size").G("y")) * Gv.F(0.5)) - Gv.F(4.0));
    Gv r_b = (R * Gv.F(0.7));
    Self.C("draw_circle", (c + Gd.Ctor("Vector2", Gv.I(2L), Gv.I(3L))), (r_b + Gv.F(3.0)), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.35)));
    Self.C("draw_circle", c, r_b, Gd.Ctor("Color", Gv.S("c3c7cc")));
    Self.C("draw_arc", c, r_b, Gv.I(0L), Gd.K("TAU"), Gv.I(48L), Gd.Ctor("Color", Gv.S("5b6066")), Gv.F(2.0), Gv.B(true));
    Self.C("draw_arc", c, (r_b - Gv.F(4.0)), Gv.I(0L), Gd.K("TAU"), Gv.I(48L), Gd.Ctor("Color", Gv.F(0.93), Gv.F(0.94), Gv.F(0.95), Gv.F(0.7)), Gv.F(1.5), Gv.B(true));
    Gv val = (get_v.C("call")).AsF();
    Gv a0 = Gd.deg_to_rad(Gv.F(-135.0));
    Gv frac = (((val - Gv.F(1.0))) / Gv.F(9.0));
    Self.C("draw_arc", c, (r_b + Gv.F(6.0)), (a0 - (Gd.K("PI") * Gv.F(0.5))), ((a0 - (Gd.K("PI") * Gv.F(0.5))) + (Gd.deg_to_rad(Gv.F(270.0)) * frac)), Gv.I(48L), Gd.Ctor("Color", Gv.S("f2c230")), Gv.F(5.0), Gv.B(true));
    Self.C("draw_arc", c, (r_b + Gv.F(6.0)), ((a0 - (Gd.K("PI") * Gv.F(0.5))) + (Gd.deg_to_rad(Gv.F(270.0)) * frac)), ((a0 - (Gd.K("PI") * Gv.F(0.5))) + Gd.deg_to_rad(Gv.F(270.0))), Gv.I(48L), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.1)), Gv.F(5.0), Gv.B(true));
    foreach (Gv i in (Gv.I(10L)).Iter()) {
      Gv a = (Gd.deg_to_rad((Gv.F(-135.0) + (Gv.F(30.0) * i)))).AsF();
      Gv dv = Gd.Ctor("Vector2", Gd.sin(a), (-Gd.cos(a)));
      Self.C("draw_line", (c + (dv * ((r_b - Gv.F(10.0))))), (c + (dv * ((r_b - Gv.F(2.0))))), Gd.Ctor("Color", Gv.S("2a2d30")), Gv.F(2.0));
      Gv t = Gd.Dyn("str", (i + Gv.I(1L)));
      Gv fs = Gv.I(15L);
      Gv tw = (font.C("get_string_size", t, Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), fs).G("x")).AsF();
      Self.C("draw_string", font, ((c + (dv * ((r_b + Gv.F(20.0))))) + Gd.Ctor("Vector2", ((-tw) * Gv.F(0.5)), (fs * Gv.F(0.36)))), t, Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), fs, (Gv.Lt(Gd.absf((val - ((i + Gv.I(1L))))), Gv.F(0.5)) ? Gd.Ctor("Color", Gv.S("eef2f5")) : Gd.Ctor("Color", Gv.S("8a939b"))));
    }
    Gv rk = (r_b * Gv.F(0.62));
    Self.C("draw_circle", c, rk, Gd.Ctor("Color", Gv.S("1c1e21")));
    Self.C("draw_arc", c, rk, Gv.I(0L), Gd.K("TAU"), Gv.I(40L), Gd.StaticGet("Color", "BLACK"), Gv.F(2.0), Gv.B(true));
    foreach (Gv i_2 in (Gv.I(18L)).Iter()) {
      Gv ga = ((Gd.K("TAU") * i_2) / Gv.F(18.0));
      Self.C("draw_line", (c + (Gd.Ctor("Vector2", Gd.cos(ga), Gd.sin(ga)) * ((rk - Gv.F(4.0))))), (c + (Gd.Ctor("Vector2", Gd.cos(ga), Gd.sin(ga)) * rk)), Gd.Ctor("Color", Gv.S("33363a")), Gv.F(2.0));
    }
    Gv ang = (Gd.deg_to_rad((Gv.F(-135.0) + (Gv.F(270.0) * frac)))).AsF();
    Gv dd = Gd.Ctor("Vector2", Gd.sin(ang), (-Gd.cos(ang)));
    Self.C("draw_line", (c + (dd * Gv.F(6.0))), (c + (dd * ((rk - Gv.F(5.0))))), Gd.StaticGet("Color", "WHITE"), Gv.F(4.0), Gv.B(true));
    Gv mt = font.C("get_string_size", Gv.S("−"), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(18L)).G("x");
    Self.C("draw_string", font, (c + Gd.Ctor("Vector2", (((-r_b) * Gv.F(0.62)) - (mt * Gv.F(0.5))), (r_b + Gv.F(14.0)))), Gv.S("−"), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(18L), Gd.Ctor("Color", Gv.S("8a939b")));
    Self.C("draw_string", font, (c + Gd.Ctor("Vector2", ((r_b * Gv.F(0.62)) - (mt * Gv.F(0.5))), (r_b + Gv.F(14.0)))), Gv.S("+"), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(18L), Gd.Ctor("Color", Gv.S("8a939b")));
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "font": return this.font;
    case "get_v": return this.get_v;
    case "set_v": return this.set_v;
    case "on_click": return this.on_click;
    case "drag": return this.drag;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "font": this.font = v; return v;
    case "get_v": this.get_v = v; return v;
    case "set_v": this.set_v = v; return v;
    case "on_click": this.on_click = v; return v;
    case "drag": this.drag = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "_ready": return _ready();
    case "_val_from": return _val_from(Gd.A(a, 0));
    case "_gui_input": return _gui_input(Gd.A(a, 0));
    case "_draw": return _draw();
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_oppanel_Knob(); return Gv.O(o); }
  public override string ScriptName { get { return "S_oppanel_Knob"; } }
}
public partial class S_oppanel : GdNode {
  public static Gv YEL;
  public static Gv GREEN;
  public static Gv RED;
  public Gv m;
  public Gv S;
  public Gv rows;
  public Gv fb;
  public Gv fr;
  public Gv t_acc;
  static S_oppanel() {
    YEL = Gd.Ctor("Color", Gv.S("f2c230"));
    GREEN = Gd.Ctor("Color", Gv.S("2f9e55"));
    RED = Gd.Ctor("Color", Gv.S("c8372b"));
  }
  public S_oppanel() {
    TypeName = "PanelContainer";
    m = Gv.Null;
    S = Gv.Null;
    rows = Gd.Arr();
    fb = Gv.Null;
    fr = Gv.Null;
    t_acc = Gv.F(0.0);
  }
  public Gv setup() {
    S = m.G("sim").G("S");
    fb = m.G("font_b");
    fr = m.G("font");
    Self.S("anchor_left", Gv.F(1.0));
    Self.S("anchor_right", Gv.F(1.0));
    Self.S("anchor_bottom", Gv.F(1.0));
    Self.S("offset_top", Gv.I(52L));
    Self.S("offset_bottom", Gv.I(0L));
    Gv psb = Gd.New("StyleBoxFlat");
    psb = psb.S("bg_color", Gd.Ctor("Color", Gv.S("0b0f13")));
    psb = psb.S("border_color", Gd.Ctor("Color", Gv.S("2a323a")));
    psb = psb.S("border_width_left", Gv.I(2L));
    psb = psb.S("content_margin_left", Gv.I(10L));
    psb = psb.S("content_margin_right", Gv.I(6L));
    psb = psb.S("content_margin_top", Gv.I(8L));
    psb = psb.S("content_margin_bottom", Gv.I(8L));
    Self.C("add_theme_stylebox_override", Gv.S("panel"), psb);
    Gv sc = Gd.New("ScrollContainer");
    sc = sc.S("horizontal_scroll_mode", Gd.StaticGet("ScrollContainer", "SCROLL_MODE_DISABLED"));
    Self.C("add_child", sc);
    Gv box = Gd.New("VBoxContainer");
    box = box.S("size_flags_horizontal", Gd.StaticGet("Control", "SIZE_EXPAND_FILL"));
    box.C("add_theme_constant_override", Gv.S("separation"), Gv.I(8L));
    sc.C("add_child", box);
    _build(box);
    refresh();
    return Gv.Null;
  }
  public Gv fit() {
    ;
    return Gv.Null;
  }
  public Gv reset_view() {
    ;
    return Gv.Null;
  }
  public Gv _card(Gv box = default(Gv), Gv title = default(Gv)) {
    Gv p = Gd.New("PanelContainer");
    Gv sb = Gd.New("StyleBoxFlat");
    sb = sb.S("bg_color", Gd.Ctor("Color", Gv.S("151b21")));
    sb.C("set_corner_radius_all", Gv.I(10L));
    sb = sb.S("border_color", Gd.Ctor("Color", Gv.S("252e37")));
    sb.C("set_border_width_all", Gv.I(1L));
    sb = sb.S("content_margin_left", Gv.I(10L));
    sb = sb.S("content_margin_right", Gv.I(10L));
    sb = sb.S("content_margin_top", Gv.I(6L));
    sb = sb.S("content_margin_bottom", Gv.I(10L));
    p.C("add_theme_stylebox_override", Gv.S("panel"), sb);
    box.C("add_child", p);
    Gv v = Gd.New("VBoxContainer");
    v.C("add_theme_constant_override", Gv.S("separation"), Gv.I(6L));
    p.C("add_child", v);
    v.C("add_child", _lbl(title, Gv.I(13L), YEL));
    return v;
    return Gv.Null;
  }
  public Gv _lbl(Gv t = default(Gv), Gv size = default(Gv), Gv col = default(Gv), Gv bold = default(Gv)) {
    size = (size).AsI();
    if (bold.k == 0) bold = Gv.B(true);
    Gv l = Gd.New("Label");
    l = l.S("text", t);
    l.C("add_theme_font_override", Gv.S("font"), ((bold).T() ? fb : fr));
    l.C("add_theme_font_size_override", Gv.S("font_size"), size);
    l.C("add_theme_color_override", Gv.S("font_color"), col);
    return l;
    return Gv.Null;
  }
  public Gv _sb(Gv bg = default(Gv), Gv border = default(Gv)) {
    Gv s = Gd.New("StyleBoxFlat");
    s = s.S("bg_color", bg);
    s.C("set_corner_radius_all", Gv.I(8L));
    s = s.S("border_color", border);
    s.C("set_border_width_all", Gv.I(1L));
    s = s.S("content_margin_left", Gv.I(8L));
    s = s.S("content_margin_right", Gv.I(8L));
    s = s.S("content_margin_top", Gv.I(6L));
    s = s.S("content_margin_bottom", Gv.I(6L));
    return s;
    return Gv.Null;
  }
  public Gv _btn(Gv parent = default(Gv), Gv text = default(Gv), Gv f = default(Gv), Gv minw = default(Gv), Gv h = default(Gv)) {
    if (minw.k == 0) minw = Gv.F(0.0);
    if (h.k == 0) h = Gv.F(40.0);
    Gv b = Gd.New("Button");
    b = b.S("text", text);
    b = b.S("focus_mode", Gd.StaticGet("Control", "FOCUS_NONE"));
    b = b.S("custom_minimum_size", Gd.Ctor("Vector2", minw, h));
    b.C("add_theme_font_override", Gv.S("font"), fb);
    b.C("add_theme_font_size_override", Gv.S("font_size"), Gv.I(15L));
    _style(b, Gv.B(false));
    b.G("pressed").C("connect", Gd.Fn((_a) => {  f.C("call"); _click(); refresh(); return Gv.Null; }));
    parent.C("add_child", b);
    return b;
    return Gv.Null;
  }
  public Gv _style(Gv b = default(Gv), Gv on = default(Gv), Gv col = default(Gv)) {
    if (col.k == 0) col = GREEN;
    Gv n = _sb(((on).T() ? col : Gd.Ctor("Color", Gv.S("222a32"))), ((on).T() ? col.C("lightened", Gv.F(0.25)) : Gd.Ctor("Color", Gv.S("37424d"))));
    Gv hv = _sb(((on).T() ? col.C("lightened", Gv.F(0.12)) : Gd.Ctor("Color", Gv.S("2c3640"))), YEL);
    b.C("add_theme_stylebox_override", Gv.S("normal"), n);
    b.C("add_theme_stylebox_override", Gv.S("hover"), hv);
    b.C("add_theme_stylebox_override", Gv.S("pressed"), hv);
    b.C("add_theme_color_override", Gv.S("font_color"), ((on).T() ? Gd.StaticGet("Color", "WHITE") : Gd.Ctor("Color", Gv.S("c9d1d8"))));
    b.C("add_theme_color_override", Gv.S("font_hover_color"), Gd.StaticGet("Color", "WHITE"));
    return Gv.Null;
  }
  public Gv _toggle(Gv parent = default(Gv), Gv name = default(Gv), Gv get_on = default(Gv), Gv f = default(Gv), Gv col = default(Gv), Gv minw = default(Gv)) {
    if (col.k == 0) col = GREEN;
    if (minw.k == 0) minw = Gv.F(0.0);
    Gv b = _btn(parent, name, f, minw);
    b = b.S("size_flags_horizontal", Gd.StaticGet("Control", "SIZE_EXPAND_FILL"));
    rows.C("append", Gd.Fn((_a) => {  Gv on = get_on.C("call"); Gv t = (Gv.S("%s: %s") % Gd.Arr(name, ((on).T() ? Gv.S("ZAP") : Gv.S("vyp")))); if (Gv.Ne(b.G("text"), t)) {   b = b.S("text", t); } if (Gv.Ne(b.C("get_meta", Gv.S("on"), Gv.Null), on)) {   b.C("set_meta", Gv.S("on"), on);   _style(b, on, col); } return Gv.Null; }));
    return b;
    return Gv.Null;
  }
  public Gv _click() {
    if (Gv.Ne(m.G("game"), Gv.Null)) {
      m.G("game").C("play", Gv.S("click"));
    }
    return Gv.Null;
  }
  public Gv _cab(Gv k = default(Gv)) {
    if (Gv.Ne(m.G("cabinet"), Gv.Null)) {
      m.G("cabinet").C("act", k, Gv.B(true));
    }
    m.C("redraw");
    return Gv.Null;
  }
  public Gv _fm(Gv parent = default(Gv), Gv get_st = default(Gv), Gv set_st = default(Gv), Gv info = default(Gv)) {
    Gv h = Gd.New("HBoxContainer");
    h.C("add_theme_constant_override", Gv.S("separation"), Gv.I(10L));
    parent.C("add_child", h);
    Gv k = S_oppanel_Knob.New();
    k = k.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(170L), Gv.I(170L)));
    k = k.S("font", fb);
    k = k.S("get_v", get_st);
    k = k.S("set_v", Gd.Fn((_a) => { Gv x = Gd.A(_a, 0); set_st.C("call", x); refresh(); return Gv.Null; }));
    k = k.S("on_click", Gd.Fn((_a) => _click()));
    h.C("add_child", k);
    Gv v = Gd.New("VBoxContainer");
    v = v.S("alignment", Gd.StaticGet("BoxContainer", "ALIGNMENT_CENTER"));
    v = v.S("size_flags_horizontal", Gd.StaticGet("Control", "SIZE_EXPAND_FILL"));
    h.C("add_child", v);
    v.C("add_child", _lbl(Gv.S("STUPEŇ"), Gv.I(12L), Gd.Ctor("Color", Gv.S("9aa3ab"))));
    Gv big = _lbl(Gv.S(""), Gv.I(44L), YEL);
    v.C("add_child", big);
    Gv lb = _lbl(Gv.S(""), Gv.I(15L), Gd.Ctor("Color", Gv.S("eef2f5")), Gv.B(false));
    lb = lb.S("autowrap_mode", Gd.StaticGet("TextServer", "AUTOWRAP_WORD_SMART"));
    v.C("add_child", lb);
    Gv hint = _lbl(Gv.S("táhni kolem knoflíku / kolečko"), Gv.I(11L), Gd.Ctor("Color", Gv.S("6b757f")), Gv.B(false));
    v.C("add_child", hint);
    rows.C("append", Gd.Fn((_a) => {  Gv cur = (get_st.C("call")).AsF(); Gv t = ((Gv.S("%.1f") % cur)).C("replace", Gv.S("."), Gv.S(",")); if (Gv.Ne(big.G("text"), t)) {   big = big.S("text", t); } Gv t2 = info.C("call"); if (Gv.Ne(lb.G("text"), t2)) {   lb = lb.S("text", t2); } k.C("queue_redraw"); return Gv.Null; }));
    return Gv.Null;
  }
  public Gv _pump_st(Gv b = default(Gv)) {
    Gv F = (Gv.Ne(m.G("cabinet"), Gv.Null) ? m.G("cabinet").G("FMS").I(b) : Gd.Dict(Gv.S("min"), Gv.F(10.0), Gv.S("max"), Gv.F(400.0)));
    return ((Gv.F(1.0) + (Gv.F(9.0) * Gd.clampf((((S.I(b).G("flow") - F.G("min"))) / ((F.G("max") - F.G("min")))), Gv.F(0.0), Gv.F(1.0))))).AsF();
    return Gv.Null;
  }
  public Gv _pump_set(Gv b = default(Gv), Gv st = default(Gv)) {
    st = (st).AsF();
    Gv F = (Gv.Ne(m.G("cabinet"), Gv.Null) ? m.G("cabinet").G("FMS").I(b) : Gd.Dict(Gv.S("min"), Gv.F(10.0), Gv.S("max"), Gv.F(400.0)));
    S = S.SI(b, S.I(b).S("flow", Gd.roundf((F.G("min") + ((((F.G("max") - F.G("min"))) * ((Gd.clampf(st, Gv.F(1.0), Gv.F(10.0)) - Gv.F(1.0)))) / Gv.F(9.0))))));
    return Gv.Null;
  }
  public Gv _disp(Gv parent = default(Gv), Gv title = default(Gv), Gv val = default(Gv), Gv sp_up = default(Gv), Gv sp_dn = default(Gv), Gv sp = default(Gv)) {
    if (sp_up.k == 0) sp_up = Gv.S("");
    if (sp_dn.k == 0) sp_dn = Gv.S("");
    if (sp.k == 0) sp = Gd.Ctor("Callable");
    Gv p = Gd.New("PanelContainer");
    p = p.S("size_flags_horizontal", Gd.StaticGet("Control", "SIZE_EXPAND_FILL"));
    Gv sb = _sb(Gd.Ctor("Color", Gv.S("0d0f11")), Gd.Ctor("Color", Gv.S("3a434b")));
    p.C("add_theme_stylebox_override", Gv.S("panel"), sb);
    parent.C("add_child", p);
    Gv v = Gd.New("VBoxContainer");
    v.C("add_theme_constant_override", Gv.S("separation"), Gv.I(0L));
    p.C("add_child", v);
    v.C("add_child", _lbl(title, Gv.I(11L), Gd.Ctor("Color", Gv.S("9aa3ab"))));
    Gv h = Gd.New("HBoxContainer");
    v.C("add_child", h);
    Gv big = _lbl(Gv.S("---"), Gv.I(30L), Gd.Ctor("Color", Gv.S("ff3b2f")));
    big = big.S("size_flags_horizontal", Gd.StaticGet("Control", "SIZE_EXPAND_FILL"));
    h.C("add_child", big);
    Gv spl = Gv.Null;
    if (Gv.Ne(sp_up, Gv.S(""))) {
      Gv c = Gd.New("VBoxContainer");
      c.C("add_theme_constant_override", Gv.S("separation"), Gv.I(2L));
      h.C("add_child", c);
      Gv u = _btn(c, Gv.S("▲"), Gd.Fn((_a) => {  _cab(sp_up); return Gv.Null; }), Gv.I(34L), Gv.I(22L));
      u.C("add_theme_font_size_override", Gv.S("font_size"), Gv.I(11L));
      Gv d = _btn(c, Gv.S("▼"), Gd.Fn((_a) => {  _cab(sp_dn); return Gv.Null; }), Gv.I(34L), Gv.I(22L));
      d.C("add_theme_font_size_override", Gv.S("font_size"), Gv.I(11L));
      spl = _lbl(Gv.S(""), Gv.I(11L), YEL);
      v.C("add_child", spl);
    }
    rows.C("append", Gd.Fn((_a) => {  Gv t = val.C("call"); if (Gv.Ne(big.G("text"), t)) {   big = big.S("text", t); } if (Gv.Ne(spl, Gv.Null)) {   Gv st = (Gv.S("nastaveno %d °C") % Gd.roundi(sp.C("call")));   if (Gv.Ne(spl.G("text"), st)) {     spl = spl.S("text", st);   } } return Gv.Null; }));
    return Gv.Null;
  }
  public Gv _row(Gv parent = default(Gv)) {
    Gv h = Gd.New("HBoxContainer");
    h.C("add_theme_constant_override", Gv.S("separation"), Gv.I(6L));
    parent.C("add_child", h);
    return h;
    return Gv.Null;
  }
  public Gv _status(Gv parent = default(Gv), Gv f = default(Gv)) {
    Gv l = _lbl(Gv.S(""), Gv.I(12L), Gd.Ctor("Color", Gv.S("9aa3ab")), Gv.B(false));
    l = l.S("autowrap_mode", Gd.StaticGet("TextServer", "AUTOWRAP_WORD_SMART"));
    parent.C("add_child", l);
    rows.C("append", Gd.Fn((_a) => {  Gv t = f.C("call"); if (Gv.Ne(l.G("text"), t)) {   l = l.S("text", t); } l = l.S("visible", Gv.B(Gv.Ne(t, Gv.S("")))); return Gv.Null; }));
    return Gv.Null;
  }
  public Gv _build(Gv box = default(Gv)) {
    Gv _c = S.G("cab");
    Gv v = _card(box, Gv.S("NAPÁJENÍ ROZVADĚČE"));
    Gv r = _row(v);
    _toggle(r, Gv.S("HLAVNÍ"), Gd.Fn((_a) => {  return S.G("cab").G("main"); return Gv.Null; }), Gd.Fn((_a) => {  _cab(Gv.S("main")); return Gv.Null; }));
    _toggle(r, Gv.S("KLÍČ START"), Gd.Fn((_a) => {  return S.G("cab").G("key"); return Gv.Null; }), Gd.Fn((_a) => {  _cab(Gv.S("key")); return Gv.Null; }));
    Gv es = _toggle(r, Gv.S("NOUZ. STOP"), Gd.Fn((_a) => {  return S.G("cab").G("estop"); return Gv.Null; }), Gd.Fn((_a) => {  _cab(Gv.S("estop")); return Gv.Null; }), RED);
    es = es.S("tooltip_text", Gv.S("Nouzové vypnutí – stiskni znovu pro odblokování, pak otoč klíč START"));
    v = _card(box, Gv.S("TEPLOTY"));
    Gv g = Gd.New("GridContainer");
    g = g.S("columns", Gv.I(2L));
    g.C("add_theme_constant_override", Gv.S("h_separation"), Gv.I(6L));
    g.C("add_theme_constant_override", Gv.S("v_separation"), Gv.I(6L));
    v.C("add_child", g);
    _disp(g, Gv.S("PÁNEV (MP)"), Gd.Fn((_a) => {  return (Gv.Ne(m.G("cabinet"), Gv.Null) ? m.G("cabinet").C("_disp_text", Gv.S("mp")) : Gv.S("")); return Gv.Null; }), Gv.S("spmp_u"), Gv.S("spmp_d"), Gd.Fn((_a) => {  return S.G("cab").C("get", Gv.S("mpSet"), Gv.I(50L)); return Gv.Null; }));
    _disp(g, Gv.S("BOJLER"), Gd.Fn((_a) => {  return (Gv.Ne(m.G("cabinet"), Gv.Null) ? m.G("cabinet").C("_disp_text", Gv.S("bo")) : Gv.S("")); return Gv.Null; }), Gv.S("spbo_u"), Gv.S("spbo_d"), Gd.Fn((_a) => {  return S.G("hlt").G("set"); return Gv.Null; }));
    _disp(g, Gv.S("SCEZOVACÍ KÁĎ"), Gd.Fn((_a) => {  return (Gv.Ne(m.G("cabinet"), Gv.Null) ? m.G("cabinet").C("_disp_text", Gv.S("sk")) : Gv.S("")); return Gv.Null; }));
    _disp(g, Gv.S("MLADINA (za chladičem)"), Gd.Fn((_a) => {  return (Gv.Ne(m.G("cabinet"), Gv.Null) ? m.G("cabinet").C("_disp_text", Gv.S("ml")) : Gv.S("")); return Gv.Null; }));
    v = _card(box, (Gv.S("RMUTOVACÍ ČERPADLO ") + m.C("pump_name", Gv.S("p1"))));
    _pump(v, Gv.S("p1"), Gv.S("pvarna"), Gv.B(true));
    v = _card(box, (Gv.S("ČERPADLO BOILERU ") + m.C("pump_name", Gv.S("phw"))));
    r = _row(v);
    Gv pb = _toggle(r, Gv.S("ČERPADLO BOILERU"), Gd.Fn((_a) => {  return Gv.B(((S.G("rvpWater").G("on")).T() || (S.G("skWater").G("on")).T())); return Gv.Null; }), Gd.Fn((_a) => {  _cab(Gv.S("pboil")); return Gv.Null; }));
    pb = pb.S("custom_minimum_size", pb.G("custom_minimum_size").S("y", Gv.I(48L)));
    pb.C("add_theme_font_size_override", Gv.S("font_size"), Gv.I(18L));
    _fm(v, Gd.Fn((_a) => {  return (Gv.F(1.0) + (Gv.F(9.0) * Gd.clampf((((S.G("cfg").G("waterFlow") - Gv.F(50.0))) / Gv.F(250.0)), Gv.F(0.0), Gv.F(1.0)))); return Gv.Null; }), Gd.Fn((_a) => { Gv st = Gd.A(_a, 0); S = S.S("cfg", S.G("cfg").S("waterFlow", Gd.roundf((Gv.F(50.0) + ((Gv.F(250.0) * ((Gd.clampf(st, Gv.F(1.0), Gv.F(10.0)) - Gv.F(1.0)))) / Gv.F(9.0)))))); return Gv.Null; }), Gd.Fn((_a) => {  return ((S.G("rvpWater").G("on")).T() ? ((Gv.S("napouští pánev %d / %d L · %d L/min") % Gd.Arr(Gd.roundi(S.G("rvpWater").G("added")), Gd.roundi(S.G("rvpWater").G("target")), Gd.roundi(S.G("cfg").G("waterFlow"))))) : (((S.G("skWater").G("on")).T() ? (Gv.S("vyslazuje %d L/min") % Gd.roundi(S.G("skWater").G("flow"))) : (Gv.S("nastaveno %d L/min") % Gd.roundi(S.G("cfg").G("waterFlow")))))); return Gv.Null; }));
    v = _card(box, Gv.S("OHŘEV PÁNVE – RUČNĚ / AUTOMAT"));
    r = _row(v);
    Gv bm = _btn(r, Gv.S("RUČNĚ"), Gd.Fn((_a) => {  if ((S.G("cab").C("get", Gv.S("mpAuto"), Gv.B(false))).T()) {   _cab(Gv.S("mpauto")); } return Gv.Null; }), Gv.I(0L), Gv.I(48L));
    Gv ba = _btn(r, Gv.S("AUTOMAT"), Gd.Fn((_a) => {  if (!(S.G("cab").C("get", Gv.S("mpAuto"), Gv.B(false))).T()) {   _cab(Gv.S("mpauto")); } return Gv.Null; }), Gv.I(0L), Gv.I(48L));
    bm = bm.S("size_flags_horizontal", Gd.StaticGet("Control", "SIZE_EXPAND_FILL"));
    ba = ba.S("size_flags_horizontal", Gd.StaticGet("Control", "SIZE_EXPAND_FILL"));
    bm.C("add_theme_font_size_override", Gv.S("font_size"), Gv.I(18L));
    ba.C("add_theme_font_size_override", Gv.S("font_size"), Gv.I(18L));
    rows.C("append", Gd.Fn((_a) => {  Gv a = S.G("cab").C("get", Gv.S("mpAuto"), Gv.B(false)); if (Gv.Ne(bm.C("get_meta", Gv.S("on"), Gv.Null), (Gv.B(!(a).T())))) {   bm.C("set_meta", Gv.S("on"), Gv.B(!(a).T()));   _style(bm, Gv.B(!(a).T()), Gd.Ctor("Color", Gv.S("2f6fb0"))); } if (Gv.Ne(ba.C("get_meta", Gv.S("on"), Gv.Null), a)) {   ba.C("set_meta", Gv.S("on"), a);   _style(ba, a, Gd.Ctor("Color", Gv.S("2f6fb0"))); } return Gv.Null; }));
    r = _row(v);
    Gv oh = _toggle(r, Gv.S("OHŘEV PÁNVE"), Gd.Fn((_a) => {  return S.G("cab").C("get", Gv.S("mpHeatOn"), Gv.B(false)); return Gv.Null; }), Gd.Fn((_a) => {  _cab(Gv.S("mpheat")); return Gv.Null; }), Gd.Ctor("Color", Gv.S("c46a1f")));
    oh = oh.S("custom_minimum_size", oh.G("custom_minimum_size").S("y", Gv.I(48L)));
    oh.C("add_theme_font_size_override", Gv.S("font_size"), Gv.I(18L));
    Gv hr = _row(v);
    Gv hl_ = _lbl(Gv.S("pára:"), Gv.I(14L), Gd.Ctor("Color", Gv.S("c9d1d8")), Gv.B(false));
    hr.C("add_child", hl_);
    Gv hb = Gd.Arr();
    foreach (Gv p in (Gd.Arr(Gv.I(0L), Gv.I(25L), Gv.I(50L), Gv.I(75L), Gv.I(100L))).Iter()) {
      Gv pp = (p).AsI();
      Gv b = _btn(hr, (Gv.S("%d %%") % p), Gd.Fn((_a) => {  S = S.S("rvp", S.G("rvp").S("heat", (pp / Gv.F(100.0)))); S = S.S("cab", S.G("cab").S("mpHeatOn", Gv.B(Gv.Gt(pp, Gv.I(0L))))); m.C("redraw"); return Gv.Null; }), Gv.I(0L), Gv.I(40L));
      b = b.S("size_flags_horizontal", Gd.StaticGet("Control", "SIZE_EXPAND_FILL"));
      hb.C("append", Gd.Arr(b, pp));
    }
    Gv spr = _row(v);
    spr.C("add_child", _lbl(Gv.S("automat drží:"), Gv.I(14L), Gd.Ctor("Color", Gv.S("c9d1d8")), Gv.B(false)));
    Gv spl = _lbl(Gv.S(""), Gv.I(26L), YEL);
    spl = spl.S("size_flags_horizontal", Gd.StaticGet("Control", "SIZE_EXPAND_FILL"));
    spl = spl.S("horizontal_alignment", Gd.K("HORIZONTAL_ALIGNMENT_CENTER"));
    spr.C("add_child", spl);
    Gv su = _btn(spr, Gv.S("▲"), Gd.Fn((_a) => {  _cab(Gv.S("spmp_u")); return Gv.Null; }), Gv.I(48L), Gv.I(40L));
    Gv sd = _btn(spr, Gv.S("▼"), Gd.Fn((_a) => {  _cab(Gv.S("spmp_d")); return Gv.Null; }), Gv.I(48L), Gv.I(40L));
    _status(v, Gd.Fn((_a) => {  return (!(S.G("cab").C("get", Gv.S("mpAuto"), Gv.B(false))).T() ? ((Gv.S("RUČNĚ: pára %d %% podle tlačítek.") % Gd.roundi((S.G("rvp").G("heat") * Gv.F(100.0))))) : ((Gv.S("AUTOMAT: pustí páru, když je pánev pod %d °C (teď %s °C).") % Gd.Arr(Gd.roundi(S.G("cab").C("get", Gv.S("mpSet"), Gv.I(50L))), m.C("c1", S.G("rvp").G("Tsens")))))); return Gv.Null; }));
    rows.C("append", Gd.Fn((_a) => {  Gv a_2 = S.G("cab").C("get", Gv.S("mpAuto"), Gv.B(false)); hr = hr.S("visible", Gv.B(!(a_2).T())); spr = spr.S("visible", a_2); Gv t = (Gv.S("%d °C") % Gd.roundi(S.G("cab").C("get", Gv.S("mpSet"), Gv.I(50L)))); if (Gv.Ne(spl.G("text"), t)) {   spl = spl.S("text", t); } foreach (Gv x in (hb).Iter()) {   Gv on = Gv.B(Gv.Eq(Gd.roundi((S.G("rvp").G("heat") * Gv.F(100.0))), x.I(Gv.I(1L))));   if (Gv.Ne(x.I(Gv.I(0L)).C("get_meta", Gv.S("on"), Gv.Null), on)) {     x.I(Gv.I(0L)).C("set_meta", Gv.S("on"), on);     _style(x.I(Gv.I(0L)), on, Gd.Ctor("Color", Gv.S("c46a1f")));   } } return Gv.Null; }));
    v = _card(box, Gv.S("KOTEL A BOJLER"));
    r = _row(v);
    _toggle(r, Gv.S("PÁRA Z KOTLE"), Gd.Fn((_a) => {  return S.G("boiler").G("on"); return Gv.Null; }), Gd.Fn((_a) => {  S = S.S("boiler", S.G("boiler").S("on", Gv.B(!(S.G("boiler").G("on")).T()))); m.C("redraw"); return Gv.Null; }), Gd.Ctor("Color", Gv.S("c46a1f")));
    _toggle(r, Gv.S("OHŘEV BOJLERU"), Gd.Fn((_a) => {  return S.G("hlt").G("auto"); return Gv.Null; }), Gd.Fn((_a) => {  _cab(Gv.S("hltheat")); return Gv.Null; }), Gd.Ctor("Color", Gv.S("c46a1f")));
    _status(v, Gd.Fn((_a) => {  return ((S.G("boiler").G("on")).T() ? ((Gv.S("kotel zatížen %d %%") % Gd.roundi((S.G("boiler").G("load") * Gv.F(100.0))))) : Gv.S("")); return Gv.Null; }));
    v = _card(box, Gv.S("MÍCHADLO PÁNVE"));
    r = _row(v);
    _toggle(r, Gv.S("MÍCHADLO MP"), Gd.Fn((_a) => {  return S.G("rvp").G("stir"); return Gv.Null; }), Gd.Fn((_a) => {  _cab(Gv.S("stirmp")); return Gv.Null; }));
    _fm(v, Gd.Fn((_a) => {  return (Gv.F(1.0) + (Gv.F(9.0) * Gd.clampf((((S.G("rvp").G("hz") - Gv.F(10.0))) / Gv.F(40.0)), Gv.F(0.0), Gv.F(1.0)))); return Gv.Null; }), Gd.Fn((_a) => { Gv st_2 = Gd.A(_a, 0); S = S.S("rvp", S.G("rvp").S("hz", Gd.roundf((Gv.F(10.0) + ((Gv.F(40.0) * ((Gd.clampf(st_2, Gv.F(1.0), Gv.F(10.0)) - Gv.F(1.0)))) / Gv.F(9.0)))))); m.C("redraw"); return Gv.Null; }), Gd.Fn((_a) => {  return (Gv.S("%d Hz") % Gd.roundi(S.G("rvp").G("hz"))); return Gv.Null; }));
    v = _card(box, Gv.S("SCEZOVACÍ KÁĎ – KOPAČKA"));
    r = _row(v);
    _toggle(r, Gv.S("KOPAČKA"), Gd.Fn((_a) => {  return S.G("sk").G("rake"); return Gv.Null; }), Gd.Fn((_a) => {  _cab(Gv.S("rake")); return Gv.Null; }));
    _fm(v, Gd.Fn((_a) => {  return (Gv.F(1.0) + (Gv.F(9.0) * S.G("cab").C("get", Gv.S("potsk"), Gv.F(0.5)))); return Gv.Null; }), Gd.Fn((_a) => { Gv st_3 = Gd.A(_a, 0); S = S.S("cab", S.G("cab").S("potsk", (((Gd.clampf(st_3, Gv.F(1.0), Gv.F(10.0)) - Gv.F(1.0))) / Gv.F(9.0)))); m.C("redraw"); return Gv.Null; }), Gd.Fn((_a) => {  return Gv.S("otáčky kopačky"); return Gv.Null; }));
    r = _row(v);
    _toggle(r, Gv.S("HYDRAULIKA"), Gd.Fn((_a) => {  return S.G("cab").C("get", Gv.S("hydOn"), Gv.B(false)); return Gv.Null; }), Gd.Fn((_a) => {  _cab(Gv.S("hyd")); return Gv.Null; }));
    Gv up = Gd.New("Button");
    up = up.S("text", Gv.S("▲ NAHORU"));
    up = up.S("focus_mode", Gd.StaticGet("Control", "FOCUS_NONE"));
    up = up.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(0L), Gv.I(44L)));
    up = up.S("size_flags_horizontal", Gd.StaticGet("Control", "SIZE_EXPAND_FILL"));
    _style(up, Gv.B(false));
    up.C("add_theme_font_override", Gv.S("font"), fb);
    Gv dn = Gd.New("Button");
    dn = dn.S("text", Gv.S("▼ DOLŮ"));
    dn = dn.S("focus_mode", Gd.StaticGet("Control", "FOCUS_NONE"));
    dn = dn.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(0L), Gv.I(44L)));
    dn = dn.S("size_flags_horizontal", Gd.StaticGet("Control", "SIZE_EXPAND_FILL"));
    _style(dn, Gv.B(false));
    dn.C("add_theme_font_override", Gv.S("font"), fb);
    up.G("button_down").C("connect", Gd.Fn((_a) => {  m.G("cabinet").C("act", Gv.S("hup"), Gv.B(true)); return Gv.Null; }));
    up.G("button_up").C("connect", Gd.Fn((_a) => {  m.G("cabinet").C("act", Gv.S("hup"), Gv.B(false)); return Gv.Null; }));
    dn.G("button_down").C("connect", Gd.Fn((_a) => {  m.G("cabinet").C("act", Gv.S("hdn"), Gv.B(true)); return Gv.Null; }));
    dn.G("button_up").C("connect", Gd.Fn((_a) => {  m.G("cabinet").C("act", Gv.S("hdn"), Gv.B(false)); return Gv.Null; }));
    Gv r2 = _row(v);
    r2.C("add_child", up);
    r2.C("add_child", dn);
    _status(v, Gd.Fn((_a) => {  return (Gv.S("hloubka kopačky %d %% (drž tlačítko)") % Gd.roundi((S.G("sk").G("rakeH") * Gv.F(100.0)))); return Gv.Null; }));
    v = _card(box, Gv.S("OSTATNÍ ČERPADLA"));
    _pump(v, Gv.S("p2"), Gv.S("bxs"), Gv.B(true));
    _pump(v, Gv.S("p3"), Gv.S("rez2"), Gv.B(true));
    _sep(v);
    r = _row(v);
    _toggle(r, (m.C("pump_name", Gv.S("plv")) + Gv.S(" LEDOVÁ VODA")), Gd.Fn((_a) => {  return S.G("lv").G("pump"); return Gv.Null; }), Gd.Fn((_a) => {  _cab(Gv.S("rez1")); return Gv.Null; }));
    return Gv.Null;
  }
  public Gv _sep(Gv v = default(Gv)) {
    Gv s = Gd.New("ColorRect");
    s = s.S("color", Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.06)));
    s = s.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(0L), Gv.I(1L)));
    v.C("add_child", s);
    return Gv.Null;
  }
  public Gv _pump(Gv v = default(Gv), Gv b = default(Gv), Gv cab_key = default(Gv), Gv fm = default(Gv)) {
    if (Gv.Gt(v.C("get_child_count"), Gv.I(1L))) {
      _sep(v);
    }
    Gv r = _row(v);
    Gv pt = _toggle(r, m.C("pump_name", b), Gd.Fn((_a) => {  return S.I(b).G("on"); return Gv.Null; }), Gd.Fn((_a) => {  _cab(cab_key); return Gv.Null; }));
    pt = pt.S("custom_minimum_size", pt.G("custom_minimum_size").S("y", Gv.I(48L)));
    pt.C("add_theme_font_size_override", Gv.S("font_size"), Gv.I(18L));
    if ((fm).T()) {
      _fm(v, Gd.Fn((_a) => {  return _pump_st(b); return Gv.Null; }), Gd.Fn((_a) => { Gv st = Gd.A(_a, 0); _pump_set(b, st); m.C("redraw"); return Gv.Null; }), Gd.Fn((_a) => {  return ((S.I(b).G("on")).T() ? ((Gv.S("teče %d L/min (nastaveno %d)") % Gd.Arr(Gd.roundi(S.I(b).G("q")), Gd.roundi(S.I(b).G("flow"))))) : ((Gv.S("nastaveno %d L/min") % Gd.roundi(S.I(b).G("flow"))))); return Gv.Null; }));
    }
    _status(v, Gd.Fn((_a) => {  return ((S.I(b).G("on")).T() ? S.I(b).G("status") : Gv.S("")); return Gv.Null; }));
    return Gv.Null;
  }
  public Gv _process(Gv delta = default(Gv)) {
    delta = (delta).AsF();
    t_acc = (t_acc + delta);
    if ((Gv.Gt(t_acc, Gv.F(0.2)) && (Self.G("visible")).T())) {
      t_acc = Gv.F(0.0);
      refresh();
    }
    return Gv.Null;
  }
  public Gv refresh() {
    foreach (Gv f in (rows).Iter()) {
      f.C("call");
    }
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "YEL": return S_oppanel.YEL;
    case "GREEN": return S_oppanel.GREEN;
    case "RED": return S_oppanel.RED;
    case "m": return this.m;
    case "S": return this.S;
    case "rows": return this.rows;
    case "fb": return this.fb;
    case "fr": return this.fr;
    case "t_acc": return this.t_acc;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "m": this.m = v; return v;
    case "S": this.S = v; return v;
    case "rows": this.rows = v; return v;
    case "fb": this.fb = v; return v;
    case "fr": this.fr = v; return v;
    case "t_acc": this.t_acc = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "setup": return setup();
    case "fit": return fit();
    case "reset_view": return reset_view();
    case "_card": return _card(Gd.A(a, 0), Gd.A(a, 1));
    case "_lbl": return _lbl(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "_sb": return _sb(Gd.A(a, 0), Gd.A(a, 1));
    case "_btn": return _btn(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4));
    case "_style": return _style(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "_toggle": return _toggle(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5));
    case "_click": return _click();
    case "_cab": return _cab(Gd.A(a, 0));
    case "_fm": return _fm(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "_pump_st": return _pump_st(Gd.A(a, 0));
    case "_pump_set": return _pump_set(Gd.A(a, 0), Gd.A(a, 1));
    case "_disp": return _disp(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5));
    case "_row": return _row(Gd.A(a, 0));
    case "_status": return _status(Gd.A(a, 0), Gd.A(a, 1));
    case "_build": return _build(Gd.A(a, 0));
    case "_sep": return _sep(Gd.A(a, 0));
    case "_pump": return _pump(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "_process": return _process(Gd.A(a, 0));
    case "refresh": return refresh();
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    case "YEL": return YEL;
    case "GREEN": return GREEN;
    case "RED": return RED;
    case "Knob": return Gd.Script("S_oppanel_Knob");
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_oppanel(); return Gv.O(o); }
  public override string ScriptName { get { return "oppanel"; } }
}
}
