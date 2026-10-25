// Hybridní režim: simulace, ovládání, rozvaděč a lupa běží v Godotu (přeložený kód), tohle je most do 3D scény.
//  • kamera 3D scény se řídí kamerou Godotu (tažení myší a kolečko fungují jako v původní aplikaci),
//  • plán (nádoby, trubky, klapky, čerpadla) se bere z Godotu – po úpravě v editoru se 3D varna přestaví,
//  • stav simulace se přenáší do 3D: hladiny a barvy kapalin, var a pára, tok v potrubí, polohy klapek, čerpadla, míchadla a hrabla.
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Pivovar.Core;
using Pivovar.Gd;
using Pivovar.Gd.Gen;
using G = Pivovar.Gd.Gd;

namespace Pivovar
{
    public sealed class SimSync : MonoBehaviour
    {
        public static bool Slaved;

        BreweryBuilder b;
        S_main m;
        readonly Dictionary<string, Gv> gItems = new Dictionary<string, Gv>();
        readonly Dictionary<string, string> itemSig = new Dictionary<string, string>();
        readonly Dictionary<string, string> valveSig = new Dictionary<string, string>();
        readonly Dictionary<string, Color> lastColor = new Dictionary<string, Color>();
        readonly Dictionary<string, LiquidState> lastLook = new Dictionary<string, LiquidState>();
        float vesselDt = 0.05f;
        readonly Dictionary<string, float> lastFill = new Dictionary<string, float>();
        readonly Dictionary<string, Vector2[]> pipeEnds = new Dictionary<string, Vector2[]>();
        float tStruct, tVessel, tPipe;
        bool ready;

        static readonly string[] SigKeys = { "id", "type", "x", "y", "w", "h", "shape", "bind", "rot", "dir", "dn", "kind", "hose", "look", "k", "len", "pts", "medium", "dark" };
        static readonly HashSet<string> LiquidBinds = new HashSet<string> { "rvp", "mp", "sk", "wp", "hlt", "lv", "spilka" };

        public void Init(BreweryBuilder builder, S_main main)
        {
            b = builder; m = main; Slaved = true;
            RebuildAll();
        }

        void OnDestroy() { Slaved = false; }

        // ───────────── plán z Godotu ─────────────
        Gv PlanDict { get { return m.plan; } }

        void RebuildAll()
        {
            try
            {
                if (PlanDict.k != Pivovar.Gd.K.Dict || PlanDict.Dict.Count == 0) { Debug.LogWarning("Pivovar: Godot ještě nemá načtený plán"); ready = false; return; }
                string json = G.Json(PlanDict, false);
                Plan plan = Plan.Parse(json);
                b.BeginHybrid(plan);
                IndexItems();
                valveSig.Clear(); lastColor.Clear(); lastFill.Clear(); lastLook.Clear();
                ready = true;
                Debug.Log("Pivovar: 3D varna postavena z plánu Godotu (" + m.items.Arr.Count + " položek)");
            }
            catch (Exception e) { Debug.LogError("Pivovar: stavba 3D z plánu Godotu selhala: " + e); ready = false; }
        }

        void IndexItems()
        {
            gItems.Clear(); itemSig.Clear(); pipeEnds.Clear();
            var arr = m.items.Arr; if (arr == null) return;
            foreach (Gv it in arr.l)
            {
                string id = it.G("id").Str; if (id == null) continue;
                gItems[id] = it;
                itemSig[id] = ItemSig(it);
                if (it.G("type").Str == "pipe")
                {
                    var pts = it.G("pts").Arr;
                    if (pts != null && pts.Count >= 2)
                        pipeEnds[id] = new[] { Pt(pts.l[0]), Pt(pts.l[pts.Count - 1]) };
                }
            }
        }

        static Vector2 Pt(Gv p)
        {
            if (p.k == Pivovar.Gd.K.Vec2 || p.k == Pivovar.Gd.K.Vec2i) return new Vector2((float)p.d, (float)p.e);
            var a = p.Arr; if (a != null && a.Count >= 2) return new Vector2((float)a.l[0].d, (float)a.l[1].d);
            return Vector2.zero;
        }

