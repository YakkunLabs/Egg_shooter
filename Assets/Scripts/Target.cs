using UnityEngine;

public class Target : MonoBehaviour
{
    public float maxHealth = 50f;
    public float currentHealth;
    
    // Drag the "CrackShell" object here in the Inspector
    public Renderer crackShellRenderer; 

    public GameObject deathEffect;

    void Start()
    {
        currentHealth = maxHealth;
        
        // Ensure cracks are invisible at start
        if (crackShellRenderer != null)
        {
            Color c = crackShellRenderer.material.color;
            c.a = 0f; // 0 Alpha = Invisible
            crackShellRenderer.material.color = c;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        // --- UPDATE CRACKS ---
        if (crackShellRenderer != null)
        {
            // Calculate damage % (0 is healthy, 1 is dead)
            float damagePercent = 1f - (currentHealth / maxHealth);
            
            // Get current color
            Color c = crackShellRenderer.material.color;
            
            // Set transparency equal to damage
            c.a = damagePercent; 
            
            // Apply it back
            crackShellRenderer.material.color = c;
        }
        // ---------------------

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (deathEffect != null) Instantiate(deathEffect, transform.position, Quaternion.identity);

        PlayerHealth playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        if (playerScript != null)
        {
            playerScript.AddKill();
        }

        Destroy(gameObject);
    }

}