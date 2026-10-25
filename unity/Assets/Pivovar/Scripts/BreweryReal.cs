// Realističtější díly podle fotek skutečné varny: tri-clamp spoje, motýlkové a kulové klapky s barevnými pákami,
// čerpadla s žebrovaným motorem, příruby se šrouby, nohy nádob, černý vnitřek nádob s hrably a sprchovým ramenem.
// Trasy potrubí a jejich body zůstávají z plan.json (Godot) beze změny.
using System.Collections.Generic;
using UnityEngine;
using Pivovar.Core;

namespace Pivovar
{
    public partial class BreweryBuilder
    {
        Material mBlue, mRed, mAmber, mLining, mSieve, mDarkSteel, mBolt;

        void EnsureRealMats()
        {
            if (mBlue != null) return;
            mBlue = HdrpUtil.Lit(new Color(0.08f, 0.30f, 0.92f), 0f, 0.55f);
            mRed = HdrpUtil.Lit(new Color(0.85f, 0.09f, 0.07f), 0f, 0.55f);
            mAmber = HdrpUtil.Lit(new Color(0.96f, 0.60f, 0.10f), 0f, 0.55f);
            mLining = HdrpUtil.Lit(new Color(0.022f, 0.024f, 0.030f), 0.15f, 0.30f);
            mSieve = HdrpUtil.Lit(new Color(0.34f, 0.35f, 0.38f), 0.9f, 0.40f);
            mDarkSteel = HdrpUtil.Lit(new Color(0.10f, 0.105f, 0.115f), 0.8f, 0.35f);
            mBolt = HdrpUtil.Lit(new Color(0.55f, 0.56f, 0.60f), 1f, 0.5f);
        }

        /// <summary>Obrátí plochy meshe dovnitř (pro vnitřní obklad nádob).</summary>
        static Mesh FlipMesh(Mesh m)
        {
            var tri = m.triangles;
            for (int i = 0; i < tri.Length; i += 3) { int t = tri[i + 1]; tri[i + 1] = tri[i + 2]; tri[i + 2] = t; }
            var nr = m.normals;
            for (int i = 0; i < nr.Length; i++) nr[i] = -nr[i];
            m.triangles = tri; m.normals = nr;
            m.RecalculateBounds();
            return m;
        }

        /// <summary>Tri-clamp spoj na potrubí: dvě hrdla, těsnění, svěrný kroužek s křídlovou matkou (poloha v lokálních souřadnicích Unity).</summary>
        void RealClamp(Transform parent, Vector3 p, Vector3 dir, float rad)
        {
            EnsureRealMats();
            dir.Normalize();
            Quaternion q = Quaternion.FromToRotation(Vector3.up, dir);
            Vector3 side = Mathf.Abs(dir.y) > 0.9f ? Vector3.right : Vector3.up;
            foreach (float off in new float[] { -0.0085f, 0.0085f })
                MakeOriented(parent, MeshKit.Cylinder(rad * 1.6f, 0.011f, 24), steelP, p + dir * off, q);
            MakeOriented(parent, MeshKit.Cylinder(rad * 1.5f, 0.007f, 24), rubber, p, q);
            MakeOriented(parent, MeshKit.Cylinder(rad * 1.9f, 0.030f, 24), steelB, p, q);
            Quaternion qs = Quaternion.LookRotation(dir, side);
            MakeOriented(parent, MeshKit.Box(new Vector3(0.022f, 0.034f, 0.030f)), steelP, p + side * (rad * 1.95f), qs);
            MakeOriented(parent, MeshKit.Box(new Vector3(0.060f, 0.007f, 0.012f)), steelP, p + side * (rad * 1.95f + 0.022f), qs);
            MakeOriented(parent, MeshKit.Box(new Vector3(0.020f, 0.018f, 0.030f)), steelP, p - side * (rad * 1.9f), qs);
        }

        /// <summary>Tri-clamp na ose x uzlu (klapky): hrdla, těsnění a svěrný kroužek s křídlovou matkou nahoře.</summary>
        void ClampX(Transform n, float x, float rp)
        {
            CylX(n, rp * 1.85f, 0.011f, steelP, new Vector3(x - 0.0085f, 0, 0), 24);
            CylX(n, rp * 1.85f, 0.011f, steelP, new Vector3(x + 0.0085f, 0, 0), 24);
            CylX(n, rp * 1.62f, 0.006f, rubber, new Vector3(x, 0, 0), 24);
            CylX(n, rp * 2.15f, 0.028f, steelB, new Vector3(x, 0, 0), 24);
            Box(n, new Vector3(0.022f, 0.030f, 0.030f), steelP, new Vector3(x, rp * 2.2f, 0), Vector3.zero);
            Box(n, new Vector3(0.012f, 0.006f, 0.052f), steelP, new Vector3(x, rp * 2.2f + 0.020f, 0), Vector3.zero);
        }

