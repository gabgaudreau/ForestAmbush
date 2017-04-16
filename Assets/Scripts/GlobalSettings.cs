using UnityEngine;

public class GlobalSettings : MonoBehaviour {
    [SerializeField]
    float timeScale;

    public static GlobalSettings instance;

    void Awake() {
        instance = this;
        Application.targetFrameRate = -1;
        Time.timeScale = timeScale;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Cursor.visible = !Cursor.visible;
        }
    }
}
