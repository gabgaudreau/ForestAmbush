using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeEnemy : Enemy {
    [SerializeField]
    AudioClip[] _voiceClips;
    float _lastVoiceTime;

	[SerializeField]
	GameObject _baseModel;

	[SerializeField]
	GameObject _deathModel;

    GameObject target;
    List<PoVNode> path = new List<PoVNode>();
    PathFinding pf = new PathFinding();
    bool searching = true;
    int curNodeIndex = 0, nearRadius = 5;
    Vector3 direction, directionVector;
    float curRotationVel, maxRotationAccel = 0.15f, maxRotationSpeed = 15.0f, finalRadius = 15f;


    void Start() {
        target = GameObject.FindGameObjectWithTag("Player");
        _hp = 5;
    }

    void Update() {
		if (_hp < 0) {
			_baseModel.SetActive(false);
			_deathModel.SetActive(true);
			transform.position = _deathPosition;
            GetComponent<CapsuleCollider>().enabled = false;
            Destroy(gameObject, 5.0f);
			return;
		}

        if (Vector3.Distance(transform.position, target.transform.position) > 100.0f) {
            Destroy(gameObject);
        }

        PoV();

        if (Time.timeSinceLevelLoad - _lastVoiceTime > 5.0f) {
            _lastVoiceTime = Time.timeSinceLevelLoad;
            int randomIndex = Random.Range(0, (_voiceClips.Length - 1));
            GetComponent<AudioSource>().PlayOneShot(_voiceClips[randomIndex]);
            //Every 5 seconds recalculate path
            if (curNodeIndex != 1) {
                searching = true;
                curNodeIndex = 0;
            }
        }
    }

    private void PoV() {
        if (searching) {
            searching = !searching;
            PoVNode start = GetClosestNode(transform.position);
            PoVNode goal = GetClosestNode(target.transform.position);

            if (goal != null && start != null) {
				path = pf.PoVPathFinding(start, goal);
			}
        }

        if (!searching) {
            if (curNodeIndex == -1) {
                if (Vector3.Distance(transform.position, target.transform.position) <= 25.0f)
                    SteeringArrive(target.transform.position);
                else {
                    curNodeIndex = 0;
                    searching = !searching;
                }
            }
            else {
                // No path -- don't go out of index
                if (path == null || (path != null && path.Count == 0)) {
                    searching = !searching;
                    return;
                }

                PoVNode curr = path[curNodeIndex];
                direction = (curr.GetWorldPos() - transform.position).normalized;
                SteeringArrive(curr.GetWorldPos());

                // Always check if its the last node
                if (curr == path[path.Count - 1] && Vector3.Distance(transform.position, curr.GetWorldPos()) <= finalRadius)
                    curNodeIndex = -1;
                // Check if in near rad? Give next node, otherwise keep going
                else if (Vector3.Distance(transform.position, curr.GetWorldPos()) <= nearRadius)
                    curNodeIndex++;

                if (Vector3.Distance(transform.position, target.transform.position) <= finalRadius)
                    curNodeIndex = -1;
            }
        }
    }

    /// <summary>
    /// Calculate direction, and rotate character
    /// If NPC not at start, move, if at start rotate towards it then move
    /// </summary>
    /// <param name="targetPos">Where the NPC is headed</param>
    /// <param name="startNode">If NPC at start node</param>
    private void SteeringArrive(Vector3 targetPos) {
        //Find the direction vector based on the target's position
        directionVector = (targetPos - transform.position).normalized;
        //Find the current rotation velocity
        curRotationVel = Mathf.Min(curRotationVel + maxRotationAccel, maxRotationSpeed);
        //Interpolate the orientation of the NPC object
        Quaternion targetRotation = Quaternion.LookRotation(directionVector);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, curRotationVel * Time.deltaTime);
        //Update the position
        transform.position += transform.forward * Time.deltaTime * 6f;
    }
}
