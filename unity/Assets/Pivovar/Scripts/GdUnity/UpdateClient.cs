// Aktualizace bez nového stahování (Windows, Mono):
//  1) stáhne manifest.json z GitHubu (větev unity-update, ji plní CI po každé úspěšné sestavě),
//  2) porovná SHA-256 souborů v instalaci (<program>_Data/Managed/PivovarCore.dll, PivovarGdGen.dll, StreamingAssets/…),
//  3) stáhne JEN soubory, které se liší, ověří velikost i hash,
//  4) po zavření programu je dávka nahradí (záloha .bak, při chybě návrat) a program spustí znovu.
// Tlačítko „⟳“ v HUD (updater.gd) volá UnityUpdate.start/poll přes Gd.UpdateHook.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Pivovar.Core;
using Pivovar.Gd;

namespace Pivovar.GdUnity
{
    public sealed class UpdateClient : MonoBehaviour
    {
        /// <summary>Revize skriptů v programu (Assembly-CSharp). Zvyšuje se jen když nové knihovny vyžadují novější skripty – pak je nutné program sestavit znovu.</summary>
        public const int ScriptsRev = 1;
        public const string BaseUrl = "https://raw.githubusercontent.com/janvasilecko-droid/vasil/unity-update/";

        static UpdateClient inst;
        readonly Queue<string> msgs = new Queue<string>();
        bool busy;

        public static void Install(GameObject host)
        {
            if (inst != null) return;
            inst = host.AddComponent<UpdateClient>();
            Pivovar.Gd.Gd.UpdateHook = inst.Call;
        }

        Gv Call(string name, Gv[] a)
        {
            switch (name)
            {
                case "poll": return Gv.S(msgs.Count > 0 ? msgs.Dequeue() : "");
                case "start": Begin(a.Length > 0 && a[0].T()); return Gv.Null;
                case "ver": return Gv.I((int)(InstalledVer() % 1000000000L));
            }
            return Gv.Null;
        }

        void Say(string kind, string text) { msgs.Enqueue("status|" + kind + "|" + text); UnityEngine.Debug.Log("[aktualizace] " + text); }

        static string DataDir { get { return Application.dataPath; } }
        static string WorkDir { get { return Path.Combine(Application.persistentDataPath, "update"); } }

        static long InstalledVer()
        {
            long v = 0;
            try
            {
                string st = Path.Combine(WorkDir, "installed.json");
                if (File.Exists(st)) { var d = MiniJson.Parse(File.ReadAllText(st)) as Dictionary<string, object>; if (d != null && d.ContainsKey("ver") && d["ver"] is double) v = (long)(double)d["ver"]; }
                string bs = Path.Combine(Application.streamingAssetsPath, "build_stamp.txt");
                long b; if (File.Exists(bs) && long.TryParse(File.ReadAllText(bs).Trim(), out b)) v = Math.Max(v, b);
            }
            catch (Exception) { }
            return v;
        }

        void Begin(bool download)
        {
            if (busy) return;
            busy = true;
            StartCoroutine(Run(download));
        }

