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
        // NEW: Find the player and add score
        PlayerHealth playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        if (playerScript != null)
        {
            playerScript.AddKill();
        }

        Destroy(gameObject);
    }
}