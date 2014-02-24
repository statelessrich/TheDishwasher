using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameState : MonoBehaviour {
    #region variables
    private DishwasherManager dishwasherManager;

    private PlateManager plateManager;
    public GameObject plate;
    public GameObject plateSpawnpoint;

    public GameObject customer1;
    private GameObject customer1Instance;
    private CustomerManager customer1Manager;

    public GameObject customer2;
    private GameObject customer2Instance;
    private CustomerManager customer2Manager;

    public GameObject customer3;
    private GameObject customer3Instance;
    private CustomerManager customer3Manager;

    public GameObject customer4;
    private GameObject customer4Instance;
    private CustomerManager customer4Manager;

    public GameObject customer5;
    private GameObject customer5Instance;
    private CustomerManager customer5Manager;

    public GameObject customer6;
    private GameObject customer6Instance;
    private CustomerManager customer6Manager;

    private Vector2 customerPositionSpawn;
    private Vector2 customerPositionMid;
    private Vector2 customerPositionEnd;
    
    public GameObject winText;
    public GameObject debugText;

    private Camera diningCamera;
    private Camera dishCamera;

    public List<GameObject> customers;

    public int[] customerWaves;
    private const int totalWaves = 3;
    public int currentWave = 0;

    // Customers currently on screen.
    public int activeCustomers = 0;
    // Total customers created in current wave.
    public int customerCount = 0;
    public int customersInWave = 0;
    public int currentWaveKillCount = 0;
    public double pctCustomersKilled = 0.5;
    
    private int killCount = 0;

    public int platePileLevel = 1;
    public int maxPlatePileLevel = 3;

    public AudioSource BGM;
    public AudioSource plateHitSound;

    public const bool debug = false;
    
    public State currentState;
    private float dishSpawnDelay = 0.5f;

    private bool throwing = false;

    public enum State
    {
        Menu,
        Dining,
        Dish,
        Win,
        GameOver
    }

    #endregion variables

    // Use this for initialization
	void Start ()
	{
	    //debug = false;

	    //PlateManager = GameObject.FindGameObjectWithTag("Plate").GetComponent<PlateManager>();
        //plateSpawnpoint = GameObject.FindGameObjectWithTag("PlateSpawnpoint");
	    //customer1Spawnpoint = GameObject.FindGameObjectWithTag("Customer1Spawnpoint");
	    //customer2Spawnpoint = GameObject.FindGameObjectWithTag("Customer2Spawnpoint");

        // Load prefabs.
        //plate = Resources.Load("Prefabs/Plate.prefab", typeof(GameObject)) as GameObject;
        //customer1 = Resources.Load("Prefabs/Customer.prefab", typeof(GameObject)) as GameObject;
        //customer2 = Resources.Load("Prefabs/Customer.prefab", typeof(GameObject)) as GameObject;
	    dishwasherManager = GameObject.FindGameObjectWithTag("Dishwasher").GetComponent<DishwasherManager>();

        // Customers
        customerPositionSpawn = GameObject.Find("CustomerPositionSpawn").GetComponent<Transform>().position;
        customerPositionMid = GameObject.Find("CustomerPositionMid").GetComponent<Transform>().position;
        customerPositionEnd = GameObject.Find("CustomerPositionEnd").GetComponent<Transform>().position;

        diningCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        dishCamera = GameObject.FindGameObjectWithTag("DishRoomCamera").GetComponent<Camera>();
        dishCamera.enabled = false;

	    SetState(State.Dining);

	    //customerWaves = new int[totalWaves] {4, 4, 4};
        
		//SpawnCustomer1();
        // Initial spawns.
        /*
        
        SpawnCustomer2();
        SpawnCustomer3();
        SpawnCustomer4();
         * */
        
        //SpawnRandomCustomer();
        
        customersInWave = customerWaves[currentWave];

	}

    // Update is called once per frame.
    void Update() {
        if (currentState == State.Dining) 
        {
            if (!diningCamera.enabled) {
                dishCamera.enabled = false;
                diningCamera.enabled = true;
            }

            GetInput();
        } 
        else if (currentState == State.Dish) {
            if (!dishCamera.enabled) {
                diningCamera.enabled = false;
                dishCamera.enabled = true;

                StartCoroutine(DishScene(1));
            }
        } 
        else if (currentState == State.Win) {
            winText.SetActive(true);
        }

        if (debug) {
            List<String> debugInfo = new List<String>();
            debugInfo.Add("wave: " + (currentWave + 1));
            debugInfo.Add("customers in wave: " + customersInWave);
            debugInfo.Add("active customers: " + activeCustomers);
            debugInfo.Add("total customers: " + customerCount);
            debugInfo.Add("total kills: " + killCount);
            debugInfo.Add("current wave kills: " + currentWaveKillCount);
            debugInfo.Add("plate pile level (1-3): " + platePileLevel);
            
            String str = "";
            foreach (String info in debugInfo) {
                str += info;
                str += "\n";
            } 
                
            debugText.guiText.text = str;
        }

    }

    IEnumerator DishScene(float duration)
    {
        //yield return new WaitForSeconds(duration);

        Debug.Log("kill goal: " + (double)customersInWave * pctCustomersKilled);
        // Change plate pile level.
        if (currentWaveKillCount < ((double)customersInWave * pctCustomersKilled)) {
            
        } else {
            
        }

        //yield return new WaitForSeconds(duration);

        if (currentState != State.GameOver) {
            dishwasherManager.SetState(DishwasherManager.DishwasherState.MoveToBusPosition);
            //MoveToDiningRoom();
        }

        return null;
    }

    private void SpawnRandomCustomer()
    {
        activeCustomers++;
        customerCount++;
        int random = Random.Range(1, 6);

        switch (random)
        {
            case 1:
                SpawnCustomer1();
                break;
            case 2:
                SpawnCustomer2();
                break;
            case 3:
                SpawnCustomer3();
                break;
            case 4:
                SpawnCustomer4();
                break;
            case 5:
                SpawnCustomer5();
                break;
            case 6:
                SpawnCustomer6();
                break;
        }
    }

    void SpawnCustomer1()
    {
        SpawnCustomer(customer1, customer1Instance, customer1Manager, "Customer1");
        //currentCustomer++;
    }

    void SpawnCustomer2()
    {
        SpawnCustomer(customer2, customer2Instance, customer2Manager, "Customer2");
        //currentCustomer++;
    }

    void SpawnCustomer3() 
    {
         SpawnCustomer(customer3, customer3Instance, customer3Manager, "Customer3");
         //currentCustomer++;
    }
    void SpawnCustomer4() 
    {
        SpawnCustomer(customer4, customer4Instance, customer4Manager, "Customer4");
        //currentCustomer++;
    }
    void SpawnCustomer5() {
        SpawnCustomer(customer5, customer5Instance, customer5Manager, "Customer5");
        //currentCustomer++;
    }
    void SpawnCustomer6() {
        SpawnCustomer(customer6, customer6Instance, customer6Manager, "Customer6");
        //currentCustomer++;
    }

    public void SpawnCustomer(GameObject customer, GameObject customerInstance, CustomerManager customerManager, string tag) {
        // Instantiate, add collider, set to active.
        customerInstance = Instantiate(customer, customerPositionSpawn, gameObject.transform.rotation) as GameObject;

        if (customerInstance != null) {
            //customerInstance.AddComponent<PolygonCollider2D>();
            //customerInstance.GetComponent<PolygonCollider2D>().isTrigger = true;
            customerInstance.SetActive(true);
            //customerInstance.tag = tag;
            customerInstance.name = "Customer_" + customerCount;

            customerManager = customerInstance.GetComponent<CustomerManager>();
            customerManager.enabled = true;
            customerManager.SetPositionMid(customerPositionMid);
            customerManager.SetPositionEnd(customerPositionEnd);

            // Add to customer list.
            customers.Add(customerInstance);

            // Enter state.
            customerManager.SetState(CustomerManager.CustomerStates.MoveToMid);
        } else {
            Debug.Log("Failed to instantiate " + tag);
        }
    }
	
    void GetInput() 
    {
        if (Input.GetMouseButtonDown(0) && !throwing) 
        {
            if (dishwasherManager.GetDishThrowCooldown() <= 0f || debug)
            {
                throwing = true;
                StartCoroutine(SpawnDish(diningCamera.ScreenToWorldPoint(Input.mousePosition), dishSpawnDelay));
                dishwasherManager.GetComponent<Animator>().Play("throwing");
            }
        } 
        
        if (Input.GetKey(KeyCode.RightArrow)) 
        {
            Debug.Log("hit right");
            Application.LoadLevel("The Dishwasher");
        }
    }

    public IEnumerator SpawnDish(Vector2 targetPosition, float duration)
    {
        if (!debug)
        {
            yield return new WaitForSeconds(duration);
        }

        GameObject plateInstance = Instantiate(plate, plateSpawnpoint.transform.position, plateSpawnpoint.transform.rotation) as GameObject;
        
        plateInstance.SetActive(true);
        
        if (plateInstance != null)
        {
            plateManager = plateInstance.GetComponent<PlateManager>();
        }
        else
        {
            Debug.Log("Failed to instantiate plate");
        }
        plateManager.SetTarget(targetPosition);
        plateManager.ChangeState(PlateManager.PlateStates.Throw);
        dishwasherManager.StartDishThrowCooldown();
        throwing = false;
    }

    public void HitCustomer(GameObject customer)
    {
        //plateHitSound.Play();
        customer.GetComponent<CustomerManager>().SetState(CustomerManager.CustomerStates.Hit);
    }

    public void KilledCustomer(String customerTag)
    {
        killCount++;
        currentWaveKillCount++;
        CustomerLeft();

        if (customerCount < customersInWave)
        {
            SpawnCustomer(customerTag);
        }
        else
        {
            //SetState(State.Win);
            //winText.SetActive(true);
        }
    }

    public void SpawnCustomer(String customerTag)
    {
        // Spawn new customer.
        switch (customerTag) {
            case "Customer1":
                SpawnCustomer1();
                break;
            case "Customer2":
                SpawnCustomer2();
                break;
            case "Customer3":
                SpawnCustomer3();
                break;
            case "Customer4":
                SpawnCustomer4();
                break;
        }
    }

    public void SetState(State state)
    {
        currentState = state;

        if (currentState == State.Dining)
        {
            EnterDiningState();
        }

        if (currentState == State.Win) {
            EnterWinState();
        }
        else if (currentState == State.GameOver) {
            EnterGameOverState();
        }
    }

    private void EnterWinState() {

    }

    private void EnterGameOverState() {
        winText.guiText.text = "GAME OVER";
        winText.SetActive(true);
    }  

    public State GetCurrentState()
    {
        return currentState;
    }

    public void CustomerLeft()
    {
        activeCustomers--;
        if (customerCount < customersInWave)
        {
            SpawnRandomCustomer();
        }
        else if (activeCustomers == 0)
        {
            Debug.Log("wave " + currentWave + " complete");
            EndWave();
        }
    }

    private void StartWave() {
        currentWave++;
        currentWaveKillCount = 0;
        customersInWave = customerWaves[currentWave];
        customerCount = 0;

        SpawnRandomCustomer();

        // Spawn first 4 customers in wave.
        for (int i = 0; i < 4; i++) {
            //SpawnRandomCustomer();
        }
    }

    private void EndWave()
    {
        if (currentWave == customerWaves.Length - 1)
        {
            Debug.Log("all waves complete");
            SetState(State.Win);
        }
        else
        {
            customers.Clear();
            dishwasherManager.SetState(DishwasherManager.DishwasherState.MoveToDiningExit);
        }
    }
    
    public void EnterDiningState()
    {
        StartWave();
    }
}
