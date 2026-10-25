// Vstup do ovládacích prvků: zásah myší (pořadí kreslení, mouse_filter, ořez), tlačítka, posuvníky, rolování, textová pole, tooltipy.
using System;
using System.Collections.Generic;
using KK = Pivovar.Gd.K;

namespace Pivovar.Gd
{
    public static partial class GdUi
    {
        public static GdNode HoverNode, GrabNode, TooltipRoot, TooltipSrc;
        public static bool TooltipVisible;
        static double hoverT;
        static GdNode tipLabel;
        static bool tipBuilt;

        static bool Inside(RenderItem it, double sx, double sy, out double lx, out double ly)
        {
            lx = ly = 0;
            var n = it.node; double w, h; Vec(n, "size", out w, out h);
            var inv = it.m.Inv(); inv.Apply(sx, sy, out lx, out ly);
            if (lx < 0 || ly < 0 || lx > w || ly > h) return false;
            if (it.clip != null && (sx < it.clip[0] || sy < it.clip[1] || sx > it.clip[2] || sy > it.clip[3])) return false;
            return true;
        }

        static int Filter(GdNode n) { var v = n.Raw("mouse_filter"); return v.IsNum ? (int)v.d : DefaultMouseFilter(n.TypeName); }

        /// <summary>Nejvyšší ovládací prvek pod bodem (mimo mouse_filter = IGNORE).</summary>
        public static GdNode HitTest(double sx, double sy)
        {
            var l = GdRender.CtlItems;
            for (int i = l.Count - 1; i >= 0; i--)
            {
                var it = l[i]; if (Filter(it.node) == 2) continue;
                if (it.node.TypeName == "ConfirmationDialog" && !VisibleSelf(it.node)) continue;
                double lx, ly; if (Inside(it, sx, sy, out lx, out ly)) return it.node;
            }
            return null;
        }

        static RenderItem ItemOf(GdNode n) { foreach (var it in GdRender.CtlItems) if (it.node == n) return it; return null; }

        static bool IsButton(GdNode n) { switch (n.TypeName) { case "Button": case "CheckBox": case "CheckButton": case "OptionButton": case "MenuButton": case "LinkButton": return true; } return false; }

        static void SetHover(GdNode n)
        {
            if (HoverNode == n) return;
            if (HoverNode != null) { HoverNode.Hover = false; HoverNode.UiVer++; HoverNode.Sig("mouse_exited").Emit(new Gv[0]); }
            HoverNode = n; hoverT = 0; TooltipVisible = false;
            if (n != null) { n.Hover = true; n.UiVer++; n.Sig("mouse_entered").Emit(new Gv[0]); }
        }

        /// <summary>Předá událost ovládacím prvkům. Vrací true, pokud ji prvek pohltil (nemá se předat _unhandled_input).</summary>
        public static bool GuiInput(GdNode root, GdEvent e)
        {
            if (!Active) return false;
            if (e.Type == "InputEventMouseMotion") return OnMotion(e);
            if (e.Type == "InputEventMouseButton") return OnButton(e);
            if (e.Type == "InputEventKey") return OnKey(e);
            return false;
        }

        static bool OnMotion(GdEvent e)
        {
            var pos = e.Get("position"); double sx = pos.d, sy = pos.e;
            if (GrabNode != null)
            {
                DragMove(GrabNode, sx, sy);
                return true;
            }
            var hit = HitTest(sx, sy);
            // Hover jen pro prvek, který událost zachytí (tlačítka apod.)
            SetHover(hit);
            if (hit != null) Deliver(hit, e, false);
            return hit != null && Filter(hit) == 0 && ConsumesPointer(hit);
        }

        static bool ConsumesPointer(GdNode n)
        {
            // STOP prvek pohltí událost; čisté panely a pozadí také (jako v Godotu)
            return true;
        }

