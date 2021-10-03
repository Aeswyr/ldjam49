using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booth : MonoBehaviour
{
    [SerializeField] private FoodObject currentFood;
    [SerializeField] private SpriteRenderer foodDisplay;

    [SerializeField] private SeatLocation[] seatLocations;

    [SerializeField] private Transform exitLocation;

    [System.NonSerialized] public LevelController levelController;

    // Start is called before the first frame update
    void Start()
    {
        if (currentFood != null)
            foodDisplay.sprite = currentFood.sprite;

        for (int i = 0; i<seatLocations.Length;i++)
        {
            seatLocations[i].parentBooth = this;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetFood(FoodObject newFood)
    {
        currentFood = newFood;
        foodDisplay.sprite = currentFood.sprite;
    }

    public void OnValidate()
    {
        if (currentFood != null)
            foodDisplay.sprite = currentFood.sprite;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Customer")
        {
            Customer currCustomer = other.GetComponent<Customer>();
            if (currCustomer.status != Customer.customerStatus.hungry)
                return;

            if(currCustomer.desiredFood == currentFood)
            {
                //Check for any open spots
                foreach(SeatLocation currSeat in seatLocations)
                {
                    if (currSeat.IsOccupied() == false)
                    {
                        //Put customer at table
                        currCustomer.StartEating();
                        currSeat.SeatCustomer(currCustomer);
                        break;
                    }
                }
            }
            else
            {
                //Wrong booth for this customer
            }
        }
    }

    public void FreeCustomer(Customer customer)
    {
        customer.transform.position = exitLocation.position;
        customer.DoneEating();
        levelController.CustomerFed(customer.PointsWorth());
    }
}
