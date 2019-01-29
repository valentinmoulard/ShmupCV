using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{

    public Text titleText;
    public Text resumeText;
    public Text quitText;
    private Manager manager;
    //public GameObject pauseObject;
    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<Manager>().GetComponent<Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        CoolFlash(titleText);
        
    }

    void CoolFlash(Text text)
    {
        text.color = new Color(Random.value, Random.value, Random.value);
    }

    public void ResumeGame()
    {
        manager.gameStopped = false;
    }
    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }

    public void HoverStart()
    {
        
        resumeText.color = new Color(0f, 1f, 0f);
        resumeText.fontSize = 70;
    }

    public void UnHoverStart()
    {
        
        resumeText.color = new Color(1f, 1f, 1f);
        resumeText.fontSize = 50;
    }

    public void HoverQuit()
    {
        
        quitText.color = new Color(1f, 0f, 0f);
        quitText.fontSize = 70;
    }

    public void UnHoverQuit()
    {
        
        quitText.color = new Color(1f, 1f, 1f);
        quitText.fontSize = 50;
    }
}
