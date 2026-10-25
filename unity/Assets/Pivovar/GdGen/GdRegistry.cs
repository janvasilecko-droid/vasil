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
    Gd.Register("S_main_CabDock", S_main_CabDock.New, S_main_CabDock.StaticGet, S_main_CabDock.StaticCall);
    Gd.Register("S_main_StaticLayer", S_main_StaticLayer.New, S_main_StaticLayer.StaticGet, S_main_StaticLayer.StaticCall);
    Gd.Register("S_main_Mini", S_main_Mini.New, S_main_Mini.StaticGet, S_main_Mini.StaticCall);
    Gd.Register("S_main_MiniView", S_main_MiniView.New, S_main_MiniView.StaticGet, S_main_MiniView.StaticCall);
    Gd.Register("S_main_FlowLayer", S_main_FlowLayer.New, S_main_FlowLayer.StaticGet, S_main_FlowLayer.StaticCall);
    Gd.Register("S_main_LiveNode", S_main_LiveNode.New, S_main_LiveNode.StaticGet, S_main_LiveNode.StaticCall);
    Gd.Register("S_main_ItemLayer", S_main_ItemLayer.New, S_main_ItemLayer.StaticGet, S_main_ItemLayer.StaticCall);
    Gd.Register("S_main_Hud", S_main_Hud.New, S_main_Hud.StaticGet, S_main_Hud.StaticCall);
    Gd.Register("main", S_main.New, S_main.StaticGet, S_main.StaticCall);
    Gd.Register("S_ctl_Trend", S_ctl_Trend.New, S_ctl_Trend.StaticGet, S_ctl_Trend.StaticCall);
    Gd.Register("ctl", S_ctl.New, S_ctl.StaticGet, S_ctl.StaticCall);
    Gd.Register("cabinet", S_cabinet.New, S_cabinet.StaticGet, S_cabinet.StaticCall);
    Gd.Register("S_liquidfx_Gpu", S_liquidfx_Gpu.New, S_liquidfx_Gpu.StaticGet, S_liquidfx_Gpu.StaticCall);
    Gd.Register("liquidfx", S_liquidfx.New, S_liquidfx.StaticGet, S_liquidfx.StaticCall);
    Gd.Register("S_oppanel_Knob", S_oppanel_Knob.New, S_oppanel_Knob.StaticGet, S_oppanel_Knob.StaticCall);
    Gd.Register("oppanel", S_oppanel.New, S_oppanel.StaticGet, S_oppanel.StaticCall);
    Gd.Register("rotametr", S_rotametr.New, S_rotametr.StaticGet, S_rotametr.StaticCall);
    Gd.Register("sight", S_sight.New, S_sight.StaticGet, S_sight.StaticCall);
    Gd.Register("iodine", S_iodine.New, S_iodine.StaticGet, S_iodine.StaticCall);
    Gd.Register("editor", S_editor.New, S_editor.StaticGet, S_editor.StaticCall);
    Gd.Register("S_game_HoverLayer", S_game_HoverLayer.New, S_game_HoverLayer.StaticGet, S_game_HoverLayer.StaticCall);
    Gd.Register("game", S_game.New, S_game.StaticGet, S_game.StaticCall);
    Gd.Register("boot", S_boot.New, S_boot.StaticGet, S_boot.StaticCall);
    Gd.Register("updater", S_updater.New, S_updater.StaticGet, S_updater.StaticCall);
  }
}
}
