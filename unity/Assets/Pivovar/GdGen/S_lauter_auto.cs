// Vygenerováno z lauter_auto.gd překladačem gd2cs.py – needituj ručně.
using System; using System.Collections.Generic;
namespace Pivovar.Gd.Gen {
public partial class S_lauter_auto : GdObject {
  public Gv simulation_ref;
  public static Gv SIGHT_ID;
  public static Gv CLEAR_TURB;
  static S_lauter_auto() {
    SIGHT_ID = Gv.S("inl_mur9u2hg");
    CLEAR_TURB = Gv.F(15.0);
  }
  public S_lauter_auto() {
    simulation_ref = Gv.Null;
  }
  public Gv get_sim() {
    return simulation_ref.C("get_ref");
    return Gv.Null;
  }
  public Gv _init(Gv simulation = default(Gv)) {
    simulation_ref = Gd.weakref(simulation);
    return Gv.Null;
  }
  public Gv state() {
    if (!(get_sim().G("S").C("has", Gv.S("lauterAuto"))).T()) {
      get_sim().G("S").S("lauterAuto", Gd.Dict(Gv.S("on"), Gv.B(false), Gv.S("stage"), Gv.S("vypnut"), Gv.S("settle"), Gv.F(0.0), Gv.S("clear"), Gv.F(0.0), Gv.S("reason"), Gv.S(""), Gv.S("routeVersion"), Gv.I(0L)));
    }
    return get_sim().G("S").G("lauterAuto");
    return Gv.Null;
  }
  public Gv route(Gv dst = default(Gv)) {
    Gv valves = get_sim().C("route_valves", Gv.S("sk"), dst, Gd.Arr(Gv.S("p2")), (Gv.Eq(dst, Gv.S("sk")) ? Gv.I(0L) : Gv.I(-1L)));
    Gv water = get_sim().C("route_valves", Gv.S("hlt"), Gv.S("sk"), Gd.Arr(Gv.S("phw")), Gv.I(0L));
    if ((Gv.Eq(valves, Gv.Null) || Gv.Eq(water, Gv.Null))) {
      return Gv.B(false);
    }
    Gv steam_valves = Gd.Dict();
    foreach (Gv node in (get_sim().G("net").G("nodes").C("values")).Iter()) {
      if (Gv.Eq(node.G("valve"), Gv.Null)) {
        continue;
      }
      foreach (Gv edge in (node.G("edges")).Iter()) {
        if (Gv.Eq(edge.G("medium"), Gv.S("steam"))) {
          steam_valves = steam_valves.SI(node.G("valve"), Gv.B(true));
        }
      }
    }
    foreach (Gv it in (get_sim().G("net").G("byId").C("values")).Iter()) {
      if ((Gv.Eq(it.G("type"), Gv.S("valve")) && !(steam_valves.C("has", it.G("id"))).T())) {
        it = it.SI(Gv.S("open"), Gv.B(false));
      }
      if ((Gv.Eq(it.G("type"), Gv.S("vessel")) && Gv.Eq(it.C("get", Gv.S("shape")), Gv.S("inlet")))) {
        it = it.SI(Gv.S("on"), Gv.B(false));
      }
    }
    foreach (Gv key in (get_sim().G("main").G("outlet_open")).Iter()) {
      get_sim().G("main").G("outlet_open").SI(key, Gv.B(false));
    }
    foreach (Gv key_2 in ((valves + water)).Iter()) {
      if ((get_sim().G("main").G("outlet_open").C("has", key_2)).T()) {
        get_sim().G("main").G("outlet_open").SI(key_2, Gv.B(true));
      } else if ((get_sim().G("net").G("byId").C("has", key_2)).T()) {
        get_sim().G("net").G("byId").I(key_2).SI(Gv.S("open"), Gv.B(true));
      }
    }
    get_sim().S("valve_ver", (get_sim().G("valve_ver") + Gv.I(1L)));
    state().S("routeVersion", get_sim().G("valve_ver"));
    return Gv.B(true);
    return Gv.Null;
  }
  public Gv start() {
    Gv S = get_sim().G("S");
    Gv a = state();
    if (((((Gv.Ne(S.G("sk").G("mode"), Gv.S("lauter")) || Gv.Lt(S.G("sk").G("g"), Gv.I(1L))) || (S.G("p1").G("on")).T()) || (S.G("p3").G("on")).T()) || (S.G("rvpWater").G("on")).T())) {
      get_sim().C("logm", Gv.S("Automat scezování: nejprve odrmutuj (tlačítko Odrmutovat); zastav C1, C3 a napouštění pánve."), Gv.S("warn"));
      return Gv.B(false);
    }
    if ((Gv.Gt(get_sim().C("vol", S.G("rvp")), Gv.I(100L)) || Gv.Gt(S.G("rvp").G("heat"), Gv.I(0L)))) {
      get_sim().C("logm", Gv.S("Automat scezování potřebuje prázdnou pánev s vypnutou párou."), Gv.S("warn"));
      return Gv.B(false);
    }
    if (!(route(Gv.S("sk"))).T()) {
      get_sim().C("logm", Gv.S("Automat nemá cestu pro vorlauf a vyslazovací vodu."), Gv.S("bad"));
      return Gv.B(false);
    }
    a = a.S("on", Gv.B(true));
    a = a.S("stage", Gv.S("usazení"));
    a = a.S("settle", Gv.F(0.0));
    a = a.S("clear", Gv.F(0.0));
    a = a.S("reason", Gv.S(""));
    S = S.S("p2", S.G("p2").S("on", Gv.B(false)));
    S = S.S("skWater", S.G("skWater").S("on", Gv.B(false)));
    S = S.S("sk", S.G("sk").S("rake", Gv.B(false)));
    S = S.S("sk", S.G("sk").S("hydT", Gv.F(0.4)));
    if (Gv.Gt(S.G("sk").C("get", Gv.S("dep"), Gv.F(0.0)), Gv.F(0.0))) {
      S = S.S("sk", S.G("sk").SI(Gv.S("dep"), Gv.F(0.0)));
      get_sim().C("logm", Gv.S("Automat scezování prokopl síta (několik trhnutí klapkou) – nános ze dna je pryč."));
    }
    get_sim().C("logm", Gv.S("Automat scezování přebírá kapalinné klapky: klid, vorlauf, odběr a vyslazování."));
    return Gv.B(true);
    return Gv.Null;
  }
  public Gv stop(Gv reason = default(Gv)) {
    Gv a = state();
    a = a.S("on", Gv.B(false));
    a = a.S("stage", Gv.S("zastaven"));
    a = a.S("reason", reason);
    get_sim().G("S").G("p2").S("on", Gv.B(false));
    get_sim().G("S").G("skWater").S("on", Gv.B(false));
    get_sim().C("logm", (Gv.S("Automat scezování zastaven: ") + reason));
    return Gv.Null;
  }
  public Gv update(Gv seconds = default(Gv)) {
    seconds = (seconds).AsF();
    Gv a = state();
    if (!(a.G("on")).T()) {
      return Gv.Null;
    }
    Gv S = get_sim().G("S");
    Gv sk = S.G("sk");
    if (((Gv.Ne(get_sim().G("valve_ver"), a.C("get", Gv.S("routeVersion"), get_sim().G("valve_ver"))) || Gv.Ne(sk.G("mode"), Gv.S("lauter"))) || Gv.Lt(sk.G("g"), Gv.I(1L)))) {
      stop(Gv.S("změna klapek nebo filtračního lože obsluhou"));
      return Gv.Null;
    }
    if ((((!(get_sim().C("cab_live")).T() || (S.G("p1").G("on")).T()) || (S.G("p3").G("on")).T()) || Gv.Gt(S.G("rvp").G("heat"), Gv.I(0L)))) {
      stop(Gv.S("zásah obsluhy / nepovolený souběh"));
      return Gv.Null;
    }
    if ((Gv.Le(S.G("hlt").G("w"), Gv.I(150L)) || Gv.Lt(S.G("hlt").G("T"), Gv.I(76L)))) {
      stop(Gv.S("nedostatek horké vyslazovací vody"));
      return Gv.Null;
    }
    if (Gv.Eq(a.G("stage"), Gv.S("usazení"))) {
      S = S.S("p2", S.G("p2").S("on", Gv.B(false)));
      S = S.S("skWater", S.G("skWater").S("on", Gv.B(false)));
      sk = sk.S("rake", Gv.B(false));
      a = a.S("settle", ((S.G("skFillNow")).T() ? Gv.F(0.0) : (a.G("settle") + seconds)));
      if (Gv.Ge(a.G("settle"), Gv.I(1200L))) {
        a = a.S("stage", Gv.S("vorlauf"));
        S = S.S("p2", S.G("p2").S("flow", Gv.I(25L)));
        S = S.S("p2", S.G("p2").S("on", Gv.B(true)));
      }
      return Gv.Null;
    }
    Gv sample = get_sim().C("sight_sample", get_sim().G("net").G("byId").I(SIGHT_ID));
    Gv actual = (sample.C("get", Gv.S("flow"), Gv.F(0.0))).AsF();
    if (Gv.Eq(a.G("stage"), Gv.S("vorlauf"))) {
      S = S.S("p2", S.G("p2").S("flow", Gd.clampf((sk.G("Qmax") * Gv.F(0.5)), Gv.F(8.0), Gv.F(25.0))));
      S = S.S("p2", S.G("p2").S("on", Gv.B(true)));
      Gv clear = Gv.B((((Gv.Eq(sample.C("get", Gv.S("kind"), Gv.S("empty")), Gv.S("wort")) && Gv.Gt(actual, Gv.I(0L))) && Gv.Lt(sample.C("get", Gv.S("turb"), Gv.I(999L)), CLEAR_TURB)) && Gv.Ge(sk.G("vorlaufL"), Gv.I(220L))));
      a = a.S("clear", ((clear).T() ? (a.G("clear") + seconds) : Gv.F(0.0)));
      if (Gv.Ge(a.G("clear"), Gv.I(60L))) {
        if (!(route(Gv.S("rvp"))).T()) {
          stop(Gv.S("neprůchodná cesta do pánve"));
          return Gv.Null;
        }
        a = a.S("stage", Gv.S("odběr"));
        state().S("routeVersion", get_sim().G("valve_ver"));
      }
      return Gv.Null;
    }
    if ((Gv.Gt(sample.C("get", Gv.S("turb"), Gv.I(0L)), Gv.I(40L)) && Gv.Gt(actual, Gv.I(0L)))) {
      if (!(route(Gv.S("sk"))).T()) {
        stop(Gv.S("neprůchodný návrat zákalu"));
        return Gv.Null;
      }
      a = a.S("stage", Gv.S("vorlauf"));
      a = a.S("clear", Gv.I(0L));
      S = S.S("skWater", S.G("skWater").S("on", Gv.B(false)));
      get_sim().C("logm", Gv.S("Zákal: automat vrací sladinu do SK a zpomaluje odběr."), Gv.S("warn"));
      return Gv.Null;
    }
    if ((Gv.Ge(get_sim().C("hotV", S.G("rvp")), S.G("cfg").C("get", Gv.S("preboilV"), Gv.F(4430.0))) || (((Gv.Gt(get_sim().C("hotV", S.G("rvp")), Gv.I(2000L)) && Gv.Gt(actual, Gv.I(0L))) && Gv.Ne(sample.C("get", Gv.S("kind"), Gv.S("empty")), Gv.S("empty"))) && Gv.Le(sample.C("get", Gv.S("P"), Gv.I(99L)), S.G("cfg").G("spargeEndP"))))) {
      get_sim().C("logm", (Gv.S("Konec: průhledítko %.2f °P, %s, %.0f L/min; podsítí %.2f °P") % Gd.Arr(sample.C("get", Gv.S("P"), Gv.I(-1L)), sample.C("get", Gv.S("kind"), Gv.S("empty")), actual, sk.G("outP"))));
      stop(Gv.S("cílový objem nebo slabé výstřelky"));
      return Gv.Null;
    }
    if (Gv.Gt(get_sim().C("tannin_risk"), Gv.F(0.5))) {
      stop(Gv.S("rizikové pH / teplota vyslazování; ověř měřením"));
      return Gv.Null;
    }
    if (Gv.Lt(sk.G("Qmax"), Gv.I(10L))) {
      stop(Gv.S("utáhnuté lože: nejprve ručně mělce nakypři a obnov vorlauf"));
      return Gv.Null;
    }
    S = S.S("p2", S.G("p2").S("flow", Gd.clampf(((sk.G("Qmax") * Gv.F(5000.0)) / Gd.maxf(Gv.F(6000.0), sk.G("dpAvail"))), Gv.I(15L), Gv.I(60L))));
    S = S.S("p2", S.G("p2").S("on", Gv.B(true)));
    if (Gv.Gt(sk.G("dp"), Gv.F(150.0))) {
      S = S.S("p2", S.G("p2").S("flow", Gd.maxf(Gv.F(10.0), (S.G("p2").G("flow") * Gv.F(0.5)))));
      sk = sk.S("rake", Gv.B(true));
      sk = sk.S("hydT", Gv.F(0.7));
      get_sim().C("once", (Gv.S("autorake2%d") % get_sim().C("tb", Gv.I(900L))), (Gv.S("Automat: Δp %d mbar – zpomaluji odběr a kypřím hlouběji.") % Gd.roundi(sk.G("dp"))), Gv.S("warn"));
    } else if (Gv.Gt(sk.G("dp"), Gv.F(90.0))) {
      sk = sk.S("rake", Gv.B(true));
      sk = sk.S("hydT", Gv.F(0.4));
    } else if ((Gv.Lt(sk.G("dp"), Gv.F(60.0)) && (sk.G("rake")).T())) {
      sk = sk.S("rake", Gv.B(false));
    }
    S = S.S("skWater", S.G("skWater").S("on", Gv.B(Gv.Lt(get_sim().C("skHead"), Gv.F(0.1)))));
    S = S.S("skWater", S.G("skWater").S("T", Gv.I(77L)));
    S = S.S("mixAuto", Gv.B(true));
    S = S.S("skWater", S.G("skWater").S("flow", Gd.clampf((sk.G("Qact") + (((((Gv.F(0.1) - get_sim().C("skHead"))) * S.G("cfg").G("skArea")) * Gv.F(1000.0)) / Gv.F(2.0))), Gv.I(10L), Gv.I(100L))));
    if ((S.G("skWater").G("on")).T()) {
      a = a.S("stage", Gv.S("vyslazování"));
    }
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "simulation_ref": return this.simulation_ref;
    case "sim": return get_sim();
    case "SIGHT_ID": return S_lauter_auto.SIGHT_ID;
    case "CLEAR_TURB": return S_lauter_auto.CLEAR_TURB;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "simulation_ref": this.simulation_ref = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "_init": return _init(Gd.A(a, 0));
    case "state": return state();
    case "route": return route(Gd.A(a, 0));
    case "start": return start();
    case "stop": return stop(Gd.A(a, 0));
    case "update": return update(Gd.A(a, 0));
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    case "SIGHT_ID": return SIGHT_ID;
    case "CLEAR_TURB": return CLEAR_TURB;
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_lauter_auto(); o._init(Gd.A(a, 0)); return Gv.O(o); }
  public override string ScriptName { get { return "lauter_auto"; } }
}
}
