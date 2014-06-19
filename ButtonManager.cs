using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public TitleManager titleManager;
    public CursorManager cursorManager;
    public GameState gameState;

	// Use this for initialization
	void Start ()
	{

	}

    void OnMouseDown()
    {
        switch (name)
        {
            // Title screen Play button.
            case "PlayButton":
                titleManager.DisplayTutorial();
                titleManager.Clicked();
                break;

            // Title screen Credits button.
            case "CreditsButton":
                titleManager.DisplayCredits();
                titleManager.Clicked();
                break;

            // Pause menu Resume button.
            case "ResumeButton":
                gameState.PlayMenuSFX();
                gameState.UnpauseGame();
                break;

            // Pause menu Exit button.
            case "ExitButton":
                gameState.ExitToTitle();
                break;
        }
    }

    void OnMouseOver()
    {
        // Red cursor on button hover.
        cursorManager.SetCursorHover();
    }

    void OnMouseExit()
    {
        // White cursor when leaving button.
        cursorManager.SetCursorNormal();
    }
}
