using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class LevelController : Singleton<LevelController> {

    public int LevelToLoad;
    public static GameSettings GameSettings;

    [SerializeField]
    private GameObject _player;
    private BotController _playerController;
    private AudioSource _audioSource;
    private AudioClip _generatorBuildSound;
    private AudioClip _winningSound;

    public LayerMask LayersThatDontBlockLineOfSight;
    private GameObject _testBug;
    private SpawnController _spawnController;
    private GameObject _pickUpPreFab;

    public int GeneratorCost { get; private set; }

    public PathGraph Graph { get; private set; }
    public DebugIt DebugIt { get; set; }

    public bool HasShields { get; private set; }
    public bool HasFreeze { get; private set; }
    public bool HasAmmoPlus { get; private set; }
    public bool HasBfg { get; internal set; }
    public bool HasSpeed { get; internal set; }
    public bool HasWin { get; internal set; }
    public bool IsDebugEnabled { get; internal set; }

    void Awake()
    {
        switch (LevelToLoad)
        {
            case 0:
                AwakeMenu();
                break;
            case 1:
                AwakeLevel1();
                break;
            case 2:
                AwakeLevel2();
                break;
            case 3:
                AwakeLevel3();
                break;
            default:
                break;
        }
    }

    private void AwakeMenu()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.volume = 1;
        _playerController = _player.GetComponent<BotController>();
        InitTurretController(0);
    }

    protected void AwakeLevel1()
    {
        _audioSource = GetComponent<AudioSource>();
        _playerController = _player.GetComponent<BotController>();
        BotController.AudioSource = _audioSource;
        BotController.PickupEnergySound = (AudioClip)Resources.Load("SoundWaves\\sndPickUpMetal");
        BotController.PlayerDiedSound = (AudioClip)Resources.Load("SoundWaves\\sndPlayerDeath");
        GeneratorCost = 70;
        InitTurretController(40);
    }

    protected void AwakeLevel2()
    {
        _audioSource = GetComponent<AudioSource>();
        _playerController = _player.GetComponent<BotController>();
        BotController.AudioSource = _audioSource;
        BotController.PickupEnergySound = (AudioClip)Resources.Load("SoundWaves\\sndPickUpMetal");
        BotController.PlayerDiedSound = (AudioClip)Resources.Load("SoundWaves\\sndPlayerDeath");
        GeneratorCost = 85;
        InitTurretController(50);
    }

    protected void AwakeLevel3()
    {
        _audioSource = GetComponent<AudioSource>();
        _playerController = _player.GetComponent<BotController>();
        BotController.AudioSource = _audioSource;
        BotController.PickupEnergySound = (AudioClip)Resources.Load("SoundWaves\\sndPickUpMetal");
        BotController.PlayerDiedSound = (AudioClip)Resources.Load("SoundWaves\\sndPlayerDeath");
        GeneratorCost = 100;
        InitTurretController(60);
    }

    void Start ()
    {
        switch (LevelToLoad)
        {
            case 0:
                InitMenu();
                break;
            case 1:
                InitLevel1();
                break;
            case 2:
                InitLevel2();
                break;
            case 3:
                InitLevel3();
                break;
            default:
                break;
        }
    }

    private void InitMenu()
    {
        TurretController.ResetAll();
        LayersThatDontBlockLineOfSight = ~LayersThatDontBlockLineOfSight;
        Graph = new PathGraph(9, 15, new Vector2(-6.65f, 3.90f));
        for (int row = 1; row <= 7; row++)
        {
            for (int col = 1; col <= 13; col++)
            {
                Graph.SetUnreachableNode(row, col);
            }
        }
        _playerController.IsAttractMode = true;
        InitBulletController();
        InitSpawnController(0, 0, 1000, new Vector2(6.75f, -3.75f), 0, GameSettings.DifficultyLevels.Easy);
        BugController bug = _spawnController.Spawn();
        bug.IgnoreTurrets = true;

        SpawnTurret(0, 7);
        SpawnTurret(4, 0);
        SpawnTurret(8, 7);
        SpawnTurret(4, 14);
        //DebugIt = new DebugIt(Graph, _spawnController.BugsOnMap[0]);
        //Debug.Log(Application.persistentDataPath);
    }

    private void InitLevelAny()
    {
        TurretController.ResetAll();
        LayersThatDontBlockLineOfSight = ~LayersThatDontBlockLineOfSight;
        Graph = new PathGraph(9, 15, new Vector2(-3.93f, 4.57f));
        _generatorBuildSound = (AudioClip)Resources.Load("SoundWaves\\sndGeneratorBuild");

        InitBulletController();
        _pickUpPreFab = (GameObject)Resources.Load("Prefabs\\EnergyPickUp");
        _winningSound = (AudioClip)Resources.Load("SoundWaves\\sndWin");
    }

    private void InitLevel1()
    {
        InitLevelAny();
        var generatorGO = (GameObject)Resources.Load("Prefabs\\GeneratorWithText");
        Graph.SetGeneratorNode(8, 14, generatorGO);
        Graph.SetGeneratorNode(8, 13, generatorGO);
        Graph.SetGeneratorNode(8, 12, generatorGO);
        Graph.SetGeneratorNode(8, 11, generatorGO);
        Graph.SetGeneratorNode(8, 10, generatorGO);
        Graph.SetGeneratorNode(8, 9, generatorGO);

        InitSpawnController(9, 10, 6, new Vector2(3.75f, 5f), 8, GameSettings.LastDifficulty);
        InitTestBug();
        IsDebugEnabled = false;
        _spawnController.Spawn();

    }
    private void InitLevel2()
    {
        InitLevelAny();
        var generatorGO = (GameObject)Resources.Load("Prefabs\\GeneratorWithText");
        Graph.SetGeneratorNode(2, 0, generatorGO);
        Graph.SetGeneratorNode(7, 0, generatorGO);
        Graph.SetGeneratorNode(2, 14, generatorGO);
        Graph.SetGeneratorNode(7, 14, generatorGO);
        Graph.SetGeneratorNode(2, 2, generatorGO);
        Graph.SetGeneratorNode(2, 12, generatorGO);

        InitSpawnController(8, 10, 6, new Vector2(2.75f, 5.15f), 10, GameSettings.LastDifficulty);
        InitTestBug();
        //DebugIt = new DebugIt(Graph, _testBug);
        IsDebugEnabled = false;
        _spawnController.Spawn();
    }

    private void InitLevel3()
    {
        InitLevelAny();
        var generatorGO = (GameObject)Resources.Load("Prefabs\\GeneratorWithText");
        Graph.SetGeneratorNode(1, 7, generatorGO);
        Graph.SetGeneratorNode(2, 7, generatorGO);
        Graph.SetGeneratorNode(3, 7, generatorGO);
        Graph.SetGeneratorNode(4, 7, generatorGO);
        Graph.SetGeneratorNode(5, 7, generatorGO);
        Graph.SetGeneratorNode(6, 7, generatorGO);

        InitSpawnController(10, 10, 6, new Vector2(2.75f, 5.15f), 12, GameSettings.LastDifficulty);
        InitTestBug();
        //DebugIt = new DebugIt(Graph, _testBug);
        IsDebugEnabled = false;
        _spawnController.Spawn();
    }

    private void InitTurretController(int cost)
    {
        TurretController.PreFab = (GameObject)Resources.Load("Prefabs\\Turret");
        TurretController.TurretBuildSound = (AudioClip)Resources.Load("SoundWaves\\sndTurretBuild");
        TurretController.TurretDestroyedSound = (AudioClip)Resources.Load("SoundWaves\\sndTurretDeath");
        TurretController.TurretOutOfAmmoSound = (AudioClip)Resources.Load("SoundWaves\\sndTurretOutOfAmmo");
        TurretController.TurretAudioSource = _audioSource;
        TurretController.TurretCost = cost;
    }

    private void InitBulletController()
    {
        BulletController.PlayerBullet = (GameObject)Resources.Load("Prefabs\\Bullet");
        BulletController.TurretBullet = (GameObject)Resources.Load("Prefabs\\TurretBullet");
        BulletController.TurretFreezeBullet = (GameObject)Resources.Load("Prefabs\\TurretFreezeBullet");
        BulletController.PlayerBulletSoundNormal = (AudioClip)Resources.Load("SoundWaves\\sndFirePlayerBullet");
        BulletController.PlayerBulletSoundBFG = (AudioClip)Resources.Load("SoundWaves\\sndFirePlayerBFG");
        BulletController.TurretBulletSound = (AudioClip)Resources.Load("SoundWaves\\sndFireTurretGun");
        BulletController.BulletAudioSource = _audioSource;
    }

    internal List<GameObject> BugsOnMap()
    {
        return _spawnController.BugsOnMap;
    }

    private void InitSpawnController(int spawnRate, int attackValue, int hitpoints, Vector2 spawnPoint, int maxBugs, 
        GameSettings.DifficultyLevels difficulty)
    {
        switch (difficulty)
        {
            case GameSettings.DifficultyLevels.Easy:
                BugController.Speed = 1.4f;
                break;
            case GameSettings.DifficultyLevels.Normal:
                BugController.Speed = 1.5f;
                spawnRate -= 1;
                attackValue += 4;
                hitpoints += 2;
                break;
            case GameSettings.DifficultyLevels.Hard:
                BugController.Speed = 1.6f;
                spawnRate -= 3;
                attackValue += 5;
                hitpoints += 5;
                break;
        }
        _spawnController = gameObject.AddComponent<SpawnController>();
        _spawnController.Init((GameObject)Resources.Load("Prefabs\\Bug"), spawnPoint);
        _spawnController.SpawnRate = spawnRate;
        _spawnController.AttackValue = attackValue;
        _spawnController.HitPoints = hitpoints;
        _spawnController.MaxBugsAllowed = maxBugs;
        BugController.BugAudioSource = _audioSource;
        BugController.BugDeathSound = (AudioClip)Resources.Load("SoundWaves\\sndBugDeath");
        BugController.BugHitSound = (AudioClip)Resources.Load("SoundWaves\\sndHitBug");
        BugController.BugHitBFGSound = (AudioClip)Resources.Load("SoundWaves\\sndHitBFGBug");
        BugController.BugFreezeSound = (AudioClip)Resources.Load("SoundWaves\\sndHitBugFreeze");
    }

    private void InitTestBug()
    {
        _testBug = GameObject.Find("TestBug");
        if (_testBug != null)
        {
            BugController bc = _testBug.GetComponent<BugController>();
            _spawnController.BugsOnMap.Add(_testBug);
            bc.AttackValue = 0;
            bc.InitHitPoints(50);
            //DebugIt = new DebugIt(Graph, _testBug);
        }
    }

    internal PathNode GetRandomNode(PathNode excludeNode)
    {
        PathNode retVal = Graph.GetRandomNode();
        while (retVal == excludeNode)
        {
            retVal = Graph.GetRandomNode();
        }
        return retVal;
    }

    // Update is called once per frame
    void Update ()
    {
        if (IsDebugEnabled)
        {
            Debugs();
        }
    }

    internal List<PathNode> GetPathBetween(Vector3 position, PathNode destNode)
    {
        var retVal = new List<PathNode>();
        PathNode start = Graph.FindClosestNode(position);
        var asps = new AStarPathSearch();
        retVal = asps.FindPath(start.AStar, destNode.AStar);
        return retVal;
    }


    internal bool TryBuildShieldGenerator()
    {
        bool retVal = BuildGenerator(b => HasShields = b, "Shields");
        if (retVal)
        {
            _playerController.InitShields(25, 2);
        }
        return retVal;
    }

    private bool BuildGenerator(Action<bool> markAsBuilt, string title)
    {
        bool retVal = false;
        var pn = GetPlayerNode();
        if (pn.AllowBuildGenerator)
        {
            retVal = true;
            markAsBuilt(true);
            pn.AllowBuildGenerator = false;
            _playerController.Energy.CurrentValue -= GeneratorCost;
            pn.GeneratorSR.color = new Color(1f, 1f, 1f, 1f); //Full color
            pn.GeneratorTEXT.text = title;
            _audioSource.PlayOneShot(_generatorBuildSound, 1f);
            _spawnController.SpawnRate -= 1;  //Faster spawn
            BugController.Speed += 0.1f;
        }
        else
        {
            Graph.FlashGenLocations(pn);
        }
        return retVal;
    }

    internal bool TryBuildAmmoPlusGenerator()
    {
        return BuildGenerator(b => HasAmmoPlus = b, "Ammo+");
    }

    private void Debugs()
    {
        if (Input.GetKeyDown("space"))
        {
            //List<PathNode> path = DebugIt.CreateTestPath(_player);
            //DebugIt.DisplayCurrent(path);
            _spawnController.Spawn();
        }
        if (Input.GetKeyDown("f1"))
        {
            if (DebugIt != null)
            {
                DebugIt.DisplayNextInAllBest();
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (_testBug != null)
            {
                List<PathNode> path = DebugIt.GetPathBetweenBugAndMouse();
                DebugIt.DisplayCurrent(path);
                //BugController bc = _testBug.GetComponent<BugController>();
                //bc.SetNewPath(path);
            }
        }
    }

    internal bool TryBuildBFGGenerator()
    {
        bool retVal = BuildGenerator(b => HasBfg = b, "BFG");
        if (retVal)
        {
            _playerController.HasBFG = true;
        }
        return retVal;
    }

    internal bool TryBuildFreezeGenerator()
    {
        return BuildGenerator(b => HasFreeze = b, "Freeze");
    }

    internal bool TryBuildSpeedGenerator()
    {
        bool retVal = BuildGenerator(b => HasSpeed = b, "Speed");
        if (retVal)
        {
            _playerController.Speed = 2.2f;
        }
        return retVal;
    }

    internal bool TryBuildWinGenerator()
    {
        BuildGenerator(b => HasWin = b, "Winner");
        if (HasWin)
        {
            _audioSource.PlayOneShot(_winningSound, 1f);
            Time.timeScale = 0f;
            StartCoroutine(LoadNextLevel());
        }
        return HasWin;
    }

    internal IEnumerator LoadNextLevel()
    {
        string loadScene = string.Empty;
        switch (LevelToLoad)
        {
            case 1:
                UIButtonHandler.Instance.DisplayGameOver($"You Defeated Level 1 on {GameSettings.LastDifficulty}.");
                loadScene = "Level02";
                yield return WaitForRealSeconds(3.0f);
                break;
            case 2:
                UIButtonHandler.Instance.DisplayGameOver($"You Defeated Level 2 on {GameSettings.LastDifficulty}.");
                loadScene = "Level03";
                yield return WaitForRealSeconds(3.0f);
                break;
            case 3:
                switch (GameSettings.LastDifficulty)
                {
                    case GameSettings.DifficultyLevels.Easy:
                        GameSettings.MaxDifficulty = GameSettings.DifficultyLevels.Normal;
                        UIButtonHandler.Instance.DisplayGameOver($"Congratulations! You unlocked {GameSettings.DifficultyLevels.Normal} difficulty.");
                        break;
                    case GameSettings.DifficultyLevels.Normal:
                        GameSettings.MaxDifficulty = GameSettings.DifficultyLevels.Hard;
                        UIButtonHandler.Instance.DisplayGameOver($"Congratulations! You unlocked {GameSettings.DifficultyLevels.Hard} difficulty.");
                        break;
                    default:
                        UIButtonHandler.Instance.DisplayGameOver($"Congratulations! You beat {GameSettings.DifficultyLevels.Hard}.  IT'S OVER!");
                        break;
                }
                loadScene = "MainMenu";
                yield return WaitForRealSeconds(3.0f);
                GameSettings.LastDifficulty = GameSettings.MaxDifficulty;
                break;
            default:
                break;
        }
        Time.timeScale = 1f;
        StartCoroutine(UIButtonHandler.Instance.LoadSceneWithAmimation(loadScene, "Exit"));
    }

    public static IEnumerator WaitForRealSeconds(float time)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + time)
        {
            yield return null;
        }
    }

    public bool IsClearPath(GameObject A, GameObject B)
    {
        bool retVal = true;
        Physics2D.queriesStartInColliders = true;
        Vector3 direction = B.transform.position - A.transform.position;
        float distance = Vector3.Distance(A.transform.position, B.transform.position);
        RaycastHit2D[] hits = Physics2D.RaycastAll(A.transform.position, direction, distance);
        foreach (var hit in hits)
        {
            if (hit.collider.GetComponent<SpriteRenderer>().sortingLayerName == "Walls")
            {
                retVal = false;
                break;
            }
        }
        return retVal;
    }

    public Vector3 GetPlayerLocation()
    {
        return _player.transform.position;
    }

    public PathNode GetPlayerNode()
    {
        return Graph.FindClosestNode(_player.transform.position);
    }

    //public List<PathNode> GetPathToTarget(Vector3 startPosition, Vector3 targetPosition, out int recursiveDepth)
    //{
    //    var retVal = new List<PathNode>();
    //    recursiveDepth = 0;
    //    PathNode startNode = Graph.FindClosestNode(startPosition);
    //    PathNode targetNode = Graph.FindClosestNode(targetPosition);
    //    var ps = new PathSearch();
    //    retVal = ps.FindPathBetween(startNode, targetNode, Graph.MinimumDistanceBetweenNodes(startNode, targetNode));
    //    recursiveDepth = ps.CurrentRecuriveDepth;
    //    return retVal;
    //}

    public List<PathNode> GetPathToTarget(Vector3 startPosition, Vector3 targetPosition)
    {
        var retVal = new List<PathNode>();
        PathNode startNode = Graph.FindClosestNode(startPosition);
        PathNode targetNode = Graph.FindClosestNode(targetPosition);
        var asps = new AStarPathSearch();
        retVal = asps.FindPath(startNode.AStar, targetNode.AStar);
        return retVal;
    }

    public BotController GetPlayerController()
    {
        return _player.GetComponent<BotController>();
    }

    public bool CanSee(Vector3 source, Vector3 target, string targetTag, ref Vector3 targetPosition)
    {
        bool retVal = false;

        Physics2D.queriesStartInColliders = false;
        Vector3 direction = target - source;
        RaycastHit2D[] hits = Physics2D.RaycastAll(source, direction, 2000.0f);
        //Debug.DrawRay(source, direction, Color.yellow, 2000.0f);
        RaycastHit2D targetHit = (from h in hits where h.collider.tag == targetTag orderby h.distance select h).FirstOrDefault();

        if (targetHit != default(RaycastHit2D))
        {
            var foundWall = (from h in hits
                             where h.collider.tag == "Walls" && h.distance <= targetHit.distance
                             select h).ToList();
            if (foundWall.Count() == 0)
            {
                retVal = true;
                targetPosition = targetHit.transform.position;
            }
            //else
            //{
            //    foreach (var wall in foundWall)
            //    {
            //        Debug.Log(wall.collider.name);
            //    }
            //}
        }
        return retVal;
    }

    public bool CanSeeTarget(Transform source, string targetTag, ref Vector3 targetPosition)
    {
        bool retVal = false;
        //Debug.DrawLine(source.position, new Vector3(source.position.x, source.position.y - 1000, 0), Color.yellow, 2000.0f);

        if (CanSee(source.position, new Vector3(source.position.x + 1000, source.position.y, 0), targetTag, ref targetPosition))
        {
            retVal = true;
        }
        else if (CanSee(source.position, new Vector3(source.position.x, source.position.y + 1000, 0), targetTag, ref targetPosition))
        {
            retVal = true;
        }
        else if (CanSee(source.position, new Vector3(source.position.x - 1000, source.position.y, 0), targetTag, ref targetPosition))
        {
            retVal = true;
        }
        else if (CanSee(source.position, new Vector3(source.position.x, source.position.y - 1000, 0), targetTag, ref targetPosition))
        {
            retVal = true;
        }
        if (retVal)
        {
            Debug.DrawLine(source.position, targetPosition, Color.red);
        }

        return retVal;
    }

    public void BugDied(GameObject bug)
    {
        Instantiate(_pickUpPreFab, bug.transform.position, Quaternion.identity);
        _spawnController.BugDied(bug);
    }

    private void SpawnTurret(int row, int col)
    {
        PathNode pn = LevelController.Instance.Graph.Nodes[row, col];
        int maxShields = 0;
        int ammunition = 10000;
        float turnSpeed = 200;
        if (TurretController.TrySpawn(pn.transform.position, 1, maxShields, ammunition, turnSpeed, false, 0.50f))
        {
            pn.AllowBuildTurret = false;
        }
    }

    public static void StopGame()
    {
        GameSettings.Persist(LevelController.GameSettings);
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    public void PlayerHasDied()
    {
        UIButtonHandler.Instance.DisplayGameOver("D E F E A T !!");
        WaitForRealSeconds(2.0f);
        UIButtonHandler.Instance.MenuButtonClicked();
    }
}
