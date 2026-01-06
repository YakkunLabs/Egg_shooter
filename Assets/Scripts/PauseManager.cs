using UnityEngine;
using UnityEngine.SceneManagement; // Needed to load Main Menu

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public static bool isPaused = false; // Static so other scripts can check it if needed

    void Update()
    {
        // Listen for the ESC key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        // 1. Hide the Menu
        pauseMenuUI.SetActive(false);
        
        // 2. Unfreeze Time
        Time.timeScale = 1f;
        
        // 3. Update Status
        isPaused = false;

        // 4. Lock Cursor again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        // 1. Show the Menu
        pauseMenuUI.SetActive(true);
        
        // 2. Freeze Time (Pauses physics and movement)
        Time.timeScale = 0f;
        
        // 3. Update Status
        isPaused = true;

        // 4. Unlock Cursor so we can click buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadMainMenu()
    {
        // IMPORTANT: Unfreeze time before leaving, or the Menu will be stuck frozen!
        Time.timeScale = 1f; 
        isPaused = false;
        
        // Load the menu scene (Make sure it is Index 0 in Build Settings)
        SceneManager.LoadScene(0); 
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}