using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WandererAI : MonoBehaviour {

    DialogueTrigger dialogueTrigger;
     
    public float wanderRadius;
    public float wanderTimer;

    private Transform myTransform;
    private Transform target;
    private NavMeshAgent agent;
    private float timer;

    float damping = 5f;

    void OnEnable()
    {
        myTransform = transform;
        dialogueTrigger = GetComponent<DialogueTrigger>();
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
    }

    private void Start()
    {
        target = PlayerManager.instance.player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogueTrigger.isTalking)
        {
            agent.isStopped = true;

            var lookPos = target.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);

            var playerLookPos = transform.position - target.position;
            playerLookPos.y = 0;
            var playerRotation = Quaternion.LookRotation(playerLookPos);
            target.rotation = Quaternion.Slerp(target.rotation, playerRotation, Time.deltaTime * damping);

        }
        else
        {
            agent.isStopped = false;
            timer += Time.deltaTime;
            if (timer >= wanderTimer)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPos);
                timer = 0;
            }
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
}