        /// <summary>Tyto 3 klapky jdou plynule přiškrcovat (stejně jako v Godot aplikaci): škrtidlo SK, ventil u čerpadla C4, Belimo.</summary>
        public static readonly string[] ThrottleIds = { "valve_muraqe36", "valve_murbq6e1", "valve_murciy3u" };
        public static bool CanThrottle(PlanItem it) { return it != null && System.Array.IndexOf(ThrottleIds, it.Id) >= 0; }

        /// <summary>Otevření klapky v % (0 zavřeno, 100 otevřeno) – stejná pravidla jako v Godotu (pct u škrtidla, part u mezipolohy).</summary>
        public static float ValvePct(PlanItem it)
        {
            object o;
            bool open = !(it.D.TryGetValue("open", out o) && o is bool && !(bool)o);
            if (it.Str("kind", "") == "skrt") return Mathf.Clamp((float)it.Num("pct", 50), 0f, 100f);
            if (open && it.D.ContainsKey("part")) return Mathf.Clamp((float)it.Num("part", 100), 1f, 99f);
            return open ? 100f : 0f;
        }

        /// <summary>Nastaví otevření klapky v % a překreslí ji.</summary>
        public void SetValvePct(PlanItem it, float pct)
        {
            pct = Mathf.Clamp(Mathf.Round(pct), 0f, 100f);
            if (it.Str("kind", "") == "skrt") { it.D["pct"] = (double)pct; it.D["open"] = pct > 0f; }
            else
            {
                it.D["open"] = pct > 0f;
                if (pct >= 100f || pct <= 0f) it.D.Remove("part"); else it.D["part"] = (double)pct;
            }
            RemovePart(it); BuildValve(it);
        }

        public PlanItem ValveNear(Vector2 planPx, float radiusPx)
        {
            PlanItem best = null; float bd = radiusPx;
            foreach (PlanItem v in plan.OfType("valve"))
            {
                float d = Vector2.Distance(planPx, new Vector2((float)v.X, (float)v.Y));
                if (d < bd) { bd = d; best = v; }
            }
            return best;
        }

        public IEnumerable<PlanItem> Valves() { return plan.OfType("valve"); }

        void RealValve(PlanItem it)
        {
            EnsureRealMats();
            var n = new GameObject("klapka " + it.Id).transform; n.SetParent(root, false);
            partObjs[it.Id] = n.gameObject;
            n.localPosition = Pp(it.X, it.Y, ZP);
            n.localRotation = Quaternion.Euler(0f, 0f, -(float)it.Num("rot", 0));
            float rp = 0.028f;
            string kind = it.Str("kind", "klapka");
            float pct = ValvePct(it);
            bool open = pct > 0.5f;
            Material knob = !open ? mRed : (pct < 99.5f ? mAmber : mBlue);

            if (kind == "kulovy")
            {
                CylX(n, rp * 1.45f, 0.11f, steelB, Vector3.zero, 24);
                Add(n, MeshKit.Sphere(0.047f, 24, 14), steelP, Vector3.zero, Vector3.zero, "koule");
            }
            else
            {
                CylX(n, rp * 1.55f, 0.075f, steelB, Vector3.zero, 24);
                Add(n, MeshKit.Torus(rp * 1.55f, 0.007f, 24, 8), steelP, Vector3.zero, new Vector3(0, 0, 90f), "kroužek");
            }
            ClampX(n, -0.068f, rp);
            ClampX(n, 0.068f, rp);
            // krk a hlava
            Cyl(n, 0.016f, 0.075f, steelP, new Vector3(0, 0.06f, 0), Vector3.zero, 16);
            Cyl(n, 0.026f, 0.014f, steelB, new Vector3(0, 0.038f, 0), Vector3.zero, 18);
            Cyl(n, 0.024f, 0.012f, steelP, new Vector3(0, 0.095f, 0), Vector3.zero, 18);
            // páka svisle bez ohledu na natočení potrubí
            var stem = new GameObject("páka").transform; stem.SetParent(n, false);
            stem.localPosition = new Vector3(0, 0.108f, 0);
            stem.rotation = Quaternion.identity;
            if (kind == "kulovy")
            {
                Box(stem, new Vector3(0.20f, 0.008f, 0.022f), steelP, new Vector3(0.09f, 0f, 0f), Vector3.zero);
                Box(stem, new Vector3(0.11f, 0.014f, 0.028f), knob, new Vector3(0.15f, 0.003f, 0f), Vector3.zero);
            }
            else
            {
                Box(stem, new Vector3(0.15f, 0.008f, 0.022f), steelP, new Vector3(0.065f, 0f, 0f), Vector3.zero);
                CylZ(stem, 0.011f, 0.022f, steelP, new Vector3(0.14f, 0f, 0f), 14);
                Cyl(stem, 0.0035f, 0.028f, steelP, new Vector3(0.14f, 0.016f, 0f), Vector3.zero, 8);
                Add(stem, MeshKit.Sphere(0.017f, 16, 10), knob, new Vector3(0.14f, 0.042f, 0f), Vector3.zero, "kulička");
                Add(stem, MeshKit.Sphere(0.008f, 12, 8), steelP, new Vector3(-0.025f, 0.004f, 0f), Vector3.zero, "pata");
            }
            stem.localRotation = Quaternion.Euler(0f, 90f * (1f - pct / 100f), 0f);   // páka podél trubky = otevřeno, napříč = zavřeno, mezi tím přiškrceno
            if (kind == "belimo")
            {
                Box(n, new Vector3(0.13f, 0.1f, 0.09f), orange, new Vector3(0, 0.22f, 0), Vector3.zero);
                Box(n, new Vector3(0.05f, 0.02f, 0.02f), dark, new Vector3(0, 0.27f, 0.05f), Vector3.zero);
            }
        }

