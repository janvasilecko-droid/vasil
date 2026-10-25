// Procedurální meshe: rotační tělesa (nádoby), trubky, kvádry, koule, torus.
// Souřadnice stavěné varny drží konvenci z Godotu (z směrem k divákovi); převod na Unity (z = -z) řeší Z().
using System.Collections.Generic;
using UnityEngine;

namespace Pivovar
{
    public static class MeshKit
    {
        /// <summary>Rotační těleso z profilu (x = poloměr, y = výška) v úhlovém rozsahu a0..a1 (radiány).</summary>
        public static Mesh Lathe(IList<Vector2> prof, int seg, float a0, float a1, Vector2 uvScale)
        {
            int n = prof.Count;
            var verts = new List<Vector3>(); var norms = new List<Vector3>(); var uvs = new List<Vector2>(); var idx = new List<int>();
            var lens = new float[n];
            for (int i = 1; i < n; i++) lens[i] = lens[i - 1] + Vector2.Distance(prof[i], prof[i - 1]);
            for (int i = 0; i < n; i++)
            {
                Vector2 p = prof[i];
                Vector2 t = (prof[Mathf.Min(i + 1, n - 1)] - prof[Mathf.Max(i - 1, 0)]);
                if (t.sqrMagnitude < 1e-12f) t = Vector2.up;
                t.Normalize();
                Vector2 nr = new Vector2(t.y, -t.x);
                for (int k = 0; k <= seg; k++)
                {
                    float a = a0 + (a1 - a0) * k / seg;
                    float c = Mathf.Cos(a), s = Mathf.Sin(a);
                    verts.Add(new Vector3(p.x * c, p.y, -p.x * s));
                    norms.Add(new Vector3(nr.x * c, nr.y, -nr.x * s).normalized);
                    uvs.Add(new Vector2((float)k / seg * uvScale.x, lens[i] * uvScale.y));
                }
            }
            for (int i = 0; i < n - 1; i++)
                for (int k = 0; k < seg; k++)
                {
                    int a = i * (seg + 1) + k, b = a + 1, c = a + seg + 1, d = c + 1;
                    idx.Add(a); idx.Add(c); idx.Add(b);
                    idx.Add(b); idx.Add(c); idx.Add(d);
                }
            return Finish(verts, norms, uvs, idx);
        }

        public static Mesh Cylinder(float r, float h, int seg = 24, float r2 = -1f)
        {
            float rb = r2 < 0f ? r : r2;
            var prof = new List<Vector2> { new Vector2(0f, -h * 0.5f), new Vector2(rb, -h * 0.5f), new Vector2(r, h * 0.5f), new Vector2(0f, h * 0.5f) };
            return Lathe(prof, seg, 0f, Mathf.PI * 2f, new Vector2(2f, 1f));
        }

        public static Mesh Sphere(float r, int seg = 20, int rings = 10)
        {
            var prof = new List<Vector2>();
            for (int i = 0; i <= rings; i++)
            {
                float t = Mathf.PI * i / rings;
                prof.Add(new Vector2(r * Mathf.Sin(t), -r * Mathf.Cos(t)));
            }
            return Lathe(prof, seg, 0f, Mathf.PI * 2f, new Vector2(2f, 1f));
        }

        public static Mesh Box(Vector3 size)
        {
            Vector3 h = size * 0.5f;
            var verts = new List<Vector3>(); var norms = new List<Vector3>(); var uvs = new List<Vector2>(); var idx = new List<int>();
            Vector3[] nrm = { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };
            for (int f = 0; f < 6; f++)
            {
                Vector3 n = nrm[f];
                Vector3 u = f < 2 ? Vector3.forward : (f < 4 ? Vector3.right : Vector3.right);
                if (f == 2 || f == 3) u = Vector3.right;
                Vector3 v = Vector3.Cross(n, u);
                int b = verts.Count;
                for (int q = 0; q < 4; q++)
                {
                    float su = (q == 0 || q == 3) ? -1f : 1f, sv = (q < 2) ? -1f : 1f;
                    Vector3 pos = Vector3.Scale(n, h) + Vector3.Scale(u, h) * su + Vector3.Scale(v, h) * sv;
                    verts.Add(pos); norms.Add(n); uvs.Add(new Vector2(su * 0.5f + 0.5f, sv * 0.5f + 0.5f));
                }
                idx.Add(b); idx.Add(b + 1); idx.Add(b + 2); idx.Add(b); idx.Add(b + 2); idx.Add(b + 3);
            }
            return Finish(verts, norms, uvs, idx);
        }