        static string ItemSig(Gv it)
        {
            var d = it.Dict; if (d == null) return "";
            var sb = new StringBuilder(96);
            for (int i = 0; i < SigKeys.Length; i++)
            {
                Gv v;
                if (!d.TryGet(Gv.S(SigKeys[i]), out v)) continue;
                sb.Append(SigKeys[i]).Append('=');
                if (v.IsNum) sb.Append(Math.Round(v.d * 10.0));
                else if (v.k == Pivovar.Gd.K.Str) sb.Append(v.Str);
                else if (v.k == Pivovar.Gd.K.Bool) sb.Append(v.d != 0 ? '1' : '0');
                else sb.Append(G.Json(v, false));
                sb.Append(';');
            }
            return sb.ToString();
        }

        /// <summary>Zjistí, jestli se v Godotu změnil plán (editor, tažení hadice): jen trubky se přestaví samy, jinak celá varna.</summary>
        void CheckStructure()
        {
            var arr = m.items.Arr; if (arr == null) return;
            bool full = arr.Count != gItems.Count;
            var changedPipes = new List<Gv>();
            if (!full)
            {
                foreach (Gv it in arr.l)
                {
                    string id = it.G("id").Str;
                    string old;
                    if (id == null || !itemSig.TryGetValue(id, out old)) { full = true; break; }
                    string now = ItemSig(it);
                    if (now == old) continue;
                    if (it.G("type").Str == "pipe") changedPipes.Add(it); else { full = true; break; }
                }
            }
            if (full) { RebuildAll(); return; }
            foreach (Gv it in changedPipes)
            {
                string id = it.G("id").Str;
                PlanItem pi = b.plan.ById(id); if (pi == null) { RebuildAll(); return; }
                try
                {
                    var d = MiniJson.Parse(G.Json(it, false)) as Dictionary<string, object>;
                    pi.D.Clear(); foreach (var kv in d) pi.D[kv.Key] = kv.Value;
                    b.RebuildPipe(pi);
                    itemSig[id] = ItemSig(it);
                    gItems[id] = it;
                    var pts = it.G("pts").Arr;
                    if (pts != null && pts.Count >= 2) pipeEnds[id] = new[] { Pt(pts.l[0]), Pt(pts.l[pts.Count - 1]) };
                }
                catch (Exception e) { Debug.LogWarning("Pivovar: trubka " + id + ": " + e.Message); }
            }
        }

        // ───────────── každý snímek ─────────────
        void LateUpdate()
        {
            if (m == null || b == null) return;
            if (!ready) { tStruct += Time.unscaledDeltaTime; if (tStruct > 0.5f) { tStruct = 0; RebuildAll(); } return; }
            try
            {
                SlaveCamera();
                SyncValves();
                SyncPumps();
                SyncSpinners();
                tVessel += Time.unscaledDeltaTime; tPipe += Time.unscaledDeltaTime; tStruct += Time.unscaledDeltaTime;
                if (tVessel > 0.05f) { vesselDt = tVessel; tVessel = 0; SyncVessels(); }
                if (tPipe > 0.12f) { tPipe = 0; SyncPipes(); }
                if (tStruct > 0.5f) { tStruct = 0; CheckStructure(); }
            }
            catch (Exception e) { if (Time.frameCount % 300 == 0) Debug.LogWarning("Pivovar SimSync: " + e); }
        }

        void SlaveCamera()
        {
            Camera cam = b.Cam; if (cam == null) return;
            Gv gc = m.cam; if (gc.Obj == null) return;
            Gv p = gc.G("position"), z = gc.G("zoom");
            double zoom = (z.k == Pivovar.Gd.K.Vec2 && z.d > 0.001) ? z.d : 1.0;
            double sw = Math.Max(2.0, G.ScreenH);
            cam.orthographic = true;
            cam.orthographicSize = (float)(sw * 0.5 / zoom * BreweryBuilder.K);
            Vector3 pos = new Vector3((float)p.d * BreweryBuilder.K, (b.floorY - (float)p.e) * BreweryBuilder.K, -10f);
            cam.transform.position = pos;
            cam.transform.rotation = Quaternion.identity;
            cam.nearClipPlane = 0.1f; cam.farClipPlane = 80f;
        }

        void SyncValves()
        {
            foreach (PlanItem v in b.Valves())
            {
                Gv gi; if (!gItems.TryGetValue(v.Id, out gi)) continue;
                Gv open = gi.G("open"), part = gi.G("part"), pct = gi.G("pct");
                string sig = (open.IsNull ? "-" : (open.T() ? "1" : "0")) + "|" + (part.IsNum ? ((int)part.d).ToString() : "-") + "|" + (pct.IsNum ? ((int)pct.d).ToString() : "-");
                string old; if (valveSig.TryGetValue(v.Id, out old) && old == sig) continue;
                valveSig[v.Id] = sig;
                if (open.IsNull) v.D.Remove("open"); else v.D["open"] = open.T();
                if (part.IsNum) v.D["part"] = part.d; else v.D.Remove("part");
                if (pct.IsNum) v.D["pct"] = pct.d; else v.D.Remove("pct");
                if (old != null) b.RefreshValve(v);   // první průchod jen zapíše stav, plán už klapky postavil s ním
            }
        }

