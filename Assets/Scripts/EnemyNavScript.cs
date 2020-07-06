using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(EnemyAttackScript))]
public class EnemyNavScript : MonoBehaviour
{
	private Transform player;
	public Transform energyCore;
	public float playerDetectionRange;
	public float forgetPlayerRange;
	public float attackRange = 3.0f;
	public float recalculateRange;
	public LayerMask terrainLayer;
	//public int damage;
	//public float attackTime;
	//private float previousAttackTime;
	private float previousCheckTime;
	public float rayCheckDistance;
	//private string target;
	public Vector3 standardRotation;

	public float speed;
	public float ySpeed;
	public float turnSpeed;
	public float waypointLeniency = 0.35f;
	public List<Vector3> waypoints;
	public Vector3 currentDestination;
	public int currentWaypoint;
	public bool achievablePath;

	public float heightOffGround;
	public float timeBetweenHeightChecks;
	private float previousHeightCheckTime;

	private float currentYPosition;

	public int moneyToDrop;

	public Texture alienGUITexture;

	public Object testCube;
	public Object testCubeClosest;

	void Start()
	{
		energyCore = GameObject.FindGameObjectWithTag("EnergyCore").transform;
		GetComponent<EnemyAttackScript>().ChangeTarget(energyCore.gameObject);
		waypoints = GetComponent<AiCalculationScript>().CalculatePath(energyCore.position, heightOffGround);
		if(waypoints.Count > 0)
		{
			achievablePath = true;
			currentWaypoint = 0;
		}
		else
		{
			achievablePath = false;
			currentWaypoint = 0;
		}
		currentWaypoint = 0;
	}
	
