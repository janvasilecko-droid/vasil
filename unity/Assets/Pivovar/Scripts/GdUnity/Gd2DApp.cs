// Spuštění 2D režimu (věrná kopie Godot aplikace): po Play se vytvoří UI dokument s plátnem a spustí se přeložený kód z Godotu.
// Přepnutí na 3D: klávesa F9 (uloží se do PlayerPrefs "pivovar.mode"), nebo PlayerPrefs.SetString("pivovar.mode","3d").
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using Pivovar.Gd;
using G = Pivovar.Gd.Gd;

namespace Pivovar.GdUnity
{
    public sealed class Gd2DApp : MonoBehaviour
    {
        // Režimy (PlayerPrefs "pivovar.app"): "hybrid" (výchozí) = 3D varna z Unity + simulace, ovládání, rozvaděč a lupa z Godotu;
        // "2d" = jen kopie Godot aplikace; "3d" = jen 3D prohlížeč bez simulace.
        public const string ModeKey = "pivovar.app";
        public static string Mode { get { return PlayerPrefs.GetString(ModeKey, "hybrid"); } }
        public static bool Active { get { return Mode == "2d"; } }
        public static bool Hybrid { get { return Mode == "hybrid"; } }

        UIDocument doc; GdCanvasElement canvas; GdNode root;
        int frameNo; float lastLog;
        double lastX = -1, lastY = -1;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot()
        {
            if (!Active && !Hybrid) return;
            if (FindFirstObjectByType<Gd2DApp>() != null) return;
            new GameObject("Pivovar2D").AddComponent<Gd2DApp>();
        }

        void Start()
        {
            G.ResDir = Path.Combine(Application.streamingAssetsPath, "godot");
            G.UserDir = Path.Combine(Application.persistentDataPath, "user");
            Directory.CreateDirectory(G.UserDir);
            G.CmdArgs = new Gv[0];
            G.DisplayName = "unity";
            G.Lenient = true;
            GdUi.Active = true;
            G.PrintHook = line => Debug.Log("[gd] " + line);
            G.ErrHook = e => Debug.LogWarning("[gd] " + e);
            GdFontProvider.Init();
            Pivovar.Gd.Gen.GdRegistry.Init();
            UpdateClient.Install(gameObject);

            if (!Hybrid)
            {
                var cam = Camera.main;
                if (cam == null) { var go = new GameObject("Cam2D"); cam = go.AddComponent<Camera>(); go.tag = "MainCamera"; }
                cam.clearFlags = CameraClearFlags.SolidColor; cam.backgroundColor = new Color(0.08f, 0.09f, 0.1f); cam.cullingMask = 0;
            }
            else SeedUserPlan();

            var ps = ScriptableObject.CreateInstance<PanelSettings>();
            ps.scaleMode = PanelScaleMode.ConstantPixelSize; ps.scale = 1f;
            ps.themeStyleSheet = ScriptableObject.CreateInstance<ThemeStyleSheet>();
            ps.clearColor = !Hybrid; ps.colorClearValue = new Color(0.08f, 0.09f, 0.1f);
            doc = gameObject.AddComponent<UIDocument>();
            doc.panelSettings = ps;
            var rootVe = doc.rootVisualElement;
            rootVe.style.flexGrow = 1;
            canvas = new GdCanvasElement();
            rootVe.Add(canvas);
            HookInput(rootVe);

            G.ScreenW = Screen.width; G.ScreenH = Screen.height;
            try
            {
                var mv = Pivovar.Gd.Gen.S_main.New();
                root = (GdNode)mv.Obj;
                root.EnterTree();
            }
            catch (Exception e) { Debug.LogError("Pivovar2D: start selhal: " + e); return; }
            canvas.Root = root;
            if (Hybrid) StartHybrid();
            Debug.Log("Pivovar2D: spuštěno, okno " + Screen.width + "x" + Screen.height);
        }

        /// <summary>První spuštění: uživatelský plán Godotu (user://plan_upravy.json) se vezme z plan.json, který se používá ve 3D (Vasilův plán).</summary>
        static void SeedUserPlan()
        {
            try
            {
                string dst = Path.Combine(G.UserDir, "plan_upravy.json");
                string src = Path.Combine(Application.streamingAssetsPath, "plan.json");
                if (!File.Exists(dst) && File.Exists(src)) { File.Copy(src, dst); Debug.Log("Pivovar: uživatelský plán založen z " + src); }
            }
            catch (Exception e) { Debug.LogWarning("Pivovar: plán nejde založit: " + e.Message); }
        }

