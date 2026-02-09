using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ServerMessagePopup : MonoBehaviour
{
    public static ServerMessagePopup Instance;

    [Header("UI References")]
    public GameObject panelObj;      // The background panel (Black/Dark)
    public TextMeshProUGUI msgText;  // The text (Red)

    void Awake()
    {
        Instance = this;
        Hide(); // Start hidden
    }

    public void ShowError(string message)
    {
        if (panelObj != null) 
        {
            panelObj.SetActive(true);
            
            if (msgText != null)
            {
                msgText.text = message;
                msgText.color = Color.red; // Force RED color
            }
        }
    }

    public void Hide()
    {
        if (panelObj != null) panelObj.SetActive(false);
    }
    
    // Optional: Call this from a button on the panel to close it
    public void OnCloseButton()
    {
        Hide();
    }
}