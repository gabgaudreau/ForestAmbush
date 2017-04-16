using UnityEngine;

/**
 * Reparenting script
 * 
 * NOTE: Used by tools (i.e.: pistol) so they can be left outside
 *       of the hierarchy in the scene and still be parented correctly
 *       later.
 */
public class Reparent : MonoBehaviour {
    [SerializeField]
    Transform parent;

    [SerializeField]
    Vector3 initialLocalPosition;

    [SerializeField]
    Vector3 initialLocalRotation;

    void Awake() {
        transform.parent = parent;
        transform.localPosition = initialLocalPosition;
        transform.localEulerAngles = initialLocalRotation;
    }
}
