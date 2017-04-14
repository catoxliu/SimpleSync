using UnityEditor;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public class EditorLoader
{
    static string m_kAndroidActivity = "SimpleSync_StartActivity";

    static EditorLoader()
    {
        EditorApplication.playmodeStateChanged += PlayModeChanged;
    }

    static void PlayModeChanged()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = EditorPrefs.GetString("AndroidSdkRoot") + "/platform-tools/adb.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Arguments = @" forward tcp:15556 tcp:15555";
            p.Start();
            p.WaitForExit();
            p.Close();
        }
    }

    [MenuItem("SimpleSync/Start Android App")]
    static void StartAndroidApp()
    {
        System.Diagnostics.Process p = new System.Diagnostics.Process();
        p.StartInfo.FileName = EditorPrefs.GetString("AndroidSdkRoot") + "/platform-tools/adb.exe";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.Arguments = @" shell am start -n " + PlayerSettings.bundleIdentifier + "/" + EditorPrefs.GetString(m_kAndroidActivity, "com.unity3d.player.UnityPlayerActivity");
        p.Start();
        StreamReader outputStreamReader = p.StandardOutput;
        StreamReader errorStreamReader = p.StandardError;
        p.WaitForExit();
        string outputMessage = outputStreamReader.ReadToEnd();
        string errorMessage = errorStreamReader.ReadToEnd();
        if (!string.IsNullOrEmpty(outputMessage)) Debug.Log(outputMessage);
        if (!string.IsNullOrEmpty(errorMessage)) Debug.LogError(errorMessage);
        p.Close();
    }
}
