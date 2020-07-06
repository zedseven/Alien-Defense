using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(EnemyNavScript))]
public class EnemyAttackScript : MonoBehaviour
{
	public int damage;

	public bool randomizeDamage;
	public int randomAmount;

	public float timeBetweenAttacks;
	private float countdown;

	public bool canAttack;
	public GameObject target;

	void Update()
	{
		if(GameObject.FindGameObjectWithTag("GUIObject").GetComponent<DefenceGUIScript>().tDefenceOn == false && GameObject.FindGameObjectWithTag("SpawnObject").GetComponent<AlienSpawnScript>().createRoundGUIs == false)
		{
			if(canAttack && target != null && (new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z) - transform.position).magnitude <= GetComponent<EnemyNavScript>().attackRange)
			{
				if(countdown <= 0.0f)
				{
					countdown = timeBetweenAttacks;
					//deal the damage
					int damageToDeal = damage;
					if(randomizeDamage == true)
					{
						damageToDeal = Random.Range(damage - randomAmount, damage + randomAmount);
					}
					target.GetComponent<HealthScript>().TakeDamage(damageToDeal);

					//play the animation...
				}
				else
				{
					countdown -= Time.deltaTime;
				}
			}
		}
	}

	public void ChangeTarget(GameObject newTarget)
	{
		target = newTarget;
		canAttack = true;
		countdown = 0;
	}
}
