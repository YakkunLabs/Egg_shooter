using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 50f;
    public float damage = 10f;
    public float lifetime = 3f;

    [Header("Visuals")]
    public GameObject enemyHitEffect;

    private Rigidbody rb;

    void Start()
    {
        // 1. Get Rigidbody
        rb = GetComponent<Rigidbody>();

        // 2. APPLY VELOCITY (This replaces transform.Translate)
        // This makes the physics engine handle the movement, ensuring collisions work.
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
        }

        Destroy(gameObject, lifetime);
    }

    // Note: We do NOT use Update() anymore. The Rigidbody handles movement.

    void OnCollisionEnter(Collision collision)
    {
        // Debugging: Print what we hit to the console
        Debug.Log("Bullet hit: " + collision.gameObject.name);

        Target enemy = collision.gameObject.GetComponent<Target>();
        
        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            if (enemyHitEffect != null)
            {
                ContactPoint hit = collision.contacts[0];
                Instantiate(enemyHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
        
        // Don't destroy if we just hit the player (to prevent self-damage bugs)
        if (collision.gameObject.CompareTag("Player")) return;

        Destroy(gameObject);
    }
}