        void RealPump(PlanItem it)
        {
            EnsureRealMats();
            var n = new GameObject("čerpadlo " + it.Id).transform; n.SetParent(root, false);
            partObjs[it.Id] = n.gameObject;
            n.localPosition = Pp(it.X, it.Y, ZP);
            float dir = (float)it.Num("dir", 0);
            // spirála čerpadla s víkem a svěrným kroužkem
            CylZ(n, 0.135f, 0.09f, inox, Vector3.zero, 40);
            CylZ(n, 0.112f, 0.03f, steelP, new Vector3(0, 0, 0.06f), 36);
            Add(n, MeshKit.Torus(0.122f, 0.012f, 36, 8), steelB, new Vector3(0, 0, 0.048f), new Vector3(90f, 0, 0), "svěrka");
            for (int k = 0; k < 3; k++)
            {
                float a = k * Mathf.PI * 2f / 3f + 0.5f;
                Box(n, new Vector3(0.024f, 0.024f, 0.022f), steelP, new Vector3(Mathf.Cos(a) * 0.125f, Mathf.Sin(a) * 0.125f, 0.05f), Vector3.zero);
            }
            // výtlak nahoru s tri-clampem
            Cyl(n, 0.034f, 0.17f, inox, new Vector3(0, 0.15f, 0), Vector3.zero, 20);
            Cyl(n, 0.056f, 0.012f, steelP, new Vector3(0, 0.236f, 0), Vector3.zero, 24);
            Cyl(n, 0.064f, 0.026f, steelB, new Vector3(0, 0.248f, 0), Vector3.zero, 24);
            Cyl(n, 0.056f, 0.012f, steelP, new Vector3(0, 0.262f, 0), Vector3.zero, 24);
            // motor v osovém směru (dir z plánu); sání na opačné straně, žebrovaný plášť, kryt ventilátoru
            var mg = new GameObject("motor").transform; mg.SetParent(n, false);
            mg.localRotation = Quaternion.Euler(0f, 0f, -dir);
            CylX(mg, 0.034f, 0.12f, inox, new Vector3(-0.12f, 0, 0), 20);
            CylX(mg, 0.056f, 0.012f, steelP, new Vector3(-0.176f, 0, 0), 24);
            CylX(mg, 0.064f, 0.026f, steelB, new Vector3(-0.19f, 0, 0), 24);
            CylX(mg, 0.056f, 0.012f, steelP, new Vector3(-0.204f, 0, 0), 24);
            CylX(mg, 0.10f, 0.07f, inox, new Vector3(0.115f, 0, 0), 32);
            CylX(mg, 0.112f, 0.30f, inox, new Vector3(0.30f, 0, 0), 40);
            for (int k = 0; k < 13; k++) CylX(mg, 0.116f, 0.008f, steelP, new Vector3(0.17f + k * 0.021f, 0, 0), 40);
            CylX(mg, 0.115f, 0.10f, steelB, new Vector3(0.50f, 0, 0), 40);
            for (int k = -4; k <= 4; k++) Box(mg, new Vector3(0.012f, 0.14f, 0.007f), dark, new Vector3(0.553f, 0f, k * 0.019f), Vector3.zero);
            Box(mg, new Vector3(0.10f, 0.05f, 0.12f), steelB, new Vector3(0.30f, 0.12f, 0f), Vector3.zero);   // svorkovnice
            // podstavec pod spirálou
            Box(n, new Vector3(0.16f, 0.04f, 0.2f), steelB, new Vector3(0, -0.15f, 0), Vector3.zero);
            // kontrolka stavu na svorkovnici (zelená jede, modrá zavodňuje, oranžová běží bez průtoku) – řídí ji simulace
            var led = Box(mg, new Vector3(0.028f, 0.02f, 0.028f), HdrpUtil.Lit(new Color(0.1f, 0.1f, 0.1f), 0f, 0.3f), new Vector3(0.30f, 0.15f, 0.04f), Vector3.zero);
            var lr = led.GetComponent<Renderer>(); if (lr != null) pumpLeds[it.Id] = lr;
        }

