using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==エネミーの攻撃を管理するクラス
//
//使用方法：攻撃判定に使うコライダーのついたゲームオブジェクトにアタッチ
public class EnemyAttack : MonoBehaviour {
	[SerializeField] EnemyController _enemyController = null;
	[SerializeField] string[] _attackStateName = new string[3];

	// Use this for initialization
	void Start () {
		if (!_enemyController) {
			Debug.LogError ("Inspector上から_enemyControllerを設定してください");
			UnityEditor.EditorApplication.isPlaying = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
	}


	void OnTriggerEnter( Collider collider ) {
		if (collider.gameObject.tag != "Player") return;
		for (int i = 0; i < _attackStateName.Length; i++) {
			if (_enemyController.GetState ().ToString () == _attackStateName [i]) {
				Debug.Log (_attackStateName[i] + " hit!");
				break;
			}
		}
	}

}
