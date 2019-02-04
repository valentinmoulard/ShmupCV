using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailSystem : MonoBehaviour
{
    public GameObject trailPrefab;

    // Update is called once per frame
    void Update()
    {
        if (Random.Range(0,100) < 30)
        {
            float randomSpawn = Random.Range(-3.0f, 3.0f);

            Instantiate(trailPrefab, new Vector3(randomSpawn, 3, -1), Quaternion.identity);
        }
    }
}
