using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 50f;

    // This function will be called by your Gun
    public void TakeDamage(float amount)
    {
        health -= amount;

        // If health runs out, the egg dies
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        // Simple destruction for now
        Destroy(gameObject);
        
        // OPTIONAL: Later you can spawn a "Scrambled Egg" particle effect here
        // Instantiate(scrambledEggEffect, transform.position, Quaternion.identity);
    }
}