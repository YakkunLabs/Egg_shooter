using UnityEngine;

public class NetworkPlayerSetup : MonoBehaviour
{
    [Header("Visual Parts")]
    public Renderer bodyRenderer; // The mesh to change color/texture
    public Transform gunHolder;   // Parent object where guns are stored

    [Header("Configuration Libraries")]
    // The server sends an INT (0, 1, 2...). We use that to pick from these lists.
    public Material[] availableSkins; 
    public GameObject[] availableGuns; 

    // --- CALLED BY NETWORK MANAGER ---
    public void UpdateVisuals(int skinIndex, int gunIndex)
    {
        // 1. Apply Skin
        if (skinIndex >= 0 && skinIndex < availableSkins.Length)
        {
            if (bodyRenderer != null)
            {
                bodyRenderer.material = availableSkins[skinIndex];
            }
        }
        else
        {
            Debug.LogError($"Skin Index {skinIndex} is invalid!");
        }

        // 2. Apply Gun
        // First, hide all guns
        foreach (GameObject gun in availableGuns)
        {
            if (gun != null) gun.SetActive(false);
        }

        // Then, show the correct one
        if (gunIndex >= 0 && gunIndex < availableGuns.Length)
        {
            if (availableGuns[gunIndex] != null)
            {
                availableGuns[gunIndex].SetActive(true);
            }
        }
        else
        {
            Debug.LogError($"Gun Index {gunIndex} is invalid!");
        }
    }
}