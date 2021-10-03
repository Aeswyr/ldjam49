using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Header("Level Settings + Objects")]
    [SerializeField] private int numberOfTotalCustomers;
    [SerializeField] private List<FoodObject> foodObjects;
    [SerializeField] private bool spawnNewCustomerAfterEating;

    [Header("Object References + Prefabs")]
    [SerializeField] private Booth[] booths;
    [SerializeField] private Transform customerSpawnLocation;
    [SerializeField] private Transform customerParent;

    [Header("Other References (Do not need to change)")]
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private EndLevelCanvas endLevelCanvas;

    private int customersSpawned;
    private int customersFed;
    private int score;

    // Start is called before the first frame update
    void Start()
    {
        customersSpawned = 0;
        customersFed = 0;
        score = 0;

        List<FoodObject> foodObjectCopy = new List<FoodObject>(foodObjects);

        foreach(Booth booth in booths)
        {
            int randFoodIndex = Random.Range(0, foodObjectCopy.Count);
            booth.SetFood(foodObjectCopy[randFoodIndex]);
            booth.levelController = this;

            foodObjectCopy.RemoveAt(randFoodIndex);
        }

        SpawnCustomer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnCustomer()
    {
        if (customersSpawned >= numberOfTotalCustomers)
            return;

        GameObject newCustomer = Instantiate(customerPrefab, customerParent);
        newCustomer.transform.position = customerSpawnLocation.position;

        FoodObject foodToEat = foodObjects[Random.Range(0,foodObjects.Count)];
        float eatTime = Random.Range(2f, 5f);

        newCustomer.GetComponent<Customer>().SetValues(foodToEat, eatTime);

        customersSpawned++;
    }

    public void CustomerFed(int pointsToAdd)
    {
        customersFed++;
        score += pointsToAdd;

        if (customersFed >= numberOfTotalCustomers)
            LevelEnd();

        if (spawnNewCustomerAfterEating)
            SpawnCustomer();
    }

    public void LevelEnd()
    {
        endLevelCanvas.gameObject.SetActive(true);
        endLevelCanvas.SetDisplayScore(score);
    }
}
