using UnityEngine;
using System.Collections;

public class WallExplosionScript : MonoBehaviour
{
	private bool hasExploded;

	public float detectionRadius;
	public float detectionAngle;

	public Transform explosive;
	public Object wallPieceObject;
	public Object steamBurstSystem;
	public Object explosionSystem;

	public float explosionForce;
	public float explosionRadius;

	public Transform steamBurstSocket;

	public float timeForSteam;

	public float destroyRigidbodyTime;

	private bool canCountDown;
	private float countdown;

	private string explosionStage = "SteamBurst";

	private GameObject currentSteamObject;
	private GameObject currentWallObject;
	private GameObject currentExplosionObject;

	private Transform player;

	void Update()
	{
		if(player == null)
		{
			player = GameObject.FindGameObjectWithTag("Player").transform;
		}

		float angle = Vector3.Angle(-transform.forward, player.position - transform.position);
		if((player.position - transform.position).magnitude <= detectionRadius && angle >= -detectionAngle && angle <= detectionAngle && hasExploded == false)
		{
			Explode();
			hasExploded = true;
		}

		if(canCountDown == true)
		{
			countdown -= Time.deltaTime;
			if(countdown <= 0.0f)
			{
				canCountDown = false;
				explosionStage = "Explosion";
				Explode();
			}
		}
	}

	void Explode()
	{
		if(explosionStage == "SteamBurst")
		{
			currentSteamObject = (GameObject) Instantiate(steamBurstSystem, steamBurstSocket.position, steamBurstSocket.rotation);
			currentSteamObject.particleSystem.Play();
			countdown = timeForSteam;
			canCountDown = true;
		}
		else if(explosionStage == "Explosion")
		{
			Destroy(currentSteamObject);
			currentWallObject = (GameObject) Instantiate(wallPieceObject, transform.position, transform.rotation);
			renderer.enabled = false;
			collider.enabled = false;
			currentExplosionObject = (GameObject) Instantiate(explosionSystem, explosive.position, explosive.rotation);
			currentExplosionObject.particleSystem.Play();
			foreach(Transform shard in currentWallObject.GetComponentsInChildren<Transform>())
			{
				if(shard != currentWallObject.transform)
				{
					shard.rigidbody.AddExplosionForce(explosionForce, explosive.position, explosionRadius);
					Destroy(shard.rigidbody, destroyRigidbodyTime);
				}
			}
		}
	}
}
