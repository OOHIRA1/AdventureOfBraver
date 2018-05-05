using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==TPSカメラワークを管理するクラス
//
//使用方法：TPSカメラワークをするカメラにアタッチ
public class TPSCamera1 : MonoBehaviour {
	[SerializeField] GameObject _target = null;
	[SerializeField] Vector3 _firstTToCPoint = new Vector3(0, 0.3f, -3f);	//_targetから見たカメラ(TargetToCamera)の初期座標
	[SerializeField] float _firstDistance = 0;									//_targetのy軸との距離
	[SerializeField] Vector3 _currentVelocity = Vector3.zero;
	[SerializeField] Vector3 _tToCPoint = Vector3.zero;
	public float _smoothTime = 0.1f;
	public GameObject _z;
	float a;


	// Use this for initialization
	void Start () {
		transform.position = _target.transform.position + _firstTToCPoint;
		Vector2 targetVec = new Vector2 ( _target.transform.position.x, _target.transform.position.z );
		Vector2 cameraVec = new Vector2 (transform.position.x, transform.position.z);
		_firstDistance = Vector2.Distance ( targetVec, cameraVec );
		_tToCPoint = _firstTToCPoint;
		Vector3 point = new Vector3 ( _target.transform.position.x, transform.position.y, _target.transform.position.z );
		Vector3 line = new Vector3 (transform.position.x - point.x, 0, transform.position.z - point.z);
		a = -Vector3.Angle ( Vector3.right, line );
	}
	
	// Update is called once per frame
	void Update () {
		float distance = _z.transform.position.magnitude;
		_z.transform.position = new Vector3 ( distance*Mathf.Cos(a), _z.transform.position.y, distance*Mathf.Sin(a) );
		//a+= 0.1f;
		Debug.Log(a);
	}


	void LateUpdate() {
		//transform.LookAt ( _target.transform.position );	//こいつを使うとブレます！！
		if (Vector3.Distance( _target.transform.position + _tToCPoint,  transform.position ) > 0f) {
			//transform.position = Vector3.SmoothDamp (transform.position, _target.transform.position + _tToCPoint, ref _currentVelocity, _smoothTime);
			transform.position = _target.transform.position + _tToCPoint;
		}
		Vector3 point = new Vector3 ( _target.transform.position.x, transform.position.y, _target.transform.position.z );
		Vector2 line = new Vector2 (transform.position.x - point.x, transform.position.z - point.z);
		float anglePerflame = 3.14f / 180f;
		if (Input.GetAxis ("CameraHorizontal") != 0) {
			if (Input.GetAxis ("CameraHorizontal") > 0) {
				a += anglePerflame;
			} else {
				a -= anglePerflame;
			}

			Vector3 movedLine = new Vector3 (line.magnitude * Mathf.Cos (a), 0, line.magnitude * Mathf.Sin (a));
			transform.position = point + movedLine;
			//transform.position = Vector3.MoveTowards( transform.position,  point + movedLine, 1f );
			transform.forward = Vector3.RotateTowards (transform.forward, -movedLine.normalized, Vector3.Angle(transform.forward, -movedLine), 0);
			_tToCPoint = transform.position - _target.transform.position;

			//transform.RotateAround (point, Vector3.up, Input.GetAxis ("CameraHorizontal"));	//こいつを使うと回転軸が動いた時の挙動がおかしいです！
			if (Input.GetAxis ("Horizontal") == 0 && Input.GetAxis ("Vertical") == 0) {
				_tToCPoint = transform.position - _target.transform.position;
			}
		}
		//カメラの位置を初期位置に戻す処理(未完成)----------------------------------------------------------------------------------------------------
		if (Input.GetAxis ("CameraCancel") != 0) {
			_tToCPoint = _target.transform.InverseTransformPoint( _target.transform.position + _firstTToCPoint);
			transform.position = _target.transform.position + _tToCPoint;
			Vector3 point2 = new Vector3 ( _target.transform.position.x, transform.position.y, _target.transform.position.z );
			Vector3 line2 = new Vector3 (transform.position.x - point.x, 0, transform.position.z - point.z);
			transform.forward = Vector3.RotateTowards (transform.forward, -line2.normalized, Vector3.Angle(transform.forward, -line2), 0);
			a = -Vector3.Angle ( Vector3.right, line );
		}
		//------------------------------------------------------------------------------------------------------------------------------------
	}
}
