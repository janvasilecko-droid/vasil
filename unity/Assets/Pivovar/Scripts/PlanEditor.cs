// Editor plánu přímo v Unity (za běhu, klávesa TAB): výběr a posun trubek, bodů trubek, klapek, čerpadel a nádob,
// číselné úpravy, nová trubka, přidání / smazání dílu, zpět (Ctrl+Z) a uložení do stejného JSONu jako 2D verze.
// Souřadnice jsou v px plánu (1 px = 5 mm). Výběr se dělá v obrazovkových souřadnicích, takže funguje z libovolného pohledu;
// tlačítko "Pohled zepředu" ale dává nejpřesnější polohu.
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using Pivovar.Core;

namespace Pivovar
{
    public class PlanEditor : MonoBehaviour
    {
        /// <summary>True, když uživatel píše do textového pole (ať kamera a klávesy F/X nereagují).</summary>
        public static bool Typing;
        public static bool Active;

        enum Mode { Select, NewPipe }

        BreweryBuilder b;
        bool on;
        Mode mode = Mode.Select;
        PlanItem sel;
        int selPt = -1;
        readonly List<double[]> draw = new List<double[]>();
        readonly List<string> undo = new List<string>();
        string lastUndoKey = ""; float lastUndoTime;
        float grid = 5f;
        bool snapPipes = true;
        string status = "";
        bool pendingRebuild;

        // tažení
        bool pressing, dragging;
        Vector2 pressMouse;
        Vector2 grab;                 // posun mezi místem kliknutí a počátkem dílu (px plánu)
        bool wholePipe;
        List<double[]> pipeStart;     // body trubky na začátku tažení (pro posun celé trubky)
        Vector2 pressPlan;

        readonly Dictionary<string, string> buf = new Dictionary<string, string>();
        Vector2 scroll;
        Rect panelRect;               // v souřadnicích GUI po škálování
        float uiScale = 1f;

        public void Init(BreweryBuilder builder) { b = builder; }

        // ───────────────────────── pomocné ─────────────────────────
        Camera Cam { get { return b != null ? b.Cam : null; } }

        Vector2 Scr(double px, double py, float zGodot, out bool visible)
        {
            Vector3 s = Cam.WorldToScreenPoint(b.FromPlan(px, py, zGodot));
            visible = s.z > 0.01f;
            return new Vector2(s.x, Screen.height - s.y);
        }

        Vector2 Scr(double px, double py, float zGodot) { bool v; return Scr(px, py, zGodot, out v); }

        bool RayPlan(Vector2 mp, float zGodot, out Vector2 plan)
        {
            plan = Vector2.zero;
            Ray r = Cam.ScreenPointToRay(new Vector3(mp.x, Screen.height - mp.y, 0f));
            if (Mathf.Abs(r.direction.z) < 1e-5f) return false;
            float t = (-zGodot - r.origin.z) / r.direction.z;
            if (t < 0f || t > 120f) return false;
            plan = b.ToPlan(r.origin + r.direction * t);
            return InRange(plan.x, plan.y);
        }

        // rozumný rozsah plánu v px (plán má x -1322…7410, y -25…1660); mimo něj je to chyba tažení, ne úprava
        static bool InRange(double x, double y) { return x > -2500 && x < 9000 && y > -600 && y < 2600; }

        float ZOf(PlanItem it)
        {
            if (it.Type == "vessel") { Vessel v = b.VesselOf(it); return v != null ? v.z : 0f; }
            if (it.Type == "deco") return 0.2f;
            return BreweryBuilder.ZP;
        }

        static string Fmt(double v) { return v.ToString("0.##", CultureInfo.InvariantCulture); }

        static bool IsPointItem(PlanItem it)
        {
            switch (it.Type)
            {
                case "valve": case "pump": case "deco": case "nozzle": case "inl": case "flow": case "spray":
                case "hopf": case "fitting": case "sw": case "manifold": return true;
            }
            return false;
        }

        static bool SnapsToPipe(PlanItem it)
        {
            return it.Type == "valve" || it.Type == "pump" || it.Type == "inl" || it.Type == "flow" || it.Type == "hopf" || it.Type == "fitting";
        }

        bool Protected(PlanItem it)
        {
            if (it.Type == "vessel") return true;
            object o;
            return it.D.TryGetValue("bind", out o) && o is string && ((string)o).Length > 0;
        }

        // ───────────────────────── zpět / uložení ─────────────────────────
        void PushUndo(string key)
        {
            if (key.Length > 0 && key == lastUndoKey && Time.unscaledTime - lastUndoTime < 1.5f) { lastUndoTime = Time.unscaledTime; return; }
            lastUndoKey = key; lastUndoTime = Time.unscaledTime;
            undo.Add(b.plan.ToJson());
            if (undo.Count > 60) undo.RemoveAt(0);
        }

