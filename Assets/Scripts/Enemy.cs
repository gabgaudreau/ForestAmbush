using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    protected int _hp;
	protected Vector3 _deathPosition;
    [SerializeField]
    LayerMask onlyLayer;
    bool hitEnemy;

    void Start() {
    }

    void Update() {
    }

    private void FixedUpdate() {
        hitEnemy = false;
    }

    protected PoVNode GetClosestNode(Vector3 pos) {
        //Get closest node
        Collider[] collided = Physics.OverlapSphere(pos, 20.0f, onlyLayer);
        float smallestDistance = Mathf.Infinity;
        PoVNode closest = null;
        foreach (Collider c in collided) {
            if (c.GetType() == typeof(SphereCollider)) {
                GameObject a = GameObject.Find(c.name);
                PoVNode temp = a.GetComponent<PoVNode>();
                if (Vector3.Distance(temp.GetWorldPos(), transform.position) < smallestDistance) {
                    smallestDistance = Vector3.Distance(temp.GetWorldPos(), transform.position);
                    closest = temp;
                }
            }
        }
        return closest;
    }

    protected void OnTriggerEnter(Collider col) {
        if (col.tag == "Bullet" && !hitEnemy) {
            hitEnemy = true;
            _hp--;

            if (gameObject.tag.Equals("Flocker"))
            {
                GetComponent<Animator>().SetTrigger("Take Hit");
            }
        }
		if (_hp < 0) {
			_deathPosition = transform.position;
            GetComponent<Collider>().enabled = false;
			Destroy(gameObject, 4.0f);
		}
    }
}
