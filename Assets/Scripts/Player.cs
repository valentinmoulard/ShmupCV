using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Spaceship spaceship;
    SpriteRenderer spaceShipRenderer;
    GameObject playerBullet;
    // move speed
    public float speed = 5;

    private bool isAlive = true;
    private Manager gameManager;
    enum Color { Yellow, Blue };
    Color currentColor;






    void Start()
    {
        currentColor = Color.Blue;
        spaceship = this.GetComponent<Spaceship>();

        spaceShipRenderer = gameObject.GetComponent<SpriteRenderer>();
        playerBullet = spaceship.bullet;
        this.GetComponent<Animator>().SetBool("isAlive", isAlive);

        gameManager = GameObject.FindWithTag("GameManager").GetComponent<Manager>();


        StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        while (isAlive)
        {
            spaceship.Shot(transform);

            yield return new WaitForSeconds(spaceship.shotDelay);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.keyboardMode)
        {
            Debug.Log("Keyboard");
            ColorManagement();
        }
        else
        {
            InputManager();
        }
        BorderManagement();


    }

    /// <summary>
    /// Use for the keyboardMode
    /// </summary>
    private void InputManager()
    {
        // left, right
        float x = Input.GetAxisRaw("Horizontal");

        // up, down
        float y = Input.GetAxisRaw("Vertical");

        // move direction
        Vector2 direction = new Vector2(x, y).normalized;

        // move direction and velocity
        GetComponent<Rigidbody2D>().velocity = direction * speed;

        //Color Switch
        if (Input.GetKeyUp(KeyCode.Space))
        {
            currentColor = (currentColor == Color.Yellow) ? Color.Blue : Color.Yellow;
            ColorSwitch();
        }

        //Suicide
        if (Input.GetKeyUp(KeyCode.D))
        {
            Destruction();
        }

    }

    /// <summary>
    /// Switch the SpriteRenderer's color
    /// </summary>
    private void ColorSwitch()
    {
        if (currentColor == Color.Yellow)
        {
            spaceShipRenderer.color = gameManager.yellow;
            playerBullet.transform.GetChild(0).GetComponent<SpriteRenderer>().color = gameManager.yellow;
            playerBullet.transform.GetChild(1).GetComponent<SpriteRenderer>().color = gameManager.yellow;

        }
        else
        {
            spaceShipRenderer.color = gameManager.blue;
            playerBullet.transform.GetChild(0).GetComponent<SpriteRenderer>().color = gameManager.blue;
            playerBullet.transform.GetChild(1).GetComponent<SpriteRenderer>().color = gameManager.blue;
        }

    }

    /// <summary>
    /// Switch between the player's colors, and corrected his position
    /// </summary>
    private void ColorManagement()
    {
        if (ColorDetection.yellowTuple.Item1 < -10000)
        {
            currentColor = Color.Blue;
            ColorSwitch();
        }
        else if (ColorDetection.blueTuple.Item1 < -10000)
        {
            currentColor = Color.Yellow;
            ColorSwitch();
        }

        //Debug.Log("yellow : " + x + " - " + y);
        //resolution de la webcam : 620 x 460
        //taille de la camera : 40 x 20
        //ce sont les calcul pour cadrer les vaisseau dans la camera
        float x, y, posX, posY;

        switch (currentColor)
        {
            case Color.Yellow:

                x = ColorDetection.yellowTuple.Item1;
                y = ColorDetection.yellowTuple.Item2;

                posX = ((x - 310f) / 310.0f) * 3.5f;
                posY = (((y - 230) / 230.0f) * 2.6f) * -1.0f;

                gameObject.transform.position = new Vector3(posX, posY);
                break;

            case Color.Blue:
                x = ColorDetection.blueTuple.Item1;
                y = ColorDetection.blueTuple.Item2;

                posX = ((x - 310f) / 310.0f) * 3.5f;
                posY = (((y - 230) / 230.0f) * 2.6f) * -1.0f;

                gameObject.transform.position = new Vector3(posX, posY);
                break;
            default:
                break;
        }

    }


    /// <summary>
    /// Check the current position of the player and make borders
    /// </summary>
    private void BorderManagement()
    {
        if (transform.position.x > 3.5f)
        {
            transform.position = new Vector3(3.5f, transform.position.y);
        }
        if (transform.position.x < -3.5f)
        {
            transform.position = new Vector3(-3.5f, transform.position.y);
        }
        if (transform.position.y > 2.6f)
        {
            transform.position = new Vector3(transform.position.x, 2.6f);
        }
        if (transform.position.y < -2.6f)
        {
            transform.position = new Vector3(transform.position.x, -2.6f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Invoke Layer name
        string layerName = LayerMask.LayerToName(collision.gameObject.layer);


        if (layerName == "Bullet (Enemy)" || layerName == "Enemy")
        {

            Destroy(collision.gameObject);
            Destruction();
        }

    }


    private void Destruction()
    {
        //Explosion
        spaceship.Explosion();
        if (isAlive)
        {
            Manager.lifeCounter -= 1;
            isAlive = false;
            this.GetComponent<Animator>().SetBool("isAlive", isAlive);
            spaceShipRenderer.enabled = false;
            Invoke("Respawn", 2);
        }
    }


    private void Respawn()
    {
        gameObject.transform.position = new Vector3(0, -1.25f);
        spaceShipRenderer.enabled = true;
        Invoke("Invincible", 2);
    }


    private void Invincible()
    {
        isAlive = true;
        this.GetComponent<Animator>().SetBool("isAlive", isAlive);
        StartCoroutine("Shoot");
    }



}
