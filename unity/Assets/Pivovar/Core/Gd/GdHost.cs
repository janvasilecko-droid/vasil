// Hostitel scény: smyčka snímků (_process, časovače, tweeny, signál process_frame), překreslení uzlů a předávání vstupu.
using System;
using System.Collections.Generic;
using System.Reflection;
using KK = Pivovar.Gd.K;

namespace Pivovar.Gd
{
    /// <summary>Událost vstupu (InputEventMouseButton, …Motion, …Key, …PanGesture, …MagnifyGesture).</summary>
    public sealed class GdEvent : GdObject
    {
        public string Type;
        public GdEvent(string type) { Type = type; }
        public override string ScriptName { get { return Type; } }
        public override bool IsA(string t)
        {
            if (t == Type || t == "InputEvent" || t == "Resource" || t == "RefCounted" || t == "Object") return true;
            if ((Type == "InputEventMouseButton" || Type == "InputEventMouseMotion") && (t == "InputEventMouse" || t == "InputEventWithModifiers")) return true;
            if (Type == "InputEventKey" && t == "InputEventWithModifiers") return true;
            return false;
        }
        public override Gv Invoke(string n, Gv[] a)
        {
            switch (n)
            {
                case "is_pressed": return Get("pressed");
                case "is_action_pressed": return Gv.B(false);
                case "is_action": return Gv.B(false);
                case "as_text": return Gv.S(Type);
                case "is_echo": return Get("echo");
                case "xformed_by": return Self;
            }
            return base.Invoke(n, a);
        }
        public override Gv Get(string n)
        {
            var v = base.Get(n);
            if (v.IsNull)
            {
                switch (n)
                {
                    case "pressed": case "echo": case "shift_pressed": case "ctrl_pressed": case "alt_pressed": case "meta_pressed": case "double_click": case "command_or_control_autoremap": return Gv.B(false);
                    case "position": case "global_position": case "relative": case "velocity": case "delta": return Gv.Vec(0, 0);
                    case "button_index": case "keycode": case "physical_keycode": case "unicode": case "button_mask": case "factor": return Gv.I(0);
                }
            }
            return v;
        }
        public static GdEvent Make(string type, params object[] kv)
        {
            var e = new GdEvent(type);
            for (int i = 0; i + 1 < kv.Length; i += 2)
            {
                string k = (string)kv[i]; object v = kv[i + 1]; Gv g;
                if (v is bool) g = Gv.B((bool)v); else if (v is int) g = Gv.I((int)v); else if (v is double) g = Gv.F((double)v);
                else if (v is Gv) g = (Gv)v; else g = Gv.Null;
                e.Set(k, g);
            }
            return e;
        }
    }

    /// <summary>Tween: sekvence kroků (tween_property, tween_callback, tween_interval) s volitelnou paralelitou.</summary>
    public sealed class GdTween : GdObject
    {
        sealed class Step { public double delay, dur, t; public bool started; public Action begin; public Action<double> apply; public Action end; public bool parallelWithPrev; }
        readonly List<Step> steps = new List<Step>();
        bool parallel; public bool Done, Killed; double groupTime;
        public override string ScriptName { get { return "Tween"; } }
        public GdSignal finished = new GdSignal();
        public override Gv Get(string n) { if (n == "finished") return Gv.O(finished); return base.Get(n); }

        public override Gv Invoke(string n, Gv[] a)
        {
            switch (n)
            {
                case "set_parallel": parallel = a.Length == 0 || a[0].T(); return Self;
                case "set_trans": case "set_ease": case "set_loops": case "bind_node": case "set_process_mode": case "pause": case "play": return Self;
                case "kill": Killed = true; Done = true; return Gv.Null;
                case "is_running": return Gv.B(!Done);
                case "is_valid": return Gv.B(!Done);
                case "chain": parallel = false; return Self;
                case "parallel": parallel = true; return Self;
                case "tween_interval": { var s = new Step(); s.dur = Gd.A(a, 0).d; s.parallelWithPrev = parallel && steps.Count > 0; steps.Add(s); return Self; }
                case "tween_callback": { var cb = Gd.A(a, 0); var s = new Step(); s.end = () => cb.Call(); s.parallelWithPrev = parallel && steps.Count > 0; steps.Add(s); return Self; }
                case "tween_property":
                    {
                        var obj = Gd.A(a, 0); string prop = Gd.A(a, 1).Str; var to = Gd.A(a, 2); double dur = Gd.A(a, 3).d;
                        Gv from = default(Gv); string sub = null;
                        int ci = prop.IndexOf(':'); if (ci >= 0) { sub = prop.Substring(ci + 1); prop = prop.Substring(0, ci); }
                        var s = new Step(); s.dur = dur; s.parallelWithPrev = parallel && steps.Count > 0;
                        s.begin = () => { from = obj.G(prop); if (sub != null) from = from.G(sub); };
                        s.apply = t =>
                        {
                            Gv cur = Lerp(from, to, t);
                            if (sub != null) { var whole = obj.G(prop); obj.C("set", Gv.S(prop), whole.S(sub, cur)); }
                            else obj.C("set", Gv.S(prop), cur);
                        };
                        steps.Add(s); return Self;
                    }
            }
            return base.Invoke(n, a);
        }