        SimSync sync;
        void StartHybrid()
        {
            GdRender.HideWorld = true;
            GdRender.KeepWorld = n =>
            {
                var ln = n as Pivovar.Gd.Gen.S_main_LiveNode;
                if (ln == null) return false;
                string k = ln.kind.Str;
                return k == "card" || k == "pump" || k == "leaks";
            };
            var sm = (Pivovar.Gd.Gen.S_main)root;
            sm.o3 = Gv.B(true);
            var builder = FindFirstObjectByType<BreweryBuilder>();
            if (builder == null) { Debug.LogError("Pivovar: chybí 3D scéna (BreweryBuilder)"); return; }
            sync = gameObject.AddComponent<SimSync>();
            sync.Init(builder, sm);
        }

        void Update()
        {
            if (root == null) return;
            var l = canvas.layout;
            if (l.width > 1 && l.height > 1) { G.ScreenW = l.width; G.ScreenH = l.height; }
            double dt = Math.Min(Time.unscaledDeltaTime, 0.1);
            try { GdHost.Frame(root, dt); } catch (Exception e) { G.Err("Frame", e); }
            try { GdHost.RedrawDirty(root); } catch (Exception e) { G.Err("Redraw", e); }
            canvas.MarkDirtyRepaint();
            frameNo++;
            if (Time.unscaledTime - lastLog > 5f && frameNo > 30)
            {
                lastLog = Time.unscaledTime;
                Debug.Log("Pivovar2D: položek " + canvas.LastItems + ", trojúhelníků " + canvas.LastTris + ", dávek " + canvas.LastBatches + ", chyb " + G.Errors.Count);
            }
            if (G.Quit) { G.Quit = false; Application.Quit(); }
        }

        // ---------- vstup ----------
        void HookInput(VisualElement ve)
        {
            ve.RegisterCallback<PointerDownEvent>(e => Mouse(e.position, e.button, true, e.modifiers, e.clickCount > 1), TrickleDown.TrickleDown);
            ve.RegisterCallback<PointerUpEvent>(e => Mouse(e.position, e.button, false, e.modifiers, false), TrickleDown.TrickleDown);
            ve.RegisterCallback<PointerMoveEvent>(e => Motion(e.position, e.modifiers), TrickleDown.TrickleDown);
            ve.RegisterCallback<WheelEvent>(e =>
            {
                int b = e.delta.y > 0 ? 5 : 4;
                SetMouse(e.mousePosition);
                Send(Ev("InputEventMouseButton", b, true, e.mousePosition, e.modifiers, false));
                Send(Ev("InputEventMouseButton", b, false, e.mousePosition, e.modifiers, false));
            }, TrickleDown.TrickleDown);
            ve.RegisterCallback<KeyDownEvent>(e => SendKey(e.keyCode, e.character, e.modifiers, true), TrickleDown.TrickleDown);
            ve.RegisterCallback<KeyUpEvent>(e => SendKey(e.keyCode, e.character, e.modifiers, false), TrickleDown.TrickleDown);
            canvas.RegisterCallback<AttachToPanelEvent>(_ => canvas.Focus());
            ve.RegisterCallback<PointerDownEvent>(_ => canvas.Focus());
        }

        static void SetMouse(Vector2 p) { G.MousePos = Gv.Vec(p.x, p.y); }
        static int GodotButton(int b) { return b == 0 ? 1 : b == 1 ? 2 : b == 2 ? 3 : b + 1; }

        GdEvent Ev(string type, int button, bool pressed, Vector2 pos, EventModifiers mods, bool dbl)
        {
            var e = GdEvent.Make(type, "button_index", button, "pressed", pressed, "double_click", dbl,
                "position", Gv.Vec(pos.x, pos.y), "global_position", Gv.Vec(pos.x, pos.y),
                "shift_pressed", (mods & EventModifiers.Shift) != 0, "ctrl_pressed", (mods & EventModifiers.Control) != 0,
                "alt_pressed", (mods & EventModifiers.Alt) != 0, "meta_pressed", (mods & EventModifiers.Command) != 0);
            int mask = 0; foreach (var d in G.MouseDown) mask |= d == 1 ? 1 : d == 2 ? 2 : d == 3 ? 4 : 0;
            e.Set("button_mask", Gv.I(mask));
            return e;
        }

