// Nádoby: rotační plášť s řezem, kapalina, svary, nohy, průlez, průhledítko, míchadlo, hrabla scezováku.
using System.Collections.Generic;
using UnityEngine;
using Pivovar.Core;

namespace Pivovar
{
    /// <summary>Jedna nádoba ve scéně. Simulace později nastaví SetFill a barvu kapaliny.</summary>
    public class Vessel
    {
        public PlanItem item;
        public string bind;
        public GameObject shellCut, shellFull;
        public GameObject liqCyl, liqCone, liqDish;
        public Material liqMat;
        public GameObject foam;
        public ParticleSystem bubbles;
        float boil;
        public float by, hcyl, r, cx, z;
        public SteamPuffs steam;          // pára nad rvp (řídí simulace)
        public float hy, hrr;             // ležatá nádoba: osa (y) a poloměr kapaliny
        public Vector3 hpos;
        public bool cone;
        float fill;

        public void ApplyCut(bool cut)
        {
            if (shellCut != null) shellCut.SetActive(cut);
            if (shellFull != null) shellFull.SetActive(!cut);
        }

        /// <summary>fill 0..1 = podíl válcové části; kužel a dno se plní vždy dřív.</summary>
        public void SetFill(float f)
        {
            fill = Mathf.Clamp01(f);
            bool any = fill > 0.003f;
            if (liqCone != null) liqCone.SetActive(any);
            if (liqDish != null) liqDish.SetActive(any);
            if (liqCyl != null)
            {
                liqCyl.SetActive(any);
                float h = Mathf.Max(0.001f, fill * hcyl);
                liqCyl.transform.localScale = new Vector3(1f, h / hcyl, 1f);
                liqCyl.transform.localPosition = new Vector3(cx, by + h * 0.5f, -z);
                if (foam != null)
                {
                    foam.SetActive(any);
                    foam.transform.localPosition = new Vector3(cx, by + h, -z);
                }
                if (bubbles != null)
                {
                    bubbles.transform.localPosition = new Vector3(cx, by + 0.03f, -z);
                    var m = bubbles.main; m.startLifetime = Mathf.Max(0.3f, (h - 0.03f) / 0.45f);
                }
            }
        }

        /// <summary>Var 0..1: bubliny stoupají od dna, pěna na hladině houstne.</summary>
        public void SetBoil(float k)
        {
            boil = Mathf.Clamp01(k);
            if (bubbles != null)
            {
                var em = bubbles.emission; em.rateOverTime = 220f * boil;
                if (boil > 0.01f) { if (!bubbles.isPlaying) bubbles.Play(); } else bubbles.Stop();
            }
            if (foam != null) foam.transform.localScale = new Vector3(1f, 1f + 4f * boil, 1f);
        }

        public void SetColor(Color c)
        {
            if (liqMat == null) return;
            if (liqMat.HasProperty("_BaseColor")) liqMat.SetColor("_BaseColor", c); else liqMat.color = c;
            HdrpUtil.SetEmissive(liqMat, new Color(c.r, c.g, c.b), 260f);   // vlastní záře kapaliny jako ve videu
        }
    }

    public partial class BreweryBuilder
    {
        static readonly Dictionary<string, float> DemoFill = new Dictionary<string, float>
        { {"rvp", 0.70f}, {"mp", 0.6f}, {"sk", 0.55f}, {"wp", 0.60f}, {"hlt", 0.85f}, {"lv", 0.9f}, {"spilka", 0.5f} };

        Color LiquidColor(string bind)
        {
            if (bind == "hlt" || bind == "lv") return new Color(0.45f, 0.70f, 0.95f, 0.55f);   // voda
            return new Color(0.86f, 0.55f, 0.14f, 0.82f);                                       // sladina / mladina
        }

