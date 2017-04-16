using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
    private Renderer rend;
    private bool onObject;

    [SerializeField]
    Camera playerCamera, menuCamera;

    void Start() {
        rend = GetComponent<Renderer>();
    }

    void OnMouseEnter() {
        onObject = true;
        rend.material.color = new Color(0.360784314f, 0.309803922f, 0.301960784f);
    }
    void OnMouseExit() {
        onObject = false;
        rend.material.color = Color.white;
    }

    void Update() {
        if (!playerCamera.gameObject.activeSelf) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            RenderSettings.fogMode = FogMode.Linear;
        }

        if (onObject && Input.GetMouseButtonDown(0)) {
            if (gameObject.name.Contains("1")) { //Start Game
                RenderSettings.fogMode = FogMode.ExponentialSquared;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                GameObject menu = GameObject.FindGameObjectWithTag("Menu");
                menu.SetActive(false);
                playerCamera.gameObject.SetActive(true);
                menuCamera.gameObject.SetActive(false);
            }
            else if (gameObject.name.Contains("2")) { //How To Play
                SceneManager.LoadScene("How To Play");
            }
            else if (gameObject.name.Contains("3")) //Quit
                Application.Quit();
        }
    }
}
