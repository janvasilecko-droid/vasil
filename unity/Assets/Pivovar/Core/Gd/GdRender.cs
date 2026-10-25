// Sestavení snímku: projde strom uzlů, spočítá globální transformace, pořadí (vrstva, z_index, strom) a mesh každého uzlu.
using System;
using System.Collections.Generic;
using KK = Pivovar.Gd.K;

namespace Pivovar.Gd
{
    public struct Mat
    {
        public double a, b, c, d, tx, ty;
        public static Mat Id { get { Mat m; m.a = 1; m.b = 0; m.c = 0; m.d = 1; m.tx = 0; m.ty = 0; return m; } }
        /// <summary>this ∘ o  (nejdřív o, potom this)</summary>
        public Mat Mul(Mat o)
        {
            Mat r;
            r.a = a * o.a + c * o.b; r.b = b * o.a + d * o.b;
            r.c = a * o.c + c * o.d; r.d = b * o.c + d * o.d;
            r.tx = a * o.tx + c * o.ty + tx; r.ty = b * o.tx + d * o.ty + ty;
            return r;
        }
        public void Apply(double x, double y, out double ox, out double oy) { ox = a * x + c * y + tx; oy = b * x + d * y + ty; }
        public Mat Inv()
        {
            double det = a * d - b * c; Mat r;
            if (Math.Abs(det) < 1e-12) return Id;
            r.a = d / det; r.b = -b / det; r.c = -c / det; r.d = a / det;
            r.tx = -(r.a * tx + r.c * ty); r.ty = -(r.b * tx + r.d * ty);
            return r;
        }
        public static Mat Trs(double px, double py, double rot, double sx, double sy)
        {
            double cs = Math.Cos(rot), sn = Math.Sin(rot); Mat m;
            m.a = cs * sx; m.b = sn * sx; m.c = -sn * sy; m.d = cs * sy; m.tx = px; m.ty = py; return m;
        }
        public double Scale { get { return Math.Sqrt(Math.Abs(a * d - b * c)); } }
    }

    public sealed class RenderItem
    {
        public GdNode node; public Mat m; public int layer, z, seq; public float mr = 1, mg = 1, mb = 1, ma = 1;
        public TessOut mesh;
        public double[] clip;          // x0,y0,x1,y1 ve screen souřadnicích nebo null
        public bool world;
    }

    public static class GdRender
    {
        public static GdNode CurrentCam;
        public static Func<GdNode, bool> KeepWorld;   // výjimky z HideWorld (kartičky nádob, stavy čerpadel, úniky)
        public static bool HideWorld;          // režim 3D: svět z Godotu se nekreslí (kreslí ho Unity), jen ovládací prvky a pohledy SubViewportů
        static GdNode mainRoot;
        static bool inSub;                     // uvnitř SubViewportu (svět se nekryje HideWorld)
        static bool subPass;                   // právě se kreslí svět do SubViewportu (bez CanvasLayerů a ovládacích prvků)
        public static List<RenderItem> CtlItems = new List<RenderItem>();   // ovládací prvky v pořadí kreslení (pro zásah myší)

        static Gv P(GdNode n, string k) { return n.Get(k); }

        public static Mat LocalMat(GdNode n)
        {
            var pos = n.Get("position"); var rot = n.Get("rotation"); var sc = n.Get("scale");
            double px = pos.k == KK.Vec2 ? pos.d : 0, py = pos.k == KK.Vec2 ? pos.e : 0;
            double sx = sc.k == KK.Vec2 ? sc.d : 1, sy = sc.k == KK.Vec2 ? sc.e : 1;
            double r = rot.IsNum ? rot.d : 0;
            if (n.TypeName == "CanvasLayer") { var off = n.Get("offset"); px = off.k == KK.Vec2 ? off.d : 0; py = off.k == KK.Vec2 ? off.e : 0; r = n.Get("rotation").IsNum ? n.Get("rotation").d : 0; sc = n.Get("scale"); sx = sc.k == KK.Vec2 ? sc.d : 1; sy = sc.k == KK.Vec2 ? sc.e : 1; }
            if (px == 0 && py == 0 && r == 0 && sx == 1 && sy == 1) return Mat.Id;
            return Mat.Trs(px, py, r, sx, sy);
        }

