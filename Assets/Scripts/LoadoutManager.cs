using UnityEngine;

public class LoadoutManager : MonoBehaviour
{
    [Header("Drag your guns here in order")]
    // Element 0 = Pistol, Element 1 = Rifle
    public GameObject[] weapons; 

    void Start()
    {
        // 1. Read the saved ID (Default to 0 if nothing saved)
        int selectedID = PlayerPrefs.GetInt("SelectedWeapon", 0);

        // 2. Loop through all weapons
        for (int i = 0; i < weapons.Length; i++)
        {
            if (i == selectedID)
            {
                // Activate the chosen one
                weapons[i].SetActive(true);
            }
            else
            {
                // Hide the others
                weapons[i].SetActive(false);
            }
        }
    }
}