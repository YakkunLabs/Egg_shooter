using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor; // Required for Handles.Label
#endif

public class ItemSpawnManager : MonoBehaviour
{
    public static ItemSpawnManager Instance;

    [Header("Configuration")]
    // ⚠️ Drag Empty GameObjects here. Order MUST match Server (0, 1, 2...)
    public Transform[] spawnLocations; 

    // ⚠️ Drag Prefabs here. Index matches Item ID (Element 1 = Pistol, 11 = Rifle)
    public GameObject[] itemPrefabs; 

    // Internal tracker of what is currently spawned
    private GameObject[] _activeItems;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Initialize array to track spawned objects
        _activeItems = new GameObject[spawnLocations.Length];
    }

    void Start()
    {
        // ✅ LOG: Print all locations to Console on Start
        LogAllLocations();
    }

    // Called by NetClient
    public void UpdateLocation(int locationIndex, int itemId)
    {
        // 1. Validate Input
        if (locationIndex < 0 || locationIndex >= spawnLocations.Length) return;

        // 2. Cleanup Old Item
        if (_activeItems[locationIndex] != null)
        {
            Destroy(_activeItems[locationIndex]);
            _activeItems[locationIndex] = null;
        }

        // 3. Spawn New Item
        if (itemId > 0 && itemId < itemPrefabs.Length && itemPrefabs[itemId] != null)
        {
            Transform loc = spawnLocations[locationIndex];
            GameObject newItem = Instantiate(itemPrefabs[itemId], loc.position, loc.rotation);
            _activeItems[locationIndex] = newItem;

            // ✅ LOG: Print when an item actually spawns
            Debug.Log($"[ItemManager] 📦 Spawned '{itemPrefabs[itemId].name}' at Location ID: {locationIndex}");
        }
    }

    // ---------------------------------------------------------
    // ✅ DEBUG TOOLS (Copy this part!)
    // ---------------------------------------------------------
    
    public void LogAllLocations()
    {
        Debug.Log("--- 🗺️ SPAWN LOCATION MAP ---");
        for (int i = 0; i < spawnLocations.Length; i++)
        {
            if (spawnLocations[i] != null)
            {
                string status = (_activeItems[i] != null) ? _activeItems[i].name : "Empty";
                Debug.Log($"[ID {i}] Position: {spawnLocations[i].position} | Current Item: {status}");
            }
            else
            {
                Debug.LogError($"[ID {i}] ❌ MISSING TRANSFORM! Check Inspector.");
            }
        }
        Debug.Log("-----------------------------");
    }

#if UNITY_EDITOR
    // Visualizes IDs in the Scene View
    private void OnDrawGizmos()
    {
        if (spawnLocations == null) return;

        for (int i = 0; i < spawnLocations.Length; i++)
        {
            if (spawnLocations[i] != null)
            {
                // Green if occupied, Yellow if empty
                bool hasItem = (_activeItems != null && i < _activeItems.Length && _activeItems[i] != null);
                Gizmos.color = hasItem ? Color.green : Color.yellow;
                
                Gizmos.DrawWireSphere(spawnLocations[i].position, 0.5f);

                // Draw Text Label
                string itemName = hasItem ? _activeItems[i].name : "Empty";
                string label = $"ID: {i}\n{itemName}";

                Handles.Label(spawnLocations[i].position + Vector3.up * 1.5f, label, new GUIStyle() 
                { 
                    normal = new GUIStyleState() { textColor = Gizmos.color },
                    fontSize = 15,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                });
            }
        }
    }
#endif
}