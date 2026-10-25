// Objekty běhové knihovny: GdObject (základ všech přeložených skriptů), GdNode (stín uzlů Godotu), GdSignal.
using System;
using System.Collections.Generic;

namespace Pivovar.Gd
{
    public sealed class GdSignal : GdObject
    {
        public readonly List<Gv> targets = new List<Gv>();
        public override string ScriptName { get { return "Signal"; } }
        public void Connect(Gv f) { targets.Add(f); }
        public void Emit(Gv[] a) { foreach (var t in targets.ToArray()) t.Call(a); }
        public override Gv Invoke(string n, Gv[] a)
        {
            switch (n)
            {
                case "connect": Connect(Gd.A(a, 0)); return Gv.Null;
                case "emit": Emit(a); return Gv.Null;
                case "disconnect": { var f = Gd.A(a, 0); if (f.IsNull) targets.Clear(); else targets.RemoveAll(t => ReferenceEquals(t.o, f.o)); return Gv.Null; }
                case "get_connections": { var arr = new GdArray(); foreach (var t in targets) { var d = new GdDict(); d.Set(Gv.S("callable"), t); d.Set(Gv.S("signal"), Self); arr.l.Add(Gv.D(d)); } return Gv.A(arr); }
                case "is_connected": return Gv.B(targets.Count > 0);
            }
            return base.Invoke(n, a);
        }
    }

    /// <summary>Základ všech přeložených skriptů (obdoba Object/RefCounted).</summary>
    public class GdObject
    {
        public Gv Self;
        protected Dictionary<string, Gv> props;
        public GdObject() { Self = Gv.O(this); }

        public virtual string ScriptName { get { return "Object"; } }
        public virtual bool IsA(string t) { return t == "Object" || t == "RefCounted" || t == ScriptName; }

        public virtual Gv Get(string n)
        {
            Gv v;
            if (props != null && props.TryGetValue(n, out v)) return v;
            return Gv.Null;
        }
        public virtual Gv Set(string n, Gv v)
        {
            if (props == null) props = new Dictionary<string, Gv>();
            props[n] = v;
            return v;
        }
        public bool HasProp(string n) { return props != null && props.ContainsKey(n); }
        /// <summary>Čtení vlastnosti bez vedlejších účinků (bez výpočtu rozložení).</summary>
        public Gv Raw(string n) { Gv v; if (props != null && props.TryGetValue(n, out v)) return v; return Gv.Null; }
        public void SetRaw(string n, Gv v) { if (props == null) props = new Dictionary<string, Gv>(); props[n] = v; }

        public virtual Gv Invoke(string n, Gv[] a)
        {
            switch (n)
            {
                case "call": return Gv.Null;
                case "get": return Get(Gd.A(a, 0).Str);
                case "set": Set(Gd.A(a, 0).Str, Gd.A(a, 1)); return Gv.Null;
                case "has_method": return Gv.B(false);
                case "is_instance_valid": return Gv.B(true);
                case "free": case "queue_free": return Gv.Null;
                case "get_class": return Gv.S(ScriptName);
                case "set_meta": Set("meta:" + Gd.A(a, 0).Str, Gd.A(a, 1)); return Gv.Null;
                case "get_meta": { var key = "meta:" + Gd.A(a, 0).Str; return HasProp(key) ? Get(key) : Gd.A(a, 1); }
                case "has_meta": return Gv.B(HasProp("meta:" + Gd.A(a, 0).Str));
                case "connect": return Gv.Null;
                case "emit_signal": return Gv.Null;
                case "get_ref": return Self;
            }
            return Gv.Null;
        }

        public override string ToString() { return "<" + ScriptName + ">"; }
    }

    /// <summary>Plátno, na které uzly kreslí (implementuje Unity vrstva). V testech je null.</summary>
    public interface IGdCanvas
    {
        void Call(GdNode node, string method, Gv[] a);
    }

