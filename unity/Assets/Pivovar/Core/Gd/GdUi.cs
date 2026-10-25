// Ovládací prvky (Control): rozložení (kotvy, offsety, kontejnery), minimální velikosti, motiv (theme) a konstanty.
// Chování odpovídá Godotu 4: kontejnery rozdělují místo podle size_flags, ostatní prvky podle kotev/offsetů.
using System;
using System.Collections.Generic;
using KK = Pivovar.Gd.K;

namespace Pivovar.Gd
{
    public static partial class GdUi
    {
        /// <summary>Zapíná se v hostiteli, který vykresluje (Unity, --gdboot). V testech je vypnuto.</summary>
        public static bool Active;
        public static int Ver = 1;            // verze rozložení – zvyšuje se při změně čehokoli, co ovlivňuje rozložení
        public static int Pass;               // číslo průchodu (mezipaměť minimálních velikostí)
        public static bool Accepted;          // accept_event()
        static bool inLayout, needAgain;

        static readonly HashSet<string> ControlTypes = new HashSet<string> {
            "Control", "Button", "CheckBox", "CheckButton", "OptionButton", "MenuButton", "LinkButton", "TextureButton", "Label", "RichTextLabel", "LineEdit", "TextEdit", "SpinBox",
            "ProgressBar", "HSlider", "VSlider", "HScrollBar", "VScrollBar", "ColorRect", "TextureRect", "Panel", "PanelContainer", "MarginContainer", "VBoxContainer", "HBoxContainer",
            "BoxContainer", "GridContainer", "HFlowContainer", "VFlowContainer", "FlowContainer", "ScrollContainer", "CenterContainer", "SubViewportContainer", "TabContainer",
            "HSeparator", "VSeparator", "ItemList", "AspectRatioContainer", "SplitContainer", "HSplitContainer", "VSplitContainer", "ConfirmationDialog", "AcceptDialog", "Window",
            "FileDialog", "PopupMenu", "Popup" };
        public static bool IsControlType(string t) { return ControlTypes.Contains(t); }

        static readonly HashSet<string> NoLayoutKeys = new HashSet<string> {
            "button_pressed", "value", "modulate", "self_modulate", "tooltip_text", "mouse_filter", "focus_mode", "disabled", "z_index", "color", "texture",
            "show_percentage", "mouse_default_cursor_shape", "toggle_mode", "pressed", "flat", "scroll_vertical", "scroll_horizontal", "editable", "selected", "step", "min_value", "max_value", "stretch" };

