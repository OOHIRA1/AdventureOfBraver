using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==TPSカメラワークを管理するクラス
//
//使用方法：TPSカメラワークをするカメラにアタッチ
public class TPSCamera1 : MonoBehaviour {
	const float PI = 3.14f;
	const float ANGLE_PER_SECOND = PI / 3f;								//角速度[rad / second]

	[SerializeField] Transform _targetTransform = null;
	[SerializeField] Vector3 _firstTToCPoint = new Vector3(0, 0.3f, -3f);	//_targetから見たカメラ(TargetToCamera)の初期座標
	Vector3 _tToCPoint = Vector3.zero;										//_targetから見たカメラ(TargetToCamera)の座標
	float _angle;															//_tToCPointのzx平面への正射影ベクトルのなす角
	//[SerializeField] float _firstDistance = 0;								//_targetのy軸との距離
	//[SerializeField] Vector3 _currentVelocity = Vector3.zero;				//カメラゆっくり近づく速度
	//[SerializeField] float _smoothTime = 0.1f;							//カメラが近づき終わる時間


	// Use this for initialization
	void Start () {
		transform.position = _targetTransform.position + _firstTToCPoint;
		//Vector2 targetVec = new Vector2 ( _target.transform.position.x, _target.transform.position.z );
		//Vector2 cameraVec = new Vector2 (transform.position.x, transform.position.z);
		//_firstDistance = Vector2.Distance ( targetVec, cameraVec );
		_tToCPoint = _firstTToCPoint;
		_angle = -Vector3.Angle ( Vector3.right, Vector3.ProjectOnPlane(_tToCPoint, Vector3.up) );
	}
	
	// Update is called once per frame
	void Update () {
	}


	void LateUpdate() {
		//カメラを追従する処理-------------------------------------------------------------------------------------
		if (_targetTransform.position + _tToCPoint - transform.position != Vector3.zero) {
			transform.position = _targetTransform.position + _tToCPoint;
			//transform.position = Vector3.SmoothDamp (transform.position, _target.transform.position + _tToCPoint, ref _currentVelocity, _smoothTime);	//ゆっくりカメラが近づく処理
		}
		//--------------------------------------------------------------------------------------------------------

		//カメラをターゲットを向きながら円運動させる処理----------------------------------------------------------------------------------------------
		if (Input.GetAxis ("CameraHorizontal") != 0) {
			//回転--------------------------------------------------------------------------------------------------------------------------
			Vector3 centerOfRotation = new Vector3 ( _targetTransform.position.x, transform.position.y, _targetTransform.position.z );	//回転の中心
			Vector3 rotationVector = Vector3.ProjectOnPlane (_tToCPoint, Vector3.up);	//回転するベクトル
			_angle += ( ANGLE_PER_SECOND * Input.GetAxis ("CameraHorizontal") ) * Time.deltaTime;
			rotationVector = new Vector3 (rotationVector.magnitude * Mathf.Cos (_angle), 0, rotationVector.magnitude * Mathf.Sin (_angle));
			transform.position = centerOfRotation + rotationVector;
			//transform.position = Vector3.MoveTowards( transform.position,  point + rotationVector, 1f );	//これでも出来る
			//------------------------------------------------------------------------------------------------------------------------------

			//カメラをターゲットに向かせる----------------------------------------------------------------------------------------------------
			transform.forward = Vector3.RotateTowards (transform.forward, -rotationVector.normalized, Vector3.Angle(transform.forward, -rotationVector), 0);
			//transform.LookAt ( _target.transform.position );	//こいつを使うとブレます！！
			//transform.RotateAround (centerOfRotation, Vector3.up, Input.GetAxis ("CameraHorizontal"));	//こいつを使うと回転の中心が動いた時の挙動がおかしい！
			//-------------------------------------------------------------------------------------------------------------------------------

			_tToCPoint = transform.position - _targetTransform.position;	//_tToCPointの更新
		}
		//------------------------------------------------------------------------------------------------------------------------------------------

		//カメラの位置を初期位置に戻す処理(未完成)----------------------------------------------------------------------------------------------------
		if (Input.GetAxis ("CameraCancel") != 0) {
			_tToCPoint = _targetTransform.InverseTransformPoint( _targetTransform.position + _firstTToCPoint);
			transform.position = _targetTransform.position + _tToCPoint;
			Vector3 point2 = new Vector3 ( _targetTransform.position.x, transform.position.y, _targetTransform.position.z );
			Vector3 line2 = new Vector3 (transform.position.x - point2.x, 0, transform.position.z - point2.z);
			transform.forward = Vector3.RotateTowards (transform.forward, -line2.normalized, Vector3.Angle(transform.forward, -line2), 0);
			_angle = -Vector3.Angle ( Vector3.right, line2 );
		}
		//------------------------------------------------------------------------------------------------------------------------------------
	}
}
