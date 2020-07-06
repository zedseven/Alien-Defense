using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//[ExecuteInEditMode]

public class DefenceGUIScript : MonoBehaviour
{
	//important stuff
	public Vector2 sizeFindScale;
	public float scaleX;
	public float scaleY;
	public int gameMoney;
	public string tab = "turrets";
	public bool hasMetRequirementsGoldGun;
	public int goldGunIndex;
	public float deathContinuePercentage;

	//positions and sizes
	public Vector2 mapBackgroundPosition;
	public Vector2 mapBackgroundSize;
	public Vector2 mapPosition;
	public Vector2 mapSize;
	public Vector2 buildingGUISize;
	public Vector2 descPosition;
	public Vector2 descSize;
	public Vector2 backButtonSize;
	public Vector2 backGroupSize;
	public Vector2 tabTitlePosition;
	public Vector2 tabTitleSize;
	public Vector2 turretTexStartingPos;
	public Vector2 turretTexSize;
	public Vector2 buyTurretSize;
	public Vector2 moneySize;
	public Vector2 moneyPos;
	public Vector2 menuMoneyPos;
	public Vector2 deathText1Position;
	public Vector2 deathText1Size;
	public Vector2 deathText2Position;
	public Vector2 deathText2Size;
	public Vector2 deathContinuePosition;
	public Vector2 deathContinueSize;
	public Vector2 deathQuitPosition;
	public Vector2 deathQuitSize;
	public Vector2 endTitlePosition;
	public Vector2 endTitleSize;
	public Vector2 endRoundPosition;
	public Vector2 endRoundSize;
	public Vector2 endMoneyPosition;
	public Vector2 endMoneySize;
	public Vector2 killedAliensPosition;
	public Vector2 killedAliensSize;
	public Vector2 turretsBoughtPosition;
	public Vector2 turretsBoughtSize;
	public Vector2 bestGunEndLabelPosition;
	public Vector2 bestGunEndLabelSize;
	public Vector2 bestGunEndTexPosition;
	public Vector2 bestGunEndTexSize;
	public Vector2 bestGunEndNamePosition;
	public Vector2 bestGunEndNameSize;
	public Vector2 okayDeathButtonPosition;
	public Vector2 okayDeathButtonSize;

	//other stuff
	public GUISkin tDefenceSkin;
	public bool tDefenceOn;
	public bool deathGUIOn;
	public string whoDied;
	public List<Transform> buildings;
	private Vector2 terrainSize;
	public Vector2 spaceBetweenTurretTexes;
	public int turretTexRowAmount;
	public int selectedBuilding = -1;
	public int selectedTurretType = -1;
	public int selectedGunType = 0;
	public int equippedGunType = 0;
	public float costReSellPercentage; //percentage i want / 100, eg: 86% / 100 = 0.86
	private int numOfTurretsBought = 0;
	private Transform currentGun = null;
	private GameObject player;
	public Object playerPrefab;
	public Transform playerSpawn;
	public float playerInvincibilityLength;
	private float invincibilityLeft;
	//private bool displayingBackMenu;
	private bool displayingQuitCheck;
	private float quitCheckStartTime;
	public float timeToDisplayQuitCheck;
	public string terrainTag;

	//textures
	public Texture backgroundTex;
	public Texture background2Tex;
	public Texture mapBackgroundTex;
	public Texture buildingNoTurretsTex;
	public Texture deathBackgroundTex;
	public Texture coreDeathBackgroundTex;
	//public Texture backGroupTex;

	//styles
	public GUIStyle menuButtonStyle;
	public GUIStyle tabTitleStyle;
	public GUIStyle buildingButtonStyle;
	public GUIStyle buildingButtonStyle2;
	public GUIStyle moneyStyle;
	public GUIStyle menuMoneyStyle;
	public GUIStyle turretTexStyle;
	public GUIStyle turretTexStyle2;
	public GUIStyle standardTextStyle;
	public GUIStyle buyButtonStyle;
	public GUIStyle cantBuyButtonStyle;
	public GUIStyle turretNameStyle;
	public GUIStyle deathTextStyle1;
	public GUIStyle deathTextStyle2;
	public GUIStyle deathTextStyle3;
	public GUIStyle deathButtonStyle;
	//public GUIStyle backGroupButtonStyle;

