using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//機能：ゴブリンのアニメーションを管理するスクリプト
//
//アタッチ：ゴブリンにアタッチ(EnemyController.csをアタッチすることで自動でアタッチされる)
[RequireComponent(typeof(Animator))]
public class GoblinAnimationController : MonoBehaviour
{
	const float LOCOMOTION_ANIMATION_SMOOTH_TIME = .05f;
	const float ATTACK_INTERVAL = 5f;					//攻撃インターバル
	const int ATTACK_TYPE = 3;							//攻撃のバリエーション数

	Animator _animator;
	//NavMeshAgent _agent;
	float _attackTime;		//攻撃インターバル

	// Use this for initialization
	void Start ()
	{
		_animator = GetComponent<Animator> ();
		//_agent = GetComponent<NavMeshAgent>();
		_attackTime = 0;
	}

	// Update is called once per frame
	void Update()
	{
		if (_attackTime > 0) {
			_attackTime -= Time.deltaTime;
		}
	}


	//====================================================================================================
	//public関数
	//=====================================================================================================
	//--移動・待機アニメーションをする関数
	public void MoveAnim( float speedPercent )
	{
		_animator.SetFloat("speedPercent", speedPercent, LOCOMOTION_ANIMATION_SMOOTH_TIME, Time.deltaTime);
	}



	//--攻撃待機状態のオンオフをする関数
	public void ChangeAttacking( bool x )
	{
		_animator.SetBool ("attacking", x);
	}


	//--攻撃待機状態かどうかを確認する関数
	public bool CheckAttacking( )
	{
		return _animator.GetBool ("attacking");
	}

	//--攻撃アニメーションをする関数
	public void AttackAnim( )
	{
		if (!CheckAttacking()) return;
		if (_attackTime > 0) return;
		//if (_animator.GetBool ("block")) return; block値がtrueでもattackのトリガーがtrueになっていたらparametersの順番のせいで攻撃をしてしまうのでtransitionのconditionsで操作

		int n = Random.Range (1, (ATTACK_TYPE + 1) + 3);
		string[] attack = { "attack1", "attack2", "attack3" };
		for (int i = 0; i < attack.Length; i++) {
			if (n > attack.Length) {	//attack1の確立を上げるための処理
				_animator.SetTrigger ("attack1");
				_attackTime = ATTACK_INTERVAL;
				break;
			}
			if (i + 1 == n) {
				_animator.SetTrigger (attack [i]);
				_attackTime = ATTACK_INTERVAL;
				break;
			}
		}
	}


	//--防御状態をオンオフする関数
	public void ChangeBlock( bool x )
	{
		_animator.SetBool ("block", x);
	}


	//--防御状態かどうかを確認する関数
	public bool CheckBlock( ) 
	{
		return _animator.GetBool ("block");
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
		_animator.SetTrigger("death");
	}

	//--nameのアニメーションが再生中かどうかを返す関数
	public bool IsPlaying( string name ) {
		AnimatorClipInfo[] animatorClipInfo = _animator.GetCurrentAnimatorClipInfo (0);	//どうやらanimatorClipInfoの参照を渡すように配列にしているだけで要素数は1のみみたい。
		return animatorClipInfo[0].clip.name == name;
	}
	//==============================================================================================================================================
	//==============================================================================================================================================
}
