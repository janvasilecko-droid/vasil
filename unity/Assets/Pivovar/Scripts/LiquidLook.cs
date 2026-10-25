// Vzhled kapalin: voda, sladina, mladina, rmut (kaše se šrotem a plevami) a mláto.
// Všechny textury se generují za běhu (bez souborů), materiály jsou HDRP/Lit a jdou přes HdrpUtil.
// Hodnoty, které se dají ladit podle chuti, jsou nahoře jako konstanty.
using System.Collections.Generic;
using UnityEngine;

namespace Pivovar
{
    public enum LiqKind { Empty = 0, Water = 1, Wort = 2, Mash = 3 }

    /// <summary>Co je v nádobě: simulace to posílá přes SimSync.</summary>
    public struct LiquidState
    {
        public LiqKind kind;
        public Color col;        // barva kapaliny (sladina / rmut) z Godotu
        public float solids;     // podíl pevné sušiny v objemu, 0..1 (rmut ~ 0,1–0,3)
        public float turb;       // zákal 0..1
        public float T;          // °C
        public float stir;       // míchání 0..1
        public float boil;       // var 0..1
        public float foam;       // pěna 0..1 (kvašení, var)
        public float swirlDps;   // otáčení hladiny ve °/s (míchadlo, kopačka)
    }

    public static class LiquidLook
    {
        // ladění vzhledu
        public const float WaterAlpha = 0.34f;      // průhlednost čisté vody (0 = neviditelná, 1 = neprůhledná)
        public const float WortEmit = 38f;          // vlastní záře sladiny / mladiny (nit) – jen aby nebyla černá ve stínu
        public const float WaterEmit = 14f;
        public const float MashEmit = 22f;
        public const float PipeGlassAlpha = 0.16f;  // průhlednost stěny trubky s vodou uvnitř
        public const float PipeEmptyAlpha = 0.5f;   // prázdná trubka

        // ───────────────────────── šum a textury ─────────────────────────
        static float[] Noise(int n, int cx, int cy, int seed)
        {
            var rnd = new System.Random(seed);
            var lat = new float[cx * cy];
            for (int i = 0; i < lat.Length; i++) lat[i] = (float)rnd.NextDouble();
            var h = new float[n * n];
            for (int y = 0; y < n; y++)
            {
                float fy = (float)y / n * cy; int iy = (int)fy; float ty = fy - iy; ty = ty * ty * (3f - 2f * ty);
                int y0 = iy % cy, y1 = (iy + 1) % cy;
                for (int x = 0; x < n; x++)
                {
                    float fx = (float)x / n * cx; int ix = (int)fx; float tx = fx - ix; tx = tx * tx * (3f - 2f * tx);
                    int x0 = ix % cx, x1 = (ix + 1) % cx;
                    float a = Mathf.Lerp(lat[y0 * cx + x0], lat[y0 * cx + x1], tx);
                    float b = Mathf.Lerp(lat[y1 * cx + x0], lat[y1 * cx + x1], tx);
                    h[y * n + x] = Mathf.Lerp(a, b, ty);
                }
            }
            return h;
        }

        static float[] Fbm(int n, int cx, int cy, int oct, float gain, int seed)
        {
            var sum = new float[n * n]; float amp = 1f, tot = 0f;
            for (int o = 0; o < oct; o++)
            {
                var nz = Noise(n, cx << o, cy << o, seed + o * 17);
                for (int i = 0; i < sum.Length; i++) sum[i] += nz[i] * amp;
                tot += amp; amp *= gain;
            }
            for (int i = 0; i < sum.Length; i++) sum[i] /= tot;
            return sum;
        }

