using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Stats")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Regeneration Settings (NEW)")]
    public bool canRegenerate = true;     // Turn this off if you want "Hard Mode"
    public float regenAmount = 20f;       // How much health to restore per second
    public float timeBeforeRegen = 3.0f;  // Seconds to wait after getting hit
    private float lastDamageTime;         // Tracks when we last got hit

    [Header("Score Stats")]
    public int kills = 0; 

    [Header("UI Reference")]
    public Slider healthSlider; 
    public GameObject gameOverScreen;
    public TextMeshProUGUI text_score_hud; 
    public TextMeshProUGUI text_score_final; 

    [Header("Audio")]
    public AudioSource audioSource; 
    public AudioClip hurtSound;     

    void Start()
    {
        currentHealth = maxHealth;
        
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        UpdateHealthUI();
        UpdateScoreUI(); 
        
        Time.timeScale = 1f; 
    }

    void Update()
    {
        // --- NEW REGENERATION LOGIC ---
        // 1. Are we hurt?
        if (currentHealth < maxHealth && currentHealth > 0 && canRegenerate)
        {
            // 2. Has enough time passed since the last hit?
            if (Time.time > lastDamageTime + timeBeforeRegen)
            {
                // 3. Add health smoothly over time
                currentHealth += regenAmount * Time.deltaTime;

                // 4. Don't go over the max
                if (currentHealth > maxHealth)
                {
                    currentHealth = maxHealth;
                }

                // 5. Update the green bar
                UpdateHealthUI();
            }
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        
        // --- IMPORTANT: Reset the Regeneration Timer ---
        lastDamageTime = Time.time; 

        if (audioSource != null && hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void AddKill()
    {
        kills++;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
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