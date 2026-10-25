// Vygenerováno z practice.gd překladačem gd2cs.py – needituj ručně.
using System; using System.Collections.Generic;
namespace Pivovar.Gd.Gen {
public partial class S_practice : GdObject {
  public Gv m;
  public Gv sim;
  public static Gv RULES;
  public static Gv QN;
  public Gv _ph;
  public Gv _nostir;
  static S_practice() {
    RULES = Gd.Arr(Gd.Arr(Gv.S("Spuštění čerpadla"), Gv.S("otevřít klapky PŘED čerpadlem → nechat zavodnit (natéct, odvzdušnit) → otevřít klapky ZA čerpadlem → pustit na nízké otáčky → přidávat postupně")), Gd.Arr(Gv.S("Vypnutí čerpadla"), Gv.S("ubrat otáčky → VYPNOUT čerpadlo → zavřít klapku ZA čerpadlem → zavřít klapku PŘED čerpadlem (čerpadlo zůstane zavodněné)")), Gd.Arr(Gv.S("Nikdy"), Gv.S("nezavírat klapky za chodu čerpadla (nasucho / tlačí do zavřené klapky), nepouštět čerpadlo nasucho")), Gd.Arr(Gv.S("Pára"), Gv.S("nejdřív míchadlo, pak pára; páru otevírat pozvolna (odvodnit kondenzát), plášť musí být pod hladinou")), Gd.Arr(Gv.S("Vystírání"), Gv.S("míchadlo běží dřív, než se pustí slad; voda 35–40 °C (nad 60 °C trpí β-amyláza)")), Gd.Arr(Gv.S("Horká mladina"), Gv.S("u čerpání horké mladiny hlídat hladinu v sací nádobě – při nízké hladině kavituje")), Gd.Arr(Gv.S("Chlazení"), Gv.S("nejdřív pustit chladivo (voda / ledová), pak mladinu; končit obráceně – nejdřív mladinu zastavit")), Gd.Arr(Gv.S("Kotel"), Gv.S("nepouštět naplno všechny ohřevy naráz – tlak páry spadne a nic se neohřívá")));
    QN = Gd.Dict(Gv.S("p1"), Gv.F(400.0), Gv.S("p2"), Gv.F(150.0), Gv.S("p3"), Gv.F(200.0));
  }
  public S_practice() {
    m = Gv.Null;
    sim = Gv.Null;
    _ph = Gd.Dict();
    _nostir = Gd.Dict();
  }
  public Gv _S() {
    return sim.G("S");
    return Gv.Null;
  }
  public Gv _bad(Gv rule = default(Gv), Gv msg = default(Gv), Gv why = default(Gv), Gv fix = default(Gv)) {
    Gv S = _S();
    if (!(S.C("has", Gv.S("practice"))).T()) {
      S = S.S("practice", Gd.Dict(Gv.S("n"), Gv.I(0L), Gv.S("rules"), Gd.Dict()));
    }
    S = S.S("practice", S.G("practice").S("n", (S.G("practice").G("n") + Gv.I(1L))));
    S = S.S("practice", S.G("practice").S("rules", S.G("practice").G("rules").SI(rule, (S.G("practice").G("rules").C("get", rule, Gv.I(0L)) + Gv.I(1L)))));
    sim.C("logm", (Gv.S("ZÁSADA – %s %s Správně: %s") % Gd.Arr(msg, (Gv.Ne(why, Gv.S("")) ? ((Gv.S("Proč: ") + why) + Gv.S(".")) : Gv.S("")), fix)), Gv.S("warn"));
    if ((Gv.Ne(m, Gv.Null) && Gv.Ne(m.G("hud"), Gv.Null))) {
      m.G("hud").C("toast", (Gv.S("⚠ ") + msg));
    }
    return Gv.Null;
  }
  public Gv _tip(Gv key = default(Gv), Gv msg = default(Gv)) {
    sim.C("once", key, (Gv.S("TIP – ") + msg), Gv.S("info"));
    return Gv.Null;
  }
  public Gv _pump_name(Gv b = default(Gv)) {
    return (Gv.Ne(m, Gv.Null) ? m.C("pump_name", b) : b.C("to_upper"));
    return Gv.Null;
  }
  public Gv _ends(Gv b = default(Gv)) {
    Gv pis = sim.C("items_by_bind", b);
    if (!(pis.C("size")).T()) {
      return Gd.Dict();
    }
    Gv pn = sim.G("net").G("pumpNodes").C("get", pis.I(Gv.I(0L)).G("id"), Gd.Dict(Gv.S("in"), Gd.Arr(), Gv.S("out"), Gd.Arr()));
    Gv src = Gv.Null;
    Gv dst = Gd.Arr();
    foreach (Gv hh in (sim.C("bfs", pn.I(Gv.S("in"))).G("hits")).Iter()) {
      if (((hh.G("att").G("low")).T() && Gv.In(hh.G("att").G("bind"), Gd.Arr(Gv.S("rvp"), Gv.S("mp"), Gv.S("sk"), Gv.S("wp"), Gv.S("hlt"), Gv.S("spilka"), Gv.S("lv"))))) {
        if ((Gv.Eq(src, Gv.Null) || Gv.Gt(sim.C("_src_level", hh.G("att").G("bind")), sim.C("_src_level", src)))) {
          src = hh.G("att").G("bind");
        }
      }
    }
    foreach (Gv d in (sim.C("bfs", pn.G("out")).G("hits")).Iter()) {
      if (!Gv.In(d.G("att").G("bind"), dst)) {
        dst.C("append", d.G("att").G("bind"));
      }
    }
    return Gd.Dict(Gv.S("src"), src, Gv.S("dst"), dst);
    return Gv.Null;
  }
  public Gv _level_frac(Gv b = default(Gv)) {
    Gv cap = (Gv.F(0.0)).AsF();
    {
      Gv _t1 = b;
      if (Gv.Eq(_t1, Gv.S("rvp"))) {
        cap = (_S().G("cfg").C("get", Gv.S("rvpCap"), Gv.F(5000.0))).AsF();
      }
      else if (Gv.Eq(_t1, Gv.S("mp"))) {
        cap = (_S().G("cfg").C("get", Gv.S("mpCap"), Gv.F(5000.0))).AsF();
      }
      else if (Gv.Eq(_t1, Gv.S("wp"))) {
        cap = (_S().G("cfg").C("get", Gv.S("wpCap"), Gv.F(5000.0))).AsF();
      }
      else if (Gv.Eq(_t1, Gv.S("sk"))) {
        cap = (_S().G("cfg").G("skCap")).AsF();
      }
      else if (Gv.Eq(_t1, Gv.S("hlt"))) {
        cap = (_S().G("cfg").G("hltCap")).AsF();
      }
      else if (Gv.Eq(_t1, Gv.S("spilka"))) {
        cap = (_S().G("cfg").C("get", Gv.S("spCap"), Gv.F(5000.0))).AsF();
      }
      else if (Gv.Eq(_t1, Gv.S("lv"))) {
        cap = (_S().G("cfg").G("lvVol")).AsF();
      }
    }
    if (Gv.Le(cap, Gv.F(0.0))) {
      return (Gv.F(0.5)).AsF();
    }
    return ((Gv.Ne(b, Gv.S("lv")) ? (sim.C("_src_level", b) / cap) : (_S().G("lv").G("w") / cap))).AsF();
    return Gv.Null;
  }
  public Gv _temp(Gv b = default(Gv)) {
    Gv S = _S();
    {
      Gv _t1 = b;
      if (Gv.Eq(_t1, Gv.S("rvp")) || Gv.Eq(_t1, Gv.S("mp")) || Gv.Eq(_t1, Gv.S("wp"))) {
        return (S.I(b).G("T")).AsF();
      }
      else if (Gv.Eq(_t1, Gv.S("hlt"))) {
        return (S.G("hlt").G("T")).AsF();
      }
      else if (Gv.Eq(_t1, Gv.S("lv"))) {
        return (S.G("lv").G("T")).AsF();
      }
      else if (Gv.Eq(_t1, Gv.S("sk"))) {
        return (S.G("sk").G("T")).AsF();
      }
    }
    return (Gv.F(20.0)).AsF();
    return Gv.Null;
  }
  public Gv on_pump_start(Gv b = default(Gv)) {
    Gv S = _S();
    if (!Gv.In(b, Gd.Arr(Gv.S("p1"), Gv.S("p2"), Gv.S("p3")))) {
      if (Gv.Eq(b, Gv.S("plv"))) {
        return Gv.Null;
      }
      return Gv.Null;
    }
    Gv q = S.I(b);
    Gv pn = _pump_name(b);
    if ((Gv.Lt(q.C("get", Gv.S("prime"), Gv.F(1.0)), Gv.F(0.95)) && !(q.C("get", Gv.S("airLock"), Gv.B(false))).T())) {
      _bad(Gv.S("zavodneni"), (Gv.S("%s jsi pustil nezavodněné.") % pn), Gv.S("odstředivé čerpadlo vzduch nenasaje – běží naprázdno, hučí a ničí se mechanická ucpávka"), Gv.S("otevři klapky před čerpadlem, počkej, až čerpadlo zavodní (modrá kapka u čerpadla), teprve pak ho pusť"));
    }
    if (Gv.Gt(q.C("get", Gv.S("flow"), Gv.F(0.0)), (Gv.F(0.6) * QN.C("get", b, Gv.F(200.0))))) {
      _bad(Gv.S("rozjezd"), (Gv.S("%s jsi rozjel rovnou na vysoké otáčky (%d L/min).") % Gd.Arr(pn, Gd.roundi(q.G("flow")))), Gv.S("prudký rozjezd udělá ráz v potrubí, strhne vzduch; rmutu to láme zrno, u scezování utáhne lože"), Gv.S("pusť na nízké otáčky (FM 2–3) a přidávej postupně podle průtoku"));
    }
    Gv e = _ends(b);
    if ((e.C("is_empty")).T()) {
      return Gv.Null;
    }
    if (((Gv.Ne(e.G("src"), Gv.Null) && Gv.Gt(_temp(e.G("src")), Gv.F(90.0))) && Gv.Lt(_level_frac(e.G("src")), Gv.F(0.15)))) {
      _bad(Gv.S("kavitace"), (Gv.S("%s sání z %s: horká kapalina (%d °C) a nízká hladina.") % Gd.Arr(pn, sim.C("VN", e.G("src")), Gd.roundi(_temp(e.G("src"))))), Gv.S("vroucí kapalina má na sání skoro žádnou rezervu tlaku (NPSH) – čerpadlo kavituje: rachotí, nečerpá, ničí oběžné kolo"), Gv.S("čerpej horkou mladinu s dostatečnou hladinou, ke konci ubírej otáčky"));
    }
    foreach (Gv d in (e.G("dst")).Iter()) {
      if ((Gv.In(d, Gd.Arr(Gv.S("rvp"), Gv.S("mp"), Gv.S("sk"), Gv.S("wp"), Gv.S("hlt"), Gv.S("spilka"))) && Gv.Gt(_level_frac(d), Gv.F(0.95)))) {
        _bad(Gv.S("preplneni"), (Gv.S("%s tlačí do nádoby, která je skoro plná (%s).") % Gd.Arr(pn, d)), Gv.S("nádoba přeteče – ztráta a nebezpečí opaření"), Gv.S("zkontroluj objem v cílové nádobě dřív, než pustíš čerpadlo"));
      }
    }
    if ((Gv.Eq(b, Gv.S("p3")) && Gv.In(Gv.S("spilka"), e.G("dst")))) {
      Gv inlet_on = Gv.B(false);
      foreach (Gv it in (m.G("items")).Iter()) {
        if (((Gv.Eq(it.G("type"), Gv.S("vessel")) && Gv.Eq(it.C("get", Gv.S("shape")), Gv.S("inlet"))) && (it.C("get", Gv.S("on"), Gv.B(false))).T())) {
          inlet_on = Gv.B(true);
        }
      }
      if ((!(S.G("lv").G("pump")).T() && !(inlet_on).T())) {
        _bad(Gv.S("chlazeni"), Gv.S("Mladinu (C3) jsi pustil do chladiče dřív než chladivo."), Gv.S("bez vody / ledové vody jde mladina do spilky horká – zabije kvasnice, hrozí infekce a teplé zakvašení"), Gv.S("nejdřív pusť chladicí vodu a ledovou vodu (C5), pak C3 s mladinou"));
      }
    }
    return Gv.Null;
  }
  public Gv on_pump_stop(Gv b = default(Gv)) {
    Gv S = _S();
    if ((Gv.Eq(b, Gv.S("plv")) && (S.G("p3").G("on")).T())) {
      _bad(Gv.S("chlazeni_konec"), Gv.S("Ledovou vodu jsi zastavil, zatímco C3 ještě žene mladinu chladičem."), Gv.S("zbytek mladiny poteče do spilky nevychlazený"), Gv.S("na konci chlazení nejdřív zastav C3, pak ledovou vodu"));
      return Gv.Null;
    }
    if (!Gv.In(b, Gd.Arr(Gv.S("p1"), Gv.S("p2"), Gv.S("p3")))) {
      return Gv.Null;
    }
    if (Gv.Eq(sim.C("pump_interlock", b), Gv.S(""))) {
      _tip((Gv.S("stoptip%s%d") % Gd.Arr(b, sim.C("tb", Gv.I(1800L)))), (Gv.S("%s vypnuto. Teď zavři klapku ZA čerpadlem a pak PŘED ním – čerpadlo zůstane zavodněné a nic nepoteče samospádem zpátky.") % _pump_name(b)));
    }
    return Gv.Null;
  }
  public Gv on_valve(Gv _v = default(Gv)) {
    ;
    return Gv.Null;
  }
  public Gv tick(Gv dt = default(Gv)) {
    dt = (dt).AsF();
    Gv S = _S();
    foreach (Gv name in (Gd.Arr(Gv.S("rvp"), Gv.S("mp"))).Iter()) {
      Gv v = S.I(name);
      Gv h = (Gd.Dyn("float", v.C("get", Gv.S("heat"), Gv.F(0.0)))).AsF();
      Gv prev = (_ph.C("get", name, h)).AsF();
      if ((((Gv.Le(prev, Gv.F(0.05)) && Gv.Ge(h, Gv.F(0.6))) && (S.G("boiler").G("on")).T()) && Gv.Gt(sim.C("liqVol", v), Gv.I(100L)))) {
        _bad(Gv.S("para_naraz"), (Gv.S("Páru do %s jsi otevřel naráz na %d %%.") % Gd.Arr(sim.C("VN", name), Gd.roundi((h * Gv.F(100.0))))), Gv.S("v plášti a potrubí stojí kondenzát – prudce otevřená pára ho rozžene a udělá vodní ráz (rány, poškozené těsnění)"), Gv.S("otevři páru nejdřív na 10–20 %, nech odvodnit kondenzát a pak přidávej"));
      }
      _ph = _ph.SI(name, h);
      Gv solids = ((Gd.Dyn("float", v.C("get", Gv.S("g"), Gv.F(0.0))) + Gd.Dyn("float", v.C("get", Gv.S("su"), Gv.F(0.0))))).AsF();
      if ((((Gv.Gt(h, Gv.F(0.05)) && !(v.C("get", Gv.S("stir"), Gv.B(false))).T()) && Gv.Gt(solids, Gv.F(5.0))) && Gv.Gt(sim.C("liqVol", v), Gv.I(100L)))) {
        _nostir = _nostir.SI(name, (_nostir.C("get", name, Gv.F(0.0)) + dt));
        if (Gv.Gt(_nostir.I(name), Gv.F(30.0))) {
          _nostir = _nostir.SI(name, Gv.F(-540.0));
          _bad(Gv.S("micha_para"), (Gv.S("%s: topíš párou a míchadlo stojí.") % sim.C("VN", name)), Gv.S("stojící rmut se u horké stěny připaluje – připálenina zhorší chuť i přestup tepla a ohřev zpomalí"), Gv.S("nejdřív pusť míchadlo, pak otevři páru; při vypínání obráceně"));
        }
      } else if (Gv.Gt(_nostir.C("get", name, Gv.F(0.0)), Gv.F(0.0))) {
        _nostir = _nostir.SI(name, Gv.F(0.0));
      } else if (Gv.Lt(_nostir.C("get", name, Gv.F(0.0)), Gv.F(0.0))) {
        _nostir = _nostir.SI(name, Gd.minf(Gv.F(0.0), (_nostir.I(name) + dt)));
      }
      if (((((Gv.Eq(name, Gv.S("rvp")) && (S.G("malt").C("get", Gv.S("on"), Gv.B(false))).T()) && (S.G("malt").C("get", Gv.S("gate"), Gv.B(false))).T()) && !(v.C("get", Gv.S("stir"), Gv.B(false))).T()) && Gv.Gt(v.G("w"), Gv.I(50L)))) {
        if (!(S.C("get", Gv.S("_vystir_warn"), Gv.B(false))).T()) {
          S = S.S("_vystir_warn", Gv.B(true));
          _bad(Gv.S("vystirani"), Gv.S("Sypeš slad do pánve a míchadlo stojí."), Gv.S("slad se nerozmíchá – vzniknou hrudky se suchým středem, ztratíš extrakt a enzymy se nedostanou ke škrobu"), Gv.S("pusť míchadlo dřív, než otevřeš šoupě násypky"));
        }
      }
    }
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "m": return this.m;
    case "sim": return this.sim;
    case "RULES": return S_practice.RULES;
    case "QN": return S_practice.QN;
    case "_ph": return this._ph;
    case "_nostir": return this._nostir;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "m": this.m = v; return v;
    case "sim": this.sim = v; return v;
    case "_ph": this._ph = v; return v;
    case "_nostir": this._nostir = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "_S": return _S();
    case "_bad": return _bad(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "_tip": return _tip(Gd.A(a, 0), Gd.A(a, 1));
    case "_pump_name": return _pump_name(Gd.A(a, 0));
    case "_ends": return _ends(Gd.A(a, 0));
    case "_level_frac": return _level_frac(Gd.A(a, 0));
    case "_temp": return _temp(Gd.A(a, 0));
    case "on_pump_start": return on_pump_start(Gd.A(a, 0));
    case "on_pump_stop": return on_pump_stop(Gd.A(a, 0));
    case "on_valve": return on_valve(Gd.A(a, 0));
    case "tick": return tick(Gd.A(a, 0));
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    case "RULES": return RULES;
    case "QN": return QN;
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_practice(); return Gv.O(o); }
  public override string ScriptName { get { return "practice"; } }
}
}
