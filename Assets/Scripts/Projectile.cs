using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 50f;
    public float damage = 10f;
    public float lifetime = 3f; // Destroy after 3 seconds if it misses

    void Start()
    {
        // Destroy bullet automatically after X seconds to save memory
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move the bullet forward every frame
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // 1. Check if we hit an Enemy
        // (Make sure your Enemy has the script "Target" or "EnemyHealth")
        Target enemy = other.GetComponent<Target>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        // 2. Ignore Player (Don't shoot yourself)
        if (other.CompareTag("Player")) return;

        // 3. Destroy Bullet on impact (Visual effect optional)
        Destroy(gameObject); 
    }
}