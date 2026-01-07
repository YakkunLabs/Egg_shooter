using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyAI : MonoBehaviour
{
    public Transform player;
    public GameObject bulletPrefab; 
    public Transform firePoint;     

    [Header("Stats")]
    public float shootingRange = 10f; 
    public float fireRate = 1.5f;     
    private float nextFireTime = 0f;

    [Header("Audio")]
    public AudioSource audioSource; 
    public AudioClip shootSound;    

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
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
            // Stop moving
            agent.isStopped = true;
            
            // BODY ROTATION: Keep looking horizontally (so the egg doesn't tilt)
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
        // 1. Play Sound
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        // --- NEW FIX: AIM AT THE PLAYER'S CENTER ---
        // Instead of using firePoint.rotation (which is flat), 
        // we calculate the precise direction to the player's chest.
        
        Vector3 aimDirection = (player.position - firePoint.position).normalized;
        
        // Create a rotation that looks exactly at the player
        Quaternion bulletRotation = Quaternion.LookRotation(aimDirection);

        // Spawn the bullet using this NEW rotation
        Instantiate(bulletPrefab, firePoint.position, bulletRotation);
    }
}