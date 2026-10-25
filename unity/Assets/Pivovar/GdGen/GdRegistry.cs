// Vygenerováno – registr skriptů
using System; using System.Collections.Generic;
namespace Pivovar.Gd.Gen {
public static class GdRegistry {
  public static void Init() {
    Gd.Register("physics", S_physics.New, S_physics.StaticGet, S_physics.StaticCall);
    Gd.Register("params", S_params.New, S_params.StaticGet, S_params.StaticCall);
    Gd.Register("proc", S_proc.New, S_proc.StaticGet, S_proc.StaticCall);
    Gd.Register("auto", S_auto.New, S_auto.StaticGet, S_auto.StaticCall);
    Gd.Register("lauter_auto", S_lauter_auto.New, S_lauter_auto.StaticGet, S_lauter_auto.StaticCall);
    Gd.Register("training", S_training.New, S_training.StaticGet, S_training.StaticCall);
    Gd.Register("practice", S_practice.New, S_practice.StaticGet, S_practice.StaticCall);
    Gd.Register("sim", S_sim.New, S_sim.StaticGet, S_sim.StaticCall);
    Gd.Register("tests", S_tests.New, S_tests.StaticGet, S_tests.StaticCall);
    Gd.Register("audit_tests", S_audit_tests.New, S_audit_tests.StaticGet, S_audit_tests.StaticCall);
    Gd.Register("art", S_art.New, S_art.StaticGet, S_art.StaticCall);
  }
}
}
