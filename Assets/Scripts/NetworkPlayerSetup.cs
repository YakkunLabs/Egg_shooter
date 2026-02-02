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
    // NetClient sends: (WeaponID, Ammo, Reserve, Reloading)
    // ------------------------------------------------------------------------
    public void UpdateVisuals(int weaponIndex, int ammo, int reserve, bool isReloading)
    {
        // 1. Map the First Argument (WeaponID) to our Gun Logic
        ApplyGun(weaponIndex);

        // 2. Server doesn't send SkinID yet, so default to 0
        ApplySkin(0);

        // 3. (Optional) You can use 'isReloading' here later for animations
        // if (isReloading) animator.SetTrigger("Reload");
    }

    // ------------------------------------------------------------------------
    // METHOD 2: The fallback if NetClient tries the 2-argument version
    // NetClient sends: (WeaponID, Ammo)
    // ------------------------------------------------------------------------
    public void UpdateVisuals(int weaponIndex, int ammo)
    {
        // NetClient passes WeaponID as the first number.
        // We use that for the Gun. We ignore Ammo for visuals.
        ApplyGun(weaponIndex);
        ApplySkin(0);
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

        // 1. Hide all guns first
        foreach (GameObject gun in availableGuns)
        {
            if (gun != null) gun.SetActive(false);
        }

        // 2. Show the requested one
        // IMPORTANT: Ensure your availableGuns array matches the Server IDs:
        // 0 = None (Hands)
        // 1 = Pistol
        // 2 = Rifle
        // etc.
        if (gunIndex >= 0 && gunIndex < availableGuns.Length)
        {
            if (availableGuns[gunIndex] != null)
                availableGuns[gunIndex].SetActive(true);
        }
        else
        {
            // Debug.LogWarning($"Gun Index {gunIndex} is out of range (Max: {availableGuns.Length-1})");
        }
    }
}