        public static Mat CameraMat()
        {
            var cam = CurrentCam;
            double px = 0, py = 0, z = 1;
            if (cam != null)
            {
                var p = cam.Get("position"); var zm = cam.Get("zoom");
                if (p.k == KK.Vec2) { px = p.d; py = p.e; }
                if (zm.k == KK.Vec2 && zm.d > 0) z = zm.d;
            }
            Mat m; m.a = z; m.b = 0; m.c = 0; m.d = z; m.tx = -px * z + Gd.ScreenW * 0.5; m.ty = -py * z + Gd.ScreenH * 0.5;
            return m;
        }

        /// <summary>Matice z místních souřadnic uzlu na obrazovku (včetně kamery u světa).</summary>
        public static Mat GlobalMat(GdNode n)
        {
            var chain = new List<GdNode>();
            for (var p = n; p != null; p = p.Parent) chain.Add(p);
            Mat m = Mat.Id; bool layer = false;
            for (int i = chain.Count - 1; i >= 0; i--)
            {
                var c = chain[i];
                if (c.TypeName == "CanvasLayer") { m = Mat.Id; layer = true; }
                m = m.Mul(LocalMat(c));
            }
            if (!layer) m = CameraMat().Mul(m);
            return m;
        }

        /// <summary>Matice plátna uzlu (bez jeho vlastní transformace): kamera nebo CanvasLayer.</summary>
        public static Mat CanvasMat(GdNode n)
        {
            Mat m = Mat.Id; bool layer = false;
            var chain = new List<GdNode>();
            for (var p = n.Parent; p != null; p = p.Parent) chain.Add(p);
            for (int i = chain.Count - 1; i >= 0; i--)
            {
                var c = chain[i];
                if (c.TypeName == "CanvasLayer") { m = Mat.Id; layer = true; }
                m = m.Mul(LocalMat(c));
            }
            return layer ? m : CameraMat().Mul(m);
        }

        public static Gv ScreenToLocal(GdNode n, Gv screen)
        {
            var inv = GlobalMat(n).Inv(); double x, y; inv.Apply(screen.d, screen.e, out x, out y); return Gv.Vec(x, y);
        }
        public static Gv ScreenToCanvas(GdNode n, Gv screen)
        {
            // souřadnice rodiče (kamera/vrstva), jako get_global_mouse_position u Node2D
            var parent = n.Parent; Mat m = parent != null ? GlobalMat(parent) : (CameraMat());
            var inv = m.Inv(); double x, y; inv.Apply(screen.d, screen.e, out x, out y); return Gv.Vec(x, y);
        }

        static bool Vis(GdNode n) { var v = n.Get("visible"); return !(v.k == KK.Bool && v.d == 0); }

        /// <summary>Sesbírá kreslené uzly v pořadí vykreslení.</summary>
        public static List<RenderItem> Collect(GdNode root)
        {
            var list = new List<RenderItem>(); int seq = 0; mainRoot = root; subPass = false; inSub = false;
            GdUi.LayoutAll(root);
            if (GdUi.Active) GdUi.LayoutTooltip();
            CtlItems = new List<RenderItem>();
            Rec(root, Mat.Id, CameraMat(), true, 0, 0, 1, 1, 1, 1, list, ref seq, null);
            if (GdUi.Active && GdUi.TooltipRoot != null && GdUi.TooltipVisible) Rec(GdUi.TooltipRoot, Mat.Id, CameraMat(), false, 1000, 0, 1, 1, 1, 1, list, ref seq, null);
            Comparison<RenderItem> cmp = (x, y) =>
            {
                int c = x.layer.CompareTo(y.layer); if (c != 0) return c;
                c = x.z.CompareTo(y.z); if (c != 0) return c;
                return x.seq.CompareTo(y.seq);
            };
            list.Sort(cmp);
            CtlItems.Sort(cmp);
            return list;
        }

