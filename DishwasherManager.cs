using UnityEngine;

public class DishwasherManager : MonoBehaviour
{
    private GameState gameState;
    public DishwasherState currentState;

    private GameObject diningExit;
    private GameObject dishExit;
    private GameObject diningPosition;
    private GameObject dishwasherBusPosition;
    private GameObject busPosition;
    private GameObject bus;
    
    private float speed = 5;
    private float dishThrowCooldownMax = 1f;
    private float dishThrowCooldown = 0f;

    public enum DishwasherState
    {
        Idle,
        MoveToDiningExit,
        MoveToBusPosition,
        MoveToDishExit
    }

    public float GetDishThrowCooldown()
    {
        return dishThrowCooldown;
    }

    public void StartDishThrowCooldown()
    {
        dishThrowCooldown = dishThrowCooldownMax;
    }

	// Use this for initialization
	void Start ()
	{
	    bus = GameObject.Find("PlateBus");
        busPosition = GameObject.Find("BusPosition");

        gameState = GameObject.FindGameObjectWithTag("GameState").GetComponent<GameState>();
	    
        diningExit = GameObject.FindGameObjectWithTag("DishwasherDiningExit");
        dishExit = GameObject.FindGameObjectWithTag("DishwasherDishExit");
        dishwasherBusPosition = GameObject.Find("DishwasherBusPosition");
        diningPosition = GameObject.FindGameObjectWithTag("DishwasherDiningPosition");

        SetState(DishwasherState.Idle);
	}
	
	// Update is called once per frame
    private void Update()
    {
        if (dishThrowCooldown > 0)
        {
            // Decrement dish throwing cooldown.
            dishThrowCooldown -= Time.deltaTime;
        }

        switch (currentState)
        {
            case DishwasherState.Idle:
                break;

            case DishwasherState.MoveToDiningExit:
                // Check if Dishwasher has reached exit.
                if (gameObject.transform.position.x <= diningExit.transform.position.x)
                {
                    Debug.Log("REACHED EXIT");
                    gameState.SetState(GameState.State.Dish);
                    transform.position = dishExit.transform.position;
                }
                // Move Dishwasher to the left, off the screen.
                transform.position = Vector2.MoveTowards(transform.position, diningExit.transform.position, speed*Time.deltaTime);
                break;

            case DishwasherState.MoveToBusPosition:
                if (gameObject.transform.position.x <= dishwasherBusPosition.transform.position.x) {
                    SetState(DishwasherState.MoveToDishExit);
                }
                // Move Dishwasher to the left, to the bus cart.
                transform.position = Vector2.MoveTowards(transform.position, dishwasherBusPosition.transform.position, speed * Time.deltaTime);
                break;

            case DishwasherState.MoveToDishExit:
                // Check if Dishwasher has reached exit.
                if (gameObject.transform.position.x >= dishExit.transform.position.x) {
                    gameState.SetState(GameState.State.Dining);
                    
                    // Set idle state and animation.
                    SetState(DishwasherState.Idle);
                    GetComponent<Animator>().Play("idle");
                }

                // Move Dishwasher and bus to the right, toward dining room.
                transform.position = Vector2.MoveTowards(transform.position, dishExit.transform.position, speed * Time.deltaTime);
                bus.transform.position = Vector2.MoveTowards(bus.transform.position, new Vector2(15, bus.transform.position.y), speed * Time.deltaTime);
                break;
        }
    }

    public void SetState(DishwasherState state)
    {
        currentState = state;

        switch (currentState)
        {
            case DishwasherState.Idle:
                EnterDiningState();
                break;

            case DishwasherState.MoveToBusPosition:
                EnterMoveToBusPositionState();
                break;

            case DishwasherState.MoveToDiningExit:
                EnterMoveToDiningExitState();
                break;

            case DishwasherState.MoveToDishExit:
                EnterMoveToDishExitState();
                break;
        }
    }

    void EnterDiningState() {
        GetComponent<Animator>().Play("Idle");
        gameObject.transform.position = diningPosition.transform.position;
    }

    void EnterMoveToBusPositionState()
    {
        bus.transform.position = busPosition.transform.position;
        transform.position = dishExit.transform.position;
        // Flip Dishwasher to face left.
        transform.eulerAngles = new Vector3(0, -180, 0);
        GetComponent<Animator>().Play("Walking");
    }

    void EnterMoveToDiningExitState() {
        GetComponent<Animator>().Play("Walking");
    }

    void EnterMoveToDishExitState() {
        // Flip Dishwasher to face right.
        gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        // Set bus position.
        bus.transform.position = new Vector2(bus.transform.position.x + 7, bus.transform.position.y);
    }
    
    public float GetPositionX()
    {
        return transform.position.x;
    }
}
