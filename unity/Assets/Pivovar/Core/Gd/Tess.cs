// Převod kreslicích příkazů na trojúhelníky (čistý C#, bez Unity). Výstup: dávky podle textury, vrcholy x,y,u,v,r,g,b,a.
using System;
using System.Collections.Generic;

namespace Pivovar.Gd
{
    public struct GlyphQuad { public float x0, y0, x1, y1, u0, v0, u1, v1; }

    /// <summary>Poskytovatel měření textu (v Unity z TextCore FontAsset); v testech není.</summary>
    public interface ITextProvider
    {
        /// <summary>Šířka a výška textu při velikosti písma size (v pixelech).</summary>
        void Measure(string text, double size, out double w, out double h);
        /// <summary>Vzdálenost od horního okraje řádku k základní čáře.</summary>
        double Ascent(double size);
    }

    public struct TextRun { public string text; public double x, y, size; public float r, g, b, a; }

    public sealed class TessBatch
    {
        public GdTexture tex;
        public List<TextRun> runs;                 // není-li null, jde o dávku textu (kreslí se jinak než trojúhelníky)
        public List<float> v = new List<float>();
        public List<int> idx = new List<int>();
        public int VCount { get { return v.Count / 8; } }
    }

    public sealed class TessOut
    {
        public List<TessBatch> batches = new List<TessBatch>();
        public TessBatch Get(GdTexture tex)
        {
            if (batches.Count > 0) { var l = batches[batches.Count - 1]; if (l.runs == null && ReferenceEquals(l.tex, tex)) return l; }
            var b = new TessBatch(); b.tex = tex; batches.Add(b); return b;
        }
        public TessBatch GetText()
        {
            if (batches.Count > 0) { var l = batches[batches.Count - 1]; if (l.runs != null) return l; }
            var b = new TessBatch(); b.runs = new List<TextRun>(); batches.Add(b); return b;
        }
        public int Tris { get { int n = 0; foreach (var b in batches) n += b.idx.Count / 3; return n; } }
    }

    public static class Tess
    {
        public static ITextProvider Text;

        struct P { public double x, y; }
        struct Cl { public float r, g, b, a; }

        static Cl C(GdColor c) { Cl o; o.r = (float)c.r; o.g = (float)c.g; o.b = (float)c.b; o.a = (float)c.a; return o; }
        static Cl Mul(Cl a, Cl b) { Cl o; o.r = a.r * b.r; o.g = a.g * b.g; o.b = a.b * b.b; o.a = a.a * b.a; return o; }

        static void Vtx(TessBatch b, double x, double y, double u, double v, Cl c)
        {
            b.v.Add((float)x); b.v.Add((float)y); b.v.Add((float)u); b.v.Add((float)v); b.v.Add(c.r); b.v.Add(c.g); b.v.Add(c.b); b.v.Add(c.a);
        }

        // ---------- hlavní vstup ----------
        public static void Add(TessOut o, DrawCmd c)
        {
            switch (c.k)
            {
                case DK.Line: AddLine(o, c); break;
                case DK.Rect: AddRect(o, c); break;
                case DK.Circle: AddCircle(o, c); break;
                case DK.Arc: AddArc(o, c); break;
                case DK.Poly: AddPoly(o, c); break;
                case DK.Polyline: AddPolyline(o, c); break;
                case DK.Multiline: AddMultiline(o, c); break;
                case DK.Str: AddStr(o, c, 0, 0); break;
                case DK.StrOutline: AddOutline(o, c); break;
                case DK.Tex: AddTex(o, c); break;
                case DK.StyleBox: AddStyleBox(o, c); break;
                case DK.Prim: AddPrim(o, c); break;
            }
        }

