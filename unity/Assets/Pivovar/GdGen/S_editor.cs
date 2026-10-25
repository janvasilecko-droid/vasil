// Vygenerováno z editor.gd překladačem gd2cs.py – needituj ručně.
using System; using System.Collections.Generic;
namespace Pivovar.Gd.Gen {
public partial class S_editor : GdNode {
  public static Gv GRID;
  public static Gv PX_PER_M;
  public static Gv PARTS;
  public static Gv MEDIA;
  public static Gv COLORS;
  public static Gv ON_PIPE;
  public static Gv PROTECT;
  public static Gv VBINDS;
  public static Gv PBINDS;
  public static Gv SHAPES;
  public static Gv VKINDS;
  public static Gv IKINDS;
  public Gv m;
  public Gv on;
  public Gv tool;
  public Gv part_key;
  public Gv sel;
  public Gv multi;
  public Gv drag;
  public Gv drag_mode;
  public Gv drag_off;
  public Gv drag_start;
  public Gv drag_orig;
  public Gv box_a;
  public Gv draw_pts;
  public Gv pitch_a;
  public Gv meas_a;
  public Gv route_a;
  public Gv route_hl;
  public Gv paint_col;
  public Gv hover_w;
  public Gv undo_stack;
  public Gv undo_key;
  public Gv undo_t;
  public Gv grave;
  public Gv del_arm;
  public Gv dirty;
  public Gv ui;
  public Gv info;
  public Gv props;
  public Gv live_s;
  public Gv live_i;
  public Gv btns;
  public Gv dn_new;
  public Gv med_new;
  public Gv fdlg;
  public Gv dock_was;
  public static Gv TOOLS;
  public Gv last_z;
  static S_editor() {
    GRID = Gv.F(10.0);
    PX_PER_M = Gv.F(100.0);
    PARTS = Gd.Arr(Gd.Arr(Gv.S("klapka"), Gv.S("⋈ Klapka"), Gv.S("valve"), Gd.Dict(Gv.S("kind"), Gv.S("klapka"), Gv.S("label"), Gv.S("klapka"))), Gd.Arr(Gv.S("kulovy"), Gv.S("◉ Kulový ventil"), Gv.S("valve"), Gd.Dict(Gv.S("kind"), Gv.S("kulovy"), Gv.S("label"), Gv.S("kulový ventil"))), Gd.Arr(Gv.S("skrt"), Gv.S("◐ Škrtidlo"), Gv.S("valve"), Gd.Dict(Gv.S("kind"), Gv.S("skrt"), Gv.S("label"), Gv.S("škrtidlo"))), Gd.Arr(Gv.S("belimo"), Gv.S("▣ Belimo"), Gv.S("valve"), Gd.Dict(Gv.S("kind"), Gv.S("belimo"), Gv.S("label"), Gv.S("Belimo"))), Gd.Arr(Gv.S("pruh"), Gv.S("▭ Průhledítko"), Gv.S("inl"), Gd.Dict(Gv.S("k"), Gv.S("pruh"), Gv.S("label"), Gv.S("průhledítko"))), Gd.Arr(Gv.S("zpk"), Gv.S("▷| Zpětná klapka"), Gv.S("inl"), Gd.Dict(Gv.S("k"), Gv.S("zpk"), Gv.S("label"), Gv.S("zpětná klapka"))), Gd.Arr(Gv.S("ysit"), Gv.S("Y Sítko"), Gv.S("inl"), Gd.Dict(Gv.S("k"), Gv.S("ysit"), Gv.S("label"), Gv.S("Y-sítko"))), Gd.Arr(Gv.S("red"), Gv.S("◁ Redukce"), Gv.S("inl"), Gd.Dict(Gv.S("k"), Gv.S("red"), Gv.S("label"), Gv.S("redukce"))), Gd.Arr(Gv.S("tep"), Gv.S("° Teploměr"), Gv.S("inl"), Gd.Dict(Gv.S("k"), Gv.S("tep"), Gv.S("label"), Gv.S("teploměr"))), Gd.Arr(Gv.S("man"), Gv.S("◔ Manometr"), Gv.S("inl"), Gd.Dict(Gv.S("k"), Gv.S("man"), Gv.S("label"), Gv.S("manometr"))), Gd.Arr(Gv.S("odk"), Gv.S("K Odvaděč kond."), Gv.S("inl"), Gd.Dict(Gv.S("k"), Gv.S("odk"), Gv.S("label"), Gv.S("odvaděč kondenzátu"))), Gd.Arr(Gv.S("flow"), Gv.S("⇕ Rotametr"), Gv.S("flow"), Gd.Dict(Gv.S("label"), Gv.S("rotametr"))), Gd.Arr(Gv.S("pump"), Gv.S("⊙ Čerpadlo"), Gv.S("pump"), Gd.Dict(Gv.S("label"), Gv.S("čerpadlo"), Gv.S("dir"), Gv.I(0L), Gv.S("bind"), Gv.S(""))), Gd.Arr(Gv.S("fitT"), Gv.S("┳ T-kus"), Gv.S("fitting"), Gd.Dict(Gv.S("ports"), Gd.Arr(Gv.I(180L), Gv.I(0L), Gv.I(90L)), Gv.S("arm"), Gv.I(30L), Gv.S("label"), Gv.S("T-kus"))), Gd.Arr(Gv.S("fitL"), Gv.S("┗ Koleno L"), Gv.S("fitting"), Gd.Dict(Gv.S("ports"), Gd.Arr(Gv.I(270L), Gv.I(0L)), Gv.S("arm"), Gv.I(30L), Gv.S("label"), Gv.S("koleno"))), Gd.Arr(Gv.S("fitRL"), Gv.S("┛ Koleno ⅃"), Gv.S("fitting"), Gd.Dict(Gv.S("ports"), Gd.Arr(Gv.I(270L), Gv.I(180L)), Gv.S("arm"), Gv.I(30L), Gv.S("label"), Gv.S("koleno"))), Gd.Arr(Gv.S("tank"), Gv.S("▯ Nádoba"), Gv.S("vessel"), Gd.Dict(Gv.S("label"), Gv.S("nádoba"), Gv.S("shape"), Gv.S("tank"), Gv.S("w"), Gv.I(220L), Gv.S("h"), Gv.I(300L))), Gd.Arr(Gv.S("cone"), Gv.S("▽ Tank kónický"), Gv.S("vessel"), Gd.Dict(Gv.S("label"), Gv.S("tank"), Gv.S("shape"), Gv.S("cone"), Gv.S("w"), Gv.I(220L), Gv.S("h"), Gv.I(440L))), Gd.Arr(Gv.S("horiz"), Gv.S("▭ Ležatý tank"), Gv.S("vessel"), Gd.Dict(Gv.S("label"), Gv.S("ležatý tank"), Gv.S("shape"), Gv.S("horiz"), Gv.S("w"), Gv.I(420L), Gv.S("h"), Gv.I(200L))), Gd.Arr(Gv.S("platform"), Gv.S("▬ Podesta"), Gv.S("vessel"), Gd.Dict(Gv.S("label"), Gv.S("podesta"), Gv.S("shape"), Gv.S("platform"), Gv.S("w"), Gv.I(400L), Gv.S("h"), Gv.I(16L))), Gd.Arr(Gv.S("spray"), Gv.S("✺ Sprcha"), Gv.S("spray"), Gd.Dict(Gv.S("label"), Gv.S("sprcha"))), Gd.Arr(Gv.S("nozzle"), Gv.S("⇣ Tryska"), Gv.S("nozzle"), Gd.Dict(Gv.S("label"), Gv.S("tryska"), Gv.S("dir"), Gv.I(270L))), Gd.Arr(Gv.S("hopf"), Gv.S("▤ Chmelový filtr"), Gv.S("hopf"), Gd.Dict(Gv.S("label"), Gv.S("chmelový filtr"), Gv.S("rot"), Gv.I(0L))), Gd.Arr(Gv.S("text"), Gv.S("T Popisek"), Gv.S("text"), Gd.Dict(Gv.S("label"), Gv.S("popisek"), Gv.S("size"), Gv.I(14L))));
    MEDIA = Gd.Arr(Gd.Arr(Gv.S("mash"), Gv.S("rmut")), Gd.Arr(Gv.S("wort"), Gv.S("sladina")), Gd.Arr(Gv.S("hot"), Gv.S("horká")), Gd.Arr(Gv.S("water"), Gv.S("studená")), Gd.Arr(Gv.S("ice"), Gv.S("ledová")), Gd.Arr(Gv.S("steam"), Gv.S("pára")));
    COLORS = Gd.Arr(Gd.Arr(Gv.S(""), Gv.S("bez")), Gd.Arr(Gv.S("#e0452f"), Gv.S("červená")), Gd.Arr(Gv.S("#f08c1a"), Gv.S("oranžová")), Gd.Arr(Gv.S("#f2c230"), Gv.S("žlutá")), Gd.Arr(Gv.S("#2fbf5b"), Gv.S("zelená")), Gd.Arr(Gv.S("#2e7bd6"), Gv.S("modrá")), Gd.Arr(Gv.S("#9b59d0"), Gv.S("fialová")), Gd.Arr(Gv.S("#e84393"), Gv.S("růžová")));
    ON_PIPE = Gd.Arr(Gv.S("valve"), Gv.S("inl"), Gv.S("flow"), Gv.S("pump"));
    PROTECT = Gd.Arr(Gv.S("vessel"), Gv.S("pump"), Gv.S("panel"), Gv.S("manifold"));
    VBINDS = Gd.Arr(Gd.Arr(Gv.S(""), Gv.S("žádná (jen výkres)")), Gd.Arr(Gv.S("rvp"), Gv.S("pánev")), Gd.Arr(Gv.S("mp"), Gv.S("rmutovací pánev")), Gd.Arr(Gv.S("sk"), Gv.S("scezovací káď")), Gd.Arr(Gv.S("wp"), Gv.S("vířivka")), Gd.Arr(Gv.S("hlt"), Gv.S("bojler")), Gd.Arr(Gv.S("lv"), Gv.S("ledovka")), Gd.Arr(Gv.S("spilka"), Gv.S("spilka")), Gd.Arr(Gv.S("vodovod"), Gv.S("vodovod")), Gd.Arr(Gv.S("glyk"), Gv.S("glykol")));
    PBINDS = Gd.Arr(Gd.Arr(Gv.S(""), Gv.S("žádná (jen výkres)")), Gd.Arr(Gv.S("p1"), Gv.S("C1 rmutovací")), Gd.Arr(Gv.S("p2"), Gv.S("C2 sladina")), Gd.Arr(Gv.S("p3"), Gv.S("C3 mladina")), Gd.Arr(Gv.S("phw"), Gv.S("C4 horká voda")), Gd.Arr(Gv.S("plv"), Gv.S("C5 ledovka")));
    SHAPES = Gd.Arr(Gd.Arr(Gv.S("tank"), Gv.S("válcová")), Gd.Arr(Gv.S("cone"), Gv.S("kónická")), Gd.Arr(Gv.S("tall"), Gv.S("vysoká")), Gd.Arr(Gv.S("horiz"), Gv.S("ležatá")), Gd.Arr(Gv.S("lauter"), Gv.S("scezovací")), Gd.Arr(Gv.S("platform"), Gv.S("podesta")), Gd.Arr(Gv.S("plate"), Gv.S("deskový chladič")), Gd.Arr(Gv.S("chiller"), Gv.S("glykolový chladič")), Gd.Arr(Gv.S("inlet"), Gv.S("řad")));
    VKINDS = Gd.Arr(Gd.Arr(Gv.S("klapka"), Gv.S("klapka")), Gd.Arr(Gv.S("kulovy"), Gv.S("kulový ventil")), Gd.Arr(Gv.S("skrt"), Gv.S("škrtidlo")), Gd.Arr(Gv.S("belimo"), Gv.S("Belimo")));
    IKINDS = Gd.Arr(Gd.Arr(Gv.S("pruh"), Gv.S("průhledítko")), Gd.Arr(Gv.S("zpk"), Gv.S("zpětná klapka")), Gd.Arr(Gv.S("ysit"), Gv.S("Y-sítko")), Gd.Arr(Gv.S("red"), Gv.S("redukce")), Gd.Arr(Gv.S("tep"), Gv.S("teploměr")), Gd.Arr(Gv.S("man"), Gv.S("manometr")), Gd.Arr(Gv.S("odk"), Gv.S("odvaděč kondenzátu")), Gd.Arr(Gv.S("redp"), Gv.S("redukční ventil páry")), Gd.Arr(Gv.S("aer"), Gv.S("provzdušňovač")), Gd.Arr(Gv.S("tc"), Gv.S("čidlo")));
    TOOLS = Gd.Arr(Gv.S("select"), Gv.S("pipe"), Gv.S("hose"), Gv.S("pitch"), Gv.S("joint"), Gv.S("measure"), Gv.S("route"), Gv.S("paint"));
  }
  public S_editor() {
    TypeName = "Node2D";
    m = Gv.Null;
    on = Gv.B(false);
    tool = Gv.S("select");
    part_key = Gv.S("klapka");
    sel = Gv.Null;
    multi = Gd.Arr();
    drag = Gv.B(false);
    drag_mode = Gv.S("");
    drag_off = Gd.StaticGet("Vector2", "ZERO");
    drag_start = Gd.StaticGet("Vector2", "ZERO");
    drag_orig = Gd.Arr();
    box_a = Gd.StaticGet("Vector2", "ZERO");
    draw_pts = Gd.Arr();
    pitch_a = Gv.Null;
    meas_a = Gv.Null;
    route_a = Gv.Null;
    route_hl = Gd.Arr();
    paint_col = Gv.S("#e0452f");
    hover_w = Gd.StaticGet("Vector2", "ZERO");
    undo_stack = Gd.Arr();
    undo_key = Gv.S("");
    undo_t = Gv.I(0L);
    grave = Gd.Dict();
    del_arm = Gv.I(0L);
    dirty = Gv.B(false);
    ui = Gv.Null;
    info = Gv.Null;
    props = Gv.Null;
    live_s = Gv.Null;
    live_i = Gv.Null;
    btns = Gd.Dict();
    dn_new = Gv.I(50L);
    med_new = Gv.S("wort");
    fdlg = Gv.Null;
    dock_was = Gv.B(false);
    last_z = Gv.F(0.0);
  }
  public Gv setup() {
    Self.S("z_index", Gv.I(60L));
    ui = Gd.New("CanvasLayer");
    ui = ui.S("layer", Gv.I(3L));
    ui = ui.S("visible", Gv.B(false));
    m.C("add_child", ui);
    Gv p = Gd.New("PanelContainer");
    Gv sb = Gd.New("StyleBoxFlat");
    sb = sb.S("bg_color", Gd.Ctor("Color", Gv.F(0.05), Gv.F(0.065), Gv.F(0.08), Gv.F(0.95)));
    sb.C("set_corner_radius_all", Gv.I(10L));
    sb = sb.S("border_color", Gd.Ctor("Color", Gv.S("f2c230")));
    sb.C("set_border_width_all", Gv.I(2L));
    sb = sb.S("content_margin_left", Gv.I(10L));
    sb = sb.S("content_margin_right", Gv.I(10L));
    sb = sb.S("content_margin_top", Gv.I(8L));
    sb = sb.S("content_margin_bottom", Gv.I(8L));
    p.C("add_theme_stylebox_override", Gv.S("panel"), sb);
    p = p.S("position", Gd.Ctor("Vector2", Gv.I(12L), Gv.I(62L)));
    p = p.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(610L), Gv.I(0L)));
    ui.C("add_child", p);
    Gv th = Gd.New("Theme");
    th = th.S("default_font_size", Gv.I(13L));
    p = p.S("theme", th);
    Gv v = Gd.New("VBoxContainer");
    v.C("add_theme_constant_override", Gv.S("separation"), Gv.I(5L));
    p.C("add_child", v);
    Gv hb = Gd.New("HBoxContainer");
    v.C("add_child", hb);
    Gv t = Gd.New("Label");
    t = t.S("text", Gv.S("✎ EDITOR VARNY"));
    t.C("add_theme_color_override", Gv.S("font_color"), Gd.Ctor("Color", Gv.S("f2c230")));
    t.C("add_theme_font_override", Gv.S("font"), m.G("font_b"));
    t.C("add_theme_font_size_override", Gv.S("font_size"), Gv.I(15L));
    t = t.S("size_flags_horizontal", Gd.StaticGet("Control", "SIZE_EXPAND_FILL"));
    hb.C("add_child", t);
    Gv more = Gd.New("VBoxContainer");
    more.C("add_theme_constant_override", Gv.S("separation"), Gv.I(5L));
    Gv cb = _b(hb, Gv.S("▴ sbalit"), Gd.Fn((_a) => {  _ = Self.G("pass"); return Gv.Null; }));
    cb.G("pressed").C("connect", Gd.Fn((_a) => {  more = more.S("visible", Gv.B(!(more.G("visible")).T())); info = info.S("visible", more.G("visible")); cb = cb.S("text", ((more.G("visible")).T() ? Gv.S("▴ sbalit") : Gv.S("▾ rozbalit"))); return Gv.Null; }));
    Gv r = _row(v);
    foreach (Gv d in (Gd.Arr(Gd.Arr(Gv.S("select"), Gv.S("⬚ Výběr (V)")), Gd.Arr(Gv.S("pipe"), Gv.S("╱ Trubka (T)")), Gd.Arr(Gv.S("hose"), Gv.S("∿ Nerez. hadice (H)")), Gd.Arr(Gv.S("pitch"), Gv.S("⤳ Spílací hadice")), Gd.Arr(Gv.S("joint"), Gv.S("● Spoj (J)")), Gd.Arr(Gv.S("measure"), Gv.S("⟷ Měřit (M)")), Gd.Arr(Gv.S("route"), Gv.S("➜ Cesta (C)")), Gd.Arr(Gv.S("paint"), Gv.S("▦ Barva (B)")))).Iter()) {
      Gv b = _b(r, d.I(Gv.I(1L)), Gd.Fn((_a) => _set_tool(Gd.A(_a, 0))).C("bind", d.I(Gv.I(0L))));
      b = b.S("toggle_mode", Gv.B(true));
      btns = btns.SI(d.I(Gv.I(0L)), b);
    }
    v.C("add_child", more);
    r = _row(more);
    _lab(r, Gv.S("Do trubky:"));
    foreach (Gv pd in (PARTS).Iter()) {
      if (Gv.In(pd.I(Gv.I(2L)), ON_PIPE)) {
        _part_btn(r, pd);
      }
    }
    r = _row(more);
    _lab(r, Gv.S("Díly:"));
    foreach (Gv pd_2 in (PARTS).Iter()) {
      if (!Gv.In(pd_2.I(Gv.I(2L)), ON_PIPE)) {
        _part_btn(r, pd_2);
      }
    }
    r = _row(more);
    _lab(r, Gv.S("Nová trubka:"));
    foreach (Gv dn in (Gd.Arr(Gv.I(10L), Gv.I(15L), Gv.I(25L), Gv.I(40L), Gv.I(50L), Gv.I(75L))).Iter()) {
      Gv bd = _b(r, (Gv.S("DN%d") % dn), Gd.Fn((_a) => _set_dn(Gd.A(_a, 0))).C("bind", dn));
      bd = bd.S("toggle_mode", Gv.B(true));
      btns = btns.SI((Gv.S("dn%d") % dn), bd);
    }
    _lab(r, Gv.S("  teče:"));
    foreach (Gv md in (MEDIA).Iter()) {
      Gv bm = _b(r, md.I(Gv.I(1L)), Gd.Fn((_a) => _set_med(Gd.A(_a, 0))).C("bind", md.I(Gv.I(0L))));
      bm = bm.S("toggle_mode", Gv.B(true));
      btns = btns.SI((Gv.S("med_") + md.I(Gv.I(0L))), bm);
    }
    r = _row(more);
    _lab(r, Gv.S("Barva cesty:"));
    foreach (Gv c in (COLORS).Iter()) {
      Gv bc = _b(r, c.I(Gv.I(1L)), Gd.Fn((_a) => _set_paint(Gd.A(_a, 0))).C("bind", c.I(Gv.I(0L))));
      bc = bc.S("toggle_mode", Gv.B(true));
      btns = btns.SI((Gv.S("col_") + c.I(Gv.I(0L))), bc);
      if (Gv.Ne(c.I(Gv.I(0L)), Gv.S(""))) {
        bc.C("add_theme_color_override", Gv.S("font_color"), Gd.Ctor("Color", c.I(Gv.I(0L))));
        bc.C("add_theme_color_override", Gv.S("font_pressed_color"), Gd.Ctor("Color", c.I(Gv.I(0L))).C("lightened", Gv.F(0.3)));
      }
    }
    props = Gd.New("VBoxContainer");
    props.C("add_theme_constant_override", Gv.S("separation"), Gv.I(4L));
    v.C("add_child", props);
    r = _row(v);
    _b(r, Gv.S("↶ Zpět (Ctrl+Z)"), Gd.Fn((_a) => undo()));
    _b(r, Gv.S("✓ Uložit (Ctrl+S)"), Gd.Fn((_a) => save()));
    _b(r, Gv.S("⟳ Uložit a načíst znovu"), Gd.Fn((_a) => save_reload()));
    _b(r, Gv.S("⇪ Export"), Gd.Fn((_a) => _export()));
    _b(r, Gv.S("⇩ Import"), Gd.Fn((_a) => _import()));
    _b(r, Gv.S("Obnovit původní"), Gd.Fn((_a) => reset_plan()));
    _b(r, Gv.S("✔ Hotovo (E)"), Gd.Fn((_a) => toggle()));
    info = Gd.New("Label");
    info = info.S("autowrap_mode", Gd.StaticGet("TextServer", "AUTOWRAP_WORD_SMART"));
    info = info.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(590L), Gv.I(0L)));
    info.C("add_theme_color_override", Gv.S("font_color"), Gd.Ctor("Color", Gv.S("c9d1d8")));
    info.C("add_theme_font_size_override", Gv.S("font_size"), Gv.I(12L));
    v.C("add_child", info);
    _set_dn(Gv.I(50L));
    _set_med(Gv.S("wort"));
    _set_paint(Gv.S("#e0452f"));
    _set_tool(Gv.S("select"));
    return Gv.Null;
  }
  public Gv _part_btn(Gv r = default(Gv), Gv pd = default(Gv)) {
    Gv bp = _b(r, pd.I(Gv.I(1L)), Gd.Fn((_a) => _set_part(Gd.A(_a, 0))).C("bind", pd.I(Gv.I(0L))));
    bp = bp.S("toggle_mode", Gv.B(true));
    btns = btns.SI((Gv.S("part_") + pd.I(Gv.I(0L))), bp);
    return Gv.Null;
  }
  public Gv _row(Gv parent = default(Gv)) {
    Gv r = Gd.New("HFlowContainer");
    r.C("add_theme_constant_override", Gv.S("h_separation"), Gv.I(4L));
    r.C("add_theme_constant_override", Gv.S("v_separation"), Gv.I(4L));
    parent.C("add_child", r);
    return r;
    return Gv.Null;
  }
  public Gv _lab(Gv parent = default(Gv), Gv text = default(Gv)) {
    Gv l = Gd.New("Label");
    l = l.S("text", text);
    l.C("add_theme_color_override", Gv.S("font_color"), Gd.Ctor("Color", Gv.S("8fa0ad")));
    parent.C("add_child", l);
    return l;
    return Gv.Null;
  }
  public Gv _b(Gv parent = default(Gv), Gv text = default(Gv), Gv f = default(Gv)) {
    Gv b = Gd.New("Button");
    b = b.S("text", text);
    b = b.S("focus_mode", Gd.StaticGet("Control", "FOCUS_NONE"));
    b.G("pressed").C("connect", f);
    parent.C("add_child", b);
    return b;
    return Gv.Null;
  }
  public Gv _set_tool(Gv t = default(Gv)) {
    tool = t;
    draw_pts.C("clear");
    pitch_a = Gv.Null;
    meas_a = Gv.Null;
    route_a = Gv.Null;
    if (Gv.Ne(t, Gv.S("route"))) {
      route_hl.C("clear");
    }
    foreach (Gv k in (btns).Iter()) {
      if (Gv.In(k, TOOLS)) {
        btns.I(k).C("set_pressed_no_signal", Gv.B(Gv.Eq(k, t)));
      } else if ((k.C("begins_with", Gv.S("part_"))).T()) {
        btns.I(k).C("set_pressed_no_signal", Gv.B((Gv.Eq(t, Gv.S("part")) && Gv.Eq(k, (Gv.S("part_") + part_key)))));
      }
    }
    _hint();
    Self.C("queue_redraw");
    return Gv.Null;
  }
  public Gv _set_part(Gv k = default(Gv)) {
    part_key = k;
    _set_tool(Gv.S("part"));
    return Gv.Null;
  }
  public Gv _set_dn(Gv dn = default(Gv)) {
    dn = (dn).AsI();
    dn_new = dn;
    foreach (Gv d in (Gd.Arr(Gv.I(10L), Gv.I(15L), Gv.I(25L), Gv.I(40L), Gv.I(50L), Gv.I(75L))).Iter()) {
      btns.I((Gv.S("dn%d") % d)).C("set_pressed_no_signal", Gv.B(Gv.Eq(d, dn)));
    }
    return Gv.Null;
  }
  public Gv _set_med(Gv md = default(Gv)) {
    med_new = md;
    foreach (Gv x in (MEDIA).Iter()) {
      btns.I((Gv.S("med_") + x.I(Gv.I(0L)))).C("set_pressed_no_signal", Gv.B(Gv.Eq(x.I(Gv.I(0L)), md)));
    }
    return Gv.Null;
  }
  public Gv _set_paint(Gv c = default(Gv)) {
    paint_col = c;
    foreach (Gv x in (COLORS).Iter()) {
      btns.I((Gv.S("col_") + x.I(Gv.I(0L)))).C("set_pressed_no_signal", Gv.B(Gv.Eq(x.I(Gv.I(0L)), c)));
    }
    if (Gv.Ne(tool, Gv.S("paint"))) {
      _set_tool(Gv.S("paint"));
    }
    return Gv.Null;
  }
  public Gv _part_def(Gv k = default(Gv)) {
    foreach (Gv pd in (PARTS).Iter()) {
      if (Gv.Eq(pd.I(Gv.I(0L)), k)) {
        return pd;
      }
    }
    return PARTS.I(Gv.I(0L));
    return Gv.Null;
  }
  public Gv _hint() {
    Gv h = Gd.Dict(Gv.S("select"), Gv.S("Klik = vybrat a táhnout. Tažení po prázdnu = rámeček (Shift přidá k výběru). Bod trubky přetáhni, dvojklik na trubku přidá bod, pravým na bod ho smaže. Roh nádoby táhni = velikost. Šipky posun, Del smaže, R otočí, Ctrl+D kopie. Posun plánku: prostřední tlačítko / WASD, zoom kolečkem."), Gv.S("pipe"), Gv.S("KRESLENÍ TRUBKY: klikej body (mřížka 10 cm, svisle/vodorovně; Alt = volně). Konec na jiné trubce nebo hrdle se napojí a trubka skončí. Jinak ukončí pravé tlačítko, Enter nebo klik na poslední bod. Esc zruší."), Gv.S("hose"), Gv.S("NEREZOVÁ HADICE (opletená): klikni začátek (hrdlo katety / konec trubky) a konec – hadice se prověsí obloukem."), Gv.S("pitch"), Gv.S("SPÍLACÍ HADICE: klikni začátek (výstup z chladiče / trubka), pak tank nebo spilku. Vznikne hadice s průhledítkem a T-kusem se dvěma klapkami (do tanku / výpust)."), Gv.S("joint"), Gv.S("SPOJ / ROZPOJ: klikni na místo, kde se trubky kříží nebo kde konec dosedá na jinou trubku – přepne propojení (žlutý bod = spojeno)."), Gv.S("measure"), Gv.S("MĚŘENÍ: klikni dva body – ukáže vzdálenost v metrech."), Gv.S("route"), Gv.S("CESTA: klikni na první nádobu, pak na druhou – ukáže, kudy se dá potrubím projet a přes které klapky a čerpadlo. Klapky nemění."), Gv.S("paint"), Gv.S("BARVA: klikni na trubku – obarví se proužkem zvolené barvy (pro přehled cest). Barva „bez“ proužek smaže."), Gv.S("part"), ((Gv.S("PŘIDAT „%s“: ") % Gd.Dyn("str", _part_def(part_key).I(Gv.I(1L)))) + ((Gv.In(_part_def(part_key).I(Gv.I(2L)), ON_PIPE) ? Gv.S("klikni na trubku – díl se vloží do trubky ve směru trubky.") : Gv.S("klikni do plánu. Vlastnosti (velikost, tvar, vazba na simulaci) nastavíš vpravo dole v panelu.")))));
    info = info.S("text", (h.C("get", tool, Gv.S("")) + (((dirty).T() ? Gv.S("\n● neuložené změny – Ctrl+S") : Gv.S("")))));
    return Gv.Null;
  }
  public Gv toggle() {
    on = Gv.B(!(on).T());
    ui = ui.S("visible", on);
    Self.S("visible", on);
    if ((on).T()) {
      m = m.S("running", Gv.B(false));
      if (Gv.Ne(m.G("tiles_root"), Gv.Null)) {
        m = m.S("tiles_root", m.G("tiles_root").S("visible", Gv.B(false)));
      }
      if (Gv.Ne(m.G("detail_spr"), Gv.Null)) {
        m = m.S("detail_spr", m.G("detail_spr").S("visible", Gv.B(false)));
      }
      Gv lv = m.C("make_plan_layers");
      live_s = lv.I(Gv.I(0L));
      live_i = lv.I(Gv.I(1L));
      if (Gv.Ne(m.G("game"), Gv.Null)) {
        m = m.S("game", m.G("game").S("quest", m.G("game").G("quest").S("visible", Gv.B(false))));
      }
      dock_was = m.G("dock_on");
      if ((m.G("dock_on")).T()) {
        m.C("dock_temp", Gv.B(false));
      }
      m.G("hud").C("toast", Gv.S("Editor: varna pozastavena. Úpravy se projeví hned, Ctrl+S je uloží."));
    } else {
      if (Gv.Ne(live_s, Gv.Null)) {
        live_s.C("queue_free");
        live_i.C("queue_free");
        live_s = Gv.Null;
      }
      if (Gv.Ne(m.G("tiles_root"), Gv.Null)) {
        m = m.S("tiles_root", m.G("tiles_root").S("visible", Gv.B(true)));
      }
      if (Gv.Ne(m.G("game"), Gv.Null)) {
        m = m.S("game", m.G("game").S("quest", m.G("game").G("quest").S("visible", Gv.B(true))));
      }
      if (((dock_was).T() && !(m.G("dock_on")).T())) {
        m.C("dock_temp", Gv.B(true));
      }
      m = m.S("detail_rect", Gd.Ctor("Rect2"));
      m = m.S("detail_sig", Gv.S(""));
      if ((dirty).T()) {
        m.G("hud").C("toast", Gv.S("Pozor: změny nejsou uložené (✎ → Uložit). Platí do zavření programu."));
      }
      m.C("compute_layout");
      m.C("_bake");
    }
    sel = Gv.Null;
    multi.C("clear");
    draw_pts.C("clear");
    pitch_a = Gv.Null;
    route_hl.C("clear");
    _props();
    _hint();
    Self.C("queue_redraw");
    return Gv.Null;
  }
  public Gv _snap_undo(Gv key = default(Gv)) {
    if (key.k == 0) key = Gv.S("");
    Gv now = Gd.Static("Time", "get_ticks_msec");
    if (((Gv.Ne(key, Gv.S("")) && Gv.Eq(key, undo_key)) && Gv.Lt((now - undo_t), Gv.I(1500L)))) {
      undo_t = now;
      return Gv.Null;
    }
    undo_key = key;
    undo_t = now;
    undo_stack.C("append", Gd.Static("JSON", "stringify", Gd.Dict(Gv.S("items"), _clean_items(), Gv.S("joints"), m.G("plan").C("get", Gv.S("joints"), Gd.Arr()))));
    if (Gv.Gt(undo_stack.C("size"), Gv.I(100L))) {
      undo_stack.C("pop_front");
    }
    return Gv.Null;
  }
  public Gv undo() {
    if ((undo_stack.C("is_empty")).T()) {
      m.G("hud").C("toast", Gv.S("Není co vracet."));
      return Gv.Null;
    }
    Gv d = Gd.Static("JSON", "parse_string", undo_stack.C("pop_back"));
    undo_key = Gv.S("");
    _restore(d.G("items"), d.G("joints"));
    sel = Gv.Null;
    multi.C("clear");
    _changed();
    _props();
    return Gv.Null;
  }
  public Gv _restore(Gv new_items = default(Gv), Gv joints = default(Gv)) {
    Gv byid = Gd.Dict();
    foreach (Gv it in (m.G("items")).Iter()) {
      byid = byid.SI(it.G("id"), it);
    }
    foreach (Gv k in (grave).Iter()) {
      if (!(byid.C("has", k)).T()) {
        byid = byid.SI(k, grave.I(k));
      }
    }
    Gv out_ = Gd.Arr();
    foreach (Gv nd in (new_items).Iter()) {
      Gv o = byid.C("get", nd.G("id"));
      if (Gv.Ne(o, Gv.Null)) {
        o.C("clear");
        o.C("merge", nd);
        out_.C("append", o);
      } else {
        out_.C("append", nd);
      }
    }
    foreach (Gv it_2 in (m.G("items")).Iter()) {
      if (!(out_.C("has", it_2)).T()) {
        grave = grave.SI(it_2.G("id"), it_2);
      }
    }
    m.G("items").C("clear");
    m.G("items").C("append_array", out_);
    m = m.S("plan", m.G("plan").S("items", m.G("items")));
    m = m.S("plan", m.G("plan").S("joints", joints));
    return Gv.Null;
  }
  public Gv _changed() {
    dirty = Gv.B(true);
    foreach (Gv v__it in (m.G("items")).Iter()) {
      Gv v = v__it;
      if (Gv.Eq(v.G("type"), Gv.S("valve"))) {
        v = v.SI(Gv.S("_ang"), Gd.deg_to_rad(Gd.Dyn("float", v.C("get", Gv.S("rot"), Gv.I(0L)))));
      }
    }
    foreach (Gv n__it in (m.G("live_nodes")).Iter()) {
      Gv n = n__it;
      if (Gv.Eq(n.G("it"), Gv.Null)) {
        continue;
      }
      n = n.S("visible", m.G("items").C("has", n.G("it")));
      if (Gv.Eq(n.G("kind"), Gv.S("pump"))) {
        n = n.S("position", Gd.Ctor("Vector2", Gd.Dyn("float", n.G("it").G("x")), Gd.Dyn("float", n.G("it").G("y"))));
      }
    }
    m.C("_init_outlets");
    m.G("sim").C("build_net", m.G("items"), m.G("plan").C("get", Gv.S("joints"), Gd.Arr()));
    m = m.S("sim", m.G("sim").S("valve_ver", (m.G("sim").G("valve_ver") + Gv.I(1L))));
    if (Gv.Ne(m.G("flow_layer"), Gv.Null)) {
      m.G("flow_layer").C("build");
    }
    if (Gv.Ne(live_s, Gv.Null)) {
      live_s.C("queue_redraw");
      live_i.C("queue_redraw");
    }
    m.C("redraw");
    _hint();
    Self.C("queue_redraw");
    return Gv.Null;
  }
  public Gv _live_redraw() {
    foreach (Gv n__it in (m.G("live_nodes")).Iter()) {
      Gv n = n__it;
      if ((Gv.Ne(n.G("it"), Gv.Null) && Gv.Eq(n.G("kind"), Gv.S("pump")))) {
        n = n.S("position", Gd.Ctor("Vector2", Gd.Dyn("float", n.G("it").G("x")), Gd.Dyn("float", n.G("it").G("y"))));
      }
    }
    if (Gv.Ne(live_s, Gv.Null)) {
      live_s.C("queue_redraw");
      live_i.C("queue_redraw");
    }
    m.C("redraw");
    Self.C("queue_redraw");
    return Gv.Null;
  }
  public Gv _clean_items() {
    Gv its = Gd.Arr();
    foreach (Gv it in (m.G("items")).Iter()) {
      Gv c = it.C("duplicate", Gv.B(true));
      foreach (Gv k in (c.C("keys")).Iter()) {
        if (((Gd.Dyn("str", k).C("begins_with", Gv.S("_"))).T() && Gv.Ne(k, Gv.S("_on")))) {
          c.C("erase", k);
        }
      }
      its.C("append", c);
    }
    return its;
    return Gv.Null;
  }
  public Gv _clean_plan() {
    Gv out_ = m.G("plan").C("duplicate", Gv.B(false));
    out_ = out_.S("items", _clean_items());
    return out_;
    return Gv.Null;
  }
  public Gv save() {
    m = m.S("plan", m.G("plan").S("items", m.G("items")));
    Gv f = Gd.Static("FileAccess", "open", m.G("USER_PLAN"), Gd.StaticGet("FileAccess", "WRITE"));
    if (Gv.Eq(f, Gv.Null)) {
      m.G("hud").C("toast", Gv.S("Plán nejde uložit."));
      return Gv.B(false);
    }
    f.C("store_string", Gd.Static("JSON", "stringify", _clean_plan(), Gv.S("  ")));
    f.C("close");
    dirty = Gv.B(false);
    m = m.S("plan_user", Gv.B(true));
    _hint();
    m.G("hud").C("toast", (Gv.S("Plán varny uložen (%d dílů). Načte se i po restartu.") % m.G("items").C("size")));
    m.G("sim").C("logm", Gv.S("Editor: upravený plán varny uložen."));
    return Gv.B(true);
    return Gv.Null;
  }
  public Gv save_reload() {
    if (!(save()).T()) {
      return Gv.Null;
    }
    m.C("save_game");
    m.G("hud").C("toast", Gv.S("Plán uložen – načítám trenažér znovu…"));
    _ = Self.C("get_tree").C("create_timer", Gv.F(0.5)).G("timeout");
    Self.C("get_tree").C("reload_current_scene");
    return Gv.Null;
  }
  public Gv _export() {
    _dialog(Gd.StaticGet("FileDialog", "FILE_MODE_SAVE_FILE"), Gd.Fn((_a) => { Gv path = Gd.A(_a, 0); Gv f = Gd.Static("FileAccess", "open", path, Gd.StaticGet("FileAccess", "WRITE")); if (Gv.Eq(f, Gv.Null)) {   m.G("hud").C("toast", Gv.S("Export se nepovedl."));   return Gv.Null; } f.C("store_string", Gd.Static("JSON", "stringify", _clean_plan(), Gv.S("  "))); f.C("close"); m.G("hud").C("toast", (Gv.S("Plán vyexportován: ") + path)); return Gv.Null; }));
    return Gv.Null;
  }
  public Gv _import() {
    _dialog(Gd.StaticGet("FileDialog", "FILE_MODE_OPEN_FILE"), Gd.Fn((_a) => { Gv path = Gd.A(_a, 0); Gv f = Gd.Static("FileAccess", "open", path, Gd.StaticGet("FileAccess", "READ")); if (Gv.Eq(f, Gv.Null)) {   m.G("hud").C("toast", Gv.S("Soubor nejde otevřít."));   return Gv.Null; } Gv d = Gd.Static("JSON", "parse_string", f.C("get_as_text")); if (!(((Gv.Is(d, "Dictionary") && (d.C("has", Gv.S("items"))).T()) && Gv.Is(d.G("items"), "Array")) && Gv.Gt(d.G("items").C("size"), Gv.I(5L)))) {   m.G("hud").C("toast", Gv.S("Tohle není plán varny (chybí díly)."));   return Gv.Null; } _snap_undo(); _restore(d.G("items"), d.C("get", Gv.S("joints"), Gd.Arr())); sel = Gv.Null; multi.C("clear"); _changed(); _props(); m.G("hud").C("toast", (Gv.S("Plán načten (%d dílů). „Uložit a načíst znovu“ ho uloží natrvalo.") % m.G("items").C("size"))); return Gv.Null; }));
    return Gv.Null;
  }
  public Gv _dialog(Gv mode = default(Gv), Gv cb = default(Gv)) {
    mode = (mode).AsI();
    if (Gv.Ne(fdlg, Gv.Null)) {
      fdlg.C("queue_free");
    }
    fdlg = Gd.New("FileDialog");
    fdlg = fdlg.S("access", Gd.StaticGet("FileDialog", "ACCESS_FILESYSTEM"));
    fdlg = fdlg.S("file_mode", mode);
    fdlg = fdlg.S("filters", Gd.Ctor("PackedStringArray", Gd.Arr(Gv.S("*.json ; Plán varny"))));
    fdlg = fdlg.S("current_file", Gv.S("plan_varny.json"));
    fdlg = fdlg.S("use_native_dialog", Gv.B(true));
    fdlg = fdlg.S("title", Gv.S("Plán varny"));
    fdlg.G("file_selected").C("connect", cb);
    ui.C("add_child", fdlg);
    fdlg.C("popup_centered", Gd.Ctor("Vector2i", Gv.I(900L), Gv.I(600L)));
    return Gv.Null;
  }
  public Gv reset_plan() {
    if ((Gd.Static("FileAccess", "file_exists", m.G("USER_PLAN"))).T()) {
      Gd.Static("DirAccess", "remove_absolute", Gd.Static("ProjectSettings", "globalize_path", m.G("USER_PLAN")));
    }
    m.C("save_game");
    m.G("hud").C("toast", Gv.S("Vrácen původní plán – načítám znovu…"));
    _ = Self.C("get_tree").C("create_timer", Gv.F(0.6)).G("timeout");
    Self.C("get_tree").C("reload_current_scene");
    return Gv.Null;
  }
  public Gv _z() {
    return ((Gv.Ne(m.G("cam"), Gv.Null) ? m.G("cam").G("zoom").G("x") : Gv.F(1.0))).AsF();
    return Gv.Null;
  }
  public Gv _snap(Gv p = default(Gv)) {
    return (((p / GRID)).C("round") * GRID);
    return Gv.Null;
  }
  public Gv _uid(Gv pre = default(Gv)) {
    return (Gv.S("%s_ed%d") % Gd.Arr(pre, Gd.Static("Time", "get_ticks_usec")));
    return Gv.Null;
  }
  public Gv _protected(Gv it = default(Gv)) {
    return Gv.B((Gv.In(it.G("type"), PROTECT) || Gv.Ne(Gd.Dyn("str", it.C("get", Gv.S("bind"), Gv.S(""))), Gv.S(""))));
    return Gv.Null;
  }
  public Gv _is_pipe(Gv it = default(Gv)) {
    return Gv.B(Gv.Eq(it.G("type"), Gv.S("pipe")));
    return Gv.Null;
  }
  public Gv _name(Gv it = default(Gv)) {
    Gv l = Gd.Dyn("str", it.C("get", Gv.S("label"), Gv.S("")));
    return (Gv.Ne(l, Gv.S("")) ? l : Gd.Dyn("str", it.G("type")));
    return Gv.Null;
  }
  public Gv _pipes(Gv with_hoses = default(Gv)) {
    if (with_hoses.k == 0) with_hoses = Gv.B(false);
    return m.G("items").C("filter", Gd.Fn((_a) => { Gv it = Gd.A(_a, 0); return Gv.B(((Gv.Eq(it.G("type"), Gv.S("pipe")) && ((with_hoses).T() || !(it.C("get", Gv.S("hose"), Gv.B(false))).T())) && Gv.Ge(it.G("pts").C("size"), Gv.I(2L)))); return Gv.Null; }));
    return Gv.Null;
  }
  public Gv _near_pt(Gv w = default(Gv), Gv tol = default(Gv), Gv with_hoses = default(Gv)) {
    tol = (tol).AsF();
    if (with_hoses.k == 0) with_hoses = Gv.B(true);
    Gv best = Gv.Null;
    Gv bd = tol;
    foreach (Gv p in (_pipes(with_hoses)).Iter()) {
      foreach (Gv i in (p.G("pts").C("size")).Iter()) {
        Gv d = w.C("distance_to", Gd.Ctor("Vector2", p.G("pts").I(i).I(Gv.I(0L)), p.G("pts").I(i).I(Gv.I(1L))));
        if (Gv.Lt(d, bd)) {
          bd = d;
          best = Gd.Dict(Gv.S("t"), Gv.S("pt"), Gv.S("it"), p, Gv.S("i"), i);
        }
      }
    }
    return best;
    return Gv.Null;
  }
  public Gv _near_seg(Gv w = default(Gv), Gv tol = default(Gv), Gv with_hoses = default(Gv)) {
    tol = (tol).AsF();
    if (with_hoses.k == 0) with_hoses = Gv.B(false);
    Gv best = Gv.Null;
    Gv bd = tol;
    foreach (Gv p in (_pipes(with_hoses)).Iter()) {
      foreach (Gv i in ((p.G("pts").C("size") - Gv.I(1L))).Iter()) {
        Gv a = Gd.Ctor("Vector2", p.G("pts").I(i).I(Gv.I(0L)), p.G("pts").I(i).I(Gv.I(1L)));
        Gv b = Gd.Ctor("Vector2", p.G("pts").I((i + Gv.I(1L))).I(Gv.I(0L)), p.G("pts").I((i + Gv.I(1L))).I(Gv.I(1L)));
        Gv q = Gd.Static("Geometry2D", "get_closest_point_to_segment", w, a, b);
        Gv d = w.C("distance_to", q);
        if (Gv.Lt(d, bd)) {
          bd = d;
          best = Gd.Dict(Gv.S("it"), p, Gv.S("i"), i, Gv.S("q"), q, Gv.S("ang"), Gd.rad_to_deg(((b - a)).C("angle")), Gv.S("a"), a, Gv.S("b"), b);
        }
      }
    }
    return best;
    return Gv.Null;
  }
  public Gv _vrect(Gv it = default(Gv)) {
    return Gd.Ctor("Rect2", Gd.Dyn("float", it.G("x")), Gd.Dyn("float", it.G("y")), Gd.Dyn("float", it.C("get", Gv.S("w"), Gv.I(100L))), Gd.Dyn("float", it.C("get", Gv.S("h"), Gv.I(100L))));
    return Gv.Null;
  }
  public Gv _near_item(Gv w = default(Gv), Gv tol = default(Gv)) {
    tol = (tol).AsF();
    Gv best = Gv.Null;
    Gv bd = tol;
    Gv vbest = Gv.Null;
    Gv va = Gv.F(1e+18);
    foreach (Gv it in (m.G("items")).Iter()) {
      if ((!(it.C("has", Gv.S("x"))).T() || Gv.In(it.G("type"), Gd.Arr(Gv.S("zone"), Gv.S("pipe"))))) {
        continue;
      }
      if ((Gv.Eq(it.G("type"), Gv.S("vessel")) && (it.C("has", Gv.S("w"))).T())) {
        Gv r = _vrect(it);
        if (((r.C("grow", Gv.F(4.0)).C("has_point", w)).T() && Gv.Lt(r.C("get_area"), va))) {
          va = r.C("get_area");
          vbest = it;
        }
        continue;
      }
      if ((Gv.Eq(it.G("type"), Gv.S("panel")) && (it.C("has", Gv.S("w"))).T())) {
        if (((_vrect(it).C("has_point", w)).T() && Gv.Eq(vbest, Gv.Null))) {
          vbest = it;
        }
        continue;
      }
      Gv c = Gd.Ctor("Vector2", it.G("x"), it.G("y"));
      if (Gv.Eq(it.G("type"), Gv.S("text"))) {
        c = (c + Gd.Ctor("Vector2", Gv.I(30L), Gv.I(-6L)));
      }
      Gv d = w.C("distance_to", c);
      if (Gv.Lt(d, bd)) {
        bd = d;
        best = it;
      }
    }
    return (Gv.Ne(best, Gv.Null) ? best : vbest);
    return Gv.Null;
  }
  public Gv _ports() {
    Gv o = Gd.Arr();
    foreach (Gv f in (m.G("items")).Iter()) {
      if (Gv.Eq(f.G("type"), Gv.S("fitting"))) {
        foreach (Gv q in (m.G("sim").C("fit_ports", f)).Iter()) {
          o.C("append", q);
        }
      }
    }
    foreach (Gv ol in (m.G("outlets")).Iter()) {
      o.C("append", ol.G("front"));
    }
    return o;
    return Gv.Null;
  }
  public Gv _snap_pipe_pt(Gv w = default(Gv), Gv alt = default(Gv)) {
    Gv tol = (Gv.F(14.0) / _z());
    foreach (Gv q in (_ports()).Iter()) {
      if (Gv.Lt(w.C("distance_to", q), tol)) {
        return Gd.Dict(Gv.S("p"), q, Gv.S("kind"), Gv.S("port"));
      }
    }
    Gv np = _near_pt(w, tol);
    if (Gv.Ne(np, Gv.Null)) {
      return Gd.Dict(Gv.S("p"), Gd.Ctor("Vector2", np.G("it").G("pts").I(np.G("i")).I(Gv.I(0L)), np.G("it").G("pts").I(np.G("i")).I(Gv.I(1L))), Gv.S("kind"), Gv.S("pt"));
    }
    Gv ns = _near_seg(w, tol);
    if (Gv.Ne(ns, Gv.Null)) {
      return Gd.Dict(Gv.S("p"), ns.G("q").C("round"), Gv.S("kind"), Gv.S("seg"));
    }
    return Gd.Dict(Gv.S("p"), ((alt).T() ? w : _snap(w)), Gv.S("kind"), Gv.S("free"));
    return Gv.Null;
  }
  public Gv _sel_items() {
    if ((multi.C("size")).T()) {
      return multi;
    }
    if (Gv.Ne(sel, Gv.Null)) {
      return Gd.Arr(sel.G("it"));
    }
    return Gd.Arr();
    return Gv.Null;
  }
  public Gv handle(Gv e = default(Gv)) {
    if (!(on).T()) {
      return Gv.B(false);
    }
    Gv fo = Self.C("get_viewport").C("gui_get_focus_owner");
    if ((Gv.Is(e, "InputEventKey") && Gv.Is(fo, "LineEdit"))) {
      return Gv.B(false);
    }
    if ((Gv.Is(e, "InputEventKey") && (e.G("pressed")).T())) {
      Gv k = e;
      if (((k.G("ctrl_pressed")).T() && Gv.Eq(k.G("keycode"), Gd.K("KEY_Z")))) {
        undo();
        return Gv.B(true);
      }
      if (((k.G("ctrl_pressed")).T() && Gv.Eq(k.G("keycode"), Gd.K("KEY_S")))) {
        save();
        return Gv.B(true);
      }
      if (((k.G("ctrl_pressed")).T() && Gv.Eq(k.G("keycode"), Gd.K("KEY_D")))) {
        _duplicate();
        return Gv.B(true);
      }
      if (((k.G("ctrl_pressed")).T() && Gv.Eq(k.G("keycode"), Gd.K("KEY_A")))) {
        _select_visible();
        return Gv.B(true);
      }
      if ((Gv.In(k.G("keycode"), Gd.Arr(Gd.K("KEY_LEFT"), Gd.K("KEY_RIGHT"), Gd.K("KEY_UP"), Gd.K("KEY_DOWN"))) && (_sel_items().C("size")).T())) {
        Gv st = ((k.G("shift_pressed")).T() ? Gv.F(100.0) : GRID);
        Gv dv = Gd.Dict(Gd.K("KEY_LEFT"), Gd.Ctor("Vector2", (-st), Gv.I(0L)), Gd.K("KEY_RIGHT"), Gd.Ctor("Vector2", st, Gv.I(0L)), Gd.K("KEY_UP"), Gd.Ctor("Vector2", Gv.I(0L), (-st)), Gd.K("KEY_DOWN"), Gd.Ctor("Vector2", Gv.I(0L), st)).I(k.G("keycode"));
        _snap_undo(Gv.S("arrows"));
        _move_items(_sel_items(), dv);
        _changed();
        _props();
        return Gv.B(true);
      }
      {
        Gv _t1 = k.G("keycode");
        if (Gv.Eq(_t1, Gd.K("KEY_ESCAPE"))) {
          if (((((draw_pts.C("size")).T() || Gv.Ne(pitch_a, Gv.Null)) || Gv.Ne(meas_a, Gv.Null)) || Gv.Ne(route_a, Gv.Null))) {
            draw_pts.C("clear");
            pitch_a = Gv.Null;
            meas_a = Gv.Null;
            route_a = Gv.Null;
            Self.C("queue_redraw");
          } else if ((Gv.Ne(sel, Gv.Null) || (multi.C("size")).T())) {
            sel = Gv.Null;
            multi.C("clear");
            _props();
            Self.C("queue_redraw");
          } else {
            toggle();
          }
          return Gv.B(true);
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_E"))) {
          toggle();
          return Gv.B(true);
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_DELETE")) || Gv.Eq(_t1, Gd.K("KEY_BACKSPACE"))) {
          _delete();
          return Gv.B(true);
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_R"))) {
          _rotate();
          return Gv.B(true);
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_ENTER")) || Gv.Eq(_t1, Gd.K("KEY_KP_ENTER"))) {
          _finish_pipe();
          return Gv.B(true);
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_V"))) {
          _set_tool(Gv.S("select"));
          return Gv.B(true);
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_T"))) {
          _set_tool(Gv.S("pipe"));
          return Gv.B(true);
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_H"))) {
          _set_tool(Gv.S("hose"));
          return Gv.B(true);
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_J"))) {
          _set_tool(Gv.S("joint"));
          return Gv.B(true);
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_M"))) {
          _set_tool(Gv.S("measure"));
          return Gv.B(true);
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_C"))) {
          _set_tool(Gv.S("route"));
          return Gv.B(true);
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_B"))) {
          _set_tool(Gv.S("paint"));
          return Gv.B(true);
        }
        else if (Gv.Eq(_t1, Gd.K("KEY_K"))) {
          _set_part(Gv.S("klapka"));
          return Gv.B(true);
        }
      }
      return Gv.B(false);
    }
    if (Gv.Is(e, "InputEventMouseMotion")) {
      hover_w = m.C("screen_to_world", e.G("position"));
      if ((drag).T()) {
        _drag_to(hover_w, e.G("alt_pressed"));
        return Gv.B(true);
      }
      if (Gv.Ne(tool, Gv.S("select"))) {
        Self.C("queue_redraw");
      }
      return Gv.B(false);
    }
    if (Gv.Is(e, "InputEventMouseButton")) {
      Gv mb = e;
      if (Gv.In(mb.G("button_index"), Gd.Arr(Gd.K("MOUSE_BUTTON_WHEEL_UP"), Gd.K("MOUSE_BUTTON_WHEEL_DOWN"), Gd.K("MOUSE_BUTTON_MIDDLE")))) {
        return Gv.B(false);
      }
      Gv w = m.C("screen_to_world", mb.G("position"));
      if (Gv.Ne(fo, Gv.Null)) {
        fo.C("release_focus");
      }
      if (Gv.Eq(mb.G("button_index"), Gd.K("MOUSE_BUTTON_LEFT"))) {
        if ((mb.G("pressed")).T()) {
          if (((mb.G("double_click")).T() && Gv.Eq(tool, Gv.S("select")))) {
            _insert_point(w);
            return Gv.B(true);
          }
          {
            Gv _t2 = tool;
            if (Gv.Eq(_t2, Gv.S("select"))) {
              _press_select(w, mb.G("shift_pressed"));
            }
            else if (Gv.Eq(_t2, Gv.S("pipe"))) {
              _pipe_click(w, mb.G("alt_pressed"));
            }
            else if (Gv.Eq(_t2, Gv.S("hose"))) {
              _hose_click(w);
            }
            else if (Gv.Eq(_t2, Gv.S("pitch"))) {
              _pitch_click(w);
            }
            else if (Gv.Eq(_t2, Gv.S("joint"))) {
              _joint_click(w);
            }
            else if (Gv.Eq(_t2, Gv.S("measure"))) {
              _measure_click(w);
            }
            else if (Gv.Eq(_t2, Gv.S("route"))) {
              _route_click(w);
            }
            else if (Gv.Eq(_t2, Gv.S("paint"))) {
              _paint_click(w);
            }
            else if (Gv.Eq(_t2, Gv.S("part"))) {
              _add_part(w);
            }
          }
        } else {
          if ((drag).T()) {
            _release();
          }
        }
        return Gv.B(true);
      }
      if ((Gv.Eq(mb.G("button_index"), Gd.K("MOUSE_BUTTON_RIGHT")) && (mb.G("pressed")).T())) {
        if ((Gv.Eq(tool, Gv.S("pipe")) && (draw_pts.C("size")).T())) {
          _finish_pipe();
          return Gv.B(true);
        }
        if (Gv.Ne(tool, Gv.S("select"))) {
          _set_tool(Gv.S("select"));
          return Gv.B(true);
        }
        Gv np = _near_pt(w, (Gv.F(12.0) / _z()));
        if ((Gv.Ne(np, Gv.Null) && Gv.Gt(np.G("it").G("pts").C("size"), Gv.I(2L)))) {
          _snap_undo();
          np.G("it").G("pts").C("remove_at", np.G("i"));
          sel = Gv.Null;
          _changed();
          _props();
        }
        return Gv.B(true);
      }
    }
    return Gv.B(false);
    return Gv.Null;
  }
  public Gv _press_select(Gv w = default(Gv), Gv shift = default(Gv)) {
    drag_start = w;
    if ((((Gv.Ne(sel, Gv.Null) && Gv.Eq(sel.G("t"), Gv.S("item"))) && Gv.In(sel.G("it").G("type"), Gd.Arr(Gv.S("vessel"), Gv.S("panel")))) && (sel.G("it").C("has", Gv.S("w"))).T())) {
      Gv r = _vrect(sel.G("it"));
      if (Gv.Lt(w.C("distance_to", r.G("end")), (Gv.F(14.0) / _z()))) {
        _snap_undo();
        drag = Gv.B(true);
        drag_mode = Gv.S("size");
        return Gv.Null;
      }
    }
    Gv np = _near_pt(w, (Gv.F(10.0) / _z()));
    Gv it = _near_item(w, (Gv.F(22.0) / _z()));
    Gv ns = Gv.Null;
    if ((Gv.Eq(np, Gv.Null) && Gv.Eq(it, Gv.Null))) {
      ns = _near_seg(w, (Gv.F(10.0) / _z()), Gv.B(true));
    }
    Gv hit = (Gv.Ne(it, Gv.Null) ? it : ((Gv.Ne(ns, Gv.Null) ? ns.G("it") : ((Gv.Ne(np, Gv.Null) ? np.G("it") : Gv.Null)))));
    if (((Gv.Ne(hit, Gv.Null) && (multi.C("has", hit)).T()) && !(shift).T())) {
      _snap_undo();
      drag = Gv.B(true);
      drag_mode = Gv.S("group");
      drag_orig = _orig(multi);
      return Gv.Null;
    }
    if ((shift).T()) {
      if (Gv.Ne(hit, Gv.Null)) {
        if ((Gv.Ne(sel, Gv.Null) && !(multi.C("has", sel.G("it"))).T())) {
          multi.C("append", sel.G("it"));
        }
        sel = Gv.Null;
        if ((multi.C("has", hit)).T()) {
          multi.C("erase", hit);
        } else {
          multi.C("append", hit);
        }
        _props();
        Self.C("queue_redraw");
        return Gv.Null;
      }
      drag = Gv.B(true);
      drag_mode = Gv.S("box");
      box_a = w;
      return Gv.Null;
    }
    multi.C("clear");
    if ((Gv.Ne(np, Gv.Null) && (Gv.Eq(it, Gv.Null) || Gv.Lt(w.C("distance_to", Gd.Ctor("Vector2", np.G("it").G("pts").I(np.G("i")).I(Gv.I(0L)), np.G("it").G("pts").I(np.G("i")).I(Gv.I(1L)))), w.C("distance_to", Gd.Ctor("Vector2", it.G("x"), it.G("y"))))))) {
      sel = np;
      _snap_undo();
      drag = Gv.B(true);
      drag_mode = Gv.S("one");
      Self.C("queue_redraw");
      _props();
      return Gv.Null;
    }
    if (Gv.Ne(it, Gv.Null)) {
      sel = Gd.Dict(Gv.S("t"), Gv.S("item"), Gv.S("it"), it);
      _snap_undo();
      drag = Gv.B(true);
      drag_mode = Gv.S("one");
      drag_off = (Gd.Ctor("Vector2", it.G("x"), it.G("y")) - w);
      Self.C("queue_redraw");
      _props();
      return Gv.Null;
    }
    if (Gv.Ne(ns, Gv.Null)) {
      sel = Gd.Dict(Gv.S("t"), Gv.S("pipe"), Gv.S("it"), ns.G("it"));
      drag = Gv.B(true);
      drag_mode = Gv.S("group");
      multi = Gd.Arr();
      drag_orig = _orig(Gd.Arr(ns.G("it")));
      _snap_undo();
      Self.C("queue_redraw");
      _props();
      return Gv.Null;
    }
    sel = Gv.Null;
    _props();
    drag = Gv.B(true);
    drag_mode = Gv.S("box");
    box_a = w;
    Self.C("queue_redraw");
    return Gv.Null;
  }
  public Gv _orig(Gv list = default(Gv)) {
    Gv o = Gd.Arr();
    foreach (Gv it in (list).Iter()) {
      o.C("append", Gd.Arr(it, ((it.C("has", Gv.S("pts"))).T() ? it.G("pts").C("duplicate", Gv.B(true)) : Gd.Ctor("Vector2", Gd.Dyn("float", it.C("get", Gv.S("x"), Gv.I(0L))), Gd.Dyn("float", it.C("get", Gv.S("y"), Gv.I(0L)))))));
    }
    return o;
    return Gv.Null;
  }
  public Gv _move_items(Gv list = default(Gv), Gv dv = default(Gv)) {
    foreach (Gv it__it in (list).Iter()) {
      Gv it = it__it;
      if ((it.C("has", Gv.S("pts"))).T()) {
        foreach (Gv q__it in (it.G("pts")).Iter()) {
          Gv q = q__it;
          q = q.SI(Gv.I(0L), (Gd.Dyn("float", q.I(Gv.I(0L))) + dv.G("x")));
          q = q.SI(Gv.I(1L), (Gd.Dyn("float", q.I(Gv.I(1L))) + dv.G("y")));
        }
      } else if ((it.C("has", Gv.S("x"))).T()) {
        it = it.S("x", (Gd.Dyn("float", it.G("x")) + dv.G("x")));
        it = it.S("y", (Gd.Dyn("float", it.G("y")) + dv.G("y")));
      }
    }
    return Gv.Null;
  }
  public Gv _drag_to(Gv w = default(Gv), Gv alt = default(Gv)) {
    {
      Gv _t1 = drag_mode;
      if (Gv.Eq(_t1, Gv.S("box"))) {
        Self.C("queue_redraw");
        return Gv.Null;
      }
      else if (Gv.Eq(_t1, Gv.S("size"))) {
        Gv it = sel.G("it");
        Gv e2 = ((alt).T() ? w : _snap(w));
        it = it.S("w", Gd.maxf(Gv.F(20.0), (e2.G("x") - Gd.Dyn("float", it.G("x")))));
        it = it.S("h", Gd.maxf(Gv.F(10.0), (e2.G("y") - Gd.Dyn("float", it.G("y")))));
        _live_redraw();
        _props_soft();
        return Gv.Null;
      }
      else if (Gv.Eq(_t1, Gv.S("group"))) {
        Gv dv = (w - drag_start);
        if (!(alt).T()) {
          dv = _snap(dv);
        }
        foreach (Gv o in (drag_orig).Iter()) {
          Gv it_2 = o.I(Gv.I(0L));
          if ((it_2.C("has", Gv.S("pts"))).T()) {
            foreach (Gv i in (it_2.G("pts").C("size")).Iter()) {
              it_2 = it_2.S("pts", it_2.G("pts").SI(i, Gd.Arr((o.I(Gv.I(1L)).I(i).I(Gv.I(0L)) + dv.G("x")), (o.I(Gv.I(1L)).I(i).I(Gv.I(1L)) + dv.G("y")))));
            }
          } else {
            it_2 = it_2.S("x", (o.I(Gv.I(1L)).G("x") + dv.G("x")));
            it_2 = it_2.S("y", (o.I(Gv.I(1L)).G("y") + dv.G("y")));
          }
        }
        _live_redraw();
        return Gv.Null;
      }
    }
    if (Gv.Eq(sel, Gv.Null)) {
      return Gv.Null;
    }
    if (Gv.Eq(sel.G("t"), Gv.S("pt"))) {
      Gv own = Gd.Ctor("Vector2", sel.G("it").G("pts").I(sel.G("i")).I(Gv.I(0L)), sel.G("it").G("pts").I(sel.G("i")).I(Gv.I(1L)));
      Gv sp = _snap_pipe_pt(w, alt);
      Gv p = sp.G("p");
      if ((Gv.Eq(sp.G("kind"), Gv.S("pt")) && Gv.Eq(p, own))) {
        p = ((alt).T() ? w : _snap(w));
      }
      sel = sel.S("it", sel.G("it").S("pts", sel.G("it").G("pts").SI(sel.G("i"), Gd.Arr(p.G("x"), p.G("y")))));
    } else if (Gv.Eq(sel.G("t"), Gv.S("item"))) {
      Gv it_3 = sel.G("it");
      Gv p2 = (w + drag_off);
      if (Gv.In(it_3.G("type"), ON_PIPE)) {
        Gv ns = _near_seg(p2, (Gv.F(24.0) / _z()));
        if (Gv.Ne(ns, Gv.Null)) {
          p2 = ns.G("q").C("round");
          if (Gv.Ne(it_3.G("type"), Gv.S("pump"))) {
            it_3 = it_3.SI(Gv.S("rot"), Gd.roundi(ns.G("ang")));
          }
        } else {
          p2 = _snap(p2);
        }
      } else if (!(alt).T()) {
        p2 = _snap(p2);
      }
      it_3 = it_3.S("x", p2.G("x"));
      it_3 = it_3.S("y", p2.G("y"));
    }
    _live_redraw();
    _props_soft();
    return Gv.Null;
  }
  public Gv _release() {
    drag = Gv.B(false);
    if (Gv.Eq(drag_mode, Gv.S("box"))) {
      Gv r = Gd.Ctor("Rect2", box_a, Gd.StaticGet("Vector2", "ZERO")).C("expand", hover_w);
      if (Gv.Gt(r.G("size").C("length"), (Gv.F(6.0) / _z()))) {
        foreach (Gv it in (m.G("items")).Iter()) {
          if (Gv.Eq(it.G("type"), Gv.S("zone"))) {
            continue;
          }
          Gv inside = Gv.B(false);
          if ((it.C("has", Gv.S("pts"))).T()) {
            inside = Gv.B(Gv.Gt(it.G("pts").C("size"), Gv.I(0L)));
            foreach (Gv q in (it.G("pts")).Iter()) {
              if (!(r.C("has_point", Gd.Ctor("Vector2", q.I(Gv.I(0L)), q.I(Gv.I(1L))))).T()) {
                inside = Gv.B(false);
                break;
              }
            }
          } else if ((Gv.In(it.G("type"), Gd.Arr(Gv.S("vessel"), Gv.S("panel"))) && (it.C("has", Gv.S("w"))).T())) {
            inside = r.C("encloses", _vrect(it));
          } else if ((it.C("has", Gv.S("x"))).T()) {
            inside = r.C("has_point", Gd.Ctor("Vector2", it.G("x"), it.G("y")));
          }
          if (((inside).T() && !(multi.C("has", it)).T())) {
            multi.C("append", it);
          }
        }
        if (Gv.Eq(multi.C("size"), Gv.I(1L))) {
          sel = Gd.Dict(Gv.S("t"), ((multi.I(Gv.I(0L)).C("has", Gv.S("pts"))).T() ? Gv.S("pipe") : Gv.S("item")), Gv.S("it"), multi.I(Gv.I(0L)));
          multi.C("clear");
        } else if ((multi.C("size")).T()) {
          sel = Gv.Null;
        }
      }
      drag_mode = Gv.S("");
      _props();
      Self.C("queue_redraw");
      return Gv.Null;
    }
    drag_mode = Gv.S("");
    _changed();
    _props();
    return Gv.Null;
  }
  public Gv _select_visible() {
    Gv r = m.C("visible_world_rect");
    multi.C("clear");
    sel = Gv.Null;
    foreach (Gv it in (m.G("items")).Iter()) {
      if (Gv.Eq(it.G("type"), Gv.S("zone"))) {
        continue;
      }
      Gv p = (((it.C("has", Gv.S("pts"))).T() && (it.G("pts").C("size")).T()) ? Gd.Ctor("Vector2", it.G("pts").I(Gv.I(0L)).I(Gv.I(0L)), it.G("pts").I(Gv.I(0L)).I(Gv.I(1L))) : Gd.Ctor("Vector2", Gd.Dyn("float", it.C("get", Gv.S("x"), Gv.F(-1000000000.0))), Gd.Dyn("float", it.C("get", Gv.S("y"), Gv.F(-1000000000.0)))));
      if ((r.C("has_point", p)).T()) {
        multi.C("append", it);
      }
    }
    _props();
    Self.C("queue_redraw");
    m.G("hud").C("toast", (Gv.S("Vybráno %d dílů na obrazovce.") % multi.C("size")));
    return Gv.Null;
  }
  public Gv _insert_point(Gv w = default(Gv)) {
    Gv ns = _near_seg(w, (Gv.F(10.0) / _z()), Gv.B(true));
    if (Gv.Eq(ns, Gv.Null)) {
      return Gv.Null;
    }
    _snap_undo();
    Gv q = ns.G("q").C("round");
    ns.G("it").G("pts").C("insert", (ns.G("i") + Gv.I(1L)), Gd.Arr(q.G("x"), q.G("y")));
    sel = Gd.Dict(Gv.S("t"), Gv.S("pt"), Gv.S("it"), ns.G("it"), Gv.S("i"), (ns.G("i") + Gv.I(1L)));
    multi.C("clear");
    _changed();
    _props();
    return Gv.Null;
  }
  public Gv _pipe_click(Gv w = default(Gv), Gv alt = default(Gv)) {
    Gv sp = _snap_pipe_pt(w, alt);
    Gv p = sp.G("p");
    if ((((draw_pts.C("size")).T() && !(alt).T()) && Gv.Eq(sp.G("kind"), Gv.S("free")))) {
      Gv last = draw_pts.I((draw_pts.C("size") - Gv.I(1L))).G("p");
      if (Gv.Gt(Gd.absf((p.G("x") - last.G("x"))), Gd.absf((p.G("y") - last.G("y"))))) {
        p = p.S("y", last.G("y"));
      } else {
        p = p.S("x", last.G("x"));
      }
    }
    if (((draw_pts.C("size")).T() && Gv.Lt(p.C("distance_to", draw_pts.I((draw_pts.C("size") - Gv.I(1L))).G("p")), (Gv.F(6.0) / _z())))) {
      _finish_pipe();
      return Gv.Null;
    }
    draw_pts.C("append", Gd.Dict(Gv.S("p"), p, Gv.S("kind"), sp.G("kind")));
    if ((Gv.Ge(draw_pts.C("size"), Gv.I(2L)) && Gv.Ne(sp.G("kind"), Gv.S("free")))) {
      _finish_pipe();
      return Gv.Null;
    }
    m.G("hud").C("toast", (Gv.S("Bod %d – další klik pokračuje, pravé tlačítko / Enter trubku ukončí.") % draw_pts.C("size")));
    Self.C("queue_redraw");
    return Gv.Null;
  }
  public Gv _finish_pipe() {
    if (Gv.Lt(draw_pts.C("size"), Gv.I(2L))) {
      if (Gv.Eq(draw_pts.C("size"), Gv.I(1L))) {
        m.G("hud").C("toast", Gv.S("Trubka potřebuje aspoň dva body."));
      }
      draw_pts.C("clear");
      Self.C("queue_redraw");
      return Gv.Null;
    }
    _snap_undo();
    Gv pts = Gd.Arr();
    foreach (Gv d in (draw_pts).Iter()) {
      pts.C("append", Gd.Arr(d.G("p").G("x"), d.G("p").G("y")));
    }
    Gv np = Gd.Dict(Gv.S("id"), _uid(Gv.S("pipe")), Gv.S("type"), Gv.S("pipe"), Gv.S("dn"), dn_new, Gv.S("medium"), med_new, Gv.S("label"), Gv.S(""), Gv.S("pts"), pts);
    m.G("items").C("append", np);
    Gv J = m.G("plan").C("get", Gv.S("joints"), Gd.Arr());
    foreach (Gv d_2 in (Gd.Arr(draw_pts.I(Gv.I(0L)), draw_pts.I((draw_pts.C("size") - Gv.I(1L))))).Iter()) {
      if (Gv.Eq(d_2.G("kind"), Gv.S("seg"))) {
        J.C("append", Gd.Arr(d_2.G("p").G("x"), d_2.G("p").G("y")));
      }
    }
    m = m.S("plan", m.G("plan").S("joints", J));
    draw_pts.C("clear");
    _changed();
    sel = Gd.Dict(Gv.S("t"), Gv.S("pipe"), Gv.S("it"), np);
    multi.C("clear");
    _props();
    m.G("hud").C("toast", (Gv.S("Trubka přidána (DN%d). Konce na jiné trubce se napojily.") % dn_new));
    return Gv.Null;
  }
  public Gv _hose_pts(Gv A = default(Gv), Gv B = default(Gv)) {
    Gv d = (B - A);
    Gv L = d.C("length");
    Gv n = (Gd.Ctor("Vector2", (-d.G("y")), d.G("x")) / Gd.maxf(Gv.F(1.0), L));
    if (Gv.Lt(n.G("y"), Gv.I(0L))) {
      n = (-n);
    }
    Gv bow = Gd.minf(Gv.F(120.0), (L * Gv.F(0.18)));
    Gv o = Gd.Arr();
    foreach (Gv i in (Gv.I(5L)).Iter()) {
      Gv t = (i / Gv.F(4.0));
      Gv q = (A.C("lerp", B, t) + ((n * bow) * Gd.sin((Gd.K("PI") * t))));
      o.C("append", Gd.Arr(Gd.roundf(q.G("x")), Gd.roundf(q.G("y"))));
    }
    return o;
    return Gv.Null;
  }
  public Gv _hose_click(Gv w = default(Gv)) {
    Gv sp = _snap_pipe_pt(w, Gv.B(false));
    draw_pts.C("append", Gd.Dict(Gv.S("p"), sp.G("p"), Gv.S("kind"), sp.G("kind")));
    if (Gv.Lt(draw_pts.C("size"), Gv.I(2L))) {
      m.G("hud").C("toast", Gv.S("Hadice: teď klikni na konec."));
      Self.C("queue_redraw");
      return Gv.Null;
    }
    Gv A = draw_pts.I(Gv.I(0L)).G("p");
    Gv B = draw_pts.I(Gv.I(1L)).G("p");
    draw_pts.C("clear");
    if (Gv.Lt(A.C("distance_to", B), Gv.F(40.0))) {
      m.G("hud").C("toast", Gv.S("Hadice je moc krátká."));
      return Gv.Null;
    }
    _snap_undo();
    Gv h = Gd.Dict(Gv.S("id"), _uid(Gv.S("pipe")), Gv.S("type"), Gv.S("pipe"), Gv.S("hose"), Gv.B(true), Gv.S("look"), Gv.S("hose"), Gv.S("dn"), Gd.mini(dn_new, Gv.I(50L)), Gv.S("medium"), med_new, Gv.S("label"), Gv.S("nerezová hadice"), Gv.S("pts"), _hose_pts(A, B));
    m.G("items").C("append", h);
    _changed();
    sel = Gd.Dict(Gv.S("t"), Gv.S("pipe"), Gv.S("it"), h);
    multi.C("clear");
    _props();
    m.G("hud").C("toast", Gv.S("Hadice přidána."));
    return Gv.Null;
  }
  public Gv _pitch_click(Gv w = default(Gv)) {
    Gv sp = _snap_pipe_pt(w, Gv.B(false));
    if (Gv.Eq(pitch_a, Gv.Null)) {
      pitch_a = sp.G("p");
      m.G("hud").C("toast", Gv.S("Spílací hadice: teď klikni na tank / spilku (konec hadice)."));
      Self.C("queue_redraw");
      return Gv.Null;
    }
    Gv A = pitch_a;
    Gv B = sp.G("p");
    pitch_a = Gv.Null;
    Gv d = (B - A);
    Gv L = d.C("length");
    if (Gv.Lt(L, Gv.F(200.0))) {
      m.G("hud").C("toast", Gv.S("Moc krátké – konec dej dál (aspoň 2 m)."));
      return Gv.Null;
    }
    _snap_undo();
    Gv u = (d / L);
    Gv nrm = Gd.Ctor("Vector2", (-u.G("y")), u.G("x"));
    Gv ang = Gd.rad_to_deg(u.C("angle"));
    Gv st = Gd.Dyn("str", Gd.Static("Time", "get_ticks_usec"));
    Gv C = ((B - (u * Gv.F(110.0)))).C("round");
    Gv fit = Gd.Dict(Gv.S("id"), (Gv.S("fit_ed%sp") % st), Gv.S("type"), Gv.S("fitting"), Gv.S("label"), Gv.S("T-kus spílací"), Gv.S("x"), C.G("x"), Gv.S("y"), C.G("y"), Gv.S("ports"), Gd.Arr(Gd.fposmod((ang + Gv.F(180.0)), Gv.F(360.0)), Gd.fposmod(ang, Gv.F(360.0)), Gd.fposmod((ang + Gv.F(90.0)), Gv.F(360.0))), Gv.S("arm"), Gv.I(30L));
    m.G("items").C("append", fit);
    Gv fp = m.G("sim").C("fit_ports", fit);
    Gv hp = _hose_pts(A, fp.I(Gv.I(0L)));
    m.G("items").C("append", Gd.Dict(Gv.S("id"), (Gv.S("pipe_ed%sh") % st), Gv.S("type"), Gv.S("pipe"), Gv.S("medium"), Gv.S("wort"), Gv.S("hose"), Gv.B(true), Gv.S("dn"), Gv.I(50L), Gv.S("label"), Gv.S("spílací hadice"), Gv.S("pts"), hp));
    Gv s0 = Gd.Ctor("Vector2", hp.I(Gv.I(2L)).I(Gv.I(0L)), hp.I(Gv.I(2L)).I(Gv.I(1L)));
    Gv s1 = Gd.Ctor("Vector2", hp.I(Gv.I(3L)).I(Gv.I(0L)), hp.I(Gv.I(3L)).I(Gv.I(1L)));
    m.G("items").C("append", Gd.Dict(Gv.S("id"), (Gv.S("inl_ed%ss") % st), Gv.S("type"), Gv.S("inl"), Gv.S("k"), Gv.S("pruh"), Gv.S("label"), Gv.S("průhledítko"), Gv.S("x"), Gd.roundf((((s0.G("x") + s1.G("x"))) / Gv.F(2.0))), Gv.S("y"), Gd.roundf((((s0.G("y") + s1.G("y"))) / Gv.F(2.0))), Gv.S("rot"), Gd.roundi(Gd.rad_to_deg(((s1 - s0)).C("angle")))));
    m.G("items").C("append", Gd.Dict(Gv.S("id"), (Gv.S("pipe_ed%st") % st), Gv.S("type"), Gv.S("pipe"), Gv.S("medium"), Gv.S("wort"), Gv.S("dn"), Gv.I(50L), Gv.S("label"), Gv.S(""), Gv.S("pts"), Gd.Arr(Gd.Arr(fp.I(Gv.I(1L)).G("x"), fp.I(Gv.I(1L)).G("y")), Gd.Arr(B.G("x"), B.G("y")))));
    Gv vt = ((((fp.I(Gv.I(1L)) + B)) / Gv.F(2.0))).C("round");
    m.G("items").C("append", Gd.Dict(Gv.S("id"), (Gv.S("valve_ed%s1") % st), Gv.S("type"), Gv.S("valve"), Gv.S("kind"), Gv.S("klapka"), Gv.S("label"), Gv.S("klapka tank"), Gv.S("open"), Gv.B(false), Gv.S("x"), vt.G("x"), Gv.S("y"), vt.G("y"), Gv.S("rot"), Gd.roundi(ang)));
    Gv E = ((fp.I(Gv.I(2L)) + (nrm * Gv.F(90.0)))).C("round");
    m.G("items").C("append", Gd.Dict(Gv.S("id"), (Gv.S("pipe_ed%sv") % st), Gv.S("type"), Gv.S("pipe"), Gv.S("medium"), Gv.S("wort"), Gv.S("dn"), Gv.I(50L), Gv.S("label"), Gv.S("výpust"), Gv.S("pts"), Gd.Arr(Gd.Arr(fp.I(Gv.I(2L)).G("x"), fp.I(Gv.I(2L)).G("y")), Gd.Arr(E.G("x"), E.G("y")))));
    Gv vv = ((((fp.I(Gv.I(2L)) + E)) / Gv.F(2.0))).C("round");
    m.G("items").C("append", Gd.Dict(Gv.S("id"), (Gv.S("valve_ed%s2") % st), Gv.S("type"), Gv.S("valve"), Gv.S("kind"), Gv.S("klapka"), Gv.S("label"), Gv.S("klapka výpust"), Gv.S("open"), Gv.B(false), Gv.S("x"), vv.G("x"), Gv.S("y"), vv.G("y"), Gv.S("rot"), Gd.roundi((ang + Gv.F(90.0)))));
    if (Gv.Eq(sp.G("kind"), Gv.S("seg"))) {
      Gv J = m.G("plan").C("get", Gv.S("joints"), Gd.Arr());
      J.C("append", Gd.Arr(B.G("x"), B.G("y")));
      m = m.S("plan", m.G("plan").S("joints", J));
    }
    _changed();
    _set_tool(Gv.S("select"));
    m.G("hud").C("toast", Gv.S("Spílací hadice s průhledítkem a T-kusem se dvěma klapkami (do tanku / výpust) přidána. Klapky jsou zavřené."));
    return Gv.Null;
  }
  public Gv _joint_click(Gv w = default(Gv)) {
    Gv J = m.G("plan").C("get", Gv.S("joints"), Gd.Arr());
    Gv tol = (Gv.F(12.0) / _z());
    foreach (Gv i in (J.C("size")).Iter()) {
      if (Gv.Lt(w.C("distance_to", Gd.Ctor("Vector2", J.I(i).I(Gv.I(0L)), J.I(i).I(Gv.I(1L)))), tol)) {
        _snap_undo();
        J.C("remove_at", i);
        m = m.S("plan", m.G("plan").S("joints", J));
        _changed();
        m.G("hud").C("toast", Gv.S("Rozpojeno – trubky tudy jen procházejí přes sebe."));
        return Gv.Null;
      }
    }
    Gv best = Gd.StaticGet("Vector2", "INF");
    Gv bd = tol;
    Gv ps = _pipes();
    foreach (Gv a in (ps.C("size")).Iter()) {
      foreach (Gv b in Gd.RangeIter((a + Gv.I(1L)), ps.C("size"))) {
        Gv A = ps.I(a);
        Gv B = ps.I(b);
        foreach (Gv i_2 in ((A.G("pts").C("size") - Gv.I(1L))).Iter()) {
          foreach (Gv j in ((B.G("pts").C("size") - Gv.I(1L))).Iter()) {
            Gv x = Gd.Static("Geometry2D", "segment_intersects_segment", Gd.Ctor("Vector2", A.G("pts").I(i_2).I(Gv.I(0L)), A.G("pts").I(i_2).I(Gv.I(1L))), Gd.Ctor("Vector2", A.G("pts").I((i_2 + Gv.I(1L))).I(Gv.I(0L)), A.G("pts").I((i_2 + Gv.I(1L))).I(Gv.I(1L))), Gd.Ctor("Vector2", B.G("pts").I(j).I(Gv.I(0L)), B.G("pts").I(j).I(Gv.I(1L))), Gd.Ctor("Vector2", B.G("pts").I((j + Gv.I(1L))).I(Gv.I(0L)), B.G("pts").I((j + Gv.I(1L))).I(Gv.I(1L))));
            if ((Gv.Ne(x, Gv.Null) && Gv.Lt(w.C("distance_to", x), bd))) {
              bd = w.C("distance_to", x);
              best = (x).C("round");
            }
          }
        }
      }
    }
    if (Gv.Eq(best, Gd.StaticGet("Vector2", "INF"))) {
      Gv np = _near_pt(w, tol, Gv.B(false));
      if (Gv.Ne(np, Gv.Null)) {
        best = Gd.Ctor("Vector2", np.G("it").G("pts").I(np.G("i")).I(Gv.I(0L)), np.G("it").G("pts").I(np.G("i")).I(Gv.I(1L)));
      }
    }
    if (Gv.Eq(best, Gd.StaticGet("Vector2", "INF"))) {
      m.G("hud").C("toast", Gv.S("Tady se žádné trubky nekříží ani nedotýkají."));
      return Gv.Null;
    }
    _snap_undo();
    J.C("append", Gd.Arr(best.G("x"), best.G("y")));
    m = m.S("plan", m.G("plan").S("joints", J));
    _changed();
    m.G("hud").C("toast", Gv.S("Spojeno – kapalina tudy může přetékat mezi trubkami."));
    return Gv.Null;
  }
  public Gv _measure_click(Gv w = default(Gv)) {
    Gv p = _snap_pipe_pt(w, Gv.B(false)).G("p");
    if (Gv.Eq(meas_a, Gv.Null)) {
      meas_a = p;
      m.G("hud").C("toast", Gv.S("Měření: klikni na druhý bod."));
      Self.C("queue_redraw");
      return Gv.Null;
    }
    Gv d = (p - meas_a);
    m.G("hud").C("toast", (Gv.S("Vzdálenost %.2f m (vodorovně %.2f m, svisle %.2f m)") % Gd.Arr((d.C("length") / PX_PER_M), (Gd.absf(d.G("x")) / PX_PER_M), (Gd.absf(d.G("y")) / PX_PER_M))));
    meas_a = Gv.Null;
    Self.C("queue_redraw");
    return Gv.Null;
  }
  public Gv _vessel_at(Gv w = default(Gv)) {
    Gv best = Gv.Null;
    Gv ba = Gv.F(1e+18);
    foreach (Gv it in (m.G("items")).Iter()) {
      if ((((Gv.Eq(it.G("type"), Gv.S("vessel")) && (it.C("has", Gv.S("w"))).T()) && Gv.Ne(Gd.Dyn("str", it.C("get", Gv.S("bind"), Gv.S(""))), Gv.S(""))) && (_vrect(it).C("grow", Gv.F(8.0)).C("has_point", w)).T())) {
        if (Gv.Lt(_vrect(it).C("get_area"), ba)) {
          ba = _vrect(it).C("get_area");
          best = it;
        }
      }
    }
    return best;
    return Gv.Null;
  }
  public Gv _route_click(Gv w = default(Gv)) {
    Gv v = _vessel_at(w);
    if (Gv.Eq(v, Gv.Null)) {
      m.G("hud").C("toast", Gv.S("Klikni na nádobu (pánev, SK, vířivka, bojler, spilka…)."));
      return Gv.Null;
    }
    if (Gv.Eq(route_a, Gv.Null)) {
      route_a = v;
      route_hl.C("clear");
      m.G("hud").C("toast", (Gv.S("Cesta z: %s – teď klikni na cílovou nádobu.") % _name(v)));
      Self.C("queue_redraw");
      return Gv.Null;
    }
    Gv a = route_a;
    route_a = Gv.Null;
    Gv found = Gv.Null;
    Gv via = Gv.S("");
    Gv r0 = m.G("sim").C("route_valves", a.G("bind"), v.G("bind"), Gd.Arr());
    if (Gv.Ne(r0, Gv.Null)) {
      found = r0;
    } else {
      foreach (Gv pb in (Gd.Arr(Gv.S("p1"), Gv.S("p2"), Gv.S("p3"), Gv.S("phw"), Gv.S("plv"))).Iter()) {
        Gv r1 = m.G("sim").C("route_valves", a.G("bind"), v.G("bind"), Gd.Arr(pb));
        if (Gv.Ne(r1, Gv.Null)) {
          found = r1;
          via = pb;
          break;
        }
      }
    }
    route_hl.C("clear");
    if (Gv.Eq(found, Gv.Null)) {
      m.G("hud").C("toast", (Gv.S("Z %s do %s se potrubím nedostaneš (chybí spoj nebo trubka).") % Gd.Arr(_name(a), _name(v))));
      Self.C("queue_redraw");
      return Gv.Null;
    }
    Gv keys = m.G("sim").G("last_route");
    foreach (Gv i in ((keys.C("size") - Gv.I(1L))).Iter()) {
      Gv n = m.G("sim").G("net").G("nodes").C("get", keys.I(i));
      if (Gv.Eq(n, Gv.Null)) {
        continue;
      }
      Gv pl = Gd.Ctor("PackedVector2Array", Gd.Arr(Gd.Ctor("Vector2", n.G("x"), n.G("y")), Gd.Ctor("Vector2", m.G("sim").G("net").G("nodes").I(keys.I((i + Gv.I(1L)))).G("x"), m.G("sim").G("net").G("nodes").I(keys.I((i + Gv.I(1L)))).G("y"))));
      foreach (Gv e in (n.G("edges")).Iter()) {
        if (((Gv.Eq(e.G("a"), keys.I(i)) && Gv.Eq(e.G("b"), keys.I((i + Gv.I(1L))))) || (Gv.Eq(e.G("b"), keys.I(i)) && Gv.Eq(e.G("a"), keys.I((i + Gv.I(1L))))))) {
          pl = Gd.Ctor("PackedVector2Array", e.G("pts"));
          break;
        }
      }
      route_hl.C("append", pl);
    }
    Gv pn = Gv.S("");
    if (Gv.Ne(via, Gv.S(""))) {
      foreach (Gv it in (m.G("items")).Iter()) {
        if ((Gv.Eq(it.G("type"), Gv.S("pump")) && Gv.Eq(it.C("get", Gv.S("bind"), Gv.S("")), via))) {
          pn = (Gv.S(" přes čerpadlo %s") % _name(it));
        }
      }
    }
    m.G("hud").C("toast", (Gv.S("Cesta %s → %s%s: %d klapek na trase (zvýrazněno žlutě, klapky jsem neměnil).") % Gd.Arr(_name(a), _name(v), pn, found.C("size"))));
    Self.C("queue_redraw");
    return Gv.Null;
  }
  public Gv _paint_click(Gv w = default(Gv)) {
    Gv ns = _near_seg(w, (Gv.F(12.0) / _z()), Gv.B(true));
    if (Gv.Eq(ns, Gv.Null)) {
      m.G("hud").C("toast", Gv.S("Klikni na trubku."));
      return Gv.Null;
    }
    _snap_undo();
    if (Gv.Eq(paint_col, Gv.S(""))) {
      ns.G("it").C("erase", Gv.S("color"));
    } else {
      ns = ns.S("it", ns.G("it").SI(Gv.S("color"), paint_col));
    }
    _changed();
    return Gv.Null;
  }
  public Gv _add_part(Gv w = default(Gv)) {
    Gv pd = _part_def(part_key);
    Gv typ = pd.I(Gv.I(2L));
    Gv it = (pd.I(Gv.I(3L))).C("duplicate", Gv.B(true));
    it = it.SI(Gv.S("type"), typ);
    it = it.SI(Gv.S("id"), _uid(Gd.Dict(Gv.S("valve"), Gv.S("valve"), Gv.S("inl"), Gv.S("inl"), Gv.S("flow"), Gv.S("flow"), Gv.S("fitting"), Gv.S("fit"), Gv.S("text"), Gv.S("text"), Gv.S("vessel"), Gv.S("vessel"), Gv.S("pump"), Gv.S("pump")).C("get", typ, typ)));
    if (Gv.In(typ, ON_PIPE)) {
      Gv ns = _near_seg(w, (Gv.F(16.0) / _z()));
      if (Gv.Eq(ns, Gv.Null)) {
        m.G("hud").C("toast", (Gv.S("%s vlož přímo na trubku.") % Gd.Dyn("str", pd.I(Gv.I(1L))).C("substr", Gv.I(2L))));
        return Gv.Null;
      }
      Gv q = ns.G("q").C("round");
      it = it.S("x", q.G("x"));
      it = it.S("y", q.G("y"));
      if (Gv.Eq(typ, Gv.S("pump"))) {
        it = it.S("dir", Gd.posmod((Gd.roundi((ns.G("ang") / Gv.F(90.0))) * Gv.I(90L)), Gv.I(360L)));
      } else {
        it = it.S("rot", Gd.roundi(ns.G("ang")));
      }
      if (Gv.Eq(typ, Gv.S("valve"))) {
        it = it.S("open", Gv.B(false));
      }
    } else if (Gv.Eq(typ, Gv.S("fitting"))) {
      Gv sp = _snap_pipe_pt(w, Gv.B(false));
      Gv c = sp.G("p");
      it = it.S("x", c.G("x"));
      it = it.S("y", c.G("y"));
      Gv np = _near_pt(w, (Gv.F(14.0) / _z()), Gv.B(false));
      if ((Gv.Ne(np, Gv.Null) && (Gv.Eq(np.G("i"), Gv.I(0L)) || Gv.Eq(np.G("i"), (np.G("it").G("pts").C("size") - Gv.I(1L)))))) {
        Gv o = Gd.Ctor("Vector2", np.G("it").G("pts").I((Gv.Eq(np.G("i"), Gv.I(0L)) ? Gv.I(1L) : (np.G("i") - Gv.I(1L)))).I(Gv.I(0L)), np.G("it").G("pts").I((Gv.Eq(np.G("i"), Gv.I(0L)) ? Gv.I(1L) : (np.G("i") - Gv.I(1L)))).I(Gv.I(1L)));
        Gv a0 = Gd.rad_to_deg(((o - c)).C("angle"));
        Gv base_ = (Gd.Dyn("float", it.G("ports").I(Gv.I(0L)))).AsF();
        Gv newp = Gd.Arr();
        foreach (Gv pa in (it.G("ports")).Iter()) {
          newp.C("append", Gd.fposmod(((Gd.Dyn("float", pa) - base_) + a0), Gv.F(360.0)));
        }
        it = it.S("ports", newp);
        Gv cc = (c - (Gd.Static("Vector2", "from_angle", Gd.deg_to_rad(a0)) * Gd.Dyn("float", it.G("arm"))));
        it = it.S("x", Gd.roundf(cc.G("x")));
        it = it.S("y", Gd.roundf(cc.G("y")));
      }
    } else if (Gv.Eq(typ, Gv.S("vessel"))) {
      Gv c2 = _snap((w - (Gd.Ctor("Vector2", Gd.Dyn("float", it.G("w")), Gd.Dyn("float", it.G("h"))) * Gv.F(0.5))));
      it = it.S("x", c2.G("x"));
      it = it.S("y", c2.G("y"));
    } else {
      Gv c3 = _snap(w);
      it = it.S("x", c3.G("x"));
      it = it.S("y", c3.G("y"));
    }
    _snap_undo();
    m.G("items").C("append", it);
    sel = Gd.Dict(Gv.S("t"), Gv.S("item"), Gv.S("it"), it);
    multi.C("clear");
    _changed();
    _props();
    if (Gv.In(typ, Gd.Arr(Gv.S("vessel"), Gv.S("pump")))) {
      m.G("hud").C("toast", (Gv.S("%s přidáno. Vazbu na simulaci nastav v panelu, pak „Uložit a načíst znovu“.") % _name(it)));
    }
    return Gv.Null;
  }
  public Gv _duplicate() {
    Gv list = _sel_items();
    if ((list.C("is_empty")).T()) {
      return Gv.Null;
    }
    _snap_undo();
    Gv out_ = Gd.Arr();
    Gv n = Gv.I(0L);
    foreach (Gv it in (list).Iter()) {
      Gv c = it.C("duplicate", Gv.B(true));
      c.C("erase", Gv.S("_ang"));
      c = c.S("id", (_uid(Gd.Dyn("str", it.G("type"))) + Gd.Dyn("str", n)));
      n = (n + Gv.I(1L));
      if (((c.C("has", Gv.S("bind"))).T() && Gv.Ne(Gd.Dyn("str", c.G("bind")), Gv.S("")))) {
        c = c.S("bind", Gv.S(""));
      }
      m.G("items").C("append", c);
      out_.C("append", c);
    }
    _move_items(out_, Gd.Ctor("Vector2", Gv.I(40L), Gv.I(40L)));
    if (Gv.Eq(out_.C("size"), Gv.I(1L))) {
      sel = Gd.Dict(Gv.S("t"), ((out_.I(Gv.I(0L)).C("has", Gv.S("pts"))).T() ? Gv.S("pipe") : Gv.S("item")), Gv.S("it"), out_.I(Gv.I(0L)));
      multi.C("clear");
    } else {
      sel = Gv.Null;
      multi = out_;
    }
    _changed();
    _props();
    m.G("hud").C("toast", (Gv.S("Zkopírováno %d dílů – přetáhni je na místo.") % out_.C("size")));
    return Gv.Null;
  }
  public Gv _delete() {
    Gv list = _sel_items();
    if ((list.C("is_empty")).T()) {
      return Gv.Null;
    }
    Gv prot = list.C("filter", Gd.Fn((_a) => { Gv it = Gd.A(_a, 0); return _protected(it); return Gv.Null; }));
    if (((prot.C("size")).T() && Gv.Gt((Gd.Static("Time", "get_ticks_msec") - del_arm), Gv.I(3000L)))) {
      del_arm = Gd.Static("Time", "get_ticks_msec");
      m.G("hud").C("toast", (Gv.S("Ve výběru je %s – má roli v simulaci. Opravdu smazat? Stiskni Del (Smazat) znovu.") % _name(prot.I(Gv.I(0L)))));
      return Gv.Null;
    }
    del_arm = Gv.I(0L);
    _snap_undo();
    foreach (Gv it_2 in (list).Iter()) {
      grave = grave.SI(it_2.G("id"), it_2);
      m.G("items").C("erase", it_2);
    }
    sel = Gv.Null;
    multi.C("clear");
    _changed();
    _props();
    m.G("hud").C("toast", (Gv.S("Smazáno %d dílů (Ctrl+Z vrátí).") % list.C("size")));
    return Gv.Null;
  }
  public Gv _rot_item(Gv it = default(Gv), Gv deg = default(Gv)) {
    deg = (deg).AsF();
    {
      Gv _t1 = it.G("type");
      if (Gv.Eq(_t1, Gv.S("valve")) || Gv.Eq(_t1, Gv.S("inl")) || Gv.Eq(_t1, Gv.S("flow")) || Gv.Eq(_t1, Gv.S("hopf"))) {
        it = it.S("rot", Gd.posmod((Gd.Dyn("int", it.C("get", Gv.S("rot"), Gv.I(0L))) + Gd.Dyn("int", deg)), Gv.I(360L)));
      }
      else if (Gv.Eq(_t1, Gv.S("pump")) || Gv.Eq(_t1, Gv.S("nozzle"))) {
        it = it.S("dir", Gd.posmod((Gd.Dyn("int", it.C("get", Gv.S("dir"), Gv.I(0L))) + Gd.Dyn("int", deg)), Gv.I(360L)));
      }
      else if (Gv.Eq(_t1, Gv.S("fitting"))) {
        Gv np = Gd.Arr();
        foreach (Gv pa in (it.G("ports")).Iter()) {
          np.C("append", Gd.fposmod((Gd.Dyn("float", pa) + deg), Gv.F(360.0)));
        }
        it = it.S("ports", np);
      }
      else if (Gv.Eq(_t1, Gv.S("vessel"))) {
        if ((Gv.Eq(deg, Gv.F(90.0)) || Gv.Eq(deg, Gv.F(270.0)))) {
          Gv w0 = Gd.Dyn("float", it.C("get", Gv.S("w"), Gv.I(100L)));
          it = it.S("w", it.C("get", Gv.S("h"), Gv.I(100L)));
          it = it.S("h", w0);
        }
      }
    }
    return Gv.Null;
  }
  public Gv _rotate() {
    if ((multi.C("size")).T()) {
      _rotate_group();
      return Gv.Null;
    }
    if ((Gv.Eq(sel, Gv.Null) || Gv.Ne(sel.G("t"), Gv.S("item")))) {
      return Gv.Null;
    }
    Gv it = sel.G("it");
    _snap_undo();
    _rot_item(it, (Gv.In(it.G("type"), Gd.Arr(Gv.S("valve"), Gv.S("inl"), Gv.S("flow"), Gv.S("pump"))) ? Gv.F(180.0) : Gv.F(90.0)));
    _changed();
    _props();
    return Gv.Null;
  }
  public Gv _rotate_group() {
    Gv list = multi;
    Gv bb = _bbox(list);
    Gv c = bb.C("get_center").C("round");
    _snap_undo();
    Gv rp = Gd.Fn((_a) => { Gv q = Gd.A(_a, 0); return (c + ((q - c)).C("rotated", (Gd.K("PI") * Gv.F(0.5)))); return Gv.Null; });
    foreach (Gv it__it in (list).Iter()) {
      Gv it = it__it;
      if ((it.C("has", Gv.S("pts"))).T()) {
        foreach (Gv i in (it.G("pts").C("size")).Iter()) {
          Gv q_2 = rp.C("call", Gd.Ctor("Vector2", it.G("pts").I(i).I(Gv.I(0L)), it.G("pts").I(i).I(Gv.I(1L)))).C("round");
          it = it.S("pts", it.G("pts").SI(i, Gd.Arr(q_2.G("x"), q_2.G("y"))));
        }
      } else if ((Gv.In(it.G("type"), Gd.Arr(Gv.S("vessel"), Gv.S("panel"))) && (it.C("has", Gv.S("w"))).T())) {
        Gv cen = rp.C("call", _vrect(it).C("get_center"));
        _rot_item(it, Gv.F(90.0));
        it = it.S("x", Gd.roundf((cen.G("x") - (Gd.Dyn("float", it.G("w")) / Gv.F(2.0)))));
        it = it.S("y", Gd.roundf((cen.G("y") - (Gd.Dyn("float", it.G("h")) / Gv.F(2.0)))));
      } else if ((it.C("has", Gv.S("x"))).T()) {
        Gv q2 = rp.C("call", Gd.Ctor("Vector2", it.G("x"), it.G("y"))).C("round");
        it = it.S("x", q2.G("x"));
        it = it.S("y", q2.G("y"));
        _rot_item(it, Gv.F(90.0));
      }
    }
    Gv J = m.G("plan").C("get", Gv.S("joints"), Gd.Arr());
    foreach (Gv i_2 in (J.C("size")).Iter()) {
      if ((bb.C("grow", Gv.F(2.0)).C("has_point", Gd.Ctor("Vector2", J.I(i_2).I(Gv.I(0L)), J.I(i_2).I(Gv.I(1L))))).T()) {
        Gv qj = rp.C("call", Gd.Ctor("Vector2", J.I(i_2).I(Gv.I(0L)), J.I(i_2).I(Gv.I(1L)))).C("round");
        J = J.SI(i_2, Gd.Arr(qj.G("x"), qj.G("y")));
      }
    }
    _changed();
    _props();
    return Gv.Null;
  }
  public Gv _bbox(Gv list = default(Gv)) {
    Gv r = Gv.Null;
    foreach (Gv it in (list).Iter()) {
      Gv ri = Gv.Null;
      if ((it.C("has", Gv.S("pts"))).T()) {
        ri = Gd.Ctor("Rect2", Gd.Ctor("Vector2", it.G("pts").I(Gv.I(0L)).I(Gv.I(0L)), it.G("pts").I(Gv.I(0L)).I(Gv.I(1L))), Gd.StaticGet("Vector2", "ZERO"));
        foreach (Gv q in (it.G("pts")).Iter()) {
          ri = ri.C("expand", Gd.Ctor("Vector2", q.I(Gv.I(0L)), q.I(Gv.I(1L))));
        }
      } else if ((Gv.In(it.G("type"), Gd.Arr(Gv.S("vessel"), Gv.S("panel"))) && (it.C("has", Gv.S("w"))).T())) {
        ri = _vrect(it);
      } else if ((it.C("has", Gv.S("x"))).T()) {
        ri = Gd.Ctor("Rect2", Gd.Ctor("Vector2", it.G("x"), it.G("y")), Gd.StaticGet("Vector2", "ZERO"));
      } else {
        continue;
      }
      r = (Gv.Eq(r, Gv.Null) ? ri : r.C("merge", ri));
    }
    return (Gv.Ne(r, Gv.Null) ? r : Gd.Ctor("Rect2"));
    return Gv.Null;
  }
  public Gv _align(Gv mode = default(Gv)) {
    Gv list = multi.C("filter", Gd.Fn((_a) => { Gv it = Gd.A(_a, 0); return Gv.B((!(it.C("has", Gv.S("pts"))).T() && (it.C("has", Gv.S("x"))).T())); return Gv.Null; }));
    if (Gv.Lt(list.C("size"), Gv.I(2L))) {
      m.G("hud").C("toast", Gv.S("Vyber aspoň dva díly (ne trubky)."));
      return Gv.Null;
    }
    _snap_undo();
    Gv cen = Gd.Fn((_a) => { Gv it_2 = Gd.A(_a, 0); return ((it_2.C("has", Gv.S("w"))).T() ? _vrect(it_2).C("get_center") : Gd.Ctor("Vector2", it_2.G("x"), it_2.G("y"))); return Gv.Null; });
    Gv ref_ = cen.C("call", list.I(Gv.I(0L)));
    {
      Gv _t1 = mode;
      if (Gv.Eq(_t1, Gv.S("row"))) {
        foreach (Gv it_3__it in (list).Iter()) {
          Gv it_3 = it_3__it;
          it_3 = it_3.S("y", ((Gd.Dyn("float", it_3.G("y")) + ref_.G("y")) - cen.C("call", it_3).G("y")));
        }
      }
      else if (Gv.Eq(_t1, Gv.S("col"))) {
        foreach (Gv it_4__it in (list).Iter()) {
          Gv it_4 = it_4__it;
          it_4 = it_4.S("x", ((Gd.Dyn("float", it_4.G("x")) + ref_.G("x")) - cen.C("call", it_4).G("x")));
        }
      }
      else if (Gv.Eq(_t1, Gv.S("bottom"))) {
        Gv by = ((Gd.Dyn("float", list.I(Gv.I(0L)).G("y")) + Gd.Dyn("float", list.I(Gv.I(0L)).C("get", Gv.S("h"), Gv.I(0L))))).AsF();
        foreach (Gv it_5__it in (list).Iter()) {
          Gv it_5 = it_5__it;
          it_5 = it_5.S("y", (by - Gd.Dyn("float", it_5.C("get", Gv.S("h"), Gv.I(0L)))));
        }
      }
      else if (Gv.Eq(_t1, Gv.S("space"))) {
        list.C("sort_custom", Gd.Fn((_a) => { Gv a = Gd.A(_a, 0); Gv b = Gd.A(_a, 1); return Gv.B(Gv.Lt(cen.C("call", a).G("x"), cen.C("call", b).G("x"))); return Gv.Null; }));
        Gv x0 = (cen.C("call", list.I(Gv.I(0L))).G("x")).AsF();
        Gv x1 = (cen.C("call", list.I((list.C("size") - Gv.I(1L)))).G("x")).AsF();
        foreach (Gv i in (list.C("size")).Iter()) {
          it_shift_x(list.I(i), ((x0 + ((((x1 - x0)) * i) / Gd.Dyn("float", (list.C("size") - Gv.I(1L))))) - cen.C("call", list.I(i)).G("x")));
        }
      }
    }
    foreach (Gv it_6__it in (list).Iter()) {
      Gv it_6 = it_6__it;
      it_6 = it_6.S("x", Gd.roundf(Gd.Dyn("float", it_6.G("x"))));
      it_6 = it_6.S("y", Gd.roundf(Gd.Dyn("float", it_6.G("y"))));
    }
    _changed();
    _props();
    return Gv.Null;
  }
  public Gv it_shift_x(Gv it = default(Gv), Gv dx = default(Gv)) {
    dx = (dx).AsF();
    it = it.S("x", (Gd.Dyn("float", it.G("x")) + dx));
    return Gv.Null;
  }
  public Gv _props_soft() {
    if ((Gv.Eq(sel, Gv.Null) || Gv.Ne(sel.G("t"), Gv.S("item")))) {
      return Gv.Null;
    }
    foreach (Gv c in (props.C("find_children", Gv.S("*"), Gv.S("SpinBox"), Gv.B(true), Gv.B(false))).Iter()) {
      Gv k = c.C("get_meta", Gv.S("key"), Gv.S(""));
      if ((Gv.Ne(k, Gv.S("")) && (sel.G("it").C("has", k)).T())) {
        c.C("set_value_no_signal", Gd.Dyn("float", sel.G("it").I(k)));
      }
    }
    return Gv.Null;
  }
  public Gv _props() {
    foreach (Gv c in (props.C("get_children")).Iter()) {
      c.C("queue_free");
    }
    if ((multi.C("size")).T()) {
      Gv hb = _row(props);
      Gv l = _lab(hb, (Gv.S("Vybráno %d dílů:") % multi.C("size")));
      l.C("add_theme_color_override", Gv.S("font_color"), Gd.Ctor("Color", Gv.S("f2c230")));
      _b(hb, Gv.S("⇔ do řady"), Gd.Fn((_a) => _align(Gd.A(_a, 0))).C("bind", Gv.S("row")));
      _b(hb, Gv.S("⇕ pod sebe"), Gd.Fn((_a) => _align(Gd.A(_a, 0))).C("bind", Gv.S("col")));
      _b(hb, Gv.S("▁ dna stejně"), Gd.Fn((_a) => _align(Gd.A(_a, 0))).C("bind", Gv.S("bottom")));
      _b(hb, Gv.S("↔ stejné rozestupy"), Gd.Fn((_a) => _align(Gd.A(_a, 0))).C("bind", Gv.S("space")));
      _b(hb, Gv.S("↻ 90°"), Gd.Fn((_a) => _rotate_group()));
      _b(hb, Gv.S("+ Kopie (Ctrl+D)"), Gd.Fn((_a) => _duplicate()));
      _b(hb, Gv.S("✕ Smazat (Del)"), Gd.Fn((_a) => _delete()));
      return Gv.Null;
    }
    if (Gv.Eq(sel, Gv.Null)) {
      return Gv.Null;
    }
    Gv it = sel.G("it");
    Gv hb2 = _row(props);
    Gv title = Gd.Dict(Gv.S("pipe"), ((it.C("get", Gv.S("hose"), Gv.B(false))).T() ? Gv.S("Hadice") : Gv.S("Trubka")), Gv.S("valve"), Gv.S("Armatura"), Gv.S("inl"), Gv.S("Díl v trubce"), Gv.S("flow"), Gv.S("Rotametr"), Gv.S("fitting"), Gv.S("Tvarovka"), Gv.S("text"), Gv.S("Popisek"), Gv.S("pump"), Gv.S("Čerpadlo"), Gv.S("vessel"), Gv.S("Nádoba"), Gv.S("spray"), Gv.S("Sprcha"), Gv.S("nozzle"), Gv.S("Tryska"), Gv.S("hopf"), Gv.S("Chmelový filtr"), Gv.S("panel"), Gv.S("Rozvaděč"), Gv.S("manifold"), Gv.S("Kateta"), Gv.S("gauge"), Gv.S("Stavoznak")).C("get", it.G("type"), Gd.Dyn("str", it.G("type")));
    Gv l2 = _lab(hb2, (Gv.S("%s · %s") % Gd.Arr(title, Gd.Dyn("str", it.G("id")))));
    l2.C("add_theme_color_override", Gv.S("font_color"), Gd.Ctor("Color", Gv.S("f2c230")));
    if (Gv.In(it.G("type"), Gd.Arr(Gv.S("valve"), Gv.S("pump"), Gv.S("fitting"), Gv.S("inl"), Gv.S("flow"), Gv.S("hopf"), Gv.S("nozzle"), Gv.S("vessel")))) {
      _b(hb2, Gv.S("⟳ Otočit (R)"), Gd.Fn((_a) => _rotate()));
    }
    _b(hb2, Gv.S("+ Kopie (Ctrl+D)"), Gd.Fn((_a) => _duplicate()));
    _b(hb2, Gv.S("✕ Smazat (Del)"), Gd.Fn((_a) => _delete()));
    Gv g = Gd.New("GridContainer");
    g = g.S("columns", Gv.I(4L));
    g.C("add_theme_constant_override", Gv.S("h_separation"), Gv.I(6L));
    props.C("add_child", g);
    _ftext(g, it, Gv.S("label"), Gv.S("popis"));
    {
      Gv _t1 = it.G("type");
      if (Gv.Eq(_t1, Gv.S("pipe"))) {
        _fenum(g, it, Gv.S("dn"), Gv.S("DN"), Gd.Arr(Gd.Arr(Gv.I(10L), Gv.S("DN10")), Gd.Arr(Gv.I(15L), Gv.S("DN15")), Gd.Arr(Gv.I(25L), Gv.S("DN25")), Gd.Arr(Gv.I(40L), Gv.S("DN40")), Gd.Arr(Gv.I(50L), Gv.S("DN50")), Gd.Arr(Gv.I(75L), Gv.S("DN75"))));
        _fenum(g, it, Gv.S("medium"), Gv.S("teče"), MEDIA);
        _fenum(g, it, Gv.S("color"), Gv.S("barva"), COLORS);
        _fbool(g, it, Gv.S("hose"), Gv.S("hadice"));
        _fbool(g, it, Gv.S("solo"), Gv.S("nepropojovat s křížením"));
        _lab(g, Gv.S("délka"));
        _lab(g, (Gv.S("%.2f m") % _plen(it)));
      }
      else if (Gv.Eq(_t1, Gv.S("valve"))) {
        _fenum(g, it, Gv.S("kind"), Gv.S("typ"), VKINDS);
        _fbool(g, it, Gv.S("open"), Gv.S("otevřená"));
        _fnum(g, it, Gv.S("rot"), Gv.S("úhel °"), Gv.I(-360L), Gv.I(360L), Gv.I(1L));
      }
      else if (Gv.Eq(_t1, Gv.S("inl"))) {
        _fenum(g, it, Gv.S("k"), Gv.S("druh"), IKINDS);
        _fnum(g, it, Gv.S("rot"), Gv.S("úhel °"), Gv.I(-360L), Gv.I(360L), Gv.I(1L));
      }
      else if (Gv.Eq(_t1, Gv.S("flow")) || Gv.Eq(_t1, Gv.S("hopf"))) {
        _fnum(g, it, Gv.S("rot"), Gv.S("úhel °"), Gv.I(-360L), Gv.I(360L), Gv.I(1L));
      }
      else if (Gv.Eq(_t1, Gv.S("fitting"))) {
        _fnum(g, it, Gv.S("arm"), Gv.S("rameno"), Gv.I(10L), Gv.I(120L), Gv.I(1L));
        _ftext(g, it, Gv.S("ports"), Gv.S("hrdla ° (čárkou)"));
      }
      else if (Gv.Eq(_t1, Gv.S("text"))) {
        _fnum(g, it, Gv.S("size"), Gv.S("písmo"), Gv.I(8L), Gv.I(80L), Gv.I(1L));
      }
      else if (Gv.Eq(_t1, Gv.S("vessel")) || Gv.Eq(_t1, Gv.S("panel"))) {
        _fnum(g, it, Gv.S("w"), Gv.S("šířka"), Gv.I(10L), Gv.I(3000L), Gv.I(10L));
        _fnum(g, it, Gv.S("h"), Gv.S("výška"), Gv.I(10L), Gv.I(3000L), Gv.I(10L));
        if (Gv.Eq(it.G("type"), Gv.S("vessel"))) {
          _fenum(g, it, Gv.S("shape"), Gv.S("tvar"), SHAPES);
          _fenum(g, it, Gv.S("bind"), Gv.S("v simulaci"), VBINDS);
        }
      }
      else if (Gv.Eq(_t1, Gv.S("pump"))) {
        _fenum(g, it, Gv.S("dir"), Gv.S("směr"), Gd.Arr(Gd.Arr(Gv.I(0L), Gv.S("→")), Gd.Arr(Gv.I(90L), Gv.S("↓")), Gd.Arr(Gv.I(180L), Gv.S("←")), Gd.Arr(Gv.I(270L), Gv.S("↑"))));
        _fenum(g, it, Gv.S("bind"), Gv.S("v simulaci"), PBINDS);
      }
      else if (Gv.Eq(_t1, Gv.S("nozzle"))) {
        _fenum(g, it, Gv.S("dir"), Gv.S("směr"), Gd.Arr(Gd.Arr(Gv.I(0L), Gv.S("→")), Gd.Arr(Gv.I(90L), Gv.S("↓")), Gd.Arr(Gv.I(180L), Gv.S("←")), Gd.Arr(Gv.I(270L), Gv.S("↑"))));
      }
    }
    if (((it.C("has", Gv.S("x"))).T() && !(it.C("has", Gv.S("pts"))).T())) {
      _fnum(g, it, Gv.S("x"), Gv.S("x (cm)"), Gv.I(-100000L), Gv.I(100000L), Gv.I(10L));
      _fnum(g, it, Gv.S("y"), Gv.S("y (cm)"), Gv.I(-100000L), Gv.I(100000L), Gv.I(10L));
    }
    return Gv.Null;
  }
  public Gv _plen(Gv it = default(Gv)) {
    Gv L = Gv.F(0.0);
    foreach (Gv i in ((it.G("pts").C("size") - Gv.I(1L))).Iter()) {
      L = (L + Gd.Ctor("Vector2", it.G("pts").I(i).I(Gv.I(0L)), it.G("pts").I(i).I(Gv.I(1L))).C("distance_to", Gd.Ctor("Vector2", it.G("pts").I((i + Gv.I(1L))).I(Gv.I(0L)), it.G("pts").I((i + Gv.I(1L))).I(Gv.I(1L)))));
    }
    return ((L / PX_PER_M)).AsF();
    return Gv.Null;
  }
  public Gv _set_field(Gv it = default(Gv), Gv k = default(Gv), Gv val = default(Gv)) {
    _snap_undo(((Gd.Dyn("str", it.G("id")) + Gv.S(":")) + k));
    if (((Gv.Eq(Gd.Dyn("typeof", val), Gd.K("TYPE_STRING")) && Gv.Eq(val, Gv.S(""))) && Gv.In(k, Gd.Arr(Gv.S("color"))))) {
      it.C("erase", k);
    } else {
      it = it.SI(k, val);
    }
    if (Gv.Eq(k, Gv.S("bind"))) {
      m.G("hud").C("toast", Gv.S("Vazba na simulaci se plně projeví po „Uložit a načíst znovu“."));
    }
    _changed();
    return Gv.Null;
  }
  public Gv _ftext(Gv g = default(Gv), Gv it = default(Gv), Gv k = default(Gv), Gv title = default(Gv)) {
    _lab(g, title);
    Gv le = Gd.New("LineEdit");
    le = le.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(170L), Gv.I(0L)));
    Gv cur = it.C("get", k, Gv.S(""));
    le = le.S("text", (Gv.Is(cur, "Array") ? Gv.S(",").C("join", Gd.Ctor("PackedStringArray", cur.C("map", Gd.Fn((_a) => { Gv x = Gd.A(_a, 0); return Gd.Dyn("str", Gd.Dyn("int", x)); return Gv.Null; })))) : Gd.Dyn("str", cur)));
    le.G("text_submitted").C("connect", Gd.Fn((_a) => { Gv t = Gd.A(_a, 0); if (Gv.Eq(k, Gv.S("ports"))) {   Gv arr = Gd.Arr();   foreach (Gv s in (t.C("split", Gv.S(","))).Iter()) {     if ((s.C("strip_edges").C("is_valid_float")).T()) {       arr.C("append", Gd.Dyn("float", s));     }   }   if (Gv.Ge(arr.C("size"), Gv.I(2L))) {     _set_field(it, k, arr);   } } else {   _set_field(it, k, t); } le.C("release_focus"); return Gv.Null; }));
    le.G("focus_exited").C("connect", Gd.Fn((_a) => {  if ((Gv.Ne(k, Gv.S("ports")) && Gv.Ne(le.G("text"), Gd.Dyn("str", it.C("get", k, Gv.S("")))))) {   _set_field(it, k, le.G("text")); } return Gv.Null; }));
    g.C("add_child", le);
    return Gv.Null;
  }
  public Gv _fnum(Gv g = default(Gv), Gv it = default(Gv), Gv k = default(Gv), Gv title = default(Gv), Gv mn = default(Gv), Gv mx = default(Gv), Gv step = default(Gv)) {
    mn = (mn).AsF();
    mx = (mx).AsF();
    step = (step).AsF();
    _lab(g, title);
    Gv sp = Gd.New("SpinBox");
    sp = sp.S("min_value", mn);
    sp = sp.S("max_value", mx);
    sp = sp.S("step", step);
    sp = sp.S("value", Gd.Dyn("float", it.C("get", k, Gv.I(0L))));
    sp = sp.S("custom_minimum_size", Gd.Ctor("Vector2", Gv.I(110L), Gv.I(0L)));
    sp.C("set_meta", Gv.S("key"), k);
    sp.G("value_changed").C("connect", Gd.Fn((_a) => { Gv v = Gd.A(_a, 0); _set_field(it, k, v); return Gv.Null; }));
    g.C("add_child", sp);
    return Gv.Null;
  }
  public Gv _fenum(Gv g = default(Gv), Gv it = default(Gv), Gv k = default(Gv), Gv title = default(Gv), Gv opts = default(Gv)) {
    _lab(g, title);
    Gv ob = Gd.New("OptionButton");
    ob = ob.S("focus_mode", Gd.StaticGet("Control", "FOCUS_NONE"));
    Gv cur = it.C("get", k, opts.I(Gv.I(0L)).I(Gv.I(0L)));
    if (Gv.Eq(cur, Gv.Null)) {
      cur = Gv.S("");
    }
    Gv idx = Gv.I(-1L);
    foreach (Gv i in (opts.C("size")).Iter()) {
      ob.C("add_item", Gd.Dyn("str", opts.I(i).I(Gv.I(1L))));
      if ((Gv.Eq(Gd.Dyn("str", opts.I(i).I(Gv.I(0L))), Gd.Dyn("str", cur)) || ((Gv.Eq(Gd.Dyn("typeof", cur), Gd.K("TYPE_FLOAT")) && Gv.Eq(Gd.Dyn("typeof", opts.I(i).I(Gv.I(0L))), Gd.K("TYPE_INT"))) && Gv.Eq(Gd.Dyn("int", cur), opts.I(i).I(Gv.I(0L)))))) {
        idx = i;
      }
    }
    if (Gv.Lt(idx, Gv.I(0L))) {
      ob.C("add_item", (Gv.S("%s (vlastní)") % Gd.Dyn("str", cur)));
      idx = opts.C("size");
    }
    ob.C("select", idx);
    ob.G("item_selected").C("connect", Gd.Fn((_a) => { Gv i_2 = Gd.A(_a, 0); if (Gv.Lt(i_2, opts.C("size"))) {   _set_field(it, k, opts.I(i_2).I(Gv.I(0L))); } return Gv.Null; }));
    g.C("add_child", ob);
    return Gv.Null;
  }
  public Gv _fbool(Gv g = default(Gv), Gv it = default(Gv), Gv k = default(Gv), Gv title = default(Gv)) {
    _lab(g, title);
    Gv cb = Gd.New("CheckBox");
    cb = cb.S("button_pressed", Gd.Dyn("bool", it.C("get", k, Gv.B(false))));
    cb = cb.S("focus_mode", Gd.StaticGet("Control", "FOCUS_NONE"));
    cb.G("toggled").C("connect", Gd.Fn((_a) => { Gv v = Gd.A(_a, 0); _set_field(it, k, v); return Gv.Null; }));
    g.C("add_child", cb);
    return Gv.Null;
  }
  public Gv _process(Gv _d = default(Gv)) {
    _d = (_d).AsF();
    if ((((on).T() && Gv.Ne(m.G("cam"), Gv.Null)) && Gv.Gt(Gd.absf((m.G("cam").G("zoom").G("x") - last_z)), Gv.F(0.0001)))) {
      last_z = m.G("cam").G("zoom").G("x");
      Self.C("queue_redraw");
    }
    return Gv.Null;
  }
  public Gv _draw() {
    if (!(on).T()) {
      return Gv.Null;
    }
    Gv z = _z();
    Gv hs = (Gv.F(4.0) / z);
    if (Gv.Gt(z, Gv.F(0.15))) {
      foreach (Gv p in (_pipes(Gv.B(true))).Iter()) {
        foreach (Gv q in (p.G("pts")).Iter()) {
          Self.C("draw_rect", Gd.Ctor("Rect2", (Gd.Ctor("Vector2", q.I(Gv.I(0L)), q.I(Gv.I(1L))) - Gd.Ctor("Vector2", hs, hs)), (Gd.Ctor("Vector2", hs, hs) * Gv.F(2.0))), Gd.Ctor("Color", Gv.F(0.35), Gv.F(0.75), Gv.F(1.0), Gv.F(0.8)), Gv.B(false), (Gv.F(1.2) / z));
        }
      }
      foreach (Gv q_2 in (_ports()).Iter()) {
        Self.C("draw_arc", q_2, (Gv.F(6.0) / z), Gv.I(0L), Gd.K("TAU"), Gv.I(16L), Gd.Ctor("Color", Gv.F(0.3), Gv.F(1.0), Gv.F(0.6), Gv.F(0.7)), (Gv.F(1.5) / z));
      }
      foreach (Gv j in (m.G("plan").C("get", Gv.S("joints"), Gd.Arr())).Iter()) {
        Self.C("draw_circle", Gd.Ctor("Vector2", j.I(Gv.I(0L)), j.I(Gv.I(1L))), (Gv.F(5.0) / z), Gd.Ctor("Color", Gv.I(1L), Gv.F(0.8), Gv.F(0.2), Gv.F(0.85)));
      }
    }
    Gv col = Gd.Ctor("Color", Gv.S("f2c230"));
    foreach (Gv it in (multi).Iter()) {
      _draw_sel(it, Gd.Ctor("Color", Gv.F(0.3), Gv.F(0.8), Gv.F(1.0)), z);
    }
    if (Gv.Ne(sel, Gv.Null)) {
      if (Gv.In(sel.G("t"), Gd.Arr(Gv.S("pipe"), Gv.S("pt")))) {
        _draw_sel(sel.G("it"), col, z);
        if (Gv.Eq(sel.G("t"), Gv.S("pt"))) {
          Gv pp = Gd.Ctor("Vector2", sel.G("it").G("pts").I(sel.G("i")).I(Gv.I(0L)), sel.G("it").G("pts").I(sel.G("i")).I(Gv.I(1L)));
          Self.C("draw_circle", pp, (Gv.F(8.0) / z), col);
        }
      } else {
        _draw_sel(sel.G("it"), col, z);
        if ((Gv.In(sel.G("it").G("type"), Gd.Arr(Gv.S("vessel"), Gv.S("panel"))) && (sel.G("it").C("has", Gv.S("w"))).T())) {
          Gv e = _vrect(sel.G("it")).G("end");
          Self.C("draw_rect", Gd.Ctor("Rect2", (e - (Gd.Ctor("Vector2", Gv.I(7L), Gv.I(7L)) / z)), (Gd.Ctor("Vector2", Gv.I(14L), Gv.I(14L)) / z)), col);
        }
      }
    }
    foreach (Gv pl in (route_hl).Iter()) {
      Self.C("draw_polyline", pl, Gd.Ctor("Color", Gv.I(1L), Gv.F(0.85), Gv.F(0.1), Gv.F(0.45)), Gv.F(22.0), Gv.B(true));
      Self.C("draw_polyline", pl, Gd.Ctor("Color", Gv.I(1L), Gv.F(0.9), Gv.F(0.2)), Gv.F(4.0), Gv.B(true));
    }
    if (((drag).T() && Gv.Eq(drag_mode, Gv.S("box")))) {
      Gv r = Gd.Ctor("Rect2", box_a, Gd.StaticGet("Vector2", "ZERO")).C("expand", hover_w);
      Self.C("draw_rect", r, Gd.Ctor("Color", Gv.F(0.3), Gv.F(0.7), Gv.F(1.0), Gv.F(0.12)));
      Self.C("draw_rect", r, Gd.Ctor("Color", Gv.F(0.4), Gv.F(0.8), Gv.F(1.0), Gv.F(0.9)), Gv.B(false), (Gv.F(1.5) / z));
    }
    Gv cross = Gd.Ctor("Color", Gv.S("4fd18b"));
    {
      Gv _t1 = tool;
      if (Gv.Eq(_t1, Gv.S("pipe"))) {
        Gv pl2 = Gd.Ctor("PackedVector2Array");
        foreach (Gv d in (draw_pts).Iter()) {
          pl2.C("append", d.G("p"));
        }
        Gv sp = _snap_pipe_pt(hover_w, Gd.Static("Input", "is_key_pressed", Gd.K("KEY_ALT")));
        Gv hp = sp.G("p");
        if ((((draw_pts.C("size")).T() && Gv.Eq(sp.G("kind"), Gv.S("free"))) && !(Gd.Static("Input", "is_key_pressed", Gd.K("KEY_ALT"))).T())) {
          Gv last = draw_pts.I((draw_pts.C("size") - Gv.I(1L))).G("p");
          if (Gv.Gt(Gd.absf((hp.G("x") - last.G("x"))), Gd.absf((hp.G("y") - last.G("y"))))) {
            hp = hp.S("y", last.G("y"));
          } else {
            hp = hp.S("x", last.G("x"));
          }
        }
        pl2.C("append", hp);
        if (Gv.Ge(pl2.C("size"), Gv.I(2L))) {
          Self.C("draw_polyline", pl2, Gd.Ctor("Color", Gv.F(0.4), Gv.F(0.8), Gv.F(1.0), Gv.F(0.35)), (((Gv.Le(dn_new, Gv.I(32L)) ? Gd.clampf((dn_new * Gv.F(0.24)), Gv.F(3.0), Gv.F(6.0)) : ((Gv.F(12.0) * dn_new) / Gv.F(50.0)))) + (Gv.F(6.0) / z)), Gv.B(true));
          Self.C("draw_polyline", pl2, Gd.Ctor("Color", Gv.F(0.6), Gv.F(0.9), Gv.F(1.0)), (Gv.F(2.0) / z), Gv.B(true));
        }
        foreach (Gv d_2 in (draw_pts).Iter()) {
          Self.C("draw_circle", d_2.G("p"), (Gv.F(5.0) / z), Gd.Ctor("Color", Gv.F(0.6), Gv.F(0.9), Gv.F(1.0)));
        }
        _crosshair(hp, (Gv.Ne(sp.G("kind"), Gv.S("free")) ? cross : Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.9))), z);
      }
      else if (Gv.Eq(_t1, Gv.S("hose")) || Gv.Eq(_t1, Gv.S("pitch"))) {
        Gv sp2 = _snap_pipe_pt(hover_w, Gv.B(false));
        Gv a = (Gv.Eq(tool, Gv.S("pitch")) ? pitch_a : (((draw_pts.C("size")).T() ? draw_pts.I(Gv.I(0L)).G("p") : Gv.Null)));
        if (Gv.Ne(a, Gv.Null)) {
          Gv hp2 = _hose_pts(a, sp2.G("p"));
          Gv pl3 = Gd.Ctor("PackedVector2Array");
          foreach (Gv q_3 in (hp2).Iter()) {
            pl3.C("append", Gd.Ctor("Vector2", q_3.I(Gv.I(0L)), q_3.I(Gv.I(1L))));
          }
          Self.C("draw_polyline", pl3, Gd.Ctor("Color", Gv.F(0.85), Gv.F(0.63), Gv.F(0.23), Gv.F(0.8)), Gv.F(8.0), Gv.B(true));
        }
        _crosshair(sp2.G("p"), (Gv.Ne(sp2.G("kind"), Gv.S("free")) ? cross : Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.9))), z);
      }
      else if (Gv.Eq(_t1, Gv.S("measure"))) {
        Gv p3 = _snap_pipe_pt(hover_w, Gv.B(false)).G("p");
        if (Gv.Ne(meas_a, Gv.Null)) {
          Self.C("draw_line", meas_a, p3, Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.9)), (Gv.F(2.0) / z));
          Self.C("draw_string", m.G("font_b"), (p3 + (Gd.Ctor("Vector2", Gv.I(12L), Gv.I(-12L)) / z)), (Gv.S("%.2f m") % ((p3.C("distance_to", meas_a) / PX_PER_M))), Gd.K("HORIZONTAL_ALIGNMENT_LEFT"), Gv.I(-1L), Gd.Dyn("int", (Gv.F(16.0) / z)), Gd.StaticGet("Color", "WHITE"));
        }
        _crosshair(p3, Gd.StaticGet("Color", "WHITE"), z);
      }
      else if (Gv.Eq(_t1, Gv.S("route"))) {
        if (Gv.Ne(route_a, Gv.Null)) {
          Self.C("draw_rect", _vrect(route_a).C("grow", (Gv.F(8.0) / z)), Gd.Ctor("Color", Gv.I(1L), Gv.F(0.85), Gv.F(0.1)), Gv.B(false), (Gv.F(3.0) / z));
        }
        Gv hv = _vessel_at(hover_w);
        if (Gv.Ne(hv, Gv.Null)) {
          Self.C("draw_rect", _vrect(hv).C("grow", (Gv.F(4.0) / z)), Gd.Ctor("Color", Gv.I(1L), Gv.F(0.85), Gv.F(0.1), Gv.F(0.5)), Gv.B(false), (Gv.F(2.0) / z));
        }
      }
      else if (Gv.Eq(_t1, Gv.S("paint"))) {
        Gv ns2 = _near_seg(hover_w, (Gv.F(12.0) / z), Gv.B(true));
        if (Gv.Ne(ns2, Gv.Null)) {
          Gv plc = Gd.Ctor("PackedVector2Array");
          foreach (Gv q_4 in (ns2.G("it").G("pts")).Iter()) {
            plc.C("append", Gd.Ctor("Vector2", q_4.I(Gv.I(0L)), q_4.I(Gv.I(1L))));
          }
          Self.C("draw_polyline", plc, (Gv.Ne(paint_col, Gv.S("")) ? Gd.Ctor("Color", paint_col) : Gd.Ctor("Color", Gv.I(1L), Gv.I(1L), Gv.I(1L), Gv.F(0.5))), (Gv.F(6.0) / z), Gv.B(true));
        }
      }
      else if (Gv.Eq(_t1, Gv.S("joint"))) {
        _crosshair(hover_w, Gd.Ctor("Color", Gv.S("f2c230")), z);
      }
      else if (Gv.Eq(_t1, Gv.S("part"))) {
        Gv pd = _part_def(part_key);
        if (Gv.In(pd.I(Gv.I(2L)), ON_PIPE)) {
          Gv ns = _near_seg(hover_w, (Gv.F(16.0) / z));
          if (Gv.Ne(ns, Gv.Null)) {
            Self.C("draw_circle", ns.G("q"), (Gv.F(12.0) / z), Gd.Ctor("Color", Gv.F(0.3), Gv.F(1.0), Gv.F(0.5), Gv.F(0.6)));
          } else {
            _crosshair(hover_w, Gd.Ctor("Color", Gv.I(1L), Gv.F(0.4), Gv.F(0.3), Gv.F(0.9)), z);
          }
        } else if (Gv.Eq(pd.I(Gv.I(2L)), Gv.S("vessel"))) {
          Gv dd = pd.I(Gv.I(3L));
          Gv vw = Gd.Dyn("float", dd.G("w"));
          Gv vh = Gd.Dyn("float", dd.G("h"));
          Self.C("draw_rect", Gd.Ctor("Rect2", _snap((hover_w - (Gd.Ctor("Vector2", vw, vh) * Gv.F(0.5)))), Gd.Ctor("Vector2", vw, vh)), Gd.Ctor("Color", Gv.F(0.3), Gv.F(1.0), Gv.F(0.6), Gv.F(0.6)), Gv.B(false), (Gv.F(2.0) / z));
        } else {
          _crosshair(_snap_pipe_pt(hover_w, Gv.B(false)).G("p"), cross, z);
        }
      }
    }
    return Gv.Null;
  }
  public Gv _draw_sel(Gv it = default(Gv), Gv col = default(Gv), Gv z = default(Gv)) {
    z = (z).AsF();
    if ((it.C("has", Gv.S("pts"))).T()) {
      Gv pl = Gd.Ctor("PackedVector2Array");
      foreach (Gv q in (it.G("pts")).Iter()) {
        pl.C("append", Gd.Ctor("Vector2", q.I(Gv.I(0L)), q.I(Gv.I(1L))));
      }
      if (Gv.Ge(pl.C("size"), Gv.I(2L))) {
        Self.C("draw_polyline", pl, Gd.Ctor("Color", col, Gv.F(0.35)), (Gv.F(18.0) / z), Gv.B(true));
        Self.C("draw_polyline", pl, col, (Gv.F(2.0) / z), Gv.B(true));
      }
    } else if ((Gv.In(it.G("type"), Gd.Arr(Gv.S("vessel"), Gv.S("panel"))) && (it.C("has", Gv.S("w"))).T())) {
      Self.C("draw_rect", _vrect(it).C("grow", (Gv.F(6.0) / z)), col, Gv.B(false), (Gv.F(3.0) / z));
    } else if (Gv.Eq(it.G("type"), Gv.S("text"))) {
      Self.C("draw_rect", Gd.Ctor("Rect2", (Gd.Dyn("float", it.G("x")) - Gv.I(4L)), (Gd.Dyn("float", it.G("y")) - Gv.I(18L)), ((Gv.F(8.0) * Gd.Dyn("str", it.C("get", Gv.S("label"), Gv.S(""))).C("length")) + Gv.I(8L)), Gv.I(24L)), col, Gv.B(false), (Gv.F(2.0) / z));
    } else if ((it.C("has", Gv.S("x"))).T()) {
      Self.C("draw_arc", Gd.Ctor("Vector2", it.G("x"), it.G("y")), (Gv.F(26.0) / z), Gv.I(0L), Gd.K("TAU"), Gv.I(32L), col, (Gv.F(3.0) / z), Gv.B(true));
    }
    return Gv.Null;
  }
  public Gv _crosshair(Gv p = default(Gv), Gv col = default(Gv), Gv z = default(Gv)) {
    z = (z).AsF();
    Gv r = (Gv.F(12.0) / z);
    Self.C("draw_circle", p, (Gv.F(6.0) / z), col);
    Self.C("draw_line", (p - Gd.Ctor("Vector2", (r * Gv.F(2.0)), Gv.I(0L))), (p + Gd.Ctor("Vector2", (r * Gv.F(2.0)), Gv.I(0L))), Gd.Ctor("Color", col, Gv.F(0.6)), (Gv.F(1.5) / z));
    Self.C("draw_line", (p - Gd.Ctor("Vector2", Gv.I(0L), (r * Gv.F(2.0)))), (p + Gd.Ctor("Vector2", Gv.I(0L), (r * Gv.F(2.0)))), Gd.Ctor("Color", col, Gv.F(0.6)), (Gv.F(1.5) / z));
    return Gv.Null;
  }
  public Gv selftest() {
    Gv ok = Gv.B(true);
    if (Gv.Ne(m.G("cam"), Gv.Null)) {
      m = m.S("cam", m.G("cam").S("zoom", Gd.StaticGet("Vector2", "ONE")));
    }
    Gv n0 = (m.G("items").C("size")).AsI();
    Gv e0 = (m.G("sim").G("net").G("edges").C("size")).AsI();
    Gv j0 = (m.G("plan").C("get", Gv.S("joints"), Gd.Arr()).C("size")).AsI();
    Gv ps = _pipes();
    Gv p0 = ps.I(Gv.I(0L));
    Gv a = Gd.Ctor("Vector2", p0.G("pts").I(Gv.I(0L)).I(Gv.I(0L)), p0.G("pts").I(Gv.I(0L)).I(Gv.I(1L)));
    draw_pts = Gd.Arr(Gd.Dict(Gv.S("p"), a, Gv.S("kind"), Gv.S("pt")), Gd.Dict(Gv.S("p"), (a + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(200L))), Gv.S("kind"), Gv.S("free")), Gd.Dict(Gv.S("p"), (a + Gd.Ctor("Vector2", Gv.I(300L), Gv.I(200L))), Gv.S("kind"), Gv.S("free")));
    _finish_pipe();
    Gv e1 = (m.G("sim").G("net").G("edges").C("size")).AsI();
    Gd.print((Gv.S("EDTEST trubka: položek %d→%d, hran %d→%d") % Gd.Arr(n0, m.G("items").C("size"), e0, e1)));
    ok = Gv.B((((ok).T() && Gv.Eq(m.G("items").C("size"), (n0 + Gv.I(1L)))) && Gv.Gt(e1, e0)));
    part_key = Gv.S("klapka");
    _add_part((a + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(100L))));
    Gv v = m.G("items").I((m.G("items").C("size") - Gv.I(1L)));
    Gv onnet = m.G("sim").G("net").G("byId").C("has", v.G("id"));
    Gd.print((Gv.S("EDTEST klapka na síti: %s, rot %d") % Gd.Arr(onnet, Gd.Dyn("int", v.G("rot")))));
    ok = Gv.B(((ok).T() && (onnet).T()));
    sel = Gd.Dict(Gv.S("t"), Gv.S("item"), Gv.S("it"), v);
    _rotate();
    Gd.print((Gv.S("EDTEST otočení: rot %d") % Gd.Dyn("int", v.G("rot"))));
    foreach (Gv k in (Gd.Arr(Gv.S("pruh"), Gv.S("flow"), Gv.S("fitT"), Gv.S("text"), Gv.S("tank"), Gv.S("spray"), Gv.S("hopf"))).Iter()) {
      part_key = k;
      Gv nb = (m.G("items").C("size")).AsI();
      _add_part((a + Gd.Ctor("Vector2", Gv.I(150L), Gv.I(200L))));
      Gd.print((Gv.S("EDTEST díl %s: %s") % Gd.Arr(k, (Gv.Eq(m.G("items").C("size"), (nb + Gv.I(1L))) ? Gv.S("OK") : Gv.S("CHYBA")))));
      ok = Gv.B(((ok).T() && Gv.Eq(m.G("items").C("size"), (nb + Gv.I(1L)))));
    }
    part_key = Gv.S("pump");
    Gv npb = (m.G("items").C("size")).AsI();
    _add_part((a + Gd.Ctor("Vector2", Gv.I(150L), Gv.I(200L))));
    Gd.print((Gv.S("EDTEST čerpadlo na trubce: %s") % ((Gv.Eq(m.G("items").C("size"), (npb + Gv.I(1L))) ? Gv.S("OK") : Gv.S("CHYBA")))));
    ok = Gv.B(((ok).T() && Gv.Eq(m.G("items").C("size"), (npb + Gv.I(1L)))));
    Gv tk = Gv.Null;
    foreach (Gv it in (m.G("items")).Iter()) {
      if ((Gv.Eq(it.G("type"), Gv.S("vessel")) && Gv.Eq(Gd.Dyn("str", it.C("get", Gv.S("label"), Gv.S(""))), Gv.S("nádoba")))) {
        tk = it;
      }
    }
    sel = Gd.Dict(Gv.S("t"), Gv.S("item"), Gv.S("it"), tk);
    _set_field(tk, Gv.S("w"), Gv.F(300.0));
    _set_field(tk, Gv.S("shape"), Gv.S("cone"));
    Gd.print((Gv.S("EDTEST vlastnosti nádoby: w %d tvar %s") % Gd.Arr(Gd.Dyn("int", tk.G("w")), tk.G("shape"))));
    ok = Gv.B((((ok).T() && Gv.Eq(Gd.Dyn("int", tk.G("w")), Gv.I(300L))) && Gv.Eq(tk.G("shape"), Gv.S("cone"))));
    Gv nh = (m.G("items").C("size")).AsI();
    _pitch_click((a + Gd.Ctor("Vector2", Gv.I(600L), Gv.I(0L))));
    _pitch_click((a + Gd.Ctor("Vector2", Gv.I(1100L), Gv.I(300L))));
    Gd.print((Gv.S("EDTEST spílací hadice: +%d dílů (má být 7)") % ((m.G("items").C("size") - nh))));
    ok = Gv.B(((ok).T() && Gv.Eq((m.G("items").C("size") - nh), Gv.I(7L))));
    multi = m.G("items").C("slice", nh, (nh + Gv.I(7L)));
    sel = Gv.Null;
    Gv bb0 = _bbox(multi);
    _move_items(multi, Gd.Ctor("Vector2", Gv.I(100L), Gv.I(0L)));
    _rotate_group();
    Gd.print((Gv.S("EDTEST skupina: posun + otočení OK, rámeček %s") % Gd.Dyn("str", _bbox(multi).G("size"))));
    Gv nd = (m.G("items").C("size")).AsI();
    _duplicate();
    Gd.print((Gv.S("EDTEST duplikace skupiny: +%d (má být 7)") % ((m.G("items").C("size") - nd))));
    ok = Gv.B(((ok).T() && Gv.Eq(m.G("items").C("size"), (nd + Gv.I(7L)))));
    _delete();
    Gd.print((Gv.S("EDTEST smazání skupiny: %d (má být %d)") % Gd.Arr(m.G("items").C("size"), nd)));
    ok = Gv.B(((ok).T() && Gv.Eq(m.G("items").C("size"), nd)));
    draw_pts = Gd.Arr(Gd.Dict(Gv.S("p"), (a + Gd.Ctor("Vector2", Gv.I(-100L), Gv.I(100L))), Gv.S("kind"), Gv.S("free")), Gd.Dict(Gv.S("p"), (a + Gd.Ctor("Vector2", Gv.I(100L), Gv.I(100L))), Gv.S("kind"), Gv.S("free")));
    _finish_pipe();
    _joint_click((a + Gd.Ctor("Vector2", Gv.I(0L), Gv.I(100L))));
    Gd.print((Gv.S("EDTEST spoj na křížení: spojů %d → %d") % Gd.Arr(j0, m.G("plan").G("joints").C("size"))));
    ok = Gv.B(((ok).T() && Gv.Eq(m.G("plan").G("joints").C("size"), (j0 + Gv.I(1L)))));
    Gv rvp = Gv.Null;
    foreach (Gv it_2 in (m.G("items")).Iter()) {
      if (Gv.Eq(it_2.C("get", Gv.S("bind"), Gv.S("")), Gv.S("rvp"))) {
        rvp = it_2;
      }
    }
    Gv nr = (m.G("items").C("size")).AsI();
    sel = Gd.Dict(Gv.S("t"), Gv.S("item"), Gv.S("it"), rvp);
    multi.C("clear");
    _delete();
    Gv warned = Gv.B(Gv.Eq(m.G("items").C("size"), nr));
    _delete();
    Gv gone = Gv.B(!(m.G("items").C("has", rvp)).T());
    undo();
    Gd.print((Gv.S("EDTEST mazání pánve: varování %s, smazáno %s, zpět vráceno %s") % Gd.Arr(warned, gone, m.G("items").C("has", rvp))));
    ok = Gv.B(((((ok).T() && (warned).T()) && (gone).T()) && (m.G("items").C("has", rvp)).T()));
    _route_click(_vrect(rvp).C("get_center"));
    Gv sk = Gv.Null;
    foreach (Gv it_3 in (m.G("items")).Iter()) {
      if (Gv.Eq(it_3.C("get", Gv.S("bind"), Gv.S("")), Gv.S("sk"))) {
        sk = it_3;
      }
    }
    _route_click(_vrect(sk).C("get_center"));
    Gd.print((Gv.S("EDTEST cesta pánev → SK: %d úseků") % route_hl.C("size")));
    ok = Gv.B(((ok).T() && Gv.Gt(route_hl.C("size"), Gv.I(0L))));
    sel = Gv.Null;
    multi.C("clear");
    while ((undo_stack.C("size")).T()) {
      undo();
    }
    Gd.print((Gv.S("EDTEST zpět: položek %d (má být %d), hran %d (má být %d)") % Gd.Arr(m.G("items").C("size"), n0, m.G("sim").G("net").G("edges").C("size"), e0)));
    ok = Gv.B((((ok).T() && Gv.Eq(m.G("items").C("size"), n0)) && Gv.Eq(m.G("sim").G("net").G("edges").C("size"), e0)));
    Gd.print((Gv.S("EDTEST ") + (((ok).T() ? Gv.S("OK") : Gv.S("CHYBA")))));
    return Gv.Null;
  }
  public Gv clicktest() {
    m = m.S("cam", m.G("cam").S("zoom", Gd.StaticGet("Vector2", "ONE")));
    m = m.S("cam", m.G("cam").S("position", Gd.Ctor("Vector2", Gv.I(700L), Gv.I(900L))));
    _ = Self.C("get_tree").G("process_frame");
    Gv vp = m.C("vp_size");
    Gv to_s = Gd.Fn((_a) => { Gv w = Gd.A(_a, 0); return ((((w - m.G("cam").C("get_screen_center_position"))) * m.G("cam").G("zoom").G("x")) + (vp * Gv.F(0.5))); return Gv.Null; });
    Gv n0 = (m.G("items").C("size")).AsI();
    _set_tool(Gv.S("pipe"));
    foreach (Gv w_2 in (Gd.Arr(Gd.Ctor("Vector2", Gv.I(420L), Gv.I(1060L)), Gd.Ctor("Vector2", Gv.I(420L), Gv.I(1160L)), Gd.Ctor("Vector2", Gv.I(620L), Gv.I(1160L)))).Iter()) {
      foreach (Gv pr in (Gd.Arr(Gv.B(true), Gv.B(false))).Iter()) {
        Gv ev = Gd.New("InputEventMouseButton");
        ev = ev.S("button_index", Gd.K("MOUSE_BUTTON_LEFT"));
        ev = ev.S("pressed", pr);
        ev = ev.S("position", to_s.C("call", w_2));
        ev = ev.S("global_position", ev.G("position"));
        Self.C("get_viewport").C("push_input", ev);
        _ = Self.C("get_tree").G("process_frame");
      }
    }
    Gv ev2 = Gd.New("InputEventMouseButton");
    ev2 = ev2.S("button_index", Gd.K("MOUSE_BUTTON_RIGHT"));
    ev2 = ev2.S("pressed", Gv.B(true));
    ev2 = ev2.S("position", to_s.C("call", Gd.Ctor("Vector2", Gv.I(620L), Gv.I(1160L))));
    Self.C("get_viewport").C("push_input", ev2);
    _ = Self.C("get_tree").G("process_frame");
    Gv last = m.G("items").I((m.G("items").C("size") - Gv.I(1L)));
    Gd.print((Gv.S("EDCLICK trubka myší: položek %d→%d, body %s") % Gd.Arr(n0, m.G("items").C("size"), Gd.Dyn("str", last.C("get", Gv.S("pts"), Gd.Arr())))));
    Gd.print((Gv.S("EDCLICK ") + (((Gv.Eq(m.G("items").C("size"), (n0 + Gv.I(1L))) && Gv.Eq(last.G("type"), Gv.S("pipe"))) ? Gv.S("OK") : Gv.S("CHYBA")))));
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "GRID": return S_editor.GRID;
    case "PX_PER_M": return S_editor.PX_PER_M;
    case "PARTS": return S_editor.PARTS;
    case "MEDIA": return S_editor.MEDIA;
    case "COLORS": return S_editor.COLORS;
    case "ON_PIPE": return S_editor.ON_PIPE;
    case "PROTECT": return S_editor.PROTECT;
    case "VBINDS": return S_editor.VBINDS;
    case "PBINDS": return S_editor.PBINDS;
    case "SHAPES": return S_editor.SHAPES;
    case "VKINDS": return S_editor.VKINDS;
    case "IKINDS": return S_editor.IKINDS;
    case "m": return this.m;
    case "on": return this.on;
    case "tool": return this.tool;
    case "part_key": return this.part_key;
    case "sel": return this.sel;
    case "multi": return this.multi;
    case "drag": return this.drag;
    case "drag_mode": return this.drag_mode;
    case "drag_off": return this.drag_off;
    case "drag_start": return this.drag_start;
    case "drag_orig": return this.drag_orig;
    case "box_a": return this.box_a;
    case "draw_pts": return this.draw_pts;
    case "pitch_a": return this.pitch_a;
    case "meas_a": return this.meas_a;
    case "route_a": return this.route_a;
    case "route_hl": return this.route_hl;
    case "paint_col": return this.paint_col;
    case "hover_w": return this.hover_w;
    case "undo_stack": return this.undo_stack;
    case "undo_key": return this.undo_key;
    case "undo_t": return this.undo_t;
    case "grave": return this.grave;
    case "del_arm": return this.del_arm;
    case "dirty": return this.dirty;
    case "ui": return this.ui;
    case "info": return this.info;
    case "props": return this.props;
    case "live_s": return this.live_s;
    case "live_i": return this.live_i;
    case "btns": return this.btns;
    case "dn_new": return this.dn_new;
    case "med_new": return this.med_new;
    case "fdlg": return this.fdlg;
    case "dock_was": return this.dock_was;
    case "TOOLS": return S_editor.TOOLS;
    case "last_z": return this.last_z;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "m": this.m = v; return v;
    case "on": this.on = v; return v;
    case "tool": this.tool = v; return v;
    case "part_key": this.part_key = v; return v;
    case "sel": this.sel = v; return v;
    case "multi": this.multi = v; return v;
    case "drag": this.drag = v; return v;
    case "drag_mode": this.drag_mode = v; return v;
    case "drag_off": this.drag_off = v; return v;
    case "drag_start": this.drag_start = v; return v;
    case "drag_orig": this.drag_orig = v; return v;
    case "box_a": this.box_a = v; return v;
    case "draw_pts": this.draw_pts = v; return v;
    case "pitch_a": this.pitch_a = v; return v;
    case "meas_a": this.meas_a = v; return v;
    case "route_a": this.route_a = v; return v;
    case "route_hl": this.route_hl = v; return v;
    case "paint_col": this.paint_col = v; return v;
    case "hover_w": this.hover_w = v; return v;
    case "undo_stack": this.undo_stack = v; return v;
    case "undo_key": this.undo_key = v; return v;
    case "undo_t": this.undo_t = v; return v;
    case "grave": this.grave = v; return v;
    case "del_arm": this.del_arm = v; return v;
    case "dirty": this.dirty = v; return v;
    case "ui": this.ui = v; return v;
    case "info": this.info = v; return v;
    case "props": this.props = v; return v;
    case "live_s": this.live_s = v; return v;
    case "live_i": this.live_i = v; return v;
    case "btns": this.btns = v; return v;
    case "dn_new": this.dn_new = v; return v;
    case "med_new": this.med_new = v; return v;
    case "fdlg": this.fdlg = v; return v;
    case "dock_was": this.dock_was = v; return v;
    case "last_z": this.last_z = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "setup": return setup();
    case "_part_btn": return _part_btn(Gd.A(a, 0), Gd.A(a, 1));
    case "_row": return _row(Gd.A(a, 0));
    case "_lab": return _lab(Gd.A(a, 0), Gd.A(a, 1));
    case "_b": return _b(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "_set_tool": return _set_tool(Gd.A(a, 0));
    case "_set_part": return _set_part(Gd.A(a, 0));
    case "_set_dn": return _set_dn(Gd.A(a, 0));
    case "_set_med": return _set_med(Gd.A(a, 0));
    case "_set_paint": return _set_paint(Gd.A(a, 0));
    case "_part_def": return _part_def(Gd.A(a, 0));
    case "_hint": return _hint();
    case "toggle": return toggle();
    case "_snap_undo": return _snap_undo(Gd.A(a, 0));
    case "undo": return undo();
    case "_restore": return _restore(Gd.A(a, 0), Gd.A(a, 1));
    case "_changed": return _changed();
    case "_live_redraw": return _live_redraw();
    case "_clean_items": return _clean_items();
    case "_clean_plan": return _clean_plan();
    case "save": return save();
    case "save_reload": return save_reload();
    case "_export": return _export();
    case "_import": return _import();
    case "_dialog": return _dialog(Gd.A(a, 0), Gd.A(a, 1));
    case "reset_plan": return reset_plan();
    case "_z": return _z();
    case "_snap": return _snap(Gd.A(a, 0));
    case "_uid": return _uid(Gd.A(a, 0));
    case "_protected": return _protected(Gd.A(a, 0));
    case "_is_pipe": return _is_pipe(Gd.A(a, 0));
    case "_name": return _name(Gd.A(a, 0));
    case "_pipes": return _pipes(Gd.A(a, 0));
    case "_near_pt": return _near_pt(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "_near_seg": return _near_seg(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "_vrect": return _vrect(Gd.A(a, 0));
    case "_near_item": return _near_item(Gd.A(a, 0), Gd.A(a, 1));
    case "_ports": return _ports();
    case "_snap_pipe_pt": return _snap_pipe_pt(Gd.A(a, 0), Gd.A(a, 1));
    case "_sel_items": return _sel_items();
    case "handle": return handle(Gd.A(a, 0));
    case "_press_select": return _press_select(Gd.A(a, 0), Gd.A(a, 1));
    case "_orig": return _orig(Gd.A(a, 0));
    case "_move_items": return _move_items(Gd.A(a, 0), Gd.A(a, 1));
    case "_drag_to": return _drag_to(Gd.A(a, 0), Gd.A(a, 1));
    case "_release": return _release();
    case "_select_visible": return _select_visible();
    case "_insert_point": return _insert_point(Gd.A(a, 0));
    case "_pipe_click": return _pipe_click(Gd.A(a, 0), Gd.A(a, 1));
    case "_finish_pipe": return _finish_pipe();
    case "_hose_pts": return _hose_pts(Gd.A(a, 0), Gd.A(a, 1));
    case "_hose_click": return _hose_click(Gd.A(a, 0));
    case "_pitch_click": return _pitch_click(Gd.A(a, 0));
    case "_joint_click": return _joint_click(Gd.A(a, 0));
    case "_measure_click": return _measure_click(Gd.A(a, 0));
    case "_vessel_at": return _vessel_at(Gd.A(a, 0));
    case "_route_click": return _route_click(Gd.A(a, 0));
    case "_paint_click": return _paint_click(Gd.A(a, 0));
    case "_add_part": return _add_part(Gd.A(a, 0));
    case "_duplicate": return _duplicate();
    case "_delete": return _delete();
    case "_rot_item": return _rot_item(Gd.A(a, 0), Gd.A(a, 1));
    case "_rotate": return _rotate();
    case "_rotate_group": return _rotate_group();
    case "_bbox": return _bbox(Gd.A(a, 0));
    case "_align": return _align(Gd.A(a, 0));
    case "it_shift_x": return it_shift_x(Gd.A(a, 0), Gd.A(a, 1));
    case "_props_soft": return _props_soft();
    case "_props": return _props();
    case "_plen": return _plen(Gd.A(a, 0));
    case "_set_field": return _set_field(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "_ftext": return _ftext(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "_fnum": return _fnum(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4), Gd.A(a, 5), Gd.A(a, 6));
    case "_fenum": return _fenum(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3), Gd.A(a, 4));
    case "_fbool": return _fbool(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    case "_process": return _process(Gd.A(a, 0));
    case "_draw": return _draw();
    case "_draw_sel": return _draw_sel(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "_crosshair": return _crosshair(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2));
    case "selftest": return selftest();
    case "clicktest": return clicktest();
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    case "GRID": return GRID;
    case "PX_PER_M": return PX_PER_M;
    case "PARTS": return PARTS;
    case "MEDIA": return MEDIA;
    case "COLORS": return COLORS;
    case "ON_PIPE": return ON_PIPE;
    case "PROTECT": return PROTECT;
    case "VBINDS": return VBINDS;
    case "PBINDS": return PBINDS;
    case "SHAPES": return SHAPES;
    case "VKINDS": return VKINDS;
    case "IKINDS": return IKINDS;
    case "TOOLS": return TOOLS;
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_editor(); return Gv.O(o); }
  public override string ScriptName { get { return "editor"; } }
}
}
