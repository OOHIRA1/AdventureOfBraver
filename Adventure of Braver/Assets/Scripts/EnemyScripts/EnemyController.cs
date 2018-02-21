using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//機能：ゴブリンの動きを管理するスクリプト
//
//使用方法：ゴブリンにアタッチ
[RequireComponent(typeof(EnemyNav))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(GoblinAnimationController))]
public class EnemyController : MonoBehaviour {
	const float locomotionAnimationSmootTime = .05f;


    [SerializeField]
	Transform _target;
    EnemyNav _enemyNav;
    NavMeshAgent _agent;
	GoblinAnimationController _animController;
	EnemyHealth _enemyHealth;
	EnemyGenerator _enemyGenerator;

	[SerializeField] float _lookRadius = 10f;		//ゴブリンの見える範囲
	[SerializeField] float _assistRadius = 12f;		//ゴブリンの加勢する範囲
	[SerializeField] float _viewAngle = ( float )3.14 / 2;	//視野角
	public GameObject _targetForGizmos;					//ターゲット（ギズモ用）
	bool _lockOn;										//プレイヤーをロックオンしているかどうかのフラグ
	bool _death;										//死亡しているかどうか
	[SerializeField] List<GameObject> _goblinList = new List<GameObject>( );			//他のゴブリン


	// Use this for initialization
	void Start()
	{
		_target = PlayerManager.instance.player.transform;
        _enemyNav = GetComponent<EnemyNav>();
        _agent = GetComponent<NavMeshAgent>();
		_animController = GetComponent<GoblinAnimationController>();
		_enemyHealth = GetComponent<EnemyHealth> ();
		_enemyGenerator = GameObject.Find ("EnemyGenerator").GetComponent<EnemyGenerator> ();
		_lockOn = false;
		_death = false;
		_goblinList = _enemyGenerator.GetGoblinList( );
	}

	// Update is called once per frame
	void Update()
	{
		Act ();
		//UpdateGoblinList ();
	}


	//--エネミーの行動を表す関数
	void Act()
	{
		float distance = Vector3.Distance(_target.position, transform.position);
		//_targetの位置に移動する処理-------------------------------------
		if (!_animController.CheckAttacking () ) {
			if (!_lockOn) {		//ロックオン前処理
				//視野に使用するベクトル------------------
				Vector3 forwardPos = transform.position + transform.forward * _lookRadius;
				Vector3 rightForwardPos = forwardPos + transform.right * _lookRadius * Mathf.Tan (_viewAngle / 2);
				Vector3 leftForwardPos = forwardPos + (-transform.right) * _lookRadius * Mathf.Tan (_viewAngle / 2);
				Vector3 localTargetPos = transform.InverseTransformPoint (_target.transform.position);					//ローカル座標から見たtargetの座標
				//----------------------------------------------

				//視野内にtargetがいたら行動する処理---------------------------------
				if (Vector3.Cross (localTargetPos, transform.InverseTransformPoint (leftForwardPos)).y < 0
					&& Vector3.Cross (localTargetPos - transform.InverseTransformPoint (leftForwardPos), transform.InverseTransformPoint (rightForwardPos) - transform.InverseTransformPoint (leftForwardPos)).y < 0
					&& Vector3.Cross (localTargetPos - transform.InverseTransformPoint (rightForwardPos), Vector3.zero - transform.InverseTransformPoint (rightForwardPos)).y < 0)
				{
					_enemyNav.MoveToTarget (_target.position);
					if (distance <= _lookRadius / 2) {
						_lockOn = true;
					}
				}
				//---------------------------------------------------------------------------------
			} else { 			//ロックオン時処理			
				if (distance <= _lookRadius / 2) {
					_enemyNav.MoveToTarget (_target.position);
				} else {
					_lockOn = false;
				}
			}
			for (int i = 0; i < _goblinList.Count; i++) {
				if (_goblinList[i].GetComponent<EnemyController> ()._animController.CheckAttacking ()) {
					if (distance <= _assistRadius) {
						_enemyNav.MoveToTarget (_target.position);
						break;
					}
				}
			}
		}
		//---------------------------------------------------------------

		//至近距離時の処理---------------------------------------------------------
		if (distance <= _agent.stoppingDistance) {
			//FaceTarget ();
			ActInField( "FaceTarget", null );	//視野内にいる時のみ向きを変える
			_animController.ChangeAttacking (true);
		} else {
			_animController.ChangeAttacking (false);
		}
		//------------------------------------------------------------------------

		//walk・idle・runアニメーション処理---------------------------------
		float speedPercent = _agent.velocity.magnitude / _agent.speed;
		_animController.MoveAnim ( speedPercent );
		//---------------------------------------------------

		//攻撃-------------------------------------
		if (_animController.CheckAttacking ()) {
			_animController.AttackAnim ();
		}
		//------------------------------------------

		//防御------------------------------------
		if (Input.GetKey (KeyCode.G) && _animController.CheckAttacking ()) {	//キーコードの所をプレイヤーの攻撃しているかどうかの判定に変更
			_animController.ChangeBlock (true);
		} else {
			_animController.ChangeBlock (false);
		}
		//-----------------------------------------

		//ノックバック---------------------------
		if (Input.GetKeyDown (KeyCode.N) && _animController.CheckBlock ()) {	//のちのちOnCollisionEnterでやると思われる
			_animController.BlockHitAnim ();
		}
		//---------------------------------------

		//死亡-----------------------------------
		if (_enemyHealth.isDead /*&& !_death*/) {
			_animController.DeadAnim ( );
			_death = true;
			Debug.Log (gameObject.name + "death");
		} 
		//---------------------------------------

		if (Input.GetKeyDown (KeyCode.Alpha0)) {	//デバッグ用ダメージ処理
			_enemyHealth.TakeDamage (1f);
		}

		if (_death) {	//死亡したら処理をやめる
			enabled = false;
		}
	}

