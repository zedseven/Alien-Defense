using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurretScript : MonoBehaviour
{
	public List<Transform> barrels;
	private int currentBarrelToUse = 0;
	public Object bulletRemains;
	public Color rayColor;
	
	public Object rayRenderer;
	public float timeToDestroyRay;
	public float rayWidth;
	
	public int damage;
	public float timeBetweenShots;
	private float lastShotTime;
	public float shotOffset;

	public Transform enemyHolder;
	public float detectionRange;
	public Transform currentEnemy;
	private Transform energyCore;

	public string turretName;
	public int cost;
	public string description;
	public Texture texture;

	public AudioClip shotSound;

	void Start()
	{
		energyCore = GameObject.FindGameObjectWithTag("EnergyCore").transform;
	}

	void Update()
	{
		if(GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().tDefenceOn == false && GameObject.FindGameObjectWithTag("SpawnObject").GetComponent<AlienSpawnScript>().createRoundGUIs == false)
		{
			//detect enemies to shoot stuff at
			if(currentEnemy != null)
			{
				if((currentEnemy.position - transform.position).magnitude > detectionRange)
				{
					currentEnemy = null;
				}
				else if(currentEnemy.position == new Vector3(0,0,0))
				{
					currentEnemy = null;
				}
			}
			if(currentEnemy == null)
			{
				float bestEnemyDistance = 9999999.9f;
				Transform bestEnemy = null;
				foreach(Transform enemy in enemyHolder.GetComponentsInChildren<Transform>())
				{
					if(enemy != enemyHolder && (new Vector3(enemy.position.x, transform.position.y, enemy.position.z) - transform.position).magnitude < detectionRange && (new Vector3(enemy.position.x, energyCore.position.y, enemy.position.z) - energyCore.position).magnitude < bestEnemyDistance)
					{
						bestEnemyDistance = (new Vector3(enemy.position.x, energyCore.position.y, enemy.position.z) - energyCore.position).magnitude;
						bestEnemy = enemy;
					}
				}
				if(bestEnemy != null)
				{
					currentEnemy = bestEnemy;
				}
			}
				
			if(currentEnemy != null)
			{
				transform.LookAt(currentEnemy.position);
			}
				
			//shoot stuff at detected enemies
			if(currentEnemy != null)
			{
				if(Time.time >= (lastShotTime + timeBetweenShots))
				{
					Transform currentBarrel = null;
					if(barrels.Count > 1)
					{
						currentBarrel = barrels[currentBarrelToUse];
						if(currentBarrelToUse < (barrels.Count - 1))
						{
							currentBarrelToUse++;
						}
						else
						{
							currentBarrelToUse = 0;
						}
					}
					else
					{
						currentBarrel = barrels[0];
					}

					lastShotTime = Time.time;
					audio.PlayOneShot(shotSound);
					Vector3 dir = transform.forward + new Vector3(Random.Range(-shotOffset, shotOffset), Random.Range(-shotOffset, shotOffset), 0);
					RaycastHit hit;
					if(Physics.Raycast(currentBarrel.position, dir, out hit))
					{
						GameObject ray = (GameObject) Instantiate(rayRenderer, currentBarrel.position, transform.rotation);
						ray.GetComponent<LineRenderer>().SetColors(rayColor, rayColor);
						ray.renderer.material.color = rayColor;
						ray.GetComponent<LineRenderer>().SetPosition(0, currentBarrel.position);
						ray.GetComponent<LineRenderer>().SetPosition(1, hit.point);
						ray.GetComponent<LineRenderer>().SetWidth(rayWidth, rayWidth);
						ray.GetComponent<LineRenderer>().enabled = true;
						ray.name = "Ray";
						ray.renderer.castShadows = false;
						ray.renderer.receiveShadows = false;
						Destroy(ray, timeToDestroyRay);
						
						//print("You hit " + hit.collider.name + ".");
						if(hit.collider.tag == "Enemy")
						{
							//damage enemy;
							hit.collider.GetComponent<HealthScript>().TakeDamage(damage);
						}
						else if(hit.collider.tag == "Object" || hit.collider.tag == "Terrain")
						{
							//add bullet remains;
							GameObject rayRemains = (GameObject) Instantiate(bulletRemains, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
							rayRemains.transform.position += rayRemains.transform.up * 0.01f;
							rayRemains.GetComponent<RayRemainsSpread>().remainsColor = rayColor;
						}
					}
					else
					{
						GameObject ray = (GameObject) Instantiate(rayRenderer, currentBarrel.position, transform.rotation);
						ray.GetComponent<LineRenderer>().SetColors(rayColor, rayColor);
						ray.renderer.material.color = rayColor;
						ray.GetComponent<LineRenderer>().SetPosition(0, currentBarrel.position);
						ray.GetComponent<LineRenderer>().SetPosition(1, dir * 1000);
						ray.GetComponent<LineRenderer>().SetWidth(rayWidth, rayWidth);
						ray.GetComponent<LineRenderer>().enabled = true;
						ray.name = "Ray";
						Destroy(ray, timeToDestroyRay);
					}
				}
			}
		}
	}
}
