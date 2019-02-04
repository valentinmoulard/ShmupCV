using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float lifeTime = 5;
    
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
