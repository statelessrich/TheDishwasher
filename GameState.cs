using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameState : MonoBehaviour {
    
    #region Properties

    #region Dishwasher
    private GameObject dishwasher;
    private GameObject dishwasherWalking;
    private DishwasherManager dishwasherManager;
    #endregion Dishwasher

    #region Misc. Managers
    private CursorManager cursorManager;
    private AudioManager audioManager;
    #endregion

    #region Dish
    private DishManager dishManager;
    public GameObject dish;
    public GameObject dishSpawnpoint;
    private const float dishSpawnDelay = 0.5f;
    #endregion Dish

    #region Customers
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

    private Vector2 customerPositionSpawn1;
    private Vector2 customerPositionSpawn2;
    private Vector2 customerPositionMid;
    private Vector2 customerPositionEnd;
    #endregion Customers

    #region GUI
    public GameObject debugText;
    public GameObject mousePositionText;
    private GameObject exitButton;
    private GameObject resumeButton;
    #endregion GUI

    #region Cameras
    private Camera diningCamera;
    private Camera dishCamera;
    private Camera transitionCamera;
    private Camera pauseCamera;
    private Camera lastCamera;
    private Camera currentCamera;
    #endregion Cameras

    #region Customer/Wave Info
    public List<GameObject> customers;
    public int[] customerWaves;
    public int currentWave;
    // Customers currently on screen.
    public int activeCustomers;
    // Total customers created in current wave.
    public int customerCount;
    public int customersInWave;
    public int currentWaveKillCount;
    private const float customerSpawnCooldownMax = 2f;
    private float customerSpawnCooldown = 0f;
    #endregion Customer/Wave Info

    #region Scoring
    public int missCount;
    #endregion Scoring

    #region State
    public enum State {
        Title,
        Transition,
        Pause,
        Tutorial,
        Credits,
        Dining,
        Dish,
        KillScreen
    }
    public State currentState;
    private State previousState;
    #endregion State

    #region Flags
    public const bool debug = false;
    private bool throwing;
    private bool clicked;
    private bool clickedInHitArea;
    private bool customerInHitArea;
    #endregion Flags

    #region Transitions
    private SpriteRenderer richTransitionSprite;
    private SpriteRenderer livingTransitionSprite;
    private SpriteRenderer minimumTransitionSprite;
    #endregion Transitions

    #endregion Properties

    // Use this for initialization
	void Start ()
	{	   
        // Managers
	    cursorManager = GetComponent<CursorManager>();
        
        /* Check if AudioManager from Title scene exists. 
         * If so, set to Title scene AudioManager and delete the game scene's version.
         * Else, set the AudioManager to the game scene's audio manager.
         */
        if (GameObject.Find("AudioManager")) {
            Destroy(GameObject.Find("AudioManagerGame"));
            audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        } else {
            audioManager = GameObject.Find("AudioManagerGame").GetComponent<AudioManager>();
        }

		// Dishwasher
	    dishwasherManager = GameObject.FindGameObjectWithTag("Dishwasher").GetComponent<DishwasherManager>();
	    dishwasher = GameObject.Find("Dishwasher");
        dishwasherWalking = GameObject.Find("DishwasherWalking");
        
        // Customers
        customerPositionSpawn1 = GameObject.Find("CustomerPositionSpawn1").GetComponent<Transform>().position;
        customerPositionSpawn2 = GameObject.Find("CustomerPositionSpawn2").GetComponent<Transform>().position;
        customerPositionMid = GameObject.Find("CustomerPositionMid").GetComponent<Transform>().position;
        customerPositionEnd = GameObject.Find("CustomerPositionEnd").GetComponent<Transform>().position;

	    // Cameras
        diningCamera = GameObject.Find("DiningCamera").GetComponent<Camera>();
        dishCamera = GameObject.Find("DishCamera").GetComponent<Camera>();
        transitionCamera = GameObject.Find("TransitionCamera").GetComponent<Camera>();
	    pauseCamera = GameObject.Find("PauseCamera").GetComponent<Camera>();
	    currentCamera = dishCamera;

        // GUI
	    mousePositionText = GameObject.Find("MousePositionText");
	    exitButton = GameObject.Find("ExitButton");
	    resumeButton = GameObject.Find("ResumeButton");

        // Transition screens
	    richTransitionSprite = GameObject.Find("RichTransition").GetComponent<SpriteRenderer>();
        livingTransitionSprite = GameObject.Find("LivingTransition").GetComponent<SpriteRenderer>();
        minimumTransitionSprite = GameObject.Find("MinimumTransition").GetComponent<SpriteRenderer>();

		// Initial state
	    SetState(State.Dish);
	}

    // Update is called once per frame.
    void Update() {
        if (debug)
        {
            mousePositionText.guiText.text = Input.mousePosition.x + "," + Input.mousePosition.y;
        }

        GetInput();

        switch (currentState)
        {
            case State.Dining:                
                // End wave if missed 5 consecutive times.
                if (missCount == 5)
                {
                    EndWave();
                }

                if (customerSpawnCooldown > 0)
                {
                    // Decrement cooldown.
                    customerSpawnCooldown -= Time.deltaTime;
                }
                else if (customerCount < customersInWave)
                {
                    // Cooldown over. Spawn another customer if there are more in this wave.
                    SpawnRandomCustomer();
                }
                break;
        }

        if (debug) {
            List<String> debugInfo = new List<String>();
            debugInfo.Add("wave: " + (currentWave + 1));
            debugInfo.Add("customers in wave: " + customersInWave);
            debugInfo.Add("active customers: " + activeCustomers);
            debugInfo.Add("total customers: " + customerCount);
            
            String str = "";
            foreach (String info in debugInfo) {
                str += info;
                str += "\n";
            } 
                
            debugText.guiText.text = str;
        }
    }

    public void StartCustomerSpawnCooldown() 
    {
        customerSpawnCooldown = customerSpawnCooldownMax;
    }

    private void SpawnRandomCustomer()
    {
        activeCustomers++;
        customerCount++;
        int random = Random.Range(1, 7);

        switch (random)
        {
            case 1:
                SpawnCustomer(customer1, customer1Instance, customer1Manager);
                break;
            case 2:
                SpawnCustomer(customer2, customer2Instance, customer2Manager);
                break;
            case 3:
                SpawnCustomer(customer3, customer3Instance, customer3Manager);
                break;
            case 4:
                SpawnCustomer(customer4, customer4Instance, customer4Manager);
                break;
            case 5:
                SpawnCustomer(customer5, customer5Instance, customer5Manager);
                break;
            case 6:
                SpawnCustomer(customer6, customer6Instance, customer6Manager);
                break;
        }
    }

    //TODO: Refactor?
    public void SpawnCustomer(GameObject customer, GameObject customerInstance, CustomerManager customerManager) {
        // Randomly pick one of two spawnpoints.
        int random = Random.Range(1, 3);

        Vector2 customerPositionSpawn;

        if (random == 1)
        {
            // Spawn at bottom of screen. Adjust spawnpoint y by a random amount.
            customerPositionSpawn = customerPositionSpawn1;
            random = Random.Range(1, 4);
            customerPositionSpawn.y -= random;
        }
        else
        {
            // Spawn at left of screen. Adjust spawnpoint x by a random amount.
            customerPositionSpawn = customerPositionSpawn2;
            random = Random.Range(1, 4);
            customerPositionSpawn.x -= random;
        }

        Debug.Log("spawnpoint = " + customerPositionSpawn.x + "," + customerPositionSpawn.y);

        // Instantiate customer.
        customerInstance = (GameObject)Instantiate(customer, customerPositionSpawn, gameObject.transform.rotation);

        if (customerInstance != null) {
            customerInstance.SetActive(true);
            customerInstance.name = "Customer_" + customerCount;

            customerManager = customerInstance.GetComponent<CustomerManager>();
            customerManager.enabled = true;
            customerManager.SetPositionMid(customerPositionMid);
            customerManager.SetPositionEnd(customerPositionEnd);

            // Add to customer list.
            customers.Add(customerInstance);

            // Enter state.
            customerManager.SetState(CustomerManager.CustomerStates.MoveToMid);

            // Start cooldown until another customer spawns.
            StartCustomerSpawnCooldown();
        } else {
            Debug.Log("Failed to instantiate customer");
        }
    }

    bool IsValidClick(Vector3 mousePosition)
    {
        // Click position must not be too close to the Dishwasher, and cannot currently be throwing.
        if (diningCamera.ScreenToWorldPoint(mousePosition).x > dishwasherManager.GetPositionX() + 4 && !throwing)
        {
            return true;
        }

        return false;
    }
	
    void GetInput() 
    {
        if (currentState == State.Dining)
        {
            if (!clicked && Input.GetMouseButtonDown(0) && IsValidClick(Input.mousePosition))
            {
                //Debug.Log("dish x: " + dishwasherManager.GetPositionX());
                if (dishwasherManager.GetDishThrowCooldown() <= 0f || debug)
                {
                    throwing = true;
                    Vector2 clickedPosition = diningCamera.ScreenToWorldPoint(Input.mousePosition);
                    
                    StartCoroutine(SpawnDish(clickedPosition, dishSpawnDelay));
                    dishwasherManager.GetComponent<Animator>().Play("throwing");
                }
            }
            else if (clicked)
            {
                clicked = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (currentState)
            {
                case State.Dining:
                    previousState = State.Dining;
                    PauseGame();
                    break;
                
                case State.Dish:
                    previousState = State.Dish;
                    PauseGame();
                    break;
                
                case State.Transition:
                    previousState = State.Transition;
                    PauseGame();
                    break;

                case State.Pause:
                    currentState = previousState;
                    // Unpause game.
                    audioManager.PlayMenuSFX();
                    UnpauseGame();
                    break;
            }
        }
    }

    public void PauseGame()
    {
        // Display pause screen.
        currentState = State.Pause;
        currentCamera.enabled = false;
        pauseCamera.enabled = true;

        mousePositionText.guiText.enabled = false;
        audioManager.PauseThemeBGM();

        cursorManager.ShowCursor();

        // Enable pause screen buttons.
        exitButton.SetActive(true);
        resumeButton.SetActive(true);
        
        // Pause game.
        Time.timeScale = 0;
    }

    public void UnpauseGame()
    {
        // Must set to true, else click will be processed in Update() as well.
        clicked = true;
        currentState = previousState;
        Time.timeScale = 1;
        mousePositionText.guiText.enabled = true;
        audioManager.PlayThemeBGM();

        // Disable pause screen buttons.
        exitButton.SetActive(false);
        resumeButton.SetActive(false);

        // Re-enable last camera.
        switch (currentCamera.name) { 
            case "DiningCamera":
                diningCamera.enabled = true;
                cursorManager.SetCursorNormal();
                break;
            
            case "DishCamera":
                dishCamera.enabled = true;
                cursorManager.HideCursor();
                break;

            case "TransitionCamera":
                transitionCamera.enabled = true;
                cursorManager.HideCursor();
                break;
        }
    }

    public IEnumerator SpawnDish(Vector2 clickedPosition, float duration)
    {
        if (!debug)
        {
            yield return new WaitForSeconds(duration);
        }

        GameObject dishInstance = Instantiate(dish, dishSpawnpoint.transform.position, dishSpawnpoint.transform.rotation) as GameObject;
        
        dishInstance.SetActive(true);
        
        if (dishInstance != null)
        {
            dishManager = dishInstance.GetComponent<DishManager>();
        }
        else
        {
            Debug.Log("Failed to instantiate dish");
        }
        dishManager.SetClickedPosition(clickedPosition);
        dishManager.ChangeState(DishManager.DishStates.Throw);
        dishwasherManager.StartDishThrowCooldown();
        throwing = false;
    }

    public void HitCustomer(GameObject customer)
    {
        customer.GetComponent<CustomerManager>().SetState(CustomerManager.CustomerStates.Hit);
    }

    public void KilledCustomer(int customerPointValue)
    {
        missCount = 0;
        CustomerLeft();
    }

    public void SetState(State state)
    {
        currentState = state;

        if (currentState == State.Dish)
        {
            EnterDishState();
        }
        if (currentState == State.Dining)
        {
            EnterDiningState();
        }

        if (currentState == State.KillScreen)
        {
            EnterKillScreenState();
        }
    }

    public State GetCurrentState()
    {
        return currentState;
    }

    public void CustomerLeft()
    {
        SetCustomerInHitArea(false);
        activeCustomers--;

        if (activeCustomers == 0)
        {
            //Debug.Log("wave " + currentWave + " complete");
            EndWave();
        }
    }

    private void StartWave()
    {
        //Debug.Log("start wave " + currentWave);
        currentWaveKillCount = 0;
        missCount = 0;
        customerCount = 0;
        customersInWave = customerWaves[currentWave];
        SpawnRandomCustomer();
    }

    private void EndWave()
    {
        if (currentWave == customerWaves.Length - 1)
        {
            // Finished last wave. Display kill screen.
            SetState(State.KillScreen);
        }
        else
        {
            // Show transition and start next wave.
            SetState(State.Transition);
            StartCoroutine(DisplayTransition());

            customers.Clear();
            currentWave++;
        }
    }

    private IEnumerator DisplayTransition()
    {
        cursorManager.HideCursor();
        // Wait 2 seconds.
        yield return new WaitForSeconds(2);

        switch (currentWave)
        {
            case 1:
                minimumTransitionSprite.enabled = true;
                break;
            case 2:
                minimumTransitionSprite.enabled = false;
                livingTransitionSprite.enabled = true;
                break;
            case 3:
                livingTransitionSprite.enabled = false;
                richTransitionSprite.enabled = true;
                break;
        }

        diningCamera.enabled = false;
        transitionCamera.enabled = true;
        currentCamera = transitionCamera;
        
        // Wait 5 seconds.
        yield return new WaitForSeconds(5);
        transitionCamera.enabled = false; 
        diningCamera.enabled = true;
        currentCamera = diningCamera;

        EnterDishState();
    }

    private void EnterDiningState()
    {
        cursorManager.ShowCursor();
        dishwasher.gameObject.SetActive(true);
        dishwasherWalking.gameObject.SetActive(false);
        dishwasherManager = GameObject.Find("Dishwasher").GetComponent<DishwasherManager>();
        
        diningCamera.enabled = true;
        dishCamera.enabled = false;
        currentCamera = diningCamera;

        StartWave();
    }

    private void EnterDishState()
    {
        // Disable regular Dishwasher game object.
        dishwasher.gameObject.SetActive(false);

        // Enable walking Dishwasher game object. Walk toward dish room.
        dishwasherWalking.gameObject.SetActive(true);
        dishwasherManager = GameObject.Find("DishwasherWalking").GetComponent<DishwasherManager>();
        dishwasherManager.SetState(DishwasherManager.DishwasherState.MoveToBusPosition);

        diningCamera.enabled = false;
        dishCamera.enabled = true;
        currentCamera = dishCamera;
    }

    void EnterKillScreenState() {
        // TODO: Kill screen.
    }

    public void DishMissedCustomer()
    {
        missCount++;
    }

    public void PlayMenuSFX()
    {
        audioManager.PlayMenuSFX();
    }
    public void PlayExplosionSFX() 
    {
        audioManager.PlayExplosionSFX();
    }

    public void SetCursorHover()
    {
        if (currentState == State.Dining)
        {
            cursorManager.SetCursorHover();
        }
    }

    public void SetCursorNormal()
    {
        if (currentState == State.Dining)
        {
            cursorManager.SetCursorNormal();
        }
    }

    public void SetCustomerInHitArea(bool inHitArea)
    {
        this.customerInHitArea = inHitArea;
        //Debug.Log("customer in hit area: " + customerInHitArea);
    }

    public void SetClickedInHitArea(bool inHitArea)
    {
        this.clickedInHitArea = inHitArea;
        //Debug.Log("clicked in hit area: " + clickedInHitArea);
    }

    public bool GetCustomerInHitArea()
    {
        return customerInHitArea;
    }

    public bool GetClickedInHitArea()
    {
        return clickedInHitArea;
    }
}
