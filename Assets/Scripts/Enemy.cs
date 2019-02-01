using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Spaceship spaceship;
    public int scoreValue = 0;
    GameObject enemyBullet;
    SpriteRenderer spaceShipRenderer;

    public int speedRotate = 3;

    public Spaceship.ColorType currentColor;


    IEnumerator Start()
    {
        spaceship = this.GetComponent<Spaceship>();
        enemyBullet = spaceship.bullet;
        //enemyBullet = GameObject.Instantiate(spaceship.bullet);
        spaceShipRenderer = gameObject.GetComponent<SpriteRenderer>();

        ColorSwitch();

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

        if (layerName != "Bullet (Player)")
        {
            return;
        }
        //If opposite color between the enemy and the bullet, destroy the enemy
        else if (collision.gameObject.GetComponentInParent<Bullet>().currentColor != currentColor)
        {
            Manager.score += scoreValue;
            scoreValue = 0;
            //Debug.Log(Manager.score);
            Destroy(collision.gameObject);
            spaceship.Explosion();

            Destroy(gameObject);
        }

    }

    /// <summary>
    /// Switch the SpriteRenderer's color & ColorType of bullet
    /// </summary>
    private void ColorSwitch()
    {
        //Debug.Log("Switch color");
        if (currentColor == Spaceship.ColorType.firstColor)
        {
            spaceShipRenderer.color = spaceship.firstColor;
            enemyBullet.GetComponent<SpriteRenderer>().color = spaceship.firstColor;
        }
        else
        {
            spaceShipRenderer.color = spaceship.secondColor;
            enemyBullet.GetComponent<SpriteRenderer>().color = spaceship.secondColor;

        }

        enemyBullet.GetComponent<Bullet>().currentColor = currentColor;

    }

}
