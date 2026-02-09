using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MatchResultsUI : MonoBehaviour
{
    public static MatchResultsUI Instance;

    [Header("UI References")]
    public GameObject panelObj;       // The "Game Over" background panel
    public Transform listContainer;   // The Vertical Layout Group to hold rows
    public GameObject rowPrefab;      // A prefab with 2 Text fields (Name, Score)

    void Awake()
    {
        Instance = this;
        Hide(); // Start hidden
    }

    public void ShowResults(List<string> results)
    {
        if (panelObj == null) return;

        // 1. Show Panel
        panelObj.SetActive(true);

        // 2. Clear old rows (if any)
        foreach (Transform child in listContainer)
        {
            Destroy(child.gameObject);
        }

        // 3. Create new rows
        foreach (string line in results)
        {
            if (rowPrefab != null)
            {
                GameObject row = Instantiate(rowPrefab, listContainer);
                TextMeshProUGUI text = row.GetComponentInChildren<TextMeshProUGUI>();
                
                // Format: "PlayerName ..... 150"
                if (text != null) text.text = line; 
            }
        }
    }

    public void Hide()
    {
        if (panelObj != null) panelObj.SetActive(false);
    }
}