        // ---------- konstanty ----------
        public static bool TryConst(string cls, string n, out Gv v)
        {
            v = default(Gv);
            switch (cls)
            {
                case "Control":
                    switch (n)
                    {
                        case "SIZE_FILL": v = Gv.I(1); return true; case "SIZE_EXPAND": v = Gv.I(2); return true; case "SIZE_EXPAND_FILL": v = Gv.I(3); return true;
                        case "SIZE_SHRINK_BEGIN": v = Gv.I(0); return true; case "SIZE_SHRINK_CENTER": v = Gv.I(4); return true; case "SIZE_SHRINK_END": v = Gv.I(8); return true;
                        case "MOUSE_FILTER_STOP": v = Gv.I(0); return true; case "MOUSE_FILTER_PASS": v = Gv.I(1); return true; case "MOUSE_FILTER_IGNORE": v = Gv.I(2); return true;
                        case "FOCUS_NONE": v = Gv.I(0); return true; case "FOCUS_CLICK": v = Gv.I(1); return true; case "FOCUS_ALL": v = Gv.I(2); return true;
                        case "GROW_DIRECTION_BEGIN": v = Gv.I(0); return true; case "GROW_DIRECTION_END": v = Gv.I(1); return true; case "GROW_DIRECTION_BOTH": v = Gv.I(2); return true;
                        case "CURSOR_ARROW": v = Gv.I(0); return true; case "CURSOR_POINTING_HAND": v = Gv.I(2); return true; case "CURSOR_DRAG": v = Gv.I(13); return true;
                        case "PRESET_TOP_LEFT": v = Gv.I(0); return true; case "PRESET_TOP_RIGHT": v = Gv.I(1); return true; case "PRESET_BOTTOM_LEFT": v = Gv.I(2); return true;
                        case "PRESET_BOTTOM_RIGHT": v = Gv.I(3); return true; case "PRESET_CENTER_LEFT": v = Gv.I(4); return true; case "PRESET_CENTER_TOP": v = Gv.I(5); return true;
                        case "PRESET_CENTER_RIGHT": v = Gv.I(6); return true; case "PRESET_CENTER_BOTTOM": v = Gv.I(7); return true; case "PRESET_CENTER": v = Gv.I(8); return true;
                        case "PRESET_LEFT_WIDE": v = Gv.I(9); return true; case "PRESET_TOP_WIDE": v = Gv.I(10); return true; case "PRESET_RIGHT_WIDE": v = Gv.I(11); return true;
                        case "PRESET_BOTTOM_WIDE": v = Gv.I(12); return true; case "PRESET_VCENTER_WIDE": v = Gv.I(13); return true; case "PRESET_HCENTER_WIDE": v = Gv.I(14); return true;
                        case "PRESET_FULL_RECT": v = Gv.I(15); return true;
                    }
                    break;
                case "TextServer":
                    switch (n)
                    {
                        case "AUTOWRAP_OFF": v = Gv.I(0); return true; case "AUTOWRAP_ARBITRARY": v = Gv.I(1); return true; case "AUTOWRAP_WORD": v = Gv.I(2); return true; case "AUTOWRAP_WORD_SMART": v = Gv.I(3); return true;
                        case "OVERRUN_NO_TRIMMING": v = Gv.I(0); return true; case "OVERRUN_TRIM_CHAR": v = Gv.I(1); return true; case "OVERRUN_TRIM_WORD": v = Gv.I(2); return true;
                        case "OVERRUN_TRIM_ELLIPSIS": v = Gv.I(3); return true; case "OVERRUN_TRIM_WORD_ELLIPSIS": v = Gv.I(4); return true;
                    }
                    break;
                case "ScrollContainer":
                    switch (n) { case "SCROLL_MODE_DISABLED": v = Gv.I(0); return true; case "SCROLL_MODE_AUTO": v = Gv.I(1); return true; case "SCROLL_MODE_SHOW_ALWAYS": v = Gv.I(2); return true; case "SCROLL_MODE_SHOW_NEVER": v = Gv.I(3); return true; }
                    break;
                case "BoxContainer":
                    switch (n) { case "ALIGNMENT_BEGIN": v = Gv.I(0); return true; case "ALIGNMENT_CENTER": v = Gv.I(1); return true; case "ALIGNMENT_END": v = Gv.I(2); return true; }
                    break;
                case "SubViewport":
                    switch (n) { case "UPDATE_DISABLED": v = Gv.I(0); return true; case "UPDATE_ONCE": v = Gv.I(1); return true; case "UPDATE_WHEN_VISIBLE": v = Gv.I(2); return true; case "UPDATE_WHEN_PARENT_VISIBLE": v = Gv.I(3); return true; case "UPDATE_ALWAYS": v = Gv.I(4); return true; }
                    break;
                case "CanvasItem":
                    switch (n)
                    {
                        case "TEXTURE_FILTER_NEAREST": v = Gv.I(1); return true; case "TEXTURE_FILTER_LINEAR": v = Gv.I(2); return true; case "TEXTURE_FILTER_NEAREST_WITH_MIPMAPS": v = Gv.I(3); return true;
                        case "TEXTURE_FILTER_LINEAR_WITH_MIPMAPS": v = Gv.I(4); return true; case "TEXTURE_REPEAT_DISABLED": v = Gv.I(1); return true; case "TEXTURE_REPEAT_ENABLED": v = Gv.I(2); return true;
                    }
                    break;
                case "Line2D":
                    switch (n) { case "LINE_TEXTURE_NONE": v = Gv.I(0); return true; case "LINE_TEXTURE_TILE": v = Gv.I(1); return true; case "LINE_TEXTURE_STRETCH": v = Gv.I(2); return true; }
                    break;
                case "AudioStreamWAV":
                    switch (n) { case "FORMAT_8_BITS": v = Gv.I(0); return true; case "FORMAT_16_BITS": v = Gv.I(1); return true; case "LOOP_DISABLED": v = Gv.I(0); return true; case "LOOP_FORWARD": v = Gv.I(1); return true; }
                    break;
                case "Viewport":
                    switch (n) { case "SCREEN_SPACE_AA_DISABLED": v = Gv.I(0); return true; case "SCREEN_SPACE_AA_FXAA": v = Gv.I(1); return true; case "MSAA_DISABLED": v = Gv.I(0); return true; case "MSAA_4X": v = Gv.I(2); return true; }
                    break;
            }
            return false;
        }

        // ---------- pomocné čtení vlastností ----------
        public static double Num(GdNode n, string k, double d) { var v = n.Raw(k); return v.IsNum ? v.d : d; }
        public static bool Flag(GdNode n, string k, bool d) { var v = n.Raw(k); return v.k == KK.Bool ? v.d != 0 : d; }
        public static bool VisibleSelf(GdNode n) { var v = n.Raw("visible"); return !(v.k == KK.Bool && v.d == 0); }
        static bool IsCtl(GdNode n) { return ControlTypes.Contains(n.TypeName); }
        static void Vec(GdNode n, string k, out double x, out double y) { var v = n.Raw(k); if (v.k == KK.Vec2 || v.k == KK.Vec2i) { x = v.d; y = v.e; } else { x = 0; y = 0; } }

        public static List<GdNode> Kids(GdNode n)
        {
            var l = new List<GdNode>();
            foreach (var c in n.Children) if (!c.QueuedFree && IsCtl(c) && VisibleSelf(c) && !Flag(c, "top_level", false)) l.Add(c);
            return l;
        }

