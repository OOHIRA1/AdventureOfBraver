using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//機能：エネミーの動きを管理するスクリプト
//
//アタッチ：エネミーにアタッチ
public class EnemyController : MonoBehaviour
{
	public float lookRadius = 10f;
	const float locomotionAnimationSmootTime = .05f;

    [SerializeField]
	Transform target;
    EnemyNav enemyNav;
    NavMeshAgent agent;
	Animator anim;

	public float _viewAngle;
	public GameObject _target;

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

		Vector3 a = transform.position + transform.forward * lookRadius;
		Vector3 b = a + transform.right * lookRadius * Mathf.Tan (_viewAngle / 2);
		Vector3 c = a + (-transform.right) * lookRadius * Mathf.Tan (_viewAngle / 2);
		Vector3 pos = _target.transform.position - transform.position;
		Vector3 localpos = transform.InverseTransformPoint (_target.transform.position);

		/*if (distance <= lookRadius)
		{*/
		if (localpos.z < transform.InverseTransformPoint (a).z && localpos.z >= transform.InverseTransformPoint (c).z / transform.InverseTransformPoint (c).x * localpos.x && localpos.z >= transform.InverseTransformPoint (b).z / transform.InverseTransformPoint (b).x * localpos.x) {

            enemyNav.MoveToTarget(target.position);

			if (distance <= agent.stoppingDistance)
			{
				FaceTarget();
				Debug.Log ("a");
			}
		}

		//walk
		float speedPercent = agent.velocity.magnitude / agent.speed;
		anim.SetFloat("speedPercent", speedPercent, locomotionAnimationSmootTime, Time.deltaTime);

	}

	//--プレイヤー（target）の方向を向く関数
	void FaceTarget()
	{
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
	}


	//選択時lookRadiusをギズモで表示する関数
	void OnDrawGizmosSelected()
	{
		//Gizmos.color = Color.red;
		Gizmos.color = Color.blue;
		//Gizmos.DrawWireSphere(transform.position, lookRadius);
		Vector3 a = transform.position + transform.forward * lookRadius;
		Vector3 b = a + transform.right * lookRadius * Mathf.Tan (_viewAngle / 2);
		Vector3 c = a + (-transform.right) * lookRadius * Mathf.Tan (_viewAngle / 2);
		Vector3 pos = _target.transform.position - transform.position;
		Vector3 localpos = transform.InverseTransformPoint (_target.transform.position);
		/*if (localpos.z > 0 && localpos.z < transform.InverseTransformPoint( a ).z
			&& localpos.x > transform.InverseTransformPoint( c ).x && localpos.x < transform.InverseTransformPoint( b ).x ) {
			Gizmos.color = Color.red;
		}*/
		/*方法１：領域
		if (localpos.z < transform.InverseTransformPoint (a).z && localpos.z >= transform.InverseTransformPoint (c).z / transform.InverseTransformPoint (c).x * localpos.x && localpos.z >= transform.InverseTransformPoint (b).z / transform.InverseTransformPoint (b).x * localpos.x) {
			Gizmos.color = Color.red;
		}*/
		//方法２：外積
		if (Vector3.Cross (localpos, transform.InverseTransformPoint (c)).y < 0 && Vector3.Cross (localpos - transform.InverseTransformPoint(c), transform.InverseTransformPoint(b) - transform.InverseTransformPoint(c)).y < 0 && Vector3.Cross (localpos - transform.InverseTransformPoint(b), Vector3.zero - transform.InverseTransformPoint(b)).y < 0) {
			Gizmos.color = Color.red;
		}
			/*if (pos.magnitude <= lookRadius) {
			Gizmos.color = Color.red;
		}*/
		Gizmos.DrawLine( transform.position, a );
		Gizmos.DrawLine ( b , c );
		Gizmos.DrawLine ( transform.position, b );
		Gizmos.DrawLine ( transform.position, c );
	}
}