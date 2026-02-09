using UnityEngine;
using TMPro;
using System.Collections;

public class JoinNotification : MonoBehaviour
{
    public static JoinNotification Instance;

    [Header("UI References")]
    public GameObject panelObj;      // The actual visual panel (drag JoinPanel here)
    public TextMeshProUGUI msgText;  // The text inside it

    [Header("Settings")]
    public float displayDuration = 5f;

    private Coroutine currentRoutine;

    void Awake()
    {
        // 1. Singleton Setup
        if (Instance == null) 
        {
            Instance = this;
        }
        else 
        {
            Destroy(gameObject);
            return;
        }

        // 2. Ensure visuals start hidden
        if (panelObj != null) 
        {
            panelObj.SetActive(false);
        }
        
        Debug.Log("✅ JoinNotification Initialized!");
    }

    public void ShowMessage(string playerName)
    {
        Debug.Log($"[UI] Attempting to show join message for: {playerName}");

        if (panelObj == null || msgText == null) 
        {
            Debug.LogError("❌ JoinNotification: Panel or Text is missing!");
            return;
        }

        // 3. Update Text & Show
        msgText.text = $"<color=green>{playerName}</color> joined the game!";
        
        // Restart the timer if already showing
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(ShowAndHide());
    }

    IEnumerator ShowAndHide()
    {
        panelObj.SetActive(true);
        yield return new WaitForSeconds(displayDuration);
        panelObj.SetActive(false);
        currentRoutine = null;
    }
}