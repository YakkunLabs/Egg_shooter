using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI; 

public class MainMenu : MonoBehaviour
{
    [Header("Secondary Weapon Selection")]
    public GameObject[] secondaryWeaponModels; 
    
    // ✅ CHANGE: Use ShinyButton instead of Button
    public ShinyButton[] weaponButtons; 

    [Header("Skin Selection")]
    public MeshRenderer menuPlayerMesh; 
    public Material[] allSkins;
    
    // ✅ CHANGE: Use ShinyButton instead of Button
    public ShinyButton[] skinButtons; 

    // We store the choice here
    private int selectedSecondaryWeaponIndex = 0;
    private int selectedSkinIndex = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 1. LOAD SAVED DATA
        selectedSecondaryWeaponIndex = PlayerPrefs.GetInt("SelectedWeapon", 1); 
        selectedSkinIndex = PlayerPrefs.GetInt("SelectedSkin", 0);

        // 2. APPLY VISUALS
        UpdateWeaponVisuals();
        UpdateSkinVisuals();
        
        // 3. APPLY SHINY HIGHLIGHTS
        UpdateShinySelection();
    }
    
    public void PlayGame()
    {
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
    public void SelectSecondaryWeapon(int index)
    {
        selectedSecondaryWeaponIndex = index;
        PlayerPrefs.SetInt("SelectedWeapon", selectedSecondaryWeaponIndex);
        PlayerPrefs.Save();
        
        UpdateWeaponVisuals();
        UpdateShinySelection(); // ✅ Update Shine
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

        // Show the selected one
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
        UpdateShinySelection(); // ✅ Update Shine
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

    // --- NEW: SHINY OUTLINE LOGIC ---
    void UpdateShinySelection()
    {
        // 1. Highlight Weapon Buttons
        if (weaponButtons != null)
        {
            for (int i = 0; i < weaponButtons.Length; i++)
            {
                if (weaponButtons[i] == null) continue;
                
                // If 'i' matches the selected index, turn ON shine. Otherwise OFF.
                weaponButtons[i].SetSelected(i == selectedSecondaryWeaponIndex);
            }
        }

        // 2. Highlight Skin Buttons
        if (skinButtons != null)
        {
            for (int i = 0; i < skinButtons.Length; i++)
            {
                if (skinButtons[i] == null) continue;

                // If 'i' matches the selected index, turn ON shine. Otherwise OFF.
                skinButtons[i].SetSelected(i == selectedSkinIndex);
            }
        }
    }
}