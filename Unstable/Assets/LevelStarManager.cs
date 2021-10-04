using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelStarManager : MonoBehaviour
{
    [System.Serializable]
    public struct LevelStatus
    {
        public bool isUnlocked;
        public int starsEarned;
    }

    [SerializeField] private LevelStatus[] levelStars;

    private void Start()
    {
        if (GameObject.FindGameObjectsWithTag("StarManager").Length > 1)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public LevelStatus GetStatusOfLevel(int levelNum)
    {
        return levelStars[levelNum - 1];
    }

    public void SetStars(int levelNum, int newStars)
    {
        levelStars[levelNum - 1].starsEarned = Mathf.Max(newStars, levelStars[levelNum -1].starsEarned);

        if ((levelNum < levelStars.Length) && (newStars > 0) && (levelNum < (SceneManager.sceneCountInBuildSettings -2))) //Unlock next level
            levelStars[levelNum].isUnlocked = true;
    }
}
