using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI; // Required for UI Buttons

public class MainMenu : MonoBehaviour
{
    [Header("Secondary Weapon Selection")]
    // Drag your weapon models here (e.g. 0=Pistol, 1=Rifle, 2=Shotgun)
    public GameObject[] secondaryWeaponModels; 
    public Button[] weaponButtons; // <--- NEW: Drag your UI Buttons here (Order must match models!)

    [Header("Skin Selection")]
    public MeshRenderer menuPlayerMesh; 
    public Material[] allSkins;
    public Button[] skinButtons;   // <--- NEW: Drag your Skin Buttons here

    [Header("Highlight Settings")]
    public Color selectedColor = Color.green; 
    public Color normalColor = Color.white;

    // We store the "Secondary Weapon" choice here
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
        
        // 3. APPLY HIGHLIGHTS INITIALY
        UpdateHighlights();
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
        UpdateHighlights(); // <--- NEW: Updates colors when clicked
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
        UpdateHighlights(); // <--- NEW: Updates colors when clicked
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

    // --- NEW: HIGHLIGHT LOGIC ---
    void UpdateHighlights()
    {
        // 1. Highlight Weapon Buttons
        if (weaponButtons != null)
        {
            for (int i = 0; i < weaponButtons.Length; i++)
            {
                if (weaponButtons[i] == null) continue;
                
                Image btnImg = weaponButtons[i].GetComponent<Image>();
                if (btnImg != null)
                {
                    // If index matches selection, use Green. Else White.
                    btnImg.color = (i == selectedSecondaryWeaponIndex) ? selectedColor : normalColor;
                }
            }
        }

        // 2. Highlight Skin Buttons
        if (skinButtons != null)
        {
            for (int i = 0; i < skinButtons.Length; i++)
            {
                if (skinButtons[i] == null) continue;

                Image btnImg = skinButtons[i].GetComponent<Image>();
                if (btnImg != null)
                {
                    btnImg.color = (i == selectedSkinIndex) ? selectedColor : normalColor;
                }
            }
        }
    }
}