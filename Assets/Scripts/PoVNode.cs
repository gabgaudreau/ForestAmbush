/*
* @author Gabriel Gaudreau - PoVNode.cs
* @class COMP 476 Advanced Game Development
* @date March 10th 2017
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoVNode : MonoBehaviour{
    private Dictionary<PoVNode, float> neighbors = new Dictionary<PoVNode, float>();
    private float distToGoal;
    private PoVNode parent;

	void OnDrawGizmos() {
		Gizmos.color = Color.white;
		foreach (PoVNode current in neighbors.Keys) {
			Gizmos.DrawLine(transform.position, current.transform.position);
		}
	}

    /// <summary>
    /// Get method for neighbors.
    /// </summary>
    /// <returns>Returns Dictionary<PoVNode, float></returns>
    public Dictionary<PoVNode, float> GetNeighbors() {
        return neighbors;
    }

    /// <summary>
    /// Addneighbor method to keep the dictionary private.
    /// </summary>
    /// <param name="n">PoVNode</param>
    /// <param name="distance">float for the distance</param>
    public void AddNeighbor(PoVNode n, float distance) {
        neighbors.Add(n, distance);
    }

    /// <summary>
    /// Get method for worldPosition
    /// </summary>
    /// <returns>Returns Vector3</returns>
    public Vector3 GetWorldPos() {
        return transform.position;
    }

    /// <summary>
    /// Set method for distance to goal
    /// </summary>
    /// <param name="d">float distance</param>
    public void SetDistToGoal(float d) {
        distToGoal = d;
    }

    /// <summary>
    /// Get method for distance from goal
    /// </summary>
    /// <returns>Returns float</returns>
    public float GetDistanceFromGoal() {
        return distToGoal;
    }

    /// <summary>
    /// Set method for parent
    /// </summary>
    /// <param name="p">PoVNode parent</param>
    public void SetParent(PoVNode p) {
        parent = p;
    }

    /// <summary>
    /// Get method for parent
    /// </summary>
    /// <returns>Returns PoVNode</returns>
    public PoVNode GetParent() {
        return parent;
    }

    /// <summary>
    /// Get method for distance between neighbors
    /// </summary>
    /// <param name="n">PoVNode passed in.</param>
    /// <returns>Returns float</returns>
    public float GetNeighborDistance(PoVNode n) {
        return neighbors[n];
    }
}
