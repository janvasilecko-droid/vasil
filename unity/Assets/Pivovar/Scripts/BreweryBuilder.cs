// Postaví 3D varnu z plan.json (stejné rozmístění nádob a potrubí jako ve 2D verzi).
// Spuštění: stačí stisknout Play v jakékoli scéně – skript se vytvoří sám (RuntimeInitializeOnLoadMethod).
// Měřítko: 1 px plánu = 5 mm. Konvence souřadnic z Godotu (z k divákovi) se převádí funkcí Pz().
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Pivovar.Core;

namespace Pivovar
{
    public partial class BreweryBuilder : MonoBehaviour
    {
        public const float K = 0.005f;        // 1 px plánu = 5 mm
        public const float ZP = 0.42f;        // rovina potrubí (m před osou nádob)

        [System.NonSerialized] public Plan plan;
        [System.NonSerialized] public string planPath;
        public float floorY;                  // px plánu, kde stojí podlaha
        public bool demoFlow;
        public bool cutaway = true;

        Transform root;
        Camera cam;
        Material inox, steelB, steelP, dark, rubber, brass, glassMat;
        Texture2D brushed;
        readonly List<PipeFlow> flows = new List<PipeFlow>();
        readonly List<Vessel> vessels = new List<Vessel>();
        public sealed class Spinner { public Transform t; public string bind; public float dps = 40f; }
        readonly List<Spinner> spinners = new List<Spinner>();
        readonly Dictionary<string, Renderer> pumpLeds = new Dictionary<string, Renderer>();
        void AddSpinner(Transform t, string bind) { spinners.Add(new Spinner { t = t, bind = bind }); }

        // ── propojení se simulací z Godotu (hybridní režim): kreslí 3D svět, simulace a ovládání běží v Godotu ──
        public bool Hybrid { get { return Pivovar.GdUnity.Gd2DApp.Hybrid; } }
        public List<Vessel> VesselList { get { return vessels; } }
        public List<Spinner> SpinnerList { get { return spinners; } }
        public Dictionary<string, Renderer> PumpLeds { get { return pumpLeds; } }
        public PipeFlow FlowOf(string pipeId)
        {
            GameObject g; if (pipeObjs.TryGetValue(pipeId, out g) && g != null) return g.GetComponentInChildren<PipeFlow>();
            return null;
        }
        public List<PipeFlow> AllFlows() { return flows; }
        public void RefreshValve(PlanItem it) { RemovePart(it); BuildValve(it); }

