using UnityEngine;
using System.Collections;

public class MenuArmourScript : MonoBehaviour
{
	private MenuScript menuScript;

	public float rotationSpeed;

	public string armourName;
	public string armourDescription;
	//public float armourStats;
	public float defense;
	public float speed;

	public string menuDefense;
	public string menuSpeed;

	public enum RotationDirection
	{
		Left = 0,
		Right = 1
	}
	public RotationDirection rotationDirection = RotationDirection.Left;

	void Start()
	{
		menuScript = GameObject.FindGameObjectWithTag("GUIObject").GetComponent<MenuScript>();
	}

	void Update()
	{
		if(menuScript.displayingArmour == true)
		{
			renderer.enabled = true;

			if(rotationDirection == RotationDirection.Left)
			{
				transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + (rotationSpeed * Time.deltaTime), transform.eulerAngles.z);
			}
			else if(rotationDirection == RotationDirection.Right)
			{
				transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - (rotationSpeed * Time.deltaTime), transform.eulerAngles.z);
			}
		}
		else
		{
			renderer.enabled = false;
		}
	}
}
