using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToFloor : MonoBehaviour {
    void Update() {
        RaycastHit hitDown;
        float deltaY = 0;   // height difference, based on model
        float distanceToGround = Mathf.Infinity;
		if (Physics.Raycast(transform.position, Vector3.down, out hitDown)) {
            distanceToGround = hitDown.distance;

            if (gameObject.tag.Equals("Large Enemy"))
                deltaY = 1.25f;
            else if (gameObject.tag.Equals("Flocker"))
                deltaY = 0;

            if (distanceToGround < 2.0f)
                transform.position = new Vector3(transform.position.x, hitDown.point.y + deltaY, transform.position.z);
        }
    }
}