        static double Scale(DrawCmd c)
        {
            if (c.m == null) return 1.0;
            double det = Math.Abs(c.m[0] * c.m[3] - c.m[1] * c.m[2]);
            return Math.Sqrt(det);
        }
        static P Tr(DrawCmd c, double x, double y)
        {
            P p;
            if (c.m == null) { p.x = x; p.y = y; return p; }
            p.x = c.m[0] * x + c.m[2] * y + c.m[4]; p.y = c.m[1] * x + c.m[3] * y + c.m[5]; return p;
        }
        static List<P> TrPts(DrawCmd c, double[] xy)
        {
            var l = new List<P>(xy.Length / 2);
            for (int i = 0; i + 1 < xy.Length; i += 2) l.Add(Tr(c, xy[i], xy[i + 1]));
            return l;
        }
        static double LW(DrawCmd c) { double w = c.w <= 0 ? 1.0 : c.w; return w * Scale(c); }

        // ---------- základní tvary ----------
        static void Tri(TessBatch b, int a, int bb, int c) { b.idx.Add(a); b.idx.Add(bb); b.idx.Add(c); }

        static void Quad(TessBatch b, P p0, P p1, P p2, P p3, Cl c0, Cl c1, Cl c2, Cl c3, double u0, double v0, double u1, double v1)
        {
            int i = b.VCount;
            Vtx(b, p0.x, p0.y, u0, v0, c0); Vtx(b, p1.x, p1.y, u1, v0, c1); Vtx(b, p2.x, p2.y, u1, v1, c2); Vtx(b, p3.x, p3.y, u0, v1, c3);
            Tri(b, i, i + 1, i + 2); Tri(b, i, i + 2, i + 3);
        }

        static void FillConvex(TessBatch b, List<P> pts, Cl col)
        {
            if (pts.Count < 3) return;
            double cx = 0, cy = 0; foreach (var p in pts) { cx += p.x; cy += p.y; }
            cx /= pts.Count; cy /= pts.Count;
            int c0 = b.VCount; Vtx(b, cx, cy, 0, 0, col);
            for (int i = 0; i < pts.Count; i++) Vtx(b, pts[i].x, pts[i].y, 0, 0, col);
            for (int i = 0; i < pts.Count; i++) Tri(b, c0, c0 + 1 + i, c0 + 1 + (i + 1) % pts.Count);
        }

        static void AddLine(TessOut o, DrawCmd c)
        {
            var a = Tr(c, c.xy[0], c.xy[1]); var bp = Tr(c, c.xy[2], c.xy[3]);
            StrokeSeg(o.Get(null), a, bp, LW(c), C(c.col), C(c.col));
        }

        static void StrokeSeg(TessBatch b, P a, P bp, double w, Cl ca, Cl cb)
        {
            double dx = bp.x - a.x, dy = bp.y - a.y, l = Math.Sqrt(dx * dx + dy * dy);
            if (l < 1e-9) return;
            double h = w * 0.5, nx = -dy / l * h, ny = dx / l * h;
            Quad(b, new P { x = a.x + nx, y = a.y + ny }, new P { x = bp.x + nx, y = bp.y + ny }, new P { x = bp.x - nx, y = bp.y - ny }, new P { x = a.x - nx, y = a.y - ny }, ca, cb, cb, ca, 0, 0, 0, 0);
        }

        static void AddMultiline(TessOut o, DrawCmd c)
        {
            var b = o.Get(null); var pts = TrPts(c, c.xy); double w = LW(c);
            for (int i = 0; i + 1 < pts.Count; i += 2)
            {
                Cl c0 = c.cols != null ? ColAt(c.cols, i) : C(c.col), c1 = c.cols != null ? ColAt(c.cols, i + 1) : C(c.col);
                StrokeSeg(b, pts[i], pts[i + 1], w, c0, c1);
            }
        }

        static Cl ColAt(double[] cols, int i)
        {
            Cl o; int k = Math.Min(i, cols.Length / 4 - 1) * 4;
            if (k < 0) { o.r = o.g = o.b = o.a = 1; return o; }
            o.r = (float)cols[k]; o.g = (float)cols[k + 1]; o.b = (float)cols[k + 2]; o.a = (float)cols[k + 3]; return o;
        }

