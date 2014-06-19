
using UnityEngine;
using System.Collections;

public class CustomerManager : MonoBehaviour {
    public GameState gameState;

    public Vector2 positionMid;
    public Vector2 positionEnd;
    public Vector2 currentTargetPosition;

    public Vector2 cursorTarget;
    public CustomerStates currentState;
    public float moveSpeed = 3f;//3f;

    public GameObject explosion;
    private GameObject explosionInstance;

    private int explosionIdleState;
    private AnimatorStateInfo currentAnimatorState;

    public bool inHitArea;

    public enum CustomerStates
    {
        Enter,
        MoveToMid,
        MoveToEnd,
        Exit,
        Hit,
        Die
    }

	// Use this for initialization
	void Start ()
	{
        // Get idle animation name hash.
	    explosionIdleState = Animator.StringToHash("Base.Idle");
	}
	
	// Update is called once per frame
	void Update () {
        switch (currentState) {
            case CustomerStates.MoveToMid:
                // Check if reached midpoint.
                // TODO: Refactor to use Vector3.Distance or something.
                if (transform.position.x == positionMid.x && transform.position.y == positionMid.y) 
                {
                    SetState(CustomerStates.MoveToEnd);
                }
                transform.position = Vector2.MoveTowards(transform.position, positionMid, moveSpeed * Time.deltaTime);
                break;

            case CustomerStates.MoveToEnd:
                // Check if reached endpoint.
                // TODO: Refactor to use Vector3.Distance or something.
                if (transform.position.x == positionEnd.x && transform.position.y == positionEnd.y) 
                {
                    SetState(CustomerStates.Exit);
                }
                transform.position = Vector2.MoveTowards(transform.position, positionEnd, moveSpeed * Time.deltaTime);
                break;

            case CustomerStates.Exit:
                Destroy(gameObject);
                gameState.CustomerLeft();
                break;

            case CustomerStates.Hit:
                // Get current animator state. This will be either idle, explosion1, or explosion2.
                currentAnimatorState = explosionInstance.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
                // Compare current animator state to idle state. If they match, animation returned to idle state.
                if (currentAnimatorState.nameHash == explosionIdleState) {
                    Destroy(explosionInstance);
                    SetState(CustomerStates.Die);
                }
                break;

            case CustomerStates.Die:
                gameState.KilledCustomer();
                Destroy(gameObject);
                break;
        }
	}

    public void SetState(CustomerStates state)
    {
        currentState = state;
        
        switch (state)
        {
            case CustomerStates.Hit:
                // Disable customer sprite.
                gameObject.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                
                // Instantiate explosion and play animation.
                Vector2 explosionPosition = gameObject.transform.position;
                explosionPosition.y += 3;
                explosionInstance = Instantiate(explosion, explosionPosition, gameObject.transform.rotation) as GameObject;
                explosionInstance.SetActive(true);
                explosionInstance.GetComponent<Animator>().Play("Explosion2");
                gameState.PlayExplosionSFX();
                break;
        }
    }

    public void SetPositionMid(Vector2 positionMid)
    {
        this.positionMid = positionMid;
    }

    public void SetPositionEnd(Vector2 positionEnd)
    {
        this.positionEnd = positionEnd;
    }

    void OnMouseOver()
    {
        if (inHitArea)
        {
            gameState.SetCursorHover();
        }
    }
    void OnMouseExit() 
    {
        if (!inHitArea)
        {
            gameState.SetCursorNormal();
        }
    }

    public void SetCustomerInHitArea(bool inHitArea)
    {
        this.inHitArea = inHitArea;
    }

    public bool GetCustomerInHitArea()
    {
        return inHitArea;
    }
}
