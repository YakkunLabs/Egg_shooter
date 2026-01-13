using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Settings")]
    public float delay = 3f;
    public float radius = 5f;
    public float force = 700f;
    
    [Header("Effects")]
    public GameObject explosionEffect;
    public AudioClip explosionSound; // <-- NEW: Drag your BOOM sound here

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

        // 2. Play Sound (The "Ghost Speaker" Trick)
        // This creates a temporary object at the explosion spot just to play the sound
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        // 3. Physics & Damage Logic
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force, transform.position, radius);
            }
            // Add damage logic here if needed
        }

        // 4. Destroy the grenade
        Destroy(gameObject);
    }
}