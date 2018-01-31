using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
	public float lookRadius = 10f;
	const float locomotionAnimationSmootTime = .05f;

    [SerializeField]
	Transform target;
    EnemyNav enemyNav;
    NavMeshAgent agent;
	Animator anim;

	// Use this for initialization
	void Start()
	{
		target = PlayerManager.instance.player.transform;
        enemyNav = GetComponent<EnemyNav>();
        agent = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update()
	{
		float distance = Vector3.Distance(target.position, transform.position);

		if (distance <= lookRadius)
		{
            enemyNav.MoveToTarget(target.position);

			if (distance <= agent.stoppingDistance)
			{
				FaceTarget();
			}
		}

		//walk
		float speedPercent = agent.velocity.magnitude / agent.speed;
		anim.SetFloat("speedPercent", speedPercent, locomotionAnimationSmootTime, Time.deltaTime);

	}

	void FaceTarget()
	{
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
	}

	void OnDrawGizmosSelected()
	{

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, lookRadius);
	}
}