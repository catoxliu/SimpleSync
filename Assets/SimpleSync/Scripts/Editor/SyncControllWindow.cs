using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Collections.Generic;

using SimpleSync.Serializer;

namespace SimpleSync
{
    public class SyncControllWindow : EditorWindow
    {
        static Dictionary<int, string> m_kSceneObjectDic = new Dictionary<int, string>();
        static Dictionary<int, GameObject> m_kSyncObjects = new Dictionary<int, GameObject>();
        static Dictionary<int, GameObject> m_kControlObjects = new Dictionary<int, GameObject>();

        public static void RedrawHierachy(int instanceID, Rect selectionRect)
        {
            if (m_kSyncObjects.ContainsKey(instanceID))
            {
                GUI.color = Color.yellow;
                Rect newRect = new Rect(selectionRect);
                newRect.x = selectionRect.x - 10;
                GUI.Label(newRect, "*");
                GUI.color = Color.white;
            }
            else if (m_kControlObjects.ContainsKey(instanceID))
            {
                GUI.color = Color.red;
                Rect newRect = new Rect(selectionRect);
                newRect.x = selectionRect.x - 10;
                GUI.Label(selectionRect, "*");
                GUI.color = Color.white;
            }
        }

        void Awake()
        {
            titleContent.text = "Simple Sync";
        }

        void OnGUI()
        {
            if (EditorApplication.isPlaying)
            {
                if (!EditorSync.IsConnected)
                {
                    ShowMessage("Simply Sync didn't connect to the device!", true);
                }
                else if(SyncManager.SyncSceneID == -1)
                {
                    GameObject go = GameObject.Find("SimpleSync");
                    if (go != null)
                    {
                        Selection.activeGameObject = go;
                        ShowMessage("Try to Request Sync Scene!");
                    }
                    else
                    {
                        ShowMessage("Can't find SyncCenter! Make sure you use Sync Scene!", true);
                    }
                }
                else
                {
                    ShowSyncPanel();
                }
            }
            else
            {
                ShowConfigPanel();
            }
        }

        void OnInspectorUpdate()
        {
            if (EditorApplication.isPlaying)
                Repaint();
        }

        void OnHierarchyChange()
        {
            if (EditorApplication.isPlaying)
            {
                Scene activeScene = SceneManager.GetActiveScene();
                GameObject[] rootGOs = activeScene.GetRootGameObjects();
                foreach (var rootGO in rootGOs)
                {
                    GetSceneGameName(rootGO.transform);
                }
            }
        }

        void OnSelectionChange()
        {
            if (EditorApplication.isPlaying)
                Repaint();
        }

        void ShowConfigPanel()
        {
            ShowMessage("Please use scene SyncMain.unity to start Simply sync.");
        }

        void ShowSyncPanel()
        {
            bool bShowButtons = false;
            if (Selection.instanceIDs.Length > 0)
            {
                foreach (var id in Selection.instanceIDs)
                {
                    if (m_kSceneObjectDic.ContainsKey(id))
                    {
                        if (!bShowButtons)
                        {
                            bShowButtons = true;
                            ShowMessage("GameObjects Selected : ");
                        }
                        ShowMessage(m_kSceneObjectDic[id]);
                    }
                }
            }
            EditorGUILayout.BeginHorizontal();
            if (SyncManager.IsD2ESyncStart) ShowStopSyncButton();
            else if (bShowButtons) ShowStartSyncButton();
            if (SyncManager.IsE2DSyncStart) ShowStopControlButton();
            else if (bShowButtons) ShowStartControlButton();
            EditorGUILayout.EndHorizontal();

            if (!bShowButtons && !SyncManager.IsD2ESyncStart && !SyncManager.IsE2DSyncStart)
            {
                ShowMessage("Select Objects in scene to Sync or Control!");
            }
        }

        void GetSceneGameName(Transform go, string parent = "")
        {
            string objectName = parent + go.name;
            int objectID = go.gameObject.GetInstanceID();
            if (!m_kSceneObjectDic.ContainsKey(objectID))
            {
                m_kSceneObjectDic.Add(objectID, objectName);
            }
            if (go.childCount > 0)
            {
                parent += go.name + "/";
                for (int i = 0, max = go.childCount; i < max; i++)
                {
                    GetSceneGameName(go.transform.GetChild(i), parent);
                }
            }
        }

        void ShowStopSyncButton()
        {
            if (GUILayout.Button("Stop D2E Sync!"))
            {
                EditorSync.StopSync();
                m_kSyncObjects.Clear();
            }
        }

        void ShowStartSyncButton()
        {
            if (GUILayout.Button("Request D2E Sync"))
            {
                SyncManager.ResetD2ESyncObjects();
                SelectObjects(ref m_kSyncObjects, SyncManager.AddD2ESyncObject, SyncManager.StartD2ESync);
            }
        }

        void ShowStopControlButton()
        {
            if (GUILayout.Button("Stop E2D Control!"))
            {
                EditorSync.StopControl();
                m_kControlObjects.Clear();
            }
        }

        void ShowStartControlButton()
        {
            if (GUILayout.Button("Request E2D Control"))
            {
                SyncManager.ResetE2DControlObjects();
                SelectObjects(ref m_kControlObjects, SyncManager.AddE2DControlObject, SyncManager.StartE2DControl);
            }
        }

        void SelectObjects(ref Dictionary<int, GameObject> selectedList, System.Action<int, GameObject> callback, System.Action<byte[]> send)
        {
            List<string> selectObjects = new List<string>();
            selectedList.Clear();
            int index = 0;
            foreach (var id in Selection.instanceIDs)
            {
                if (m_kSceneObjectDic.ContainsKey(id))
                {
                    GameObject selectedObject = GameObject.Find(m_kSceneObjectDic[id]);
                    callback(index, selectedObject);
                    index++;
                    selectObjects.Add(m_kSceneObjectDic[id]);
                    selectedList.Add(id, selectedObject);
                }
            }

            send(FBHelper.SerializeSyncIDList(selectObjects));
        }

        void ShowMessage(string msg, bool warning = false)
        {
            if (warning) GUI.color = Color.red;
            GUILayout.Label(msg);
            if (warning) GUI.color = Color.white;
        }

    }
}