        /// <summary>Hybridní režim: plán přichází z Godotu (po načtení a po každé úpravě v editoru) a celá 3D varna se postaví znovu.</summary>
        public void BeginHybrid(Plan p)
        {
            plan = p; planPath = null;
            Build();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot()
        {
            if (Pivovar.GdUnity.Gd2DApp.Active) return;   // 2D Godot kopie je jen záloha (PlayerPrefs pivovar.mode=2d)
            if (FindFirstObjectByType<BreweryBuilder>() != null) return;
            new GameObject("Pivovar").AddComponent<BreweryBuilder>();
        }

        void Start()
        {
            if (Hybrid) return;   // plán dodá Godot (BeginHybrid)
            string path = Path.Combine(Application.streamingAssetsPath, "plan.json");
            if (!File.Exists(path)) { Debug.LogError("Pivovar: chybí " + path + " (zkopíruj složku StreamingAssets z repozitáře)."); return; }
            try { plan = Plan.Parse(File.ReadAllText(path)); }
            catch (System.Exception e) { Debug.LogError("Pivovar: plan.json nejde načíst: " + e.Message); return; }
            planPath = path;
            Build();
            gameObject.AddComponent<PlanEditor>().Init(this);
            gameObject.AddComponent<ValveControl>().Init(this);
            Debug.Log("Pivovar: postaveno – nádob " + vessels.Count + ", trubek " + flows.Count + ". Ovládání: pravé tlačítko otáčí, kolečko přibližuje, F = tok v potrubí, X = řez/celé nádoby.");
        }

        // převod souřadnic: metry (Godot konvence) -> Unity
        public static Vector3 Pz(float x, float y, float zGodot) { return new Vector3(x, y, -zGodot); }
        Vector3 Pp(double px, double py, float z) { return Pz((float)px * K, (floorY - (float)py) * K, z); }

        void Update()
        {
            if (!Hybrid && !PlanEditor.Typing && Inp.Pressed("F")) SetDemoFlow(!demoFlow);
            if (!Hybrid && !PlanEditor.Typing && Inp.Pressed("X")) { cutaway = !cutaway; foreach (Vessel v in vessels) v.ApplyCut(cutaway); }
            float dt = Time.deltaTime;
            foreach (Spinner s in spinners) if (s.t != null) s.t.Rotate(0f, (Hybrid ? s.dps : 40f) * dt, 0f, Space.Self);
        }

        void OnGUI()
        {
            if (Hybrid) return;
            GUI.Label(new Rect(10, 8, 900, 24), "Pivovar Zajíc · HDRP · V = plochý/3D pohled, tažení myší = posun, kolečko = přiblížení k myši, WASD = posun, F = tok, X = řez, TAB = editor plánu");
        }

        void SetDemoFlow(bool on)
        {
            demoFlow = on;
            foreach (PipeFlow f in flows) { f.fill = on ? 1f : 0f; f.speed = on ? 1.5f : 0f; }
        }

        // ───────────────────────── stavba ─────────────────────────
        public void Rebuild() { Build(); }

        void Build()
        {
            if (root != null) DestroyImmediate(root.gameObject);
            root = new GameObject("Varna").transform;
            root.SetParent(transform, false);
            flows.Clear(); vessels.Clear(); spinners.Clear(); partObjs.Clear(); pumpLeds.Clear();

            floorY = 0f;
            foreach (PlanItem it in plan.OfType("vessel"))
            {
                string sh = it.Shape;
                if (sh == "platform" || sh == "inlet" || sh == "chiller") continue;
                floorY = Mathf.Max(floorY, (float)(it.Y + it.H));
            }
            floorY += 6f;

            hoseMat = null;
            MakeMaterials();
            MakePartMaterials();
            BuildEnvironment();
            BuildHall();
            foreach (PlanItem it in plan.OfType("vessel")) BuildVessel(it);
            BuildPipes();
            foreach (PlanItem it in plan.OfType("valve")) BuildValve(it);
            foreach (PlanItem it in plan.OfType("pump")) BuildPump(it);
            foreach (PlanItem it in plan.OfType("deco")) BuildDeco(it);
            BuildExtras();
            SetupCamera();
            SetDemoFlow(demoFlow);
        }

        // ───────────────────────── materiály ─────────────────────────
        static Texture2D BrushedNormal()
        {
            const int n = 256;
            var h = new float[n * n];
            var rnd = new System.Random(7);
            for (int y = 0; y < n; y++)
            {
                float rowBase = 0.35f + 0.3f * (float)rnd.NextDouble();
                for (int x = 0; x < n; x++)
                    h[y * n + x] = rowBase + 0.05f * Mathf.Sin(x * 0.12f + y) + 0.03f * ((float)rnd.NextDouble() - 0.5f);
            }
            var t = new Texture2D(n, n, TextureFormat.RGBA32, true, true);
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    float dx = h[y * n + (x + 1) % n] - h[y * n + (x + n - 1) % n];
                    float dy = h[((y + 1) % n) * n + x] - h[((y + n - 1) % n) * n + x];
                    Vector3 nn = new Vector3(-dx * 2.5f, -dy * 2.5f, 1f).normalized;
                    t.SetPixel(x, y, new Color(nn.x * 0.5f + 0.5f, nn.y * 0.5f + 0.5f, nn.z * 0.5f + 0.5f, 1f));
                }
            t.wrapMode = TextureWrapMode.Repeat; t.filterMode = FilterMode.Trilinear; t.anisoLevel = 8;
            t.Apply(true);
            return t;
        }

        void MakeMaterials()
        {
            brushed = BrushedNormal();
            inox = HdrpUtil.Lit(new Color(0.74f, 0.76f, 0.80f), 1f, 0.62f);
            HdrpUtil.SetNormalMap(inox, brushed, 0.5f, new Vector2(3f, 3f));
            steelB = HdrpUtil.Lit(new Color(0.70f, 0.72f, 0.76f), 1f, 0.55f);
            HdrpUtil.SetNormalMap(steelB, brushed, 0.5f, new Vector2(3f, 3f));
            steelP = HdrpUtil.Lit(new Color(0.80f, 0.82f, 0.85f), 1f, 0.85f);
            dark = HdrpUtil.Lit(new Color(0.06f, 0.06f, 0.07f), 0.3f, 0.45f);
            rubber = HdrpUtil.Lit(new Color(0.05f, 0.05f, 0.06f), 0f, 0.2f);
            brass = HdrpUtil.Lit(new Color(0.78f, 0.60f, 0.25f), 1f, 0.65f);
            glassMat = HdrpUtil.Transparent(new Color(0.75f, 0.85f, 0.95f, 0.22f), 0f, 0.95f);
        }

