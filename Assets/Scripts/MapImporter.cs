using UnityEngine;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapImporter : MonoBehaviour
{
    [Header("Import Settings")]
    public string fileName = "map_data.json";
    public Material ghostMaterial; // Optional: Drag a transparent red material here to look cool

    // --- REUSE THE DATA STRUCTURES (Must match Exporter) ---
    [System.Serializable]
    public class MapData
    {
        public List<ColliderEntry> colliders = new List<ColliderEntry>();
    }

    [System.Serializable]
    public class ColliderEntry
    {
        public string type;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public string tag;
        public string layer;
    }

    // --- RECONSTRUCTION LOGIC ---
    public void ReconstructMap()
    {
        // 1. Read the File
        string path = Path.Combine(Application.dataPath, fileName);
        if (!File.Exists(path))
        {
            Debug.LogError("Could not find file: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        MapData data = JsonUtility.FromJson<MapData>(json);

        // 2. Clean up old reconstruction (if any)
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        // 3. Loop through data and build objects
        foreach (ColliderEntry entry in data.colliders)
        {
            GameObject ghostObj = null;

            // Create the shape based on Type
            if (entry.type == "Box")
            {
                ghostObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }
            else if (entry.type == "Sphere")
            {
                ghostObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            }
            else if (entry.type == "Capsule")
            {
                ghostObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            }

            if (ghostObj != null)
            {
                // Parent it to this script's object so we keep the scene clean
                ghostObj.transform.SetParent(transform);
                ghostObj.name = $"{entry.type} [{entry.tag}]";

                // Set Position & Rotation directly
                ghostObj.transform.position = entry.position;
                ghostObj.transform.rotation = entry.rotation;
                ghostObj.transform.localScale = entry.scale;

                // Set Layer if it exists
                int layerIndex = LayerMask.NameToLayer(entry.layer);
                if (layerIndex >= 0) ghostObj.layer = layerIndex;

                // Visual Polish: Apply a "Ghost" material if you have one
                if (ghostMaterial != null)
                {
                    ghostObj.GetComponent<Renderer>().material = ghostMaterial;
                }
            }
        }

        Debug.Log($"<b>[MapImporter]</b> Reconstructed {data.colliders.Count} objects from JSON!");
    }
}

// --- EDITOR BUTTON ---
#if UNITY_EDITOR
[CustomEditor(typeof(MapImporter))]
public class MapImporterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapImporter script = (MapImporter)target;
        if (GUILayout.Button("Reconstruct from JSON"))
        {
            script.ReconstructMap();
        }
    }
}
#endif