using UnityEngine;
using System.Linq;

public class PistolControl : MonoBehaviour {
    [SerializeField]
    GameObject bulletHolePrefab;

    [SerializeField]
    AudioClip shootingSound;

    [SerializeField]
    float moveBackSpeed;

    [SerializeField]
    float moveBackDistance;

    private Vector3 initialLocalPosition;

    void Start() {
        initialLocalPosition = transform.localPosition;
    }

    void Update() {
        bool gunWasShot = Input.GetMouseButtonDown(0);

        // Raycast from camera and get first hit
        GameObject hitGameObject = null;
        RaycastHit hitGameObjectInfo = new RaycastHit();
        Transform eye = Camera.main.transform;
        Ray pointer = new Ray(eye.position, eye.forward);
        RaycastHit[] hits = Physics.RaycastAll(pointer);
        foreach (RaycastHit hit in hits.OrderBy(x => x.distance).ToList()) {
            if (hit.collider.CompareTag("Level")) {
                hitGameObject = hit.collider.gameObject;
                hitGameObjectInfo = hit;
                break;
            }
        }

        if (gunWasShot) {
            // Sound
            AudioSource.PlayClipAtPoint(shootingSound, transform.position, 0.6f);

            // Visual recoil
            Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
            transform.localPosition -= localForward * moveBackDistance;

            if (hitGameObject) {
                // Spawn a decal on hit point
                GameObject bulletHoleInstance = ObjectPool.instance.GetObjectForType(bulletHolePrefab, false);
                float offset = 0.01f; // Offset to avoid z-fighting
                bulletHoleInstance.transform.position = hitGameObjectInfo.point + hitGameObjectInfo.normal * offset;
                bulletHoleInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitGameObjectInfo.normal);
            }
        }
        // Move back recoil
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, initialLocalPosition, moveBackSpeed * Time.deltaTime);
    }
}
