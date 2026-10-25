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

    static int RunGdTests(string resDir)
    {
        int gp = 0, gf = 0;
        Pivovar.Gd.Gd.PrintHook = line =>
        {
            Console.WriteLine(line);
            if (line.StartsWith("PASS")) gp++; else if (line.StartsWith("FAIL")) gf++;
        };
        Pivovar.Gd.Gd.ResDir = resDir;
        Pivovar.Gd.Gd.UserDir = Path.Combine(Path.GetTempPath(), "zajic_user_" + Environment.ProcessId);
        Pivovar.Gd.Gd.CmdArgs = new[] { Pivovar.Gd.Gv.S("--tests") };
        try
        {
            Pivovar.Gd.Gen.GdRegistry.Init();
            var mv = Pivovar.Gd.Gen.S_main.New();
            ((Pivovar.Gd.GdNode)mv.Obj).EnterTree();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unhandled výjimka v přeloženém kódu: " + ex.GetType().Name + ": " + ex.Message);
            var st = ex.StackTrace ?? "";
            var lines = st.Split('\n');
            for (int i = 0; i < Math.Min(14, lines.Length); i++) Console.WriteLine("   " + lines[i].Trim());
            Console.WriteLine("celkem (Godot testy, přerušeno) PASS " + gp + " / FAIL " + gf);
            return 2;
        }
        Console.WriteLine("celkem (Godot testy) PASS " + gp + " / FAIL " + gf + (Pivovar.Gd.Gd.Quit ? "" : "  [nedoběhly do konce]"));
        return (gf == 0 && Pivovar.Gd.Gd.Quit) ? 0 : 1;
    }

    static int RunGdAuto(string resDir, string[] extra)
    {
        var errs = new List<string>();
        Pivovar.Gd.Gd.PrintHook = line => Console.WriteLine(line);
        Pivovar.Gd.Gd.ErrHook = e => { errs.Add(e); Console.WriteLine("CHYBA " + e); };
        Pivovar.Gd.Gd.ResDir = resDir;
        Pivovar.Gd.Gd.UserDir = Path.Combine(Path.GetTempPath(), "zajic_user_" + Environment.ProcessId);
        var ca = new List<Pivovar.Gd.Gv> { Pivovar.Gd.Gv.S("--autotest") };
        if (Array.IndexOf(extra, "--userplan") >= 0)
        {
            Directory.CreateDirectory(Pivovar.Gd.Gd.UserDir);
            File.Copy(Path.Combine(resDir, "user_plan.json"), Path.Combine(Pivovar.Gd.Gd.UserDir, "plan_upravy.json"), true);
        }
        foreach (var x in extra) ca.Add(Pivovar.Gd.Gv.S(x));
        Pivovar.Gd.Gd.CmdArgs = ca.ToArray();
        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            Pivovar.Gd.Gen.GdRegistry.Init();
            var mv = Pivovar.Gd.Gen.S_main.New();
            ((Pivovar.Gd.GdNode)mv.Obj).EnterTree();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unhandled výjimka: " + ex.GetType().Name + ": " + ex.Message);
            var lines = (ex.StackTrace ?? "").Split('\n');
            for (int i = 0; i < Math.Min(14, lines.Length); i++) Console.WriteLine("   " + lines[i].Trim());
            return 2;
        }
        Console.WriteLine("celkem (autotest) chyb: " + errs.Count + ", čas výpočtu " + sw.Elapsed.TotalSeconds.ToString("0") + " s");
        return errs.Count == 0 ? 0 : 1;
    }

    static int RunGdBoot(string resDir)
    {
        var errs = new List<string>();
        Pivovar.Gd.Gd.PrintHook = line => { if (line.Length < 400) Console.WriteLine(line); };
        Pivovar.Gd.Gd.ErrHook = e => { errs.Add(e); Console.WriteLine("CHYBA " + e); };
        Pivovar.Gd.Gd.Lenient = true;
        Pivovar.Gd.GdUi.Active = true;
        Pivovar.Gd.Gd.ResDir = resDir;
        Pivovar.Gd.Gd.UserDir = Path.Combine(Path.GetTempPath(), "zajic_user_" + Environment.ProcessId);
        Pivovar.Gd.Gd.CmdArgs = new Pivovar.Gd.Gv[0];
        Pivovar.Gd.Gd.DisplayName = "unity";
        Pivovar.Gd.Gen.GdRegistry.Init();
        var mv = Pivovar.Gd.Gen.S_main.New();
        var root = (Pivovar.Gd.GdNode)mv.Obj;
        try { root.EnterTree(); } catch (Exception ex) { Pivovar.Gd.Gd.Err("EnterTree", ex); }
        int nodes = 0; Pivovar.Gd.GdHost.Walk(root, n => nodes++);
        Console.WriteLine("uzlů ve scéně: " + nodes);
        for (int f = 0; f < 180; f++)
        {
            try { Pivovar.Gd.GdHost.Frame(root, 1.0 / 60.0); } catch (Exception ex) { Pivovar.Gd.Gd.Err("Frame", ex); }
            try { Pivovar.Gd.GdHost.RedrawDirty(root); } catch (Exception ex) { Pivovar.Gd.Gd.Err("Redraw", ex); }
        }
        var items = Pivovar.Gd.GdRender.Collect(root);
        int tris = 0, batches = 0, runs = 0;
        foreach (var it in items) { if (it.mesh == null) continue; tris += it.mesh.Tris; foreach (var b in it.mesh.batches) { batches++; if (b.runs != null) runs += b.runs.Count; } }
        Console.WriteLine("vykreslovaných uzlů: " + items.Count + ", trojúhelníků: " + tris + ", dávek: " + batches + ", textů: " + runs);
        Console.WriteLine("celkem (boot) chyb: " + Pivovar.Gd.Gd.Errors.Count);
        return Pivovar.Gd.Gd.Errors.Count == 0 ? 0 : 1;
    }

    static int Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "--gdboot") return RunGdBoot(args.Length > 1 ? args[1] : "../GodotSrc");
        if (args.Length > 0 && args[0] == "--gdauto") return RunGdAuto(args.Length > 1 ? args[1] : "../GodotSrc", args.Length > 2 ? args[2..] : new string[0]);
        if (args.Length > 0 && args[0] == "--gdtests") return RunGdTests(args.Length > 1 ? args[1] : "../GodotSrc");
        MiniJsonTests();
        string planPath = args.Length > 0 ? args[0] : "../Assets/StreamingAssets/plan.json";
        PlanTests(planPath);
        WriteTests(planPath);
        UpdateTests();
        Console.WriteLine();
        Console.WriteLine("celkem PASS " + pass + " / FAIL " + fail);
        return fail == 0 ? 0 : 1;
    }

    static void UpdateTests()
    {
        string h1 = new string('a', 64), h2 = new string('b', 64);
        string json = "{\"ver\": 202610251200, \"commit\": \"abc\", \"notes\": \"Test\", \"requires\": 1, \"files\": [" +
            "{\"path\": \"Managed/PivovarGdGen.dll\", \"sha256\": \"" + h1 + "\", \"size\": 100}," +
            "{\"path\": \"StreamingAssets/godot/art/x.png\", \"sha256\": \"" + h2 + "\", \"size\": 5}]}";
        var m = UpdateManifest.Parse(json);
        Chk("aktualizace: manifest se načte", m.Ver == 202610251200L && m.Files.Count == 2 && m.Requires == 1 && m.Notes == "Test");
        // jen změněné soubory: první shodný hash, druhý jiný
        var ch = UpdateLogic.Changed(m, p => p.EndsWith("dll") ? h1 : h1);
        Chk("aktualizace: stahují se jen soubory s jiným hashem", ch.Count == 1 && ch[0].Path.EndsWith("x.png"));
        var chMissing = UpdateLogic.Changed(m, p => null);
        Chk("aktualizace: chybějící soubor se stáhne", chMissing.Count == 2);
        var chNone = UpdateLogic.Changed(m, p => p.EndsWith("dll") ? h1 : h2);
        Chk("aktualizace: nic se nestahuje, když vše sedí", chNone.Count == 0);
        Chk("aktualizace: rozhodnutí – nová verze", UpdateLogic.Decide(m, 202610241200L, 1, 1) == 1);
        Chk("aktualizace: rozhodnutí – už mám novější nebo stejnou", UpdateLogic.Decide(m, 202610251200L, 1, 1) == 0 && UpdateLogic.Decide(m, 202610260000L, 1, 2) == 0);
        Chk("aktualizace: rozhodnutí – stejné soubory = nic", UpdateLogic.Decide(m, 1L, 1, 0) == 0);
        Chk("aktualizace: starý program (skripty) vyžaduje nové sestavení", UpdateLogic.Decide(m, 1L, 0, 1) == 2);
        // bezpečnost cest
        Chk("aktualizace: cesty „..“, absolutní a zpětná lomítka se odmítnou",
            !UpdateLogic.SafePath("../x.dll") && !UpdateLogic.SafePath("Managed/../../x") && !UpdateLogic.SafePath("/etc/x") && !UpdateLogic.SafePath("C:/x") && !UpdateLogic.SafePath("Managed\\x.dll") && !UpdateLogic.SafePath("Jine/x.dll") && UpdateLogic.SafePath("Managed/PivovarCore.dll"));
        bool threw = false;
        try { UpdateManifest.Parse(json.Replace("Managed/PivovarGdGen.dll", "../evil.dll")); } catch (FormatException) { threw = true; }
        Chk("aktualizace: manifest s nebezpečnou cestou se odmítne", threw);
        threw = false;
        try { UpdateManifest.Parse(json.Replace(h1, "abc")); } catch (FormatException) { threw = true; }
        Chk("aktualizace: manifest se špatným hashem se odmítne", threw);
        // hash
        string tmp = Path.Combine(Path.GetTempPath(), "upd_" + Environment.ProcessId + ".bin");
        File.WriteAllBytes(tmp, new byte[] { 1, 2, 3 });
        Chk("aktualizace: SHA-256 souboru = SHA-256 bajtů (známá hodnota)", UpdateLogic.Sha256File(tmp) == UpdateLogic.Sha256Bytes(new byte[] { 1, 2, 3 }) && UpdateLogic.Sha256Bytes(new byte[0]) == "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");
        File.Delete(tmp);
        // dávka
        var moves = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(@"C:\u\s\a.dll", @"C:\app\Managed\a.dll") };
        string cmd = UpdateLogic.BuildApplyScript(1234, @"C:\app\Pivovar.exe", @"C:\u\apply.log", moves);
        Chk("aktualizace: dávka čeká na ukončení, zálohuje, kopíruje, při chybě vrací a spouští program",
            cmd.Contains("PID eq 1234") && cmd.Contains("a.dll.bak") && cmd.Contains(":rollback") && cmd.Contains("start \"\" \"C:\\app\\Pivovar.exe\"") && cmd.IndexOf("copy /Y \"C:\\u\\s\\a.dll\"") > cmd.IndexOf(".bak"));
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

    static bool Same(object a, object b)
    {
        if (a == null || b == null) return a == b;
        if (a is double && b is double) return Math.Abs((double)a - (double)b) < 1e-9;
        if (a is string || a is bool) return a.Equals(b);
        var la = a as List<object>; var lb = b as List<object>;
        if (la != null && lb != null)
        {
            if (la.Count != lb.Count) return false;
            for (int i = 0; i < la.Count; i++) if (!Same(la[i], lb[i])) return false;
            return true;
        }
        var da = a as Dictionary<string, object>; var db = b as Dictionary<string, object>;
        if (da != null && db != null)
        {
            if (da.Count != db.Count) return false;
            foreach (var kv in da) { object o; if (!db.TryGetValue(kv.Key, out o) || !Same(kv.Value, o)) return false; }
            return true;
        }
        return false;
    }

    static void WriteTests(string path)
    {
        string src = File.ReadAllText(path);
        Plan p = Plan.Parse(src);
        string out1 = p.ToJson();
        Plan p2 = Plan.Parse(out1);
        Chk("zápis: plán po uložení a načtení je shodný (všechny klíče)", Same(p.Root, p2.Root));
        Chk("zápis: celá čísla bez desetinné tečky", out1.Contains("\"x\": 600,") || out1.Contains("\"x\": 600}"));
        Chk("zápis: počet položek zachován", p2.Items.Count == p.Items.Count, p2.Items.Count.ToString());

        // úprava bodu trubky se projeví v JSONu
        PlanItem pipe = null;
        foreach (var it in p.OfType("pipe")) { if (it.Pts().Count >= 2) { pipe = it; break; } }
        var pts = pipe.Pts(); pts[0][0] += 12; pts.Add(new double[] { 1, 2 });
        pipe.SetPts(pts);
        Plan p3 = Plan.Parse(p.ToJson());
        var q = p3.ById(pipe.Id).Pts();
        Chk("zápis: změněný bod trubky se uloží", Math.Abs(q[0][0] - pts[0][0]) < 1e-9 && q.Count == pts.Count);

        // přidání a smazání položky
        int n0 = p.Items.Count;
        var d = new Dictionary<string, object>(); d["type"] = "valve"; d["id"] = "valve_test"; d["x"] = 5.0; d["y"] = 6.0;
        p.AddItem(d);
        Plan p4 = Plan.Parse(p.ToJson());
        Chk("zápis: nová položka je v JSONu", p4.Items.Count == n0 + 1 && p4.ById("valve_test") != null);
        p.RemoveItem(p.ById("valve_test"));
        Chk("zápis: smazaná položka zmizí", Plan.Parse(p.ToJson()).Items.Count == n0);

        // znaky, které umí JSON: uvozovky, zpětné lomítko, nový řádek, česká písmena
        var tst = new Dictionary<string, object>(); tst["s"] = "a\"b\\c\nd á";
        object back = MiniJson.Parse(MiniJson.Write(tst, false));
        Chk("zápis: escape řetězců", Same(tst, back));
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
