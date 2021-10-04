using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unstable;

public class MainMenuController : MonoBehaviour
{
    public void StartButton()
    {
        AudioSrcMgr.instance.PlayOneShot("click_basic");
        SceneManager.LoadScene("LevelSelect", LoadSceneMode.Single);
    }

    public void ExitButton()
    {
        AudioSrcMgr.instance.PlayOneShot("click_basic");
        Application.Quit();
    }
}
