// Aktualizace bez nového stahování – čistá logika bez Unity (testuje se v CI):
//  manifest.json (seznam souborů + SHA-256) → které soubory se liší → skript, který je po zavření programu nahradí a program spustí znovu.
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Pivovar.Core
{
    public sealed class UpdateFile
    {
        public string Path;     // relativně k <program>_Data, s lomítky "/", např. "Managed/PivovarGdGen.dll"
        public string Sha256;   // malá písmena hex
        public long Size;
    }

    public sealed class UpdateManifest
    {
        public long Ver;                 // rostoucí číslo verze (UTC čas sestavení yyyyMMddHHmm)
        public string Notes = "";
        public string Commit = "";
        public int Requires;             // minimální revize skriptů v programu (UpdateClient.ScriptsRev)
        public List<UpdateFile> Files = new List<UpdateFile>();

        public static UpdateManifest Parse(string json)
        {
            var d = MiniJson.Parse(json) as Dictionary<string, object>;
            if (d == null) throw new FormatException("manifest: kořen není objekt");
            var m = new UpdateManifest();
            m.Ver = (long)Num(d, "ver");
            m.Notes = Str(d, "notes");
            m.Commit = Str(d, "commit");
            m.Requires = (int)Num(d, "requires");
            var arr = d.ContainsKey("files") ? d["files"] as List<object> : null;
            if (arr == null) throw new FormatException("manifest: chybí files");
            foreach (var o in arr)
            {
                var f = o as Dictionary<string, object>;
                if (f == null) throw new FormatException("manifest: špatná položka");
                var uf = new UpdateFile { Path = Str(f, "path"), Sha256 = Str(f, "sha256").ToLowerInvariant(), Size = (long)Num(f, "size") };
                if (!UpdateLogic.SafePath(uf.Path)) throw new FormatException("manifest: nebezpečná cesta " + uf.Path);
                if (uf.Sha256.Length != 64) throw new FormatException("manifest: špatný hash u " + uf.Path);
                m.Files.Add(uf);
            }
            return m;
        }

        static double Num(Dictionary<string, object> d, string k) { object v; return d.TryGetValue(k, out v) && v is double ? (double)v : 0.0; }
        static string Str(Dictionary<string, object> d, string k) { object v; return d.TryGetValue(k, out v) && v is string ? (string)v : ""; }
    }

    public static class UpdateLogic
    {
        /// <summary>Povolené jsou jen relativní cesty bez „..“, bez disku a bez zpětných lomítek; jen Managed/ a StreamingAssets/.</summary>
        public static bool SafePath(string p)
        {
            if (string.IsNullOrEmpty(p) || p.Length > 240) return false;
            if (p.IndexOf('\\') >= 0 || p.IndexOf(':') >= 0 || p[0] == '/') return false;
            foreach (var part in p.Split('/')) if (part.Length == 0 || part == "." || part == "..") return false;
            return p.StartsWith("Managed/", StringComparison.Ordinal) || p.StartsWith("StreamingAssets/", StringComparison.Ordinal);
        }

        public static string Sha256File(string path)
        {
            using (var s = File.OpenRead(path)) return Hex(SHA256.Create().ComputeHash(s));
        }

        public static string Sha256Bytes(byte[] b) { return Hex(SHA256.Create().ComputeHash(b)); }

        static string Hex(byte[] h)
        {
            var sb = new StringBuilder(h.Length * 2);
            foreach (var x in h) sb.Append(x.ToString("x2"));
            return sb.ToString();
        }

        /// <summary>Soubory z manifestu, které se v instalaci liší (chybí nebo mají jiný hash). localHash(cesta) vrací null, když soubor není.</summary>
        public static List<UpdateFile> Changed(UpdateManifest m, Func<string, string> localHash)
        {
            var r = new List<UpdateFile>();
            foreach (var f in m.Files)
            {
                var h = localHash(f.Path);
                if (h == null || !string.Equals(h, f.Sha256, StringComparison.OrdinalIgnoreCase)) r.Add(f);
            }
            return r;
        }

        /// <summary>Rozhodnutí: 0 = nic nového, 1 = k dispozici, 2 = vyžaduje novější program (úplné stažení).</summary>
        public static int Decide(UpdateManifest m, long installedVer, int scriptsRev, int changedCount)
        {
            if (m.Requires > scriptsRev) return 2;
            if (m.Ver <= installedVer || changedCount == 0) return 0;
            return 1;
        }

        /// <summary>Dávka pro Windows: počká na ukončení programu, soubory nahradí (se zálohou .bak), při chybě vrátí zálohy a program spustí znovu.</summary>
        public static string BuildApplyScript(int pid, string exePath, string logPath, IList<KeyValuePair<string, string>> moves)
        {
            var sb = new StringBuilder();
            sb.Append("@echo off\r\nchcp 65001 >nul\r\nsetlocal\r\n");
            sb.Append("set LOG=\"").Append(logPath).Append("\"\r\n");
            sb.Append("echo [%date% %time%] start aktualizace > %LOG%\r\n");
            sb.Append("set /a N=0\r\n:wait\r\n");
            sb.Append("tasklist /FI \"PID eq ").Append(pid).Append("\" 2>nul | find \"").Append(pid).Append("\" >nul\r\n");
            sb.Append("if %ERRORLEVEL%==0 (\r\n  set /a N+=1\r\n  if %N% GEQ 60 goto fail\r\n  timeout /t 1 /nobreak >nul\r\n  goto wait\r\n)\r\n");
            int i = 0;
            foreach (var mv in moves)
            {
                i++;
                sb.Append("if exist \"").Append(mv.Value).Append("\" copy /Y \"").Append(mv.Value).Append("\" \"").Append(mv.Value).Append(".bak\" >nul\r\n");
                sb.Append("copy /Y \"").Append(mv.Key).Append("\" \"").Append(mv.Value).Append("\" >nul\r\n");
                sb.Append("if errorlevel 1 (echo kopirovani selhalo: ").Append(mv.Value).Append(" >> %LOG% & goto rollback)\r\n");
            }
            sb.Append("echo hotovo >> %LOG%\r\ngoto run\r\n");
            sb.Append(":rollback\r\n");
            foreach (var mv in moves)
                sb.Append("if exist \"").Append(mv.Value).Append(".bak\" copy /Y \"").Append(mv.Value).Append(".bak\" \"").Append(mv.Value).Append("\" >nul\r\n");
            sb.Append("echo vraceno zpet >> %LOG%\r\ngoto run\r\n");
            sb.Append(":fail\r\necho program se neukoncil >> %LOG%\r\n");
            sb.Append(":run\r\nstart \"\" \"").Append(exePath).Append("\"\r\nendlocal\r\nexit /b 0\r\n");
            return sb.ToString();
        }
    }
}
