
using UnityEngine;
using System.Collections;

public class CustomerManager : MonoBehaviour {
    private GameState gameState;
    public Vector2 endpoint;
    public Vector2 cursorTarget;
    private Camera camera;
    public CustomerStates currentState;
    private float moveSpeed = 5f;
    public Sprite[] sprites;

    public enum CustomerStates
    {
        Enter = 0,
        Idle = 1,
        Exit = 2,
        Hit = 3,
        Die = 4
    }

	// Use this for initialization
	void Start ()
	{
        gameState = GameObject.FindGameObjectWithTag("GameState").GetComponent<GameState>();
	    camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        
        // Randomize and set sprite.
	    int index = Random.Range(0, sprites.Length);
	    GetComponent<SpriteRenderer>().sprite = sprites[index];
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.position.x == endpoint.x && transform.position.y == endpoint.y)
	    {
	        SetState(CustomerStates.Exit);
	    }

        switch (currentState) {
            case CustomerStates.Enter:
                transform.position = Vector2.MoveTowards(transform.position, endpoint, moveSpeed * Time.deltaTime);
                break;

            case CustomerStates.Idle:
                break;

            case CustomerStates.Exit:
                // Off screen.
                Destroy(gameObject);
                gameState.CustomerLeft();
                break;

            case CustomerStates.Hit:
                // Explode sprite.
                SetState(CustomerStates.Die);
                break;

            case CustomerStates.Die:
                Destroy(gameObject);
                gameState.KilledCustomer(gameObject.tag);
                break;
        }
	}

    public void SetState(CustomerStates state)
    {
        this.currentState = state;
    }

    public void SetEndpoint(Vector2 endpoint)
    {
        this.endpoint = endpoint;
    }

    void OnMouseDown()
    {
        if (gameState.GetCurrentState() == GameState.State.Dining)
        {
            //Debug.Log("Clicked on customer");
            // Left click.
            if (Input.GetMouseButton(0))
            {
                //Debug.Log("FIRE DISH!!");
                //cursorTarget = camera.ScreenToWorldPoint(Input.mousePosition);
                //gameState.FireDish(cursorTarget);
            }
        }
    }



}
