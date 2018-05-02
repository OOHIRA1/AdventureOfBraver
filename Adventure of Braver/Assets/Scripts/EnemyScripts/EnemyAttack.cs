using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==エネミーの攻撃を管理するクラス
//
//使用方法：攻撃判定に使うコライダーのついたゲームオブジェクトにアタッチ
public class EnemyAttack : MonoBehaviour {
	[SerializeField] Collider _attackTrigger;	//攻撃の当たり判定に使うトリガー
	[SerializeField] EnemyController _enemyController = null;
	[SerializeField] string[] _attackStateName = new string[3];

	// Use this for initialization
	void Start () {
		_attackTrigger = GetComponent<Collider> ();
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
		switch (_enemyController.GetState ()) {
		case EnemyController.State.ATTACK1:
			Debug.Log ("attack1 hit!");
			break;
		case EnemyController.State.ATTACK2:
			Debug.Log ("attack2 hit!");
			break;
		case EnemyController.State.ATTACK3:
			Debug.Log ("attack3 hit!");
			break;
		default:
			break;
		}
	}

}
