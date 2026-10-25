// Menu Pivovar > Sestavit .exe: sestaví samostatnou aplikaci na plochu (bez Unity).
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.Build;

namespace Pivovar.EditorTools
{
    public static class BuildExe
    {
        [MenuItem("Pivovar/Sestavit .exe na plochu")]
        public static void Build()
        {
            string dir = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory), "Pivovar-HDRP-aplikace");
            Directory.CreateDirectory(dir);
            string scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;
            if (string.IsNullOrEmpty(scene)) scene = "Assets/Scenes/SampleScene.unity";
            // Aktualizace bez nového stahování potřebuje Mono (knihovny PivovarCore.dll / PivovarGdGen.dll se při aktualizaci vyměňují)
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
            // časová značka sestavení: podle ní program pozná, jestli je aktualizace novější než on sám
            Directory.CreateDirectory(Application.streamingAssetsPath);
            File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "build_stamp.txt"), System.DateTime.UtcNow.ToString("yyyyMMddHHmm"));
            var opts = new BuildPlayerOptions
            {
                scenes = new[] { scene },
                locationPathName = Path.Combine(dir, "Pivovar.exe"),
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };
            var rep = BuildPipeline.BuildPlayer(opts);
            Debug.Log("Pivovar build: " + rep.summary.result + " -> " + opts.locationPathName + " (" + rep.summary.totalErrors + " chyb)");
        }
    }
}
#endif
