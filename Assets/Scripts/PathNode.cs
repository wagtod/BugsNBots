using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathNode : MonoBehaviour {

    public const float WIDTH = 0.64f;
    public const float HEIGHT = 0.64f;

    public AStarPathNode AStar { get; set; }
    public bool AllowBuildTurret { get; set; }
    public bool AllowBuildGenerator { get; set; }
    public SpriteRenderer GeneratorSR { get; set; }
    public Text GeneratorTEXT { get; set; }
    public bool UnReachable { get; internal set; }

    private SpriteRenderer _sprite;

    private float _hideMeTimer;
    private bool _amVisible;
    private float _flashDuration;
    private float _flashTimer;
    private Color _flashColor;
    


    public void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        //_sprite.color = UnReachable ? Color.red : Color.black;  //Show nodes for debug.
    }

    private void Awake()
    {
        AStar = new AStarPathNode(this);
        _amVisible = false;
        _hideMeTimer = 0;
        AllowBuildTurret = true;
        AllowBuildGenerator = false;
        _flashDuration = 0f;
        _flashTimer = 0f;
        _flashColor = new Color(0, 0, 0, 0); 
    }

    public static PathNode MakePathNode(Vector2 position)
    {
        UnityEngine.Object prefab = Resources.Load("Prefabs/PathNode");
        GameObject gobj = (GameObject)GameObject.Instantiate(prefab, position, Quaternion.identity);
        return gobj.AddComponent<PathNode>();
    }

    public void Update()
    {
        //DebugDraw(AStar.TopNeighbor.Home);
        //DebugDraw(AStar.RightNeighbor.Home);
        //DebugDraw(AStar.LeftNeighbor.Home);
        //DebugDraw(AStar.BottomNeighbor.Home);
        if (_amVisible)
        {
            _hideMeTimer -= Time.deltaTime;
            if (_hideMeTimer <= 0)
            {
                HideMe();
            }
        }
        HandleFlash();
    }

    private void HandleFlash()
    {
        if (_flashDuration > 0) //Flash in progess
        {
            _flashDuration -= Time.deltaTime;
            _flashTimer -= Time.deltaTime;
            if (_flashTimer <= 0)
            {
                _flashTimer = .15f;
                if (_sprite.color == _flashColor)
                {
                    _sprite.color = new Color(0, 0, 0, 0);
                }
                else
                {
                    _sprite.color = _flashColor;
                }
            }
        }
        else if (_flashDuration < 0)    //Last flash
        {
            _sprite.color = new Color(0, 0, 0, 0);
            _flashDuration = 0f;
            _flashTimer = 0f;
        }
    }

    public void Flash(Color flashColor, float flashDuration)
    {
        _flashColor = flashColor;
        _flashTimer = .15f;
        _flashDuration = flashDuration;
    }

    public void HideMe()
    {
        if (_sprite != null)
        {
            _sprite.color = new Color(0, 0, 0, 0);
            _amVisible = false;
        }
    }

    public void ColorMe(Color color)
    {
        _amVisible = true;
        _sprite.color = color;
    }


    private void DebugDraw(PathNode neighbor)
    {
        if (neighbor != null)
        {
            Debug.DrawLine(transform.position, neighbor.transform.position, Color.black);
        }
    }

    private void OnMouseOver()
    {
        if (UIButtonHandler.Instance != null && UIButtonHandler.Instance.IsTurretClicked)
        {
            if (CanBuildTurretHere())
            {
                _sprite.color = Color.green;
            }
            else
            {
                _sprite.color = Color.red;
            }
            _amVisible = true;
            _hideMeTimer = 0.1f;
        }
    }

    public bool CanBuildTurretHere()
    {
        var playerNode = LevelController.Instance.GetPlayerNode();
        bool canBuild = false;
        if (AllowBuildTurret)
        {
            if (playerNode.AStar.TopNeighbor?.Home == this ||
                playerNode.AStar.RightNeighbor?.Home == this ||
                playerNode.AStar.BottomNeighbor?.Home == this ||
                playerNode.AStar.LeftNeighbor?.Home == this ||
                playerNode == this)
            {
                canBuild = true;
            }
        }
        return canBuild;
    }
}

public  struct NodePair
{
    public PathNode StartNode;
    public PathNode EndNode;
}