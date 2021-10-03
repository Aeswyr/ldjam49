using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndLevelCanvas : MonoBehaviour
{
    [SerializeField] private Text pointsText;
    [SerializeField] private Transform starParent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDisplayScore(int points)
    {
        pointsText.text = ("Score: " + points);
    }

    public void DisplayStars(int numStars)
    {
        for (int i = 0; i < numStars; i++)
            starParent.GetChild(i).gameObject.SetActive(true);
    }

    public void ToNextLevel()
    {
        Debug.Log("Next Level Clicked");
    }
}
