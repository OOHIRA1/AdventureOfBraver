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
	private bool _cursorMoveFlag;											//カーソルを動かせるかどうかのフラグ(項目選択時・フェードイン中に動かせないようにするため)



	// Use this for initialization
	void Start () {
		_buttonIndex = 1;
		transform.position = new Vector3 ( transform.position.x, _buttons[_buttonIndex].transform.position.y, transform.position.z );
		_cursorMoveFlag = false;
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


	//---カーソルを動かす関数(キーボード操作)
	void CursorMove() {
		if (Input.GetKeyDown (KeyCode.W)) {
			_buttonIndex--;
			if (_buttonIndex > -1) {
				transform.position = new Vector3 (transform.position.x, _buttons [_buttonIndex].transform.position.y, transform.position.z);
				_audio.clip = _clips [0];
				_audio.Play ();
			} else {
				_buttonIndex = 0;
			}
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			_buttonIndex++;
			if (_buttonIndex < _buttons.Length) {
				transform.position = new Vector3 (transform.position.x, _buttons [_buttonIndex].transform.position.y, transform.position.z);
				_audio.clip = _clips [1];
				_audio.Play ();
			} else {
				_buttonIndex = 2;
			}
		}
		if (Input.GetKeyDown (KeyCode.Return)) {
			_cursorMoveFlag = false;
			_audio.clip = _clips [2];
			_audio.Play ();
			if (transform.position.y == _buttons [0].transform.position.y) {
				_sceneManager.GetComponent<SceneTransition> ().RequestSceneChange ("test");
			} else if (transform.position.y == _buttons [1].transform.position.y) {
				_sceneManager.GetComponent<SceneTransition> ().RequestSceneChange ("Option");
			} else {
				Debug.Log ("ゲームの終了");
				//Application.Quit ();		←アプリケーションの時の処理
				UnityEditor.EditorApplication.isPlaying = false;
			}
		}
	}
}
