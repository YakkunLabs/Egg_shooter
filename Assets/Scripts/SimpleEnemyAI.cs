using UnityEngine;
using UnityEngine.AI; // REQUIRED for NavMesh

public class SimpleEnemyAI : MonoBehaviour
{
    public Transform player; 
    public float damageToPlayer = 20f;
    
    // Attack Settings
    public float attackRange = 2.0f; // Increased slightly for NavMesh
    public float attackCooldown = 1.0f; 
    private float nextAttackTime = 0f;

    // Reference to the Agent component
    private NavMeshAgent agent;

    void Start()
    {
        // Get the NavMeshAgent component we just added
        agent = GetComponent<NavMeshAgent>();

        // Find Player automatically if needed
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) 
            {
                player = playerObj.transform;
            }
        }
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // 1. Tell the Agent to go to the Player
            // The Agent handles all the physics and walking around walls automatically!
            agent.SetDestination(player.position);

            // 2. Attack Logic
            if (distanceToPlayer <= attackRange)
            {
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
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageToPlayer);
        }
    }
}