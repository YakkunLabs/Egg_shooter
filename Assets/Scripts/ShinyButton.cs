using UnityEngine;
using UnityEngine.UI;

public class ShinyButton : MonoBehaviour
{
    [Header("Settings")]
    public Color normalColor = Color.white;
    public Color shineColor = Color.cyan; // Default to Cyan
    public float shineSpeed = 3f;
    public Vector2 outlineWidth = new Vector2(4f, 4f);

    [Header("State")]
    public bool isSelected = false;

    private Outline _outline;
    private Image _image;

    void Awake()
    {
        _image = GetComponent<Image>();
        
        // Auto-add Outline component if missing
        _outline = GetComponent<Outline>();
        if (_outline == null) _outline = gameObject.AddComponent<Outline>();

        _outline.enabled = false;
        _outline.effectDistance = outlineWidth;
    }

    void Update()
    {
        // If selected, pulse the outline alpha
        if (isSelected && _outline != null)
        {
            float shine = Mathf.PingPong(Time.time * shineSpeed, 0.6f) + 0.4f; // 0.4 to 1.0
            
            Color c = shineColor;
            c.a = shine;
            _outline.effectColor = c;
        }
    }

    // Call this from MainMenu
    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (_outline != null) 
            _outline.enabled = selected;

        // Reset the image color to normal (removes the old green tint)
        if (_image != null) 
            _image.color = normalColor; 
    }
}