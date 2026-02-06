using UnityEngine;

public class BulletTracer : MonoBehaviour
{
    [Header("Visual Settings")]
    public float stretchFactor = 0.5f; // Higher = Longer streak
    public float minLength = 1.0f;     // Minimum length of the bullet

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (rb != null && rb.linearVelocity.sqrMagnitude > 1f)
        {
            // 1. Rotate to face velocity
            transform.forward = rb.linearVelocity.normalized;

            // 2. Stretch the Z-scale based on speed
            float speed = rb.linearVelocity.magnitude;
            float newZ = Mathf.Max(minLength, speed * stretchFactor * Time.deltaTime);

            // Keep X and Y scale normal (thickness)
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newZ);
        }
    }
}