        static void AddRect(TessOut o, DrawCmd c)
        {
            var b = o.Get(null);
            double x0 = c.xy[0], y0 = c.xy[1], x1 = c.xy[2], y1 = c.xy[3];
            if (c.filled)
            {
                var col = C(c.col);
                Quad(b, Tr(c, x0, y0), Tr(c, x1, y0), Tr(c, x1, y1), Tr(c, x0, y1), col, col, col, col, 0, 0, 0, 0);
            }
            else
            {
                var pts = new List<P> { Tr(c, x0, y0), Tr(c, x1, y0), Tr(c, x1, y1), Tr(c, x0, y1) };
                StrokePoly(b, pts, true, LW(c), null, C(c.col));
            }
        }

        static int CircSeg(double r)
        {
            int n = (int)(8 + Math.Sqrt(Math.Abs(r)) * 5.0);
            return n < 12 ? 12 : (n > 140 ? 140 : n);
        }

        static void AddCircle(TessOut o, DrawCmd c)
        {
            var b = o.Get(null); double r = c.p[0] * Scale(c); var ctr = Tr(c, c.xy[0], c.xy[1]);
            if (r <= 0) return;
            int n = CircSeg(r); var pts = new List<P>(n);
            for (int i = 0; i < n; i++) { double a = i * 2 * Math.PI / n; pts.Add(new P { x = ctr.x + Math.Cos(a) * r, y = ctr.y + Math.Sin(a) * r }); }
            if (c.filled) { FillConvex(b, pts, C(c.col)); }
            else StrokePoly(b, pts, true, LW(c), null, C(c.col));
        }

        static void AddArc(TessOut o, DrawCmd c)
        {
            var b = o.Get(null); double r = c.p[0] * Scale(c); var ctr = Tr(c, c.xy[0], c.xy[1]);
            double a0 = c.p[1], a1 = c.p[2]; int n = (int)Math.Max(2, Math.Min(240, c.p[3]));
            var pts = new List<P>(n + 1);
            for (int i = 0; i <= n; i++) { double a = a0 + (a1 - a0) * i / n; pts.Add(new P { x = ctr.x + Math.Cos(a) * r, y = ctr.y + Math.Sin(a) * r }); }
            StrokePoly(b, pts, false, LW(c), null, C(c.col));
        }

        static void AddPolyline(TessOut o, DrawCmd c)
        {
            var pts = TrPts(c, c.xy);
            bool closed = pts.Count > 2 && Math.Abs(pts[0].x - pts[pts.Count - 1].x) < 1e-9 && Math.Abs(pts[0].y - pts[pts.Count - 1].y) < 1e-9;
            StrokePoly(o.Get(null), pts, false, LW(c), c.cols, C(c.col));
        }

