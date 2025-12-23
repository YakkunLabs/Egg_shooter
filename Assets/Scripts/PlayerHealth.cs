using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 
using TMPro; // NEW: Needed for TextMeshPro

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Stats")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Score Stats")]
    public int kills = 0; // NEW: Tracks kills

    [Header("UI Reference")]
    public Slider healthSlider; 
    public GameObject gameOverScreen;
    public TextMeshProUGUI text_score_hud; // NEW: The text on screen
    public TextMeshProUGUI text_score_final; // NEW: The text on Game Over

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        UpdateScoreUI(); // Initialize score text
        
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

    // NEW: Function to add points
    public void AddKill()
    {
        kills++;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        // Update the top corner text
        if (text_score_hud != null)
            text_score_hud.text = "KILLS: " + kills;
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
        gameOverScreen.SetActive(true);
        
        // NEW: Update the Game Over text with final score
        if (text_score_final != null)
            text_score_final.text = "TOTAL KILLS: " + kills;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}