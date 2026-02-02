using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Stats")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Regeneration Settings")]
    public bool canRegenerate = true;     
    public float regenAmount = 20f;       
    public float timeBeforeRegen = 3.0f;  
    private float lastDamageTime;         

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
        
        // --- NEW: AUTO-FIND UI ---
        // Since this player is spawned by NetClient, we must find the UI automatically.
        if (healthSlider == null)
        {
            GameObject sliderObj = GameObject.Find("HealthSlider"); // MAKE SURE YOUR SLIDER IS NAMED THIS!
            if (sliderObj != null) healthSlider = sliderObj.GetComponent<Slider>();
        }

        if (gameOverScreen == null)
        {
            GameObject goScreen = GameObject.Find("GameOverScreen");
            if (goScreen != null) gameOverScreen = goScreen;
        }
        // -------------------------

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        UpdateHealthUI();
        UpdateScoreUI(); 
        
        Time.timeScale = 1f; 
    }

    void Update()
    {
        // Only regenerate if we are NOT dead
        if (currentHealth <= 0) return;

        // --- REGENERATION LOGIC ---
        if (currentHealth < maxHealth && canRegenerate)
        {
            if (Time.time > lastDamageTime + timeBeforeRegen)
            {
                currentHealth += regenAmount * Time.deltaTime;

                if (currentHealth > maxHealth)
                    currentHealth = maxHealth;

                UpdateHealthUI();
            }
        }
    }

    // Called by LOCAL logic (falling, traps, etc)
    public void TakeDamage(float amount)
    {
        // For networked games, usually we wait for server updates, 
        // but for instant feedback/prediction we can do this:
        currentHealth -= amount;
        lastDamageTime = Time.time; 

        if (audioSource != null && hurtSound != null)
            audioSource.PlayOneShot(hurtSound);

        UpdateHealthUI();

        if (currentHealth <= 0) Die();
    }

    // --- CALLED BY NETCLIENT (The Server is Boss) ---
    public void UpdateHealthFromServer(int serverHealth)
    {
        // 1. Check if we took damage (Server HP is lower than what we thought)
        if (serverHealth < currentHealth)
        {
            lastDamageTime = Time.time; // Reset regen timer
            
            // Play sound if significant damage
            if (audioSource != null && hurtSound != null)
                audioSource.PlayOneShot(hurtSound);
        }

        // 2. Force health to match server
        currentHealth = serverHealth; 

        // 3. Update UI correctly (0.0 to 1.0)
        UpdateHealthUI();

        // 4. Check for Death
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
            // Fix: Slider value must be between 0 and 1
            healthSlider.value = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        if (gameOverScreen != null) gameOverScreen.SetActive(true);
        
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