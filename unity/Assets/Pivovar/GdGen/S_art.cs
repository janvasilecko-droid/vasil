// Vygenerováno z art.gd překladačem gd2cs.py – needituj ručně.
using System; using System.Collections.Generic;
namespace Pivovar.Gd.Gen {
public partial class S_art : GdObject {
  public static Gv STAINLESS;
  public static Gv COPPER;
  public static Gv CLAD;
  public static Gv NAVY;
  public static Gv PIPE_STEEL;
  public static Gv _sb;
  public static Gv _tx;
  static S_art() {
    STAINLESS = Gd.Arr(Gd.Arr(Gv.F(0.0), Gv.S("2c3237")), Gd.Arr(Gv.F(0.07), Gv.S("5f6870")), Gd.Arr(Gv.F(0.18), Gv.S("aeb7bf")), Gd.Arr(Gv.F(0.27), Gv.S("eef3f6")), Gd.Arr(Gv.F(0.31), Gv.S("ffffff")), Gd.Arr(Gv.F(0.36), Gv.S("dde3e8")), Gd.Arr(Gv.F(0.52), Gv.S("b3bcc4")), Gd.Arr(Gv.F(0.72), Gv.S("858e96")), Gd.Arr(Gv.F(0.88), Gv.S("5a636b")), Gd.Arr(Gv.F(1.0), Gv.S("2a3035")));
    COPPER = Gd.Arr(Gd.Arr(Gv.F(0.0), Gv.S("4a230e")), Gd.Arr(Gv.F(0.15), Gv.S("8c4620")), Gd.Arr(Gv.F(0.32), Gv.S("f0b07a")), Gd.Arr(Gv.F(0.45), Gv.S("c47a42")), Gd.Arr(Gv.F(0.75), Gv.S("7e3d18")), Gd.Arr(Gv.F(1.0), Gv.S("3a1a08")));
    CLAD = Gd.Arr(Gd.Arr(Gv.F(0.0), Gv.S("7d858c")), Gd.Arr(Gv.F(0.16), Gv.S("bcc3c9")), Gd.Arr(Gv.F(0.3), Gv.S("f1f4f6")), Gd.Arr(Gv.F(0.34), Gv.S("ffffff")), Gd.Arr(Gv.F(0.42), Gv.S("e6eaed")), Gd.Arr(Gv.F(0.65), Gv.S("c9cfd4")), Gd.Arr(Gv.F(0.88), Gv.S("98a0a7")), Gd.Arr(Gv.F(1.0), Gv.S("6f777e")));
    NAVY = Gd.Arr(Gd.Arr(Gv.F(0.0), Gv.S("16233a")), Gd.Arr(Gv.F(0.25), Gv.S("2c4a78")), Gd.Arr(Gv.F(0.4), Gv.S("4f74a8")), Gd.Arr(Gv.F(0.6), Gv.S("2f4f80")), Gd.Arr(Gv.F(1.0), Gv.S("142035")));
    PIPE_STEEL = Gd.Ctor("Color", Gv.S("a6b0b9"));
    _sb = Gd.Dict();
    _tx = Gd.Dict();
  }
  public S_art() {
  }
  public static Gv grad(Gv stops = default(Gv), Gv t = default(Gv)) {
    t = (t).AsF();
    t = (Gd.clampf(t, Gv.F(0.0), Gv.F(1.0))).AsF();
    foreach (Gv i in Gd.RangeIter(Gv.I(1L), stops.C("size"))) {
      if (Gv.Le(t, stops.I(i).I(Gv.I(0L)))) {
        Gv a = stops.I((i - Gv.I(1L)));
        Gv b = stops.I(i);
        Gv k = ((((t - a.I(Gv.I(0L)))) / Gd.maxf(Gv.F(0.0001), (b.I(Gv.I(0L)) - a.I(Gv.I(0L)))))).AsF();
        return Gd.Ctor("Color", a.I(Gv.I(1L))).C("lerp", Gd.Ctor("Color", b.I(Gv.I(1L))), k);
      }
    }
    return Gd.Ctor("Color", stops.I((stops.C("size") - Gv.I(1L))).I(Gv.I(1L)));
    return Gv.Null;
  }
  public static Gv cyl_v(Gv ci = default(Gv), Gv r = default(Gv), Gv stops = default(Gv), Gv n = default(Gv)) {
    if (n.k == 0) n = Gv.I(24L);
    foreach (Gv i in (n).Iter()) {
      Gv t0 = (Gd.Dyn("float", i) / n);
      Gv t1 = (Gd.Dyn("float", (i + Gv.I(1L))) / n);
      Gv x0 = (r.G("position").G("x") + (r.G("size").G("x") * t0));
      Gv x1 = (r.G("position").G("x") + (r.G("size").G("x") * t1));
      Gv c0 = grad(stops, t0);
      Gv c1 = grad(stops, t1);
      ci.C("draw_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", x0, r.G("position").G("y")), Gd.Ctor("Vector2", x1, r.G("position").G("y")), Gd.Ctor("Vector2", x1, r.G("end").G("y")), Gd.Ctor("Vector2", x0, r.G("end").G("y")))), Gd.Ctor("PackedColorArray", Gd.Arr(c0, c1, c1, c0)));
    }
    return Gv.Null;
  }
  public static Gv cyl_h(Gv ci = default(Gv), Gv r = default(Gv), Gv stops = default(Gv), Gv n = default(Gv)) {
    if (n.k == 0) n = Gv.I(20L);
    foreach (Gv i in (n).Iter()) {
      Gv t0 = (Gd.Dyn("float", i) / n);
      Gv t1 = (Gd.Dyn("float", (i + Gv.I(1L))) / n);
      Gv y0 = (r.G("position").G("y") + (r.G("size").G("y") * t0));
      Gv y1 = (r.G("position").G("y") + (r.G("size").G("y") * t1));
      Gv c0 = grad(stops, t0);
      Gv c1 = grad(stops, t1);
      ci.C("draw_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", r.G("position").G("x"), y0), Gd.Ctor("Vector2", r.G("end").G("x"), y0), Gd.Ctor("Vector2", r.G("end").G("x"), y1), Gd.Ctor("Vector2", r.G("position").G("x"), y1))), Gd.Ctor("PackedColorArray", Gd.Arr(c0, c0, c1, c1)));
    }
    return Gv.Null;
  }
  public static Gv cone(Gv ci = default(Gv), Gv cx = default(Gv), Gv y = default(Gv), Gv w = default(Gv), Gv wb = default(Gv), Gv h = default(Gv), Gv stops = default(Gv), Gv n = default(Gv)) {
    cx = (cx).AsF();
    y = (y).AsF();
    w = (w).AsF();
    wb = (wb).AsF();
    h = (h).AsF();
    if (n.k == 0) n = Gv.I(20L);
    foreach (Gv i in (n).Iter()) {
      Gv t0 = (Gd.Dyn("float", i) / n);
      Gv t1 = (Gd.Dyn("float", (i + Gv.I(1L))) / n);
      Gv a0 = ((cx - (w * Gv.F(0.5))) + (w * t0));
      Gv a1 = ((cx - (w * Gv.F(0.5))) + (w * t1));
      Gv b0 = ((cx - (wb * Gv.F(0.5))) + (wb * t0));
      Gv b1 = ((cx - (wb * Gv.F(0.5))) + (wb * t1));
      Gv c0 = grad(stops, t0).C("darkened", Gv.F(0.08));
      Gv c1 = grad(stops, t1).C("darkened", Gv.F(0.08));
      ci.C("draw_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", a0, y), Gd.Ctor("Vector2", a1, y), Gd.Ctor("Vector2", b1, (y + h)), Gd.Ctor("Vector2", b0, (y + h)))), Gd.Ctor("PackedColorArray", Gd.Arr(c0, c1, c1, c0)));
    }
    return Gv.Null;
  }
  public static Gv dome(Gv ci = default(Gv), Gv cx = default(Gv), Gv y = default(Gv), Gv w = default(Gv), Gv h = default(Gv), Gv stops = default(Gv), Gv n = default(Gv)) {
    cx = (cx).AsF();
    y = (y).AsF();
    w = (w).AsF();
    h = (h).AsF();
    if (n.k == 0) n = Gv.I(24L);
    foreach (Gv i in (n).Iter()) {
      Gv t0 = (Gd.Dyn("float", i) / n);
      Gv t1 = (Gd.Dyn("float", (i + Gv.I(1L))) / n);
      Gv x0 = ((cx - (w * Gv.F(0.5))) + (w * t0));
      Gv x1 = ((cx - (w * Gv.F(0.5))) + (w * t1));
      Gv e0 = Gd.sqrt(Gd.maxf(Gv.F(0.0), (Gv.F(1.0) - Gd.pow(((t0 * Gv.F(2.0)) - Gv.F(1.0)), Gv.I(2L)))));
      Gv e1 = Gd.sqrt(Gd.maxf(Gv.F(0.0), (Gv.F(1.0) - Gd.pow(((t1 * Gv.F(2.0)) - Gv.F(1.0)), Gv.I(2L)))));
      Gv c0 = grad(stops, t0);
      Gv c1 = grad(stops, t1);
      ci.C("draw_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", x0, y), Gd.Ctor("Vector2", x0, (y - (h * e0))), Gd.Ctor("Vector2", x1, (y - (h * e1))), Gd.Ctor("Vector2", x1, y))), Gd.Ctor("PackedColorArray", Gd.Arr(c0, c0.C("lightened", Gv.F(0.08)), c1.C("lightened", Gv.F(0.08)), c1)));
    }
    return Gv.Null;
  }
  public static Gv shadow(Gv ci = default(Gv), Gv r = default(Gv), Gv off = default(Gv), Gv spread = default(Gv)) {
    if (off.k == 0) off = Gd.Ctor("Vector2", Gv.I(10L), Gv.I(14L));
    if (spread.k == 0) spread = Gv.F(14.0);
    foreach (Gv i in (Gv.I(5L)).Iter()) {
      Gv g = ((spread * ((Gv.I(5L) - i))) / Gv.F(5.0));
      ci.C("draw_rect", Gd.Ctor("Rect2", ((r.G("position") + off) - (Gd.Ctor("Vector2", g, g) * Gv.F(0.5))), (r.G("size") + Gd.Ctor("Vector2", g, g))), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.07)));
    }
    return Gv.Null;
  }
  public static Gv ellipse_shadow(Gv ci = default(Gv), Gv c = default(Gv), Gv rx = default(Gv), Gv ry = default(Gv)) {
    rx = (rx).AsF();
    ry = (ry).AsF();
    foreach (Gv i in (Gv.I(5L)).Iter()) {
      Gv k = (Gv.F(1.0) + (i * Gv.F(0.12)));
      Gv pts = Gd.Ctor("PackedVector2Array");
      foreach (Gv a in (Gv.I(24L)).Iter()) {
        pts.C("append", (c + Gd.Ctor("Vector2", ((Gd.cos(((Gd.K("TAU") * a) / Gv.F(24.0))) * rx) * k), ((Gd.sin(((Gd.K("TAU") * a) / Gv.F(24.0))) * ry) * k))));
      }
      ci.C("draw_colored_polygon", pts, Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.06)));
    }
    return Gv.Null;
  }
  public static Gv rrect(Gv ci = default(Gv), Gv r = default(Gv), Gv col = default(Gv), Gv rad = default(Gv)) {
    if (rad.k == 0) rad = Gv.F(6.0);
    rrect_border(ci, r, col, col, Gv.I(0L), rad);
    return Gv.Null;
  }
  public static Gv rrect_border(Gv ci = default(Gv), Gv r = default(Gv), Gv col = default(Gv), Gv border = default(Gv), Gv bw = default(Gv), Gv rad = default(Gv)) {
    if (bw.k == 0) bw = Gv.I(2L);
    if (rad.k == 0) rad = Gv.F(6.0);
    Gv key = (Gv.S("%s|%s|%d|%d") % Gd.Arr(col.C("to_html"), border.C("to_html"), bw, Gd.Dyn("int", rad)));
    Gv sb = _sb.C("get", key);
    if (Gv.Eq(sb, Gv.Null)) {
      sb = Gd.New("StyleBoxFlat");
      sb = sb.S("bg_color", col);
      sb = sb.S("border_color", border);
      sb.C("set_border_width_all", bw);
      sb.C("set_corner_radius_all", Gd.Dyn("int", rad));
      sb = sb.S("anti_aliasing", Gv.B(true));
      _sb = _sb.SI(key, sb);
    }
    ci.C("draw_style_box", sb, r);
    return Gv.Null;
  }
  public static Gv pipe(Gv ci = default(Gv), Gv pts = default(Gv), Gv w = default(Gv)) {
    w = (w).AsF();
    ci.C("draw_polyline", pts, Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.16)), (w + Gv.F(9.0)), Gv.B(true));
    ci.C("draw_polyline", pts, Gd.Ctor("Color", Gv.S("1b2025")), (w + Gv.F(3.0)), Gv.B(true));
    ci.C("draw_polyline", pts, PIPE_STEEL, (w + Gv.F(1.0)), Gv.B(true));
    ci.C("draw_polyline", pts, Gd.Ctor("Color", Gv.S("d9e0e5")), Gd.maxf(Gv.F(1.5), (w * Gv.F(0.45))), Gv.B(true));
    ci.C("draw_polyline", pts, Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.85)), Gd.maxf(Gv.F(1.0), (w * Gv.F(0.14))), Gv.B(true));
    if (Gv.Gt(w, Gv.F(8.0))) {
      ci.C("draw_polyline", pts, Gd.Ctor("Color", Gv.S("2a3137")), (w - Gv.F(5.0)), Gv.B(true));
      ci.C("draw_polyline", pts, Gd.Ctor("Color", Gv.S("39424a")), Gd.maxf(Gv.F(1.0), (w - Gv.F(9.0))), Gv.B(true));
    }
    return Gv.Null;
  }
  public static Gv floor_shadow(Gv ci = default(Gv), Gv c = default(Gv), Gv rx = default(Gv), Gv ry = default(Gv)) {
    rx = (rx).AsF();
    ry = (ry).AsF();
    foreach (Gv i in (Gv.I(6L)).Iter()) {
      Gv k = (Gv.F(1.0) + (i * Gv.F(0.16)));
      Gv pts = Gd.Ctor("PackedVector2Array");
      foreach (Gv a in (Gv.I(32L)).Iter()) {
        pts.C("append", (c + Gd.Ctor("Vector2", ((Gd.cos(((Gd.K("TAU") * a) / Gv.F(32.0))) * rx) * k), ((Gd.sin(((Gd.K("TAU") * a) / Gv.F(32.0))) * ry) * k))));
      }
      ci.C("draw_colored_polygon", pts, Gd.Ctor("Color", Gv.F(0.1), Gv.F(0.12), Gv.F(0.14), Gv.F(0.07)));
    }
    return Gv.Null;
  }
  public static Gv brushed(Gv ci = default(Gv), Gv r = default(Gv)) {
    Gv x = (r.G("position").G("x") + Gv.F(3.0));
    Gv i = Gv.I(0L);
    while (Gv.Lt(x, (r.G("end").G("x") - Gv.F(2.0)))) {
      Gv a = (Gv.F(0.035) + (Gv.F(0.03) * Gd.fmod((i * Gv.F(0.618)), Gv.F(1.0))));
      ci.C("draw_line", Gd.Ctor("Vector2", x, (r.G("position").G("y") + Gv.I(2L))), Gd.Ctor("Vector2", x, (r.G("end").G("y") - Gv.I(2L))), (Gv.Eq((i % Gv.I(2L)), Gv.I(0L)) ? Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), a) : Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), (a * Gv.F(0.8)))), Gv.F(1.0));
      x = (x + (Gv.F(3.0) + Gd.fmod((i * Gv.F(1.7)), Gv.F(4.0))));
      i = (i + Gv.I(1L));
    }
    return Gv.Null;
  }
  public static Gv text_c(Gv ci = default(Gv), Gv f = default(Gv), Gv t = default(Gv), Gv pos = default(Gv), Gv size = default(Gv), Gv col = default(Gv)) {
    size = (size).AsI();
    Gv w = f.C("get_string_size", t, Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), size).G("x");
    ci.C("draw_string", f, (pos - Gd.Ctor("Vector2", (w * Gv.F(0.5)), Gv.I(0L))), t, Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), size, col);
    return Gv.Null;
  }
  public static Gv pill(Gv ci = default(Gv), Gv f = default(Gv), Gv t = default(Gv), Gv pos = default(Gv), Gv size = default(Gv), Gv fg = default(Gv), Gv bg = default(Gv)) {
    if (size.k == 0) size = Gv.I(13L);
    if (fg.k == 0) fg = Gd.Ctor("Color", Gv.S("e6ebef"));
    if (bg.k == 0) bg = Gd.Ctor("Color", Gv.F(0.07), Gv.F(0.09), Gv.F(0.11), Gv.F(0.78));
    if (Gv.Eq(t, Gv.S(""))) {
      return Gv.Null;
    }
    Gv w = f.C("get_string_size", t, Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), size).G("x");
    rrect_border(ci, Gd.Ctor("Rect2", ((pos.G("x") - (w * Gv.F(0.5))) - Gv.I(8L)), ((pos.G("y") - size) - Gv.I(3L)), (w + Gv.I(16L)), (size + Gv.I(9L))), bg, Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.16)), Gv.I(1L), (((size + Gv.I(9L))) * Gv.F(0.5)));
    ci.C("draw_string", f, Gd.Ctor("Vector2", (pos.G("x") - (w * Gv.F(0.5))), pos.G("y")), t, Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), size, fg);
    return Gv.Null;
  }
  public static Gv nameplate(Gv ci = default(Gv), Gv f = default(Gv), Gv t = default(Gv), Gv cx = default(Gv), Gv y = default(Gv), Gv size = default(Gv)) {
    cx = (cx).AsF();
    y = (y).AsF();
    if (size.k == 0) size = Gv.I(16L);
    Gv w = (f.C("get_string_size", t, Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), size).G("x") + Gv.I(34L));
    Gv r = Gd.Ctor("Rect2", (cx - (w * Gv.F(0.5))), y, w, (size + Gv.I(14L)));
    rrect(ci, Gd.Ctor("Rect2", (r.G("position") + Gd.Ctor("Vector2", Gv.I(2L), Gv.I(3L))), r.G("size")), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.25)), Gv.F(5.0));
    rrect_border(ci, r, Gd.Ctor("Color", Gv.S("171c21")), Gd.Ctor("Color", Gv.S("f2c230")), Gv.I(2L), Gv.F(5.0));
    ci.C("draw_line", (r.G("position") + Gd.Ctor("Vector2", Gv.I(6L), Gv.I(3L))), Gd.Ctor("Vector2", (r.G("end").G("x") - Gv.I(6L)), (r.G("position").G("y") + Gv.I(3L))), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.12)), Gv.F(1.0));
    foreach (Gv sx in (Gd.Arr((r.G("position").G("x") + Gv.F(8.0)), (r.G("end").G("x") - Gv.F(8.0)))).Iter()) {
      ci.C("draw_circle", Gd.Ctor("Vector2", sx, (r.G("position").G("y") + (r.G("size").G("y") * Gv.F(0.5)))), Gv.F(2.2), Gd.Ctor("Color", Gv.S("8a939b")));
      ci.C("draw_line", Gd.Ctor("Vector2", (sx - Gv.F(1.4)), (r.G("position").G("y") + (r.G("size").G("y") * Gv.F(0.5)))), Gd.Ctor("Vector2", (sx + Gv.F(1.4)), (r.G("position").G("y") + (r.G("size").G("y") * Gv.F(0.5)))), Gd.Ctor("Color", Gv.S("3b434a")), Gv.F(1.0));
    }
    ci.C("draw_string", f, Gd.Ctor("Vector2", ((cx - (w * Gv.F(0.5))) + Gv.I(17L)), ((y + size) + Gv.I(4L))), t, Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), size, Gd.Ctor("Color", Gv.S("f2c230")));
    return Gv.Null;
  }
  public static Gv stadium(Gv ci = default(Gv), Gv r = default(Gv), Gv stops = default(Gv), Gv n = default(Gv)) {
    if (n.k == 0) n = Gv.I(56L);
    Gv rr = (r.G("size").G("y") * Gv.F(0.5));
    Gv cy = (r.G("position").G("y") + rr);
    foreach (Gv i in (n).Iter()) {
      Gv t0 = (Gd.Dyn("float", i) / n);
      Gv t1 = (Gd.Dyn("float", (i + Gv.I(1L))) / n);
      Gv y0 = (r.G("position").G("y") + (r.G("size").G("y") * t0));
      Gv y1 = (r.G("position").G("y") + (r.G("size").G("y") * t1));
      Gv dy = Gd.absf(((((y0 + y1)) * Gv.F(0.5)) - cy));
      Gv half = Gd.sqrt(Gd.maxf(Gv.F(0.0), ((rr * rr) - (dy * dy))));
      Gv xl = ((r.G("position").G("x") + rr) - half);
      Gv xr = ((r.G("end").G("x") - rr) + half);
      Gv c0 = grad(stops, t0);
      Gv c1 = grad(stops, t1);
      ci.C("draw_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", xl, y0), Gd.Ctor("Vector2", xr, y0), Gd.Ctor("Vector2", xr, y1), Gd.Ctor("Vector2", xl, y1))), Gd.Ctor("PackedColorArray", Gd.Arr(c0, c0, c1, c1)));
    }
    Gv pts = Gd.Ctor("PackedVector2Array");
    foreach (Gv k in (Gv.I(25L)).Iter()) {
      pts.C("append", (Gd.Ctor("Vector2", (r.G("end").G("x") - rr), cy) + (Gd.Ctor("Vector2", Gd.cos((((-Gd.K("PI")) * Gv.F(0.5)) + ((Gd.K("PI") * k) / Gv.F(24.0)))), Gd.sin((((-Gd.K("PI")) * Gv.F(0.5)) + ((Gd.K("PI") * k) / Gv.F(24.0))))) * rr)));
    }
    foreach (Gv k_2 in (Gv.I(25L)).Iter()) {
      pts.C("append", (Gd.Ctor("Vector2", (r.G("position").G("x") + rr), cy) + (Gd.Ctor("Vector2", Gd.cos(((Gd.K("PI") * Gv.F(0.5)) + ((Gd.K("PI") * k_2) / Gv.F(24.0)))), Gd.sin(((Gd.K("PI") * Gv.F(0.5)) + ((Gd.K("PI") * k_2) / Gv.F(24.0))))) * rr)));
    }
    pts.C("append", pts.I(Gv.I(0L)));
    ci.C("draw_polyline", pts, Gd.Ctor("Color", Gv.S("0d1013")), Gv.F(2.5), Gv.B(true));
    return Gv.Null;
  }
  public static Gv tex(Gv n = default(Gv)) {
    if ((_tx.C("has", n)).T()) {
      return _tx.I(n);
    }
    Gv t = Gv.Null;
    Gv p = (Gv.S("res://art/%s.png") % n);
    if ((Gd.Static("ResourceLoader", "exists", p)).T()) {
      Gv src = Gd.load(p);
      Gv im = src.C("get_image");
      if (Gv.Ne(im, Gv.Null)) {
        im.C("generate_mipmaps");
        t = Gd.Static("ImageTexture", "create_from_image", im);
      }
    }
    _tx = _tx.SI(n, t);
    return t;
    return Gv.Null;
  }
  public static Gv sprite(Gv ci = default(Gv), Gv n = default(Gv), Gv p = default(Gv), Gv rot = default(Gv), Gv scale = default(Gv), Gv pivot = default(Gv), Gv flip = default(Gv)) {
    rot = (rot).AsF();
    scale = (scale).AsF();
    if (flip.k == 0) flip = Gv.B(false);
    Gv t = tex(n);
    if (Gv.Eq(t, Gv.Null)) {
      return Gv.B(false);
    }
    ci = ci.S("texture_filter", Gd.StaticGet("CanvasItem", "TEXTURE_FILTER_LINEAR_WITH_MIPMAPS"));
    ci.C("draw_set_transform", p, rot, Gd.Ctor("Vector2", ((flip).T() ? (-scale) : scale), scale));
    ci.C("draw_texture", t, (-pivot));
    ci.C("draw_set_transform", Gd.StaticGet("Vector2", "ZERO"), Gv.I(0L), Gd.StaticGet("Vector2", "ONE"));
    return Gv.B(true);
    return Gv.Null;
  }
  public static Gv tex_rect(Gv ci = default(Gv), Gv n = default(Gv), Gv r = default(Gv), Gv tint = default(Gv)) {
    if (tint.k == 0) tint = Gd.StaticGet("Color", "WHITE");
    Gv t = tex(n);
    if (Gv.Eq(t, Gv.Null)) {
      return Gv.B(false);
    }
    ci = ci.S("texture_filter", Gd.StaticGet("CanvasItem", "TEXTURE_FILTER_LINEAR_WITH_MIPMAPS"));
    ci.C("draw_texture_rect", t, r, Gv.B(false), tint);
    return Gv.B(true);
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "STAINLESS": return S_art.STAINLESS;
    case "COPPER": return S_art.COPPER;
    case "CLAD": return S_art.CLAD;
    case "NAVY": return S_art.NAVY;
    case "PIPE_STEEL": return S_art.PIPE_STEEL;
    case "_sb": return S_art._sb;
    case "_tx": return S_art._tx;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "_sb": S_art._sb = v; return v;
    case "_tx": S_art._tx = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "grad": return S_art.grad(Gd.A(a, 0), Gd.A(a, 1));
    case "cyl_v": return S_art.cyl_v(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "cyl_h": return S_art.cyl_h(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "cone": return S_art.cone(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6), Gd.A(a, 7));
    case "dome": return S_art.dome(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6));
    case "shadow": return S_art.shadow(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "ellipse_shadow": return S_art.ellipse_shadow(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "rrect": return S_art.rrect(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "rrect_border": return S_art.rrect_border(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5));
    case "pipe": return S_art.pipe(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "floor_shadow": return S_art.floor_shadow(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "brushed": return S_art.brushed(Gd.A(a, 0), Gd.A(a, 1));
    case "text_c": return S_art.text_c(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5));
    case "pill": return S_art.pill(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6));
    case "nameplate": return S_art.nameplate(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5));
    case "stadium": return S_art.stadium(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "tex": return S_art.tex(Gd.A(a, 0));
    case "sprite": return S_art.sprite(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6));
    case "tex_rect": return S_art.tex_rect(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    case "STAINLESS": return STAINLESS;
    case "COPPER": return COPPER;
    case "CLAD": return CLAD;
    case "NAVY": return NAVY;
    case "PIPE_STEEL": return PIPE_STEEL;
    case "_sb": return _sb;
    case "_tx": return _tx;
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "grad": return grad(Gd.A(a, 0), Gd.A(a, 1));
    case "cyl_v": return cyl_v(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "cyl_h": return cyl_h(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "cone": return cone(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6), Gd.A(a, 7));
    case "dome": return dome(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6));
    case "shadow": return shadow(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "ellipse_shadow": return ellipse_shadow(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "rrect": return rrect(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "rrect_border": return rrect_border(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5));
    case "pipe": return pipe(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "floor_shadow": return floor_shadow(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "brushed": return brushed(Gd.A(a, 0), Gd.A(a, 1));
    case "text_c": return text_c(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5));
    case "pill": return pill(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6));
    case "nameplate": return nameplate(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5));
    case "stadium": return stadium(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "tex": return tex(Gd.A(a, 0));
    case "sprite": return sprite(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6));
    case "tex_rect": return tex_rect(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_art(); return Gv.O(o); }
  public override string ScriptName { get { return "art"; } }
}
}
