using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapGraph;

public class Pathfinding 
{
    internal static Stack<MapGraph.UGraphNode> shortestPathToNode(MapGraph.UGraphNode src, MapGraph.UGraphNode dst)
    {
        // Perform BFS
        Dictionary<UGraphNode, int> dist = new Dictionary<UGraphNode, int>();
        Dictionary<UGraphNode, UGraphNode> pred = new Dictionary<UGraphNode, UGraphNode>();

        Queue<UGraphNode> q = new Queue<UGraphNode>();

        //Debug.Log(src.xLoc + ", " + src.yLoc);
        // Add src as the root
        dist.Add(src, 0);
        pred.Add(src, null);

        q.Enqueue(src);

        // Perform BFS
        while (q.Count > 0)
        {
            UGraphNode node = q.Dequeue();

            foreach (UGraphNode e in node.Edges)
            {
                if (!dist.ContainsKey(e))
                {
                    pred.Add(e, node);
                    dist.Add(e, dist[node] + 1);
                    q.Enqueue(e);
                }
            }
        }

        // Build path from predecessors
        Stack<UGraphNode> path = new Stack<UGraphNode>();
        //Debug.Log(pred.Keys.Count);
        UGraphNode currNode = dst;
        while (currNode != null) {
            //Debug.Log(currNode.xLoc + ", " + currNode.yLoc);
            path.Push(currNode);
            currNode = pred[currNode];
        }

        return path;
    }



}
