using UnityEngine;
using System.Collections;

public class LoadScreenGUIScript : MonoBehaviour
{
	public Vector2 sizeFindScale;
	public float scaleX;
	public float scaleY;

	public Texture backgroundTexture;
	public Texture companyLogo;
	//public Texture unityLogo;
	public Vector2 companyLogoPosition;
	public Vector2 companyLogoSize;
	//public Vector2 unityLogoPosition = new Vector2(433, 100);
	//public Vector2 unityLogoSize = new Vector2(500, 260);
	public string companyName;
	public Vector2 namePosition;
	public Vector2 nameSize;
	public GUIStyle nameStyle;

	public Vector2 resizeInstructionsPosition;
	public Vector2 resizeInstructionsSize;
	public GUIStyle instructionsStyle;

	public Vector2 resizeSquarePosition;
	public Vector2 resizeSquareSize;
	public Texture resizeSquareTex;

	public Vector2 standardResPosition;
	public Vector2 standardResSize;

	public Vector2 currentResPosition;
	public Vector2 currentResSize;

	public Vector2 continueButtonPosition;
	public Vector2 continueButtonSize;
	public GUIStyle continueButtonStyle;

	private string sectionOfStart = "";
	private float previousChangeTime;
	public float timeBetweenChanges;
	
	void Start()
	{
		Screen.showCursor = false;
		LoadStartSection();
		previousChangeTime = Time.time;
	}

	void Update()
	{
		if(Time.time >= (previousChangeTime + timeBetweenChanges))
		{
			if(sectionOfStart != "resizeSquare")
			{
				LoadStartSection();
				previousChangeTime = Time.time;
			}
		}
	}

	void LoadStartSection()
	{
		if(sectionOfStart == "")
		{
			sectionOfStart = "logoAndName";
		}
		else if(sectionOfStart == "logoAndName")
		{
			//sectionOfStart = "unityLogo";
			sectionOfStart = "resizeSquare";
			Screen.lockCursor = false;
			Screen.showCursor = true;
		}
		/*else if(sectionOfStart == "unityLogo")
		{
			Screen.showCursor = true;
			sectionOfStart = "resizeSquare";
		}*/
		else if(sectionOfStart == "resizeSquare")
		{
			//Screen.showCursor = true;
			Application.LoadLevel("Start Menu");
		}
	}

	void OnGUI()
	{
		scaleX = Screen.width / sizeFindScale.x;
		scaleY = Screen.height / sizeFindScale.y;

		GUI.DrawTexture(new Rect(0, 0, 1366 * scaleX, 598 * scaleY), backgroundTexture);

		if(sectionOfStart == "logoAndName")
		{
			GUI.DrawTexture(new Rect(companyLogoPosition.x * scaleX, companyLogoPosition.y * scaleY, companyLogoSize.x * scaleX, companyLogoSize.y * scaleY), companyLogo);
			GUI.Label(new Rect(namePosition.x * scaleX, namePosition.y * scaleY, nameSize.x * scaleX, nameSize.y * scaleY), companyName, nameStyle);
		}
		/*if(sectionOfStart == "unityLogo")
		{
			GUI.DrawTexture(new Rect(unityLogoPosition.x * scaleX, unityLogoPosition.y * scaleY, unityLogoSize.x * scaleX, unityLogoSize.y * scaleY), unityLogo);
			GUI.Label(new Rect(namePosition.x * scaleX, namePosition.y * scaleY, nameSize.x * scaleX, nameSize.y * scaleY), "Powered By Unity", nameStyle);
		}*/
		if(sectionOfStart == "resizeSquare")
		{
			GUI.Label(new Rect(resizeInstructionsPosition.x * scaleX, resizeInstructionsPosition.y * scaleY, resizeInstructionsSize.x * scaleX, resizeInstructionsSize.y * scaleY), "Please resize the window until this is a square.", instructionsStyle);
			GUI.DrawTexture(new Rect(resizeSquarePosition.x * scaleX, resizeSquarePosition.y * scaleY, resizeSquareSize.x * scaleX, resizeSquareSize.y * scaleY), resizeSquareTex);

			GUI.Label(new Rect(standardResPosition.x * scaleX, standardResPosition.y * scaleY, standardResSize.x * scaleX, standardResSize.y * scaleY), "Standard Res: " + sizeFindScale.x + " x " + sizeFindScale.y, instructionsStyle);
			GUI.Label(new Rect(currentResPosition.x * scaleX, currentResPosition.y * scaleY, currentResSize.x * scaleX, currentResSize.y * scaleY), "Current Res: " + Screen.width + " x " + Screen.height, instructionsStyle);

			if(GUI.Button(new Rect(continueButtonPosition.x * scaleX, continueButtonPosition.y * scaleY, continueButtonSize.x * scaleX, continueButtonSize.y * scaleY), "Continue", continueButtonStyle))
			{
				LoadStartSection();
			}
		}
	}
}