        static readonly Color LedOff = new Color(0.12f, 0.12f, 0.13f);
        void SyncPumps()
        {
            foreach (var kv in b.PumpLeds)
            {
                Gv gi; if (!gItems.TryGetValue(kv.Key, out gi) || kv.Value == null) continue;
                int st = (int)m.pump_state(gi).d;
                Color c = st == 2 ? new Color(0.18f, 0.85f, 0.35f) : st == 3 ? new Color(0.2f, 0.55f, 1f) : st == 1 ? new Color(1f, 0.6f, 0.15f) : LedOff;
                var mat = kv.Value.material;
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c); else mat.color = c;
                HdrpUtil.SetEmissive(mat, c, st == 0 ? 0f : 40f);
            }
        }

        void SyncSpinners()
        {
            Gv S = m.sim.G("S");
            foreach (BreweryBuilder.Spinner sp in b.SpinnerList)
            {
                if (sp.t == null) continue;
                float dps = 0f;
                if (sp.bind == "sk") { if (S.G("sk").G("rake").T()) dps = (float)(m.sim.C("rkRpm").d * 36.0); }
                else if (sp.bind == "rvp" || sp.bind == "mp")
                {
                    Gv vv = S.G(sp.bind);
                    if (vv.G("stir").T()) dps = (float)(vv.G("hz").d * 14.4);
                }
                sp.dps = Mathf.Lerp(sp.dps, dps, 0.15f);
            }
        }

        static Color ToColor(Gv c, float a)
        {
            var cl = c.Cl; if (cl == null) return new Color(0.4f, 0.6f, 0.8f, a);
            return new Color((float)cl.r, (float)cl.g, (float)cl.b, a);
        }

        void SyncVessels()
        {
            Gv sim = m.sim; Gv S = sim.G("S");
            foreach (Vessel v in b.VesselList)
            {
                string bind = v.bind;
                if (string.IsNullOrEmpty(bind) || !LiquidBinds.Contains(bind)) continue;
                Gv gi; if (!gItems.TryGetValue(v.item.Id, out gi)) continue;
                float lyPx = (float)m.level_y(gi).d;
                float lvl = (b.floorY - lyPx) * BreweryBuilder.K;                 // výška hladiny v metrech
                float bottom = (b.floorY - (float)(v.item.Y + v.item.H)) * BreweryBuilder.K;
                bool horiz = v.hrr > 0.01f;
                float fillAll = Mathf.Clamp01((lvl - bottom) / Mathf.Max(0.1f, (float)v.item.H * BreweryBuilder.K));
                Color col = ToColor(m.vessel_color(gi), 0.85f);
                LiquidState ls = StateOf(bind, S, col);
                LiquidState lo;
                if (!lastLook.TryGetValue(v.item.Id, out lo) || LookChanged(lo, ls)) { lastLook[v.item.Id] = ls; v.SetLook(ls); }
                if (ls.swirlDps != 0f) v.Swirl(ls.swirlDps * vesselDt);
                if (bind == "sk")
                {
                    float vesselH = (float)v.item.H * BreweryBuilder.K;
                    float bedH = (float)m.sim.C("skBedH").d / 0.943f * vesselH;      // stejné měřítko jako v Godotu
                    v.SetBed(bedH, lvl >= v.BedY0 + bedH);
                }

                if (horiz)
                {
                    if (v.liqCyl != null)
                    {
                        bool any = fillAll > 0.03f;
                        v.liqCyl.SetActive(any);
                        if (any)
                        {
                            float s = Mathf.Clamp(Mathf.Sqrt(fillAll), 0.2f, 1f);
                            v.liqCyl.transform.localScale = new Vector3(s, 1f, s);
                            v.liqCyl.transform.localPosition = new Vector3(v.hpos.x, v.hy + v.hrr * s, v.hpos.z);
                        }
                    }
                    continue;
                }
                float f = (lvl - v.by) / Mathf.Max(0.1f, v.hcyl);
                if (f <= 0f) f = fillAll > 0.004f ? 0.004f : 0f;
                float lf; if (!lastFill.TryGetValue(v.item.Id, out lf) || Mathf.Abs(lf - f) > 0.002f) { lastFill[v.item.Id] = f; v.SetFill(f); }

                float boil = 0f; bool steam = false;
                if (bind == "rvp" || bind == "mp" || bind == "wp")
                {
                    Gv vv = S.G(bind);
                    bool boiling = vv.G("boiling").T();
                    boil = boiling ? 1f : (vv.G("whirlNow").T() ? 0.15f : 0f);
                    steam = boiling;
                }
                v.SetBoil(boil);
                if (v.steam != null) v.steam.strength = steam ? 1f : 0f;
            }
        }

