// Prvky: minimální velikosti textových prvků, RichTextLabel (BBCode), OptionButton, dialogy, fokus.
using System;
using System.Collections.Generic;
using System.Text;
using KK = Pivovar.Gd.K;

namespace Pivovar.Gd
{
    public sealed class RAtom { public string text; public double x, w, fs; public GdColor col; public bool bold; public double lh; }
    public sealed class RLine { public List<RAtom> atoms = new List<RAtom>(); public double w, h; public int align; public double y; }

    public static partial class GdUi
    {
        public static GdNode FocusNode;

        public static void SetFocus(GdNode n)
        {
            if (FocusNode == n) return;
            var old = FocusNode; FocusNode = n;
            if (old != null) { old.Focused = false; old.UiVer++; old.Sig("focus_exited").Emit(new Gv[0]); }
            if (n != null) { n.Focused = true; n.UiVer++; n.Sig("focus_entered").Emit(new Gv[0]); }
        }

        static string StrOf(GdNode n, string k) { var v = n.Raw(k); return v.k == KK.Str ? v.Str : (v.IsNull ? "" : v.ToStr()); }

        // ---------- Label ----------
        static void LabelMin(GdNode n, out double w, out double h)
        {
            string text = StrOf(n, "text"); double fs = FontSize(n, "Label");
            int wrap = (int)Num(n, "autowrap_mode", 0); int overrun = (int)Num(n, "text_overrun_behavior", 0);
            double cx, cy; Vec(n, "custom_minimum_size", out cx, out cy);
            double lh = LineH(fs);
            if (wrap > 0)
            {
                double basis = cx > 0 ? cx : Num0(n);
                n.WrapUsed = basis;
                if (basis <= 0) { w = 0; h = lh; return; }
                var lines = Wrap(text, fs, basis);
                w = 0; h = lines.Count * lh + 3 * (lines.Count - 1); return;
            }
            double tw, th; TextSize(text, fs, out tw, out th);
            w = overrun > 0 ? 0 : tw; h = Math.Max(th, lh);
            n.WrapUsed = n.Raw("size").d;
        }

        static double Num0(GdNode n) { var s = n.Raw("size"); return (s.k == KK.Vec2 || s.k == KK.Vec2i) ? s.d : 0; }

        // ---------- Button ----------
        static void ButtonMin(GdNode n, out double w, out double h)
        {
            string text = StrOf(n, "text"); double fs = FontSize(n, "Button");
            double tw, th; TextSize(text, fs, out tw, out th); if (text.Length == 0) { tw = 0; th = LineH(fs); }
            double ml, mt, mr, mb; StyleMargins(StyleOf(n, "Button", "normal"), out ml, out mt, out mr, out mb);
            w = tw + ml + mr; h = Math.Max(th, LineH(fs)) + mt + mb;
            if (n.TypeName == "CheckBox" || n.TypeName == "CheckButton") w += 24 + 4;
            if (n.TypeName == "OptionButton") { w += 24; var it = ItemsOf(n); double mx = 0; foreach (var g in it) { double iw, ih; TextSize(g.ToStr(), fs, out iw, out ih); mx = Math.Max(mx, iw); } w = Math.Max(w, mx + ml + mr + 24); }
        }

        // ---------- RichTextLabel ----------
        static void RichMin(GdNode n, out double w, out double h)
        {
            w = 0; h = 0;
            if (!Flag(n, "fit_content", false)) return;
            double cx, cy; Vec(n, "custom_minimum_size", out cx, out cy);
            double basis = cx > 0 ? cx : Num0(n);
            n.WrapUsed = basis;
            var lines = RichLines(n, basis > 0 ? basis : 100000);
            foreach (var l in lines) h += l.h;
        }

