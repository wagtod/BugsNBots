using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugController : MonoBehaviour
{
    public static AudioSource BugAudioSource { get; set; }
    public static AudioClip BugDeathSound { get; set; }
    public static AudioClip BugHitSound { get; set; }
    public static AudioClip BugHitBFGSound { get; set; }
    public static AudioClip BugFreezeSound { get; set; }

    public float AttackRate { get; set; }
    private bool _canAttack;
    private float _attackTimer;
    public bool IgnoreTurrets { get; set; }

    public static float Speed = 1.5f;
    private Rigidbody2D _body;

    private List<PathNode> _currentPath;
    private int _destinationNDX;
    private bool _isPathEnd;
    private bool _isNodeEnd;
    private bool _frozen;
    private float _freezeTimer;
    private DebugIt _debugIt;
    private PathNode _currentNode;

    [SerializeField]
    private Stat _health;
    private int _highestRecursiveDepth;

    public float HitPoints
    {
        get { return _health.CurrentValue;}
        set { _health.CurrentValue = value; }
    }
    public void InitHitPoints(float max)
    {
        //_health.Initialize();
        _health.MaxVal = max;
        _health.CurrentValue = max;
        HitPoints = max;
    }

    public int AttackValue { get; set; }

    // Use this for initialization
    void Start()
    {
        AttackRate = 0.4f;
        _canAttack = true;
        _attackTimer = AttackRate;
        _body = GetComponent<Rigidbody2D>();
        _currentPath = new List<PathNode>();
        _isPathEnd = true;
        _isNodeEnd = true;
    }

    private void Awake()
    {
        _health.Initialize();
        _frozen = false;
    }

    void FixedUpdate()
    {
        if (_frozen)
        {
            _freezeTimer -= Time.deltaTime;
            if (_freezeTimer <= 0.0f)
            {
                _frozen = false;
            }
        }
        if (!_canAttack)
        {
            _attackTimer -= Time.deltaTime;
            if (_attackTimer <= 0.0f)
            {
                _canAttack = true;
            }
        }
        FollowPath();
        if (_isNodeEnd)
        {
            int recursiveDepth = 0;
            Vector3 newTarget = default(Vector3);
            if (CanSeeTarget(ref newTarget))
            {
                SetNewPath(LevelController.Instance.GetPathToTarget(this.transform.position, newTarget));
                //Debug.Log("See Player");
            }
            else
            {
                if (_isPathEnd)
                {
                    PathNode pn = LevelController.Instance.GetRandomNode(WhatNodeAmIOn());
                    if (_debugIt != null)
                    {
                        var sr = pn.GetComponent<SpriteRenderer>();
                        sr.color = Color.green;
                        Debug.Log("SetRandom Path");
                    }
                    SetNewPath(LevelController.Instance.GetPathBetween(this.transform.position, pn));
                }
            }
            if (recursiveDepth > _highestRecursiveDepth)
            {
                _highestRecursiveDepth = recursiveDepth;
                Debug.Log($"Bug {this.GetInstanceID()} highest recursive depth = {_highestRecursiveDepth}");
            }
        }
    }

    public void SetNewPath(List<PathNode> path)
    {
        int foundIt = path.FindIndex(pn => pn == WhatNodeAmIOn());
        if (foundIt >= 0)
        {
            _destinationNDX = foundIt;
            _destinationNDX++;  //I keep trying to do without this but it keeps coming back.
            _currentPath = path;
            _isPathEnd = false;
            _isNodeEnd = false;
        }
        else
        {
            Debug.LogError("SetNewPath() on bug did NOT contain its current node.");
        }
    }

    private bool CanSeeTarget(ref Vector3 targetPosition)
    {
        bool retVal = false;
        targetPosition = default(Vector3);
        if (LevelController.Instance.CanSeeTarget(transform, "Player", ref targetPosition))
        {
            retVal = true;
        }
        else if (!IgnoreTurrets && LevelController.Instance.CanSeeTarget(transform, "Turret", ref targetPosition))
        {
            retVal = true;
        }
        return retVal;
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    This is no good as it make the sprite vibrate.
    //    Vector3 move = new Vector3(-1 * Input.GetAxis("Horizontal"), -1 * Input.GetAxis("Vertical"), 0);
    //    float distance = 2 * Time.deltaTime;
    //    transform.position += move * distance;
    //    Debug.Log("Bug Collision with " + collision.name);

    //}

    private PathNode WhatNodeAmIOn()
    {
        if (_currentNode == null)
        {
            _currentNode = LevelController.Instance.Graph.FindClosestNode(_body.transform.position);
        }
        return _currentNode;
    }

    private void FollowPath()
    {
        if (_debugIt != null)
        {
            Debug.Log(string.Format(this.GetInstanceID().ToString() + "  destinationNDX={0} currentPath.Count={1}", _destinationNDX, _currentPath.Count));
        }
        if (_currentPath.Count > 0 && _destinationNDX >= 0 && _destinationNDX < _currentPath.Count)
        {
            _isNodeEnd = false;
            AdvanceDestNdx();
            Vector2 dest = _currentPath[_destinationNDX].transform.position;
            if (transform.position.x < dest.x)
            {
                //look right
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else if (transform.position.x > dest.x)
            {
                //look left
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            float speedMult = _frozen ? 0.5f : 1.0f;
            _currentNode = null;
            transform.position = Vector2.MoveTowards(transform.position, dest, Speed * Time.deltaTime * speedMult);
        }
        else
        {
            _isNodeEnd = true;
        }
    }

    private void AdvanceDestNdx()
    {
        #region 5th Node bug fix:
        // All the nodes 5th from the left cannot be moved to exactly.  The code that exposed the bug was this.x == dest.x.  When moving
        // to these bad nodes, you can never get an exact match at many decimal points.  Solution: use distance function and look for
        // it to go sufficiently small and consider yourself there.
        #endregion
        // Increase the destination index by one if bug has arrived at current destination
        // If at the end of the path, just stay there.
        if (_debugIt != null)
        {
            Debug.DrawLine(this.transform.position, _currentPath[_destinationNDX].transform.position);
            var dist = Vector3.Distance(this.transform.position, _currentPath[_destinationNDX].transform.position);
            Debug.Log(this.GetInstanceID().ToString() + "  Distance=" + dist.ToString());
        }
        if (Vector3.Distance(this.transform.position, _currentPath[_destinationNDX].transform.position) <= 0.001)
        {
            _isNodeEnd = true;

            if ((_destinationNDX + 1) < _currentPath.Count)
            {
                _destinationNDX++;
            }
            else
            {
                _isPathEnd = true;
            }
            PerformDebug();
        }
    }

    private void PerformDebug()
    {
        if (_debugIt != null)
        {
            _debugIt.DisplayCurrent(_currentPath, _currentPath[_destinationNDX]);
        }
    }

    public void TakeDamage(int damage, bool withFreeze)
    {
        HitPoints -= damage;
        if (withFreeze)
        {
            _frozen = true;
            _freezeTimer = 2.5f;
            BugAudioSource.PlayOneShot(BugFreezeSound, 0.6f);
        }
        else
        {
            if (damage >= 3)
            {
                BugAudioSource.PlayOneShot(BugHitBFGSound, Random.Range(0.5f, 1f));
            }
            else
            {
                BugAudioSource.PlayOneShot(BugHitSound, Random.Range(0.2f, 0.5f));
            }
        }
        if (HitPoints <= 0)
        {
            LevelController.Instance.BugDied(this.gameObject);
            BugAudioSource.PlayOneShot(BugDeathSound, 1f);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log($"CanAttack={_canAttack}  Collision with {collision.tag}");
        if (_canAttack)
        {
            if (collision.tag == "Player")
            {
                ResetAttackTimer();
                LevelController.Instance.GetPlayerController().TakeDamage(this.AttackValue);
            }
            else if (collision.tag == "Turret")
            {
                ResetAttackTimer();
                TurretController tc = collision.gameObject.GetComponent<TurretController>();
                tc.TakeDamage(this.AttackValue);
            }
        }
    }


    private void ResetAttackTimer()
    {
        _canAttack = false;
        _attackTimer = AttackRate;
    }
    public void OnMouseDown()
    {
        if (LevelController.Instance.IsDebugEnabled)
        {
            _debugIt = new DebugIt(LevelController.Instance.Graph, this.gameObject);
            Debug.Log("Bug in debug mode: " + this.GetInstanceID().ToString());
        }
    }

}
