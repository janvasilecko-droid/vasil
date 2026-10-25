// Kreslení ovládacích prvků (Panel, Label, Button, ProgressBar, posuvník, …) do kreslicích příkazů; výsledek je v mezipaměti uzlu.
using System;
using System.Collections.Generic;
using KK = Pivovar.Gd.K;

namespace Pivovar.Gd
{
    public static partial class GdUi
    {
        public static int ThemeVer;

        static void Box(List<DrawCmd> L, GdObject sb, double x, double y, double w, double h)
        {
            if (sb == null || w <= 0 || h <= 0) return;
            L.Add(new DrawCmd { k = DK.StyleBox, sb = sb, xy = new double[] { x, y, x + w, y + h }, col = new GdColor(1, 1, 1, 1) });
        }
        static void Fill(List<DrawCmd> L, double x, double y, double w, double h, GdColor c)
        {
            if (w <= 0 || h <= 0 || c.a <= 0) return;
            L.Add(new DrawCmd { k = DK.Rect, xy = new double[] { x, y, x + w, y + h }, col = c, filled = true });
        }
        static void Str(List<DrawCmd> L, string t, double x, double base_, double fs, GdColor c)
        {
            if (string.IsNullOrEmpty(t)) return;
            L.Add(new DrawCmd { k = DK.Str, xy = new double[] { x, base_ }, text = t, fs = fs, col = c, align = 0, maxw = -1 });
        }

        /// <summary>Trojúhelníky vlastního vzhledu prvku (pod potomky); null, pokud nic nekreslí.</summary>
        public static TessOut Paint(GdNode n)
        {
            int key = n.UiVer + ThemeVer * 1000003;
            if (n.UiMeshKey == key && n.UiMesh != null) return n.UiMesh;
            var L = new List<DrawCmd>();
            try { PaintInto(n, L); } catch (Exception ex) { Gd.Err("Paint " + n.TypeName, ex); }
            var t = new TessOut();
            foreach (var c in L) { try { Tess.Add(t, c); } catch (Exception ex) { Gd.Err("Tess " + c.k, ex); } }
            n.UiMesh = t.batches.Count > 0 ? t : null; n.UiMeshKey = key;
            return n.UiMesh;
        }

        /// <summary>Kreslení nad potomky (posuvníky).</summary>
        public static TessOut PaintPost(GdNode n)
        {
            if (n.TypeName != "ScrollContainer") return null;
            int key = n.UiVer + ThemeVer * 1000003 + (int)(n.ScrollV * 7);
            if (n.PostMeshKey == key && n.PostMesh != null) return n.PostMesh;
            var L = new List<DrawCmd>();
            double w, h; Vec(n, "size", out w, out h);
            int vm = (int)Num(n, "vertical_scroll_mode", 1);
            if (vm != 0 && vm != 3 && n.MaxScrollV > 0.5 && n.ContentH > 0)
            {
                double x = w - ScrollBarW;
                Box(L, StyleOf(n, "VScrollBar", "scroll"), x, 0, ScrollBarW, h);
                double gh = Math.Max(24, h * h / n.ContentH), gy = (h - gh) * (n.ScrollV / n.MaxScrollV);
                Box(L, StyleOf(n, "VScrollBar", n.Down ? "grabber_pressed" : "grabber"), x, gy, ScrollBarW, gh);
            }
            var t = new TessOut(); foreach (var c in L) Tess.Add(t, c);
            n.PostMesh = t.batches.Count > 0 ? t : null; n.PostMeshKey = key;
            return n.PostMesh;
        }

        static void PaintInto(GdNode n, List<DrawCmd> L)
        {
            double w, h; Vec(n, "size", out w, out h);
            switch (n.TypeName)
            {
                case "Panel": case "PanelContainer": Box(L, StyleOf(n, n.TypeName, "panel"), 0, 0, w, h); break;
                case "ConfirmationDialog": case "AcceptDialog": if (VisibleSelf(n)) Box(L, StyleOf(n, n.TypeName, "panel"), 0, 0, w, h); break;
                case "ColorRect": { var c = n.Raw("color").Cl ?? new GdColor(1, 1, 1, 1); Fill(L, 0, 0, w, h, c); break; }
                case "TextureRect": { var t = n.Raw("texture").o as GdTexture; if (t != null) L.Add(new DrawCmd { k = DK.Tex, tex = t, xy = new double[] { 0, 0, w, h }, col = new GdColor(1, 1, 1, 1) }); break; }
                case "Label": PaintLabel(n, L, w, h); break;
                case "RichTextLabel": PaintRich(n, L, w, h); break;
                case "Button": case "CheckBox": case "CheckButton": case "OptionButton": case "MenuButton": case "LinkButton": PaintButton(n, L, w, h); break;
                case "ProgressBar": PaintProgress(n, L, w, h); break;
                case "HSlider": PaintSlider(n, L, w, h); break;
                case "LineEdit": case "SpinBox": PaintEdit(n, L, w, h); break;
                case "HSeparator": Fill(L, 0, h * 0.5 - 0.5, w, 1, new GdColor(1, 1, 1, 0.2)); break;
                case "VSeparator": Fill(L, w * 0.5 - 0.5, 0, 1, h, new GdColor(1, 1, 1, 0.2)); break;
            }
        }