        static Gv Lerp(Gv a, Gv b, double t)
        {
            if (a.IsNum && b.IsNum) return Gv.F(a.d + (b.d - a.d) * t);
            if (a.k == KK.Vec2 && b.k == KK.Vec2) return Gv.Vec(a.d + (b.d - a.d) * t, a.e + (b.e - a.e) * t);
            if (a.k == KK.Color && b.k == KK.Color) return a.C("lerp", b, Gv.F(t));
            return t >= 1 ? b : a;
        }

        public void Step_(double dt)
        {
            if (Done) return;
            // spustí aktivní skupinu: první nedokončený krok + následující s parallelWithPrev
            int i = 0;
            while (i < steps.Count && steps[i].t >= steps[i].dur && steps[i].started) i++;
            if (i >= steps.Count) { Done = true; finished.Emit(new Gv[0]); return; }
            int j = i;
            do
            {
                var s = steps[j];
                if (!s.started) { s.started = true; if (s.begin != null) s.begin(); }
                s.t += dt;
                double k = s.dur <= 0 ? 1.0 : Math.Min(1.0, s.t / s.dur);
                if (s.apply != null) s.apply(k);
                if (s.t >= s.dur && s.end != null) { var e = s.end; s.end = null; e(); }
                j++;
            } while (j < steps.Count && steps[j].parallelWithPrev);
        }
    }

    public static class GdHost
    {
        public static double Time;
        public static readonly List<GdTween> Tweens = new List<GdTween>();
        public static readonly List<GdNode> TimerNodes = new List<GdNode>();
        static readonly Dictionary<Type, bool[]> cache = new Dictionary<Type, bool[]>();   // [draw, process, physics]

        static bool[] Info(Type t)
        {
            bool[] r;
            if (cache.TryGetValue(t, out r)) return r;
            var f = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            r = new bool[3];
            for (var tt = t; tt != null && tt != typeof(GdNode) && tt != typeof(GdObject); tt = tt.BaseType)
            {
                if (tt.GetMethod("_draw", f) != null) r[0] = true;
                if (tt.GetMethod("_process", f) != null) r[1] = true;
                if (tt.GetMethod("_physics_process", f) != null) r[2] = true;
            }
            cache[t] = r; return r;
        }
        public static bool HasDraw(GdNode n) { return Info(n.GetType())[0]; }
        public static bool HasProcess(GdNode n) { return Info(n.GetType())[1]; }

        public static bool IsVisible(GdNode n)
        {
            for (var p = n; p != null; p = p.Parent)
            {
                var v = p.Get("visible");
                if (v.k == KK.Bool && v.d == 0) return false;
            }
            return true;
        }

        public static void Walk(GdNode n, Action<GdNode> f)
        {
            f(n);
            var ch = n.Children.ToArray();
            foreach (var c in ch) if (!c.QueuedFree) Walk(c, f);
        }

        /// <summary>Jeden snímek: časovače, tweeny, signál process_frame, _process uzlů, překreslení.</summary>
        public static void Frame(GdNode root, double dt)
        {
            Time += dt;
            var tree = Gd.Tree.Obj as GdTree;
            if (tree != null)
            {
                foreach (var t in tree.Timers.ToArray())
                {
                    t.Left -= dt;
                    if (t.Left <= 0) { tree.Timers.Remove(t); t.Sig("timeout").Emit(new Gv[0]); }
                }
                tree.Sig("process_frame").Emit(new Gv[0]);
            }
            foreach (var tw in Tweens.ToArray()) { tw.Step_(dt); if (tw.Done) Tweens.Remove(tw); }
            var dtg = new Gv[] { Gv.F(dt) };
            Walk(root, n =>
            {
                if (n.TypeName == "Timer" && n.Get("running").T())
                {
                    double left = n.Get("left").d - dt;
                    if (left <= 0)
                    {
                        n.Sig("timeout").Emit(new Gv[0]);
                        if (n.Get("one_shot").T()) n.Set("running", Gv.B(false)); else n.Set("left", Gv.F(Math.Max(0.001, n.Get("wait_time").d)));
                    }
                    else n.Set("left", Gv.F(left));
                }
                var inf = Info(n.GetType());
                try
                {
                    if (inf[1]) n.Invoke("_process", dtg);
                    if (inf[2]) n.Invoke("_physics_process", dtg);
                }
                catch (Exception ex) { if (!Gd.Lenient) throw; Gd.Err("_process " + n.ScriptName, ex); }
            });
        }

        /// <summary>Překreslí uzly označené jako špinavé (jen viditelné).</summary>
        public static int RedrawDirty(GdNode root)
        {
            int cnt = 0;
            Walk(root, n =>
            {
                if (n.DrawDirty && Info(n.GetType())[0] && IsVisible(n)) { n.Redraw(); cnt++; }
            });
            return cnt;
        }

        /// <summary>Předá událost uzlům: _input (od posledního), pak _unhandled_input.</summary>
        public static void Dispatch(GdNode root, GdEvent e)
        {
            var all = new List<GdNode>();
            Walk(root, n => all.Add(n));
            var args = new Gv[] { e.Self };
            for (int i = all.Count - 1; i >= 0; i--) { var n = all[i]; if (n.Get("_input_off").T()) continue; n.Invoke("_input", args); }
            for (int i = all.Count - 1; i >= 0; i--)
            {
                var n = all[i];
                n.Invoke("_unhandled_input", args);
                if (e.Type == "InputEventKey") n.Invoke("_unhandled_key_input", args);
            }
        }
    }
}