        void BuildVessel(PlanItem it)
        {
            float w = (float)it.W * K, h = (float)it.H * K;
            float cx = ((float)it.X + (float)it.W * 0.5f) * K;
            float yb = (floorY - (float)it.Y - (float)it.H) * K;
            float r = w * 0.5f;
            string shape = it.Shape;
            var pv = new GameObject("Nádoba " + it.Id).transform;
            pv.SetParent(root, false);

            switch (shape)
            {
                case "platform":
                    BuildPlatform(pv, cx, yb, w); return;
                case "horiz":
                    BuildHoriz(it, pv, cx, yb, w, h); return;
                case "plate":
                    Box(pv, new Vector3(w * 1.6f, h, 0.55f), HdrpUtil.Lit(new Color(0.75f, 0.2f, 0.15f), 0.5f, 0.5f), new Vector3(cx, yb + h * 0.5f, 0f), Vector3.zero);
                    for (int i = 0; i < 14; i++)
                        Box(pv, new Vector3(w * 1.5f, h - 0.1f, 0.012f), steelP, new Vector3(cx, yb + h * 0.5f, -0.28f + i * 0.04f), Vector3.zero);
                    return;
                case "chiller":
                    Box(pv, new Vector3(w, h, 1.1f), HdrpUtil.Lit(new Color(0.22f, 0.48f, 0.70f), 0.4f, 0.55f), new Vector3(cx, yb + h * 0.5f, -0.4f), Vector3.zero);
                    var led = HdrpUtil.Lit(new Color(0.3f, 0.8f, 1f), 0f, 0.3f); HdrpUtil.SetEmissive(led, new Color(0.3f, 0.8f, 1f), 800f);
                    Box(pv, new Vector3(w * 0.9f, 0.2f, 0.05f), led, new Vector3(cx, yb + h * 0.7f, 0.16f), Vector3.zero);
                    return;
                case "inlet":
                    Cyl(pv, 0.12f, 0.5f, inox, new Vector3(cx, yb + 0.25f, 0f), Vector3.zero, 16);
                    return;
                default:
                    BuildTank(it, pv, shape, cx, yb, r, h); return;
            }
        }

        void BuildPlatform(Transform pv, float cx, float yb, float w)
        {
            Material deck = HdrpUtil.Lit(new Color(0.72f, 0.74f, 0.78f), 0.55f, 0.45f);
            Box(pv, new Vector3(w, 0.1f, 5.2f), deck, new Vector3(cx, yb + 0.05f, -2f), Vector3.zero);
            for (int i = 0; i < 14; i++) Box(pv, new Vector3(w, 0.012f, 0.03f), steelB, new Vector3(cx, yb + 0.106f, -4.5f + i * 0.4f), Vector3.zero);
            Box(pv, new Vector3(w, 0.14f, 0.05f), steelP, new Vector3(cx, yb - 0.02f, 0.58f), Vector3.zero);
            Box(pv, new Vector3(w, 0.14f, 0.05f), steelP, new Vector3(cx, yb - 0.02f, -4.58f), Vector3.zero);
            for (int s = 0; s < 6; s++)
            {
                float px = cx - w * 0.5f + 0.25f + s * (w - 0.5f) / 5f;
                Box(pv, new Vector3(0.1f, yb, 0.1f), steelB, new Vector3(px, yb * 0.5f, 0.45f), Vector3.zero);
                Box(pv, new Vector3(0.1f, yb, 0.1f), steelB, new Vector3(px, yb * 0.5f, -4.45f), Vector3.zero);
            }
            for (int s = 0; s < 11; s++)
            {
                float rx = cx - w * 0.5f + 0.1f + s * (w - 0.2f) / 10f;
                Cyl(pv, 0.02f, 1.05f, steelP, new Vector3(rx, yb + 0.62f, 0.55f), Vector3.zero, 8);
            }
            foreach (float hh in new float[] { 0.55f, 1.1f })
                CylX(pv, 0.02f, w, steelP, new Vector3(cx, yb + hh + 0.1f, 0.55f), 8);
        }

