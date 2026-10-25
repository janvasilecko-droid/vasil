// Kamera: pravé tlačítko otáčí, prostřední nebo Shift+pravé posouvá, kolečko přibližuje, WASD/QE jezdí.
using UnityEngine;

namespace Pivovar
{
    public class OrbitCamera : MonoBehaviour
    {
        public Vector3 target = new Vector3(2.5f, 2.2f, 0f);
        public float yaw = -0.25f;
        public float pitch = 0.12f;
        public float dist = 9f;

        // Plochý pohled: ortografická kamera zepředu (vypadá jako 2D schéma, ale světlo, stíny, materiály a kapalina jsou 3D). Klávesa V přepíná.
        public const string ViewKey = "pivovar.view";
        static int flat = -1;
        public static bool Flat
        {
            get { if (flat < 0) flat = PlayerPrefs.GetString(ViewKey, "flat") != "3d" ? 1 : 0; return flat == 1; }
            set { flat = value ? 1 : 0; }
        }
        bool lastFlat; bool init;
        Camera cam;

        void ApplyProjection()
        {
            if (cam == null) cam = GetComponent<Camera>();
            if (cam == null) return;
            cam.orthographic = Flat;
            if (Flat) { yaw = 0f; pitch = 0f; dist = 4.0f / 0.52f; float asp = Mathf.Max(0.5f, (float)Screen.width / Mathf.Max(1, Screen.height)); target = new Vector3(-6.8f + 4.0f * asp, 4f, 0f); }   // záběr přes celou výšku haly, začátek vlevo; dál se jede myší
        }

        void LateUpdate()
        {
            if (!init || lastFlat != Flat) { init = true; lastFlat = Flat; ApplyProjection(); }
            if (!PlanEditor.Typing && Inp.Pressed("V"))
            {
                Flat = !Flat; PlayerPrefs.SetString(ViewKey, Flat ? "flat" : "3d"); PlayerPrefs.Save();
                lastFlat = Flat; ApplyProjection();
            }
            Vector2 d = Inp.MouseDelta();
            bool shift = Inp.Held("Shift");
            if (Flat)
            {
                float aspect = Mathf.Max(0.5f, (float)Screen.width / Mathf.Max(1, Screen.height));
                float ortho = Mathf.Clamp(dist * 0.52f, 0.8f, 16f);
                // tažení myší po ploše: levé tlačítko (když není zapnutý editor), pravé nebo prostřední vždy
                bool drag = Inp.Right() || Inp.Middle() || (Inp.Left() && !PlanEditor.Active);
                if (drag)
                {
                    float k = 2f * ortho / Mathf.Max(1, Screen.height);
                    target += Vector3.right * (-d.x * k) + Vector3.up * (-d.y * k);
                }
                // kolečko: přiblížení k ukazateli myši
                float sc2 = Inp.Scroll();
                if (Mathf.Abs(sc2) > 0.001f)
                {
                    Vector2 mp = Inp.MousePos();
                    float nx = (mp.x / Mathf.Max(1, Screen.width)) * 2f - 1f, ny = (mp.y / Mathf.Max(1, Screen.height)) * 2f - 1f;
                    Vector3 under = target + new Vector3(nx * ortho * aspect, ny * ortho, 0f);
                    dist = Mathf.Clamp(dist * Mathf.Pow(0.78f, sc2), 1.5f, 31f);
                    ortho = Mathf.Clamp(dist * 0.52f, 0.8f, 16f);
                    target = under - new Vector3(nx * ortho * aspect, ny * ortho, 0f);
                }
                float sp2 = PlanEditor.Typing ? 0f : ortho * 1.2f * Time.unscaledDeltaTime;
                if (Inp.Held("W") || Inp.Held("E")) target += Vector3.up * sp2;
                if (Inp.Held("S") || Inp.Held("Q")) target -= Vector3.up * sp2;
                if (Inp.Held("D")) target += Vector3.right * sp2;
                if (Inp.Held("A")) target -= Vector3.right * sp2;
                // držet záběr uvnitř haly (x -11.7..45.7, y 0..8), ať není vidět šedá mimo varnu
                float hx = ortho * aspect, hy = ortho;
                target.x = (2f * hx >= 57.4f) ? 17f : Mathf.Clamp(target.x, -11.7f + hx, 45.7f - hx);
                target.y = (2f * hy >= 8.0f) ? 4f : Mathf.Clamp(target.y, hy, 8f - hy);
                target.z = 0f;
                if (cam != null) cam.orthographicSize = ortho;
                transform.position = target + new Vector3(0f, 0f, -10f);
                transform.rotation = Quaternion.identity;
                return;
            }
            if (Inp.Right() && !shift) { yaw -= d.x * 0.0035f; pitch = Mathf.Clamp(pitch - d.y * 0.0035f, -0.2f, 1.45f); }
            else if (Inp.Middle() || (Inp.Right() && shift))
            {
                float k = dist * 0.0016f;
                target += transform.right * (-d.x * k) + transform.up * (-d.y * k);
            }
            float sc = Inp.Scroll();
            if (Mathf.Abs(sc) > 0.001f) dist = Mathf.Clamp(dist * Mathf.Pow(0.9f, sc), 1.2f, 90f);

            float sp = PlanEditor.Typing ? 0f : dist * 0.6f * Time.unscaledDeltaTime;
            Vector3 fwd = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
            if (Inp.Held("W")) target += fwd * sp;
            if (Inp.Held("S")) target -= fwd * sp;
            if (Inp.Held("D")) target += transform.right * sp;
            if (Inp.Held("A")) target -= transform.right * sp;
            if (Inp.Held("E")) target += Vector3.up * sp;
            if (Inp.Held("Q")) target -= Vector3.up * sp;

            // yaw 0 = pohled zepředu (kamera na straně -z, vidí osu x doprava)
            Vector3 dir = new Vector3(Mathf.Sin(yaw) * Mathf.Cos(pitch), Mathf.Sin(pitch), -Mathf.Cos(yaw) * Mathf.Cos(pitch));
            transform.position = target + dir * dist;
            transform.LookAt(target, Vector3.up);
        }
    }
}
