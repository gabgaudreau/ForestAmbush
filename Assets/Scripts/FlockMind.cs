using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockMind : Enemy {
    [SerializeField]
    GameObject flockerPrefab;

    GameObject target;

    List<GameObject> flockerList;

    Vector3 targetPos;  // NOTE: always standardize height to flockCenter height

    Vector3 spawnPos;

    float lateralDistance = 2.0f;
    float kOffset = 1;

    float flockSpeed = 0.1f;
    int flockCount;


    float _lastVoiceTime;

    List<PoVNode> path = new List<PoVNode>();
    PathFinding pf = new PathFinding();
    bool searching = true;
    int curNodeIndex = 0, nearRadius = 5;
    Vector3 direction, directionVector;
    float curRotationVel, maxRotationAccel = 0.15f, maxRotationSpeed = 15.0f, finalRadius = 15f;



    // Use this for initialization
    void Start() {
        spawnPos = transform.position;

        target = GameObject.FindWithTag("Player");

        flockerList = new List<GameObject>();
        flockCount = Random.Range(5, 8);

        // instatiate flockers
        for (int i = 0; i < flockCount; i++) {
            GameObject flocker = Instantiate(flockerPrefab, spawnPos + i * Vector3.left * 1.25f, Quaternion.identity);
            flocker.GetComponent<Flocker>().id = i;
            flockerList.Add(flocker);
        }

        targetPos = target.transform.position;

        PositionFlock();
    }

    // Update is called once per frame
    void Update() {
        if (Vector3.Distance(transform.position, target.transform.position) > 80.0f) {
            Destroy(gameObject);
        }

        GetNearbyFlockers();

        if (flockCount > flockerList.Count) {
            flockCount = flockerList.Count;
            PositionFlock();
        }

        targetPos = target.transform.position;

        if (flockerList.Count > 0) {
            PoV();
        }
    }

    void GetNearbyFlockers() {
        Collider[] nearbyFlockers = Physics.OverlapSphere(GetCenter(), 5.0f);  // radius may need tweaking. allows merging too
        flockerList.Clear();

        foreach (Collider col in nearbyFlockers) {
            if (col.gameObject.tag.Equals("Flocker")) {
                flockerList.Add(col.gameObject);
            }
        }
    }

    void MoveFlockers() {
        Vector3 flockCenter = GetCenter();
        Vector3 facingDir = (targetPos - flockCenter).normalized;

        // correct height
        facingDir = new Vector3(facingDir.x, 0, facingDir.z);

        Vector3 flockVelocity = facingDir * flockSpeed;

        // Assign new slot position - wrt flockCenter
        foreach (GameObject f in flockerList) {
            Flocker flocker = f.GetComponent<Flocker>();

            flocker.assignedSlot += flockVelocity;
        }
    }

    Vector3 GetCenter() {
        int count = 0;
        Vector3 avgPos = Vector3.zero;

        foreach (GameObject go in flockerList) {
            if (go != null) {
                count++;
                avgPos += go.transform.position;
            }
        }

        if (count > 0) {
            return avgPos / count;
        }
        else {
            return transform.position;
        }
    }

    void PositionFlock() {
        Vector3 flockCenter = GetCenter();
        Vector3 facingDir = (targetPos - flockCenter).normalized;

        // correct height
        facingDir = new Vector3(facingDir.x, 0, facingDir.z).normalized;

        // Perpendicular vector to the right
        Vector3 perpDir = Vector3.Cross(facingDir, Vector3.down);

        switch (flockerList.Count) {
            case 1:
                flockerList[0].GetComponent<Flocker>().assignedSlot = flockCenter;
                break;
            case 2:
                flockerList[0].GetComponent<Flocker>().assignedSlot = flockCenter - perpDir * 2.0f;
                flockerList[1].GetComponent<Flocker>().assignedSlot = flockCenter + perpDir * 2.0f;
                break;
            case 3:
                flockerList[0].GetComponent<Flocker>().assignedSlot = flockCenter + facingDir * lateralDistance;
                flockerList[1].GetComponent<Flocker>().assignedSlot = flockCenter - perpDir * lateralDistance - facingDir * lateralDistance;
                flockerList[2].GetComponent<Flocker>().assignedSlot = flockCenter + perpDir * lateralDistance - facingDir * lateralDistance; ;
                break;
            case 4:
                flockerList[0].GetComponent<Flocker>().assignedSlot = flockCenter - perpDir * lateralDistance + facingDir * lateralDistance;
                flockerList[1].GetComponent<Flocker>().assignedSlot = flockCenter + perpDir * lateralDistance + facingDir * lateralDistance;
                flockerList[2].GetComponent<Flocker>().assignedSlot = flockCenter + perpDir * lateralDistance - facingDir * lateralDistance;
                flockerList[3].GetComponent<Flocker>().assignedSlot = flockCenter - perpDir * lateralDistance - facingDir * lateralDistance;
                break;
            case 5:
                flockerList[0].GetComponent<Flocker>().assignedSlot = flockCenter + facingDir * lateralDistance * 2;
                flockerList[1].GetComponent<Flocker>().assignedSlot = flockCenter - perpDir * lateralDistance * 2 + facingDir * lateralDistance;
                flockerList[2].GetComponent<Flocker>().assignedSlot = flockCenter + perpDir * lateralDistance * 2 + facingDir * lateralDistance;
                flockerList[3].GetComponent<Flocker>().assignedSlot = flockCenter + perpDir * lateralDistance - facingDir * lateralDistance;
                flockerList[4].GetComponent<Flocker>().assignedSlot = flockCenter - perpDir * lateralDistance - facingDir * lateralDistance;
                break;
            case 6:
                flockerList[0].GetComponent<Flocker>().assignedSlot = flockCenter + facingDir * lateralDistance * 2;
                flockerList[1].GetComponent<Flocker>().assignedSlot = flockCenter - perpDir * lateralDistance * 2 + facingDir * lateralDistance;
                flockerList[2].GetComponent<Flocker>().assignedSlot = flockCenter + perpDir * lateralDistance * 2 + facingDir * lateralDistance;
                flockerList[3].GetComponent<Flocker>().assignedSlot = flockCenter + perpDir * lateralDistance * 2 - facingDir * lateralDistance;
                flockerList[4].GetComponent<Flocker>().assignedSlot = flockCenter - perpDir * lateralDistance * 2 - facingDir * lateralDistance;
                flockerList[5].GetComponent<Flocker>().assignedSlot = flockCenter;
                break;
            case 7:
                flockerList[0].GetComponent<Flocker>().assignedSlot = flockCenter + facingDir * lateralDistance * 2;
                flockerList[1].GetComponent<Flocker>().assignedSlot = flockCenter - perpDir * lateralDistance * 2 + facingDir * lateralDistance;
                flockerList[2].GetComponent<Flocker>().assignedSlot = flockCenter + perpDir * lateralDistance * 2 + facingDir * lateralDistance;
                flockerList[3].GetComponent<Flocker>().assignedSlot = flockCenter + perpDir * lateralDistance * 2 - facingDir * lateralDistance;
                flockerList[4].GetComponent<Flocker>().assignedSlot = flockCenter - perpDir * lateralDistance * 2 - facingDir * lateralDistance;
                flockerList[5].GetComponent<Flocker>().assignedSlot = flockCenter;
                flockerList[6].GetComponent<Flocker>().assignedSlot = flockCenter - facingDir * lateralDistance * 2; ;
                break;
        }
    }

    private void PoV() {
        if (searching) {
            searching = !searching;
            PoVNode start = GetClosestNode(GetCenter());
            PoVNode goal = GetClosestNode(target.transform.position);

            if (goal != null && start != null) {
                path = pf.PoVPathFinding(start, goal);
            }
        }

        if (!searching) {
            if (curNodeIndex == -1) {
                if (Vector3.Distance(GetCenter(), target.transform.position) <= 75.0f) {
                    SteeringArrive(target.transform.position);
                }
                else {
                    curNodeIndex = 0;
                    searching = !searching;
                }
            }
            else {
                // No path -- don't go out of index
                if (path.Count == 0) {
                    searching = !searching;
                    return;
                }

                PoVNode curr = path[curNodeIndex];
                direction = (curr.GetWorldPos() - GetCenter()).normalized;
                SteeringArrive(curr.GetWorldPos());

                // Always check if its the last node
                if (curr == path[path.Count - 1] && Vector3.Distance(GetCenter(), curr.GetWorldPos()) <= finalRadius)
                    curNodeIndex = -1;
                // Check if in near rad? Give next node, otherwise keep going
                else if (Vector3.Distance(GetCenter(), curr.GetWorldPos()) <= nearRadius)
                    curNodeIndex++;

                if (Vector3.Distance(GetCenter(), target.transform.position) <= finalRadius)
                    curNodeIndex = -1;
            }
        }
    }

    private void SteeringArrive(Vector3 targetPos) {

        Vector3 flockCenter = GetCenter();
        Vector3 facingDir = (targetPos - flockCenter).normalized;

        // correct height
        // facingDir = new Vector3(facingDir.x, 0, facingDir.z);

        Vector3 flockVelocity = facingDir * flockSpeed;

        // Assign new slot position - wrt flockCenter
        foreach (GameObject f in flockerList) {
            Flocker flocker = f.GetComponent<Flocker>();

            flocker.assignedSlot += flockVelocity;
        }
    }
}