        void Undo()
        {
            if (undo.Count == 0) { status = "Není co vrátit."; return; }
            string id = sel != null ? sel.Id : null;
            string snap = undo[undo.Count - 1]; undo.RemoveAt(undo.Count - 1);
            b.plan = Plan.Parse(snap);
            sel = id != null ? b.plan.ById(id) : null;
            if (sel == null) selPt = -1;
            lastUndoKey = "";
            b.Rebuild();
            status = "Vráceno zpět.";
        }

        string FindOutOfRange()
        {
            foreach (PlanItem it in b.plan.Items)
            {
                if (it.Type == "pipe")
                {
                    foreach (double[] pt in it.Pts()) if (!InRange(pt[0], pt[1])) return "trubka " + it.Id;
                }
                else if (it.D.ContainsKey("x") && it.D.ContainsKey("y") && !InRange(it.X, it.Y)) return it.Type + " " + it.Id;
            }
            return null;
        }

        void Save()
        {
            try
            {
                string path = b.planPath;
                string dir = Path.GetDirectoryName(path);
                string bak = Path.Combine(dir, "plan.zaloha.json");
                if (!File.Exists(bak)) File.Copy(path, bak);
                string bad = FindOutOfRange();
                if (bad != null) { status = "Neuloženo: " + bad + " je mimo plán. Vrať to zpět (Ctrl+Z)."; return; }
                string json = b.plan.ToJson();
                var enc = new UTF8Encoding(false);
                File.WriteAllText(path, json, enc);
                File.WriteAllText(Path.Combine(dir, "plan_upravy.json"), json, enc);
                status = "Uloženo: plan.json + plan_upravy.json (importuj v 2D editoru).";
            }
            catch (System.Exception e) { status = "Uložení selhalo: " + e.Message; }
        }

        // ───────────────────────── změny ─────────────────────────
        void Changed(PlanItem it, bool heavy)
        {
            if (it == null) return;
            if (it.Type == "pipe") b.RebuildPipe(it);
            else if (!BreweryBuilder.IsLight(it) || heavy) pendingRebuild = true;
            else b.MovePart(it);
        }

        Vector2 SnapPipePoint(Vector2 p, PlanItem self, int selfIdx, bool noSnap)
        {
            if (noSnap) return p;
            if (snapPipes)
            {
                double best = 12; double[] hit = null;
                foreach (PlanItem q in b.plan.OfType("pipe"))
                {
                    List<double[]> pts = q.Pts();
                    for (int i = 0; i < pts.Count; i++)
                    {
                        if (q == self && i == selfIdx) continue;
                        double d = Mathf.Sqrt((float)((pts[i][0] - p.x) * (pts[i][0] - p.x) + (pts[i][1] - p.y) * (pts[i][1] - p.y)));
                        if (d < best) { best = d; hit = pts[i]; }
                    }
                }
                if (hit != null) return new Vector2((float)hit[0], (float)hit[1]);
            }
            return Grid(p);
        }

        Vector2 Grid(Vector2 p)
        {
            if (grid <= 0f) return p;
            return new Vector2(Mathf.Round(p.x / grid) * grid, Mathf.Round(p.y / grid) * grid);
        }

        /// <summary>Nejbližší bod na nějaké trubce (pro přichycení klapky a čerpadla).</summary>
        bool NearestOnPipes(Vector2 p, double tol, out Vector2 res)
        {
            res = p; double best = tol; bool found = false;
            foreach (PlanItem q in b.plan.OfType("pipe"))
            {
                if (q.IsHose) continue;
                List<double[]> pts = q.Pts();
                for (int i = 0; i + 1 < pts.Count; i++)
                {
                    Vector2 a = new Vector2((float)pts[i][0], (float)pts[i][1]);
                    Vector2 c = new Vector2((float)pts[i + 1][0], (float)pts[i + 1][1]);
                    Vector2 ac = c - a;
                    float l2 = ac.sqrMagnitude;
                    float t = l2 < 1e-6f ? 0f : Mathf.Clamp01(Vector2.Dot(p - a, ac) / l2);
                    Vector2 pr = a + ac * t;
                    float d = (pr - p).magnitude;
                    if (d < best) { best = d; res = pr; found = true; }
                }
            }
            return found;
        }

        // ───────────────────────── výběr ─────────────────────────
        static float SegDist(Vector2 p, Vector2 a, Vector2 c, out float t)
        {
            Vector2 ac = c - a;
            float l2 = ac.sqrMagnitude;
            t = l2 < 1e-6f ? 0f : Mathf.Clamp01(Vector2.Dot(p - a, ac) / l2);
            return (a + ac * t - p).magnitude;
        }

