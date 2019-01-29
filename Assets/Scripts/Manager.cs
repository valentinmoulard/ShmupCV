using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{

    public static int score = 0;
    public static int lifeCounter;

    [Header("Scripts")]
    public GameObject Player;

    [Header("Game Mode")]
    public bool keyboardMode = false;

    [Header("Color Attributes")]
    public Color blue;
    public Color yellow;

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
