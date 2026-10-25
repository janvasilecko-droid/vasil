// Geometrie: Geometry2D (průnik polygonů, bod v polygonu, průsečík úseček) a Curve2D (Bézierova křivka s tessellate).
using System;
using System.Collections.Generic;
using KK = Pivovar.Gd.K;

namespace Pivovar.Gd
{
    public sealed class GdCurve2D : GdNode
    {
        public readonly List<double[]> pts = new List<double[]>();   // x,y,inx,iny,outx,outy
        public GdCurve2D() { TypeName = "Curve2D"; }
        public override Gv Invoke(string n, Gv[] a)
        {
            switch (n)
            {
                case "add_point":
                    {
                        var p = Gd.A(a, 0); var i = a.Length > 1 ? a[1] : Gv.Vec(0, 0); var o = a.Length > 2 ? a[2] : Gv.Vec(0, 0);
                        pts.Add(new[] { p.d, p.e, i.d, i.e, o.d, o.e }); return Gv.Null;
                    }
                case "clear_points": pts.Clear(); return Gv.Null;
                case "get_point_count": return Gv.I(pts.Count);
                case "tessellate": return Gv.A(Tess(a.Length > 1 ? Gd.A(a, 1).d : 4));
                case "get_baked_points": return Gv.A(Tess(0.6));
                case "get_baked_length": { var l = Tess(0.6).l; double s = 0; for (int i = 1; i < l.Count; i++) s += Math.Sqrt((l[i].d - l[i - 1].d) * (l[i].d - l[i - 1].d) + (l[i].e - l[i - 1].e) * (l[i].e - l[i - 1].e)); return Gv.F(s); }
            }
            return base.Invoke(n, a);
        }
        GdArray Tess(double tolDeg)
        {
            var res = new GdArray(); res.typed = "PackedVector2Array";
            if (pts.Count == 0) return res;
            res.l.Add(Gv.Vec(pts[0][0], pts[0][1]));
            double flat = tolDeg > 0.5 ? 0.35 : 0.2;
            for (int i = 0; i + 1 < pts.Count; i++)
            {
                var p0 = pts[i]; var p1 = pts[i + 1];
                double ax = p0[0], ay = p0[1], bx = p0[0] + p0[4], by = p0[1] + p0[5], cx = p1[0] + p1[2], cy = p1[1] + p1[3], dx = p1[0], dy = p1[1];
                Sub(res, ax, ay, bx, by, cx, cy, dx, dy, flat, 0);
            }
            return res;
        }
        static void Sub(GdArray r, double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy, double tol, int depth)
        {
            // rovinnost: vzdálenost řídicích bodů od tětivy
            double vx = dx - ax, vy = dy - ay, l = Math.Sqrt(vx * vx + vy * vy);
            double d1, d2;
            if (l < 1e-9) { d1 = Math.Sqrt((bx - ax) * (bx - ax) + (by - ay) * (by - ay)); d2 = Math.Sqrt((cx - ax) * (cx - ax) + (cy - ay) * (cy - ay)); }
            else { d1 = Math.Abs((bx - ax) * vy - (by - ay) * vx) / l; d2 = Math.Abs((cx - ax) * vy - (cy - ay) * vx) / l; }
            if (depth >= 10 || Math.Max(d1, d2) <= tol) { r.l.Add(Gv.Vec(dx, dy)); return; }
            double abx = (ax + bx) / 2, aby = (ay + by) / 2, bcx = (bx + cx) / 2, bcy = (by + cy) / 2, cdx = (cx + dx) / 2, cdy = (cy + dy) / 2;
            double abcx = (abx + bcx) / 2, abcy = (aby + bcy) / 2, bcdx = (bcx + cdx) / 2, bcdy = (bcy + cdy) / 2, mx = (abcx + bcdx) / 2, my = (abcy + bcdy) / 2;
            Sub(r, ax, ay, abx, aby, abcx, abcy, mx, my, tol, depth + 1);
            Sub(r, mx, my, bcdx, bcdy, cdx, cdy, dx, dy, tol, depth + 1);
        }
    }

