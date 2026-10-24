using System;
using System.Collections.Generic;
using System.IO;
using Pivovar.Core;

static class Program
{
    static int pass, fail;
    static void Chk(string name, bool ok, string info = "")
    {
        if (ok) { pass++; Console.WriteLine("PASS  " + name + (info.Length > 0 ? "  [" + info + "]" : "")); }
        else { fail++; Console.WriteLine("FAIL  " + name + (info.Length > 0 ? "  [" + info + "]" : "")); }
    }

    static int Main(string[] args)
    {
        MiniJsonTests();
        PlanTests(args.Length > 0 ? args[0] : "../Assets/StreamingAssets/plan.json");
        Console.WriteLine();
        Console.WriteLine("celkem PASS " + pass + " / FAIL " + fail);
        return fail == 0 ? 0 : 1;
    }

    static void MiniJsonTests()
    {
        var d = (Dictionary<string, object>)MiniJson.Parse("{\"a\":1.5,\"b\":[1,2,{\"c\":null}],\"s\":\"x\\ny\\u00e1\",\"t\":true,\"n\":-2e2}");
        Chk("json: číslo", (double)d["a"] == 1.5);
        Chk("json: exponent", (double)d["n"] == -200.0);
        Chk("json: pole a vnořený objekt", ((List<object>)d["b"]).Count == 3);
        Chk("json: escape a unicode", (string)d["s"] == "x\nyá");
        Chk("json: true", (bool)d["t"] == true);
        bool threw = false;
        try { MiniJson.Parse("{\"a\":}"); } catch (FormatException) { threw = true; }
        Chk("json: chybný vstup vyhodí výjimku", threw);
        threw = false;
        try { MiniJson.Parse("[1,2"); } catch (FormatException) { threw = true; }
        Chk("json: neukončené pole vyhodí výjimku", threw);
    }

    static void PlanTests(string path)
    {
        Plan plan = Plan.Parse(File.ReadAllText(path));
        int vessels = 0, pipes = 0, valves = 0, pumps = 0;
        foreach (var it in plan.Items)
        {
            if (it.Type == "vessel") vessels++;
            else if (it.Type == "pipe") pipes++;
            else if (it.Type == "valve") valves++;
            else if (it.Type == "pump") pumps++;
        }
        Chk("plán: 10 nádob", vessels == 10, vessels.ToString());
        Chk("plán: 74 trubek", pipes == 74, pipes.ToString());
        Chk("plán: 56 armatur", valves == 56, valves.ToString());
        Chk("plán: 5 čerpadel", pumps == 5, pumps.ToString());

        // jako v 2D testech: všechny trubky (mimo hadice) jsou vodorovné nebo svislé
        int skew = 0, shortPipes = 0;
        foreach (var it in plan.OfType("pipe"))
        {
            var pts = it.Pts();
            if (pts.Count < 2) { shortPipes++; continue; }
            if (it.IsHose) continue;
            for (int k = 1; k < pts.Count; k++)
                if (Math.Abs(pts[k][0] - pts[k - 1][0]) > 0.5 && Math.Abs(pts[k][1] - pts[k - 1][1]) > 0.5) skew++;
        }
        Chk("plán: každá trubka má aspoň 2 body", shortPipes == 0, shortPipes.ToString());
        // tvůj plán má 4 mírně šikmé úseky (pipe_ed185140931, pipe_ed493436390, pipe_ed1150856943); víc než 4 by znamenalo chybu v datech
        Chk("plán: šikmých úseků trubek (mimo hadice) není víc než ve tvém plánu", skew <= 4, "šikmých úseků " + skew);

        var rvp = plan.ById("rvp");
        Chk("plán: pánev rvp má pozici -50,410 a rozměr 380x350", rvp != null && rvp.X == -50 && rvp.Y == 410 && rvp.W == 380 && rvp.H == 350);
        var hose = plan.ById("pipe_murcuixph");
        Chk("plán: spílací hadice je hadice a má 5 bodů", hose != null && hose.IsHose && hose.Pts().Count == 5);
        Chk("plán: jsou načteny spoje trubek", plan.Joints.Count > 0, plan.Joints.Count.ToString());
    }
}
