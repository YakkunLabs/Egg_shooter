using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon List")]
    // Element 0: Rifle, Element 1: Pistol, Element 2: Sniper
    public GameObject[] allWeapons; 

    void Start()
    {
        // 1. Load the saved weapon when the game starts
        int savedIndex = PlayerPrefs.GetInt("SelectedWeapon", 0);
        EquipWeapon(savedIndex);
    }

    public void EquipWeapon(int index)
    {
        // Safety Check
        if (index >= allWeapons.Length || index < 0) return;

        // 1. Turn off ALL weapons first
        foreach (GameObject weapon in allWeapons)
        {
            weapon.SetActive(false);
        }

        // 2. Turn on only the requested weapon
        allWeapons[index].SetActive(true);

        // 3. Save the choice (so it remembers if you restart)
        PlayerPrefs.SetInt("SelectedWeapon", index);
        PlayerPrefs.Save();
    }
}