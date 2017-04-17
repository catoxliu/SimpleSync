using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace SimpleSync
{

    public class SyncCenter : MonoBehaviour
    {

        private static SyncCenter m_kInstance = null;

        public static SyncCenter Instance
        {
            get { return m_kInstance; }
        }

        [Range(1, 90)]
        public int m_iSyncFrameRate = 1;
        public string m_kAndroidActivity = "com.unity3d.player.UnityPlayerActivity";
        public bool m_bWirelessConnection = false;
        public string m_kDeviceIPAddress = "";

        WaitForEndOfFrame wfeof = new WaitForEndOfFrame();
        WaitForSeconds wt = new WaitForSeconds(1f);

        public int Test1;

        void Awake()
        {
            m_kInstance = this;
            DontDestroyOnLoad(gameObject);
#if UNITY_EDITOR
            EditorSync.Init();
#else
            DeviceSync.Init();
#endif
        }

        void Start()
        {
            StartCoroutine(UpdateLogic());
        }

        void OnDestroy()
        {
            StopAllCoroutines();
            m_kInstance = null;
        }

        public void SetSyncFrameRate(int frameRate)
        {
            float frameTime = 1.0f / (float)frameRate;
            wt = new WaitForSeconds(frameTime);
        }

        IEnumerator UpdateLogic()
        {
            while(true)
            {
                yield return wt;
                yield return wfeof;
#if UNITY_EDITOR
                EditorSync.UpdateLogic();
#else
                DeviceSync.UpdateLogic();
#endif
            }
        }

    }
}
