using UnityEngine;
using System.Collections;

public class AmmoPickup : MonoBehaviour
{
    [Header("Settings")]
    public float rotateSpeed = 50f; 
    public float respawnTime = 10f; 
    public AudioClip pickupSound; // <-- NEW: Drag your sound here!

    private Renderer[] allRenderers;
    private Collider[] allColliders;
    private Light[] allLights;

    void Start()
    {
        allRenderers = GetComponentsInChildren<Renderer>();
        allColliders = GetComponentsInChildren<Collider>();
        allLights = GetComponentsInChildren<Light>();
    }

    void Update()
    {
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
                // 1. Play Sound (The "Cha-Ching!")
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }

                // 2. Refill & Respawn
                currentGun.RefillAmmo();
                StartCoroutine(RespawnRoutine());
            }
        }
    }

    IEnumerator RespawnRoutine()
    {
        SetCrateState(false);
        yield return new WaitForSeconds(respawnTime);
        SetCrateState(true);
    }

    void SetCrateState(bool isActive)
    {
        foreach (Renderer r in allRenderers) r.enabled = isActive;
        foreach (Collider c in allColliders) c.enabled = isActive;
        foreach (Light l in allLights) l.enabled = isActive;
    }
}