        static Texture2D NormalFrom(float[] h, int n, float strength)
        {
            var px = new Color32[n * n];
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    float dx = h[y * n + (x + 1) % n] - h[y * n + (x + n - 1) % n];
                    float dy = h[((y + 1) % n) * n + x] - h[((y + n - 1) % n) * n + x];
                    Vector3 v = new Vector3(-dx * strength, -dy * strength, 1f).normalized;
                    px[y * n + x] = new Color32((byte)((v.x * 0.5f + 0.5f) * 255f), (byte)((v.y * 0.5f + 0.5f) * 255f), (byte)((v.z * 0.5f + 0.5f) * 255f), 255);
                }
            var t = new Texture2D(n, n, TextureFormat.RGBA32, true, true);
            t.SetPixels32(px);
            t.wrapMode = TextureWrapMode.Repeat; t.filterMode = FilterMode.Trilinear; t.anisoLevel = 4;
            t.Apply(true);
            return t;
        }

        static Texture2D ColorTex(Color[] c, int n)
        {
            var px = new Color32[n * n];
            for (int i = 0; i < px.Length; i++) px[i] = c[i];
            var t = new Texture2D(n, n, TextureFormat.RGBA32, true, false);
            t.SetPixels32(px);
            t.wrapMode = TextureWrapMode.Repeat; t.filterMode = FilterMode.Trilinear; t.anisoLevel = 4;
            t.Apply(true);
            return t;
        }

        static void Stamp(float[] hh, Color[] cc, int n, float x, float y, float rad, Color col, float dh)
        {
            int r = Mathf.CeilToInt(rad);
            for (int oy = -r; oy <= r; oy++)
                for (int ox = -r; ox <= r; ox++)
                {
                    float d = Mathf.Sqrt(ox * ox + oy * oy);
                    if (d > rad) continue;
                    int px = ((int)x + ox) % n; if (px < 0) px += n;
                    int py = ((int)y + oy) % n; if (py < 0) py += n;
                    float k = 1f - d / (rad + 0.001f);
                    int i = py * n + px;
                    cc[i] = Color.Lerp(cc[i], col, Mathf.Clamp01(k * 1.6f));
                    hh[i] += dh * k;
                }
        }

        /// <summary>Šrot a plevy: světlá drť s tmavšími plevami (podlouhlé úlomky) a mouční tečky. Barva je skoro neutrální,
        /// protože se násobí barvou rmutu z Godotu.</summary>
        static void Grist(int n, int seed, float huskDensity, float darken, out Color[] colors, out float[] height)
        {
            var low = Fbm(n, 4, 4, 4, 0.55f, seed);
            var fine = Fbm(n, 24, 24, 2, 0.5f, seed + 5);
            colors = new Color[n * n]; height = new float[n * n];
            for (int i = 0; i < colors.Length; i++)
            {
                float v = (0.80f + 0.30f * low[i]) * (0.90f + 0.20f * fine[i]) * darken;
                colors[i] = new Color(Mathf.Clamp01(v * 1.00f), Mathf.Clamp01(v * 0.96f), Mathf.Clamp01(v * 0.88f), 1f);
                height[i] = low[i] * 0.6f + fine[i] * 0.4f;
            }
            var rnd = new System.Random(seed * 31 + 7);
            int husks = (int)(huskDensity * n * n / 28f);
            for (int k = 0; k < husks; k++)
            {
                float cx = (float)rnd.NextDouble() * n, cy = (float)rnd.NextDouble() * n;
                float ang = (float)rnd.NextDouble() * Mathf.PI;
                float len = 3f + (float)rnd.NextDouble() * 7f, wid = 0.9f + (float)rnd.NextDouble() * 1.2f;
                float tone = (float)rnd.NextDouble();
                Color hc = Color.Lerp(new Color(0.42f, 0.30f, 0.17f), new Color(0.78f, 0.64f, 0.40f), tone) * darken;
                hc.a = 1f;
                for (float s = -len; s <= len; s += 0.8f)
                    Stamp(height, colors, n, cx + Mathf.Cos(ang) * s, cy + Mathf.Sin(ang) * s * 0.9f, wid, hc, 0.07f);
            }
            int flour = (int)(huskDensity * n * n / 14f);
            for (int k = 0; k < flour; k++)
                Stamp(height, colors, n, (float)rnd.NextDouble() * n, (float)rnd.NextDouble() * n, 1.0f + (float)rnd.NextDouble(), new Color(darken, 0.98f * darken, 0.92f * darken, 1f), 0.03f);
        }

