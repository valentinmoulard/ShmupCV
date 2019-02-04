using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    public float lifeTime = 2;
    public float speed = 5;
    void Start()
    {
        float trailLength = Random.Range(0.005f, 0.02f);

        gameObject.GetComponent<TrailRenderer>().time = trailLength;
        float randomSpeed = Random.Range(3f, 5f);
        this.GetComponent<Rigidbody2D>().velocity = -transform.up * randomSpeed;
        Destroy(gameObject, lifeTime);
    }
}
