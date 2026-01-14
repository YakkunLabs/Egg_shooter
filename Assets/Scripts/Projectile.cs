using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 100f;
    public float damage = 10f;
    public float explosionRadius = 0f; // 0 = Bullet, >0 = Rocket
    public float lifetime = 5f;

    [Header("Visuals")]
    public GameObject impactEffect; // Assign an explosion particle here for rockets
    public GameObject explosionSoundPrefab; // Optional: Prefab with AudioSource that plays on death

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime); 

        // --- NEW: IGNORE PLAYER & GUN COLLISIONS ---
        Collider myCollider = GetComponent<Collider>();
        
        // 1. Find the Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && myCollider != null)
        {
            // Ignore the main Character Controller/Collider
            Collider playerCollider = player.GetComponent<Collider>();
            if (playerCollider != null) Physics.IgnoreCollision(myCollider, playerCollider);
            
            // 2. Ignore all children colliders (like the Gun itself)
            Collider[] allPlayerColliders = player.GetComponentsInChildren<Collider>();
            foreach (Collider c in allPlayerColliders)
            {
                Physics.IgnoreCollision(myCollider, c);
            }
        }
    }
    void FixedUpdate()
    {
        // Move the bullet/rocket forward
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 1. Play Impact Effect
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        // 2. Handle Damage
        if (explosionRadius > 0)
        {
            Explode(); // It's a Rocket!
        }
        else
        {
            DirectHit(collision.gameObject); // It's a Bullet!
        }

        // 3. Destroy Projectile
        Destroy(gameObject);
    }

    void DirectHit(GameObject target)
    {
        // Find enemy script (Target, EnemyHealth, etc.)
        Target enemy = target.GetComponent<Target>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }

    void Explode()
    {
        // Find all colliders in the radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        
        foreach (Collider nearbyObject in colliders)
        {
            // Damage Enemy
            Target enemy = nearbyObject.GetComponent<Target>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); 
            }

            // Physics Push
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(700f, transform.position, explosionRadius);
            }
        }
    }
}