        // ───────────────────────── sdílené textury ─────────────────────────
        static Texture2D ripple, flowN, flowA, mashA, mashN, bedA, bedN, whiteT;

        public static Texture2D White
        {
            get
            {
                if (whiteT == null)
                {
                    whiteT = new Texture2D(4, 4, TextureFormat.RGBA32, false, false);
                    var px = new Color32[16]; for (int i = 0; i < 16; i++) px[i] = new Color32(255, 255, 255, 255);
                    whiteT.SetPixels32(px); whiteT.Apply(false);
                }
                return whiteT;
            }
        }

        /// <summary>Jemné vlnky hladiny (normálová mapa).</summary>
        public static Texture2D Ripple { get { if (ripple == null) ripple = NormalFrom(Fbm(128, 3, 3, 4, 0.5f, 11), 128, 5f); return ripple; } }

        /// <summary>Proužky proudu v trubce: protáhlé podél směru toku (osa x textury = délka trubky).</summary>
        public static Texture2D FlowNormal { get { if (flowN == null) flowN = NormalFrom(Fbm(128, 2, 10, 3, 0.55f, 23), 128, 4f); return flowN; } }

        /// <summary>Barevné proužky a drobné bublinky ve vodě v trubce.</summary>
        public static Texture2D FlowColor
        {
            get
            {
                if (flowA == null)
                {
                    const int n = 128;
                    var s = Fbm(n, 2, 10, 3, 0.55f, 29);
                    var c = new Color[n * n];
                    for (int i = 0; i < c.Length; i++) { float v = 0.78f + 0.22f * s[i]; c[i] = new Color(v, v, v, 1f); }
                    var rnd = new System.Random(5);
                    for (int k = 0; k < 90; k++)
                    {
                        float x = (float)rnd.NextDouble() * n, y = (float)rnd.NextDouble() * n;
                        float r = 0.8f + (float)rnd.NextDouble() * 1.6f;
                        for (int oy = -2; oy <= 2; oy++)
                            for (int ox = -3; ox <= 3; ox++)
                            {
                                float d = Mathf.Sqrt(ox * ox / 2.2f + oy * oy);
                                if (d > r) continue;
                                int px = ((int)x + ox + n) % n, py = ((int)y + oy + n) % n;
                                float t = 1f - d / r;
                                c[py * n + px] = Color.Lerp(c[py * n + px], Color.white, t * 0.85f);
                            }
                    }
                    flowA = ColorTex(c, n);
                }
                return flowA;
            }
        }

        public static Texture2D MashAlbedo { get { EnsureMash(); return mashA; } }
        public static Texture2D MashNormal { get { EnsureMash(); return mashN; } }
        public static Texture2D BedAlbedo { get { EnsureBed(); return bedA; } }
        public static Texture2D BedNormal { get { EnsureBed(); return bedN; } }

        static void EnsureMash()
        {
            if (mashA != null) return;
            Color[] c; float[] h;
            Grist(256, 3, 1.0f, 1.0f, out c, out h);
            mashA = ColorTex(c, 256); mashN = NormalFrom(h, 256, 7f);
        }

        static void EnsureBed()
        {
            if (bedA != null) return;
            Color[] c; float[] h;
            Grist(256, 9, 1.8f, 0.78f, out c, out h);   // mláto: víc plev, tmavší, vlhké
            bedA = ColorTex(c, 256); bedN = NormalFrom(h, 256, 9f);
        }

        // ───────────────────────── materiály ─────────────────────────
        /// <summary>Nový materiál kapaliny: průhledný, jen přední strany (zadní strana válce by jinak prosvítala).</summary>
        public static Material NewLiquidMaterial()
        {
            var m = HdrpUtil.Transparent(new Color(0.8f, 0.6f, 0.2f, 0.8f), 0f, 0.9f);
            HdrpUtil.SetCull(m, 2);
            HdrpUtil.SetNormalMap(m, Ripple, 0.3f, new Vector2(4f, 2f));
            return m;
        }

        static float Smooth(float a, float b, float x) { x = Mathf.Clamp01((x - a) / (b - a)); return x * x * (3f - 2f * x); }