        // ───────────────────────── pomocné kreslení ─────────────────────────
        public GameObject Add(Transform parent, Mesh mesh, Material mat, Vector3 gpos, Vector3 euler, string name = "m", bool shadows = true)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent != null ? parent : root, false);
            go.transform.localPosition = new Vector3(gpos.x, gpos.y, -gpos.z);
            go.transform.localRotation = Quaternion.Euler(euler);
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            var mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterial = mat;
            if (!shadows) mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            return go;
        }

        public GameObject Cyl(Transform parent, float r, float h, Material mat, Vector3 gpos, Vector3 euler, int seg = 24, float r2 = -1f)
        {
            return Add(parent, MeshKit.Cylinder(r, h, seg, r2), mat, gpos, euler, "cyl");
        }
        public GameObject Box(Transform parent, Vector3 size, Material mat, Vector3 gpos, Vector3 euler)
        {
            return Add(parent, MeshKit.Box(size), mat, gpos, euler, "box");
        }
        GameObject CylX(Transform parent, float r, float h, Material mat, Vector3 gpos, int seg = 24)
        {
            return Add(parent, MeshKit.Cylinder(r, h, seg), mat, gpos, new Vector3(0, 0, 90f), "cylx");
        }
        GameObject CylZ(Transform parent, float r, float h, Material mat, Vector3 gpos, int seg = 24)
        {
            return Add(parent, MeshKit.Cylinder(r, h, seg), mat, gpos, new Vector3(90f, 0, 0), "cylz");
        }

        // ───────────────────────── prostředí a hala ─────────────────────────
        void BuildEnvironment()
        {
            var vol = HdrpUtil.MakeGlobalVolume(root, "Volume (prostředí)");
            var ve = HdrpUtil.Add(vol, "VisualEnvironment");
            HdrpUtil.Set(ve, "skyType", 3);               // Gradient sky = ambient pro halu
            HdrpUtil.Set(ve, "skyAmbientMode", 1);        // Dynamic
            var gs = HdrpUtil.Add(vol, "GradientSky");
            HdrpUtil.Set(gs, "top", new Color(0.60f, 0.66f, 0.76f));
            HdrpUtil.Set(gs, "middle", new Color(0.55f, 0.55f, 0.55f));
            HdrpUtil.Set(gs, "bottom", new Color(0.30f, 0.30f, 0.33f));
            HdrpUtil.Set(gs, "gradientDiffusion", 3f);
            HdrpUtil.Set(gs, "skyIntensityMode", "Multiplier");
            HdrpUtil.Set(gs, "multiplier", 350f);          // nity: bez toho je obloha ~1 nit a kov odráží černo
            var ex = HdrpUtil.Add(vol, "Exposure");
            HdrpUtil.Set(ex, "mode", "Fixed");
            HdrpUtil.Set(ex, "fixedExposure", 8.4f);
            HdrpUtil.Set(ex, "compensation", 0f);
            var tm = HdrpUtil.Add(vol, "Tonemapping");
            HdrpUtil.Set(tm, "mode", "ACES");
            var bl = HdrpUtil.Add(vol, "Bloom");
            HdrpUtil.Set(bl, "intensity", 0.18f);
            HdrpUtil.Set(bl, "threshold", 1.0f);
            var ao = HdrpUtil.Add(vol, "ScreenSpaceAmbientOcclusion");
            HdrpUtil.Set(ao, "intensity", 1.2f);
            HdrpUtil.Set(ao, "radius", 0.8f);
            var ssr = HdrpUtil.Add(vol, "ScreenSpaceReflection");
            HdrpUtil.Set(ssr, "enabled", true);
            var fog = HdrpUtil.Add(vol, "Fog");
            HdrpUtil.Set(fog, "enabled", true);
            HdrpUtil.Set(fog, "meanFreePath", 120f);
            HdrpUtil.Set(fog, "enableVolumetricFog", true);
            HdrpUtil.Set(fog, "albedo", new Color(0.9f, 0.92f, 0.98f));
            var vg = HdrpUtil.Add(vol, "Vignette");
            HdrpUtil.Set(vg, "intensity", 0.18f);
            var mb = HdrpUtil.Add(vol, "MotionBlur");
            HdrpUtil.Set(mb, "intensity", 0f);

            // šablona HDRP má vlastní slunce se stíny; HDRP umí stíny jen z jednoho směrového světla, tak ho vypneme
            foreach (Light l in FindObjectsByType<Light>(FindObjectsSortMode.None))
                if (l.type == LightType.Directional && l.transform.root != root) l.enabled = false;

            // slunce přes světlík (strop a stěny nevrhají stíny, ať světlo prochází)
            HdrpUtil.MakeLight(root, "Slunce", LightType.Directional, new Vector3(0, 10, 0), new Vector3(55f, 155f, 0f), new Color(1f, 0.95f, 0.88f), 2500f, "Lux", 0f, true);
        }

        void BuildHall()
        {
            float x0 = -12f, x1 = 46f, depth = 30f, hgt = 8f;
            Texture2D tiles = FloorTexture(), wtiles = WallTileTexture();
            Material fm = HdrpUtil.Lit(Color.white, 0f, 0.55f, false);
            if (fm.HasProperty("_BaseColorMap")) { fm.SetTexture("_BaseColorMap", tiles); fm.SetTextureScale("_BaseColorMap", new Vector2((x1 - x0) / 1.2f, depth / 1.2f)); }
            Add(root, MeshKit.Box(new Vector3(x1 - x0, 0.2f, depth)), fm, new Vector3((x0 + x1) * 0.5f, -0.1f, 0f), Vector3.zero, "podlaha");

            Material wall = HdrpUtil.Lit(new Color(0.86f, 0.86f, 0.84f), 0f, 0.25f, false);
            Material tileWall = HdrpUtil.Lit(new Color(0.95f, 0.96f, 0.97f), 0f, 0.85f, false);
            if (tileWall.HasProperty("_BaseColorMap")) { tileWall.SetTexture("_BaseColorMap", wtiles); tileWall.SetTextureScale("_BaseColorMap", new Vector2((x1 - x0) / 0.6f, 3.2f / 0.6f)); }
            Add(root, MeshKit.Box(new Vector3(x1 - x0, 3.2f, 0.3f)), tileWall, new Vector3((x0 + x1) * 0.5f, 1.6f, -depth * 0.5f), Vector3.zero, "obklad", false);
            Add(root, MeshKit.Box(new Vector3(x1 - x0, hgt - 3.2f, 0.3f)), wall, new Vector3((x0 + x1) * 0.5f, 3.2f + (hgt - 3.2f) * 0.5f, -depth * 0.5f), Vector3.zero, "stena", false);
            Add(root, MeshKit.Box(new Vector3(0.3f, hgt, depth)), wall, new Vector3(x0, hgt * 0.5f, 0f), Vector3.zero, "stena L", false);
            Add(root, MeshKit.Box(new Vector3(0.3f, hgt, depth)), wall, new Vector3(x1, hgt * 0.5f, 0f), Vector3.zero, "stena P", false);
            Add(root, MeshKit.Box(new Vector3(x1 - x0, 0.3f, depth)), wall, new Vector3((x0 + x1) * 0.5f, hgt, 0f), Vector3.zero, "strop", false);

            // okna (svítící plochy) s příčkami
            Material win = HdrpUtil.Lit(new Color(0.7f, 0.82f, 1f), 0f, 0.1f, false);
            HdrpUtil.SetEmissive(win, new Color(0.7f, 0.82f, 1f), 3500f);
            Material frame = HdrpUtil.Lit(new Color(0.2f, 0.2f, 0.22f), 0.2f, 0.5f, false);
            for (int i = 0; i < 6; i++)
            {
                float wx = 2f + i * 8f;
                Add(root, MeshKit.Box(new Vector3(3.2f, 1.8f, 0.05f)), win, new Vector3(wx, 5f, -depth * 0.5f + 0.2f), Vector3.zero, "okno", false);
                for (int s = -1; s <= 1; s++)
                    Add(root, MeshKit.Box(new Vector3(0.06f, 1.8f, 0.07f)), frame, new Vector3(wx + s * 1.0f, 5f, -depth * 0.5f + 0.22f), Vector3.zero, "pricka", false);
            }

            // vazníky a svítidla
            Material beam = HdrpUtil.Lit(new Color(0.25f, 0.27f, 0.30f), 0.6f, 0.4f, false);
            Material lampMat = HdrpUtil.Lit(new Color(1f, 0.96f, 0.85f), 0f, 0.3f, false);
            HdrpUtil.SetEmissive(lampMat, new Color(1f, 0.96f, 0.85f), 2500f);
            for (int i = 0; i < 8; i++)
            {
                float bx = -4f + i * 7f;
                Add(root, MeshKit.Box(new Vector3(0.3f, 0.45f, depth)), beam, new Vector3(bx, hgt - 0.4f, 0f), Vector3.zero, "vaznik", false);
                Add(root, MeshKit.Box(new Vector3(1.4f, 0.12f, 0.35f)), lampMat, new Vector3(bx, hgt - 1.0f, 1.0f), Vector3.zero, "lampa", false);
                if (i % 2 == 0)
                    HdrpUtil.MakeLight(root, "Světlo " + i, LightType.Point, Pz(bx, hgt - 1.4f, 1.5f), Vector3.zero, new Color(1f, 0.95f, 0.85f), 20000f, "Lumen", 16f, false);
            }
            // odtokový žlab
            Add(root, MeshKit.Box(new Vector3(x1 - x0 - 2f, 0.02f, 0.22f)), HdrpUtil.Lit(new Color(0.12f, 0.12f, 0.13f), 0.8f, 0.5f, false), new Vector3((x0 + x1) * 0.5f, 0.01f, 2.6f), Vector3.zero, "zlab", false);
        }

        // podlaha: velké šedobéžové dlaždice 0,4 m s tmavou spárou a nepravidelným odstínem (jako na fotkách)
        static Texture2D FloorTexture()
        {
            const int n = 512;
            var t = new Texture2D(n, n, TextureFormat.RGBA32, true, false);
            var rnd = new System.Random(11);
            float[] tint = new float[9]; for (int i = 0; i < 9; i++) tint[i] = (float)rnd.NextDouble();
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    int cx = Mathf.Min(2, x / (n / 3)), cy = Mathf.Min(2, y / (n / 3));   // 512 / 170 = 3,01 → poslední 2 sloupce/řádky by mířily mimo pole tint[9]
                    int lx = x % (n / 3), ly = y % (n / 3);
                    bool edge = lx < 4 || ly < 4;
                    float k = tint[cy * 3 + cx];
                    float noise = 0.04f * Mathf.Sin(x * 0.9f + y * 0.31f) * Mathf.Cos(y * 0.7f - x * 0.13f) + 0.03f * Mathf.Sin(x * 0.05f + k * 9f);
                    Color baseC = Color.Lerp(new Color(0.55f, 0.53f, 0.50f), new Color(0.66f, 0.62f, 0.55f), k);
                    Color c = edge ? new Color(0.30f, 0.29f, 0.27f) : baseC * (1f + noise);
                    c.a = 1f; t.SetPixel(x, y, c);
                }
            t.wrapMode = TextureWrapMode.Repeat; t.filterMode = FilterMode.Trilinear; t.anisoLevel = 8;
            t.Apply(true);
            return t;
        }

        // obklad: bílé dlaždice 0,2 × 0,3 m se světlou spárou
        static Texture2D WallTileTexture()
        {
            const int n = 256;
            var t = new Texture2D(n, n, TextureFormat.RGBA32, true, false);
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    bool edge = (x % (n / 3) < 2) || (y % (n / 2) < 2);
                    float v = 0.93f + 0.02f * Mathf.Sin(x * 0.31f) * Mathf.Cos(y * 0.23f);
                    Color c = edge ? new Color(0.62f, 0.62f, 0.60f) : new Color(v, v, v * 1.01f);
                    c.a = 1f; t.SetPixel(x, y, c);
                }
            t.wrapMode = TextureWrapMode.Repeat; t.filterMode = FilterMode.Trilinear; t.anisoLevel = 8;
            t.Apply(true);
            return t;
        }

        // ───────────────────────── kamera ─────────────────────────
        void SetupCamera()
        {
            cam = Camera.main;
            if (cam == null)
            {
                var go = new GameObject("Kamera");
                go.tag = "MainCamera";
                cam = go.AddComponent<Camera>();
                go.AddComponent<AudioListener>();
            }
            cam.fieldOfView = 55f; cam.nearClipPlane = 0.1f; cam.farClipPlane = 300f;
            cam.clearFlags = CameraClearFlags.Skybox;
            if (cam.GetComponent<OrbitCamera>() == null) cam.gameObject.AddComponent<OrbitCamera>();
            // vyhlazení hran (SMAA) přes reflexi, aby kód nezávisel na verzi HDRP
            try
            {
                System.Type t = System.Type.GetType("UnityEngine.Rendering.HighDefinition.HDAdditionalCameraData, Unity.RenderPipelines.HighDefinition.Runtime");
                if (t != null)
                {
                    Component d = cam.GetComponent(t); if (d == null) d = cam.gameObject.AddComponent(t);
                    System.Reflection.PropertyInfo p = t.GetProperty("antialiasing");
                    if (p != null) p.SetValue(d, System.Enum.Parse(p.PropertyType, "SubpixelMorphologicalAntiAliasing"));
                }
            }
            catch (System.Exception e) { Debug.LogWarning("Antialiasing: " + e.Message); }
        }
    }
}
