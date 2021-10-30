using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class TurretControllerDebug : MonoBehaviour
{
    private GameObject _target;
    private GameObject _barrelA;
    private GameObject _barrelB;

    public static GameObject PreFab { get; set; }
    public float FireRate { get; set; }
    public float TargetingRate { get; set; }
    public int AttackValue { get; set; }
    public static List<GameObject> TurretsOnMap { get; set; }

    static TurretControllerDebug()
    {
        TurretsOnMap = new List<GameObject>();
    }

    void Start()
    {
        BulletController.TurretBullet = (GameObject)Resources.Load("Prefabs\\TurretBullet");
        TurretsOnMap.Add(this.gameObject);
        _target = null;
        FireRate = .25f;
        TargetingRate = .50f;
        AttackValue = 1;
        _barrelA = transform.Find("GunBarrelA").gameObject;
        _barrelB = transform.Find("GunBarrelB").gameObject;
    }

    void Update()
    {
        //if (Input.GetKey("a"))
        //{
        //    transform.Rotate(transform.forward, 10f * Time.deltaTime * 10);
        //}
        //if (Input.GetKey("d"))
        //{
        //    transform.Rotate(transform.forward, -10f * Time.deltaTime * 10);
        //}
        //if (Input.GetKey("space"))
        //{
        //    //List<PathNode> path = DebugIt.CreateTestPath(_player);
        //    //DebugIt.DisplayCurrent(path);
        //    FireTarget();
        //}

    }

    public static void Spawn(Vector2 spawnPoint, int attackValue)
    {
        GameObject newTurret = Instantiate(PreFab, spawnPoint, Quaternion.identity);
        TurretController tc = newTurret.GetComponent<TurretController>();
        TurretsOnMap.Add(newTurret);
        tc.AttackValue = attackValue;
    }

    private void FaceTarget()
    {
        Vector3 vectorToTarget = _target.transform.position - transform.position;
        float angle = (Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg) - 90f;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, Time.deltaTime * 200);
     }

    private void FireTarget()
    {
        //BulletController.FireTurretBullet(transform.position, _target.transform.position, transform.rotation);
        BulletController.FireTurretBullet(_barrelA.transform.position, _barrelA.transform.up, transform.rotation);
        BulletController.FireTurretBullet(_barrelB.transform.position, _barrelB.transform.up, transform.rotation);

        //BulletController.FireTurretBullet(_barrelB, _target.transform.position, transform.rotation, 8);
    }



    public void TakeDamage(int damage)
    {
        TurretsOnMap.Remove(this.gameObject);
        Destroy(this.gameObject);
    }

}
