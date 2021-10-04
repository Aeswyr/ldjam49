using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unstable;

public class LevelController : MonoBehaviour
{
    [System.Serializable]
    public struct customerStruct
    {
        public float spawnTime;

        public FoodObject desiredFood;
        public float timeTakenToEat;
    }

    [Header("Level Settings + Objects")]
    [SerializeField] private List<customerStruct> customersToSpawn;
    [SerializeField] private float[] starThresholds = new float[5];
    //[SerializeField] private List<FoodObject> foodObjects;
    //[SerializeField] private bool spawnNewCustomerAfterEating;

    [Header("Object References + Prefabs")]
    [SerializeField] private Booth[] booths;
    [SerializeField] private Transform customerSpawnLocation;
    [SerializeField] private Transform customerParent;

    [Header("Other References (Do not need to change)")]
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private EndLevelCanvas endLevelCanvas;

    private int numberOfTotalCustomers;
    private int numCustomersSpawned;
    private int numCustomersFed;
    private int score;
    private float timeSinceGameStarted;

    // Start is called before the first frame update
    void Start()
    {
        numberOfTotalCustomers = customersToSpawn.Count;
        numCustomersSpawned = 0;
        numCustomersFed = 0;
        score = 0;

        // List<FoodObject> foodObjectCopy = new List<FoodObject>(foodObjects);

        foreach(Booth booth in booths)
        {
            //int randFoodIndex = Random.Range(0, foodObjectCopy.Count);
            //booth.SetFood(foodObjectCopy[randFoodIndex]);
            booth.levelController = this;

            //foodObjectCopy.RemoveAt(randFoodIndex);
        }

        AudioSrcMgr.instance.PlayAudio("music_level", true);

        //SpawnCustomer(); 
    }

    private void OnDestroy()
    {
        AudioSrcMgr.instance.StopAudio();
    }

    public void OnValidate()
    {
        customersToSpawn.Sort((a, b) => a.spawnTime.CompareTo(b.spawnTime));
    }

    // Update is called once per frame
    void Update()
    {
        if (numCustomersSpawned >= numberOfTotalCustomers)
            return;

        timeSinceGameStarted += Time.deltaTime;
        if (timeSinceGameStarted >= customersToSpawn[numCustomersSpawned].spawnTime)
            SpawnCustomer();
    }

    public void SpawnCustomer()
    {
        if (numCustomersSpawned >= numberOfTotalCustomers)
            return;

        GameObject newCustomer = Instantiate(customerPrefab, customerParent);
        newCustomer.transform.position = customerSpawnLocation.position;
        AudioSrcMgr.instance.PlayOneShot("customer_spawn");

        //FoodObject foodToEat = foodObjects[Random.Range(0,foodObjects.Count)];
        //float eatTime = Random.Range(2f, 5f);

        newCustomer.GetComponent<Customer>().SetValues(customersToSpawn[numCustomersSpawned].desiredFood, customersToSpawn[numCustomersSpawned].timeTakenToEat);

        numCustomersSpawned++;
    }

    public void CustomerFed(int pointsToAdd)
    {
        numCustomersFed++;
        score += pointsToAdd;

        if (numCustomersFed >= numberOfTotalCustomers)
            LevelEnd();

        //if (spawnNewCustomerAfterEating)
          //  SpawnCustomer();
    }

    public void LevelEnd()
    {
        endLevelCanvas.gameObject.SetActive(true);
        endLevelCanvas.SetDisplayScore(score);
        int numStars = 0;
        for (int i=0; i<5; i++)
        {
            if (score >= starThresholds[i])
                numStars++;
        }
        endLevelCanvas.DisplayStars(numStars);
    }
}
