using UnityEngine;

public class NetworkPuppet : MonoBehaviour
{
    [Header("Settings")]
    public float smoothSpeed = 10f; // How fast we blend to the new position
    public Animator animator;       // Drag the Animator component here

    // Where the server says we should be
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    void Start()
    {
        // Start where we spawned
        targetPosition = transform.position;
        targetRotation = transform.rotation;

        if (animator == null) animator = GetComponent<Animator>();
    }

    // --- COMMANDS (The real server will call these later) ---

    public void Server_MoveTo(Vector3 newPos, Quaternion newRot)
    {
        targetPosition = newPos;
        targetRotation = newRot;
    }

    public void Server_Shoot()
    {
        // Trigger the shooting animation
        if (animator != null)
        {
            animator.SetTrigger("Attack"); // Ensure your Animator has an "Attack" trigger!
            // Optional: You could also play a sound or flash here
        }
    }

    // --- UPDATE LOOP (Smooths the movement) ---
    void Update()
    {
        // 1. Smoothly Slide to Target (Lerp)
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);

        // 2. Animate Walking
        // Calculate how fast we are actually moving
        float speed = Vector3.Distance(transform.position, targetPosition) / Time.deltaTime;
        
        if (animator != null)
        {
            // If we are moving more than 0.1 units per second, tell animator to walk
            // Assumes your Animator has a "Speed" float parameter
            animator.SetFloat("Speed", speed > 0.1f ? 1f : 0f);
        }
    }
}