using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 50f;
    public GameObject deathEffect; // Drag your Particle Prefab here

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        // 1. Add Score 
        PlayerHealth playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        if (playerScript != null) playerScript.AddKill();

        // 2. Spawn the Explosion Effect
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // 3. Destroy the Enemy Instantly
        Destroy(gameObject);
    }
}