// Motiv (theme): přepsání na uzlu, motiv předků, výchozí (tmavý) motiv Godotu; měření textu s mezipamětí.
using System;
using System.Collections.Generic;
using KK = Pivovar.Gd.K;

namespace Pivovar.Gd
{
    public static partial class GdUi
    {
        static readonly Dictionary<string, GdNode> defStyles = new Dictionary<string, GdNode>();

        public static string ThemeBase(string t)
        {
            switch (t)
            {
                case "CheckBox": case "CheckButton": case "OptionButton": case "MenuButton": case "LinkButton": return "Button";
                case "SpinBox": return "LineEdit";
                case "VBoxContainer": case "HBoxContainer": return "BoxContainer";
                case "HFlowContainer": case "VFlowContainer": return "FlowContainer";
            }
            return null;
        }

        /// <summary>Hodnota z motivu: přepsání na uzlu → motivy předků (typ, jeho základ) → null.</summary>
        public static Gv ThemeGet(GdNode n, string kind, string type, string name)
        {
            var v = n.Raw("ovr:" + kind + "/" + name); if (!v.IsNull) return v;
            for (var p = n; p != null; p = p.Parent)
            {
                var th = p.Raw("theme").Obj as GdNode; if (th == null) continue;
                v = th.Raw("theme:" + type + "/" + name); if (!v.IsNull) return v;
                var b = ThemeBase(type); if (b != null) { v = th.Raw("theme:" + b + "/" + name); if (!v.IsNull) return v; }
            }
            return Gv.Null;
        }

        public static GdColor ColorOf(GdNode n, string type, string name, double r, double g, double b, double a)
        {
            var v = ThemeGet(n, "color", type, name);
            if (v.Cl != null) return v.Cl;
            return new GdColor(r, g, b, a);
        }

        public static double FontSize(GdNode n, string type)
        {
            var v = ThemeGet(n, "font_size", type, "font_size"); if (v.IsNum) return v.d;
            for (var p = n; p != null; p = p.Parent)
            {
                var th = p.Raw("theme").Obj as GdNode; if (th == null) continue;
                var d = th.Raw("default_font_size"); if (d.IsNum && d.d > 0) return d.d;
            }
            return 16;
        }

        static GdNode Flat(string key, double r, double g, double b, double a, double ml, double mt, double mr, double mb, double rad = 0, double bw = 0, double br = 0, double bg2 = 0, double bb = 0, double ba = 0)
        {
            GdNode s;
            if (defStyles.TryGetValue(key, out s)) return s;
            s = new GdNode(); s.TypeName = "StyleBoxFlat";
            s.SetRaw("bg_color", Gv.Col(r, g, b, a));
            s.SetRaw("content_margin_left", Gv.F(ml)); s.SetRaw("content_margin_top", Gv.F(mt)); s.SetRaw("content_margin_right", Gv.F(mr)); s.SetRaw("content_margin_bottom", Gv.F(mb));
            foreach (var k in new[] { "top_left", "top_right", "bottom_right", "bottom_left" }) s.SetRaw("corner_radius_" + k, Gv.F(rad));
            if (bw > 0) { foreach (var k in new[] { "left", "top", "right", "bottom" }) s.SetRaw("border_width_" + k, Gv.F(bw)); s.SetRaw("border_color", Gv.Col(br, bg2, bb, ba)); }
            defStyles[key] = s; return s;
        }

        static GdNode Empty()
        {
            GdNode s; if (defStyles.TryGetValue("empty", out s)) return s;
            s = new GdNode(); s.TypeName = "StyleBoxEmpty"; s.SetRaw("draw_center", Gv.B(false)); defStyles["empty"] = s; return s;
        }

