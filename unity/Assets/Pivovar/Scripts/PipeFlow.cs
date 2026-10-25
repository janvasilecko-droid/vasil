// Voda v potrubí: stěna trubky je lehce průhledná a uvnitř je sloupec kapaliny (vlastní mesh).
//  - vodorovná trubka se plní do výšky (rovná hladina), svislá zdola;
//  - povrch proudí rychlostí skutečného toku (m/s), protože textura se posouvá po metrech trubky;
//  - rychlý proud je neklidnější (silnější vlnky, víc bublinek), stojící voda je hladká.
using System.Collections.Generic;
using UnityEngine;

namespace Pivovar
{
    public class PipeFlow : MonoBehaviour
    {
        /// <summary>Vnější stěna trubky (průhledná).</summary>
        public Material mat;
        public Color liquid = new Color(0.3f, 0.6f, 0.9f);
        /// <summary>0 = prázdná, 1 = plná</summary>
        public float fill;
        /// <summary>rychlost proudu v m/s, znaménko = směr podél trubky v plánu, 0 = stojí</summary>
        public float speed;
        public float glow = 2.2f;
        /// <summary>0 = voda / sladina, 1 = hustý rmut (neprůhledná kaše se šrotem)</summary>
        public float thick;
        bool mashMap;

        // sloupec kapaliny
        public List<Vector3> path;
        public float rad;
        GameObject liqGo; MeshFilter liqMf; MeshRenderer liqMr; Material liqMat; Mesh liqMesh;
        int builtLevel = -1;
        float off;
        Color lastCol = new Color(-1f, 0f, 0f);
        const float Tile = 0.28f;                 // délka opakování textury proudu v metrech
        const int Levels = 14;

        void EnsureLiquid()
        {
            if (liqGo != null || path == null || path.Count < 2) return;
            liqGo = new GameObject("voda v trubce");
            liqGo.transform.SetParent(transform, false);
            liqMf = liqGo.AddComponent<MeshFilter>();
            liqMr = liqGo.AddComponent<MeshRenderer>();
            liqMr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            liqMat = HdrpUtil.Transparent(new Color(0.5f, 0.75f, 0.95f, 0.78f), 0f, 0.93f);
            HdrpUtil.SetCull(liqMat, 0);
            HdrpUtil.SetBaseMap(liqMat, LiquidLook.FlowColor, new Vector2(1f / Tile, 1f));
            HdrpUtil.SetNormalMap(liqMat, LiquidLook.FlowNormal, 0.5f, new Vector2(1f / Tile, 1f));
            liqMr.sharedMaterial = liqMat;
        }

        void Rebuild(int level)
        {
            builtLevel = level;
            if (liqMesh != null) Destroy(liqMesh);
            liqMesh = LiquidLook.PipeLiquid(path, rad, 12, level / (float)Levels);
            liqMf.sharedMesh = liqMesh;
            liqMr.enabled = level > 0;
        }

        void OnDestroy()
        {
            if (liqMesh != null) Destroy(liqMesh);
            if (liqMat != null) Destroy(liqMat);
        }

        void Update()
        {
            EnsureLiquid();
            float f = Mathf.Clamp01(fill);
            float air = Mathf.Abs(speed) / 4f;     // rychlý proud nese víc bublin a víří
            // vnější stěna: prázdná trubka je spíš mléčná, plná je skoro čirá a vidět je voda
            if (mat != null)
            {
                Color c = mat.HasProperty("_BaseColor") ? mat.GetColor("_BaseColor") : mat.color;
                c.a = Mathf.Lerp(LiquidLook.PipeEmptyAlpha, LiquidLook.PipeGlassAlpha, Mathf.Clamp01(f * 1.5f));
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c); else mat.color = c;
            }
            if (liqGo == null) return;
            int lv = Mathf.RoundToInt(f * Levels);
            if (f > 0.99f) lv = Levels;
            if (lv != builtLevel) Rebuild(lv);
            if (lv <= 0) return;

            // barva: kapalina v trubce, plná trubka je syté zbarvená, napůl prázdná světlejší
            if (liquid != lastCol)
            {
                lastCol = liquid;
                var col = new Color(liquid.r, liquid.g, liquid.b, mashMap ? 0.98f : 0.82f);
                liqMat.SetColor("_BaseColor", col);
                HdrpUtil.SetEmissive(liqMat, new Color(liquid.r, liquid.g, liquid.b), 24f);
            }
            bool wantMash = thick > 0.5f;
            if (wantMash != mashMap)
            {
                mashMap = wantMash;
                if (wantMash) { HdrpUtil.SetBaseMap(liqMat, LiquidLook.MashAlbedo, new Vector2(1f / 0.45f, 1f)); HdrpUtil.SetNormalMap(liqMat, LiquidLook.MashNormal, 0.9f, new Vector2(1f / 0.45f, 1f)); }
                else { HdrpUtil.SetBaseMap(liqMat, LiquidLook.FlowColor, new Vector2(1f / Tile, 1f)); HdrpUtil.SetNormalMap(liqMat, LiquidLook.FlowNormal, 0.5f, new Vector2(1f / Tile, 1f)); }
                lastCol = new Color(-1f, 0f, 0f);
            }
            float tileLen = mashMap ? 0.45f : Tile;
            liqMat.SetFloat("_Smoothness", mashMap ? 0.3f : 0.93f);
            if (!mashMap) liqMat.SetFloat("_NormalScale", Mathf.Lerp(0.35f, 1.4f, Mathf.Clamp01(air)));
            // proudění: u = délka v metrech, tiling 1/Tile, takže posun o Tile za 1 / |v| sekund
            off -= (speed / tileLen) * Time.deltaTime;
            off = Mathf.Repeat(off, 64f);
            Vector2 o = new Vector2(off, 0f);
            HdrpUtil.SetMapOffset(liqMat, "_BaseColorMap", o);
            HdrpUtil.SetMapOffset(liqMat, "_NormalMap", o);
        }
    }
}