        // ---------- hooky z GdNode ----------
        public static Gv GetCtl(GdNode n, string key, Gv v)
        {
            switch (key)
            {
                case "size": case "position": case "global_position":
                    if (n.LayoutVer != Ver) EnsureLayout(n);
                    v = n.Raw(key == "global_position" ? "position" : key);
                    if (key == "global_position") { var m = GdRender.GlobalMat(n); double gx, gy; m.Apply(0, 0, out gx, out gy); return Gv.Vec(gx, gy); }
                    return v.IsNull ? Gv.Vec(0, 0) : v;
                case "custom_minimum_size": return v.IsNull ? Gv.Vec(0, 0) : v;
                case "anchor_left": case "anchor_top": case "anchor_right": case "anchor_bottom": case "offset_left": case "offset_top": case "offset_right": case "offset_bottom":
                    if (v.IsNull) { if (n.LayoutVer != Ver) EnsureLayout(n); return Gv.F(ImplicitOffset(n, key)); }
                    return v;
                case "text": if (v.IsNull) return Gv.S(""); return v;
                case "button_pressed": case "disabled": case "toggle_mode": case "flat": if (v.IsNull) return Gv.B(false); return v;
                case "visible": if (v.IsNull) return Gv.B(true); return v;
                case "size_flags_horizontal": case "size_flags_vertical": if (v.IsNull) return Gv.I(1); return v;
                case "mouse_filter": if (v.IsNull) return Gv.I(DefaultMouseFilter(n.TypeName)); return v;
                case "value": case "scroll_vertical": case "scroll_horizontal": if (v.IsNull) return Gv.F(key == "value" ? 0 : n.ScrollV); return v;
                case "min_value": if (v.IsNull) return Gv.F(0); return v;
                case "max_value": if (v.IsNull) return Gv.F(100); return v;
                case "step": if (v.IsNull) return Gv.F(1); return v;
            }
            return v;
        }

        static double ImplicitOffset(GdNode n, string key)
        {
            double x, y, w, h; Vec(n, "position", out x, out y); Vec(n, "size", out w, out h);
            switch (key)
            {
                case "offset_left": return x - Num(n, "anchor_left", 0) * n.PW;
                case "offset_top": return y - Num(n, "anchor_top", 0) * n.PH;
                case "offset_right": return x + w - Num(n, "anchor_right", 0) * n.PW;
                case "offset_bottom": return y + h - Num(n, "anchor_bottom", 0) * n.PH;
            }
            return 0;
        }

        public static int DefaultMouseFilter(string t)
        {
            switch (t)
            {
                case "Control": case "VBoxContainer": case "HBoxContainer": case "BoxContainer": case "GridContainer": case "HFlowContainer": case "VFlowContainer": case "FlowContainer":
                case "CenterContainer": case "MarginContainer": case "ColorRect": case "TextureRect": case "Label": case "SubViewportContainer": case "HSeparator": case "VSeparator": return t == "Label" || t == "ColorRect" || t == "TextureRect" ? 2 : (t == "Control" ? 0 : 1);
            }
            return 0;
        }

        public static void OnSet(GdNode n, string key, Gv v, Gv old)
        {
            if (key.StartsWith("meta:")) return;
            if (!NoLayoutKeys.Contains(key)) Ver++;
            n.UiVer++;
            switch (key)
            {
                case "position":
                    if (n.AnchorSet && v.k == KK.Vec2)
                    {   // posun při kotvách = změna offsetů
                        double ox, oy; Vec(n, "position", out ox, out oy); var prev = n.Raw("position");
                        // n.Raw už obsahuje novou hodnotu; vrátíme se k předchozí (old) a posuneme offsety o rozdíl
                        double px = old.k == KK.Vec2 ? old.d : 0, py = old.k == KK.Vec2 ? old.e : 0;
                        double dx = v.d - px, dy = v.e - py;
                        n.SetRaw("offset_left", Gv.F(Num(n, "offset_left", 0) + dx)); n.SetRaw("offset_right", Gv.F(Num(n, "offset_right", 0) + dx));
                        n.SetRaw("offset_top", Gv.F(Num(n, "offset_top", 0) + dy)); n.SetRaw("offset_bottom", Gv.F(Num(n, "offset_bottom", 0) + dy));
                    }
                    else n.PosSet = true;
                    break;
                case "size":
                    if (n.AnchorSet && (v.k == KK.Vec2 || v.k == KK.Vec2i))
                    {
                        double ow = 0, oh = 0; if (old.k == KK.Vec2 || old.k == KK.Vec2i) { ow = old.d; oh = old.e; }
                        n.SetRaw("offset_right", Gv.F(Num(n, "offset_right", 0) + v.d - ow)); n.SetRaw("offset_bottom", Gv.F(Num(n, "offset_bottom", 0) + v.e - oh));
                    }
                    else n.SizeSet = true;
                    break;
                case "anchor_left": case "anchor_top": case "anchor_right": case "anchor_bottom": case "offset_left": case "offset_top": case "offset_right": case "offset_bottom":
                    if (!n.AnchorSet)
                    {   // přechod z volného režimu: zachovat dosavadní polohu/velikost v offsetech
                        n.AnchorSet = true;
                        double x, y, w, h; Vec(n, "position", out x, out y); Vec(n, "size", out w, out h);
                        if (key.StartsWith("anchor_")) { if (!n.HasProp("offset_left")) n.SetRaw("offset_left", Gv.F(x)); if (!n.HasProp("offset_top")) n.SetRaw("offset_top", Gv.F(y)); if (!n.HasProp("offset_right")) n.SetRaw("offset_right", Gv.F(x + w)); if (!n.HasProp("offset_bottom")) n.SetRaw("offset_bottom", Gv.F(y + h)); }
                        else
                        {
                            if (key != "offset_left" && !n.HasProp("offset_left")) n.SetRaw("offset_left", Gv.F(x));
                            if (key != "offset_top" && !n.HasProp("offset_top")) n.SetRaw("offset_top", Gv.F(y));
                            if (key != "offset_right" && !n.HasProp("offset_right")) n.SetRaw("offset_right", Gv.F(x + w));
                            if (key != "offset_bottom" && !n.HasProp("offset_bottom")) n.SetRaw("offset_bottom", Gv.F(y + h));
                        }
                    }
                    break;
                case "text": case "value": case "button_pressed": break;
            }
        }

