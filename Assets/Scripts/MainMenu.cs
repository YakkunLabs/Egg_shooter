using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading levels

public class MainMenu : MonoBehaviour
{
    [Header("The Guns in the Menu Player's Hands")]
    // Drag the gun OBJECTS from the Scene (inside the Menu Player)
    public GameObject[] menuGuns; 

    private int selectedWeaponIndex = 0;

    void Start()
    {
        // Unlock the cursor so the player can click buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 1. Load the last saved choice (Optional, remembers what you picked last time)
        selectedWeaponIndex = PlayerPrefs.GetInt("SelectedWeapon", 0);
        UpdateMenuVisuals();
    }
    
    public void PlayGame()
    {
        // Loads the next scene in the queue (The Game)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT GAME!"); // Shows in Unity Editor to prove it works
        Application.Quit();      // Closes the window in the real game
    }

    // Call this function from your Buttons
    // 0 = Pistol, 1 = Rifle (or whatever order you like)
// Connect this to your WEAPON Buttons
    public void SelectWeapon(int index)
    {
        selectedWeaponIndex = index;
        
        // Save choice for the Game Scene
        PlayerPrefs.SetInt("SelectedWeapon", selectedWeaponIndex);
        PlayerPrefs.Save();

        // Update the visual character immediately
        UpdateMenuVisuals();
    }

    void UpdateMenuVisuals()
    {
        // Turn off all guns, then turn on only the selected one
        for (int i = 0; i < menuGuns.Length; i++)
        {
            if (i == selectedWeaponIndex)
            {
                menuGuns[i].SetActive(true);
            }
            else
            {
                menuGuns[i].SetActive(false);
            }
        }
    }
}