        // ---------- tah čáry se spoji ----------
        static void StrokePoly(TessBatch b, List<P> src, bool closed, double w, double[] cols, Cl col)
        {
            // odstranění duplicitních bodů
            var P_ = new List<P>(src.Count); var ci = new List<int>(src.Count);
            for (int i = 0; i < src.Count; i++)
            {
                if (P_.Count > 0 && Math.Abs(P_[P_.Count - 1].x - src[i].x) < 1e-9 && Math.Abs(P_[P_.Count - 1].y - src[i].y) < 1e-9) continue;
                P_.Add(src[i]); ci.Add(i);
            }
            if (closed && P_.Count > 1 && Math.Abs(P_[0].x - P_[P_.Count - 1].x) < 1e-9 && Math.Abs(P_[0].y - P_[P_.Count - 1].y) < 1e-9) { P_.RemoveAt(P_.Count - 1); ci.RemoveAt(ci.Count - 1); }
            int m = P_.Count; if (m < 2) return;
            int ns = closed ? m : m - 1;
            double h = w * 0.5;
            var dx = new double[ns]; var dy = new double[ns]; var ln = new double[ns];
            for (int i = 0; i < ns; i++)
            {
                var a = P_[i]; var bb = P_[(i + 1) % m];
                double ddx = bb.x - a.x, ddy = bb.y - a.y, l = Math.Sqrt(ddx * ddx + ddy * ddy);
                ln[i] = l; dx[i] = ddx / l; dy[i] = ddy / l;
            }
            // krajní body segmentů: SL,SR (začátek), EL,ER (konec)
            var SL = new P[ns]; var SR = new P[ns]; var EL = new P[ns]; var ER = new P[ns];
            for (int i = 0; i < ns; i++)
            {
                double nx = -dy[i] * h, ny = dx[i] * h; var a = P_[i]; var bb = P_[(i + 1) % m];
                SL[i] = new P { x = a.x + nx, y = a.y + ny }; SR[i] = new P { x = a.x - nx, y = a.y - ny };
                EL[i] = new P { x = bb.x + nx, y = bb.y + ny }; ER[i] = new P { x = bb.x - nx, y = bb.y - ny };
            }
            int nj = closed ? ns : ns - 1;
            for (int j = 0; j < nj; j++)
            {
                int i0 = j, i1 = (j + 1) % ns; var p = P_[(j + 1) % m];
                double cross = dx[i0] * dy[i1] - dy[i0] * dx[i1], dot = dx[i0] * dx[i1] + dy[i0] * dy[i1];
                if (Math.Abs(cross) < 1e-7 && dot > 0) continue;
                double n0x = -dy[i0], n0y = dx[i0], n1x = -dy[i1], n1y = dx[i1];
                double mx = n0x + n1x, my = n0y + n1y, ml = Math.Sqrt(mx * mx + my * my);
                bool miter = false;
                if (ml > 1e-9)
                {
                    mx /= ml; my /= ml;
                    double cosh = mx * n0x + my * n0y;       // cos(φ/2)
                    double mlen = cosh > 1e-6 ? h / cosh : 1e9;
                    double ext = h * Math.Abs(cross) / (1.0 + dot);
                    if (mlen <= h * 3.0 && ext <= Math.Min(ln[i0], ln[i1]) && dot > -0.8)
                    {
                        miter = true;
                        var L = new P { x = p.x + mx * mlen, y = p.y + my * mlen }; var R = new P { x = p.x - mx * mlen, y = p.y - my * mlen };
                        EL[i0] = L; SL[i1] = L; ER[i0] = R; SR[i1] = R;
                    }
                }
                if (!miter)
                {
                    // zkosení vnější strany: trojúhelník (p, konec předchozího, začátek dalšího)
                    int vi = b.VCount; Cl cc = JointCol(cols, ci, (j + 1) % m, col);
                    if (cross > 0) { Vtx(b, p.x, p.y, 0, 0, cc); Vtx(b, ER[i0].x, ER[i0].y, 0, 0, cc); Vtx(b, SR[i1].x, SR[i1].y, 0, 0, cc); }
                    else { Vtx(b, p.x, p.y, 0, 0, cc); Vtx(b, EL[i0].x, EL[i0].y, 0, 0, cc); Vtx(b, SL[i1].x, SL[i1].y, 0, 0, cc); }
                    Tri(b, vi, vi + 1, vi + 2);
                }
            }
            for (int i = 0; i < ns; i++)
            {
                Cl c0 = JointCol(cols, ci, i, col), c1 = JointCol(cols, ci, (i + 1) % m, col);
                Quad(b, SL[i], EL[i], ER[i], SR[i], c0, c1, c1, c0, 0, 0, 0, 0);
            }
        }

        static Cl JointCol(double[] cols, List<int> ci, int k, Cl def)
        {
            if (cols == null) return def;
            return ColAt(cols, ci[k]);
        }