        // ---------- volání metod ovládacích prvků ----------
        public static bool InvokeCtl(GdNode n, string name, Gv[] a, out Gv r)
        {
            r = Gv.Null;
            switch (name)
            {
                case "set_anchors_preset": SetPreset(n, (int)Gd.A(a, 0).d, a.Length > 1 && a[1].T()); return true;
                case "set_anchor": { string[] k = { "anchor_left", "anchor_top", "anchor_right", "anchor_bottom" }; int i = (int)Gd.A(a, 0).d; if (i >= 0 && i < 4) n.Set(k[i], Gd.A(a, 1)); return true; }
                case "set_offset": { string[] k = { "offset_left", "offset_top", "offset_right", "offset_bottom" }; int i = (int)Gd.A(a, 0).d; if (i >= 0 && i < 4) n.Set(k[i], Gd.A(a, 1)); return true; }
                case "set_offsets_preset": SetPreset(n, (int)Gd.A(a, 0).d, true); return true;
                case "get_combined_minimum_size": case "get_minimum_size": { if (n.LayoutVer != Ver) EnsureLayout(n); double w, h; Pass++; MinSize(n, out w, out h); r = Gv.Vec(w, h); return true; }
                case "reset_size": { n.SizeSet = false; n.SetRaw("size", Gv.Vec(0, 0)); Ver++; n.UiVer++; return true; }
                case "get_rect": { if (n.LayoutVer != Ver) EnsureLayout(n); double x, y, w, h; Vec(n, "position", out x, out y); Vec(n, "size", out w, out h); r = Gv.Rect(x, y, w, h); return true; }
                case "get_global_rect": { if (n.LayoutVer != Ver) EnsureLayout(n); double w, h; Vec(n, "size", out w, out h); var m = GdRender.GlobalMat(n); double gx, gy; m.Apply(0, 0, out gx, out gy); r = Gv.Rect(gx, gy, w * m.a, h * m.d); return true; }
                case "get_global_position": { var m = GdRender.GlobalMat(n); double gx, gy; m.Apply(0, 0, out gx, out gy); r = Gv.Vec(gx, gy); return true; }
                case "get_size": if (n.LayoutVer != Ver) EnsureLayout(n); r = n.Get("size"); return true;
                case "get_position": if (n.LayoutVer != Ver) EnsureLayout(n); r = n.Get("position"); return true;
                case "grab_focus": SetFocus(n); return true;
                case "release_focus": if (FocusNode == n) SetFocus(null); return true;
                case "has_focus": r = Gv.B(FocusNode == n); return true;
                case "queue_sort": Ver++; return true;
                case "is_visible_in_tree": { bool vis = true; for (var p = n; p != null; p = p.Parent) if (!VisibleSelf(p)) { vis = false; break; } r = Gv.B(vis); return true; }
                case "set_position": n.Set("position", Gd.A(a, 0)); return true;
                case "set_size": n.Set("size", Gd.A(a, 0)); return true;
                case "set_global_position": n.Set("position", Gd.A(a, 0)); return true;
            }
            return InvokeWidget(n, name, a, out r);
        }

        public static void SetPreset(GdNode n, int p, bool keepOffsets)
        {
            double l = 0, t = 0, rr = 0, b = 0;
            switch (p)
            {
                case 0: break;                                   // top left
                case 1: l = 1; rr = 1; break;                    // top right
                case 2: t = 1; b = 1; break;                     // bottom left
                case 3: l = 1; rr = 1; t = 1; b = 1; break;      // bottom right
                case 4: t = .5; b = .5; break;                   // center left
                case 5: l = .5; rr = .5; break;                  // center top
                case 6: l = 1; rr = 1; t = .5; b = .5; break;    // center right
                case 7: l = .5; rr = .5; t = 1; b = 1; break;    // center bottom
                case 8: l = .5; rr = .5; t = .5; b = .5; break;  // center
                case 9: rr = 0; b = 1; break;                    // left wide
                case 10: rr = 1; break;                          // top wide
                case 11: l = 1; rr = 1; b = 1; break;           // right wide
                case 12: t = 1; rr = 1; b = 1; break;           // bottom wide
                case 13: t = 0; l = .5; rr = .5; b = 1; break;   // vcenter wide
                case 14: t = .5; rr = 1; b = .5; break;          // hcenter wide
                case 15: rr = 1; b = 1; break;                   // full rect
            }
            if (p == 13) { l = .5; rr = .5; t = 0; b = 1; }
            n.Set("anchor_left", Gv.F(l)); n.Set("anchor_top", Gv.F(t)); n.Set("anchor_right", Gv.F(rr)); n.Set("anchor_bottom", Gv.F(b));
            n.AnchorSet = true;
            if (!n.HasProp("offset_left")) { n.SetRaw("offset_left", Gv.F(0)); n.SetRaw("offset_top", Gv.F(0)); n.SetRaw("offset_right", Gv.F(0)); n.SetRaw("offset_bottom", Gv.F(0)); }
        }