        public struct Params
        {
            public Color col; public float smooth, emit, normalScale; public bool mashy; public float thick;
        }

        /// <summary>Z popisu obsahu spočte fyzikálně podložený vzhled: čistá voda je skoro průhledná a modravá, sladina a mladina jsou
        /// jantarové s průhledností podle barvy a zákalu, rmut je neprůhledná kaše se šrotem a plevami.</summary>
        public static Params Evaluate(LiquidState s)
        {
            var p = new Params();
            float thick = Smooth(0.015f, 0.10f, s.solids);
            if (s.kind == LiqKind.Mash) thick = Mathf.Max(thick, 0.35f);
            p.thick = thick;
            Color c = s.col;
            float lum = 0.3f * c.r + 0.59f * c.g + 0.11f * c.b;
            if (s.kind == LiqKind.Water || s.kind == LiqKind.Empty)
            {
                // čistá voda: lehce modrozelená, horká o trochu světlejší (pára, mikrobublinky)
                float hot = Mathf.Clamp01((s.T - 50f) / 45f);
                p.col = new Color(0.62f, 0.80f, 0.92f, Mathf.Lerp(WaterAlpha, WaterAlpha + 0.1f, hot));
                p.smooth = 0.97f; p.emit = WaterEmit; p.normalScale = 0.25f + 0.55f * s.stir + 0.8f * s.boil;
                p.mashy = false;
                return p;
            }
            // sladina / mladina: tmavší = neprůhlednější, zákal zhoršuje lesk
            float alphaWort = Mathf.Clamp01(0.48f + 0.5f * (1f - lum) + 0.22f * s.turb);
            float smoothWort = Mathf.Lerp(0.93f, 0.55f, s.turb);
            // rmut: neprůhledný, matný
            Color tint = new Color(Mathf.Clamp01(c.r * 1.12f), Mathf.Clamp01(c.g * 1.12f), Mathf.Clamp01(c.b * 1.12f), 1f);
            p.col = Color.Lerp(new Color(c.r, c.g, c.b, alphaWort), tint, thick);
            p.col.a = Mathf.Lerp(alphaWort, 1f, thick);
            p.smooth = Mathf.Lerp(smoothWort, 0.28f, thick);
            p.emit = Mathf.Lerp(WortEmit, MashEmit, thick);
            p.normalScale = Mathf.Lerp(0.25f + 0.5f * s.stir + 0.9f * s.boil, 1.0f, thick);
            p.mashy = thick > 0.12f;
            return p;
        }

        /// <summary>Aplikuje vzhled na materiál boku / dna kapaliny.</summary>
        public static void ApplyBody(Material m, Params p, ref bool lastMashy, float tileU, float tileV)
        {
            if (m == null) return;
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", p.col); else m.color = p.col;
            if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", p.smooth);
            Color e = new Color(p.col.r, p.col.g, p.col.b);
            HdrpUtil.SetEmissive(m, e, p.emit);
            if (m.HasProperty("_NormalScale")) m.SetFloat("_NormalScale", p.normalScale);
            if (p.mashy != lastMashy)
            {
                lastMashy = p.mashy;
                if (p.mashy)
                {
                    HdrpUtil.SetBaseMap(m, MashAlbedo, new Vector2(tileU, tileV));
                    HdrpUtil.SetNormalMap(m, MashNormal, 1.0f, new Vector2(tileU, tileV));
                    HdrpUtil.SetZWrite(m, true);
                }
                else
                {
                    HdrpUtil.SetBaseMap(m, null, new Vector2(1f, 1f));
                    HdrpUtil.SetNormalMap(m, Ripple, 0.3f, new Vector2(tileU * 0.5f, tileV * 0.5f));
                    HdrpUtil.SetZWrite(m, false);
                }
            }
        }