    /// <summary>Stín uzlu Godotu (Node/Node2D/Control…): strom potomků, signály, kreslení předává plátnu.</summary>
    public class GdNode : GdObject
    {
        public string TypeName = "Node";
        public GdNode Parent;
        public readonly List<GdNode> Children = new List<GdNode>();
        public IGdCanvas Canvas;
        // záznam kreslení (viz GdDraw.cs)
        public List<DrawCmd> Cmds;            // poslední zaznamenané příkazy
        public List<DrawCmd> rec;             // právě zaznamenávané (jen uvnitř _draw)
        public double[] curM;                 // aktuální draw_set_transform
        public bool DrawDirty = true;
        public int DrawVer;
        public bool HasDraw;                  // true, pokud třída definuje _draw
        public TessOut Mesh; public int MeshVer = -1;   // mezipaměť trojúhelníků (viz GdRender)
        public bool InTree;
        public bool Ready;
        public bool QueuedFree;
        readonly Dictionary<string, GdSignal> sigs = new Dictionary<string, GdSignal>();
        // ---- ovládací prvky (viz GdUi*.cs) ----
        public bool PosSet, SizeSet, AnchorSet;       // co nastavil skript (rozlišení režimu rozložení)
        public int LayoutVer = -1, UiVer, UiMeshKey = -1, PostMeshKey = -1;
        public TessOut UiMesh, PostMesh;
        public bool Hover, Down, Focused;
        public double PW, PH;                          // velikost rodiče při posledním rozložení (pro offsety)
        public double MinW, MinH; public int MinPass = -1;   // mezipaměť minimální velikosti
        public double ScrollV, ScrollH, MaxScrollV, MaxScrollH, ContentH, ContentW, WrapUsed;
        public object RichCache; public string RichKey;
        public bool IsCtl { get { return GdUi.IsControlType(TypeName); } }
        public GdNode SubCam;                           // aktuální kamera SubViewportu (Camera2D.make_current uvnitř SubViewportu)

        public override string ScriptName { get { return TypeName; } }
        public override bool IsA(string t) { return t == TypeName || t == "Node" || t == "CanvasItem" || t == "Object" || t == "Node2D" || t == "Control" || base.IsA(t); }

        public GdSignal Sig(string n)
        {
            GdSignal s; if (!sigs.TryGetValue(n, out s)) { s = new GdSignal(); sigs[n] = s; }
            return s;
        }

        public virtual void OnEnterTree() { }

        /// <summary>Spustí _draw a uloží příkazy.</summary>
        public void Redraw()
        {
            rec = new List<DrawCmd>(); curM = null;
            try { Invoke("_draw", new Gv[0]); }
            catch (Exception ex) { if (!Gd.Lenient) throw; Gd.Err("_draw " + ScriptName, ex); }
            finally { Cmds = rec; rec = null; curM = null; DrawDirty = false; DrawVer++; }
        }

        public void EnterTree()
        {
            if (InTree) return;
            InTree = true;
            foreach (var c in Children.ToArray()) c.EnterTree();
            if (!Ready)
            {
                Ready = true;
                try { Invoke("_ready", new Gv[0]); }
                catch (Exception ex) { if (!Gd.Lenient) throw; Gd.Err("_ready " + ScriptName, ex); }
                if (TypeName == "Timer" && Get("autostart").T()) Invoke("start", new Gv[0]);
            }
        }

        static readonly HashSet<string> SigNames = new HashSet<string> {
            "process_frame", "physics_frame", "timeout", "pressed", "toggled", "button_down", "button_up", "visibility_changed", "resized", "size_changed",
            "gui_input", "item_selected", "item_activated", "text_changed", "text_submitted", "text_set", "value_changed", "mouse_entered", "mouse_exited",
            "confirmed", "canceled", "request_completed", "finished", "focus_entered", "focus_exited", "tree_entered", "tree_exited", "tree_exiting",
            "child_entered_tree", "child_exiting_tree", "draw", "hidden", "ready", "renamed", "meta_clicked", "meta_hover_started", "meta_hover_ended",
            "id_pressed", "index_pressed", "popup_hide", "about_to_popup", "close_requested", "files_dropped", "drag_started", "drag_ended", "color_changed",
            "scrolling", "scroll_started", "scroll_ended", "item_rect_changed", "minimum_size_changed", "theme_changed", "sort_children", "input_event" };

