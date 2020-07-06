using UnityEngine;
using System.Collections;

public class CoreSpinner : MonoBehaviour
{
	public float rotationSpeed;

	private GameObject defenceGUIObject;
	private GameObject spawnObject;

	void Start()
	{
		defenceGUIObject = GameObject.FindGameObjectWithTag("GUIObject");
		spawnObject = GameObject.FindGameObjectWithTag("SpawnObject");
	}

	void Update()
	{
		if(defenceGUIObject.GetComponent<DefenceGUIScript>().tDefenceOn == false && spawnObject.GetComponent<AlienSpawnScript>().createRoundGUIs == false)
		{
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + (rotationSpeed * Time.deltaTime), transform.eulerAngles.z);
		}
	}
}
