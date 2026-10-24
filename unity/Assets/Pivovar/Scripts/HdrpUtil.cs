// Pomocné funkce pro HDRP. Všechno, co se liší mezi verzemi balíčku (materiály, světla, Volume), jde přes reflexi,
// takže projekt přežije změnu API v nové verzi Unity: když něco chybí, vypíše se varování a scéna se postaví dál.
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

namespace Pivovar
{
    public static class HdrpUtil
    {
        const string Asm = "Unity.RenderPipelines.HighDefinition.Runtime";
        const string Ns = "UnityEngine.Rendering.HighDefinition.";

        static readonly System.Collections.Generic.Dictionary<string, Type> typeCache = new System.Collections.Generic.Dictionary<string, Type>();

        /// <summary>Najde typ podle jména: nejdřív v HDRP, potom v kterékoli načtené assembly v UnityEngine.Rendering*
        /// (nové verze přesouvají post-processing komponenty z HighDefinition do Core).</summary>
        static Type HdType(string name)
        {
            Type t;
            if (typeCache.TryGetValue(name, out t)) return t;
            t = Type.GetType(Ns + name + ", " + Asm);
            if (t == null)
            {
                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Type[] types;
                    try { types = a.GetTypes(); } catch (ReflectionTypeLoadException e) { types = e.Types; } catch (Exception) { continue; }
                    foreach (Type c in types)
                    {
                        if (c == null || c.Name != name || c.Namespace == null || !c.Namespace.StartsWith("UnityEngine.Rendering")) continue;
                        if (t == null || c.Namespace.Contains("HighDefinition")) t = c;
                    }
                }
            }
            typeCache[name] = t;
            return t;
        }

        public static bool IsHdrp { get { return HdType("HDMaterial") != null; } }

        static Shader litShader;
        static Shader LitShader
        {
            get
            {
                if (litShader == null) litShader = Shader.Find("HDRP/Lit");
                if (litShader == null) litShader = Shader.Find("Universal Render Pipeline/Lit");
                if (litShader == null) litShader = Shader.Find("Standard");
                return litShader;
            }
        }

        static void SetF(Material m, string n, float v) { if (m.HasProperty(n)) m.SetFloat(n, v); }

