using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//機能：ゴブリンのアニメーションを管理するスクリプト
//
//アタッチ：ゴブリンにアタッチ
[RequireComponent(typeof(Animator))]
public class GoblinAnimationController : MonoBehaviour
{
	const float locomotionAnimationSmootTime = .05f;

	Animator _animator;
	NavMeshAgent _agent;

	// Use this for initialization
	void Start ()
	{
		_animator = GetComponent<Animator> ();
		_agent = GetComponent<NavMeshAgent>();
	}

	// Update is called once per frame
	void Update()
	{
		//walk
		float speedPercent = _agent.velocity.magnitude / _agent.speed;
		_animator.SetFloat("speedPercent", speedPercent, locomotionAnimationSmootTime, Time.deltaTime);
	}


	//---------------------
	//--public関数
	//---------------------
	//--攻撃待機状態のオンオフをする関数
	public void ChangeAttacking( bool x )
	{
		_animator.SetBool ("attacking", x);
	}


	//--攻撃アニメーションをする関数
	public void AttackAnim( )
	{
		if (!_animator.GetBool ("attacking")) return;

		int n = Random.Range (1, 4);
		switch (n) 
		{
		case 1:
			_animator.SetTrigger ("attack1");
			break;
		case 2:
			_animator.SetTrigger ("attack2");
			break;
		case 3:
			_animator.SetTrigger ("attack3");
			break;
		default :
			Debug.LogError ("想定外の処理がされました。");
			break;
		}
	}


	//--防御状態をオンオフする関数
	public void ChangeBlock( bool x )
	{
		_animator.SetBool ("block", x);
	}


	//--ノックバックアニメーションをする関数
	public void BlockHitAnim()
	{
		if (!_animator.GetBool ("block")) return;

		_animator.SetTrigger ("block_hit");
	}


	//--死亡アニメーションをする関数
	public void DeadAnim()
	{
		_animator.SetBool ("death", true);
	}
	//-----------------------------------
	//-----------------------------------
}