        static void PaintLabel(GdNode n, List<DrawCmd> L, double w, double h)
        {
            string text = StrOf(n, "text"); if (text.Length == 0) return;
            double fs = FontSize(n, "Label"); var col = ColorOf(n, "Label", "font_color", 1, 1, 1, 1);
            int wrap = (int)Num(n, "autowrap_mode", 0), ha = (int)Num(n, "horizontal_alignment", 0), va = (int)Num(n, "vertical_alignment", 0), overrun = (int)Num(n, "text_overrun_behavior", 0);
            var lines = wrap > 0 ? Wrap(text, fs, w) : new List<string>(text.Split('\n'));
            double lh = LineH(fs), total = lines.Count * lh + 3 * (lines.Count - 1);
            double y0 = va == 1 ? (h - total) * 0.5 : va == 2 ? h - total : 0, asc = Ascent(fs);
            for (int i = 0; i < lines.Count; i++)
            {
                string s = lines[i]; double tw, th; TextSize(s, fs, out tw, out th);
                if (overrun >= 3 && tw > w && w > 8)
                {
                    while (s.Length > 1 && tw > w) { s = s.Substring(0, s.Length - 1); TextSize(s + "…", fs, out tw, out th); }
                    s += "…";
                }
                double x = ha == 1 ? (w - tw) * 0.5 : ha == 2 ? w - tw : 0;
                Str(L, s, x, y0 + i * (lh + 3) + asc, fs, col);
            }
        }

        static void PaintRich(GdNode n, List<DrawCmd> L, double w, double h)
        {
            var lines = RichLines(n, Math.Max(1, w)); double asc;
            foreach (var l in lines)
            {
                double off = l.align == 1 ? (w - l.w) * 0.5 : l.align == 2 ? w - l.w : 0;
                foreach (var a in l.atoms)
                {
                    asc = Ascent(a.fs); double lhA = LineH(a.fs);
                    Str(L, a.text, off + a.x, l.y + (l.h - lhA) + asc, a.fs, a.col);
                    if (a.bold) Str(L, a.text, off + a.x + 0.6, l.y + (l.h - lhA) + asc, a.fs, a.col);
                }
            }
        }

        static void PaintButton(GdNode n, List<DrawCmd> L, double w, double h)
        {
            bool disabled = Flag(n, "disabled", false), toggled = Flag(n, "button_pressed", false);
            bool pressed = toggled || (n.Down && n.Hover);
            string st = disabled ? "disabled" : pressed ? (n.Hover ? "hover_pressed" : "pressed") : n.Hover ? "hover" : "normal";
            if (n.TypeName != "LinkButton" && !Flag(n, "flat", false) || (Flag(n, "flat", false) && (pressed || n.Hover)))
                Box(L, StyleOf(n, "Button", st), 0, 0, w, h);
            string fcn = disabled ? "font_disabled_color" : pressed ? (n.Hover ? "font_hover_pressed_color" : "font_pressed_color") : n.Hover ? "font_hover_color" : "font_color";
            double dr = disabled ? 0.875 * 0.5 : pressed ? 1 : n.Hover ? 0.95 : 0.875;
            var col = ColorOf(n, "Button", fcn, dr, dr, dr, 1);
            if (disabled && ThemeGet(n, "color", "Button", fcn).IsNull) col = new GdColor(0.875, 0.875, 0.875, 0.5);
            double fs = FontSize(n, "Button"); string text = StrOf(n, "text");
            if (n.TypeName == "OptionButton") { var it = ItemsOf(n); int sel = (int)Num(n, "selected", 0); text = sel >= 0 && sel < it.Count ? it[sel].ToStr() : ""; }
            double ml, mt, mr, mb; StyleMargins(StyleOf(n, "Button", st), out ml, out mt, out mr, out mb);
            double tw, th; TextSize(text, fs, out tw, out th);
            double lh = LineH(fs); double cx0 = ml, cw = w - ml - mr;
            if (n.TypeName == "CheckBox" || n.TypeName == "CheckButton")
            {
                double bs = 16, by = (h - bs) * 0.5;
                Box(L, CheckStyle(toggled), cx0, by, bs, bs);
                if (toggled) { double x0 = cx0 + 3.5, y0 = by + 8; L.Add(new DrawCmd { k = DK.Polyline, xy = new double[] { x0, y0, x0 + 3.2, y0 + 3.4, x0 + 8.5, y0 - 4 }, col = new GdColor(0.1, 0.12, 0.14, 1), w = 2 }); }
                cx0 += 24 + 4; cw -= 28;
            }
            int al = n.HasProp("alignment") ? (int)Num(n, "alignment", 1) : (n.TypeName == "OptionButton" || n.TypeName == "CheckBox" ? 0 : 1);
            if (n.TypeName == "OptionButton") cw -= 20;
            double x = al == 0 ? cx0 : al == 2 ? cx0 + cw - tw : cx0 + (cw - tw) * 0.5;
            if (text.Length > 0)
            {
                if (tw > cw && cw > 8 && n.TypeName != "Button") { }
                double ty = mt + (h - mt - mb - lh) * 0.5 + Ascent(fs);
                Str(L, text, x, ty, fs, col);
            }
            if (n.TypeName == "OptionButton")
            {
                double ax = w - mr - 12, ay = h * 0.5;
                L.Add(new DrawCmd { k = DK.Poly, xy = new double[] { ax, ay - 2, ax + 8, ay - 2, ax + 4, ay + 3 }, col = col });
            }
        }

