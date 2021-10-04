using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unstable;

public class Customer : MonoBehaviour
{
    public enum customerStatus
    {
        hungry,
        eating,
        doneEating
    }

    private Slidable slidable;

    public FoodObject desiredFood;
    [SerializeField] private GameObject thoughtBubble;
    [SerializeField] private SpriteRenderer foodIcon;
    [SerializeField] private CircleFill eatTimer;
    [SerializeField] private float eatRateInSecs;
    public customerStatus status;
    private float pointsWorth;

    // Start is called before the first frame update
    void Start()
    {
        status = customerStatus.hungry;
        if (foodIcon != null)
            foodIcon.sprite = desiredFood.sprite;

        slidable = GetComponent<Slidable>();

        pointsWorth = 100;
    }


    public void SetValues(FoodObject newFood, float newEatTime)
    {
        desiredFood = newFood;
        eatRateInSecs = newEatTime;

        foodIcon.sprite = desiredFood.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (status == customerStatus.hungry)
            pointsWorth -= Mathf.Max(0,Time.deltaTime * 10);
    }

    public void StartEating()
    {
        slidable.SetLocked(true);
        status = customerStatus.eating;
        thoughtBubble.SetActive(false);
    }

    public void DoneEating()
    {
        slidable.SetLocked(false);
        status = customerStatus.doneEating;
        eatTimer.gameObject.SetActive(false);
    }

    public int PointsWorth()
    {
        return Mathf.FloorToInt(pointsWorth);
    }

    public float EatRateInSecs()
    {
        return eatRateInSecs;
    }

    public void UpdateEatTimer(float fraction)
    {
        eatTimer.UpdateFill(fraction);
    }
}
