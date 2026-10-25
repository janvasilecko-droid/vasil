// Vygenerováno z rotametr.gd překladačem gd2cs.py – needituj ručně.
using System; using System.Collections.Generic;
namespace Pivovar.Gd.Gen {
public partial class S_rotametr : GdNode {
  public static Gv IDEAL;
  public static Gv MAXV;
  public static Gv YEL;
  public Gv m;
  public Gv btn;
  public Gv shown;
  public Gv shake;
  public Gv tapmsg;
  static S_rotametr() {
    IDEAL = Gv.F(20.0);
    MAXV = Gv.F(40.0);
    YEL = Gd.Ctor("Color", Gv.S("f2c230"));
  }
  public S_rotametr() {
    m = Gv.Null;
    btn = Gv.Null;
    shown = Gv.F(0.0);
    shake = Gv.F(0.0);
    tapmsg = Gv.F(0.0);
  }
  public Gv _ready() {
    Self.S("mouse_filter", Gd.StaticGet("Control", "MOUSE_FILTER_IGNORE"));
    btn = Gd.New("Button");
    btn = btn.S("text", Gv.S("PROKLEPAT"));
    btn = btn.S("focus_mode", Gd.StaticGet("Control", "FOCUS_NONE"));
    btn = btn.S("tooltip_text", Gv.S("Poklepat na rotametr – uvolní zrna, co drží plovák (T)"));
    btn.G("pressed").C("connect", Gd.Fn((_a) => tap()));
    Self.C("add_child", btn);
    return Gv.Null;
  }
  public Gv tap() {
    Gv S = m.G("sim").G("S");
    if (!(S.C("has", Gv.S("rota"))).T()) {
      S = S.SI(Gv.S("rota"), Gd.Dict(Gv.S("stick"), Gv.F(0.0)));
    }
    S = S.S("rota", S.G("rota").S("stick", Gd.maxf(Gv.F(0.0), (S.G("rota").G("stick") - Gv.F(1.0)))));
    shake = Gv.F(0.5);
    tapmsg = Gv.F(2.0);
    m.C("_say", Gv.S("Rotametr proklepnut – plovák je volný."));
    return Gv.Null;
  }
  public Gv _process(Gv delta = default(Gv)) {
    delta = (delta).AsF();
    if ((Gv.Eq(m, Gv.Null) || Gv.Eq(m.G("sim"), Gv.Null))) {
      return Gv.Null;
    }
    Gv vs = Self.C("get_viewport").C("get_visible_rect").G("size");
    Gv h = (Gd.clampf((vs.G("y") - Gv.F(400.0)), Gv.F(260.0), Gv.F(360.0))).AsF();
    Self.S("size", Gd.Ctor("Vector2", Gv.I(150L), h));
    Self.S("position", Gd.Ctor("Vector2", Gv.I(12L), (Gv.Gt(vs.G("y"), Gv.I(700L)) ? ((vs.G("y") - h) - Gv.F(80.0)) : Gv.F(250.0))));
    Self.S("position", Self.G("position").S("y", Gd.maxf(Self.G("position").G("y"), Gv.F(250.0))));
    btn = btn.S("position", Gd.Ctor("Vector2", Gv.I(10L), (h - Gv.I(34L))));
    btn = btn.S("size", Gd.Ctor("Vector2", Gv.I(130L), Gv.I(28L)));
    Gv S = m.G("sim").G("S");
    Gv q = (((S.G("p2").G("on")).T() ? S.G("p2").G("q") : Gv.F(0.0))).AsF();
    Gv st = (S.C("get", Gv.S("rota"), Gd.Dict()).C("get", Gv.S("stick"), Gv.F(0.0))).AsF();
    Gv target = ((q * ((Gv.F(1.0) - (Gv.F(0.6) * Gd.clampf(st, Gv.F(0.0), Gv.F(1.0))))))).AsF();
    shown = Gd.lerpf(shown, target, Gd.minf(Gv.F(1.0), (delta * Gv.F(3.0))));
    shake = Gd.maxf(Gv.F(0.0), (shake - delta));
    tapmsg = Gd.maxf(Gv.F(0.0), (tapmsg - delta));
    Self.C("queue_redraw");
    return Gv.Null;
  }
  public Gv _unhandled_key_input(Gv e = default(Gv)) {
    if (((((Gv.Is(e, "InputEventKey") && (e.G("pressed")).T()) && !(e.G("echo")).T()) && Gv.Eq(e.G("keycode"), Gd.K("KEY_T"))) && Gv.Eq(m.G("editor"), Gv.Null))) {
      tap();
    }
    return Gv.Null;
  }
  public Gv _draw() {
    Gv f = m.G("font");
    Gv S = m.G("sim").G("S");
    Gv st = (S.C("get", Gv.S("rota"), Gd.Dict()).C("get", Gv.S("stick"), Gv.F(0.0))).AsF();
    Self.C("draw_rect", Gd.Ctor("Rect2", Gd.StaticGet("Vector2", "ZERO"), Self.G("size")), Gd.Ctor("Color", Gv.F(0.07), Gv.F(0.09), Gv.F(0.11), Gv.F(0.92)), Gv.B(true));
    Self.C("draw_rect", Gd.Ctor("Rect2", Gd.StaticGet("Vector2", "ZERO"), Self.G("size")), YEL, Gv.B(false), Gv.F(2.0));
    Self.C("draw_string", f, Gd.Ctor("Vector2", Gv.I(10L), Gv.I(18L)), Gv.S("ROTAMETR SCEZOVÁNÍ"), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(12L), YEL);
    Gv top = (Gv.F(34.0)).AsF();
    Gv bot = ((Self.G("size").G("y") - Gv.F(120.0))).AsF();
    Gv tx = (Gv.F(20.0)).AsF();
    Gv tw = (Gv.F(56.0)).AsF();
    Self.C("draw_rect", Gd.Ctor("Rect2", tx, top, tw, (bot - top)), Gd.Ctor("Color", Gv.F(0.55), Gv.F(0.8), Gv.F(0.95), Gv.F(0.18)), Gv.B(true));
    Self.C("draw_rect", Gd.Ctor("Rect2", tx, top, tw, (bot - top)), Gd.Ctor("Color", Gv.S("c9d1d9")), Gv.B(false), Gv.F(2.0));
    Gv yv = Gd.Fn((_a) => { Gv v = Gd.A(_a, 0); return ((bot - (((bot - top)) * Gd.clampf((v / MAXV), Gv.F(0.0), Gv.F(1.0))))).AsF(); return Gv.Null; });
    Self.C("draw_rect", Gd.Ctor("Rect2", tx, yv.C("call", (IDEAL + Gv.F(2.0))), tw, (yv.C("call", (IDEAL - Gv.F(2.0))) - yv.C("call", (IDEAL + Gv.F(2.0))))), Gd.Ctor("Color", Gv.F(0.2), Gv.F(0.8), Gv.F(0.35), Gv.F(0.28)), Gv.B(true));
    foreach (Gv v_2 in Gd.RangeIter(Gv.I(0L), Gv.I(41L), Gv.I(5L))) {
      Gv y = (yv.C("call", Gd.Dyn("float", v_2))).AsF();
      Self.C("draw_line", Gd.Ctor("Vector2", (tx + tw), y), Gd.Ctor("Vector2", ((tx + tw) + ((Gv.Eq((v_2 % Gv.I(10L)), Gv.I(0L)) ? Gv.F(14.0) : Gv.F(8.0)))), y), Gd.Ctor("Color", Gv.S("c9d1d9")), Gv.F(1.5));
      if (Gv.Eq((v_2 % Gv.I(10L)), Gv.I(0L))) {
        Self.C("draw_string", f, Gd.Ctor("Vector2", ((tx + tw) + Gv.I(18L)), (y + Gv.I(4L))), Gd.Dyn("str", v_2), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(13L), Gd.Ctor("Color", Gv.S("e6edf3")));
      }
    }
    Gv yi = (yv.C("call", IDEAL)).AsF();
    Self.C("draw_line", Gd.Ctor("Vector2", (tx - Gv.I(8L)), yi), Gd.Ctor("Vector2", ((tx + tw) + Gv.I(12L)), yi), Gd.Ctor("Color", Gv.S("35d06a")), Gv.F(3.0));
    Self.C("draw_string", f, Gd.Ctor("Vector2", ((tx + tw) + Gv.I(18L)), (yi - Gv.I(6L))), Gv.S("ideál 20"), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(11L), Gd.Ctor("Color", Gv.S("35d06a")));
    Gv fy = (yv.C("call", shown)).AsF();
    if (Gv.Gt(shake, Gv.F(0.0))) {
      fy = ((fy + (((Gd.sin((shake * Gv.F(60.0))) * Gv.F(4.0)) * shake) * Gv.F(2.0)))).AsF();
    }
    Gv cx = ((tx + (tw * Gv.F(0.5)))).AsF();
    Gv col = (Gv.Lt(st, Gv.F(0.5)) ? Gd.Ctor("Color", Gv.S("e0452f")) : Gd.Ctor("Color", Gv.S("b58a3a")));
    Self.C("draw_colored_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", (cx - Gv.I(20L)), (fy - Gv.I(12L))), Gd.Ctor("Vector2", (cx + Gv.I(20L)), (fy - Gv.I(12L))), Gd.Ctor("Vector2", (cx + Gv.I(12L)), (fy + Gv.I(14L))), Gd.Ctor("Vector2", (cx - Gv.I(12L)), (fy + Gv.I(14L))))), col);
    Self.C("draw_line", Gd.Ctor("Vector2", (cx - Gv.I(20L)), (fy - Gv.I(12L))), Gd.Ctor("Vector2", (cx + Gv.I(20L)), (fy - Gv.I(12L))), Gd.Ctor("Color", Gv.S("f4f6f8")), Gv.F(3.0));
    if (Gv.Gt(st, Gv.F(0.3))) {
      foreach (Gv k in ((Gd.Dyn("int", (st * Gv.F(8.0))) + Gv.I(1L))).Iter()) {
        Self.C("draw_circle", Gd.Ctor("Vector2", ((cx - Gv.I(16L)) + (((k * Gv.I(7L))) % Gv.I(32L))), ((fy + Gv.I(16L)) + (((k * Gv.I(5L))) % Gv.I(6L)))), Gv.F(2.0), Gd.Ctor("Color", Gv.S("8a6a3a")));
      }
    }
    Gv ry = ((bot + Gv.F(16.0))).AsF();
    Self.C("draw_rect", Gd.Ctor("Rect2", Gv.I(10L), ry, Gv.I(130L), Gv.I(40L)), Gd.Ctor("Color", Gv.S("0b0f12")), Gv.B(true));
    Self.C("draw_rect", Gd.Ctor("Rect2", Gv.I(10L), ry, Gv.I(130L), Gv.I(40L)), Gd.Ctor("Color", Gv.S("444c54")), Gv.B(false), Gv.F(1.0));
    Gv ok = Gv.B(Gv.Le(Gd.absf((shown - IDEAL)), Gv.F(2.0)));
    Self.C("draw_string", f, Gd.Ctor("Vector2", Gv.I(18L), (ry + Gv.I(30L))), (Gv.S("%.1f") % shown), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(28L), ((ok).T() ? Gd.Ctor("Color", Gv.S("35d06a")) : ((Gv.Gt(shown, Gv.F(1.0)) ? Gd.Ctor("Color", Gv.S("ffb020")) : Gd.Ctor("Color", Gv.S("8a939b"))))));
    Self.C("draw_string", f, Gd.Ctor("Vector2", Gv.I(104L), (ry + Gv.I(30L))), Gv.S("L/min"), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(12L), Gd.Ctor("Color", Gv.S("c9d1d9")));
    Gv msg = Gv.S("");
    if (Gv.Gt(tapmsg, Gv.F(0.0))) {
      msg = Gv.S("proklepnuto ✔");
    } else if (Gv.Gt(st, Gv.F(0.6))) {
      msg = Gv.S("plovák se lepí – proklepat!");
    } else if (Gv.Gt(st, Gv.F(0.3))) {
      msg = Gv.S("zrna se usazují…");
    }
    Self.C("draw_string", f, Gd.Ctor("Vector2", Gv.I(10L), (ry + Gv.I(58L))), msg, Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(12L), Gd.Ctor("Color", Gv.S("ffb020")));
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "IDEAL": return S_rotametr.IDEAL;
    case "MAXV": return S_rotametr.MAXV;
    case "YEL": return S_rotametr.YEL;
    case "m": return this.m;
    case "btn": return this.btn;
    case "shown": return this.shown;
    case "shake": return this.shake;
    case "tapmsg": return this.tapmsg;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "m": this.m = v; return v;
    case "btn": this.btn = v; return v;
    case "shown": this.shown = v; return v;
    case "shake": this.shake = v; return v;
    case "tapmsg": this.tapmsg = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "_ready": return _ready();
    case "tap": return tap();
    case "_process": return _process(Gd.A(a, 0));
    case "_unhandled_key_input": return _unhandled_key_input(Gd.A(a, 0));
    case "_draw": return _draw();
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    case "IDEAL": return IDEAL;
    case "MAXV": return MAXV;
    case "YEL": return YEL;
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_rotametr(); return Gv.O(o); }
  public override string ScriptName { get { return "rotametr"; } }
}
}
