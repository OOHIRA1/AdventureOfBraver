using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {

    [SerializeField] private float maxHealth;
    public float currentHealth { get; private set; }
    public bool isDead = false;

	void Awake () {
        currentHealth = maxHealth;
    }

    public void TakeDamage( float amount )
    {
        currentHealth -= amount;
        Debug.Log(transform.name + " takes " + amount + " damage.");

        if (currentHealth <= 0)
        {
            isDead = true;
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject, 1f);
    }
}