        bool PickPoint(Vector2 mp, PlanItem pipe, out int idx)
        {
            idx = -1;
            List<double[]> pts = pipe.Pts();
            float best = 12f * uiScale;
            for (int i = 0; i < pts.Count; i++)
            {
                bool vis; Vector2 s = Scr(pts[i][0], pts[i][1], BreweryBuilder.ZP, out vis);
                if (!vis) continue;
                float d = (s - mp).magnitude;
                if (d < best) { best = d; idx = i; }
            }
            return idx >= 0;
        }

        void Pick(Vector2 mp, out PlanItem item, out int pt)
        {
            item = null; pt = -1;
            int idx;
            // 1) body vybrané trubky
            if (sel != null && sel.Type == "pipe" && PickPoint(mp, sel, out idx)) { item = sel; pt = idx; return; }
            // 2) klapky, čerpadla, dekorace
            float bestD = 24f * uiScale; PlanItem bi = null;
            foreach (PlanItem it in b.plan.Items)
            {
                if (!IsPointItem(it)) continue;
                bool vis; Vector2 s = Scr(it.X, it.Y, ZOf(it), out vis);
                if (!vis) continue;
                float d = (s - mp).magnitude;
                if (d < bestD) { bestD = d; bi = it; }
            }
            if (bi != null) { item = bi; return; }
            // 3) body ostatních trubek
            foreach (PlanItem it in b.plan.OfType("pipe"))
                if (PickPoint(mp, it, out idx)) { item = it; pt = idx; return; }
            // 4) úseky trubek
            float bestS = 8f * uiScale; PlanItem bp = null;
            foreach (PlanItem it in b.plan.OfType("pipe"))
            {
                List<double[]> pts = it.Pts();
                for (int i = 0; i + 1 < pts.Count; i++)
                {
                    bool v1, v2;
                    Vector2 a = Scr(pts[i][0], pts[i][1], BreweryBuilder.ZP, out v1);
                    Vector2 c = Scr(pts[i + 1][0], pts[i + 1][1], BreweryBuilder.ZP, out v2);
                    if (!v1 || !v2) continue;
                    float t; float d = SegDist(mp, a, c, out t);
                    if (d < bestS) { bestS = d; bp = it; }
                }
            }
            if (bp != null) { item = bp; return; }
            // 5) nádoby (paprskem do roviny nádoby)
            foreach (PlanItem it in b.plan.OfType("vessel"))
            {
                Vector2 p;
                if (!RayPlan(mp, ZOf(it), out p)) continue;
                if (p.x >= it.X && p.x <= it.X + it.W && p.y >= it.Y && p.y <= it.Y + it.H) { item = it; return; }
            }
        }

        // ───────────────────────── vstup ─────────────────────────
        void Toggle()
        {
            on = !on; Active = on;
            if (!on) { mode = Mode.Select; draw.Clear(); Typing = false; }
            status = on ? "Editor zapnut. Klik = výběr, tažení = posun, Shift+klik na úsek trubky = nový bod." : "";
        }

        void Update()
        {
            if (pendingRebuild && !Typing && !Inp.Left())
            {
                pendingRebuild = false;
                string id = sel != null ? sel.Id : null;
                b.Rebuild();
                sel = id != null ? b.plan.ById(id) : null;
            }
        }

        void OnGUI()
        {
            if (b == null || b.plan == null || Cam == null) return;
            Event e = Event.current;

            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Tab && !Typing) { Toggle(); e.Use(); return; }
            if (!on) return;
            if (pressing && !Inp.Left() && e.type != EventType.MouseUp) { pressing = false; dragging = false; }

            uiScale = Mathf.Clamp(Screen.height / 900f, 1f, 3f);
            if (e.type == EventType.Repaint) DrawOverlay();

            DrawPanel();
            Typing = GUI.GetNameOfFocusedControl().StartsWith("f_");

            if (e.type == EventType.Used) return;
            Vector2 mp = e.mousePosition;
            bool overPanel = panelRect.Contains(mp / uiScale);

            if (e.type == EventType.KeyDown && !Typing) HandleKey(e);
            else if (e.type == EventType.MouseDown && e.button == 0 && !overPanel) { OnMouseDown(e, mp); e.Use(); }
            else if (e.type == EventType.MouseDrag && e.button == 0 && pressing) { OnMouseDrag(e, mp); e.Use(); }
            else if (e.type == EventType.MouseUp && e.button == 0 && pressing) { OnMouseUp(mp); e.Use(); }
        }