        // ---------- výplň polygonu ----------
        static void AddPoly(TessOut o, DrawCmd c)
        {
            int n = c.xy.Length / 2; if (n < 3) return;
            var pts = TrPts(c, c.xy);
            GdTexture tex = c.tex as GdTexture;
            var b = o.Get(tex);
            int b0 = b.VCount;
            for (int i = 0; i < n; i++)
            {
                Cl col = c.cols != null ? ColAt(c.cols, i) : C(c.col);
                double u = 0, v = 0;
                if (c.uv != null && i * 2 + 1 < c.uv.Length) { u = c.uv[2 * i]; v = c.uv[2 * i + 1]; if (tex != null && (u < -0.001 || u > 1.001 || v < -0.001 || v > 1.001)) tex.Repeat = true; }
                Vtx(b, pts[i].x, pts[i].y, u, v, col);
            }
            var idx = Triangulate(pts);
            foreach (int k in idx) b.idx.Add(b0 + k);
        }

        static void AddPrim(TessOut o, DrawCmd c)
        {
            int n = c.xy.Length / 2; var pts = TrPts(c, c.xy);
            GdTexture tex = c.tex as GdTexture; var b = o.Get(tex);
            if (n == 2) { StrokeSeg(b, pts[0], pts[1], LW(c), c.cols != null ? ColAt(c.cols, 0) : C(c.col), c.cols != null ? ColAt(c.cols, 1) : C(c.col)); return; }
            int b0 = b.VCount;
            for (int i = 0; i < n; i++)
            {
                double u = c.uv != null && 2 * i + 1 < c.uv.Length ? c.uv[2 * i] : 0, v = c.uv != null && 2 * i + 1 < c.uv.Length ? c.uv[2 * i + 1] : 0;
                Vtx(b, pts[i].x, pts[i].y, u, v, c.cols != null ? ColAt(c.cols, i) : C(c.col));
            }
            if (n == 3) Tri(b, b0, b0 + 1, b0 + 2);
            else if (n == 4) { Tri(b, b0, b0 + 1, b0 + 2); Tri(b, b0, b0 + 2, b0 + 3); }
        }

        /// <summary>Rozklad jednoduchého polygonu na trojúhelníky (ear clipping). Vrací indexy po trojicích.</summary>
        public static List<int> Triangulate(List<double[]> unused) { return null; }

