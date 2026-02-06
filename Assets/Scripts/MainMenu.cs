using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenu : MonoBehaviour
{
    [Header("Weapon Selection")]
    public GameObject[] menuGuns;

    [Header("Skin Selection")]
    // Drag the MeshRenderer of your visible menu player here
    public MeshRenderer menuPlayerMesh; 
    // Drag all your skin Materials here (Element 0 = Default)
    public Material[] allSkins;

    private int selectedSkinIndex = 0;
    private int selectedWeaponIndex = 0;

    void Start()
    {
        // Unlock the cursor so the player can click buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Load saved skin (Default to 0 if none saved)
        selectedWeaponIndex = PlayerPrefs.GetInt("SelectedWeapon", 0);
        selectedSkinIndex = PlayerPrefs.GetInt("SelectedSkin", 0);

        UpdateWeaponVisuals();
        UpdateSkinVisuals();
    }
    
    public void PlayGame()
    {
        // Loads the next scene (The World)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT GAME!");
        Application.Quit();
    }

    // Connect this to your SKIN Buttons (0, 1, 2, etc.)
    public void SelectSkin(int index)
    {
        selectedSkinIndex = index;

        // 1. Change the material immediately in the menu
        UpdateSkinVisuals();

        // 2. Save the choice for the Game Scene (NetClient will read this)
        PlayerPrefs.SetInt("SelectedSkin", selectedSkinIndex);
        PlayerPrefs.Save();
    }

    public void SelectWeapon(int index)
    {
        selectedWeaponIndex = index;
        PlayerPrefs.SetInt("SelectedWeapon", selectedWeaponIndex);
        PlayerPrefs.Save();
        UpdateWeaponVisuals();
    }

    void UpdateSkinVisuals()
    {
        if (menuPlayerMesh != null && allSkins != null && selectedSkinIndex < allSkins.Length)
        {
             menuPlayerMesh.material = allSkins[selectedSkinIndex];
        }
    }
    void UpdateWeaponVisuals()
    {
        if (menuGuns == null) return;
        for (int i = 0; i < menuGuns.Length; i++)
        {
            if (menuGuns[i] != null)
                menuGuns[i].SetActive(i == selectedWeaponIndex);
        }
    }
}