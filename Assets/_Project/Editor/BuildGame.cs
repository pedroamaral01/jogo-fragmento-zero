using UnityEditor;
using UnityEngine;
using System.IO;

public static class BuildGame
{
    [MenuItem("FragmentoZero/Build Game (Windows)")]
    static void BuildWindows()
    {
        string buildDir = "Builds/Windows";
        Directory.CreateDirectory(buildDir);

        var options = new BuildPlayerOptions
        {
            scenes = ScenesToBuild(),
            locationPathName = Path.Combine(buildDir, "FragmentoZero.exe"),
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log($"Build concluído: {options.locationPathName} ({report.summary.totalSize / 1024 / 1024} MB)");
            EditorUtility.RevealInFinder(options.locationPathName);
        }
        else
        {
            Debug.LogError($"Build falhou: {report.summary.result}");
        }
    }

    static string[] ScenesToBuild()
    {
        var scenes = EditorBuildSettings.scenes;
        if (scenes.Length > 0)
        {
            var paths = new string[scenes.Length];
            for (int i = 0; i < scenes.Length; i++) paths[i] = scenes[i].path;
            return paths;
        }
        return new[] { UnityEngine.SceneManagement.SceneManager.GetActiveScene().path };
    }
}