        void BuildHoriz(PlanItem it, Transform pv, float cx, float yb, float w, float h)
        {
            float rr = h * 0.5f, zc = -rr - 0.3f;
            var v = new Vessel { item = it, bind = it.Bind, r = rr, cx = cx, z = zc };
            // ležatý válec: plášť s řezem a plná kapalina
            var prof = new List<Vector2> { new Vector2(0f, -w * 0.5f), new Vector2(rr, -w * 0.5f), new Vector2(rr, w * 0.5f), new Vector2(0f, w * 0.5f) };
            Mesh full = MeshKit.Lathe(prof, 48, 0f, Mathf.PI * 2f, new Vector2(4f, 1f));
            Mesh cutm = MeshKit.Lathe(prof, 48, Mathf.PI - 0.6435f, Mathf.PI * 2f + 0.6435f, new Vector2(4f, 1f));
            Vector3 pos = new Vector3(cx, yb + rr, -zc);
            // osa válce podél x: z Unity osy y otočíme o 90° kolem z
            v.shellFull = AddRaw(pv, full, inox, pos, new Vector3(0, 0, 90f), "plášť");
            v.shellCut = AddRaw(pv, cutm, inox, pos, new Vector3(0, 0, 90f), "plášť řez");
            Material lm = HdrpUtil.Transparent(LiquidColor(it.Bind), 0f, 0.35f);
            HdrpUtil.SetEmissive(lm, new Color(LiquidColor(it.Bind).r, LiquidColor(it.Bind).g, LiquidColor(it.Bind).b), 260f);
            v.liqMat = lm;
            v.liqCyl = AddRaw(pv, MeshKit.Cylinder(rr * 0.98f, w * 0.98f, 40), lm, pos, new Vector3(0, 0, 90f), "kapalina");
            v.hy = yb; v.hrr = rr; v.hpos = pos;
            v.ApplyCut(cutaway);
            for (int s = -1; s <= 1; s += 2)
                Box(pv, new Vector3(0.2f, rr * 0.9f, rr * 1.6f), inox, new Vector3(cx + s * w * 0.35f, rr * 0.45f, zc), Vector3.zero);
            vessels.Add(v);
        }

        GameObject AddRaw(Transform parent, Mesh mesh, Material mat, Vector3 unityPos, Vector3 euler, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = unityPos; go.transform.localRotation = Quaternion.Euler(euler);
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            go.AddComponent<MeshRenderer>().sharedMaterial = mat;
            return go;
        }

        ParticleSystem MakeBubbles(Transform pv, float radius)
        {
            var go = new GameObject("bubliny");
            go.transform.SetParent(pv, false);
            var ps = go.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = ps.main;
            main.loop = true; main.playOnAwake = false;
            main.startSize = new ParticleSystem.MinMaxCurve(0.014f, 0.04f);
            main.startSpeed = 0.45f;
            main.startColor = new Color(1f, 0.96f, 0.85f, 0.9f);
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            main.maxParticles = 700;
            var sh = ps.shape;
            sh.enabled = true; sh.shapeType = ParticleSystemShapeType.Cone; sh.angle = 0f; sh.radius = radius; sh.radiusThickness = 1f;
            sh.rotation = new Vector3(-90f, 0f, 0f);
            var em = ps.emission; em.rateOverTime = 0f;
            var rend = go.GetComponent<ParticleSystemRenderer>();
            rend.sharedMaterial = bubbleMat;
            rend.renderMode = ParticleSystemRenderMode.Billboard;
            rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            return ps;
        }

