using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using System.Collections;

public class DishwasherManager : MonoBehaviour
{
    private GameState gameState;
    public DishwasherState currentState;
    private GameObject diningExit;
    private GameObject dishExit;
    private GameObject diningPosition;
    private GameObject dishwasherBusPosition;
    private GameObject busPosition;
    private float speed = 5;
    private SpriteRenderer spriteRenderer;
    private GameObject bus;

    public enum DishwasherState
    {
        Idle,
        MoveToDishExit,
        MoveToDiningExit,
        MoveToBusPosition,
        MoveToDiningPosition
    }

	// Use this for initialization
	void Start ()
	{
	    spriteRenderer = GetComponentInChildren<SpriteRenderer>();

	    bus = GameObject.Find("PlateBus");

        gameState = GameObject.FindGameObjectWithTag("GameState").GetComponent<GameState>();
	    diningExit = GameObject.FindGameObjectWithTag("DishwasherDiningExit");
        dishExit = GameObject.FindGameObjectWithTag("DishwasherDishExit");
        dishwasherBusPosition = GameObject.Find("DishwasherBusPosition");
	    busPosition = GameObject.Find("BusPosition");
        diningPosition = GameObject.FindGameObjectWithTag("DishwasherDiningPosition");

        SetState(DishwasherState.Idle);

	    //GetComponent<Animator>().Play("Throw Dish");
        //GetComponent<Animator>().Play("Idle");
	}
	
	// Update is called once per frame
    private void Update()
    {
        switch (currentState)
        {
            case DishwasherState.Idle:
                break;

            case DishwasherState.MoveToDiningExit:
                gameObject.transform.eulerAngles = new Vector3(0, -180, 0);

                // Off screen.
                if (gameObject.transform.position.x <= diningExit.transform.position.x)
                {
                    gameState.SetState(GameState.State.Dish);
                    transform.position = dishExit.transform.position;
                }
                // Move off screen.
                transform.position = Vector2.MoveTowards(transform.position, diningExit.transform.position,
                    speed*Time.deltaTime);
                break;

            case DishwasherState.MoveToBusPosition:
                //gameObject.transform.eulerAngles = new Vector3(0, -180, 0);

                if (gameObject.transform.position.x <= dishwasherBusPosition.transform.position.x) {
                    SetState(DishwasherState.MoveToDishExit);
                    EnterMoveToDishExitState();
                }

                transform.position = Vector2.MoveTowards(transform.position, dishwasherBusPosition.transform.position,
                    speed * Time.deltaTime);
                break;

            case DishwasherState.MoveToDishExit:
                gameObject.transform.eulerAngles = new Vector3(0, 0, 0);

                if (gameObject.transform.position.x >= dishExit.transform.position.x) {
                    gameState.SetState(GameState.State.Dining);
                    
                    SetState(DishwasherState.Idle);
                }

                // Move Dishwasher and bus toward dining room.
                transform.position = Vector2.MoveTowards(transform.position, dishExit.transform.position,
                    speed * Time.deltaTime);
                bus.transform.position = Vector2.MoveTowards(bus.transform.position, 
                    new Vector2(15, bus.transform.position.y),
                    speed * Time.deltaTime);
                break;

            case DishwasherState.MoveToDiningPosition:
                gameObject.transform.eulerAngles = new Vector3(0, 0, 0);

                if (!spriteRenderer.isVisible) {
                    gameState.SetState(GameState.State.Dining);
                }
                transform.position = Vector2.MoveTowards(transform.position, diningPosition.transform.position,
                    speed * Time.deltaTime);
                break;

           
        }
    }

    void EnterMoveToDishExitState()
    {
        // Set bus position.
        bus.transform.position = new Vector2(bus.transform.position.x + 7, bus.transform.position.y);
    }

    public void SetState(DishwasherState state)
    {
        currentState = state;

        if (currentState == DishwasherState.Idle)
        {
            EnterDiningState();
        } 
        else if (currentState == DishwasherState.MoveToBusPosition)
        {
            bus.transform.position = busPosition.transform.position;
        }
    }

    void EnterDiningState()
    {
        gameObject.transform.position = diningPosition.transform.position;
    }








}
