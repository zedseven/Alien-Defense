using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurretSpotScript : MonoBehaviour
{
	public List<Transform> turretSockets;
	public int turretType = -1;
	public List<Transform> currentTurrets;

	public void AssignTurrets(Object turret, int newTurretType)
	{
		turretType = newTurretType;
		for(int i = 0; i < currentTurrets.Count; i++)
		{
			Destroy(currentTurrets[i].gameObject);
		}
		currentTurrets.Clear();
		for(int i = 0; i < turretSockets.Count; i++)
		{
			GameObject newTurret = (GameObject) Instantiate(turret, turretSockets[i].position, turretSockets[i].rotation);
			newTurret.GetComponent<TurretScript>().enemyHolder = GameObject.FindGameObjectWithTag("EnemyHolder").transform;
			newTurret.transform.parent = transform;
			currentTurrets.Add(newTurret.transform);
		}
	}
}
