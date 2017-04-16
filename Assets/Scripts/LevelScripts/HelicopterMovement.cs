using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterMovement : MonoBehaviour
{
    private Vector3 startPos;

    private float amplitudeXZ = 0.5f;
    private float speedXZ = 0.5f;

    private float amplitudeY = 0.75f;
    private float speedY = 0.75f;

    void Start ()
    {
        startPos = transform.position;
	}
	
    void Update()
    {
        // Oscillate along all axes
        transform.position = new Vector3(startPos.x + amplitudeXZ * Mathf.Sin(speedXZ * Time.time), 
                                         startPos.y + amplitudeY * Mathf.Sin(speedY * Time.time), 
                                         startPos.z + amplitudeXZ * Mathf.Sin(speedXZ * Time.time));
    }
}