	void Update()
	{
		if(player == null)
		{
			player = GameObject.FindGameObjectWithTag("Player").transform;
		}
		if(achievablePath == false && GetComponent<EnemyAttackScript>().target != energyCore.gameObject)
		{
			List<Vector3> checkWaypoints = GetComponent<AiCalculationScript>().CalculatePath(player.position, heightOffGround);
			if(checkWaypoints.Count <= 0)
			{
				GetComponent<EnemyAttackScript>().ChangeTarget(energyCore.gameObject);
				waypoints = GetComponent<AiCalculationScript>().CalculatePath(energyCore.position, heightOffGround);
			}
			else
			{
				waypoints = checkWaypoints;
			}
		}
		if(GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().tDefenceOn == false && GameObject.FindGameObjectWithTag("SpawnObject").GetComponent<AlienSpawnScript>().createRoundGUIs == false)
		{
			if(previousHeightCheckTime < (Time.time + timeBetweenHeightChecks))
			{
				previousHeightCheckTime = Time.time;
				RaycastHit hit;
				if(Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, terrainLayer))
				{
					currentYPosition = hit.point.y + heightOffGround;
				}
				else if(Physics.Raycast(transform.position, Vector3.up, out hit, Mathf.Infinity, terrainLayer))
				{
					currentYPosition = hit.point.y + heightOffGround;
				}
				else
				{
					Debug.LogWarning("Terrain not found. " + gameObject);
				}
			}

			//do navigation stuff
			if((energyCore.position - transform.position).magnitude > forgetPlayerRange)
			{
				if((player.position - transform.position).magnitude <= playerDetectionRange)
				{
					if(waypoints.Count > 0)
					{
						if((waypoints[waypoints.Count - 1] - player.position).magnitude >= recalculateRange)
						{
							waypoints = GetComponent<AiCalculationScript>().CalculatePath(player.position, heightOffGround);
							if(GetComponent<EnemyAttackScript>().target != player.gameObject)
							{
								GetComponent<EnemyAttackScript>().ChangeTarget(player.gameObject);
							}
							if(waypoints.Count > 0)
							{
								achievablePath = true;
								currentWaypoint = 0;
							}
							else
							{
								achievablePath = false;
								currentWaypoint = 0;
							}
							//SetDestination(player.position);
							//print("Player has moved, and/or is in sight. Setting new destination.");
						}
					}
				}
				else
				{
					if(waypoints.Count > 0)
					{
						if(waypoints[waypoints.Count - 1] != new Vector3(energyCore.position.x, waypoints[waypoints.Count - 1].y, energyCore.position.z))
						{
							waypoints = GetComponent<AiCalculationScript>().CalculatePath(energyCore.position, heightOffGround);
							if(GetComponent<EnemyAttackScript>().target != energyCore.gameObject)
							{
								GetComponent<EnemyAttackScript>().ChangeTarget(energyCore.gameObject);
							}
							if(waypoints.Count > 0)
							{
								achievablePath = true;
								currentWaypoint = 0;
							}
							else
							{
								achievablePath = false;
								currentWaypoint = 0;
							}
							//SetDestination(energyCore.position);
							//print("Player is not in sight, setting new destination.");
						}
					}
				}
			}
			else
			{
				if(waypoints.Count > 0)
				{
					if(waypoints[waypoints.Count - 1] != new Vector3(energyCore.position.x, waypoints[waypoints.Count - 1].y, energyCore.position.z))
					{
						waypoints = GetComponent<AiCalculationScript>().CalculatePath(energyCore.position, heightOffGround);
						if(GetComponent<EnemyAttackScript>().target != energyCore.gameObject)
						{
							GetComponent<EnemyAttackScript>().ChangeTarget(energyCore.gameObject);
						}
						if(waypoints.Count > 0)
						{
							achievablePath = true;
							currentWaypoint = 0;
						}
						else
						{
							achievablePath = false;
							currentWaypoint = 0;
						}
					}
				}
			}

			Vector3 destinationPos = new Vector3(0, -500, 0);
			if(achievablePath == true)
			{
				/*if(currentDestination == Vector3.zero)
				{
					currentDestination = destination;
				}
				else
				{*/
				if((new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(waypoints[currentWaypoint].x, 0, waypoints[currentWaypoint].z)).magnitude <= waypointLeniency)
				{
					if(currentWaypoint < (waypoints.Count - 1))
					{
						currentWaypoint++;
					}
				}
				//}
				//if(currentDestination == Vector3.zero)
				//{

				if(currentWaypoint < (waypoints.Count - 1) || (currentWaypoint == (waypoints.Count - 1) && (new Vector3(waypoints[currentWaypoint].x, 0, waypoints[currentWaypoint].z) - new Vector3(transform.position.x, 0, transform.position.z)).magnitude > attackRange))
				{
					destinationPos = /*Vector3.MoveTowards(transform.position, */new Vector3(waypoints[currentWaypoint].x, currentYPosition, waypoints[currentWaypoint].z)/*, speed * Time.deltaTime)*/;
				}
				else
				{
					destinationPos = /*Vector3.MoveTowards(transform.position, */new Vector3(transform.position.x, currentYPosition, transform.position.z)/*, 0.0f * Time.deltaTime)*/;
					/*if(currentWaypoint == (waypoints.Count - 1) && (new Vector3(waypoints[currentWaypoint].x, 0, waypoints[currentWaypoint].z) - new Vector3(transform.position.x, 0, transform.position.z)).magnitude <= attackRange)
					{
						GetComponent<EnemyAttackScript>().canAttack = true;
					}*/
				}
				//}
				//else
				//{
				//	transform.position = Vector3.MoveTowards(transform.position, new Vector3(currentDestination.x, currentYPosition, currentDestination.z), speed * Time.deltaTime);
				//}
			}
			if(destinationPos == new Vector3(0, -500, 0))
			{
				destinationPos = transform.position;
			}
			destinationPos.y = currentYPosition;
			transform.position = Vector3.MoveTowards(transform.position, new Vector3(destinationPos.x, transform.position.y, destinationPos.z), speed * Time.deltaTime);
			transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, destinationPos.y, transform.position.z), ySpeed * Time.deltaTime);
			if(waypoints.Count > 0)
			{
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(Quaternion.LookRotation(new Vector3(waypoints[currentWaypoint].x, 0, waypoints[currentWaypoint].z) - new Vector3(transform.position.x, 0, transform.position.z)).eulerAngles + standardRotation), turnSpeed * Time.deltaTime);
			}

			/*if(Time.time > (previousCheckTime + timeBetweenRayChecks))
			{
				previousCheckTime = Time.time;
				RaycastHit hit;
				if(Physics.Raycast(transform.position, transform.forward, out hit, rayCheckDistance))
				{
					if(hit.transform.tag == "Object")
					{
						Transform obstacle = hit.transform;
						Vector2 cornerLL = new Vector2((obstacle.collider.bounds.extents.x) + obstacle.position.x, (obstacle.collider.bounds.extents.z) + obstacle.position.z);
						Vector2 cornerLR = new Vector2((obstacle.collider.bounds.extents.x) + obstacle.position.x, -(obstacle.collider.bounds.extents.z) + obstacle.position.z);
						Vector2 cornerRL = new Vector2(-(obstacle.collider.bounds.extents.x) + obstacle.position.x, (obstacle.collider.bounds.extents.z) + obstacle.position.z);
						Vector2 cornerRR = new Vector2(-(obstacle.collider.bounds.extents.x) + obstacle.position.x, -(obstacle.collider.bounds.extents.z) + obstacle.position.z);
						
						//for testing
						for(int i = 0; i < 4; i++)
						{
							switch(i)
							{
								case 0:
								GameObject test1 = (GameObject) Instantiate(testCube, new Vector3(cornerLL.x, obstacle.position.y, cornerLL.y), Quaternion.identity);
								test1.name = "CubeLL";
									break;
								case 1:
								GameObject test2 = (GameObject) Instantiate(testCube, new Vector3(cornerLR.x, obstacle.position.y, cornerLR.y), Quaternion.identity);
								test2.name = "CubeLR";
									break;
								case 2:
								GameObject test3 = (GameObject) Instantiate(testCube, new Vector3(cornerRL.x, obstacle.position.y, cornerRL.y), Quaternion.identity);
								test3.name = "CubeRL";
									break;
								case 3:
								GameObject test4 = (GameObject) Instantiate(testCube, new Vector3(cornerRR.x, obstacle.position.y, cornerRR.y), Quaternion.identity);
								test4.name = "CubeRR";
									break;
							}
						}

						//find out which corner is closest to the enemy, then figure out which one(on either side of the closest one) is closest to the direct route to the destination. place waypoints around it with bounds.extents.whatever + obstacleAvoidanceAmount
						float closestDistance = Mathf.Infinity;
						string closestCorner = "";
						if((new Vector3(cornerLL.x, 0, cornerLL.y) - new Vector3(transform.position.x, 0, transform.position.z)).magnitude < closestDistance)
						{
							closestDistance = (new Vector3(cornerLL.x, 0, cornerLL.y) - new Vector3(transform.position.x, 0, transform.position.z)).magnitude;
							closestCorner = "CornerLL";
						}
						if((new Vector3(cornerLR.x, 0, cornerLR.y) - new Vector3(transform.position.x, 0, transform.position.z)).magnitude < closestDistance)
						{
							closestDistance = (new Vector3(cornerLR.x, 0, cornerLR.y) - new Vector3(transform.position.x, 0, transform.position.z)).magnitude;
							closestCorner = "CornerLR";
						}
						if((new Vector3(cornerRL.x, 0, cornerRL.y) - new Vector3(transform.position.x, 0, transform.position.z)).magnitude < closestDistance)
						{
							closestDistance = (new Vector3(cornerRL.x, 0, cornerRL.y) - new Vector3(transform.position.x, 0, transform.position.z)).magnitude;
							closestCorner = "CornerRL";
						}
						if((new Vector3(cornerRR.x, 0, cornerRR.y) - new Vector3(transform.position.x, 0, transform.position.z)).magnitude < closestDistance)
						{
							closestDistance = (new Vector3(cornerRR.x, 0, cornerRR.y) - new Vector3(transform.position.x, 0, transform.position.z)).magnitude;
							closestCorner = "CornerRR";
						}

						switch(closestCorner)
						{
							case "CornerLL":

							GameObject testClosest1 = (GameObject) Instantiate(testCubeClosest, new Vector3(cornerLL.x, obstacle.position.y, cornerLL.y), Quaternion.identity);
							testClosest1.name = "CubeClosestLL";

							//cornerLR && cornerRL
							Vector3 directRoute1 = new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(destination.x, 0, destination.z);
							Vector3 corner1Route1 = new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(cornerLR.x, 0, cornerLR.y);
							Vector3 corner2Route1 = new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(cornerRL.x, 0, cornerRL.y);
							if((directRoute1 - corner1Route1).magnitude < (directRoute1 - corner2Route1).magnitude)
							{
								//cornerLR
								PlaceWaypoints(obstacle, new Vector3(cornerLR.x + obstacleAvoidanceAmount, 0, cornerLR.y - obstacleAvoidanceAmount), "cornerLR");
							}
							else
							{
								//cornerRL
								PlaceWaypoints(obstacle, new Vector3(cornerRL.x - obstacleAvoidanceAmount, 0, cornerRL.y + obstacleAvoidanceAmount), "cornerRL");
							}
								break;
							case "CornerLR":

							GameObject testClosest2 = (GameObject) Instantiate(testCubeClosest, new Vector3(cornerLR.x, obstacle.position.y, cornerLR.y), Quaternion.identity);
							testClosest2.name = "CubeClosestLR";

							//cornerLL && cornerRR
							Vector3 directRoute2 = new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(destination.x, 0, destination.z);
							Vector3 corner1Route2 = new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(cornerLL.x, 0, cornerLL.y);
							Vector3 corner2Route2 = new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(cornerRR.x, 0, cornerRR.y);
							if((directRoute2 - corner1Route2).magnitude < (directRoute2 - corner2Route2).magnitude)
							{
								//cornerLL
								PlaceWaypoints(obstacle, new Vector3(cornerLL.x + obstacleAvoidanceAmount, 0, cornerLL.y + obstacleAvoidanceAmount), "cornerLL");
							}
							else
							{
								//cornerRR
								PlaceWaypoints(obstacle, new Vector3(cornerRR.x - obstacleAvoidanceAmount, 0, cornerRR.y - obstacleAvoidanceAmount), "cornerRR");
							}
								break;
							case "CornerRL":

							GameObject testClosest3 = (GameObject) Instantiate(testCubeClosest, new Vector3(cornerRL.x, obstacle.position.y, cornerRL.y), Quaternion.identity);
							testClosest3.name = "CubeClosestRL";

							//cornerLL && cornerRR
							Vector3 directRoute3 = new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(destination.x, 0, destination.z);
							Vector3 corner1Route3 = new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(cornerLL.x, 0, cornerLL.y);
							Vector3 corner2Route3 = new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(cornerRR.x, 0, cornerRR.y);
							if((directRoute3 - corner1Route3).magnitude < (directRoute3 - corner2Route3).magnitude)
							{
								//cornerLL
								PlaceWaypoints(obstacle, new Vector3(cornerLL.x + obstacleAvoidanceAmount, 0, cornerLL.y - obstacleAvoidanceAmount), "cornerLL");
							}
							else
							{
								//cornerRR
								PlaceWaypoints(obstacle, new Vector3(cornerRR.x - obstacleAvoidanceAmount, 0, cornerRR.y + obstacleAvoidanceAmount), "cornerRR");
							}
								break;
							case "CornerRR":

							GameObject testClosest4 = (GameObject) Instantiate(testCubeClosest, new Vector3(cornerRR.x, obstacle.position.y, cornerRR.y), Quaternion.identity);
							testClosest4.name = "CubeClosestRR";

							//cornerLR && cornerRL
							Vector3 directRoute4 = new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(destination.x, 0, destination.z);
							Vector3 corner1Route4 = new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(cornerLR.x, 0, cornerLR.y);
							Vector3 corner2Route4 = new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(cornerRL.x, 0, cornerRL.y);
							if((directRoute4 - corner1Route4).magnitude < (directRoute4 - corner2Route4).magnitude)
							{
								//cornerLR
								PlaceWaypoints(obstacle, new Vector3(cornerLR.x + obstacleAvoidanceAmount, 0, cornerLR.y + obstacleAvoidanceAmount), "cornerLR");
							}
							else
							{
								//cornerRL
								PlaceWaypoints(obstacle, new Vector3(cornerRL.x - obstacleAvoidanceAmount, 0, cornerRL.y - obstacleAvoidanceAmount), "cornerRL");
							}
								break;
						}
						//print(transform.forward + " : " + (transform.forward + new Vector3(transform.forward.x + 35, 0, 0)).magnitude + " : " + (transform.forward + new Vector3(transform.forward.x + 14, 0, 0)).magnitude);
					}
				}
			}*/
		}
	}

	/*void PlaceWaypoints(Transform obstacleToAvoid, Vector3 waypoint, string corner)
	{
		currentDestination = waypoint;
		print(waypoint);
		GameObject testWaypoint = (GameObject) Instantiate(testCube, new Vector3(waypoint.x, obstacleToAvoid.position.y, waypoint.z), Quaternion.identity);
		testWaypoint.name = "WaypointTest " + corner;
	}*/
}
