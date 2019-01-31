using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Spaceship spaceship;
    public int scoreValue = 0;

    IEnumerator Start()
    {
        spaceship = this.GetComponent<Spaceship>();

        spaceship.Move(transform.up * -1);

        if (spaceship.canShot == false)
        {
            yield break;
        }

        while (true)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform shotPosition = transform.GetChild(i);
                spaceship.Shot(shotPosition);
            }

            yield return new WaitForSeconds(spaceship.shotDelay);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Invoke Layer name
        string layerName = LayerMask.LayerToName(collision.gameObject.layer);

        if (layerName != "Bullet (Player)") return;

        Manager.score += scoreValue;
        scoreValue = 0;
        //Debug.Log(Manager.score);
        //Destroy(collision.gameObject);
        spaceship.Explosion();

        Destroy(gameObject);
    }

}
