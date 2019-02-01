using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EnemyType { Basic, RotatorRight,RotatorLeft };

public class Enemy : MonoBehaviour
{
    Spaceship spaceship;
    public int scoreValue = 0;
    public EnemyType enemyType;
    public float movementDelay = 0f;
    private float timePassed = 0;
    public int speedRotate = 20;
    private GameObject camera;
    GameObject enemyBullet;
    SpriteRenderer spaceShipRenderer;

    public int speedRotate = 3;

    public Spaceship.ColorType currentColor;

    IEnumerator Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        spaceship = this.GetComponent<Spaceship>();

        switch (enemyType)
        {
            case EnemyType.RotatorLeft:
                movementDelay = 3f;
                break;
            case EnemyType.RotatorRight:
                movementDelay = 3f;
                break;
            case EnemyType.Basic:
                movementDelay = 0f;
                break;
        }

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

    void Update()
    {
        Vector3 stopPosition;
        switch (enemyType)
        {
            case EnemyType.RotatorRight :
               
                transform.Rotate(Vector3.forward * Time.deltaTime * speedRotate);
                // moving the object
                
                stopPosition = new Vector3(camera.transform.position.x / 2, transform.position.y, transform.position.z);
                Debug.Log(stopPosition);
                transform.position =  Vector3.MoveTowards(transform.position, stopPosition, Time.deltaTime);
                break;
            case EnemyType.RotatorLeft:

                transform.Rotate(Vector3.forward * Time.deltaTime * speedRotate);
                // moving the object
                float cameraWidth = camera.GetComponent<Camera>().orthographicSize * 2f * camera.GetComponent<Camera>().aspect;
                float stopPosX = camera.transform.position.x + (cameraWidth - camera.transform.position.x) / 2;
                stopPosition = new Vector3(stopPosX, transform.position.y, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, stopPosition, Time.deltaTime);
                break;
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
