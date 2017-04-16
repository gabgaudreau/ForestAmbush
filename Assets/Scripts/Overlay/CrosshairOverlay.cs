using UnityEngine;
using UnityEngine.UI;

public class CrosshairOverlay : MonoBehaviour {
    [SerializeField]
    GameObject player;

    [SerializeField]
    Sprite buildDefaultCrosshair;

    [SerializeField]
    Sprite buildCrossCrosshair;

    [SerializeField]
    Sprite crossCrosshair;

    private Image cachedImage;

    void Start() {
        cachedImage = GetComponent<Image>();
    }

    // TODO: Change crosshair depending on item
    /*
    void Update() {
        cachedImage.sprite = crossCrosshair;
    }
    */
}
