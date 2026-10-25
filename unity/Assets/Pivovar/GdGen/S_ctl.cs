// Vygenerováno z ctl.gd překladačem gd2cs.py – needituj ručně.
using System; using System.Collections.Generic;
namespace Pivovar.Gd.Gen {
public partial class S_ctl_Trend : GdNode {
  public Gv ctl;
  static S_ctl_Trend() {
  }
  public S_ctl_Trend() {
    ctl = Gv.Null;
  }
  public Gv _draw() {
    Gv r = Gd.Ctor("Rect2", Gd.StaticGet("Vector2", "ZERO"), Self.G("size"));
    S_art.rrect(Self, r, Gd.Ctor("Color", Gv.S("0b0f13")), Gv.F(6.0));
    Gv plot = Gd.Ctor("Rect2", Gv.I(28L), Gv.I(8L), (Self.G("size").G("x") - Gv.I(36L)), (Self.G("size").G("y") - Gv.I(52L)));
    Gv f = ctl.G("m").G("font");
    foreach (Gv v in (Gd.Arr(Gv.I(0L), Gv.I(25L), Gv.I(50L), Gv.I(75L), Gv.I(100L))).Iter()) {
      Gv y = ((plot.G("end").G("y") - ((plot.G("size").G("y") * v) / Gv.F(100.0)))).AsF();
      Self.C("draw_line", Gd.Ctor("Vector2", plot.G("position").G("x"), y), Gd.Ctor("Vector2", plot.G("end").G("x"), y), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), (((v % Gv.I(50L))).T() ? Gv.F(0.06) : Gv.F(0.1))), Gv.F(1.0));
      Self.C("draw_string", f, Gd.Ctor("Vector2", Gv.I(2L), (y + Gv.I(4L))), Gd.Dyn("str", v), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(10L), Gd.Ctor("Color", Gv.S("6b757f")));
    }
    Gv H = ctl.G("hist");
    if (Gv.Ge(H.C("size"), Gv.I(2L))) {
      Gv t0 = (H.I(Gv.I(0L)).I(Gv.I(0L))).AsF();
      Gv t1 = (Gd.maxf(H.I((H.C("size") - Gv.I(1L))).I(Gv.I(0L)), (t0 + Gv.F(600.0)))).AsF();
      foreach (Gv si in (S_ctl.SERIES.C("size")).Iter()) {
        Gv col = Gd.Ctor("Color", S_ctl.SERIES.I(si).I(Gv.I(2L)));
        Gv seg = Gd.Ctor("PackedVector2Array");
        foreach (Gv row in (H).Iter()) {
          Gv v_2 = row.I((si + Gv.I(1L)));
          if (Gv.Eq(v_2, Gv.Null)) {
            if (Gv.Ge(seg.C("size"), Gv.I(2L))) {
              Self.C("draw_polyline", seg, col, Gv.F(2.0), Gv.B(true));
            }
            seg = Gd.Ctor("PackedVector2Array");
            continue;
          }
          seg.C("append", Gd.Ctor("Vector2", (plot.G("position").G("x") + ((plot.G("size").G("x") * ((row.I(Gv.I(0L)) - t0))) / ((t1 - t0)))), (plot.G("end").G("y") - (plot.G("size").G("y") * Gd.clampf((v_2 / Gv.F(100.0)), Gv.F(0.0), Gv.F(1.05))))));
        }
        if (Gv.Ge(seg.C("size"), Gv.I(2L))) {
          Self.C("draw_polyline", seg, col, Gv.F(2.0), Gv.B(true));
        }
        if (Gv.Ge(seg.C("size"), Gv.I(1L))) {
          Self.C("draw_circle", seg.I((seg.C("size") - Gv.I(1L))), Gv.F(3.0), col);
        }
      }
      Gv mins = (((t1 - t0)) / Gv.F(60.0));
      Self.C("draw_string", f, Gd.Ctor("Vector2", plot.G("position").G("x"), (plot.G("end").G("y") + Gv.I(13L))), (Gv.S("před %d min") % Gd.roundi(mins)), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(10L), Gd.Ctor("Color", Gv.S("6b757f")));
      Self.C("draw_string", f, Gd.Ctor("Vector2", (plot.G("end").G("x") - Gv.I(24L)), (plot.G("end").G("y") + Gv.I(13L))), Gv.S("teď"), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(10L), Gd.Ctor("Color", Gv.S("6b757f")));
    } else {
      Self.C("draw_string", f, Gd.Ctor("Vector2", (plot.G("position").G("x") + Gv.I(10L)), (plot.G("position").G("y") + (plot.G("size").G("y") * Gv.F(0.5)))), Gv.S("Graf se kreslí, jak běží čas várky…"), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(12L), Gd.Ctor("Color", Gv.S("6b757f")));
    }
    Gv lx = Gv.F(6.0);
    Gv ly = (plot.G("end").G("y") + Gv.F(28.0));
    Gv last = ((H.C("size")).T() ? H.I((H.C("size") - Gv.I(1L))) : Gd.Arr());
    foreach (Gv si_2 in (S_ctl.SERIES.C("size")).Iter()) {
      Gv v2 = ((last.C("size")).T() ? last.I((si_2 + Gv.I(1L))) : Gv.Null);
      Gv txt = (S_ctl.SERIES.I(si_2).I(Gv.I(0L)) + ((Gv.Ne(v2, Gv.Null) ? (Gv.S(" %d°") % Gd.roundi(v2)) : Gv.S(""))));
      Gv col2 = Gd.Ctor("Color", S_ctl.SERIES.I(si_2).I(Gv.I(2L)));
      Gv iw = (f.C("get_string_size", txt, Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(10L)).G("x") + Gv.F(20.0));
      if (Gv.Gt((lx + iw), (Self.G("size").G("x") - Gv.F(2.0)))) {
        lx = Gv.F(6.0);
        ly = (ly + Gv.F(13.0));
      }
      Self.C("draw_rect", Gd.Ctor("Rect2", lx, (ly - Gv.I(7L)), Gv.I(8L), Gv.I(3L)), col2);
      Self.C("draw_string", f, Gd.Ctor("Vector2", (lx + Gv.I(11L)), ly), txt, Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(10L), (Gv.Ne(v2, Gv.Null) ? Gd.Ctor("Color", Gv.S("c9d1d8")) : Gd.Ctor("Color", Gv.S("5b646c"))));
      lx = (lx + (f.C("get_string_size", txt, Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(10L)).G("x") + Gv.F(20.0)));
    }
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "ctl": return this.ctl;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "ctl": this.ctl = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "_draw": return _draw();
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_ctl_Trend(); return Gv.O(o); }
  public override string ScriptName { get { return "S_ctl_Trend"; } }
}
public partial class S_ctl : GdNode {
  public static Gv Art;
  public Gv m;
  public Gv S;
  public Gv refs;
  public Gv log_box;
  public Gv state_lbl;
  public Gv check_lbl;
  public Gv t_acc;
  public Gv last_log;
  public Gv sv;
  public Gv trend;
  public Gv hist;
  public static Gv HIST_DT;
  public static Gv HIST_N;
  public static Gv SERIES;
  public Gv changed;
  public Gv pend;
  public static Gv YEL;
  public static Gv DIM;
  public static Gv COLLAPSED0;
  public static Gv PANEL_CFG;
  public Gv collapsed;
  public Gv sections;
  static S_ctl() {
    Art = Gd.Script("art");
    HIST_DT = Gv.F(30.0);
    HIST_N = Gv.I(360L);
    SERIES = Gd.Arr(Gd.Arr(Gv.S("Pánev"), Gv.S("rvp"), Gv.S("ff9a3c")), Gd.Arr(Gv.S("SK"), Gv.S("sk"), Gv.S("f2c230")), Gd.Arr(Gv.S("Bojler"), Gv.S("hlt"), Gv.S("ff5a4a")), Gd.Arr(Gv.S("Vířivka"), Gv.S("wp"), Gv.S("4fd1c5")), Gd.Arr(Gv.S("Spilka"), Gv.S("spilka"), Gv.S("5aa9ff")));
    YEL = Gd.Ctor("Color", Gv.S("f2c230"));
    DIM = Gd.Ctor("Color", Gv.S("9aa3ab"));
    COLLAPSED0 = Gd.Arr(Gv.S("JODOVÁ ZKOUŠKA"), Gv.S("CESTY (nápověda)"), Gv.S("LEDOVKA"), Gv.S("RECEPT"), Gv.S("VODA A pH RMUTU"), Gv.S("SPILKA – KVAŠENÍ"), Gv.S("CIP SANITACE"), Gv.S("NÁCVIK PORUCH A VÝSTRAHY"), Gv.S("HODNOCENÍ VÁRKY"));
    PANEL_CFG = Gv.S("user://panel.json");
  }
  public S_ctl() {
    m = Gv.Null;
    S = Gv.Null;
    refs = Gd.Dict();
    log_box = Gv.Null;
    state_lbl = Gv.Null;
    check_lbl = Gv.Null;
    t_acc = Gv.F(0.0);
    last_log = Gv.F(-1.0);
    sv = Gv.Null;
    trend = Gv.Null;
    hist = Gd.Arr();
    changed = Gv.B(false);
    pend = Gv.I(1L);
    collapsed = Gd.Dict();
    sections = Gd.Dict();
  }
  public Gv _ready() {
    S = m.G("sim").G("S");
    Gv th = Gd.New("Theme");
    th = th.S("default_font", m.G("font_b"));
    th = th.S("default_font_size", Gv.I(14L));
    Gv sb = Gd.New("StyleBoxFlat");
    sb = sb.S("bg_color", Gd.Ctor("Color", Gv.S("262d35")));
    sb.C("set_corner_radius_all", Gv.I(6L));
    sb = sb.S("border_color", Gd.Ctor("Color", Gv.S("3a444e")));
    sb.C("set_border_width_all", Gv.I(1L));
    sb = sb.S("content_margin_left", Gv.I(9L));
    sb = sb.S("content_margin_right", Gv.I(9L));
    sb = sb.S("content_margin_top", Gv.I(4L));
    sb = sb.S("content_margin_bottom", Gv.I(4L));
    Gv sh = sb.C("duplicate");
    sh = sh.S("bg_color", Gd.Ctor("Color", Gv.S("333c46")));
    sh = sh.S("border_color", YEL);
    Gv sp = sb.C("duplicate");
    sp = sp.S("bg_color", YEL);
    th.C("set_stylebox", Gv.S("normal"), Gv.S("Button"), sb);
    th.C("set_stylebox", Gv.S("hover"), Gv.S("Button"), sh);
    th.C("set_stylebox", Gv.S("pressed"), Gv.S("Button"), sp);
    th.C("set_stylebox", Gv.S("hover_pressed"), Gv.S("Button"), sp);
    th.C("set_stylebox", Gv.S("focus"), Gv.S("Button"), Gd.New("StyleBoxEmpty"));
    th.C("set_color", Gv.S("font_color"), Gv.S("Button"), Gd.Ctor("Color", Gv.S("e6ebef")));
    th.C("set_color", Gv.S("font_hover_color"), Gv.S("Button"), Gd.StaticGet("Color", "WHITE"));
    th.C("set_color", Gv.S("font_pressed_color"), Gv.S("Button"), Gd.Ctor("Color", Gv.S("1b2026")));
    th.C("set_color", Gv.S("font_hover_pressed_color"), Gv.S("Button"), Gd.Ctor("Color", Gv.S("1b2026")));
    th.C("set_color", Gv.S("font_color"), Gv.S("Label"), Gd.Ctor("Color", Gv.S("d5dbe0")));
    Gv scb = Gd.New("StyleBoxFlat");
    scb = scb.S("bg_color", Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.I(0L)));
    scb = scb.S("content_margin_left", Gv.I(6L));
    scb = scb.S("content_margin_right", Gv.I(0L));
    Gv grb = Gd.New("StyleBoxFlat");
    grb = grb.S("bg_color", Gd.Ctor("Color", Gv.S("36404a")));
    grb.C("set_corner_radius_all", Gv.I(4L));
    Gv grh = grb.C("duplicate");
    grh = grh.S("bg_color", Gd.Ctor("Color", Gv.S("4a5662")));
    Gv grp = grb.C("duplicate");
    grp = grp.S("bg_color", YEL);
    th.C("set_stylebox", Gv.S("scroll"), Gv.S("VScrollBar"), scb);
    th.C("set_stylebox", Gv.S("grabber"), Gv.S("VScrollBar"), grb);
    th.C("set_stylebox", Gv.S("grabber_highlight"), Gv.S("VScrollBar"), grh);
    th.C("set_stylebox", Gv.S("grabber_pressed"), Gv.S("VScrollBar"), grp);
    Gv panel = Gd.New("PanelContainer");
    panel = panel.S("theme", th);
    Gv psb = Gd.New("StyleBoxFlat");
    psb = psb.S("bg_color", Gd.Ctor("Color", Gv.S("0e1216")));
    psb = psb.S("border_color", Gd.Ctor("Color", Gv.S("2a323a")));
    psb = psb.S("border_width_left", Gv.I(1L));
    psb = psb.S("content_margin_left", Gv.I(10L));
    psb = psb.S("content_margin_right", Gv.I(8L));
    psb = psb.S("content_margin_top", Gv.I(10L));
    psb = psb.S("content_margin_bottom", Gv.I(10L));
    panel.C("add_theme_stylebox_override", Gv.S("panel"), psb);
    Gv svc = Gd.New("SubViewportContainer");
    svc = svc.S("stretch", Gv.B(true));
    svc = svc.S("anchor_left", Gv.F(1.0));
    svc = svc.S("anchor_right", Gv.F(1.0));
    svc = svc.S("anchor_bottom", Gv.F(1.0));
    svc = svc.S("offset_left", (-m.G("PANEL_W")));
    svc = svc.S("offset_top", Gv.I(52L));
    svc = svc.S("offset_right", Gv.I(0L));
    svc = svc.S("offset_bottom", Gv.I(0L));
    Self.C("add_child", svc);
    sv = Gd.New("SubViewport");
    sv = sv.S("transparent_bg", Gv.B(false));
    sv = sv.S("render_target_update_mode", Gd.StaticGet("SubViewport", "UPDATE_ONCE"));
    svc.C("add_child", sv);
    panel.C("set_anchors_preset", Gd.StaticGet("Control", "PRESET_FULL_RECT"));
    sv.C("add_child", panel);
    svc.G("gui_input").C("connect", Gd.Fn((_a) => { Gv _e = Gd.A(_a, 0); dirty(); return Gv.Null; }));
    svc.G("mouse_exited").C("connect", Gd.Fn((_a) => dirty()));
    svc.G("resized").C("connect", Gd.Fn((_a) => dirty()));
    Gv sc = Gd.New("ScrollContainer");
    sc = sc.S("horizontal_scroll_mode", Gd.StaticGet("ScrollContainer", "SCROLL_MODE_DISABLED"));
    panel.C("add_child", sc);
    Gv box = Gd.New("VBoxContainer");
    box = box.S("size_flags_horizontal", Gd.StaticGet("Control", "SIZE_EXPAND_FILL"));
    box.C("add_theme_constant_override", Gv.S("separation"), Gv.I(5L));
    sc.C("add_child", box);
    _head(box, Gv.S("ČAS"));
    Gv r = _row(box);
    refs = refs.S("play", _btn(r, Gv.S("Spustit"), Gd.Fn((_a) => {  m = m.S("running", Gv.B(!(m.G("running")).T())); refresh(); return Gv.Null; })));
    foreach (Gv s in (Gd.Arr(Gv.I(1L), Gv.I(10L), Gv.I(60L), Gv.I(180L))).Iter()) {
      Gv b = _btn(r, (Gv.S("x%d") % s), Gd.Fn((_a) => {  m = m.S("speed", Gd.Dyn("float", s)); refresh(); return Gv.Null; }));
      b = b.S("toggle_mode", Gv.B(true));
      refs = refs.SI((Gv.S("sp%d") % s), b);
    }
    refs = refs.S("time", _lbl(box, Gv.S("")));
    r = _row(box);
    _btn(r, Gv.S("Uložit"), Gd.Fn((_a) => {  m.C("save_game"); m.G("hud").C("toast", Gv.S("Várka uložena (ukládá se i automaticky a při zavření)")); return Gv.Null; }));
    Gv nb = _btn(r, Gv.S("Nová várka"), Gd.Ctor("Callable"));
    nb.G("pressed").C("disconnect", nb.G("pressed").C("get_connections").I(Gv.I(0L)).G("callable"));
    nb.G("pressed").C("connect", m.G("new_game"));
    refs = refs.S("hall", _btn(r, Gv.S(""), Gd.Fn((_a) => {  m.C("set_hall", Gv.B(!(m.G("hall_dark")).T())); refresh(); return Gv.Null; })));
    _head(box, Gv.S("TREND TEPLOT"));
    trend = S_ctl_Trend.New();
    trend = trend.S("ctl", Self);
    trend = trend.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(0L), Gv.I(160L)));
    box.C("add_child", trend);
    _head(box, Gv.S("AUTOMATICKÁ VÁRKA"));
    r = _row(box);
    refs = refs.S("auto", _btn(r, Gv.S(""), Gd.Fn((_a) => _toggle_auto())));
    refs = refs.S("autost", _lbl(box, Gv.S("")));
    refs = refs.S("autost", refs.G("autost").S("autowrap_mode", Gd.StaticGet("TextServer", "AUTOWRAP_WORD_SMART")));
    refs = refs.S("autost", refs.G("autost").S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(280L), Gv.I(0L))));
    _head(box, Gv.S("ROZVADĚČ"));
    r = _row(box);
    refs = refs.S("cmain", _btn(r, Gv.S(""), Gd.Fn((_a) => {  S = S.S("cab", S.G("cab").S("main", Gv.B(!(S.G("cab").G("main")).T()))); refresh(); return Gv.Null; })));
    refs = refs.S("ckey", _btn(r, Gv.S(""), Gd.Fn((_a) => {  S = S.S("cab", S.G("cab").S("key", Gv.B(!(S.G("cab").G("key")).T()))); refresh(); return Gv.Null; })));
    refs = refs.S("cest", _btn(r, Gv.S(""), Gd.Fn((_a) => {  S = S.S("cab", S.G("cab").S("estop", Gv.B(!(S.G("cab").G("estop")).T()))); refresh(); return Gv.Null; })));
    r = _row(box);
    refs = refs.S("cint", _btn(r, Gv.S(""), Gd.Fn((_a) => {  S = S.S("cab", S.G("cab").S("interlock", Gv.B(!(S.G("cab").C("get", Gv.S("interlock"), Gv.B(true))).T()))); m.G("sim").C("logm", (Gv.S("Ochrany čerpadel (interlock) %s.") % (((S.G("cab").G("interlock")).T() ? Gv.S("zapnuty – čerpadlo nepůjde pustit do zavřené / prázdné trasy") : Gv.S("VYPNUTY – chyba obsluhy bude mít skutečný následek (zavzdušnění, tlaková špička)")))), ((S.G("cab").G("interlock")).T() ? Gv.S("info") : Gv.S("warn"))); refresh(); return Gv.Null; })));
    _head(box, Gv.S("JODOVÁ ZKOUŠKA"));
    r = _row(box);
    _btn(r, Gv.S("Dílo v pánvi"), Gd.Fn((_a) => {  m.C("open_iodine", Gv.S("rvp")); return Gv.Null; }));
    _btn(r, Gv.S("Rmut / sladina v SK"), Gd.Fn((_a) => {  m.C("open_iodine", Gv.S("sk")); return Gv.Null; }));
    _lbl(box, Gv.S("Barva ukáže, jak moc se škrob zcukřil."));
    _head(box, Gv.S("KOTEL A PÁRA"));
    r = _row(box);
    refs = refs.S("boiler", _btn(r, Gv.S(""), Gd.Fn((_a) => {  S = S.S("boiler", S.G("boiler").S("on", Gv.B(!(S.G("boiler").G("on")).T()))); refresh(); return Gv.Null; })));
    _head(box, Gv.S("PÁNEV (RMUTOMLADINOVÁ)"));
    r = _row(box);
    _lbl(r, Gv.S("Pára do pláště:"));
    foreach (Gv p in (Gd.Arr(Gv.I(0L), Gv.I(25L), Gv.I(50L), Gv.I(75L), Gv.I(100L))).Iter()) {
      Gv b2 = _btn(r, (Gv.S("%d") % p), Gd.Fn((_a) => {  S = S.S("rvp", S.G("rvp").S("heat", (p / Gv.F(100.0)))); refresh(); return Gv.Null; }));
      b2 = b2.S("toggle_mode", Gv.B(true));
      refs = refs.SI((Gv.S("h%d") % p), b2);
    }
    r = _row(box);
    refs = refs.S("stir", _btn(r, Gv.S(""), Gd.Fn((_a) => {  S = S.S("rvp", S.G("rvp").S("stir", Gv.B(!(S.G("rvp").G("stir")).T()))); refresh(); return Gv.Null; })));
    r = _row(box);
    _lbl(r, Gv.S("Otáčky míchadla:"));
    _btn(r, Gv.S("-"), Gd.Fn((_a) => {  S = S.S("rvp", S.G("rvp").S("hz", Gd.maxf(Gv.F(10.0), (S.G("rvp").G("hz") - Gv.F(5.0))))); refresh(); return Gv.Null; }));
    refs = refs.S("hz", _lbl(r, Gv.S("")));
    _btn(r, Gv.S("+"), Gd.Fn((_a) => {  S = S.S("rvp", S.G("rvp").S("hz", Gd.minf(Gv.F(50.0), (S.G("rvp").G("hz") + Gv.F(5.0))))); refresh(); return Gv.Null; }));
    r = _row(box);
    _lbl(r, Gv.S("Voda z bojleru:"));
    _btn(r, Gv.S("−"), Gd.Fn((_a) => {  S = S.S("rvpWater", S.G("rvpWater").S("target", Gd.maxf(S.G("rvpWater").G("added"), (S.G("rvpWater").G("target") - Gv.F(100.0))))); refresh(); return Gv.Null; }));
    refs = refs.S("wtarget", _lbl(r, Gv.S("")));
    _btn(r, Gv.S("+"), Gd.Fn((_a) => {  S = S.S("rvpWater", S.G("rvpWater").S("target", (S.G("rvpWater").G("target") + Gv.F(100.0)))); refresh(); return Gv.Null; }));
    r = _row(box);
    refs = refs.S("water", _btn(r, Gv.S(""), Gd.Fn((_a) => _water())));
    refs = refs.S("mix", _btn(r, Gv.S(""), Gd.Fn((_a) => {  S = S.S("mixAuto", Gv.B(!(S.G("mixAuto")).T())); refresh(); return Gv.Null; })));
    r = _row(box);
    _lbl(r, Gv.S("Směšovač na:"));
    _btn(r, Gv.S("−"), Gd.Fn((_a) => {  S = S.S("rvpWater", S.G("rvpWater").S("T", Gd.maxf(S.G("cool").G("waterT"), (S.G("rvpWater").G("T") - Gv.F(1.0))))); S = S.S("skWater", S.G("skWater").S("T", S.G("rvpWater").G("T"))); refresh(); return Gv.Null; }));
    refs = refs.S("mixT", _lbl(r, Gv.S("")));
    _btn(r, Gv.S("+"), Gd.Fn((_a) => {  S = S.S("rvpWater", S.G("rvpWater").S("T", Gd.minf(Gv.F(85.0), (S.G("rvpWater").G("T") + Gv.F(1.0))))); S = S.S("skWater", S.G("skWater").S("T", S.G("rvpWater").G("T"))); refresh(); return Gv.Null; }));
    r = _row(box);
    refs = refs.S("malt", _btn(r, Gv.S(""), Gd.Fn((_a) => _malt())));
    r = _row(box);
    refs = refs.S("gate", _btn(r, Gv.S(""), Gd.Fn((_a) => {  S = S.S("malt", S.G("malt").S("gate", Gv.B(!(S.G("malt").G("gate")).T()))); m.C("redraw"); refresh(); return Gv.Null; })));
    r = _row(box);
    refs = refs.S("hatch", _btn(r, Gv.S(""), Gd.Fn((_a) => {  S = S.S("sk", S.G("sk").SI(Gv.S("hatch"), Gv.B(!(S.G("sk").C("get", Gv.S("hatch"), Gv.B(false))).T()))); m.C("redraw"); refresh(); return Gv.Null; })));
    r = _row(box);
    refs = refs.S("hop", _btn(r, Gv.S(""), Gd.Fn((_a) => {  m.G("sim").C("hopPlanAdd"); refresh(); return Gv.Null; })));
    _head(box, Gv.S("SCEZOVACÍ KÁĎ"));
    r = _row(box);
    refs = refs.S("sparge", _btn(r, Gv.S(""), Gd.Fn((_a) => {  S = S.S("skWater", S.G("skWater").S("on", Gv.B(!(S.G("skWater").G("on")).T()))); refresh(); return Gv.Null; })));
    _btn(r, Gv.S("−"), Gd.Fn((_a) => {  S = S.S("skWater", S.G("skWater").S("flow", Gd.maxf(Gv.F(10.0), (S.G("skWater").G("flow") - Gv.F(10.0))))); refresh(); return Gv.Null; }));
    refs = refs.S("sflow", _lbl(r, Gv.S("")));
    _btn(r, Gv.S("+"), Gd.Fn((_a) => {  S = S.S("skWater", S.G("skWater").S("flow", Gd.minf(Gv.F(200.0), (S.G("skWater").G("flow") + Gv.F(10.0))))); refresh(); return Gv.Null; }));
    r = _row(box);
    _lbl(r, Gv.S("pH rmutu:"));
    refs = refs.S("ph", _lbl(r, Gv.S("")));
    r = _row(box);
    refs = refs.S("rake", _btn(r, Gv.S(""), Gd.Fn((_a) => {  S = S.S("sk", S.G("sk").S("rake", Gv.B(!(S.G("sk").G("rake")).T()))); refresh(); return Gv.Null; })));
    _btn(r, Gv.S("Zvednout"), Gd.Fn((_a) => {  S = S.S("sk", S.G("sk").S("hydT", Gv.F(0.0))); refresh(); return Gv.Null; }));
    _btn(r, Gv.S("Střed"), Gd.Fn((_a) => {  S = S.S("sk", S.G("sk").S("hydT", Gv.F(0.4))); refresh(); return Gv.Null; }));
    _btn(r, Gv.S("U síta"), Gd.Fn((_a) => {  S = S.S("sk", S.G("sk").S("hydT", Gv.F(1.0))); refresh(); return Gv.Null; }));
    refs = refs.S("lauter", _btn(box, Gv.S(""), Gd.Fn((_a) => {  m.G("sim").C("start_lautering"); refresh(); return Gv.Null; })));
    refs = refs.S("lauter_auto", _btn(box, Gv.S(""), Gd.Fn((_a) => {  if ((m.G("sim").G("lauter_auto").C("state").G("on")).T()) {   m.G("sim").G("lauter_auto").C("stop", Gv.S("ruční převzetí")); } else {   m.G("sim").G("lauter_auto").C("start"); } refresh(); return Gv.Null; })));
    _lbl(box, Gv.S("Automat scezování přebírá kapalinné klapky varny."));
    _btn(box, Gv.S("Zvětšit průhledítko C2"), Gd.Fn((_a) => {  m.C("open_sight_c2"); return Gv.Null; }));
    r = _row(box);
    _btn(r, Gv.S("Hluboké kypření"), Gd.Fn((_a) => {  m.G("sim").C("kypr"); refresh(); return Gv.Null; }));
    _btn(r, Gv.S("Vyhrabat mláto"), Gd.Fn((_a) => {  m.G("sim").C("emptySK"); refresh(); return Gv.Null; }));
    _head(box, Gv.S("ČERPADLA"));
    foreach (Gv pr in (Gd.Arr(Gd.Arr(Gv.S("p1"), Gv.F(25.0), Gv.F(50.0), Gv.F(400.0)), Gd.Arr(Gv.S("p2"), Gv.F(10.0), Gv.F(10.0), Gv.F(200.0)), Gd.Arr(Gv.S("p3"), Gv.F(10.0), Gv.F(20.0), Gv.F(150.0)))).Iter()) {
      Gv key = pr.I(Gv.I(0L));
      Gv stp = (pr.I(Gv.I(1L))).AsF();
      Gv lo = (pr.I(Gv.I(2L))).AsF();
      Gv hi = (pr.I(Gv.I(3L))).AsF();
      r = _row(box);
      refs = refs.SI(key, _btn(r, Gv.S(""), Gd.Fn((_a) => {  m.C("toggle_pump", key); refresh(); return Gv.Null; })));
      _btn(r, Gv.S("−"), Gd.Fn((_a) => {  S = S.SI(key, S.I(key).S("flow", Gd.maxf(lo, (S.I(key).G("flow") - stp)))); refresh(); return Gv.Null; }));
      refs = refs.SI((key + Gv.S("f")), _lbl(r, Gv.S("")));
      _btn(r, Gv.S("+"), Gd.Fn((_a) => {  S = S.SI(key, S.I(key).S("flow", Gd.minf(hi, (S.I(key).G("flow") + stp)))); refresh(); return Gv.Null; }));
      refs = refs.SI((key + Gv.S("s")), _lbl(box, Gv.S("")));
      refs.I((key + Gv.S("s"))).C("add_theme_color_override", Gv.S("font_color"), DIM);
    }
    r = _row(box);
    refs = refs.S("plv", _btn(r, Gv.S(""), Gd.Fn((_a) => {  m.C("toggle_pump", Gv.S("plv")); refresh(); return Gv.Null; })));
    _head(box, Gv.S("CESTY (nápověda)"));
    foreach (Gv rt in (Gd.Arr(Gd.Arr(Gv.S("Voda z bojleru → pánev"), Gv.S("hlt"), Gv.S("rvp"), Gd.Arr(Gv.S("phw")), Gv.I(-1L)), Gd.Arr(Gv.S("Pánev → SK (C1)"), Gv.S("rvp"), Gv.S("sk"), Gd.Arr(Gv.S("p1")), Gv.I(-1L)), Gd.Arr(Gv.S("SK → pánev (C1, rmut)"), Gv.S("sk"), Gv.S("rvp"), Gd.Arr(Gv.S("p1")), Gv.I(-1L)), Gd.Arr(Gv.S("SK → SK (C2, vorlauf)"), Gv.S("sk"), Gv.S("sk"), Gd.Arr(Gv.S("p2")), Gv.I(0L)), Gd.Arr(Gv.S("SK → pánev (C2, sladina)"), Gv.S("sk"), Gv.S("rvp"), Gd.Arr(Gv.S("p2")), Gv.I(-1L)), Gd.Arr(Gv.S("Zpod sít → C2 → pánev"), Gv.S("sk"), Gv.S("rvp"), Gd.Arr(Gv.S("p2")), Gv.I(-1L), Gd.Arr(Gv.S("valve_murah5ei"), Gv.S("valve_muraglp3"))), Gd.Arr(Gv.S("Pánev → vířivka (C1)"), Gv.S("rvp"), Gv.S("wp"), Gd.Arr(Gv.S("p1")), Gv.I(-1L)), Gd.Arr(Gv.S("Vířivka → spilka (C3)"), Gv.S("wp"), Gv.S("spilka"), Gd.Arr(Gv.S("p3")), Gv.I(-1L)), Gd.Arr(Gv.S("Horká voda → SK (sprchy)"), Gv.S("hlt"), Gv.S("sk"), Gd.Arr(Gv.S("phw")), Gv.I(0L)), Gd.Arr(Gv.S("Bojler → podsítí SK"), Gv.S("hlt"), Gv.S("sk"), Gd.Arr(Gv.S("phw")), Gv.I(1L)), Gd.Arr(Gv.S("Voda z řadu → ledovka"), Gv.S("vodovod"), Gv.S("lv"), Gd.Arr(), Gv.I(-1L)), Gd.Arr(Gv.S("Voda z řadu → bojler"), Gv.S("vodovod"), Gv.S("hlt"), Gd.Arr(), Gv.I(-1L)))).Iter()) {
      r = _row(box);
      Gv lb = _lbl(r, rt.I(Gv.I(0L)));
      lb = lb.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(150L), Gv.I(0L)));
      lb = lb.S("clip_text", Gv.B(true));
      Gv a = rt;
      _btn(r, Gv.S("Ukázat"), Gd.Fn((_a) => {  m.C("show_route", a.I(Gv.I(1L)), a.I(Gv.I(2L)), a.I(Gv.I(3L)), Gv.B(false), a.I(Gv.I(4L)), (Gv.Gt(a.C("size"), Gv.I(5L)) ? a.I(Gv.I(5L)) : Gd.Arr())); return Gv.Null; }));
      _btn(r, Gv.S("Otevřít"), Gd.Fn((_a) => {  m.C("show_route", a.I(Gv.I(1L)), a.I(Gv.I(2L)), a.I(Gv.I(3L)), Gv.B(true), a.I(Gv.I(4L)), (Gv.Gt(a.C("size"), Gv.I(5L)) ? a.I(Gv.I(5L)) : Gd.Arr())); return Gv.Null; }));
    }
    _head(box, Gv.S("LEDOVKA"));
    r = _row(box);
    refs = refs.S("chill", _btn(r, Gv.S(""), Gd.Fn((_a) => {  m.G("sim").C("lvChillSet", Gv.B(!(S.G("lv").G("chill")).T())); refresh(); return Gv.Null; })));
    _head(box, Gv.S("RECEPT"));
    r = _row(box);
    refs = refs.S("recipe", _btn(r, Gv.S(""), Gd.Fn((_a) => {  Gv n = ((((m.G("sim").G("recipe_idx") + Gv.I(1L))) % m.G("sim").G("RECIPES").C("size"))).AsI(); m = m.S("sim", m.G("sim").S("recipe_idx", n)); Gv f = Gd.Static("FileAccess", "open", Gv.S("user://recept.txt"), Gd.StaticGet("FileAccess", "WRITE")); f.C("store_string", Gd.Dyn("str", n)); f.C("close"); m.G("hud").C("toast", (Gv.S("Recept „%s“ se použije po stisku Nová várka.") % m.G("sim").G("RECIPES").I(n).G("name"))); refresh(); return Gv.Null; })));
    refs = refs.S("recipe_info", _lbl(box, Gv.S("")));
    refs = refs.S("recipe_info", refs.G("recipe_info").S("autowrap_mode", Gd.StaticGet("TextServer", "AUTOWRAP_WORD_SMART")));
    refs = refs.S("recipe_info", refs.G("recipe_info").S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(280L), Gv.I(0L))));
    _head(box, Gv.S("VODA A pH RMUTU"));
    r = _row(box);
    refs = refs.S("water_t", _btn(r, Gv.S(""), Gd.Fn((_a) => {  Gv W = m.G("sim").G("proc").G("WATERS"); Gv i = Gv.I(0L); foreach (Gv k in (W.C("size")).Iter()) {   if (Gv.Eq(W.I(k).G("name"), S.G("water").C("get", Gv.S("name"), Gv.S("")))) {     i = k;   } } S = S.S("water", W.I((((i + Gv.I(1L))) % W.C("size"))).C("duplicate")); refresh(); return Gv.Null; })));
    r = _row(box);
    _lbl(r, Gv.S("Kyselina mléčná 80 %:"));
    _btn(r, Gv.S("−"), Gd.Fn((_a) => {  S = S.S("lactic", Gd.maxf(Gv.F(0.0), (S.G("lactic") - Gv.F(250.0)))); refresh(); return Gv.Null; }));
    refs = refs.S("lactic", _lbl(r, Gv.S("")));
    _btn(r, Gv.S("+"), Gd.Fn((_a) => {  S = S.S("lactic", Gd.minf(Gv.F(5000.0), (S.G("lactic") + Gv.F(250.0)))); refresh(); return Gv.Null; }));
    refs = refs.S("ph_info", _lbl(box, Gv.S("")));
    refs = refs.S("ph_info", refs.G("ph_info").S("autowrap_mode", Gd.StaticGet("TextServer", "AUTOWRAP_WORD_SMART")));
    refs = refs.S("ph_info", refs.G("ph_info").S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(280L), Gv.I(0L))));
    _head(box, Gv.S("SPILKA – KVAŠENÍ"));
    r = _row(box);
    refs = refs.S("fer", _btn(r, Gv.S(""), Gd.Fn((_a) => {  if ((S.G("fer").G("on")).T()) {   S = S.S("fer", S.G("fer").S("on", Gv.B(false)));   m.G("sim").C("logm", Gv.S("Kvašení ukončeno – pivo jde na ležení.")); } else {   m.G("sim").G("proc").C("fer_start"); } refresh(); return Gv.Null; })));
    _btn(r, Gv.S("Přeskočit 12 h"), Gd.Fn((_a) => {  if ((S.G("fer").G("on")).T()) {   m.G("sim").G("proc").C("fer_skip", Gv.F(12.0)); } refresh(); return Gv.Null; }));
    r = _row(box);
    _lbl(r, Gv.S("Teplota spilky:"));
    _btn(r, Gv.S("−"), Gd.Fn((_a) => {  S = S.S("fer", S.G("fer").S("set", Gd.maxf(Gv.F(4.0), (S.G("fer").G("set") - Gv.F(0.5))))); refresh(); return Gv.Null; }));
    refs = refs.S("fer_set", _lbl(r, Gv.S("")));
    _btn(r, Gv.S("+"), Gd.Fn((_a) => {  S = S.S("fer", S.G("fer").S("set", Gd.minf(Gv.F(16.0), (S.G("fer").G("set") + Gv.F(0.5))))); refresh(); return Gv.Null; }));
    refs = refs.S("fer_info", _lbl(box, Gv.S("")));
    refs = refs.S("fer_info", refs.G("fer_info").S("autowrap_mode", Gd.StaticGet("TextServer", "AUTOWRAP_WORD_SMART")));
    refs = refs.S("fer_info", refs.G("fer_info").S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(280L), Gv.I(0L))));
    _head(box, Gv.S("CIP SANITACE"));
    r = _row(box);
    refs = refs.S("cip_t", _btn(r, Gv.S(""), Gd.Fn((_a) => {  Gv T = m.G("sim").G("proc").G("CIP_TARGETS"); Gv i_2 = Gv.I(0L); foreach (Gv k_2 in (T.C("size")).Iter()) {   if (Gv.Eq(T.I(k_2).I(Gv.I(0L)), S.G("cip").G("target"))) {     i_2 = k_2;   } } S = S.S("cip", S.G("cip").S("target", T.I((((i_2 + Gv.I(1L))) % T.C("size"))).I(Gv.I(0L)))); refresh(); return Gv.Null; })));
    refs = refs.S("cip_m", _btn(r, Gv.S(""), Gd.Fn((_a) => {  Gv M = m.G("sim").G("proc").G("CIP_MEDIA"); Gv i_3 = Gv.I(0L); foreach (Gv k_3 in (M.C("size")).Iter()) {   if (Gv.Eq(M.I(k_3).I(Gv.I(0L)), S.G("cip").G("medium"))) {     i_3 = k_3;   } } S = S.S("cip", S.G("cip").S("medium", M.I((((i_3 + Gv.I(1L))) % M.C("size"))).I(Gv.I(0L)))); S = S.S("cip", S.G("cip").S("min", Gv.F(0.0))); S = S.S("cip", S.G("cip").S("set", Gd.Dict(Gv.S("voda"), Gv.F(50.0), Gv.S("louh"), Gv.F(80.0), Gv.S("kyselina"), Gv.F(50.0)).I(S.G("cip").G("medium")))); S = S.S("cip", S.G("cip").S("conc", Gd.Dict(Gv.S("voda"), Gv.F(0.0), Gv.S("louh"), Gv.F(1.5), Gv.S("kyselina"), Gv.F(1.0)).I(S.G("cip").G("medium")))); refresh(); return Gv.Null; })));
    r = _row(box);
    refs = refs.S("cip_run", _btn(r, Gv.S(""), Gd.Fn((_a) => {  S = S.S("cip", S.G("cip").S("run", Gv.B(!(S.G("cip").G("run")).T()))); S = S.S("cip", S.G("cip").S("min", Gv.F(0.0))); m.G("sim").C("logm", (Gv.S("CIP %s: %s %s.") % Gd.Arr(m.G("sim").G("proc").C("cip_name", S.G("cip").G("target")), S.G("cip").G("medium"), ((S.G("cip").G("run")).T() ? Gv.S("start") : Gv.S("stop"))))); refresh(); return Gv.Null; })));
    _btn(r, Gv.S("−"), Gd.Fn((_a) => {  S = S.S("cip", S.G("cip").S("set", Gd.maxf(Gv.F(20.0), (S.G("cip").G("set") - Gv.F(5.0))))); refresh(); return Gv.Null; }));
    refs = refs.S("cip_set", _lbl(r, Gv.S("")));
    _btn(r, Gv.S("+"), Gd.Fn((_a) => {  S = S.S("cip", S.G("cip").S("set", Gd.minf(Gv.F(95.0), (S.G("cip").G("set") + Gv.F(5.0))))); refresh(); return Gv.Null; }));
    r = _row(box);
    _lbl(r, Gv.S("Koncentrace:"));
    _btn(r, Gv.S("−"), Gd.Fn((_a) => {  S = S.S("cip", S.G("cip").S("conc", Gd.maxf(Gv.F(0.0), Gd.snappedf((S.G("cip").C("get", Gv.S("conc"), Gv.F(1.5)) - Gv.F(0.25)), Gv.F(0.25))))); refresh(); return Gv.Null; }));
    refs = refs.S("cip_conc", _lbl(r, Gv.S("")));
    _btn(r, Gv.S("+"), Gd.Fn((_a) => {  S = S.S("cip", S.G("cip").S("conc", Gd.minf(Gv.F(5.0), Gd.snappedf((S.G("cip").C("get", Gv.S("conc"), Gv.F(1.5)) + Gv.F(0.25)), Gv.F(0.25))))); refresh(); return Gv.Null; }));
    refs = refs.S("cip_info", _lbl(box, Gv.S("")));
    refs = refs.S("cip_info", refs.G("cip_info").S("autowrap_mode", Gd.StaticGet("TextServer", "AUTOWRAP_WORD_SMART")));
    refs = refs.S("cip_info", refs.G("cip_info").S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(280L), Gv.I(0L))));
    _head(box, Gv.S("NÁCVIK PORUCH A VÝSTRAHY"));
    r = _row(box);
    refs = refs.S("faults", _btn(r, Gv.S(""), Gd.Fn((_a) => {  S = S.S("faults", S.G("faults").S("on", Gv.B(!(S.G("faults").G("on")).T()))); S = S.S("faults", S.G("faults").S("next", Gv.F(0.0))); S = S.S("faults", S.G("faults").S("nextMinor", Gv.F(0.0))); m.G("sim").C("logm", (Gv.S("Nácvik poruch %s – náhodné poruchy a provozní výstrahy se objeví během várky.") % (((S.G("faults").G("on")).T() ? Gv.S("zapnut") : Gv.S("vypnut"))))); refresh(); return Gv.Null; })));
    r = _row(box);
    _btn(r, Gv.S("Odvzdušnit čerpadlo"), Gd.Fn((_a) => {  m.G("sim").G("proc").C("fault_fix", Gv.S("air")); refresh(); return Gv.Null; }));
    _btn(r, Gv.S("Reset kotle"), Gd.Fn((_a) => {  m.G("sim").G("proc").C("fault_fix", Gv.S("steam")); refresh(); return Gv.Null; }));
    r = _row(box);
    _btn(r, Gv.S("Vyčistit chladič"), Gd.Fn((_a) => {  m.G("sim").G("proc").C("fault_fix", Gv.S("plate")); refresh(); return Gv.Null; }));
    _btn(r, Gv.S("Uvolnit klapku"), Gd.Fn((_a) => {  m.G("sim").G("proc").C("fault_fix", Gv.S("valve")); refresh(); return Gv.Null; }));
    r = _row(box);
    _btn(r, Gv.S("Kalibrace teploměru pánve"), Gd.Fn((_a) => {  m.G("sim").G("proc").C("fault_fix", Gv.S("sensor")); refresh(); return Gv.Null; }));
    refs = refs.S("faults_info", _lbl(box, Gv.S("")));
    _head(box, Gv.S("HODNOCENÍ VÁRKY"));
    refs = refs.S("score", Gd.New("RichTextLabel"));
    refs = refs.S("score", refs.G("score").S("bbcode_enabled", Gv.B(true)));
    refs = refs.S("score", refs.G("score").S("fit_content", Gv.B(true)));
    refs = refs.S("score", refs.G("score").S("scroll_active", Gv.B(false)));
    refs.G("score").C("add_theme_font_override", Gv.S("normal_font"), m.G("font"));
    refs.G("score").C("add_theme_font_override", Gv.S("bold_font"), m.G("font_b"));
    refs.G("score").C("add_theme_font_size_override", Gv.S("normal_font_size"), Gv.I(13L));
    refs.G("score").C("add_theme_font_size_override", Gv.S("bold_font_size"), Gv.I(13L));
    box.C("add_child", refs.G("score"));
    _head(box, Gv.S("POSTUP VÁRKY (dvourmutový)"));
    r = _row(box);
    refs = refs.S("next_step", _btn(r, Gv.S("Potvrdit krok a pokračovat"), Gd.Fn((_a) => {  m.G("sim").G("training").C("confirm"); refresh(); return Gv.Null; })));
    _btn(r, Gv.S("Proč?"), Gd.Fn((_a) => {  m.G("sim").G("training").C("state").S("why", Gv.B(!(m.G("sim").G("training").C("state").C("get", Gv.S("why"), Gv.B(false))).T())); refresh(); return Gv.Null; }));
    refs = refs.S("step_pause", _btn(r, Gv.S(""), Gd.Fn((_a) => {  m.G("sim").G("training").C("state").S("pause", Gv.B(!(m.G("sim").G("training").C("state").G("pause")).T())); refresh(); return Gv.Null; })));
    check_lbl = Gd.New("RichTextLabel");
    check_lbl = check_lbl.S("bbcode_enabled", Gv.B(true));
    check_lbl = check_lbl.S("fit_content", Gv.B(true));
    check_lbl = check_lbl.S("scroll_active", Gv.B(false));
    check_lbl.C("add_theme_font_override", Gv.S("normal_font"), m.G("font"));
    check_lbl.C("add_theme_font_size_override", Gv.S("normal_font_size"), Gv.I(13L));
    box.C("add_child", check_lbl);
    _head(box, Gv.S("STAV"));
    state_lbl = Gd.New("RichTextLabel");
    state_lbl = state_lbl.S("bbcode_enabled", Gv.B(true));
    state_lbl = state_lbl.S("fit_content", Gv.B(true));
    state_lbl = state_lbl.S("scroll_active", Gv.B(false));
    state_lbl.C("add_theme_font_override", Gv.S("normal_font"), m.G("font"));
    state_lbl.C("add_theme_font_override", Gv.S("bold_font"), m.G("font_b"));
    state_lbl.C("add_theme_font_size_override", Gv.S("normal_font_size"), Gv.I(13L));
    state_lbl.C("add_theme_font_size_override", Gv.S("bold_font_size"), Gv.I(13L));
    box.C("add_child", state_lbl);
    _head(box, Gv.S("HLÁŠENÍ"));
    log_box = Gd.New("RichTextLabel");
    log_box = log_box.S("bbcode_enabled", Gv.B(true));
    log_box = log_box.S("fit_content", Gv.B(true));
    log_box = log_box.S("scroll_active", Gv.B(false));
    log_box.C("add_theme_font_override", Gv.S("normal_font"), m.G("font"));
    log_box.C("add_theme_font_size_override", Gv.S("normal_font_size"), Gv.I(13L));
    box.C("add_child", log_box);
    _cardify(box);
    refresh();
    foreach (Gv a_2 in (Gd.Static("OS", "get_cmdline_user_args")).Iter()) {
      if ((a_2.C("begins_with", Gv.S("--scroll="))).T()) {
        _ = Self.C("get_tree").G("process_frame");
        _ = Self.C("get_tree").G("process_frame");
        sc = sc.S("scroll_vertical", Gd.Dyn("int", a_2.C("substr", Gv.I(9L))));
        dirty();
      }
    }
    return Gv.Null;
  }
  public Gv dirty() {
    pend = Gv.I(2L);
    return Gv.Null;
  }
  public Gv _head(Gv box = default(Gv), Gv t = default(Gv)) {
    Gv l = Gd.New("Label");
    l = l.S("text", t);
    l.C("add_theme_color_override", Gv.S("font_color"), YEL);
    l.C("add_theme_font_size_override", Gv.S("font_size"), Gv.I(13L));
    l.C("set_meta", Gv.S("head"), Gv.B(true));
    box.C("add_child", l);
    return Gv.Null;
  }
  public Gv _load_collapsed() {
    collapsed.C("clear");
    Gv f = Gd.Static("FileAccess", "open", PANEL_CFG, Gd.StaticGet("FileAccess", "READ"));
    Gv d = (Gv.Ne(f, Gv.Null) ? Gd.Static("JSON", "parse_string", f.C("get_as_text")) : Gv.Null);
    if (Gv.Eq(Gd.Dyn("typeof", d), Gd.K("TYPE_ARRAY"))) {
      foreach (Gv k in (d).Iter()) {
        collapsed = collapsed.SI(Gd.Dyn("str", k), Gv.B(true));
      }
    } else {
      foreach (Gv k_2 in (COLLAPSED0).Iter()) {
        collapsed = collapsed.SI(k_2, Gv.B(true));
      }
    }
    return Gv.Null;
  }
  public Gv _save_collapsed() {
    Gv f = Gd.Static("FileAccess", "open", PANEL_CFG, Gd.StaticGet("FileAccess", "WRITE"));
    if (Gv.Ne(f, Gv.Null)) {
      f.C("store_string", Gd.Static("JSON", "stringify", collapsed.C("keys")));
    }
    return Gv.Null;
  }
  public Gv _cardify(Gv box = default(Gv)) {
    _load_collapsed();
    Gv csb = Gd.New("StyleBoxFlat");
    csb = csb.S("bg_color", Gd.Ctor("Color", Gv.S("161c22")));
    csb.C("set_corner_radius_all", Gv.I(8L));
    csb = csb.S("border_color", Gd.Ctor("Color", Gv.S("262f38")));
    csb.C("set_border_width_all", Gv.I(1L));
    csb = csb.S("content_margin_left", Gv.I(10L));
    csb = csb.S("content_margin_right", Gv.I(10L));
    csb = csb.S("content_margin_top", Gv.I(4L));
    csb = csb.S("content_margin_bottom", Gv.I(8L));
    Gv hsb = Gd.New("StyleBoxFlat");
    hsb = hsb.S("bg_color", Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.I(0L)));
    hsb = hsb.S("content_margin_left", Gv.I(2L));
    hsb = hsb.S("content_margin_right", Gv.I(2L));
    hsb = hsb.S("content_margin_top", Gv.I(4L));
    hsb = hsb.S("content_margin_bottom", Gv.I(4L));
    Gv hsh = hsb.C("duplicate");
    hsh = hsh.S("bg_color", Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.04)));
    hsh.C("set_corner_radius_all", Gv.I(5L));
    Gv kids = box.C("get_children");
    Gv content = Gv.Null;
    foreach (Gv c in (kids).Iter()) {
      if ((c.C("has_meta", Gv.S("head"))).T()) {
        Gv title = c.G("text");
        box.C("remove_child", c);
        c.C("queue_free");
        Gv card = Gd.New("PanelContainer");
        card.C("add_theme_stylebox_override", Gv.S("panel"), csb);
        box.C("add_child", card);
        Gv v = Gd.New("VBoxContainer");
        v.C("add_theme_constant_override", Gv.S("separation"), Gv.I(5L));
        card.C("add_child", v);
        Gv hb = Gd.New("Button");
        hb = hb.S("alignment", Gd.K("HORIZONTAL_ALIGNMENT_LEFT"));
        hb = hb.S("focus_mode", Gd.StaticGet("Control", "FOCUS_NONE"));
        foreach (Gv st in (Gd.Arr(Gv.S("normal"), Gv.S("pressed"), Gv.S("hover_pressed"), Gv.S("disabled"))).Iter()) {
          hb.C("add_theme_stylebox_override", st, hsb);
        }
        hb.C("add_theme_stylebox_override", Gv.S("hover"), hsh);
        foreach (Gv fc in (Gd.Arr(Gv.S("font_color"), Gv.S("font_pressed_color"), Gv.S("font_hover_pressed_color"))).Iter()) {
          hb.C("add_theme_color_override", fc, YEL);
        }
        hb.C("add_theme_color_override", Gv.S("font_hover_color"), Gd.Ctor("Color", Gv.S("ffd95a")));
        hb.C("add_theme_font_size_override", Gv.S("font_size"), Gv.I(13L));
        v.C("add_child", hb);
        content = Gd.New("VBoxContainer");
        content.C("add_theme_constant_override", Gv.S("separation"), Gv.I(5L));
        v.C("add_child", content);
        sections = sections.SI(title, Gd.Arr(hb, content));
        hb.G("pressed").C("connect", Gd.Fn((_a) => _toggle_sec(Gd.A(_a, 0))).C("bind", title));
        _apply_sec(title);
      } else if (Gv.Ne(content, Gv.Null)) {
        box.C("remove_child", c);
        content.C("add_child", c);
      }
    }
    box.C("add_theme_constant_override", Gv.S("separation"), Gv.I(8L));
    _fit_width(box, Gv.F(266.0));
    if (Gv.In(Gv.S("--paneldebug"), Gd.Static("OS", "get_cmdline_user_args"))) {
      _ = Self.C("get_tree").G("process_frame");
      Gd.print((Gv.S("PANEL box min %s, sv %s") % Gd.Arr(box.C("get_combined_minimum_size"), sv.G("size"))));
      _dbg_wide(box, Gv.I(0L));
    }
    return Gv.Null;
  }
  public Gv _dbg_wide(Gv n = default(Gv), Gv d = default(Gv)) {
    d = (d).AsI();
    foreach (Gv c in (n.C("get_children")).Iter()) {
      if ((Gv.Is(c, "Control") && Gv.Gt(c.C("get_combined_minimum_size").G("x"), Gv.F(250.0)))) {
        Gd.print(Gv.S("  ").C("repeat", d), c.C("get_class"), Gv.S(" "), c.C("get_combined_minimum_size"), Gv.S(" "), (Gv.Ne(c.C("get", Gv.S("text")), Gv.Null) ? c.C("get", Gv.S("text")) : Gv.S("")));
        _dbg_wide(c, (d + Gv.I(1L)));
      }
    }
    return Gv.Null;
  }
  public Gv _fit_width(Gv n = default(Gv), Gv maxw = default(Gv)) {
    maxw = (maxw).AsF();
    foreach (Gv c__it in (n.C("get_children")).Iter()) {
      Gv c = c__it;
      if (Gv.Is(c, "Label")) {
        if (!Gv.Is(n, "HFlowContainer")) {
          c = c.S("autowrap_mode", Gd.StaticGet("TextServer", "AUTOWRAP_WORD_SMART"));
          c = c.S("custom_minimum_size", c.G("custom_minimum_size").S("x", Gd.minf(c.G("custom_minimum_size").G("x"), maxw)));
        }
      } else if (Gv.Is(c, "Button")) {
        if (Gv.Gt(c.C("get_combined_minimum_size").G("x"), maxw)) {
          c = c.S("clip_text", Gv.B(true));
          c = c.S("text_overrun_behavior", Gd.StaticGet("TextServer", "OVERRUN_TRIM_ELLIPSIS"));
          c = c.S("custom_minimum_size", c.G("custom_minimum_size").S("x", maxw));
        }
      }
      if ((Gv.Gt(c.C("get_child_count"), Gv.I(0L)) && !Gv.Is(c, "Button"))) {
        _fit_width(c, maxw);
      }
    }
    return Gv.Null;
  }
  public Gv _apply_sec(Gv title = default(Gv)) {
    Gv sct = sections.I(title);
    Gv shut = collapsed.C("has", title);
    sct = sct.SI(Gv.I(0L), sct.I(Gv.I(0L)).S("text", ((((shut).T() ? Gv.S("▸  ") : Gv.S("▾  "))) + title)));
    sct = sct.SI(Gv.I(1L), sct.I(Gv.I(1L)).S("visible", Gv.B(!(shut).T())));
    return Gv.Null;
  }
  public Gv _toggle_sec(Gv title = default(Gv)) {
    if ((collapsed.C("has", title)).T()) {
      collapsed.C("erase", title);
    } else {
      collapsed = collapsed.SI(title, Gv.B(true));
    }
    _apply_sec(title);
    _save_collapsed();
    dirty();
    return Gv.Null;
  }
  public Gv _row(Gv box = default(Gv)) {
    Gv r = Gd.New("HFlowContainer");
    r.C("add_theme_constant_override", Gv.S("h_separation"), Gv.I(4L));
    r.C("add_theme_constant_override", Gv.S("v_separation"), Gv.I(4L));
    box.C("add_child", r);
    return r;
    return Gv.Null;
  }
  public Gv _btn(Gv r = default(Gv), Gv t = default(Gv), Gv f = default(Gv)) {
    Gv b = Gd.New("Button");
    b = b.S("text", t);
    b.G("pressed").C("connect", Gd.Fn((_a) => _on_btn(Gd.A(_a, 0))).C("bind", f));
    r.C("add_child", b);
    return b;
    return Gv.Null;
  }
  public Gv _on_btn(Gv f = default(Gv)) {
    f.C("call");
    m.C("redraw");
    return Gv.Null;
  }
  public Gv _toggle_auto() {
    if ((m.G("auto").G("on")).T()) {
      m.G("auto").C("stop", Gv.S("zastavena obsluhou"));
    } else {
      m.G("auto").C("start");
    }
    refresh();
    return Gv.Null;
  }
  public Gv _water() {
    if ((S.G("rvpWater").G("on")).T()) {
      S = S.S("rvpWater", S.G("rvpWater").S("on", Gv.B(false)));
    } else {
      if (Gv.Ge(S.G("rvpWater").G("added"), (S.G("rvpWater").G("target") - Gv.I(1L)))) {
        S = S.S("rvpWater", S.G("rvpWater").S("target", (S.G("rvpWater").G("added") + S.G("cfg").G("water"))));
      }
      S = S.S("rvpWater", S.G("rvpWater").S("on", Gv.B(true)));
    }
    if ((S.G("rvpWater").G("on")).T()) {
      if (!(m.G("running")).T()) {
        m = m.S("running", Gv.B(true));
        m.C("_say", Gv.S("Čas spuštěn, ať voda teče."));
      }
      Gv vs = m.G("sim").C("route_valves", Gv.S("hlt"), Gv.S("rvp"), Gd.Arr(Gv.S("phw")));
      if (Gv.Eq(vs, Gv.Null)) {
        m.C("_say", Gv.S("Z bojleru do pánve ve schématu cesta není (chybí trubka nebo čerpadlo)."));
      } else {
        Gv zav = (Gv.I(0L)).AsI();
        foreach (Gv id in (vs).Iter()) {
          if (!(m.G("sim").C("valve_open", id)).T()) {
            zav = ((zav + Gv.I(1L))).AsI();
          }
        }
        if (Gv.Gt(zav, Gv.I(0L))) {
          m.C("_say", (Gv.S("Voda z bojleru zatím nepoteče – na cestě je %d zavřených klapek. V panelu Cesty → „Voda z bojleru → pánev“: Ukázat (vidíš které) nebo Otevřít (otevře je).") % zav));
        }
      }
    }
    refresh();
    return Gv.Null;
  }
  public Gv _malt() {
    if ((!(S.G("malt").G("on")).T() && Gv.Ge(S.G("malt").G("added"), (S.G("malt").G("target") - Gv.F(0.01))))) {
      return Gv.Null;
    }
    S = S.S("malt", S.G("malt").S("on", Gv.B(!(S.G("malt").G("on")).T())));
    refresh();
    return Gv.Null;
  }
  public Gv _lbl(Gv r = default(Gv), Gv t = default(Gv)) {
    Gv l = Gd.New("Label");
    l = l.S("text", t);
    r.C("add_child", l);
    return l;
    return Gv.Null;
  }
  public Gv _t(Gv c = default(Gv), Gv s = default(Gv)) {
    if (Gv.Ne(c.G("text"), s)) {
      c = c.S("text", s);
      changed = Gv.B(true);
    }
    return Gv.Null;
  }
  public Gv _p(Gv b = default(Gv), Gv on = default(Gv)) {
    if (Gv.Ne(b.G("button_pressed"), on)) {
      b = b.S("button_pressed", on);
      changed = Gv.B(true);
    }
    return Gv.Null;
  }
  public Gv onoff(Gv b = default(Gv)) {
    return ((b).T() ? Gv.S("ZAP") : Gv.S("vyp"));
    return Gv.Null;
  }
  public Gv c1(Gv x = default(Gv)) {
    x = (x).AsF();
    return ((Gv.S("%.1f") % x)).C("replace", Gv.S("."), Gv.S(","));
    return Gv.Null;
  }
  public Gv _process(Gv delta = default(Gv)) {
    delta = (delta).AsF();
    if (Gv.Gt(pend, Gv.I(0L))) {
      pend = (pend - Gv.I(1L));
      sv = sv.S("render_target_update_mode", Gd.StaticGet("SubViewport", "UPDATE_ONCE"));
    }
    t_acc = (t_acc + delta);
    if (Gv.Gt(t_acc, Gv.F(0.6))) {
      t_acc = Gv.F(0.0);
      refresh();
    }
    return Gv.Null;
  }
  public Gv _sample() {
    Gv t = (S.G("t")).AsF();
    if (((hist.C("size")).T() && Gv.Lt(t, (hist.I((hist.C("size") - Gv.I(1L))).I(Gv.I(0L)) - Gv.F(1.0))))) {
      hist.C("clear");
    }
    if (((hist.C("size")).T() && Gv.Lt((t - hist.I((hist.C("size") - Gv.I(1L))).I(Gv.I(0L))), HIST_DT))) {
      return Gv.Null;
    }
    Gv row = Gd.Arr(t);
    foreach (Gv se in (SERIES).Iter()) {
      Gv d = m.C("vessel_card", se.I(Gv.I(1L)));
      row.C("append", ((((d.C("is_empty")).T() || (d.G("empty")).T()) || Gv.Eq(d.G("T"), Gv.Null)) ? Gv.Null : Gd.Dyn("float", d.G("T"))));
    }
    hist.C("append", row);
    if (Gv.Gt(hist.C("size"), HIST_N)) {
      hist.C("pop_front");
    }
    if (Gv.Ne(trend, Gv.Null)) {
      trend.C("queue_redraw");
    }
    changed = Gv.B(true);
    return Gv.Null;
  }
  public Gv refresh() {
    _sample();
    _t(refs.G("play"), ((m.G("running")).T() ? Gv.S("PAUZA") : Gv.S("SPUSTIT (čas stojí)")));
    _t(refs.G("hall"), ((m.G("hall_dark")).T() ? Gv.S("☀ Světlá hala") : Gv.S("☾ Tmavá hala")));
    foreach (Gv s in (Gd.Arr(Gv.I(1L), Gv.I(10L), Gv.I(60L), Gv.I(180L))).Iter()) {
      _p(refs.I((Gv.S("sp%d") % s)), Gv.B(Gv.Eq(Gd.Dyn("int", m.G("speed")), s)));
    }
    Gv t = (S.G("t")).AsF();
    _t(refs.G("time"), (Gv.S("Čas várky %d:%02d:%02d   (rychlost x%d)") % Gd.Arr(Gd.Dyn("int", (t / Gv.I(3600L))), (Gd.Dyn("int", (t / Gv.I(60L))) % Gv.I(60L)), (Gd.Dyn("int", t) % Gv.I(60L)), Gd.Dyn("int", m.G("speed")))));
    _t(refs.G("auto"), ((m.G("auto").G("on")).T() ? Gv.S("STOP automatu") : ((!(m.G("auto").G("done")).T() ? Gv.S("Spustit automatickou várku") : Gv.S("Hotovo – spustit znovu")))));
    _t(refs.G("autost"), (Gv.Ne(m.G("auto").G("status"), Gv.S("")) ? m.G("auto").G("status") : Gv.S("Automat sám otvírá cesty, pouští čerpadla, topí a přidává chmel. Když se něco zasekne, zastaví se a napíše proč.")));
    _t(refs.G("cmain"), (Gv.S("Hlavní: %s") % onoff(S.G("cab").G("main"))));
    _t(refs.G("ckey"), (Gv.S("Klíč: %s") % (((S.G("cab").G("key")).T() ? Gv.S("START") : Gv.S("vyp")))));
    _t(refs.G("cest"), (Gv.S("Nouzové stop: %s") % (((S.G("cab").G("estop")).T() ? Gv.S("STISKNUTO") : Gv.S("ok")))));
    _t(refs.G("cint"), (Gv.S("Ochrany čerpadel (interlock): %s") % (((S.G("cab").C("get", Gv.S("interlock"), Gv.B(true))).T() ? Gv.S("ZAP") : Gv.S("VYP – zkouška")))));
    _t(refs.G("boiler"), (Gv.S("Pára z kotle: %s   (zatížení %d %%)") % Gd.Arr(onoff(S.G("boiler").G("on")), Gd.roundi((S.G("boiler").G("load") * Gv.F(100.0))))));
    foreach (Gv p in (Gd.Arr(Gv.I(0L), Gv.I(25L), Gv.I(50L), Gv.I(75L), Gv.I(100L))).Iter()) {
      _p(refs.I((Gv.S("h%d") % p)), Gv.B(Gv.Eq(Gd.roundi((S.G("rvp").G("heat") * Gv.F(100.0))), p)));
    }
    _t(refs.G("hz"), (Gv.S("%d Hz") % Gd.roundi(S.G("rvp").G("hz"))));
    _t(refs.G("stir"), (Gv.S("Míchadlo: %s") % onoff(S.G("rvp").G("stir"))));
    _t(refs.G("wtarget"), (Gv.S("%d L") % Gd.roundi(S.G("rvpWater").G("target"))));
    _t(refs.G("water"), ((S.G("rvpWater").G("on")).T() ? ((Gv.S("Stop  %d / %d L") % Gd.Arr(Gd.roundi(S.G("rvpWater").G("added")), Gd.roundi(S.G("rvpWater").G("target"))))) : Gv.S("Napouštět pánev")));
    _t(refs.G("mix"), (Gv.S("Směšovač: %s") % (((S.G("mixAuto")).T() ? Gv.S("ZAP") : Gv.S("vyp")))));
    _t(refs.G("mixT"), (Gv.S("%d °C") % Gd.roundi(S.G("rvpWater").G("T"))));
    if (Gv.Ge(S.G("malt").G("added"), (S.G("malt").G("target") - Gv.F(0.01)))) {
      _t(refs.G("malt"), (Gv.S("Slad vystřen (%d kg)") % Gd.roundi(S.G("malt").G("added"))));
    } else {
      _t(refs.G("malt"), ((S.G("malt").G("on")).T() ? ((Gv.S("Stop šrotu  %d / %d kg") % Gd.Arr(Gd.roundi(S.G("malt").G("added")), Gd.roundi(S.G("malt").G("target"))))) : (Gv.S("Šrotovat a vystírat (%d kg)") % Gd.roundi((S.G("malt").G("target") - S.G("malt").G("added"))))));
    }
    _t(refs.G("gate"), (Gv.S("Šoupě násypky: %s") % (((S.G("malt").G("gate")).T() ? Gv.S("OTEVŘENO") : Gv.S("zavřeno")))));
    _t(refs.G("hatch"), (Gv.S("Výhoz mláta: %s") % (((S.G("sk").C("get", Gv.S("hatch"), Gv.B(false))).T() ? Gv.S("OTEVŘEN") : Gv.S("zavřen")))));
    Gv hp = m.G("sim").C("hopPlanNext");
    if (Gv.Ne(hp, Gv.Null)) {
      Gv nm = Gv.S("");
      foreach (Gv h in (m.G("sim").G("HOPS")).Iter()) {
        if (Gv.Eq(h.G("id"), hp.G("hop"))) {
          nm = h.G("name");
        }
      }
      _t(refs.G("hop"), (Gv.S("Chmel: %s kg %s (%d. min varu)") % Gd.Arr(c1(hp.G("kg")), nm, Gd.roundi(hp.G("min")))));
    } else {
      _t(refs.G("hop"), (Gv.S("Chmel podle receptu přidán (%d dávky)") % S.G("hops").C("size")));
    }
    _t(refs.G("sparge"), (((S.G("skWater").G("on")).T() ? Gv.S("Stop vyslazování") : Gv.S("Vyslazovací voda"))));
    _t(refs.G("ph"), (Gv.S("%s") % c1(S.C("get", Gv.S("mashPH"), S.G("pH")))));
    Gv pr = m.G("sim").G("proc");
    if ((Gv.Ne(pr, Gv.Null) && (S.C("has", Gv.S("water"))).T())) {
      Gv R = m.G("sim").G("RECIPES").I(m.G("sim").G("recipe_idx"));
      _t(refs.G("recipe"), (Gv.S("Recept: %s ▸") % R.G("name")));
      Gv hs = Gd.Arr();
      foreach (Gv h_2 in (R.G("hops")).Iter()) {
        hs.C("append", (Gv.S("%s kg %s v %d.") % Gd.Arr(c1(h_2.G("kg")), h_2.G("hop").C("to_upper"), Gd.roundi(h_2.G("min")))));
      }
      _t(refs.G("recipe_info"), (Gv.S("%s °P · %d IBU · var %d min · sypání %s · chmel: %s%s") % Gd.Arr(c1(R.G("P")), Gd.roundi(R.G("IBU")), Gd.roundi(R.G("boil")), Gv.S(", ").C("join", Gd.Ctor("PackedStringArray", R.G("grist").C("keys").C("map", Gd.Fn((_a) => { Gv k = Gd.A(_a, 0); return (Gv.S("%s %d kg") % Gd.Arr(k, Gd.roundi(R.G("grist").I(k)))); return Gv.Null; })))), Gv.S("; ").C("join", Gd.Ctor("PackedStringArray", hs)), (Gv.Eq(S.G("cfg").C("get", Gv.S("recipe"), Gv.S("")), R.G("name")) ? Gv.S("") : Gv.S("  (po stisku Nová várka)")))));
      _t(refs.G("water_t"), (Gv.S("Voda: %s ▸") % S.G("water").C("get", Gv.S("name"), Gv.S("vlastní"))));
      _t(refs.G("lactic"), (Gv.S("%d mL") % Gd.roundi(S.G("lactic"))));
      _t(refs.G("ph_info"), (Gv.S("Alkalita %d, Ca %d, Mg %d mg/L → zbytková alkalita %d mg/L CaCO₃. pH rmutu %s (optimum 5,4–5,6). Aktivita α %d %%, β %d %%.") % Gd.Arr(Gd.roundi(S.G("water").G("alk")), Gd.roundi(S.G("water").G("ca")), Gd.roundi(S.G("water").G("mg")), Gd.roundi(pr.C("residual_alk", S.G("water"))), c1(S.C("get", Gv.S("mashPH"), pr.C("mash_ph"))), Gd.roundi((S.C("get", Gv.S("phF"), Gd.StaticGet("Vector2", "ONE")).G("x") * Gv.F(100.0))), Gd.roundi((S.C("get", Gv.S("phF"), Gd.StaticGet("Vector2", "ONE")).G("y") * Gv.F(100.0))))));
      Gv F = S.G("fer");
      _t(refs.G("fer"), ((F.G("on")).T() ? Gv.S("Ukončit kvašení") : Gv.S("Zakvasit")));
      _t(refs.G("fer_set"), (Gv.S("%s °C") % c1(F.G("set"))));
      _t(refs.G("fer_info"), (((F.G("on")).T() || Gv.Gt(F.G("t"), Gv.I(0L))) ? ((Gv.S("Den %s · %s °C · zdánlivý extrakt %s °P · alkohol %s %% obj. · diacetyl %d ppb · kvasnice %s g/L") % Gd.Arr(c1((F.G("t") / Gv.F(86400.0))), c1(F.G("T")), c1(pr.C("app_extract")), c1(((F.G("E") / Gv.F(10.0)) / Gv.F(0.79))), Gd.roundi(F.G("D")), c1(F.G("X"))))) : Gv.S("Po protlačce se zakvasí samo, nebo stiskni Zakvasit. Přeskoč 12 h a sleduj diacetyl.")));
      Gv C = S.G("cip");
      _t(refs.G("cip_t"), (Gv.S("Kde: %s ▸") % pr.C("cip_name", C.G("target"))));
      _t(refs.G("cip_m"), (Gv.S("Čím: %s ▸") % C.G("medium")));
      _t(refs.G("cip_run"), ((C.G("run")).T() ? Gv.S("STOP CIP") : Gv.S("Spustit CIP")));
      _t(refs.G("cip_set"), (Gv.S("%d °C") % Gd.roundi(C.G("set"))));
      _t(refs.G("cip_conc"), (Gv.S("%s %%") % c1(C.C("get", Gv.S("conc"), Gv.F(1.5)))));
      _t(refs.G("cip_info"), (((((C.G("run")).T() ? (Gv.S("Běží %d min, roztok %s °C · ") % Gd.Arr(Gd.roundi(C.G("min")), c1(C.G("T")))) : Gv.S(""))) + pr.C("cip_status")) + (Gv.S(". Louh ≥ 75 °C a ≥ %s %% NaOH 20 min, kyselina ≤ 60 °C a ≥ %s %% 20 min, mezi tím proplach vodou. Hodnotí se dodržení postupu, ne mikrobiologická čistota.") % Gd.Arr(c1(S.G("cfg").C("get", Gv.S("cipLyeMin"), Gv.F(1.0))), c1(S.G("cfg").C("get", Gv.S("cipAcidMin"), Gv.F(0.5)))))));
      _t(refs.G("faults"), (Gv.S("Nácvik poruch: %s") % (((S.G("faults").G("on")).T() ? Gv.S("ZAP") : Gv.S("vyp")))));
      _t(refs.G("faults_info"), pr.C("faults_text"));
      Gv sc = pr.C("scorecard_text");
      if (Gv.Ne(refs.G("score").G("text"), sc)) {
        refs = refs.S("score", refs.G("score").S("text", sc));
        changed = Gv.B(true);
      }
    }
    _t(refs.G("sflow"), (Gv.S("%d L/min") % Gd.roundi(S.G("skWater").G("flow"))));
    _t(refs.G("rake"), (Gv.S("Kopačka: %s · hloubka %d %%") % Gd.Arr(onoff(S.G("sk").G("rake")), Gd.roundi((S.G("sk").G("rakeH") * Gv.F(100.0))))));
    foreach (Gv key in (Gd.Arr(Gv.S("p1"), Gv.S("p2"), Gv.S("p3"))).Iter()) {
      _t(refs.I(key), (Gv.S("%s %s") % Gd.Arr(m.C("pump_name", key), ((S.I(key).G("on")).T() ? Gv.S("STOP") : Gv.S("start")))));
      _t(refs.I((key + Gv.S("f"))), (Gv.S("%d L/min") % Gd.roundi(S.I(key).G("flow"))));
      _t(refs.I((key + Gv.S("s"))), (((S.I(key).G("on")).T() && Gv.Ne(S.I(key).G("status"), Gv.S(""))) ? ((Gv.S("   ") + S.I(key).G("status"))) : Gv.S("")));
      Gv vis = Gv.B(Gv.Ne(refs.I((key + Gv.S("s"))).G("text"), Gv.S("")));
      if (Gv.Ne(refs.I((key + Gv.S("s"))).G("visible"), vis)) {
        refs = refs.SI((key + Gv.S("s")), refs.I((key + Gv.S("s"))).S("visible", vis));
        changed = Gv.B(true);
      }
    }
    _t(refs.G("plv"), (Gv.S("%s ledová voda %s") % Gd.Arr(m.C("pump_name", Gv.S("plv")), ((S.G("lv").G("pump")).T() ? Gv.S("STOP") : Gv.S("start")))));
    _t(refs.G("chill"), (Gv.S("Agregát (glykol): %s") % onoff(S.G("lv").G("chill"))));
    Gv sim = m.G("sim");
    Gv lines = Gd.Arr();
    lines.C("append", (Gv.S("[b]Pánev:[/b] %s") % m.C("vessel_text", Gv.S("rvp"))));
    Gv io = sim.C("iodine", S.G("rvp"));
    if ((Gv.Ne(io, Gv.Null) && Gv.Gt(S.G("rvp").G("g"), Gv.I(5L)))) {
      lines.C("append", (Gv.S("   jodová zkouška: %s") % Gd.Dict(Gv.S("neg"), Gv.S("[color=#7ee08a]negativní (zcukřeno)[/color]"), Gv.S("weak"), Gv.S("[color=#f2c230]slabě modrá[/color]"), Gv.S("pos"), Gv.S("[color=#ff6b5b]modrá – škrob[/color]")).I(io)));
    }
    lines.C("append", (Gv.S("[b]Scezovací káď:[/b] %s") % m.C("vessel_text", Gv.S("sk"))));
    if (Gv.Gt(S.G("sk").G("g"), Gv.I(1L))) {
      Gv sk_ = S.G("sk");
      Gv risk_ = (Gv.Lt(S.G("tannin"), Gv.F(0.3)) ? Gv.S("nízké") : ((Gv.Lt(S.G("tannin"), Gv.F(0.6)) ? Gv.S("střední") : Gv.S("VYSOKÉ"))));
      Gv load = Gd.roundi(((Gv.F(100.0) * sk_.G("Qact")) / Gd.maxf(Gv.F(1.0), sk_.G("Qmax"))));
      lines.C("append", (Gv.S("   lože: průtok %d z max %d L/min (%d %%), tlak %d mbar, zhutnění %d %%, kanálky %d %%, zákal %d, částic %s kg%s") % Gd.Arr(Gd.roundi(sk_.G("Qact")), Gd.roundi(sk_.G("Qmax")), load, Gd.roundi(sk_.G("dp")), Gd.roundi((sk_.G("c") * Gv.F(25.0))), Gd.roundi((sk_.G("ch") * Gv.F(200.0))), Gd.roundi(sk_.G("turb")), c1(sk_.G("fines")), (Gv.Gt(sk_.G("air"), Gv.F(0.05)) ? Gv.S(" · VZDUCH") : Gv.S("")))));
      lines.C("append", (Gv.S("   třísloviny (varovný index podle pH, teploty a zředění, ne koncentrace): %s") % risk_));
    }
    lines.C("append", (Gv.S("[b]Vířivá káď:[/b] %s") % m.C("vessel_text", Gv.S("wp"))));
    lines.C("append", (Gv.S("[b]Bojler:[/b] %s") % m.C("vessel_text", Gv.S("hlt"))));
    lines.C("append", (Gv.S("[b]Ledovka:[/b] %s") % m.C("vessel_text", Gv.S("lv"))));
    lines.C("append", (Gv.S("[b]Spilka:[/b] %s") % m.C("vessel_text", Gv.S("spilka"))));
    Gv tr = sim.G("training");
    Gv cs = (((tr.C("text") + Gv.S("\n[color=#7b858e]")) + tr.C("list_text")) + Gv.S("[/color]"));
    if (Gv.Ne(check_lbl.G("text"), cs)) {
      check_lbl = check_lbl.S("text", cs);
      changed = Gv.B(true);
    }
    Gv tcur = tr.C("current");
    refs = refs.S("next_step", refs.G("next_step").S("disabled", Gv.B((!(tcur.G("ready")).T() || (tcur.G("complete")).T()))));
    _t(refs.G("step_pause"), (Gv.S("Pauza při splnění: ") + onoff(tr.C("state").G("pause"))));
    Gv las = sim.G("lauter_auto").C("state");
    _t(refs.G("lauter_auto"), ((((las.G("on")).T() ? Gv.S("Zastavit automat scezování: ") : Gv.S("Spustit automat scezování: "))) + las.G("stage")));
    _t(refs.G("lauter"), (Gv.Eq(S.G("sk").G("mode"), Gv.S("mash")) ? Gv.S("Odrmutovat a vytvořit lože") : Gv.S("Lože připraveno – scezování")));
    refs = refs.S("lauter", refs.G("lauter").S("disabled", Gv.B(Gv.Eq(S.G("sk").G("mode"), Gv.S("lauter")))));
    Gv rep = sim.C("spReport");
    if (Gv.Ne(rep, Gv.Null)) {
      lines.C("append", (Gv.S("[b]Protokol (spilka):[/b] %s hl · %s °P · hořkost %d IBU (cíl %d) · barva %s EBC · DMS %d ppb · zdánlivá zkvasitelnost %d %% · výtěžnost %s %%") % Gd.Arr(c1(rep.G("V")), c1(rep.G("P")), Gd.roundi(rep.G("IBU")), Gd.roundi(S.G("cfg").G("targetIBU")), c1(rep.G("EBC")), Gd.roundi(rep.G("dms")), Gd.roundi(rep.G("att")), c1(rep.G("yield")))));
    }
    if (Gv.Ne(S.G("flags").G("knock"), Gv.Null)) {
      Gv yy = m.G("sim").C("yields");
      lines.C("append", (Gv.S("[color=#9aa3ab]Vyrážka: %s hl horké při %s °P · výtěžnost varny %s %% · využití lab. extraktu %s %%[/color]") % Gd.Arr(c1((S.G("flags").G("knock").G("V") / Gv.F(100.0))), c1(S.G("flags").G("knock").G("P")), c1(yy.C("get", Gv.S("VV_varna"), Gv.F(0.0))), c1(yy.C("get", Gv.S("ucinnost"), Gv.F(0.0))))));
    }
    Gv lost = ((S.G("drainL") + S.G("spillL"))).AsF();
    if (Gv.Gt(lost, Gv.I(1L))) {
      lines.C("append", (Gv.S("[color=#ff6b5b][b]Vyteklo / do kanálu:[/b] %d L[/color]") % Gd.roundi(lost)));
    }
    lines.C("append", (Gv.S("[color=#9aa3ab]Voda z bojleru celkem %d L · pára %d °C[/color]") % Gd.Arr(Gd.roundi(S.G("usedHot")), Gd.roundi(S.G("Tsteam")))));
    Gv st = Gv.S("\n").C("join", Gd.Ctor("PackedStringArray", lines));
    if (Gv.Ne(state_lbl.G("text"), st)) {
      state_lbl = state_lbl.S("text", st);
      changed = Gv.B(true);
    }
    if ((((S.G("log").C("size")).T() && Gv.Ne(S.G("log").I(Gv.I(0L)).G("t"), last_log)) || Gv.Eq(log_box.G("text"), Gv.S("")))) {
      last_log = ((S.G("log").C("size")).T() ? S.G("log").I(Gv.I(0L)).G("t") : Gv.F(-1.0));
      Gv ll = Gd.Arr();
      foreach (Gv l in (S.G("log").C("slice", Gv.I(0L), Gv.I(9L))).Iter()) {
        Gv c = (Gv.Eq(l.G("type"), Gv.S("bad")) ? Gv.S("#ff6b5b") : ((Gv.Eq(l.G("type"), Gv.S("warn")) ? Gv.S("#f2c230") : Gv.S("#c9d1d8"))));
        ll.C("append", (Gv.S("[color=#7b858e]%d:%02d[/color] [color=%s]%s[/color]") % Gd.Arr(Gd.Dyn("int", (l.G("t") / Gv.I(3600L))), (Gd.Dyn("int", (l.G("t") / Gv.I(60L))) % Gv.I(60L)), c, l.G("msg"))));
      }
      changed = Gv.B(true);
      log_box = log_box.S("text", ((ll.C("size")).T() ? Gv.S("\n").C("join", Gd.Ctor("PackedStringArray", ll)) : Gv.S("[color=#7b858e]Zatím nic. Otevři klapky, pusť vodu a spusť čas (Spustit).[/color]")));
    }
    if ((changed).T()) {
      changed = Gv.B(false);
      dirty();
    }
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "Art": return S_ctl.Art;
    case "m": return this.m;
    case "S": return this.S;
    case "refs": return this.refs;
    case "log_box": return this.log_box;
    case "state_lbl": return this.state_lbl;
    case "check_lbl": return this.check_lbl;
    case "t_acc": return this.t_acc;
    case "last_log": return this.last_log;
    case "sv": return this.sv;
    case "trend": return this.trend;
    case "hist": return this.hist;
    case "HIST_DT": return S_ctl.HIST_DT;
    case "HIST_N": return S_ctl.HIST_N;
    case "SERIES": return S_ctl.SERIES;
    case "changed": return this.changed;
    case "pend": return this.pend;
    case "YEL": return S_ctl.YEL;
    case "DIM": return S_ctl.DIM;
    case "COLLAPSED0": return S_ctl.COLLAPSED0;
    case "PANEL_CFG": return S_ctl.PANEL_CFG;
    case "collapsed": return this.collapsed;
    case "sections": return this.sections;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "m": this.m = v; return v;
    case "S": this.S = v; return v;
    case "refs": this.refs = v; return v;
    case "log_box": this.log_box = v; return v;
    case "state_lbl": this.state_lbl = v; return v;
    case "check_lbl": this.check_lbl = v; return v;
    case "t_acc": this.t_acc = v; return v;
    case "last_log": this.last_log = v; return v;
    case "sv": this.sv = v; return v;
    case "trend": this.trend = v; return v;
    case "hist": this.hist = v; return v;
    case "changed": this.changed = v; return v;
    case "pend": this.pend = v; return v;
    case "collapsed": this.collapsed = v; return v;
    case "sections": this.sections = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "_ready": return _ready();
    case "dirty": return dirty();
    case "_head": return _head(Gd.A(a, 0), Gd.A(a, 1));
    case "_load_collapsed": return _load_collapsed();
    case "_save_collapsed": return _save_collapsed();
    case "_cardify": return _cardify(Gd.A(a, 0));
    case "_dbg_wide": return _dbg_wide(Gd.A(a, 0), Gd.A(a, 1));
    case "_fit_width": return _fit_width(Gd.A(a, 0), Gd.A(a, 1));
    case "_apply_sec": return _apply_sec(Gd.A(a, 0));
    case "_toggle_sec": return _toggle_sec(Gd.A(a, 0));
    case "_row": return _row(Gd.A(a, 0));
    case "_btn": return _btn(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "_on_btn": return _on_btn(Gd.A(a, 0));
    case "_toggle_auto": return _toggle_auto();
    case "_water": return _water();
    case "_malt": return _malt();
    case "_lbl": return _lbl(Gd.A(a, 0), Gd.A(a, 1));
    case "_t": return _t(Gd.A(a, 0), Gd.A(a, 1));
    case "_p": return _p(Gd.A(a, 0), Gd.A(a, 1));
    case "onoff": return onoff(Gd.A(a, 0));
    case "c1": return c1(Gd.A(a, 0));
    case "_process": return _process(Gd.A(a, 0));
    case "_sample": return _sample();
    case "refresh": return refresh();
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    case "Art": return Art;
    case "HIST_DT": return HIST_DT;
    case "HIST_N": return HIST_N;
    case "SERIES": return SERIES;
    case "YEL": return YEL;
    case "DIM": return DIM;
    case "COLLAPSED0": return COLLAPSED0;
    case "PANEL_CFG": return PANEL_CFG;
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_ctl(); return Gv.O(o); }
  public override string ScriptName { get { return "ctl"; } }
}
}
