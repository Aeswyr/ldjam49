using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unstable;

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
