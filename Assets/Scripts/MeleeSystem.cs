using UnityEngine;

public class MeleeSystem : MonoBehaviour
{
    [Header("Stats")]
    public int damage = 50;         // High damage!
    public float range = 2.5f;      // How far you can hit
    public float attackRate = 1f;   // Time between swings
    private float nextAttackTime = 0f;

    [Header("References")]
    public Transform attackPoint;   // The center of the screen (Camera)
    public LayerMask enemyLayer;    // What can we hit?
    public Animator animator;       // For the swing animation

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip swingSound;    // "Whoosh"
    public AudioClip hitSound;      // "BONK!"

    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Attack Input
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Swing();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void Swing()
    {
        // 1. Play Animation
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // 2. Play "Whoosh" Sound
        if (audioSource != null && swingSound != null)
        {
            audioSource.PlayOneShot(swingSound);
        }

        // 3. Detect Hit (Delayed slightly to match animation is better, but this works for now)
        Invoke("CheckHit", 0.4f); 
    }

    void CheckHit()
    {
        RaycastHit hit;
        if (Physics.Raycast(attackPoint.position, attackPoint.forward, out hit, range, enemyLayer))
        {
            Debug.Log("Bonk! Hit: " + hit.transform.name);

            // Play Sound
            if (audioSource != null && hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }

            // --- DAMAGE LOGIC (UNCOMMENTED) ---
            
            // 1. Try to find the 'Target' script on the object we hit
            Target enemy = hit.transform.GetComponent<Target>();
            
            // 2. If we found it, deal damage!
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            
            // ----------------------------------

            // Physics Push (Optional)
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * 500f);
            }
        }
    }
}