        public static List<RLine> RichLines(GdNode n, double width)
        {
            string text = StrOf(n, "text"); bool bb = Flag(n, "bbcode_enabled", false); double fs0 = FontSize(n, "RichTextLabel");
            string key = text + "|" + width.ToString("0.#") + "|" + fs0 + "|" + bb;
            if (n.RichKey == key && n.RichCache is List<RLine>) return (List<RLine>)n.RichCache;
            var col0 = ColorOf(n, "RichTextLabel", "default_color", 0.875, 0.875, 0.875, 1);
            var atoms = new List<object>();   // RAtom nebo "\n"
            var colStack = new List<GdColor> { col0 }; var fsStack = new List<double> { fs0 }; int bold = 0; var alignStack = new List<int> { 0 };
            var sb = new StringBuilder();
            Action flush = () => { };
            int i = 0; var curAlign = new List<int>();
            Action<string> emit = (s) =>
            {
                if (s.Length == 0) return;
                // rozdělit na slova (včetně mezery za slovem)
                int p = 0;
                while (p < s.Length)
                {
                    int q = p; while (q < s.Length && s[q] != ' ') q++; while (q < s.Length && s[q] == ' ') q++;
                    var a = new RAtom { text = s.Substring(p, q - p), col = colStack[colStack.Count - 1], fs = fsStack[fsStack.Count - 1], bold = bold > 0 };
                    atoms.Add(a); atoms.Add(alignStack[alignStack.Count - 1]); p = q;
                }
            };
            while (i < text.Length)
            {
                char c = text[i];
                if (c == '\n') { emit(sb.ToString()); sb.Length = 0; atoms.Add("\n"); i++; continue; }
                if (bb && c == '[')
                {
                    int e = text.IndexOf(']', i);
                    if (e > i)
                    {
                        string tag = text.Substring(i + 1, e - i - 1); string lt = tag.ToLowerInvariant();
                        string nm = lt, arg = null; int eq = tag.IndexOf('='); if (eq > 0) { nm = lt.Substring(0, eq); arg = tag.Substring(eq + 1); }
                        bool handled = true;
                        switch (nm)
                        {
                            case "b": emit(sb.ToString()); sb.Length = 0; bold++; break;
                            case "/b": emit(sb.ToString()); sb.Length = 0; bold = Math.Max(0, bold - 1); break;
                            case "i": case "/i": case "u": case "/u": case "s": case "/s": case "url": case "/url": case "code": case "/code": break;
                            case "color": emit(sb.ToString()); sb.Length = 0; colStack.Add(arg != null ? Gd.ParseColor(arg.Trim()) : col0); break;
                            case "/color": emit(sb.ToString()); sb.Length = 0; if (colStack.Count > 1) colStack.RemoveAt(colStack.Count - 1); break;
                            case "font_size": emit(sb.ToString()); sb.Length = 0; { double v; fsStack.Add(arg != null && double.TryParse(arg, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out v) ? v : fs0); } break;
                            case "/font_size": emit(sb.ToString()); sb.Length = 0; if (fsStack.Count > 1) fsStack.RemoveAt(fsStack.Count - 1); break;
                            case "center": emit(sb.ToString()); sb.Length = 0; alignStack.Add(1); break;
                            case "right": emit(sb.ToString()); sb.Length = 0; alignStack.Add(2); break;
                            case "/center": case "/right": emit(sb.ToString()); sb.Length = 0; if (alignStack.Count > 1) alignStack.RemoveAt(alignStack.Count - 1); break;
                            case "br": emit(sb.ToString()); sb.Length = 0; atoms.Add("\n"); break;
                            case "lb": sb.Append('['); break;
                            case "rb": sb.Append(']'); break;
                            default: handled = false; break;
                        }
                        if (handled) { i = e + 1; continue; }
                    }
                }
                sb.Append(c); i++;
            }
            emit(sb.ToString());
            // zalomení
            var lines = new List<RLine>(); var cur = new RLine(); double x = 0; bool any = false;
            Action<RLine> closeLine = (l) =>
            {
                double lh = 0; foreach (var a in l.atoms) lh = Math.Max(lh, LineH(a.fs));
                if (l.atoms.Count == 0) lh = LineH(fs0);
                l.h = lh; l.w = x; lines.Add(l);
            };
            for (int k = 0; k < atoms.Count; k++)
            {
                var o = atoms[k];
                if (o is string) { cur.align = cur.atoms.Count > 0 ? cur.align : 0; closeLine(cur); cur = new RLine(); x = 0; any = false; continue; }
                var a = o as RAtom; if (a == null) continue;
                int al = (int)atoms[k + 1]; k++;
                double tw, th; TextSize(a.text, a.fs, out tw, out th);
                double trimmed = tw; if (a.text.EndsWith(" ")) { double sw, sh; TextSize(a.text.TrimEnd(' '), a.fs, out sw, out sh); trimmed = sw; }
                if (any && x + trimmed > width + 0.01) { closeLine(cur); cur = new RLine(); x = 0; any = false; }
                if (!any) cur.align = al;
                a.x = x; a.w = tw; x += tw; cur.atoms.Add(a); any = true;
            }
            if (any || lines.Count == 0) closeLine(cur);
            double yy = 0; foreach (var l in lines) { l.y = yy; yy += l.h; }
            n.RichKey = key; n.RichCache = lines;
            return lines;
        }