        // ---------- rozložení ----------
        public static void EnsureLayout(GdNode n)
        {
            if (inLayout) return;
            var top = n;
            while (top.Parent != null && IsCtl(top.Parent)) top = top.Parent;
            if (top.LayoutVer == Ver) return;
            LayoutRoot(top);
        }

        /// <summary>Rozloží všechny kořenové ovládací prvky stromu, které mají zastaralé rozložení.</summary>
        public static void LayoutAll(GdNode root)
        {
            if (!Active) return;
            Walk(root);
        }

        static void Walk(GdNode n)
        {
            if (n.QueuedFree) return;
            if (IsCtl(n) && (n.Parent == null || !IsCtl(n.Parent))) { if (n.LayoutVer != Ver && VisibleSelf(n)) LayoutRoot(n); return; }
            foreach (var c in n.Children.ToArray()) Walk(c);
        }

        static void ParentSize(GdNode c, out double pw, out double ph)
        {
            pw = Gd.ScreenW; ph = Gd.ScreenH;
            var p = c.Parent;
            if (p != null && p.TypeName == "SubViewport") { double sx, sy; Vec(p, "size", out sx, out sy); if (sx > 0 && sy > 0) { pw = sx; ph = sy; } }
        }

        static void LayoutRoot(GdNode c)
        {
            if (inLayout) return;
            inLayout = true;
            try
            {
                double pw, ph; ParentSize(c, out pw, out ph);
                for (int iter = 0; iter < 3; iter++)
                {
                    Pass++; needAgain = false;
                    LayoutFree(c, pw, ph);
                    if (!needAgain) break;
                }
                MarkLayout(c);
            }
            finally { inLayout = false; }
        }

        static void MarkLayout(GdNode n)
        {
            n.LayoutVer = Ver;
            foreach (var c in n.Children) if (IsCtl(c)) MarkLayout(c);
        }

        static void SetRect(GdNode c, double x, double y, double w, double h)
        {
            var op = c.Raw("position"); var os = c.Raw("size");
            if (!(op.k == KK.Vec2 && op.d == x && op.e == y)) { c.SetRaw("position", Gv.Vec(x, y)); c.UiVer++; }
            if (!((os.k == KK.Vec2 || os.k == KK.Vec2i) && os.d == w && os.e == h)) { c.SetRaw("size", Gv.Vec(w, h)); c.UiVer++; }
            if (c.TypeName == "Label" || c.TypeName == "RichTextLabel") { if (Math.Abs(w - c.WrapUsed) > 0.5 && Num(c, "autowrap_mode", 0) > 0) needAgain = true; }
        }

        static void LayoutFree(GdNode c, double pw, double ph)
        {
            c.PW = pw; c.PH = ph;
            double mw, mh; MinSize(c, out mw, out mh);
            double x, y, w, h;
            if (c.AnchorSet)
            {
                double al = Num(c, "anchor_left", 0), at = Num(c, "anchor_top", 0), ar = Num(c, "anchor_right", 0), ab = Num(c, "anchor_bottom", 0);
                double ol = Num(c, "offset_left", 0), ot = Num(c, "offset_top", 0), orr = Num(c, "offset_right", 0), ob = Num(c, "offset_bottom", 0);
                double left = al * pw + ol, right = ar * pw + orr, top = at * ph + ot, bottom = ab * ph + ob;
                w = right - left; h = bottom - top;
                if (w < mw) { int gh = (int)Num(c, "grow_horizontal", 1); if (gh == 0) left -= mw - w; else if (gh == 2) left -= (mw - w) * 0.5; w = mw; }
                if (h < mh) { int gv = (int)Num(c, "grow_vertical", 1); if (gv == 0) top -= mh - h; else if (gv == 2) top -= (mh - h) * 0.5; h = mh; }
                x = left; y = top;
            }
            else
            {
                Vec(c, "position", out x, out y);
                double sx = 0, sy = 0; if (c.SizeSet) Vec(c, "size", out sx, out sy);
                w = Math.Max(sx, mw); h = Math.Max(sy, mh);
            }
            SetRect(c, x, y, w, h);
            LayoutChildren(c);
        }

        static void FitChild(GdNode k, double x, double y, double w, double h)
        {
            double mw, mh; MinSize(k, out mw, out mh);
            int fh = (int)Num(k, "size_flags_horizontal", 1), fv = (int)Num(k, "size_flags_vertical", 1);
            double cw = w, ch = h, cx = x, cy = y;
            if ((fh & 1) == 0) { cw = Math.Min(w, mw); if ((fh & 8) != 0) cx = x + w - cw; else if ((fh & 4) != 0) cx = x + (w - cw) * 0.5; } else cw = Math.Max(w, mw);
            if ((fv & 1) == 0) { ch = Math.Min(h, mh); if ((fv & 8) != 0) cy = y + h - ch; else if ((fv & 4) != 0) cy = y + (h - ch) * 0.5; } else ch = Math.Max(h, mh);
            k.PW = w; k.PH = h;
            SetRect(k, cx, cy, Math.Max(cw, mw), Math.Max(ch, mh));
            LayoutChildren(k);
        }

