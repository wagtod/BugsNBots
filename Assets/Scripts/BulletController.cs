using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour
{
    public int Damage { get; set; }
    public static GameObject PlayerBullet { get; set; }
    public static GameObject TurretBullet { get; set; }
    public static GameObject TurretFreezeBullet { get; set; }
    public static AudioClip PlayerBulletSoundNormal { get; set; }
    public static AudioClip PlayerBulletSoundBFG { get; set; }
    public static AudioClip TurretBulletSound { get; set; }
    public static AudioSource BulletAudioSource { get; set; }

    private bool _hasFreeze;

    void Start()
    {
    }

    public static void FirePlayerBullet(Vector2 origin, Vector2 direction, float speed = 18, int damage = 1, float scaleMultiplier = 1)
    {
        GameObject bulletPreFab = Instantiate(PlayerBullet, origin, Quaternion.identity);
        direction = Camera.main.ScreenToWorldPoint(direction);
        direction = direction - origin;
        FireBullet(direction.normalized, speed * 1.5f, damage, scaleMultiplier, bulletPreFab);
        if (scaleMultiplier > 1)
        {
            BulletAudioSource.PlayOneShot(PlayerBulletSoundBFG, 1f);
        }
        else
        {
            BulletAudioSource.PlayOneShot(PlayerBulletSoundNormal, 0.8f);
        }
    }

    public static void FireTurretBullet(Vector2 spawn, Vector2 direction, Quaternion rotation, 
        float speed = 25, int damage = 1, float scaleMultiplier = 1)
    {
        GameObject bulletPreFab = Instantiate(TurretBullet, spawn, rotation);
        FireBullet(direction, speed, damage, scaleMultiplier, bulletPreFab);
        BulletAudioSource.PlayOneShot(TurretBulletSound, Random.Range(.5f, 1f));
    }

    public static void FireTurretFreezeBullet(Vector2 spawn, Vector2 direction, Quaternion rotation,
        float speed = 25, int damage = 1, float scaleMultiplier = 1)
    {
        GameObject bulletPreFab = Instantiate(TurretFreezeBullet, spawn, rotation);
        BulletController controller = FireBullet(direction, speed, damage, scaleMultiplier, bulletPreFab);
        controller._hasFreeze = true;
        BulletAudioSource.PlayOneShot(TurretBulletSound, .4f);
    }

    private static BulletController FireBullet(Vector2 direction, float speed, int damage, float scaleMultiplier, GameObject bulletPreFab)
    {
        Rigidbody2D body = bulletPreFab.GetComponent<Rigidbody2D>();
        body.transform.localScale *= scaleMultiplier;
        body.transform.Rotate(body.transform.forward, -90f);
        body.AddForce(direction * speed * 10);

        BulletController controller = bulletPreFab.GetComponent<BulletController>();
        controller.Damage = damage;
        return controller;
    }

    private static Vector2 FireBulletOld(Vector2 origin, Vector2 direction, float speed, int damage, float scaleMultiplier, GameObject bulletPreFab)
    {
        BulletController controller = bulletPreFab.GetComponent<BulletController>();
        controller.Damage = damage;
        Rigidbody2D body = bulletPreFab.GetComponent<Rigidbody2D>();
        body.transform.localScale *= scaleMultiplier;
        direction = direction - origin;
        direction.Normalize();
        body.velocity = direction * speed;
        return direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Walls")
        {
            Destroy(this.gameObject);
        }
        if (collision.tag == "Enemy")
        {
            BugController bc = collision.GetComponent<BugController>();
            bc.TakeDamage(Damage, _hasFreeze);
            Destroy(this.gameObject);
        }
    }

}