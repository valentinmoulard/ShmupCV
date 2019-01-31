using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{

    Spaceship spaceship;
    public int speedRotate = 3;
    public int scoreValue = 1000;
    float shootCooldown = 2;
    float timer = 0;
    bool shootAvailable = false;

    int health = 5;

    // Start is called before the first frame update
    void Start()
    {
        spaceship = this.GetComponent<Spaceship>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * speedRotate * Time.deltaTime);

        if (shootAvailable)
        {
            ClassicShoot();
            shootAvailable = false;
        }

        timer += Time.deltaTime;
        if (timer > shootCooldown)
        {
            timer = 0;
            shootAvailable = true;
        }
        
    }


    void ClassicShoot()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform shotPosition = transform.GetChild(i);
            spaceship.Shot(shotPosition);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Invoke Layer name
        string layerName = LayerMask.LayerToName(collision.gameObject.layer);

        if (layerName != "Bullet (Player)") return;

        Manager.score += scoreValue;
        spaceship.Explosion();

        Destroy(gameObject);
    }


}