    public static class GdGeom
    {
        static List<double[]> Poly(Gv v)
        {
            var r = new List<double[]>(); var a = v.Arr; if (a == null) return r;
            foreach (var p in a.l) r.Add(new[] { p.d, p.e });
            return r;
        }
        static Gv ToGv(List<double[]> p)
        {
            var a = new GdArray(); a.typed = "PackedVector2Array";
            foreach (var q in p) a.l.Add(Gv.Vec(q[0], q[1]));
            return Gv.A(a);
        }
        static double Area(List<double[]> p)
        {
            double s = 0; for (int i = 0; i < p.Count; i++) { var a = p[i]; var b = p[(i + 1) % p.Count]; s += a[0] * b[1] - b[0] * a[1]; }
            return s * 0.5;
        }
        static bool Convex(List<double[]> p)
        {
            int sign = 0, n = p.Count; if (n < 3) return false;
            for (int i = 0; i < n; i++)
            {
                var a = p[i]; var b = p[(i + 1) % n]; var c = p[(i + 2) % n];
                double cr = (b[0] - a[0]) * (c[1] - b[1]) - (b[1] - a[1]) * (c[0] - b[0]);
                if (Math.Abs(cr) < 1e-9) continue;
                int s = cr > 0 ? 1 : -1; if (sign == 0) sign = s; else if (s != sign) return false;
            }
            return true;
        }

        /// <summary>Sutherland–Hodgman: libovolný (i nekonvexní) subjekt ořezaný konvexním polygonem.</summary>
        static List<double[]> ClipConvex(List<double[]> subj, List<double[]> clip)
        {
            var cl = new List<double[]>(clip); if (Area(cl) < 0) cl.Reverse();   // proti směru hodin v matematických osách (y dolů: záleží jen na konzistenci)
            var output = new List<double[]>(subj);
            for (int i = 0; i < cl.Count && output.Count > 0; i++)
            {
                var a = cl[i]; var b = cl[(i + 1) % cl.Count];
                var input = output; output = new List<double[]>();
                for (int j = 0; j < input.Count; j++)
                {
                    var p = input[j]; var q = input[(j + 1) % input.Count];
                    double sp = Side(a, b, p), sq = Side(a, b, q);
                    bool pin = sp >= 0, qin = sq >= 0;
                    if (pin && qin) output.Add(q);
                    else if (pin && !qin) output.Add(Inter(p, q, sp, sq));
                    else if (!pin && qin) { output.Add(Inter(p, q, sp, sq)); output.Add(q); }
                }
            }
            return output;
        }
        static double Side(double[] a, double[] b, double[] p) { return (b[0] - a[0]) * (p[1] - a[1]) - (b[1] - a[1]) * (p[0] - a[0]); }
        static double[] Inter(double[] p, double[] q, double sp, double sq) { double t = sp / (sp - sq); return new[] { p[0] + (q[0] - p[0]) * t, p[1] + (q[1] - p[1]) * t }; }

        public static bool PointInPoly(double x, double y, List<double[]> p)
        {
            bool inside = false;
            for (int i = 0, j = p.Count - 1; i < p.Count; j = i++)
                if (((p[i][1] > y) != (p[j][1] > y)) && (x < (p[j][0] - p[i][0]) * (y - p[i][1]) / (p[j][1] - p[i][1]) + p[i][0])) inside = !inside;
            return inside;
        }