        /// <summary>Příruba na uzlu (osa x) se šrouby po obvodu.</summary>
        void RealFlange(Transform n, float dx, float r)
        {
            EnsureRealMats();
            CylX(n, r, 0.016f, steelP, new Vector3(dx, 0, 0), 28);
            CylX(n, r * 0.92f, 0.006f, rubber, new Vector3(dx + (dx < 0 ? 0.011f : -0.011f), 0, 0), 28);
            int nb = 8;
            for (int k = 0; k < nb; k++)
            {
                float a = Mathf.PI * 2f * k / nb;
                CylX(n, 0.0042f, 0.024f, mBolt, new Vector3(dx, Mathf.Sin(a) * r * 0.78f, Mathf.Cos(a) * r * 0.78f), 6);
            }
        }

        /// <summary>Noha nádoby: tlustá nerezová trubka s přírubou ve dvou třetinách a patní deskou se šrouby.</summary>
        void RealLeg(Transform pv, float lx, float lz, float yb, float height, float rad, bool flanged)
        {
            EnsureRealMats();
            Cyl(pv, rad, height, inox, new Vector3(lx, yb + height * 0.5f, lz), Vector3.zero, 24);
            Cyl(pv, rad * 1.9f, 0.016f, steelB, new Vector3(lx, yb + 0.008f, lz), Vector3.zero, 28);
            for (int k = 0; k < 6; k++)
            {
                float a = Mathf.PI * 2f * k / 6f;
                Cyl(pv, 0.008f, 0.03f, mBolt, new Vector3(lx + Mathf.Cos(a) * rad * 1.55f, yb + 0.03f, lz + Mathf.Sin(a) * rad * 1.55f), Vector3.zero, 6);
            }
            if (!flanged) return;
            float fy = yb + height * 0.42f;
            Cyl(pv, rad * 1.5f, 0.016f, steelP, new Vector3(lx, fy - 0.011f, lz), Vector3.zero, 28);
            Cyl(pv, rad * 1.5f, 0.016f, steelP, new Vector3(lx, fy + 0.011f, lz), Vector3.zero, 28);
            Cyl(pv, rad * 1.42f, 0.007f, rubber, new Vector3(lx, fy, lz), Vector3.zero, 28);
            for (int k = 0; k < 10; k++)
            {
                float a = Mathf.PI * 2f * k / 10f;
                Cyl(pv, 0.009f, 0.05f, mBolt, new Vector3(lx + Mathf.Cos(a) * rad * 1.3f, fy, lz + Mathf.Sin(a) * rad * 1.3f), Vector3.zero, 6);
            }
        }

        /// <summary>Tmavý vnitřní obklad pláště (stěny a víko) a světlé dno – vidět v řezu nádoby podle fotek.</summary>
        void RealInterior(Vessel v, List<Vector2> bottom, List<Vector2> wall, Vector3 pos)
        {
            EnsureRealMats();
            foreach (var host in new GameObject[] { v.shellFull, v.shellCut })
            {
                bool cutHost = host == v.shellCut;
                float a0 = cutHost ? Mathf.PI - 0.6435f : 0f, a1 = cutHost ? Mathf.PI * 2f + 0.6435f : Mathf.PI * 2f;
                var wl = new GameObject("vnitřní stěna"); wl.transform.SetParent(host.transform, false);
                wl.AddComponent<MeshFilter>().sharedMesh = FlipMesh(MeshKit.Lathe(wall, 64, a0, a1, new Vector2(8f, 1.6f)));
                wl.AddComponent<MeshRenderer>().sharedMaterial = mLining;
                var bt = new GameObject("vnitřní dno"); bt.transform.SetParent(host.transform, false);
                bt.AddComponent<MeshFilter>().sharedMesh = FlipMesh(MeshKit.Lathe(bottom, 64, a0, a1, new Vector2(8f, 1.6f)));
                bt.AddComponent<MeshRenderer>().sharedMaterial = mSieve;
            }
        }

