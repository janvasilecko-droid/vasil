// Písmo pro 2D režim: dynamický FontAsset z písma systému (Bahnschrift / Segoe UI / Arial); měření textu z posunů glyfů.
using System;
using UnityEngine;
using Pivovar.Gd;
#if UNITY_6000_0_OR_NEWER
using UnityEngine.TextCore.Text;
#endif

namespace Pivovar.GdUnity
{
    public sealed class GdFontProvider : ITextProvider
    {
#if UNITY_6000_0_OR_NEWER
        public static FontAsset Asset;
        static float ptSize = 90f;
#else
        public static object Asset;
#endif
        public static GdFontProvider Instance;

        public static void Init()
        {
            if (Instance != null) return;
            Instance = new GdFontProvider();
#if UNITY_6000_0_OR_NEWER
            foreach (var fam in new[] { "Bahnschrift", "Segoe UI", "Arial", "Liberation Sans", "DejaVu Sans" })
            {
                try
                {
                    var fa = FontAsset.CreateFontAsset(fam, "Regular", 90);
                    if (fa != null) { Asset = fa; ptSize = fa.faceInfo.pointSize > 0 ? fa.faceInfo.pointSize : 90f; Debug.Log("Pivovar2D: písmo " + fam); break; }
                }
                catch (Exception e) { Debug.LogWarning("Pivovar2D: písmo " + fam + ": " + e.Message); }
            }
            if (Asset != null) Asset.atlasPopulationMode = AtlasPopulationMode.Dynamic;
#endif
            Tess.Text = Instance;
        }

        public static double AscentPx(double size)
        {
#if UNITY_6000_0_OR_NEWER
            if (Asset != null) { var fi = Asset.faceInfo; return fi.ascentLine * size / ptSize; }
#endif
            return size * 0.95;
        }

        public void Measure(string text, double size, out double w, out double h)
        {
            w = 0; h = size * 1.2;
#if UNITY_6000_0_OR_NEWER
            if (Asset != null && !string.IsNullOrEmpty(text))
            {
                try
                {
                    string miss; Asset.TryAddCharacters(text, out miss);
                    double adv = 0, best = 0;
                    foreach (char ch in text)
                    {
                        if (ch == '\n') { best = Math.Max(best, adv); adv = 0; continue; }
                        Character c;
                        if (Asset.characterLookupTable.TryGetValue((uint)ch, out c) && c.glyph != null) adv += c.glyph.metrics.horizontalAdvance;
                        else adv += ptSize * 0.5;
                    }
                    best = Math.Max(best, adv);
                    w = best * size / ptSize;
                    h = (Asset.faceInfo.ascentLine - Asset.faceInfo.descentLine) * size / ptSize;
                    return;
                }
                catch { }
            }
#endif
            w = text == null ? 0 : text.Length * size * 0.55;
        }

        public double Ascent(double size) { return AscentPx(size); }
    }
}
