using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{

    Spaceship spaceship;
    public int speedRotate = 6;
    public float speed = 5;
    public int scoreValue = 1000;


    public GameObject bullet;
    public GameObject bullet2;
    public GameObject laser;
    public GameObject explosion;
    public GameObject shield;

    public Vector3 GoToPosition;

    float shootCooldown = 2;
    float shootTimer = 0;
    float laserCooldown = 10;
    float laserTimer = 0;
    float bulletRainCooldown = 3;
    float bulletRainTimer = 0;

    float invincibleTimer = 0;
    float invincibleTime = 5;

    int health = 5;

    public List<GameObject> canonPositions = new List<GameObject>();


    public enum BossPhase {phase0, phase1, phase2, phase3};
    public BossPhase phase;

    public Sprite spritePhase1;
    public Sprite spritePhase2;
    public Sprite spritePhase3;

    Sprite currentSprite;

    float bulletNumber = 8;
    
    void Start()
    {
        invincibleTimer = 5;
        phase = BossPhase.phase0;
        
    }
    
    void Update()
    {

        switch (phase)
        {
            case BossPhase.phase0:
                BossRotate(speedRotate * 7);
                transform.position = Vector2.MoveTowards(transform.position, GoToPosition, speed * Time.deltaTime);
                if (Vector3.Distance(GoToPosition,transform.position) < 0.1f)
                {
                    phase = BossPhase.phase1;
                }
                break;

            case BossPhase.phase1:
                GetComponent<SpriteRenderer>().sprite = spritePhase1;

                BossRotate(speedRotate * 3);

                ClassicShoot(shootCooldown);

                shootTimer += Time.deltaTime;
                break;

            case BossPhase.phase2:
                GetComponent<SpriteRenderer>().sprite = spritePhase2;

                BossRotate(speedRotate * 6);

                ClassicShoot(shootCooldown - 1f);
                LaserShoot(laserCooldown);

                shootTimer += Time.deltaTime;
                laserTimer += Time.deltaTime;
                break;

            case BossPhase.phase3:
                GetComponent<SpriteRenderer>().sprite = spritePhase3;

                BossRotate(speedRotate * 8);

                ClassicShoot(shootCooldown - 1.5f);
                LaserShoot(laserCooldown - 2f);
                BulletRain(bulletNumber, bulletRainCooldown);

                shootTimer += Time.deltaTime;
                laserTimer += Time.deltaTime;
                bulletRainTimer += Time.deltaTime;
                break;
            default:

                break;
        }

        invincibleTimer += Time.deltaTime;

        if (health < 4 && health >=2)
        {
            phase = BossPhase.phase2;
        }
        else if (health < 2)
        {
            phase = BossPhase.phase3;
        }

    }


    void BossRotate(float speedRotate)
    {
        transform.Rotate(Vector3.forward * speedRotate * Time.deltaTime);
    }

    void ClassicShoot(float shootCooldown)
    {
        if (shootTimer > shootCooldown)
        {
            for (int i = 0; i < canonPositions.Count; i++)
            {
                Transform shotPosition = canonPositions[i].transform;
                Shoot(shotPosition);
            }
            shootTimer = 0;
        }
    }


    void LaserShoot(float laserCooldown)
    {
        if (laserTimer > laserCooldown)
        {
            for (int i = 0; i < canonPositions.Count; i++)
            {
                Transform shotPosition = canonPositions[i].transform;
                Laser(shotPosition);
            }
            laserTimer = 0;
        }

    }
    
    public void BulletRain(float bulletNumber, float bulletRainCooldown)
    {
        if (bulletRainTimer > bulletRainCooldown)
        {
            float anglePart = 360/bulletNumber;
            
            for (int i = 0; i < bulletNumber; i++)
            {
                Instantiate(bullet2, transform.position, Quaternion.Euler(0.0f, 0.0f, anglePart * i));
            }
            bulletRainTimer = 0;
        }
        
    }

    public void Explosion()
    {
        Instantiate(explosion, transform.position, transform.rotation);
    }

    public void Shoot(Transform origin)
    {
        Instantiate(bullet, origin.position, origin.rotation);
    }
    

    public void Laser(Transform origin)
    {
        Instantiate(laser, origin.position, origin.rotation, origin.transform);
    }

    public void Shield()
    {
        Instantiate(shield, transform.position, transform.rotation);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Invoke Layer name
        string layerName = LayerMask.LayerToName(collision.gameObject.layer);

        if (layerName != "Bullet (Player)" || phase == BossPhase.phase0) return;
        Destroy(collision.gameObject);

        if (invincibleTimer > invincibleTime)
        {
            if (health > 0)
            {
                health--;
                invincibleTimer = 0;
                Shield();
                return;
            }

            Manager.score += scoreValue;

            Explosion();

            Destroy(gameObject);
        }


    }


}
