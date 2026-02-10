using UnityEngine;
using TMPro;

public class ItemSpawnVisuals : MonoBehaviour
{
    [Header("Weapon Models")]
    public GameObject[] weaponModels; 

    [Header("UI Prompt")]
    public GameObject pickupText; // Drag your "Press F" text object here

    private int currentItemType = 0;
    private Transform _localPlayer; // Cache the player for performance

    void Start()
    {
        HideAll();
        if (pickupText != null) pickupText.SetActive(false);
    }

    public void SetItem(int itemType)
    {
        currentItemType = itemType;
        HideAll();

        // Only show model if itemType is valid
        if (itemType > 0 && itemType < weaponModels.Length)
        {
            if (weaponModels[itemType] != null)
            {
                weaponModels[itemType].SetActive(true);
            }
        }
        // If itemType is 0 (None), we hide everything, including text
        else 
        {
            if (pickupText != null) pickupText.SetActive(false);
        }
    }

    void HideAll()
    {
        foreach (var model in weaponModels)
        {
            if (model != null) model.SetActive(false);
        }
    }

    void Update()
    {
        // 1. If there is no item here, never show text
        if (currentItemType == 0 || pickupText == null) 
        {
            if (pickupText != null) pickupText.SetActive(false);
            return;
        }

        // 2. Find the Local Player (Only once!)
        if (_localPlayer == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) _localPlayer = p.transform;
        }

        // 3. Calculate Distance
        if (_localPlayer != null)
        {
            float dist = Vector3.Distance(transform.position, _localPlayer.position);
            
            // Show if closer than 3 meters
            bool isClose = dist < 3.0f; 
            pickupText.SetActive(isClose);

            // 4. (Optional) Make text always face the camera
            if (isClose && Camera.main != null)
            {
                pickupText.transform.LookAt(Camera.main.transform);
                pickupText.transform.Rotate(0, 180, 0); // Flip so it's not backwards
            }
        }
    }
}