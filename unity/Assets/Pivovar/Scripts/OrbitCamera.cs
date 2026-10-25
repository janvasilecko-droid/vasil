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
            if (Flat) { yaw = 0f; pitch = 0f; }
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
                float ortho = dist * 0.52f;                       // stejný výřez jako perspektiva 55° ve vzdálenosti dist
                if (cam != null) { cam.orthographicSize = ortho; }
                if (Inp.Right() || Inp.Middle())
                {
                    float k = 2f * ortho / Mathf.Max(1, Screen.height);
                    target += transform.right * (-d.x * k) + transform.up * (-d.y * k);
                }
                float sc2 = Inp.Scroll();
                if (Mathf.Abs(sc2) > 0.001f) dist = Mathf.Clamp(dist * Mathf.Pow(0.9f, sc2), 1.2f, 90f);
                float sp2 = PlanEditor.Typing ? 0f : dist * 0.6f * Time.unscaledDeltaTime;
                if (Inp.Held("W") || Inp.Held("E")) target += Vector3.up * sp2;
                if (Inp.Held("S") || Inp.Held("Q")) target -= Vector3.up * sp2;
                if (Inp.Held("D")) target += Vector3.right * sp2;
                if (Inp.Held("A")) target -= Vector3.right * sp2;
                target.z = 0f;
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
