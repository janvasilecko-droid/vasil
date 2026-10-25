// Vygenerováno z main.gd překladačem gd2cs.py – needituj ručně.
using System; using System.Collections.Generic;
namespace Pivovar.Gd.Gen {
public partial class S_main_CabDock : GdNode {
  public Gv m;
  public Gv sv;
  public Gv cam2;
  public Gv hold;
  public Gv lens;
  public Gv lsv;
  public Gv lcam;
  public Gv lens_on;
  public Gv lens_btn;
  public Gv pot_in_lens;
  public Gv user;
  public Gv panning;
  public Gv pot;
  static S_main_CabDock() {
  }
  public S_main_CabDock() {
    TypeName = "SubViewportContainer";
    m = Gv.Null;
    sv = Gv.Null;
    cam2 = Gv.Null;
    hold = Gv.S("");
    lens = Gv.Null;
    lsv = Gv.Null;
    lcam = Gv.Null;
    lens_on = Gv.B(false);
    lens_btn = Gv.Null;
    pot_in_lens = Gv.B(false);
    user = Gv.B(false);
    panning = Gv.B(false);
    pot = Gd.Dict();
  }
  public Gv _lens_open(Gv p = default(Gv)) {
    if (Gv.Eq(lens, Gv.Null)) {
      return Gv.Null;
    }
    Gv w = _to_world(p);
    lens = lens.S("size", Gd.Ctor("Vector2", Gd.maxf(Gv.F(250.0), (Self.G("size").G("x") - Gv.F(16.0))), Gv.F(250.0)));
    lens = lens.S("custom_minimum_size", lens.G("size"));
    lcam.C("make_current");
    Gv z = (cam2.G("zoom").G("x") * Gv.F(2.9));
    lcam = lcam.S("position", w);
    lcam = lcam.S("zoom", Gd.Ctor("Vector2", z, z));
    Gv ly = ((Gv.Lt(p.G("y"), (Self.G("size").G("y") * Gv.F(0.5))) ? ((Self.G("size").G("y") - lens.G("size").G("y")) - Gv.F(8.0)) : Gv.F(8.0))).AsF();
    lens = lens.S("position", Gd.Ctor("Vector2", Gv.F(8.0), ly));
    lens = lens.S("visible", Gv.B(true));
    lens_on = Gv.B(true);
    lens_btn = lens_btn.S("position", Gd.Ctor("Vector2", ((lens.G("position").G("x") + lens.G("size").G("x")) - Gv.F(92.0)), (lens.G("position").G("y") + Gv.F(6.0))));
    lens_btn = lens_btn.S("visible", Gv.B(true));
    return Gv.Null;
  }
  public Gv _lens_close() {
    lens_on = Gv.B(false);
    if (Gv.Ne(lens, Gv.Null)) {
      lens = lens.S("visible", Gv.B(false));
    }
    if (Gv.Ne(lens_btn, Gv.Null)) {
      lens_btn = lens_btn.S("visible", Gv.B(false));
    }
    return Gv.Null;
  }
  public Gv _ew(Gv p = default(Gv)) {
    Gv use_lens = Gv.B(false);
    if ((!(pot.C("is_empty")).T() || Gv.Ne(hold, Gv.S("")))) {
      use_lens = pot_in_lens;
    } else {
      use_lens = Gv.B(((lens_on).T() && (Gd.Ctor("Rect2", lens.G("position"), lens.G("size")).C("has_point", p)).T()));
    }
    if ((use_lens).T()) {
      return (lcam.G("position") + ((((p - lens.G("position")) - (lens.G("size") * Gv.F(0.5)))) / lcam.G("zoom").G("x")));
    }
    return _to_world(p);
    return Gv.Null;
  }
  public Gv setup() {
    Self.S("stretch", Gv.B(true));
    Self.S("mouse_filter", Gd.StaticGet("Control", "MOUSE_FILTER_STOP"));
    Self.S("anchor_left", Gv.F(1.0));
    Self.S("anchor_right", Gv.F(1.0));
    Self.S("anchor_bottom", Gv.F(1.0));
    Self.S("offset_top", Gv.I(52L));
    Self.S("offset_bottom", Gv.I(0L));
    sv = Gd.New("SubViewport");
    sv = sv.S("world_2d", m.C("get_viewport").G("world_2d"));
    sv = sv.S("transparent_bg", Gv.B(false));
    Self.C("add_child", sv);
    cam2 = Gd.New("Camera2D");
    sv.C("add_child", cam2);
    cam2 = cam2.S("enabled", Gv.B(true));
    cam2.C("make_current");
    Self.G("resized").C("connect", Gd.Fn((_a) => reset_view()));
    Self.G("gui_input").C("connect", Gd.Fn((_a) => _on_input(Gd.A(_a, 0))));
    lens = Gd.New("SubViewportContainer");
    lens = lens.S("stretch", Gv.B(true));
    lens = lens.S("mouse_filter", Gd.StaticGet("Control", "MOUSE_FILTER_IGNORE"));
    lens = lens.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(330L), Gv.I(170L)));
    lens = lens.S("size", Gd.Ctor("Vector2", Gv.I(330L), Gv.I(170L)));
    lens = lens.S("visible", Gv.B(false));
    lsv = Gd.New("SubViewport");
    lsv = lsv.S("world_2d", m.C("get_viewport").G("world_2d"));
    lsv = lsv.S("transparent_bg", Gv.B(false));
    lens.C("add_child", lsv);
    lcam = Gd.New("Camera2D");
    lsv.C("add_child", lcam);
    lcam = lcam.S("enabled", Gv.B(true));
    Self.C("add_child", lens);
    Gv fr = Gd.New("Panel");
    fr = fr.S("mouse_filter", Gd.StaticGet("Control", "MOUSE_FILTER_IGNORE"));
    fr.C("set_anchors_preset", Gd.StaticGet("Control", "PRESET_FULL_RECT"));
    Gv fsb = Gd.New("StyleBoxFlat");
    fsb = fsb.S("bg_color", Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.I(0L)));
    fsb = fsb.S("border_color", Gd.Ctor("Color", Gv.S("f2c230")));
    fsb.C("set_border_width_all", Gv.I(3L));
    fsb.C("set_corner_radius_all", Gv.I(8L));
    fr.C("add_theme_stylebox_override", Gv.S("panel"), fsb);
    lens.C("add_child", fr);
    lens_btn = Gd.New("Button");
    lens_btn = lens_btn.S("text", Gv.S("✕ zavřít"));
    lens_btn = lens_btn.S("focus_mode", Gd.StaticGet("Control", "FOCUS_NONE"));
    lens_btn = lens_btn.S("visible", Gv.B(false));
    lens_btn.G("pressed").C("connect", Gd.Fn((_a) => _lens_close()));
    Self.C("add_child", lens_btn);
    m.C("get_viewport").G("size_changed").C("connect", Gd.Fn((_a) => {  m.C("apply_ui"); return Gv.Null; }));
    return Gv.Null;
  }
  public Gv rect() {
    Gv it = m.G("cabinet").G("it");
    Gv cs = (m.G("cabinet").G("sc")).AsF();
    return Gd.Ctor("Rect2", (Gd.Dyn("float", it.G("x")) + (Gv.F(190.0) * cs)), (Gd.Dyn("float", it.G("y")) - Gv.F(10.0)), ((Gv.F(580.0) * cs) + Gv.F(10.0)), ((Gv.F(1130.0) * cs) + Gv.F(20.0)));
    return Gv.Null;
  }
  public Gv base_zoom() {
    Gv r = rect();
    return ((!(m.G("dock_big")).T() ? Gd.minf((Self.G("size").G("x") / r.G("size").G("x")), (Self.G("size").G("y") / r.G("size").G("y"))) : (Self.G("size").G("x") / r.G("size").G("x")))).AsF();
    return Gv.Null;
  }
  public Gv reset_view() {
    user = Gv.B(false);
    fit();
    return Gv.Null;
  }
  public Gv fit() {
    if (((Gv.Eq(cam2, Gv.Null) || Gv.Eq(m.G("cabinet"), Gv.Null)) || Gv.Lt(Self.G("size").G("x"), Gv.I(10L)))) {
      return Gv.Null;
    }
    if ((user).T()) {
      _clamp();
      return Gv.Null;
    }
    Gv r = rect();
    Gv z = base_zoom();
    cam2 = cam2.S("zoom", Gd.Ctor("Vector2", z, z));
    cam2 = cam2.S("position", Gd.Ctor("Vector2", r.C("get_center").G("x"), (r.G("position").G("y") + ((Self.G("size").G("y") * Gv.F(0.5)) / z))));
    _clamp();
    return Gv.Null;
  }
  public Gv _clamp() {
    Gv r = rect();
    Gv z = (cam2.G("zoom").G("x")).AsF();
    Gv half = ((Self.G("size") * Gv.F(0.5)) / z);
    Gv lo = (r.G("position") + half);
    Gv hi = (r.G("end") - half);
    cam2 = cam2.S("position", cam2.G("position").S("x", (Gv.Gt(lo.G("x"), hi.G("x")) ? r.C("get_center").G("x") : Gd.clampf(cam2.G("position").G("x"), lo.G("x"), hi.G("x")))));
    cam2 = cam2.S("position", cam2.G("position").S("y", (Gv.Gt(lo.G("y"), hi.G("y")) ? ((r.G("position").G("y") + half.G("y"))) : Gd.clampf(cam2.G("position").G("y"), lo.G("y"), hi.G("y")))));
    return Gv.Null;
  }
  public Gv _zoom_at(Gv f = default(Gv), Gv p = default(Gv)) {
    f = (f).AsF();
    Gv w0 = _to_world(p);
    Gv z = Gd.clampf((cam2.G("zoom").G("x") * f), base_zoom(), (base_zoom() * Gv.F(3.5)));
    cam2 = cam2.S("zoom", Gd.Ctor("Vector2", z, z));
    cam2 = cam2.S("position", (cam2.G("position") + (w0 - _to_world(p))));
    user = Gv.B(true);
    _clamp();
    return Gv.Null;
  }
  public Gv _draw() {
    Self.C("draw_rect", Gd.Ctor("Rect2", Gd.StaticGet("Vector2", "ZERO"), Gd.Ctor("Vector2", Gv.I(2L), Self.G("size").G("y"))), Gd.Ctor("Color", Gv.S("2a323a")));
    return Gv.Null;
  }
  public Gv _to_world(Gv p = default(Gv)) {
    return (cam2.G("position") + (((p - (Self.G("size") * Gv.F(0.5)))) / cam2.G("zoom").G("x")));
    return Gv.Null;
  }
  public Gv _pot_from(Gv p = default(Gv)) {
    Gv d = (_ew(p) - pot.G("c"));
    Gv a = Gd.rad_to_deg(Gd.atan2(d.G("x"), (-d.G("y"))));
    m.G("cabinet").C("set_pot", pot.G("k"), (((Gd.clampf(a, Gv.F(-135.0), Gv.F(135.0)) + Gv.F(135.0))) / Gv.F(270.0)));
    m.C("redraw");
    return Gv.Null;
  }
  public Gv _on_input(Gv e = default(Gv)) {
    if ((((Gv.Is(e, "InputEventMouseButton") && Gv.Eq(e.G("button_index"), Gd.K("MOUSE_BUTTON_LEFT"))) && !(e.G("pressed")).T()) && !(pot.C("is_empty")).T())) {
      pot = Gd.Dict();
      Self.C("accept_event");
      return Gv.Null;
    }
    if (((Gv.Is(e, "InputEventMouseButton") && Gv.Eq(e.G("button_index"), Gd.K("MOUSE_BUTTON_LEFT"))) && (e.G("pressed")).T())) {
      Gv in_lens = Gv.B(((lens_on).T() && (Gd.Ctor("Rect2", lens.G("position"), lens.G("size")).C("has_point", e.G("position"))).T()));
      if (!(in_lens).T()) {
        _lens_open(e.G("position"));
        Self.C("accept_event");
        return Gv.Null;
      }
      pot_in_lens = Gv.B(true);
      Gv ph = m.G("cabinet").C("pot_hit", _ew(e.G("position")));
      if (!(ph.C("is_empty")).T()) {
        pot = ph;
        _pot_from(e.G("position"));
        if (Gv.Ne(m.G("game"), Gv.Null)) {
          m.G("game").C("play", Gv.S("click"));
        }
        Self.C("accept_event");
        return Gv.Null;
      }
    }
    if ((Gv.Is(e, "InputEventMouseMotion") && !(pot.C("is_empty")).T())) {
      _pot_from(e.G("position"));
      Self.C("accept_event");
      return Gv.Null;
    }
    if (((Gv.Is(e, "InputEventMouseButton") && (e.G("pressed")).T()) && Gv.In(e.G("button_index"), Gd.Arr(Gd.K("MOUSE_BUTTON_WHEEL_UP"), Gd.K("MOUSE_BUTTON_WHEEL_DOWN"))))) {
      Gv up_ = Gv.B(Gv.Eq(e.G("button_index"), Gd.K("MOUSE_BUTTON_WHEEL_UP")));
      Gv ph2 = m.G("cabinet").C("pot_hit", _ew(e.G("position")));
      if ((!(ph2.C("is_empty")).T() && !(e.G("ctrl_pressed")).T())) {
        m.G("cabinet").C("set_pot", ph2.G("k"), (m.G("cabinet").C("pot_value", ph2.G("k")) + (((((up_).T() ? Gv.F(1.0) : Gv.F(-1.0))) * Gv.F(0.2)) / Gv.F(9.0))));
        m.C("redraw");
      } else if ((e.G("ctrl_pressed")).T()) {
        _zoom_at(((up_).T() ? Gv.F(1.15) : (Gv.F(1.0) / Gv.F(1.15))), e.G("position"));
      } else {
        cam2 = cam2.S("position", cam2.G("position").S("y", (cam2.G("position").G("y") + ((((up_).T() ? Gv.F(-60.0) : Gv.F(60.0))) / cam2.G("zoom").G("x")))));
        user = Gv.B(true);
        _clamp();
      }
      Self.C("accept_event");
      return Gv.Null;
    }
    if ((Gv.Is(e, "InputEventMouseButton") && Gv.Eq(e.G("button_index"), Gd.K("MOUSE_BUTTON_LEFT")))) {
      Gv w = _ew(e.G("position"));
      if ((e.G("pressed")).T()) {
        Gv hk = m.G("cabinet").C("hit", w);
        if ((Gv.Eq(hk, Gv.S("hup")) || Gv.Eq(hk, Gv.S("hdn")))) {
          hold = hk;
          m.G("cabinet").C("act", hk, Gv.B(true));
        } else if (Gv.Ne(hk, Gv.S(""))) {
          m.G("cabinet").C("act", hk, Gv.B(true));
        }
        m.C("redraw");
      } else if (Gv.Ne(hold, Gv.S(""))) {
        m.G("cabinet").C("act", hold, Gv.B(false));
        hold = Gv.S("");
      }
      Self.C("accept_event");
    } else if (Gv.Is(e, "InputEventMouseButton")) {
      Gv mb = e;
      if (Gv.In(mb.G("button_index"), Gd.Arr(Gd.K("MOUSE_BUTTON_RIGHT"), Gd.K("MOUSE_BUTTON_MIDDLE")))) {
        panning = mb.G("pressed");
      }
      Self.C("accept_event");
    } else if ((Gv.Is(e, "InputEventMouseMotion") && (panning).T())) {
      cam2 = cam2.S("position", (cam2.G("position") - ((e).G("relative") / cam2.G("zoom").G("x"))));
      user = Gv.B(true);
      _clamp();
      Self.C("accept_event");
    } else if ((Gv.Is(e, "InputEventMouseMotion") && Gv.Ne(m.G("game"), Gv.Null))) {
      Gv hk_2 = m.G("cabinet").C("hit", _ew(e.G("position")));
      Gv isp = Gv.B(!(m.G("cabinet").C("pot_hit", _ew(e.G("position"))).C("is_empty")).T());
      Self.S("mouse_default_cursor_shape", ((isp).T() ? Gd.StaticGet("Control", "CURSOR_DRAG") : ((Gv.Ne(hk_2, Gv.S("")) ? Gd.StaticGet("Control", "CURSOR_POINTING_HAND") : Gd.StaticGet("Control", "CURSOR_ARROW")))));
      Self.S("tooltip_text", ((isp).T() ? (Gv.S("FM: táhni dokola, nebo kolečko myši (stupeň %s)") % m.G("cabinet").C("_stf", m.G("cabinet").C("pot_value", m.G("cabinet").C("pot_hit", _ew(e.G("position"))).G("k")))) : Gv.S("")));
    }
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "m": return this.m;
    case "sv": return this.sv;
    case "cam2": return this.cam2;
    case "hold": return this.hold;
    case "lens": return this.lens;
    case "lsv": return this.lsv;
    case "lcam": return this.lcam;
    case "lens_on": return this.lens_on;
    case "lens_btn": return this.lens_btn;
    case "pot_in_lens": return this.pot_in_lens;
    case "user": return this.user;
    case "panning": return this.panning;
    case "pot": return this.pot;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "m": this.m = v; return v;
    case "sv": this.sv = v; return v;
    case "cam2": this.cam2 = v; return v;
    case "hold": this.hold = v; return v;
    case "lens": this.lens = v; return v;
    case "lsv": this.lsv = v; return v;
    case "lcam": this.lcam = v; return v;
    case "lens_on": this.lens_on = v; return v;
    case "lens_btn": this.lens_btn = v; return v;
    case "pot_in_lens": this.pot_in_lens = v; return v;
    case "user": this.user = v; return v;
    case "panning": this.panning = v; return v;
    case "pot": this.pot = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "_lens_open": return _lens_open(Gd.A(a, 0));
    case "_lens_close": return _lens_close();
    case "_ew": return _ew(Gd.A(a, 0));
    case "setup": return setup();
    case "rect": return rect();
    case "base_zoom": return base_zoom();
    case "reset_view": return reset_view();
    case "fit": return fit();
    case "_clamp": return _clamp();
    case "_zoom_at": return _zoom_at(Gd.A(a, 0), Gd.A(a, 1));
    case "_draw": return _draw();
    case "_to_world": return _to_world(Gd.A(a, 0));
    case "_pot_from": return _pot_from(Gd.A(a, 0));
    case "_on_input": return _on_input(Gd.A(a, 0));
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_main_CabDock(); return Gv.O(o); }
  public override string ScriptName { get { return "S_main_CabDock"; } }
}
public partial class S_main_StaticLayer : GdNode {
  public Gv m;
  public Gv clip;
  public Gv _pends;
  public Gv _occ;
  static S_main_StaticLayer() {
  }
  public S_main_StaticLayer() {
    TypeName = "Node2D";
    m = Gv.Null;
    clip = Gd.Ctor("Rect2");
    _pends = Gd.Dict();
    _occ = Gd.Ctor("PackedVector2Array");
  }
  public Gv _seen(Gv pts = default(Gv)) {
    if (Gv.Eq(clip.G("size"), Gd.StaticGet("Vector2", "ZERO"))) {
      return Gv.B(true);
    }
    Gv lo = pts.I(Gv.I(0L));
    Gv hi = pts.I(Gv.I(0L));
    foreach (Gv p in (pts).Iter()) {
      lo = Gd.Ctor("Vector2", Gd.minf(lo.G("x"), p.G("x")), Gd.minf(lo.G("y"), p.G("y")));
      hi = Gd.Ctor("Vector2", Gd.maxf(hi.G("x"), p.G("x")), Gd.maxf(hi.G("y"), p.G("y")));
    }
    return clip.C("intersects", Gd.Ctor("Rect2", lo, (hi - lo)).C("grow", Gv.F(30.0)));
    return Gv.Null;
  }
  public Gv _pipe_layers(Gv pts = default(Gv), Gv w = default(Gv), Gv rounded = default(Gv)) {
    w = (w).AsF();
    _pipe_outline(pts, w, rounded);
    _pipe_fill(pts, w, rounded);
    return Gv.Null;
  }
  public Gv _pends_build() {
    _pends.C("clear");
    _occ.C("clear");
    foreach (Gv it0 in (m.G("items")).Iter()) {
      if ((Gv.In(it0.G("type"), Gd.Arr(Gv.S("valve"), Gv.S("pump"), Gv.S("fitting"), Gv.S("inl"), Gv.S("manifold"))) && (it0.C("has", Gv.S("x"))).T())) {
        _occ.C("append", Gd.Ctor("Vector2", it0.G("x"), it0.C("get", Gv.S("y"), Gv.I(0L))));
      }
    }
    foreach (Gv it in (m.G("items")).Iter()) {
      if ((Gv.Eq(it.G("type"), Gv.S("pipe")) && !(it.C("get", Gv.S("hose"), Gv.B(false))).T())) {
        Gv n = (it.G("pts").C("size")).AsI();
        foreach (Gv i in (n).Iter()) {
          Gv k = Gd.Ctor("Vector2i", Gd.roundi(it.G("pts").I(i).I(Gv.I(0L))), Gd.roundi(it.G("pts").I(i).I(Gv.I(1L))));
          _pends = _pends.SI(k, (_pends.C("get", k, Gv.I(0L)) + Gv.I(1L)));
        }
      }
    }
    foreach (Gv j in (m.G("plan").C("get", Gv.S("joints"), Gd.Arr())).Iter()) {
      _pends = _pends.SI(Gd.Ctor("Vector2i", Gd.roundi(j.I(Gv.I(0L))), Gd.roundi(j.I(Gv.I(1L)))), Gv.I(9L));
    }
    return Gv.Null;
  }
  public Gv _fillet(Gv pts = default(Gv), Gv w = default(Gv)) {
    w = (w).AsF();
    if (Gv.Lt(pts.C("size"), Gv.I(3L))) {
      return pts;
    }
    Gv out_ = Gd.Ctor("PackedVector2Array", Gd.Arr(pts.I(Gv.I(0L))));
    Gv R = (Gd.maxf(Gv.F(8.0), (w * Gv.F(1.5)))).AsF();
    foreach (Gv i in Gd.RangeIter(Gv.I(1L), (pts.C("size") - Gv.I(1L)))) {
      Gv P = pts.I(i);
      Gv A = pts.I((i - Gv.I(1L)));
      Gv B = pts.I((i + Gv.I(1L)));
      Gv d1 = ((A - P));
      Gv d2 = ((B - P));
      Gv l1 = d1.C("length");
      Gv l2 = d2.C("length");
      if ((((Gv.Lt(l1, Gv.F(1.0)) || Gv.Lt(l2, Gv.F(1.0))) || Gv.Lt(Gd.absf(d1.C("normalized").C("cross", d2.C("normalized"))), Gv.F(0.2))) || Gv.Gt(_pends.C("get", Gd.Ctor("Vector2i", Gd.roundi(P.G("x")), Gd.roundi(P.G("y"))), Gv.I(0L)), Gv.I(1L)))) {
        out_.C("append", P);
        continue;
      }
      Gv r = (Gd.minf(R, (Gd.minf(l1, l2) * Gv.F(0.5)))).AsF();
      Gv T1 = (P + ((d1 / l1) * r));
      Gv T2 = (P + ((d2 / l2) * r));
      foreach (Gv k in (Gv.I(9L)).Iter()) {
        Gv t = (Gd.Dyn("float", k) / Gv.F(8.0));
        out_.C("append", T1.C("lerp", P, t).C("lerp", P.C("lerp", T2, t), t));
      }
    }
    out_.C("append", pts.I((pts.C("size") - Gv.I(1L))));
    return out_;
    return Gv.Null;
  }
  public Gv _pipe_outline(Gv pts = default(Gv), Gv w = default(Gv), Gv rounded = default(Gv)) {
    w = (w).AsF();
    pts = _fillet(pts, w);
    Self.C("draw_polyline", pts, Gd.Ctor("Color", Gv.S("1e2429")), (w + Gv.F(3.5)), rounded);
    return Gv.Null;
  }
  public Gv _pipe_fill(Gv pts = default(Gv), Gv w = default(Gv), Gv rounded = default(Gv)) {
    w = (w).AsF();
    pts = _fillet(pts, w);
    Self.C("draw_polyline", pts, Gd.Ctor("Color", Gv.S("7d8892")), (w + Gv.F(1.0)), rounded);
    Self.C("draw_polyline", pts, Gd.Ctor("Color", Gv.S("a9b3bb")), Gd.maxf(Gv.F(1.5), (w * Gv.F(0.8))), rounded);
    Self.C("draw_polyline", pts, Gd.Ctor("Color", Gv.S("cfd6dc")), Gd.maxf(Gv.F(1.3), (w * Gv.F(0.52))), rounded);
    Self.C("draw_polyline", pts, Gd.Ctor("Color", Gv.S("f3f6f8")), Gd.maxf(Gv.F(1.0), (w * Gv.F(0.2))), rounded);
    return Gv.Null;
  }
  public Gv _pipe_clamps(Gv P = default(Gv)) {
    Gv pp = P.I(Gv.I(0L));
    Gv w = (P.I(Gv.I(1L))).AsF();
    if ((Gv.Lt(pp.C("size"), Gv.I(2L)) || Gv.Lt(w, Gv.F(5.0)))) {
      return Gv.Null;
    }
    foreach (Gv k in (Gv.I(2L)).Iter()) {
      Gv e = (Gv.Eq(k, Gv.I(0L)) ? pp.I(Gv.I(0L)) : pp.I((pp.C("size") - Gv.I(1L))));
      Gv nb = (Gv.Eq(k, Gv.I(0L)) ? pp.I(Gv.I(1L)) : pp.I((pp.C("size") - Gv.I(2L))));
      Gv d = ((nb - e));
      if (Gv.Lt(d.C("length"), Gv.F(14.0))) {
        continue;
      }
      d = d.C("normalized");
      Gv nr = Gd.Ctor("Vector2", (-d.G("y")), d.G("x"));
      Gv c = (e + (d * Gv.F(8.0)));
      Gv hw = (((w * Gv.F(0.5)) + Gv.F(3.5))).AsF();
      Self.C("draw_line", (c - (nr * hw)), (c + (nr * hw)), Gd.Ctor("Color", Gv.S("1e2429")), Gv.F(7.0));
      Self.C("draw_line", (c - (nr * ((hw - Gv.F(1.0))))), (c + (nr * ((hw - Gv.F(1.0))))), Gd.Ctor("Color", Gv.S("c3ccd3")), Gv.F(5.0));
      Self.C("draw_line", ((c - (nr * ((hw - Gv.F(1.0))))) - (d * Gv.F(1.2))), ((c + (nr * ((hw - Gv.F(1.0))))) - (d * Gv.F(1.2))), Gd.Ctor("Color", Gv.S("f2f5f7")), Gv.F(1.2));
      Self.C("draw_line", ((c - (nr * (((w * Gv.F(0.5)) + Gv.F(1.0))))) + (d * Gv.F(6.0))), ((c + (nr * (((w * Gv.F(0.5)) + Gv.F(1.0))))) + (d * Gv.F(6.0))), Gd.Ctor("Color", Gv.S("5a646c")), Gv.F(1.0));
    }
    return Gv.Null;
  }
  public Gv _pipe_joints(Gv P = default(Gv)) {
    Gv pp = P.I(Gv.I(0L));
    Gv w = (P.I(Gv.I(1L))).AsF();
    if ((Gv.Lt(pp.C("size"), Gv.I(2L)) || Gv.Lt(w, Gv.F(8.0)))) {
      return Gv.Null;
    }
    foreach (Gv i in ((pp.C("size") - Gv.I(1L))).Iter()) {
      Gv a = pp.I(i);
      Gv b = pp.I((i + Gv.I(1L)));
      Gv L = (a.C("distance_to", b)).AsF();
      if (Gv.Lt(L, Gv.F(300.0))) {
        continue;
      }
      Gv d = (((b - a)) / L);
      Gv nr = Gd.Ctor("Vector2", (-d.G("y")), d.G("x"));
      Gv n = (Gd.Dyn("int", (L / Gv.F(280.0)))).AsI();
      foreach (Gv k in (n).Iter()) {
        Gv c = (a + (d * (((L * ((k + Gv.F(0.5)))) / n))));
        Gv near = Gv.B(false);
        foreach (Gv q in (_occ).Iter()) {
          if (Gv.Lt(q.C("distance_to", c), Gv.F(56.0))) {
            near = Gv.B(true);
            break;
          }
        }
        if ((near).T()) {
          continue;
        }
        foreach (Gv e in (Gd.Arr(a, b)).Iter()) {
          if (Gv.Lt(e.C("distance_to", c), Gv.F(50.0))) {
            near = Gv.B(true);
          }
        }
        if ((near).T()) {
          continue;
        }
        Gv hw = (((w * Gv.F(0.5)) + Gv.F(3.0))).AsF();
        foreach (Gv fo in (Gd.Arr(Gv.F(-4.5), Gv.F(4.5))).Iter()) {
          Self.C("draw_line", ((c + (d * fo)) - (nr * hw)), ((c + (d * fo)) + (nr * hw)), Gd.Ctor("Color", Gv.S("1e2429")), Gv.F(4.5));
          Self.C("draw_line", ((c + (d * fo)) - (nr * ((hw - Gv.F(0.8))))), ((c + (d * fo)) + (nr * ((hw - Gv.F(0.8))))), Gd.Ctor("Color", Gv.S("b9c2ca")), Gv.F(2.8));
        }
        Self.C("draw_line", (c - (nr * ((hw + Gv.F(1.0))))), (c + (nr * ((hw + Gv.F(1.0))))), Gd.Ctor("Color", Gv.S("1e2429")), Gv.F(6.0));
        Self.C("draw_line", (c - (nr * hw)), (c + (nr * hw)), Gd.Ctor("Color", Gv.S("8e98a2")), Gv.F(4.2));
        Self.C("draw_line", ((c - (nr * hw)) - (d * Gv.F(1.0))), ((c + (nr * hw)) - (d * Gv.F(1.0))), Gd.Ctor("Color", Gv.S("e9eef1")), Gv.F(1.2));
        Gv wn = (c + (nr * ((hw + Gv.F(3.5)))));
        Self.C("draw_circle", wn, Gv.F(3.6), Gd.Ctor("Color", Gv.S("1e2429")));
        Self.C("draw_circle", wn, Gv.F(2.6), Gd.Ctor("Color", Gv.S("aeb7bf")));
        Self.C("draw_circle", (wn - Gd.Ctor("Vector2", Gv.F(0.7), Gv.F(0.7))), Gv.F(1.0), Gd.Ctor("Color", Gv.S("ffffff")));
        Self.C("draw_line", wn, (wn + (nr * Gv.F(2.5))), Gd.Ctor("Color", Gv.S("5a646c")), Gv.F(2.0));
      }
    }
    return Gv.Null;
  }
  public Gv _joint_near(Gv joints = default(Gv), Gv c = default(Gv)) {
    foreach (Gv j in (joints).Iter()) {
      if ((Gv.Lt(Gd.absf((j.I(Gv.I(0L)) - c.G("x"))), Gv.F(2.5)) && Gv.Lt(Gd.absf((j.I(Gv.I(1L)) - c.G("y"))), Gv.F(2.5)))) {
        return Gv.B(true);
      }
    }
    return Gv.B(false);
    return Gv.Null;
  }
  public Gv _vgrad(Gv r = default(Gv), Gv a = default(Gv), Gv b = default(Gv)) {
    if ((Gv.Le(r.G("size").G("x"), Gv.F(0.0)) || Gv.Le(r.G("size").G("y"), Gv.F(0.0)))) {
      return Gv.Null;
    }
    Self.C("draw_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(r.G("position"), Gd.Ctor("Vector2", r.G("end").G("x"), r.G("position").G("y")), r.G("end"), Gd.Ctor("Vector2", r.G("position").G("x"), r.G("end").G("y")))), Gd.Ctor("PackedColorArray", Gd.Arr(a, a, b, b)));
    return Gv.Null;
  }
  public Gv _hall(Gv B = default(Gv), Gv G = default(Gv)) {
    Self.C("draw_rect", (Gv.Eq(clip.G("size"), Gd.StaticGet("Vector2", "ZERO")) ? B : G), Gd.StaticGet("Color", "WHITE"));
    return Gv.Null;
  }
  public Gv _glow(Gv c = default(Gv), Gv rx = default(Gv), Gv ry = default(Gv), Gv col = default(Gv), Gv n = default(Gv)) {
    rx = (rx).AsF();
    ry = (ry).AsF();
    if (n.k == 0) n = Gv.I(40L);
    Gv edge = Gd.Ctor("Color", col, Gv.F(0.0));
    foreach (Gv i in (n).Iter()) {
      Gv a0 = ((Gd.K("TAU") * i) / n);
      Gv a1 = ((Gd.K("TAU") * ((i + Gv.I(1L)))) / n);
      Self.C("draw_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(c, (c + Gd.Ctor("Vector2", (Gd.cos(a0) * rx), (Gd.sin(a0) * ry))), (c + Gd.Ctor("Vector2", (Gd.cos(a1) * rx), (Gd.sin(a1) * ry))))), Gd.Ctor("PackedColorArray", Gd.Arr(col, edge, edge)));
    }
    return Gv.Null;
  }
  public Gv _reflect(Gv x0 = default(Gv), Gv y0 = default(Gv), Gv w = default(Gv), Gv h = default(Gv), Gv c = default(Gv), Gv a = default(Gv)) {
    x0 = (x0).AsF();
    y0 = (y0).AsF();
    w = (w).AsF();
    h = (h).AsF();
    a = (a).AsF();
    Gv e = Gd.minf((w * Gv.F(0.18)), Gv.F(24.0));
    Gv xs = Gd.Arr(x0, (x0 + e), ((x0 + w) - e), (x0 + w));
    Gv at = Gd.Arr(Gv.F(0.0), a, a, Gv.F(0.0));
    foreach (Gv i in (Gv.I(3L)).Iter()) {
      Self.C("draw_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", xs.I(i), y0), Gd.Ctor("Vector2", xs.I((i + Gv.I(1L))), y0), Gd.Ctor("Vector2", xs.I((i + Gv.I(1L))), (y0 + h)), Gd.Ctor("Vector2", xs.I(i), (y0 + h)))), Gd.Ctor("PackedColorArray", Gd.Arr(Gd.Ctor("Color", c, at.I(i)), Gd.Ctor("Color", c, at.I((i + Gv.I(1L)))), Gd.Ctor("Color", c, Gv.F(0.0)), Gd.Ctor("Color", c, Gv.F(0.0)))));
    }
    return Gv.Null;
  }
  public Gv _lamp_items() {
    Gv out_ = Gd.Arr();
    foreach (Gv it in (m.G("items")).Iter()) {
      if ((Gv.Eq(it.G("type"), Gv.S("vessel")) && Gv.In(it.C("get", Gv.S("shape")), Gd.Arr(Gv.S("tank"), Gv.S("lauter"), Gv.S("cone"), Gv.S("tall"), Gv.S("horiz"))))) {
        out_.C("append", it);
      }
    }
    return out_;
    return Gv.Null;
  }
  public Gv _hall_dark(Gv B = default(Gv), Gv G = default(Gv)) {
    Gv R = (Gv.Eq(clip.G("size"), Gd.StaticGet("Vector2", "ZERO")) ? B : G);
    Gv fy = (m.G("floor_y")).AsF();
    Gv ty0 = (fy - Gv.F(300.0));
    Gv ceil_y = (B.G("position").G("y") + Gv.F(30.0));
    Gv wall_top = Gd.Ctor("Color", Gv.S("101317"));
    Gv wall_bot = Gd.Ctor("Color", Gv.S("2a3036"));
    Gv k0 = Gd.clampf((((R.G("position").G("y") - B.G("position").G("y"))) / Gd.maxf(Gv.F(1.0), (ty0 - B.G("position").G("y")))), Gv.F(0.0), Gv.F(1.0));
    Gv k1 = Gd.clampf((((Gd.minf(R.G("end").G("y"), ty0) - B.G("position").G("y"))) / Gd.maxf(Gv.F(1.0), (ty0 - B.G("position").G("y")))), Gv.F(0.0), Gv.F(1.0));
    _vgrad(Gd.Ctor("Rect2", R.G("position").G("x"), R.G("position").G("y"), R.G("size").G("x"), (Gd.minf(R.G("end").G("y"), ty0) - R.G("position").G("y"))), wall_top.C("lerp", wall_bot, k0), wall_top.C("lerp", wall_bot, k1));
    Gv px = (Gd.floorf((R.G("position").G("x") / Gv.F(400.0))) * Gv.F(400.0));
    while (Gv.Lt(px, R.G("end").G("x"))) {
      Self.C("draw_line", Gd.Ctor("Vector2", px, Gd.maxf(R.G("position").G("y"), ceil_y)), Gd.Ctor("Vector2", px, Gd.minf(R.G("end").G("y"), ty0)), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.35)), Gv.F(2.0));
      Self.C("draw_line", Gd.Ctor("Vector2", (px + Gv.I(2L)), Gd.maxf(R.G("position").G("y"), ceil_y)), Gd.Ctor("Vector2", (px + Gv.I(2L)), Gd.minf(R.G("end").G("y"), ty0)), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.035)), Gv.F(1.0));
      px = (px + Gv.F(400.0));
    }
    if ((Gv.Gt(R.G("end").G("y"), ty0) && Gv.Lt(R.G("position").G("y"), fy))) {
      Gv tr = Gd.Ctor("Rect2", R.G("position").G("x"), Gd.maxf(ty0, R.G("position").G("y")), R.G("size").G("x"), (Gd.minf(fy, R.G("end").G("y")) - Gd.maxf(ty0, R.G("position").G("y"))));
      _vgrad(tr, Gd.Ctor("Color", Gv.S("343b41")), Gd.Ctor("Color", Gv.S("2b3136")));
      Gv gy = (ty0 + Gv.F(50.0));
      while (Gv.Lt(gy, fy)) {
        if ((Gv.Gt(gy, (R.G("position").G("y") - Gv.I(2L))) && Gv.Lt(gy, (R.G("end").G("y") + Gv.I(2L))))) {
          Self.C("draw_line", Gd.Ctor("Vector2", R.G("position").G("x"), gy), Gd.Ctor("Vector2", R.G("end").G("x"), gy), Gd.Ctor("Color", Gv.S("1d2125")), Gv.F(2.0));
          Self.C("draw_line", Gd.Ctor("Vector2", R.G("position").G("x"), (gy + Gv.I(2L))), Gd.Ctor("Vector2", R.G("end").G("x"), (gy + Gv.I(2L))), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.045)), Gv.F(1.0));
        }
        gy = (gy + Gv.F(50.0));
      }
      Gv row = Gv.I(0L);
      Gv ry = ty0;
      while (Gv.Lt(ry, fy)) {
        Gv off = (Gv.Eq((row % Gv.I(2L)), Gv.I(1L)) ? Gv.F(50.0) : Gv.F(0.0));
        Gv gx = ((Gd.floorf((((R.G("position").G("x") - off)) / Gv.F(100.0))) * Gv.F(100.0)) + off);
        while (Gv.Lt(gx, R.G("end").G("x"))) {
          Self.C("draw_line", Gd.Ctor("Vector2", gx, Gd.maxf(ry, R.G("position").G("y"))), Gd.Ctor("Vector2", gx, Gd.minf(Gd.minf((ry + Gv.F(50.0)), fy), R.G("end").G("y"))), Gd.Ctor("Color", Gv.S("1d2125")), Gv.F(2.0));
          gx = (gx + Gv.F(100.0));
        }
        ry = (ry + Gv.F(50.0));
        row = (row + Gv.I(1L));
      }
      Self.C("draw_line", Gd.Ctor("Vector2", R.G("position").G("x"), ty0), Gd.Ctor("Vector2", R.G("end").G("x"), ty0), Gd.Ctor("Color", Gv.S("596169")), Gv.F(5.0));
      Self.C("draw_line", Gd.Ctor("Vector2", R.G("position").G("x"), (ty0 - Gv.I(2L))), Gd.Ctor("Vector2", R.G("end").G("x"), (ty0 - Gv.I(2L))), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.18)), Gv.F(1.0));
    }
    if (Gv.Lt(R.G("position").G("y"), (ceil_y + Gv.I(10L)))) {
      _vgrad(Gd.Ctor("Rect2", R.G("position").G("x"), B.G("position").G("y"), R.G("size").G("x"), (ceil_y - B.G("position").G("y"))), Gd.Ctor("Color", Gv.S("07090b")), Gd.Ctor("Color", Gv.S("15191d")));
      Self.C("draw_line", Gd.Ctor("Vector2", R.G("position").G("x"), ceil_y), Gd.Ctor("Vector2", R.G("end").G("x"), ceil_y), Gd.Ctor("Color", Gv.S("3a4148")), Gv.F(3.0));
    }
    Gv lamps = _lamp_items();
    foreach (Gv it in (lamps).Iter()) {
      Gv cx = ((it.G("x") + (it.G("w") * Gv.F(0.5)))).AsF();
      Gv ly = (ceil_y + Gv.F(70.0));
      Gv top_y = (Gd.maxf((ly + Gv.F(30.0)), (Gd.Dyn("float", it.G("y")) - Gv.F(60.0)))).AsF();
      Gv warm = Gd.Ctor("Color", Gv.F(1.0), Gv.F(0.87), Gv.F(0.62));
      _glow(Gd.Ctor("Vector2", cx, (it.G("y") + (it.G("h") * Gv.F(0.45)))), (it.G("w") * Gv.F(1.1)), ((it.G("h") * Gv.F(0.8)) + Gv.F(140.0)), Gd.Ctor("Color", warm, Gv.F(0.15)), Gv.I(48L));
      Self.C("draw_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", (cx - Gv.I(30L)), (ly + Gv.I(22L))), Gd.Ctor("Vector2", (cx + Gv.I(30L)), (ly + Gv.I(22L))), Gd.Ctor("Vector2", ((cx + (it.G("w") * Gv.F(0.75))) + Gv.I(60L)), (top_y + Gv.I(140L))), Gd.Ctor("Vector2", ((cx - (it.G("w") * Gv.F(0.75))) - Gv.I(60L)), (top_y + Gv.I(140L))))), Gd.Ctor("PackedColorArray", Gd.Arr(Gd.Ctor("Color", warm, Gv.F(0.16)), Gd.Ctor("Color", warm, Gv.F(0.16)), Gd.Ctor("Color", warm, Gv.F(0.0)), Gd.Ctor("Color", warm, Gv.F(0.0)))));
    }
    if (Gv.Gt(R.G("end").G("y"), (fy - Gv.F(16.0)))) {
      _vgrad(Gd.Ctor("Rect2", R.G("position").G("x"), (fy - Gv.F(16.0)), R.G("size").G("x"), Gv.F(16.0)), Gd.Ctor("Color", Gv.S("3f474e")), Gd.Ctor("Color", Gv.S("1a1e22")));
      Gv fr = Gd.Ctor("Rect2", R.G("position").G("x"), Gd.maxf(fy, R.G("position").G("y")), R.G("size").G("x"), (R.G("end").G("y") - Gd.maxf(fy, R.G("position").G("y"))));
      Gv kf0 = Gd.clampf((((fr.G("position").G("y") - fy)) / Gd.maxf(Gv.F(1.0), (B.G("end").G("y") - fy))), Gv.F(0.0), Gv.F(1.0));
      Gv kf1 = Gd.clampf((((fr.G("end").G("y") - fy)) / Gd.maxf(Gv.F(1.0), (B.G("end").G("y") - fy))), Gv.F(0.0), Gv.F(1.0));
      _vgrad(fr, Gd.Ctor("Color", Gv.S("262b30")).C("lerp", Gd.Ctor("Color", Gv.S("0e1013")), kf0), Gd.Ctor("Color", Gv.S("262b30")).C("lerp", Gd.Ctor("Color", Gv.S("0e1013")), kf1));
      Self.C("draw_line", Gd.Ctor("Vector2", R.G("position").G("x"), fy), Gd.Ctor("Vector2", R.G("end").G("x"), fy), Gd.Ctor("Color", Gv.S("0a0c0e")), Gv.F(2.0));
      Self.C("draw_line", Gd.Ctor("Vector2", R.G("position").G("x"), (fy + Gv.I(2L))), Gd.Ctor("Vector2", R.G("end").G("x"), (fy + Gv.I(2L))), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.1)), Gv.F(1.0));
      foreach (Gv it_2 in (lamps).Iter()) {
        Gv cx2 = ((it_2.G("x") + (it_2.G("w") * Gv.F(0.5)))).AsF();
        _glow(Gd.Ctor("Vector2", cx2, (fy + Gv.F(70.0))), (it_2.G("w") * Gv.F(0.95)), Gv.F(55.0), Gd.Ctor("Color", Gv.F(1.0), Gv.F(0.86), Gv.F(0.6), Gv.F(0.1)), Gv.I(36L));
        Gv rh = Gd.minf(Gv.F(200.0), ((B.G("end").G("y") - fy) - Gv.F(4.0)));
        if (Gv.Gt(rh, Gv.F(10.0))) {
          Gv on_floor = Gv.B(Gv.Ge(m.C("foot_y", it_2), (fy - Gv.F(2.0))));
          Gv refl = Gd.Arr();
          if ((on_floor).T()) {
            refl.C("append", Gd.Arr((Gd.Dyn("float", it_2.G("x")) + Gv.F(6.0)), (Gd.Dyn("float", it_2.G("w")) - Gv.F(12.0)), rh, Gv.F(0.11)));
          } else {
            refl.C("append", Gd.Arr((Gd.Dyn("float", it_2.G("x")) - Gv.F(22.0)), Gv.F(14.0), (rh * Gv.F(0.6)), Gv.F(0.16)));
            refl.C("append", Gd.Arr(((Gd.Dyn("float", it_2.G("x")) + Gd.Dyn("float", it_2.G("w"))) + Gv.F(8.0)), Gv.F(14.0), (rh * Gv.F(0.6)), Gv.F(0.16)));
          }
          foreach (Gv rf in (refl).Iter()) {
            _reflect(rf.I(Gv.I(0L)), (fy + Gv.F(3.0)), rf.I(Gv.I(1L)), rf.I(Gv.I(2L)), Gd.Ctor("Color", Gv.S("aeb7bf")), rf.I(Gv.I(3L)));
          }
        }
      }
      Gv fl = (fy + Gv.F(110.0));
      while (Gv.Lt(fl, R.G("end").G("y"))) {
        Self.C("draw_line", Gd.Ctor("Vector2", R.G("position").G("x"), fl), Gd.Ctor("Vector2", R.G("end").G("x"), fl), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.025)), Gv.F(2.0));
        fl = (fl + Gv.F(130.0));
      }
      Gv dy = (fy + Gv.F(46.0));
      if (Gv.Lt((dy + Gv.F(16.0)), B.G("end").G("y"))) {
        Self.C("draw_rect", Gd.Ctor("Rect2", R.G("position").G("x"), dy, R.G("size").G("x"), Gv.F(16.0)), Gd.Ctor("Color", Gv.S("2b3136")));
        Self.C("draw_rect", Gd.Ctor("Rect2", R.G("position").G("x"), (dy + Gv.F(1.0)), R.G("size").G("x"), Gv.F(2.0)), Gd.Ctor("Color", Gv.S("7c858d")));
        Gv sx = (Gd.floorf((R.G("position").G("x") / Gv.F(14.0))) * Gv.F(14.0));
        while (Gv.Lt(sx, R.G("end").G("x"))) {
          Self.C("draw_rect", Gd.Ctor("Rect2", sx, (dy + Gv.F(5.0)), Gv.F(8.0), Gv.F(9.0)), Gd.Ctor("Color", Gv.S("0b0d0f")));
          sx = (sx + Gv.F(14.0));
        }
      }
    }
    _stands(R, fy);
    foreach (Gv it_3 in (lamps).Iter()) {
      Gv cx3 = ((it_3.G("x") + (it_3.G("w") * Gv.F(0.5)))).AsF();
      Gv ly2 = (ceil_y + Gv.F(70.0));
      Self.C("draw_line", Gd.Ctor("Vector2", cx3, ceil_y), Gd.Ctor("Vector2", cx3, ly2), Gd.Ctor("Color", Gv.S("2b3136")), Gv.F(3.0));
      _glow(Gd.Ctor("Vector2", cx3, (ly2 + Gv.F(24.0))), Gv.F(70.0), Gv.F(40.0), Gd.Ctor("Color", Gv.F(1.0), Gv.F(0.9), Gv.F(0.65), Gv.F(0.35)), Gv.I(32L));
      Gv shade = Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", (cx3 - Gv.I(8L)), ly2), Gd.Ctor("Vector2", (cx3 + Gv.I(8L)), ly2), Gd.Ctor("Vector2", (cx3 + Gv.I(36L)), (ly2 + Gv.I(22L))), Gd.Ctor("Vector2", (cx3 - Gv.I(36L)), (ly2 + Gv.I(22L)))));
      Self.C("draw_polygon", shade, Gd.Ctor("PackedColorArray", Gd.Arr(Gd.Ctor("Color", Gv.S("4a535b")), Gd.Ctor("Color", Gv.S("4a535b")), Gd.Ctor("Color", Gv.S("1b2025")), Gd.Ctor("Color", Gv.S("2b3136")))));
      Self.C("draw_line", Gd.Ctor("Vector2", (cx3 - Gv.I(36L)), (ly2 + Gv.I(22L))), Gd.Ctor("Vector2", (cx3 + Gv.I(36L)), (ly2 + Gv.I(22L))), Gd.Ctor("Color", Gv.S("6c757d")), Gv.F(2.0));
      Self.C("draw_rect", Gd.Ctor("Rect2", (cx3 - Gv.I(26L)), (ly2 + Gv.I(22L)), Gv.I(52L), Gv.I(5L)), Gd.Ctor("Color", Gv.S("fff1c8")));
    }
    if (Gv.Eq(clip.G("size"), Gd.StaticGet("Vector2", "ZERO"))) {
      Self.C("draw_rect", B, Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.5)), Gv.B(false), Gv.F(6.0));
    }
    return Gv.Null;
  }
  public Gv _hall_light(Gv B = default(Gv), Gv G = default(Gv)) {
    Gv R = (Gv.Eq(clip.G("size"), Gd.StaticGet("Vector2", "ZERO")) ? B : G);
    Gv fy = (m.G("floor_y")).AsF();
    Gv wall_top = Gd.Ctor("Color", Gv.S("c9d0d6"));
    Gv wall_bot = Gd.Ctor("Color", Gv.S("e3e8ec"));
    Gv k0 = Gd.clampf((((R.G("position").G("y") - B.G("position").G("y"))) / Gd.maxf(Gv.F(1.0), (fy - B.G("position").G("y")))), Gv.F(0.0), Gv.F(1.0));
    Gv k1 = Gd.clampf((((Gd.minf(R.G("end").G("y"), fy) - B.G("position").G("y"))) / Gd.maxf(Gv.F(1.0), (fy - B.G("position").G("y")))), Gv.F(0.0), Gv.F(1.0));
    _vgrad(Gd.Ctor("Rect2", R.G("position").G("x"), R.G("position").G("y"), R.G("size").G("x"), (Gd.minf(R.G("end").G("y"), fy) - R.G("position").G("y"))), wall_top.C("lerp", wall_bot, k0), wall_top.C("lerp", wall_bot, k1));
    Gv px = (Gd.floorf((R.G("position").G("x") / Gv.F(400.0))) * Gv.F(400.0));
    while (Gv.Lt(px, R.G("end").G("x"))) {
      Self.C("draw_line", Gd.Ctor("Vector2", px, R.G("position").G("y")), Gd.Ctor("Vector2", px, Gd.minf(R.G("end").G("y"), (fy - Gv.F(300.0)))), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.06)), Gv.F(2.0));
      Self.C("draw_line", Gd.Ctor("Vector2", (px + Gv.I(2L)), R.G("position").G("y")), Gd.Ctor("Vector2", (px + Gv.I(2L)), Gd.minf(R.G("end").G("y"), (fy - Gv.F(300.0)))), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.25)), Gv.F(1.0));
      px = (px + Gv.F(400.0));
    }
    Gv ty0 = (fy - Gv.F(300.0));
    if ((Gv.Gt(R.G("end").G("y"), ty0) && Gv.Lt(R.G("position").G("y"), fy))) {
      _vgrad(Gd.Ctor("Rect2", R.G("position").G("x"), Gd.maxf(ty0, R.G("position").G("y")), R.G("size").G("x"), (Gd.minf(fy, R.G("end").G("y")) - Gd.maxf(ty0, R.G("position").G("y")))), Gd.Ctor("Color", Gv.S("eef1f3")), Gd.Ctor("Color", Gv.S("e2e6e9")));
      Self.C("draw_line", Gd.Ctor("Vector2", R.G("position").G("x"), ty0), Gd.Ctor("Vector2", R.G("end").G("x"), ty0), Gd.Ctor("Color", Gv.S("9aa4ad")), Gv.F(4.0));
      Self.C("draw_line", Gd.Ctor("Vector2", R.G("position").G("x"), (ty0 + Gv.I(3L))), Gd.Ctor("Vector2", R.G("end").G("x"), (ty0 + Gv.I(3L))), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.7)), Gv.F(1.5));
      Gv gy = (ty0 + Gv.F(50.0));
      while (Gv.Lt(gy, fy)) {
        if ((Gv.Gt(gy, (R.G("position").G("y") - Gv.I(2L))) && Gv.Lt(gy, (R.G("end").G("y") + Gv.I(2L))))) {
          Self.C("draw_line", Gd.Ctor("Vector2", R.G("position").G("x"), gy), Gd.Ctor("Vector2", R.G("end").G("x"), gy), Gd.Ctor("Color", Gv.S("cfd6dc")), Gv.F(1.5));
        }
        gy = (gy + Gv.F(50.0));
      }
      Gv gx = (Gd.floorf((R.G("position").G("x") / Gv.F(50.0))) * Gv.F(50.0));
      while (Gv.Lt(gx, R.G("end").G("x"))) {
        Self.C("draw_line", Gd.Ctor("Vector2", gx, Gd.maxf(ty0, R.G("position").G("y"))), Gd.Ctor("Vector2", gx, Gd.minf(fy, R.G("end").G("y"))), Gd.Ctor("Color", Gv.S("d6dce1")), Gv.F(1.2));
        gx = (gx + Gv.F(50.0));
      }
    }
    if (Gv.Gt(R.G("end").G("y"), fy)) {
      _vgrad(Gd.Ctor("Rect2", R.G("position").G("x"), (fy - Gv.F(14.0)), R.G("size").G("x"), Gv.F(14.0)), Gd.Ctor("Color", Gv.S("8e979f")), Gd.Ctor("Color", Gv.S("5d666e")));
      Gv fr = Gd.Ctor("Rect2", R.G("position").G("x"), Gd.maxf(fy, R.G("position").G("y")), R.G("size").G("x"), (R.G("end").G("y") - Gd.maxf(fy, R.G("position").G("y"))));
      Gv kf0 = Gd.clampf((((fr.G("position").G("y") - fy)) / Gd.maxf(Gv.F(1.0), (B.G("end").G("y") - fy))), Gv.F(0.0), Gv.F(1.0));
      Gv kf1 = Gd.clampf((((fr.G("end").G("y") - fy)) / Gd.maxf(Gv.F(1.0), (B.G("end").G("y") - fy))), Gv.F(0.0), Gv.F(1.0));
      _vgrad(fr, Gd.Ctor("Color", Gv.S("a3abb0")).C("lerp", Gd.Ctor("Color", Gv.S("7a8389")), kf0), Gd.Ctor("Color", Gv.S("a3abb0")).C("lerp", Gd.Ctor("Color", Gv.S("7a8389")), kf1));
      Self.C("draw_line", Gd.Ctor("Vector2", R.G("position").G("x"), fy), Gd.Ctor("Vector2", R.G("end").G("x"), fy), Gd.Ctor("Color", Gv.S("3b434a")), Gv.F(2.0));
      Self.C("draw_line", Gd.Ctor("Vector2", R.G("position").G("x"), (fy + Gv.I(2L))), Gd.Ctor("Vector2", R.G("end").G("x"), (fy + Gv.I(2L))), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.25)), Gv.F(1.0));
      Gv fx = (Gd.floorf((R.G("position").G("x") / Gv.F(200.0))) * Gv.F(200.0));
      while (Gv.Lt(fx, (R.G("end").G("x") + Gv.F(200.0)))) {
        Self.C("draw_line", Gd.Ctor("Vector2", fx, (fy + Gv.I(4L))), Gd.Ctor("Vector2", (fx - Gv.F(60.0)), B.G("end").G("y")), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.06)), Gv.F(2.0));
        fx = (fx + Gv.F(200.0));
      }
      Gv fl = (fy + Gv.F(90.0));
      while (Gv.Lt(fl, R.G("end").G("y"))) {
        Self.C("draw_line", Gd.Ctor("Vector2", R.G("position").G("x"), fl), Gd.Ctor("Vector2", R.G("end").G("x"), fl), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.07)), Gv.F(2.0));
        fl = (fl + Gv.F(120.0));
      }
      Gv dy = (fy + Gv.F(46.0));
      if (Gv.Lt((dy + Gv.F(16.0)), B.G("end").G("y"))) {
        Self.C("draw_rect", Gd.Ctor("Rect2", R.G("position").G("x"), dy, R.G("size").G("x"), Gv.F(16.0)), Gd.Ctor("Color", Gv.S("4d565e")));
        Self.C("draw_rect", Gd.Ctor("Rect2", R.G("position").G("x"), (dy + Gv.F(1.0)), R.G("size").G("x"), Gv.F(3.0)), Gd.Ctor("Color", Gv.S("c2cad1")));
        Gv sx = (Gd.floorf((R.G("position").G("x") / Gv.F(14.0))) * Gv.F(14.0));
        while (Gv.Lt(sx, R.G("end").G("x"))) {
          Self.C("draw_rect", Gd.Ctor("Rect2", sx, (dy + Gv.F(5.0)), Gv.F(8.0), Gv.F(9.0)), Gd.Ctor("Color", Gv.S("262c31")));
          sx = (sx + Gv.F(14.0));
        }
      }
    }
    _stands(R, fy);
    if (Gv.Eq(clip.G("size"), Gd.StaticGet("Vector2", "ZERO"))) {
      Self.C("draw_rect", B, Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.35)), Gv.B(false), Gv.F(6.0));
    }
    return Gv.Null;
  }
  public Gv _stands(Gv R = default(Gv), Gv fy = default(Gv)) {
    fy = (fy).AsF();
    foreach (Gv it in (m.G("items")).Iter()) {
      if ((Gv.Ne(it.G("type"), Gv.S("vessel")) || !Gv.In(it.C("get", Gv.S("shape")), Gd.Arr(Gv.S("tank"), Gv.S("lauter"), Gv.S("cone"), Gv.S("tall"))))) {
        continue;
      }
      Gv x = (it.G("x")).AsF();
      Gv w = (it.G("w")).AsF();
      Gv yd = (((Gd.Dyn("float", it.G("y")) + Gd.Dyn("float", it.G("h"))) + Gv.F(38.0))).AsF();
      if (Gv.Lt((fy - yd), Gv.F(30.0))) {
        continue;
      }
      if (!(R.C("intersects", Gd.Ctor("Rect2", (x - Gv.I(60L)), (yd - Gv.I(20L)), (w + Gv.I(120L)), ((fy - yd) + Gv.I(40L))))).T()) {
        continue;
      }
      Gv cols = Gd.Arr((x - Gv.F(22.0)), ((x + w) + Gv.F(8.0)));
      foreach (Gv cx in (cols).Iter()) {
        Self.C("draw_rect", Gd.Ctor("Rect2", (cx + Gv.I(6L)), (yd + Gv.I(12L)), Gv.I(14L), ((fy - yd) - Gv.I(12L))), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.08)));
        _hgrad(Gd.Ctor("Rect2", cx, (yd + Gv.I(8L)), Gv.I(14L), ((fy - yd) - Gv.I(8L))), Gd.Ctor("Color", Gv.S("6f7880")), Gd.Ctor("Color", Gv.S("c3cbd2")), Gd.Ctor("Color", Gv.S("5a636b")));
        Self.C("draw_rect", Gd.Ctor("Rect2", (cx - Gv.I(5L)), (fy - Gv.I(6L)), Gv.I(24L), Gv.I(6L)), Gd.Ctor("Color", Gv.S("3b434a")));
      }
      Gv h = ((fy - yd) - Gv.F(12.0));
      if (Gv.Gt(h, Gv.F(60.0))) {
        Gv a0 = Gd.Ctor("Vector2", (cols.I(Gv.I(0L)) + Gv.I(14L)), (yd + Gv.I(14L)));
        Gv a1 = Gd.Ctor("Vector2", cols.I(Gv.I(1L)), (fy - Gv.I(10L)));
        Gv b0 = Gd.Ctor("Vector2", cols.I(Gv.I(1L)), (yd + Gv.I(14L)));
        Gv b1 = Gd.Ctor("Vector2", (cols.I(Gv.I(0L)) + Gv.I(14L)), (fy - Gv.I(10L)));
        Self.C("draw_line", a0, a1, Gd.Ctor("Color", Gv.S("7c858d")), Gv.F(3.0), Gv.B(true));
        Self.C("draw_line", b0, b1, Gd.Ctor("Color", Gv.S("7c858d")), Gv.F(3.0), Gv.B(true));
      }
      Self.C("draw_rect", Gd.Ctor("Rect2", (x - Gv.I(30L)), (yd + Gv.I(6L)), (w + Gv.I(60L)), Gv.I(8L)), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.12)));
      _vgrad(Gd.Ctor("Rect2", (x - Gv.I(30L)), (yd - Gv.I(2L)), (w + Gv.I(60L)), Gv.I(10L)), Gd.Ctor("Color", Gv.S("c9d1d8")), Gd.Ctor("Color", Gv.S("6c757d")));
      Self.C("draw_rect", Gd.Ctor("Rect2", (x - Gv.I(30L)), (yd - Gv.I(2L)), (w + Gv.I(60L)), Gv.I(10L)), Gd.Ctor("Color", Gv.S("2a3137")), Gv.B(false), Gv.F(1.0));
    }
    return Gv.Null;
  }
  public Gv _hgrad(Gv r = default(Gv), Gv a = default(Gv), Gv mid = default(Gv), Gv b = default(Gv)) {
    Gv xm = (r.G("position").G("x") + (r.G("size").G("x") * Gv.F(0.35)));
    Self.C("draw_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(r.G("position"), Gd.Ctor("Vector2", xm, r.G("position").G("y")), Gd.Ctor("Vector2", xm, r.G("end").G("y")), Gd.Ctor("Vector2", r.G("position").G("x"), r.G("end").G("y")))), Gd.Ctor("PackedColorArray", Gd.Arr(a, mid, mid, a)));
    Self.C("draw_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", xm, r.G("position").G("y")), Gd.Ctor("Vector2", r.G("end").G("x"), r.G("position").G("y")), r.G("end"), Gd.Ctor("Vector2", xm, r.G("end").G("y")))), Gd.Ctor("PackedColorArray", Gd.Arr(mid, b, b, mid)));
    return Gv.Null;
  }
  public Gv _draw() {
    Gv B = m.G("bounds");
    Gv G = (Gv.Eq(clip.G("size"), Gd.StaticGet("Vector2", "ZERO")) ? B : B.C("intersection", clip.C("grow", Gv.F(60.0))));
    _hall(B, G);
    foreach (Gv it in (m.G("items")).Iter()) {
      if (Gv.Eq(it.G("type"), Gv.S("zone"))) {
        Gv r = Gd.Ctor("Rect2", it.G("x"), it.G("y"), it.G("w"), it.G("h"));
        if (!(m.G("hall_dark")).T()) {
          Self.C("draw_rect", r, Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.07)));
          Self.C("draw_rect", r, Gd.Ctor("Color", Gv.F(0.35), Gv.F(0.4), Gv.F(0.45), Gv.F(0.12)), Gv.B(false), Gv.F(2.0));
        }
        Gv tw = ((m.G("font_b").C("get_string_size", it.G("label"), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(22L)).G("x") + Gv.I(36L))).AsF();
        S_art.rrect(Self, Gd.Ctor("Rect2", (it.G("x") + Gv.I(14L)), (it.G("y") + Gv.I(14L)), tw, Gv.I(36L)), Gd.Ctor("Color", Gv.S("f2c230")), Gv.F(5.0));
        Self.C("draw_string", m.G("font_b"), Gd.Ctor("Vector2", (it.G("x") + Gv.I(32L)), (it.G("y") + Gv.I(41L))), it.G("label"), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(22L), Gd.Ctor("Color", Gv.S("1b2026")));
      }
    }
    Gv pipes = Gd.Arr();
    Gv solo = Gd.Arr();
    foreach (Gv it_2 in (m.G("items")).Iter()) {
      if ((Gv.Eq(it_2.G("type"), Gv.S("pipe")) && Gv.Ge(it_2.G("pts").C("size"), Gv.I(2L)))) {
        if ((it_2.C("get", Gv.S("hose"), Gv.B(false))).T()) {
          continue;
        }
        Gv pts = Gd.Ctor("PackedVector2Array");
        foreach (Gv p in (it_2.G("pts")).Iter()) {
          pts.C("append", Gd.Ctor("Vector2", p.I(Gv.I(0L)), p.I(Gv.I(1L))));
        }
        Gv dn = Gd.Dyn("float", it_2.C("get", Gv.S("dn"), Gv.I(50L)));
        Gv w = (Gv.Le(dn, Gv.I(32L)) ? Gd.clampf((dn * Gv.F(0.24)), Gv.F(3.0), Gv.F(6.0)) : Gd.maxf(Gv.F(9.0), Gd.roundf(((Gv.F(12.0) * dn) / Gv.F(50.0)))));
        if (!(_seen(pts)).T()) {
          continue;
        }
        if ((it_2.C("get", Gv.S("solo"), Gv.B(false))).T()) {
          solo.C("append", Gd.Arr(pts, w));
        } else {
          pipes.C("append", Gd.Arr(pts, w));
        }
      } else if (Gv.Eq(it_2.G("type"), Gv.S("manifold"))) {
        foreach (Gv q in (m.C("mf_ports", it_2)).Iter()) {
          Gv mp_ = Gd.Ctor("PackedVector2Array", Gd.Arr(q.G("back"), q.G("front")));
          if ((_seen(mp_)).T()) {
            solo.C("append", Gd.Arr(mp_, Gv.F(12.0)));
          }
        }
      } else if (Gv.Eq(it_2.G("type"), Gv.S("fitting"))) {
        foreach (Gv q_2 in (m.G("sim").C("fit_ports", it_2)).Iter()) {
          Gv fp_ = Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", it_2.G("x"), it_2.G("y")), q_2));
          if ((_seen(fp_)).T()) {
            solo.C("append", Gd.Arr(fp_, Gv.F(12.0)));
          }
        }
      }
    }
    foreach (Gv p_2 in ((pipes + solo)).Iter()) {
      Gv sh = Gd.Ctor("PackedVector2Array");
      foreach (Gv q_3 in (p_2.I(Gv.I(0L))).Iter()) {
        sh.C("append", (q_3 + Gd.Ctor("Vector2", Gv.I(4L), Gv.I(6L))));
      }
      Self.C("draw_polyline", sh, Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.1)), (p_2.I(Gv.I(1L)) + Gv.F(6.0)), Gv.B(true));
    }
    _pends_build();
    foreach (Gv p_3 in (pipes).Iter()) {
      _pipe_outline(p_3.I(Gv.I(0L)), p_3.I(Gv.I(1L)), Gv.B(true));
    }
    foreach (Gv p_4 in (pipes).Iter()) {
      _pipe_fill(p_4.I(Gv.I(0L)), p_4.I(Gv.I(1L)), Gv.B(true));
    }
    foreach (Gv p_5 in (pipes).Iter()) {
      _pipe_clamps(p_5);
    }
    foreach (Gv p_6 in (pipes).Iter()) {
      _pipe_joints(p_6);
    }
    Gv joints = m.G("plan").C("get", Gv.S("joints"), Gd.Arr());
    Gv jset = Gd.Dict();
    foreach (Gv j in (joints).Iter()) {
      jset = jset.SI(Gd.Ctor("Vector2i", Gd.roundi(j.I(Gv.I(0L))), Gd.roundi(j.I(Gv.I(1L)))), Gv.B(true));
    }
    Gv crosses = Gd.Arr();
    Gv dots = Gd.Dict();
    foreach (Gv bi in Gd.RangeIter(Gv.I(1L), pipes.C("size"))) {
      foreach (Gv ai in (bi).Iter()) {
        Gv PA = pipes.I(ai);
        Gv PB = pipes.I(bi);
        foreach (Gv jb in ((PB.I(Gv.I(0L)).C("size") - Gv.I(1L))).Iter()) {
          Gv b0 = PB.I(Gv.I(0L)).I(jb);
          Gv b1 = PB.I(Gv.I(0L)).I((jb + Gv.I(1L)));
          Gv sv = (b1 - b0);
          Gv sl = sv.C("length");
          if (Gv.Lt(sl, Gv.F(1.0))) {
            continue;
          }
          foreach (Gv ia in ((PA.I(Gv.I(0L)).C("size") - Gv.I(1L))).Iter()) {
            Gv a0 = PA.I(Gv.I(0L)).I(ia);
            Gv rv = (PA.I(Gv.I(0L)).I((ia + Gv.I(1L))) - a0);
            Gv den = ((rv.G("x") * sv.G("y")) - (rv.G("y") * sv.G("x")));
            if (Gv.Lt(Gd.absf(den), Gv.F(1e-09))) {
              continue;
            }
            Gv t = ((((((b0.G("x") - a0.G("x"))) * sv.G("y")) - (((b0.G("y") - a0.G("y"))) * sv.G("x")))) / den);
            Gv uu = ((((((b0.G("x") - a0.G("x"))) * rv.G("y")) - (((b0.G("y") - a0.G("y"))) * rv.G("x")))) / den);
            if ((((Gv.Le(t, Gv.F(0.001)) || Gv.Ge(t, Gv.F(0.999))) || Gv.Le(uu, Gv.F(0.001))) || Gv.Ge(uu, Gv.F(0.999)))) {
              continue;
            }
            Gv c = (a0 + (rv * t));
            if ((_joint_near(joints, c)).T()) {
              dots = dots.SI(Gd.Ctor("Vector2i", Gd.roundi(c.G("x")), Gd.roundi(c.G("y"))), c);
              continue;
            }
            crosses.C("append", Gd.Arr(c, (sv / sl), ((((PA.I(Gv.I(1L)) + Gv.F(6.0))) / Gv.F(2.0)) + Gv.F(4.0)), PB.I(Gv.I(1L))));
          }
        }
      }
    }
    foreach (Gv P in (pipes).Iter()) {
      Gv pp = P.I(Gv.I(0L));
      if (Gv.Lt(pp.C("size"), Gv.I(4L))) {
        continue;
      }
      foreach (Gv i in ((pp.C("size") - Gv.I(1L))).Iter()) {
        foreach (Gv j_2 in Gd.RangeIter((i + Gv.I(2L)), (pp.C("size") - Gv.I(1L)))) {
          Gv a0_2 = pp.I(i);
          Gv rv_2 = (pp.I((i + Gv.I(1L))) - a0_2);
          Gv b0_2 = pp.I(j_2);
          Gv sv_2 = (pp.I((j_2 + Gv.I(1L))) - b0_2);
          Gv den_2 = ((rv_2.G("x") * sv_2.G("y")) - (rv_2.G("y") * sv_2.G("x")));
          if (Gv.Lt(Gd.absf(den_2), Gv.F(1e-09))) {
            continue;
          }
          Gv t_2 = ((((((b0_2.G("x") - a0_2.G("x"))) * sv_2.G("y")) - (((b0_2.G("y") - a0_2.G("y"))) * sv_2.G("x")))) / den_2);
          Gv uu_2 = ((((((b0_2.G("x") - a0_2.G("x"))) * rv_2.G("y")) - (((b0_2.G("y") - a0_2.G("y"))) * rv_2.G("x")))) / den_2);
          if ((((Gv.Le(t_2, Gv.F(0.001)) || Gv.Ge(t_2, Gv.F(0.999))) || Gv.Le(uu_2, Gv.F(0.001))) || Gv.Ge(uu_2, Gv.F(0.999)))) {
            continue;
          }
          Gv c_2 = (a0_2 + (rv_2 * t_2));
          if ((_joint_near(joints, c_2)).T()) {
            continue;
          }
          Gv dv = rv_2;
          if (Gv.Gt(Gd.absf(sv_2.G("x")), Gd.absf(rv_2.G("x")))) {
            dv = sv_2;
          }
          crosses.C("append", Gd.Arr(c_2, dv.C("normalized"), ((P.I(Gv.I(1L)) / Gv.F(2.0)) + Gv.F(7.0)), P.I(Gv.I(1L))));
        }
      }
    }
    foreach (Gv x in (crosses).Iter()) {
      Gv pa = (x.I(Gv.I(0L)) - (x.I(Gv.I(1L)) * x.I(Gv.I(2L))));
      Gv pb = (x.I(Gv.I(0L)) + (x.I(Gv.I(1L)) * x.I(Gv.I(2L))));
      _pipe_layers(Gd.Ctor("PackedVector2Array", Gd.Arr(pa, pb)), x.I(Gv.I(3L)), Gv.B(false));
    }
    foreach (Gv p_7 in (solo).Iter()) {
      _pipe_layers(p_7.I(Gv.I(0L)), p_7.I(Gv.I(1L)), Gv.B(true));
    }
    foreach (Gv itc in (m.G("items")).Iter()) {
      if ((((Gv.Eq(itc.G("type"), Gv.S("pipe")) && Gv.Ne(Gd.Dyn("str", itc.C("get", Gv.S("color"), Gv.S(""))), Gv.S(""))) && Gv.Ge(itc.G("pts").C("size"), Gv.I(2L))) && !(itc.C("get", Gv.S("hose"), Gv.B(false))).T())) {
        Gv plc = Gd.Ctor("PackedVector2Array");
        foreach (Gv q_4 in (itc.G("pts")).Iter()) {
          plc.C("append", Gd.Ctor("Vector2", q_4.I(Gv.I(0L)), q_4.I(Gv.I(1L))));
        }
        if ((_seen(plc)).T()) {
          Self.C("draw_polyline", plc, Gd.Ctor("Color", Gd.Dyn("str", itc.G("color")), Gv.F(0.85)), Gd.maxf(Gv.F(3.0), ((Gv.F(4.0) * Gd.Dyn("float", itc.C("get", Gv.S("dn"), Gv.I(50L)))) / Gv.F(50.0))), Gv.B(true));
        }
      }
    }
    foreach (Gv c_3 in (dots.C("values")).Iter()) {
      Self.C("draw_circle", c_3, Gv.F(10.5), Gd.Ctor("Color", Gv.S("1e2429")));
      Self.C("draw_circle", c_3, Gv.F(9.0), Gd.Ctor("Color", Gv.S("a9b3bb")));
      Self.C("draw_circle", c_3, Gv.F(6.5), Gd.Ctor("Color", Gv.S("d7dde2")));
      Self.C("draw_circle", c_3, Gv.F(4.0), Gd.Ctor("Color", Gv.S("f2c230")));
    }
    foreach (Gv it_3 in (m.G("items")).Iter()) {
      if (Gv.Eq(it_3.G("type"), Gv.S("text"))) {
        Self.C("draw_string", m.G("font"), Gd.Ctor("Vector2", it_3.G("x"), it_3.G("y")), it_3.G("label"), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gd.Dyn("int", it_3.C("get", Gv.S("size"), Gv.I(12L))), ((m.G("hall_dark")).T() ? Gd.Ctor("Color", Gv.S("aeb7bf")) : Gd.Ctor("Color", Gv.S("2b3138"))));
      }
    }
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "m": return this.m;
    case "clip": return this.clip;
    case "_pends": return this._pends;
    case "_occ": return this._occ;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "m": this.m = v; return v;
    case "clip": this.clip = v; return v;
    case "_pends": this._pends = v; return v;
    case "_occ": this._occ = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "_seen": return _seen(Gd.A(a, 0));
    case "_pipe_layers": return _pipe_layers(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "_pends_build": return _pends_build();
    case "_fillet": return _fillet(Gd.A(a, 0), Gd.A(a, 1));
    case "_pipe_outline": return _pipe_outline(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "_pipe_fill": return _pipe_fill(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "_pipe_clamps": return _pipe_clamps(Gd.A(a, 0));
    case "_pipe_joints": return _pipe_joints(Gd.A(a, 0));
    case "_joint_near": return _joint_near(Gd.A(a, 0), Gd.A(a, 1));
    case "_vgrad": return _vgrad(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "_hall": return _hall(Gd.A(a, 0), Gd.A(a, 1));
    case "_glow": return _glow(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4));
    case "_reflect": return _reflect(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5));
    case "_lamp_items": return _lamp_items();
    case "_hall_dark": return _hall_dark(Gd.A(a, 0), Gd.A(a, 1));
    case "_hall_light": return _hall_light(Gd.A(a, 0), Gd.A(a, 1));
    case "_stands": return _stands(Gd.A(a, 0), Gd.A(a, 1));
    case "_hgrad": return _hgrad(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "_draw": return _draw();
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_main_StaticLayer(); return Gv.O(o); }
  public override string ScriptName { get { return "S_main_StaticLayer"; } }
}
public partial class S_main_Mini : GdNode {
  public Gv m;
  public Gv sc;
  public Gv org;
  public Gv view;
  static S_main_Mini() {
  }
  public S_main_Mini() {
    TypeName = "Control";
    m = Gv.Null;
    sc = Gv.F(1.0);
    org = Gd.StaticGet("Vector2", "ZERO");
    view = Gv.Null;
  }
  public Gv setup() {
    Gv B = m.G("bounds");
    sc = Gd.minf((Gv.F(330.0) / B.G("size").G("x")), (Gv.F(110.0) / B.G("size").G("y")));
    org = B.G("position");
    Self.S("custom_minimum_size", ((B.G("size") * sc)).C("ceil"));
    Self.S("size", Self.G("custom_minimum_size"));
    Self.S("mouse_filter", Gd.StaticGet("Control", "MOUSE_FILTER_STOP"));
    Self.S("anchor_top", Gv.F(1.0));
    Self.S("anchor_bottom", Gv.F(1.0));
    Self.S("offset_left", Gv.I(12L));
    Self.S("offset_right", (Gv.I(12L) + Self.G("size").G("x")));
    Self.S("offset_top", (Gv.I(-14L) - Self.G("size").G("y")));
    Self.S("offset_bottom", Gv.I(-14L));
    Self.S("clip_contents", Gv.B(true));
    view = S_main_MiniView.New();
    view = view.S("m", m);
    view = view.S("mini", Self);
    view = view.S("mouse_filter", Gd.StaticGet("Control", "MOUSE_FILTER_IGNORE"));
    Self.C("add_child", view);
    view.C("set_anchors_preset", Gd.StaticGet("Control", "PRESET_FULL_RECT"));
    Self.C("queue_redraw");
    return Gv.Null;
  }
  public Gv w2m(Gv p = default(Gv)) {
    return (((p - org)) * sc);
    return Gv.Null;
  }
  public Gv _pipe_layers(Gv pts = default(Gv), Gv w = default(Gv), Gv rounded = default(Gv)) {
    w = (w).AsF();
    Self.C("draw_polyline", pts, Gd.Ctor("Color", Gv.S("0d1013")), (w + Gv.F(4.0)), rounded);
    Self.C("draw_polyline", pts, S_art.PIPE_STEEL, (w + Gv.F(1.0)), rounded);
    if (Gv.Gt(w, Gv.F(5.5))) {
      Self.C("draw_polyline", pts, Gd.Ctor("Color", Gv.S("eef2f5")), Gd.maxf(Gv.F(1.2), (w * Gv.F(0.2))), rounded);
    }
    if (Gv.Gt(w, Gv.F(8.0))) {
      Self.C("draw_polyline", pts, Gd.Ctor("Color", Gv.S("14181c")), (w - Gv.F(5.0)), rounded);
    }
    return Gv.Null;
  }
  public Gv _pipe_outline(Gv pts = default(Gv), Gv w = default(Gv), Gv rounded = default(Gv)) {
    w = (w).AsF();
    Self.C("draw_polyline", pts, Gd.Ctor("Color", Gv.S("0d1013")), (w + Gv.F(4.0)), rounded);
    return Gv.Null;
  }
  public Gv _pipe_fill(Gv pts = default(Gv), Gv w = default(Gv), Gv rounded = default(Gv)) {
    w = (w).AsF();
    Self.C("draw_polyline", pts, S_art.PIPE_STEEL, (w + Gv.F(1.0)), rounded);
    if (Gv.Gt(w, Gv.F(5.5))) {
      Self.C("draw_polyline", pts, Gd.Ctor("Color", Gv.S("eef2f5")), Gd.maxf(Gv.F(1.2), (w * Gv.F(0.2))), rounded);
    }
    if (Gv.Gt(w, Gv.F(8.0))) {
      Self.C("draw_polyline", pts, Gd.Ctor("Color", Gv.S("14181c")), (w - Gv.F(5.0)), rounded);
    }
    return Gv.Null;
  }
  public Gv _joint_near(Gv joints = default(Gv), Gv c = default(Gv)) {
    foreach (Gv j in (joints).Iter()) {
      if ((Gv.Lt(Gd.absf((j.I(Gv.I(0L)) - c.G("x"))), Gv.F(2.5)) && Gv.Lt(Gd.absf((j.I(Gv.I(1L)) - c.G("y"))), Gv.F(2.5)))) {
        return Gv.B(true);
      }
    }
    return Gv.B(false);
    return Gv.Null;
  }
  public Gv _draw() {
    Gv B = m.G("bounds");
    S_art.rrect(Self, Gd.Ctor("Rect2", Gd.Ctor("Vector2", Gv.I(3L), Gv.I(4L)), (B.G("size") * sc)), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.3)), Gv.F(6.0));
    S_art.rrect_border(Self, Gd.Ctor("Rect2", Gd.StaticGet("Vector2", "ZERO"), (B.G("size") * sc)), Gd.Ctor("Color", Gv.F(0.07), Gv.F(0.085), Gv.F(0.1), Gv.F(0.8)), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.18)), Gv.I(1L), Gv.F(6.0));
    Gv fy = ((((m.G("floor_y") - B.G("position").G("y"))) * sc)).AsF();
    Self.C("draw_rect", Gd.Ctor("Rect2", Gv.I(0L), fy, (B.G("size").G("x") * sc), ((B.G("size").G("y") * sc) - fy)), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.05)));
    foreach (Gv it in (m.G("items")).Iter()) {
      {
        Gv _t1 = it.G("type");
        if (Gv.Eq(_t1, Gv.S("zone"))) {
          Self.C("draw_rect", Gd.Ctor("Rect2", w2m(Gd.Ctor("Vector2", it.G("x"), it.G("y"))), (Gd.Ctor("Vector2", it.G("w"), it.G("h")) * sc)), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.06)));
        }
        else if (Gv.Eq(_t1, Gv.S("pipe"))) {
          if ((it.C("get", Gv.S("hose"), Gv.B(false))).T()) {
            continue;
          }
          Gv pts = Gd.Ctor("PackedVector2Array");
          foreach (Gv p in (it.G("pts")).Iter()) {
            pts.C("append", w2m(Gd.Ctor("Vector2", p.I(Gv.I(0L)), p.I(Gv.I(1L)))));
          }
          if (Gv.Ge(pts.C("size"), Gv.I(2L))) {
            Self.C("draw_polyline", pts, Gd.Ctor("Color", Gv.S("7b858e")), Gv.F(1.0));
          }
        }
        else if (Gv.Eq(_t1, Gv.S("vessel"))) {
          if (Gv.In(it.C("get", Gv.S("shape")), Gd.Arr(Gv.S("platform"), Gv.S("inlet")))) {
            continue;
          }
          Self.C("draw_rect", Gd.Ctor("Rect2", w2m(Gd.Ctor("Vector2", it.G("x"), it.G("y"))), (Gd.Ctor("Vector2", it.G("w"), it.G("h")) * sc)), Gd.Ctor("Color", Gv.S("c7cfd6")));
        }
        else if (Gv.Eq(_t1, Gv.S("panel"))) {
          Self.C("draw_rect", Gd.Ctor("Rect2", w2m(Gd.Ctor("Vector2", it.G("x"), it.G("y"))), (Gd.Ctor("Vector2", it.C("get", Gv.S("w"), Gv.I(200L)), it.C("get", Gv.S("h"), Gv.I(200L))) * sc)), Gd.Ctor("Color", Gv.S("59626a")));
        }
      }
    }
    return Gv.Null;
  }
  public Gv _gui_input(Gv e = default(Gv)) {
    if ((((Gv.Is(e, "InputEventMouseButton") && (e.G("pressed")).T()) && Gv.Eq(e.G("button_index"), Gd.K("MOUSE_BUTTON_LEFT"))) || (Gv.Is(e, "InputEventMouseMotion") && Gv.Ne((Gv.BitAnd(e.G("button_mask"), Gd.K("MOUSE_BUTTON_MASK_LEFT"))), Gv.I(0L))))) {
      m = m.S("cam", m.G("cam").S("position", (((e.G("position") / sc) + org) - (Gd.Ctor("Vector2", (m.C("right_w") * Gv.F(0.5)), Gv.F(-30.0)) / m.G("cam").G("zoom").G("x")))));
      m = m.S("vel", Gd.StaticGet("Vector2", "ZERO"));
      m = m.S("zoom_anim", Gv.B(false));
      Self.C("accept_event");
    }
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "m": return this.m;
    case "sc": return this.sc;
    case "org": return this.org;
    case "view": return this.view;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "m": this.m = v; return v;
    case "sc": this.sc = v; return v;
    case "org": this.org = v; return v;
    case "view": this.view = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "setup": return setup();
    case "w2m": return w2m(Gd.A(a, 0));
    case "_pipe_layers": return _pipe_layers(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "_pipe_outline": return _pipe_outline(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "_pipe_fill": return _pipe_fill(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "_joint_near": return _joint_near(Gd.A(a, 0), Gd.A(a, 1));
    case "_draw": return _draw();
    case "_gui_input": return _gui_input(Gd.A(a, 0));
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_main_Mini(); return Gv.O(o); }
  public override string ScriptName { get { return "S_main_Mini"; } }
}
public partial class S_main_MiniView : GdNode {
  public Gv m;
  public Gv mini;
  public Gv last;
  static S_main_MiniView() {
  }
  public S_main_MiniView() {
    TypeName = "Control";
    m = Gv.Null;
    mini = Gv.Null;
    last = Gd.StaticGet("Vector3", "ZERO");
  }
  public Gv _process(Gv _d = default(Gv)) {
    _d = (_d).AsF();
    Gv c = Gd.Ctor("Vector3", m.G("cam").G("position").G("x"), m.G("cam").G("position").G("y"), m.G("cam").G("zoom").G("x"));
    if (Gv.Ne(c, last)) {
      last = c;
      Self.C("queue_redraw");
    }
    return Gv.Null;
  }
  public Gv _draw() {
    Gv s = m.C("vp_size");
    Gv z = (m.G("cam").G("zoom").G("x")).AsF();
    Gv avail = (Gd.Ctor("Vector2", (s.G("x") - m.C("right_w")), (s.G("y") - Gv.F(60.0))) / z);
    Gv ctr = (m.G("cam").G("position") - (Gd.Ctor("Vector2", (m.C("right_w") * Gv.F(0.5)), Gv.F(-30.0)) / z));
    if ((Gv.Lt(Self.G("size").G("x"), Gv.F(4.0)) || Gv.Lt(Self.G("size").G("y"), Gv.F(4.0)))) {
      return Gv.Null;
    }
    Gv r = Gd.Ctor("Rect2", mini.C("w2m", (ctr - (avail * Gv.F(0.5)))), (avail * mini.G("sc"))).C("abs").C("intersection", Gd.Ctor("Rect2", Gd.Ctor("Vector2", Gv.I(1L), Gv.I(1L)), (Self.G("size") - Gd.Ctor("Vector2", Gv.I(2L), Gv.I(2L)))));
    if ((Gv.Le(r.G("size").G("x"), Gv.F(0.0)) || Gv.Le(r.G("size").G("y"), Gv.F(0.0)))) {
      return Gv.Null;
    }
    Self.C("draw_rect", r, Gd.Ctor("Color", Gv.F(0.95), Gv.F(0.76), Gv.F(0.19), Gv.F(0.14)));
    Self.C("draw_rect", r, Gd.Ctor("Color", Gv.S("f2c230")), Gv.B(false), Gv.F(1.5));
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "m": return this.m;
    case "mini": return this.mini;
    case "last": return this.last;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "m": this.m = v; return v;
    case "mini": this.mini = v; return v;
    case "last": this.last = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "_process": return _process(Gd.A(a, 0));
    case "_draw": return _draw();
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_main_MiniView(); return Gv.O(o); }
  public override string ScriptName { get { return "S_main_MiniView"; } }
}
public partial class S_main_FlowLayer : GdNode {
  public Gv m;
  public Gv mat;
  public Gv tex;
  public Gv fill_mi;
  public Gv dash_mi;
  public Gv edges;
  public Gv last_cols;
  public Gv last_dash;
  public Gv prev_full;
  public Gv fmode;
  public Gv rng;
  static S_main_FlowLayer() {
  }
  public S_main_FlowLayer() {
    TypeName = "Node2D";
    m = Gv.Null;
    mat = Gv.Null;
    tex = Gv.Null;
    fill_mi = Gv.Null;
    dash_mi = Gv.Null;
    edges = Gd.Arr();
    last_cols = Gd.Arr();
    last_dash = Gd.Arr();
    prev_full = Gd.Dict();
    fmode = Gd.Dict();
    rng = Gd.Dict();
  }
  public Gv build() {
    if (Gv.Eq(mat, Gv.Null)) {
      Gv sh = Gd.New("Shader");
      sh = sh.S("code", S_main.PIPE_LIQ);
      mat = Gd.New("ShaderMaterial");
      mat = mat.S("shader", sh);
      fill_mi = Gd.New("MeshInstance2D");
      Self.C("add_child", fill_mi);
      dash_mi = Gd.New("MeshInstance2D");
      dash_mi = dash_mi.S("material", mat);
      Self.C("add_child", dash_mi);
    }
    Gv hoses = Gd.Dict();
    foreach (Gv it in (m.G("items")).Iter()) {
      if ((Gv.Eq(it.G("type"), Gv.S("pipe")) && (it.C("get", Gv.S("hose"), Gv.B(false))).T())) {
        hoses = hoses.SI(it.G("id"), Gv.B(true));
      }
    }
    edges.C("clear");
    foreach (Gv e in (m.G("sim").G("net").G("edges")).Iter()) {
      if ((hoses.C("has", e.G("pipe"))).T()) {
        continue;
      }
      Gv dn = (e.G("dn")).AsF();
      Gv w = (Gv.Le(dn, Gv.I(32L)) ? Gv.F(6.0) : Gd.maxf(Gv.F(9.0), Gd.roundf(((Gv.F(12.0) * dn) / Gv.F(50.0)))));
      Gv wp = (Gv.Le(dn, Gv.I(32L)) ? Gd.clampf((dn * Gv.F(0.24)), Gv.F(3.0), Gv.F(6.0)) : Gd.maxf(Gv.F(9.0), Gd.roundf(((Gv.F(12.0) * dn) / Gv.F(50.0)))));
      edges.C("append", Gd.Arr(e, Gd.maxf(Gv.F(3.0), (w - Gv.F(5.0))), wp));
    }
    last_cols = Gd.Arr();
    last_dash = Gd.Arr();
    refresh();
    return Gv.Null;
  }
  public Gv refresh() {
    Gv sim = m.G("sim");
    Gv cols = Gd.Arr();
    Gv dsh = Gd.Arr();
    rng.C("clear");
    foreach (Gv E in (edges).Iter()) {
      Gv e = E.I(Gv.I(0L));
      Gv full = ((sim.C("vol", e.G("c")) / e.G("cap"))).AsF();
      if (Gv.Lt(full, Gv.F(0.02))) {
        cols.C("append", Gv.Null);
        prev_full = prev_full.SI(e.G("id"), full);
        fmode = fmode.SI(e.G("id"), Gv.I(0L));
        continue;
      }
      Gv pf = (prev_full.C("get", e.G("id"), full)).AsF();
      if (Gv.Gt(Gd.absf((full - pf)), Gv.F(0.002))) {
        fmode = fmode.SI(e.G("id"), (Gv.Gt(full, pf) ? Gv.I(1L) : Gv.I(-1L)));
        prev_full = prev_full.SI(e.G("id"), full);
      }
      Gv md = (fmode.C("get", e.G("id"), Gv.I(0L))).AsI();
      Gv part = Gv.B(((Gv.Ne(e.G("flow"), Gv.I(0L)) && Gv.Ne(md, Gv.I(0L))) && Gv.Lt(full, Gv.F(0.97))));
      Gv col = m.C("kind_color", sim.C("contentKind", e.G("c")), e.G("c").G("T"), (e.G("c").G("col") / Gd.maxf(Gv.F(0.01), sim.C("liqVol", e.G("c")))));
      Gv s0 = Gv.F(0.0);
      Gv s1 = Gv.F(1.0);
      if ((part).T()) {
        Gv f = Gd.snappedf(Gd.clampf(full, Gv.F(0.0), Gv.F(1.0)), Gv.F(0.01));
        Gv at_a = Gv.B(Gv.Eq((Gv.B(Gv.Eq(e.G("from"), Gv.S("a")))), (Gv.B(Gv.Gt(md, Gv.I(0L))))));
        if ((at_a).T()) {
          s1 = f;
        } else {
          s0 = (Gv.F(1.0) - f);
        }
        col = col.S("a", Gv.F(1.0));
      } else {
        if (Gv.Lt(full, Gv.F(0.15))) {
          cols.C("append", Gv.Null);
          continue;
        }
        col = col.S("a", Gv.F(1.0));
      }
      rng = rng.SI(e.G("id"), Gd.Arr(s0, s1));
      cols.C("append", Gd.Arr(Gd.Ctor("Color", Gd.snappedf(col.G("r"), Gv.F(0.03)), Gd.snappedf(col.G("g"), Gv.F(0.03)), Gd.snappedf(col.G("b"), Gv.F(0.03)), Gv.F(1.0)), s0, s1, ((Gv.Ne(e.G("flow"), Gv.I(0L)) ? _vpx(e) : Gv.I(0L)))));
    }
    if (Gv.Ne(cols, last_cols)) {
      last_cols = cols;
      _mesh_fill();
    }
    return Gv.Null;
  }
  public Gv _sub(Gv pts = default(Gv), Gv s0 = default(Gv), Gv s1 = default(Gv)) {
    s0 = (s0).AsF();
    s1 = (s1).AsF();
    if ((Gv.Le(s0, Gv.F(0.0)) && Gv.Ge(s1, Gv.F(1.0)))) {
      return pts;
    }
    Gv tot = Gv.F(0.0);
    foreach (Gv k in ((pts.C("size") - Gv.I(1L))).Iter()) {
      tot = (tot + pts.I(k).C("distance_to", pts.I((k + Gv.I(1L)))));
    }
    Gv a = (s0 * tot);
    Gv b = (s1 * tot);
    Gv o = Gd.Arr();
    Gv acc = Gv.F(0.0);
    foreach (Gv k_2 in ((pts.C("size") - Gv.I(1L))).Iter()) {
      Gv L = (pts.I(k_2).C("distance_to", pts.I((k_2 + Gv.I(1L))))).AsF();
      Gv lo = Gd.maxf(a, acc);
      Gv hi = Gd.minf(b, (acc + L));
      if ((Gv.Gt(hi, lo) && Gv.Gt(L, Gv.F(0.0)))) {
        Gv p0 = pts.I(k_2).C("lerp", pts.I((k_2 + Gv.I(1L))), (((lo - acc)) / L));
        Gv p1 = pts.I(k_2).C("lerp", pts.I((k_2 + Gv.I(1L))), (((hi - acc)) / L));
        if (((o.C("is_empty")).T() || Gv.Ne(o.I((o.C("size") - Gv.I(1L))), p0))) {
          o.C("append", p0);
        }
        o.C("append", p1);
      }
      acc = (acc + L);
    }
    return o;
    return Gv.Null;
  }
  public Gv _spd(Gv e = default(Gv), Gv w = default(Gv)) {
    w = (w).AsF();
    Gv area = ((Gv.F(0.00196) * Gd.pow((e.G("dn") / Gv.F(50.0)), Gv.I(2L)))).AsF();
    Gv v_px = ((((e.G("flow") / Gv.F(60000.0)) / area) * Gv.F(100.0))).AsF();
    return (Gd.clampi(Gd.roundi(((v_px / ((w * Gv.F(4.0)))) * Gv.F(2.0))), Gv.I(1L), Gv.I(24L))).AsI();
    return Gv.Null;
  }
  public Gv _fillet(Gv pts = default(Gv), Gv w = default(Gv)) {
    w = (w).AsF();
    if (Gv.Lt(pts.C("size"), Gv.I(3L))) {
      return pts;
    }
    Gv out_ = Gd.Arr(pts.I(Gv.I(0L)));
    Gv R = (Gd.maxf(Gv.F(8.0), (w * Gv.F(1.5)))).AsF();
    foreach (Gv i in Gd.RangeIter(Gv.I(1L), (pts.C("size") - Gv.I(1L)))) {
      Gv P = pts.I(i);
      Gv A = pts.I((i - Gv.I(1L)));
      Gv B = pts.I((i + Gv.I(1L)));
      Gv d1 = ((A - P));
      Gv d2 = ((B - P));
      Gv l1 = d1.C("length");
      Gv l2 = d2.C("length");
      if (((Gv.Lt(l1, Gv.F(1.0)) || Gv.Lt(l2, Gv.F(1.0))) || Gv.Lt(Gd.absf(d1.C("normalized").C("cross", d2.C("normalized"))), Gv.F(0.2)))) {
        out_.C("append", P);
        continue;
      }
      Gv r = (Gd.minf(R, (Gd.minf(l1, l2) * Gv.F(0.5)))).AsF();
      Gv T1 = (P + ((d1 / l1) * r));
      Gv T2 = (P + ((d2 / l2) * r));
      foreach (Gv k in (Gv.I(9L)).Iter()) {
        Gv t = (Gd.Dyn("float", k) / Gv.F(8.0));
        out_.C("append", T1.C("lerp", P, t).C("lerp", P.C("lerp", T2, t), t));
      }
    }
    out_.C("append", pts.I((pts.C("size") - Gv.I(1L))));
    return out_;
    return Gv.Null;
  }
  public Gv _cap(Gv v = default(Gv), Gv uv = default(Gv), Gv c = default(Gv), Gv p = default(Gv), Gv dir = default(Gv), Gv w = default(Gv), Gv col = default(Gv)) {
    w = (w).AsF();
    Gv r = (w * Gv.F(0.5));
    Gv base_ = dir.C("angle");
    foreach (Gv k in (Gv.I(6L)).Iter()) {
      Gv a0 = ((base_ - (Gd.K("PI") * Gv.F(0.5))) + ((Gd.K("PI") * k) / Gv.F(6.0)));
      Gv a1 = ((base_ - (Gd.K("PI") * Gv.F(0.5))) + ((Gd.K("PI") * ((k + Gv.I(1L)))) / Gv.F(6.0)));
      v.C("append_array", Gd.Arr(p, (p + (Gd.Ctor("Vector2", Gd.cos(a0), Gd.sin(a0)) * r)), (p + (Gd.Ctor("Vector2", Gd.cos(a1), Gd.sin(a1)) * r))));
      uv.C("append_array", Gd.Arr(Gd.Ctor("Vector2", Gv.F(0.5), Gv.F(0.5)), Gd.Ctor("Vector2", Gv.F(0.5), Gv.F(0.5)), Gd.Ctor("Vector2", Gv.F(0.5), Gv.F(0.5))));
      foreach (Gv i in (Gv.I(3L)).Iter()) {
        c.C("append", col);
      }
    }
    return Gv.Null;
  }
  public Gv _shift(Gv pp = default(Gv), Gv off = default(Gv)) {
    Gv o = Gd.Arr();
    foreach (Gv q in (pp).Iter()) {
      o.C("append", (q + off));
    }
    return o;
    return Gv.Null;
  }
  public Gv _water_line(Gv v = default(Gv), Gv uv = default(Gv), Gv c = default(Gv), Gv pp = default(Gv), Gv w = default(Gv), Gv col = default(Gv)) {
    w = (w).AsF();
    if (Gv.Lt(pp.C("size"), Gv.I(2L))) {
      return Gv.Null;
    }
    foreach (Gv k in ((pp.C("size") - Gv.I(1L))).Iter()) {
      _seg(v, uv, c, pp.I(k), pp.I((k + Gv.I(1L))), w, col, Gv.F(0.0), Gv.F(1.0), Gv.F(0.0), Gv.B(false));
    }
    Gv d0 = ((pp.I(Gv.I(0L)) - pp.I(Gv.I(1L))));
    Gv d1 = ((pp.I((pp.C("size") - Gv.I(1L))) - pp.I((pp.C("size") - Gv.I(2L)))));
    if (Gv.Gt(d0.C("length"), Gv.F(0.01))) {
      _cap(v, uv, c, pp.I(Gv.I(0L)), d0.C("normalized"), w, col);
    }
    if (Gv.Gt(d1.C("length"), Gv.F(0.01))) {
      _cap(v, uv, c, pp.I((pp.C("size") - Gv.I(1L))), d1.C("normalized"), w, col);
    }
    return Gv.Null;
  }
  public Gv _seg(Gv v = default(Gv), Gv uv = default(Gv), Gv c = default(Gv), Gv a = default(Gv), Gv b = default(Gv), Gv w = default(Gv), Gv col = default(Gv), Gv u0 = default(Gv), Gv u1 = default(Gv), Gv yb = default(Gv), Gv ext = default(Gv)) {
    w = (w).AsF();
    u0 = (u0).AsF();
    u1 = (u1).AsF();
    if (yb.k == 0) yb = Gv.F(0.0);
    if (ext.k == 0) ext = Gv.B(true);
    Gv d = ((b - a)).C("normalized");
    Gv n = ((Gd.Ctor("Vector2", (-d.G("y")), d.G("x")) * w) * Gv.F(0.5));
    Gv ex = ((ext).T() ? ((d * w) * Gv.F(0.5)) : Gd.StaticGet("Vector2", "ZERO"));
    Gv a2 = (a - ex);
    Gv b2 = (b + ex);
    v.C("append_array", Gd.Arr((a2 + n), (b2 + n), (b2 - n), (a2 + n), (b2 - n), (a2 - n)));
    uv.C("append_array", Gd.Arr(Gd.Ctor("Vector2", u0, (yb + Gv.F(0.1))), Gd.Ctor("Vector2", u1, (yb + Gv.F(0.1))), Gd.Ctor("Vector2", u1, (yb + Gv.F(0.9))), Gd.Ctor("Vector2", u0, (yb + Gv.F(0.1))), Gd.Ctor("Vector2", u1, (yb + Gv.F(0.9))), Gd.Ctor("Vector2", u0, (yb + Gv.F(0.9)))));
    foreach (Gv i in (Gv.I(6L)).Iter()) {
      c.C("append", col);
    }
    return Gv.Null;
  }
  public Gv _set_mesh(Gv mi = default(Gv), Gv v = default(Gv), Gv uv = default(Gv), Gv c = default(Gv)) {
    if (Gv.Eq(v.C("size"), Gv.I(0L))) {
      mi = mi.S("mesh", Gv.Null);
      return Gv.Null;
    }
    Gv arr = Gd.Arr();
    arr.C("resize", Gd.StaticGet("Mesh", "ARRAY_MAX"));
    arr = arr.SI(Gd.StaticGet("Mesh", "ARRAY_VERTEX"), v);
    arr = arr.SI(Gd.StaticGet("Mesh", "ARRAY_TEX_UV"), uv);
    arr = arr.SI(Gd.StaticGet("Mesh", "ARRAY_COLOR"), c);
    Gv am = Gd.New("ArrayMesh");
    am.C("add_surface_from_arrays", Gd.StaticGet("Mesh", "PRIMITIVE_TRIANGLES"), arr);
    mi = mi.S("mesh", am);
    return Gv.Null;
  }
  public Gv _vpx(Gv e = default(Gv)) {
    Gv area = ((Gv.F(0.00196) * Gd.pow((e.G("dn") / Gv.F(50.0)), Gv.I(2L)))).AsF();
    Gv v_px = ((((e.G("flow") / Gv.F(60000.0)) / area) * Gv.F(100.0))).AsF();
    return (Gd.clampi((Gd.Dyn("int", Gd.roundf((v_px / Gv.F(10.0)))) * Gv.I(10L)), Gv.I(10L), Gv.I(300L))).AsI();
    return Gv.Null;
  }
  public Gv _trim(Gv pts = default(Gv), Gv t0 = default(Gv), Gv t1 = default(Gv)) {
    t0 = (t0).AsF();
    t1 = (t1).AsF();
    Gv tot = Gv.F(0.0);
    foreach (Gv k in ((pts.C("size") - Gv.I(1L))).Iter()) {
      tot = (tot + pts.I(k).C("distance_to", pts.I((k + Gv.I(1L)))));
    }
    if (Gv.Lt(tot, ((t0 + t1) + Gv.F(4.0)))) {
      return pts;
    }
    return _sub(pts, (t0 / tot), (Gv.F(1.0) - (t1 / tot)));
    return Gv.Null;
  }
  public Gv _strip(Gv v = default(Gv), Gv uv = default(Gv), Gv c = default(Gv), Gv pp = default(Gv), Gv w = default(Gv), Gv col = default(Gv), Gv flat = default(Gv)) {
    w = (w).AsF();
    Gv n = (pp.C("size")).AsI();
    if (Gv.Lt(n, Gv.I(2L))) {
      return Gv.Null;
    }
    Gv L = Gd.Arr();
    Gv U = Gd.Arr();
    Gv Rr = Gd.Arr();
    Gv acc = Gv.F(0.0);
    foreach (Gv i in (n).Iter()) {
      Gv d0 = Gd.StaticGet("Vector2", "ZERO");
      Gv d1 = Gd.StaticGet("Vector2", "ZERO");
      if (Gv.Gt(i, Gv.I(0L))) {
        d0 = ((pp.I(i) - pp.I((i - Gv.I(1L))))).C("normalized");
      }
      if (Gv.Lt(i, (n - Gv.I(1L)))) {
        d1 = ((pp.I((i + Gv.I(1L))) - pp.I(i))).C("normalized");
      }
      Gv dd = ((d0 + d1));
      if (Gv.Lt(dd.C("length"), Gv.F(0.001))) {
        dd = (Gv.Ne(d1, Gd.StaticGet("Vector2", "ZERO")) ? d1 : d0);
      }
      dd = dd.C("normalized");
      Gv nn = Gd.Ctor("Vector2", (-dd.G("y")), dd.G("x"));
      Gv k = (Gv.F(1.0)).AsF();
      if ((Gv.Ne(d0, Gd.StaticGet("Vector2", "ZERO")) && Gv.Ne(d1, Gd.StaticGet("Vector2", "ZERO")))) {
        k = (Gd.clampf((Gv.F(1.0) / Gd.maxf(Gv.F(0.35), dd.C("dot", d1))), Gv.F(1.0), Gv.F(1.6))).AsF();
      }
      L.C("append", (pp.I(i) + (((nn * w) * Gv.F(0.5)) * k)));
      Rr.C("append", (pp.I(i) - (((nn * w) * Gv.F(0.5)) * k)));
      if (Gv.Gt(i, Gv.I(0L))) {
        acc = (acc + pp.I(i).C("distance_to", pp.I((i - Gv.I(1L)))));
      }
      U.C("append", acc);
    }
    foreach (Gv i_2 in ((n - Gv.I(1L))).Iter()) {
      v.C("append_array", Gd.Arr(L.I(i_2), L.I((i_2 + Gv.I(1L))), Rr.I((i_2 + Gv.I(1L))), L.I(i_2), Rr.I((i_2 + Gv.I(1L))), Rr.I(i_2)));
      uv.C("append_array", Gd.Arr(Gd.Ctor("Vector2", U.I(i_2), Gv.F(0.0)), Gd.Ctor("Vector2", U.I((i_2 + Gv.I(1L))), Gv.F(0.0)), Gd.Ctor("Vector2", U.I((i_2 + Gv.I(1L))), Gv.F(1.0)), Gd.Ctor("Vector2", U.I(i_2), Gv.F(0.0)), Gd.Ctor("Vector2", U.I((i_2 + Gv.I(1L))), Gv.F(1.0)), Gd.Ctor("Vector2", U.I(i_2), Gv.F(1.0))));
      foreach (Gv q in (Gv.I(6L)).Iter()) {
        c.C("append", col);
      }
    }
    return Gv.Null;
  }
  public Gv _scap(Gv v = default(Gv), Gv uv = default(Gv), Gv c = default(Gv), Gv p = default(Gv), Gv dir = default(Gv), Gv w = default(Gv), Gv col = default(Gv), Gv u = default(Gv)) {
    w = (w).AsF();
    u = (u).AsF();
    Gv r = (w * Gv.F(0.5));
    Gv base_ = dir.C("angle");
    Gv nr = Gd.Ctor("Vector2", (-dir.G("y")), dir.G("x"));
    foreach (Gv k in (Gv.I(8L)).Iter()) {
      Gv a0 = ((base_ - (Gd.K("PI") * Gv.F(0.5))) + ((Gd.K("PI") * k) / Gv.F(8.0)));
      Gv a1 = ((base_ - (Gd.K("PI") * Gv.F(0.5))) + ((Gd.K("PI") * ((k + Gv.I(1L)))) / Gv.F(8.0)));
      Gv q0 = (Gd.Ctor("Vector2", Gd.cos(a0), Gd.sin(a0)) * r);
      Gv q1 = (Gd.Ctor("Vector2", Gd.cos(a1), Gd.sin(a1)) * r);
      v.C("append_array", Gd.Arr(p, (p + q0), (p + q1)));
      uv.C("append_array", Gd.Arr(Gd.Ctor("Vector2", u, Gv.F(0.5)), Gd.Ctor("Vector2", u, (Gv.F(0.5) + ((Gv.F(0.5) * q0.C("dot", nr)) / r))), Gd.Ctor("Vector2", u, (Gv.F(0.5) + ((Gv.F(0.5) * q1.C("dot", nr)) / r)))));
      foreach (Gv i in (Gv.I(3L)).Iter()) {
        c.C("append", col);
      }
    }
    return Gv.Null;
  }
  public Gv _mesh_fill() {
    Gv vo = Gd.Ctor("PackedVector2Array");
    Gv uo = Gd.Ctor("PackedVector2Array");
    Gv co = Gd.Ctor("PackedColorArray");
    Gv vb = Gd.Ctor("PackedVector2Array");
    Gv ub = Gd.Ctor("PackedVector2Array");
    Gv cb = Gd.Ctor("PackedColorArray");
    Gv nodes = m.G("sim").G("net").G("nodes");
    foreach (Gv i in (edges.C("size")).Iter()) {
      Gv ci = last_cols.I(i);
      if (Gv.Eq(ci, Gv.Null)) {
        continue;
      }
      Gv e = edges.I(i).I(Gv.I(0L));
      Gv w = (edges.I(i).I(Gv.I(1L))).AsF();
      Gv pts = _sub(_fillet(e.G("pts"), edges.I(i).I(Gv.I(2L))), ci.I(Gv.I(1L)), ci.I(Gv.I(2L))).C("duplicate");
      if (Gv.Lt(pts.C("size"), Gv.I(2L))) {
        continue;
      }
      Gv ta = (Gv.F(0.0)).AsF();
      Gv tb = (Gv.F(0.0)).AsF();
      if (((Gv.Le(ci.I(Gv.I(1L)), Gv.F(0.0)) && (nodes.C("has", e.G("a"))).T()) && Gv.Ge(nodes.I(e.G("a")).G("edges").C("size"), Gv.I(3L)))) {
        ta = ((w * Gv.F(0.55))).AsF();
      }
      if (((Gv.Ge(ci.I(Gv.I(2L)), Gv.F(1.0)) && (nodes.C("has", e.G("b"))).T()) && Gv.Ge(nodes.I(e.G("b")).G("edges").C("size"), Gv.I(3L)))) {
        tb = ((w * Gv.F(0.55))).AsF();
      }
      pts = _trim(pts, ta, tb);
      Gv cap_a = Gv.B(Gv.Gt(ci.I(Gv.I(1L)), Gv.F(0.0)));
      Gv cap_b = Gv.B(Gv.Lt(ci.I(Gv.I(2L)), Gv.F(1.0)));
      if ((Gv.Ne(e.G("flow"), Gv.I(0L)) && Gv.Eq(e.G("from"), Gv.S("b")))) {
        pts.C("reverse");
        Gv tmp = cap_a;
        cap_a = cap_b;
        cap_b = tmp;
      }
      Gv col = ci.I(Gv.I(0L));
      Gv oc = col.C("darkened", Gv.F(0.3));
      _strip(vo, uo, co, pts, w, oc, Gv.B(true));
      Gv bc = Gd.Ctor("Color", col.G("r"), col.G("g"), col.G("b"), Gd.clampf((Gd.Dyn("float", ci.I(Gv.I(3L))) / Gv.F(300.0)), Gv.F(0.0), Gv.F(1.0)));
      _strip(vb, ub, cb, pts, (w * Gv.F(0.8)), bc, Gv.B(false));
      Gv tot = Gv.F(0.0);
      foreach (Gv k in ((pts.C("size") - Gv.I(1L))).Iter()) {
        tot = (tot + pts.I(k).C("distance_to", pts.I((k + Gv.I(1L)))));
      }
      if ((cap_a).T()) {
        Gv d0 = ((pts.I(Gv.I(0L)) - pts.I(Gv.I(1L)))).C("normalized");
        _scap(vo, uo, co, pts.I(Gv.I(0L)), d0, w, oc, Gv.F(0.0));
        _scap(vb, ub, cb, pts.I(Gv.I(0L)), d0, (w * Gv.F(0.8)), bc, Gv.F(0.0));
      }
      if ((cap_b).T()) {
        Gv d1 = ((pts.I((pts.C("size") - Gv.I(1L))) - pts.I((pts.C("size") - Gv.I(2L))))).C("normalized");
        _scap(vo, uo, co, pts.I((pts.C("size") - Gv.I(1L))), d1, w, oc, tot);
        _scap(vb, ub, cb, pts.I((pts.C("size") - Gv.I(1L))), d1, (w * Gv.F(0.8)), bc, tot);
      }
    }
    _set_mesh(fill_mi, vo, uo, co);
    _set_mesh(dash_mi, vb, ub, cb);
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "m": return this.m;
    case "mat": return this.mat;
    case "tex": return this.tex;
    case "fill_mi": return this.fill_mi;
    case "dash_mi": return this.dash_mi;
    case "edges": return this.edges;
    case "last_cols": return this.last_cols;
    case "last_dash": return this.last_dash;
    case "prev_full": return this.prev_full;
    case "fmode": return this.fmode;
    case "rng": return this.rng;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "m": this.m = v; return v;
    case "mat": this.mat = v; return v;
    case "tex": this.tex = v; return v;
    case "fill_mi": this.fill_mi = v; return v;
    case "dash_mi": this.dash_mi = v; return v;
    case "edges": this.edges = v; return v;
    case "last_cols": this.last_cols = v; return v;
    case "last_dash": this.last_dash = v; return v;
    case "prev_full": this.prev_full = v; return v;
    case "fmode": this.fmode = v; return v;
    case "rng": this.rng = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "build": return build();
    case "refresh": return refresh();
    case "_sub": return _sub(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "_spd": return _spd(Gd.A(a, 0), Gd.A(a, 1));
    case "_fillet": return _fillet(Gd.A(a, 0), Gd.A(a, 1));
    case "_cap": return _cap(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6));
    case "_shift": return _shift(Gd.A(a, 0), Gd.A(a, 1));
    case "_water_line": return _water_line(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5));
    case "_seg": return _seg(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6), Gd.A(a, 7), Gd.A(a, 8), Gd.A(a, 9), Gd.A(a, 10));
    case "_set_mesh": return _set_mesh(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "_vpx": return _vpx(Gd.A(a, 0));
    case "_trim": return _trim(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "_strip": return _strip(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6));
    case "_scap": return _scap(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6), Gd.A(a, 7));
    case "_mesh_fill": return _mesh_fill();
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_main_FlowLayer(); return Gv.O(o); }
  public override string ScriptName { get { return "S_main_FlowLayer"; } }
}
public partial class S_main_LiveNode : S_main_ItemLayer {
  public Gv anim_t;
  public Gv kind;
  public Gv it;
  public Gv sig;
  public Gv st;
  public Gv dash;
  static S_main_LiveNode() {
  }
  public S_main_LiveNode() {
    anim_t = Gv.F(0.0);
    kind = Gv.S("");
    it = Gv.Null;
    sig = Gv.S("");
    st = Gv.I(0L);
    dash = Gd.Arr();
  }
  public Gv upd() {
    Gv s = _sig();
    if (Gv.Eq(s, sig)) {
      return Gv.Null;
    }
    sig = s;
    Self.C("queue_redraw");
    if ((Gv.Eq(kind, Gv.S("streams")) || Gv.Eq(kind, Gv.S("leaks")))) {
      _rebuild_dash();
    }
    return Gv.Null;
  }
  public Gv _process(Gv delta = default(Gv)) {
    delta = (delta).AsF();
    if (Gv.Eq(kind, Gv.S("vessel"))) {
      Gv bb = it.C("get", Gv.S("bind"));
      Gv SS = m.G("sim").G("S");
      anim_t = (anim_t + delta);
      if ((Gv.Gt(anim_t, Gv.F(0.07)) && Gv.In(bb, Gd.Arr(Gv.S("rvp"), Gv.S("mp"), Gv.S("wp"), Gv.S("sk"))))) {
        Gv fxs = fx.C("get", it.G("id"), Gd.Dict());
        if ((((((Gv.Gt(Gd.Dyn("float", fxs.C("get", Gv.S("stir"), Gv.F(0.0))), Gv.F(0.02)) || Gv.Gt(Gd.Dyn("float", fxs.C("get", Gv.S("susp"), Gv.F(0.0))), Gv.F(0.03))) || Gv.Gt(Gd.Dyn("float", fxs.C("get", Gv.S("slosh"), Gv.F(0.0))), Gv.F(0.1))) || (Gv.Ne(bb, Gv.S("sk")) && ((SS.I(bb).G("boiling")).T() || (SS.I(bb).G("whirlNow")).T()))) || ((Gv.Eq(bb, Gv.S("sk")) && Gv.Gt(Gd.Dyn("float", SS.G("sk").G("turb")), Gv.F(3.0))) && Gv.Gt(m.G("sim").C("skVolTotal"), Gv.F(50.0)))) || ((Gv.Ne(bb, Gv.S("sk")) && Gv.Gt(m.G("sim").C("vol", SS.I(bb)), Gv.F(50.0))) && (Gv.Gt(SS.I(bb).G("trubSus"), Gv.F(0.05)) || Gv.Gt(((SS.I(bb).G("alpha") + SS.I(bb).G("iso"))), Gv.F(0.5)))))) {
          anim_t = Gv.F(0.0);
          Self.C("queue_redraw");
        }
      }
      if ((Gv.Eq(bb, Gv.S("rvp")) && (SS.G("rvp").G("stir")).T())) {
        spin = (spin + (((((delta * SS.G("rvp").G("hz")) * Gv.F(0.8)) * Gv.F(3.0)) / Gv.F(60.0)) * Gd.K("TAU")));
        Self.C("queue_redraw");
      } else if ((Gv.Eq(bb, Gv.S("sk")) && (SS.G("sk").G("rake")).T())) {
        spin = (spin + ((((delta * m.G("sim").C("rkRpm")) * Gv.F(6.0)) / Gv.F(60.0)) * Gd.K("TAU")));
        Self.C("queue_redraw");
      }
    }
    if ((Gv.Eq(kind, Gv.S("pump")) && Gv.Ne(st, Gv.I(0L)))) {
      Gv b = it.C("get", Gv.S("bind"), Gv.S(""));
      Gv q = ((Gv.In(b, Gd.Arr(Gv.S("p1"), Gv.S("p2"), Gv.S("p3"))) ? Gd.Dyn("float", m.G("sim").G("S").I(b).C("get", Gv.S("q"), Gv.F(0.0))) : Gv.F(100.0))).AsF();
      Gv qn = (Gd.Dict(Gv.S("p1"), Gv.F(400.0), Gv.S("p2"), Gv.F(150.0), Gv.S("p3"), Gv.F(200.0)).C("get", b, Gv.F(200.0))).AsF();
      spin = (spin + (delta * ((Gv.Eq(st, Gv.I(2L)) ? ((Gv.F(4.0) + (Gv.F(12.0) * Gd.clampf((q / qn), Gv.F(0.0), Gv.F(1.5))))) : Gv.F(2.0)))));
      Self.C("queue_redraw");
    }
    return Gv.Null;
  }
  public Gv _sig() {
    Gv S = m.G("sim").G("S");
    {
      Gv _t1 = kind;
      if (Gv.Eq(_t1, Gv.S("vessel"))) {
        Gv b = it.C("get", Gv.S("bind"));
        Gv boil = Gv.B(false);
        if (Gv.In(b, Gd.Arr(Gv.S("rvp"), Gv.S("mp"), Gv.S("wp")))) {
          boil = Gv.B(((S.I(b).G("boiling")).T() || ((S.I(b).G("stir")).T() && Gv.Gt(m.G("sim").C("vol", S.I(b)), Gv.I(50L)))));
        }
        Gv ph = ((boil).T() ? (Gd.Dyn("int", (Gd.Static("Time", "get_ticks_msec") / Gv.I(100L))) % Gv.I(40L)) : Gv.I(0L));
        Gv vo = (Gv.Eq(b, Gv.S("rvp")) ? Gd.roundi((S.G("rvp").G("vort") * Gv.F(1000.0))) : Gv.I(0L));
        return (Gv.S("%d|%s|%s|%d|%d|%d|%s") % Gd.Arr(Gd.roundi((m.C("level_y", it) / Gv.F(2.0))), m.C("vessel_color", it).C("to_html", Gv.B(false)), m.C("vessel_text", b), ph, m.C("label_size"), (vo / Gv.I(5L)), (m.C("hard_sig", it) + ((Gv.Eq(b, Gv.S("sk")) ? ((Gv.S("|%d%s") % Gd.Arr(Gd.roundi((S.G("sk").G("rakeH") * Gv.F(50.0))), Gd.Dyn("str", S.G("sk").G("rake"))))) : Gv.S(""))))));
      }
      else if (Gv.Eq(_t1, Gv.S("card"))) {
        Gv dc = m.C("vessel_card", it.C("get", Gv.S("bind")));
        return (Gv.S("%s|%d|%d") % Gd.Arr(Gd.Dyn("str", dc), m.C("label_size"), Gd.roundi(m.C("level_y", it))));
      }
      else if (Gv.Eq(_t1, Gv.S("pump"))) {
        st = m.C("pump_state", it);
        return (Gv.S("%d|%d") % Gd.Arr(st, ((m.C("pump_primed", it)).T() ? Gv.I(1L) : Gv.I(0L))));
      }
      else if (Gv.Eq(_t1, Gv.S("streams"))) {
        Gv o = Gd.Arr();
        foreach (Gv s in (_stream_segs()).Iter()) {
          o.C("append", (Gv.S("%d,%d,%d,%d,%s") % Gd.Arr(s.I(Gv.I(0L)).G("x"), s.I(Gv.I(0L)).G("y"), s.I(Gv.I(1L)).G("x"), s.I(Gv.I(1L)).G("y"), s.I(Gv.I(2L)).C("to_html"))));
        }
        return Gv.S(";").C("join", Gd.Ctor("PackedStringArray", o));
      }
      else if (Gv.Eq(_t1, Gv.S("leaks"))) {
        Gv ks = Gd.Dict();
        foreach (Gv p in (S.G("leaks")).Iter()) {
          ks = ks.SI((Gv.S("%d,%d") % Gd.Arr(p.G("x"), p.G("y"))), Gv.B(true));
        }
        Gv arr = ks.C("keys");
        arr.C("sort");
        return Gv.S(";").C("join", Gd.Ctor("PackedStringArray", arr));
      }
    }
    return Gv.S("");
    return Gv.Null;
  }
  public Gv _draw() {
    {
      Gv _t1 = kind;
      if (Gv.Eq(_t1, Gv.S("vessel"))) {
        _liquid(it);
        _gauge_levels(it);
        _internals(it);
      }
      else if (Gv.Eq(_t1, Gv.S("card"))) {
        _vstatus(it);
      }
      else if (Gv.Eq(_t1, Gv.S("pump"))) {
        if (Gv.Eq(st, Gv.I(0L))) {
          if ((m.C("pump_primed", it)).T()) {
            Self.C("draw_circle", Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-34L)), Gv.F(5.0), Gd.Ctor("Color", Gv.S("2f8fe0")));
          }
          return Gv.Null;
        }
        Gv col = (Gv.Eq(st, Gv.I(2L)) ? Gd.Ctor("Color", Gv.S("2fbf5b")) : ((Gv.Eq(st, Gv.I(3L)) ? Gd.Ctor("Color", Gv.S("2f8fe0")) : Gd.Ctor("Color", Gv.S("f39a2b")))));
        Gv tt = ((Gd.Static("Time", "get_ticks_msec") / Gv.F(1000.0))).AsF();
        Gv pulse = ((Gv.F(0.5) + (Gv.F(0.5) * Gd.sin((tt * Gv.F(5.0)))))).AsF();
        Self.C("draw_circle", Gd.StaticGet("Vector2", "ZERO"), (Gv.F(34.0) + (Gv.F(3.0) * pulse)), Gd.Ctor("Color", col, (Gv.F(0.07) + (Gv.F(0.06) * pulse))));
        Self.C("draw_circle", Gd.StaticGet("Vector2", "ZERO"), Gv.F(20.5), Gd.Ctor("Color", Gv.S("0b0e11")));
        Self.C("draw_circle", Gd.StaticGet("Vector2", "ZERO"), Gv.F(19.0), Gd.Ctor("Color", Gv.F(0.16), Gv.F(0.19), Gv.F(0.22)));
        Gv nb = Gv.I(6L);
        foreach (Gv k in (nb).Iter()) {
          Gv a = ((spin + ((Gd.K("TAU") * k) / nb))).AsF();
          Gv trail = (Gv.F(0.9)).AsF();
          foreach (Gv j in (Gv.I(7L)).Iter()) {
            Gv f = ((Gd.Dyn("float", j) / Gv.F(6.0))).AsF();
            Self.C("draw_arc", Gd.StaticGet("Vector2", "ZERO"), Gv.F(11.5), (a - (trail * ((Gv.F(1.0) - f)))), (((a - (trail * ((Gv.F(1.0) - f)))) + (trail / Gv.F(6.0))) + Gv.F(0.02)), Gv.I(6L), Gd.Ctor("Color", Gv.F(0.78), Gv.F(0.86), Gv.F(0.93), (Gv.F(0.05) + (Gv.F(0.28) * f))), Gv.F(14.0), Gv.B(true));
          }
          Gv q0 = Gd.Ctor("Vector2", Gd.cos(a), Gd.sin(a));
          Gv q1 = Gd.Ctor("Vector2", Gd.cos((a + Gv.F(0.35))), Gd.sin((a + Gv.F(0.35))));
          Self.C("draw_polyline", Gd.Ctor("PackedVector2Array", Gd.Arr((q0 * Gv.F(4.0)), (q1 * Gv.F(11.0)), (Gd.Ctor("Vector2", Gd.cos((a + Gv.F(0.7))), Gd.sin((a + Gv.F(0.7)))) * Gv.F(18.0)))), Gd.Ctor("Color", Gv.S("e4eaee")), Gv.F(2.6), Gv.B(true));
        }
        Self.C("draw_circle", Gd.StaticGet("Vector2", "ZERO"), Gv.F(5.0), Gd.Ctor("Color", Gv.S("9aa6af")));
        Self.C("draw_circle", Gd.StaticGet("Vector2", "ZERO"), Gv.F(3.2), Gd.Ctor("Color", Gv.S("dfe6ea")));
        Self.C("draw_circle", Gd.Ctor("Vector2", Gv.I(-1L), Gv.I(-1L)), Gv.F(1.2), Gd.StaticGet("Color", "WHITE"));
        Self.C("draw_arc", Gd.StaticGet("Vector2", "ZERO"), Gv.F(20.0), Gv.I(0L), Gd.K("TAU"), Gv.I(40L), Gd.Ctor("Color", Gv.S("6c7780")), Gv.F(2.0), Gv.B(true));
        Self.C("draw_arc", Gd.StaticGet("Vector2", "ZERO"), Gv.F(15.0), (Gd.K("PI") * Gv.F(1.15)), (Gd.K("PI") * Gv.F(1.55)), Gv.I(12L), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.45)), Gv.F(2.5), Gv.B(true));
        Self.C("draw_circle", Gd.Ctor("Vector2", Gv.I(30L), Gv.I(-38L)), Gv.F(3.6), Gd.Ctor("Color", Gv.S("0d1013")));
        Self.C("draw_circle", Gd.Ctor("Vector2", Gv.I(30L), Gv.I(-38L)), Gv.F(2.6), col.C("lightened", (Gv.F(0.15) + (Gv.F(0.2) * pulse))));
        Gv b2 = it.C("get", Gv.S("bind"), Gv.S(""));
        Gv txt = Gv.S("BĚŽÍ");
        if (Gv.Eq(st, Gv.I(3L))) {
          txt = Gv.S("ZAVODŇUJE");
        } else if (Gv.Eq(st, Gv.I(1L))) {
          txt = Gv.S("BĚŽÍ – BEZ PRŮTOKU");
        }
        if ((Gv.Eq(st, Gv.I(2L)) && Gv.In(b2, Gd.Arr(Gv.S("p1"), Gv.S("p2"), Gv.S("p3"))))) {
          txt = (Gv.S("BĚŽÍ · %d L/min") % Gd.roundi(Gd.Dyn("float", m.G("sim").G("S").I(b2).C("get", Gv.S("q"), Gv.F(0.0)))));
        }
        S_art.pill(Self, m.G("font_b"), (Gv.S("%s %s") % Gd.Arr(Gd.Dyn("str", it.C("get", Gv.S("label"), Gv.S(""))), txt)), Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-58L)), Gv.I(13L), col.C("lightened", Gv.F(0.2)));
      }
      else if (Gv.Eq(_t1, Gv.S("streams"))) {
        foreach (Gv s in (_stream_segs()).Iter()) {
          Self.C("draw_line", s.I(Gv.I(0L)), s.I(Gv.I(1L)), Gd.Ctor("Color", s.I(Gv.I(2L)), Gv.F(0.28)), s.I(Gv.I(3L)), Gv.B(true));
          if ((s.I(Gv.I(4L))).T()) {
            Self.C("draw_arc", s.I(Gv.I(1L)), Gv.F(11.0), Gd.K("PI"), Gd.K("TAU"), Gv.I(12L), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.6)), Gv.F(2.0), Gv.B(true));
          }
        }
      }
      else if (Gv.Eq(_t1, Gv.S("leaks"))) {
        Gv seen = Gd.Dict();
        foreach (Gv p in (m.G("sim").G("S").G("leaks")).Iter()) {
          Gv key = (Gv.S("%d,%d") % Gd.Arr(p.G("x"), p.G("y")));
          if ((seen.C("has", key)).T()) {
            continue;
          }
          seen = seen.SI(key, Gv.B(true));
          Gv pts = Gd.Ctor("PackedVector2Array");
          foreach (Gv a_2 in (Gv.I(20L)).Iter()) {
            pts.C("append", ((p + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(52L))) + Gd.Ctor("Vector2", (Gd.cos(((Gd.K("TAU") * a_2) / Gv.F(20.0))) * Gv.I(46L)), (Gd.sin(((Gd.K("TAU") * a_2) / Gv.F(20.0))) * Gv.I(12L)))));
          }
          Self.C("draw_colored_polygon", pts, Gd.Ctor("Color", Gv.F(0.35), Gv.F(0.65), Gv.F(0.9), Gv.F(0.45)));
          Self.C("draw_line", p, (p + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(46L))), Gd.Ctor("Color", Gv.F(0.35), Gv.F(0.65), Gv.F(0.9), Gv.F(0.5)), Gv.F(5.0));
          Self.C("draw_arc", p, Gv.F(22.0), Gv.I(0L), Gd.K("TAU"), Gv.I(24L), Gd.Ctor("Color", Gv.S("e0412f")), Gv.F(3.0), Gv.B(true));
          S_art.pill(Self, m.G("font_b"), Gv.S("VYTÉKÁ!"), (p + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-28L))), Gv.I(13L), Gd.Ctor("Color", Gv.S("ff6b5b")));
        }
      }
    }
    return Gv.Null;
  }
  public Gv _stream_segs() {
    Gv S = m.G("sim").G("S");
    Gv net = m.G("sim").G("net");
    Gv out_ = Gd.Arr();
    foreach (Gv k in (S.G("streams")).Iter()) {
      Gv n = net.G("nodes").C("get", k);
      if (Gv.Eq(n, Gv.Null)) {
        continue;
      }
      Gv a = Gv.Null;
      foreach (Gv at in (n.G("att")).Iter()) {
        if (Gv.Eq(at.G("kind"), Gv.S("vessel"))) {
          a = at;
        }
      }
      if ((Gv.Eq(a, Gv.Null) || (a.G("low")).T())) {
        continue;
      }
      Gv v = net.G("byId").I(a.G("id"));
      Gv s = S.G("streams").I(k);
      Gv col = m.C("kind_color", s.G("kind"), s.G("T"), Gv.F(0.0));
      col = Gd.Ctor("Color", Gd.snappedf(col.G("r"), Gv.F(0.05)), Gd.snappedf(col.G("g"), Gv.F(0.05)), Gd.snappedf(col.G("b"), Gv.F(0.05)));
      Gv surf = ((Gd.roundf((m.C("level_y", v) / Gv.F(4.0))) * Gv.F(4.0))).AsF();
      if (Gv.Ne(a.G("spray"), Gv.Null)) {
        Gv sp = net.G("byId").I(a.G("spray"));
        Gv o = Gd.Ctor("Vector2", sp.G("x"), sp.G("y"));
        foreach (Gv j in (Gv.I(7L)).Iter()) {
          Gv ang = Gd.deg_to_rad((Gv.F(60.0) + (j * Gv.F(10.0))));
          out_.C("append", Gd.Arr(o, Gd.Ctor("Vector2", Gd.roundf((o.G("x") + ((Gd.cos(ang) * ((surf - o.G("y")))) * Gv.F(0.6)))), surf), col, Gv.F(2.0), Gv.B(false)));
        }
      } else {
        Gv x0 = Gd.clampf(Gd.Dyn("float", n.G("x")), (v.G("x") + Gv.I(14L)), ((v.G("x") + v.G("w")) - Gv.I(14L)));
        Gv y1 = (Gd.maxf(n.G("y"), v.G("y"))).AsF();
        out_.C("append", Gd.Arr(Gd.Ctor("Vector2", x0, y1), Gd.Ctor("Vector2", x0, surf), col, Gv.F(7.0), Gv.B(true)));
      }
    }
    return out_;
    return Gv.Null;
  }
  public Gv _rebuild_dash() {
    foreach (Gv d in (dash).Iter()) {
      d.C("queue_free");
    }
    dash.C("clear");
    Gv fl = m.G("flow_layer");
    Gv segs = Gd.Arr();
    if (Gv.Eq(kind, Gv.S("streams"))) {
      foreach (Gv s in (_stream_segs()).Iter()) {
        segs.C("append", Gd.Arr(s.I(Gv.I(0L)), s.I(Gv.I(1L)), s.I(Gv.I(2L)), (Gv.Lt(s.I(Gv.I(3L)), Gv.I(5L)) ? Gv.F(3.0) : Gv.F(5.0))));
      }
    } else {
      Gv seen = Gd.Dict();
      foreach (Gv p in (m.G("sim").G("S").G("leaks")).Iter()) {
        Gv key = (Gv.S("%d,%d") % Gd.Arr(p.G("x"), p.G("y")));
        if ((seen.C("has", key)).T()) {
          continue;
        }
        seen = seen.SI(key, Gv.B(true));
        segs.C("append", Gd.Arr(p, (p + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(46L))), Gd.Ctor("Color", Gv.F(0.55), Gv.F(0.8), Gv.F(1.0)), Gv.F(5.0)));
      }
    }
    foreach (Gv s_2 in (segs).Iter()) {
      Gv l = Gd.New("Line2D");
      l = l.S("points", Gd.Ctor("PackedVector2Array", Gd.Arr(s_2.I(Gv.I(0L)), s_2.I(Gv.I(1L)))));
      l = l.S("width", s_2.I(Gv.I(3L)));
      l = l.S("texture", fl.G("tex"));
      l = l.S("texture_mode", Gd.StaticGet("Line2D", "LINE_TEXTURE_TILE"));
      l = l.S("texture_repeat", Gd.StaticGet("CanvasItem", "TEXTURE_REPEAT_ENABLED"));
      l = l.S("material", fl.G("mat"));
      l = l.S("default_color", s_2.I(Gv.I(2L)).C("lightened", Gv.F(0.25)));
      Self.C("add_child", l);
      dash.C("append", l);
    }
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "anim_t": return this.anim_t;
    case "kind": return this.kind;
    case "it": return this.it;
    case "sig": return this.sig;
    case "st": return this.st;
    case "dash": return this.dash;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "anim_t": this.anim_t = v; return v;
    case "kind": this.kind = v; return v;
    case "it": this.it = v; return v;
    case "sig": this.sig = v; return v;
    case "st": this.st = v; return v;
    case "dash": this.dash = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "upd": return upd();
    case "_process": return _process(Gd.A(a, 0));
    case "_sig": return _sig();
    case "_draw": return _draw();
    case "_stream_segs": return _stream_segs();
    case "_rebuild_dash": return _rebuild_dash();
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_main_LiveNode(); return Gv.O(o); }
  public override string ScriptName { get { return "S_main_LiveNode"; } }
}
public partial class S_main_ItemLayer : GdNode {
  public Gv fx;
  public Gv spin;
  public Gv m;
  public Gv live_only;
  public Gv skip_valves;
  public Gv valves_only;
  public Gv clip;
  public Gv gpu_node;
  public static Gv LQ_T;
  public static Gv LQ_F;
  static S_main_ItemLayer() {
    LQ_T = Gd.Arr(Gv.F(0.0), Gv.F(0.1), Gv.F(0.24), Gv.F(0.33), Gv.F(0.5), Gv.F(0.75), Gv.F(0.9), Gv.F(1.0));
    LQ_F = Gd.Arr(Gv.F(-0.42), Gv.F(-0.2), Gv.F(0.05), Gv.F(0.16), Gv.F(0.02), Gv.F(-0.1), Gv.F(-0.24), Gv.F(-0.45));
  }
  public S_main_ItemLayer() {
    TypeName = "Node2D";
    fx = Gd.Dict();
    spin = Gv.F(0.0);
    m = Gv.Null;
    live_only = Gv.B(false);
    skip_valves = Gv.B(false);
    valves_only = Gv.B(false);
    clip = Gd.Ctor("Rect2");
    gpu_node = Gv.Null;
  }
  public Gv _near(Gv it = default(Gv)) {
    if (Gv.Eq(clip.G("size"), Gd.StaticGet("Vector2", "ZERO"))) {
      return Gv.B(true);
    }
    if ((!(it.C("has", Gv.S("x"))).T() || !(it.C("has", Gv.S("y"))).T())) {
      return Gv.B(false);
    }
    Gv w = (Gd.maxf(Gd.Dyn("float", it.C("get", Gv.S("w"), Gv.I(0L))), Gd.Dyn("float", it.C("get", Gv.S("len"), Gv.I(0L))))).AsF();
    return clip.C("intersects", Gd.Ctor("Rect2", (it.G("x") - Gv.F(90.0)), (it.G("y") - Gv.F(150.0)), (w + Gv.F(180.0)), (Gd.Dyn("float", it.C("get", Gv.S("h"), Gv.I(0L))) + Gv.F(300.0))));
    return Gv.Null;
  }
  public Gv _draw() {
    if ((valves_only).T()) {
      foreach (Gv it in (m.G("items")).Iter()) {
        if (Gv.Eq(it.G("type"), Gv.S("valve"))) {
          _valve(it);
        }
      }
      return Gv.Null;
    }
    if ((live_only).T()) {
      _live_static();
      return Gv.Null;
    }
    _baked();
    return Gv.Null;
  }
  public Gv _hatch_live(Gv it = default(Gv)) {
    Gv is_open = m.G("sim").G("S").G("sk").C("get", Gv.S("hatch"), Gv.B(false));
    Gv p = Gd.Ctor("Vector2", it.G("x"), it.G("y"));
    Gv gcol = ((is_open).T() ? Gd.Ctor("Color", Gv.S("2fbf5b")) : Gd.Ctor("Color", Gv.S("e0412f")));
    Self.C("draw_circle", (p + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(11L))), Gv.F(24.0), Gd.Ctor("Color", gcol, Gv.F(0.18)));
    if ((is_open).T()) {
      Self.C("draw_colored_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr((p + Gd.Ctor("Vector2", Gv.I(-32L), Gv.I(3L))), (p + Gd.Ctor("Vector2", Gv.I(32L), Gv.I(3L))), (p + Gd.Ctor("Vector2", Gv.I(26L), Gv.I(19L))), (p + Gd.Ctor("Vector2", Gv.I(-26L), Gv.I(19L))))), Gd.Ctor("Color", Gv.S("14181c")));
      Self.C("draw_colored_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr((p + Gd.Ctor("Vector2", Gv.I(-32L), Gv.I(3L))), (p + Gd.Ctor("Vector2", Gv.I(-48L), Gv.I(24L))), (p + Gd.Ctor("Vector2", Gv.I(-38L), Gv.I(28L))), (p + Gd.Ctor("Vector2", Gv.I(-26L), Gv.I(8L))))), gcol);
    } else {
      S_art.rrect(Self, Gd.Ctor("Rect2", (p - Gd.Ctor("Vector2", Gv.I(32L), Gv.I(-4L))), Gd.Ctor("Vector2", Gv.I(64L), Gv.I(14L))), gcol, Gv.F(3.0));
    }
    S_art.pill(Self, m.G("font_b"), (Gv.S("výhoz mláta: %s") % (((is_open).T() ? Gv.S("OTEVŘEN") : Gv.S("zavřen")))), (p + Gd.Ctor("Vector2", Gv.I(112L), Gv.I(12L))), Gv.I(11L), gcol);
    return Gv.Null;
  }
  public Gv _splid_live(Gv it = default(Gv)) {
    Gv open = m.G("sim").G("S").G("sp").C("get", Gv.S("lidOpen"), Gv.B(false));
    Gv pr = (m.G("sim").G("S").G("sp").C("get", Gv.S("press"), Gv.F(0.0))).AsF();
    Gv p = Gd.Ctor("Vector2", it.G("x"), it.G("y"));
    Gv gcol = ((open).T() ? Gd.Ctor("Color", Gv.S("2fbf5b")) : ((Gv.Gt(pr, Gv.F(40.0)) ? Gd.Ctor("Color", Gv.S("e0412f")) : Gd.Ctor("Color", Gv.S("f2c230")))));
    S_art.rrect_border(Self, Gd.Ctor("Rect2", (p + Gd.Ctor("Vector2", Gv.I(-30L), Gv.I(-4L))), Gd.Ctor("Vector2", Gv.I(60L), Gv.I(14L))), Gd.Ctor("Color", Gv.S("aab4bc")), Gd.Ctor("Color", Gv.S("3b444c")), Gv.I(2L), Gv.F(3.0));
    if ((open).T()) {
      Self.C("draw_colored_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr((p + Gd.Ctor("Vector2", Gv.I(-26L), Gv.I(-2L))), (p + Gd.Ctor("Vector2", Gv.I(26L), Gv.I(-2L))), (p + Gd.Ctor("Vector2", Gv.I(22L), Gv.I(6L))), (p + Gd.Ctor("Vector2", Gv.I(-22L), Gv.I(6L))))), Gd.Ctor("Color", Gv.S("14181c")));
      Gv hp = (p + Gd.Ctor("Vector2", Gv.I(-30L), Gv.I(-4L)));
      Self.C("draw_colored_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(hp, (hp + Gd.Ctor("Vector2", Gv.I(-8L), Gv.I(-44L))), (hp + Gd.Ctor("Vector2", Gv.I(2L), Gv.I(-46L))), (hp + Gd.Ctor("Vector2", Gv.I(14L), Gv.I(-2L))))), Gd.Ctor("Color", Gv.S("c3ccd3")));
      Self.C("draw_polyline", Gd.Ctor("PackedVector2Array", Gd.Arr(hp, (hp + Gd.Ctor("Vector2", Gv.I(-8L), Gv.I(-44L))), (hp + Gd.Ctor("Vector2", Gv.I(2L), Gv.I(-46L))), (hp + Gd.Ctor("Vector2", Gv.I(14L), Gv.I(-2L))), hp)), Gd.Ctor("Color", Gv.S("3b444c")), Gv.F(2.0), Gv.B(true));
    } else {
      S_art.rrect_border(Self, Gd.Ctor("Rect2", (p + Gd.Ctor("Vector2", Gv.I(-34L), Gv.I(-16L))), Gd.Ctor("Vector2", Gv.I(68L), Gv.I(14L))), Gd.Ctor("Color", Gv.S("c3ccd3")), Gd.Ctor("Color", Gv.S("3b444c")), Gv.I(2L), Gv.F(4.0));
      Self.C("draw_circle", (p + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-9L))), Gv.F(4.0), Gd.Ctor("Color", Gv.S("59626a")));
    }
    S_art.pill(Self, m.G("font_b"), (Gv.S("víčko: %s%s") % Gd.Arr(((open).T() ? Gv.S("OTEVŘENÉ") : Gv.S("zavřené")), ((!(open).T() && Gv.Gt(pr, Gv.F(5.0))) ? ((Gv.S("  přetlak %d mbar!") % Gd.roundi(pr))) : Gv.S("")))), (p + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-62L))), Gv.I(11L), gcol);
    return Gv.Null;
  }
  public Gv _live_static() {
    foreach (Gv it in (m.G("items")).Iter()) {
      if ((Gv.Eq(it.G("type"), Gv.S("deco")) && Gv.Eq(it.C("get", Gv.S("k"), Gv.S("")), Gv.S("hatch")))) {
        _hatch_live(it);
      }
      if ((Gv.Eq(it.G("type"), Gv.S("deco")) && Gv.Eq(it.C("get", Gv.S("k"), Gv.S("")), Gv.S("splid")))) {
        _splid_live(it);
      }
    }
    foreach (Gv it_2 in (m.G("items")).Iter()) {
      if ((Gv.Eq(it_2.G("type"), Gv.S("deco")) && Gv.Eq(it_2.C("get", Gv.S("k"), Gv.S("")), Gv.S("conv")))) {
        _malt_gate(it_2);
      }
    }
    foreach (Gv it_3 in (m.G("items")).Iter()) {
      {
        Gv _t1 = it_3.G("type");
        if (Gv.Eq(_t1, Gv.S("dip"))) {
          _dip(it_3);
        }
        else if (Gv.Eq(_t1, Gv.S("gauge"))) {
          _gauge_cocks(it_3);
        }
        else if (Gv.Eq(_t1, Gv.S("sw"))) {
          _sw(it_3);
        }
        else if (Gv.Eq(_t1, Gv.S("vessel"))) {
          if (Gv.Eq(it_3.C("get", Gv.S("shape")), Gv.S("inlet"))) {
            _inlet(it_3);
          }
        }
      }
    }
    foreach (Gv it_4 in (m.G("items")).Iter()) {
      if ((Gv.Eq(it_4.G("type"), Gv.S("pipe")) && (it_4.C("get", Gv.S("hose"), Gv.B(false))).T())) {
        _hose(it_4);
      } else if ((Gv.Eq(it_4.G("type"), Gv.S("pipe")) && Gv.Eq(it_4.C("get", Gv.S("look"), Gv.S("")), Gv.S("hose")))) {
        _hose_vis(it_4);
      }
    }
    foreach (Gv o in (m.G("outlets")).Iter()) {
      _outlet(o);
    }
    return Gv.Null;
  }
  public Gv _baked() {
    foreach (Gv it in (m.G("items")).Iter()) {
      if (((Gv.Eq(it.G("type"), Gv.S("vessel")) && !Gv.In(it.C("get", Gv.S("shape")), Gd.Arr(Gv.S("platform"), Gv.S("inlet")))) && (_near(it)).T())) {
        Gv w = (it.C("get", Gv.S("w"), Gv.I(100L))).AsF();
        Gv h = (it.C("get", Gv.S("h"), Gv.I(100L))).AsF();
        if (Gv.In(it.C("get", Gv.S("shape")), Gd.Arr(Gv.S("plate"), Gv.S("chiller")))) {
          S_art.ellipse_shadow(Self, Gd.Ctor("Vector2", ((it.G("x") + (w * Gv.F(0.5))) + Gv.I(12L)), ((it.G("y") + h) + Gv.I(26L))), (w * Gv.F(0.55)), Gv.F(18.0));
        } else {
          S_art.floor_shadow(Self, Gd.Ctor("Vector2", ((it.G("x") + (w * Gv.F(0.5))) + Gv.I(10L)), (m.C("foot_y", it) - Gv.F(2.0))), (w * Gv.F(0.52)), Gv.F(9.0));
        }
      }
    }
    foreach (Gv it_2 in (m.G("items")).Iter()) {
      if (!(_near(it_2)).T()) {
        continue;
      }
      {
        Gv _t1 = it_2.G("type");
        if (Gv.Eq(_t1, Gv.S("vessel"))) {
          if (Gv.Ne(it_2.C("get", Gv.S("shape")), Gv.S("inlet"))) {
            _vessel(it_2);
          }
        }
        else if (Gv.Eq(_t1, Gv.S("pump"))) {
          _pump(it_2);
        }
        else if (Gv.Eq(_t1, Gv.S("spray"))) {
          _spray(it_2);
        }
        else if (Gv.Eq(_t1, Gv.S("nozzle"))) {
          _nozzle(it_2);
        }
        else if (Gv.Eq(_t1, Gv.S("panel"))) {
          ;
        }
        else if (Gv.Eq(_t1, Gv.S("manifold"))) {
          _manifold(it_2);
        }
        else if (Gv.Eq(_t1, Gv.S("gauge"))) {
          _gauge(it_2);
        }
        else if (Gv.Eq(_t1, Gv.S("deco"))) {
          _deco(it_2);
        }
        else if (Gv.Eq(_t1, Gv.S("rake"))) {
          _rake(it_2);
        }
        else if (Gv.Eq(_t1, Gv.S("fitting")) || Gv.Eq(_t1, Gv.S("inl")) || Gv.Eq(_t1, Gv.S("hopf")) || Gv.Eq(_t1, Gv.S("flow"))) {
          _small(it_2);
        }
      }
    }
    if (!(skip_valves).T()) {
      foreach (Gv it_3 in (m.G("items")).Iter()) {
        if (Gv.Eq(it_3.G("type"), Gv.S("valve"))) {
          _valve(it_3);
        }
      }
    }
    return Gv.Null;
  }
  public Gv _vessel(Gv it = default(Gv)) {
    Gv x = (it.G("x")).AsF();
    Gv y = (it.G("y")).AsF();
    Gv w = (it.C("get", Gv.S("w"), Gv.I(100L))).AsF();
    Gv h = (it.C("get", Gv.S("h"), Gv.I(100L))).AsF();
    Gv shape = it.C("get", Gv.S("shape"), Gv.S("tank"));
    Gv bind = it.C("get", Gv.S("bind"));
    Gv cx = (x + (w * Gv.F(0.5)));
    {
      Gv _t1 = shape;
      if (Gv.Eq(_t1, Gv.S("platform"))) {
        Self.C("draw_rect", Gd.Ctor("Rect2", x, y, w, h), Gd.Ctor("Color", Gv.S("3b434b")));
        Gv i = Gv.F(0.0);
        while (Gv.Lt(i, w)) {
          Self.C("draw_colored_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", (x + i), (y + h)), Gd.Ctor("Vector2", ((x + i) + Gv.I(12L)), y), Gd.Ctor("Vector2", ((x + i) + Gv.I(24L)), y), Gd.Ctor("Vector2", ((x + i) + Gv.I(12L)), (y + h)))), Gd.Ctor("Color", Gv.S("f2c230")));
          i = (i + Gv.F(36.0));
        }
        Self.C("draw_rect", Gd.Ctor("Rect2", x, y, w, h), Gd.Ctor("Color", Gv.S("15191d")), Gv.B(false), Gv.F(2.0));
        S_art.pill(Self, m.G("font"), it.G("label"), Gd.Ctor("Vector2", ((x + w) - Gv.I(50L)), (y - Gv.I(8L))), Gv.I(12L));
        return Gv.Null;
      }
      else if (Gv.Eq(_t1, Gv.S("inlet"))) {
        S_art.rrect_border(Self, Gd.Ctor("Rect2", x, y, w, h), Gd.Ctor("Color", Gv.S("2457b8")), Gd.Ctor("Color", Gv.S("0e2a5c")), Gv.I(2L), Gv.F(6.0));
        S_art.text_c(Self, m.G("font_b"), (false ? Gv.S("💧 řad") : Gv.S("ŘAD")), Gd.Ctor("Vector2", cx, ((y + (h * Gv.F(0.5))) + Gv.I(6L))), Gv.I(14L), Gd.StaticGet("Color", "WHITE"));
        S_art.pill(Self, m.G("font"), it.G("label"), Gd.Ctor("Vector2", cx, (y - Gv.I(10L))), Gv.I(12L));
        return Gv.Null;
      }
      else if (Gv.Eq(_t1, Gv.S("plate"))) {
        _plate(it);
        return Gv.Null;
      }
      else if (Gv.Eq(_t1, Gv.S("chiller"))) {
        _chiller(it);
        return Gv.Null;
      }
    }
    Gv stops = S_art.STAINLESS;
    if (Gv.Eq(bind, Gv.S("hlt"))) {
      stops = S_art.CLAD;
    } else if ((Gv.Eq(shape, Gv.S("horiz")) || (it.C("get", Gv.S("dark"), Gv.B(false))).T())) {
      stops = S_art.NAVY;
    }
    if (Gv.Ne(shape, Gv.S("horiz"))) {
      Gv fy_ = (m.C("foot_y", it)).AsF();
      foreach (Gv lx in (Gd.Arr((x + (w * Gv.F(0.12))), ((x + (w * Gv.F(0.88))) - Gv.I(14L)))).Iter()) {
        Gv ll_ = ((fy_ - Gv.F(8.0)) - (((y + h) - Gv.I(10L))));
        S_art.cyl_v(Self, Gd.Ctor("Rect2", lx, ((y + h) - Gv.I(10L)), Gv.I(14L), ll_), S_art.STAINLESS, Gv.I(6L));
        Self.C("draw_rect", Gd.Ctor("Rect2", lx, ((y + h) - Gv.I(10L)), Gv.I(14L), ll_), Gd.Ctor("Color", Gv.S("1e2429")), Gv.B(false), Gv.F(1.0));
        Self.C("draw_rect", Gd.Ctor("Rect2", (lx + Gv.I(5L)), (fy_ - Gv.I(9L)), Gv.I(4L), Gv.I(5L)), Gd.Ctor("Color", Gv.S("5d666e")));
        S_art.rrect(Self, Gd.Ctor("Rect2", (lx - Gv.I(5L)), (fy_ - Gv.I(5L)), Gv.I(24L), Gv.I(5L)), Gd.Ctor("Color", Gv.S("2b3138")), Gv.F(2.0));
      }
    } else {
      foreach (Gv lx_2 in (Gd.Arr((x + (w * Gv.F(0.2))), ((x + (w * Gv.F(0.8))) - Gv.I(30L)))).Iter()) {
        Gv sp_ = Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", lx_2, (y + (h * Gv.F(0.7)))), Gd.Ctor("Vector2", (lx_2 + Gv.I(30L)), (y + (h * Gv.F(0.7)))), Gd.Ctor("Vector2", (lx_2 + Gv.I(40L)), ((y + h) + Gv.I(30L))), Gd.Ctor("Vector2", (lx_2 - Gv.I(10L)), ((y + h) + Gv.I(30L)))));
        Self.C("draw_polygon", sp_, Gd.Ctor("PackedColorArray", Gd.Arr(Gd.Ctor("Color", Gv.S("5d666e")), Gd.Ctor("Color", Gv.S("3b434b")), Gd.Ctor("Color", Gv.S("2b3138")), Gd.Ctor("Color", Gv.S("4d555c")))));
        Self.C("draw_rect", Gd.Ctor("Rect2", (lx_2 - Gv.I(14L)), ((y + h) + Gv.I(28L)), Gv.I(58L), Gv.I(6L)), Gd.Ctor("Color", Gv.S("2b3138")));
      }
    }
    if (Gv.Eq(shape, Gv.S("horiz"))) {
      Gv rr = (h * Gv.F(0.5));
      if (!(S_art.tex_rect(Self, Gv.S("vs_horiz"), Gd.Ctor("Rect2", x, y, w, h), Gd.Ctor("Color", Gv.F(0.42), Gv.F(0.58), Gv.F(1.0)))).T()) {
        S_art.stadium(Self, Gd.Ctor("Rect2", x, y, w, h), stops);
        Self.C("draw_line", Gd.Ctor("Vector2", (x + rr), (y + (h * Gv.F(0.22)))), Gd.Ctor("Vector2", ((x + w) - rr), (y + (h * Gv.F(0.22)))), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.22)), (h * Gv.F(0.06)));
      }
      foreach (Gv bx in (Gd.Arr((x + (w * Gv.F(0.3))), (x + (w * Gv.F(0.7))))).Iter()) {
        Self.C("draw_line", Gd.Ctor("Vector2", bx, (y + Gv.I(3L))), Gd.Ctor("Vector2", bx, ((y + h) - Gv.I(3L))), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.3)), Gv.F(3.0));
      }
      S_art.nameplate(Self, m.G("font_b"), it.G("label"), cx, (y - Gv.I(34L)), Gv.I(16L));
      return Gv.Null;
    }
    Gv cone_h = Gv.F(0.0);
    Gv bottom_w = w;
    if ((Gv.Eq(shape, Gv.S("tank")) && Gv.Eq(bind, Gv.S("rvp")))) {
      cone_h = Gd.minf((h * Gv.F(0.12)), Gv.F(40.0));
      bottom_w = Gv.F(20.0);
    } else if ((Gv.Eq(shape, Gv.S("cone")) && Gv.Ne(bind, Gv.S("wp")))) {
      cone_h = Gd.minf((h * Gv.F(0.25)), Gv.F(60.0));
      bottom_w = Gv.F(16.0);
    }
    Gv body = Gd.Ctor("Rect2", x, y, w, (h - cone_h));
    Gv tint = (Gv.Eq(bind, Gv.S("hlt")) ? Gd.Ctor("Color", Gv.F(0.86), Gv.F(0.9), Gv.F(1.0)) : Gd.StaticGet("Color", "WHITE"));
    if (!(S_art.tex_rect(Self, Gv.S("vs_body"), body, tint)).T()) {
      S_art.cyl_v(Self, body, stops, Gv.I(40L));
      S_art.brushed(Self, body);
    }
    Self.C("draw_rect", Gd.Ctor("Rect2", (x + (w * Gv.F(0.73))), (y + Gv.I(6L)), (w * Gv.F(0.035)), (body.G("size").G("y") - Gv.I(12L))), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.1)));
    Self.C("draw_rect", Gd.Ctor("Rect2", (x + (w * Gv.F(0.79))), (y + Gv.I(6L)), (w * Gv.F(0.012)), (body.G("size").G("y") - Gv.I(12L))), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.08)));
    if (Gv.Gt(cone_h, Gv.F(0.0))) {
      if (!(S_art.tex_rect(Self, Gv.S("vs_cone"), Gd.Ctor("Rect2", x, ((y + h) - cone_h), w, cone_h))).T()) {
        S_art.cone(Self, cx, ((y + h) - cone_h), w, bottom_w, cone_h, stops);
      }
      S_art.cyl_v(Self, Gd.Ctor("Rect2", ((cx - (bottom_w * Gv.F(0.5))) - Gv.I(3L)), ((y + h) - Gv.I(2L)), (bottom_w + Gv.I(6L)), Gv.I(8L)), S_art.STAINLESS, Gv.I(6L));
    } else {
      S_art.cyl_v(Self, Gd.Ctor("Rect2", x, ((y + h) - Gv.I(10L)), w, Gv.I(10L)), stops, Gv.I(24L));
      Self.C("draw_rect", Gd.Ctor("Rect2", x, ((y + h) - Gv.I(10L)), w, Gv.I(10L)), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.22)));
    }
    foreach (Gv k in (Gd.Arr(Gv.F(0.08), Gv.F(0.5), Gv.F(0.92))).Iter()) {
      Gv by = ((y + (((h - cone_h)) * k))).AsF();
      Self.C("draw_line", Gd.Ctor("Vector2", x, by), Gd.Ctor("Vector2", (x + w), by), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.22)), Gv.F(3.0));
      Self.C("draw_line", Gd.Ctor("Vector2", x, (by + Gv.I(2L))), Gd.Ctor("Vector2", (x + w), (by + Gv.I(2L))), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.12)), Gv.F(1.0));
    }
    Gv o = Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", x, y), Gd.Ctor("Vector2", x, ((y + h) - cone_h))));
    if (Gv.Gt(cone_h, Gv.F(0.0))) {
      o.C("append_array", Gd.Arr(Gd.Ctor("Vector2", (cx - (bottom_w * Gv.F(0.5))), (y + h)), Gd.Ctor("Vector2", (cx + (bottom_w * Gv.F(0.5))), (y + h))));
    } else {
      o.C("append_array", Gd.Arr(Gd.Ctor("Vector2", x, (y + h)), Gd.Ctor("Vector2", (x + w), (y + h))));
    }
    o.C("append_array", Gd.Arr(Gd.Ctor("Vector2", (x + w), ((y + h) - cone_h)), Gd.Ctor("Vector2", (x + w), y)));
    Self.C("draw_polyline", o, Gd.Ctor("Color", Gv.S("0d1013")), Gv.F(3.0), Gv.B(true));
    Gv lid_stops = (Gv.In(bind, Gd.Arr(Gv.S("rvp"), Gv.S("sk"))) ? S_art.COPPER : stops);
    if (Gv.In(shape, Gd.Arr(Gv.S("tank"), Gv.S("lauter"), Gv.S("tall"), Gv.S("cone")))) {
      Gv dh = (Gv.Eq(bind, Gv.S("wp")) ? Gv.F(14.0) : ((Gv.Ne(bind, Gv.S("hlt")) ? Gv.F(34.0) : Gv.F(24.0))));
      Gv cop = (Gv.In(bind, Gd.Arr(Gv.S("rvp"), Gv.S("sk"))) ? Gd.Ctor("Color", Gv.F(1.22), Gv.F(0.78), Gv.F(0.6)) : tint);
      if (!(S_art.tex_rect(Self, Gv.S("vs_dome"), Gd.Ctor("Rect2", (cx - (((w + Gv.I(8L))) * Gv.F(0.5))), (y - dh), (w + Gv.I(8L)), (dh + Gv.I(1L))), cop)).T()) {
        S_art.dome(Self, cx, y, (w + Gv.I(8L)), dh, lid_stops);
      }
      S_art.cyl_v(Self, Gd.Ctor("Rect2", (x - Gv.I(6L)), (y - Gv.I(4L)), (w + Gv.I(12L)), Gv.I(8L)), S_art.STAINLESS, Gv.I(24L));
      Self.C("draw_rect", Gd.Ctor("Rect2", (x - Gv.I(6L)), (y - Gv.I(4L)), (w + Gv.I(12L)), Gv.I(8L)), Gd.Ctor("Color", Gv.S("1e2429")), Gv.B(false), Gv.F(1.5));
      Gv bx_ = (x + Gv.F(4.0));
      while (Gv.Lt(bx_, (x + w))) {
        Self.C("draw_circle", Gd.Ctor("Vector2", bx_, y), Gv.F(1.6), Gd.Ctor("Color", Gv.S("3b434a")));
        bx_ = (bx_ + Gv.F(22.0));
      }
    } else if (Gv.Eq(bind, Gv.S("wp"))) {
      Self.C("draw_rect", Gd.Ctor("Rect2", (x - Gv.I(4L)), (y - Gv.I(6L)), (w + Gv.I(8L)), Gv.I(8L)), S_art.grad(stops, Gv.F(0.3)).C("darkened", Gv.F(0.1)));
    }
    if (Gv.In(bind, Gd.Arr(Gv.S("rvp"), Gv.S("sk"), Gv.S("wp"), Gv.S("hlt")))) {
      Gv gx = ((x + w) - Gv.I(26L));
      S_art.rrect(Self, Gd.Ctor("Rect2", gx, (y + Gv.I(30L)), Gv.I(12L), (((h - cone_h)) - Gv.I(60L))), Gd.Ctor("Color", Gv.F(0.55), Gv.F(0.78), Gv.F(0.9), Gv.F(0.35)), Gv.F(4.0));
      Self.C("draw_rect", Gd.Ctor("Rect2", gx, (y + Gv.I(30L)), Gv.I(12L), (((h - cone_h)) - Gv.I(60L))), Gd.Ctor("Color", Gv.S("0d1013")), Gv.B(false), Gv.F(1.5));
    }
    S_art.nameplate(Self, m.G("font_b"), it.G("label"), cx, (y + Gv.I(16L)), Gv.I(16L));
    _vdetails(it, x, y, w, h, cone_h, bottom_w);
    return Gv.Null;
  }
  public Gv _lid_fit(Gv px = default(Gv), Gv py = default(Gv), Gv kind = default(Gv)) {
    px = (px).AsF();
    py = (py).AsF();
    Gv dark = Gd.Ctor("Color", Gv.S("0d1013"));
    Gv bolt = Gd.Ctor("Color", Gv.S("3b434a"));
    if (Gv.Eq(kind, Gv.S("cip"))) {
      S_art.rrect(Self, Gd.Ctor("Rect2", (px - Gv.I(15L)), (py - Gv.I(4L)), Gv.I(30L), Gv.I(7L)), Gd.Ctor("Color", Gv.S("8e98a2")), Gv.F(2.0));
      S_art.cyl_v(Self, Gd.Ctor("Rect2", (px - Gv.I(8L)), (py - Gv.I(24L)), Gv.I(16L), Gv.I(22L)), S_art.STAINLESS, Gv.I(8L));
      Self.C("draw_rect", Gd.Ctor("Rect2", (px - Gv.I(8L)), (py - Gv.I(24L)), Gv.I(16L), Gv.I(22L)), dark, Gv.B(false), Gv.F(1.2));
      S_art.rrect(Self, Gd.Ctor("Rect2", (px - Gv.I(13L)), (py - Gv.I(29L)), Gv.I(26L), Gv.I(6L)), Gd.Ctor("Color", Gv.S("aeb7bf")), Gv.F(2.0));
      foreach (Gv k in (Gv.I(4L)).Iter()) {
        Self.C("draw_circle", Gd.Ctor("Vector2", ((px - Gv.I(9L)) + (k * Gv.F(6.0))), (py - Gv.I(26L))), Gv.F(1.2), bolt);
      }
      S_art.cyl_v(Self, Gd.Ctor("Rect2", (px - Gv.I(5L)), (py - Gv.I(37L)), Gv.I(10L), Gv.I(8L)), S_art.STAINLESS, Gv.I(6L));
    } else if (Gv.Eq(kind, Gv.S("vent"))) {
      S_art.rrect(Self, Gd.Ctor("Rect2", (px - Gv.I(13L)), (py - Gv.I(4L)), Gv.I(26L), Gv.I(7L)), Gd.Ctor("Color", Gv.S("8e98a2")), Gv.F(2.0));
      S_art.cyl_v(Self, Gd.Ctor("Rect2", (px - Gv.I(7L)), (py - Gv.I(30L)), Gv.I(14L), Gv.I(28L)), S_art.STAINLESS, Gv.I(8L));
      Self.C("draw_rect", Gd.Ctor("Rect2", (px - Gv.I(7L)), (py - Gv.I(30L)), Gv.I(14L), Gv.I(28L)), dark, Gv.B(false), Gv.F(1.2));
      S_art.cyl_v(Self, Gd.Ctor("Rect2", (px - Gv.I(11L)), (py - Gv.I(40L)), Gv.I(22L), Gv.I(11L)), S_art.STAINLESS, Gv.I(10L));
      Self.C("draw_rect", Gd.Ctor("Rect2", (px - Gv.I(11L)), (py - Gv.I(40L)), Gv.I(22L), Gv.I(11L)), dark, Gv.B(false), Gv.F(1.2));
      Self.C("draw_circle", Gd.Ctor("Vector2", px, (py - Gv.I(43L))), Gv.F(6.0), Gd.Ctor("Color", Gv.S("c7cfd6")));
      Self.C("draw_arc", Gd.Ctor("Vector2", px, (py - Gv.I(43L))), Gv.F(6.0), Gv.I(0L), Gd.K("TAU"), Gv.I(16L), dark, Gv.F(1.2), Gv.B(true));
    } else if (Gv.Eq(kind, Gv.S("sight"))) {
      S_art.rrect(Self, Gd.Ctor("Rect2", (px - Gv.I(22L)), (py - Gv.I(8L)), Gv.I(44L), Gv.I(10L)), Gd.Ctor("Color", Gv.S("8e98a2")), Gv.F(3.0));
      S_art.rrect(Self, Gd.Ctor("Rect2", (px - Gv.I(16L)), (py - Gv.I(13L)), Gv.I(32L), Gv.I(6L)), Gd.Ctor("Color", Gv.F(0.62), Gv.F(0.78), Gv.F(0.9), Gv.F(0.55)), Gv.F(3.0));
      Self.C("draw_line", Gd.Ctor("Vector2", (px - Gv.I(11L)), (py - Gv.I(11L))), Gd.Ctor("Vector2", (px - Gv.I(2L)), (py - Gv.I(11L))), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.7)), Gv.F(1.5));
      foreach (Gv k_2 in (Gv.I(3L)).Iter()) {
        Self.C("draw_circle", Gd.Ctor("Vector2", ((px - Gv.I(18L)) + (k_2 * Gv.F(18.0))), (py - Gv.I(3L))), Gv.F(1.4), bolt);
      }
    }
    return Gv.Null;
  }
  public Gv _vdetails(Gv it = default(Gv), Gv x = default(Gv), Gv y = default(Gv), Gv w = default(Gv), Gv h = default(Gv), Gv cone_h = default(Gv), Gv bottom_w = default(Gv)) {
    x = (x).AsF();
    y = (y).AsF();
    w = (w).AsF();
    h = (h).AsF();
    cone_h = (cone_h).AsF();
    bottom_w = (bottom_w).AsF();
    Gv bind = it.C("get", Gv.S("bind"));
    Gv shape = it.C("get", Gv.S("shape"), Gv.S("tank"));
    Gv cx = (x + (w * Gv.F(0.5)));
    Gv bh = ((h - cone_h)).AsF();
    Gv dark = Gd.Ctor("Color", Gv.S("0d1013"));
    Gv bolt = Gd.Ctor("Color", Gv.S("3b434a"));
    if (Gv.In(shape, Gd.Arr(Gv.S("tank"), Gv.S("lauter"), Gv.S("tall"), Gv.S("cone")))) {
      Self.C("draw_line", Gd.Ctor("Vector2", (x - Gv.I(6L)), (y - Gv.I(3L))), Gd.Ctor("Vector2", ((x + w) + Gv.I(6L)), (y - Gv.I(3L))), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.32)), Gv.F(1.5));
      Self.C("draw_line", Gd.Ctor("Vector2", (x - Gv.I(6L)), (y + Gv.I(3L))), Gd.Ctor("Vector2", ((x + w) + Gv.I(6L)), (y + Gv.I(3L))), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.3)), Gv.F(1.5));
      if (Gv.Ne(bind, Gv.S("wp"))) {
        Gv dh = ((Gv.Ne(bind, Gv.S("hlt")) ? Gv.F(34.0) : Gv.F(24.0))).AsF();
        Gv rr = ((((w + Gv.I(8L))) * Gv.F(0.5))).AsF();
        foreach (Gv f in (Gd.Arr(Gd.Arr(Gv.F(0.15), Gv.S("cip")), Gd.Arr(Gv.F(-0.3), Gv.S("vent")), Gd.Arr(Gv.F(-0.12), Gv.S("sight")))).Iter()) {
          if ((Gv.Eq(bind, Gv.S("lv")) && Gv.Eq(f.I(Gv.I(1L)), Gv.S("cip")))) {
            continue;
          }
          Gv dx = ((w * Gd.Dyn("float", f.I(Gv.I(0L))))).AsF();
          Gv k = (Gd.clampf((Gd.absf(dx) / rr), Gv.F(0.0), Gv.F(0.98))).AsF();
          _lid_fit((cx + dx), ((y - (dh * Gd.sqrt((Gv.F(1.0) - (k * k))))) + Gv.F(2.0)), Gd.Dyn("str", f.I(Gv.I(1L))));
        }
      }
    }
    Gv ym = ((y + (bh * Gv.F(0.58)))).AsF();
    Gv mr = (Gd.minf(Gv.F(36.0), (bh * Gv.F(0.2)))).AsF();
    S_art.cyl_h(Self, Gd.Ctor("Rect2", (x - Gv.I(14L)), (ym - mr), Gv.I(14L), (mr * Gv.F(2.0))), S_art.STAINLESS, Gv.I(10L));
    Self.C("draw_rect", Gd.Ctor("Rect2", (x - Gv.I(14L)), (ym - mr), Gv.I(14L), (mr * Gv.F(2.0))), dark, Gv.B(false), Gv.F(1.5));
    foreach (Gv k_2 in (Gv.I(6L)).Iter()) {
      Self.C("draw_circle", Gd.Ctor("Vector2", (x - Gv.I(7L)), (((ym - mr) + Gv.I(6L)) + ((k_2 * (((mr * Gv.F(2.0)) - Gv.I(12L)))) / Gv.F(5.0)))), Gv.F(1.6), bolt);
    }
    S_art.rrect(Self, Gd.Ctor("Rect2", (x - Gv.I(22L)), (ym - (mr * Gv.F(0.82))), Gv.I(9L), (mr * Gv.F(1.64))), Gd.Ctor("Color", Gv.S("aeb7bf")), Gv.F(3.0));
    Self.C("draw_rect", Gd.Ctor("Rect2", (x - Gv.I(22L)), (ym - (mr * Gv.F(0.82))), Gv.I(9L), (mr * Gv.F(1.64))), dark, Gv.B(false), Gv.F(1.2));
    S_art.rrect(Self, Gd.Ctor("Rect2", (x - Gv.I(20L)), ((ym - mr) - Gv.I(9L)), Gv.I(12L), Gv.I(9L)), Gd.Ctor("Color", Gv.S("6c757d")), Gv.F(2.0));
    Self.C("draw_line", Gd.Ctor("Vector2", (x - Gv.I(17L)), (ym - Gv.I(3L))), Gd.Ctor("Vector2", (x - Gv.I(30L)), (ym - Gv.I(3L))), Gd.Ctor("Color", Gv.S("2b3138")), Gv.F(3.0), Gv.B(true));
    Self.C("draw_circle", Gd.Ctor("Vector2", (x - Gv.I(31L)), (ym - Gv.I(3L))), Gv.F(3.0), Gd.Ctor("Color", Gv.S("c7cfd6")));
    Gv sy = ((y + (bh * Gv.F(0.3)))).AsF();
    Self.C("draw_line", Gd.Ctor("Vector2", (x + w), sy), Gd.Ctor("Vector2", ((x + w) + Gv.I(14L)), sy), Gd.Ctor("Color", Gv.S("8e98a2")), Gv.F(5.0), Gv.B(true));
    S_art.rrect(Self, Gd.Ctor("Rect2", ((x + w) + Gv.I(12L)), (sy - Gv.I(9L)), Gv.I(14L), Gv.I(18L)), Gd.Ctor("Color", Gv.S("20262b")), Gv.F(3.0));
    Self.C("draw_rect", Gd.Ctor("Rect2", ((x + w) + Gv.I(12L)), (sy - Gv.I(9L)), Gv.I(14L), Gv.I(18L)), Gd.Ctor("Color", Gv.S("5d666e")), Gv.B(false), Gv.F(1.2));
    Self.C("draw_circle", Gd.Ctor("Vector2", ((x + w) + Gv.I(19L)), (sy - Gv.I(3L))), Gv.F(2.0), Gd.Ctor("Color", Gv.S("c7cfd6")));
    Gv cab = Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", ((x + w) + Gv.I(19L)), (sy + Gv.I(9L))), Gd.Ctor("Vector2", ((x + w) + Gv.I(19L)), (sy + Gv.I(22L))), Gd.Ctor("Vector2", ((x + w) + Gv.I(11L)), (sy + Gv.I(34L))), Gd.Ctor("Vector2", ((x + w) + Gv.I(11L)), (sy + Gv.I(50L)))));
    Self.C("draw_polyline", cab, Gd.Ctor("Color", Gv.S("15191d")), Gv.F(2.0), Gv.B(true));
    if (Gv.Gt(cone_h, Gv.F(0.0))) {
      Gv yb = ((y + h)).AsF();
      S_art.cyl_v(Self, Gd.Ctor("Rect2", (cx - Gv.I(8L)), (yb + Gv.I(5L)), Gv.I(16L), Gv.I(14L)), S_art.STAINLESS, Gv.I(6L));
      foreach (Gv fy in (Gd.Arr((yb + Gv.F(7.0)), (yb + Gv.F(14.0)))).Iter()) {
        S_art.cyl_v(Self, Gd.Ctor("Rect2", (cx - Gv.I(14L)), fy, Gv.I(28L), Gv.I(4L)), S_art.STAINLESS, Gv.I(8L));
        Self.C("draw_rect", Gd.Ctor("Rect2", (cx - Gv.I(14L)), fy, Gv.I(28L), Gv.I(4L)), dark, Gv.B(false), Gv.F(1.0));
      }
      Self.C("draw_rect", Gd.Ctor("Rect2", (cx - Gv.I(15L)), (yb + Gv.F(10.5)), Gv.I(30L), Gv.I(4L)), Gd.Ctor("Color", Gv.S("8e98a2")));
      Self.C("draw_circle", Gd.Ctor("Vector2", (cx + Gv.I(15L)), (yb + Gv.F(12.5))), Gv.F(3.5), Gd.Ctor("Color", Gv.S("c7cfd6")));
      Self.C("draw_arc", Gd.Ctor("Vector2", (cx + Gv.I(15L)), (yb + Gv.F(12.5))), Gv.F(3.5), Gv.I(0L), Gd.K("TAU"), Gv.I(12L), dark, Gv.F(1.0), Gv.B(true));
    }
    if (Gv.Ne(shape, Gv.S("horiz"))) {
      Gv fy_ = (m.C("foot_y", it)).AsF();
      Gv l1 = (((x + (w * Gv.F(0.12))) + Gv.F(7.0))).AsF();
      Gv l2 = (((x + (w * Gv.F(0.88))) - Gv.F(7.0))).AsF();
      Gv ytop = (((y + h) - Gv.F(10.0))).AsF();
      Gv ll = (((fy_ - Gv.F(8.0)) - ytop)).AsF();
      if (Gv.Gt(ll, Gv.F(50.0))) {
        Gv yb1 = ((ytop + (ll * Gv.F(0.35)))).AsF();
        Gv yb2 = ((ytop + (ll * Gv.F(0.8)))).AsF();
        Self.C("draw_line", Gd.Ctor("Vector2", l1, yb2), Gd.Ctor("Vector2", l2, yb2), Gd.Ctor("Color", Gv.S("2b3138")), Gv.F(4.0));
        Self.C("draw_line", Gd.Ctor("Vector2", l1, (yb2 - Gv.F(1.5))), Gd.Ctor("Vector2", l2, (yb2 - Gv.F(1.5))), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.25)), Gv.F(1.0));
        Self.C("draw_line", Gd.Ctor("Vector2", l1, yb1), Gd.Ctor("Vector2", l2, yb2), Gd.Ctor("Color", Gv.S("2b3138")), Gv.F(2.5), Gv.B(true));
        Self.C("draw_line", Gd.Ctor("Vector2", l2, yb1), Gd.Ctor("Vector2", l1, yb2), Gd.Ctor("Color", Gv.S("2b3138")), Gv.F(2.5), Gv.B(true));
      }
      foreach (Gv lx in (Gd.Arr((x + (w * Gv.F(0.12))), ((x + (w * Gv.F(0.88))) - Gv.I(14L)))).Iter()) {
        foreach (Gv k_3 in (Gv.I(2L)).Iter()) {
          Self.C("draw_circle", Gd.Ctor("Vector2", ((lx - Gv.I(1L)) + (k_3 * Gv.F(16.0))), (fy_ - Gv.F(2.5))), Gv.F(1.3), Gd.Ctor("Color", Gv.S("8e98a2")));
        }
      }
    }
    return Gv.Null;
  }
  public Gv _plate(Gv it = default(Gv)) {
    Gv x = (it.G("x")).AsF();
    Gv y = (it.G("y")).AsF();
    Gv w = (it.G("w")).AsF();
    Gv h = (it.G("h")).AsF();
    S_art.shadow(Self, Gd.Ctor("Rect2", x, y, w, h));
    Self.C("draw_rect", Gd.Ctor("Rect2", (x - Gv.I(6L)), (y - Gv.I(10L)), (w + Gv.I(12L)), Gv.I(14L)), Gd.Ctor("Color", Gv.S("2457b8")));
    Self.C("draw_rect", Gd.Ctor("Rect2", (x - Gv.I(6L)), ((y + h) - Gv.I(4L)), (w + Gv.I(12L)), Gv.I(14L)), Gd.Ctor("Color", Gv.S("2457b8")));
    S_art.cyl_v(Self, Gd.Ctor("Rect2", x, y, w, h), S_art.STAINLESS, Gv.I(8L));
    Gv yy = (y + Gv.F(6.0));
    while (Gv.Lt(yy, ((y + h) - Gv.I(4L)))) {
      Self.C("draw_line", Gd.Ctor("Vector2", (x + Gv.I(3L)), yy), Gd.Ctor("Vector2", ((x + w) - Gv.I(3L)), yy), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.35)), Gv.F(2.0));
      yy = (yy + Gv.F(7.0));
    }
    Self.C("draw_rect", Gd.Ctor("Rect2", x, y, w, h), Gd.Ctor("Color", Gv.S("0d1013")), Gv.B(false), Gv.F(2.0));
    S_art.pill(Self, m.G("font"), it.G("label"), Gd.Ctor("Vector2", (x + (w * Gv.F(0.5))), ((y + h) + Gv.I(30L))), Gv.I(12L));
    return Gv.Null;
  }
  public Gv _chiller(Gv it = default(Gv)) {
    Gv r = Gd.Ctor("Rect2", it.G("x"), it.G("y"), it.G("w"), it.G("h"));
    S_art.shadow(Self, r);
    S_art.rrect_border(Self, r, Gd.Ctor("Color", Gv.S("d9dee2")), Gd.Ctor("Color", Gv.S("6c757d")), Gv.I(3L), Gv.F(8.0));
    foreach (Gv k in (Gv.I(2L)).Iter()) {
      Gv c = (r.G("position") + Gd.Ctor("Vector2", (r.G("size").G("x") * ((Gv.F(0.3) + (Gv.F(0.4) * k)))), (r.G("size").G("y") * Gv.F(0.5))));
      Self.C("draw_circle", c, (r.G("size").G("y") * Gv.F(0.32)), Gd.Ctor("Color", Gv.S("2b3138")));
      Self.C("draw_arc", c, (r.G("size").G("y") * Gv.F(0.32)), Gv.I(0L), Gd.K("TAU"), Gv.I(32L), Gd.Ctor("Color", Gv.S("8a939b")), Gv.F(3.0), Gv.B(true));
      foreach (Gv b in (Gv.I(4L)).Iter()) {
        Gv a = (((Gd.K("TAU") * b) / Gv.F(4.0)) + Gv.F(0.4));
        Self.C("draw_line", c, (c + ((Gd.Ctor("Vector2", Gd.cos(a), Gd.sin(a)) * r.G("size").G("y")) * Gv.F(0.28))), Gd.Ctor("Color", Gv.S("aab2ba")), Gv.F(6.0), Gv.B(true));
      }
    }
    S_art.nameplate(Self, m.G("font_b"), it.G("label"), (r.G("position").G("x") + (r.G("size").G("x") * Gv.F(0.5))), (r.G("position").G("y") - Gv.I(34L)), Gv.I(14L));
    return Gv.Null;
  }
  public Gv _malt_gate(Gv it = default(Gv)) {
    Gv is_open = m.G("sim").G("S").G("malt").C("get", Gv.S("gate"), Gv.B(false));
    Gv g = m.C("malt_gate_pos", it);
    Gv gcol = ((is_open).T() ? Gd.Ctor("Color", Gv.S("2fbf5b")) : Gd.Ctor("Color", Gv.S("e0412f")));
    Self.C("draw_circle", g, Gv.F(20.0), Gd.Ctor("Color", gcol, Gv.F(0.22)));
    S_art.rrect(Self, Gd.Ctor("Rect2", (g.G("x") - Gv.I(22L)), (g.G("y") - Gv.I(7L)), Gv.I(44L), Gv.I(14L)), Gd.Ctor("Color", Gv.S("8e98a2")), Gv.F(3.0));
    Gv goff = (!(is_open).T() ? Gv.F(0.0) : Gv.F(30.0));
    Self.C("draw_rect", Gd.Ctor("Rect2", ((g.G("x") - Gv.I(15L)) - goff), (g.G("y") - Gv.I(4L)), Gv.I(30L), Gv.I(8L)), gcol);
    Self.C("draw_rect", Gd.Ctor("Rect2", ((g.G("x") - Gv.I(15L)) - goff), (g.G("y") - Gv.I(4L)), Gv.I(30L), Gv.I(8L)), Gd.Ctor("Color", Gv.S("0d1013")), Gv.B(false), Gv.F(2.0));
    Self.C("draw_line", Gd.Ctor("Vector2", ((g.G("x") - Gv.I(15L)) - goff), g.G("y")), Gd.Ctor("Vector2", ((g.G("x") - Gv.I(25L)) - goff), g.G("y")), Gd.Ctor("Color", Gv.S("2b3138")), Gv.F(3.0), Gv.B(true));
    Self.C("draw_circle", Gd.Ctor("Vector2", ((g.G("x") - Gv.I(28L)) - goff), g.G("y")), Gv.F(5.0), Gd.Ctor("Color", Gv.S("c7cfd6")));
    Self.C("draw_circle", Gd.Ctor("Vector2", ((g.G("x") - Gv.I(28L)) - goff), g.G("y")), Gv.F(5.0), Gd.Ctor("Color", Gv.S("0d1013")), Gv.B(false), Gv.F(1.5));
    S_art.pill(Self, m.G("font_b"), (Gv.S("šoupě: %s") % (((is_open).T() ? Gv.S("otevřeno") : Gv.S("zavřeno")))), (g + Gd.Ctor("Vector2", Gv.I(78L), Gv.I(0L))), Gv.I(11L), gcol);
    return Gv.Null;
  }
  public Gv _valve(Gv it = default(Gv)) {
    Gv p = Gd.Ctor("Vector2", it.G("x"), it.G("y"));
    Gv kind = it.C("get", Gv.S("kind"), Gv.S("klapka"));
    Gv open = Gv.B(Gv.Ne(it.C("get", Gv.S("open"), Gv.B(true)), Gv.B(false)));
    Gv rot = (it.C("get", Gv.S("_ang"), Gd.deg_to_rad(Gd.Dyn("float", it.C("get", Gv.S("rot"), Gv.I(0L)))))).AsF();
    Gv ax = Gd.Ctor("Vector2", Gd.cos(rot), Gd.sin(rot));
    Gv nx = Gd.Ctor("Vector2", (-ax.G("y")), ax.G("x"));
    Gv col = ((open).T() ? Gd.Ctor("Color", Gv.S("2fbf5b")) : Gd.Ctor("Color", Gv.S("e0412f")));
    Gv pct = Gv.I(100L);
    if (Gv.Eq(kind, Gv.S("skrt"))) {
      pct = Gd.Dyn("int", it.C("get", Gv.S("pct"), Gv.I(50L)));
      col = (Gv.Gt(pct, Gv.I(0L)) ? Gd.Ctor("Color", Gv.S("f39a2b")) : Gd.Ctor("Color", Gv.S("e0412f")));
    } else if (((it.C("has", Gv.S("part"))).T() && (open).T())) {
      pct = Gd.Dyn("int", it.G("part"));
      col = Gd.Ctor("Color", Gv.S("f2c230"));
    }
    if ((it.C("get", Gv.S("stuck"), Gv.B(false))).T()) {
      col = col.C("darkened", Gv.F(0.35));
    }
    Gv is_open = Gv.B(((open).T() && (Gv.Ne(kind, Gv.S("skrt")) || Gv.Ge(pct, Gv.I(50L)))));
    if ((S_art.sprite(Self, ((((m.G("hard")).T() ? Gv.S("valveh_") : Gv.S("valve_"))) + (((is_open).T() ? Gv.S("open") : Gv.S("closed")))), p, rot, Gv.F(0.1643), Gd.Ctor("Vector2", Gv.I(434L), Gv.I(630L)))).T()) {
      if (Gv.Eq(kind, Gv.S("belimo"))) {
        S_art.rrect(Self, Gd.Ctor("Rect2", ((p - (nx * Gv.I(42L))) - Gd.Ctor("Vector2", Gv.I(10L), Gv.I(8L))), Gd.Ctor("Vector2", Gv.I(20L), Gv.I(16L))), Gd.Ctor("Color", Gv.S("f39a2b")), Gv.F(3.0));
      }
      if ((Gv.Eq(kind, Gv.S("skrt")) || ((Gv.Ne(kind, Gv.S("skrt")) && (it.C("has", Gv.S("part"))).T()) && (open).T()))) {
        S_art.pill(Self, m.G("font_b"), (Gv.Gt(pct, Gv.I(0L)) ? ((Gv.S("%d %%") % pct)) : Gv.S("zavřeno")), ((p + (nx * Gv.I(30L))) + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(6L))), Gv.I(12L), (Gv.Eq(kind, Gv.S("skrt")) ? Gd.Ctor("Color", Gv.S("f39a2b")) : Gd.Ctor("Color", Gv.S("f2c230"))));
      }
      if (((m.G("hl").C("has", it.G("id"))).T() && !(open).T())) {
        Self.C("draw_arc", p, Gv.F(25.0), Gv.I(0L), Gd.K("TAU"), Gv.I(28L), Gd.Ctor("Color", Gv.S("f2c230")), Gv.F(4.0), Gv.B(true));
      }
      return Gv.Null;
    }
    Gv t1 = Gd.Ctor("PackedVector2Array", Gd.Arr(((p - (ax * Gv.I(13L))) + (nx * Gv.I(9L))), ((p - (ax * Gv.I(13L))) - (nx * Gv.I(9L))), p));
    Gv t2 = Gd.Ctor("PackedVector2Array", Gd.Arr(((p + (ax * Gv.I(13L))) + (nx * Gv.I(9L))), ((p + (ax * Gv.I(13L))) - (nx * Gv.I(9L))), p));
    Self.C("draw_colored_polygon", t1, col);
    Self.C("draw_colored_polygon", t2, col);
    Gv o1 = t1.C("duplicate");
    o1.C("append", t1.I(Gv.I(0L)));
    Gv o2 = t2.C("duplicate");
    o2.C("append", t2.I(Gv.I(0L)));
    Self.C("draw_polyline", o1, Gd.Ctor("Color", Gv.S("0d1013")), Gv.F(2.0), Gv.B(true));
    Self.C("draw_polyline", o2, Gd.Ctor("Color", Gv.S("0d1013")), Gv.F(2.0), Gv.B(true));
    Self.C("draw_line", p, (p - (nx * Gv.I(14L))), Gd.Ctor("Color", Gv.S("2b3138")), Gv.F(3.0), Gv.B(true));
    if (Gv.Eq(kind, Gv.S("kulovy"))) {
      Gv la = (((open).T() ? ax : (nx * Gv.F(-1.0))));
      Self.C("draw_line", (p - (nx * Gv.I(14L))), ((p - (nx * Gv.I(14L))) + (la * Gv.I(18L))), Gd.Ctor("Color", Gv.S("e0412f")), Gv.F(5.0), Gv.B(true));
    } else if (Gv.Eq(kind, Gv.S("belimo"))) {
      S_art.rrect(Self, Gd.Ctor("Rect2", ((p - (nx * Gv.I(26L))) - Gd.Ctor("Vector2", Gv.I(10L), Gv.I(8L))), Gd.Ctor("Vector2", Gv.I(20L), Gv.I(16L))), Gd.Ctor("Color", Gv.S("f39a2b")), Gv.F(3.0));
    } else if (Gv.Eq(kind, Gv.S("skrt"))) {
      Gv ang = ((rot - (Gd.K("PI") * Gv.F(0.5))) + (((Gd.K("PI") * Gv.F(0.5)) * pct) / Gv.F(100.0)));
      Self.C("draw_line", (p - (nx * Gv.I(14L))), ((p - (nx * Gv.I(14L))) + (Gd.Ctor("Vector2", Gd.cos(ang), Gd.sin(ang)) * Gv.I(18L))), Gd.Ctor("Color", Gv.S("f39a2b")), Gv.F(5.0), Gv.B(true));
      S_art.pill(Self, m.G("font_b"), (Gv.Gt(pct, Gv.I(0L)) ? ((Gv.S("%d %%") % pct)) : Gv.S("zavřeno")), ((p + (nx * Gv.I(30L))) + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(6L))), Gv.I(12L), Gd.Ctor("Color", Gv.S("f39a2b")));
    } else {
      Self.C("draw_circle", (p - (nx * Gv.I(14L))), Gv.F(4.0), Gd.Ctor("Color", Gv.S("c7cfd6")));
    }
    if (((Gv.Ne(kind, Gv.S("skrt")) && (it.C("has", Gv.S("part"))).T()) && (open).T())) {
      S_art.pill(Self, m.G("font_b"), (Gv.S("%d %%") % pct), ((p + (nx * Gv.I(30L))) + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(6L))), Gv.I(12L), Gd.Ctor("Color", Gv.S("f2c230")));
    }
    Self.C("draw_circle", p, Gv.F(3.0), Gd.Ctor("Color", Gv.S("0d1013")));
    if (((m.G("hl").C("has", it.G("id"))).T() && !(open).T())) {
      Self.C("draw_arc", p, Gv.F(25.0), Gv.I(0L), Gd.K("TAU"), Gv.I(28L), Gd.Ctor("Color", Gv.S("f2c230")), Gv.F(4.0), Gv.B(true));
    }
    return Gv.Null;
  }
  public Gv _hose_vis(Gv it = default(Gv)) {
    Gv c = Gd.New("Curve2D");
    Gv hp = Gd.Arr();
    foreach (Gv q in (it.G("pts")).Iter()) {
      hp.C("append", Gd.Ctor("Vector2", q.I(Gv.I(0L)), q.I(Gv.I(1L))));
    }
    if (Gv.Eq(hp.C("size"), Gv.I(2L))) {
      Gv a0 = hp.I(Gv.I(0L));
      Gv b0 = hp.I(Gv.I(1L));
      Gv dd = (b0 - a0);
      Gv nn0 = Gd.Ctor("Vector2", (-dd.G("y")), dd.G("x")).C("normalized");
      Gv sg = ((Gv.Eq((Gd.Dyn("int", a0.G("x")) % Gv.I(2L)), Gv.I(0L)) ? Gv.F(1.0) : Gv.F(-1.0))).AsF();
      hp = Gd.Arr(a0, ((a0 + (dd * Gv.F(0.33))) + ((nn0 * Gv.F(5.0)) * sg)), ((a0 + (dd * Gv.F(0.67))) - ((nn0 * Gv.F(5.0)) * sg)), b0);
    }
    Gv n = (hp.C("size")).AsI();
    foreach (Gv i in (n).Iter()) {
      Gv p = hp.I(i);
      Gv t = Gd.StaticGet("Vector2", "ZERO");
      if ((Gv.Gt(i, Gv.I(0L)) && Gv.Lt(i, (n - Gv.I(1L))))) {
        t = (((hp.I((i + Gv.I(1L))) - hp.I((i - Gv.I(1L))))) * Gv.F(0.22));
      }
      c.C("add_point", p, (-t), t);
    }
    Gv pts = c.C("tessellate", Gv.I(4L), Gv.F(3.0));
    Self.C("draw_polyline", pts, Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.16)), Gv.F(16.0), Gv.B(true));
    Self.C("draw_polyline", pts, Gd.Ctor("Color", Gv.S("3a434b")), Gv.F(12.0), Gv.B(true));
    Self.C("draw_polyline", pts, Gd.Ctor("Color", Gv.S("aeb7bf")), Gv.F(9.5), Gv.B(true));
    Self.C("draw_polyline", pts, Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.55)), Gv.F(2.6), Gv.B(true));
    foreach (Gv i_2 in Gd.RangeIter(Gv.I(1L), pts.C("size"))) {
      Gv d = (pts.I(i_2) - pts.I((i_2 - Gv.I(1L))));
      Gv l = d.C("length");
      if (Gv.Lt(l, Gv.F(0.01))) {
        continue;
      }
      Gv u = (d / l);
      Gv nn = Gd.Ctor("Vector2", (-u.G("y")), u.G("x"));
      Gv q_2 = Gv.F(0.0);
      while (Gv.Lt(q_2, l)) {
        Gv pp = (pts.I((i_2 - Gv.I(1L))) + (u * q_2));
        Self.C("draw_line", ((pp - (nn * Gv.F(4.6))) - (u * Gv.F(2.2))), ((pp + (nn * Gv.F(4.6))) + (u * Gv.F(2.2))), Gd.Ctor("Color", Gv.F(0.12), Gv.F(0.15), Gv.F(0.18), Gv.F(0.55)), Gv.F(1.1));
        Self.C("draw_line", ((pp - (nn * Gv.F(4.6))) + (u * Gv.F(2.2))), ((pp + (nn * Gv.F(4.6))) - (u * Gv.F(2.2))), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.5)), Gv.F(1.1));
        q_2 = (q_2 + Gv.F(2.6));
      }
    }
    Self.C("draw_polyline", pts, Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.35)), Gv.F(1.6), Gv.B(true));
    foreach (Gv k in (Gv.I(2L)).Iter()) {
      Gv ee = (Gv.Eq(k, Gv.I(0L)) ? hp.I(Gv.I(0L)) : hp.I((n - Gv.I(1L))));
      Gv nb = (Gv.Eq(k, Gv.I(0L)) ? pts.I(Gd.mini(Gv.I(3L), (pts.C("size") - Gv.I(1L)))) : pts.I(Gd.maxi(Gv.I(0L), (pts.C("size") - Gv.I(4L)))));
      Gv dr = ((nb - ee)).C("normalized");
      Gv nr = Gd.Ctor("Vector2", (-dr.G("y")), dr.G("x"));
      Gv e2 = (ee + (dr * Gv.F(9.0)));
      Self.C("draw_line", ee, e2, Gd.Ctor("Color", Gv.S("3a434b")), Gv.F(14.0));
      Self.C("draw_line", ee, e2, Gd.Ctor("Color", Gv.S("c3ccd3")), Gv.F(11.5));
      Self.C("draw_line", (ee + (nr * Gv.F(3.0))), (e2 + (nr * Gv.F(3.0))), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.7)), Gv.F(2.0));
      Self.C("draw_line", ((ee + (dr * Gv.F(3.0))) - (nr * Gv.F(6.0))), ((ee + (dr * Gv.F(3.0))) + (nr * Gv.F(6.0))), Gd.Ctor("Color", Gv.S("5a646c")), Gv.F(1.2));
      Self.C("draw_line", ((ee + (dr * Gv.F(6.0))) - (nr * Gv.F(6.0))), ((ee + (dr * Gv.F(6.0))) + (nr * Gv.F(6.0))), Gd.Ctor("Color", Gv.S("5a646c")), Gv.F(1.2));
    }
    return Gv.Null;
  }
  public Gv _hose(Gv it = default(Gv)) {
    Gv c = Gd.New("Curve2D");
    Gv n = (it.G("pts").C("size")).AsI();
    foreach (Gv i in (n).Iter()) {
      Gv p = Gd.Ctor("Vector2", it.G("pts").I(i).I(Gv.I(0L)), it.G("pts").I(i).I(Gv.I(1L)));
      Gv t = Gd.StaticGet("Vector2", "ZERO");
      if ((Gv.Gt(i, Gv.I(0L)) && Gv.Lt(i, (n - Gv.I(1L))))) {
        t = (((Gd.Ctor("Vector2", it.G("pts").I((i + Gv.I(1L))).I(Gv.I(0L)), it.G("pts").I((i + Gv.I(1L))).I(Gv.I(1L))) - Gd.Ctor("Vector2", it.G("pts").I((i - Gv.I(1L))).I(Gv.I(0L)), it.G("pts").I((i - Gv.I(1L))).I(Gv.I(1L))))) * Gv.F(0.22));
      }
      c.C("add_point", p, (-t), t);
    }
    Gv pts = c.C("tessellate", Gv.I(4L), Gv.F(3.0));
    Gv w = (Gv.Le(Gd.Dyn("float", it.C("get", Gv.S("dn"), Gv.I(50L))), Gv.I(40L)) ? Gv.F(11.0) : Gv.F(13.0));
    Self.C("draw_polyline", pts, Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.18)), (w + Gv.F(10.0)), Gv.B(true));
    Self.C("draw_polyline", pts, Gd.Ctor("Color", Gv.S("3a0c0a")), (w + Gv.F(4.0)), Gv.B(true));
    Self.C("draw_polyline", pts, Gd.Ctor("Color", Gv.S("b3312a")), w, Gv.B(true));
    Self.C("draw_polyline", pts, Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.22)), Gd.maxf(Gv.F(1.5), (w * Gv.F(0.25))), Gv.B(true));
    foreach (Gv idx in (Gd.Arr(Gv.I(0L), (n - Gv.I(1L)))).Iter()) {
      Gv e = Gd.Ctor("Vector2", it.G("pts").I(idx).I(Gv.I(0L)), it.G("pts").I(idx).I(Gv.I(1L)));
      Gv on = Gv.B(Gv.Ne(m.C("outlet_at", e, Gv.F(2.0)), Gv.Null));
      if (!(on).T()) {
        on = m.G("pipe_pts").C("has", Gd.Ctor("Vector2i", e.C("round")));
        if (!(on).T()) {
          Self.C("draw_arc", e, Gv.F(16.0), Gv.I(0L), Gd.K("TAU"), Gv.I(24L), Gd.Ctor("Color", Gv.S("e0412f")), Gv.F(3.0), Gv.B(true));
        }
      }
      Self.C("draw_circle", e, Gv.F(9.0), Gd.Ctor("Color", Gv.S("6b4a10")));
      Self.C("draw_circle", e, Gv.F(7.0), Gd.Ctor("Color", Gv.S("d8a13a")));
    }
    return Gv.Null;
  }
  public Gv _outlet(Gv o = default(Gv)) {
    Gv p = o.G("front");
    Gv open = m.G("outlet_open").C("get", o.G("key"), Gv.B(false));
    Gv col = ((open).T() ? Gd.Ctor("Color", Gv.S("2fbf5b")) : Gd.Ctor("Color", Gv.S("e0412f")));
    Self.C("draw_circle", p, Gv.F(13.0), Gd.Ctor("Color", Gv.S("0d1013")));
    Self.C("draw_circle", p, Gv.F(11.0), S_art.grad(S_art.STAINLESS, Gv.F(0.35)));
    Self.C("draw_arc", p, Gv.F(11.0), Gv.I(0L), Gd.K("TAU"), Gv.I(24L), col, Gv.F(3.0), Gv.B(true));
    Gv la = ((open).T() ? Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-1L)) : Gd.Ctor("Vector2", Gv.I(1L), Gv.I(0L)));
    Self.C("draw_line", p, (p + (la * Gv.F(22.0))), Gd.Ctor("Color", Gv.S("0d1013")), Gv.F(7.0), Gv.B(true));
    Self.C("draw_line", p, (p + (la * Gv.F(22.0))), col, Gv.F(4.5), Gv.B(true));
    Self.C("draw_circle", p, Gv.F(4.0), Gd.Ctor("Color", Gv.S("2b3138")));
    if (((m.G("hl").C("has", o.G("key"))).T() && !(open).T())) {
      Self.C("draw_arc", p, Gv.F(22.0), Gv.I(0L), Gd.K("TAU"), Gv.I(28L), Gd.Ctor("Color", Gv.S("f2c230")), Gv.F(4.0), Gv.B(true));
    }
    return Gv.Null;
  }
  public Gv _pump(Gv it = default(Gv)) {
    Gv p = Gd.Ctor("Vector2", it.G("x"), it.G("y"));
    Gv a = Gd.deg_to_rad(Gd.Dyn("float", it.C("get", Gv.S("dir"), Gv.I(0L))));
    if ((S_art.sprite(Self, Gv.S("pump_inline"), p, Gv.F(0.0), Gv.F(0.1214), Gd.Ctor("Vector2", Gv.I(700L), Gv.I(1056L)))).T()) {
      Gv f2 = Gd.Ctor("Vector2", Gd.cos(a), Gd.sin(a));
      Self.C("draw_colored_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(((p + (f2 * Gv.I(30L))) + (Gd.Ctor("Vector2", (-f2.G("y")), f2.G("x")) * Gv.I(5L))), (p + (f2 * Gv.I(38L))), ((p + (f2 * Gv.I(30L))) - (Gd.Ctor("Vector2", (-f2.G("y")), f2.G("x")) * Gv.I(5L))))), Gd.Ctor("Color", Gv.F(0.95), Gv.F(0.76), Gv.F(0.19), Gv.F(0.9)));
      S_art.pill(Self, m.G("font_b"), it.C("get", Gv.S("label"), Gv.S("")), (p + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(48L))), Gv.I(13L), Gd.Ctor("Color", Gv.S("f2c230")));
      return Gv.Null;
    }
    Self.C("draw_set_transform", p, a, Gd.StaticGet("Vector2", "ONE"));
    S_art.cyl_h(Self, Gd.Ctor("Rect2", Gv.I(-26L), Gv.I(-5L), Gv.I(52L), Gv.I(10L)), S_art.STAINLESS, Gv.I(8L));
    Self.C("draw_rect", Gd.Ctor("Rect2", Gv.I(-26L), Gv.I(-5L), Gv.I(52L), Gv.I(10L)), Gd.Ctor("Color", Gv.S("1e2429")), Gv.B(false), Gv.F(1.0));
    foreach (Gv fx in (Gd.Arr(Gv.F(-27.0), Gv.F(21.0))).Iter()) {
      S_art.cyl_h(Self, Gd.Ctor("Rect2", fx, Gv.I(-8L), Gv.I(6L), Gv.I(16L)), S_art.STAINLESS, Gv.I(6L));
      Self.C("draw_rect", Gd.Ctor("Rect2", fx, Gv.I(-8L), Gv.I(6L), Gv.I(16L)), Gd.Ctor("Color", Gv.S("1e2429")), Gv.B(false), Gv.F(1.0));
    }
    Self.C("draw_set_transform", Gd.StaticGet("Vector2", "ZERO"), Gv.I(0L), Gd.StaticGet("Vector2", "ONE"));
    Gv mc = (p + Gd.Ctor("Vector2", Gv.I(2L), Gv.I(-31L)));
    S_art.cyl_v(Self, Gd.Ctor("Rect2", (p.G("x") - Gv.I(7L)), (p.G("y") - Gv.I(21L)), Gv.I(14L), Gv.I(10L)), S_art.STAINLESS, Gv.I(6L));
    Self.C("draw_line", Gd.Ctor("Vector2", (p.G("x") - Gv.I(7L)), (p.G("y") - Gv.I(16L))), Gd.Ctor("Vector2", (p.G("x") + Gv.I(7L)), (p.G("y") - Gv.I(16L))), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.35)), Gv.F(1.0));
    Gv body = Gd.Ctor("Rect2", (mc.G("x") - Gv.I(21L)), (mc.G("y") - Gv.I(13L)), Gv.I(44L), Gv.I(26L));
    S_art.cyl_h(Self, body, S_art.NAVY, Gv.I(12L));
    Gv fx2 = (body.G("position").G("x") + Gv.F(4.0));
    while (Gv.Lt(fx2, (body.G("end").G("x") - Gv.F(2.0)))) {
      Self.C("draw_line", Gd.Ctor("Vector2", fx2, (body.G("position").G("y") + Gv.I(2L))), Gd.Ctor("Vector2", fx2, (body.G("end").G("y") - Gv.I(2L))), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.28)), Gv.F(1.5));
      Self.C("draw_line", Gd.Ctor("Vector2", (fx2 + Gv.F(1.5)), (body.G("position").G("y") + Gv.I(2L))), Gd.Ctor("Vector2", (fx2 + Gv.F(1.5)), (body.G("end").G("y") - Gv.I(2L))), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.1)), Gv.F(1.0));
      fx2 = (fx2 + Gv.F(5.0));
    }
    Self.C("draw_rect", body, Gd.Ctor("Color", Gv.S("161b20")), Gv.B(false), Gv.F(1.5));
    Gv fan = Gd.Ctor("Rect2", (body.G("position").G("x") - Gv.I(11L)), (mc.G("y") - Gv.I(11L)), Gv.I(12L), Gv.I(22L));
    S_art.cyl_h(Self, fan, S_art.NAVY, Gv.I(8L));
    Self.C("draw_rect", fan, Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.25)));
    foreach (Gv gy in (Gd.Arr(Gv.F(-7.0), Gv.F(-3.5), Gv.F(0.0), Gv.F(3.5), Gv.F(7.0))).Iter()) {
      Self.C("draw_line", Gd.Ctor("Vector2", (fan.G("position").G("x") + Gv.I(2L)), (mc.G("y") + gy)), Gd.Ctor("Vector2", (fan.G("end").G("x") - Gv.I(2L)), (mc.G("y") + gy)), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.45)), Gv.F(1.0));
    }
    Self.C("draw_rect", fan, Gd.Ctor("Color", Gv.S("161b20")), Gv.B(false), Gv.F(1.5));
    Gv tb = Gd.Ctor("Rect2", (mc.G("x") - Gv.I(4L)), (body.G("position").G("y") - Gv.I(7L)), Gv.I(14L), Gv.I(8L));
    S_art.cyl_h(Self, tb, S_art.NAVY, Gv.I(4L));
    Self.C("draw_rect", tb, Gd.Ctor("Color", Gv.S("161b20")), Gv.B(false), Gv.F(1.0));
    Self.C("draw_rect", Gd.Ctor("Rect2", body.G("end").G("x"), (mc.G("y") - Gv.I(8L)), Gv.I(3L), Gv.I(16L)), Gd.Ctor("Color", Gv.S("39424a")));
    Gv pts = Gd.Ctor("PackedVector2Array");
    Gv cols = Gd.Ctor("PackedColorArray");
    foreach (Gv i in (Gv.I(33L)).Iter()) {
      Gv ang = ((Gd.K("TAU") * i) / Gv.F(32.0));
      Gv rr = (Gv.F(18.0) + (Gv.F(2.0) * ((Gd.Dyn("float", i) / Gv.F(32.0)))));
      pts.C("append", (p + (Gd.Ctor("Vector2", Gd.cos(ang), Gd.sin(ang)) * rr)));
      cols.C("append", S_art.grad(S_art.STAINLESS, (Gv.F(0.5) + (Gv.F(0.45) * Gd.cos((ang + Gv.F(2.3)))))));
    }
    Self.C("draw_polygon", pts, cols);
    Self.C("draw_polyline", pts, Gd.Ctor("Color", Gv.S("1e2429")), Gv.F(2.0), Gv.B(true));
    foreach (Gv k in (Gv.I(8L)).Iter()) {
      Gv ba = (((Gd.K("TAU") * k) / Gv.F(8.0)) + Gv.F(0.2));
      Self.C("draw_circle", (p + (Gd.Ctor("Vector2", Gd.cos(ba), Gd.sin(ba)) * Gv.F(14.5))), Gv.F(1.6), Gd.Ctor("Color", Gv.S("4a535b")));
    }
    Self.C("draw_circle", p, Gv.F(8.0), Gd.Ctor("Color", Gv.S("1e2429")));
    Self.C("draw_circle", p, Gv.F(6.8), Gd.Ctor("Color", Gv.S("c9d1d8")));
    Self.C("draw_circle", (p + Gd.Ctor("Vector2", Gv.F(-1.5), Gv.F(-1.5))), Gv.F(3.0), Gd.Ctor("Color", Gv.S("eef3f6")));
    Gv f = Gd.Ctor("Vector2", Gd.cos(a), Gd.sin(a));
    Gv n = Gd.Ctor("Vector2", (-f.G("y")), f.G("x"));
    Self.C("draw_colored_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(((p - (f * Gv.I(6L))) - (n * Gv.I(8L))), (p + (f * Gv.I(9L))), ((p - (f * Gv.I(6L))) + (n * Gv.I(8L))))), Gd.Ctor("Color", Gv.S("f2c230")));
    S_art.pill(Self, m.G("font_b"), it.C("get", Gv.S("label"), Gv.S("")), (p + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(48L))), Gv.I(13L), Gd.Ctor("Color", Gv.S("f2c230")));
    return Gv.Null;
  }
  public Gv _spray(Gv it = default(Gv)) {
    Gv p = Gd.Ctor("Vector2", it.G("x"), it.G("y"));
    Self.C("draw_circle", p, Gv.F(12.0), Gd.Ctor("Color", Gv.S("0d1013")));
    Self.C("draw_circle", p, Gv.F(10.5), Gd.Ctor("Color", Gv.S("c7cfd6")));
    foreach (Gv i in (Gv.I(8L)).Iter()) {
      Gv a = ((Gd.K("TAU") * i) / Gv.F(8.0));
      Self.C("draw_circle", (p + (Gd.Ctor("Vector2", Gd.cos(a), Gd.sin(a)) * Gv.F(6.0))), Gv.F(1.5), Gd.Ctor("Color", Gv.S("2b3138")));
    }
    Self.C("draw_circle", p, Gv.F(2.0), Gd.Ctor("Color", Gv.S("2b3138")));
    return Gv.Null;
  }
  public Gv _nozzle(Gv it = default(Gv)) {
    Gv p = Gd.Ctor("Vector2", it.G("x"), it.G("y"));
    Gv a = Gd.deg_to_rad(Gd.Dyn("float", it.C("get", Gv.S("dir"), Gv.I(0L))));
    Gv f = Gd.Ctor("Vector2", Gd.cos(a), Gd.sin(a));
    Gv n = Gd.Ctor("Vector2", (-f.G("y")), f.G("x"));
    Self.C("draw_colored_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr((p - (n * Gv.I(7L))), (p + (n * Gv.I(7L))), ((p + (f * Gv.I(18L))) + (n * Gv.I(3L))), ((p + (f * Gv.I(18L))) - (n * Gv.I(3L))))), Gd.Ctor("Color", Gv.S("c7cfd6")));
    Self.C("draw_polyline", Gd.Ctor("PackedVector2Array", Gd.Arr((p - (n * Gv.I(7L))), ((p + (f * Gv.I(18L))) - (n * Gv.I(3L))), ((p + (f * Gv.I(18L))) + (n * Gv.I(3L))), (p + (n * Gv.I(7L))), (p - (n * Gv.I(7L))))), Gd.Ctor("Color", Gv.S("0d1013")), Gv.F(1.5), Gv.B(true));
    return Gv.Null;
  }
  public Gv _panel(Gv it = default(Gv)) {
    Gv r = Gd.Ctor("Rect2", it.G("x"), it.G("y"), it.C("get", Gv.S("w"), Gv.I(260L)), it.C("get", Gv.S("h"), Gv.I(320L)));
    S_art.shadow(Self, r, Gd.Ctor("Vector2", Gv.I(16L), Gv.I(20L)), Gv.F(24.0));
    S_art.rrect_border(Self, r, Gd.Ctor("Color", Gv.S("b9c0c6")), Gd.Ctor("Color", Gv.S("59626a")), Gv.I(6L), Gv.F(10.0));
    Self.C("draw_line", Gd.Ctor("Vector2", (r.G("position").G("x") + (r.G("size").G("x") * Gv.F(0.5))), (r.G("position").G("y") + Gv.I(90L))), Gd.Ctor("Vector2", (r.G("position").G("x") + (r.G("size").G("x") * Gv.F(0.5))), (r.G("end").G("y") - Gv.I(20L))), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.18)), Gv.F(3.0));
    S_art.rrect(Self, Gd.Ctor("Rect2", (r.G("position").G("x") + Gv.I(30L)), (r.G("position").G("y") + Gv.I(24L)), (r.G("size").G("x") - Gv.I(60L)), Gv.I(56L)), Gd.Ctor("Color", Gv.S("1b2026")), Gv.F(6.0));
    S_art.text_c(Self, m.G("font_b"), it.C("get", Gv.S("label"), Gv.S("ROZVADĚČ")), Gd.Ctor("Vector2", (r.G("position").G("x") + (r.G("size").G("x") * Gv.F(0.5))), (r.G("position").G("y") + Gv.I(62L))), Gv.I(30L), Gd.Ctor("Color", Gv.S("f2c230")));
    Gv cols = Gd.Arr(Gd.Ctor("Color", Gv.S("2fbf5b")), Gd.Ctor("Color", Gv.S("e0412f")), Gd.Ctor("Color", Gv.S("f2c230")), Gd.Ctor("Color", Gv.S("2e7bd6")), Gd.Ctor("Color", Gv.S("eef1f4")), Gd.Ctor("Color", Gv.S("2fbf5b")));
    foreach (Gv row in (Gv.I(8L)).Iter()) {
      foreach (Gv col in (Gv.I(6L)).Iter()) {
        Gv c = (r.G("position") + Gd.Ctor("Vector2", (Gv.I(95L) + (col * Gv.I(140L))), (Gv.I(180L) + (row * Gv.I(185L)))));
        if ((Gv.Lt(c.G("x"), (r.G("end").G("x") - Gv.I(50L))) && Gv.Lt(c.G("y"), (r.G("end").G("y") - Gv.I(80L))))) {
          S_art.rrect_border(Self, Gd.Ctor("Rect2", (c - Gd.Ctor("Vector2", Gv.I(48L), Gv.I(50L))), Gd.Ctor("Vector2", Gv.I(96L), Gv.I(130L))), Gd.Ctor("Color", Gv.S("eceff1")), Gd.Ctor("Color", Gv.S("8a939b")), Gv.I(2L), Gv.F(6.0));
          Self.C("draw_rect", Gd.Ctor("Rect2", (c - Gd.Ctor("Vector2", Gv.I(36L), Gv.I(42L))), Gd.Ctor("Vector2", Gv.I(72L), Gv.I(18L))), Gd.Ctor("Color", Gv.S("1b2026")));
          Self.C("draw_circle", (c + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(30L))), Gv.F(26.0), Gd.Ctor("Color", Gv.S("2b3138")));
          Self.C("draw_circle", (c + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(30L))), Gv.F(21.0), cols.I((((row + col)) % cols.C("size"))));
          Self.C("draw_circle", (c + Gd.Ctor("Vector2", Gv.I(-7L), Gv.I(23L))), Gv.F(6.0), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.35)));
        }
      }
    }
    return Gv.Null;
  }
  public Gv _manifold(Gv it = default(Gv)) {
    Gv g = Gd.Dyn("float", it.C("get", Gv.S("gap"), Gv.I(60L)));
    Gv cols = Gd.Dyn("int", it.C("get", Gv.S("cols"), Gv.I(4L)));
    Gv rows = Gd.Dyn("int", it.C("get", Gv.S("rows"), Gv.I(3L)));
    Gv W = (cols * g);
    Gv H = (rows * g);
    Gv ctr = Gd.Ctor("Vector2", (it.G("x") + (W * Gv.F(0.5))), (it.G("y") + (H * Gv.F(0.5))));
    Gv rot = Gd.deg_to_rad(Gd.Dyn("float", it.C("get", Gv.S("rot"), Gv.I(0L))));
    Gv bw = ((Gd.absf(Gd.cos(rot)) * W) + (Gd.absf(Gd.sin(rot)) * H));
    Gv bh = ((Gd.absf(Gd.cos(rot)) * H) + (Gd.absf(Gd.sin(rot)) * W));
    S_art.shadow(Self, Gd.Ctor("Rect2", (ctr - (Gd.Ctor("Vector2", bw, bh) * Gv.F(0.5))), Gd.Ctor("Vector2", bw, bh)));
    Self.C("draw_set_transform", ctr, rot, Gd.StaticGet("Vector2", "ONE"));
    Gv r = Gd.Ctor("Rect2", ((-W) * Gv.F(0.5)), ((-H) * Gv.F(0.5)), W, H);
    S_art.rrect_border(Self, r.C("grow", Gv.I(6L)), Gd.Ctor("Color", Gv.S("1f252b")), Gd.Ctor("Color", Gv.S("6c757d")), Gv.I(2L), Gv.F(9.0));
    S_art.rrect(Self, r, Gd.Ctor("Color", Gv.S("c9d0d6")), Gv.F(6.0));
    foreach (Gv ry in (rows).Iter()) {
      foreach (Gv cx in (cols).Iter()) {
        Gv c = Gd.Ctor("Vector2", ((((-W) * Gv.F(0.5)) + (g * Gv.F(0.5))) + (cx * g)), ((((-H) * Gv.F(0.5)) + (g * Gv.F(0.5))) + (ry * g)));
        Self.C("draw_circle", c, Gv.F(21.0), Gd.Ctor("Color", Gv.S("0d1013")));
        Self.C("draw_circle", c, Gv.F(19.0), Gd.Ctor("Color", Gv.S("9aa3ab")));
        foreach (Gv b in (Gv.I(4L)).Iter()) {
          Gv a = (((Gd.K("TAU") * b) / Gv.F(4.0)) + (Gd.K("PI") * Gv.F(0.25)));
          Self.C("draw_circle", (c + (Gd.Ctor("Vector2", Gd.cos(a), Gd.sin(a)) * Gv.F(15.0))), Gv.F(2.2), Gd.Ctor("Color", Gv.S("3b434b")));
        }
        Self.C("draw_circle", c, Gv.F(10.5), Gd.Ctor("Color", Gv.S("15191d")));
      }
    }
    Self.C("draw_set_transform", Gd.StaticGet("Vector2", "ZERO"), Gv.I(0L), Gd.StaticGet("Vector2", "ONE"));
    Gv top = (ctr.G("y") - ((((Gd.absf(Gd.cos(rot)) * H) + (Gd.absf(Gd.sin(rot)) * W))) * Gv.F(0.5)));
    S_art.pill(Self, m.G("font_b"), it.G("label"), Gd.Ctor("Vector2", ctr.G("x"), (top - Gv.I(18L))), Gv.I(13L));
    return Gv.Null;
  }
  public Gv _gauge(Gv it = default(Gv)) {
    Gv G = m.C("gauge_geo", it);
    Gv x = (G.G("x")).AsF();
    Gv y1 = (G.G("y1")).AsF();
    Gv y2 = (G.G("y2")).AsF();
    Gv h = (y2 - y1);
    Gv tubeb = Gd.Ctor("Color", Gv.S("4b555e"));
    Gv dark = Gd.Ctor("Color", Gv.S("23282e"));
    if ((G.G("free")).T()) {
      Self.C("draw_line", Gd.Ctor("Vector2", x, (y1 - Gv.I(30L))), Gd.Ctor("Vector2", x, (y1 - Gv.I(6L))), Gd.Ctor("Color", Gv.S("8a939b")), Gv.F(5.0));
      Self.C("draw_line", Gd.Ctor("Vector2", x, (y2 + Gv.I(6L))), Gd.Ctor("Vector2", x, (y2 + Gv.I(30L))), Gd.Ctor("Color", Gv.S("8a939b")), Gv.F(5.0));
      S_art.rrect_border(Self, Gd.Ctor("Rect2", (x - Gv.I(6L)), (y1 - Gv.I(6L)), Gv.I(12L), (h + Gv.I(12L))), Gd.Ctor("Color", Gv.F(0.62), Gv.F(0.83), Gv.F(0.94), Gv.F(0.28)), tubeb, Gv.I(2L), Gv.F(5.0));
      Self.C("draw_circle", Gd.Ctor("Vector2", x, (y1 - Gv.I(30L))), Gv.F(4.5), Gd.Ctor("Color", Gv.S("b9c2ca")));
      Self.C("draw_circle", Gd.Ctor("Vector2", x, (y2 + Gv.I(30L))), Gv.F(4.5), Gd.Ctor("Color", Gv.S("b9c2ca")));
      Self.C("draw_arc", Gd.Ctor("Vector2", x, (y1 - Gv.I(30L))), Gv.F(4.5), Gv.I(0L), Gd.K("TAU"), Gv.I(14L), dark, Gv.F(1.5), Gv.B(true));
      Self.C("draw_arc", Gd.Ctor("Vector2", x, (y2 + Gv.I(30L))), Gv.F(4.5), Gv.I(0L), Gd.K("TAU"), Gv.I(14L), dark, Gv.F(1.5), Gv.B(true));
    } else {
      Gv cx = (G.G("cx")).AsF();
      foreach (Gv yy in (Gd.Arr(y1, y2)).Iter()) {
        Self.C("draw_line", Gd.Ctor("Vector2", cx, yy), Gd.Ctor("Vector2", x, yy), Gd.Ctor("Color", Gv.S("8a939b")), Gv.F(5.0));
      }
      S_art.rrect_border(Self, Gd.Ctor("Rect2", (x - Gv.I(6L)), (y1 - Gv.I(6L)), Gv.I(12L), (h + Gv.I(12L))), Gd.Ctor("Color", Gv.F(0.62), Gv.F(0.83), Gv.F(0.94), Gv.F(0.28)), tubeb, Gv.I(2L), Gv.F(5.0));
      S_art.rrect_border(Self, Gd.Ctor("Rect2", (x - Gv.I(9L)), (y1 - Gv.I(12L)), Gv.I(18L), Gv.I(7L)), S_art.grad(S_art.STAINLESS, Gv.F(0.4)), dark, Gv.I(1L), Gv.F(2.0));
      S_art.rrect_border(Self, Gd.Ctor("Rect2", (x - Gv.I(9L)), (y2 + Gv.I(5L)), Gv.I(18L), Gv.I(7L)), S_art.grad(S_art.STAINLESS, Gv.F(0.4)), dark, Gv.I(1L), Gv.F(2.0));
    }
    foreach (Gv i in Gd.RangeIter(Gv.I(1L), Gv.I(10L))) {
      Gv yy2 = ((y1 + ((h * i) / Gv.F(10.0)))).AsF();
      Self.C("draw_line", Gd.Ctor("Vector2", (x + Gv.I(6L)), yy2), Gd.Ctor("Vector2", (x + ((((i % Gv.I(5L))).T() ? Gv.I(10L) : Gv.I(14L)))), yy2), Gd.Ctor("Color", Gv.S("6c757d")), Gv.F(1.2));
    }
    return Gv.Null;
  }
  public Gv _gauge_cocks(Gv it = default(Gv)) {
    Gv G = m.C("gauge_geo", it);
    Gv dark = Gd.Ctor("Color", Gv.S("23282e"));
    Gv vt = Gv.B(Gv.Ne(it.C("get", Gv.S("top"), Gv.B(true)), Gv.B(false)));
    Gv vb = Gv.B(Gv.Ne(it.C("get", Gv.S("bot"), Gv.B(true)), Gv.B(false)));
    if ((G.G("free")).T()) {
      foreach (Gv pair in (Gd.Arr(Gd.Arr((G.G("y1") - Gv.F(18.0)), vt), Gd.Arr((G.G("y2") + Gv.F(18.0)), vb))).Iter()) {
        Gv yy = (pair.I(Gv.I(0L))).AsF();
        Gv o = pair.I(Gv.I(1L));
        Self.C("draw_circle", Gd.Ctor("Vector2", G.G("x"), yy), Gv.F(7.0), dark);
        Self.C("draw_circle", Gd.Ctor("Vector2", G.G("x"), yy), Gv.F(6.0), S_art.grad(S_art.STAINLESS, Gv.F(0.35)));
        S_art.rrect(Self, Gd.Ctor("Rect2", (G.G("x") - (((o).T() ? Gv.I(2L) : Gv.I(6L)))), (yy - (((o).T() ? Gv.I(6L) : Gv.I(2L)))), ((o).T() ? Gv.I(4L) : Gv.I(12L)), ((o).T() ? Gv.I(12L) : Gv.I(4L))), ((o).T() ? Gd.Ctor("Color", Gv.S("2fbf5b")) : Gd.Ctor("Color", Gv.S("d93a2c"))), Gv.F(2.0));
      }
    } else {
      Gv mx = ((((G.G("cx") + G.G("x"))) / Gv.F(2.0))).AsF();
      foreach (Gv pair_2 in (Gd.Arr(Gd.Arr(G.G("y1"), vt), Gd.Arr(G.G("y2"), vb))).Iter()) {
        Gv yy_2 = (pair_2.I(Gv.I(0L))).AsF();
        Gv o_2 = pair_2.I(Gv.I(1L));
        Self.C("draw_circle", Gd.Ctor("Vector2", mx, yy_2), Gv.F(7.0), dark);
        Self.C("draw_circle", Gd.Ctor("Vector2", mx, yy_2), Gv.F(6.0), S_art.grad(S_art.STAINLESS, Gv.F(0.35)));
        S_art.rrect(Self, Gd.Ctor("Rect2", (mx - Gv.I(2L)), (yy_2 - (((o_2).T() ? Gv.I(2L) : Gv.I(12L)))), ((o_2).T() ? Gv.I(12L) : Gv.I(4L)), ((o_2).T() ? Gv.I(4L) : Gv.I(12L))), ((o_2).T() ? Gd.Ctor("Color", Gv.S("2fbf5b")) : Gd.Ctor("Color", Gv.S("d93a2c"))), Gv.F(2.0));
      }
    }
    return Gv.Null;
  }
  public Gv _sw(Gv it = default(Gv)) {
    Gv p = Gd.Ctor("Vector2", it.G("x"), it.G("y"));
    Gv on = it.C("get", Gv.S("on"), Gv.B(false));
    Gv r = Gd.Ctor("Rect2", (p - Gd.Ctor("Vector2", Gv.I(26L), Gv.I(34L))), Gd.Ctor("Vector2", Gv.I(52L), Gv.I(68L)));
    S_art.shadow(Self, r, Gd.Ctor("Vector2", Gv.I(6L), Gv.I(8L)), Gv.F(10.0));
    S_art.rrect_border(Self, r, Gd.Ctor("Color", Gv.S("e3e6e9")), Gd.Ctor("Color", Gv.S("6c757d")), Gv.I(2L), Gv.F(6.0));
    Self.C("draw_circle", (p + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-8L))), Gv.F(12.0), Gd.Ctor("Color", Gv.S("2b3138")));
    Self.C("draw_line", (p + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-8L))), (p + Gd.Ctor("Vector2", ((on).T() ? Gv.I(8L) : Gv.I(-8L)), Gv.I(-22L))), Gd.Ctor("Color", Gv.S("e0412f")), Gv.F(5.0), Gv.B(true));
    Self.C("draw_circle", (p + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(20L))), Gv.F(7.0), ((on).T() ? Gd.Ctor("Color", Gv.S("2fbf5b")) : Gd.Ctor("Color", Gv.S("3a1a18"))));
    S_art.pill(Self, m.G("font"), it.C("get", Gv.S("label"), Gv.S("")), (p + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(56L))), Gv.I(11L));
    return Gv.Null;
  }
  public Gv _deco(Gv it = default(Gv)) {
    Gv p = Gd.Ctor("Vector2", it.G("x"), it.G("y"));
    {
      Gv _t1 = it.C("get", Gv.S("k"), Gv.S(""));
      if (Gv.Eq(_t1, Gv.S("conv"))) {
        Gv L = Gd.Dyn("float", it.C("get", Gv.S("len"), Gv.I(280L)));
        S_art.cyl_h(Self, Gd.Ctor("Rect2", p, Gd.Ctor("Vector2", L, Gv.I(22L))), S_art.STAINLESS, Gv.I(10L));
        Gv xx = (p.G("x") + Gv.I(8L));
        while (Gv.Lt(xx, ((p.G("x") + L) - Gv.I(8L)))) {
          Self.C("draw_line", Gd.Ctor("Vector2", xx, (p.G("y") + Gv.I(2L))), Gd.Ctor("Vector2", (xx + Gv.I(10L)), (p.G("y") + Gv.I(20L))), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.35)), Gv.F(2.0));
          xx = (xx + Gv.F(14.0));
        }
        Self.C("draw_rect", Gd.Ctor("Rect2", p, Gd.Ctor("Vector2", L, Gv.I(22L))), Gd.Ctor("Color", Gv.S("0d1013")), Gv.B(false), Gv.F(2.0));
        S_art.pill(Self, m.G("font"), it.C("get", Gv.S("label"), Gv.S("")), (p + Gd.Ctor("Vector2", (L * Gv.F(0.5)), Gv.I(36L))), Gv.I(11L));
        Gv hx = (m.C("malt_gate_pos", it).G("x")).AsF();
        Gv fun = Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", (hx - Gv.I(44L)), (p.G("y") - Gv.I(92L))), Gd.Ctor("Vector2", (hx + Gv.I(44L)), (p.G("y") - Gv.I(92L))), Gd.Ctor("Vector2", (hx + Gv.I(13L)), (p.G("y") - Gv.I(30L))), Gd.Ctor("Vector2", (hx - Gv.I(13L)), (p.G("y") - Gv.I(30L)))));
        Self.C("draw_colored_polygon", fun, Gd.Ctor("Color", Gv.S("c9d0d6")));
        Self.C("draw_colored_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", (hx - Gv.I(44L)), (p.G("y") - Gv.I(92L))), Gd.Ctor("Vector2", (hx - Gv.I(20L)), (p.G("y") - Gv.I(92L))), Gd.Ctor("Vector2", (hx - Gv.I(5L)), (p.G("y") - Gv.I(30L))), Gd.Ctor("Vector2", (hx - Gv.I(13L)), (p.G("y") - Gv.I(30L))))), Gd.Ctor("Color", Gv.S("eef1f4")));
        Self.C("draw_polyline", Gd.Ctor("PackedVector2Array", Gd.Arr(fun.I(Gv.I(0L)), fun.I(Gv.I(1L)), fun.I(Gv.I(2L)), fun.I(Gv.I(3L)), fun.I(Gv.I(0L)))), Gd.Ctor("Color", Gv.S("0d1013")), Gv.F(2.0), Gv.B(true));
        Self.C("draw_rect", Gd.Ctor("Rect2", (hx - Gv.I(48L)), (p.G("y") - Gv.I(98L)), Gv.I(96L), Gv.I(8L)), Gd.Ctor("Color", Gv.S("aab2ba")));
        Self.C("draw_rect", Gd.Ctor("Rect2", (hx - Gv.I(48L)), (p.G("y") - Gv.I(98L)), Gv.I(96L), Gv.I(8L)), Gd.Ctor("Color", Gv.S("0d1013")), Gv.B(false), Gv.F(2.0));
        Self.C("draw_rect", Gd.Ctor("Rect2", (hx - Gv.I(13L)), (p.G("y") - Gv.I(30L)), Gv.I(26L), Gv.I(30L)), Gd.Ctor("Color", Gv.S("b4bcc4")));
        Self.C("draw_rect", Gd.Ctor("Rect2", (hx - Gv.I(13L)), (p.G("y") - Gv.I(30L)), Gv.I(26L), Gv.I(30L)), Gd.Ctor("Color", Gv.S("0d1013")), Gv.B(false), Gv.F(2.0));
        S_art.pill(Self, m.G("font"), Gv.S("násypka"), Gd.Ctor("Vector2", hx, (p.G("y") - Gv.I(112L))), Gv.I(11L));
      }
      else if (Gv.Eq(_t1, Gv.S("vent"))) {
        S_art.cyl_v(Self, Gd.Ctor("Rect2", (p - Gd.Ctor("Vector2", Gv.I(15L), Gv.I(70L))), Gd.Ctor("Vector2", Gv.I(30L), Gv.I(70L))), S_art.STAINLESS, Gv.I(8L));
        Self.C("draw_rect", Gd.Ctor("Rect2", (p - Gd.Ctor("Vector2", Gv.I(15L), Gv.I(70L))), Gd.Ctor("Vector2", Gv.I(30L), Gv.I(70L))), Gd.Ctor("Color", Gv.S("0d1013")), Gv.B(false), Gv.F(2.0));
      }
      else if (Gv.Eq(_t1, Gv.S("hatch"))) {
        S_art.rrect(Self, Gd.Ctor("Rect2", (p - Gd.Ctor("Vector2", Gv.I(36L), Gv.I(-2L))), Gd.Ctor("Vector2", Gv.I(72L), Gv.I(22L))), Gd.Ctor("Color", Gv.S("2b3138")), Gv.F(4.0));
      }
      else if (Gv.Eq(_t1, Gv.S("splid"))) {
        ;
      }
      else if (true) {
        Self.C("draw_circle", p, Gv.F(10.0), Gd.Ctor("Color", Gv.S("c7cfd6")));
      }
    }
    return Gv.Null;
  }
  public Gv _internals(Gv it = default(Gv)) {
    Gv b = it.C("get", Gv.S("bind"));
    Gv S = m.G("sim").G("S");
    Gv x = (it.G("x")).AsF();
    Gv w = (it.G("w")).AsF();
    Gv cx = ((x + (w * Gv.F(0.5)))).AsF();
    Gv steel = Gd.Ctor("Color", Gv.S("aab4bc"));
    Gv dk = Gd.Ctor("Color", Gv.S("3b444c"));
    if (Gv.Eq(b, Gv.S("rvp"))) {
      Gv top = ((it.G("y") - Gv.F(4.0))).AsF();
      Gv cc = (Gd.minf((it.G("h") * Gv.F(0.12)), Gv.F(40.0))).AsF();
      Gv bot = ((((it.G("y") + it.G("h")) - cc) - Gv.F(26.0))).AsF();
      Gv run = Gv.B(((S.G("rvp").G("stir")).T() && Gv.Gt(S.G("rvp").G("w"), Gv.I(200L))));
      S_art.rrect_border(Self, Gd.Ctor("Rect2", (cx - Gv.I(26L)), (top - Gv.I(78L)), Gv.I(52L), Gv.I(30L)), Gd.Ctor("Color", Gv.S("2457b8")), dk, Gv.I(2L), Gv.F(5.0));
      S_art.rrect_border(Self, Gd.Ctor("Rect2", (cx - Gv.I(14L)), (top - Gv.I(50L)), Gv.I(28L), Gv.I(34L)), Gd.Ctor("Color", Gv.S("8d979f")), dk, Gv.I(2L), Gv.F(3.0));
      foreach (Gv k in (Gv.I(4L)).Iter()) {
        Self.C("draw_line", Gd.Ctor("Vector2", ((cx - Gv.I(22L)) + (k * Gv.I(14L))), (top - Gv.I(76L))), Gd.Ctor("Vector2", ((cx - Gv.I(22L)) + (k * Gv.I(14L))), (top - Gv.I(52L))), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.3)), Gv.F(2.0));
      }
      S_art.cyl_v(Self, Gd.Ctor("Rect2", (cx - Gv.I(5L)), (top - Gv.I(16L)), Gv.I(10L), ((bot - top) + Gv.I(16L))), S_art.STAINLESS, Gv.I(6L));
      Gv a = (spin).AsF();
      Gv yb = ((bot + Gv.F(16.0))).AsF();
      Gv Rm = ((w * Gv.F(0.4))).AsF();
      Gv sd = Gd.Ctor("Color", Gv.S("6c7680"));
      Gv sdk = Gd.Ctor("Color", Gv.S("2c3238"));
      Self.C("draw_colored_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", (cx - Gv.I(26L)), (yb + Gv.I(14L))), Gd.Ctor("Vector2", (cx + Gv.I(26L)), (yb + Gv.I(14L))), Gd.Ctor("Vector2", (cx + Gv.I(18L)), (yb + Gv.I(4L))), Gd.Ctor("Vector2", (cx - Gv.I(18L)), (yb + Gv.I(4L))))), Gd.Ctor("Color", Gv.S("8d979f")));
      Self.C("draw_polyline", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", (cx - Gv.I(26L)), (yb + Gv.I(14L))), Gd.Ctor("Vector2", (cx + Gv.I(26L)), (yb + Gv.I(14L))), Gd.Ctor("Vector2", (cx + Gv.I(18L)), (yb + Gv.I(4L))), Gd.Ctor("Vector2", (cx - Gv.I(18L)), (yb + Gv.I(4L))), Gd.Ctor("Vector2", (cx - Gv.I(26L)), (yb + Gv.I(14L))))), sdk, Gv.F(1.5), Gv.B(true));
      foreach (Gv k_2 in (Gv.I(3L)).Iter()) {
        Self.C("draw_circle", Gd.Ctor("Vector2", ((cx - Gv.I(16L)) + (k_2 * Gv.I(16L))), (yb + Gv.I(10L))), Gv.F(2.2), sdk);
      }
      foreach (Gv arm in (Gv.I(2L)).Iter()) {
        Gv aa = ((a + (arm * Gd.K("PI")))).AsF();
        Gv R = ((Gv.Eq(arm, Gv.I(0L)) ? Rm : (Rm * Gv.F(0.9)))).AsF();
        Gv cs = (Gd.cos(aa)).AsF();
        Gv sn = (Gd.sin(aa)).AsF();
        Gv back = Gv.B(Gv.Lt(sn, Gv.F(0.0)));
        Gv cl = sd.C("darkened", ((back).T() ? Gv.F(0.3) : Gv.F(0.0)));
        Gv xe = ((cx + (cs * R))).AsF();
        Gv xp = ((cx + ((cs * R) * Gv.F(0.42)))).AsF();
        Self.C("draw_line", Gd.Ctor("Vector2", cx, yb), Gd.Ctor("Vector2", xe, (yb + Gv.I(3L))), sdk, Gv.F(8.0), Gv.B(true));
        Self.C("draw_line", Gd.Ctor("Vector2", cx, yb), Gd.Ctor("Vector2", xe, (yb + Gv.I(3L))), cl, Gv.F(5.5), Gv.B(true));
        Gv ph = ((Gv.Eq(arm, Gv.I(0L)) ? Gv.F(30.0) : Gv.F(38.0))).AsF();
        Gv pl = Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", xp, (yb - (ph * Gv.F(0.45)))), Gd.Ctor("Vector2", xe, (yb - (ph * Gv.F(0.7)))), Gd.Ctor("Vector2", xe, (yb + (ph * Gv.F(0.3)))), Gd.Ctor("Vector2", xp, (yb + (ph * Gv.F(0.55))))));
        Self.C("draw_colored_polygon", pl, cl);
        Self.C("draw_polyline", Gd.Ctor("PackedVector2Array", Gd.Arr(pl.I(Gv.I(0L)), pl.I(Gv.I(1L)), pl.I(Gv.I(2L)), pl.I(Gv.I(3L)), pl.I(Gv.I(0L)))), sdk, Gv.F(1.3), Gv.B(true));
        foreach (Gv hi in (Gv.I(3L)).Iter()) {
          Gv f = ((Gv.F(0.35) + (hi * Gv.F(0.25)))).AsF();
          Gv hx = (Gd.lerpf(xp, xe, f)).AsF();
          Self.C("draw_circle", Gd.Ctor("Vector2", hx, (yb - (ph * Gv.F(0.1)))), (Gv.F(3.6) * ((Gv.F(0.45) + (Gv.F(0.55) * Gd.absf(sn))))), Gd.Ctor("Color", Gv.F(0.05), Gv.F(0.06), Gv.F(0.07), Gv.F(0.9)));
        }
      }
      Self.C("draw_circle", Gd.Ctor("Vector2", cx, yb), Gv.F(5.5), Gd.Ctor("Color", Gv.S("20262b")));
      Self.C("draw_circle", Gd.Ctor("Vector2", cx, yb), Gv.F(3.0), Gd.Ctor("Color", Gv.S("9aa4ad")));
      _cip_pipe(Gd.Ctor("Vector2", (x + (w * Gv.F(0.2))), (it.G("y") + Gv.F(8.0))));
    } else if (Gv.Eq(b, Gv.S("sk"))) {
      Gv sieve = ((((it.G("y") + it.G("h")) - Gv.F(3.0)) - Gv.F(22.0))).AsF();
      Gv rk = (S.G("sk").G("rakeH")).AsF();
      Gv ry = (((sieve - Gv.F(46.0)) - (((Gv.F(1.0) - rk)) * Gv.F(90.0)))).AsF();
      Gv run2 = S.G("sk").G("rake");
      Gv top2 = ((it.G("y") - Gv.F(4.0))).AsF();
      S_art.rrect_border(Self, Gd.Ctor("Rect2", (cx - Gv.I(30L)), (top2 - Gv.I(74L)), Gv.I(60L), Gv.I(34L)), Gd.Ctor("Color", Gv.S("2457b8")), dk, Gv.I(2L), Gv.F(5.0));
      S_art.rrect_border(Self, Gd.Ctor("Rect2", (cx - Gv.I(16L)), (top2 - Gv.I(42L)), Gv.I(32L), Gv.I(30L)), Gd.Ctor("Color", Gv.S("8d979f")), dk, Gv.I(2L), Gv.F(3.0));
      S_art.cyl_v(Self, Gd.Ctor("Rect2", (cx - Gv.I(6L)), (top2 - Gv.I(12L)), Gv.I(12L), ((ry - top2) + Gv.I(12L))), S_art.STAINLESS, Gv.I(6L));
      S_art.rrect_border(Self, Gd.Ctor("Rect2", (cx - Gv.I(9L)), (ry - Gv.I(14L)), Gv.I(18L), Gv.I(20L)), Gd.Ctor("Color", Gv.S("c3ccd3")), dk, Gv.I(2L), Gv.F(3.0));
      Gv half = (((w * Gv.F(0.5)) - Gv.F(26.0))).AsF();
      Gv ang = (spin).AsF();
      Gv bm = Gd.Ctor("Color", Gv.S("565f68"));
      Gv bmh = Gd.Ctor("Color", Gv.S("9aa4ad"));
      Gv bmd = Gd.Ctor("Color", Gv.S("1f252a"));
      S_art.cyl_h(Self, Gd.Ctor("Rect2", (cx - Gv.I(38L)), (ry - Gv.I(36L)), Gv.I(76L), Gv.I(12L)), S_art.STAINLESS, Gv.I(6L));
      Self.C("draw_rect", Gd.Ctor("Rect2", (cx - Gv.I(38L)), (ry - Gv.I(36L)), Gv.I(76L), Gv.I(12L)), bmd, Gv.B(false), Gv.F(1.2));
      S_art.rrect_border(Self, Gd.Ctor("Rect2", (cx - Gv.I(14L)), (ry - Gv.I(26L)), Gv.I(28L), Gv.I(28L)), Gd.Ctor("Color", Gv.S("39424a")), bmd, Gv.I(1L), Gv.F(3.0));
      foreach (Gv arm_2 in (Gv.I(2L)).Iter()) {
        Gv aa_2 = ((ang + (arm_2 * Gd.K("PI")))).AsF();
        Gv cs_2 = (Gd.cos(aa_2)).AsF();
        Gv sn_2 = (Gd.sin(aa_2)).AsF();
        Gv ex = ((cx + (cs_2 * half))).AsF();
        Gv front = Gv.B(Gv.Gt(sn_2, Gv.F(0.0)));
        Gv cc2 = ((front).T() ? bm : bm.C("darkened", Gv.F(0.3)));
        Self.C("draw_line", Gd.Ctor("Vector2", cx, (ry + Gv.I(2L))), Gd.Ctor("Vector2", ex, (ry + Gv.I(4L))), bmd, Gv.F(17.0), Gv.B(true));
        Self.C("draw_line", Gd.Ctor("Vector2", cx, (ry + Gv.I(2L))), Gd.Ctor("Vector2", ex, (ry + Gv.I(4L))), cc2, Gv.F(14.0), Gv.B(true));
        Self.C("draw_line", Gd.Ctor("Vector2", cx, (ry - Gv.I(3L))), Gd.Ctor("Vector2", ex, (ry - Gv.I(1L))), ((front).T() ? bmh : bmh.C("darkened", Gv.F(0.3))), Gv.F(3.0), Gv.B(true));
        Gv n = (Gd.Dyn("int", (half / Gv.F(46.0)))).AsI();
        foreach (Gv i in Gd.RangeIter(Gv.I(1L), n)) {
          Gv tx = ((cx + (cs_2 * (((half * i) / Gd.Dyn("float", n)))))).AsF();
          Gv wd = ((Gv.F(4.0) + (Gv.F(12.0) * Gd.absf(sn_2)))).AsF();
          Gv kl = (Gv.F(42.0)).AsF();
          Gv blade = Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", (tx - wd), (ry + Gv.I(12L))), Gd.Ctor("Vector2", (tx + wd), (ry + Gv.I(12L))), Gd.Ctor("Vector2", (tx + wd), (ry + kl))));
          Gv zz = Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", (tx - wd), (ry + Gv.I(12L)))));
          Gv steps = (Gv.I(4L)).AsI();
          foreach (Gv q in ((steps + Gv.I(1L))).Iter()) {
            Gv qx = (((tx - wd) + ((((Gv.F(2.0) * wd)) * q) / Gd.Dyn("float", steps)))).AsF();
            zz.C("append", Gd.Ctor("Vector2", qx, ((ry + kl) - ((Gv.Eq((q % Gv.I(2L)), Gv.I(1L)) ? Gv.F(7.0) : Gv.F(0.0))))));
          }
          zz.C("append", Gd.Ctor("Vector2", (tx + wd), (ry + Gv.I(12L))));
          Self.C("draw_colored_polygon", zz, cc2.C("lightened", Gv.F(0.08)));
          Self.C("draw_polyline", zz, bmd, Gv.F(1.1), Gv.B(true));
        }
        Gv sg = ((Gv.Ge(cs_2, Gv.F(0.0)) ? Gv.F(1.0) : Gv.F(-1.0))).AsF();
        Gv pw = ((Gv.F(8.0) + (Gv.F(26.0) * Gd.absf(sn_2)))).AsF();
        Gv pl_2 = Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", (ex - Gv.I(4L)), (ry + Gv.I(8L))), Gd.Ctor("Vector2", (ex + (Gv.I(4L) * sg)), (ry + Gv.I(8L))), Gd.Ctor("Vector2", (ex + (Gv.I(4L) * sg)), (ry + Gv.I(58L))), Gd.Ctor("Vector2", (ex - (sg * pw)), (ry + Gv.I(66L))), Gd.Ctor("Vector2", (ex - (sg * pw)), (ry + Gv.I(30L)))));
        Self.C("draw_colored_polygon", pl_2, cc2.C("darkened", Gv.F(0.05)));
        Self.C("draw_polyline", Gd.Ctor("PackedVector2Array", Gd.Arr(pl_2.I(Gv.I(0L)), pl_2.I(Gv.I(1L)), pl_2.I(Gv.I(2L)), pl_2.I(Gv.I(3L)), pl_2.I(Gv.I(4L)), pl_2.I(Gv.I(0L)))), bmd, Gv.F(1.4), Gv.B(true));
        foreach (Gv hi_2 in (Gv.I(2L)).Iter()) {
          Self.C("draw_circle", Gd.Ctor("Vector2", (ex - (sg * ((Gv.F(4.0) + ((pw * Gv.F(0.3)) * ((hi_2 + Gv.I(1L)))))))), ((ry + Gv.I(40L)) + (hi_2 * Gv.I(3L)))), (Gv.F(3.0) * ((Gv.F(0.4) + (Gv.F(0.6) * Gd.absf(sn_2))))), bmd);
        }
      }
      Self.C("draw_circle", Gd.Ctor("Vector2", cx, (ry + Gv.I(2L))), Gv.F(5.0), bmd);
      _cip_pipe(Gd.Ctor("Vector2", (x + (w * Gv.F(0.2))), (it.G("y") + Gv.F(8.0))));
    }
    return Gv.Null;
  }
  public Gv _cip_pipe(Gv p = default(Gv)) {
    Gv st = Gd.Ctor("Color", Gv.S("b9c2ca"));
    Gv d = Gd.Ctor("Color", Gv.S("1f252a"));
    Self.C("draw_line", p, (p + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(26L))), d, Gv.F(6.0));
    Self.C("draw_line", p, (p + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(26L))), st, Gv.F(4.0));
    Self.C("draw_arc", (p + Gd.Ctor("Vector2", Gv.I(10L), Gv.I(26L))), Gv.F(10.0), Gd.K("PI"), (Gd.K("PI") * Gv.F(0.5)), Gv.I(10L), d, Gv.F(6.0), Gv.B(true));
    Self.C("draw_arc", (p + Gd.Ctor("Vector2", Gv.I(10L), Gv.I(26L))), Gv.F(10.0), Gd.K("PI"), (Gd.K("PI") * Gv.F(0.5)), Gv.I(10L), st, Gv.F(4.0), Gv.B(true));
    Self.C("draw_line", (p + Gd.Ctor("Vector2", Gv.I(10L), Gv.I(36L))), (p + Gd.Ctor("Vector2", Gv.I(10L), Gv.I(44L))), d, Gv.F(6.0));
    Self.C("draw_line", (p + Gd.Ctor("Vector2", Gv.I(10L), Gv.I(36L))), (p + Gd.Ctor("Vector2", Gv.I(10L), Gv.I(44L))), st, Gv.F(4.0));
    Self.C("draw_circle", (p + Gd.Ctor("Vector2", Gv.I(10L), Gv.I(47L))), Gv.F(5.0), d);
    Self.C("draw_circle", (p + Gd.Ctor("Vector2", Gv.I(10L), Gv.I(47L))), Gv.F(3.8), Gd.Ctor("Color", Gv.S("dfe6ea")));
    return Gv.Null;
  }
  public Gv _rake(Gv it = default(Gv)) {
    return Gv.Null;
    Gv x = (it.G("x")).AsF();
    Gv y = (it.G("y")).AsF();
    Gv w = (it.C("get", Gv.S("w"), Gv.I(300L))).AsF();
    Gv h = (it.C("get", Gv.S("h"), Gv.I(280L))).AsF();
    S_art.cyl_v(Self, Gd.Ctor("Rect2", (x - Gv.I(6L)), y, Gv.I(12L), h), S_art.STAINLESS, Gv.I(6L));
    S_art.cyl_h(Self, Gd.Ctor("Rect2", (x - (w * Gv.F(0.5))), ((y + h) - Gv.I(36L)), w, Gv.I(10L)), S_art.STAINLESS, Gv.I(6L));
    Gv k = ((x - (w * Gv.F(0.5))) + Gv.I(12L));
    while (Gv.Lt(k, ((x + (w * Gv.F(0.5))) - Gv.I(6L)))) {
      Self.C("draw_colored_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", k, ((y + h) - Gv.I(26L))), Gd.Ctor("Vector2", (k + Gv.I(6L)), ((y + h) - Gv.I(26L))), Gd.Ctor("Vector2", (k + Gv.I(10L)), ((y + h) - Gv.I(8L))), Gd.Ctor("Vector2", (k - Gv.I(4L)), ((y + h) - Gv.I(8L))))), Gd.Ctor("Color", Gv.S("aab2ba")));
      k = (k + Gv.F(30.0));
    }
    S_art.rrect(Self, Gd.Ctor("Rect2", (x - Gv.I(22L)), (y - Gv.I(40L)), Gv.I(44L), Gv.I(36L)), Gd.Ctor("Color", Gv.S("2457b8")), Gv.F(5.0));
    return Gv.Null;
  }
  public Gv _dip(Gv it = default(Gv)) {
    Gv DG = m.C("dip_geo", it);
    Gv x = (DG.G("x")).AsF();
    Gv y = (DG.G("y")).AsF();
    Gv h = (DG.G("h")).AsF();
    Gv wood = Gd.Arr(Gd.Arr(Gv.F(0.0), Gv.S("7a5228")), Gd.Arr(Gv.F(0.35), Gv.S("d9b07a")), Gd.Arr(Gv.F(0.6), Gv.S("c69355")), Gd.Arr(Gv.F(1.0), Gv.S("6b4a22")));
    S_art.cyl_v(Self, Gd.Ctor("Rect2", (x - Gv.I(7L)), y, Gv.I(14L), h), wood, Gv.I(6L));
    Self.C("draw_rect", Gd.Ctor("Rect2", (x - Gv.I(7L)), y, Gv.I(14L), h), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.5)), Gv.B(false), Gv.F(1.0));
    foreach (Gv hl in Gd.RangeIter(Gv.I(4L), Gv.I(51L))) {
      Gv ty = ((y + h) - ((h * ((hl - Gv.I(3L)))) / Gv.F(47.0)));
      if (Gv.In(hl, Gd.Arr(Gv.I(10L), Gv.I(15L), Gv.I(19L), Gv.I(20L), Gv.I(21L), Gv.I(25L), Gv.I(30L)))) {
        Self.C("draw_line", Gd.Ctor("Vector2", (x - Gv.I(7L)), ty), Gd.Ctor("Vector2", (x + Gv.I(9L)), ty), Gd.Ctor("Color", Gv.S("c0261b")), Gv.F(2.5));
        Self.C("draw_string", m.G("font_b"), Gd.Ctor("Vector2", (x + Gv.I(11L)), (ty + Gv.I(5L))), Gd.Dyn("str", hl), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(13L), Gd.Ctor("Color", Gv.S("c0261b")));
      } else if (Gv.Eq(hl, Gv.I(40L))) {
        Self.C("draw_line", Gd.Ctor("Vector2", (x - Gv.I(7L)), ty), Gd.Ctor("Vector2", (x + Gv.I(9L)), ty), Gd.Ctor("Color", Gv.S("c0261b")), Gv.F(2.5));
        Self.C("draw_string", m.G("font_b"), Gd.Ctor("Vector2", (x + Gv.I(11L)), (ty + Gv.I(6L))), Gv.S("*"), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(18L), Gd.Ctor("Color", Gv.S("c0261b")));
      } else {
        Gv big = Gv.B(Gv.Eq((hl % Gv.I(5L)), Gv.I(0L)));
        Self.C("draw_line", Gd.Ctor("Vector2", (x - Gv.I(7L)), ty), Gd.Ctor("Vector2", ((x - Gv.I(7L)) + (((big).T() ? Gv.I(10L) : Gv.I(6L)))), ty), Gd.Ctor("Color", Gv.F(0.1), Gv.F(0.07), Gv.F(0.04), Gv.F(0.55)), Gv.F(1.0));
        if ((big).T()) {
          Self.C("draw_string", m.G("font"), Gd.Ctor("Vector2", (x + Gv.I(11L)), (ty + Gv.I(4L))), Gd.Dyn("str", hl), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(10L), Gd.Ctor("Color", Gv.F(0.1), Gv.F(0.07), Gv.F(0.04), Gv.F(0.55)));
        }
      }
    }
    return Gv.Null;
  }
  public Gv _gpu_body() {
    if (Gv.Eq(gpu_node, Gv.Null)) {
      gpu_node = S_liquidfx.StaticGet("Gpu").C("new");
      Self.C("add_child", gpu_node);
    }
    return gpu_node;
    return Gv.Null;
  }
  public Gv _gpu_clear() {
    if ((Gv.Ne(gpu_node, Gv.Null) && Gv.Gt(gpu_node.G("polys").C("size"), Gv.I(0L)))) {
      gpu_node.C("set_polys", Gd.Arr());
    }
    return Gv.Null;
  }
  public Gv _liquid(Gv it = default(Gv)) {
    Gv b = it.C("get", Gv.S("bind"));
    if (!Gv.In(b, Gd.Arr(Gv.S("rvp"), Gv.S("mp"), Gv.S("sk"), Gv.S("wp"), Gv.S("hlt"), Gv.S("lv"), Gv.S("spilka")))) {
      return Gv.Null;
    }
    Gv S = m.G("sim").G("S");
    Gv sim = m.G("sim");
    Gv outline = m.C("vessel_inside", it);
    Gv lv = (m.C("level_y", it)).AsF();
    Gv x = (it.G("x")).AsF();
    Gv w = (it.G("w")).AsF();
    Gv y0 = ((it.G("y") + it.G("h"))).AsF();
    Gv col = m.C("vessel_color", it);
    if ((Gv.Ge(lv, (y0 - Gv.F(0.5))) && Gv.Ne(b, Gv.S("sk")))) {
      _gpu_clear();
      return Gv.Null;
    }
    Gv tt = (Gd.Static("Time", "get_ticks_msec") / Gv.F(1000.0));
    Gv fxs = fx.C("get", it.G("id"), Gd.Dict());
    Gv dtf = (Gd.clampf((tt - Gd.Dyn("float", fxs.C("get", Gv.S("t"), tt))), Gv.F(0.0), Gv.F(0.25))).AsF();
    fxs = fxs.SI(Gv.S("t"), tt);
    Gv vort_px = Gv.F(0.0);
    if (Gv.Eq(b, Gv.S("rvp"))) {
      vort_px = ((S.G("rvp").G("vort") * ((it.G("h") - Gd.minf((it.G("h") * Gv.F(0.12)), Gv.F(40.0))))) / Gv.F(1.06));
    }
    Gv below = Gd.Ctor("PackedVector2Array");
    Gv N = Gv.I(24L);
    Gv vv = (Gv.In(b, Gd.Arr(Gv.S("rvp"), Gv.S("mp"), Gv.S("wp"))) ? S.C("get", b) : Gv.Null);
    Gv amp = Gv.F(0.0);
    if (Gv.Ne(vv, Gv.Null)) {
      amp = ((vv.G("boiling")).T() ? Gv.F(2.6) : (((vv.G("stir")).T() ? Gv.F(1.4) : Gv.F(0.0))));
    }
    Gv stir_t = Gv.F(0.0);
    if (Gv.Ne(vv, Gv.Null)) {
      stir_t = ((((vv.G("stir")).T() || (vv.G("boiling")).T()) || (vv.G("whirlNow")).T()) ? Gv.F(1.0) : Gv.F(0.0));
    } else if (Gv.Eq(b, Gv.S("sk"))) {
      stir_t = ((S.G("sk").G("rake")).T() ? Gv.F(1.0) : Gv.F(0.0));
    }
    Gv stirK = (Gd.Dyn("float", fxs.C("get", Gv.S("stir"), Gv.F(0.0)))).AsF();
    stirK = ((stirK + (((stir_t - stirK)) * Gd.minf(Gv.F(1.0), (dtf * ((Gv.Gt(stir_t, stirK) ? Gv.F(1.6) : Gv.F(0.45)))))))).AsF();
    fxs = fxs.SI(Gv.S("stir"), stirK);
    Gv lvp = (Gd.Dyn("float", fxs.C("get", Gv.S("lv"), lv))).AsF();
    Gv slosh = (((Gd.Dyn("float", fxs.C("get", Gv.S("slosh"), Gv.F(0.0))) * Gd.exp(((-dtf) * Gv.F(0.9)))) + (Gd.absf((lv - lvp)) * Gv.F(0.7)))).AsF();
    slosh = (Gd.minf(slosh, Gv.F(5.0))).AsF();
    fxs = fxs.SI(Gv.S("lv"), lv);
    fxs = fxs.SI(Gv.S("slosh"), slosh);
    Gv is_mash = Gv.B(false);
    if (Gv.Eq(b, Gv.S("sk"))) {
      is_mash = Gv.B(Gv.Gt(sim.C("vol", S.G("sk").G("mash")), Gv.I(50L)));
    } else if (Gv.Ne(vv, Gv.Null)) {
      is_mash = Gv.B(Gv.Eq(sim.C("contentKind", vv), Gv.S("mash")));
    }
    foreach (Gv i in ((N + Gv.I(1L))).Iter()) {
      Gv px = ((x - Gv.I(20L)) + ((((w + Gv.I(40L))) * i) / Gd.Dyn("float", N)));
      Gv r = Gd.clampf((((px - ((x + (w * Gv.F(0.5)))))) / ((w * Gv.F(0.5)))), Gv.F(-1.0), Gv.F(1.0));
      Gv sl = (((slosh * Gd.sin(((tt * Gv.F(2.3)) + (r * Gv.F(2.6))))) * r)).AsF();
      below.C("append", Gd.Ctor("Vector2", px, ((((lv - (vort_px * (((r * r) - Gv.F(0.5))))) + sl) + (amp * Gd.sin(((px * Gv.F(0.045)) + (tt * Gv.F(3.1)))))) + ((amp * Gv.F(0.5)) * Gd.sin(((px * Gv.F(0.11)) - (tt * Gv.F(4.7))))))));
    }
    below.C("append", Gd.Ctor("Vector2", ((x + w) + Gv.I(20L)), (y0 + Gv.I(50L))));
    below.C("append", Gd.Ctor("Vector2", (x - Gv.I(20L)), (y0 + Gv.I(50L))));
    Gv polys = Gd.Static("Geometry2D", "intersect_polygons", outline, below);
    Gv mtex = Gv.Null;
    Gv uvoff = Gd.StaticGet("Vector2", "ZERO");
    if ((is_mash).T()) {
      mtex = S_liquidfx.tex_mash();
      Self.S("texture_repeat", Gd.StaticGet("CanvasItem", "TEXTURE_REPEAT_ENABLED"));
      uvoff = Gd.Ctor("Vector2", ((tt * Gv.F(9.0)) * stirK), ((Gd.sin((tt * Gv.F(0.7))) * Gv.F(3.0)) * stirK));
      col = col.C("lightened", Gv.F(0.12));
    }
    Gv gpu_ok = Gv.B(((Gv.Ne(b, Gv.S("lv")) && !(is_mash).T()) && !(m.G("no_gpu_liquid")).T()));
    if ((gpu_ok).T()) {
      Gv turb_g = (Gv.F(0.0)).AsF();
      if (Gv.Eq(b, Gv.S("sk"))) {
        turb_g = (Gd.clampf((Gd.Dyn("float", S.G("sk").G("turb")) / Gv.F(80.0)), Gv.F(0.0), Gv.F(1.0))).AsF();
      } else if (Gv.Ne(vv, Gv.Null)) {
        turb_g = (Gd.clampf((vv.G("trubSus") / ((Gd.maxf(Gv.F(1.0), sim.C("liqVol", vv)) * Gv.F(0.006)))), Gv.F(0.0), Gv.F(1.0))).AsF();
      }
      _gpu_body().C("set_params", col, Gd.Ctor("Rect2", x, it.G("y"), w, it.G("h")), lv, Gd.maxf(Gv.F(30.0), (y0 - lv)), turb_g, stirK, ((Gv.Ne(vv, Gv.Null) && (vv.G("boiling")).T()) ? Gv.F(1.0) : Gv.F(0.0)));
      _gpu_body().C("set_polys", polys);
    } else {
      _gpu_clear();
      foreach (Gv p in (polys).Iter()) {
        if (Gv.Eq(b, Gv.S("lv"))) {
          _shade_liquid(p, it.G("y"), it.G("h"), col, Gv.B(true));
        } else {
          _shade_liquid(p, x, w, col, Gv.B(false), lv, y0, mtex, uvoff);
        }
      }
    }
    Gv calm = ((Gv.F(1.0) - stirK)).AsF();
    Gv hopped = Gv.B(false);
    Gv area = Gd.Ctor("Rect2", (x + Gv.I(8L)), (lv + Gv.I(4L)), (w - Gv.I(16L)), Gd.maxf(Gv.F(4.0), ((y0 - lv) - Gv.I(8L))));
    if ((Gv.Ne(vv, Gv.Null) && (is_mash).T())) {
      S_liquidfx.particles(Self, fxs, Gv.S("pp"), area, outline, dtf, tt, Gd.Dict(Gv.S("kind"), Gv.S("mash"), Gv.S("stir"), stirK, Gv.S("boil"), vv.G("boiling"), Gv.S("a_husk"), Gv.F(1.0), Gv.S("seed"), Gv.F(11.0), Gv.S("col"), Gd.Ctor("Color", Gv.F(0.7), Gv.F(0.55), Gv.F(0.32))));
    } else if ((Gv.Eq(b, Gv.S("sk")) && (is_mash).T())) {
      S_liquidfx.particles(Self, fxs, Gv.S("pp"), area, outline, dtf, tt, Gd.Dict(Gv.S("kind"), Gv.S("mash"), Gv.S("stir"), stirK, Gv.S("a_husk"), Gv.F(1.0), Gv.S("seed"), Gv.F(11.0), Gv.S("col"), Gd.Ctor("Color", Gv.F(0.7), Gv.F(0.55), Gv.F(0.32))));
    }
    if ((Gv.Eq(b, Gv.S("sk")) && !(is_mash).T())) {
      Gv tk = (Gd.clampf((Gd.Dyn("float", S.G("sk").G("turb")) / Gv.F(80.0)), Gv.F(0.0), Gv.F(1.0))).AsF();
      if (Gv.Gt(tk, Gv.F(0.03))) {
        foreach (Gv p_2 in (polys).Iter()) {
          Self.C("draw_colored_polygon", p_2, Gd.Ctor("Color", Gv.F(0.93), Gv.F(0.85), Gv.F(0.64), (tk * Gv.F(0.38))));
        }
        S_liquidfx.particles(Self, fxs, Gv.S("pp"), area, outline, dtf, tt, Gd.Dict(Gv.S("kind"), Gv.S("wort"), Gv.S("stir"), Gd.maxf(stirK, (Gv.F(0.25) * tk)), Gv.S("a_fine"), Gd.minf(Gv.F(1.0), (tk * Gv.F(1.2))), Gv.S("seed"), Gv.F(5.0), Gv.S("col"), col));
      }
    }
    if ((Gv.Ne(vv, Gv.Null) && !(is_mash).T())) {
      Gv Vv = (Gd.maxf(Gv.F(1.0), sim.C("liqVol", vv))).AsF();
      Gv haze = (Gd.clampf((vv.G("trubSus") / ((Vv * Gv.F(0.006)))), Gv.F(0.0), Gv.F(1.0))).AsF();
      if (Gv.Gt(haze, Gv.F(0.01))) {
        foreach (Gv p_3 in (polys).Iter()) {
          Self.C("draw_colored_polygon", p_3, Gd.Ctor("Color", Gv.F(0.93), Gv.F(0.85), Gv.F(0.64), (haze * Gv.F(0.3))));
        }
      }
      hopped = Gv.B(Gv.Gt(((vv.G("alpha") + vv.G("iso"))), Gv.F(0.5)));
      Gv hs_now = ((vv.G("alpha") + vv.G("iso"))).AsF();
      if (((fxs.C("has", Gv.S("hs"))).T() && Gv.Gt((hs_now - Gd.Dyn("float", fxs.I(Gv.S("hs")))), Gv.F(0.5)))) {
        fxs = fxs.SI(Gv.S("hopt"), tt);
      }
      fxs = fxs.SI(Gv.S("hs"), hs_now);
      Gv wort = Gv.B(Gv.Gt(sim.C("ext", vv), Gv.F(5.0)));
      Gv a_floc = ((Gv.Gt(vv.G("coag"), Gv.F(0.01)) ? (Gd.clampf((vv.G("coag") * Gv.F(3.0)), Gv.F(0.0), Gv.F(1.0)) * ((((vv.G("boiling")).T() || (vv.G("whirlNow")).T()) ? Gv.F(1.0) : Gv.F(0.5)))) : Gv.F(0.0))).AsF();
      Gv a_hop = ((((hopped).T() && (wort).T()) ? Gd.clampf((((vv.G("alpha") + vv.G("iso"))) / Gv.F(40.0)), Gv.F(0.25), Gv.F(1.0)) : Gv.F(0.0))).AsF();
      Gv a_fine = (((wort).T() ? Gd.clampf((Gv.F(0.12) + (haze * Gv.F(0.8))), Gv.F(0.0), Gv.F(1.0)) : Gv.F(0.0))).AsF();
      if (((Gv.Gt(a_floc, Gv.F(0.0)) || Gv.Gt(a_hop, Gv.F(0.0))) || Gv.Gt(a_fine, Gv.F(0.0)))) {
        S_liquidfx.particles(Self, fxs, Gv.S("pp"), area, outline, dtf, tt, Gd.Dict(Gv.S("kind"), Gv.S("wort"), Gv.S("stir"), stirK, Gv.S("boil"), vv.G("boiling"), Gv.S("whirl"), ((vv.G("whirlNow")).T() ? vv.G("vort") : Gv.F(0.0)), Gv.S("a_floc"), a_floc, Gv.S("a_hop"), a_hop, Gv.S("a_fine"), a_fine, Gv.S("seed"), Gv.F(5.0), Gv.S("col"), col));
      }
      if ((((Gv.In(b, Gd.Arr(Gv.S("wp"), Gv.S("mp"))) && !(vv.G("whirlNow")).T()) && !(vv.G("boiling")).T()) && Gv.Gt(vv.G("trubSet"), Gv.F(0.02)))) {
        Gv pxL = ((((y0 - lv)) / Gd.maxf(Gv.F(50.0), sim.C("vol", vv)))).AsF();
        Gv hpx = (Gd.clampf((((vv.G("trubSet") * Gv.F(6.0)) * pxL) * Gv.F(5.0)), Gv.F(4.0), (it.G("h") * Gv.F(0.17)))).AsF();
        S_liquidfx.draw_trub_cone(Self, (x + (w * Gv.F(0.5))), (y0 - Gv.F(2.0)), Gd.clampf((hpx * Gv.F(1.7)), Gv.F(12.0), (w * Gv.F(0.32))), hpx, tt);
      }
    }
    if (Gv.Ne(b, Gv.S("lv"))) {
      S_liquidfx.draw_caustics(Self, area, outline, tt, ((is_mash).T() ? Gv.F(0.35) : Gv.F(1.0)), col);
    }
    if (Gv.Lt(lv, (y0 - Gv.I(2L)))) {
      Gv surf = Gd.Ctor("PackedVector2Array");
      Gv back = Gd.Ctor("PackedVector2Array");
      foreach (Gv i_2 in ((N + Gv.I(1L))).Iter()) {
        Gv px2 = ((x + Gv.I(6L)) + ((((w - Gv.I(12L))) * i_2) / Gd.Dyn("float", N)));
        Gv r2 = (((px2 - ((x + (w * Gv.F(0.5)))))) / ((w * Gv.F(0.5))));
        Gv sl2 = (((slosh * Gd.sin(((tt * Gv.F(2.3)) + (r2 * Gv.F(2.6))))) * r2)).AsF();
        Gv yy = (((((lv - (vort_px * (((r2 * r2) - Gv.F(0.5))))) + sl2) + (amp * Gd.sin(((px2 * Gv.F(0.045)) + (tt * Gv.F(3.1)))))) + ((amp * Gv.F(0.5)) * Gd.sin(((px2 * Gv.F(0.11)) - (tt * Gv.F(4.7))))))).AsF();
        surf.C("append", Gd.Ctor("Vector2", px2, yy));
        back.C("append", Gd.Ctor("Vector2", px2, (yy - (Gv.F(9.0) * Gd.sqrt(Gd.maxf(Gv.F(0.0), (Gv.F(1.0) - (r2 * r2))))))));
      }
      if (((Gd.Static("Geometry2D", "is_point_in_polygon", Gd.Ctor("Vector2", (x + (w * Gv.F(0.5))), (lv + Gv.I(2L))), outline)).T() || Gv.Eq(b, Gv.S("lv")))) {
        if (((Gv.Ne(b, Gv.S("lv")) && Gv.Ne(b, Gv.S("sk"))) || (Gv.Eq(b, Gv.S("sk")) && !(is_mash).T()))) {
          Gv lens = Gd.Ctor("PackedVector2Array", back);
          foreach (Gv i_3 in Gd.RangeIter((surf.C("size") - Gv.I(1L)), Gv.I(-1L), Gv.I(-1L))) {
            lens.C("append", surf.I(i_3));
          }
          Gv lc = (!(is_mash).T() ? col.C("lightened", Gv.F(0.3)) : col.C("lightened", Gv.F(0.05)));
          lc = lc.S("a", Gv.F(0.55));
          Gv lp = Gd.Static("Geometry2D", "intersect_polygons", outline, lens);
          foreach (Gv q in (lp).Iter()) {
            Self.C("draw_colored_polygon", q, lc);
          }
        }
        Gv sheen = Gd.Ctor("PackedVector2Array");
        foreach (Gv q_2 in (surf).Iter()) {
          sheen.C("append", (q_2 + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(5L))));
        }
        Self.C("draw_polyline", sheen, Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.1)), Gv.F(8.0), Gv.B(true));
        if (Gv.Ne(vv, Gv.Null)) {
          Gv th = (Gv.F(0.0)).AsF();
          if ((vv.G("boiling")).T()) {
            th = ((Gv.F(6.0) + Gd.minf(Gv.F(30.0), (vv.G("foam") * Gv.F(0.05))))).AsF();
          } else if (Gv.Gt(vv.G("foam"), Gv.F(3.0))) {
            th = ((Gv.F(4.0) + Gd.minf(Gv.F(30.0), (vv.G("foam") * Gv.F(0.05))))).AsF();
          }
          if (Gv.Gt(th, Gv.F(0.0))) {
            S_liquidfx.draw_foam(Self, x, (x + w), surf, th, Gd.clampf((((vv.G("alpha") + vv.G("iso"))) / Gv.F(60.0)), Gv.F(0.0), Gv.F(1.0)), tt);
          }
          if ((vv.G("boiling")).T()) {
            foreach (Gv i_4 in (Gv.I(14L)).Iter()) {
              Gv fxp = ((x + Gv.I(10L)) + Gd.fmod((i_4 * Gv.F(37.3)), (w - Gv.I(20L))));
              Self.C("draw_circle", Gd.Ctor("Vector2", fxp, ((lv - (th * Gv.F(0.5))) + (Gv.F(2.0) * Gd.sin(((tt * Gv.F(2.0)) + i_4))))), (Gv.F(2.2) + Gd.fmod((i_4 * Gv.F(1.7)), Gv.F(2.5))), Gd.Ctor("Color", Gv.I(1L), Gv.F(0.98), Gv.F(0.9), Gv.F(0.55)));
            }
          }
          if (((hopped).T() && ((vv.G("boiling")).T() || Gv.Gt(calm, Gv.F(0.5))))) {
            foreach (Gv i_5 in (Gv.I(22L)).Iter()) {
              Gv hxp = (((x + Gv.I(12L)) + Gd.fmod((i_5 * Gv.F(41.7)), (w - Gv.I(24L))))).AsF();
              Gv hyp = (((lv - (((vv.G("boiling")).T() ? (th * ((Gv.F(0.3) + (Gv.F(0.5) * S_liquidfx.h(i_5, Gv.F(2.0)))))) : Gv.F(-2.0)))) + (Gv.F(1.0) * Gd.sin(((tt * Gv.F(1.4)) + i_5))))).AsF();
              Gv hl = ((Gv.F(3.0) + (Gv.F(3.0) * S_liquidfx.h(i_5, Gv.F(3.0))))).AsF();
              Gv ha = ((S_liquidfx.h(i_5, Gv.F(4.0)) * Gv.F(3.14))).AsF();
              Gv hd = ((Gd.Ctor("Vector2", Gd.cos(ha), Gd.sin(ha)) * hl) * Gv.F(0.5));
              Self.C("draw_line", (Gd.Ctor("Vector2", hxp, hyp) - hd), (Gd.Ctor("Vector2", hxp, hyp) + hd), Gd.Ctor("Color", Gv.F(0.42), Gv.F(0.47), Gv.F(0.13), Gv.F(0.9)), Gv.F(2.4), Gv.B(true));
            }
          }
        }
        Self.C("draw_polyline", surf, col.C("lightened", Gv.F(0.45)), Gv.F(2.5), Gv.B(true));
        Self.C("draw_polyline", surf, Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.55)), Gv.F(1.0), Gv.B(true));
      }
    }
    if ((Gv.Ne(vv, Gv.Null) && (fxs.C("has", Gv.S("hopt"))).T())) {
      _hop_fall(fxs, x, w, (it.G("y") + Gv.F(26.0)), lv, tt);
    }
    if (Gv.Eq(b, Gv.S("sk"))) {
      _grain_bed(it, fxs, tt, dtf, lv, outline, col);
    }
    Gv v = (Gv.In(b, Gd.Arr(Gv.S("rvp"), Gv.S("mp"), Gv.S("wp"))) ? S.C("get", b) : Gv.Null);
    if ((Gv.Ne(v, Gv.Null) && (v.G("boiling")).T())) {
      Gv t = (Gd.Static("Time", "get_ticks_msec") / Gv.F(1000.0));
      foreach (Gv i_6 in (Gv.I(14L)).Iter()) {
        Gv bx = ((x + Gv.I(20L)) + Gd.fmod((i_6 * Gv.F(53.7)), (w - Gv.I(40L))));
        Gv ph = Gd.fmod(((t * Gv.F(1.7)) + (i_6 * Gv.F(0.37))), Gv.F(1.0));
        Self.C("draw_circle", Gd.Ctor("Vector2", bx, ((lv + Gv.I(18L)) - (ph * Gv.I(22L)))), (Gv.F(2.0) + (Gv.F(2.0) * ph)), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), (Gv.F(0.55) * ((Gv.F(1.0) - ph)))));
      }
      Gv sb = Gd.Ctor("Vector2", (x + (w * Gv.F(0.5))), (it.G("y") - Gv.F(20.0)));
      foreach (Gv dv in (m.G("items")).Iter()) {
        if (((((Gv.Eq(dv.G("type"), Gv.S("deco")) && Gv.Eq(dv.C("get", Gv.S("k")), Gv.S("vent"))) && Gv.Gt(dv.G("x"), (x - Gv.F(40.0)))) && Gv.Lt(dv.G("x"), ((x + w) + Gv.F(40.0)))) && Gv.Lt(dv.G("y"), (it.G("y") + Gv.F(40.0))))) {
          sb = Gd.Ctor("Vector2", dv.G("x"), (dv.G("y") - Gv.F(70.0)));
          break;
        }
      }
      _steam(sb, Gv.F(60.0), Gd.clampf((Gv.F(0.45) + (Gv.F(0.5) * Gd.minf(Gv.F(1.0), (sim.C("vol", v) / Gv.F(200.0))))), Gv.F(0.3), Gv.F(1.0)), t, Gd.Dyn("float", (Gd.Dyn("hash", it.G("id")) % Gv.I(50L))));
    }
    fx = fx.SI(it.G("id"), fxs);
    return Gv.Null;
  }
  public Gv _hop_fall(Gv fxs = default(Gv), Gv x = default(Gv), Gv w = default(Gv), Gv ytop = default(Gv), Gv lv = default(Gv), Gv tt = default(Gv)) {
    x = (x).AsF();
    w = (w).AsF();
    ytop = (ytop).AsF();
    lv = (lv).AsF();
    tt = (tt).AsF();
    Gv e = ((tt - Gd.Dyn("float", fxs.I(Gv.S("hopt"))))).AsF();
    if (Gv.Gt(e, Gv.F(4.5))) {
      fxs.C("erase", Gv.S("hopt"));
      return Gv.Null;
    }
    Gv cx = ((x + (w * Gv.F(0.5)))).AsF();
    foreach (Gv i in (Gv.I(34L)).Iter()) {
      Gv d0 = ((S_liquidfx.h(i, Gv.F(7.0)) * Gv.F(1.6))).AsF();
      Gv te = ((e - d0)).AsF();
      if (Gv.Lt(te, Gv.F(0.0))) {
        continue;
      }
      Gv px = (((cx + ((((S_liquidfx.h(i, Gv.F(8.0)) - Gv.F(0.5))) * w) * Gv.F(0.55))) + (Gv.F(6.0) * Gd.sin(((te * Gv.F(3.0)) + i))))).AsF();
      Gv py = ((ytop + ((Gv.F(520.0) * te) * te))).AsF();
      if (Gv.Lt(py, (lv - Gv.F(2.0)))) {
        Gv ang = (((te * ((Gv.F(4.0) + (Gv.F(6.0) * S_liquidfx.h(i, Gv.F(9.0)))))) + i)).AsF();
        Gv dv = (Gd.Ctor("Vector2", Gd.cos(ang), Gd.sin(ang)) * ((Gv.F(2.6) + (Gv.F(1.6) * S_liquidfx.h(i, Gv.F(10.0))))));
        Self.C("draw_line", (Gd.Ctor("Vector2", px, py) - dv), (Gd.Ctor("Vector2", px, py) + dv), Gd.Ctor("Color", Gv.F(0.3), Gv.F(0.38), Gv.F(0.1), Gv.F(0.95)), Gv.F(3.6), Gv.B(true));
        Self.C("draw_line", (Gd.Ctor("Vector2", px, py) - (dv * Gv.F(0.6))), (Gd.Ctor("Vector2", px, py) + (dv * Gv.F(0.6))), Gd.Ctor("Color", Gv.F(0.5), Gv.F(0.58), Gv.F(0.2), Gv.F(0.9)), Gv.F(1.6), Gv.B(true));
      } else {
        Gv tsp = ((te - Gd.sqrt(Gd.maxf(Gv.F(0.0), (((lv - ytop)) / Gv.F(520.0)))))).AsF();
        if ((Gv.Ge(tsp, Gv.F(0.0)) && Gv.Lt(tsp, Gv.F(0.7)))) {
          Gv k = ((tsp / Gv.F(0.7))).AsF();
          Self.C("draw_arc", Gd.Ctor("Vector2", px, (lv + Gv.F(1.0))), (Gv.F(3.0) + (Gv.F(14.0) * k)), Gv.F(0.0), Gd.K("TAU"), Gv.I(20L), Gd.Ctor("Color", Gv.I(1L), Gv.F(0.97), Gv.F(0.85), (Gv.F(0.55) * ((Gv.F(1.0) - k)))), Gv.F(1.5), Gv.B(true));
          if (Gv.Lt(tsp, Gv.F(0.25))) {
            Self.C("draw_circle", Gd.Ctor("Vector2", px, (lv - (((Gv.F(8.0) * tsp) * ((Gv.F(1.0) - (tsp * Gv.F(4.0))))) * Gv.F(6.0)))), Gv.F(1.6), Gd.Ctor("Color", Gv.I(1L), Gv.F(0.97), Gv.F(0.85), Gv.F(0.7)));
          }
        }
      }
    }
    return Gv.Null;
  }
  public Gv _steam(Gv base_ = default(Gv), Gv wd = default(Gv), Gv k = default(Gv), Gv tt = default(Gv), Gv sd = default(Gv)) {
    wd = (wd).AsF();
    k = (k).AsF();
    tt = (tt).AsF();
    sd = (sd).AsF();
    Gv tex = S_liquidfx.tex_puff();
    foreach (Gv i in (Gv.I(12L)).Iter()) {
      Gv ph = (Gd.fmod(((tt * ((Gv.F(0.16) + (Gv.F(0.06) * S_liquidfx.h(i, sd))))) + S_liquidfx.h(i, (sd + Gv.F(1.0)))), Gv.F(1.0))).AsF();
      Gv sz = ((((Gv.F(50.0) + (Gv.F(170.0) * ph))) * ((Gv.F(0.7) + (Gv.F(0.6) * S_liquidfx.h(i, (sd + Gv.F(2.0)))))))).AsF();
      Gv drift = ((((Gv.F(40.0) * ph) * ph) + ((Gv.F(14.0) * Gd.sin(((tt * Gv.F(0.6)) + (i * Gv.F(1.7))))) * ph))).AsF();
      Gv pos = Gd.Ctor("Vector2", ((base_.G("x") + (((S_liquidfx.h(i, (sd + Gv.F(3.0))) - Gv.F(0.5))) * wd)) + drift), (base_.G("y") - (Gv.F(230.0) * ph)));
      Gv al = ((((k * Gv.F(0.46)) * Gd.sin((Gd.clampf(ph, Gv.F(0.0), Gv.F(1.0)) * Gd.K("PI")))) * ((Gv.F(1.0) - (Gv.F(0.4) * ph))))).AsF();
      Self.C("draw_texture_rect", tex, Gd.Ctor("Rect2", (pos - (Gd.Ctor("Vector2", sz, sz) * Gv.F(0.5))), Gd.Ctor("Vector2", sz, sz)), Gv.B(false), Gd.Ctor("Color", Gv.F(0.74), Gv.F(0.79), Gv.F(0.86), Gd.minf(Gv.F(0.6), (al * Gv.F(1.5)))));
    }
    return Gv.Null;
  }
  public Gv _grain_bed(Gv it = default(Gv), Gv fxs = default(Gv), Gv tt = default(Gv), Gv dtf = default(Gv), Gv lv = default(Gv), Gv outline = default(Gv), Gv col = default(Gv)) {
    tt = (tt).AsF();
    dtf = (dtf).AsF();
    lv = (lv).AsF();
    Gv S = m.G("sim").G("S");
    Gv sim = m.G("sim");
    Gv x = (it.G("x")).AsF();
    Gv w = (it.G("w")).AsF();
    Gv y0 = ((it.G("y") + it.G("h"))).AsF();
    Gv sieve = (y0 - Gv.F(22.0));
    Gv tgt = (((sim.C("skBedH") / Gv.F(0.943)) * it.G("h"))).AsF();
    Gv bed_px = (Gd.Dyn("float", fxs.C("get", Gv.S("bed"), tgt))).AsF();
    bed_px = ((bed_px + (((tgt - bed_px)) * Gd.minf(Gv.F(1.0), (dtf * Gv.F(2.5)))))).AsF();
    fxs = fxs.SI(Gv.S("bed"), bed_px);
    if (Gv.Gt(bed_px, Gv.I(1L))) {
      Gv top_y = ((sieve - bed_px)).AsF();
      Gv covered = Gv.B(Gv.Le(lv, top_y));
      Gv rake = Gd.Dyn("float", fxs.C("get", Gv.S("stir"), Gv.F(0.0)));
      Gv tex = S_liquidfx.tex_bed();
      Self.S("texture_repeat", Gd.StaticGet("CanvasItem", "TEXTURE_REPEAT_ENABLED"));
      Gv N2 = Gv.I(40L);
      Gv prof = Gd.Ctor("PackedVector2Array");
      foreach (Gv i in ((N2 + Gv.I(1L))).Iter()) {
        Gv px = (((x + Gv.F(4.0)) + ((((w - Gv.F(8.0))) * i) / Gd.Dyn("float", N2)))).AsF();
        Gv hh = (((Gv.F(3.2) * ((S_liquidfx._vn((px / Gv.F(17.0)), Gv.F(0.5), Gv.I(64L), Gv.F(4.0)) - Gv.F(0.5)))) + (Gv.F(1.6) * ((S_liquidfx._vn((px / Gv.F(6.0)), Gv.F(2.5), Gv.I(128L), Gv.F(5.0)) - Gv.F(0.5)))))).AsF();
        hh = ((hh + (rake * (((Gv.F(3.4) * Gd.sin(((px * Gv.F(0.085)) + (tt * Gv.F(1.3))))) + (Gv.F(1.6) * Gd.sin(((px * Gv.F(0.21)) - (tt * Gv.F(2.1)))))))))).AsF();
        prof.C("append", Gd.Ctor("Vector2", px, (top_y + hh)));
      }
      Gv poly = Gd.Ctor("PackedVector2Array", prof);
      poly.C("append", Gd.Ctor("Vector2", ((x + w) - Gv.F(4.0)), sieve));
      poly.C("append", Gd.Ctor("Vector2", (x + Gv.F(4.0)), sieve));
      Gv cols = Gd.Ctor("PackedColorArray");
      foreach (Gv p in (poly).Iter()) {
        Gv rel = (Gd.clampf((((p.G("y") - top_y)) / Gd.maxf(Gv.F(1.0), bed_px)), Gv.F(0.0), Gv.F(1.0))).AsF();
        Gv side = (Gd.absf((((p.G("x") - ((x + (w * Gv.F(0.5)))))) / ((w * Gv.F(0.5)))))).AsF();
        Gv k = ((((((covered).T() ? Gv.F(0.92) : Gv.F(1.12))) - (Gv.F(0.3) * rel)) - (((Gv.F(0.26) * side) * side) * side))).AsF();
        Gv c0 = (!(covered).T() ? Gd.Ctor("Color", Gv.S("bc9c6c")) : Gd.Ctor("Color", Gv.S("9c7c4c")));
        cols.C("append", Gd.Ctor("Color", Gd.minf(Gv.F(1.0), ((c0.G("r") * k) * Gv.F(1.18))), Gd.minf(Gv.F(1.0), ((c0.G("g") * k) * Gv.F(1.18))), Gd.minf(Gv.F(1.0), ((c0.G("b") * k) * Gv.F(1.18))), Gv.F(1.0)));
      }
      S_liquidfx.draw_tex_poly(Self, poly, cols, tex, Gd.Ctor("Vector2", Gv.F(0.0), Gv.F(0.0)), Gv.F(1.15));
      Self.C("draw_polyline", prof, Gd.Ctor("Color", Gv.F(1.0), Gv.F(0.92), Gv.F(0.7), (!(covered).T() ? Gv.F(0.3) : Gv.F(0.18))), Gv.F(2.0), Gv.B(true));
      if ((covered).T()) {
        Gv tint = col;
        tint = tint.S("a", Gv.F(0.22));
        Self.C("draw_colored_polygon", poly, tint);
      }
      Gv ch = (Gd.clampf(((Gd.Dyn("float", S.G("sk").C("get", Gv.S("c"), Gv.F(0.0))) * Gv.F(3.0)) + Gd.Dyn("float", S.G("sk").C("get", Gv.S("air"), Gv.F(0.0)))), Gv.F(0.0), Gv.F(1.0))).AsF();
      if (Gv.Gt(ch, Gv.F(0.05))) {
        foreach (Gv i_2 in (Gd.Dyn("int", (Gv.I(3L) + (Gv.I(8L) * ch)))).Iter()) {
          Gv cx = (((x + Gv.F(14.0)) + (S_liquidfx.h(i_2, Gv.F(61.0)) * ((w - Gv.F(28.0)))))).AsF();
          Gv cl = ((bed_px * ((Gv.F(0.35) + (Gv.F(0.6) * S_liquidfx.h(i_2, Gv.F(62.0))))))).AsF();
          Self.C("draw_polyline", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", cx, (top_y + Gv.I(2L))), Gd.Ctor("Vector2", (cx + (Gv.I(3L) * Gd.sin(i_2))), (top_y + (cl * Gv.F(0.5)))), Gd.Ctor("Vector2", (cx - Gv.I(2L)), (top_y + cl)))), Gd.Ctor("Color", Gv.F(0.1), Gv.F(0.06), Gv.F(0.02), Gv.F(0.45)), Gv.F(1.2), Gv.B(true));
        }
      }
      if (!(covered).T()) {
        Self.C("draw_line", Gd.Ctor("Vector2", (x + Gv.I(4L)), top_y), Gd.Ctor("Vector2", ((x + w) - Gv.I(4L)), top_y), Gd.Ctor("Color", Gv.S("e0412f")), Gv.F(2.0));
      }
      Gv susp_t = Gv.F(0.0);
      if (((covered).T() && ((S.G("sk").G("rake")).T() || (S.G("skWater").G("on")).T()))) {
        susp_t = Gv.F(1.0);
      }
      Gv susp = (Gd.Dyn("float", fxs.C("get", Gv.S("susp"), Gv.F(0.0)))).AsF();
      susp = ((susp + (((susp_t - susp)) * Gd.minf(Gv.F(1.0), (dtf * ((Gv.Gt(susp_t, susp) ? Gv.F(0.9) : Gv.F(0.18)))))))).AsF();
      fxs = fxs.SI(Gv.S("susp"), susp);
      if ((Gv.Gt(susp, Gv.F(0.03)) && Gv.Lt(lv, (top_y - Gv.F(4.0))))) {
        Gv zr = Gd.Ctor("Rect2", (x + Gv.I(8L)), (lv + Gv.I(3L)), (w - Gv.I(16L)), ((top_y - lv) - Gv.I(3L)));
        S_liquidfx.particles(Self, fxs, Gv.S("sp"), zr, outline, dtf, tt, Gd.Dict(Gv.S("kind"), Gv.S("mash"), Gv.S("stir"), Gd.maxf(rake, (Gv.F(0.35) * susp)), Gv.S("a_husk"), susp, Gv.S("seed"), Gv.F(14.0), Gv.S("col"), Gd.Ctor("Color", Gv.F(0.7), Gv.F(0.55), Gv.F(0.32))));
      }
    }
    Gv sx = (x + Gv.I(4L));
    while (Gv.Lt(sx, ((x + w) - Gv.I(4L)))) {
      Self.C("draw_line", Gd.Ctor("Vector2", sx, sieve), Gd.Ctor("Vector2", Gd.minf((sx + Gv.I(8L)), ((x + w) - Gv.I(4L))), sieve), Gd.Ctor("Color", Gv.S("2b3138")), Gv.F(3.0));
      sx = (sx + Gv.F(12.0));
    }
    return Gv.Null;
  }
  public Gv _lq_f(Gv t = default(Gv)) {
    t = (t).AsF();
    t = (Gd.clampf(t, Gv.F(0.0), Gv.F(1.0))).AsF();
    foreach (Gv i in Gd.RangeIter(Gv.I(1L), LQ_T.C("size"))) {
      if (Gv.Le(t, LQ_T.I(i))) {
        return (Gd.lerpf(LQ_F.I((i - Gv.I(1L))), LQ_F.I(i), (((t - LQ_T.I((i - Gv.I(1L))))) / ((LQ_T.I(i) - LQ_T.I((i - Gv.I(1L)))))))).AsF();
      }
    }
    return (LQ_F.I((LQ_F.C("size") - Gv.I(1L)))).AsF();
    return Gv.Null;
  }
  public Gv _lq_c(Gv col = default(Gv), Gv f = default(Gv)) {
    f = (f).AsF();
    return (Gv.Gt(f, Gv.F(0.0)) ? col.C("lightened", f) : col.C("darkened", (-f)));
    return Gv.Null;
  }
  public Gv _shade_liquid(Gv p = default(Gv), Gv a0 = default(Gv), Gv span = default(Gv), Gv col = default(Gv), Gv horiz = default(Gv), Gv top = default(Gv), Gv bot = default(Gv), Gv tex = default(Gv), Gv uvoff = default(Gv)) {
    a0 = (a0).AsF();
    span = (span).AsF();
    if (top.k == 0) top = Gv.F(0.0);
    if (bot.k == 0) bot = Gv.F(0.0);
    if (tex.k == 0) tex = Gv.Null;
    if (uvoff.k == 0) uvoff = Gd.StaticGet("Vector2", "ZERO");
    Gv dep = Gd.maxf(Gv.F(1.0), (bot - top));
    Gv lo = Gd.Ctor("Vector2", Gv.F(1000000000.0), Gv.F(1000000000.0));
    Gv hi = Gd.Ctor("Vector2", Gv.F(-1000000000.0), Gv.F(-1000000000.0));
    foreach (Gv q in (p).Iter()) {
      lo = lo.C("min", q);
      hi = hi.C("max", q);
    }
    Self.C("draw_colored_polygon", p, col.C("darkened", Gv.F(0.1)));
    foreach (Gv i in ((LQ_T.C("size") - Gv.I(1L))).Iter()) {
      Gv s0 = ((a0 + (span * LQ_T.I(i)))).AsF();
      Gv s1 = ((a0 + (span * LQ_T.I((i + Gv.I(1L)))))).AsF();
      Gv strip = Gd.NewArray();
      if ((horiz).T()) {
        strip = Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", (lo.G("x") - Gv.I(2L)), s0), Gd.Ctor("Vector2", (hi.G("x") + Gv.I(2L)), s0), Gd.Ctor("Vector2", (hi.G("x") + Gv.I(2L)), s1), Gd.Ctor("Vector2", (lo.G("x") - Gv.I(2L)), s1)));
      } else {
        strip = Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", s0, (lo.G("y") - Gv.I(2L))), Gd.Ctor("Vector2", s1, (lo.G("y") - Gv.I(2L))), Gd.Ctor("Vector2", s1, (hi.G("y") + Gv.I(2L))), Gd.Ctor("Vector2", s0, (hi.G("y") + Gv.I(2L)))));
      }
      foreach (Gv q_2 in (Gd.Static("Geometry2D", "intersect_polygons", p, strip)).Iter()) {
        if ((Gv.Lt(q_2.C("size"), Gv.I(3L)) || Gv.Lt(Gd.absf(_area(q_2)), Gv.F(1.0)))) {
          continue;
        }
        Gv cs = Gd.Ctor("PackedColorArray");
        foreach (Gv v in (q_2).Iter()) {
          Gv fv = _lq_f(((((((horiz).T() ? v.G("y") : v.G("x"))) - a0)) / span));
          if (Gv.Gt(bot, top)) {
            fv = (fv - (Gv.F(0.28) * Gd.clampf((((v.G("y") - top)) / dep), Gv.F(0.0), Gv.F(1.0))));
          }
          cs.C("append", _lq_c(col, fv));
        }
        if (Gv.Ne(tex, Gv.Null)) {
          S_liquidfx.draw_tex_poly(Self, q_2, cs, tex, uvoff, Gv.F(1.3));
        } else {
          Self.C("draw_polygon", q_2, cs);
        }
      }
    }
    return Gv.Null;
  }
  public Gv _area(Gv q = default(Gv)) {
    Gv a = Gv.F(0.0);
    foreach (Gv i in (q.C("size")).Iter()) {
      a = (a + q.I(i).C("cross", q.I((((i + Gv.I(1L))) % q.C("size")))));
    }
    return ((a * Gv.F(0.5))).AsF();
    return Gv.Null;
  }
  public Gv _gauge_levels(Gv v = default(Gv)) {
    if (((m.G("hard")).T() && Gv.Eq(v.C("get", Gv.S("bind")), Gv.S("rvp")))) {
      foreach (Gv d in (m.G("items")).Iter()) {
        if (Gv.Ne(d.G("type"), Gv.S("dip"))) {
          continue;
        }
        Gv D = m.C("dip_geo", d);
        Gv calm2 = m.C("vessel_calm", v);
        Gv br2 = Gd.Ctor("Rect2", (D.G("x") + Gv.F(24.0)), ((D.G("y") + (D.G("h") * Gv.F(0.5))) - Gv.F(13.0)), Gv.F(54.0), Gv.F(26.0));
        S_art.rrect_border(Self, br2, ((calm2).T() ? Gd.Ctor("Color", Gv.S("1d4f9c")) : Gd.Ctor("Color", Gv.S("5a6169"))), Gd.Ctor("Color", Gv.S("0d1013")), Gv.I(2L), Gv.F(6.0));
        Self.C("draw_string", m.G("font_b"), Gd.Ctor("Vector2", (br2.G("position").G("x") + Gv.I(8L)), (br2.G("position").G("y") + Gv.I(18L))), Gv.S("MĚŘ"), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(14L), ((calm2).T() ? Gd.StaticGet("Color", "WHITE") : Gd.Ctor("Color", Gv.S("b4bcc3"))));
        if ((d.C("has", Gv.S("_meas"))).T()) {
          Gv wy = (((D.G("y") + D.G("h")) - ((D.G("h") * ((Gd.Dyn("float", d.I(Gv.S("_meas"))) - Gv.F(3.0)))) / Gv.F(47.0)))).AsF();
          wy = (Gd.clampf(wy, D.G("y"), (D.G("y") + D.G("h")))).AsF();
          Self.C("draw_rect", Gd.Ctor("Rect2", (D.G("x") - Gv.I(7L)), wy, Gv.I(14L), ((D.G("y") + D.G("h")) - wy)), Gd.Ctor("Color", Gv.F(0.08), Gv.F(0.05), Gv.F(0.02), Gv.F(0.62)));
          Self.C("draw_line", Gd.Ctor("Vector2", (D.G("x") - Gv.I(7L)), wy), Gd.Ctor("Vector2", (D.G("x") + Gv.I(7L)), wy), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.8)), Gv.F(1.5));
        }
      }
    }
    foreach (Gv g__it in (m.G("items")).Iter()) {
      Gv g = g__it;
      if (Gv.Ne(g.G("type"), Gv.S("gauge"))) {
        continue;
      }
      if (Gv.Ne(m.C("gauge_vessel", g), v)) {
        continue;
      }
      Gv G = m.C("gauge_geo", g);
      Gv ly = (Gd.clampf(m.C("level_y", v), G.G("y1"), G.G("y2"))).AsF();
      if ((m.G("hard")).T()) {
        Gv calm = m.C("vessel_calm", v);
        Gv br = Gd.Ctor("Rect2", (G.G("x") + Gv.F(14.0)), ((((G.G("y1") + G.G("y2"))) * Gv.F(0.5)) - Gv.F(13.0)), Gv.F(54.0), Gv.F(26.0));
        S_art.rrect_border(Self, br, ((calm).T() ? Gd.Ctor("Color", Gv.S("1d4f9c")) : Gd.Ctor("Color", Gv.S("5a6169"))), Gd.Ctor("Color", Gv.S("0d1013")), Gv.I(2L), Gv.F(6.0));
        Self.C("draw_string", m.G("font_b"), Gd.Ctor("Vector2", (br.G("position").G("x") + Gv.I(8L)), (br.G("position").G("y") + Gv.I(18L))), Gv.S("MĚŘ"), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(14L), ((calm).T() ? Gd.StaticGet("Color", "WHITE") : Gd.Ctor("Color", Gv.S("b4bcc3"))));
        if (!(g.C("has", Gv.S("_meas"))).T()) {
          continue;
        }
        Gv my = (g.I(Gv.S("_meas"))).AsF();
        Self.C("draw_rect", Gd.Ctor("Rect2", (G.G("x") - Gv.I(4L)), my, Gv.I(8L), (G.G("y2") - my)), Gd.Ctor("Color", Gv.F(0.13), Gv.F(0.12), Gv.F(0.1), Gv.F(0.9)));
        Self.C("draw_line", Gd.Ctor("Vector2", (G.G("x") - Gv.I(4L)), my), Gd.Ctor("Vector2", (G.G("x") + Gv.I(4L)), my), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.85)), Gv.F(1.5));
        continue;
      }
      if (Gv.Eq(g.C("get", Gv.S("bot"), Gv.B(true)), Gv.B(false))) {
        if (!(g.C("has", Gv.S("_frz"))).T()) {
          g = g.SI(Gv.S("_frz"), ly);
        }
        ly = (g.I(Gv.S("_frz"))).AsF();
      } else {
        g.C("erase", Gv.S("_frz"));
      }
      if (Gv.Ge(ly, (G.G("y2") - Gv.F(0.5)))) {
        continue;
      }
      Gv col = m.C("vessel_color", v);
      Self.C("draw_rect", Gd.Ctor("Rect2", (G.G("x") - Gv.I(4L)), ly, Gv.I(8L), (G.G("y2") - ly)), Gd.Ctor("Color", col, Gv.F(0.92)));
    }
    return Gv.Null;
  }
  public Gv _vstatus(Gv it = default(Gv)) {
    Gv b = it.C("get", Gv.S("bind"));
    Gv d = m.C("vessel_card", b);
    if ((d.C("is_empty")).T()) {
      return Gv.Null;
    }
    Gv fs = Gv.I(14L);
    Gv k = Gv.F(1.0);
    Gv cx = ((it.G("x") + (it.G("w") * Gv.F(0.5)))).AsF();
    Gv top = ((it.G("y") + ((Gv.Eq(it.C("get", Gv.S("shape")), Gv.S("horiz")) ? Gv.F(40.0) : Gv.F(54.0))))).AsF();
    Gv fb = m.G("font_b");
    Gv big = Gv.I(30L);
    Gv sm = Gv.I(13L);
    Gv tiny = Gv.I(11L);
    Gv ts = ((d.G("empty")).T() ? Gv.S("prázdná") : ((Gv.S("%s °C") % m.C("c1", d.G("T")))));
    Gv tcol = ((d.G("empty")).T() ? Gd.Ctor("Color", Gv.S("8a939b")) : m.C("temp_color", d.G("T")));
    Gv bigs = (!(d.G("empty")).T() ? big : Gv.I(18L));
    Gv tw = (fb.C("get_string_size", ts, Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), bigs).G("x")).AsF();
    Gv lw = Gv.F(0.0);
    foreach (Gv l in (d.G("lines")).Iter()) {
      lw = Gd.maxf(lw, m.G("font").C("get_string_size", l, Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), sm).G("x"));
    }
    Gv fw = Gv.F(0.0);
    foreach (Gv fl in (d.G("flags")).Iter()) {
      fw = (fw + (fb.C("get_string_size", fl.I(Gv.I(0L)), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), tiny).G("x") + Gv.F(18.0)));
    }
    Gv pad = Gv.F(10.0);
    Gv dial_r = ((d.G("empty")).T() ? Gv.F(0.0) : Gv.F(19.0));
    Gv dial_w = (Gv.Gt(dial_r, Gv.F(0.0)) ? ((dial_r * Gv.F(2.0)) + Gv.F(10.0)) : Gv.F(0.0));
    Gv W = (((Gd.maxf(((dial_w + tw) + ((Gv.Gt(lw, Gv.F(0.0)) ? (lw + Gv.F(14.0)) : Gv.F(0.0)))), fw) + (pad * Gv.F(2.0))) + Gv.F(4.0))).AsF();
    Gv Hc = (((Gd.maxf(((bigs * Gv.F(1.05)) + Gv.F(14.0)), ((dial_r * Gv.F(2.0)) + Gv.F(6.0))) + pad) + (((d.G("flags").C("size")).T() ? ((tiny * Gv.F(1.6)) + Gv.F(6.0)) : Gv.F(0.0))))).AsF();
    Gv sc = Gd.minf((m.C("label_size") / Gv.F(14.0)), ((it.G("w") * Gv.F(0.9)) / W));
    Self.C("draw_set_transform", (Gd.Ctor("Vector2", cx, top) * ((Gv.F(1.0) - sc))), Gv.F(0.0), Gd.Ctor("Vector2", sc, sc));
    Gv r = Gd.Ctor("Rect2", (cx - (W * Gv.F(0.5))), top, W, Hc);
    S_art.rrect(Self, Gd.Ctor("Rect2", (r.G("position") + Gd.Ctor("Vector2", Gv.I(3L), Gv.I(4L))), r.G("size")), Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.35)), Gv.F(10.0));
    S_art.rrect_border(Self, r, Gd.Ctor("Color", Gv.F(0.045), Gv.F(0.055), Gv.F(0.068), Gv.F(0.9)), Gd.Ctor("Color", tcol, Gv.F(0.6)), Gv.I(2L), Gv.F(10.0));
    Gv bx = (r.G("position").G("x") + pad);
    if (Gv.Gt(dial_r, Gv.F(0.0))) {
      _dial_at(Gd.Ctor("Vector2", ((bx + dial_r) + Gv.F(2.0)), ((r.G("position").G("y") + (pad * Gv.F(0.5))) + (Gd.maxf(((bigs * Gv.F(1.05)) + Gv.F(14.0)), ((dial_r * Gv.F(2.0)) + Gv.F(6.0))) * Gv.F(0.5)))), dial_r, d.G("T"));
      bx = (bx + dial_w);
    }
    Gv by = ((r.G("position").G("y") + (pad * Gv.F(0.4))) + bigs);
    Self.C("draw_string", fb, Gd.Ctor("Vector2", bx, by), ts, Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), bigs, tcol);
    Gv ly = ((r.G("position").G("y") + (pad * Gv.F(0.4))) + (sm * Gv.F(1.15)));
    foreach (Gv l_2 in (d.G("lines")).Iter()) {
      Self.C("draw_string", m.G("font"), Gd.Ctor("Vector2", ((bx + tw) + Gv.F(14.0)), ly), l_2, Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), sm, Gd.Ctor("Color", Gv.S("d5dbe0")));
      ly = (ly + (sm * Gv.F(1.3)));
    }
    Gv fill = Gd.clampf(((((it.G("y") + it.G("h")) - m.C("level_y", it))) / Gd.maxf(Gv.F(1.0), it.G("h"))), Gv.F(0.0), Gv.F(1.0));
    Gv bar = Gd.Ctor("Rect2", bx, (by + Gv.F(6.0)), ((r.G("end").G("x") - pad) - bx), Gv.F(4.0));
    S_art.rrect(Self, bar, Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.08)), Gv.F(2.0));
    if ((Gv.Gt(fill, Gv.F(0.005)) && !(m.G("hard")).T())) {
      S_art.rrect(Self, Gd.Ctor("Rect2", bar.G("position"), Gd.Ctor("Vector2", (bar.G("size").G("x") * fill), bar.G("size").G("y"))), Gd.Ctor("Color", tcol, Gv.F(0.85)), Gv.F(2.0));
    }
    Gv fx = (r.G("position").G("x") + pad);
    Gv fyy = ((r.G("position").G("y") + Gd.maxf(((bigs * Gv.F(1.05)) + Gv.F(14.0)), ((dial_r * Gv.F(2.0)) + Gv.F(6.0)))) + (pad * Gv.F(0.6)));
    foreach (Gv fl_2 in (d.G("flags")).Iter()) {
      Gv fwi = ((fb.C("get_string_size", fl_2.I(Gv.I(0L)), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), tiny).G("x") + Gv.F(14.0))).AsF();
      S_art.rrect(Self, Gd.Ctor("Rect2", fx, fyy, fwi, (tiny * Gv.F(1.6))), Gd.Ctor("Color", fl_2.I(Gv.I(1L)), Gv.F(0.18)), (tiny * Gv.F(0.8)));
      Self.C("draw_string", fb, Gd.Ctor("Vector2", (fx + Gv.F(7.0)), (fyy + (tiny * Gv.F(1.18)))), fl_2.I(Gv.I(0L)), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), tiny, fl_2.I(Gv.I(1L)));
      fx = (fx + (fwi + Gv.F(4.0)));
    }
    Self.C("draw_set_transform", Gd.StaticGet("Vector2", "ZERO"), Gv.F(0.0), Gd.StaticGet("Vector2", "ONE"));
    return Gv.Null;
  }
  public Gv _dial_at(Gv c = default(Gv), Gv R = default(Gv), Gv T = default(Gv)) {
    R = (R).AsF();
    T = (T).AsF();
    Self.C("draw_circle", c, (R + Gv.F(2.0)), Gd.Ctor("Color", Gv.S("0d1013")));
    Self.C("draw_circle", c, (R + Gv.F(0.5)), Gd.Ctor("Color", Gv.S("aab3bb")));
    Self.C("draw_circle", c, (R - Gv.F(1.5)), Gd.Ctor("Color", Gv.S("f4f6f8")));
    Gv a0 = Gd.deg_to_rad(Gv.F(135.0));
    Gv span = Gd.deg_to_rad(Gv.F(270.0));
    Self.C("draw_arc", c, (R - Gv.F(4.5)), (a0 + (span * Gv.F(0.78))), (a0 + span), Gv.I(12L), Gd.Ctor("Color", Gv.S("ff5a4a")), Gv.F(3.0), Gv.B(true));
    Self.C("draw_arc", c, (R - Gv.F(4.5)), (a0 + (span * Gv.F(0.6))), (a0 + (span * Gv.F(0.78))), Gv.I(10L), Gd.Ctor("Color", Gv.S("f2c230")), Gv.F(3.0), Gv.B(true));
    Self.C("draw_arc", c, (R - Gv.F(4.5)), a0, (a0 + (span * Gv.F(0.6))), Gv.I(20L), Gd.Ctor("Color", Gv.S("4fa3ff")), Gv.F(3.0), Gv.B(true));
    foreach (Gv i in (Gv.I(11L)).Iter()) {
      Gv a = (a0 + ((span * i) / Gv.F(10.0)));
      Self.C("draw_line", (c + (Gd.Ctor("Vector2", Gd.cos(a), Gd.sin(a)) * ((R - Gv.F(7.5))))), (c + (Gd.Ctor("Vector2", Gd.cos(a), Gd.sin(a)) * ((R - Gv.F(3.0))))), Gd.Ctor("Color", Gv.S("2b3138")), Gv.F(1.0));
    }
    Gv an = (a0 + (span * Gd.clampf((T / Gv.F(100.0)), Gv.F(0.0), Gv.F(1.0))));
    Self.C("draw_line", (c - (Gd.Ctor("Vector2", Gd.cos(an), Gd.sin(an)) * Gv.F(3.0))), (c + (Gd.Ctor("Vector2", Gd.cos(an), Gd.sin(an)) * ((R - Gv.F(4.0))))), Gd.Ctor("Color", Gv.S("c0261b")), Gv.F(2.0), Gv.B(true));
    Self.C("draw_circle", c, Gv.F(2.4), Gd.Ctor("Color", Gv.S("2b3138")));
    return Gv.Null;
  }
  public Gv _inlet(Gv it = default(Gv)) {
    Gv on = it.C("get", Gv.S("on"), Gv.B(false));
    Gv cx = ((it.G("x") + (it.G("w") * Gv.F(0.5)))).AsF();
    Gv cy = ((it.G("y") + (it.G("h") * Gv.F(0.5)))).AsF();
    Gv L2 = (Gd.maxf(it.G("w"), it.G("h"))).AsF();
    Gv ang = Gd.deg_to_rad((Gd.Dyn("float", it.C("get", Gv.S("rot"), Gv.I(0L))) + Gd.Dyn("float", it.C("get", Gv.S("dir"), Gv.I(0L)))));
    Self.C("draw_set_transform", Gd.Ctor("Vector2", cx, cy), ang, Gd.StaticGet("Vector2", "ONE"));
    Gv col = ((on).T() ? Gd.Ctor("Color", Gv.S("2fbf5b")) : Gd.Ctor("Color", Gv.S("d93a2c")));
    S_art.cyl_h(Self, Gd.Ctor("Rect2", ((-L2) / Gv.F(2.0)), Gv.I(-7L), L2, Gv.I(14L)), S_art.STAINLESS, Gv.I(6L));
    Self.C("draw_rect", Gd.Ctor("Rect2", ((-L2) / Gv.F(2.0)), Gv.I(-7L), L2, Gv.I(14L)), Gd.Ctor("Color", Gv.S("23282e")), Gv.B(false), Gv.F(1.5));
    Self.C("draw_rect", Gd.Ctor("Rect2", (((-L2) / Gv.F(2.0)) - Gv.I(6L)), Gv.I(-11L), Gv.I(6L), Gv.I(22L)), Gd.Ctor("Color", Gv.S("8a939b")));
    Gv wv = Gd.Ctor("PackedVector2Array");
    foreach (Gv i in (Gv.I(11L)).Iter()) {
      wv.C("append", Gd.Ctor("Vector2", ((((-L2) / Gv.F(2.0)) + Gv.I(8L)) + (i * Gv.F(2.0))), (Gv.F(-1.0) + (((Gv.Eq((i % Gv.I(4L)), Gv.I(1L)) ? Gv.F(-4.0) : ((Gv.Eq((i % Gv.I(4L)), Gv.I(3L)) ? Gv.F(4.0) : Gv.F(0.0))))) * Gv.F(0.6)))));
    }
    Self.C("draw_polyline", wv, Gd.Ctor("Color", Gv.S("bfe6ff")), Gv.F(2.0), Gv.B(true));
    Gv vx = ((L2 * Gv.F(0.12))).AsF();
    Self.C("draw_circle", Gd.Ctor("Vector2", vx, Gv.I(0L)), Gv.F(13.0), Gd.Ctor("Color", Gv.S("23282e")));
    Self.C("draw_circle", Gd.Ctor("Vector2", vx, Gv.I(0L)), Gv.F(11.5), S_art.grad(S_art.STAINLESS, Gv.F(0.35)));
    Gv dirv = Gd.Ctor("Vector2", Gv.I(1L), Gv.I(0L)).C("rotated", ((on).T() ? Gv.F(0.0) : ((-Gd.K("PI")) * Gv.F(0.5))));
    Self.C("draw_line", (Gd.Ctor("Vector2", vx, Gv.I(0L)) - (dirv * Gv.F(4.0))), (Gd.Ctor("Vector2", vx, Gv.I(0L)) + (dirv * Gv.F(30.0))), Gd.Ctor("Color", Gv.S("1b2026")), Gv.F(10.0), Gv.B(true));
    Self.C("draw_line", (Gd.Ctor("Vector2", vx, Gv.I(0L)) - (dirv * Gv.F(4.0))), (Gd.Ctor("Vector2", vx, Gv.I(0L)) + (dirv * Gv.F(30.0))), col, Gv.F(7.0), Gv.B(true));
    Self.C("draw_circle", Gd.Ctor("Vector2", vx, Gv.I(0L)), Gv.F(4.0), Gd.Ctor("Color", Gv.S("23282e")));
    Self.C("draw_colored_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", ((L2 / Gv.F(2.0)) - Gv.I(2L)), Gv.I(-9L)), Gd.Ctor("Vector2", ((L2 / Gv.F(2.0)) + Gv.I(12L)), Gv.I(0L)), Gd.Ctor("Vector2", ((L2 / Gv.F(2.0)) - Gv.I(2L)), Gv.I(9L)))), ((on).T() ? Gd.Ctor("Color", Gv.S("2e8bd6")) : Gd.Ctor("Color", Gv.S("6c757d"))));
    Self.C("draw_set_transform", Gd.StaticGet("Vector2", "ZERO"), Gv.I(0L), Gd.StaticGet("Vector2", "ONE"));
    Gv lab_y = (Gd.maxf((it.G("y") + it.G("h")), ((cy + (L2 / Gv.F(2.0))) + Gv.F(6.0)))).AsF();
    S_art.pill(Self, m.G("font_b"), (Gv.S("spoušť vody z řadu: ") + (((on).T() ? Gv.S("PUŠTĚNO") : Gv.S("ZAVŘENO (ťukni)")))), Gd.Ctor("Vector2", cx, (lab_y + Gv.I(24L))), Gv.I(12L), ((on).T() ? Gd.Ctor("Color", Gv.S("7fc4ff")) : Gd.Ctor("Color", Gv.S("e6ebef"))));
    return Gv.Null;
  }
  public Gv _small(Gv it = default(Gv)) {
    Gv p = Gd.Ctor("Vector2", it.G("x"), it.G("y"));
    Gv r = (Gd.deg_to_rad(Gd.Dyn("float", it.C("get", Gv.S("rot"), Gv.I(0L))))).AsF();
    Gv dark = Gd.Ctor("Color", Gv.S("23282e"));
    Gv steel = S_art.grad(S_art.STAINLESS, Gv.F(0.4));
    if (Gv.Eq(it.G("type"), Gv.S("inl"))) {
      Gv k = it.C("get", Gv.S("k"), Gv.S(""));
      Self.C("draw_set_transform", p, r, Gd.StaticGet("Vector2", "ONE"));
      {
        Gv _t1 = k;
        if (Gv.Eq(_t1, Gv.S("pruh"))) {
          _flange(Gv.I(-16L));
          _flange(Gv.I(16L));
          S_art.rrect_border(Self, Gd.Ctor("Rect2", Gv.I(-13L), Gv.I(-10L), Gv.I(26L), Gv.I(20L)), Gd.Ctor("Color", Gv.F(0.62), Gv.F(0.83), Gv.F(0.94), Gv.F(0.45)), dark, Gv.I(1L), Gv.F(4.0));
          Self.C("draw_line", Gd.Ctor("Vector2", Gv.I(-13L), Gv.I(-10L)), Gd.Ctor("Vector2", Gv.I(13L), Gv.I(10L)), dark, Gv.F(1.0));
          Self.C("draw_line", Gd.Ctor("Vector2", Gv.I(-13L), Gv.I(10L)), Gd.Ctor("Vector2", Gv.I(13L), Gv.I(-10L)), dark, Gv.F(1.0));
        }
        else if (Gv.Eq(_t1, Gv.S("zpk"))) {
          _flange(Gv.I(-14L));
          _flange(Gv.I(14L));
          _body(Gv.I(24L), Gv.I(16L));
          Self.C("draw_colored_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", Gv.I(-6L), Gv.I(-6L)), Gd.Ctor("Vector2", Gv.I(6L), Gv.I(0L)), Gd.Ctor("Vector2", Gv.I(-6L), Gv.I(6L)))), dark);
          Self.C("draw_line", Gd.Ctor("Vector2", Gv.I(7L), Gv.I(-7L)), Gd.Ctor("Vector2", Gv.I(7L), Gv.I(7L)), dark, Gv.F(2.5));
        }
        else if (Gv.Eq(_t1, Gv.S("ysit"))) {
          _flange(Gv.I(-16L));
          _flange(Gv.I(16L));
          _body(Gv.I(28L), Gv.I(14L));
          Self.C("draw_colored_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", Gv.I(-4L), Gv.I(5L)), Gd.Ctor("Vector2", Gv.I(8L), Gv.I(22L)), Gd.Ctor("Vector2", Gv.I(14L), Gv.I(18L)), Gd.Ctor("Vector2", Gv.I(2L), Gv.I(3L)))), steel);
        }
        else if (Gv.Eq(_t1, Gv.S("red"))) {
          Self.C("draw_colored_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", Gv.I(-16L), Gv.I(-9L)), Gd.Ctor("Vector2", Gv.I(16L), Gv.I(-5L)), Gd.Ctor("Vector2", Gv.I(16L), Gv.I(5L)), Gd.Ctor("Vector2", Gv.I(-16L), Gv.I(9L)))), steel);
          Self.C("draw_polyline", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", Gv.I(-16L), Gv.I(-9L)), Gd.Ctor("Vector2", Gv.I(16L), Gv.I(-5L)), Gd.Ctor("Vector2", Gv.I(16L), Gv.I(5L)), Gd.Ctor("Vector2", Gv.I(-16L), Gv.I(9L)), Gd.Ctor("Vector2", Gv.I(-16L), Gv.I(-9L)))), dark, Gv.F(1.5));
        }
        else if (Gv.Eq(_t1, Gv.S("odk"))) {
          _flange(Gv.I(-15L));
          _flange(Gv.I(15L));
          _body(Gv.I(22L), Gv.I(22L), Gd.Ctor("Color", Gv.S("6c737b")));
          Self.C("draw_string", m.G("font_b"), Gd.Ctor("Vector2", Gv.I(-4L), Gv.I(4L)), Gv.S("K"), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(10L), Gd.StaticGet("Color", "WHITE"));
        }
        else if (Gv.Eq(_t1, Gv.S("redp"))) {
          _flange(Gv.I(-14L));
          _flange(Gv.I(14L));
          Self.C("draw_colored_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", Gv.I(-11L), Gv.I(-9L)), Gd.Ctor("Vector2", Gv.I(11L), Gv.I(9L)), Gd.Ctor("Vector2", Gv.I(11L), Gv.I(-9L)))), steel);
          Self.C("draw_set_transform", p, Gv.F(0.0), Gd.StaticGet("Vector2", "ONE"));
          Self.C("draw_line", Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-2L)), Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-16L)), Gd.Ctor("Color", Gv.S("8a939b")), Gv.F(3.0));
          Self.C("draw_colored_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", Gv.I(-9L), Gv.I(-16L)), Gd.Ctor("Vector2", Gv.I(9L), Gv.I(-16L)), Gd.Ctor("Vector2", Gv.I(6L), Gv.I(-26L)), Gd.Ctor("Vector2", Gv.I(-6L), Gv.I(-26L)))), Gd.Ctor("Color", Gv.S("d93a2c")));
        }
        else if (Gv.Eq(_t1, Gv.S("aer"))) {
          _body(Gv.I(30L), Gv.I(12L), Gd.Ctor("Color", Gv.S("e8ecef")));
          foreach (Gv bx in (Gd.Arr(Gv.I(-8L), Gv.I(0L), Gv.I(8L))).Iter()) {
            Self.C("draw_circle", Gd.Ctor("Vector2", bx, (Gv.I(-10L) - ((Gv.Eq(bx, Gv.I(0L)) ? Gv.I(4L) : Gv.I(0L))))), Gv.F(2.2), Gd.Ctor("Color", Gv.S("bfe6ff")));
          }
        }
        else if (Gv.Eq(_t1, Gv.S("tc"))) {
          S_art.rrect_border(Self, Gd.Ctor("Rect2", Gv.I(-5L), Gv.I(-12L), Gv.I(10L), Gv.I(24L)), Gd.Ctor("Color", Gv.S("c9d1d9")), dark, Gv.I(1L), Gv.F(2.0));
          Self.C("draw_circle", Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-15L)), Gv.F(3.5), Gd.Ctor("Color", Gv.S("8a939b")));
        }
        else if (Gv.Eq(_t1, Gv.S("tep")) || Gv.Eq(_t1, Gv.S("man")) || Gv.Eq(_t1, Gv.S("pt")) || Gv.Eq(_t1, Gv.S("hlad")) || Gv.Eq(_t1, Gv.S("poj")) || Gv.Eq(_t1, Gv.S("odv")) || Gv.Eq(_t1, Gv.S("vzk")) || Gv.Eq(_t1, Gv.S("vyp"))) {
          Self.C("draw_set_transform", p, Gv.F(0.0), Gd.StaticGet("Vector2", "ONE"));
          _sensor(k);
        }
        else if (true) {
          _flange(Gv.I(-14L));
          _flange(Gv.I(14L));
          _body(Gv.I(24L), Gv.I(16L));
        }
      }
      Self.C("draw_set_transform", Gd.StaticGet("Vector2", "ZERO"), Gv.I(0L), Gd.StaticGet("Vector2", "ONE"));
      return Gv.Null;
    }
    if (Gv.Eq(it.G("type"), Gv.S("flow"))) {
      Self.C("draw_set_transform", p, r, Gd.StaticGet("Vector2", "ONE"));
      _flange(Gv.I(-19L));
      _flange(Gv.I(19L));
      Self.C("draw_set_transform", p, (r + (Gd.K("PI") * Gv.F(0.5))), Gd.StaticGet("Vector2", "ONE"));
      S_art.rrect_border(Self, Gd.Ctor("Rect2", Gv.I(-8L), Gv.I(-20L), Gv.I(16L), Gv.I(40L)), Gd.Ctor("Color", Gv.S("2a3b46")), Gd.Ctor("Color", Gv.S("c9d1d9")), Gv.I(1L), Gv.F(3.0));
      S_art.rrect(Self, Gd.Ctor("Rect2", Gv.I(-5L), Gv.I(-17L), Gv.I(10L), Gv.I(34L)), Gd.Ctor("Color", Gv.F(0.62), Gv.F(0.83), Gv.F(0.94), Gv.F(0.35)), Gv.F(2.0));
      foreach (Gv v in (Gd.Arr(Gv.I(-12L), Gv.I(-6L), Gv.I(0L), Gv.I(6L), Gv.I(12L))).Iter()) {
        Self.C("draw_line", Gd.Ctor("Vector2", Gv.I(-5L), v), Gd.Ctor("Vector2", Gv.I(-1L), v), Gd.Ctor("Color", Gv.S("c9d1d9")), Gv.F(1.0));
      }
      Self.C("draw_colored_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", Gv.I(-4L), Gv.I(2L)), Gd.Ctor("Vector2", Gv.I(4L), Gv.I(2L)), Gd.Ctor("Vector2", Gv.I(0L), Gv.I(8L)))), Gd.Ctor("Color", Gv.S("e0452f")));
      Self.C("draw_set_transform", Gd.StaticGet("Vector2", "ZERO"), Gv.I(0L), Gd.StaticGet("Vector2", "ONE"));
      S_art.pill(Self, m.G("font"), it.C("get", Gv.S("label"), Gv.S("")), (p + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-34L))), Gv.I(11L));
      return Gv.Null;
    }
    if (Gv.Eq(it.G("type"), Gv.S("hopf"))) {
      Self.C("draw_set_transform", p, r, Gd.StaticGet("Vector2", "ONE"));
      _flange(Gv.I(-23L));
      _flange(Gv.I(23L));
      Self.C("draw_set_transform", p, Gv.F(0.0), Gd.StaticGet("Vector2", "ONE"));
      S_art.rrect_border(Self, Gd.Ctor("Rect2", Gv.I(-20L), Gv.I(-46L), Gv.I(40L), Gv.I(62L)), steel, dark, Gv.I(2L), Gv.F(6.0));
      S_art.rrect_border(Self, Gd.Ctor("Rect2", Gv.I(-23L), Gv.I(-52L), Gv.I(46L), Gv.I(9L)), Gd.Ctor("Color", Gv.S("8a939b")), dark, Gv.I(1L), Gv.F(3.0));
      S_art.rrect_border(Self, Gd.Ctor("Rect2", Gv.I(-6L), Gv.I(-60L), Gv.I(12L), Gv.I(8L)), Gd.Ctor("Color", Gv.S("6c737b")), dark, Gv.I(1L), Gv.F(2.0));
      S_art.rrect_border(Self, Gd.Ctor("Rect2", Gv.I(-13L), Gv.I(-38L), Gv.I(26L), Gv.I(46L)), Gd.Ctor("Color", Gv.S("2a3a22")), Gd.Ctor("Color", Gv.S("6f8f3a")), Gv.I(1L), Gv.F(3.0));
      foreach (Gv v_2 in (Gd.Arr(Gv.I(-30L), Gv.I(-22L), Gv.I(-14L), Gv.I(-6L), Gv.I(2L))).Iter()) {
        Self.C("draw_line", Gd.Ctor("Vector2", Gv.I(-13L), v_2), Gd.Ctor("Vector2", Gv.I(13L), v_2), Gd.Ctor("Color", Gv.S("6f8f3a")), Gv.F(0.8));
      }
      foreach (Gv v_3 in (Gd.Arr(Gv.I(-7L), Gv.I(0L), Gv.I(7L))).Iter()) {
        Self.C("draw_line", Gd.Ctor("Vector2", v_3, Gv.I(-38L)), Gd.Ctor("Vector2", v_3, Gv.I(8L)), Gd.Ctor("Color", Gv.S("6f8f3a")), Gv.F(0.8));
      }
      Self.C("draw_circle", Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-24L)), Gv.F(5.0), Gd.Ctor("Color", Gv.S("8fbf3f")));
      Self.C("draw_circle", Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-8L)), Gv.F(4.5), Gd.Ctor("Color", Gv.S("8fbf3f")));
      Self.C("draw_rect", Gd.Ctor("Rect2", Gv.I(-4L), Gv.I(16L), Gv.I(8L), Gv.I(8L)), Gd.Ctor("Color", Gv.S("6c737b")));
      Self.C("draw_set_transform", Gd.StaticGet("Vector2", "ZERO"), Gv.I(0L), Gd.StaticGet("Vector2", "ONE"));
      S_art.pill(Self, m.G("font"), it.C("get", Gv.S("label"), Gv.S("")), (p + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-68L))), Gv.I(11L));
      return Gv.Null;
    }
    Self.C("draw_circle", p, Gv.F(14.0), Gd.Ctor("Color", Gv.S("0d1013")));
    Self.C("draw_circle", p, Gv.F(12.0), Gd.Ctor("Color", Gv.S("c7cfd6")));
    Self.C("draw_circle", p, Gv.F(6.0), Gd.Ctor("Color", Gv.S("6c757d")));
    return Gv.Null;
  }
  public Gv _flange(Gv dx = default(Gv)) {
    dx = (dx).AsF();
    Self.C("draw_rect", Gd.Ctor("Rect2", (dx - Gv.F(2.5)), Gv.I(-10L), Gv.I(5L), Gv.I(20L)), Gd.Ctor("Color", Gv.S("8a939b")));
    Self.C("draw_rect", Gd.Ctor("Rect2", (dx - Gv.F(2.5)), Gv.I(-10L), Gv.I(5L), Gv.I(20L)), Gd.Ctor("Color", Gv.S("23282e")), Gv.B(false), Gv.F(1.0));
    return Gv.Null;
  }
  public Gv _body(Gv w = default(Gv), Gv h = default(Gv), Gv col = default(Gv)) {
    w = (w).AsF();
    h = (h).AsF();
    if (col.k == 0) col = Gd.Ctor("Color", Gv.S("c7cfd6"));
    S_art.rrect_border(Self, Gd.Ctor("Rect2", ((-w) / Gv.F(2.0)), ((-h) / Gv.F(2.0)), w, h), col, Gd.Ctor("Color", Gv.S("23282e")), Gv.I(1L), Gv.F(3.0));
    return Gv.Null;
  }
  public Gv _sensor(Gv k = default(Gv)) {
    Gv dark = Gd.Ctor("Color", Gv.S("23282e"));
    Gv grey = Gd.Ctor("Color", Gv.S("8a939b"));
    {
      Gv _t1 = k;
      if (Gv.Eq(_t1, Gv.S("tep")) || Gv.Eq(_t1, Gv.S("man"))) {
        Self.C("draw_line", Gd.Ctor("Vector2", Gv.I(0L), Gv.I(0L)), Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-14L)), grey, Gv.F(3.0));
        Self.C("draw_circle", Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-24L)), Gv.F(11.0), dark);
        Self.C("draw_circle", Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-24L)), Gv.F(9.5), Gd.StaticGet("Color", "WHITE"));
        Self.C("draw_line", Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-24L)), Gd.Ctor("Vector2", (Gv.Eq(k, Gv.S("tep")) ? Gv.I(5L) : Gv.I(-5L)), Gv.I(-30L)), (Gv.Eq(k, Gv.S("tep")) ? Gd.Ctor("Color", Gv.S("d93a2c")) : dark), Gv.F(2.0));
        Self.C("draw_string", m.G("font"), Gd.Ctor("Vector2", Gv.I(-7L), Gv.I(-19L)), (Gv.Eq(k, Gv.S("tep")) ? Gv.S("°C") : Gv.S("bar")), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(7L), dark);
      }
      else if (Gv.Eq(_t1, Gv.S("pt"))) {
        Self.C("draw_line", Gd.Ctor("Vector2", Gv.I(0L), Gv.I(4L)), Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-18L)), grey, Gv.F(3.0));
        S_art.rrect_border(Self, Gd.Ctor("Rect2", Gv.I(-7L), Gv.I(-30L), Gv.I(14L), Gv.I(12L)), Gd.Ctor("Color", Gv.S("2e7bd6")), Gd.Ctor("Color", Gv.S("0d2c52")), Gv.I(1L), Gv.F(2.0));
        Self.C("draw_string", m.G("font"), Gd.Ctor("Vector2", Gv.I(-5L), Gv.I(-21L)), Gv.S("PT"), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(7L), Gd.StaticGet("Color", "WHITE"));
      }
      else if (Gv.Eq(_t1, Gv.S("hlad"))) {
        S_art.rrect_border(Self, Gd.Ctor("Rect2", Gv.I(-5L), Gv.I(-40L), Gv.I(10L), Gv.I(42L)), Gd.Ctor("Color", Gv.F(0.62), Gv.F(0.83), Gv.F(0.94), Gv.F(0.5)), dark, Gv.I(1L), Gv.F(3.0));
        Self.C("draw_rect", Gd.Ctor("Rect2", Gv.I(-3L), Gv.I(-18L), Gv.I(6L), Gv.I(18L)), Gd.Ctor("Color", Gv.F(0.18), Gv.F(0.48), Gv.F(0.84), Gv.F(0.7)));
      }
      else if (Gv.Eq(_t1, Gv.S("poj"))) {
        Self.C("draw_line", Gd.Ctor("Vector2", Gv.I(0L), Gv.I(0L)), Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-10L)), grey, Gv.F(3.0));
        S_art.rrect_border(Self, Gd.Ctor("Rect2", Gv.I(-8L), Gv.I(-18L), Gv.I(16L), Gv.I(8L)), Gd.Ctor("Color", Gv.S("d93a2c")), Gd.Ctor("Color", Gv.S("5a1410")), Gv.I(1L), Gv.F(1.0));
        Self.C("draw_line", Gd.Ctor("Vector2", Gv.I(-4L), Gv.I(-18L)), Gd.Ctor("Vector2", Gv.I(-4L), Gv.I(-28L)), Gd.Ctor("Color", Gv.S("5a1410")), Gv.F(2.0));
        Self.C("draw_line", Gd.Ctor("Vector2", Gv.I(4L), Gv.I(-18L)), Gd.Ctor("Vector2", Gv.I(4L), Gv.I(-28L)), Gd.Ctor("Color", Gv.S("5a1410")), Gv.F(2.0));
      }
      else if (Gv.Eq(_t1, Gv.S("odv"))) {
        Self.C("draw_line", Gd.Ctor("Vector2", Gv.I(0L), Gv.I(0L)), Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-12L)), grey, Gv.F(3.0));
        Self.C("draw_circle", Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-17L)), Gv.F(6.0), S_art.grad(S_art.STAINLESS, Gv.F(0.4)));
        Self.C("draw_line", Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-23L)), Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-31L)), Gd.Ctor("Color", Gv.S("8fd8ff")), Gv.F(1.8));
      }
      else if (Gv.Eq(_t1, Gv.S("vzk")) || Gv.Eq(_t1, Gv.S("vyp"))) {
        Gv down = Gv.B(Gv.Eq(k, Gv.S("vyp")));
        Self.C("draw_line", Gd.Ctor("Vector2", Gv.I(0L), Gv.I(0L)), Gd.Ctor("Vector2", Gv.I(0L), ((down).T() ? Gv.I(16L) : Gv.I(-12L))), grey, Gv.F(4.0));
        Self.C("draw_circle", Gd.Ctor("Vector2", Gv.I(0L), ((down).T() ? Gv.I(18L) : Gv.I(-14L))), Gv.F(5.0), S_art.grad(S_art.STAINLESS, Gv.F(0.4)));
        S_art.rrect(Self, Gd.Ctor("Rect2", ((down).T() ? Gv.I(-2L) : Gv.I(0L)), ((down).T() ? Gv.I(20L) : Gv.I(-20L)), ((down).T() ? Gv.I(4L) : Gv.I(12L)), ((down).T() ? Gv.I(10L) : Gv.I(4L))), Gd.Ctor("Color", Gv.S("d93a2c")), Gv.F(2.0));
      }
    }
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "fx": return this.fx;
    case "spin": return this.spin;
    case "m": return this.m;
    case "live_only": return this.live_only;
    case "skip_valves": return this.skip_valves;
    case "valves_only": return this.valves_only;
    case "clip": return this.clip;
    case "gpu_node": return this.gpu_node;
    case "LQ_T": return S_main_ItemLayer.LQ_T;
    case "LQ_F": return S_main_ItemLayer.LQ_F;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "fx": this.fx = v; return v;
    case "spin": this.spin = v; return v;
    case "m": this.m = v; return v;
    case "live_only": this.live_only = v; return v;
    case "skip_valves": this.skip_valves = v; return v;
    case "valves_only": this.valves_only = v; return v;
    case "clip": this.clip = v; return v;
    case "gpu_node": this.gpu_node = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "_near": return _near(Gd.A(a, 0));
    case "_draw": return _draw();
    case "_hatch_live": return _hatch_live(Gd.A(a, 0));
    case "_splid_live": return _splid_live(Gd.A(a, 0));
    case "_live_static": return _live_static();
    case "_baked": return _baked();
    case "_vessel": return _vessel(Gd.A(a, 0));
    case "_lid_fit": return _lid_fit(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "_vdetails": return _vdetails(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6));
    case "_plate": return _plate(Gd.A(a, 0));
    case "_chiller": return _chiller(Gd.A(a, 0));
    case "_malt_gate": return _malt_gate(Gd.A(a, 0));
    case "_valve": return _valve(Gd.A(a, 0));
    case "_hose_vis": return _hose_vis(Gd.A(a, 0));
    case "_hose": return _hose(Gd.A(a, 0));
    case "_outlet": return _outlet(Gd.A(a, 0));
    case "_pump": return _pump(Gd.A(a, 0));
    case "_spray": return _spray(Gd.A(a, 0));
    case "_nozzle": return _nozzle(Gd.A(a, 0));
    case "_panel": return _panel(Gd.A(a, 0));
    case "_manifold": return _manifold(Gd.A(a, 0));
    case "_gauge": return _gauge(Gd.A(a, 0));
    case "_gauge_cocks": return _gauge_cocks(Gd.A(a, 0));
    case "_sw": return _sw(Gd.A(a, 0));
    case "_deco": return _deco(Gd.A(a, 0));
    case "_internals": return _internals(Gd.A(a, 0));
    case "_cip_pipe": return _cip_pipe(Gd.A(a, 0));
    case "_rake": return _rake(Gd.A(a, 0));
    case "_dip": return _dip(Gd.A(a, 0));
    case "_gpu_body": return _gpu_body();
    case "_gpu_clear": return _gpu_clear();
    case "_liquid": return _liquid(Gd.A(a, 0));
    case "_hop_fall": return _hop_fall(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5));
    case "_steam": return _steam(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4));
    case "_grain_bed": return _grain_bed(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6));
    case "_lq_f": return _lq_f(Gd.A(a, 0));
    case "_lq_c": return _lq_c(Gd.A(a, 0), Gd.A(a, 1));
    case "_shade_liquid": return _shade_liquid(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6), Gd.A(a, 7), Gd.A(a, 8));
    case "_area": return _area(Gd.A(a, 0));
    case "_gauge_levels": return _gauge_levels(Gd.A(a, 0));
    case "_vstatus": return _vstatus(Gd.A(a, 0));
    case "_dial_at": return _dial_at(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "_inlet": return _inlet(Gd.A(a, 0));
    case "_small": return _small(Gd.A(a, 0));
    case "_flange": return _flange(Gd.A(a, 0));
    case "_body": return _body(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "_sensor": return _sensor(Gd.A(a, 0));
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    case "LQ_T": return LQ_T;
    case "LQ_F": return LQ_F;
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_main_ItemLayer(); return Gv.O(o); }
  public override string ScriptName { get { return "S_main_ItemLayer"; } }
}
public partial class S_main_Hud : GdNode {
  public Gv m;
  public Gv info;
  public Gv msg;
  public Gv toast_box;
  public Gv upd_btn;
  public Gv warn_btn;
  public Gv cab_btn;
  public Gv hard_btn;
  public Gv big_btn;
  public Gv pan_btn;
  public Gv play_btn;
  public Gv sp_btns;
  public Gv msg_t;
  static S_main_Hud() {
  }
  public S_main_Hud() {
    TypeName = "CanvasLayer";
    m = Gv.Null;
    info = Gv.Null;
    msg = Gv.Null;
    toast_box = Gv.Null;
    upd_btn = Gv.Null;
    warn_btn = Gv.Null;
    cab_btn = Gv.Null;
    hard_btn = Gv.Null;
    big_btn = Gv.Null;
    pan_btn = Gv.Null;
    play_btn = Gv.Null;
    sp_btns = Gd.Dict();
    msg_t = Gv.F(0.0);
  }
  public Gv ui_sync() {
    if (Gv.Ne(hard_btn, Gv.Null)) {
      hard_btn = hard_btn.S("text", ((m.G("hard")).T() ? Gv.S("HARD") : Gv.S("SOFT")));
    }
    if (Gv.Ne(cab_btn, Gv.Null)) {
      cab_btn.C("set_pressed_no_signal", m.G("dock_on"));
    }
    if (Gv.Ne(pan_btn, Gv.Null)) {
      pan_btn.C("set_pressed_no_signal", m.G("panel_on"));
    }
    if (Gv.Ne(big_btn, Gv.Null)) {
      big_btn.C("set_pressed_no_signal", Gv.B(((m.G("dock_big")).T() && (m.G("dock_on")).T())));
    }
    return Gv.Null;
  }
  public Gv _ready() {
    Gv th = Gd.New("Theme");
    th = th.S("default_font", m.G("font_b"));
    th = th.S("default_font_size", Gv.I(13L));
    Gv sb_n = Gd.New("StyleBoxFlat");
    sb_n = sb_n.S("bg_color", Gd.Ctor("Color", Gv.S("262d35")));
    sb_n.C("set_corner_radius_all", Gv.I(7L));
    sb_n = sb_n.S("content_margin_left", Gv.I(8L));
    sb_n = sb_n.S("content_margin_right", Gv.I(8L));
    sb_n = sb_n.S("content_margin_top", Gv.I(6L));
    sb_n = sb_n.S("content_margin_bottom", Gv.I(6L));
    sb_n = sb_n.S("border_color", Gd.Ctor("Color", Gv.S("3a444e")));
    sb_n.C("set_border_width_all", Gv.I(1L));
    Gv sb_h = sb_n.C("duplicate");
    sb_h = sb_h.S("bg_color", Gd.Ctor("Color", Gv.S("333c46")));
    sb_h = sb_h.S("border_color", Gd.Ctor("Color", Gv.S("f2c230")));
    Gv sb_p = sb_n.C("duplicate");
    sb_p = sb_p.S("bg_color", Gd.Ctor("Color", Gv.S("f2c230")));
    th.C("set_stylebox", Gv.S("normal"), Gv.S("Button"), sb_n);
    th.C("set_stylebox", Gv.S("hover"), Gv.S("Button"), sb_h);
    th.C("set_stylebox", Gv.S("pressed"), Gv.S("Button"), sb_p);
    th.C("set_stylebox", Gv.S("focus"), Gv.S("Button"), Gd.New("StyleBoxEmpty"));
    th.C("set_color", Gv.S("font_color"), Gv.S("Button"), Gd.Ctor("Color", Gv.S("e6ebef")));
    th.C("set_color", Gv.S("font_hover_color"), Gv.S("Button"), Gd.Ctor("Color", Gv.S("ffffff")));
    th.C("set_color", Gv.S("font_pressed_color"), Gv.S("Button"), Gd.Ctor("Color", Gv.S("1b2026")));
    Gv top = Gd.New("PanelContainer");
    Gv tsb = Gd.New("StyleBoxFlat");
    tsb = tsb.S("bg_color", Gd.Ctor("Color", Gv.S("0e1216")));
    tsb = tsb.S("border_color", Gd.Ctor("Color", Gv.S("f2c230")));
    tsb = tsb.S("border_width_bottom", Gv.I(2L));
    tsb = tsb.S("content_margin_left", Gv.I(14L));
    tsb = tsb.S("content_margin_right", Gv.I(14L));
    tsb = tsb.S("content_margin_top", Gv.I(8L));
    tsb = tsb.S("content_margin_bottom", Gv.I(8L));
    tsb = tsb.S("shadow_color", Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.35)));
    tsb = tsb.S("shadow_size", Gv.I(6L));
    tsb = tsb.S("shadow_offset", Gd.Ctor("Vector2", Gv.I(0L), Gv.I(2L)));
    top.C("add_theme_stylebox_override", Gv.S("panel"), tsb);
    top = top.S("theme", th);
    top = top.S("anchor_right", Gv.F(1.0));
    top = top.S("offset_right", Gv.I(0L));
    Self.C("add_child", top);
    Gv bar = Gd.New("HBoxContainer");
    bar.C("add_theme_constant_override", Gv.S("separation"), Gv.I(5L));
    top.C("add_child", bar);
    Gv logo = Gd.New("Panel");
    logo = logo.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(30L), Gv.I(30L)));
    logo = logo.S("size_flags_vertical", Gd.StaticGet("Control", "SIZE_SHRINK_CENTER"));
    Gv lsb = Gd.New("StyleBoxFlat");
    lsb = lsb.S("bg_color", Gd.Ctor("Color", Gv.S("f2c230")));
    lsb.C("set_corner_radius_all", Gv.I(15L));
    logo.C("add_theme_stylebox_override", Gv.S("panel"), lsb);
    bar.C("add_child", logo);
    Gv ll = Gd.New("Label");
    ll = ll.S("text", Gv.S("Z"));
    ll.C("add_theme_color_override", Gv.S("font_color"), Gd.Ctor("Color", Gv.S("15191d")));
    ll.C("add_theme_font_size_override", Gv.S("font_size"), Gv.I(18L));
    ll = ll.S("horizontal_alignment", Gd.K("HORIZONTAL_ALIGNMENT_CENTER"));
    ll = ll.S("vertical_alignment", Gd.K("VERTICAL_ALIGNMENT_CENTER"));
    ll.C("set_anchors_preset", Gd.StaticGet("Control", "PRESET_FULL_RECT"));
    logo.C("add_child", ll);
    Gv tv = Gd.New("VBoxContainer");
    tv.C("add_theme_constant_override", Gv.S("separation"), Gv.I(-4L));
    bar.C("add_child", tv);
    Gv title = Gd.New("Label");
    title = title.S("text", Gv.S("TRENAŽÉR VARNY"));
    title.C("add_theme_color_override", Gv.S("font_color"), Gd.Ctor("Color", Gv.S("f2c230")));
    title.C("add_theme_font_size_override", Gv.S("font_size"), Gv.I(18L));
    tv.C("add_child", title);
    Gv sub = Gd.New("Label");
    sub = sub.S("text", Gv.S("Pivovar Zajíc · 40 hl"));
    sub.C("add_theme_color_override", Gv.S("font_color"), Gd.Ctor("Color", Gv.S("7b858e")));
    sub.C("add_theme_font_size_override", Gv.S("font_size"), Gv.I(11L));
    tv.C("add_child", sub);
    Gv sp = Gd.New("Control");
    sp = sp.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(18L), Gv.I(0L)));
    bar.C("add_child", sp);
    play_btn = Gd.New("Button");
    play_btn = play_btn.S("toggle_mode", Gv.B(true));
    play_btn = play_btn.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(38L), Gv.I(0L)));
    play_btn = play_btn.S("tooltip_text", Gv.S("Spustit / pozastavit čas várky (mezerník)"));
    bar.C("add_child", play_btn);
    play_btn.G("pressed").C("connect", Gd.Fn((_a) => {  m = m.S("running", Gv.B(!(m.G("running")).T())); if (Gv.Ne(m.G("game"), Gv.Null)) {   m.G("game").C("play", Gv.S("click")); } update_info(); return Gv.Null; }));
    foreach (Gv spd in (Gd.Arr(Gv.I(1L), Gv.I(10L), Gv.I(60L), Gv.I(180L))).Iter()) {
      Gv sb2 = Gd.New("Button");
      sb2 = sb2.S("toggle_mode", Gv.B(true));
      sb2 = sb2.S("text", (Gv.S("%d×") % spd));
      sb2 = sb2.S("tooltip_text", (Gv.S("Rychlost času x%d (klávesa %d)") % Gd.Arr(spd, (Gd.Arr(Gv.I(1L), Gv.I(10L), Gv.I(60L), Gv.I(180L)).C("find", spd) + Gv.I(1L)))));
      bar.C("add_child", sb2);
      sb2.G("pressed").C("connect", Gd.Fn((_a) => {  m = m.S("speed", Gd.Dyn("float", spd)); m = m.S("running", Gv.B(true)); update_info(); return Gv.Null; }));
      sp_btns = sp_btns.SI(spd, sb2);
    }
    Gv rst = Gd.New("Button");
    rst = rst.S("text", Gv.S("↺ Reset"));
    rst = rst.S("tooltip_text", Gv.S("Začít úplně od nuly: nová várka, všechno prázdné a klapky zavřené (tvoje úpravy plánu zůstanou)"));
    bar.C("add_child", rst);
    Gv rdlg = Gd.New("ConfirmationDialog");
    rdlg = rdlg.S("title", Gv.S("Reset várky"));
    rdlg = rdlg.S("dialog_text", Gv.S("Opravdu začít úplně od nuly?\nUložená várka se smaže (úpravy trubek v editoru zůstanou)."));
    rdlg = rdlg.S("ok_button_text", Gv.S("Ano, od nuly"));
    rdlg = rdlg.S("cancel_button_text", Gv.S("Zrušit"));
    rdlg.G("confirmed").C("connect", Gd.Fn((_a) => {  m.C("new_game"); return Gv.Null; }));
    Self.C("add_child", rdlg);
    rst.G("pressed").C("connect", Gd.Fn((_a) => {  rdlg.C("popup_centered"); return Gv.Null; }));
    Gv sp2 = Gd.New("Control");
    sp2 = sp2.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(6L), Gv.I(0L)));
    bar.C("add_child", sp2);
    foreach (Gv pair in (Gd.Arr(Gd.Arr(Gv.S("Celé"), Gv.S("")), Gd.Arr(Gv.S("Varna"), Gv.S("rvp")), Gd.Arr(Gv.S("Scezovačka"), Gv.S("sk")), Gd.Arr(Gv.S("Whirlpool"), Gv.S("wp")), Gd.Arr(Gv.S("Bojler"), Gv.S("hlt")), Gd.Arr(Gv.S("Ledovka"), Gv.S("lv")), Gd.Arr(Gv.S("Spilka"), Gv.S("spilka")), Gd.Arr(Gv.S("Kateta"), Gv.S("kat")))).Iter()) {
      Gv b = Gd.New("Button");
      b = b.S("text", pair.I(Gv.I(0L)));
      bar.C("add_child", b);
      Gv bind = pair.I(Gv.I(1L));
      b.G("pressed").C("connect", Gd.Fn((_a) => {  _go(bind); return Gv.Null; }));
    }
    Gv grow = Gd.New("Control");
    grow = grow.S("size_flags_horizontal", Gd.StaticGet("Control", "SIZE_EXPAND_FILL"));
    bar.C("add_child", grow);
    warn_btn = Gd.New("Button");
    warn_btn = warn_btn.S("visible", Gv.B(false));
    warn_btn = warn_btn.S("tooltip_text", Gv.S("Klepni – ukážu rozvaděč"));
    Gv wsb = Gd.New("StyleBoxFlat");
    wsb = wsb.S("bg_color", Gd.Ctor("Color", Gv.S("5a1712")));
    wsb = wsb.S("border_color", Gd.Ctor("Color", Gv.S("ff5a4a")));
    wsb.C("set_border_width_all", Gv.I(1L));
    wsb.C("set_corner_radius_all", Gv.I(7L));
    wsb = wsb.S("content_margin_left", Gv.I(10L));
    wsb = wsb.S("content_margin_right", Gv.I(10L));
    wsb = wsb.S("content_margin_top", Gv.I(6L));
    wsb = wsb.S("content_margin_bottom", Gv.I(6L));
    foreach (Gv st in (Gd.Arr(Gv.S("normal"), Gv.S("hover"), Gv.S("pressed"))).Iter()) {
      warn_btn.C("add_theme_stylebox_override", st, wsb);
    }
    warn_btn.C("add_theme_color_override", Gv.S("font_color"), Gd.Ctor("Color", Gv.S("ffd6d1")));
    warn_btn.C("add_theme_color_override", Gv.S("font_hover_color"), Gd.StaticGet("Color", "WHITE"));
    warn_btn.G("pressed").C("connect", Gd.Fn((_a) => {  if (!(m.G("dock_on")).T()) {   m.C("toggle_dock"); } else {   _go(Gv.S("rozv")); } return Gv.Null; }));
    bar.C("add_child", warn_btn);
    cab_btn = Gd.New("Button");
    cab_btn = cab_btn.S("toggle_mode", Gv.B(true));
    cab_btn = cab_btn.S("text", Gv.S("Rozvaděč"));
    cab_btn = cab_btn.S("tooltip_text", Gv.S("Rozvaděč RT.20 vpravo (R)"));
    bar.C("add_child", cab_btn);
    cab_btn = cab_btn.S("visible", Gv.B(false));
    big_btn = Gd.New("Button");
    big_btn = big_btn.S("toggle_mode", Gv.B(true));
    big_btn = big_btn.S("text", Gv.S("⤢"));
    big_btn = big_btn.S("tooltip_text", Gv.S("Zvětšit rozvaděč (Z). Kolečko posouvá, Ctrl+kolečko přibližuje, FM se točí táhnutím."));
    bar.C("add_child", big_btn);
    big_btn.G("pressed").C("connect", Gd.Fn((_a) => {  m.C("toggle_dock_big"); return Gv.Null; }));
    hard_btn = Gd.New("Button");
    hard_btn = hard_btn.S("text", Gv.S("SOFT"));
    hard_btn = hard_btn.S("tooltip_text", Gv.S("Soft: červená/zelená klapka a viditelné toky. Hard: modré klapky, toky v trubkách nejsou vidět, hladina jen měřením (J)"));
    bar.C("add_child", hard_btn);
    hard_btn.G("pressed").C("connect", Gd.Fn((_a) => {  m.C("toggle_hard"); return Gv.Null; }));
    pan_btn = Gd.New("Button");
    pan_btn = pan_btn.S("toggle_mode", Gv.B(true));
    pan_btn = pan_btn.S("text", Gv.S("☰ Panel"));
    pan_btn = pan_btn.S("tooltip_text", Gv.S("Ovládací panel s nastavením, postupem a hlášeními"));
    bar.C("add_child", pan_btn);
    pan_btn.G("pressed").C("connect", Gd.Fn((_a) => {  m.C("toggle_panel"); return Gv.Null; }));
    upd_btn = Gd.New("Button");
    upd_btn = upd_btn.S("text", Gv.S("⟳ Aktualizovat"));
    upd_btn = upd_btn.S("tooltip_text", Gv.S("Stáhne nejnovější verzi trenažéru z internetu a restartuje ho"));
    bar.C("add_child", upd_btn);
    upd_btn.G("pressed").C("connect", Gd.Fn((_a) => {  m.G("updater").C("check", Gv.B(true)); return Gv.Null; }));
    Gv edb = Gd.New("Button");
    edb = edb.S("text", Gv.S("✎ Editor"));
    edb = edb.S("tooltip_text", Gv.S("Úpravy trubek, klapek a dílů (E)"));
    bar.C("add_child", edb);
    edb.G("pressed").C("connect", Gd.Fn((_a) => {  m.C("open_editor"); return Gv.Null; }));
    Gv b3 = Gd.New("Button");
    b3 = b3.S("text", Gv.S("🎥 3D"));
    b3 = b3.S("tooltip_text", Gv.S("3D pohled na varnu (V)"));
    bar.C("add_child", b3);
    b3.G("pressed").C("connect", Gd.Fn((_a) => {  m.C("toggle_3d"); return Gv.Null; }));
    Gv chip = Gd.New("PanelContainer");
    chip = chip.S("size_flags_vertical", Gd.StaticGet("Control", "SIZE_SHRINK_CENTER"));
    Gv csb = Gd.New("StyleBoxFlat");
    csb = csb.S("bg_color", Gd.Ctor("Color", Gv.S("1b2229")));
    csb.C("set_corner_radius_all", Gv.I(14L));
    csb = csb.S("border_color", Gd.Ctor("Color", Gv.S("2f3943")));
    csb.C("set_border_width_all", Gv.I(1L));
    csb = csb.S("content_margin_left", Gv.I(12L));
    csb = csb.S("content_margin_right", Gv.I(12L));
    csb = csb.S("content_margin_top", Gv.I(4L));
    csb = csb.S("content_margin_bottom", Gv.I(4L));
    chip.C("add_theme_stylebox_override", Gv.S("panel"), csb);
    bar.C("add_child", chip);
    info = Gd.New("Label");
    info.C("add_theme_color_override", Gv.S("font_color"), Gd.Ctor("Color", Gv.S("c9d1d8")));
    info.C("add_theme_font_size_override", Gv.S("font_size"), Gv.I(14L));
    chip.C("add_child", info);
    toast_box = Gd.New("PanelContainer");
    toast_box = toast_box.S("position", Gd.Ctor("Vector2", Gv.I(16L), Gv.I(66L)));
    toast_box = toast_box.S("visible", Gv.B(false));
    Gv msb = Gd.New("StyleBoxFlat");
    msb = msb.S("bg_color", Gd.Ctor("Color", Gv.F(0.07), Gv.F(0.085), Gv.F(0.1), Gv.F(0.9)));
    msb.C("set_corner_radius_all", Gv.I(10L));
    msb = msb.S("border_color", Gd.Ctor("Color", Gv.S("f2c230")));
    msb = msb.S("border_width_left", Gv.I(4L));
    msb = msb.S("content_margin_left", Gv.I(14L));
    msb = msb.S("content_margin_right", Gv.I(14L));
    msb = msb.S("content_margin_top", Gv.I(8L));
    msb = msb.S("content_margin_bottom", Gv.I(8L));
    msb = msb.S("shadow_color", Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.F(0.3)));
    msb = msb.S("shadow_size", Gv.I(8L));
    toast_box.C("add_theme_stylebox_override", Gv.S("panel"), msb);
    Self.C("add_child", toast_box);
    msg = Gd.New("Label");
    msg.C("add_theme_color_override", Gv.S("font_color"), Gd.Ctor("Color", Gv.S("f5f7f9")));
    msg.C("add_theme_font_override", Gv.S("font"), m.G("font_b"));
    msg.C("add_theme_font_size_override", Gv.S("font_size"), Gv.I(15L));
    toast_box.C("add_child", msg);
    return Gv.Null;
  }
  public Gv _go(Gv bind = default(Gv)) {
    if (Gv.Eq(bind, Gv.S(""))) {
      m.C("go_to", m.G("bounds"), Gv.B(true));
      return Gv.Null;
    }
    if (Gv.Eq(bind, Gv.S("rozv"))) {
      if (Gv.Ne(m.G("cabinet"), Gv.Null)) {
        Gv cs = (m.G("cabinet").G("sc")).AsF();
        m.C("go_to", Gd.Ctor("Rect2", (m.G("cabinet").G("it").G("x") - Gv.F(30.0)), m.G("cabinet").G("it").G("y"), ((Gv.F(770.0) * cs) + Gv.F(60.0)), (Gv.F(1130.0) * cs)).C("grow", Gv.I(40L)), Gv.B(true));
      }
      return Gv.Null;
    }
    if (Gv.Eq(bind, Gv.S("kat"))) {
      if ((m.G("outlets").C("size")).T()) {
        Gv r = Gd.Ctor("Rect2", m.G("outlets").I(Gv.I(0L)).G("front"), Gd.StaticGet("Vector2", "ZERO"));
        foreach (Gv o in (m.G("outlets")).Iter()) {
          r = r.C("expand", o.G("front"));
        }
        m.C("go_to", r.C("grow", Gv.I(220L)), Gv.B(true));
      }
      return Gv.Null;
    }
    Gv v = m.C("vessel_by_bind", bind);
    if (Gv.Ne(v, Gv.Null)) {
      m.C("go_to", Gd.Ctor("Rect2", v.G("x"), v.G("y"), v.C("get", Gv.S("w"), Gv.I(200L)), v.C("get", Gv.S("h"), Gv.I(200L))).C("grow", Gv.I(260L)), Gv.B(true));
    }
    return Gv.Null;
  }
  public Gv toast(Gv t = default(Gv)) {
    msg = msg.S("autowrap_mode", Gd.StaticGet("TextServer", "AUTOWRAP_WORD_SMART"));
    msg = msg.S("custom_minimum_size", Gd.Ctor("Vector2", Gd.minf((m.G("font_b").C("get_string_size", t, Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(15L)).G("x") + Gv.F(6.0)), Gv.F(620.0)), Gv.I(0L)));
    msg = msg.S("text", t);
    msg_t = (Gv.F(3.0) + (t.C("length") / Gv.F(40.0)));
    toast_box = toast_box.S("visible", Gv.B(Gv.Ne(t, Gv.S(""))));
    toast_box.C("reset_size");
    return Gv.Null;
  }
  public Gv update_info() {
    Gv t = (m.G("sim").G("S").G("t")).AsF();
    Gv run = ((m.G("running")).T() ? ((Gv.S("▶  x%d") % Gd.Dyn("int", m.G("speed")))) : Gv.S("⏸  pauza"));
    Gv fps = (Gv.In(Gv.S("--fps"), Gd.Static("OS", "get_cmdline_user_args")) ? ((Gv.S("   ·   %d fps") % Gd.Static("Engine", "get_frames_per_second"))) : Gv.S(""));
    info = info.S("text", (Gv.S("%s   ·   %d:%02d:%02d%s") % Gd.Arr(run, Gd.Dyn("int", (t / Gv.I(3600L))), (Gd.Dyn("int", (t / Gv.I(60L))) % Gv.I(60L)), (Gd.Dyn("int", t) % Gv.I(60L)), fps)));
    info = info.S("tooltip_text", (Gv.S("zoom %d %%") % Gd.roundi((m.G("cam").G("zoom").G("x") * Gv.F(100.0)))));
    if (Gv.Ne(play_btn, Gv.Null)) {
      Gv pt = ((m.G("running")).T() ? Gv.S("⏸") : Gv.S("▶"));
      if (Gv.Ne(play_btn.G("text"), pt)) {
        play_btn = play_btn.S("text", pt);
      }
      play_btn.C("set_pressed_no_signal", m.G("running"));
      foreach (Gv k in (sp_btns).Iter()) {
        sp_btns.I(k).C("set_pressed_no_signal", Gv.B(Gv.Eq(Gd.Dyn("int", m.G("speed")), k)));
      }
    }
    Gv dead = Gv.B(!(m.G("sim").C("cab_live")).T());
    if (Gv.Ne(warn_btn.G("visible"), dead)) {
      warn_btn = warn_btn.S("visible", dead);
    }
    if ((dead).T()) {
      Gv wt = (Gv.S("⚠ Rozvaděč bez napětí – ") + (((m.G("sim").G("S").G("cab").G("estop")).T() ? Gv.S("NOUZOVÉ STOP") : ((!(m.G("sim").G("S").G("cab").G("main")).T() ? Gv.S("hlavní vypínač") : Gv.S("klíč VYP"))))));
      if (Gv.Ne(warn_btn.G("text"), wt)) {
        warn_btn = warn_btn.S("text", wt);
      }
    }
    if (Gv.Gt(msg_t, Gv.F(0.0))) {
      msg_t = (msg_t - Self.C("get_process_delta_time"));
      if (Gv.Le(msg_t, Gv.F(0.0))) {
        msg = msg.S("text", Gv.S(""));
        toast_box = toast_box.S("visible", Gv.B(false));
      }
    }
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "m": return this.m;
    case "info": return this.info;
    case "msg": return this.msg;
    case "toast_box": return this.toast_box;
    case "upd_btn": return this.upd_btn;
    case "warn_btn": return this.warn_btn;
    case "cab_btn": return this.cab_btn;
    case "hard_btn": return this.hard_btn;
    case "big_btn": return this.big_btn;
    case "pan_btn": return this.pan_btn;
    case "play_btn": return this.play_btn;
    case "sp_btns": return this.sp_btns;
    case "msg_t": return this.msg_t;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "m": this.m = v; return v;
    case "info": this.info = v; return v;
    case "msg": this.msg = v; return v;
    case "toast_box": this.toast_box = v; return v;
    case "upd_btn": this.upd_btn = v; return v;
    case "warn_btn": this.warn_btn = v; return v;
    case "cab_btn": this.cab_btn = v; return v;
    case "hard_btn": this.hard_btn = v; return v;
    case "big_btn": this.big_btn = v; return v;
    case "pan_btn": this.pan_btn = v; return v;
    case "play_btn": this.play_btn = v; return v;
    case "sp_btns": this.sp_btns = v; return v;
    case "msg_t": this.msg_t = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "ui_sync": return ui_sync();
    case "_ready": return _ready();
    case "_go": return _go(Gd.A(a, 0));
    case "toast": return toast(Gd.A(a, 0));
    case "update_info": return update_info();
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_main_Hud(); return Gv.O(o); }
  public override string ScriptName { get { return "S_main_Hud"; } }
}
public partial class S_main : GdNode {
  public static Gv Art;
  public static Gv Sim;
  public static Gv Ctl;
  public static Gv Auto;
  public static Gv Sight;
  public static Gv Iodine;
  public static Gv Cabinet;
  public static Gv LiquidFx;
  public Gv cabinet;
  public Gv cab_hold;
  public Gv iodine_win;
  public Gv sight_win;
  public Gv auto;
  public static Gv PANEL_W;
  public Gv panel_on;
  public Gv dock_on;
  public Gv dock;
  public Gv dock_big;
  public Gv hard;
  public Gv calm_since;
  public Gv calm_y;
  public Gv sim;
  public Gv ctl;
  public Gv flow_layer;
  public Gv live_nodes;
  public Gv hl;
  public Gv NOLIVE;
  public Gv NOFLOW;
  public Gv T_live;
  public Gv T_flow;
  public Gv T_sim;
  public Gv N_sim;
  public Gv NOSIM;
  public Gv dyn_t;
  public Gv flow_t;
  public Gv calm_t;
  public Gv running;
  public Gv speed;
  public Gv sim_acc;
  public static Gv PIPE_OUT;
  public static Gv PIPE_MID;
  public static Gv PIPE_IN;
  public static Gv STEEL;
  public static Gv STEEL_D;
  public static Gv DARK;
  public static Gv OPEN_C;
  public static Gv SHUT_C;
  public static Gv ACCENT;
  public Gv plan;
  public Gv items;
  public Gv bounds;
  public Gv floor_y;
  public Gv no_gpu_liquid;
  public Gv hall_dark;
  public Gv cam;
  public Gv static_layer;
  public Gv item_layer;
  public Gv valve_layer;
  public Gv hud;
  public Gv game;
  public Gv mini;
  public Gv updater;
  public Gv font;
  public Gv font_b;
  public Gv zoom_target;
  public Gv zoom_anchor;
  public Gv zoom_anim;
  public Gv dragging;
  public Gv drag_moved;
  public Gv drag_start;
  public Gv vel;
  public Gv last_move_ms;
  public static Gv USER_PLAN;
  public Gv plan_user;
  public Gv editor;
  public Gv v3;
  public Gv practice;
  public Gv cam_anim;
  public Gv cam_to_p;
  public Gv cam_to_z;
  public static Gv AA_SHADER;
  public Gv aa_layer;
  public static Gv SAVE_PATH;
  public static Gv PLAN_REV;
  public Gv save_path;
  public Gv save_acc;
  public static Gv THROTTLE_IDS;
  public Gv valve_pop;
  public Gv vdrag;
  public Gv vdrag_pct;
  public Gv _vnode;
  public static Gv VNAME;
  public Gv undo_stack;
  public static Gv BAKE_SCALE;
  public static Gv TILE;
  public Gv tiles_root;
  public Gv detail_spr;
  public Gv detail_rect;
  public Gv detail_scale;
  public Gv detail_busy;
  public Gv detail_idle;
  public Gv detail_sig;
  public Gv detail_svs;
  public Gv detail_cur;
  public Gv outlets;
  public Gv outlet_open;
  public Gv hose_drag;
  public Gv pipe_pts;
  public static Gv PIPE_LIQ;
  static S_main() {
    Art = Gd.Script("art");
    Sim = Gd.Script("sim");
    Ctl = Gd.Script("ctl");
    Auto = Gd.Script("auto");
    Sight = Gd.Script("sight");
    Iodine = Gd.Script("iodine");
    Cabinet = Gd.Script("cabinet");
    LiquidFx = Gd.Script("liquidfx");
    PANEL_W = Gv.F(280.0);
    PIPE_OUT = Gd.Ctor("Color", Gv.S("15191d"));
    PIPE_MID = Gd.Ctor("Color", Gv.S("aab2ba"));
    PIPE_IN = Gd.Ctor("Color", Gv.S("0c0f12"));
    STEEL = Gd.Ctor("Color", Gv.S("8a939b"));
    STEEL_D = Gd.Ctor("Color", Gv.S("23282e"));
    DARK = Gd.Ctor("Color", Gv.S("2b3138"));
    OPEN_C = Gd.Ctor("Color", Gv.S("2fbf5b"));
    SHUT_C = Gd.Ctor("Color", Gv.S("d93a2c"));
    ACCENT = Gd.Ctor("Color", Gv.S("f2c230"));
    USER_PLAN = Gv.S("user://plan_upravy.json");
    AA_SHADER = Gv.S("\nshader_type canvas_item;\nuniform sampler2D scr : hint_screen_texture, filter_linear, repeat_disable;\nvoid fragment() {\n\tvec2 px = SCREEN_PIXEL_SIZE;\n\tvec2 uv = SCREEN_UV;\n\tvec3 L = vec3(0.299, 0.587, 0.114);\n\tvec3 cM = texture(scr, uv).rgb;\n\tvec3 cNW = texture(scr, uv + vec2(-1.0, -1.0) * px).rgb;\n\tvec3 cNE = texture(scr, uv + vec2(1.0, -1.0) * px).rgb;\n\tvec3 cSW = texture(scr, uv + vec2(-1.0, 1.0) * px).rgb;\n\tvec3 cSE = texture(scr, uv + vec2(1.0, 1.0) * px).rgb;\n\tfloat lM = dot(cM, L); float lNW = dot(cNW, L); float lNE = dot(cNE, L); float lSW = dot(cSW, L); float lSE = dot(cSE, L);\n\tfloat lMin = min(lM, min(min(lNW, lNE), min(lSW, lSE)));\n\tfloat lMax = max(lM, max(max(lNW, lNE), max(lSW, lSE)));\n\tif (lMax - lMin < max(0.04, lMax * 0.12)) { COLOR = vec4(cM, 1.0); }\n\telse {\n\t\tvec2 dir;\n\t\tdir.x = -((lNW + lNE) - (lSW + lSE));\n\t\tdir.y = ((lNW + lSW) - (lNE + lSE));\n\t\tfloat red = max((lNW + lNE + lSW + lSE) * 0.25 * 0.125, 1.0 / 128.0);\n\t\tfloat rcp = 1.0 / (min(abs(dir.x), abs(dir.y)) + red);\n\t\tdir = clamp(dir * rcp, vec2(-6.0), vec2(6.0)) * px;\n\t\tvec3 a = 0.5 * (texture(scr, uv + dir * (1.0 / 3.0 - 0.5)).rgb + texture(scr, uv + dir * (2.0 / 3.0 - 0.5)).rgb);\n\t\tvec3 b = a * 0.5 + 0.25 * (texture(scr, uv + dir * -0.5).rgb + texture(scr, uv + dir * 0.5).rgb);\n\t\tfloat lB = dot(b, L);\n\t\tCOLOR = vec4((lB < lMin || lB > lMax) ? a : b, 1.0);\n\t}\n}\n");
    SAVE_PATH = Gv.S("user://varka.sav");
    PLAN_REV = Gv.I(3L);
    THROTTLE_IDS = Gd.Arr(Gv.S("valve_muraqe36"), Gv.S("valve_murbq6e1"), Gv.S("valve_murciy3u"));
    VNAME = Gd.Dict(Gv.S("rvp"), Gv.S("pánev"), Gv.S("sk"), Gv.S("SK"), Gv.S("wp"), Gv.S("vířivka"), Gv.S("hlt"), Gv.S("bojler"), Gv.S("lv"), Gv.S("ledovka"), Gv.S("spilka"), Gv.S("spilka"), Gv.S("vodovod"), Gv.S("řad"), Gv.S("tanky"), Gv.S("tanky"), Gv.S("mp"), Gv.S("mladinová pánev"));
    BAKE_SCALE = Gv.F(1.0);
    TILE = Gv.I(2048L);
    PIPE_LIQ = Gv.S("\nshader_type canvas_item;\nfloat h21(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }\nfloat vn(vec2 p) {\n\tvec2 i = floor(p); vec2 f = fract(p); f = f * f * (3.0 - 2.0 * f);\n\treturn mix(mix(h21(i), h21(i + vec2(1.0, 0.0)), f.x), mix(h21(i + vec2(0.0, 1.0)), h21(i + vec2(1.0, 1.0)), f.x), f.y);\n}\nvoid fragment() {\n\tfloat c = UV.y * 2.0 - 1.0;\n\tfloat d = sqrt(max(0.0, 1.0 - c * c));\n\tfloat spd = COLOR.a * 300.0;\n\tvec3 base = COLOR.rgb;\n\tbase = mix(base, vec3(dot(base, vec3(0.299, 0.587, 0.114))), 0.10) * vec3(0.97, 0.95, 0.90);\n\tfloat x = UV.x - TIME * spd;\n\tvec3 deep = base * vec3(0.80, 0.64, 0.46);\n\tvec3 col = mix(base * vec3(1.10, 1.08, 1.05), deep, pow(d, 1.3));\n\tfloat st = vn(vec2(x * 0.045, c * 2.5 + 3.0));\n\tcol *= 1.0 + step(0.5, spd) * (st - 0.5) * 0.10;\n\tvec2 g = vec2(x * 0.22, c * 5.0);\n\tvec2 gi = floor(g); vec2 gf = fract(g);\n\tfloat hh = h21(gi);\n\tvec2 cp = vec2(0.5 + 0.35 * sin(hh * 40.0), 0.5 + 0.35 * cos(hh * 30.0));\n\tfloat sp = smoothstep(0.16, 0.0, length(gf - cp)) * step(0.82, hh);\n\tcol = mix(col, base * 1.35 + 0.06, sp * 0.5 * d);\n\tcol *= mix(1.0, 0.62, smoothstep(0.70, 1.0, abs(c)));\n\tcol += vec3(1.0, 0.97, 0.9) * exp(-pow((c + 0.46) / 0.10, 2.0)) * 0.38;\n\tcol += vec3(0.8, 0.85, 0.95) * exp(-pow((c - 0.62) / 0.07, 2.0)) * 0.12;\n\tCOLOR = vec4(col, 1.0);\n}\n");
  }
  public S_main() {
    TypeName = "Node2D";
    cabinet = Gv.Null;
    cab_hold = Gv.S("");
    iodine_win = Gv.Null;
    sight_win = Gv.Null;
    auto = Gv.Null;
    panel_on = Gv.B(false);
    dock_on = Gv.B(true);
    dock = Gv.Null;
    dock_big = Gv.B(false);
    hard = Gv.B(false);
    calm_since = Gd.Dict();
    calm_y = Gd.Dict();
    sim = Gv.Null;
    ctl = Gv.Null;
    flow_layer = Gv.Null;
    live_nodes = Gd.Arr();
    hl = Gd.Dict();
    NOLIVE = Gv.B(Gv.In(Gv.S("--nolive"), Gd.Static("OS", "get_cmdline_user_args")));
    NOFLOW = Gv.B(Gv.In(Gv.S("--noflow"), Gd.Static("OS", "get_cmdline_user_args")));
    T_live = Gv.I(0L);
    T_flow = Gv.I(0L);
    T_sim = Gv.I(0L);
    N_sim = Gv.I(0L);
    NOSIM = Gv.B(Gv.In(Gv.S("--nosim"), Gd.Static("OS", "get_cmdline_user_args")));
    dyn_t = Gv.F(0.0);
    flow_t = Gv.F(0.0);
    calm_t = Gv.I(0L);
    running = Gv.B(false);
    speed = Gv.F(10.0);
    sim_acc = Gv.F(0.0);
    plan = Gd.Dict();
    items = Gd.Arr();
    bounds = Gd.Ctor("Rect2");
    floor_y = Gv.F(0.0);
    no_gpu_liquid = Gv.B(false);
    hall_dark = Gv.B(true);
    cam = Gv.Null;
    static_layer = Gv.Null;
    item_layer = Gv.Null;
    valve_layer = Gv.Null;
    hud = Gv.Null;
    game = Gv.Null;
    mini = Gv.Null;
    updater = Gv.Null;
    font = Gv.Null;
    font_b = Gv.Null;
    zoom_target = Gv.F(0.4);
    zoom_anchor = Gd.StaticGet("Vector2", "ZERO");
    zoom_anim = Gv.B(false);
    dragging = Gv.B(false);
    drag_moved = Gv.B(false);
    drag_start = Gd.StaticGet("Vector2", "ZERO");
    vel = Gd.StaticGet("Vector2", "ZERO");
    last_move_ms = Gv.I(0L);
    plan_user = Gv.B(false);
    editor = Gv.Null;
    v3 = Gv.Null;
    practice = Gv.Null;
    cam_anim = Gv.B(false);
    cam_to_p = Gd.StaticGet("Vector2", "ZERO");
    cam_to_z = Gv.F(1.0);
    aa_layer = Gv.Null;
    save_path = SAVE_PATH;
    save_acc = Gv.F(0.0);
    valve_pop = Gv.Null;
    vdrag = Gv.Null;
    vdrag_pct = Gv.F(0.0);
    _vnode = Gd.Dict();
    undo_stack = Gd.Arr();
    tiles_root = Gv.Null;
    detail_spr = Gv.Null;
    detail_rect = Gd.Ctor("Rect2");
    detail_scale = Gv.F(1.0);
    detail_busy = Gv.B(false);
    detail_idle = Gv.F(0.0);
    detail_sig = Gv.S("");
    detail_svs = Gd.Arr();
    detail_cur = Gv.I(0L);
    outlets = Gd.Arr();
    outlet_open = Gd.Dict();
    hose_drag = Gv.Null;
    pipe_pts = Gd.Dict();
  }
  public Gv right_w() {
    Gv w = Gv.F(0.0);
    if ((Gv.Ne(ctl, Gv.Null) && (panel_on).T())) {
      w = (w + PANEL_W);
    }
    if ((Gv.Ne(dock, Gv.Null) && (dock_on).T())) {
      w = (w + dock_w());
    }
    return (w).AsF();
    return Gv.Null;
  }
  public Gv _load_ui() {
    if ((Gd.Static("FileAccess", "file_exists", Gv.S("user://ui.json"))).T()) {
      Gv d = Gd.Static("JSON", "parse_string", Gd.Static("FileAccess", "open", Gv.S("user://ui.json"), Gd.StaticGet("FileAccess", "READ")).C("get_as_text"));
      if (Gv.Is(d, "Dictionary")) {
        panel_on = Gd.Dyn("bool", d.C("get", Gv.S("panel"), Gv.B(false)));
        dock_on = Gv.B(true);
        dock_big = Gd.Dyn("bool", d.C("get", Gv.S("big"), Gv.B(false)));
        hard = Gd.Dyn("bool", d.C("get", Gv.S("hard"), Gv.B(false)));
      }
    }
    return Gv.Null;
  }
  public Gv _save_ui() {
    Gv f = Gd.Static("FileAccess", "open", Gv.S("user://ui.json"), Gd.StaticGet("FileAccess", "WRITE"));
    if (Gv.Ne(f, Gv.Null)) {
      f.C("store_string", Gd.Static("JSON", "stringify", Gd.Dict(Gv.S("panel"), panel_on, Gv.S("dock2"), dock_on, Gv.S("big"), dock_big, Gv.S("hard"), hard)));
    }
    return Gv.Null;
  }
  public Gv apply_ui() {
    if (Gv.Ne(flow_layer, Gv.Null)) {
      flow_layer = flow_layer.S("visible", Gv.B(!(hard).T()));
    }
    if (Gv.Ne(ctl, Gv.Null)) {
      ctl = ctl.S("visible", panel_on);
      ctl.C("dirty");
    }
    if (Gv.Ne(dock, Gv.Null)) {
      dock = dock.S("visible", dock_on);
      dock = dock.S("offset_right", (-(((Gv.Ne(ctl, Gv.Null) && (panel_on).T()) ? PANEL_W : Gv.F(0.0)))));
      dock = dock.S("offset_left", (dock.G("offset_right") - dock_w()));
      dock.C("fit");
    }
    if (Gv.Ne(hud, Gv.Null)) {
      hud.C("ui_sync");
    }
    return Gv.Null;
  }
  public Gv toggle_panel() {
    panel_on = Gv.B(!(panel_on).T());
    _save_ui();
    apply_ui();
    return Gv.Null;
  }
  public Gv toggle_dock() {
    return Gv.Null;
    return Gv.Null;
  }
  public Gv dock_temp(Gv on = default(Gv)) {
    dock_on = on;
    apply_ui();
    return Gv.Null;
  }
  public Gv toggle_hard() {
    hard = Gv.B(!(hard).T());
    _save_ui();
    if (Gv.Ne(flow_layer, Gv.Null)) {
      flow_layer = flow_layer.S("visible", Gv.B(!(hard).T()));
    }
    redraw();
    redraw_valves();
    if (Gv.Ne(hud, Gv.Null)) {
      hud.C("ui_sync");
      hud.C("toast", ((hard).T() ? Gv.S("Režim HARD: modré klapky, toky v trubkách nejsou vidět, hladinu změř tlačítkem MĚŘ") : Gv.S("Režim SOFT: červená = zavřeno, zelená = otevřeno, toky jsou vidět")));
    }
    return Gv.Null;
  }
  public Gv vessel_calm(Gv v = default(Gv)) {
    Gv b = v.C("get", Gv.S("bind"));
    Gv S = sim.G("S");
    Gv busy = Gv.B(false);
    if (Gv.In(b, Gd.Arr(Gv.S("rvp"), Gv.S("mp"), Gv.S("wp")))) {
      busy = Gv.B((((S.I(b).C("get", Gv.S("stir"), Gv.B(false))).T() || (S.I(b).C("get", Gv.S("boiling"), Gv.B(false))).T()) || (Gv.Eq(b, Gv.S("rvp")) && Gv.Gt(S.G("rvp").G("vort"), Gv.F(0.003)))));
    }
    return Gv.B((!(busy).T() && Gv.Ge((S.G("t") - calm_since.C("get", v.G("id"), Gv.F(-1000000000.0))), Gv.F(15.0))));
    return Gv.Null;
  }
  public Gv hard_sig(Gv v = default(Gv)) {
    if (!(hard).T()) {
      return Gv.S("s");
    }
    Gv o = (Gv.S("h") + (((vessel_calm(v)).T() ? Gv.S("1") : Gv.S("0"))));
    foreach (Gv g in (items).Iter()) {
      if ((Gv.Eq(g.G("type"), Gv.S("gauge")) && Gv.Eq(gauge_vessel(g), v))) {
        o = (o + Gd.Dyn("str", g.C("get", Gv.S("_meas"), Gv.I(0L))));
      }
      if ((Gv.Eq(g.G("type"), Gv.S("dip")) && Gv.Eq(v.C("get", Gv.S("bind")), Gv.S("rvp")))) {
        o = (o + Gd.Dyn("str", g.C("get", Gv.S("_meas"), Gv.I(0L))));
      }
    }
    return o;
    return Gv.Null;
  }
  public Gv _calm_tick() {
    Gv S = sim.G("S");
    foreach (Gv v in (items).Iter()) {
      if ((Gv.Ne(v.G("type"), Gv.S("vessel")) || Gv.Eq(v.C("get", Gv.S("bind")), Gv.Null))) {
        continue;
      }
      Gv y = (level_y(v)).AsF();
      Gv b = v.C("get", Gv.S("bind"));
      Gv busy = Gv.B(false);
      if (Gv.In(b, Gd.Arr(Gv.S("rvp"), Gv.S("mp"), Gv.S("wp")))) {
        busy = Gv.B((((S.I(b).C("get", Gv.S("stir"), Gv.B(false))).T() || (S.I(b).C("get", Gv.S("boiling"), Gv.B(false))).T()) || (Gv.Eq(b, Gv.S("rvp")) && Gv.Gt(S.G("rvp").G("vort"), Gv.F(0.003)))));
      }
      if ((((busy).T() || Gv.Gt(Gd.absf((y - calm_y.C("get", v.G("id"), y))), Gv.F(0.25))) || !(calm_since.C("has", v.G("id"))).T())) {
        calm_since = calm_since.SI(v.G("id"), S.G("t"));
      }
      calm_y = calm_y.SI(v.G("id"), y);
    }
    return Gv.Null;
  }
  public Gv dip_geo(Gv it = default(Gv)) {
    Gv k = vessel_by_bind(Gv.S("rvp"));
    Gv x = (it.G("x")).AsF();
    Gv y = (it.G("y")).AsF();
    Gv h = Gd.Dyn("float", it.C("get", Gv.S("h"), Gv.I(300L)));
    if (Gv.Ne(k, Gv.Null)) {
      h = (k.G("h") - Gd.minf((k.G("h") * Gv.F(0.12)), Gv.F(40.0)));
      y = (k.G("y")).AsF();
      x = (((k.G("x") + k.G("w")) + Gv.F(34.0))).AsF();
    }
    return Gd.Dict(Gv.S("x"), x, Gv.S("y"), y, Gv.S("h"), h, Gv.S("v"), k);
    return Gv.Null;
  }
  public Gv measure_dip(Gv d = default(Gv)) {
    Gv G = dip_geo(d);
    if (Gv.Eq(G.G("v"), Gv.Null)) {
      return Gv.Null;
    }
    if (!(vessel_calm(G.G("v"))).T()) {
      hud.C("toast", Gv.S("Hladina není klidná (míchadlo, vaření, plnění nebo čerpání). Zastav to a počkej cca 15 s, pak měř znovu."));
      return Gv.Null;
    }
    d = d.SI(Gv.S("_meas"), (sim.C("hotV", sim.G("S").G("rvp")) / Gv.F(100.0)));
    hud.C("toast", (Gv.S("Měrka kotle: hladina %s hl") % c1(d.I(Gv.S("_meas")))));
    redraw();
    update_live();
    return Gv.Null;
  }
  public Gv measure_gauge(Gv g = default(Gv)) {
    Gv v = gauge_vessel(g);
    if (Gv.Eq(v, Gv.Null)) {
      hud.C("toast", Gv.S("Měrka nemá nádobu."));
      return Gv.Null;
    }
    if ((Gv.Eq(g.C("get", Gv.S("bot"), Gv.B(true)), Gv.B(false)) || Gv.Eq(g.C("get", Gv.S("top"), Gv.B(true)), Gv.B(false)))) {
      hud.C("toast", Gv.S("Měrka je odpojená kohouty – otevři horní i dolní kohout."));
      return Gv.Null;
    }
    if (!(vessel_calm(v)).T()) {
      hud.C("toast", Gv.S("Hladina není klidná (míchadlo, vaření, plnění nebo čerpání). Zastav to a počkej cca 15 s, pak měř znovu."));
      return Gv.Null;
    }
    Gv G = gauge_geo(g);
    g = g.SI(Gv.S("_meas"), Gd.clampf(level_y(v), G.G("y1"), G.G("y2")));
    g = g.SI(Gv.S("_meas_t"), sim.G("S").G("t"));
    Gv fr = (((Gv.F(10.0) * ((G.G("y2") - g.I(Gv.S("_meas"))))) / Gd.maxf(Gv.F(1.0), (G.G("y2") - G.G("y1"))))).AsF();
    hud.C("toast", (Gv.S("Změřeno: hladina na %s z 10 dílků.") % c1(fr)));
    redraw();
    update_live();
    return Gv.Null;
  }
  public Gv dock_w() {
    Gv vs = vp_size();
    Gv free = ((vs.G("x") - (((Gv.Ne(ctl, Gv.Null) && (panel_on).T()) ? PANEL_W : Gv.F(0.0))))).AsF();
    Gv h = (vs.G("y") - Gv.F(52.0));
    if ((dock_big).T()) {
      return (Gd.clampf((free * Gv.F(0.5)), Gv.F(520.0), Gv.F(900.0))).AsF();
    }
    return (Gd.clampf((h * Gv.F(0.34)), Gv.F(260.0), Gv.F(380.0))).AsF();
    return Gv.Null;
  }
  public Gv toggle_dock_big() {
    dock_big = Gv.B(!(dock_big).T());
    dock_on = Gv.B(true);
    _save_ui();
    apply_ui();
    if (Gv.Ne(dock, Gv.Null)) {
      dock.C("reset_view");
    }
    return Gv.Null;
  }
  public Gv _ready() {
    if (Gv.Ne(Gd.Static("DisplayServer", "get_name"), Gv.S("headless"))) {
      S_liquidfx.prewarm();
    }
    Gv sf = Gd.New("SystemFont");
    sf = sf.S("font_names", Gd.Ctor("PackedStringArray", Gd.Arr(Gv.S("Bahnschrift"), Gv.S("Segoe UI"), Gv.S("Arial"))));
    font = sf;
    Gv sfb = Gd.New("SystemFont");
    sfb = sfb.S("font_names", Gd.Ctor("PackedStringArray", Gd.Arr(Gv.S("Bahnschrift SemiBold"), Gv.S("Bahnschrift"), Gv.S("Segoe UI Semibold"), Gv.S("Arial"))));
    sfb = sfb.S("font_weight", Gv.I(600L));
    font_b = sfb;
    if ((Gd.Static("FileAccess", "file_exists", Gv.S("user://vzhled.txt"))).T()) {
      hall_dark = Gv.B(Gv.Ne(Gd.Static("FileAccess", "open", Gv.S("user://vzhled.txt"), Gd.StaticGet("FileAccess", "READ")).C("get_as_text").C("strip_edges"), Gv.S("svetla")));
    }
    if (Gv.In(Gv.S("--light"), Gd.Static("OS", "get_cmdline_user_args"))) {
      hall_dark = Gv.B(false);
    }
    hall_dark = Gv.B(false);
    Gd.Static("RenderingServer", "set_default_clear_color", Gd.StaticGet("Color", "WHITE"));
    Gd.Static("DisplayServer", "window_set_title", Gv.S("Trenažér varny Zajíc – nativní"));
    _load_plan();
    _init_outlets();
    sim = S_sim.New();
    sim = sim.S("main", Self);
    sim = sim.S("deposit_on", Gv.B(!Gv.In(Gv.S("--tests"), Gd.Static("OS", "get_cmdline_user_args"))));
    if ((Gd.Static("FileAccess", "file_exists", Gv.S("user://recept.txt"))).T()) {
      sim = sim.S("recipe_idx", Gd.Dyn("int", Gd.Static("FileAccess", "open", Gv.S("user://recept.txt"), Gd.StaticGet("FileAccess", "READ")).C("get_as_text")));
    }
    auto = S_auto.New();
    auto = auto.S("m", Self);
    auto = auto.S("sim", sim);
    practice = Gd.Script("practice").C("new");
    practice = practice.S("m", Self);
    practice = practice.S("sim", sim);
    Gv tb0 = Gd.Static("Time", "get_ticks_msec");
    sim.C("setup", items, plan.C("get", Gv.S("joints"), Gd.Arr()));
    Gd.print((Gv.S("síť: %d uzlů, %d trubek, %d ms") % Gd.Arr(sim.G("net").G("nodes").C("size"), sim.G("net").G("edges").C("size"), (Gd.Static("Time", "get_ticks_msec") - tb0))));
    Gv restored = Gv.B(false);
    if (!((((Gv.In(Gv.S("--simtest"), Gd.Static("OS", "get_cmdline_user_args")) || Gv.In(Gv.S("--demo"), Gd.Static("OS", "get_cmdline_user_args"))) || Gv.In(Gv.S("--fresh"), Gd.Static("OS", "get_cmdline_user_args"))) || Gv.In(Gv.S("--autotest"), Gd.Static("OS", "get_cmdline_user_args"))) || Gv.In(Gv.S("--tests"), Gd.Static("OS", "get_cmdline_user_args")))) {
      restored = load_game();
    }
    if (Gv.In(Gv.S("--savetest"), Gd.Static("OS", "get_cmdline_user_args"))) {
      _savetest();
      Self.C("get_tree").C("quit");
      return Gv.Null;
    }
    if (Gv.In(Gv.S("--tests"), Gd.Static("OS", "get_cmdline_user_args"))) {
      save_path = Gv.S("user://test_tmp.sav");
      _ = _run_tests();
      return Gv.Null;
    }
    if (Gv.In(Gv.S("--autotest"), Gd.Static("OS", "get_cmdline_user_args"))) {
      _autotest();
      Self.C("get_tree").C("quit");
      return Gv.Null;
    }
    if (Gv.In(Gv.S("--simtest"), Gd.Static("OS", "get_cmdline_user_args"))) {
      _simtest();
      Self.C("get_tree").C("quit");
      return Gv.Null;
    }
    foreach (Gv a_ in (Gd.Static("OS", "get_cmdline_user_args")).Iter()) {
      if ((a_.C("begins_with", Gv.S("--diag="))).T()) {
        Gv D = Gd.load(a_.C("substr", Gv.I(7L))).C("new");
        D = D.S("m", Self);
        D.C("run");
        Self.C("get_tree").C("quit");
        return Gv.Null;
      }
    }
    cam = Gd.New("Camera2D");
    Self.C("add_child", cam);
    cam.C("make_current");
    updater = Gd.Script("updater").C("new");
    updater = updater.S("main", Self);
    Self.C("add_child", updater);
    hud = S_main_Hud.New();
    hud = hud.S("m", Self);
    Self.C("add_child", hud);
    if ((Gv.Ne(Gd.Static("DisplayServer", "get_name"), Gv.S("headless")) && !Gv.In(Gv.S("--noaa"), Gd.Static("OS", "get_cmdline_user_args")))) {
      _make_aa();
    }
    v3 = Gd.Load("res://view3d.gd").C("new");
    v3 = v3.S("m", Self);
    Self.C("add_child", v3);
    updater.G("status").C("connect", Gd.Fn((_a) => { Gv t = Gd.A(_a, 0); Gv _k = Gd.A(_a, 1); hud.C("toast", t); return Gv.Null; }));
    updater.G("available").C("connect", Gd.Fn((_a) => { Gv v = Gd.A(_a, 0); hud = hud.S("upd_btn", hud.G("upd_btn").S("text", (Gv.S("⟳ Nová verze %d") % v))); hud.G("upd_btn").C("add_theme_color_override", Gv.S("font_color"), Gd.Ctor("Color", Gv.S("f2c230"))); return Gv.Null; }));
    if ((Gv.Ne(Gd.Static("DisplayServer", "get_name"), Gv.S("headless")) && Gv.Eq(Gd.Static("OS", "get_cmdline_user_args").C("size"), Gv.I(0L)))) {
      Self.C("get_tree").C("create_timer", Gv.F(4.0)).G("timeout").C("connect", Gd.Fn((_a) => {  updater.C("check", Gv.B(false)); return Gv.Null; }));
    }
    mini = S_main_Mini.New();
    mini = mini.S("m", Self);
    hud.C("add_child", mini);
    mini.C("setup");
    hud.C("toast", Gv.S("Připravuji plánek…"));
    _ = Self.C("get_tree").G("process_frame");
    fit_all();
    if (Gv.Ne(Gd.Static("DisplayServer", "get_name"), Gv.S("headless"))) {
      _ = _bake();
    }
    flow_layer = S_main_FlowLayer.New();
    flow_layer = flow_layer.S("m", Self);
    Self.C("add_child", flow_layer);
    flow_layer.C("build");
    flow_layer = flow_layer.S("visible", Gv.B(!(hard).T()));
    foreach (Gv it in (items).Iter()) {
      if ((Gv.Eq(it.G("type"), Gv.S("vessel")) && Gv.In(it.C("get", Gv.S("bind")), Gd.Arr(Gv.S("rvp"), Gv.S("mp"), Gv.S("sk"), Gv.S("wp"), Gv.S("hlt"), Gv.S("lv"), Gv.S("spilka"))))) {
        _live_node(Gv.S("vessel"), it);
      }
    }
    foreach (Gv it_2 in (items).Iter()) {
      if (Gv.Eq(it_2.G("type"), Gv.S("pump"))) {
        _live_node(Gv.S("pump"), it_2).S("position", Gd.Ctor("Vector2", it_2.G("x"), it_2.G("y")));
      }
    }
    _live_node(Gv.S("streams"), Gv.Null);
    _live_node(Gv.S("leaks"), Gv.Null);
    foreach (Gv itp in (items).Iter()) {
      if (Gv.Eq(itp.G("type"), Gv.S("panel"))) {
        cabinet = S_cabinet.New();
        cabinet = cabinet.S("m", Self);
        cabinet.C("setup", itp);
        Self.C("add_child", cabinet);
        cabinet = cabinet.S("visibility_layer", Gv.I(2L));
        Self.C("get_viewport").S("canvas_cull_mask", Gv.I(4294967293L));
        break;
      }
    }
    item_layer = S_main_ItemLayer.New();
    item_layer = item_layer.S("m", Self);
    item_layer = item_layer.S("live_only", Gv.B(true));
    Self.C("add_child", item_layer);
    valve_layer = S_main_ItemLayer.New();
    valve_layer = valve_layer.S("m", Self);
    valve_layer = valve_layer.S("live_only", Gv.B(true));
    valve_layer = valve_layer.S("valves_only", Gv.B(true));
    Self.C("add_child", valve_layer);
    foreach (Gv it_3 in (items).Iter()) {
      if ((Gv.Eq(it_3.G("type"), Gv.S("vessel")) && Gv.In(it_3.C("get", Gv.S("bind")), Gd.Arr(Gv.S("rvp"), Gv.S("mp"), Gv.S("sk"), Gv.S("wp"), Gv.S("hlt"), Gv.S("lv"), Gv.S("spilka"))))) {
        _live_node(Gv.S("card"), it_3);
      }
    }
    _load_ui();
    hard = Gv.B(((hard).T() || Gv.In(Gv.S("--hard"), Gd.Static("OS", "get_cmdline_user_args"))));
    apply_ui();
    if (Gv.In(Gv.S("--panel"), Gd.Static("OS", "get_cmdline_user_args"))) {
      panel_on = Gv.B(true);
    }
    if (Gv.In(Gv.S("--dockbig"), Gd.Static("OS", "get_cmdline_user_args"))) {
      dock_big = Gv.B(true);
    }
    if (!Gv.In(Gv.S("--nopanel"), Gd.Static("OS", "get_cmdline_user_args"))) {
      ctl = S_ctl.New();
      ctl = ctl.S("m", Self);
      Self.C("add_child", ctl);
    }
    if (Gv.Ne(cabinet, Gv.Null)) {
      dock = S_main_CabDock.New();
      dock = dock.S("m", Self);
      hud.C("add_child", dock);
      dock.C("setup");
    }
    game = Gd.Script("game").C("new");
    game = game.S("m", Self);
    Self.C("add_child", game);
    Gv rota = Gd.Script("rotametr").C("new");
    rota = rota.S("m", Self);
    game.C("add_child", rota);
    apply_ui();
    _ = Self.C("get_tree").G("process_frame");
    if (Gv.Ne(dock, Gv.Null)) {
      dock.C("fit");
    }
    fit_all();
    foreach (Gv a0 in (Gd.Static("OS", "get_cmdline_user_args")).Iter()) {
      if ((a0.C("begins_with", Gv.S("--go="))).T()) {
        hud.C("_go", a0.C("substr", Gv.I(5L)));
      }
      if ((a0.C("begins_with", Gv.S("--view="))).T()) {
        Gv vv = a0.C("substr", Gv.I(7L)).C("split", Gv.S(","));
        cam = cam.S("position", Gd.Ctor("Vector2", Gd.Dyn("float", vv.I(Gv.I(0L))), Gd.Dyn("float", vv.I(Gv.I(1L)))));
        cam = cam.S("zoom", Gd.Ctor("Vector2", Gd.Dyn("float", vv.I(Gv.I(2L))), Gd.Dyn("float", vv.I(Gv.I(2L)))));
      }
    }
    if ((Gv.In(Gv.S("--editor"), Gd.Static("OS", "get_cmdline_user_args")) || Gv.In(Gv.S("--edtest"), Gd.Static("OS", "get_cmdline_user_args")))) {
      open_editor();
    }
    if (Gv.In(Gv.S("--edtest"), Gd.Static("OS", "get_cmdline_user_args"))) {
      editor.C("selftest");
      Self.C("get_tree").C("quit");
      return Gv.Null;
    }
    foreach (Gv a1 in (Gd.Static("OS", "get_cmdline_user_args")).Iter()) {
      if (((a1.C("begins_with", Gv.S("--edsel="))).T() && Gv.Ne(editor, Gv.Null))) {
        foreach (Gv it1 in (items).Iter()) {
          if (Gv.Eq(Gd.Dyn("str", it1.C("get", Gv.S("bind"), Gv.S(""))), a1.C("substr", Gv.I(8L)))) {
            editor = editor.S("sel", Gd.Dict(Gv.S("t"), Gv.S("item"), Gv.S("it"), it1));
            editor.C("_props");
            editor.C("queue_redraw");
          }
        }
      }
    }
    if (Gv.In(Gv.S("--edclick"), Gd.Static("OS", "get_cmdline_user_args"))) {
      if ((Gv.Eq(editor, Gv.Null) || !(editor.G("on")).T())) {
        open_editor();
      }
      _ = editor.C("clicktest");
      Self.C("get_tree").C("quit");
      return Gv.Null;
    }
    hud.C("toast", ((restored).T() ? (Gv.S("Rozjetá várka obnovena (%d:%02d)") % Gd.Arr(Gd.Dyn("int", (sim.G("S").G("t") / Gv.I(3600L))), (Gd.Dyn("int", (sim.G("S").G("t") / Gv.I(60L))) % Gv.I(60L)))) : Gv.S("")));
    if (Gv.In(Gv.S("--demo"), Gd.Static("OS", "get_cmdline_user_args"))) {
      foreach (Gv id in (sim.C("route_valves", Gv.S("hlt"), Gv.S("rvp"), Gd.Arr(Gv.S("phw")))).Iter()) {
        if ((sim.G("net").G("byId").C("has", id)).T()) {
          sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(id, sim.G("net").G("byId").I(id).SI(Gv.S("open"), Gv.B(true)))));
        }
        if ((outlet_open.C("has", id)).T()) {
          outlet_open = outlet_open.SI(id, Gv.B(true));
        }
      }
      sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
      sim = sim.S("S", sim.G("S").S("rvpWater", sim.G("S").G("rvpWater").S("on", Gv.B(true))));
      sim = sim.S("S", sim.G("S").S("rvp", sim.G("S").G("rvp").S("stir", Gv.B(true))));
      foreach (Gv i in (Gv.I(240L)).Iter()) {
        sim.C("sim_step", Gv.F(1.0));
      }
      sim = sim.S("S", sim.G("S").S("malt", sim.G("S").G("malt").S("gate", Gv.B(true))));
      sim = sim.S("S", sim.G("S").S("malt", sim.G("S").G("malt").S("on", Gv.B(true))));
      running = Gv.B(!Gv.In(Gv.S("--paused"), Gd.Static("OS", "get_cmdline_user_args")));
      speed = Gv.F(10.0);
      foreach (Gv a in (Gd.Static("OS", "get_cmdline_user_args")).Iter()) {
        if ((a.C("begins_with", Gv.S("--speed="))).T()) {
          speed = Gd.Dyn("float", a.C("substr", Gv.I(8L)));
        }
      }
      flow_layer.C("refresh");
      update_live();
      if (Gv.In(Gv.S("--iodine"), Gd.Static("OS", "get_cmdline_user_args"))) {
        open_iodine(Gv.S("rvp"));
      }
      if (Gv.In(Gv.S("--pumpdemo"), Gd.Static("OS", "get_cmdline_user_args"))) {
        foreach (Gv id_2 in (sim.C("route_valves", Gv.S("rvp"), Gv.S("sk"), Gd.Arr(Gv.S("p1")))).Iter()) {
          if ((sim.G("net").G("byId").C("has", id_2)).T()) {
            sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(id_2, sim.G("net").G("byId").I(id_2).SI(Gv.S("open"), Gv.B(true)))));
          }
        }
        sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
        sim = sim.S("S", sim.G("S").S("cab", sim.G("S").G("cab").S("key", Gv.B(true))));
        sim = sim.S("S", sim.G("S").S("p1", sim.G("S").G("p1").S("on", Gv.B(true))));
      }
      if (Gv.In(Gv.S("--sight"), Gd.Static("OS", "get_cmdline_user_args"))) {
        foreach (Gv it0 in (items).Iter()) {
          if ((Gv.Eq(it0.G("type"), Gv.S("inl")) && Gv.Eq(it0.C("get", Gv.S("k"), Gv.S("")), Gv.S("pruh")))) {
            sight_win = S_sight.New();
            sight_win = sight_win.S("m", Self);
            hud.C("add_child", sight_win);
            sight_win.C("open", it0);
            break;
          }
        }
      }
    }
    if (Gv.In(Gv.S("--v3"), Gd.Static("OS", "get_cmdline_user_args"))) {
      toggle_3d(Gv.B(true));
      foreach (Gv a_2 in (Gd.Static("OS", "get_cmdline_user_args")).Iter()) {
        if ((a_2.C("begins_with", Gv.S("--yaw="))).T()) {
          v3 = v3.S("yaw", Gd.Dyn("float", a_2.C("substr", Gv.I(6L))));
        }
        if ((a_2.C("begins_with", Gv.S("--pitch="))).T()) {
          v3 = v3.S("pitch", Gd.Dyn("float", a_2.C("substr", Gv.I(8L))));
        }
        if ((a_2.C("begins_with", Gv.S("--dist="))).T()) {
          v3 = v3.S("dist", Gd.Dyn("float", a_2.C("substr", Gv.I(7L))));
        }
        if ((a_2.C("begins_with", Gv.S("--look="))).T()) {
          v3.C("look_at_item", a_2.C("substr", Gv.I(7L)), v3.G("dist"));
        }
        if ((a_2.C("begins_with", Gv.S("--tx="))).T()) {
          v3 = v3.S("target", v3.G("target").S("x", Gd.Dyn("float", a_2.C("substr", Gv.I(5L)))));
        }
      }
      v3.C("_cam");
    }
    foreach (Gv a_3 in (Gd.Static("OS", "get_cmdline_user_args")).Iter()) {
      if ((a_3.C("begins_with", Gv.S("--rect="))).T()) {
        Gv q = a_3.C("substr", Gv.I(7L)).C("split", Gv.S(","));
        go_to(Gd.Ctor("Rect2", Gd.Dyn("float", q.I(Gv.I(0L))), Gd.Dyn("float", q.I(Gv.I(1L))), Gd.Dyn("float", q.I(Gv.I(2L))), Gd.Dyn("float", q.I(Gv.I(3L)))));
      }
      if ((a_3.C("begins_with", Gv.S("--go="))).T()) {
        hud.C("_go", a_3.C("substr", Gv.I(5L)));
      }
    }
    if (Gv.In(Gv.S("--bench"), Gd.Static("OS", "get_cmdline_user_args"))) {
      _ = Self.C("get_tree").C("create_timer", Gv.F(1.0)).G("timeout");
      Gv t0 = Gd.Static("Time", "get_ticks_usec");
      Gv frames = Gv.I(0L);
      Gv worst = Gv.F(0.0);
      Gv last = t0;
      while (Gv.Lt((Gd.Static("Time", "get_ticks_usec") - t0), Gv.I(6000000L))) {
        Gv el = (((Gd.Static("Time", "get_ticks_usec") - t0)) / Gv.F(1000000.0));
        cam = cam.S("position", cam.G("position").S("x", (cam.G("position").G("x") + (((Gv.F(600.0) * Self.C("get_process_delta_time")) / cam.G("zoom").G("x")) * ((Gv.Lt(Gd.fmod(el, Gv.F(2.0)), Gv.F(1.0)) ? Gv.F(1.0) : Gv.F(-1.0)))))));
        if (Gv.Lt(Gd.fmod(el, Gv.F(3.0)), Gv.F(0.05))) {
          zoom_at((Gv.Lt(Gd.fmod(el, Gv.F(6.0)), Gv.F(3.0)) ? Gv.F(1.6) : Gv.F(0.62)), (vp_size() * Gv.F(0.5)));
        }
        _ = Self.C("get_tree").G("process_frame");
        Gv now = Gd.Static("Time", "get_ticks_usec");
        worst = Gd.maxf(worst, (((now - last)) / Gv.F(1000.0)));
        last = now;
        frames = (frames + Gv.I(1L));
      }
      Gd.print((Gv.S("TIMES live=%d ms flow=%d ms sim=%d ms (%d kroků)") % Gd.Arr((T_live / Gv.I(1000L)), (T_flow / Gv.I(1000L)), (T_sim / Gv.I(1000L)), N_sim)));
      Gd.print((Gv.S("BENCH snímků/s=%.1f nejhorší snímek=%.1f ms") % Gd.Arr((frames / ((((Gd.Static("Time", "get_ticks_usec") - t0)) / Gv.F(1000000.0)))), worst)));
      Self.C("get_tree").C("quit");
    }
    if (Gv.In(Gv.S("--hosetest"), Gd.Static("OS", "get_cmdline_user_args"))) {
      _ = Self.C("get_tree").C("create_timer", Gv.F(0.5)).G("timeout");
      Gv hs = Gv.Null;
      foreach (Gv it_4 in (items).Iter()) {
        if (((Gv.Eq(it_4.G("type"), Gv.S("pipe")) && (it_4.C("get", Gv.S("hose"), Gv.B(false))).T()) && Gv.Eq(it_4.C("get", Gv.S("dn"), Gv.I(50L)), Gv.I(40L)))) {
          hs = it_4;
        }
      }
      Gv a_4 = Gd.Ctor("Vector2", hs.G("pts").I(Gv.I(0L)).I(Gv.I(0L)), hs.G("pts").I(Gv.I(0L)).I(Gv.I(1L)));
      Gv b = outlets.I(Gv.I(0L)).G("front");
      Gv to_s = Gd.Fn((_a) => { Gv w = Gd.A(_a, 0); return ((((w - cam.G("position"))) * cam.G("zoom").G("x")) + (vp_size() * Gv.F(0.5))); return Gv.Null; });
      Gv ev = Gd.New("InputEventMouseButton");
      ev = ev.S("button_index", Gd.K("MOUSE_BUTTON_LEFT"));
      ev = ev.S("pressed", Gv.B(true));
      ev = ev.S("position", to_s.C("call", a_4));
      Gd.Static("Input", "parse_input_event", ev);
      foreach (Gv i_2 in (Gv.I(11L)).Iter()) {
        Gv mv = Gd.New("InputEventMouseMotion");
        mv = mv.S("position", to_s.C("call", a_4.C("lerp", b, (i_2 / Gv.F(10.0)))));
        Gd.Static("Input", "parse_input_event", mv);
        _ = Self.C("get_tree").G("process_frame");
      }
      _ = Self.C("get_tree").G("process_frame");
      Gv up = Gd.New("InputEventMouseButton");
      up = up.S("button_index", Gd.K("MOUSE_BUTTON_LEFT"));
      up = up.S("pressed", Gv.B(false));
      up = up.S("position", to_s.C("call", b));
      Gd.Static("Input", "parse_input_event", up);
      _ = Self.C("get_tree").G("process_frame");
      Gd.print(Gv.S("HOSETEST konec="), hs.G("pts").I(Gv.I(0L)), Gv.S(" vývod="), b);
    }
    foreach (Gv a_5 in (Gd.Static("OS", "get_cmdline_user_args")).Iter()) {
      if ((a_5.C("begins_with", Gv.S("--shot="))).T()) {
        _ = Self.C("get_tree").C("create_timer", Gv.F(4.0)).G("timeout");
        Self.C("get_viewport").C("get_texture").C("get_image").C("save_png", a_5.C("substr", Gv.I(7L)));
        Self.C("get_tree").C("quit");
      }
    }
    return Gv.Null;
  }
  public Gv _savetest() {
    Gv S = sim.G("S");
    S = S.S("rvpWater", S.G("rvpWater").S("on", Gv.B(true)));
    foreach (Gv id in (sim.C("route_valves", Gv.S("hlt"), Gv.S("rvp"), Gd.Arr(Gv.S("phw")))).Iter()) {
      if ((sim.G("net").G("byId").C("has", id)).T()) {
        sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(id, sim.G("net").G("byId").I(id).SI(Gv.S("open"), Gv.B(true)))));
      }
      if ((outlet_open.C("has", id)).T()) {
        outlet_open = outlet_open.SI(id, Gv.B(true));
      }
    }
    sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
    foreach (Gv i in (Gv.I(300L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
    }
    Gv before = Gd.Arr(S.G("t"), sim.C("vol", S.G("rvp")), S.G("rvp").G("T"), S.G("hlt").G("w"));
    save_game();
    sim.C("new_state");
    sim.C("build_net", items, plan.C("get", Gv.S("joints"), Gd.Arr()));
    Gv ok = load_game();
    Gv S2 = sim.G("S");
    Gd.print((Gv.S("SAVETEST načteno=%s před=%s po=%s") % Gd.Arr(ok, Gd.Dyn("str", before), Gd.Dyn("str", Gd.Arr(S2.G("t"), sim.C("vol", S2.G("rvp")), S2.G("rvp").G("T"), S2.G("hlt").G("w"))))));
    Gd.Static("DirAccess", "remove_absolute", Gd.Static("ProjectSettings", "globalize_path", save_path));
    return Gv.Null;
  }
  public Gv _run_tests() {
    Gv T = Gd.Script("tests").C("new");
    T = T.S("m", Self);
    Gv failed = (T.C("run")).AsI();
    Self.C("get_tree").C("quit", (Gv.Gt(failed, Gv.I(0L)) ? Gv.I(1L) : Gv.I(0L)));
    return Gv.Null;
  }
  public Gv _autotest() {
    foreach (Gv a0 in (Gd.Static("OS", "get_cmdline_user_args")).Iter()) {
      if ((a0.C("begins_with", Gv.S("--recipe="))).T()) {
        sim = sim.S("recipe_idx", Gd.Dyn("int", a0.C("substr", Gv.I(9L))));
      }
    }
    sim.C("new_state");
    sim.C("build_net", items, plan.C("get", Gv.S("joints"), Gd.Arr()));
    Gv S = sim.G("S");
    auto = auto.S("S", S);
    foreach (Gv rt in (Gd.Arr(Gd.Arr(Gv.S("lv"), Gv.S("lv"), Gd.Arr(Gv.S("plv"))), Gd.Arr(Gv.S("lv"), Gv.S("hlt"), Gd.Arr(Gv.S("plv"))), Gd.Arr(Gv.S("lv"), Gv.S("tanky"), Gd.Arr(Gv.S("plv"))), Gd.Arr(Gv.S("wp"), Gv.S("spilka"), Gd.Arr(Gv.S("p3"))), Gd.Arr(Gv.S("vodovod"), Gv.S("spilka"), Gd.Arr()), Gd.Arr(Gv.S("vodovod"), Gv.S("lv"), Gd.Arr()))).Iter()) {
      Gv vs = sim.C("route_valves", rt.I(Gv.I(0L)), rt.I(Gv.I(1L)), rt.I(Gv.I(2L)));
      Gd.print(Gv.S("TRASA "), rt, Gv.S(" -> "), vs, Gv.S(" uzly: "), sim.G("last_route").C("size"));
    }
    auto.C("start");
    Gv last = Gv.I(-1L);
    Gv t0 = Gd.Static("Time", "get_ticks_msec");
    Gv maxs = (Gv.I(26L) * Gv.I(3600L));
    Gv adt = Gv.F(1.0);
    foreach (Gv a in (Gd.Static("OS", "get_cmdline_user_args")).Iter()) {
      if ((a.C("begins_with", Gv.S("--autodt="))).T()) {
        adt = Gd.Dyn("float", a.C("substr", Gv.I(9L)));
      }
    }
    Gv lt = Gv.F(0.0);
    while (((auto.G("on")).T() && Gv.Lt(S.G("t"), maxs))) {
      auto.C("tick", adt);
      sim.C("sim_step", adt);
      if (((Gv.Eq(S.G("sk").G("mode"), Gv.S("lauter")) && Gv.Ge((S.G("t") - lt), Gv.F(300.0))) && Gv.In(Gv.S("--lauterlog"), Gd.Static("OS", "get_cmdline_user_args")))) {
        lt = S.G("t");
        Gd.print((Gv.S("  SK t=%.0f min Qmax %.0f Qact %.0f C2 %.0f dp %.0f mbar c %.2f ch %.2f teig %.1f kg lože %.0f cm hladina %.0f cm °P %.1f turb %.0f pánev %.0f L") % Gd.Arr((S.G("t") / Gv.F(60.0)), S.G("sk").G("Qmax"), S.G("sk").G("Qact"), S.G("p2").G("flow"), S.G("sk").G("dp"), S.G("sk").G("c"), S.G("sk").G("ch"), S.G("sk").C("get", Gv.S("teig"), Gv.F(0.0)), (sim.C("skBedH") * Gv.F(100.0)), (sim.C("skHead") * Gv.F(100.0)), S.G("sk").G("outP"), S.G("sk").G("turb"), sim.C("vol", S.G("rvp")))));
      }
      if (Gv.Ne(auto.G("i"), last)) {
        last = auto.G("i");
        Gv rr = S.G("rvp");
        Gv mm = (Gv.Gt(sim.C("vol", S.G("sk").G("mash")), Gv.I(100L)) ? S.G("sk").G("mash") : rr);
        Gv tl = ((((mm.G("sg") + mm.G("dx")) + mm.G("ld")) + mm.G("f"))).AsF();
        Gd.print((Gv.S("   jod: kapalina %.2f %% škrobu (sg %.0f dx %.0f ld %.0f f %.0f), se zrny %.2f %%, T=%.1f") % Gd.Arr(((Gv.F(100.0) * mm.G("sg")) / Gd.maxf(Gv.F(1.0), tl)), mm.G("sg"), mm.G("dx"), mm.G("ld"), mm.G("f"), ((Gv.F(100.0) * (((mm.G("sg") + mm.G("su")) + mm.G("sc")))) / Gd.maxf(Gv.F(1.0), ((tl + mm.G("su")) + mm.G("sc")))), mm.G("T"))));
        Gd.print((Gv.S("   rmut: extrakt %.0f kg (zkv %.0f, dx %.0f, ld %.0f, ns %.0f) škrob %.0f (sg %.0f su %.0f sc %.0f) ni %.0f g %.0f | SK extrakt %.0f kg") % Gd.Arr(sim.C("ext", rr), rr.G("f"), rr.G("dx"), rr.G("ld"), rr.G("ns"), ((rr.G("sg") + rr.G("su")) + rr.G("sc")), rr.G("sg"), rr.G("su"), rr.G("sc"), rr.G("ni"), rr.G("g"), (sim.C("ext", S.G("sk").G("mash")) + S.G("sk").G("cells").C("reduce", Gd.Fn((_a) => { Gv a_2 = Gd.A(_a, 0); Gv c = Gd.A(_a, 1); return ((a_2 + c.G("f")) + c.G("n")); return Gv.Null; }), Gv.F(0.0))))));
        if ((Gd.Dyn("str", auto.G("status")).C("begins_with", Gv.S("Chmelovar"))).T()) {
          Gd.print(Gv.S("BILANCE PŘED VAREM: "), sim.C("extract_inventory"));
        }
        Gd.print((Gv.S("[%5.1f min] %s | pánev %.0f L %.1f °C | SK %.0f L | WP %.0f L | spilka %.0f L | led. %.0f L %.1f °C") % Gd.Arr((S.G("t") / Gv.F(60.0)), auto.G("status"), sim.C("vol", S.G("rvp")), S.G("rvp").G("T"), sim.C("skVolTotal"), sim.C("vol", S.G("wp")), S.G("sp").G("L"), S.G("lv").G("w"), S.G("lv").G("T"))));
      }
    }
    Gd.print((Gv.S("KONEC: done=%s failed='%s' čas=%.1f h, výpočet %d s") % Gd.Arr(auto.G("done"), auto.G("failed"), (S.G("t") / Gv.F(3600.0)), (((Gd.Static("Time", "get_ticks_msec") - t0)) / Gv.I(1000L)))));
    Gv rep = sim.C("spReport");
    Gd.print(Gv.S("PROTOKOL: "), rep);
    Gd.print(Gv.S("BILANCE EXTRAKTU: "), sim.C("extract_inventory"));
    if (Gv.In(Gv.S("--pipeext"), Gd.Static("OS", "get_cmdline_user_args"))) {
      Gv lst = Gd.Arr();
      foreach (Gv e in (sim.G("net").G("edges")).Iter()) {
        Gv x = ((((sim.C("ext", e.G("c")) + e.G("c").G("su")) + e.G("c").G("sc")) + e.G("c").G("ni"))).AsF();
        if (Gv.Gt(x, Gv.F(0.2))) {
          lst.C("append", Gd.Arr(x, e.C("get", Gv.S("pipe"), e.C("get", Gv.S("id"), Gv.S("?"))), sim.C("vol", e.G("c"))));
        }
      }
      lst.C("sort_custom", Gd.Fn((_a) => { Gv a1 = Gd.A(_a, 0); Gv b1 = Gd.A(_a, 1); return Gv.B(Gv.Gt(a1.I(Gv.I(0L)), b1.I(Gv.I(0L)))); return Gv.Null; }));
      foreach (Gv r in (lst.C("slice", Gv.I(0L), Gv.I(15L))).Iter()) {
        Gd.print((Gv.S("  TRUBKA %.1f kg  %s  %.0f L") % r));
      }
    }
    Gd.print((Gv.S("vyteklo L: %.0f, vyrážka: %s") % Gd.Arr((S.G("drainL") + S.G("spillL")), Gd.Dyn("str", S.G("flags").G("knock")))));
    foreach (Gv l in (S.G("log").C("slice", Gv.I(0L), Gv.I(14L))).Iter()) {
      Gd.print((Gv.S("  [%s] %d:%02d %s") % Gd.Arr(l.G("type"), Gd.Dyn("int", (l.G("t") / Gv.I(3600L))), (Gd.Dyn("int", (l.G("t") / Gv.I(60L))) % Gv.I(60L)), l.G("msg"))));
    }
    return Gv.Null;
  }
  public Gv _simtest() {
    foreach (Gv s in (Gd.Arr(Gv.S("Spustit 12"), Gv.S("▶ Spustit 13"), Gv.S("Spustit 14"), Gv.S("▶ Spustit 15"), Gv.S("■ 16"))).Iter()) {
      Gv t0 = Gd.Static("Time", "get_ticks_usec");
      Gv w = font_b.C("get_string_size", s, Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(14L)).G("x");
      Gd.print((Gv.S("sazba '%s' %.1f ms (šířka %d)") % Gd.Arr(s, (((Gd.Static("Time", "get_ticks_usec") - t0)) / Gv.F(1000.0)), w)));
    }
    Gv S = sim.G("S");
    Gv pr = Gd.Fn((_a) => { Gv lbl = Gd.A(_a, 0); Gv r = S.G("rvp"); Gd.print((Gv.S("%s t=%d min | pánev %.0f L %.1f °C %.2f °P škrob=%.1f | bojler %.0f L | SK %.0f L | únik %.0f L | toky=%s") % Gd.Arr(lbl, (S.G("t") / Gv.I(60L)), sim.C("vol", r), r.G("T"), sim.C("plato", r), (r.G("sg") + r.G("su")), S.G("hlt").G("w"), sim.C("skVolTotal"), (S.G("drainL") + S.G("spillL")), Gd.Dyn("str", S.G("flows"))))); return Gv.Null; });
    foreach (Gv rt in (Gd.Arr(Gd.Arr(Gv.S("hlt"), Gv.S("rvp"), Gd.Arr(Gv.S("phw"))), Gd.Arr(Gv.S("rvp"), Gv.S("sk"), Gd.Arr(Gv.S("p1"))), Gd.Arr(Gv.S("sk"), Gv.S("rvp"), Gd.Arr(Gv.S("p2"))), Gd.Arr(Gv.S("rvp"), Gv.S("wp"), Gd.Arr(Gv.S("p1"))), Gd.Arr(Gv.S("wp"), Gv.S("spilka"), Gd.Arr(Gv.S("p3"))), Gd.Arr(Gv.S("vodovod"), Gv.S("lv"), Gd.Arr()), Gd.Arr(Gv.S("hlt"), Gv.S("sk"), Gd.Arr(Gv.S("phw"))))).Iter()) {
      Gv vs = sim.C("route_valves", rt.I(Gv.I(0L)), rt.I(Gv.I(1L)), rt.I(Gv.I(2L)));
      Gv desc = Gd.Arr();
      if (Gv.Ne(vs, Gv.Null)) {
        foreach (Gv id in (vs).Iter()) {
          Gv v = sim.G("net").G("byId").C("get", id);
          desc.C("append", (Gv.S("%s(%s)") % Gd.Arr(id, (((sim.C("valve_open", id)).T() ? Gv.S("otevř") : Gv.S("ZAVŘ"))))));
        }
      }
      Gd.print((Gv.S("cesta %s→%s: %s") % Gd.Arr(rt.I(Gv.I(0L)), rt.I(Gv.I(1L)), (Gv.Eq(vs, Gv.Null) ? Gv.S("NENÍ") : Gv.S(", ").C("join", Gd.Ctor("PackedStringArray", desc))))));
    }
    foreach (Gv id_2 in (sim.C("route_valves", Gv.S("hlt"), Gv.S("rvp"), Gd.Arr(Gv.S("phw")))).Iter()) {
      if ((sim.G("net").G("byId").C("has", id_2)).T()) {
        sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(id_2, sim.G("net").G("byId").I(id_2).SI(Gv.S("open"), Gv.B(true)))));
      }
      if ((outlet_open.C("has", id_2)).T()) {
        outlet_open = outlet_open.SI(id_2, Gv.B(true));
      }
    }
    sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
    S = S.S("rvpWater", S.G("rvpWater").S("on", Gv.B(true)));
    Gv t = Gd.Static("Time", "get_ticks_usec");
    sim = sim.S("prof_on", Gv.B(true));
    sim = sim.S("prof", Gd.Dict());
    foreach (Gv i in (Gv.I(600L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
    }
    Gd.print((Gv.S("krok %.2f ms") % ((((Gd.Static("Time", "get_ticks_usec") - t)) / Gv.F(600000.0)))));
    sim = sim.S("prof_on", Gv.B(false));
    Gv pa = sim.C("lp");
    pa = pa.S("w", Gv.F(5.0));
    pa = pa.S("T", Gv.F(40.0));
    Gv pb = sim.C("lp");
    pb = pb.S("w", Gv.F(7.0));
    pb = pb.S("T", Gv.F(40.1));
    Gv t2 = Gd.Static("Time", "get_ticks_usec");
    foreach (Gv i_2 in (Gv.I(10000L)).Iter()) {
      sim.C("_similar", pa, pb);
    }
    Gd.print((Gv.S("  mb_similar %.1f us") % ((((Gd.Static("Time", "get_ticks_usec") - t2)) / Gv.F(10000.0)))));
    t2 = Gd.Static("Time", "get_ticks_usec");
    foreach (Gv i_3 in (Gv.I(10000L)).Iter()) {
      sim.C("vol", pa);
    }
    Gd.print((Gv.S("  mb_vol %.1f us") % ((((Gd.Static("Time", "get_ticks_usec") - t2)) / Gv.F(10000.0)))));
    t2 = Gd.Static("Time", "get_ticks_usec");
    Gv ee = sim.G("net").G("edges").I(Gv.I(0L));
    foreach (Gv i_4 in (Gv.I(10000L)).Iter()) {
      sim.C("edge_push", ee, pa, Gv.B(true));
    }
    Gd.print((Gv.S("  mb_push %.1f us (q %d)") % Gd.Arr((((Gd.Static("Time", "get_ticks_usec") - t2)) / Gv.F(10000.0)), ee.G("q").C("size"))));
    foreach (Gv k in (sim.G("prof")).Iter()) {
      Gd.print((Gv.S("  %s %.2f ms") % Gd.Arr(k, (sim.G("prof").I(k) / Gv.F(600000.0)))));
    }
    pr.C("call", Gv.S("voda"));
    S = S.S("rvp", S.G("rvp").S("stir", Gv.B(true)));
    S = S.S("malt", S.G("malt").S("gate", Gv.B(true)));
    S = S.S("malt", S.G("malt").S("on", Gv.B(true)));
    foreach (Gv i_5 in (Gv.I(1200L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
    }
    pr.C("call", Gv.S("vystírání"));
    S = S.S("rvp", S.G("rvp").S("heat", Gv.F(0.6)));
    foreach (Gv i_6 in (Gv.I(3600L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
    }
    pr.C("call", Gv.S("ohřev"));
    S = S.S("p1", S.G("p1").S("on", Gv.B(true)));
    foreach (Gv i_7 in (Gv.I(1200L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
    }
    pr.C("call", Gv.S("C1"));
    foreach (Gv id_3 in (sim.C("route_valves", Gv.S("rvp"), Gv.S("sk"), Gd.Arr(Gv.S("p1")))).Iter()) {
      if ((sim.G("net").G("byId").C("has", id_3)).T()) {
        sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(id_3, sim.G("net").G("byId").I(id_3).SI(Gv.S("open"), Gv.B(true)))));
      }
      if ((outlet_open.C("has", id_3)).T()) {
        outlet_open = outlet_open.SI(id_3, Gv.B(true));
      }
    }
    sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
    S = S.S("rvp", S.G("rvp").S("heat", Gv.F(0.0)));
    foreach (Gv i_8 in (Gv.I(120L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
    }
    Gd.print((Gv.S("C1 nastaveno %d L/min, skutečně %.0f L/min, stav '%s' | SK %.0f L, pánev %.0f L, kopačka/lože %.0f cm") % Gd.Arr(S.G("p1").G("flow"), S.G("p1").G("q"), S.G("p1").G("status"), sim.C("skVolTotal"), sim.C("vol", S.G("rvp")), (sim.C("skBedH") * Gv.F(100.0)))));
    foreach (Gv l in (S.G("log").C("slice", Gv.I(0L), Gv.I(25L))).Iter()) {
      Gd.print((Gv.S("  [%s] %d:%02d %s") % Gd.Arr(l.G("type"), Gd.Dyn("int", (l.G("t") / Gv.I(3600L))), (Gd.Dyn("int", (l.G("t") / Gv.I(60L))) % Gv.I(60L)), l.G("msg"))));
    }
    return Gv.Null;
  }
  public Gv make_plan_layers() {
    Gv sl = S_main_StaticLayer.New();
    sl = sl.S("m", Self);
    Self.C("add_child", sl);
    Self.C("move_child", sl, Gv.I(1L));
    Gv il = S_main_ItemLayer.New();
    il = il.S("m", Self);
    il = il.S("skip_valves", Gv.B(true));
    Self.C("add_child", il);
    Self.C("move_child", il, Gv.I(2L));
    return Gd.Arr(sl, il);
    return Gv.Null;
  }
  public Gv open_editor() {
    if (Gv.Eq(editor, Gv.Null)) {
      editor = Gd.Script("editor").C("new");
      editor = editor.S("m", Self);
      Self.C("add_child", editor);
      editor.C("setup");
    }
    editor.C("toggle");
    return Gv.Null;
  }
  public Gv _patch_pitch(Gv data = default(Gv)) {
    if (Gv.Ge(Gd.Dyn("int", data.C("get", Gv.S("_pitch2"), Gv.I(0L))), Gv.I(1L))) {
      return Gv.Null;
    }
    Gv its = data.C("get", Gv.S("items"), Gd.Arr());
    Gv by = Gd.Dict();
    foreach (Gv it in (its).Iter()) {
      by = by.SI(it.C("get", Gv.S("id"), Gv.S("")), it);
    }
    if ((by.C("has", Gv.S("pipe_murcuixph"))).T()) {
      by = by.SI(Gv.S("pipe_murcuixph"), by.I(Gv.S("pipe_murcuixph")).SI(Gv.S("pts"), Gd.Arr(Gd.Arr(Gv.I(4986L), Gv.I(924L)), Gd.Arr(Gv.I(5005L), Gv.I(996L)), Gd.Arr(Gv.I(5053L), Gv.I(1048L)), Gd.Arr(Gv.I(5100L), Gv.I(1070L)), Gd.Arr(Gv.I(5130L), Gv.I(1082L)))));
    }
    if ((by.C("has", Gv.S("fit_murcuixpp"))).T()) {
      by = by.SI(Gv.S("fit_murcuixpp"), by.I(Gv.S("fit_murcuixpp")).SI(Gv.S("x"), Gv.I(5160L)));
      by = by.SI(Gv.S("fit_murcuixpp"), by.I(Gv.S("fit_murcuixpp")).SI(Gv.S("y"), Gv.I(1082L)));
    }
    if ((by.C("has", Gv.S("inl_murcuixps"))).T()) {
      by = by.SI(Gv.S("inl_murcuixps"), by.I(Gv.S("inl_murcuixps")).SI(Gv.S("x"), Gv.I(5076L)));
      by = by.SI(Gv.S("inl_murcuixps"), by.I(Gv.S("inl_murcuixps")).SI(Gv.S("y"), Gv.I(1059L)));
      by = by.SI(Gv.S("inl_murcuixps"), by.I(Gv.S("inl_murcuixps")).SI(Gv.S("rot"), Gv.I(25L)));
    }
    if ((by.C("has", Gv.S("pipe_murcuixpt"))).T()) {
      by = by.SI(Gv.S("pipe_murcuixpt"), by.I(Gv.S("pipe_murcuixpt")).SI(Gv.S("pts"), Gd.Arr(Gd.Arr(Gv.I(5190L), Gv.I(1082L)), Gd.Arr(Gv.I(5190L), Gv.I(1140L)), Gd.Arr(Gv.I(5320L), Gv.I(1140L)))));
    }
    if ((by.C("has", Gv.S("pipe_murcuixpv"))).T()) {
      by = by.SI(Gv.S("pipe_murcuixpv"), by.I(Gv.S("pipe_murcuixpv")).SI(Gv.S("pts"), Gd.Arr(Gd.Arr(Gv.I(5160L), Gv.I(1112L)), Gd.Arr(Gv.I(5160L), Gv.I(1195L)))));
    }
    if ((by.C("has", Gv.S("valve_murcuixp2"))).T()) {
      by = by.SI(Gv.S("valve_murcuixp2"), by.I(Gv.S("valve_murcuixp2")).SI(Gv.S("x"), Gv.I(5160L)));
      by = by.SI(Gv.S("valve_murcuixp2"), by.I(Gv.S("valve_murcuixp2")).SI(Gv.S("y"), Gv.I(1154L)));
      by = by.SI(Gv.S("valve_murcuixp2"), by.I(Gv.S("valve_murcuixp2")).SI(Gv.S("rot"), Gv.I(90L)));
    }
    if ((by.C("has", Gv.S("valve_murcuixp1"))).T()) {
      its.C("erase", by.I(Gv.S("valve_murcuixp1")));
    }
    data = data.SI(Gv.S("_pitch2"), Gv.I(1L));
    return Gv.Null;
  }
  public Gv _patch_wpvalve(Gv data = default(Gv)) {
    if (Gv.Ge(Gd.Dyn("int", data.C("get", Gv.S("_wpv"), Gv.I(0L))), Gv.I(1L))) {
      return Gv.Null;
    }
    Gv its = data.C("get", Gv.S("items"), Gd.Arr());
    Gv tgt = Gv.Null;
    foreach (Gv it in (its).Iter()) {
      if (Gv.Eq(it.C("get", Gv.S("id"), Gv.S("")), Gv.S("pipe_murb0t3b"))) {
        tgt = it;
      }
      if (Gv.Eq(it.C("get", Gv.S("id"), Gv.S("")), Gv.S("valve_wp_odbocka"))) {
        return Gv.Null;
      }
    }
    if (Gv.Eq(tgt, Gv.Null)) {
      return Gv.Null;
    }
    Gv pts = tgt.C("get", Gv.S("pts"), Gd.Arr());
    Gv ok = Gv.B(false);
    foreach (Gv i in ((pts.C("size") - Gv.I(1L))).Iter()) {
      if (((((Gd.is_equal_approx(Gd.Dyn("float", pts.I(i).I(Gv.I(0L))), Gv.F(2460.0))).T() && (Gd.is_equal_approx(Gd.Dyn("float", pts.I((i + Gv.I(1L))).I(Gv.I(0L))), Gv.F(2460.0))).T()) && Gv.Le(Gd.minf(Gd.Dyn("float", pts.I(i).I(Gv.I(1L))), Gd.Dyn("float", pts.I((i + Gv.I(1L))).I(Gv.I(1L)))), Gv.F(470.0))) && Gv.Ge(Gd.maxf(Gd.Dyn("float", pts.I(i).I(Gv.I(1L))), Gd.Dyn("float", pts.I((i + Gv.I(1L))).I(Gv.I(1L)))), Gv.F(890.0)))) {
        ok = Gv.B(true);
      }
    }
    if (!(ok).T()) {
      return Gv.Null;
    }
    its.C("append", Gd.Dict(Gv.S("id"), Gv.S("valve_wp_odbocka"), Gv.S("type"), Gv.S("valve"), Gv.S("kind"), Gv.S("klapka"), Gv.S("label"), Gv.S("klapka"), Gv.S("open"), Gv.B(false), Gv.S("rot"), Gv.I(90L), Gv.S("x"), Gv.I(2460L), Gv.S("y"), Gv.I(600L), Gv.S("_on"), Gv.B(true)));
    data = data.SI(Gv.S("_wpv"), Gv.I(1L));
    return Gv.Null;
  }
  public Gv _patch_skstub(Gv data = default(Gv)) {
    if (Gv.Ge(Gd.Dyn("int", data.C("get", Gv.S("_skstub"), Gv.I(0L))), Gv.I(1L))) {
      return Gv.Null;
    }
    foreach (Gv it__it in (data.C("get", Gv.S("items"), Gd.Arr())).Iter()) {
      Gv it = it__it;
      if (Gv.Eq(it.C("get", Gv.S("id"), Gv.S("")), Gv.S("pipe_ed1103256632"))) {
        Gv pts = it.C("get", Gv.S("pts"), Gd.Arr());
        if (((Gv.Eq(pts.C("size"), Gv.I(4L)) && Gv.Eq(Gd.Dyn("int", pts.I(Gv.I(0L)).I(Gv.I(0L))), Gv.I(1520L))) && Gv.Eq(Gd.Dyn("int", pts.I(Gv.I(0L)).I(Gv.I(1L))), Gv.I(780L)))) {
          it = it.SI(Gv.S("pts"), Gd.Arr(Gd.Arr(Gv.I(1518L), Gv.I(800L)), Gd.Arr(Gv.I(1518L), Gv.I(760L)), Gd.Arr(Gv.I(1440L), Gv.I(760L)), Gd.Arr(Gv.I(1440L), Gv.I(800L))));
          data = data.SI(Gv.S("_skstub"), Gv.I(1L));
        }
        return Gv.Null;
      }
    }
    return Gv.Null;
  }
  public Gv _patch_skjoint(Gv data = default(Gv)) {
    if (Gv.Ge(Gd.Dyn("int", data.C("get", Gv.S("_skj"), Gv.I(0L))), Gv.I(1L))) {
      return Gv.Null;
    }
    Gv ring = Gv.B(false);
    Gv suc = Gv.B(false);
    foreach (Gv it in (data.C("get", Gv.S("items"), Gd.Arr())).Iter()) {
      Gv pts = it.C("get", Gv.S("pts"), Gd.Arr());
      if (Gv.Eq(it.C("get", Gv.S("id"), Gv.S("")), Gv.S("pipe_mur9keqg"))) {
        foreach (Gv q in (pts).Iter()) {
          if ((Gv.Eq(Gd.Dyn("int", q.I(Gv.I(0L))), Gv.I(1070L)) && Gv.Eq(Gd.Dyn("int", q.I(Gv.I(1L))), Gv.I(850L)))) {
            ring = Gv.B(true);
          }
        }
      }
      if ((((Gv.Eq(it.C("get", Gv.S("id"), Gv.S("")), Gv.S("pipe_mur9pa1c")) && Gv.Gt(pts.C("size"), Gv.I(1L))) && Gv.Eq(Gd.Dyn("int", pts.I(Gv.I(0L)).I(Gv.I(0L))), Gv.I(1070L))) && Gv.Eq(Gd.Dyn("int", pts.I(Gv.I(0L)).I(Gv.I(1L))), Gv.I(850L)))) {
        suc = Gv.B(true);
      }
    }
    if (!((ring).T() && (suc).T())) {
      return Gv.Null;
    }
    Gv js = data.C("get", Gv.S("joints"), Gd.Arr());
    foreach (Gv q_2 in (js).Iter()) {
      if ((Gv.Eq(Gd.Dyn("int", q_2.I(Gv.I(0L))), Gv.I(1070L)) && Gv.Eq(Gd.Dyn("int", q_2.I(Gv.I(1L))), Gv.I(850L)))) {
        data = data.SI(Gv.S("_skj"), Gv.I(1L));
        return Gv.Null;
      }
    }
    js.C("append", Gd.Arr(Gv.I(1070L), Gv.I(850L)));
    data = data.SI(Gv.S("joints"), js);
    data = data.SI(Gv.S("_skj"), Gv.I(1L));
    return Gv.Null;
  }
  public Gv _patch_splid(Gv data = default(Gv)) {
    Gv its = data.C("get", Gv.S("items"), Gd.Arr());
    foreach (Gv it in (its).Iter()) {
      if (Gv.Eq(it.C("get", Gv.S("k"), Gv.S("")), Gv.S("splid"))) {
        return Gv.Null;
      }
    }
    its.C("append", Gd.Dict(Gv.S("id"), Gv.S("deco_splid"), Gv.S("type"), Gv.S("deco"), Gv.S("k"), Gv.S("splid"), Gv.S("x"), Gv.I(5318L), Gv.S("y"), Gv.I(662L)));
    return Gv.Null;
  }
  public Gv _patch_ring(Gv data = default(Gv)) {
    if (Gv.Ge(Gd.Dyn("int", data.C("get", Gv.S("_ring"), Gv.I(0L))), Gv.I(1L))) {
      return Gv.Null;
    }
    foreach (Gv it__it in (data.C("get", Gv.S("items"), Gd.Arr())).Iter()) {
      Gv it = it__it;
      if (Gv.Eq(it.C("get", Gv.S("id"), Gv.S("")), Gv.S("pipe_mur9keqg"))) {
        it = it.SI(Gv.S("ringmul"), Gd.K("PI"));
        data = data.SI(Gv.S("_ring"), Gv.I(1L));
      }
    }
    return Gv.Null;
  }
  public Gv _patch_sk(Gv data = default(Gv)) {
    if ((Gv.Ge(Gd.Dyn("int", data.C("get", Gv.S("_sk3"), Gv.I(0L))), Gv.I(1L)) || Gv.Ne(Gd.Static("OS", "get_environment", Gv.S("NOSK")), Gv.S("")))) {
      return Gv.Null;
    }
    Gv its = data.C("get", Gv.S("items"), Gd.Arr());
    Gv by = Gd.Dict();
    foreach (Gv it in (its).Iter()) {
      by = by.SI(it.C("get", Gv.S("id"), Gv.S("")), it);
    }
    foreach (Gv id in (Gd.Arr(Gv.S("pipe_mur9i4w4"), Gv.S("pipe_mur9jylp"), Gv.S("pipe_mur9msnh"), Gv.S("valve_mur9o327"), Gv.S("valve_mur9o0mr"), Gv.S("valve_mur9ntsh"))).Iter()) {
      if (!(by.C("has", id)).T()) {
        return Gv.Null;
      }
    }
    foreach (Gv hid in (Gd.Arr(Gv.S("pipe_mur9ht2r"), Gv.S("pipe_mur9i4w4"), Gv.S("pipe_mur9jylp"), Gv.S("pipe_mur9mxq9"), Gv.S("pipe_mur9n2hh"), Gv.S("pipe_mur9na1o"))).Iter()) {
      if ((by.C("has", hid)).T()) {
        by = by.SI(hid, by.I(hid).SI(Gv.S("look"), Gv.S("hose")));
      }
    }
    by = by.SI(Gv.S("pipe_mur9jylp"), by.I(Gv.S("pipe_mur9jylp")).SI(Gv.S("pts"), Gd.Arr(Gd.Arr(Gv.I(1180L), Gv.I(800L)), Gd.Arr(Gv.I(1280L), Gv.I(800L)))));
    by = by.SI(Gv.S("pipe_mur9i4w4"), by.I(Gv.S("pipe_mur9i4w4")).SI(Gv.S("pts"), Gd.Arr(Gd.Arr(Gv.I(1280L), Gv.I(800L)), Gd.Arr(Gv.I(1590L), Gv.I(800L)), Gd.Arr(Gv.I(1590L), Gv.I(890L)))));
    by = by.SI(Gv.S("pipe_mur9msnh"), by.I(Gv.S("pipe_mur9msnh")).SI(Gv.S("pts"), Gd.Arr(Gd.Arr(Gv.I(1390L), Gv.I(850L)), Gd.Arr(Gv.I(1445L), Gv.I(850L)), Gd.Arr(Gv.I(1445L), Gv.I(890L)))));
    by.I(Gv.S("pipe_mur9msnh")).C("erase", Gv.S("look"));
    by = by.SI(Gv.S("valve_mur9ntsh"), by.I(Gv.S("valve_mur9ntsh")).SI(Gv.S("x"), Gv.I(1590L)));
    by = by.SI(Gv.S("valve_mur9ntsh"), by.I(Gv.S("valve_mur9ntsh")).SI(Gv.S("y"), Gv.I(852L)));
    by = by.SI(Gv.S("valve_mur9ntsh"), by.I(Gv.S("valve_mur9ntsh")).SI(Gv.S("rot"), Gv.I(90L)));
    by = by.SI(Gv.S("valve_mur9o327"), by.I(Gv.S("valve_mur9o327")).SI(Gv.S("x"), Gv.I(1435L)));
    by = by.SI(Gv.S("valve_mur9o327"), by.I(Gv.S("valve_mur9o327")).SI(Gv.S("y"), Gv.I(820L)));
    by = by.SI(Gv.S("valve_mur9o327"), by.I(Gv.S("valve_mur9o327")).SI(Gv.S("rot"), Gv.I(0L)));
    by = by.SI(Gv.S("valve_mur9o0mr"), by.I(Gv.S("valve_mur9o0mr")).SI(Gv.S("x"), Gv.I(1485L)));
    by = by.SI(Gv.S("valve_mur9o0mr"), by.I(Gv.S("valve_mur9o0mr")).SI(Gv.S("y"), Gv.I(820L)));
    by = by.SI(Gv.S("valve_mur9o0mr"), by.I(Gv.S("valve_mur9o0mr")).SI(Gv.S("rot"), Gv.I(0L)));
    if (!(by.C("has", Gv.S("pipe_sk_proplach"))).T()) {
      its.C("append", Gd.Dict(Gv.S("id"), Gv.S("pipe_sk_proplach"), Gv.S("type"), Gv.S("pipe"), Gv.S("medium"), Gv.S("mash"), Gv.S("dn"), Gv.I(25L), Gv.S("label"), Gv.S(""), Gv.S("look"), Gv.S("hose"), Gv.S("pts"), Gd.Arr(Gd.Arr(Gv.I(1530L), Gv.I(890L)), Gd.Arr(Gv.I(1530L), Gv.I(820L)), Gd.Arr(Gv.I(1390L), Gv.I(820L)))));
    } else {
      by = by.SI(Gv.S("pipe_sk_proplach"), by.I(Gv.S("pipe_sk_proplach")).SI(Gv.S("pts"), Gd.Arr(Gd.Arr(Gv.I(1530L), Gv.I(890L)), Gd.Arr(Gv.I(1530L), Gv.I(820L)), Gd.Arr(Gv.I(1390L), Gv.I(820L)))));
    }
    if (!(by.C("has", Gv.S("valve_sk_vystup"))).T()) {
      its.C("append", Gd.Dict(Gv.S("id"), Gv.S("valve_sk_vystup"), Gv.S("type"), Gv.S("valve"), Gv.S("kind"), Gv.S("klapka"), Gv.S("label"), Gv.S("klapka"), Gv.S("open"), Gv.B(false), Gv.S("rot"), Gv.I(0L), Gv.S("x"), Gv.I(1417L), Gv.S("y"), Gv.I(850L), Gv.S("_on"), Gv.B(true)));
    } else {
      by = by.SI(Gv.S("valve_sk_vystup"), by.I(Gv.S("valve_sk_vystup")).SI(Gv.S("x"), Gv.I(1417L)));
      by = by.SI(Gv.S("valve_sk_vystup"), by.I(Gv.S("valve_sk_vystup")).SI(Gv.S("y"), Gv.I(850L)));
      by = by.SI(Gv.S("valve_sk_vystup"), by.I(Gv.S("valve_sk_vystup")).SI(Gv.S("rot"), Gv.I(0L)));
    }
    Gv js = data.C("get", Gv.S("joints"), Gd.Arr());
    Gv nj = Gd.Arr();
    Gv dropl = Gd.Arr(Gd.Arr(Gv.I(1440L), Gv.I(890L)), Gd.Arr(Gv.I(1440L), Gv.I(850L)), Gd.Arr(Gv.I(1440L), Gv.I(830L)), Gd.Arr(Gv.I(1390L), Gv.I(830L)), Gd.Arr(Gv.I(1500L), Gv.I(890L)), Gd.Arr(Gv.I(1570L), Gv.I(890L)), Gd.Arr(Gv.I(1570L), Gv.I(830L)), Gd.Arr(Gv.I(1445L), Gv.I(890L)), Gd.Arr(Gv.I(1530L), Gv.I(890L)), Gd.Arr(Gv.I(1590L), Gv.I(890L)), Gd.Arr(Gv.I(1390L), Gv.I(820L)));
    foreach (Gv q in (js).Iter()) {
      Gv drop = Gv.B(false);
      foreach (Gv r in (dropl).Iter()) {
        if ((Gv.Eq(Gd.Dyn("int", q.I(Gv.I(0L))), r.I(Gv.I(0L))) && Gv.Eq(Gd.Dyn("int", q.I(Gv.I(1L))), r.I(Gv.I(1L))))) {
          drop = Gv.B(true);
        }
      }
      if (!(drop).T()) {
        nj.C("append", q);
      }
    }
    foreach (Gv r_2 in (Gd.Arr(Gd.Arr(Gv.I(1445L), Gv.I(890L)), Gd.Arr(Gv.I(1530L), Gv.I(890L)), Gd.Arr(Gv.I(1590L), Gv.I(890L)), Gd.Arr(Gv.I(1390L), Gv.I(820L)))).Iter()) {
      nj.C("append", Gd.Arr(r_2.I(Gv.I(0L)), r_2.I(Gv.I(1L))));
    }
    data = data.SI(Gv.S("joints"), nj);
    data = data.SI(Gv.S("_sk3"), Gv.I(1L));
    return Gv.Null;
  }
  public Gv _load_plan() {
    Gv path = Gv.S("res://plan.json");
    plan_user = Gv.B(((Gd.Static("FileAccess", "file_exists", USER_PLAN)).T() && !(Gv.In(Gv.S("--tests"), Gd.Static("OS", "get_cmdline_user_args")) || Gv.In(Gv.S("--autotest"), Gd.Static("OS", "get_cmdline_user_args")))));
    if ((plan_user).T()) {
      path = USER_PLAN;
    }
    Gv f = Gd.Static("FileAccess", "open", path, Gd.StaticGet("FileAccess", "READ"));
    if (Gv.Eq(f, Gv.Null)) {
      Gd.Dyn("push_error", (Gv.S("Plánek nenalezen: ") + path));
      return Gv.Null;
    }
    Gv data = Gd.Static("JSON", "parse_string", f.C("get_as_text"));
    if (Gv.Ne(Gd.Dyn("typeof", data), Gd.K("TYPE_DICTIONARY"))) {
      Gd.Dyn("push_error", Gv.S("Plánek nejde přečíst"));
      return Gv.Null;
    }
    if (!(plan_user).T()) {
      _patch_sk(data);
      _patch_ring(data);
    }
    _patch_splid(data);
    _patch_wpvalve(data);
    _patch_skjoint(data);
    _patch_skstub(data);
    _patch_pitch(data);
    plan = data;
    items = plan.C("get", Gv.S("items"), Gd.Arr());
    foreach (Gv it0__it in (items).Iter()) {
      Gv it0 = it0__it;
      if (((Gv.Eq(it0.C("get", Gv.S("type"), Gv.S("")), Gv.S("valve")) && (it0.C("has", Gv.S("part"))).T()) && !(can_throttle(it0)).T())) {
        it0.C("erase", Gv.S("part"));
      }
      if (Gv.Eq(it0.C("get", Gv.S("id"), Gv.S("")), Gv.S("pipe_murcuixph"))) {
        it0 = it0.SI(Gv.S("fixed"), Gv.B(true));
      }
    }
    compute_layout();
    return Gv.Null;
  }
  public Gv compute_layout() {
    Gv mn = Gd.Ctor("Vector2", Gv.F(1000000000.0), Gv.F(1000000000.0));
    Gv mx = Gd.Ctor("Vector2", Gv.F(-1000000000.0), Gv.F(-1000000000.0));
    foreach (Gv it in (items).Iter()) {
      if ((it.C("has", Gv.S("pts"))).T()) {
        foreach (Gv p in (it.G("pts")).Iter()) {
          mn = mn.C("min", Gd.Ctor("Vector2", p.I(Gv.I(0L)), p.I(Gv.I(1L))));
          mx = mx.C("max", Gd.Ctor("Vector2", p.I(Gv.I(0L)), p.I(Gv.I(1L))));
        }
      } else if ((it.C("has", Gv.S("x"))).T()) {
        Gv p0 = Gd.Ctor("Vector2", it.G("x"), it.C("get", Gv.S("y"), Gv.I(0L)));
        Gv p1 = (p0 + Gd.Ctor("Vector2", it.C("get", Gv.S("w"), Gv.I(0L)), it.C("get", Gv.S("h"), Gv.I(0L))));
        mn = mn.C("min", (p0 - Gd.Ctor("Vector2", Gv.I(60L), Gv.I(60L))));
        mx = mx.C("max", (p1 + Gd.Ctor("Vector2", Gv.I(60L), Gv.I(60L))));
      }
    }
    bounds = Gd.Ctor("Rect2", mn, (mx - mn)).C("grow", Gv.I(80L));
    floor_y = Gv.F(-1000000000.0);
    foreach (Gv it_2 in (items).Iter()) {
      if ((Gv.Eq(it_2.G("type"), Gv.S("vessel")) && Gv.In(it_2.C("get", Gv.S("shape")), Gd.Arr(Gv.S("tank"), Gv.S("lauter"), Gv.S("cone"), Gv.S("tall"), Gv.S("horiz"))))) {
        floor_y = Gd.maxf(floor_y, ((Gd.Dyn("float", it_2.G("y")) + Gd.Dyn("float", it_2.C("get", Gv.S("h"), Gv.I(0L)))) + Gv.F(38.0)));
      } else if (Gv.Eq(it_2.G("type"), Gv.S("pump"))) {
        floor_y = Gd.maxf(floor_y, (Gd.Dyn("float", it_2.G("y")) + Gv.F(30.0)));
      }
    }
    if (Gv.Lt(floor_y, bounds.G("position").G("y"))) {
      floor_y = (bounds.G("end").G("y") - Gv.F(200.0));
    }
    foreach (Gv v__it in (items).Iter()) {
      Gv v = v__it;
      if (Gv.Eq(v.G("type"), Gv.S("valve"))) {
        v = v.SI(Gv.S("_ang"), Gd.deg_to_rad(Gd.Dyn("float", v.C("get", Gv.S("rot"), Gv.I(0L)))));
      }
    }
    return Gv.Null;
  }
  public Gv foot_y(Gv it = default(Gv)) {
    Gv yd = ((Gd.Dyn("float", it.G("y")) + Gd.Dyn("float", it.C("get", Gv.S("h"), Gv.I(0L)))) + Gv.F(38.0));
    Gv gap = (floor_y - yd);
    if ((Gv.Ge(gap, Gv.F(-4.0)) && Gv.Lt(gap, Gv.F(30.0)))) {
      return ((floor_y - Gv.F(1.0))).AsF();
    }
    return (yd).AsF();
    return Gv.Null;
  }
  public Gv vp_size() {
    return Self.C("get_viewport").C("get_visible_rect").G("size");
    return Gv.Null;
  }
  public Gv screen_to_world(Gv sp = default(Gv)) {
    return (cam.G("position") + (((sp - (vp_size() * Gv.F(0.5)))) / cam.G("zoom").G("x")));
    return Gv.Null;
  }
  public Gv fit_all() {
    go_to(bounds);
    return Gv.Null;
  }
  public Gv go_to(Gv rect = default(Gv), Gv smooth = default(Gv)) {
    if (smooth.k == 0) smooth = Gv.B(false);
    Gv s = vp_size();
    Gv pw = right_w();
    Gv z = Gd.clampf(Gd.minf((((s.G("x") - pw)) / rect.G("size").G("x")), (((s.G("y") - Gv.I(60L))) / rect.G("size").G("y"))), Gv.F(0.03), Gv.F(4.0));
    Gv p = (rect.C("get_center") + (Gd.Ctor("Vector2", (pw * Gv.F(0.5)), Gv.F(-30.0)) / z));
    zoom_anim = Gv.B(false);
    vel = Gd.StaticGet("Vector2", "ZERO");
    if (((smooth).T() && Gv.Ne(game, Gv.Null))) {
      cam_to_p = p;
      cam_to_z = z;
      cam_anim = Gv.B(true);
      return Gv.Null;
    }
    zoom_target = z;
    cam = cam.S("zoom", Gd.Ctor("Vector2", z, z));
    cam = cam.S("position", p);
    cam_anim = Gv.B(false);
    return Gv.Null;
  }
  public Gv zoom_at(Gv factor = default(Gv), Gv screen_pos = default(Gv)) {
    factor = (factor).AsF();
    cam_anim = Gv.B(false);
    zoom_target = Gd.clampf((zoom_target * factor), Gv.F(0.03), Gv.F(4.0));
    zoom_anchor = screen_pos;
    zoom_anim = Gv.B(true);
    return Gv.Null;
  }
  public Gv _process(Gv delta = default(Gv)) {
    delta = (delta).AsF();
    if ((Gv.Eq(hud, Gv.Null) || Gv.Eq(cam, Gv.Null))) {
      return Gv.Null;
    }
    if ((zoom_anim).T()) {
      Gv z0 = cam.G("zoom").G("x");
      Gv z1 = (z0 * Gd.pow((zoom_target / z0), Gd.minf(Gv.F(1.0), (delta * Gv.F(14.0)))));
      if (Gv.Lt(Gd.absf(Gd.log((zoom_target / z1))), Gv.F(0.002))) {
        z1 = zoom_target;
        zoom_anim = Gv.B(false);
      }
      Gv w0 = screen_to_world(zoom_anchor);
      cam = cam.S("zoom", Gd.Ctor("Vector2", z1, z1));
      Gv w1 = screen_to_world(zoom_anchor);
      cam = cam.S("position", (cam.G("position") + (w0 - w1)));
    }
    if ((!(dragging).T() && Gv.Gt(vel.C("length"), Gv.F(2.0)))) {
      cam = cam.S("position", (cam.G("position") - ((vel * delta) / cam.G("zoom").G("x"))));
      vel = (vel * Gd.pow(Gv.F(0.02), delta));
    }
    Gv k = Gd.Ctor("Vector2", (Gd.Dyn("float", Gv.B(((Gd.Static("Input", "is_key_pressed", Gd.K("KEY_RIGHT"))).T() || (Gd.Static("Input", "is_key_pressed", Gd.K("KEY_D"))).T()))) - Gd.Dyn("float", Gv.B(((Gd.Static("Input", "is_key_pressed", Gd.K("KEY_LEFT"))).T() || (Gd.Static("Input", "is_key_pressed", Gd.K("KEY_A"))).T())))), (Gd.Dyn("float", Gv.B(((Gd.Static("Input", "is_key_pressed", Gd.K("KEY_DOWN"))).T() || (Gd.Static("Input", "is_key_pressed", Gd.K("KEY_S"))).T()))) - Gd.Dyn("float", Gv.B(((Gd.Static("Input", "is_key_pressed", Gd.K("KEY_UP"))).T() || (Gd.Static("Input", "is_key_pressed", Gd.K("KEY_W"))).T())))));
    if (Gv.Ne(k, Gd.StaticGet("Vector2", "ZERO"))) {
      cam_anim = Gv.B(false);
    }
    if ((cam_anim).T()) {
      Gv a = Gd.minf(Gv.F(1.0), (delta * Gv.F(7.0)));
      Gv zz = (Gd.exp(Gd.lerpf(Gd.log(cam.G("zoom").G("x")), Gd.log(cam_to_z), a))).AsF();
      cam = cam.S("zoom", Gd.Ctor("Vector2", zz, zz));
      cam = cam.S("position", cam.G("position").C("lerp", cam_to_p, a));
      if ((Gv.Lt(Gd.absf(Gd.log((zz / cam_to_z))), Gv.F(0.003)) && Gv.Lt(cam.G("position").C("distance_to", cam_to_p), Gv.F(1.0)))) {
        cam = cam.S("zoom", Gd.Ctor("Vector2", cam_to_z, cam_to_z));
        cam = cam.S("position", cam_to_p);
        cam_anim = Gv.B(false);
      }
      zoom_target = cam.G("zoom").G("x");
    }
    if (Gv.Ne(k, Gd.StaticGet("Vector2", "ZERO"))) {
      Gv sp = (Gv.F(900.0) * (((Gd.Static("Input", "is_key_pressed", Gd.K("KEY_SHIFT"))).T() ? Gv.F(2.5) : Gv.F(1.0))));
      cam = cam.S("position", (cam.G("position") + (((k * sp) * delta) / cam.G("zoom").G("x"))));
    }
    hud.C("update_info");
    if (((!(running).T() && Gv.Ne(sim, Gv.Null)) && Gv.Ne(sim.G("S").G("sk").G("hyd"), Gv.F(0.0)))) {
      sim = sim.S("S", sim.G("S").S("sk", sim.G("S").G("sk").S("rakeH", Gd.clampf((sim.G("S").G("sk").G("rakeH") + ((sim.G("S").G("sk").G("hyd") * delta) / Gv.F(15.0))), Gv.F(0.0), Gv.F(1.0)))));
    }
    if ((((running).T() && Gv.Ne(sim, Gv.Null)) && Gv.Ne(item_layer, Gv.Null))) {
      Gv dts = Gd.clampf((speed / Gv.F(30.0)), Gv.F(0.25), Gv.F(6.0));
      sim_acc = (sim_acc + (Gd.minf(delta, Gv.F(0.1)) * speed));
      Gv n = Gv.I(0L);
      Gv _ts = Gd.Static("Time", "get_ticks_usec");
      while (((Gv.Ge(sim_acc, dts) && Gv.Lt(n, Gv.I(6L))) && Gv.Lt((Gd.Static("Time", "get_ticks_usec") - _ts), Gv.I(8000L)))) {
        Gv _c = Gd.Static("Time", "get_ticks_usec");
        if (!(NOSIM).T()) {
          auto.C("tick", dts);
          sim.C("sim_step", dts);
          if ((Gv.Ne(practice, Gv.Null) && !(auto.G("on")).T())) {
            practice.C("tick", dts);
          }
        }
        T_sim = (T_sim + (Gd.Static("Time", "get_ticks_usec") - _c));
        N_sim = (N_sim + Gv.I(1L));
        sim_acc = (sim_acc - dts);
        n = (n + Gv.I(1L));
      }
      if (Gv.Gt(sim_acc, (dts * Gv.I(6L)))) {
        sim_acc = Gv.F(0.0);
      }
      dyn_t = (dyn_t + delta);
      if (Gv.Ge(dyn_t, ((Gv.Lt(speed, Gv.F(60.0)) ? Gv.F(0.05) : Gv.F(0.1))))) {
        dyn_t = Gv.F(0.0);
        Gv _a = Gd.Static("Time", "get_ticks_usec");
        if (!(NOLIVE).T()) {
          update_live();
        }
        T_live = (T_live + (Gd.Static("Time", "get_ticks_usec") - _a));
      }
    } else if ((zoom_anim).T()) {
      update_live();
    }
    save_acc = (save_acc + delta);
    if ((Gv.Gt(save_acc, Gv.F(20.0)) && (running).T())) {
      save_acc = Gv.F(0.0);
      save_game();
    }
    if (Gv.Ne(flow_layer, Gv.Null)) {
      flow_t = (flow_t + delta);
      if (Gv.Gt(flow_t, ((Gv.Lt(speed, Gv.F(60.0)) ? Gv.F(0.07) : Gv.F(0.12))))) {
        flow_t = Gv.F(0.0);
        calm_t = (calm_t + Gv.I(1L));
        if (Gv.Eq((calm_t % Gv.I(4L)), Gv.I(0L))) {
          _calm_tick();
        }
        Gv _b = Gd.Static("Time", "get_ticks_usec");
        if (!(NOFLOW).T()) {
          flow_layer.C("refresh");
        }
        T_flow = (T_flow + (Gd.Static("Time", "get_ticks_usec") - _b));
      }
    }
    _detail_tick(delta);
    if ((Gv.Ne(cabinet, Gv.Null) && !(running).T())) {
      cabinet.C("upd");
    }
    return Gv.Null;
  }
  public Gv _make_aa() {
    hud = hud.S("layer", Gv.I(3L));
    aa_layer = Gd.New("CanvasLayer");
    aa_layer = aa_layer.S("layer", Gv.I(2L));
    Self.C("add_child", aa_layer);
    Gv sh = Gd.New("Shader");
    sh = sh.S("code", AA_SHADER);
    Gv mat = Gd.New("ShaderMaterial");
    mat = mat.S("shader", sh);
    Gv cr = Gd.New("ColorRect");
    cr = cr.S("material", mat);
    cr = cr.S("mouse_filter", Gd.StaticGet("Control", "MOUSE_FILTER_IGNORE"));
    aa_layer.C("add_child", cr);
    cr.C("set_anchors_preset", Gd.StaticGet("Control", "PRESET_FULL_RECT"));
    return Gv.Null;
  }
  public Gv toggle_3d(Gv on = default(Gv)) {
    if (on.k == 0) on = Gv.Null;
    if (Gv.Eq(v3, Gv.Null)) {
      return Gv.Null;
    }
    Gv want = (Gv.Eq(on, Gv.Null) ? (Gv.B(!(v3.G("active")).T())) : Gd.Dyn("bool", on));
    if ((((want).T() && Gv.Ne(editor, Gv.Null)) && (editor.G("on")).T())) {
      return Gv.Null;
    }
    v3.C("set_active", want);
    if (Gv.Ne(aa_layer, Gv.Null)) {
      aa_layer = aa_layer.S("visible", Gv.B(!(want).T()));
    }
    hud.C("toast", ((want).T() ? Gv.S("3D pohled (V = zpět na 2D, X = průhledné nádoby)") : Gv.S("2D pohled")));
    return Gv.Null;
  }
  public Gv _unhandled_input(Gv e = default(Gv)) {
    if (((((Gv.Is(e, "InputEventKey") && (e.G("pressed")).T()) && !(e.G("echo")).T()) && Gv.Eq(e.G("keycode"), Gd.K("KEY_V"))) && !(Gv.Ne(editor, Gv.Null) && (editor.G("on")).T()))) {
      toggle_3d();
      return Gv.Null;
    }
    if ((Gv.Ne(v3, Gv.Null) && (v3.G("active")).T())) {
      if ((v3.C("handle", e)).T()) {
        return Gv.Null;
      }
      if ((Gv.Is(e, "InputEventMouseButton") || Gv.Is(e, "InputEventMouseMotion"))) {
        return Gv.Null;
      }
    }
    if (((Gv.Ne(editor, Gv.Null) && (editor.G("on")).T()) && (editor.C("handle", e)).T())) {
      return Gv.Null;
    }
    if (Gv.Is(e, "InputEventMouseButton")) {
      Gv mb = e;
      if ((((Gv.Eq(mb.G("button_index"), Gd.K("MOUSE_BUTTON_WHEEL_UP")) || Gv.Eq(mb.G("button_index"), Gd.K("MOUSE_BUTTON_WHEEL_DOWN"))) && (mb.G("pressed")).T()) && (mb.G("ctrl_pressed")).T())) {
        _valve_pct(screen_to_world(mb.G("position")), ((((mb.G("shift_pressed")).T() ? Gv.I(10L) : Gv.I(2L))) * ((Gv.Eq(mb.G("button_index"), Gd.K("MOUSE_BUTTON_WHEEL_UP")) ? Gv.I(1L) : Gv.I(-1L)))));
      } else if ((Gv.Eq(mb.G("button_index"), Gd.K("MOUSE_BUTTON_WHEEL_UP")) && (mb.G("pressed")).T())) {
        zoom_at(Gv.F(1.18), mb.G("position"));
      } else if ((Gv.Eq(mb.G("button_index"), Gd.K("MOUSE_BUTTON_WHEEL_DOWN")) && (mb.G("pressed")).T())) {
        zoom_at((Gv.F(1.0) / Gv.F(1.18)), mb.G("position"));
      } else if ((Gv.Eq(mb.G("button_index"), Gd.K("MOUSE_BUTTON_LEFT")) && (Gv.Ne(hose_drag, Gv.Null) || ((mb.G("pressed")).T() && (_hose_grab(mb.G("position"))).T())))) {
        if (!(mb.G("pressed")).T()) {
          _hose_drop();
        }
      } else if (Gv.In(mb.G("button_index"), Gd.Arr(Gd.K("MOUSE_BUTTON_LEFT"), Gd.K("MOUSE_BUTTON_RIGHT"), Gd.K("MOUSE_BUTTON_MIDDLE")))) {
        if ((mb.G("pressed")).T()) {
          if ((Gv.Eq(mb.G("button_index"), Gd.K("MOUSE_BUTTON_LEFT")) && Gv.Ne(cabinet, Gv.Null))) {
            Gv hk = cabinet.C("hit", screen_to_world(mb.G("position")));
            if ((Gv.Eq(hk, Gv.S("hup")) || Gv.Eq(hk, Gv.S("hdn")))) {
              cab_hold = hk;
              cabinet.C("act", hk, Gv.B(true));
              return Gv.Null;
            }
          }
          vdrag = Gv.Null;
          if ((Gv.Eq(mb.G("button_index"), Gd.K("MOUSE_BUTTON_LEFT")) && !(Gv.Ne(editor, Gv.Null) && (editor.G("on")).T()))) {
            vdrag = _valve_at(screen_to_world(mb.G("position")));
            if ((Gv.Ne(vdrag, Gv.Null) && !(can_throttle(vdrag)).T())) {
              vdrag = Gv.Null;
            }
            if (Gv.Ne(vdrag, Gv.Null)) {
              vdrag_pct = (Gv.Eq(vdrag.C("get", Gv.S("kind"), Gv.S("")), Gv.S("skrt")) ? Gd.Dyn("float", vdrag.C("get", Gv.S("pct"), Gv.I(50L))) : Gd.Dyn("float", vdrag.C("get", Gv.S("part"), (Gv.Ne(vdrag.C("get", Gv.S("open"), Gv.B(true)), Gv.B(false)) ? Gv.I(100L) : Gv.I(0L)))));
            }
          }
          dragging = Gv.B(true);
          drag_moved = Gv.B(false);
          drag_start = mb.G("position");
          vel = Gd.StaticGet("Vector2", "ZERO");
        } else {
          if (Gv.Ne(cab_hold, Gv.S(""))) {
            cabinet.C("act", cab_hold, Gv.B(false));
            cab_hold = Gv.S("");
            dragging = Gv.B(false);
            return Gv.Null;
          }
          dragging = Gv.B(false);
          if ((!(drag_moved).T() && Gv.Eq(mb.G("button_index"), Gd.K("MOUSE_BUTTON_LEFT")))) {
            _tap(screen_to_world(mb.G("position")));
          } else if ((!(drag_moved).T() && Gv.Eq(mb.G("button_index"), Gd.K("MOUSE_BUTTON_RIGHT")))) {
            if (!(_valve_slider(screen_to_world(mb.G("position")), mb.G("position"))).T()) {
              _throttle_step(screen_to_world(mb.G("position")), Gv.I(-10L));
            }
          }
          if (Gv.Gt((Gd.Static("Time", "get_ticks_msec") - last_move_ms), Gv.I(80L))) {
            vel = Gd.StaticGet("Vector2", "ZERO");
          }
        }
      }
    } else if ((Gv.Is(e, "InputEventMouseMotion") && Gv.Ne(hose_drag, Gv.Null))) {
      Gv mpos = (e).G("position");
      if ((!(hose_drag.G("moved")).T() && Gv.Gt(mpos.C("distance_to", hose_drag.G("start")), Gv.F(5.0)))) {
        hose_drag = hose_drag.S("moved", Gv.B(true));
      }
      if ((hose_drag.G("moved")).T()) {
        Gv it = hose_drag.G("pipe");
        Gv idx = (hose_drag.G("idx")).AsI();
        Gv w = screen_to_world(mpos);
        Gv o = outlet_at(w, (Gv.F(26.0) / Gd.minf(Gv.F(1.0), cam.G("zoom").G("x"))));
        if ((Gv.Ne(o, Gv.Null) && Gv.In(hose_at(o.G("front")), Gd.Arr(Gv.Null, Gd.Arr(it, idx))))) {
          w = o.G("front");
        }
        Gv other_i = (((it.G("pts").C("size") - Gv.I(1L)) - idx)).AsI();
        Gv other = Gd.Ctor("Vector2", it.G("pts").I(other_i).I(Gv.I(0L)), it.G("pts").I(other_i).I(Gv.I(1L)));
        if (Gv.Eq(idx, Gv.I(0L))) {
          hose_shape(it, w, other);
        } else {
          hose_shape(it, other, w);
        }
        redraw();
      }
    } else if ((Gv.Is(e, "InputEventMouseMotion") && (dragging).T())) {
      Gv mm = e;
      if ((!(drag_moved).T() && Gv.Gt(mm.G("position").C("distance_to", drag_start), Gv.F(5.0)))) {
        drag_moved = Gv.B(true);
      }
      if (((drag_moved).T() && Gv.Ne(vdrag, Gv.Null))) {
        if (!(vdrag.C("get", Gv.S("stuck"), Gv.B(false))).T()) {
          vdrag_pct = Gd.clampf((vdrag_pct + (((mm.G("relative").G("x") - mm.G("relative").G("y"))) * (((mm.G("shift_pressed")).T() ? Gv.F(0.17) : Gv.F(0.5))))), Gv.F(0.0), Gv.F(100.0));
          Gv pi_ = (Gd.roundi(vdrag_pct)).AsI();
          Gv cur = ((Gv.Eq(vdrag.C("get", Gv.S("kind"), Gv.S("")), Gv.S("skrt")) ? Gd.Dyn("int", vdrag.C("get", Gv.S("pct"), Gv.I(50L))) : Gd.Dyn("int", vdrag.C("get", Gv.S("part"), (Gv.Ne(vdrag.C("get", Gv.S("open"), Gv.B(true)), Gv.B(false)) ? Gv.I(100L) : Gv.I(0L)))))).AsI();
          if (Gv.Ne(pi_, cur)) {
            _set_valve_pct(vdrag, pi_);
            hud.C("toast", (Gv.S("%s: otevřeno %d %% (táhni vpravo/nahoru = víc, Shift = jemně)") % Gd.Arr(vdrag.C("get", Gv.S("label"), Gv.S("klapka")), pi_)));
          }
        }
        return Gv.Null;
      }
      if ((drag_moved).T()) {
        cam_anim = Gv.B(false);
        cam = cam.S("position", (cam.G("position") - (mm.G("relative") / cam.G("zoom").G("x"))));
        Gv dt = Gd.maxf(Gv.F(0.001), (((Gd.Static("Time", "get_ticks_msec") - last_move_ms)) / Gv.F(1000.0)));
        vel = vel.C("lerp", (mm.G("relative") / dt), Gv.F(0.35));
        last_move_ms = Gd.Static("Time", "get_ticks_msec");
      }
    } else if (Gv.Is(e, "InputEventMagnifyGesture")) {
      zoom_at((e).G("factor"), (e).G("position"));
    } else if (Gv.Is(e, "InputEventPanGesture")) {
      cam = cam.S("position", (cam.G("position") + (((e).G("delta") * Gv.F(30.0)) / cam.G("zoom").G("x"))));
    } else if ((Gv.Is(e, "InputEventKey") && (e.G("pressed")).T())) {
      {
        Gv _t1 = (e).G("keycode");
        if (Gv.Eq(_t1, Gd.K("KEY_PLUS")) || Gv.Eq(_t1, Gd.K("KEY_KP_ADD")) || Gv.Eq(_t1, Gd.K("KEY_EQUAL"))) {
          zoom_at(Gv.F(1.25), (vp_size() * Gv.F(0.5)));
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_MINUS")) || Gv.Eq(_t1, Gd.K("KEY_KP_SUBTRACT"))) {
          zoom_at(Gv.F(0.8), (vp_size() * Gv.F(0.5)));
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_HOME"))) {
          fit_all();
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_SPACE"))) {
          running = Gv.B(!(running).T());
          hud.C("toast", (Gv.S("Čas várky: ") + (((running).T() ? Gv.S("BĚŽÍ") : Gv.S("POZASTAVEN")))));
          if (Gv.Ne(game, Gv.Null)) {
            game.C("play", Gv.S("click"));
          }
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_1")) || Gv.Eq(_t1, Gd.K("KEY_2")) || Gv.Eq(_t1, Gd.K("KEY_3")) || Gv.Eq(_t1, Gd.K("KEY_4"))) {
          speed = Gd.Arr(Gv.F(1.0), Gv.F(10.0), Gv.F(60.0), Gv.F(180.0)).I(((e).G("keycode") - Gd.K("KEY_1")));
          running = Gv.B(true);
          hud.C("toast", (Gv.S("Rychlost x%d") % Gd.Dyn("int", speed)));
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_ENTER")) || Gv.Eq(_t1, Gd.K("KEY_KP_ENTER"))) {
          if (Gv.Ne(game, Gv.Null)) {
            game.C("confirm");
          }
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_Q"))) {
          if (Gv.Ne(game, Gv.Null)) {
            game.C("toggle_quest");
          }
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_TAB"))) {
          toggle_panel();
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_J"))) {
          toggle_hard();
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_R"))) {
          toggle_dock();
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_Z"))) {
          if (((e).G("ctrl_pressed")).T()) {
            undo();
          } else {
            toggle_dock_big();
          }
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_N"))) {
          if (Gv.Ne(game, Gv.Null)) {
            game.C("toggle_mute");
          }
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_F1")) || Gv.Eq(_t1, Gd.K("KEY_H"))) {
          if (Gv.Ne(game, Gv.Null)) {
            game.C("toggle_help");
          }
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_ESCAPE"))) {
          if ((Gv.Ne(game, Gv.Null) && (game.G("help").G("visible")).T())) {
            game.C("toggle_help");
          }
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_F5"))) {
          hud.C("_go", Gv.S("rvp"));
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_F6"))) {
          hud.C("_go", Gv.S("sk"));
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_F7"))) {
          hud.C("_go", Gv.S("wp"));
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_F8"))) {
          hud.C("_go", Gv.S("hlt"));
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_F9"))) {
          hud.C("_go", Gv.S("spilka"));
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_E"))) {
          open_editor();
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_M"))) {
          if (Gv.Ne(mini, Gv.Null)) {
            mini = mini.S("visible", Gv.B(!(mini.G("visible")).T()));
            hud.C("toast", (Gv.S("Mapka: ") + (((mini.G("visible")).T() ? Gv.S("zobrazena") : Gv.S("skrytá (M ji vrátí)")))));
          }
        }
      }
    }
    return Gv.Null;
  }
  public Gv save_game() {
    if ((Gv.Eq(sim, Gv.Null) || (sim.G("net").C("is_empty")).T())) {
      return Gv.Null;
    }
    Gv iv = Gd.Dict();
    foreach (Gv it in (items).Iter()) {
      {
        Gv _t1 = it.G("type");
        if (Gv.Eq(_t1, Gv.S("valve"))) {
          iv = iv.SI(it.G("id"), Gd.Dict(Gv.S("open"), it.C("get", Gv.S("open"), Gv.B(true))));
          if (Gv.Eq(it.C("get", Gv.S("kind"), Gv.S("")), Gv.S("skrt"))) {
            iv = iv.SI(it.G("id"), iv.I(it.G("id")).SI(Gv.S("pct"), it.C("get", Gv.S("pct"), Gv.I(50L))));
          }
          if ((it.C("has", Gv.S("part"))).T()) {
            iv = iv.SI(it.G("id"), iv.I(it.G("id")).SI(Gv.S("part"), it.G("part")));
          }
        }
        else if (Gv.Eq(_t1, Gv.S("pipe"))) {
          if ((it.C("get", Gv.S("hose"), Gv.B(false))).T()) {
            iv = iv.SI(it.G("id"), Gd.Dict(Gv.S("pts"), it.G("pts")));
            if ((it.C("get", Gv.S("user"), Gv.B(false))).T()) {
              iv = iv.SI(it.G("id"), Gd.Dict(Gv.S("pts"), it.G("pts"), Gv.S("user"), Gv.B(true), Gv.S("dn"), it.C("get", Gv.S("dn"), Gv.I(40L)), Gv.S("medium"), it.C("get", Gv.S("medium"), Gv.S("water")), Gv.S("id"), it.G("id"), Gv.S("type"), Gv.S("pipe"), Gv.S("hose"), Gv.B(true)));
            }
          }
        }
        else if (Gv.Eq(_t1, Gv.S("sw"))) {
          iv = iv.SI(it.G("id"), Gd.Dict(Gv.S("on"), it.C("get", Gv.S("on"), Gv.B(false))));
        }
        else if (Gv.Eq(_t1, Gv.S("vessel"))) {
          if (Gv.Eq(it.C("get", Gv.S("shape")), Gv.S("inlet"))) {
            iv = iv.SI(it.G("id"), Gd.Dict(Gv.S("on"), it.C("get", Gv.S("on"), Gv.B(false))));
          }
        }
      }
    }
    Gv ec = Gd.Dict();
    foreach (Gv e in (sim.G("net").G("edges")).Iter()) {
      ec = ec.SI(e.G("id"), Gd.Dict(Gv.S("c"), e.G("c"), Gv.S("q"), e.G("q"), Gv.S("from"), e.G("from")));
    }
    Gv f = Gd.Static("FileAccess", "open", save_path, Gd.StaticGet("FileAccess", "WRITE"));
    if (Gv.Eq(f, Gv.Null)) {
      return Gv.Null;
    }
    f.C("store_var", Gd.Dict(Gv.S("v"), Gv.I(1L), Gv.S("S"), sim.G("S"), Gv.S("items"), iv, Gv.S("outlets"), outlet_open, Gv.S("edges"), ec, Gv.S("speed"), speed, Gv.S("auto"), auto.C("to_dict"), Gv.S("running"), running, Gv.S("plan_rev"), PLAN_REV));
    f.C("close");
    return Gv.Null;
  }
  public Gv load_game() {
    if (!(Gd.Static("FileAccess", "file_exists", save_path)).T()) {
      return Gv.B(false);
    }
    Gv f = Gd.Static("FileAccess", "open", save_path, Gd.StaticGet("FileAccess", "READ"));
    if (Gv.Eq(f, Gv.Null)) {
      return Gv.B(false);
    }
    Gv d = f.C("get_var");
    f.C("close");
    if (((Gv.Ne(Gd.Dyn("typeof", d), Gd.K("TYPE_DICTIONARY")) || Gv.Ne(d.C("get", Gv.S("v"), Gv.I(0L)), Gv.I(1L))) || Gv.Ne(Gd.Dyn("typeof", d.C("get", Gv.S("S"))), Gd.K("TYPE_DICTIONARY")))) {
      return Gv.B(false);
    }
    Gv iv = d.G("items");
    foreach (Gv uid in (iv).Iter()) {
      if (((Gd.Ctor("String", uid).C("begins_with", Gv.S("hose_user_"))).T() && (iv.I(uid).C("get", Gv.S("user"), Gv.B(false))).T())) {
        Gv have = Gv.B(false);
        foreach (Gv it0 in (items).Iter()) {
          if (Gv.Eq(it0.G("id"), uid)) {
            have = Gv.B(true);
          }
        }
        if (!(have).T()) {
          items.C("append", iv.I(uid).C("duplicate", Gv.B(true)));
        }
      }
    }
    foreach (Gv it__it in (items).Iter()) {
      Gv it = it__it;
      Gv s = iv.C("get", it.G("id"));
      if (Gv.Eq(s, Gv.Null)) {
        continue;
      }
      foreach (Gv k in (s).Iter()) {
        if (((Gv.Eq(k, Gv.S("pts")) && Gv.Ne(d.C("get", Gv.S("plan_rev"), Gv.I(0L)), PLAN_REV)) && !(it.C("get", Gv.S("user"), Gv.B(false))).T())) {
          continue;
        }
        if ((Gv.Eq(k, Gv.S("pts")) && (it.C("get", Gv.S("fixed"), Gv.B(false))).T())) {
          continue;
        }
        if (((Gv.Eq(k, Gv.S("part")) && Gv.Eq(it.C("get", Gv.S("type"), Gv.S("")), Gv.S("valve"))) && !(can_throttle(it)).T())) {
          continue;
        }
        if (((Gv.Eq(k, Gv.S("pct")) && Gv.Eq(it.C("get", Gv.S("type"), Gv.S("")), Gv.S("valve"))) && Gv.Ne(it.C("get", Gv.S("kind"), Gv.S("")), Gv.S("skrt")))) {
          continue;
        }
        it = it.SI(k, s.I(k));
      }
    }
    foreach (Gv k_2 in (d.G("outlets")).Iter()) {
      if ((outlet_open.C("has", k_2)).T()) {
        outlet_open = outlet_open.SI(k_2, d.G("outlets").I(k_2));
      }
    }
    Gv had_mode = d.G("S").G("sk").C("has", Gv.S("mode"));
    sim = sim.S("S", d.G("S"));
    Gv fresh = S_sim.New();
    fresh = fresh.S("main", Self);
    fresh.C("new_state");
    _merge_defaults(sim.G("S"), fresh.G("S"));
    Gv old_mu = Gv.B(!(sim.G("S").G("cfg").C("has", Gv.S("skMuP"))).T());
    _merge_defaults(sim.G("S").G("cfg"), fresh.G("S").G("cfg"));
    if (((old_mu).T() && Gv.Lt(Gd.absf((Gd.Dyn("float", sim.G("S").G("cfg").G("skPerm")) - Gv.F(1e-11))), Gv.F(1e-13)))) {
      sim = sim.S("S", sim.G("S").S("cfg", sim.G("S").G("cfg").S("skPerm", Gv.F(1.5e-11))));
    }
    if (Gv.Lt(Gd.absf((Gd.Dyn("float", sim.G("S").G("cfg").C("get", Gv.S("skRetentionLkg"), Gv.F(1.0))) - Gv.F(1.56))), Gv.F(1e-06))) {
      sim = sim.S("S", sim.G("S").S("cfg", sim.G("S").G("cfg").S("skRetentionLkg", Gv.F(1.0))));
    }
    _fix_parts(sim.G("S"));
    sim = sim.S("calib_applied", sim.C("apply_calibration", sim.G("S").G("cfg")));
    if ((sim.G("calib_applied").C("size")).T()) {
      sim.C("logm", (Gv.S("Kalibrace z kalibrace.json (i pro uloženou várku): %s") % Gv.S(", ").C("join", Gd.Ctor("PackedStringArray", sim.G("calib_applied")))));
    }
    if ((sim.G("calib_rejected").C("size")).T()) {
      sim.C("logm", (Gv.S("Kalibrace – odmítnuto: %s") % Gv.S("; ").C("join", Gd.Ctor("PackedStringArray", sim.G("calib_rejected")))), Gv.S("warn"));
    }
    if (!(sim.G("S").G("cab").C("get", Gv.S("estop"), Gv.B(false))).T()) {
      sim = sim.S("S", sim.G("S").S("cab", sim.G("S").G("cab").S("key", Gv.B(true))));
    }
    if (Gv.Lt(Gd.Dyn("float", sim.G("S").G("cfg").C("get", Gv.S("skPerm"), Gv.F(1e-11))), Gv.F(5e-12))) {
      sim = sim.S("S", sim.G("S").S("cfg", sim.G("S").G("cfg").S("skPerm", Gv.F(1e-11))));
    }
    if ((!(had_mode).T() && Gv.Gt(sim.G("S").G("sk").G("g"), Gv.I(1L)))) {
      sim = sim.S("S", sim.G("S").S("sk", sim.G("S").G("sk").S("mode", Gv.S("lauter"))));
    }
    sim.C("build_net", items, plan.C("get", Gv.S("joints"), Gd.Arr()));
    foreach (Gv e__it in (sim.G("net").G("edges")).Iter()) {
      Gv e = e__it;
      Gv s2 = d.G("edges").C("get", e.G("id"));
      if (Gv.Ne(s2, Gv.Null)) {
        e = e.S("c", s2.G("c"));
        e = e.S("from", s2.G("from"));
        foreach (Gv k0 in (sim.C("lp")).Iter()) {
          if (!(e.G("c").C("has", k0)).T()) {
            e = e.S("c", e.G("c").SI(k0, Gv.F(0.0)));
          }
        }
        if ((s2.C("has", Gv.S("q"))).T()) {
          e = e.S("q", s2.G("q"));
        } else if (Gv.Gt(sim.C("vol", e.G("c")), Gv.F(1e-09))) {
          e = e.S("q", Gd.Arr(e.G("c").C("duplicate")));
        }
        foreach (Gv b__it in (e.G("q")).Iter()) {
          Gv b = b__it;
          foreach (Gv k_3 in (sim.C("lp")).Iter()) {
            if (!(b.C("has", k_3)).T()) {
              b = b.SI(k_3, Gv.F(0.0));
            }
          }
        }
        foreach (Gv k_4 in (sim.C("lp")).Iter()) {
          if (!(e.G("c").C("has", k_4)).T()) {
            e = e.S("c", e.G("c").SI(k_4, Gv.F(0.0)));
          }
        }
      }
    }
    speed = d.C("get", Gv.S("speed"), Gv.F(10.0));
    auto = auto.S("S", sim.G("S"));
    if ((d.C("has", Gv.S("auto"))).T()) {
      auto.C("from_dict", d.G("auto"));
    }
    return Gv.B(true);
    return Gv.Null;
  }
  public Gv show_route(Gv src = default(Gv), Gv dst = default(Gv), Gv pass2 = default(Gv), Gv do_open = default(Gv), Gv where = default(Gv), Gv extra = default(Gv)) {
    if (where.k == 0) where = Gv.I(-1L);
    if (extra.k == 0) extra = Gd.Arr();
    Gv vs = sim.C("route_valves", src, dst, pass2, where);
    if (Gv.Eq(vs, Gv.Null)) {
      hud.C("toast", Gv.S("Cesta ve schématu neexistuje (chybí trubka nebo čerpadlo)."));
      return Gv.Null;
    }
    foreach (Gv ex in (extra).Iter()) {
      if ((!(vs.C("has", ex)).T() && (sim.G("net").G("byId").C("has", ex)).T())) {
        vs.C("append", ex);
      }
    }
    Gv need = Gd.Arr();
    foreach (Gv id in (vs).Iter()) {
      Gv open = sim.C("valve_open", id);
      if (((do_open).T() && !(open).T())) {
        if ((outlet_open.C("has", id)).T()) {
          outlet_open = outlet_open.SI(id, Gv.B(true));
        } else if ((sim.G("net").G("byId").C("has", id)).T()) {
          sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(id, sim.G("net").G("byId").I(id).SI(Gv.S("open"), Gv.B(true)))));
          if ((Gv.Eq(sim.G("net").G("byId").I(id).C("get", Gv.S("kind"), Gv.S("")), Gv.S("skrt")) && Gv.Eq(Gd.Dyn("int", sim.G("net").G("byId").I(id).C("get", Gv.S("pct"), Gv.I(50L))), Gv.I(0L)))) {
            sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(id, sim.G("net").G("byId").I(id).SI(Gv.S("pct"), Gv.I(100L)))));
          }
        }
      } else if (!(open).T()) {
        need.C("append", id);
      }
    }
    hl.C("clear");
    if (((do_open).T() && Gv.Eq(src, Gv.S("vodovod")))) {
      foreach (Gv it__it in (items).Iter()) {
        Gv it = it__it;
        if (((Gv.Eq(it.G("type"), Gv.S("vessel")) && Gv.Eq(it.C("get", Gv.S("bind")), Gv.S("vodovod"))) && Gv.Eq(it.C("get", Gv.S("shape")), Gv.S("inlet")))) {
          it = it.SI(Gv.S("on"), Gv.B(true));
        }
      }
    }
    if ((do_open).T()) {
      sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
      hud.C("toast", (Gv.S("Cesta otevřena (%d klapek)%s. Zavřené klapky mimo cestu zůstaly zavřené.") % Gd.Arr(vs.C("size"), (Gv.Eq(src, Gv.S("vodovod")) ? Gv.S(", spoušť řadu puštěna") : Gv.S("")))));
    } else {
      foreach (Gv id_2 in (need).Iter()) {
        hl = hl.SI(id_2, Gv.B(true));
      }
      hud.C("toast", ((need.C("size")).T() ? (Gv.S("Cesta: otevři %d klapek (žluté kroužky).") % need.C("size")) : Gv.S("Cesta je celá otevřená.")));
    }
    redraw();
    return Gv.Null;
  }
  public Gv _fix_parts(Gv x = default(Gv)) {
    if (Gv.Is(x, "Dictionary")) {
      if (((x.C("has", Gv.S("aB"))).T() && !(x.C("has", Gv.S("aBs"))).T())) {
        x = x.SI(Gv.S("aBs"), Gv.F(0.0));
      }
      foreach (Gv k in (x).Iter()) {
        _fix_parts(x.I(k));
      }
    } else if (Gv.Is(x, "Array")) {
      foreach (Gv y in (x).Iter()) {
        _fix_parts(y);
      }
    }
    return Gv.Null;
  }
  public Gv _merge_defaults(Gv dst = default(Gv), Gv src = default(Gv)) {
    foreach (Gv k in (src).Iter()) {
      if (!(dst.C("has", k)).T()) {
        dst = dst.SI(k, src.I(k));
      } else if ((Gv.Eq(Gd.Dyn("typeof", dst.I(k)), Gd.K("TYPE_DICTIONARY")) && Gv.Eq(Gd.Dyn("typeof", src.I(k)), Gd.K("TYPE_DICTIONARY")))) {
        _merge_defaults(dst.I(k), src.I(k));
      }
    }
    return Gv.Null;
  }
  public Gv open_iodine(Gv src = default(Gv)) {
    if ((Gv.Ne(iodine_win, Gv.Null) && (Gd.is_instance_valid(iodine_win)).T())) {
      iodine_win.C("queue_free");
    }
    iodine_win = S_iodine.New();
    iodine_win = iodine_win.S("m", Self);
    hud.C("add_child", iodine_win);
    iodine_win.C("open", src);
    return Gv.Null;
  }
  public Gv open_sight_c2() {
    foreach (Gv it0 in (items).Iter()) {
      if (Gv.Eq(it0.G("id"), Gv.S("inl_mur9u2hg"))) {
        if ((Gv.Ne(sight_win, Gv.Null) && (Gd.is_instance_valid(sight_win)).T())) {
          sight_win.C("queue_free");
        }
        sight_win = S_sight.New();
        sight_win = sight_win.S("m", Self);
        hud.C("add_child", sight_win);
        sight_win.C("open", it0);
        return Gv.Null;
      }
    }
    return Gv.Null;
  }
  public Gv new_game() {
    Gd.Static("DirAccess", "remove_absolute", Gd.Static("ProjectSettings", "globalize_path", save_path));
    Self.C("get_tree").C("reload_current_scene");
    return Gv.Null;
  }
  public Gv _notification(Gv what = default(Gv)) {
    what = (what).AsI();
    if (Gv.Eq(what, Gd.K("NOTIFICATION_WM_CLOSE_REQUEST"))) {
      save_game();
    }
    return Gv.Null;
  }
  public Gv _hose_grab(Gv sp = default(Gv)) {
    Gv w = screen_to_world(sp);
    Gv h = hose_at(w, (Gv.F(22.0) / Gd.minf(Gv.F(1.0), cam.G("zoom").G("x"))), Gv.B(true));
    if (Gv.Eq(h, Gv.Null)) {
      return Gv.B(false);
    }
    hose_drag = Gd.Dict(Gv.S("pipe"), h.I(Gv.I(0L)), Gv.S("idx"), h.I(Gv.I(1L)), Gv.S("start"), sp, Gv.S("moved"), Gv.B(false), Gv.S("orig"), h.I(Gv.I(0L)).G("pts").C("duplicate", Gv.B(true)));
    vel = Gd.StaticGet("Vector2", "ZERO");
    return Gv.B(true);
    return Gv.Null;
  }
  public Gv _hose_drop() {
    Gv d = hose_drag;
    hose_drag = Gv.Null;
    if (!(d.G("moved")).T()) {
      _tap(screen_to_world(d.G("start")));
      return Gv.Null;
    }
    Gv it = d.G("pipe");
    Gv idx = (d.G("idx")).AsI();
    Gv end = Gd.Ctor("Vector2", it.G("pts").I(idx).I(Gv.I(0L)), it.G("pts").I(idx).I(Gv.I(1L)));
    Gv o = outlet_at(end, Gv.F(2.0));
    if (Gv.Ne(o, Gv.Null)) {
      hud.C("toast", (Gv.S("Hadice nasazena na vývod – klapka vývodu je %s.") % (((outlet_open.I(o.G("key"))).T() ? Gv.S("otevřená") : Gv.S("ZAVŘENÁ (ťukni na ni)")))));
    } else {
      hud.C("toast", Gv.S("Konec hadice visí volně – co do ní poteče, vyteče na zem!"));
    }
    sim.C("build_net", items, plan.C("get", Gv.S("joints"), Gd.Arr()));
    if (Gv.Ne(flow_layer, Gv.Null)) {
      flow_layer.C("build");
    }
    redraw();
    return Gv.Null;
  }
  public Gv _throttle_step(Gv w = default(Gv), Gv step = default(Gv)) {
    step = (step).AsI();
    Gv best = Gv.Null;
    Gv bd = (Gv.Ne(cam, Gv.Null) ? Gd.maxf(Gv.F(18.0), Gd.minf(Gv.F(40.0), (Gv.F(26.0) / cam.G("zoom").G("x")))) : Gv.F(18.0));
    foreach (Gv it in (items).Iter()) {
      if ((Gv.Eq(it.G("type"), Gv.S("valve")) && Gv.Eq(it.C("get", Gv.S("kind"), Gv.S("")), Gv.S("skrt")))) {
        Gv d = w.C("distance_to", Gd.Ctor("Vector2", it.G("x"), it.G("y")));
        if (Gv.Lt(d, bd)) {
          bd = d;
          best = it;
        }
      }
    }
    if (Gv.Eq(best, Gv.Null)) {
      return Gv.Null;
    }
    Gv pct = Gd.Dyn("int", best.C("get", Gv.S("pct"), Gv.I(50L)));
    if (Gv.Gt(step, Gv.I(0L))) {
      pct = (Gv.Ge(pct, Gv.I(100L)) ? Gv.I(0L) : mini.Call(Gv.I(100L), (pct + step)));
    } else {
      pct = Gd.maxi(Gv.I(0L), (pct + step));
    }
    best = best.SI(Gv.S("pct"), pct);
    best = best.SI(Gv.S("open"), Gv.B(Gv.Gt(pct, Gv.I(0L))));
    if (((sim.G("net").C("has", Gv.S("byId"))).T() && (sim.G("net").G("byId").C("has", best.G("id"))).T())) {
      sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(best.G("id"), sim.G("net").G("byId").I(best.G("id")).SI(Gv.S("pct"), pct))));
      sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(best.G("id"), sim.G("net").G("byId").I(best.G("id")).SI(Gv.S("open"), Gv.B(Gv.Gt(pct, Gv.I(0L)))))));
    }
    sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
    redraw();
    hud.C("toast", (Gv.S("%s: otevřeno %d %% (levé tlačítko +10 %%, pravé -10 %%)") % Gd.Arr(best.C("get", Gv.S("label"), Gv.S("škrtidlo")), pct)));
    return Gv.Null;
  }
  public Gv _valve_pct(Gv w = default(Gv), Gv step = default(Gv)) {
    step = (step).AsI();
    Gv best = Gv.Null;
    Gv bd = (Gv.Ne(cam, Gv.Null) ? Gd.maxf(Gv.F(18.0), Gd.minf(Gv.F(40.0), (Gv.F(26.0) / cam.G("zoom").G("x")))) : Gv.F(18.0));
    foreach (Gv it in (items).Iter()) {
      if ((Gv.Eq(it.G("type"), Gv.S("valve")) && Gv.In(it.C("get", Gv.S("kind"), Gv.S("")), Gd.Arr(Gv.S("klapka"), Gv.S("belimo"), Gv.S("kulovy"))))) {
        Gv d = w.C("distance_to", Gd.Ctor("Vector2", it.G("x"), it.G("y")));
        if (Gv.Lt(d, bd)) {
          bd = d;
          best = it;
        }
      }
    }
    if (Gv.Eq(best, Gv.Null)) {
      _throttle_step(w, step);
      return Gv.Null;
    }
    if ((best.C("get", Gv.S("stuck"), Gv.B(false))).T()) {
      hud.C("toast", (Gv.S("%s je zadřená – nejde přestavit.") % best.C("get", Gv.S("label"), Gv.S("Klapka"))));
      return Gv.Null;
    }
    if (!(can_throttle(best)).T()) {
      hud.C("toast", Gv.S("Tahle klapka je jen otevřeno / zavřeno (klik)."));
      return Gv.Null;
    }
    Gv pct = (Gd.Dyn("int", best.C("get", Gv.S("part"), (Gv.Ne(best.C("get", Gv.S("open"), Gv.B(true)), Gv.B(false)) ? Gv.I(100L) : Gv.I(0L))))).AsI();
    _set_valve_pct(best, Gd.clampi((pct + step), Gv.I(0L), Gv.I(100L)));
    hud.C("toast", (Gv.S("%s: otevřeno %d %% (Ctrl + kolečko 2 %, Ctrl+Shift 10 %, pravé tlačítko = posuvník po 1 %)") % Gd.Arr(best.C("get", Gv.S("label"), Gv.S("klapka")), Gd.Dyn("int", best.C("get", Gv.S("part"), (Gv.Ne(best.C("get", Gv.S("open"), Gv.B(true)), Gv.B(false)) ? Gv.I(100L) : Gv.I(0L)))))));
    return Gv.Null;
  }
  public Gv _set_valve_pct(Gv best = default(Gv), Gv pct = default(Gv)) {
    pct = (pct).AsI();
    if (Gv.Eq(best.C("get", Gv.S("kind"), Gv.S("")), Gv.S("skrt"))) {
      best = best.SI(Gv.S("pct"), pct);
      best = best.SI(Gv.S("open"), Gv.B(Gv.Gt(pct, Gv.I(0L))));
      if (((sim.G("net").C("has", Gv.S("byId"))).T() && (sim.G("net").G("byId").C("has", best.G("id"))).T())) {
        sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(best.G("id"), sim.G("net").G("byId").I(best.G("id")).SI(Gv.S("pct"), pct))));
        sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(best.G("id"), sim.G("net").G("byId").I(best.G("id")).SI(Gv.S("open"), Gv.B(Gv.Gt(pct, Gv.I(0L)))))));
      }
      sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
      redraw_valves();
      return Gv.Null;
    }
    best = best.SI(Gv.S("open"), Gv.B(Gv.Gt(pct, Gv.I(0L))));
    if ((Gv.Ge(pct, Gv.I(100L)) || Gv.Le(pct, Gv.I(0L)))) {
      best.C("erase", Gv.S("part"));
    } else {
      best = best.SI(Gv.S("part"), pct);
    }
    if (((sim.G("net").C("has", Gv.S("byId"))).T() && (sim.G("net").G("byId").C("has", best.G("id"))).T())) {
      sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(best.G("id"), sim.G("net").G("byId").I(best.G("id")).SI(Gv.S("open"), Gv.B(Gv.Gt(pct, Gv.I(0L)))))));
      if ((Gv.Ge(pct, Gv.I(100L)) || Gv.Le(pct, Gv.I(0L)))) {
        sim.G("net").G("byId").I(best.G("id")).C("erase", Gv.S("part"));
      } else {
        sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(best.G("id"), sim.G("net").G("byId").I(best.G("id")).SI(Gv.S("part"), pct))));
      }
    }
    sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
    redraw_valves();
    return Gv.Null;
  }
  public Gv can_throttle(Gv it = default(Gv)) {
    return Gv.B((Gv.Ne(it, Gv.Null) && (THROTTLE_IDS.C("has", Gd.Dyn("str", it.C("get", Gv.S("id"), Gv.S(""))))).T()));
    return Gv.Null;
  }
  public Gv _valve_at(Gv w = default(Gv)) {
    Gv best = Gv.Null;
    Gv bd = (Gv.Ne(cam, Gv.Null) ? Gd.maxf(Gv.F(14.0), Gd.minf(Gv.F(30.0), (Gv.F(20.0) / cam.G("zoom").G("x")))) : Gv.F(14.0));
    foreach (Gv it in (items).Iter()) {
      if ((Gv.Eq(it.G("type"), Gv.S("valve")) && Gv.In(it.C("get", Gv.S("kind"), Gv.S("")), Gd.Arr(Gv.S("klapka"), Gv.S("belimo"), Gv.S("kulovy"), Gv.S("skrt"))))) {
        Gv d = w.C("distance_to", Gd.Ctor("Vector2", it.G("x"), it.G("y")));
        if (Gv.Lt(d, bd)) {
          bd = d;
          best = it;
        }
      }
    }
    return best;
    return Gv.Null;
  }
  public Gv _valve_slider(Gv w = default(Gv), Gv spos = default(Gv)) {
    Gv best = Gv.Null;
    Gv bd = (Gv.Ne(cam, Gv.Null) ? Gd.maxf(Gv.F(18.0), Gd.minf(Gv.F(40.0), (Gv.F(26.0) / cam.G("zoom").G("x")))) : Gv.F(18.0));
    foreach (Gv it in (items).Iter()) {
      if ((Gv.Eq(it.G("type"), Gv.S("valve")) && Gv.In(it.C("get", Gv.S("kind"), Gv.S("")), Gd.Arr(Gv.S("klapka"), Gv.S("belimo"), Gv.S("kulovy"), Gv.S("skrt"))))) {
        Gv d = w.C("distance_to", Gd.Ctor("Vector2", it.G("x"), it.G("y")));
        if (Gv.Lt(d, bd)) {
          bd = d;
          best = it;
        }
      }
    }
    if (Gv.Eq(best, Gv.Null)) {
      return Gv.B(false);
    }
    if (!(can_throttle(best)).T()) {
      return Gv.B(false);
    }
    if ((best.C("get", Gv.S("stuck"), Gv.B(false))).T()) {
      hud.C("toast", (Gv.S("%s je zadřená – nejde přestavit.") % best.C("get", Gv.S("label"), Gv.S("Klapka"))));
      return Gv.B(true);
    }
    if ((Gv.Ne(valve_pop, Gv.Null) && (Gd.is_instance_valid(valve_pop)).T())) {
      valve_pop.C("queue_free");
    }
    Gv pc = Gd.New("PanelContainer");
    valve_pop = pc;
    Gv sb = Gd.New("StyleBoxFlat");
    sb = sb.S("bg_color", Gd.Ctor("Color", Gv.F(0.07), Gv.F(0.09), Gv.F(0.11), Gv.F(0.96)));
    sb = sb.S("border_color", Gd.Ctor("Color", Gv.S("f2c230")));
    sb.C("set_border_width_all", Gv.I(2L));
    sb.C("set_corner_radius_all", Gv.I(8L));
    sb.C("set_content_margin_all", Gv.I(10L));
    pc.C("add_theme_stylebox_override", Gv.S("panel"), sb);
    Gv v = Gd.New("VBoxContainer");
    pc.C("add_child", v);
    Gv lb = Gd.New("Label");
    v.C("add_child", lb);
    Gv sl = Gd.New("HSlider");
    sl = sl.S("min_value", Gv.I(0L));
    sl = sl.S("max_value", Gv.I(100L));
    sl = sl.S("step", Gv.I(1L));
    sl = sl.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(220L), Gv.I(24L)));
    sl = sl.S("value", (Gv.Eq(best.C("get", Gv.S("kind"), Gv.S("")), Gv.S("skrt")) ? Gd.Dyn("int", best.C("get", Gv.S("pct"), Gv.I(50L))) : Gd.Dyn("int", best.C("get", Gv.S("part"), (Gv.Ne(best.C("get", Gv.S("open"), Gv.B(true)), Gv.B(false)) ? Gv.I(100L) : Gv.I(0L))))));
    v.C("add_child", sl);
    Gv lab0 = Gd.Dyn("str", best.C("get", Gv.S("label"), Gv.S("klapka")));
    lb = lb.S("text", (Gv.S("%s – otevřeno %d %%") % Gd.Arr(lab0, Gd.Dyn("int", sl.G("value")))));
    sl.G("value_changed").C("connect", Gd.Fn((_a) => { Gv x = Gd.A(_a, 0); _set_valve_pct(best, Gd.Dyn("int", x)); lb = lb.S("text", (Gv.S("%s – otevřeno %d %%") % Gd.Arr(lab0, Gd.Dyn("int", x)))); return Gv.Null; }));
    Gv cb = Gd.New("Button");
    cb = cb.S("text", Gv.S("hotovo"));
    cb.G("pressed").C("connect", Gd.Fn((_a) => {  pc.C("queue_free"); return Gv.Null; }));
    v.C("add_child", cb);
    hud.C("add_child", pc);
    sl.C("grab_focus");
    pc = pc.S("position", ((spos + Gd.Ctor("Vector2", Gv.I(14L), Gv.I(14L)))).C("min", (Self.C("get_viewport").C("get_visible_rect").G("size") - Gd.Ctor("Vector2", Gv.I(250L), Gv.I(120L)))));
    return Gv.B(true);
    return Gv.Null;
  }
  public Gv malt_gate_pos(Gv it = default(Gv)) {
    return Gd.Ctor("Vector2", (it.G("x") + Gv.F(36.0)), (it.G("y") - Gv.F(16.0)));
    return Gv.Null;
  }
  public Gv _say(Gv t = default(Gv)) {
    if (Gv.Ne(hud, Gv.Null)) {
      hud.C("toast", t);
    }
    return Gv.Null;
  }
  public Gv _shift_hose(Gv w = default(Gv)) {
    Gv zt = ((Gv.Ne(cam, Gv.Null) ? Gd.minf(Gv.F(1.0), cam.G("zoom").G("x")) : Gv.F(1.0))).AsF();
    Gv hh = hose_at(w, (Gv.F(22.0) / zt), Gv.B(true));
    if (Gv.Ne(hh, Gv.Null)) {
      Gv hp = hh.I(Gv.I(0L));
      if ((hp.C("get", Gv.S("user"), Gv.B(false))).T()) {
        Gv oe = outlet_at(Gd.Ctor("Vector2", hp.G("pts").I(Gv.I(0L)).I(Gv.I(0L)), hp.G("pts").I(Gv.I(0L)).I(Gv.I(1L))), Gv.F(2.0));
        items.C("erase", hp);
        if (Gv.Ne(oe, Gv.Null)) {
          outlet_open = outlet_open.SI(oe.G("key"), Gv.B(Gv.Ne(hose_at(oe.G("front")), Gv.Null)));
        }
        sim.C("build_net", items, plan.C("get", Gv.S("joints"), Gd.Arr()));
        if (Gv.Ne(flow_layer, Gv.Null)) {
          flow_layer.C("build");
        }
        redraw();
        _say(Gv.S("Přidaná hadice odebrána."));
        return Gv.B(true);
      }
      return Gv.B(false);
    }
    Gv o = outlet_at(w, (Gv.F(24.0) / zt));
    if (Gv.Eq(o, Gv.Null)) {
      return Gv.B(false);
    }
    if (Gv.Ne(hose_at(o.G("front")), Gv.Null)) {
      _say(Gv.S("Na tomto vývodu už hadice je."));
      return Gv.B(true);
    }
    Gv n = Gv.I(1L);
    foreach (Gv it in (items).Iter()) {
      if ((it.G("id").C("begins_with", Gv.S("hose_user_"))).T()) {
        n = Gd.maxi(n, (Gd.Dyn("int", it.G("id").C("substr", Gv.I(10L))) + Gv.I(1L)));
      }
    }
    Gv h = Gd.Dict(Gv.S("id"), (Gv.S("hose_user_%d") % n), Gv.S("type"), Gv.S("pipe"), Gv.S("hose"), Gv.B(true), Gv.S("dn"), Gv.I(40L), Gv.S("medium"), Gv.S("water"), Gv.S("user"), Gv.B(true), Gv.S("pts"), Gd.Arr());
    hose_shape(h, o.G("front"), (o.G("front") + Gd.Ctor("Vector2", Gv.F(24.0), Gv.F(150.0))));
    items.C("append", h);
    outlet_open = outlet_open.SI(o.G("key"), Gv.B(true));
    sim.C("build_net", items, plan.C("get", Gv.S("joints"), Gd.Arr()));
    if (Gv.Ne(flow_layer, Gv.Null)) {
      flow_layer.C("build");
    }
    redraw();
    _say(Gv.S("Přidána hadice na vývod katety (vývod otevřen). Volný konec přetáhni myší na jiný vývod; Shift + klepnutí na konec ji odebere."));
    return Gv.B(true);
    return Gv.Null;
  }
  public Gv flow_reach(Gv vid = default(Gv)) {
    if ((_vnode.C("is_empty")).T()) {
      foreach (Gv k in (sim.G("net").G("nodes")).Iter()) {
        Gv n = sim.G("net").G("nodes").I(k);
        if (Gv.Ne(n.G("valve"), Gv.Null)) {
          _vnode = _vnode.SI(n.G("valve"), k);
        }
      }
    }
    if ((!(_vnode.C("has", vid)).T() || !(sim.C("valve_open", vid)).T())) {
      return Gd.Arr();
    }
    Gv r = sim.C("bfs", Gd.Arr(_vnode.I(vid)));
    Gv out_ = Gd.Arr();
    Gv seen = Gd.Dict();
    foreach (Gv d in (r.G("hits")).Iter()) {
      foreach (Gv st in (sim.C("path_to", r, d.G("k"))).Iter()) {
        if (((st.C("has", Gv.S("e"))).T() && !(seen.C("has", st.G("e").G("id"))).T())) {
          seen = seen.SI(st.G("e").G("id"), Gv.B(true));
          Gv pl = Gd.Ctor("PackedVector2Array");
          foreach (Gv p in (st.G("e").G("pts")).Iter()) {
            pl.C("append", (Gv.Is(p, "Vector2") ? Gd.Ctor("Vector2", p.G("x"), p.G("y")) : Gd.Ctor("Vector2", p.I(Gv.I(0L)), p.I(Gv.I(1L)))));
          }
          out_.C("append", pl);
        }
      }
    }
    return out_;
    return Gv.Null;
  }
  public Gv flow_targets(Gv vid = default(Gv)) {
    if ((_vnode.C("is_empty")).T()) {
      flow_reach(Gv.S(""));
    }
    if ((!(_vnode.C("has", vid)).T() || !(sim.C("valve_open", vid)).T())) {
      return Gv.S("");
    }
    Gv r = sim.C("bfs", Gd.Arr(_vnode.I(vid)));
    Gv names = Gd.Dict();
    foreach (Gv d in (r.G("hits")).Iter()) {
      names = names.SI(VNAME.C("get", d.G("att").G("bind"), d.G("att").G("bind")), Gv.B(true));
    }
    if ((r.G("leaks").C("size")).T()) {
      names = names.SI(Gv.S("⚠ otevřený konec"), Gv.B(true));
    }
    return Gv.S(", ").C("join", Gd.Ctor("PackedStringArray", names.C("keys")));
    return Gv.Null;
  }
  public Gv undo() {
    if ((undo_stack.C("is_empty")).T()) {
      hud.C("toast", Gv.S("Není co vrátit."));
      return Gv.Null;
    }
    Gv u = undo_stack.C("pop_back");
    {
      Gv _t1 = u.G("t");
      if (Gv.Eq(_t1, Gv.S("valve"))) {
        foreach (Gv it__it in (items).Iter()) {
          Gv it = it__it;
          if (Gv.Eq(it.G("id"), u.G("id"))) {
            it = it.SI(Gv.S("open"), u.G("open"));
            if ((u.C("has", Gv.S("part"))).T()) {
              it = it.SI(Gv.S("part"), u.G("part"));
            } else {
              it.C("erase", Gv.S("part"));
            }
          }
        }
        sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
        redraw_valves();
        hud.C("toast", (Gv.S("Vráceno: %s je zase %s") % Gd.Arr(u.C("get", Gv.S("label"), Gv.S("klapka")), ((u.G("open")).T() ? Gv.S("otevřená") : Gv.S("zavřená")))));
      }
      else if (Gv.Eq(_t1, Gv.S("outlet"))) {
        outlet_open = outlet_open.SI(u.G("id"), u.G("open"));
        sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
        redraw();
        hud.C("toast", Gv.S("Vráceno: vývod katety"));
      }
      else if (Gv.Eq(_t1, Gv.S("pump"))) {
        toggle_pump(u.G("b"));
        undo_stack.C("pop_back");
      }
    }
    if (Gv.Ne(game, Gv.Null)) {
      game.C("play", Gv.S("click"));
    }
    return Gv.Null;
  }
  public Gv pick(Gv w = default(Gv)) {
    Gv S = sim.G("S");
    Gv bd = Gd.maxf(Gv.F(18.0), Gd.minf(Gv.F(40.0), (Gv.F(26.0) / cam.G("zoom").G("x"))));
    Gv best = Gv.Null;
    foreach (Gv it in (items).Iter()) {
      if (Gv.Eq(it.G("type"), Gv.S("valve"))) {
        Gv d = w.C("distance_to", Gd.Ctor("Vector2", it.G("x"), it.G("y")));
        if (Gv.Lt(d, bd)) {
          bd = d;
          best = it;
        }
      }
    }
    if (Gv.Ne(best, Gv.Null)) {
      Gv open = sim.C("valve_open", best.G("id"));
      Gv nm = best.C("get", Gv.S("label"), Gv.S("klapka"));
      Gv t = Gv.S("");
      if (Gv.Eq(best.C("get", Gv.S("kind"), Gv.S("")), Gv.S("skrt"))) {
        t = (Gv.S("%s · otevřeno %d %%\nlevé tlačítko +10 %%, pravé −10 %%") % Gd.Arr(nm, Gd.Dyn("int", best.C("get", Gv.S("pct"), Gv.I(50L)))));
      } else if (Gv.Eq(best.C("get", Gv.S("kind"), Gv.S("")), Gv.S("zasl"))) {
        t = (Gv.S("%s · %s") % Gd.Arr(nm, ((open).T() ? Gv.S("otevřená") : Gv.S("zavřená"))));
      } else {
        if ((can_throttle(best)).T()) {
          t = (Gv.S("%s · %s%s\nklik = %s · tažení myší = přiškrcení · pravé tl. = posuvník") % Gd.Arr(nm, ((open).T() ? Gv.S("OTEVŘENÁ") : Gv.S("zavřená")), ((best.C("has", Gv.S("part"))).T() ? ((Gv.S(" %d %%") % Gd.Dyn("int", best.G("part")))) : Gv.S("")), ((open).T() ? Gv.S("zavřít") : Gv.S("otevřít"))));
        } else {
          t = (Gv.S("%s · %s\nklik = %s") % Gd.Arr(nm, ((open).T() ? Gv.S("OTEVŘENÁ") : Gv.S("zavřená")), ((open).T() ? Gv.S("zavřít") : Gv.S("otevřít"))));
        }
        if ((open).T()) {
          Gv ft = flow_targets(best.G("id"));
          if (Gv.Ne(ft, Gv.S(""))) {
            t = (t + (Gv.S("\nvede do: %s  (podrž Alt = ukázat trasu)") % ft));
          }
        }
      }
      if ((best.C("get", Gv.S("stuck"), Gv.B(false))).T()) {
        t = (t + Gv.S("\n⚠ zadřená"));
      }
      return Gd.Dict(Gv.S("key"), best.G("id"), Gv.S("pos"), Gd.Ctor("Vector2", best.G("x"), best.G("y")), Gv.S("r"), Gv.F(20.0), Gv.S("text"), t, Gv.S("act"), Gv.B(true), Gv.S("col"), ((open).T() ? Gd.Ctor("Color", Gv.S("4fd18b")) : Gd.Ctor("Color", Gv.S("ff5a4a"))));
    }
    Gv o = outlet_at(w, Gv.F(20.0));
    if (Gv.Ne(o, Gv.Null)) {
      return Gd.Dict(Gv.S("key"), o.G("key"), Gv.S("pos"), o.G("front"), Gv.S("r"), Gv.F(16.0), Gv.S("text"), (Gv.S("Vývod katety · %s\nklik = %s · hadici přetáhni myší") % Gd.Arr(((outlet_open.I(o.G("key"))).T() ? Gv.S("otevřen") : Gv.S("zavřen")), ((outlet_open.I(o.G("key"))).T() ? Gv.S("zavřít") : Gv.S("otevřít")))), Gv.S("act"), Gv.B(true));
    }
    foreach (Gv it_2 in (items).Iter()) {
      if ((Gv.Eq(it_2.G("type"), Gv.S("pump")) && Gv.Lt(w.C("distance_to", Gd.Ctor("Vector2", it_2.G("x"), it_2.G("y"))), Gv.I(30L)))) {
        Gv b = it_2.C("get", Gv.S("bind"), Gv.S(""));
        Gv on = (Gv.Ne(b, Gv.S("")) ? ((Gv.In(b, Gd.Arr(Gv.S("p1"), Gv.S("p2"), Gv.S("p3"))) ? S.I(b).G("on") : ((Gv.Eq(b, Gv.S("plv")) ? S.G("lv").G("pump") : (Gv.B(((S.G("rvpWater").G("on")).T() || (S.G("skWater").G("on")).T()))))))) : Gv.B(false));
        Gv t2 = (Gv.S("%s · %s") % Gd.Arr(pump_name(b), ((on).T() ? Gv.S("BĚŽÍ") : Gv.S("stojí"))));
        if (Gv.In(b, Gd.Arr(Gv.S("p1"), Gv.S("p2"), Gv.S("p3")))) {
          t2 = (t2 + (Gv.S(" · %d L/min") % Gd.roundi(((on).T() ? S.I(b).G("q") : S.I(b).G("flow")))));
          if (((on).T() && Gv.Ne(S.I(b).G("status"), Gv.S("")))) {
            t2 = (t2 + (Gv.S("\n") + S.I(b).G("status")));
          }
        }
        t2 = (t2 + (Gv.S("\nklik = ") + (((on).T() ? Gv.S("zastavit") : Gv.S("spustit")))));
        return Gd.Dict(Gv.S("key"), (Gv.S("pump") + b), Gv.S("pos"), Gd.Ctor("Vector2", it_2.G("x"), it_2.G("y")), Gv.S("r"), Gv.F(32.0), Gv.S("text"), t2, Gv.S("act"), Gv.B(true), Gv.S("col"), ((on).T() ? Gd.Ctor("Color", Gv.S("4fd18b")) : Gd.Ctor("Color", Gv.S("5aa9ff"))));
      }
    }
    foreach (Gv it_3 in (items).Iter()) {
      if (((Gv.Eq(it_3.G("type"), Gv.S("vessel")) && Gv.Eq(it_3.C("get", Gv.S("shape")), Gv.S("inlet"))) && Gv.Lt(w.C("distance_to", Gd.Ctor("Vector2", (it_3.G("x") + (it_3.G("w") * Gv.F(0.5))), (it_3.G("y") + (it_3.G("h") * Gv.F(0.5))))), ((Gd.maxf(it_3.G("w"), it_3.G("h")) * Gv.F(0.5)) + Gv.F(16.0))))) {
        return Gd.Dict(Gv.S("key"), Gv.S("inlet"), Gv.S("pos"), Gd.Ctor("Vector2", (it_3.G("x") + (it_3.G("w") * Gv.F(0.5))), (it_3.G("y") + (it_3.G("h") * Gv.F(0.5)))), Gv.S("r"), ((Gd.maxf(it_3.G("w"), it_3.G("h")) * Gv.F(0.5)) + Gv.F(6.0)), Gv.S("text"), (Gv.S("Spoušť vody z řadu · %s\nklik = %s") % Gd.Arr(((it_3.C("get", Gv.S("on"), Gv.B(false))).T() ? Gv.S("PUŠTĚNA") : Gv.S("zavřená")), ((it_3.C("get", Gv.S("on"), Gv.B(false))).T() ? Gv.S("zavřít") : Gv.S("pustit")))), Gv.S("act"), Gv.B(true));
      }
      if ((Gv.Eq(it_3.G("type"), Gv.S("sw")) && (Gd.Ctor("Rect2", (it_3.G("x") - Gv.I(26L)), (it_3.G("y") - Gv.I(34L)), Gv.I(52L), Gv.I(68L)).C("has_point", w)).T())) {
        return Gd.Dict(Gv.S("key"), (Gv.S("sw") + Gd.Dyn("str", it_3.C("get", Gv.S("id"), Gv.S("")))), Gv.S("rect"), Gd.Ctor("Rect2", (it_3.G("x") - Gv.I(26L)), (it_3.G("y") - Gv.I(34L)), Gv.I(52L), Gv.I(68L)), Gv.S("text"), (Gv.S("%s · %s\nklik = přepnout") % Gd.Arr(it_3.C("get", Gv.S("label"), Gv.S("spínač")), ((it_3.C("get", Gv.S("on"), Gv.B(false))).T() ? Gv.S("ZAP") : Gv.S("vyp")))), Gv.S("act"), Gv.B(true));
      }
    }
    foreach (Gv ith in (items).Iter()) {
      if (((Gv.Eq(ith.G("type"), Gv.S("deco")) && Gv.Eq(ith.C("get", Gv.S("k"), Gv.S("")), Gv.S("splid"))) && (Gd.Ctor("Rect2", (ith.G("x") - Gv.F(44.0)), (ith.G("y") - Gv.F(46.0)), Gv.F(88.0), Gv.F(60.0)).C("has_point", w)).T())) {
        return Gd.Dict(Gv.S("key"), Gv.S("splid"), Gv.S("rect"), Gd.Ctor("Rect2", (ith.G("x") - Gv.F(44.0)), (ith.G("y") - Gv.F(46.0)), Gv.F(88.0), Gv.F(60.0)), Gv.S("text"), (Gv.S("Víčko spilky · %s\nmusí být OTEVŘENÉ, jinak se tank tlakuje a mladina netěče\nklik = přepnout") % (((S.G("sp").C("get", Gv.S("lidOpen"), Gv.B(false))).T() ? Gv.S("OTEVŘENÉ") : Gv.S("zavřené")))), Gv.S("act"), Gv.B(true), Gv.S("col"), Gd.Ctor("Color", Gv.S("f2c230")));
      }
      if (((Gv.Eq(ith.G("type"), Gv.S("deco")) && Gv.Eq(ith.C("get", Gv.S("k"), Gv.S("")), Gv.S("hatch"))) && (Gd.Ctor("Rect2", (ith.G("x") - Gv.F(46.0)), (ith.G("y") - Gv.F(8.0)), Gv.F(92.0), Gv.F(52.0)).C("has_point", w)).T())) {
        return Gd.Dict(Gv.S("key"), Gv.S("hatch"), Gv.S("rect"), Gd.Ctor("Rect2", (ith.G("x") - Gv.F(46.0)), (ith.G("y") - Gv.F(8.0)), Gv.F(92.0), Gv.F(52.0)), Gv.S("text"), (Gv.S("Výhoz mláta · %s\nklik = přepnout") % (((S.G("sk").C("get", Gv.S("hatch"), Gv.B(false))).T() ? Gv.S("OTEVŘEN") : Gv.S("zavřen")))), Gv.S("act"), Gv.B(true), Gv.S("col"), Gd.Ctor("Color", Gv.S("f2c230")));
      }
      if (((Gv.Eq(ith.G("type"), Gv.S("deco")) && Gv.Eq(ith.C("get", Gv.S("k"), Gv.S("")), Gv.S("conv"))) && Gv.Lt(w.C("distance_to", malt_gate_pos(ith)), Gv.F(30.0)))) {
        return Gd.Dict(Gv.S("key"), Gv.S("gate"), Gv.S("pos"), malt_gate_pos(ith), Gv.S("r"), Gv.F(24.0), Gv.S("text"), (Gv.S("Šoupě násypky · %s\nklik = přepnout") % (((S.G("malt").C("get", Gv.S("gate"), Gv.B(false))).T() ? Gv.S("otevřeno") : Gv.S("zavřeno")))), Gv.S("act"), Gv.B(true), Gv.S("col"), Gd.Ctor("Color", Gv.S("f2c230")));
      }
    }
    if ((Gv.Ne(cabinet, Gv.Null) && Gv.Ne(cabinet.C("hit", w), Gv.S("")))) {
      return Gd.Dict(Gv.S("key"), Gv.S("cab"), Gv.S("text"), Gv.S(""), Gv.S("act"), Gv.B(true));
    }
    foreach (Gv it_4 in (items).Iter()) {
      if (((Gv.Eq(it_4.G("type"), Gv.S("vessel")) && Gv.In(it_4.C("get", Gv.S("bind")), Gd.Arr(Gv.S("rvp"), Gv.S("sk"), Gv.S("wp"), Gv.S("hlt"), Gv.S("lv"), Gv.S("spilka")))) && (Gd.Ctor("Rect2", it_4.G("x"), it_4.G("y"), it_4.G("w"), it_4.G("h")).C("has_point", w)).T())) {
        return Gd.Dict(Gv.S("key"), (Gv.S("v") + it_4.G("bind")), Gv.S("rect"), Gd.Ctor("Rect2", it_4.G("x"), it_4.G("y"), it_4.G("w"), it_4.G("h")), Gv.S("text"), (Gv.S("%s\n%s") % Gd.Arr(it_4.C("get", Gv.S("label"), Gv.S("")), vessel_text(it_4.G("bind")))), Gv.S("act"), Gv.B(false), Gv.S("col"), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.5)));
      }
    }
    return Gd.Dict();
    return Gv.Null;
  }
  public Gv _tap(Gv w = default(Gv)) {
    if (Gv.Ne(game, Gv.Null)) {
      game.G("hover").C("ripple", w);
      Gv pk = pick(w);
      if ((pk.C("get", Gv.S("act"), Gv.B(false))).T()) {
        game.C("play", (((pk.G("key").C("begins_with", Gv.S("valve"))).T() || (pk.G("key").C("begins_with", Gv.S("manifold"))).T()) ? Gv.S("valve") : Gv.S("click")));
      }
    }
    if (((Gd.Static("Input", "is_key_pressed", Gd.K("KEY_SHIFT"))).T() && (_shift_hose(w)).T())) {
      return Gv.Null;
    }
    if ((false && Gv.Ne(cabinet, Gv.Null))) {
      Gv ck = cabinet.C("hit", w);
      if (Gv.Ne(ck, Gv.S(""))) {
        if ((Gv.Ne(ck, Gv.S("hup")) && Gv.Ne(ck, Gv.S("hdn")))) {
          cabinet.C("act", ck, Gv.B(true));
        }
        return Gv.Null;
      }
    }
    foreach (Gv ith in (items).Iter()) {
      if (((Gv.Eq(ith.G("type"), Gv.S("deco")) && Gv.Eq(ith.C("get", Gv.S("k"), Gv.S("")), Gv.S("splid"))) && (Gd.Ctor("Rect2", (ith.G("x") - Gv.F(44.0)), (ith.G("y") - Gv.F(46.0)), Gv.F(88.0), Gv.F(60.0)).C("has_point", w)).T())) {
        sim = sim.S("S", sim.G("S").S("sp", sim.G("S").G("sp").SI(Gv.S("lidOpen"), Gv.B(!(sim.G("S").G("sp").C("get", Gv.S("lidOpen"), Gv.B(false))).T()))));
        _say((Gv.S("Víčko spilky: %s") % (((sim.G("S").G("sp").G("lidOpen")).T() ? Gv.S("OTEVŘENÉ – tank se netlakuje, mladina může téct dovnitř") : Gv.S("zavřené – při plnění se tank bude tlakovat!")))));
        redraw();
        return Gv.Null;
      }
      if (((Gv.Eq(ith.G("type"), Gv.S("deco")) && Gv.Eq(ith.C("get", Gv.S("k"), Gv.S("")), Gv.S("hatch"))) && (Gd.Ctor("Rect2", (ith.G("x") - Gv.F(46.0)), (ith.G("y") - Gv.F(8.0)), Gv.F(92.0), Gv.F(52.0)).C("has_point", w)).T())) {
        sim = sim.S("S", sim.G("S").S("sk", sim.G("S").G("sk").SI(Gv.S("hatch"), Gv.B(!(sim.G("S").G("sk").C("get", Gv.S("hatch"), Gv.B(false))).T()))));
        _say((Gv.S("Výhoz mláta: %s") % (((sim.G("S").G("sk").G("hatch")).T() ? Gv.S("OTEVŘEN – kapalina odtéká do kanálu, mláto vypadává (s kopačkou rychleji)") : Gv.S("zavřen")))));
        redraw();
        return Gv.Null;
      }
    }
    foreach (Gv itg in (items).Iter()) {
      if (((Gv.Eq(itg.G("type"), Gv.S("deco")) && Gv.Eq(itg.C("get", Gv.S("k"), Gv.S("")), Gv.S("conv"))) && Gv.Lt(w.C("distance_to", malt_gate_pos(itg)), Gv.F(30.0)))) {
        sim = sim.S("S", sim.G("S").S("malt", sim.G("S").G("malt").S("gate", Gv.B(!(sim.G("S").G("malt").C("get", Gv.S("gate"), Gv.B(false))).T()))));
        hud.C("toast", (Gv.S("Šoupě násypky: %s") % (((sim.G("S").G("malt").G("gate")).T() ? Gv.S("otevřeno – slad padá, když je zapnuté šrotování") : Gv.S("zavřeno")))));
        redraw();
        return Gv.Null;
      }
    }
    if ((hard).T()) {
      foreach (Gv dm in (items).Iter()) {
        if (Gv.Ne(dm.G("type"), Gv.S("dip"))) {
          continue;
        }
        Gv DM = dip_geo(dm);
        if ((Gd.Ctor("Rect2", (DM.G("x") + Gv.F(24.0)), ((DM.G("y") + (DM.G("h") * Gv.F(0.5))) - Gv.F(13.0)), Gv.F(54.0), Gv.F(26.0)).C("has_point", w)).T()) {
          measure_dip(dm);
          return Gv.Null;
        }
      }
      foreach (Gv gm in (items).Iter()) {
        if (Gv.Ne(gm.G("type"), Gv.S("gauge"))) {
          continue;
        }
        Gv GM = gauge_geo(gm);
        if ((Gd.Ctor("Rect2", (GM.G("x") + Gv.F(14.0)), ((((GM.G("y1") + GM.G("y2"))) * Gv.F(0.5)) - Gv.F(13.0)), Gv.F(54.0), Gv.F(26.0)).C("has_point", w)).T()) {
          measure_gauge(gm);
          return Gv.Null;
        }
      }
    }
    foreach (Gv gk__it in (items).Iter()) {
      Gv gk = gk__it;
      if (Gv.Ne(gk.G("type"), Gv.S("gauge"))) {
        continue;
      }
      Gv GG = gauge_geo(gk);
      Gv spots = Gd.Dict();
      if ((GG.G("free")).T()) {
        spots = Gd.Dict(Gv.S("top"), Gd.Ctor("Vector2", GG.G("x"), (GG.G("y1") - Gv.F(18.0))), Gv.S("bot"), Gd.Ctor("Vector2", GG.G("x"), (GG.G("y2") + Gv.F(18.0))));
      } else {
        Gv mx = ((((GG.G("cx") + GG.G("x"))) / Gv.F(2.0))).AsF();
        spots = Gd.Dict(Gv.S("top"), Gd.Ctor("Vector2", mx, GG.G("y1")), Gv.S("bot"), Gd.Ctor("Vector2", mx, GG.G("y2")));
      }
      foreach (Gv key in (spots).Iter()) {
        if (Gv.Lt(w.C("distance_to", spots.I(key)), Gv.F(16.0))) {
          gk = gk.SI(key, Gv.B(!Gv.Ne(gk.C("get", key, Gv.B(true)), Gv.B(false))));
          sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
          hud.C("toast", (Gv.S("Stavoznak: %s kohout %s") % Gd.Arr((Gv.Eq(key, Gv.S("top")) ? Gv.S("horní") : Gv.S("dolní")), (Gv.Ne(gk.C("get", key, Gv.B(true)), Gv.B(false)) ? Gv.S("otevřen") : Gv.S("zavřen")))));
          redraw();
          update_live();
          return Gv.Null;
        }
      }
    }
    foreach (Gv it0 in (items).Iter()) {
      if (((Gv.Eq(it0.G("type"), Gv.S("inl")) && Gv.Eq(it0.C("get", Gv.S("k"), Gv.S("")), Gv.S("pruh"))) && Gv.Lt(w.C("distance_to", Gd.Ctor("Vector2", it0.G("x"), it0.G("y"))), Gv.F(26.0)))) {
        if ((Gv.Ne(sight_win, Gv.Null) && (Gd.is_instance_valid(sight_win)).T())) {
          sight_win.C("queue_free");
        }
        sight_win = S_sight.New();
        sight_win = sight_win.S("m", Self);
        hud.C("add_child", sight_win);
        sight_win.C("open", it0);
        return Gv.Null;
      }
    }
    Gv o = outlet_at(w, Gv.F(20.0));
    if (Gv.Ne(o, Gv.Null)) {
      undo_stack.C("append", Gd.Dict(Gv.S("t"), Gv.S("outlet"), Gv.S("id"), o.G("key"), Gv.S("open"), outlet_open.I(o.G("key"))));
      outlet_open = outlet_open.SI(o.G("key"), Gv.B(!(outlet_open.I(o.G("key"))).T()));
      sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
      redraw();
      hud.C("toast", (Gv.S("Vývod katety: %s%s") % Gd.Arr(((outlet_open.I(o.G("key"))).T() ? Gv.S("otevřen") : Gv.S("zavřen")), (Gv.Ne(hose_at(o.G("front")), Gv.Null) ? Gv.S("") : Gv.S(" (bez hadice – teklo by na zem)")))));
      return Gv.Null;
    }
    Gv best = Gv.Null;
    Gv bd = (Gv.Ne(cam, Gv.Null) ? Gd.maxf(Gv.F(18.0), Gd.minf(Gv.F(40.0), (Gv.F(26.0) / cam.G("zoom").G("x")))) : Gv.F(18.0));
    foreach (Gv it in (items).Iter()) {
      if (Gv.Eq(it.G("type"), Gv.S("valve"))) {
        Gv d = w.C("distance_to", Gd.Ctor("Vector2", it.G("x"), it.G("y")));
        if (Gv.Lt(d, bd)) {
          bd = d;
          best = it;
        }
      }
    }
    if ((Gv.Ne(best, Gv.Null) && Gv.Ne(best.C("get", Gv.S("kind"), Gv.S("")), Gv.S("zasl")))) {
      if ((best.C("get", Gv.S("stuck"), Gv.B(false))).T()) {
        hud.C("toast", (Gv.S("%s nejde přestavit – páka drhne, klapka je zadřená (Nácvik poruch → Uvolnit klapku).") % best.C("get", Gv.S("label"), Gv.S("Klapka"))));
        sim.C("logm", (Gv.S("%s je zadřená – nejde přestavit.") % best.C("get", Gv.S("label"), Gv.S("Klapka"))), Gv.S("warn"));
        return Gv.Null;
      }
      if (Gv.Eq(best.C("get", Gv.S("kind"), Gv.S("")), Gv.S("skrt"))) {
        _throttle_step(Gd.Ctor("Vector2", best.G("x"), best.G("y")), Gv.I(10L));
        return Gv.Null;
      } else {
        Gv ue = Gd.Dict(Gv.S("t"), Gv.S("valve"), Gv.S("id"), best.G("id"), Gv.S("open"), Gv.B(Gv.Ne(best.C("get", Gv.S("open"), Gv.B(true)), Gv.B(false))), Gv.S("label"), best.C("get", Gv.S("label"), Gv.S("klapka")));
        if ((best.C("has", Gv.S("part"))).T()) {
          ue = ue.SI(Gv.S("part"), best.G("part"));
        }
        undo_stack.C("append", ue);
        if (Gv.Gt(undo_stack.C("size"), Gv.I(50L))) {
          undo_stack.C("pop_front");
        }
        best = best.SI(Gv.S("open"), Gv.B(!Gv.Ne(best.C("get", Gv.S("open"), Gv.B(true)), Gv.B(false))));
        best.C("erase", Gv.S("part"));
        if (((sim.G("net").G("byId").C("has", best.G("id"))).T() && Gv.Ne(sim.G("net").G("byId").I(best.G("id")), best))) {
          sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(best.G("id"), sim.G("net").G("byId").I(best.G("id")).SI(Gv.S("open"), best.G("open")))));
          sim.G("net").G("byId").I(best.G("id")).C("erase", Gv.S("part"));
        }
      }
      sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
      redraw_valves();
      hud.C("toast", (Gv.S("%s: %s") % Gd.Arr(best.C("get", Gv.S("label"), Gv.S("klapka")), (Gv.Ne(best.C("get", Gv.S("open"), Gv.B(true)), Gv.B(false)) ? Gv.S("otevřena") : Gv.S("zavřena")))));
      return Gv.Null;
    }
    foreach (Gv it_2 in (items).Iter()) {
      if ((Gv.Eq(it_2.G("type"), Gv.S("pump")) && Gv.Lt(w.C("distance_to", Gd.Ctor("Vector2", it_2.G("x"), it_2.G("y"))), Gv.I(30L)))) {
        toggle_pump(it_2.C("get", Gv.S("bind"), Gv.S("")));
        return Gv.Null;
      }
    }
    foreach (Gv it_3__it in (items).Iter()) {
      Gv it_3 = it_3__it;
      if (((Gv.Eq(it_3.G("type"), Gv.S("vessel")) && Gv.Eq(it_3.C("get", Gv.S("shape")), Gv.S("inlet"))) && Gv.Lt(w.C("distance_to", Gd.Ctor("Vector2", (it_3.G("x") + (it_3.G("w") * Gv.F(0.5))), (it_3.G("y") + (it_3.G("h") * Gv.F(0.5))))), ((Gd.maxf(it_3.G("w"), it_3.G("h")) * Gv.F(0.5)) + Gv.F(16.0))))) {
        it_3 = it_3.SI(Gv.S("on"), Gv.B(!(it_3.C("get", Gv.S("on"), Gv.B(false))).T()));
        hud.C("toast", (Gv.S("Voda z řadu: %s") % (((it_3.G("on")).T() ? Gv.S("PUŠTĚNA") : Gv.S("zavřena")))));
        redraw();
        return Gv.Null;
      }
      if ((Gv.Eq(it_3.G("type"), Gv.S("sw")) && (Gd.Ctor("Rect2", (it_3.G("x") - Gv.I(26L)), (it_3.G("y") - Gv.I(34L)), Gv.I(52L), Gv.I(68L)).C("has_point", w)).T())) {
        it_3 = it_3.SI(Gv.S("on"), Gv.B(!(it_3.C("get", Gv.S("on"), Gv.B(false))).T()));
        hud.C("toast", (Gv.S("%s: %s") % Gd.Arr(it_3.C("get", Gv.S("label"), Gv.S("spínač")), ((it_3.G("on")).T() ? Gv.S("ZAPNUT") : Gv.S("vypnut")))));
        redraw();
        return Gv.Null;
      }
    }
    return Gv.Null;
  }
  public Gv cab_why() {
    Gv c = sim.G("S").G("cab");
    if ((c.G("estop")).T()) {
      return Gv.S("je stisknuté NOUZOVÉ STOP (uvolni ho)");
    }
    if (!(c.G("main")).T()) {
      return Gv.S("hlavní vypínač je vypnutý");
    }
    if (!(c.G("key")).T()) {
      return Gv.S("klíč na rozvaděči je na VYP (otoč na START)");
    }
    return Gv.S("");
    return Gv.Null;
  }
  public Gv toggle_pump(Gv b = default(Gv)) {
    Gv S = sim.G("S");
    Gv on_now = (Gv.In(b, Gd.Arr(Gv.S("p1"), Gv.S("p2"), Gv.S("p3"))) ? S.I(b).G("on") : ((Gv.Eq(b, Gv.S("plv")) ? S.G("lv").G("pump") : (Gv.B(((S.G("rvpWater").G("on")).T() || (S.G("skWater").G("on")).T()))))));
    if ((!(on_now).T() && !(sim.C("cab_live")).T())) {
      _say((Gv.S("%s nejde pustit – rozvaděč je bez napětí: %s. Rozvaděč najdeš vlevo (tlačítko Rozvaděč nahoře) nebo v panelu ROZVADĚČ.") % Gd.Arr(pump_name(b), cab_why())));
      sim.C("logm", (Gv.S("%s nejde pustit – rozvaděč je bez napětí (%s).") % Gd.Arr(pump_name(b), cab_why())), Gv.S("warn"));
      redraw();
      return Gv.Null;
    }
    {
      Gv _t1 = b;
      if (Gv.Eq(_t1, Gv.S("p1")) || Gv.Eq(_t1, Gv.S("p2")) || Gv.Eq(_t1, Gv.S("p3"))) {
        if ((!(S.I(b).G("on")).T() && (S.G("cab").C("get", Gv.S("interlock"), Gv.B(true))).T())) {
          Gv why = sim.C("pump_interlock", b);
          if (Gv.Ne(why, Gv.S(""))) {
            _say((Gv.S("OCHRANA %s: %s") % Gd.Arr(pump_name(b), why)));
            sim.C("logm", (Gv.S("Ochrana nepustila %s: %s") % Gd.Arr(pump_name(b), why)), Gv.S("warn"));
            redraw();
            return Gv.Null;
          }
        }
        S = S.SI(b, S.I(b).S("on", Gv.B(!(S.I(b).G("on")).T())));
        _say((Gv.S("%s: %s") % Gd.Arr(pump_name(b), ((S.I(b).G("on")).T() ? Gv.S("START") : Gv.S("stop")))));
        if (Gv.Ne(practice, Gv.Null)) {
          if ((S.I(b).G("on")).T()) {
            practice.C("on_pump_start", b);
          } else {
            practice.C("on_pump_stop", b);
          }
        }
        undo_stack.C("append", Gd.Dict(Gv.S("t"), Gv.S("pump"), Gv.S("b"), b));
        if (Gv.Ne(game, Gv.Null)) {
          game.C("play", ((S.I(b).G("on")).T() ? Gv.S("start") : Gv.S("stop")));
        }
        if ((S.I(b).G("on")).T()) {
          Gv wn = sim.C("pump_warn", b);
          if (Gv.Ne(wn, Gv.S(""))) {
            sim.C("logm", (Gv.S("%s: %s") % Gd.Arr(pump_name(b), wn)), Gv.S("warn"));
          }
        }
      }
      else if (Gv.Eq(_t1, Gv.S("plv"))) {
        S = S.S("lv", S.G("lv").S("pump", Gv.B(!(S.G("lv").G("pump")).T())));
        _say((Gv.S("%s (ledová voda): %s") % Gd.Arr(pump_name(b), ((S.G("lv").G("pump")).T() ? Gv.S("START") : Gv.S("stop")))));
        if ((Gv.Ne(practice, Gv.Null) && !(S.G("lv").G("pump")).T())) {
          practice.C("on_pump_stop", Gv.S("plv"));
        }
      }
      else if (Gv.Eq(_t1, Gv.S("phw"))) {
        if (((S.G("rvpWater").G("on")).T() || (S.G("skWater").G("on")).T())) {
          S = S.S("rvpWater", S.G("rvpWater").S("on", Gv.B(false)));
          S = S.S("skWater", S.G("skWater").S("on", Gv.B(false)));
          _say(Gv.S("Čerpadlo horké vody: stop"));
        } else {
          S = S.S("rvpWater", S.G("rvpWater").S("on", Gv.B(true)));
          _say((Gv.S("Čerpadlo horké vody: napouští pánev (%d L) – objem a vyslazování nastavíš vpravo") % Gd.roundi((S.G("rvpWater").G("target") - S.G("rvpWater").G("added")))));
        }
      }
      else if (true) {
        _say(Gv.S("Čerpadlo nemá v simulaci roli."));
      }
    }
    if (!(running).T()) {
      _say(Gv.S("Čas várky stojí – nic neteče. Stiskni SPUSTIT (vpravo nahoře v panelu)."));
    }
    redraw();
    return Gv.Null;
  }
  public Gv pump_name(Gv b = default(Gv)) {
    foreach (Gv it in (items).Iter()) {
      if ((Gv.Eq(it.G("type"), Gv.S("pump")) && Gv.Eq(it.C("get", Gv.S("bind")), b))) {
        return it.C("get", Gv.S("label"), b.C("to_upper"));
      }
    }
    return b.C("to_upper");
    return Gv.Null;
  }
  public Gv pump_primed(Gv it = default(Gv)) {
    Gv b = it.C("get", Gv.S("bind"), Gv.S(""));
    return Gv.B(((Gv.In(b, Gd.Arr(Gv.S("p1"), Gv.S("p2"), Gv.S("p3"))) && !(sim.G("S").I(b).G("on")).T()) && Gv.Ge(sim.G("S").I(b).G("prime"), Gv.F(0.95))));
    return Gv.Null;
  }
  public Gv pump_state(Gv it = default(Gv)) {
    Gv S = sim.G("S");
    Gv b = it.C("get", Gv.S("bind"), Gv.S(""));
    Gv on = Gv.B(false);
    {
      Gv _t1 = b;
      if (Gv.Eq(_t1, Gv.S("p1")) || Gv.Eq(_t1, Gv.S("p2")) || Gv.Eq(_t1, Gv.S("p3"))) {
        on = S.I(b).G("on");
      }
      else if (Gv.Eq(_t1, Gv.S("plv"))) {
        on = S.G("lv").G("pump");
      }
      else if (Gv.Eq(_t1, Gv.S("phw"))) {
        on = Gv.B((((S.G("rvpWater").G("on")).T() || (S.G("skWater").G("on")).T()) || (S.C("get", Gv.S("flush"), Gd.Dict()).C("get", Gv.S("on"), Gv.B(false))).T()));
      }
    }
    if (!(on).T()) {
      return (Gv.I(0L)).AsI();
    }
    if ((Gv.In(b, Gd.Arr(Gv.S("p1"), Gv.S("p2"), Gv.S("p3"))) && Gv.Lt(S.I(b).G("prime"), Gv.F(0.95)))) {
      return (Gv.I(3L)).AsI();
    }
    if (Gv.Eq(b, Gv.S("phw"))) {
      return (((((S.G("_rvpIn")).T() || (S.G("_skTop")).T()) || (S.G("_skLow")).T()) ? Gv.I(2L) : Gv.I(1L))).AsI();
    }
    foreach (Gv f in (S.G("flows")).Iter()) {
      if (Gv.Eq(f.G("pump"), it.G("id"))) {
        return (Gv.I(2L)).AsI();
      }
    }
    return (Gv.I(1L)).AsI();
    return Gv.Null;
  }
  public Gv set_hall(Gv dark = default(Gv)) {
    hall_dark = dark;
    Gv f = Gd.Static("FileAccess", "open", Gv.S("user://vzhled.txt"), Gd.StaticGet("FileAccess", "WRITE"));
    if (Gv.Ne(f, Gv.Null)) {
      f.C("store_string", ((dark).T() ? Gv.S("tmava") : Gv.S("svetla")));
    }
    Gd.Static("RenderingServer", "set_default_clear_color", Gd.StaticGet("Color", "WHITE"));
    if (Gv.Ne(detail_spr, Gv.Null)) {
      detail_spr = detail_spr.S("visible", Gv.B(false));
    }
    detail_rect = Gd.Ctor("Rect2");
    detail_sig = Gv.S("");
    hud.C("toast", Gv.S("Překresluji halu…"));
    _ = _bake();
    hud.C("toast", (Gv.S("Hala: ") + (((dark).T() ? Gv.S("tmavá") : Gv.S("světlá")))));
    return Gv.Null;
  }
  public Gv _bake() {
    if ((tiles_root).T()) {
      tiles_root.C("queue_free");
    }
    tiles_root = Gd.New("Node2D");
    Self.C("add_child", tiles_root);
    Self.C("move_child", tiles_root, Gv.I(1L));
    Gv sl = S_main_StaticLayer.New();
    sl = sl.S("m", Self);
    tiles_root.C("add_child", sl);
    Gv il = S_main_ItemLayer.New();
    il = il.S("m", Self);
    il = il.S("skip_valves", Gv.B(true));
    tiles_root.C("add_child", il);
    return Gv.Null;
  }
  public Gv visible_world_rect() {
    Gv a = screen_to_world(Gd.StaticGet("Vector2", "ZERO"));
    Gv b = screen_to_world(vp_size());
    return Gd.Ctor("Rect2", a, Gd.StaticGet("Vector2", "ZERO")).C("expand", b);
    return Gv.Null;
  }
  public Gv _detail_tick(Gv delta = default(Gv)) {
    delta = (delta).AsF();
    return Gv.Null;
    return Gv.Null;
  }
  public Gv _bake_detail(Gv vr = default(Gv), Gv z = default(Gv)) {
    z = (z).AsF();
    return Gv.Null;
    return Gv.Null;
  }
  public Gv mf_ports(Gv mf = default(Gv)) {
    Gv g = Gd.Dyn("float", mf.C("get", Gv.S("gap"), Gv.I(60L)));
    Gv R = Gd.Dyn("int", mf.C("get", Gv.S("rows"), Gv.I(3L)));
    Gv C = Gd.Dyn("int", mf.C("get", Gv.S("cols"), Gv.I(4L)));
    Gv side = mf.C("get", Gv.S("back"), Gv.S("top"));
    Gv W = (C * g);
    Gv H = (R * g);
    Gv x = (mf.G("x")).AsF();
    Gv y = (mf.G("y")).AsF();
    Gv ctr = Gd.Ctor("Vector2", (x + (W * Gv.F(0.5))), (y + (H * Gv.F(0.5))));
    Gv rot = Gd.deg_to_rad(Gd.Dyn("float", mf.C("get", Gv.S("rot"), Gv.I(0L))));
    Gv o = Gd.Arr();
    foreach (Gv r in (R).Iter()) {
      foreach (Gv c in (C).Iter()) {
        Gv nx = ((x + (g * Gv.F(0.5))) + (c * g));
        Gv ny = ((y + (g * Gv.F(0.5))) + (r * g));
        Gv p = Gd.StaticGet("Vector2", "ZERO");
        if (Gv.Eq(side, Gv.S("both"))) {
          Gv t = Gd.Dyn("int", Gd.ceil((R / Gv.F(2.0))));
          Gv up = Gv.B(Gv.Lt(r, t));
          Gv cnt = ((up).T() ? t : (R - t));
          Gv j = ((up).T() ? r : (r - t));
          p = Gd.Ctor("Vector2", (nx + (((j - (((cnt - Gv.I(1L))) / Gv.F(2.0)))) * Gd.minf(Gv.F(12.0), (g / ((cnt + Gv.I(1L))))))), ((up).T() ? (y - Gv.I(16L)) : ((y + H) + Gv.I(16L))));
        } else if (Gv.Eq(side, Gv.S("top"))) {
          p = Gd.Ctor("Vector2", (nx + (((r - (((R - Gv.I(1L))) / Gv.F(2.0)))) * Gd.minf(Gv.F(12.0), (g / ((R + Gv.I(1L))))))), (y - Gv.I(16L)));
        } else if (Gv.Eq(side, Gv.S("bottom"))) {
          p = Gd.Ctor("Vector2", (nx + (((r - (((R - Gv.I(1L))) / Gv.F(2.0)))) * Gd.minf(Gv.F(12.0), (g / ((R + Gv.I(1L))))))), ((y + H) + Gv.I(16L)));
        } else if (Gv.Eq(side, Gv.S("left"))) {
          p = Gd.Ctor("Vector2", (x - Gv.I(16L)), (ny + (((c - (((C - Gv.I(1L))) / Gv.F(2.0)))) * Gd.minf(Gv.F(12.0), (g / ((C + Gv.I(1L))))))));
        } else {
          p = Gd.Ctor("Vector2", ((x + W) + Gv.I(16L)), (ny + (((c - (((C - Gv.I(1L))) / Gv.F(2.0)))) * Gd.minf(Gv.F(12.0), (g / ((C + Gv.I(1L))))))));
        }
        Gv f = (((Gd.Ctor("Vector2", nx, ny) - ctr)).C("rotated", rot) + ctr);
        Gv b = (((p - ctr)).C("rotated", rot) + ctr);
        o.C("append", Gd.Dict(Gv.S("front"), f.C("round"), Gv.S("back"), b.C("round"), Gv.S("key"), (Gv.S("%s_%d") % Gd.Arr(mf.G("id"), o.C("size")))));
      }
    }
    return o;
    return Gv.Null;
  }
  public Gv _init_outlets() {
    outlets.C("clear");
    foreach (Gv q in (items).Iter()) {
      if ((Gv.Eq(q.G("type"), Gv.S("pipe")) && !(q.C("get", Gv.S("hose"), Gv.B(false))).T())) {
        foreach (Gv p in (q.G("pts")).Iter()) {
          pipe_pts = pipe_pts.SI(Gd.Ctor("Vector2i", Gd.Dyn("int", p.I(Gv.I(0L))), Gd.Dyn("int", p.I(Gv.I(1L)))), Gv.B(true));
        }
      }
    }
    foreach (Gv it in (items).Iter()) {
      if (Gv.Eq(it.G("type"), Gv.S("manifold"))) {
        outlets.C("append_array", mf_ports(it));
      }
    }
    foreach (Gv o in (outlets).Iter()) {
      outlet_open = outlet_open.SI(o.G("key"), Gv.B(Gv.Ne(hose_at(o.G("front")), Gv.Null)));
    }
    return Gv.Null;
  }
  public Gv hose_at(Gv p = default(Gv), Gv tol = default(Gv), Gv grab = default(Gv)) {
    if (tol.k == 0) tol = Gv.F(6.0);
    if (grab.k == 0) grab = Gv.B(false);
    foreach (Gv it in (items).Iter()) {
      if (((Gv.Eq(it.G("type"), Gv.S("pipe")) && (it.C("get", Gv.S("hose"), Gv.B(false))).T()) && !((grab).T() && (it.C("get", Gv.S("fixed"), Gv.B(false))).T()))) {
        foreach (Gv idx in (Gd.Arr(Gv.I(0L), (it.G("pts").C("size") - Gv.I(1L)))).Iter()) {
          if (Gv.Le(Gd.Ctor("Vector2", it.G("pts").I(idx).I(Gv.I(0L)), it.G("pts").I(idx).I(Gv.I(1L))).C("distance_to", p), tol)) {
            return Gd.Arr(it, idx);
          }
        }
      }
    }
    return Gv.Null;
    return Gv.Null;
  }
  public Gv outlet_at(Gv p = default(Gv), Gv tol = default(Gv)) {
    tol = (tol).AsF();
    Gv best = Gv.Null;
    Gv bd = tol;
    foreach (Gv o in (outlets).Iter()) {
      Gv d = (o.G("front").C("distance_to", p)).AsF();
      if (Gv.Lt(d, bd)) {
        bd = d;
        best = o;
      }
    }
    return best;
    return Gv.Null;
  }
  public Gv hose_shape(Gv it = default(Gv), Gv a = default(Gv), Gv b = default(Gv)) {
    Gv L = Gd.maxf(Gv.F(1.0), a.C("distance_to", b));
    Gv mid = (((a + b)) * Gv.F(0.5));
    Gv n = (Gd.Ctor("Vector2", (-((b.G("y") - a.G("y")))), (b.G("x") - a.G("x"))) / L);
    if (Gv.Lt(n.G("y"), Gv.I(0L))) {
      n = (-n);
    }
    Gv bow = Gd.minf(Gv.F(90.0), (L * Gv.F(0.35)));
    Gv pts = Gd.Arr(a, ((mid + ((n * bow) * Gv.F(0.7))) - (((b - a)) * Gv.F(0.25))), (mid + (n * bow)), ((mid + ((n * bow) * Gv.F(0.7))) + (((b - a)) * Gv.F(0.25))), b);
    Gv arr = Gd.Arr();
    foreach (Gv q in (pts).Iter()) {
      arr.C("append", Gd.Arr(Gd.roundf(q.G("x")), Gd.roundf(q.G("y"))));
    }
    it = it.SI(Gv.S("pts"), arr);
    return Gv.Null;
  }
  public Gv gauge_geo(Gv it = default(Gv)) {
    Gv v = Gv.Null;
    foreach (Gv q in (items).Iter()) {
      if (Gv.Eq(q.G("id"), it.C("get", Gv.S("vid"), Gv.S("-")))) {
        v = q;
      }
    }
    if (Gv.Eq(v, Gv.Null)) {
      return Gd.Dict(Gv.S("x"), Gd.Dyn("float", it.G("x")), Gv.S("y1"), Gd.Dyn("float", it.G("y")), Gv.S("y2"), (Gd.Dyn("float", it.G("y")) + Gd.Dyn("float", it.C("get", Gv.S("h"), Gv.I(200L)))), Gv.S("cx"), Gd.Dyn("float", it.G("x")), Gv.S("free"), Gv.B(true), Gv.S("v"), Gv.Null);
    }
    Gv right = Gv.B(Gv.Ne(it.C("get", Gv.S("side"), Gv.S("R")), Gv.S("L")));
    return Gd.Dict(Gv.S("x"), ((right).T() ? ((v.G("x") + v.G("w")) + Gv.F(22.0)) : (v.G("x") - Gv.F(22.0))), Gv.S("y1"), (v.G("y") + Gd.minf(Gv.F(30.0), (v.G("h") * Gv.F(0.1)))), Gv.S("y2"), ((v.G("y") + v.G("h")) - Gd.minf(Gv.F(20.0), (v.G("h") * Gv.F(0.06)))), Gv.S("cx"), ((right).T() ? ((v.G("x") + v.G("w"))) : Gd.Dyn("float", v.G("x"))), Gv.S("free"), Gv.B(false), Gv.S("v"), v);
    return Gv.Null;
  }
  public Gv gauge_vessel(Gv it = default(Gv)) {
    Gv g = gauge_geo(it);
    if (!(g.G("free")).T()) {
      return g.G("v");
    }
    return sim.C("gauge_vessel_free", it);
    return Gv.Null;
  }
  public Gv vgeo(Gv it = default(Gv)) {
    Gv shape = it.C("get", Gv.S("shape"), Gv.S("tank"));
    Gv bind = it.C("get", Gv.S("bind"));
    if ((Gv.Eq(shape, Gv.S("tank")) && Gv.Eq(bind, Gv.S("rvp")))) {
      return Gd.Arr(Gd.minf((it.G("h") * Gv.F(0.12)), Gv.F(40.0)), Gv.F(20.0));
    }
    if ((Gv.Eq(shape, Gv.S("cone")) && Gv.Ne(bind, Gv.S("wp")))) {
      return Gd.Arr(Gd.minf((it.G("h") * Gv.F(0.25)), Gv.F(60.0)), Gv.F(16.0));
    }
    return Gd.Arr(Gv.F(0.0), Gd.Dyn("float", it.G("w")));
    return Gv.Null;
  }
  public Gv vessel_inside(Gv it = default(Gv)) {
    Gv x = ((it.G("x") + Gv.I(4L))).AsF();
    Gv w = ((it.G("w") - Gv.I(8L))).AsF();
    Gv y = ((it.G("y") + Gv.I(2L))).AsF();
    Gv y0 = (((it.G("y") + it.G("h")) - Gv.I(3L))).AsF();
    if (Gv.Eq(it.C("get", Gv.S("shape")), Gv.S("horiz"))) {
      Gv rr = ((it.G("h") * Gv.F(0.5))).AsF();
      Gv pts = Gd.Ctor("PackedVector2Array");
      foreach (Gv a in (Gv.I(13L)).Iter()) {
        Gv ang = (((-Gd.K("PI")) * Gv.F(0.5)) + ((Gd.K("PI") * a) / Gv.F(12.0)));
        pts.C("append", Gd.Ctor("Vector2", (((it.G("x") + it.G("w")) - rr) + (Gd.cos(ang) * ((rr - Gv.I(3L))))), ((it.G("y") + rr) + (Gd.sin(ang) * ((rr - Gv.I(3L)))))));
      }
      foreach (Gv a_2 in (Gv.I(13L)).Iter()) {
        Gv ang2 = ((Gd.K("PI") * Gv.F(0.5)) + ((Gd.K("PI") * a_2) / Gv.F(12.0)));
        pts.C("append", Gd.Ctor("Vector2", ((it.G("x") + rr) + (Gd.cos(ang2) * ((rr - Gv.I(3L))))), ((it.G("y") + rr) + (Gd.sin(ang2) * ((rr - Gv.I(3L)))))));
      }
      return pts;
    }
    Gv g = vgeo(it);
    Gv c = (g.I(Gv.I(0L))).AsF();
    Gv bw = (g.I(Gv.I(1L))).AsF();
    Gv cx = ((it.G("x") + (it.G("w") * Gv.F(0.5)))).AsF();
    if (Gv.Gt(c, Gv.I(0L))) {
      return Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", x, y), Gd.Ctor("Vector2", (x + w), y), Gd.Ctor("Vector2", (x + w), (y0 - c)), Gd.Ctor("Vector2", ((cx + (bw * Gv.F(0.5))) - Gv.I(2L)), y0), Gd.Ctor("Vector2", ((cx - (bw * Gv.F(0.5))) + Gv.I(2L)), y0), Gd.Ctor("Vector2", x, (y0 - c))));
    }
    return Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", x, y), Gd.Ctor("Vector2", (x + w), y), Gd.Ctor("Vector2", (x + w), y0), Gd.Ctor("Vector2", x, y0)));
    return Gv.Null;
  }
  public Gv level_y(Gv it = default(Gv)) {
    Gv S = sim.G("S");
    Gv c = S.G("cfg");
    Gv b = it.C("get", Gv.S("bind"));
    Gv y0 = (((it.G("y") + it.G("h")) - Gv.I(3L))).AsF();
    Gv H = ((it.G("h") - Gv.I(5L))).AsF();
    Gv V = Gv.F(0.0);
    Gv cap = Gv.F(1.0);
    {
      Gv _t1 = b;
      if (Gv.Eq(_t1, Gv.S("rvp"))) {
        Gv hl = ((sim.C("hotV", S.G("rvp")) / Gv.F(100.0))).AsF();
        Gv cc = Gd.minf((it.G("h") * Gv.F(0.12)), Gv.F(40.0));
        Gv px = ((Gv.Le(hl, Gv.I(3L)) ? (cc * Gd.pow((Gd.maxf(Gv.F(0.0), hl) / Gv.F(3.0)), (Gv.F(1.0) / Gv.F(3.0)))) : (cc + ((((hl - Gv.F(3.0))) / Gv.F(47.0)) * ((it.G("h") - cc)))))).AsF();
        return ((y0 - Gd.minf(px, H))).AsF();
      }
      else if (Gv.Eq(_t1, Gv.S("lv"))) {
        Gv f = Gd.clampf((S.G("lv").G("w") / c.G("lvVol")), Gv.F(0.0), Gv.F(1.0));
        Gv lo = Gv.F(0.0);
        Gv hi = Gv.F(1.0);
        foreach (Gv i in (Gv.I(22L)).Iter()) {
          Gv mid = (((lo + hi)) * Gv.F(0.5));
          Gv th = (Gv.F(2.0) * Gd.acos((Gv.F(1.0) - (Gv.F(2.0) * mid))));
          if (Gv.Lt((((th - Gd.sin(th))) / Gd.K("TAU")), f)) {
            lo = mid;
          } else {
            hi = mid;
          }
        }
        return ((((it.G("y") + it.G("h")) - Gv.I(3L)) - (lo * ((it.G("h") - Gv.I(6L)))))).AsF();
      }
      else if (Gv.Eq(_t1, Gv.S("sk"))) {
        return ((y0 - (Gd.minf(Gv.F(1.0), (sim.C("skVolTotal") / c.G("skCap"))) * H))).AsF();
      }
      else if (Gv.Eq(_t1, Gv.S("wp"))) {
        V = sim.C("vol", S.G("wp"));
        cap = c.G("wpCap");
      }
      else if (Gv.Eq(_t1, Gv.S("mp"))) {
        V = sim.C("vol", S.G("mp"));
        cap = c.G("mpCap");
      }
      else if (Gv.Eq(_t1, Gv.S("hlt"))) {
        V = S.G("hlt").G("w");
        cap = c.G("hltCap");
      }
      else if (Gv.Eq(_t1, Gv.S("spilka"))) {
        V = S.G("sp").G("L");
        cap = c.G("spCap");
      }
      else if (true) {
        return (y0).AsF();
      }
    }
    Gv g = vgeo(it);
    Gv cn = (g.I(Gv.I(0L))).AsF();
    Gv Vc = (((cap * ((cn / Gv.F(3.0)))) / ((((it.G("h") - cn)) + (cn / Gv.F(3.0)))))).AsF();
    Gv p2 = Gv.F(0.0);
    if ((Gv.Gt(cn, Gv.I(0L)) && Gv.Le(V, Vc))) {
      p2 = (cn * Gd.pow((Gd.maxf(Gv.F(0.0), V) / Vc), (Gv.F(1.0) / Gv.F(3.0))));
    } else {
      p2 = (cn + ((((V - Vc)) / Gd.maxf(Gv.F(1.0), (cap - Vc))) * ((it.G("h") - cn))));
    }
    return ((y0 - Gd.clampf(p2, Gv.F(0.0), H))).AsF();
    return Gv.Null;
  }
  public Gv wort_color(Gv ebc = default(Gv)) {
    ebc = (ebc).AsF();
    Gv stops = Gd.Arr(Gd.Arr(Gv.F(0.0), Gv.S("f6e7a1")), Gd.Arr(Gv.F(6.0), Gv.S("efc463")), Gd.Arr(Gv.F(12.0), Gv.S("dc9a2e")), Gd.Arr(Gv.F(25.0), Gv.S("b5651d")), Gd.Arr(Gv.F(50.0), Gv.S("6e3412")), Gd.Arr(Gv.F(100.0), Gv.S("2e1206")));
    foreach (Gv i in Gd.RangeIter(Gv.I(1L), stops.C("size"))) {
      if (Gv.Le(ebc, stops.I(i).I(Gv.I(0L)))) {
        return Gd.Ctor("Color", stops.I((i - Gv.I(1L))).I(Gv.I(1L))).C("lerp", Gd.Ctor("Color", stops.I(i).I(Gv.I(1L))), (((ebc - stops.I((i - Gv.I(1L))).I(Gv.I(0L)))) / ((stops.I(i).I(Gv.I(0L)) - stops.I((i - Gv.I(1L))).I(Gv.I(0L))))));
      }
    }
    return Gd.Ctor("Color", Gv.S("2e1206"));
    return Gv.Null;
  }
  public Gv kind_color(Gv kind = default(Gv), Gv T = default(Gv), Gv ebc = default(Gv)) {
    T = (T).AsF();
    ebc = (ebc).AsF();
    {
      Gv _t1 = kind;
      if (Gv.Eq(_t1, Gv.S("wort"))) {
        return wort_color(ebc);
      }
      else if (Gv.Eq(_t1, Gv.S("mash"))) {
        return wort_color(ebc).C("lerp", Gd.Ctor("Color", Gv.S("cdb68a")), Gv.F(0.55));
      }
      else if (Gv.Eq(_t1, Gv.S("ice"))) {
        return Gd.Ctor("Color", Gv.S("9fd8ff"));
      }
      else if (Gv.Eq(_t1, Gv.S("empty"))) {
        return Gd.Ctor("Color", Gv.I(0L), Gv.I(0L), Gv.I(0L), Gv.I(0L));
      }
    }
    return Gd.Ctor("Color", Gv.S("5aaee6")).C("lerp", Gd.Ctor("Color", Gv.S("e8743b")), Gd.clampf((((T - Gv.F(20.0))) / Gv.F(70.0)), Gv.F(0.0), Gv.F(1.0)));
    return Gv.Null;
  }
  public Gv vessel_color(Gv it = default(Gv)) {
    Gv S = sim.G("S");
    Gv b = it.C("get", Gv.S("bind"));
    Gv col = Gv.Col(0, 0, 0, 1);
    {
      Gv _t1 = b;
      if (Gv.Eq(_t1, Gv.S("rvp")) || Gv.Eq(_t1, Gv.S("mp")) || Gv.Eq(_t1, Gv.S("wp"))) {
        Gv v = S.I(b);
        col = kind_color(sim.C("contentKind", v), v.G("T"), (v.G("col") / Gd.maxf(Gv.F(1.0), sim.C("liqVol", v))));
        if ((Gv.Eq(sim.C("contentKind", v), Gv.S("wort")) && Gv.Gt(((v.G("alpha") + v.G("iso"))), Gv.F(0.5)))) {
          col = col.C("lerp", Gd.Ctor("Color", Gv.S("c4701c")), Gv.F(0.14));
        }
      }
      else if (Gv.Eq(_t1, Gv.S("hlt"))) {
        col = kind_color(Gv.S("water"), S.G("hlt").G("T"), Gv.F(0.0));
      }
      else if (Gv.Eq(_t1, Gv.S("lv"))) {
        col = kind_color((Gv.Lt(S.G("lv").G("T"), Gv.I(6L)) ? Gv.S("ice") : Gv.S("water")), S.G("lv").G("T"), Gv.F(0.0));
      }
      else if (Gv.Eq(_t1, Gv.S("spilka"))) {
        col = (Gv.Gt((S.G("sp").G("f") + S.G("sp").G("n")), Gv.I(1L)) ? wort_color((S.G("sp").G("col") / Gd.maxf(Gv.F(1.0), S.G("sp").G("L")))) : kind_color(Gv.S("water"), Gv.F(20.0), Gv.F(0.0)));
      }
      else if (Gv.Eq(_t1, Gv.S("sk"))) {
        Gv c0 = S.G("sk").G("cells").I(Gv.I(0L));
        if (Gv.Gt(sim.C("vol", S.G("sk").G("mash")), Gv.I(50L))) {
          col = kind_color(Gv.S("mash"), S.G("sk").G("mash").G("T"), (S.G("sk").G("mash").G("col") / Gd.maxf(Gv.F(1.0), sim.C("liqVol", S.G("sk").G("mash")))));
        } else if (Gv.Gt((c0.G("f") + c0.G("n")), (Gv.F(0.005) * Gd.maxf(Gv.F(1.0), c0.G("w"))))) {
          col = wort_color((c0.G("col") / Gd.maxf(Gv.F(1.0), c0.G("w"))));
        } else {
          col = kind_color(Gv.S("water"), S.G("sk").G("T"), Gv.F(0.0));
        }
      }
      else if (true) {
        col = Gd.Ctor("Color", Gv.F(0.4), Gv.F(0.6), Gv.F(0.8));
      }
    }
    col = col.S("a", Gv.F(0.88));
    return col;
    return Gv.Null;
  }
  public Gv c1(Gv x = default(Gv)) {
    x = (x).AsF();
    return ((Gv.S("%.1f") % x)).C("replace", Gv.S("."), Gv.S(","));
    return Gv.Null;
  }
  public Gv vessel_text(Gv b = default(Gv)) {
    Gv t = _vessel_text0(b);
    if ((hard).T()) {
      Gv re = Gd.New("RegEx");
      re.C("compile", Gv.S(" · [0-9,]+ hl|[0-9,]+ hl · "));
      t = re.C("sub", t, Gv.S(""), Gv.B(true));
    }
    return t;
    return Gv.Null;
  }
  public Gv _vessel_text0(Gv b = default(Gv)) {
    Gv S = sim.G("S");
    {
      Gv _t1 = b;
      if (Gv.Eq(_t1, Gv.S("rvp")) || Gv.Eq(_t1, Gv.S("mp")) || Gv.Eq(_t1, Gv.S("wp"))) {
        Gv v = S.I(b);
        Gv V = ((Gv.Eq(b, Gv.S("rvp")) ? sim.C("hotV", v) : sim.C("vol", v))).AsF();
        if (Gv.Lt(V, Gv.I(5L))) {
          return Gv.S("prázdná");
        }
        Gv t = (Gv.S("%s °C · %s hl") % Gd.Arr(c1(v.G("Tsens")), c1((V / Gv.F(100.0)))));
        if ((Gv.Gt(v.G("Pin"), Gv.F(1.0)) && Gv.Gt((v.G("Twall") - v.G("T")), Gv.F(1.5)))) {
          t = (t + (Gv.S(" (u pláště %s °C)") % c1(v.G("Twall"))));
        }
        if (Gv.Gt(V, Gv.I(300L))) {
          t = (t + (Gv.S(" · ρ %s kg/l") % ((Gv.S("%.3f") % sim.C("density", v))).C("replace", Gv.S("."), Gv.S(","))));
        }
        if (Gv.Gt(sim.C("ext", v), Gv.I(5L))) {
          t = (t + (Gv.S(" · %s °P") % c1(sim.C("plato", v))));
        }
        if ((v.G("boiling")).T()) {
          t = (t + Gv.S(" · VŘE"));
        }
        if ((v.G("stir")).T()) {
          t = (t + Gv.S(" · míchá"));
        }
        return t;
      }
      else if (Gv.Eq(_t1, Gv.S("sk"))) {
        Gv V2 = (sim.C("skVolTotal")).AsF();
        if (Gv.Lt(V2, Gv.I(5L))) {
          return (Gv.S("prázdná · stěny %s °C") % c1(S.G("sk").G("Tw")));
        }
        Gv t2 = (Gv.S("%s °C · %s hl") % Gd.Arr(c1(S.G("sk").G("T")), c1((V2 / Gv.F(100.0)))));
        if (Gv.Gt(S.G("sk").G("g"), Gv.I(1L))) {
          t2 = (t2 + (Gv.S(" · lože %d cm") % Gd.roundi((sim.C("skBedH") * Gv.F(100.0)))));
        }
        if ((S.G("sk").G("drawn")).T()) {
          t2 = (t2 + (Gv.S(" · výtok %s °P, zákal %d") % Gd.Arr(c1(S.G("sk").G("outP")), Gd.roundi(S.G("sk").G("turb")))));
        }
        return t2;
      }
      else if (Gv.Eq(_t1, Gv.S("hlt"))) {
        return (Gv.S("%s °C · %s hl") % Gd.Arr(c1(S.G("hlt").G("T")), c1((S.G("hlt").G("w") / Gv.F(100.0)))));
      }
      else if (Gv.Eq(_t1, Gv.S("lv"))) {
        if (Gv.Lt(S.G("lv").G("w"), Gv.I(5L))) {
          return Gv.S("prázdná");
        }
        return (Gv.S("%s °C · %s hl%s") % Gd.Arr(c1(S.G("lv").G("T")), c1((S.G("lv").G("w") / Gv.F(100.0))), ((S.G("lv").G("chillOn")).T() ? Gv.S(" · agregát chladí") : Gv.S(""))));
      }
      else if (Gv.Eq(_t1, Gv.S("spilka"))) {
        if (Gv.Lt(S.G("sp").G("L"), Gv.I(5L))) {
          return Gv.S("prázdná");
        }
        Gv e = ((S.G("sp").G("f") + S.G("sp").G("n"))).AsF();
        return (Gv.S("%s hl · %s °P · %s °C") % Gd.Arr(c1((S.G("sp").G("L") / Gv.F(100.0))), c1(((Gv.F(100.0) * e) / Gd.maxf(Gv.F(1.0), (e + S.G("sp").G("w"))))), c1((S.G("sp").G("H") / S.G("sp").G("L")))));
      }
    }
    return Gv.S("");
    return Gv.Null;
  }
  public Gv vessel_card(Gv b = default(Gv)) {
    Gv S = sim.G("S");
    Gv d = Gd.Dict(Gv.S("T"), Gv.Null, Gv.S("lines"), Gd.Arr(), Gv.S("flags"), Gd.Arr(), Gv.S("empty"), Gv.B(false));
    {
      Gv _t1 = b;
      if (Gv.Eq(_t1, Gv.S("rvp")) || Gv.Eq(_t1, Gv.S("mp")) || Gv.Eq(_t1, Gv.S("wp"))) {
        Gv v = S.I(b);
        Gv V = ((Gv.Eq(b, Gv.S("rvp")) ? sim.C("hotV", v) : sim.C("vol", v))).AsF();
        if (Gv.Lt(V, Gv.I(5L))) {
          d = d.S("empty", Gv.B(true));
          return d;
        }
        d = d.S("T", v.G("Tsens"));
        d.G("lines").C("append", (Gv.S("%s hl") % c1((V / Gv.F(100.0)))));
        if (Gv.Gt(sim.C("ext", v), Gv.I(5L))) {
          d.G("lines").C("append", (Gv.S("%s °P") % c1(sim.C("plato", v))));
        } else if (Gv.Gt(V, Gv.I(300L))) {
          d.G("lines").C("append", (Gv.S("ρ %s") % ((Gv.S("%.3f") % sim.C("density", v))).C("replace", Gv.S("."), Gv.S(","))));
        }
        if ((v.G("boiling")).T()) {
          d.G("flags").C("append", Gd.Arr(Gv.S("VŘE"), Gd.Ctor("Color", Gv.S("ff6b4a"))));
        }
        if ((v.G("stir")).T()) {
          d.G("flags").C("append", Gd.Arr(Gv.S("míchá"), Gd.Ctor("Color", Gv.S("5aa9ff"))));
        }
        if ((Gv.Gt(v.G("Pin"), Gv.F(1.0)) && Gv.Gt((v.G("Twall") - v.G("T")), Gv.F(1.5)))) {
          d.G("flags").C("append", Gd.Arr((Gv.S("plášť %d °C") % Gd.roundi(v.G("Twall"))), Gd.Ctor("Color", Gv.S("ff9a3c"))));
        }
      }
      else if (Gv.Eq(_t1, Gv.S("sk"))) {
        Gv V2 = (sim.C("skVolTotal")).AsF();
        if (Gv.Lt(V2, Gv.I(5L))) {
          d = d.S("empty", Gv.B(true));
          d.G("lines").C("append", (Gv.S("stěny %s °C") % c1(S.G("sk").G("Tw"))));
          return d;
        }
        d = d.S("T", S.G("sk").G("T"));
        d.G("lines").C("append", (Gv.S("%s hl") % c1((V2 / Gv.F(100.0)))));
        if (Gv.Gt(S.G("sk").G("g"), Gv.I(1L))) {
          d.G("lines").C("append", (Gv.S("lože %d cm") % Gd.roundi((sim.C("skBedH") * Gv.F(100.0)))));
        }
        if ((S.G("sk").G("drawn")).T()) {
          d.G("flags").C("append", Gd.Arr((Gv.S("výtok %s °P") % c1(S.G("sk").G("outP"))), Gd.Ctor("Color", Gv.S("f2c230"))));
          d.G("flags").C("append", Gd.Arr((Gv.S("zákal %d") % Gd.roundi(S.G("sk").G("turb"))), Gd.Ctor("Color", Gv.S("c9d1d8"))));
        }
        if ((Gv.Gt(S.G("sk").C("get", Gv.S("dp"), Gv.F(0.0)), Gv.F(1.0)) && Gv.Gt(S.G("sk").G("g"), Gv.I(1L)))) {
          d.G("flags").C("append", Gd.Arr((Gv.S("Δp %d mbar") % Gd.roundi(S.G("sk").G("dp"))), (Gv.Gt(S.G("sk").G("dp"), Gv.I(150L)) ? Gd.Ctor("Color", Gv.S("ff6b4a")) : ((Gv.Gt(S.G("sk").G("dp"), Gv.I(80L)) ? Gd.Ctor("Color", Gv.S("f2c230")) : Gd.Ctor("Color", Gv.S("4fd18b")))))));
        }
      }
      else if (Gv.Eq(_t1, Gv.S("hlt"))) {
        d = d.S("T", S.G("hlt").G("T"));
        d.G("lines").C("append", (Gv.S("%s hl") % c1((S.G("hlt").G("w") / Gv.F(100.0)))));
      }
      else if (Gv.Eq(_t1, Gv.S("lv"))) {
        if (Gv.Lt(S.G("lv").G("w"), Gv.I(5L))) {
          d = d.S("empty", Gv.B(true));
          return d;
        }
        d = d.S("T", S.G("lv").G("T"));
        d.G("lines").C("append", (Gv.S("%s hl") % c1((S.G("lv").G("w") / Gv.F(100.0)))));
        if ((S.G("lv").G("chillOn")).T()) {
          d.G("flags").C("append", Gd.Arr(Gv.S("agregát chladí"), Gd.Ctor("Color", Gv.S("5aa9ff"))));
        }
      }
      else if (Gv.Eq(_t1, Gv.S("spilka"))) {
        if (Gv.Lt(S.G("sp").G("L"), Gv.I(5L))) {
          d = d.S("empty", Gv.B(true));
          return d;
        }
        Gv e = ((S.G("sp").G("f") + S.G("sp").G("n"))).AsF();
        d = d.S("T", (S.G("sp").G("H") / S.G("sp").G("L")));
        d.G("lines").C("append", (Gv.S("%s hl") % c1((S.G("sp").G("L") / Gv.F(100.0)))));
        d.G("lines").C("append", (Gv.S("%s °P") % c1(((Gv.F(100.0) * e) / Gd.maxf(Gv.F(1.0), (e + S.G("sp").G("w")))))));
        if ((S.C("get", Gv.S("fer"), Gd.Dict()).C("get", Gv.S("on"), Gv.B(false))).T()) {
          d.G("flags").C("append", Gd.Arr(Gv.S("kvasí"), Gd.Ctor("Color", Gv.S("4fd18b"))));
        }
      }
      else if (true) {
        return Gd.Dict();
      }
    }
    if ((hard).T()) {
      Gv keep = Gd.Arr();
      foreach (Gv l in (d.G("lines")).Iter()) {
        if (!(Gd.Dyn("str", l).C("ends_with", Gv.S(" hl"))).T()) {
          keep.C("append", l);
        }
      }
      d = d.S("lines", keep);
    }
    return d;
    return Gv.Null;
  }
  public static Gv temp_color(Gv T = default(Gv)) {
    T = (T).AsF();
    if (Gv.Lt(T, Gv.F(15.0))) {
      return Gd.Ctor("Color", Gv.S("5aa9ff"));
    }
    if (Gv.Lt(T, Gv.F(45.0))) {
      return Gd.Ctor("Color", Gv.S("4fd1c5"));
    }
    if (Gv.Lt(T, Gv.F(66.0))) {
      return Gd.Ctor("Color", Gv.S("9be15d"));
    }
    if (Gv.Lt(T, Gv.F(80.0))) {
      return Gd.Ctor("Color", Gv.S("f2c230"));
    }
    if (Gv.Lt(T, Gv.F(95.0))) {
      return Gd.Ctor("Color", Gv.S("ff9a3c"));
    }
    return Gd.Ctor("Color", Gv.S("ff5a4a"));
    return Gv.Null;
  }
  public Gv _live_node(Gv kind = default(Gv), Gv it = default(Gv)) {
    Gv n = S_main_LiveNode.New();
    n = n.S("m", Self);
    n = n.S("live_only", Gv.B(true));
    n = n.S("kind", kind);
    n = n.S("it", it);
    Self.C("add_child", n);
    live_nodes.C("append", n);
    return n;
    return Gv.Null;
  }
  public Gv update_live() {
    foreach (Gv n in (live_nodes).Iter()) {
      n.C("upd");
    }
    if (Gv.Ne(cabinet, Gv.Null)) {
      cabinet.C("upd");
    }
    return Gv.Null;
  }
  public Gv label_size() {
    Gv z = (cam.G("zoom").G("x")).AsF();
    return ((Gv.Ge(z, Gv.F(0.8)) ? Gv.I(14L) : ((Gv.Ge(z, Gv.F(0.5)) ? Gv.I(20L) : ((Gv.Ge(z, Gv.F(0.33)) ? Gv.I(28L) : Gv.I(38L))))))).AsI();
    return Gv.Null;
  }
  public Gv redraw() {
    flow_t = Gv.F(99.0);
    if (Gv.Ne(item_layer, Gv.Null)) {
      item_layer.C("queue_redraw");
    }
    if (Gv.Ne(valve_layer, Gv.Null)) {
      valve_layer.C("queue_redraw");
    }
    update_live();
    return Gv.Null;
  }
  public Gv redraw_valves() {
    if (Gv.Ne(valve_layer, Gv.Null)) {
      valve_layer.C("queue_redraw");
    }
    flow_t = Gv.F(99.0);
    return Gv.Null;
  }
  public Gv vessel_by_bind(Gv b = default(Gv)) {
    foreach (Gv it in (items).Iter()) {
      if ((Gv.Eq(it.G("type"), Gv.S("vessel")) && Gv.Eq(it.C("get", Gv.S("bind")), b))) {
        return it;
      }
    }
    return Gv.Null;
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "Art": return S_main.Art;
    case "Sim": return S_main.Sim;
    case "Ctl": return S_main.Ctl;
    case "Auto": return S_main.Auto;
    case "Sight": return S_main.Sight;
    case "Iodine": return S_main.Iodine;
    case "Cabinet": return S_main.Cabinet;
    case "LiquidFx": return S_main.LiquidFx;
    case "cabinet": return this.cabinet;
    case "cab_hold": return this.cab_hold;
    case "iodine_win": return this.iodine_win;
    case "sight_win": return this.sight_win;
    case "auto": return this.auto;
    case "PANEL_W": return S_main.PANEL_W;
    case "panel_on": return this.panel_on;
    case "dock_on": return this.dock_on;
    case "dock": return this.dock;
    case "dock_big": return this.dock_big;
    case "hard": return this.hard;
    case "calm_since": return this.calm_since;
    case "calm_y": return this.calm_y;
    case "sim": return this.sim;
    case "ctl": return this.ctl;
    case "flow_layer": return this.flow_layer;
    case "live_nodes": return this.live_nodes;
    case "hl": return this.hl;
    case "NOLIVE": return this.NOLIVE;
    case "NOFLOW": return this.NOFLOW;
    case "T_live": return this.T_live;
    case "T_flow": return this.T_flow;
    case "T_sim": return this.T_sim;
    case "N_sim": return this.N_sim;
    case "NOSIM": return this.NOSIM;
    case "dyn_t": return this.dyn_t;
    case "flow_t": return this.flow_t;
    case "calm_t": return this.calm_t;
    case "running": return this.running;
    case "speed": return this.speed;
    case "sim_acc": return this.sim_acc;
    case "PIPE_OUT": return S_main.PIPE_OUT;
    case "PIPE_MID": return S_main.PIPE_MID;
    case "PIPE_IN": return S_main.PIPE_IN;
    case "STEEL": return S_main.STEEL;
    case "STEEL_D": return S_main.STEEL_D;
    case "DARK": return S_main.DARK;
    case "OPEN_C": return S_main.OPEN_C;
    case "SHUT_C": return S_main.SHUT_C;
    case "ACCENT": return S_main.ACCENT;
    case "plan": return this.plan;
    case "items": return this.items;
    case "bounds": return this.bounds;
    case "floor_y": return this.floor_y;
    case "no_gpu_liquid": return this.no_gpu_liquid;
    case "hall_dark": return this.hall_dark;
    case "cam": return this.cam;
    case "static_layer": return this.static_layer;
    case "item_layer": return this.item_layer;
    case "valve_layer": return this.valve_layer;
    case "hud": return this.hud;
    case "game": return this.game;
    case "mini": return this.mini;
    case "updater": return this.updater;
    case "font": return this.font;
    case "font_b": return this.font_b;
    case "zoom_target": return this.zoom_target;
    case "zoom_anchor": return this.zoom_anchor;
    case "zoom_anim": return this.zoom_anim;
    case "dragging": return this.dragging;
    case "drag_moved": return this.drag_moved;
    case "drag_start": return this.drag_start;
    case "vel": return this.vel;
    case "last_move_ms": return this.last_move_ms;
    case "USER_PLAN": return S_main.USER_PLAN;
    case "plan_user": return this.plan_user;
    case "editor": return this.editor;
    case "v3": return this.v3;
    case "practice": return this.practice;
    case "cam_anim": return this.cam_anim;
    case "cam_to_p": return this.cam_to_p;
    case "cam_to_z": return this.cam_to_z;
    case "AA_SHADER": return S_main.AA_SHADER;
    case "aa_layer": return this.aa_layer;
    case "SAVE_PATH": return S_main.SAVE_PATH;
    case "PLAN_REV": return S_main.PLAN_REV;
    case "save_path": return this.save_path;
    case "save_acc": return this.save_acc;
    case "THROTTLE_IDS": return S_main.THROTTLE_IDS;
    case "valve_pop": return this.valve_pop;
    case "vdrag": return this.vdrag;
    case "vdrag_pct": return this.vdrag_pct;
    case "_vnode": return this._vnode;
    case "VNAME": return S_main.VNAME;
    case "undo_stack": return this.undo_stack;
    case "BAKE_SCALE": return S_main.BAKE_SCALE;
    case "TILE": return S_main.TILE;
    case "tiles_root": return this.tiles_root;
    case "detail_spr": return this.detail_spr;
    case "detail_rect": return this.detail_rect;
    case "detail_scale": return this.detail_scale;
    case "detail_busy": return this.detail_busy;
    case "detail_idle": return this.detail_idle;
    case "detail_sig": return this.detail_sig;
    case "detail_svs": return this.detail_svs;
    case "detail_cur": return this.detail_cur;
    case "outlets": return this.outlets;
    case "outlet_open": return this.outlet_open;
    case "hose_drag": return this.hose_drag;
    case "pipe_pts": return this.pipe_pts;
    case "PIPE_LIQ": return S_main.PIPE_LIQ;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "cabinet": this.cabinet = v; return v;
    case "cab_hold": this.cab_hold = v; return v;
    case "iodine_win": this.iodine_win = v; return v;
    case "sight_win": this.sight_win = v; return v;
    case "auto": this.auto = v; return v;
    case "panel_on": this.panel_on = v; return v;
    case "dock_on": this.dock_on = v; return v;
    case "dock": this.dock = v; return v;
    case "dock_big": this.dock_big = v; return v;
    case "hard": this.hard = v; return v;
    case "calm_since": this.calm_since = v; return v;
    case "calm_y": this.calm_y = v; return v;
    case "sim": this.sim = v; return v;
    case "ctl": this.ctl = v; return v;
    case "flow_layer": this.flow_layer = v; return v;
    case "live_nodes": this.live_nodes = v; return v;
    case "hl": this.hl = v; return v;
    case "NOLIVE": this.NOLIVE = v; return v;
    case "NOFLOW": this.NOFLOW = v; return v;
    case "T_live": this.T_live = v; return v;
    case "T_flow": this.T_flow = v; return v;
    case "T_sim": this.T_sim = v; return v;
    case "N_sim": this.N_sim = v; return v;
    case "NOSIM": this.NOSIM = v; return v;
    case "dyn_t": this.dyn_t = v; return v;
    case "flow_t": this.flow_t = v; return v;
    case "calm_t": this.calm_t = v; return v;
    case "running": this.running = v; return v;
    case "speed": this.speed = v; return v;
    case "sim_acc": this.sim_acc = v; return v;
    case "plan": this.plan = v; return v;
    case "items": this.items = v; return v;
    case "bounds": this.bounds = v; return v;
    case "floor_y": this.floor_y = v; return v;
    case "no_gpu_liquid": this.no_gpu_liquid = v; return v;
    case "hall_dark": this.hall_dark = v; return v;
    case "cam": this.cam = v; return v;
    case "static_layer": this.static_layer = v; return v;
    case "item_layer": this.item_layer = v; return v;
    case "valve_layer": this.valve_layer = v; return v;
    case "hud": this.hud = v; return v;
    case "game": this.game = v; return v;
    case "mini": this.mini = v; return v;
    case "updater": this.updater = v; return v;
    case "font": this.font = v; return v;
    case "font_b": this.font_b = v; return v;
    case "zoom_target": this.zoom_target = v; return v;
    case "zoom_anchor": this.zoom_anchor = v; return v;
    case "zoom_anim": this.zoom_anim = v; return v;
    case "dragging": this.dragging = v; return v;
    case "drag_moved": this.drag_moved = v; return v;
    case "drag_start": this.drag_start = v; return v;
    case "vel": this.vel = v; return v;
    case "last_move_ms": this.last_move_ms = v; return v;
    case "plan_user": this.plan_user = v; return v;
    case "editor": this.editor = v; return v;
    case "v3": this.v3 = v; return v;
    case "practice": this.practice = v; return v;
    case "cam_anim": this.cam_anim = v; return v;
    case "cam_to_p": this.cam_to_p = v; return v;
    case "cam_to_z": this.cam_to_z = v; return v;
    case "aa_layer": this.aa_layer = v; return v;
    case "save_path": this.save_path = v; return v;
    case "save_acc": this.save_acc = v; return v;
    case "valve_pop": this.valve_pop = v; return v;
    case "vdrag": this.vdrag = v; return v;
    case "vdrag_pct": this.vdrag_pct = v; return v;
    case "_vnode": this._vnode = v; return v;
    case "undo_stack": this.undo_stack = v; return v;
    case "tiles_root": this.tiles_root = v; return v;
    case "detail_spr": this.detail_spr = v; return v;
    case "detail_rect": this.detail_rect = v; return v;
    case "detail_scale": this.detail_scale = v; return v;
    case "detail_busy": this.detail_busy = v; return v;
    case "detail_idle": this.detail_idle = v; return v;
    case "detail_sig": this.detail_sig = v; return v;
    case "detail_svs": this.detail_svs = v; return v;
    case "detail_cur": this.detail_cur = v; return v;
    case "outlets": this.outlets = v; return v;
    case "outlet_open": this.outlet_open = v; return v;
    case "hose_drag": this.hose_drag = v; return v;
    case "pipe_pts": this.pipe_pts = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "right_w": return right_w();
    case "_load_ui": return _load_ui();
    case "_save_ui": return _save_ui();
    case "apply_ui": return apply_ui();
    case "toggle_panel": return toggle_panel();
    case "toggle_dock": return toggle_dock();
    case "dock_temp": return dock_temp(Gd.A(a, 0));
    case "toggle_hard": return toggle_hard();
    case "vessel_calm": return vessel_calm(Gd.A(a, 0));
    case "hard_sig": return hard_sig(Gd.A(a, 0));
    case "_calm_tick": return _calm_tick();
    case "dip_geo": return dip_geo(Gd.A(a, 0));
    case "measure_dip": return measure_dip(Gd.A(a, 0));
    case "measure_gauge": return measure_gauge(Gd.A(a, 0));
    case "dock_w": return dock_w();
    case "toggle_dock_big": return toggle_dock_big();
    case "_ready": return _ready();
    case "_savetest": return _savetest();
    case "_run_tests": return _run_tests();
    case "_autotest": return _autotest();
    case "_simtest": return _simtest();
    case "make_plan_layers": return make_plan_layers();
    case "open_editor": return open_editor();
    case "_patch_pitch": return _patch_pitch(Gd.A(a, 0));
    case "_patch_wpvalve": return _patch_wpvalve(Gd.A(a, 0));
    case "_patch_skstub": return _patch_skstub(Gd.A(a, 0));
    case "_patch_skjoint": return _patch_skjoint(Gd.A(a, 0));
    case "_patch_splid": return _patch_splid(Gd.A(a, 0));
    case "_patch_ring": return _patch_ring(Gd.A(a, 0));
    case "_patch_sk": return _patch_sk(Gd.A(a, 0));
    case "_load_plan": return _load_plan();
    case "compute_layout": return compute_layout();
    case "foot_y": return foot_y(Gd.A(a, 0));
    case "vp_size": return vp_size();
    case "screen_to_world": return screen_to_world(Gd.A(a, 0));
    case "fit_all": return fit_all();
    case "go_to": return go_to(Gd.A(a, 0), Gd.A(a, 1));
    case "zoom_at": return zoom_at(Gd.A(a, 0), Gd.A(a, 1));
    case "_process": return _process(Gd.A(a, 0));
    case "_make_aa": return _make_aa();
    case "toggle_3d": return toggle_3d(Gd.A(a, 0));
    case "_unhandled_input": return _unhandled_input(Gd.A(a, 0));
    case "save_game": return save_game();
    case "load_game": return load_game();
    case "show_route": return show_route(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5));
    case "_fix_parts": return _fix_parts(Gd.A(a, 0));
    case "_merge_defaults": return _merge_defaults(Gd.A(a, 0), Gd.A(a, 1));
    case "open_iodine": return open_iodine(Gd.A(a, 0));
    case "open_sight_c2": return open_sight_c2();
    case "new_game": return new_game();
    case "_notification": return _notification(Gd.A(a, 0));
    case "_hose_grab": return _hose_grab(Gd.A(a, 0));
    case "_hose_drop": return _hose_drop();
    case "_throttle_step": return _throttle_step(Gd.A(a, 0), Gd.A(a, 1));
    case "_valve_pct": return _valve_pct(Gd.A(a, 0), Gd.A(a, 1));
    case "_set_valve_pct": return _set_valve_pct(Gd.A(a, 0), Gd.A(a, 1));
    case "can_throttle": return can_throttle(Gd.A(a, 0));
    case "_valve_at": return _valve_at(Gd.A(a, 0));
    case "_valve_slider": return _valve_slider(Gd.A(a, 0), Gd.A(a, 1));
    case "malt_gate_pos": return malt_gate_pos(Gd.A(a, 0));
    case "_say": return _say(Gd.A(a, 0));
    case "_shift_hose": return _shift_hose(Gd.A(a, 0));
    case "flow_reach": return flow_reach(Gd.A(a, 0));
    case "flow_targets": return flow_targets(Gd.A(a, 0));
    case "undo": return undo();
    case "pick": return pick(Gd.A(a, 0));
    case "_tap": return _tap(Gd.A(a, 0));
    case "cab_why": return cab_why();
    case "toggle_pump": return toggle_pump(Gd.A(a, 0));
    case "pump_name": return pump_name(Gd.A(a, 0));
    case "pump_primed": return pump_primed(Gd.A(a, 0));
    case "pump_state": return pump_state(Gd.A(a, 0));
    case "set_hall": return set_hall(Gd.A(a, 0));
    case "_bake": return _bake();
    case "visible_world_rect": return visible_world_rect();
    case "_detail_tick": return _detail_tick(Gd.A(a, 0));
    case "_bake_detail": return _bake_detail(Gd.A(a, 0), Gd.A(a, 1));
    case "mf_ports": return mf_ports(Gd.A(a, 0));
    case "_init_outlets": return _init_outlets();
    case "hose_at": return hose_at(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "outlet_at": return outlet_at(Gd.A(a, 0), Gd.A(a, 1));
    case "hose_shape": return hose_shape(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "gauge_geo": return gauge_geo(Gd.A(a, 0));
    case "gauge_vessel": return gauge_vessel(Gd.A(a, 0));
    case "vgeo": return vgeo(Gd.A(a, 0));
    case "vessel_inside": return vessel_inside(Gd.A(a, 0));
    case "level_y": return level_y(Gd.A(a, 0));
    case "wort_color": return wort_color(Gd.A(a, 0));
    case "kind_color": return kind_color(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "vessel_color": return vessel_color(Gd.A(a, 0));
    case "c1": return c1(Gd.A(a, 0));
    case "vessel_text": return vessel_text(Gd.A(a, 0));
    case "_vessel_text0": return _vessel_text0(Gd.A(a, 0));
    case "vessel_card": return vessel_card(Gd.A(a, 0));
    case "temp_color": return S_main.temp_color(Gd.A(a, 0));
    case "_live_node": return _live_node(Gd.A(a, 0), Gd.A(a, 1));
    case "update_live": return update_live();
    case "label_size": return label_size();
    case "redraw": return redraw();
    case "redraw_valves": return redraw_valves();
    case "vessel_by_bind": return vessel_by_bind(Gd.A(a, 0));
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    case "Art": return Art;
    case "Sim": return Sim;
    case "Ctl": return Ctl;
    case "Auto": return Auto;
    case "Sight": return Sight;
    case "Iodine": return Iodine;
    case "Cabinet": return Cabinet;
    case "LiquidFx": return LiquidFx;
    case "PANEL_W": return PANEL_W;
    case "PIPE_OUT": return PIPE_OUT;
    case "PIPE_MID": return PIPE_MID;
    case "PIPE_IN": return PIPE_IN;
    case "STEEL": return STEEL;
    case "STEEL_D": return STEEL_D;
    case "DARK": return DARK;
    case "OPEN_C": return OPEN_C;
    case "SHUT_C": return SHUT_C;
    case "ACCENT": return ACCENT;
    case "USER_PLAN": return USER_PLAN;
    case "AA_SHADER": return AA_SHADER;
    case "SAVE_PATH": return SAVE_PATH;
    case "PLAN_REV": return PLAN_REV;
    case "THROTTLE_IDS": return THROTTLE_IDS;
    case "VNAME": return VNAME;
    case "BAKE_SCALE": return BAKE_SCALE;
    case "TILE": return TILE;
    case "PIPE_LIQ": return PIPE_LIQ;
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "temp_color": return temp_color(Gd.A(a, 0));
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_main(); return Gv.O(o); }
  public override string ScriptName { get { return "main"; } }
}
}
