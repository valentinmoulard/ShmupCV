using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

    private bool sound = true;
    private bool show = true;
    private bool locked = false;
    public Text soundText;
    public Text titleText;
    public Text startText;
    public Text quitText;
    public Text[] textClignotant;
    public Button modeSprite;
    public Manager gameManager;
    public Sprite keyboard;
    public Sprite cam;

    private bool clignote = true;
    // Use this for initialization
    void Start () {
        textClignotant = new Text[] { startText, quitText };
        if (Manager.keyboardMode)
        {
            modeSprite.image.sprite = keyboard;
        }
        else
        {
            modeSprite.image.sprite = cam;
        }
    }
	
	// Update is called once per frame
	void Update () {
        CoolFlash(titleText);
        if (!locked) { 

            StartCoroutine(Clignoter(textClignotant));

        }
    }

    void CoolFlash(Text text)
    {
        text.color = new Color(Random.value, Random.value, Random.value);
    }

    IEnumerator Clignoter(Text[] texts)
    {
        locked = true;

        yield return new WaitForSeconds(0.3f);
        if (show)
        {
            for(int i = 0; i < texts.Length; i++) {
                
                texts[i].color = new Color(texts[i].color.r, texts[i].color.g, texts[i].color.b, 1f);
            }
        }
        else if(clignote)
        {
            for (int i = 0; i < texts.Length; i++)
            {
                
                texts[i].color = new Color(texts[i].color.r, texts[i].color.g, texts[i].color.b, 0f);
            }
            
        }
        locked = false;
        show = !show;
    }
    public void StartGame()
    {
        //gameManager.gameStopped = false;
        SceneManager.LoadScene(1);
    }

    public void HoverStart()
    {
        clignote = false;
        startText.color = new Color(0f, 1f, 0f);
        startText.fontSize = 120; 
    }

    public void UnHoverStart()
    {
        clignote = true;
        startText.color = new Color(1f, 1f, 1f);
        startText.fontSize = 100;
    }

    public void HoverQuit()
    {
        clignote = false;
        quitText.color = new Color(1f, 0f, 0f);
        quitText.fontSize = 120;
    }

    public void UnHoverQuit()
    {
        clignote = true;
        quitText.color = new Color(1f, 1f, 1f);
        quitText.fontSize = 100;
    }

    public void hoverMode()
    {
        modeSprite.image.color = new Color(1f, 0.1084906f, 0.1084906f, 0.7686275f);
    }

    public void UnhoverMode()
    {
        modeSprite.image.color = new Color(1f, 1f, 1f, 0.7686275f);
    }

    public void changeMode()
    {
        Manager.keyboardMode = !Manager.keyboardMode;
        Debug.Log(Manager.keyboardMode);
        if (Manager.keyboardMode)
        {
            modeSprite.image.sprite = keyboard;
        }
        else
        {
            modeSprite.image.sprite = cam;
        }
    }

    public void ExitGame()
    {
        Debug.Log("Clicked");
        Application.Quit();
    }
    public void SwitchSound()
    {
        sound = !sound;
        if (sound)
        {
            soundText.text = "Sound : ON";
        }
        else
        {
            soundText.text = "Sound : OFF";
        }
    }
}
