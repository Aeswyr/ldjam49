using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeatLocation : MonoBehaviour
{
    [System.NonSerialized] public Booth parentBooth;

    [SerializeField] private Transform customerLocation;

    [SerializeField] private Customer currCustomer;
    private bool isOccupied;
    private float maxEatTime;
    private float eatTimer;

    private void Update()
    {
        if (isOccupied)
        {
            eatTimer -= Time.deltaTime;

            currCustomer.UpdateEatTimer(1 - (eatTimer / maxEatTime));

            if (eatTimer <= 0) //Customer finished eating
            {
                eatTimer = 0;
                isOccupied = false;
                
                parentBooth.FreeCustomer(currCustomer);
                currCustomer = null;
            }
        }
    }

    public void SeatCustomer(Customer newCustomer)
    {
        if (isOccupied)
        {
            Debug.Log("ERROR: SEATING CUSTOMER AT ALREADY OCCUPIED SEAT");
            return;
        }

        currCustomer = newCustomer;

        eatTimer = newCustomer.EatRateInSecs();
        maxEatTime = eatTimer;

        isOccupied = true;

        currCustomer.transform.position = customerLocation.position;
    }

    public bool IsOccupied()
    {
        return isOccupied;
    }
}
