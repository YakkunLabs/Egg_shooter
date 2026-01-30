using UnityEngine;

public class BulletImpact : MonoBehaviour
{
    public GameObject impactVFX; // Drag a "Spark" or "Dust" particle here

    void OnTriggerEnter(Collider other)
    {
        // Ignore players (for now, to avoid blood effects on laggy players)
        if (other.CompareTag("Player")) return;

        // Hit a Wall/Ground
        if (impactVFX != null)
        {
            Instantiate(impactVFX, transform.position, Quaternion.identity);
        }

        // Destroy bullet immediately on hit
        Destroy(gameObject);
    }
}