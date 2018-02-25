using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//機能：エネミーの音の反応を管理するスクリプト
//
//アタッチ：反応したいエネミーにアタッチ
public class EnemySoundReaction : MonoBehaviour {
	[SerializeField] List<AudioSource> _audioSourceList = new List<AudioSource>();	//エネミーが反応するAudioSourceのリスト

	//=================================================
	//ゲッター
	//=================================================
	public List<AudioSource> GetAudioSourceList() {
		return _audioSourceList;
	}
	//=================================================
	//=================================================


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	//========================================================================================================
	//public関数
	//========================================================================================================
	//--エネミーが音を聞こえたかどうかを判定する関数
	public bool Hear( int index ) {
		if (index >= _audioSourceList.Count) return false;		//_audioSourceListにあるかどうかの判定
		if (!_audioSourceList [index].isPlaying) return false;	//音が鳴ているかどうかの判定
		bool flag = false;
		float distance = Vector3.Distance ( transform.position, _audioSourceList[index].transform.position );
		if (distance < _audioSourceList [index].maxDistance) {
			flag = true;
		}
		return flag;
	}
	//=========================================================================================================
	//=========================================================================================================
}
