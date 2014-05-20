using UnityEngine;
using System.Collections;

public class CursorManager : MonoBehaviour
{
    public Texture2D crosshairTextureOff;
    public Texture2D crosshairTextureOn;
    private Texture2D currentCrosshairTexture;

    int cursorSizeX = 50;  // set to width of your cursor texture
    int cursorSizeY = 50;  // set to height of your cursor texture

	// Use this for initialization
	void Start ()
	{
	    Screen.showCursor = false;
	}
	
	// Update is called once per frame
	void Update ()
	{   
        
	}

    void OnGUI() {
        GUI.DrawTexture(new Rect(Input.mousePosition.x, // - cursorSizeX / 2,// + cursorSizeX / 2,
            (Screen.height - Input.mousePosition.y) - cursorSizeY/2 - 15, // + cursorSizeY / 2, 
            cursorSizeX, cursorSizeY),
            currentCrosshairTexture);
        
    }

    public void SetCursorNormal()
    {
        currentCrosshairTexture = crosshairTextureOff;
    }

    public void SetCursorHover()
    {
        currentCrosshairTexture = crosshairTextureOn;
    }

    public void HideCursor()
    {
        currentCrosshairTexture = null;
    }

    public void ShowCursor()
    {
        currentCrosshairTexture = crosshairTextureOff;
    }
}
