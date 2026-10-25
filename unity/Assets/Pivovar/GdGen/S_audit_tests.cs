// Vygenerováno z audit_tests.gd překladačem gd2cs.py – needituj ručně.
using System; using System.Collections.Generic;
namespace Pivovar.Gd.Gen {
public partial class S_audit_tests : GdObject {
  public Gv T;
  public Gv sim;
  public Gv m;
  static S_audit_tests() {
  }
  public S_audit_tests() {
    T = Gv.Null;
    sim = Gv.Null;
    m = Gv.Null;
  }
  public Gv run(Gv t = default(Gv)) {
    T = t;
    sim = t.G("sim");
    m = t.G("m");
    _energy();
    _plate();
    _hlt_boil();
    _lauter();
    _pumps();
    _protlacka();
    _process();
    _ui_regress();
    _practice_tests();
    _physics_lit_tests();
    _spec_tests();
    _steel_tests();
    _calib_tests();
    _ferm_cip_tests();
    _mains_tests();
    _reference_tests();
    _whirl_tests();
    return Gv.Null;
  }
  public Gv _open(Gv a = default(Gv), Gv b = default(Gv), Gv pumps = default(Gv), Gv where = default(Gv)) {
    if (where.k == 0) where = Gv.I(-1L);
    Gv vv = sim.C("route_valves", a, b, pumps, where);
    if (Gv.Eq(vv, Gv.Null)) {
      return Gv.Null;
    }
    foreach (Gv id in (vv).Iter()) {
      if ((m.G("outlet_open").C("has", id)).T()) {
        m = m.S("outlet_open", m.G("outlet_open").SI(id, Gv.B(true)));
      } else if ((sim.G("net").G("byId").C("has", id)).T()) {
        sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(id, sim.G("net").G("byId").I(id).SI(Gv.S("open"), Gv.B(true)))));
      }
    }
    sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
    return Gv.Null;
  }
  public Gv _energy() {
    T.C("fresh");
    Gv S = sim.G("S");
    Gv c = S.G("cfg");
    Gv v = S.G("rvp");
    v = v.S("w", Gv.F(2000.0));
    v = v.S("T", Gv.F(20.0));
    v = v.S("Tsens", Gv.F(20.0));
    v = v.S("stir", Gv.B(true));
    v = v.S("heat", Gv.F(0.6));
    Gv Ein = Gv.F(0.0);
    Gv Eloss = Gv.F(0.0);
    Gv H0 = ((((sim.C("cC", v) + sim.C("steelC", c.G("rvpSteel")))) * v.G("T"))).AsF();
    foreach (Gv i in (Gv.I(1500L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
      Ein = (Ein + (v.G("Pin") * Gv.F(1.0)));
      Eloss = (Eloss + ((c.G("rvpUA") * ((v.G("T") - c.G("ambientT")))) * Gv.F(1.0)));
      if (Gv.Gt(v.G("T"), Gv.F(85.0))) {
        break;
      }
    }
    Gv H1 = ((((sim.C("cC", v) + sim.C("steelC", c.G("rvpSteel")))) * v.G("T"))).AsF();
    Gv err = ((Gd.absf((((Ein - Eloss)) - ((H1 - H0)))) / Gd.maxf(Gv.F(1.0), Ein))).AsF();
    T.C("chk", Gv.S("AUDIT energie: pára do pláště = ohřev vody a oceli + ztráty (1. zákon)"), Gv.B(Gv.Lt(err, Gv.F(0.02))), (Gv.S("%.2f %% (dodáno %d MJ)") % Gd.Arr((err * Gv.F(100.0)), Gd.roundi((Ein / Gv.F(1000.0))))));
    Gv rate = ((((v.G("T") - Gv.F(20.0))) / ((Gv.F(1500.0) / Gv.F(60.0))))).AsF();
    T.C("chk", Gv.S("AUDIT ohřev 2000 L páru 60 %: realistická rychlost 0,5–3 K/min"), Gv.B((Gv.Gt(rate, Gv.F(0.5)) && Gv.Lt(rate, Gv.F(3.0)))), (Gv.S("%.2f K/min") % rate));
    v = v.S("w", Gv.F(4500.0));
    v = v.S("T", sim.G("S").G("cfg").G("tbWater"));
    v = v.S("heat", Gv.F(0.85));
    Gv w0 = (v.G("w")).AsF();
    Gv E2 = Gv.F(0.0);
    Gv L2 = Gv.F(0.0);
    foreach (Gv i_2 in (Gv.I(600L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
      E2 = (E2 + v.G("Pin"));
      L2 = (L2 + (c.G("rvpUA") * ((v.G("T") - c.G("ambientT")))));
    }
    Gv evap = ((w0 - v.G("w"))).AsF();
    Gv err2 = ((Gd.absf((((E2 - L2)) - (evap * Gv.F(2257.0)))) / Gd.maxf(Gv.F(1.0), E2))).AsF();
    T.C("chk", Gv.S("AUDIT var: přebytek tepla = odpařená voda × 2257 kJ/kg"), Gv.B(Gv.Lt(err2, Gv.F(0.03))), (Gv.S("odpar %.0f L/h, chyba %.1f %%") % Gd.Arr((evap * Gv.F(6.0)), (err2 * Gv.F(100.0)))));
    Gv pct_h = ((((evap * Gv.F(6.0)) / w0) * Gv.F(100.0))).AsF();
    T.C("chk", Gv.S("AUDIT var plné pánve (4500 L, pára 85 %): odpar 4–10 % objemu za hodinu"), Gv.B((Gv.Gt(pct_h, Gv.F(4.0)) && Gv.Lt(pct_h, Gv.F(10.0)))), (Gv.S("%.1f %%/h") % pct_h));
    Gv Ts = Gd.Arr();
    foreach (Gv dt in (Gd.Arr(Gv.F(1.0), Gv.F(5.0))).Iter()) {
      T.C("fresh");
      Gv w = sim.G("S").G("rvp");
      w = w.S("w", Gv.F(2000.0));
      w = w.S("T", Gv.F(20.0));
      w = w.S("Tsens", Gv.F(20.0));
      w = w.S("stir", Gv.B(true));
      w = w.S("heat", Gv.F(0.6));
      foreach (Gv i_3 in (Gd.Dyn("int", (Gv.F(1200.0) / dt))).Iter()) {
        sim.C("sim_step", dt);
      }
      Ts.C("append", w.G("T"));
    }
    T.C("chk", Gv.S("AUDIT ohřev nezávisí na časovém kroku (1 s vs 5 s)"), Gv.B(Gv.Lt(Gd.absf((Ts.I(Gv.I(0L)) - Ts.I(Gv.I(1L)))), Gv.F(0.3))), (Gv.S("%.2f vs %.2f °C") % Gd.Arr(Ts.I(Gv.I(0L)), Ts.I(Gv.I(1L)))));
    return Gv.Null;
  }
  public Gv _plate() {
    T.C("fresh");
    Gv S = sim.G("S");
    Gv pid = Gv.S("");
    foreach (Gv it in (m.G("items")).Iter()) {
      if ((Gv.Eq(it.G("type"), Gv.S("vessel")) && Gv.Eq(it.C("get", Gv.S("shape"), Gv.S("")), Gv.S("plate")))) {
        pid = it.G("id");
      }
    }
    if (Gv.Eq(pid, Gv.S(""))) {
      T.C("chk", Gv.S("AUDIT chladič existuje v plánku"), Gv.B(false));
      return Gv.Null;
    }
    S = S.S("plates", S.G("plates").SI(pid, Gd.Dict(Gv.S("kind"), Gv.S("water"), Gv.S("T"), Gv.F(12.0), Gv.S("flow"), Gv.F(200.0))));
    Gv Tout = (sim.C("plate_cool", pid, Gv.F(95.0), Gv.F(150.0), (Gv.F(1.0) / Gv.F(60.0)))).AsF();
    Gv Tc = (S.G("plates").I(pid).G("Tcout")).AsF();
    Gv Qw = (((((Gv.F(150.0) / Gv.F(60.0)) * Gv.F(1.04)) * Gv.F(4.0)) * ((Gv.F(95.0) - Tout)))).AsF();
    Gv Qc = ((((Gv.F(200.0) / Gv.F(60.0)) * Gv.F(4.19)) * ((Tc - Gv.F(12.0))))).AsF();
    T.C("chk", Gv.S("AUDIT chladič: teplo mladiny = teplo chladiva (1. zákon)"), Gv.B(Gv.Lt(Gd.absf((Qw - Qc)), (Gv.F(0.01) * Qw))), (Gv.S("%.1f vs %.1f kW") % Gd.Arr(Qw, Qc)));
    T.C("chk", Gv.S("AUDIT chladič: mladina se neochladí pod vstup chladiva a chladivo neohřeje nad mladinu (2. zákon)"), Gv.B((Gv.Ge(Tout, Gv.F(12.0)) && Gv.Le(Tc, Gv.F(95.0)))), (Gv.S("mladina %.1f °C, voda %.1f °C") % Gd.Arr(Tout, Tc)));
    return Gv.Null;
  }
  public Gv _hlt_boil() {
    T.C("fresh");
    Gv S = sim.G("S");
    Gv h = S.G("hlt");
    h = h.S("T", (S.G("cfg").G("tbWater") - Gv.F(0.1)));
    h = h.S("heat", Gv.F(1.0));
    h = h.S("auto", Gv.B(false));
    Gv w0 = (h.G("w")).AsF();
    foreach (Gv i in (Gv.I(600L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
    }
    T.C("chk", Gv.S("AUDIT bojler: voda nepřekročí bod varu, přebytek tepla odpaří vodu"), Gv.B((Gv.Le(h.G("T"), (S.G("cfg").G("tbWater") + Gv.F(0.01))) && Gv.Lt(h.G("w"), (w0 - Gv.F(1.0))))), (Gv.S("%.2f °C, odpar %.1f L/10 min") % Gd.Arr(h.G("T"), (w0 - h.G("w")))));
    return Gv.Null;
  }
  public Gv _draw_run(Gv minutes = default(Gv), Gv Q = default(Gv), Gv rake = default(Gv), Gv rakeH = default(Gv)) {
    minutes = (minutes).AsF();
    Q = (Q).AsF();
    if (rakeH.k == 0) rakeH = Gv.F(0.4);
    Gv S = sim.G("S");
    Gv sk = S.G("sk");
    sk = sk.S("rake", rake);
    sk = sk.S("rakeH", rakeH);
    sk = sk.S("hydT", Gv.Null);
    Gv dp0 = Gv.F(-1.0);
    Gv dpmax = Gv.F(0.0);
    Gv got = Gv.F(0.0);
    foreach (Gv i in (Gd.Dyn("int", (minutes * Gv.F(6.0)))).Iter()) {
      sim.C("sim_step", Gv.F(10.0));
      sk = sk.S("Qact", Gv.F(0.0));
      sim.C("skQmax");
      Gv p = sim.C("skDraw", Q, (Gv.F(10.0) / Gv.F(60.0)));
      got = (got + sim.C("liqVol", p));
      sim.C("skAddWater", sim.C("liqVol", p), Gv.F(77.0), Gv.B(false));
      if (Gv.Lt(dp0, Gv.I(0L))) {
        dp0 = sk.G("dp");
      }
      dpmax = Gd.maxf(dpmax, sk.G("dp"));
    }
    return Gd.Dict(Gv.S("dp0"), dp0, Gv.S("dp"), sk.G("dp"), Gv.S("dpmax"), dpmax, Gv.S("c"), sk.G("c"), Gv.S("Qmax"), sk.G("Qmax"), Gv.S("got"), got, Gv.S("teig"), sk.G("teig"));
    return Gv.Null;
  }
  public Gv _lauter() {
    T.C("bed");
    Gv sk = sim.G("S").G("sk");
    sim.C("skAddWater", Gv.F(800.0), Gv.F(77.0), Gv.B(false));
    sk = sk.S("settle", Gv.F(30.0));
    sim.C("skQmax");
    Gv q0 = (sk.G("Qmax")).AsF();
    sk = sk.S("Qact", Gv.F(0.0));
    sim.C("skDraw", Gv.F(40.0), Gv.F(0.05));
    T.C("chk", Gv.S("AUDIT lože: čerstvé lože při 40 L/min má Δp 15–90 mbar"), Gv.B((Gv.Gt(sk.G("dp"), Gv.F(15.0)) && Gv.Lt(sk.G("dp"), Gv.F(90.0)))), (Gv.S("%.0f mbar (Qmax %.0f L/min)") % Gd.Arr(sk.G("dp"), q0)));
    T.C("bed");
    sim.C("skAddWater", Gv.F(800.0), Gv.F(77.0), Gv.B(false));
    sim = sim.S("S", sim.G("S").S("sk", sim.G("S").G("sk").S("settle", Gv.F(30.0))));
    Gv a = _draw_run(Gv.F(90.0), Gv.F(40.0), Gv.B(true));
    T.C("chk", Gv.S("AUDIT scezování 40 L/min s mělkým kypřením 90 min: Δp pod 100 mbar, lože se neutáhne"), Gv.B((Gv.Lt(a.G("dpmax"), Gv.F(100.0)) && Gv.Lt(a.G("c"), Gv.F(0.5)))), (Gv.S("Δp %.0f→%.0f mbar, max %.0f, c %.2f") % Gd.Arr(a.G("dp0"), a.G("dp"), a.G("dpmax"), a.G("c"))));
    T.C("bed");
    sim.C("skAddWater", Gv.F(800.0), Gv.F(77.0), Gv.B(false));
    sim = sim.S("S", sim.G("S").S("sk", sim.G("S").G("sk").S("settle", Gv.F(30.0))));
    Gv b = _draw_run(Gv.F(90.0), Gv.F(40.0), Gv.B(false));
    T.C("chk", Gv.S("AUDIT bez kypření roste Δp (horní těsto + sedání lože)"), Gv.B(Gv.Gt(b.G("dp"), (a.G("dp") * Gv.F(1.2)))), (Gv.S("%.0f vs %.0f mbar, těsto %.1f vs %.1f kg") % Gd.Arr(b.G("dp"), a.G("dp"), b.G("teig"), a.G("teig"))));
    T.C("bed");
    sim.C("skAddWater", Gv.F(800.0), Gv.F(77.0), Gv.B(false));
    sim = sim.S("S", sim.G("S").S("sk", sim.G("S").G("sk").S("settle", Gv.F(30.0))));
    Gv c = _draw_run(Gv.F(15.0), Gv.F(150.0), Gv.B(false));
    T.C("chk", Gv.S("AUDIT přetahování 150 L/min: do 15 min Δp nad 150 mbar a propustnost spadne o víc než 40 %"), Gv.B((Gv.Gt(c.G("dpmax"), Gv.F(150.0)) && Gv.Lt(c.G("Qmax"), (q0 * Gv.F(0.6))))), (Gv.S("Δp max %.0f mbar, Qmax %.0f→%.0f L/min") % Gd.Arr(c.G("dpmax"), q0, c.G("Qmax"))));
    T.C("chk", Gv.S("AUDIT přetahování: pod sítem vzniká podtlak (čerpadlo chce víc, než lože pustí)"), Gv.B(Gv.Gt(sim.G("S").G("sk").G("vac"), Gv.F(0.0))), (Gv.S("%.2f") % sim.G("S").G("sk").G("vac")));
    Gv sk2 = sim.G("S").G("sk");
    sk2 = sk2.S("Qact", Gv.F(0.0));
    sim.C("skQmax");
    sim.C("skDraw", Gv.F(30.0), Gv.F(0.05));
    Gv dp_tight = (sk2.G("dp")).AsF();
    Gv c_tight = (sk2.G("c")).AsF();
    _draw_run(Gv.F(10.0), Gv.F(0.0), Gv.B(true), Gv.F(0.7));
    sim.C("kypr");
    Gv d = _draw_run(Gv.F(2.0), Gv.F(30.0), Gv.B(true), Gv.F(0.4));
    T.C("chk", Gv.S("AUDIT zadřené lože: zastavit, hluboko nakypřit → Δp při 30 L/min výrazně klesne"), Gv.B((Gv.Lt(d.G("c"), (c_tight * Gv.F(0.5))) && Gv.Lt(d.G("dp"), (dp_tight * Gv.F(0.6))))), (Gv.S("c %.2f→%.2f, Δp %.0f→%.0f mbar") % Gd.Arr(c_tight, d.G("c"), dp_tight, d.G("dp"))));
    T.C("bed");
    sim.C("skAddWater", Gv.F(800.0), Gv.F(77.0), Gv.B(false));
    Gv s2 = sim.G("S").G("sk");
    sim.C("skQmax");
    Gv qa = (s2.G("Qmax")).AsF();
    s2 = s2.S("g", (s2.G("g") * Gv.F(1.5)));
    sim.C("skQmax");
    T.C("chk", Gv.S("AUDIT vyšší vrstva mláta (víc sladu) = menší propustnost"), Gv.B(Gv.Lt(s2.G("Qmax"), (qa * Gv.F(0.8)))), (Gv.S("%.0f → %.0f L/min") % Gd.Arr(qa, s2.G("Qmax"))));
    T.C("bed");
    sim.C("skAddWater", Gv.F(800.0), Gv.F(77.0), Gv.B(false));
    Gv s3 = sim.G("S").G("sk");
    sim.C("skQmax");
    Gv qh = (s3.G("Qmax")).AsF();
    s3 = s3.S("T", Gv.F(50.0));
    sim.C("skQmax");
    T.C("chk", Gv.S("AUDIT chladnější lože (50 °C) teče pomaleji než při 77 °C"), Gv.B(Gv.Lt(s3.G("Qmax"), (qh * Gv.F(0.85)))), (Gv.S("%.0f → %.0f L/min") % Gd.Arr(qh, s3.G("Qmax"))));
    return Gv.Null;
  }
  public Gv _pumps() {
    T.C("fresh");
    Gv S = sim.G("S");
    _open(Gv.S("rvp"), Gv.S("sk"), Gd.Arr(Gv.S("p1")));
    S = S.S("rvp", S.G("rvp").S("w", Gv.F(2500.0)));
    S = S.S("rvp", S.G("rvp").S("T", Gv.F(60.0)));
    S = S.S("rvp", S.G("rvp").S("stir", Gv.B(true)));
    S = S.S("p1", S.G("p1").S("flow", Gv.F(300.0)));
    S = S.S("p1", S.G("p1").S("on", Gv.B(true)));
    S = S.S("p1", S.G("p1").S("prime", Gv.F(1.0)));
    sim.C("net_step", Gv.F(1.0));
    Gv q1 = (S.G("p1").G("q")).AsF();
    Gv hz1 = (S.G("p1").C("get", Gv.S("hz"), Gv.F(0.0))).AsF();
    T.C("chk", Gv.S("AUDIT C1 voda pánev→SK: měnič drží 300 L/min pod 50 Hz"), Gv.B(((Gv.Gt(q1, Gv.F(270.0)) && Gv.Gt(hz1, Gv.F(10.0))) && Gv.Lt(hz1, Gv.F(50.0)))), (Gv.S("%.0f L/min, %.0f Hz, %.1f bar") % Gd.Arr(q1, hz1, S.G("p1").C("get", Gv.S("bar"), Gv.F(0.0)))));
    S = S.S("p1", S.G("p1").S("flow", Gv.F(400.0)));
    sim.C("net_step", Gv.F(1.0));
    T.C("chk", Gv.S("AUDIT C1 na 400 L/min potřebuje víc otáček"), Gv.B(Gv.Gt(S.G("p1").C("get", Gv.S("hz"), Gv.F(0.0)), hz1)), (Gv.S("%.0f Hz") % S.G("p1").C("get", Gv.S("hz"), Gv.F(0.0))));
    Gv vv = sim.C("route_valves", Gv.S("rvp"), Gv.S("sk"), Gd.Arr(Gv.S("p1")));
    Gv thr = Gv.Null;
    foreach (Gv id in (vv).Iter()) {
      Gv it = sim.G("net").G("byId").C("get", id);
      if ((Gv.Ne(it, Gv.Null) && Gv.Eq(it.G("type"), Gv.S("valve")))) {
        thr = it;
        break;
      }
    }
    if (Gv.Ne(thr, Gv.Null)) {
      thr = thr.SI(Gv.S("part"), Gv.F(15.0));
      S = S.S("p1", S.G("p1").S("flow", Gv.F(300.0)));
      sim.C("net_step", Gv.F(1.0));
      T.C("chk", Gv.S("AUDIT přivřená klapka (15 %) sníží průtok, čerpadlo jede na plné otáčky"), Gv.B(Gv.Lt(S.G("p1").G("q"), (q1 * Gv.F(0.8)))), (Gv.S("%.0f L/min, %.0f Hz") % Gd.Arr(S.G("p1").G("q"), S.G("p1").C("get", Gv.S("hz"), Gv.F(0.0)))));
      thr.C("erase", Gv.S("part"));
    }
    S = S.S("p1", S.G("p1").S("on", Gv.B(false)));
    return Gv.Null;
  }
  public Gv _protlacka() {
    T.C("fresh");
    Gv S = sim.G("S");
    Gv ids = Gd.Dict();
    Gv ex0 = Gv.F(0.0);
    Gv n = Gv.I(0L);
    foreach (Gv e__it in (sim.G("net").G("edges")).Iter()) {
      Gv e = e__it;
      if (Gv.Ge(n, Gv.I(5L))) {
        break;
      }
      Gv p = sim.C("lp");
      p = p.S("w", (e.G("cap") * Gv.F(0.88)));
      p = p.S("f", (e.G("cap") * Gv.F(0.12)));
      p = p.S("T", Gv.F(8.0));
      e = e.S("c", p.C("duplicate"));
      e = e.S("q", Gd.Arr(p));
      ids = ids.SI(e.G("id"), Gv.B(true));
      ex0 = (ex0 + p.G("f"));
      n = (n + Gv.I(1L));
    }
    S = S.SI(Gv.S("_castEdges"), ids);
    S = S.S("sp", S.G("sp").S("L", Gv.F(1000.0)));
    S = S.S("sp", S.G("sp").S("w", Gv.F(880.0)));
    S = S.S("sp", S.G("sp").S("f", Gv.F(120.0)));
    S = S.S("sp", S.G("sp").S("H", Gv.F(8000.0)));
    Gv tot0 = (((ex0 + S.G("sp").G("f")) + S.G("sp").G("n"))).AsF();
    sim.C("push");
    Gv ex1 = Gv.F(0.0);
    foreach (Gv e_2 in (sim.G("net").G("edges")).Iter()) {
      if ((ids.C("has", e_2.G("id"))).T()) {
        ex1 = (ex1 + sim.C("ext", e_2.G("c")));
      }
    }
    T.C("chk", Gv.S("AUDIT protlačka přesune extrakt z potrubí do spilky (zachování hmoty)"), Gv.B((Gv.Lt(Gd.absf((((ex1 + S.G("sp").G("f")) + S.G("sp").G("n")) - tot0)), Gv.F(1e-06)) && Gv.Lt(ex1, Gv.F(1e-09)))), (Gv.S("%.3f kg v trubkách, spilka %.3f kg") % Gd.Arr(ex1, (S.G("sp").G("f") + S.G("sp").G("n")))));
    return Gv.Null;
  }
  public Gv _process() {
    T.C("fresh");
    Gv S = sim.G("S");
    Gv P = sim.G("proc");
    S = S.S("rvp", S.G("rvp").S("w", Gv.F(2000.0)));
    S = S.S("malt", S.G("malt").S("added", Gv.F(700.0)));
    S = S.S("water", P.G("WATERS").I(Gv.I(2L)).C("duplicate"));
    S = S.S("lactic", Gv.F(0.0));
    Gv ph_hard = (P.C("mash_ph")).AsF();
    S = S.S("water", P.G("WATERS").I(Gv.I(1L)).C("duplicate"));
    Gv ph_soft = (P.C("mash_ph")).AsF();
    S = S.S("lactic", Gv.F(1000.0));
    Gv ph_acid = (P.C("mash_ph")).AsF();
    T.C("chk", Gv.S("AUDIT pH: tvrdá voda zvedá pH, měkká a kyselina mléčná snižují (Kolbach)"), Gv.B((((Gv.Gt(ph_hard, ph_soft) && Gv.Lt(ph_acid, ph_soft)) && Gv.Gt(ph_hard, Gv.F(5.8))) && Gv.Lt(ph_soft, Gv.F(5.8)))), (Gv.S("tvrdá %.2f, měkká %.2f, + kyselina %.2f") % Gd.Arr(ph_hard, ph_soft, ph_acid)));
    Gv f_bad = P.C("ph_factors", Gv.F(6.1));
    Gv f_ok = P.C("ph_factors", Gv.F(5.5));
    T.C("chk", Gv.S("AUDIT pH: amylázy jsou při pH 6,1 pomalejší než při 5,5"), Gv.B((Gv.Lt(f_bad.G("x"), f_ok.G("x")) && Gv.Lt(f_bad.G("y"), f_ok.G("y")))), (Gv.S("β %.2f vs %.2f") % Gd.Arr(f_bad.G("y"), f_ok.G("y"))));
    T.C("fresh");
    S = sim.G("S");
    P = sim.G("proc");
    Gv e = (((Gv.F(4000.0) * Gv.F(1.048)) * Gv.F(0.12))).AsF();
    S = S.S("sp", S.G("sp").S("L", Gv.F(4000.0)));
    S = S.S("sp", S.G("sp").S("w", ((Gv.F(4000.0) * Gv.F(1.048)) - e)));
    S = S.S("sp", S.G("sp").S("f", (e * Gv.F(0.78))));
    S = S.S("sp", S.G("sp").S("n", (e * Gv.F(0.22))));
    S = S.S("sp", S.G("sp").S("H", (Gv.F(9.0) * Gv.F(4000.0))));
    S = S.S("fer", S.G("fer").S("set", Gv.F(9.0)));
    P.C("fer_start");
    P.C("fer_skip", (Gv.F(24.0) * Gv.F(8.0)));
    Gv ae = (P.C("app_extract")).AsF();
    Gv abv = (((S.G("fer").G("E") / Gv.F(10.0)) / Gv.F(0.79))).AsF();
    T.C("chk", Gv.S("AUDIT kvašení 12° při 9 °C za 8 dní: zdánlivý extrakt 2–4,5 °P, alkohol 4–5,5 % obj."), Gv.B((((Gv.Gt(ae, Gv.F(2.0)) && Gv.Lt(ae, Gv.F(4.5))) && Gv.Gt(abv, Gv.F(4.0))) && Gv.Lt(abv, Gv.F(5.5)))), (Gv.S("%.1f °P, %.1f %% obj., diacetyl %d ppb") % Gd.Arr(ae, abv, Gd.roundi(S.G("fer").G("D")))));
    T.C("fresh");
    S = sim.G("S");
    P = sim.G("proc");
    S = S.S("cip", S.G("cip").S("target", Gv.S("rvp")));
    S = S.S("cip", S.G("cip").S("medium", Gv.S("louh")));
    S = S.S("cip", S.G("cip").S("set", Gv.F(80.0)));
    S = S.S("cip", S.G("cip").S("T", Gv.F(20.0)));
    S = S.S("cip", S.G("cip").S("run", Gv.B(true)));
    foreach (Gv i in (Gv.I(80L)).Iter()) {
      P.C("cip_step", Gv.F(60.0));
    }
    T.C("chk", Gv.S("AUDIT CIP: louh 80 °C – ohřev stanice a 20 min nad 75 °C splní krok"), S.G("cip").G("done").C("has", Gv.S("rvp:louh!")), (Gv.S("%.0f °C, %.0f min v pásmu") % Gd.Arr(S.G("cip").G("T"), S.G("cip").G("done").C("get", Gv.S("rvp:louh"), Gv.F(0.0)))));
    S = S.S("cip", S.G("cip").S("medium", Gv.S("kyselina")));
    S = S.S("cip", S.G("cip").S("set", Gv.F(70.0)));
    S = S.S("cip", S.G("cip").S("T", Gv.F(70.0)));
    S.G("once").C("clear");
    P.C("cip_step", Gv.F(60.0));
    Gv warned = Gv.B(false);
    foreach (Gv l in (S.G("log")).Iter()) {
      if ((Gv.In(Gv.S("Kyselina nad 60"), l.G("msg")) || Gv.In(Gv.S("kyselina nad 60"), l.G("msg")))) {
        warned = Gv.B(true);
      }
    }
    T.C("chk", Gv.S("AUDIT CIP: kyselina nad 60 °C vyvolá výstrahu a nepočítá se"), Gv.B(((warned).T() && !(S.G("cip").G("done").C("has", Gv.S("rvp:kyselina"))).T())), Gv.S(""));
    return Gv.Null;
  }
  public Gv _ui_regress() {
    T.C("fresh");
    Gv S = sim.G("S");
    Gv rt = sim.C("route_valves", Gv.S("rvp"), Gv.S("sk"), Gd.Arr(Gv.S("p1")));
    Gv goal = (Gv.Ne(sim.G("last_goal"), Gv.Null) ? sim.G("net").G("nodes").C("get", sim.G("last_goal")) : Gv.Null);
    Gv under_ok = Gv.B(false);
    Gv low_ok = Gv.B(false);
    if (Gv.Ne(goal, Gv.Null)) {
      foreach (Gv at in (goal.G("att")).Iter()) {
        if ((Gv.Eq(at.G("kind"), Gv.S("vessel")) && Gv.Eq(at.G("bind"), Gv.S("sk")))) {
          under_ok = Gv.B(!(at.C("get", Gv.S("under"), Gv.B(true))).T());
          low_ok = at.G("low");
        }
      }
    }
    T.C("chk", Gv.S("PLÁN přívod rmutu (C1) do SK ústí nad síto, pod hladinu"), Gv.B(((Gv.Ne(rt, Gv.Null) && (under_ok).T()) && (low_ok).T())), (Gv.S("uzel %s") % Gd.Dyn("str", sim.G("last_goal"))));
    Gv kl = Gv.Null;
    foreach (Gv it in (m.G("items")).Iter()) {
      if ((Gv.Eq(it.G("type"), Gv.S("valve")) && Gv.Ne(it.C("get", Gv.S("kind"), Gv.S("")), Gv.S("skrt")))) {
        kl = it;
        break;
      }
    }
    kl = kl.SI(Gv.S("pct"), Gv.I(50L));
    m.C("save_game");
    kl.C("erase", Gv.S("pct"));
    Gv f = Gd.Static("FileAccess", "open", m.G("save_path"), Gd.StaticGet("FileAccess", "READ"));
    Gv d = f.C("get_var");
    f.C("close");
    d = d.S("items", d.G("items").SI(kl.G("id"), d.G("items").I(kl.G("id")).SI(Gv.S("pct"), Gv.I(50L))));
    f = Gd.Static("FileAccess", "open", m.G("save_path"), Gd.StaticGet("FileAccess", "WRITE"));
    f.C("store_var", d);
    f.C("close");
    m.C("load_game");
    Gv kl2 = Gv.Null;
    foreach (Gv it_2 in (m.G("items")).Iter()) {
      if (Gv.Eq(it_2.G("id"), kl.G("id"))) {
        kl2 = it_2;
      }
    }
    T.C("chk", Gv.S("REGRESE staré uložení: klapka po načtení není přivřená na 50 %"), Gv.B(((Gv.Ne(kl2, Gv.Null) && !(kl2.C("has", Gv.S("pct"))).T()) && !(kl2.C("has", Gv.S("part"))).T())), (Gv.Ne(kl2, Gv.Null) ? Gd.Dyn("str", kl2.C("get", Gv.S("pct"), Gv.S("-"))) : Gv.S("?")));
    T.C("fresh");
    S = sim.G("S");
    S = S.S("cab", S.G("cab").S("key", Gv.B(false)));
    sim.C("sim_step", Gv.F(1.0));
    m.C("toggle_pump", Gv.S("p1"));
    T.C("chk", Gv.S("REGRESE klíč VYP: C1 nejde pustit a hlášení řekne proč"), Gv.B(((!(S.G("p1").G("on")).T() && Gv.Gt(S.G("log").C("size"), Gv.I(0L))) && Gv.In(Gv.S("klíč"), S.G("log").I(Gv.I(0L)).G("msg")))), ((S.G("log").C("size")).T() ? S.G("log").I(Gv.I(0L)).G("msg") : Gv.S("")));
    S = S.S("cab", S.G("cab").S("key", Gv.B(true)));
    sim.C("sim_step", Gv.F(1.0));
    S = S.S("rvp", S.G("rvp").S("w", Gv.F(1500.0)));
    S = S.S("p1", S.G("p1").S("prime", Gv.F(0.0)));
    _open(Gv.S("rvp"), Gv.S("sk"), Gd.Arr(Gv.S("p1")));
    m.C("toggle_pump", Gv.S("p1"));
    T.C("chk", Gv.S("REGRESE C1 se pustí i před plným zavodněním (sání zaplavené)"), S.G("p1").G("on"), ((S.G("log").C("size")).T() ? S.G("log").I(Gv.I(0L)).G("msg") : Gv.S("")));
    foreach (Gv i in (Gv.I(30L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
    }
    T.C("chk", Gv.S("REGRESE C1 po pár sekundách čerpá"), Gv.B(Gv.Gt(S.G("p1").G("q"), Gv.F(100.0))), (Gv.S("%.0f L/min %s") % Gd.Arr(S.G("p1").G("q"), S.G("p1").G("status"))));
    S = S.S("p1", S.G("p1").S("on", Gv.B(false)));
    return Gv.Null;
  }
  public Gv _water_total() {
    Gv S = sim.G("S");
    Gv w = Gv.F(0.0);
    foreach (Gv k in (Gd.Arr(Gv.S("rvp"), Gv.S("mp"), Gv.S("wp"))).Iter()) {
      w = (w + S.I(k).G("w"));
    }
    w = (w + S.G("sk").G("mash").G("w"));
    foreach (Gv c in (S.G("sk").G("cells")).Iter()) {
      w = (w + c.G("w"));
    }
    w = (w + ((S.G("sp").G("w") + S.G("hlt").G("w")) + S.G("lv").G("w")));
    foreach (Gv e in (sim.G("net").G("edges")).Iter()) {
      w = (w + e.G("c").G("w"));
    }
    return (w).AsF();
    return Gv.Null;
  }
  public Gv _ext_total() {
    Gv x = sim.C("extract_inventory");
    return (x.G("celkem")).AsF();
    return Gv.Null;
  }
  public Gv _spec_tests() {
    T.C("fresh");
    Gv S = sim.G("S");
    S = S.S("rvp", S.G("rvp").S("w", Gv.F(2000.0)));
    S = S.S("rvp", S.G("rvp").S("f", Gv.F(200.0)));
    S = S.S("rvp", S.G("rvp").S("T", Gv.F(50.0)));
    S = S.S("rvp", S.G("rvp").S("Tsens", Gv.F(50.0)));
    S = S.S("boiler", S.G("boiler").S("on", Gv.B(false)));
    S = S.S("hlt", S.G("hlt").S("auto", Gv.B(false)));
    _open(Gv.S("rvp"), Gv.S("sk"), Gd.Arr(Gv.S("p1")));
    S = S.S("p1", S.G("p1").S("on", Gv.B(true)));
    S = S.S("p1", S.G("p1").S("prime", Gv.F(1.0)));
    S = S.S("p1", S.G("p1").S("flow", Gv.F(200.0)));
    Gv w0 = (_water_total()).AsF();
    Gv e0 = ((((S.G("rvp").G("f") + S.G("rvp").G("dx")) + S.G("rvp").G("ld")) + S.G("rvp").G("ns"))).AsF();
    foreach (Gv i in (Gv.I(240L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
    }
    S = S.S("p1", S.G("p1").S("on", Gv.B(false)));
    foreach (Gv i_2 in (Gv.I(30L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
    }
    Gv w1 = (((_water_total() + S.G("drainL")) + S.C("get", Gv.S("evapTot"), Gv.F(0.0)))).AsF();
    Gv ex = sim.C("extract_inventory");
    Gv e1 = (((((ex.G("rvp") + ex.G("sk")) + ex.G("skmash")) + ex.G("trubky")) + ex.G("vyteklo"))).AsF();
    T.C("chk", Gv.S("ZADÁNÍ bilance vody: přečerpání 4 min (voda v nádobách + potrubí + odtok + odpar) se zachová"), Gv.B(Gv.Lt(Gd.absf((w1 - w0)), Gv.F(0.5))), (Gv.S("%.2f → %.2f kg, přečerpáno do SK %.0f L") % Gd.Arr(w0, w1, sim.C("skVolTotal"))));
    T.C("chk", Gv.S("ZADÁNÍ bilance extraktu: při přečerpání extrakt nevzniká ani nemizí"), Gv.B(Gv.Lt(Gd.absf((e1 - e0)), Gv.F(0.05))), (Gv.S("%.3f → %.3f kg") % Gd.Arr(e0, e1)));
    T.C("fresh");
    S = sim.G("S");
    S = S.S("malt", S.G("malt").S("added", Gv.F(700.0)));
    S = S.S("extIn", Gv.F(538.0));
    Gv P = Gv.F(12.5);
    Gv rho = (Gv.F(1.0) + (Gv.F(0.004) * P));
    Gv e = Gv.F(500.0);
    Gv V20 = (((e / ((P / Gv.F(100.0))))) / rho);
    S = S.S("flags", S.G("flags").S("knock", Gd.Dict(Gv.S("V"), (V20 / Gv.F(0.96)), Gv.S("P"), P, Gv.S("e"), e, Gv.S("t"), Gv.F(0.0))));
    Gv y = sim.C("yields");
    T.C("chk", Gv.S("ZADÁNÍ výtěžnost varny = extrakt ve vyrážce / slad"), Gv.B(Gv.Lt(Gd.absf((y.G("VV_varna") - ((Gv.F(500.0) / Gv.F(700.0)) * Gv.F(100.0)))), Gv.F(0.01))), (Gv.S("%.2f %%") % y.G("VV_varna")));
    T.C("chk", Gv.S("ZADÁNÍ výtěžnost z objemu horké vyrážky (×0,96) souhlasí s extraktem"), Gv.B(Gv.Lt(Gd.absf((y.G("VV_cz") - y.G("VV_varna"))), Gv.F(0.1))), (Gv.S("%.2f vs %.2f %%") % Gd.Arr(y.G("VV_cz"), y.G("VV_varna"))));
    T.C("chk", Gv.S("ZADÁNÍ využití lab. extraktu = extrakt ve vyrážce / potenciál sladu"), Gv.B(Gv.Lt(Gd.absf((y.G("ucinnost") - ((Gv.F(500.0) / Gv.F(538.0)) * Gv.F(100.0)))), Gv.F(0.01))), (Gv.S("%.1f %%") % y.G("ucinnost")));
    Gv rc = sim.C("recipe_check", sim.G("RECIPES").I(Gv.I(0L)));
    T.C("chk", Gv.S("ZADÁNÍ kontrola receptu spočte potřebný slad z cílového objemu a °P"), Gv.B(Gv.In(Gv.S("kg sladu"), rc)), rc);
    Gv f = Gd.Static("FileAccess", "open", sim.G("CALIB_PATH"), Gd.StaticGet("FileAccess", "WRITE"));
    f.C("store_string", Gv.S("{\"rvpUAst\": {\"value\": 7.25, \"source\": \"CALIBRATED\"}, \"neznamy\": 3}"));
    f.C("close");
    sim.C("new_state");
    T.C("chk", Gv.S("ZADÁNÍ kalibrace.json přepíše parametr (rvpUAst 7,25) a ignoruje neznámé klíče"), Gv.B((Gv.Lt(Gd.absf((sim.G("S").G("cfg").G("rvpUAst") - Gv.F(7.25))), Gv.F(1e-06)) && !(sim.G("S").G("cfg").C("has", Gv.S("neznamy"))).T())), Gd.Dyn("str", sim.G("calib_applied")));
    Gd.Static("DirAccess", "remove_absolute", Gd.Static("ProjectSettings", "globalize_path", sim.G("CALIB_PATH")));
    sim.C("new_state");
    T.C("chk", Gv.S("ZADÁNÍ bez kalibrace platí výchozí hodnoty"), Gv.B(Gv.Lt(Gd.absf((sim.G("S").G("cfg").G("rvpUAst") - sim.G("CFG_DEFAULT").G("rvpUAst"))), Gv.F(1e-06))));
    Gv res = Gd.Arr();
    foreach (Gv run in (Gv.I(2L)).Iter()) {
      T.C("fresh");
      S = sim.G("S");
      S = S.S("rvp", S.G("rvp").S("w", Gv.F(1500.0)));
      S = S.S("rvp", S.G("rvp").S("T", Gv.F(40.0)));
      S = S.S("rvp", S.G("rvp").S("Tsens", Gv.F(40.0)));
      S = S.S("rvp", S.G("rvp").S("stir", Gv.B(true)));
      S = S.S("rvp", S.G("rvp").S("heat", Gv.F(0.7)));
      S = S.S("boiler", S.G("boiler").S("on", Gv.B(true)));
      foreach (Gv i_3 in (Gv.I(600L)).Iter()) {
        sim.C("sim_step", Gv.F(1.0));
      }
      res.C("append", Gd.Arr(S.G("rvp").G("T"), S.G("rvp").G("w"), S.G("hlt").G("T")));
    }
    T.C("chk", Gv.S("ZADÁNÍ determinismus: stejný ohřev 10 min dvakrát = stejné teploty i hmotnosti"), Gv.B(Gv.Eq(res.I(Gv.I(0L)), res.I(Gv.I(1L)))), Gd.Dyn("str", res));
    T.C("fresh");
    S = sim.G("S");
    S = S.S("rvp", S.G("rvp").S("w", Gv.F(2000.0)));
    S = S.S("rvp", S.G("rvp").S("T", Gv.F(40.0)));
    S = S.S("rvp", S.G("rvp").S("Tsens", Gv.F(40.0)));
    S = S.S("rvp", S.G("rvp").S("stir", Gv.B(true)));
    S = S.S("rvp", S.G("rvp").S("heat", Gv.F(0.6)));
    S = S.S("boiler", S.G("boiler").S("on", Gv.B(true)));
    sim.C("sim_step", Gv.F(1.0));
    Gv T0 = (S.G("rvp").G("T")).AsF();
    foreach (Gv i_4 in (Gv.I(60L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
    }
    Gv dTdt = (((S.G("rvp").G("T") - T0))).AsF();
    Gv P_need = ((((Gv.F(2000.0) * Gv.F(4.19)) * dTdt) / Gv.F(60.0))).AsF();
    T.C("chk", Gv.S("ZADÁNÍ ohřev 2000 L: výkon na vodu z rychlosti ohřevu je v řádu stovek kW (zadání: 2,1 K/min ≈ 293 kW)"), Gv.B((Gv.Gt(P_need, Gv.F(150.0)) && Gv.Lt(P_need, Gv.F(500.0)))), (Gv.S("%.2f K/min → %.0f kW jen na vodu") % Gd.Arr(dTdt, P_need)));
    return Gv.Null;
  }
  public Gv _steel_tests() {
    T.C("fresh");
    Gv S = sim.G("S");
    Gv c = S.G("cfg");
    Gv v = S.G("rvp");
    v = v.S("w", Gv.F(1000.0));
    v = v.S("T", Gv.F(20.0));
    T.C("chk", Gv.S("OCEL kapacita kapaliny: 1000 kg vody = 4190 kJ/K"), Gv.B(Gv.Lt(Gd.absf((sim.C("cC", v) - Gv.F(4190.0))), Gv.F(0.01))), (Gv.S("%.1f kJ/K") % sim.C("cC", v)));
    T.C("chk", Gv.S("OCEL příspěvek kovu = m · c_p (450 kg × 0,50 = 225 kJ/K)"), Gv.B(Gv.Lt(Gd.absf((sim.C("steelC", Gv.F(450.0)) - Gv.F(225.0))), Gv.F(1e-06))), (Gv.S("%.1f kJ/K") % sim.C("steelC", Gv.F(450.0))));
    Gv Ce = Gd.Arr();
    foreach (Gv st in (Gd.Arr(Gv.F(0.0), Gv.F(450.0))).Iter()) {
      T.C("fresh");
      S = sim.G("S");
      c = S.G("cfg");
      c = c.S("rvpSteel", st);
      c = c.S("rvpUA", Gv.F(0.0));
      v = S.G("rvp");
      v = v.S("w", Gv.F(1000.0));
      v = v.S("T", Gv.F(40.0));
      v = v.S("Tsens", Gv.F(40.0));
      v = v.S("stir", Gv.B(true));
      v = v.S("heat", Gv.F(0.5));
      S = S.S("boiler", S.G("boiler").S("on", Gv.B(true)));
      Gv E = Gv.F(0.0);
      foreach (Gv i in (Gv.I(300L)).Iter()) {
        sim.C("sim_step", Gv.F(1.0));
        E = (E + v.G("Pin"));
      }
      Ce.C("append", (E / ((v.G("T") - Gv.F(40.0)))));
    }
    T.C("chk", Gv.S("OCEL ohřev s 450 kg oceli: navíc ≈ 225 kJ/K (ne 450 – kov se nepočítá dvakrát ani s c_p 1,0)"), Gv.B(Gv.Lt(Gd.absf((((Ce.I(Gv.I(1L)) - Ce.I(Gv.I(0L)))) - Gv.F(225.0))), Gv.F(8.0))), (Gv.S("0 kg: %.0f, 450 kg: %.0f kJ/K → rozdíl %.0f") % Gd.Arr(Ce.I(Gv.I(0L)), Ce.I(Gv.I(1L)), (Ce.I(Gv.I(1L)) - Ce.I(Gv.I(0L))))));
    T.C("chk", Gv.S("OCEL bez oceli a ztrát: E/ΔT = kapacita vody (1000 kg → 4190 kJ/K)"), Gv.B(Gv.Lt(Gd.absf((Ce.I(Gv.I(0L)) - Gv.F(4190.0))), Gv.F(30.0))), (Gv.S("%.0f kJ/K") % Ce.I(Gv.I(0L))));
    T.C("fresh");
    S = sim.G("S");
    c = S.G("cfg");
    v = S.G("rvp");
    v = v.S("w", Gv.F(1000.0));
    v = v.S("T", Gv.F(20.0));
    Gv p = sim.C("lp");
    p = p.S("w", Gv.F(500.0));
    p = p.S("T", Gv.F(80.0));
    Gv Eb = ((((sim.C("cC", v) * Gv.F(20.0)) + (sim.C("steelC", c.G("rvpSteel")) * Gv.F(20.0))) + (sim.C("cC", p) * Gv.F(80.0)))).AsF();
    sim.C("addTo", v, p, c.G("rvpSteel"));
    Gv Ea = ((((sim.C("cC", v) + sim.C("steelC", c.G("rvpSteel")))) * v.G("T"))).AsF();
    T.C("chk", Gv.S("OCEL směšování přítoku: entalpie vody + oceli se zachová"), Gv.B(Gv.Lt(Gd.absf((Ea - Eb)), Gv.F(1.0))), (Gv.S("%.1f vs %.1f kJ, T = %.2f °C") % Gd.Arr(Eb, Ea, v.G("T"))));
    return Gv.Null;
  }
  public Gv _wcal(Gv txt = default(Gv)) {
    Gv f = Gd.Static("FileAccess", "open", sim.G("CALIB_PATH"), Gd.StaticGet("FileAccess", "WRITE"));
    f.C("store_string", txt);
    f.C("close");
    return Gv.Null;
  }
  public Gv _delcal() {
    Gd.Static("DirAccess", "remove_absolute", Gd.Static("ProjectSettings", "globalize_path", sim.G("CALIB_PATH")));
    return Gv.Null;
  }
  public Gv _c1_flow() {
    T.C("fresh");
    Gv S = sim.G("S");
    S = S.S("rvp", S.G("rvp").S("w", Gv.F(2000.0)));
    S = S.S("rvp", S.G("rvp").S("T", Gv.F(40.0)));
    _open(Gv.S("rvp"), Gv.S("sk"), Gd.Arr(Gv.S("p1")));
    S = S.S("p1", S.G("p1").S("on", Gv.B(true)));
    S = S.S("p1", S.G("p1").S("prime", Gv.F(1.0)));
    S = S.S("p1", S.G("p1").S("flow", Gv.F(400.0)));
    foreach (Gv i in (Gv.I(5L)).Iter()) {
      sim.C("net_step", Gv.F(1.0));
    }
    Gv q = (S.G("p1").C("get", Gv.S("hz"), Gv.F(0.0))).AsF();
    S = S.S("p1", S.G("p1").S("on", Gv.B(false)));
    return (q).AsF();
    return Gv.Null;
  }
  public Gv _calib_tests() {
    _wcal(Gv.S("{\"skArea\": 0, \"steamBar\": -3, \"rvpUAst\": \"x\", \"UA1\": 30}"));
    sim.C("new_state");
    T.C("chk", Gv.S("KALIBRACE nesmyslné hodnoty odmítnuty (skArea 0, steamBar −3, text), platná přijata (UA1 30)"), Gv.B((((Gv.Lt(Gd.absf((sim.G("S").G("cfg").G("skArea") - Gv.F(5.3))), Gv.F(1e-06)) && Gv.Lt(Gd.absf((sim.G("S").G("cfg").G("steamBar") - Gv.F(2.5))), Gv.F(1e-06))) && Gv.Lt(Gd.absf((sim.G("S").G("cfg").G("UA1") - Gv.F(30.0))), Gv.F(1e-06))) && Gv.Eq(sim.G("calib_rejected").C("size"), Gv.I(3L)))), Gv.S("; ").C("join", Gd.Ctor("PackedStringArray", sim.G("calib_rejected"))));
    _delcal();
    sim.C("new_state");
    T.C("fresh");
    m.C("save_game");
    _wcal(Gv.S("{\"rvpUAst\": 8.0}"));
    m.C("load_game");
    T.C("chk", Gv.S("KALIBRACE platí i pro načtenou uloženou várku"), Gv.B(Gv.Lt(Gd.absf((sim.G("S").G("cfg").G("rvpUAst") - Gv.F(8.0))), Gv.F(1e-06))), (Gv.S("%.2f") % sim.G("S").G("cfg").G("rvpUAst")));
    _delcal();
    Gv q0 = (_c1_flow()).AsF();
    _wcal(Gv.S("{\"pipeLenScale\": 2.5}"));
    sim.C("new_state");
    Gv q1 = (_c1_flow()).AsF();
    _wcal(Gv.S("{\"pipeLenScale\": 3}"));
    Gv q3 = (_c1_flow()).AsF();
    _wcal(Gv.S("{\"pipeLenScale\": 3, \"p1_H0\": 12}"));
    Gv q2 = (_c1_flow()).AsF();
    _delcal();
    T.C("chk", Gv.S("KALIBRACE delší trasy (pipeLenScale 2,5) → měnič musí přidat otáčky pro stejných 400 L/min"), Gv.B(Gv.Gt(q1, (q0 + Gv.F(0.5)))), (Gv.S("%.1f → %.1f Hz") % Gd.Arr(q0, q1)));
    T.C("chk", Gv.S("KALIBRACE slabší čerpadlo (p1_H0 12 m) na odporové trase → vyšší otáčky pro stejný průtok"), Gv.B(Gv.Gt(q2, (q3 + Gv.F(0.5)))), (Gv.S("H0 32 m: %.1f Hz, H0 12 m: %.1f Hz") % Gd.Arr(q3, q2)));
    Gv mu0 = (sim.C("skViscosity", Gv.F(75.0), Gv.F(0.0))).AsF();
    Gv mu18 = (sim.C("skViscosity", Gv.F(75.0), Gv.F(18.0))).AsF();
    T.C("chk", Gv.S("VISKOZITA sladina 18 °P / voda při 75 °C ≈ e^(0,047·18) = 2,3 (EBC kongresní sladina 8,6 % ≈ 1,4–1,7× vody)"), Gv.B(Gv.Lt(Gd.absf(((mu18 / mu0) - Gd.exp((Gv.F(0.047) * Gv.F(18.0))))), Gv.F(0.01))), (Gv.S("%.2f×, μ(8,6 °P) = %.2f× vody") % Gd.Arr((mu18 / mu0), (sim.C("skViscosity", Gv.F(20.0), Gv.F(8.6)) / sim.C("skViscosity", Gv.F(20.0), Gv.F(0.0))))));
    Gv dT = Gd.Arr();
    foreach (Gv amb in (Gd.Arr(Gv.F(10.0), Gv.F(30.0))).Iter()) {
      T.C("fresh");
      Gv S = sim.G("S");
      S = S.S("cfg", S.G("cfg").S("ambientT", amb));
      S = S.S("lv", S.G("lv").S("w", Gv.F(8000.0)));
      S = S.S("lv", S.G("lv").S("T", Gv.F(2.0)));
      S = S.S("lv", S.G("lv").S("chill", Gv.B(false)));
      foreach (Gv i in (Gv.I(3600L)).Iter()) {
        sim.C("sim_step", Gv.F(1.0));
      }
      dT.C("append", (S.G("lv").G("T") - Gv.F(2.0)));
    }
    T.C("chk", Gv.S("LEDOVKA ohřev od okolí závisí na ambientT (10 vs 30 °C)"), Gv.B(Gv.Gt(dT.I(Gv.I(1L)), (dT.I(Gv.I(0L)) * Gv.F(2.0)))), (Gv.S("+%.2f vs +%.2f K/h") % Gd.Arr(dT.I(Gv.I(0L)), dT.I(Gv.I(1L)))));
    T.C("fresh");
    Gv gi = (sim.C("grain_dry_insoluble", Gv.F(700.0))).AsF();
    T.C("chk", Gv.S("SLAD nerozpustná sušina + extrakt + voda = 700 kg sypání (stejné MALTS)"), Gv.B((Gv.Gt(gi, Gv.F(100.0)) && Gv.Lt(gi, Gv.F(160.0)))), (Gv.S("%.1f kg pluch") % gi));
    return Gv.Null;
  }
  public Gv _ferm_cip_tests() {
    T.C("fresh");
    Gv S = sim.G("S");
    S = S.S("sp", S.G("sp").S("L", Gv.F(4000.0)));
    S = S.S("sp", S.G("sp").S("w", Gv.F(3500.0)));
    S = S.S("sp", S.G("sp").S("f", Gv.F(400.0)));
    S = S.S("sp", S.G("sp").S("n", Gv.F(80.0)));
    S = S.S("sp", S.G("sp").S("H", (Gv.F(4000.0) * Gv.F(9.0))));
    sim.G("proc").C("fer_start");
    Gv f = S.G("fer");
    Gv S0 = (f.G("Sf")).AsF();
    foreach (Gv i in (Gv.I(96L)).Iter()) {
      sim.G("proc").C("fer_step", Gv.F(3600.0));
    }
    Gv used = ((S0 - f.G("Sf"))).AsF();
    Gv prod = (((f.G("E") + f.C("get", Gv.S("CO2"), Gv.F(0.0))) + f.C("get", Gv.S("XB"), Gv.F(0.0)))).AsF();
    T.C("chk", Gv.S("KVAŠENÍ bilance (Balling): spotřebovaný extrakt = etanol + CO₂ + biomasa"), Gv.B(Gv.Lt(Gd.absf((prod - used)), (Gv.F(0.01) * Gd.maxf(Gv.F(1.0), used)))), (Gv.S("%.1f g/L → etanol %.1f + CO₂ %.1f + biomasa %.1f") % Gd.Arr(used, f.G("E"), f.C("get", Gv.S("CO2"), Gv.F(0.0)), f.C("get", Gv.S("XB"), Gv.F(0.0)))));
    T.C("fresh");
    S = sim.G("S");
    S = S.S("cip", S.G("cip").S("target", Gv.S("rvp")));
    S = S.S("cip", S.G("cip").S("medium", Gv.S("louh")));
    S = S.S("cip", S.G("cip").S("set", Gv.F(80.0)));
    S = S.S("cip", S.G("cip").S("T", Gv.F(80.0)));
    S = S.S("cip", S.G("cip").S("conc", Gv.F(0.5)));
    S = S.S("cip", S.G("cip").S("run", Gv.B(true)));
    foreach (Gv i_2 in (Gv.I(30L)).Iter()) {
      sim.G("proc").C("cip_step", Gv.F(60.0));
    }
    T.C("chk", Gv.S("CIP slabý louh (0,5 % NaOH) se nezapočítá ani při 80 °C 30 min"), Gv.B(!(S.G("cip").G("done").C("has", Gv.S("rvp:louh!"))).T()));
    S = S.S("cip", S.G("cip").S("conc", Gv.F(1.5)));
    S = S.S("cip", S.G("cip").S("done", Gd.Dict()));
    foreach (Gv i_3 in (Gv.I(25L)).Iter()) {
      sim.G("proc").C("cip_step", Gv.F(60.0));
    }
    T.C("chk", Gv.S("CIP louh 1,5 % při 80 °C 25 min = splněno"), S.G("cip").G("done").C("has", Gv.S("rvp:louh!")));
    return Gv.Null;
  }
  public Gv _practice_tests() {
    T.C("fresh");
    Gv S = sim.G("S");
    Gv pr = m.G("practice");
    if (Gv.Eq(pr, Gv.Null)) {
      T.C("chk", Gv.S("ZÁSADY modul načten"), Gv.B(false));
      return Gv.Null;
    }
    S = S.S("p1", S.G("p1").S("prime", Gv.F(0.0)));
    S = S.S("p1", S.G("p1").S("flow", Gv.F(350.0)));
    pr.C("on_pump_start", Gv.S("p1"));
    T.C("chk", Gv.S("ZÁSADY nezavodněné čerpadlo + rozjezd naplno = 2 porušení"), Gv.B(Gv.Eq(Gd.Dyn("int", S.G("practice").G("n")), Gv.I(2L))), Gd.Dyn("str", S.G("practice").G("rules")));
    S = S.S("rvp", S.G("rvp").S("w", Gv.F(1500.0)));
    S = S.S("rvp", S.G("rvp").S("T", Gv.F(40.0)));
    S = S.S("rvp", S.G("rvp").S("heat", Gv.F(0.0)));
    pr.C("tick", Gv.F(1.0));
    S = S.S("rvp", S.G("rvp").S("heat", Gv.F(1.0)));
    S = S.S("rvp", S.G("rvp").S("stir", Gv.B(true)));
    pr.C("tick", Gv.F(1.0));
    T.C("chk", Gv.S("ZÁSADY pára otevřená naráz je porušení"), S.G("practice").G("rules").C("has", Gv.S("para_naraz")));
    S = S.S("rvp", S.G("rvp").S("g", Gv.F(100.0)));
    S = S.S("rvp", S.G("rvp").S("stir", Gv.B(false)));
    foreach (Gv i in (Gv.I(35L)).Iter()) {
      pr.C("tick", Gv.F(1.0));
    }
    T.C("chk", Gv.S("ZÁSADY topení bez míchadla (rmut) je porušení"), S.G("practice").G("rules").C("has", Gv.S("micha_para")));
    Gv n0 = (Gd.Dyn("int", S.G("practice").G("n"))).AsI();
    foreach (Gv i_2 in (Gv.I(120L)).Iter()) {
      pr.C("tick", Gv.F(1.0));
    }
    T.C("chk", Gv.S("ZÁSADY stejné upozornění se neopakuje každou sekundu"), Gv.B(Gv.Le(Gd.Dyn("int", S.G("practice").G("n")), (n0 + Gv.I(1L)))), (Gv.S("%d → %d") % Gd.Arr(n0, Gd.Dyn("int", S.G("practice").G("n")))));
    S = S.S("p3", S.G("p3").S("on", Gv.B(true)));
    pr.C("on_pump_stop", Gv.S("plv"));
    T.C("chk", Gv.S("ZÁSADY zastavení ledové vody před C3 je porušení"), S.G("practice").G("rules").C("has", Gv.S("chlazeni_konec")));
    Gv card = (Gv.Ne(sim.C("get", Gv.S("proc")), Gv.Null) ? sim.G("proc").C("scorecard") : Gd.Arr());
    Gv found = Gv.B(false);
    foreach (Gv r in (card).Iter()) {
      if ((Gd.Dyn("str", r.I(Gv.I(0L))).C("begins_with", Gv.S("Provozní zásady"))).T()) {
        found = Gv.B(true);
      }
    }
    T.C("chk", Gv.S("ZÁSADY jsou v hodnocení várky"), Gv.B(((found).T() || (card.C("is_empty")).T())));
    return Gv.Null;
  }
  public Gv _physics_lit_tests() {
    T.C("fresh");
    Gv S = sim.G("S");
    Gv v = sim.C("lp");
    v = v.S("w", Gv.F(1000.0));
    v = v.S("T", Gv.F(100.0));
    v = v.S("smm", Gv.F(1.0));
    S = S.S("pH", Gv.F(5.2));
    v = v.SI(Gv.S("boiling"), Gv.B(false));
    v = v.SI(Gv.S("evapKgMin"), Gv.F(0.0));
    foreach (Gv i in (Gv.I(38L)).Iter()) {
      sim.C("react", v, Gv.F(1.0));
    }
    T.C("chk", Gv.S("LIT SMM poločas při 100 °C, pH 5,2 ≈ 38 min (Dickenson)"), Gv.B((Gv.Lt(Gd.absf(((v.G("smm") + v.G("dms")) - Gv.F(1.0))), Gv.F(0.6)) && Gv.Lt(Gd.absf((v.G("smm") - Gv.F(0.5))), Gv.F(0.05)))), (Gv.S("%.3f") % v.G("smm")));
    v = sim.C("lp");
    v = v.S("w", Gv.F(1000.0));
    v = v.S("T", Gv.F(94.0));
    v = v.S("smm", Gv.F(1.0));
    v = v.SI(Gv.S("boiling"), Gv.B(false));
    v = v.SI(Gv.S("evapKgMin"), Gv.F(0.0));
    foreach (Gv i_2 in (Gv.I(76L)).Iter()) {
      sim.C("react", v, Gv.F(1.0));
    }
    T.C("chk", Gv.S("LIT SMM při 94 °C: poločas 2× delší (každých −6 °C)"), Gv.B(Gv.Lt(Gd.absf((v.G("smm") - Gv.F(0.5))), Gv.F(0.05))), (Gv.S("%.3f") % v.G("smm")));
    v = sim.C("lp");
    v = v.S("w", Gv.F(1000.0));
    v = v.S("T", Gv.F(72.0));
    v = v.S("aA", Gv.F(1.0));
    v = v.S("aB", Gv.F(0.87));
    v = v.S("aBs", Gv.F(0.13));
    v = v.SI(Gv.S("boiling"), Gv.B(false));
    v = v.SI(Gv.S("evapKgMin"), Gv.F(0.0));
    foreach (Gv i_3 in (Gv.I(15L)).Iter()) {
      sim.C("react", v, Gv.F(1.0));
    }
    T.C("chk", Gv.S("LIT α-amyláza při 72 °C: poločas ≈ 15 min (Marc)"), Gv.B(Gv.Lt(Gd.absf((v.G("aA") - Gv.F(0.5))), Gv.F(0.06))), (Gv.S("%.3f") % v.G("aA")));
    T.C("chk", Gv.S("LIT β-amyláza při 72 °C: labilní podíl zmizí, stabilní zůstane (De Schepper)"), Gv.B((Gv.Lt(v.G("aB"), Gv.F(0.001)) && Gv.Gt(v.G("aBs"), Gv.F(0.08)))), (Gv.S("aB %.4f aBs %.3f") % Gd.Arr(v.G("aB"), v.G("aBs"))));
    T.C("fresh");
    S = sim.G("S");
    S = S.S("cfg", S.G("cfg").S("boilerKW", Gv.F(200.0)));
    S = S.S("rvp", S.G("rvp").S("w", Gv.F(4000.0)));
    S = S.S("rvp", S.G("rvp").S("T", Gv.F(60.0)));
    S = S.S("rvp", S.G("rvp").S("heat", Gv.F(1.0)));
    S = S.S("rvp", S.G("rvp").S("stir", Gv.B(true)));
    S = S.S("hlt", S.G("hlt").S("heat", Gv.F(1.0)));
    S = S.S("hlt", S.G("hlt").S("auto", Gv.B(false)));
    S = S.S("hlt", S.G("hlt").S("T", Gv.F(40.0)));
    foreach (Gv i_4 in (Gv.I(600L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
    }
    Gv p_low = (S.G("boiler").G("p")).AsF();
    S = S.S("rvp", S.G("rvp").S("heat", Gv.F(0.0)));
    S = S.S("hlt", S.G("hlt").S("heat", Gv.F(0.0)));
    foreach (Gv i_5 in (Gv.I(600L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
    }
    T.C("chk", Gv.S("LIT kotel: při odběru nad výkon tlak páry klesá a po odlehčení se vrátí"), Gv.B((Gv.Lt(p_low, (S.G("cfg").G("steamBar") - Gv.F(0.3))) && Gv.Gt(S.G("boiler").G("p"), (p_low + Gv.F(0.2))))), (Gv.S("%.2f → %.2f bar") % Gd.Arr(p_low, S.G("boiler").G("p"))));
    return Gv.Null;
  }
  public Gv _mains_outs(Gv src = default(Gv)) {
    Gv r = sim.C("bfs", sim.C("starts_of", src.G("id")));
    Gv outs = Gd.Arr();
    foreach (Gv d in (r.G("hits")).Iter()) {
      if (Gv.Ne(d.G("att").G("bind"), Gv.S("vodovod"))) {
        outs.C("append", Gd.Dict(Gv.S("d"), d, Gv.S("path"), sim.C("path_to", r, d.G("k"))));
      }
    }
    foreach (Gv o__it in (outs).Iter()) {
      Gv o = o__it;
      o = o.SI(Gv.S("f"), sim.C("path_pct", o.G("path")));
    }
    return outs;
    return Gv.Null;
  }
  public Gv _mains_tests() {
    T.C("fresh");
    Gv S = sim.G("S");
    Gv c = S.G("cfg");
    Gv src = Gv.Null;
    foreach (Gv it in (sim.C("items_by_bind", Gv.S("vodovod"))).Iter()) {
      if (Gv.Eq(it.C("get", Gv.S("shape")), Gv.S("inlet"))) {
        src = it;
      }
    }
    if (Gv.Eq(src, Gv.Null)) {
      T.C("chk", Gv.S("AUDIT řad: zdroj vody z řadu existuje"), Gv.B(false));
      return Gv.Null;
    }
    _open(Gv.S("vodovod"), Gv.S("lv"), Gd.Arr());
    src = src.SI(Gv.S("on"), Gv.B(true));
    Gv outs = _mains_outs(src);
    Gv iLv = Gv.I(-1L);
    foreach (Gv i in (outs.C("size")).Iter()) {
      if (Gv.Eq(outs.I(i).G("d").G("att").G("bind"), Gv.S("lv"))) {
        iLv = i;
      }
    }
    if ((Gv.Lt(iLv, Gv.I(0L)) || Gv.Le(outs.I(iLv).G("f"), Gv.F(0.0)))) {
      T.C("chk", Gv.S("AUDIT řad: cesta řad → ledovka je otevřená"), Gv.B(false));
      return Gv.Null;
    }
    Gv Q1 = (sim.C("mains_solve", outs, src).I(iLv)).AsF();
    Gv Qmax = (c.G("mainsQ")).AsF();
    Gv Hm = (((c.G("mainsBar") * Gv.F(100000.0)) / ((Gv.F(1000.0) * Gv.F(9.81))))).AsF();
    Gv mu = (sim.C("skViscosity", S.G("cool").G("waterT"), Gv.F(0.0))).AsF();
    Gv Qt = Gv.F(0.0);
    foreach (Gv q in (sim.C("mains_solve", outs, src)).Iter()) {
      Qt = (Qt + q);
    }
    Gv Hs = ((Hm * ((Gv.F(1.0) - Gd.pow((Qt / Qmax), Gv.I(2L)))))).AsF();
    Gv path = outs.I(iLv).G("path");
    Gv last = path.I((path.C("size") - Gv.I(1L)));
    Gv ysrc = (sim.G("net").G("nodes").I(sim.C("starts_of", src.G("id")).I(Gv.I(0L))).G("y")).AsF();
    Gv hst = ((((ysrc - sim.G("net").G("nodes").I(last.G("to")).G("y"))) / Gv.F(100.0))).AsF();
    Gv Hn = ((sim.C("path_head", path, Q1, Gv.F(1.0), mu) + hst)).AsF();
    T.C("chk", Gv.S("AUDIT řad: tlak zdroje při odběru = ztráta trasy + výška (bilance tlaku v uzlu)"), Gv.B(Gv.Lt(Gd.absf((Hs - Hn)), (Gv.F(0.02) * Gd.maxf(Gv.F(1.0), Hs)))), (Gv.S("zdroj %.2f m, trasa %.2f m, Q %.0f L/min") % Gd.Arr(Hs, Hn, Q1)));
    T.C("chk", Gv.S("AUDIT řad: průtok je kladný a nepřekročí kapacitu přívodu"), Gv.B((Gv.Gt(Q1, Gv.F(20.0)) && Gv.Le(Qt, (Qmax * Gv.F(1.001))))), (Gv.S("%.0f L/min z max %.0f") % Gd.Arr(Q1, Qmax)));
    c = c.S("mainsBar", Gv.F(8.0));
    Gv Q2 = (sim.C("mains_solve", outs, src).I(iLv)).AsF();
    c = c.S("mainsBar", Gv.F(4.0));
    T.C("chk", Gv.S("AUDIT řad: dvojnásobný tlak dá víc vody, ale méně než dvojnásobek"), Gv.B((Gv.Gt(Q2, (Q1 * Gv.F(1.05))) && Gv.Lt(Q2, (Q1 * Gv.F(1.99))))), (Gv.S("%.0f → %.0f L/min") % Gd.Arr(Q1, Q2)));
    Gv vv = sim.C("route_valves", Gv.S("vodovod"), Gv.S("lv"), Gd.Arr());
    Gv thr = Gv.Null;
    foreach (Gv id in (vv).Iter()) {
      Gv it2 = sim.G("net").G("byId").C("get", id);
      if ((Gv.Ne(it2, Gv.Null) && Gv.Eq(it2.G("type"), Gv.S("valve")))) {
        thr = it2;
        break;
      }
    }
    if (Gv.Ne(thr, Gv.Null)) {
      thr = thr.SI(Gv.S("part"), Gv.F(20.0));
      Gv outs2 = _mains_outs(src);
      Gv Q3 = (Gv.F(0.0)).AsF();
      foreach (Gv i_2 in (outs2.C("size")).Iter()) {
        if (Gv.Eq(outs2.I(i_2).G("d").G("att").G("bind"), Gv.S("lv"))) {
          Q3 = (sim.C("mains_solve", outs2, src).I(i_2)).AsF();
        }
      }
      thr.C("erase", Gv.S("part"));
      T.C("chk", Gv.S("AUDIT řad: přivřená klapka (20 %) sníží průtok, ale voda dál teče"), Gv.B((Gv.Lt(Q3, (Q1 * Gv.F(0.9))) && Gv.Gt(Q3, Gv.F(0.0)))), (Gv.S("%.0f → %.0f L/min") % Gd.Arr(Q1, Q3)));
    }
    Gv Q4 = (sim.C("mains_solve", outs, src).I(iLv)).AsF();
    T.C("chk", Gv.S("AUDIT řad: opakované řešení je deterministické"), Gv.B(Gv.Lt(Gd.absf((Q4 - Q1)), Gv.F(1e-09))));
    S = S.S("lv", S.G("lv").S("w", Gv.F(1000.0)));
    Gv w0 = (S.G("lv").G("w")).AsF();
    foreach (Gv i_3 in (Gv.I(30L)).Iter()) {
      sim.C("net_step", Gv.F(1.0));
    }
    Gv dw = ((S.G("lv").G("w") - w0)).AsF();
    Gv exp_ = (((Q1 / Gv.F(60.0)) * Gv.F(30.0))).AsF();
    T.C("chk", Gv.S("AUDIT řad: do ledovky za 30 s přiteče nejvýš průtok × čas a řádově tolik"), Gv.B((Gv.Gt(dw, (Gv.F(0.6) * exp_)) && Gv.Lt(dw, (Gv.F(1.02) * exp_)))), (Gv.S("%.0f L, řešení tlaku dává %.0f L") % Gd.Arr(dw, exp_)));
    src = src.SI(Gv.S("on"), Gv.B(false));
    return Gv.Null;
  }
  public Gv _rel(Gv a = default(Gv), Gv b = default(Gv)) {
    a = (a).AsF();
    b = (b).AsF();
    return ((Gd.absf((a - b)) / Gd.maxf(Gv.F(1e-12), Gd.absf(b)))).AsF();
    return Gv.Null;
  }
  public Gv _mkv(Gv Tc = default(Gv)) {
    Tc = (Tc).AsF();
    Gv v = sim.C("vessel");
    v = v.S("w", Gv.F(1000.0));
    v = v.S("T", Tc);
    v = v.S("Tsens", Tc);
    return v;
    return Gv.Null;
  }
  public Gv _reference_tests() {
    T.C("fresh");
    Gv S = sim.G("S");
    Gv c = S.G("cfg");
    Gv f5 = (sim.C("first_order", Gv.F(5.0), Gv.F(1.0))).AsF();
    T.C("chk", Gv.S("REF reakce 1. řádu: k = 5 /min, 1 min → 1 − e^−5 (dřív min(1, k·m) = 100 %)"), Gv.B((Gv.Lt(Gd.absf((f5 - ((Gv.F(1.0) - Gd.exp(Gv.F(-5.0)))))), Gv.F(1e-12)) && Gv.Lt(f5, Gv.F(1.0)))), (Gv.S("%.5f") % f5));
    Gv rest = Gv.F(1.0);
    foreach (Gv i in (Gv.I(600L)).Iter()) {
      rest = (rest * (Gv.F(1.0) - sim.C("first_order", Gv.F(0.3), (Gv.F(1.0) / Gv.F(60.0)))));
    }
    T.C("chk", Gv.S("REF reakce 1. řádu: 600 krátkých kroků = jeden dlouhý (přesné řešení)"), Gv.B(Gv.Lt(Gd.absf((((Gv.F(1.0) - rest)) - sim.C("first_order", Gv.F(0.3), Gv.F(10.0)))), Gv.F(1e-09))), (Gv.S("%.9f") % ((Gv.F(1.0) - rest))));
    Gv va = _mkv(Gv.F(63.0));
    va = va.S("aA", Gv.F(10.0));
    Gv vb = _mkv(Gv.F(63.0));
    vb = vb.S("aA", Gv.F(10.0));
    sim.C("react", va, Gv.F(30.0));
    foreach (Gv i_2 in (Gv.I(1800L)).Iter()) {
      sim.C("react", vb, (Gv.F(1.0) / Gv.F(60.0)));
    }
    Gv Aref = ((Gv.F(10.0) * Gd.exp(((-c.G("kdAlpha63")) * Gv.F(30.0))))).AsF();
    T.C("chk", Gv.S("REF α-amylasa 63 °C, 30 min: A = A0·e^(−k·t)"), Gv.B(Gv.Lt(_rel(va.G("aA"), Aref), Gv.F(1e-06))), (Gv.S("%.4f vs %.4f") % Gd.Arr(va.G("aA"), Aref)));
    T.C("chk", Gv.S("REF α-amylasa: krok 30 min a 1 s dávají totéž"), Gv.B(Gv.Lt(_rel(va.G("aA"), vb.G("aA")), Gv.F(1e-06))), (Gv.S("%.5f / %.5f") % Gd.Arr(va.G("aA"), vb.G("aA"))));
    S = S.SI(Gv.S("pH"), Gv.F(5.2));
    Gv vs1 = _mkv(Gv.F(100.0));
    vs1 = vs1.S("smm", Gv.F(10.0));
    Gv vs2 = _mkv(Gv.F(100.0));
    vs2 = vs2.S("smm", Gv.F(10.0));
    sim.C("react", vs1, Gv.F(60.0));
    foreach (Gv i_3 in (Gv.I(3600L)).Iter()) {
      sim.C("react", vs2, (Gv.F(1.0) / Gv.F(60.0)));
    }
    Gv Sref = ((Gv.F(10.0) * Gd.exp((Gv.F(-0.01824) * Gv.F(60.0))))).AsF();
    T.C("chk", Gv.S("REF SMM při 100 °C, pH 5,2, 60 min: e^(−0,01824·t)"), Gv.B(Gv.Lt(_rel(vs1.G("smm"), Sref), Gv.F(0.002))), (Gv.S("%.4f vs %.4f") % Gd.Arr(vs1.G("smm"), Sref)));
    T.C("chk", Gv.S("REF SMM: krok 60 min a 1 s dávají totéž"), Gv.B((Gv.Lt(_rel(vs1.G("smm"), vs2.G("smm")), Gv.F(1e-06)) && Gv.Lt(_rel(vs1.G("dms"), vs2.G("dms")), Gv.F(1e-06)))), (Gv.S("DMS %.5f / %.5f") % Gd.Arr(vs1.G("dms"), vs2.G("dms"))));
    T.C("chk", Gv.S("REF SMM → DMS: hmota se zachovává (SMM + DMS ≤ počáteční, zbytek se odpaří)"), Gv.B(Gv.Le((vs1.G("smm") + vs1.G("dms")), (Gv.F(10.0) + Gv.F(1e-09)))));
    Gv vh1 = _mkv(Gv.F(100.0));
    vh1 = vh1.S("alpha", Gv.F(100.0));
    Gv vh2 = _mkv(Gv.F(100.0));
    vh2 = vh2.S("alpha", Gv.F(100.0));
    sim.C("react", vh1, Gv.F(60.0));
    foreach (Gv i_4 in (Gv.I(3600L)).Iter()) {
      sim.C("react", vh2, (Gv.F(1.0) / Gv.F(60.0)));
    }
    Gv TK = (Gv.F(373.15)).AsF();
    Gv k1 = ((Gv.F(790000000000.0) * Gd.exp((Gv.F(-11858.0) / TK)))).AsF();
    Gv k2 = ((Gv.F(4100000000000.0) * Gd.exp((Gv.F(-12994.0) / TK)))).AsF();
    Gv isoRef = ((((k1 * Gv.F(100.0)) * ((Gd.exp(((-k1) * Gv.F(60.0))) - Gd.exp(((-k2) * Gv.F(60.0)))))) / ((k2 - k1)))).AsF();
    T.C("chk", Gv.S("REF chmel 100 °C, 60 min: izo-α podle analytického řešení dvou následných reakcí"), Gv.B(Gv.Lt(_rel(vh1.G("iso"), isoRef), Gv.F(1e-06))), (Gv.S("%.3f vs %.3f") % Gd.Arr(vh1.G("iso"), isoRef)));
    T.C("chk", Gv.S("REF chmel: α klesá jako e^(−k1·t) a krok nehraje roli"), Gv.B((Gv.Lt(_rel(vh1.G("alpha"), (Gv.F(100.0) * Gd.exp(((-k1) * Gv.F(60.0))))), Gv.F(1e-06)) && Gv.Lt(_rel(vh1.G("iso"), vh2.G("iso")), Gv.F(1e-05)))), (Gv.S("iso %.4f / %.4f") % Gd.Arr(vh1.G("iso"), vh2.G("iso"))));
    T.C("chk", Gv.S("REF chmel: využití izomerizací za 60 min varu je v rozumných mezích 20–65 %"), Gv.B((Gv.Gt(vh1.G("iso"), Gv.F(20.0)) && Gv.Lt(vh1.G("iso"), Gv.F(65.0)))), (Gv.S("%.1f %% α → iso") % vh1.G("iso")));
    T.C("fresh");
    S = sim.G("S");
    c = S.G("cfg");
    Gv v = S.G("rvp");
    v = v.S("w", Gv.F(4000.0));
    v = v.S("f", Gv.F(450.0));
    v = v.S("T", c.G("tbWater"));
    v = v.S("Tsens", c.G("tbWater"));
    v = v.S("stir", Gv.B(true));
    v = v.S("heat", Gv.F(0.85));
    Gv e0 = (sim.C("ext", v)).AsF();
    Gv w0 = (v.G("w")).AsF();
    Gv dr0 = (S.G("drainExt")).AsF();
    Gv dl0 = (S.G("drainL")).AsF();
    foreach (Gv i_5 in (Gv.I(900L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
    }
    Gv e1 = ((sim.C("ext", v) + ((S.G("drainExt") - dr0)))).AsF();
    Gv P1 = (((Gv.F(100.0) * sim.C("ext", v)) / ((sim.C("ext", v) + v.G("w"))))).AsF();
    T.C("chk", Gv.S("REF var: extrakt (v nádobě + odtečený) se při odparu zachová"), Gv.B(Gv.Lt(_rel(e1, e0), Gv.F(0.005))), (Gv.S("%.2f → %.2f kg") % Gd.Arr(e0, e1)));
    T.C("chk", Gv.S("REF var: vody ubývá (odpar) a °P = 100·E/(E+W) roste"), Gv.B((Gv.Lt(v.G("w"), (w0 - Gv.F(20.0))) && Gv.Gt(P1, (((Gv.F(100.0) * e0) / ((e0 + w0))) + Gv.F(0.1))))), (Gv.S("voda %.0f → %.0f kg, %.2f °P") % Gd.Arr(w0, v.G("w"), P1)));
    T.C("chk", Gv.S("REF var: plato() v modelu odpovídá 100·E/(E+W) (±0,3 °P)"), Gv.B(Gv.Lt(Gd.absf((sim.C("plato", v) - P1)), Gv.F(0.3))), (Gv.S("%.2f vs %.2f") % Gd.Arr(sim.C("plato", v), P1)));
    T.C("fresh");
    S = sim.G("S");
    c = S.G("cfg");
    Gv mm = Gv.F(1000.0);
    S = S.S("malt", S.G("malt").S("added", mm));
    Gv elab = ((((mm * ((Gv.F(1.0) - (c.G("maltMoist") / Gv.F(100.0))))) * c.G("maltExtract")) / Gv.F(100.0))).AsF();
    S = S.S("extIn", elab);
    S = S.S("flags", S.G("flags").SI(Gv.S("knock"), Gd.Dict(Gv.S("P"), Gv.F(12.0), Gv.S("e"), (Gv.F(0.807) * elab), Gv.S("V"), Gv.F(71.0))));
    Gv Y = sim.C("yields");
    T.C("chk", Gv.S("REF výtěžnost: laboratorní extrakt = slad·(1−vlhkost)·extrakt sušiny"), Gv.B(Gv.Lt(_rel(Y.G("e_lab"), elab), Gv.F(1e-09))), (Gv.S("%.1f kg") % Y.G("e_lab")));
    T.C("chk", Gv.S("REF výtěžnost: VV_varna = účinnost × e_lab/m_slad (80,7 % účinnosti ≈ 61,7 kg/100 kg)"), Gv.B((Gv.Lt(Gd.absf((Y.G("VV_varna") - ((((Y.G("ucinnost") / Gv.F(100.0)) * Y.G("e_lab")) / Y.G("m_slad")) * Gv.F(100.0)))), Gv.F(1e-09)) && Gv.Lt(Gd.absf((Y.G("VV_varna") - Gv.F(61.65))), Gv.F(0.2)))), (Gv.S("VV %.2f, účinnost %.1f %%") % Gd.Arr(Y.G("VV_varna"), Y.G("ucinnost"))));
    Gv H0 = (c.C("get", Gv.S("p1_H0"), Gv.F(32.0))).AsF();
    Gv Qn = (c.C("get", Gv.S("p1_Qn"), Gv.F(400.0))).AsF();
    Gv A = (((Gv.F(0.25) * H0) / ((Qn * Qn)))).AsF();
    Gv pp = sim.C("pump_point", Gv.S("p1"), Gv.F(5000.0), Gv.F(5.0), Gd.Arr(), Gd.Arr(), Gv.F(1.0), Gv.F(0.001), Gv.F(0.0));
    Gv Qref = (Gd.sqrt((((H0 - Gv.F(5.0))) / A))).AsF();
    T.C("chk", Gv.S("REF čerpadlo: konstantní statická výška 5 m → Q = √((H0−Hst)/A) (průsečík s parabolou)"), Gv.B(Gv.Lt(_rel(pp.G("Q"), Qref), Gv.F(0.001))), (Gv.S("%.1f vs %.1f L/min") % Gd.Arr(pp.G("Q"), Qref)));
    pp = sim.C("pump_point", Gv.S("p1"), Gv.F(300.0), Gv.F(5.0), Gd.Arr(), Gd.Arr(), Gv.F(1.0), Gv.F(0.001), Gv.F(0.0));
    Gv hzRef = ((Gv.F(50.0) * Gd.sqrt((((Gv.F(5.0) + ((A * Gv.F(300.0)) * Gv.F(300.0)))) / H0)))).AsF();
    T.C("chk", Gv.S("REF čerpadlo: měnič drží 300 L/min při frekvenci dané afinitním zákonem H ~ n²"), Gv.B((Gv.Lt(_rel(pp.G("Q"), Gv.F(300.0)), Gv.F(1e-06)) && Gv.Lt(_rel(pp.G("hz"), hzRef), Gv.F(0.001)))), (Gv.S("%.2f Hz vs %.2f Hz") % Gd.Arr(pp.G("hz"), hzRef)));
    pp = sim.C("pump_point", Gv.S("p1"), Gv.F(300.0), (H0 + Gv.F(5.0)), Gd.Arr(), Gd.Arr(), Gv.F(1.0), Gv.F(0.001), Gv.F(0.0));
    T.C("chk", Gv.S("REF čerpadlo: výška nad uzavřenou výškou H0 → žádný průtok"), Gv.B(Gv.Eq(pp.G("Q"), Gv.F(0.0))), (Gv.S("%.1f L/min") % pp.G("Q")));
    Gv D = ((sim.C("bore_mm", Gv.F(25.0)) / Gv.F(1000.0))).AsF();
    Gv Lm = (((Gv.F(1000.0) / Gv.F(100.0)) * c.G("pipeLenScale"))).AsF();
    Gv path = Gd.Arr(Gd.Dict(Gv.S("e"), Gd.Dict(Gv.S("dn"), Gv.F(25.0), Gv.S("len"), Gv.F(1000.0))));
    Gv Ql = (Gv.F(5.0)).AsF();
    Gv vl = (((Ql / Gv.F(60000.0)) / ((((Gd.K("PI") * D) * D) / Gv.F(4.0))))).AsF();
    Gv Hlam = ((((((Gv.F(128.0) * Gv.F(0.001)) * Lm) * ((Ql / Gv.F(60000.0)))) / ((((Gd.K("PI") * Gd.pow(D, Gv.F(4.0))) * Gv.F(1000.0)) * Gv.F(9.81)))) + (((Gv.F(0.4) * vl) * vl) / ((Gv.F(2.0) * Gv.F(9.81)))))).AsF();
    Gv Hm1 = (sim.C("path_head", path, Ql, Gv.F(1.0), Gv.F(0.001))).AsF();
    T.C("chk", Gv.S("REF potrubí: laminární proudění odpovídá Hagen–Poiseuille (+ místní ztráta)"), Gv.B(Gv.Lt(_rel(Hm1, Hlam), Gv.F(0.02))), (Gv.S("%.5f vs %.5f m") % Gd.Arr(Hm1, Hlam)));
    Gv Qt2 = (Gv.F(500.0)).AsF();
    Gv vt = (((Qt2 / Gv.F(60000.0)) / ((((Gd.K("PI") * D) * D) / Gv.F(4.0))))).AsF();
    Gv Ret = ((((Gv.F(1000.0) * vt) * D) / Gv.F(0.001))).AsF();
    Gv fc = (Gv.F(0.02)).AsF();
    foreach (Gv i_6 in (Gv.I(60L)).Iter()) {
      fc = (Gd.pow((Gv.F(1.0) / (((Gv.F(-2.0) * Gd.log(((Gv.F(1.5e-05) / ((Gv.F(3.7) * D))) + (Gv.F(2.51) / ((Ret * Gd.sqrt(fc))))))) * Gv.F(0.4342944819)))), Gv.F(2.0))).AsF();
    }
    Gv Hturb = ((((((((fc * Lm) / D) + Gv.F(0.4))) * vt) * vt) / ((Gv.F(2.0) * Gv.F(9.81))))).AsF();
    Gv Hm2 = (sim.C("path_head", path, Qt2, Gv.F(1.0), Gv.F(0.001))).AsF();
    T.C("chk", Gv.S("REF potrubí: turbulence – Swamee–Jain v modelu souhlasí s Colebrookem do 2 %"), Gv.B(Gv.Lt(_rel(Hm2, Hturb), Gv.F(0.02))), (Gv.S("%.3f vs %.3f m, f=%.4f") % Gd.Arr(Hm2, Hturb, fc)));
    T.C("chk", Gv.S("REF potrubí: ztráta roste s průtokem (≈ Q²) a se zavřením klapky"), Gv.B(Gv.Gt(sim.C("path_head", path, (Gv.F(2.0) * Qt2), Gv.F(1.0), Gv.F(0.001)), (Gv.F(3.3) * Hm2))), Gv.S(""));
    Gv Pm = Gd.Script("physics");
    Gv cases = Gd.Arr(Gd.Arr(Gv.F(2.0), Gv.F(1.0), Gv.F(0.5)), Gd.Arr(Gv.F(2.0), Gv.F(4.0), Gv.F(1.0)), Gd.Arr(Gv.F(1.0), Gv.F(3.0), Gv.F(3.0)), Gd.Arr(Gv.F(0.5), Gv.F(2.0), Gv.F(2.0)));
    Gv okE = Gv.B(true);
    Gv det = Gv.S("");
    foreach (Gv cs in (cases).Iter()) {
      Gv UA = (cs.I(Gv.I(0L))).AsF();
      Gv Ch = (cs.I(Gv.I(1L))).AsF();
      Gv Cc = (cs.I(Gv.I(2L))).AsF();
      Gv cmin = (Gd.minf(Ch, Cc)).AsF();
      Gv cmax = (Gd.maxf(Ch, Cc)).AsF();
      Gv cr = ((cmin / cmax)).AsF();
      Gv ntu = ((UA / cmin)).AsF();
      Gv eref = ((Gv.Lt(Gd.absf((Gv.F(1.0) - cr)), Gv.F(1e-09)) ? (ntu / ((Gv.F(1.0) + ntu))) : (((Gv.F(1.0) - Gd.exp(((-ntu) * ((Gv.F(1.0) - cr)))))) / ((Gv.F(1.0) - (cr * Gd.exp(((-ntu) * ((Gv.F(1.0) - cr)))))))))).AsF();
      Gv em = (Pm.C("exchanger_effectiveness", UA, Ch, Cc)).AsF();
      if (Gv.Gt(Gd.absf((em - eref)), Gv.F(1e-09))) {
        okE = Gv.B(false);
      }
      det = (det + (Gv.S("%.3f ") % em));
    }
    T.C("chk", Gv.S("REF výměník: ε-NTU protiproud souhlasí s uzavřeným vzorcem (Cr<1 i Cr=1)"), okE, det);
    Gv emax = (Pm.C("exchanger_effectiveness", Gv.F(1000.0), Gv.F(1.0), Gv.F(2.0))).AsF();
    T.C("chk", Gv.S("REF výměník: nekonečně velká plocha → ε → 1, nulová plocha → 0"), Gv.B((Gv.Gt(emax, Gv.F(0.999)) && Gv.Eq(Pm.C("exchanger_effectiveness", Gv.F(0.0), Gv.F(1.0), Gv.F(2.0)), Gv.F(0.0)))), (Gv.S("%.5f") % emax));
    T.C("fresh");
    S = sim.G("S");
    c = S.G("cfg");
    Gv vh = S.G("rvp");
    vh = vh.S("w", Gv.F(2000.0));
    vh = vh.S("T", Gv.F(20.0));
    vh = vh.S("Tsens", Gv.F(20.0));
    vh = vh.S("stir", Gv.B(true));
    vh = vh.S("heat", Gv.F(0.5));
    Gv Cth = ((sim.C("cC", vh) + sim.C("steelC", c.G("rvpSteel")))).AsF();
    Gv Ein = Gv.F(0.0);
    Gv Lo = Gv.F(0.0);
    Gv T0 = (vh.G("T")).AsF();
    foreach (Gv i_7 in (Gv.I(300L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
      Ein = (Ein + vh.G("Pin"));
      Lo = (Lo + (c.G("rvpUA") * ((vh.G("T") - c.G("ambientT")))));
    }
    Gv Tref = ((T0 + (((Ein - Lo)) / Cth))).AsF();
    T.C("chk", Gv.S("REF ohřev: T = T0 + (E_dodaná − E_ztracená)/C (C = voda + ocel, kJ/K)"), Gv.B(Gv.Lt(Gd.absf((vh.G("T") - Tref)), Gv.F(0.6))), (Gv.S("%.2f vs %.2f °C") % Gd.Arr(vh.G("T"), Tref)));
    return Gv.Null;
  }
  public Gv _settle(Gv whirl = default(Gv), Gv minutes = default(Gv)) {
    whirl = (whirl).AsF();
    minutes = (minutes).AsF();
    T.C("fresh");
    Gv S = sim.G("S");
    Gv v = S.G("wp");
    v = v.S("w", Gv.F(3800.0));
    v = v.S("T", Gv.F(85.0));
    v = v.S("Tsens", Gv.F(85.0));
    v = v.S("trubSus", Gv.F(10.0));
    v = v.S("trubHB", Gv.F(10.0));
    v = v.S("whirlMin", whirl);
    v = v.S("whirlNow", Gv.B(false));
    v = v.S("boiling", Gv.B(false));
    foreach (Gv i in (Gd.Dyn("int", (minutes * Gv.F(60.0)))).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
    }
    return (v.G("trubSet")).AsF();
    return Gv.Null;
  }
  public Gv _whirl_tests() {
    Gv c = sim.G("S").G("cfg");
    Gv s0 = (_settle(Gv.F(0.0), Gv.F(10.0))).AsF();
    Gv s1 = (_settle(Gv.F(1.0), Gv.F(10.0))).AsF();
    Gv s3 = (_settle(Gv.F(3.0), Gv.F(10.0))).AsF();
    Gv s8 = (_settle(Gv.F(8.0), Gv.F(10.0))).AsF();
    T.C("chk", Gv.S("REF vířivka: čím déle se vířilo, tím víc kalu se za stejný klid usadí (plynule, bez prahu)"), Gv.B(((Gv.Lt(s0, s1) && Gv.Lt(s1, s3)) && Gv.Lt(s3, s8))), (Gv.S("%.2f / %.2f / %.2f / %.2f kg") % Gd.Arr(s0, s1, s3, s8)));
    Gv kst = (c.G("trubKstill")).AsF();
    Gv kw = (c.G("trubKwhirl")).AsF();
    Gv wi = ((Gv.F(1.0) - Gd.exp((Gv.F(-8.0) / c.G("whirlTau"))))).AsF();
    Gv k8 = ((kst + (((kw - kst)) * wi))).AsF();
    Gv ref8 = ((Gv.F(9.0) * ((Gv.F(1.0) - Gd.exp(((-k8) * Gv.F(10.0))))))).AsF();
    T.C("chk", Gv.S("REF vířivka: usazená hmota = (kal − nesedimentující 10 %)·(1 − e^(−k·t))"), Gv.B(Gv.Lt(Gd.absf((s8 - ref8)), ((Gv.F(0.03) * ref8) + Gv.F(0.02)))), (Gv.S("%.3f vs %.3f kg") % Gd.Arr(s8, ref8)));
    Gv sl = (_settle(Gv.F(8.0), Gv.F(40.0))).AsF();
    T.C("chk", Gv.S("REF vířivka: po dlouhém klidu se usadí všechen kal kromě podílu, který zůstane v suspenzi (10 %)"), Gv.B((Gv.Gt(sl, Gv.F(8.5)) && Gv.Le(sl, (Gv.F(9.0) + Gv.F(1e-06))))), (Gv.S("%.2f z 9,0 kg") % sl));
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "T": return this.T;
    case "sim": return this.sim;
    case "m": return this.m;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "T": this.T = v; return v;
    case "sim": this.sim = v; return v;
    case "m": this.m = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "run": return run(Gd.A(a, 0));
    case "_open": return _open(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "_energy": return _energy();
    case "_plate": return _plate();
    case "_hlt_boil": return _hlt_boil();
    case "_draw_run": return _draw_run(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "_lauter": return _lauter();
    case "_pumps": return _pumps();
    case "_protlacka": return _protlacka();
    case "_process": return _process();
    case "_ui_regress": return _ui_regress();
    case "_water_total": return _water_total();
    case "_ext_total": return _ext_total();
    case "_spec_tests": return _spec_tests();
    case "_steel_tests": return _steel_tests();
    case "_wcal": return _wcal(Gd.A(a, 0));
    case "_delcal": return _delcal();
    case "_c1_flow": return _c1_flow();
    case "_calib_tests": return _calib_tests();
    case "_ferm_cip_tests": return _ferm_cip_tests();
    case "_practice_tests": return _practice_tests();
    case "_physics_lit_tests": return _physics_lit_tests();
    case "_mains_outs": return _mains_outs(Gd.A(a, 0));
    case "_mains_tests": return _mains_tests();
    case "_rel": return _rel(Gd.A(a, 0), Gd.A(a, 1));
    case "_mkv": return _mkv(Gd.A(a, 0));
    case "_reference_tests": return _reference_tests();
    case "_settle": return _settle(Gd.A(a, 0), Gd.A(a, 1));
    case "_whirl_tests": return _whirl_tests();
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_audit_tests(); return Gv.O(o); }
  public override string ScriptName { get { return "audit_tests"; } }
}
}