        public static bool IsSignalName(string n) { return SigNames.Contains(n); }

        public override Gv Get(string n)
        {
            var v = base.Get(n);
            if (v.IsNull && SigNames.Contains(n)) return Gv.O(Sig(n));
            if (v.IsNull && (n == "modulate" || n == "self_modulate")) return Gv.Col(1, 1, 1, 1);
            if (GdUi.Active && GdUi.IsControlType(TypeName)) return GdUi.GetCtl(this, n, v);
            return v;
        }
        public override Gv Set(string n, Gv v)
        {
            if (GdUi.Active && GdUi.IsControlType(TypeName)) { var old = Raw(n); base.Set(n, v); GdUi.OnSet(this, n, v, old); if (n.StartsWith("theme") || n.StartsWith("ovr:") || n == "default_font_size" || n == "default_font") GdUi.ThemeVer++; return v; }
            if (n.StartsWith("theme") || n.StartsWith("ovr:") || n == "default_font_size" || n == "default_font") { GdUi.Ver++; GdUi.ThemeVer++; }
            else if (TypeName.StartsWith("StyleBox")) GdUi.ThemeVer++;
            return base.Set(n, v);
        }

        public override Gv Invoke(string n, Gv[] a)
        {
            if (GdUi.Active && GdUi.IsControlType(TypeName)) { Gv cr; if (GdUi.InvokeCtl(this, n, a, out cr)) return cr; }
            switch (n)
            {
                case "add_child":
                    {
                        var c = Gd.A(a, 0).Obj as GdNode;
                        if (c != null) { if (c.Parent != null) c.Parent.Children.Remove(c); c.Parent = this; Children.Add(c); c.Canvas = Canvas; GdUi.Ver++; if (InTree) c.EnterTree(); }
                        return Gv.Null;
                    }
                case "remove_child": { var c = Gd.A(a, 0).Obj as GdNode; if (c != null) { Children.Remove(c); c.Parent = null; GdUi.Ver++; } return Gv.Null; }
                case "queue_free": case "free": QueuedFree = true; if (Parent != null) Parent.Children.Remove(this); Parent = null; GdUi.Ver++; return Gv.Null;
                case "get_children": { var l = new List<Gv>(); foreach (var c in Children) l.Add(c.Self); return Gv.A(new GdArray(l)); }
                case "get_child": { int i = (int)Gd.A(a, 0).d; return i >= 0 && i < Children.Count ? Children[i].Self : Gv.Null; }
                case "get_child_count": return Gv.I(Children.Count);
                case "get_parent": return Parent != null ? Parent.Self : Gv.Null;
                case "is_inside_tree": return Gv.B(InTree);
                case "get_tree": return Gd.Tree;
                case "get_viewport": return Gd.Viewport;
                case "get_window": return Gd.Viewport;
                case "get_visible_rect": case "get_viewport_rect": return Gv.Rect(0, 0, Gd.ScreenW, Gd.ScreenH);
                case "get_process_delta_time": return Gv.F(Gd.Delta);
                case "queue_redraw": case "update": DrawDirty = true; return Gv.Null;
                case "queue_sort": return Gv.Null;
                case "connect":
                    {
                        var sv = Get(Gd.A(a, 0).Str).o as GdSignal;
                        if (sv != null) sv.Connect(Gd.A(a, 1));
                        return Gv.Null;
                    }
                case "emit_signal":
                    {
                        var sv = Get(Gd.A(a, 0).Str).o as GdSignal;
                        if (sv != null) sv.Emit(Slice(a, 1));
                        return Gv.Null;
                    }
                case "create_tween": { var t = new GdTween(); GdHost.Tweens.Add(t); return t.Self; }
                case "start":
                    if (TypeName == "Timer") { Set("running", Gv.B(true)); Set("left", Gv.F(a.Length > 0 && a[0].IsNum && a[0].d > 0 ? a[0].d : Math.Max(0.001, Get("wait_time").d))); return Gv.Null; }
                    break;
                case "stop":
                    if (TypeName == "Timer") { Set("running", Gv.B(false)); return Gv.Null; }
                    break;
                case "duplicate":
                    {
                        var d = new GdNode(); d.TypeName = TypeName;
                        if (props != null) foreach (var kv in props) d.Set(kv.Key, kv.Value);
                        return d.Self;
                    }
                case "set_corner_radius_all": { var v = Gd.A(a, 0); foreach (var k in new[] { "top_left", "top_right", "bottom_right", "bottom_left" }) Set("corner_radius_" + k, v); return Gv.Null; }
                case "set_border_width_all": { var v = Gd.A(a, 0); foreach (var k in new[] { "left", "top", "right", "bottom" }) Set("border_width_" + k, v); return Gv.Null; }
                case "set_content_margin_all": { var v = Gd.A(a, 0); foreach (var k in new[] { "left", "top", "right", "bottom" }) Set("content_margin_" + k, v); return Gv.Null; }
                case "set_expand_margin_all": { var v = Gd.A(a, 0); foreach (var k in new[] { "left", "top", "right", "bottom" }) Set("expand_margin_" + k, v); return Gv.Null; }
                case "set_stylebox": case "set_color": case "set_font_size": case "set_constant": case "set_font": case "set_icon":
                    Set("theme:" + Gd.A(a, 1).ToStr() + "/" + Gd.A(a, 0).ToStr(), Gd.A(a, 2)); return Gv.Null;
                case "set_pressed_no_signal": Set("button_pressed", Gd.A(a, 0)); return Gv.Null;
                case "set_process": case "set_physics_process": case "set_process_input": case "set_process_unhandled_input": case "set_process_unhandled_key_input": return Gv.Null;
                case "show": Set("visible", Gv.B(true)); return Gv.Null;
                case "hide": Set("visible", Gv.B(false)); return Gv.Null;
                case "is_visible_in_tree": return Gv.B(!HasProp("visible") || Get("visible").T());
                case "get_global_mouse_position": return GdRender.ScreenToCanvas(this, Gd.MousePos);
                case "get_local_mouse_position": return GdRender.ScreenToLocal(this, Gd.MousePos);
                case "get_node": case "get_node_or_null": case "find_child": return Gv.Null;
                case "make_current": if (TypeName == "Camera2D") { if (Parent != null && Parent.TypeName == "SubViewport") Parent.SubCam = this; else GdRender.CurrentCam = this; } return Gv.Null;
                case "move_child":
                    {
                        var c = Gd.A(a, 0).Obj as GdNode; int i = (int)Gd.A(a, 1).d;
                        if (c != null && Children.Remove(c)) { if (i < 0) i += Children.Count + 1; Children.Insert(Math.Max(0, Math.Min(i, Children.Count)), c); }
                        return Gv.Null;
                    }
                case "add_theme_stylebox_override": Set("ovr:stylebox/" + Gd.A(a, 0).ToStr(), Gd.A(a, 1)); return Gv.Null;
                case "add_theme_color_override": Set("ovr:color/" + Gd.A(a, 0).ToStr(), Gd.A(a, 1)); return Gv.Null;
                case "add_theme_font_size_override": Set("ovr:font_size/" + Gd.A(a, 0).ToStr(), Gd.A(a, 1)); return Gv.Null;
                case "add_theme_font_override": Set("ovr:font/" + Gd.A(a, 0).ToStr(), Gd.A(a, 1)); return Gv.Null;
                case "add_theme_constant_override": Set("ovr:constant/" + Gd.A(a, 0).ToStr(), Gd.A(a, 1)); return Gv.Null;
                case "set_deferred": Set(Gd.A(a, 0).ToStr(), Gd.A(a, 1)); return Gv.Null;
                case "call_deferred": { var nm = Gd.A(a, 0).ToStr(); return Invoke(nm, Slice(a, 1)); }
                case "set_size": Set("size", Gd.A(a, 0)); return Gv.Null;
                case "set_position": Set("position", Gd.A(a, 0)); return Gv.Null;
                case "accept_event": Gd.InputHandled = true; GdUi.Accepted = true; return Gv.Null;
                case "reparent": case "add_to_group": return Gv.Null;
                case "get_size": return Get("size");
                case "get_rect": { var p = Get("position"); var s = Get("size"); return Gv.Rect(p.d, p.e, s.d, s.e); }
                case "get_global_rect": { var p = Get("position"); var s = Get("size"); return Gv.Rect(p.d, p.e, s.d, s.e); }
                case "get_theme_default_font": return Gv.Null;
            }
            if (n.StartsWith("draw_")) { GdDrawRec.Record(this, n, a); return Gv.Null; }
            if (n == "get_string_size") return Gd.StringSize(Gd.A(a, 1), Gd.A(a, 3));
            if (n.StartsWith("get_theme_")) return Gv.Null;
            return base.Invoke(n, a);
        }