        static bool LookChanged(LiquidState a, LiquidState c)
        {
            return a.kind != c.kind || Mathf.Abs(a.solids - c.solids) > 0.004f || Mathf.Abs(a.turb - c.turb) > 0.03f || Mathf.Abs(a.T - c.T) > 2f
                || Mathf.Abs(a.stir - c.stir) > 0.05f || Mathf.Abs(a.boil - c.boil) > 0.05f || Mathf.Abs(a.foam - c.foam) > 0.03f
                || Mathf.Abs(a.col.r - c.col.r) + Mathf.Abs(a.col.g - c.col.g) + Mathf.Abs(a.col.b - c.col.b) > 0.012f;
        }

        static LiqKind KindOf(string k)
        {
            switch (k) { case "mash": return LiqKind.Mash; case "wort": return LiqKind.Wort; case "empty": return LiqKind.Empty; default: return LiqKind.Water; }
        }

        /// <summary>Dávka (pánev, vířivka, rmut ve scezováku): druh obsahu, podíl sušiny, zákal, míchání, var, pěna.</summary>
        LiquidState FromBatch(Gv v, Color col)
        {
            Gv sim = m.sim;
            var ls = new LiquidState { col = col };
            ls.kind = KindOf(sim.C("contentKind", v).Str);
            double vol = Math.Max(1.0, sim.C("vol", v).d), liq = Math.Max(1.0, sim.C("liqVol", v).d);
            ls.solids = (float)(sim.C("sol", v).d * 0.65 / vol);
            ls.turb = Mathf.Clamp01((float)(v.G("trubSus").d / liq * 1500.0));
            ls.T = (float)v.G("T").d;
            bool stir = v.G("stir").T();
            double hz = v.G("hz").d;
            ls.stir = stir ? Mathf.Clamp01((float)(hz / 35.0)) : 0f;
            bool whirl = v.G("whirlNow").T();
            ls.boil = v.G("boiling").T() ? 1f : (whirl ? 0.15f : 0f);
            ls.foam = Mathf.Clamp01((float)(v.G("foam").d / Math.Max(60.0, vol * 0.25)));
            ls.swirlDps = stir ? (float)(hz * 14.4 * 0.35) : (whirl ? 18f : 0f);
            return ls;
        }

        LiquidState StateOf(string bind, Gv S, Color col)
        {
            Gv sim = m.sim;
            var ls = new LiquidState { kind = LiqKind.Water, col = col, T = 20f };
            switch (bind)
            {
                case "rvp": case "mp": case "wp":
                    return FromBatch(S.G(bind), col);
                case "sk":
                    {
                        Gv sk = S.G("sk"); Gv mash = sk.G("mash");
                        if (sim.C("vol", mash).d > 50.0) { ls = FromBatch(mash, col); ls.stir = Mathf.Max(ls.stir, sk.G("rake").T() ? 0.5f : 0f); }
                        else
                        {
                            Gv cells = sk.G("cells");
                            var ca = cells.Arr;
                            double w = 1.0, ex = 0.0;
                            if (ca != null && ca.Count > 0) { Gv c0 = ca.l[0]; w = Math.Max(1.0, c0.G("w").d); ex = c0.G("f").d + c0.G("n").d; }
                            ls.kind = ex > 0.005 * w ? LiqKind.Wort : LiqKind.Water;
                            ls.T = (float)sk.G("T").d;
                            ls.turb = Mathf.Clamp01((float)(sk.G("turb").d / 150.0));
                            ls.stir = sk.G("rake").T() ? 0.5f : 0f;
                        }
                        if (sk.G("rake").T()) ls.swirlDps = (float)(sim.C("rkRpm").d * 36.0);
                        return ls;
                    }
                case "hlt": ls.T = (float)S.G("hlt").G("T").d; return ls;
                case "lv": ls.T = (float)S.G("lv").G("T").d; return ls;
                case "spilka":
                    {
                        Gv sp = S.G("sp"); Gv fer = S.G("fer");
                        double L = Math.Max(1.0, sp.G("L").d);
                        ls.kind = sp.G("f").d + sp.G("n").d > 1.0 ? LiqKind.Wort : LiqKind.Water;
                        ls.T = (float)(sp.G("H").d / L);
                        ls.turb = Mathf.Clamp01((float)(sp.G("trub").d / L * 1500.0));
                        if (fer.G("on").T())
                        {
                            double sf = fer.G("Sf").d, s0 = Math.Max(1.0, fer.G("S0").d);
                            double act = Math.Min(1.0, fer.G("X").d / 2.5) * (sf > 0.3 * s0 ? 1.0 : 0.35);
                            ls.foam = Mathf.Clamp01((float)act * 0.8f);      // krausen při hlavním kvašení
                            ls.boil = Mathf.Clamp01((float)act * 0.25f);       // CO2 bublinky
                            ls.turb = Mathf.Max(ls.turb, Mathf.Clamp01((float)(fer.G("X").d / 6.0)));
                        }
                        return ls;
                    }
            }
            return ls;
        }

