using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public enum customerStatus
    { 
        hungry,
        eating,
        doneEating
    }


    public FoodObject desiredFood;
    [SerializeField] private GameObject thoughtBubble;
    [SerializeField] private SpriteRenderer foodIcon;
    [SerializeField] private CircleFill eatTimer;
    public customerStatus status;

    // Start is called before the first frame update
    void Start()
    {
        status = customerStatus.hungry;
        foodIcon.sprite = desiredFood.sprite;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartEating()
    {
        status = customerStatus.eating;
        thoughtBubble.SetActive(false);
    }

    public void DoneEating()
    {
        status = customerStatus.doneEating;
        eatTimer.gameObject.SetActive(false);
    }

    public void UpdateEatTimer(float fraction)
    {
        eatTimer.UpdateFill(fraction);
    }
}