        /// <summary>Ohnuté sprchové rameno s kulovou hlavicí zavěšené z víka (jako na fotkách).</summary>
        void RealSprayArm(Transform pv, float px, float ly, float pzGodot, float towardX)
        {
            EnsureRealMats();
            float z = -pzGodot;
            var pts = new List<Vector3>
            {
                new Vector3(px, ly + 0.03f, z), new Vector3(px, ly - 0.34f, z),
                new Vector3(px + towardX * 0.40f, ly - 0.34f, z), new Vector3(px + towardX * 0.40f, ly - 0.52f, z)
            };
            AddRaw(pv, MeshKit.Tube(MeshKit.RoundPath(pts, 0.14f), 0.017f, 12), steelP, Vector3.zero, Vector3.zero, "sprchové rameno");
            Add(pv, MeshKit.Cylinder(0.03f, 0.03f, 16), steelB, new Vector3(px, ly - 0.02f, pzGodot), Vector3.zero, "příruba ramene");
            Add(pv, MeshKit.Sphere(0.036f, 16, 10), steelP, new Vector3(px + towardX * 0.40f, ly - 0.55f, pzGodot), Vector3.zero, "sprchová koule");
        }

        /// <summary>Vnitřek scezováku podle fotek: síto s hubem, dvě ramena hrabel s děrovanými lopatkami.</summary>
        void RealLauterInside(Transform pv, float cx, float by, float zc, float r, float hcyl, string bind = "sk")
        {
            EnsureRealMats();
            Cyl(pv, r * 0.97f, 0.03f, mSieve, new Vector3(cx, by + 0.18f, zc), Vector3.zero, 56);
            for (int k = 1; k < 7; k++) Cyl(pv, r * 0.97f * k / 7f, 0.004f, steelP, new Vector3(cx, by + 0.2f, zc), Vector3.zero, 56);
            for (int k = 0; k < 12; k++)
            {
                float a = Mathf.PI * 2f * k / 12f;
                Box(pv, new Vector3(r * 0.9f, 0.006f, 0.02f), mDarkSteel, new Vector3(cx + Mathf.Cos(a) * r * 0.5f, by + 0.202f, zc + Mathf.Sin(a) * r * 0.5f), new Vector3(0, -a * Mathf.Rad2Deg, 0));
            }
            // střed s přírubou
            Cyl(pv, 0.30f, 0.03f, mDarkSteel, new Vector3(cx, by + 0.22f, zc), Vector3.zero, 36);
            for (int k = 0; k < 8; k++)
            {
                float a = Mathf.PI * 2f * k / 8f;
                Cyl(pv, 0.014f, 0.02f, mBolt, new Vector3(cx + Mathf.Cos(a) * 0.24f, by + 0.245f, zc + Mathf.Sin(a) * 0.24f), Vector3.zero, 8);
            }
            var arms = new GameObject("hrabla").transform; arms.SetParent(pv, false);
            arms.localPosition = new Vector3(cx, by + 0.45f, -zc);
            Cyl(pv, 0.05f, hcyl * 0.8f, mDarkSteel, new Vector3(cx, by + hcyl * 0.4f + 0.3f, zc), Vector3.zero, 12);
            for (int a = 0; a < 2; a++)
            {
                var arm = new GameObject("rameno").transform; arm.SetParent(arms, false); arm.localRotation = Quaternion.Euler(0, a * 180f, 0);
                Box(arm, new Vector3(r * 0.95f, 0.05f, 0.075f), mDarkSteel, new Vector3(-r * 0.47f, 0f, 0f), Vector3.zero);
                for (int k = 0; k < 3; k++)
                {
                    float px = -r * (0.30f + 0.26f * k);
                    Box(arm, new Vector3(0.24f, 0.22f, 0.012f), mDarkSteel, new Vector3(px, -0.14f, 0f), new Vector3(0, 0, 0));
                    for (int h = 0; h < 3; h++)
                        CylZ(arm, 0.022f, 0.016f, mSieve, new Vector3(px - 0.07f + h * 0.07f, -0.10f, 0f), 12);
                }
            }
            AddSpinner(arms, bind);
        }
    }
}