        void Mouse(Vector2 pos, int b, bool down, EventModifiers mods, bool dbl)
        {
            int gb = GodotButton(b); SetMouse(pos);
            if (down) G.MouseDown.Add(gb); else G.MouseDown.Remove(gb);
            Send(Ev("InputEventMouseButton", gb, down, pos, mods, dbl));
        }

        void Motion(Vector2 pos, EventModifiers mods)
        {
            double rx = lastX < 0 ? 0 : pos.x - lastX, ry = lastY < 0 ? 0 : pos.y - lastY; lastX = pos.x; lastY = pos.y;
            SetMouse(pos);
            var e = Ev("InputEventMouseMotion", 0, false, pos, mods, false);
            e.Set("relative", Gv.Vec(rx, ry)); e.Set("velocity", Gv.Vec(rx * 60, ry * 60));
            Send(e);
        }


        void SendKey(KeyCode kc, char ch, EventModifiers mods, bool down)
        {
            // F9 přepíná 3D/2D
            if (down && kc == KeyCode.F9)
            {
                string next = Hybrid ? "2d" : "hybrid";
                PlayerPrefs.SetString(ModeKey, next); PlayerPrefs.Save();
                Debug.Log("Pivovar: příští spuštění bude '" + next + "' (PlayerPrefs " + ModeKey + ")");
            }
            int code = Godot(kc, ch);
            if (code == 0 && ch == 0) return;
            if (down) G.KeysDown.Add(code); else G.KeysDown.Remove(code);
            var ev = GdEvent.Make("InputEventKey", "pressed", down, "echo", false, "keycode", code, "physical_keycode", code,
                "unicode", (int)(ch >= 32 ? ch : (char)0),
                "shift_pressed", (mods & EventModifiers.Shift) != 0, "ctrl_pressed", (mods & EventModifiers.Control) != 0,
                "alt_pressed", (mods & EventModifiers.Alt) != 0, "meta_pressed", (mods & EventModifiers.Command) != 0);
            Send(ev);
        }

        static int Godot(KeyCode k, char ch)
        {
            if (k >= KeyCode.A && k <= KeyCode.Z) return 'A' + (k - KeyCode.A);
            if (k >= KeyCode.Alpha0 && k <= KeyCode.Alpha9) return '0' + (k - KeyCode.Alpha0);
            if (k >= KeyCode.Keypad0 && k <= KeyCode.Keypad9) return 4194438 + (k - KeyCode.Keypad0);
            if (k >= KeyCode.F1 && k <= KeyCode.F12) return 4194332 + (k - KeyCode.F1);
            switch (k)
            {
                case KeyCode.Space: return 32; case KeyCode.Escape: return 4194305; case KeyCode.Tab: return 4194306;
                case KeyCode.Backspace: return 4194308; case KeyCode.Return: return 4194309; case KeyCode.KeypadEnter: return 4194310;
                case KeyCode.Insert: return 4194311; case KeyCode.Delete: return 4194312;
                case KeyCode.Home: return 4194317; case KeyCode.End: return 4194318;
                case KeyCode.LeftArrow: return 4194319; case KeyCode.UpArrow: return 4194320; case KeyCode.RightArrow: return 4194321; case KeyCode.DownArrow: return 4194322;
                case KeyCode.PageUp: return 4194323; case KeyCode.PageDown: return 4194324;
                case KeyCode.LeftShift: case KeyCode.RightShift: return 4194325;
                case KeyCode.LeftControl: case KeyCode.RightControl: return 4194326;
                case KeyCode.LeftAlt: case KeyCode.RightAlt: return 4194328;
                case KeyCode.Equals: return 61; case KeyCode.Minus: return 45; case KeyCode.Plus: return 43;
                case KeyCode.KeypadPlus: return 4194437; case KeyCode.KeypadMinus: return 4194435; case KeyCode.KeypadMultiply: return 4194433; case KeyCode.KeypadDivide: return 4194434; case KeyCode.KeypadPeriod: return 4194436;
            }
            return ch >= 32 ? ch : 0;
        }

        void Send(GdEvent e)
        {
            if (root == null) return;
            G.InputHandled = false;
            try { GdHost.Dispatch(root, e); } catch (Exception ex) { G.Err("Dispatch", ex); }
        }
    }
}
