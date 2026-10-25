// Menu Pivovar > Sestavit .exe: sestaví samostatnou aplikaci na plochu (bez Unity).
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

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
