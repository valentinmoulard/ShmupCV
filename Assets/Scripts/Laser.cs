using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float lifeTime = 5;
    
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
