// Záznam kreslicích příkazů uzlů (draw_line, draw_rect …) – nezávislý na Unity; renderér si příkazy převede na trojúhelníky.
using System;
using System.Collections.Generic;
using KK = Pivovar.Gd.K;

namespace Pivovar.Gd
{
    public enum DK : byte { Line, Rect, Circle, Arc, Poly, Polyline, Multiline, Str, StrOutline, Tex, StyleBox, Prim }

    public sealed class DrawCmd
    {
        public DK k;
        public double[] xy;          // body (x,y,x,y…)
        public double[] cols;        // barvy po bodech (r,g,b,a…) nebo null
        public double[] uv;          // uv po bodech nebo null
        public double[] p;           // další čísla (podle druhu)
        public GdColor col;
        public double w = -1;
        public bool filled = true, aa;
        public string text; public double fs = 16; public int align; public double maxw = -1;
        public GdObject tex;         // GdTexture
        public GdObject sb;          // StyleBox (GdNode s vlastnostmi)
        public double[] m;           // transformace (a,b,c,d,tx,ty) nebo null
    }

    /// <summary>Textura (Texture2D / ImageTexture / Image) – drží velikost a případně pixely; Unity si k ní přidá nativní objekt.</summary>
    public sealed class GdTexture : GdObject
    {
        public string path; public int w, h; public float[] rgba;   // rgba v pořadí r,g,b,a po řádcích, 0..1 (null = ze souboru)
        public object Native; public int Version;
        public bool Repeat;
        public override string ScriptName { get { return "Texture2D"; } }
        public override bool IsA(string t) { return t == "Texture2D" || t == "ImageTexture" || t == "Image" || t == "Resource" || base.IsA(t); }
        public override Gv Get(string n)
        {
            switch (n) { case "size": return Gv.Veci(w, h); case "resource_path": return Gv.S(path ?? ""); }
            return base.Get(n);
        }
        public override Gv Invoke(string n, Gv[] a)
        {
            switch (n)
            {
                case "get_width": return Gv.I(w);
                case "get_height": return Gv.I(h);
                case "get_size": return Gv.Veci(w, h);
                case "get_image": return Self;
                case "generate_mipmaps": case "save_png": case "convert": case "resize": case "set_data": case "update": return Gv.Null;
                case "fill":
                    {
                        EnsurePixels(); var c = Gd.A(a, 0).Cl; if (c == null) return Gv.Null;
                        for (int i = 0; i < w * h; i++) { rgba[i * 4] = (float)c.r; rgba[i * 4 + 1] = (float)c.g; rgba[i * 4 + 2] = (float)c.b; rgba[i * 4 + 3] = (float)c.a; }
                        Version++; return Gv.Null;
                    }
                case "set_pixel":
                    {
                        EnsurePixels(); int x = (int)Gd.A(a, 0).d, y = (int)Gd.A(a, 1).d; var c = Gd.A(a, 2).Cl;
                        if (c == null || x < 0 || y < 0 || x >= w || y >= h) return Gv.Null;
                        int i = (y * w + x) * 4; rgba[i] = (float)c.r; rgba[i + 1] = (float)c.g; rgba[i + 2] = (float)c.b; rgba[i + 3] = (float)c.a; Version++; return Gv.Null;
                    }
                case "set_pixelv":
                    {
                        EnsurePixels(); var pos = Gd.A(a, 0); int x = (int)pos.d, y = (int)pos.e; var c = Gd.A(a, 1).Cl;
                        if (c == null || x < 0 || y < 0 || x >= w || y >= h) return Gv.Null;
                        int i = (y * w + x) * 4; rgba[i] = (float)c.r; rgba[i + 1] = (float)c.g; rgba[i + 2] = (float)c.b; rgba[i + 3] = (float)c.a; Version++; return Gv.Null;
                    }
                case "get_pixel":
                    {
                        EnsurePixels(); int x = (int)Gd.A(a, 0).d, y = (int)Gd.A(a, 1).d;
                        if (x < 0 || y < 0 || x >= w || y >= h) return Gv.Col(0, 0, 0, 0);
                        int i = (y * w + x) * 4; return Gv.Col(rgba[i], rgba[i + 1], rgba[i + 2], rgba[i + 3]);
                    }
                case "fill_rect":
                    {
                        EnsurePixels(); var r = Gd.A(a, 0).Rc; var c = Gd.A(a, 1).Cl; if (r == null || c == null) return Gv.Null;
                        for (int y = (int)r.y; y < (int)(r.y + r.h); y++) for (int x = (int)r.x; x < (int)(r.x + r.w); x++)
                            { if (x < 0 || y < 0 || x >= w || y >= h) continue; int i = (y * w + x) * 4; rgba[i] = (float)c.r; rgba[i + 1] = (float)c.g; rgba[i + 2] = (float)c.b; rgba[i + 3] = (float)c.a; }
                        Version++; return Gv.Null;
                    }
                case "duplicate": { var t = new GdTexture(); t.w = w; t.h = h; t.path = path; if (rgba != null) t.rgba = (float[])rgba.Clone(); return t.Self; }
            }
            return base.Invoke(n, a);
        }
        public void EnsurePixels() { if (rgba == null) rgba = new float[Math.Max(1, w * h) * 4]; }
        public static GdTexture Create(int w, int h) { var t = new GdTexture(); t.w = w; t.h = h; t.rgba = new float[Math.Max(1, w * h) * 4]; return t; }
    }

