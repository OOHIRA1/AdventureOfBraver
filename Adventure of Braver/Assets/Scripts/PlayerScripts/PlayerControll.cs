using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==プレイヤーの動きを管理するクラス
//
//使用方法：プレイヤーにアタッチ
[RequireComponent(typeof(PlayerAnimationControll))]
public class PlayerControll : MonoBehaviour {

	public enum State {
		LOCOMOTION,
		ATTACK_WAITING,
		ATTACK_SINGLE,
		ATTACK_DOUBLE,
		ATTACK_WIND_WHEEL,
		ATTACK_JUMP,
		DEATH
	}


	CharacterController _charactorController;
	PlayerAnimationControll _animController;
	[SerializeField] float _maxWalkSpeed = 0;		//歩く速さ
	[SerializeField] float _maxRunSpeed  = 0;		//走る速さ
	[SerializeField] bool _isRuning = false;	//走るかどうかのフラグ
	[SerializeField] State _state;					//プレイヤーのState
	[SerializeField] float _velocityY = 0;



	//------------------------------------
	//ゲッター
	public State GetState() {
		return _state;
	}
	//------------------------------------
	//------------------------------------


	//------------------------------------
	//セッター
	//------------------------------------
	public void SetState ( State state ) {
		_state = state;
	}
	//------------------------------------
	//------------------------------------


	// Use this for initialization
	void Start () {
		_charactorController = GetComponent<CharacterController> ();
		_animController = GetComponent<PlayerAnimationControll> ();
		_state = State.LOCOMOTION;
	}
	
	// Update is called once per frame
	void Update () {
		bool attackButtonClicked = Input.GetKeyDown (KeyCode.JoystickButton1) || Input.GetMouseButton (0) || Input.GetKeyDown (KeyCode.Return);	//攻撃をするボタン

		switch (_state) {
		case State.LOCOMOTION:
			LocomotionAction ();
			if (attackButtonClicked) {
				_animController.AttackSingle ();
			}
			break;
		case State.ATTACK_WAITING:
			AttackWaitingAction ();
			if (attackButtonClicked) {
				_animController.AttackSingle ();
			}
			break;
		case State.ATTACK_SINGLE:
			if (attackButtonClicked) {
				_animController.AttackDouble ();
			}
			break;
		case State.ATTACK_DOUBLE:
			if (attackButtonClicked) {
				_animController.AttackSingle ();
			}
			break;
		case State.DEATH:
			DeathAction ();
			break;
		default:
			break;
		}
		if (Input.GetKeyDown (KeyCode.P))
			SetState ( State.DEATH );
		UpdateState ();
		Debug.Log (gameObject.name + " State : " + _state);

	}





	//--LOCOMOTION中のアクションをする関数
	void LocomotionAction() {
		Move ();
	}


	//--ATTACK_WAITING中のアクションをする関数
	void AttackWaitingAction() {
	}


	//--DEATH中のアクションをする関数
	void DeathAction() {
		//死亡-----------------------------------
		_animController.Death ( );
		Debug.Log (gameObject.name + " death");
		enabled = false;	//死亡したら処理をやめる
		//---------------------------------------
	}


	//--移動をする関数
	void Move () {
		Vector3 inputVector = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));	//入力ベクトル
		//inputVector = Camera.main.transform.right * Input.GetAxis ("Horizontal") + Camera.main.transform.forward * Input.GetAxis ("Horizontal");
		Vector3 moveDirection = inputVector.normalized;	//移動方向ベクトル
		//Debug.Log(moveDirection);

		//向きを変える処理-----------------------------------
		transform.LookAt( transform.position + moveDirection );
		//--------------------------------------------------



		//走るかどうかの処理--------------------------------
		if (Input.GetAxis ("Dash") > 0) {
			_isRuning = true;
		}
		if (moveDirection == Vector3.zero) {
			_isRuning = false;
		}
		//--------------------------------------------------

		//速度測定---------------------------------------------------------------------
		Vector3 velocity = Vector3.zero;
		float inputPower = Mathf.InverseLerp (0, 1, inputVector.magnitude);	//入力の強さ
		if (_isRuning) {
			velocity = (inputPower * _maxRunSpeed) * moveDirection;
		} else {
			velocity = (inputPower * _maxWalkSpeed) * moveDirection;
		}

		//------------------------------------------------------------------------------


		//walk・idle・runアニメーション処理--------------------------(重力処理の前に行う)
		float speedPercent = velocity.magnitude / _maxRunSpeed;
		_animController.MoveAnim ( speedPercent );
		//----------------------------------------------------------

		//重力処理---------------------------------------------------
		const float gravity = -98f;
		if (!_charactorController.isGrounded) {
			_velocityY += gravity * Time.deltaTime;
		} else {
			velocity.y = 0;
			_velocityY = 0;
		}
		velocity.y += _velocityY;
		//-----------------------------------------------------------

		_charactorController.Move (velocity * Time.deltaTime);
	}


	//--攻撃をする関数
	void Attack() {
		_animController.AttackSingle ();
		if (_animController.IsPlaying ("AttackSingle")) {
			_animController.AttackDouble ();
		}
		if (_animController.IsPlaying ("AttackDouble")) {
			_animController.AttackSingle ();
		}
	}


	//--Stateを更新する関数
	void UpdateState() {
		if (!_animController.IsPlaying ("AttackWaiting")) {
			SetState (State.LOCOMOTION);
		} else {
			SetState (State.ATTACK_WAITING);
		}
		if (_animController.IsPlaying ("AttackSingle")) {
			SetState (State.ATTACK_SINGLE);
		}
		if (_animController.IsPlaying ("AttackDouble")) {
			SetState (State.ATTACK_DOUBLE);
		}
		if (_animController.IsPlaying ("SkillWindWheel")) {
			SetState (State.ATTACK_WIND_WHEEL);
		}
		if (_animController.IsPlaying ("SkillJumpAttack")) {
			SetState (State.ATTACK_JUMP);
		}
		if (_animController.IsPlaying ("Death")) {
			SetState (State.DEATH);
		}
	}

}