        static string Group(string t)
        {
            switch (t)
            {
                case "VBoxContainer": return "vbox"; case "HBoxContainer": return "hbox"; case "BoxContainer": return "hbox";
                case "GridContainer": return "grid"; case "HFlowContainer": case "FlowContainer": return "flow"; case "VFlowContainer": return "vflow";
                case "PanelContainer": return "panelc"; case "MarginContainer": return "margin"; case "ScrollContainer": return "scroll"; case "CenterContainer": return "center";
                case "SubViewportContainer": return "svc"; case "AspectRatioContainer": case "ConfirmationDialog": case "AcceptDialog": return "panelc";
            }
            return null;
        }

        static void LayoutChildren(GdNode n)
        {
            double w, h; Vec(n, "size", out w, out h);
            var g = Group(n.TypeName);
            if (g == null)
            {
                foreach (var c in n.Children) if (!c.QueuedFree && IsCtl(c) && VisibleSelf(c)) LayoutFree(c, w, h);
                if (n.TypeName == "ConfirmationDialog" || n.TypeName == "AcceptDialog") { }
                return;
            }
            var kids = Kids(n);
            // ovládací prvky s top_level se skládají volně
            foreach (var c in n.Children) if (!c.QueuedFree && IsCtl(c) && VisibleSelf(c) && Flag(c, "top_level", false)) LayoutFree(c, w, h);
            switch (g)
            {
                case "vbox": case "hbox": LayoutBox(n, kids, g == "vbox", w, h); break;
                case "grid": LayoutGrid(n, kids, w, h); break;
                case "flow": LayoutFlow(n, kids, w, h); break;
                case "panelc": case "margin":
                    {
                        double ml, mt, mr, mb; ContentMargins(n, out ml, out mt, out mr, out mb);
                        foreach (var k in kids) FitChild(k, ml, mt, w - ml - mr, h - mt - mb);
                        break;
                    }
                case "center": foreach (var k in kids) { double kw, kh; MinSize(k, out kw, out kh); k.PW = w; k.PH = h; SetRect(k, (w - kw) * 0.5, (h - kh) * 0.5, kw, kh); LayoutChildren(k); } break;
                case "scroll": LayoutScroll(n, kids, w, h); break;
                case "svc":
                    foreach (var k in kids) { k.PW = w; k.PH = h; SetRect(k, 0, 0, w, h); LayoutChildren(k); }
                    foreach (var c in n.Children) if (c.TypeName == "SubViewport" && Flag(n, "stretch", false)) c.SetRaw("size", Gv.Veci(Math.Max(1, w), Math.Max(1, h)));
                    break;
            }
        }

        static double Sep(GdNode n, string name, double def)
        {
            var v = n.Raw("ovr:constant/" + name); if (v.IsNum) return v.d;
            var t = ThemeGet(n, "constant", n.TypeName, name); if (t.IsNum) return t.d;
            return def;
        }

        static void LayoutBox(GdNode n, List<GdNode> kids, bool vertical, double w, double h)
        {
            double sep = Sep(n, "separation", 4);
            int cnt = kids.Count; if (cnt == 0) return;
            var mn = new double[cnt]; var mc = new double[cnt]; var fl = new int[cnt];
            double total = 0; int expanders = 0;
            for (int i = 0; i < cnt; i++)
            {
                double kw, kh; MinSize(kids[i], out kw, out kh);
                mn[i] = vertical ? kh : kw; mc[i] = vertical ? kw : kh;
                fl[i] = (int)Num(kids[i], vertical ? "size_flags_vertical" : "size_flags_horizontal", 1);
                total += mn[i]; if ((fl[i] & 2) != 0) expanders++;
            }
            total += sep * (cnt - 1);
            double avail = vertical ? h : w, cross = vertical ? w : h;
            double extra = avail - total;
            var size = new double[cnt]; for (int i = 0; i < cnt; i++) size[i] = mn[i];
            if (extra > 0 && expanders > 0)
            {
                // rozdělení s respektováním stretch_ratio (výchozí 1)
                double sumRatio = 0; for (int i = 0; i < cnt; i++) if ((fl[i] & 2) != 0) sumRatio += Math.Max(0.0001, Num(kids[i], "size_flags_stretch_ratio", 1));
                for (int i = 0; i < cnt; i++) if ((fl[i] & 2) != 0) size[i] = mn[i] + extra * Math.Max(0.0001, Num(kids[i], "size_flags_stretch_ratio", 1)) / sumRatio;
            }
            double pos = 0;
            if (extra > 0 && expanders == 0) { int al = (int)Num(n, "alignment", 0); if (al == 1) pos = extra * 0.5; else if (al == 2) pos = extra; }
            for (int i = 0; i < cnt; i++)
            {
                var k = kids[i]; int cf = (int)Num(k, vertical ? "size_flags_horizontal" : "size_flags_vertical", 1);
                double cs = cross, cp = 0;
                if ((cf & 1) == 0) { cs = Math.Min(cross, mc[i]); if ((cf & 8) != 0) cp = cross - cs; else if ((cf & 4) != 0) cp = (cross - cs) * 0.5; }
                else cs = Math.Max(cross, mc[i]);
                cs = Math.Max(cs, mc[i]);
                k.PW = w; k.PH = h;
                if (vertical) SetRect(k, cp, pos, cs, size[i]); else SetRect(k, pos, cp, size[i], cs);
                LayoutChildren(k);
                pos += size[i] + sep;
            }
        }

