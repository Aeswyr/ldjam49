using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndLevelCanvas : MonoBehaviour
{
    [SerializeField] private Text pointsText;
    [SerializeField] private Transform starParent;

    [SerializeField] private GameObject levelWon;
    [SerializeField] private GameObject levelFailed;

    private int levelNum;
    public LevelStarManager starManager;

    // Start is called before the first frame update
    void Awake()
    {
        levelNum = SceneManager.GetActiveScene().buildIndex - 1;

        starManager = GameObject.FindGameObjectsWithTag("StarManager")[0].GetComponent<LevelStarManager>();
    }

    public void SetDisplayScore(int points)
    {
        pointsText.text = ("Score: " + points);
    }

    public void DisplayStars(int numStars)
    {
        for (int i = 0; i < numStars; i++)
            starParent.GetChild(i).gameObject.SetActive(true);

        levelWon.SetActive(numStars > 0);
        levelFailed.SetActive(numStars == 0);

        starManager.SetStars(levelNum, numStars); //Store number stars earned and unlocks next level
    }

    public void BackToLevelSelect()
    {
        SceneManager.LoadScene(1);
    }

    public void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToNextLevel()
    {
        int nextLevelIndex = levelNum + 2;
        if (nextLevelIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextLevelIndex);
        else
            SceneManager.LoadScene(1); //Failsafe, go to level select
    }
}
