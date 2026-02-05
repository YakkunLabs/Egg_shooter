using UnityEngine;

public class NetworkPlayerSetup : MonoBehaviour
{
    [Header("Visual Parts")]
    public Renderer bodyRenderer; // The mesh to change color/texture
    public Transform gunHolder;   // Parent object where guns are stored

    [Header("Configuration Libraries")]
    public Material[] availableSkins; 
    public GameObject[] availableGuns; 

    // ------------------------------------------------------------------------
    // METHOD 1: The one NetClient looks for first (Priority 1)
    // ------------------------------------------------------------------------
    public void UpdateVisuals(int weaponIndex, int ammo, int reserve, bool isReloading, int skin)
    {
        ApplyGun(weaponIndex);
        ApplySkin(skin);
    }

    // ------------------------------------------------------------------------
    // METHOD 2: Fallback
    // ------------------------------------------------------------------------
    public void UpdateVisuals(int weaponIndex, int ammo, int skin)
    {
        ApplyGun(weaponIndex);
        ApplySkin(skin);
    }

    // ------------------------------------------------------------------------
    // HELPER FUNCTIONS
    // ------------------------------------------------------------------------

    private void ApplySkin(int skinIndex)
    {
        if (availableSkins == null || availableSkins.Length == 0) return;

        if (skinIndex >= 0 && skinIndex < availableSkins.Length)
        {
            if (bodyRenderer != null)
                bodyRenderer.material = availableSkins[skinIndex];
        }
    }

    private void ApplyGun(int gunIndex)
    {
        if (availableGuns == null) return;

        // --- THE CRITICAL FIX ---
        // Check if the requested gun is ALREADY active.
        // If it is, we MUST return immediately.
        // If we continue, we would disable the object and kill the Reload Coroutine.
        if (gunIndex >= 0 && gunIndex < availableGuns.Length)
        {
            GameObject targetGun = availableGuns[gunIndex];
            if (targetGun != null && targetGun.activeSelf)
            {
                return; // 🛑 STOP! The gun is already correct. Don't reset it.
            }
        }
        // -------------------------

        // 1. Hide all guns first
        foreach (GameObject gun in availableGuns)
        {
            if (gun != null) gun.SetActive(false);
        }

        // 2. Show the requested one
        if (gunIndex >= 0 && gunIndex < availableGuns.Length)
        {
            if (availableGuns[gunIndex] != null)
                availableGuns[gunIndex].SetActive(true);
        }
        else
        {
            // Debug.LogWarning($"Gun Index {gunIndex} is out of range");
        }
    }
}