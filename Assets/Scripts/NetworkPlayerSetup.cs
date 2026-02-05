using UnityEngine;

public class NetworkPlayerSetup : MonoBehaviour
{
    [Header("Visual Parts")]
    public Renderer bodyRenderer; 
    public Transform gunHolder;   

    [Header("Configuration Libraries")]
    public Material[] availableSkins; 
    public GameObject[] availableGuns; 

    // ------------------------------------------------------------------------
    // METHOD 1: The Full Version (Used by NetClient)
    // ------------------------------------------------------------------------
    public void UpdateVisuals(int weaponIndex, int ammo, int reserve, bool isReloading, int skinIndex)
    {
        ApplySkin(skinIndex);
        ApplyGun(weaponIndex);
        
    }

    // ------------------------------------------------------------------------
    // METHOD 2: The Fallback (Used by simple calls)
    // ------------------------------------------------------------------------
    public void UpdateVisuals(int weaponIndex, int ammo)
    {
        ApplySkin(0);
        ApplyGun(weaponIndex);
        
    }

    // ------------------------------------------------------------------------
    // METHOD 3: THE COMPILER FIX (Used by EnemyTestSpawner)
    // This handles calls that send (Weapon, Ammo, Reserve) but no Skin/Reload
    // ------------------------------------------------------------------------
    public void UpdateVisuals(int weaponIndex, int ammo, int reserve)
    {
        ApplySkin(0); // Default to Skin 0
        ApplyGun(weaponIndex);
        
    }

    // ------------------------------------------------------------------------
    // HELPER FUNCTIONS
    // ------------------------------------------------------------------------

    private void ApplySkin(int skinIndex)
    {
        if (availableSkins == null || availableSkins.Length == 0) return;

        if (bodyRenderer != null && skinIndex >= 0 && skinIndex < availableSkins.Length)
                {
                    if (bodyRenderer.sharedMaterial != availableSkins[skinIndex])
                    {
                        bodyRenderer.material = availableSkins[skinIndex];
                    }
                }
    }

    private void ApplyGun(int gunIndex)
    {
        if (availableGuns == null) return;

        // 1. CRITICAL RELOAD FIX:
        // If the correct gun is already active, DO NOTHING.
        if (gunIndex >= 0 && gunIndex < availableGuns.Length)
        {
            GameObject targetGun = availableGuns[gunIndex];
            if (targetGun != null && targetGun.activeSelf)
            {
                return; // Stop here. Don't reset the gun.
            }
        }

        // 2. Hide all guns
        foreach (GameObject gun in availableGuns)
        {
            if (gun != null) gun.SetActive(false);
        }

        // 3. Show requested gun
        if (gunIndex >= 0 && gunIndex < availableGuns.Length)
        {
            if (availableGuns[gunIndex] != null)
                availableGuns[gunIndex].SetActive(true);
        }
    }
}