        static List<int> Triangulate(List<P> pts)
        {
            int n = pts.Count; var res = new List<int>();
            if (n < 3) return res;
            double area = 0; for (int i = 0; i < n; i++) { var a = pts[i]; var b = pts[(i + 1) % n]; area += a.x * b.y - b.x * a.y; }
            var ix = new List<int>(n); for (int i = 0; i < n; i++) ix.Add(area >= 0 ? i : n - 1 - i);
            int guard = n * n + 10;
            while (ix.Count > 3 && guard-- > 0)
            {
                bool clipped = false;
                for (int i = 0; i < ix.Count; i++)
                {
                    int i0 = ix[(i + ix.Count - 1) % ix.Count], i1 = ix[i], i2 = ix[(i + 1) % ix.Count];
                    var a = pts[i0]; var b = pts[i1]; var c = pts[i2];
                    double cr = (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
                    if (cr <= 1e-12) continue;
                    bool inside = false;
                    foreach (int k in ix)
                    {
                        if (k == i0 || k == i1 || k == i2) continue;
                        if (PointInTri(pts[k], a, b, c)) { inside = true; break; }
                    }
                    if (inside) continue;
                    res.Add(i0); res.Add(i1); res.Add(i2);
                    ix.RemoveAt(i); clipped = true; break;
                }
                if (!clipped)
                {
                    // degenerovaný polygon: zbytek vějířem
                    for (int i = 1; i + 1 < ix.Count; i++) { res.Add(ix[0]); res.Add(ix[i]); res.Add(ix[i + 1]); }
                    return res;
                }
            }
            if (ix.Count == 3) { res.Add(ix[0]); res.Add(ix[1]); res.Add(ix[2]); }
            return res;
        }
        static bool PointInTri(P p, P a, P b, P c)
        {
            double d1 = (p.x - b.x) * (a.y - b.y) - (a.x - b.x) * (p.y - b.y);
            double d2 = (p.x - c.x) * (b.y - c.y) - (b.x - c.x) * (p.y - c.y);
            double d3 = (p.x - a.x) * (c.y - a.y) - (c.x - a.x) * (p.y - a.y);
            bool neg = d1 < -1e-12 || d2 < -1e-12 || d3 < -1e-12, pos = d1 > 1e-12 || d2 > 1e-12 || d3 > 1e-12;
            return !(neg && pos);
        }

        // ---------- textury ----------
        static void AddTex(TessOut o, DrawCmd c)
        {
            var t = c.tex as GdTexture; if (t == null) return;
            var b = o.Get(t); var col = C(c.col);
            double x0 = c.xy[0], y0 = c.xy[1], x1 = c.xy[2], y1 = c.xy[3];
            double u0 = 0, v0 = 0, u1 = 1, v1 = 1;
            if (c.uv != null) { u0 = c.uv[0]; v0 = c.uv[1]; u1 = c.uv[2]; v1 = c.uv[3]; }
            if (c.p != null && c.p.Length > 0 && c.p[0] > 0) { u1 = Math.Abs(x1 - x0) / Math.Max(1, t.w); v1 = Math.Abs(y1 - y0) / Math.Max(1, t.h); t.Repeat = true; }
            Quad(b, Tr(c, x0, y0), Tr(c, x1, y0), Tr(c, x1, y1), Tr(c, x0, y1), col, col, col, col, u0, v0, u1, v1);
        }

        // ---------- text ----------
        public static double TextWidth(string s, double size)
        {
            double w, h;
            if (Text != null) { Text.Measure(s, size, out w, out h); return w; }
            return s.Length * size * 0.52;
        }

        static void AddStr(TessOut o, DrawCmd c, double ox, double oy)
        {
            double tw = TextWidth(c.text, c.fs), sx = 0;
            if (c.maxw > 0) { if (c.align == 1) sx = (c.maxw - tw) * 0.5; else if (c.align == 2) sx = c.maxw - tw; }
            var p = Tr(c, c.xy[0] + sx + ox, c.xy[1] + oy); var col = C(c.col);
            o.GetText().runs.Add(new TextRun { text = c.text, x = p.x, y = p.y, size = c.fs * Scale(c), r = col.r, g = col.g, b = col.b, a = col.a });
        }
        static void AddOutline(TessOut o, DrawCmd c)
        {
            double w = Math.Max(1.0, c.w);
            for (int k = 0; k < 8; k++)
            {
                double a = k * Math.PI / 4;
                AddStr(o, c, Math.Cos(a) * w * 0.5, Math.Sin(a) * w * 0.5);
            }
        }

        // ---------- StyleBox ----------
        static double Num(GdObject sb, string n, double def) { var v = sb.Get(n); return v.IsNum ? v.d : def; }
        static Cl ColOf(GdObject sb, string n, Cl def) { var v = sb.Get(n); var c = v.Cl; return c == null ? def : C(c); }

        static List<P> RoundRect(double x0, double y0, double x1, double y1, double rtl, double rtr, double rbr, double rbl, int seg)
        {
            var l = new List<P>();
            double maxr = Math.Max(0, Math.Min(x1 - x0, y1 - y0) * 0.5);
            rtl = Math.Min(rtl, maxr); rtr = Math.Min(rtr, maxr); rbr = Math.Min(rbr, maxr); rbl = Math.Min(rbl, maxr);
            Action<double, double, double, double, double> arc = (cx, cy, r, a0, a1) =>
            {
                if (r <= 0.01) { l.Add(new P { x = cx, y = cy }); return; }
                for (int i = 0; i <= seg; i++) { double a = a0 + (a1 - a0) * i / seg; l.Add(new P { x = cx + Math.Cos(a) * r, y = cy + Math.Sin(a) * r }); }
            };
            arc(x0 + rtl, y0 + rtl, rtl, Math.PI, 1.5 * Math.PI);
            arc(x1 - rtr, y0 + rtr, rtr, 1.5 * Math.PI, 2 * Math.PI);
            arc(x1 - rbr, y1 - rbr, rbr, 0, 0.5 * Math.PI);
            arc(x0 + rbl, y1 - rbl, rbl, 0.5 * Math.PI, Math.PI);
            return l;
        }

        static void AddStyleBox(TessOut o, DrawCmd c)
        {
            var sb = c.sb; if (sb == null) return;
            var b = o.Get(null);
            double x0 = c.xy[0], y0 = c.xy[1], x1 = c.xy[2], y1 = c.xy[3];
            double em_l = Num(sb, "expand_margin_left", 0), em_t = Num(sb, "expand_margin_top", 0), em_r = Num(sb, "expand_margin_right", 0), em_b = Num(sb, "expand_margin_bottom", 0);
            x0 -= em_l; y0 -= em_t; x1 += em_r; y1 += em_b;
            double rtl = Num(sb, "corner_radius_top_left", 0), rtr = Num(sb, "corner_radius_top_right", 0), rbr = Num(sb, "corner_radius_bottom_right", 0), rbl = Num(sb, "corner_radius_bottom_left", 0);
            double bl = Num(sb, "border_width_left", 0), bt = Num(sb, "border_width_top", 0), br = Num(sb, "border_width_right", 0), bb = Num(sb, "border_width_bottom", 0);
            int seg = 6;
            double ss = Num(sb, "shadow_size", 0);
            if (ss > 0)
            {
                var sc = ColOf(sb, "shadow_color", new Cl { r = 0, g = 0, b = 0, a = 0.6f }); var so = sb.Get("shadow_offset");
                double sx = so.k == K.Vec2 ? so.d : 0, sy = so.k == K.Vec2 ? so.e : 0;
                // měkký stín: 3 vrstvy
                for (int i = 3; i >= 1; i--)
                {
                    double g = ss * i / 3.0; var cc = sc; cc.a = sc.a / (i * 1.6f + 1f);
                    var pts = new List<P>(RoundRect(x0 - g + sx, y0 - g + sy, x1 + g + sx, y1 + g + sy, rtl + g, rtr + g, rbr + g, rbl + g, seg));
                    FillConvex(b, pts, cc);
                }
            }
            var bcol = ColOf(sb, "border_color", new Cl { r = 0.8f, g = 0.8f, b = 0.8f, a = 1 });
            var bg = ColOf(sb, "bg_color", new Cl { r = 0.6f, g = 0.6f, b = 0.6f, a = 1 });
            bool center = !sb.Get("draw_center").IsNull ? sb.Get("draw_center").T() : true;
            if (bl + bt + br + bb > 0)
            {
                FillConvex(b, RoundRect(x0, y0, x1, y1, rtl, rtr, rbr, rbl, seg), bcol);
                if (center) FillConvex(b, RoundRect(x0 + bl, y0 + bt, x1 - br, y1 - bb, Math.Max(0, rtl - Math.Max(bl, bt)), Math.Max(0, rtr - Math.Max(br, bt)), Math.Max(0, rbr - Math.Max(br, bb)), Math.Max(0, rbl - Math.Max(bl, bb)), seg), bg);
            }
            else if (center) FillConvex(b, RoundRect(x0, y0, x1, y1, rtl, rtr, rbr, rbl, seg), bg);
        }
    }
}
