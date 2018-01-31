using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//機能：カーソルを動かす
//
//アタッチ：Cursorにアタッチする
public class CursorController : MonoBehaviour {

	[SerializeField] private GameObject[] _buttons = new GameObject[3];
	private int _buttonIndex;
	[SerializeField] private AudioSource _audio = null;
	[SerializeField] private AudioClip[] _clips = new AudioClip[3];
	[SerializeField] private GameObject _sceneManager = null;
	[SerializeField] private string[] _sceneName = new string[2];
	private bool _cursorMoveFlag;											//カーソルを動かせるかどうかのフラグ(項目選択時・フェードイン中に動かせないようにするため)
	private float[] _freezeTime = new float[2];												//ボタン入力を受け付けない時間（0:上 1:下）



	// Use this for initialization
	void Start () {
		_buttonIndex = 1;
		transform.position = new Vector3 ( transform.position.x, _buttons[_buttonIndex].transform.position.y, transform.position.z );
		_cursorMoveFlag = false;
		for (int i = 0; i < _freezeTime.Length; i++) {
			_freezeTime [i] = 0f;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (_sceneManager.GetComponent<SceneTransition> ().GetSceneStartFlag ()) {
			_cursorMoveFlag = true;
		}
		if (_cursorMoveFlag) {
			CursorMove ();
		}
	}


	//---カーソルを動かす関数(キーボード操作 or コントローラー操作)
	void CursorMove() {
		if (Input.GetAxis ("Vertical") > 0.1f && _freezeTime[0] <= 0f) {
			_buttonIndex--;
			if (_buttonIndex > -1) {
				transform.position = new Vector3 (transform.position.x, _buttons [_buttonIndex].transform.position.y, transform.position.z);
				_audio.clip = _clips [0];
				_audio.Play ();
			} else {
				_buttonIndex = 0;
			}
			_freezeTime [0] = 1f;
			_freezeTime [1] = 0f;
		}
		if (Input.GetAxis ("Vertical") < -0.1f && _freezeTime[1] <= 0f) {
			_buttonIndex++;
			if (_buttonIndex < _buttons.Length) {
				transform.position = new Vector3 (transform.position.x, _buttons [_buttonIndex].transform.position.y, transform.position.z);
				_audio.clip = _clips [1];
				_audio.Play ();
			} else {
				_buttonIndex = 2;
			}
			_freezeTime [0] = 0f;
			_freezeTime [1] = 1f;
		}
		if (Input.GetKeyDown (KeyCode.JoystickButton0) || Input.GetKeyDown (KeyCode.Return)) {
			_cursorMoveFlag = false;
			_audio.clip = _clips [2];
			_audio.Play ();
			if (transform.position.y == _buttons [0].transform.position.y) {
				_sceneManager.GetComponent<SceneTransition> ().RequestSceneChange (_sceneName[0]);
			} else if (transform.position.y == _buttons [1].transform.position.y) {
				_sceneManager.GetComponent<SceneTransition> ().RequestSceneChange (_sceneName[1]);
			} else {
				Debug.Log ("ゲームの終了");
				//Application.Quit ();		←アプリケーションの時の処理
				UnityEditor.EditorApplication.isPlaying = false;
			}
		}
		for (int i = 0; i < _freezeTime.Length; i++) {
			if (_freezeTime [i] > 0f) {
				_freezeTime [i] -= Time.deltaTime;
			}
			if (Input.GetAxis ("Vertical") > -0.1f && Input.GetAxis ("Vertical") < 0.1f) {
				_freezeTime [i] = 0f;
			}
		}
	}
}
