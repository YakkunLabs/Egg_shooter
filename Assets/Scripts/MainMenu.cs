using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenu : MonoBehaviour
{
    [Header("Skin Selection")]
    // Drag the MeshRenderer of your visible menu player here
    public MeshRenderer menuPlayerMesh; 
    // Drag all your skin Materials here (Element 0 = Default)
    public Material[] allSkins;

    private int selectedSkinIndex = 0;

    void Start()
    {
        // Unlock the cursor so the player can click buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Load saved skin (Default to 0 if none saved)
        selectedSkinIndex = PlayerPrefs.GetInt("SelectedSkin", 0);
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

    void UpdateSkinVisuals()
    {
        if (menuPlayerMesh != null && allSkins != null && selectedSkinIndex < allSkins.Length)
        {
             menuPlayerMesh.material = allSkins[selectedSkinIndex];
        }
    }
}