        void OnMouseDown(Event e, Vector2 mp)
        {
            GUI.FocusControl(null);
            Vector2 plan;
            if (mode == Mode.NewPipe)
            {
                if (!RayPlan(mp, BreweryBuilder.ZP, out plan)) return;
                if (draw.Count >= 2)
                {
                    bool vis; Vector2 last = Scr(draw[draw.Count - 1][0], draw[draw.Count - 1][1], BreweryBuilder.ZP, out vis);
                    if ((last - mp).magnitude < 10f * uiScale) { FinishPipe(); return; }
                }
                Vector2 q = SnapPipePoint(plan, null, -1, e.alt);
                if (e.shift && draw.Count > 0) q = AxisAlign(q, new Vector2((float)draw[draw.Count - 1][0], (float)draw[draw.Count - 1][1]));
                draw.Add(new double[] { q.x, q.y });
                return;
            }

            PlanItem it; int pt;
            Pick(mp, out it, out pt);
            pressing = true; dragging = false; pressMouse = mp; wholePipe = false;

            // Shift+klik na úsek vybrané trubky = vložit bod
            if (e.shift && sel != null && sel.Type == "pipe" && it == sel && pt < 0)
            {
                if (InsertPoint(mp)) { pressing = false; return; }
            }
            sel = it; selPt = pt;
            if (it == null) { pressing = false; return; }

            if (RayPlan(mp, ZOf(it), out plan))
            {
                pressPlan = plan;
                if (it.Type == "pipe")
                {
                    List<double[]> pts = it.Pts();
                    if (pt >= 0) grab = new Vector2((float)pts[pt][0] - plan.x, (float)pts[pt][1] - plan.y);
                    else { wholePipe = true; grab = Vector2.zero; }
                    pipeStart = pts;
                }
                else grab = new Vector2((float)it.X - plan.x, (float)it.Y - plan.y);
            }
        }

        Vector2 AxisAlign(Vector2 p, Vector2 refPt)
        {
            if (Mathf.Abs(p.x - refPt.x) < Mathf.Abs(p.y - refPt.y)) return new Vector2(refPt.x, p.y);
            return new Vector2(p.x, refPt.y);
        }

        void OnMouseDrag(Event e, Vector2 mp)
        {
            if (sel == null) return;
            if (!dragging)
            {
                if ((mp - pressMouse).magnitude < 4f) return;
                dragging = true;
                PushUndo("");
            }
            Vector2 plan;
            if (!RayPlan(mp, ZOf(sel), out plan)) return;

            if (sel.Type == "pipe")
            {
                if (wholePipe)
                {
                    Vector2 d = Grid(plan - pressPlan);
                    if (e.alt) d = plan - pressPlan;
                    var np = new List<double[]>();
                    foreach (double[] p in pipeStart) np.Add(new double[] { p[0] + d.x, p[1] + d.y });
                    sel.SetPts(np);
                }
                else if (selPt >= 0)
                {
                    List<double[]> pts = sel.Pts();
                    Vector2 q = SnapPipePoint(plan + grab, sel, selPt, e.alt);
                    if (e.shift)
                    {
                        int nb = selPt > 0 ? selPt - 1 : selPt + 1;
                        if (nb >= 0 && nb < pts.Count) q = AxisAlign(q, new Vector2((float)pts[nb][0], (float)pts[nb][1]));
                    }
                    pts[selPt][0] = q.x; pts[selPt][1] = q.y;
                    sel.SetPts(pts);
                }
                b.RebuildPipe(sel);
            }
            else
            {
                Vector2 q = plan + grab;
                if (SnapsToPipe(sel) && snapPipes && !e.alt)
                {
                    Vector2 on2;
                    if (NearestOnPipes(q, 14, out on2)) q = on2; else q = Grid(q);
                }
                else if (!e.alt) q = Grid(q);
                sel.SetNum("x", q.x); sel.SetNum("y", q.y);
                if (BreweryBuilder.IsLight(sel)) b.MovePart(sel);
            }
        }

        void OnMouseUp(Vector2 mp)
        {
            pressing = false;
            if (dragging && sel != null && sel.Type != "pipe" && !BreweryBuilder.IsLight(sel)) pendingRebuild = true;
            dragging = false;
        }

        bool InsertPoint(Vector2 mp)
        {
            List<double[]> pts = sel.Pts();
            float best = 12f * uiScale; int seg = -1; float bt = 0f;
            for (int i = 0; i + 1 < pts.Count; i++)
            {
                bool v1, v2;
                Vector2 a = Scr(pts[i][0], pts[i][1], BreweryBuilder.ZP, out v1);
                Vector2 c = Scr(pts[i + 1][0], pts[i + 1][1], BreweryBuilder.ZP, out v2);
                if (!v1 || !v2) continue;
                float t; float d = SegDist(mp, a, c, out t);
                if (d < best) { best = d; seg = i; bt = t; }
            }
            if (seg < 0) return false;
            PushUndo("");
            double x = pts[seg][0] + (pts[seg + 1][0] - pts[seg][0]) * bt;
            double y = pts[seg][1] + (pts[seg + 1][1] - pts[seg][1]) * bt;
            Vector2 g = Grid(new Vector2((float)x, (float)y));
            pts.Insert(seg + 1, new double[] { g.x, g.y });
            sel.SetPts(pts);
            selPt = seg + 1;
            b.RebuildPipe(sel);
            status = "Vložen bod " + (seg + 1) + ".";
            return true;
        }

