// Plátno UI Toolkit: vykreslí snímek z GdRender (trojúhelníky + text) do jednoho prvku přes MeshGenerationContext.
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Pivovar.Gd;
#if UNITY_6000_0_OR_NEWER
using UnityEngine.TextCore.Text;
#endif

namespace Pivovar.GdUnity
{
    public sealed class GdCanvasElement : VisualElement
    {
        public GdNode Root;
        public static bool FlipWinding;
        public int LastTris, LastItems, LastBatches;
        readonly Dictionary<GdTexture, Texture2D> texCache = new Dictionary<GdTexture, Texture2D>();
        readonly Dictionary<GdTexture, int> texVer = new Dictionary<GdTexture, int>();
        static Vertex[] vbuf = new Vertex[65536];
        static ushort[] ibuf = new ushort[65536 * 3];

        public GdCanvasElement()
        {
            style.position = Position.Absolute;
            style.left = 0; style.top = 0; style.right = 0; style.bottom = 0;
            generateVisualContent += OnGenerate;
            focusable = true;
        }

        // ---------- textury ----------
        public Texture2D TexOf(GdTexture t)
        {
            if (t == null) return null;
            Texture2D tx; int ver;
            if (texCache.TryGetValue(t, out tx) && tx != null)
            {
                if (t.rgba == null || (texVer.TryGetValue(t, out ver) && ver == t.Version)) { ApplyWrap(tx, t); return tx; }
            }
            if (t.rgba != null)
            {
                if (tx == null || tx.width != t.w || tx.height != t.h) { tx = new Texture2D(Math.Max(1, t.w), Math.Max(1, t.h), TextureFormat.RGBA32, false); tx.filterMode = FilterMode.Bilinear; }
                var px = new Color32[t.w * t.h];
                for (int y = 0; y < t.h; y++)
                    for (int x = 0; x < t.w; x++)
                    {
                        int i = (y * t.w + x) * 4;
                        px[(t.h - 1 - y) * t.w + x] = new Color32(F(t.rgba[i]), F(t.rgba[i + 1]), F(t.rgba[i + 2]), F(t.rgba[i + 3]));
                    }
                tx.SetPixels32(px); tx.Apply(false);
            }
            else if (t.path != null)
            {
                try
                {
                    var bytes = System.IO.File.ReadAllBytes(Gd.MapPath(t.path));
                    tx = new Texture2D(2, 2, TextureFormat.RGBA32, true);
                    ImageConversion.LoadImage(tx, bytes, false);
                    tx.filterMode = FilterMode.Trilinear; tx.anisoLevel = 4;
                    t.w = tx.width; t.h = tx.height;
                }
                catch (Exception e) { Debug.LogWarning("Pivovar2D: textura " + t.path + ": " + e.Message); tx = Texture2D.whiteTexture; }
            }
            else tx = Texture2D.whiteTexture;
            texCache[t] = tx; texVer[t] = t.Version;
            ApplyWrap(tx, t);
            return tx;
        }
        static byte F(float v) { return (byte)Mathf.Clamp(Mathf.RoundToInt(v * 255f), 0, 255); }
        static void ApplyWrap(Texture2D tx, GdTexture t) { if (tx != Texture2D.whiteTexture) tx.wrapMode = t.Repeat ? TextureWrapMode.Repeat : TextureWrapMode.Clamp; }

        // ---------- kreslení ----------
        void OnGenerate(MeshGenerationContext mgc)
        {
            if (Root == null) return;
            var items = GdRender.Collect(Root);
            int tris = 0, batches = 0;
            foreach (var it in items)
            {
                if (it.mesh == null) continue;
                foreach (var b in it.mesh.batches)
                {
                    batches++;
                    if (b.runs != null) { DrawRuns(mgc, it, b); continue; }
                    if (b.idx.Count == 0) continue;
                    tris += b.idx.Count / 3;
                    EmitBatch(mgc, it, b);
                }
            }
            LastTris = tris; LastItems = items.Count; LastBatches = batches;
        }

