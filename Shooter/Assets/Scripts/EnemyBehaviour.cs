using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public float health = 100f;
    public bool isDead = false;

    void Die()
    {
        Destroy(gameObject);
    }

    public void TakeDamage(float ammount)
    {
        health -= ammount;
        if (health <= 0)
        {
            isDead = true;
            Die();
        }
    }
}
