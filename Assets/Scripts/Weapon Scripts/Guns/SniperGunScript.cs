using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SniperGunScript : MonoBehaviour
{
	public Transform barrel;
	public Transform beamStart;
	public Object bulletRemains;
	public Color rayColor;

	public Object rayRenderer;
	public float timeToDestroyRay;
	public float rayWidth;

	public LayerMask layersToCastOn;
	public LayerMask beamLayersToCastOn;

	public Color beamColor;
	public float beamWidth;
	
	public int damage;
	public float timeBetweenShots;
	private float timeSinceLastShot;
	public float minVolume;
	public float maxVolume;
	public float shotOffset;

	public AudioClip shotSound;

	public Vector2 chargeDisplayBoxPosition;
	public Vector2 chargeDisplayBoxSize;
	public GUIStyle chargeDisplayBackgroundStyle;
	public GUIStyle chargeDisplayBarStyle;
	public GUIStyle chargeDisplayTextStyle;

	void Start()
	{
		timeSinceLastShot = timeBetweenShots;
	}

	void Update()
	{
		if(GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().tDefenceOn == false && GameObject.FindGameObjectWithTag("SpawnObject").GetComponent<AlienSpawnScript>().createRoundGUIs == false)
		{
			//GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
			//transform.localEulerAngles = new Vector3(camera.transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);

			timeSinceLastShot += Time.deltaTime;

			if(Input.GetMouseButtonDown(0))
			{
				float volume = 0.0f;
				if(timeSinceLastShot < timeBetweenShots)
				{
					volume = ((timeSinceLastShot / timeBetweenShots) * (maxVolume - minVolume)) + minVolume;
				}
				else
				{
					volume = maxVolume;
				}
				audio.volume = volume;
				audio.PlayOneShot(shotSound);
				Vector3 dir = barrel.forward + new Vector3(Random.Range(-shotOffset, shotOffset), Random.Range(-shotOffset, shotOffset), 0);
				RaycastHit hit;
				if(Physics.Raycast(barrel.position, dir, out hit, Mathf.Infinity, layersToCastOn))
				{
					GameObject ray = (GameObject) Instantiate(rayRenderer, barrel.position, transform.rotation);
					ray.GetComponent<LineRenderer>().SetColors(rayColor, rayColor);
					ray.renderer.material.color = rayColor;
					ray.GetComponent<LineRenderer>().SetPosition(0, barrel.position);
					ray.GetComponent<LineRenderer>().SetPosition(1, hit.point);
					ray.GetComponent<LineRenderer>().SetWidth(rayWidth, rayWidth);
					ray.GetComponent<LineRenderer>().enabled = true;
					ray.name = "Ray";
					Destroy(ray, timeToDestroyRay);

					//print("You hit " + hit.collider.name + ".");
					if(hit.collider.tag == "Enemy")
					{
						//damage enemy;
						float damageToDeal = 0.0f;
						if(timeSinceLastShot < timeBetweenShots)
						{
							damageToDeal = (timeSinceLastShot / timeBetweenShots) * damage;
						}
						else
						{
							damageToDeal = damage;
						}
						hit.collider.GetComponent<HealthScript>().TakeDamage((int) damageToDeal);
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
					GameObject ray = (GameObject) Instantiate(rayRenderer, barrel.position, transform.rotation);
					ray.GetComponent<LineRenderer>().SetColors(rayColor, rayColor);
					ray.renderer.material.color = rayColor;
					ray.GetComponent<LineRenderer>().SetPosition(0, barrel.position);
					ray.GetComponent<LineRenderer>().SetPosition(1, dir * 1000);
					ray.GetComponent<LineRenderer>().SetWidth(rayWidth, rayWidth);
					ray.GetComponent<LineRenderer>().enabled = true;
					ray.name = "Ray";
					Destroy(ray, timeToDestroyRay);
				}
				timeSinceLastShot = 0.0f;
			}
		}
		Vector3 beamDir = beamStart.forward;
		RaycastHit beamHit;
		beamStart.GetComponent<LineRenderer>().SetColors(beamColor, beamColor);
		beamStart.renderer.material.color = beamColor;
		beamStart.GetComponent<LineRenderer>().SetPosition(0, beamStart.position);
		beamStart.GetComponent<LineRenderer>().SetWidth(beamWidth, beamWidth);
		beamStart.GetComponent<LineRenderer>().enabled = true;
		if(Physics.Raycast(beamStart.position, beamDir, out beamHit, Mathf.Infinity, beamLayersToCastOn))
		{
			beamStart.GetComponent<LineRenderer>().SetPosition(1, beamHit.point);
		}
		else
		{
			beamStart.GetComponent<LineRenderer>().SetPosition(1, beamDir * 1000);
		}
	}

	void OnGUI()
	{
		float scaleX = GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().scaleX;
		float scaleY = GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().scaleY;
		
		float timeSince = 0.0f;
		if(timeSinceLastShot < timeBetweenShots)
		{
			timeSince = timeSinceLastShot;
		}
		else
		{
			timeSince = timeBetweenShots;
		}
		
		GUI.Label(new Rect(chargeDisplayBoxPosition.x * scaleX, chargeDisplayBoxPosition.y * scaleY, chargeDisplayBoxSize.x * scaleX, chargeDisplayBoxSize.y * scaleY), "", chargeDisplayBackgroundStyle);
		GUI.Label(new Rect(chargeDisplayBoxPosition.x * scaleX, chargeDisplayBoxPosition.y * scaleY, ((chargeDisplayBoxSize.x / timeBetweenShots) * timeSince) * scaleX, chargeDisplayBoxSize.y * scaleY), "", chargeDisplayBarStyle);
		GUI.Label(new Rect(chargeDisplayBoxPosition.x * scaleX, chargeDisplayBoxPosition.y * scaleY, chargeDisplayBoxSize.x * scaleX, chargeDisplayBoxSize.y * scaleY), "Charge: " + Mathf.Floor((timeSince / timeBetweenShots) * damage) + " Damage", chargeDisplayTextStyle);
	}
}
