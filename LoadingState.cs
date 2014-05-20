using UnityEngine;
using System.Collections;

public class LoadingState : MonoBehaviour
{
    private float progress = 0;
    private GameObject progressText;

	// Use this for initialization
	void Start ()
	{
	    progressText = GameObject.Find("ProgressText");
        progressText.guiText.text = progress.ToString();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    progress = Application.GetStreamProgressForLevel("The Dishwasher");
	    if (progress == 1)
	    {
	        Application.LoadLevel("The Dishwasher");
	    }
	    else
	    {
	        progressText.guiText.text = progress.ToString();
	    }
	}
}
