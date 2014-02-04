using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;
using System.Collections;

public class PlateManager : MonoBehaviour
{
    private GameState gameState;
    private float moveSpeed = 50f;
    private Vector2 targetPosition;
    public PlateStates currentState;
    private bool hitCustomer = false;

    public enum PlateStates
    {
        Throw = 0,
        Hit = 1,
        Destroy = 2
    }

    // Use this for initialization
    private void Start()
    {
        gameState = GameObject.FindGameObjectWithTag("GameState").GetComponent<GameState>();
    }

    // Update is called once per frame
    private void Update()
    {
        switch (currentState)
        {
            case PlateStates.Throw:
                //transform.position = Vector2.Lerp(transform.targetPosition, targetPosition, moveSpeed);
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                if (transform.position.x == targetPosition.x && transform.position.y == targetPosition.y)
                {
                    ChangeState(PlateStates.Hit);
                }
                break;

            case PlateStates.Hit:
                //Debug.Log("Hit!");
                ChangeState(PlateStates.Destroy);
                break;

            case PlateStates.Destroy:
                Destroy(gameObject);
                break;
        }
    }

    public void SetTarget(Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!hitCustomer && (Vector2)gameObject.transform.position == targetPosition)
        {
            if (collider.gameObject.tag == "Customer1" ||
                collider.gameObject.tag == "Customer2" ||
                collider.gameObject.tag == "Customer3" ||
                collider.gameObject.tag == "Customer4")
            {
                hitCustomer = true;
                ChangeState(PlateStates.Hit);
                gameState.HitCustomer(collider.gameObject);
            }
        }
    }


    public void ChangeState(PlateStates state)
    {
        currentState = state;
        Debug.Log("Changed state to " + currentState.ToString());
    }
}