        IEnumerator Run(bool download)
        {
            try
            {
                if (download) Say("info", "Hledám novou verzi…");
                string text = null;
                using (var rq = UnityWebRequest.Get(BaseUrl + "manifest.json?t=" + DateTime.UtcNow.Ticks))
                {
                    rq.timeout = 20;
                    yield return rq.SendWebRequest();
                    if (rq.result != UnityWebRequest.Result.Success)
                    {
                        if (download) Say("warn", "Aktualizace: server neodpověděl (" + rq.error + ") – zkontroluj internet.");
                        yield break;
                    }
                    text = rq.downloadHandler.text;
                }
                UpdateManifest m;
                try { m = UpdateManifest.Parse(text); }
                catch (Exception e) { Say("warn", "Aktualizace: špatný manifest (" + e.Message + ")."); yield break; }

                var changed = UpdateLogic.Changed(m, p =>
                {
                    string f = Path.Combine(DataDir, p);
                    return File.Exists(f) ? UpdateLogic.Sha256File(f) : null;
                });
                long inst_ = InstalledVer();
                int dec = UpdateLogic.Decide(m, inst_, ScriptsRev, changed.Count);
                if (dec == 2)
                {
                    if (download) Say("warn", "Tahle aktualizace vyžaduje novější program (úplné stažení / nové sestavení .exe).");
                    yield break;
                }
                if (dec == 0)
                {
                    if (download) Say("info", "Máš nejnovější verzi.");
                    yield break;
                }
                msgs.Enqueue("avail|" + (int)(m.Ver % 1000000000L) + "|");
                if (!download) yield break;

                long total = 0; foreach (var f in changed) total += f.Size;
                Say("info", "Stahuji " + changed.Count + " změněných souborů (" + (total / 1024) + " kB)…");
                string stage = Path.Combine(WorkDir, "staged");
                if (Directory.Exists(stage)) Directory.Delete(stage, true);
                Directory.CreateDirectory(stage);
                foreach (var f in changed)
                {
                    byte[] body = null;
                    using (var rq = UnityWebRequest.Get(BaseUrl + "files/" + f.Path + "?v=" + m.Ver))
                    {
                        rq.timeout = 180;
                        yield return rq.SendWebRequest();
                        if (rq.result != UnityWebRequest.Result.Success) { Say("warn", "Stažení selhalo u " + f.Path + " (" + rq.error + ")."); yield break; }
                        body = rq.downloadHandler.data;
                    }
                    if (body == null || body.LongLength != f.Size || UpdateLogic.Sha256Bytes(body) != f.Sha256)
                    {
                        Say("warn", "Soubor " + f.Path + " je poškozený nebo neúplný – nic se nezměnilo, zkus to znovu.");
                        yield break;
                    }
                    string dst = Path.Combine(stage, f.Path.Replace('/', Path.DirectorySeparatorChar));
                    Directory.CreateDirectory(Path.GetDirectoryName(dst));
                    File.WriteAllBytes(dst, body);
                }
                File.WriteAllText(Path.Combine(stage, "installed.json"), "{\"ver\": " + m.Ver + "}");

                if (Application.platform != RuntimePlatform.WindowsPlayer)
                {
                    Say("info", "Soubory jsou stažené (" + stage + "), automatické nahrazení funguje jen ve Windows verzi programu.");
                    yield break;
                }
                // zápis do složky programu musí jít (ne Program Files bez práv)
                try { string probe = Path.Combine(DataDir, "Managed", ".zapis"); File.WriteAllText(probe, "x"); File.Delete(probe); }
                catch (Exception) { Say("warn", "Do složky programu nejde zapisovat (" + DataDir + ") – přesuň program do běžné složky (např. Plocha)."); yield break; }

                var moves = new List<KeyValuePair<string, string>>();
                foreach (var f in changed)
                    moves.Add(new KeyValuePair<string, string>(Path.Combine(stage, f.Path.Replace('/', Path.DirectorySeparatorChar)), Path.Combine(DataDir, f.Path.Replace('/', Path.DirectorySeparatorChar))));
                moves.Add(new KeyValuePair<string, string>(Path.Combine(stage, "installed.json"), Path.Combine(WorkDir, "installed.json")));
                string exe = Process.GetCurrentProcess().MainModule.FileName;
                string cmd = Path.Combine(WorkDir, "apply.cmd");
                File.WriteAllText(cmd, UpdateLogic.BuildApplyScript(Process.GetCurrentProcess().Id, exe, Path.Combine(WorkDir, "apply.log"), moves), new UTF8Encoding(false));
                Say("info", "Verze " + m.Ver + " stažena – program se restartuje…");
                yield return new WaitForSeconds(1.5f);
                var psi = new ProcessStartInfo("cmd.exe", "/c \"\"" + cmd + "\"\"") { CreateNoWindow = true, UseShellExecute = false, WindowStyle = ProcessWindowStyle.Hidden };
                Process.Start(psi);
                Application.Quit();
            }
            finally { busy = false; }
        }
    }
}
