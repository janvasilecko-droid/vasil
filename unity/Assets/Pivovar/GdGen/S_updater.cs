// Vygenerováno z updater.gd překladačem gd2cs.py – needituj ručně.
using System; using System.Collections.Generic;
namespace Pivovar.Gd.Gen {
public partial class S_updater : GdNode {
  public static Gv BASE;
  public Gv main;
  public Gv http;
  public Gv stage;
  public Gv remote;
  public Gv status = Gv.O(new GdSignal());
  public Gv available = Gv.O(new GdSignal());
  static S_updater() {
    BASE = Gv.S("https://raw.githubusercontent.com/janvasilecko-droid/vasil/main/trenazer2d/");
  }
  public S_updater() {
    main = Gv.Null;
    http = Gv.Null;
    stage = Gv.S("");
    remote = Gd.Dict();
  }
  public Gv _ready() {
    http = Gd.New("HTTPRequest");
    http = http.S("timeout", Gv.F(180.0));
    Self.C("add_child", http);
    http.G("request_completed").C("connect", Gd.Fn((_a) => _done(Gd.A(_a, 0), Gd.A(_a, 1), Gd.A(_a, 2), Gd.A(_a, 3))));
    return Gv.Null;
  }
  public Gv current_ver() {
    Gv f = Gd.Static("FileAccess", "open", Gv.S("res://version.json"), Gd.StaticGet("FileAccess", "READ"));
    if (Gv.Eq(f, Gv.Null)) {
      return (Gv.I(0L)).AsI();
    }
    Gv d = Gd.Static("JSON", "parse_string", f.C("get_as_text"));
    return ((Gv.Is(d, "Dictionary") ? Gd.Dyn("int", d.C("get", Gv.S("ver"), Gv.I(0L))) : Gv.I(0L))).AsI();
    return Gv.Null;
  }
  public Gv check(Gv download = default(Gv)) {
    if (download.k == 0) download = Gv.B(true);
    if (Gv.Ne(stage, Gv.S(""))) {
      return Gv.Null;
    }
    stage = ((download).T() ? Gv.S("check_dl") : Gv.S("check"));
    if ((download).T()) {
      status.C("emit", Gv.S("Hledám novou verzi…"), Gv.S("info"));
    }
    Gv err = http.C("request", (BASE + (Gv.S("version.json?t=%d") % Gd.Dyn("int", Gd.Static("Time", "get_unix_time_from_system")))));
    if (Gv.Ne(err, Gd.K("OK"))) {
      stage = Gv.S("");
      status.C("emit", (Gv.S("Aktualizace: nejde se připojit (%d).") % err), Gv.S("warn"));
    }
    return Gv.Null;
  }
  public Gv _done(Gv result = default(Gv), Gv code = default(Gv), Gv _h = default(Gv), Gv body = default(Gv)) {
    result = (result).AsI();
    code = (code).AsI();
    Gv st = stage;
    stage = Gv.S("");
    if ((Gv.Ne(result, Gd.StaticGet("HTTPRequest", "RESULT_SUCCESS")) || Gv.Ne(code, Gv.I(200L)))) {
      http = http.S("download_file", Gv.S(""));
      if (Gv.Ne(st, Gv.S("check"))) {
        status.C("emit", (Gv.S("Aktualizace: server neodpověděl (chyba %d / HTTP %d) – zkontroluj internet.") % Gd.Arr(result, code)), Gv.S("warn"));
      }
      return Gv.Null;
    }
    if ((st.C("begins_with", Gv.S("check"))).T()) {
      Gv d = Gd.Static("JSON", "parse_string", body.C("get_string_from_utf8"));
      if (!Gv.Is(d, "Dictionary")) {
        status.C("emit", Gv.S("Aktualizace: špatný soubor verze."), Gv.S("warn"));
        return Gv.Null;
      }
      remote = d;
      Gv cv = current_ver();
      if (Gv.Le(Gd.Dyn("int", d.C("get", Gv.S("ver"), Gv.I(0L))), cv)) {
        if (Gv.Eq(st, Gv.S("check_dl"))) {
          status.C("emit", (Gv.S("Máš nejnovější verzi (%d).") % cv), Gv.S("info"));
        }
        return Gv.Null;
      }
      available.C("emit", Gd.Dyn("int", d.G("ver")));
      if (Gv.Eq(st, Gv.S("check"))) {
        Gv dlg = Gd.New("ConfirmationDialog");
        dlg = dlg.S("title", Gv.S("Nová verze trenažéru"));
        dlg = dlg.S("dialog_text", (Gv.S("Je k dispozici verze %d (máš %d).\n\n%s\n\nStáhnout a restartovat teď?") % Gd.Arr(Gd.Dyn("int", d.G("ver")), cv, Gd.Dyn("str", d.C("get", Gv.S("notes"), Gv.S(""))))));
        dlg = dlg.S("dialog_autowrap", Gv.B(true));
        dlg = dlg.S("ok_button_text", Gv.S("Aktualizovat"));
        dlg = dlg.S("cancel_button_text", Gv.S("Později"));
        dlg.G("confirmed").C("connect", Gd.Fn((_a) => {  check(Gv.B(true)); return Gv.Null; }));
        Self.C("get_tree").G("root").C("add_child", dlg);
        dlg.C("popup_centered", Gd.Ctor("Vector2i", Gv.I(560L), Gv.I(260L)));
        return Gv.Null;
      }
      status.C("emit", (Gv.S("Stahuji verzi %d…") % Gd.Dyn("int", d.G("ver"))), Gv.S("info"));
      stage = Gv.S("pck");
      http = http.S("download_file", Gv.S("user://update2d.pck.part"));
      Gv err = http.C("request", (BASE + (Gv.S("trenazer2d.pck?v=%d") % Gd.Dyn("int", d.G("ver")))));
      if (Gv.Ne(err, Gd.K("OK"))) {
        stage = Gv.S("");
        http = http.S("download_file", Gv.S(""));
        status.C("emit", (Gv.S("Stažení nejde spustit (%d).") % err), Gv.S("warn"));
      }
    } else if (Gv.Eq(st, Gv.S("pck"))) {
      http = http.S("download_file", Gv.S(""));
      Gv f = Gd.Static("FileAccess", "open", Gv.S("user://update2d.pck.part"), Gd.StaticGet("FileAccess", "READ"));
      Gv size = ((Gv.Ne(f, Gv.Null) ? f.C("get_length") : Gv.I(0L))).AsI();
      if (Gv.Ne(f, Gv.Null)) {
        f.C("close");
      }
      if ((Gv.Lt(size, Gv.I(1000L)) || ((remote.C("has", Gv.S("size"))).T() && Gv.Ne(size, Gd.Dyn("int", remote.G("size")))))) {
        status.C("emit", (Gv.S("Stažený soubor je neúplný (%d B) – zkus to znovu.") % size), Gv.S("warn"));
        return Gv.Null;
      }
      Gv dir = Gd.Static("DirAccess", "open", Gv.S("user://"));
      if ((dir.C("file_exists", Gv.S("update2d.pck"))).T()) {
        dir.C("remove", Gv.S("update2d.pck"));
      }
      dir.C("rename", Gv.S("update2d.pck.part"), Gv.S("update2d.pck"));
      Gv vf = Gd.Static("FileAccess", "open", Gv.S("user://update2d_ver.json"), Gd.StaticGet("FileAccess", "WRITE"));
      vf.C("store_string", Gd.Static("JSON", "stringify", remote));
      vf.C("close");
      status.C("emit", (Gv.S("Verze %d stažena – program se restartuje…") % Gd.Dyn("int", remote.G("ver"))), Gv.S("info"));
      if ((Gv.Ne(main, Gv.Null) && (main.C("has_method", Gv.S("save_game"))).T())) {
        main.C("save_game");
      }
      _ = Self.C("get_tree").C("create_timer", Gv.F(1.2)).G("timeout");
      Gd.Static("OS", "set_restart_on_exit", Gv.B(true));
      Self.C("get_tree").C("quit");
    }
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    case "BASE": return S_updater.BASE;
    case "main": return this.main;
    case "http": return this.http;
    case "stage": return this.stage;
    case "remote": return this.remote;
    case "status": return this.status;
    case "available": return this.available;
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    case "main": this.main = v; return v;
    case "http": this.http = v; return v;
    case "stage": this.stage = v; return v;
    case "remote": this.remote = v; return v;
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "_ready": return _ready();
    case "current_ver": return current_ver();
    case "check": return check(Gd.A(a, 0));
    case "_done": return _done(Gd.A(a, 0), Gd.A(a, 1), Gd.A(a, 2), Gd.A(a, 3));
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    case "BASE": return BASE;
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_updater(); return Gv.O(o); }
  public override string ScriptName { get { return "updater"; } }
}
}
