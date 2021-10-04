using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unstable;

public class MainMenuController : MonoBehaviour
{
    private void Start()
    {
        AudioSrcMgr.instance.PlayAudio("music_theme");
    }

    public void StartButton()
    {
        AudioSrcMgr.instance.StopAudio();
        AudioSrcMgr.instance.PlayOneShot("click_basic");
        SceneManager.LoadScene("LevelSelect", LoadSceneMode.Single);
    }

    public void ExitButton()
    {
        AudioSrcMgr.instance.StopAudio();
        AudioSrcMgr.instance.PlayOneShot("click_basic");
        Application.Quit();
    }
}
