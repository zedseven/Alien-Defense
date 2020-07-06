using UnityEngine;
using System.Collections;
[ExecuteInEditMode]

/// This script is purely a setup script, not an actual game script.
/// All it does is position all nodes in the scene at a certain 
/// height off the ground, then you just copy and paste the results.

public class NodeHeightSet : MonoBehaviour
{
	public string nodeTag;
	public string terrainTag;

	public float heightOffGround;

	public bool setHeight;

	void Update()
	{
		if(setHeight == true)
		{
			foreach(GameObject node in GameObject.FindGameObjectsWithTag(nodeTag))
			{
				RaycastHit hit;
				Vector3 hitPoint = new Vector3(0, -500, 0);
				if(Physics.Raycast(node.transform.position, Vector3.down, out hit))
				{
					if(hit.transform.tag == terrainTag)
					{
						hitPoint = hit.point;
					}
				}
				if(hitPoint == new Vector3(0, -500, 0))
				{
					if(Physics.Raycast(node.transform.position, Vector3.up, out hit))
					{
						if(hit.transform.tag == terrainTag)
						{
							hitPoint = hit.point;
						}
					}
				}
				if(hitPoint != new Vector3(0, -500, 0))
				{
					node.transform.position = new Vector3(node.transform.position.x, hitPoint.y + heightOffGround, node.transform.position.z);
				}
				else
				{
					Debug.LogError("Terrain Couldn't be found. Please check the tag on the terrain/ground, and the terrainTag value on the script as well.");
				}
			}
			setHeight = false;
		}
	}
}
