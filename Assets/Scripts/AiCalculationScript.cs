using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <-AI Script->
/// Try playing around with the values to get an understanding
/// of what needs to be changed.
/// 
/// <-To Use->
/// Add this script to wherever you need it. From another
/// script, call the CalculatePath() with a destination, and it
/// will return a set of waypoints.
/// 
/// <-Important Stuff to Know->
/// -This script, when fed a destination, heightOffGround(if not
/// necessary, just send a 0), and CalculatePath() is called, 
/// will return a list of waypoints to get to the destination.
/// -The heightOffGround value is an extra, and it is added to the
/// node height for things like flying enemies flying over a wall
/// unlike their grounded counterparts, who go around it. This value
/// should probably be your enemy/player's height off of the ground
/// - your nodes' rough height off of the ground. heightOffGround 
/// -This script will return a List<Vector3> of the waypoints
/// when it has figured out the best route. If there is no
/// path, it returns an empty list.
/// -If using in a patrol script, calculate the path between
/// patrol points at the start, and any time the map changes,
/// then iterate through the waypoints forwards and backwards
/// when necessary.
/// 
/// By Zacchary Dempsey-Plante

public class AiCalculationScript : MonoBehaviour
{
	//important base stuff
	private List<Transform> nodes = new List<Transform>();
	private List<Transform> openNodes = new List<Transform>();
	private List<Transform> closedNodes = new List<Transform>();
	private List<Transform> currentNodes = new List<Transform>();
	private List<Transform> pathLeaders = new List<Transform>();
	//public List<Transform> startingNodes = new List<Transform>();
	//public List<List<Transform>> paths = new List<List<Transform>>();
	//public List<float> pathScores = new List<float>();
	public string nodeTag;
	public LayerMask terrainLayer;
	public LayerMask layersToCastOn;
	public List<Vector3> waypoints;

	public enum StorageType
	{
		Singular = 0,
		Cloud = 1
	}
	public StorageType nodeStorageType = StorageType.Singular;

	public GameObject nodeRepStorage;
	public static List<Transform> freeReps = new List<Transform>();

	public float averageNodeHeightOffGround; //useless if calculateANHoG is true
	public bool calculateA_N_H_o_G = true;
	private bool calculatedA_N_H_o_G = false;

	private bool calculating = false;

	public bool debugMode = false; //only on when debugging
	public Transform debugDestinationObject;
	public float debugHeightOffGround;

	void Update()
	{
		if(Input.GetMouseButtonDown(0) && debugMode == true)
		{
			CalculatePath(debugDestinationObject.position, debugHeightOffGround);
		}
	}

	void InitializeStorage()
	{
		if(nodeStorageType == StorageType.Cloud)
		{
			GameObject storage = GameObject.Find("NodeRepStorage");
			if(storage == null)
			{
				GameObject cloudStorage = new GameObject("NodeRepStorage");
				cloudStorage.transform.position = Vector3.zero;
				nodeRepStorage = cloudStorage;
			}
			else
			{
				nodeRepStorage = storage;
			}
		}
	}

