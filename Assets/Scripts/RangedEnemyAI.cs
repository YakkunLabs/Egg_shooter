using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyAI : MonoBehaviour
{
    public Transform player;
    public GameObject bulletPrefab; // The yellow sphere we just made
    public Transform firePoint;     // Where the bullet comes out

    [Header("Stats")]
    public float shootingRange = 10f; // Stops 10 meters away
    public float fireRate = 1.5f;     // Shoots every 1.5 seconds
    private float nextFireTime = 0f;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // 1. Movement Logic
        if (distance > shootingRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            // Stop moving if close enough
            agent.isStopped = true;
            
            // Look at the player
            Vector3 targetPostition = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.LookAt(targetPostition);

            // 2. Shooting Logic
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void Shoot()
    {
        // Spawn the bullet at the firePoint position and rotation
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}