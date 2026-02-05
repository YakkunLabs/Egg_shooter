using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode switchKey = KeyCode.Q; // Press Q to switch
    public int pistolId = 0;              // ID of the Pistol (Default)

    private NetClient netClient;
    private int primaryWeaponId;
    private bool isHoldingPrimary = true; // We start with Primary selected

    void Start()
    {
        // 1. Find the NetClient in the scene
        netClient = FindFirstObjectByType<NetClient>();

        // 2. Load the choice from Main Menu
        // (Default to 1 if nothing saved, assuming 0 is pistol)
        primaryWeaponId = PlayerPrefs.GetInt("SelectedWeapon", 1);
        
        // Prevent bug: If we chose Pistol in menu, we don't need to switch
        if (primaryWeaponId == pistolId) primaryWeaponId = pistolId;
    }

    void Update()
    {
        // Only run this for MY local player
        if (netClient == null || netClient.myPlayerId == 0) return;
        
        // Check if this script is on the local player object
        // (We compare the name or use a NetworkIdentity check if you have one)
        // For now, we assume this script is only enabled on the Local Player.
        
        if (Input.GetKeyDown(switchKey))
        {
            ToggleWeapon();
        }
    }

    void ToggleWeapon()
    {
        if (isHoldingPrimary)
        {
            // Switch to Pistol
            Debug.Log("[WeaponController] Switching to Secondary (Pistol)");
            // netClient.SendSelectWeapon(pistolId);
            isHoldingPrimary = false;
        }
        else
        {
            // Switch back to Primary (Rifle/etc)
            Debug.Log($"[WeaponController] Switching to Primary (ID: {primaryWeaponId})");
            // netClient.SendSelectWeapon(primaryWeaponId);
            isHoldingPrimary = true;
        }
    }
}