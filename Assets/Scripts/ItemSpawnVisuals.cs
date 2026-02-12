using UnityEngine;
using TMPro;

public class ItemSpawnVisuals : MonoBehaviour
{
    [Header("Weapon Models")]
    public GameObject[] weaponModels; 

    [Header("UI Prompt")]
    public GameObject pickupText; // Drag your "Press F" text object here

    [Header("Item Animation")] // ✅ NEW: Animation Settings
    public float rotationSpeed = 50f; // Speed of spinning
    public bool enableBobbing = true; // Should it float up and down?
    public float bobSpeed = 2f;       // How fast it bobs
    public float bobHeight = 0.15f;   // How high it bobs

    private int currentItemType = 0;
    private Transform _localPlayer; 
    private Vector3[] _initialLocalPositions; // To store original positions

    void Start()
    {
        // ✅ 1. Remember the starting positions of your models
        // This ensures the bobbing animation stays in the right place relative to the Spawn Point.
        _initialLocalPositions = new Vector3[weaponModels.Length];
        for (int i = 0; i < weaponModels.Length; i++)
        {
            if (weaponModels[i] != null) 
                _initialLocalPositions[i] = weaponModels[i].transform.localPosition;
        }

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
        // ✅ 2. ANIMATE THE ACTIVE ITEM (Spin & Bob)
        if (currentItemType > 0 && currentItemType < weaponModels.Length)
        {
            GameObject activeModel = weaponModels[currentItemType];
            if (activeModel != null && activeModel.activeSelf)
            {
                // A. Rotate (Spin)
                activeModel.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

                // B. Bobbing (Float Up/Down)
                if (enableBobbing)
                {
                    Vector3 startPos = _initialLocalPositions[currentItemType];
                    // Calculate new Y using Sine wave
                    float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
                    
                    // Apply position relative to the Spawn Point parent
                    activeModel.transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
                }
            }
        }

        // --- EXISTING UI LOGIC ---

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

            // 4. Make text always face the camera
            if (isClose && Camera.main != null)
            {
                pickupText.transform.LookAt(Camera.main.transform);
                pickupText.transform.Rotate(0, 180, 0); // Flip so it's not backwards
            }
        }
    }
}