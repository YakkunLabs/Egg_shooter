using UnityEngine;

public class SimpleEnemyAI : MonoBehaviour
{
    public Transform player; // Drag your Player object here
    public float speed = 3f;
    public float damageToPlayer = 20f;

    void Update()
    {
        if (player != null)
        {
            // 1. Look at the player
            // We lock the y-axis so the egg doesn't tilt up/down weirdly
            Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.LookAt(targetPosition);

            // 2. Move towards the player
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
    }

    // 3. Attack when touching the player
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            // Find the health script on the player and hurt them
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageToPlayer);
                
                // Knock the enemy back slightly so they don't instant-kill you
                // (Simple bounce back effect)
                transform.position -= transform.forward * 1.0f;
            }
        }
    }
}