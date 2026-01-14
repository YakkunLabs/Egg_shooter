using UnityEngine;
using TMPro;

public class GrenadeThrower : MonoBehaviour
{
    [Header("Settings")]
    public float throwForce = 20f;
    public GameObject grenadePrefab;
    public Transform throwPoint;

    [Header("Audio (NEW)")]
    public AudioSource audioSource; // Drag the Player's AudioSource here
    public AudioClip throwSound;    // Drag a "Whoosh" or "Pin Pull" sound here

    [Header("Stats")]
    public int maxGrenades = 3;
    public int currentGrenades;
    public TextMeshProUGUI grenadeText;

    void Start()
    {
        currentGrenades = maxGrenades;
        // Find audio source if not assigned
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (currentGrenades > 0)
            {
                ThrowGrenade();
            }
        }
    }

    void ThrowGrenade()
    {
        currentGrenades--;
        UpdateUI();

        // 1. Play Throw Sound
        if (audioSource != null && throwSound != null)
        {
            audioSource.PlayOneShot(throwSound);
        }

        // 2. Spawn and Throw
        GameObject grenade = Instantiate(grenadePrefab, throwPoint.position, throwPoint.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.AddForce(throwPoint.forward * throwForce + transform.up * 2f, ForceMode.VelocityChange);
    }

    void UpdateUI()
    {
        if(grenadeText != null)
            grenadeText.text = "G: " + currentGrenades;
    }

    // --- NEW REFILL FUNCTION ---
    public void RefillGrenades()
    {
        currentGrenades = maxGrenades; // Reset to 3 (or whatever max is)
        UpdateUI(); // Update the text on screen immediately
        Debug.Log("Grenades Refilled!");
    }
}