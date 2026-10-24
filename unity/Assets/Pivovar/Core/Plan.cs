// Plán varny (plan.json) – stejná data jako používá 2D verze: nádoby, trubky, armatury, čerpadla.
// Souřadnice jsou v pixelech plánu: 1 px = 5 mm (PxPerM = 200 ve 3D; 2D simulace počítá s 100 px/m pro délky trubek).
using System;
using System.Collections.Generic;

namespace Pivovar.Core
{
    public sealed class PlanItem
    {
        public readonly Dictionary<string, object> D;
        public PlanItem(Dictionary<string, object> d) { D = d; }

        public string Id { get { return Str("id", ""); } }
        public string Type { get { return Str("type", ""); } }
        public double X { get { return Num("x", 0); } }
        public double Y { get { return Num("y", 0); } }
        public double W { get { return Num("w", 0); } }
        public double H { get { return Num("h", 0); } }
        public string Shape { get { return Str("shape", ""); } }
        public string Bind { get { return Str("bind", ""); } }
        public bool IsHose { get { return Bool("hose", false); } }

        public string Str(string k, string def)
        {
            object o; if (D.TryGetValue(k, out o) && o is string) return (string)o; return def;
        }
        public double Num(string k, double def)
        {
            object o; if (D.TryGetValue(k, out o) && o is double) return (double)o; return def;
        }
        public bool Bool(string k, bool def)
        {
            object o; if (D.TryGetValue(k, out o) && o is bool) return (bool)o; return def;
        }
        public bool Has(string k) { return D.ContainsKey(k); }

        /// <summary>Lomená čára trubky v px plánu: [[x,y],...]</summary>
        public List<double[]> Pts()
        {
            var res = new List<double[]>();
            object o;
            if (!D.TryGetValue("pts", out o)) return res;
            var l = o as List<object>;
            if (l == null) return res;
            foreach (object q in l)
            {
                var a = q as List<object>;
                if (a == null || a.Count < 2) continue;
                res.Add(new double[] { (double)a[0], (double)a[1] });
            }
            return res;
        }
    }

    public sealed class Plan
    {
        public readonly List<PlanItem> Items = new List<PlanItem>();
        public readonly List<double[]> Joints = new List<double[]>();
        public int Version;

        public static Plan Parse(string json)
        {
            var root = MiniJson.Parse(json) as Dictionary<string, object>;
            if (root == null) throw new FormatException("plan.json: kořen není objekt");
            var plan = new Plan();
            object o;
            if (root.TryGetValue("ver", out o) && o is double) plan.Version = (int)(double)o;
            if (root.TryGetValue("items", out o))
            {
                var l = o as List<object>;
                if (l != null)
                    foreach (object q in l)
                    {
                        var d = q as Dictionary<string, object>;
                        if (d != null) plan.Items.Add(new PlanItem(d));
                    }
            }
            if (root.TryGetValue("joints", out o))
            {
                var l = o as List<object>;
                if (l != null)
                    foreach (object q in l)
                    {
                        var a = q as List<object>;
                        if (a != null && a.Count >= 2) plan.Joints.Add(new double[] { (double)a[0], (double)a[1] });
                    }
            }
            return plan;
        }

        public IEnumerable<PlanItem> OfType(string type)
        {
            foreach (var it in Items) if (it.Type == type) yield return it;
        }

        public PlanItem ById(string id)
        {
            foreach (var it in Items) if (it.Id == id) return it;
            return null;
        }
    }
}
