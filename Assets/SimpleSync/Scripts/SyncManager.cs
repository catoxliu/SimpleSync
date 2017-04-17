using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

using SimpleSync.Serializer;

namespace SimpleSync
{
    public class SyncManager
    {
        public static bool IsD2ESyncStart = false;
        public static bool IsE2DSyncStart = false;

        private static int m_iSyncSceneID = -1;
        private static int m_iSyncFrameRate = 1;
        private static Dictionary<int, GameObject> m_dD2ESyncObjectDic = new Dictionary<int, GameObject>();
        private static Dictionary<int, GameObject> m_dE2DControlObjectDic = new Dictionary<int, GameObject>();

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
                    IsD2ESyncStart = false;
                    IsE2DSyncStart = false;
                    m_iSyncSceneID = value;
                    SceneManager.LoadScene(value);
                }
            }
        }

        public static int SyncFrameRate
        {
            get { return m_iSyncFrameRate; }
            set
            {
                if (m_iSyncFrameRate != value && value > 0)
                {
                    m_iSyncFrameRate = value;
                    SyncCenter.Instance.SetSyncFrameRate(value);
                }
            }
        }

        public static int D2ESyncObjectsCount
        {
            get { return m_dD2ESyncObjectDic.Count; }
        }

        public static int E2DControlObjectsCount
        {
            get { return m_dE2DControlObjectDic.Count; }
        }

        public static void StartD2ESync(byte[] data)
        {
            //Debug.Log(data.Length);
            EditorSync.SendSyncRequest(data);
        }

        public static void StartE2DControl(byte[] data)
        {
            EditorSync.SendControlRequest(data);
        }

        public static void AddD2ESyncObject(int index, GameObject go)
        {
            if (go == null) return;
            m_dD2ESyncObjectDic[index] = go;
        }

        public static void AddE2DControlObject(int index, GameObject go)
        {
            if (go == null) return;
            m_dE2DControlObjectDic[index] = go;
        }

        public static void ResetD2ESyncObjects()
        {
            m_dD2ESyncObjectDic.Clear();
        }

        public static void ResetE2DControlObjects()
        {
            m_dE2DControlObjectDic.Clear();
        }

        public static GameObject GetD2EObjectByID(int id)
        {
            return GetObjectByID(id, m_dD2ESyncObjectDic);
        }

        public static GameObject GetE2DObjectByID(int id)
        {
            return GetObjectByID(id, m_dE2DControlObjectDic);
        }

        private static GameObject GetObjectByID(int id, IDictionary<int, GameObject> targetDic)
        {
            if (targetDic.ContainsKey(id))
            {
                return targetDic[id];
            }
            return null;
        }

        public static void FindD2ESyncObjects(Dictionary<int, string> requestSyncObjectList)
        {
            FindSyncObjects(requestSyncObjectList, m_dD2ESyncObjectDic);
        }

        public static void FindE2DControlObjects(Dictionary<int, string> requestSyncObjectList)
        {
            FindSyncObjects(requestSyncObjectList, m_dE2DControlObjectDic);
        }

        public static void FindSyncObjects(Dictionary<int, string> requestSyncObjectList, IDictionary<int, GameObject> targetDic)
        {
            targetDic.Clear();
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
                    targetDic.Add(item.Key, go);
                }
            }
        }

        public static byte[] GetD2ESyncData()
        {
            return FBHelper.SerializeSyncObject(m_dD2ESyncObjectDic);
        }

        public static byte[] GetE2DControlData()
        {
            return FBHelper.SerializeSyncObject(m_dE2DControlObjectDic);
        }

        public static void D2ESyncUpdate(byte[] data)
        {
            FBHelper.DeserializeSyncObject(data, m_dD2ESyncObjectDic);
        }

        public static void E2DControlUpdate(byte[] data)
        {
            FBHelper.DeserializeSyncObject(data, m_dE2DControlObjectDic);
        }

    }

}
