using UnityEngine;
using System.Collections.Generic;

public class ItemSpawnManager : MonoBehaviour
{
    public static ItemSpawnManager Instance;

    [Header("Configuration")]
    // ⚠️ Drag Empty GameObjects here. Order MUST match Server (0, 1, 2...)
    public Transform[] spawnLocations; 

    // ⚠️ Drag Prefabs here. Index matches Item ID (Element 1 = Pistol, 11 = Rifle)
    // Make sure Element 0 is Empty!
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

    // Called by NetClient
    public void UpdateLocation(int locationIndex, int itemId)
    {
        // 1. Validate Input
        if (locationIndex < 0 || locationIndex >= spawnLocations.Length) return;

        // 2. Cleanup Old Item (If exists)
        if (_activeItems[locationIndex] != null)
        {
            Destroy(_activeItems[locationIndex]);
            _activeItems[locationIndex] = null;
        }

        // 3. Spawn New Item (If ID is valid)
        // We check if we have a prefab for this ID
        if (itemId > 0 && itemId < itemPrefabs.Length && itemPrefabs[itemId] != null)
        {
            Transform loc = spawnLocations[locationIndex];
            
            // Instantiate the correct prefab at the location
            GameObject newItem = Instantiate(itemPrefabs[itemId], loc.position, loc.rotation);
            
            // Keep track of it so we can delete it later
            _activeItems[locationIndex] = newItem;
        }
    }
    private void OnDrawGizmos()
    {
        if (spawnLocations == null) return;

        for (int i = 0; i < spawnLocations.Length; i++)
        {
            if (spawnLocations[i] != null)
            {
                // 1. Draw a Cyan Sphere to show the spot
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(spawnLocations[i].position, 0.5f);

                // 2. Draw the ID Number text (Only visible in Unity Editor)
                #if UNITY_EDITOR
                UnityEditor.Handles.Label(spawnLocations[i].position + Vector3.up * 1.5f, $"SERVER ID: {i}", new GUIStyle() 
                { 
                    normal = new GUIStyleState() { textColor = Color.yellow },
                    fontSize = 20,
                    fontStyle = FontStyle.Bold
                });
                #endif
            }
        }}
}