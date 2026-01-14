using UnityEngine;
using System.Collections;

public class AmmoPickup : MonoBehaviour
{
    [Header("Settings")]
    public float rotateSpeed = 50f; 
    public float respawnTime = 10f; 

    // Arrays to store all the parts of the crate
    private Renderer[] allRenderers;
    private Collider[] allColliders;
    private Light[] allLights;

    void Start()
    {
        // 1. Find EVERY visual and physical part in this object AND its children
        allRenderers = GetComponentsInChildren<Renderer>();
        allColliders = GetComponentsInChildren<Collider>();
        allLights = GetComponentsInChildren<Light>();
    }

    void Update()
    {
        // Only spin if the first renderer is visible (a simple check to see if we are 'alive')
        if (allRenderers.Length > 0 && allRenderers[0].enabled)
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AdvancedGunSystem currentGun = other.GetComponentInChildren<AdvancedGunSystem>();

            if (currentGun != null)
            {
                currentGun.RefillAmmo();
                StartCoroutine(RespawnRoutine());
            }
        }
    }

    IEnumerator RespawnRoutine()
    {
        // --- HIDE EVERYTHING ---
        SetCrateState(false);

        // --- WAIT ---
        yield return new WaitForSeconds(respawnTime);

        // --- SHOW EVERYTHING ---
        SetCrateState(true);
    }

    // A helper function to turn everything On or Off at once
    void SetCrateState(bool isActive)
    {
        // 1. Toggle Visuals (Mesh + Particles)
        foreach (Renderer r in allRenderers)
        {
            r.enabled = isActive;
        }

        // 2. Toggle Colliders (Triggers)
        foreach (Collider c in allColliders)
        {
            c.enabled = isActive;
        }

        // 3. Toggle Lights (Glow)
        foreach (Light l in allLights)
        {
            l.enabled = isActive;
        }
    }
}