        static GdNode CheckStyle(bool on)
        {
            GdNode s; string k = on ? "cko" : "ckf"; if (defStyles.TryGetValue(k, out s)) return s;
            s = new GdNode(); s.TypeName = "StyleBoxFlat";
            s.SetRaw("bg_color", on ? Gv.Col(0.95, 0.76, 0.19, 1) : Gv.Col(0.1, 0.1, 0.1, 0.8));
            foreach (var c in new[] { "top_left", "top_right", "bottom_right", "bottom_left" }) s.SetRaw("corner_radius_" + c, Gv.F(3));
            foreach (var c in new[] { "left", "top", "right", "bottom" }) s.SetRaw("border_width_" + c, Gv.F(1));
            s.SetRaw("border_color", Gv.Col(0.7, 0.7, 0.7, 1)); defStyles[k] = s; return s;
        }

        static void PaintProgress(GdNode n, List<DrawCmd> L, double w, double h)
        {
            double mn = Num(n, "min_value", 0), mx = Num(n, "max_value", 100), v = Num(n, "value", 0);
            double r = mx > mn ? Math.Max(0, Math.Min(1, (v - mn) / (mx - mn))) : 0;
            Box(L, StyleOf(n, "ProgressBar", "background"), 0, 0, w, h);
            Box(L, StyleOf(n, "ProgressBar", "fill"), 0, 0, w * r, h);
            if (Flag(n, "show_percentage", true))
            {
                double fs = FontSize(n, "ProgressBar"); var col = ColorOf(n, "ProgressBar", "font_color", 0.875, 0.875, 0.875, 1);
                string t = ((int)Math.Round(r * 100)) + " %"; double tw, th; TextSize(t, fs, out tw, out th);
                Str(L, t, (w - tw) * 0.5, (h - LineH(fs)) * 0.5 + Ascent(fs), fs, col);
            }
        }

        static void PaintSlider(GdNode n, List<DrawCmd> L, double w, double h)
        {
            double mn = Num(n, "min_value", 0), mx = Num(n, "max_value", 100), v = Num(n, "value", 0);
            double r = mx > mn ? Math.Max(0, Math.Min(1, (v - mn) / (mx - mn))) : 0;
            double th = 4, ty = (h - th) * 0.5;
            Box(L, StyleOf(n, "HSlider", "slider"), 0, ty, w, th);
            Box(L, StyleOf(n, "HSlider", "grabber_area"), 0, ty, Math.Max(th, w * r), th);
            double gx = 8 + (w - 16) * r;
            L.Add(new DrawCmd { k = DK.Circle, xy = new double[] { gx, h * 0.5 }, p = new double[] { 8 }, col = new GdColor(0.9, 0.9, 0.9, 1), filled = true });
        }

        static void PaintEdit(GdNode n, List<DrawCmd> L, double w, double h)
        {
            var sb = StyleOf(n, "LineEdit", "normal"); Box(L, sb, 0, 0, w, h);
            double ml, mt, mr, mb; StyleMargins(sb, out ml, out mt, out mr, out mb);
            double fs = FontSize(n, "LineEdit"); string text = n.TypeName == "SpinBox" ? SpinText(n) : StrOf(n, "text");
            var col = ColorOf(n, "LineEdit", "font_color", 0.875, 0.875, 0.875, 1);
            double lh = LineH(fs), by = mt + (h - mt - mb - lh) * 0.5 + Ascent(fs);
            if (text.Length == 0 && n.TypeName == "LineEdit") { string ph = StrOf(n, "placeholder_text"); Str(L, ph, ml, by, fs, new GdColor(col.r, col.g, col.b, 0.4)); }
            else Str(L, text, ml, by, fs, col);
            if (n.Focused)
            {
                double tw, th; TextSize(text, fs, out tw, out th);
                double cx = ml + tw + 1; if ((int)(GdHost.Time * 2) % 2 == 0) Fill(L, cx, mt, 1.5, h - mt - mb, new GdColor(1, 1, 1, 0.9));
                n.UiVer++;   // blikání
            }
        }

        public static string SpinText(GdNode n)
        {
            if (n.Focused && n.Raw("_edit").k == KK.Str) return n.Raw("_edit").Str;
            double v = Num(n, "value", 0), st = Num(n, "step", 1);
            int dec = st >= 1 || st == 0 ? 0 : Math.Min(6, (int)Math.Ceiling(-Math.Log10(st) - 1e-9));
            return v.ToString("F" + dec, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
