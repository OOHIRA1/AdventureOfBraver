using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//機能：視野を表示するスクリプト
//
//アタッチ：視野が必要なゲームオブジェクトにアタッチ
public class FieldOfView : MonoBehaviour {
	[SerializeField] float _lookRadius = 10f;		//見える範囲(距離)
	[SerializeField] float _viewAngle = ( float )3.14 / 2;	//視野角
	[SerializeField] Transform _target = null;					//ターゲット(Gizmos用)
	[SerializeField] bool _drawGismos = true;					//ギズモ表示するかどうかのフラグ

	//----------------------------------------
	//セッター
	//-----------------------------------------
	public void SetTarget( Transform x ) {
		_target = x;
	}
	public void SetDrawGizmos( bool x ) {
		_drawGismos = x;
	}
	//-----------------------------------------
	//-----------------------------------------

	//-----------------------------------------
	//ゲッター
	//-----------------------------------------
	public float GetLookRadius() {
		return _lookRadius;
	}
	public float GetViewAngle() {
		return _viewAngle;
	}
	//-----------------------------------------
	//-----------------------------------------


	// Use this for initialization
	void Start () {
		if (!_target) {
			Debug.LogError ("Inspector上から_targetを設定してください");
			UnityEditor.EditorApplication.isPlaying = false;
		}
		_drawGismos = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	//--選択時視野ギズモ表示する関数
	void OnDrawGizmosSelected() {
		if (!_drawGismos) return;

		Gizmos.color = Color.blue;
		if (IsInFieldOfView (_target)) {
			Gizmos.color = Color.red;
		}
		//視野に使用するベクトル-----------------------------------------------------------------------------------
		Vector3 forwardPos = transform.position + transform.forward * _lookRadius;
		Vector3 rightForwardPos = forwardPos + transform.right * _lookRadius * Mathf.Tan (_viewAngle / 2);	
		Vector3 leftForwardPos = forwardPos + (-transform.right) * _lookRadius * Mathf.Tan (_viewAngle / 2);
		//--------------------------------------------------------------------------------------------------------
		//ギズモの描画---------------------------------------------
		Gizmos.DrawLine (transform.position, forwardPos);
		Gizmos.DrawLine (rightForwardPos, leftForwardPos);
		Gizmos.DrawLine (transform.position, rightForwardPos);
		Gizmos.DrawLine (transform.position, leftForwardPos);
		//---------------------------------------------------------

	}




	//---------------------------
	//public関数
	//---------------------------
	//--視野内にtargetがいるかどうか判定する関数
	public bool IsInFieldOfView( Transform target ) {
		bool flag = false;
		//視野に使用するベクトル-----------------------------------------------------------------------------------
		Vector3 forwardPos = transform.position + transform.forward * _lookRadius;
		Vector3 rightForwardPos = forwardPos + transform.right * _lookRadius * Mathf.Tan (_viewAngle / 2);	
		Vector3 leftForwardPos = forwardPos + (-transform.right) * _lookRadius * Mathf.Tan (_viewAngle / 2);
		//--------------------------------------------------------------------------------------------------------
		//視野内に_targetが入っているかの処理---------------------------
		//方法１：領域
		/*
		//ローカル座標への変換--------------------------------------------------
		Vector3 localFPos = transform.InverseTransformPoint (forwardPos);
		Vector3 localRFPos = transform.InverseTransformPoint (rightForwardPos);
		Vector3 localLFPos = transform.InverseTransformPoint (leftForwardPos);
		Vector3 localTargetPos = transform.InverseTransformPoint (target.position);	//ローカル座標から見たtargetの座標
		//---------------------------------------------------------------------
		if (	localTargetPos.z < localFPos.z
				&& localTargetPos.z >= localLFPos.z / localLFPos.x * localTargetPos.x
				&& localTargetPos.z >= localRFPos.z / localRFPos.x * localTargetPos.x ) {
			Gizmos.color = Color.red;
		}
		*/
		//方法２：外積
		//ローカル座標への変換--------------------------------------------------
		Vector3 localRFPos = transform.InverseTransformPoint (rightForwardPos);
		Vector3 localLFPos = transform.InverseTransformPoint (leftForwardPos);
		Vector3 localTargetPos = transform.InverseTransformPoint (target.position);	//ローカル座標から見たtargetの座標
		//---------------------------------------------------------------------
		if (	Vector3.Cross (localTargetPos, localLFPos).y < 0
		   		&& Vector3.Cross (localTargetPos - localLFPos, localRFPos - localLFPos).y < 0
		    	&& Vector3.Cross (localTargetPos - localRFPos, Vector3.zero - localRFPos).y < 0) {
			flag = true;
		}
		return flag;
	}
	//---------------------------------------------------
	//----------------------------------------------------


}