        public static bool Call(string n, Gv[] a, out Gv r)
        {
            r = Gv.Null;
            switch (n)
            {
                case "is_point_in_polygon": { var pt = Gd.A(a, 0); r = Gv.B(PointInPoly(pt.d, pt.e, Poly(Gd.A(a, 1)))); return true; }
                case "segment_intersects_segment":
                    {
                        var p1 = Gd.A(a, 0); var p2 = Gd.A(a, 1); var q1 = Gd.A(a, 2); var q2 = Gd.A(a, 3);
                        double d = (p2.d - p1.d) * (q2.e - q1.e) - (p2.e - p1.e) * (q2.d - q1.d);
                        if (Math.Abs(d) < 1e-12) return true;
                        double t = ((q1.d - p1.d) * (q2.e - q1.e) - (q1.e - p1.e) * (q2.d - q1.d)) / d, u = ((q1.d - p1.d) * (p2.e - p1.e) - (q1.e - p1.e) * (p2.d - p1.d)) / d;
                        if (t >= 0 && t <= 1 && u >= 0 && u <= 1) r = Gv.Vec(p1.d + (p2.d - p1.d) * t, p1.e + (p2.e - p1.e) * t);
                        return true;
                    }
                case "intersect_polygons":
                    {
                        var A = Poly(Gd.A(a, 0)); var B = Poly(Gd.A(a, 1)); var res = new GdArray();
                        List<double[]> o = null;
                        if (Convex(B)) o = ClipConvex(A, B); else if (Convex(A)) o = ClipConvex(B, A);
                        else
                        {   // obě nekonvexní: ořez po trojúhelnících jednoho polygonu
                            o = null;
                            foreach (var tri in Triangulate(A)) { var c = ClipConvexInTri(B, tri); if (c != null && c.Count >= 3) res.l.Add(ToGv(c)); }
                            r = Gv.A(res); return true;
                        }
                        if (o != null && o.Count >= 3) res.l.Add(ToGv(o));
                        r = Gv.A(res); return true;
                    }
                case "clip_polygons": case "merge_polygons": case "offset_polygon": { var res = new GdArray(); if (n == "merge_polygons") res.l.Add(Gd.A(a, 0)); else if (n == "clip_polygons") res.l.Add(Gd.A(a, 0)); else res.l.Add(Gd.A(a, 0)); r = Gv.A(res); return true; }
            }
            return false;
        }

        static List<double[]> ClipConvexInTri(List<double[]> subj, List<double[]> tri) { return ClipConvex(subj, tri); }

        static List<List<double[]>> Triangulate(List<double[]> poly)
        {
            var res = new List<List<double[]>>(); var idx = new List<int>(); for (int i = 0; i < poly.Count; i++) idx.Add(i);
            bool ccw = Area(poly) > 0; int guard = 0;
            while (idx.Count > 3 && guard++ < 10000)
            {
                bool cut = false;
                for (int i = 0; i < idx.Count; i++)
                {
                    var a = poly[idx[(i + idx.Count - 1) % idx.Count]]; var b = poly[idx[i]]; var c = poly[idx[(i + 1) % idx.Count]];
                    double cr = (b[0] - a[0]) * (c[1] - b[1]) - (b[1] - a[1]) * (c[0] - b[0]);
                    if ((cr > 0) != ccw || Math.Abs(cr) < 1e-12) continue;
                    bool ok = true;
                    foreach (int k in idx) { var p = poly[k]; if (p == a || p == b || p == c) continue; if (InTri(p, a, b, c)) { ok = false; break; } }
                    if (!ok) continue;
                    res.Add(new List<double[]> { a, b, c }); idx.RemoveAt(i); cut = true; break;
                }
                if (!cut) break;
            }
            if (idx.Count == 3) res.Add(new List<double[]> { poly[idx[0]], poly[idx[1]], poly[idx[2]] });
            return res;
        }
        static bool InTri(double[] p, double[] a, double[] b, double[] c)
        {
            double d1 = Side(a, b, p), d2 = Side(b, c, p), d3 = Side(c, a, p);
            bool neg = d1 < 0 || d2 < 0 || d3 < 0, pos = d1 > 0 || d2 > 0 || d3 > 0; return !(neg && pos);
        }
    }
}