        /// <summary>Vodorovný kruh (hladina), rovinná UV v metrech, normála nahoru.</summary>
        public static Mesh Disc(float r, int seg)
        {
            var verts = new List<Vector3>(); var norms = new List<Vector3>(); var uvs = new List<Vector2>(); var idx = new List<int>();
            verts.Add(Vector3.zero); norms.Add(Vector3.up); uvs.Add(Vector2.zero);
            for (int k = 0; k <= seg; k++)
            {
                float a = Mathf.PI * 2f * k / seg;
                float x = Mathf.Cos(a) * r, z = Mathf.Sin(a) * r;
                verts.Add(new Vector3(x, 0f, z)); norms.Add(Vector3.up); uvs.Add(new Vector2(x, z));
            }
            for (int k = 0; k < seg; k++) { idx.Add(0); idx.Add(k + 2); idx.Add(k + 1); }
            return MeshKit.Finish(verts, norms, uvs, idx);
        }

        // ───────────────────────── voda v trubce ─────────────────────────
        /// <summary>Sloupec kapaliny v trubce. Vodorovná trubka je zaplněná do výšky depth (hladina je rovná),
        /// svislá se plní zdola. U prázdné části se vrchol stáhne na dno, takže nevzniká žádná plocha.</summary>
        public static Mesh PipeLiquid(IList<Vector3> pts, float rad, int seg, float depth)
        {
            int n = pts.Count;
            var verts = new List<Vector3>(); var norms = new List<Vector3>(); var uvs = new List<Vector2>(); var idx = new List<int>();
            if (n < 2 || depth <= 0.02f) return MeshKit.Finish(verts, norms, uvs, idx);
            depth = Mathf.Clamp01(depth);
            float minY = float.MaxValue, maxY = float.MinValue;
            for (int i = 0; i < n; i++) { minY = Mathf.Min(minY, pts[i].y); maxY = Mathf.Max(maxY, pts[i].y); }
            float vLevel = (minY - rad) + depth * ((maxY + rad) - (minY - rad));
            float acc = 0f;
            for (int i = 0; i < n; i++)
            {
                Vector3 tg;
                if (i == 0) tg = pts[1] - pts[0];
                else if (i == n - 1) tg = pts[i] - pts[i - 1];
                else tg = (pts[i] - pts[i - 1]).normalized + (pts[i + 1] - pts[i]).normalized;
                if (tg.sqrMagnitude < 1e-8f) tg = Vector3.right;
                tg.Normalize();
                Vector3 up = Mathf.Abs(Vector3.Dot(tg, Vector3.up)) < 0.95f ? Vector3.up : Vector3.right;
                Vector3 sd = Vector3.Cross(tg, up).normalized;
                Vector3 u = Vector3.Cross(sd, tg).normalized;
                if (i > 0) acc += (pts[i] - pts[i - 1]).magnitude;
                float tVert = Smooth(0.55f, 0.9f, Mathf.Abs(tg.y));
                float hLevel = pts[i].y - rad + 2f * rad * depth;
                float level = Mathf.Lerp(hLevel, vLevel, tVert);
                bool empty = tVert < 0.5f && depth < 0.04f;
                for (int k = 0; k <= seg; k++)
                {
                    float ang = Mathf.PI * 2f * k / seg;
                    Vector3 nn = sd * Mathf.Cos(ang) + u * Mathf.Sin(ang);
                    Vector3 pos = pts[i] + nn * (rad * 0.93f);
                    Vector3 nrm = nn;
                    if (empty) { pos = pts[i] + Vector3.down * (rad * 0.93f); nrm = Vector3.up; }
                    else if (pos.y > level)
                    {
                        pos.y = Mathf.Max(level, tVert < 0.5f ? pts[i].y - rad * 0.93f : level);
                        nrm = Vector3.up;
                    }
                    verts.Add(pos); norms.Add(nrm); uvs.Add(new Vector2(acc, (float)k / seg));
                }
            }
            for (int i = 0; i < n - 1; i++)
                for (int k = 0; k < seg; k++)
                {
                    int a = i * (seg + 1) + k, b = a + 1, c = a + seg + 1, d = c + 1;
                    idx.Add(a); idx.Add(c); idx.Add(b);
                    idx.Add(b); idx.Add(c); idx.Add(d);
                }
            return MeshKit.Finish(verts, norms, uvs, idx);
        }
    }
}
