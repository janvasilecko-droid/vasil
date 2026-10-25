// Vygenerováno z cabinet.gd překladačem gd2cs.py – needituj ručně.
using System; using System.Collections.Generic;
namespace Pivovar.Gd.Gen {
public partial class S_cabinet : GdNode {
  public static Gv Art;
  public Gv m;
  public Gv it;
  public Gv sig;
  public Gv sc;
  public Gv fault_extra;
  public static Gv CAB_W;
  public static Gv CAB_H;
  public static Gv PX;
  public static Gv PW;
  public static Gv COLS;
  public static Gv ROW_Y;
  public static Gv ROWS;
  public static Gv DISP;
  public static Gv DISP_X;
  public static Gv DISP_Y;
  public static Gv FMS;
  public Gv flash;
  static S_cabinet() {
    Art = Gd.Script("art");
    CAB_W = Gv.F(770.0);
    CAB_H = Gv.F(1130.0);
    PX = Gv.F(200.0);
    PW = Gv.F(570.0);
    COLS = Gd.Arr(Gv.F(266.0), Gv.F(353.0), Gv.F(441.0), Gv.F(528.0), Gv.F(616.0), Gv.F(703.0));
    ROW_Y = Gd.Arr(Gv.F(312.0), Gv.F(452.0), Gv.F(592.0), Gv.F(732.0));
    ROWS = Gd.Arr(Gd.Arr(Gd.Arr(Gv.S("bxs"), Gd.Arr(Gv.S("ČERPADLO"), Gv.S("C2"), Gv.S("SLADINA")), Gv.S("sel"), Gv.S("black")), Gd.Arr(Gv.S("bxp"), Gd.Arr(Gv.S("RYCHLOST"), Gv.S("C2")), Gv.S("pot"), Gv.S("")), Gd.Arr(Gv.S("run"), Gd.Arr(Gv.S("PROVOZ")), Gv.S("lamp"), Gv.S("green")), Gd.Arr(Gv.S("blank"), Gd.Arr(Gv.S("")), Gv.S("blank"), Gv.S("")), Gd.Arr(Gv.S("lmp"), Gd.Arr(Gv.S("OSVĚTLENÍ"), Gv.S("MP")), Gv.S("sel"), Gv.S("green")), Gd.Arr(Gv.S("lsk"), Gd.Arr(Gv.S("OSVĚTLENÍ"), Gv.S("SK")), Gv.S("sel"), Gv.S("green"))), Gd.Arr(Gd.Arr(Gv.S("stirmp"), Gd.Arr(Gv.S("MÍCHADLO"), Gv.S("MP")), Gv.S("sel"), Gv.S("green")), Gd.Arr(Gv.S("rstA"), Gd.Arr(Gv.S("RESET"), Gv.S("PORUCHY"), Gv.S("FMB")), Gv.S("push"), Gv.S("blue")), Gd.Arr(Gv.S("potmp"), Gd.Arr(Gv.S("RYCHLOST"), Gv.S("FMB")), Gv.S("pot"), Gv.S("")), Gd.Arr(Gv.S("mpauto"), Gd.Arr(Gv.S("OHŘEV MP"), Gv.S("MAN./AUTO")), Gv.S("sel"), Gv.S("black")), Gd.Arr(Gv.S("mpheat"), Gd.Arr(Gv.S("OHŘEV MP")), Gv.S("sel"), Gv.S("green")), Gd.Arr(Gv.S("rez1"), Gd.Arr(Gv.S("ČERPADLO"), Gv.S("LEDOVÁ V.")), Gv.S("sel"), Gv.S("green"))), Gd.Arr(Gd.Arr(Gv.S("rake"), Gd.Arr(Gv.S("MÍCHADLO"), Gv.S("SK")), Gv.S("sel"), Gv.S("green")), Gd.Arr(Gv.S("rstB"), Gd.Arr(Gv.S("RESET"), Gv.S("PORUCHY"), Gv.S("SK")), Gv.S("push"), Gv.S("blue")), Gd.Arr(Gv.S("potsk"), Gd.Arr(Gv.S("RYCHLOST"), Gv.S("SK")), Gv.S("pot"), Gv.S("")), Gd.Arr(Gv.S("hyd"), Gd.Arr(Gv.S("HYDRAULIKA"), Gv.S("SK")), Gv.S("sel"), Gv.S("green")), Gd.Arr(Gv.S("hup"), Gd.Arr(Gv.S("NAHORU"), Gv.S("SK")), Gv.S("push"), Gv.S("green")), Gd.Arr(Gv.S("hdn"), Gd.Arr(Gv.S("DOLŮ"), Gv.S("SK")), Gv.S("push"), Gv.S("green"))), Gd.Arr(Gd.Arr(Gv.S("pboil"), Gd.Arr(Gv.S("BOILER"), Gv.S("ČERPADLO"), Gv.S("FMB")), Gv.S("sel"), Gv.S("green")), Gd.Arr(Gv.S("rstC"), Gd.Arr(Gv.S("RESET"), Gv.S("PORUCHY"), Gv.S("FMB")), Gv.S("push"), Gv.S("blue")), Gd.Arr(Gv.S("potp"), Gd.Arr(Gv.S("RYCHLOST"), Gv.S("FMB")), Gv.S("pot"), Gv.S("")), Gd.Arr(Gv.S("hltheat"), Gd.Arr(Gv.S("BOILER"), Gv.S("OHŘEV")), Gv.S("sel"), Gv.S("black")), Gd.Arr(Gv.S("pvarna"), Gd.Arr(Gv.S("VÁRNA"), Gv.S("ČERPADLO"), Gv.S("FMB")), Gv.S("sel"), Gv.S("black")), Gd.Arr(Gv.S("rez2"), Gd.Arr(Gv.S("ČERPADLO"), Gv.S("SPÍLACÍ")), Gv.S("sel"), Gv.S("black"))));
    DISP = Gd.Arr(Gd.Arr(Gv.S("mp"), Gv.S("TEPLOTA MP")), Gd.Arr(Gv.S("sk"), Gv.S("TEPLOTA SK")), Gd.Arr(Gv.S("ml"), Gv.S("TEPLOTA MLADINA")), Gd.Arr(Gv.S("bo"), Gv.S("TEPLOTA BOILER")));
    DISP_X = Gd.Arr(Gv.F(222.0), Gv.F(357.0), Gv.F(492.0), Gv.F(627.0));
    DISP_Y = Gv.F(178.0);
    FMS = Gd.Dict(Gv.S("p1"), Gd.Dict(Gv.S("min"), Gv.F(50.0), Gv.S("max"), Gv.F(400.0), Gv.S("step"), Gv.F(25.0)), Gv.S("p2"), Gd.Dict(Gv.S("min"), Gv.F(10.0), Gv.S("max"), Gv.F(200.0), Gv.S("step"), Gv.F(10.0)), Gv.S("p3"), Gd.Dict(Gv.S("min"), Gv.F(20.0), Gv.S("max"), Gv.F(150.0), Gv.S("step"), Gv.F(10.0)));
  }
  public S_cabinet() {
    m = Gv.Null;
    it = Gv.Null;
    sig = Gv.S("");
    sc = Gv.F(1.0);
    fault_extra = Gv.B(false);
    flash = Gd.Dict();
  }
  public Gv setup(Gv item = default(Gv)) {
    it = item;
    sc = Gd.minf((Gd.Dyn("float", it.C("get", Gv.S("w"), CAB_W)) / CAB_W), (Gd.Dyn("float", it.C("get", Gv.S("h"), CAB_H)) / CAB_H));
    return Gv.Null;
  }
  public Gv S() {
    return m.G("sim").G("S");
    return Gv.Null;
  }
  public Gv _live() {
    return m.G("sim").C("cab_live");
    return Gv.Null;
  }
  public Gv _sel_on(Gv k = default(Gv)) {
    Gv s = S();
    Gv c = s.G("cab");
    {
      Gv _t1 = k;
      if (Gv.Eq(_t1, Gv.S("lmp"))) {
        return c.C("get", Gv.S("lmp"), Gv.B(true));
      }
      else if (Gv.Eq(_t1, Gv.S("lsk"))) {
        return c.C("get", Gv.S("lsk"), Gv.B(true));
      }
      else if (Gv.Eq(_t1, Gv.S("stirmp"))) {
        return s.G("rvp").G("stir");
      }
      else if (Gv.Eq(_t1, Gv.S("mpauto"))) {
        return c.C("get", Gv.S("mpAuto"), Gv.B(false));
      }
      else if (Gv.Eq(_t1, Gv.S("mpheat"))) {
        return c.C("get", Gv.S("mpHeatOn"), Gv.B(false));
      }
      else if (Gv.Eq(_t1, Gv.S("rake"))) {
        return s.G("sk").G("rake");
      }
      else if (Gv.Eq(_t1, Gv.S("hyd"))) {
        return c.C("get", Gv.S("hydOn"), Gv.B(false));
      }
      else if (Gv.Eq(_t1, Gv.S("hltheat"))) {
        return s.G("hlt").G("auto");
      }
      else if (Gv.Eq(_t1, Gv.S("pboil"))) {
        return Gv.B(((s.G("rvpWater").G("on")).T() || (s.G("skWater").G("on")).T()));
      }
      else if (Gv.Eq(_t1, Gv.S("pvarna"))) {
        return s.G("p1").G("on");
      }
      else if (Gv.Eq(_t1, Gv.S("rez2"))) {
        return s.G("p3").G("on");
      }
      else if (Gv.Eq(_t1, Gv.S("rez1"))) {
        return s.G("lv").G("pump");
      }
      else if (Gv.Eq(_t1, Gv.S("bxs"))) {
        return s.G("p2").G("on");
      }
    }
    return Gv.B(false);
    return Gv.Null;
  }
  public Gv _pot_frac(Gv k = default(Gv)) {
    Gv s = S();
    {
      Gv _t1 = k;
      if (Gv.Eq(_t1, Gv.S("potmp"))) {
        return (Gd.clampf((((s.G("rvp").G("hz") - Gv.F(10.0))) / Gv.F(40.0)), Gv.F(0.0), Gv.F(1.0))).AsF();
      }
      else if (Gv.Eq(_t1, Gv.S("potsk"))) {
        return (Gd.clampf(s.G("cab").C("get", Gv.S("potsk"), Gv.F(0.5)), Gv.F(0.0), Gv.F(1.0))).AsF();
      }
      else if (Gv.Eq(_t1, Gv.S("potp"))) {
        return (_fm_frac(Gv.S("p1"))).AsF();
      }
      else if (Gv.Eq(_t1, Gv.S("bxp"))) {
        return (_fm_frac(Gv.S("p2"))).AsF();
      }
    }
    return (Gv.F(0.5)).AsF();
    return Gv.Null;
  }
  public Gv _st(Gv frac = default(Gv)) {
    frac = (frac).AsF();
    return (Gd.clampi(Gd.roundi((Gv.F(1.0) + (Gv.F(9.0) * frac))), Gv.I(1L), Gv.I(10L))).AsI();
    return Gv.Null;
  }
  public Gv _stf(Gv frac = default(Gv)) {
    frac = (frac).AsF();
    return ((Gv.S("%.1f") % ((Gv.F(1.0) + (Gv.F(9.0) * Gd.clampf(frac, Gv.F(0.0), Gv.F(1.0))))))).C("replace", Gv.S("."), Gv.S(","));
    return Gv.Null;
  }
  public Gv _frac_of(Gv st = default(Gv)) {
    st = (st).AsI();
    return ((((Gd.clampi(st, Gv.I(1L), Gv.I(10L)) - Gv.I(1L))) / Gv.F(9.0))).AsF();
    return Gv.Null;
  }
  public Gv pot_hit(Gv w = default(Gv)) {
    if (Gv.Eq(it, Gv.Null)) {
      return Gd.Dict();
    }
    Gv d = (((w - Gd.Ctor("Vector2", it.G("x"), it.G("y")))) / sc);
    foreach (Gv ri in (Gv.I(4L)).Iter()) {
      foreach (Gv ci in (Gv.I(6L)).Iter()) {
        Gv def = ROWS.I(ri).I(ci);
        if (Gv.Ne(def.I(Gv.I(2L)), Gv.S("pot"))) {
          continue;
        }
        Gv cc = Gd.Ctor("Vector2", COLS.I(ci), (ROW_Y.I(ri) + Gv.F(80.0)));
        if (Gv.Lt(d.C("distance_to", cc), Gv.F(40.0))) {
          return Gd.Dict(Gv.S("k"), def.I(Gv.I(0L)), Gv.S("c"), (Gd.Ctor("Vector2", it.G("x"), it.G("y")) + (cc * sc)));
        }
      }
    }
    return Gd.Dict();
    return Gv.Null;
  }
  public Gv pot_value(Gv k = default(Gv)) {
    return (_pot_frac(k)).AsF();
    return Gv.Null;
  }
  public Gv set_pot(Gv k = default(Gv), Gv f = default(Gv)) {
    f = (f).AsF();
    Gv s = S();
    f = (Gd.clampf(f, Gv.F(0.0), Gv.F(1.0))).AsF();
    {
      Gv _t1 = k;
      if (Gv.Eq(_t1, Gv.S("potmp"))) {
        s = s.S("rvp", s.G("rvp").S("hz", (Gv.F(10.0) + (Gv.F(40.0) * f))));
      }
      else if (Gv.Eq(_t1, Gv.S("potsk"))) {
        s = s.S("cab", s.G("cab").S("potsk", f));
      }
      else if (Gv.Eq(_t1, Gv.S("potp"))) {
        s = s.S("p1", s.G("p1").S("flow", Gd.roundf((FMS.G("p1").G("min") + (((FMS.G("p1").G("max") - FMS.G("p1").G("min"))) * f)))));
      }
      else if (Gv.Eq(_t1, Gv.S("bxp"))) {
        s = s.S("p2", s.G("p2").S("flow", Gd.roundf((FMS.G("p2").G("min") + (((FMS.G("p2").G("max") - FMS.G("p2").G("min"))) * f)))));
      }
    }
    Self.C("queue_redraw");
    return Gv.Null;
  }
  public Gv _fm_frac(Gv b = default(Gv)) {
    Gv F = FMS.I(b);
    return (Gd.clampf((((S().I(b).G("flow") - F.G("min"))) / ((F.G("max") - F.G("min")))), Gv.F(0.0), Gv.F(1.0))).AsF();
    return Gv.Null;
  }
  public Gv _status(Gv k = default(Gv)) {
    Gv s = S();
    {
      Gv _t1 = k;
      if (Gv.Eq(_t1, Gv.S("potmp"))) {
        return (Gv.S("%s · %d Hz") % Gd.Arr(_stf(_pot_frac(Gv.S("potmp"))), Gd.roundi(s.G("rvp").G("hz"))));
      }
      else if (Gv.Eq(_t1, Gv.S("potsk"))) {
        return (Gv.S("stupeň %s") % _stf(_pot_frac(Gv.S("potsk"))));
      }
      else if (Gv.Eq(_t1, Gv.S("potp"))) {
        return (Gv.S("%s · %d L/min") % Gd.Arr(_stf(_pot_frac(Gv.S("potp"))), Gd.roundi(s.G("p1").G("flow"))));
      }
      else if (Gv.Eq(_t1, Gv.S("bxp"))) {
        return (Gv.S("%s · %d L/min") % Gd.Arr(_stf(_pot_frac(Gv.S("bxp"))), Gd.roundi(s.G("p2").G("flow"))));
      }
      else if (Gv.Eq(_t1, Gv.S("stirmp"))) {
        return ((s.G("rvp").G("stir")).T() ? ((Gv.S("%d Hz") % Gd.roundi(s.G("rvp").G("hz")))) : Gv.S("stojí"));
      }
      else if (Gv.Eq(_t1, Gv.S("rake"))) {
        return ((s.G("sk").G("rake")).T() ? ((Gv.S("hloubka %d %%") % Gd.roundi((s.G("sk").G("rakeH") * Gv.F(100.0))))) : Gv.S("stojí"));
      }
      else if (Gv.Eq(_t1, Gv.S("hyd"))) {
        return ((s.G("cab").C("get", Gv.S("hydOn"), Gv.B(false))).T() ? Gv.S("zapnuta") : Gv.S("vypnuta"));
      }
      else if (Gv.Eq(_t1, Gv.S("pboil"))) {
        Gv w = s.G("rvpWater");
        return ((w.G("on")).T() ? ((Gv.S("%d/%d L") % Gd.Arr(Gd.roundi(w.G("added")), Gd.roundi(w.G("target"))))) : (((s.G("skWater").G("on")).T() ? Gv.S("vyslazuje") : Gv.S("stojí"))));
      }
      else if (Gv.Eq(_t1, Gv.S("pvarna"))) {
        return ((s.G("p1").G("on")).T() ? ((Gv.Ne(s.G("p1").G("status"), Gv.S("")) ? s.G("p1").G("status") : Gv.S("běží"))) : Gv.S("stojí"));
      }
      else if (Gv.Eq(_t1, Gv.S("rez2"))) {
        return ((s.G("p3").G("on")).T() ? ((Gv.Ne(s.G("p3").G("status"), Gv.S("")) ? s.G("p3").G("status") : Gv.S("běží"))) : Gv.S("stojí"));
      }
      else if (Gv.Eq(_t1, Gv.S("rez1"))) {
        return ((s.G("lv").G("pump")).T() ? ((Gv.Ge(s.G("lv").G("w"), Gv.I(300L)) ? Gv.S("čerpá") : Gv.S("ledovka prázdná"))) : Gv.S("stojí"));
      }
      else if (Gv.Eq(_t1, Gv.S("bxs"))) {
        return ((s.G("p2").G("on")).T() ? ((Gv.Ne(s.G("p2").G("status"), Gv.S("")) ? s.G("p2").G("status") : Gv.S("běží"))) : Gv.S("stojí"));
      }
      else if (Gv.Eq(_t1, Gv.S("mpauto"))) {
        return ((s.G("cab").C("get", Gv.S("mpAuto"), Gv.B(false))).T() ? Gv.S("AUTOMAT") : Gv.S("ručně"));
      }
      else if (Gv.Eq(_t1, Gv.S("mpheat"))) {
        return (Gv.Gt(s.G("rvp").G("heat"), Gv.I(0L)) ? ((Gv.S("pára %d %%") % Gd.roundi((s.G("rvp").G("heat") * Gv.F(100.0))))) : Gv.S("vyp"));
      }
      else if (Gv.Eq(_t1, Gv.S("hltheat"))) {
        return ((s.G("hlt").G("auto")).T() ? ((Gv.S("termostat %d °C") % Gd.roundi(s.G("hlt").G("set")))) : Gv.S("vyp"));
      }
    }
    return Gv.S("");
    return Gv.Null;
  }
  public Gv _disp_text(Gv k = default(Gv)) {
    Gv s = S();
    Gv sim = m.G("sim");
    if (!(s.G("cab").G("main")).T()) {
      return Gv.S("");
    }
    Gv T = Gv.Null;
    Gv V = Gv.F(0.0);
    {
      Gv _t1 = k;
      if (Gv.Eq(_t1, Gv.S("mp"))) {
        T = s.G("rvp").G("Tsens");
        V = sim.C("vol", s.G("rvp"));
      }
      else if (Gv.Eq(_t1, Gv.S("sk"))) {
        T = (((Gv.Le(s.G("sk").G("g"), Gv.F(1.0)) && Gv.Gt(sim.C("vol", s.G("sk").G("mash")), Gv.F(20.0))) ? s.G("sk").G("mash").G("T") : s.G("sk").G("T")));
        V = sim.C("skVolTotal");
      }
      else if (Gv.Eq(_t1, Gv.S("ml"))) {
        T = (Gv.Ne(s.G("cool").G("Tout"), Gv.Null) ? s.G("cool").G("Tout") : ((Gv.Gt(sim.C("vol", s.G("wp")), Gv.F(20.0)) ? s.G("wp").G("T") : Gv.Null)));
        V = (Gv.Ne(T, Gv.Null) ? Gv.F(100.0) : Gv.F(0.0));
      }
      else if (Gv.Eq(_t1, Gv.S("bo"))) {
        T = s.G("hlt").G("T");
        V = s.G("hlt").G("w");
      }
    }
    if ((Gv.Eq(T, Gv.Null) || Gv.Lt(V, Gv.F(20.0)))) {
      return Gv.S("---");
    }
    return (Gv.Ge(T, Gv.F(100.0)) ? ((Gv.S("%d") % Gd.roundi(T))) : ((Gv.S("%.1f") % T)).C("replace", Gv.S("."), Gv.S(",")));
    return Gv.Null;
  }
  public Gv _sig() {
    Gv s = S();
    Gv c = s.G("cab");
    Gv a = Gd.Arr(c.G("main"), c.G("key"), c.G("estop"), c.C("get", Gv.S("mpHeatOn"), Gv.B(false)), Gd.roundi((c.C("get", Gv.S("potsk"), Gv.F(0.5)) * Gv.F(90.0))), c.C("get", Gv.S("lmp"), Gv.B(true)), c.C("get", Gv.S("lsk"), Gv.B(true)), c.C("get", Gv.S("mpAuto"), Gv.B(false)), c.C("get", Gv.S("hydOn"), Gv.B(false)), c.C("get", Gv.S("fault"), Gv.B(false)), Gd.roundi((c.C("get", Gv.S("potsk"), Gv.F(0.5)) * Gv.F(10.0))), c.C("get", Gv.S("mpSet"), Gv.I(50L)), s.G("hlt").G("set"), s.G("hlt").G("auto"));
    a.C("append_array", Gd.Arr(s.G("rvp").G("stir"), Gd.roundi(s.G("rvp").G("hz")), Gd.roundi((s.G("rvp").G("heat") * Gv.F(100.0))), s.G("sk").G("rake"), Gd.roundi((s.G("sk").G("rakeH") * Gv.F(20.0))), s.G("p1").G("on"), s.G("p2").G("on"), s.G("p3").G("on"), s.G("lv").G("pump"), s.G("rvpWater").G("on"), s.G("skWater").G("on")));
    a.C("append_array", Gd.Arr(Gd.roundi(s.G("p1").G("flow")), Gd.roundi(s.G("p2").G("flow")), Gd.roundi(s.G("p3").G("flow")), s.G("p1").G("status"), s.G("p2").G("status"), s.G("p3").G("status"), Gd.roundi((s.G("rvpWater").G("added") / Gv.F(20.0))), Gv.B(Gv.Ge(s.G("lv").G("w"), Gv.I(300L)))));
    foreach (Gv d in (DISP).Iter()) {
      a.C("append", _disp_text(d.I(Gv.I(0L))));
    }
    foreach (Gv fk in (flash).Iter()) {
      if (Gv.Gt(_flashing(fk), Gv.F(0.0))) {
        a.C("append", (Gv.S("%s%d") % Gd.Arr(fk, Gd.Dyn("int", (Gd.Static("Time", "get_ticks_msec") / Gv.I(50L))))));
      }
    }
    return Gd.Dyn("str", a);
    return Gv.Null;
  }
  public Gv upd() {
    Gv g = _sig();
    if (Gv.Ne(g, sig)) {
      sig = g;
      Self.C("queue_redraw");
    }
    return Gv.Null;
  }
  public Gv _t(Gv text = default(Gv), Gv pos = default(Gv), Gv size = default(Gv), Gv col = default(Gv), Gv bold = default(Gv)) {
    size = (size).AsI();
    if (bold.k == 0) bold = Gv.B(true);
    S_art.text_c(Self, ((bold).T() ? m.G("font_b") : m.G("font")), text, pos, size, col);
    return Gv.Null;
  }
  public Gv _bezel(Gv c = default(Gv), Gv r = default(Gv)) {
    r = (r).AsF();
    Self.C("draw_circle", c, r, Gd.Ctor("Color", Gv.S("c3c7cc")));
    Self.C("draw_arc", c, r, Gv.I(0L), Gd.K("TAU"), Gv.I(28L), Gd.Ctor("Color", Gv.S("5b6066")), Gv.F(1.6), Gv.B(true));
    Self.C("draw_arc", c, (r - Gv.F(3.0)), Gv.I(0L), Gd.K("TAU"), Gv.I(28L), Gd.Ctor("Color", Gv.F(0.93), Gv.F(0.94), Gv.F(0.95), Gv.F(0.7)), Gv.F(1.0), Gv.B(true));
    return Gv.Null;
  }
  public Gv _sticker(Gv cx = default(Gv), Gv top = default(Gv), Gv lines = default(Gv), Gv sw = default(Gv)) {
    cx = (cx).AsF();
    top = (top).AsF();
    sw = (sw).AsF();
    Gv lh = Gv.F(10.8);
    Gv hh = (((lines.C("size") * lh) + Gv.F(8.0))).AsF();
    S_art.rrect_border(Self, Gd.Ctor("Rect2", (cx - (sw / Gv.F(2.0))), top, sw, hh), Gd.Ctor("Color", Gv.S("f3f3f0")), Gd.Ctor("Color", Gv.S("b9b9b3")), Gv.I(1L), Gv.F(2.0));
    foreach (Gv i in (lines.C("size")).Iter()) {
      Gv l = lines.I(i);
      _t(l, Gd.Ctor("Vector2", cx, ((top + Gv.F(10.0)) + (i * lh))), (Gv.Ge(l.C("length"), Gv.I(9L)) ? Gv.I(7L) : ((Gv.Ge(l.C("length"), Gv.I(7L)) ? Gv.I(8L) : Gv.I(9L)))), Gd.Ctor("Color", Gv.S("3a3d40")));
    }
    return Gv.Null;
  }
  public Gv _colors(Gv n = default(Gv)) {
    {
      Gv _t1 = n;
      if (Gv.Eq(_t1, Gv.S("green"))) {
        return Gd.Arr(Gd.Ctor("Color", Gv.S("1f6b43")), Gd.Ctor("Color", Gv.S("0f3b27")));
      }
      else if (Gv.Eq(_t1, Gv.S("blue"))) {
        return Gd.Arr(Gd.Ctor("Color", Gv.S("2a55b8")), Gd.Ctor("Color", Gv.S("17306c")));
      }
      else if (Gv.Eq(_t1, Gv.S("red"))) {
        return Gd.Arr(Gd.Ctor("Color", Gv.S("d8402e")), Gd.Ctor("Color", Gv.S("7a1d14")));
      }
    }
    return Gd.Arr(Gd.Ctor("Color", Gv.S("26292d")), Gd.Ctor("Color", Gv.S("111315")));
    return Gv.Null;
  }
  public Gv _lamp(Gv cx = default(Gv), Gv cy = default(Gv), Gv on = default(Gv), Gv kind = default(Gv)) {
    cx = (cx).AsF();
    cy = (cy).AsF();
    Gv c = Gd.Ctor("Vector2", cx, cy);
    _bezel(c, Gv.F(24.0));
    Gv off = (Gv.Eq(kind, Gv.S("white")) ? Gd.Ctor("Color", Gv.S("d9dcdf")) : ((Gv.Eq(kind, Gv.S("green")) ? Gd.Ctor("Color", Gv.S("1d4d36")) : Gd.Ctor("Color", Gv.S("5a1d1d")))));
    Gv onc = (Gv.Eq(kind, Gv.S("white")) ? Gd.Ctor("Color", Gv.S("ffffff")) : ((Gv.Eq(kind, Gv.S("green")) ? Gd.Ctor("Color", Gv.S("46ff86")) : Gd.Ctor("Color", Gv.S("ff4b3a")))));
    if ((on).T()) {
      _glow(c, onc, Gv.F(1.0));
    }
    Self.C("draw_circle", c, Gv.F(19.0), ((on).T() ? onc : off));
    Self.C("draw_arc", c, Gv.F(19.0), Gv.I(0L), Gd.K("TAU"), Gv.I(24L), Gd.Ctor("Color", Gv.S("1b1e21")), Gv.F(1.5), Gv.B(true));
    Self.C("draw_circle", (c + Gd.Ctor("Vector2", Gv.I(-6L), Gv.I(-7L))), Gv.F(5.0), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.3)));
    return Gv.Null;
  }
  public Gv _flashing(Gv k = default(Gv)) {
    Gv t = (flash.C("get", k, Gv.I(-100000L))).AsI();
    return (Gd.clampf((Gv.F(1.0) - (((Gd.Static("Time", "get_ticks_msec") - t)) / Gv.F(700.0))), Gv.F(0.0), Gv.F(1.0))).AsF();
    return Gv.Null;
  }
  public Gv _glow(Gv c = default(Gv), Gv col = default(Gv), Gv a = default(Gv)) {
    a = (a).AsF();
    foreach (Gv i in (Gv.I(4L)).Iter()) {
      Self.C("draw_circle", c, (Gv.F(40.0) - (i * Gv.F(4.0))), Gd.Ctor("Color", col, (((Gv.F(0.07) + (i * Gv.F(0.03)))) * a)));
    }
    return Gv.Null;
  }
  public Gv _push(Gv cx = default(Gv), Gv cy = default(Gv), Gv col = default(Gv), Gv pressed = default(Gv), Gv k = default(Gv)) {
    cx = (cx).AsF();
    cy = (cy).AsF();
    if (pressed.k == 0) pressed = Gv.B(false);
    if (k.k == 0) k = Gv.S("");
    Gv c = Gd.Ctor("Vector2", cx, cy);
    Gv cc = _colors(col);
    Gv fl = Gd.maxf(((pressed).T() ? Gv.F(1.0) : Gv.F(0.0)), _flashing(k));
    if (Gv.Gt(fl, Gv.F(0.0))) {
      _glow(c, cc.I(Gv.I(0L)).C("lightened", Gv.F(0.5)), fl);
      Self.C("draw_rect", Gd.Ctor("Rect2", (cx - Gv.I(28L)), (cy - Gv.I(80L)), Gv.I(56L), Gv.I(112L)), Gd.Ctor("Color", cc.I(Gv.I(0L)).C("lightened", Gv.F(0.5)), (Gv.F(0.9) * fl)), Gv.B(false), Gv.F(3.0));
    }
    _bezel(c, Gv.F(24.0));
    Self.C("draw_circle", c, Gv.F(18.5), cc.I(Gv.I(0L)).C("lightened", ((Gv.F(0.45) * fl) + (((pressed).T() ? Gv.F(0.25) : Gv.F(0.0))))));
    Self.C("draw_arc", c, Gv.F(18.5), Gv.I(0L), Gd.K("TAU"), Gv.I(24L), cc.I(Gv.I(1L)), Gv.F(2.0), Gv.B(true));
    Self.C("draw_circle", (c + Gd.Ctor("Vector2", Gv.I(-6L), Gv.I(-7L))), Gv.F(5.0), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.28)));
    return Gv.Null;
  }
  public Gv _sel(Gv cx = default(Gv), Gv cy = default(Gv), Gv col = default(Gv), Gv on = default(Gv), Gv k = default(Gv)) {
    cx = (cx).AsF();
    cy = (cy).AsF();
    if (k.k == 0) k = Gv.S("");
    Gv c = Gd.Ctor("Vector2", cx, cy);
    Gv cc = _colors(col);
    Gv fl = _flashing(k);
    if ((Gv.Gt(fl, Gv.F(0.0)) && !(on).T())) {
      _glow(c, Gd.Ctor("Color", Gv.S("ffb13b")), fl);
      Self.C("draw_rect", Gd.Ctor("Rect2", (cx - Gv.I(28L)), (cy - Gv.I(80L)), Gv.I(56L), Gv.I(112L)), Gd.Ctor("Color", Gv.F(1.0), Gv.F(0.7), Gv.F(0.25), (Gv.F(0.9) * fl)), Gv.B(false), Gv.F(3.0));
    }
    if ((on).T()) {
      foreach (Gv i in (Gv.I(4L)).Iter()) {
        Self.C("draw_circle", c, (Gv.F(40.0) - (i * Gv.F(4.0))), Gd.Ctor("Color", Gv.F(0.2), Gv.F(1.0), Gv.F(0.35), (Gv.F(0.07) + (i * Gv.F(0.03)))));
      }
      Self.C("draw_rect", Gd.Ctor("Rect2", (cx - Gv.I(28L)), (cy - Gv.I(80L)), Gv.I(56L), Gv.I(112L)), Gd.Ctor("Color", Gv.F(0.25), Gv.F(1.0), Gv.F(0.4), Gv.F(0.9)), Gv.B(false), Gv.F(3.0));
      cc = Gd.Arr(Gd.Ctor("Color", Gv.S("2fe063")), Gd.Ctor("Color", Gv.S("0f6b2a")));
    }
    _bezel(c, Gv.F(24.0));
    if ((on).T()) {
      Self.C("draw_arc", c, Gv.F(24.0), Gv.I(0L), Gd.K("TAU"), Gv.I(32L), Gd.Ctor("Color", Gv.S("4dff7c")), Gv.F(3.0), Gv.B(true));
    }
    Self.C("draw_circle", c, Gv.F(18.5), cc.I(Gv.I(0L)));
    Self.C("draw_arc", c, Gv.F(18.5), Gv.I(0L), Gd.K("TAU"), Gv.I(24L), cc.I(Gv.I(1L)), Gv.F(2.0), Gv.B(true));
    Gv ang = (Gd.deg_to_rad(((on).T() ? Gv.F(35.0) : Gv.F(-35.0)))).AsF();
    Gv dir = Gd.Ctor("Vector2", Gd.sin(ang), Gd.cos(ang));
    Self.C("draw_line", (c + (dir * Gv.F(3.0))), (c + (dir * Gv.F(27.0))), cc.I(Gv.I(1L)), Gv.F(9.0), Gv.B(true));
    Self.C("draw_line", (c + (dir * Gv.F(3.0))), (c + (dir * Gv.F(27.0))), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.18)), Gv.F(3.0), Gv.B(true));
    return Gv.Null;
  }
  public Gv _key(Gv cx = default(Gv), Gv cy = default(Gv), Gv on = default(Gv)) {
    cx = (cx).AsF();
    cy = (cy).AsF();
    Gv c = Gd.Ctor("Vector2", cx, cy);
    _bezel(c, Gv.F(24.0));
    Self.C("draw_circle", c, Gv.F(17.0), Gd.Ctor("Color", Gv.S("2b2e32")));
    Self.C("draw_arc", c, Gv.F(17.0), Gv.I(0L), Gd.K("TAU"), Gv.I(24L), Gd.Ctor("Color", Gv.S("0e0f11")), Gv.F(1.5), Gv.B(true));
    Gv ang = (Gd.deg_to_rad(((on).T() ? Gv.F(40.0) : Gv.F(-40.0)))).AsF();
    Gv d = Gd.Ctor("Vector2", Gd.sin(ang), Gd.cos(ang));
    Self.C("draw_line", (c - (d * Gv.F(8.0))), (c + (d * Gv.F(8.0))), Gd.Ctor("Color", Gv.S("0d0e10")), Gv.F(6.0), Gv.B(true));
    Self.C("draw_line", (c + (d * Gv.F(6.0))), (c + (d * Gv.F(42.0))), Gd.Ctor("Color", Gv.S("9aa0a6")), Gv.F(4.0), Gv.B(true));
    Self.C("draw_arc", (c + (d * Gv.F(44.0))), Gv.F(6.0), Gv.I(0L), Gd.K("TAU"), Gv.I(14L), Gd.Ctor("Color", Gv.S("9aa0a6")), Gv.F(2.0), Gv.B(true));
    return Gv.Null;
  }
  public Gv _pot(Gv cx = default(Gv), Gv cy = default(Gv), Gv frac = default(Gv)) {
    cx = (cx).AsF();
    cy = (cy).AsF();
    frac = (frac).AsF();
    Gv c = Gd.Ctor("Vector2", cx, cy);
    _bezel(c, Gv.F(27.0));
    foreach (Gv i in (Gv.I(10L)).Iter()) {
      Gv a = (Gd.deg_to_rad((Gv.F(-135.0) + (Gv.F(30.0) * i)))).AsF();
      Gv r1 = Gv.F(21.0);
      Gv r2 = Gv.F(26.0);
      Self.C("draw_line", (c + (Gd.Ctor("Vector2", Gd.sin(a), (-Gd.cos(a))) * r1)), (c + (Gd.Ctor("Vector2", Gd.sin(a), (-Gd.cos(a))) * r2)), Gd.Ctor("Color", Gv.S("2a2d30")), Gv.F(1.4));
      Gv tp = (c + (Gd.Ctor("Vector2", Gd.sin(a), (-Gd.cos(a))) * Gv.F(37.0)));
      Gv ts = Gd.Dyn("str", (i + Gv.I(1L)));
      Gv tw = (m.G("font_b").C("get_string_size", ts, Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(13L)).G("x")).AsF();
      Self.C("draw_string_outline", m.G("font_b"), (tp + Gd.Ctor("Vector2", ((-tw) * Gv.F(0.5)), Gv.I(5L))), ts, Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(13L), Gv.I(4L), Gd.Ctor("Color", Gv.S("0d0e10")));
      Self.C("draw_string", m.G("font_b"), (tp + Gd.Ctor("Vector2", ((-tw) * Gv.F(0.5)), Gv.I(5L))), ts, Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gv.I(13L), Gd.Ctor("Color", Gv.S("ffd95a")));
    }
    Self.C("draw_circle", c, Gv.F(16.0), Gd.Ctor("Color", Gv.S("1c1e21")));
    Self.C("draw_arc", c, Gv.F(16.0), Gv.I(0L), Gd.K("TAU"), Gv.I(20L), Gd.StaticGet("Color", "BLACK"), Gv.F(1.5), Gv.B(true));
    Gv ang = (Gd.deg_to_rad((Gv.F(-135.0) + (Gv.F(270.0) * frac)))).AsF();
    Gv dd = Gd.Ctor("Vector2", Gd.sin(ang), (-Gd.cos(ang)));
    Self.C("draw_line", (c + (dd * Gv.F(3.0))), (c + (dd * Gv.F(15.0))), Gd.StaticGet("Color", "WHITE"), Gv.F(2.4), Gv.B(true));
    _t(Gv.S("−"), (c + Gd.Ctor("Vector2", Gv.I(-22L), Gv.I(38L))), Gv.I(13L), Gd.Ctor("Color", Gv.S("6a7077")));
    _t(Gv.S("+"), (c + Gd.Ctor("Vector2", Gv.I(22L), Gv.I(38L))), Gv.I(13L), Gd.Ctor("Color", Gv.S("6a7077")));
    return Gv.Null;
  }
  public Gv _draw() {
    if (((Gv.Eq(it, Gv.Null) || Gv.Eq(m, Gv.Null)) || Gv.Eq(m.G("sim"), Gv.Null))) {
      return Gv.Null;
    }
    Gv s = S();
    Gv c = s.G("cab");
    Gv live = _live();
    Self.C("draw_set_transform", Gd.Ctor("Vector2", it.G("x"), it.G("y")), Gv.F(0.0), Gd.Ctor("Vector2", sc, sc));
    S_art.shadow(Self, Gd.Ctor("Rect2", (PX + Gv.F(4.0)), Gv.F(6.0), PW, CAB_H), Gd.Ctor("Vector2", Gv.I(14L), Gv.I(18L)), Gv.F(22.0));
    S_art.rrect_border(Self, Gd.Ctor("Rect2", PX, Gv.I(0L), PW, CAB_H), Gd.Ctor("Color", Gv.S("c6c9cd")), Gd.Ctor("Color", Gv.S("6b7076")), Gv.I(3L), Gv.F(6.0));
    foreach (Gv i in (Gv.I(26L)).Iter()) {
      Gv sx = ((((PX + Gv.F(8.0)) + (i * Gv.F(22.0))) + (((i * Gv.I(37L))) % Gv.I(11L)))).AsF();
      Self.C("draw_line", Gd.Ctor("Vector2", sx, Gv.I(6L)), Gd.Ctor("Vector2", sx, (CAB_H - Gv.I(6L))), (((i % Gv.I(3L))).T() ? Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.08)) : Gd.Ctor("Color", Gv.F(0.49), Gv.F(0.51), Gv.F(0.53), Gv.F(0.09))), (Gv.Eq((i % Gv.I(4L)), Gv.I(0L)) ? Gv.F(2.0) : Gv.F(1.0)));
    }
    foreach (Gv sp_ in (Gd.Arr(Gd.Arr((PX + Gv.F(14.0)), Gv.F(14.0)), Gd.Arr(((PX + PW) - Gv.F(14.0)), Gv.F(14.0)), Gd.Arr((PX + Gv.F(14.0)), (CAB_H - Gv.F(14.0))), Gd.Arr(((PX + PW) - Gv.F(14.0)), (CAB_H - Gv.F(14.0))))).Iter()) {
      Self.C("draw_circle", Gd.Ctor("Vector2", sp_.I(Gv.I(0L)), sp_.I(Gv.I(1L))), Gv.F(5.0), Gd.Ctor("Color", Gv.S("9ea3a8")));
      Self.C("draw_arc", Gd.Ctor("Vector2", sp_.I(Gv.I(0L)), sp_.I(Gv.I(1L))), Gv.F(5.0), Gv.I(0L), Gd.K("TAU"), Gv.I(12L), Gd.Ctor("Color", Gv.S("5b6066")), Gv.F(1.0));
    }
    S_art.rrect_border(Self, Gd.Ctor("Rect2", (PX + Gv.I(22L)), Gv.I(28L), Gv.I(150L), Gv.I(62L)), Gd.Ctor("Color", Gv.S("f6f6f3")), Gd.Ctor("Color", Gv.S("2a2d30")), Gv.I(2L), Gv.F(3.0));
    _t(Gv.S("RT.20"), Gd.Ctor("Vector2", (PX + Gv.I(97L)), Gv.I(56L)), Gv.I(30L), Gd.Ctor("Color", Gv.S("2a2d30")));
    _t(Gv.S("3/PEN AC 400/230V"), Gd.Ctor("Vector2", (PX + Gv.I(97L)), Gv.I(78L)), Gv.I(11L), Gd.Ctor("Color", Gv.S("2a2d30")));
    _t(Gd.Ctor("String", it.C("get", Gv.S("label"), Gv.S("ROZVADĚČ VARNA"))), Gd.Ctor("Vector2", ((PX + PW) - Gv.I(90L)), Gv.I(50L)), Gv.I(13L), Gd.Ctor("Color", Gv.S("3a3d40")));
    foreach (Gv i_2 in (Gv.I(4L)).Iter()) {
      Gv dx = (DISP_X.I(i_2)).AsF();
      Gv k = DISP.I(i_2).I(Gv.I(0L));
      S_art.rrect_border(Self, Gd.Ctor("Rect2", dx, (DISP_Y - Gv.I(34L)), Gv.I(122L), Gv.I(22L)), Gd.Ctor("Color", Gv.S("f3f3f0")), Gd.Ctor("Color", Gv.S("b9b9b3")), Gv.I(1L), Gv.F(2.0));
      _t(DISP.I(i_2).I(Gv.I(1L)), Gd.Ctor("Vector2", (dx + Gv.I(61L)), (DISP_Y - Gv.I(23L))), Gv.I(10L), Gd.Ctor("Color", Gv.S("2a2d30")));
      S_art.rrect_border(Self, Gd.Ctor("Rect2", dx, DISP_Y, Gv.I(122L), Gv.I(70L)), Gd.Ctor("Color", Gv.S("8a8e92")), Gd.Ctor("Color", Gv.S("3a3d40")), Gv.I(2L), Gv.F(5.0));
      Self.C("draw_rect", Gd.Ctor("Rect2", (dx + Gv.I(24L)), (DISP_Y + Gv.I(9L)), Gv.I(70L), Gv.I(40L)), Gd.Ctor("Color", Gv.S("0a0c0d")));
      Gv tx = ((c.G("main")).T() ? _disp_text(k) : Gv.S(""));
      Self.C("draw_string", m.G("font_b"), Gd.Ctor("Vector2", (dx + Gv.I(26L)), (DISP_Y + Gv.I(43L))), tx, Gd.K("HORIZONTAL_ALIGNMENT_RIGHT"), Gv.I(66L), Gv.I(27L), Gd.Ctor("Color", Gv.S("ff4a38")));
      Self.C("draw_circle", Gd.Ctor("Vector2", (dx + Gv.I(12L)), (DISP_Y + Gv.I(18L))), Gv.F(5.0), Gd.Ctor("Color", Gv.S("6e7276")));
      Self.C("draw_circle", Gd.Ctor("Vector2", (dx + Gv.I(12L)), (DISP_Y + Gv.I(40L))), Gv.F(5.0), Gd.Ctor("Color", Gv.S("6e7276")));
      foreach (Gv ar in (Gd.Arr(Gd.Arr(Gv.F(8.0), Gv.B(true)), Gd.Arr(Gv.F(28.0), Gv.B(false)))).Iter()) {
        S_art.rrect_border(Self, Gd.Ctor("Rect2", (dx + Gv.I(100L)), (DISP_Y + ar.I(Gv.I(0L))), Gv.I(18L), Gv.I(16L)), Gd.Ctor("Color", Gv.S("6e7276")), Gd.Ctor("Color", Gv.S("3a3d40")), Gv.I(1L), Gv.F(3.0));
        Gv cxa = ((dx + Gv.I(109L))).AsF();
        Gv cya = (((DISP_Y + ar.I(Gv.I(0L))) + Gv.F(8.0))).AsF();
        if ((ar.I(Gv.I(1L))).T()) {
          Self.C("draw_colored_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", cxa, (cya - Gv.I(5L))), Gd.Ctor("Vector2", (cxa + Gv.I(5L)), (cya + Gv.I(5L))), Gd.Ctor("Vector2", (cxa - Gv.I(5L)), (cya + Gv.I(5L))))), Gd.Ctor("Color", Gv.S("d6d9dc")));
        } else {
          Self.C("draw_colored_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", cxa, (cya + Gv.I(5L))), Gd.Ctor("Vector2", (cxa + Gv.I(5L)), (cya - Gv.I(5L))), Gd.Ctor("Vector2", (cxa - Gv.I(5L)), (cya - Gv.I(5L))))), Gd.Ctor("Color", Gv.S("d6d9dc")));
        }
      }
      _t(Gv.S("dixell"), Gd.Ctor("Vector2", (dx + Gv.I(67L)), (DISP_Y + Gv.I(65L))), Gv.I(8L), Gd.Ctor("Color", Gv.S("d6d9dc")));
      if ((Gv.Eq(k, Gv.S("mp")) && (c.G("main")).T())) {
        _t((Gv.S("SP %d °C") % Gd.roundi(c.C("get", Gv.S("mpSet"), Gv.I(50L)))), Gd.Ctor("Vector2", (dx + Gv.I(61L)), (DISP_Y + Gv.I(84L))), Gv.I(10L), Gd.Ctor("Color", Gv.S("7a5a00")));
      }
      if ((Gv.Eq(k, Gv.S("bo")) && (c.G("main")).T())) {
        _t((Gv.S("SP %d °C") % Gd.roundi(s.G("hlt").G("set"))), Gd.Ctor("Vector2", (dx + Gv.I(61L)), (DISP_Y + Gv.I(84L))), Gv.I(10L), Gd.Ctor("Color", Gv.S("7a5a00")));
      }
    }
    Gv fault = c.C("get", Gv.S("fault"), Gv.B(false));
    Gv pots_later = Gd.Arr();
    foreach (Gv ri in (Gv.I(4L)).Iter()) {
      Gv row = ROWS.I(ri);
      foreach (Gv ci in (Gv.I(6L)).Iter()) {
        Gv def = row.I(ci);
        Gv k_2 = def.I(Gv.I(0L));
        Gv ty = def.I(Gv.I(2L));
        if (Gv.Eq(ty, Gv.S("blank"))) {
          continue;
        }
        Gv X = (COLS.I(ci)).AsF();
        Gv top = (ROW_Y.I(ri)).AsF();
        Gv cy = (top + Gv.F(80.0));
        Self.C("draw_rect", Gd.Ctor("Rect2", (X - Gv.I(28L)), top, Gv.I(56L), Gv.I(112L)), Gd.Ctor("Color", Gv.S("14171a")));
        _sticker(X, (top + Gv.F(5.0)), def.I(Gv.I(1L)), Gv.F(50.0));
        {
          Gv _t1 = ty;
          if (Gv.Eq(_t1, Gv.S("lamp"))) {
            Gv on = Gv.B(false);
            {
              Gv _t2 = k_2;
              if (Gv.Eq(_t2, Gv.S("net"))) {
                on = c.G("main");
              }
              else if (Gv.Eq(_t2, Gv.S("run"))) {
                on = live;
              }
              else if (Gv.Eq(_t2, Gv.S("flt"))) {
                on = Gv.B(((c.G("estop")).T() || (fault).T()));
              }
            }
            _lamp(X, cy, on, def.I(Gv.I(3L)));
          }
          else if (Gv.Eq(_t1, Gv.S("key"))) {
            _key(X, cy, c.G("key"));
          }
          else if (Gv.Eq(_t1, Gv.S("push"))) {
            _push(X, cy, def.I(Gv.I(3L)), Gv.B(((Gv.Eq(k_2, Gv.S("hup")) && Gv.Lt(s.G("sk").G("hyd"), Gv.F(0.0))) || (Gv.Eq(k_2, Gv.S("hdn")) && Gv.Gt(s.G("sk").G("hyd"), Gv.F(0.0))))), k_2);
          }
          else if (Gv.Eq(_t1, Gv.S("sel"))) {
            _sel(X, cy, def.I(Gv.I(3L)), _sel_on(k_2), k_2);
          }
          else if (Gv.Eq(_t1, Gv.S("pot"))) {
            pots_later.C("append", Gd.Arr(X, cy, _pot_frac(k_2)));
          }
        }
        if ((Gv.In(ty, Gd.Arr(Gv.S("sel"), Gv.S("pot"))) || Gv.In(k_2, Gd.Arr(Gv.S("hup"), Gv.S("hdn"))))) {
          Gv st = _status(k_2);
          if (Gv.Ne(st, Gv.S(""))) {
            _t(st, Gd.Ctor("Vector2", X, (top + Gv.F(128.0))), Gv.I(10L), Gd.Ctor("Color", Gv.S("1d2126")));
          }
        }
      }
    }
    foreach (Gv pl in (pots_later).Iter()) {
      _pot(pl.I(Gv.I(0L)), pl.I(Gv.I(1L)), pl.I(Gv.I(2L)));
    }
    S_art.rrect_border(Self, Gd.Ctor("Rect2", (PX + Gv.I(22L)), Gv.I(890L), Gv.I(104L), Gv.I(20L)), Gd.Ctor("Color", Gv.S("f2c230")), Gd.Ctor("Color", Gv.S("8a6a00")), Gv.I(1L), Gv.F(2.0));
    _t(Gv.S("HLAVNÍ VYPÍNAČ"), Gd.Ctor("Vector2", (PX + Gv.I(74L)), Gv.I(900L)), Gv.I(10L), Gd.Ctor("Color", Gv.S("2a2400")));
    S_art.rrect_border(Self, Gd.Ctor("Rect2", (PX + Gv.I(30L)), Gv.I(916L), Gv.I(88L), Gv.I(88L)), Gd.Ctor("Color", Gv.S("18191b")), Gd.StaticGet("Color", "BLACK"), Gv.I(2L), Gv.F(6.0));
    Gv mc = Gd.Ctor("Vector2", (PX + Gv.I(74L)), Gv.I(960L));
    Self.C("draw_circle", mc, Gv.F(34.0), Gd.Ctor("Color", Gv.S("232527")));
    Self.C("draw_arc", mc, Gv.F(34.0), Gv.I(0L), Gd.K("TAU"), Gv.I(28L), Gd.StaticGet("Color", "BLACK"), Gv.F(2.0), Gv.B(true));
    Self.C("draw_circle", mc, Gv.F(27.0), Gd.Ctor("Color", Gv.S("2c2f32")));
    Gv mang = (Gd.deg_to_rad(((c.G("main")).T() ? Gv.F(90.0) : Gv.F(0.0)))).AsF();
    Gv md = Gd.Ctor("Vector2", (-Gd.sin(mang)), (-Gd.cos(mang)));
    Self.C("draw_line", (mc + (md * Gv.F(8.0))), (mc + (md * Gv.F(28.0))), Gd.Ctor("Color", Gv.S("f1f1ef")), Gv.F(6.0), Gv.B(true));
    _t(((c.G("main")).T() ? Gv.S("ZAP") : Gv.S("VYP")), Gd.Ctor("Vector2", (PX + Gv.I(74L)), Gv.I(1022L)), Gv.I(9L), ((c.G("main")).T() ? Gd.Ctor("Color", Gv.S("2fbf5b")) : Gd.Ctor("Color", Gv.S("a33a2f"))));
    S_art.rrect_border(Self, Gd.Ctor("Rect2", (PX + Gv.I(150L)), Gv.I(900L), Gv.I(128L), Gv.I(124L)), Gd.Ctor("Color", Gv.S("f3f3f0")), Gd.Ctor("Color", Gv.S("b9b9b3")), Gv.I(1L), Gv.F(4.0));
    S_art.rrect(Self, Gd.Ctor("Rect2", (PX + Gv.I(150L)), Gv.I(900L), Gv.I(128L), Gv.I(26L)), Gd.Ctor("Color", Gv.S("d7332a")), Gv.F(4.0));
    _t(Gv.S("VÝSTRAHA"), Gd.Ctor("Vector2", (PX + Gv.I(214L)), Gv.I(913L)), Gv.I(12L), Gd.StaticGet("Color", "WHITE"));
    foreach (Gv i_3 in (Gv.I(4L)).Iter()) {
      Self.C("draw_rect", Gd.Ctor("Rect2", (PX + Gv.I(158L)), (Gv.I(934L) + (i_3 * Gv.I(21L))), Gv.I(112L), Gv.I(3L)), Gd.Ctor("Color", Gv.S("8c8f93")));
      Self.C("draw_rect", Gd.Ctor("Rect2", (PX + Gv.I(158L)), (Gv.I(941L) + (i_3 * Gv.I(21L))), Gv.I(88L), Gv.I(3L)), Gd.Ctor("Color", Gv.S("b5b8bb")));
    }
    S_art.rrect_border(Self, Gd.Ctor("Rect2", (PX + Gv.I(290L)), Gv.I(888L), Gv.I(150L), Gv.I(22L)), Gd.Ctor("Color", Gv.S("f6f6f3")), Gd.Ctor("Color", Gv.S("b9b9b3")), Gv.I(1L), Gv.F(2.0));
    _t(Gv.S("NOUZOVÉ VYPNUTÍ"), Gd.Ctor("Vector2", (PX + Gv.I(365L)), Gv.I(899L)), Gv.I(11L), Gd.Ctor("Color", Gv.S("c1281d")));
    Gv ec = Gd.Ctor("Vector2", (PX + Gv.I(365L)), Gv.I(972L));
    Self.C("draw_circle", ec, Gv.F(52.0), Gd.Ctor("Color", Gv.S("f0b323")));
    Self.C("draw_arc", ec, Gv.F(52.0), Gv.I(0L), Gd.K("TAU"), Gv.I(32L), Gd.Ctor("Color", Gv.S("8a6200")), Gv.F(2.0), Gv.B(true));
    Self.C("draw_circle", ec, Gv.F(38.0), Gd.Ctor("Color", Gv.S("e09a14")));
    Self.C("draw_circle", ec, (!(c.G("estop")).T() ? Gv.F(30.0) : Gv.F(26.0)), (!(c.G("estop")).T() ? Gd.Ctor("Color", Gv.S("e03021")) : Gd.Ctor("Color", Gv.S("8f2016"))));
    Self.C("draw_arc", ec, Gv.F(30.0), Gv.I(0L), Gd.K("TAU"), Gv.I(28L), Gd.Ctor("Color", Gv.S("7a130b")), Gv.F(3.0), Gv.B(true));
    if (!(c.G("estop")).T()) {
      Self.C("draw_circle", (ec + Gd.Ctor("Vector2", Gv.I(-9L), Gv.I(-9L))), Gv.F(8.0), Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.28)));
    }
    _t(Gv.S("EMERGENCY STOP"), (ec + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(-34L))), Gv.I(7L), Gd.Ctor("Color", Gv.S("7a5200")));
    S_art.rrect_border(Self, Gd.Ctor("Rect2", (PX + Gv.I(456L)), Gv.I(900L), Gv.I(104L), Gv.I(124L)), Gd.Ctor("Color", Gv.S("f3f3f0")), Gd.Ctor("Color", Gv.S("b9b9b3")), Gv.I(1L), Gv.F(4.0));
    S_art.rrect(Self, Gd.Ctor("Rect2", (PX + Gv.I(456L)), Gv.I(900L), Gv.I(104L), Gv.I(34L)), Gd.Ctor("Color", Gv.S("1f5e3b")), Gv.F(4.0));
    _t(Gv.S("BEZPEČNOSTNÍ"), Gd.Ctor("Vector2", (PX + Gv.I(508L)), Gv.I(910L)), Gv.I(9L), Gd.StaticGet("Color", "WHITE"));
    _t(Gv.S("INSTRUKCE"), Gd.Ctor("Vector2", (PX + Gv.I(508L)), Gv.I(923L)), Gv.I(9L), Gd.StaticGet("Color", "WHITE"));
    foreach (Gv i_4 in (Gv.I(3L)).Iter()) {
      Self.C("draw_rect", Gd.Ctor("Rect2", (PX + Gv.I(464L)), (Gv.I(946L) + (i_4 * Gv.I(26L))), Gv.I(88L), Gv.I(3L)), Gd.Ctor("Color", Gv.S("8c8f93")));
      Self.C("draw_rect", Gd.Ctor("Rect2", (PX + Gv.I(464L)), (Gv.I(953L) + (i_4 * Gv.I(26L))), Gv.I(70L), Gv.I(3L)), Gd.Ctor("Color", Gv.S("b5b8bb")));
    }
    S_art.rrect_border(Self, Gd.Ctor("Rect2", (PX + Gv.I(300L)), Gv.I(1040L), Gv.I(132L), Gv.I(76L)), Gd.Ctor("Color", Gv.S("f6f6f3")), Gd.Ctor("Color", Gv.S("b9b9b3")), Gv.I(1L), Gv.F(3.0));
    Self.C("draw_colored_polygon", Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", (PX + Gv.I(366L)), Gv.I(1046L)), Gd.Ctor("Vector2", (PX + Gv.I(394L)), Gv.I(1092L)), Gd.Ctor("Vector2", (PX + Gv.I(338L)), Gv.I(1092L)))), Gd.Ctor("Color", Gv.S("f4d21e")));
    Self.C("draw_rect", Gd.Ctor("Rect2", (PX + Gv.I(300L)), Gv.I(1096L), Gv.I(132L), Gv.I(20L)), Gd.Ctor("Color", Gv.S("f4d21e")));
    _t(Gv.S("POZOR ELEKTRICKÉ ZAŘÍZENÍ"), Gd.Ctor("Vector2", (PX + Gv.I(366L)), Gv.I(1106L)), Gv.I(7L), Gd.Ctor("Color", Gv.S("1a1a1a")));
    Self.C("draw_set_transform", Gd.StaticGet("Vector2", "ZERO"), Gv.F(0.0), Gd.StaticGet("Vector2", "ONE"));
    return Gv.Null;
  }
  public Gv hit(Gv w = default(Gv)) {
    if (Gv.Eq(it, Gv.Null)) {
      return Gv.S("");
    }
    Gv d = (((w - Gd.Ctor("Vector2", it.G("x"), it.G("y")))) / sc);
    if ((((Gv.Lt(d.G("x"), Gv.F(0.0)) || Gv.Lt(d.G("y"), Gv.F(0.0))) || Gv.Gt(d.G("x"), CAB_W)) || Gv.Gt(d.G("y"), CAB_H))) {
      return Gv.S("");
    }
    foreach (Gv ri in (Gv.I(4L)).Iter()) {
      foreach (Gv ci in (Gv.I(6L)).Iter()) {
        Gv def = ROWS.I(ri).I(ci);
        Gv ty = def.I(Gv.I(2L));
        Gv cc = Gd.Ctor("Vector2", COLS.I(ci), (ROW_Y.I(ri) + Gv.F(80.0)));
        if (Gv.Eq(ty, Gv.S("pot"))) {
          if ((Gd.Ctor("Rect2", (cc.G("x") - Gv.I(30L)), (cc.G("y") - Gv.I(30L)), Gv.I(60L), Gv.I(60L)).C("has_point", d)).T()) {
            return (def.I(Gv.I(0L)) + ((Gv.Lt(d.G("x"), cc.G("x")) ? Gv.S("_d") : Gv.S("_u"))));
          }
        } else if (((Gv.Ne(ty, Gv.S("lamp")) && Gv.Ne(ty, Gv.S("blank"))) && Gv.Lt(d.C("distance_to", cc), Gv.F(27.0)))) {
          return def.I(Gv.I(0L));
        }
      }
    }
    foreach (Gv i in (Gd.Arr(Gv.I(0L), Gv.I(3L))).Iter()) {
      Gv dx = (DISP_X.I(i)).AsF();
      if ((Gd.Ctor("Rect2", (dx + Gv.I(100L)), (DISP_Y + Gv.I(8L)), Gv.I(18L), Gv.I(16L)).C("has_point", d)).T()) {
        return (((Gv.Eq(i, Gv.I(0L)) ? Gv.S("spmp") : Gv.S("spbo"))) + Gv.S("_u"));
      }
      if ((Gd.Ctor("Rect2", (dx + Gv.I(100L)), (DISP_Y + Gv.I(28L)), Gv.I(18L), Gv.I(16L)).C("has_point", d)).T()) {
        return (((Gv.Eq(i, Gv.I(0L)) ? Gv.S("spmp") : Gv.S("spbo"))) + Gv.S("_d"));
      }
    }
    if ((Gd.Ctor("Rect2", (PX + Gv.I(30L)), Gv.I(916L), Gv.I(88L), Gv.I(88L)).C("has_point", d)).T()) {
      return Gv.S("main");
    }
    if (Gv.Lt(d.C("distance_to", Gd.Ctor("Vector2", (PX + Gv.I(365L)), Gv.I(972L))), Gv.F(52.0))) {
      return Gv.S("estop");
    }
    return Gv.S("");
    return Gv.Null;
  }
  public Gv _dead() {
    Gv c = S().G("cab");
    if (!(c.G("main")).T()) {
      m.C("_say", Gv.S("Rozvaděč je bez napětí – zapni HLAVNÍ VYPÍNAČ."));
    } else if ((c.G("estop")).T()) {
      m.C("_say", Gv.S("Nouzové vypnutí je stisknuté – klepni na hřib – odblokuje se."));
    } else {
      m.C("_say", Gv.S("Rozvaděč neběží – otoč klíč START."));
    }
    return Gv.Null;
  }
  public Gv _log(Gv t = default(Gv), Gv ty = default(Gv)) {
    if (ty.k == 0) ty = Gv.S("info");
    m.G("sim").C("logm", t, ty);
    return Gv.Null;
  }
  public Gv _bind_step(Gv b = default(Gv), Gv dir = default(Gv)) {
    dir = (dir).AsI();
    Gv s = S();
    if ((FMS.C("has", b)).T()) {
      Gv F = FMS.I(b);
      Gv st = (_st(_fm_frac(b)) + dir);
      s = s.SI(b, s.I(b).S("flow", Gd.roundf((F.G("min") + (((F.G("max") - F.G("min"))) * _frac_of(st))))));
    }
    return Gv.Null;
  }
  public Gv _stop_shock(Gv which = default(Gv)) {
    Gv s = S();
    Gv tgt = (Gv.Eq(which, Gv.S("mp")) ? s.G("rvp") : s.G("sk"));
    Gv sev = (Gv.F(0.0)).AsF();
    if (Gv.Eq(which, Gv.S("mp"))) {
      Gv load = (Gv.F(0.2)).AsF();
      if (Gv.Gt(s.G("rvp").G("w"), Gv.I(200L))) {
        load = (Gd.clampf((Gv.F(0.4) + (Gv.F(0.6) * m.G("sim").C("thick_factor", s.G("rvp")))), Gv.F(0.4), Gv.F(1.0))).AsF();
      }
      sev = ((Gd.clampf((((s.G("rvp").G("hz") - Gv.F(15.0))) / Gv.F(35.0)), Gv.F(0.0), Gv.F(1.0)) * load)).AsF();
    } else {
      Gv pf = (s.G("cab").C("get", Gv.S("potsk"), Gv.F(0.5))).AsF();
      sev = ((Gd.clampf((((pf - Gv.F(0.25))) / Gv.F(0.75)), Gv.F(0.0), Gv.F(1.0)) * Gd.clampf((s.G("sk").G("rakeH") * Gv.F(1.2)), Gv.F(0.0), Gv.F(1.0)))).AsF();
    }
    if (Gv.Lt(sev, Gv.F(0.08))) {
      return Gv.Null;
    }
    tgt = tgt.SI(Gv.S("wear"), (tgt.C("get", Gv.S("wear"), Gv.F(0.0)) + (sev * Gv.F(0.5))));
    Gv what = (Gv.Eq(which, Gv.S("mp")) ? Gv.S("míchadla MP") : Gv.S("kopačky SK"));
    if ((Gv.Gt(sev, Gv.F(0.55)) || Gv.Ge(tgt.G("wear"), Gv.F(1.0)))) {
      tgt = tgt.SI(Gv.S("broken"), Gv.B(true));
      _log((Gv.S("RÁZ! Vypnutí %s za plných otáček – hřídel se prudce zastavila, ulomila se spojka/lopatka. Nejdřív sniž otáčky, pak vypínej. Oprava: RESET PORUCHY.") % what), Gv.S("bad"));
      if (Gv.Ne(m.G("hud"), Gv.Null)) {
        m.G("hud").C("toast", (Gv.S("RÁZ – poškození %s!") % what));
      }
    } else if (Gv.Gt(sev, Gv.F(0.2))) {
      _log((Gv.S("Silný ráz při vypnutí %s (otáčky byly vysoké) – namáhá spojku a převodovku. Příště nejdřív sniž otáčky.") % what), Gv.S("warn"));
    } else {
      _log((Gv.S("Lehký ráz při vypnutí %s – radši nejdřív sniž otáčky.") % what), Gv.S("warn"));
    }
    return Gv.Null;
  }
  public Gv act(Gv k = default(Gv), Gv down = default(Gv)) {
    Gv s = S();
    Gv c = s.G("cab");
    Gv sk = s.G("sk");
    Gv rv = s.G("rvp");
    if ((down).T()) {
      flash = flash.SI(k.C("trim_suffix", Gv.S("_u")).C("trim_suffix", Gv.S("_d")), Gd.Static("Time", "get_ticks_msec"));
    }
    if ((Gv.Eq(k, Gv.S("hup")) || Gv.Eq(k, Gv.S("hdn")))) {
      if (!(down).T()) {
        sk = sk.S("hyd", Gv.F(0.0));
        return Gv.Null;
      }
      if (!(_live()).T()) {
        _dead();
        return Gv.Null;
      }
      if (!(c.C("get", Gv.S("hydOn"), Gv.B(false))).T()) {
        m.C("_say", Gv.S("Hydraulika SK je vypnutá – zapni volič HYDRAULIKA SK."));
        return Gv.Null;
      }
      sk = sk.S("hyd", (Gv.Eq(k, Gv.S("hup")) ? Gv.F(-1.0) : Gv.F(1.0)));
      return Gv.Null;
    }
    if (!(down).T()) {
      return Gv.Null;
    }
    if (Gv.Eq(k, Gv.S("main"))) {
      c = c.S("main", Gv.B(!(c.G("main")).T()));
      _log(((c.G("main")).T() ? Gv.S("Hlavní vypínač zapnut – rozvaděč pod napětím.") : Gv.S("Hlavní vypínač vypnut – rozvaděč bez napětí, všechno elektrické stojí.")), ((c.G("main")).T() ? Gv.S("info") : Gv.S("bad")));
      m.C("redraw");
      return Gv.Null;
    }
    if (Gv.Eq(k, Gv.S("key"))) {
      if (!(c.G("main")).T()) {
        m.C("_say", Gv.S("Nejdřív zapni HLAVNÍ VYPÍNAČ."));
        return Gv.Null;
      }
      if ((c.G("estop")).T()) {
        m.C("_say", Gv.S("Nouzové vypnutí je stisknuté – nejdřív ho odblokuj (klepni na hřib)."));
        return Gv.Null;
      }
      c = c.S("key", Gv.B(!(c.G("key")).T()));
      _log(((c.G("key")).T() ? Gv.S("START: rozvaděč v provozu.") : Gv.S("Klíč START vypnut – rozvaděč v klidu, všechno elektrické stojí.")), ((c.G("key")).T() ? Gv.S("info") : Gv.S("warn")));
      m.C("redraw");
      return Gv.Null;
    }
    if (Gv.Eq(k, Gv.S("estop"))) {
      c = c.S("estop", Gv.B(!(c.G("estop")).T()));
      if ((c.G("estop")).T()) {
        c = c.S("key", Gv.B(false));
        _log(Gv.S("NOUZOVÉ VYPNUTÍ! Všechno elektrické vypnuto. Odblokuj hřib (klepni na něj)."), Gv.S("bad"));
      } else {
        c = c.S("key", Gv.B(true));
        _log(Gv.S("Nouzové vypnutí odblokováno – rozvaděč zase v provozu."), Gv.S("info"));
      }
      m.C("redraw");
      return Gv.Null;
    }
    if (!(_live()).T()) {
      _dead();
      return Gv.Null;
    }
    if (!(m.G("running")).T()) {
      m.C("_say", Gv.S("Čas stojí – nahoře stiskni SPUSTIT, ať se to rozběhne."));
    }
    {
      Gv _t1 = k;
      if (Gv.Eq(_t1, Gv.S("lmp"))) {
        c = c.S("lmp", Gv.B(!(c.C("get", Gv.S("lmp"), Gv.B(true))).T()));
        _log((Gv.S("Osvětlení MP %s.") % (((c.G("lmp")).T() ? Gv.S("zapnuto") : Gv.S("vypnuto")))));
      }
      else if (Gv.Eq(_t1, Gv.S("lsk"))) {
        c = c.S("lsk", Gv.B(!(c.C("get", Gv.S("lsk"), Gv.B(true))).T()));
        _log((Gv.S("Osvětlení SK %s.") % (((c.G("lsk")).T() ? Gv.S("zapnuto") : Gv.S("vypnuto")))));
      }
      else if (Gv.Eq(_t1, Gv.S("stirmp"))) {
        if ((!(rv.G("stir")).T() && (rv.C("get", Gv.S("broken"), Gv.B(false))).T())) {
          _log(Gv.S("Míchadlo MP je poškozené po rázu – nejdřív oprava (RESET PORUCHY FMB = výměna spojky/lopatky)."), Gv.S("bad"));
          return Gv.Null;
        }
        rv = rv.S("stir", Gv.B(!(rv.G("stir")).T()));
        _log((Gv.S("Míchadlo MP %s.") % (((rv.G("stir")).T() ? ((Gv.S("zapnuto (%d Hz)") % Gd.roundi(rv.G("hz")))) : Gv.S("vypnuto")))));
        if (!(rv.G("stir")).T()) {
          _stop_shock(Gv.S("mp"));
        }
      }
      else if (Gv.Eq(_t1, Gv.S("potmp_u")) || Gv.Eq(_t1, Gv.S("potmp_d"))) {
        rv = rv.S("hz", Gd.roundf((Gv.F(10.0) + (Gv.F(40.0) * _frac_of((_st(_pot_frac(Gv.S("potmp"))) + ((Gv.Eq(k, Gv.S("potmp_u")) ? Gv.I(1L) : Gv.I(-1L)))))))));
      }
      else if (Gv.Eq(_t1, Gv.S("mpauto"))) {
        c = c.S("mpAuto", Gv.B(!(c.C("get", Gv.S("mpAuto"), Gv.B(false))).T()));
        _log(((c.G("mpAuto")).T() ? ((Gv.S("Ohřev MP: AUTOMAT – drží %d °C (nastavíš šipkami na displeji TEPLOTA MP).") % Gd.roundi(c.C("get", Gv.S("mpSet"), Gv.I(50L))))) : Gv.S("Ohřev MP: RUČNĚ – pára podle nastavení ohřevu vpravo.")));
      }
      else if (Gv.Eq(_t1, Gv.S("mpheat"))) {
        c = c.S("mpHeatOn", Gv.B(!(c.C("get", Gv.S("mpHeatOn"), Gv.B(false))).T()));
        if ((c.G("mpHeatOn")).T()) {
          if ((!(c.C("get", Gv.S("mpAuto"), Gv.B(false))).T() && Gv.Le(rv.G("heat"), Gv.F(0.0)))) {
            rv = rv.S("heat", c.C("get", Gv.S("heatMem"), Gv.F(1.0)));
          }
        } else {
          if (Gv.Gt(rv.G("heat"), Gv.F(0.0))) {
            c = c.SI(Gv.S("heatMem"), rv.G("heat"));
          }
          rv = rv.S("heat", Gv.F(0.0));
        }
        _log((Gv.S("Ohřev MP %s.") % (((c.G("mpHeatOn")).T() ? Gv.S("zapnut") : Gv.S("vypnut")))));
        if (((c.G("mpHeatOn")).T() && !(s.G("boiler").G("on")).T())) {
          m.C("_say", Gv.S("Parní kotel je vypnutý – pára nejde (v panelu vpravo zapni kotel)."));
        }
      }
      else if (Gv.Eq(_t1, Gv.S("spmp_u")) || Gv.Eq(_t1, Gv.S("spmp_d"))) {
        c = c.S("mpSet", Gd.clampf((c.C("get", Gv.S("mpSet"), Gv.I(50L)) + ((Gv.Eq(k, Gv.S("spmp_u")) ? Gv.F(1.0) : Gv.F(-1.0)))), Gv.F(20.0), Gv.F(105.0)));
      }
      else if (Gv.Eq(_t1, Gv.S("spbo_u")) || Gv.Eq(_t1, Gv.S("spbo_d"))) {
        s = s.S("hlt", s.G("hlt").S("set", Gd.clampf((s.G("hlt").G("set") + ((Gv.Eq(k, Gv.S("spbo_u")) ? Gv.F(1.0) : Gv.F(-1.0)))), Gv.F(40.0), Gv.F(95.0))));
      }
      else if (Gv.Eq(_t1, Gv.S("rake"))) {
        if ((!(sk.G("rake")).T() && (sk.C("get", Gv.S("broken"), Gv.B(false))).T())) {
          _log(Gv.S("Kopačka SK je poškozená po rázu – nejdřív oprava (RESET PORUCHY SK = výměna střižného kolíku)."), Gv.S("bad"));
          return Gv.Null;
        }
        sk = sk.S("rake", Gv.B(!(sk.G("rake")).T()));
        if (!(sk.G("rake")).T()) {
          _stop_shock(Gv.S("sk"));
        }
        _log(((sk.G("rake")).T() ? ((Gv.Lt(sk.G("rakeH"), Gv.F(0.4)) ? Gv.S("Míchadlo SK (kopačka) se točí, ale je nahoře – spusť ji hydraulikou do lože.") : Gv.S("Míchadlo SK (kopačka) běží."))) : Gv.S("Míchadlo SK (kopačka) zastaveno.")));
      }
      else if (Gv.Eq(_t1, Gv.S("potsk_u")) || Gv.Eq(_t1, Gv.S("potsk_d"))) {
        c = c.S("potsk", _frac_of((_st(_pot_frac(Gv.S("potsk"))) + ((Gv.Eq(k, Gv.S("potsk_u")) ? Gv.I(1L) : Gv.I(-1L))))));
      }
      else if (Gv.Eq(_t1, Gv.S("hyd"))) {
        c = c.S("hydOn", Gv.B(!(c.C("get", Gv.S("hydOn"), Gv.B(false))).T()));
        if (!(c.G("hydOn")).T()) {
          sk = sk.S("hyd", Gv.F(0.0));
        }
        _log((Gv.S("Hydraulika SK %s.") % (((c.G("hydOn")).T() ? Gv.S("zapnuta") : Gv.S("vypnuta")))));
      }
      else if (Gv.Eq(_t1, Gv.S("pboil"))) {
        m.C("toggle_pump", Gv.S("phw"));
      }
      else if (Gv.Eq(_t1, Gv.S("pvarna"))) {
        m.C("toggle_pump", Gv.S("p1"));
      }
      else if (Gv.Eq(_t1, Gv.S("rez2"))) {
        m.C("toggle_pump", Gv.S("p3"));
      }
      else if (Gv.Eq(_t1, Gv.S("rez1"))) {
        m.C("toggle_pump", Gv.S("plv"));
      }
      else if (Gv.Eq(_t1, Gv.S("bxs"))) {
        m.C("toggle_pump", Gv.S("p2"));
      }
      else if (Gv.Eq(_t1, Gv.S("potp_u")) || Gv.Eq(_t1, Gv.S("potp_d"))) {
        _bind_step(Gv.S("p1"), (Gv.Eq(k, Gv.S("potp_u")) ? Gv.I(1L) : Gv.I(-1L)));
      }
      else if (Gv.Eq(_t1, Gv.S("bxp_u")) || Gv.Eq(_t1, Gv.S("bxp_d"))) {
        _bind_step(Gv.S("p2"), (Gv.Eq(k, Gv.S("bxp_u")) ? Gv.I(1L) : Gv.I(-1L)));
      }
      else if (Gv.Eq(_t1, Gv.S("hltheat"))) {
        s = s.S("hlt", s.G("hlt").S("auto", Gv.B(!(s.G("hlt").G("auto")).T())));
        _log((Gv.S("Ohřev boileru %s.") % (((s.G("hlt").G("auto")).T() ? ((Gv.S("zapnut (termostat %d °C)") % Gd.roundi(s.G("hlt").G("set")))) : Gv.S("vypnut")))));
      }
      else if (Gv.Eq(_t1, Gv.S("rstA")) || Gv.Eq(_t1, Gv.S("rstB")) || Gv.Eq(_t1, Gv.S("rstC"))) {
        c = c.S("fault", Gv.B(false));
        if ((Gv.Eq(k, Gv.S("rstA")) && (rv.C("get", Gv.S("broken"), Gv.B(false))).T())) {
          rv = rv.SI(Gv.S("broken"), Gv.B(false));
          rv = rv.SI(Gv.S("wear"), Gv.F(0.0));
          _log(Gv.S("Míchadlo MP opraveno (nová spojka / lopatka)."));
        }
        if ((Gv.Eq(k, Gv.S("rstB")) && (sk.C("get", Gv.S("broken"), Gv.B(false))).T())) {
          sk = sk.SI(Gv.S("broken"), Gv.B(false));
          sk = sk.SI(Gv.S("wear"), Gv.F(0.0));
          _log(Gv.S("Kopačka SK opravena (nový střižný kolík)."));
        }
        foreach (Gv pk in (Gd.Arr(Gv.S("p1"), Gv.S("p2"), Gv.S("p3"))).Iter()) {
          s = s.SI(pk, s.I(pk).S("dry", Gv.F(0.0)));
          s = s.SI(pk, s.I(pk).S("dead", Gv.F(0.0)));
        }
        _log(Gv.S("Reset poruchy měniče."));
      }
    }
    m.C("redraw");
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "Art": return S_cabinet.Art;
    case "m": return this.m;
    case "it": return this.it;
    case "sig": return this.sig;
    case "sc": return this.sc;
    case "fault_extra": return this.fault_extra;
    case "CAB_W": return S_cabinet.CAB_W;
    case "CAB_H": return S_cabinet.CAB_H;
    case "PX": return S_cabinet.PX;
    case "PW": return S_cabinet.PW;
    case "COLS": return S_cabinet.COLS;
    case "ROW_Y": return S_cabinet.ROW_Y;
    case "ROWS": return S_cabinet.ROWS;
    case "DISP": return S_cabinet.DISP;
    case "DISP_X": return S_cabinet.DISP_X;
    case "DISP_Y": return S_cabinet.DISP_Y;
    case "FMS": return S_cabinet.FMS;
    case "flash": return this.flash;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "m": this.m = v; return v;
    case "it": this.it = v; return v;
    case "sig": this.sig = v; return v;
    case "sc": this.sc = v; return v;
    case "fault_extra": this.fault_extra = v; return v;
    case "flash": this.flash = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "setup": return setup(Gd.A(a, 0));
    case "S": return S();
    case "_live": return _live();
    case "_sel_on": return _sel_on(Gd.A(a, 0));
    case "_pot_frac": return _pot_frac(Gd.A(a, 0));
    case "_st": return _st(Gd.A(a, 0));
    case "_stf": return _stf(Gd.A(a, 0));
    case "_frac_of": return _frac_of(Gd.A(a, 0));
    case "pot_hit": return pot_hit(Gd.A(a, 0));
    case "pot_value": return pot_value(Gd.A(a, 0));
    case "set_pot": return set_pot(Gd.A(a, 0), Gd.A(a, 1));
    case "_fm_frac": return _fm_frac(Gd.A(a, 0));
    case "_status": return _status(Gd.A(a, 0));
    case "_disp_text": return _disp_text(Gd.A(a, 0));
    case "_sig": return _sig();
    case "upd": return upd();
    case "_t": return _t(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4));
    case "_bezel": return _bezel(Gd.A(a, 0), Gd.A(a, 1));
    case "_sticker": return _sticker(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "_colors": return _colors(Gd.A(a, 0));
    case "_lamp": return _lamp(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "_flashing": return _flashing(Gd.A(a, 0));
    case "_glow": return _glow(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "_push": return _push(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4));
    case "_sel": return _sel(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4));
    case "_key": return _key(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "_pot": return _pot(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "_draw": return _draw();
    case "hit": return hit(Gd.A(a, 0));
    case "_dead": return _dead();
    case "_log": return _log(Gd.A(a, 0), Gd.A(a, 1));
    case "_bind_step": return _bind_step(Gd.A(a, 0), Gd.A(a, 1));
    case "_stop_shock": return _stop_shock(Gd.A(a, 0));
    case "act": return act(Gd.A(a, 0), Gd.A(a, 1));
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    case "Art": return Art;
    case "CAB_W": return CAB_W;
    case "CAB_H": return CAB_H;
    case "PX": return PX;
    case "PW": return PW;
    case "COLS": return COLS;
    case "ROW_Y": return ROW_Y;
    case "ROWS": return ROWS;
    case "DISP": return DISP;
    case "DISP_X": return DISP_X;
    case "DISP_Y": return DISP_Y;
    case "FMS": return FMS;
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_cabinet(); return Gv.O(o); }
  public override string ScriptName { get { return "cabinet"; } }
}
}
