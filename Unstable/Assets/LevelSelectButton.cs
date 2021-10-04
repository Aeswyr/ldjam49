using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] private int levelNumber;
    private bool isLocked;
    private int numStars;

    [SerializeField] private Transform starParent;
    [SerializeField] private GameObject lockOverlay;

    // Start is called before the first frame update
    void Start()
    {
        LevelStarManager manager = GameObject.FindWithTag("StarManager").GetComponent<LevelStarManager>();

        LevelStarManager.LevelStatus status = manager.GetStatusOfLevel(levelNumber);
        isLocked = !status.isUnlocked;
        numStars = status.starsEarned;

        UpdateVisuals();
    }

    // Update is called once per frame
    public void Clicked()
    {
        if ((!isLocked) && (levelNumber + 1 < SceneManager.sceneCountInBuildSettings))
        {
            SceneManager.LoadScene(levelNumber + 1);
        }
        else
        {
            //Maybe play lock wiggle animation?
        }
    }

    public void UpdateVisuals()
    {
        lockOverlay.SetActive(isLocked);
        ShowStars();
    }

    public void ShowStars()
    {
        for (int i = 0; i < numStars; i++)
            starParent.GetChild(i).gameObject.SetActive(true);
    }
}
