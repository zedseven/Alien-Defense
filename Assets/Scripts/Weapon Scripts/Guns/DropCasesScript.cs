using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropCasesScript : MonoBehaviour
{
	public List<Transform> cases;
	public float timeBeforeNextSpawn;
	public float timeToDestroyCase;
	private bool reloading;

	public Object caseToMake;
	public Vector3 caseLocalPos;
	public Vector3 caseLocalRot;
	private int numOfCases;

	public List<Transform> caseParents;
	
	public float reloadCountdown;

	public void DropCases(float timeToNextSpawn)
	{
		for(int i = 0; i < cases.Count; i++)
		{
			cases[i].parent = GameObject.FindGameObjectWithTag("GUIObject").transform.parent;
			cases[i].name = "CaseRemains";
			cases[i].collider.enabled = true;
			cases[i].gameObject.AddComponent<Rigidbody>();
			Destroy(cases[i].gameObject, timeToDestroyCase);
		}
		cases.Clear();
		reloadCountdown = 0.0f;
		reloading = true;
		timeBeforeNextSpawn = timeToNextSpawn;
	}

	void Start()
	{
		//caseToMake = cases[0].gameObject;
		caseLocalPos = cases[0].localPosition;
		caseLocalRot = cases[0].localEulerAngles;
		numOfCases = cases.Count;
	}

	void Update()
	{
		if(reloading == true && GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().tDefenceOn == false && GameObject.FindGameObjectWithTag("SpawnObject").GetComponent<AlienSpawnScript>().createRoundGUIs == false)
		{
			reloadCountdown += Time.deltaTime;
		}
		//print(destroyTime + " : " + timeBeforeNextSpawn + " : " + (destroyTime + timeBeforeNextSpawn) + " : " + (Time.time >= (destroyTime + timeBeforeNextSpawn)));
		if(reloading == true && reloadCountdown >= timeBeforeNextSpawn && cases.Count < 1)
		{
			reloading = false;
			for(int i = 0; i < numOfCases; i++)
			{
				GameObject newCase = (GameObject) Instantiate(caseToMake, caseLocalPos, Quaternion.Euler(caseLocalRot));
				newCase.transform.parent = caseParents[i];
				newCase.transform.localPosition = caseLocalPos;
				newCase.transform.localEulerAngles = caseLocalRot;
				if(newCase.GetComponent<Rigidbody>() != null)
				{
					Destroy(newCase.GetComponent<Rigidbody>());
				}
				newCase.name = "BatteryPack";
				cases.Add(newCase.transform);
			}
			GetComponent<DualGunScript>().shotsLeft = GetComponent<DualGunScript>().clipSize;
		}
	}
}
