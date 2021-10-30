using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour
{
    public bool IsAttractMode { get; set; }
    public float Speed { get; set; }
    public static AudioSource AudioSource { get; set; }
    public static AudioClip PickupEnergySound { get; set; }
    public static AudioClip PlayerDiedSound { get; set; }
    private Rigidbody2D _body;
    private Vector2 _currentDestination;
    private ShieldController _shieldController;
    private static Vector2[] _attractDestinations = new Vector2[4] { new Vector2(-6.65f, 3.9f), new Vector2(6.79f, 3.9f), new Vector2(6.79f, -3.78f), new Vector2(-6.65f, -3.78f) };
    private int _attractDestNDX;

    [SerializeField]
    private Stat _health;
    [SerializeField]
    public Stat Energy;

    public float FireRate { get; set; }
    private float _fireTimer;
    public bool HasBFG { get; set; }

    // Use this for initialization
    void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _currentDestination = _body.position;
        _health.MaxVal = 50;
        _health.CurrentValue = 50;
        Energy.MaxVal = 100;
        Energy.CurrentValue = 100;
        FireRate = .33f;
        _fireTimer = 0f;
        Speed = 2.0f;
        _attractDestNDX = 0;
    }


    void Awake()
    {
        _health.Initialize();
        Energy.Initialize();
        if (UIButtonHandler.Instance != null)
        {
            Energy.StatCurrentValueChanged += UIButtonHandler.Instance.HandleEnergyChanged;
        }
        var t = transform.Find("ShieldBubble");
        _shieldController = t.GetComponent<ShieldController>();
        HasBFG = false;
    }

    private void Update()
    {
        if (IsAttractMode)
        {
            ControlledAutomatically();
        }
        else
        {
            ControlledByPlayer();
        }
    }

    private void ControlledAutomatically()
    {
        //Movement for attract mode
        transform.position = Vector2.MoveTowards(transform.position, _attractDestinations[_attractDestNDX], Speed * Time.deltaTime);
        if (transform.position.x == _attractDestinations[_attractDestNDX].x && transform.position.y == _attractDestinations[_attractDestNDX].y)
        {
            _attractDestNDX++;
            if (_attractDestNDX > 3)
            {
                _attractDestNDX = 0;
            }
        }
    }

    private void ControlledByPlayer()
    {
        _fireTimer += Time.deltaTime;
     
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) //Ignore if point over UI element.
        {
            if (!UIButtonHandler.Instance.ClickInProgress())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    ProcessMouseShooting();
                }
            }
            else if (UIButtonHandler.Instance.IsTurretClicked)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    SpawnTurret();
                }
                else if (Input.GetMouseButtonUp(1))
                {
                    UIButtonHandler.Instance.Reset();
                }
            }
        }
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        if (inputX != 0 || inputY != 0)
        {
            ProcessKeyboardMovement(inputX, inputY);
        }
    }

    private void SpawnTurret()
    {
        if (Energy.CurrentValue >= TurretController.TurretCost)
        {
            Vector2 click = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            PathNode pn = LevelController.Instance.Graph.FindClosestNode(click);
            if (pn.CanBuildTurretHere())
            {
                int maxShields = LevelController.Instance.HasShields ? 25 : 0;
                int ammunition = LevelController.Instance.HasAmmoPlus ? 200 : 100;
                float turnSpeed = LevelController.Instance.HasSpeed ? 300 : 200;
                if (TurretController.TrySpawn(pn.transform.position, 1, maxShields, ammunition, turnSpeed, LevelController.Instance.HasFreeze))
                {
                    Energy.CurrentValue -= TurretController.TurretCost;
                    UIButtonHandler.Instance.Reset();
                    pn.AllowBuildTurret = false;
                }
            }
        }
        else
        {
            Energy.Bar.FlashIconDuration = 2;
        }
    }

    private void ProcessMouseShooting()
    {
        if (_fireTimer > FireRate)
        {
            _fireTimer = 0f;
            if (HasBFG)
            {
                BulletController.FirePlayerBullet(transform.position, Input.mousePosition, Speed * 6, 3, 1.5f);
            }
            else
            {
                BulletController.FirePlayerBullet(transform.position, Input.mousePosition, Speed * 10);
            }
        }
    }

    private void ProcessKeyboardMovement(float inputX, float inputY)
    {
        if (inputX != 0 || inputY != 0)
        {
            PathNode currentNode = WhatNodeAmIOn();
            Vector2 newDest = _currentDestination;
            if (inputX < 0 && currentNode.AStar.LeftNeighbor != null)
            {
                newDest = currentNode.AStar.LeftNeighbor.Home.transform.position;
            }
            else if (inputY < 0 && currentNode.AStar.BottomNeighbor != null)
            {
                newDest = currentNode.AStar.BottomNeighbor.Home.transform.position;
            }
            else if (inputX > 0 && currentNode.AStar.RightNeighbor != null)
            {
                newDest = currentNode.AStar.RightNeighbor.Home.transform.position;
            }
            else if (inputY > 0 && currentNode.AStar.TopNeighbor != null)
            {
                newDest = currentNode.AStar.TopNeighbor.Home.transform.position;
            }
            _body.position = Vector2.MoveTowards(_body.position, newDest, Speed * Time.deltaTime);
            _currentDestination = newDest;
        }
    }

    internal void InitShields(int maxShield, int regenPerSecond)
    {
        _shieldController.Init(maxShield, regenPerSecond);
    }

    private PathNode WhatNodeAmIOn()
    {
        return LevelController.Instance.Graph.FindClosestNode(_body.transform.position);
    }

    public void TakeDamage(int damage)
    {
        if (_shieldController.ShieldPoints > 0)
        {
            if (damage <= _shieldController.ShieldPoints)
            {
                _shieldController.ShieldPoints -= damage;
                damage = 0;
            }
            else
            {
                damage -= _shieldController.ShieldPoints;
                _shieldController.ShieldPoints = 0;
            }
        }
        _health.CurrentValue -= damage;
        if (_health.CurrentValue <= 0 && !LevelController.Instance.HasWin)
        {
            AudioSource.PlayOneShot(PlayerDiedSound, 1f);
            Destroy(this.gameObject);
            LevelController.Instance.PlayerHasDied();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "EnergyPickUp")
        {
            Energy.CurrentValue += 15;
            Destroy(collision.gameObject);
            AudioSource.PlayOneShot(PickupEnergySound, 1f);
        }

    }

}
