using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

    [SerializeField] float attackPower, attackCooldown;
    private float attackDelay = 1f;

    Animator animator;
    Targeting targeting;
    EnemyHealth enemyHealth;

	void Start () {

        animator = GetComponent<Animator>();
        targeting = GetComponent<Targeting>();

	}

    void Update()
    {
        attackCooldown -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && attackCooldown <= 0)
        {
            StartCoroutine("Attack");
        }
    }

    void GetEnemyHealth()
    {
        enemyHealth = targeting.selectedTarget.GetComponent<EnemyHealth>();
    }


    IEnumerator Attack()
    {
        yield return new WaitForSeconds(0);
        GetEnemyHealth();
        attackCooldown = attackDelay;
        animator.SetTrigger("Attack");
        enemyHealth.TakeDamage( attackPower );
    }
}
