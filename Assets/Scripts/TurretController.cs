using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class TurretController : MonoBehaviour
{
    private GameObject _target;
    private GameObject _turretGO;
    private float _fireTimer;
    private float _targetTimer;
    private float _deathTimer;
    private int _freezeCount;
    private GameObject _barrelA;
    private GameObject _barrelB;
    private bool _hasLock;
    private ShieldController _shieldController;


    public static GameObject PreFab { get; set; }
    public static AudioClip TurretBuildSound { get; set; }
    public static AudioClip TurretDestroyedSound { get; set; }
    public static AudioClip TurretOutOfAmmoSound { get; set; }
    public static AudioSource TurretAudioSource { get; set; }
    public static int TurretCost { get; set; }
    public float FireRate { get; set; }
    public float TargetingRate { get; set; }
    public float TurnSpeed { get; set; }
    public int AttackValue { get; set; }
    public int Ammunition { get; set; }
    public bool HasFreezeAmmo { get; set; }
    public static List<GameObject> TurretsOnMap { get; set; }

    static TurretController()
    {
        if (TurretsOnMap == null)
        {
            TurretsOnMap = new List<GameObject>();
        }
    }

    private void Start()
    {
    }

    private void Awake()
    {
        _target = null;
        _fireTimer = 0f;
        _targetTimer = 0f;
        //FireRate = .20f;
        TargetingRate = .50f;
        //TurnSpeed = 200;
        //AttackValue = 1;
        _freezeCount = 10;
        //HasFreezeAmmo = false;
        _barrelA = transform.Find("GunBarrelA").gameObject;
        _barrelB = transform.Find("GunBarrelB").gameObject;
    }

    void FixedUpdate()
    {
        _fireTimer += Time.deltaTime;
        _targetTimer += Time.deltaTime;
        if (_targetTimer >= TargetingRate)
        {
            _targetTimer -= TargetingRate;
            SetTarget();
        }
        if (_fireTimer >= FireRate)
        {
            _fireTimer = 0;
            if (_target != null && _hasLock)
            {
                FireTarget();
            }
        }
        if (_target != null && Ammunition > 0)
        {
            FaceTarget();
        }
        else
        {
            _hasLock = false;
        }
        if (Ammunition <= 0)
        {
            _deathTimer += Time.deltaTime;
            if (_deathTimer > 5f)
            {
                TurretAudioSource.PlayOneShot(TurretOutOfAmmoSound, 1f);
                RemoveTurret();
            }
        }
 
    }

    internal void InitShields(int maxShield, int regenPerSecond)
    {
        _shieldController.Init(maxShield, regenPerSecond);
    }

    public static bool TrySpawn(Vector2 spawnPoint, int attackValue, int maxShield, int ammunition, float turnSpeed, bool hasFreeze, float fireRate = 0.20f)
    {
        bool retVal = false;
        if (!HasTurret(spawnPoint))
        {
            GameObject newTurret = Instantiate(PreFab, spawnPoint, Quaternion.identity);
            Transform turretBody = newTurret.transform.Find("TurretMain");
            TurretController tc = turretBody.GetComponent<TurretController>();
            tc._turretGO = newTurret;
            TurretsOnMap.Add(newTurret);
            tc.AttackValue = attackValue;
            Transform sb = newTurret.transform.Find("ShieldBubble");
            tc._shieldController = sb.GetComponent<ShieldController>();
            tc.InitShields(maxShield, 2);
            tc.Ammunition = ammunition;
            tc.TurnSpeed = turnSpeed;
            tc.HasFreezeAmmo = hasFreeze;
            tc.FireRate = fireRate;
            if (TurretAudioSource != null)
            {
                TurretAudioSource.PlayOneShot(TurretBuildSound, 1f);
            }
            retVal = true;
        }
        return retVal;
    }

    internal static void ResetAll()
    {
        if (TurretsOnMap != null)
        {
            TurretsOnMap.Clear();
        } 
        TurretsOnMap = new List<GameObject>();
    }

    private static bool HasTurret(Vector2 spawnPoint)
    {
        GameObject foundIt = null;
        if (spawnPoint != null)
        {
            foundIt = (from t in TurretsOnMap
                           where t.transform.position.x == spawnPoint.x && t.transform.position.y == spawnPoint.y
                           select t).FirstOrDefault();

        }        
        return (foundIt != null);
    }

    private void FaceTarget()
    {
        Vector3 vectorToTarget = _target.transform.position - transform.position;
        float angle = (Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg) - 90f;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, Time.deltaTime * TurnSpeed);
        if (Vector3.Distance(transform.eulerAngles, q.eulerAngles) <= 5f)
        {
            _hasLock = true;
        }
        else
        {
            _hasLock = false;
        }
    }

    private void FireTarget()
    {
        if (Ammunition > 0)
        {
            if (HasFreezeAmmo)
            {
                _freezeCount -= 2;
            }
            Ammunition -= 2;

            if (_freezeCount <= 0 && HasFreezeAmmo)
            {
                _freezeCount = 10;
                BulletController.FireTurretFreezeBullet(_barrelA.transform.position, _barrelA.transform.up, transform.rotation);
                BulletController.FireTurretFreezeBullet(_barrelB.transform.position, _barrelB.transform.up, transform.rotation);
            }
            else
            {
                BulletController.FireTurretBullet(_barrelA.transform.position, _barrelA.transform.up, transform.rotation);
                BulletController.FireTurretBullet(_barrelB.transform.position, _barrelB.transform.up, transform.rotation);
            }
            if (Ammunition <= 0)
            {
                TurretAudioSource.PlayOneShot(TurretOutOfAmmoSound, 1f);
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                sr.color = new Color(1f, 1f, 1f, 0.50f);
                _deathTimer = 0f;
            }
        }
    }

    private void SetTarget()
    {
        //Is existing target still visible
        if (_target != null)
        {
            Vector3 direction = _target.transform.position - transform.position;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1000f, LevelController.Instance.LayersThatDontBlockLineOfSight);
            if (hit != default(RaycastHit2D) &&  hit.collider.CompareTag("Enemy"))
            {
                _target = hit.collider.gameObject;
            }
            else
            {
                _target = null;
            }
        }

        if (_target == null)
        {
            //Look for closest new target
            Physics2D.queriesStartInColliders = false;
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            foreach (GameObject target in LevelController.Instance.BugsOnMap())
            {
                Vector3 direction = target.transform.position - transform.position;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1000f, LevelController.Instance.LayersThatDontBlockLineOfSight);
                if ((hit.collider != null) && (hit.collider.CompareTag("Enemy")))
                {
                    hits.Add(hit);
                }
            }
            if (hits.Count > 0)
            {
                RaycastHit2D targetHit = (from h in hits orderby h.distance select h).FirstOrDefault();
                _target = targetHit.collider.gameObject;
            }
        }
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
        if (damage > 0)
        {
            RemoveTurret();
            TurretAudioSource.PlayOneShot(TurretDestroyedSound, 0.8f);
        }
    }

    private void RemoveTurret()
    {
        PathNode pn = LevelController.Instance.Graph.FindClosestNode(transform.position);
        pn.AllowBuildTurret = true;
        TurretsOnMap.Remove(_turretGO);
        Destroy(_turretGO);
    }
}