	//--プレイヤー（target）の方向を向く関数
	void FaceTarget()
	{
		Vector3 direction = (_target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation (new Vector3 (direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * 5f);
	}


	//--視野内に_targetがいたらfunctionName関数を呼ぶ関数(現在未使用)
	void ActInField( string functionName, object value )
	{
		//視野に使用するベクトル------------------
		Vector3 forwardPos = transform.position + transform.forward * _lookRadius;
		Vector3 rightForwardPos = forwardPos + transform.right * _lookRadius * Mathf.Tan (_viewAngle / 2);
		Vector3 leftForwardPos = forwardPos + (-transform.right) * _lookRadius * Mathf.Tan (_viewAngle / 2);
		Vector3 localTargetPos = transform.InverseTransformPoint (_target.transform.position);					//ローカル座標から見たtargetの座標
		//----------------------------------------------

		//視野内にtargetがいたら行動する処理---------------------------------
		if (Vector3.Cross (localTargetPos, transform.InverseTransformPoint (leftForwardPos)).y < 0
			&& Vector3.Cross (localTargetPos - transform.InverseTransformPoint (leftForwardPos), transform.InverseTransformPoint (rightForwardPos) - transform.InverseTransformPoint (leftForwardPos)).y < 0
			&& Vector3.Cross (localTargetPos - transform.InverseTransformPoint (rightForwardPos), Vector3.zero - transform.InverseTransformPoint (rightForwardPos)).y < 0)
		{
			SendMessage ( functionName, value );
			//_enemyNav.MoveToTarget (_target.position);
		}
		//---------------------------------------------------------------------------------
	}


	//--_goblinListを更新する関数(なくてもいいかも)
	void UpdateGoblinList() {
		_goblinList = _enemyGenerator.GetGoblinList( );
	}


	//選択時lookRadiusをギズモで表示する関数
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		//視野ギズモに使用するベクトル------------------
		Vector3 forwardPos = transform.position + transform.forward * _lookRadius;
		Vector3 rightForwardPos = forwardPos + transform.right * _lookRadius * Mathf.Tan (_viewAngle / 2);	
		Vector3 leftForwardPos = forwardPos + (-transform.right) * _lookRadius * Mathf.Tan (_viewAngle / 2);
		Vector3 localTargetPos = transform.InverseTransformPoint (_targetForGizmos.transform.position);					//ローカル座標から見た_targetの座標
		//---------------------------------------------
		if (!_lockOn) {
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
			    && Vector3.Cross (localTargetPos - transform.InverseTransformPoint (rightForwardPos), Vector3.zero - transform.InverseTransformPoint (rightForwardPos)).y < 0) {
				Gizmos.color = Color.red;
			}
			//----------------------------------------------------------------
		} else {
			float distance = Vector3.Distance (_targetForGizmos.transform.position, transform.position);
			if (distance <= _lookRadius / 2) {
				Gizmos.color = Color.red;
			}
		}

		//ギズモの描画---------------------------------------------
		if (!_lockOn) {
			Gizmos.DrawLine (transform.position, forwardPos);
			Gizmos.DrawLine (rightForwardPos, leftForwardPos);
			Gizmos.DrawLine (transform.position, rightForwardPos);
			Gizmos.DrawLine (transform.position, leftForwardPos);
		} else {
			Gizmos.DrawWireSphere (transform.position, _lookRadius / 2);
		}
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere ( transform.position, _assistRadius );
		//---------------------------------------------------------
	}
}