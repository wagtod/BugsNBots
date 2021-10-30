using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AStarPathSearch 
{
    private bool _isDebugMode = false;

    public List<PathNode> FindPath(AStarPathNode startNode, AStarPathNode targetNode)
    {
        ResetNodes();
        startNode.IsDebugMode = _isDebugMode;
        targetNode.IsDebugMode = _isDebugMode;
        if (_isDebugMode)
        {
            startNode.Home.ColorMe(Color.green);
            targetNode.Home.ColorMe(Color.red);
        }

        var retVal = new List<PathNode>();
        var openSet = new List<AStarPathNode>();
        var closedSet = new HashSet<AStarPathNode>();
        openSet.Add(startNode);
        AStarPathNode currentNode = null;
        while (openSet.Count > 0)
        {
            currentNode = openSet.OrderBy(a => a.FCost).ThenBy(b => b.HCost).First();

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                retVal.Add(currentNode.Home);
                while (currentNode.PathParent != null)
                {
                    currentNode = currentNode.PathParent;
                    retVal.Add(currentNode.Home);
                }
                retVal.Reverse();
                break;
            }

            foreach (var neighbor in currentNode.Neighbors())
            {
                if (!closedSet.Contains(neighbor))
                {
                    int newGCost = currentNode.GCost + 1; //All neighbors are considered 1 unit away.  No diagonals.
                    if (newGCost < neighbor.GCost || !openSet.Contains(neighbor))
                    {
                        neighbor.GCost = newGCost;
                        neighbor.HCost = CoordinateDistanceBetween(neighbor, targetNode);
                        neighbor.PathParent = currentNode;
                        if (!openSet.Contains(neighbor))
                        {
                            neighbor.IsDebugMode = _isDebugMode;
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

        }
        return retVal;
    }

    private int CoordinateDistanceBetween(AStarPathNode a, AStarPathNode b)
    {
        //simple grid distance as integer
        return Mathf.Abs(a.PathGraphRow - b.PathGraphRow) + Mathf.Abs(a.PathGraphCol - b.PathGraphCol);
    }
   
    private void ResetNodes()
    {
        foreach (var node in LevelController.Instance.Graph.Nodes)
        {
            node.AStar.Reset();
            if (_isDebugMode)
            {
                node.HideMe();
            }
            node.AStar.IsDebugMode = false;
        }
    }
}
