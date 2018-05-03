using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==プレイヤーのアニメーションを管理するクラス
//
//使用方法：プレイヤーにアタッチ
[RequireComponent(typeof(Animator))]
public class PlayerAnimationControll : MonoBehaviour {
	const float LOCOMOTION_ANIMATION_SMOOTH_TIME = 0.5f;
	const float ATTAACK_INTERVAL = 1f;

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
			_animator.SetTrigger ("Attack");
		if (Input.GetKeyDown (KeyCode.C))
			_animator.SetTrigger ("attackDouble");
		if (Input.GetKeyDown (KeyCode.D))
			_animator.SetTrigger ("death");
		if (Input.GetKeyDown (KeyCode.J))
			_animator.SetTrigger ("skillJumpAttack");
		if (Input.GetKeyDown (KeyCode.H))
			_animator.SetTrigger ("skillWindWheel");
		
	}

	//==========================================================
	//public関数
	//--移動・待機アニメーションをする関数
	public void MoveAnim( float speedPercent )
	{
		_animator.SetFloat("speedPercent", speedPercent, LOCOMOTION_ANIMATION_SMOOTH_TIME, Time.deltaTime);
	}



	//==========================================================
	//==========================================================
}
