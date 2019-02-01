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

        if (layerName != "Bullet (Player)") return;
        Manager.score += scoreValue;
        scoreValue = 0;
        //Debug.Log(Manager.score);
        //Destroy(collision.gameObject);
        spaceship.Explosion();

        Destroy(gameObject);
    }

}
