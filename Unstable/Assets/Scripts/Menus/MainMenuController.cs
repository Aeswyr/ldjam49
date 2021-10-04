using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartButton()
    {
        SceneManager.LoadScene("LevelSelect", LoadSceneMode.Single);
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
