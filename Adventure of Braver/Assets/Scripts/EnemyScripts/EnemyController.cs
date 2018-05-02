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
[RequireComponent(typeof(EnemySoundReaction))]
public class EnemyController : MonoBehaviour {
	const float locomotionAnimationSmootTime = .05f;

	public enum State {
		LOCOMOTION,
		COMBIN_IDLE,
		ATTACK1,
		ATTACK2,
		ATTACK3,
		BLOCK,
		ESCAPE,
		DEATH
	}


    [SerializeField] Transform _target;
	NavMeshAgent _agent;
    EnemyNav _enemyNav;
	EnemyHealth _enemyHealth;
	GoblinAnimationController _animController;
	EnemyGenerator _enemyGenerator;
	FieldOfView _fieldOfView;
	EnemySoundReaction _soundReaction;

	[SerializeField] State _state;														//ゴブリンのステート
	[SerializeField] float _assistRadius = 12f;											//ゴブリンの加勢する範囲
	bool _lockOn;																		//プレイヤーをロックオンしているかどうかのフラグ
	[SerializeField] float _lockOnRadius;												//ロックオンする範囲
	[SerializeField] List<GameObject> _goblinList = new List<GameObject>( );			//他のゴブリンのリスト
	[SerializeField] Transform _findedBomb;												//ゴブリンが見つけた爆弾(フィールド上にある爆弾を入れる変数)


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