        /// <summary>Pohled SubViewportu do kontejneru: sdílený svět (rozvaděč, lupa) se nakreslí jinou kamerou a ořízne na obdélník kontejneru.</summary>
        static void RecSub(GdNode n, Mat full, int layer, int zz, float mr, float mg, float mb, float ma, List<RenderItem> list, ref int seq, double[] clip)
        {
            GdNode sv = null;
            foreach (var c in n.Children) if (c.TypeName == "SubViewport") { sv = c; break; }
            if (sv == null) return;
            var sz = n.Get("size"); if (!(sz.k == KK.Vec2 || sz.k == KK.Vec2i)) return;
            double cw = sz.d, ch = sz.e;
            if (cw < 2 || ch < 2) return;
            double ox, oy, ex, ey; full.Apply(0, 0, out ox, out oy); full.Apply(cw, ch, out ex, out ey);
            var rect = new double[] { Math.Min(ox, ex), Math.Min(oy, ey), Math.Max(ox, ex), Math.Max(oy, ey) };
            if (clip != null) { rect[0] = Math.Max(rect[0], clip[0]); rect[1] = Math.Max(rect[1], clip[1]); rect[2] = Math.Min(rect[2], clip[2]); rect[3] = Math.Min(rect[3], clip[3]); }
            if (rect[2] <= rect[0] || rect[3] <= rect[1]) return;
            bool own = false;
            foreach (var c in sv.Children) if (c.TypeName != "Camera2D") { own = true; break; }
            double px = 0, py = 0, z = 1; var cam = sv.SubCam;
            Mat camM;
            if (cam != null)
            {
                var p = cam.Get("position"); var zm = cam.Get("zoom");
                if (p.k == KK.Vec2) { px = p.d; py = p.e; }
                if (zm.k == KK.Vec2 && zm.d > 0) z = zm.d;
                camM.a = z; camM.b = 0; camM.c = 0; camM.d = z; camM.tx = -px * z + ox + (ex - ox) * 0.5; camM.ty = -py * z + oy + (ey - oy) * 0.5;
            }
            else { camM = Mat.Id; camM.tx = ox; camM.ty = oy; }
            var sub = new List<RenderItem>(); int s2 = 0;
            bool oldSub = subPass, oldIn = inSub; subPass = !own ? true : subPass; inSub = true;
            try
            {
                if (own)
                {
                    foreach (var c in sv.Children) if (c.TypeName != "Camera2D") Rec(c, Mat.Id, camM, true, 0, 0, 1, 1, 1, 1, sub, ref s2, rect);
                }
                else Rec(mainRoot, Mat.Id, camM, true, 0, 0, 1, 1, 1, 1, sub, ref s2, rect);
            }
            finally { subPass = oldSub; inSub = oldIn; }
            Comparison<RenderItem> cmp = (x, y) => { int c = x.z.CompareTo(y.z); return c != 0 ? c : x.seq.CompareTo(y.seq); };
            sub.Sort(cmp);
            foreach (var it in sub)
            {
                it.layer = layer; it.z = zz; it.seq = seq++;
                it.mr *= mr; it.mg *= mg; it.mb *= mb; it.ma *= ma;
                list.Add(it);
            }
        }

