using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndLevelCanvas : MonoBehaviour
{
    [SerializeField] private Text pointsText;
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

    public void ToNextLevel()
    {
        Debug.Log("Next Level Clicked");
    }
}
