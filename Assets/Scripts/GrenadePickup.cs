using UnityEngine;
using System.Collections;

public class GrenadePickup : MonoBehaviour
{
    [Header("Settings")]
    public float rotateSpeed = 50f; 
    public float respawnTime = 15f; // Time before it comes back

    // Arrays to store visuals
    private Renderer[] allRenderers;
    private Collider[] allColliders;
    private Light[] allLights;

    void Start()
    {
        // Find all visual parts (Child objects, lights, etc.)
        allRenderers = GetComponentsInChildren<Renderer>();
        allColliders = GetComponentsInChildren<Collider>();
        allLights = GetComponentsInChildren<Light>();
    }

    void Update()
    {
        // Spin if visible
        if (allRenderers.Length > 0 && allRenderers[0].enabled)
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Look for the GrenadeThrower script on the Player (or their children)
            GrenadeThrower thrower = other.GetComponentInChildren<GrenadeThrower>();

            if (thrower != null)
            {
                // 2. Refill!
                thrower.RefillGrenades();

                // 3. Hide and Wait
                StartCoroutine(RespawnRoutine());
            }
        }
    }

    IEnumerator RespawnRoutine()
    {
        SetCrateState(false); // Hide
        yield return new WaitForSeconds(respawnTime); // Wait
        SetCrateState(true);  // Show
    }

    void SetCrateState(bool isActive)
    {
        foreach (Renderer r in allRenderers) r.enabled = isActive;
        foreach (Collider c in allColliders) c.enabled = isActive;
        foreach (Light l in allLights) l.enabled = isActive;
    }
}