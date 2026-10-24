// Kamera: pravé tlačítko otáčí, prostřední nebo Shift+pravé posouvá, kolečko přibližuje, WASD/QE jezdí.
using UnityEngine;

namespace Pivovar
{
    public class OrbitCamera : MonoBehaviour
    {
        public Vector3 target = new Vector3(5f, 2.5f, 0f);
        public float yaw = -0.25f;
        public float pitch = 0.2f;
        public float dist = 18f;

        void LateUpdate()
        {
            Vector2 d = Inp.MouseDelta();
            bool shift = Inp.Held("Shift");
            if (Inp.Right() && !shift) { yaw -= d.x * 0.0035f; pitch = Mathf.Clamp(pitch - d.y * 0.0035f, -0.2f, 1.45f); }
            else if (Inp.Middle() || (Inp.Right() && shift))
            {
                float k = dist * 0.0016f;
                target += transform.right * (-d.x * k) + transform.up * (-d.y * k);
            }
            float sc = Inp.Scroll();
            if (Mathf.Abs(sc) > 0.001f) dist = Mathf.Clamp(dist * Mathf.Pow(0.9f, sc), 1.2f, 90f);

            float sp = dist * 0.6f * Time.unscaledDeltaTime;
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
