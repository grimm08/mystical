using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private string levelName = "intro";

    public GameObject credits;


    public void OnPressedNewGameButton()
    {
        SceneManager.LoadScene(levelName);
    }

    public void OnPressedQuitButton()
    {
        Application.Quit();
    }

    public void Credits()
    {
        credits.SetActive(!credits.activeSelf);   
    }

}
