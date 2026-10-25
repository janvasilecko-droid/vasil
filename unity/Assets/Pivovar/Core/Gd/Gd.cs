// Globální funkce, konstanty a statické třídy Godotu (Time, OS, FileAccess, JSON, Geometry2D …) pro přeložený GDScript.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using KK = Pivovar.Gd.K;

namespace Pivovar.Gd
{
    public sealed class GdScriptRef
    {
        public readonly string name;
        public GdScriptRef(string n) { name = n; }
    }

    public sealed class GdWeak : GdObject
    {
        public Gv target;
        public override string ScriptName { get { return "WeakRef"; } }
        public override Gv Invoke(string n, Gv[] a) { if (n == "get_ref") return target; return base.Invoke(n, a); }
    }

    public sealed class GdRng : GdObject
    {
        public Random r = new Random();
        public override string ScriptName { get { return "RandomNumberGenerator"; } }
        public override Gv Invoke(string n, Gv[] a)
        {
            switch (n)
            {
                case "randomize": r = new Random(); return Gv.Null;
                case "randf": return Gv.F(r.NextDouble());
                case "randi": return Gv.I((long)(r.NextDouble() * 4294967296.0));
                case "randf_range": { double lo = Gd.A(a, 0).d, hi = Gd.A(a, 1).d; return Gv.F(lo + r.NextDouble() * (hi - lo)); }
                case "randi_range": { long lo = (long)Gd.A(a, 0).d, hi = (long)Gd.A(a, 1).d; return Gv.I(lo + (long)(r.NextDouble() * (hi - lo + 1))); }
                case "randfn": { double m = a.Length > 0 ? a[0].d : 0, dv = a.Length > 1 ? a[1].d : 1; double u1 = 1.0 - r.NextDouble(), u2 = r.NextDouble(); return Gv.F(m + dv * Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2)); }
            }
            if (n == "set" && Gd.A(a, 0).Str == "seed") { r = new Random((int)Gd.A(a, 1).d); return Gv.Null; }
            return base.Invoke(n, a);
        }
        public override Gv Set(string n, Gv v) { if (n == "seed") { r = new Random((int)v.d); return v; } return base.Set(n, v); }
    }

    public sealed class GdFile : GdObject
    {
        public string path; public bool write; public StringBuilder buf = new StringBuilder(); public string text = ""; public int pos; public bool closed;
        public override string ScriptName { get { return "FileAccess"; } }
        public override Gv Invoke(string n, Gv[] a)
        {
            switch (n)
            {
                case "get_as_text": return Gv.S(text);
                case "get_length": return Gv.I(text.Length);
                case "eof_reached": return Gv.B(pos >= text.Length);
                case "get_line":
                    {
                        if (pos >= text.Length) return Gv.S("");
                        int e = text.IndexOf('\n', pos); string line;
                        if (e < 0) { line = text.Substring(pos); pos = text.Length; } else { line = text.Substring(pos, e - pos); pos = e + 1; }
                        return Gv.S(line.TrimEnd('\r'));
                    }
                case "store_string": buf.Append(Gd.A(a, 0).ToStr()); return Gv.B(true);
                case "store_line": buf.Append(Gd.A(a, 0).ToStr()).Append('\n'); return Gv.B(true);
                case "store_var": buf.Append(Gd.Json(Gd.A(a, 0), false)); return Gv.B(true);
                case "get_var": return Gd.JsonParse(text);
                case "flush": Flush(); return Gv.Null;
                case "close": Close(); return Gv.Null;
                case "is_open": return Gv.B(!closed);
                case "get_path": return Gv.S(path);
            }
            return base.Invoke(n, a);
        }
        void Flush() { if (write && !closed) { try { Directory.CreateDirectory(Path.GetDirectoryName(Gd.MapPath(path))); File.WriteAllText(Gd.MapPath(path), buf.ToString(), new UTF8Encoding(false)); } catch { } } }
        void Close() { Flush(); closed = true; }
    }

    public sealed class GdRegEntry
    {
        public Func<Gv[], Gv> New; public Func<string, Gv> SGet; public Func<string, Gv[], Gv> SCall;
    }

    public static partial class Gd
    {
        // ---------- prostředí ----------
        public static Random Rng = new Random(20261025);
        public static string UserDir = Path.Combine(Path.GetTempPath(), "zajic_user");
        public static string ResDir = ".";
        public static Action<string> PrintHook = s => Console.WriteLine(s);
        public static Func<string, Gv> ResourceLoader;      // Unity vrstva: načtení textur apod.
        public static Gv[] CmdArgs = new Gv[0];
        static readonly Stopwatch clock = Stopwatch.StartNew();
        static readonly Dictionary<string, GdRegEntry> reg = new Dictionary<string, GdRegEntry>();

        static GdTree tree; static GdNode viewport;
        public static Gv Tree { get { if (tree == null) tree = new GdTree(); return tree.Self; } }
        public static Gv Viewport
        {
            get
            {
                if (viewport == null) { viewport = new GdNode(); viewport.TypeName = "Viewport"; viewport.Set("size", Gv.Vec(1920, 1080)); }
                return viewport.Self;
            }
        }
        public static Gv MousePos = Gv.Vec(0, 0);
        public static int QuitCode; public static bool Quit;
        /// <summary>Shovívavý režim: chyba v jednom uzlu se zaznamená a program běží dál.</summary>
        public static bool Lenient;
        public static readonly Dictionary<string, int> Errors = new Dictionary<string, int>();
        public static Action<string> ErrHook;
        public static void Err(string where, Exception ex)
        {
            var key = where + " :: " + ex.GetType().Name + ": " + ex.Message;
            int c; Errors.TryGetValue(key, out c); Errors[key] = c + 1;
            if (c == 0)
            {
                var st = (ex.StackTrace ?? "").Split('\n'); var sb = new StringBuilder(key);
                for (int i = 0; i < Math.Min(8, st.Length); i++) sb.Append("\n      ").Append(st[i].Trim());
                if (ErrHook != null) ErrHook(sb.ToString()); else PrintHook("[chyba] " + sb);
            }
        }
        public static string DisplayName = "headless";

        public static string MapPath(string p)
        {
            if (p.StartsWith("user://")) return Path.Combine(UserDir, p.Substring(7));
            if (p.StartsWith("res://")) return Path.Combine(ResDir, p.Substring(6));
            return p;
        }

        // ---------- konstrukce ----------
        public static Gv A(Gv[] a, int i) { return i < a.Length ? a[i] : default(Gv); }
        public static Gv Arr(params Gv[] a) { return Gv.A(new GdArray(new List<Gv>(a))); }
        public static Gv Dict(params Gv[] kv)
        {
            var d = new GdDict();
            for (int i = 0; i + 1 < kv.Length; i += 2) d.Set(kv[i], kv[i + 1]);
            return Gv.D(d);
        }
        public static Gv NewDict() { return Gv.D(new GdDict()); }
        public static Gv NewArray() { return Gv.A(new GdArray()); }
        public static Gv Fn(Func<Gv[], Gv> f) { return Gv.Fn(f); }
        public static IEnumerable<Gv> RangeIter(params Gv[] a)
        {
            if (a.Length == 1) { long n = (long)a[0].d; for (long i = 0; i < n; i++) yield return Gv.I(i); }
            else if (a.Length >= 2)
            {
                long b = (long)a[0].d, e = (long)a[1].d, st = a.Length > 2 ? (long)a[2].d : 1;
                if (st == 0) throw new GdError("range(): krok je 0");
                if (st > 0) for (long i = b; i < e; i += st) yield return Gv.I(i);
                else for (long i = b; i > e; i += st) yield return Gv.I(i);
            }
        }

        public static Gv Ctor(string t, params Gv[] a)
        {
            switch (t)
            {
                case "Vector2": return a.Length >= 2 ? Gv.Vec(a[0].d, a[1].d) : Gv.Vec(0, 0);
                case "Vector2i": return a.Length >= 2 ? Gv.Veci(a[0].d, a[1].d) : Gv.Veci(0, 0);
                case "Color":
                    if (a.Length == 0) return Gv.Col(0, 0, 0, 1);
                    if (a[0].k == KK.Str) { var c = ParseColor(a[0].Str); return a.Length > 1 ? Gv.Col(c.r, c.g, c.b, a[1].d) : Gv.Col(c.r, c.g, c.b, c.a); }
                    if (a[0].k == KK.Color) { var c = a[0].Cl; return Gv.Col(c.r, c.g, c.b, a.Length > 1 ? a[1].d : c.a); }
                    return Gv.Col(a[0].d, A(a, 1).d, A(a, 2).d, a.Length > 3 ? a[3].d : 1.0);
                case "Rect2":
                    if (a.Length == 4) return Gv.Rect(a[0].d, a[1].d, a[2].d, a[3].d);
                    if (a.Length == 2) return Gv.Rect(a[0].d, a[0].e, a[1].d, a[1].e);
                    return Gv.Rect(0, 0, 0, 0);
                case "Array": case "PackedStringArray": case "PackedVector2Array": case "PackedColorArray": case "PackedFloat32Array":
                case "PackedFloat64Array": case "PackedInt32Array": case "PackedInt64Array": case "PackedByteArray":
                    {
                        var arr = new GdArray(); arr.typed = t;
                        if (a.Length > 0 && a[0].k == KK.Arr) arr.l.AddRange(a[0].Arr.l);
                        return Gv.A(arr);
                    }
                case "Dictionary": return NewDict();
                case "String": case "StringName": return Gv.S(a.Length > 0 ? a[0].ToStr() : "");
                case "int": return a.Length > 0 ? a[0].AsI() : Gv.I(0);
                case "float": return a.Length > 0 ? a[0].AsF() : Gv.F(0);
                case "Callable": return Gv.Fn(x => Gv.Null);
            }
            return New(t, a);
        }

        public static Gv New(string t, params Gv[] a)
        {
            switch (t)
            {
                case "RandomNumberGenerator": return new GdRng().Self;
                case "JSON": { var n = new GdNode(); n.TypeName = "JSON"; return n.Self; }
            }
            var node = new GdNode(); node.TypeName = t; return node.Self;
        }

        public static Gv ParseColorGv(string s) { var c = ParseColor(s); return Gv.Col(c.r, c.g, c.b, c.a); }
        public static GdColor ParseColor(string s)
        {
            if (s.StartsWith("#")) s = s.Substring(1);
            try
            {
                if (s.Length == 6) return new GdColor(H(s, 0), H(s, 2), H(s, 4), 1);
                if (s.Length == 8) return new GdColor(H(s, 0), H(s, 2), H(s, 4), H(s, 6));
                if (s.Length == 3) return new GdColor(H1(s, 0), H1(s, 1), H1(s, 2), 1);
            }
            catch { }
            switch (s.ToLowerInvariant())
            {
                case "white": return new GdColor(1, 1, 1, 1);
                case "black": return new GdColor(0, 0, 0, 1);
                case "red": return new GdColor(1, 0, 0, 1);
                case "green": return new GdColor(0, 1, 0, 1);
                case "blue": return new GdColor(0, 0, 1, 1);
                case "yellow": return new GdColor(1, 1, 0, 1);
                case "gray": case "grey": return new GdColor(0.745, 0.745, 0.745, 1);
                case "transparent": return new GdColor(1, 1, 1, 0);
                case "orange": return new GdColor(1, 0.647, 0, 1);
            }
            return new GdColor(0, 0, 0, 1);
        }
        static double H(string s, int i) { return Convert.ToInt32(s.Substring(i, 2), 16) / 255.0; }
        static double H1(string s, int i) { return Convert.ToInt32(s.Substring(i, 1) + s.Substring(i, 1), 16) / 255.0; }

        // ---------- konstanty ----------
        public static Gv K(string n)
        {
            switch (n)
            {
                case "PI": return Gv.F(Math.PI);
                case "TAU": return Gv.F(Math.PI * 2);
                case "INF": return Gv.F(double.PositiveInfinity);
                case "NAN": return Gv.F(double.NaN);
                case "HORIZONTAL_ALIGNMENT_LEFT": return Gv.I(0);
                case "HORIZONTAL_ALIGNMENT_CENTER": return Gv.I(1);
                case "HORIZONTAL_ALIGNMENT_RIGHT": return Gv.I(2);
                case "HORIZONTAL_ALIGNMENT_FILL": return Gv.I(3);
                case "VERTICAL_ALIGNMENT_TOP": return Gv.I(0);
                case "VERTICAL_ALIGNMENT_CENTER": return Gv.I(1);
                case "VERTICAL_ALIGNMENT_BOTTOM": return Gv.I(2);
                case "TYPE_NIL": return Gv.I(0);
                case "TYPE_BOOL": return Gv.I(1);
                case "TYPE_INT": return Gv.I(2);
                case "TYPE_FLOAT": return Gv.I(3);
                case "TYPE_STRING": return Gv.I(4);
                case "TYPE_VECTOR2": return Gv.I(5);
                case "TYPE_VECTOR2I": return Gv.I(6);
                case "TYPE_RECT2": return Gv.I(7);
                case "TYPE_COLOR": return Gv.I(20);
                case "TYPE_OBJECT": return Gv.I(24);
                case "TYPE_CALLABLE": return Gv.I(25);
                case "TYPE_DICTIONARY": return Gv.I(27);
                case "TYPE_ARRAY": return Gv.I(28);
                case "MOUSE_BUTTON_LEFT": return Gv.I(1);
                case "MOUSE_BUTTON_RIGHT": return Gv.I(2);
                case "MOUSE_BUTTON_MIDDLE": return Gv.I(3);
                case "MOUSE_BUTTON_WHEEL_UP": return Gv.I(4);
                case "MOUSE_BUTTON_WHEEL_DOWN": return Gv.I(5);
                case "OK": return Gv.I(0);
                case "FAILED": return Gv.I(1);
            }
            if (n.StartsWith("KEY_"))
            {
                string kn = n.Substring(4);
                if (kn.Length == 1) return Gv.I(char.IsDigit(kn[0]) || char.IsLetter(kn[0]) ? kn[0] : 0);
                switch (kn)
                {
                    case "SPACE": return Gv.I(32); case "EQUAL": return Gv.I(61); case "MINUS": return Gv.I(45); case "PLUS": return Gv.I(43);
                    case "ESCAPE": return Gv.I(4194305); case "TAB": return Gv.I(4194306); case "BACKSPACE": return Gv.I(4194308);
                    case "ENTER": return Gv.I(4194309); case "KP_ENTER": return Gv.I(4194310); case "INSERT": return Gv.I(4194311); case "DELETE": return Gv.I(4194312);
                    case "HOME": return Gv.I(4194317); case "END": return Gv.I(4194318); case "LEFT": return Gv.I(4194319); case "UP": return Gv.I(4194320);
                    case "RIGHT": return Gv.I(4194321); case "DOWN": return Gv.I(4194322); case "PAGEUP": return Gv.I(4194323); case "PAGEDOWN": return Gv.I(4194324);
                    case "SHIFT": return Gv.I(4194325); case "CTRL": return Gv.I(4194326); case "META": return Gv.I(4194327); case "ALT": return Gv.I(4194328);
                    case "KP_MULTIPLY": return Gv.I(4194433); case "KP_DIVIDE": return Gv.I(4194434); case "KP_SUBTRACT": return Gv.I(4194435); case "KP_PERIOD": return Gv.I(4194436); case "KP_ADD": return Gv.I(4194437);
                }
                if (kn.StartsWith("F") && kn.Length <= 3) { int f; if (int.TryParse(kn.Substring(1), out f)) return Gv.I(4194331 + f); }
                if (kn.StartsWith("KP_")) { int d; if (int.TryParse(kn.Substring(3), out d)) return Gv.I(4194438 + d); }
                return Gv.I(0);
            }
            switch (n)
            {
                case "MOUSE_BUTTON_MASK_LEFT": return Gv.I(1); case "MOUSE_BUTTON_MASK_RIGHT": return Gv.I(2); case "MOUSE_BUTTON_MASK_MIDDLE": return Gv.I(4);
                case "NOTIFICATION_WM_CLOSE_REQUEST": return Gv.I(1006);
                case "NOTIFICATION_PREDELETE": return Gv.I(1);
            }
            if (n.StartsWith("PRESET_") || n.StartsWith("SIZE_") || n.StartsWith("ANCHOR") || n.StartsWith("CURSOR_") || n.StartsWith("MOUSE_FILTER") || n.StartsWith("TEXT_") || n.StartsWith("BASE_") || n.StartsWith("CLOCK") || n.StartsWith("LINE_"))
                return Gv.I(Math.Abs(n.GetHashCode() % 100000));
            throw new GdError("Neznámá konstanta '" + n + "'");
        }

        public static Gv StaticGet(string cls, string n)
        {
            switch (cls)
            {
                case "Vector2":
                    switch (n) { case "ZERO": return Gv.Vec(0, 0); case "ONE": return Gv.Vec(1, 1); case "UP": return Gv.Vec(0, -1); case "DOWN": return Gv.Vec(0, 1); case "LEFT": return Gv.Vec(-1, 0); case "RIGHT": return Gv.Vec(1, 0); case "INF": return Gv.Vec(double.PositiveInfinity, double.PositiveInfinity); }
                    break;
                case "Vector2i":
                    switch (n) { case "ZERO": return Gv.Veci(0, 0); case "ONE": return Gv.Veci(1, 1); }
                    break;
                case "Color":
                    switch (n)
                    {
                        case "WHITE": return Gv.Col(1, 1, 1, 1); case "BLACK": return Gv.Col(0, 0, 0, 1); case "RED": return Gv.Col(1, 0, 0, 1);
                        case "GREEN": return Gv.Col(0, 1, 0, 1); case "BLUE": return Gv.Col(0, 0, 1, 1); case "YELLOW": return Gv.Col(1, 1, 0, 1);
                        case "TRANSPARENT": return Gv.Col(1, 1, 1, 0); case "GRAY": return Gv.Col(0.745, 0.745, 0.745, 1); case "ORANGE": return Gv.Col(1, 0.647, 0, 1);
                    }
                    break;
                case "Image":
                    switch (n) { case "FORMAT_RGBA8": return Gv.I(5); case "FORMAT_RGB8": return Gv.I(4); case "FORMAT_L8": return Gv.I(0); case "FORMAT_LA8": return Gv.I(1); }
                    break;
                case "FileAccess":
                    switch (n) { case "READ": return Gv.I(1); case "WRITE": return Gv.I(2); case "READ_WRITE": return Gv.I(3); case "WRITE_READ": return Gv.I(7); }
                    break;
                case "CanvasItem":
                    if (n.StartsWith("TEXTURE_FILTER") || n.StartsWith("TEXTURE_REPEAT")) return Gv.I(Math.Abs(n.GetHashCode() % 10));
                    break;
                case "Input":
                    return Gv.I(0);
                case "Control":
                    return Gv.I(Math.Abs(n.GetHashCode() % 100));
            }
            // statika přeložených skriptů přes registr
            GdRegEntry r;
            if (reg.TryGetValue(cls, out r)) return r.SGet(n);
            if (n.ToUpperInvariant() == n) return Gv.I(Math.Abs(n.GetHashCode() % 1000));
            throw new GdError("Neznámá statická hodnota " + cls + "." + n);
        }

        public static Gv Static(string cls, string n, params Gv[] a)
        {
            switch (cls)
            {
                case "Time":
                    switch (n)
                    {
                        case "get_ticks_usec": return Gv.I(clock.ElapsedTicks * 1000000L / Stopwatch.Frequency);
                        case "get_ticks_msec": return Gv.I(clock.ElapsedMilliseconds);
                        case "get_unix_time_from_system": return Gv.F(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0);
                    }
                    break;
                case "OS":
                    switch (n)
                    {
                        case "get_cmdline_user_args": case "get_cmdline_args": return Gv.A(new GdArray(new List<Gv>(CmdArgs)));
                        case "get_name": return Gv.S("Unity");
                        case "has_feature": return Gv.B(false);
                        case "get_ticks_msec": return Gv.I(clock.ElapsedMilliseconds);
                        case "get_environment": return Gv.S(Environment.GetEnvironmentVariable(A(a, 0).Str ?? "") ?? "");
                    }
                    break;
                case "ProjectSettings":
                    if (n == "globalize_path") return Gv.S(MapPath(A(a, 0).Str));
                    break;
                case "FileAccess":
                    switch (n)
                    {
                        case "file_exists": return Gv.B(File.Exists(MapPath(A(a, 0).Str)));
                        case "open":
                            {
                                var p = A(a, 0).Str; int fl = (int)A(a, 1).d; var f = new GdFile(); f.path = p;
                                if ((fl & 2) != 0) { f.write = true; return f.Self; }
                                try { f.text = File.ReadAllText(MapPath(p)); } catch { return Gv.Null; }
                                return f.Self;
                            }
                        case "get_file_as_string": try { return Gv.S(File.ReadAllText(MapPath(A(a, 0).Str))); } catch { return Gv.S(""); }
                    }
                    break;
                case "DirAccess":
                    switch (n)
                    {
                        case "remove_absolute": try { File.Delete(MapPath(A(a, 0).Str)); return Gv.I(0); } catch { return Gv.I(1); }
                        case "make_dir_recursive_absolute": try { Directory.CreateDirectory(MapPath(A(a, 0).Str)); return Gv.I(0); } catch { return Gv.I(1); }
                        case "dir_exists_absolute": return Gv.B(Directory.Exists(MapPath(A(a, 0).Str)));
                        case "file_exists": return Gv.B(File.Exists(MapPath(A(a, 0).Str)));
                    }
                    break;
                case "ResourceLoader":
                    if (n == "exists") { var p = A(a, 0).Str; return Gv.B(File.Exists(MapPath(p))); }
                    if (n == "load") return Load(A(a, 0).Str);
                    break;
                case "JSON":
                    if (n == "parse_string") return JsonParse(A(a, 0).Str);
                    if (n == "stringify") return Gv.S(Json(A(a, 0), a.Length > 1 && A(a, 1).Str != null && A(a, 1).Str.Length > 0));
                    break;
                case "Geometry2D":
                    if (n == "get_closest_point_to_segment")
                    {
                        Gv p = A(a, 0), s1 = A(a, 1), s2 = A(a, 2);
                        double dx = s2.d - s1.d, dy = s2.e - s1.e, l2 = dx * dx + dy * dy;
                        if (l2 == 0) return s1;
                        double t = ((p.d - s1.d) * dx + (p.e - s1.e) * dy) / l2; t = t < 0 ? 0 : (t > 1 ? 1 : t);
                        return Gv.Vec(s1.d + dx * t, s1.e + dy * t);
                    }
                    break;
                case "Image":
                    if (n == "create") { var t = GdTexture.Create((int)A(a, 0).d, (int)A(a, 1).d); return t.Self; }
                    if (n == "load_from_file") return Load(A(a, 0).Str);
                    break;
                case "ImageTexture":
                    if (n == "create_from_image") { var t = A(a, 0).o as GdTexture; if (t != null) t.Version++; return A(a, 0); }
                    break;
                case "Font": case "SystemFont":
                    { var o = new GdNode(); o.TypeName = cls; return o.Self; }
                case "Input":
                    switch (n) { case "is_action_pressed": case "is_key_pressed": case "is_mouse_button_pressed": case "is_action_just_pressed": return Gv.B(false); case "get_mouse_position": return MousePos; }
                    return Gv.Null;
                case "DisplayServer": case "RenderingServer": case "Engine": case "Performance":
                    switch (n) { case "get_name": return Gv.S(DisplayName); case "get_frames_per_second": return Gv.I(60); case "get_time_scale": return Gv.F(1.0); case "is_editor_hint": return Gv.B(false); case "window_get_size": return Gv.Veci(1920, 1080); case "screen_get_size": return Gv.Veci(1920, 1080); }
                    return Gv.Null;
                case "Color":
                    if (n == "html") return ParseColorGv(A(a, 0).Str);
                    if (n == "from_hsv") return FromHsv(A(a, 0).d, A(a, 1).d, A(a, 2).d, a.Length > 3 ? a[3].d : 1.0);
                    break;
                case "Vector2":
                    if (n == "from_angle") return Gv.Vec(Math.Cos(A(a, 0).d), Math.Sin(A(a, 0).d));
                    break;
            }
            GdRegEntry r;
            if (reg.TryGetValue(cls, out r)) return r.SCall(n, a);
            throw new GdError("Neznámé statické volání " + cls + "." + n);
        }

        static Gv FromHsv(double h, double s, double v, double a)
        {
            h = ((h % 1.0) + 1.0) % 1.0; double c = v * s, x = c * (1 - Math.Abs((h * 6) % 2 - 1)), m = v - c, r = 0, g = 0, b = 0;
            int i = (int)(h * 6);
            switch (i) { case 0: r = c; g = x; break; case 1: r = x; g = c; break; case 2: g = c; b = x; break; case 3: g = x; b = c; break; case 4: r = x; b = c; break; default: r = c; b = x; break; }
            return Gv.Col(r + m, g + m, b + m, a);
        }

        // ---------- skripty ----------
        public static void Register(string key, Func<Gv[], Gv> nw, Func<string, Gv> sget, Func<string, Gv[], Gv> scall)
        {
            reg[key] = new GdRegEntry { New = nw, SGet = sget, SCall = scall };
        }
        public static Gv Script(string name) { return Gv.Scr(new GdScriptRef(name)); }
        public static Gv Load(string path)
        {
            if (path.EndsWith(".gd"))
            {
                var stem = Path.GetFileNameWithoutExtension(path);
                return Script(stem);
            }
            if (ResourceLoader != null) { var r = ResourceLoader(path); if (!r.IsNull) return r; }
            if (path.EndsWith(".png"))
            {
                var t = new GdTexture(); t.path = path; t.w = 64; t.h = 64;
                try { using (var fs = File.OpenRead(MapPath(path))) { var hd = new byte[24]; if (fs.Read(hd, 0, 24) == 24) { t.w = (hd[16] << 24) | (hd[17] << 16) | (hd[18] << 8) | hd[19]; t.h = (hd[20] << 24) | (hd[21] << 16) | (hd[22] << 8) | hd[23]; } } } catch { }
                return t.Self;
            }
            var o = new GdNode(); o.TypeName = "Resource"; o.Set("path", Gv.S(path)); o.Set("size", Gv.Vec(64, 64));
            return o.Self;
        }
        static GdRegEntry Entry(Gv self)
        {
            var s = (GdScriptRef)self.o; GdRegEntry r;
            reg.TryGetValue(s.name, out r); return r;
        }
        public static Gv ScriptGet(Gv self, string n)
        {
            var r = Entry(self);
            if (r == null) return Gv.Null;
            return r.SGet(n);
        }
        public static Gv ScriptCall(Gv self, string n, Gv[] a)
        {
            var r = Entry(self);
            if (r == null)
            {
                if (n == "new") { var node = new GdNode(); node.TypeName = ((GdScriptRef)self.o).name; return node.Self; }
                return Gv.Null;
            }
            if (n == "new") return r.New(a);
            return r.SCall(n, a);
        }

        // ---------- text ----------
        public static string FloatStr(double d)
        {
            if (double.IsNaN(d)) return "nan";
            if (double.IsPositiveInfinity(d)) return "inf";
            if (double.IsNegativeInfinity(d)) return "-inf";
            string s = d.ToString("R", CultureInfo.InvariantCulture);
            if (s.IndexOf('E') >= 0)
            {
                // 1E+20 -> 1e+20 ; Godot píše 1e+20
                s = s.Replace("E+", "e+").Replace("E-", "e-");
                int e = s.IndexOf('e');
                string mant = s.Substring(0, e), ex = s.Substring(e + 1);
                string sign = ex.Substring(0, 1), digs = ex.Substring(1).TrimStart('0'); if (digs.Length < 2) digs = digs.PadLeft(2, '0');
                return mant + "e" + sign + digs;
            }
            if (s.IndexOf('.') < 0) s += ".0";
            return s;
        }
        public static double ParseDouble(string s)
        {
            if (s == null) return 0;
            s = s.Trim();
            double v;
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out v)) return v;
            // vedoucí číselná část
            int i = 0; if (i < s.Length && (s[i] == '-' || s[i] == '+')) i++;
            int st = i; while (i < s.Length && (char.IsDigit(s[i]) || s[i] == '.')) i++;
            if (i < s.Length && (s[i] == 'e' || s[i] == 'E')) { int j = i + 1; if (j < s.Length && (s[j] == '-' || s[j] == '+')) j++; if (j < s.Length && char.IsDigit(s[j])) { while (j < s.Length && char.IsDigit(s[j])) j++; i = j; } }
            if (i > st && double.TryParse(s.Substring(0, i), NumberStyles.Float, CultureInfo.InvariantCulture, out v)) return v;
            return 0;
        }
        public static long ParseLeadingInt(string s)
        {
            if (s == null) return 0;
            s = s.Trim(); int i = 0; if (i < s.Length && (s[i] == '-' || s[i] == '+')) i++;
            int st = i; while (i < s.Length && char.IsDigit(s[i])) i++;
            if (i == st) return 0;
            long v; long.TryParse(s.Substring(0, i), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out v); return v;
        }

        public static string Format(string fmt, Gv arg)
        {
            List<Gv> vals = arg.k == KK.Arr ? arg.Arr.l : new List<Gv> { arg };
            var sb = new StringBuilder(); int vi = 0;
            for (int i = 0; i < fmt.Length; i++)
            {
                char c = fmt[i];
                if (c != '%') { sb.Append(c); continue; }
                i++;
                if (i >= fmt.Length) break;
                if (fmt[i] == '%') { sb.Append('%'); continue; }
                bool minus = false, plus = false, zero = false, space = false;
                for (; i < fmt.Length; i++)
                {
                    if (fmt[i] == '-') minus = true; else if (fmt[i] == '+') plus = true; else if (fmt[i] == '0') zero = true; else if (fmt[i] == ' ') space = true; else if (fmt[i] == '#') { } else break;
                }
                int width = 0, prec = -1;
                if (i < fmt.Length && fmt[i] == '*') { width = (int)(vi < vals.Count ? vals[vi++].d : 0); i++; }
                else while (i < fmt.Length && char.IsDigit(fmt[i])) { width = width * 10 + (fmt[i] - '0'); i++; }
                if (i < fmt.Length && fmt[i] == '.')
                {
                    i++; prec = 0;
                    if (i < fmt.Length && fmt[i] == '*') { prec = (int)(vi < vals.Count ? vals[vi++].d : 0); i++; }
                    else while (i < fmt.Length && char.IsDigit(fmt[i])) { prec = prec * 10 + (fmt[i] - '0'); i++; }
                }
                if (i >= fmt.Length) break;
                char t = fmt[i];
                Gv v = vi < vals.Count ? vals[vi++] : Gv.Null;
                string body;
                switch (t)
                {
                    case 'd': case 'i':
                        {
                            long n = v.IsNum || v.k == KK.Bool ? (long)Math.Truncate(v.d) : (long)ParseDouble(v.ToStr());
                            body = Math.Abs(n).ToString(CultureInfo.InvariantCulture);
                            if (prec > body.Length) body = body.PadLeft(prec, '0');
                            string sgn = n < 0 ? "-" : (plus ? "+" : (space ? " " : ""));
                            body = Pad(sgn, body, width, minus, zero);
                            sb.Append(body); continue;
                        }
                    case 'f':
                        {
                            double x = v.IsNum ? v.d : ParseDouble(v.ToStr());
                            if (prec < 0) prec = 6;
                            string s = Math.Abs(x).ToString("F" + prec, CultureInfo.InvariantCulture);
                            if (double.IsNaN(x)) s = "nan"; else if (double.IsInfinity(x)) s = "inf";
                            bool neg = x < 0 || (x == 0 && 1 / x < 0 && false);
                            if (neg && s.Trim('0', '.').Length == 0) neg = true;
                            string sgn = neg ? "-" : (plus ? "+" : (space ? " " : ""));
                            sb.Append(Pad(sgn, s, width, minus, zero && !double.IsNaN(x))); continue;
                        }
                    case 'e': case 'E':
                        {
                            double x = v.d; if (prec < 0) prec = 6;
                            string s = Math.Abs(x).ToString((t == 'e' ? "0." + new string('0', prec) + "e+00" : "0." + new string('0', prec) + "E+00"), CultureInfo.InvariantCulture);
                            if (prec == 0) s = s.Replace(".", "");
                            sb.Append(Pad(x < 0 ? "-" : (plus ? "+" : ""), s, width, minus, zero)); continue;
                        }
                    case 'g':
                        sb.Append(Pad("", FloatStr(v.d), width, minus, false)); continue;
                    case 'x': sb.Append(Pad("", ((long)v.d).ToString("x"), width, minus, zero)); continue;
                    case 'X': sb.Append(Pad("", ((long)v.d).ToString("X"), width, minus, zero)); continue;
                    case 'o': sb.Append(Pad("", Convert.ToString((long)v.d, 8), width, minus, zero)); continue;
                    case 'c': sb.Append(((char)(int)v.d).ToString()); continue;
                    case 's':
                        {
                            body = v.ToStr();
                            if (prec >= 0 && prec < body.Length) body = body.Substring(0, prec);
                            sb.Append(Pad("", body, width, minus, false)); continue;
                        }
                    case 'v': sb.Append(v.ToStr()); continue;
                    default: throw new GdError("Neplatný formát '%" + t + "'");
                }
            }
            return sb.ToString();
        }
        static string Pad(string sign, string body, int width, bool left, bool zero)
        {
            int len = sign.Length + body.Length;
            if (len >= width) return sign + body;
            if (left) return sign + body + new string(' ', width - len);
            if (zero) return sign + new string('0', width - len) + body;
            return new string(' ', width - len) + sign + body;
        }
        public static string FormatNamed(string t, Gv dict)
        {
            if (dict.k == KK.Dict)
            {
                var d = dict.Dict;
                for (int i = 0; i < d.Count; i++) t = t.Replace("{" + d.keys[i].ToStr() + "}", d.vals[i].ToStr());
            }
            else if (dict.k == KK.Arr)
            {
                var l = dict.Arr.l;
                for (int i = 0; i < l.Count; i++)
                {
                    if (l[i].k == KK.Arr && l[i].Arr.l.Count == 2) t = t.Replace("{" + l[i].Arr.l[0].ToStr() + "}", l[i].Arr.l[1].ToStr());
                    t = t.Replace("{" + i + "}", l[i].ToStr());
                }
            }
            return t;
        }
        public static Gv StringSize(Gv text, Gv size)
        {
            double sz = size.IsNum ? size.d : 16;
            string s = text.Str ?? text.ToStr();
            double w, h;
            if (Tess.Text != null) { Tess.Text.Measure(s, sz, out w, out h); return Gv.Vec(w, h); }
            return Gv.Vec(s.Length * sz * 0.52, sz * 1.2);
        }

        // ---------- JSON ----------
        public static Gv JsonParse(string s)
        {
            if (s == null) return Gv.Null;
            int i = 0;
            try
            {
                var v = JV(s, ref i);
                JWs(s, ref i);
                if (i != s.Length) return Gv.Null;
                return v;
            }
            catch (FormatException) { return Gv.Null; }
        }
        static void JWs(string s, ref int i) { while (i < s.Length && char.IsWhiteSpace(s[i])) i++; }
        static Gv JV(string s, ref int i)
        {
            JWs(s, ref i);
            if (i >= s.Length) throw new FormatException();
            char c = s[i];
            if (c == '{')
            {
                i++; var d = new GdDict(); JWs(s, ref i);
                if (i < s.Length && s[i] == '}') { i++; return Gv.D(d); }
                while (true)
                {
                    JWs(s, ref i); var k = JV(s, ref i); if (k.k != KK.Str) throw new FormatException();
                    JWs(s, ref i); if (i >= s.Length || s[i] != ':') throw new FormatException(); i++;
                    d.Set(k, JV(s, ref i)); JWs(s, ref i);
                    if (i < s.Length && s[i] == ',') { i++; continue; }
                    if (i < s.Length && s[i] == '}') { i++; break; }
                    throw new FormatException();
                }
                return Gv.D(d);
            }
            if (c == '[')
            {
                i++; var l = new List<Gv>(); JWs(s, ref i);
                if (i < s.Length && s[i] == ']') { i++; return Gv.A(new GdArray(l)); }
                while (true)
                {
                    l.Add(JV(s, ref i)); JWs(s, ref i);
                    if (i < s.Length && s[i] == ',') { i++; continue; }
                    if (i < s.Length && s[i] == ']') { i++; break; }
                    throw new FormatException();
                }
                return Gv.A(new GdArray(l));
            }
            if (c == '"')
            {
                i++; var sb = new StringBuilder();
                while (true)
                {
                    if (i >= s.Length) throw new FormatException();
                    char ch = s[i++];
                    if (ch == '"') break;
                    if (ch == '\\')
                    {
                        if (i >= s.Length) throw new FormatException();
                        char e = s[i++];
                        switch (e)
                        {
                            case 'n': sb.Append('\n'); break; case 't': sb.Append('\t'); break; case 'r': sb.Append('\r'); break;
                            case 'b': sb.Append('\b'); break; case 'f': sb.Append('\f'); break;
                            case 'u': sb.Append((char)Convert.ToInt32(s.Substring(i, 4), 16)); i += 4; break;
                            default: sb.Append(e); break;
                        }
                    }
                    else sb.Append(ch);
                }
                return Gv.S(sb.ToString());
            }
            if (s.Length - i >= 4 && string.CompareOrdinal(s, i, "true", 0, 4) == 0) { i += 4; return Gv.B(true); }
            if (s.Length - i >= 5 && string.CompareOrdinal(s, i, "false", 0, 5) == 0) { i += 5; return Gv.B(false); }
            if (s.Length - i >= 4 && string.CompareOrdinal(s, i, "null", 0, 4) == 0) { i += 4; return Gv.Null; }
            int st = i;
            while (i < s.Length && ("+-0123456789.eE".IndexOf(s[i]) >= 0)) i++;
            if (i == st) throw new FormatException();
            double v;
            if (!double.TryParse(s.Substring(st, i - st), NumberStyles.Float, CultureInfo.InvariantCulture, out v)) throw new FormatException();
            return Gv.F(v);    // Godot: všechna čísla v JSON jsou float
        }
        public static string Json(Gv v, bool indent)
        {
            var sb = new StringBuilder(); JW(sb, v, indent, 0); return sb.ToString();
        }
        static void JW(StringBuilder sb, Gv v, bool ind, int lvl)
        {
            switch (v.k)
            {
                case KK.Null: sb.Append("null"); return;
                case KK.Bool: sb.Append(v.d != 0 ? "true" : "false"); return;
                case KK.Int: sb.Append(((long)v.d).ToString(CultureInfo.InvariantCulture)); return;
                case KK.Float:
                    {
                        if (v.d == Math.Floor(v.d) && Math.Abs(v.d) < 1e15) sb.Append(((long)v.d).ToString(CultureInfo.InvariantCulture) + ".0");
                        else sb.Append(v.d.ToString("R", CultureInfo.InvariantCulture)); return;
                    }
                case KK.Str: JStr(sb, (string)v.o); return;
                case KK.Vec2: case KK.Vec2i: JStr(sb, v.ToStr()); return;
                case KK.Color: JStr(sb, v.ToStr()); return;
                case KK.Arr:
                    {
                        var l = v.Arr.l; if (l.Count == 0) { sb.Append("[]"); return; }
                        sb.Append('[');
                        for (int i = 0; i < l.Count; i++)
                        {
                            if (i > 0) sb.Append(',');
                            if (ind) { sb.Append('\n').Append(new string('\t', lvl + 1)); }
                            JW(sb, l[i], ind, lvl + 1);
                        }
                        if (ind) sb.Append('\n').Append(new string('\t', lvl));
                        sb.Append(']'); return;
                    }
                case KK.Dict:
                    {
                        var d = v.Dict; if (d.Count == 0) { sb.Append("{}"); return; }
                        sb.Append('{');
                        for (int i = 0; i < d.Count; i++)
                        {
                            if (i > 0) sb.Append(',');
                            if (ind) { sb.Append('\n').Append(new string('\t', lvl + 1)); }
                            JStr(sb, d.keys[i].ToStr()); sb.Append(ind ? ": " : ":");
                            JW(sb, d.vals[i], ind, lvl + 1);
                        }
                        if (ind) sb.Append('\n').Append(new string('\t', lvl));
                        sb.Append('}'); return;
                    }
                default: JStr(sb, v.ToStr()); return;
            }
        }
        static void JStr(StringBuilder sb, string s)
        {
            sb.Append('"');
            foreach (char c in s)
            {
                switch (c)
                {
                    case '"': sb.Append("\\\""); break; case '\\': sb.Append("\\\\"); break; case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break; case '\t': sb.Append("\\t"); break;
                    default: if (c < 32) sb.Append("\\u" + ((int)c).ToString("x4")); else sb.Append(c); break;
                }
            }
            sb.Append('"');
        }

        // ---------- globální funkce (přímá volání z přeloženého kódu) ----------
        public static Gv absf(Gv x) { return Gv.F(Math.Abs(x.d)); }
        public static Gv absi(Gv x) { return Gv.I(Math.Abs((long)x.d)); }
        public static Gv abs(Gv x) { return x.k == KK.Int ? Gv.I(Math.Abs((long)x.d)) : (x.k == KK.Vec2 ? Gv.Vec(Math.Abs(x.d), Math.Abs(x.e)) : Gv.F(Math.Abs(x.d))); }
        public static Gv maxf(Gv a, Gv b) { return Gv.F(a.d >= b.d || double.IsNaN(a.d) ? a.d : b.d); }
        public static Gv minf(Gv a, Gv b) { return Gv.F(a.d <= b.d || double.IsNaN(a.d) ? a.d : b.d); }
        public static Gv maxi(Gv a, Gv b) { return Gv.I((long)Math.Max((long)a.d, (long)b.d)); }
        public static Gv mini(Gv a, Gv b) { return Gv.I((long)Math.Min((long)a.d, (long)b.d)); }
        public static Gv max(params Gv[] a) { Gv m = a[0]; for (int i = 1; i < a.Length; i++) if (Gv.Lt(m, a[i])) m = a[i]; return m; }
        public static Gv min(params Gv[] a) { Gv m = a[0]; for (int i = 1; i < a.Length; i++) if (Gv.Lt(a[i], m)) m = a[i]; return m; }
        public static Gv clampf(Gv x, Gv lo, Gv hi) { double v = x.d; return Gv.F(v < lo.d ? lo.d : (v > hi.d ? hi.d : v)); }
        public static Gv clampi(Gv x, Gv lo, Gv hi) { long v = (long)x.d; return Gv.I(v < (long)lo.d ? (long)lo.d : (v > (long)hi.d ? (long)hi.d : v)); }
        public static Gv clamp(Gv x, Gv lo, Gv hi)
        {
            if (x.k == KK.Int && lo.k == KK.Int && hi.k == KK.Int) return clampi(x, lo, hi);
            if (x.k == KK.Vec2) return Gv.Vec(Math.Max(lo.d, Math.Min(hi.d, x.d)), Math.Max(lo.e, Math.Min(hi.e, x.e)));
            return clampf(x, lo, hi);
        }
        public static Gv roundi(Gv x) { return Gv.I((long)Math.Round(x.d, MidpointRounding.AwayFromZero)); }
        public static Gv roundf(Gv x) { return Gv.F(Math.Round(x.d, MidpointRounding.AwayFromZero)); }
        public static Gv round(Gv x) { return x.k == KK.Vec2 ? Gv.Vec(Math.Round(x.d, MidpointRounding.AwayFromZero), Math.Round(x.e, MidpointRounding.AwayFromZero)) : roundf(x); }
        public static Gv floor(Gv x) { return x.k == KK.Vec2 ? Gv.Vec(Math.Floor(x.d), Math.Floor(x.e)) : Gv.F(Math.Floor(x.d)); }
        public static Gv floorf(Gv x) { return Gv.F(Math.Floor(x.d)); }
        public static Gv floori(Gv x) { return Gv.I((long)Math.Floor(x.d)); }
        public static Gv ceil(Gv x) { return Gv.F(Math.Ceiling(x.d)); }
        public static Gv ceilf(Gv x) { return Gv.F(Math.Ceiling(x.d)); }
        public static Gv ceili(Gv x) { return Gv.I((long)Math.Ceiling(x.d)); }
        public static Gv sqrt(Gv x) { return Gv.F(Math.Sqrt(x.d)); }
        public static Gv pow(Gv a, Gv b) { return Gv.F(Math.Pow(a.d, b.d)); }
        public static Gv exp(Gv x) { return Gv.F(Math.Exp(x.d)); }
        public static Gv log(Gv x) { return Gv.F(Math.Log(x.d)); }
        public static Gv sin(Gv x) { return Gv.F(Math.Sin(x.d)); }
        public static Gv cos(Gv x) { return Gv.F(Math.Cos(x.d)); }
        public static Gv tan(Gv x) { return Gv.F(Math.Tan(x.d)); }
        public static Gv asin(Gv x) { return Gv.F(Math.Asin(x.d)); }
        public static Gv acos(Gv x) { return Gv.F(Math.Acos(x.d)); }
        public static Gv atan(Gv x) { return Gv.F(Math.Atan(x.d)); }
        public static Gv atan2(Gv y, Gv x) { return Gv.F(Math.Atan2(y.d, x.d)); }
        public static Gv fmod(Gv a, Gv b) { return Gv.F(a.d % b.d); }
        public static Gv fposmod(Gv a, Gv b) { double r = a.d % b.d; if (r < 0 && b.d > 0 || r > 0 && b.d < 0) r += b.d; return Gv.F(r); }
        public static Gv posmod(Gv a, Gv b) { long x = (long)a.d, y = (long)b.d; long r = x % y; if (r < 0 && y > 0 || r > 0 && y < 0) r += y; return Gv.I(r); }
        public static Gv signf(Gv x) { return Gv.F(x.d > 0 ? 1 : (x.d < 0 ? -1 : 0)); }
        public static Gv signi(Gv x) { return Gv.I(x.d > 0 ? 1 : (x.d < 0 ? -1 : 0)); }
        public static Gv sign(Gv x) { return x.k == KK.Int ? signi(x) : signf(x); }
        public static Gv deg_to_rad(Gv x) { return Gv.F(x.d * Math.PI / 180.0); }
        public static Gv rad_to_deg(Gv x) { return Gv.F(x.d * 180.0 / Math.PI); }
        public static Gv lerp(Gv a, Gv b, Gv t)
        {
            if (a.k == KK.Vec2) return Gv.Vec(a.d + (b.d - a.d) * t.d, a.e + (b.e - a.e) * t.d);
            if (a.k == KK.Color) return a.C("lerp", b, t);
            return Gv.F(a.d + (b.d - a.d) * t.d);
        }
        public static Gv lerpf(Gv a, Gv b, Gv t) { return Gv.F(a.d + (b.d - a.d) * t.d); }
        public static Gv inverse_lerp(Gv a, Gv b, Gv v) { return Gv.F((v.d - a.d) / (b.d - a.d)); }
        public static Gv remap(Gv v, Gv a, Gv b, Gv c, Gv d) { return Gv.F(c.d + (d.d - c.d) * ((v.d - a.d) / (b.d - a.d))); }
        public static Gv smoothstep(Gv a, Gv b, Gv x) { double t = Math.Max(0, Math.Min(1, (x.d - a.d) / (b.d - a.d))); return Gv.F(t * t * (3 - 2 * t)); }
        public static Gv move_toward(Gv f, Gv t, Gv d) { return Gv.F(Math.Abs(t.d - f.d) <= d.d ? t.d : f.d + Math.Sign(t.d - f.d) * d.d); }
        public static Gv snappedf(Gv v, Gv s) { return Gv.F(s.d == 0 ? v.d : Math.Floor(v.d / s.d + 0.5) * s.d); }
        public static Gv snapped(Gv v, Gv s) { return v.k == KK.Int && s.k == KK.Int ? Gv.I((long)(s.d == 0 ? v.d : Math.Floor(v.d / s.d + 0.5) * s.d)) : snappedf(v, s); }
        public static Gv is_nan(Gv x) { return Gv.B(double.IsNaN(x.d)); }
        public static Gv is_finite(Gv x) { return Gv.B(!double.IsNaN(x.d) && !double.IsInfinity(x.d)); }
        public static Gv is_inf(Gv x) { return Gv.B(double.IsInfinity(x.d)); }
        public static Gv is_equal_approx(Gv a, Gv b) { return Gv.B(Math.Abs(a.d - b.d) <= 1e-5 * Math.Max(1.0, Math.Abs(a.d))); }
        public static Gv is_zero_approx(Gv a) { return Gv.B(Math.Abs(a.d) < 1e-5); }
        public static Gv randi() { return Gv.I((long)(Rng.NextDouble() * 4294967296.0)); }
        public static Gv randf() { return Gv.F(Rng.NextDouble()); }
        public static Gv randf_range(Gv lo, Gv hi) { return Gv.F(lo.d + Rng.NextDouble() * (hi.d - lo.d)); }
        public static Gv randi_range(Gv lo, Gv hi) { long a = (long)lo.d, b = (long)hi.d; return Gv.I(a + (long)(Rng.NextDouble() * (b - a + 1))); }
        public static Gv print(params Gv[] a)
        {
            var sb = new StringBuilder(); foreach (var v in a) sb.Append(v.ToStr());
            PrintHook(sb.ToString()); return Gv.Null;
        }
        public static Gv weakref(Gv x) { var w = new GdWeak(); w.target = x; return w.Self; }
        public static Gv load(Gv path) { return Load(path.Str); }
        public static Gv len(Gv x)
        {
            switch (x.k) { case KK.Str: return Gv.I(x.Str.Length); case KK.Arr: return Gv.I(x.Arr.Count); case KK.Dict: return Gv.I(x.Dict.Count); }
            return Gv.I(0);
        }
        public static Gv is_instance_valid(Gv x) { return Gv.B(!x.IsNull); }

        // ---------- dynamické globální volání (jména, která nejsou přímo ve statice) ----------
        public static Gv Dyn(string n, params Gv[] a)
        {
            switch (n)
            {
                case "int": return A(a, 0).k == KK.Str ? Gv.I(ParseLeadingInt(a[0].Str)) : A(a, 0).AsI();
                case "float": return A(a, 0).AsF();
                case "str": { var sb = new StringBuilder(); foreach (var v in a) sb.Append(v.ToStr()); return Gv.S(sb.ToString()); }
                case "bool": return Gv.B(A(a, 0).T());
                case "typeof":
                    switch (A(a, 0).k)
                    {
                        case KK.Null: return Gv.I(0); case KK.Bool: return Gv.I(1); case KK.Int: return Gv.I(2); case KK.Float: return Gv.I(3);
                        case KK.Str: return Gv.I(4); case KK.Vec2: return Gv.I(5); case KK.Vec2i: return Gv.I(6); case KK.Rect2: return Gv.I(7);
                        case KK.Color: return Gv.I(20); case KK.Obj: return Gv.I(24); case KK.Fn: return Gv.I(25); case KK.Dict: return Gv.I(27); case KK.Arr: return Gv.I(28);
                    }
                    return Gv.I(0);
                case "prints": { var sb = new StringBuilder(); for (int i = 0; i < a.Length; i++) { if (i > 0) sb.Append(' '); sb.Append(a[i].ToStr()); } PrintHook(sb.ToString()); return Gv.Null; }
                case "printerr": case "push_error": case "push_warning": { var sb = new StringBuilder(); foreach (var v in a) sb.Append(v.ToStr()); PrintHook("[chyba] " + sb); return Gv.Null; }
                case "randomize": Rng = new Random(); return Gv.Null;
                case "seed": case "srand": Rng = new Random((int)A(a, 0).d); return Gv.Null;
                case "hash": return Gv.I(A(a, 0).KeyHash());
                case "preload": return Load(A(a, 0).Str);
                case "range": return Arr(new List<Gv>(RangeIter(a)).ToArray());
                case "str_to_var": return JsonParse(A(a, 0).Str);
                case "var_to_str": return Gv.S(Json(A(a, 0), false));
                case "error_string": return Gv.S("chyba " + A(a, 0).ToStr());
                case "linear_to_db": return Gv.F(20.0 * Math.Log10(Math.Max(1e-9, A(a, 0).d)));
                case "db_to_linear": return Gv.F(Math.Pow(10.0, A(a, 0).d / 20.0));
                case "sinh": return Gv.F(Math.Sinh(A(a, 0).d));
                case "cosh": return Gv.F(Math.Cosh(A(a, 0).d));
                case "tanh": return Gv.F(Math.Tanh(A(a, 0).d));
                case "log10": return Gv.F(Math.Log10(A(a, 0).d));
                case "wrapf": { double v = A(a, 0).d, lo = A(a, 1).d, hi = A(a, 2).d, r = hi - lo; return Gv.F(r == 0 ? lo : lo + (((v - lo) % r) + r) % r); }
                case "wrapi": { long v = (long)A(a, 0).d, lo = (long)A(a, 1).d, hi = (long)A(a, 2).d, r = hi - lo; return Gv.I(r == 0 ? lo : lo + (((v - lo) % r) + r) % r); }
                case "ease": return A(a, 0);
                case "nearest_po2": { long v = (long)A(a, 0).d, p = 1; while (p < v) p <<= 1; return Gv.I(p); }
                case "get_stack": return Arr();
                case "assert": return Gv.Null;
                case "Callable": return Gv.Fn(x => Gv.Null);
            }
            // zbytek: přímé statiky přes reflexi není potřeba – znamé funkce volají přímo
            switch (n)
            {
                case "absf": return absf(A(a, 0)); case "absi": return absi(A(a, 0)); case "abs": return abs(A(a, 0));
                case "maxf": return maxf(A(a, 0), A(a, 1)); case "minf": return minf(A(a, 0), A(a, 1));
                case "maxi": return maxi(A(a, 0), A(a, 1)); case "mini": return mini(A(a, 0), A(a, 1));
                case "max": return max(a); case "min": return min(a);
                case "clampf": return clampf(A(a, 0), A(a, 1), A(a, 2)); case "clampi": return clampi(A(a, 0), A(a, 1), A(a, 2)); case "clamp": return clamp(A(a, 0), A(a, 1), A(a, 2));
                case "roundi": return roundi(A(a, 0)); case "roundf": return roundf(A(a, 0)); case "round": return round(A(a, 0));
                case "floor": return floor(A(a, 0)); case "floorf": return floorf(A(a, 0)); case "floori": return floori(A(a, 0));
                case "ceil": return ceil(A(a, 0)); case "ceilf": return ceilf(A(a, 0)); case "ceili": return ceili(A(a, 0));
                case "sqrt": return sqrt(A(a, 0)); case "pow": return pow(A(a, 0), A(a, 1)); case "exp": return exp(A(a, 0)); case "log": return log(A(a, 0));
                case "sin": return sin(A(a, 0)); case "cos": return cos(A(a, 0)); case "tan": return tan(A(a, 0));
                case "asin": return asin(A(a, 0)); case "acos": return acos(A(a, 0)); case "atan": return atan(A(a, 0)); case "atan2": return atan2(A(a, 0), A(a, 1));
                case "fmod": return fmod(A(a, 0), A(a, 1)); case "fposmod": return fposmod(A(a, 0), A(a, 1)); case "posmod": return posmod(A(a, 0), A(a, 1));
                case "signf": return signf(A(a, 0)); case "signi": return signi(A(a, 0)); case "sign": return sign(A(a, 0));
                case "deg_to_rad": return deg_to_rad(A(a, 0)); case "rad_to_deg": return rad_to_deg(A(a, 0));
                case "lerp": return lerp(A(a, 0), A(a, 1), A(a, 2)); case "lerpf": return lerpf(A(a, 0), A(a, 1), A(a, 2));
                case "inverse_lerp": return inverse_lerp(A(a, 0), A(a, 1), A(a, 2)); case "remap": return remap(A(a, 0), A(a, 1), A(a, 2), A(a, 3), A(a, 4));
                case "smoothstep": return smoothstep(A(a, 0), A(a, 1), A(a, 2)); case "move_toward": return move_toward(A(a, 0), A(a, 1), A(a, 2));
                case "snappedf": return snappedf(A(a, 0), A(a, 1)); case "snapped": return snapped(A(a, 0), A(a, 1));
                case "is_finite": return is_finite(A(a, 0)); case "is_nan": return is_nan(A(a, 0)); case "is_inf": return is_inf(A(a, 0));
                case "is_equal_approx": return is_equal_approx(A(a, 0), A(a, 1)); case "is_zero_approx": return is_zero_approx(A(a, 0));
                case "randi": return randi(); case "randf": return randf();
                case "randf_range": return randf_range(A(a, 0), A(a, 1)); case "randi_range": return randi_range(A(a, 0), A(a, 1));
                case "print": return print(a); case "weakref": return weakref(A(a, 0)); case "load": return load(A(a, 0));
                case "len": return len(A(a, 0)); case "is_instance_valid": return is_instance_valid(A(a, 0));
            }
            throw new GdError("Neznámá globální funkce '" + n + "'");
        }
    }
}
