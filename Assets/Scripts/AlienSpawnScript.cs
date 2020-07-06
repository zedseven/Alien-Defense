using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlienSpawnScript : MonoBehaviour
{
	public List<Object> alienTypes;
	public List<Transform> spawners;

	public Transform enemyHolder;

	public int numOfAliensAllowed;

	public List<string> roundEnemyCodes;
	/*10-digit code. first 4 are alienTypes indexes, next 3 are the number of enemies to be spawned.(a non-int digit acts as a null digit), next 1 is the 
	number to spawn at a time, next 1 is time between spawns(roughly, it may be slightly randomized), last 1 is spawnType.*/

	//code access stuff
	public int aliensSDigit = 0;
	public int aliensEDigit = 3;
	public int numSpawnsDigitS = 4;//4
	public int numSpawnsDigitE = 6;//6
	public int numToSpawnAtTimeDigit = 7;
	public int timeBetweenSpawnsDigit = 8;
	public int spawnTypeDigit = 9;

	//GUI positions and sizes
	public Vector2 newRoundBackgroundPosition;
	public Vector2 newRoundBackgroundSize;
	public Vector2 newRoundTextPosition;
	public Vector2 alienExpectationTextPosition;
	public Vector2 expectedAliensPosition;
	public Vector2 expectedAliensSize;
	public Vector2 expectedAliensNamesPosition;
	public Vector2 expectedAliensNamesSize;
	public Vector2 unlockGoldGunTextPosition;
	public Vector2 unlockGoldGunTextSize;
	public Vector2 unlockGoldGunImagePosition;
	public Vector2 unlockGoldGunImageSize;
	
	//GUI textures
	public Texture2D newRoundBackgroundTex;

	//GUI styles
	public GUIStyle newRoundStyle;
	public GUIStyle alienExpectationStyle;
	public GUIStyle expectedAlienNamesStyle;
	public GUIStyle unlockGoldGunStyle;

	//other stuff
	public float coreRegenPercentage;

	public int roundNum;
	private string roundCode;

	public int roundNumRequirementGoldGun;

	public LayerMask terrainLayer;

	private int numberToSpawn;
	private int numberOfSpawnedAliens;
	public int numberOfKilledAliens;
	public int numberOfKilledAliensTotal;

	private float recentSpawnTime;
	private float timeSincePaused;

	public bool createRoundGUIs;
	private float roundGuiStartTime;
	public float timeToDisplayNewRound;

	public int distanceBetweenAlienTexes;

	private DefenceGUIScript guiStuffScript;

	private List<string> runningAlienTypes = new List<string>();

	private List<GameObject> sampleAliens = new List<GameObject>();

	private List<int> alienIndexesCurrentRound = new List<int>();

	void Start()
	{
		NewRound(false);
		guiStuffScript = GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>();
		for(int i = 0; i < alienTypes.Count; i++)
		{
			GameObject sampleAlien = (GameObject) Instantiate(alienTypes[i], new Vector3(0,-100,0), Quaternion.identity);
			sampleAlien.name = "AlienSample";
			sampleAlien.GetComponent<EnemyNavScript>().enabled = false;
			sampleAlien.GetComponent<AiCalculationScript>().enabled = false;
			sampleAliens.Add(sampleAlien);
		}
	}

	void Update()
	{
		if(guiStuffScript.tDefenceOn == true)
		{
			timeSincePaused += Time.deltaTime;
		}
		if(numberOfSpawnedAliens < numberToSpawn && createRoundGUIs == false && guiStuffScript.tDefenceOn == false)
		{
			if(int.Parse(roundCode[spawnTypeDigit].ToString()) == 1)
			{
				//print(Time.time + " : " + recentSpawnTime + (float) int.Parse(roundCode[timeBetweenSpawnsDigit].ToString()) + " : " + roundCode[timeBetweenSpawnsDigit] + " : " + recentSpawnTime);
				if(Time.time >= (recentSpawnTime + (float) int.Parse(roundCode[timeBetweenSpawnsDigit].ToString()) + timeSincePaused))
				{
					recentSpawnTime = Time.time;
					timeSincePaused = 0;
					for(int i = 0; i < int.Parse(roundCode[numToSpawnAtTimeDigit].ToString()); i++)
					{
						numberOfSpawnedAliens++;
						int alienNum = int.Parse(roundCode[Random.Range(aliensSDigit, (aliensEDigit + 1))].ToString());
						int spawnerNum = Random.Range(0, spawners.Count);
						float spawnedAlienHeight = 0.0f;
						RaycastHit hit;
						if(Physics.Raycast(spawners[spawnerNum].position, Vector3.down, out hit, Mathf.Infinity, terrainLayer))
						{
							spawnedAlienHeight = hit.point.y + sampleAliens[alienNum].GetComponent<EnemyNavScript>().heightOffGround;
						}
						GameObject spawnedAlien = (GameObject) Instantiate(alienTypes[alienNum], new Vector3(spawners[spawnerNum].position.x, spawnedAlienHeight, spawners[spawnerNum].position.z), Quaternion.identity);
						spawnedAlien.transform.parent = enemyHolder;
						//spawnedAlien.GetComponent<EnemyNavScript>().enabled = false;
					}
				}
			}
			else if(int.Parse(roundCode[spawnTypeDigit].ToString()) == 2)
			{
				if(Time.time >= (recentSpawnTime + (float) int.Parse(roundCode[timeBetweenSpawnsDigit].ToString()) + timeSincePaused))
				{
					//print("spawn alien.");
					recentSpawnTime = Time.time;
					timeSincePaused = 0;
					for(int i = 0; i < int.Parse(roundCode[numToSpawnAtTimeDigit].ToString()); i++)
					{
						if(numberOfSpawnedAliens < numberToSpawn)
						{
							numberOfSpawnedAliens++;
							int alienNum = int.Parse(roundCode[Random.Range(aliensSDigit, (aliensEDigit + 1))].ToString());
							int spawnerNum = Random.Range(0, spawners.Count);
							float spawnedAlienHeight = 0.0f;
							RaycastHit hit;
							if(Physics.Raycast(spawners[spawnerNum].position, Vector3.down, out hit, Mathf.Infinity, terrainLayer))
							{
								spawnedAlienHeight = hit.point.y + sampleAliens[alienNum].GetComponent<EnemyNavScript>().heightOffGround;
							}
							GameObject spawnedAlien = (GameObject) Instantiate(alienTypes[alienNum], new Vector3(spawners[spawnerNum].position.x, spawnedAlienHeight, spawners[spawnerNum].position.z), Quaternion.identity);
							spawnedAlien.transform.parent = enemyHolder;
							//spawnedAlien.GetComponent<EnemyNavScript>().enabled = false;
						}
					}
				}
			}
		}
		if(numberOfSpawnedAliens == numberOfKilledAliens && numberOfSpawnedAliens >= numberToSpawn)
		{
			NewRound(true);
		}
		if(Time.time >= (roundGuiStartTime + timeToDisplayNewRound) && createRoundGUIs == true)
		{
			createRoundGUIs = false;
		}
	}

	void NewRound(bool changeRound)
	{
		if(changeRound)
		{
			roundNum++;
			GameObject energyCore = GameObject.FindGameObjectWithTag("EnergyCore");
			if(energyCore.GetComponent<HealthScript>().currentHealth + (int) (Mathf.Round((float) energyCore.GetComponent<HealthScript>().maxHealth * (coreRegenPercentage / 100.0f))) > energyCore.GetComponent<HealthScript>().maxHealth)
			{
				energyCore.GetComponent<HealthScript>().currentHealth = energyCore.GetComponent<HealthScript>().maxHealth;
			}
			else
			{
				energyCore.GetComponent<HealthScript>().currentHealth += (int) (Mathf.Round((float) energyCore.GetComponent<HealthScript>().maxHealth * (coreRegenPercentage / 100.0f)));
			}
		}
		createRoundGUIs = true;
		roundGuiStartTime = Time.time;

		numberOfSpawnedAliens = 0;
		numberOfKilledAliens = 0;
		if(roundEnemyCodes.Count > roundNum)
		{
			roundCode = roundEnemyCodes[roundNum].ToString();
		}
		else if(roundCode == null || roundNum >= roundEnemyCodes.Count)
		{
			roundCode = roundEnemyCodes[roundEnemyCodes.Count - 1].ToString();
		}
		alienIndexesCurrentRound.Clear();
		string spawnNumber = "";//roundCode[numSpawnsDigitS].ToString();
		for(int i = 0; i < (numSpawnsDigitE - numSpawnsDigitS) + 1; i++)
		{
			int digit = 0;
			if(int.TryParse(roundCode[i + ((numSpawnsDigitE - numSpawnsDigitS) + 2)].ToString(), out digit))
			{
				spawnNumber = spawnNumber + digit;//roundCode[i].ToString();
			}
		}
		for(int i = 0; i < (aliensEDigit - aliensSDigit) + 1; i++)
		{
			if(alienIndexesCurrentRound.Contains(int.Parse(roundCode[i + aliensSDigit].ToString()))){}
			else
			{
				alienIndexesCurrentRound.Add(int.Parse(roundCode[i + aliensSDigit].ToString()));
			}
		}
		//print(spawnNumber);
		numberToSpawn = int.Parse(spawnNumber);

		if(roundNum == roundNumRequirementGoldGun || (roundNum > roundNumRequirementGoldGun && GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().hasMetRequirementsGoldGun == false))
		{
			GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().hasMetRequirementsGoldGun = true;
		}
	}

	void OnGUI()
	{
		if(createRoundGUIs == true)
		{
			float scaleX = GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().scaleX;
			float scaleY = GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().scaleY;

			GUI.DrawTexture(new Rect(newRoundBackgroundPosition.x * scaleX, newRoundBackgroundPosition.y * scaleY, newRoundBackgroundSize.x * scaleX, newRoundBackgroundSize.y * scaleY), newRoundBackgroundTex);
			GUI.Label(new Rect(newRoundTextPosition.x * scaleX, newRoundTextPosition.y * scaleY, 800 * scaleX, 300 * scaleY), "Round " + (roundNum + 1), newRoundStyle);
			GUI.Label(new Rect(alienExpectationTextPosition.x * scaleX, alienExpectationTextPosition.y * scaleY, 800 * scaleX, 60 * scaleY), "Aliens to expect: ", alienExpectationStyle);
			for(int i = 0; i < (aliensEDigit - aliensSDigit) + 1; i++)
			{
				if(runningAlienTypes.Contains(alienTypes[int.Parse(roundCode[i + aliensSDigit].ToString())].name)){}
				else
				{
					runningAlienTypes.Add(alienTypes[int.Parse(roundCode[i + aliensSDigit].ToString())].name);
				}
			}
			//print(runningAlienTypes.Count);
			for(int i = 0; i < runningAlienTypes.Count; i++)
			{
				Rect alienTexPos = new Rect((((((expectedAliensSize.x * runningAlienTypes.Count) + (distanceBetweenAlienTexes * (runningAlienTypes.Count - 1))) / runningAlienTypes.Count) * ((float) i - (float) runningAlienTypes.Count / 2)) + expectedAliensPosition.x) * scaleX, expectedAliensPosition.y * scaleY, expectedAliensSize.x * scaleX, expectedAliensSize.y * scaleY);
				Rect alienNamePos = new Rect((((((expectedAliensSize.x * runningAlienTypes.Count) + (distanceBetweenAlienTexes * (runningAlienTypes.Count - 1))) / runningAlienTypes.Count) * ((float) i - (float) runningAlienTypes.Count / 2)) + expectedAliensNamesPosition.x) * scaleX, expectedAliensNamesPosition.y * scaleY, expectedAliensNamesSize.x * scaleX, expectedAliensNamesSize.y * scaleY);
				GUI.DrawTexture(alienTexPos, sampleAliens[alienIndexesCurrentRound[i]].GetComponent<EnemyNavScript>().alienGUITexture);
				GUI.Label(alienNamePos, runningAlienTypes[i], expectedAlienNamesStyle);
			}
			runningAlienTypes.Clear();

			if(roundNum == roundNumRequirementGoldGun && createRoundGUIs == true)
			{
				GUI.Label(new Rect(unlockGoldGunTextPosition.x * scaleX, unlockGoldGunTextPosition.y * scaleY, unlockGoldGunTextSize.x * scaleX, unlockGoldGunTextSize.y * scaleY), "You unlocked the " + GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().sampleGuns[GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().goldGunIndex].GetComponent<GunValueScript>().gunName + "!", unlockGoldGunStyle);
				GUI.DrawTexture(new Rect(unlockGoldGunImagePosition.x * scaleX, unlockGoldGunImagePosition.y * scaleY, unlockGoldGunImageSize.x * scaleX, unlockGoldGunImageSize.y * scaleY), GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().sampleGuns[GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().goldGunIndex].GetComponent<GunValueScript>().texture);
			}
		}
	}
}