	public List<Vector3> CalculatePath(Vector3 destination, float heightOffGround)
	{
		if(calculating == false)
		{
			//print(heightOffGround - averageNodeHeightOffGround);
			calculating = true;
			InitializeStorage();
			if(nodeStorageType == StorageType.Cloud && nodeRepStorage != null)
			{
				//freeReps.Clear();
				Transform[] children = nodeRepStorage.GetComponentsInChildren<Transform>();
				if(children.Length > 0)
				{
					foreach(Transform nodeRep in children)
					{
						if(nodeRep != nodeRepStorage.transform && freeReps.Contains(nodeRep) == false)
						{
							freeReps.Add(nodeRep);
						}
					}
				}
			}
			nodes.Clear();
			foreach(GameObject node in GameObject.FindGameObjectsWithTag(nodeTag))
			{
				if(nodeStorageType == StorageType.Singular)
				{
					if(node.GetComponent<NodeScript>() != null)
					{
						node.GetComponent<NodeScript>().previous = null;
						node.GetComponent<NodeScript>().count = 0;
					}
				}
				else if(nodeStorageType == StorageType.Cloud)
				{
					if(node.GetComponent<NodeScript>() != null)
					{
						Destroy(node.GetComponent<NodeScript>());
					}
				}
				nodes.Add(node.transform);
			}
			if(calculateA_N_H_o_G == true && calculatedA_N_H_o_G == false)
			{
				calculatedA_N_H_o_G = true;
				float heights = 0.0f;
				for(int i = 0; i < nodes.Count; i++)
				{
					RaycastHit hit;
					Vector3 hitPoint = new Vector3(0, -500, 0);
					if(Physics.Raycast(nodes[i].transform.position, Vector3.down, out hit, Mathf.Infinity, terrainLayer))
					{
						hitPoint = hit.point;
					}
					else if(Physics.Raycast(nodes[i].transform.position, Vector3.up, out hit))
					{
						hitPoint = hit.point;
					}
					if(hitPoint != new Vector3(0, -500, 0))
					{
						heights += (nodes[i].position.y - hitPoint.y);
					}
				}
				averageNodeHeightOffGround = heights / nodes.Count;
			}
			currentNodes.Clear();
			currentNodes.Add(transform);
			openNodes = nodes;
			closedNodes.Clear();
			pathLeaders.Clear();

			while(currentNodes.Count > 0)
			{
				List<Transform> newCurrentNodes = new List<Transform>();
				List<Transform> newClosedNodes = new List<Transform>();
				for(int i = 0; i < currentNodes.Count; i++)
				{
					Vector3 cPos = new Vector3(currentNodes[i].position.x, (currentNodes[i].position.y - averageNodeHeightOffGround) + heightOffGround, currentNodes[i].position.z);
					for(int ii = 0; ii < openNodes.Count; ii++)
					{
						Vector3 oPos = new Vector3(openNodes[ii].position.x, (openNodes[ii].position.y - averageNodeHeightOffGround) + heightOffGround, openNodes[ii].position.z);
						RaycastHit hit;
						if(Physics.Raycast(cPos, oPos - cPos, out hit, (oPos - cPos).magnitude, layersToCastOn))
						{}
						else
						{
							//double waypoints are caused by the obstacle raycast saving it as a previous then dest. raycast setting it again. might be solvable by casting to dest. before all nodes.

							if(nodeStorageType == StorageType.Singular)
							{
								newCurrentNodes.Add(openNodes[ii]);
								if(openNodes[ii].GetComponent<NodeScript>() == null)
								{
									openNodes[ii].gameObject.AddComponent<NodeScript>();
								}
								openNodes[ii].gameObject.GetComponent<NodeScript>().previous = currentNodes[i];
								openNodes[ii].gameObject.GetComponent<NodeScript>().count = (currentNodes[i].position - openNodes[ii].position).magnitude;
							}
							else if(nodeStorageType == StorageType.Cloud)
							{
								Transform nodeRep = null;
								if(freeReps.Count <= 0)
								{
									GameObject newNodeRep = new GameObject("NodeRep");
									newNodeRep.transform.position = openNodes[ii].position;
									newNodeRep.transform.parent = nodeRepStorage.transform;
									newNodeRep.AddComponent<NodeScript>();
									nodeRep = newNodeRep.transform;
								}
								else
								{
									nodeRep = freeReps[0];
									freeReps.Remove(nodeRep);
								}

								nodeRep.GetComponent<NodeScript>().previous = currentNodes[i];
								nodeRep.GetComponent<NodeScript>().count = (currentNodes[i].position - openNodes[ii].position).magnitude;
								newCurrentNodes.Add(nodeRep);
							}
							//currentNodes[i].GetComponent<NodeScript>().nextNodes.Add(openNodes[ii]);
							if(debugMode == true)
							{
								Debug.DrawRay(new Vector3(currentNodes[i].position.x, (currentNodes[i].position.y - averageNodeHeightOffGround) + heightOffGround, currentNodes[i].position.z), oPos - cPos, Color.black, 2.0f);
							}
							newClosedNodes.Add(openNodes[ii]);
						}
					}
					Vector3 dPos = new Vector3(destination.x, (destination.y - averageNodeHeightOffGround) + heightOffGround, destination.z);
					RaycastHit hit2;
					if(Physics.Raycast(cPos, dPos - cPos, out hit2, (dPos - cPos).magnitude, layersToCastOn))
					{}
					else
					{
						//print("Found destination!");
						if(debugMode == true)
						{
							Debug.DrawRay(new Vector3(currentNodes[i].position.x, (currentNodes[i].position.y - averageNodeHeightOffGround) + heightOffGround, currentNodes[i].position.z), dPos - cPos, Color.green, 2.0f);
						}
						if(nodeStorageType == StorageType.Singular)
						{
							pathLeaders.Add(currentNodes[i]);
						}
						else if(nodeStorageType == StorageType.Cloud)
						{
							Transform nodeRep = null;
							if(freeReps.Count <= 0)
							{
								GameObject newNodeRep = new GameObject("NodeRep");
								newNodeRep.transform.position = currentNodes[i].position; //<-- double waypoint assisting
								newNodeRep.transform.parent = nodeRepStorage.transform;
								newNodeRep.AddComponent<NodeScript>();
								nodeRep = newNodeRep.transform;
							}
							else
							{
								nodeRep = freeReps[0];
								freeReps.Remove(nodeRep);
							}

							nodeRep.GetComponent<NodeScript>().previous = currentNodes[i];
							nodeRep.gameObject.GetComponent<NodeScript>().count = (currentNodes[i].position - destination).magnitude;
							pathLeaders.Add(nodeRep.transform);
						}
					}
				}
				currentNodes = newCurrentNodes;
				for(int i = 0; i < newClosedNodes.Count; i++)
				{
					closedNodes.Add(newClosedNodes[i]);
					openNodes.Remove(newClosedNodes[i]);
				}
			}
			/*if(pathLeaders.Count <= 0)
			{
				print("There is no way to reach the destination.");
			}
			else
			{
				print("There is a way to reach the destination!");
			}*/

			//PlaceWaypoints(requestingObject, requestingScript, destination);

			//Place the waypoints

			List<float> counts = new List<float>();
			for(int i = 0; i < pathLeaders.Count; i++)
			{
				counts.Add((new Vector3(destination.x, 0, destination.z) - new Vector3(pathLeaders[i].position.x, 0, pathLeaders[i].position.z)).magnitude);
				bool foundAll = false;
				Transform previousNode = pathLeaders[i];

				while(foundAll == false)
				{
					if(previousNode != transform)
					{
						counts[i] += previousNode.GetComponent<NodeScript>().count;
						previousNode = previousNode.GetComponent<NodeScript>().previous;
					}
					else
					{
						foundAll = true;
					}
				}
			}
			waypoints.Clear();
			if(pathLeaders.Count > 0)
			{
				float bestCount = Mathf.Infinity;
				int bestIndex = -1;
				for(int i = 0; i < counts.Count; i++)
				{
					if(counts[i] < bestCount)
					{
						bestCount = counts[i];
						bestIndex = i;
					}
					else if(counts[i] == bestCount)
					{
						if(Random.Range(0,2) == 1)
						{
							bestIndex = i;
						}
					}
				}
				bool foundPath = false;
				Transform previousWaypoint = pathLeaders[bestIndex];
				List<Vector3> waypointNodes = new List<Vector3>();
				waypointNodes.Add(new Vector3(destination.x, transform.position.y, destination.z));
				waypointNodes.Add(new Vector3(pathLeaders[bestIndex].position.x, transform.position.y, pathLeaders[bestIndex].position.z));
				
				while(foundPath == false)
				{
					if(previousWaypoint != transform)
					{
						waypointNodes.Add(new Vector3(previousWaypoint.position.x, transform.position.y, previousWaypoint.position.z));
						previousWaypoint = previousWaypoint.GetComponent<NodeScript>().previous;
					}
					else
					{
						foundPath = true;
					}
				}

				if(nodeStorageType == StorageType.Cloud)
				{
					foreach(Transform nodeRep in nodeRepStorage.GetComponentsInChildren<Transform>())
					{
						if(nodeRep != nodeRepStorage)
						{
							//Destroy(nodeRep.gameObject);
							if(nodeRep.GetComponent<NodeScript>() != null)
							{
								nodeRep.position = Vector3.zero;
								nodeRep.GetComponent<NodeScript>().previous = null;
								nodeRep.GetComponent<NodeScript>().count = 0;
							}
							if(freeReps.Contains(nodeRep) == false)
							{
								freeReps.Add(nodeRep);
							}
						}
					}
					//Destroy(nodeRepStorage);
				}

				Vector3 previousWaypointNode = new Vector3(0, -500, 0);
				waypoints.Clear();
				for(int i = waypointNodes.Count - 1; i > -1; i--)
				{
					//check for doubles
					if(waypointNodes[i] != previousWaypointNode && waypointNodes[i] != transform.position)
					{
						waypoints.Add(waypointNodes[i]);
					}
					previousWaypointNode = waypointNodes[i];
				}
				
				if(debugMode == true)
				{
					Vector3 currentPos = transform.position;
					for(int i = 0; i < waypoints.Count; i++)
					{
						Debug.DrawLine(currentPos, waypoints[i], Color.red, 10.0f);
						currentPos = waypoints[i];
					}
				}
			}
		}
		calculating = false;
		if(pathLeaders.Count > 0)
		{
			return waypoints;
		}
		else
		{
			return new List<Vector3>();
		}
	}

