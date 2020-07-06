using UnityEngine;
using System.Collections;

public class AcidGunScript : MonoBehaviour
{
	public Transform barrel;

	public int damage;

	public float rateOfFire;

	private bool isSpraying = false;
	public bool isSample = false;

	void Start()
	{
		if(isSample == true)
		{
			rateOfFire = barrel.particleSystem.emissionRate;
			GetComponent<AcidGunScript>().enabled = false;
		}
	}
	
	void Update()
	{
		if(GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().tDefenceOn == false && GameObject.FindGameObjectWithTag("SpawnObject").GetComponent<AlienSpawnScript>().createRoundGUIs == false)
		{
			if(audio.isPlaying == false)
			{
				audio.Play();
			}
			if(barrel.particleSystem.isPaused == true)
			{
				if(isSpraying == true)
				{
					barrel.particleSystem.Play();
				}
				else
				{
					barrel.particleSystem.Play();
					barrel.particleSystem.Stop();
				}
			}
			//GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
			//transform.localEulerAngles = new Vector3(camera.transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
			
			if(Input.GetMouseButton(0))
			{
				if(audio.isPlaying == false)
				{
					audio.Play();
				}
				if(isSpraying == false)
				{
					isSpraying = true;
					barrel.particleSystem.Play();
				}

				/*//print("You hit " + hit.collider.name + ".");
				if(hit.collider.tag == "Enemy")
				{
					//damage enemy;
					hit.collider.GetComponent<HealthScript>().TakeDamage(damage);
				}
				else if(hit.collider.tag == "Object" || hit.collider.tag == "Terrain")
				{
					//add bullet remains;
					GameObject rayRemains = (GameObject) Instantiate(bulletRemains, hit.point, Quaternion.LookRotation(Vector3.up, hit.normal));
					rayRemains.GetComponent<RayRemainsSpread>().remainsColor = rayColor;
				}*/
			}
			else
			{
				if(audio.isPlaying == true)
				{
					audio.Pause();
				}
				if(isSpraying == true)
				{
					isSpraying = false;
					barrel.particleSystem.Stop();
				}
			}
		}
		else if(barrel.particleSystem.isPlaying == true)//isSpraying == true)// && barrel.particleSystem.isPaused == false)
		{
			audio.Pause();
			barrel.particleSystem.Pause();
		}
	}
}
