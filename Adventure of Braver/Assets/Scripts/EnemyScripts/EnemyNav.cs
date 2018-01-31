using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    public void MoveToTarget( Vector3 targetPos)
    {
        navAgent.SetDestination(targetPos);
    }
}

