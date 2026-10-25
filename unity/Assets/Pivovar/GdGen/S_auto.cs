// Vygenerováno z auto.gd překladačem gd2cs.py – needituj ručně.
using System; using System.Collections.Generic;
namespace Pivovar.Gd.Gen {
public partial class S_auto : GdObject {
  public Gv m;
  public Gv sim;
  public Gv S;
  public Gv on;
  public Gv i;
  public Gv t0;
  public Gv held;
  public Gv cool_tf;
  public static Gv COOL_TAU;
  public static Gv COOL_G;
  public Gv T_set;
  public Gv status;
  public Gv failed;
  public Gv done;
  public Gv mashV;
  public Gv total_mash;
  public Gv boil_hold;
  public Gv clear_s;
  public Gv keep;
  public Gv ice_prep;
  public Gv pend;
  public static Gv STEPS;
  static S_auto() {
    COOL_TAU = Gv.F(120.0);
    COOL_G = Gv.F(0.25);
    STEPS = Gd.Arr(Gd.Arr(Gv.S("Kontrola: zavřít všechny klapky"), Gv.I(1L), Gv.S("check")), Gd.Arr(Gv.S("Napouštím vodu do pánve (38 °C)"), Gv.I(60L), Gv.S("fill")), Gd.Arr(Gv.S("Vystírám slad, míchadlo"), Gv.I(40L), Gv.S("malt")), Gd.Arr(Gv.S("Ohřev na 50 °C a prodleva 15 min"), Gv.I(90L), Gv.S("r52")), Gd.Arr(Gv.S("Podsítí SK: zaplavit vodou"), Gv.I(60L), Gv.S("under")), Gd.Arr(Gv.S("1. rmut: polovinu díla do SK (C1)"), Gv.I(90L), Gv.S("d1split")), Gd.Arr(Gv.S("1. rmut: 63 °C, 20 min"), Gv.I(120L), Gv.S("d1_63")), Gd.Arr(Gv.S("1. rmut: 72 °C, 20 min"), Gv.I(120L), Gv.S("d1_72")), Gd.Arr(Gv.S("1. rmut: var 10 min"), Gv.I(120L), Gv.S("d1_boil")), Gd.Arr(Gv.S("1. rmut: zpět do SK"), Gv.I(90L), Gv.S("d1_ret")), Gd.Arr(Gv.S("2. rmut: dvě třetiny díla ze SK do pánve"), Gv.I(90L), Gv.S("d2split")), Gd.Arr(Gv.S("2. rmut: 63 °C, 20 min"), Gv.I(120L), Gv.S("d2_63")), Gd.Arr(Gv.S("2. rmut: 72 °C, 20 min"), Gv.I(120L), Gv.S("d2_72")), Gd.Arr(Gv.S("2. rmut: var 10 min"), Gv.I(120L), Gv.S("d2_boil")), Gd.Arr(Gv.S("2. rmut: zpět do SK"), Gv.I(90L), Gv.S("d2_ret")), Gd.Arr(Gv.S("Zcukření v SK (10 min)"), Gv.I(40L), Gv.S("conv")), Gd.Arr(Gv.S("Odrmutovat: vytvořit filtrační lože"), Gv.I(1L), Gv.S("bed")), Gd.Arr(Gv.S("Usazení lože 20 min"), Gv.I(25L), Gv.S("settle")), Gd.Arr(Gv.S("Proplach pod síty proudem horké vody z bojleru (do kanálu)"), Gv.I(30L), Gv.S("flush")), Gd.Arr(Gv.S("Vorlauf: sladina přes lože, než se vyčistí"), Gv.I(90L), Gv.S("vorlauf")), Gd.Arr(Gv.S("Scezování 1: první sladina do pánve (~30 hl)"), Gv.I(150L), Gv.S("run1")), Gd.Arr(Gv.S("Voda pod síta a promíchání mláta s ~1 hl vody"), Gv.I(30L), Gv.S("mix")), Gd.Arr(Gv.S("Podržení mláta (klid)"), Gv.I(20L), Gv.S("hold")), Gd.Arr(Gv.S("Dosladit do konečného objemu"), Gv.I(240L), Gv.S("sparge")), Gd.Arr(Gv.S("Doplnit pánev horkou vodou na objem (poslední podíl pod 1 °P se nescezuje)"), Gv.I(30L), Gv.S("topup")), Gd.Arr(Gv.S("Chmelovar"), Gv.I(300L), Gv.S("boil")), Gd.Arr(Gv.S("Přečerpání do vířivky"), Gv.I(90L), Gv.S("wp")), Gd.Arr(Gv.S("Usazení kalu ve vířivce"), Gv.I(90L), Gv.S("wpset")), Gd.Arr(Gv.S("Ledovka: dočkat se vychlazení"), Gv.I(300L), Gv.S("ice")), Gd.Arr(Gv.S("Chlazení a spilka (C3 + ledová voda)"), Gv.I(120L), Gv.S("cool")), Gd.Arr(Gv.S("Protlačka a hotovo"), Gv.I(5L), Gv.S("push")));
  }
  public S_auto() {
    m = Gv.Null;
    sim = Gv.Null;
    S = Gv.Null;
    on = Gv.B(false);
    i = Gv.I(-1L);
    t0 = Gv.F(0.0);
    held = Gv.F(0.0);
    cool_tf = Gv.F(-99.0);
    T_set = Gv.F(0.0);
    status = Gv.S("");
    failed = Gv.S("");
    done = Gv.B(false);
    mashV = Gv.F(0.0);
    total_mash = Gv.F(0.0);
    boil_hold = Gv.F(0.0);
    clear_s = Gv.F(0.0);
    keep = Gd.Dict();
    ice_prep = Gv.B(false);
    pend = Gd.Dict();
  }
  public Gv start() {
    S = sim.G("S");
    on = Gv.B(true);
    done = Gv.B(false);
    failed = Gv.S("");
    i = Gv.I(0L);
    _begin();
    S = S.S("sp", S.G("sp").SI(Gv.S("lidOpen"), Gv.B(true)));
    m = m.S("running", Gv.B(true));
    S = S.S("mixAuto", Gv.B(true));
    sim.C("logm", Gv.S("AUTO-VÁRKA: start."), Gv.S("info"));
    return Gv.Null;
  }
  public Gv stop(Gv msg = default(Gv)) {
    if (msg.k == 0) msg = Gv.S("");
    on = Gv.B(false);
    S = S.S("mixAuto", Gv.B(false));
    pend.C("clear");
    foreach (Gv p in (Gd.Arr(Gv.S("p1"), Gv.S("p2"), Gv.S("p3"))).Iter()) {
      S = S.SI(p, S.I(p).S("on", Gv.B(false)));
    }
    S = S.S("rvpWater", S.G("rvpWater").S("on", Gv.B(false)));
    S = S.S("skWater", S.G("skWater").S("on", Gv.B(false)));
    S = S.S("malt", S.G("malt").S("on", Gv.B(false)));
    S = S.S("rvp", S.G("rvp").S("heat", Gv.F(0.0)));
    if (Gv.Ne(msg, Gv.S(""))) {
      sim.C("logm", (Gv.S("AUTO-VÁRKA: ") + msg), Gv.S("warn"));
    }
    return Gv.Null;
  }
  public Gv _log(Gv t = default(Gv)) {
    sim.C("logm", (Gv.S("AUTO: ") + t));
    return Gv.Null;
  }
  public Gv _id() {
    return ((Gv.Ge(i, Gv.I(0L)) && Gv.Lt(i, STEPS.C("size"))) ? STEPS.I(i).I(Gv.I(2L)) : Gv.S(""));
    return Gv.Null;
  }
  public Gv _close_all() {
    pend.C("clear");
    foreach (Gv pk in (Gd.Arr(Gv.S("p1"), Gv.S("p2"), Gv.S("p3"))).Iter()) {
      S = S.SI(pk, S.I(pk).S("on", Gv.B(false)));
    }
    foreach (Gv it__it in (m.G("items")).Iter()) {
      Gv it = it__it;
      if ((Gv.Eq(it.G("type"), Gv.S("valve")) && Gv.Ne(it.C("get", Gv.S("kind"), Gv.S("")), Gv.S("zasl")))) {
        it = it.SI(Gv.S("open"), keep.C("has", it.G("id")));
      }
    }
    foreach (Gv k in (m.G("outlet_open")).Iter()) {
      m = m.S("outlet_open", m.G("outlet_open").SI(k, keep.C("has", k)));
    }
    _inlet(Gv.B(((ice_prep).T() && Gv.Lt(S.G("lv").G("w"), (Gv.F(0.75) * S.G("cfg").G("lvVol"))))));
    return Gv.Null;
  }
  public Gv _ice_start() {
    keep.C("clear");
    foreach (Gv pair in (Gd.Arr(Gd.Arr(Gv.S("vodovod"), Gv.S("lv"), Gd.Arr()), Gd.Arr(Gv.S("lv"), Gv.S("lv"), Gd.Arr(Gv.S("plv"))))).Iter()) {
      Gv vs = sim.C("route_valves", pair.I(Gv.I(0L)), pair.I(Gv.I(1L)), pair.I(Gv.I(2L)));
      if (Gv.Eq(vs, Gv.Null)) {
        continue;
      }
      foreach (Gv id in (vs).Iter()) {
        keep = keep.SI(id, Gv.B(true));
      }
    }
    ice_prep = Gv.B(true);
    foreach (Gv it__it in (m.G("items")).Iter()) {
      Gv it = it__it;
      if (Gv.Eq(it.G("type"), Gv.S("sw"))) {
        it = it.SI(Gv.S("on"), Gv.B(true));
      }
    }
    S = S.S("lv", S.G("lv").S("pump", Gv.B(true)));
    foreach (Gv id_2 in (keep).Iter()) {
      if ((m.G("outlet_open").C("has", id_2)).T()) {
        m = m.S("outlet_open", m.G("outlet_open").SI(id_2, Gv.B(true)));
      } else if ((sim.G("net").G("byId").C("has", id_2)).T()) {
        sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(id_2, sim.G("net").G("byId").I(id_2).SI(Gv.S("open"), Gv.B(true)))));
      }
    }
    _inlet(Gv.B(true));
    return Gv.Null;
  }
  public Gv _pump(Gv key = default(Gv)) {
    if ((((Gv.Eq(S.G("rvp").G("stir"), Gv.B(false)) && Gv.Gt(sim.C("vol", S.G("rvp")), Gv.F(100.0))) && Gv.Gt(sim.C("thick_factor", S.G("rvp")), Gv.F(0.15))) && Gv.In(_id(), Gd.Arr(Gv.S("d1split"), Gv.S("d1_ret"), Gv.S("d2_ret"))))) {
      S = S.S("rvp", S.G("rvp").S("stir", Gv.B(true)));
    }
    if (((Gv.Eq(S.G("sk").G("mode"), Gv.S("mash")) && Gv.Gt(sim.C("vol", S.G("sk").G("mash")), Gv.F(5.0))) && Gv.Gt(sim.C("thick_factor", S.G("sk").G("mash")), Gv.F(0.15)))) {
      S = S.S("sk", S.G("sk").S("rake", Gv.B(true)));
    }
    pend = pend.SI(key, Gv.B(true));
    return Gv.Null;
  }
  public Gv _pump_wait() {
    foreach (Gv k in (pend.C("keys")).Iter()) {
      if (Gv.Ge(S.I(k).G("prime"), Gv.F(0.95))) {
        S = S.SI(k, S.I(k).S("on", Gv.B(true)));
        pend.C("erase", k);
      }
    }
    return Gv.Null;
  }
  public Gv _route(Gv a = default(Gv), Gv b = default(Gv), Gv pumps = default(Gv), Gv where = default(Gv), Gv plate = default(Gv)) {
    if (where.k == 0) where = Gv.I(-1L);
    if (plate.k == 0) plate = Gv.S("");
    Gv vs = sim.C("route_valves", a, b, pumps, where, plate);
    if (Gv.Eq(vs, Gv.Null)) {
      _fail((Gv.S("Cesta %s → %s ve schématu neexistuje.") % Gd.Arr(sim.C("VN", a), sim.C("VN", b))));
      return Gv.B(false);
    }
    foreach (Gv id in (vs).Iter()) {
      if ((m.G("outlet_open").C("has", id)).T()) {
        m = m.S("outlet_open", m.G("outlet_open").SI(id, Gv.B(true)));
      } else if ((sim.G("net").G("byId").C("has", id)).T()) {
        sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(id, sim.G("net").G("byId").I(id).SI(Gv.S("open"), Gv.B(true)))));
        if ((Gv.Eq(sim.G("net").G("byId").I(id).C("get", Gv.S("kind"), Gv.S("")), Gv.S("skrt")) && Gv.Eq(Gd.Dyn("int", sim.G("net").G("byId").I(id).C("get", Gv.S("pct"), Gv.I(50L))), Gv.I(0L)))) {
          sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(id, sim.G("net").G("byId").I(id).SI(Gv.S("pct"), Gv.I(100L)))));
        }
      }
    }
    sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
    return Gv.B(true);
    return Gv.Null;
  }
  public Gv _plate_id() {
    foreach (Gv it in (m.G("items")).Iter()) {
      if ((Gv.Eq(it.G("type"), Gv.S("vessel")) && Gv.Eq(it.C("get", Gv.S("shape")), Gv.S("plate")))) {
        return it.G("id");
      }
    }
    return Gv.S("");
    return Gv.Null;
  }
  public Gv ice_inlet_on() {
    foreach (Gv it in (m.G("items")).Iter()) {
      if ((Gv.Eq(it.G("type"), Gv.S("vessel")) && Gv.Eq(it.C("get", Gv.S("shape")), Gv.S("inlet")))) {
        return it.C("get", Gv.S("on"), Gv.B(false));
      }
    }
    return Gv.B(false);
    return Gv.Null;
  }
  public Gv _inlet(Gv on_ = default(Gv)) {
    foreach (Gv it__it in (m.G("items")).Iter()) {
      Gv it = it__it;
      if ((Gv.Eq(it.G("type"), Gv.S("vessel")) && Gv.Eq(it.C("get", Gv.S("shape")), Gv.S("inlet")))) {
        it = it.SI(Gv.S("on"), on_);
      }
    }
    sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
    return Gv.Null;
  }
  public Gv _fail(Gv why = default(Gv)) {
    failed = why;
    status = (Gv.S("ZASEKLO SE: ") + why);
    sim.C("logm", (Gv.S("AUTO-VÁRKA se zastavila – ") + why), Gv.S("bad"));
    stop();
    return Gv.Null;
  }
  public Gv _regulate() {
    Gv r = S.G("rvp");
    if (Gv.Le(T_set, Gv.F(0.0))) {
      return Gv.Null;
    }
    Gv e = ((T_set - r.G("Tsens"))).AsF();
    r = r.S("heat", ((Gv.Lt(r.G("w"), Gv.I(20L)) || Gv.Le(e, Gv.F(-0.3))) ? Gv.F(0.0) : Gd.clampf((e / Gv.F(3.0)), Gv.F(0.12), Gv.F(1.0))));
    return Gv.Null;
  }
  public Gv _wait_min() {
    return ((((S.G("t") - t0)) / Gv.F(60.0))).AsF();
    return Gv.Null;
  }
  public Gv _is_hold(Gv id = default(Gv)) {
    return Gv.B(Gv.In(id, Gd.Arr(Gv.S("r52"), Gv.S("d1_63"), Gv.S("d1_72"), Gv.S("d2_63"), Gv.S("d2_72"))));
    return Gv.Null;
  }
  public Gv _begin() {
    Gv s = STEPS.I(i);
    Gv id = s.I(Gv.I(2L));
    status = (Gv.S("%d/%d  %s") % Gd.Arr((i + Gv.I(1L)), STEPS.C("size"), s.I(Gv.I(0L))));
    t0 = S.G("t");
    held = Gv.F(0.0);
    clear_s = Gv.F(0.0);
    cool_tf = Gv.F(-99.0);
    Gv c = S.G("cfg");
    _log(s.I(Gv.I(0L)));
    if (Gv.In(id, Gd.Arr(Gv.S("fill"), Gv.S("under"), Gv.S("d1split"), Gv.S("d1_ret"), Gv.S("d2split"), Gv.S("d2_ret"), Gv.S("conv"), Gv.S("settle"), Gv.S("flush"), Gv.S("vorlauf"), Gv.S("run1"), Gv.S("mix"), Gv.S("hold"), Gv.S("sparge"), Gv.S("topup"), Gv.S("wp"), Gv.S("cool")))) {
      _close_all();
    }
    {
      Gv _t1 = id;
      if (Gv.Eq(_t1, Gv.S("check"))) {
        _close_all();
        _ice_start();
      }
      else if (Gv.Eq(_t1, Gv.S("fill"))) {
        if (!(_route(Gv.S("hlt"), Gv.S("rvp"), Gd.Arr(Gv.S("phw")))).T()) {
          return Gv.Null;
        }
        S = S.S("mixAuto", Gv.B(true));
        S = S.S("rvpWater", S.G("rvpWater").S("T", Gv.F(38.0)));
        S = S.S("rvpWater", S.G("rvpWater").S("k", Gv.F(1.0)));
        S = S.S("rvpWater", S.G("rvpWater").S("target", (S.G("rvpWater").G("added") + c.G("water"))));
        S = S.S("rvpWater", S.G("rvpWater").S("on", Gv.B(true)));
      }
      else if (Gv.Eq(_t1, Gv.S("malt"))) {
        S = S.S("rvp", S.G("rvp").S("stir", Gv.B(true)));
        S = S.S("malt", S.G("malt").S("target", Gd.maxf(S.G("malt").G("added"), c.G("malt"))));
        S = S.S("malt", S.G("malt").S("gate", Gv.B(true)));
        S = S.S("malt", S.G("malt").S("on", Gv.B(true)));
      }
      else if (Gv.Eq(_t1, Gv.S("r52"))) {
        T_set = Gv.F(50.0);
      }
      else if (Gv.Eq(_t1, Gv.S("under"))) {
        T_set = Gv.F(50.0);
        if (!(_route(Gv.S("hlt"), Gv.S("sk"), Gd.Arr(Gv.S("phw")), Gv.I(1L))).T()) {
          return Gv.Null;
        }
        S = S.S("skWater", S.G("skWater").S("T", Gv.F(78.0)));
        S = S.S("skWater", S.G("skWater").S("on", Gv.B(true)));
      }
      else if (Gv.Eq(_t1, Gv.S("d1split"))) {
        S = S.S("skWater", S.G("skWater").S("on", Gv.B(false)));
        T_set = Gv.F(0.0);
        S = S.S("rvp", S.G("rvp").S("heat", Gv.F(0.0)));
        if (!(_route(Gv.S("rvp"), Gv.S("sk"), Gd.Arr(Gv.S("p1")))).T()) {
          return Gv.Null;
        }
        mashV = sim.C("vol", S.G("rvp"));
        total_mash = mashV;
        S = S.S("p1", S.G("p1").S("flow", Gv.F(250.0)));
        _pump(Gv.S("p1"));
      }
      else if (Gv.Eq(_t1, Gv.S("d1_63")) || Gv.Eq(_t1, Gv.S("d2_63"))) {
        T_set = Gv.F(63.0);
      }
      else if (Gv.Eq(_t1, Gv.S("d1_72")) || Gv.Eq(_t1, Gv.S("d2_72"))) {
        T_set = Gv.F(72.0);
      }
      else if (Gv.Eq(_t1, Gv.S("d1_boil")) || Gv.Eq(_t1, Gv.S("d2_boil"))) {
        T_set = Gv.F(0.0);
        S = S.S("rvp", S.G("rvp").S("stir", Gv.B(true)));
        S = S.S("rvp", S.G("rvp").S("heat", Gv.F(1.0)));
      }
      else if (Gv.Eq(_t1, Gv.S("d1_ret")) || Gv.Eq(_t1, Gv.S("d2_ret"))) {
        T_set = Gv.F(0.0);
        S = S.S("rvp", S.G("rvp").S("heat", Gv.F(0.0)));
        if (!(_route(Gv.S("rvp"), Gv.S("sk"), Gd.Arr(Gv.S("p1")))).T()) {
          return Gv.Null;
        }
        S = S.S("p1", S.G("p1").S("flow", Gv.F(250.0)));
        _pump(Gv.S("p1"));
      }
      else if (Gv.Eq(_t1, Gv.S("d2split"))) {
        if (!(_route(Gv.S("sk"), Gv.S("rvp"), Gd.Arr(Gv.S("p1")))).T()) {
          return Gv.Null;
        }
        total_mash = (sim.C("vol", S.G("rvp")) + sim.C("vol", S.G("sk").G("mash")));
        S = S.S("p1", S.G("p1").S("flow", Gv.F(250.0)));
        _pump(Gv.S("p1"));
      }
      else if (Gv.Eq(_t1, Gv.S("conv"))) {
        T_set = Gv.F(0.0);
      }
      else if (Gv.Eq(_t1, Gv.S("bed"))) {
        sim.C("start_lautering");
      }
      else if (Gv.Eq(_t1, Gv.S("settle"))) {
        ;
      }
      else if (Gv.Eq(_t1, Gv.S("flush"))) {
        if (!(_route(Gv.S("hlt"), Gv.S("sk"), Gd.Arr(Gv.S("phw")), Gv.I(1L))).T()) {
          return Gv.Null;
        }
        Gv dv = sim.C("drain_valves", Gv.S("sk"));
        if (Gv.Eq(dv, Gv.Null)) {
          sim.C("logm", Gv.S("Plán nemá pod síty SK výpust do kanálu – proplach (protlačka) se přeskakuje. Doplň odpadní trubku s klapkou ode dna pod sity."), Gv.S("warn"));
          S = S.S("flush", Gd.Dict(Gv.S("on"), Gv.B(false), Gv.S("L"), Gv.F(300.0), Gv.S("target"), Gv.F(300.0), Gv.S("vs"), Gd.Arr(), Gv.S("rate"), Gv.F(100.0)));
          S = S.S("sk", S.G("sk").SI(Gv.S("dep"), Gv.F(0.0)));
          return Gv.Null;
        }
        foreach (Gv vid in (dv).Iter()) {
          if ((m.G("outlet_open").C("has", vid)).T()) {
            m = m.S("outlet_open", m.G("outlet_open").SI(vid, Gv.B(true)));
          } else if ((sim.G("net").G("byId").C("has", vid)).T()) {
            sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(vid, sim.G("net").G("byId").I(vid).SI(Gv.S("open"), Gv.B(true)))));
          }
        }
        Gv wv = sim.C("route_valves", Gv.S("hlt"), Gv.S("sk"), Gd.Arr(Gv.S("phw")), Gv.I(1L));
        S = S.S("flush", Gd.Dict(Gv.S("on"), Gv.B(true), Gv.S("L"), Gv.F(0.0), Gv.S("target"), Gv.F(300.0), Gv.S("vs"), (((Gv.Ne(wv, Gv.Null) ? wv : Gd.Arr())) + dv), Gv.S("rate"), Gv.F(100.0)));
        sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
      }
      else if (Gv.Eq(_t1, Gv.S("topup"))) {
        S = S.S("p2", S.G("p2").S("on", Gv.B(false)));
        S = S.S("skWater", S.G("skWater").S("on", Gv.B(false)));
        Gv need = (Gd.maxf(Gv.F(0.0), (c.C("get", Gv.S("preboilV"), Gv.F(4430.0)) - sim.C("hotV", S.G("rvp"))))).AsF();
        keep = keep.SI(Gv.S("topup"), need);
        if (Gv.Gt(need, Gv.F(20.0))) {
          if (!(_route(Gv.S("hlt"), Gv.S("rvp"), Gd.Arr(Gv.S("phw")))).T()) {
            return Gv.Null;
          }
          S = S.S("rvpWater", S.G("rvpWater").S("T", Gv.F(78.0)));
          S = S.S("rvpWater", S.G("rvpWater").S("k", Gv.F(1.0)));
          S = S.S("rvpWater", S.G("rvpWater").S("target", (S.G("rvpWater").G("added") + need)));
          S = S.S("rvpWater", S.G("rvpWater").S("on", Gv.B(true)));
        }
      }
      else if (Gv.Eq(_t1, Gv.S("vorlauf"))) {
        if (!(_route(Gv.S("sk"), Gv.S("sk"), Gd.Arr(Gv.S("p2")), Gv.I(0L))).T()) {
          return Gv.Null;
        }
        S = S.S("p2", S.G("p2").S("flow", Gv.F(20.0)));
        _pump(Gv.S("p2"));
        S = S.S("sk", S.G("sk").S("rake", Gv.B(false)));
        S = S.S("sk", S.G("sk").S("hydT", Gv.F(0.4)));
      }
      else if (Gv.Eq(_t1, Gv.S("run1"))) {
        S = S.S("p2", S.G("p2").S("on", Gv.B(false)));
        S = S.S("skWater", S.G("skWater").S("on", Gv.B(false)));
        if (!(_route(Gv.S("sk"), Gv.S("rvp"), Gd.Arr(Gv.S("p2")))).T()) {
          return Gv.Null;
        }
        S = S.S("p2", S.G("p2").S("flow", Gv.F(40.0)));
        _pump(Gv.S("p2"));
      }
      else if (Gv.Eq(_t1, Gv.S("mix"))) {
        S = S.S("p2", S.G("p2").S("on", Gv.B(false)));
        if (!(_route(Gv.S("hlt"), Gv.S("sk"), Gd.Arr(Gv.S("phw")), Gv.I(1L))).T()) {
          return Gv.Null;
        }
        keep = keep.SI(Gv.S("mix0"), S.G("skWater").G("total"));
        S = S.S("skWater", S.G("skWater").S("T", Gv.F(77.0)));
        S = S.S("skWater", S.G("skWater").S("flow", Gv.F(50.0)));
        S = S.S("skWater", S.G("skWater").S("on", Gv.B(true)));
        S = S.S("sk", S.G("sk").S("rake", Gv.B(true)));
        S = S.S("sk", S.G("sk").S("hydT", Gv.F(0.7)));
      }
      else if (Gv.Eq(_t1, Gv.S("hold"))) {
        S = S.S("skWater", S.G("skWater").S("on", Gv.B(false)));
        S = S.S("p2", S.G("p2").S("on", Gv.B(false)));
        S = S.S("sk", S.G("sk").S("rake", Gv.B(false)));
      }
      else if (Gv.Eq(_t1, Gv.S("sparge"))) {
        S = S.S("p2", S.G("p2").S("on", Gv.B(false)));
        if (!(_route(Gv.S("sk"), Gv.S("rvp"), Gd.Arr(Gv.S("p2")))).T()) {
          return Gv.Null;
        }
        if (!(_route(Gv.S("hlt"), Gv.S("sk"), Gd.Arr(Gv.S("phw")), Gv.I(0L))).T()) {
          return Gv.Null;
        }
        S = S.S("skWater", S.G("skWater").S("T", Gv.F(77.0)));
        S = S.S("skWater", S.G("skWater").S("flow", Gv.F(60.0)));
        S = S.S("p2", S.G("p2").S("flow", Gv.F(60.0)));
        S = S.S("skWater", S.G("skWater").S("on", Gv.B(true)));
        _pump(Gv.S("p2"));
      }
      else if (Gv.Eq(_t1, Gv.S("boil"))) {
        S = S.S("p2", S.G("p2").S("on", Gv.B(false)));
        S = S.S("skWater", S.G("skWater").S("on", Gv.B(false)));
        S = S.S("rvp", S.G("rvp").S("stir", Gv.B(false)));
        T_set = Gv.F(0.0);
        S = S.S("rvp", S.G("rvp").S("heat", Gv.F(0.85)));
        boil_hold = Gv.F(0.0);
      }
      else if (Gv.Eq(_t1, Gv.S("wp"))) {
        S = S.S("rvp", S.G("rvp").S("heat", Gv.F(0.0)));
        T_set = Gv.F(0.0);
        if (!(_route(Gv.S("rvp"), Gv.S("wp"), Gd.Arr(Gv.S("p1")))).T()) {
          return Gv.Null;
        }
        S = S.S("p1", S.G("p1").S("flow", Gv.F(300.0)));
        _pump(Gv.S("p1"));
      }
      else if (Gv.Eq(_t1, Gv.S("wpset"))) {
        S = S.S("p1", S.G("p1").S("on", Gv.B(false)));
      }
      else if (Gv.Eq(_t1, Gv.S("ice"))) {
        ;
      }
      else if (Gv.Eq(_t1, Gv.S("cool"))) {
        if (!(_route(Gv.S("wp"), Gv.S("spilka"), Gd.Arr(Gv.S("p3")))).T()) {
          return Gv.Null;
        }
        if (!(_route(Gv.S("lv"), Gv.S("hlt"), Gd.Arr(Gv.S("plv")), Gv.I(-1L), _plate_id())).T()) {
          return Gv.Null;
        }
        if (!(_route(Gv.S("lv"), Gv.S("lv"), Gd.Arr(Gv.S("plv")))).T()) {
          return Gv.Null;
        }
        S = S.S("lv", S.G("lv").S("pump", Gv.B(true)));
        S = S.S("p3", S.G("p3").S("flow", Gv.F(40.0)));
        _pump(Gv.S("p3"));
      }
      else if (Gv.Eq(_t1, Gv.S("push"))) {
        sim.C("push");
      }
    }
    m.C("redraw");
    return Gv.Null;
  }
  public Gv _finished() {
    Gv f = S.G("flags");
    Gv c = S.G("cfg");
    Gv r = S.G("rvp");
    if ((((_is_hold(_id())).T() && Gv.Le(held, Gv.F(0.0))) && Gv.Gt(r.G("Tsens"), (T_set + Gv.F(2.0))))) {
      sim.C("logm", (Gv.S("AUTO: rmut má už %s °C (nad %s °C) – nechladí se, pokračuje se od této teploty.") % Gd.Arr(sim.C("fmt1", r.G("Tsens")), sim.C("fmt0", T_set))));
      return Gv.B(true);
    }
    {
      Gv _t1 = _id();
      if (Gv.Eq(_t1, Gv.S("check"))) {
        return Gv.B(true);
      }
      else if (Gv.Eq(_t1, Gv.S("fill"))) {
        return Gv.B((!(S.G("rvpWater").G("on")).T() && Gv.Ge(S.G("rvpWater").G("added"), (S.G("rvpWater").G("target") - Gv.F(5.0)))));
      }
      else if (Gv.Eq(_t1, Gv.S("malt"))) {
        return Gv.B(Gv.Ge(S.G("malt").G("added"), (S.G("malt").G("target") - Gv.F(1.0))));
      }
      else if (Gv.Eq(_t1, Gv.S("r52"))) {
        return Gv.B(Gv.Ge(held, Gv.F(15.0)));
      }
      else if (Gv.Eq(_t1, Gv.S("under"))) {
        return Gv.B(Gv.Ge(S.G("sk").G("under"), Gv.F(150.0)));
      }
      else if (Gv.Eq(_t1, Gv.S("d1split"))) {
        return Gv.B((!(S.G("p1").G("on")).T() && Gv.Gt(sim.C("vol", S.G("sk").G("mash")), Gv.F(500.0))));
      }
      else if (Gv.Eq(_t1, Gv.S("d1_63")) || Gv.Eq(_t1, Gv.S("d2_63"))) {
        return Gv.B(Gv.Ge(held, Gv.F(20.0)));
      }
      else if (Gv.Eq(_t1, Gv.S("d1_72")) || Gv.Eq(_t1, Gv.S("d2_72"))) {
        return Gv.B(Gv.Ge(held, Gv.F(20.0)));
      }
      else if (Gv.Eq(_t1, Gv.S("d1_boil")) || Gv.Eq(_t1, Gv.S("d2_boil"))) {
        return Gv.B(((r.G("boiling")).T() && Gv.Ge(r.G("grainBoilMin"), Gv.F(10.0))));
      }
      else if (Gv.Eq(_t1, Gv.S("d1_ret")) || Gv.Eq(_t1, Gv.S("d2_ret"))) {
        return Gv.B((!(S.G("p1").G("on")).T() && Gv.Le(sim.C("vol", r), (c.G("rvpDead") + Gv.F(12.0)))));
      }
      else if (Gv.Eq(_t1, Gv.S("d2split"))) {
        return Gv.B((!(S.G("p1").G("on")).T() && Gv.Ge(sim.C("vol", r), (((total_mash * Gv.F(2.0)) / Gv.F(3.0)) * Gv.F(0.9)))));
      }
      else if (Gv.Eq(_t1, Gv.S("conv"))) {
        return Gv.B((Gv.Ge(_wait_min(), Gv.F(10.0)) && Gv.Ge(S.G("sk").G("mash").G("T"), Gv.F(68.0))));
      }
      else if (Gv.Eq(_t1, Gv.S("bed"))) {
        return Gv.B(Gv.Eq(S.G("sk").G("mode"), Gv.S("lauter")));
      }
      else if (Gv.Eq(_t1, Gv.S("settle"))) {
        return Gv.B(Gv.Ge(_wait_min(), Gv.F(20.0)));
      }
      else if (Gv.Eq(_t1, Gv.S("flush"))) {
        return Gv.B(Gv.Ge(S.G("flush").G("L"), (S.G("flush").G("target") - Gv.F(0.5))));
      }
      else if (Gv.Eq(_t1, Gv.S("vorlauf"))) {
        return Gv.B((Gv.Ge(clear_s, Gv.F(60.0)) && Gv.Ge(S.G("sk").G("vorlaufL"), Gv.F(220.0))));
      }
      else if (Gv.Eq(_t1, Gv.S("run1"))) {
        return Gv.B((((Gv.Ge(sim.C("liqVol", r), Gv.F(3000.0)) || Gv.Lt(sim.C("cellVol", S.G("sk").G("cells").I(Gv.I(0L))), Gv.F(60.0))) || Gv.Le(sim.C("skVolTotal"), (sim.C("skRet") + Gv.F(20.0)))) || ((S.G("sk").G("drawn")).T() && Gv.Lt(S.G("sk").G("outP"), c.G("spargeEndP")))));
      }
      else if (Gv.Eq(_t1, Gv.S("mix"))) {
        return Gv.B(Gv.Ge((S.G("skWater").G("total") - Gd.Dyn("float", keep.C("get", Gv.S("mix0"), Gv.F(0.0)))), Gv.F(100.0)));
      }
      else if (Gv.Eq(_t1, Gv.S("hold"))) {
        return Gv.B(Gv.Ge(_wait_min(), Gv.F(10.0)));
      }
      else if (Gv.Eq(_t1, Gv.S("sparge"))) {
        return Gv.B((((Gv.Eq(f.G("full"), Gv.I(1L)) || ((!(S.G("skWater").G("on")).T() && Gv.Gt(S.G("skWater").G("total"), Gv.F(600.0))) && Gv.Le(sim.C("skVolTotal"), (sim.C("skRet") + Gv.F(20.0))))) || (Gv.Ge(S.G("skWater").G("total"), Gv.F(3000.0)) && !(S.G("p2").G("on")).T())) || (((S.G("sk").G("drawn")).T() && Gv.Lt(S.G("sk").G("outP"), c.G("spargeEndP"))) && Gv.Gt(S.G("skWater").G("total"), Gv.F(600.0)))));
      }
      else if (Gv.Eq(_t1, Gv.S("topup"))) {
        return Gv.B((Gv.Le(Gd.Dyn("float", keep.C("get", Gv.S("topup"), Gv.F(0.0))), Gv.F(20.0)) || (!(S.G("rvpWater").G("on")).T() && Gv.Ge(S.G("rvpWater").G("added"), (S.G("rvpWater").G("target") - Gv.F(5.0))))));
      }
      else if (Gv.Eq(_t1, Gv.S("boil"))) {
        return Gv.B(Gv.Ge(f.G("wortBoil"), c.G("boilMin")));
      }
      else if (Gv.Eq(_t1, Gv.S("wp"))) {
        return Gv.B((!(S.G("p1").G("on")).T() && Gv.Gt(sim.C("vol", S.G("wp")), Gv.F(3000.0))));
      }
      else if (Gv.Eq(_t1, Gv.S("wpset"))) {
        return Gv.B(Gv.Eq(f.G("whirlDone"), Gv.I(1L)));
      }
      else if (Gv.Eq(_t1, Gv.S("ice"))) {
        return Gv.B((Gv.Ge(S.G("lv").G("w"), (Gv.F(0.7) * c.G("lvVol"))) && Gv.Le(S.G("lv").G("T"), Gv.F(5.0))));
      }
      else if (Gv.Eq(_t1, Gv.S("cool"))) {
        return Gv.B((!(S.G("p3").G("on")).T() && Gv.Gt(S.G("sp").G("L"), Gv.F(3000.0))));
      }
      else if (Gv.Eq(_t1, Gv.S("push"))) {
        return Gv.B(Gv.Eq(f.G("pushed"), Gv.I(1L)));
      }
    }
    return Gv.B(true);
    return Gv.Null;
  }
  public Gv _end_step() {
    {
      Gv _t1 = _id();
      if (Gv.Eq(_t1, Gv.S("d1split")) || Gv.Eq(_t1, Gv.S("d2split")) || Gv.Eq(_t1, Gv.S("d1_ret")) || Gv.Eq(_t1, Gv.S("d2_ret")) || Gv.Eq(_t1, Gv.S("wp"))) {
        S = S.S("p1", S.G("p1").S("on", Gv.B(false)));
      }
      else if (Gv.Eq(_t1, Gv.S("flush"))) {
        S = S.S("flush", S.G("flush").S("on", Gv.B(false)));
      }
      else if (Gv.Eq(_t1, Gv.S("run1"))) {
        S = S.S("p2", S.G("p2").S("on", Gv.B(false)));
      }
      else if (Gv.Eq(_t1, Gv.S("mix"))) {
        S = S.S("skWater", S.G("skWater").S("on", Gv.B(false)));
        S = S.S("sk", S.G("sk").S("rake", Gv.B(false)));
      }
      else if (Gv.Eq(_t1, Gv.S("sparge"))) {
        S = S.S("skWater", S.G("skWater").S("on", Gv.B(false)));
        S = S.S("p2", S.G("p2").S("on", Gv.B(false)));
      }
      else if (Gv.Eq(_t1, Gv.S("boil")) || Gv.Eq(_t1, Gv.S("d1_boil")) || Gv.Eq(_t1, Gv.S("d2_boil"))) {
        S = S.S("rvp", S.G("rvp").S("heat", Gv.F(0.0)));
        T_set = Gv.F(0.0);
      }
      else if (Gv.Eq(_t1, Gv.S("malt"))) {
        S = S.S("malt", S.G("malt").S("gate", Gv.B(false)));
      }
    }
    if (Gv.Eq(S.G("sk").G("mode"), Gv.S("mash"))) {
      S = S.S("sk", S.G("sk").S("rake", Gv.B(false)));
    }
    i = (i + Gv.I(1L));
    if (Gv.Ge(i, STEPS.C("size"))) {
      on = Gv.B(false);
      done = Gv.B(true);
      status = Gv.S("AUTO-VÁRKA HOTOVA");
      sim.C("logm", Gv.S("AUTO-VÁRKA dokončena."), Gv.S("info"));
      stop();
      return Gv.Null;
    }
    _begin();
    return Gv.Null;
  }
  public Gv tick(Gv dt = default(Gv)) {
    dt = (dt).AsF();
    if (!(on).T()) {
      return Gv.Null;
    }
    if ((Gv.Lt(i, Gv.I(0L)) || Gv.Ge(i, STEPS.C("size")))) {
      return Gv.Null;
    }
    Gv r = S.G("rvp");
    Gv f = S.G("flags");
    Gv c = S.G("cfg");
    Gv id = _id();
    _pump_wait();
    if (Gv.Gt(T_set, Gv.I(0L))) {
      if (((_is_hold(id)).T() && Gv.Le(Gd.absf((r.G("Tsens") - T_set)), Gv.F(1.5)))) {
        held = (held + (dt / Gv.F(60.0)));
      }
      _regulate();
    }
    if (((Gv.Eq(id, Gv.S("d1split")) && (S.G("p1").G("on")).T()) && Gv.Le(sim.C("vol", r), (mashV * Gv.F(0.5))))) {
      S = S.S("p1", S.G("p1").S("on", Gv.B(false)));
    }
    if (((Gv.Eq(id, Gv.S("d2split")) && (S.G("p1").G("on")).T()) && Gv.Ge(sim.C("vol", r), ((total_mash * Gv.F(2.0)) / Gv.F(3.0))))) {
      S = S.S("p1", S.G("p1").S("on", Gv.B(false)));
    }
    if ((Gv.Eq(id, Gv.S("d1_boil")) || Gv.Eq(id, Gv.S("d2_boil")))) {
      S = S.S("rvp", S.G("rvp").S("stir", Gv.B(true)));
      if (((r.G("boiling")).T() && Gv.Ge(r.G("grainBoilMin"), Gv.F(10.0)))) {
        r = r.S("heat", Gv.F(0.0));
      }
    }
    if (Gv.Eq(id, Gv.S("boil"))) {
      if ((r.G("boiling")).T()) {
        boil_hold = (boil_hold + (dt / Gv.F(60.0)));
        r = r.S("heat", (Gv.Lt(boil_hold, Gv.F(15.0)) ? Gv.F(0.55) : Gv.F(0.85)));
        Gv nx = sim.C("hopPlanNext");
        if ((Gv.Ne(nx, Gv.Null) && Gv.Ge(boil_hold, nx.G("min")))) {
          r = r.S("heat", Gv.F(0.3));
          if (Gv.Lt(r.G("evapKgMin"), Gv.F(4.0))) {
            sim.C("hopPlanAdd");
          }
        }
      } else {
        r = r.S("heat", (Gv.Gt(S.G("rvp").G("T"), Gv.I(90L)) ? Gv.F(0.85) : Gv.F(1.0)));
      }
    }
    if (Gv.Eq(id, Gv.S("vorlauf"))) {
      S = S.S("p2", S.G("p2").S("flow", Gd.clampf((S.G("sk").G("Qmax") * Gv.F(0.5)), Gv.F(8.0), Gv.F(25.0))));
      Gv smp = sim.C("sight_sample", sim.G("net").G("byId").I(Gv.S("inl_mur9u2hg")));
      Gv clear = Gv.B((((Gv.Eq(smp.C("get", Gv.S("kind"), Gv.S("empty")), Gv.S("wort")) && Gv.Gt(smp.C("get", Gv.S("flow"), Gv.F(0.0)), Gv.F(0.0))) && Gv.Lt(smp.C("get", Gv.S("turb"), Gv.F(999.0)), Gv.F(15.0))) && Gv.Lt(S.G("sk").G("turb"), Gv.F(20.0))));
      clear_s = ((clear).T() ? (clear_s + dt) : Gv.F(0.0));
    }
    if (Gv.Eq(id, Gv.S("run1"))) {
      S = S.S("p2", S.G("p2").S("flow", Gd.clampf(((S.G("sk").G("Qmax") * Gv.F(5500.0)) / Gd.maxf(Gv.F(6000.0), S.G("sk").G("dpAvail"))), Gv.F(15.0), Gv.F(70.0))));
      if (Gv.Gt(S.G("sk").G("dp"), Gv.F(150.0))) {
        S = S.S("p2", S.G("p2").S("flow", Gd.maxf(Gv.F(12.0), (S.G("p2").G("flow") * Gv.F(0.5)))));
      }
      S = S.S("skWater", S.G("skWater").S("on", Gv.B(false)));
      if (Gv.Le(S.G("sk").G("dig"), Gv.I(0L))) {
        if (Gv.Gt(S.G("sk").G("dp"), Gv.F(120.0))) {
          S = S.S("sk", S.G("sk").S("hydT", Gv.F(0.7)));
          S = S.S("sk", S.G("sk").S("rake", Gv.B(true)));
        } else if ((Gv.Gt(S.G("sk").G("dp"), Gv.F(70.0)) || !(S.G("sk").G("rake")).T())) {
          S = S.S("sk", S.G("sk").S("hydT", Gv.F(0.4)));
          S = S.S("sk", S.G("sk").S("rake", Gv.B(true)));
        }
      }
    }
    if (Gv.Eq(id, Gv.S("sparge"))) {
      S = S.S("p2", S.G("p2").S("flow", Gd.clampf(((S.G("sk").G("Qmax") * Gv.F(5500.0)) / Gd.maxf(Gv.F(6000.0), S.G("sk").G("dpAvail"))), Gv.F(15.0), Gv.F(70.0))));
      if (Gv.Gt(S.G("sk").G("dp"), Gv.F(150.0))) {
        S = S.S("p2", S.G("p2").S("flow", Gd.maxf(Gv.F(12.0), (S.G("p2").G("flow") * Gv.F(0.5)))));
      }
      S = S.S("skWater", S.G("skWater").S("flow", Gd.clampf((S.G("p2").G("flow") * Gv.F(0.95)), Gv.F(10.0), Gv.F(100.0))));
      Gv free_sk = (Gd.maxf(Gv.F(0.0), (sim.C("skVolTotal") - sim.C("skRet")))).AsF();
      S = S.S("skWater", S.G("skWater").S("on", Gv.B((((Gv.Lt(S.G("skWater").G("total"), Gv.F(3000.0)) && Gv.Lt(sim.C("skVolTotal"), (c.G("skCap") * Gv.F(0.97)))) && Gv.Ne(f.G("full"), Gv.I(1L))) && Gv.Lt((sim.C("hotV", S.G("rvp")) + free_sk), c.C("get", Gv.S("preboilV"), Gv.F(4430.0)))))));
      if (Gv.Le(S.G("sk").G("dig"), Gv.I(0L))) {
        if (Gv.Gt(S.G("sk").G("dp"), Gv.F(120.0))) {
          S = S.S("sk", S.G("sk").S("hydT", Gv.F(0.7)));
          S = S.S("sk", S.G("sk").S("rake", Gv.B(true)));
        } else if ((Gv.Gt(S.G("sk").G("dp"), Gv.F(70.0)) || !(S.G("sk").G("rake")).T())) {
          S = S.S("sk", S.G("sk").S("hydT", Gv.F(0.4)));
          S = S.S("sk", S.G("sk").S("rake", Gv.B(true)));
        }
      }
    }
    if ((Gv.Eq(id, Gv.S("cool")) && Gv.Ne(S.G("cool").G("Tout"), Gv.Null))) {
      if (Gv.Lt(cool_tf, Gv.F(-50.0))) {
        cool_tf = S.G("cool").G("Tout");
      }
      cool_tf = (cool_tf + (((S.G("cool").G("Tout") - cool_tf)) * Gd.minf(Gv.F(1.0), (dt / COOL_TAU))));
      S = S.S("p3", S.G("p3").S("flow", Gd.clampf((S.G("p3").G("flow") + (((((Gv.F(8.5) - cool_tf)) * COOL_G) * dt) / Gv.F(60.0))), Gv.F(30.0), Gv.F(200.0))));
    }
    if ((ice_prep).T()) {
      if ((Gv.Ge(S.G("lv").G("w"), (Gv.F(0.35) * c.G("lvVol"))) && !(S.G("lv").G("chill")).T())) {
        sim.C("lvChillSet", Gv.B(true));
      }
      if ((Gv.Ge(S.G("lv").G("w"), (Gv.F(0.75) * c.G("lvVol"))) && (ice_inlet_on()).T())) {
        _inlet(Gv.B(false));
      }
    }
    if ((_finished()).T()) {
      _end_step();
      return Gv.Null;
    }
    if (Gv.Gt(_wait_min(), STEPS.I(i).I(Gv.I(1L)))) {
      _fail(_why());
    }
    return Gv.Null;
  }
  public Gv _why() {
    Gv S_ = S;
    {
      Gv _t1 = _id();
      if (Gv.Eq(_t1, Gv.S("fill"))) {
        return (Gv.S("Voda z bojleru do pánve neteče (napuštěno %d L). Zkontroluj cestu a čerpadlo horké vody. %s") % Gd.Arr(Gd.roundi(S_.G("rvpWater").G("added")), _recent()));
      }
      else if (Gv.Eq(_t1, Gv.S("malt"))) {
        return (Gv.S("Slad se nevystírá. %s") % _recent());
      }
      else if (Gv.Eq(_t1, Gv.S("r52")) || Gv.Eq(_t1, Gv.S("d1_63")) || Gv.Eq(_t1, Gv.S("d1_72")) || Gv.Eq(_t1, Gv.S("d2_63")) || Gv.Eq(_t1, Gv.S("d2_72"))) {
        return (Gv.S("Pánev nedosáhla/neudržela %s °C (je %s °C): pára nemá cestu do pláště nebo je málo vody. %s") % Gd.Arr(sim.C("fmt0", T_set), sim.C("fmt1", S_.G("rvp").G("T")), _recent()));
      }
      else if (Gv.Eq(_t1, Gv.S("d1_boil")) || Gv.Eq(_t1, Gv.S("d2_boil"))) {
        return (Gv.S("Rmut nevře dost dlouho (%s °C, var %d min). %s") % Gd.Arr(sim.C("fmt1", S_.G("rvp").G("T")), Gd.roundi(S_.G("rvp").G("grainBoilMin")), _recent()));
      }
      else if (Gv.Eq(_t1, Gv.S("under"))) {
        return (Gv.S("Voda pod síta SK neteče (podsítí %d L). %s") % Gd.Arr(Gd.roundi(S_.G("sk").G("under")), _recent()));
      }
      else if (Gv.Eq(_t1, Gv.S("d1split")) || Gv.Eq(_t1, Gv.S("d1_ret")) || Gv.Eq(_t1, Gv.S("d2split")) || Gv.Eq(_t1, Gv.S("d2_ret"))) {
        return (Gv.S("Rmut mezi pánví a SK neteče (v pánvi %d L, v SK %d L). %s") % Gd.Arr(Gd.roundi(sim.C("vol", S_.G("rvp"))), Gd.roundi(sim.C("vol", S_.G("sk").G("mash"))), _recent()));
      }
      else if (Gv.Eq(_t1, Gv.S("flush"))) {
        return (Gv.S("Proplach pod síty neteče (proteklo %d L). Zkontroluj cestu bojler → pod síta a výpust do kanálu. %s") % Gd.Arr(Gd.roundi(S_.G("flush").G("L")), _recent()));
      }
      else if (Gv.Eq(_t1, Gv.S("run1"))) {
        return (Gv.S("První sladina se nescezuje: v pánvi %d L. %s") % Gd.Arr(Gd.roundi(sim.C("liqVol", S_.G("rvp"))), _recent()));
      }
      else if (Gv.Eq(_t1, Gv.S("mix"))) {
        return (Gv.S("Voda pod síta neteče (přidáno %d L). %s") % Gd.Arr(Gd.roundi((S_.G("skWater").G("total") - Gd.Dyn("float", keep.C("get", Gv.S("mix0"), Gv.F(0.0))))), _recent()));
      }
      else if (Gv.Eq(_t1, Gv.S("vorlauf"))) {
        return (Gv.S("Vorlauf se nevyčistil (zákal %d). %s") % Gd.Arr(Gd.roundi(S_.G("sk").G("turb")), _recent()));
      }
      else if (Gv.Eq(_t1, Gv.S("sparge"))) {
        return (Gv.S("Sladina se nescezuje: v pánvi %d L. %s") % Gd.Arr(Gd.roundi(sim.C("liqVol", S_.G("rvp"))), _recent()));
      }
      else if (Gv.Eq(_t1, Gv.S("topup"))) {
        return (Gv.S("Horká voda z bojleru do pánve neteče (napuštěno %d L). %s") % Gd.Arr(Gd.roundi(S_.G("rvpWater").G("added")), _recent()));
      }
      else if (Gv.Eq(_t1, Gv.S("boil"))) {
        return (Gv.S("Mladina nevře dost dlouho (%d min). %s") % Gd.Arr(Gd.roundi(S_.G("flags").G("wortBoil")), _recent()));
      }
      else if (Gv.Eq(_t1, Gv.S("wp"))) {
        return (Gv.S("Do vířivky natekla jen %d L. %s") % Gd.Arr(Gd.roundi(sim.C("vol", S_.G("wp"))), _recent()));
      }
      else if (Gv.Eq(_t1, Gv.S("ice"))) {
        return (Gv.S("Ledovka se nenapustila/nenachladila (%d hl, %s °C). %s") % Gd.Arr(Gd.roundi((S_.G("lv").G("w") / Gv.F(100.0))), sim.C("fmt1", S_.G("lv").G("T")), _recent()));
      }
      else if (Gv.Eq(_t1, Gv.S("cool"))) {
        return (Gv.S("Do spilky natekla jen %d L. %s") % Gd.Arr(Gd.roundi(S_.G("sp").G("L")), _recent()));
      }
    }
    return (Gv.S("Krok trvá příliš dlouho. ") + _recent());
    return Gv.Null;
  }
  public Gv _recent() {
    Gv out_ = Gd.Arr();
    foreach (Gv l in (S.G("log").C("slice", Gv.I(0L), Gv.I(12L))).Iter()) {
      if ((((Gv.Eq(l.G("type"), Gv.S("bad")) || Gv.Eq(l.G("type"), Gv.S("warn"))) && !(l.G("msg").C("begins_with", Gv.S("AUTO"))).T()) && !(out_.C("has", l.G("msg"))).T())) {
        out_.C("append", l.G("msg"));
      }
      if (Gv.Ge(out_.C("size"), Gv.I(2L))) {
        break;
      }
    }
    return Gv.S(" | ").C("join", Gd.Ctor("PackedStringArray", out_));
    return Gv.Null;
  }
  public Gv to_dict() {
    return Gd.Dict(Gv.S("sid"), _id(), Gv.S("on"), on, Gv.S("i"), i, Gv.S("t0"), t0, Gv.S("held"), held, Gv.S("T_set"), T_set, Gv.S("status"), status, Gv.S("failed"), failed, Gv.S("done"), done, Gv.S("mashV"), mashV, Gv.S("total_mash"), total_mash, Gv.S("boil_hold"), boil_hold, Gv.S("clear_s"), clear_s, Gv.S("keep"), keep, Gv.S("ice_prep"), ice_prep);
    return Gv.Null;
  }
  public Gv from_dict(Gv d = default(Gv)) {
    on = d.C("get", Gv.S("on"), Gv.B(false));
    i = d.C("get", Gv.S("i"), Gv.I(-1L));
    if ((d.C("has", Gv.S("sid"))).T()) {
      foreach (Gv k in (STEPS.C("size")).Iter()) {
        if (Gv.Eq(STEPS.I(k).I(Gv.I(2L)), d.G("sid"))) {
          i = k;
        }
      }
    }
    t0 = d.C("get", Gv.S("t0"), Gv.F(0.0));
    held = d.C("get", Gv.S("held"), Gv.F(0.0));
    T_set = d.C("get", Gv.S("T_set"), Gv.F(0.0));
    status = d.C("get", Gv.S("status"), Gv.S(""));
    failed = d.C("get", Gv.S("failed"), Gv.S(""));
    done = d.C("get", Gv.S("done"), Gv.B(false));
    mashV = d.C("get", Gv.S("mashV"), Gv.F(0.0));
    total_mash = d.C("get", Gv.S("total_mash"), Gv.F(0.0));
    boil_hold = d.C("get", Gv.S("boil_hold"), Gv.F(0.0));
    clear_s = d.C("get", Gv.S("clear_s"), Gv.F(0.0));
    keep = d.C("get", Gv.S("keep"), Gd.Dict());
    ice_prep = d.C("get", Gv.S("ice_prep"), Gv.B(false));
    if (Gv.Ge(i, STEPS.C("size"))) {
      on = Gv.B(false);
    }
    S = sim.G("S");
    if ((on).T()) {
      S = S.S("mixAuto", Gv.B(true));
    }
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "m": return this.m;
    case "sim": return this.sim;
    case "S": return this.S;
    case "on": return this.on;
    case "i": return this.i;
    case "t0": return this.t0;
    case "held": return this.held;
    case "cool_tf": return this.cool_tf;
    case "COOL_TAU": return S_auto.COOL_TAU;
    case "COOL_G": return S_auto.COOL_G;
    case "T_set": return this.T_set;
    case "status": return this.status;
    case "failed": return this.failed;
    case "done": return this.done;
    case "mashV": return this.mashV;
    case "total_mash": return this.total_mash;
    case "boil_hold": return this.boil_hold;
    case "clear_s": return this.clear_s;
    case "keep": return this.keep;
    case "ice_prep": return this.ice_prep;
    case "pend": return this.pend;
    case "STEPS": return S_auto.STEPS;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "m": this.m = v; return v;
    case "sim": this.sim = v; return v;
    case "S": this.S = v; return v;
    case "on": this.on = v; return v;
    case "i": this.i = v; return v;
    case "t0": this.t0 = v; return v;
    case "held": this.held = v; return v;
    case "cool_tf": this.cool_tf = v; return v;
    case "T_set": this.T_set = v; return v;
    case "status": this.status = v; return v;
    case "failed": this.failed = v; return v;
    case "done": this.done = v; return v;
    case "mashV": this.mashV = v; return v;
    case "total_mash": this.total_mash = v; return v;
    case "boil_hold": this.boil_hold = v; return v;
    case "clear_s": this.clear_s = v; return v;
    case "keep": this.keep = v; return v;
    case "ice_prep": this.ice_prep = v; return v;
    case "pend": this.pend = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "start": return start();
    case "stop": return stop(Gd.A(a, 0));
    case "_log": return _log(Gd.A(a, 0));
    case "_id": return _id();
    case "_close_all": return _close_all();
    case "_ice_start": return _ice_start();
    case "_pump": return _pump(Gd.A(a, 0));
    case "_pump_wait": return _pump_wait();
    case "_route": return _route(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4));
    case "_plate_id": return _plate_id();
    case "ice_inlet_on": return ice_inlet_on();
    case "_inlet": return _inlet(Gd.A(a, 0));
    case "_fail": return _fail(Gd.A(a, 0));
    case "_regulate": return _regulate();
    case "_wait_min": return _wait_min();
    case "_is_hold": return _is_hold(Gd.A(a, 0));
    case "_begin": return _begin();
    case "_finished": return _finished();
    case "_end_step": return _end_step();
    case "tick": return tick(Gd.A(a, 0));
    case "_why": return _why();
    case "_recent": return _recent();
    case "to_dict": return to_dict();
    case "from_dict": return from_dict(Gd.A(a, 0));
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    case "COOL_TAU": return COOL_TAU;
    case "COOL_G": return COOL_G;
    case "STEPS": return STEPS;
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_auto(); return Gv.O(o); }
  public override string ScriptName { get { return "auto"; } }
}
}
