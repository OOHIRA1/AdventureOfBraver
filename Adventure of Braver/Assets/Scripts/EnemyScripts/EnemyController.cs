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
[RequireComponent(typeof(FieldOfView))]
public class EnemyController : MonoBehaviour {
	const float locomotionAnimationSmootTime = .05f;


    [SerializeField]
	Transform _target;
	NavMeshAgent _agent;
    EnemyNav _enemyNav;
	EnemyHealth _enemyHealth;
	GoblinAnimationController _animController;
	EnemyGenerator _enemyGenerator;
	FieldOfView _fieldOfView;

	[SerializeField] float _assistRadius = 12f;											//ゴブリンの加勢する範囲
	bool _lockOn;																		//プレイヤーをロックオンしているかどうかのフラグ
	[SerializeField] float LockOnRadius;												//ロックオンする範囲
	[SerializeField] List<GameObject> _goblinList = new List<GameObject>( );			//他のゴブリンのリスト


	// Use this for initialization
	void Start()
	{
		_target = PlayerManager.instance.player.transform;
        _agent = GetComponent<NavMeshAgent>();
		_enemyNav = GetComponent<EnemyNav>();
		_enemyHealth = GetComponent<EnemyHealth> ();
		_animController = GetComponent<GoblinAnimationController>();
		_enemyGenerator = GameObject.Find ("EnemyGenerator").GetComponent<EnemyGenerator> ();
		_fieldOfView = GetComponent<FieldOfView> ();
		if (_target) {	//視野表示のターゲットを_targetに変更
			_fieldOfView.SetTarget ( _target );
		}
		_lockOn = false;
		LockOnRadius = _fieldOfView.GetLookRadius () / 2;
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
		//_targetの位置に移動する処理----------------------------------------------------
		if (!_animController.CheckAttacking () ) {
			if (!_lockOn) {		//ロックオン前処理
				//視野内に_targetがいたら行動する処理-----------
				if (_fieldOfView.IsInFieldOfView (_target)) {
					_enemyNav.MoveToTarget (_target.position);
					if (distance <= LockOnRadius) {
						_lockOn = true;
					}
				}
				//---------------------------------------------
			} else { 			//ロックオン時処理			
				if (distance <= LockOnRadius) {
					_enemyNav.MoveToTarget (_target.position);
				} else {
					_lockOn = false;
				}
			}
			//加勢に入る処理------------------------------------------------------------------------------------------------
			for (int i = 0; i < _goblinList.Count; i++) {
				GoblinAnimationController goblinListAnim = _goblinList [i].GetComponent<EnemyController> ()._animController;
				if (goblinListAnim.CheckAttacking ()) {
					if (distance <= _assistRadius) {
						_enemyNav.MoveToTarget (_target.position);
						break;
					}
				}
			}
			//-------------------------------------------------------------------------------------------------------------
		}
		//-------------------------------------------------------------------------------

		//至近距離時の処理---------------------------------------------------------
		if (distance <= _agent.stoppingDistance) {
			if (_fieldOfView.IsInFieldOfView ( _target )) {	//視野内にいる時のみ向きを変える
				FaceTarget ();
			}
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
		if (_enemyHealth.isDead) {
			_animController.DeadAnim ( );
			Debug.Log (gameObject.name + "death");
			enabled = false;	//死亡したら処理をやめる
		} 
		//---------------------------------------

		if (Input.GetKeyDown (KeyCode.Alpha0)) {	//デバッグ用ダメージ処理
			_enemyHealth.TakeDamage (1f);
		}
	}


	//--プレイヤー（target）の方向を向く関数
	void FaceTarget()
	{
		Vector3 direction = (_target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation (new Vector3 (direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * 5f);
	}


	//--_goblinListを更新する関数(なくてもいいかも)
	void UpdateGoblinList() {
		_goblinList = _enemyGenerator.GetGoblinList( );
	}


	//--選択時_lockOn時のギズモ・加勢する範囲のギズモを表示する関数
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		//ギズモの描画---------------------------------------------
		if (_lockOn) {
			_fieldOfView.SetDrawGizmos (false);		//視野ギズモ非表示
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere (transform.position, LockOnRadius);
		} else {
			if (_fieldOfView) {		//プレビュー前は_fieldOfViewの参照が取れていないので処理しない
				_fieldOfView.SetDrawGizmos (true);
			}
		}
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere ( transform.position, _assistRadius );
		//---------------------------------------------------------
	}
}

/*
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
*/