	//turrets & guns
	public List<Object> turrets;
	public List<GameObject> sampleTurrets;
	public List<Object> guns;
	public List<GameObject> sampleGuns;
	public List<bool> ownedGuns;

	void Start()
	{
		player = (GameObject) Instantiate(playerPrefab, playerSpawn.position, Quaternion.Euler(playerSpawn.eulerAngles));//GameObject.FindGameObjectWithTag("Player");
		if(GameObject.FindGameObjectWithTag(terrainTag) != null)
		{
			if(GameObject.FindGameObjectWithTag(terrainTag).transform.GetComponent<Terrain>() != null)
			{
				terrainSize = new Vector2(GameObject.FindGameObjectWithTag(terrainTag).transform.GetComponent<Terrain>().terrainData.size.x, GameObject.FindGameObjectWithTag(terrainTag).transform.GetComponent<Terrain>().terrainData.size.z);
			}
			else
			{
				terrainSize = new Vector2(GameObject.FindGameObjectWithTag(terrainTag).transform.localScale.x, GameObject.FindGameObjectWithTag(terrainTag).transform.localScale.z);
			}
		}
		for(int i = 0; i < turrets.Count; i++)
		{
			GameObject sampleTurret = (GameObject) Instantiate(turrets[i], new Vector3(0,-100,0), Quaternion.identity);
			sampleTurret.GetComponent<TurretScript>().enabled = false;
			sampleTurret.name = "TurretSample";
			sampleTurrets.Add(sampleTurret);
		}
		for(int i = 0; i < guns.Count; i++)
		{
			GameObject sampleGun = (GameObject) Instantiate(guns[i], new Vector3(0,-100,0), Quaternion.identity);
			if(sampleGun.GetComponent<AcidGunScript>() != null)
			{
				sampleGun.GetComponent<AcidGunScript>().isSample = true;
			}
			else if(sampleGun.GetComponent<SniperGunScript>() != null)
			{
				sampleGun.GetComponent<SniperGunScript>().enabled = false;
			}
			else if(sampleGun.GetComponent<DualGunScript>() != null)
			{
				sampleGun.GetComponent<DualGunScript>().enabled = false;
			}
			else if(sampleGun.GetComponent<ShotgunScript>() != null)
			{
				sampleGun.GetComponent<ShotgunScript>().enabled = false;
			}
			else
			{
				sampleGun.GetComponent<GunScript>().enabled = false;
			}
			sampleGun.name = sampleGun.GetComponent<GunValueScript>().gunName + " Sample";
			sampleGuns.Add(sampleGun);
		}
		ownedGuns.Clear();
		for(int i = 0; i < guns.Count; i++)
		{
			ownedGuns.Add(false);
		}
		ownedGuns[equippedGunType] = true;
	}

	void Update()
	{
		if(Input.GetKeyDown("q") && GameObject.FindGameObjectWithTag("SpawnObject").GetComponent<AlienSpawnScript>().createRoundGUIs == false)
		{
			if(tDefenceOn == true)
			{
				tDefenceOn = false;
			}
			else
			{
				tDefenceOn = true;
			}
		}

		if(displayingQuitCheck == true)
		{
			if(Time.time >= (quitCheckStartTime + timeToDisplayQuitCheck))
			{
				displayingQuitCheck = false;
			}
		}

		if(player.GetComponent<HealthScript>().invincible == true)
		{
			if(invincibilityLeft <= 0.0f)
			{
				player.GetComponent<HealthScript>().invincible = false;
			}
			else
			{
				invincibilityLeft -= Time.deltaTime;
			}
		}

		//if the gun is already there, replace it if need be.
		if(currentGun != null)
		{
			//GameObject tempObject = GameObject.FindGameObjectWithTag("Player").transform.FindChild("Main Camera").FindChild("CurrentGun").gameObject;
			if(currentGun.GetComponent<GunValueScript>().gunName != sampleGuns[equippedGunType].GetComponent<GunValueScript>().gunName)
			{
				Destroy(currentGun.gameObject);
				currentGun = null;
				Vector3 pos = sampleGuns[equippedGunType].GetComponent<GunValueScript>().gunEquipPosition;
				Vector3 rot = sampleGuns[equippedGunType].GetComponent<GunValueScript>().gunEquipRotation;
				GameObject newGun = (GameObject) Instantiate(guns[equippedGunType], pos, Quaternion.Euler(rot));
				newGun.transform.parent = GameObject.FindGameObjectWithTag("Player").transform.FindChild("Main Camera");
				newGun.transform.rotation = GameObject.FindGameObjectWithTag("Player").transform.FindChild("Main Camera").transform.rotation;
				newGun.transform.localEulerAngles = rot;
				newGun.transform.localPosition = pos;
				newGun.name = "CurrentGun";
				currentGun = newGun.transform;
			}
		}
		//if there is no gun, add one.
		else
		{
			Vector3 pos = sampleGuns[equippedGunType].GetComponent<GunValueScript>().gunEquipPosition;
			Vector3 rot = sampleGuns[equippedGunType].GetComponent<GunValueScript>().gunEquipRotation;
			GameObject newGun = (GameObject) Instantiate(guns[equippedGunType], pos, Quaternion.Euler(rot));
			newGun.transform.parent = GameObject.FindGameObjectWithTag("Player").transform.FindChild("Main Camera");
			newGun.transform.rotation = GameObject.FindGameObjectWithTag("Player").transform.FindChild("Main Camera").transform.rotation;
			newGun.transform.localEulerAngles = rot;
			newGun.transform.localPosition = pos;
			newGun.name = "CurrentGun";
			currentGun = newGun.transform;
		}
	}

