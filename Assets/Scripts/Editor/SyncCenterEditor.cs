using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace SimpleSync
{
    [CustomEditor(typeof(SyncCenter))]
    public class SyncCenterEditor : Editor
    {
        static Dictionary<string, bool> m_kSyncObjectList = new Dictionary<string, bool>();
        

        public override void OnInspectorGUI()
        {
            //serializedObject.Update();
            //EditorGUILayout.PropertyField(lookAtPoint);
            //serializedObject.ApplyModifiedProperties();
            if (EditorApplication.isPlaying)
            {
                if (SyncManager.IsSyncStart)
                {
                    ShowStopSyncButton();
                }
                else if (SyncManager.SyncSceneID != -1)
                {
                    ShowSyncPanel();
                }
                else
                {
                    if (GUILayout.Button("Request Sync Scene!"))
                    {
                        EditorSync.StartSync();
                    }
                }
            }
            else
            {
                //Use for Debug;
                ShowSyncPanel();
            }
        }

        void ShowSyncPanel()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            GameObject[] rootGOs = activeScene.GetRootGameObjects();
            foreach (var rootGO in rootGOs)
            {
                DrawGameObject(rootGO.transform);
            }
            if (GUILayout.Button("Request Sync"))
            {
                SyncManager.ResetSyncObjects();
                List<string> syncObjects = new List<string>();
                int index = 0;
                foreach (var item in m_kSyncObjectList)
                {
                    if (item.Value)
                    {
                        SyncManager.AddSyncObject(index, GameObject.Find(item.Key));
                        index++;
                        syncObjects.Add(item.Key);
                    }
                }
                SyncManager.StartSync(SimpleSync.Serializer.FBHelper.SerializeSyncIDList(syncObjects));
            }
        }

        void DrawGameObject(Transform go, string parent = "")
        {
            if (go.gameObject.isStatic) return;
            string key = parent + go.name;
            if (!m_kSyncObjectList.ContainsKey(key))
            {
                m_kSyncObjectList.Add(key, false);
            }
            m_kSyncObjectList[key] = EditorGUILayout.Toggle(key, m_kSyncObjectList[key]);
            if (go.childCount > 0)
            {
                parent += go.name + "/";
                for (int i = 0, max = go.childCount; i < max; i++)
                {
                    DrawGameObject(go.transform.GetChild(i), parent);
                }
            }
        }

        void ShowStopSyncButton()
        {
            if (GUILayout.Button("Stop Sync!"))
            {
                EditorSync.StopSync();
            }
        }
    }
}
