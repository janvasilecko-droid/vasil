// Měkká pára: otočené průhledné čtverce se vzorem radiálního gradientu, plynule rostou a mizí.
using UnityEngine;

namespace Pivovar
{
    public class SteamPuffs : MonoBehaviour
    {
        public float strength = 1f;      // 0 = žádná pára
        public float height = 2.6f;
        public float spread = 0.5f;
        const int N = 14;
        Transform[] q = new Transform[N];
        Material[] mats = new Material[N];
        float[] seed = new float[N];
        static Texture2D puff;
        Camera cam;

        static Texture2D PuffTex()
        {
            if (puff != null) return puff;
            const int n = 64;
            puff = new Texture2D(n, n, TextureFormat.RGBA32, true, false);
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    float dx = (x + 0.5f - n * 0.5f) / (n * 0.5f), dy = (y + 0.5f - n * 0.5f) / (n * 0.5f);
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    float a = Mathf.Clamp01(1f - d);
                    a = a * a * (3f - 2f * a);
                    puff.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                }
            puff.wrapMode = TextureWrapMode.Clamp;
            puff.Apply(true);
            return puff;
        }

        void Start()
        {
            cam = Camera.main;
            Texture2D tex = PuffTex();
            for (int i = 0; i < N; i++)
            {
                GameObject g = GameObject.CreatePrimitive(PrimitiveType.Quad);
                var col = g.GetComponent<Collider>(); if (col != null) Destroy(col);
                g.name = "puff" + i;
                g.transform.SetParent(transform, false);
                var mr = g.GetComponent<MeshRenderer>();
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mr.receiveShadows = false;
                Material m = HdrpUtil.Transparent(new Color(0.9f, 0.93f, 0.97f, 0f), 0f, 0.1f);
                if (m.HasProperty("_BaseColorMap")) m.SetTexture("_BaseColorMap", tex);
                else m.mainTexture = tex;
                HdrpUtil.SetEmissive(m, new Color(0.9f, 0.93f, 0.97f), 0.6f);
                mr.sharedMaterial = m;
                mats[i] = m; q[i] = g.transform;
                seed[i] = Hash(i * 1.7f);
            }
        }

        static float Hash(float x) { return Mathf.Repeat(Mathf.Sin(x * 12.9898f) * 43758.5453f, 1f); }

        void Update()
        {
            if (cam == null) cam = Camera.main;
            float t = Time.time;
            for (int i = 0; i < N; i++)
            {
                if (q[i] == null) continue;
                float ph = Mathf.Repeat(t * (0.14f + 0.05f * Hash(i * 3.1f)) + seed[i], 1f);
                float sz = (0.35f + 1.3f * ph) * (0.7f + 0.6f * Hash(i * 5.3f));
                Vector3 p = new Vector3((Hash(i * 7.7f) - 0.5f) * spread + 0.4f * ph * ph + 0.12f * Mathf.Sin(t * 0.6f + i) * ph, height * ph, 0f);
                q[i].localPosition = p;
                q[i].localScale = new Vector3(sz, sz, 1f);
                if (cam != null) q[i].rotation = cam.transform.rotation;
                float a = strength * 0.30f * Mathf.Sin(Mathf.Clamp01(ph) * Mathf.PI) * (1f - 0.4f * ph);
                Color c = new Color(0.9f, 0.93f, 0.97f, a);
                if (mats[i].HasProperty("_BaseColor")) mats[i].SetColor("_BaseColor", c); else mats[i].color = c;
            }
        }
    }
}
