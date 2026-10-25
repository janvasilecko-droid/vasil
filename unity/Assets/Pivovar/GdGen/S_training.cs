// Vygenerováno z training.gd překladačem gd2cs.py – needituj ručně.
using System; using System.Collections.Generic;
namespace Pivovar.Gd.Gen {
public partial class S_training : GdObject {
  public Gv simulation_ref;
  public static Gv STEPS;
  public static Gv WHY;
  static S_training() {
    STEPS = Gd.Arr(Gd.Arr(Gv.S("inspection"), Gv.S("Kontrola varny"), Gv.S("Prohlédni čistotu nádob, síta, hadice a klapky. Sanitaci nádob a chladiče proveď v sekci CIP (louh ≥ 75 °C, kyselina ≤ 60 °C), nebo potvrď vlastní kontrolu."), Gv.I(0L)), Gd.Arr(Gv.S("steam"), Gv.S("Připravit páru a horkou vodu"), Gv.S("Zapni rozvaděč a kotel. Bojler musí obsahovat nejméně 25 hl vody při alespoň 78 °C. Ledovku můžeš připravovat souběžně."), Gv.I(0L)), Gd.Arr(Gv.S("fill"), Gv.S("Napustit vystírací vodu"), Gv.S("Bojler → pánev přes C4. Zapni směšovač na 38 °C, napusť dávku z receptu a napouštění zastav. Samotný bojler dává horkou vodu, nikoli požadovanou směs."), Gv.I(0L)), Gd.Arr(Gv.S("malt"), Gv.S("Zapnout míchadlo a vystřít slad"), Gv.S("Míchadlo musí běžet. Postupně nasyp celou dávku 700 kg; udrž vystírku přibližně 35–45 °C."), Gv.I(0L)), Gd.Arr(Gv.S("rest52"), Gv.S("Ohřát dílo na 50 °C"), Gv.S("S míchadlem pozvolna ohřej a drž 48–52 °C po dobu 15 minut. Při překročení pásma se tato souvislá prodleva počítá znovu."), Gv.I(15L)), Gd.Arr(Gv.S("prime"), Gv.S("Prohřát SK a podrazit podsítí"), Gv.S("Přiveď horkou vodu pod síta: alespoň 120 L před příchodem mláta. Prohřej stěny SK nad 55 °C. Během přípravy drž hlavní dílo kolem 50 °C. Zavři výpustě; voda nesmí před odrmutováním utéct."), Gv.I(0L)), Gd.Arr(Gv.S("split1"), Gv.S("Oddělit první rmut"), Gv.S("C1 přečerpá polovinu díla do SK (bez filtračního lože – SK je ve stavu „rmut“), rmut k povaření zůstane v pánvi. Mimo používanou cestu klapky zavři. Postup: míchadlo na vysoké otáčky (hustý rmut se jinak usadí a ucpe spodní výpust), otevři klapky cesty (nejdřív před čerpadlem, nech čerpadlo zavodnit), potom za čerpadlem, teprve pak čerpadlo zapni. Na konci čerpadlo vypni, zavři klapku za ním a pak před ním."), Gv.I(0L)), Gd.Arr(Gv.S("rest63a"), Gv.S("První rmut: 63 °C"), Gv.S("V pánvi drž 61–65 °C s míchadlem 20 minut. Zbytek díla v SK nevař: musí v něm zůstat živé enzymy."), Gv.I(20L)), Gd.Arr(Gv.S("rest72a"), Gv.S("První rmut: 72 °C"), Gv.S("Ohřej na 70–74 °C a drž 20 minut s míchadlem."), Gv.I(20L)), Gd.Arr(Gv.S("boil1"), Gv.S("První rmut: povařit"), Gv.S("Přiveď rmut k varu a povař jej 10 minut. Míchadlo musí běžet, jinak se rmut připaluje."), Gv.I(10L)), Gd.Arr(Gv.S("return1"), Gv.S("Spojit první rmut"), Gv.S("Vypni páru. C1: pánev → SK. Přečerpej povařený podíl ke zbytku a C1 zastav; sleduj výslednou teplotu. Nepouštěj zatím scezovací C2. Postup: míchadlo na vysoké otáčky (hustý rmut se jinak usadí a ucpe spodní výpust), otevři klapky cesty (nejdřív před čerpadlem, nech čerpadlo zavodnit), potom za čerpadlem, teprve pak čerpadlo zapni. Na konci čerpadlo vypni, zavři klapku za ním a pak před ním."), Gv.I(0L)), Gd.Arr(Gv.S("split2"), Gv.S("Oddělit druhý rmut"), Gv.S("C1: SK → pánev, cesta pro dílo. Přečerpej dvě třetiny díla do pánve; C1 zastav. C2 je určené pro kapalinu pod síty. Postup: míchadlo na vysoké otáčky (hustý rmut se jinak usadí a ucpe spodní výpust), otevři klapky cesty (nejdřív před čerpadlem, nech čerpadlo zavodnit), potom za čerpadlem, teprve pak čerpadlo zapni. Na konci čerpadlo vypni, zavři klapku za ním a pak před ním."), Gv.I(0L)), Gd.Arr(Gv.S("rest63b"), Gv.S("Druhý rmut: 63 °C"), Gv.S("Drž 61–65 °C 20 minut s míchadlem. Pouhé čekání při jiné teplotě se za prodlevu nepočítá."), Gv.I(20L)), Gd.Arr(Gv.S("rest72b"), Gv.S("Druhý rmut: 72 °C"), Gv.S("S míchadlem drž 70–74 °C 20 minut. Nepřekračuj teplotu předčasně."), Gv.I(20L)), Gd.Arr(Gv.S("boil2"), Gv.S("Druhý rmut: povařit"), Gv.S("Povař druhý podíl 10 minut s míchadlem, potom páru vypni."), Gv.I(10L)), Gd.Arr(Gv.S("return2"), Gv.S("Spojit druhý rmut"), Gv.S("C1: pánev → SK. Po úplném vrácení druhého rmutu C1 zastav. Cílem je dílo přibližně 75–78 °C."), Gv.I(0L)), Gd.Arr(Gv.S("conversion"), Gv.S("Odrmutovací teplota a zcukření"), Gv.S("V SK drž dílo 74–78 °C 10 minut a ověř negativní jodovou zkoušku. Nezcukřený rmut po zničení enzymů nelze zachránit dalším ohřevem."), Gv.I(10L)), Gd.Arr(Gv.S("bed"), Gv.S("Odrmutovat: vytvořit filtrační lože"), Gv.S("Stiskni Odrmutovat a vytvořit lože. Vypni C1, C2 a kopačku; před prvním odběrem nech mláto usadit."), Gv.I(0L)), Gd.Arr(Gv.S("settle"), Gv.S("Nechat lože 20 minut v klidu"), Gv.S("Bez odběru, přítoku a kopačky nech lože vytvořit. Podsítí musí být zaplavené; během klidu nemá protékat voda do kanálu."), Gv.I(20L)), Gd.Arr(Gv.S("vorlauf"), Gv.S("Podrážení a vorlauf do SK"), Gv.S("C2: SK → SK. Začni malým odběrem kolem 20–30 L/min. Zvětši průhledítko: vracej proud, dokud není aspoň minutu čirý. Hladinu drž nad ložem."), Gv.I(1L)), Gd.Arr(Gv.S("collect"), Gv.S("Scezovat první sladinu do pánve"), Gv.S("C2: SK → pánev až po vyčiření. Udrž přibližně 5–15 cm kapaliny nad mlátem. Klesá-li hladina, začni rovnoměrně vyslazovat vodou 76–78 °C; silný odběr lože utáhne."), Gv.I(0L)), Gd.Arr(Gv.S("sparge"), Gv.S("Dokončit vyslazování"), Gv.S("Sleduj průhledítko, objem pánve a extrakt posledních výstřelků. Konči kolem 45 hl před varem nebo při poklesu pod 1,5 °P. Zastav C2 a vyslazovací vodu; další voda pivo pouze ředí."), Gv.I(0L)), Gd.Arr(Gv.S("hop0"), Gv.S("Začít chmelovar a první dávku"), Gv.S("Přiveď sladinu k varu, přidej první dávku podle receptu v 0. minutě. Na začátku hlídej pěnu a ubírej páru, aby mladina nepřekypěla."), Gv.I(0L)), Gd.Arr(Gv.S("hop40"), Gv.S("Druhá dávka chmele: 40. minuta"), Gv.S("Pokračuj v chmelovaru a přidej druhou dávku v 40. minutě skutečného varu, nikoli času od spuštění simulátoru."), Gv.I(0L)), Gd.Arr(Gv.S("hop65"), Gv.S("Třetí dávka chmele: 65. minuta"), Gv.S("Přidej třetí dávku v 65. minutě varu. Zastavený čas var nenahrazuje."), Gv.I(0L)), Gd.Arr(Gv.S("boil_end"), Gv.S("Dokončit chmelovar"), Gv.S("Dodrž dobu z receptu, poté vypni páru. Varem klesá objem a stoupá stupňovitost; odstraňuje se DMS."), Gv.I(0L)), Gd.Arr(Gv.S("whirlpool"), Gv.S("Přečerpat do vířivé kádě"), Gv.S("C1: pánev → vířivka. Po přečerpání C1 zastav. Proud vytvoří vír, který soustředí kaly. Klapky otvírej a zavírej ve stejném pořadí jako u rmutu: před čerpadlem, zavodnit, za čerpadlem, zapnout; na konci čerpadlo vypnout dřív než klapky."), Gv.I(0L)), Gd.Arr(Gv.S("whirl_rest"), Gv.S("Usadit kaly: 20 minut"), Gv.S("Vířivou káď nech v klidu bez přítoku a odběru. Připrav zatím průchozí chladicí okruh."), Gv.I(20L)), Gd.Arr(Gv.S("cooling"), Gv.S("Připravit chlazení a spilku"), Gv.S("Ledovka: nejméně 35 % objemu, voda do 4 °C. Pusť C5 a otevři stranu chladiva deskového chladiče. Zkontroluj čistotu spilky; její sanitace se musí provést mimo tento model."), Gv.I(0L)), Gd.Arr(Gv.S("cast"), Gv.S("Spílat a sledovat teplotu"), Gv.S("C3: vířivka → chladič → spilka. Průtok dolaď na přibližně 8,5 °C. Příliš rychlý proud se nestačí ochladit. Před nasátím kalů a vzduchu C3 zastav."), Gv.I(0L)), Gd.Arr(Gv.S("report"), Gv.S("Vyhodnotit várku"), Gv.S("Porovnej objem, °P, hořkost, DMS, zákal, ztráty a teplotu s receptem. Dokončený postup sám o sobě neznamená kvalitní pivo. Kvašení pak pokračuje ve spilce (sekce Spilka – kvašení, diacetyl)."), Gv.I(0L)));
    WHY = Gd.Dict(Gv.S("inspection"), Gv.S("Zbytky mláta, kalů nebo louhu v nádobách a hadicích by znehodnotily várku (infekce, pachuti). Kontrola před várkou je levnější než vylitá várka."), Gv.S("steam"), Gv.S("Bez páry se nic neohřeje a bez dost horké vody nejde vystírat ani vyslazovat. Bojler musí mít zásobu na celou várku (vystírka + podrážka + vyslazování)."), Gv.S("fill"), Gv.S("Vystírací teplota a poměr voda : slad určují, jak husté je dílo a jak rychle se rozpustí škrob. Příliš horká voda by hned zničila enzymy."), Gv.S("malt"), Gv.S("Slad sypeme do míchané vody, aby se netvořily hrudky – v hrudce zůstane škrob, který se nezcukří (ztráta výtěžnosti, škrobový zákal)."), Gv.S("rest52"), Gv.S("Bílkovinná prodleva: proteázy štěpí bílkoviny na menší (pěna, výživa kvasinek), β-glukanázy snižují viskozitu – dílo se pak lépe scezuje."), Gv.S("prime"), Gv.S("Zaplavené podsítí zabrání tomu, aby rmut dosedl na suchá síta a ucpal je; prohřátá káď rmut neochladí."), Gv.S("split1"), Gv.S("Dekokce: část díla povaříme, tím zmazovatí škrob a vrácením zvýšíme teplotu celku bez přepalování enzymů ve zbytku."), Gv.S("rest63a"), Gv.S("63 °C je optimum β-amylázy – vzniká zkvasitelná maltóza (vyšší prokvašení, sušší pivo)."), Gv.S("rest72a"), Gv.S("72 °C je optimum α-amylázy – štěpí škrob na dextriny, dokončí zcukření (jodová zkouška negativní), dává plnost."), Gv.S("boil1"), Gv.S("Povaření rozruší škrobová zrna a tvrdé části sladu (lepší výtěžnost), vznikají melanoidiny (barva, chuť) – typické pro český ležák."), Gv.S("return1"), Gv.S("Vrácením povařeného podílu zvedneme teplotu celého díla na další prodlevu – teplota vznikne z tepelné bilance obou částí."), Gv.S("split2"), Gv.S("Druhý rmut: opět oddělíme hustou část, ve zbytku zůstávají živé enzymy pro zcukření."), Gv.S("rest63b"), Gv.S("Druhá maltózová prodleva – další zkvasitelný cukr."), Gv.S("rest72b"), Gv.S("Druhá zcukřovací prodleva – zbytkový škrob se rozštěpí."), Gv.S("boil2"), Gv.S("Druhé povaření – opět lepší rozklad škrobu a barva."), Gv.S("return2"), Gv.S("Spojením dosáhneme odrmutovací teploty ~76–78 °C: nižší viskozita sladiny = rychlejší scezování, enzymy se zastaví."), Gv.S("conversion"), Gv.S("Negativní jodová zkouška = žádný škrob. Škrob v mladině dělá zákal a je to ztracený extrakt."), Gv.S("bed"), Gv.S("Mláto (pluchy) tvoří přírodní filtr. Nejdřív se musí usadit do rovnoměrné vrstvy, jinak sladina teče kalná."), Gv.S("settle"), Gv.S("Klidem se jemné částice usadí nahoře (těsto) a pluchy dole – vznikne filtrační lože."), Gv.S("vorlauf"), Gv.S("První sladina nese jemné částice; vracíme ji na lože, dokud neteče čirá. Kalná sladina = horší chuť a stabilita piva."), Gv.S("collect"), Gv.S("Předek (první sladina) je nejsilnější. Stahujeme pomalu – rychlý odběr vytvoří podtlak pod sítem, lože se stlačí a utáhne."), Gv.S("sparge"), Gv.S("Vyslazování vymývá extrakt z mláta horkou vodou 76–78 °C. Při < 1,5 °P nebo vyšší teplotě/pH se už vyluhují třísloviny (svíravá chuť)."), Gv.S("hop0"), Gv.S("Var sterilizuje mladinu, sráží bílkoviny (lom), odpařuje DMS. Hořké α-kyseliny se izomerují jen varem – proto chmel na začátku dává hořkost."), Gv.S("hop40"), Gv.S("Prostřední dávka dává hořkost i část chmelového aroma."), Gv.S("hop65"), Gv.S("Pozdní dávka: kratší var = méně hořkosti, víc aromatických olejů zůstane v mladině."), Gv.S("boil_end"), Gv.S("Dost dlouhý var (receptura) zajistí izomeraci, odpar DMS a vysrážení bílkovin. Krátký var = DMS (vařená zelenina) a slabá hořkost."), Gv.S("whirlpool"), Gv.S("Tangenciální nátok roztočí mladinu, kaly se soustředí do kužele uprostřed (efekt čajových lístků)."), Gv.S("whirl_rest"), Gv.S("Klid nechá kaly usadit – čistá mladina pak jde na chladič bez kalů."), Gv.S("cooling"), Gv.S("Ledová voda musí být připravená předem – chlazení musí stihnout celou mladinu na zákvasnou teplotu."), Gv.S("cast"), Gv.S("Rychlé zchlazení na 8–9 °C chrání mladinu před infekcí a připraví ji pro spodní kvasnice; provzdušnění dodá kyslík pro množení."), Gv.S("report"), Gv.S("Porovnáním s receptem se učíš, co v postupu ovlivnilo výsledek (objem, °P, hořkost, výtěžnost, ztráty)."));
  }
  public S_training() {
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
    if (!(get_sim().G("S").C("has", Gv.S("training"))).T()) {
      get_sim().G("S").S("training", Gd.Dict(Gv.S("index"), Gv.I(0L), Gv.S("elapsed"), Gv.F(0.0), Gv.S("done"), Gd.Arr(), Gv.S("baseline"), Gd.Dict(), Gv.S("pause"), Gv.B(false), Gv.S("notified"), Gv.B(false)));
    }
    return get_sim().G("S").G("training");
    return Gv.Null;
  }
  public Gv enter() {
    Gv data = state();
    data = data.S("elapsed", Gv.F(0.0));
    data = data.S("notified", Gv.B(false));
    data = data.S("baseline", Gd.Dict());
    return Gv.Null;
  }
  public Gv current() {
    Gv data = state();
    if (Gv.Ge(data.G("index"), STEPS.C("size"))) {
      return Gd.Dict(Gv.S("complete"), Gv.B(true), Gv.S("ready"), Gv.B(true), Gv.S("title"), Gv.S("Postup dokončen"), Gv.S("hint"), Gv.S("Zhodnoť výsledek várky podle protokolu."), Gv.S("detail"), Gv.S(""));
    }
    Gv step = STEPS.I(data.G("index"));
    Gv S = get_sim().G("S");
    Gv r = S.G("rvp");
    Gv sk = S.G("sk");
    Gv condition = Gv.B(false);
    Gv detail = Gv.S("");
    Gv skip = Gv.B(false);
    {
      Gv _t1 = step.I(Gv.I(0L));
      if (Gv.Eq(_t1, Gv.S("inspection"))) {
        condition = Gv.B(true);
      }
      else if (Gv.Eq(_t1, Gv.S("steam"))) {
        condition = Gv.B(((((get_sim().C("cab_live")).T() && (S.G("boiler").G("on")).T()) && Gv.Ge(S.G("hlt").G("w"), Gv.I(2500L))) && Gv.Ge(S.G("hlt").G("T"), Gv.I(78L))));
      }
      else if (Gv.Eq(_t1, Gv.S("fill"))) {
        condition = Gv.B((((Gv.Ge(S.G("rvpWater").G("added"), (S.G("cfg").G("water") * Gv.F(0.98))) && !(S.G("rvpWater").G("on")).T()) && Gv.Ge(r.G("T"), Gv.I(33L))) && Gv.Le(r.G("T"), Gv.I(40L))));
        detail = (Gv.S("Voda %.0f / %.0f L, %.1f °C") % Gd.Arr(S.G("rvpWater").G("added"), S.G("cfg").G("water"), r.G("T")));
      }
      else if (Gv.Eq(_t1, Gv.S("malt"))) {
        condition = Gv.B((((Gv.Ge(S.G("malt").G("added"), (S.G("malt").G("target") - Gv.I(1L))) && (r.G("stir")).T()) && Gv.Ge(r.G("T"), Gv.I(33L))) && Gv.Le(r.G("T"), Gv.I(45L))));
      }
      else if (Gv.Eq(_t1, Gv.S("rest52"))) {
        condition = Gv.B((((Gv.Gt(r.G("g"), Gv.I(20L)) && (r.G("stir")).T()) && Gv.Ge(r.G("Tsens"), Gv.I(48L))) && Gv.Le(r.G("Tsens"), Gv.I(52L))));
      }
      else if (Gv.Eq(_t1, Gv.S("prime"))) {
        condition = Gv.B(((Gv.Ge(sk.G("under"), Gv.I(120L)) && Gv.Ge(sk.G("Tw"), Gv.I(55L))) && !(sk.G("landed")).T()));
        detail = (Gv.S("Podsítí %.0f L, stěny %.1f °C (minimum 120 L / 55 °C)") % Gd.Arr(sk.G("under"), sk.G("Tw")));
      }
      else if (Gv.Eq(_t1, Gv.S("split1"))) {
        condition = Gv.B((((((sk.G("landed")).T() && Gv.Gt(get_sim().C("vol", sk.G("mash")), Gv.I(500L))) && !(S.G("p1").G("on")).T()) && Gv.Lt(get_sim().C("vol", r), (Gv.F(0.6) * ((get_sim().C("vol", r) + get_sim().C("vol", sk.G("mash"))))))) && Gv.Gt(get_sim().C("vol", r), Gv.I(800L))));
        detail = (Gv.S("Pánev %.0f L, SK %.0f L (polovina díla zůstává v pánvi)") % Gd.Arr(get_sim().C("vol", r), get_sim().C("vol", sk.G("mash"))));
      }
      else if (Gv.Eq(_t1, Gv.S("split2"))) {
        condition = Gv.B(((((Gv.Gt(get_sim().C("vol", r), Gv.I(500L)) && !(S.G("p1").G("on")).T()) && Gv.Gt(get_sim().C("vol", sk.G("mash")), Gv.I(800L))) && Gv.Ge(get_sim().C("vol", r), (Gv.F(0.6) * ((get_sim().C("vol", r) + get_sim().C("vol", sk.G("mash"))))))) && Gv.Le(get_sim().C("vol", r), (Gv.F(0.75) * ((get_sim().C("vol", r) + get_sim().C("vol", sk.G("mash"))))))));
        detail = (Gv.S("Pánev %.0f L (dvě třetiny díla), SK %.0f L") % Gd.Arr(get_sim().C("vol", r), get_sim().C("vol", sk.G("mash"))));
      }
      else if (Gv.Eq(_t1, Gv.S("rest63a")) || Gv.Eq(_t1, Gv.S("rest63b"))) {
        if ((!(data.G("baseline").C("has", Gv.S("T0"))).T() && Gv.Gt(r.G("g"), Gv.I(20L)))) {
          data = data.S("baseline", data.G("baseline").SI(Gv.S("T0"), r.G("Tsens")));
        }
        if (Gv.Gt(data.G("baseline").C("get", Gv.S("T0"), Gv.F(0.0)), Gv.F(65.0))) {
          skip = Gv.B(true);
          condition = Gv.B(true);
          detail = (Gv.S("Rmut měl při převzetí %.1f °C – nad 63 °C se nechladí, pokračuj ohřevem na 72 °C.") % data.G("baseline").G("T0"));
        } else {
          condition = Gv.B((((Gv.Gt(r.G("g"), Gv.I(20L)) && (r.G("stir")).T()) && Gv.Ge(r.G("Tsens"), Gv.I(61L))) && Gv.Le(r.G("Tsens"), Gv.I(65L))));
        }
      }
      else if (Gv.Eq(_t1, Gv.S("rest72a")) || Gv.Eq(_t1, Gv.S("rest72b"))) {
        condition = Gv.B((((Gv.Gt(r.G("g"), Gv.I(20L)) && (r.G("stir")).T()) && Gv.Ge(r.G("Tsens"), Gv.I(70L))) && Gv.Le(r.G("Tsens"), Gv.I(74L))));
      }
      else if (Gv.Eq(_t1, Gv.S("boil1")) || Gv.Eq(_t1, Gv.S("boil2"))) {
        condition = Gv.B(((Gv.Gt(r.G("g"), Gv.I(20L)) && (r.G("boiling")).T()) && (r.G("stir")).T()));
      }
      else if (Gv.Eq(_t1, Gv.S("return1")) || Gv.Eq(_t1, Gv.S("return2"))) {
        condition = Gv.B((((Gv.Le(get_sim().C("vol", r), (S.G("cfg").G("rvpDead") + Gv.I(5L))) && Gv.Gt(get_sim().C("vol", sk.G("mash")), Gv.I(1000L))) && !(S.G("p1").G("on")).T()) && Gv.Eq(r.G("heat"), Gv.I(0L))));
      }
      else if (Gv.Eq(_t1, Gv.S("conversion"))) {
        condition = Gv.B((((Gv.Gt(get_sim().C("vol", sk.G("mash")), Gv.I(1000L)) && Gv.Ge(sk.G("mash").G("T"), Gv.I(74L))) && Gv.Le(sk.G("mash").G("T"), Gv.I(78L))) && Gv.Eq(get_sim().C("iodine", sk.G("mash")), Gv.S("neg"))));
        detail = (Gv.S("Dílo v SK %.1f °C; jodová zkouška %s") % Gd.Arr(sk.G("mash").G("T"), Gd.Dyn("str", get_sim().C("iodine", sk.G("mash")))));
      }
      else if (Gv.Eq(_t1, Gv.S("bed"))) {
        condition = Gv.B(((((Gv.Eq(sk.G("mode"), Gv.S("lauter")) && Gv.Gt(sk.G("g"), Gv.I(20L))) && !(S.G("p1").G("on")).T()) && !(S.G("p2").G("on")).T()) && !(sk.G("rake")).T()));
      }
      else if (Gv.Eq(_t1, Gv.S("settle"))) {
        condition = Gv.B((((((Gv.Eq(sk.G("mode"), Gv.S("lauter")) && !(sk.G("rake")).T()) && !(sk.G("filling")).T()) && !(S.G("p1").G("on")).T()) && !(S.G("p2").G("on")).T()) && !(S.G("skWater").G("on")).T()));
      }
      else if (Gv.Eq(_t1, Gv.S("vorlauf"))) {
        Gv sample = get_sim().C("sight_sample", get_sim().G("net").G("byId").I(Gv.S("inl_mur9u2hg")));
        Gv return_flow = Gv.B(false);
        foreach (Gv flow in (S.G("flows")).Iter()) {
          if ((Gv.Eq(flow.G("src"), Gv.S("sk")) && (flow.G("dst").C("has", Gv.S("sk"))).T())) {
            return_flow = Gv.B(true);
          }
        }
        condition = Gv.B((((((return_flow).T() && Gv.Ge(sk.G("vorlaufL"), Gv.I(220L))) && Gv.Gt(sample.C("get", Gv.S("flow"), Gv.F(0.0)), Gv.I(0L))) && Gv.Lt(sample.C("get", Gv.S("turb"), Gv.F(999.0)), Gv.I(15L))) && Gv.Eq(sample.C("get", Gv.S("kind"), Gv.S("empty")), Gv.S("wort"))));
        detail = (Gv.S("Vráceno %.0f L; průhledítko %.0f L/min, zákal %.0f") % Gd.Arr(sk.G("vorlaufL"), sample.C("get", Gv.S("flow"), Gv.F(0.0)), sample.C("get", Gv.S("turb"), Gv.F(0.0))));
      }
      else if (Gv.Eq(_t1, Gv.S("collect"))) {
        condition = Gv.B((Gv.Ge(get_sim().C("hotV", r), Gv.I(3000L)) && Gv.Eq(sk.G("mode"), Gv.S("lauter"))));
      }
      else if (Gv.Eq(_t1, Gv.S("sparge"))) {
        condition = Gv.B((((Gv.Ge(get_sim().C("hotV", r), Gv.I(4500L)) || (Gv.Gt(sk.G("filt"), Gv.I(200L)) && Gv.Le(sk.G("outP"), S.G("cfg").G("spargeEndP")))) && !(S.G("p2").G("on")).T()) && !(S.G("skWater").G("on")).T()));
        detail = (Gv.S("Pánev %.0f L; poslední výstřelky %.1f °P") % Gd.Arr(get_sim().C("hotV", r), sk.G("outP")));
      }
      else if (Gv.Eq(_t1, Gv.S("hop0")) || Gv.Eq(_t1, Gv.S("hop40")) || Gv.Eq(_t1, Gv.S("hop65"))) {
        Gv idxs = hop_group(step.I(Gv.I(0L)));
        condition = Gv.B(Gv.Gt(idxs.C("size"), Gv.I(0L)));
        foreach (Gv index in (idxs).Iter()) {
          if (Gv.Le(S.G("hops").C("size"), index)) {
            condition = Gv.B(false);
            continue;
          }
          Gv hop = S.G("hops").I(index);
          Gv wanted = S.G("hopPlan").I(index);
          if (!(((hop.G("boil")).T() && Gv.Le(Gd.absf((hop.C("get", Gv.S("boilMin"), Gv.F(-999.0)) - wanted.G("min"))), Gv.I(2L))) && Gv.Le(Gd.absf((hop.G("kg") - wanted.G("kg"))), Gv.F(0.05)))) {
            condition = Gv.B(false);
          }
        }
        if ((idxs.C("is_empty")).T()) {
          skip = Gv.B(true);
          condition = Gv.B(true);
        }
        detail = (Gv.S("Skutečný var %.1f min; dávky %d / %d") % Gd.Arr(r.G("boilMin"), S.G("hops").C("size"), S.G("hopPlan").C("size")));
      }
      else if (Gv.Eq(_t1, Gv.S("boil_end"))) {
        condition = Gv.B((Gv.Ge(r.G("boilMin"), S.G("cfg").G("boilMin")) && Gv.Eq(r.G("heat"), Gv.I(0L))));
      }
      else if (Gv.Eq(_t1, Gv.S("whirlpool"))) {
        condition = Gv.B(((Gv.Le(get_sim().C("vol", r), (S.G("cfg").G("rvpDead") + Gv.I(5L))) && Gv.Gt(get_sim().C("vol", S.G("wp")), Gv.I(2500L))) && !(S.G("p1").G("on")).T()));
      }
      else if (Gv.Eq(_t1, Gv.S("whirl_rest"))) {
        condition = Gv.B((((Gv.Gt(S.G("wp").G("w"), Gv.I(1000L)) && !(S.G("wp").G("whirlNow")).T()) && !(S.G("p1").G("on")).T()) && !(S.G("p3").G("on")).T()));
      }
      else if (Gv.Eq(_t1, Gv.S("cooling"))) {
        condition = Gv.B((((Gv.Ge(S.G("lv").G("w"), (S.G("cfg").G("lvVol") * Gv.F(0.35))) && Gv.Le(S.G("lv").G("T"), Gv.I(4L))) && (S.G("lv").G("pump")).T()) && !(S.G("plates").C("is_empty")).T()));
      }
      else if (Gv.Eq(_t1, Gv.S("cast"))) {
        condition = Gv.B(((Gv.Gt(S.G("sp").G("L"), Gv.I(2000L)) && !(S.G("p3").G("on")).T()) && Gv.Le(get_sim().C("vol", S.G("wp")), ((S.G("cfg").G("wpDead") + (S.G("wp").G("trubSet") * Gv.I(6L))) + Gv.I(10L)))));
      }
      else if (Gv.Eq(_t1, Gv.S("report"))) {
        condition = Gv.B(Gv.Ne(get_sim().C("spReport"), Gv.Null));
      }
    }
    Gv ready = Gv.B(((condition).T() && (((skip).T() || Gv.Eq(step.I(Gv.I(3L)), Gv.I(0L))) || Gv.Ge(data.G("elapsed"), (Gd.Dyn("float", step.I(Gv.I(3L))) * Gv.F(60.0))))));
    if (Gv.Gt(step.I(Gv.I(3L)), Gv.I(0L))) {
      detail = (detail + (Gv.S("\nSouvislá prodleva %.1f / %d min") % Gd.Arr((data.G("elapsed") / Gv.F(60.0)), step.I(Gv.I(3L)))));
    }
    Gv title = step.I(Gv.I(1L));
    Gv hint = step.I(Gv.I(2L)).C("replace", Gv.S("700 kg"), (Gv.S("%d kg") % Gd.roundi(S.G("malt").G("target"))));
    if (Gv.In(step.I(Gv.I(0L)), Gd.Arr(Gv.S("hop0"), Gv.S("hop40"), Gv.S("hop65")))) {
      Gv g = hop_group(step.I(Gv.I(0L)));
      Gv names = Gd.Arr();
      foreach (Gv index_2 in (g).Iter()) {
        Gv hp = S.G("hopPlan").I(index_2);
        Gv nm = hp.G("hop");
        foreach (Gv h in (get_sim().G("HOPS")).Iter()) {
          if (Gv.Eq(h.G("id"), hp.G("hop"))) {
            nm = h.G("name");
          }
        }
        names.C("append", (Gv.S("%s kg %s v %d. min") % Gd.Arr(get_sim().C("fmt1", hp.G("kg")), nm, Gd.roundi(hp.G("min")))));
      }
      title = (Gv.S("Chmel: ") + (((names.C("size")).T() ? Gv.S(", ").C("join", Gd.Ctor("PackedStringArray", names)) : Gv.S("(recept nemá tuto dávku)"))));
    }
    return Gd.Dict(Gv.S("complete"), Gv.B(false), Gv.S("ready"), ready, Gv.S("condition"), condition, Gv.S("title"), title, Gv.S("hint"), hint, Gv.S("detail"), detail, Gv.S("id"), step.I(Gv.I(0L)));
    return Gv.Null;
  }
  public Gv hop_group(Gv id = default(Gv)) {
    Gv P = get_sim().G("S").G("hopPlan");
    Gv n = (P.C("size")).AsI();
    if (Gv.Eq(n, Gv.I(0L))) {
      return Gd.Arr();
    }
    Gv first = Gd.Arr();
    Gv last = Gd.Arr();
    Gv mid = Gd.Arr();
    foreach (Gv i in (n).Iter()) {
      if (Gv.Eq(P.I(i).G("min"), P.I(Gv.I(0L)).G("min"))) {
        first.C("append", i);
      }
    }
    if (Gv.Gt((n - Gv.I(1L)), first.I((first.C("size") - Gv.I(1L))))) {
      last = Gd.Arr((n - Gv.I(1L)));
    }
    foreach (Gv i_2 in (n).Iter()) {
      if ((!Gv.In(i_2, first) && !Gv.In(i_2, last))) {
        mid.C("append", i_2);
      }
    }
    return Gd.Dict(Gv.S("hop0"), first, Gv.S("hop40"), mid, Gv.S("hop65"), last).I(id);
    return Gv.Null;
  }
  public Gv update(Gv seconds = default(Gv)) {
    seconds = (seconds).AsF();
    Gv data = state();
    if (Gv.Ge(data.G("index"), STEPS.C("size"))) {
      return Gv.Null;
    }
    Gv view = current();
    if (Gv.Gt(STEPS.I(data.G("index")).I(Gv.I(3L)), Gv.I(0L))) {
      data = data.S("elapsed", ((view.G("condition")).T() ? (data.G("elapsed") + seconds) : Gv.F(0.0)));
    }
    view = current();
    if (((view.G("ready")).T() && !(data.G("notified")).T())) {
      data = data.S("notified", Gv.B(true));
      get_sim().C("logm", (Gv.S("Krok připraven k potvrzení: ") + view.G("title")));
      if (((data.G("pause")).T() && Gv.Ne(get_sim().G("main"), Gv.Null))) {
        get_sim().G("main").S("running", Gv.B(false));
      }
    }
    return Gv.Null;
  }
  public Gv confirm() {
    Gv data = state();
    Gv view = current();
    if (((view.G("complete")).T() || !(view.G("ready")).T())) {
      return Gv.B(false);
    }
    data.G("done").C("append", Gd.Dict(Gv.S("id"), view.G("id"), Gv.S("t"), get_sim().G("S").G("t")));
    get_sim().C("logm", (Gv.S("Potvrzen krok: ") + view.G("title")));
    data = data.S("index", (data.G("index") + Gv.I(1L)));
    enter();
    return Gv.B(true);
    return Gv.Null;
  }
  public Gv text() {
    Gv data = state();
    Gv view = current();
    Gv result = (Gv.S("[b]%d / %d: %s[/b]\n%s\n[color=#f2c230]%s[/color]") % Gd.Arr(Gd.mini((data.G("index") + Gv.I(1L)), STEPS.C("size")), STEPS.C("size"), view.G("title"), view.G("hint"), view.G("detail")));
    if (((data.C("get", Gv.S("why"), Gv.B(false))).T() && (WHY.C("has", view.C("get", Gv.S("id"), Gv.S("")))).T())) {
      result = (result + (Gv.S("\n[color=#8fc7ff][b]Proč to děláme?[/b] %s[/color]") % WHY.I(view.G("id"))));
    }
    if (!(view.G("complete")).T()) {
      result = (result + ((view.G("ready")).T() ? Gv.S("\n[color=#7ee08a]Splněno – potvrď další krok.[/color]") : Gv.S("\n[color=#eeb772]Čeká na skutečné splnění podmínek.[/color]")));
    }
    return result;
    return Gv.Null;
  }
  public Gv list_text() {
    Gv data = state();
    Gv out_ = Gd.Arr();
    foreach (Gv i in (STEPS.C("size")).Iter()) {
      Gv mark = (Gv.Lt(i, data.G("index")) ? Gv.S("+") : ((Gv.Eq(i, data.G("index")) ? Gv.S(">") : Gv.S("·"))));
      Gv tt = STEPS.I(i).I(Gv.I(1L));
      if (Gv.In(STEPS.I(i).I(Gv.I(0L)), Gd.Arr(Gv.S("hop0"), Gv.S("hop40"), Gv.S("hop65")))) {
        Gv g = hop_group(STEPS.I(i).I(Gv.I(0L)));
        if ((g.C("size")).T()) {
          tt = (Gv.S("Chmel v %s. min") % Gv.S(", ").C("join", Gd.Ctor("PackedStringArray", g.C("map", Gd.Fn((_a) => { Gv x = Gd.A(_a, 0); return Gd.Dyn("str", Gd.roundi(get_sim().G("S").G("hopPlan").I(x).G("min"))); return Gv.Null; })))));
        }
      }
      out_.C("append", (Gv.S("%s %d. %s") % Gd.Arr(mark, (i + Gv.I(1L)), tt)));
    }
    return Gv.S("\n").C("join", Gd.Ctor("PackedStringArray", out_));
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "simulation_ref": return this.simulation_ref;
    case "sim": return get_sim();
    case "STEPS": return S_training.STEPS;
    case "WHY": return S_training.WHY;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "simulation_ref": this.simulation_ref = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "_init": return _init(Gd.A(a, 0));
    case "state": return state();
    case "enter": return enter();
    case "current": return current();
    case "hop_group": return hop_group(Gd.A(a, 0));
    case "update": return update(Gd.A(a, 0));
    case "confirm": return confirm();
    case "text": return text();
    case "list_text": return list_text();
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    case "STEPS": return STEPS;
    case "WHY": return WHY;
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_training(); o._init(Gd.A(a, 0)); return Gv.O(o); }
  public override string ScriptName { get { return "training"; } }
}
}
