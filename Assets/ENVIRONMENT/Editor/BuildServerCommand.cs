using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildServerCommand : MonoBehaviour
{
    [MenuItem("Build/Build and Run Server")]
    public static void MyBuildServer()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] {"Assets/GAME/Scenes/Main.unity", "Assets/GAME/Scenes/Server.unity"};
        buildPlayerOptions.locationPathName = System.IO.Path.Combine("Build", "Server", "ServerBuild.exe");
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.AutoRunPlayer;
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone,
            ProjectsBuildSymbols.SERVER.ToString());

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;


        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }


    [MenuItem("Build/Build and Run Client | PC Local")]
    public static void MyBuildDemoClient()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] {"Assets/GAME/Scenes/Main.unity", "Assets/GAME/Scenes/Client.unity"};
        buildPlayerOptions.locationPathName = System.IO.Path.Combine("Build", "WIFI_PC_Client", "ClientBuild.exe");
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.AutoRunPlayer;
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone,
            ProjectsBuildSymbols.DEBUG_CLIENT.ToString());

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;


        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }
    
    

    [MenuItem("Build/Build and Run Client | Android WIFI ")]
    public static void MyBuildAndroidDebugClient()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] {"Assets/GAME/Scenes/Main.unity", "Assets/GAME/Scenes/Client.unity"};
        buildPlayerOptions.locationPathName = System.IO.Path.Combine("Build", "Wifi_Android_Client", "ClientBuild.apk");
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.AutoRunPlayer;
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android,
            ProjectsBuildSymbols.DEBUG_CLIENT.ToString());

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;


        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }
}