    public static class GdDrawRec
    {
        static double N(Gv[] a, int i, double def) { return i < a.Length && a[i].IsNum ? a[i].d : def; }
        static bool B(Gv[] a, int i, bool def) { return i < a.Length && a[i].k != KK.Null ? a[i].T() : def; }

        public static double[] Pts(Gv v)
        {
            if (v.k != KK.Arr) return new double[0];
            var l = v.Arr.l; var r = new double[l.Count * 2];
            for (int i = 0; i < l.Count; i++) { r[2 * i] = l[i].d; r[2 * i + 1] = l[i].e; }
            return r;
        }
        static double[] Cols(Gv v)
        {
            if (v.k != KK.Arr) return null;
            var l = v.Arr.l; var r = new double[l.Count * 4];
            for (int i = 0; i < l.Count; i++) { var c = l[i].Cl; if (c == null) continue; r[4 * i] = c.r; r[4 * i + 1] = c.g; r[4 * i + 2] = c.b; r[4 * i + 3] = c.a; }
            return r;
        }
        static GdColor C(Gv[] a, int i) { var c = i < a.Length ? a[i].Cl : null; return c ?? new GdColor(1, 1, 1, 1); }

        /// <summary>Zapíše příkaz do seznamu; vrací false, pokud metoda není kreslicí.</summary>
        public static bool Record(GdNode node, string n, Gv[] a)
        {
            var rec = node.rec; if (rec == null) return n.StartsWith("draw_");
            DrawCmd c = null;
            switch (n)
            {
                case "draw_set_transform":
                    {
                        var pos = a.Length > 0 ? a[0] : Gv.Vec(0, 0); double rot = N(a, 1, 0); var sc = a.Length > 2 && a[2].k == KK.Vec2 ? a[2] : Gv.Vec(1, 1);
                        double cs = Math.Cos(rot), sn = Math.Sin(rot);
                        node.curM = (pos.d == 0 && pos.e == 0 && rot == 0 && sc.d == 1 && sc.e == 1) ? null : new double[] { cs * sc.d, sn * sc.d, -sn * sc.e, cs * sc.e, pos.d, pos.e };
                        return true;
                    }
                case "draw_line":
                    c = new DrawCmd { k = DK.Line, xy = new double[] { a[0].d, a[0].e, a[1].d, a[1].e }, col = C(a, 2), w = N(a, 3, -1), aa = B(a, 4, false) }; break;
                case "draw_rect":
                    { var r = a[0].Rc; c = new DrawCmd { k = DK.Rect, xy = new double[] { r.x, r.y, r.x + r.w, r.y + r.h }, col = C(a, 1), filled = B(a, 2, true), w = N(a, 3, -1), aa = B(a, 4, false) }; break; }
                case "draw_circle":
                    c = new DrawCmd { k = DK.Circle, xy = new double[] { a[0].d, a[0].e }, p = new double[] { a[1].d }, col = C(a, 2), filled = B(a, 3, true), w = N(a, 4, -1), aa = B(a, 5, false) }; break;
                case "draw_arc":
                    c = new DrawCmd { k = DK.Arc, xy = new double[] { a[0].d, a[0].e }, p = new double[] { a[1].d, a[2].d, a[3].d, N(a, 4, 24) }, col = C(a, 5), w = N(a, 6, -1), aa = B(a, 7, false) }; break;
                case "draw_polyline":
                    c = new DrawCmd { k = DK.Polyline, xy = Pts(a[0]), col = C(a, 1), w = N(a, 2, -1), aa = B(a, 3, false) }; break;
                case "draw_polyline_colors":
                    c = new DrawCmd { k = DK.Polyline, xy = Pts(a[0]), cols = Cols(a[1]), col = new GdColor(1, 1, 1, 1), w = N(a, 2, -1), aa = B(a, 3, false) }; break;
                case "draw_multiline":
                    c = new DrawCmd { k = DK.Multiline, xy = Pts(a[0]), col = C(a, 1), w = N(a, 2, -1), aa = B(a, 3, false) }; break;
                case "draw_multiline_colors":
                    c = new DrawCmd { k = DK.Multiline, xy = Pts(a[0]), cols = Cols(a[1]), col = new GdColor(1, 1, 1, 1), w = N(a, 2, -1), aa = B(a, 3, false) }; break;
                case "draw_polygon":
                    c = new DrawCmd { k = DK.Poly, xy = Pts(a[0]), cols = Cols(a.Length > 1 ? a[1] : Gv.Null), col = new GdColor(1, 1, 1, 1), uv = a.Length > 2 && a[2].k == KK.Arr && a[2].Arr.Count > 0 ? Pts(a[2]) : null, tex = a.Length > 3 ? a[3].o as GdObject : null }; break;
                case "draw_colored_polygon":
                    c = new DrawCmd { k = DK.Poly, xy = Pts(a[0]), col = C(a, 1), uv = a.Length > 2 && a[2].k == KK.Arr && a[2].Arr.Count > 0 ? Pts(a[2]) : null, tex = a.Length > 3 ? a[3].o as GdObject : null }; break;
                case "draw_primitive":
                    c = new DrawCmd { k = DK.Prim, xy = Pts(a[0]), cols = Cols(a.Length > 1 ? a[1] : Gv.Null), col = new GdColor(1, 1, 1, 1), uv = a.Length > 2 && a[2].k == KK.Arr && a[2].Arr.Count > 0 ? Pts(a[2]) : null, tex = a.Length > 3 ? a[3].o as GdObject : null, w = N(a, 4, -1) }; break;
                case "draw_string":
                    {
                        string t = a.Length > 2 ? a[2].ToStr() : ""; if (t.Length == 0) return true;
                        c = new DrawCmd { k = DK.Str, xy = new double[] { a[1].d, a[1].e }, text = t, align = (int)N(a, 3, 0), maxw = N(a, 4, -1), fs = N(a, 5, 16), col = C(a, 6), tex = a.Length > 0 ? a[0].o as GdObject : null };
                        break;
                    }
                case "draw_string_outline":
                    {
                        string t = a.Length > 2 ? a[2].ToStr() : ""; if (t.Length == 0) return true;
                        c = new DrawCmd { k = DK.StrOutline, xy = new double[] { a[1].d, a[1].e }, text = t, align = (int)N(a, 3, 0), maxw = N(a, 4, -1), fs = N(a, 5, 16), w = N(a, 6, 1), col = C(a, 7), tex = a.Length > 0 ? a[0].o as GdObject : null };
                        break;
                    }
                case "draw_texture":
                    {
                        var t = a[0].o as GdTexture; if (t == null) return true;
                        c = new DrawCmd { k = DK.Tex, tex = t, xy = new double[] { a[1].d, a[1].e, a[1].d + t.w, a[1].e + t.h }, col = C(a, 2) }; break;
                    }
                case "draw_texture_rect":
                    {
                        var t = a[0].o as GdTexture; if (t == null) return true; var r = a[1].Rc;
                        c = new DrawCmd { k = DK.Tex, tex = t, xy = new double[] { r.x, r.y, r.x + r.w, r.y + r.h }, col = C(a, 3) };
                        if (B(a, 2, false)) c.p = new double[] { 1 };   // dlaždicování
                        break;
                    }
                case "draw_texture_rect_region":
                    {
                        var t = a[0].o as GdTexture; if (t == null) return true; var r = a[1].Rc; var s = a[2].Rc;
                        double iw = Math.Max(1, t.w), ih = Math.Max(1, t.h);
                        c = new DrawCmd { k = DK.Tex, tex = t, xy = new double[] { r.x, r.y, r.x + r.w, r.y + r.h }, col = C(a, 3), uv = new double[] { s.x / iw, s.y / ih, (s.x + s.w) / iw, (s.y + s.h) / ih } };
                        break;
                    }
                case "draw_style_box":
                    {
                        var r = a[1].Rc; c = new DrawCmd { k = DK.StyleBox, sb = a[0].o as GdObject, xy = new double[] { r.x, r.y, r.x + r.w, r.y + r.h }, col = new GdColor(1, 1, 1, 1) }; break;
                    }
                default:
                    return n.StartsWith("draw_");
            }
            if (c.xy == null) return true;
            c.m = node.curM;
            rec.Add(c);
            return true;
        }
    }
}
