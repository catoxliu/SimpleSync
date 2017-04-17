using UnityEngine;
using System.Collections;

public class TestCube : MonoBehaviour {

    Transform m_kCameraTrans;
#if !UNITY_EDITOR
    // Use this for initialization
    void Start () {
        Input.gyro.enabled = true;
        Input.gyro.updateInterval = 0.01f;
        Input.multiTouchEnabled = false;
        m_kCameraTrans = Camera.main.transform;
        StartCoroutine(RollCube());
        SimpleSync.SyncManager.Init();

    }

    // Update is called once per frame
    void Update () {
        m_kCameraTrans.rotation = Input.gyro.attitude;

    }

    IEnumerator RollCube()
    {
        while (true)
        {
            transform.Rotate(Vector3.up, 0.2f);
            yield return null;
        }
    }
#endif
}
