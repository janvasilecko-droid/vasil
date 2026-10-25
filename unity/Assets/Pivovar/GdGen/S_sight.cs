// Vygenerováno z sight.gd překladačem gd2cs.py – needituj ručně.
using System; using System.Collections.Generic;
namespace Pivovar.Gd.Gen {
public partial class S_sight : GdNode {
  public Gv m;
  public Gv it;
  public Gv sample;
  public Gv acc;
  public Gv t_anim;
  public Gv parts;
  public Gv rng;
  public Gv next_burst;
  public Gv last_ch;
  public Gv last_dep;
  public static Gv Art;
  static S_sight() {
    Art = Gd.Script("art");
  }
  public S_sight() {
    TypeName = "Control";
    m = Gv.Null;
    it = Gv.Null;
    sample = Gd.Dict();
    acc = Gv.F(0.0);
    t_anim = Gv.F(0.0);
    parts = Gd.Arr();
    rng = Gd.New("RandomNumberGenerator");
    next_burst = Gv.F(6.0);
    last_ch = Gv.F(0.0);
    last_dep = Gv.F(0.0);
  }
  public Gv open(Gv item = default(Gv)) {
    it = item;
    Self.S("mouse_filter", Gd.StaticGet("Control", "MOUSE_FILTER_STOP"));
    Self.S("size", Gd.Ctor("Vector2", Gv.I(330L), Gv.I(450L)));
    Self.S("position", Gd.Ctor("Vector2", Gv.I(172L), Gv.I(226L)));
    refresh();
    return Gv.Null;
  }
  public Gv _gui_input(Gv e = default(Gv)) {
    if (((Gv.Is(e, "InputEventMouseButton") && (e.G("pressed")).T()) && Gv.Eq(e.G("button_index"), Gd.K("MOUSE_BUTTON_LEFT")))) {
      if ((Gd.Ctor("Rect2", (Self.G("size").G("x") - Gv.I(34L)), Gv.I(4L), Gv.I(28L), Gv.I(28L)).C("has_point", e.G("position"))).T()) {
        Self.C("queue_free");
      }
      Self.C("accept_event");
    }
    return Gv.Null;
  }
  public Gv _process(Gv d = default(Gv)) {
    d = (d).AsF();
    acc = (acc + d);
    t_anim = (t_anim + d);
    _parts_step(d);
    if (Gv.Ge(acc, Gv.F(0.25))) {
      acc = Gv.F(0.0);
      refresh();
    }
    Self.C("queue_redraw");
    return Gv.Null;
  }
  public Gv _burst(Gv n = default(Gv), Gv big = default(Gv)) {
    n = (n).AsI();
    foreach (Gv i in (n).Iter()) {
      parts.C("append", Gd.Dict(Gv.S("x"), (Gv.Eq(sample.C("get", Gv.S("from"), Gv.S("a")), Gv.S("a")) ? Gv.F(-106.0) : Gv.F(106.0)), Gv.S("y"), rng.C("randf_range", Gv.F(-70.0), Gv.F(70.0)), Gv.S("sz"), rng.C("randf_range", Gv.F(4.0), ((big).T() ? Gv.F(9.0) : Gv.F(6.5))), Gv.S("rot"), (rng.C("randf") * Gd.K("TAU")), Gv.S("rs"), rng.C("randf_range", Gv.F(-2.5), Gv.F(2.5)), Gv.S("k"), (Gv.Lt(rng.C("randf"), Gv.F(0.72)) ? Gv.I(0L) : Gv.I(1L)), Gv.S("v"), rng.C("randf_range", Gv.F(0.7), Gv.F(1.25))));
    }
    return Gv.Null;
  }
  public Gv _parts_step(Gv d = default(Gv)) {
    d = (d).AsF();
    if ((((Gv.Eq(m, Gv.Null) || Gv.Eq(m.G("sim"), Gv.Null)) || (sample.C("get", Gv.S("empty"), Gv.B(true))).T()) || (sample.C("get", Gv.S("none"), Gv.B(false))).T())) {
      parts.C("clear");
      return Gv.Null;
    }
    Gv S = m.G("sim").G("S");
    Gv flow = (sample.C("get", Gv.S("flow"), Gv.F(0.0))).AsF();
    Gv haze = (Gd.clampf((sample.C("get", Gv.S("fines"), Gv.F(0.0)) * Gv.F(400.0)), Gv.F(0.0), Gv.F(1.0))).AsF();
    Gv dir = ((Gv.Eq(sample.C("get", Gv.S("from"), Gv.S("a")), Gv.S("a")) ? Gv.F(1.0) : Gv.F(-1.0))).AsF();
    Gv spd = (((flow / Gv.F(60.0)) * Gv.F(90.0))).AsF();
    if ((Gv.Gt(flow, Gv.F(0.5)) && Gv.In(sample.C("get", Gv.S("kind"), Gv.S("")), Gd.Arr(Gv.S("wort"), Gv.S("mash"))))) {
      if (Gv.Lt(rng.C("randf"), ((haze * Gv.F(5.0)) * d))) {
        _burst(Gv.I(1L), Gv.B(false));
      }
      next_burst = (next_burst - d);
      if (Gv.Le(next_burst, Gv.F(0.0))) {
        next_burst = (rng.C("randf_range", Gv.F(5.0), Gv.F(16.0)) / ((Gv.F(1.0) + (haze * Gv.F(2.0)))));
        _burst((rng.C("randi_range", Gv.I(1L), Gv.I(3L)) + ((Gv.Gt(haze, Gv.F(0.3)) ? rng.C("randi_range", Gv.I(2L), Gv.I(5L)) : Gv.I(0L)))), Gv.B(false));
      }
      Gv ch = (S.G("sk").G("ch")).AsF();
      Gv dep = (S.G("sk").C("get", Gv.S("dep"), Gv.F(0.0))).AsF();
      if (Gv.Gt(ch, (last_ch + Gv.F(0.001)))) {
        _burst(rng.C("randi_range", Gv.I(10L), Gv.I(22L)), Gv.B(true));
      }
      if (Gv.Lt(dep, (last_dep - Gv.F(0.01)))) {
        _burst(rng.C("randi_range", Gv.I(8L), Gv.I(18L)), Gv.B(true));
      }
      last_ch = ch;
      last_dep = dep;
    }
    foreach (Gv p__it in (parts).Iter()) {
      Gv p = p__it;
      if (Gv.Gt(flow, Gv.F(0.5))) {
        p = p.S("x", (p.G("x") + (((spd * dir) * p.G("v")) * d)));
      } else {
        p = p.S("y", (p.G("y") + (Gv.F(6.0) * d)));
      }
      p = p.S("y", (p.G("y") + ((Gd.sin(((t_anim * Gv.F(3.0)) + p.G("rot"))) * Gv.F(6.0)) * d)));
      p = p.S("rot", (p.G("rot") + (p.G("rs") * d)));
    }
    parts = parts.C("filter", Gd.Fn((_a) => { Gv p_2 = Gd.A(_a, 0); return Gv.B((Gv.Lt(Gd.absf(p_2.G("x")), Gv.F(112.0)) && Gv.Lt(Gd.absf(p_2.G("y")), Gv.F(112.0)))); return Gv.Null; }));
    if (Gv.Gt(parts.C("size"), Gv.I(90L))) {
      parts = parts.C("slice", (parts.C("size") - Gv.I(90L)));
    }
    return Gv.Null;
  }
  public Gv take_sample(Gv pos = default(Gv)) {
    Gv sim = m.G("sim");
    Gv best = Gv.Null;
    Gv bd = Gv.F(40.0);
    Gv bt = Gv.F(0.0);
    foreach (Gv e in (sim.G("net").G("edges")).Iter()) {
      Gv pts = e.G("pts");
      Gv tot = Gv.F(0.0);
      foreach (Gv i in ((pts.C("size") - Gv.I(1L))).Iter()) {
        tot = (tot + pts.I(i).C("distance_to", pts.I((i + Gv.I(1L)))));
      }
      Gv run = Gv.F(0.0);
      foreach (Gv i_2 in ((pts.C("size") - Gv.I(1L))).Iter()) {
        Gv a = pts.I(i_2);
        Gv b = pts.I((i_2 + Gv.I(1L)));
        Gv c = Gd.Static("Geometry2D", "get_closest_point_to_segment", pos, a, b);
        Gv dd = c.C("distance_to", pos);
        if (Gv.Lt(dd, bd)) {
          bd = dd;
          best = e;
          bt = (((run + a.C("distance_to", c))) / Gd.maxf(Gv.F(1.0), tot));
        }
        run = (run + a.C("distance_to", b));
      }
    }
    if (Gv.Eq(best, Gv.Null)) {
      return Gd.Dict(Gv.S("none"), Gv.B(true));
    }
    Gv e_2 = best;
    Gv total = (sim.C("vol", e_2.G("c"))).AsF();
    Gv x = ((bt * e_2.G("cap"))).AsF();
    Gv lo = ((Gv.Eq(e_2.G("from"), Gv.S("a")) ? Gv.F(0.0) : (e_2.G("cap") - total))).AsF();
    Gv rel = ((x - lo)).AsF();
    Gv found = Gv.Null;
    if ((Gv.Ge(rel, Gv.F(0.0)) && Gv.Le(rel, total))) {
      Gv cum = Gv.F(0.0);
      foreach (Gv b_2 in (e_2.G("q")).Iter()) {
        Gv vb = (sim.C("vol", b_2)).AsF();
        if (Gv.Le(rel, ((cum + vb) + Gv.F(1e-09)))) {
          found = b_2;
          break;
        }
        cum = (cum + vb);
      }
    }
    Gv s = Gd.Dict(Gv.S("edge"), e_2, Gv.S("flow"), e_2.G("flow"), Gv.S("from"), e_2.G("from"), Gv.S("cap"), e_2.G("cap"), Gv.S("filled"), total);
    if (Gv.Eq(found, Gv.Null)) {
      s = s.SI(Gv.S("empty"), Gv.B(true));
      return s;
    }
    Gv V = (Gd.maxf(Gv.F(1e-09), sim.C("vol", found))).AsF();
    s = s.SI(Gv.S("empty"), Gv.B(false));
    s = s.SI(Gv.S("kind"), sim.C("contentKind", found));
    s = s.SI(Gv.S("T"), found.G("T"));
    s = s.SI(Gv.S("P"), sim.C("plato", found));
    s = s.SI(Gv.S("fines"), (found.G("fines") / V));
    s = s.SI(Gv.S("air"), (found.G("air") / V));
    s = s.SI(Gv.S("col"), (found.G("col") / Gd.maxf(Gv.F(0.01), sim.C("liqVol", found))));
    return s;
    return Gv.Null;
  }
  public Gv refresh() {
    if (((Gv.Eq(it, Gv.Null) || Gv.Eq(m, Gv.Null)) || Gv.Eq(m.G("sim"), Gv.Null))) {
      return Gv.Null;
    }
    sample = take_sample(Gd.Ctor("Vector2", it.G("x"), it.G("y")));
    return Gv.Null;
  }
  public Gv _draw() {
    Gv f = m.G("font");
    Gv fb = m.G("font_b");
    S_art.rrect_border(Self, Gd.Ctor("Rect2", Gd.StaticGet("Vector2", "ZERO"), Self.G("size")), Gd.Ctor("Color", Gv.F(0.07), Gv.F(0.085), Gv.F(0.1), Gv.F(0.97)), Gd.Ctor("Color", Gv.S("f2c230")), Gv.I(2L), Gv.F(8.0));
    Self.C("draw_string", fb, Gd.Ctor("Vector2", Gv.I(12L), Gv.I(22L)), Gv.S("PRŮHLEDÍTKO"), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(15L), Gd.Ctor("Color", Gv.S("f2c230")));
    Self.C("draw_string", f, Gd.Ctor("Vector2", Gv.I(12L), Gv.I(38L)), Gd.Dyn("str", it.C("get", Gv.S("label"), Gv.S(""))), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(250L), Gv.I(11L), Gd.Ctor("Color", Gv.S("9aa3ab")));
    Self.C("draw_string", fb, Gd.Ctor("Vector2", (Self.G("size").G("x") - Gv.I(26L)), Gv.I(22L)), Gv.S("X"), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(18L), Gd.Ctor("Color", Gv.S("e6ebef")));
    Gv c = Gd.Ctor("Vector2", (Self.G("size").G("x") * Gv.F(0.5)), Gv.I(156L));
    Gv R = Gv.F(106.0);
    _frame(c, R);
    if ((sample.C("get", Gv.S("none"), Gv.B(false))).T()) {
      Self.C("draw_circle", c, R, Gd.Ctor("Color", Gv.F(0.15), Gv.F(0.18), Gv.F(0.2)));
      Self.C("draw_string", f, Gd.Ctor("Vector2", Gv.I(14L), (Self.G("size").G("y") - Gv.I(14L))), Gv.S("Průhledítko není na potrubí."), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(13L), Gd.Ctor("Color", Gv.S("ff6b5b")));
      return Gv.Null;
    }
    Gv flow = (sample.C("get", Gv.S("flow"), Gv.F(0.0))).AsF();
    if ((sample.C("get", Gv.S("empty"), Gv.B(true))).T()) {
      Self.C("draw_circle", c, R, Gd.Ctor("Color", Gv.F(0.45), Gv.F(0.52), Gv.F(0.58), Gv.F(0.35)));
      _glass(c, R);
      _lines(Gd.Arr(Gv.S("Sklo je prázdné – v tomto místě potrubí nic není.")), Gd.Arr(Gv.S("e6ebef")));
      _flow_arrow(c, R, flow);
      return Gv.Null;
    }
    Gv col = m.C("kind_color", sample.G("kind"), sample.G("T"), sample.G("col"));
    col = col.S("a", Gv.F(1.0));
    Self.C("draw_circle", c, R, col.C("darkened", Gv.F(0.28)));
    Self.C("draw_circle", c, (R * Gv.F(0.86)), col.C("darkened", Gv.F(0.12)));
    Self.C("draw_circle", c, (R * Gv.F(0.6)), col);
    Self.C("draw_circle", c, (R * Gv.F(0.3)), col.C("lightened", Gv.F(0.1)));
    Gv haze = (Gd.clampf((sample.G("fines") * Gv.F(400.0)), Gv.F(0.0), Gv.F(1.0))).AsF();
    Self.C("draw_circle", c, R, Gd.Ctor("Color", Gv.F(0.93), Gv.F(0.89), Gv.F(0.78), (haze * Gv.F(0.7))));
    Gv spd = (((flow / Gv.F(60.0)) * Gv.F(90.0))).AsF();
    Gv dir = ((Gv.Eq(sample.G("from"), Gv.S("a")) ? Gv.F(1.0) : Gv.F(-1.0))).AsF();
    Gv n = (Gd.Dyn("int", (haze * Gv.F(220.0)))).AsI();
    foreach (Gv i in (n).Iter()) {
      Gv rx = ((Gd.fposmod(((Gd.Dyn("float", i) * Gv.F(37.13)) + ((t_anim * spd) * dir)), (Gv.F(2.0) * R)) - R)).AsF();
      Gv ry = ((Gd.fposmod((Gd.Dyn("float", i) * Gv.F(91.7)), (Gv.F(2.0) * R)) - R)).AsF();
      if (Gv.Lt(((rx * rx) + (ry * ry)), (((R - Gv.F(6.0))) * ((R - Gv.F(6.0)))))) {
        Self.C("draw_circle", (c + Gd.Ctor("Vector2", rx, ry)), (Gv.F(1.0) + Gd.fposmod((Gd.Dyn("float", i) * Gv.F(0.37)), Gv.F(1.8))), Gd.Ctor("Color", Gv.F(0.98), Gv.F(0.95), Gv.F(0.85), Gv.F(0.5)));
      }
    }
    foreach (Gv p in (parts).Iter()) {
      Gv pp = (c + Gd.Ctor("Vector2", p.G("x"), p.G("y")));
      if (Gv.Gt(pp.C("distance_to", c), (R - Gv.F(9.0)))) {
        continue;
      }
      Gv dv = (Gd.Ctor("Vector2", Gd.cos(p.G("rot")), Gd.sin(p.G("rot"))) * Gd.Dyn("float", p.G("sz")));
      Gv nv = (Gd.Ctor("Vector2", (-dv.G("y")), dv.G("x")) * Gv.F(0.28));
      if (Gv.Eq(p.G("k"), Gv.I(0L))) {
        Self.C("draw_colored_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr((pp - dv), ((pp - (nv * Gv.F(0.6))) + (dv * Gv.F(0.0))), (pp + dv), (pp + (nv * Gv.F(0.6))))), Gd.Ctor("Color", Gv.S("d9be82")));
        Self.C("draw_line", (pp - dv), (pp + dv), Gd.Ctor("Color", Gv.S("8a6a3a")), Gv.F(1.0), Gv.B(true));
      } else {
        Self.C("draw_circle", pp, (p.G("sz") * Gv.F(0.42)), Gd.Ctor("Color", Gv.S("6f4f28")));
        Self.C("draw_circle", (pp - Gd.Ctor("Vector2", Gv.I(1L), Gv.I(1L))), (p.G("sz") * Gv.F(0.2)), Gd.Ctor("Color", Gv.S("a98350")));
      }
    }
    Gv nb = (Gd.Dyn("int", Gd.clampf((sample.G("air") * Gv.F(2500.0)), Gv.F(0.0), Gv.F(40.0)))).AsI();
    foreach (Gv i_2 in (nb).Iter()) {
      Gv bx = ((Gd.fposmod(((Gd.Dyn("float", i_2) * Gv.F(53.7)) + ((t_anim * spd) * dir)), (Gv.F(2.0) * R)) - R)).AsF();
      Gv by = ((Gd.fposmod(((Gd.Dyn("float", i_2) * Gv.F(29.3)) - (t_anim * Gv.F(30.0))), (Gv.F(2.0) * R)) - R)).AsF();
      if (Gv.Lt(((bx * bx) + (by * by)), (((R - Gv.F(8.0))) * ((R - Gv.F(8.0)))))) {
        Self.C("draw_arc", (c + Gd.Ctor("Vector2", bx, by)), (Gv.F(3.0) + Gd.fposmod((Gd.Dyn("float", i_2) * Gv.F(0.7)), Gv.F(3.0))), Gv.I(0L), Gd.K("TAU"), Gv.I(12L), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.8)), Gv.F(1.5), Gv.B(true));
      }
    }
    if (Gv.Gt(flow, Gv.F(0.5))) {
      foreach (Gv i_3 in (Gv.I(6L)).Iter()) {
        Gv sy = ((((-R) * Gv.F(0.75)) + ((i_3 * R) * Gv.F(0.3)))).AsF();
        Gv hw = (Gd.sqrt(Gd.maxf(Gv.F(0.0), (((R * R) * Gv.F(0.9)) - (sy * sy))))).AsF();
        Gv sx = ((Gd.fposmod(((((t_anim * spd) * dir) * Gv.F(1.6)) + (i_3 * Gv.F(41.0))), (Gv.F(2.0) * hw)) - hw)).AsF();
        Self.C("draw_line", (c + Gd.Ctor("Vector2", (sx - Gv.F(12.0)), sy)), (c + Gd.Ctor("Vector2", (sx + Gv.F(12.0)), sy)), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.1)), Gv.F(2.0), Gv.B(true));
      }
    }
    _glass(c, R);
    _flow_arrow(c, R, flow);
    Gv lines = Gd.Arr();
    Gv cols = Gd.Arr();
    Gv kind_cz = Gd.Dict(Gv.S("wort"), Gv.S("sladina / mladina"), Gv.S("mash"), Gv.S("rmut (s pevnými částicemi)"), Gv.S("water"), Gv.S("voda"), Gv.S("hot"), Gv.S("horká voda"), Gv.S("ice"), Gv.S("ledová voda")).C("get", sample.G("kind"), sample.G("kind"));
    lines.C("append", (Gv.S("Kapalina: ") + kind_cz));
    cols.C("append", Gv.S("e6ebef"));
    lines.C("append", (Gv.S("Teplota: %s °C   Extrakt: %s °P") % Gd.Arr(m.C("c1", sample.G("T")), m.C("c1", sample.G("P")))));
    cols.C("append", Gv.S("e6ebef"));
    if (Gv.Gt(flow, Gv.F(0.5))) {
      lines.C("append", (Gv.S("Průtok: %d L/min (%s)") % Gd.Arr(Gd.roundi(flow), (Gv.Eq(sample.G("from"), Gv.S("a")) ? Gv.S("a → b") : Gv.S("b → a")))));
      cols.C("append", Gv.S("7ee08a"));
    } else {
      lines.C("append", Gv.S("Kapalina STOJÍ."));
      cols.C("append", Gv.S("f2c230"));
    }
    Gv hz = (Gv.Lt(haze, Gv.F(0.08)) ? Gv.S("čirá, jiskrná") : ((Gv.Lt(haze, Gv.F(0.2)) ? Gv.S("lehce opalizující") : ((Gv.Lt(haze, Gv.F(0.5)) ? Gv.S("zakalená") : ((Gv.Lt(haze, Gv.F(0.75)) ? Gv.S("velmi zakalená") : Gv.S("kalná, slupky"))))))));
    lines.C("append", (Gv.S("Čirost: %s (%s g/l jemných částic)") % Gd.Arr(hz, m.C("c1", (sample.G("fines") * Gv.F(1000.0))))));
    cols.C("append", (Gv.Lt(haze, Gv.F(0.2)) ? Gv.S("7ee08a") : ((Gv.Lt(haze, Gv.F(0.5)) ? Gv.S("f2c230") : Gv.S("ff6b5b")))));
    lines.C("append", (Gv.S("Vzduch: %s") % ((Gv.Gt(nb, Gv.I(0L)) ? Gv.S("bubliny (zavzdušnění, odhad)") : Gv.S("žádný")))));
    cols.C("append", (Gv.Gt(nb, Gv.I(0L)) ? Gv.S("ff6b5b") : Gv.S("9aa3ab")));
    if ((Gv.Eq(sample.G("kind"), Gv.S("wort")) && Gv.Gt(flow, Gv.F(0.5)))) {
      lines.C("append", (Gv.Ge(haze, Gv.F(0.2)) ? Gv.S("Vorlauf hotový, až je sladina čirá.") : Gv.S("Čirá – můžeš scezovat do pánve.")));
      cols.C("append", (Gv.Ge(haze, Gv.F(0.2)) ? Gv.S("9aa3ab") : Gv.S("7ee08a")));
    }
    _lines(lines, cols);
    Gv by0 = (Self.G("size").G("y") - Gv.F(26.0));
    Self.C("draw_rect", Gd.Ctor("Rect2", Gv.I(14L), by0, (Self.G("size").G("x") - Gv.I(28L)), Gv.I(8L)), Gd.Ctor("Color", Gv.F(0.15), Gv.F(0.18), Gv.F(0.2)));
    Gv gr = Gd.Ctor("Color", Gv.S("3fbf5f")).C("lerp", Gd.Ctor("Color", Gv.S("e0a020")), Gd.clampf((haze * Gv.F(2.0)), Gv.F(0.0), Gv.F(1.0))).C("lerp", Gd.Ctor("Color", Gv.S("e0452f")), Gd.clampf(((haze * Gv.F(2.0)) - Gv.F(1.0)), Gv.F(0.0), Gv.F(1.0)));
    Self.C("draw_rect", Gd.Ctor("Rect2", Gv.I(14L), by0, (((Self.G("size").G("x") - Gv.I(28L))) * haze), Gv.I(8L)), gr);
    Self.C("draw_string", f, Gd.Ctor("Vector2", Gv.I(14L), (by0 - Gv.I(3L))), Gv.S("zákal"), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(10L), Gd.Ctor("Color", Gv.S("9aa3ab")));
    return Gv.Null;
  }
  public Gv _frame(Gv c = default(Gv), Gv R = default(Gv)) {
    R = (R).AsF();
    Self.C("draw_circle", c, (R + Gv.I(16L)), Gd.Ctor("Color", Gv.S("1b2025")));
    Self.C("draw_circle", c, (R + Gv.I(14L)), Gd.Ctor("Color", Gv.S("8e98a1")));
    Self.C("draw_circle", c, (R + Gv.I(10L)), Gd.Ctor("Color", Gv.S("c3ccd3")));
    Self.C("draw_circle", c, (R + Gv.I(5L)), Gd.Ctor("Color", Gv.S("3a434b")));
    Self.C("draw_circle", c, (R + Gv.I(2L)), Gd.Ctor("Color", Gv.S("111417")));
    foreach (Gv i in (Gv.I(8L)).Iter()) {
      Gv a = (((Gd.K("TAU") * i) / Gv.F(8.0)) + Gv.F(0.2));
      Gv p = (c + (Gd.Ctor("Vector2", Gd.cos(a), Gd.sin(a)) * ((R + Gv.F(10.0)))));
      Self.C("draw_circle", p, Gv.F(3.2), Gd.Ctor("Color", Gv.S("59626a")));
      Self.C("draw_circle", p, Gv.F(1.6), Gd.Ctor("Color", Gv.S("aeb7bf")));
    }
    return Gv.Null;
  }
  public Gv _glass(Gv c = default(Gv), Gv R = default(Gv)) {
    R = (R).AsF();
    Self.C("draw_arc", c, (R - Gv.F(1.0)), Gv.I(0L), Gd.K("TAU"), Gv.I(64L), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.35)), Gv.F(4.0), Gv.B(true));
    Self.C("draw_arc", c, (R - Gv.F(5.0)), (Gd.K("PI") * Gv.F(1.05)), (Gd.K("PI") * Gv.F(1.55)), Gv.I(20L), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.45)), Gv.F(4.0), Gv.B(true));
    Self.C("draw_arc", c, (R - Gv.F(12.0)), (Gd.K("PI") * Gv.F(1.15)), (Gd.K("PI") * Gv.F(1.4)), Gv.I(12L), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.25)), Gv.F(3.0), Gv.B(true));
    Self.C("draw_circle", (c + Gd.Ctor("Vector2", (R * Gv.F(0.45)), (R * Gv.F(0.5)))), (R * Gv.F(0.08)), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.12)));
    return Gv.Null;
  }
  public Gv _lines(Gv lines = default(Gv), Gv cols = default(Gv)) {
    Gv y = Gv.F(318.0);
    foreach (Gv i in (lines.C("size")).Iter()) {
      Self.C("draw_string", m.G("font"), Gd.Ctor("Vector2", Gv.I(14L), y), lines.I(i), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), (Self.G("size").G("x") - Gv.I(28L)), Gv.I(12L), Gd.Ctor("Color", cols.I(i)));
      y = (y + Gv.F(18.0));
    }
    return Gv.Null;
  }
  public Gv _flow_arrow(Gv c = default(Gv), Gv R = default(Gv), Gv flow = default(Gv)) {
    R = (R).AsF();
    flow = (flow).AsF();
    if (Gv.Le(flow, Gv.F(0.5))) {
      return Gv.Null;
    }
    Gv up = Gv.B(Gv.Eq(sample.C("get", Gv.S("from"), Gv.S("a")), Gv.S("a")));
    Gv p = (c + Gd.Ctor("Vector2", Gv.I(0L), (R + Gv.I(26L))));
    Gv dx = (Gv.F(30.0) * (((up).T() ? Gv.F(1.0) : Gv.F(-1.0))));
    Self.C("draw_line", (p - Gd.Ctor("Vector2", dx, Gv.I(0L))), (p + Gd.Ctor("Vector2", dx, Gv.I(0L))), Gd.Ctor("Color", Gv.S("7ee08a")), Gv.F(3.0), Gv.B(true));
    Self.C("draw_colored_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr((p + Gd.Ctor("Vector2", dx, Gv.I(0L))), (p + Gd.Ctor("Vector2", (dx * Gv.F(0.6)), Gv.I(-8L))), (p + Gd.Ctor("Vector2", (dx * Gv.F(0.6)), Gv.I(8L))))), Gd.Ctor("Color", Gv.S("7ee08a")));
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "m": return this.m;
    case "it": return this.it;
    case "sample": return this.sample;
    case "acc": return this.acc;
    case "t_anim": return this.t_anim;
    case "parts": return this.parts;
    case "rng": return this.rng;
    case "next_burst": return this.next_burst;
    case "last_ch": return this.last_ch;
    case "last_dep": return this.last_dep;
    case "Art": return S_sight.Art;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "m": this.m = v; return v;
    case "it": this.it = v; return v;
    case "sample": this.sample = v; return v;
    case "acc": this.acc = v; return v;
    case "t_anim": this.t_anim = v; return v;
    case "parts": this.parts = v; return v;
    case "rng": this.rng = v; return v;
    case "next_burst": this.next_burst = v; return v;
    case "last_ch": this.last_ch = v; return v;
    case "last_dep": this.last_dep = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "open": return open(Gd.A(a, 0));
    case "_gui_input": return _gui_input(Gd.A(a, 0));
    case "_process": return _process(Gd.A(a, 0));
    case "_burst": return _burst(Gd.A(a, 0), Gd.A(a, 1));
    case "_parts_step": return _parts_step(Gd.A(a, 0));
    case "take_sample": return take_sample(Gd.A(a, 0));
    case "refresh": return refresh();
    case "_draw": return _draw();
    case "_frame": return _frame(Gd.A(a, 0), Gd.A(a, 1));
    case "_glass": return _glass(Gd.A(a, 0), Gd.A(a, 1));
    case "_lines": return _lines(Gd.A(a, 0), Gd.A(a, 1));
    case "_flow_arrow": return _flow_arrow(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    case "Art": return Art;
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_sight(); return Gv.O(o); }
  public override string ScriptName { get { return "sight"; } }
}
}