	/*void PlaceWaypoints(GameObject requestingObject, string requestingScript, Vector3 destination)
	{
		List<float> counts = new List<float>();
		for(int i = 0; i < pathLeaders.Count; i++)
		{
			counts.Add((new Vector3(destination.x, 0, destination.z) - new Vector3(pathLeaders[i].position.x, 0, pathLeaders[i].position.z)).magnitude);
			bool foundAll = false;
			Transform previousNode = pathLeaders[i];

			while(foundAll == false)
			{
				if(previousNode != transform)
				{
					counts[i] += previousNode.GetComponent<NodeScript>().count;
					previousNode = previousNode.GetComponent<NodeScript>().previous;
				}
				else
				{
					foundAll = true;
				}
			}
		}
		waypoints.Clear();
		if(pathLeaders.Count > 0)
		{
			float bestCount = Mathf.Infinity;
			int bestIndex = -1;
			for(int i = 0; i < counts.Count; i++)
			{
				if(counts[i] < bestCount)
				{
					bestCount = counts[i];
					bestIndex = i;
				}
				else if(counts[i] == bestCount)
				{
					if(Random.Range(0,2) == 1)
					{
						bestIndex = i;
					}
				}
			}
			bool foundPath = false;
			Transform previousWaypoint = pathLeaders[bestIndex];
			List<Vector3> waypointNodes = new List<Vector3>();
			waypointNodes.Add(destination);
			waypointNodes.Add(new Vector3(pathLeaders[bestIndex].position.x, transform.position.y, pathLeaders[bestIndex].position.z));
				
			while(foundPath == false)
			{
				if(previousWaypoint != transform)
				{
					waypointNodes.Add(new Vector3(previousWaypoint.position.x, transform.position.y, previousWaypoint.position.z));
					previousWaypoint = previousWaypoint.GetComponent<NodeScript>().previous;
				}
				else
				{
					foundPath = true;
				}
			}

			waypoints.Clear();
			for(int i = waypointNodes.Count - 1; i > -1; i--)
			{
				waypoints.Add(waypointNodes[i]);
			}

			requestingObject.GetComponent<>(requestingScript).waypoints = waypoints;
				
			if(debugMode == true)
			{
				Vector3 currentPos = transform.position;
				for(int i = 0; i < waypoints.Count; i++)
				{
					Debug.DrawLine(currentPos, waypoints[i], Color.red, 10.0f);
					currentPos = waypoints[i];
				}
			}
		}
	}*/
}