        void BuildTank(PlanItem it, Transform pv, string shape, float cx, float yb, float r, float h)
        {
            bool cone = shape == "cone";
            float zc = -r - 0.35f;
            float hd = r * 0.30f;
            float ht = r * (shape == "lauter" ? 0.20f : 0.32f);
            float hb = cone ? h * 0.5f : hd + 0.24f;
            float hcyl = Mathf.Max(0.6f, h - hb - ht);
            float by = yb + hb;
            var v = new Vessel { item = it, bind = it.Bind, r = r, cx = cx, z = zc, by = by, hcyl = hcyl, cone = cone };

            // profil pláště zespodu nahoru
            var prof = new List<Vector2>();
            if (cone) { prof.Add(new Vector2(0.09f, yb)); prof.Add(new Vector2(r, by)); }
            else
                for (int i = 0; i <= 14; i++)
                {
                    float t = i / 14f * Mathf.PI * 0.5f;
                    prof.Add(new Vector2(r * Mathf.Sin(t), by - hd * Mathf.Cos(t)));
                }
            prof.Add(new Vector2(r, by + hcyl));
            for (int i = 1; i <= 14; i++)
            {
                float t2 = i / 14f * Mathf.PI * 0.5f;
                prof.Add(new Vector2(r * Mathf.Cos(t2), by + hcyl + ht * Mathf.Sin(t2)));
            }
            Vector3 pos = new Vector3(cx, 0f, -zc);
            v.shellFull = AddRaw(pv, MeshKit.Lathe(prof, 64, 0f, Mathf.PI * 2f, new Vector2(8f, 1.6f)), inox, pos, Vector3.zero, "plášť");
            // řez: v přední části chybí pás (jako v 3D z Godotu: z > 0.04 r a |x| < 0.8 r)
            v.shellCut = AddRaw(pv, MeshKit.Lathe(prof, 64, Mathf.PI - 0.6435f, Mathf.PI * 2f + 0.6435f, new Vector2(8f, 1.6f)), inox, pos, Vector3.zero, "plášť řez");
            {
                int iBot = cone ? 1 : 14, sc = prof.Count;
                var bot = new List<Vector2>(); var wal = new List<Vector2>();
                for (int i = 0; i < sc; i++)
                {
                    Vector2 q = new Vector2(prof[i].x * 0.985f, prof[i].y);
                    if (i <= iBot) bot.Add(q);
                    if (i >= iBot) wal.Add(q);
                }
                if (it.Bind != "lv" && it.Bind != "hlt") RealInterior(v, bot, wal, pos);
            }
            v.ApplyCut(cutaway);

            // kapalina
            Material lm = HdrpUtil.Transparent(LiquidColor(it.Bind), 0f, 0.35f);
            HdrpUtil.SetEmissive(lm, new Color(LiquidColor(it.Bind).r, LiquidColor(it.Bind).g, LiquidColor(it.Bind).b), 260f);
            v.liqMat = lm;
            if (cone)
                v.liqCone = AddRaw(pv, MeshKit.Cylinder(r * 0.985f, hb, 48, 0.09f), lm, new Vector3(cx, yb + hb * 0.5f, -zc), Vector3.zero, "kapalina kužel");
            else
            {
                var dp = new List<Vector2>();
                for (int i = 0; i <= 14; i++)
                {
                    float t = i / 14f * Mathf.PI * 0.5f;
                    dp.Add(new Vector2(r * 0.985f * Mathf.Sin(t), by - hd * 0.985f * Mathf.Cos(t)));
                }
                dp.Add(new Vector2(0f, by));
                v.liqDish = AddRaw(pv, MeshKit.Lathe(dp, 48, 0f, Mathf.PI * 2f, new Vector2(4f, 1f)), lm, pos, Vector3.zero, "kapalina dno");
            }
            v.liqCyl = AddRaw(pv, MeshKit.Cylinder(r * 0.985f, hcyl, 56), lm, new Vector3(cx, by + hcyl * 0.5f, -zc), Vector3.zero, "kapalina válec");
            // pěna na hladině a bubliny od dna
            v.foam = AddRaw(pv, MeshKit.Cylinder(r * 0.975f, 0.03f, 48), foamMat, pos, Vector3.zero, "pěna");
            v.bubbles = MakeBubbles(pv, r * 0.9f);
            float f0; if (!DemoFill.TryGetValue(it.Bind, out f0)) f0 = 0.5f;
            v.SetFill(f0);
            v.SetBoil(it.Bind == "rvp" || it.Bind == "wp" ? 0.55f : 0f);

            // svary a obruče
            foreach (float yy in new float[] { by, by + hcyl }) Cyl(pv, r * 1.004f, 0.014f, steelB, new Vector3(cx, yy, zc), Vector3.zero, 64);
            foreach (float f in new float[] { 0.33f, 0.66f }) Cyl(pv, r * 1.006f, 0.02f, steelB, new Vector3(cx, by + hcyl * f, zc), Vector3.zero, 64);
            Cyl(pv, r * 1.02f, 0.05f, steelB, new Vector3(cx, by + hcyl + 0.01f, zc), Vector3.zero, 64);
            Add(pv, MeshKit.Torus(r * 1.012f, 0.022f, 64, 8), steelP, new Vector3(cx, by + hcyl + 0.012f, zc), Vector3.zero, "lem víka");

            // nohy: tlusté nerezové trubky s přírubou a patní deskou (podle fotek)
            float[] la = cone ? new float[] { 0.8f, 2.4f, 3.9f, 5.5f } : new float[] { 0.55f, 2.1f, 3.7f, 5.3f };
            foreach (float a in la)
            {
                float k = cone ? 0.9f : 0.86f;
                float lx = cx + Mathf.Cos(a) * r * k, lz = zc + Mathf.Sin(a) * r * k;
                RealLeg(pv, lx, lz, yb, cone ? hb * 0.97f : hb * 0.95f, cone ? 0.085f : 0.10f, true);
            }

            // průlez (v řezu na zadní straně)
            float mdir = -1f;
            float mz = zc + r * mdir; float mh = by + hcyl * 0.58f;
            CylZ(pv, 0.27f, 0.07f, steelP, new Vector3(cx, mh, mz + 0.03f * mdir), 32);
            CylZ(pv, 0.23f, 0.04f, steelB, new Vector3(cx, mh, mz + 0.075f * mdir), 32);
            for (int k = 0; k < 12; k++)
            {
                float aa = Mathf.PI * 2f * k / 12f;
                CylZ(pv, 0.014f, 0.025f, steelP, new Vector3(cx + Mathf.Cos(aa) * 0.25f, mh + Mathf.Sin(aa) * 0.25f, mz + 0.09f * mdir), 8);
            }

            // stavoznak na boku
            float gx = cx + r + 0.1f;
            Cyl(pv, 0.04f, hcyl * 0.9f, glassMat, new Vector3(gx, by + hcyl * 0.5f, zc), Vector3.zero, 14);
            foreach (float yy2 in new float[] { by + hcyl * 0.05f, by + hcyl * 0.95f })
            {
                Cyl(pv, 0.06f, 0.05f, steelP, new Vector3(gx, yy2, zc), Vector3.zero, 14);
                Box(pv, new Vector3(0.12f, 0.03f, 0.03f), steelP, new Vector3(gx - 0.06f, yy2, zc), Vector3.zero);
            }

            // spodní výpust s tri-clamp spojem
            float oy = cone ? yb : by - hd;
            Cyl(pv, 0.062f, 0.16f, steelP, new Vector3(cx, oy - 0.04f, zc), Vector3.zero, 18);
            Cyl(pv, 0.10f, 0.022f, steelB, new Vector3(cx, oy - 0.10f, zc), Vector3.zero, 24);
            Cyl(pv, 0.10f, 0.022f, steelB, new Vector3(cx, oy - 0.135f, zc), Vector3.zero, 24);
            Add(pv, MeshKit.Torus(0.103f, 0.012f, 32, 8), steelP, new Vector3(cx, oy - 0.1175f, zc), Vector3.zero, "clamp");
            Cyl(pv, 0.062f, 0.12f, steelP, new Vector3(cx, oy - 0.20f, zc), Vector3.zero, 18);

            // CIP kulová hlavice, ventil a průhledítko ve víku
            if (it.Bind != "spilka" && it.Bind != "lv")
            {
                float rho = r * 0.42f; float px = cx + Mathf.Cos(0.9f) * rho, pz = zc + Mathf.Sin(0.9f) * rho; float ly = LidY(by, hcyl, r, ht, rho);
                Cyl(pv, 0.06f, 0.16f, steelP, new Vector3(px, ly + 0.07f, pz), Vector3.zero, 16);
                RealSprayArm(pv, px, ly, pz, -Mathf.Sign(Mathf.Cos(0.9f)));
            }
            {
                float rho2 = r * 0.62f; float vx = cx + Mathf.Cos(2.3f) * rho2, vz = zc + Mathf.Sin(2.3f) * rho2; float vy = LidY(by, hcyl, r, ht, rho2);
                Cyl(pv, 0.045f, 0.22f, steelP, new Vector3(vx, vy + 0.1f, vz), Vector3.zero, 14);
                Cyl(pv, 0.058f, 0.07f, steelB, new Vector3(vx, vy + 0.25f, vz), Vector3.zero, 18);
                Add(pv, MeshKit.Sphere(0.05f), steelP, new Vector3(vx, vy + 0.30f, vz), Vector3.zero, "ventil");
                float rho3 = r * 0.52f; float sx = cx + Mathf.Cos(4.1f) * rho3, sz = zc + Mathf.Sin(4.1f) * rho3; float sy = LidY(by, hcyl, r, ht, rho3);
                Cyl(pv, 0.10f, 0.05f, steelP, new Vector3(sx, sy + 0.02f, sz), Vector3.zero, 24);
                Cyl(pv, 0.075f, 0.012f, glassMat, new Vector3(sx, sy + 0.05f, sz), Vector3.zero, 24);
            }

            // míchadlo a motor (pánve)
            if (it.Bind == "rvp" || it.Bind == "mp")
            {
                Cyl(pv, 0.04f, hcyl * 0.95f, steelP, new Vector3(cx, by + hcyl * 0.5f + 0.12f, zc), Vector3.zero, 12);
                var rotor = new GameObject("rotor").transform; rotor.SetParent(pv, false);
                rotor.localPosition = new Vector3(cx, by + 0.3f, -zc);
                for (int a = 0; a < 2; a++)
                {
                    var blade = Box(rotor, new Vector3(r * 1.5f, 0.05f, 0.22f), mDarkSteel, Vector3.zero, new Vector3(0f, -(a * 90f + 17f), -10f));
                }
                AddSpinner(rotor, it.Bind);
                Cyl(pv, 0.2f, 0.55f, HdrpUtil.Lit(new Color(0.12f, 0.32f, 0.62f), 0.5f, 0.55f), new Vector3(cx, by + hcyl + ht + 0.3f, zc), Vector3.zero, 28);
                Cyl(pv, 0.28f, 0.12f, steelB, new Vector3(cx, by + hcyl + ht + 0.02f, zc), Vector3.zero, 28);
            }
            // odtah par u rvp + pára
            if (it.Bind == "rvp")
            {
                float sxp = cx + r * 0.55f, syp = by + hcyl + ht * 0.8f;
                Cyl(pv, 0.17f, 1.6f, steelB, new Vector3(sxp, syp + 0.8f, zc), Vector3.zero, 20);
                Cyl(pv, 0.24f, 0.12f, steelP, new Vector3(sxp, syp + 1.65f, zc), Vector3.zero, 24);
                var st = new GameObject("pára"); st.transform.SetParent(pv, false);
                st.transform.localPosition = new Vector3(sxp, syp + 1.75f, -zc);
                var sp = st.AddComponent<SteamPuffs>(); sp.strength = 1f; sp.height = 2.6f; v.steam = sp;
            }
            // vnitřek scezováku podle fotek
            if (shape == "lauter") RealLauterInside(pv, cx, by, zc, r, hcyl, it.Bind);
            vessels.Add(v);
        }

        static float LidY(float by, float hcyl, float r, float ht, float rho)
        {
            float k = Mathf.Clamp01(rho / r);
            return by + hcyl + ht * Mathf.Sqrt(1f - k * k);
        }
    }
}
