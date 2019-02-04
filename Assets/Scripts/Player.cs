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
    private float borderValueX = 2.85f;
    private float borderValueY = 2.7f;
    private bool isAlive = false;
    private Manager gameManager;

    Spaceship.ColorType currentColor;


    void Start()
    {
        spaceship = this.GetComponent<Spaceship>();
        currentColor = Spaceship.ColorType.secondColor;

        spaceShipRenderer = gameObject.GetComponent<SpriteRenderer>();
        playerBullet = spaceship.bullet;
        this.GetComponent<Animator>().SetBool("isAlive", isAlive);
        Respawn();

        gameManager = GameObject.FindWithTag("GameManager").GetComponent<Manager>();

        currentColor = Spaceship.ColorType.firstColor;
        ColorSwitch();

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
        if (!Manager.keyboardMode)
        {
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

        spaceship.Move(direction);

        //Color Switch
        if (Input.GetKeyUp(KeyCode.Space))
        {
            currentColor = (currentColor == Spaceship.ColorType.firstColor) ? Spaceship.ColorType.secondColor : Spaceship.ColorType.firstColor;
            ColorSwitch();
        }

        //Suicide
        if (Input.GetKeyUp(KeyCode.RightControl))
        {
            Destruction();
        }

    }

    /// <summary>
    /// Switch the SpriteRenderer's color & ColorType of bullet
    /// </summary>
    private void ColorSwitch()
    {
        if (currentColor == Spaceship.ColorType.firstColor)
        {
            spaceShipRenderer.color = spaceship.firstColor;
            playerBullet.transform.GetChild(0).GetComponent<SpriteRenderer>().color = spaceship.firstColor;
            playerBullet.transform.GetChild(1).GetComponent<SpriteRenderer>().color = spaceship.firstColor;
        }
        else
        {
            spaceShipRenderer.color = spaceship.secondColor;
            playerBullet.transform.GetChild(0).GetComponent<SpriteRenderer>().color = spaceship.secondColor;
            playerBullet.transform.GetChild(1).GetComponent<SpriteRenderer>().color = spaceship.secondColor;
        }

        playerBullet.GetComponent<Bullet>().currentColor = currentColor;


    }

    /// <summary>
    /// Switch between the player's colors, and corrected his position
    /// </summary>
    private void ColorManagement()
    {
        if (ColorDetection.yellowTuple.Item1 < -10000)
        {
            currentColor = Spaceship.ColorType.secondColor;
            ColorSwitch();
        }
        else if (ColorDetection.blueTuple.Item1 < -10000)
        {
            currentColor = Spaceship.ColorType.firstColor;
            ColorSwitch();
        }

        //Debug.Log("yellow : " + x + " - " + y);
        //resolution de la webcam : 620 x 460
        //taille de la camera : 40 x 20
        //ce sont les calcul pour cadrer les vaisseau dans la camera
        float x, y, posX, posY;

        switch (currentColor)
        {
            case Spaceship.ColorType.firstColor:

                x = ColorDetection.yellowTuple.Item1;
                y = ColorDetection.yellowTuple.Item2;

                posX = ((x - 310f) / 310.0f) * 3.5f;
                posY = (((y - 230) / 230.0f) * 2.6f) * -1.0f;

                gameObject.transform.position = new Vector3(posX, posY);
                break;

            case Spaceship.ColorType.secondColor:
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
        if (transform.position.x > borderValueX)
        {
            transform.position = new Vector3(borderValueX, transform.position.y);
        }
        if (transform.position.x < -borderValueX)
        {
            transform.position = new Vector3(-borderValueX, transform.position.y);
        }
        if (transform.position.y > borderValueY)
        {
            transform.position = new Vector3(transform.position.x, borderValueY);
        }
        if (transform.position.y < -borderValueY)
        {
            transform.position = new Vector3(transform.position.x, -borderValueY);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Invoke Layer name
        string layerName = LayerMask.LayerToName(collision.gameObject.layer);


        if ((layerName == "Bullet (Enemy)" && (collision.gameObject.GetComponent<Bullet>().currentColor != currentColor))  || layerName == "Enemy")
        {
            if (!collision.CompareTag("Laser"))
            {
                Destroy(collision.gameObject);
            }
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
