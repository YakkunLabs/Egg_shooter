using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 50f;
    public float damage = 10f;
    public float explosionRadius = 0f; // Set this for rockets
    public float lifetime = 3f;

    [Header("Visuals")]
    public GameObject enemyHitEffect;
    public TrailRenderer trail;

    private Vector3 lastPosition;
    private bool initialized = false;

    void Start()
    {
        // 1. Setup visuals
        if (trail == null) trail = GetComponent<TrailRenderer>();
        if (trail != null)
        {
            trail.Clear();
            trail.emitting = true;
        }

        // 2. Initialize state
        lastPosition = transform.position;
        initialized = true;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (!initialized) return;

        // Calculate distance to move this frame
        float moveDistance = speed * Time.deltaTime;
        
        // Raycast ahead to detect hits (Prevents tunneling / missing targets at high speed)
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, moveDistance))
        {
            HandleHit(hit);
        }

        // Move the bullet
        transform.position += transform.forward * moveDistance;
        lastPosition = transform.position;
    }

    void HandleHit(RaycastHit hit)
    {
        // Ignore Player
        if (hit.collider.CompareTag("Player")) return;

        // Visual Effect (Explosion or Impact)
        if (enemyHitEffect != null)
        {
            Instantiate(enemyHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }

        // EXPLOSION LOGIC
        if (explosionRadius > 0)
        {
            Collider[] colliders = Physics.OverlapSphere(hit.point, explosionRadius);
            foreach (Collider col in colliders)
            {
                Target target = col.GetComponent<Target>();
                if (target != null)
                {
                    target.TakeDamage(damage); // Could apply falloff based on distance if desired
                }
            }
        }
        else 
        {
            // SINGLE TARGET logic
            Target enemy = hit.collider.GetComponent<Target>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        // Stop trail properly
        if (trail != null)
        {
            trail.transform.parent = null; // Detach trail so it fades out naturally
            trail.autodestruct = true;
        }

        Destroy(gameObject);
    }
}