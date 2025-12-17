using UnityEngine;

public class SimpleEnemyAI : MonoBehaviour
{
    public Transform player; 
    public float speed = 3f;
    public float damageToPlayer = 20f;
    
    // Attack Settings
    public float attackRange = 1.5f; // How close to be to hit
    public float attackCooldown = 1.0f; // Time between hits
    private float nextAttackTime = 0f;

    void Update()
    {
        if (player != null)
        {
            // 1. Calculate Distance
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // 2. Look at the player
            Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.LookAt(targetPosition);

            // 3. Move OR Attack
            if (distanceToPlayer > attackRange)
            {
                // If too far, Move closer
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            }
            else
            {
                // If close enough, Attack!
                if (Time.time >= nextAttackTime)
                {
                    AttackPlayer();
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
        }
    }

    void AttackPlayer()
    {
        // Find the health script on the player and hurt them
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageToPlayer);
            Debug.Log("Enemy Attacked! Player Health should drop.");
        }
    }
}