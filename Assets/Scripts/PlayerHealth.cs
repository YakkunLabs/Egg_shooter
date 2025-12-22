using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Stats")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI Reference")]
    public Slider healthSlider; 
    public GameObject gameOverScreen; // The black panel we just made

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        
        // Ensure game is running and cursor is locked when we start
        Time.timeScale = 1f; 
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        // 1. Show the Game Over Screen
        gameOverScreen.SetActive(true);

        // 2. Unlock the Mouse so user can click buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 3. Pause the Game (Stop enemies moving)
        Time.timeScale = 0f;
    }

    // We will link this function to the Button
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}