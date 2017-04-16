/*
* @author Gabriel Gaudreau - PoVGraph.cs
* @class COMP 476 Advanced Game Development
* @date March 10th 2017
*/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoVGraph : MonoBehaviour {
    [SerializeField]
    float maxDistanceBetweenNodes;

    private PoVNode[] nodes;
    private int edgeCount;

    /// <summary>
    /// Start method, will find all nodes in the scene and create a graph with said nodes.
    /// </summary>
    void Start() {
        edgeCount = 0;
        nodes = GameObject.FindObjectsOfType<PoVNode>();
        float start = Time.realtimeSinceStartup;
        CreateGraph();
        Debug.Log("Time spent creating graph: " + (Time.realtimeSinceStartup - start) + "s");
        Debug.Log("Nodes count: " + nodes.Length + ", Edge count: " + edgeCount);
    }

    /// <summary>
    /// This method will create the graph with all the PoVNodes found in the scenes, O(n(n-1)/2) using raycasting to check for line of sight between nodes.
    /// </summary>
    void CreateGraph() {
        for (int i = 0; i < nodes.Length - 1; i++) {
            for (int j = i + 1; j < nodes.Length; j++) {
                Vector3 direction = (nodes[j].GetWorldPos() - nodes[i].GetWorldPos()).normalized;
                float distance = Vector3.Distance(nodes[i].GetWorldPos(), nodes[j].GetWorldPos());

                if (distance < maxDistanceBetweenNodes) {
                    bool collided = false;
                    RaycastHit[] hits = Physics.RaycastAll(nodes[i].GetWorldPos(), direction);
                    foreach (RaycastHit hit in hits.OrderBy(h => h.distance).ToArray()) {
                        if (hit.collider.gameObject == gameObject)
                            continue;
                        else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Node") && hit.collider.gameObject == nodes[j].gameObject) {
                            collided = true;
                            break;
                        }
                        else
                            break;
                    }

                    if (collided) {
                        nodes[i].AddNeighbor(nodes[j], distance);
                        nodes[j].AddNeighbor(nodes[i], distance);
                        edgeCount += 2;
                    }
                }
            }
        }
    }
}
