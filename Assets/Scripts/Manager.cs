using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{

    public static int score = 0;
    public static int lifeCounter;
    // Singleton Game Object
    public static Manager instance = null;


    [Header("Scripts")]
    public GameObject Player;

    [Header("Game Mode")]
    public bool keyboardMode = false;

    [Header("Color Attributes")]
    public Color blue;
    public Color yellow;


    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }


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
