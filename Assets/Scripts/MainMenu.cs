using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading levels

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        // Unlock the cursor so the player can click buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
}