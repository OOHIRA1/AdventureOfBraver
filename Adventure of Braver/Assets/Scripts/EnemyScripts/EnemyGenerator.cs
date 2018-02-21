using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//機能：エネミーを生成するスクリプト
//
//アタッチ：常にアクティブなゲームオブジェクトにアタッチ
public class EnemyGenerator : MonoBehaviour {
	[SerializeField] List<GameObject> _goblinList = new List<GameObject>();

	// Use this for initialization
	void Start () {
		
	}

	//---------------
	//ゲッター
	//---------------
	public List<GameObject> GetGoblinList( )
	{
		return _goblinList;
	}
	//----------------
	//----------------

	// Update is called once per frame
	void Update () {
		
	}
}
