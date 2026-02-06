using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI; // Required if you are using UI Buttons

public class MainMenu : MonoBehaviour
{
    [Header("Secondary Weapon Selection")]
    // Drag your weapon models here (e.g. 0=Pistol, 1=Rifle, 2=Shotgun)
    // Make sure the order matches your Server's WeaponType ID!
    public GameObject[] secondaryWeaponModels; 

    [Header("Skin Selection")]
    public MeshRenderer menuPlayerMesh; 
    public Material[] allSkins;

    // We store the "Secondary Weapon" choice here
    private int selectedSecondaryWeaponIndex = 0;
    private int selectedSkinIndex = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 1. LOAD SAVED DATA
        // Default to 1 (Rifle) or 0 (None) depending on your preference
        selectedSecondaryWeaponIndex = PlayerPrefs.GetInt("SelectedWeapon", 1); 
        selectedSkinIndex = PlayerPrefs.GetInt("SelectedSkin", 0);

        // 2. APPLY VISUALS
        UpdateWeaponVisuals();
        UpdateSkinVisuals();
    }
    
    public void PlayGame()
    {
        // Save everything one last time before leaving
        PlayerPrefs.SetInt("SelectedWeapon", selectedSecondaryWeaponIndex);
        PlayerPrefs.SetInt("SelectedSkin", selectedSkinIndex);
        PlayerPrefs.Save();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT GAME!");
        Application.Quit();
    }

    // --- SECONDARY WEAPON BUTTONS ---
    // Link your Buttons to this: SelectSecondaryWeapon(1), SelectSecondaryWeapon(2), etc.
    // NOTE: Check your Schema! If 1=Pistol, Button 1 should pass 1.
    public void SelectSecondaryWeapon(int index)
    {
        selectedSecondaryWeaponIndex = index;
        PlayerPrefs.SetInt("SelectedWeapon", selectedSecondaryWeaponIndex);
        PlayerPrefs.Save();
        UpdateWeaponVisuals();
    }

    void UpdateWeaponVisuals()
    {
        if (secondaryWeaponModels == null) return;

        // Hide all models first
        for (int i = 0; i < secondaryWeaponModels.Length; i++)
        {
            if (secondaryWeaponModels[i] != null)
                secondaryWeaponModels[i].SetActive(false);
        }

        // Show the selected one (if valid)
        // Note: Arrays are 0-indexed. If your ID '1' matches model at index '0', adjust here.
        // Assuming models are ordered exactly by ID:
        if (selectedSecondaryWeaponIndex >= 0 && selectedSecondaryWeaponIndex < secondaryWeaponModels.Length)
        {
            if (secondaryWeaponModels[selectedSecondaryWeaponIndex] != null)
                secondaryWeaponModels[selectedSecondaryWeaponIndex].SetActive(true);
        }
    }

    // --- SKIN BUTTONS ---
    public void SelectSkin(int index)
    {
        selectedSkinIndex = index;
        PlayerPrefs.SetInt("SelectedSkin", selectedSkinIndex);
        PlayerPrefs.Save();
        UpdateSkinVisuals();
    }

    void UpdateSkinVisuals()
    {
        if (menuPlayerMesh != null && allSkins != null)
        {
            if (selectedSkinIndex >= 0 && selectedSkinIndex < allSkins.Length)
            {
                menuPlayerMesh.material = allSkins[selectedSkinIndex];
            }
        }
    }
}