        /// <summary>Trubka podél lomené čáry (body v Unity souřadnicích). U souřadnice = délka v metrech.</summary>
        public static Mesh Tube(IList<Vector3> pts, float rad, int seg = 14)
        {
            int n = pts.Count;
            var verts = new List<Vector3>(); var norms = new List<Vector3>(); var uvs = new List<Vector2>(); var idx = new List<int>();
            if (n < 2) return Finish(verts, norms, uvs, idx);
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
                Vector3 s = Vector3.Cross(tg, up).normalized;
                Vector3 u = Vector3.Cross(s, tg).normalized;
                if (i > 0) acc += (pts[i] - pts[i - 1]).magnitude;
                for (int k = 0; k <= seg; k++)
                {
                    float ang = Mathf.PI * 2f * k / seg;
                    Vector3 nn = s * Mathf.Cos(ang) + u * Mathf.Sin(ang);
                    verts.Add(pts[i] + nn * rad); norms.Add(nn); uvs.Add(new Vector2(acc, (float)k / seg));
                }
            }
            for (int i = 0; i < n - 1; i++)
                for (int k = 0; k < seg; k++)
                {
                    int a = i * (seg + 1) + k, b = a + 1, c = a + seg + 1, d = c + 1;
                    idx.Add(a); idx.Add(c); idx.Add(b);
                    idx.Add(b); idx.Add(c); idx.Add(d);
                }
            return Finish(verts, norms, uvs, idx);
        }

        public static Mesh Torus(float R, float tube, int seg = 40, int tseg = 10)
        {
            var pts = new List<Vector3>();
            for (int i = 0; i <= seg; i++)
            {
                float a = Mathf.PI * 2f * i / seg;
                pts.Add(new Vector3(Mathf.Cos(a) * R, 0f, Mathf.Sin(a) * R));
            }
            return Tube(pts, tube, tseg);
        }

        /// <summary>Zaoblení rohů lomené čáry (kvadratická Bézierova křivka v každém rohu).</summary>
        public static List<Vector3> RoundPath(IList<Vector3> pts, float rb)
        {
            var outp = new List<Vector3>();
            if (pts.Count < 3) { outp.AddRange(pts); return outp; }
            outp.Add(pts[0]);
            for (int i = 1; i < pts.Count - 1; i++)
            {
                Vector3 a = pts[i - 1], b = pts[i], c = pts[i + 1];
                Vector3 d1 = b - a, d2 = c - b;
                float l1 = d1.magnitude, l2 = d2.magnitude;
                if (l1 < 0.001f || l2 < 0.001f) continue;
                d1 /= l1; d2 /= l2;
                float ang = Vector3.Angle(d1, d2) * Mathf.Deg2Rad;
                if (ang < 0.05f) { outp.Add(b); continue; }
                float dd = Mathf.Min(rb * Mathf.Tan(ang * 0.5f), Mathf.Min(l1, l2) * 0.45f);
                Vector3 p1 = b - d1 * dd, p2 = b + d2 * dd;
                for (int s = 0; s <= 6; s++)
                {
                    float t = s / 6f;
                    outp.Add(Vector3.Lerp(Vector3.Lerp(p1, b, t), Vector3.Lerp(b, p2, t), t));
                }
            }
            outp.Add(pts[pts.Count - 1]);
            return outp;
        }

        public static Mesh Finish(List<Vector3> v, List<Vector3> n, List<Vector2> uv, List<int> idx)
        {
            var m = new Mesh();
            if (v.Count > 65000) m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            m.SetVertices(v); m.SetNormals(n); m.SetUVs(0, uv); m.SetTriangles(idx, 0);
            m.RecalculateBounds();
            if (v.Count > 0) m.RecalculateTangents();
            return m;
        }
    }
}