        static void LayoutGrid(GdNode n, List<GdNode> kids, double w, double h)
        {
            int cols = Math.Max(1, (int)Num(n, "columns", 1)); double hs = Sep(n, "h_separation", 4), vs = Sep(n, "v_separation", 4);
            int rows = (kids.Count + cols - 1) / cols; if (rows == 0) return;
            var cw = new double[cols]; var rh = new double[rows]; var cexp = new bool[cols]; var rexp = new bool[rows];
            for (int i = 0; i < kids.Count; i++)
            {
                int c = i % cols, r = i / cols; double kw, kh; MinSize(kids[i], out kw, out kh);
                cw[c] = Math.Max(cw[c], kw); rh[r] = Math.Max(rh[r], kh);
                if (((int)Num(kids[i], "size_flags_horizontal", 1) & 2) != 0) cexp[c] = true;
                if (((int)Num(kids[i], "size_flags_vertical", 1) & 2) != 0) rexp[r] = true;
            }
            double tw = hs * (cols - 1), th = vs * (rows - 1); foreach (var v in cw) tw += v; foreach (var v in rh) th += v;
            int ce = 0, re = 0; foreach (var b in cexp) if (b) ce++; foreach (var b in rexp) if (b) re++;
            if (w > tw && ce > 0) for (int c = 0; c < cols; c++) if (cexp[c]) cw[c] += (w - tw) / ce;
            if (h > th && re > 0) for (int r = 0; r < rows; r++) if (rexp[r]) rh[r] += (h - th) / re;
            var cx = new double[cols]; double x = 0; for (int c = 0; c < cols; c++) { cx[c] = x; x += cw[c] + hs; }
            var ry = new double[rows]; double y = 0; for (int r = 0; r < rows; r++) { ry[r] = y; y += rh[r] + vs; }
            for (int i = 0; i < kids.Count; i++) { int c = i % cols, r = i / cols; FitChild(kids[i], cx[c], ry[r], cw[c], rh[r]); }
        }

        static void LayoutFlow(GdNode n, List<GdNode> kids, double w, double h)
        {
            double hs = Sep(n, "h_separation", 4), vs = Sep(n, "v_separation", 4);
            double x = 0, y = 0, rowH = 0;
            foreach (var k in kids)
            {
                double kw, kh; MinSize(k, out kw, out kh);
                if (x > 0 && x + kw > w + 0.01) { x = 0; y += rowH + vs; rowH = 0; }
                k.PW = w; k.PH = h; SetRect(k, x, y, kw, kh); LayoutChildren(k);
                x += kw + hs; rowH = Math.Max(rowH, kh);
            }
        }

        public const double ScrollBarW = 10;

        static void LayoutScroll(GdNode n, List<GdNode> kids, double w, double h)
        {
            int hm = (int)Num(n, "horizontal_scroll_mode", 1), vm = (int)Num(n, "vertical_scroll_mode", 1);
            foreach (var k in kids)
            {
                double kw, kh; MinSize(k, out kw, out kh);
                bool vbar = vm != 0 && vm != 3 && (kh > h || vm == 2);
                double vw = w - (vbar && vm != 3 ? ScrollBarW : 0);
                double cw = hm == 0 ? vw : Math.Max(kw, vw);
                double ch = vm == 0 ? h : Math.Max(kh, h);
                if (hm == 0) cw = Math.Max(vw, 0);
                double maxScroll = Math.Max(0, ch - h);
                n.ScrollV = Math.Max(0, Math.Min(n.ScrollV, maxScroll)); n.MaxScrollV = maxScroll;
                double maxH = Math.Max(0, cw - vw); n.ScrollH = Math.Max(0, Math.Min(n.ScrollH, maxH)); n.MaxScrollH = maxH;
                k.PW = w; k.PH = h;
                SetRect(k, -n.ScrollH, -n.ScrollV, cw, ch);
                LayoutChildren(k);
                n.ContentH = ch; n.ContentW = cw;
            }
        }

        // ---------- minimální velikosti ----------
        public static void ContentMargins(GdNode n, out double l, out double t, out double r, out double b)
        {
            l = t = r = b = 0;
            if (n.TypeName == "MarginContainer")
            {
                l = Sep(n, "margin_left", 0); t = Sep(n, "margin_top", 0); r = Sep(n, "margin_right", 0); b = Sep(n, "margin_bottom", 0); return;
            }
            var sb = StyleOf(n, (n.TypeName == "PanelContainer" || n.TypeName == "ConfirmationDialog" || n.TypeName == "AcceptDialog") ? n.TypeName : "Panel", "panel");
            StyleMargins(sb, out l, out t, out r, out b);
        }

