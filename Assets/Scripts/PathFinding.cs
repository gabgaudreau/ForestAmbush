using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinding : MonoBehaviour {

    /// <summary>
    /// Path Finding method for PoV Nodes. This algorithm is A* with 3 possible heuristics, namely, none, euclidean distance and cluster.
    /// </summary>
    /// <param name="start">Start PoVNode</param>
    /// <param name="goal">Goal PoVNode</param>
    /// <param name="cluster">bool cluster</param>
    /// <param name="eu">bool euclidean</param>
    /// <returns>Returns List<PoVNode> the path found.</returns>
    public List<PoVNode> PoVPathFinding(PoVNode start, PoVNode goal) {
        List<PoVNode> open = new List<PoVNode>() { start };
        ArrayList closed = new ArrayList();
        Dictionary<PoVNode, float> startDistanceScore = new Dictionary<PoVNode, float>();
        Dictionary<PoVNode, float> total_score = new Dictionary<PoVNode, float>();
        int index = 0;
        startDistanceScore.Add(start, 0);
        start.SetParent(null);
        while (open.Count > 0) {
            startDistanceScore = startDistanceScore.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            total_score = total_score.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            index = 0;
            for (int i = 0; i < total_score.Count; i++) {
                if (open.Contains(total_score.ElementAt(i).Key)) {
                    index = open.IndexOf(total_score.ElementAt(i).Key);
                    break;
                }
            }
            PoVNode best = open[index];
            if (best == goal)
                return GetPath(best);

            open.Remove(best);
            closed.Add(best);

            foreach (KeyValuePair<PoVNode, float> n in best.GetNeighbors()) {
                if (closed.Contains(n.Key))
                    continue;

                float tempDistance = startDistanceScore[best] + n.Key.GetNeighborDistance(best);

                if (!open.Contains(n.Key))
                    open.Add(n.Key);
                else if (tempDistance >= startDistanceScore[n.Key])
                    continue;

                n.Key.SetParent(best);
                startDistanceScore[n.Key] = tempDistance;
                total_score[n.Key] = tempDistance + euclidianDistHeu(n.Key, goal);
            }
        }
        return null;
    }

    /// <summary>
    /// Returns the euclidean distance between the two given nodes.
    /// </summary>
    /// <param name="curr">Current PoVNode</param>
    /// <param name="goal">Goal PoVNode</param>
    /// <returns>Returns float</returns>
    private float euclidianDistHeu(PoVNode curr, PoVNode goal) {
        return Vector3.Distance(curr.GetWorldPos(), goal.GetWorldPos());
    }

    /// <summary>
    /// GetPath will trace back the path taken to reach the current node passed in. 
    /// Using the parent variable stored in each node.
    /// </summary>
    /// <param name="current">Current PoVNode</param>
    /// <returns>List<PoVNode></returns>
    private static List<PoVNode> GetPath(PoVNode current) {
        List<PoVNode> path = new List<PoVNode>() { current };
        while (current.GetParent() != null) {
            current = current.GetParent();
            path.Add(current);
        }
        path.Reverse();
        return path;
    }
}
