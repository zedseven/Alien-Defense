using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class LightBehaviourScript : MonoBehaviour
{
	public enum BehaviourType
	{
		Flicker = 0,
		Alarm = 1,
		FlickerAndAlarm = 2
	}
	public BehaviourType behaviourType = BehaviourType.FlickerAndAlarm;

	//Flicker Stuff
	public float flickerIntensity;
	public float timeFlickerLasts;
	public float averageFlickerTimeBetween;
	public float flickerRandomizationAmount;

	private float flickerCountdown;
	private bool countingToNextFlicker = true;
	private float previousIntensity;

	//Alarm Stuff
	public float alarmSpeed;
	public float minIntensity;
	public float maxIntensity;

	private bool intensityUp = true;

	//Other Stuff
	public float onRange = 15.0f; //determines at what range from the player the light is turned on 

	private Transform player;
	private GameObject defenceGUIObject;
	private GameObject spawnObject;

	void Start()
	{
		defenceGUIObject = GameObject.FindGameObjectWithTag("GUIObject");
		spawnObject = GameObject.FindGameObjectWithTag("SpawnObject");
	}

	void Update()
	{
		if(player == null)
		{
			player = GameObject.FindGameObjectWithTag("Player").transform;
		}

		if((new Vector3(player.position.x, transform.position.y, player.position.z) - transform.position).magnitude <= onRange)
		{
			if(light.enabled == false)
			{
				light.enabled = true;
			}

			if((behaviourType == BehaviourType.Flicker || behaviourType == BehaviourType.FlickerAndAlarm) && defenceGUIObject.GetComponent<DefenceGUIScript>().tDefenceOn == false && spawnObject.GetComponent<AlienSpawnScript>().createRoundGUIs == false)
			{
				if(countingToNextFlicker == true)
				{
					flickerCountdown -= Time.deltaTime;
					if(flickerCountdown <= 0.0f)
					{
						countingToNextFlicker = false;
						flickerCountdown = timeFlickerLasts;
						previousIntensity = light.intensity;
						light.intensity = flickerIntensity;
					}
				}
				else
				{
					flickerCountdown -= Time.deltaTime;
					if(flickerCountdown <= 0.0f)
					{
						countingToNextFlicker = true;
						flickerCountdown = averageFlickerTimeBetween + Random.Range(-flickerRandomizationAmount, flickerRandomizationAmount);
						light.intensity = previousIntensity;
					}
				}
			}

			if((behaviourType == BehaviourType.Alarm || behaviourType == BehaviourType.FlickerAndAlarm) && defenceGUIObject.GetComponent<DefenceGUIScript>().tDefenceOn == false && spawnObject.GetComponent<AlienSpawnScript>().createRoundGUIs == false)
			{
				if(light.intensity <= minIntensity)
				{
					intensityUp = true;
				}
				else if(light.intensity >= maxIntensity)
				{
					intensityUp = false;
				}

				if(intensityUp == false)
				{
					light.intensity -= (alarmSpeed * (maxIntensity - minIntensity)) * Time.deltaTime;
				}
				else
				{
					light.intensity += (alarmSpeed * (maxIntensity - minIntensity)) * Time.deltaTime;
				}
			}
		}
		else
		{
			if(light.enabled == true)
			{
				light.enabled = false;
			}
		}
	}
}
