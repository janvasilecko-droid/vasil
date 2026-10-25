// Vygenerováno z tests.gd překladačem gd2cs.py – needituj ručně.
using System; using System.Collections.Generic;
namespace Pivovar.Gd.Gen {
public partial class S_tests : GdObject {
  public Gv m;
  public Gv sim;
  public Gv results;
  static S_tests() {
  }
  public S_tests() {
    m = Gv.Null;
    sim = Gv.Null;
    results = Gd.Arr();
  }
  public Gv chk(Gv name = default(Gv), Gv ok = default(Gv), Gv detail = default(Gv)) {
    if (detail.k == 0) detail = Gv.S("");
    results.C("append", Gd.Arr(name, ok, detail));
    Gd.print((Gv.S("%s  %s%s") % Gd.Arr(((ok).T() ? Gv.S("PASS") : Gv.S("FAIL")), name, (Gv.Ne(detail, Gv.S("")) ? (((Gv.S("  [") + detail) + Gv.S("]"))) : Gv.S("")))));
    return Gv.Null;
  }
  public Gv near(Gv a = default(Gv), Gv b = default(Gv), Gv tol = default(Gv)) {
    a = (a).AsF();
    b = (b).AsF();
    tol = (tol).AsF();
    return Gv.B(Gv.Le(Gd.absf((a - b)), tol));
    return Gv.Null;
  }
  public Gv fresh() {
    foreach (Gv it__it in (m.G("items")).Iter()) {
      Gv it = it__it;
      if (Gv.Eq(it.G("type"), Gv.S("valve"))) {
        it = it.SI(Gv.S("open"), Gv.B(false));
      }
      if ((Gv.Eq(it.G("type"), Gv.S("vessel")) && Gv.Eq(it.C("get", Gv.S("shape")), Gv.S("inlet")))) {
        it = it.SI(Gv.S("on"), Gv.B(false));
      }
    }
    foreach (Gv k in (m.G("outlet_open")).Iter()) {
      m = m.S("outlet_open", m.G("outlet_open").SI(k, Gv.B(false)));
    }
    sim = sim.S("net", Gd.Dict());
    sim.C("new_state");
    sim.C("build_net", m.G("items"), m.G("plan").C("get", Gv.S("joints"), Gd.Arr()));
    m = m.S("auto", m.G("auto").S("S", sim.G("S")));
    m = m.S("auto", m.G("auto").S("on", Gv.B(false)));
    return Gv.Null;
  }
  public Gv bed() {
    fresh();
    Gv S = sim.G("S");
    Gv sk = S.G("sk");
    Gv p = sim.C("vessel");
    p = p.S("T", Gv.F(72.0));
    p = p.S("w", Gv.F(2300.0));
    p = p.S("g", Gv.F(133.0));
    p = p.S("f", Gv.F(330.0));
    p = p.S("dx", Gv.F(20.0));
    p = p.S("ns", Gv.F(70.0));
    p = p.S("ld", Gv.F(60.0));
    p = p.S("sg", Gv.F(15.0));
    p = p.S("col", Gv.F(4000.0));
    sk = sk.S("under", Gv.F(200.0));
    sk = sk.S("underOK", Gv.B(true));
    sim.C("dstGive", Gv.S("sk"), p, Gd.Dict());
    sim.C("skMashToBed");
    sk = sk.S("mode", Gv.S("lauter"));
    sk = sk.S("settle", Gv.F(30.0));
    return Gv.Null;
  }
  public Gv bed_mash_only() {
    fresh();
    Gv S = sim.G("S");
    Gv sk = S.G("sk");
    Gv p = sim.C("vessel");
    p = p.S("T", Gv.F(72.0));
    p = p.S("w", Gv.F(2300.0));
    p = p.S("g", Gv.F(133.0));
    p = p.S("f", Gv.F(330.0));
    p = p.S("dx", Gv.F(20.0));
    p = p.S("ns", Gv.F(70.0));
    p = p.S("ld", Gv.F(60.0));
    p = p.S("sg", Gv.F(15.0));
    p = p.S("col", Gv.F(4000.0));
    sk = sk.S("under", Gv.F(200.0));
    sk = sk.S("underOK", Gv.B(true));
    sim.C("dstGive", Gv.S("sk"), p, Gd.Dict());
    return Gv.Null;
  }
  public Gv run() {
    sim = m.G("sim");
    fresh();
    Gv S = sim.G("S");
    Gv dissolved = (((Gv.F(17.0) * Gv.F(16.0)) / Gv.F(84.0))).AsF();
    Gv collected = ((((Gv.F(26.0) * Gv.F(0.97)) * Gv.F(1.043)) * Gv.F(0.107))).AsF();
    Gv additional = ((((Gv.F(0.6) * Gv.F(0.97)) * Gv.F(1.024)) * Gv.F(0.06))).AsF();
    Gv remaining = (((dissolved - collected) - additional)).AsF();
    Gv retained = ((Gv.F(4.3) * Gv.F(1.56))).AsF();
    Gv water_after = (((retained - (remaining * Gv.F(0.62))) + Gv.F(8.6))).AsF();
    Gv pred = (((Gv.F(100.0) * remaining) / ((remaining + water_after)))).AsF();
    chk(Gv.S("Troester: zbývající extrakt 0,38776 kg"), near(remaining, Gv.F(0.38776), Gv.F(0.0001)), (Gv.S("%.5f") % remaining));
    chk(Gv.S("Troester: predikce promývacího vzorku 2,509 °P (retence 1,56 L/kg)"), near(pred, Gv.F(2.509), Gv.F(0.01)), (Gv.S("%.3f") % pred));
    Gv pred_bad = (((Gv.F(100.0) * remaining) / ((remaining + ((((Gv.F(4.3) * Gv.F(0.8)) - (remaining * Gv.F(0.62))) + Gv.F(8.6))))))).AsF();
    chk(Gv.S("Troester: s chybnou retencí 0,8 L/kg vychází 3,182 °P"), near(pred_bad, Gv.F(3.182), Gv.F(0.01)), (Gv.S("%.3f") % pred_bad));
    S = S.S("sk", S.G("sk").S("g", ((((Gv.F(4.3) * Gv.F(0.2)) * Gv.F(0.955)) * Gv.F(0.191)) / Gv.F(0.191))));
    S = S.S("sk", S.G("sk").S("g", (Gv.F(4.3) * Gv.F(0.191))));
    chk(Gv.S("skRet() = skMalt * skRetentionLkg"), near(sim.C("skRet"), (sim.C("skMalt") * S.G("cfg").G("skRetentionLkg")), Gv.F(1e-06)), (Gv.S("%.3f") % sim.C("skRet")));
    Gv big = sim.C("lp");
    big = big.S("w", Gv.F(1000.0));
    big = big.S("f", Gv.F(100.0));
    Gv small = sim.C("lp");
    small = small.S("w", Gv.F(0.3));
    small = small.S("f", Gv.F(0.03));
    chk(Gv.S("°P v dávce 0,3 L je stejné jako ve velké (plato)"), near(sim.C("plato", big), sim.C("plato", small), Gv.F(1e-09)), (Gv.S("%.3f / %.3f") % Gd.Arr(sim.C("plato", big), sim.C("plato", small))));
    Gv cell_small = Gd.Dict(Gv.S("w"), Gv.F(0.3), Gv.S("f"), Gv.F(0.03), Gv.S("n"), Gv.F(0.0), Gv.S("smm"), Gv.F(0.0), Gv.S("dms"), Gv.F(0.0), Gv.S("col"), Gv.F(0.0));
    chk(Gv.S("°P v malé buňce lože (cellP)"), near(sim.C("cellP", cell_small), sim.C("plato", small), Gv.F(1e-09)));
    Gv io = sim.C("lp");
    io = io.S("su", Gv.F(10.0));
    io = io.S("sc", Gv.F(5.0));
    io = io.S("dx", Gv.F(1.0));
    io = io.S("f", Gv.F(100.0));
    chk(Gv.S("jodová zkouška kapalného vzorku je negativní"), Gv.B((Gv.Eq(sim.C("iodine", io), Gv.Null) || Gv.Eq(sim.C("iodine", io), Gv.S("neg")))), Gd.Dyn("str", sim.C("iodine", io)));
    chk(Gv.S("jodová zkouška včetně zrn ukáže nezcukřený škrob"), Gv.B(Gv.Eq(sim.C("iodine", io, Gv.B(true)), Gv.S("pos"))), Gd.Dyn("str", sim.C("iodine", io, Gv.B(true))));
    Gv old = S.C("duplicate", Gv.B(true));
    old.G("cfg").C("erase", Gv.S("skRetentionLkg"));
    old.G("sk").C("erase", Gv.S("fines"));
    old.G("sk").C("erase", Gv.S("ch"));
    Gv fr = m.G("Sim").C("new");
    fr = fr.S("main", m);
    fr.C("new_state");
    m.C("_merge_defaults", old, fr.G("S"));
    chk(Gv.S("starší uložení se doplní o nová pole (retence, částice, kanálky)"), Gv.B((((old.G("cfg").C("has", Gv.S("skRetentionLkg"))).T() && (old.G("sk").C("has", Gv.S("fines"))).T()) && (old.G("sk").C("has", Gv.S("ch"))).T())));
    Gv d1 = sim.C("lp");
    d1 = d1.S("w", Gv.F(100.0));
    d1 = d1.S("T", Gv.F(20.0));
    Gv p1 = sim.C("lp");
    p1 = p1.S("w", Gv.F(100.0));
    p1 = p1.S("T", Gv.F(80.0));
    sim.C("addTo", d1, p1, Gv.F(0.0));
    chk(Gv.S("míchání 100 L 20 °C + 100 L 80 °C = 50 °C"), Gv.B(((near(d1.G("T"), Gv.F(50.0), Gv.F(0.01))).T() && (near(d1.G("w"), Gv.F(200.0), Gv.F(1e-09))).T())), (Gv.S("%.2f") % d1.G("T")));
    Gv e = Gd.Dict(Gv.S("cap"), Gv.F(10.0), Gv.S("c"), sim.C("lp"), Gv.S("q"), Gd.Arr(), Gv.S("from"), Gv.S("a"), Gv.S("flow"), Gv.I(0L));
    Gv mk = Gd.Fn((_a) => { Gv w = Gd.A(_a, 0); Gv T = Gd.A(_a, 1); Gv ex = Gd.A(_a, 2); Gv q = sim.C("lp"); q = q.S("w", w); q = q.S("T", T); q = q.S("f", ex); return q; return Gv.Null; });
    Gv acc = Gd.Dict(Gv.S("in"), Gv.F(0.0), Gv.S("out"), Gv.F(0.0));
    Gv feed = Gd.Fn((_a) => { Gv w_2 = Gd.A(_a, 0); Gv T_2 = Gd.A(_a, 1); Gv ex_2 = Gd.A(_a, 2); Gv from_a = Gd.A(_a, 3); Gv part = mk.C("call", w_2, T_2, ex_2); Gv o = sim.C("edge_push", e, part, from_a); acc = acc.SI(Gv.S("in"), (acc.I(Gv.S("in")) + w_2)); acc = acc.SI(Gv.S("out"), (acc.I(Gv.S("out")) + o.G("w"))); return o; return Gv.Null; });
    Gv o1 = feed.C("call", Gv.F(5.0), Gv.F(20.0), Gv.F(0.0), Gv.B(true));
    Gv o2 = feed.C("call", Gv.F(5.0), Gv.F(80.0), Gv.F(0.0), Gv.B(true));
    chk(Gv.S("FIFO: dokud se trubka nezaplní, nic nevyteče"), Gv.B((Gv.Lt(o1.G("w"), Gv.F(1e-09)) && Gv.Lt(o2.G("w"), Gv.F(1e-09)))));
    Gv o3 = feed.C("call", Gv.F(5.0), Gv.F(30.0), Gv.F(0.0), Gv.B(true));
    chk(Gv.S("FIFO: nejdřív vyteče nejstarší dávka (20 °C), ne průměr"), Gv.B(((near(o3.G("w"), Gv.F(5.0), Gv.F(1e-06))).T() && (near(o3.G("T"), Gv.F(20.0), Gv.F(0.01))).T())), (Gv.S("%.2f L %.1f °C") % Gd.Arr(o3.G("w"), o3.G("T"))));
    Gv o4 = feed.C("call", Gv.F(5.0), Gv.F(30.0), Gv.F(0.0), Gv.B(true));
    chk(Gv.S("FIFO: teplá dávka (80 °C) přijde se zpožděním"), near(o4.G("T"), Gv.F(80.0), Gv.F(0.01)), (Gv.S("%.1f °C") % o4.G("T")));
    Gv o5 = feed.C("call", Gv.F(6.0), Gv.F(80.0), Gv.F(0.0), Gv.B(false));
    Gv o6 = feed.C("call", Gv.F(6.0), Gv.F(50.0), Gv.F(0.0), Gv.B(false));
    chk(Gv.S("FIFO: po obrácení toku vytéká obsah ze strany a (30 °C)"), Gv.B(((near(o5.G("T"), Gv.F(30.0), Gv.F(0.01))).T() && (near(o5.G("w"), Gv.F(6.0), Gv.F(1e-06))).T())), (Gv.S("%.1f °C") % o5.G("T")));
    Gv exp_T = (((((Gv.F(4.0) * Gv.F(30.0)) + (Gv.F(2.0) * Gv.F(80.0)))) / Gv.F(6.0))).AsF();
    chk(Gv.S("FIFO: po obrácení se pořadí obrátí (směs 4 L 30 °C + 2 L 80 °C)"), near(o6.G("T"), exp_T, Gv.F(0.2)), (Gv.S("%.1f / %.1f °C") % Gd.Arr(o6.G("T"), exp_T)));
    Gv stored = (e.G("c").G("w")).AsF();
    chk(Gv.S("FIFO: zachování vody (vtok = výtok + obsah)"), near(acc.I(Gv.S("in")), (acc.I(Gv.S("out")) + stored), Gv.F(1e-06)), (Gv.S("%.3f vs %.3f") % Gd.Arr(acc.I(Gv.S("in")), (acc.I(Gv.S("out")) + stored))));
    Gv qsum = Gv.F(0.0);
    foreach (Gv b in (e.G("q")).Iter()) {
      qsum = (qsum + b.G("w"));
    }
    chk(Gv.S("FIFO: agregát obsahu odpovídá součtu dávek"), near(qsum, e.G("c").G("w"), Gv.F(1e-06)));
    Gv Vm = (((((Gd.K("PI") * Gd.pow((Gv.F(50.0) / Gv.F(1000.0)), Gv.I(2L))) / Gv.F(4.0)) * Gv.F(1.0)) * Gv.F(1000.0))).AsF();
    chk(Gv.S("objem DN50 je asi 1,9635 L/m"), near(Vm, Gv.F(1.9635), Gv.F(0.001)), (Gv.S("%.4f") % Vm));
    bed();
    S = sim.G("S");
    Gv sk = S.G("sk");
    sim.C("skQmax");
    Gv q0 = (sk.G("Qmax")).AsF();
    chk(Gv.S("Darcy: průtok lože je ve smysluplném rozsahu (40–200 L/min)"), Gv.B((Gv.Gt(q0, Gv.F(40.0)) && Gv.Lt(q0, Gv.F(200.0)))), (Gv.S("%.0f L/min") % q0));
    sk = sk.S("c", Gv.F(1.0));
    sim.C("skQmax");
    chk(Gv.S("Darcy: zhutnění snižuje propustnost"), Gv.B(Gv.Lt(sk.G("Qmax"), (q0 * Gv.F(0.5)))), (Gv.S("%.0f -> %.0f") % Gd.Arr(q0, sk.G("Qmax"))));
    sk = sk.S("c", Gv.F(0.0));
    sim.C("skQmax");
    Gv hot = (sk.G("Qmax")).AsF();
    sk = sk.S("T", Gv.F(40.0));
    sim.C("skQmax");
    chk(Gv.S("Darcy: studená (viskóznější) sladina teče pomaleji"), Gv.B(Gv.Lt(sk.G("Qmax"), hot)), (Gv.S("%.0f -> %.0f") % Gd.Arr(hot, sk.G("Qmax"))));
    sk = sk.S("T", Gv.F(72.0));
    sim.C("skQmax");
    bed();
    S = sim.G("S");
    sk = S.G("sk");
    sim.C("skQmax");
    Gv qm = (sk.G("Qmax")).AsF();
    Gv c_before = (sk.G("c")).AsF();
    Gv volsum = Gv.F(0.0);
    foreach (Gv i in (Gv.I(20L)).Iter()) {
      sim.C("skQmax");
      sk = sk.S("Qact", Gv.F(0.0));
      Gv pp = sim.C("skDraw", (qm * Gv.F(2.5)), ((Gv.F(1.0) / Gv.F(60.0)) * Gv.F(3.0)));
      volsum = (volsum + sim.C("liqVol", pp));
    }
    chk(Gv.S("odběr 2,5× nad propustností nevytvoří víc kapaliny (max Qmax)"), Gv.B(Gv.Le(volsum, (((qm * Gv.F(3.0)) / Gv.F(1.0)) * Gv.F(1.001)))), (Gv.S("%.0f L za 3 min, Qmax %.0f") % Gd.Arr(volsum, qm)));
    chk(Gv.S("příliš rychlý odběr zhutní lože a vytvoří kanálky"), Gv.B((Gv.Gt(sk.G("c"), (c_before + Gv.F(0.05))) && Gv.Gt(sk.G("ch"), Gv.F(0.0)))), (Gv.S("c %.2f ch %.2f") % Gd.Arr(sk.G("c"), sk.G("ch"))));
    bed();
    S = sim.G("S");
    sk = S.G("sk");
    sim.C("skAddWater", Gv.F(1500.0), Gv.F(78.0), Gv.B(false));
    sim.C("skQmax");
    Gv calm = sim.C("skDraw", (sk.G("Qmax") * Gv.F(0.5)), Gv.F(0.5));
    Gv P_calm = (sim.C("plato", calm)).AsF();
    bed();
    S = sim.G("S");
    sk = S.G("sk");
    sim.C("skAddWater", Gv.F(1500.0), Gv.F(78.0), Gv.B(false));
    sk = sk.S("ch", Gv.F(0.4));
    sim.C("skQmax");
    Gv chan = sim.C("skDraw", (sk.G("Qmax") * Gv.F(0.5)), Gv.F(0.5));
    Gv P_chan = (sim.C("plato", chan)).AsF();
    chk(Gv.S("kanálkování: část toku obtéká lože, sladina je řidší (extrakt zůstává v mlátě)"), Gv.B(Gv.Lt(P_chan, (P_calm - Gv.F(0.05)))), (Gv.S("%.2f vs %.2f °P") % Gd.Arr(P_chan, P_calm)));
    Gv res = Gd.Arr();
    foreach (Gv pos in (Gd.Arr(Gv.F(0.05), Gv.F(0.4), Gv.F(1.0))).Iter()) {
      bed();
      S = sim.G("S");
      sk = S.G("sk");
      sk = sk.S("c", Gv.F(1.0));
      sk = sk.S("rake", Gv.B(true));
      sk = sk.S("rakeH", pos);
      Gv f0 = (sk.G("fines")).AsF();
      foreach (Gv i_2 in (Gv.I(600L)).Iter()) {
        sim.C("sim_step", Gv.F(1.0));
      }
      res.C("append", Gd.Arr(sk.G("c"), (Gv.Gt(sk.G("fines"), f0) ? (sk.G("fines") - f0) : Gv.F(0.0))));
    }
    chk(Gv.S("kopačka nahoře nekypří (žádné kouzlo)"), Gv.B(((near(res.I(Gv.I(0L)).I(Gv.I(0L)), Gv.F(1.0), Gv.F(0.01))).T() && Gv.Lt(res.I(Gv.I(0L)).I(Gv.I(1L)), Gv.F(1e-06)))), (Gv.S("c=%.2f") % res.I(Gv.I(0L)).I(Gv.I(0L))));
    chk(Gv.S("mírné řezání povolí lože"), Gv.B(Gv.Lt(res.I(Gv.I(1L)).I(Gv.I(0L)), Gv.F(0.95))), (Gv.S("c=%.2f") % res.I(Gv.I(1L)).I(Gv.I(0L))));
    chk(Gv.S("hluboké řezání u síta uvolní mnohem víc částic než mírné"), Gv.B(Gv.Gt(res.I(Gv.I(2L)).I(Gv.I(1L)), (res.I(Gv.I(1L)).I(Gv.I(1L)) * Gv.F(3.0)))), (Gv.S("%.3f vs %.3f kg") % Gd.Arr(res.I(Gv.I(2L)).I(Gv.I(1L)), res.I(Gv.I(1L)).I(Gv.I(1L)))));
    bed();
    S = sim.G("S");
    sk = S.G("sk");
    Gv mass0 = (((sk.G("g") + sk.G("fines")) + sk.G("teig"))).AsF();
    Gv clear_after = Gv.F(-1.0);
    Gv litres = Gv.F(0.0);
    foreach (Gv i_3 in (Gv.I(400L)).Iter()) {
      sim.C("skQmax");
      sk = sk.S("Qact", Gv.F(0.0));
      Gv pv = sim.C("skDraw", Gd.minf(Gv.F(40.0), (sk.G("Qmax") * Gv.F(0.5))), ((Gv.F(1.0) / Gv.F(60.0)) * Gv.F(6.0)));
      litres = (litres + sim.C("liqVol", pv));
      sim.C("dstGive", Gv.S("sk"), pv, Gd.Dict(Gv.S("src"), Gv.S("sk")));
      if ((Gv.Lt(sk.G("turb"), Gv.F(20.0)) && Gv.Lt(clear_after, Gv.F(0.0)))) {
        clear_after = litres;
      }
    }
    chk(Gv.S("vorlauf skutečně vyčistí kapalinu (zákal < 20)"), Gv.B((Gv.Gt(clear_after, Gv.F(0.0)) && Gv.Lt(sk.G("turb"), Gv.F(20.0)))), (Gv.S("čirá po %.0f L, zákal %.1f") % Gd.Arr(clear_after, sk.G("turb"))));
    chk(Gv.S("vorlauf zachovává hmotnost mláta a částic"), near(((sk.G("g") + sk.G("fines")) + sk.G("teig")), mass0, Gv.F(1e-06)), (Gv.S("%.4f vs %.4f") % Gd.Arr(((sk.G("g") + sk.G("fines")) + sk.G("teig")), mass0)));
    bed();
    S = sim.G("S");
    sk = S.G("sk");
    Gv f_before = (sk.G("fines")).AsF();
    Gv t1 = (sk.G("turb")).AsF();
    sim.C("skQmax");
    sk = sk.S("Qact", Gv.F(0.0));
    sim.C("skDraw", Gv.F(30.0), Gv.F(0.01));
    chk(Gv.S("samotné přepnutí fáze nevyčistí kapalinu (zákal zůstává vysoký po čerstvém loži)"), Gv.B((Gv.Gt(sk.G("fines"), (f_before * Gv.F(0.9))) && Gv.Gt(sk.G("turb"), Gv.F(20.0)))), (Gv.S("zákal %.0f") % sk.G("turb")));
    fresh();
    S = sim.G("S");
    Gv best = Gv.Null;
    foreach (Gv ed in (sim.G("net").G("edges")).Iter()) {
      if (Gv.Gt(ed.G("cap"), Gv.F(3.0))) {
        best = ed;
        break;
      }
    }
    if (Gv.Ne(best, Gv.Null)) {
      Gv mid = best.G("pts").I(Gv.I(0L)).C("lerp", best.G("pts").I((best.G("pts").C("size") - Gv.I(1L))), Gv.F(0.5));
      Gv batch = sim.C("lp");
      batch = batch.S("w", Gv.F(0.4));
      batch = batch.S("f", Gv.F(0.04));
      batch = batch.S("T", Gv.F(70.0));
      best = best.S("from", Gv.S("a"));
      sim.C("edge_push", best, batch, Gv.B(true));
      Gv SW = Gd.Script("sight").C("new");
      SW = SW.S("m", m);
      Gv smp = SW.C("take_sample", (best.G("pts").I(Gv.I(0L)) + (((best.G("pts").I(Gv.I(1L)) - best.G("pts").I(Gv.I(0L)))) * Gv.F(0.02))));
      chk(Gv.S("průhledítko: dávka menší než 1 L má správné °P a teplotu"), Gv.B(((!(smp.C("get", Gv.S("empty"), Gv.B(true))).T() && (near(smp.G("P"), Gv.F(9.09), Gv.F(0.1))).T()) && (near(smp.G("T"), Gv.F(70.0), Gv.F(0.1))).T())), (Gv.S("%s °P") % Gd.Dyn("str", smp.C("get", Gv.S("P")))));
      Gv far = SW.C("take_sample", best.G("pts").I((best.G("pts").C("size") - Gv.I(1L))));
      chk(Gv.S("průhledítko na druhém konci úseku je (zatím) prázdné – vzorek je z konkrétního místa"), Gv.B(((far.C("get", Gv.S("empty"), Gv.B(false))).T() || (far.C("get", Gv.S("none"), Gv.B(false))).T())));
      SW.C("free");
      best = best.S("flow", Gv.F(0.0));
      chk(Gv.S("stojící proud je označen jako stojící (průtok 0)"), Gv.B(Gv.Eq(best.G("flow"), Gv.F(0.0))));
    } else {
      chk(Gv.S("průhledítko: existuje úsek potrubí"), Gv.B(false));
    }
    fresh();
    S = sim.G("S");
    Gv vs = sim.C("route_valves", Gv.S("hlt"), Gv.S("rvp"), Gd.Arr(Gv.S("phw")));
    chk(Gv.S("cesta bojler -> pánev existuje"), Gv.B(Gv.Ne(vs, Gv.Null)));
    if (Gv.Ne(vs, Gv.Null)) {
      foreach (Gv id in (vs).Iter()) {
        if ((m.G("outlet_open").C("has", id)).T()) {
          m = m.S("outlet_open", m.G("outlet_open").SI(id, Gv.B(true)));
        } else if ((sim.G("net").G("byId").C("has", id)).T()) {
          sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(id, sim.G("net").G("byId").I(id).SI(Gv.S("open"), Gv.B(true)))));
        }
      }
      sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
      Gv hlt0 = (S.G("hlt").G("w")).AsF();
      S = S.S("rvpWater", S.G("rvpWater").S("target", Gv.F(800.0)));
      S = S.S("rvpWater", S.G("rvpWater").S("on", Gv.B(true)));
      S = S.S("mixAuto", Gv.B(true));
      S = S.S("rvpWater", S.G("rvpWater").S("T", Gv.F(60.0)));
      foreach (Gv i_4 in (Gv.I(900L)).Iter()) {
        sim.C("sim_step", Gv.F(1.0));
      }
      Gv pipes_w = Gv.F(0.0);
      foreach (Gv ed_2 in (sim.G("net").G("edges")).Iter()) {
        pipes_w = (pipes_w + ed_2.G("c").G("w"));
      }
      Gv used = ((hlt0 - S.G("hlt").G("w"))).AsF();
      Gv in_sys = ((((S.G("rvp").G("w") + pipes_w) + S.G("drainL")) + S.G("spillL"))).AsF();
      chk(Gv.S("bilance vody: z bojleru (+ studená z řadu) = pánev + potrubí + ztráty"), Gv.B((Gv.Ge(in_sys, (used * Gv.F(0.98))) && Gv.Le(in_sys, (used * Gv.F(1.6))))), (Gv.S("bojler -%.0f L, pánev+potrubí+ztráty %.0f L") % Gd.Arr(used, in_sys)));
      chk(Gv.S("napouštění zastaví při cílovém objemu"), Gv.B((Gv.Le(S.G("rvp").G("w"), (Gv.F(800.0) + Gv.F(20.0))) && Gv.Ge(S.G("rvp").G("w"), Gv.F(780.0)))), (Gv.S("%.0f L") % S.G("rvp").G("w")));
    }
    Gv Pm = Gd.Script("physics");
    Gv one = Pm.C("chain", Gv.F(100.0), Gv.F(20.0), Gv.F(0.05), Gv.F(0.01), Gv.F(80.0));
    Gv many = Gd.Arr(Gv.F(100.0), Gv.F(20.0));
    foreach (Gv i_5 in (Gv.I(1600L)).Iter()) {
      many = Pm.C("chain", many.I(Gv.I(0L)), many.I(Gv.I(1L)), Gv.F(0.05), Gv.F(0.01), Gv.F(0.05));
    }
    chk(Gv.S("reakce A->B->ztráta nezávisí na dělení kroku"), Gv.B(Gv.Lt((Gd.absf((one.I(Gv.I(0L)) - many.I(Gv.I(0L)))) + Gd.absf((one.I(Gv.I(1L)) - many.I(Gv.I(1L))))), Gv.F(1e-08))));
    chk(Gv.S("výměník: konečná účinnost a limit stejných kapacit"), Gv.B(Gv.Lt(Gd.absf((Pm.C("exchanger_effectiveness", Gv.F(10.0), Gv.F(5.0), Gv.F(5.0)) - (Gv.F(2.0) / Gv.F(3.0)))), Gv.F(1e-09))));
    Gv wall = Pm.C("exchange_bodies", Gv.F(80.0), Gv.F(100.0), Gv.F(20.0), Gv.F(50.0), Gv.F(2.0), Gv.F(3600.0));
    chk(Gv.S("výměna tepla těleso/stěna zachovává citelné teplo"), Gv.B(Gv.Lt(Gd.absf((((wall.I(Gv.I(0L)) * Gv.F(100.0)) + (wall.I(Gv.I(1L)) * Gv.F(50.0))) - (((Gv.F(80.0) * Gv.F(100.0)) + (Gv.F(20.0) * Gv.F(50.0)))))), Gv.F(1e-06))));
    fresh();
    S = sim.G("S");
    Gv sk2 = S.G("sk");
    sk2 = sk2.S("Tw", Gv.F(20.0));
    Gv spr = sim.C("lp");
    spr = spr.S("w", Gv.F(2000.0));
    spr = spr.S("T", Gv.F(78.0));
    sim.C("skWallHeat", spr);
    chk(Gv.S("sprchování nezahřeje stěnu nad teplotu vody"), Gv.B((Gv.Le(sk2.G("Tw"), Gv.F(78.0)) && Gv.Ge(spr.G("T"), Gv.F(20.0)))));
    S = S.S("hlt", S.G("hlt").S("w", Gv.F(11990.0)));
    Gv inc = sim.C("lp");
    inc = inc.S("w", Gv.F(25.0));
    inc = inc.S("T", Gv.F(70.0));
    sim.C("dstGive", Gv.S("hlt"), inc);
    chk(Gv.S("plný bojler přijme jen zbývající objem, zbytek přeteče"), Gv.B(((near(S.G("hlt").G("w"), Gv.F(12000.0), Gv.F(1e-06))).T() && (near(S.G("drainL"), Gv.F(15.0), Gv.F(1e-06))).T())));
    bed_mash_only();
    chk(Gv.S("SK s rmutem je ve stavu rmut (bez lože)"), Gv.B((Gv.Eq(sim.G("S").G("sk").G("mode"), Gv.S("mash")) && Gv.Gt(sim.C("vol", sim.G("S").G("sk").G("mash")), Gv.F(1000.0)))));
    sim = sim.S("S", sim.G("S").S("p2", sim.G("S").G("p2").S("on", Gv.B(true))));
    foreach (Gv rt in (sim.C("route_valves", Gv.S("sk"), Gv.S("sk"), Gd.Arr(Gv.S("p2")), Gv.I(0L))).Iter()) {
      if ((m.G("outlet_open").C("has", rt)).T()) {
        m = m.S("outlet_open", m.G("outlet_open").SI(rt, Gv.B(true)));
      } else if ((sim.G("net").G("byId").C("has", rt)).T()) {
        sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(rt, sim.G("net").G("byId").I(rt).SI(Gv.S("open"), Gv.B(true)))));
      }
    }
    sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
    sim.C("net_step", Gv.F(1.0));
    chk(Gv.S("C2 se v režimu rmut zastaví (nevytvoří z rmutu lože)"), Gv.B((!(sim.G("S").G("p2").G("on")).T() && Gv.Lt(sim.G("S").G("sk").G("g"), Gv.F(1.0)))));
    sim.C("start_lautering");
    chk(Gv.S("Odrmutovat vytvoří lože a zapamatuje počáteční extrakt"), Gv.B(((Gv.Eq(sim.G("S").G("sk").G("mode"), Gv.S("lauter")) && Gv.Gt(sim.G("S").G("sk").G("g"), Gv.F(20.0))) && Gv.Gt(sim.G("S").G("sk").G("initialExtract"), Gv.F(100.0)))), (Gv.S("extrakt %.0f kg") % sim.G("S").G("sk").G("initialExtract")));
    chk(Gv.S("automat scezování odmítne start před odrmutováním"), (Gd.Fn((_a) => {  fresh(); return Gv.B(!(sim.G("lauter_auto").C("start")).T()); return Gv.Null; })).C("call"));
    fresh();
    S = sim.G("S");
    Gv tr = sim.G("training");
    Gv cur = tr.C("current");
    chk(Gv.S("postup: první krok (kontrola) je splněn a jde potvrdit"), Gv.B(((cur.G("ready")).T() && !(cur.G("complete")).T())));
    Gv water0 = (S.G("hlt").G("w")).AsF();
    Gv T0 = (S.G("rvp").G("T")).AsF();
    tr.C("confirm");
    chk(Gv.S("postup: potvrzení posune krok, ale nemění vodu ani teplotu"), Gv.B(((Gv.Eq(tr.C("state").G("index"), Gv.I(1L)) && (near(S.G("hlt").G("w"), water0, Gv.F(1e-09))).T()) && (near(S.G("rvp").G("T"), T0, Gv.F(1e-09))).T())));
    tr.C("confirm");
    Gv c2 = tr.C("current");
    chk(Gv.S("postup: krok „napustit vodu“ se nedá potvrdit, dokud není voda v pánvi"), Gv.B((!(c2.G("ready")).T() && !(tr.C("confirm")).T())));
    Gv smp_2 = sim.C("sight_sample", sim.G("net").G("byId").I(Gv.S("inl_mur9u2hg")));
    chk(Gv.S("sight_sample vrací stav připojení a průtok"), Gv.B(((smp_2.C("has", Gv.S("connected"))).T() && (smp_2.C("has", Gv.S("flow"))).T())));
    fresh();
    S = sim.G("S");
    Gv vs2 = sim.C("route_valves", Gv.S("hlt"), Gv.S("rvp"), Gd.Arr(Gv.S("phw")));
    foreach (Gv id_2 in (vs2).Iter()) {
      if ((m.G("outlet_open").C("has", id_2)).T()) {
        m = m.S("outlet_open", m.G("outlet_open").SI(id_2, Gv.B(true)));
      } else if ((sim.G("net").G("byId").C("has", id_2)).T()) {
        sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(id_2, sim.G("net").G("byId").I(id_2).SI(Gv.S("open"), Gv.B(true)))));
      }
    }
    sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
    S = S.S("hlt", S.G("hlt").S("w", Gv.F(160.0)));
    S = S.S("mixAuto", Gv.B(false));
    S = S.S("rvpWater", S.G("rvpWater").S("target", Gv.F(5000.0)));
    S = S.S("rvpWater", S.G("rvpWater").S("on", Gv.B(true)));
    Gv wbefore = (Gv.F(160.0)).AsF();
    foreach (Gv i_6 in (Gv.I(60L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
    }
    Gv pipes2 = Gv.F(0.0);
    foreach (Gv ed_3 in (sim.G("net").G("edges")).Iter()) {
      pipes2 = (pipes2 + ed_3.G("c").G("w"));
    }
    chk(Gv.S("prázdný bojler (rezerva 150 L) nevyrobí vodu"), Gv.B((Gv.Ge(S.G("hlt").G("w"), Gv.F(149.9)) && Gv.Le((((S.G("rvp").G("w") + pipes2) + S.G("drainL")) + S.G("spillL")), ((wbefore - S.G("hlt").G("w")) + Gv.F(0.5))))), (Gv.S("pánev %.1f L, potrubí %.1f L, usedHot %.1f usedWater %.1f lv %.1f drain %.1f spill %.1f mix %s") % Gd.Arr(S.G("rvp").G("w"), pipes2, S.G("usedHot"), S.G("usedWater"), S.G("lv").G("w"), S.G("drainL"), S.G("spillL"), Gd.Dyn("str", S.G("mixAuto")))));
    Gv pq = Gd.Fn((_a) => { Gv mash = Gd.A(_a, 0); Gv stir = Gd.A(_a, 1); Gv clog = Gd.A(_a, 2); fresh(); S = sim.G("S"); foreach (Gv id_3 in (sim.C("route_valves", Gv.S("rvp"), Gv.S("sk"), Gd.Arr(Gv.S("p1")))).Iter()) {   if ((m.G("outlet_open").C("has", id_3)).T()) {     m = m.S("outlet_open", m.G("outlet_open").SI(id_3, Gv.B(true)));   } else if ((sim.G("net").G("byId").C("has", id_3)).T()) {     sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(id_3, sim.G("net").G("byId").I(id_3).SI(Gv.S("open"), Gv.B(true)))));   } } sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L))); Gv rv = S.G("rvp"); rv = rv.S("w", Gv.F(2000.0)); rv = rv.S("T", Gv.F(63.0)); rv = rv.S("Tsens", Gv.F(63.0)); rv = rv.S("stir", stir); if ((mash).T()) {   rv = rv.S("g", Gv.F(150.0));   rv = rv.S("su", Gv.F(250.0));   rv = rv.S("sg", Gv.F(30.0));   rv = rv.S("f", Gv.F(150.0)); } S = S.S("clog", S.G("clog").S("rvp", clog)); S = S.S("p1", S.G("p1").S("flow", Gv.F(250.0))); S = S.S("p1", S.G("p1").S("on", Gv.B(true))); S = S.S("p1", S.G("p1").S("prime", Gv.F(1.0))); sim.C("net_step", Gv.F(1.0)); return (S.G("p1").G("q")).AsF(); return Gv.Null; });
    Gv q_water = (pq.C("call", Gv.B(false), Gv.B(true), Gv.F(0.0))).AsF();
    Gv q_mash = (pq.C("call", Gv.B(true), Gv.B(true), Gv.F(0.0))).AsF();
    Gv q_clog = (pq.C("call", Gv.B(true), Gv.B(false), Gv.F(1.0))).AsF();
    chk(Gv.S("C1: hustý rmut teče pomaleji než voda (viskozita suspenze)"), Gv.B((Gv.Gt(q_water, Gv.F(1.0)) && Gv.Lt(q_mash, (q_water * Gv.F(0.85))))), (Gv.S("voda %.0f, rmut %.0f L/min") % Gd.Arr(q_water, q_mash)));
    chk(Gv.S("C1: usazené zrno ve výpusti průtok rmutu dál škrtí"), Gv.B(Gv.Lt(q_clog, (q_mash * Gv.F(0.5)))), (Gv.S("%.0f L/min") % q_clog));
    fresh();
    S = sim.G("S");
    S = S.S("rvp", S.G("rvp").S("w", Gv.F(2000.0)));
    S = S.S("rvp", S.G("rvp").S("g", Gv.F(150.0)));
    S = S.S("rvp", S.G("rvp").S("su", Gv.F(250.0)));
    S = S.S("rvp", S.G("rvp").S("sg", Gv.F(30.0)));
    S = S.S("rvp", S.G("rvp").S("stir", Gv.B(false)));
    foreach (Gv i_7 in (Gv.I(600L)).Iter()) {
      sim.C("net_step", Gv.F(1.0));
    }
    Gv c_still = (S.G("clog").G("rvp")).AsF();
    S = S.S("rvp", S.G("rvp").S("stir", Gv.B(true)));
    foreach (Gv i_8 in (Gv.I(120L)).Iter()) {
      sim.C("net_step", Gv.F(1.0));
    }
    chk(Gv.S("bez míchání se zrno za ~5 min usadí, míchadlo ho za ~1 min rozvíří"), Gv.B((Gv.Gt(c_still, Gv.F(0.9)) && Gv.Lt(S.G("clog").G("rvp"), Gv.F(0.1)))), (Gv.S("%.2f -> %.2f") % Gd.Arr(c_still, S.G("clog").G("rvp"))));
    fresh();
    S = sim.G("S");
    Gv open_route = Gd.Fn((_a) => { Gv a = Gd.A(_a, 0); Gv b_2 = Gd.A(_a, 1); Gv pumps = Gd.A(_a, 2); Gv vv0 = sim.C("route_valves", a, b_2, pumps); Gv ids = Gd.Arr(); foreach (Gv id_4 in (vv0).Iter()) {   ids.C("append", id_4);   if ((m.G("outlet_open").C("has", id_4)).T()) {     m = m.S("outlet_open", m.G("outlet_open").SI(id_4, Gv.B(true)));   } else if ((sim.G("net").G("byId").C("has", id_4)).T()) {     sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(id_4, sim.G("net").G("byId").I(id_4).SI(Gv.S("open"), Gv.B(true)))));   } } sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L))); return ids; return Gv.Null; });
    Gv rids = open_route.C("call", Gv.S("rvp"), Gv.S("sk"), Gd.Arr(Gv.S("p1")));
    S = S.S("rvp", S.G("rvp").S("w", Gv.F(2000.0)));
    S = S.S("rvp", S.G("rvp").S("T", Gv.F(63.0)));
    S = S.S("rvp", S.G("rvp").S("Tsens", Gv.F(63.0)));
    S = S.S("rvp", S.G("rvp").S("stir", Gv.B(true)));
    S = S.S("p1", S.G("p1").S("prime", Gv.F(0.0)));
    S = S.S("p1", S.G("p1").S("flow", Gv.F(250.0)));
    S = S.S("p1", S.G("p1").S("on", Gv.B(false)));
    foreach (Gv i_9 in (Gv.I(3L)).Iter()) {
      sim.C("net_step", Gv.F(1.0));
    }
    Gv pr_a = (S.G("p1").G("prime")).AsF();
    foreach (Gv i_10 in (Gv.I(10L)).Iter()) {
      sim.C("net_step", Gv.F(1.0));
    }
    chk(Gv.S("stojící čerpadlo se při otevřené klapce před ním zavodní samo (~6 s)"), Gv.B((Gv.Lt(pr_a, Gv.F(0.95)) && Gv.Ge(S.G("p1").G("prime"), Gv.F(0.95)))), (Gv.S("po 3 s %.2f, po 13 s %.2f") % Gd.Arr(pr_a, S.G("p1").G("prime"))));
    S = S.S("p1", S.G("p1").S("prime", Gv.F(0.0)));
    S = S.S("p1", S.G("p1").S("on", Gv.B(true)));
    sim.C("net_step", Gv.F(1.0));
    chk(Gv.S("čerpadlo spuštěné nezavodněné nedává plný průtok (zavodňuje se)"), Gv.B((Gv.Lt(S.G("p1").G("q"), Gv.F(60.0)) && (S.G("p1").G("status").C("begins_with", Gv.S("zavod"))).T())), (Gv.S("%.0f L/min '%s'") % Gd.Arr(S.G("p1").G("q"), S.G("p1").G("status"))));
    foreach (Gv i_11 in (Gv.I(20L)).Iter()) {
      sim.C("net_step", Gv.F(1.0));
    }
    chk(Gv.S("po zavodnění teče plným průtokem"), Gv.B(((Gv.Ge(S.G("p1").G("prime"), Gv.F(0.95)) && Gv.Gt(S.G("p1").G("q"), Gv.F(100.0))) && Gv.Eq(S.G("p1").G("dry"), Gv.F(0.0)))), (Gv.S("%.0f L/min") % S.G("p1").G("q")));
    foreach (Gv it__it in (m.G("items")).Iter()) {
      Gv it = it__it;
      if ((Gv.Eq(it.G("type"), Gv.S("valve")) && (rids.C("has", it.G("id"))).T())) {
        it = it.SI(Gv.S("open"), Gv.B(false));
      }
    }
    sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
    foreach (Gv i_12 in (Gv.I(20L)).Iter()) {
      sim.C("net_step", Gv.F(1.0));
    }
    chk(Gv.S("zavřené klapky za chodu: čerpadlo nasucho ztrácí zavodnění"), Gv.B(Gv.Lt(S.G("p1").G("prime"), Gv.F(0.5))), (Gv.S("%.2f") % S.G("p1").G("prime")));
    foreach (Gv i_13 in (Gv.I(60L)).Iter()) {
      sim.C("net_step", Gv.F(1.0));
    }
    chk(Gv.S("ochrana chodu nasucho čerpadlo vypne"), Gv.B(!(S.G("p1").G("on")).T()));
    chk(Gv.S("zavřená klapka za chodu čerpadla se zapíše do deníku jako chyba postupu"), S.G("log").C("any", Gd.Fn((_a) => { Gv l = Gd.A(_a, 0); return Gv.B((Gv.Eq(l.G("type"), Gv.S("bad")) && ((l.G("msg").C("contains", Gv.S("klapka před čerpadlem"))).T() || (l.G("msg").C("contains", Gv.S("klapka za čerpadlem"))).T()))); return Gv.Null; })));
    fresh();
    S = sim.G("S");
    open_route.C("call", Gv.S("rvp"), Gv.S("sk"), Gd.Arr(Gv.S("p1")));
    S = S.S("rvp", S.G("rvp").S("w", Gv.F(2000.0)));
    S = S.S("rvp", S.G("rvp").S("g", Gv.F(150.0)));
    S = S.S("rvp", S.G("rvp").S("su", Gv.F(250.0)));
    S = S.S("rvp", S.G("rvp").S("sg", Gv.F(30.0)));
    S = S.S("rvp", S.G("rvp").S("T", Gv.F(63.0)));
    S = S.S("rvp", S.G("rvp").S("Tsens", Gv.F(63.0)));
    S = S.S("rvp", S.G("rvp").S("stir", Gv.B(true)));
    S = S.S("clog", S.G("clog").S("rvp", Gv.F(1.0)));
    S = S.S("p1", S.G("p1").S("prime", Gv.F(1.0)));
    S = S.S("p1", S.G("p1").S("flow", Gv.F(250.0)));
    S = S.S("p1", S.G("p1").S("on", Gv.B(true)));
    foreach (Gv i_14 in (Gv.I(2L)).Iter()) {
      sim.C("net_step", Gv.F(1.0));
    }
    Gv q_blocked = (S.G("p1").G("q")).AsF();
    foreach (Gv i_15 in (Gv.I(150L)).Iter()) {
      sim.C("net_step", Gv.F(1.0));
    }
    chk(Gv.S("ucpanou výpust čerpadlo nejdřív sotva táhne, po protlačení s mícháním teče"), Gv.B((((Gv.Lt(q_blocked, Gv.F(60.0)) && Gv.Lt(S.G("clog").G("rvp"), Gv.F(0.2))) && Gv.Gt(S.G("p1").G("q"), Gv.F(80.0))) && (S.G("p1").G("on")).T())), (Gv.S("%.0f -> %.0f L/min, clog %.2f") % Gd.Arr(q_blocked, S.G("p1").G("q"), S.G("clog").G("rvp"))));
    chk(Gv.S("škrtidlo: 60 % a víc skoro nebrzdí, pod 40 % průtok rychle klesá"), Gv.B(((((Gv.Eq(sim.C("throttle_flow", Gv.I(100L)), Gv.F(1.0)) && Gv.Gt(sim.C("throttle_flow", Gv.I(60L)), Gv.F(0.8))) && Gv.Lt(sim.C("throttle_flow", Gv.I(30L)), Gv.F(0.45))) && Gv.Lt(sim.C("throttle_flow", Gv.I(10L)), Gv.F(0.08))) && Gv.Eq(sim.C("throttle_flow", Gv.I(0L)), Gv.F(0.0)))), (Gv.S("%.2f %.2f %.2f %.2f") % Gd.Arr(sim.C("throttle_flow", Gv.I(60L)), sim.C("throttle_flow", Gv.I(40L)), sim.C("throttle_flow", Gv.I(30L)), sim.C("throttle_flow", Gv.I(10L)))));
    fresh();
    S = sim.G("S");
    Gv pid = Gv.S("");
    foreach (Gv it_2 in (m.G("items")).Iter()) {
      if ((Gv.Eq(it_2.G("type"), Gv.S("vessel")) && Gv.Eq(it_2.C("get", Gv.S("shape")), Gv.S("plate")))) {
        pid = it_2.G("id");
      }
    }
    Gv cool_at = Gd.Fn((_a) => { Gv wort = Gd.A(_a, 0); Gv ice = Gd.A(_a, 1); S = S.S("plates", Gd.Dict(pid, Gd.Dict(Gv.S("kind"), Gv.S("ice"), Gv.S("T"), Gv.F(2.0), Gv.S("once"), Gv.B(true), Gv.S("flow"), ice))); Gv t = (sim.C("plate_cool", pid, Gv.F(95.0), wort, (Gv.F(1.0) / Gv.F(60.0)))).AsF(); return Gd.Arr(t, S.G("plates").I(pid).G("Tcout")); return Gv.Null; });
    Gv fast = cool_at.C("call", Gv.F(140.0), Gv.F(60.0));
    Gv slow = cool_at.C("call", Gv.F(40.0), Gv.F(60.0));
    Gv more_ice = cool_at.C("call", Gv.F(140.0), Gv.F(200.0));
    chk(Gv.S("chladič: přiškrcená (pomalejší) mladina vychází studenější"), Gv.B(Gv.Lt(slow.I(Gv.I(0L)), (fast.I(Gv.I(0L)) - Gv.F(10.0)))), (Gv.S("140 L/min %.1f °C, 40 L/min %.1f °C") % Gd.Arr(fast.I(Gv.I(0L)), slow.I(Gv.I(0L)))));
    chk(Gv.S("chladič: víc ledové vody = studenější mladina"), Gv.B(Gv.Lt(more_ice.I(Gv.I(0L)), (fast.I(Gv.I(0L)) - Gv.F(5.0)))), (Gv.S("%.1f °C") % more_ice.I(Gv.I(0L))));
    Gv q_wort = (((((Gv.F(140.0) / Gv.F(60.0)) * Gv.F(1.04)) * Gv.F(4.0)) * ((Gv.F(95.0) - fast.I(Gv.I(0L)))))).AsF();
    Gv q_ice = ((((Gv.F(60.0) / Gv.F(60.0)) * Gv.F(4.19)) * ((fast.I(Gv.I(1L)) - Gv.F(2.0))))).AsF();
    chk(Gv.S("chladič: teplo odebrané mladině = teplo přijaté ledovou vodou"), near(q_wort, q_ice, Gv.F(0.5)), (Gv.S("%.1f / %.1f kW, voda ven %.1f °C") % Gd.Arr(q_wort, q_ice, fast.I(Gv.I(1L)))));
    Gv raw = sim.C("lp");
    raw = raw.S("w", Gv.F(2000.0));
    raw = raw.S("sg", Gv.F(120.0));
    raw = raw.S("dx", Gv.F(20.0));
    raw = raw.S("f", Gv.F(100.0));
    Gv rawc = sim.C("iodine_info", raw);
    Gv conv = sim.C("lp");
    conv = conv.S("w", Gv.F(2000.0));
    conv = conv.S("f", Gv.F(400.0));
    conv = conv.S("ld", Gv.F(60.0));
    Gv convc = sim.C("iodine_info", conv);
    Gv part_2 = sim.C("lp");
    part_2 = part_2.S("w", Gv.F(2000.0));
    part_2 = part_2.S("dx", Gv.F(60.0));
    part_2 = part_2.S("f", Gv.F(400.0));
    part_2 = part_2.S("ld", Gv.F(60.0));
    part_2 = part_2.S("sg", Gv.F(2.0));
    Gv partc = sim.C("iodine_info", part_2);
    chk(Gv.S("jod: nezcukřený rmut je modročerný (tmavý, modrá převažuje)"), Gv.B((Gv.Gt(rawc.G("color").G("b"), rawc.G("color").G("r")) && Gv.Lt(rawc.G("color").G("v"), Gv.F(0.35)))), rawc.G("name"));
    chk(Gv.S("jod: zcukřený rmut je žlutohnědý (barva jódu)"), Gv.B(((convc.G("name").C("begins_with", Gv.S("žlutohnědá"))).T() && Gv.Gt(convc.G("color").G("r"), convc.G("color").G("b")))), convc.G("name"));
    chk(Gv.S("jod: částečně zcukřený je červenohnědý/oranžový (dextriny)"), Gv.B(((Gv.Gt(partc.G("color").G("r"), partc.G("color").G("b")) && Gv.Gt(partc.G("color").G("r"), Gv.F(0.55))) && Gv.Ne(partc.G("name"), convc.G("name")))), partc.G("name"));
    Gv gr = sim.C("lp");
    gr = gr.S("w", Gv.F(2000.0));
    gr = gr.S("f", Gv.F(400.0));
    gr = gr.S("su", Gv.F(30.0));
    chk(Gv.S("jod: zrna s nezcukřeným škrobem ukážou škrob, přecezená kapalina ne"), Gv.B((Gv.Gt(sim.C("iodine_info", gr, Gv.B(true)).G("starch"), Gv.F(0.02)) && Gv.Lt(sim.C("iodine_info", gr, Gv.B(false)).G("starch"), Gv.F(1e-09)))));
    fresh();
    S = sim.G("S");
    Gv gg = Gv.Null;
    foreach (Gv it_3 in (m.G("items")).Iter()) {
      if ((Gv.Eq(it_3.G("type"), Gv.S("gauge")) && (m.C("gauge_geo", it_3).G("free")).T())) {
        gg = it_3;
      }
    }
    chk(Gv.S("plán má volný stavoznak"), Gv.B(Gv.Ne(gg, Gv.Null)));
    if (Gv.Ne(gg, Gv.Null)) {
      Gv ends = Gv.I(0L);
      Gv leaking = Gv.I(0L);
      foreach (Gv n in (sim.G("net").G("nodes").C("values")).Iter()) {
        if (Gv.Eq(n.C("get", Gv.S("gaugeId"), Gv.S("")), gg.G("id"))) {
          ends = (ends + Gv.I(1L));
          if ((n.G("openEnd")).T()) {
            leaking = (leaking + Gv.I(1L));
          }
        }
      }
      chk(Gv.S("konce trubek u stavoznaku jsou rozpoznané a nejsou únik"), Gv.B((Gv.Ge(ends, Gv.I(1L)) && Gv.Eq(leaking, Gv.I(0L)))), (Gv.S("konců %d, úniků %d") % Gd.Arr(ends, leaking)));
      Gv gv0 = sim.C("gauge_vessel_free", gg);
      chk(Gv.S("stavoznak je trubkou napojen přímo na ledovku (horní hrdlo nemá klapku)"), Gv.B((Gv.Ne(gv0, Gv.Null) && Gv.Eq(gv0.C("get", Gv.S("bind")), Gv.S("lv")))));
      foreach (Gv it_4__it in (m.G("items")).Iter()) {
        Gv it_4 = it_4__it;
        if (Gv.Eq(it_4.G("type"), Gv.S("valve"))) {
          it_4 = it_4.SI(Gv.S("open"), Gv.B(true));
        }
      }
      foreach (Gv k in (m.G("outlet_open")).Iter()) {
        m = m.S("outlet_open", m.G("outlet_open").SI(k, Gv.B(true)));
      }
      sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
      Gv gv = sim.C("gauge_vessel_free", gg);
      chk(Gv.S("s otevřenými klapkami ukazuje hladinu připojené nádoby"), Gv.B((Gv.Ne(gv, Gv.Null) && Gv.Eq(gv.C("get", Gv.S("bind")), Gv.S("lv")))), Gd.Dyn("str", (Gv.Ne(gv, Gv.Null) ? gv.C("get", Gv.S("bind")) : Gv.Null)));
      gg = gg.SI(Gv.S("top"), Gv.B(false));
      gg = gg.SI(Gv.S("bot"), Gv.B(false));
      sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
      chk(Gv.S("se zavřenými oběma kohouty stavoznak nic neukazuje"), Gv.B(Gv.Eq(sim.C("gauge_vessel_free", gg), Gv.Null)));
      gg = gg.SI(Gv.S("top"), Gv.B(true));
      gg = gg.SI(Gv.S("bot"), Gv.B(true));
      foreach (Gv it_5__it in (m.G("items")).Iter()) {
        Gv it_5 = it_5__it;
        if (Gv.Eq(it_5.G("type"), Gv.S("valve"))) {
          it_5 = it_5.SI(Gv.S("open"), Gv.B(false));
        }
      }
      foreach (Gv k_2 in (m.G("outlet_open")).Iter()) {
        m = m.S("outlet_open", m.G("outlet_open").SI(k_2, Gv.B(false)));
      }
    }
    Gv runs = Gd.Dict();
    foreach (Gv cfg in (Gd.Arr(Gd.Arr(Gv.S("stoji"), Gv.B(false), Gv.F(35.0)), Gd.Arr(Gv.S("pomalu"), Gv.B(true), Gv.F(10.0)), Gd.Arr(Gv.S("normalne"), Gv.B(true), Gv.F(35.0)), Gd.Arr(Gv.S("rychle"), Gv.B(true), Gv.F(50.0)))).Iter()) {
      fresh();
      S = sim.G("S");
      Gv rv_2 = S.G("rvp");
      rv_2 = rv_2.S("w", Gv.F(2000.0));
      rv_2 = rv_2.S("T", Gv.F(40.0));
      rv_2 = rv_2.S("Tsens", Gv.F(40.0));
      rv_2 = rv_2.S("g", Gv.F(80.0));
      rv_2 = rv_2.S("su", Gv.F(120.0));
      rv_2 = rv_2.S("sg", Gv.F(20.0));
      rv_2 = rv_2.S("f", Gv.F(150.0));
      rv_2 = rv_2.S("stir", cfg.I(Gv.I(1L)));
      rv_2 = rv_2.S("hz", cfg.I(Gv.I(2L)));
      rv_2 = rv_2.S("heat", Gv.F(0.5));
      foreach (Gv i_16 in (Gv.I(600L)).Iter()) {
        sim.C("sim_step", Gv.F(1.0));
      }
      runs = runs.SI(cfg.I(Gv.I(0L)), Gd.Arr(rv_2.G("T"), rv_2.G("Twall"), rv_2.G("Tsens")));
    }
    chk(Gv.S("míchání: rychlejší míchadlo ohřívá hustý rmut rychleji"), Gv.B(((Gv.Gt(runs.I(Gv.S("normalne")).I(Gv.I(0L)), runs.I(Gv.S("pomalu")).I(Gv.I(0L))) && Gv.Gt(runs.I(Gv.S("pomalu")).I(Gv.I(0L)), (runs.I(Gv.S("stoji")).I(Gv.I(0L)) - Gv.F(0.01)))) && Gv.Ge(runs.I(Gv.S("rychle")).I(Gv.I(0L)), runs.I(Gv.S("normalne")).I(Gv.I(0L))))), Gd.Dyn("str", runs));
    chk(Gv.S("míchání: bez míchadla je kapalina u pláště výrazně teplejší než střed nádoby"), Gv.B(Gv.Gt((runs.I(Gv.S("stoji")).I(Gv.I(1L)) - runs.I(Gv.S("stoji")).I(Gv.I(0L))), (Gv.F(1.5) * ((runs.I(Gv.S("normalne")).I(Gv.I(1L)) - runs.I(Gv.S("normalne")).I(Gv.I(0L))))))), (Gv.S("stoji +%.1f K, normalne +%.1f K") % Gd.Arr((runs.I(Gv.S("stoji")).I(Gv.I(1L)) - runs.I(Gv.S("stoji")).I(Gv.I(0L))), (runs.I(Gv.S("normalne")).I(Gv.I(1L)) - runs.I(Gv.S("normalne")).I(Gv.I(0L))))));
    fresh();
    S = sim.G("S");
    Gv hv = S.G("rvp");
    hv = hv.S("w", Gv.F(2000.0));
    hv = hv.S("T", Gv.F(50.0));
    hv = hv.S("Tsens", Gv.F(50.0));
    hv = hv.S("stir", Gv.B(true));
    hv = hv.S("hz", Gv.F(35.0));
    hv = hv.S("heat", Gv.F(1.0));
    foreach (Gv i_17 in (Gv.I(20L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
    }
    chk(Gv.S("teploměr má dobu odezvy (ukazuje méně než skutečná teplota při ohřevu)"), Gv.B((Gv.Lt(hv.G("Tsens"), (hv.G("T") - Gv.F(0.05))) && Gv.Gt(hv.G("Tsens"), Gv.F(50.0)))), (Gv.S("T %.2f, čidlo %.2f") % Gd.Arr(hv.G("T"), hv.G("Tsens"))));
    Gv thin = sim.C("vessel");
    thin = thin.S("w", Gv.F(2000.0));
    thin = thin.S("stir", Gv.B(true));
    thin = thin.S("hz", Gv.F(35.0));
    Gv thick_ = sim.C("vessel");
    thick_ = thick_.S("w", Gv.F(2000.0));
    thick_ = thick_.S("stir", Gv.B(true));
    thick_ = thick_.S("hz", Gv.F(35.0));
    thick_ = thick_.S("g", Gv.F(150.0));
    thick_ = thick_.S("su", Gv.F(300.0));
    chk(Gv.S("hustý rmut zhoršuje přestup tepla proti řídké kapalině"), Gv.B(Gv.Lt(sim.C("film_factor", thick_), (sim.C("film_factor", thin) * Gv.F(0.9)))));
    chk(Gv.S("referenční podmínky (35 Hz, řídká kapalina) = plný prostup"), Gv.B(Gv.Lt(Gd.absf((sim.C("film_factor", thin) - Gv.F(1.0))), Gv.F(0.02))), (Gv.S("%.3f") % sim.C("film_factor", thin)));
    Gv wtr = sim.C("vessel");
    wtr = wtr.S("w", Gv.F(1000.0));
    wtr = wtr.S("T", Gv.F(20.0));
    chk(Gv.S("hustota vody při 20 °C je 0,998 kg/l"), Gv.B(Gv.Lt(Gd.absf((sim.C("density", wtr) - Gv.F(0.998))), Gv.F(0.002))), (Gv.S("%.4f") % sim.C("density", wtr)));
    Gv wort_2 = sim.C("vessel");
    wort_2 = wort_2.S("w", Gv.F(880.0));
    wort_2 = wort_2.S("f", Gv.F(120.0));
    wort_2 = wort_2.S("T", Gv.F(20.0));
    chk(Gv.S("hustota 12 °P sladiny při 20 °C je asi 1,048 kg/l"), Gv.B(Gv.Lt(Gd.absf((sim.C("density", wort_2) - Gv.F(1.048))), Gv.F(0.004))), (Gv.S("%.4f") % sim.C("density", wort_2)));
    Gv hot_wort = sim.C("vessel");
    hot_wort = hot_wort.S("w", Gv.F(880.0));
    hot_wort = hot_wort.S("f", Gv.F(120.0));
    hot_wort = hot_wort.S("T", Gv.F(95.0));
    chk(Gv.S("horká sladina je řidší než studená (tepelná roztažnost)"), Gv.B(Gv.Lt(sim.C("density", hot_wort), (sim.C("density", wort_2) - Gv.F(0.02)))));
    Gv mash_2 = sim.C("vessel");
    mash_2 = mash_2.S("w", Gv.F(2000.0));
    mash_2 = mash_2.S("f", Gv.F(100.0));
    mash_2 = mash_2.S("g", Gv.F(130.0));
    mash_2 = mash_2.S("su", Gv.F(300.0));
    mash_2 = mash_2.S("T", Gv.F(20.0));
    chk(Gv.S("rmut s pevnými částicemi je hustší než sladina téhož extraktu"), Gv.B(Gv.Gt(sim.C("density", mash_2), Gv.F(1.04))));
    fresh();
    S = sim.G("S");
    m.G("auto").C("start");
    foreach (Gv i_18 in (Gv.I(1500L)).Iter()) {
      m.G("auto").C("tick", Gv.F(1.0));
      sim.C("sim_step", Gv.F(1.0));
    }
    Gv snap_t = (S.G("t")).AsF();
    Gv snap_i = (m.G("auto").G("i")).AsI();
    Gv snap_v = (sim.C("vol", S.G("rvp"))).AsF();
    m.C("save_game");
    sim.C("new_state");
    sim.C("build_net", m.G("items"), m.G("plan").C("get", Gv.S("joints"), Gd.Arr()));
    m = m.S("auto", m.G("auto").S("S", sim.G("S")));
    m = m.S("auto", m.G("auto").S("on", Gv.B(false)));
    m = m.S("auto", m.G("auto").S("i", Gv.I(-1L)));
    Gv ok = m.C("load_game");
    S = sim.G("S");
    chk(Gv.S("uložení: várka se načte"), ok);
    chk(Gv.S("uložení: čas, objem v pánvi a krok automatu jsou stejné"), Gv.B((((near(S.G("t"), snap_t, Gv.F(1e-06))).T() && (near(sim.C("vol", S.G("rvp")), snap_v, Gv.F(1e-06))).T()) && Gv.Eq(m.G("auto").G("i"), snap_i))), (Gv.S("t=%.0f i=%d") % Gd.Arr(S.G("t"), m.G("auto").G("i"))));
    chk(Gv.S("uložení: aktivní automat zůstane aktivní (obnova není ruční zásah)"), m.G("auto").G("on"));
    Gd.Static("DirAccess", "remove_absolute", Gd.Static("ProjectSettings", "globalize_path", m.G("save_path")));
    chk(Gv.S("testy používají vlastní uložený soubor, ne várku uživatele"), Gv.B(Gv.Ne(m.G("save_path"), m.G("SAVE_PATH"))));
    fresh();
    S = sim.G("S");
    Gv plate_id = Gv.S("");
    foreach (Gv it_6 in (m.G("items")).Iter()) {
      if ((Gv.Eq(it_6.G("type"), Gv.S("vessel")) && Gv.Eq(it_6.C("get", Gv.S("shape")), Gv.S("plate")))) {
        plate_id = it_6.G("id");
      }
    }
    foreach (Gv rt_2 in (Gd.Arr(Gd.Arr(Gv.S("hlt"), Gv.S("rvp"), Gd.Arr(Gv.S("phw")), Gv.I(-1L), Gv.S("")), Gd.Arr(Gv.S("hlt"), Gv.S("sk"), Gd.Arr(Gv.S("phw")), Gv.I(1L), Gv.S("")), Gd.Arr(Gv.S("hlt"), Gv.S("sk"), Gd.Arr(Gv.S("phw")), Gv.I(0L), Gv.S("")), Gd.Arr(Gv.S("rvp"), Gv.S("sk"), Gd.Arr(Gv.S("p1")), Gv.I(-1L), Gv.S("")), Gd.Arr(Gv.S("sk"), Gv.S("rvp"), Gd.Arr(Gv.S("p1")), Gv.I(-1L), Gv.S("")), Gd.Arr(Gv.S("rvp"), Gv.S("wp"), Gd.Arr(Gv.S("p1")), Gv.I(-1L), Gv.S("")), Gd.Arr(Gv.S("sk"), Gv.S("sk"), Gd.Arr(Gv.S("p2")), Gv.I(0L), Gv.S("")), Gd.Arr(Gv.S("sk"), Gv.S("rvp"), Gd.Arr(Gv.S("p2")), Gv.I(-1L), Gv.S("")), Gd.Arr(Gv.S("wp"), Gv.S("spilka"), Gd.Arr(Gv.S("p3")), Gv.I(-1L), Gv.S("")), Gd.Arr(Gv.S("lv"), Gv.S("lv"), Gd.Arr(Gv.S("plv")), Gv.I(-1L), Gv.S("")), Gd.Arr(Gv.S("lv"), Gv.S("hlt"), Gd.Arr(Gv.S("plv")), Gv.I(-1L), plate_id), Gd.Arr(Gv.S("vodovod"), Gv.S("lv"), Gd.Arr(), Gv.I(-1L), Gv.S("")))).Iter()) {
      Gv rv_ = sim.C("route_valves", rt_2.I(Gv.I(0L)), rt_2.I(Gv.I(1L)), rt_2.I(Gv.I(2L)), rt_2.I(Gv.I(3L)), rt_2.I(Gv.I(4L)));
      chk((Gv.S("cesta %s → %s (%s, kde %d%s) je průchozí") % Gd.Arr(rt_2.I(Gv.I(0L)), rt_2.I(Gv.I(1L)), Gv.S(",").C("join", Gd.Ctor("PackedStringArray", rt_2.I(Gv.I(2L)))), rt_2.I(Gv.I(3L)), (Gv.Ne(rt_2.I(Gv.I(4L)), Gv.S("")) ? Gv.S(", deska") : Gv.S("")))), Gv.B(Gv.Ne(rv_, Gv.Null)));
    }
    Gv vn_ = Gd.Dict();
    foreach (Gv nk in (sim.G("net").G("nodes")).Iter()) {
      Gv nn = sim.G("net").G("nodes").I(nk);
      if (Gv.Ne(nn.G("valve"), Gv.Null)) {
        vn_ = vn_.SI(nn.G("valve"), nk);
      }
    }
    Gv pair_ok = Gv.B(false);
    if (((vn_.C("has", Gv.S("valve_mur9syhl"))).T() && (vn_.C("has", Gv.S("valve_mur9svxz"))).T())) {
      pair_ok = Gv.B(Gv.Ne(sim.C("_route", Gd.Arr(vn_.I(Gv.S("valve_mur9syhl"))), Gd.Arr(vn_.I(Gv.S("valve_mur9svxz"))), Gv.S(""), Gv.I(-1L)), Gv.Null));
    }
    chk(Gv.S("cesta mezi klapkami u průhledítka (760,850 ↔ 820,850) je průchozí"), pair_ok);
    Gv skew = Gv.I(0L);
    foreach (Gv it_7 in (m.G("items")).Iter()) {
      if ((Gv.Eq(it_7.G("type"), Gv.S("pipe")) && !(it_7.C("get", Gv.S("hose"), Gv.B(false))).T())) {
        foreach (Gv i_19 in ((it_7.G("pts").C("size") - Gv.I(1L))).Iter()) {
          if ((Gv.Gt(Gd.absf((it_7.G("pts").I(i_19).I(Gv.I(0L)) - it_7.G("pts").I((i_19 + Gv.I(1L))).I(Gv.I(0L)))), Gv.F(0.01)) && Gv.Gt(Gd.absf((it_7.G("pts").I(i_19).I(Gv.I(1L)) - it_7.G("pts").I((i_19 + Gv.I(1L))).I(Gv.I(1L)))), Gv.F(0.01)))) {
            skew = (skew + Gv.I(1L));
          }
        }
      }
    }
    chk(Gv.S("všechny trubky (mimo hadice) jsou vodorovné nebo svislé"), Gv.B(Gv.Eq(skew, Gv.I(0L))), (Gv.S("šikmých úseků %d") % skew));
    fresh();
    S = sim.G("S");
    Gv free_o = Gv.Null;
    foreach (Gv o_2 in (m.G("outlets")).Iter()) {
      if (Gv.Eq(m.C("hose_at", o_2.G("front")), Gv.Null)) {
        free_o = o_2;
        break;
      }
    }
    chk(Gv.S("kateta má volný vývod pro hadici"), Gv.B(Gv.Ne(free_o, Gv.Null)));
    if (Gv.Ne(free_o, Gv.Null)) {
      Gv n_items = (m.G("items").C("size")).AsI();
      Gv added = m.C("_shift_hose", free_o.G("front"));
      chk(Gv.S("Shift + klepnutí na volný vývod přidá hadici"), Gv.B(((((added).T() && Gv.Eq(m.G("items").C("size"), (n_items + Gv.I(1L)))) && Gv.Ne(m.C("hose_at", free_o.G("front")), Gv.Null)) && (m.G("outlet_open").I(free_o.G("key"))).T())));
      Gv hh = m.C("hose_at", free_o.G("front"));
      chk(Gv.S("přidaná hadice je uživatelská a má volný konec"), Gv.B(((Gv.Ne(hh, Gv.Null) && (hh.I(Gv.I(0L)).C("get", Gv.S("user"), Gv.B(false))).T()) && Gv.Ge(hh.I(Gv.I(0L)).G("pts").C("size"), Gv.I(2L)))));
      Gv removed = m.C("_shift_hose", free_o.G("front"));
      chk(Gv.S("Shift + klepnutí na přidanou hadici ji odebere"), Gv.B((((removed).T() && Gv.Eq(m.G("items").C("size"), n_items)) && Gv.Eq(m.C("hose_at", free_o.G("front")), Gv.Null))));
    }
    fresh();
    S = sim.G("S");
    Gv route_ids = open_route.C("call", Gv.S("hlt"), Gv.S("rvp"), Gd.Arr(Gv.S("phw")));
    S = S.S("mixAuto", Gv.B(false));
    S = S.S("rvpWater", S.G("rvpWater").S("target", Gv.F(5000.0)));
    S = S.S("rvpWater", S.G("rvpWater").S("on", Gv.B(true)));
    foreach (Gv i_20 in (Gv.I(240L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
    }
    Gv wet = Gv.I(0L);
    foreach (Gv ed_4 in (sim.G("net").G("edges")).Iter()) {
      if (Gv.Gt(ed_4.G("c").G("w"), Gv.F(0.01))) {
        wet = (wet + Gv.I(1L));
      }
    }
    chk(Gv.S("voda z bojleru vyplní i větve mimo cestu k pánvi (ne jen jedinou trasu)"), Gv.B(Gv.Gt(wet, ((route_ids.C("size") * Gv.I(2L)) + Gv.I(3L)))), (Gv.S("mokrých úseků %d, klapek na trase %d") % Gd.Arr(wet, route_ids.C("size"))));
    foreach (Gv gt in (Gd.Arr(Gd.Arr(Gv.S("sk"), Gv.S("wp")), Gd.Arr(Gv.S("rvp"), Gv.S("wp")), Gd.Arr(Gv.S("rvp"), Gv.S("sk")))).Iter()) {
      fresh();
      S = sim.G("S");
      Gv gids = open_route.C("call", gt.I(Gv.I(0L)), gt.I(Gv.I(1L)), Gd.Arr());
      Gv srcv = (Gv.Eq(gt.I(Gv.I(0L)), Gv.S("sk")) ? S.G("sk").G("mash") : S.I(gt.I(Gv.I(0L))));
      srcv = srcv.S("w", Gv.F(2500.0));
      srcv = srcv.S("T", Gv.F(20.0));
      Gv v0 = (sim.C("vol", srcv)).AsF();
      foreach (Gv i_21 in (Gv.I(600L)).Iter()) {
        sim.C("sim_step", Gv.F(1.0));
      }
      Gv moved = ((v0 - sim.C("vol", srcv))).AsF();
      Gv dst_v = ((Gv.Eq(gt.I(Gv.I(1L)), Gv.S("wp")) ? sim.C("vol", S.G("wp")) : ((Gv.Eq(gt.I(Gv.I(1L)), Gv.S("rvp")) ? sim.C("vol", S.G("rvp")) : sim.C("skVolTotal"))))).AsF();
      Gv piped = Gv.F(0.0);
      foreach (Gv ed_5 in (sim.G("net").G("edges")).Iter()) {
        piped = (piped + sim.C("vol", ed_5.G("c")));
      }
      if (Gv.Eq(gt.I(Gv.I(1L)), Gv.S("sk"))) {
        chk(Gv.S("samospád: do SK se shora (nad hladinou) nepřetéká, voda nejde do kopce"), Gv.B(Gv.Lt(moved, Gv.F(1.0))), (Gv.S("%.1f L") % moved));
      } else {
        chk((Gv.S("samospád %s → %s: z vyšší hladiny do nižší, nic se neztratí") % Gd.Arr(gt.I(Gv.I(0L)), gt.I(Gv.I(1L)))), Gv.B((Gv.Gt(moved, Gv.F(500.0)) && Gv.Lt(Gd.absf(((moved - dst_v) - piped)), Gv.F(5.0)))), (Gv.S("přeteklo %.0f L, cíl %.0f, v potrubí %.0f") % Gd.Arr(moved, dst_v, piped)));
      }
    }
    foreach (Gv wh in (Gd.Arr(Gv.I(1L), Gv.I(0L))).Iter()) {
      fresh();
      S = sim.G("S");
      Gv wids = open_route.C("call", Gv.S("hlt"), Gv.S("sk"), Gd.Arr(Gv.S("phw")));
      Gv vsw = sim.C("route_valves", Gv.S("hlt"), Gv.S("sk"), Gd.Arr(Gv.S("phw")), wh);
      foreach (Gv idw in (vsw).Iter()) {
        if ((m.G("outlet_open").C("has", idw)).T()) {
          m = m.S("outlet_open", m.G("outlet_open").SI(idw, Gv.B(true)));
        } else if ((sim.G("net").G("byId").C("has", idw)).T()) {
          sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(idw, sim.G("net").G("byId").I(idw).SI(Gv.S("open"), Gv.B(true)))));
        }
      }
      sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
      S = S.S("mixAuto", Gv.B(false));
      S = S.S("skWater", S.G("skWater").S("flow", Gv.F(60.0)));
      S = S.S("skWater", S.G("skWater").S("on", Gv.B(true)));
      foreach (Gv i_22 in (Gv.I(240L)).Iter()) {
        sim.C("sim_step", Gv.F(1.0));
      }
      Gv got = ((Gv.Eq(wh, Gv.I(1L)) ? S.G("sk").G("under") : S.G("skWater").G("total"))).AsF();
      chk((Gv.S("%s: voda z bojleru teče do SK (ručně otevřená cesta)") % ((Gv.Eq(wh, Gv.I(1L)) ? Gv.S("podrážení") : Gv.S("vyslazování")))), Gv.B(Gv.Gt(got, Gv.F(30.0))), (Gv.S("do SK přiteklo %.0f L, bojler %.0f L") % Gd.Arr(got, S.G("hlt").G("w"))));
    }
    fresh();
    S = sim.G("S");
    Gv cab_item = Gv.Null;
    foreach (Gv itc in (m.G("items")).Iter()) {
      if (Gv.Eq(itc.G("type"), Gv.S("panel"))) {
        cab_item = itc;
      }
    }
    Gv cb = Gd.load(Gv.S("res://cabinet.gd")).C("new");
    cb = cb.S("m", m);
    cb.C("setup", cab_item);
    Gv dpos = Gd.Fn((_a) => { Gv dx = Gd.A(_a, 0); Gv dy = Gd.A(_a, 1); return (Gd.Ctor("Vector2", cab_item.G("x"), cab_item.G("y")) + (Gd.Ctor("Vector2", dx, dy) * cb.G("sc"))); return Gv.Null; });
    chk(Gv.S("rozvaděč: hlavní vypínač a hřib se najdou klepnutím"), Gv.B((Gv.Eq(cb.C("hit", dpos.C("call", Gv.F(274.0), Gv.F(960.0))), Gv.S("main")) && Gv.Eq(cb.C("hit", dpos.C("call", Gv.F(565.0), Gv.F(972.0))), Gv.S("estop")))));
    chk(Gv.S("rozvaděč: volič čerpadla C1 a potenciometr se najdou klepnutím"), Gv.B(((Gv.Eq(cb.C("hit", dpos.C("call", Gv.F(616.0), Gv.F(812.0))), Gv.S("pvarna")) || Gv.Eq(cb.C("hit", dpos.C("call", Gv.F(441.0), Gv.F(812.0))), Gv.S("potp_d"))) || Gv.Eq(cb.C("hit", dpos.C("call", Gv.F(450.0), Gv.F(812.0))), Gv.S("potp_u")))));
    Gv f0_2 = (S.G("p1").G("flow")).AsF();
    cb.C("act", Gv.S("potp_u"), Gv.B(true));
    chk(Gv.S("rozvaděč: potenciometr měniče zvýší průtok čerpadla C1 o krok"), Gv.B(Gv.Gt(S.G("p1").G("flow"), f0_2)), (Gv.S("%.0f → %.0f L/min") % Gd.Arr(f0_2, S.G("p1").G("flow"))));
    Gv hz0 = (S.G("rvp").G("hz")).AsF();
    cb.C("act", Gv.S("potmp_d"), Gv.B(true));
    chk(Gv.S("rozvaděč: potenciometr míchadla sníží otáčky (Hz)"), Gv.B(Gv.Lt(S.G("rvp").G("hz"), hz0)), (Gv.S("%.1f → %.1f Hz") % Gd.Arr(hz0, S.G("rvp").G("hz"))));
    sim = sim.S("deposit_on", Gv.B(false));
    fresh();
    S = sim.G("S");
    S = S.S("sk", S.G("sk").S("cells", S.G("sk").G("cells").SI(Gv.I(0L), S.G("sk").G("cells").I(Gv.I(0L)).S("w", Gv.F(800.0)))));
    S = S.S("sk", S.G("sk").S("cells", S.G("sk").G("cells").SI(Gv.I(4L), S.G("sk").G("cells").I(Gv.I(4L)).S("w", Gv.F(150.0)))));
    S = S.S("sk", S.G("sk").S("g", Gv.F(60.0)));
    S = S.S("sk", S.G("sk").S("mode", Gv.S("lauter")));
    sim.C("skQmax");
    S = S.S("sk", S.G("sk").SI(Gv.S("qSlow"), Gv.F(15.0)));
    Gv ch0 = (S.G("sk").G("ch")).AsF();
    sim.C("skDraw", Gd.min(Gv.F(70.0), S.G("sk").G("Qmax")), (Gv.F(1.0) / Gv.F(60.0)));
    chk(Gv.S("trhnutí: skok průtoku z 15 na vysoký utrhne lože (kanálky)"), Gv.B((Gv.Gt(S.G("sk").G("ch"), ch0) || Gv.Lt(S.G("sk").G("Qmax"), Gv.F(30.0)))), (Gv.S("ch %.2f → %.2f, Qmax %.0f") % Gd.Arr(ch0, S.G("sk").G("ch"), S.G("sk").G("Qmax"))));
    fresh();
    S = sim.G("S");
    S = S.S("sk", S.G("sk").S("cells", S.G("sk").G("cells").SI(Gv.I(0L), S.G("sk").G("cells").I(Gv.I(0L)).S("w", Gv.F(800.0)))));
    S = S.S("sk", S.G("sk").S("cells", S.G("sk").G("cells").SI(Gv.I(4L), S.G("sk").G("cells").I(Gv.I(4L)).S("w", Gv.F(150.0)))));
    S = S.S("sk", S.G("sk").S("g", Gv.F(60.0)));
    S = S.S("sk", S.G("sk").S("mode", Gv.S("lauter")));
    sim.C("skQmax");
    ch0 = (S.G("sk").G("ch")).AsF();
    sim.C("skDraw", Gv.F(25.0), (Gv.F(1.0) / Gv.F(60.0)));
    chk(Gv.S("trhnutí: rozjezd čerpadla z nuly lože netrhá"), Gv.B(Gv.Le(S.G("sk").G("ch"), (ch0 + Gv.F(0.001)))));
    sim = sim.S("deposit_on", Gv.B(true));
    fresh();
    S = sim.G("S");
    S = S.S("sk", S.G("sk").S("cells", S.G("sk").G("cells").SI(Gv.I(0L), S.G("sk").G("cells").I(Gv.I(0L)).S("w", Gv.F(800.0)))));
    S = S.S("sk", S.G("sk").S("cells", S.G("sk").G("cells").SI(Gv.I(4L), S.G("sk").G("cells").I(Gv.I(4L)).S("w", Gv.F(150.0)))));
    S = S.S("sk", S.G("sk").S("g", Gv.F(60.0)));
    S = S.S("sk", S.G("sk").S("mode", Gv.S("lauter")));
    S = S.S("sk", S.G("sk").SI(Gv.S("dep"), Gv.F(1.0)));
    sim.C("skQmax");
    Gv qm_dep = (S.G("sk").G("Qmax")).AsF();
    S = S.S("sk", S.G("sk").SI(Gv.S("qSlow"), Gv.F(8.0)));
    foreach (Gv i_23 in (Gv.I(6L)).Iter()) {
      S = S.S("sk", S.G("sk").SI(Gv.S("tearT"), Gv.F(0.0)));
      S = S.S("sk", S.G("sk").SI(Gv.S("qSlow"), Gv.F(5.0)));
      sim.C("skDraw", Gd.maxf(Gv.F(45.0), (S.G("sk").G("Qmax") * Gv.F(1.5))), (Gv.F(1.0) / Gv.F(60.0)));
      sim.C("skQmax");
    }
    chk(Gv.S("nános: několik trhnutí odstraní nános a zvedne Qmax"), Gv.B((Gv.Lt(S.G("sk").C("get", Gv.S("dep"), Gv.F(1.0)), Gv.F(0.4)) && Gv.Gt(S.G("sk").G("Qmax"), (qm_dep * Gv.F(1.2))))), (Gv.S("Qmax %.0f → %.0f, nános %.2f") % Gd.Arr(qm_dep, S.G("sk").G("Qmax"), S.G("sk").C("get", Gv.S("dep"), Gv.F(1.0)))));
    sim = sim.S("deposit_on", Gv.B(false));
    fresh();
    S = sim.G("S");
    S = S.S("sp", S.G("sp").SI(Gv.S("lidOpen"), Gv.B(false)));
    S = S.S("sp", S.G("sp").S("L", Gv.F(100.0)));
    sim.C("sim_step", Gv.F(1.0));
    S = S.S("sp", S.G("sp").S("L", Gv.F(400.0)));
    sim.C("sim_step", Gv.F(1.0));
    chk(Gv.S("víčko spilky: zavřené při plnění vyrobí přetlak"), Gv.B(Gv.Gt(S.G("sp").C("get", Gv.S("press"), Gv.F(0.0)), Gv.F(40.0))), (Gv.S("%.0f mbar") % S.G("sp").C("get", Gv.S("press"), Gv.F(0.0))));
    S = S.S("sp", S.G("sp").SI(Gv.S("lidOpen"), Gv.B(true)));
    sim.C("sim_step", Gv.F(1.0));
    chk(Gv.S("víčko spilky: otevřené = žádný přetlak"), Gv.B(Gv.Eq(S.G("sp").C("get", Gv.S("press"), Gv.F(1.0)), Gv.F(0.0))));
    S = S.S("cab", S.G("cab").S("key", Gv.B(true)));
    S = S.S("cab", S.G("cab").S("main", Gv.B(true)));
    S = S.S("cab", S.G("cab").S("estop", Gv.B(false)));
    S = S.S("rvp", S.G("rvp").S("w", Gv.F(3000.0)));
    S = S.S("rvp", S.G("rvp").S("g", Gv.F(700.0)));
    S = S.S("rvp", S.G("rvp").S("hz", Gv.F(50.0)));
    S = S.S("rvp", S.G("rvp").S("stir", Gv.B(true)));
    cb.C("act", Gv.S("stirmp"), Gv.B(true));
    chk(Gv.S("ráz: vypnutí míchadla MP na 50 Hz v rmutu ho poškodí"), Gv.B(((S.G("rvp").C("get", Gv.S("broken"), Gv.B(false))).T() && !(S.G("rvp").G("stir")).T())));
    cb.C("act", Gv.S("rstA"), Gv.B(true));
    chk(Gv.S("ráz: reset poruchy míchadlo opraví"), Gv.B(!(S.G("rvp").C("get", Gv.S("broken"), Gv.B(false))).T()));
    S = S.S("rvp", S.G("rvp").S("hz", Gv.F(10.0)));
    S = S.S("rvp", S.G("rvp").S("stir", Gv.B(true)));
    cb.C("act", Gv.S("stirmp"), Gv.B(true));
    chk(Gv.S("ráz: při nízkých otáčkách se vypne bez škody"), Gv.B((!(S.G("rvp").C("get", Gv.S("broken"), Gv.B(false))).T() && !(S.G("rvp").G("stir")).T())));
    S = S.S("cab", S.G("cab").S("key", Gv.B(true)));
    S = S.S("cab", S.G("cab").S("main", Gv.B(true)));
    S = S.S("cab", S.G("cab").S("estop", Gv.B(false)));
    cb.C("act", Gv.S("hyd"), Gv.B(true));
    S = S.S("sk", S.G("sk").S("rakeH", Gv.F(0.5)));
    cb.C("act", Gv.S("hup"), Gv.B(true));
    foreach (Gv i_24 in (Gv.I(3L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
    }
    cb.C("act", Gv.S("hup"), Gv.B(false));
    chk(Gv.S("rozvaděč: HYDRAULIKA SK + NAHORU zvedá kopačku (0 = nahoře)"), Gv.B(Gv.Lt(S.G("sk").G("rakeH"), Gv.F(0.5))), (Gv.S("výška %.2f") % S.G("sk").G("rakeH")));
    Gv rh1 = (S.G("sk").G("rakeH")).AsF();
    cb.C("act", Gv.S("hdn"), Gv.B(true));
    foreach (Gv i_25 in (Gv.I(3L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
    }
    cb.C("act", Gv.S("hdn"), Gv.B(false));
    chk(Gv.S("rozvaděč: DOLŮ spouští kopačku do lože"), Gv.B(Gv.Gt(S.G("sk").G("rakeH"), rh1)), (Gv.S("výška %.2f → %.2f") % Gd.Arr(rh1, S.G("sk").G("rakeH"))));
    cb.C("act", Gv.S("hyd"), Gv.B(true));
    S = S.S("cab", S.G("cab").S("interlock", Gv.B(true)));
    cb.C("act", Gv.S("pvarna"), Gv.B(true));
    chk(Gv.S("ochrana: C1 nejde pustit se zavřeným sáním / výtlakem (interlock)"), Gv.B(!(S.G("p1").G("on")).T()));
    S = S.S("cab", S.G("cab").S("interlock", Gv.B(false)));
    cb.C("act", Gv.S("pvarna"), Gv.B(true));
    chk(Gv.S("rozvaděč: volič VÁRNA ČERPADLO spustí C1 (ochrany vypnuté)"), S.G("p1").G("on"));
    cb.C("act", Gv.S("key"), Gv.B(true));
    sim.C("sim_step", Gv.F(1.0));
    chk(Gv.S("rozvaděč: klíč START vypnut = čerpadla stojí a nic se nedá zapnout"), Gv.B((!(S.G("p1").G("on")).T() && !(sim.C("cab_live")).T())));
    cb.C("act", Gv.S("pvarna"), Gv.B(true));
    chk(Gv.S("rozvaděč: bez napětí volič nic nezapne"), Gv.B(!(S.G("p1").G("on")).T()));
    cb.C("act", Gv.S("key"), Gv.B(true));
    cb.C("act", Gv.S("estop"), Gv.B(true));
    chk(Gv.S("rozvaděč: hřib nouzového vypnutí vypne rozvaděč a shodí klíč"), Gv.B((((S.G("cab").G("estop")).T() && !(S.G("cab").G("key")).T()) && !(sim.C("cab_live")).T())));
    cb.C("act", Gv.S("estop"), Gv.B(true));
    chk(Gv.S("rozvaděč: po odblokování hřibu je zase pod napětím (klíč START už na rozvaděči není)"), sim.C("cab_live"));
    cb.C("act", Gv.S("mpauto"), Gv.B(true));
    cb.C("act", Gv.S("mpheat"), Gv.B(true));
    S = S.S("rvp", S.G("rvp").S("T", Gv.F(20.0)));
    S = S.S("rvp", S.G("rvp").S("Tsens", Gv.F(20.0)));
    S = S.S("rvp", S.G("rvp").S("w", Gv.F(1000.0)));
    sim.C("sim_step", Gv.F(1.0));
    chk(Gv.S("rozvaděč: ohřev MP v automatu pustí páru, když je pod nastavenou teplotou"), Gv.B(Gv.Gt(S.G("rvp").G("heat"), Gv.F(0.5))), (Gv.S("ohřev %.2f") % S.G("rvp").G("heat")));
    fresh();
    S = sim.G("S");
    S = S.S("sk", S.G("sk").S("cells", S.G("sk").G("cells").SI(Gv.I(0L), S.G("sk").G("cells").I(Gv.I(0L)).S("w", Gv.F(800.0)))));
    S = S.S("sk", S.G("sk").S("cells", S.G("sk").G("cells").SI(Gv.I(4L), S.G("sk").G("cells").I(Gv.I(4L)).S("w", Gv.F(150.0)))));
    S = S.S("sk", S.G("sk").S("g", Gv.F(60.0)));
    S = S.S("sk", S.G("sk").S("mode", Gv.S("lauter")));
    Gv drained0 = (S.G("spillL")).AsF();
    foreach (Gv i_26 in (Gv.I(120L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
    }
    chk(Gv.S("zavřený výhoz mláta nic nevypouští"), Gv.B((Gv.Lt((S.G("spillL") - drained0), Gv.F(0.1)) && Gv.Gt(S.G("sk").G("g"), Gv.F(59.9)))));
    S = S.S("sk", S.G("sk").SI(Gv.S("hatch"), Gv.B(true)));
    S = S.S("sk", S.G("sk").S("rake", Gv.B(false)));
    foreach (Gv i_27 in (Gv.I(300L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
    }
    Gv g_norake = ((Gv.F(60.0) - S.G("sk").G("g"))).AsF();
    Gv liq_left = Gv.F(0.0);
    foreach (Gv cl_ in (S.G("sk").G("cells")).Iter()) {
      liq_left = (liq_left + sim.C("cellVol", cl_));
    }
    chk(Gv.S("otevřený výhoz mláta vypustí kapalinu do kanálu"), Gv.B((Gv.Gt((S.G("spillL") - drained0), Gv.F(800.0)) && Gv.Lt(liq_left, Gv.F(5.0)))), (Gv.S("vyteklo %.0f L, kapaliny zbývá %.0f L") % Gd.Arr((S.G("spillL") - drained0), liq_left)));
    fresh();
    S = sim.G("S");
    S = S.S("sk", S.G("sk").S("cells", S.G("sk").G("cells").SI(Gv.I(0L), S.G("sk").G("cells").I(Gv.I(0L)).S("w", Gv.F(800.0)))));
    S = S.S("sk", S.G("sk").S("g", Gv.F(60.0)));
    S = S.S("sk", S.G("sk").S("mode", Gv.S("lauter")));
    S = S.S("sk", S.G("sk").SI(Gv.S("hatch"), Gv.B(true)));
    S = S.S("sk", S.G("sk").S("rake", Gv.B(true)));
    foreach (Gv i_28 in (Gv.I(300L)).Iter()) {
      sim.C("sim_step", Gv.F(1.0));
    }
    chk(Gv.S("s kopačkou mláto z výhozu vypadává rychleji než bez ní"), Gv.B(Gv.Gt(((Gv.F(60.0) - S.G("sk").G("g"))), (g_norake * Gv.F(1.5)))), (Gv.S("bez %.1f kg, s kopačkou %.1f kg") % Gd.Arr(g_norake, (Gv.F(60.0) - S.G("sk").G("g")))));
    Gv valves_ = Gd.Arr();
    foreach (Gv it_8 in (m.G("items")).Iter()) {
      if ((Gv.Eq(it_8.G("type"), Gv.S("valve")) && Gv.Ne(it_8.C("get", Gv.S("kind"), Gv.S("")), Gv.S("zasl")))) {
        valves_.C("append", it_8);
      }
    }
    foreach (Gv seed_ in (Gd.Arr(Gv.I(11L), Gv.I(23L))).Iter()) {
      fresh();
      S = sim.G("S");
      Gv rng = Gd.New("RandomNumberGenerator");
      rng = rng.S("seed", seed_);
      foreach (Gv step in (Gv.I(3000L)).Iter()) {
        if (Gv.Eq((step % Gv.I(6L)), Gv.I(0L))) {
          {
            Gv _t1 = rng.C("randi_range", Gv.I(0L), Gv.I(9L));
            if (Gv.Eq(_t1, Gv.I(0L)) || Gv.Eq(_t1, Gv.I(1L)) || Gv.Eq(_t1, Gv.I(2L)) || Gv.Eq(_t1, Gv.I(3L))) {
              Gv vv = valves_.I(rng.C("randi_range", Gv.I(0L), (valves_.C("size") - Gv.I(1L))));
              vv = vv.SI(Gv.S("open"), Gv.B(!Gv.Ne(vv.C("get", Gv.S("open"), Gv.B(true)), Gv.B(false))));
              if ((sim.G("net").G("byId").C("has", vv.G("id"))).T()) {
                sim = sim.S("net", sim.G("net").S("byId", sim.G("net").G("byId").SI(vv.G("id"), sim.G("net").G("byId").I(vv.G("id")).SI(Gv.S("open"), vv.I(Gv.S("open"))))));
              }
              sim = sim.S("valve_ver", (sim.G("valve_ver") + Gv.I(1L)));
            }
            else if (Gv.Eq(_t1, Gv.I(4L)) || Gv.Eq(_t1, Gv.I(5L))) {
              S = S.SI(Gd.Arr(Gv.S("p1"), Gv.S("p2"), Gv.S("p3")).I(rng.C("randi_range", Gv.I(0L), Gv.I(2L))), S.I(Gd.Arr(Gv.S("p1"), Gv.S("p2"), Gv.S("p3")).I(rng.C("randi_range", Gv.I(0L), Gv.I(2L)))).S("on", Gv.B(Gv.Lt(rng.C("randf"), Gv.F(0.5)))));
            }
            else if (Gv.Eq(_t1, Gv.I(6L))) {
              S = S.S("rvpWater", S.G("rvpWater").S("on", Gv.B(Gv.Lt(rng.C("randf"), Gv.F(0.5)))));
            }
            else if (Gv.Eq(_t1, Gv.I(7L))) {
              S = S.S("rvp", S.G("rvp").S("stir", Gv.B(Gv.Lt(rng.C("randf"), Gv.F(0.5)))));
              S = S.S("rvp", S.G("rvp").S("heat", rng.C("randf")));
            }
            else if (Gv.Eq(_t1, Gv.I(8L))) {
              S = S.S("skWater", S.G("skWater").S("on", Gv.B(Gv.Lt(rng.C("randf"), Gv.F(0.5)))));
            }
            else if (Gv.Eq(_t1, Gv.I(9L))) {
              S = S.S("malt", S.G("malt").S("on", Gv.B(Gv.Lt(rng.C("randf"), Gv.F(0.5)))));
            }
          }
        }
        sim.C("sim_step", Gv.F(1.0));
      }
      Gv sane = Gv.B(true);
      Gv why = Gv.S("");
      foreach (Gv vk in (Gd.Arr(S.G("rvp"), S.G("mp"), S.G("wp"), S.G("sk").G("mash"))).Iter()) {
        foreach (Gv k_3 in (vk).Iter()) {
          if ((Gv.Eq(Gd.Dyn("typeof", vk.I(k_3)), Gd.K("TYPE_FLOAT")) && (!(Gd.is_finite(vk.I(k_3))).T() || (Gv.Lt(vk.I(k_3), Gv.F(-0.001)) && !Gv.In(k_3, Gd.Arr(Gv.S("O2"), Gv.S("smm"))))))) {
            sane = Gv.B(false);
            why = (Gv.S("%s=%s") % Gd.Arr(k_3, Gd.Dyn("str", vk.I(k_3))));
          }
        }
      }
      foreach (Gv k2 in (Gd.Arr(Gv.S("hlt"), Gv.S("lv"))).Iter()) {
        if (((!(Gd.is_finite(S.I(k2).G("w"))).T() || Gv.Lt(S.I(k2).G("w"), Gv.F(-0.001))) || !(Gd.is_finite(S.I(k2).G("T"))).T())) {
          sane = Gv.B(false);
          why = k2;
        }
      }
      foreach (Gv p_ in (Gd.Arr(Gv.S("p1"), Gv.S("p2"), Gv.S("p3"))).Iter()) {
        if (((!(Gd.is_finite(S.I(p_).G("q"))).T() || Gv.Lt(S.I(p_).G("prime"), Gv.F(0.0))) || Gv.Gt(S.I(p_).G("prime"), Gv.F(1.0001)))) {
          sane = Gv.B(false);
          why = p_;
        }
      }
      chk((Gv.S("odolnost: náhodné klikání (seed %d) nechá stav konečný a nezáporný") % seed_), sane, why);
    }
    Gv AU = Gd.Script("audit_tests").C("new");
    AU.C("run", Self);
    Gv bad = Gv.I(0L);
    foreach (Gv r in (results).Iter()) {
      if (!(r.I(Gv.I(1L))).T()) {
        bad = (bad + Gv.I(1L));
      }
    }
    Gd.print((Gv.S("TESTY: %d kontrol, %d selhání") % Gd.Arr(results.C("size"), bad)));
    return (bad).AsI();
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "m": return this.m;
    case "sim": return this.sim;
    case "results": return this.results;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "m": this.m = v; return v;
    case "sim": this.sim = v; return v;
    case "results": this.results = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "chk": return chk(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "near": return near(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "fresh": return fresh();
    case "bed": return bed();
    case "bed_mash_only": return bed_mash_only();
    case "run": return run();
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_tests(); return Gv.O(o); }
  public override string ScriptName { get { return "tests"; } }
}
}
