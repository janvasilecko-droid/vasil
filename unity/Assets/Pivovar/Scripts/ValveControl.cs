// Ovládání klapek v plochém pohledu: klik otevře/zavře, škrtidlo SK = levé +10 % / pravé −10 %,
// u ventilu C4 a Belima Ctrl + kolečko mění otevření po 10 % (Shift po 2 %), nad klapkou se ukáže popisek s procenty.
using UnityEngine;
using Pivovar.Core;

namespace Pivovar
{
    public class ValveControl : MonoBehaviour
    {
        public static bool WheelBusy;
        BreweryBuilder b;
        bool lDown, rDown; Vector2 lPos, rPos;
        string toast = ""; float toastT;
        PlanItem hover;
        GUIStyle st;

        public void Init(BreweryBuilder builder) { b = builder; }

        PlanItem Pick(Vector2 screen)
        {
            Camera c = b.Cam; if (c == null) return null;
            Ray ray = c.ScreenPointToRay(new Vector3(screen.x, screen.y, 0f));
            // rovina potrubí: Unity z = -ZP
            if (Mathf.Abs(ray.direction.z) < 1e-5f) return null;
            float t = (-BreweryBuilder.ZP - ray.origin.z) / ray.direction.z;
            Vector3 w = ray.origin + ray.direction * t;
            Vector2 px = b.ToPlan(w);
            return b.ValveNear(px, 38f);
        }

        void Say(string t) { toast = t; toastT = Time.unscaledTime + 2.5f; }

        static string Name(PlanItem v) { return v.Str("label", "klapka"); }

        void Update()
        {
            WheelBusy = false;
            if (b == null || PlanEditor.Active || !OrbitCamera.Flat) { hover = null; return; }
            Vector2 mp = Inp.MousePos();
            hover = Pick(mp);
            bool ctrl = Inp.Held("Ctrl");
            if (hover != null && ctrl && BreweryBuilder.CanThrottle(hover)) WheelBusy = true;
            if (WheelBusy)
            {
                float sc = Inp.Scroll();
                if (Mathf.Abs(sc) > 0.001f)
                {
                    float step = Inp.Held("Shift") ? 2f : 10f;
                    float p = BreweryBuilder.ValvePct(hover) + Mathf.Sign(sc) * step;
                    b.SetValvePct(hover, p);
                    Say(Name(hover) + ": otevřeno " + Mathf.RoundToInt(BreweryBuilder.ValvePct(hover)) + " %");
                }
            }
            bool l = Inp.Left(), r = Inp.Right();
            if (l && !lDown) { lDown = true; lPos = mp; }
            if (!l && lDown) { lDown = false; if ((mp - lPos).magnitude < 6f) Click(Pick(mp), +1); }
            if (r && !rDown) { rDown = true; rPos = mp; }
            if (!r && rDown) { rDown = false; if ((mp - rPos).magnitude < 6f) Click(Pick(mp), -1); }
        }

        void Click(PlanItem v, int dir)
        {
            if (v == null) return;
            if (v.Str("kind", "") == "skrt")
            {
                float p = BreweryBuilder.ValvePct(v) + 10f * dir;
                b.SetValvePct(v, p);
                Say(Name(v) + ": otevřeno " + Mathf.RoundToInt(BreweryBuilder.ValvePct(v)) + " % (levé +10 %, pravé −10 %)");
                return;
            }
            if (dir < 0) return;
            bool open = BreweryBuilder.ValvePct(v) > 0.5f;
            b.SetValvePct(v, open ? 0f : 100f);
            Say(Name(v) + ": " + (open ? "zavřeno" : "otevřeno") + (BreweryBuilder.CanThrottle(v) ? " (Ctrl + kolečko = přiškrcení)" : ""));
        }

        void OnGUI()
        {
            if (b == null || !OrbitCamera.Flat || PlanEditor.Active) return;
            if (st == null) { st = new GUIStyle(GUI.skin.box); st.fontSize = 13; st.normal.textColor = Color.white; st.alignment = TextAnchor.MiddleCenter; }
            Camera c = b.Cam; if (c == null) return;
            foreach (PlanItem v in b.Valves())
            {
                bool thr = BreweryBuilder.CanThrottle(v);
                if (!thr && v != hover) continue;
                Vector3 sp = c.WorldToScreenPoint(b.FromPlan(v.X, v.Y, BreweryBuilder.ZP) + new Vector3(0f, 0.34f, 0f));
                if (sp.z < 0f) continue;
                string t = Mathf.RoundToInt(BreweryBuilder.ValvePct(v)) + " %";
                if (v == hover) t = Name(v) + " · " + t;
                var rc = new Rect(sp.x - 70f, Screen.height - sp.y - 12f, 140f, 24f);
                GUI.Box(rc, t, st);
            }
            if (Time.unscaledTime < toastT) GUI.Box(new Rect(Screen.width * 0.5f - 260f, 40f, 520f, 28f), toast, st);
        }
    }
}
