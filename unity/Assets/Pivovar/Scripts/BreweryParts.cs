// Ostatní díly z 2D plánu: průhledítka a armatury v potrubí, rotametr, chmelový filtr, T-kus, trysky, sprchy,
// rozbočovač, stavoznak, měrka kotle, spínač, dopravník, víčko, výhoz, rozvaděč.
// Souřadnice uvnitř dílu: x doprava, y nahoru, kladné "z" (konvence Godotu) k divákovi – jako ve funkci Add().
using System.Collections.Generic;
using UnityEngine;
using Pivovar.Core;

namespace Pivovar
{
    public partial class BreweryBuilder
    {
        Material wood, white, redM, blueM, greenGlow, hopGreen, sightLiq, orange, foamMat, bubbleMat;

        static Texture2D bubbleTex;
        static Texture2D BubbleTexture()
        {
            if (bubbleTex != null) return bubbleTex;
            const int n = 32;
            bubbleTex = new Texture2D(n, n, TextureFormat.RGBA32, true, false);
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), new Vector2(n * 0.5f - 0.5f, n * 0.5f - 0.5f)) / (n * 0.5f);
                    float ring = Mathf.Clamp01(1f - Mathf.Abs(d - 0.82f) / 0.14f);
                    float fill = d < 0.82f ? 0.18f : 0f;
                    float hl = Mathf.Clamp01(1f - Vector2.Distance(new Vector2(x, y), new Vector2(n * 0.34f, n * 0.66f)) / 4f) * 0.9f;
                    float a = Mathf.Clamp01(Mathf.Max(ring, fill) + hl);
                    if (d > 1f) a = 0f;
                    bubbleTex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                }
            bubbleTex.wrapMode = TextureWrapMode.Clamp; bubbleTex.filterMode = FilterMode.Bilinear;
            bubbleTex.Apply(true);
            return bubbleTex;
        }

        void MakePartMaterials()
        {
            foamMat = HdrpUtil.Transparent(new Color(0.97f, 0.91f, 0.78f, 0.82f), 0f, 0.25f);
            bubbleMat = HdrpUtil.Transparent(Color.white, 0f, 0.9f);
            if (bubbleMat.HasProperty("_BaseColorMap")) bubbleMat.SetTexture("_BaseColorMap", BubbleTexture());
            HdrpUtil.SetEmissive(bubbleMat, new Color(1f, 0.93f, 0.75f), 60f);
            wood = HdrpUtil.Lit(new Color(0.72f, 0.55f, 0.32f), 0f, 0.35f);
            white = HdrpUtil.Lit(new Color(0.88f, 0.89f, 0.9f), 0f, 0.55f);
            redM = HdrpUtil.Lit(new Color(0.78f, 0.12f, 0.08f), 0f, 0.5f);
            blueM = HdrpUtil.Lit(new Color(0.12f, 0.35f, 0.75f), 0.2f, 0.5f);
            orange = HdrpUtil.Lit(new Color(0.95f, 0.55f, 0.12f), 0f, 0.5f);
            greenGlow = HdrpUtil.Lit(new Color(0.2f, 0.85f, 0.3f), 0f, 0.4f);
            HdrpUtil.SetEmissive(greenGlow, new Color(0.2f, 0.85f, 0.3f), 300f);
            hopGreen = HdrpUtil.Lit(new Color(0.30f, 0.45f, 0.14f), 0f, 0.6f);
            sightLiq = HdrpUtil.Transparent(new Color(0.9f, 0.58f, 0.12f, 0.75f), 0f, 0.3f);
            HdrpUtil.SetEmissive(sightLiq, new Color(0.9f, 0.58f, 0.12f), 120f);
        }

        Transform PartNode(PlanItem it, float rotDeg, float zGodot)
        {
            var n = new GameObject(it.Type + " " + it.Id).transform;
            n.SetParent(root, false);
            n.localPosition = Pp(it.X, it.Y, zGodot);
            n.localRotation = Quaternion.Euler(0f, 0f, -rotDeg);
            partObjs[it.Id] = n.gameObject;
            return n;
        }

        void Flange(Transform n, float dx, float r = 0.052f)
        {
            CylX(n, r, 0.016f, steelP, new Vector3(dx, 0, 0), 24);
            CylX(n, r * 0.92f, 0.006f, rubber, new Vector3(dx + (dx < 0 ? 0.011f : -0.011f), 0, 0), 24);
        }

        /// <summary>Kosmetické vložky z plánu, které nejsou trubka ani nádoba.</summary>
        void BuildExtras()
        {
            foreach (PlanItem it in plan.Items)
            {
                switch (it.Type)
                {
                    case "inl": BuildInl(it); break;
                    case "flow": BuildFlow(it); break;
                    case "hopf": BuildHopf(it); break;
                    case "fitting": BuildFitting(it); break;
                    case "nozzle": BuildNozzle(it); break;
                    case "spray": BuildSpray(it); break;
                    case "manifold": BuildManifold(it); break;
                    case "gauge": BuildGauge(it); break;
                    case "sw": BuildSwitch(it); break;
                    case "dip": BuildDip(it); break;
                    case "panel": BuildPanel(it); break;
                }
            }
        }

        // ───────────── průhledítka a armatury v potrubí ─────────────
        void BuildInl(PlanItem it)
        {
            string k = it.Str("k", "");
            bool sensor = k == "tep" || k == "man" || k == "pt" || k == "hlad" || k == "poj" || k == "odv" || k == "vzk" || k == "vyp" || k == "tc" || k == "aer" || k == "redp";
            Transform n = PartNode(it, sensor ? 0f : (float)it.Num("rot", 0), ZP);
            switch (k)
            {
                case "pruh":
                    Flange(n, -0.085f); Flange(n, 0.085f);
                    CylX(n, 0.046f, 0.155f, glassMat, Vector3.zero, 24);
                    CylX(n, 0.030f, 0.15f, sightLiq, Vector3.zero, 20);
                    for (int i = 0; i < 2; i++) CylX(n, 0.050f, 0.012f, steelB, new Vector3(i == 0 ? -0.07f : 0.07f, 0, 0), 24);
                    Box(n, new Vector3(0.15f, 0.006f, 0.006f), dark, new Vector3(0, 0.05f, 0.0f), Vector3.zero);
                    Box(n, new Vector3(0.15f, 0.006f, 0.006f), dark, new Vector3(0, -0.05f, 0.0f), Vector3.zero);
                    break;
                case "zpk":
                    Flange(n, -0.07f); Flange(n, 0.07f);
                    CylX(n, 0.05f, 0.12f, steelB, Vector3.zero, 24);
                    Box(n, new Vector3(0.04f, 0.04f, 0.012f), dark, new Vector3(0, 0, 0.052f), Vector3.zero);
                    break;
                case "ysit":
                    Flange(n, -0.08f); Flange(n, 0.08f);
                    CylX(n, 0.05f, 0.14f, steelB, Vector3.zero, 24);
                    Cyl(n, 0.032f, 0.14f, steelP, new Vector3(0.03f, -0.09f, 0), new Vector3(0, 0, 35f), 20);
                    Cyl(n, 0.04f, 0.02f, steelP, new Vector3(0.062f, -0.15f, 0), new Vector3(0, 0, 35f), 20);
                    break;
                case "red":
                    Cyl(n, 0.050f, 0.16f, steelP, Vector3.zero, new Vector3(0, 0, 90f), 24, 0.028f);
                    break;
                case "odk":
                    Flange(n, -0.075f); Flange(n, 0.075f);
                    Add(n, MeshKit.Sphere(0.06f, 20, 12), HdrpUtil.Lit(new Color(0.42f, 0.45f, 0.48f), 0.8f, 0.5f), Vector3.zero, Vector3.zero, "odkalovač");
                    break;
                case "redp":
                    Flange(n, -0.07f); Flange(n, 0.07f);
                    Box(n, new Vector3(0.12f, 0.09f, 0.09f), steelB, Vector3.zero, Vector3.zero);
                    Cyl(n, 0.012f, 0.1f, steelP, new Vector3(0, 0.09f, 0), Vector3.zero, 12);
                    Cyl(n, 0.05f, 0.07f, redM, new Vector3(0, 0.17f, 0), Vector3.zero, 24, 0.03f);
                    break;
                case "aer":
                    Box(n, new Vector3(0.16f, 0.06f, 0.06f), white, new Vector3(0, 0.03f, 0), Vector3.zero);
                    for (int i = -1; i <= 1; i++) Add(n, MeshKit.Sphere(0.011f, 10, 6), glassMat, new Vector3(i * 0.04f, 0.075f + (i == 0 ? 0.02f : 0f), 0), Vector3.zero, "ventilek");
                    break;
                case "tc":
                    Cyl(n, 0.02f, 0.14f, steelP, new Vector3(0, 0.07f, 0), Vector3.zero, 14);
                    Add(n, MeshKit.Sphere(0.02f, 10, 6), steelB, new Vector3(0, 0.15f, 0), Vector3.zero, "hlavice");
                    break;
                case "tep":
                case "man":
                    Cyl(n, 0.014f, 0.07f, steelP, new Vector3(0, 0.035f, 0), Vector3.zero, 12);
                    Cyl(n, 0.056f, 0.03f, dark, new Vector3(0, 0.12f, 0), new Vector3(90f, 0, 0), 28);
                    Cyl(n, 0.049f, 0.012f, white, new Vector3(0, 0.12f, 0.016f), new Vector3(90f, 0, 0), 28);
                    Box(n, new Vector3(0.004f, 0.04f, 0.004f), k == "tep" ? redM : dark, new Vector3(0.012f, 0.134f, 0.024f), new Vector3(0, 0, -30f));
                    break;
                case "pt":
                    Cyl(n, 0.014f, 0.08f, steelP, new Vector3(0, 0.04f, 0), Vector3.zero, 12);
                    Box(n, new Vector3(0.07f, 0.06f, 0.05f), blueM, new Vector3(0, 0.11f, 0), Vector3.zero);
                    break;
                case "hlad":
                    Cyl(n, 0.022f, 0.2f, glassMat, new Vector3(0, 0.1f, 0), Vector3.zero, 16);
                    Cyl(n, 0.014f, 0.09f, sightLiq, new Vector3(0, 0.055f, 0), Vector3.zero, 12);
                    break;
                case "poj":
                    Cyl(n, 0.016f, 0.05f, steelP, new Vector3(0, 0.025f, 0), Vector3.zero, 12);
                    Box(n, new Vector3(0.08f, 0.04f, 0.05f), redM, new Vector3(0, 0.065f, 0), Vector3.zero);
                    Cyl(n, 0.008f, 0.06f, redM, new Vector3(-0.02f, 0.115f, 0), Vector3.zero, 8);
                    Cyl(n, 0.008f, 0.06f, redM, new Vector3(0.02f, 0.115f, 0), Vector3.zero, 8);
                    break;
                case "odv":
                    Cyl(n, 0.014f, 0.06f, steelP, new Vector3(0, 0.03f, 0), Vector3.zero, 12);
                    Add(n, MeshKit.Sphere(0.03f, 14, 8), steelP, new Vector3(0, 0.085f, 0), Vector3.zero, "odvzdušnění");
                    break;
                case "vzk":
                case "vyp":
                    {
                        float s = k == "vyp" ? -1f : 1f;
                        Cyl(n, 0.018f, 0.1f, steelP, new Vector3(0, s * 0.05f, 0), Vector3.zero, 12);
                        Add(n, MeshKit.Sphere(0.026f, 12, 8), steelP, new Vector3(0, s * 0.11f, 0), Vector3.zero, "kohout");
                        Box(n, new Vector3(0.06f, 0.014f, 0.014f), redM, new Vector3(0.03f, s * 0.11f, 0), Vector3.zero);
                        break;
                    }
                default:
                    Flange(n, -0.07f); Flange(n, 0.07f);
                    CylX(n, 0.045f, 0.12f, steelB, Vector3.zero, 24);
                    break;
            }
        }

        void BuildFlow(PlanItem it)
        {
            Transform n = PartNode(it, (float)it.Num("rot", 0), ZP);
            Flange(n, -0.095f); Flange(n, 0.095f);
            CylX(n, 0.04f, 0.18f, steelB, Vector3.zero, 20);
            var up = new GameObject("rotametr").transform; up.SetParent(n, false);
            up.rotation = Quaternion.identity;
            Cyl(up, 0.034f, 0.34f, glassMat, new Vector3(0, 0.15f, 0), Vector3.zero, 24);
            Cyl(up, 0.045f, 0.03f, steelP, new Vector3(0, -0.02f, 0), Vector3.zero, 24);
            Cyl(up, 0.045f, 0.03f, steelP, new Vector3(0, 0.32f, 0), Vector3.zero, 24);
            Add(up, MeshKit.Sphere(0.016f, 12, 8), redM, new Vector3(0, 0.12f, 0), Vector3.zero, "plovák");
            for (int i = -2; i <= 2; i++) Box(up, new Vector3(0.03f, 0.003f, 0.003f), white, new Vector3(0.0f, 0.15f + i * 0.06f, 0.036f), Vector3.zero);
        }

        void BuildHopf(PlanItem it)
        {
            Transform n = PartNode(it, 0f, ZP);
            Flange(n, -0.115f); Flange(n, 0.115f);
            Box(n, new Vector3(0.20f, 0.31f, 0.17f), steelB, new Vector3(0, 0.075f, 0), Vector3.zero);
            Box(n, new Vector3(0.23f, 0.045f, 0.20f), steelP, new Vector3(0, 0.2375f, 0), Vector3.zero);
            Cyl(n, 0.03f, 0.04f, steelP, new Vector3(0, 0.28f, 0), Vector3.zero, 16);
            Box(n, new Vector3(0.13f, 0.23f, 0.10f), hopGreen, new Vector3(0, 0.075f, 0.01f), Vector3.zero);
            Box(n, new Vector3(0.14f, 0.24f, 0.004f), glassMat, new Vector3(0, 0.075f, 0.092f), Vector3.zero);
            for (int i = 0; i < 6; i++)
                Add(n, MeshKit.Sphere(0.018f, 8, 5), HdrpUtil.Lit(new Color(0.55f, 0.75f, 0.25f), 0f, 0.5f), new Vector3((i % 3 - 1) * 0.035f, 0.02f + (i / 3) * 0.07f, 0.075f), Vector3.zero, "chmel");
        }

        void BuildFitting(PlanItem it)
        {
            Transform n = PartNode(it, 0f, ZP);
            float arm = (float)it.Num("arm", 30) * K;
            Add(n, MeshKit.Sphere(0.04f, 18, 10), steelB, Vector3.zero, Vector3.zero, "T-kus");
            object o;
            var angs = new List<double>();
            if (it.D.TryGetValue("ports", out o) && o is List<object>) foreach (object q in (List<object>)o) if (q is double) angs.Add((double)q);
            if (angs.Count == 0) { angs.Add(0); angs.Add(90); }
            foreach (double a in angs)
            {
                float r = (float)a * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(Mathf.Cos(r), -Mathf.Sin(r), 0f);
                AddRaw(n, MeshKit.Tube(new List<Vector3> { Vector3.zero, dir * arm }, 0.03f, 14), steelP, Vector3.zero, Vector3.zero, "hrdlo");
                ClampAt(n, dir * arm * 0.92f, dir, 0.03f);
            }
        }

        void BuildNozzle(PlanItem it)
        {
            Transform n = PartNode(it, 0f, ZP);
            float a = (float)it.Num("dir", 0) * Mathf.Deg2Rad;
            Vector3 f = new Vector3(Mathf.Cos(a), -Mathf.Sin(a), 0f);
            var go = Cyl(n, 0.016f, 0.09f, steelP, Vector3.zero, Vector3.zero, 20, 0.036f);
            go.transform.localPosition = f * 0.045f;
            go.transform.localRotation = Quaternion.FromToRotation(Vector3.up, f);
        }

        void BuildSpray(PlanItem it)
        {
            Transform n = PartNode(it, 0f, ZP);
            Cyl(n, 0.06f, 0.022f, steelP, Vector3.zero, Vector3.zero, 24);
            Cyl(n, 0.03f, 0.03f, steelB, new Vector3(0, 0.024f, 0), Vector3.zero, 16);
            for (int i = 0; i < 8; i++)
            {
                float a = Mathf.PI * 2f * i / 8f;
                Cyl(n, 0.006f, 0.012f, dark, new Vector3(Mathf.Cos(a) * 0.042f, -0.006f, Mathf.Sin(a) * 0.042f), Vector3.zero, 8);
            }
        }

        void BuildManifold(PlanItem it)
        {
            float g = (float)it.Num("gap", 60), W = (float)it.Num("cols", 4) * g, H = (float)it.Num("rows", 3) * g;
            double rotDeg = it.Num("rot", 0);
            int cols = (int)it.Num("cols", 4), rows = (int)it.Num("rows", 3);
            string back = it.Str("back", "top");
            Vector2 ctr = new Vector2((float)it.X + W * 0.5f, (float)it.Y + H * 0.5f);
            var n = new GameObject("rozbočovač " + it.Id).transform; n.SetParent(root, false);
            partObjs[it.Id] = n.gameObject;
            n.localPosition = Pp(ctr.x, ctr.y, ZP - 0.07f);
            n.localRotation = Quaternion.Euler(0f, 0f, -(float)rotDeg);
            Box(n, new Vector3(W * K + 0.06f, H * K + 0.06f, 0.14f), HdrpUtil.Lit(new Color(0.12f, 0.14f, 0.16f), 0.4f, 0.4f), Vector3.zero, Vector3.zero);
            Box(n, new Vector3(W * K, H * K, 0.15f), steelB, Vector3.zero, Vector3.zero);
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    Vector3 p = new Vector3((-W * 0.5f + g * 0.5f + c * g) * K, (H * 0.5f - g * 0.5f - r * g) * K, 0.08f);
                    Cyl(n, 0.105f, 0.03f, steelP, p, new Vector3(90f, 0, 0), 28);
                    Cyl(n, 0.052f, 0.034f, dark, p + new Vector3(0, 0, 0.004f), new Vector3(90f, 0, 0), 20);
                    for (int b2 = 0; b2 < 4; b2++)
                    {
                        float a = Mathf.PI * 0.5f * b2 + Mathf.PI * 0.25f;
                        Cyl(n, 0.011f, 0.036f, steelB, p + new Vector3(Mathf.Cos(a) * 0.075f, Mathf.Sin(a) * 0.075f, 0.002f), new Vector3(90f, 0, 0), 8);
                    }
                }
            // trubky z otvorů ven (jako mf_ports ve 2D)
            float rad = (float)(rotDeg * Mathf.Deg2Rad);
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    float nx = (float)it.X + g * 0.5f + c * g, ny = (float)it.Y + g * 0.5f + r * g;
                    Vector2 bp;
                    float sp = Mathf.Min(12f, g / (rows + 1f));
                    float spc = Mathf.Min(12f, g / (cols + 1f));
                    float y0 = (float)it.Y;
                    if (back == "both")
                    {
                        int t = Mathf.CeilToInt(rows / 2f); bool up = r < t; int cnt = up ? t : rows - t; int j = up ? r : r - t;
                        bp = new Vector2(nx + (j - (cnt - 1) / 2f) * Mathf.Min(12f, g / (cnt + 1f)), up ? y0 - 16f : y0 + H + 16f);
                    }
                    else if (back == "top") bp = new Vector2(nx + (r - (rows - 1) / 2f) * sp, y0 - 16f);
                    else if (back == "bottom") bp = new Vector2(nx + (r - (rows - 1) / 2f) * sp, y0 + H + 16f);
                    else if (back == "left") bp = new Vector2((float)it.X - 16f, ny + (c - (cols - 1) / 2f) * spc);
                    else bp = new Vector2((float)it.X + W + 16f, ny + (c - (cols - 1) / 2f) * spc);
                    Vector2 fr = new Vector2(nx, ny);
                    Vector2 fr2 = Rot(fr - ctr, rad) + ctr, bk2 = Rot(bp - ctr, rad) + ctr;
                    AddRaw(root, MeshKit.Tube(new List<Vector3> { Pp(fr2.x, fr2.y, ZP + 0.02f), Pp(bk2.x, bk2.y, ZP + 0.02f) }, 0.03f, 14), steelP, Vector3.zero, Vector3.zero, "výstup");
                }
        }

        static Vector2 Rot(Vector2 v, float a)
        {
            float c = Mathf.Cos(a), s = Mathf.Sin(a);
            return new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
        }

        void BuildGauge(PlanItem it)
        {
            PlanItem v = plan.ById(it.Str("vid", "-"));
            float x, y1, y2, cxw; bool free;
            if (v == null) { x = (float)it.X; y1 = (float)it.Y; y2 = y1 + (float)it.Num("h", 200); cxw = x; free = true; }
            else
            {
                bool right = it.Str("side", "R") != "L";
                x = (float)(right ? v.X + v.W + 22 : v.X - 22);
                y1 = (float)(v.Y + Mathf.Min(30f, (float)v.H * 0.1f));
                y2 = (float)(v.Y + v.H - Mathf.Min(20f, (float)v.H * 0.06f));
                cxw = (float)(right ? v.X + v.W : v.X); free = false;
            }
            var n = new GameObject("stavoznak " + it.Id).transform; n.SetParent(root, false);
            partObjs[it.Id] = n.gameObject;
            float hh = (y2 - y1) * K;
            Vector3 mid = Pp(x, (y1 + y2) * 0.5f, ZP);
            n.localPosition = mid;
            Cyl(n, 0.032f, hh + 0.05f, glassMat, Vector3.zero, Vector3.zero, 20);
            Cyl(n, 0.022f, hh * 0.55f, sightLiq, new Vector3(0, -hh * 0.225f, 0), Vector3.zero, 16);
            for (int s = -1; s <= 1; s += 2)
            {
                Cyl(n, 0.048f, 0.035f, steelP, new Vector3(0, s * (hh * 0.5f + 0.03f), 0), Vector3.zero, 24);
                Cyl(n, 0.044f, 0.01f, rubber, new Vector3(0, s * (hh * 0.5f + 0.012f), 0), Vector3.zero, 24);
            }
            for (int i = 1; i < 10; i++)
                Box(n, new Vector3(i % 5 == 0 ? 0.04f : 0.025f, 0.003f, 0.004f), white, new Vector3(0.055f + (i % 5 == 0 ? 0.01f : 0f), hh * 0.5f - hh * i / 10f, 0.0f), Vector3.zero);
            if (!free)
            {
                foreach (float yy in new float[] { y1, y2 })
                {
                    var pts = new List<Vector3> { Pp(cxw, yy, 0.0f), Pp(x, yy, 0.0f), Pp(x, yy, ZP) };
                    AddRaw(root, MeshKit.Tube(MeshKit.RoundPath(pts, 0.05f), 0.016f, 12), steelP, Vector3.zero, Vector3.zero, "přípojka stavoznaku");
                    Vector3 cock = Pp((cxw + x) * 0.5f, yy, 0.0f);
                    Add(root, MeshKit.Sphere(0.028f, 12, 8), steelB, cock, Vector3.zero, "kohout");
                    Box(root, new Vector3(0.06f, 0.012f, 0.012f), greenGlow, cock + new Vector3(0.03f, 0, 0), Vector3.zero);
                }
            }
        }

        void BuildSwitch(PlanItem it)
        {
            Transform n = PartNode(it, 0f, ZP);
            bool on = it.Bool("on", false);
            Box(n, new Vector3(0.26f, 0.34f, 0.09f), white, Vector3.zero, Vector3.zero);
            Cyl(n, 0.06f, 0.03f, dark, new Vector3(0, 0.04f, 0.055f), new Vector3(90f, 0, 0), 24);
            Box(n, new Vector3(0.03f, 0.11f, 0.03f), redM, new Vector3(on ? 0.03f : -0.03f, 0.075f, 0.075f), new Vector3(0, 0, on ? -22f : 22f));
            Cyl(n, 0.035f, 0.02f, on ? greenGlow : dark, new Vector3(0, -0.1f, 0.05f), new Vector3(90f, 0, 0), 16);
        }

        void BuildDip(PlanItem it)
        {
            PlanItem k = null;
            foreach (PlanItem q in plan.OfType("vessel")) if (q.Bind == "rvp") k = q;
            double x = it.X, y = it.Y, h = it.Num("h", 300);
            if (k != null) { h = k.H - Mathf.Min((float)k.H * 0.12f, 40f); y = k.Y; x = k.X + k.W + 34; }
            var n = new GameObject("měrka " + it.Id).transform; n.SetParent(root, false);
            partObjs[it.Id] = n.gameObject;
            n.localPosition = Pp(x, y + h * 0.5, ZP);
            float hm = (float)h * K;
            Box(n, new Vector3(0.07f, hm, 0.025f), wood, Vector3.zero, Vector3.zero);
            for (int hl = 4; hl <= 50; hl++)
            {
                float ty = -hm * 0.5f + hm * (hl - 3) / 47f;
                bool big = hl % 5 == 0;
                bool red = hl == 10 || hl == 15 || hl == 19 || hl == 20 || hl == 21 || hl == 25 || hl == 30 || hl == 40;
                if (red) Box(n, new Vector3(0.08f, 0.006f, 0.006f), redM, new Vector3(0.004f, ty, 0.014f), Vector3.zero);
                else Box(n, new Vector3(big ? 0.05f : 0.03f, 0.002f, 0.004f), dark, new Vector3(-0.012f, ty, 0.014f), Vector3.zero);
            }
        }

        void BuildPanel(PlanItem it)
        {
            // rozvaděč je ve 2D ovládací obrazovka; ve 3D stojí zmenšený (0,22×) u levé stěny
            float s = 0.22f;
            float w = (float)it.Num("w", 260) * K * s, h = (float)it.Num("h", 320) * K * s;
            var n = new GameObject("rozvaděč " + it.Id).transform; n.SetParent(root, false);
            partObjs[it.Id] = n.gameObject;
            n.localPosition = Pz((float)it.X * K * 0.4f - 3f, h * 0.5f + 0.02f, -2.2f);
            Box(n, new Vector3(w, h, 0.4f), HdrpUtil.Lit(new Color(0.70f, 0.73f, 0.76f), 0.3f, 0.45f), Vector3.zero, Vector3.zero);
            Box(n, new Vector3(w - 0.12f, 0.20f, 0.02f), dark, new Vector3(0, h * 0.5f - 0.16f, 0.21f), Vector3.zero);
            Color[] cs = { new Color(0.18f, 0.75f, 0.36f), new Color(0.88f, 0.25f, 0.18f), new Color(0.95f, 0.76f, 0.19f), new Color(0.18f, 0.48f, 0.84f), new Color(0.93f, 0.95f, 0.96f), new Color(0.18f, 0.75f, 0.36f) };
            int cols = Mathf.Max(1, Mathf.RoundToInt((w - 0.2f) / 0.2f)), rows = Mathf.Max(1, Mathf.RoundToInt((h - 0.5f) / 0.26f));
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    Color col = cs[(r + c) % cs.Length];
                    Material m = HdrpUtil.Lit(col, 0f, 0.4f);
                    HdrpUtil.SetEmissive(m, col, 60f);
                    Vector3 p = new Vector3(-w * 0.5f + 0.15f + c * (w - 0.3f) / Mathf.Max(1, cols - 1 + (cols == 1 ? 1 : 0)), h * 0.5f - 0.5f - r * 0.24f, 0.22f);
                    Box(n, new Vector3(0.14f, 0.2f, 0.02f), white, p, Vector3.zero);
                    Cyl(n, 0.035f, 0.03f, m, p + new Vector3(0, -0.03f, 0.02f), new Vector3(90f, 0, 0), 20);
                }
        }

        // ───────────── dekorace: dopravník, výhoz mláta, víčko spilky ─────────────
        void BuildDecoExtra(PlanItem it)
        {
            string k = it.Str("k", "");
            if (k == "conv")
            {
                float L = (float)it.Num("len", 280) * K;
                var n = new GameObject("dopravník " + it.Id).transform; n.SetParent(root, false);
                partObjs[it.Id] = n.gameObject;
                n.localPosition = Pp(it.X + it.Num("len", 280) * 0.5, it.Y + 11, ZP - 0.05f);
                CylX(n, 0.065f, L, steelB, Vector3.zero, 24);
                for (int i = 0; i < (int)(L / 0.07f); i++)
                    Box(n, new Vector3(0.012f, 0.14f, 0.14f), steelP, new Vector3(-L * 0.5f + 0.04f + i * 0.07f, 0, 0), new Vector3(0, 0, 0));
                float hx = (float)(it.X + 36 - (it.X + it.Num("len", 280) * 0.5)) * K;
                Cyl(n, 0.22f, 0.32f, white, new Vector3(hx, 0.22f, 0), Vector3.zero, 4, 0.065f);
                Cyl(n, 0.12f, 0.06f, steelP, new Vector3(hx, 0.4f, 0), Vector3.zero, 24);
            }
            else if (k == "hatch")
            {
                var n = new GameObject("výhoz " + it.Id).transform; n.SetParent(root, false);
                partObjs[it.Id] = n.gameObject;
                n.localPosition = Pp(it.X, it.Y + 18, ZP - 0.1f);
                Box(n, new Vector3(0.46f, 0.26f, 0.05f), steelB, Vector3.zero, Vector3.zero);
                Box(n, new Vector3(0.06f, 0.18f, 0.03f), dark, new Vector3(0.17f, 0, 0.035f), Vector3.zero);
            }
            else if (k == "splid")
            {
                var n = new GameObject("víčko spilky " + it.Id).transform; n.SetParent(root, false);
                partObjs[it.Id] = n.gameObject;
                n.localPosition = Pp(it.X, it.Y - 10, ZP - 0.1f);
                Cyl(n, 0.22f, 0.03f, steelP, Vector3.zero, Vector3.zero, 28);
                Cyl(n, 0.05f, 0.05f, steelB, new Vector3(0, 0.035f, 0), Vector3.zero, 16);
            }
        }
    }
}
