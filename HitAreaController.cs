using UnityEngine;
using System.Collections;

public class HitAreaController : MonoBehaviour
{
    private GameState gameState;
	// Use this for initialization
	void Start ()
	{
	    gameState = GameObject.Find("GameState").GetComponent<GameState>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnMouseOver()
    {
        gameState.SetCursorInHitArea(true);
        gameState.SetCursorHover();
    }

    private void OnMouseExit()
    {
        gameState.SetCursorInHitArea(false);
        gameState.SetCursorNormal();
    }

    private void OnMouseDown()
    {
        gameState.SetClickedInHitArea(true);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Customer")
        {
            collider.gameObject.transform.parent.transform.parent.GetComponent<CustomerManager>().SetCustomerInHitArea(true);
        }
        else if (collider.gameObject.tag == "Dish")
        {
            collider.gameObject.GetComponent<DishManager>().SetDishInHitArea(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Customer") 
        {
            collider.gameObject.transform.parent.transform.parent.GetComponent<CustomerManager>().SetCustomerInHitArea(false);
        }
    }
}
