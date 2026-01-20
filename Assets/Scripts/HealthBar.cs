using UnityEngine;
using UnityEngine.UI; 

public class HealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Slider slider;
    public Image fillImage; 

    [Header("Settings")]
    public Gradient colorGradient; // The magic color changer

    [Header("Billboarding (Face Camera)")]
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void SetMaxHealth(float maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
        
        // Set color to the "100%" mark of the gradient (Green)
        fillImage.color = colorGradient.Evaluate(1f);
    }

    public void SetHealth(float currentHealth)
    {
        slider.value = currentHealth;

        // Calculate percentage (0 to 1)
        float percentage = slider.value / slider.maxValue;

        // Pick the color from the gradient based on percentage
        fillImage.color = colorGradient.Evaluate(percentage);
    }

    void LateUpdate()
    {
        // Make the bar face the camera so text isn't backwards
        transform.LookAt(transform.position + mainCamera.transform.forward);
    }
}