        static bool OnButton(GdEvent e)
        {
            var pos = e.Get("position"); double sx = pos.d, sy = pos.e; int b = (int)e.Get("button_index").d; bool pressed = e.Get("pressed").T();
            if (b == 4 || b == 5)
            {   // kolečko
                if (!pressed) return HitTest(sx, sy) != null && Filter(HitTest(sx, sy)) == 0;
                var h = HitTest(sx, sy); GdNode stopper = null;
                for (var c = h; c != null && IsCtl(c); c = c.Parent)
                {
                    if (c.TypeName == "ScrollContainer" && c.MaxScrollV > 0.5) { ScrollBy(c, (b == 4 ? -1 : 1) * Math.Max(24, Num(c, "size", 0) * 0 + 40)); return true; }
                    if (c.TypeName == "HSlider" || c.TypeName == "SpinBox") { }
                    if (Filter(c) == 0) { stopper = c; break; }
                }
                Deliver(h, e, true);
                return stopper != null;
            }
            if (pressed)
            {
                var hit = HitTest(sx, sy);
                if (OpenPopup != null && (hit == null || !IsInside(hit, OpenPopup))) ClosePopup();
                if (hit == null) { if (FocusNode != null && IsEditable(FocusNode)) SetFocus(null); return false; }
                if (FocusNode != null && FocusNode != hit && IsEditable(FocusNode)) { CommitEdit(FocusNode); SetFocus(null); }
                Deliver(hit, e, true);
                if (b == 1) PressOn(hit, sx, sy);
                return Filter(hit) == 0 || GrabNode != null;
            }
            else
            {
                bool had = GrabNode != null;
                if (b == 1 && GrabNode != null) { ReleaseOn(GrabNode, sx, sy); GrabNode = null; return true; }
                var hit = HitTest(sx, sy);
                if (hit != null) Deliver(hit, e, true);
                return had || (hit != null && Filter(hit) == 0);
            }
        }

        static bool IsInside(GdNode n, GdNode anc) { for (var p = n; p != null; p = p.Parent) if (p == anc) return true; return false; }

        /// <summary>_gui_input a signál gui_input; při filtru PASS dál na rodiče.</summary>
        static void Deliver(GdNode target, GdEvent e, bool isButton)
        {
            Accepted = false;
            for (var c = target; c != null && IsCtl(c); c = c.Parent)
            {
                try { c.Invoke("_gui_input", new Gv[] { e.Self }); } catch (Exception ex) { if (!Gd.Lenient) throw; Gd.Err("_gui_input " + c.ScriptName, ex); }
                c.Sig("gui_input").Emit(new Gv[] { e.Self });
                if (Accepted || Filter(c) == 0) break;
            }
        }

        static void PressOn(GdNode n, double sx, double sy)
        {
            if (IsButton(n))
            {
                if (Flag(n, "disabled", false)) return;
                n.Down = true; n.UiVer++; GrabNode = n; n.Sig("button_down").Emit(new Gv[0]);
                if (n.TypeName == "OptionButton") { }
                return;
            }
            switch (n.TypeName)
            {
                case "HSlider": GrabNode = n; n.Down = true; SliderTo(n, sx); return;
                case "LineEdit": case "SpinBox": if (Num(n, "focus_mode", 2) != 0) SetFocus(n); return;
                case "ScrollContainer":
                    {
                        double lx, ly; var it = ItemOf(n); if (it != null && Inside(it, sx, sy, out lx, out ly) && n.MaxScrollV > 0.5)
                        {
                            double w, h; Vec(n, "size", out w, out h);
                            if (lx >= w - ScrollBarW) { GrabNode = n; n.Down = true; DragMove(n, sx, sy); }
                        }
                        return;
                    }
            }
            // posuvník v rodiči (táhnutí za scroll bar přes děti není třeba)
        }

        static void ReleaseOn(GdNode n, double sx, double sy)
        {
            n.Down = false; n.UiVer++;
            if (IsButton(n))
            {
                var hit = HitTest(sx, sy);
                n.Sig("button_up").Emit(new Gv[0]);
                if (hit == n && !Flag(n, "disabled", false))
                {
                    if (n.TypeName == "OptionButton") { ShowOptionPopup(n); return; }
                    bool toggle = Flag(n, "toggle_mode", false) || n.TypeName == "CheckBox" || n.TypeName == "CheckButton";
                    if (toggle) { bool nv = !Flag(n, "button_pressed", false); n.Set("button_pressed", Gv.B(nv)); n.Sig("pressed").Emit(new Gv[0]); n.Sig("toggled").Emit(new Gv[] { Gv.B(nv) }); }
                    else n.Sig("pressed").Emit(new Gv[0]);
                }
            }
            else if (n.TypeName == "HSlider" || n.TypeName == "ScrollContainer") { }
        }

        static void DragMove(GdNode n, double sx, double sy)
        {
            if (n.TypeName == "HSlider") { SliderTo(n, sx); return; }
            if (n.TypeName == "ScrollContainer")
            {
                var it = ItemOf(n); if (it == null) return; double lx, ly; var inv = it.m.Inv(); inv.Apply(sx, sy, out lx, out ly);
                double w, h; Vec(n, "size", out w, out h); double gh = Math.Max(24, h * h / Math.Max(1, n.ContentH));
                double r = (ly - gh * 0.5) / Math.Max(1, h - gh); n.Set("scroll_vertical", Gv.F(Math.Max(0, Math.Min(1, r)) * n.MaxScrollV)); n.ScrollV = Math.Max(0, Math.Min(1, r)) * n.MaxScrollV; n.UiVer++; Ver++;
            }
        }

