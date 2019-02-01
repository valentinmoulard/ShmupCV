using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public int speed = 10;
    public float lifeTime = 5;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Rigidbody2D>().velocity = transform.up * speed;
        Destroy(gameObject, lifeTime);
    }
}
