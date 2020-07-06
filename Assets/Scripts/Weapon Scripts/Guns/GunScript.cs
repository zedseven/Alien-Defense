using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GunScript : MonoBehaviour
{
	public List<Transform> barrels;
	private int currentBarrelToUse = 0;
	public Object bulletRemains;
	public Color rayColor;

	public Object rayRenderer;
	public float timeToDestroyRay;
	public float rayWidth;

	public LayerMask layersToCastOn;

	public int damage;
	public float timeBetweenShots;
	private float lastShotTime;
	public float shotOffset;

	public AudioClip shotSound;
	
	void Update()
	{
		if(GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().tDefenceOn == false && GameObject.FindGameObjectWithTag("SpawnObject").GetComponent<AlienSpawnScript>().createRoundGUIs == false)
		{
			//GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
			//transform.localEulerAngles = new Vector3(camera.transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
				
			if(Input.GetMouseButton(0))
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
					Vector3 dir = currentBarrel.forward + new Vector3(Random.Range(-shotOffset, shotOffset), Random.Range(-shotOffset, shotOffset), 0);
					RaycastHit hit;
					if(Physics.Raycast(currentBarrel.position, dir, out hit, Mathf.Infinity, layersToCastOn))
					{
						GameObject ray = (GameObject) Instantiate(rayRenderer, currentBarrel.position, transform.rotation);
						ray.GetComponent<LineRenderer>().SetColors(rayColor, rayColor);
						ray.renderer.material.color = rayColor;
						ray.GetComponent<LineRenderer>().SetPosition(0, currentBarrel.position);
						ray.GetComponent<LineRenderer>().SetPosition(1, hit.point);
						ray.GetComponent<LineRenderer>().SetWidth(rayWidth, rayWidth);
						ray.GetComponent<LineRenderer>().enabled = true;
						ray.name = "Ray";
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
