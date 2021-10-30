using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebugIt
{
    private PathGraph Graph;
    private GameObject _testBug;

    public DebugIt(PathGraph graph, GameObject testBug)
    {
        Graph = graph;
        _testBug = testBug;
    }

    //public List<PathNode> CreateTestPath(GameObject player)
    //{
    //    List<PathNode> path;
    //    PathSearch ps = new PathSearch();
    //    PathNode start = Graph.FindClosestNode(player.transform.position);
    //    PathNode end = Graph.Nodes[4, 8];
    //    int x; int y;
    //    Graph.FindNodeGridCoordinates(start, out x, out y);
    //    int shortestPath = Mathf.Abs(4 - x) + Mathf.Abs(8 - y);
    //    path = ps.FindPathBetween(start, end, shortestPath);
    //    return path;
    //}

    public void DisplayCurrent(List<PathNode> path, PathNode intermediateDest = null)
    {
        foreach (PathNode pn in Graph.Nodes)
        {
            pn.GetComponent<SpriteRenderer>().color = Color.white;
        }

        Color currentPathColor = Color.green;
        int ndx = 1;
        foreach (PathNode pn in path)
        {
            if (pn == intermediateDest)
            {
                pn.GetComponent<SpriteRenderer>().color = Color.cyan;
            }
            else
            {
                pn.GetComponent<SpriteRenderer>().color = currentPathColor;
            }
            ndx++;
            currentPathColor = (ndx >= path.Count) ? Color.red : Color.blue;
        }
    }

    internal List<PathNode> GetPathBetweenBugAndMouse()
    {
        List<PathNode> retVal = new List<PathNode>();
        if (_testBug != null)
        {
            var asps = new AStarPathSearch();
            PathNode start = Graph.FindClosestNode(_testBug.transform.position);
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            PathNode end = Graph.FindClosestNode(mousePos);
            retVal = asps.FindPath(start.AStar, end.AStar);
        }        
        return retVal;
    }

    public void DisplayNextInAllBest()
    {
        //var values = PathSearch.AllBestPathsFound.Values.ToArray();        
        //DisplayCurrent(values[_currentAllBestNdx]);
        //_currentAllBestNdx++;
    }


}
