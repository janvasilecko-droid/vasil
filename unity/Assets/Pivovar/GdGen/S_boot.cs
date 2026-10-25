// Vygenerováno z boot.gd překladačem gd2cs.py – needituj ručně.
using System; using System.Collections.Generic;
namespace Pivovar.Gd.Gen {
public partial class S_boot : GdNode {
  static S_boot() {
  }
  public S_boot() {
  }
  public static Gv ver_of(Gv path = default(Gv)) {
    if (!(Gd.Static("FileAccess", "file_exists", path)).T()) {
      return (Gv.I(0L)).AsI();
    }
    Gv d = Gd.Static("JSON", "parse_string", Gd.Static("FileAccess", "open", path, Gd.StaticGet("FileAccess", "READ")).C("get_as_text"));
    return ((Gv.Is(d, "Dictionary") ? Gd.Dyn("int", d.C("get", Gv.S("ver"), Gv.I(0L))) : Gv.I(0L))).AsI();
    return Gv.Null;
  }
  public Gv _ready() {
    Gv base_ = ver_of(Gv.S("res://version.json"));
    Gv upd = ver_of(Gv.S("user://update2d_ver.json"));
    if ((Gv.Gt(upd, base_) && (Gd.Static("FileAccess", "file_exists", Gv.S("user://update2d.pck"))).T())) {
      if (!(Gd.Static("ProjectSettings", "load_resource_pack", Gv.S("user://update2d.pck"), Gv.B(true))).T()) {
        Gd.Dyn("push_warning", Gv.S("Aktualizaci se nepodařilo načíst – jede vestavěná verze."));
      }
    }
    Gd.print((Gv.S("Trenažér varny: verze %d (vestavěná %d, stažená %d)") % Gd.Arr(ver_of(Gv.S("res://version.json")), base_, upd)));
    Self.C("get_tree").G("change_scene_to_file").C("call_deferred", Gv.S("res://main.tscn"));
    return Gv.Null;
  }
  public override Gv Get(string n) { switch (n) {
    default: return base.Get(n); } }
  public override Gv Set(string n, Gv v) { switch (n) {
    default: return base.Set(n, v); } }
  public override Gv Invoke(string n, Gv[] a) { switch (n) {
    case "ver_of": return S_boot.ver_of(Gd.A(a, 0));
    case "_ready": return _ready();
    default: return base.Invoke(n, a); } }
  public static Gv StaticGet(string n) { switch (n) {
    default: return Gv.Null; } }
  public static Gv StaticCall(string n, Gv[] a) { switch (n) {
    case "ver_of": return ver_of(Gd.A(a, 0));
    case "new": return New(a);
    default: return Gv.Null; } }
  public static Gv New(params Gv[] a) { var o = new S_boot(); return Gv.O(o); }
  public override string ScriptName { get { return "boot"; } }
}
}
