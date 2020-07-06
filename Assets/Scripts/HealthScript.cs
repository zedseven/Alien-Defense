using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealthScript : MonoBehaviour
{
	public enum HealthType
	{
		Player = 0,
		Enemy = 1,
		Core = 2
	}
	public HealthType healthType = HealthType.Player;
	
	public int maxHealth;
	public int currentHealth;
	public float playerDefense = 1.0f;
	public float playerSpeed;

	public bool invincible = false;

	public int moneyToLose;

	public List<Object> armourTypes;
	
	public AudioClip deathSound;

	public GUIStyle styleH;
	public GUIStyle styleCH;
	public GUIStyle style1;
	public GUIStyle style2;

	public Vector2 barPos;
	public Vector2 barSize; //200 x 18
	public Vector2 bar2Pos;
	public Vector2 bar2Size;
	public Vector2 offset;

	public bool hasDied;

	private ParticleSystem.CollisionEvent[] collisionEvents = new ParticleSystem.CollisionEvent[16];

	void Start()
	{
		if(healthType == HealthType.Player)
		{
			GameObject armourSample = (GameObject) Instantiate(armourTypes[PlayerPrefs.GetInt("ArmourType")], new Vector3(0, -100, 0), Quaternion.identity);
			armourSample.GetComponent<MenuArmourScript>().enabled = false;
			Destroy(armourSample, 1);

			playerDefense = armourSample.GetComponent<MenuArmourScript>().defense;
			playerSpeed = armourSample.GetComponent<MenuArmourScript>().speed;
			GetComponent<MeshFilter>().mesh = armourSample.GetComponent<MeshFilter>().mesh;
			renderer.material = armourSample.renderer.material;
		}
	}

	void Update()
	{
		if(currentHealth > maxHealth)
		{
			currentHealth = maxHealth;
		}
	}

	public void TakeDamage(int damage)
	{
		if(invincible == false)
		{
			//currentHealth -= (damage - (playerDefense / 2));
			if(healthType == HealthType.Player)
			{
				currentHealth -= (int)((float)damage * playerDefense);
			}
			else
			{
				currentHealth -= damage;
			}
			if(currentHealth <= 0 && hasDied == false)
			{
				hasDied = true;
				//print(healthType.ToString() + " has died.");
				if(healthType == HealthType.Player)
				{
					//lose some money, etc.
					GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().tDefenceOn = true;
					GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().deathGUIOn = true;
					GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().whoDied = "player";
				}
				else if(healthType == HealthType.Enemy)
				{
					//give coins, remove, etc.
					//audio.clip = deathSound;
					audio.PlayOneShot(deathSound);
					GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().gameMoney += transform.GetComponent<EnemyNavScript>().moneyToDrop;
					GameObject.FindGameObjectWithTag("SpawnObject").GetComponent<AlienSpawnScript>().numberOfKilledAliens++;
					GameObject.FindGameObjectWithTag("SpawnObject").GetComponent<AlienSpawnScript>().numberOfKilledAliensTotal++;
					Destroy(gameObject);
				}
				else if(healthType == HealthType.Core)
				{
					//end level, etc.
					GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().tDefenceOn = true;
					GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().deathGUIOn = true;
					GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().whoDied = "energyCore";
				}
			}
		}
	}

	void OnParticleCollision(GameObject other)
	{
		if(other.transform.parent.GetComponent<AcidGunScript>() != null)
		{
			if(healthType == HealthType.Enemy)
			{
				ParticleSystem particleSystem;
				particleSystem = other.GetComponent<ParticleSystem>();
				int safeLength = particleSystem.safeCollisionEventSize;
				if(collisionEvents.Length < safeLength)
				{
					collisionEvents = new ParticleSystem.CollisionEvent[safeLength];
				}
				
				int numCollisionEvents = particleSystem.GetCollisionEvents(gameObject, collisionEvents);
				int i = 0;
				int damageToTake = 0;
				while(i < numCollisionEvents)
				{
					damageToTake += other.transform.parent.GetComponent<AcidGunScript>().damage;
					i++;
				}
				TakeDamage(damageToTake);
				//print("Damage Taken: " + damageToTake);
			}
		}
	}
		
	void OnGUI()
	{
		if(healthType == HealthType.Player && GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().tDefenceOn == false)
		{
			float scaleX = GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().scaleX;
			float scaleY = GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().scaleY;

			GUI.Label(new Rect(barPos.x * scaleX, barPos.y * scaleY, barSize.x * scaleX, barSize.y * scaleY), "", style1);
			GUI.Label(new Rect(barPos.x * scaleX, barPos.y * scaleY, ((barSize.x / maxHealth) * currentHealth) * scaleX, barSize.y * scaleY), "", styleH);
			GUI.Label(new Rect(barPos.x * scaleX, barPos.y * scaleY, barSize.x * scaleX, barSize.y * scaleY), "Health: " + (Mathf.Round((((float) currentHealth / (float) maxHealth) * 100) * 10)) / 10 + "%", style2);
		
			float coreHealth = (float) GameObject.FindGameObjectWithTag("EnergyCore").GetComponent<HealthScript>().currentHealth;
			float coreMaxHealth = (float) GameObject.FindGameObjectWithTag("EnergyCore").GetComponent<HealthScript>().maxHealth;
			GUI.Label(new Rect(bar2Pos.x * scaleX, bar2Pos.y * scaleY, bar2Size.x * scaleX, bar2Size.y * scaleY), "", style1);
			GUI.Label(new Rect(bar2Pos.x * scaleX, bar2Pos.y * scaleY, ((bar2Size.x / coreMaxHealth) * coreHealth) * scaleX, bar2Size.y * scaleY), "", styleCH);
			GUI.Label(new Rect(bar2Pos.x * scaleX, bar2Pos.y * scaleY, bar2Size.x * scaleX, bar2Size.y * scaleY), "Core Health: " + (Mathf.Round(((coreHealth / coreMaxHealth) * 100) * 10)) / 10 + "%", style2);
		}
		/*if(healthType == HealthType.Core)
		{
			Vector3 characterPos = Camera.main.WorldToScreenPoint(transform.position);
			
			characterPos = new Vector3(Mathf.Clamp(characterPos.x,0 + (barSize.x / 2),Screen.width - (barSize.x / 2)),
			Mathf.Clamp(characterPos.y,50,Screen.height), characterPos.z);
			
			GUILayout.BeginArea(new Rect((characterPos.x + offset.x) - (barSize.x / 2), (Screen.height - characterPos.y) + offset.y, barSize.x, barSize.y));
			
			// GUI CODE GOES HERE
			float scaleX = GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().scaleX;
			float scaleY = GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().scaleY;

			GUI.Label(new Rect (0, 0, bar2Size.x * scaleX, bar2Size.y * scaleY), "", style1);
			GUI.Label(new Rect (0, 0, ((bar2Size.x / maxHealth) * currentHealth) * scaleX, bar2Size.y * scaleY), "", styleCH);
				
			GUILayout.EndArea();
		}*/
	}
}
