using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//機能：シーン遷移を行うスクリプト
//
//アタッチ：常にアクティブなGameObjectにアタッチ
public class SceneTransition : MonoBehaviour {
	string _sceneName;								//遷移先のシーン
	Image _fadeInOutPanel;							//シーン遷移時にフェードアウトするときに使用するパネル
	float _alpha;									//_fadeInOutPanelのアルファ値
	bool _sceneChangeFlag;							//シーン遷移を行うかどうかのフラグ
	bool _sceneStartFlag;							//シーンを始められるかどうかのフラグ


	//------------------------------------
	//--ゲッター
	//------------------------------------
	public bool GetSceneStartFlag( ) {
		return _sceneStartFlag;
	}
	//------------------------------------
	//------------------------------------


	//------------------------------------
	//--セッター
	//------------------------------------
	public void SetSceneName( string x ) {
		_sceneName = x;
	}
	public void SetChangeSceneFlag( bool x ) {
		_sceneChangeFlag = x;
	}
	//------------------------------------
	//------------------------------------


	// Use this for initialization
	void Start () {
		_fadeInOutPanel = GameObject.Find ("FadeInOutPanel").GetComponent<Image>();
		SetChangeSceneFlag ( false );
		_alpha = 1f;
		_fadeInOutPanel.color = new Color (0,0,0,_alpha);
		_sceneStartFlag = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (_sceneChangeFlag) {
			StartCoroutine (ChangeSceneWithFadeOut(_sceneName));
		}
		if (!GetSceneStartFlag( )) {
			FadeIn ( );
		}

	}



	//--フェードアウト後シーン遷移を行う関数(コルーチン)
	IEnumerator ChangeSceneWithFadeOut( string x ) {
		SetSceneName (x);
		_fadeInOutPanel.color = new Color (0, 0, 0, _alpha);
		_alpha += 0.01f;
		if (_alpha >= 1f) {
			SceneManager.LoadSceneAsync (x);
		}
		yield return null;
	}
		
	//--フェードインでシーン開始を行う関数
	void FadeIn( ) {
		_alpha -= 0.03f;
		_fadeInOutPanel.color = new Color (0, 0, 0, _alpha);
		if (_alpha <= 0f) {
			_sceneStartFlag = true;
		}
	}


	//-----------------------------------
	//--Public関数
	//-----------------------------------

	//--シーン遷移を要求する関数
	public void RequestSceneChange( string x ) {
		SetSceneName ( x );
		SetChangeSceneFlag (true);
	}
	//-------------------------------------
	//-------------------------------------
}