        void FinishPipe()
        {
            if (draw.Count >= 2)
            {
                PushUndo("");
                var d = new Dictionary<string, object>();
                d["type"] = "pipe";
                d["id"] = "pipe_ed" + System.DateTime.Now.Ticks % 100000000;
                d["dn"] = 50.0; d["medium"] = "water"; d["label"] = "";
                var item = new PlanItem(d);
                item.SetPts(draw);
                b.plan.AddItem(d);
                sel = b.plan.ById((string)d["id"]); selPt = -1;
                b.RebuildPipe(sel);
                status = "Trubka vytvořena. Médium a průměr nastavíš v panelu.";
            }
            draw.Clear();
            mode = Mode.Select;
        }

        void HandleKey(Event e)
        {
            KeyCode k = e.keyCode;
            if (e.control && k == KeyCode.Z) { Undo(); e.Use(); return; }
            if (e.control && k == KeyCode.S) { Save(); e.Use(); return; }
            if (k == KeyCode.Escape) { draw.Clear(); mode = Mode.Select; sel = null; selPt = -1; e.Use(); return; }
            if (mode == Mode.NewPipe && (k == KeyCode.Return || k == KeyCode.KeypadEnter)) { FinishPipe(); e.Use(); return; }
            if (sel == null) return;
            if (k == KeyCode.Delete || k == KeyCode.Backspace) { DeleteSel(); e.Use(); return; }
            if (e.control && k == KeyCode.D) { Duplicate(); e.Use(); return; }
            if (k == KeyCode.R && (sel.Type == "valve" || sel.Type == "pump" || sel.Type == "inl" || sel.Type == "flow" || sel.Type == "nozzle")) { RotateSel(); e.Use(); return; }
            float step = e.shift ? 10f : 1f;
            Vector2 dv = Vector2.zero;
            if (k == KeyCode.LeftArrow) dv.x = -step; else if (k == KeyCode.RightArrow) dv.x = step;
            else if (k == KeyCode.UpArrow) dv.y = -step; else if (k == KeyCode.DownArrow) dv.y = step;
            if (dv != Vector2.zero) { Nudge(dv); e.Use(); }
        }

        void Nudge(Vector2 dv)
        {
            PushUndo("nudge");
            if (sel.Type == "pipe")
            {
                List<double[]> pts = sel.Pts();
                if (selPt >= 0) { pts[selPt][0] += dv.x; pts[selPt][1] += dv.y; }
                else foreach (double[] p in pts) { p[0] += dv.x; p[1] += dv.y; }
                sel.SetPts(pts);
                b.RebuildPipe(sel);
            }
            else
            {
                sel.SetNum("x", sel.X + dv.x); sel.SetNum("y", sel.Y + dv.y);
                Changed(sel, false);
            }
        }

        void RotateSel()
        {
            PushUndo("");
            string key = (sel.Type == "pump" || sel.Type == "nozzle") ? "dir" : "rot";
            double v = sel.Num(key, 0) + 90;
            if (v > 180) v -= 360;
            sel.SetNum(key, v);
            Changed(sel, false);
        }

        void DeleteSel()
        {
            if (sel.Type == "pipe" && selPt >= 0)
            {
                List<double[]> pts = sel.Pts();
                if (pts.Count > 2)
                {
                    PushUndo("");
                    pts.RemoveAt(selPt);
                    sel.SetPts(pts); selPt = -1;
                    b.RebuildPipe(sel);
                    status = "Bod smazán.";
                    return;
                }
            }
            if (Protected(sel)) { status = "Tenhle díl je navázaný na simulaci – nejde smazat."; return; }
            PushUndo("");
            b.RemovePart(sel);
            b.plan.RemoveItem(sel);
            sel = null; selPt = -1;
            status = "Díl smazán.";
        }

