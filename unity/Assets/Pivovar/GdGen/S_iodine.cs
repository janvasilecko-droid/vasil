// Vygenerováno z iodine.gd překladačem gd2cs.py – needituj ručně.
using System; using System.Collections.Generic;
namespace Pivovar.Gd.Gen {
public partial class S_iodine : GdNode {
  public Gv m;
  public Gv source;
  public Gv grist;
  public Gv info;
  public Gv anim;
  public static Gv Art;
  static S_iodine() {
    Art = Gd.Script("art");
  }
  public S_iodine() {
    TypeName = "Control";
    m = Gv.Null;
    source = Gv.S("rvp");
    grist = Gv.B(false);
    info = Gd.Dict();
    anim = Gv.F(0.0);
  }
  public Gv open(Gv src = default(Gv)) {
    source = src;
    Self.S("mouse_filter", Gd.StaticGet("Control", "MOUSE_FILTER_STOP"));
    Self.S("size", Gd.Ctor("Vector2", Gv.I(640L), Gv.I(360L)));
    Self.S("position", Gd.Ctor("Vector2", Gv.I(80L), Gv.I(70L)));
    refresh();
    return Gv.Null;
  }
  public Gv refresh() {
    Gv smp = m.G("sim").C("iodine_sample", source, grist);
    info = (Gv.Eq(smp, Gv.Null) ? Gd.Dict(Gv.S("empty"), Gv.B(true)) : m.G("sim").C("iodine_info", smp, grist));
    if (Gv.Ne(smp, Gv.Null)) {
      info = info.SI(Gv.S("T"), smp.C("get", Gv.S("T"), Gv.F(20.0)));
      Gv ig = m.G("sim").C("iodine_info", smp, Gv.B(true));
      info = info.SI(Gv.S("hidden"), Gv.B((((!(grist).T() && !(ig.C("get", Gv.S("empty"), Gv.B(true))).T()) && Gv.Gt(ig.G("starch"), Gv.F(0.02))) && Gv.Lt(info.C("get", Gv.S("starch"), Gv.F(0.0)), Gv.F(0.02)))));
    }
    Self.C("queue_redraw");
    return Gv.Null;
  }
  public Gv _gui_input(Gv e = default(Gv)) {
    if (((Gv.Is(e, "InputEventMouseButton") && (e.G("pressed")).T()) && Gv.Eq(e.G("button_index"), Gd.K("MOUSE_BUTTON_LEFT")))) {
      Gv pos = e.G("position");
      if ((Gd.Ctor("Rect2", (Self.G("size").G("x") - Gv.I(40L)), Gv.I(6L), Gv.I(32L), Gv.I(30L)).C("has_point", pos)).T()) {
        Self.C("queue_free");
      } else if ((Gd.Ctor("Rect2", Gv.I(14L), Gv.I(300L), Gv.I(230L), Gv.I(40L)).C("has_point", pos)).T()) {
        grist = Gv.B(!(grist).T());
        refresh();
      } else if ((Gd.Ctor("Rect2", Gv.I(260L), Gv.I(300L), Gv.I(120L), Gv.I(40L)).C("has_point", pos)).T()) {
        source = (Gv.Eq(source, Gv.S("rvp")) ? Gv.S("sk") : Gv.S("rvp"));
        refresh();
      }
      Self.C("accept_event");
    }
    return Gv.Null;
  }
  public Gv _draw() {
    Gv f = m.G("font");
    Gv fb = m.G("font_b");
    S_art.rrect_border(Self, Gd.Ctor("Rect2", Gd.StaticGet("Vector2", "ZERO"), Self.G("size")), Gd.Ctor("Color", Gv.F(0.07), Gv.F(0.085), Gv.F(0.1), Gv.F(0.97)), Gd.Ctor("Color", Gv.S("f2c230")), Gv.I(2L), Gv.F(8.0));
    Self.C("draw_string", fb, Gd.Ctor("Vector2", Gv.I(14L), Gv.I(28L)), (Gv.S("JODOVÁ ZKOUŠKA – %s") % ((Gv.Eq(source, Gv.S("rvp")) ? Gv.S("dílo v pánvi") : Gv.S("rmut / sladina v SK")))), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(17L), Gd.Ctor("Color", Gv.S("f2c230")));
    Self.C("draw_string", fb, Gd.Ctor("Vector2", (Self.G("size").G("x") - Gv.I(30L)), Gv.I(27L)), Gv.S("X"), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(18L), Gd.Ctor("Color", Gv.S("e6ebef")));
    Gv c = Gd.Ctor("Vector2", Gv.I(150L), Gv.I(160L));
    Self.C("draw_circle", c, Gv.F(112.0), Gd.Ctor("Color", Gv.S("8f9aa3")));
    Self.C("draw_circle", c, Gv.F(108.0), Gd.Ctor("Color", Gv.S("f4f6f7")));
    Self.C("draw_circle", c, Gv.F(98.0), Gd.Ctor("Color", Gv.S("e6eaed")));
    if ((info.C("get", Gv.S("empty"), Gv.B(true))).T()) {
      Self.C("draw_string", f, Gd.Ctor("Vector2", Gv.I(14L), Gv.I(350L)), Gv.S(""), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(1L), Gd.StaticGet("Color", "WHITE"));
      _text(Gd.Arr(Gv.S("V nádobě není co vzorkovat."), Gv.S("Zkoušku dělej s dílem po prodlevě nebo s rmutem v SK.")), Gd.Arr(Gd.Ctor("Color", Gv.S("e6ebef")), Gd.Ctor("Color", Gv.S("9aa3ab"))));
    } else {
      Gv col = info.G("color");
      Self.C("draw_circle", c, Gv.F(44.0), Gd.Ctor("Color", Gv.S("e9d9a8")).C("lerp", col, Gv.F(0.25)));
      Gv k = (Gd.minf(Gv.F(1.0), anim)).AsF();
      Self.C("draw_circle", c, (Gv.F(36.0) * ((Gv.F(0.4) + (Gv.F(0.6) * k)))), Gd.Ctor("Color", col, Gv.F(0.95)));
      Self.C("draw_arc", c, (Gv.F(36.0) * ((Gv.F(0.4) + (Gv.F(0.6) * k)))), Gv.I(0L), Gd.K("TAU"), Gv.I(40L), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.25)), Gv.F(2.0), Gv.B(true));
      Self.C("draw_circle", (c + Gd.Ctor("Vector2", Gv.I(-12L), Gv.I(-12L))), Gv.F(6.0), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.25)));
      _text(Gd.Arr(Gv.S("Barva po kapce jódu:"), info.G("name"), (Gv.S("Vzorek: %s, ochlazený na pokojovou teplotu.") % ((!(grist).T() ? Gv.S("přecezená kapalina") : Gv.S("kapalina s rozetřenými zrny")))), (Gv.S("Škrob: %s · dextriny: %s") % Gd.Arr(_lvl(info.G("starch")), _lvl((info.G("dextrin") * Gv.F(0.4))))), ((info.C("get", Gv.S("hidden"), Gv.B(false))).T() ? Gv.S("POZOR: v zrnech ještě zůstává škrob – kapalina je negativní, ale zrna ne. Zkus „se zrny“.") : Gv.S(""))), Gd.Arr(Gd.Ctor("Color", Gv.S("9aa3ab")), Gd.Ctor("Color", Gv.S("f2c230")), Gd.Ctor("Color", Gv.S("c9d1d8")), Gd.Ctor("Color", Gv.S("7b858e")), Gd.Ctor("Color", Gv.S("ff6b5b"))));
    }
    S_art.rrect_border(Self, Gd.Ctor("Rect2", Gv.I(14L), Gv.I(300L), Gv.I(230L), Gv.I(40L)), Gd.Ctor("Color", Gv.S("262d35")), Gd.Ctor("Color", Gv.S("3a444e")), Gv.I(1L), Gv.F(6.0));
    Self.C("draw_string", fb, Gd.Ctor("Vector2", Gv.I(26L), Gv.I(326L)), (Gv.S("Vzorek: %s") % (((grist).T() ? Gv.S("se zrny (rozetřená drť)") : Gv.S("přecezená kapalina")))), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(14L), Gd.Ctor("Color", Gv.S("e6ebef")));
    S_art.rrect_border(Self, Gd.Ctor("Rect2", Gv.I(260L), Gv.I(300L), Gv.I(120L), Gv.I(40L)), Gd.Ctor("Color", Gv.S("262d35")), Gd.Ctor("Color", Gv.S("3a444e")), Gv.I(1L), Gv.F(6.0));
    Self.C("draw_string", fb, Gd.Ctor("Vector2", Gv.I(274L), Gv.I(326L)), Gv.S("Pánev / SK"), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(14L), Gd.Ctor("Color", Gv.S("e6ebef")));
    return Gv.Null;
  }
  public Gv _lvl(Gv x = default(Gv)) {
    x = (x).AsF();
    return (Gv.Lt(x, Gv.F(0.002)) ? Gv.S("žádný") : ((Gv.Lt(x, Gv.F(0.01)) ? Gv.S("stopy") : ((Gv.Lt(x, Gv.F(0.04)) ? Gv.S("málo") : ((Gv.Lt(x, Gv.F(0.15)) ? Gv.S("hodně") : Gv.S("velmi mnoho"))))))));
    return Gv.Null;
  }
  public Gv _text(Gv lines = default(Gv), Gv cols = default(Gv)) {
    Gv y = Gv.F(78.0);
    foreach (Gv i in (lines.C("size")).Iter()) {
      Self.C("draw_string", m.G("font"), Gd.Ctor("Vector2", Gv.I(280L), y), lines.I(i), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(340L), Gv.I(13L), cols.I(i));
      y = (y + (Gv.Eq(i, Gv.I(1L)) ? Gv.F(40.0) : Gv.F(24.0)));
    }
    return Gv.Null;
  }
  public Gv _process(Gv d = default(Gv)) {
    d = (d).AsF();
    anim = (anim + (d * Gv.F(1.4)));
    if (Gv.Lt(anim, Gv.F(1.2))) {
      Self.C("queue_redraw");
    }
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "m": return this.m;
    case "source": return this.source;
    case "grist": return this.grist;
    case "info": return this.info;
    case "anim": return this.anim;
    case "Art": return S_iodine.Art;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "m": this.m = v; return v;
    case "source": this.source = v; return v;
    case "grist": this.grist = v; return v;
    case "info": this.info = v; return v;
    case "anim": this.anim = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "open": return open(Gd.A(a, 0));
    case "refresh": return refresh();
    case "_gui_input": return _gui_input(Gd.A(a, 0));
    case "_draw": return _draw();
    case "_lvl": return _lvl(Gd.A(a, 0));
    case "_text": return _text(Gd.A(a, 0), Gd.A(a, 1));
    case "_process": return _process(Gd.A(a, 0));
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    case "Art": return Art;
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_iodine(); return Gv.O(o); }
  public override string ScriptName { get { return "iodine"; } }
}
}
