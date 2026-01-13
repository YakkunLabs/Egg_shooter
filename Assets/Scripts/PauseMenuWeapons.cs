using UnityEngine;

public class PauseMenuWeapons : MonoBehaviour
{
    private WeaponManager playerManager;

    public void ChangeToWeapon(int weaponIndex)
    {
        // 1. Find the player automatically (if we haven't already)
        if (playerManager == null)
        {
            // Look for the object with the "WeaponManager" script
            playerManager = FindObjectOfType<WeaponManager>();
        }

        // 2. Tell the player to switch
        if (playerManager != null)
        {
            playerManager.EquipWeapon(weaponIndex);
            Debug.Log("Weapon Switched to: " + weaponIndex);
        }
    }
}