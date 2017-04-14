using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

using SimpleSync.Serializer;

namespace SimpleSync
{
    public class SyncManager
    {
        public static bool IsSyncStart = false;

        private static int m_iSyncSceneID = -1;
        private static Dictionary<int, GameObject> m_dSyncObjectDic = new Dictionary<int, GameObject>();

        public static void Init()
        {
            if (SyncCenter.Instance == false)
            {
                new GameObject("SimpleSync").AddComponent<SyncCenter>();
            }
        }

        public static int SyncSceneID
        {
            get
            {
                return m_iSyncSceneID;
            }
            set
            {
                if (m_iSyncSceneID != value)
                {
                    m_iSyncSceneID = value;
                    SceneManager.LoadScene(value);
                }
            }
        }

        public static int SyncObjectsCount
        {
            get { return m_dSyncObjectDic.Count; }
        }

        public static void StartSync(byte[] data)
        {
            //Debug.Log(data.Length);
            EditorSync.SendSyncRequest(data);
        }

        public static void AddSyncObject(int index, GameObject go)
        {
            m_dSyncObjectDic[index] = go;
        }

        public static void ResetSyncObjects()
        {
            m_dSyncObjectDic.Clear();
        }

        public static GameObject GetObjectByID(int id)
        {
            if (m_dSyncObjectDic.ContainsKey(id))
            {
                return m_dSyncObjectDic[id];
            }
            return null;
        }

        public static void FindSyncObjects(Dictionary<int, string> requestSyncObjectList)
        {
            m_dSyncObjectDic.Clear();
            foreach (var item in requestSyncObjectList)
            {
                Debug.Log("Try to find GameObject : " + item.Value);
                GameObject go = GameObject.Find(item.Value);
                if (go == null)
                {
                    Debug.LogError("Can't find GameObject : " + item.Value);
                }
                else
                {
                    m_dSyncObjectDic.Add(item.Key, go);
                }
            }
        }

        public static byte[] GetSyncData()
        {
            return FBHelper.SerializeSyncObject(m_dSyncObjectDic);
        }

    }

}
