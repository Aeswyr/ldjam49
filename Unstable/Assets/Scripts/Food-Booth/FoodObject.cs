using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FoodObject", menuName = "ScriptableObjects/FoodObject")]
public class FoodObject : ScriptableObject
{
    public string foodName;
    public Sprite sprite;

    public void OnValidate()
    {
        if (foodName.Length == 0)
        {
            foodName = name;
        }
    }
}
