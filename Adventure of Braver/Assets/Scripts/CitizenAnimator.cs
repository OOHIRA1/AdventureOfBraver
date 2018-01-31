using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CitizenAnimator : MonoBehaviour
{

    const float locomotionAnimationSmootTime = .05f;

    NavMeshAgent agent;
    Animator anim;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float speedPercent = agent.velocity.magnitude / agent.speed;
        anim.SetFloat("speedPercent", speedPercent, locomotionAnimationSmootTime, Time.deltaTime);
    }
}