        sealed class PipeAcc { public float full, flow, dn = 50f; public Color col; public int dir; public bool mash; }
        readonly Dictionary<string, PipeAcc> acc = new Dictionary<string, PipeAcc>();

        void SyncPipes()
        {
            Gv sim = m.sim; Gv edges = sim.G("net").G("edges");
            var arr = edges.Arr; if (arr == null) return;
            acc.Clear();
            foreach (Gv e in arr.l)
            {
                string pid = e.G("pipe").Str; if (pid == null) continue;
                Gv c = e.G("c");
                double cap = e.G("cap").d; if (cap <= 0.0001) continue;
                float full = (float)(sim.C("vol", c).d / cap);
                float flow = (float)e.G("flow").d;
                PipeAcc a; if (!acc.TryGetValue(pid, out a)) { a = new PipeAcc(); acc[pid] = a; }
                if (flow > a.flow) { a.flow = flow; a.dn = (float)e.G("dn").d; }
                if (full > a.full)
                {
                    a.full = full;
                    Gv kind = sim.C("contentKind", c);
                    double liq = Math.Max(0.01, sim.C("liqVol", c).d);
                    a.col = ToColor(m.kind_color(kind, c.G("T"), Gv.F(c.G("col").d / liq)), 1f);
                    a.mash = kind.Str == "mash";
                }
                if (flow > 0.01f)
                {
                    int dirE = e.G("from").Str == "a" ? 1 : -1;
                    var pts = e.G("pts").Arr; Vector2[] ends;
                    if (pts != null && pts.Count >= 2 && pipeEnds.TryGetValue(pid, out ends))
                    {
                        Vector2 p0 = Pt(pts.l[0]);
                        if ((p0 - ends[0]).sqrMagnitude > (p0 - ends[1]).sqrMagnitude) dirE = -dirE;   // hrana je vedena proti směru trubky v plánu
                    }
                    a.dir = dirE;
                }
            }
            foreach (var kv in acc)
            {
                PipeFlow pf = b.FlowOf(kv.Key); if (pf == null) continue;
                PipeAcc a = kv.Value;
                bool has = a.full >= 0.06f;
                pf.fill = has ? Mathf.Clamp01(a.full * 1.15f) : 0f;
                if (has) pf.liquid = a.col;
                pf.thick = a.mash ? 1f : 0f;
                float area = 0.00196f * Mathf.Pow(Mathf.Max(10f, a.dn) / 50f, 2f);
                float v = a.flow / 60000f / area;                          // m/s
                pf.speed = (has && a.flow > 0.01f) ? a.dir * Mathf.Clamp(v, 0.15f, 6f) : 0f;      // m/s; textura proudu se posouvá po metrech trubky
            }
            // trubky, které simulace nezná (nebo úplně prázdné)
            foreach (PipeFlow pf in b.AllFlows()) if (pf != null && !HasAny(pf)) { pf.fill = 0f; pf.speed = 0f; }
        }

        bool HasAny(PipeFlow pf)
        {
            // trubka s editovaným id (název objektu "trubka <id>") – je v acc, pokud ji simulace počítá
            var t = pf.transform.parent; if (t == null) return false;
            string n = t.name; if (!n.StartsWith("trubka ")) return false;
            return acc.ContainsKey(n.Substring(7));
        }
    }
}
