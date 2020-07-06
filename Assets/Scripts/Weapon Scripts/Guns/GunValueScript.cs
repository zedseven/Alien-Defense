using UnityEngine;
using System.Collections;

public class GunValueScript : MonoBehaviour
{
	public Vector3 gunEquipPosition = new Vector3(0.65f, -0.120818f, -0.2142947f);
	public Vector3 gunEquipRotation = new Vector3(0, 90, 0);

	public int damage;
	
	public float timeBetweenShots;
	
	public string gunName;
	public string description;
	public int cost;
	public Texture texture;

	void Start()
	{
		if(GetComponent<GunScript>() != null)
		{
			damage = GetComponent<GunScript>().damage;
			timeBetweenShots = GetComponent<GunScript>().timeBetweenShots;
		}
		else if(GetComponent<AcidGunScript>() != null)
		{
			damage = GetComponent<AcidGunScript>().damage;
			timeBetweenShots = 1 / GetComponent<AcidGunScript>().rateOfFire;
		}
		else if(GetComponent<SniperGunScript>() != null)
		{
			damage = GetComponent<SniperGunScript>().damage;
			timeBetweenShots = GetComponent<SniperGunScript>().timeBetweenShots;
		}
		else if(GetComponent<DualGunScript>() != null)
		{
			damage = GetComponent<DualGunScript>().damage;
			timeBetweenShots = GetComponent<DualGunScript>().timeBetweenShots;
		}
		else if(GetComponent<ShotgunScript>() != null)
		{
			damage = GetComponent<ShotgunScript>().damage * GetComponent<ShotgunScript>().shotsAtOnce;
			timeBetweenShots = GetComponent<ShotgunScript>().timeBetweenShots;
		}
	}
}
