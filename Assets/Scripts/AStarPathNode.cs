using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AStarPathNode 
{
    public PathNode Home { get; set; }
    public AStarPathNode TopNeighbor { get; set; }
    public AStarPathNode RightNeighbor { get; set; }
    public AStarPathNode BottomNeighbor { get; set; }
    public AStarPathNode LeftNeighbor { get; set; }
    public AStarPathNode PathParent { get; set; }
    public int PathGraphRow { get; private set; }
    public int PathGraphCol { get; private set; }

    private bool _isDebugMode;

    public bool IsDebugMode
    {
        get { return _isDebugMode; }
        set 
        { 
            _isDebugMode = value;
            _debugCanvas.enabled = value;
        }
    }


    private int _gCost;
    public int GCost
    {
        get { return _gCost; }
        set 
        { 
            _gCost = value;
            _debugUIController.GCost = value;
        }
    }

    private int _hCost;
    public int HCost
    {
        get { return _hCost; }
        set 
        { 
            _hCost = value;
            _debugUIController.HCost = value;
        }
    }
    private AStarPanelController _debugUIController;
    private Canvas _debugCanvas;


    public AStarPathNode(PathNode home)
    {
        Home = home;
        _debugUIController = home.GetComponentInChildren<AStarPanelController>();
        _debugCanvas = home.GetComponentInChildren<Canvas>();
        _debugCanvas.enabled = false;
    }

    public void SetPathGraphCoordinates(int row, int col)
    {
        PathGraphRow = row;
        PathGraphCol = col;
    }

    public int FCost { get { return _gCost + HCost; } }

    //This will be refactored away when the real A* algorithm goes in to effect
    public List<PathNode> GetNeighbors()
    {
        List<PathNode> retVal = new List<PathNode>();
        if (TopNeighbor != null) retVal.Add(TopNeighbor.Home);
        if (RightNeighbor != null) retVal.Add(RightNeighbor.Home);
        if (BottomNeighbor != null) retVal.Add(BottomNeighbor.Home);
        if (LeftNeighbor != null) retVal.Add(LeftNeighbor.Home);
        return retVal;
    }

    public List<AStarPathNode> Neighbors()
    {
        var retVal = new List<AStarPathNode>();
        if (TopNeighbor != null) retVal.Add(TopNeighbor);
        if (RightNeighbor != null) retVal.Add(RightNeighbor);
        if (BottomNeighbor != null) retVal.Add(BottomNeighbor);
        if (LeftNeighbor != null) retVal.Add(LeftNeighbor);
        return retVal;
    }

    public void Reset()
    {
        GCost = 0;
        HCost = 0;
        PathParent = null;
    }
}
