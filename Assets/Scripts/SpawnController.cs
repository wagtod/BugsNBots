using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{

    public int SpawnRate { get; set; }
    public int AttackValue { get; set; }
    public float HitPoints { get; set; }
    public int MaxBugsAllowed { get; set; }
    public List<GameObject> BugsOnMap { get; set; }
    
    private GameObject _bugPrefab;
    private Vector2 _spawnPoint;
    private float _timer;

    public void Init(GameObject bugPrefab, Vector2 spawnPoint)
    {
        _bugPrefab = bugPrefab;
        _spawnPoint = spawnPoint;
        BugsOnMap = new List<GameObject>();
    }

    public BugController Spawn()
    {
        GameObject newBug = Instantiate(_bugPrefab, _spawnPoint, Quaternion.identity);
        BugController bc = newBug.GetComponent<BugController>();
        BugsOnMap.Add(newBug);
        bc.AttackValue = AttackValue;
        bc.InitHitPoints(HitPoints);
        return bc;
    }

    void Update()
    {
        if (SpawnRate > 0)
        {
            _timer += Time.deltaTime;
            if (_timer > SpawnRate)
            {
                _timer -= SpawnRate;
                if (BugsOnMap.Count < MaxBugsAllowed)
                {
                    Spawn();
                }
            }
        }
    }

    public void BugDied(GameObject bug)
    {
        BugsOnMap.Remove(bug);
        Destroy(bug);
    }

}