	void OnGUI()
	{
		scaleX = Screen.width / sizeFindScale.x;
		scaleY = Screen.height / sizeFindScale.y;

		if(tDefenceOn == false)
		{
			Screen.lockCursor = true;
			selectedBuilding = -1;
			selectedTurretType = -1;

			GUI.Label(new Rect(moneyPos.x * scaleX, moneyPos.y * scaleY, moneySize.x * scaleX, moneySize.y * scaleY), "$" + gameMoney.ToString("n0"), moneyStyle);
		}
		else if(deathGUIOn == false)
		{
			if(tab != "gameEnded")
			{
				Screen.lockCursor = false;

				GUI.BeginGroup(new Rect(mapBackgroundPosition.x * scaleX, mapBackgroundPosition.y * scaleY, mapBackgroundSize.x * scaleX, mapBackgroundSize.y * scaleY));
				GUI.DrawTexture(new Rect(0, 0, mapBackgroundSize.x * scaleX, mapBackgroundSize.y * scaleY), backgroundTex);
				GUI.DrawTexture(new Rect(descPosition.x * scaleX, descPosition.y * scaleY, descSize.x * scaleX, descSize.y * scaleY), background2Tex);
				GUI.DrawTexture(new Rect(0, 0, descSize.x * scaleX, descSize.y * scaleY), background2Tex);

				GUI.Label(new Rect(menuMoneyPos.x * scaleX, menuMoneyPos.y * scaleY, moneySize.x * scaleX, moneySize.y * scaleY), "$" + gameMoney.ToString("n0"), menuMoneyStyle);

				if(displayingQuitCheck == false)
				{
					if(GUI.Button(new Rect(0, 0, backButtonSize.x * scaleX, backButtonSize.y * scaleY), "Quit", menuButtonStyle))
					{
						displayingQuitCheck = true;
						quitCheckStartTime = Time.time;
					}
				}

				if(GUI.Button(new Rect(backButtonSize.x * scaleX, 0, backButtonSize.x * scaleX, backButtonSize.y * scaleY), "Turrets", menuButtonStyle))
				{
					tab = "turrets";
				}
				else if(GUI.Button(new Rect((backButtonSize.x * 2) * scaleX, 0, backButtonSize.x * scaleX, backButtonSize.y * scaleY), "Guns", menuButtonStyle))
				{
					tab = "guns";
				}

				if(displayingQuitCheck == true)
				{
					if(GUI.Button(new Rect(0, 0, backButtonSize.x * scaleX, backButtonSize.y * scaleY), "Sure?", menuButtonStyle))
					{
						//Application.LoadLevel(1);
						deathGUIOn = false;
						tab = "gameEnded";
					}
				}

				/*if(displayingBackMenu == true)
				{
					GUI.BeginGroup(new Rect(0, 0, backGroupSize.x * scaleX, backGroupSize.y * scaleY));
					GUI.DrawTexture(new Rect(0, 0, backGroupSize.x * scaleX, backGroupSize.y * scaleY), backGroupTex);
					bool buttonClicked = false;
					if(displayingQuitCheck == false)
					{
						if(GUI.Button(new Rect(0, (backGroupSize.y / 2) * scaleY, backGroupSize.x * scaleX, (backGroupSize.y / 2) * scaleY), "Quit", backGroupButtonStyle))
						{
							displayingQuitCheck = true;
							buttonClicked = true;
						}
					}
					else
					{
						if(GUI.Button(new Rect(0, (backGroupSize.y / 2) * scaleY, backGroupSize.x * scaleX, (backGroupSize.y / 2) * scaleY), "Are You Sure?", backGroupButtonStyle))
						{
							Application.LoadLevel(1);
						}
					}
					if(GUI.Button(new Rect(0, 0, backGroupSize.x * scaleX, (backGroupSize.y / 2) * scaleY), "Back", backGroupButtonStyle))
					{
						tDefenceOn = false;
						buttonClicked = true;
					}

					if(Input.GetMouseButtonDown(0) && buttonClicked == false)
					{
						displayingBackMenu = false;
						displayingQuitCheck = false;
					}

					GUI.EndGroup();
				}*/

				if(tab == "turrets")
				{
					//create the turret images
					for(int i = 0; i < sampleTurrets.Count; i++)
					{
						//boxInitialPos.x + ((i - ((i / rowAmount) * rowAmount)) * (boxSize.x / 2 - spaceBetweenBoxes))
						//boxInitialPos.y + ((i / rowAmount) * (boxSize.y / 2 - spaceBetweenBoxes))
						Rect imagePos = new Rect((turretTexStartingPos.x - ((i - ((i / turretTexRowAmount) * turretTexRowAmount)) * (turretTexSize.x / 2 - spaceBetweenTurretTexes.x))) * scaleX, (turretTexStartingPos.y - ((i / turretTexRowAmount) * (turretTexSize.y / 2 - spaceBetweenTurretTexes.y))) * scaleY, turretTexSize.x * scaleX, turretTexSize.y * scaleY);
						GUIStyle turretTexCurrentStyle = null;
						if(selectedTurretType == i)
						{
							turretTexCurrentStyle = turretTexStyle2;
						}
						else
						{
							turretTexCurrentStyle = turretTexStyle;
						}
						GUI.DrawTexture(imagePos, sampleTurrets[i].GetComponent<TurretScript>().texture);
						if(GUI.Button(imagePos, /*sampleTurrets[i].GetComponent<TurretScript>().texture*/ "", turretTexCurrentStyle))
						{
							selectedTurretType = i;
						}
					}
					if(selectedTurretType >= 0)
					{
						GUI.Label(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + 20) * scaleY, (descSize.x - 40) * scaleX, 30 * scaleY), sampleTurrets[selectedTurretType].GetComponent<TurretScript>().turretName, turretNameStyle);
						GUI.Label(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + 60) * scaleY, (descSize.x - 80) * scaleX, (descSize.y - 240) * scaleY), sampleTurrets[selectedTurretType].GetComponent<TurretScript>().description);
						GUI.Label(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + (descSize.y - 215)) * scaleY, (descSize.x - 40) * scaleX, 40 * scaleY), "Fire Rate: " + (1 / sampleTurrets[selectedTurretType].GetComponent<TurretScript>().timeBetweenShots) + " shots/second");
						GUI.Label(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + (descSize.y - 200)) * scaleY, (descSize.x - 40) * scaleX, 40 * scaleY), "Damage: " + sampleTurrets[selectedTurretType].GetComponent<TurretScript>().damage);
						GUI.Label(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + (descSize.y - 185)) * scaleY, (descSize.x - 40) * scaleX, 40 * scaleY), "DPS(Damage/Second): ~" + Mathf.Floor(sampleTurrets[selectedTurretType].GetComponent<TurretScript>().damage * (1 / sampleTurrets[selectedTurretType].GetComponent<TurretScript>().timeBetweenShots)));
						GUI.Label(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + (descSize.y - 170)) * scaleY, (descSize.x - 40) * scaleX, 40 * scaleY), "Range: " + sampleTurrets[selectedTurretType].GetComponent<TurretScript>().detectionRange);
						GUI.Label(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + (descSize.y - 155)) * scaleY, (descSize.x - 40) * scaleX, 40 * scaleY), "Normal Cost: $" + sampleTurrets[selectedTurretType].GetComponent<TurretScript>().cost);
						if(selectedBuilding >= 0 && selectedTurretType >= 0)
						{
							if(buildings[selectedBuilding].GetComponent<TurretSpotScript>().turretType >= 0)
							{
								if(selectedTurretType != buildings[selectedBuilding].GetComponent<TurretSpotScript>().turretType)
								{
									int costToSwitchTurrets = (int) (sampleTurrets[selectedTurretType].GetComponent<TurretScript>().cost - (sampleTurrets[buildings[selectedBuilding].GetComponent<TurretSpotScript>().turretType].GetComponent<TurretScript>().cost * (costReSellPercentage / 100)));
										
									GUI.Label(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + (descSize.y - 130)) * scaleY, buyTurretSize.x * scaleX, buyTurretSize.y * scaleY), sampleTurrets[selectedTurretType].GetComponent<TurretScript>().cost + " - (" + costReSellPercentage + "% of " + sampleTurrets[buildings[selectedBuilding].GetComponent<TurretSpotScript>().turretType].GetComponent<TurretScript>().cost + " = " + (costReSellPercentage / 100) * sampleTurrets[buildings[selectedBuilding].GetComponent<TurretSpotScript>().turretType].GetComponent<TurretScript>().cost + ") = $" + costToSwitchTurrets, standardTextStyle);
										
									if(gameMoney >= costToSwitchTurrets)
									{
										//if(GUI.Button(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + (descSize.y - 85)) * scaleY, buyTurretSize.x * scaleX, buyTurretSize.y * scaleY), "Buy? " + costToSwitchTurrets))
	/*different Position*/				if(GUI.Button(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + (descSize.y - 85)) * scaleY, buyTurretSize.x * scaleX, buyTurretSize.y * scaleY), "Buy? $" + costToSwitchTurrets, buyButtonStyle))
										{
											numOfTurretsBought++;
											buildings[selectedBuilding].GetComponent<TurretSpotScript>().AssignTurrets(turrets[selectedTurretType], selectedTurretType);
											gameMoney -= costToSwitchTurrets;
										}
									}
									else
									{
										GUI.Label(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + (descSize.y - 85)) * scaleY, buyTurretSize.x * scaleX, buyTurretSize.y * scaleY), "Can't Afford. $" + costToSwitchTurrets, cantBuyButtonStyle);
									}
								}
							}
							else
							{
								int costToSwitchTurrets = sampleTurrets[selectedTurretType].GetComponent<TurretScript>().cost;
									
								GUI.Label(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + (descSize.y - 130)) * scaleY, buyTurretSize.x * scaleX, buyTurretSize.y * scaleY), "$" + costToSwitchTurrets, standardTextStyle);
									
								if(gameMoney >= costToSwitchTurrets)
								{
									if(GUI.Button(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + (descSize.y - 85)) * scaleY, buyTurretSize.x * scaleX, buyTurretSize.y * scaleY), "Buy? $" + costToSwitchTurrets, buyButtonStyle))
									{
										numOfTurretsBought++;
										buildings[selectedBuilding].GetComponent<TurretSpotScript>().AssignTurrets(turrets[selectedTurretType], selectedTurretType);
										gameMoney -= costToSwitchTurrets;
									}
								}
								else
								{
									GUI.Label(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + (descSize.y - 85)) * scaleY, buyTurretSize.x * scaleX, buyTurretSize.y * scaleY), "Can't Afford. $" + costToSwitchTurrets, cantBuyButtonStyle);
								}
							}
						}
					}
						
					GUI.Label(new Rect(tabTitlePosition.x * scaleX, tabTitlePosition.y * scaleY, tabTitleSize.x * scaleX, tabTitleSize.y * scaleY), "Turrets", tabTitleStyle);

					GUI.BeginGroup(new Rect(mapPosition.x * scaleX, mapPosition.y * scaleY, mapSize.x * scaleX, mapSize.y * scaleY));
					GUI.DrawTexture(new Rect(0, 0, mapSize.x * scaleX, mapSize.y * scaleY), mapBackgroundTex);

					for(int i = 0; i < buildings.Count; i++)
					{
						Rect buildingPos = new Rect(((mapSize.x / terrainSize.x) * (buildings[i].position.x + (terrainSize.x / 2))) * scaleX, ((mapSize.y / terrainSize.y) * (buildings[i].position.z  + (terrainSize.y / 2))) * scaleY, /*((mapSize.x / terrainSize.x) * buildings[i].localScale.x) * scaleX, ((mapSize.y / terrainSize.y) * buildings[i].localScale.z) * scaleY*/ buildingGUISize.x * scaleX, buildingGUISize.y * scaleY);
						GUIStyle buildingButtonCurrentStyle = null;
						if(selectedBuilding == i)
						{
							buildingButtonCurrentStyle = buildingButtonStyle2;
						}
						else
						{
							buildingButtonCurrentStyle = buildingButtonStyle;
						}
						if(buildings[i].GetComponent<TurretSpotScript>().turretType >= 0)
						{
							if(GUI.Button(buildingPos, sampleTurrets[buildings[i].GetComponent<TurretSpotScript>().turretType].GetComponent<TurretScript>().texture, buildingButtonCurrentStyle))
							{
								selectedBuilding = i;
							}
						}
						else
						{
							if(GUI.Button(buildingPos, buildingNoTurretsTex, buildingButtonCurrentStyle))
							{
								selectedBuilding = i;
							}
						}
					}
						
					//-50, 0, 25, 25

					GUI.EndGroup();
				}
				else if(tab == "guns")
				{
					//create the gun images
					for(int i = 0; i < sampleGuns.Count; i++)
					{
						//boxInitialPos.x + ((i - ((i / rowAmount) * rowAmount)) * (boxSize.x / 2 - spaceBetweenBoxes))
						//boxInitialPos.y + ((i / rowAmount) * (boxSize.y / 2 - spaceBetweenBoxes))
						Rect imagePos = new Rect((turretTexStartingPos.x - ((i - ((i / turretTexRowAmount) * turretTexRowAmount)) * (turretTexSize.x / 2 - spaceBetweenTurretTexes.x))) * scaleX, (turretTexStartingPos.y - ((i / turretTexRowAmount) * (turretTexSize.y / 2 - spaceBetweenTurretTexes.y))) * scaleY, turretTexSize.x * scaleX, turretTexSize.y * scaleY);
						GUIStyle turretTexCurrentStyle = null;
						if(selectedGunType == i)
						{
	/*Target/Image*/		turretTexCurrentStyle = turretTexStyle2;
						}
						else
						{
							turretTexCurrentStyle = turretTexStyle;
						}
						GUI.DrawTexture(imagePos, sampleGuns[i].GetComponent<GunValueScript>().texture);
						if(GUI.Button(imagePos, /*sampleGuns[i].GetComponent<GunValueScript>().texture*/ "", turretTexCurrentStyle))
						{
							selectedGunType = i;
						}
					}
					if(selectedGunType >= 0)
					{
						GUI.Label(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + 20) * scaleY, (descSize.x - 40) * scaleX, 30 * scaleY), sampleGuns[selectedGunType].GetComponent<GunValueScript>().gunName, turretNameStyle);
						GUI.Label(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + 60) * scaleY, (descSize.x - 80) * scaleX, (descSize.y - 240) * scaleY), sampleGuns[selectedGunType].GetComponent<GunValueScript>().description);
						GUI.Label(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + (descSize.y - 215)) * scaleY, (descSize.x - 40) * scaleX, 40 * scaleY), "Fire Rate: " + (1 / sampleGuns[selectedGunType].GetComponent<GunValueScript>().timeBetweenShots) + " shots/second");
						GUI.Label(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + (descSize.y - 200)) * scaleY, (descSize.x - 40) * scaleX, 40 * scaleY), "Damage: " + sampleGuns[selectedGunType].GetComponent<GunValueScript>().damage);
						GUI.Label(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + (descSize.y - 185)) * scaleY, (descSize.x - 40) * scaleX, 40 * scaleY), "DPS(Damage/Second): ~" + Mathf.Floor(sampleGuns[selectedGunType].GetComponent<GunValueScript>().damage * (1 / sampleGuns[selectedGunType].GetComponent<GunValueScript>().timeBetweenShots)));
						GUI.Label(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + (descSize.y - 170)) * scaleY, (descSize.x - 40) * scaleX, 40 * scaleY), "Cost: $" + sampleGuns[selectedGunType].GetComponent<GunValueScript>().cost);
						
						if(selectedGunType >= 0)
						{
							if(ownedGuns[selectedGunType] == false)
							{				
								if(selectedGunType != goldGunIndex || (selectedGunType == goldGunIndex && hasMetRequirementsGoldGun == true))
								{
									int costToBuyGun = sampleGuns[selectedGunType].GetComponent<GunValueScript>().cost;
										
									if(gameMoney >= costToBuyGun)
									{
										if(GUI.Button(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + (descSize.y - 85)) * scaleY, buyTurretSize.x * scaleX, buyTurretSize.y * scaleY), "Buy? $" + costToBuyGun, buyButtonStyle))
										{
											gameMoney -= costToBuyGun;
											ownedGuns[selectedGunType] = true;
										}
									}
									else
									{
										GUI.Label(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + (descSize.y - 85)) * scaleY, buyTurretSize.x * scaleX, buyTurretSize.y * scaleY), "Can't Afford. $" + costToBuyGun, cantBuyButtonStyle);
									}
								}
								else if(selectedGunType == goldGunIndex && hasMetRequirementsGoldGun == false)
								{
									GUI.Label(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + (descSize.y - 85)) * scaleY, buyTurretSize.x * scaleX, buyTurretSize.y * scaleY), "Unlock First", cantBuyButtonStyle);
								}
							}
							else
							{
								if(equippedGunType != selectedGunType)
								{
									if(GUI.Button(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + (descSize.y - 85)) * scaleY, buyTurretSize.x * scaleX, buyTurretSize.y * scaleY), "Equip", buyButtonStyle))
									{
										equippedGunType = selectedGunType;
									}
								}
								else
								{
									GUI.Label(new Rect((descPosition.x + 20) * scaleX, (descPosition.y + (descSize.y - 85)) * scaleY, buyTurretSize.x * scaleX, buyTurretSize.y * scaleY), "Equipped", cantBuyButtonStyle);
								}
							}
						}
					}

					GUI.Label(new Rect(tabTitlePosition.x * scaleX, tabTitlePosition.y * scaleY, tabTitleSize.x * scaleX, tabTitleSize.y * scaleY), "Guns", tabTitleStyle);

					GUI.BeginGroup(new Rect(mapPosition.x * scaleX, mapPosition.y * scaleY, mapSize.x * scaleX, mapSize.y * scaleY));
					//something is wrong with the selectedGunType, or sampleGuns count. check both.
					//print(selectedGunType + " : " + sampleGuns.Count); //-1 : 2 //probably not setting the selectedGunType

					if(selectedGunType >= 0)
					{
						GUI.DrawTexture(new Rect(0, 0, mapSize.x * scaleX, mapSize.y * scaleY), sampleGuns[selectedGunType].GetComponent<GunValueScript>().texture);
					}
					
					//-50, 0, 25, 25
					
					GUI.EndGroup();
				}
				GUI.EndGroup();
			}
			else
			{
				Screen.lockCursor = false;
				GUI.DrawTexture(new Rect(0, 0, mapBackgroundSize.x * scaleX, mapBackgroundSize.y * scaleY), deathBackgroundTex);
				GUI.Label(new Rect(endTitlePosition.x * scaleX, endTitlePosition.y * scaleY, endTitleSize.x * scaleX, endTitleSize.y * scaleY), "You Lost!", deathTextStyle1);
				GUI.Label(new Rect(endRoundPosition.x * scaleX, endRoundPosition.y * scaleY, endRoundSize.x * scaleX, endRoundSize.y * scaleY), "Last Round: " + (GameObject.FindGameObjectWithTag("SpawnObject").GetComponent<AlienSpawnScript>().roundNum + 1), deathTextStyle3);
				GUI.Label(new Rect(endMoneyPosition.x * scaleX, endMoneyPosition.y * scaleY, endMoneySize.x * scaleX, endMoneySize.y * scaleY), "Final Money: $" + gameMoney, deathTextStyle3);
				GUI.Label(new Rect(killedAliensPosition.x * scaleX, killedAliensPosition.y * scaleY, killedAliensSize.x * scaleX, killedAliensSize.y * scaleY), "Aliens Killed: " + GameObject.FindGameObjectWithTag("SpawnObject").GetComponent<AlienSpawnScript>().numberOfKilledAliensTotal, deathTextStyle3);
				GUI.Label(new Rect(turretsBoughtPosition.x * scaleX, turretsBoughtPosition.y * scaleY, turretsBoughtSize.x * scaleX, turretsBoughtSize.y * scaleY), "Turrets Bought: " + numOfTurretsBought, deathTextStyle3);
				int bestGunIndex = -1;
				for(int i = 0; i < ownedGuns.Count; i++)
				{
					if(ownedGuns[i] == true)
					{
						if(i > bestGunIndex)
						{
							bestGunIndex = i;
						}
					}
				}
				GUI.Label(new Rect(bestGunEndLabelPosition.x * scaleX, bestGunEndLabelPosition.y * scaleY, bestGunEndLabelSize.x * scaleX, bestGunEndLabelSize.y * scaleY), "Best Gun:", deathTextStyle3);
				GUI.DrawTexture(new Rect(bestGunEndTexPosition.x * scaleX, bestGunEndTexPosition.y * scaleY, bestGunEndTexSize.x * scaleX, bestGunEndTexSize.y * scaleY), sampleGuns[bestGunIndex].GetComponent<GunValueScript>().texture);
				GUI.Label(new Rect(bestGunEndNamePosition.x * scaleX, bestGunEndNamePosition.y * scaleY, bestGunEndNameSize.x * scaleX, bestGunEndNameSize.y * scaleY), sampleGuns[bestGunIndex].GetComponent<GunValueScript>().gunName, deathTextStyle1);
				if(GUI.Button(new Rect(okayDeathButtonPosition.x * scaleX, okayDeathButtonPosition.y * scaleY, okayDeathButtonSize.x * scaleX, okayDeathButtonSize.y * scaleY), "Okay", deathButtonStyle))
				{
					Application.LoadLevel(1);
				}
			}
		}
		else
		{
			Screen.lockCursor = false;
			if(whoDied == "player")
			{
				GUI.DrawTexture(new Rect(0, 0, mapBackgroundSize.x * scaleX, mapBackgroundSize.y * scaleY), deathBackgroundTex);
				GUI.Label(new Rect(deathText1Position.x * scaleX, deathText1Position.y * scaleY, deathText1Size.x * scaleX, deathText1Size.y * scaleY), "You Died!", deathTextStyle1);
				GUI.Label(new Rect(deathText2Position.x * scaleX, deathText2Position.y * scaleY, deathText2Size.x * scaleX, deathText2Size.y * scaleY), "When you continue, you will lose " + deathContinuePercentage + "% of your total money.", deathTextStyle2);
				int continueCost = (int)(gameMoney * (deathContinuePercentage / 100));
				if(GUI.Button(new Rect(deathContinuePosition.x * scaleX, deathContinuePosition.y * scaleY, deathContinueSize.x * scaleX, deathContinueSize.y * scaleY), "Continue $" + continueCost, deathButtonStyle))
				{
					gameMoney -= continueCost;
					player.GetComponent<HealthScript>().currentHealth = player.GetComponent<HealthScript>().maxHealth;
					player.GetComponent<HealthScript>().hasDied = false;
					player.GetComponent<HealthScript>().invincible = true;
					invincibilityLeft = playerInvincibilityLength;
					player.transform.position = playerSpawn.position;
					player.transform.eulerAngles = playerSpawn.eulerAngles;
					tDefenceOn = false;
					deathGUIOn = false;
				}
				if(GUI.Button(new Rect(deathQuitPosition.x * scaleX, deathQuitPosition.y * scaleY, deathQuitSize.x * scaleX, deathQuitSize.y * scaleY), "Quit", deathButtonStyle))
				{
					deathGUIOn = false;
					tab = "gameEnded";
				}
			}
			else if(whoDied == "energyCore")
			{
				deathGUIOn = false;
				tab = "gameEnded";
			}
		}
	}
}
