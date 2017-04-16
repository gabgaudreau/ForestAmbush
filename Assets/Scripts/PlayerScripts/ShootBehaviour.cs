using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBehaviour : MonoBehaviour {
    [SerializeField]
    GameObject explosionPrefab;

	[SerializeField]
	AudioClip[] _explosionClips;

    float lastTime;

    void Awake() {
        lastTime = Time.time;
    }

    void Update() {
        if (Input.GetMouseButton(0) && Camera.main.gameObject.activeSelf && Time.time - lastTime > 0.5f) {
            lastTime = Time.time;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hitPoints = Physics.RaycastAll(ray, Mathf.Infinity);
            foreach (RaycastHit hitPoint in hitPoints) {
                if (hitPoint.collider.gameObject != gameObject) {
					int randomIndex = Random.Range(0, (_explosionClips.Length - 1));
					GetComponent<AudioSource>().PlayOneShot(_explosionClips[randomIndex], 0.05f);

                    GameObject explosionInstance = Instantiate(explosionPrefab);
                    explosionInstance.transform.position = hitPoint.point;
                    explosionInstance.transform.localScale *= 5.0f;
                }
            }
        }
    }
}
