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

        WaitForEndOfFrame wfeof = new WaitForEndOfFrame();

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

        IEnumerator UpdateLogic()
        {
            while(true)
            {
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
