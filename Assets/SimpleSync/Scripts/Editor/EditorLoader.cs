using UnityEditor;
using UnityEngine;
using System.IO;

using SimpleSync;

[InitializeOnLoad]
public class EditorLoader
{
    static string ANDROID_ACTIVITY_KEY = "SimpleSync_StartActivity";

    static EditorLoader()
    {
        EditorApplication.playmodeStateChanged += PlayModeChanged;
        EditorApplication.hierarchyWindowItemOnGUI += SimplySyncHierarchyWindowItemCallback;
    }

    static void PlayModeChanged()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            RunAdbCommand(@" forward tcp:15556 tcp:15555");
        }
    }

    static void SimplySyncHierarchyWindowItemCallback(int instanceID, Rect selectionRect)
    {
        if (EditorApplication.isPlaying && (SyncManager.IsD2ESyncStart || SyncManager.IsE2DSyncStart))
        {
            SyncControllWindow.RedrawHierachy(instanceID, selectionRect);
        }
    }

    public static void RunAdbCommand(string arguments, bool bShowLog = false)
    {
        System.Diagnostics.Process p = new System.Diagnostics.Process();
        p.StartInfo.FileName = EditorPrefs.GetString("AndroidSdkRoot") + "/platform-tools/adb.exe";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.RedirectStandardOutput = bShowLog;
        p.StartInfo.RedirectStandardError = bShowLog;
        p.StartInfo.Arguments = arguments;
        p.Start();
        StreamReader outputStreamReader = null, errorStreamReader = null;
        if (bShowLog)
        {
            outputStreamReader = p.StandardOutput;
            errorStreamReader = p.StandardError;
        }
        p.WaitForExit();
        if (bShowLog)
        {
            string outputMessage = outputStreamReader.ReadToEnd();
            string errorMessage = errorStreamReader.ReadToEnd();
            if (!string.IsNullOrEmpty(outputMessage)) Debug.Log(outputMessage);
            if (!string.IsNullOrEmpty(errorMessage)) Debug.LogError(errorMessage);
        }
        p.Close();
    }

}
