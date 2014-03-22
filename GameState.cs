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
    private TitleManager titleManager;
    #endregion

    #region Dish
    private DishManager dishManager;
    public GameObject dish;
    public GameObject dishSpawnpoint;
    private const float dishSpawnDelay = 0.5f;
    #endregion Dish

    #region Customer Positions
    private Vector2 customerPositionSpawn1;
    private Vector2 customerPositionSpawn2;
    private Vector2 customerPositionMid;
    private Vector2 customerPositionEnd;
    #endregion Customer Positions

    #region GUI
    private GameObject exitButton;
    private GameObject resumeButton;
    #endregion GUI

    #region Cameras
    private Camera diningCamera;
    private Camera dishCamera;
    private Camera transitionCamera;
    private Camera pauseCamera;
    private Camera currentCamera;
    #endregion Cameras

    #region Customer/Wave Info
    public int[] customerWaves;
    private int currentWave;
    // Customers currently on screen.
    private int activeCustomers;
    // Total customers created in current wave.
    private int customerCount;
    private int customersInWave;
    private const float customerSpawnCooldownMax = 2f;
    private float customerSpawnCooldown = 0f;
    #endregion Customer/Wave Info

    #region State
    public enum State 
    {
        Title,
        Transition,
        Pause,
        Tutorial,
        Credits,
        Dining,
        Dish
    }
    private State currentState;
    private State previousState;
    #endregion State

    #region Flags
    private bool debug = false;
    private bool throwing;
    private bool clicked;
    private bool clickedInHitArea;
    private bool cursorInHitArea;
    #endregion Flags

    private Ray ray;
    private RaycastHit hit;

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
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
	    titleManager = GameObject.Find("TitleManager").GetComponent<TitleManager>();

		// Dishwasher
	    dishwasherManager = GameObject.Find("Dishwasher").GetComponent<DishwasherManager>();
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
	    exitButton = GameObject.Find("ExitButton");
	    resumeButton = GameObject.Find("ResumeButton");

        // Transition screens
	    richTransitionSprite = GameObject.Find("RichTransition").GetComponent<SpriteRenderer>();
        livingTransitionSprite = GameObject.Find("LivingTransition").GetComponent<SpriteRenderer>();
        minimumTransitionSprite = GameObject.Find("MinimumTransition").GetComponent<SpriteRenderer>();

		// Initial state
	    SetState(State.Title);
	    if (debug)
	    {
	        SetState(State.Dining);
	    }
	}

    // Update is called once per frame.
    void Update() 
    {
        GetInput();

        switch (currentState)
        {
            case State.Dining:
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
                /* 
                 * Cast ray from mouse position to the immediate right. If did not hit the hit area collider,
                 * set cursor outside of hit area. This is done because we can't rely on the hit area's
                 * OnTriggerEnter and OnTriggerExit.
                 * 
                 */
                if (!Physics2D.Raycast(diningCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.right, 1f))
                {
                    SetCursorInHitArea(false);
                }
                break;
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
        SpawnCustomer(GameObject.FindGameObjectWithTag("Customer" + random));
    }

    public void SpawnCustomer(GameObject customer) {
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

        // Instantiate customer.
        GameObject customerInstance = (GameObject)Instantiate(customer, customerPositionSpawn, gameObject.transform.rotation);

        if (customerInstance != null) {
            customerInstance.SetActive(true);
            customerInstance.name = "Customer_" + customerCount;

            CustomerManager customerManager = customerInstance.GetComponent<CustomerManager>();
            customerManager.enabled = true;
            customerManager.SetPositionMid(customerPositionMid);
            customerManager.SetPositionEnd(customerPositionEnd);

            // Enter state.
            customerManager.SetState(CustomerManager.CustomerStates.MoveToMid);

            // Start cooldown until another customer spawns.
            StartCustomerSpawnCooldown();
        } 
        else 
        {
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

        dishManager.SetClickedPosition(clickedPosition);
        dishManager.ChangeState(DishManager.DishStates.Throw);
        dishwasherManager.StartDishThrowCooldown();
        throwing = false;
        audioManager.PlayRandomVoiceover();
    }

    public void HitCustomer(GameObject customer)
    {
        customer.GetComponent<CustomerManager>().SetState(CustomerManager.CustomerStates.Hit);
    }

    public void KilledCustomer()
    {
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

        if (currentState == State.Credits)
        {
            EnterCreditsState();
        }
    }

    public State GetCurrentState()
    {
        return currentState;
    }

    public void CustomerLeft()
    {
        activeCustomers--;

        if (activeCustomers == 0)
        {
            EndWave();
        }
    }

    private void StartWave()
    {
        customerCount = 0;
        customersInWave = customerWaves[currentWave];
        SpawnRandomCustomer();
    }

    private void EndWave()
    {
        if (currentWave == customerWaves.Length - 1)
        {
            // Finished last wave. Display kill screen.
            SetState(State.Credits);
        }
        else
        {
            // Show transition and start next wave.
            SetState(State.Transition);
            StartCoroutine(DisplayTransition());

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

        cursorManager.HideCursor();
        diningCamera.enabled = false;
        dishCamera.enabled = true;
        currentCamera = dishCamera;
    }

    void EnterCreditsState()
    {
        StartCoroutine(WaitAndDisplayCredits());
    }

    IEnumerator WaitAndDisplayCredits()
    {
        cursorManager.HideCursor();
        yield return new WaitForSeconds(2);
        titleManager.ChangeState(TitleManager.TitleState.EndCredits);
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

    public void SetCursorInHitArea(bool inHitArea)
    {
        this.cursorInHitArea = inHitArea;
    }

    public void SetClickedInHitArea(bool inHitArea)
    {
        this.clickedInHitArea = inHitArea;
    }

    public bool GetCursorInHitArea() 
    {
        return cursorInHitArea;
    }

    public bool GetClickedInHitArea()
    {
        return clickedInHitArea;
    }

    public void HideCursor()
    {
        cursorManager.HideCursor();
    }

    public void ShowCursor()
    {
        cursorManager.ShowCursor();
    }

    public void ExitToTitle()
    {
        SetState(State.Title);
        titleManager.ChangeState(TitleManager.TitleState.Title);
        titleManager.DisplayTitle();
        audioManager.PlayThemeBGM();
    }

    public void ResetGame()
    {
        currentWave = 0;
        Time.timeScale = 1;
        SetState(State.Dish);
        dishwasherManager.SetState(DishwasherManager.DishwasherState.MoveToBusPosition);
    }
}