        public static void StyleMargins(GdObject sb, out double l, out double t, out double r, out double b)
        {
            l = t = r = b = 0; if (sb == null) return;
            l = Mg(sb, "left"); t = Mg(sb, "top"); r = Mg(sb, "right"); b = Mg(sb, "bottom");
        }
        static double Mg(GdObject sb, string side)
        {
            var v = sb.Get("content_margin_" + side); if (v.IsNum && v.d >= 0) return v.d;
            var bw = sb.Get("border_width_" + side); return bw.IsNum ? bw.d : 0;
        }

        public static void MinSize(GdNode n, out double w, out double h)
        {
            if (n.MinPass == Pass) { w = n.MinW; h = n.MinH; return; }
            double cx, cy; Vec(n, "custom_minimum_size", out cx, out cy);
            double kw = 0, kh = 0;
            ContentMin(n, out kw, out kh);
            w = Math.Max(cx, kw); h = Math.Max(cy, kh);
            n.MinPass = Pass; n.MinW = w; n.MinH = h;
        }

        static void ContentMin(GdNode n, out double w, out double h)
        {
            w = 0; h = 0;
            switch (n.TypeName)
            {
                case "Label": LabelMin(n, out w, out h); return;
                case "RichTextLabel": RichMin(n, out w, out h); return;
                case "Button": case "CheckBox": case "CheckButton": case "OptionButton": case "MenuButton": case "LinkButton": ButtonMin(n, out w, out h); return;
                case "LineEdit": case "SpinBox": { double fs = FontSize(n, "LineEdit"); double tw, th; TextSize("0000000", fs, out tw, out th); double ml, mt, mr, mb; StyleMargins(StyleOf(n, "LineEdit", "normal"), out ml, out mt, out mr, out mb); w = Math.Max(57, ml + mr + 24); h = th + mt + mb; if (n.TypeName == "SpinBox") w += 24; return; }
                case "ProgressBar": h = Flag(n, "show_percentage", true) ? 27 : 4; w = 0; return;
                case "HSlider": h = 16; return;
                case "VSlider": w = 16; return;
                case "TextureRect": { var t = n.Raw("texture").o as GdTexture; if (t != null) { w = t.w; h = t.h; } return; }
                case "HSeparator": h = 4; return;
                case "VSeparator": w = 4; return;
            }
            var g = Group(n.TypeName);
            if (g == null) return;
            var kids = Kids(n);
            switch (g)
            {
                case "vbox": case "hbox":
                    {
                        bool v = g == "vbox"; double sep = Sep(n, "separation", 4); double main = 0, cross = 0;
                        foreach (var k in kids) { double kw, kh; MinSize(k, out kw, out kh); main += v ? kh : kw; cross = Math.Max(cross, v ? kw : kh); }
                        if (kids.Count > 1) main += sep * (kids.Count - 1);
                        if (v) { w = cross; h = main; } else { w = main; h = cross; }
                        return;
                    }
                case "grid":
                    {
                        int cols = Math.Max(1, (int)Num(n, "columns", 1)); double hs = Sep(n, "h_separation", 4), vs = Sep(n, "v_separation", 4);
                        int rows = (kids.Count + cols - 1) / cols; var cw = new double[cols]; var rh = new double[Math.Max(1, rows)];
                        for (int i = 0; i < kids.Count; i++) { double kw, kh; MinSize(kids[i], out kw, out kh); cw[i % cols] = Math.Max(cw[i % cols], kw); rh[i / cols] = Math.Max(rh[i / cols], kh); }
                        foreach (var v in cw) w += v; foreach (var v in rh) h += v;
                        if (cols > 1) w += hs * (cols - 1); if (rows > 1) h += vs * (rows - 1);
                        return;
                    }
                case "flow":
                    {
                        double hs = Sep(n, "h_separation", 4), vs = Sep(n, "v_separation", 4);
                        double avail = Num(n, "_flow_w", 0); double pw0, ph0; Vec(n, "size", out pw0, out ph0); avail = pw0;
                        double x = 0, y = 0, rowH = 0, maxw = 0;
                        foreach (var k in kids)
                        {
                            double kw, kh; MinSize(k, out kw, out kh); maxw = Math.Max(maxw, kw);
                            if (x > 0 && avail > 0 && x + kw > avail + 0.01) { x = 0; y += rowH + vs; rowH = 0; }
                            x += kw + hs; rowH = Math.Max(rowH, kh);
                        }
                        w = maxw; h = y + rowH; return;
                    }
                case "panelc": case "margin":
                    {
                        double ml, mt, mr, mb; ContentMargins(n, out ml, out mt, out mr, out mb);
                        double mw = 0, mh = 0; foreach (var k in kids) { double kw, kh; MinSize(k, out kw, out kh); mw = Math.Max(mw, kw); mh = Math.Max(mh, kh); }
                        w = mw + ml + mr; h = mh + mt + mb; return;
                    }
                case "center": foreach (var k in kids) { double kw, kh; MinSize(k, out kw, out kh); w = Math.Max(w, kw); h = Math.Max(h, kh); } return;
                case "scroll":
                    {
                        int hm = (int)Num(n, "horizontal_scroll_mode", 1), vm = (int)Num(n, "vertical_scroll_mode", 1);
                        foreach (var k in kids) { double kw, kh; MinSize(k, out kw, out kh); if (hm == 0) w = Math.Max(w, kw); if (vm == 0) h = Math.Max(h, kh); }
                        return;
                    }
            }
        }
    }
}