        static void SliderTo(GdNode n, double sx)
        {
            var it = ItemOf(n); if (it == null) return; double lx, ly; var inv = it.m.Inv(); inv.Apply(sx, 0, out lx, out ly);
            double w, h; Vec(n, "size", out w, out h);
            double mn = Num(n, "min_value", 0), mx = Num(n, "max_value", 100), st = Num(n, "step", 1);
            double r = Math.Max(0, Math.Min(1, (lx - 8) / Math.Max(1, w - 16))); double v = mn + (mx - mn) * r;
            if (st > 0) v = Math.Round(v / st) * st; v = Math.Max(mn, Math.Min(mx, v));
            if (Math.Abs(v - Num(n, "value", mn)) > 1e-12) { n.Set("value", Gv.F(v)); n.Sig("value_changed").Emit(new Gv[] { Gv.F(v) }); }
        }

        static void ScrollBy(GdNode c, double d)
        {
            double nv = Math.Max(0, Math.Min(c.MaxScrollV, c.ScrollV + d)); c.ScrollV = nv; c.Set("scroll_vertical", Gv.F(nv)); Ver++; c.UiVer++;
        }

        // ---------- klávesnice (textová pole) ----------
        static bool IsEditable(GdNode n) { return n.TypeName == "LineEdit" || n.TypeName == "SpinBox"; }

        static void CommitEdit(GdNode n)
        {
            if (n.TypeName == "SpinBox" && n.Raw("_edit").k == KK.Str)
            {
                double v; string s = n.Raw("_edit").Str.Replace(',', '.');
                if (double.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out v))
                {
                    double mn = Num(n, "min_value", 0), mx = Num(n, "max_value", 100), st = Num(n, "step", 1);
                    if (st > 0) v = Math.Round(v / st) * st; v = Math.Max(mn, Math.Min(mx, v));
                    n.Set("value", Gv.F(v)); n.Sig("value_changed").Emit(new Gv[] { Gv.F(v) });
                }
                n.SetRaw("_edit", Gv.Null); n.UiVer++;
            }
        }

        static bool OnKey(GdEvent e)
        {
            var f = FocusNode; if (f == null || !IsEditable(f)) return false;
            if (!e.Get("pressed").T()) return true;
            int key = (int)e.Get("keycode").d; int uni = (int)e.Get("unicode").d;
            bool spin = f.TypeName == "SpinBox";
            string txt = spin ? SpinText(f) : StrOf(f, "text");
            switch (key)
            {
                case 4194308: if (txt.Length > 0) txt = txt.Substring(0, txt.Length - 1); break;       // Backspace
                case 4194312: break;                                                                  // Delete
                case 4194309: case 4194310:                                                           // Enter
                    if (spin) CommitEdit(f); else f.Sig("text_submitted").Emit(new Gv[] { Gv.S(txt) });
                    SetFocus(null); return true;
                case 4194305: SetFocus(null); return true;                                            // Esc
                default:
                    if (uni >= 32 && !e.Get("ctrl_pressed").T()) txt += char.ConvertFromUtf32(uni); else return true;
                    break;
            }
            if (spin) f.SetRaw("_edit", Gv.S(txt));
            else { f.Set("text", Gv.S(txt)); f.Sig("text_changed").Emit(new Gv[] { Gv.S(txt) }); }
            f.UiVer++; return true;
        }

        // ---------- OptionButton: rozbalovací nabídka ----------
        public static GdNode OpenPopup;
        static void ClosePopup() { if (OpenPopup != null) { var p = OpenPopup.Parent; if (p != null) p.Children.Remove(OpenPopup); OpenPopup.Parent = null; OpenPopup = null; Ver++; } }

