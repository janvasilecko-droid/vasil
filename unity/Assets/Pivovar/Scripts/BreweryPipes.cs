// Potrubí, armatury, čerpadla a doplňky podle plan.json.
using System.Collections.Generic;
using UnityEngine;
using Pivovar.Core;

namespace Pivovar
{
    public partial class BreweryBuilder
    {
        static Texture2D dashTex;

        static Texture2D DashTexture()
        {
            if (dashTex != null) return dashTex;
            const int n = 64;
            dashTex = new Texture2D(n, 4, TextureFormat.RGBA32, true, false);
            for (int x = 0; x < n; x++)
            {
                float s = x / (float)n;
                float dash = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.30f, 0.45f, s)) * (1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.55f, 0.70f, s)));
                float v = 0.18f + 0.82f * dash;
                for (int y = 0; y < 4; y++) dashTex.SetPixel(x, y, new Color(v, v, v, 1f));
            }
            dashTex.wrapMode = TextureWrapMode.Repeat; dashTex.filterMode = FilterMode.Bilinear;
            dashTex.Apply(true);
            return dashTex;
        }

        static Color MediumColor(string medium)
        {
            switch (medium)
            {
                case "water": return new Color(0.30f, 0.62f, 0.95f);
                case "mash": return new Color(0.78f, 0.55f, 0.22f);
                case "wort": return new Color(0.95f, 0.62f, 0.12f);
                case "steam": return new Color(0.92f, 0.92f, 0.96f);
                case "glycol": return new Color(0.30f, 0.85f, 1.00f);
                default: return new Color(0.45f, 0.70f, 0.90f);
            }
        }

        Vessel VesselAt(double x, double y)
        {
            foreach (Vessel v in vessels)
            {
                PlanItem it = v.item;
                if (x > it.X - 8 && x < it.X + it.W + 8 && y > it.Y - 8 && y < it.Y + it.H + 8) return v;
            }
            return null;
        }

        void ClampAt(Transform parent, Vector3 p, Vector3 dir, float rad) { RealClamp(parent, p, dir, rad); }

        void MakeOriented(Transform parent, Mesh mesh, Material mat, Vector3 unityPos, Quaternion q)
        {
            var go = new GameObject("clamp");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = unityPos; go.transform.localRotation = q;
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            go.AddComponent<MeshRenderer>().sharedMaterial = mat;
        }

        Transform pipesRoot;
        readonly Dictionary<string, GameObject> pipeObjs = new Dictionary<string, GameObject>();
        readonly Dictionary<string, GameObject> partObjs = new Dictionary<string, GameObject>();

        Material hoseMat;
        Material HoseMat()
        {
            if (hoseMat != null) return hoseMat;
            hoseMat = HdrpUtil.Lit(new Color(0.78f, 0.80f, 0.83f), 1f, 0.5f);
            HdrpUtil.SetNormalMap(hoseMat, brushed, 1.0f, new Vector2(40f, 3f));   // nerezový oplet
            return hoseMat;
        }

        void BuildPipes()
        {
            pipesRoot = new GameObject("Potrubí").transform; pipesRoot.SetParent(root, false);
            pipeObjs.Clear();
            foreach (PlanItem it in plan.OfType("pipe")) BuildPipe(it);
        }

        /// <summary>Znovu postaví jednu trubku (po úpravě bodů v editoru).</summary>
        public void RebuildPipe(PlanItem it)
        {
            GameObject old;
            if (pipeObjs.TryGetValue(it.Id, out old) && old != null) DestroyImmediate(old);
            pipeObjs.Remove(it.Id);
            flows.RemoveAll(f => f == null);
            if (it.Type == "pipe") BuildPipe(it);
            SetDemoFlow(demoFlow);
        }

        void BuildPipe(PlanItem it)
        {
            {
                List<double[]> pts = it.Pts();
                if (pts.Count < 2) return;
                var pp = new GameObject("trubka " + it.Id).transform; pp.SetParent(pipesRoot, false);
                pipeObjs[it.Id] = pp.gameObject;
                if (it.IsHose)
                {
                    var hp = new List<Vector3>();
                    foreach (double[] p in pts) hp.Add(Pp(p[0], p[1], ZP + 0.12f));
                    AddRaw(pp, MeshKit.Tube(MeshKit.RoundPath(hp, 0.2f), 0.036f, 14), HoseMat(), Vector3.zero, Vector3.zero, "hadice " + it.Id);
                    foreach (int e in new int[] { 0, hp.Count - 1 })
                    {
                        Vector3 nb = hp[e == 0 ? 1 : hp.Count - 2];
                        Vector3 dr = (nb - hp[e]).normalized;
                        MakeOriented(pp, MeshKit.Cylinder(0.046f, 0.07f, 20), steelP, hp[e] + dr * 0.035f, Quaternion.FromToRotation(Vector3.up, dr));
                    }
                    return;
                }
                var raw = new List<Vector3>();
                foreach (double[] p in pts) raw.Add(Pp(p[0], p[1], ZP));
                float dn2 = (float)it.Num("dn", 50);
                float w2d = dn2 <= 32f ? Mathf.Clamp(dn2 * 0.24f, 3f, 6f) : Mathf.Max(9f, Mathf.Round(12f * dn2 / 50f));   // šířka trubky ve 2D v px
                float rad = w2d * K * 0.5f * 1.3f;
                List<Vector3> path = MeshKit.RoundPath(raw, Mathf.Max(0.09f, rad * 3f));

                // stěna trubky je lehce průhledná (jako nerez s okénkem), uvnitř je viditelný sloupec vody (PipeFlow)
                Material mat = HdrpUtil.Transparent(new Color(0.80f, 0.83f, 0.88f, LiquidLook.PipeEmptyAlpha), 0f, 0.88f);
                var go = AddRaw(pp, MeshKit.Tube(path, rad, 14), mat, Vector3.zero, Vector3.zero, "trubka " + it.Id);
                go.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                var flow = go.AddComponent<PipeFlow>();
                flow.mat = go.GetComponent<MeshRenderer>().material;   // vlastní instance materiálu pro každou trubku
                flow.path = path; flow.rad = rad;
                flow.liquid = MediumColor(it.Str("medium", ""));
                flows.Add(flow);

                // tri-clamp spoje na koncích a po cca 3 m
                for (int e = 0; e < 2; e++)
                {
                    Vector3 dv = e == 0 ? raw[1] - raw[0] : raw[raw.Count - 1] - raw[raw.Count - 2];
                    if (dv.magnitude > 0.01f) ClampAt(pp, raw[e == 0 ? 0 : raw.Count - 1] + dv.normalized * (e == 0 ? 0.06f : -0.06f), dv, rad);
                }
                for (int k = 0; k < raw.Count - 1; k++)
                {
                    Vector3 dv = raw[k + 1] - raw[k];
                    float sl = dv.magnitude;
                    int nj = (int)(sl / 3f);
                    for (int j = 1; j <= nj; j++) ClampAt(pp, raw[k] + dv.normalized * (sl * j / (nj + 1)), dv, rad);
                }
                // nátrubky do nádoby
                for (int e = 0; e < 2; e++)
                {
                    double[] pt = pts[e == 0 ? 0 : pts.Count - 1];
                    Vessel vh = VesselAt(pt[0], pt[1]);
                    if (vh == null) continue;
                    float dx = (float)pt[0] * K - vh.cx;
                    float zs = vh.item.Shape == "horiz" ? vh.z + vh.r * 0.8f : vh.z + Mathf.Sqrt(Mathf.Max(0.02f, vh.r * vh.r - dx * dx));
                    Vector3 a = raw[e == 0 ? 0 : raw.Count - 1];
                    Vector3 b = new Vector3(a.x, a.y, -zs);
                    AddRaw(pp, MeshKit.Tube(new List<Vector3> { a, b }, rad, 14), mat, Vector3.zero, Vector3.zero, "nátrubek");
                    MakeOriented(pp, MeshKit.Cylinder(rad * 2.2f, 0.03f, 20), steelP, b, Quaternion.Euler(90f, 0f, 0f));
                }
            }
        }

        void BuildValve(PlanItem it) { RealValve(it); }

        void BuildPump(PlanItem it) { RealPump(it); }

        void BuildDeco(PlanItem it)
        {
            if (it.Str("k", "") != "vent") { BuildDecoExtra(it); return; }
            float hgt = 70f * K;
            partObjs[it.Id] = Cyl(root, 0.15f, hgt, steelB, new Vector3((float)it.X * K, (floorY - (float)it.Y + 35f) * K, -0.2f), Vector3.zero, 20);
        }

        /// <summary>Při tažení v editoru: přesune klapku / čerpadlo / dekoraci na novou polohu z plánu.</summary>
        public void MovePart(PlanItem it)
        {
            GameObject g;
            if (!partObjs.TryGetValue(it.Id, out g) || g == null) return;
            if (it.Type == "deco") g.transform.localPosition = new Vector3((float)it.X * K, (floorY - (float)it.Y + 35f) * K, -0.2f);
            else
            {
                g.transform.localPosition = Pp(it.X, it.Y, ZP);
                if (it.Type == "valve") g.transform.localRotation = Quaternion.Euler(0f, 0f, -(float)it.Num("rot", 0));
            }
        }

        /// <summary>Postaví jeden díl (klapka, čerpadlo, dekorace) – po přidání v editoru.</summary>
        public void BuildPart(PlanItem it)
        {
            if (it.Type == "valve") BuildValve(it);
            else if (it.Type == "pump") BuildPump(it);
            else if (it.Type == "deco") BuildDeco(it);
            else if (it.Type == "inl") BuildInl(it);
            else if (it.Type == "flow") BuildFlow(it);
            else if (it.Type == "hopf") BuildHopf(it);
            else if (it.Type == "fitting") BuildFitting(it);
            else if (it.Type == "nozzle") BuildNozzle(it);
            else if (it.Type == "spray") BuildSpray(it);
            else if (it.Type == "manifold") BuildManifold(it);
            else if (it.Type == "sw") BuildSwitch(it);
        }

        /// <summary>Díly, které se při tažení v editoru dají přesunout bez přestavby celé scény.</summary>
        public static bool IsLight(PlanItem it)
        {
            return it.Type == "valve" || it.Type == "pump" || (it.Type == "deco" && it.Str("k", "") == "vent");
        }

        /// <summary>Odstraní objekt dílu nebo trubky ze scény (plán se maže zvlášť).</summary>
        public void RemovePart(PlanItem it)
        {
            GameObject g;
            if (partObjs.TryGetValue(it.Id, out g) && g != null) DestroyImmediate(g);
            partObjs.Remove(it.Id);
            if (pipeObjs.TryGetValue(it.Id, out g) && g != null) DestroyImmediate(g);
            pipeObjs.Remove(it.Id);
            flows.RemoveAll(f => f == null);
        }

        /// <summary>Svět (Unity) -> souřadnice plánu v px pro rovinu v hloubce zGodot.</summary>
        public Vector2 ToPlan(Vector3 w) { return new Vector2(w.x / K, floorY - w.y / K); }
        public Vector3 FromPlan(double px, double py, float zGodot) { return Pp(px, py, zGodot); }
        public Vessel VesselOf(PlanItem it) { foreach (Vessel v in vessels) if (v.item == it) return v; return null; }
        public Camera Cam { get { return cam; } }
    }
}
