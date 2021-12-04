using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

class AutomatedTesting
{
    static string[] SCENES = FindEnabledEditorScenes();
    
    static string APP_NAME = "theMalevolence";
    static string TARGET_DIR = "target";

    static void PerformAllBuilds()
    {
        PerformMacOSXBuild();
        PerformWindowsBuild();
    }


    [MenuItem("Custom/CI/Build Mac OS X")]
    static void PerformMacOSXBuild()
    {
        string target_dir = APP_NAME + ".app";
        GenericBuild(SCENES, TARGET_DIR + "/" + target_dir, BuildTarget.StandaloneOSXIntel, BuildOptions.None);
    }

    static void PerformWindowsBuild()
    {
        string target_dir = APP_NAME + ".exe";
        GenericBuild(SCENES, TARGET_DIR + "/" + target_dir, BuildTarget.StandaloneWindows, BuildOptions.None);
    }
/*
    static void PerformAndroidBuild()
    {
        //Set the path to the Android SDK on the machine, since Unity cannot retain the state properly
        AndroidSDKFolder.Path = "${ANDROID_HOME}";
        string target_dir = APP_NAME + ".apk";
        GenericBuild(SCENES, TARGET_DIR + "/" + target_dir, BuildTarget.Android, BuildOptions.None);
    }
    */
    private static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }

    static void GenericBuild(string[] scenes, string target_dir, BuildTarget build_target, BuildOptions build_options)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(build_target);
        string res = $"{ BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options)}";
        if (res.Length > 0)
        {
            throw new Exception("BuildPlayer failure: " + res);
        }
    }
}
