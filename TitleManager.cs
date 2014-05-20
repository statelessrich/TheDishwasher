using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using System.Collections;

public class TitleManager : MonoBehaviour
{
    // Cameras.
    private Camera mainCamera;
    private Camera creditsCamera;
    private Camera tutorialCamera;
    private Camera splashCamera;

    private bool displayingSplash;

    // Managers.
    private CursorManager cursorManager;

    // State.
    private TitleState currentTitleState;
    private GameState gameState;

    // Credits screen stuff
    private float creditsExplosionCooldown = -1;
    private float creditsExplosionCooldownMax = .2f;

    public GameObject explosion;
    private GameObject explosionInstance;
    private bool waiting = false;
    private int explosionCount;
    private int maxExplosions = 20;

    private bool debug = false;

    public enum TitleState
    {
        Splash,
        Title,
        Credits,
        EndCredits,
        Tutorial,
        Play
    }

    // TODO: Refactor.
    // Flag to prevent button clicks in Update() from being processed right after OnMouseDown().
    private bool clicked;

	// Use this for initialization
	void Start ()
	{
        // Cameras.
	    creditsCamera = GameObject.Find("CreditsCamera").GetComponent<Camera>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
	    tutorialCamera = GameObject.Find("TutorialCamera").GetComponent<Camera>();
	    splashCamera = GameObject.Find("SplashCamera").GetComponent<Camera>();

	    gameState = GameObject.Find("GameState").GetComponent<GameState>();

	    gameState.HideCursor();
        clicked = false;
	
	    if (debug)
	    {
            ChangeState(TitleState.EndCredits);
	    }
	}
	// Update is called once per frame
	
    void Update () {
	    if (!clicked)
	    {
	        switch (currentTitleState)
	        {
	            case TitleState.Splash:
	                if (!displayingSplash)
	                {
	                    displayingSplash = true;
	                    StartCoroutine(WaitAndDisplayTitle());
	                }
	                break;

	            case TitleState.Credits:
	                // Left click or ESC to return to title screen.
	                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Escape))
	                {
	                    DisplayTitle();
	                }
	                break;

                case TitleState.EndCredits:
                    if (Input.GetKeyDown(KeyCode.Escape) || (explosionCount == maxExplosions))
                    {
                        // Destroy all the instantiated explosions on the credits screen.
                        foreach (GameObject explosion in GameObject.FindGameObjectsWithTag("Explosion")) 
                        {
                            Destroy(explosion);
                        }
                        // Return to title screen.
	                    DisplayTitle();
	                }
	                if (!waiting)
	                {
	                    if (creditsExplosionCooldown <= 0)
	                    {
	                        // Instantiate explosion at random position and play animation/SFX.
	                        int randomX = Random.Range(-74, -60);
	                        int randomY = Random.Range(3, 7);
	                        Vector2 explosionPosition = new Vector2(randomX, randomY);
	                        explosionInstance = Instantiate(explosion, explosionPosition, gameObject.transform.rotation) as GameObject;
	                        explosionInstance.SetActive(true);
	                        explosionInstance.GetComponent<Animator>().Play("Explosion2");
	                        gameState.PlayExplosionSFX();
	                        explosionCount++;
	                        // Reset cooldown.
	                        creditsExplosionCooldown = creditsExplosionCooldownMax;
	                    }
	                    else if (creditsExplosionCooldown > 0)
	                    {
	                        creditsExplosionCooldown -= Time.deltaTime;
	                    }
	                }
	                break;

	            case TitleState.Tutorial:
	                // Left click to start the game. There is also a 10 second timer.
	                if (Input.GetMouseButtonDown(0))
	                {
	                    StartGame();
	                }
	                break;
	        }
	    }
	    else
	    {
	        clicked = false;
	    }
	}

    public void ChangeState(TitleState state)
    {
        currentTitleState = state;
        if (currentTitleState == TitleState.EndCredits)
        {
            EnterEndCreditsState();
        }
    }

    private void EnterEndCreditsState()
    {
        mainCamera.enabled = false;
        creditsCamera.enabled = true;
        waiting = true;
        StartCoroutine(WaitAndDisplayExplosions());
    }

    private IEnumerator WaitAndDisplayExplosions()
    {
        yield return new WaitForSeconds(5);
        creditsExplosionCooldown = creditsExplosionCooldownMax;
        waiting = false;
    }

    private IEnumerator WaitAndDisplayTitle()
    {
        yield return new WaitForSeconds(5);
        DisplayTitle();
    }

    public void DisplayTitle()
    {
        ChangeState(TitleState.Title);
        mainCamera.enabled = true;
        splashCamera.enabled = false;
        creditsCamera.enabled = false;

        gameState.ShowCursor();
    }

    public void DisplayTutorial()
    {
        ChangeState(TitleState.Tutorial);
        PlayMenuSFX();
        mainCamera.enabled = false;
        tutorialCamera.enabled = true;
        StartCoroutine(WaitAndStartGame());
    }

    public void DisplayCredits()
    {
        ChangeState(TitleState.Credits);
        PlayMenuSFX();
        mainCamera.enabled = false;
        creditsCamera.enabled = true;
    }

    private IEnumerator WaitAndStartGame()
    {
        yield return new WaitForSeconds(10);
        if (gameState.GetCurrentState() == GameState.State.Title)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        ChangeState(TitleState.Play);
        tutorialCamera.enabled = false;
        gameState.ResetGame();
    }

    public void Clicked() 
    {
        clicked = true;
    }

    public void PlayMenuSFX() 
    {
        gameState.PlayMenuSFX();
    }
}
