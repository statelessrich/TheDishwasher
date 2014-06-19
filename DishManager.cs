using UnityEngine;

public class DishManager : MonoBehaviour
{
    public GameState gameState;
    private float moveSpeed = 10f;

    private Vector2 endPosition;
    private Vector2 clickedPosition;

    private DishStates currentState;
    private bool hitCustomer = false;

    private GameObject customerHit;

    private bool inHitArea;

    public enum DishStates
    {
        Throw,
        Collide,
        Hit,
        Destroy
    }

    // Use this for initialization
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        switch (currentState)
        {
            case DishStates.Throw:
                // Move dish toward end position.
                transform.position = Vector2.MoveTowards(transform.position, endPosition, moveSpeed * Time.deltaTime);

                // Dish reached clicked position.
                if (transform.position.x == clickedPosition.x && transform.position.y == clickedPosition.y)
                {
                    /* Register customer collision if:
                     * 1. Dish is currently colliding with customer.
                     * 2. Customer is inside hit area.
                     * 3. Player clicked inside hit area.
                     */
                    if (hitCustomer && customerHit.GetComponent<CustomerManager>().GetCustomerInHitArea() && gameState.GetClickedInHitArea())
                    {
                        gameState.HitCustomer(customerHit);
                        gameState.SetClickedInHitArea(false);
                        ChangeState(DishStates.Hit);
                    }
                    else 
                    {
                        // Dish missed.
                        ChangeState(DishStates.Destroy);
                    }
                }
                break;

            case DishStates.Hit:
                ChangeState(DishStates.Destroy);
                break;

            case DishStates.Destroy:
                Destroy(gameObject);
                break;
        }
    }

    private void SetEndPosition(Vector2 endPosition)
    {
        // Increase x and y so dish flies off screen.
        //endPosition.x *= 5;
        //endPosition.y *= 2;
        this.endPosition = endPosition;
    }

    public void SetClickedPosition(Vector2 clickedPosition)
    {
        this.clickedPosition = clickedPosition;
        SetEndPosition(clickedPosition);
    }
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!hitCustomer) {
            if (collider.gameObject.tag.Contains("Customer")) {
                hitCustomer = true;
                customerHit = collider.gameObject.transform.parent.gameObject.transform.parent.gameObject;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag.Contains("Customer"))
        {
            hitCustomer = false;
        }
    }

    public void ChangeState(DishStates state)
    {
        currentState = state;
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

    public void SetDishInHitArea(bool inHitArea)
    {
        this.inHitArea = inHitArea;
    }
}
