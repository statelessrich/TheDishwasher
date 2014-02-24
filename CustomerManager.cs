
using UnityEngine;
using System.Collections;

public class CustomerManager : MonoBehaviour {
    private GameState gameState;

    public Vector2 positionMid;
    public Vector2 positionEnd;
    public Vector2 currentTargetPosition;

    public Vector2 cursorTarget;
    public CustomerStates currentState;
    private float moveSpeed = 3f;

    public enum CustomerStates
    {
        Enter = 0,
        MoveToMid = 1,
        MoveToEnd = 2,
        Exit = 3,
        Hit = 4,
        Die = 5
    }

	// Use this for initialization
	void Start ()
	{
        gameState = GameObject.FindGameObjectWithTag("GameState").GetComponent<GameState>();

        // Randomize and set sprite.
	    //int index = Random.Range(0, sprites.Length);
	    //GetComponent<SpriteRenderer>().sprite = sprites[index];
	}
	
	// Update is called once per frame
	void Update () {

        switch (currentState) {
            case CustomerStates.MoveToMid:
                // Check if reached midpoint.
                if (transform.position.x == positionMid.x && transform.position.y == positionMid.y) {
                    SetState(CustomerStates.MoveToEnd);
                }
                transform.position = Vector2.MoveTowards(transform.position, positionMid, moveSpeed * Time.deltaTime);
                break;

            case CustomerStates.MoveToEnd:
                // Check if reached endpoint.
                if (transform.position.x == positionEnd.x && transform.position.y == positionEnd.y) {
                    SetState(CustomerStates.Exit);
                }
                transform.position = Vector2.MoveTowards(transform.position, positionEnd, moveSpeed * Time.deltaTime);
                break;

            case CustomerStates.Exit:
                Destroy(gameObject);
                gameState.CustomerLeft();
                break;

            case CustomerStates.Hit:
                // Explode sprite.
                SetState(CustomerStates.Die);
                break;

            case CustomerStates.Die:
                Debug.Log("-----------------die");
                Destroy(gameObject);
                gameState.KilledCustomer(gameObject.tag);
                break;
        }
	}

    public void SetState(CustomerStates state)
    {
        this.currentState = state;
    }

    public void SetPositionMid(Vector2 positionMid)
    {
        this.positionMid = positionMid;
    }

    public void SetPositionEnd(Vector2 positionEnd)
    {
        this.positionEnd = positionEnd;
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
