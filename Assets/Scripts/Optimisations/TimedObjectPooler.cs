using UnityEngine;

public class TimedObjectPooler : MonoBehaviour {
    [SerializeField]
    private float m_TimeOut = 1.0f;

    void OnEnable() {
        Invoke("PoolNow", m_TimeOut);
    }

    void PoolNow() {
        ObjectPool.instance.PoolObject(gameObject);
    }
}
