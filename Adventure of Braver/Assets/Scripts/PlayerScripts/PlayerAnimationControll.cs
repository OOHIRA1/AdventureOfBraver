using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==プレイヤーのアニメーションを管理するクラス
//
//使用方法：プレイヤーにアタッチ
[RequireComponent(typeof(Animator))]
public class PlayerAnimationControll : MonoBehaviour {
	const float LOCOMOTION_ANIMATION_SMOOTH_TIME = 0.5f;
	const float ATTACK_INTERVAL = 1f;

	Animator _animator;
	float _attackTime;

	// Use this for initialization
	void Start () {
		_animator = GetComponent<Animator> ();
		_attackTime = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.F))
			AttackSingle ();
		if (Input.GetKeyDown (KeyCode.C))
			AttackDouble ();
		if (Input.GetKeyDown (KeyCode.D))
			Death ();
		if (Input.GetKeyDown (KeyCode.J))
			_animator.SetTrigger ("skillJumpAttack");
		if (Input.GetKeyDown (KeyCode.H))
			AttackWindWheel ();
		if (Input.GetKeyDown (KeyCode.Q))
		if ( !CheckAttacking() )
			ChangeAttacking ( true );
		else
			ChangeAttacking ( false );
		if (_attackTime > 0) {
			_attackTime -= Time.deltaTime;
		}
	}

	//==========================================================
	//public関数
	//--移動・待機アニメーションをする関数
	public void MoveAnim( float speedPercent )
	{
		_animator.SetFloat("speedPercent", speedPercent, LOCOMOTION_ANIMATION_SMOOTH_TIME, Time.deltaTime);
	}


	//--攻撃（AttackSingle）のアニメーションをする関数
	public void AttackSingle() {
		if (_attackTime > 0) return;
		_animator.SetTrigger ("Attack");
		_attackTime = ATTACK_INTERVAL;
	}


	//--攻撃（attackDouble）のアニメーションをする関数
	public void AttackDouble() {
		_animator.SetTrigger ("attackDouble");
	}


	//--攻撃（skillWindWheel）のアニメーションをする関数
	public void AttackWindWheel() {
		_animator.SetTrigger ("skillWindWheel");
	}


	//--死亡アニメーションをする関数
	public void Death() {
		_animator.SetTrigger ("death");
	}


	//--攻撃待機状態のオンオフをする関数
	public void ChangeAttacking( bool x )
	{
		_animator.SetBool ("attackingWaiting", x);
	}


	//--攻撃待機状態かどうかを確認する関数
	public bool CheckAttacking( )
	{
		return _animator.GetBool ("attackingWaiting");
	}


	//--nameのアニメーションが再生中かどうかを返す関数
	public bool IsPlaying( string name ) {
		AnimatorClipInfo[] animatorClipInfo = _animator.GetCurrentAnimatorClipInfo (0);	//どうやらanimatorClipInfoの参照を渡すように配列にしているだけで要素数は1のみみたい。
		return animatorClipInfo[0].clip.name == name;
	}
	//==========================================================
	//==========================================================
}