        public static void Validate(Material m)
        {
            Type t = HdType("HDMaterial");
            if (t == null) return;
            MethodInfo mi = t.GetMethod("ValidateMaterial", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(Material) }, null);
            if (mi != null) { try { mi.Invoke(null, new object[] { m }); } catch (Exception e) { Debug.LogWarning("HDMaterial.ValidateMaterial: " + e.Message); } }
        }

        /// <summary>PBR materiál (HDRP/Lit). Dvoustranný, protože řezy nádob a trubky se vidí z obou stran.</summary>
        public static Material Lit(Color c, float metallic, float smoothness, bool doubleSided = true)
        {
            var m = new Material(LitShader);
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c); else m.color = c;
            SetF(m, "_Metallic", metallic);
            SetF(m, "_Smoothness", smoothness);
            if (doubleSided)
            {
                SetF(m, "_DoubleSidedEnable", 1f);
                SetF(m, "_DoubleSidedNormalMode", 1f);
                SetF(m, "_CullMode", 0f);
                SetF(m, "_CullModeForward", 0f);
                m.EnableKeyword("_DOUBLESIDED_ON");
            }
            Validate(m);
            return m;
        }

        public static Material Transparent(Color c, float metallic, float smoothness)
        {
            var m = new Material(LitShader);
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c); else m.color = c;
            SetF(m, "_Metallic", metallic);
            SetF(m, "_Smoothness", smoothness);
            SetF(m, "_SurfaceType", 1f);
            SetF(m, "_BlendMode", 0f);
            SetF(m, "_ZWrite", 0f);
            SetF(m, "_AlphaCutoffEnable", 0f);
            SetF(m, "_DoubleSidedEnable", 1f);
            SetF(m, "_DoubleSidedNormalMode", 1f);
            SetF(m, "_CullMode", 0f);
            SetF(m, "_CullModeForward", 0f);
            SetF(m, "_EnableBlendModePreserveSpecularLighting", 1f);
            m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            m.EnableKeyword("_DOUBLESIDED_ON");
            m.renderQueue = 3000;
            Validate(m);
            m.renderQueue = 3000;
            return m;
        }

        public static void SetNormalMap(Material m, Texture2D nm, float scale, Vector2 tiling)
        {
            if (!m.HasProperty("_NormalMap")) return;
            m.SetTexture("_NormalMap", nm);
            SetF(m, "_NormalScale", scale);
            m.SetTextureScale("_NormalMap", tiling);
            m.EnableKeyword("_NORMALMAP");
            m.EnableKeyword("_NORMALMAP_TANGENT_SPACE");
            Validate(m);
        }

        public static void SetEmissive(Material m, Color c, float intensity)
        {
            if (!m.HasProperty("_EmissiveColor")) return;
            SetF(m, "_UseEmissiveIntensity", 0f);
            m.SetColor("_EmissiveColor", c * intensity);
            m.EnableKeyword("_EMISSION");
            m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        }

        public static void SetEmissiveMap(Material m, Texture2D map, Vector2 tiling)
        {
            if (!m.HasProperty("_EmissiveColorMap")) return;
            m.SetTexture("_EmissiveColorMap", map);
            m.SetTextureScale("_EmissiveColorMap", tiling);
            m.EnableKeyword("_EMISSIVE_COLOR_MAP");
            Validate(m);
        }

        public static void SetEmissiveOffset(Material m, Vector2 off)
        {
            if (m.HasProperty("_EmissiveColorMap")) m.SetTextureOffset("_EmissiveColorMap", off);
        }

        // ───────── světla ─────────
        public static Light MakeLight(Transform parent, string name, LightType type, Vector3 pos, Vector3 euler, Color col, float value, string unit, float range, bool shadows)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = pos; go.transform.rotation = Quaternion.Euler(euler);
            var l = go.AddComponent<Light>();
            l.type = type; l.color = col; l.range = range;
            l.shadows = shadows ? LightShadows.Soft : LightShadows.None;
            Type hd = HdType("HDAdditionalLightData");
            if (hd != null)
            {
                Component d = go.GetComponent(hd);
                if (d == null) d = go.AddComponent(hd);
                Type ut = HdType("LightUnit");
                if (ut != null)
                {
                    foreach (MethodInfo mi in hd.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (mi.Name != "SetIntensity") continue;
                        ParameterInfo[] ps = mi.GetParameters();
                        if (ps.Length != 2 || ps[1].ParameterType != ut) continue;
                        try { mi.Invoke(d, new object[] { value, Enum.Parse(ut, unit) }); }
                        catch (Exception e) { Debug.LogWarning("SetIntensity: " + e.Message); l.intensity = value; }
                        return l;
                    }
                }
            }
            l.intensity = value;
            return l;
        }

        // ───────── Volume (post-processing a prostředí) ─────────
        public static Volume MakeGlobalVolume(Transform parent, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var v = go.AddComponent<Volume>();
            v.isGlobal = true; v.priority = 10f;
            v.sharedProfile = ScriptableObject.CreateInstance<VolumeProfile>();
            return v;
        }

        public static VolumeComponent Add(Volume v, string typeName)
        {
            Type t = HdType(typeName);
            if (t == null) { Debug.LogWarning("HDRP: komponenta " + typeName + " v této verzi není"); return null; }
            return v.sharedProfile.Add(t, true);
        }

        /// <summary>Nastaví parametr Volume komponenty podle jména (přepíše override i hodnotu).</summary>
        public static void Set(VolumeComponent c, string field, object val)
        {
            if (c == null) return;
            try
            {
                Type t = c.GetType();
                object p = null;
                FieldInfo f = t.GetField(field, BindingFlags.Public | BindingFlags.Instance);
                if (f != null) p = f.GetValue(c);
                else { PropertyInfo pi = t.GetProperty(field, BindingFlags.Public | BindingFlags.Instance); if (pi != null) p = pi.GetValue(c); }
                if (p == null) { Debug.LogWarning("HDRP: " + t.Name + "." + field + " neexistuje"); return; }
                Type pt = p.GetType();
                pt.GetProperty("overrideState").SetValue(p, true);
                PropertyInfo vp = pt.GetProperty("value");
                Type vt = vp.PropertyType;
                object conv = val;
                if (vt.IsEnum && val is string) conv = Enum.Parse(vt, (string)val);
                else if (vt.IsEnum && val is int) conv = Enum.ToObject(vt, (int)val);
                else if (vt == typeof(float)) conv = Convert.ToSingle(val);
                else if (vt == typeof(int)) conv = Convert.ToInt32(val);
                else if (vt == typeof(bool)) conv = Convert.ToBoolean(val);
                vp.SetValue(p, conv);
            }
            catch (Exception e) { Debug.LogWarning("HDRP Set " + field + ": " + e.Message); }
        }
    }
}
