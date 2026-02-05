using UnityEngine;
using CapnpGen; // Needed for WeaponType enum

public class PauseMenuWeapons : MonoBehaviour
{
    private WeaponManager playerManager;
    private NetClient netClient; 

    public void ChangeToWeapon(int weaponIndex)
    {
        // 1. Find the local player manager 
        if (playerManager == null)
        {
            // CHANGE: Use FindObjectOfType (Old Unity) instead of FindFirstObjectOfType (New Unity)
            playerManager = FindFirstObjectByType<WeaponManager>();
        }

        // 2. Tell the LOCAL player to switch 
        if (playerManager != null)
        {
            playerManager.EquipWeapon(weaponIndex);
            Debug.Log("Weapon Switched Locally to: " + weaponIndex);
        }

        // 3. TELL THE SERVER 
        if (netClient == null) 
        {
            // CHANGE: Use FindObjectOfType here too
            netClient = FindFirstObjectByType<NetClient>();
        }

        if (netClient != null)
        {
            // Convert the int (1, 2, 3) to the Network Enum
            CapnpGen.WeaponType networkWeapon = (CapnpGen.WeaponType)weaponIndex;

            //netClient.SendSelectWeapon(networkWeapon);
            Debug.Log($"[Network] Sent weapon change to server: {networkWeapon}");
        }
    }
}