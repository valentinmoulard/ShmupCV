using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{

    public static int score = 0;
    public static int lifeCounter;
    // Singleton Game Object
    public static Manager instance = null;
    public Image[] lives;
    private Text scoreText;

    public bool gameStopped = false;
    public GameObject pauseObject;
    public GameObject pauseManager;
    public GameObject gameOver;


    [Header("Scripts")]
    public GameObject Player;

    [Header("Game Mode")]
    public static bool keyboardMode = false;




    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this) { 
           /* if(SceneManager.GetActiveScene().buildIndex == 0)
            {
                Debug.Log("reinit");
                Reinit();
            }
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            else*/
                Destroy(gameObject);
        }
        //Sets this to not be destroyed when reloading scene
        //DontDestroyOnLoad(gameObject);
    }


        void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            return;
        lifeCounter = 3;
        scoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<Text>();
        scoreText.text = score.ToString();

        //Debug.Log(SceneManager.GetActiveScene().buildIndex.ToString()) ;

    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) {
            Reinit();
            return;
        }
        if (lifeCounter == 2)
        {
            lives[2].gameObject.SetActive(false);
        }
        else if (lifeCounter == 1)
        {
            lives[1].gameObject.SetActive(false);

        }
        else if (lifeCounter == 0)
        {
            lives[0].gameObject.SetActive(false);

        }
        if (lifeCounter < 0)
        {
            Destroy(Player);
            gameOver.SetActive(true);
            gameOver.transform.GetChild(2).GetComponent<Text>().text = "Score : "+score;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameStopped = !gameStopped;
        }

        if (gameStopped)
        {
            Time.timeScale = 0f;
            pauseObject.SetActive(true);
            pauseManager.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            pauseObject.SetActive(false);
            pauseManager.SetActive(false);
        }

        //Score

        scoreText.text = score.ToString();

        if (Input.GetKey(KeyCode.KeypadEnter))
        {
            Debug.Log("test");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            //NextLevel();
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        }


    }

    void Reinit()
    {
        Manager.score = 0;
        lifeCounter = 3;
        gameStopped = false;
    }




}
