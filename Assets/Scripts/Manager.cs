using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{

    public static int score = 0;
    public static int lifeCounter;

    public GameObject Player;

    void Start()
    {
        lifeCounter = 3;
    }

    private void Update()
    {
        if (lifeCounter < 0)
        {
            Destroy(Player);
        }
    }




}
