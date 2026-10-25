// Vygenerováno z sim.gd překladačem gd2cs.py – needituj ručně.
using System; using System.Collections.Generic;
namespace Pivovar.Gd.Gen {
public partial class S_sim : GdObject {
  public static Gv Physics;
  public static Gv Training;
  public static Gv LauterAuto;
  public static Gv Proc;
  public static Gv LIQ;
  public static Gv SOL;
  public static Gv CELLK;
  public static Gv CFG_DEFAULT;
  public static Gv HOPS;
  public static Gv MALTS;
  public static Gv RECIPE;
  public static Gv RECIPES;
  public static Gv TSAT;
  public static Gv PIPE_L_PER_M;
  public static Gv PX_PER_M;
  public Gv S;
  public Gv net;
  public Gv main;
  public Gv training;
  public Gv lauter_auto;
  public Gv deposit_on;
  public Gv proc;
  public Gv recipe_idx;
  public Gv last_route;
  public Gv last_goal;
  public Gv valve_ver;
  public Gv _cache;
  public Gv _cache_ver;
  public static Gv CALIB_PATH;
  public Gv calib_applied;
  public static Gv CALIB_RANGE;
  public Gv calib_rejected;
  public static Gv HA_REF;
  public Gv prof_on;
  public Gv prof;
  public Gv _dead_acc;
  public static Gv PUMPS;
  public static Gv DN_REAL;
  public Gv _mains_cache;
  static S_sim() {
    Physics = Gd.Script("physics");
    Training = Gd.Script("training");
    LauterAuto = Gd.Script("lauter_auto");
    Proc = Gd.Script("proc");
    LIQ = Gd.Arr(Gv.S("w"), Gv.S("sg"), Gv.S("dx"), Gv.S("ld"), Gv.S("f"), Gv.S("ns"), Gv.S("aA"), Gv.S("aB"), Gv.S("aBs"), Gv.S("smm"), Gv.S("dms"), Gv.S("alpha"), Gv.S("iso"), Gv.S("trubSus"), Gv.S("col"), Gv.S("fines"), Gv.S("air"));
    SOL = Gd.Arr(Gv.S("g"), Gv.S("su"), Gv.S("sc"), Gv.S("ni"));
    CELLK = Gd.Arr(Gv.S("w"), Gv.S("f"), Gv.S("n"), Gv.S("smm"), Gv.S("dms"), Gv.S("col"));
    CFG_DEFAULT = Gd.Dict(Gv.S("rvpCap"), Gv.F(5000.0), Gv.S("rvpUA"), Gv.F(0.25), Gv.S("rvpSteel"), Gv.F(450.0), Gv.S("rvpDead"), Gv.F(20.0), Gv.S("mpCap"), Gv.F(6000.0), Gv.S("mpUA"), Gv.F(0.3), Gv.S("mpSteel"), Gv.F(600.0), Gv.S("mpDead"), Gv.F(60.0), Gv.S("wpCap"), Gv.F(5000.0), Gv.S("wpDead"), Gv.F(30.0), Gv.S("wpUA"), Gv.F(0.35), Gv.S("wpSteel"), Gv.F(550.0), Gv.S("skCap"), Gv.F(5000.0), Gv.S("skArea"), Gv.F(5.3), Gv.S("skUA"), Gv.F(0.2), Gv.S("skSteel"), Gv.F(500.0), Gv.S("skRetentionLkg"), Gv.F(1.0), Gv.S("spargeEndP"), Gv.F(1.0), Gv.S("ambientT"), Gv.F(20.0), Gv.S("skContactUA"), Gv.F(4.0), Gv.S("skPerm"), Gv.F(1.5e-11), Gv.S("skSuction"), Gv.F(20000.0), Gv.S("spCap"), Gv.F(6000.0), Gv.S("waterFlow"), Gv.F(250.0), Gv.S("maltRate"), Gv.F(40.0), Gv.S("mainsQ"), Gv.F(900.0), Gv.S("mainsBar"), Gv.F(4.0), Gv.S("iboKeep"), Gv.F(0.44), Gv.S("coolWaterT"), Gv.F(16.0), Gv.S("UA1"), Gv.F(25.0), Gv.S("UA2"), Gv.F(12.0), Gv.S("boilerKW"), Gv.F(650.0), Gv.S("hltCap"), Gv.F(12000.0), Gv.S("hltStart"), Gv.F(11000.0), Gv.S("hltT0"), Gv.F(83.0), Gv.S("hltSet"), Gv.F(83.0), Gv.S("lvVol"), Gv.F(10000.0), Gv.S("lvStart"), Gv.F(0.0), Gv.S("lvT0"), Gv.F(2.0), Gv.S("lvSet"), Gv.F(2.0), Gv.S("chillerKW"), Gv.F(45.0), Gv.S("pivLoad"), Gv.F(10.0), Gv.S("tbWater"), Gv.F(99.3), Gv.S("malt"), Gv.F(680.0), Gv.S("maltExtract"), Gv.F(80.0), Gv.S("maltMoist"), Gv.F(4.5), Gv.S("mill"), Gv.F(1.0), Gv.S("steamBar"), Gv.F(2.5), Gv.S("rvpUAst"), Gv.F(6.5), Gv.S("mpUAst"), Gv.F(4.5), Gv.S("hltUAst"), Gv.F(2.0), Gv.S("rvpJacketL"), Gv.F(2200.0), Gv.S("mpJacketL"), Gv.F(2500.0), Gv.S("targetEBC"), Gv.F(8.0), Gv.S("pitchT"), Gv.F(8.5), Gv.S("targetV"), Gv.F(40.0), Gv.S("preboilV"), Gv.F(4430.0), Gv.S("mashFilmK"), Gv.F(4.6), Gv.S("boilerWaterKg"), Gv.F(1500.0), Gv.S("plateRefQ"), Gv.F(80.0), Gv.S("betaAct"), Gv.F(4.5), Gv.S("kdAlpha63"), Gv.F(0.022), Gv.S("kdBeta63"), Gv.F(0.13), Gv.S("targetP"), Gv.F(12.0), Gv.S("targetIBU"), Gv.F(32.0), Gv.S("boilMin"), Gv.F(90.0), Gv.S("cipLyeMin"), Gv.F(1.0), Gv.S("cipAcidMin"), Gv.F(0.5), Gv.S("cpSteel"), Gv.F(0.5), Gv.S("hltSteel"), Gv.F(600.0), Gv.S("hltUA"), Gv.F(0.01), Gv.S("skMuP"), Gv.F(0.047), Gv.S("pipeLenScale"), Gv.F(0.4), Gv.S("bore25"), Gv.F(50.0), Gv.S("bore40"), Gv.F(50.0), Gv.S("bore50"), Gv.F(75.0), Gv.S("p1_H0"), Gv.F(32.0), Gv.S("p1_Qn"), Gv.F(400.0), Gv.S("p2_H0"), Gv.F(22.0), Gv.S("p2_Qn"), Gv.F(150.0), Gv.S("p3_H0"), Gv.F(26.0), Gv.S("p3_Qn"), Gv.F(200.0), Gv.S("skDpComp"), Gv.F(30000.0), Gv.S("skTeigRho"), Gv.F(350.0), Gv.S("skTeigK"), Gv.F(0.03), Gv.S("skCreepK"), Gv.F(0.0015), Gv.S("skDpCompact"), Gv.F(80.0), Gv.S("skCompK"), Gv.F(0.05), Gv.S("skDpChannel"), Gv.F(160.0), Gv.S("skChanK"), Gv.F(0.02));
    HOPS = Gd.Arr(Gd.Dict(Gv.S("id"), Gv.S("zpc"), Gv.S("name"), Gv.S("Žatecký poloraný červeňák"), Gv.S("a"), Gv.F(3.5)), Gd.Dict(Gv.S("id"), Gv.S("sla"), Gv.S("name"), Gv.S("Sládek"), Gv.S("a"), Gv.F(6.5)), Gd.Dict(Gv.S("id"), Gv.S("pre"), Gv.S("name"), Gv.S("Premiant"), Gv.S("a"), Gv.F(8.0)), Gd.Dict(Gv.S("id"), Gv.S("kaz"), Gv.S("name"), Gv.S("Kazbek"), Gv.S("a"), Gv.F(5.5)), Gd.Dict(Gv.S("id"), Gv.S("agn"), Gv.S("name"), Gv.S("Agnus"), Gv.S("a"), Gv.F(10.0)));
    MALTS = Gd.Dict(Gv.S("plz"), Gd.Dict(Gv.S("name"), Gv.S("Plzeňský slad"), Gv.S("ext"), Gv.F(80.5), Gv.S("ebc"), Gv.F(3.5), Gv.S("enz"), Gv.F(1.0), Gv.S("unf"), Gv.F(0.0)), Gv.S("mni"), Gd.Dict(Gv.S("name"), Gv.S("Mnichovský slad"), Gv.S("ext"), Gv.F(79.0), Gv.S("ebc"), Gv.F(15.0), Gv.S("enz"), Gv.F(0.7), Gv.S("unf"), Gv.F(0.0)), Gv.S("kar"), Gd.Dict(Gv.S("name"), Gv.S("Karamelový slad"), Gv.S("ext"), Gv.F(75.0), Gv.S("ebc"), Gv.F(120.0), Gv.S("enz"), Gv.F(0.0), Gv.S("unf"), Gv.F(0.55)), Gv.S("plzS"), Gd.Dict(Gv.S("name"), Gv.S("Plzeňský slad Soufflet"), Gv.S("ext"), Gv.F(81.6), Gv.S("ebc"), Gv.F(4.0), Gv.S("enz"), Gv.F(1.0), Gv.S("unf"), Gv.F(0.0), Gv.S("moist"), Gv.F(4.5)), Gv.S("mniW"), Gd.Dict(Gv.S("name"), Gv.S("Mnichovský slad Weyermann typ 1"), Gv.S("ext"), Gv.F(78.0), Gv.S("ebc"), Gv.F(15.0), Gv.S("enz"), Gv.F(0.7), Gv.S("unf"), Gv.F(0.0), Gv.S("moist"), Gv.F(4.5)), Gv.S("cpl"), Gd.Dict(Gv.S("name"), Gv.S("Carapils Weyermann"), Gv.S("ext"), Gv.F(75.0), Gv.S("ebc"), Gv.F(4.5), Gv.S("enz"), Gv.F(0.0), Gv.S("unf"), Gv.F(0.35), Gv.S("moist"), Gv.F(7.0)), Gv.S("bar"), Gd.Dict(Gv.S("name"), Gv.S("Barvicí slad"), Gv.S("ext"), Gv.F(70.0), Gv.S("ebc"), Gv.F(1300.0), Gv.S("enz"), Gv.F(0.0), Gv.S("unf"), Gv.F(0.3)), Gv.S("vid"), Gd.Dict(Gv.S("name"), Gv.S("Vídeňský slad"), Gv.S("ext"), Gv.F(79.5), Gv.S("ebc"), Gv.F(8.0), Gv.S("enz"), Gv.F(0.9), Gv.S("unf"), Gv.F(0.0), Gv.S("moist"), Gv.F(4.5)));
    RECIPE = Gd.Dict(Gv.S("id"), Gv.S("125"), Gv.S("name"), Gv.S("12,5° Světlá (Zajíc)"), Gv.S("P"), Gv.F(12.5), Gv.S("IBU"), Gv.F(32.0), Gv.S("EBC"), Gv.F(9.0), Gv.S("grist"), Gd.Dict(Gv.S("plzS"), Gv.F(600.0), Gv.S("cpl"), Gv.F(75.0), Gv.S("mniW"), Gv.F(25.0)), Gv.S("water"), Gv.F(2000.0), Gv.S("boil"), Gv.F(80.0), Gv.S("pitch"), Gv.F(8.5), Gv.S("hops"), Gd.Arr(Gd.Dict(Gv.S("min"), Gv.F(0.0), Gv.S("hop"), Gv.S("sla"), Gv.S("kg"), Gv.F(4.2)), Gd.Dict(Gv.S("min"), Gv.F(40.0), Gv.S("hop"), Gv.S("zpc"), Gv.S("kg"), Gv.F(3.0)), Gd.Dict(Gv.S("min"), Gv.F(65.0), Gv.S("hop"), Gv.S("zpc"), Gv.S("kg"), Gv.F(3.0))));
    RECIPES = Gd.Arr(RECIPE, Gd.Dict(Gv.S("id"), Gv.S("120"), Gv.S("name"), Gv.S("12° Světlá (rozpis Vasil)"), Gv.S("P"), Gv.F(12.0), Gv.S("IBU"), Gv.F(36.0), Gv.S("EBC"), Gv.F(9.0), Gv.S("grist"), Gd.Dict(Gv.S("plzS"), Gv.F(625.0), Gv.S("cpl"), Gv.F(60.0), Gv.S("vid"), Gv.F(50.0)), Gv.S("water"), Gv.F(2000.0), Gv.S("boil"), Gv.F(90.0), Gv.S("pitch"), Gv.F(8.5), Gv.S("hops"), Gd.Arr(Gd.Dict(Gv.S("min"), Gv.F(0.0), Gv.S("hop"), Gv.S("zpc"), Gv.S("kg"), Gv.F(1.5)), Gd.Dict(Gv.S("min"), Gv.F(0.0), Gv.S("hop"), Gv.S("sla"), Gv.S("kg"), Gv.F(2.28)), Gd.Dict(Gv.S("min"), Gv.F(45.0), Gv.S("hop"), Gv.S("pre"), Gv.S("kg"), Gv.F(2.0)), Gd.Dict(Gv.S("min"), Gv.F(75.0), Gv.S("hop"), Gv.S("zpc"), Gv.S("kg"), Gv.F(5.0)))), Gd.Dict(Gv.S("id"), Gv.S("100"), Gv.S("name"), Gv.S("10° Výčepní"), Gv.S("P"), Gv.F(10.0), Gv.S("IBU"), Gv.F(25.0), Gv.S("EBC"), Gv.F(7.0), Gv.S("grist"), Gd.Dict(Gv.S("plzS"), Gv.F(560.0), Gv.S("cpl"), Gv.F(30.0)), Gv.S("water"), Gv.F(1900.0), Gv.S("boil"), Gv.F(80.0), Gv.S("pitch"), Gv.F(8.5), Gv.S("hops"), Gd.Arr(Gd.Dict(Gv.S("min"), Gv.F(0.0), Gv.S("hop"), Gv.S("sla"), Gv.S("kg"), Gv.F(4.2)), Gd.Dict(Gv.S("min"), Gv.F(60.0), Gv.S("hop"), Gv.S("zpc"), Gv.S("kg"), Gv.F(2.0)))));
    TSAT = Gd.Arr(Gd.Arr(Gv.F(1.0), Gv.F(100.0)), Gd.Arr(Gv.F(2.0), Gv.F(120.2)), Gd.Arr(Gv.F(3.0), Gv.F(133.5)), Gd.Arr(Gv.F(4.0), Gv.F(143.6)), Gd.Arr(Gv.F(5.0), Gv.F(151.8)), Gd.Arr(Gv.F(6.0), Gv.F(158.8)), Gd.Arr(Gv.F(8.0), Gv.F(170.4)));
    PIPE_L_PER_M = Gv.F(2.0);
    PX_PER_M = Gv.F(100.0);
    CALIB_PATH = Gv.S("user://kalibrace.json");
    CALIB_RANGE = Gd.Dict(Gv.S("rvpCap"), Gd.Arr(Gv.F(1000.0), Gv.F(10000.0)), Gv.S("mpCap"), Gd.Arr(Gv.F(1000.0), Gv.F(10000.0)), Gv.S("wpCap"), Gd.Arr(Gv.F(1000.0), Gv.F(10000.0)), Gv.S("skCap"), Gd.Arr(Gv.F(1000.0), Gv.F(10000.0)), Gv.S("spCap"), Gd.Arr(Gv.F(1000.0), Gv.F(20000.0)), Gv.S("rvpUA"), Gd.Arr(Gv.F(0.0), Gv.F(3.0)), Gv.S("mpUA"), Gd.Arr(Gv.F(0.0), Gv.F(3.0)), Gv.S("wpUA"), Gd.Arr(Gv.F(0.0), Gv.F(3.0)), Gv.S("skUA"), Gd.Arr(Gv.F(0.0), Gv.F(3.0)), Gv.S("hltUA"), Gd.Arr(Gv.F(0.0), Gv.F(3.0)), Gv.S("rvpSteel"), Gd.Arr(Gv.F(0.0), Gv.F(3000.0)), Gv.S("mpSteel"), Gd.Arr(Gv.F(0.0), Gv.F(3000.0)), Gv.S("wpSteel"), Gd.Arr(Gv.F(0.0), Gv.F(3000.0)), Gv.S("skSteel"), Gd.Arr(Gv.F(0.0), Gv.F(3000.0)), Gv.S("hltSteel"), Gd.Arr(Gv.F(0.0), Gv.F(5000.0)), Gv.S("cpSteel"), Gd.Arr(Gv.F(0.4), Gv.F(0.6)), Gv.S("rvpUAst"), Gd.Arr(Gv.F(0.5), Gv.F(30.0)), Gv.S("mpUAst"), Gd.Arr(Gv.F(0.5), Gv.F(30.0)), Gv.S("hltUAst"), Gd.Arr(Gv.F(0.1), Gv.F(20.0)), Gv.S("rvpJacketL"), Gd.Arr(Gv.F(200.0), Gv.F(6000.0)), Gv.S("mpJacketL"), Gd.Arr(Gv.F(200.0), Gv.F(6000.0)), Gv.S("skArea"), Gd.Arr(Gv.F(1.0), Gv.F(20.0)), Gv.S("skPerm"), Gd.Arr(Gv.F(1e-13), Gv.F(1e-09)), Gv.S("skDpComp"), Gd.Arr(Gv.F(1000.0), Gv.F(200000.0)), Gv.S("skSuction"), Gd.Arr(Gv.F(0.0), Gv.F(60000.0)), Gv.S("skRetentionLkg"), Gd.Arr(Gv.F(0.5), Gv.F(3.0)), Gv.S("preboilV"), Gd.Arr(Gv.F(2000.0), Gv.F(5000.0)), Gv.S("mashFilmK"), Gd.Arr(Gv.F(0.0), Gv.F(30.0)), Gv.S("boilerWaterKg"), Gd.Arr(Gv.F(50.0), Gv.F(20000.0)), Gv.S("plateRefQ"), Gd.Arr(Gv.F(10.0), Gv.F(500.0)), Gv.S("betaAct"), Gd.Arr(Gv.F(0.1), Gv.F(50.0)), Gv.S("kdAlpha63"), Gd.Arr(Gv.F(0.001), Gv.F(1.0)), Gv.S("kdBeta63"), Gd.Arr(Gv.F(0.005), Gv.F(3.0)), Gv.S("skDpCompact"), Gd.Arr(Gv.F(10.0), Gv.F(500.0)), Gv.S("skDpChannel"), Gd.Arr(Gv.F(20.0), Gv.F(800.0)), Gv.S("skMuP"), Gd.Arr(Gv.F(0.0), Gv.F(0.1)), Gv.S("UA1"), Gd.Arr(Gv.F(1.0), Gv.F(200.0)), Gv.S("UA2"), Gd.Arr(Gv.F(1.0), Gv.F(200.0)), Gv.S("boilerKW"), Gd.Arr(Gv.F(50.0), Gv.F(3000.0)), Gv.S("steamBar"), Gd.Arr(Gv.F(0.2), Gv.F(8.0)), Gv.S("chillerKW"), Gd.Arr(Gv.F(5.0), Gv.F(500.0)), Gv.S("hltCap"), Gd.Arr(Gv.F(1000.0), Gv.F(30000.0)), Gv.S("lvVol"), Gd.Arr(Gv.F(1000.0), Gv.F(30000.0)), Gv.S("tbWater"), Gd.Arr(Gv.F(95.0), Gv.F(100.5)), Gv.S("ambientT"), Gd.Arr(Gv.F(-5.0), Gv.F(40.0)), Gv.S("coolWaterT"), Gd.Arr(Gv.F(1.0), Gv.F(30.0)), Gv.S("waterFlow"), Gd.Arr(Gv.F(10.0), Gv.F(600.0)), Gv.S("mainsQ"), Gd.Arr(Gv.F(100.0), Gv.F(2000.0)), Gv.S("mainsBar"), Gd.Arr(Gv.F(0.5), Gv.F(10.0)), Gv.S("iboKeep"), Gd.Arr(Gv.F(0.2), Gv.F(1.0)), Gv.S("maltRate"), Gd.Arr(Gv.F(5.0), Gv.F(200.0)), Gv.S("pipeLenScale"), Gd.Arr(Gv.F(0.05), Gv.F(3.0)), Gv.S("bore25"), Gd.Arr(Gv.F(15.0), Gv.F(120.0)), Gv.S("bore40"), Gd.Arr(Gv.F(15.0), Gv.F(120.0)), Gv.S("bore50"), Gd.Arr(Gv.F(15.0), Gv.F(150.0)), Gv.S("p1_H0"), Gd.Arr(Gv.F(3.0), Gv.F(80.0)), Gv.S("p2_H0"), Gd.Arr(Gv.F(3.0), Gv.F(80.0)), Gv.S("p3_H0"), Gd.Arr(Gv.F(3.0), Gv.F(80.0)), Gv.S("p1_Qn"), Gd.Arr(Gv.F(20.0), Gv.F(1500.0)), Gv.S("p2_Qn"), Gd.Arr(Gv.F(20.0), Gv.F(1500.0)), Gv.S("p3_Qn"), Gd.Arr(Gv.F(20.0), Gv.F(1500.0)), Gv.S("maltExtract"), Gd.Arr(Gv.F(60.0), Gv.F(85.0)), Gv.S("maltMoist"), Gd.Arr(Gv.F(2.0), Gv.F(10.0)), Gv.S("cipLyeMin"), Gd.Arr(Gv.F(0.3), Gv.F(5.0)), Gv.S("cipAcidMin"), Gd.Arr(Gv.F(0.1), Gv.F(5.0)));
    HA_REF = Gv.F(25.0);
    PUMPS = Gd.Dict(Gv.S("p1"), Gd.Dict(Gv.S("H0"), Gv.F(32.0), Gv.S("Qn"), Gv.F(400.0)), Gv.S("p2"), Gd.Dict(Gv.S("H0"), Gv.F(22.0), Gv.S("Qn"), Gv.F(150.0)), Gv.S("p3"), Gd.Dict(Gv.S("H0"), Gv.F(26.0), Gv.S("Qn"), Gv.F(200.0)));
    DN_REAL = Gd.Dict(Gv.F(25.0), Gv.F(50.0), Gv.F(40.0), Gv.F(50.0), Gv.F(50.0), Gv.F(75.0));
  }
  public S_sim() {
    S = Gd.Dict();
    net = Gd.Dict();
    main = Gv.Null;
    training = Gv.Null;
    lauter_auto = Gv.Null;
    deposit_on = Gv.B(false);
    proc = Gv.Null;
    recipe_idx = Gv.I(0L);
    last_route = Gd.Arr();
    last_goal = Gv.Null;
    valve_ver = Gv.I(0L);
    _cache = Gd.Dict();
    _cache_ver = Gv.I(-1L);
    calib_applied = Gd.Arr();
    calib_rejected = Gd.Arr();
    prof_on = Gv.B(false);
    prof = Gd.Dict();
    _dead_acc = Gd.Dict();
    _mains_cache = Gd.Dict();
  }
  public Gv vessel() {
    Gv v = Gd.Dict(Gv.S("T"), Gv.F(18.0), Gv.S("heat"), Gv.F(0.0), Gv.S("stir"), Gv.B(false), Gv.S("hz"), Gv.F(35.0), Gv.S("burn"), Gv.F(0.0), Gv.S("evapKgMin"), Gv.F(0.0), Gv.S("boiling"), Gv.B(false), Gv.S("boilMin"), Gv.F(0.0), Gv.S("coag"), Gv.F(0.0), Gv.S("trubSet"), Gv.F(0.0), Gv.S("trubHB"), Gv.F(0.0), Gv.S("whirlMin"), Gv.F(0.0), Gv.S("foam"), Gv.F(0.0), Gv.S("turbSum"), Gv.F(0.0), Gv.S("turbVol"), Gv.F(0.0), Gv.S("grainBoiling"), Gv.B(false), Gv.S("grainBoilMin"), Gv.F(0.0), Gv.S("settleMin"), Gv.F(0.0), Gv.S("Tsens"), Gv.F(18.0), Gv.S("Twall"), Gv.F(18.0), Gv.S("whirlNow"), Gv.B(false), Gv.S("boilStartT"), Gv.F(0.0), Gv.S("Pin"), Gv.F(0.0), Gv.S("vort"), Gv.F(0.0), Gv.S("virt"), Gv.B(false));
    foreach (Gv k in (LIQ).Iter()) {
      v = v.SI(k, Gv.F(0.0));
    }
    foreach (Gv k_2 in (SOL).Iter()) {
      v = v.SI(k_2, Gv.F(0.0));
    }
    return v;
    return Gv.Null;
  }
  public Gv cell() {
    return Gd.Dict(Gv.S("w"), Gv.F(0.0), Gv.S("f"), Gv.F(0.0), Gv.S("n"), Gv.F(0.0), Gv.S("smm"), Gv.F(0.0), Gv.S("dms"), Gv.F(0.0), Gv.S("col"), Gv.F(0.0));
    return Gv.Null;
  }
  public Gv apply_calibration(Gv c = default(Gv)) {
    Gv used = Gd.Arr();
    calib_rejected = Gd.Arr();
    if (!(Gd.Static("FileAccess", "file_exists", CALIB_PATH)).T()) {
      return used;
    }
    Gv d = Gd.Static("JSON", "parse_string", Gd.Static("FileAccess", "open", CALIB_PATH, Gd.StaticGet("FileAccess", "READ")).C("get_as_text"));
    if (!Gv.Is(d, "Dictionary")) {
      calib_rejected.C("append", Gv.S("soubor není platný JSON"));
      return used;
    }
    foreach (Gv k in (d).Iter()) {
      if (!(CFG_DEFAULT.C("has", k)).T()) {
        calib_rejected.C("append", (Gv.S("%s (neznámý parametr)") % k));
        continue;
      }
      Gv v = d.I(k);
      if (Gv.Is(v, "Dictionary")) {
        v = v.C("get", Gv.S("value"), Gv.Null);
      }
      if (!(Gv.Is(v, "float") || Gv.Is(v, "int"))) {
        calib_rejected.C("append", (Gv.S("%s (není číslo)") % k));
        continue;
      }
      Gv r = CALIB_RANGE.C("get", k, Gd.Arr(Gv.F(-1e+30), Gv.F(1e+30)));
      if (((Gv.Lt(Gd.Dyn("float", v), r.I(Gv.I(0L))) || Gv.Gt(Gd.Dyn("float", v), r.I(Gv.I(1L)))) || (Gd.is_nan(Gd.Dyn("float", v))).T())) {
        calib_rejected.C("append", (Gv.S("%s = %s (mimo rozsah %s–%s)") % Gd.Arr(k, Gd.Dyn("str", v), Gd.Dyn("str", r.I(Gv.I(0L))), Gd.Dyn("str", r.I(Gv.I(1L))))));
        continue;
      }
      c = c.SI(k, Gd.Dyn("float", v));
      used.C("append", k);
    }
    return used;
    return Gv.Null;
  }
  public Gv new_state() {
    Gv c = CFG_DEFAULT.C("duplicate");
    Gv R = RECIPES.I(Gd.clampi(recipe_idx, Gv.I(0L), (RECIPES.C("size") - Gv.I(1L))));
    c = c.S("recipe", R.G("name"));
    c = c.S("grist", R.G("grist").C("duplicate"));
    Gv mt = Gv.F(0.0);
    foreach (Gv k in (c.G("grist")).Iter()) {
      mt = (mt + c.G("grist").I(k));
    }
    c = c.S("malt", mt);
    c = c.S("targetP", R.G("P"));
    c = c.S("targetIBU", R.G("IBU"));
    c = c.S("targetEBC", R.G("EBC"));
    c = c.S("hopPlan", R.G("hops").C("duplicate", Gv.B(true)));
    c = c.S("pitchT", R.G("pitch"));
    c = c.S("water", R.G("water"));
    c = c.S("boilMin", R.G("boil"));
    calib_applied = apply_calibration(c);
    S = Gd.Dict(Gv.S("cfg"), c, Gv.S("t"), Gv.F(0.0), Gv.S("rvp"), vessel(), Gv.S("mp"), vessel(), Gv.S("wp"), vessel(), Gv.S("sk"), Gd.Dict(Gv.S("mash"), vessel(), Gv.S("cells"), Gd.Arr(cell(), cell(), cell(), cell(), cell()), Gv.S("T"), Gv.F(18.0), Gv.S("Tw"), Gv.F(20.0), Gv.S("g"), Gv.F(0.0), Gv.S("lost"), Gv.F(0.0), Gv.S("c"), Gv.F(0.0), Gv.S("rake"), Gv.B(false), Gv.S("rakeH"), Gv.F(1.0), Gv.S("dig"), Gv.F(0.0), Gv.S("hyd"), Gv.F(0.0), Gv.S("hydT"), Gv.Null, Gv.S("burst"), Gv.F(0.0), Gv.S("mode"), Gv.S("mash"), Gv.S("iodLiquid"), Gd.Dict(), Gv.S("iodGrist"), Gd.Dict(), Gv.S("vorlaufL"), Gv.F(0.0), Gv.S("initialExtract"), Gv.F(0.0), Gv.S("fines"), Gv.F(0.0), Gv.S("ch"), Gv.F(0.0), Gv.S("air"), Gv.F(0.0), Gv.S("dpAvail"), Gv.F(0.0), Gv.S("filt"), Gv.F(0.0), Gv.S("settle"), Gv.F(0.0), Gv.S("filling"), Gv.B(false), Gv.S("Qact"), Gv.F(0.0), Gv.S("Qmax"), Gv.F(0.0), Gv.S("dp"), Gv.F(0.0), Gv.S("outP"), Gv.F(0.0), Gv.S("outP0"), Gv.F(0.0), Gv.S("turb"), Gv.F(0.0), Gv.S("emptied"), Gv.B(false), Gv.S("hatch"), Gv.B(false), Gv.S("under"), Gv.F(0.0), Gv.S("landed"), Gv.B(false), Gv.S("underOK"), Gv.B(false), Gv.S("bedMade"), Gv.B(false), Gv.S("drawn"), Gv.B(false), Gv.S("warmLog"), Gv.B(false), Gv.S("lostExt"), Gv.F(0.0), Gv.S("teig"), Gv.F(0.0), Gv.S("dpPa"), Gv.F(0.0), Gv.S("vac"), Gv.F(0.0)), Gv.S("sp"), Gd.Dict(Gv.S("w"), Gv.F(0.0), Gv.S("f"), Gv.F(0.0), Gv.S("n"), Gv.F(0.0), Gv.S("iso"), Gv.F(0.0), Gv.S("alpha"), Gv.F(0.0), Gv.S("dms"), Gv.F(0.0), Gv.S("trub"), Gv.F(0.0), Gv.S("col"), Gv.F(0.0), Gv.S("H"), Gv.F(0.0), Gv.S("L"), Gv.F(0.0), Gv.S("O2"), Gv.F(0.0), Gv.S("Tmax"), Gv.F(-99.0), Gv.S("Tmin"), Gv.F(99.0)), Gv.S("grist"), c.G("grist"), Gv.S("hopPlan"), c.G("hopPlan"), Gv.S("hopIdx"), Gv.I(0L), Gv.S("p1"), Gd.Dict(Gv.S("dry"), Gv.F(0.0), Gv.S("dead"), Gv.F(0.0), Gv.S("on"), Gv.B(false), Gv.S("flow"), Gv.F(300.0), Gv.S("status"), Gv.S(""), Gv.S("q"), Gv.F(0.0), Gv.S("prime"), Gv.F(0.0), Gv.S("pt"), Gv.F(0.0), Gv.S("flowing"), Gv.B(false)), Gv.S("p2"), Gd.Dict(Gv.S("dry"), Gv.F(0.0), Gv.S("dead"), Gv.F(0.0), Gv.S("on"), Gv.B(false), Gv.S("flow"), Gv.F(60.0), Gv.S("status"), Gv.S(""), Gv.S("q"), Gv.F(0.0), Gv.S("prime"), Gv.F(0.0), Gv.S("pt"), Gv.F(0.0), Gv.S("flowing"), Gv.B(false)), Gv.S("p3"), Gd.Dict(Gv.S("dry"), Gv.F(0.0), Gv.S("dead"), Gv.F(0.0), Gv.S("on"), Gv.B(false), Gv.S("flow"), Gv.F(70.0), Gv.S("status"), Gv.S(""), Gv.S("q"), Gv.F(0.0), Gv.S("prime"), Gv.F(0.0), Gv.S("pt"), Gv.F(0.0), Gv.S("flowing"), Gv.B(false)), Gv.S("rvpWater"), Gd.Dict(Gv.S("on"), Gv.B(false), Gv.S("T"), Gv.F(38.0), Gv.S("target"), c.G("water"), Gv.S("added"), Gv.F(0.0), Gv.S("k"), Gv.F(1.0)), Gv.S("malt"), Gd.Dict(Gv.S("on"), Gv.B(false), Gv.S("target"), c.G("malt"), Gv.S("added"), Gv.F(0.0), Gv.S("clump"), Gv.F(0.0), Gv.S("gate"), Gv.B(false)), Gv.S("rota"), Gd.Dict(Gv.S("stick"), Gv.F(0.0)), Gv.S("skWater"), Gd.Dict(Gv.S("on"), Gv.B(false), Gv.S("T"), Gv.F(78.0), Gv.S("flow"), Gv.F(60.0), Gv.S("total"), Gv.F(0.0)), Gv.S("cool"), Gd.Dict(Gv.S("waterT"), c.G("coolWaterT"), Gv.S("ice"), Gv.F(40.0), Gv.S("Tout"), Gv.Null, Gv.S("T1"), Gv.Null, Gv.S("Tw"), c.G("coolWaterT")), Gv.S("hlt"), Gd.Dict(Gv.S("w"), c.G("hltStart"), Gv.S("T"), c.G("hltT0"), Gv.S("set"), c.G("hltSet"), Gv.S("auto"), Gv.B(false), Gv.S("heat"), Gv.F(0.0), Gv.S("fill"), Gv.B(false)), Gv.S("lv"), Gd.Dict(Gv.S("w"), c.G("lvStart"), Gv.S("T"), (Gv.Gt(c.G("lvStart"), Gv.I(0L)) ? c.G("lvT0") : c.G("coolWaterT")), Gv.S("set"), c.G("lvSet"), Gv.S("chill"), Gv.B(Gv.Gt(c.G("lvStart"), Gv.I(0L))), Gv.S("chillOn"), Gv.B(false), Gv.S("varna"), Gv.B(true), Gv.S("piv"), Gv.B(true), Gv.S("pump"), Gv.B(Gv.Gt(c.G("lvStart"), Gv.I(0L))), Gv.S("reached"), Gv.B(false), Gv.S("glyk"), Gv.Null, Gv.S("glykWhy"), Gv.S(""), Gv.S("_circ"), Gv.B(false)), Gv.S("cab"), Gd.Dict(Gv.S("main"), Gv.B(true), Gv.S("key"), Gv.B(true), Gv.S("estop"), Gv.B(false), Gv.S("dead"), Gv.B(false), Gv.S("hs"), Gv.F(0.0), Gv.S("lmp"), Gv.B(true), Gv.S("lsk"), Gv.B(true), Gv.S("interlock"), Gv.B(true), Gv.S("mpAuto"), Gv.B(false), Gv.S("mpHeatOn"), Gv.B(false), Gv.S("hydOn"), Gv.B(false), Gv.S("potsk"), Gv.F(0.5), Gv.S("mpSet"), Gv.F(50.0), Gv.S("fault"), Gv.B(false), Gv.S("heatMem"), Gv.F(1.0)), Gv.S("boiler"), Gd.Dict(Gv.S("on"), Gv.B(true), Gv.S("load"), Gv.F(0.0), Gv.S("scale"), Gv.F(1.0), Gv.S("hltScale"), Gv.F(1.0), Gv.S("p"), Gv.F(2.5)), Gv.S("clog"), Gd.Dict(Gv.S("rvp"), Gv.F(0.0), Gv.S("sk"), Gv.F(0.0)), Gv.S("rek"), Gv.B(true), Gv.S("usedHot"), Gv.F(0.0), Gv.S("aer"), Gv.B(true), Gv.S("mixAuto"), Gv.B(false), Gv.S("hops"), Gd.Arr(), Gv.S("log"), Gd.Arr(), Gv.S("once"), Gd.Dict(), Gv.S("hist"), Gd.Arr(), Gv.S("flags"), Gd.Dict(Gv.S("rest52"), Gv.F(0.0), Gv.S("decCount"), Gv.I(0L), Gv.S("dmsPre"), Gv.Null, Gv.S("knock"), Gv.Null, Gv.S("boilStart"), Gv.Null, Gv.S("decStart"), Gv.I(0L), Gv.S("r63"), Gv.F(0.0), Gv.S("r72"), Gv.F(0.0), Gv.S("r72b"), Gv.F(0.0), Gv.S("r63b"), Gv.F(0.0), Gv.S("inSK"), Gv.I(0L), Gv.S("under"), Gv.I(0L), Gv.S("clearOK"), Gv.I(0L), Gv.S("full"), Gv.I(0L), Gv.S("wortBoil"), Gv.F(0.0), Gv.S("whirlDone"), Gv.I(0L), Gv.S("castStart"), Gv.Null, Gv.S("pushed"), Gv.I(0L)), Gv.S("extIn"), Gv.F(0.0), Gv.S("pH"), Gv.F(5.6), Gv.S("tannin"), Gv.F(0.0), Gv.S("usedWater"), Gv.F(0.0), Gv.S("maxSparge"), Gv.F(0.0), Gv.S("drainL"), Gv.F(0.0), Gv.S("drainExt"), Gv.F(0.0), Gv.S("spillL"), Gv.F(0.0), Gv.S("turbSum"), Gv.F(0.0), Gv.S("turbVol"), Gv.F(0.0), Gv.S("steamOK"), Gd.Dict(Gv.S("rvp"), Gv.B(true), Gv.S("mp"), Gv.B(true), Gv.S("hlt"), Gv.B(true)), Gv.S("_pd"), Gd.Dict(), Gv.S("Tsteam"), Gv.F(20.0), Gv.S("plates"), Gd.Dict(), Gv.S("plateDuty"), Gd.Dict(), Gv.S("plateFlow"), Gd.Dict(), Gv.S("lastDuty"), Gd.Dict(), Gv.S("lastPlateFlow"), Gd.Dict(), Gv.S("flows"), Gd.Arr(), Gv.S("_skTop"), Gv.I(0L), Gv.S("_skLow"), Gv.I(0L), Gv.S("_rvpIn"), Gv.I(0L), Gv.S("skFillNow"), Gv.B(false), Gv.S("lvFillLog"), Gv.B(false), Gv.S("leaks"), Gd.Arr(), Gv.S("streams"), Gd.Dict(), Gv.S("flush"), Gd.Dict(Gv.S("on"), Gv.B(false), Gv.S("L"), Gv.F(0.0), Gv.S("target"), Gv.F(300.0), Gv.S("vs"), Gd.Arr(), Gv.S("rate"), Gv.F(100.0)));
    if (Gv.Ne(proc, Gv.Null)) {
      proc.C("init_state");
    }
    return Gv.Null;
  }
  public Gv VN(Gv k = default(Gv)) {
    return Gd.Dict(Gv.S("rvp"), Gv.S("Pánev"), Gv.S("mp"), Gv.S("MP"), Gv.S("wp"), Gv.S("Vířivá káď"), Gv.S("sk"), Gv.S("Scezovací káď"), Gv.S("hlt"), Gv.S("Bojler"), Gv.S("lv"), Gv.S("Ledovka"), Gv.S("spilka"), Gv.S("Spilka")).C("get", k, k);
    return Gv.Null;
  }
  public Gv logm(Gv msg = default(Gv), Gv type = default(Gv)) {
    if (type.k == 0) type = Gv.S("info");
    S.G("log").C("push_front", Gd.Dict(Gv.S("t"), S.G("t"), Gv.S("msg"), msg, Gv.S("type"), type));
    if (Gv.Gt(S.G("log").C("size"), Gv.I(200L))) {
      S.G("log").C("pop_back");
    }
    return Gv.Null;
  }
  public Gv once(Gv key = default(Gv), Gv msg = default(Gv), Gv type = default(Gv)) {
    if (type.k == 0) type = Gv.S("info");
    if (!(S.G("once").C("has", key)).T()) {
      S = S.S("once", S.G("once").SI(key, Gv.I(1L)));
      logm(msg, type);
    }
    return Gv.Null;
  }
  public Gv tb(Gv x = default(Gv)) {
    x = (x).AsF();
    return (Gd.Dyn("int", Gd.floor((S.G("t") / x)))).AsI();
    return Gv.Null;
  }
  public Gv ext(Gv v = default(Gv)) {
    return (((((v.G("f") + v.G("dx")) + v.G("ld")) + v.G("ns")) + v.G("sg"))).AsF();
    return Gv.Null;
  }
  public Gv sol(Gv v = default(Gv)) {
    return ((((v.G("g") + v.G("su")) + v.G("sc")) + v.G("ni"))).AsF();
    return Gv.Null;
  }
  public Gv liqVol(Gv v = default(Gv)) {
    return ((v.G("w") + (ext(v) * Gv.F(0.62)))).AsF();
    return Gv.Null;
  }
  public Gv hotV(Gv v = default(Gv)) {
    return ((liqVol(v) * ((Gv.F(1.0) + (Gv.F(0.00052) * Gd.maxf(Gv.F(0.0), (v.G("T") - Gv.F(20.0)))))))).AsF();
    return Gv.Null;
  }
  public Gv vol(Gv v = default(Gv)) {
    return ((liqVol(v) + (sol(v) * Gv.F(0.65)))).AsF();
    return Gv.Null;
  }
  public Gv cC(Gv v = default(Gv)) {
    return (((Gv.F(4.19) * v.G("w")) + (Gv.F(1.5) * ((ext(v) + sol(v)))))).AsF();
    return Gv.Null;
  }
  public Gv plato(Gv v = default(Gv)) {
    Gv e = (ext(v)).AsF();
    Gv mass = ((e + v.G("w"))).AsF();
    return ((Gv.Gt(mass, Gv.F(1e-09)) ? ((Gv.F(100.0) * e) / mass) : Gv.F(0.0))).AsF();
    return Gv.Null;
  }
  public Gv cellVol(Gv c = default(Gv)) {
    return ((c.G("w") + (((c.G("f") + c.G("n"))) * Gv.F(0.62)))).AsF();
    return Gv.Null;
  }
  public Gv cellP(Gv c = default(Gv)) {
    Gv e = ((c.G("f") + c.G("n"))).AsF();
    Gv mass = ((e + c.G("w"))).AsF();
    return ((Gv.Gt(mass, Gv.F(1e-09)) ? ((Gv.F(100.0) * e) / mass) : Gv.F(0.0))).AsF();
    return Gv.Null;
  }
  public Gv stirQ(Gv v = default(Gv)) {
    return (((v.G("stir")).T() ? Gd.minf(Gv.F(1.0), (v.G("hz") / Gv.F(25.0))) : Gv.F(0.0))).AsF();
    return Gv.Null;
  }
  public Gv tsat(Gv pAbs = default(Gv)) {
    pAbs = (pAbs).AsF();
    foreach (Gv i in Gd.RangeIter(Gv.I(1L), TSAT.C("size"))) {
      if (Gv.Le(pAbs, TSAT.I(i).I(Gv.I(0L)))) {
        Gv a = TSAT.I((i - Gv.I(1L)));
        Gv b = TSAT.I(i);
        return ((a.I(Gv.I(1L)) + ((((b.I(Gv.I(1L)) - a.I(Gv.I(1L)))) * ((Gd.maxf(a.I(Gv.I(0L)), pAbs) - a.I(Gv.I(0L))))) / ((b.I(Gv.I(0L)) - a.I(Gv.I(0L))))))).AsF();
      }
    }
    return (Gv.F(170.0)).AsF();
    return Gv.Null;
  }
  public Gv lp() {
    return Gd.Dict(Gv.S("T"), Gv.F(18.0), Gv.S("w"), Gv.F(0.0), Gv.S("sg"), Gv.F(0.0), Gv.S("dx"), Gv.F(0.0), Gv.S("ld"), Gv.F(0.0), Gv.S("f"), Gv.F(0.0), Gv.S("ns"), Gv.F(0.0), Gv.S("aA"), Gv.F(0.0), Gv.S("aB"), Gv.F(0.0), Gv.S("aBs"), Gv.F(0.0), Gv.S("smm"), Gv.F(0.0), Gv.S("dms"), Gv.F(0.0), Gv.S("alpha"), Gv.F(0.0), Gv.S("iso"), Gv.F(0.0), Gv.S("trubSus"), Gv.F(0.0), Gv.S("col"), Gv.F(0.0), Gv.S("fines"), Gv.F(0.0), Gv.S("air"), Gv.F(0.0), Gv.S("g"), Gv.F(0.0), Gv.S("su"), Gv.F(0.0), Gv.S("sc"), Gv.F(0.0), Gv.S("ni"), Gv.F(0.0));
    return Gv.Null;
  }
  public Gv takePart(Gv src = default(Gv), Gv L = default(Gv), Gv solidFac = default(Gv)) {
    L = (L).AsF();
    solidFac = (solidFac).AsF();
    Gv V = vol(src);
    Gv p = lp();
    p = p.S("T", src.G("T"));
    if (Gv.Le(V, Gv.F(1e-06))) {
      return p;
    }
    Gv fr = Gd.minf(Gv.F(1.0), (L / V));
    Gv fs = Gd.minf(Gv.F(1.0), (fr * solidFac));
    foreach (Gv k in (LIQ).Iter()) {
      p = p.SI(k, (src.I(k) * fr));
      src = src.SI(k, (src.I(k) - p.I(k)));
    }
    foreach (Gv k_2 in (SOL).Iter()) {
      p = p.SI(k_2, (src.I(k_2) * fs));
      src = src.SI(k_2, (src.I(k_2) - p.I(k_2)));
    }
    return p;
    return Gv.Null;
  }
  public Gv steelC(Gv kg = default(Gv)) {
    kg = (kg).AsF();
    return ((kg * S.G("cfg").C("get", Gv.S("cpSteel"), Gv.F(0.5)))).AsF();
    return Gv.Null;
  }
  public Gv grain_dry_insoluble(Gv kg = default(Gv)) {
    kg = (kg).AsF();
    Gv gt = Gv.F(0.0);
    Gv out_ = Gv.F(0.0);
    foreach (Gv id in (S.G("grist")).Iter()) {
      gt = (gt + S.G("grist").I(id));
    }
    if (Gv.Le(gt, Gv.F(0.0))) {
      return (((kg * ((Gv.F(1.0) - (S.G("cfg").G("maltMoist") / Gv.F(100.0))))) * ((Gv.F(1.0) - (S.G("cfg").G("maltExtract") / Gv.F(100.0)))))).AsF();
    }
    foreach (Gv id_2 in (S.G("grist")).Iter()) {
      Gv M = MALTS.C("get", id_2, MALTS.G("plz"));
      Gv k = (((kg * S.G("grist").I(id_2)) / gt)).AsF();
      out_ = (out_ + ((k * ((Gv.F(1.0) - (M.C("get", Gv.S("moist"), S.G("cfg").G("maltMoist")) / Gv.F(100.0))))) * ((Gv.F(1.0) - (M.G("ext") / Gv.F(100.0))))));
    }
    return (out_).AsF();
    return Gv.Null;
  }
  public Gv addTo(Gv dst = default(Gv), Gv p = default(Gv), Gv steel = default(Gv)) {
    steel = (steel).AsF();
    Gv Cd = (cC(dst) + steelC(steel));
    Gv Cp = cC(p);
    if (Gv.Gt((Cd + Cp), Gv.I(0L))) {
      dst = dst.S("T", ((((Cd * dst.G("T")) + (Cp * p.G("T")))) / ((Cd + Cp))));
    }
    foreach (Gv k in (LIQ).Iter()) {
      dst = dst.SI(k, (dst.I(k) + p.I(k)));
    }
    foreach (Gv k_2 in (SOL).Iter()) {
      dst = dst.SI(k_2, (dst.I(k_2) + p.I(k_2)));
    }
    return Gv.Null;
  }
  public Gv takeCell(Gv c = default(Gv), Gv L = default(Gv)) {
    L = (L).AsF();
    Gv V = cellVol(c);
    Gv p = cell();
    if (Gv.Le(V, Gv.F(1e-09))) {
      return p;
    }
    Gv fr = Gd.minf(Gv.F(1.0), (L / V));
    foreach (Gv k in (CELLK).Iter()) {
      p = p.SI(k, (c.I(k) * fr));
      c = c.SI(k, (c.I(k) - p.I(k)));
    }
    return p;
    return Gv.Null;
  }
  public Gv mergeCell(Gv c = default(Gv), Gv p = default(Gv)) {
    foreach (Gv k in (CELLK).Iter()) {
      c = c.SI(k, (c.I(k) + p.I(k)));
    }
    return Gv.Null;
  }
  public Gv contentKind(Gv c = default(Gv)) {
    Gv V = (liqVol(c) + (sol(c) * Gv.F(0.65)));
    if (Gv.Lt(V, Gv.F(0.05))) {
      return Gv.S("empty");
    }
    if (Gv.Gt((sol(c) + c.G("sg")), (Gv.F(0.02) * V))) {
      return Gv.S("mash");
    }
    if (Gv.Gt(ext(c), (Gv.F(0.005) * V))) {
      return Gv.S("wort");
    }
    if (Gv.Gt(c.G("T"), Gv.I(45L))) {
      return Gv.S("hot");
    }
    if (Gv.Lt(c.G("T"), Gv.I(6L))) {
      return Gv.S("ice");
    }
    return Gv.S("water");
    return Gv.Null;
  }
  public Gv iodine(Gv v = default(Gv), Gv include_grist = default(Gv)) {
    if (include_grist.k == 0) include_grist = Gv.B(false);
    Gv st = (v.G("sg")).AsF();
    if ((include_grist).T()) {
      st = ((st + (v.G("su") + v.G("sc")))).AsF();
    }
    Gv tot = ((((st + v.G("dx")) + v.G("ld")) + v.G("f"))).AsF();
    if (Gv.Lt(tot, Gv.F(1.0))) {
      return Gv.Null;
    }
    Gv r = (st / tot);
    return (Gv.Lt(r, Gv.F(0.01)) ? Gv.S("neg") : ((Gv.Lt(r, Gv.F(0.04)) ? Gv.S("weak") : Gv.S("pos"))));
    return Gv.Null;
  }
  public Gv iodine_sample(Gv source = default(Gv), Gv grist = default(Gv)) {
    if (Gv.Eq(source, Gv.S("rvp"))) {
      return (Gv.Gt(vol(S.G("rvp")), Gv.F(30.0)) ? S.G("rvp") : Gv.Null);
    }
    if (Gv.Eq(source, Gv.S("sk"))) {
      if ((Gv.Eq(S.G("sk").G("mode"), Gv.S("mash")) && Gv.Gt(vol(S.G("sk").G("mash")), Gv.F(30.0)))) {
        return S.G("sk").G("mash");
      }
      Gv d = ((grist).T() ? S.G("sk").G("iodGrist") : S.G("sk").G("iodLiquid"));
      if ((Gv.Gt(S.G("sk").G("g"), Gv.F(1.0)) && Gv.Gt(d.C("size"), Gv.I(0L)))) {
        return d;
      }
    }
    return Gv.Null;
    return Gv.Null;
  }
  public Gv iodine_info(Gv v = default(Gv), Gv include_grist = default(Gv)) {
    if (include_grist.k == 0) include_grist = Gv.B(false);
    Gv starch = ((v.G("sg") + (((include_grist).T() ? (v.G("su") + v.G("sc")) : Gv.F(0.0))))).AsF();
    Gv total = (((((starch + v.G("dx")) + v.G("ld")) + v.G("f")) + ((!(include_grist).T() ? Gv.F(0.0) : Gv.F(0.0))))).AsF();
    if ((Gv.Lt(v.G("w"), Gv.F(1.0)) && Gv.Lt(total, Gv.F(0.5)))) {
      return Gd.Dict(Gv.S("empty"), Gv.B(true));
    }
    total = (Gd.maxf(total, Gv.F(0.5))).AsF();
    Gv s = ((starch / total)).AsF();
    Gv d = ((v.G("dx") / total)).AsF();
    Gv ws = (Gd.clampf((s * Gv.F(14.0)), Gv.F(0.0), Gv.F(1.0))).AsF();
    Gv wd = ((Gd.clampf((d * Gv.F(3.0)), Gv.F(0.0), Gv.F(0.85)) * ((Gv.F(1.0) - ws)))).AsF();
    Gv col = Gd.Ctor("Color", Gv.F(0.8), Gv.F(0.52), Gv.F(0.12)).C("lerp", Gd.Ctor("Color", Gv.F(0.62), Gv.F(0.2), Gv.F(0.14)), wd).C("lerp", Gd.Ctor("Color", Gv.F(0.08), Gv.F(0.05), Gv.F(0.22)), ws);
    Gv name = Gv.S("");
    if (Gv.Gt(ws, Gv.F(0.75))) {
      name = Gv.S("modročerná – hodně nezcukřeného škrobu");
    } else if (Gv.Gt(ws, Gv.F(0.4))) {
      name = Gv.S("tmavě modrá – škrob (nezcukřeno)");
    } else if (Gv.Gt(ws, Gv.F(0.15))) {
      name = Gv.S("fialová – škrob a velké dextriny");
    } else if (Gv.Gt(ws, Gv.F(0.04))) {
      name = Gv.S("červenofialová – dextriny, skoro zcukřeno");
    } else if (Gv.Gt(wd, Gv.F(0.4))) {
      name = Gv.S("červenohnědá – erythrodextriny (zcukřeno částečně)");
    } else if (Gv.Gt(wd, Gv.F(0.12))) {
      name = Gv.S("oranžovohnědá – stopy dextrinů");
    } else {
      name = Gv.S("žlutohnědá (barva jódu) – ve vzorku není škrob ani dextriny (negativní)");
    }
    return Gd.Dict(Gv.S("empty"), Gv.B(false), Gv.S("color"), col, Gv.S("name"), name, Gv.S("starch"), s, Gv.S("dextrin"), d, Gv.S("grist"), include_grist);
    return Gv.Null;
  }
  public Gv skMalt() {
    return ((S.G("sk").G("g") / Gv.F(0.191))).AsF();
    return Gv.Null;
  }
  public Gv skBedH() {
    return (((skMalt() * Gv.F(0.0027)) / S.G("cfg").G("skArea"))).AsF();
    return Gv.Null;
  }
  public Gv skRet() {
    return ((skMalt() * S.G("cfg").G("skRetentionLkg"))).AsF();
    return Gv.Null;
  }
  public Gv skC() {
    Gv C = ((Gv.F(1.4) * S.G("sk").G("g"))).AsF();
    foreach (Gv c in (S.G("sk").G("cells")).Iter()) {
      C = ((C + ((Gv.F(4.19) * c.G("w")) + (Gv.F(1.5) * ((c.G("f") + c.G("n"))))))).AsF();
    }
    return (C).AsF();
    return Gv.Null;
  }
  public Gv skHomog() {
    Gv sk = S.G("sk");
    Gv tot = cell();
    foreach (Gv c in (sk.G("cells")).Iter()) {
      mergeCell(tot, c);
    }
    Gv V = cellVol(tot);
    Gv cap = (skRet() / Gv.F(4.0));
    Gv rem = V;
    Gv vols = Gd.Arr(Gv.F(0.0), Gv.F(0.0), Gv.F(0.0), Gv.F(0.0), Gv.F(0.0));
    foreach (Gv i in Gd.RangeIter(Gv.I(4L), Gv.I(0L), Gv.I(-1L))) {
      vols = vols.SI(i, Gd.minf(cap, rem));
      rem = (rem - vols.I(i));
    }
    vols = vols.SI(Gv.I(0L), rem);
    foreach (Gv i_2 in (Gv.I(5L)).Iter()) {
      Gv fr = ((Gv.Gt(V, Gv.I(0L)) ? (vols.I(i_2) / V) : Gv.F(0.0))).AsF();
      foreach (Gv k in (CELLK).Iter()) {
        sk = sk.S("cells", sk.G("cells").SI(i_2, sk.G("cells").I(i_2).SI(k, (tot.I(k) * fr))));
      }
    }
    return Gv.Null;
  }
  public Gv skAddMash(Gv p = default(Gv)) {
    Gv sk = S.G("sk");
    Gv C0 = skC();
    Gv liqC = (((Gv.F(4.19) * p.G("w")) + (Gv.F(1.5) * ((ext(p) + sol(p)))))).AsF();
    sk = sk.S("T", ((((C0 * sk.G("T")) + (liqC * p.G("T")))) / ((C0 + liqC))));
    sk = sk.S("g", (sk.G("g") + p.G("g")));
    sk = sk.S("lost", (sk.G("lost") + (p.G("su") + p.G("sc"))));
    Gv top = sk.G("cells").I(Gv.I(0L));
    top = top.S("w", (top.G("w") + p.G("w")));
    top = top.S("f", (top.G("f") + p.G("f")));
    top = top.S("n", (top.G("n") + ((((p.G("sg") + p.G("dx")) + p.G("ld")) + p.G("ns")) + p.G("ni"))));
    top = top.S("smm", (top.G("smm") + p.G("smm")));
    top = top.S("dms", (top.G("dms") + p.G("dms")));
    top = top.S("col", (top.G("col") + p.G("col")));
    skHomog();
    return Gv.Null;
  }
  public Gv skVolTotal() {
    Gv V = Gv.F(0.0);
    foreach (Gv c in (S.G("sk").G("cells")).Iter()) {
      V = (V + cellVol(c));
    }
    return (((V + (skMalt() * Gv.F(0.65))) + vol(S.G("sk").G("mash")))).AsF();
    return Gv.Null;
  }
  public Gv rkRpm() {
    return (((Gv.F(1.5) * ((Gv.F(1.0) + (Gv.F(9.0) * S.G("cab").C("get", Gv.S("potsk"), Gv.F(0.5)))))) / Gv.F(5.5))).AsF();
    return Gv.Null;
  }
  public Gv skViscosity(Gv T = default(Gv), Gv P = default(Gv)) {
    T = (T).AsF();
    P = (P).AsF();
    return (((Gv.F(2.414e-05) * Gd.pow(Gv.F(10.0), (Gv.F(247.8) / ((T + Gv.F(133.15)))))) * Gd.exp((S.G("cfg").C("get", Gv.S("skMuP"), Gv.F(0.047)) * P)))).AsF();
    return Gv.Null;
  }
  public Gv skPerm() {
    Gv perm = (Gd.Arr(Gv.F(0.6), Gv.F(1.0), Gv.F(1.5)).I(Gd.clampi((Gd.roundi(S.G("cfg").G("mill")) - Gv.I(1L)), Gv.I(0L), Gv.I(2L)))).AsF();
    return (((S.G("cfg").G("skPerm") * perm) / ((Gv.F(1.0) + (Gv.F(2.5) * S.G("sk").G("c")))))).AsF();
    return Gv.Null;
  }
  public Gv skRes(Gv mu = default(Gv)) {
    mu = (mu).AsF();
    Gv sk = S.G("sk");
    Gv c = S.G("cfg");
    Gv k_bed = ((skPerm() / ((Gv.F(1.0) + (Gd.maxf(Gv.F(0.0), sk.C("get", Gv.S("dpPa"), Gv.F(0.0))) / c.G("skDpComp")))))).AsF();
    Gv L = (Gd.maxf(Gv.F(0.05), skBedH())).AsF();
    Gv Lt = ((sk.C("get", Gv.S("teig"), Gv.F(0.0)) / ((c.G("skArea") * c.G("skTeigRho"))))).AsF();
    Gv kt = ((c.G("skPerm") * c.G("skTeigK"))).AsF();
    return ((((mu * (((L / Gd.maxf(Gv.F(1e-16), k_bed)) + (Lt / kt)))) / Gd.pow((Gv.F(1.0) - (Gv.F(0.62) * sk.C("get", Gv.S("dep"), Gv.F(0.0)))), Gv.I(2L))) / ((c.G("skArea") * ((Gv.F(1.0) + (Gv.F(1.5) * sk.G("ch")))))))).AsF();
    return Gv.Null;
  }
  public Gv skQmax() {
    Gv sk = S.G("sk");
    Gv c = S.G("cfg");
    Gv outP = cellP((Gv.Gt(sk.G("cells").I(Gv.I(4L)).G("w"), Gv.I(1L)) ? sk.G("cells").I(Gv.I(4L)) : sk.G("cells").I(Gv.I(0L))));
    Gv mu = (skViscosity(sk.G("T"), outP)).AsF();
    Gv h_over = (((cellVol(sk.G("cells").I(Gv.I(0L))) / Gv.F(1000.0)) / c.G("skArea"))).AsF();
    Gv dpa = (((Gv.F(9810.0) * h_over) + c.G("skSuction"))).AsF();
    sk = sk.S("dpAvail", dpa);
    Gv q = (((Gv.F(60000.0) * dpa) / skRes(mu))).AsF();
    sk = sk.S("Qmax", (Gv.Lt(sk.G("g"), Gv.I(1L)) ? Gv.F(400.0) : q));
    sk = sk.S("outP0", outP);
    return Gv.Null;
  }
  public Gv skThermalStep(Gv seconds = default(Gv)) {
    seconds = (seconds).AsF();
    Gv sk = S.G("sk");
    Gv c = S.G("cfg");
    Gv capacities = Gd.Arr(skC(), cC(sk.G("mash")));
    Gv total = ((capacities.I(Gv.I(0L)) + capacities.I(Gv.I(1L)))).AsF();
    if (Gv.Gt(total, Gv.F(1e-09))) {
      foreach (Gv i in (Gv.I(2L)).Iter()) {
        if (Gv.Le(capacities.I(i), Gv.F(1e-09))) {
          continue;
        }
        Gv temperature = ((Gv.Eq(i, Gv.I(0L)) ? sk.G("T") : sk.G("mash").G("T"))).AsF();
        Gv nx = S_physics.exchange_bodies(temperature, capacities.I(i), sk.G("Tw"), steelC(c.G("skSteel")), ((c.G("skContactUA") * capacities.I(i)) / total), seconds);
        if (Gv.Eq(i, Gv.I(0L))) {
          sk = sk.S("T", nx.I(Gv.I(0L)));
        } else {
          sk = sk.S("mash", sk.G("mash").S("T", nx.I(Gv.I(0L))));
        }
        sk = sk.S("Tw", nx.I(Gv.I(1L)));
      }
    }
    sk = sk.S("Tw", (c.G("ambientT") + (((sk.G("Tw") - c.G("ambientT"))) * Gd.exp((((-c.G("skUA")) * seconds) / steelC(c.G("skSteel")))))));
    if (Gv.Gt(skC(), Gv.F(1.0))) {
      sk = sk.S("T", (sk.G("T") - ((((c.G("skUA") * Gv.F(0.3)) * ((sk.G("T") - c.G("ambientT")))) * seconds) / skC())));
    }
    return Gv.Null;
  }
  public Gv skMashToBed() {
    Gv sk = S.G("sk");
    if (Gv.Lt(vol(sk.G("mash")), Gv.I(1L))) {
      return Gv.Null;
    }
    Gv first = Gv.B(!(sk.G("bedMade")).T());
    if ((first).T()) {
      sk = sk.S("bedMade", Gv.B(true));
      sk = sk.SI(Gv.S("dep"), ((deposit_on).T() ? Gv.F(1.0) : Gv.F(0.0)));
      if (!(sk.G("underOK")).T()) {
        sk = sk.S("c", (sk.G("c") + Gv.F(0.18)));
      }
    }
    Gv cells = Gv.F(0.0);
    foreach (Gv x in (sk.G("cells")).Iter()) {
      cells = (cells + cellVol(x));
    }
    if ((Gv.Lt(sk.G("g"), Gv.I(1L)) && Gv.Lt(cells, Gv.I(5L)))) {
      sk = sk.S("T", sk.G("mash").G("T"));
    }
    Gv p = sk.G("mash");
    sk = sk.S("mash", vessel());
    Gv tl_ = ((((p.G("sg") + p.G("dx")) + p.G("ld")) + p.G("f"))).AsF();
    sk = sk.S("iodLiquid", Gd.Dict(Gv.S("sg"), p.G("sg"), Gv.S("su"), Gv.F(0.0), Gv.S("sc"), Gv.F(0.0), Gv.S("dx"), p.G("dx"), Gv.S("ld"), p.G("ld"), Gv.S("f"), p.G("f")));
    sk = sk.S("iodGrist", Gd.Dict(Gv.S("sg"), p.G("sg"), Gv.S("su"), p.G("su"), Gv.S("sc"), p.G("sc"), Gv.S("dx"), p.G("dx"), Gv.S("ld"), p.G("ld"), Gv.S("f"), p.G("f")));
    skAddMash(p);
    if ((first).T()) {
      Gv fl = (Gd.minf((sk.G("g") * Gv.F(0.2)), ((sk.G("underOK")).T() ? Gv.F(5.5) : Gv.F(9.0)))).AsF();
      sk = sk.S("g", (sk.G("g") - fl));
      sk = sk.S("fines", (sk.G("fines") + fl));
      Gv tg0 = (Gd.minf((sk.G("g") * Gv.F(0.1)), (Gv.F(0.01) * skMalt()))).AsF();
      sk = sk.S("g", (sk.G("g") - tg0));
      sk = sk.S("teig", (sk.C("get", Gv.S("teig"), Gv.F(0.0)) + tg0));
    }
    return Gv.Null;
  }
  public Gv skDraw(Gv dem = default(Gv), Gv m = default(Gv)) {
    dem = (dem).AsF();
    m = (m).AsF();
    skMashToBed();
    Gv sk = S.G("sk");
    Gv Q = Gd.minf(dem, sk.G("Qmax"));
    sk = sk.S("Qact", (sk.G("Qact") + Q));
    sk = sk.S("drawn", Gv.B(true));
    Gv ratio = ((dem / Gd.maxf(Gv.F(1.0), sk.G("Qmax")))).AsF();
    Gv qs = (sk.C("get", Gv.S("qSlow"), Gv.F(0.0))).AsF();
    Gv surge = ((Q - qs)).AsF();
    Gv dts = ((m * Gv.F(60.0))).AsF();
    sk = sk.SI(Gv.S("qSlow"), (qs + (((Q - qs)) * ((Gv.F(1.0) - Gd.exp(((-dts) / Gv.F(8.0))))))));
    sk = sk.SI(Gv.S("tearT"), Gd.maxf(Gv.F(0.0), (sk.C("get", Gv.S("tearT"), Gv.F(0.0)) - dts)));
    if (Gv.Gt(sk.G("g"), Gv.I(1L))) {
      Gv mu0 = (skViscosity(sk.G("T"), sk.G("outP0"))).AsF();
      Gv dp_pa = (((Q / Gv.F(60000.0)) * skRes(mu0))).AsF();
      sk = sk.S("dpPa", dp_pa);
      Gv dpm = ((dp_pa / Gv.F(100.0))).AsF();
      Gv c = S.G("cfg");
      Gv d1 = (c.G("skDpCompact")).AsF();
      Gv d2 = (c.G("skDpChannel")).AsF();
      sk = sk.S("c", Gd.minf(Gv.F(4.0), (sk.G("c") + (((m * c.G("skCreepK")) * dpm) / d1))));
      if (Gv.Gt(dpm, d1)) {
        sk = sk.S("c", Gd.minf(Gv.F(4.0), (sk.G("c") + ((m * c.G("skCompK")) * Gd.pow((((dpm - d1)) / d1), Gv.I(2L))))));
      }
      if (Gv.Gt(dpm, d2)) {
        sk = sk.S("ch", Gd.minf(Gv.F(0.5), (sk.G("ch") + (((c.G("skChanK") * m) * ((dpm - d2))) / d1))));
      }
      sk = sk.S("vac", Gd.clampf((((dem - sk.G("Qmax"))) / Gd.maxf(Gv.F(1.0), sk.G("Qmax"))), Gv.F(0.0), Gv.F(1.0)));
      if (Gv.Gt(ratio, Gv.F(1.05))) {
        once((Gv.S("stuckflow%d") % tb(Gv.I(900L))), (Gv.S("Čerpadlo táhne víc, než projde ložem – pod sítem vzniká podtlak (Δp %d mbar), mláto se utahuje. Zpomal C2 nebo nakypři.") % Gd.roundi(dpm)), Gv.S("warn"));
      } else if (Gv.Gt(dpm, Gv.F(120.0))) {
        once((Gv.S("dphigh%d") % tb(Gv.I(900L))), (Gv.S("Tlakový rozdíl na loži %d mbar – lože se slehává. Zpomal odběr a mělce nakypři (kopačka na střed).") % Gd.roundi(dpm)), Gv.S("warn"));
      }
      Gv tg = (Gd.minf((sk.G("g") * Gv.F(0.05)), ((Gv.F(0.0012) * Q) * m))).AsF();
      sk = sk.S("g", (sk.G("g") - tg));
      sk = sk.S("teig", (sk.C("get", Gv.S("teig"), Gv.F(0.0)) + tg));
    }
    Gv dep = (sk.C("get", Gv.S("dep"), Gv.F(0.0))).AsF();
    if ((((Gv.Gt(sk.G("g"), Gv.I(1L)) && Gv.Gt(surge, Gv.F(3.0))) && Gv.Gt(Q, Gv.F(8.0))) && Gv.Le(sk.G("tearT"), Gv.F(0.0)))) {
      Gv dv = (((surge / Gv.F(60000.0)) / Gv.F(0.00196))).AsF();
      Gv dpt = (((((Gv.F(1000.0) * Gv.F(12.0)) * dv) / Gd.clampf(dts, Gv.F(0.5), Gv.F(3.0))) / Gv.F(100.0))).AsF();
      if (Gv.Gt(dpt, Gv.F(10.0))) {
        sk = sk.S("tearT", Gv.F(10.0));
        if (Gv.Gt(dep, Gv.F(0.02))) {
          sk = sk.SI(Gv.S("dep"), Gd.maxf(Gv.F(0.0), (dep - (Gv.F(0.55) * Gd.clampf((((dpt - Gv.F(10.0))) / Gv.F(30.0)), Gv.F(0.2), Gv.F(1.5))))));
          sk = sk.S("fines", (sk.G("fines") + (Gv.F(0.4) * Gd.clampf((dpt / Gv.F(40.0)), Gv.F(0.3), Gv.F(1.5)))));
          logm((Gv.S("Ráz klapkou (+%d mbar na sítech) utrhl část nánosu ze dna%s.") % Gd.Arr(Gd.roundi(dpt), (Gv.Gt(sk.G("dep"), Gv.F(0.05)) ? ((Gv.S(" – zbývá asi %d %%, trhni ještě") % Gd.roundi((sk.G("dep") * Gv.F(100.0))))) : Gv.S(" – síta jsou volná, můžeš scezovat")))), Gv.S("info"));
        } else if (Gv.Gt(((sk.G("dpPa") / Gv.F(100.0)) + dpt), S.G("cfg").G("skDpCompact"))) {
          Gv st = (Gd.clampf(((((((sk.G("dpPa") / Gv.F(100.0)) + dpt) - S.G("cfg").G("skDpCompact"))) / S.G("cfg").G("skDpCompact")) + Gv.F(0.3)), Gv.F(0.3), Gv.F(1.5))).AsF();
          sk = sk.S("ch", Gd.minf(Gv.F(0.5), (sk.G("ch") + (Gv.F(0.15) * st))));
          sk = sk.S("c", Gd.minf(Gv.F(4.0), (sk.G("c") + (Gv.F(0.3) * st))));
          Gv tr = (Gd.minf(((sk.G("g") * Gv.F(0.012)) * st), Gv.F(6.0))).AsF();
          sk = sk.S("g", (sk.G("g") - tr));
          sk = sk.S("fines", (sk.G("fines") + tr));
          sk = sk.S("air", Gd.minf(Gv.F(1.0), (sk.G("air") + (Gv.F(0.1) * st))));
          logm((Gv.S("TRHNUTÍ! Rychlé otevření klapky dalo ráz +%d mbar (celkem %d mbar nad prahem stlačení %d) – lože se utrhlo, vznikly kanálky a zákal. Klapku otevírej pomalu.") % Gd.Arr(Gd.roundi(dpt), Gd.roundi(((sk.G("dpPa") / Gv.F(100.0)) + dpt)), Gd.roundi(S.G("cfg").G("skDpCompact")))), Gv.S("bad"));
        }
      }
    }
    Gv q = (Q * m);
    Gv out_ = Gd.NewDict();
    if (Gv.Lt(sk.G("g"), Gv.I(1L))) {
      out_ = takeCell(sk.G("cells").I(Gv.I(0L)), q);
    } else {
      out_ = takeCell(sk.G("cells").I(Gv.I(4L)), (q * ((Gv.F(1.0) - sk.G("ch")))));
      if (Gv.Gt(sk.G("ch"), Gv.F(0.001))) {
        mergeCell(out_, takeCell(sk.G("cells").I(Gv.I(0L)), (q * sk.G("ch"))));
      }
      Gv cap = (skRet() / Gv.F(4.0));
      foreach (Gv i in Gd.RangeIter(Gv.I(4L), Gv.I(0L), Gv.I(-1L))) {
        Gv deficit = (cap - cellVol(sk.G("cells").I(i)));
        if (Gv.Gt(deficit, Gv.I(0L))) {
          mergeCell(sk.G("cells").I(i), takeCell(sk.G("cells").I((i - Gv.I(1L))), deficit));
        }
      }
      if ((Gv.Lt(cellVol(sk.G("cells").I(Gv.I(0L))), Gv.I(3L)) && Gv.Gt(Q, Gv.I(0L)))) {
        sk = sk.S("c", (sk.G("c") + (Gv.F(0.08) * m)));
        sk = sk.S("ch", Gd.minf(Gv.F(0.5), (sk.G("ch") + (Gv.F(0.05) * m))));
        sk = sk.S("air", Gd.minf(Gv.F(1.0), (sk.G("air") + (Gv.F(0.4) * m))));
        once(Gv.S("drybed"), Gv.S("Hladina klesla pod mláto – do lože se tahá vzduch, lože se slehává a vznikají kanálky!"), Gv.S("bad"));
      }
    }
    Gv Lout = cellVol(out_);
    Gv fo = ((sk.G("fines") * ((Gv.F(1.0) - Gd.exp(((-Lout) / Gv.F(250.0))))))).AsF();
    sk = sk.S("fines", (sk.G("fines") - fo));
    sk = sk.S("turb", (Gv.F(3.0) + ((Gv.F(25.0) * ((sk.G("fines") + (fo * Gv.F(0.5))))) * ((Gv.Lt(sk.G("settle"), Gv.I(10L)) ? ((Gv.F(1.0) + (((Gv.F(10.0) - sk.G("settle"))) / Gv.F(10.0)))) : Gv.F(1.0))))));
    Gv op = cellP(out_);
    sk = sk.S("outP", (Gv.Gt(op, Gv.I(0L)) ? op : sk.G("outP0")));
    sk = sk.S("filt", (sk.G("filt") + Lout));
    Gv mu = (skViscosity(sk.G("T"), sk.G("outP"))).AsF();
    Gv dpn = (((Q / Gv.F(60000.0)) * skRes(mu))).AsF();
    sk = sk.S("dp", Gd.maxf(Gv.F(5.0), (dpn / Gv.F(100.0))));
    Gv p = lp();
    p = p.S("T", sk.G("T"));
    p = p.S("w", out_.G("w"));
    p = p.S("f", out_.G("f"));
    p = p.S("ns", out_.G("n"));
    p = p.S("smm", out_.G("smm"));
    p = p.S("dms", out_.G("dms"));
    p = p.S("col", out_.G("col"));
    p = p.S("fines", fo);
    p = p.S("air", ((sk.G("air") * Lout) * Gv.F(0.02)));
    return p;
    return Gv.Null;
  }
  public Gv sanSpray(Gv id = default(Gv)) {
    if ((Gv.Eq(id, Gv.Null) || Gv.Eq(id, Gv.S("")))) {
      return Gv.B(false);
    }
    Gv sp = net.G("byId").C("get", id);
    if ((Gv.Eq(sp, Gv.Null) || Gv.Ne(sp.G("type"), Gv.S("spray")))) {
      return Gv.B(false);
    }
    Gv sks = items_by_bind(Gv.S("sk"));
    if (!(sks.C("size")).T()) {
      return Gv.B(false);
    }
    Gv v = sks.I(Gv.I(0L));
    return Gv.B(((Gv.Ge(sp.G("x"), (v.G("x") - Gv.I(40L))) && Gv.Le(sp.G("x"), ((v.G("x") + v.G("w")) + Gv.I(40L)))) && Gv.Gt(Gd.absf((sp.G("x") - ((v.G("x") + (v.G("w") / Gv.F(2.0)))))), (v.G("w") * Gv.F(0.2)))));
    return Gv.Null;
  }
  public Gv skWallHeat(Gv part = default(Gv)) {
    Gv L = liqVol(part);
    if (Gv.Le(L, Gv.F(1e-06))) {
      return Gv.Null;
    }
    Gv sk = S.G("sk");
    Gv Tw = (sk.G("Tw")).AsF();
    Gv C = (steelC(S.G("cfg").G("skSteel"))).AsF();
    Gv Cw = (cC(part)).AsF();
    Gv Q = ((((Gv.F(0.6) * Cw) * C) / ((Cw + C))) * ((part.G("T") - Tw)));
    sk = sk.S("Tw", (Tw + (Q / C)));
    part = part.S("T", (part.G("T") - (Q / Cw)));
    if ((Gv.Ge(sk.G("Tw"), Gv.I(60L)) && !(sk.G("warmLog")).T())) {
      sk = sk.S("warmLog", Gv.B(true));
      logm((Gv.S("Scezovací káď prohřátá – stěny %s °C. Rmut se v ní tolik neochladí.") % fmt0(sk.G("Tw"))));
    }
    return Gv.Null;
  }
  public Gv skAddWater(Gv L = default(Gv), Gv T = default(Gv), Gv low = default(Gv)) {
    L = (L).AsF();
    T = (T).AsF();
    if (low.k == 0) low = Gv.B(false);
    Gv sk = S.G("sk");
    if ((Gv.Lt(skVolTotal(), Gv.I(5L)) && Gv.Le(vol(sk.G("mash")), Gv.I(5L)))) {
      sk = sk.S("T", sk.G("Tw"));
    }
    if ((((low).T() && Gv.Lt(sk.G("g"), Gv.I(1L))) && Gv.Le(vol(sk.G("mash")), Gv.I(20L)))) {
      Gv C0 = skC();
      Gv Cw = (Gv.F(4.19) * L);
      sk = sk.S("T", ((((C0 * sk.G("T")) + (Cw * T))) / ((C0 + Cw))));
      sk = sk.S("cells", sk.G("cells").SI(Gv.I(4L), sk.G("cells").I(Gv.I(4L)).S("w", (sk.G("cells").I(Gv.I(4L)).G("w") + L))));
      sk = sk.S("under", (sk.G("under") + L));
      if (Gv.Ge(sk.G("under"), Gv.I(120L))) {
        once(Gv.S("underok"), Gv.S("Podsítí zaplaveno vodou – síta jsou pod vodou, rmut dosedne měkce."));
      }
      return Gv.Null;
    }
    if (Gv.Gt(vol(sk.G("mash")), Gv.I(20L))) {
      Gv p = lp();
      p = p.S("w", L);
      p = p.S("T", T);
      addTo(sk.G("mash"), p, Gv.F(0.0));
      return Gv.Null;
    }
    Gv C1 = skC();
    Gv Cw1 = (Gv.F(4.19) * L);
    sk = sk.S("T", ((((C1 * sk.G("T")) + (Cw1 * T))) / ((C1 + Cw1))));
    sk = sk.S("cells", sk.G("cells").SI(Gv.I(0L), sk.G("cells").I(Gv.I(0L)).S("w", (sk.G("cells").I(Gv.I(0L)).G("w") + L))));
    return Gv.Null;
  }
  public Gv first_order(Gv k = default(Gv), Gv m = default(Gv)) {
    k = (k).AsF();
    m = (m).AsF();
    return ((Gv.F(1.0) - Gd.exp((-Gd.minf(Gv.F(50.0), Gd.maxf(Gv.F(0.0), (k * m))))))).AsF();
    return Gv.Null;
  }
  public Gv react(Gv v = default(Gv), Gv m = default(Gv)) {
    m = (m).AsF();
    if (Gv.Lt(v.G("w"), Gv.I(1L))) {
      return Gv.Null;
    }
    Gv T = (v.G("T")).AsF();
    Gv Vm3 = Gd.maxf(Gv.F(0.05), (liqVol(v) / Gv.F(1000.0)));
    if (Gv.Gt(v.G("ni"), Gv.I(0L))) {
      Gv d = ((v.G("ni") * first_order((Gv.F(0.08) * ((Gv.F(0.5) + (T / Gv.F(60.0))))), m))).AsF();
      v = v.S("ni", (v.G("ni") - d));
      v = v.S("f", (v.G("f") + (Gv.F(0.5) * d)));
      v = v.S("ns", (v.G("ns") + (Gv.F(0.5) * d)));
    }
    if (Gv.Gt(v.G("su"), Gv.I(0L))) {
      Gv d2 = ((v.G("su") * first_order((Gv.F(0.3) / ((Gv.F(1.0) + Gd.exp(((-((T - Gv.F(62.0)))) / Gv.F(1.3)))))), m))).AsF();
      v = v.S("su", (v.G("su") - d2));
      v = v.S("sg", (v.G("sg") + d2));
    }
    if (Gv.Gt(v.G("sc"), Gv.I(0L))) {
      Gv k = (Gv.Ge(T, Gv.I(95L)) ? Gv.F(0.08) : ((Gv.Gt(T, Gv.I(70L)) ? Gv.F(0.004) : Gv.F(0.0))));
      Gv d3 = ((v.G("sc") * first_order(k, m))).AsF();
      v = v.S("sc", (v.G("sc") - d3));
      v = v.S("sg", (v.G("sg") + d3));
    }
    Gv phf = S.C("get", Gv.S("phF"), Gd.StaticGet("Vector2", "ONE"));
    if ((Gv.Gt(v.G("aA"), Gv.F(0.01)) && Gv.Gt(v.G("sg"), Gv.I(0L)))) {
      Gv r = (((((Gv.F(0.0015) * ((v.G("aA") / Vm3))) * Gd.exp((-Gd.pow((((T - Gv.F(72.0))) / Gv.F(9.0)), Gv.I(2L))))) * m) * phf.G("x"))).AsF();
      Gv d4 = ((v.G("sg") * ((Gv.F(1.0) - Gd.exp((-Gd.minf(Gv.F(50.0), r))))))).AsF();
      Gv ldf = ((Gv.F(0.1) + (Gv.F(0.2) * Gd.clampf((((T - Gv.F(60.0))) / Gv.F(15.0)), Gv.F(0.0), Gv.F(1.0))))).AsF();
      v = v.S("sg", (v.G("sg") - d4));
      v = v.S("dx", (v.G("dx") + (((Gv.F(1.0) - ldf)) * d4)));
      v = v.S("ld", (v.G("ld") + (ldf * d4)));
    }
    Gv aBt = ((v.G("aB") + v.G("aBs"))).AsF();
    if (Gv.Gt(aBt, Gv.F(0.01))) {
      Gv r2 = ((((((Gv.F(0.0016) * S.G("cfg").C("get", Gv.S("betaAct"), Gv.F(4.5))) * ((aBt / Vm3))) * Gd.exp((-Gd.pow((((T - Gv.F(63.0))) / Gv.F(7.0)), Gv.I(2L))))) * m) * phf.G("y"))).AsF();
      Gv d5 = ((v.G("dx") * ((Gv.F(1.0) - Gd.exp((-Gd.minf(Gv.F(50.0), r2))))))).AsF();
      Gv d6 = ((v.G("sg") * ((Gv.F(1.0) - Gd.exp((-Gd.minf(Gv.F(50.0), (Gv.F(0.25) * r2)))))))).AsF();
      v = v.S("dx", (v.G("dx") - d5));
      v = v.S("sg", (v.G("sg") - d6));
      v = v.S("f", (v.G("f") + (d5 + d6)));
    }
    Gv iT = (((Gv.F(1.0) / Gv.F(336.15)) - (Gv.F(1.0) / ((T + Gv.F(273.15)))))).AsF();
    Gv kA = ((S.G("cfg").C("get", Gv.S("kdAlpha63"), Gv.F(0.022)) * Gd.exp((Gv.F(10067.0) * iT)))).AsF();
    Gv kB = ((S.G("cfg").C("get", Gv.S("kdBeta63"), Gv.F(0.13)) * Gd.exp((Gv.F(22612.0) * iT)))).AsF();
    v = v.S("aA", (v.G("aA") * Gd.exp((-Gd.minf(Gv.F(50.0), (kA * m))))));
    v = v.S("aB", (v.G("aB") * Gd.exp((-Gd.minf(Gv.F(50.0), (kB * m))))));
    v = v.S("aBs", (v.G("aBs") * Gd.exp((-Gd.minf(Gv.F(50.0), ((kB / Gv.F(40.0)) * m))))));
    Gv ph_d = (Gd.clampf(Gd.Dyn("float", S.C("get", Gv.S("pH"), Gv.F(5.5))), Gv.F(5.0), Gv.F(5.8))).AsF();
    Gv k100 = ((Gv.F(0.01824) + ((((Gv.F(0.0213) - Gv.F(0.01824))) * ((ph_d - Gv.F(5.2)))) / Gv.F(0.3)))).AsF();
    Gv ksmm = (Gv.Gt(T, Gv.I(60L)) ? (Gd.maxf(Gv.F(0.0), k100) * Gd.pow(Gv.F(2.0), (((T - Gv.F(100.0))) / Gv.F(6.0)))) : Gv.F(0.0));
    Gv kd = Gv.F(0.0);
    if ((v.G("boiling")).T()) {
      kd = ((v.G("evapKgMin") / Gd.maxf(Gv.F(1.0), v.G("w"))) * Gv.F(70.0));
    } else if (Gv.Gt(T, Gv.I(80L))) {
      kd = Gv.F(0.003);
    }
    if ((Gv.Gt(v.G("smm"), Gv.I(0L)) || Gv.Gt(v.G("dms"), Gv.I(0L)))) {
      Gv dn = S_physics.chain(v.G("smm"), v.G("dms"), ksmm, kd, m);
      v = v.S("smm", dn.I(Gv.I(0L)));
      v = v.S("dms", dn.I(Gv.I(1L)));
    }
    if ((Gv.Gt(v.G("alpha"), Gv.I(0L)) || Gv.Gt(v.G("iso"), Gv.I(0L)))) {
      Gv TK = (T + Gv.F(273.15));
      Gv k1 = (Gv.F(790000000000.0) * Gd.exp((Gv.F(-11858.0) / TK)));
      Gv k2 = (Gv.F(4100000000000.0) * Gd.exp((Gv.F(-12994.0) / TK)));
      Gv hn = S_physics.chain(v.G("alpha"), v.G("iso"), k1, k2, m);
      v = v.S("alpha", hn.I(Gv.I(0L)));
      v = v.S("iso", hn.I(Gv.I(1L)));
    }
    return Gv.Null;
  }
  public Gv thick_factor(Gv v = default(Gv)) {
    return (Gd.clampf(((((sol(v) + v.G("sg"))) / Gd.maxf(Gv.F(1.0), vol(v))) * Gv.F(6.0)), Gv.F(0.0), Gv.F(1.0))).AsF();
    return Gv.Null;
  }
  public Gv film_hA(Gv v = default(Gv)) {
    Gv hz = (((v.G("stir")).T() ? v.G("hz") : Gv.F(4.0))).AsF();
    Gv visc = ((Gv.F(1.0) / ((Gv.F(1.0) + (S.G("cfg").C("get", Gv.S("mashFilmK"), Gv.F(4.6)) * thick_factor(v)))))).AsF();
    Gv hA = (((HA_REF * Gd.pow((Gd.maxf(hz, Gv.F(4.0)) / Gv.F(35.0)), Gv.F(0.67))) * visc)).AsF();
    if ((v.C("get", Gv.S("boiling"), Gv.B(false))).T()) {
      hA = (Gd.maxf(hA, ((Gv.F(2.0) * HA_REF) * visc))).AsF();
    }
    return (hA).AsF();
    return Gv.Null;
  }
  public Gv film_factor(Gv v = default(Gv)) {
    Gv ua_ref = (S.G("cfg").G("rvpUAst")).AsF();
    Gv gs = ((Gv.F(1.0) / Gd.maxf(Gv.F(1e-06), ((Gv.F(1.0) / ua_ref) - (Gv.F(1.0) / HA_REF))))).AsF();
    Gv ua = ((Gv.F(1.0) / (((Gv.F(1.0) / gs) + (Gv.F(1.0) / film_hA(v)))))).AsF();
    return ((ua / ua_ref)).AsF();
    return Gv.Null;
  }
  public Gv density(Gv v = default(Gv)) {
    Gv liq_mass = ((v.G("w") + ext(v))).AsF();
    Gv P = (((Gv.F(100.0) * ext(v)) / Gd.maxf(Gv.F(1e-09), liq_mass))).AsF();
    Gv sg = ((Gv.F(1.0) + (P / ((Gv.F(258.6) - (Gv.F(0.88) * P)))))).AsF();
    Gv exp_f = ((Gv.F(1.0) + (Gv.F(0.00052) * Gd.maxf(Gv.F(0.0), (v.G("T") - Gv.F(20.0)))))).AsF();
    Gv sol_mass = ((((v.G("g") + v.G("su")) + v.G("sc")) + v.G("ni"))).AsF();
    Gv volume = (((((liq_mass / ((sg * Gv.F(0.9982)))) * exp_f) + (sol_mass * Gv.F(0.65))) + (v.G("sg") * Gv.F(0.0)))).AsF();
    return ((((liq_mass + sol_mass)) / Gd.maxf(Gv.F(1e-09), volume))).AsF();
    return Gv.Null;
  }
  public Gv heat(Gv v = default(Gv), Gv m = default(Gv), Gv dt = default(Gv), Gv name = default(Gv)) {
    m = (m).AsF();
    dt = (dt).AsF();
    Gv c = S.G("cfg");
    Gv pdv = S.G("_pd").C("get", name, Gd.Dict(Gv.S("P"), Gv.F(0.0), Gv.S("wet"), Gv.F(1.0)));
    Gv UA = ((Gv.Eq(name, Gv.S("rvp")) ? c.G("rvpUA") : c.G("mpUA"))).AsF();
    Gv steel = ((Gv.Eq(name, Gv.S("rvp")) ? c.G("rvpSteel") : c.G("mpSteel"))).AsF();
    Gv Pin = (((pdv.G("P") * S.G("boiler").G("scale")) * (((S.G("steamOK").I(name)).T() ? Gv.F(1.0) : Gv.F(0.0))))).AsF();
    Gv thick = thick_factor(v);
    Gv hA = (film_hA(v)).AsF();
    Gv ff = (film_factor(v)).AsF();
    Pin = ((Pin * ff)).AsF();
    v = v.S("Twall", (v.G("T") + (Pin / Gd.maxf(Gv.F(0.5), hA))));
    if (((Gv.Gt(v.G("heat"), Gv.I(0L)) && Gv.Le(pdv.G("wet"), Gv.I(0L))) && (S.G("boiler").G("on")).T())) {
      once((Gv.S("dry%s%d") % Gd.Arr(name, tb(Gv.I(600L)))), (Gv.S("%s: parní plášť není zaplavený – ohřev neběží, napusť víc.") % VN(name)), Gv.S("warn"));
    }
    v = v.S("Pin", Pin);
    if (((Gv.Gt(Pin, Gv.I(0L)) && Gv.Gt((sol(v) + v.G("sg")), Gv.I(40L))) && Gv.Gt(v.G("Twall"), Gv.F(100.0)))) {
      v = v.S("burn", (v.G("burn") + (m * Gd.clampf((((v.G("Twall") - Gv.F(100.0))) / Gv.F(8.0)), Gv.F(0.0), Gv.F(2.0)))));
      if (Gv.Gt(v.G("burn"), Gv.F(1.5))) {
        once((Gv.S("burn") + name), (Gv.S("%s: rmut se připaluje u stěny (%s °C u pláště)! Zapni míchadlo nebo přidej otáčky, ubírej páru.") % Gd.Arr(VN(name), fmt0(v.G("Twall")))), Gv.S("bad"));
      }
      Pin = ((Pin * Gv.F(0.9))).AsF();
    }
    if ((Gv.Gt(v.G("heat"), Gv.I(0L)) && !(S.G("boiler").G("on")).T())) {
      once((Gv.S("noboil%d") % tb(Gv.I(900L))), Gv.S("Kotel je vypnutý – pára nejde."), Gv.S("warn"));
    }
    Gv Tb = ((c.G("tbWater") + (Gv.F(0.03) * plato(v)))).AsF();
    Gv C = ((cC(v) + steelC(steel))).AsF();
    Gv netP = ((Pin - (UA * ((v.G("T") - c.G("ambientT")))))).AsF();
    Gv T = ((v.G("T") + ((netP * dt) / C))).AsF();
    v = v.S("evapKgMin", Gv.F(0.0));
    Gv wasBoil = v.G("boiling");
    if ((Gv.Ge(T, (Tb - Gv.F(0.02))) && Gv.Gt(netP, Gv.I(0L)))) {
      Gv E = (((netP * dt) - (Gd.maxf(Gv.F(0.0), (Tb - v.G("T"))) * C))).AsF();
      T = (Tb).AsF();
      if (Gv.Gt(E, Gv.I(0L))) {
        Gv ev = (Gd.minf(v.G("w"), (E / Gv.F(2257.0)))).AsF();
        v = v.S("w", (v.G("w") - ev));
        v = v.S("evapKgMin", (ev / m));
        S = S.S("evapTot", (S.C("get", Gv.S("evapTot"), Gv.F(0.0)) + ev));
      }
      v = v.S("boiling", Gv.B(true));
      v = v.S("boilMin", (v.G("boilMin") + m));
    } else {
      v = v.S("boiling", Gv.B((Gv.Ge(T, (Tb - Gv.F(0.3))) && Gv.Gt(Pin, Gv.I(0L)))));
    }
    if ((!(wasBoil).T() && (v.G("boiling")).T())) {
      v = v.S("boilStartT", S.G("t"));
    }
    v = v.S("T", T);
    if (Gv.Lt(vol(v), Gv.F(5.0))) {
      v = v.S("Tsens", T);
    } else {
      v = v.S("Tsens", (v.G("Tsens") + ((((T + ((Gv.Eq(name, Gv.S("rvp")) ? S.C("get", Gv.S("sensOff"), Gv.F(0.0)) : Gv.F(0.0)))) - v.G("Tsens"))) * ((Gv.F(1.0) - Gd.exp(((-dt) / ((Gv.F(20.0) + (Gv.F(70.0) * ((Gv.F(1.0) - stirQ(v)))))))))))));
    }
    return Gv.Null;
  }
  public Gv drawHot(Gv L = default(Gv)) {
    L = (L).AsF();
    Gv h = S.G("hlt");
    Gv usable = Gd.maxf(Gv.F(0.0), (h.G("w") - Gv.F(150.0)));
    Gv hot = Gd.minf(L, usable);
    if (Gv.Lt(hot, L)) {
      once((Gv.S("hltempty%d") % tb(Gv.I(1200L))), Gv.S("Bojler je prázdný – horká voda neteče!"), Gv.S("bad"));
    }
    h = h.S("w", (h.G("w") - hot));
    S = S.S("usedHot", (S.G("usedHot") + hot));
    return ((Gv.Gt(hot, Gv.F(0.01)) ? h.G("T") : S.G("cool").G("waterT"))).AsF();
    return Gv.Null;
  }
  public Gv drawWater(Gv L = default(Gv), Gv Tset = default(Gv)) {
    L = (L).AsF();
    Tset = (Tset).AsF();
    Gv cold = (S.G("cool").G("waterT")).AsF();
    Gv h = S.G("hlt");
    Gv usable = Gd.maxf(Gv.F(0.0), (h.G("w") - Gv.F(150.0)));
    Gv hot = Gv.F(0.0);
    Gv T = cold;
    if (Gv.Gt(Tset, (cold + Gv.F(0.5)))) {
      if (Gv.Gt(h.G("T"), Tset)) {
        hot = ((L * ((Tset - cold))) / ((h.G("T") - cold)));
      } else {
        hot = L;
        if (Gv.Lt(h.G("T"), (Tset - Gv.I(1L)))) {
          once((Gv.S("hltcold%d") % tb(Gv.I(1200L))), (Gv.S("Horká voda má jen %s °C – požadovaných %s °C nedáš.") % Gd.Arr(fmt0(h.G("T")), fmt0(Tset))), Gv.S("warn"));
        }
      }
      if (Gv.Gt(hot, usable)) {
        hot = usable;
        once((Gv.S("hltempty%d") % tb(Gv.I(1200L))), Gv.S("Tank horké vody je prázdný – teče jen studená voda!"), Gv.S("bad"));
      }
      T = (Gv.Gt(L, Gv.I(0L)) ? ((((hot * h.G("T")) + (((L - hot)) * cold))) / L) : cold);
    }
    h = h.S("w", (h.G("w") - hot));
    S = S.S("usedHot", (S.G("usedHot") + hot));
    return (T).AsF();
    return Gv.Null;
  }
  public Gv srcTake(Gv key = default(Gv), Gv Lmin = default(Gv), Gv m = default(Gv), Gv pump = default(Gv)) {
    Lmin = (Lmin).AsF();
    m = (m).AsF();
    if (pump.k == 0) pump = Gv.S("");
    Gv c = S.G("cfg");
    Gv L = (Lmin * m);
    if ((Gv.Eq(key, Gv.S("rvp")) || Gv.Eq(key, Gv.S("mp")))) {
      Gv v = S.I(key);
      Gv dead = ((Gv.Eq(key, Gv.S("rvp")) ? c.G("rvpDead") : (c.G("mpDead") + (v.G("trubSet") * Gv.F(6.0))))).AsF();
      Gv LL = Gd.minf(L, Gd.maxf(Gv.F(0.0), (vol(v) - dead)));
      if (Gv.Le(LL, Gv.F(0.01))) {
        return Gv.Null;
      }
      return takePart(v, LL, ((v.G("stir")).T() ? Gv.F(1.0) : Gv.F(1.5)));
    }
    if (Gv.Eq(key, Gv.S("wp"))) {
      Gv w = S.G("wp");
      Gv LL2 = Gd.minf(L, Gd.maxf(Gv.F(0.0), ((vol(w) - c.G("wpDead")) - (w.G("trubSet") * Gv.F(6.0)))));
      if (Gv.Le(LL2, Gv.F(0.01))) {
        return Gv.Null;
      }
      return takePart(w, LL2, Gv.F(1.0));
    }
    if (((Gv.Eq(key, Gv.S("sk")) && Gv.Gt(vol(S.G("sk").G("mash")), Gv.I(5L))) && Gv.Ne(pump, Gv.S("p2")))) {
      Gv LL3 = Gd.minf(L, (vol(S.G("sk").G("mash")) - Gv.F(5.0)));
      if (Gv.Le(LL3, Gv.F(0.01))) {
        return Gv.Null;
      }
      return takePart(S.G("sk").G("mash"), LL3, Gv.F(1.0));
    }
    if (Gv.Eq(key, Gv.S("sk"))) {
      if ((Gv.Eq(pump, Gv.S("p2")) || Gv.Eq(pump, Gv.S("")))) {
        skMashToBed();
      }
      Gv tot = Gv.F(0.0);
      foreach (Gv x in (S.G("sk").G("cells")).Iter()) {
        tot = (tot + cellVol(x));
      }
      if (Gv.Lt(tot, Gv.F(0.5))) {
        return Gv.Null;
      }
      return skDraw(Lmin, m);
    }
    if (Gv.Eq(key, Gv.S("hlt"))) {
      Gv h = S.G("hlt");
      Gv LL4 = Gd.minf(L, Gd.maxf(Gv.F(0.0), (h.G("w") - Gv.F(150.0))));
      if (Gv.Le(LL4, Gv.F(0.01))) {
        return Gv.Null;
      }
      h = h.S("w", (h.G("w") - LL4));
      Gv p = lp();
      p = p.S("w", LL4);
      p = p.S("T", h.G("T"));
      return p;
    }
    if (Gv.Eq(key, Gv.S("spilka"))) {
      Gv sp = S.G("sp");
      if (Gv.Lt(sp.G("L"), Gv.I(5L))) {
        return Gv.Null;
      }
      Gv LL5 = Gd.minf(L, sp.G("L"));
      Gv fr = ((LL5 / sp.G("L"))).AsF();
      Gv q = lp();
      q = q.S("w", (sp.G("w") * fr));
      q = q.S("f", (sp.G("f") * fr));
      q = q.S("ns", (sp.G("n") * fr));
      q = q.S("iso", (sp.G("iso") * fr));
      q = q.S("dms", (sp.G("dms") * fr));
      q = q.S("col", (sp.G("col") * fr));
      q = q.S("T", (sp.G("H") / sp.G("L")));
      foreach (Gv k in (Gd.Arr(Gv.S("w"), Gv.S("f"), Gv.S("n"), Gv.S("iso"), Gv.S("alpha"), Gv.S("dms"), Gv.S("trub"), Gv.S("col"), Gv.S("H"), Gv.S("L"), Gv.S("O2"))).Iter()) {
        sp = sp.SI(k, (sp.I(k) * ((Gv.F(1.0) - fr))));
      }
      return q;
    }
    if (Gv.Eq(key, Gv.S("lv"))) {
      if (Gv.Lt(S.G("lv").G("w"), Gv.I(300L))) {
        return Gv.Null;
      }
      Gv p2 = lp();
      p2 = p2.S("w", L);
      p2 = p2.S("T", S.G("lv").G("T"));
      p2 = p2.S("virt", Gv.B(true));
      return p2;
    }
    return Gv.Null;
    return Gv.Null;
  }
  public Gv spAdd(Gv p = default(Gv)) {
    Gv sp = S.G("sp");
    Gv Tout = (p.G("T")).AsF();
    S = S.S("cool", S.G("cool").S("Tout", Tout));
    sp = sp.S("w", (sp.G("w") + p.G("w")));
    sp = sp.S("f", (sp.G("f") + p.G("f")));
    sp = sp.S("n", (sp.G("n") + (((p.G("sg") + p.G("dx")) + p.G("ld")) + p.G("ns"))));
    sp = sp.S("iso", (sp.G("iso") + p.G("iso")));
    sp = sp.S("alpha", (sp.G("alpha") + p.G("alpha")));
    sp = sp.S("dms", (sp.G("dms") + p.G("dms")));
    sp = sp.S("trub", (sp.G("trub") + p.G("trubSus")));
    sp = sp.S("col", (sp.G("col") + p.G("col")));
    Gv Lp = liqVol(p);
    sp = sp.S("H", (sp.G("H") + (Lp * Tout)));
    sp = sp.S("L", (sp.G("L") + Lp));
    sp = sp.S("O2", (sp.G("O2") + (Lp * (((S.G("aer")).T() ? Gv.F(8.0) : Gv.F(0.5))))));
    if (Gv.Gt(sp.G("L"), S.G("cfg").G("spCap"))) {
      Gv o = ((sp.G("L") - S.G("cfg").G("spCap"))).AsF();
      Gv fr = ((o / sp.G("L"))).AsF();
      S = S.S("drainExt", (S.G("drainExt") + (((sp.G("f") + sp.G("n"))) * fr)));
      foreach (Gv k in (Gd.Arr(Gv.S("w"), Gv.S("f"), Gv.S("n"), Gv.S("iso"), Gv.S("alpha"), Gv.S("dms"), Gv.S("trub"), Gv.S("col"), Gv.S("H"), Gv.S("L"), Gv.S("O2"))).Iter()) {
        sp = sp.SI(k, (sp.I(k) * ((Gv.F(1.0) - fr))));
      }
      S = S.S("drainL", (S.G("drainL") + o));
      once((Gv.S("spover%d") % tb(Gv.I(600L))), Gv.S("Spilka přetéká!"), Gv.S("bad"));
    }
    if (Gv.Gt(ext(p), Gv.F(0.01))) {
      sp = sp.S("Tmax", Gd.maxf(sp.G("Tmax"), Tout));
      sp = sp.S("Tmin", Gd.minf(sp.G("Tmin"), Tout));
      if (Gv.Gt(Tout, Gv.I(14L))) {
        once((Gv.S("hotcast%d") % tb(Gv.I(600L))), (Gv.S("Mladina jde do spilky při %s °C – pro spodní kvasnice moc teplá. Přidej ledovou vodu nebo zpomal.") % fmt1(Tout)), Gv.S("warn"));
      }
      if (Gv.Lt(Tout, Gv.I(4L))) {
        once((Gv.S("coldcast%d") % tb(Gv.I(600L))), (Gv.S("Mladina jde do spilky při %s °C – příliš studená, uber ledovou.") % fmt1(Tout)), Gv.S("warn"));
      }
      if (!(S.G("once").C("has", Gv.S("castStart"))).T()) {
        S = S.S("once", S.G("once").S("castStart", Gv.I(1L)));
        S = S.S("flags", S.G("flags").S("castStart", S.G("t")));
        logm((Gv.S("Do spilky teče mladina, %s °C.") % fmt1(Tout)));
      }
    } else if ((Gv.Gt(p.G("w"), Gv.F(0.5)) && Gv.Gt(sp.G("L"), Gv.I(100L)))) {
      once((Gv.S("spwater%d") % tb(Gv.I(300L))), Gv.S("Do spilky teče voda."));
    }
    return Gv.Null;
  }
  public Gv dstGive(Gv key = default(Gv), Gv p = default(Gv), Gv ctx = default(Gv)) {
    if (ctx.k == 0) ctx = Gd.Dict();
    Gv c = S.G("cfg");
    Gv sk = S.G("sk");
    Gv mp = S.G("mp");
    Gv Lp = liqVol(p);
    if ((Gv.Le(Lp, Gv.F(1e-06)) && Gv.Le(sol(p), Gv.F(1e-06)))) {
      return Gv.Null;
    }
    if ((Gv.Eq(key, Gv.S("rvp")) || Gv.Eq(key, Gv.S("mp")))) {
      addTo(S.I(key), p, (Gv.Eq(key, Gv.S("rvp")) ? c.G("rvpSteel") : c.G("mpSteel")));
      S = S.SI(key, S.I(key).S("trubSus", (S.I(key).G("trubSus") + S.I(key).G("fines"))));
      S = S.SI(key, S.I(key).S("fines", Gv.F(0.0)));
      S = S.SI(key, S.I(key).S("air", Gv.F(0.0)));
      if ((Gv.Eq(key, Gv.S("mp")) && Gv.Eq(ctx.C("get", Gv.S("src")), Gv.S("mp")))) {
        mp = mp.S("whirlNow", Gv.B(true));
        mp = mp.S("whirlMin", (mp.G("whirlMin") + ctx.C("get", Gv.S("m"), Gv.F(0.0))));
        mp = mp.S("settleMin", Gv.F(0.0));
      }
      if (Gv.Eq(ctx.C("get", Gv.S("src")), Gv.S("sk"))) {
        Gv v = S.I(key);
        Gv tu = (ctx.C("get", Gv.S("turb"), Gv.F(0.0))).AsF();
        v = v.S("turbSum", (v.G("turbSum") + (tu * Lp)));
        v = v.S("turbVol", (v.G("turbVol") + Lp));
        S = S.S("turbSum", (S.G("turbSum") + (tu * Lp)));
        S = S.S("turbVol", (S.G("turbVol") + Lp));
        if (((Gv.Lt(sk.G("outP"), Gv.F(0.8)) && Gv.Gt(sk.G("g"), Gv.I(1L))) && Gv.Gt(v.G("turbVol"), Gv.I(2000L)))) {
          once(Gv.S("lastrun"), Gv.S("Poslední podíl pod 0,8 °P – dál už jen ředíš a vyluhuješ třísloviny."), Gv.S("warn"));
        }
      }
      return Gv.Null;
    }
    if (Gv.Eq(key, Gv.S("wp"))) {
      if (((Gv.Eq(S.G("flags").G("knock"), Gv.Null) && Gv.Gt(ext(p), Gv.F(0.002))) && Gv.Gt(ext(S.G("rvp")), Gv.I(50L)))) {
        Gv r = S.G("rvp");
        Gv e = (ext(r) + ext(p));
        Gv w = ((r.G("w") + p.G("w"))).AsF();
        S = S.S("flags", S.G("flags").S("knock", Gd.Dict(Gv.S("V"), (hotV(r) + hotV(p)), Gv.S("P"), ((Gv.F(100.0) * e) / Gd.maxf(Gv.F(1.0), (e + w))), Gv.S("e"), e, Gv.S("t"), S.G("t"))));
      }
      addTo(S.G("wp"), p, c.G("wpSteel"));
      S = S.S("wp", S.G("wp").S("whirlNow", Gv.B(true)));
      S = S.S("wp", S.G("wp").S("whirlMin", (S.G("wp").G("whirlMin") + ctx.C("get", Gv.S("m"), Gv.F(0.0)))));
      S = S.S("wp", S.G("wp").S("settleMin", Gv.F(0.0)));
      return Gv.Null;
    }
    if (Gv.Eq(key, Gv.S("sk"))) {
      if ((Gv.Eq(sk.G("mode"), Gv.S("lauter")) && Gv.Eq(ctx.C("get", Gv.S("src"), Gv.S("")), Gv.S("sk")))) {
        sk = sk.S("vorlaufL", (sk.G("vorlaufL") + Lp));
      }
      if (Gv.Gt(p.G("fines"), Gv.I(0L))) {
        sk = sk.S("teig", (sk.C("get", Gv.S("teig"), Gv.F(0.0)) + (p.G("fines") * Gv.F(0.5))));
        sk = sk.S("g", (sk.G("g") + (p.G("fines") * Gv.F(0.4))));
        sk = sk.S("fines", (sk.G("fines") + (p.G("fines") * Gv.F(0.1))));
        p = p.S("fines", Gv.F(0.0));
      }
      if (Gv.Gt((((p.G("g") + p.G("su")) + p.G("sc")) + p.G("ni")), Gv.F(0.01))) {
        if (!(sk.G("landed")).T()) {
          sk = sk.S("landed", Gv.B(true));
          sk = sk.S("underOK", Gv.B(Gv.Ge(sk.G("under"), Gv.I(120L))));
          if (!(sk.G("underOK")).T()) {
            once(Gv.S("underno"), Gv.S("Podsítí nebylo zaplavené – rmut dosedá na suchá síta, lože se utáhne a scezování půjde hůř."), Gv.S("warn"));
          }
        }
        if (Gv.Lt(sk.G("g"), Gv.I(1L))) {
          if (Gv.Lt(vol(sk.G("mash")), Gv.I(1L))) {
            sk = sk.S("mash", sk.G("mash").S("T", sk.G("Tw")));
          }
          addTo(sk.G("mash"), p, Gv.F(0.0));
        } else {
          skAddMash(p);
        }
        sk = sk.S("filling", Gv.B(true));
        S = S.S("skFillNow", Gv.B(true));
      } else if (Gv.Gt(vol(sk.G("mash")), Gv.I(1L))) {
        addTo(sk.G("mash"), p, Gv.F(0.0));
      } else {
        Gv C0 = skC();
        Gv Cp = cC(p);
        sk = sk.S("T", ((((C0 * sk.G("T")) + (Cp * p.G("T")))) / ((C0 + Cp))));
        Gv t = sk.G("cells").I(Gv.I(0L));
        t = t.S("w", (t.G("w") + p.G("w")));
        t = t.S("f", (t.G("f") + p.G("f")));
        t = t.S("n", (t.G("n") + (((p.G("sg") + p.G("dx")) + p.G("ld")) + p.G("ns"))));
        t = t.S("smm", (t.G("smm") + p.G("smm")));
        t = t.S("dms", (t.G("dms") + p.G("dms")));
        t = t.S("col", (t.G("col") + p.G("col")));
      }
      return Gv.Null;
    }
    if (Gv.Eq(key, Gv.S("spilka"))) {
      spAdd(p);
      return Gv.Null;
    }
    if (Gv.Eq(key, Gv.S("hlt"))) {
      Gv h = S.G("hlt");
      Gv received = Gd.minf(p.G("w"), Gd.maxf(Gv.F(0.0), (c.G("hltCap") - h.G("w"))));
      if (Gv.Gt(received, Gv.I(0L))) {
        h = h.S("T", ((((h.G("w") * h.G("T")) + (received * p.G("T")))) / ((h.G("w") + received))));
        h = h.S("w", (h.G("w") + received));
      }
      if (Gv.Gt(p.G("w"), received)) {
        S = S.S("drainL", (S.G("drainL") + (p.G("w") - received)));
        once((Gv.S("hltfull%d") % tb(Gv.I(1200L))), Gv.S("Tank horké vody je plný – přepad do kanálu."), Gv.S("warn"));
      }
      return Gv.Null;
    }
    if (Gv.Eq(key, Gv.S("lv"))) {
      Gv lv = S.G("lv");
      if ((p.C("get", Gv.S("virt"), Gv.B(false))).T()) {
        return Gv.Null;
      }
      Gv L = (p.G("w")).AsF();
      lv = lv.S("T", (Gv.Lt(lv.G("w"), Gv.I(1L)) ? p.G("T") : ((((lv.G("w") * lv.G("T")) + (L * p.G("T")))) / ((lv.G("w") + L)))));
      lv = lv.S("w", (lv.G("w") + L));
      if (!(S.G("lvFillLog")).T()) {
        S = S.S("lvFillLog", Gv.B(true));
        logm((Gv.S("Napouštím ledovku vodou z řadu %s °C – až naběhne hladina, pusť agregát (okruh) a nech ji nachladit.") % fmt0(p.G("T"))));
      }
      if (Gv.Gt(lv.G("w"), c.G("lvVol"))) {
        Gv o = ((lv.G("w") - c.G("lvVol"))).AsF();
        lv = lv.S("w", c.G("lvVol"));
        S = S.S("drainL", (S.G("drainL") + o));
        once((Gv.S("lvover%d") % tb(Gv.I(600L))), Gv.S("Ledovka je plná a přetéká přepadem – zavři napouštění."), Gv.S("warn"));
      }
      return Gv.Null;
    }
    if (Gv.Eq(key, Gv.S("tanky"))) {
      return Gv.Null;
    }
    S = S.S("drainL", (S.G("drainL") + Lp));
    S = S.S("drainExt", (S.G("drainExt") + (((ext(p) + p.G("su")) + p.G("sc")) + p.G("ni"))));
    return Gv.Null;
  }
  public Gv cab_live() {
    Gv c = S.G("cab");
    return Gv.B((((c.G("main")).T() && (c.G("key")).T()) && !(c.G("estop")).T()));
    return Gv.Null;
  }
  public Gv cab_gate() {
    Gv c = S.G("cab");
    Gv rv = S.G("rvp");
    if (!(cab_live()).T()) {
      if (!(c.G("dead")).T()) {
        c = c.S("dead", Gv.B(true));
        logm(Gv.S("Rozvaděč bez napětí – všechno elektrické stojí (čerpadla, míchadlo, kopačka, ohřev)."), Gv.S("bad"));
      }
      S = S.S("p1", S.G("p1").S("on", Gv.B(false)));
      S = S.S("p2", S.G("p2").S("on", Gv.B(false)));
      S = S.S("p3", S.G("p3").S("on", Gv.B(false)));
      S = S.S("lv", S.G("lv").S("pump", Gv.B(false)));
      rv = rv.S("stir", Gv.B(false));
      S = S.S("mp", S.G("mp").S("stir", Gv.B(false)));
      S = S.S("sk", S.G("sk").S("rake", Gv.B(false)));
      S = S.S("sk", S.G("sk").S("hyd", Gv.F(0.0)));
      S = S.S("rvpWater", S.G("rvpWater").S("on", Gv.B(false)));
      S = S.S("skWater", S.G("skWater").S("on", Gv.B(false)));
      S = S.S("malt", S.G("malt").S("on", Gv.B(false)));
      if (Gv.Gt(rv.G("heat"), Gv.I(0L))) {
        c = c.S("hs", rv.G("heat"));
        rv = rv.S("heat", Gv.F(0.0));
      }
      return Gv.Null;
    }
    if ((c.G("dead")).T()) {
      c = c.S("dead", Gv.B(false));
      logm(Gv.S("Rozvaděč pod napětím."));
      if (Gv.Gt(c.G("hs"), Gv.I(0L))) {
        rv = rv.S("heat", c.G("hs"));
        c = c.S("hs", Gv.F(0.0));
      }
    }
    return Gv.Null;
  }
  public Gv sim_step(Gv dt = default(Gv)) {
    dt = (dt).AsF();
    cab_gate();
    Gv spd = S.G("sp");
    if (((spd.C("get", Gv.S("lidOpen"), Gv.B(false))).T() || !(spd.C("has", Gv.S("lidL0"))).T())) {
      spd = spd.SI(Gv.S("lidL0"), spd.G("L"));
      spd = spd.SI(Gv.S("press"), Gv.F(0.0));
    } else {
      spd = spd.SI(Gv.S("lidL0"), (spd.G("lidL0") + (((spd.G("L") - spd.G("lidL0"))) * ((Gv.F(1.0) - Gd.exp(((-dt) / Gv.F(900.0))))))));
      spd = spd.SI(Gv.S("press"), ((Gv.F(1013.0) * Gd.maxf(Gv.F(0.0), (spd.G("L") - spd.G("lidL0")))) / Gd.maxf(Gv.F(50.0), (S.G("cfg").G("spCap") - spd.G("L")))));
    }
    Gv c = S.G("cfg");
    Gv m = (dt / Gv.F(60.0));
    Gv rvp = S.G("rvp");
    Gv mp = S.G("mp");
    Gv sk = S.G("sk");
    Gv h = S.G("hlt");
    Gv lv = S.G("lv");
    if ((((S.G("cab").C("get", Gv.S("mpAuto"), Gv.B(false))).T() && (cab_live()).T()) && !(main.G("auto").G("on")).T())) {
      rvp = rvp.S("heat", ((S.G("cab").C("get", Gv.S("mpHeatOn"), Gv.B(false))).T() ? Gd.clampf((((S.G("cab").C("get", Gv.S("mpSet"), Gv.F(50.0)) - rvp.G("Tsens"))) / Gv.F(8.0)), Gv.F(0.0), Gv.F(1.0)) : Gv.F(0.0)));
    }
    if ((h.G("auto")).T()) {
      h = h.S("heat", (Gv.Lt(h.G("T"), (h.G("set") - Gv.F(0.5))) ? Gv.F(1.0) : ((Gv.Gt(h.G("T"), (h.G("set") + Gv.F(0.2))) ? Gv.F(0.0) : h.G("heat")))));
    }
    Gv pb = (Gd.Dyn("float", S.G("boiler").C("get", Gv.S("p"), c.G("steamBar")))).AsF();
    Gv Tst = (((S.G("boiler").G("on")).T() || Gv.Gt(pb, Gv.F(0.05))) ? tsat((pb + Gv.F(1.013))) : Gv.F(20.0));
    S = S.S("Tsteam", Tst);
    S = S.S("_pd", Gd.Dict(Gv.S("rvp"), _pd(rvp, c.G("rvpUAst"), c.G("rvpJacketL"), Tst), Gv.S("mp"), _pd(mp, c.G("mpUAst"), c.G("mpJacketL"), Tst), Gv.S("hlt"), Gd.Dict(Gv.S("P"), ((c.G("hltUAst") * h.G("heat")) * Gd.maxf(Gv.F(0.0), (Tst - h.G("T")))), Gv.S("wet"), Gv.F(1.0))));
    if ((!(S.G("boiler").G("on")).T() && Gv.Le(pb, Gv.F(0.05)))) {
      foreach (Gv k_ in (Gd.Arr(Gv.S("rvp"), Gv.S("mp"), Gv.S("hlt"))).Iter()) {
        S = S.S("_pd", S.G("_pd").SI(k_, S.G("_pd").I(k_).S("P", Gv.F(0.0))));
      }
    }
    Gv dem = (((S.G("_pd").G("rvp").G("P") + S.G("_pd").G("mp").G("P")) + S.G("_pd").G("hlt").G("P"))).AsF();
    Gv Qb = (Gv.F(0.0)).AsF();
    if ((S.G("boiler").G("on")).T()) {
      Qb = ((Gv.Lt(pb, (c.G("steamBar") - Gv.F(0.02))) ? c.G("boilerKW") : Gd.minf(c.G("boilerKW"), dem))).AsF();
    }
    Gv Cst = ((Gd.maxf(Gv.F(50.0), c.C("get", Gv.S("boilerWaterKg"), Gv.F(1500.0))) * Gv.F(57.0))).AsF();
    pb = (Gd.clampf((pb + ((((Qb - dem)) * dt) / Cst)), Gv.F(0.0), c.G("steamBar"))).AsF();
    S = S.S("boiler", S.G("boiler").S("p", pb));
    S = S.S("boiler", S.G("boiler").S("scale", Gv.F(1.0)));
    S = S.S("boiler", S.G("boiler").S("hltScale", Gv.F(1.0)));
    S = S.S("boiler", S.G("boiler").S("load", (Qb / c.G("boilerKW"))));
    if ((Gv.Gt(dem, (c.G("boilerKW") * Gv.F(1.02))) && (S.G("boiler").G("on")).T())) {
      once((Gv.S("boilmax%d") % tb(Gv.I(1800L))), (Gv.S("Kotel nestačí: odběr %d kW, kotel dá max %d kW – tlak páry klesá (%s bar), ohřev všude zpomalí. Neohřívej pánev a bojler naráz naplno.") % Gd.Arr(Gd.roundi(dem), Gd.roundi(c.G("boilerKW")), fmt1(pb))), Gv.S("warn"));
    }
    if (((S.G("boiler").G("on")).T() && Gv.Lt(pb, (c.G("steamBar") - Gv.F(1.0))))) {
      once((Gv.S("lowp%d") % tb(Gv.I(900L))), (Gv.S("Tlak páry spadl na %s bar (nastaveno %s) – pára má jen %s °C, var bude slabý (DMS!). Uber jiné odběry a počkej, až kotel natlakuje.") % Gd.Arr(fmt1(pb), fmt1(c.G("steamBar")), fmt0(Tst))), Gv.S("bad"));
    }
    Gv Pin = (((S.G("_pd").G("hlt").G("P") * S.G("boiler").G("hltScale")) * (((S.G("steamOK").G("hlt")).T() ? Gv.F(1.0) : Gv.F(0.0))))).AsF();
    Gv Ch = (((Gv.F(4.19) * Gd.maxf(Gv.F(50.0), h.G("w"))) + steelC(c.G("hltSteel")))).AsF();
    h = h.S("T", (h.G("T") + ((((Pin - (c.G("hltUA") * ((h.G("T") - c.G("ambientT")))))) * dt) / Ch)));
    if (Gv.Gt(h.G("T"), c.G("tbWater"))) {
      Gv ev = (Gd.minf(h.G("w"), ((((h.G("T") - c.G("tbWater"))) * Ch) / Gv.F(2257.0)))).AsF();
      h = h.S("w", (h.G("w") - ev));
      S = S.S("evapTot", (S.C("get", Gv.S("evapTot"), Gv.F(0.0)) + ev));
      h = h.S("T", c.G("tbWater"));
      once((Gv.S("hltboil%d") % tb(Gv.I(1800L))), (Gv.S("Horká voda v bojleru vře – pára uniká odvětráním (%s L/h). Ubírej páru.") % fmt0(((ev / Gd.maxf(Gv.F(1e-06), dt)) * Gv.F(3600.0)))), Gv.S("warn"));
    }
    Gv Cl = ((Gd.maxf(Gv.F(200.0), lv.G("w")) * Gv.F(4.19))).AsF();
    Gv Q = (((Gv.F(0.15) * ((c.G("ambientT") - lv.G("T")))) * Gd.maxf(Gv.F(0.2), (lv.G("w") / c.G("lvVol"))))).AsF();
    if (((lv.G("pump")).T() && (lv.G("piv")).T())) {
      Q = ((Q + c.G("pivLoad"))).AsF();
    }
    if ((lv.G("chill")).T()) {
      if (Gv.Gt(lv.G("T"), (lv.G("set") + Gv.F(0.5)))) {
        lv = lv.S("chillOn", Gv.B(true));
      } else if (Gv.Lt(lv.G("T"), (lv.G("set") - Gv.F(0.3)))) {
        lv = lv.S("chillOn", Gv.B(false));
      }
    } else {
      lv = lv.S("chillOn", Gv.B(false));
    }
    if (((lv.G("chill")).T() && Gv.Eq(lv.G("glyk"), Gv.B(false)))) {
      lv = lv.S("chillOn", Gv.B(false));
      if (Gv.Ge(lv.G("w"), (Gv.F(0.52) * c.G("lvVol")))) {
        once((Gv.S("noglyk%d") % tb(Gv.I(900L))), (Gv.S("Ledovka se nechladí: %s. Voda v ní má %s °C – na %s °C ji dostane jen okruh přes glykol.") % Gd.Arr((Gv.Ne(lv.G("glykWhy"), Gv.S("")) ? lv.G("glykWhy") : Gv.S("glykolový okruh není průchozí")), fmt1(lv.G("T")), fmt0(lv.G("set")))), Gv.S("bad"));
      }
    }
    if (((lv.G("chillOn")).T() && Gv.Lt(lv.G("w"), (Gv.F(0.35) * c.G("lvVol"))))) {
      lv = lv.S("chillOn", Gv.B(false));
      once((Gv.S("lvlow%d") % tb(Gv.I(900L))), (Gv.S("Ledovka má jen %d hl – výparník agregátu není pod vodou, agregát stojí. Dopusť vodu.") % Gd.roundi((lv.G("w") / Gv.F(100.0)))), Gv.S("bad"));
    }
    if ((lv.G("chillOn")).T()) {
      Q = ((Q - c.G("chillerKW"))).AsF();
    }
    lv = lv.S("T", Gd.maxf(Gv.F(-0.5), (lv.G("T") + ((Q * dt) / Cl))));
    if (Gv.Le(lv.G("T"), (lv.G("set") + Gv.F(0.5)))) {
      lv = lv.S("reached", Gv.B(true));
    }
    if (((Gv.Ge(lv.G("w"), (Gv.F(0.35) * c.G("lvVol"))) && Gv.Gt(lv.G("T"), (lv.G("set") + Gv.I(3L)))) && !(lv.G("chill")).T())) {
      once((Gv.S("lvwarm0%d") % tb(Gv.I(1800L))), (Gv.S("Ledovka má %s °C (voda z řadu) – pusť agregát (okruh), ať ji nachladí na %s °C.") % Gd.Arr(fmt1(lv.G("T")), fmt0(lv.G("set")))), Gv.S("warn"));
    } else if ((((lv.G("chillOn")).T() && Gv.Gt(lv.G("T"), Gv.I(5L))) && (lv.G("reached")).T())) {
      once((Gv.S("lvwarm%d") % tb(Gv.I(1800L))), (Gv.S("Ledová voda má %s °C – agregát nestíhá.") % fmt1(lv.G("T"))), Gv.S("warn"));
    }
    if ((S.G("malt").G("on")).T()) {
      if (Gv.Lt(rvp.G("w"), Gv.I(200L))) {
        S = S.S("malt", S.G("malt").S("on", Gv.B(false)));
        logm(Gv.S("Nejdřív napusť do pánve vodu, pak teprve sypej slad."), Gv.S("warn"));
      } else if (!(S.G("malt").C("get", Gv.S("gate"), Gv.B(false))).T()) {
        once((Gv.S("maltgate%d") % tb(Gv.I(120L))), Gv.S("Šoupě násypky je zavřené – slad nepadá do pánve. Otevři šoupě (klepni na něj u násypky nebo tlačítko vpravo)."), Gv.S("warn"));
      } else {
        Gv kg = (Gd.minf((c.G("maltRate") * m), (S.G("malt").G("target") - S.G("malt").G("added")))).AsF();
        if (Gv.Gt(kg, Gv.I(0L))) {
          Gv ratio = ((rvp.G("w") / Gd.maxf(Gv.F(1.0), (S.G("malt").G("added") + kg)))).AsF();
          Gv cl = (Gv.F(0.15) - (Gv.F(0.14) * stirQ(rvp)));
          if (Gv.Lt(ratio, Gv.F(2.5))) {
            cl = (cl + Gv.F(0.05));
          }
          if (Gv.Lt(stirQ(rvp), Gv.F(0.3))) {
            once(Gv.S("clump"), Gv.S("Vystíráš bez míchadla – tvoří se hrudky, část škrobu zůstane nezcukřená."), Gv.S("warn"));
          }
          Gv coarse = (Gd.Arr(Gv.F(0.03), Gv.F(0.08), Gv.F(0.16)).I(Gd.clampi((Gd.roundi(c.G("mill")) - Gv.I(1L)), Gv.I(0L), Gv.I(2L)))).AsF();
          Gv p = lp();
          p = p.S("T", Gv.F(18.0));
          Gv gt = Gv.F(0.0);
          foreach (Gv id in (S.G("grist")).Iter()) {
            gt = (gt + S.G("grist").I(id));
          }
          gt = Gd.maxf(gt, Gv.F(1.0));
          foreach (Gv id_2 in (S.G("grist")).Iter()) {
            Gv M = MALTS.C("get", id_2, MALTS.G("plz"));
            Gv k = (((kg * S.G("grist").I(id_2)) / gt)).AsF();
            Gv dry = ((k * ((Gv.F(1.0) - (M.C("get", Gv.S("moist"), c.G("maltMoist")) / Gv.F(100.0)))))).AsF();
            Gv pot = (((dry * M.G("ext")) / Gv.F(100.0))).AsF();
            Gv unf = ((pot * M.G("unf"))).AsF();
            Gv st = (((pot - unf)) * Gv.F(0.78));
            Gv ni = (((pot - unf)) * Gv.F(0.22));
            S = S.S("extIn", (S.G("extIn") + pot));
            p = p.S("w", (p.G("w") + (k - dry)));
            p = p.S("g", (p.G("g") + (dry - pot)));
            p = p.S("ld", (p.G("ld") + unf));
            p = p.S("su", (p.G("su") + (st * (((Gv.F(1.0) - cl) - coarse)))));
            p = p.S("sc", (p.G("sc") + (st * ((cl + coarse)))));
            p = p.S("ni", (p.G("ni") + ni));
            p = p.S("aA", (p.G("aA") + (k * M.G("enz"))));
            p = p.S("aB", (p.G("aB") + ((k * M.G("enz")) * Gv.F(0.87))));
            p = p.S("aBs", (p.G("aBs") + ((k * M.G("enz")) * Gv.F(0.13))));
            p = p.S("smm", (p.G("smm") + ((k * Gv.F(0.0027)) * ((Gv.Gt(M.G("enz"), Gv.I(0L)) ? Gv.F(1.0) : Gv.F(0.3))))));
            p = p.S("col", (p.G("col") + ((k * M.G("ebc")) * Gv.F(10.0))));
          }
          addTo(rvp, p, c.G("rvpSteel"));
          S = S.S("malt", S.G("malt").S("added", (S.G("malt").G("added") + kg)));
          S = S.S("malt", S.G("malt").S("clump", (S.G("malt").G("clump") + (cl * kg))));
        }
        if (Gv.Ge(S.G("malt").G("added"), (S.G("malt").G("target") - Gv.F(0.01)))) {
          S = S.S("malt", S.G("malt").S("on", Gv.B(false)));
          logm((Gv.S("Vystírání hotovo: %d kg sladu, rmut %s °C.") % Gd.Arr(Gd.roundi(S.G("malt").G("added")), fmt1(rvp.G("T")))));
          if (Gv.Gt(rvp.G("T"), Gv.I(60L))) {
            logm(Gv.S("Vystírací teplota nad 60 °C – beta-amyláza už trpí, rmut bude méně zkvasitelný."), Gv.S("warn"));
          }
        }
      }
    }
    skQmax();
    if ((((S.C("has", Gv.S("rota"))).T() && (S.G("p2").G("on")).T()) && Gv.Gt(S.G("p2").G("q"), Gv.F(1.0)))) {
      S = S.S("rota", S.G("rota").S("stick", Gd.minf(Gv.F(1.5), (S.G("rota").G("stick") + (dt / Gv.F(900.0))))));
    }
    sk = sk.S("rakeH", Gd.clampf((sk.G("rakeH") + ((sk.G("hyd") * m) / Gv.F(0.25))), Gv.F(0.0), Gv.F(1.0)));
    if (Gv.Gt(sk.G("dig"), Gv.I(0L))) {
      sk = sk.S("dig", Gd.maxf(Gv.F(0.0), (sk.G("dig") - dt)));
      sk = sk.S("rakeH", Gd.minf(Gv.F(1.0), (sk.G("rakeH") + (dt / Gv.F(8.0)))));
    }
    if (Gv.Ne(sk.G("hydT"), Gv.Null)) {
      Gv d = ((sk.G("hydT") - sk.G("rakeH"))).AsF();
      Gv stp = (m / Gv.F(0.25));
      if (Gv.Le(Gd.absf(d), stp)) {
        sk = sk.S("rakeH", sk.G("hydT"));
        sk = sk.S("hydT", Gv.Null);
      } else {
        sk = sk.S("rakeH", (sk.G("rakeH") + (Gd.signf(d) * stp)));
      }
    }
    Gv rk = (((m * rkRpm()) / Gv.F(1.5))).AsF();
    if ((((sk.G("rake")).T() && Gv.Gt(sk.G("g"), Gv.I(1L))) && Gv.Ge(sk.G("rakeH"), Gv.F(0.15)))) {
      if (Gv.Lt(sk.G("rakeH"), Gv.F(0.6))) {
        sk = sk.S("c", Gd.maxf(Gv.F(0.0), (sk.G("c") - (Gv.F(0.03) * rk))));
        sk = sk.S("ch", Gd.maxf(Gv.F(0.0), (sk.G("ch") - (Gv.F(0.05) * rk))));
        Gv rl = (Gd.minf(sk.G("g"), (Gv.F(0.01) * rk))).AsF();
        sk = sk.S("g", (sk.G("g") - rl));
        sk = sk.S("fines", (sk.G("fines") + rl));
        Gv tc = ((sk.C("get", Gv.S("teig"), Gv.F(0.0)) * ((Gv.F(1.0) - Gd.exp((Gv.F(-0.25) * rk)))))).AsF();
        sk = sk.S("teig", (sk.G("teig") - tc));
        sk = sk.S("g", (sk.G("g") + (tc * Gv.F(0.9))));
        sk = sk.S("fines", (sk.G("fines") + (tc * Gv.F(0.1))));
      } else {
        sk = sk.S("c", Gd.maxf(Gv.F(0.0), (sk.G("c") - (Gv.F(0.05) * rk))));
        sk = sk.S("filt", Gd.maxf(Gv.F(0.0), (sk.G("filt") - (Gv.F(15.0) * rk))));
        Gv rl2 = (Gd.minf(sk.G("g"), (Gv.F(0.08) * rk))).AsF();
        sk = sk.S("g", (sk.G("g") - rl2));
        sk = sk.S("fines", (sk.G("fines") + rl2));
        Gv tc2 = ((sk.C("get", Gv.S("teig"), Gv.F(0.0)) * ((Gv.F(1.0) - Gd.exp((Gv.F(-0.6) * rk)))))).AsF();
        sk = sk.S("teig", (sk.G("teig") - tc2));
        sk = sk.S("g", (sk.G("g") + (tc2 * Gv.F(0.7))));
        sk = sk.S("fines", (sk.G("fines") + (tc2 * Gv.F(0.3))));
      }
    }
    sk = sk.S("air", Gd.maxf(Gv.F(0.0), (sk.G("air") - (Gv.F(0.5) * m))));
    Gv Tw_ = (sk.G("T")).AsF();
    Gv ph_ = (S.G("pH")).AsF();
    Gv dil = (Gd.clampf((((Gv.F(3.5) - sk.G("outP"))) / Gv.F(3.0)), Gv.F(0.0), Gv.F(1.0))).AsF();
    Gv risk = (Gv.F(0.0)).AsF();
    if (((sk.G("drawn")).T() && Gv.Gt(sk.G("g"), Gv.I(1L)))) {
      risk = (Gd.clampf((((Gv.F(0.5) * Gd.clampf((((ph_ - Gv.F(5.7))) / Gv.F(0.4)), Gv.F(0.0), Gv.F(1.0))) + (Gv.F(0.3) * Gd.clampf((((Tw_ - Gv.F(76.0))) / Gv.F(8.0)), Gv.F(0.0), Gv.F(1.0)))) + (Gv.F(0.2) * dil)), Gv.F(0.0), Gv.F(1.0))).AsF();
      if ((Gv.Lt(ph_, Gv.F(5.7)) && Gv.Lt(Tw_, Gv.F(78.0)))) {
        risk = ((risk * Gv.F(0.3))).AsF();
      }
    }
    S = S.S("tannin", (S.G("tannin") + (((risk - S.G("tannin"))) * ((Gv.F(1.0) - Gd.exp(((-dt) / Gv.F(120.0))))))));
    if (Gv.Gt(S.G("tannin"), Gv.F(0.6))) {
      once((Gv.S("tannin%d") % tb(Gv.I(1800L))), (Gv.S("Riziko vyluhování tříslovin (odhad podle pH %s, teploty %s °C a zředění sladiny %s °P) – uber teplotu vody nebo pH, nebo zastav vyslazování.") % Gd.Arr(fmt1(ph_), fmt0(Tw_), fmt1(sk.G("outP")))), Gv.S("warn"));
    }
    sk = sk.S("Qact", Gv.F(0.0));
    sk = sk.S("drawn", Gv.B(false));
    S = S.S("skFillNow", Gv.B(false));
    mp = mp.S("whirlNow", Gv.B(false));
    S = S.S("wp", S.G("wp").S("whirlNow", Gv.B(false)));
    S = S.S("cool", S.G("cool").S("Tout", Gv.Null));
    S = S.S("cool", S.G("cool").S("T1", Gv.Null));
    Gv _t0 = Gd.Static("Time", "get_ticks_usec");
    net_step(dt);
    if ((prof_on).T()) {
      prof = prof.SI(Gv.S("net"), ((prof.C("get", Gv.S("net"), Gv.I(0L)) + Gd.Static("Time", "get_ticks_usec")) - _t0));
      _t0 = Gd.Static("Time", "get_ticks_usec");
    }
    if ((S.G("skWater").G("on")).T()) {
      S = S.S("maxSparge", Gd.maxf(S.G("maxSparge"), S.G("skWater").G("T")));
    }
    if ((!(S.G("skFillNow")).T() && (sk.G("filling")).T())) {
      sk = sk.S("filling", Gv.B(false));
      sk = sk.S("settle", Gv.F(0.0));
      sk = sk.S("filt", Gv.F(0.0));
    }
    if ((sk.G("filling")).T()) {
      sk = sk.S("settle", Gv.F(0.0));
    }
    sk = sk.S("settle", (sk.G("settle") + m));
    if (!(sk.G("drawn")).T()) {
      sk = sk.S("dp", Gv.F(20.0));
      sk = sk.S("turb", Gv.F(0.0));
      sk = sk.S("dpPa", Gv.F(0.0));
      sk = sk.S("vac", Gv.F(0.0));
    }
    if (((Gv.Lt(sk.G("Qmax"), Gv.I(15L)) && Gv.Gt(sk.G("g"), Gv.I(1L))) && (S.G("p2").G("on")).T())) {
      once((Gv.S("stuck%d") % tb(Gv.I(1800L))), Gv.S("Scezovací káď je zadřená – nakypři mláto a zpomal."), Gv.S("bad"));
    }
    if ((prof_on).T()) {
      prof = prof.SI(Gv.S("skmid"), ((prof.C("get", Gv.S("skmid"), Gv.I(0L)) + Gd.Static("Time", "get_ticks_usec")) - _t0));
      _t0 = Gd.Static("Time", "get_ticks_usec");
    }
    skThermalStep(dt);
    if (Gv.Gt(skVolTotal(), c.G("skCap"))) {
      Gv ov0 = takeCell(sk.G("cells").I(Gv.I(0L)), (skVolTotal() - c.G("skCap")));
      S = S.S("drainL", (S.G("drainL") + cellVol(ov0)));
      S = S.S("drainExt", (S.G("drainExt") + (ov0.G("f") + ov0.G("n"))));
      once((Gv.S("skover%d") % tb(Gv.I(600L))), Gv.S("Scezovací káď přetéká!"), Gv.S("bad"));
    }
    if ((prof_on).T()) {
      prof = prof.SI(Gv.S("skth"), ((prof.C("get", Gv.S("skth"), Gv.I(0L)) + Gd.Static("Time", "get_ticks_usec")) - _t0));
      _t0 = Gd.Static("Time", "get_ticks_usec");
    }
    foreach (Gv pair in (Gd.Arr(Gd.Arr(rvp, Gv.S("rvp")), Gd.Arr(mp, Gv.S("mp")))).Iter()) {
      Gv v = pair.I(Gv.I(0L));
      Gv n = pair.I(Gv.I(1L));
      heat(v, m, dt, n);
      react(v, m);
      if (Gv.Eq(n, Gv.S("rvp"))) {
        Gv on = Gv.B(((v.G("stir")).T() && Gv.Gt(v.G("w"), Gv.I(200L))));
        Gv rpm = (((on).T() ? (v.G("hz") * Gv.F(0.8)) : Gv.F(0.0))).AsF();
        Gv kk = (Gv.Gt((sol(v) + v.G("sg")), Gv.I(20L)) ? Gv.F(0.8) : Gv.F(1.0));
        Gv om = ((((rpm / Gv.F(60.0)) * Gd.K("TAU")) * Gv.F(0.6)) * kk);
        Gv tgt = Gd.minf(Gv.F(0.15), ((((om * om) * Gv.F(1.19)) * Gv.F(1.19)) / ((Gv.F(4.0) * Gv.F(9.81)))));
        v = v.S("vort", (v.G("vort") + (((tgt - v.G("vort"))) * ((Gv.F(1.0) - Gd.exp(((-dt) / Gv.F(20.0))))))));
      }
      Gv cap = ((Gv.Eq(n, Gv.S("rvp")) ? c.G("rvpCap") : c.G("mpCap"))).AsF();
      if (Gv.Gt(vol(v), cap)) {
        Gv ov = takePart(v, (vol(v) - cap), Gv.F(1.0));
        S = S.S("drainL", (S.G("drainL") + vol(ov)));
        S = S.S("drainExt", (S.G("drainExt") + (((ext(ov) + ov.G("su")) + ov.G("sc")) + ov.G("ni"))));
        once((Gv.S("over%s%d") % Gd.Arr(n, tb(Gv.I(600L)))), (Gv.S("%s přetéká!") % VN(n)), Gv.S("bad"));
      }
    }
    foreach (Gv pair_2 in (Gd.Arr(Gd.Arr(rvp, Gv.S("rvp")), Gd.Arr(mp, Gv.S("mp")))).Iter()) {
      Gv v_2 = pair_2.I(Gv.I(0L));
      if ((((v_2.G("boiling")).T() && Gv.Gt(ext(v_2), Gv.I(100L))) && Gv.Lt(v_2.G("g"), Gv.I(5L)))) {
        Gv pct = ((((v_2.G("evapKgMin") * Gv.F(60.0)) / Gd.maxf(Gv.F(1.0), v_2.G("w"))) * Gv.F(100.0))).AsF();
        v_2 = v_2.S("col", (v_2.G("col") + (((liqVol(v_2) * Gv.F(2.5)) / Gv.F(60.0)) * m)));
        v_2 = v_2.S("coag", (v_2.G("coag") + ((((pct / Gv.F(7.0))) * m) / Gv.F(60.0))));
        Gv form = (((Gv.F(0.0006) * v_2.G("w")) * Gd.minf(Gv.F(1.0), v_2.G("coag")))).AsF();
        if (Gv.Gt(form, v_2.G("trubHB"))) {
          v_2 = v_2.S("trubSus", (v_2.G("trubSus") + (form - v_2.G("trubHB"))));
          v_2 = v_2.S("trubHB", form);
        }
        if (Gv.Eq(S.G("flags").G("dmsPre"), Gv.Null)) {
          S = S.S("flags", S.G("flags").S("dmsPre", ((((v_2.G("smm") + v_2.G("dms"))) * Gv.F(1000000.0)) / Gd.maxf(Gv.F(1.0), liqVol(v_2)))));
          S = S.S("flags", S.G("flags").S("boilStart", S.G("t")));
        }
      }
      if (Gv.Ne(pair_2.I(Gv.I(1L)), Gv.S(""))) {
        if ((((v_2.G("boiling")).T() && Gv.Gt(ext(v_2), Gv.I(100L))) && Gv.Lt(v_2.G("g"), Gv.I(5L)))) {
          Gv fp = (Gd.clampf((Gv.F(1.0) - (Gv.F(3.0) * v_2.G("coag"))), Gv.F(0.15), Gv.F(1.0))).AsF();
          v_2 = v_2.S("foam", (v_2.G("foam") + ((((Gv.F(10.0) * v_2.G("evapKgMin")) * v_2.G("evapKgMin")) * fp) * m)));
        }
        v_2 = v_2.S("foam", (v_2.G("foam") - (v_2.G("foam") * ((Gv.F(1.0) - Gd.exp(((-m) / Gv.F(1.0))))))));
        Gv free = ((((Gv.Eq(pair_2.I(Gv.I(1L)), Gv.S("rvp")) ? c.G("rvpCap") : c.G("mpCap"))) - hotV(v_2))).AsF();
        if ((Gv.Gt(v_2.G("foam"), free) && Gv.Gt(v_2.G("foam"), Gv.F(1.0)))) {
          Gv lost = (Gd.minf(vol(v_2), (((v_2.G("foam") - Gd.maxf(Gv.F(0.0), free))) * Gv.F(0.15)))).AsF();
          Gv bo = takePart(v_2, lost, Gv.F(1.0));
          S = S.S("drainL", (S.G("drainL") + vol(bo)));
          S = S.S("drainExt", (S.G("drainExt") + (((ext(bo) + bo.G("su")) + bo.G("sc")) + bo.G("ni"))));
          v_2 = v_2.S("foam", Gd.maxf(Gv.F(0.0), free));
          once((Gv.S("boilover%s%d") % Gd.Arr(pair_2.I(Gv.I(1L)), tb(Gv.I(300L)))), (Gv.S("%s překypěla! Pěna je nad okrajem (%d L pěny). Uber páru, dokud nepadne pěna (lom), chmel přidávej při slabším varu.") % Gd.Arr(VN(pair_2.I(Gv.I(1L))), Gd.roundi((v_2.G("foam") + (lost / Gv.F(0.15)))))), Gv.S("bad"));
        }
      }
      if (((v_2.G("boiling")).T() && Gv.Gt(v_2.G("g"), Gv.I(20L)))) {
        if (!(v_2.G("grainBoiling")).T()) {
          v_2 = v_2.S("grainBoiling", Gv.B(true));
          v_2 = v_2.S("grainBoilMin", Gv.F(0.0));
        }
        v_2 = v_2.S("grainBoilMin", (v_2.G("grainBoilMin") + m));
      }
      if (((v_2.G("grainBoiling")).T() && (Gv.Lt(v_2.G("g"), Gv.I(5L)) || Gv.Lt(v_2.G("T"), Gv.I(85L))))) {
        v_2 = v_2.S("grainBoiling", Gv.B(false));
        if (Gv.Ge(v_2.G("grainBoilMin"), Gv.I(5L))) {
          S = S.S("flags", S.G("flags").S("decCount", (S.G("flags").G("decCount") + Gv.I(1L))));
          logm((Gv.S("Rmut č. %d povařen %d min a spojen.") % Gd.Arr(S.G("flags").G("decCount"), Gd.roundi(v_2.G("grainBoilMin")))));
        }
      }
    }
    Gv wv = S.G("wp");
    if (Gv.Gt(wv.G("w"), Gv.I(1L))) {
      Gv Cw = ((cC(wv) + steelC(c.G("wpSteel")))).AsF();
      wv = wv.S("T", (wv.G("T") - (((c.G("wpUA") * ((wv.G("T") - c.G("ambientT")))) * dt) / Cw)));
      react(wv, m);
    }
    Gv hm = sk.G("mash");
    if (Gv.Gt(hm.G("w"), Gv.I(1L))) {
      react(hm, m);
    }
    foreach (Gv v_3__it in (Gd.Arr(mp, S.G("wp"))).Iter()) {
      Gv v_3 = v_3__it;
      if (((!(v_3.G("whirlNow")).T() && !(v_3.G("boiling")).T()) && Gv.Gt(v_3.G("trubSus"), Gv.I(0L)))) {
        v_3 = v_3.S("settleMin", (v_3.G("settleMin") + m));
        Gv k_2 = (Gv.Ge(v_3.G("whirlMin"), Gv.I(3L)) ? (Gv.F(1.0) / Gv.F(5.0)) : (Gv.F(1.0) / Gv.F(90.0)));
        Gv fl = ((Gv.F(0.1) * v_3.G("trubHB"))).AsF();
        Gv dd = ((Gd.maxf(Gv.F(0.0), (v_3.G("trubSus") - fl)) * ((Gv.F(1.0) - Gd.exp(((-k_2) * m)))))).AsF();
        v_3 = v_3.S("trubSus", (v_3.G("trubSus") - dd));
        v_3 = v_3.S("trubSet", (v_3.G("trubSet") + dd));
      }
    }
    if ((prof_on).T()) {
      prof = prof.SI(Gv.S("heat"), ((prof.C("get", Gv.S("heat"), Gv.I(0L)) + Gd.Static("Time", "get_ticks_usec")) - _t0));
      _t0 = Gd.Static("Time", "get_ticks_usec");
    }
    Gv decoOK = Gv.B(false);
    foreach (Gv v_4 in (Gd.Arr(sk.G("mash"), mp)).Iter()) {
      if ((Gv.Gt(v_4.G("g"), Gv.I(20L)) && (Gv.Gt(v_4.G("aA"), Gv.F(0.05)) || Gv.Gt((v_4.G("aB") + v_4.G("aBs")), Gv.F(0.05))))) {
        decoOK = Gv.B(true);
      }
    }
    if (((Gv.Gt(rvp.G("T"), Gv.I(80L)) && !(decoOK).T()) && Gv.Gt((((rvp.G("su") + rvp.G("sg")) + rvp.G("sc"))), (Gv.F(0.05) * ((((ext(rvp) + rvp.G("su")) + rvp.G("sc")) + Gv.F(1.0))))))) {
      once(Gv.S("rvphot"), Gv.S("Pánev nad 80 °C s nezcukřeným škrobem – enzymy jsou zničené, škrob už nezcukří."), Gv.S("bad"));
    }
    Gv f = S.G("flags");
    Gv gTot = (grain_dry_insoluble(S.G("malt").G("added"))).AsF();
    Gv grainV = Gd.Arr();
    foreach (Gv v_5 in (Gd.Arr(rvp, mp)).Iter()) {
      if (Gv.Gt(v_5.G("g"), Gv.I(20L))) {
        grainV.C("append", v_5);
      }
    }
    foreach (Gv v_6 in (grainV).Iter()) {
      if (((Gv.Ge(v_6.G("T"), Gv.I(49L)) && Gv.Le(v_6.G("T"), Gv.I(54L))) && Gv.Gt(S.G("malt").G("added"), Gv.I(100L)))) {
        f = f.S("rest52", (f.G("rest52") + m));
        break;
      }
    }
    Gv held = (sk.G("mash").G("g")).AsF();
    if (((!(f.G("decStart")).T() && Gv.Gt(gTot, Gv.I(0L))) && ((Gv.Gt(rvp.G("g"), (Gv.F(0.15) * gTot)) && (Gv.Gt(mp.G("g"), (Gv.F(0.15) * gTot)) || Gv.Gt(held, (Gv.F(0.15) * gTot)))) || (Gv.Gt(mp.G("g"), (Gv.F(0.15) * gTot)) && Gv.Gt(held, (Gv.F(0.15) * gTot)))))) {
      f = f.S("decStart", Gv.I(1L));
    }
    foreach (Gv v_7 in (grainV).Iter()) {
      if (((f.G("decStart")).T() && !(f.G("decCount")).T())) {
        if ((Gv.Ge(v_7.G("T"), Gv.I(61L)) && Gv.Le(v_7.G("T"), Gv.I(65L)))) {
          f = f.S("r63", (f.G("r63") + m));
        }
        if ((Gv.Ge(v_7.G("T"), Gv.I(70L)) && Gv.Le(v_7.G("T"), Gv.I(74L)))) {
          f = f.S("r72", (f.G("r72") + m));
        }
      }
      if (((Gv.Ge(f.G("decCount"), Gv.I(1L)) && Gv.Ge(v_7.G("T"), Gv.I(70L))) && Gv.Le(v_7.G("T"), Gv.I(74L)))) {
        f = f.S("r72b", (f.G("r72b") + m));
      }
      if (((Gv.Ge(f.G("decCount"), Gv.I(1L)) && Gv.Ge(v_7.G("T"), Gv.I(61L))) && Gv.Le(v_7.G("T"), Gv.I(66L)))) {
        f = f.S("r63b", (f.G("r63b") + m));
      }
    }
    if ((Gv.Gt(gTot, Gv.I(0L)) && Gv.Gt((sk.G("g") + held), (Gv.F(0.9) * gTot)))) {
      f = f.S("inSK", Gv.I(1L));
    }
    if (Gv.Ge(sk.G("under"), Gv.I(120L))) {
      f = f.S("under", Gv.I(1L));
    }
    if ((((sk.G("drawn")).T() && Gv.Gt(sk.G("g"), Gv.I(1L))) && Gv.Lt(sk.G("turb"), Gv.I(20L)))) {
      f = f.S("clearOK", Gv.I(1L));
    }
    Gv wb = Gv.B(false);
    foreach (Gv v_8 in (Gd.Arr(rvp, mp)).Iter()) {
      if (((Gv.Lt(v_8.G("g"), Gv.I(5L)) && Gv.Gt(ext(v_8), Gv.I(200L))) && Gv.Ge(hotV(v_8), S.G("cfg").C("get", Gv.S("preboilV"), Gv.F(4430.0))))) {
        f = f.S("full", Gv.I(1L));
      }
      if ((((v_8.G("boiling")).T() && Gv.Gt(ext(v_8), Gv.I(100L))) && Gv.Lt(v_8.G("g"), Gv.I(5L)))) {
        wb = Gv.B(true);
      }
    }
    if ((wb).T()) {
      f = f.S("wortBoil", (f.G("wortBoil") + m));
    }
    foreach (Gv v_9 in (Gd.Arr(mp, S.G("wp"))).Iter()) {
      if ((Gv.Ge(v_9.G("whirlMin"), Gv.I(3L)) && Gv.Ge(v_9.G("settleMin"), Gv.I(20L)))) {
        f = f.S("whirlDone", Gv.I(1L));
      }
    }
    if ((prof_on).T()) {
      prof = prof.SI(Gv.S("checks"), ((prof.C("get", Gv.S("checks"), Gv.I(0L)) + Gd.Static("Time", "get_ticks_usec")) - _t0));
      _t0 = Gd.Static("Time", "get_ticks_usec");
    }
    training.C("update", dt);
    lauter_auto.C("update", dt);
    if (Gv.Ne(proc, Gv.Null)) {
      proc.C("step", dt);
    }
    if ((prof_on).T()) {
      prof = prof.SI(Gv.S("train"), ((prof.C("get", Gv.S("train"), Gv.I(0L)) + Gd.Static("Time", "get_ticks_usec")) - _t0));
    }
    S = S.S("t", (S.G("t") + dt));
    return Gv.Null;
  }
  public Gv _pd(Gv v = default(Gv), Gv UA = default(Gv), Gv jL = default(Gv), Gv Tst = default(Gv)) {
    UA = (UA).AsF();
    jL = (jL).AsF();
    Tst = (Tst).AsF();
    Gv V = (liqVol(v) + (sol(v) * Gv.F(0.65)));
    Gv wet = Gd.clampf((((V - Gv.F(100.0))) / ((jL - Gv.F(100.0)))), Gv.F(0.0), Gv.F(1.0));
    return Gd.Dict(Gv.S("P"), (((UA * v.G("heat")) * wet) * Gd.maxf(Gv.F(0.0), (Tst - v.G("T")))), Gv.S("wet"), wet);
    return Gv.Null;
  }
  public Gv hopTarget() {
    Gv best = Gv.Null;
    foreach (Gv v in (Gd.Arr(S.G("rvp"), S.G("mp"))).Iter()) {
      if ((Gv.Lt(v.G("g"), Gv.I(5L)) && Gv.Gt(ext(v), Gv.I(50L)))) {
        if (((Gv.Eq(best, Gv.Null) || Gv.Gt(Gd.Dyn("int", v.G("boiling")), Gd.Dyn("int", best.G("boiling")))) || (Gv.Eq(v.G("boiling"), best.G("boiling")) && Gv.Gt(v.G("T"), best.G("T"))))) {
          best = v;
        }
      }
    }
    if (Gv.Ne(best, Gv.Null)) {
      return best;
    }
    return (Gv.Gt(S.G("mp").G("w"), S.G("rvp").G("w")) ? S.G("mp") : S.G("rvp"));
    return Gv.Null;
  }
  public Gv hopAdd(Gv hopId = default(Gv), Gv kg = default(Gv)) {
    kg = (kg).AsF();
    Gv hp = Gv.Null;
    foreach (Gv x in (HOPS).Iter()) {
      if (Gv.Eq(x.G("id"), hopId)) {
        hp = x;
      }
    }
    if ((Gv.Eq(hp, Gv.Null) || Gv.Le(kg, Gv.I(0L)))) {
      return Gv.Null;
    }
    Gv v = hopTarget();
    v = v.S("alpha", (v.G("alpha") + ((kg * hp.G("a")) * Gv.F(10.0))));
    v = v.S("trubSus", (v.G("trubSus") + (kg * Gv.F(0.7))));
    if ((v.G("boiling")).T()) {
      v = v.S("foam", (v.G("foam") + ((Gd.minf(Gv.F(1.0), (kg / Gv.F(3.0))) * Gd.clampf((Gv.F(1.0) - (Gv.F(3.0) * v.G("coag"))), Gv.F(0.15), Gv.F(1.0))) * ((Gv.F(150.0) + (Gv.F(60.0) * v.G("evapKgMin")))))));
    }
    S.G("hops").C("append", Gd.Dict(Gv.S("t"), S.G("t"), Gv.S("name"), hp.G("name"), Gv.S("kg"), kg, Gv.S("a"), hp.G("a"), Gv.S("boil"), v.G("boiling"), Gv.S("T"), v.G("T"), Gv.S("boilMin"), v.G("boilMin")));
    logm((Gv.S("Chmel: %s kg %s (%s %% alfa) při %s °C.") % Gd.Arr(fmt1(kg), hp.G("name"), fmt1(hp.G("a")), fmt0(v.G("T")))));
    return Gv.Null;
  }
  public Gv hopPlanNext() {
    return (Gv.Lt(S.G("hopIdx"), S.G("hopPlan").C("size")) ? S.G("hopPlan").I(S.G("hopIdx")) : Gv.Null);
    return Gv.Null;
  }
  public Gv hopPlanAdd() {
    Gv hp = hopPlanNext();
    if (Gv.Eq(hp, Gv.Null)) {
      return Gv.Null;
    }
    hopAdd(hp.G("hop"), hp.G("kg"));
    S = S.S("hopIdx", (S.G("hopIdx") + Gv.I(1L)));
    return Gv.Null;
  }
  public Gv kypr() {
    Gv kl = (Gd.minf(S.G("sk").G("g"), Gv.F(3.2))).AsF();
    S = S.S("sk", S.G("sk").S("g", (S.G("sk").G("g") - kl)));
    S = S.S("sk", S.G("sk").S("c", (S.G("sk").G("c") * Gv.F(0.35))));
    S = S.S("sk", S.G("sk").S("fines", (S.G("sk").G("fines") + kl)));
    S = S.S("sk", S.G("sk").S("filt", Gd.maxf(Gv.F(0.0), (S.G("sk").G("filt") - Gv.F(100.0)))));
    Gv tk = ((S.G("sk").C("get", Gv.S("teig"), Gv.F(0.0)) * Gv.F(0.8))).AsF();
    S = S.S("sk", S.G("sk").S("teig", (S.G("sk").G("teig") - tk)));
    S = S.S("sk", S.G("sk").S("g", (S.G("sk").G("g") + (tk * Gv.F(0.7)))));
    S = S.S("sk", S.G("sk").S("fines", (S.G("sk").G("fines") + (tk * Gv.F(0.3)))));
    S = S.S("sk", S.G("sk").S("dpPa", Gv.F(0.0)));
    logm(Gv.S("Hluboké kypření – lože povoleno, sladina se na chvíli zakalí. Vrať ji do vorlaufu."));
    return Gv.Null;
  }
  public Gv emptySK() {
    Gv sk = S.G("sk");
    Gv e = Gv.F(0.0);
    foreach (Gv c in (sk.G("cells")).Iter()) {
      e = (e + (c.G("f") + c.G("n")));
    }
    sk = sk.S("lostExt", (sk.G("lostExt") + e));
    logm((Gv.S("Mláto vyhrabáno (%d kg mokrého). V mlátě zůstalo %s kg extraktu.") % Gd.Arr(Gd.roundi(((skMalt() * Gv.F(0.26)) / Gv.F(0.2))), fmt1(e))));
    sk = sk.S("cells", Gd.Arr(cell(), cell(), cell(), cell(), cell()));
    sk = sk.S("g", Gv.F(0.0));
    sk = sk.S("c", Gv.F(0.0));
    sk = sk.S("burst", Gv.F(0.0));
    sk = sk.S("emptied", Gv.B(true));
    sk = sk.S("dig", Gv.F(45.0));
    sk = sk.S("teig", Gv.F(0.0));
    sk = sk.S("dpPa", Gv.F(0.0));
    sk = sk.S("ch", Gv.F(0.0));
    sk = sk.SI(Gv.S("dep"), Gv.F(0.0));
    sk = sk.S("under", Gv.F(0.0));
    sk = sk.S("landed", Gv.B(false));
    sk = sk.S("underOK", Gv.B(false));
    sk = sk.S("bedMade", Gv.B(false));
    return Gv.Null;
  }
  public Gv lvChillSet(Gv on = default(Gv)) {
    Gv pct = ((S.G("lv").G("w") / S.G("cfg").G("lvVol"))).AsF();
    if (((on).T() && Gv.Lt(pct, Gv.F(0.35)))) {
      logm((Gv.S("Okruh ledovky nejde pustit: nádrž je napuštěná jen na %d %% – nejdřív ji napusť z řadu (aspoň 35 %%).") % Gd.roundi((pct * Gv.F(100.0)))), Gv.S("warn"));
      return Gv.B(false);
    }
    S = S.S("lv", S.G("lv").S("chill", on));
    logm(((on).T() ? Gv.S("Agregát ledovky zapnut – okruh chladí vodu v nádrži.") : Gv.S("Agregát ledovky vypnut.")));
    return Gv.B(true);
    return Gv.Null;
  }
  public Gv push() {
    Gv sp = S.G("sp");
    if (Gv.Lt(sp.G("L"), Gv.I(10L))) {
      logm(Gv.S("Protlačka: ve spilce ještě nic není."), Gv.S("warn"));
      return Gv.Null;
    }
    if ((S.G("flags").G("pushed")).T()) {
      logm(Gv.S("Protlačka už proběhla – další voda jen ředí mladinu."), Gv.S("warn"));
    }
    if (Gv.In(Gv.S("--lauterlog"), Gd.Static("OS", "get_cmdline_user_args"))) {
      Gd.print(Gv.S("INV před protlačkou: "), extract_inventory());
    }
    Gv ids = S.C("get", Gv.S("_castEdges"), Gd.Dict());
    Gv moved = Gv.F(0.0);
    Gv ex = Gv.F(0.0);
    Gv tout = (Gv.Ne(S.G("cool").G("Tout"), Gv.Null) ? S.G("cool").G("Tout") : Gv.F(8.0));
    foreach (Gv e__it in (net.G("edges")).Iter()) {
      Gv e = e__it;
      if ((!(ids.C("has", e.G("id"))).T() || Gv.Lt(vol(e.G("c")), Gv.F(0.01)))) {
        continue;
      }
      Gv part = lp();
      foreach (Gv k in (LIQ).Iter()) {
        part = part.SI(k, e.G("c").I(k));
      }
      foreach (Gv k_2 in (SOL).Iter()) {
        part = part.SI(k_2, e.G("c").I(k_2));
      }
      part = part.S("T", Gd.minf(e.G("c").G("T"), (tout + Gv.F(2.0))));
      moved = (moved + vol(part));
      ex = (ex + ext(part));
      spAdd(part);
      Gv wfill = lp();
      wfill = wfill.S("w", e.G("cap"));
      wfill = wfill.S("T", S.G("cool").G("waterT"));
      e = e.S("c", wfill.C("duplicate"));
      e = e.S("q", Gd.Arr(wfill));
    }
    Gv wtail = lp();
    wtail = wtail.S("w", Gv.F(8.0));
    wtail = wtail.S("T", S.G("cool").G("waterT"));
    spAdd(wtail);
    if (Gv.In(Gv.S("--lauterlog"), Gd.Static("OS", "get_cmdline_user_args"))) {
      Gd.print(Gv.S("INV po protlačce: "), extract_inventory());
    }
    S = S.S("flags", S.G("flags").S("pushed", Gv.I(1L)));
    logm((Gv.S("Protlačka: %s L mladiny (%s kg extraktu) z chladiče a potrubí dotlačeno vodou do tanku, trubky zůstaly plné vody.") % Gd.Arr(fmt0(moved), fmt1(ex))));
    return Gv.Null;
  }
  public Gv gauge_vessel_free(Gv g = default(Gv)) {
    Gv key = (Gv.S("gv|%s|%s|%s") % Gd.Arr(g.G("id"), Gd.Dyn("str", g.C("get", Gv.S("top"), Gv.B(true))), Gd.Dyn("str", g.C("get", Gv.S("bot"), Gv.B(true)))));
    Gv cached = _cached(key);
    if (Gv.Ne(cached, Gv.Null)) {
      return cached.I(Gv.I(0L));
    }
    Gv starts = Gd.Arr();
    Gv G = main.C("gauge_geo", g);
    Gv ports = Gd.Arr();
    if (Gv.Ne(g.C("get", Gv.S("top"), Gv.B(true)), Gv.B(false))) {
      ports.C("append", Gd.Ctor("Vector2", G.G("x"), (G.G("y1") - Gv.F(30.0))));
    }
    if (Gv.Ne(g.C("get", Gv.S("bot"), Gv.B(true)), Gv.B(false))) {
      ports.C("append", Gd.Ctor("Vector2", G.G("x"), (G.G("y2") + Gv.F(30.0))));
    }
    foreach (Gv pt in (ports).Iter()) {
      foreach (Gv n in (net.G("nodes").C("values")).Iter()) {
        if (((Gv.Gt(n.G("edges").C("size"), Gv.I(0L)) && Gv.Le(Gd.Ctor("Vector2", n.G("x"), n.G("y")).C("distance_to", pt), Gv.F(14.0))) && !(starts.C("has", n.G("k"))).T())) {
          starts.C("append", n.G("k"));
        }
      }
      foreach (Gv e in (net.G("edges")).Iter()) {
        Gv near = Gv.B(false);
        foreach (Gv i in ((e.G("pts").C("size") - Gv.I(1L))).Iter()) {
          if (Gv.Le(Gd.Static("Geometry2D", "get_closest_point_to_segment", pt, e.G("pts").I(i), e.G("pts").I((i + Gv.I(1L)))).C("distance_to", pt), Gv.F(14.0))) {
            near = Gv.B(true);
            break;
          }
        }
        if ((near).T()) {
          foreach (Gv k in (Gd.Arr(e.G("a"), e.G("b"))).Iter()) {
            if (!(starts.C("has", k)).T()) {
              starts.C("append", k);
            }
          }
        }
      }
    }
    Gv found = Gv.Null;
    if ((starts.C("size")).T()) {
      Gv r = bfs(starts);
      foreach (Gv h in (r.G("hits")).Iter()) {
        if (Gv.In(h.G("att").G("bind"), Gd.Arr(Gv.S("rvp"), Gv.S("mp"), Gv.S("sk"), Gv.S("wp"), Gv.S("hlt"), Gv.S("lv"), Gv.S("spilka")))) {
          found = net.G("byId").I(h.G("att").G("id"));
          break;
        }
      }
    }
    _cache = _cache.SI(key, Gd.Arr(found));
    return found;
    return Gv.Null;
  }
  public Gv skHead() {
    return ((cellVol(S.G("sk").G("cells").I(Gv.I(0L))) / ((S.G("cfg").G("skArea") * Gv.F(1000.0))))).AsF();
    return Gv.Null;
  }
  public Gv tannin_risk() {
    return (S.G("tannin")).AsF();
    return Gv.Null;
  }
  public Gv lauter_efficiency() {
    return ((Gv.Gt(S.G("sk").G("initialExtract"), Gv.I(0L)) ? ((Gv.F(100.0) * ext(S.G("rvp"))) / Gd.maxf(Gv.F(1e-09), S.G("sk").G("initialExtract"))) : Gv.F(0.0))).AsF();
    return Gv.Null;
  }
  public Gv start_lautering() {
    if (Gv.Eq(S.G("sk").G("mode"), Gv.S("lauter"))) {
      return Gv.Null;
    }
    S = S.S("sk", S.G("sk").S("mode", Gv.S("lauter")));
    skMashToBed();
    skQmax();
    S = S.S("sk", S.G("sk").S("initialExtract", ext(S.G("rvp"))));
    foreach (Gv layer in (S.G("sk").G("cells")).Iter()) {
      S = S.S("sk", S.G("sk").S("initialExtract", (S.G("sk").G("initialExtract") + (layer.G("f") + layer.G("n")))));
    }
    foreach (Gv edge in (net.C("get", Gv.S("edges"), Gd.Arr())).Iter()) {
      S = S.S("sk", S.G("sk").S("initialExtract", (S.G("sk").G("initialExtract") + ext(edge.G("c")))));
    }
    logm(Gv.S("Odrmutováno: nech lože usadit, potom vracej první zakalenou sladinu do SK. Do pánve až čirá."));
    return Gv.Null;
  }
  public Gv turbidity(Gv p = default(Gv)) {
    Gv V = (liqVol(p)).AsF();
    if (Gv.Le(V, Gv.F(1e-08))) {
      return (Gv.F(0.0)).AsF();
    }
    return ((Gv.F(3.0) + ((Gv.F(5600.0) * p.G("fines")) / V))).AsF();
    return Gv.Null;
  }
  public Gv sight_sample(Gv it = default(Gv)) {
    Gv pos = Gd.Ctor("Vector2", it.G("x"), it.G("y"));
    Gv best = Gv.Null;
    Gv bd = Gv.F(40.0);
    Gv bt = Gv.F(0.0);
    foreach (Gv e in (net.G("edges")).Iter()) {
      Gv pts = e.G("pts");
      Gv tot = Gv.F(0.0);
      foreach (Gv i in ((pts.C("size") - Gv.I(1L))).Iter()) {
        tot = (tot + pts.I(i).C("distance_to", pts.I((i + Gv.I(1L)))));
      }
      Gv run = Gv.F(0.0);
      foreach (Gv i_2 in ((pts.C("size") - Gv.I(1L))).Iter()) {
        Gv a = pts.I(i_2);
        Gv b = pts.I((i_2 + Gv.I(1L)));
        Gv c = Gd.Static("Geometry2D", "get_closest_point_to_segment", pos, a, b);
        Gv dd = c.C("distance_to", pos);
        if (Gv.Lt(dd, bd)) {
          bd = dd;
          best = e;
          bt = (((run + a.C("distance_to", c))) / Gd.maxf(Gv.F(1.0), tot));
        }
        run = (run + a.C("distance_to", b));
      }
    }
    if (Gv.Eq(best, Gv.Null)) {
      return Gd.Dict(Gv.S("connected"), Gv.B(false), Gv.S("volume"), Gv.F(0.0), Gv.S("flow"), Gv.F(0.0));
    }
    Gv e_2 = best;
    Gv total = (vol(e_2.G("c"))).AsF();
    Gv lo = ((Gv.Eq(e_2.G("from"), Gv.S("a")) ? Gv.F(0.0) : (e_2.G("cap") - total))).AsF();
    Gv rel = (((bt * e_2.G("cap")) - lo)).AsF();
    Gv found = Gv.Null;
    if ((Gv.Ge(rel, Gv.F(0.0)) && Gv.Le(rel, total))) {
      Gv cum = Gv.F(0.0);
      foreach (Gv b_2 in (e_2.G("q")).Iter()) {
        Gv vb = (vol(b_2)).AsF();
        if (Gv.Le(rel, ((cum + vb) + Gv.F(1e-09)))) {
          found = b_2;
          break;
        }
        cum = (cum + vb);
      }
    }
    if (Gv.Eq(found, Gv.Null)) {
      return Gd.Dict(Gv.S("connected"), Gv.B(true), Gv.S("volume"), Gv.F(0.0), Gv.S("flow"), e_2.G("flow"), Gv.S("direction"), (Gv.Eq(e_2.G("from"), Gv.S("a")) ? Gv.F(1.0) : Gv.F(-1.0)));
    }
    Gv V = (Gd.maxf(Gv.F(1e-09), vol(found))).AsF();
    Gv LV = (Gd.maxf(Gv.F(1e-09), liqVol(found))).AsF();
    Gv fines_mg = (((found.G("fines") * Gv.F(1000000.0)) / V)).AsF();
    return Gd.Dict(Gv.S("connected"), Gv.B(true), Gv.S("volume"), vol(found), Gv.S("flow"), e_2.G("flow"), Gv.S("T"), found.G("T"), Gv.S("P"), plato(found), Gv.S("turb"), turbidity(found), Gv.S("fine"), fines_mg, Gv.S("husk"), (Gv.F(0.15) * fines_mg), Gv.S("direction"), (Gv.Eq(e_2.G("from"), Gv.S("a")) ? Gv.F(1.0) : Gv.F(-1.0)), Gv.S("air"), (found.G("air") / LV), Gv.S("EBC"), (found.G("col") / Gd.maxf(Gv.F(0.01), LV)), Gv.S("kind"), contentKind(found));
    return Gv.Null;
  }
  public Gv extract_inventory() {
    Gv x = Gd.Dict();
    Gv pot = Gd.Fn((_a) => { Gv v = Gd.A(_a, 0); return (((ext(v) + v.G("su")) + v.G("sc")) + v.G("ni")); return Gv.Null; });
    x = x.S("rvp", pot.C("call", S.G("rvp")));
    x = x.S("mp", pot.C("call", S.G("mp")));
    x = x.S("wp", pot.C("call", S.G("wp")));
    x = x.S("skmash", pot.C("call", S.G("sk").G("mash")));
    Gv sk = Gv.F(0.0);
    foreach (Gv c in (S.G("sk").G("cells")).Iter()) {
      sk = (sk + (c.G("f") + c.G("n")));
    }
    x = x.S("sk", sk);
    x = x.S("sp", (S.G("sp").G("f") + S.G("sp").G("n")));
    Gv tr = Gv.F(0.0);
    foreach (Gv e in (net.G("edges")).Iter()) {
      tr = (tr + pot.C("call", e.G("c")));
    }
    x = x.S("trubky", tr);
    x = x.S("vyteklo", S.G("drainExt"));
    x = x.S("mlato", S.G("sk").G("lostExt"));
    x = x.S("skrob_v_lozi", S.G("sk").G("lost"));
    x = x.S("celkem", (((((((((x.G("rvp") + x.G("mp")) + x.G("wp")) + x.G("skmash")) + x.G("sk")) + x.G("sp")) + tr) + S.G("drainExt")) + x.G("mlato")) + x.G("skrob_v_lozi")));
    x = x.S("vystreno", S.G("extIn"));
    return x;
    return Gv.Null;
  }
  public Gv spReport() {
    Gv sp = S.G("sp");
    Gv e = ((sp.G("f") + sp.G("n"))).AsF();
    if (Gv.Lt(sp.G("L"), Gv.I(10L))) {
      return Gv.Null;
    }
    Gv P = (((Gv.F(100.0) * e) / ((e + sp.G("w"))))).AsF();
    Gv rho = ((Gv.F(1.0) + (Gv.F(0.004) * P))).AsF();
    Gv V20 = ((((sp.G("w") + e)) / rho)).AsF();
    Gv rep = Gd.Dict(Gv.S("V"), (V20 / Gv.F(100.0)), Gv.S("P"), P, Gv.S("IBU"), (((sp.G("iso") * Gv.F(1000.0)) / V20) * Gd.Dyn("float", S.G("cfg").C("get", Gv.S("iboKeep"), Gv.F(0.44)))), Gv.S("T"), (sp.G("H") / sp.G("L")), Gv.S("EBC"), (sp.G("col") / Gd.maxf(Gv.F(1.0), sp.G("L"))), Gv.S("att"), (Gv.Gt(e, Gv.I(0L)) ? ((((sp.G("f") / e)) / Gv.F(0.81)) * Gv.F(100.0)) : Gv.F(0.0)), Gv.S("dms"), ((sp.G("dms") * Gv.F(1000000.0)) / V20), Gv.S("yield"), ((e / Gd.maxf(Gv.F(1.0), S.G("malt").G("added"))) * Gv.F(100.0)));
    rep.C("merge", yields());
    return rep;
    return Gv.Null;
  }
  public Gv yields() {
    Gv m = (Gd.maxf(Gv.F(1.0), S.G("malt").G("added"))).AsF();
    Gv out_ = Gd.Dict(Gv.S("m_slad"), m, Gv.S("e_lab"), S.G("extIn"));
    Gv kn = S.G("flags").C("get", Gv.S("knock"));
    if (Gv.Ne(kn, Gv.Null)) {
      Gv P = (kn.G("P")).AsF();
      Gv rho = ((Gv.F(1.0) + (Gv.F(0.004) * P))).AsF();
      out_ = out_.SI(Gv.S("e_vyr"), kn.G("e"));
      out_ = out_.SI(Gv.S("VV_varna"), ((kn.G("e") / m) * Gv.F(100.0)));
      out_ = out_.SI(Gv.S("VV_cz"), (((((((kn.G("V") * Gv.F(0.96))) * rho) * P) / Gv.F(100.0)) / m) * Gv.F(100.0)));
      out_ = out_.SI(Gv.S("ucinnost"), ((kn.G("e") / Gd.maxf(Gv.F(1.0), S.G("extIn"))) * Gv.F(100.0)));
    }
    Gv e_sp = ((S.G("sp").G("f") + S.G("sp").G("n"))).AsF();
    if (Gv.Gt(e_sp, Gv.F(1.0))) {
      out_ = out_.SI(Gv.S("e_sp"), e_sp);
      out_ = out_.SI(Gv.S("VV_spilka"), ((e_sp / m) * Gv.F(100.0)));
    }
    return out_;
    return Gv.Null;
  }
  public Gv recipe_check(Gv R = default(Gv), Gv vv = default(Gv)) {
    if (vv.k == 0) vv = Gv.F(71.0);
    Gv m = Gv.F(0.0);
    foreach (Gv k in (R.C("get", Gv.S("grist"), Gd.Dict())).Iter()) {
      m = (m + Gd.Dyn("float", R.G("grist").I(k)));
    }
    Gv P = (R.C("get", Gv.S("P"), Gv.F(12.0))).AsF();
    Gv V = ((S.G("cfg").C("get", Gv.S("targetV"), Gv.F(40.0)) * Gv.F(100.0))).AsF();
    Gv e_need = (((((V * Gv.F(0.96)) * ((Gv.F(1.0) + (Gv.F(0.004) * P)))) * P) / Gv.F(100.0))).AsF();
    Gv m_need = ((e_need / ((vv / Gv.F(100.0))))).AsF();
    return (Gv.S("Cíl vyrážka %s hl × %s °P = %d kg extraktu → při varní výtěžnosti %d %% je potřeba ≈ %d kg sladu (v receptu %d kg).") % Gd.Arr(fmt1((V / Gv.F(100.0))), fmt1(P), Gd.roundi(e_need), Gd.roundi(vv), Gd.roundi(m_need), Gd.roundi(m)));
    return Gv.Null;
  }
  public Gv fmt0(Gv x = default(Gv)) {
    x = (x).AsF();
    return (Gv.S("%d") % Gd.roundi(x));
    return Gv.Null;
  }
  public Gv fmt1(Gv x = default(Gv)) {
    x = (x).AsF();
    return ((Gv.S("%.1f") % x)).C("replace", Gv.S("."), Gv.S(","));
    return Gv.Null;
  }
  public Gv K(Gv p = default(Gv)) {
    return (Gv.S("%d,%d") % Gd.Arr(Gd.roundi(p.G("x")), Gd.roundi(p.G("y"))));
    return Gv.Null;
  }
  public Gv on_seg_t(Gv p = default(Gv), Gv a = default(Gv), Gv b = default(Gv), Gv tolr = default(Gv)) {
    if (tolr.k == 0) tolr = Gv.F(1.5);
    Gv d = (b - a);
    Gv l2 = d.C("length_squared");
    if (Gv.Lt(l2, Gv.I(1L))) {
      return (Gv.F(-1.0)).AsF();
    }
    Gv cr = (Gd.absf(((d.G("x") * ((p.G("y") - a.G("y")))) - (d.G("y") * ((p.G("x") - a.G("x")))))) / Gd.sqrt(l2));
    if (Gv.Gt(cr, tolr)) {
      return (Gv.F(-1.0)).AsF();
    }
    Gv t = ((((((p.G("x") - a.G("x"))) * d.G("x")) + (((p.G("y") - a.G("y"))) * d.G("y")))) / l2);
    return (((Gv.Gt(t, Gv.F(1e-06)) && Gv.Lt(t, (Gv.F(1.0) - Gv.F(1e-06)))) ? t : Gv.F(-1.0))).AsF();
    return Gv.Null;
  }
  public Gv v_local(Gv v = default(Gv), Gv p = default(Gv)) {
    Gv r = Gd.deg_to_rad(Gd.Dyn("float", v.C("get", Gv.S("rot"), Gv.I(0L))));
    if (Gv.Eq(r, Gv.I(0L))) {
      return p;
    }
    Gv c = Gd.Ctor("Vector2", (v.G("x") + (v.C("get", Gv.S("w"), Gv.I(0L)) / Gv.F(2.0))), (v.G("y") + (v.C("get", Gv.S("h"), Gv.I(0L)) / Gv.F(2.0))));
    return (((p - c)).C("rotated", (-r)) + c);
    return Gv.Null;
  }
  public Gv fit_ports(Gv f = default(Gv)) {
    Gv A = Gd.Dyn("float", f.C("get", Gv.S("arm"), Gv.I(30L)));
    Gv o = Gd.Arr();
    foreach (Gv a in (f.C("get", Gv.S("ports"), Gd.Arr(Gv.I(0L), Gv.I(90L)))).Iter()) {
      Gv r = Gd.deg_to_rad(Gd.Dyn("float", a));
      o.C("append", Gd.Ctor("Vector2", (f.G("x") + (Gd.cos(r) * A)), (f.G("y") + (Gd.sin(r) * A))).C("round"));
    }
    return o;
    return Gv.Null;
  }
  public Gv _node(Gv nodes = default(Gv), Gv p = default(Gv), Gv sfx = default(Gv)) {
    Gv k = (K(p) + sfx);
    if (!(nodes.C("has", k)).T()) {
      nodes = nodes.SI(k, Gd.Dict(Gv.S("k"), k, Gv.S("x"), Gd.roundf(p.G("x")), Gv.S("y"), Gd.roundf(p.G("y")), Gv.S("edges"), Gd.Arr(), Gv.S("att"), Gd.Arr(), Gv.S("valve"), Gv.Null, Gv.S("end"), Gv.I(0L), Gv.S("noatt"), Gv.B(false), Gv.S("openEnd"), Gv.B(false), Gv.S("_tap"), Gv.B(false), Gv.S("_chil"), Gv.S(""), Gv.S("pside"), Gv.Null));
    }
    return nodes.I(k);
    return Gv.Null;
  }
  public Gv build_net(Gv items = default(Gv), Gv joints = default(Gv)) {
    Gv old = Gd.Dict();
    if ((net.C("has", Gv.S("edges"))).T()) {
      foreach (Gv e in (net.G("edges")).Iter()) {
        old = old.SI(e.G("id"), e);
      }
    }
    Gv byId = Gd.Dict();
    foreach (Gv it in (items).Iter()) {
      byId = byId.SI(it.G("id"), it);
    }
    Gv jv = Gd.Arr();
    foreach (Gv q in (joints).Iter()) {
      jv.C("append", Gd.Ctor("Vector2", q.I(Gv.I(0L)), q.I(Gv.I(1L))));
    }
    Gv pipes = Gd.Arr();
    foreach (Gv it_2 in (items).Iter()) {
      if ((Gv.Eq(it_2.G("type"), Gv.S("pipe")) && Gv.Ge(it_2.G("pts").C("size"), Gv.I(2L)))) {
        Gv pts = Gd.Arr();
        foreach (Gv q_2 in (it_2.G("pts")).Iter()) {
          pts.C("append", Gd.Ctor("Vector2", q_2.I(Gv.I(0L)), q_2.I(Gv.I(1L))));
        }
        pipes.C("append", Gd.Dict(Gv.S("id"), it_2.G("id"), Gv.S("ringmul"), Gd.Dyn("float", it_2.C("get", Gv.S("ringmul"), Gv.F(1.0))), Gv.S("medium"), it_2.C("get", Gv.S("medium"), Gv.S("wort")), Gv.S("dn"), Gd.Dyn("float", it_2.C("get", Gv.S("dn"), Gv.I(50L))), Gv.S("pts"), pts, Gv.S("solo"), it_2.C("get", Gv.S("solo"), Gv.B(false)), Gv.S("off"), it_2.C("get", Gv.S("off"), Gv.Null)));
      }
    }
    foreach (Gv mf in (items).Iter()) {
      if (Gv.Eq(mf.G("type"), Gv.S("manifold"))) {
        Gv k = Gv.I(0L);
        foreach (Gv q_3 in (main.C("mf_ports", mf)).Iter()) {
          pipes.C("append", Gd.Dict(Gv.S("id"), (Gv.S("%s_b%d") % Gd.Arr(mf.G("id"), k)), Gv.S("medium"), Gv.S("water"), Gv.S("dn"), Gv.F(50.0), Gv.S("pts"), Gd.Arr(q_3.G("back"), q_3.G("front")), Gv.S("solo"), Gv.B(true), Gv.S("off"), Gv.Null));
          k = (k + Gv.I(1L));
        }
      }
    }
    foreach (Gv f in (items).Iter()) {
      if (Gv.Eq(f.G("type"), Gv.S("fitting"))) {
        Gv k2 = Gv.I(0L);
        foreach (Gv q_4 in (fit_ports(f)).Iter()) {
          pipes.C("append", Gd.Dict(Gv.S("id"), (Gv.S("%s_%d") % Gd.Arr(f.G("id"), k2)), Gv.S("medium"), Gv.S("water"), Gv.S("dn"), Gv.F(50.0), Gv.S("pts"), Gd.Arr(Gd.Ctor("Vector2", f.G("x"), f.G("y")), q_4), Gv.S("solo"), Gv.B(false), Gv.S("off"), Gv.Null));
          k2 = (k2 + Gv.I(1L));
        }
      }
    }
    Gv valves = Gd.Arr();
    foreach (Gv it_3 in (items).Iter()) {
      if (Gv.Eq(it_3.G("type"), Gv.S("valve"))) {
        valves.C("append", it_3);
      }
    }
    Gv JS = Gd.Dict();
    foreach (Gv q_5 in (jv).Iter()) {
      JS = JS.SI(K(q_5), Gv.B(true));
    }
    foreach (Gv f_2 in (items).Iter()) {
      if (Gv.Eq(f_2.G("type"), Gv.S("fitting"))) {
        JS = JS.SI(K(Gd.Ctor("Vector2", f_2.G("x"), f_2.G("y"))), Gv.B(true));
        foreach (Gv q_6 in (fit_ports(f_2)).Iter()) {
          JS = JS.SI(K(q_6), Gv.B(true));
        }
      }
    }
    foreach (Gv o in (main.G("outlets")).Iter()) {
      JS = JS.SI(K(o.G("front")), Gv.B(true));
      JS = JS.SI(K(o.G("back")), Gv.B(true));
    }
    Gv offd = Gd.Fn((_a) => { Gv p = Gd.A(_a, 0); Gv s = Gd.A(_a, 1); return Gv.B((Gv.Ne(p.G("off"), Gv.Null) && (p.G("off").C("get", s, Gv.B(false))).T())); return Gv.Null; });
    Gv cnt = Gd.Dict();
    foreach (Gv p_2 in (pipes).Iter()) {
      if ((((p_2.G("solo")).T() || (offd.C("call", p_2, Gv.S("a"))).T()) || (offd.C("call", p_2, Gv.S("b"))).T())) {
        continue;
      }
      foreach (Gv q_7 in (Gd.Arr(p_2.G("pts").I(Gv.I(0L)), p_2.G("pts").I((p_2.G("pts").C("size") - Gv.I(1L))))).Iter()) {
        cnt = cnt.SI(K(q_7), (cnt.C("get", K(q_7), Gv.I(0L)) + Gv.I(1L)));
      }
    }
    foreach (Gv k_2 in (cnt).Iter()) {
      if (Gv.Ge(cnt.I(k_2), Gv.I(2L))) {
        JS = JS.SI(k_2, Gv.B(true));
      }
    }
    Gv ends = Gd.Arr();
    foreach (Gv p_3 in (pipes).Iter()) {
      if ((((p_3.G("solo")).T() || (offd.C("call", p_3, Gv.S("a"))).T()) || (offd.C("call", p_3, Gv.S("b"))).T())) {
        continue;
      }
      Gv a0 = p_3.G("pts").I(Gv.I(0L));
      Gv b0 = p_3.G("pts").I((p_3.G("pts").C("size") - Gv.I(1L)));
      if ((!(JS.C("has", K(a0))).T() && !(JS.C("has", K(b0))).T())) {
        continue;
      }
      ends.C("append", Gd.Dict(Gv.S("pid"), p_3.G("id"), Gv.S("pt"), a0));
      ends.C("append", Gd.Dict(Gv.S("pid"), p_3.G("id"), Gv.S("pt"), b0));
    }
    Gv nodes = Gd.Dict();
    Gv edges = Gd.Arr();
    Gv pumps = Gd.Arr();
    Gv inlets = Gd.Arr();
    Gv chillers = Gd.Arr();
    Gv vessels = Gd.Arr();
    Gv sprays = Gd.Arr();
    foreach (Gv it_4 in (items).Iter()) {
      if (Gv.Eq(it_4.G("type"), Gv.S("pump"))) {
        pumps.C("append", it_4);
      } else if (Gv.Eq(it_4.G("type"), Gv.S("vessel"))) {
        if (Gv.Eq(it_4.C("get", Gv.S("shape")), Gv.S("inlet"))) {
          inlets.C("append", it_4);
        }
        if ((Gv.Eq(it_4.C("get", Gv.S("shape")), Gv.S("chiller")) && !(it_4.C("get", Gv.S("rot"), Gv.I(0L))).T())) {
          chillers.C("append", it_4);
        }
        if (Gv.Ne(it_4.C("get", Gv.S("shape")), Gv.S("platform"))) {
          vessels.C("append", it_4);
        }
      } else if ((Gv.Eq(it_4.G("type"), Gv.S("spray")) || Gv.Eq(it_4.G("type"), Gv.S("nozzle")))) {
        sprays.C("append", it_4);
      }
    }
    Gv cut = Gd.Dict();
    Gv tapPts = Gd.Dict();
    Gv tapPipes = Gd.Dict();
    Gv chilPts = Gd.Dict();
    foreach (Gv p_4 in (pipes).Iter()) {
      Gv ref_ = Gd.Arr(p_4.G("pts").I(Gv.I(0L)));
      foreach (Gv i in ((p_4.G("pts").C("size") - Gv.I(1L))).Iter()) {
        Gv a = p_4.G("pts").I(i);
        Gv b = p_4.G("pts").I((i + Gv.I(1L)));
        Gv extra = Gd.Arr();
        if (!(p_4.G("solo")).T()) {
          foreach (Gv e_2 in (ends).Iter()) {
            if ((Gv.Eq(e_2.G("pid"), p_4.G("id")) || !(JS.C("has", K(e_2.G("pt")))).T())) {
              continue;
            }
            Gv t = on_seg_t(e_2.G("pt"), a, b, Gv.F(5.0));
            if (Gv.Gt(t, Gv.I(0L))) {
              extra.C("append", Gd.Arr(t, e_2.G("pt")));
            }
          }
        }
        foreach (Gv q_8 in (jv).Iter()) {
          Gv t2 = on_seg_t(q_8, a, b);
          if (Gv.Gt(t2, Gv.I(0L))) {
            extra.C("append", Gd.Arr(t2, q_8));
          }
        }
        foreach (Gv v in (valves).Iter()) {
          Gv t3 = on_seg_t(Gd.Ctor("Vector2", v.G("x"), v.G("y")), a, b);
          if (Gv.Gt(t3, Gv.I(0L))) {
            extra.C("append", Gd.Arr(t3, Gd.Ctor("Vector2", v.G("x"), v.G("y"))));
          }
        }
        Gv d = (b - a);
        Gv l2 = d.C("length_squared");
        if (Gv.Ge(l2, Gv.I(1L))) {
          foreach (Gv u in (chillers).Iter()) {
            Gv r = Gd.Ctor("Rect2", u.G("x"), u.G("y"), u.G("w"), u.G("h"));
            if (((r.C("has_point", a)).T() || (r.C("has_point", b)).T())) {
              continue;
            }
            Gv tt = ((((((((u.G("x") + (u.G("w") / Gv.F(2.0))) - a.G("x"))) * d.G("x")) + ((((u.G("y") + (u.G("h") / Gv.F(2.0))) - a.G("y"))) * d.G("y")))) / l2)).AsF();
            if ((Gv.Le(tt, Gv.F(0.001)) || Gv.Ge(tt, Gv.F(0.999)))) {
              continue;
            }
            Gv q_9 = ((a + (d * tt))).C("round");
            if (!(r.C("has_point", q_9)).T()) {
              continue;
            }
            extra.C("append", Gd.Arr(tt, q_9));
            chilPts = chilPts.SI(K(q_9), u.G("id"));
          }
          foreach (Gv u_2 in (inlets).Iter()) {
            Gv uc = Gd.Ctor("Vector2", (u_2.G("x") + (u_2.G("w") / Gv.F(2.0))), (u_2.G("y") + (u_2.G("h") / Gv.F(2.0))));
            Gv tt2 = (((((((uc.G("x") - a.G("x"))) * d.G("x")) + (((uc.G("y") - a.G("y"))) * d.G("y")))) / l2)).AsF();
            if ((Gv.Le(tt2, Gv.F(0.001)) || Gv.Ge(tt2, Gv.F(0.999)))) {
              continue;
            }
            Gv fp = (a + (d * tt2));
            if (Gv.Gt(fp.C("distance_to", uc), Gv.I(10L))) {
              continue;
            }
            Gv q2 = fp.C("round");
            extra.C("append", Gd.Arr(tt2, q2));
            tapPts = tapPts.SI(K(q2), Gv.B(true));
          }
          Gv sl = Gd.sqrt(l2);
          if (Gv.Ge(sl, Gv.I(56L))) {
            foreach (Gv u_3 in (pumps).Iter()) {
              Gv t4 = (((((((u_3.G("x") - a.G("x"))) * d.G("x")) + (((u_3.G("y") - a.G("y"))) * d.G("y")))) / l2)).AsF();
              if ((Gv.Lt((t4 * sl), Gv.I(26L)) || Gv.Lt((((Gv.I(1L) - t4)) * sl), Gv.I(26L)))) {
                continue;
              }
              Gv fp2 = (a + (d * t4));
              if (Gv.Gt(fp2.C("distance_to", Gd.Ctor("Vector2", u_3.G("x"), u_3.G("y"))), Gv.I(8L))) {
                continue;
              }
              Gv ud = (d / sl);
              Gv q1 = ((fp2 - (ud * Gv.I(24L)))).C("round");
              Gv q3 = ((fp2 + (ud * Gv.I(24L)))).C("round");
              Gv rr = Gd.deg_to_rad(Gd.Dyn("float", u_3.C("get", Gv.S("dir"), Gv.I(0L))));
              Gv dot = ((Gd.cos(rr) * ud.G("x")) + (Gd.sin(rr) * ud.G("y")));
              Gv fwd = Gv.B((Gv.Lt(Gd.absf(dot), Gv.F(0.3)) || Gv.Gt(dot, Gv.I(0L))));
              extra.C("append", Gd.Arr((t4 - (Gv.I(24L) / sl)), q1));
              extra.C("append", Gd.Arr((t4 + (Gv.I(24L) / sl)), q3));
              cut = cut.SI(((K(q1) + Gv.S(">")) + K(q3)), Gd.Dict(Gv.S("id"), u_3.G("id"), Gv.S("a"), ((fwd).T() ? Gv.S("in") : Gv.S("out")), Gv.S("b"), ((fwd).T() ? Gv.S("out") : Gv.S("in"))));
            }
          }
        }
        extra.C("sort_custom", Gd.Fn((_a) => { Gv x = Gd.A(_a, 0); Gv y = Gd.A(_a, 1); return Gv.B(Gv.Lt(x.I(Gv.I(0L)), y.I(Gv.I(0L)))); return Gv.Null; }));
        foreach (Gv x_2 in (extra).Iter()) {
          ref_.C("append", x_2.I(Gv.I(1L)));
        }
        ref_.C("append", b);
      }
      Gv pts_2 = Gd.Arr();
      foreach (Gv i_2 in (ref_.C("size")).Iter()) {
        if ((Gv.Eq(i_2, Gv.I(0L)) || Gv.Ne(K(ref_.I(i_2)), K(ref_.I((i_2 - Gv.I(1L))))))) {
          pts_2.C("append", ref_.I(i_2));
        }
      }
      Gv last = (pts_2.C("size") - Gv.I(1L));
      Gv sx = (Gv.S("#") + p_4.G("id"));
      Gv offA = offd.C("call", p_4, Gv.S("a"));
      Gv offB = offd.C("call", p_4, Gv.S("b"));
      Gv nk = Gd.Fn((_a) => { Gv i_3 = Gd.A(_a, 0); if ((Gv.Eq(i_3, Gv.I(0L)) && (offA).T())) {   return (Gv.S("#offA") + p_4.G("id")); } if ((Gv.Eq(i_3, last) && (offB).T())) {   return (Gv.S("#offB") + p_4.G("id")); } return ((JS.C("has", K(pts_2.I(i_3)))).T() ? Gv.S("") : sx); return Gv.Null; });
      Gv n0 = _node(nodes, pts_2.I(Gv.I(0L)), nk.C("call", Gv.I(0L)));
      n0 = n0.S("end", (n0.G("end") + Gv.I(1L)));
      if ((offA).T()) {
        n0 = n0.S("noatt", Gv.B(true));
      }
      Gv n1 = _node(nodes, pts_2.I(last), nk.C("call", last));
      n1 = n1.S("end", (n1.G("end") + Gv.I(1L)));
      if ((offB).T()) {
        n1 = n1.S("noatt", Gv.B(true));
      }
      foreach (Gv i_4 in (last).Iter()) {
        Gv na = _node(nodes, pts_2.I(i_4), nk.C("call", i_4));
        Gv nb = _node(nodes, pts_2.I((i_4 + Gv.I(1L))), nk.C("call", (i_4 + Gv.I(1L))));
        if ((chilPts.C("has", K(pts_2.I(i_4)))).T()) {
          na = na.S("_chil", chilPts.I(K(pts_2.I(i_4))));
        }
        if ((chilPts.C("has", K(pts_2.I((i_4 + Gv.I(1L)))))).T()) {
          nb = nb.S("_chil", chilPts.I(K(pts_2.I((i_4 + Gv.I(1L))))));
        }
        if ((tapPts.C("has", K(pts_2.I(i_4)))).T()) {
          tapPipes = tapPipes.SI(p_4.G("id"), Gv.B(true));
          if (!(na.G("_tap")).T()) {
            na = na.S("_tap", Gv.B(true));
            na = na.S("end", (na.G("end") + Gv.I(1L)));
          }
        }
        if ((tapPts.C("has", K(pts_2.I((i_4 + Gv.I(1L)))))).T()) {
          tapPipes = tapPipes.SI(p_4.G("id"), Gv.B(true));
          if (!(nb.G("_tap")).T()) {
            nb = nb.S("_tap", Gv.B(true));
            nb = nb.S("end", (nb.G("end") + Gv.I(1L)));
          }
        }
        Gv ck = ((K(pts_2.I(i_4)) + Gv.S(">")) + K(pts_2.I((i_4 + Gv.I(1L)))));
        if ((cut.C("has", ck)).T()) {
          na = na.S("end", (na.G("end") + Gv.I(1L)));
          nb = nb.S("end", (nb.G("end") + Gv.I(1L)));
          na = na.S("pside", Gd.Dict(Gv.S("id"), cut.I(ck).G("id"), Gv.S("side"), cut.I(ck).G("a")));
          nb = nb.S("pside", Gd.Dict(Gv.S("id"), cut.I(ck).G("id"), Gv.S("side"), cut.I(ck).G("b")));
          continue;
        }
        Gv ln = (Gd.Ctor("Vector2", nb.G("x"), nb.G("y")).C("distance_to", Gd.Ctor("Vector2", na.G("x"), na.G("y")))).AsF();
        if ((Gv.Ne(p_4.C("get", Gv.S("ringmul"), Gv.F(1.0)), Gv.F(1.0)) && Gv.Lt(Gd.absf((na.G("y") - nb.G("y"))), Gv.F(1.0)))) {
          ln = ((ln * p_4.G("ringmul"))).AsF();
        }
        Gv id = ((((p_4.G("id") + Gv.S("|")) + na.G("k")) + Gv.S("|")) + nb.G("k"));
        Gv o_2 = old.C("get", id);
        Gv e_3 = Gd.Dict(Gv.S("id"), id, Gv.S("pipe"), p_4.G("id"), Gv.S("medium"), p_4.G("medium"), Gv.S("a"), na.G("k"), Gv.S("b"), nb.G("k"), Gv.S("ax"), na.G("x"), Gv.S("ay"), na.G("y"), Gv.S("bx"), nb.G("x"), Gv.S("by"), nb.G("y"), Gv.S("len"), ln, Gv.S("dn"), p_4.G("dn"), Gv.S("cap"), Gd.maxf(Gv.F(0.3), (((ln / PX_PER_M) * PIPE_L_PER_M) * Gd.pow((p_4.G("dn") / Gv.F(50.0)), Gv.I(2L)))), Gv.S("c"), (Gv.Ne(o_2, Gv.Null) ? o_2.G("c") : lp()), Gv.S("q"), (Gv.Ne(o_2, Gv.Null) ? o_2.G("q") : Gd.Arr()), Gv.S("from"), (Gv.Ne(o_2, Gv.Null) ? o_2.G("from") : Gv.S("a")), Gv.S("flow"), Gv.I(0L), Gv.S("steam"), Gv.B(false));
        edges.C("append", e_3);
        na.G("edges").C("append", e_3);
        nb.G("edges").C("append", e_3);
      }
    }
    foreach (Gv v_2 in (valves).Iter()) {
      Gv n = nodes.C("get", K(Gd.Ctor("Vector2", v_2.G("x"), v_2.G("y"))));
      if (Gv.Eq(n, Gv.Null)) {
        foreach (Gv mm in (nodes.C("values")).Iter()) {
          if ((Gv.Eq(mm.G("x"), Gd.roundf(v_2.G("x"))) && Gv.Eq(mm.G("y"), Gd.roundf(v_2.G("y"))))) {
            n = mm;
            break;
          }
        }
      }
      if (Gv.Ne(n, Gv.Null)) {
        n = n.S("valve", v_2.G("id"));
      }
    }
    foreach (Gv o_3 in (main.G("outlets")).Iter()) {
      Gv n2 = nodes.C("get", K(o_3.G("front")));
      if (Gv.Ne(n2, Gv.Null)) {
        n2 = n2.S("valve", o_3.G("key"));
      }
    }
    Gv vesselNodes = Gd.Dict();
    Gv plateNodes = Gd.Dict();
    Gv pumpNodes = Gd.Dict();
    foreach (Gv n_2 in (nodes.C("values")).Iter()) {
      if ((!(n_2.G("end")).T() || (n_2.G("noatt")).T())) {
        continue;
      }
      Gv np = Gd.Ctor("Vector2", n_2.G("x"), n_2.G("y"));
      if (Gv.Ne(n_2.G("pside"), Gv.Null)) {
        n_2.G("att").C("append", Gd.Dict(Gv.S("kind"), Gv.S("pump"), Gv.S("id"), n_2.G("pside").G("id"), Gv.S("side"), n_2.G("pside").G("side")));
        if (!(pumpNodes.C("has", n_2.G("pside").G("id"))).T()) {
          pumpNodes = pumpNodes.SI(n_2.G("pside").G("id"), Gd.Dict(Gv.S("in"), Gd.Arr(), Gv.S("out"), Gd.Arr()));
        }
        pumpNodes.I(n_2.G("pside").G("id")).I(n_2.G("pside").G("side")).C("append", n_2.G("k"));
        continue;
      }
      Gv best = Gv.Null;
      Gv area = Gv.F(1000000000000.0);
      Gv bl = Gd.StaticGet("Vector2", "ZERO");
      foreach (Gv v_3 in (vessels).Iter()) {
        Gv l = v_local(v_3, np);
        if ((((Gv.Ge(l.G("x"), (v_3.G("x") - Gv.I(14L))) && Gv.Le(l.G("x"), ((v_3.G("x") + v_3.G("w")) + Gv.I(14L)))) && Gv.Ge(l.G("y"), (v_3.G("y") - Gv.I(14L)))) && Gv.Le(l.G("y"), ((v_3.G("y") + v_3.G("h")) + Gv.I(14L))))) {
          if (Gv.Lt((v_3.G("w") * v_3.G("h")), area)) {
            area = (v_3.G("w") * v_3.G("h"));
            best = v_3;
            bl = l;
          }
        }
      }
      Gv viaSpray = Gv.Null;
      if (Gv.Eq(best, Gv.Null)) {
        foreach (Gv sp in (sprays).Iter()) {
          if (Gv.Gt(Gd.Ctor("Vector2", sp.G("x"), sp.G("y")).C("distance_to", np), Gv.I(26L))) {
            continue;
          }
          foreach (Gv v_4 in (vessels).Iter()) {
            if (Gv.In(v_4.C("get", Gv.S("shape")), Gd.Arr(Gv.S("plate"), Gv.S("inlet")))) {
              continue;
            }
            if ((((Gv.Ge(sp.G("x"), (v_4.G("x") - Gv.I(40L))) && Gv.Le(sp.G("x"), ((v_4.G("x") + v_4.G("w")) + Gv.I(40L)))) && Gv.Ge(sp.G("y"), (v_4.G("y") - Gv.I(60L)))) && Gv.Le(sp.G("y"), ((v_4.G("y") + v_4.G("h")) + Gv.I(60L))))) {
              if (Gv.Lt((v_4.G("w") * v_4.G("h")), area)) {
                area = (v_4.G("w") * v_4.G("h"));
                best = v_4;
                viaSpray = sp.G("id");
                bl = Gd.Ctor("Vector2", Gd.clampf(sp.G("x"), v_4.G("x"), (v_4.G("x") + v_4.G("w"))), (Gv.Eq(sp.G("type"), Gv.S("nozzle")) ? ((v_4.G("y") + v_4.G("h"))) : (v_4.G("y") + Gv.I(2L))));
              }
            }
          }
          if (Gv.Ne(best, Gv.Null)) {
            break;
          }
        }
      }
      if ((Gv.Ne(best, Gv.Null) && Gv.Eq(viaSpray, Gv.Null))) {
        foreach (Gv sp_2 in (sprays).Iter()) {
          if ((Gv.Eq(sp_2.G("type"), Gv.S("spray")) && Gv.Le(Gd.Ctor("Vector2", sp_2.G("x"), sp_2.G("y")).C("distance_to", np), Gv.I(26L)))) {
            viaSpray = sp_2.G("id");
            break;
          }
        }
      }
      if (Gv.Ne(best, Gv.Null)) {
        if (Gv.Eq(best.C("get", Gv.S("shape")), Gv.S("plate"))) {
          Gv side = (Gv.Ne((Gv.B(Gv.Lt(bl.G("x"), (best.G("x") + (best.G("w") / Gv.F(2.0)))))), best.C("get", Gv.S("mirror"), Gv.B(false))) ? Gv.S("A") : Gv.S("B"));
          n_2.G("att").C("append", Gd.Dict(Gv.S("kind"), Gv.S("plate"), Gv.S("id"), best.G("id"), Gv.S("side"), side));
          if (!(plateNodes.C("has", best.G("id"))).T()) {
            plateNodes = plateNodes.SI(best.G("id"), Gd.Dict(Gv.S("A"), Gd.Arr(), Gv.S("B"), Gd.Arr()));
          }
          plateNodes.I(best.G("id")).I(side).C("append", n_2.G("k"));
        } else {
          Gv low = Gv.B((Gv.Gt(bl.G("y"), (best.G("y") + (best.G("h") * ((Gv.Eq(best.C("get", Gv.S("shape")), Gv.S("horiz")) ? Gv.F(0.45) : Gv.F(0.6)))))) || Gv.In(best.C("get", Gv.S("shape")), Gd.Arr(Gv.S("source"), Gv.S("drain"), Gv.S("box")))));
          Gv under = Gv.B(((low).T() && (Gv.Ne(best.C("get", Gv.S("bind")), Gv.S("sk")) || Gv.Gt(bl.G("y"), ((best.G("y") + best.G("h")) - Gv.F(22.0))))));
          n_2.G("att").C("append", Gd.Dict(Gv.S("kind"), Gv.S("vessel"), Gv.S("id"), best.G("id"), Gv.S("bind"), best.C("get", Gv.S("bind"), best.G("id")), Gv.S("low"), low, Gv.S("under"), under, Gv.S("spray"), viaSpray));
          if (!(vesselNodes.C("has", best.G("id"))).T()) {
            vesselNodes = vesselNodes.SI(best.G("id"), Gd.Arr());
          }
          vesselNodes.I(best.G("id")).C("append", Gd.Dict(Gv.S("k"), n_2.G("k"), Gv.S("low"), low));
        }
        continue;
      }
      foreach (Gv pu in (pumps).Iter()) {
        Gv dd = (np - Gd.Ctor("Vector2", pu.G("x"), pu.G("y")));
        if (Gv.Le(dd.C("length"), Gv.I(26L))) {
          Gv r2 = Gd.deg_to_rad(Gd.Dyn("float", pu.C("get", Gv.S("dir"), Gv.I(0L))));
          Gv side2 = (Gv.Ge(((dd.G("x") * Gd.cos(r2)) + (dd.G("y") * Gd.sin(r2))), Gv.I(0L)) ? Gv.S("out") : Gv.S("in"));
          n_2.G("att").C("append", Gd.Dict(Gv.S("kind"), Gv.S("pump"), Gv.S("id"), pu.G("id"), Gv.S("side"), side2));
          if (!(pumpNodes.C("has", pu.G("id"))).T()) {
            pumpNodes = pumpNodes.SI(pu.G("id"), Gd.Dict(Gv.S("in"), Gd.Arr(), Gv.S("out"), Gd.Arr()));
          }
          pumpNodes.I(pu.G("id")).I(side2).C("append", n_2.G("k"));
          break;
        }
      }
    }
    foreach (Gv gi in (items).Iter()) {
      if (Gv.Ne(gi.G("type"), Gv.S("gauge"))) {
        continue;
      }
      Gv G = main.C("gauge_geo", gi);
      if (!(G.G("free")).T()) {
        continue;
      }
      foreach (Gv pr in (Gd.Arr(Gd.Arr(Gv.S("t"), Gd.Ctor("Vector2", G.G("x"), (G.G("y1") - Gv.F(30.0)))), Gd.Arr(Gv.S("b"), Gd.Ctor("Vector2", G.G("x"), (G.G("y2") + Gv.F(30.0)))))).Iter()) {
        foreach (Gv n_3__it in (nodes.C("values")).Iter()) {
          Gv n_3 = n_3__it;
          if (Gv.Le(Gd.Ctor("Vector2", n_3.G("x"), n_3.G("y")).C("distance_to", pr.I(Gv.I(1L))), Gv.F(14.0))) {
            n_3 = n_3.SI(Gv.S("gauge"), Gv.B(true));
            n_3 = n_3.SI(Gv.S("gaugeId"), gi.G("id"));
            n_3 = n_3.SI(Gv.S("gaugeEnd"), pr.I(Gv.I(0L)));
          }
        }
      }
    }
    foreach (Gv n_4__it in (nodes.C("values")).Iter()) {
      Gv n_4 = n_4__it;
      if ((((((n_4.G("att").C("size")).T() || Gv.Ne(n_4.G("edges").C("size"), Gv.I(1L))) || !(n_4.G("end")).T()) || (n_4.G("_tap")).T()) || (n_4.C("get", Gv.S("gauge"), Gv.B(false))).T())) {
        continue;
      }
      Gv np2 = Gd.Ctor("Vector2", n_4.G("x"), n_4.G("y"));
      Gv touch = Gv.B(false);
      foreach (Gv e_4 in (edges).Iter()) {
        if (Gv.Eq(e_4, n_4.G("edges").I(Gv.I(0L)))) {
          continue;
        }
        Gv a_2 = Gd.Ctor("Vector2", e_4.G("ax"), e_4.G("ay"));
        Gv b_2 = Gd.Ctor("Vector2", e_4.G("bx"), e_4.G("by"));
        if (Gv.Le(Gd.Static("Geometry2D", "get_closest_point_to_segment", np2, a_2, b_2).C("distance_to", np2), Gv.I(8L))) {
          touch = Gv.B(true);
          break;
        }
      }
      if (!(touch).T()) {
        foreach (Gv pu_2 in (pumps).Iter()) {
          if (Gv.Le(np2.C("distance_to", Gd.Ctor("Vector2", pu_2.G("x"), pu_2.G("y"))), Gv.I(40L))) {
            touch = Gv.B(true);
            break;
          }
        }
      }
      if (!(touch).T()) {
        foreach (Gv v_5 in (vessels).Iter()) {
          if ((((Gv.Ge(np2.G("x"), (v_5.G("x") - Gv.I(30L))) && Gv.Le(np2.G("x"), ((v_5.G("x") + v_5.G("w")) + Gv.I(30L)))) && Gv.Ge(np2.G("y"), (v_5.G("y") - Gv.I(30L)))) && Gv.Le(np2.G("y"), ((v_5.G("y") + v_5.G("h")) + Gv.I(30L))))) {
            touch = Gv.B(true);
            break;
          }
        }
      }
      if ((!(touch).T() && (tapPipes.C("has", n_4.G("edges").I(Gv.I(0L)).G("pipe"))).T())) {
        touch = Gv.B(true);
      }
      n_4 = n_4.S("openEnd", Gv.B(!(touch).T()));
    }
    Gv eset = Gd.Dict();
    foreach (Gv e_5__it in (edges).Iter()) {
      Gv e_5 = e_5__it;
      e_5 = e_5.S("pts", Gd.Arr(Gd.Ctor("Vector2", e_5.G("ax"), e_5.G("ay")), Gd.Ctor("Vector2", e_5.G("bx"), e_5.G("by"))));
      e_5 = e_5.S("ids", Gd.Arr(e_5.G("id")));
      eset = eset.SI(e_5.G("id"), e_5);
    }
    foreach (Gv n_5__it in (nodes.C("values")).Iter()) {
      Gv n_5 = n_5__it;
      if (((((((Gv.Ne(n_5.G("edges").C("size"), Gv.I(2L)) || (n_5.G("att").C("size")).T()) || Gv.Ne(n_5.G("valve"), Gv.Null)) || (n_5.G("end")).T()) || (n_5.G("_tap")).T()) || Gv.Ne(n_5.G("_chil"), Gv.S(""))) || (n_5.G("openEnd")).T())) {
        continue;
      }
      Gv e1 = n_5.G("edges").I(Gv.I(0L));
      Gv e2 = n_5.G("edges").I(Gv.I(1L));
      if ((Gv.Eq(e1, e2) || Gv.Ne(e1.G("medium"), e2.G("medium")))) {
        continue;
      }
      Gv p1 = e1.G("pts").C("duplicate");
      Gv u_4 = e1.G("a");
      if (Gv.Eq(e1.G("a"), n_5.G("k"))) {
        p1.C("reverse");
        u_4 = e1.G("b");
      }
      Gv p2 = e2.G("pts").C("duplicate");
      Gv w = e2.G("b");
      if (Gv.Eq(e2.G("b"), n_5.G("k"))) {
        p2.C("reverse");
        w = e2.G("a");
      }
      if (Gv.Eq(u_4, w)) {
        continue;
      }
      Gv nu = nodes.I(u_4);
      Gv nw = nodes.I(w);
      Gv ids = (e1.G("ids") + e2.G("ids"));
      Gv c2 = e1.G("c");
      addTo(c2, e2.G("c"), Gv.F(0.0));
      Gv q1_2 = e1.G("q").C("duplicate");
      if (Gv.Eq(e1.G("a"), n_5.G("k"))) {
        q1_2.C("reverse");
      }
      Gv q2_2 = e2.G("q").C("duplicate");
      if (Gv.Eq(e2.G("b"), n_5.G("k"))) {
        q2_2.C("reverse");
      }
      Gv ne = Gd.Dict(Gv.S("id"), Gv.S("~").C("join", Gd.Ctor("PackedStringArray", ids)), Gv.S("ids"), ids, Gv.S("pipe"), e1.G("pipe"), Gv.S("medium"), e1.G("medium"), Gv.S("a"), u_4, Gv.S("b"), w, Gv.S("ax"), nu.G("x"), Gv.S("ay"), nu.G("y"), Gv.S("bx"), nw.G("x"), Gv.S("by"), nw.G("y"), Gv.S("len"), (e1.G("len") + e2.G("len")), Gv.S("dn"), e1.G("dn"), Gv.S("cap"), (e1.G("cap") + e2.G("cap")), Gv.S("c"), c2, Gv.S("q"), (q1_2 + q2_2), Gv.S("from"), Gv.S("a"), Gv.S("flow"), Gv.I(0L), Gv.S("steam"), Gv.B(false), Gv.S("pts"), (p1 + p2.C("slice", Gv.I(1L))));
      eset.C("erase", e1.G("id"));
      eset.C("erase", e2.G("id"));
      eset = eset.SI(ne.G("id"), ne);
      nu.G("edges").C("erase", e1);
      nu.G("edges").C("append", ne);
      nw.G("edges").C("erase", e2);
      nw.G("edges").C("append", ne);
      n_5 = n_5.S("edges", Gd.Arr());
    }
    edges = eset.C("values");
    foreach (Gv e_6__it in (edges).Iter()) {
      Gv e_6 = e_6__it;
      if ((old.C("has", e_6.G("id"))).T()) {
        e_6 = e_6.S("c", old.I(e_6.G("id")).G("c"));
        e_6 = e_6.S("q", old.I(e_6.G("id")).G("q"));
        e_6 = e_6.S("from", old.I(e_6.G("id")).G("from"));
      }
    }
    Gv hasSteam = Gv.B(false);
    foreach (Gv p_5 in (pipes).Iter()) {
      if (Gv.Eq(p_5.G("medium"), Gv.S("steam"))) {
        hasSteam = Gv.B(true);
      }
    }
    Gv byBind = Gd.Dict();
    Gv sws = Gd.Arr();
    Gv gvs = Gd.Arr();
    foreach (Gv i_5 in (items).Iter()) {
      Gv b_3 = i_5.C("get", Gv.S("bind"));
      if ((Gv.Ne(b_3, Gv.Null) && Gv.Ne(b_3, Gv.S("")))) {
        if (!(byBind.C("has", b_3)).T()) {
          byBind = byBind.SI(b_3, Gd.Arr());
        }
        byBind.I(b_3).C("append", i_5);
      }
      if ((Gv.Eq(i_5.G("type"), Gv.S("sw")) && Gv.Ne(i_5.C("get", Gv.S("k"), Gv.S("")), Gv.S("pump")))) {
        sws.C("append", i_5);
      }
      if ((Gv.Eq(i_5.G("type"), Gv.S("vessel")) && Gv.In(i_5.C("get", Gv.S("bind")), Gd.Arr(Gv.S("rvp"), Gv.S("mp"), Gv.S("sk"), Gv.S("wp"), Gv.S("hlt"), Gv.S("lv"))))) {
        gvs.C("append", i_5);
      }
    }
    net = Gd.Dict(Gv.S("chilInline"), Gv.B(Gv.Gt(chilPts.C("size"), Gv.I(0L))), Gv.S("nodes"), nodes, Gv.S("edges"), edges, Gv.S("byId"), byId, Gv.S("vesselNodes"), vesselNodes, Gv.S("plateNodes"), plateNodes, Gv.S("pumpNodes"), pumpNodes, Gv.S("hasSteam"), hasSteam, Gv.S("byBind"), byBind, Gv.S("sws"), sws, Gv.S("gravity"), gvs);
    valve_ver = (valve_ver + Gv.I(1L));
    return Gv.Null;
  }
  public Gv valve_open(Gv id = default(Gv)) {
    if ((main.G("outlet_open").C("has", id)).T()) {
      return main.G("outlet_open").I(id);
    }
    Gv v = net.G("byId").C("get", id);
    if ((Gv.Ne(v, Gv.Null) && (v.C("get", Gv.S("stuck"), Gv.B(false))).T())) {
      return v.C("get", Gv.S("stuckOpen"), Gv.B(true));
    }
    return Gv.B((Gv.Eq(v, Gv.Null) || Gv.Ne(v.C("get", Gv.S("open"), Gv.B(true)), Gv.B(false))));
    return Gv.Null;
  }
  public Gv _cached(Gv key = default(Gv)) {
    if (Gv.Ne(_cache_ver, valve_ver)) {
      _cache.C("clear");
      _cache_ver = valve_ver;
    }
    return _cache.C("get", key);
    return Gv.Null;
  }
  public Gv bfs(Gv starts = default(Gv), Gv mode = default(Gv), Gv pass2 = default(Gv)) {
    if (mode.k == 0) mode = Gv.S("liq");
    if (pass2.k == 0) pass2 = Gd.Arr();
    Gv key = ((((mode + Gv.S("|")) + Gv.S(",").C("join", Gd.Ctor("PackedStringArray", starts))) + Gv.S("|")) + Gv.S(",").C("join", Gd.Ctor("PackedStringArray", pass2)));
    Gv cr = _cached(key);
    if (Gv.Ne(cr, Gv.Null)) {
      return cr;
    }
    Gv prev = Gd.Dict();
    Gv q = Gd.Arr();
    Gv hits = Gd.Arr();
    Gv leaks = Gd.Arr();
    foreach (Gv k in (starts).Iter()) {
      if ((net.G("nodes").C("has", k)).T()) {
        prev = prev.SI(k, Gv.Null);
        q.C("append", k);
      }
    }
    Gv qi = Gv.I(0L);
    Gv steam = Gv.B(Gv.Eq(mode, Gv.S("steam")));
    while (Gv.Lt(qi, q.C("size"))) {
      Gv k_2 = q.I(qi);
      qi = (qi + Gv.I(1L));
      Gv n = net.G("nodes").I(k_2);
      if (((Gv.Eq(prev.I(k_2), Gv.Null) && Gv.Ne(n.G("valve"), Gv.Null)) && !(valve_open(n.G("valve"))).T())) {
        continue;
      }
      if (Gv.Ne(prev.I(k_2), Gv.Null)) {
        Gv stop = Gv.B(false);
        foreach (Gv a in (n.G("att")).Iter()) {
          if (Gv.Eq(a.G("kind"), Gv.S("vessel"))) {
            if ((n.G("_tap")).T()) {
              continue;
            }
            hits.C("append", Gd.Dict(Gv.S("k"), k_2, Gv.S("att"), a));
            if (Gv.Ne(a.G("spray"), Gv.Null)) {
              continue;
            }
            stop = Gv.B(true);
          } else if (Gv.Eq(a.G("kind"), Gv.S("pump"))) {
            if (Gv.Eq(prev.I(k_2).C("get", Gv.S("pump"), Gv.S("")), a.G("id"))) {
              continue;
            }
            Gv pb = net.G("byId").I(a.G("id"));
            if (((pass2.C("has", pb.C("get", Gv.S("bind"), Gv.S("")))).T() && Gv.Eq(a.G("side"), Gv.S("in")))) {
              foreach (Gv k2 in (net.G("pumpNodes").C("get", a.G("id"), Gd.Dict()).C("get", Gv.S("out"), Gd.Arr())).Iter()) {
                if (!(prev.C("has", k2)).T()) {
                  prev = prev.SI(k2, Gd.Dict(Gv.S("k"), k_2, Gv.S("pump"), a.G("id")));
                  q.C("append", k2);
                }
              }
            }
            stop = Gv.B(true);
          } else if (Gv.Eq(a.G("kind"), Gv.S("plate"))) {
            foreach (Gv k2_2 in (net.G("plateNodes").C("get", a.G("id"), Gd.Dict()).C("get", a.G("side"), Gd.Arr())).Iter()) {
              if (!(prev.C("has", k2_2)).T()) {
                prev = prev.SI(k2_2, Gd.Dict(Gv.S("k"), k_2, Gv.S("plate"), a.G("id"), Gv.S("side"), a.G("side")));
                q.C("append", k2_2);
              }
            }
          }
        }
        if ((stop).T()) {
          continue;
        }
        if ((!(steam).T() && (n.G("openEnd")).T())) {
          leaks.C("append", Gd.Dict(Gv.S("k"), k_2));
        }
      }
      foreach (Gv e in (n.G("edges")).Iter()) {
        if (Gv.Ne((Gv.B(Gv.Eq(e.G("medium"), Gv.S("steam")))), steam)) {
          continue;
        }
        Gv k2_3 = (Gv.Eq(e.G("a"), k_2) ? e.G("b") : e.G("a"));
        if ((prev.C("has", k2_3)).T()) {
          continue;
        }
        Gv n2 = net.G("nodes").I(k2_3);
        if ((Gv.Ne(n2.G("valve"), Gv.Null) && !(valve_open(n2.G("valve"))).T())) {
          continue;
        }
        prev = prev.SI(k2_3, Gd.Dict(Gv.S("k"), k_2, Gv.S("e"), e));
        q.C("append", k2_3);
      }
    }
    Gv r = Gd.Dict(Gv.S("hits"), hits, Gv.S("prev"), prev, Gv.S("leaks"), leaks, Gv.S("paths"), Gd.Dict());
    _cache = _cache.SI(key, r);
    return r;
    return Gv.Null;
  }
  public Gv drain_valves(Gv src_bind = default(Gv)) {
    Gv vs_ = items_by_bind(src_bind);
    if (!(vs_.C("size")).T()) {
      return Gv.Null;
    }
    Gv v = vs_.I(Gv.I(0L));
    Gv goals = Gd.Arr();
    foreach (Gv k in (net.G("nodes")).Iter()) {
      Gv n = net.G("nodes").I(k);
      if (((((n.C("get", Gv.S("openEnd"), Gv.B(false))).T() && Gv.Ge(n.G("y"), (v.G("y") + (v.G("h") * Gv.F(0.6))))) && Gv.Ge(n.G("x"), (v.G("x") - Gv.F(200.0)))) && Gv.Le(n.G("x"), ((v.G("x") + v.G("w")) + Gv.F(200.0))))) {
        goals.C("append", k);
      }
    }
    if (!(goals.C("size")).T()) {
      return Gv.Null;
    }
    return _route(starts_of(v.G("id"), Gv.B(true)), goals, Gv.S(""), Gv.I(-1L));
    return Gv.Null;
  }
  public Gv _flush_step(Gv dt = default(Gv)) {
    dt = (dt).AsF();
    Gv f = S.G("flush");
    if (!(f.G("on")).T()) {
      return Gv.Null;
    }
    foreach (Gv id in (f.G("vs")).Iter()) {
      if (!(valve_open(id)).T()) {
        once((Gv.S("flushclosed%d") % tb(Gv.I(600L))), Gv.S("Proplach pod síty neteče – na cestě je zavřená klapka."), Gv.S("warn"));
        return Gv.Null;
      }
    }
    if (!(f.G("vs").C("size")).T()) {
      return Gv.Null;
    }
    Gv L = (((f.G("rate") * dt) / Gv.F(60.0))).AsF();
    if (Gv.Lt(S.G("hlt").G("w"), L)) {
      once(Gv.S("flushempty"), Gv.S("Proplach: v bojleru dochází voda."), Gv.S("bad"));
      return Gv.Null;
    }
    S = S.S("hlt", S.G("hlt").S("w", (S.G("hlt").G("w") - L)));
    S = S.S("usedHot", (S.G("usedHot") + L));
    S = S.S("usedWater", (S.G("usedWater") + L));
    S = S.S("drainL", (S.G("drainL") + L));
    f = f.S("L", (f.G("L") + L));
    S = S.S("_skLow", Gv.I(1L));
    Gv sk = S.G("sk");
    if (Gv.Gt(sk.C("get", Gv.S("dep"), Gv.F(0.0)), Gv.F(0.0))) {
      sk = sk.SI(Gv.S("dep"), Gd.maxf(Gv.F(0.0), (sk.G("dep") - (L / f.G("target")))));
    }
    if ((Gv.Ge(f.G("L"), f.G("target")) && !(S.G("once").C("has", Gv.S("flushDone"))).T())) {
      S = S.S("once", S.G("once").SI(Gv.S("flushDone"), Gv.I(1L)));
      logm((Gv.S("Proplach pod síty hotov: %d L horké vody z bojleru odtekly do kanálu, nános ze dna je smytý.") % Gd.roundi(f.G("L"))), Gv.S("info"));
    }
    return Gv.Null;
  }
  public Gv route_valves(Gv src_bind = default(Gv), Gv dst_bind = default(Gv), Gv pass2 = default(Gv), Gv where = default(Gv), Gv plate = default(Gv)) {
    if (pass2.k == 0) pass2 = Gd.Arr();
    if (where.k == 0) where = Gv.I(-1L);
    if (plate.k == 0) plate = Gv.S("");
    Gv start = Gd.Arr();
    foreach (Gv v in (items_by_bind(src_bind)).Iter()) {
      start.C("append_array", starts_of(v.G("id"), Gv.B(Gv.Ne(src_bind, Gv.S("vodovod")))));
    }
    Gv all = Gd.Arr();
    last_route = Gd.Arr();
    foreach (Gv pb in (pass2).Iter()) {
      Gv pumps_ = items_by_bind(pb);
      if (!(pumps_.C("size")).T()) {
        return Gv.Null;
      }
      Gv pn = net.G("pumpNodes").C("get", pumps_.I(Gv.I(0L)).G("id"));
      if (Gv.Eq(pn, Gv.Null)) {
        return Gv.Null;
      }
      Gv r1 = _route(start, pn.G("in"), Gv.S(""), Gv.I(-1L));
      if (Gv.Eq(r1, Gv.Null)) {
        return Gv.Null;
      }
      all.C("append_array", r1);
      start = pn.G("out");
    }
    if ((Gv.Ne(plate, Gv.S("")) && (net.G("plateNodes").C("has", plate)).T())) {
      Gv bn = net.G("plateNodes").I(plate).G("B");
      Gv r3 = _route(start, bn, Gv.S(""), Gv.I(-1L));
      if (Gv.Eq(r3, Gv.Null)) {
        return Gv.Null;
      }
      all.C("append_array", r3);
      Gv arrive = last_goal;
      start = Gd.Arr();
      foreach (Gv k in (bn).Iter()) {
        if (Gv.Ne(k, arrive)) {
          start.C("append", k);
        }
      }
    }
    Gv r2 = _route(start, Gd.Arr(), dst_bind, where);
    if (Gv.Eq(r2, Gv.Null)) {
      return Gv.Null;
    }
    all.C("append_array", r2);
    return all;
    return Gv.Null;
  }
  public Gv _route(Gv starts = default(Gv), Gv goal_keys = default(Gv), Gv dst_bind = default(Gv), Gv where = default(Gv)) {
    if (where.k == 0) where = Gv.I(-1L);
    Gv prev = Gd.Dict();
    Gv q = Gd.Arr();
    foreach (Gv k in (starts).Iter()) {
      prev = prev.SI(k, Gv.Null);
      q.C("append", k);
    }
    Gv qi = Gv.I(0L);
    Gv goal = Gv.Null;
    foreach (Gv k_2 in (starts).Iter()) {
      if ((goal_keys.C("has", k_2)).T()) {
        goal = k_2;
      }
    }
    while ((Gv.Lt(qi, q.C("size")) && Gv.Eq(goal, Gv.Null))) {
      Gv k_3 = q.I(qi);
      qi = (qi + Gv.I(1L));
      Gv n = net.G("nodes").I(k_3);
      if (Gv.Ne(prev.I(k_3), Gv.Null)) {
        Gv stop = Gv.B(false);
        if ((goal_keys.C("has", k_3)).T()) {
          goal = k_3;
          break;
        }
        foreach (Gv a in (n.G("att")).Iter()) {
          if ((Gv.Eq(a.G("kind"), Gv.S("vessel")) && !(n.G("_tap")).T())) {
            if (((Gv.Ne(dst_bind, Gv.S("")) && Gv.Eq(a.G("bind"), dst_bind)) && (Gv.Lt(where, Gv.I(0L)) || Gv.Eq(Gd.Dyn("int", a.G("low")), where)))) {
              goal = k_3;
              break;
            }
            if (Gv.Eq(a.G("spray"), Gv.Null)) {
              stop = Gv.B(true);
            }
          } else if (Gv.Eq(a.G("kind"), Gv.S("pump"))) {
            stop = Gv.B(true);
          } else if (Gv.Eq(a.G("kind"), Gv.S("plate"))) {
            foreach (Gv k2 in (net.G("plateNodes").C("get", a.G("id"), Gd.Dict()).C("get", a.G("side"), Gd.Arr())).Iter()) {
              if (!(prev.C("has", k2)).T()) {
                prev = prev.SI(k2, Gd.Dict(Gv.S("k"), k_3, Gv.S("plate"), a.G("id")));
                q.C("append", k2);
              }
            }
          }
        }
        if ((Gv.Ne(goal, Gv.Null) || (stop).T())) {
          continue;
        }
      }
      foreach (Gv e in (n.G("edges")).Iter()) {
        if (Gv.Eq(e.G("medium"), Gv.S("steam"))) {
          continue;
        }
        Gv k2_2 = (Gv.Eq(e.G("a"), k_3) ? e.G("b") : e.G("a"));
        if ((prev.C("has", k2_2)).T()) {
          continue;
        }
        prev = prev.SI(k2_2, Gd.Dict(Gv.S("k"), k_3, Gv.S("e"), e));
        q.C("append", k2_2);
      }
    }
    if (Gv.Eq(goal, Gv.Null)) {
      return Gv.Null;
    }
    Gv vs = Gd.Arr();
    Gv cur = goal;
    last_goal = goal;
    while (Gv.Ne(cur, Gv.Null)) {
      last_route.C("append", cur);
      Gv n2 = net.G("nodes").I(cur);
      if (Gv.Ne(n2.G("valve"), Gv.Null)) {
        vs.C("append", n2.G("valve"));
      }
      Gv p = prev.C("get", cur);
      cur = (Gv.Ne(p, Gv.Null) ? p.G("k") : Gv.Null);
    }
    vs.C("reverse");
    return vs;
    return Gv.Null;
  }
  public Gv path_to(Gv r = default(Gv), Gv k = default(Gv)) {
    if ((r.G("paths").C("has", k)).T()) {
      return r.G("paths").I(k);
    }
    Gv prev = r.G("prev");
    Gv out_ = Gd.Arr();
    Gv cur = k;
    while (Gv.Ne(prev.C("get", cur), Gv.Null)) {
      Gv p = prev.I(cur);
      if ((p.C("has", Gv.S("e"))).T()) {
        out_.C("append", Gd.Dict(Gv.S("e"), p.G("e"), Gv.S("to"), cur));
      } else if ((p.C("has", Gv.S("plate"))).T()) {
        out_.C("append", Gd.Dict(Gv.S("plate"), p.G("plate"), Gv.S("side"), p.G("side")));
      }
      cur = p.G("k");
    }
    out_.C("reverse");
    r = r.S("paths", r.G("paths").SI(k, out_));
    return out_;
    return Gv.Null;
  }
  public Gv path_len(Gv path = default(Gv)) {
    Gv L = Gv.F(0.0);
    foreach (Gv s in (path).Iter()) {
      if ((s.C("has", Gv.S("e"))).T()) {
        L = (L + s.G("e").G("len"));
      }
    }
    return (L).AsF();
    return Gv.Null;
  }
  public Gv path_pct(Gv path = default(Gv)) {
    Gv f = Gv.F(1.0);
    foreach (Gv s in (path).Iter()) {
      if (!(s.C("has", Gv.S("to"))).T()) {
        continue;
      }
      Gv n = net.G("nodes").C("get", s.G("to"));
      if ((Gv.Eq(n, Gv.Null) || Gv.Eq(n.G("valve"), Gv.Null))) {
        continue;
      }
      Gv v = net.G("byId").C("get", n.G("valve"));
      if ((Gv.Ne(v, Gv.Null) && Gv.Eq(v.C("get", Gv.S("kind"), Gv.S("")), Gv.S("skrt")))) {
        f = Gd.minf(f, throttle_flow(Gd.Dyn("float", v.C("get", Gv.S("pct"), Gv.I(50L)))));
      } else if ((Gv.Ne(v, Gv.Null) && (v.C("has", Gv.S("part"))).T())) {
        f = Gd.minf(f, throttle_flow(Gd.Dyn("float", v.G("part"))));
      }
    }
    return (f).AsF();
    return Gv.Null;
  }
  public Gv throttle_flow(Gv pct = default(Gv)) {
    pct = (pct).AsF();
    Gv p = (Gd.clampf((pct / Gv.F(100.0)), Gv.F(0.0), Gv.F(1.0))).AsF();
    if (Gv.Le(p, Gv.F(0.0))) {
      return (Gv.F(0.0)).AsF();
    }
    Gv K = ((p * p)).AsF();
    return ((Gv.F(1.0) / Gd.sqrt((Gv.F(1.0) + (Gv.F(0.05) * (((Gv.F(1.0) / ((K * K))) - Gv.F(1.0)))))))).AsF();
    return Gv.Null;
  }
  public Gv starts_of(Gv vid = default(Gv), Gv low_only = default(Gv)) {
    if (low_only.k == 0) low_only = Gv.B(false);
    Gv o = Gd.Arr();
    foreach (Gv x in (net.G("vesselNodes").C("get", vid, Gd.Arr())).Iter()) {
      if ((!(low_only).T() || (x.G("low")).T())) {
        o.C("append", x.G("k"));
      }
    }
    return o;
    return Gv.Null;
  }
  public Gv items_by_bind(Gv b = default(Gv)) {
    return net.G("byBind").C("get", b, Gd.Arr());
    return Gv.Null;
  }
  public Gv _similar(Gv c = default(Gv), Gv p = default(Gv)) {
    if (Gv.Gt(Gd.absf((c.G("T") - p.G("T"))), Gv.F(0.4))) {
      return Gv.B(false);
    }
    Gv ec = (((((c.G("f") + c.G("dx")) + c.G("ld")) + c.G("ns")) + c.G("sg"))).AsF();
    Gv sc_ = ((((c.G("g") + c.G("su")) + c.G("sc")) + c.G("ni"))).AsF();
    Gv ep = (((((p.G("f") + p.G("dx")) + p.G("ld")) + p.G("ns")) + p.G("sg"))).AsF();
    Gv sp_ = ((((p.G("g") + p.G("su")) + p.G("sc")) + p.G("ni"))).AsF();
    Gv vc = (((c.G("w") + (ec * Gv.F(0.62))) + (sc_ * Gv.F(0.65)))).AsF();
    Gv vp = (((p.G("w") + (ep * Gv.F(0.62))) + (sp_ * Gv.F(0.65)))).AsF();
    if ((Gv.Lt(vc, Gv.F(1e-09)) || Gv.Lt(vp, Gv.F(1e-09)))) {
      return Gv.B(true);
    }
    Gv ic = ((Gv.F(1.0) / vc)).AsF();
    Gv ip = ((Gv.F(1.0) / vp)).AsF();
    return Gv.B(((((Gv.Lt(Gd.absf(((c.G("w") * ic) - (p.G("w") * ip))), Gv.F(0.002)) && Gv.Lt(Gd.absf(((ec * ic) - (ep * ip))), Gv.F(0.0015))) && Gv.Lt(Gd.absf(((sc_ * ic) - (sp_ * ip))), Gv.F(0.002))) && Gv.Lt(Gd.absf(((c.G("fines") * ic) - (p.G("fines") * ip))), Gv.F(0.0004))) && Gv.Lt(Gd.absf(((c.G("aA") * ic) - (p.G("aA") * ip))), Gv.F(0.0005))));
    return Gv.Null;
  }
  public Gv _agg_sub(Gv agg = default(Gv), Gv b = default(Gv)) {
    Gv Ct = (cC(agg)).AsF();
    Gv Cb = (cC(b)).AsF();
    if (Gv.Gt((Ct - Cb), Gv.F(1e-09))) {
      agg = agg.S("T", ((((Ct * agg.G("T")) - (Cb * b.G("T")))) / ((Ct - Cb))));
    }
    foreach (Gv k in (LIQ).Iter()) {
      agg = agg.SI(k, Gd.maxf(Gv.F(0.0), (agg.I(k) - b.I(k))));
    }
    foreach (Gv k_2 in (SOL).Iter()) {
      agg = agg.SI(k_2, Gd.maxf(Gv.F(0.0), (agg.I(k_2) - b.I(k_2))));
    }
    return Gv.Null;
  }
  public Gv edge_push(Gv e = default(Gv), Gv part = default(Gv), Gv from_a = default(Gv)) {
    Gv out_ = lp();
    if ((Gv.Le(vol(part), Gv.F(1e-12)) && Gv.Le(part.G("w"), Gv.F(1e-12)))) {
      return out_;
    }
    Gv q = e.G("q");
    if (Gv.Eq(q.C("size"), Gv.I(1L))) {
      Gv q0 = q.I(Gv.I(0L));
      if (Gv.Le(Gd.absf((q0.G("T") - part.G("T"))), Gv.F(0.4))) {
        Gv ec = e.G("c");
        if ((Gv.Ge(((ec.G("w") + ((((((ec.G("f") + ec.G("dx")) + ec.G("ld")) + ec.G("ns")) + ec.G("sg"))) * Gv.F(0.62))) + (((((ec.G("g") + ec.G("su")) + ec.G("sc")) + ec.G("ni"))) * Gv.F(0.65))), (e.G("cap") - Gv.F(1e-06))) && (_similar(q0, part)).T())) {
          return part;
        }
      }
    }
    Gv bp = part.C("duplicate");
    Gv b0 = Gv.Null;
    if ((q.C("size")).T()) {
      b0 = ((from_a).T() ? q.I(Gv.I(0L)) : q.I((q.C("size") - Gv.I(1L))));
    }
    if ((Gv.Ne(b0, Gv.Null) && (_similar(b0, bp)).T())) {
      addTo(b0, bp, Gv.F(0.0));
    } else if ((from_a).T()) {
      q.C("insert", Gv.I(0L), bp);
    } else {
      q.C("append", bp);
    }
    if (Gv.Gt(q.C("size"), Gv.I(4L))) {
      if ((from_a).T()) {
        addTo(q.I(Gv.I(1L)), q.I(Gv.I(0L)), Gv.F(0.0));
        q.C("remove_at", Gv.I(0L));
      } else {
        addTo(q.I((q.C("size") - Gv.I(2L))), q.I((q.C("size") - Gv.I(1L))), Gv.F(0.0));
        q.C("remove_at", (q.C("size") - Gv.I(1L)));
      }
    }
    addTo(e.G("c"), part, Gv.F(0.0));
    Gv over = ((vol(e.G("c")) - e.G("cap"))).AsF();
    while ((Gv.Gt(over, Gv.F(1e-09)) && Gv.Gt(q.C("size"), Gv.I(0L)))) {
      Gv idx = (((from_a).T() ? (q.C("size") - Gv.I(1L)) : Gv.I(0L))).AsI();
      Gv b = q.I(idx);
      Gv vb = (vol(b)).AsF();
      if (Gv.Le(vb, (over + Gv.F(1e-12)))) {
        q.C("remove_at", idx);
        _agg_sub(e.G("c"), b);
        addTo(out_, b, Gv.F(0.0));
        over = ((over - vb)).AsF();
      } else {
        Gv tk = takePart(b, over, Gv.F(1.0));
        _agg_sub(e.G("c"), tk);
        addTo(out_, tk, Gv.F(0.0));
        over = (Gv.F(0.0)).AsF();
      }
    }
    if ((q.C("is_empty")).T()) {
      Gv z = lp();
      z = z.S("T", e.G("c").G("T"));
      foreach (Gv k in (z).Iter()) {
        e = e.S("c", e.G("c").SI(k, z.I(k)));
      }
    }
    return out_;
    return Gv.Null;
  }
  public Gv push_path(Gv path = default(Gv), Gv part = default(Gv), Gv Lmin = default(Gv), Gv m = default(Gv)) {
    Lmin = (Lmin).AsF();
    m = (m).AsF();
    foreach (Gv st in (path).Iter()) {
      if ((st.C("has", Gv.S("plate"))).T()) {
        if (Gv.Eq(st.G("side"), Gv.S("A"))) {
          part = part.S("T", plate_cool(st.G("plate"), part.G("T"), Lmin, m));
        }
        continue;
      }
      if (!(st.C("has", Gv.S("e"))).T()) {
        continue;
      }
      Gv e = st.G("e");
      if ((Gv.Le(liqVol(part), Gv.F(1e-09)) && Gv.Le(sol(part), Gv.F(1e-09)))) {
        break;
      }
      e = e.S("from", (Gv.Eq(st.G("to"), e.G("b")) ? Gv.S("a") : Gv.S("b")));
      e = e.S("flow", Gd.maxf(e.G("flow"), Lmin));
      part = edge_push(e, part, Gv.B(Gv.Eq(st.G("to"), e.G("b"))));
    }
    return part;
    return Gv.Null;
  }
  public Gv _same(Gv c = default(Gv), Gv p = default(Gv), Gv cap = default(Gv)) {
    cap = (cap).AsF();
    if (Gv.Gt(Gd.absf((c.G("T") - p.G("T"))), Gv.F(0.3))) {
      return Gv.B(false);
    }
    Gv vc = vol(c);
    if (Gv.Lt(vc, (cap - Gv.F(1e-06)))) {
      return Gv.B(false);
    }
    Gv vp = vol(p);
    if (Gv.Le(vp, Gv.F(1e-09))) {
      return Gv.B(false);
    }
    return Gv.B(((Gv.Lt(Gd.absf(((c.G("w") / vc) - (p.G("w") / vp))), Gv.F(0.002)) && Gv.Lt(Gd.absf(((sol(c) / vc) - (sol(p) / vp))), Gv.F(0.002))) && Gv.Lt(Gd.absf(((c.G("aA") / vc) - (p.G("aA") / vp))), Gv.F(0.0005))));
    return Gv.Null;
  }
  public Gv edges_only(Gv path = default(Gv)) {
    Gv o = Gd.Arr();
    foreach (Gv s in (path).Iter()) {
      if ((s.C("has", Gv.S("e"))).T()) {
        o.C("append", s);
      }
    }
    return o;
    return Gv.Null;
  }
  public Gv suction_path(Gv path = default(Gv)) {
    Gv o = Gd.Arr();
    foreach (Gv i in Gd.RangeIter((path.C("size") - Gv.I(1L)), Gv.I(-1L), Gv.I(-1L))) {
      Gv s = path.I(i);
      if ((s.C("has", Gv.S("e"))).T()) {
        o.C("append", Gd.Dict(Gv.S("e"), s.G("e"), Gv.S("to"), (Gv.Eq(s.G("to"), s.G("e").G("a")) ? s.G("e").G("b") : s.G("e").G("a"))));
      } else {
        o.C("append", s);
      }
    }
    return o;
    return Gv.Null;
  }
  public Gv plate_cool(Gv id = default(Gv), Gv Tin = default(Gv), Gv Lmin = default(Gv), Gv m = default(Gv)) {
    Tin = (Tin).AsF();
    Lmin = (Lmin).AsF();
    m = (m).AsF();
    Gv it = net.G("byId").I(id);
    Gv st = S.G("plates").C("get", id);
    Gv c = S.G("cfg");
    S = S.S("plateFlow", S.G("plateFlow").SI(id, (S.G("plateFlow").C("get", id, Gv.F(0.0)) + Lmin)));
    if (Gv.Eq(st, Gv.Null)) {
      once((Gv.S("nocool%s%d") % Gd.Arr(id, tb(Gv.I(900L)))), (Gv.S("%s: na straně chladiva nic neteče – mladina se nechladí.") % it.C("get", Gv.S("label"), Gv.S("Chladič"))), Gv.S("warn"));
      return (Tin).AsF();
    }
    Gv Cw = (((Lmin / Gv.F(60.0)) * Gv.F(1.04)) * Gv.F(4.0));
    if (Gv.Le(Cw, Gv.I(0L))) {
      return (Tin).AsF();
    }
    Gv UA0 = ((Gv.Eq(it.C("get", Gv.S("bind")), Gv.S("ch2")) ? c.G("UA2") : c.G("UA1"))).AsF();
    Gv Tout = Gv.F(0.0);
    Gv Qc = (Gd.maxf(Gv.F(1e-06), st.C("get", Gv.S("flow"), Gv.F(0.0)))).AsF();
    Gv qref = (c.C("get", Gv.S("plateRefQ"), Gv.F(80.0))).AsF();
    Gv UA = ((UA0 / (((Gv.F(0.5) * Gd.pow((qref / Gd.maxf(Gv.F(1.0), Lmin)), Gv.F(0.65))) + (Gv.F(0.5) * Gd.pow(((qref * Gv.F(1.1)) / Gd.maxf(Gv.F(1.0), Qc)), Gv.F(0.65))))))).AsF();
    Gv Cc = (((Qc / Gv.F(60.0)) * Gv.F(4.19))).AsF();
    Gv Cmin = (Gd.minf(Cw, Cc)).AsF();
    Gv Cr = ((Cmin / Gd.maxf(Cw, Cc))).AsF();
    Gv NTU = ((UA / Cmin)).AsF();
    Gv eff = Gv.F(0.0);
    if (Gv.Gt(Cr, Gv.F(0.999))) {
      eff = ((NTU / ((Gv.F(1.0) + NTU)))).AsF();
    } else {
      Gv x = (Gd.exp(((-NTU) * ((Gv.F(1.0) - Cr))))).AsF();
      eff = ((((Gv.F(1.0) - x)) / ((Gv.F(1.0) - (Cr * x))))).AsF();
    }
    Tout = ((Tin - (((eff * Cmin) * ((Tin - st.G("T")))) / Cw))).AsF();
    st = st.SI(Gv.S("Tcout"), (st.G("T") + (((eff * Cmin) * ((Tin - st.G("T")))) / Cc)));
    Gv duty = (Cw * ((Tin - Tout)));
    S = S.S("plateDuty", S.G("plateDuty").SI(id, (S.G("plateDuty").C("get", id, Gv.F(0.0)) + duty)));
    if ((Gv.Eq(st.G("kind"), Gv.S("ice")) && !(st.C("get", Gv.S("once"), Gv.B(false))).T())) {
      S = S.S("lv", S.G("lv").S("T", (S.G("lv").G("T") + (((duty * m) * Gv.F(60.0)) / ((Gd.maxf(Gv.F(300.0), S.G("lv").G("w")) * Gv.F(4.19)))))));
    }
    if ((Gv.Eq(it.C("get", Gv.S("bind")), Gv.S("ch1")) || (Gv.Eq(S.G("cool").G("T1"), Gv.Null) && Gv.Eq(st.G("kind"), Gv.S("water"))))) {
      S = S.S("cool", S.G("cool").S("T1", Tout));
    }
    return (Tout).AsF();
    return Gv.Null;
  }
  public Gv leak_where(Gv k = default(Gv)) {
    Gv n = net.G("nodes").C("get", k);
    if (Gv.Eq(n, Gv.Null)) {
      return Gv.S("");
    }
    Gv best = Gv.Null;
    Gv bd = Gv.F(1000000000.0);
    foreach (Gv it in (net.G("byId").C("values")).Iter()) {
      if (((!(Gv.Eq(it.G("type"), Gv.S("vessel")) || Gv.Eq(it.G("type"), Gv.S("pump"))) || Gv.Eq(it.C("get", Gv.S("label"), Gv.S("")), Gv.S(""))) || Gv.Eq(it.C("get", Gv.S("shape")), Gv.S("platform")))) {
        continue;
      }
      Gv d = Gd.Ctor("Vector2", ((it.G("x") + (it.C("get", Gv.S("w"), Gv.I(0L)) / Gv.F(2.0))) - n.G("x")), ((it.G("y") + (it.C("get", Gv.S("h"), Gv.I(0L)) / Gv.F(2.0))) - n.G("y"))).C("length");
      if (Gv.Lt(d, bd)) {
        bd = d;
        best = it;
      }
    }
    return (Gv.Ne(best, Gv.Null) ? (Gv.S("u „%s“") % best.G("label")) : Gv.S(""));
    return Gv.Null;
  }
  public Gv spill(Gv r = default(Gv), Gv lk = default(Gv), Gv part = default(Gv), Gv Lmin = default(Gv), Gv m = default(Gv), Gv what = default(Gv)) {
    Lmin = (Lmin).AsF();
    m = (m).AsF();
    Gv out_ = push_path(edges_only(path_to(r, lk.G("k"))), part, Lmin, m);
    S = S.S("spillL", (S.G("spillL") + vol(out_)));
    S = S.S("drainExt", (S.G("drainExt") + (((ext(out_) + out_.G("su")) + out_.G("sc")) + out_.G("ni"))));
    Gv n = net.G("nodes").I(lk.G("k"));
    S.G("leaks").C("append", Gd.Ctor("Vector2", n.G("x"), n.G("y")));
    once((Gv.S("leak%s%d") % Gd.Arr(lk.G("k"), tb(Gv.I(600L)))), (Gv.S("%s vytéká z otevřeného konce trubky %s – zavři klapku!") % Gd.Arr(what, leak_where(lk.G("k")))), Gv.S("bad"));
    return Gv.Null;
  }
  public Gv net_step(Gv dt = default(Gv)) {
    dt = (dt).AsF();
    Gv m = (dt / Gv.F(60.0));
    Gv c = S.G("cfg");
    Gv _n0 = Gd.Static("Time", "get_ticks_usec");
    Gv kcool = ((Gv.F(1.0) - Gd.exp(((-dt) / Gv.F(1800.0)))));
    foreach (Gv e__it in (net.G("edges")).Iter()) {
      Gv e = e__it;
      if (((!(e.G("flow")).T() && Gv.Gt(e.G("c").G("w"), Gv.F(0.01))) && !(e.G("steam")).T())) {
        e = e.S("c", e.G("c").S("T", (e.G("c").G("T") + (((c.G("ambientT") - e.G("c").G("T"))) * kcool))));
        foreach (Gv b__it in (e.G("q")).Iter()) {
          Gv b = b__it;
          b = b.S("T", (b.G("T") + (((c.G("ambientT") - b.G("T"))) * kcool)));
        }
      }
      e = e.S("flow", Gv.I(0L));
      e = e.S("steam", Gv.B(false));
    }
    S = S.S("_skTop", Gv.I(0L));
    S = S.S("_skLow", Gv.I(0L));
    S = S.S("_rvpIn", Gv.I(0L));
    S = S.S("leaks", Gd.Arr());
    S = S.S("streams", Gd.Dict());
    _flush_step(dt);
    S = S.S("lastDuty", S.G("plateDuty"));
    S = S.S("lastPlateFlow", S.G("plateFlow"));
    S = S.S("plates", Gd.Dict());
    S = S.S("plateDuty", Gd.Dict());
    S = S.S("plateFlow", Gd.Dict());
    S = S.S("flows", Gd.Arr());
    if ((prof_on).T()) {
      prof = prof.SI(Gv.S("n_trubky"), ((prof.C("get", Gv.S("n_trubky"), Gv.I(0L)) + Gd.Static("Time", "get_ticks_usec")) - _n0));
      _n0 = Gd.Static("Time", "get_ticks_usec");
    }
    S = S.S("steamOK", Gd.Dict(Gv.S("rvp"), Gv.B(true), Gv.S("mp"), Gv.B(true), Gv.S("hlt"), Gv.B(true)));
    Gv kotle = items_by_bind(Gv.S("kotel"));
    if (((net.G("hasSteam")).T() && (kotle.C("size")).T())) {
      Gv st = Gd.Arr();
      foreach (Gv k in (kotle).Iter()) {
        st.C("append_array", starts_of(k.G("id")));
      }
      Gv r = bfs(st, Gv.S("steam"));
      Gv reach = Gd.Dict();
      foreach (Gv hh in (r.G("hits")).Iter()) {
        reach = reach.SI(hh.G("att").G("bind"), hh);
      }
      foreach (Gv b_2 in (Gd.Arr(Gv.S("rvp"), Gv.S("mp"), Gv.S("hlt"))).Iter()) {
        Gv hasSt = Gv.B(false);
        foreach (Gv v in (items_by_bind(b_2)).Iter()) {
          foreach (Gv x in (net.G("vesselNodes").C("get", v.G("id"), Gd.Arr())).Iter()) {
            foreach (Gv e_2 in (net.G("nodes").I(x.G("k")).G("edges")).Iter()) {
              if (Gv.Eq(e_2.G("medium"), Gv.S("steam"))) {
                hasSt = Gv.B(true);
              }
            }
          }
        }
        S = S.S("steamOK", S.G("steamOK").SI(b_2, Gv.B(((reach.C("has", b_2)).T() || !(hasSt).T()))));
        Gv on = (Gv.Eq(b_2, Gv.S("hlt")) ? (Gv.B(Gv.Gt(S.G("hlt").G("heat"), Gv.I(0L)))) : (Gv.B(Gv.Gt(S.I(b_2).G("heat"), Gv.I(0L)))));
        if ((((reach.C("has", b_2)).T() && (on).T()) && (S.G("boiler").G("on")).T())) {
          foreach (Gv s__it in (path_to(r, reach.I(b_2).G("k"))).Iter()) {
            Gv s = s__it;
            if ((s.C("has", Gv.S("e"))).T()) {
              s = s.S("e", s.G("e").S("steam", Gv.B(true)));
            }
          }
        }
      }
    }
    if ((prof_on).T()) {
      prof = prof.SI(Gv.S("n_para"), ((prof.C("get", Gv.S("n_para"), Gv.I(0L)) + Gd.Static("Time", "get_ticks_usec")) - _n0));
      _n0 = Gd.Static("Time", "get_ticks_usec");
    }
    Gv plvs = items_by_bind(Gv.S("plv"));
    if ((plvs.C("size")).T()) {
      Gv plv = plvs.I(Gv.I(0L));
      S = S.S("lv", S.G("lv").S("varna", Gv.B(false)));
      S = S.S("lv", S.G("lv").S("piv", Gv.B(false)));
      S = S.S("lv", S.G("lv").S("_circ", Gv.B(false)));
      Gv pn = net.G("pumpNodes").C("get", plv.G("id"));
      if ((((S.G("lv").G("pump")).T() && Gv.Ne(pn, Gv.Null)) && Gv.Lt(S.G("lv").G("w"), Gv.I(300L)))) {
        once((Gv.S("plvdry2%d") % tb(Gv.I(600L))), Gv.S("Ledovka je skoro prázdná – čerpadlo ledové by běželo nasucho, nečerpá."), Gv.S("bad"));
      }
      if ((((S.G("lv").G("pump")).T() && Gv.Ne(pn, Gv.Null)) && Gv.Ge(S.G("lv").G("w"), Gv.I(300L)))) {
        Gv rin = bfs(pn.G("in"));
        Gv src = Gv.Null;
        foreach (Gv hh_2 in (rin.G("hits")).Iter()) {
          if ((Gv.Eq(hh_2.G("att").G("bind"), Gv.S("lv")) && (hh_2.G("att").G("low")).T())) {
            src = hh_2;
            break;
          }
        }
        if (Gv.Ne(src, Gv.Null)) {
          Gv r_2 = bfs(pn.G("out"));
          if ((net.G("chilInline")).T()) {
            Gv tg = Gd.Arr();
            foreach (Gv hh_3 in (r_2.G("hits")).Iter()) {
              if (Gv.Eq(hh_3.G("att").G("bind"), Gv.S("lv"))) {
                tg.C("append", hh_3.G("k"));
              }
            }
            foreach (Gv k_2 in (pn.G("in")).Iter()) {
              if ((r_2.G("prev").C("has", k_2)).T()) {
                tg.C("append", k_2);
              }
            }
            foreach (Gv k_3 in (tg).Iter()) {
              Gv cur = k_3;
              Gv g = Gv.I(0L);
              while ((Gv.Ne(cur, Gv.Null) && Gv.Lt(g, Gv.I(4000L)))) {
                Gv nn = net.G("nodes").C("get", cur);
                if ((Gv.Ne(nn, Gv.Null) && Gv.Ne(nn.G("_chil"), Gv.S("")))) {
                  S = S.S("lv", S.G("lv").S("_circ", Gv.B(true)));
                  break;
                }
                Gv pv = r_2.G("prev").C("get", cur);
                cur = (Gv.Ne(pv, Gv.Null) ? pv.G("k") : Gv.Null);
                g = (g + Gv.I(1L));
              }
              if ((S.G("lv").G("_circ")).T()) {
                break;
              }
            }
          }
          Gv suction = suction_path(path_to(rin, src.G("k")));
          Gv dests = r_2.G("hits");
          Gv Lm = (Gv.F(120.0) / Gd.maxf(Gv.F(1.0), dests.C("size")));
          foreach (Gv s_2 in (suction).Iter()) {
            if ((s_2.C("has", Gv.S("e"))).T()) {
              Gv p = lp();
              p = p.S("w", (Gv.F(120.0) * m));
              p = p.S("T", S.G("lv").G("T"));
              push_path(Gd.Arr(s_2), p, Gv.F(120.0), m);
            }
          }
          foreach (Gv d in (dests).Iter()) {
            Gv path = path_to(r_2, d.G("k"));
            foreach (Gv s_3 in (path).Iter()) {
              if (((s_3.C("has", Gv.S("plate"))).T() && Gv.Eq(s_3.G("side"), Gv.S("B")))) {
                S = S.S("plates", S.G("plates").SI(s_3.G("plate"), Gd.Dict(Gv.S("kind"), Gv.S("ice"), Gv.S("T"), S.G("lv").G("T"), Gv.S("once"), Gv.B(Gv.Eq(d.G("att").G("bind"), Gv.S("hlt"))), Gv.S("flow"), Gd.minf(Lm, (S.G("lv").G("w") / Gd.maxf(Gv.F(1e-06), m))))));
                S = S.S("lv", S.G("lv").S("varna", Gv.B(true)));
              }
            }
            if (Gv.Eq(d.G("att").G("bind"), Gv.S("tanky"))) {
              S = S.S("lv", S.G("lv").S("piv", Gv.B(true)));
            }
            if (Gv.Eq(d.G("att").G("bind"), Gv.S("hlt"))) {
              Gv V = Gd.minf((Lm * m), S.G("lv").G("w"));
              Gv du = Gv.F(0.0);
              foreach (Gv s_4 in (path).Iter()) {
                if (((s_4.C("has", Gv.S("plate"))).T() && Gv.Eq(s_4.G("side"), Gv.S("B")))) {
                  du = S.G("lastDuty").C("get", s_4.G("plate"), Gv.F(0.0));
                }
              }
              Gv Tw = Gd.minf(Gv.F(95.0), (S.G("lv").G("T") + (du / Gd.maxf(Gv.F(0.1), ((Lm / Gv.F(60.0)) * Gv.F(4.19))))));
              Gv room = Gd.maxf(Gv.F(0.0), (c.G("hltCap") - S.G("hlt").G("w")));
              Gv Vin = Gd.minf(V, room);
              S = S.S("lv", S.G("lv").S("w", (S.G("lv").G("w") - V)));
              if (Gv.Gt(Vin, Gv.I(0L))) {
                S = S.S("hlt", S.G("hlt").S("T", ((((S.G("hlt").G("T") * S.G("hlt").G("w")) + (Tw * Vin))) / ((S.G("hlt").G("w") + Vin)))));
                S = S.S("hlt", S.G("hlt").S("w", (S.G("hlt").G("w") + Vin)));
              }
              if (Gv.Gt(V, (Vin + Gv.F(1e-06)))) {
                S = S.S("drainL", (S.G("drainL") + (V - Vin)));
                once((Gv.S("hltfull%d") % tb(Gv.I(900L))), Gv.S("Bojler je plný – ohřátá chladicí voda teče přepadem do kanálu."), Gv.S("warn"));
              }
            }
            Gv p2 = lp();
            p2 = p2.S("w", (Lm * m));
            p2 = p2.S("T", S.G("lv").G("T"));
            push_path(edges_only(path), p2, Lm, m);
          }
          Gv dl = Gd.Arr();
          foreach (Gv d_2 in (dests).Iter()) {
            dl.C("append", d_2.G("att").G("bind"));
          }
          S.G("flows").C("append", Gd.Dict(Gv.S("pump"), plv.G("id"), Gv.S("src"), Gv.S("lv"), Gv.S("dst"), dl));
        } else {
          once((Gv.S("plvdry%d") % tb(Gv.I(900L))), Gv.S("Čerpadlo ledové vody nemá sání z nádrže – zkontroluj klapky."), Gv.S("warn"));
        }
      }
    }
    if ((prof_on).T()) {
      prof = prof.SI(Gv.S("n_ledova"), ((prof.C("get", Gv.S("n_ledova"), Gv.I(0L)) + Gd.Static("Time", "get_ticks_usec")) - _n0));
      _n0 = Gd.Static("Time", "get_ticks_usec");
    }
    Gv gl = items_by_bind(Gv.S("glyk"));
    if (!(gl.C("size")).T()) {
      S = S.S("lv", S.G("lv").S("glyk", Gv.Null));
    } else {
      Gv ok = Gv.B(false);
      Gv why = Gv.S("glykolový chladič není propojený s ledovkou okruhem tam i zpět (dvě trubky) nebo je na cestě zavřená klapka");
      if ((net.G("chilInline")).T()) {
        ok = S.G("lv").G("_circ");
        why = ((S.G("lv").G("pump")).T() ? Gv.S("ledovka necirkuluje přes glykolový chladič – nastav klapky tak, aby voda šla z čerpadla ledovky přes chladič glykolu a zpět") : Gv.S("čerpadlo ledovky stojí – okruh přes glykolový chladič neběží"));
      } else {
        foreach (Gv g_2 in (gl).Iter()) {
          Gv st2 = starts_of(g_2.G("id"));
          if (!(st2.C("size")).T()) {
            why = Gv.S("glykolový chladič nemá připojenou žádnou trubku");
            continue;
          }
          Gv r2 = bfs(st2);
          Gv ks = Gd.Dict();
          foreach (Gv hh_4 in (r2.G("hits")).Iter()) {
            if (Gv.Eq(hh_4.G("att").G("bind"), Gv.S("lv"))) {
              ks = ks.SI(hh_4.G("k"), Gv.B(true));
            }
          }
          if ((Gv.Ge(ks.C("size"), Gv.I(2L)) || (Gv.Ge(ks.C("size"), Gv.I(1L)) && Gv.Ge(st2.C("size"), Gv.I(2L))))) {
            ok = Gv.B(true);
            break;
          }
        }
      }
      Gv sws = net.G("sws");
      if ((sws.C("size")).T()) {
        Gv on_2 = Gv.B(false);
        foreach (Gv i in (sws).Iter()) {
          if ((i.C("get", Gv.S("on"), Gv.B(false))).T()) {
            on_2 = Gv.B(true);
          }
        }
        if (((on_2).T() && !(S.G("lv").G("chill")).T())) {
          S = S.S("lv", S.G("lv").S("chill", Gv.B(true)));
        }
        if ((!(on_2).T() && (S.G("lv").G("chill")).T())) {
          S = S.S("lv", S.G("lv").S("chill", Gv.B(false)));
        }
        if (((ok).T() && !(on_2).T())) {
          ok = Gv.B(false);
          why = Gv.S("spínač glykolu je vypnutý");
        }
      }
      S = S.S("lv", S.G("lv").S("glyk", ok));
      S = S.S("lv", S.G("lv").S("glykWhy", ((ok).T() ? Gv.S("") : why)));
    }
    if ((prof_on).T()) {
      prof = prof.SI(Gv.S("n_glykol"), ((prof.C("get", Gv.S("n_glykol"), Gv.I(0L)) + Gd.Static("Time", "get_ticks_usec")) - _n0));
      _n0 = Gd.Static("Time", "get_ticks_usec");
    }
    foreach (Gv src_2 in (items_by_bind(Gv.S("vodovod"))).Iter()) {
      if ((Gv.Eq(src_2.C("get", Gv.S("shape")), Gv.S("inlet")) && Gv.Ne(src_2.C("get", Gv.S("on"), Gv.B(false)), Gv.B(true)))) {
        continue;
      }
      Gv r_3 = bfs(starts_of(src_2.G("id")));
      Gv outs = Gd.Arr();
      foreach (Gv d_3 in (r_3.G("hits")).Iter()) {
        if (Gv.Ne(d_3.G("att").G("bind"), Gv.S("vodovod"))) {
          outs.C("append", Gd.Dict(Gv.S("d"), d_3, Gv.S("path"), path_to(r_3, d_3.G("k"))));
        }
      }
      foreach (Gv lk in (r_3.G("leaks")).Iter()) {
        outs.C("append", Gd.Dict(Gv.S("lk"), lk, Gv.S("path"), path_to(r_3, lk.G("k"))));
      }
      Gv nOpen = Gv.I(0L);
      foreach (Gv o__it in (outs).Iter()) {
        Gv o = o__it;
        o = o.S("f", path_pct(o.G("path")));
        if (Gv.Gt(o.G("f"), Gv.I(0L))) {
          nOpen = (nOpen + Gv.I(1L));
        }
      }
      Gv mains_q = mains_solve(outs, src_2);
      if (Gv.Gt(nOpen, Gv.I(1L))) {
        once((Gv.S("mainsplit%d") % tb(Gv.I(900L))), (Gv.S("Voda z řadu teče do %d míst naráz – tlak se dělí, do každého teče méně.") % nOpen));
      }
      foreach (Gv oi in (outs.C("size")).Iter()) {
        Gv o_2 = outs.I(oi);
        if (Gv.Le(o_2.G("f"), Gv.I(0L))) {
          continue;
        }
        Gv q = (mains_q.I(oi)).AsF();
        if (Gv.Lt(q, Gv.F(0.3))) {
          continue;
        }
        if ((o_2.C("has", Gv.S("lk"))).T()) {
          Gv pl = lp();
          pl = pl.S("w", (q * m));
          pl = pl.S("T", S.G("cool").G("waterT"));
          spill(r_3, o_2.G("lk"), pl, q, m, Gv.S("Voda z řadu"));
          continue;
        }
        Gv d_4 = o_2.G("d");
        Gv path_2 = o_2.G("path");
        Gv rate = q;
        Gv T = (S.G("cool").G("waterT")).AsF();
        Gv plates = Gd.Arr();
        foreach (Gv s_5 in (path_2).Iter()) {
          if (((s_5.C("has", Gv.S("plate"))).T() && Gv.Eq(s_5.G("side"), Gv.S("B")))) {
            plates.C("append", s_5);
          }
        }
        foreach (Gv s_6 in (plates).Iter()) {
          S = S.S("plates", S.G("plates").SI(s_6.G("plate"), Gd.Dict(Gv.S("kind"), Gv.S("water"), Gv.S("T"), S.G("cool").G("waterT"))));
          Gv pf = (S.G("lastPlateFlow").C("get", s_6.G("plate"), Gv.F(0.0))).AsF();
          if (Gv.Gt(pf, Gv.I(0L))) {
            rate = Gd.maxf(rate, ((Gv.F(1.2) * pf) * o_2.G("f")));
          }
        }
        foreach (Gv s_7 in (plates).Iter()) {
          S = S.S("plates", S.G("plates").SI(s_7.G("plate"), S.G("plates").I(s_7.G("plate")).SI(Gv.S("flow"), rate)));
        }
        foreach (Gv s_8 in (plates).Iter()) {
          T = ((T + (S.G("lastDuty").C("get", s_8.G("plate"), Gv.F(0.0)) / (((rate / Gv.F(60.0)) * Gv.F(4.19)))))).AsF();
        }
        Gv part = lp();
        part = part.S("w", (rate * m));
        part = part.S("T", S.G("cool").G("waterT"));
        foreach (Gv s_9 in (path_2).Iter()) {
          if ((s_9.C("has", Gv.S("plate"))).T()) {
            part = part.S("T", T);
            continue;
          }
          Gv e_3 = s_9.G("e");
          e_3 = e_3.S("from", (Gv.Eq(s_9.G("to"), e_3.G("b")) ? Gv.S("a") : Gv.S("b")));
          e_3 = e_3.S("flow", Gd.maxf(e_3.G("flow"), rate));
          part = edge_push(e_3, part, Gv.B(Gv.Eq(s_9.G("to"), e_3.G("b"))));
        }
        Gv out_ = part;
        if (Gv.Eq(d_4.G("att").G("bind"), Gv.S("hlt"))) {
          S = S.S("cool", S.G("cool").S("Tw", T));
        }
        _stream(d_4, out_);
        if (Gv.Eq(d_4.G("att").G("bind"), Gv.S("lv"))) {
          Gv fl = lp();
          fl = fl.S("w", (rate * m));
          fl = fl.S("T", S.G("cool").G("waterT"));
          Gv viaChil = Gv.B(false);
          foreach (Gv s_10 in (path_2).Iter()) {
            if (((s_10.C("has", Gv.S("e"))).T() && Gv.Ne(net.G("nodes").I(s_10.G("to")).G("_chil"), Gv.S("")))) {
              viaChil = Gv.B(true);
            }
          }
          if ((viaChil).T()) {
            Gv gOn = S.G("lv").G("chill");
            if ((gOn).T()) {
              fl = fl.S("T", Gd.maxf(c.G("lvSet"), (S.G("cool").G("waterT") - (c.G("chillerKW") / (((rate / Gv.F(60.0)) * Gv.F(4.19)))))));
              once(Gv.S("lvprechill"), (Gv.S("Voda z řadu teče do ledovky přes glykolový chladič – přitéká vychlazená na %s °C.") % fmt1(fl.G("T"))));
            } else {
              once((Gv.S("lvprechill0%d") % tb(Gv.I(1800L))), Gv.S("Voda z řadu teče do ledovky přes glykolový chladič, ale glykol je vypnutý – přitéká teplá z řadu. Zapni spínač glykolu."), Gv.S("warn"));
            }
          }
          dstGive(Gv.S("lv"), fl, Gd.Dict(Gv.S("src"), Gv.S("vodovod")));
        } else {
          if (Gv.Eq(d_4.G("att").G("bind"), Gv.S("sk"))) {
            if ((sanSpray(d_4.G("att").G("spray"))).T()) {
              skWallHeat(out_);
            }
            skAddWater(liqVol(out_), out_.G("T"), d_4.G("att").C("get", Gv.S("under"), d_4.G("att").G("low")));
          } else {
            dstGive(d_4.G("att").G("bind"), out_, Gd.Dict(Gv.S("src"), Gv.S("vodovod")));
          }
        }
      }
      Gv me_m = (_dead_due(Gv.S("main"), m)).AsF();
      if (Gv.Gt(me_m, Gv.F(0.0))) {
        Gv used_m = Gd.Dict();
        foreach (Gv o_3 in (outs).Iter()) {
          if (Gv.Gt(o_3.G("f"), Gv.I(0L))) {
            foreach (Gv s_11 in (o_3.G("path")).Iter()) {
              if ((s_11.C("has", Gv.S("e"))).T()) {
                used_m = used_m.SI(s_11.G("e").G("id"), Gv.B(true));
              }
            }
          }
        }
        _dead_end_apply(_dead_end_plan(r_3, used_m, Gv.F(80.0), me_m), S.G("cool").G("waterT"));
      }
    }
    if ((prof_on).T()) {
      prof = prof.SI(Gv.S("n_vodovod"), ((prof.C("get", Gv.S("n_vodovod"), Gv.I(0L)) + Gd.Static("Time", "get_ticks_usec")) - _n0));
      _n0 = Gd.Static("Time", "get_ticks_usec");
    }
    Gv wantR = S.G("rvpWater").G("on");
    Gv wantS = S.G("skWater").G("on");
    if (((wantR).T() || (wantS).T())) {
      Gv st3 = Gd.Arr();
      foreach (Gv hh_5 in (items_by_bind(Gv.S("hlt"))).Iter()) {
        st3.C("append_array", starts_of(hh_5.G("id"), Gv.B(true)));
      }
      Gv r_4 = bfs(st3, Gv.S("liq"), Gd.Arr(Gv.S("phw")));
      Gv dests_2 = Gd.Arr();
      foreach (Gv d_5 in (r_4.G("hits")).Iter()) {
        if ((Gv.Ne(d_5.G("att").G("bind"), Gv.S("hlt")) && Gv.Ne(d_5.G("att").G("bind"), Gv.S("vodovod")))) {
          dests_2.C("append", d_5);
        }
      }
      Gv hasR = Gv.B(false);
      Gv hasS = Gv.B(false);
      foreach (Gv d_6 in (dests_2).Iter()) {
        if (Gv.Eq(d_6.G("att").G("bind"), Gv.S("rvp"))) {
          hasR = Gv.B(true);
        }
        if (Gv.Eq(d_6.G("att").G("bind"), Gv.S("sk"))) {
          hasS = Gv.B(true);
        }
      }
      if (((wantR).T() && !(hasR).T())) {
        once((Gv.S("nowR%d") % tb(Gv.I(600L))), Gv.S("Voda do pánve nemá cestu – otevři klapky od horké vody."), Gv.S("warn"));
      }
      if (((wantS).T() && !(hasS).T())) {
        once((Gv.S("nowS%d") % tb(Gv.I(600L))), Gv.S("Vyslazovací voda nemá cestu do SK – otevři klapky."), Gv.S("warn"));
      }
      Gv wf = ((c.G("waterFlow") * S.G("rvpWater").G("k"))).AsF();
      Gv rate_2 = (((((wantR).T() ? wf : Gv.F(0.0))) + (((wantS).T() ? S.G("skWater").G("flow") : Gv.F(0.0))))).AsF();
      if ((wantR).T()) {
        Gv left = ((S.G("rvpWater").G("target") - S.G("rvpWater").G("added"))).AsF();
        if (Gv.Lt(left, (wf * m))) {
          rate_2 = ((rate_2 - (wf - (left / m)))).AsF();
        }
      }
      Gv Tset = (((wantR).T() ? S.G("rvpWater").G("T") : S.G("skWater").G("T"))).AsF();
      if ((((dests_2.C("size")).T() || (r_4.G("leaks").C("size")).T()) && Gv.Gt(rate_2, Gv.I(0L)))) {
        Gv L = (rate_2 * m);
        if (!(S.G("mixAuto")).T()) {
          L = Gd.minf(L, Gd.maxf(Gv.F(0.0), (S.G("hlt").G("w") - Gv.F(150.0))));
        }
        Gv T_2 = (((S.G("mixAuto")).T() ? drawWater(L, Tset) : drawHot(L))).AsF();
        rate_2 = ((L / m)).AsF();
        S = S.S("usedWater", (S.G("usedWater") + L));
        Gv ws = Gd.Dict();
        Gv wsum = Gv.F(0.0);
        foreach (Gv d_7 in (dests_2).Iter()) {
          Gv pa = path_to(r_4, d_7.G("k"));
          Gv w = (path_pct(pa) / Gd.sqrt((Gv.F(1.0) + (path_len(pa) / Gv.F(3000.0)))));
          ws = ws.SI(d_7.G("k"), w);
          wsum = (wsum + w);
        }
        foreach (Gv lk_2 in (r_4.G("leaks")).Iter()) {
          Gv pa2 = path_to(r_4, lk_2.G("k"));
          Gv w2 = (path_pct(pa2) / Gd.sqrt((Gv.F(1.0) + (path_len(pa2) / Gv.F(3000.0)))));
          ws = ws.SI(lk_2.G("k"), w2);
          wsum = (wsum + w2);
        }
        wsum = Gd.maxf(Gv.F(1e-06), wsum);
        foreach (Gv lk_3 in (r_4.G("leaks")).Iter()) {
          Gv p_2 = lp();
          p_2 = p_2.S("w", ((L * ws.I(lk_3.G("k"))) / wsum));
          p_2 = p_2.S("T", T_2);
          if (Gv.Gt(p_2.G("w"), Gv.F(1e-06))) {
            spill(r_4, lk_3, p_2, ((rate_2 * ws.I(lk_3.G("k"))) / wsum), m, Gv.S("Horká voda"));
          }
        }
        foreach (Gv d_8 in (dests_2).Iter()) {
          Gv p2_2 = lp();
          p2_2 = p2_2.S("w", ((L * ws.I(d_8.G("k"))) / wsum));
          p2_2 = p2_2.S("T", T_2);
          if (Gv.Le(p2_2.G("w"), Gv.F(1e-06))) {
            continue;
          }
          Gv _pp = Gd.Static("Time", "get_ticks_usec");
          Gv out__2 = push_path(path_to(r_4, d_8.G("k")), p2_2, rate_2, m);
          if ((prof_on).T()) {
            prof = prof.SI(Gv.S("n_h_push"), ((prof.C("get", Gv.S("n_h_push"), Gv.I(0L)) + Gd.Static("Time", "get_ticks_usec")) - _pp));
            _pp = Gd.Static("Time", "get_ticks_usec");
          }
          Gv Lo = liqVol(out__2);
          _stream(d_8, out__2);
          if (Gv.Eq(d_8.G("att").G("bind"), Gv.S("sk"))) {
            if ((d_8.G("att").C("get", Gv.S("under"), d_8.G("att").G("low"))).T()) {
              S = S.S("_skLow", Gv.I(1L));
            } else {
              S = S.S("_skTop", Gv.I(1L));
            }
          }
          if (Gv.Eq(d_8.G("att").G("bind"), Gv.S("rvp"))) {
            S = S.S("_rvpIn", Gv.I(1L));
            S = S.S("rvpWater", S.G("rvpWater").S("added", (S.G("rvpWater").G("added") + Lo)));
            addTo(S.G("rvp"), out__2, c.G("rvpSteel"));
          } else if (Gv.Eq(d_8.G("att").G("bind"), Gv.S("sk"))) {
            if ((sanSpray(d_8.G("att").G("spray"))).T()) {
              skWallHeat(out__2);
            }
            S = S.S("skWater", S.G("skWater").S("total", (S.G("skWater").G("total") + Lo)));
            skAddWater(Lo, out__2.G("T"), d_8.G("att").C("get", Gv.S("under"), d_8.G("att").G("low")));
          } else {
            dstGive(d_8.G("att").G("bind"), out__2, Gd.Dict(Gv.S("src"), Gv.S("hlt")));
            once((Gv.S("hwelse%s%d") % Gd.Arr(d_8.G("att").G("bind"), tb(Gv.I(600L)))), (Gv.S("Horká voda teče i do: %s – zavři klapku.") % net.G("byId").I(d_8.G("att").G("id")).C("get", Gv.S("label"), d_8.G("att").G("bind"))), Gv.S("warn"));
          }
        }
        Gv used_h = Gd.Dict();
        Gv me_h = (_dead_due(Gv.S("hot"), m)).AsF();
        Gv hplan = Gd.Arr();
        if (Gv.Gt(me_h, Gv.F(0.0))) {
          foreach (Gv d_9 in (dests_2).Iter()) {
            foreach (Gv s_12 in (path_to(r_4, d_9.G("k"))).Iter()) {
              if ((s_12.C("has", Gv.S("e"))).T()) {
                used_h = used_h.SI(s_12.G("e").G("id"), Gv.B(true));
              }
            }
          }
          foreach (Gv lk_4 in (r_4.G("leaks")).Iter()) {
            foreach (Gv s_13 in (path_to(r_4, lk_4.G("k"))).Iter()) {
              if ((s_13.C("has", Gv.S("e"))).T()) {
                used_h = used_h.SI(s_13.G("e").G("id"), Gv.B(true));
              }
            }
          }
          hplan = _dead_end_plan(r_4, used_h, Gv.F(60.0), me_h);
        }
        Gv Lh = Gv.F(0.0);
        foreach (Gv x_2 in (hplan).Iter()) {
          Lh = (Lh + x_2.I(Gv.I(1L)));
        }
        if (Gv.Gt(Lh, Gv.F(0.0))) {
          Gv Lh2 = (((S.G("mixAuto")).T() ? Lh : Gd.minf(Lh, Gd.maxf(Gv.F(0.0), (S.G("hlt").G("w") - Gv.F(150.0)))))).AsF();
          if (Gv.Gt(Lh2, Gv.F(0.01))) {
            Gv fk = ((Lh2 / Lh)).AsF();
            foreach (Gv x_3__it in (hplan).Iter()) {
              Gv x_3 = x_3__it;
              x_3 = x_3.SI(Gv.I(1L), (x_3.I(Gv.I(1L)) * fk));
            }
            Gv Th = (((S.G("mixAuto")).T() ? drawWater(Lh2, Tset) : drawHot(Lh2))).AsF();
            S = S.S("usedWater", (S.G("usedWater") + Lh2));
            _dead_end_apply(hplan, Th);
          }
        }
      }
      if (((wantR).T() && Gv.Ge(S.G("rvpWater").G("added"), (S.G("rvpWater").G("target") - Gv.F(0.5))))) {
        S = S.S("rvpWater", S.G("rvpWater").S("on", Gv.B(false)));
        logm((Gv.S("Pánev: napuštěno %d L vody.") % Gd.roundi(S.G("rvpWater").G("added"))));
      }
    }
    if ((prof_on).T()) {
      prof = prof.SI(Gv.S("n_horka"), ((prof.C("get", Gv.S("n_horka"), Gv.I(0L)) + Gd.Static("Time", "get_ticks_usec")) - _n0));
      _n0 = Gd.Static("Time", "get_ticks_usec");
    }
    if (!(S.C("has", Gv.S("clog"))).T()) {
      S = S.S("clog", Gd.Dict(Gv.S("rvp"), Gv.F(0.0), Gv.S("sk"), Gv.F(0.0)));
    }
    foreach (Gv cb in (Gd.Arr(Gv.S("rvp"), Gv.S("sk"))).Iter()) {
      Gv mv = (Gv.Eq(cb, Gv.S("rvp")) ? S.G("rvp") : S.G("sk").G("mash"));
      Gv mixed = (Gv.Eq(cb, Gv.S("rvp")) ? (Gv.B(((S.G("rvp").G("stir")).T() && (cab_live()).T()))) : (Gv.B(((S.G("sk").G("rake")).T() && (cab_live()).T()))));
      if ((Gv.Lt(vol(mv), Gv.I(5L)) || Gv.Lt(thick_factor(mv), Gv.F(0.15)))) {
        S = S.S("clog", S.G("clog").SI(cb, Gd.maxf(Gv.F(0.0), (S.G("clog").I(cb) - (dt / Gv.F(60.0))))));
      } else if ((mixed).T()) {
        S = S.S("clog", S.G("clog").SI(cb, Gd.maxf(Gv.F(0.0), (S.G("clog").I(cb) - (dt / Gv.F(60.0))))));
      } else {
        S = S.S("clog", S.G("clog").SI(cb, Gd.minf(Gv.F(1.0), (S.G("clog").I(cb) + (dt / Gv.F(300.0))))));
      }
    }
    foreach (Gv key in (Gd.Arr(Gv.S("p1"), Gv.S("p2"), Gv.S("p3"))).Iter()) {
      Gv q_2 = S.I(key);
      if (!(q_2.G("on")).T()) {
        q_2 = q_2.S("status", Gv.S(""));
        q_2 = q_2.S("flowing", Gv.B(false));
        q_2 = q_2.S("pt", (q_2.G("pt") + dt));
        if (Gv.Ge(q_2.G("pt"), Gv.F(2.0))) {
          _pump_prime_idle(key, q_2, q_2.G("pt"));
          q_2 = q_2.S("pt", Gv.F(0.0));
        }
        continue;
      }
      Gv pis = items_by_bind(key);
      if (!(pis.C("size")).T()) {
        once((Gv.S("nopump") + key), (Gv.S("%s není ve schématu.") % key.C("to_upper")), Gv.S("warn"));
        q_2 = q_2.S("status", Gv.S("není ve schématu"));
        continue;
      }
      Gv pumpIt = pis.I(Gv.I(0L));
      Gv pname = pumpIt.C("get", Gv.S("label"), key.C("to_upper"));
      if ((q_2.C("get", Gv.S("airLock"), Gv.B(false))).T()) {
        q_2 = q_2.S("prime", Gd.minf(q_2.G("prime"), Gv.F(0.05)));
        q_2 = q_2.S("q", Gv.F(0.0));
        q_2 = q_2.S("status", Gv.S("ZAVZDUŠNĚNO – hučí naprázdno, nečerpá (vypni, odvzdušni)"));
        q_2 = q_2.S("dry", (q_2.G("dry") + dt));
        if (Gv.Gt(q_2.G("dry"), Gv.F(120.0))) {
          q_2 = q_2.S("on", Gv.B(false));
          q_2 = q_2.S("dry", Gv.F(0.0));
          logm((Gv.S("Ochrana chodu nasucho vypnula %s – zavzdušněné čerpadlo běželo bez kapaliny.") % pname), Gv.S("bad"));
        }
        continue;
      }
      Gv pn_2 = net.G("pumpNodes").C("get", pumpIt.G("id"), Gd.Dict(Gv.S("in"), Gd.Arr(), Gv.S("out"), Gd.Arr()));
      Gv rs = bfs(pn_2.G("in"));
      Gv srcs = Gd.Arr();
      foreach (Gv hh_6 in (rs.G("hits")).Iter()) {
        if (((hh_6.G("att").G("low")).T() && Gv.In(hh_6.G("att").G("bind"), Gd.Arr(Gv.S("rvp"), Gv.S("mp"), Gv.S("sk"), Gv.S("wp"), Gv.S("hlt"), Gv.S("spilka"))))) {
          srcs.C("append", hh_6);
        }
      }
      Gv src_3 = Gv.Null;
      foreach (Gv hh_7 in (srcs).Iter()) {
        if (Gv.Gt(_src_level(hh_7.G("att").G("bind")), Gv.I(5L))) {
          src_3 = hh_7;
          break;
        }
      }
      if ((Gv.Eq(src_3, Gv.Null) && (srcs.C("size")).T())) {
        src_3 = srcs.I(Gv.I(0L));
      }
      if (Gv.Eq(src_3, Gv.Null)) {
        q_2 = q_2.S("status", Gv.S("nesaje"));
        once((Gv.S("nosrc%s%d") % Gd.Arr(key, tb(Gv.I(300L)))), (Gv.S("%s nemá odkud sát – klapka před čerpadlem je zavřená nebo je nádoba prázdná. Čerpadlo nejdřív vypni, pak zavírej klapky; při chodu nasucho se čerpadlo odvzdušní a ucpávka se ničí.") % pname), Gv.S("warn"));
        if ((q_2.C("get", Gv.S("flowing"), Gv.B(false))).T()) {
          q_2 = q_2.S("flowing", Gv.B(false));
          logm((Gv.S("%s: klapka před čerpadlem se zavřela za jeho chodu – čerpadlo nasává vzduch. Správně: nejdřív čerpadlo vypnout, pak zavřít klapku za ním a až potom před ním.") % pname), Gv.S("bad"));
        }
        q_2 = q_2.S("prime", Gd.maxf(Gv.F(0.0), (q_2.G("prime") - (dt / Gv.F(15.0)))));
        q_2 = q_2.S("dry", (q_2.G("dry") + dt));
        if (Gv.Gt(q_2.G("dry"), Gv.F(60.0))) {
          q_2 = q_2.S("on", Gv.B(false));
          q_2 = q_2.S("dry", Gv.F(0.0));
          logm((Gv.S("Ochrana chodu nasucho vypnula %s – běžela bez kapaliny (hrozilo poškození ucpávky).") % pname), Gv.S("bad"));
        }
        continue;
      }
      Gv rd = bfs(pn_2.G("out"));
      Gv dests2 = Gd.Arr();
      foreach (Gv d_10 in (rd.G("hits")).Iter()) {
        if (Gv.Ne(d_10.G("att").G("bind"), Gv.S("vodovod"))) {
          dests2.C("append", d_10);
        }
      }
      Gv lks = rd.G("leaks");
      if ((!(dests2.C("size")).T() && !(lks.C("size")).T())) {
        if ((q_2.C("get", Gv.S("flowing"), Gv.B(false))).T()) {
          q_2 = q_2.S("flowing", Gv.B(false));
          logm((Gv.S("%s: klapka za čerpadlem se zavřela za jeho chodu – čerpadlo tlačí do zavřeného potrubí (tlakový ráz, ohřev kapaliny). Správně: nejdřív čerpadlo vypnout, teprve potom zavírat klapky.") % pname), Gv.S("bad"));
        }
        q_2 = q_2.S("status", Gv.S("tlačí do zavřené klapky"));
        once((Gv.S("nodst%s%d") % Gd.Arr(key, tb(Gv.I(300L)))), (Gv.S("%s tlačí proti zavřené klapce za čerpadlem – nic neteče. Správné pořadí: čerpadlo vypnout, zavřít klapku za čerpadlem, pak před čerpadlem; spouštět naopak (otevřít před, zavodnit, otevřít za, zapnout).") % pname), Gv.S("warn"));
        q_2 = q_2.S("dead", (q_2.G("dead") + dt));
        if (Gv.Gt(q_2.G("dead"), Gv.F(90.0))) {
          q_2 = q_2.S("on", Gv.B(false));
          q_2 = q_2.S("dead", Gv.F(0.0));
          logm((Gv.S("Tepelná ochrana vypnula %s – dlouho tlačila proti zavřené klapce (kapalina v čerpadle se ohřívala).") % pname), Gv.S("bad"));
        }
        continue;
      }
      Gv sb = src_3.G("att").G("bind");
      if ((((Gv.Eq(key, Gv.S("p2")) && Gv.Eq(sb, Gv.S("sk"))) && Gv.Eq(S.G("sk").G("mode"), Gv.S("mash"))) && (Gv.Gt(vol(S.G("sk").G("mash")), Gv.I(5L)) || Gv.Gt(S.G("sk").G("g"), Gv.I(1L))))) {
        q_2 = q_2.S("on", Gv.B(false));
        q_2 = q_2.S("status", Gv.S("Nejprve odrmutuj a vytvoř lože."));
        logm(Gv.S("C2 by z rmutu nenápadně udělalo filtrační lože. Nejprve dokonči rmutování a stiskni Odrmutovat a vytvořit lože."), Gv.S("warn"));
        continue;
      }
      Gv names = Gd.Dict();
      foreach (Gv d_11 in (dests2).Iter()) {
        names = names.SI(net.G("byId").I(d_11.G("att").G("id")).C("get", Gv.S("label"), d_11.G("att").G("bind")), Gv.B(true));
      }
      q_2 = q_2.S("status", (Gv.S("%s » %s") % Gd.Arr(net.G("byId").I(src_3.G("att").G("id")).C("get", Gv.S("label"), sb), Gv.S(" + ").C("join", Gd.Ctor("PackedStringArray", names.C("keys"))))));
      Gv ids = Gd.Dict();
      foreach (Gv d_12 in (dests2).Iter()) {
        ids = ids.SI(d_12.G("att").G("id"), Gv.B(true));
      }
      if (Gv.Gt(ids.C("size"), Gv.I(1L))) {
        once((Gv.S("split%s%d") % Gd.Arr(key, tb(Gv.I(600L)))), (Gv.S("%s: průtok se dělí do %d nádob.") % Gd.Arr(pname, ids.C("size"))), Gv.S("warn"));
      }
      Gv to_sp = Gv.B(false);
      foreach (Gv d_13 in (dests2).Iter()) {
        if (Gv.Eq(d_13.G("att").G("bind"), Gv.S("spilka"))) {
          to_sp = Gv.B(true);
        }
      }
      if (((to_sp).T() && !(S.G("sp").C("get", Gv.S("lidOpen"), Gv.B(false))).T())) {
        Gv pr = (S.G("sp").C("get", Gv.S("press"), Gv.F(0.0))).AsF();
        if (Gv.Gt(pr, Gv.F(40.0))) {
          once((Gv.S("splidw%d") % tb(Gv.I(300L))), (Gv.S("Spilka se tlakuje (%d mbar) – víčko je zavřené, vzduch nemá kudy ven! Otevři víčko na tanku.") % Gd.roundi(pr)), Gv.S("warn"));
        }
        if (Gv.Gt(pr, Gv.F(120.0))) {
          q_2 = q_2.S("status", Gv.S("přetlak ve spilce – otevři víčko"));
          once((Gv.S("splidx%d") % tb(Gv.I(300L))), (Gv.S("Přetlak ve spilce %d mbar – tank není tlaková nádoba, hrozí deformace. Čerpadlo stojí, otevři víčko.") % Gd.roundi(pr)), Gv.S("bad"));
          q_2 = q_2.S("dead", (q_2.G("dead") + dt));
          if (Gv.Gt(q_2.G("dead"), Gv.F(90.0))) {
            q_2 = q_2.S("on", Gv.B(false));
            q_2 = q_2.S("dead", Gv.F(0.0));
            logm((Gv.S("Ochrana vypnula %s – přetlak ve spilce (zavřené víčko).") % pname), Gv.S("bad"));
          }
          continue;
        }
      }
      Gv sp_ = path_to(rs, src_3.G("k"));
      Gv rp = Gv.F(0.0);
      Gv nd = Gv.I(0L);
      Gv dpaths = Gd.Arr();
      foreach (Gv d_14 in (dests2).Iter()) {
        dpaths.C("append", path_to(rd, d_14.G("k")));
        nd = (nd + Gv.I(1L));
      }
      foreach (Gv lk_5 in (lks).Iter()) {
        dpaths.C("append", path_to(rd, lk_5.G("k")));
        nd = (nd + Gv.I(1L));
      }
      Gv pump_y = (pumpIt.G("y")).AsF();
      Gv sv = net.G("byId").I(src_3.G("att").G("id"));
      Gv Tsrc = _src_temp(sb);
      Gv cap_s = _src_cap(sb);
      Gv lvl_frac = (Gd.clampf((_src_level(sb) / Gd.maxf(Gv.F(1.0), cap_s)), Gv.F(0.0), Gv.F(1.0))).AsF();
      Gv lvl_m = ((lvl_frac * sv.G("h")) / PX_PER_M);
      Gv head = ((((pump_y - ((sv.G("y") + sv.G("h"))))) / PX_PER_M) + lvl_m);
      Gv fl_v = _src_fluid(sb);
      Gv th0 = (((fl_v.C("has", Gv.S("g"))).T() ? thick_factor(fl_v) : Gv.F(0.0))).AsF();
      Gv rho = (((fl_v.C("has", Gv.S("g"))).T() ? density(fl_v) : Gv.F(1.0))).AsF();
      Gv mu = ((skViscosity(Tsrc, ((fl_v.C("has", Gv.S("g"))).T() ? plato(fl_v) : Gv.F(0.0))) * ((Gv.F(1.0) + ((Gv.F(60.0) * th0) * th0))))).AsF();
      Gv y_lvl = (((sv.G("y") + sv.G("h")) - (lvl_frac * sv.G("h")))).AsF();
      Gv Hst = Gv.F(-99.0);
      foreach (Gv dp_ in (dpaths).Iter()) {
        Gv last = ((dp_.C("size")).T() ? dp_.I((dp_.C("size") - Gv.I(1L))) : Gv.Null);
        if ((Gv.Ne(last, Gv.Null) && (last.C("has", Gv.S("to"))).T())) {
          Hst = Gd.maxf(Hst, (((y_lvl - net.G("nodes").I(last.G("to")).G("y"))) / PX_PER_M));
        }
      }
      if (Gv.Lt(Hst, Gv.I(-98L))) {
        Hst = Gv.F(0.0);
      }
      Gv HY = pump_point(key, q_2.G("flow"), Hst, sp_, dpaths, rho, mu, th0);
      q_2 = q_2.S("hz", HY.G("hz"));
      q_2 = q_2.S("bar", HY.G("bar"));
      q_2 = q_2.S("hyd", HY.G("why"));
      rp = (HY.G("Q") / Gd.maxf(Gv.F(1.0), q_2.G("flow")));
      Gv Pv = (Gd.pow(Gv.F(10.0), (Gv.F(8.07131) - (Gv.F(1730.63) / ((Gv.F(233.426) + Tsrc))))) * Gv.F(0.00133322));
      Gv npsha = ((((((Gv.F(0.99) - Pv)) * Gv.F(10.2)) / rho) + head) - HY.G("hs_loss"));
      q_2 = q_2.S("npsh", npsha);
      Gv cav_f = Gd.clampf((((npsha - Gv.F(0.8))) / Gv.F(2.2)), Gv.F(0.12), Gv.F(1.0));
      if (Gv.Lt(cav_f, Gv.F(0.85))) {
        once((Gv.S("cav%s%d") % Gd.Arr(key, tb(Gv.I(600L)))), (Gv.S("%s kavituje (sání z %s %s °C, výška %s m, NPSH %s m) – čerpadlo chrastí a nedává plný průtok. Zpomal ho, nech vychladnout nebo dolej nádobu.") % Gd.Arr(pname, VN(sb), fmt0(Tsrc), fmt1(head), fmt1(npsha))), Gv.S("warn"));
      }
      if (Gv.Eq(HY.G("why"), Gv.S("max"))) {
        once((Gv.S("pmax%s%d") % Gd.Arr(key, tb(Gv.I(900L)))), (Gv.S("%s jede na plné otáčky (50 Hz) a nastavených %d L/min přes tuhle trasu nedá – teče %d L/min (odpor trasy, výška, hustota).") % Gd.Arr(pname, Gd.roundi(q_2.G("flow")), Gd.roundi(HY.G("Q")))), Gv.S("warn"));
      }
      if (Gv.Eq(HY.G("why"), Gv.S("lift"))) {
        once((Gv.S("plift%s%d") % Gd.Arr(key, tb(Gv.I(900L)))), (Gv.S("%s nepřekoná výškový rozdíl %s m – nic neteče.") % Gd.Arr(pname, fmt1(Hst))), Gv.S("bad"));
      }
      rp = (rp * cav_f);
      Gv th = (Gv.F(0.0)).AsF();
      Gv cg = (Gv.F(0.0)).AsF();
      Gv mv_src = (Gv.Eq(sb, Gv.S("rvp")) ? S.G("rvp") : (((Gv.Eq(sb, Gv.S("sk")) && Gv.Gt(vol(S.G("sk").G("mash")), Gv.I(5L))) ? S.G("sk").G("mash") : Gv.Null)));
      if (Gv.Ne(mv_src, Gv.Null)) {
        th = (thick_factor(mv_src)).AsF();
        rp = (rp / (Gv.F(1.0) + (Gv.F(0.5) * th)));
        cg = ((S.G("clog").C("get", sb, Gv.F(0.0)) * th)).AsF();
        if (Gv.Gt(cg, Gv.F(0.05))) {
          rp = (rp * (Gv.F(1.0) - (Gv.F(0.85) * cg)));
          if (Gv.Gt(cg, Gv.F(0.35))) {
            once((Gv.S("clog%s%d") % Gd.Arr(sb, tb(Gv.I(600L)))), (Gv.S("%s: zrno se bez míchání usadilo nad výpustí a ucpává ji – rmut teče pomalu. Zapni %s a nech čerpadlo rmut protlačit (zrno se uvolní, voda ze rmutu pomůže).") % Gd.Arr(VN(sb), (Gv.Eq(sb, Gv.S("rvp")) ? Gv.S("míchadlo") : Gv.S("kopačku")))), Gv.S("warn"));
          }
        }
      }
      if ((Gv.Ge(head, Gv.F(0.3)) && Gv.Lt(cg, Gv.F(0.75)))) {
        q_2 = q_2.S("prime", Gd.minf(Gv.F(1.0), (q_2.G("prime") + (dt / ((Gv.F(6.0) + (Gv.F(10.0) * th)))))));
      } else if (Gv.Ge(cg, Gv.F(0.75))) {
        q_2 = q_2.S("prime", Gd.maxf(Gv.F(0.0), (q_2.G("prime") - (dt / Gv.F(10.0)))));
      }
      if ((Gv.Lt(head, Gv.F(0.3)) && Gv.Lt(q_2.G("prime"), Gv.F(0.95)))) {
        q_2 = q_2.S("status", Gv.S("nezavodněno – hladina pod čerpadlem, nenasaje"));
        once((Gv.S("prime%s%d") % Gd.Arr(key, tb(Gv.I(600L)))), (Gv.S("%s se nezavodnilo: hladina v %s je pod čerpadlem (%s m), odstředivé čerpadlo samo nenasaje. Dolej nádobu nebo zavodni sání.") % Gd.Arr(pname, VN(sb), fmt1(head))), Gv.S("warn"));
        q_2 = q_2.S("dry", (q_2.G("dry") + dt));
        if (Gv.Gt(q_2.G("dry"), Gv.F(60.0))) {
          q_2 = q_2.S("on", Gv.B(false));
          q_2 = q_2.S("dry", Gv.F(0.0));
          logm((Gv.S("Ochrana chodu nasucho vypnula %s – běželo nezavodněné.") % pname), Gv.S("bad"));
        }
        continue;
      }
      if (Gv.Lt(q_2.G("prime"), Gv.F(0.95))) {
        once((Gv.S("startdry%s%d") % Gd.Arr(key, tb(Gv.I(120L)))), (Gv.S("%s zapnuto dřív, než se zavodnilo těleso – točí se napůl na sucho. Nejdřív otevři klapku před čerpadlem a počkej, až naběhne modrá tečka u čerpadla, pak ho zapni.") % pname), Gv.S("warn"));
        q_2 = q_2.S("dry", (q_2.G("dry") + (dt * ((Gv.F(1.0) - q_2.G("prime"))))));
        q_2 = q_2.S("status", (Gv.S("zavodňuje se (%d %%)") % Gd.roundi((q_2.G("prime") * Gv.F(100.0)))));
        if (Gv.Gt(q_2.G("dry"), Gv.F(40.0))) {
          q_2 = q_2.S("on", Gv.B(false));
          q_2 = q_2.S("dry", Gv.F(0.0));
          logm((Gv.S("Ochrana chodu nasucho vypnula %s – těleso čerpadla se nezavodnilo (otevři klapku před čerpadlem a nech čerpadlo zavodnit, než ho zapneš; ucpanou výpust nejdřív uvolni).") % pname), Gv.S("bad"));
          continue;
        }
      } else {
        q_2 = q_2.S("dry", Gv.F(0.0));
      }
      Gv Qf = (((q_2.G("flow") * rp) * q_2.G("prime"))).AsF();
      q_2 = q_2.S("q", Qf);
      if (Gv.Ge(Qf, Gv.F(0.5))) {
        q_2 = q_2.S("status", (q_2.G("status") + (Gv.S(" · %d Hz · %s bar") % Gd.Arr(Gd.roundi(q_2.C("get", Gv.S("hz"), Gv.F(0.0))), fmt1(q_2.C("get", Gv.S("bar"), Gv.F(0.0)))))));
      }
      if (Gv.Lt(Qf, Gv.F(0.5))) {
        if (Gv.Ge(q_2.G("prime"), Gv.F(0.95))) {
          q_2 = q_2.S("status", Gv.S("přiškrceno – nic neteče"));
        }
        continue;
      }
      q_2 = q_2.S("dead", Gv.F(0.0));
      q_2 = q_2.S("flowing", Gv.B(true));
      if ((Gv.Ne(mv_src, Gv.Null) && Gv.Gt(S.G("clog").C("get", sb, Gv.F(0.0)), Gv.F(0.0)))) {
        S = S.S("clog", S.G("clog").SI(sb, Gd.maxf(Gv.F(0.0), (S.G("clog").I(sb) - (dt / Gv.F(120.0))))));
      }
      Gv part0 = srcTake(sb, Qf, m, key);
      if (Gv.Eq(part0, Gv.Null)) {
        q_2 = q_2.S("on", Gv.B(false));
        logm((Gv.S("%s zastaveno – %s je prázdná.") % Gd.Arr(pname, net.G("byId").I(src_3.G("att").G("id")).C("get", Gv.S("label"), sb))));
        continue;
      }
      Gv turb = (S.G("sk").G("turb")).AsF();
      Gv part_2 = push_path(suction_path(sp_), part0, Qf, m);
      Gv nAll = (dests2.C("size") + lks.C("size"));
      if ((lks.C("size")).T()) {
        q_2 = q_2.S("status", (q_2.G("status") + ((((dests2.C("size")).T() ? Gv.S(" + ") : Gv.S(""))) + Gv.S("VYTÉKÁ z otevřené trubky"))));
      }
      foreach (Gv i_2 in (nAll).Iter()) {
        Gv pp = (Gv.Eq(i_2, (nAll - Gv.I(1L))) ? part_2 : takePart(part_2, (vol(part_2) / ((nAll - i_2))), Gv.F(1.0)));
        if (Gv.Ge(i_2, dests2.C("size"))) {
          Gv kd = contentKind(pp);
          spill(rd, lks.I((i_2 - dests2.C("size"))), pp, (Qf / nAll), m, (Gv.S("%s: %s") % Gd.Arr(pname, (Gv.Eq(kd, Gv.S("mash")) ? Gv.S("dílo") : ((Gv.Eq(kd, Gv.S("wort")) ? Gv.S("sladina / mladina") : Gv.S("kapalina")))))));
          continue;
        }
        Gv d_15 = dests2.I(i_2);
        if (Gv.Eq(d_15.G("att").G("bind"), Gv.S("spilka"))) {
          Gv ce = S.C("get", Gv.S("_castEdges"), Gd.Dict());
          foreach (Gv s_14 in (sp_).Iter()) {
            if ((s_14.C("has", Gv.S("e"))).T()) {
              ce = ce.SI(s_14.G("e").G("id"), Gv.B(true));
            }
          }
          foreach (Gv s_15 in (path_to(rd, d_15.G("k"))).Iter()) {
            if ((s_15.C("has", Gv.S("e"))).T()) {
              ce = ce.SI(s_15.G("e").G("id"), Gv.B(true));
            }
          }
          S = S.SI(Gv.S("_castEdges"), ce);
        }
        Gv out__3 = push_path(path_to(rd, d_15.G("k")), pp, (Qf / nAll), m);
        _stream(d_15, out__3);
        dstGive(d_15.G("att").G("bind"), out__3, Gd.Dict(Gv.S("src"), sb, Gv.S("turb"), turb, Gv.S("m"), m));
      }
      Gv used_p = Gd.Dict();
      Gv me_p = (_dead_due(key, m)).AsF();
      Gv pplan = Gd.Arr();
      if (Gv.Gt(me_p, Gv.F(0.0))) {
        foreach (Gv d_16 in (dests2).Iter()) {
          foreach (Gv s_16 in (path_to(rd, d_16.G("k"))).Iter()) {
            if ((s_16.C("has", Gv.S("e"))).T()) {
              used_p = used_p.SI(s_16.G("e").G("id"), Gv.B(true));
            }
          }
        }
        foreach (Gv lk_6 in (lks).Iter()) {
          foreach (Gv s_17 in (path_to(rd, lk_6.G("k"))).Iter()) {
            if ((s_17.C("has", Gv.S("e"))).T()) {
              used_p = used_p.SI(s_17.G("e").G("id"), Gv.B(true));
            }
          }
        }
        pplan = _dead_end_plan(rd, used_p, (Qf * Gv.F(0.5)), me_p);
      }
      Gv Lp = Gv.F(0.0);
      foreach (Gv x_4 in (pplan).Iter()) {
        Lp = (Lp + x_4.I(Gv.I(1L)));
      }
      if (Gv.Gt(Lp, Gv.F(0.01))) {
        Gv pfill = srcTake(sb, (Lp / me_p), me_p, key);
        if (Gv.Ne(pfill, Gv.Null)) {
          foreach (Gv xi in (pplan.C("size")).Iter()) {
            Gv x2 = pplan.I(xi);
            Gv piece = (Gv.Eq(xi, (pplan.C("size") - Gv.I(1L))) ? pfill : takePart(pfill, Gd.minf(x2.I(Gv.I(1L)), vol(pfill)), Gv.F(1.0)));
            if (Gv.Gt(vol(piece), Gv.F(1e-06))) {
              edge_push(x2.I(Gv.I(0L)), piece, Gv.B(true));
            }
          }
        }
      }
      if (Gv.Eq(sb, Gv.S("mp"))) {
        foreach (Gv d_17 in (dests2).Iter()) {
          if (Gv.Eq(d_17.G("att").G("bind"), Gv.S("mp"))) {
            S = S.S("mp", S.G("mp").S("whirlNow", Gv.B(true)));
          }
        }
      }
      Gv dl_2 = Gd.Arr();
      foreach (Gv d_18 in (dests2).Iter()) {
        dl_2.C("append", d_18.G("att").G("bind"));
      }
      S.G("flows").C("append", Gd.Dict(Gv.S("pump"), pumpIt.G("id"), Gv.S("src"), sb, Gv.S("dst"), dl_2));
    }
    if ((prof_on).T()) {
      prof = prof.SI(Gv.S("n_cerpadla"), ((prof.C("get", Gv.S("n_cerpadla"), Gv.I(0L)) + Gd.Static("Time", "get_ticks_usec")) - _n0));
      _n0 = Gd.Static("Time", "get_ticks_usec");
    }
    foreach (Gv v_2 in (net.G("gravity")).Iter()) {
      Gv b_3 = v_2.C("get", Gv.S("bind"));
      Gv gk = (Gv.S("gd|") + v_2.G("id"));
      Gv outs_2 = _cached(gk);
      if (Gv.Eq(outs_2, Gv.Null)) {
        outs_2 = Gd.Arr();
        foreach (Gv s0 in (starts_of(v_2.G("id"), Gv.B(true))).Iter()) {
          Gv r_5 = bfs(Gd.Arr(s0));
          Gv lk_7 = Gv.Null;
          foreach (Gv l in (r_5.G("leaks")).Iter()) {
            if (Gv.Ge(net.G("nodes").I(l.G("k")).G("y"), (v_2.G("y") + (v_2.G("h") * Gv.F(0.6))))) {
              lk_7 = l;
              break;
            }
          }
          if (Gv.Eq(lk_7, Gv.Null)) {
            continue;
          }
          Gv path_3 = path_to(r_5, lk_7.G("k"));
          Gv f = (path_pct(path_3) / Gd.sqrt((Gv.F(1.0) + (path_len(path_3) / Gv.F(6000.0)))));
          if (Gv.Gt(f, Gv.I(0L))) {
            outs_2.C("append", Gd.Dict(Gv.S("path"), edges_only(path_3), Gv.S("f"), f, Gv.S("lk"), lk_7));
          }
        }
        _cache = _cache.SI(gk, outs_2);
      }
      if (!(outs_2.C("size")).T()) {
        continue;
      }
      Gv fr = Gv.F(0.0);
      {
        Gv _t1 = b_3;
        if (Gv.Eq(_t1, Gv.S("sk"))) {
          fr = ((skVolTotal() / c.G("skCap"))).AsF();
        }
        else if (Gv.Eq(_t1, Gv.S("hlt"))) {
          fr = ((S.G("hlt").G("w") / c.G("hltCap"))).AsF();
        }
        else if (Gv.Eq(_t1, Gv.S("lv"))) {
          fr = ((S.G("lv").G("w") / c.G("lvVol"))).AsF();
        }
        else if (Gv.Eq(_t1, Gv.S("rvp"))) {
          fr = ((vol(S.G("rvp")) / c.G("rvpCap"))).AsF();
        }
        else if (Gv.Eq(_t1, Gv.S("wp"))) {
          fr = ((vol(S.G("wp")) / c.G("wpCap"))).AsF();
        }
        else if (true) {
          fr = ((vol(S.G("mp")) / c.G("mpCap"))).AsF();
        }
      }
      Gv head_m = ((Gd.maxf(Gv.F(0.02), fr) * v_2.G("h")) / PX_PER_M);
      Gv tot = (Gd.minf(Gv.F(240.0), (Gv.F(120.0) + (Gv.F(30.0) * outs_2.C("size")))) * Gd.minf(Gv.F(1.3), Gd.sqrt((head_m / Gv.F(1.05)))));
      foreach (Gv o_4 in (outs_2).Iter()) {
        Gv Lmin = (((tot * o_4.G("f")) / outs_2.C("size"))).AsF();
        Gv part_3 = Gv.Null;
        if (Gv.Eq(b_3, Gv.S("lv"))) {
          Gv L_2 = Gd.minf((Lmin * m), Gd.maxf(Gv.F(0.0), S.G("lv").G("w")));
          if (Gv.Gt(L_2, Gv.F(0.01))) {
            S = S.S("lv", S.G("lv").S("w", (S.G("lv").G("w") - L_2)));
            part_3 = lp();
            part_3 = part_3.S("w", L_2);
            part_3 = part_3.S("T", S.G("lv").G("T"));
          }
        } else {
          part_3 = srcTake(b_3, Lmin, m, (Gv.Eq(b_3, Gv.S("sk")) ? Gv.S("p2") : Gv.S("drain")));
        }
        if (Gv.Eq(part_3, Gv.Null)) {
          continue;
        }
        Gv kd_2 = contentKind(part_3);
        Gv out__4 = push_path(o_4.G("path"), part_3, Lmin, m);
        S = S.S("drainL", (S.G("drainL") + vol(out__4)));
        S = S.S("drainExt", (S.G("drainExt") + (((ext(out__4) + out__4.G("su")) + out__4.G("sc")) + out__4.G("ni"))));
        Gv n = net.G("nodes").I(o_4.G("lk").G("k"));
        S.G("leaks").C("append", Gd.Ctor("Vector2", n.G("x"), n.G("y")));
        once((Gv.S("gdrain%s%d") % Gd.Arr(b_3, tb(Gv.I(900L)))), (Gv.S("%s: %s vytéká spodem otevřenými klapkami do kanálu %s.") % Gd.Arr(v_2.C("get", Gv.S("label"), b_3), (Gv.Eq(kd_2, Gv.S("mash")) ? Gv.S("dílo") : ((Gv.Eq(kd_2, Gv.S("wort")) ? Gv.S("sladina") : Gv.S("voda")))), leak_where(o_4.G("lk").G("k")))), Gv.S("warn"));
      }
    }
    _vessel_gravity(m);
    skHatchStep(m);
    return Gv.Null;
  }
  public Gv skHatchStep(Gv m = default(Gv)) {
    m = (m).AsF();
    Gv sk = S.G("sk");
    if (!(sk.C("get", Gv.S("hatch"), Gv.B(false))).T()) {
      return Gv.Null;
    }
    if ((Gv.Lt(skVolTotal(), Gv.F(0.5)) && Gv.Lt(sk.G("g"), Gv.F(0.01)))) {
      return Gv.Null;
    }
    Gv Lq = ((Gv.F(450.0) * m)).AsF();
    Gv out_L = Gv.F(0.0);
    if (Gv.Gt(vol(sk.G("mash")), Gv.F(0.5))) {
      Gv pm = takePart(sk.G("mash"), Lq, Gv.F(1.0));
      out_L = (out_L + vol(pm));
      Lq = ((Lq - vol(pm))).AsF();
      S = S.S("drainExt", (S.G("drainExt") + (((ext(pm) + pm.G("su")) + pm.G("sc")) + pm.G("ni"))));
    }
    foreach (Gv i in (Gd.Arr(Gv.I(0L), Gv.I(1L), Gv.I(2L), Gv.I(3L), Gv.I(4L))).Iter()) {
      if (Gv.Le(Lq, Gv.F(0.001))) {
        break;
      }
      Gv tk = takeCell(sk.G("cells").I(i), Lq);
      Gv tv = (cellVol(tk)).AsF();
      S = S.S("drainExt", (S.G("drainExt") + (tk.G("f") + tk.G("n"))));
      out_L = (out_L + tv);
      Lq = ((Lq - tv)).AsF();
    }
    Gv dg = Gv.F(0.0);
    if (Gv.Gt(sk.G("g"), Gv.F(0.01))) {
      dg = Gd.minf(sk.G("g"), ((Gv.F(6.0) * (((sk.G("rake")).T() ? Gv.F(4.0) : Gv.F(1.0)))) * m));
      sk = sk.S("g", (sk.G("g") - dg));
      sk = sk.S("lostExt", (sk.G("lostExt") + Gv.F(0.0)));
      if (Gv.Lt(sk.G("g"), Gv.F(0.01))) {
        sk = sk.S("g", Gv.F(0.0));
      }
    }
    if ((Gv.Gt(out_L, Gv.F(0.0)) || Gv.Gt(dg, Gv.F(0.0)))) {
      S = S.S("spillL", (S.G("spillL") + out_L));
      Gv hs = items_by_bind(Gv.S("sk"));
      if ((hs.C("size")).T()) {
        foreach (Gv it in (net.G("byId").C("values")).Iter()) {
          if ((Gv.Eq(it.G("type"), Gv.S("deco")) && Gv.Eq(it.C("get", Gv.S("k"), Gv.S("")), Gv.S("hatch")))) {
            S.G("leaks").C("append", Gd.Ctor("Vector2", it.G("x"), (it.G("y") + Gv.I(24L))));
            break;
          }
        }
      }
      once((Gv.S("hatch%d") % tb(Gv.I(900L))), (Gv.S("Výhoz mláta je otevřený: kapalina odtéká do kanálu%s.") % Gd.Arr(((Gv.Gt(sk.G("g"), Gv.F(0.01)) || Gv.Gt(dg, Gv.F(0.0))) ? Gv.S(", mláto vypadává") : Gv.S("")))), Gv.S("warn"));
    }
    return Gv.Null;
  }
  public Gv _vessel_gravity(Gv m = default(Gv)) {
    m = (m).AsF();
    foreach (Gv gb in (Gd.Arr(Gv.S("rvp"), Gv.S("sk"), Gv.S("hlt"), Gv.S("wp"), Gv.S("spilka"))).Iter()) {
      Gv gvl = items_by_bind(gb);
      if ((!(gvl.C("size")).T() || Gv.Lt(_src_level(gb), Gv.F(8.0)))) {
        continue;
      }
      Gv gv = gvl.I(Gv.I(0L));
      Gv sts = starts_of(gv.G("id"), Gv.B(true));
      if (!(sts.C("size")).T()) {
        continue;
      }
      Gv oy = Gv.F(-1000000000.0);
      foreach (Gv sk_ in (sts).Iter()) {
        oy = Gd.maxf(oy, net.G("nodes").I(sk_).G("y"));
      }
      Gv sy = (main.C("level_y", gv)).AsF();
      if (Gv.Ge(sy, oy)) {
        continue;
      }
      Gv gr = bfs(sts);
      Gv bh = Gv.Null;
      Gv bhd = Gv.F(0.03);
      foreach (Gv h in (gr.G("hits")).Iter()) {
        Gv db = h.G("att").G("bind");
        if ((Gv.Eq(db, gb) || Gv.In(db, Gd.Arr(Gv.S("lv"), Gv.S("vodovod"), Gv.S("mp"))))) {
          continue;
        }
        Gv dvl = items_by_bind(db);
        if (!(dvl.C("size")).T()) {
          continue;
        }
        Gv ref_y = (Gd.minf(main.C("level_y", dvl.I(Gv.I(0L))), net.G("nodes").I(h.G("k")).G("y"))).AsF();
        Gv dh = ((((ref_y - sy)) / PX_PER_M)).AsF();
        if (Gv.Gt(dh, bhd)) {
          bhd = dh;
          bh = h;
        }
      }
      if (Gv.Eq(bh, Gv.Null)) {
        continue;
      }
      Gv db2 = bh.G("att").G("bind");
      Gv dv2 = items_by_bind(db2).I(Gv.I(0L));
      Gv pa = path_to(gr, bh.G("k"));
      Gv f = ((path_pct(pa) / Gd.sqrt((Gv.F(1.0) + (path_len(pa) / Gv.F(6000.0)))))).AsF();
      if (Gv.Le(f, Gv.F(0.0))) {
        continue;
      }
      Gv q = (((Gv.F(120.0) * f) * Gd.minf(Gv.F(1.3), Gd.sqrt((bhd / Gv.F(1.05)))))).AsF();
      Gv th = Gv.F(0.0);
      if (Gv.Eq(gb, Gv.S("rvp"))) {
        th = (thick_factor(S.G("rvp")) * ((Gv.F(1.0) - (Gv.F(0.85) * S.G("clog").C("get", Gv.S("rvp"), Gv.F(0.0))))));
      } else if ((Gv.Eq(gb, Gv.S("sk")) && Gv.Gt(vol(S.G("sk").G("mash")), Gv.I(5L)))) {
        th = (thick_factor(S.G("sk").G("mash")) * ((Gv.F(1.0) - (Gv.F(0.85) * S.G("clog").C("get", Gv.S("sk"), Gv.F(0.0))))));
      }
      q = ((q / (Gv.F(1.0) + (Gv.F(0.5) * th)))).AsF();
      Gv As = (((_src_cap(gb) / Gv.F(1000.0)) / Gd.maxf(Gv.F(0.5), (gv.G("h") / PX_PER_M)))).AsF();
      Gv Ad = (((_src_cap(db2) / Gv.F(1000.0)) / Gd.maxf(Gv.F(0.5), (dv2.G("h") / PX_PER_M)))).AsF();
      Gv v_eq = (((bhd * (((As * Ad) / ((As + Ad))))) * Gv.F(1000.0))).AsF();
      Gv room = ((_src_cap(db2) - _src_level(db2))).AsF();
      Gv Lg = (Gd.minf((q * m), Gd.minf((v_eq * Gv.F(0.6)), room))).AsF();
      if (Gv.Lt(Lg, Gv.F(0.02))) {
        continue;
      }
      Gv gp = srcTake(gb, (Lg / m), m, Gv.S("drain"));
      if (Gv.Eq(gp, Gv.Null)) {
        continue;
      }
      Gv gout = push_path(pa, gp, (Lg / m), m);
      _stream(bh, gout);
      dstGive(db2, gout, Gd.Dict(Gv.S("src"), gb, Gv.S("turb"), S.G("sk").G("turb"), Gv.S("m"), m));
      once((Gv.S("gravx%s%s%d") % Gd.Arr(gb, db2, tb(Gv.I(600L)))), (Gv.S("%s → %s: kapalina přetéká samospádem otevřenými klapkami (hladina v %s je výš). Zavři klapku, jestli nemá téct.") % Gd.Arr(VN(gb), VN(db2), VN(gb))), Gv.S("warn"));
    }
    return Gv.Null;
  }
  public Gv _dead_end_plan(Gv r = default(Gv), Gv used = default(Gv), Gv rate = default(Gv), Gv m = default(Gv)) {
    rate = (rate).AsF();
    m = (m).AsF();
    Gv out_ = Gd.Arr();
    foreach (Gv e in (net.G("edges")).Iter()) {
      if ((Gv.Eq(e.G("medium"), Gv.S("steam")) || (used.C("has", e.G("id"))).T())) {
        continue;
      }
      Gv a_in = r.G("prev").C("has", e.G("a"));
      Gv b_in = r.G("prev").C("has", e.G("b"));
      if (!((a_in).T() || (b_in).T())) {
        continue;
      }
      if (!((a_in).T() && (b_in).T())) {
        Gv other = net.G("nodes").I(((a_in).T() ? e.G("b") : e.G("a")));
        if ((Gv.Eq(other.G("valve"), Gv.Null) || (valve_open(other.G("valve"))).T())) {
          continue;
        }
      }
      if (Gv.Gt(ext(e.G("c")), Gv.F(0.05))) {
        continue;
      }
      Gv room = ((e.G("cap") - vol(e.G("c")))).AsF();
      if (Gv.Lt(room, Gv.F(0.01))) {
        continue;
      }
      out_.C("append", Gd.Arr(e, Gd.minf(room, (rate * m))));
    }
    return out_;
    return Gv.Null;
  }
  public Gv _dead_due(Gv key = default(Gv), Gv m = default(Gv)) {
    m = (m).AsF();
    Gv a = ((_dead_acc.C("get", key, Gv.F(0.0)) + m)).AsF();
    if (Gv.Lt(a, (Gv.F(2.0) / Gv.F(60.0)))) {
      _dead_acc = _dead_acc.SI(key, a);
      return (Gv.F(0.0)).AsF();
    }
    _dead_acc = _dead_acc.SI(key, Gv.F(0.0));
    return (a).AsF();
    return Gv.Null;
  }
  public Gv _dead_end_apply(Gv plan = default(Gv), Gv T = default(Gv)) {
    T = (T).AsF();
    foreach (Gv x in (plan).Iter()) {
      Gv part = lp();
      part = part.S("w", x.I(Gv.I(1L)));
      part = part.S("T", T);
      edge_push(x.I(Gv.I(0L)), part, Gv.B(true));
    }
    return Gv.Null;
  }
  public Gv pump_interlock(Gv key = default(Gv)) {
    Gv pis = items_by_bind(key);
    if (!(pis.C("size")).T()) {
      return Gv.S("");
    }
    Gv pn = net.G("pumpNodes").C("get", pis.I(Gv.I(0L)).G("id"), Gd.Dict(Gv.S("in"), Gd.Arr(), Gv.S("out"), Gd.Arr()));
    Gv rs = bfs(pn.G("in"));
    Gv src_ok = Gv.B(false);
    foreach (Gv hh in (rs.G("hits")).Iter()) {
      if ((((hh.G("att").G("low")).T() && Gv.In(hh.G("att").G("bind"), Gd.Arr(Gv.S("rvp"), Gv.S("mp"), Gv.S("sk"), Gv.S("wp"), Gv.S("hlt"), Gv.S("spilka")))) && Gv.Gt(_src_level(hh.G("att").G("bind")), Gv.I(5L)))) {
        src_ok = Gv.B(true);
      }
    }
    if (!(src_ok).T()) {
      return Gv.S("sání není otevřené z nádoby s kapalinou – čerpadlo by běželo nasucho a zavzdušnilo se. Otevři klapky před čerpadlem.");
    }
    Gv rd = bfs(pn.G("out"));
    Gv dst_ok = Gv.B(false);
    foreach (Gv d in (rd.G("hits")).Iter()) {
      if (Gv.Ne(d.G("att").G("bind"), Gv.S("vodovod"))) {
        dst_ok = Gv.B(true);
      }
    }
    if ((!(dst_ok).T() && Gv.Eq(rd.G("leaks").C("size"), Gv.I(0L)))) {
      return Gv.S("výtlak je zavřený – čerpadlo by tlačilo do zavřeného potrubí (tlaková špička, ohřev ucpávky). Otevři cestu za čerpadlem.");
    }
    return Gv.S("");
    return Gv.Null;
  }
  public Gv pump_warn(Gv key = default(Gv)) {
    Gv pis = items_by_bind(key);
    if (!(pis.C("size")).T()) {
      return Gv.S("");
    }
    Gv pn = net.G("pumpNodes").C("get", pis.I(Gv.I(0L)).G("id"), Gd.Dict(Gv.S("in"), Gd.Arr(), Gv.S("out"), Gd.Arr()));
    if ((bfs(pn.G("out")).G("leaks").C("size")).T()) {
      return Gv.S("výtlak vede i do otevřeného konce trubky – část poteče na zem.");
    }
    return Gv.S("");
    return Gv.Null;
  }
  public Gv _pump_prime_idle(Gv key = default(Gv), Gv q = default(Gv), Gv el = default(Gv)) {
    el = (el).AsF();
    if ((q.C("get", Gv.S("airLock"), Gv.B(false))).T()) {
      return Gv.Null;
    }
    Gv pis = items_by_bind(key);
    if (!(pis.C("size")).T()) {
      return Gv.Null;
    }
    Gv pumpIt = pis.I(Gv.I(0L));
    Gv pn = net.G("pumpNodes").C("get", pumpIt.G("id"), Gd.Dict(Gv.S("in"), Gd.Arr(), Gv.S("out"), Gd.Arr()));
    Gv rs = bfs(pn.G("in"));
    Gv src = Gv.Null;
    foreach (Gv hh in (rs.G("hits")).Iter()) {
      if ((((hh.G("att").G("low")).T() && Gv.In(hh.G("att").G("bind"), Gd.Arr(Gv.S("rvp"), Gv.S("mp"), Gv.S("sk"), Gv.S("wp"), Gv.S("hlt"), Gv.S("spilka")))) && Gv.Gt(_src_level(hh.G("att").G("bind")), Gv.I(5L)))) {
        src = hh;
        break;
      }
    }
    if (Gv.Eq(src, Gv.Null)) {
      return Gv.Null;
    }
    Gv sb = src.G("att").G("bind");
    Gv sv = net.G("byId").I(src.G("att").G("id"));
    Gv lvl_m = ((Gd.clampf((_src_level(sb) / Gd.maxf(Gv.F(1.0), _src_cap(sb))), Gv.F(0.0), Gv.F(1.0)) * sv.G("h")) / PX_PER_M);
    Gv head = ((((pumpIt.G("y") - ((sv.G("y") + sv.G("h"))))) / PX_PER_M) + lvl_m);
    if (Gv.Lt(head, Gv.F(0.3))) {
      return Gv.Null;
    }
    Gv th = (Gv.F(0.0)).AsF();
    if (Gv.Eq(sb, Gv.S("rvp"))) {
      th = (thick_factor(S.G("rvp"))).AsF();
    } else if ((Gv.Eq(sb, Gv.S("sk")) && Gv.Gt(vol(S.G("sk").G("mash")), Gv.I(5L)))) {
      th = (thick_factor(S.G("sk").G("mash"))).AsF();
    }
    q = q.S("prime", Gd.minf(Gv.F(1.0), (q.G("prime") + (el / ((Gv.F(6.0) + (Gv.F(10.0) * th)))))));
    return Gv.Null;
  }
  public Gv _src_fluid(Gv b = default(Gv)) {
    {
      Gv _t1 = b;
      if (Gv.Eq(_t1, Gv.S("rvp")) || Gv.Eq(_t1, Gv.S("mp")) || Gv.Eq(_t1, Gv.S("wp"))) {
        return S.I(b);
      }
      else if (Gv.Eq(_t1, Gv.S("sk"))) {
        return (Gv.Gt(vol(S.G("sk").G("mash")), Gv.I(5L)) ? S.G("sk").G("mash") : Gd.Dict(Gv.S("w"), Gv.F(1000.0), Gv.S("f"), (cellP(S.G("sk").G("cells").I(Gv.I(4L))) * Gv.F(10.0)), Gv.S("dx"), Gv.F(0.0), Gv.S("ld"), Gv.F(0.0), Gv.S("ns"), Gv.F(0.0), Gv.S("sg"), Gv.F(0.0), Gv.S("g"), Gv.F(0.0), Gv.S("su"), Gv.F(0.0), Gv.S("sc"), Gv.F(0.0), Gv.S("ni"), Gv.F(0.0), Gv.S("T"), S.G("sk").G("T")));
      }
    }
    return Gd.Dict();
    return Gv.Null;
  }
  public Gv mains_solve(Gv outs = default(Gv), Gv src = default(Gv)) {
    Gv res = Gd.Arr();
    foreach (Gv _i in (outs.C("size")).Iter()) {
      res.C("append", Gv.F(0.0));
    }
    Gv c = S.G("cfg");
    Gv Pbar = (Gd.Dyn("float", c.C("get", Gv.S("mainsBar"), Gv.F(4.0)))).AsF();
    Gv Qmax = (Gd.maxf(Gv.F(10.0), Gd.Dyn("float", c.C("get", Gv.S("mainsQ"), Gv.F(900.0))))).AsF();
    Gv Hm = (((Pbar * Gv.F(100000.0)) / ((Gv.F(1000.0) * Gv.F(9.81))))).AsF();
    Gv Tw = (S.G("cool").G("waterT")).AsF();
    Gv ysrc = (Gv.F(0.0)).AsF();
    Gv st = starts_of(src.G("id"));
    if (((st.C("size")).T() && (net.G("nodes").C("has", st.I(Gv.I(0L)))).T())) {
      ysrc = (net.G("nodes").I(st.I(Gv.I(0L))).G("y")).AsF();
    }
    Gv sig = (Gv.S("%.3f|%.1f|%.1f|%.0f|%.2f|%d") % Gd.Arr(Pbar, Qmax, Tw, ysrc, Gd.Dyn("float", c.C("get", Gv.S("pipeLenScale"), Gv.F(0.4))), outs.C("size")));
    Gv hst = Gd.Arr();
    foreach (Gv o in (outs).Iter()) {
      Gv last = ((o.G("path").C("size")).T() ? o.G("path").I((o.G("path").C("size") - Gv.I(1L))) : Gv.Null);
      Gv hs = (Gv.F(0.0)).AsF();
      if (((Gv.Ne(last, Gv.Null) && (last.C("has", Gv.S("to"))).T()) && (net.G("nodes").C("has", last.G("to"))).T())) {
        hs = ((((ysrc - net.G("nodes").I(last.G("to")).G("y"))) / PX_PER_M)).AsF();
      }
      hst.C("append", hs);
      sig = (sig + (Gv.S("|%.3f,%d,%.1f,%.1f,%.1f") % Gd.Arr(o.G("f"), o.G("path").C("size"), path_len(o.G("path")), hs, ((Gd.Dyn("float", c.C("get", Gv.S("bore50"), Gv.F(75.0))) + Gd.Dyn("float", c.C("get", Gv.S("bore25"), Gv.F(50.0)))) + Gd.Dyn("float", c.C("get", Gv.S("bore40"), Gv.F(50.0)))))));
    }
    if (((_mains_cache.C("has", Gv.S("sig"))).T() && Gv.Eq(_mains_cache.G("sig"), sig))) {
      return _mains_cache.G("q").C("duplicate");
    }
    Gv mu = (skViscosity(Tw, Gv.F(0.0))).AsF();
    Gv open_ = Gd.Arr();
    foreach (Gv i in (outs.C("size")).Iter()) {
      res = res.SI(i, Gv.F(0.0));
      if (Gv.Gt(outs.I(i).G("f"), Gv.F(0.0))) {
        open_.C("append", i);
      }
    }
    if (Gv.Gt(open_.C("size"), Gv.I(0L))) {
      Gv qof = Gd.Fn((_a) => { Gv i_2 = Gd.A(_a, 0); Gv Hn = Gd.A(_a, 1); Gv need = ((Hn - hst.I(i_2))).AsF(); if (Gv.Le(need, Gv.F(0.0))) {   return (Gv.F(0.0)).AsF(); } Gv lo = (Gv.F(0.0)).AsF(); Gv hi = ((Qmax * Gv.F(1.5))).AsF(); if (Gv.Lt(path_head(outs.I(i_2).G("path"), hi, Gv.F(1.0), mu), need)) {   return (hi).AsF(); } foreach (Gv k in (Gv.I(22L)).Iter()) {   Gv mid = ((((lo + hi)) / Gv.F(2.0))).AsF();   if (Gv.Gt(path_head(outs.I(i_2).G("path"), mid, Gv.F(1.0), mu), need)) {     hi = (mid).AsF();   } else {     lo = (mid).AsF();   } } return ((((lo + hi)) / Gv.F(2.0))).AsF(); return Gv.Null; });
      Gv lo2 = (Gv.F(0.0)).AsF();
      Gv hi2 = (Hm).AsF();
      foreach (Gv k2 in (Gv.I(26L)).Iter()) {
        Gv Hn_2 = ((((lo2 + hi2)) / Gv.F(2.0))).AsF();
        Gv Qt = (Gv.F(0.0)).AsF();
        foreach (Gv i_3 in (open_).Iter()) {
          Qt = ((Qt + qof.C("call", i_3, Hn_2))).AsF();
        }
        Gv Hs = ((Hm * ((Gv.F(1.0) - Gd.minf(Gv.F(1.0), (((Qt / Qmax)) * ((Qt / Qmax)))))))).AsF();
        if (Gv.Gt(Hs, Hn_2)) {
          lo2 = (Hn_2).AsF();
        } else {
          hi2 = (Hn_2).AsF();
        }
      }
      Gv Hf = ((((lo2 + hi2)) / Gv.F(2.0))).AsF();
      foreach (Gv i_4 in (open_).Iter()) {
        res = res.SI(i_4, qof.C("call", i_4, Hf));
      }
    }
    _mains_cache = Gd.Dict(Gv.S("sig"), sig, Gv.S("q"), res.C("duplicate"));
    return res;
    return Gv.Null;
  }
  public Gv path_head(Gv path = default(Gv), Gv Q = default(Gv), Gv rho = default(Gv), Gv mu = default(Gv)) {
    Q = (Q).AsF();
    rho = (rho).AsF();
    mu = (mu).AsF();
    if (Gv.Le(Q, Gv.F(0.0))) {
      return (Gv.F(0.0)).AsF();
    }
    Gv H = Gv.F(0.0);
    Gv g = Gv.F(9.81);
    Gv sc = (S.G("cfg").C("get", Gv.S("pipeLenScale"), Gv.F(0.4))).AsF();
    foreach (Gv st in (path).Iter()) {
      if ((st.C("has", Gv.S("plate"))).T()) {
        H = (H + (Gv.F(4.0) * Gd.pow((Q / Gv.F(200.0)), Gv.I(2L))));
        continue;
      }
      if (!(st.C("has", Gv.S("e"))).T()) {
        continue;
      }
      Gv e = st.G("e");
      Gv D = ((bore_mm(Gd.Dyn("float", e.G("dn"))) / Gv.F(1000.0))).AsF();
      Gv A = ((((Gd.K("PI") * D) * D) / Gv.F(4.0))).AsF();
      Gv v = (((Q / Gv.F(60000.0)) / A)).AsF();
      Gv Re = (((((rho * Gv.F(1000.0)) * v) * D) / Gd.maxf(Gv.F(1e-05), mu))).AsF();
      Gv fr = ((Gv.Lt(Re, Gv.F(2300.0)) ? (Gv.F(64.0) / Gd.maxf(Gv.F(1.0), Re)) : (Gv.F(0.25) / Gd.pow((Gd.log(((Gv.F(1.5e-05) / ((Gv.F(3.7) * D))) + (Gv.F(5.74) / Gd.pow(Re, Gv.F(0.9))))) / Gd.log(Gv.F(10.0))), Gv.I(2L))))).AsF();
      Gv Lm = (((e.G("len") / PX_PER_M) * sc)).AsF();
      Gv K = Gv.F(0.4);
      if ((st.C("has", Gv.S("to"))).T()) {
        Gv n = net.G("nodes").C("get", st.G("to"));
        if ((Gv.Ne(n, Gv.Null) && Gv.Ne(n.G("valve"), Gv.Null))) {
          Gv vv = net.G("byId").C("get", n.G("valve"));
          Gv p = (Gv.F(1.0)).AsF();
          if ((Gv.Ne(vv, Gv.Null) && Gv.Eq(vv.C("get", Gv.S("kind"), Gv.S("")), Gv.S("skrt")))) {
            p = (Gd.clampf((Gd.Dyn("float", vv.C("get", Gv.S("pct"), Gv.I(50L))) / Gv.F(100.0)), Gv.F(0.05), Gv.F(1.0))).AsF();
          } else if ((Gv.Ne(vv, Gv.Null) && (vv.C("has", Gv.S("part"))).T())) {
            p = (Gd.clampf((Gd.Dyn("float", vv.G("part")) / Gv.F(100.0)), Gv.F(0.05), Gv.F(1.0))).AsF();
          }
          K = (K + (Gv.F(0.6) * Gd.pow((Gv.F(1.0) / p), Gv.I(4L))));
        }
      }
      H = (H + (((((((fr * Lm) / D) + K)) * v) * v) / ((Gv.F(2.0) * g))));
    }
    return (H).AsF();
    return Gv.Null;
  }
  public Gv bore_mm(Gv dn = default(Gv)) {
    dn = (dn).AsF();
    Gv c = S.G("cfg");
    if (Gv.Le(dn, Gv.F(25.0))) {
      return (c.C("get", Gv.S("bore25"), Gv.F(50.0))).AsF();
    }
    if (Gv.Le(dn, Gv.F(40.0))) {
      return (c.C("get", Gv.S("bore40"), Gv.F(50.0))).AsF();
    }
    if (Gv.Le(dn, Gv.F(50.0))) {
      return (c.C("get", Gv.S("bore50"), Gv.F(75.0))).AsF();
    }
    return (DN_REAL.C("get", dn, dn)).AsF();
    return Gv.Null;
  }
  public Gv pump_point(Gv key = default(Gv), Gv Qset = default(Gv), Gv Hst = default(Gv), Gv sp_ = default(Gv), Gv dpaths = default(Gv), Gv rho = default(Gv), Gv mu = default(Gv), Gv th = default(Gv)) {
    Qset = (Qset).AsF();
    Hst = (Hst).AsF();
    rho = (rho).AsF();
    mu = (mu).AsF();
    th = (th).AsF();
    Gv cfg_ = S.G("cfg");
    Gv kk = (Gv.In(key, Gd.Arr(Gv.S("p1"), Gv.S("p2"), Gv.S("p3"))) ? key : Gv.S("p1"));
    Gv P = Gd.Dict(Gv.S("H0"), Gd.Dyn("float", cfg_.C("get", (kk + Gv.S("_H0")), PUMPS.I(kk).G("H0"))), Gv.S("Qn"), Gd.Dyn("float", cfg_.C("get", (kk + Gv.S("_Qn")), PUMPS.I(kk).G("Qn"))));
    Gv H0 = ((P.G("H0") / ((Gv.F(1.0) + (Gv.F(0.3) * th))))).AsF();
    Gv A = (((Gv.F(0.25) * H0) / ((P.G("Qn") * P.G("Qn"))))).AsF();
    Gv nd = (Gd.maxi(Gv.I(1L), dpaths.C("size"))).AsI();
    Gv sys = Gd.Fn((_a) => { Gv Q = Gd.A(_a, 0); Gv hd = Gv.F(0.0); foreach (Gv dp_ in (dpaths).Iter()) {   hd = Gd.maxf(hd, path_head(dp_, (Q / nd), rho, mu)); } return (((Hst + path_head(sp_, Q, rho, mu)) + hd)).AsF(); return Gv.Null; });
    Gv Q_2 = (Qset).AsF();
    Gv why = Gv.S("");
    Gv Hs_ = (sys.C("call", Qset)).AsF();
    if (Gv.Gt(Hs_, (H0 - ((A * Qset) * Qset)))) {
      if (Gv.Gt(sys.C("call", Gv.F(1.0)), H0)) {
        Q_2 = (Gv.F(0.0)).AsF();
        why = Gv.S("lift");
      } else {
        Gv lo = Gv.F(0.0);
        Gv hi = (Qset).AsF();
        foreach (Gv i in (Gv.I(30L)).Iter()) {
          Gv mid = ((((lo + hi)) / Gv.F(2.0))).AsF();
          if (Gv.Gt(sys.C("call", mid), (H0 - ((A * mid) * mid)))) {
            hi = (mid).AsF();
          } else {
            lo = mid;
          }
        }
        Q_2 = (lo).AsF();
        why = Gv.S("max");
      }
      Hs_ = (sys.C("call", Q_2)).AsF();
    }
    Gv hz = ((Gv.Gt(Q_2, Gv.I(0L)) ? (Gv.F(50.0) * Gd.sqrt(Gd.clampf((((Hs_ + ((A * Q_2) * Q_2))) / H0), Gv.F(0.0), Gv.F(1.0)))) : Gv.F(0.0))).AsF();
    return Gd.Dict(Gv.S("Q"), Q_2, Gv.S("hz"), hz, Gv.S("H"), Hs_, Gv.S("bar"), (((rho * Gv.F(9.81)) * Gd.maxf(Gv.F(0.0), Hs_)) / Gv.F(100.0)), Gv.S("why"), why, Gv.S("hs_loss"), path_head(sp_, Q_2, rho, mu));
    return Gv.Null;
  }
  public Gv _src_temp(Gv b = default(Gv)) {
    {
      Gv _t1 = b;
      if (Gv.Eq(_t1, Gv.S("sk"))) {
        return ((Gv.Lt(vol(S.G("sk").G("mash")), Gv.I(5L)) ? S.G("sk").G("T") : S.G("sk").G("mash").G("T"))).AsF();
      }
      else if (Gv.Eq(_t1, Gv.S("hlt"))) {
        return (S.G("hlt").G("T")).AsF();
      }
      else if (Gv.Eq(_t1, Gv.S("spilka"))) {
        return ((S.G("sp").G("H") / Gd.maxf(Gv.F(1.0), S.G("sp").G("L")))).AsF();
      }
      else if (true) {
        return (S.I(b).G("T")).AsF();
      }
    }
    return Gv.Null;
  }
  public Gv _src_cap(Gv b = default(Gv)) {
    Gv c = S.G("cfg");
    {
      Gv _t1 = b;
      if (Gv.Eq(_t1, Gv.S("sk"))) {
        return (c.G("skCap")).AsF();
      }
      else if (Gv.Eq(_t1, Gv.S("hlt"))) {
        return (c.G("hltCap")).AsF();
      }
      else if (Gv.Eq(_t1, Gv.S("spilka"))) {
        return (c.G("spCap")).AsF();
      }
      else if (Gv.Eq(_t1, Gv.S("rvp"))) {
        return (c.G("rvpCap")).AsF();
      }
      else if (Gv.Eq(_t1, Gv.S("wp"))) {
        return (c.G("wpCap")).AsF();
      }
      else if (true) {
        return (c.G("mpCap")).AsF();
      }
    }
    return Gv.Null;
  }
  public Gv _src_level(Gv b = default(Gv)) {
    {
      Gv _t1 = b;
      if (Gv.Eq(_t1, Gv.S("sk"))) {
        return (skVolTotal()).AsF();
      }
      else if (Gv.Eq(_t1, Gv.S("hlt"))) {
        return (S.G("hlt").G("w")).AsF();
      }
      else if (Gv.Eq(_t1, Gv.S("spilka"))) {
        return (S.G("sp").G("L")).AsF();
      }
      else if (true) {
        return (vol(S.I(b))).AsF();
      }
    }
    return Gv.Null;
  }
  public Gv _stream(Gv d = default(Gv), Gv p = default(Gv)) {
    Gv L = vol(p);
    if (Gv.Le(L, Gv.F(1e-06))) {
      return Gv.Null;
    }
    Gv k = d.G("k");
    if ((S.G("streams").C("has", k)).T()) {
      S = S.S("streams", S.G("streams").SI(k, S.G("streams").I(k).S("L", (S.G("streams").I(k).G("L") + L))));
    } else {
      S = S.S("streams", S.G("streams").SI(k, Gd.Dict(Gv.S("L"), L, Gv.S("kind"), contentKind(p), Gv.S("T"), p.G("T"))));
    }
    return Gv.Null;
  }
  public Gv setup(Gv items = default(Gv), Gv joints = default(Gv)) {
    new_state();
    training = S_training.New(Self);
    lauter_auto = S_lauter_auto.New(Self);
    proc = S_proc.New(Self);
    proc.C("init_state");
    build_net(items, joints);
    Gv R = RECIPES.I(Gd.clampi(recipe_idx, Gv.I(0L), (RECIPES.C("size") - Gv.I(1L))));
    logm((Gv.S("Kontrola receptu: ") + recipe_check(R)));
    if ((calib_applied.C("size")).T()) {
      logm((Gv.S("Kalibrace z kalibrace.json: %s") % Gv.S(", ").C("join", Gd.Ctor("PackedStringArray", calib_applied))));
    }
    if ((calib_rejected.C("size")).T()) {
      logm((Gv.S("Kalibrace – odmítnuto: %s") % Gv.S("; ").C("join", Gd.Ctor("PackedStringArray", calib_rejected))), Gv.S("warn"));
    }
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "Physics": return S_sim.Physics;
    case "Training": return S_sim.Training;
    case "LauterAuto": return S_sim.LauterAuto;
    case "Proc": return S_sim.Proc;
    case "LIQ": return S_sim.LIQ;
    case "SOL": return S_sim.SOL;
    case "CELLK": return S_sim.CELLK;
    case "CFG_DEFAULT": return S_sim.CFG_DEFAULT;
    case "HOPS": return S_sim.HOPS;
    case "MALTS": return S_sim.MALTS;
    case "RECIPE": return S_sim.RECIPE;
    case "RECIPES": return S_sim.RECIPES;
    case "TSAT": return S_sim.TSAT;
    case "PIPE_L_PER_M": return S_sim.PIPE_L_PER_M;
    case "PX_PER_M": return S_sim.PX_PER_M;
    case "S": return this.S;
    case "net": return this.net;
    case "main": return this.main;
    case "training": return this.training;
    case "lauter_auto": return this.lauter_auto;
    case "deposit_on": return this.deposit_on;
    case "proc": return this.proc;
    case "recipe_idx": return this.recipe_idx;
    case "last_route": return this.last_route;
    case "last_goal": return this.last_goal;
    case "valve_ver": return this.valve_ver;
    case "_cache": return this._cache;
    case "_cache_ver": return this._cache_ver;
    case "CALIB_PATH": return S_sim.CALIB_PATH;
    case "calib_applied": return this.calib_applied;
    case "CALIB_RANGE": return S_sim.CALIB_RANGE;
    case "calib_rejected": return this.calib_rejected;
    case "HA_REF": return S_sim.HA_REF;
    case "prof_on": return this.prof_on;
    case "prof": return this.prof;
    case "_dead_acc": return this._dead_acc;
    case "PUMPS": return S_sim.PUMPS;
    case "DN_REAL": return S_sim.DN_REAL;
    case "_mains_cache": return this._mains_cache;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "S": this.S = v; return v;
    case "net": this.net = v; return v;
    case "main": this.main = v; return v;
    case "training": this.training = v; return v;
    case "lauter_auto": this.lauter_auto = v; return v;
    case "deposit_on": this.deposit_on = v; return v;
    case "proc": this.proc = v; return v;
    case "recipe_idx": this.recipe_idx = v; return v;
    case "last_route": this.last_route = v; return v;
    case "last_goal": this.last_goal = v; return v;
    case "valve_ver": this.valve_ver = v; return v;
    case "_cache": this._cache = v; return v;
    case "_cache_ver": this._cache_ver = v; return v;
    case "calib_applied": this.calib_applied = v; return v;
    case "calib_rejected": this.calib_rejected = v; return v;
    case "prof_on": this.prof_on = v; return v;
    case "prof": this.prof = v; return v;
    case "_dead_acc": this._dead_acc = v; return v;
    case "_mains_cache": this._mains_cache = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "vessel": return vessel();
    case "cell": return cell();
    case "apply_calibration": return apply_calibration(Gd.A(a, 0));
    case "new_state": return new_state();
    case "VN": return VN(Gd.A(a, 0));
    case "logm": return logm(Gd.A(a, 0), Gd.A(a, 1));
    case "once": return once(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "tb": return tb(Gd.A(a, 0));
    case "ext": return ext(Gd.A(a, 0));
    case "sol": return sol(Gd.A(a, 0));
    case "liqVol": return liqVol(Gd.A(a, 0));
    case "hotV": return hotV(Gd.A(a, 0));
    case "vol": return vol(Gd.A(a, 0));
    case "cC": return cC(Gd.A(a, 0));
    case "plato": return plato(Gd.A(a, 0));
    case "cellVol": return cellVol(Gd.A(a, 0));
    case "cellP": return cellP(Gd.A(a, 0));
    case "stirQ": return stirQ(Gd.A(a, 0));
    case "tsat": return tsat(Gd.A(a, 0));
    case "lp": return lp();
    case "takePart": return takePart(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "steelC": return steelC(Gd.A(a, 0));
    case "grain_dry_insoluble": return grain_dry_insoluble(Gd.A(a, 0));
    case "addTo": return addTo(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "takeCell": return takeCell(Gd.A(a, 0), Gd.A(a, 1));
    case "mergeCell": return mergeCell(Gd.A(a, 0), Gd.A(a, 1));
    case "contentKind": return contentKind(Gd.A(a, 0));
    case "iodine": return iodine(Gd.A(a, 0), Gd.A(a, 1));
    case "iodine_sample": return iodine_sample(Gd.A(a, 0), Gd.A(a, 1));
    case "iodine_info": return iodine_info(Gd.A(a, 0), Gd.A(a, 1));
    case "skMalt": return skMalt();
    case "skBedH": return skBedH();
    case "skRet": return skRet();
    case "skC": return skC();
    case "skHomog": return skHomog();
    case "skAddMash": return skAddMash(Gd.A(a, 0));
    case "skVolTotal": return skVolTotal();
    case "rkRpm": return rkRpm();
    case "skViscosity": return skViscosity(Gd.A(a, 0), Gd.A(a, 1));
    case "skPerm": return skPerm();
    case "skRes": return skRes(Gd.A(a, 0));
    case "skQmax": return skQmax();
    case "skThermalStep": return skThermalStep(Gd.A(a, 0));
    case "skMashToBed": return skMashToBed();
    case "skDraw": return skDraw(Gd.A(a, 0), Gd.A(a, 1));
    case "sanSpray": return sanSpray(Gd.A(a, 0));
    case "skWallHeat": return skWallHeat(Gd.A(a, 0));
    case "skAddWater": return skAddWater(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "first_order": return first_order(Gd.A(a, 0), Gd.A(a, 1));
    case "react": return react(Gd.A(a, 0), Gd.A(a, 1));
    case "thick_factor": return thick_factor(Gd.A(a, 0));
    case "film_hA": return film_hA(Gd.A(a, 0));
    case "film_factor": return film_factor(Gd.A(a, 0));
    case "density": return density(Gd.A(a, 0));
    case "heat": return heat(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "drawHot": return drawHot(Gd.A(a, 0));
    case "drawWater": return drawWater(Gd.A(a, 0), Gd.A(a, 1));
    case "srcTake": return srcTake(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "spAdd": return spAdd(Gd.A(a, 0));
    case "dstGive": return dstGive(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "cab_live": return cab_live();
    case "cab_gate": return cab_gate();
    case "sim_step": return sim_step(Gd.A(a, 0));
    case "_pd": return _pd(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "hopTarget": return hopTarget();
    case "hopAdd": return hopAdd(Gd.A(a, 0), Gd.A(a, 1));
    case "hopPlanNext": return hopPlanNext();
    case "hopPlanAdd": return hopPlanAdd();
    case "kypr": return kypr();
    case "emptySK": return emptySK();
    case "lvChillSet": return lvChillSet(Gd.A(a, 0));
    case "push": return push();
    case "gauge_vessel_free": return gauge_vessel_free(Gd.A(a, 0));
    case "skHead": return skHead();
    case "tannin_risk": return tannin_risk();
    case "lauter_efficiency": return lauter_efficiency();
    case "start_lautering": return start_lautering();
    case "turbidity": return turbidity(Gd.A(a, 0));
    case "sight_sample": return sight_sample(Gd.A(a, 0));
    case "extract_inventory": return extract_inventory();
    case "spReport": return spReport();
    case "yields": return yields();
    case "recipe_check": return recipe_check(Gd.A(a, 0), Gd.A(a, 1));
    case "fmt0": return fmt0(Gd.A(a, 0));
    case "fmt1": return fmt1(Gd.A(a, 0));
    case "K": return K(Gd.A(a, 0));
    case "on_seg_t": return on_seg_t(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "v_local": return v_local(Gd.A(a, 0), Gd.A(a, 1));
    case "fit_ports": return fit_ports(Gd.A(a, 0));
    case "_node": return _node(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "build_net": return build_net(Gd.A(a, 0), Gd.A(a, 1));
    case "valve_open": return valve_open(Gd.A(a, 0));
    case "_cached": return _cached(Gd.A(a, 0));
    case "bfs": return bfs(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "drain_valves": return drain_valves(Gd.A(a, 0));
    case "_flush_step": return _flush_step(Gd.A(a, 0));
    case "route_valves": return route_valves(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4));
    case "_route": return _route(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "path_to": return path_to(Gd.A(a, 0), Gd.A(a, 1));
    case "path_len": return path_len(Gd.A(a, 0));
    case "path_pct": return path_pct(Gd.A(a, 0));
    case "throttle_flow": return throttle_flow(Gd.A(a, 0));
    case "starts_of": return starts_of(Gd.A(a, 0), Gd.A(a, 1));
    case "items_by_bind": return items_by_bind(Gd.A(a, 0));
    case "_similar": return _similar(Gd.A(a, 0), Gd.A(a, 1));
    case "_agg_sub": return _agg_sub(Gd.A(a, 0), Gd.A(a, 1));
    case "edge_push": return edge_push(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "push_path": return push_path(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "_same": return _same(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "edges_only": return edges_only(Gd.A(a, 0));
    case "suction_path": return suction_path(Gd.A(a, 0));
    case "plate_cool": return plate_cool(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "leak_where": return leak_where(Gd.A(a, 0));
    case "spill": return spill(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5));
    case "net_step": return net_step(Gd.A(a, 0));
    case "skHatchStep": return skHatchStep(Gd.A(a, 0));
    case "_vessel_gravity": return _vessel_gravity(Gd.A(a, 0));
    case "_dead_end_plan": return _dead_end_plan(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "_dead_due": return _dead_due(Gd.A(a, 0), Gd.A(a, 1));
    case "_dead_end_apply": return _dead_end_apply(Gd.A(a, 0), Gd.A(a, 1));
    case "pump_interlock": return pump_interlock(Gd.A(a, 0));
    case "pump_warn": return pump_warn(Gd.A(a, 0));
    case "_pump_prime_idle": return _pump_prime_idle(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "_src_fluid": return _src_fluid(Gd.A(a, 0));
    case "mains_solve": return mains_solve(Gd.A(a, 0), Gd.A(a, 1));
    case "path_head": return path_head(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "bore_mm": return bore_mm(Gd.A(a, 0));
    case "pump_point": return pump_point(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6), Gd.A(a, 7));
    case "_src_temp": return _src_temp(Gd.A(a, 0));
    case "_src_cap": return _src_cap(Gd.A(a, 0));
    case "_src_level": return _src_level(Gd.A(a, 0));
    case "_stream": return _stream(Gd.A(a, 0), Gd.A(a, 1));
    case "setup": return setup(Gd.A(a, 0), Gd.A(a, 1));
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    case "Physics": return Physics;
    case "Training": return Training;
    case "LauterAuto": return LauterAuto;
    case "Proc": return Proc;
    case "LIQ": return LIQ;
    case "SOL": return SOL;
    case "CELLK": return CELLK;
    case "CFG_DEFAULT": return CFG_DEFAULT;
    case "HOPS": return HOPS;
    case "MALTS": return MALTS;
    case "RECIPE": return RECIPE;
    case "RECIPES": return RECIPES;
    case "TSAT": return TSAT;
    case "PIPE_L_PER_M": return PIPE_L_PER_M;
    case "PX_PER_M": return PX_PER_M;
    case "CALIB_PATH": return CALIB_PATH;
    case "CALIB_RANGE": return CALIB_RANGE;
    case "HA_REF": return HA_REF;
    case "PUMPS": return PUMPS;
    case "DN_REAL": return DN_REAL;
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_sim(); return Gv.O(o); }
  public override string ScriptName { get { return "sim"; } }
}
}
