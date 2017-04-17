using UnityEngine;
using UnityEditor;
using System.IO;

namespace SimpleSync
{
    [CustomEditor(typeof(SyncCenter))]
    public class SyncCenterEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            
            SerializedProperty iFrameRate = serializedObject.FindProperty("m_iSyncFrameRate");
            SerializedProperty kActivity = serializedObject.FindProperty("m_kAndroidActivity");
            SerializedProperty bWireless = serializedObject.FindProperty("m_bWirelessConnection");
            SerializedProperty kDeviceIP = serializedObject.FindProperty("m_kDeviceIPAddress");
            serializedObject.Update();
            EditorGUILayout.PropertyField(iFrameRate);

            if (EditorApplication.isPlaying)
            {
                if (SyncManager.SyncSceneID != -1)
                {
                    if (GUILayout.Button("Update Sync Frame Rate"))
                    {
                        SyncManager.SyncFrameRate = iFrameRate.intValue;
                        EditorSync.UpdateSyncFrameRate();
                    }
                }
                else
                {
                    if (GUILayout.Button("Request Sync Scene!"))
                    {
                        SyncManager.SyncFrameRate = iFrameRate.intValue;
                        EditorSync.StartSync();
                        EditorWindow.GetWindow(typeof(SyncControllWindow));
                    }
                }
            }
            else
            {
                EditorGUILayout.PropertyField(kActivity);
                if (GUILayout.Button("Start Android Activity"))
                {
                    StartAndroidApp(kActivity.stringValue);
                }
                EditorGUILayout.PropertyField(bWireless);
                if (bWireless.boolValue)
                {
                    EditorGUILayout.PropertyField(kDeviceIP);
                    if(GUILayout.Button("Connect Device"))
                    {
                        ConnectDeviceIP(kDeviceIP.stringValue);
                    }
                }
                if (GUILayout.Button("ADB Devices"))
                {
                    EditorLoader.RunAdbCommand(@" devices", true);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        void StartAndroidApp(string kAndroidActivity)
        {
            string arguments = @" shell am start -n " + PlayerSettings.bundleIdentifier + "/" + kAndroidActivity;
            EditorLoader.RunAdbCommand(arguments, true);
        }

        void ConnectDeviceIP(string kIPAddress)
        {
            string arguments = @" connect " + kIPAddress;
            EditorLoader.RunAdbCommand(arguments, true);
        }

    }
}