	public void SetFindedBomb( Transform x ) {
		_findedBomb = x;
	}
	//------------------------------------
	//------------------------------------


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
		_state = State.LOCOMOTION;
		_soundReaction = GetComponent<EnemySoundReaction> ();
		_lockOn = false;
		_lockOnRadius = _fieldOfView.GetLookRadius () / 2;
		_goblinList = _enemyGenerator.GetGoblinList( );
		_findedBomb = null;
	}

	// Update is called once per frame
	void Update()
	{
		switch (_state) {
		case State.LOCOMOTION:
			LocomotionAction ();
			break;
		case State.COMBIN_IDLE:
			CombatIdleAction ();
			break;
		case State.BLOCK:
			BlockAction ();
			break;
		case State.ESCAPE:
			EscapeAction ();
			break;
		case State.DEATH:
			DeathAction ();
			break;
		default :
			break;
		}
		if (Input.GetKeyDown (KeyCode.B) && _fieldOfView.IsInFieldOfView (_target)) {
			SetFindedBomb (_target);	//デバッグ用として_targetを爆弾とみなす（のちに外部のスクリプトから爆弾をセットする処理を記述）
		}
		if (Input.GetKeyDown (KeyCode.Alpha0)) {	//デバッグ用ダメージ処理
			_enemyHealth.TakeDamage (1f);
		}
		UpdateState ();
		Debug.Log (_state);
		//Act ();
		//UpdateGoblinList ();
	}




	//--targetの方向を向く関数
	void FaceTarget( Transform target )
	{
		if (!target) return;	//ターゲットを見失ったら戻る
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation (new Vector3 (direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * 5f);
	}


	//--_goblinListを更新する関数(なくてもいいかも)
	void UpdateGoblinList() {
		_goblinList = _enemyGenerator.GetGoblinList( );
	}


	//--LOCOMOTION中のアクションをする関数
	void LocomotionAction( ) {
		float distance = Vector3.Distance(_target.position, transform.position);
		//_targetの位置に移動する処理-----------------------------------------------------------------------------------------------------
		if (!_lockOn) {		//ロックオン前処理
			//視野内に_targetがいたら行動する処理-----------
			if (_fieldOfView.IsInFieldOfView (_target)) {
				_enemyNav.MoveToTarget (_target.position);
				if (distance <= _lockOnRadius) {
					_lockOn = true;
				}
			}
			//---------------------------------------------
		} else { 			//ロックオン時処理			
			if (distance <= _lockOnRadius) {
				FaceTarget (_target);	//向きを変えないと移動しないことがある(爆弾使用時)ので向きを変える処理
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
		//--------------------------------------------------------------------------------------------------------------------------------

		//至近距離時の処理-------------------------------------------------------------------------------
		if (distance <= _agent.stoppingDistance) {
			if (_fieldOfView.IsInFieldOfView ( _target )) {	//視野内にいる時のみ向きを変え、攻撃待機状態に
				FaceTarget (_target);
				_animController.ChangeAttacking (true);
			}
		}
		//-----------------------------------------------------------------------------------------------


		//walk・idle・runアニメーション処理---------------------------------
		float speedPercent = _agent.velocity.magnitude * 5 / _agent.speed;
		_animController.MoveAnim ( speedPercent );
		//-----------------------------------------------------------------

		//音に反応する処理----------------------------------------------------------------------------------------------------------------
		if (_agent.velocity.magnitude <= 0) {	//移動してない時
			int count = _soundReaction.GetAudioSourceList().Count;	//_audioSountListの数
			Transform soundTrans = transform;						//聞こえた音のtransformを格納する変数
			bool setFlag = false;									//soundTransformに値を格納したか確認するフラグ
			//音が聞こえているか確認---------------------------------------------------------
			for (int i = 0; i < count; i++) {
				if (_soundReaction.Hear (i)) {
					Transform trans = _soundReaction.GetAudioSourceList () [i].transform;
					if (!setFlag
						|| Vector3.Distance (transform.position, trans.position) < Vector3.Distance (transform.position, soundTrans.position)) {	//より近い方をsoundTransformに格納
						soundTrans = trans;
						setFlag = true;
					}
				}
			}
			//-------------------------------------------------------------------------------
			//音の聞こえた方向を向く処理-----------------------------------------
			if (setFlag) {
				FaceTarget (soundTrans);
				Debug.Log (gameObject.name + " here " + soundTrans.gameObject);
			}
			//------------------------------------------------------------------
		}
		//-------------------------------------------------------------------------------------------------------------------------------	
	}


	//--攻撃待機状態時のアクションをする関数
	void CombatIdleAction() {
		//攻撃-------------------------------------
		_animController.AttackAnim ();
		//------------------------------------------

		//防御------------------------------------
		if (Input.GetKey (KeyCode.G)) {	//キーコードの所をプレイヤーの攻撃しているかどうかの判定に変更
			_animController.ChangeBlock (true);
		}
		//-----------------------------------------

		//至近距離時の処理---------------------------------------------------------
		float distance = Vector3.Distance(_target.position, transform.position);
		if (distance <= _agent.stoppingDistance) {
			FaceTarget (_target);
		} else {
			_animController.ChangeAttacking (false);
		}
		//------------------------------------------------------------------------
	}


	//--防御時のアクションをする関数
	void BlockAction() {
		//ノックバック---------------------------
		if (Input.GetKeyDown (KeyCode.N)) {	//のちのちOnCollisionEnterでやると思われる
			_animController.BlockHitAnim ();
		}
		//---------------------------------------
		_animController.ChangeBlock (false);
	}


	//--逃げるアクションをする関数
	void EscapeAction() {
		//爆弾を見たら逃げる処理---------------------------------------------------------------------------------------------------------
		//※逃げてる途中で爆弾がなくなったらどうなるか要検証
		if (_fieldOfView.IsInFieldOfView (_findedBomb)) {	//爆弾を見つけたら逃げる
			float escapeDistance = 20;
			Vector3 dir = transform.position - _findedBomb.position;
			if (dir == Vector3.zero) {	//dirが零ベクトルだった時の処理
				dir = Vector3.forward;
			}
			_enemyNav.MoveToTarget (_findedBomb.position + dir.normalized * escapeDistance);
		}
		if (_agent.velocity.magnitude <= 0) {	//逃げた後爆弾のあった方向を向く処理
			FaceTarget (_findedBomb);
			Vector3 vecA = transform.InverseTransformDirection ((_findedBomb.position - transform.position).normalized);
			Vector3 vecB = transform.InverseTransformDirection (transform.forward);
			if (Vector3.Angle (vecA, vecB) <= 1.0f) {
				SetFindedBomb (null);
			}
		}
		//-------------------------------------------------------------------------------------------------------------------------------
	}

	//--死亡時のアクションをする関数
	void DeathAction() {
		//死亡-----------------------------------
			_animController.DeadAnim ( );
			Debug.Log (gameObject.name + "death");
			enabled = false;	//死亡したら処理をやめる
		//---------------------------------------

	}


	//--Stateを更新する関数
	void UpdateState() {
		if (_animController.CheckAttacking ()) {
			SetState (State.COMBIN_IDLE);
			if (_animController.IsPlaying ("attack1")) {
				SetState (State.ATTACK1);
			}
			if (_animController.IsPlaying ("attack2")) {
				SetState (State.ATTACK2);
			}
			if (_animController.IsPlaying ("attack3")) {
				SetState (State.ATTACK3);
			}
		} else {
			SetState (State.LOCOMOTION);
			if (_findedBomb) {
				SetState (State.ESCAPE);
			}
		}
		if (_animController.CheckBlock ()) {
			SetState (State.BLOCK);
		}
		if (_enemyHealth.isDead) {
			SetState (State.DEATH);
		}
	}

	//--選択時_lockOn時のギズモ・加勢する範囲のギズモを表示する関数
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		//ギズモの描画---------------------------------------------
		if (_lockOn) {
			_fieldOfView.SetDrawGizmos (false);		//視野ギズモ非表示
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere (transform.position, _lockOnRadius);
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



//  //--エネミーの行動を表す関数
//	void Act()
//	{
//		float distance = Vector3.Distance(_target.position, transform.position);
//		//_targetの位置に移動する処理----------------------------------------------------
//		if (!_animController.CheckAttacking () && !_findedBomb) {	//戦闘待機状態では移動しない＆爆弾があるときは爆弾退避を優先
//			if (!_lockOn) {		//ロックオン前処理
//				//視野内に_targetがいたら行動する処理-----------
//				if (_fieldOfView.IsInFieldOfView (_target)) {
//					_enemyNav.MoveToTarget (_target.position);
//					if (distance <= _lockOnRadius) {
//						_lockOn = true;
//					}
//				}
//				//---------------------------------------------
//			} else { 			//ロックオン時処理			
//				if (distance <= _lockOnRadius) {
//					FaceTarget (_target);	//向きを変えないと移動しないことがある(爆弾使用時)ので向きを変える処理
//					_enemyNav.MoveToTarget (_target.position);
//				} else {
//					_lockOn = false;
//				}
//			}
//			//加勢に入る処理------------------------------------------------------------------------------------------------
//			for (int i = 0; i < _goblinList.Count; i++) {
//				GoblinAnimationController goblinListAnim = _goblinList [i].GetComponent<EnemyController> ()._animController;
//				if (goblinListAnim.CheckAttacking ()) {
//					if (distance <= _assistRadius) {
//						_enemyNav.MoveToTarget (_target.position);
//						break;
//					}
//				}
//			}
//			//-------------------------------------------------------------------------------------------------------------
//		}
//		//-------------------------------------------------------------------------------
//		
//		//至近距離時の処理---------------------------------------------------------
//		if (distance <= _agent.stoppingDistance) {
//			if (_fieldOfView.IsInFieldOfView ( _target )) {	//視野内にいる時のみ向きを変え、攻撃待機状態に
//				FaceTarget (_target);
//				_animController.ChangeAttacking (true);
//			}
//		} else {
//			_animController.ChangeAttacking (false);
//		}
//		//------------------------------------------------------------------------
//		
//		//walk・idle・runアニメーション処理---------------------------------
//		float speedPercent = _agent.velocity.magnitude * 5 / _agent.speed;
//		_animController.MoveAnim ( speedPercent );
//		//---------------------------------------------------
//		
//		//攻撃-------------------------------------
//		if (_animController.CheckAttacking ()) {
//			_animController.AttackAnim ();
//		}
//		//------------------------------------------
//		
//		//防御------------------------------------
//		if (Input.GetKey (KeyCode.G) && _animController.CheckAttacking ()) {	//キーコードの所をプレイヤーの攻撃しているかどうかの判定に変更
//			_animController.ChangeBlock (true);
//		} else {
//			_animController.ChangeBlock (false);
//		}
//		//-----------------------------------------
//		
//		//ノックバック---------------------------
//		if (Input.GetKeyDown (KeyCode.N) && _animController.CheckBlock ()) {	//のちのちOnCollisionEnterでやると思われる
//			_animController.BlockHitAnim ();
//		}
//		//---------------------------------------
//		
//		//死亡-----------------------------------
//		if (_enemyHealth.isDead) {
//			_animController.DeadAnim ( );
//			Debug.Log (gameObject.name + "death");
//			enabled = false;	//死亡したら処理をやめる
//		} 
//		//---------------------------------------
//		
//		if (Input.GetKeyDown (KeyCode.Alpha0)) {	//デバッグ用ダメージ処理
//			_enemyHealth.TakeDamage (1f);
//		}
//		
//		//爆弾を見たら逃げる処理---------------------------------------------------------------------------------------------------------
//		//※逃げてる途中で爆弾がなくなったらどうなるか要検証
//		if (Input.GetKeyDown (KeyCode.B) && _fieldOfView.IsInFieldOfView (_target)) {
//			SetFindedBomb (_target);	//デバッグ用として_targetを爆弾とみなす（のちに外部のスクリプトから爆弾をセットする処理を記述）
//		}
//		if (_findedBomb) {
//			if (_fieldOfView.IsInFieldOfView (_findedBomb) && !_animController.CheckAttacking ()) {	//攻撃状態じゃないときに爆弾を見つけたら逃げる
//				float escapeDistance = 20;
//				Vector3 dir = transform.position - _findedBomb.position;
//				if (dir == Vector3.zero) {	//dirが零ベクトルだった時の処理
//					dir = Vector3.forward;
//				}
//				_enemyNav.MoveToTarget (_findedBomb.position + dir.normalized * escapeDistance);
//			}
//			if (_agent.velocity.magnitude <= 0) {	//逃げた後爆弾のあった方向を向く処理
//				FaceTarget (_findedBomb);
//				Vector3 vecA = transform.InverseTransformDirection ((_findedBomb.position - transform.position).normalized);
//				Vector3 vecB = transform.InverseTransformDirection (transform.forward);
//				if (Vector3.Angle (vecA, vecB) <= 1.0f) {
//					SetFindedBomb (null);
//				}
//			}
//		}
//		//-------------------------------------------------------------------------------------------------------------------------------
//		
//		//音に反応する処理----------------------------------------------------------------------------------------------------------------
//		if (!_animController.CheckAttacking () && _agent.velocity.magnitude <= 0) {	//戦闘待機状態でない　＆　移動してない時
//			int count = _soundReaction.GetAudioSourceList().Count;	//_audioSountListの数
//			Transform soundTrans = transform;						//聞こえた音のtransformを格納する変数
//			bool setFlag = false;									//soundTransformに値を格納したか確認するフラグ
//			//音が聞こえているか確認---------------------------------------------------------
//			for (int i = 0; i < count; i++) {
//				if (_soundReaction.Hear (i)) {
//					Transform trans = _soundReaction.GetAudioSourceList () [i].transform;
//					if (!setFlag
//						|| Vector3.Distance (transform.position, trans.position) < Vector3.Distance (transform.position, soundTrans.position)) {	//より近い方をsoundTransformに格納
//						soundTrans = trans;
//						setFlag = true;
//					}
//				}
//			}
//			//-------------------------------------------------------------------------------
//			//音の聞こえた方向を向く処理-----------------------------------------
//			if (setFlag) {
//				FaceTarget (soundTrans);
//				Debug.Log (gameObject.name + " here " + soundTrans.gameObject);
//			}
//			//------------------------------------------------------------------
//		}
//		//-------------------------------------------------------------------------------------------------------------------------------
//	}
//
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
