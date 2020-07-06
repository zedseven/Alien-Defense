using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuScript : MonoBehaviour
{
	public Vector2 sizeFindScale;
	public float scaleX;
	public float scaleY;

	public bool displayingArmour = false;
	public bool displayingMaps = false;
	private string currentPage = "startPage";

	public Transform armourCamera;
	public Transform mapCamera;
	public float switchArmourSpeed;
	public float switchMapSpeed;
	private bool canSwitchArmour;
	private bool canSwitchMaps;

	public List<Transform> menuArmours;
	public List<Transform> menuMaps;
	public int currentArmour = 0;
	public int currentMap = 0;

	public Texture splashScreen;

	public Vector2 startButtonPosition;
	public Vector2 startButtonSize;
	public Vector2 helpButtonPosition;
	public Vector2 helpButtonSize;
	public Vector2 creditsButtonPosition;
	public Vector2 creditsButtonSize;
	public Vector2 quitButtonPosition;
	public Vector2 quitButtonSize;
	public Vector2 pageTitlePosition;
	public Vector2 pageTitleSize;
	public Vector2 armourTitlePosition;
	public Vector2 armourTitleSize;
	public Vector2 descriptionPosition;
	public Vector2 descriptionSize;
	public Vector2 statBoxSize;
	public Vector2 statBox1Position;
	public Vector2 statBox2Position;
	public Vector2 selectArmourPosition;
	public Vector2 selectArmourSize;
	public Vector2 backButtonPosition;
	public Vector2 backButtonSize;
	public Vector2 helpBoxPosition;
	public Vector2 helpBoxSize;
	public float helpBoxFullSize;
	public Vector2 creditBoxSize;
	public Vector2 credit1Position;
	public Vector2 credit2Position;
	public Vector2 credit3Position;
	public Vector2 credit4Position;
	public Vector2 credit5Position;

	public TextAsset helpText;
	//public string helpInfo;
	public string creditInfo1;
	public string creditInfo2;
	public string creditInfo3;
	public string creditInfo4;
	public string creditInfo5;
	
	public GUIStyle standardButtonStyle;
	public GUIStyle standardLabelStyle;
	public GUIStyle helpTextStyle;
	public GUIStyle pageTitleStyle;
	public GUIStyle armourTitleStyle;
	public GUIStyle creditStyle;

	void Update()
	{
		if(displayingArmour == true)
		{
			armourCamera.camera.enabled = true;
			mapCamera.camera.enabled = false;
		}
		else if(displayingMaps == true)
		{
			armourCamera.camera.enabled = false;
			mapCamera.camera.enabled = true;
		}

		if(displayingArmour == true && canSwitchArmour == true)
		{
			if(Input.GetAxis("Horizontal") > 0)
			{
				if((menuArmours.Count - 1) > currentArmour)
				{
					currentArmour++;
					canSwitchArmour = false;
				}
			}
			else if(Input.GetAxis("Horizontal") < 0)
			{
				if(currentArmour > 0)
				{
					currentArmour--;
					canSwitchArmour = false;
				}
			}
		}

		if(displayingMaps == true && canSwitchMaps == true)
		{
			if(Input.GetAxis("Horizontal") > 0)
			{
				if((menuMaps.Count - 1) > currentMap)
				{
					currentMap++;
					canSwitchMaps = false;
				}
			}
			else if(Input.GetAxis("Horizontal") < 0)
			{
				if(currentMap > 0)
				{
					currentMap--;
					canSwitchMaps = false;
				}
			}
		}

		if(displayingArmour == true)
		{
			if(armourCamera.position.x != menuArmours[currentArmour].position.x)
			{
				armourCamera.position = Vector3.MoveTowards(armourCamera.position, new Vector3(menuArmours[currentArmour].position.x, armourCamera.position.y, armourCamera.position.z), switchArmourSpeed * Time.deltaTime);
			}
			else
			{
				canSwitchArmour = true;
			}
		}
		else if(displayingMaps == true)
		{
			if(mapCamera.position.x != menuMaps[currentMap].position.x)
			{
				mapCamera.position = Vector3.MoveTowards(mapCamera.position, new Vector3(menuMaps[currentMap].position.x, mapCamera.position.y, mapCamera.position.z), switchMapSpeed * Time.deltaTime);
			}
			else
			{
				canSwitchMaps = true;
			}
		}
	}

	void OnGUI()
	{
		scaleX = Screen.width / sizeFindScale.x;
		scaleY = Screen.height / sizeFindScale.y;

		if(currentPage == "startPage")
		{
			if(splashScreen != null)
			{
				GUI.DrawTexture(new Rect(0, 0, sizeFindScale.x * scaleX, sizeFindScale.y * scaleY), splashScreen);
			}
			GUI.Label(new Rect(pageTitlePosition.x * scaleX, pageTitlePosition.y * scaleY, pageTitleSize.x * scaleX, pageTitleSize.y * scaleY), "Invasion A.", pageTitleStyle);
			if(GUI.Button(new Rect(startButtonPosition.x * scaleX, startButtonPosition.y * scaleY, startButtonSize.x * scaleX, startButtonSize.y * scaleY), "Start", standardButtonStyle))
			{
				displayingArmour = true;
				currentPage = "pickArmour";
			}
			else if(GUI.Button(new Rect(helpButtonPosition.x * scaleX, helpButtonPosition.y * scaleY, helpButtonSize.x * scaleX, helpButtonSize.y * scaleY), "Help", standardButtonStyle))
			{
				currentPage = "help";
			}
			else if(GUI.Button(new Rect(creditsButtonPosition.x * scaleX, creditsButtonPosition.y * scaleY, creditsButtonSize.x * scaleX, creditsButtonSize.y * scaleY), "Credits", standardButtonStyle))
			{
				currentPage = "credits";
			}
			else if(GUI.Button(new Rect(quitButtonPosition.x * scaleX, quitButtonPosition.y * scaleY, quitButtonSize.x * scaleX, quitButtonSize.y * scaleY), "Quit", standardButtonStyle))
			{
				Application.Quit();
			}
		}
		else if(currentPage == "help")
		{
			GUI.Label(new Rect(helpBoxPosition.x * scaleX, helpBoxPosition.y * scaleY, helpBoxSize.x * scaleX, helpBoxSize.y * scaleY), helpText.ToString(), helpTextStyle);
			if(GUI.Button(new Rect(backButtonPosition.x * scaleX, backButtonPosition.y * scaleY, backButtonSize.x * scaleX, backButtonSize.y * scaleY), "Back", standardButtonStyle))
			{
				displayingArmour = false;
				currentPage = "startPage";
			}
		}
		else if(currentPage == "pickArmour")
		{
			GUI.Label(new Rect(pageTitlePosition.x * scaleX, pageTitlePosition.y * scaleY, pageTitleSize.x * scaleX, pageTitleSize.y * scaleY), "Armour", pageTitleStyle);
			GUI.Label(new Rect(armourTitlePosition.x * scaleX, armourTitlePosition.y * scaleY, armourTitleSize.x * scaleX, armourTitleSize.y * scaleY), menuArmours[currentArmour].GetComponent<MenuArmourScript>().armourName, armourTitleStyle);
			GUI.Label(new Rect(descriptionPosition.x * scaleX, descriptionPosition.y * scaleY, descriptionSize.x * scaleX, descriptionSize.y * scaleY), menuArmours[currentArmour].GetComponent<MenuArmourScript>().armourDescription, standardLabelStyle);
			GUI.Label(new Rect(statBox1Position.x * scaleX, statBox1Position.y * scaleY, statBoxSize.x * scaleX, statBoxSize.y * scaleY), "Defense: " + menuArmours[currentArmour].GetComponent<MenuArmourScript>().menuDefense, standardLabelStyle);
			GUI.Label(new Rect(statBox2Position.x * scaleX, statBox2Position.y * scaleY, statBoxSize.x * scaleX, statBoxSize.y * scaleY), "Weight: " + menuArmours[currentArmour].GetComponent<MenuArmourScript>().menuSpeed, standardLabelStyle);

			if(GUI.Button(new Rect(selectArmourPosition.x * scaleX, selectArmourPosition.y * scaleY, selectArmourSize.x * scaleX, selectArmourSize.y * scaleY), "Select", standardButtonStyle))
			{
				PlayerPrefs.SetInt("ArmourType", currentArmour);
				displayingArmour = false;
				displayingMaps = true;
				currentPage = "pickMap";
			}
			else if(GUI.Button(new Rect(backButtonPosition.x * scaleX, backButtonPosition.y * scaleY, backButtonSize.x * scaleX, backButtonSize.y * scaleY), "Back", standardButtonStyle))
			{
				displayingArmour = false;
				currentPage = "startPage";
			}
		}
		else if(currentPage == "pickMap")
		{
			GUI.Label(new Rect(pageTitlePosition.x * scaleX, pageTitlePosition.y * scaleY, pageTitleSize.x * scaleX, pageTitleSize.y * scaleY), "Maps", pageTitleStyle);
			GUI.Label(new Rect(armourTitlePosition.x * scaleX, armourTitlePosition.y * scaleY, armourTitleSize.x * scaleX, armourTitleSize.y * scaleY), menuMaps[currentMap].GetComponent<MenuMapScript>().mapName, armourTitleStyle);
			GUI.Label(new Rect(descriptionPosition.x * scaleX, descriptionPosition.y * scaleY, descriptionSize.x * scaleX, descriptionSize.y * scaleY), menuMaps[currentMap].GetComponent<MenuMapScript>().mapDescription, standardLabelStyle);
			GUI.Label(new Rect(statBox1Position.x * scaleX, statBox1Position.y * scaleY, statBoxSize.x * scaleX, statBoxSize.y * scaleY), "Difficulty: " + menuMaps[currentMap].GetComponent<MenuMapScript>().mapDifficulty, standardLabelStyle);

			if(GUI.Button(new Rect(selectArmourPosition.x * scaleX, selectArmourPosition.y * scaleY, selectArmourSize.x * scaleX, selectArmourSize.y * scaleY), "Select", standardButtonStyle))
			{
				Application.LoadLevel(menuMaps[currentMap].GetComponent<MenuMapScript>().levelIndex);
			}
			else if(GUI.Button(new Rect(backButtonPosition.x * scaleX, backButtonPosition.y * scaleY, backButtonSize.x * scaleX, backButtonSize.y * scaleY), "Back", standardButtonStyle))
			{
				displayingArmour = true;
				displayingMaps = false;
				currentPage = "pickArmour";
			}
		}
		else if(currentPage == "credits")
		{
			GUI.Label(new Rect(pageTitlePosition.x * scaleX, pageTitlePosition.y * scaleY, pageTitleSize.x * scaleX, pageTitleSize.y * scaleY), "Credits", pageTitleStyle);
			GUI.Label(new Rect(credit1Position.x * scaleX, credit1Position.y * scaleY, creditBoxSize.x * scaleX, creditBoxSize.y * scaleY), creditInfo1, creditStyle);
			GUI.Label(new Rect(credit2Position.x * scaleX, credit2Position.y * scaleY, creditBoxSize.x * scaleX, creditBoxSize.y * scaleY), creditInfo2, creditStyle);
			GUI.Label(new Rect(credit3Position.x * scaleX, credit3Position.y * scaleY, creditBoxSize.x * scaleX, creditBoxSize.y * scaleY), creditInfo3, creditStyle);
			GUI.Label(new Rect(credit4Position.x * scaleX, credit4Position.y * scaleY, creditBoxSize.x * scaleX, creditBoxSize.y * scaleY), creditInfo4, creditStyle);
			GUI.Label(new Rect(credit5Position.x * scaleX, credit5Position.y * scaleY, creditBoxSize.x * scaleX, creditBoxSize.y * scaleY), creditInfo5, creditStyle);

			if(GUI.Button(new Rect(backButtonPosition.x * scaleX, backButtonPosition.y * scaleY, backButtonSize.x * scaleX, backButtonSize.y * scaleY), "Back", standardButtonStyle))
			{
				displayingArmour = false;
				currentPage = "startPage";
			}
		}
	}
}