        void Duplicate()
        {
            if (Protected(sel)) { status = "Nádobu / čerpadlo s vazbou na simulaci nelze kopírovat."; return; }
            PushUndo("");
            string json = MiniJson.Write(sel.D, false);
            var d = (Dictionary<string, object>)MiniJson.Parse(json);
            d["id"] = sel.Type + "_ed" + System.DateTime.Now.Ticks % 100000000;
            if (sel.Type == "pipe")
            {
                var it0 = new PlanItem(d);
                List<double[]> pts = it0.Pts();
                foreach (double[] p in pts) { p[0] += 20; p[1] += 20; }
                it0.SetPts(pts);
            }
            else { d["x"] = sel.X + 20; d["y"] = sel.Y + 20; }
            b.plan.AddItem(d);
            sel = b.plan.ById((string)d["id"]); selPt = -1;
            if (sel.Type == "pipe") b.RebuildPipe(sel); else b.BuildPart(sel);
            status = "Kopie vytvořena (posunutá o 20 px).";
        }

        void AddValve()
        {
            Vector2 c;
            if (!RayPlan(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f), BreweryBuilder.ZP, out c)) return;
            PushUndo("");
            Vector2 on2; c = Grid(c);
            if (snapPipes && NearestOnPipes(c, 60, out on2)) c = on2;
            var d = new Dictionary<string, object>();
            d["type"] = "valve"; d["id"] = "valve_ed" + System.DateTime.Now.Ticks % 100000000;
            d["kind"] = "klapka"; d["label"] = "klapka"; d["open"] = false; d["_on"] = true; d["rot"] = 0.0;
            d["x"] = (double)c.x; d["y"] = (double)c.y;
            b.plan.AddItem(d);
            sel = b.plan.ById((string)d["id"]); selPt = -1;
            b.BuildPart(sel);
            status = "Klapka přidána doprostřed obrazovky – přetáhni ji na trubku.";
        }

        void FrontView()
        {
            var oc = Cam.GetComponent<OrbitCamera>();
            if (oc == null) return;
            oc.yaw = 0f; oc.pitch = 0f;
            if (sel != null)
            {
                Vector3 w = b.FromPlan(sel.Type == "pipe" ? PtMid(sel).x : sel.X, sel.Type == "pipe" ? PtMid(sel).y : sel.Y, 0f);
                oc.target = new Vector3(w.x, w.y, 0f);
                oc.dist = Mathf.Min(oc.dist, 6f);
            }
        }

        static Vector2 PtMid(PlanItem pipe)
        {
            List<double[]> pts = pipe.Pts();
            if (pts.Count == 0) return Vector2.zero;
            double[] p = pts[pts.Count / 2];
            return new Vector2((float)p[0], (float)p[1]);
        }

        // ───────────────────────── kreslení v obraze ─────────────────────────
        static void Line(Vector2 a, Vector2 c, float w, Color col)
        {
            Matrix4x4 save = GUI.matrix;
            Vector2 d = c - a;
            float len = d.magnitude;
            if (len < 0.5f) return;
            GUIUtility.RotateAroundPivot(Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg, a);
            GUI.color = col;
            GUI.DrawTexture(new Rect(a.x, a.y - w * 0.5f, len, w), Texture2D.whiteTexture);
            GUI.matrix = save;
            GUI.color = Color.white;
        }

        static void Box(Vector2 c, float size, Color col)
        {
            GUI.color = col;
            GUI.DrawTexture(new Rect(c.x - size * 0.5f, c.y - size * 0.5f, size, size), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }

        void DrawOverlay()
        {
            float u = uiScale;
            Color dim = new Color(0.4f, 0.9f, 1f, 0.55f);
            Color hot = new Color(1f, 0.55f, 0.1f, 0.95f);
            Color yel = new Color(1f, 0.95f, 0.2f, 1f);

            foreach (PlanItem it in b.plan.OfType("pipe"))
            {
                bool isSel = it == sel;
                List<double[]> pts = it.Pts();
                Vector2 prev = Vector2.zero; bool prevVis = false;
                for (int i = 0; i < pts.Count; i++)
                {
                    bool vis; Vector2 s = Scr(pts[i][0], pts[i][1], BreweryBuilder.ZP, out vis);
                    if (isSel && vis && prevVis) Line(prev, s, 3f * u, hot);
                    if (vis) Box(s, isSel ? (i == selPt ? 13f * u : 9f * u) : 4f * u, isSel ? (i == selPt ? yel : hot) : dim);
                    prev = s; prevVis = vis;
                }
            }

            if (sel != null && sel.Type != "pipe")
            {
                if (sel.Type == "vessel")
                {
                    float z = ZOf(sel);
                    Vector2 p1 = Scr(sel.X, sel.Y, z), p2 = Scr(sel.X + sel.W, sel.Y, z), p3 = Scr(sel.X + sel.W, sel.Y + sel.H, z), p4 = Scr(sel.X, sel.Y + sel.H, z);
                    Line(p1, p2, 3f * u, hot); Line(p2, p3, 3f * u, hot); Line(p3, p4, 3f * u, hot); Line(p4, p1, 3f * u, hot);
                }
                else
                {
                    Vector2 c = Scr(sel.X, sel.Y, ZOf(sel));
                    float h = 20f * u;
                    Line(c + new Vector2(-h, -h), c + new Vector2(h, -h), 3f * u, hot);
                    Line(c + new Vector2(h, -h), c + new Vector2(h, h), 3f * u, hot);
                    Line(c + new Vector2(h, h), c + new Vector2(-h, h), 3f * u, hot);
                    Line(c + new Vector2(-h, h), c + new Vector2(-h, -h), 3f * u, hot);
                }
            }

            if (mode == Mode.NewPipe)
            {
                Vector2 prev = Vector2.zero; bool has = false;
                foreach (double[] p in draw)
                {
                    Vector2 s = Scr(p[0], p[1], BreweryBuilder.ZP);
                    if (has) Line(prev, s, 3f * u, yel);
                    Box(s, 9f * u, yel); prev = s; has = true;
                }
                Vector2 plan;
                Vector2 mp = Event.current.mousePosition;
                if (has && RayPlan(mp, BreweryBuilder.ZP, out plan)) Line(prev, Scr(Grid(plan).x, Grid(plan).y, BreweryBuilder.ZP), 2f * u, new Color(1f, 0.95f, 0.2f, 0.5f));
            }

            Vector2 mpos = Event.current.mousePosition;
            Vector2 pl;
            if (RayPlan(mpos, BreweryBuilder.ZP, out pl))
            {
                var st = new GUIStyle(GUI.skin.label); st.fontSize = Mathf.RoundToInt(13 * u); st.normal.textColor = Color.white;
                GUI.Label(new Rect(mpos.x + 16, mpos.y + 12, 260 * u, 24 * u), "x " + Mathf.RoundToInt(pl.x) + "  y " + Mathf.RoundToInt(pl.y), st);
            }
        }

        // ───────────────────────── panel ─────────────────────────
        bool Btn(string t, bool active = false)
        {
            Color c = GUI.backgroundColor;
            if (active) GUI.backgroundColor = new Color(1f, 0.7f, 0.25f);
            bool r = GUILayout.Button(t);
            GUI.backgroundColor = c;
            return r;
        }

        double NumField(string label, string id, double v)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(44));
            string cname = "f_" + id;
            GUI.SetNextControlName(cname);
            bool focused = GUI.GetNameOfFocusedControl() == cname;
            string shown = v.ToString("0.##", CultureInfo.InvariantCulture);
            if (focused) { string s; if (buf.TryGetValue(cname, out s)) shown = s; }
            string ns = GUILayout.TextField(shown, GUILayout.Width(96));
            GUILayout.EndHorizontal();
            if (ns != shown)
            {
                buf[cname] = ns;
                double r;
                if (double.TryParse(ns.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out r)) return r;
            }
            else if (!focused) buf.Remove(cname);
            return v;
        }

        string StrField(string label, string id, string v)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(44));
            GUI.SetNextControlName("f_" + id);
            string ns = GUILayout.TextField(v ?? "", GUILayout.Width(150));
            GUILayout.EndHorizontal();
            return ns;
        }

        void DrawPanel()
        {
            Matrix4x4 save = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(new Vector3(uiScale, uiScale, 1f));
            float h = Screen.height / uiScale - 60f;
            panelRect = new Rect(10, 40, 270, h);
            GUI.Box(panelRect, "");
            GUILayout.BeginArea(new Rect(panelRect.x + 6, panelRect.y + 6, panelRect.width - 12, panelRect.height - 12));
            scroll = GUILayout.BeginScrollView(scroll);
            GUILayout.Label("EDITOR PLÁNU  (Tab = konec)");

            GUILayout.BeginHorizontal();
            if (Btn("Výběr / posun", mode == Mode.Select)) { mode = Mode.Select; draw.Clear(); }
            if (Btn("Nová trubka", mode == Mode.NewPipe)) { mode = Mode.NewPipe; draw.Clear(); sel = null; selPt = -1; status = "Klikej body trubky, Enter nebo klik na poslední bod = hotovo, Esc = zrušit."; }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (Btn("+ Klapka")) AddValve();
            if (Btn("Pohled zepředu")) FrontView();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Mřížka px:", GUILayout.Width(70));
            foreach (float g in new float[] { 0f, 1f, 5f, 10f, 20f }) if (Btn(g == 0f ? "vyp" : Fmt(g), grid == g)) grid = g;
            GUILayout.EndHorizontal();
            snapPipes = GUILayout.Toggle(snapPipes, " přichytit k bodům / trubkám");
            GUILayout.BeginHorizontal();
            if (Btn("Zpět (Ctrl+Z)")) Undo();
            if (Btn("Uložit (Ctrl+S)")) Save();
            GUILayout.EndHorizontal();
            if (status.Length > 0) GUILayout.Label(status);

            GUILayout.Space(6);
            if (sel != null) DrawProps();
            else GUILayout.Label("Nic nevybráno.\nKlik na trubku, bod, klapku, čerpadlo nebo nádobu.\nAlt při tažení = bez přichycení, Shift = rovně k sousednímu bodu.\nŠipky = posun o 1 px (Shift 10), R = otočit, Del = smazat, Ctrl+D = kopie.");

            GUILayout.EndScrollView();
            GUILayout.EndArea();
            GUI.matrix = save;
        }

        void DrawProps()
        {
            GUILayout.Label(sel.Type.ToUpper() + "  " + sel.Id);
            string lab = StrField("popis", "label", sel.Str("label", ""));
            if (lab != sel.Str("label", "")) { PushUndo("label"); sel.Set("label", lab); }

            if (sel.Type == "pipe")
            {
                double dn = NumField("DN", "dn", sel.Num("dn", 50));
                if (dn != sel.Num("dn", 50)) { PushUndo("dn"); sel.SetNum("dn", dn); b.RebuildPipe(sel); }
                GUILayout.BeginHorizontal();
                foreach (string m in new string[] { "water", "mash", "wort", "steam", "glycol" })
                    if (Btn(m, sel.Str("medium", "") == m)) { PushUndo(""); sel.Set("medium", m); b.RebuildPipe(sel); }
                GUILayout.EndHorizontal();
                bool hose = GUILayout.Toggle(sel.IsHose, " hadice");
                if (hose != sel.IsHose) { PushUndo(""); sel.Set("hose", hose); b.RebuildPipe(sel); }

                GUILayout.Label("Body (px plánu):");
                List<double[]> pts = sel.Pts();
                bool ch = false; int del = -1;
                for (int i = 0; i < pts.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(i.ToString(), GUILayout.Width(16));
                    double nx = Field2("p" + i + "x", pts[i][0]);
                    double ny = Field2("p" + i + "y", pts[i][1]);
                    if (nx != pts[i][0] || ny != pts[i][1]) { if (!ch) PushUndo("pt" + i); pts[i][0] = nx; pts[i][1] = ny; ch = true; selPt = i; }
                    if (GUILayout.Button("×", GUILayout.Width(24)) && pts.Count > 2) del = i;
                    GUILayout.EndHorizontal();
                }
                if (del >= 0) { PushUndo(""); pts.RemoveAt(del); ch = true; selPt = -1; }
                if (ch) { sel.SetPts(pts); b.RebuildPipe(sel); }
            }
            else
            {
                double x = NumField("x", "x", sel.X);
                double y = NumField("y", "y", sel.Y);
                bool moved = x != sel.X || y != sel.Y;
                if (moved) { PushUndo("xy"); sel.SetNum("x", x); sel.SetNum("y", y); }
                bool resized = false;
                if (sel.Type == "vessel")
                {
                    double w = NumField("šířka", "w", sel.W);
                    double hh = NumField("výška", "h", sel.H);
                    if (w != sel.W || hh != sel.H) { PushUndo("wh"); sel.SetNum("w", w); sel.SetNum("h", hh); resized = true; }
                }
                if (sel.Type == "valve" || sel.Type == "pump" || sel.Type == "inl" || sel.Type == "flow" || sel.Type == "nozzle")
                {
                    string key = (sel.Type == "pump" || sel.Type == "nozzle") ? "dir" : "rot";
                    double r = NumField("úhel", "rot", sel.Num(key, 0));
                    if (r != sel.Num(key, 0)) { PushUndo("rot"); sel.SetNum(key, r); moved = true; }
                }
                if (moved || resized) Changed(sel, resized);
            }

            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            if (Btn("Smazat (Del)")) DeleteSel();
            if (Btn("Kopie (Ctrl+D)")) Duplicate();
            GUILayout.EndHorizontal();
            if (sel != null && Protected(sel)) GUILayout.Label("Navázáno na simulaci – nejde smazat ani kopírovat.");
        }

        double Field2(string id, double v)
        {
            string cname = "f_" + id;
            GUI.SetNextControlName(cname);
            bool focused = GUI.GetNameOfFocusedControl() == cname;
            string shown = v.ToString("0.##", CultureInfo.InvariantCulture);
            if (focused) { string s; if (buf.TryGetValue(cname, out s)) shown = s; }
            string ns = GUILayout.TextField(shown, GUILayout.Width(70));
            if (ns != shown)
            {
                buf[cname] = ns;
                double r;
                if (double.TryParse(ns.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out r)) return r;
            }
            else if (!focused) buf.Remove(cname);
            return v;
        }
    }
}
