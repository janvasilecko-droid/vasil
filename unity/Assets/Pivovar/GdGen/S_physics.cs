// Vygenerováno z physics.gd překladačem gd2cs.py – needituj ručně.
using System; using System.Collections.Generic;
namespace Pivovar.Gd.Gen {
public partial class S_physics : GdObject {
  static S_physics() {
  }
  public S_physics() {
  }
  public static Gv fraction(Gv rate_per_min = default(Gv), Gv minutes = default(Gv)) {
    rate_per_min = (rate_per_min).AsF();
    minutes = (minutes).AsF();
    return ((Gv.F(1.0) - Gd.exp(((-Gd.maxf(Gv.F(0.0), rate_per_min)) * Gd.maxf(Gv.F(0.0), minutes))))).AsF();
    return Gv.Null;
  }
  public static Gv chain(Gv a = default(Gv), Gv b = default(Gv), Gv k1 = default(Gv), Gv k2 = default(Gv), Gv minutes = default(Gv)) {
    a = (a).AsF();
    b = (b).AsF();
    k1 = (k1).AsF();
    k2 = (k2).AsF();
    minutes = (minutes).AsF();
    Gv ea = Gd.exp(((-k1) * minutes));
    Gv eb = Gd.exp(((-k2) * minutes));
    Gv formed = Gv.F(0.0);
    if (Gv.Lt(Gd.absf((k1 - k2)), Gv.F(1e-10))) {
      formed = ((((k1 * a) * minutes) * ea)).AsF();
    } else {
      formed = ((((k1 * a) * ((ea - eb))) / ((k2 - k1)))).AsF();
    }
    return Gd.Arr((a * ea), ((b * eb) + formed));
    return Gv.Null;
  }
  public static Gv exchanger_effectiveness(Gv ua = default(Gv), Gv hot_capacity = default(Gv), Gv cold_capacity = default(Gv)) {
    ua = (ua).AsF();
    hot_capacity = (hot_capacity).AsF();
    cold_capacity = (cold_capacity).AsF();
    Gv cmin = Gd.minf(hot_capacity, cold_capacity);
    Gv cmax = Gd.maxf(hot_capacity, cold_capacity);
    if ((Gv.Le(cmin, Gv.F(1e-09)) || Gv.Le(ua, Gv.F(0.0)))) {
      return (Gv.F(0.0)).AsF();
    }
    Gv cr = (cmin / cmax);
    Gv ntu = (ua / cmin);
    if (Gv.Lt(Gd.absf((Gv.F(1.0) - cr)), Gv.F(1e-07))) {
      return ((ntu / ((Gv.F(1.0) + ntu)))).AsF();
    }
    Gv x = Gd.exp(((-ntu) * ((Gv.F(1.0) - cr))));
    return (Gd.clampf((((Gv.F(1.0) - x)) / ((Gv.F(1.0) - (cr * x)))), Gv.F(0.0), Gv.F(1.0))).AsF();
    return Gv.Null;
  }
  public static Gv pipe_capacity(Gv length_m = default(Gv), Gv bore_mm = default(Gv)) {
    length_m = (length_m).AsF();
    bore_mm = (bore_mm).AsF();
    return (((((Gd.K("PI") * Gd.pow((bore_mm / Gv.F(1000.0)), Gv.F(2.0))) / Gv.F(4.0)) * length_m) * Gv.F(1000.0))).AsF();
    return Gv.Null;
  }
  public static Gv exchange_bodies(Gv t1 = default(Gv), Gv c1 = default(Gv), Gv t2 = default(Gv), Gv c2 = default(Gv), Gv ua = default(Gv), Gv seconds = default(Gv)) {
    t1 = (t1).AsF();
    c1 = (c1).AsF();
    t2 = (t2).AsF();
    c2 = (c2).AsF();
    ua = (ua).AsF();
    seconds = (seconds).AsF();
    if ((Gv.Le(c1, Gv.F(1e-09)) || Gv.Le(c2, Gv.F(1e-09)))) {
      return Gd.Arr(t1, t2);
    }
    Gv equilibrium = ((((t1 * c1) + (t2 * c2))) / ((c1 + c2)));
    Gv decay = Gd.exp((((-Gd.maxf(Gv.F(0.0), ua)) * (((Gv.F(1.0) / c1) + (Gv.F(1.0) / c2)))) * seconds));
    return Gd.Arr((equilibrium + (((t1 - equilibrium)) * decay)), (equilibrium + (((t2 - equilibrium)) * decay)));
    return Gv.Null;
  }
  public static Gv pipe_temperature(Gv temperature = default(Gv), Gv ambient = default(Gv), Gv u_w_m2k = default(Gv), Gv length_m = default(Gv), Gv bore_mm = default(Gv), Gv heat_capacity_kj_k = default(Gv), Gv seconds = default(Gv)) {
    temperature = (temperature).AsF();
    ambient = (ambient).AsF();
    u_w_m2k = (u_w_m2k).AsF();
    length_m = (length_m).AsF();
    bore_mm = (bore_mm).AsF();
    heat_capacity_kj_k = (heat_capacity_kj_k).AsF();
    seconds = (seconds).AsF();
    if (Gv.Le(heat_capacity_kj_k, Gv.F(1e-09))) {
      return (temperature).AsF();
    }
    Gv ua = (((((u_w_m2k * Gd.K("PI")) * bore_mm) / Gv.F(1000.0)) * length_m) / Gv.F(1000.0));
    return ((ambient + (((temperature - ambient)) * Gd.exp((((-ua) * seconds) / heat_capacity_kj_k))))).AsF();
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "fraction": return S_physics.fraction(Gd.A(a, 0), Gd.A(a, 1));
    case "chain": return S_physics.chain(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4));
    case "exchanger_effectiveness": return S_physics.exchanger_effectiveness(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "pipe_capacity": return S_physics.pipe_capacity(Gd.A(a, 0), Gd.A(a, 1));
    case "exchange_bodies": return S_physics.exchange_bodies(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5));
    case "pipe_temperature": return S_physics.pipe_temperature(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6));
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "fraction": return fraction(Gd.A(a, 0), Gd.A(a, 1));
    case "chain": return chain(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4));
    case "exchanger_effectiveness": return exchanger_effectiveness(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "pipe_capacity": return pipe_capacity(Gd.A(a, 0), Gd.A(a, 1));
    case "exchange_bodies": return exchange_bodies(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5));
    case "pipe_temperature": return pipe_temperature(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6));
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_physics(); return Gv.O(o); }
  public override string ScriptName { get { return "physics"; } }
}
}