        static void Rec(GdNode n, Mat parent, Mat cam, bool world, int layer, int z, float mr, float mg, float mb, float ma, List<RenderItem> list, ref int seq, double[] clip)
        {
            if (n.QueuedFree || !Vis(n)) return;
            if (n.TypeName == "SubViewport") return;           // jeho obsah kreslí kontejner (viz RecSub)
            if (subPass && n.TypeName == "CanvasLayer") return;
            Mat m;
            if (n.TypeName == "CanvasLayer")
            {
                world = false; var lv = n.Get("layer"); layer = lv.IsNum ? (int)lv.d : 1; m = LocalMat(n);
            }
            else m = parent.Mul(LocalMat(n));
            Mat full = world ? cam.Mul(m) : m;
            int zz = z;
            var zi = n.Get("z_index"); if (zi.IsNum) { var rel = n.Get("z_as_relative"); zz = (rel.k == KK.Bool && rel.d == 0) ? (int)zi.d : z + (int)zi.d; }
            var mod = n.Get("modulate").Cl; var smod = n.Get("self_modulate").Cl;
            float r = mr, g = mg, b = mb, a = ma;
            if (mod != null) { r *= (float)mod.r; g *= (float)mod.g; b *= (float)mod.b; a *= (float)mod.a; }
            // clip_contents
            double[] cl = clip;
            if (n.Get("clip_contents").T() || (GdUi.Active && n.TypeName == "ScrollContainer"))
            {
                var sz = n.Get("size"); if (sz.k == KK.Vec2 || sz.k == KK.Vec2i)
                {
                    double x0, y0, x1, y1; full.Apply(0, 0, out x0, out y0); full.Apply(sz.d, sz.e, out x1, out y1);
                    var nc = new double[] { Math.Min(x0, x1), Math.Min(y0, y1), Math.Max(x0, x1), Math.Max(y0, y1) };
                    if (cl != null) { nc[0] = Math.Max(nc[0], cl[0]); nc[1] = Math.Max(nc[1], cl[1]); nc[2] = Math.Min(nc[2], cl[2]); nc[3] = Math.Min(nc[3], cl[3]); }
                    cl = nc;
                }
            }
            if (GdUi.Active && n.IsCtl && !subPass)
            {
                CtlItems.Add(new RenderItem { node = n, m = full, layer = layer, z = zz, seq = seq, clip = cl, world = world });
                var ui = GdUi.Paint(n);
                if (ui != null)
                {
                    var ri = new RenderItem { node = n, m = full, layer = layer, z = zz, seq = seq++, mr = r, mg = g, mb = b, ma = a, clip = cl, world = world, mesh = ui };
                    if (smod != null) { ri.mr *= (float)smod.r; ri.mg *= (float)smod.g; ri.mb *= (float)smod.b; ri.ma *= (float)smod.a; }
                    list.Add(ri);
                }
            }
            if (n.Cmds != null && n.Cmds.Count > 0 && !(HideWorld && world && !inSub && !(KeepWorld != null && KeepWorld(n))))
            {
                var it = new RenderItem { node = n, m = full, layer = layer, z = zz, seq = seq++, mr = r, mg = g, mb = b, ma = a, clip = cl, world = world };
                if (smod != null) { it.mr *= (float)smod.r; it.mg *= (float)smod.g; it.mb *= (float)smod.b; it.ma *= (float)smod.a; }
                if (n.MeshVer != n.DrawVer || n.Mesh == null)
                {
                    var t = new TessOut();
                    foreach (var c in n.Cmds) { try { Tess.Add(t, c); } catch (Exception ex) { Gd.Err("Tess " + c.k, ex); } }
                    n.Mesh = t; n.MeshVer = n.DrawVer;
                }
                it.mesh = n.Mesh; list.Add(it);
            }
            else seq++;
            var kids = n.Children;
            for (int i = 0; i < kids.Count; i++) Rec(kids[i], m, cam, world, layer, zz, r, g, b, a, list, ref seq, cl);
            if (GdUi.Active && n.TypeName == "SubViewportContainer" && !subPass) RecSub(n, full, layer, zz, r, g, b, a, list, ref seq, cl);
            if (GdUi.Active && n.TypeName == "ScrollContainer")
            {
                var post = GdUi.PaintPost(n);
                if (post != null) list.Add(new RenderItem { node = n, m = full, layer = layer, z = zz, seq = seq++, mr = r, mg = g, mb = b, ma = a, clip = clip, world = world, mesh = post });
            }
        }
    }
}
