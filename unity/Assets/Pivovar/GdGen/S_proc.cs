// Vygenerováno z proc.gd překladačem gd2cs.py – needituj ručně.
using System; using System.Collections.Generic;
namespace Pivovar.Gd.Gen {
public partial class S_proc : GdObject {
  public Gv simulation_ref;
  public static Gv WATERS;
  public static Gv CIP_TARGETS;
  public static Gv CIP_MEDIA;
  public static Gv MINOR;
  public static Gv REST_T;
  static S_proc() {
    WATERS = Gd.Arr(Gd.Dict(Gv.S("name"), Gv.S("řad (odhad – zadej rozbor)"), Gv.S("alk"), Gv.F(100.0), Gv.S("ca"), Gv.F(50.0), Gv.S("mg"), Gv.F(10.0)), Gd.Dict(Gv.S("name"), Gv.S("měkká (plzeňská)"), Gv.S("alk"), Gv.F(15.0), Gv.S("ca"), Gv.F(8.0), Gv.S("mg"), Gv.F(2.0)), Gd.Dict(Gv.S("name"), Gv.S("tvrdá"), Gv.S("alk"), Gv.F(280.0), Gv.S("ca"), Gv.F(110.0), Gv.S("mg"), Gv.F(25.0)));
    CIP_TARGETS = Gd.Arr(Gd.Arr(Gv.S("rvp"), Gv.S("Pánev")), Gd.Arr(Gv.S("sk"), Gv.S("Scezovací káď")), Gd.Arr(Gv.S("wp"), Gv.S("Vířivá káď")), Gd.Arr(Gv.S("plate"), Gv.S("Deskový chladič")), Gd.Arr(Gv.S("spilka"), Gv.S("Spilka")));
    CIP_MEDIA = Gd.Arr(Gd.Arr(Gv.S("voda"), Gv.S("proplach vodou")), Gd.Arr(Gv.S("louh"), Gv.S("louh (NaOH) ≥ 75 °C")), Gd.Arr(Gv.S("kyselina"), Gv.S("kyselina ≤ 60 °C")));
    MINOR = Gd.Arr(Gv.S("Ucpávka C1 lehce kape – sleduj, při zvětšení vyměnit."), Gv.S("Tlak vody z řadu kolísá (2,5–3,5 bar)."), Gv.S("Kondenzátní hrnec u pánve syčí – zkontroluj odvod kondenzátu."), Gv.S("Teploměr pánve ukazuje o 1,5 °C víc než kontrolní teploměr – ověř kalibraci."), Gv.S("Na podlaze u vířivky je kaluž – zkontroluj šroubení a sání C3."), Gv.S("Ventilátor rozvaděče RT.20 je hlučný – zkontroluj filtr."), Gv.S("Hladinoznak bojleru se zasekává – poklepni / vyčisti."));
    REST_T = Gd.Dict(Gv.S("rest52"), Gv.F(50.0), Gv.S("rest63a"), Gv.F(63.0), Gv.S("rest63b"), Gv.F(63.0), Gv.S("rest72a"), Gv.F(72.0), Gv.S("rest72b"), Gv.F(72.0));
  }
  public S_proc() {
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
  public Gv init_state() {
    Gv S = get_sim().G("S");
    if (!(S.C("has", Gv.S("water"))).T()) {
      S = S.S("water", WATERS.I(Gv.I(0L)).C("duplicate"));
    }
    if (!(S.C("has", Gv.S("lactic"))).T()) {
      S = S.S("lactic", Gv.F(0.0));
    }
    if (!(S.C("has", Gv.S("fer"))).T()) {
      S = S.S("fer", Gd.Dict(Gv.S("on"), Gv.B(false), Gv.S("set"), Gv.F(9.0), Gv.S("T"), Gv.F(9.0), Gv.S("X"), Gv.F(0.0), Gv.S("S0"), Gv.F(0.0), Gv.S("Sf"), Gv.F(0.0), Gv.S("E"), Gv.F(0.0), Gv.S("AL"), Gv.F(0.0), Gv.S("D"), Gv.F(0.0), Gv.S("t"), Gv.F(0.0), Gv.S("pitch"), Gv.F(15.0), Gv.S("P0"), Gv.F(0.0)));
    }
    if (!(S.C("has", Gv.S("cip"))).T()) {
      S = S.S("cip", Gd.Dict(Gv.S("run"), Gv.B(false), Gv.S("target"), Gv.S("rvp"), Gv.S("medium"), Gv.S("louh"), Gv.S("set"), Gv.F(80.0), Gv.S("T"), Gv.F(20.0), Gv.S("min"), Gv.F(0.0), Gv.S("done"), Gd.Dict(), Gv.S("conc"), Gv.F(1.5)));
    }
    if (!(S.C("has", Gv.S("faults"))).T()) {
      S = S.S("faults", Gd.Dict(Gv.S("on"), Gv.B(false), Gv.S("next"), Gv.F(0.0), Gv.S("active"), Gd.Arr(), Gv.S("nextMinor"), Gv.F(0.0)));
    }
    if (!(S.C("has", Gv.S("sensOff"))).T()) {
      S = S.S("sensOff", Gv.F(0.0));
    }
    return Gv.Null;
  }
  public Gv residual_alk(Gv w = default(Gv)) {
    return ((w.G("alk") - (((w.G("ca") * Gv.F(0.714)) + (w.G("mg") * Gv.F(0.585)))))).AsF();
    return Gv.Null;
  }
  public Gv mash_ph() {
    Gv S = get_sim().G("S");
    Gv gt = Gv.F(0.0);
    Gv dark = Gv.F(0.0);
    foreach (Gv id in (S.G("grist")).Iter()) {
      gt = (gt + S.G("grist").I(id));
      Gv M = get_sim().G("MALTS").C("get", id, get_sim().G("MALTS").G("plz"));
      if (Gv.Gt(M.G("ebc"), Gv.I(30L))) {
        dark = (dark + (S.G("grist").I(id) * Gd.clampf((M.G("ebc") / Gv.F(300.0)), Gv.F(0.3), Gv.F(2.0))));
      } else if (Gv.Gt(M.G("unf"), Gv.F(0.2))) {
        dark = (dark + (S.G("grist").I(id) * Gv.F(0.3)));
      }
    }
    Gv df = ((dark / Gd.maxf(Gv.F(1.0), gt))).AsF();
    Gv V = (Gd.maxf(Gv.F(500.0), (get_sim().C("liqVol", S.G("rvp")) + get_sim().C("liqVol", S.G("sk").G("mash"))))).AsF();
    Gv malt = (Gd.maxf(Gv.F(100.0), S.G("malt").G("added"))).AsF();
    Gv ph = ((((Gv.F(5.75) - (Gv.F(0.35) * df)) + (Gv.F(0.0017) * residual_alk(S.G("water")))) - ((S.G("lactic") * Gv.F(11.8)) / ((Gv.F(35.0) * malt))))).AsF();
    return (Gd.clampf(ph, Gv.F(4.6), Gv.F(6.4))).AsF();
    return Gv.Null;
  }
  public Gv ph_factors(Gv ph = default(Gv)) {
    ph = (ph).AsF();
    return Gd.Ctor("Vector2", Gd.exp((-Gd.pow((((ph - Gv.F(5.7))) / Gv.F(0.8)), Gv.I(2L)))), Gd.exp((-Gd.pow((((ph - Gv.F(5.5))) / Gv.F(0.7)), Gv.I(2L)))));
    return Gv.Null;
  }
  public Gv fer_start() {
    Gv S = get_sim().G("S");
    Gv sp = S.G("sp");
    Gv f = S.G("fer");
    if (Gv.Lt(sp.G("L"), Gv.I(100L))) {
      get_sim().C("logm", Gv.S("Kvašení: ve spilce není mladina."), Gv.S("warn"));
      return Gv.Null;
    }
    f = f.S("on", Gv.B(true));
    f = f.S("T", (sp.G("H") / Gd.maxf(Gv.F(1.0), sp.G("L"))));
    f = f.S("X", (f.G("pitch") * Gv.F(0.04)));
    f = f.S("S0", ((sp.G("f") / sp.G("L")) * Gv.F(1000.0)));
    f = f.S("Sf", f.G("S0"));
    f = f.S("E", Gv.F(0.0));
    f = f.S("AL", Gv.F(0.0));
    f = f.S("D", Gv.F(0.0));
    f = f.S("t", Gv.F(0.0));
    f = f.S("P0", ((Gv.F(100.0) * ((sp.G("f") + sp.G("n")))) / Gd.maxf(Gv.F(1.0), ((sp.G("f") + sp.G("n")) + sp.G("w")))));
    get_sim().C("logm", (Gv.S("Zakvašeno: %s hl, %s °C, %d mil. buněk/mL, spilka chlazená na %s °C.") % Gd.Arr(get_sim().C("fmt1", (sp.G("L") / Gv.F(100.0))), get_sim().C("fmt1", f.G("T")), Gd.roundi(f.G("pitch")), get_sim().C("fmt1", f.G("set")))));
    return Gv.Null;
  }
  public Gv fer_step(Gv dt = default(Gv)) {
    dt = (dt).AsF();
    Gv S = get_sim().G("S");
    Gv f = S.G("fer");
    if (!(f.G("on")).T()) {
      return Gv.Null;
    }
    Gv h = ((dt / Gv.F(3600.0))).AsF();
    Gv a = (Gd.exp((Gv.F(0.095) * ((f.G("T") - Gv.F(12.0)))))).AsF();
    Gv mu = (((((Gv.F(0.05) * a) * f.G("Sf")) / ((Gv.F(8.0) + f.G("Sf")))) * ((Gv.F(1.0) - (f.G("X") / Gv.F(4.0)))))).AsF();
    Gv dS = (Gd.minf(f.G("Sf"), ((((((Gv.F(0.2) * a) * f.G("X")) * f.G("Sf")) / ((Gv.F(6.0) + f.G("Sf"))))) * h))).AsF();
    f = f.S("X", (f.G("X") + Gd.maxf(Gv.F(0.0), ((mu * f.G("X")) * h))));
    f = f.S("Sf", (f.G("Sf") - dS));
    f = f.S("E", (f.G("E") + (dS / Gv.F(2.0665))));
    f = f.S("CO2", (f.C("get", Gv.S("CO2"), Gv.F(0.0)) + ((dS * Gv.F(0.9565)) / Gv.F(2.0665))));
    f = f.S("XB", (f.C("get", Gv.S("XB"), Gv.F(0.0)) + ((dS * Gv.F(0.11)) / Gv.F(2.0665))));
    f = f.S("T", (f.G("T") + ((((dS * Gv.F(0.6)) * Gv.F(1000.0)) / Gv.F(4100.0)) - (((f.G("T") - f.G("set"))) * Gd.minf(Gv.F(1.0), (Gv.F(0.25) * h))))));
    f = f.S("AL", (f.G("AL") + (((mu * f.G("X")) * h) * Gv.F(0.9))));
    Gv k1 = ((Gv.F(0.012) * Gd.exp((Gv.F(0.11) * ((f.G("T") - Gv.F(12.0))))))).AsF();
    Gv conv = ((f.G("AL") * Gd.minf(Gv.F(1.0), (k1 * h)))).AsF();
    f = f.S("AL", (f.G("AL") - conv));
    f = f.S("D", (f.G("D") + (conv * Gv.F(1000.0))));
    f = f.S("D", (f.G("D") - (f.G("D") * Gd.minf(Gv.F(1.0), (((Gv.F(0.06) * a) * Gd.minf(Gv.F(1.0), (f.G("X") / Gv.F(1.5)))) * h)))));
    f = f.S("t", (f.G("t") + dt));
    if ((Gv.Lt(f.G("D"), Gv.I(100L)) && Gv.Lt(f.G("Sf"), (Gv.F(0.25) * f.G("S0"))))) {
      get_sim().C("once", Gv.S("dia_ok"), (Gv.S("Diacetyl %d ppb – pod prahem chuti (≈ 100 ppb), hlavní kvašení končí. Můžeš chladit na ležení.") % Gd.roundi(f.G("D"))));
    }
    if (((Gv.Lt(f.G("Sf"), (Gv.F(0.3) * f.G("S0"))) && Gv.Gt(f.G("D"), Gv.I(150L))) && Gv.Lt(f.G("set"), Gv.I(10L)))) {
      get_sim().C("once", Gv.S("dia_rest"), (Gv.S("Diacetyl %d ppb je vysoko a kvašení končí – zvaž diacetylovou pauzu (12–14 °C na 1–2 dny).") % Gd.roundi(f.G("D"))), Gv.S("warn"));
    }
    return Gv.Null;
  }
  public Gv fer_skip(Gv hours = default(Gv)) {
    hours = (hours).AsF();
    foreach (Gv i in (Gd.Dyn("int", (hours * Gv.F(60.0)))).Iter()) {
      fer_step(Gv.F(60.0));
    }
    return Gv.Null;
  }
  public Gv app_extract() {
    Gv f = get_sim().G("S").G("fer");
    if (Gv.Le(f.G("P0"), Gv.I(0L))) {
      return (Gv.F(0.0)).AsF();
    }
    return (((f.G("P0") - (((f.G("S0") - f.G("Sf"))) / Gv.F(10.0))) - ((Gv.F(0.46) * f.G("E")) / Gv.F(10.0)))).AsF();
    return Gv.Null;
  }
  public Gv cip_name(Gv t = default(Gv)) {
    foreach (Gv x in (CIP_TARGETS).Iter()) {
      if (Gv.Eq(x.I(Gv.I(0L)), t)) {
        return x.I(Gv.I(1L));
      }
    }
    return t;
    return Gv.Null;
  }
  public Gv cip_step(Gv dt = default(Gv)) {
    dt = (dt).AsF();
    Gv S = get_sim().G("S");
    Gv c = S.G("cip");
    if (!(c.G("run")).T()) {
      return Gv.Null;
    }
    Gv m = ((dt / Gv.F(60.0))).AsF();
    c = c.S("T", (c.G("T") + Gd.clampf((c.G("set") - c.G("T")), (Gv.F(-1.5) * m), (Gv.F(1.2) * m))));
    c = c.S("min", (c.G("min") + m));
    Gv ok = Gv.B(false);
    {
      Gv _t1 = c.G("medium");
      if (Gv.Eq(_t1, Gv.S("louh"))) {
        ok = Gv.B((Gv.Ge(c.G("T"), Gv.F(75.0)) && Gv.Ge(c.C("get", Gv.S("conc"), Gv.F(1.5)), get_sim().G("S").G("cfg").C("get", Gv.S("cipLyeMin"), Gv.F(1.0)))));
        if ((Gv.Lt(c.C("get", Gv.S("conc"), Gv.F(1.5)), get_sim().G("S").G("cfg").C("get", Gv.S("cipLyeMin"), Gv.F(1.0))) && Gv.Gt(c.G("min"), Gv.I(2L)))) {
          get_sim().C("once", (Gv.S("lyeweak%d") % get_sim().C("tb", Gv.I(600L))), (Gv.S("CIP: louh má jen %s %% NaOH – pod %s %% nerozpustí úsady. Doplň koncentrát.") % Gd.Arr(get_sim().C("fmt1", c.C("get", Gv.S("conc"), Gv.F(1.5))), get_sim().C("fmt1", get_sim().G("S").G("cfg").C("get", Gv.S("cipLyeMin"), Gv.F(1.0))))), Gv.S("warn"));
        }
        if ((Gv.Lt(c.G("T"), Gv.F(75.0)) && Gv.Gt(c.G("min"), Gv.I(5L)))) {
          get_sim().C("once", (Gv.S("lyecold%s%d") % Gd.Arr(c.G("target"), get_sim().C("tb", Gv.I(600L)))), (Gv.S("CIP: louh má jen %d °C – pod 75 °C nerozpustí bílkovinné úsady a pivní kámen.") % Gd.roundi(c.G("T"))), Gv.S("warn"));
        }
        if (Gv.Gt(c.G("T"), Gv.F(90.0))) {
          get_sim().C("once", (Gv.S("lyehot%d") % get_sim().C("tb", Gv.I(600L))), Gv.S("CIP: louh nad 90 °C – zbytečně vysoká teplota, hrozí opaření a poškození těsnění."), Gv.S("warn"));
        }
      }
      else if (Gv.Eq(_t1, Gv.S("kyselina"))) {
        ok = Gv.B(((Gv.Ge(c.G("T"), Gv.F(20.0)) && Gv.Le(c.G("T"), Gv.F(60.0))) && Gv.Ge(c.C("get", Gv.S("conc"), Gv.F(1.0)), get_sim().G("S").G("cfg").C("get", Gv.S("cipAcidMin"), Gv.F(0.5)))));
        if ((Gv.Lt(c.C("get", Gv.S("conc"), Gv.F(1.0)), get_sim().G("S").G("cfg").C("get", Gv.S("cipAcidMin"), Gv.F(0.5))) && Gv.Gt(c.G("min"), Gv.I(2L)))) {
          get_sim().C("once", (Gv.S("acidweak%d") % get_sim().C("tb", Gv.I(600L))), (Gv.S("CIP: kyselina má jen %s %% – pod %s %% neodstraní pivní kámen.") % Gd.Arr(get_sim().C("fmt1", c.C("get", Gv.S("conc"), Gv.F(1.0))), get_sim().C("fmt1", get_sim().G("S").G("cfg").C("get", Gv.S("cipAcidMin"), Gv.F(0.5))))), Gv.S("warn"));
        }
        if (Gv.Gt(c.G("T"), Gv.F(60.5))) {
          get_sim().C("once", (Gv.S("acidhot%d") % get_sim().C("tb", Gv.I(600L))), Gv.S("CIP: kyselina nad 60 °C – koroze nerezu a výpary. Uber teplotu."), Gv.S("bad"));
        }
      }
      else if (Gv.Eq(_t1, Gv.S("voda"))) {
        ok = Gv.B(true);
      }
    }
    Gv key = ((c.G("target") + Gv.S(":")) + c.G("medium"));
    if ((ok).T()) {
      c = c.S("done", c.G("done").SI(key, (c.G("done").C("get", key, Gv.F(0.0)) + m)));
    }
    Gv need = ((Gv.Ne(c.G("medium"), Gv.S("voda")) ? Gv.F(20.0) : Gv.F(5.0))).AsF();
    if ((Gv.Ge(c.G("done").C("get", key, Gv.F(0.0)), need) && !(c.G("done").C("has", (key + Gv.S("!")))).T())) {
      c = c.S("done", c.G("done").SI((key + Gv.S("!")), Gv.I(1L)));
      Gv nxt = Gd.Dict(Gv.S("voda"), Gv.S("pokračuj louhem"), Gv.S("louh"), Gv.S("proplach vodou a kyselina"), Gv.S("kyselina"), Gv.S("proplach vodou, nádoba je čistá")).C("get", c.G("medium"), Gv.S(""));
      get_sim().C("logm", (Gv.S("CIP %s: %s %d min hotovo ✔ – %s.") % Gd.Arr(cip_name(c.G("target")), c.G("medium"), Gd.roundi(need), nxt)));
    }
    return Gv.Null;
  }
  public Gv cip_status() {
    Gv c = get_sim().G("S").G("cip");
    Gv parts = Gd.Arr();
    foreach (Gv t in (CIP_TARGETS).Iter()) {
      Gv st = Gv.S("");
      foreach (Gv md in (Gd.Arr(Gv.S("voda"), Gv.S("louh"), Gv.S("kyselina"))).Iter()) {
        if ((c.G("done").C("has", (((t.I(Gv.I(0L)) + Gv.S(":")) + md) + Gv.S("!")))).T()) {
          st = (st + Gd.Dict(Gv.S("voda"), Gv.S("V"), Gv.S("louh"), Gv.S("L"), Gv.S("kyselina"), Gv.S("K")).I(md));
        }
      }
      if (Gv.Ne(st, Gv.S(""))) {
        parts.C("append", (Gv.S("%s %s") % Gd.Arr(t.I(Gv.I(1L)), st)));
      }
    }
    return ((parts.C("size")).T() ? ((Gv.S("hotovo: ") + Gv.S(", ").C("join", Gd.Ctor("PackedStringArray", parts)))) : Gv.S("zatím nic nesanitováno"));
    return Gv.Null;
  }
  public Gv faults_step(Gv dt = default(Gv)) {
    dt = (dt).AsF();
    Gv S = get_sim().G("S");
    Gv F = S.G("faults");
    if (!(F.G("on")).T()) {
      return Gv.Null;
    }
    if (Gv.Le(F.G("next"), Gv.I(0L))) {
      F = F.S("next", (S.G("t") + Gd.randf_range(Gv.I(1800L), Gv.I(4800L))));
    }
    if (Gv.Le(F.G("nextMinor"), Gv.I(0L))) {
      F = F.S("nextMinor", (S.G("t") + Gd.randf_range(Gv.I(900L), Gv.I(2400L))));
    }
    if (Gv.Ge(S.G("t"), F.G("nextMinor"))) {
      F = F.S("nextMinor", (S.G("t") + Gd.randf_range(Gv.I(1500L), Gv.I(3600L))));
      Gv i = ((Gd.randi() % MINOR.C("size"))).AsI();
      get_sim().C("logm", (Gv.S("ℹ ") + MINOR.I(i)), Gv.S("warn"));
      if (Gv.Eq(i, Gv.I(3L))) {
        S = S.S("sensOff", Gv.F(1.5));
      }
    }
    if (Gv.Lt(S.G("t"), F.G("next"))) {
      return Gv.Null;
    }
    F = F.S("next", (S.G("t") + Gd.randf_range(Gv.I(2400L), Gv.I(6000L))));
    Gv k = Gd.Arr(Gv.S("air"), Gv.S("steam"), Gv.S("plate"), Gv.S("valve")).I((Gd.randi() % Gv.I(4L)));
    {
      Gv _t1 = k;
      if (Gv.Eq(_t1, Gv.S("air"))) {
        Gv cand = Gd.Arr();
        foreach (Gv p in (Gd.Arr(Gv.S("p1"), Gv.S("p2"), Gv.S("p3"))).Iter()) {
          if (((S.I(p).G("on")).T() && Gv.Gt(S.I(p).G("q"), Gv.I(5L)))) {
            cand.C("append", p);
          }
        }
        if ((cand.C("size")).T()) {
          Gv p_2 = cand.I((Gd.randi() % cand.C("size")));
          S = S.SI(p_2, S.I(p_2).S("airLock", Gv.B(true)));
          S = S.SI(p_2, S.I(p_2).S("prime", Gv.F(0.05)));
          F.G("active").C("append", Gd.Dict(Gv.S("k"), Gv.S("air"), Gv.S("p"), p_2, Gv.S("t"), S.G("t")));
          get_sim().C("logm", (Gv.S("⚠ %s: průtok náhle spadl, čerpadlo hučí naprázdno – zavzdušnilo se.") % get_sim().G("main").C("pump_name", p_2)), Gv.S("bad"));
        }
      }
      else if (Gv.Eq(_t1, Gv.S("steam"))) {
        if (!(S.C("has", Gv.S("steamBar0"))).T()) {
          S = S.S("steamBar0", S.G("cfg").G("steamBar"));
        }
        S = S.S("cfg", S.G("cfg").S("steamBar", Gv.F(1.0)));
        F.G("active").C("append", Gd.Dict(Gv.S("k"), Gv.S("steam"), Gv.S("t"), S.G("t")));
        get_sim().C("logm", Gv.S("⚠ Kotel: tlak páry klesá na 1,0 bar – ohřev zpomalil."), Gv.S("bad"));
      }
      else if (Gv.Eq(_t1, Gv.S("plate"))) {
        if (!(S.C("has", Gv.S("UA10"))).T()) {
          S = S.S("UA10", S.G("cfg").G("UA1"));
        }
        S = S.S("cfg", S.G("cfg").S("UA1", (S.G("UA10") * Gv.F(0.45))));
        F.G("active").C("append", Gd.Dict(Gv.S("k"), Gv.S("plate"), Gv.S("t"), S.G("t")));
        get_sim().C("logm", Gv.S("⚠ Deskový chladič: mladina za chladičem je teplejší než obvykle při stejném průtoku…"), Gv.S("warn"));
      }
      else if (Gv.Eq(_t1, Gv.S("valve"))) {
        Gv vs = Gd.Arr();
        foreach (Gv v in (get_sim().G("main").G("items")).Iter()) {
          if (((Gv.Eq(v.G("type"), Gv.S("valve")) && Gv.Eq(v.C("get", Gv.S("kind"), Gv.S("")), Gv.S("klapka"))) && !(v.C("get", Gv.S("stuck"), Gv.B(false))).T())) {
            vs.C("append", v);
          }
        }
        if ((vs.C("size")).T()) {
          Gv v_2 = vs.I((Gd.randi() % vs.C("size")));
          v_2 = v_2.S("stuck", Gv.B(true));
          v_2 = v_2.S("stuckOpen", Gv.B(Gv.Ne(v_2.C("get", Gv.S("open"), Gv.B(true)), Gv.B(false))));
          F.G("active").C("append", Gd.Dict(Gv.S("k"), Gv.S("valve"), Gv.S("id"), v_2.G("id"), Gv.S("t"), S.G("t")));
        }
      }
    }
    return Gv.Null;
  }
  public Gv fault_fix(Gv k = default(Gv)) {
    Gv S = get_sim().G("S");
    Gv F = S.G("faults");
    Gv msg = Gv.S("");
    foreach (Gv f in (F.G("active").C("duplicate")).Iter()) {
      if (Gv.Ne(f.G("k"), k)) {
        continue;
      }
      {
        Gv _t1 = k;
        if (Gv.Eq(_t1, Gv.S("air"))) {
          S = S.SI(f.G("p"), S.I(f.G("p")).S("airLock", Gv.B(false)));
          msg = (Gv.S("%s odvzdušněno – čerpadlo znovu nasaje.") % get_sim().G("main").C("pump_name", f.G("p")));
        }
        else if (Gv.Eq(_t1, Gv.S("steam"))) {
          S = S.S("cfg", S.G("cfg").S("steamBar", S.C("get", Gv.S("steamBar0"), Gv.F(2.5))));
          msg = (Gv.S("Kotel: odkalení a reset hořáku – tlak páry zpět na %s bar.") % get_sim().C("fmt1", S.G("cfg").G("steamBar")));
        }
        else if (Gv.Eq(_t1, Gv.S("plate"))) {
          S = S.S("cfg", S.G("cfg").S("UA1", S.C("get", Gv.S("UA10"), S.G("cfg").G("UA1"))));
          msg = Gv.S("Chladič rozebrán a vyčištěn – přestup tepla obnoven.");
        }
        else if (Gv.Eq(_t1, Gv.S("valve"))) {
          foreach (Gv v in (get_sim().G("main").G("items")).Iter()) {
            if (Gv.Eq(v.G("id"), f.G("id"))) {
              v.C("erase", Gv.S("stuck"));
            }
          }
          msg = Gv.S("Zadřená klapka uvolněna a promazána.");
        }
      }
      F.G("active").C("erase", f);
      S = S.S("faults", S.G("faults").S("solved", (S.G("faults").C("get", Gv.S("solved"), Gv.I(0L)) + Gv.I(1L))));
      get_sim().C("logm", ((Gv.S("✔ ") + msg) + (Gv.S(" (za %d min)") % Gd.roundi((((S.G("t") - f.G("t"))) / Gv.F(60.0))))));
    }
    if (Gv.Eq(k, Gv.S("air"))) {
      foreach (Gv p in (Gd.Arr(Gv.S("p1"), Gv.S("p2"), Gv.S("p3"))).Iter()) {
        if ((S.I(p).C("get", Gv.S("airLock"), Gv.B(false))).T()) {
          S = S.SI(p, S.I(p).S("airLock", Gv.B(false)));
          msg = (Gv.S("%s odvzdušněno.") % get_sim().G("main").C("pump_name", p));
          get_sim().C("logm", (Gv.S("✔ ") + msg));
        }
      }
    }
    if ((Gv.Eq(k, Gv.S("sensor")) && Gv.Ne(S.G("sensOff"), Gv.F(0.0)))) {
      S = S.S("sensOff", Gv.F(0.0));
      msg = Gv.S("Teploměr pánve zkalibrován.");
      get_sim().C("logm", (Gv.S("✔ ") + msg));
    }
    return msg;
    return Gv.Null;
  }
  public Gv faults_text() {
    Gv F = get_sim().G("S").G("faults");
    if (!(F.G("on")).T()) {
      return Gv.S("vypnuto");
    }
    Gv n = (F.G("active").C("size")).AsI();
    return (Gv.S("zapnuto · aktivní poruchy: %d · vyřešeno: %d") % Gd.Arr(n, F.C("get", Gv.S("solved"), Gv.I(0L))));
    return Gv.Null;
  }
  public Gv scorecard() {
    Gv S = get_sim().G("S");
    Gv c = S.G("cfg");
    Gv rep = get_sim().C("spReport");
    Gv out_ = Gd.Arr();
    Gv sc = Gd.Fn((_a) => { Gv label = Gd.A(_a, 0); Gv val = Gd.A(_a, 1); Gv target = Gd.A(_a, 2); Gv pts = Gd.A(_a, 3); out_.C("append", Gd.Arr(label, val, target, Gd.clampf(pts, Gv.F(0.0), Gv.F(100.0)))); return Gv.Null; });
    if (Gv.Ne(rep, Gv.Null)) {
      sc.C("call", Gv.S("Objem mladiny"), (Gv.S("%s hl") % get_sim().C("fmt1", rep.G("V"))), (Gv.S("%s hl") % get_sim().C("fmt1", c.G("targetV"))), (Gv.F(100.0) - ((Gd.absf((rep.G("V") - c.G("targetV"))) / c.G("targetV")) * Gv.F(400.0))));
      sc.C("call", Gv.S("Stupňovitost"), (Gv.S("%s °P") % get_sim().C("fmt1", rep.G("P"))), (Gv.S("%s °P") % get_sim().C("fmt1", c.G("targetP"))), (Gv.F(100.0) - (Gd.absf((rep.G("P") - c.G("targetP"))) * Gv.F(60.0))));
      sc.C("call", Gv.S("Hořkost"), (Gv.S("%d IBU") % Gd.roundi(rep.G("IBU"))), (Gv.S("%d IBU") % Gd.roundi(c.G("targetIBU"))), (Gv.F(100.0) - (Gd.absf((rep.G("IBU") - c.G("targetIBU"))) * Gv.F(6.0))));
      sc.C("call", Gv.S("Barva"), (Gv.S("%s EBC") % get_sim().C("fmt1", rep.G("EBC"))), (Gv.S("%s EBC") % get_sim().C("fmt1", c.G("targetEBC"))), (Gv.F(100.0) - (Gd.absf((rep.G("EBC") - c.G("targetEBC"))) * Gv.F(12.0))));
      sc.C("call", Gv.S("DMS"), (Gv.S("%d ppb") % Gd.roundi(rep.G("dms"))), Gv.S("< 60 ppb"), (Gv.F(100.0) - (Gd.maxf(Gv.F(0.0), (rep.G("dms") - Gv.F(60.0))) * Gv.F(2.0))));
      sc.C("call", Gv.S("Zákaly (průměr sladiny)"), (Gv.S("%d") % Gd.roundi((S.G("turbSum") / Gd.maxf(Gv.F(1.0), S.G("turbVol"))))), Gv.S("< 20"), (Gv.F(100.0) - (Gd.maxf(Gv.F(0.0), ((S.G("turbSum") / Gd.maxf(Gv.F(1.0), S.G("turbVol"))) - Gv.F(20.0))) * Gv.F(3.0))));
      sc.C("call", Gv.S("Teplota zakvašení"), (Gv.S("%s °C") % get_sim().C("fmt1", rep.G("T"))), (Gv.S("%s °C") % get_sim().C("fmt1", c.G("pitchT"))), (Gv.F(100.0) - (Gd.absf((rep.G("T") - c.G("pitchT"))) * Gv.F(25.0))));
      sc.C("call", Gv.S("Výtěžnost do spilky (extrakt ve spilce / slad)"), (Gv.S("%s %%") % get_sim().C("fmt1", rep.G("yield"))), Gv.S("≥ 68 %"), (Gv.F(100.0) - (Gd.maxf(Gv.F(0.0), (Gv.F(68.0) - rep.G("yield"))) * Gv.F(8.0))));
      if ((rep.C("has", Gv.S("VV_varna"))).T()) {
        sc.C("call", Gv.S("Výtěžnost varny (extrakt ve vyrážce / slad)"), (Gv.S("%s %%  (provozně z objemu: %s %%)") % Gd.Arr(get_sim().C("fmt1", rep.G("VV_varna")), get_sim().C("fmt1", rep.G("VV_cz")))), Gv.S("≥ 72 %"), (Gv.F(100.0) - (Gd.maxf(Gv.F(0.0), (Gv.F(72.0) - rep.G("VV_varna"))) * Gv.F(8.0))));
        sc.C("call", Gv.S("Využití laboratorního extraktu"), (Gv.S("%s %%") % get_sim().C("fmt1", rep.G("ucinnost"))), Gv.S("≥ 92 %"), (Gv.F(100.0) - (Gd.maxf(Gv.F(0.0), (Gv.F(92.0) - rep.G("ucinnost"))) * Gv.F(6.0))));
      }
    }
    Gv lost = ((S.G("drainL") + S.G("spillL"))).AsF();
    sc.C("call", Gv.S("Ztráty do kanálu"), (Gv.S("%d L") % Gd.roundi(lost)), Gv.S("0 L"), (Gv.F(100.0) - (lost / Gv.F(10.0))));
    Gv bad = Gv.I(0L);
    foreach (Gv l in (S.G("log")).Iter()) {
      if (Gv.Eq(l.G("type"), Gv.S("bad"))) {
        bad = (bad + Gv.I(1L));
      }
    }
    sc.C("call", Gv.S("Chyby obsluhy (červená hlášení)"), (Gv.S("%d") % bad), Gv.S("0"), (Gv.F(100.0) - (bad * Gv.F(12.0))));
    Gv pr = S.C("get", Gv.S("practice"), Gd.Dict(Gv.S("n"), Gv.I(0L), Gv.S("rules"), Gd.Dict()));
    sc.C("call", Gv.S("Provozní zásady (pořadí klapek, čerpadel, páry)"), (Gv.S("%d porušení") % Gd.Dyn("int", pr.G("n"))), Gv.S("0"), (Gv.F(100.0) - (Gd.Dyn("int", pr.G("n")) * Gv.F(10.0))));
    sc.C("call", Gv.S("Prodleva 52 °C / 63 °C / 72 °C"), (Gv.S("%d / %d / %d min") % Gd.Arr(Gd.roundi(S.G("flags").G("rest52")), Gd.roundi((S.G("flags").G("r63") + S.G("flags").G("r63b"))), Gd.roundi((S.G("flags").G("r72") + S.G("flags").G("r72b"))))), Gv.S("15 / 40 / 40 min"), (((Gv.F(100.0) - (Gd.absf((S.G("flags").G("rest52") - Gv.F(15.0))) * Gv.F(3.0))) - (Gd.absf(((S.G("flags").G("r63") + S.G("flags").G("r63b")) - Gv.F(40.0))) * Gv.F(1.5))) - (Gd.absf(((S.G("flags").G("r72") + S.G("flags").G("r72b")) - Gv.F(40.0))) * Gv.F(1.5))));
    Gv ra = S.C("get", Gv.S("restAcc"), Gd.Dict(Gv.S("in"), Gv.F(0.0), Gv.S("tot"), Gv.F(0.0), Gv.S("dev"), Gv.F(0.0)));
    if (Gv.Gt(ra.G("tot"), Gv.F(60.0))) {
      Gv pin = (((Gv.F(100.0) * ra.I(Gv.S("in"))) / ra.G("tot"))).AsF();
      sc.C("call", Gv.S("Přesnost prodlev (±0,5 °C)"), (Gv.S("%d %% času, průměrná odchylka %s °C") % Gd.Arr(Gd.roundi(pin), get_sim().C("fmt1", (ra.G("dev") / ra.G("tot"))))), Gv.S("≥ 80 %"), (pin * Gv.F(1.15)));
    }
    Gv cd = S.C("get", Gv.S("cip"), Gd.Dict()).C("get", Gv.S("done"), Gd.Dict());
    Gv lye_ok = Gv.B(false);
    foreach (Gv k in (cd).Iter()) {
      if ((Gd.Ctor("String", k).C("ends_with", Gv.S(":louh!"))).T()) {
        lye_ok = Gv.B(true);
      }
    }
    sc.C("call", Gv.S("Sanitace – horký louh ≥ 75 °C 20 min"), ((lye_ok).T() ? Gv.S("splněno") : Gv.S("neprovedeno")), Gv.S("splněno"), ((lye_ok).T() ? Gv.F(100.0) : Gv.F(0.0)));
    Gv hrs = ((S.G("t") / Gv.F(3600.0))).AsF();
    sc.C("call", Gv.S("Časová efektivita várky"), (Gv.S("%s h") % get_sim().C("fmt1", hrs)), Gv.S("≤ 10 h"), (Gv.Ne(rep, Gv.Null) ? (Gv.F(100.0) - (Gd.maxf(Gv.F(0.0), (hrs - Gv.F(10.0))) * Gv.F(15.0))) : Gv.F(50.0)));
    return out_;
    return Gv.Null;
  }
  public Gv _rest_acc(Gv dt = default(Gv)) {
    dt = (dt).AsF();
    Gv S = get_sim().G("S");
    if (!(S.C("has", Gv.S("restAcc"))).T()) {
      S = S.S("restAcc", Gd.Dict(Gv.S("in"), Gv.F(0.0), Gv.S("tot"), Gv.F(0.0), Gv.S("dev"), Gv.F(0.0)));
    }
    Gv cur = get_sim().G("training").C("current");
    Gv id = cur.C("get", Gv.S("id"), Gv.S(""));
    if ((!(REST_T.C("has", id)).T() || Gv.Lt(get_sim().C("vol", S.G("rvp")), Gv.I(300L)))) {
      return Gv.Null;
    }
    Gv d = (Gd.absf((S.G("rvp").G("Tsens") - REST_T.I(id)))).AsF();
    S = S.S("restAcc", S.G("restAcc").S("tot", (S.G("restAcc").G("tot") + dt)));
    S = S.S("restAcc", S.G("restAcc").S("dev", (S.G("restAcc").G("dev") + (d * dt))));
    if (Gv.Le(d, Gv.F(0.5))) {
      S = S.S("restAcc", S.G("restAcc").SI(Gv.S("in"), (S.G("restAcc").I(Gv.S("in")) + dt)));
    }
    return Gv.Null;
  }
  public Gv scorecard_text() {
    Gv rows = scorecard();
    Gv tot = Gv.F(0.0);
    Gv lines = Gd.Arr();
    foreach (Gv r in (rows).Iter()) {
      tot = (tot + r.I(Gv.I(3L)));
      Gv col = (Gv.Ge(r.I(Gv.I(3L)), Gv.I(80L)) ? Gv.S("#7ee08a") : ((Gv.Ge(r.I(Gv.I(3L)), Gv.I(50L)) ? Gv.S("#f2c230") : Gv.S("#ff6b5b"))));
      lines.C("append", (Gv.S("[color=%s]%d[/color]  %s: %s [color=#7b858e](cíl %s)[/color]") % Gd.Arr(col, Gd.roundi(r.I(Gv.I(3L))), r.I(Gv.I(0L)), r.I(Gv.I(1L)), r.I(Gv.I(2L)))));
    }
    Gv avg = ((tot / Gd.maxf(Gv.I(1L), rows.C("size")))).AsF();
    Gv grade = (Gv.Ge(avg, Gv.I(85L)) ? Gv.S("výborně") : ((Gv.Ge(avg, Gv.I(70L)) ? Gv.S("dobře") : ((Gv.Ge(avg, Gv.I(50L)) ? Gv.S("dostatečně") : Gv.S("nedostatečně"))))));
    return ((Gv.S("[b]Celkem %d / 100 – %s[/b]\n") % Gd.Arr(Gd.roundi(avg), grade)) + Gv.S("\n").C("join", Gd.Ctor("PackedStringArray", lines)));
    return Gv.Null;
  }
  public Gv step(Gv dt = default(Gv)) {
    dt = (dt).AsF();
    init_state();
    Gv S = get_sim().G("S");
    if (Gv.Gt(S.G("malt").G("added"), Gv.I(1L))) {
      Gv ph = (mash_ph()).AsF();
      S = S.S("mashPH", ph);
      S = S.S("pH", ph);
      S = S.S("phF", ph_factors(ph));
      if (((((Gv.Lt(ph, Gv.F(5.2)) || Gv.Gt(ph, Gv.F(5.8))) && Gv.Gt(S.G("malt").G("added"), Gv.I(100L))) && Gv.Eq(S.G("sk").G("mode"), Gv.S("mash"))) && (Gv.Gt(S.G("rvp").G("g"), Gv.I(20L)) || Gv.Gt(S.G("sk").G("mash").G("g"), Gv.I(20L))))) {
        get_sim().C("once", (Gv.S("ph%d") % get_sim().C("tb", Gv.I(1800L))), (Gv.S("pH rmutu %s je mimo optimum 5,4–5,6 – amylázy pracují pomaleji, zcukření se protáhne. Uprav vodu nebo kyselinu mléčnou.") % get_sim().C("fmt1", ph)), Gv.S("warn"));
      }
    }
    fer_step(dt);
    cip_step(dt);
    faults_step(dt);
    _rest_acc(dt);
    if ((((Gv.Gt(S.G("sp").G("L"), Gv.I(100L)) && !(S.G("fer").G("on")).T()) && (S.G("flags").C("get", Gv.S("pushed"), Gv.I(0L))).T()) && Gv.Lt(S.G("p3").G("q"), Gv.I(1L)))) {
      fer_start();
    }
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "simulation_ref": return this.simulation_ref;
    case "sim": return get_sim();
    case "WATERS": return S_proc.WATERS;
    case "CIP_TARGETS": return S_proc.CIP_TARGETS;
    case "CIP_MEDIA": return S_proc.CIP_MEDIA;
    case "MINOR": return S_proc.MINOR;
    case "REST_T": return S_proc.REST_T;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "simulation_ref": this.simulation_ref = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "_init": return _init(Gd.A(a, 0));
    case "init_state": return init_state();
    case "residual_alk": return residual_alk(Gd.A(a, 0));
    case "mash_ph": return mash_ph();
    case "ph_factors": return ph_factors(Gd.A(a, 0));
    case "fer_start": return fer_start();
    case "fer_step": return fer_step(Gd.A(a, 0));
    case "fer_skip": return fer_skip(Gd.A(a, 0));
    case "app_extract": return app_extract();
    case "cip_name": return cip_name(Gd.A(a, 0));
    case "cip_step": return cip_step(Gd.A(a, 0));
    case "cip_status": return cip_status();
    case "faults_step": return faults_step(Gd.A(a, 0));
    case "fault_fix": return fault_fix(Gd.A(a, 0));
    case "faults_text": return faults_text();
    case "scorecard": return scorecard();
    case "_rest_acc": return _rest_acc(Gd.A(a, 0));
    case "scorecard_text": return scorecard_text();
    case "step": return step(Gd.A(a, 0));
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    case "WATERS": return WATERS;
    case "CIP_TARGETS": return CIP_TARGETS;
    case "CIP_MEDIA": return CIP_MEDIA;
    case "MINOR": return MINOR;
    case "REST_T": return REST_T;
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_proc(); o._init(Gd.A(a, 0)); return Gv.O(o); }
  public override string ScriptName { get { return "proc"; } }
}
}