        static void ShowOptionPopup(GdNode ob)
        {
            ClosePopup();
            var pop = new GdNode(); pop.TypeName = "PanelContainer"; pop.SetRaw("z_index", Gv.I(300)); pop.SetRaw("top_level", Gv.B(true));
            var vb = new GdNode(); vb.TypeName = "VBoxContainer"; vb.SetRaw("ovr:constant/separation", Gv.I(2));
            Action<GdNode, GdNode> add = (p, c) => { p.Children.Add(c); c.Parent = p; };
            add(pop, vb);
            var items = ItemsOf(ob);
            for (int i = 0; i < items.Count; i++)
            {
                int idx = i; var b = new GdNode(); b.TypeName = "Button"; b.SetRaw("text", Gv.S(items[i].ToStr())); b.SetRaw("alignment", Gv.I(0)); b.SetRaw("flat", Gv.B(true));
                b.Sig("pressed").Connect(Gv.Fn(x => { ob.SetRaw("selected", Gv.I(idx)); ob.UiVer++; Ver++; ClosePopup(); ob.Sig("item_selected").Emit(new Gv[] { Gv.I(idx) }); return Gv.Null; }));
                add(vb, b);
            }
            double w, h; Vec(ob, "size", out w, out h);
            var m = GdRender.GlobalMat(ob); double gx, gy; m.Apply(0, 0, out gx, out gy);
            ob.Children.Add(pop); pop.Parent = ob;
            pop.AnchorSet = false; pop.PosSet = true; pop.SetRaw("position", Gv.Vec(0, h + 2)); pop.SetRaw("custom_minimum_size", Gv.Vec(w, 0));
            pop.SetRaw("theme", ob.Raw("theme"));
            OpenPopup = pop; Ver++;
        }

        // ---------- tooltip a pravidelná aktualizace ----------
        static void BuildTip()
        {
            if (tipBuilt) return; tipBuilt = true;
            var p = new GdNode(); p.TypeName = "PanelContainer"; p.SetRaw("mouse_filter", Gv.I(2));
            var sb = new GdNode(); sb.TypeName = "StyleBoxFlat"; sb.SetRaw("bg_color", Gv.Col(0.07, 0.085, 0.1, 0.96));
            foreach (var k in new[] { "top_left", "top_right", "bottom_right", "bottom_left" }) sb.SetRaw("corner_radius_" + k, Gv.F(5));
            foreach (var k in new[] { "left", "top", "right", "bottom" }) { sb.SetRaw("border_width_" + k, Gv.F(1)); }
            sb.SetRaw("border_color", Gv.Col(0.35, 0.4, 0.45, 1));
            sb.SetRaw("content_margin_left", Gv.F(8)); sb.SetRaw("content_margin_right", Gv.F(8)); sb.SetRaw("content_margin_top", Gv.F(5)); sb.SetRaw("content_margin_bottom", Gv.F(5));
            p.SetRaw("ovr:stylebox/panel", sb.Self);
            var l = new GdNode(); l.TypeName = "Label"; l.SetRaw("ovr:font_size/font_size", Gv.F(13)); l.SetRaw("autowrap_mode", Gv.I(3)); l.SetRaw("custom_minimum_size", Gv.Vec(10, 0));
            p.Children.Add(l); l.Parent = p; TooltipRoot = p; tipLabel = l;
            p.SetRaw("position", Gv.Vec(0, 0)); p.PosSet = true;
        }

        public static void LayoutTooltip()
        {
            if (TooltipRoot != null && TooltipVisible && TooltipRoot.LayoutVer != Ver) LayoutRoot(TooltipRoot);
        }

        /// <summary>Volá hostitel každý snímek: zpožděný tooltip.</summary>
        public static void Tick(double dt)
        {
            if (!Active) return;
            hoverT += dt;
            var h = HoverNode; string tip = null;
            for (var c = h; c != null && IsCtl(c); c = c.Parent) { var t = c.Raw("tooltip_text"); if (t.k == KK.Str && t.Str.Length > 0) { tip = t.Str; break; } }
            if (tip == null || GrabNode != null) { TooltipVisible = false; return; }
            if (hoverT < 0.55) return;
            BuildTip();
            if (!TooltipVisible || StrOf(tipLabel, "text") != tip)
            {
                tipLabel.Set("text", Gv.S(tip)); double tw, th; TextSize(tip, 13, out tw, out th);
                tipLabel.SetRaw("custom_minimum_size", Gv.Vec(Math.Min(360, tw + 2), 0));
                TooltipVisible = true; Ver++;
            }
            double mx = Gd.MousePos.d + 14, my = Gd.MousePos.e + 20; double pw, ph; MinSize(TooltipRoot, out pw, out ph);
            mx = Math.Min(mx, Gd.ScreenW - pw - 6); my = Math.Min(my, Gd.ScreenH - ph - 6);
            var op = TooltipRoot.Raw("position"); if (!(op.k == KK.Vec2 && Math.Abs(op.d - mx) < 0.5 && Math.Abs(op.e - my) < 0.5)) { TooltipRoot.Set("position", Gv.Vec(mx, my)); }
        }
    }
}