        // ---------- OptionButton ----------
        public static List<Gv> ItemsOf(GdNode n)
        {
            var v = n.Raw("_items"); var a = v.Arr; if (a == null) { a = new GdArray(); n.SetRaw("_items", Gv.A(a)); }
            return a.l;
        }

        public static bool InvokeWidget(GdNode n, string name, Gv[] a, out Gv r)
        {
            r = Gv.Null;
            switch (n.TypeName)
            {
                case "OptionButton":
                    switch (name)
                    {
                        case "add_item": ItemsOf(n).Add(Gv.S(Gd.A(a, 0).ToStr())); if (n.Raw("selected").IsNull) n.SetRaw("selected", Gv.I(0)); Ver++; n.UiVer++; return true;
                        case "clear": ItemsOf(n).Clear(); n.SetRaw("selected", Gv.I(-1)); Ver++; n.UiVer++; return true;
                        case "select": n.SetRaw("selected", Gv.I((long)Gd.A(a, 0).d)); n.UiVer++; Ver++; return true;
                        case "get_selected": case "get_selected_id": r = Gv.I((long)Num(n, "selected", 0)); return true;
                        case "get_item_count": r = Gv.I(ItemsOf(n).Count); return true;
                        case "get_item_text": { int i = (int)Gd.A(a, 0).d; var it = ItemsOf(n); r = i >= 0 && i < it.Count ? it[i] : Gv.S(""); return true; }
                        case "set_item_disabled": case "set_item_text": case "set_item_icon": return true;
                    }
                    break;
                case "RichTextLabel":
                    switch (name)
                    {
                        case "clear": n.Set("text", Gv.S("")); return true;
                        case "append_text": case "add_text": n.Set("text", Gv.S(StrOf(n, "text") + Gd.A(a, 0).ToStr())); return true;
                        case "newline": n.Set("text", Gv.S(StrOf(n, "text") + "\n")); return true;
                        case "get_content_height": { double h = 0; foreach (var l in RichLines(n, Math.Max(1, Num0(n)))) h += l.h; r = Gv.I((long)h); return true; }
                        case "push_color": case "push_bold": case "pop": case "push_font_size": case "scroll_to_line": case "scroll_to_paragraph": return true;
                        case "get_v_scroll_bar": r = new GdNode().Self; return true;
                    }
                    break;
                case "LineEdit": case "SpinBox":
                    switch (name)
                    {
                        case "select_all": case "select": case "deselect": return true;
                        case "clear": n.Set("text", Gv.S("")); return true;
                        case "get_line_edit": r = n.Self; return true;
                        case "set_value_no_signal": n.Set("value", Gd.A(a, 0)); return true;
                    }
                    break;
                case "ScrollContainer":
                    switch (name)
                    {
                        case "get_v_scroll_bar": case "get_h_scroll_bar": r = new GdNode().Self; return true;
                        case "ensure_control_visible": return true;
                    }
                    break;
                case "ConfirmationDialog": case "AcceptDialog":
                    switch (name)
                    {
                        case "popup_centered": case "popup": case "popup_centered_ratio": case "popup_centered_clamped": ShowDialog(n); return true;
                        case "get_ok_button": r = EnsureDialog(n).Raw("_ok"); return true;
                        case "get_cancel_button": r = EnsureDialog(n).Raw("_cancel"); return true;
                        case "hide": n.Set("visible", Gv.B(false)); return true;
                    }
                    break;
            }
            switch (name)
            {
                case "set_value": n.Set("value", Gd.A(a, 0)); return true;
                case "get_value": r = n.Get("value"); return true;
                case "is_pressed": r = Gv.B(Flag(n, "button_pressed", false)); return true;
            }
            return false;
        }

