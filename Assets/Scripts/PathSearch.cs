using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class PathSearch
{
    public static Dictionary<NodePair, List<PathNode>> AllBestPathsFound = new Dictionary<NodePair, List<PathNode>>();
    public static int HighestRecursiveDepth {get; set;}
    public int CurrentRecuriveDepth { get; private set; }
    
    private PathNode _destination;
    private int _shortestPossible;
    private List<PathNode> _shortestPath;
    private List<PathNode> _currentPath;


    public PathSearch()
    {
        _shortestPath = new List<PathNode>();
        _currentPath = new List<PathNode>();
        CurrentRecuriveDepth = 0;
    }

    public List<PathNode> FindPathBetween(PathNode start, PathNode destination, int shortest)
    {
        float sTime = Time.realtimeSinceStartup;
        _shortestPath = FindPreviousBestPath(start, destination);
        if (_shortestPath.Count == 0)
        {
            _destination = destination;
            _currentPath = new List<PathNode>();
            _shortestPossible = shortest;
            FindDestination(start);
            AddToBestPathsFound(start, destination);
            AddSubPathsToBest(_shortestPath);
            //Debug.Log("FindPathBetween: " + (Time.realtimeSinceStartup - sTime).ToString());
        }
        else
        {
            //Debug.Log("FindPathBetween: Found existing.");
        }
        return _shortestPath;
    }

    private void AddToBestPathsFound(PathNode start, PathNode destination)
    {
        NodePair pair = new NodePair() { StartNode = start, EndNode = destination };
        AllBestPathsFound.Add(pair, _shortestPath);
    }

    private List<PathNode> FindPreviousBestPath(PathNode start, PathNode destination)
    {
        List<PathNode> retVal = new List<PathNode>();
        NodePair pair = new NodePair() { StartNode = start, EndNode = destination };
        AllBestPathsFound.TryGetValue(pair, out retVal);
        return retVal ?? new List<PathNode>();
    }

    private void AddSubPathsToBest(List<PathNode> path)
    {
        foreach (var pnStart in path)
        {
            foreach (var pnDest in path)
            {
                if ((pnStart != pnDest))
                {
                    NodePair pair = new NodePair() { StartNode = pnStart, EndNode = pnDest };
                    AddToBestPaths(pair, MakeSubPath(pair, path));
                    pair = new NodePair() { StartNode = pnDest, EndNode = pnStart }; //reversed the start and end nodes
                    AddToBestPaths(pair, MakeSubPath(pair, path));
                }
            }
        }
    }

    private static void AddToBestPaths(NodePair pair, List<PathNode> subPath)
    {
        if (!AllBestPathsFound.ContainsKey(pair) && subPath.Count > 0)
        {
            AllBestPathsFound.Add(pair, subPath);
        }
    }

    private List<PathNode> MakeSubPath(NodePair pair, List<PathNode> path)
    {
        List<PathNode> retVal = new List<PathNode>();
        int startNdx = path.IndexOf(pair.StartNode);
        int endNdx = path.IndexOf(pair.EndNode);
        if (startNdx < endNdx)
        {
            for (int ndx = startNdx; ndx <= endNdx; ndx++)
            {
                retVal.Add(path[ndx]);
            }
        }
        else
        {
            for (int ndx = startNdx; ndx >= endNdx; ndx--)
            {
                retVal.Add(path[ndx]);
            }
        }
        return retVal;
    }

    private void FindDestination(PathNode currentNode)
    {
        CurrentRecuriveDepth++;
        if (CurrentRecuriveDepth > HighestRecursiveDepth)
        {
            HighestRecursiveDepth = CurrentRecuriveDepth;
        }
        _currentPath.Add(currentNode);
        if (_shortestPath.Count > 0 && (_shortestPath.Count <= _shortestPossible))
        {
            //Best path already found
        }
        else if (_shortestPath.Count > 0 && (_currentPath.Count >= _shortestPath.Count))
        {
            //Not best path
        }
        else if (currentNode == _destination)
        {
            //New best path found
            _shortestPath = new List<PathNode>();
            _currentPath.ForEach(cn => _shortestPath.Add(cn)); //Clone
        }
        else
        {
            var subPath = new List<PathNode>();
            if (IsBestASubPath(currentNode, out subPath))
            {
                //Optimization: when currentNode to Dest is an existing best see if it is new best added to current path
                _shortestPath = new List<PathNode>();
                _currentPath.ForEach(cn => _shortestPath.Add(cn)); //Clone 
                subPath.ForEach(cn => _shortestPath.Add(cn)); //Clone 
                _shortestPath.Remove(currentNode);  //Remove duplicate
            }
            else
            {
                //Look deeper recursively
                //start here tod: randomize GetNeighbors.
                foreach (PathNode neighbor in currentNode.AStar.GetNeighbors())
                {
                    if (!_currentPath.Contains(neighbor))
                    {
                        FindDestination(neighbor);
                    }
                }
            }
        }
        _currentPath.Remove(currentNode);
    }

    private bool IsBestASubPath(PathNode currentNode, out List<PathNode> subPath)
    {
        bool retVal = false;
        var subPair = new NodePair() { StartNode = currentNode, EndNode = _destination };
        if (AllBestPathsFound.TryGetValue(subPair, out subPath))
        {
            int subLength = _currentPath.Count + subPath.Count - 1;  //Minus one to remove duplicate currentNode
            if (_shortestPath.Count == 0 || (subLength <= _shortestPath.Count))
            {
                retVal = true;
            }
        }
        return retVal;
    }

    public Dictionary<NodePair, List<PathNode>> GetAllBestPaths(PathGraph graph)
    {
        float sTime = Time.realtimeSinceStartup;
        Dictionary<NodePair, List<PathNode>> retval = new Dictionary<NodePair, List<PathNode>>();
        //foreach (PathNode startNode in graph.Nodes)
        //{
        //    foreach (PathNode endNode in graph.Nodes)
        //    {
        //        NodePair pair = new NodePair() { StartNode = startNode, EndNode = endNode };
        //        if (!retval.ContainsKey(pair))
        //        {
        //            int shortest = graph.MinimumDistanceBetweenNodes(startNode, endNode);
        //            List<PathNode> path = FindPathBetween(startNode, endNode, shortest);
        //            retval.Add(pair, path);
        //        }
        //    }
        //    Debug.Log("GetAllBestPaths: " + (Time.realtimeSinceStartup - sTime).ToString());
        //}
        //Debug.Log("GetAllBestPaths: " + (Time.realtimeSinceStartup - sTime).ToString());
        return retval;
    }


}