        static GdNode DefaultStyle(string type, string name)
        {
            switch (type)
            {
                case "Button":
                    switch (name)
                    {
                        case "normal": return Flat("bn", 0.1, 0.1, 0.1, 0.6, 8, 4, 8, 4);
                        case "hover": return Flat("bh", 0.225, 0.225, 0.225, 0.6, 8, 4, 8, 4);
                        case "pressed": return Flat("bp", 0, 0, 0, 0.6, 8, 4, 8, 4);
                        case "hover_pressed": return Flat("bhp", 0.1, 0.1, 0.1, 0.6, 8, 4, 8, 4);
                        case "disabled": return Flat("bd", 0.1, 0.1, 0.1, 0.3, 8, 4, 8, 4);
                        case "focus": return Empty();
                    }
                    break;
                case "Panel": case "PanelContainer": if (name == "panel") return Flat("pn", 0.1, 0.1, 0.1, 0.6, 0, 0, 0, 0); break;
                case "LineEdit": if (name == "normal" || name == "read_only") return Flat("le", 0.1, 0.1, 0.1, 0.6, 4, 4, 4, 4); if (name == "focus") return Empty(); break;
                case "ProgressBar": if (name == "background") return Flat("pbb", 0.1, 0.1, 0.1, 0.6, 0, 0, 0, 0); if (name == "fill") return Flat("pbf", 0.32, 0.55, 0.85, 1, 0, 0, 0, 0); break;
                case "HSlider": case "VSlider": if (name == "slider") return Flat("sl", 0.1, 0.1, 0.1, 0.6, 0, 0, 0, 0, 2); if (name == "grabber_area") return Flat("sga", 0.32, 0.55, 0.85, 1, 0, 0, 0, 0, 2); break;
                case "VScrollBar": case "HScrollBar": if (name == "scroll") return Flat("sb", 0.1, 0.1, 0.1, 0.6, 0, 0, 0, 0, 3); if (name == "grabber") return Flat("sg", 0.65, 0.65, 0.65, 0.6, 0, 0, 0, 0, 3); if (name == "grabber_highlight") return Flat("sgh", 0.8, 0.8, 0.8, 0.8, 0, 0, 0, 0, 3); if (name == "grabber_pressed") return Flat("sgp", 0.9, 0.9, 0.9, 1, 0, 0, 0, 0, 3); break;
                case "ConfirmationDialog": case "AcceptDialog": case "Window": if (name == "panel") return Flat("wd", 0.13, 0.15, 0.18, 1, 14, 10, 14, 10, 8, 1, 0.3, 0.33, 0.38, 1); break;
            }
            return Flat("none", 0, 0, 0, 0, 0, 0, 0, 0);
        }

        public static GdObject StyleOf(GdNode n, string type, string name)
        {
            var v = ThemeGet(n, "stylebox", type, name);
            if (v.Obj != null) return v.Obj;
            return DefaultStyle(type, name);
        }

        // ---------- měření textu ----------
        static readonly Dictionary<string, double[]> measureCache = new Dictionary<string, double[]>();

        public static double LineH(double fs)
        {
            double w, h; TextSize1("Ag", fs, out w, out h); return h;
        }

        static void TextSize1(string s, double fs, out double w, out double h)
        {
            string key = fs.ToString("R") + "|" + s; double[] r;
            if (measureCache.TryGetValue(key, out r)) { w = r[0]; h = r[1]; return; }
            if (Tess.Text != null) Tess.Text.Measure(s, fs, out w, out h); else { w = s.Length * fs * 0.52; h = fs * 1.2; }
            if (measureCache.Count > 20000) measureCache.Clear();
            measureCache[key] = new[] { w, h };
        }

        /// <summary>Velikost (i víceřádkového) textu bez zalamování.</summary>
        public static void TextSize(string s, double fs, out double w, out double h)
        {
            w = 0; h = 0;
            if (s == null) s = "";
            if (s.IndexOf('\n') < 0) { TextSize1(s, fs, out w, out h); return; }
            var lines = s.Split('\n'); double lh = LineH(fs);
            foreach (var l in lines) { double lw, hh; TextSize1(l, fs, out lw, out hh); w = Math.Max(w, lw); }
            h = lh * lines.Length + 3 * (lines.Length - 1);
        }

        public static double Ascent(double fs) { return Tess.Text != null ? Tess.Text.Ascent(fs) : fs * 0.95; }

        /// <summary>Zalomí text na šířku maxw (po slovech, dlouhá slova po znacích).</summary>
        public static List<string> Wrap(string text, double fs, double maxw)
        {
            var res = new List<string>();
            foreach (var para in (text ?? "").Split('\n'))
            {
                if (maxw <= 0) { res.Add(para); continue; }
                var words = para.Split(' '); string cur = "";
                foreach (var wd in words)
                {
                    string trial = cur.Length == 0 ? wd : cur + " " + wd; double tw, th; TextSize1(trial, fs, out tw, out th);
                    if (tw <= maxw || cur.Length == 0)
                    {
                        if (cur.Length == 0 && tw > maxw)
                        {   // dlouhé slovo – lámat po znacích
                            string part = "";
                            foreach (char ch in wd) { double pw, ph; TextSize1(part + ch, fs, out pw, out ph); if (pw > maxw && part.Length > 0) { res.Add(part); part = ch.ToString(); } else part += ch; }
                            cur = part;
                        }
                        else cur = trial;
                    }
                    else { res.Add(cur); cur = wd; }
                }
                res.Add(cur);
            }
            return res;
        }
    }
}