        // ---------- dialog (ConfirmationDialog / AcceptDialog) ----------
        static GdNode EnsureDialog(GdNode d)
        {
            if (d.Raw("_built").T()) return d;
            d.SetRaw("_built", Gv.B(true));
            d.SetRaw("visible", Gv.B(false)); d.SetRaw("z_index", Gv.I(200)); d.SetRaw("mouse_filter", Gv.I(0));
            var vb = new GdNode(); vb.TypeName = "VBoxContainer"; vb.SetRaw("ovr:constant/separation", Gv.I(12));
            var lbl = new GdNode(); lbl.TypeName = "Label"; lbl.SetRaw("text", Gv.S(StrOf(d, "dialog_text")));
            var hb = new GdNode(); hb.TypeName = "HBoxContainer"; hb.SetRaw("alignment", Gv.I(2)); hb.SetRaw("ovr:constant/separation", Gv.I(8));
            var ok = new GdNode(); ok.TypeName = "Button"; ok.SetRaw("text", Gv.S("OK"));
            var cancel = new GdNode(); cancel.TypeName = "Button"; cancel.SetRaw("text", Gv.S("Zrušit"));
            var title = new GdNode(); title.TypeName = "Label"; title.SetRaw("ovr:color/font_color", Gv.Col(0.95, 0.76, 0.19, 1));
            d.SetRaw("_title", title.Self); d.SetRaw("_text", lbl.Self); d.SetRaw("_ok", ok.Self); d.SetRaw("_cancel", cancel.Self);
            Action<GdNode, GdNode> add = (p, c) => { p.Children.Add(c); c.Parent = p; };
            add(vb, title); add(vb, lbl); add(hb, cancel); add(hb, ok); add(vb, hb); add(d, vb);
            ok.Sig("pressed").Connect(Gv.Fn(x => { d.Set("visible", Gv.B(false)); d.Sig("confirmed").Emit(new Gv[0]); return Gv.Null; }));
            cancel.Sig("pressed").Connect(Gv.Fn(x => { d.Set("visible", Gv.B(false)); d.Sig("canceled").Emit(new Gv[0]); return Gv.Null; }));
            Ver++; return d;
        }

        public static void ShowDialog(GdNode d)
        {
            EnsureDialog(d);
            ((GdNode)d.Raw("_text").Obj).Set("text", Gv.S(StrOf(d, "dialog_text")));
            ((GdNode)d.Raw("_title").Obj).Set("text", Gv.S(StrOf(d, "title")));
            string okt = StrOf(d, "ok_button_text"), ct = StrOf(d, "cancel_button_text");
            ((GdNode)d.Raw("_ok").Obj).Set("text", Gv.S(okt.Length > 0 ? okt : "OK"));
            var cb = (GdNode)d.Raw("_cancel").Obj; cb.Set("text", Gv.S(ct.Length > 0 ? ct : "Zrušit"));
            cb.Set("visible", Gv.B(d.TypeName == "ConfirmationDialog"));
            d.AnchorSet = true; SetPreset(d, 8, false);
            d.Set("grow_horizontal", Gv.I(2)); d.Set("grow_vertical", Gv.I(2));
            d.Set("offset_left", Gv.F(0)); d.Set("offset_right", Gv.F(0)); d.Set("offset_top", Gv.F(0)); d.Set("offset_bottom", Gv.F(0));
            d.Set("custom_minimum_size", Gv.Vec(360, 0));
            d.Set("visible", Gv.B(true));
        }
    }
}
