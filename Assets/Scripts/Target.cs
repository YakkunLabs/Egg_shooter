using UnityEngine;

public class Target : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    
    // Drag the "CrackShell" object here in the Inspector
    public Renderer crackShellRenderer; 

    public GameObject deathEffect;

    [Header("Health Bar")]
    public GameObject healthBarPrefab; // Drag your Canvas Prefab here
    private HealthBar currentHealthBar;

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

        if (healthBarPrefab != null)
        {
            // Spawn it 2 meters above the enemy
            GameObject barObj = Instantiate(healthBarPrefab, transform.position + Vector3.up * 2f, Quaternion.identity, transform);
            currentHealthBar = barObj.GetComponent<HealthBar>();

            // Setup the bar
            currentHealthBar.SetMaxHealth(currentHealth);
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

        // 2. Update the visual bar
        if (currentHealthBar != null)
        {
            currentHealthBar.SetHealth(currentHealth);
        }

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