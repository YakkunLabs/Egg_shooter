using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Settings")]
    public float delay = 3f;
    public float radius = 5f;
    public float force = 700f;
    public float damage = 100f; // <-- NEW: How much damage it deals
    
    [Header("Effects")]
    public GameObject explosionEffect;
    public AudioClip explosionSound; 

    float countdown;
    bool hasExploded = false;

    void Start()
    {
        countdown = delay;
    }

    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    void Explode()
    {
        // 1. Show Visual Effect
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, transform.rotation);
        }

        // 2. Play Sound
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        // 3. Find nearby objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        
        foreach (Collider nearbyObject in colliders)
        {
            // --- A. PHYSICS LOGIC (Push things) ---
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force, transform.position, radius);
            }

            // --- B. DAMAGE LOGIC (Kill things) ---
            // Check if the object we hit has the "Target" script (Enemy Health)
            Target enemy = nearbyObject.GetComponent<Target>();
            
            // If it DOES have the script, deal damage!
            if (enemy != null)
            {
                // Optional: Deal less damage if they are far away from the center
                // For now, we just kill them instantly if they are in range
                enemy.TakeDamage(damage);
            }
            
            // (Note: If your player has a "PlayerHealth" script, 
            // add a check here if you want the grenade to hurt YOU too!)
        }

        // 4. Destroy the grenade
        Destroy(gameObject);
    }
}