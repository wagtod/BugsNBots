using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGraph
{
    public float WALL_GAP_HEIGHT = 0.32f;
    public float WALL_GAP_WIDTH = 0.32f;

    public PathNode[,] Nodes { get; private set; }
    public List<PathNode> _generatorNodes;

    public PathGraph(int rows, int cols, Vector2 rootPosition)
    {
        //Debug.Log(rootPosition.x.ToString() + ":" + rootPosition.y.ToString());
        Nodes = new PathNode[rows, cols];
        FillNodesArray(rows, cols, rootPosition);
        SetNodeNeighbors();
        _generatorNodes = new List<PathNode>();
    }

    private void SetNodeNeighbors()
    {
        int maxRowNdx = Nodes.GetLength(0) - 1;
        int maxColNdx = Nodes.GetLength(1) - 1;
        for (int r = 0; r <= maxRowNdx; r++)
        {
            for (int c = 0; c <= maxColNdx; c++)
            {
                PathNode currentNode = Nodes[r, c];
                PathNode neighbor;

                neighbor = GetGraphNode(r, c+1, maxRowNdx, maxColNdx);
                currentNode.AStar.RightNeighbor = GetNeighbor(currentNode, neighbor);

                neighbor = GetGraphNode(r, c-1, maxRowNdx, maxColNdx);
                currentNode.AStar.LeftNeighbor = GetNeighbor(currentNode, neighbor);

                neighbor = GetGraphNode(r+1, c, maxRowNdx, maxColNdx);
                currentNode.AStar.BottomNeighbor = GetNeighbor(currentNode, neighbor);

                neighbor = GetGraphNode(r-1, c, maxRowNdx, maxColNdx);
                currentNode.AStar.TopNeighbor = GetNeighbor(currentNode, neighbor);
            }
        }
    }

    internal PathNode GetRandomNode()
    {
        PathNode retVal;
        int maxX = Nodes.GetUpperBound(0);
        int maxY = Nodes.GetUpperBound(1);
        do
        {
            retVal = Nodes[UnityEngine.Random.Range(0, maxX), UnityEngine.Random.Range(0, maxY)];

        } while (retVal.UnReachable);

        return retVal;
    }

    private AStarPathNode GetNeighbor(PathNode currentNode, PathNode potentialNeighbor)
    {
        AStarPathNode retVal = null;
        if (potentialNeighbor != null && currentNode != null )
        {
            if (LevelController.Instance.IsClearPath(currentNode.gameObject, potentialNeighbor.gameObject))
            {
                retVal = potentialNeighbor.AStar;
            }
        }
        return retVal;
    }

    private PathNode GetGraphNode(int r, int c, int maxRowNdx, int maxColNdx)
    {
        PathNode retVal = null;
        if (c >= 0 && c <= maxColNdx && r >= 0 && r <= maxRowNdx)
        {
            retVal = Nodes[r, c];
        }
        return retVal;
    }

    private void FillNodesArray(int rows, int cols, Vector2 rootPosition)
    {
        Vector2 currentPosition = new Vector2(rootPosition.x, rootPosition.y);
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                Nodes[r, c] = PathNode.MakePathNode(currentPosition);
                Nodes[r, c].AStar.SetPathGraphCoordinates(r, c);
                currentPosition.x += WALL_GAP_WIDTH + PathNode.WIDTH;
            }
            currentPosition.y -= WALL_GAP_HEIGHT + PathNode.HEIGHT;
            currentPosition.x = rootPosition.x;
        }
    }

    public PathNode FindClosestNode(Vector3 target)
    {
        PathNode closest = null;
        float distance = Mathf.Infinity;
        foreach (PathNode pn in Nodes)
        {
            Vector3 diff = pn.transform.position - target;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = pn;
                distance = curDistance;
            }
        }
        return closest;
    }

    public bool FindNodeGridCoordinates(PathNode target, out int row, out int col)
    {
        bool retVal = false;
        row = -1;
        col = -1;
        int maxRowNdx = Nodes.GetLength(0) - 1;
        int maxColNdx = Nodes.GetLength(1) - 1;
        for (int r = 0; r <= maxRowNdx; r++)
        {
            for (int c = 0; c <= maxColNdx; c++)
            {
                if (Nodes[r, c] == target)
                {
                    retVal = true;
                    row = r;
                    col = c;
                }
            }
        }
        return retVal;
    }

    internal void FlashGenLocations(PathNode pn)
    {
        pn.Flash(Color.red, 2.5f);
        _generatorNodes.ForEach(gn => gn.Flash(Color.green, 2.5f));
    }

    public int MinimumDistanceBetweenNodes(PathNode a, PathNode b)
    {
        int ax; int ay; int bx; int by;
        FindNodeGridCoordinates(a, out ax, out ay);
        FindNodeGridCoordinates(b, out bx, out by);
        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
    }

    public void SetGeneratorNode(int row, int col, GameObject generator)
    {
        var pn = Nodes[row, col];
        pn.AllowBuildGenerator = true;
        pn.AllowBuildTurret = false;
        var go = GameObject.Instantiate(generator, pn.transform.position, Quaternion.identity) as GameObject;
        var sr = go.GetComponent<SpriteRenderer>();
        sr.color = new Color(1f, 1f, 1f, 0.4f); //Fade
        pn.GeneratorSR = sr;
        pn.GeneratorTEXT = go.transform.Find("Canvas").Find("Text").GetComponent<UnityEngine.UI.Text>();
        pn.GeneratorTEXT.text = string.Empty;
        _generatorNodes.Add(pn);
    }

    public void SetUnreachableNode(int row, int col)
    {
        var pn = Nodes[row, col];
        pn.UnReachable = true;
        if (LevelController.Instance.DebugIt != null)
        {
            var sr = pn.GetComponent<SpriteRenderer>();
            sr.color = Color.red;
        }
    }
}