        void EmitBatch(MeshGenerationContext mgc, RenderItem it, TessBatch b)
        {
            Texture2D tex = b.tex != null ? TexOf(b.tex) : null;
            var m = it.m; var v = b.v; int vc = b.VCount; var idx = b.idx;
            var clip = it.clip;
            if (vc <= 65000)
            {
                if (vbuf.Length < vc) vbuf = new Vertex[vc];
                if (ibuf.Length < idx.Count) ibuf = new ushort[idx.Count];
                FillVerts(vbuf, v, vc, m, it, tex != null && b.tex != null);
                int n = idx.Count;
                for (int i = 0; i < n; i += 3)
                {
                    int i0 = idx[i], i1 = idx[i + 1], i2 = idx[i + 2];
                    double area = (vbuf[i1].position.x - vbuf[i0].position.x) * (vbuf[i2].position.y - vbuf[i0].position.y) - (vbuf[i2].position.x - vbuf[i0].position.x) * (vbuf[i1].position.y - vbuf[i0].position.y);
                    bool cw = area > 0; if (FlipWinding) cw = !cw;
                    ibuf[i] = (ushort)i0; ibuf[i + 1] = (ushort)(cw ? i1 : i2); ibuf[i + 2] = (ushort)(cw ? i2 : i1);
                }
                var wd = mgc.Allocate(vc, n, tex);
                wd.SetAllVertices(new Unity.Collections.NativeSlice<Vertex>(new Unity.Collections.NativeArray<Vertex>(vbuf, Unity.Collections.Allocator.Temp), 0, vc));
                wd.SetAllIndices(new Unity.Collections.NativeSlice<ushort>(new Unity.Collections.NativeArray<ushort>(ibuf, Unity.Collections.Allocator.Temp), 0, n));
                return;
            }
            // velká dávka: rozdělit po trojúhelnících
            var map = new Dictionary<int, int>(); var lv = new List<Vertex>(); var li = new List<ushort>();
            var all = new Vertex[vc]; FillVerts(all, v, vc, m, it, b.tex != null);
            int pos = 0;
            while (pos < idx.Count)
            {
                map.Clear(); lv.Clear(); li.Clear();
                while (pos < idx.Count && lv.Count + 3 <= 65000)
                {
                    int i0 = idx[pos], i1 = idx[pos + 1], i2 = idx[pos + 2]; pos += 3;
                    double area = (all[i1].position.x - all[i0].position.x) * (all[i2].position.y - all[i0].position.y) - (all[i2].position.x - all[i0].position.x) * (all[i1].position.y - all[i0].position.y);
                    bool cw = area > 0; if (FlipWinding) cw = !cw;
                    int a = i0, bb = cw ? i1 : i2, c = cw ? i2 : i1;
                    foreach (int k in new[] { a, bb, c })
                    {
                        int mapped; if (!map.TryGetValue(k, out mapped)) { mapped = lv.Count; map[k] = mapped; lv.Add(all[k]); }
                        li.Add((ushort)mapped);
                    }
                }
                var wd = mgc.Allocate(lv.Count, li.Count, tex);
                wd.SetAllVertices(new Unity.Collections.NativeArray<Vertex>(lv.ToArray(), Unity.Collections.Allocator.Temp));
                wd.SetAllIndices(new Unity.Collections.NativeArray<ushort>(li.ToArray(), Unity.Collections.Allocator.Temp));
            }
        }

        static void FillVerts(Vertex[] dst, List<float> v, int vc, Mat m, RenderItem it, bool textured)
        {
            for (int i = 0; i < vc; i++)
            {
                int o = i * 8;
                double x = v[o], y = v[o + 1], X, Y;
                m.Apply(x, y, out X, out Y);
                Vertex vt = new Vertex();
                vt.position = new Vector3((float)X, (float)Y, Vertex.nearZ);
                vt.uv = textured ? new Vector2(v[o + 2], 1f - v[o + 3]) : Vector2.zero;
                vt.tint = new Color32(F(v[o + 4] * it.mr), F(v[o + 5] * it.mg), F(v[o + 6] * it.mb), F(v[o + 7] * it.ma));
                dst[i] = vt;
            }
        }

        void DrawRuns(MeshGenerationContext mgc, RenderItem it, TessBatch b)
        {
#if UNITY_6000_0_OR_NEWER
            var fa = GdFontProvider.Asset;
            if (fa == null) return;
            double sc = it.m.Scale;
            foreach (var r in b.runs)
            {
                double X, Y; it.m.Apply(r.x, r.y, out X, out Y);
                double size = r.size * sc; if (size < 2) continue;
                double top = Y - GdFontProvider.AscentPx(size);
                var col = new Color(r.r * it.mr, r.g * it.mg, r.b * it.mb, r.a * it.ma);
                mgc.DrawText(r.text, new Vector2((float)X, (float)top), (float)size, col, fa);
            }
#endif
        }
    }
}
