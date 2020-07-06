using UnityEngine;
using System.Collections;

public class RayRemainsSpread : MonoBehaviour
{
	public Color remainsColor;
	public float sizeMax;
	public float sizeChangeIncrement;
	private bool hasHitMax;

	// Use this for initialization
	void Start ()
	{
		renderer.material.color = remainsColor;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().tDefenceOn == false && GameObject.FindGameObjectWithTag("SpawnObject").GetComponent<AlienSpawnScript>().createRoundGUIs == false)
		{
			if(transform.localScale.x < sizeMax && hasHitMax == false)
			{
				transform.localScale = new Vector3(transform.localScale.x + sizeChangeIncrement, transform.localScale.y, transform.localScale.z + sizeChangeIncrement);
				if(transform.localScale.x >= sizeMax)
				{
					hasHitMax = true;
				}
			}
			else if(transform.localScale.x > 0.0f && hasHitMax == true)
			{
				transform.localScale = new Vector3(transform.localScale.x - sizeChangeIncrement, transform.localScale.y, transform.localScale.z - sizeChangeIncrement);
			}
			else if(transform.localScale.x <= 0.0f && hasHitMax == true)
			{
				Destroy(gameObject);
			}
		}
	}
}
