using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocker : Enemy {
    public int id;

    public Vector3 assignedSlot;
    public Vector3 relativePos;

    Rigidbody rb;

    // Seeking
    float maxSpeed = 7.0f;
    float lowSpeed = 3.5f;
    float acceleration = 1.0f;
    float nearRadius = 1.5f;

    // Rotating
    Quaternion lookWhereYoureGoing;
    float slowDownThresshold = 50.0f;
    float rotationSpeedRads = 1.5f;
    float rotationAccelRads = 0.5f;
    float maxRotatSpeedRads = 2.5f;
    float maxRotatAccelRads = 1.0f;

    float timeToTarget;
    float goalRotatSpeedRads;

    public GameObject target;
    public Vector3 goalFacing;

    Animator animator;

    // Use this for initialization
    void Start() {
        rb = GetComponent<Rigidbody>();
        target = GameObject.FindWithTag("Player");
        _hp = 2;

        animator = GetComponent<Animator>();

        animator.SetBool("Dead", false);
    }

    // Update is called once per frame
    void Update() {
        if (_hp < 0) {
            animator.SetBool("Dead", true);
            transform.position = _deathPosition;
            GetComponent<SphereCollider>().enabled = false;
            return;
        }

        goalFacing = (target.transform.position - transform.position).normalized;

        lookWhereYoureGoing = Quaternion.LookRotation(goalFacing, Vector3.up);

        if (transform.rotation != lookWhereYoureGoing) {
            Align();
        }

        SteeringSeek();

    }

    void SteeringSeek() {
        Vector3 seekFacing = (assignedSlot - transform.position).normalized;
        float distanceFromTarget = (assignedSlot - transform.position).magnitude;

        if (distanceFromTarget > nearRadius) {
            if (rb.velocity.magnitude < maxSpeed) {
                rb.velocity += seekFacing * acceleration;
            }
            else {
                rb.velocity = seekFacing * maxSpeed;
            }
        }
        else {
            seekFacing = (target.transform.position - transform.position).normalized;
            rb.velocity = seekFacing * lowSpeed;
        }
    }

    void Align() {
        goalRotatSpeedRads = maxRotatSpeedRads * (Vector3.Angle(goalFacing, transform.forward)) / slowDownThresshold;

        timeToTarget = Vector3.Angle(goalFacing, transform.forward) / rotationSpeedRads;

        // prevent div / 0
        timeToTarget = Mathf.Max(timeToTarget, 0.1f);

        if (rotationSpeedRads < maxRotatSpeedRads) {
            rotationAccelRads = Mathf.Min(maxRotatAccelRads, Mathf.Abs((goalRotatSpeedRads - rotationSpeedRads) / timeToTarget));

            rotationSpeedRads += rotationAccelRads * Time.deltaTime;
        }
        else {
            rotationSpeedRads = maxRotatSpeedRads;
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookWhereYoureGoing, rotationSpeedRads);
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag.Equals("Flocker")) {
            Physics.IgnoreCollision(other.collider, GetComponent<SphereCollider>(), true);
        }

        if (other.gameObject.name.Equals("Terrain")) {
            Physics.IgnoreCollision(other.collider, GetComponent<SphereCollider>(), true);
        }
    }
}
