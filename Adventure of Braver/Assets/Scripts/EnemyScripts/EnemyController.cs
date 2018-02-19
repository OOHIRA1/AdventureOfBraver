using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//機能：エネミーの動きを管理するスクリプト
//
//使用方法：ゴブリンにアタッチ
public class EnemyController : MonoBehaviour
{
	public float lookRadius = 10f;	//ゴブリンの見える範囲
	const float locomotionAnimationSmootTime = .05f;

    [SerializeField]
	Transform target;
    EnemyNav enemyNav;
    NavMeshAgent agent;

	[SerializeField] float _viewAngle = ( float )3.14 / 2;	//視野角
	public GameObject _targetForGizmos;					//ターゲット（ギズモ用）
	bool _lockOn;										//プレイヤーをロックオンしているかどうかのフラグ

	// Use this for initialization
	void Start()
	{
		target = PlayerManager.instance.player.transform;
        enemyNav = GetComponent<EnemyNav>();
        agent = GetComponent<NavMeshAgent>();
		//anim = GetComponent<Animator>();
		_lockOn = false;
	}

	// Update is called once per frame
	void Update()
	{
		Act ();
	}


	//--エネミーの行動を表す関数
	void Act()
	{
		if (!_lockOn)
		{
			//視野に使用するベクトル------------------
			Vector3 forwardPos = transform.position + transform.forward * lookRadius;
			Vector3 rightForwardPos = forwardPos + transform.right * lookRadius * Mathf.Tan (_viewAngle / 2);
			Vector3 leftForwardPos = forwardPos + (-transform.right) * lookRadius * Mathf.Tan (_viewAngle / 2);
			Vector3 localTargetPos = transform.InverseTransformPoint (target.transform.position);					//ローカル座標から見たtargetの座標
			//----------------------------------------------
			//視野内にtargetがいたら目的地をtargetに設定する処理---------------------------------
			if (Vector3.Cross (localTargetPos, transform.InverseTransformPoint (leftForwardPos)).y < 0
				&& Vector3.Cross (localTargetPos - transform.InverseTransformPoint (leftForwardPos), transform.InverseTransformPoint (rightForwardPos) - transform.InverseTransformPoint (leftForwardPos)).y < 0
				&& Vector3.Cross (localTargetPos - transform.InverseTransformPoint (rightForwardPos), Vector3.zero - transform.InverseTransformPoint (rightForwardPos)).y < 0)
			{
				enemyNav.MoveToTarget (target.position);

				float distance = Vector3.Distance(target.position, transform.position);
				if (distance <= agent.stoppingDistance)
				{
					FaceTarget ();
				}
				if (distance <= lookRadius / 2)
				{
					_lockOn = true;
				}
			}
			//---------------------------------------------------------------------------------
		} else
		{
			float distance = Vector3.Distance(target.position, transform.position);
			if (distance <= lookRadius / 2) 
			{
				enemyNav.MoveToTarget (target.position);

				if (distance <= agent.stoppingDistance) {
					FaceTarget ();
				}
			} else
			{
				_lockOn = false;
			}
		}
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
		Gizmos.color = Color.blue;
		//視野ギズモに使用するベクトル------------------
		Vector3 forwardPos = transform.position + transform.forward * lookRadius;
		Vector3 rightForwardPos = forwardPos + transform.right * lookRadius * Mathf.Tan (_viewAngle / 2);	
		Vector3 leftForwardPos = forwardPos + (-transform.right) * lookRadius * Mathf.Tan (_viewAngle / 2);
		Vector3 localTargetPos = transform.InverseTransformPoint (_targetForGizmos.transform.position);					//ローカル座標から見た_targetの座標
		//---------------------------------------------
		if (!_lockOn)
		{
			//視野内に_targetが入っているかの処理---------------------------
			//方法１：領域
			/*
			if (   localTargetPos.z < transform.InverseTransformPoint (forwardPos).z
				&& localTargetPos.z >= transform.InverseTransformPoint (leftForwardPos).z / transform.InverseTransformPoint (leftForwardPos).x * localTargetPos.x
				&& localTargetPos.z >= transform.InverseTransformPoint (rightForwardPos).z / transform.InverseTransformPoint (rightForwardPos).x * localTargetPos.x )
			{
				Gizmos.color = Color.red;
			}
			*/
			//方法２：外積
			if (Vector3.Cross (localTargetPos, transform.InverseTransformPoint (leftForwardPos)).y < 0
			   && Vector3.Cross (localTargetPos - transform.InverseTransformPoint (leftForwardPos), transform.InverseTransformPoint (rightForwardPos) - transform.InverseTransformPoint (leftForwardPos)).y < 0
			   && Vector3.Cross (localTargetPos - transform.InverseTransformPoint (rightForwardPos), Vector3.zero - transform.InverseTransformPoint (rightForwardPos)).y < 0) 
			{
				Gizmos.color = Color.red;
			}
			//----------------------------------------------------------------
		} else 
		{
			float distance = Vector3.Distance (_targetForGizmos.transform.position, transform.position);
			if (distance <= lookRadius / 2)
			{
				Gizmos.color = Color.red;
			}
		}

		//ギズモの描画---------------------------------------------
		if (!_lockOn)
		{
			Gizmos.DrawLine (transform.position, forwardPos);
			Gizmos.DrawLine (rightForwardPos, leftForwardPos);
			Gizmos.DrawLine (transform.position, rightForwardPos);
			Gizmos.DrawLine (transform.position, leftForwardPos);
		} else 
		{
			Gizmos.DrawWireSphere (transform.position, lookRadius / 2);
		}
		//---------------------------------------------------------
	}
}