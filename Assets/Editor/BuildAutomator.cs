using System;
using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Debug = UnityEngine.Debug;

namespace Project.Editor
{
    public static class BuildAutomator
    {
        readonly struct Build
        {
            public readonly BuildTarget target;
            public readonly string folderName;
            
            public Build(BuildTarget t, string f)
            {
                target = t;
                folderName = f;
            }
        }
        
        const string ContentPath = @"C:\dev\sdk\tools\ContentBuilder\content";
        const string SteamUploaderBatch = @"C:\dev\sdk\tools\ContentBuilder\run-build.bat";

        [MenuItem("File/Expedited Build")]
        public static void BuildAll() => BuildGame(
            new Build(BuildTarget.StandaloneWindows64, @"win\Counter Chaos.exe"),
            new Build(BuildTarget.StandaloneOSX, "mac.app"));
        
        [MenuItem("File/Expedited Mac Build")]
        public static void BuildMac() => BuildGame(new Build(BuildTarget.StandaloneOSX, "mac.app"));

        static void BuildGame(params Build[] builds)
        {
            var buildScenes = EditorBuildSettings.scenes;
            var scenes = new string[buildScenes.Length];
            for (var i = 0; i < buildScenes.Length; i++)
                scenes[i] = buildScenes[i].path;
            
            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                options = BuildOptions.None,
            };
            
            foreach (var build in builds)
                BuildFor(build.target, options, build.folderName);

            var proc = new Process();
            proc.StartInfo.FileName = SteamUploaderBatch;
            proc.Start();
        }

        static void BuildFor(BuildTarget target, BuildPlayerOptions options, string folderName)
        {
            Debug.Log("Started build for " + target);
            
            options.target = target;
            options.locationPathName = ContentPath + @$"\{folderName}";
            var summary = BuildPipeline.BuildPlayer(options).summary;

            if (summary.result != BuildResult.Succeeded) throw new Exception("Build failed for " + target);
            
            Debug.Log($"Build succeeded for {target.ToString()}: " + summary.totalSize + " bytes");
        }
    }
}