        public static Gv[] Slice(Gv[] a, int from)
        {
            if (a.Length <= from) return new Gv[0];
            var r = new Gv[a.Length - from]; Array.Copy(a, from, r, 0, r.Length); return r;
        }
    }

    /// <summary>Okno/výřez: velikost obrazovky nastavuje hostitel (Unity) přes Gd.ScreenW/H.</summary>
    public sealed class GdViewport : GdNode
    {
        public GdViewport() { TypeName = "Viewport"; }
        public override Gv Get(string n)
        {
            if (n == "size") return Gv.Vec(Gd.ScreenW, Gd.ScreenH);
            return base.Get(n);
        }
        public override Gv Invoke(string n, Gv[] a)
        {
            switch (n)
            {
                case "get_visible_rect": return Gv.Rect(0, 0, Gd.ScreenW, Gd.ScreenH);
                case "get_mouse_position": return Gd.MousePos;
                case "set_input_as_handled": Gd.InputHandled = true; return Gv.Null;
                case "is_input_handled": return Gv.B(Gd.InputHandled);
                case "get_texture": return Gd.Load("res://__screen.png");
                case "warp_mouse": return Gv.Null;
            }
            return base.Invoke(n, a);
        }
    }

    public sealed class GdSceneTimer : GdNode
    {
        public double Left;
        public GdSceneTimer() { TypeName = "SceneTreeTimer"; }
        public override Gv Get(string n) { if (n == "time_left") return Gv.F(Left); return base.Get(n); }
    }

    /// <summary>Strom scény (get_tree()): signál process_frame, časovače.</summary>
    public sealed class GdTree : GdNode
    {
        public readonly List<GdSceneTimer> Timers = new List<GdSceneTimer>();
        public GdTree() { TypeName = "SceneTree"; }
        public override Gv Invoke(string n, Gv[] a)
        {
            switch (n)
            {
                case "create_timer": { var t = new GdSceneTimer(); t.Left = Gd.A(a, 0).d; Timers.Add(t); return t.Self; }
                case "create_tween": { var t = new GdTween(); GdHost.Tweens.Add(t); return t.Self; }
                case "quit": Gd.QuitCode = a.Length > 0 ? (int)a[0].d : 0; Gd.Quit = true; return Gv.Null;
                case "get_nodes_in_group": return Gv.A(new GdArray());
                case "change_scene_to_file": return Gv.Null;
            }
            return base.Invoke(n, a);
        }
    }
}
