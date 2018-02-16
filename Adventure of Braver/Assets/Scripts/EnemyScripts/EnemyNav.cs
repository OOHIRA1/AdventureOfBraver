using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//機能：エネミーのNavMeshAgentを管理するスクリプト
//
//アタッチ：エネミーにアタッチ
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyNav : MonoBehaviour
{
    NavMeshAgent navAgent;
    Transform target;

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        target = PlayerManager.instance.player.transform;
    }


	//--移動先の目的地点をtargetPasに変更する関数
    public void MoveToTarget( Vector3 targetPos)
    {
        navAgent.SetDestination(targetPos);
    }
}

