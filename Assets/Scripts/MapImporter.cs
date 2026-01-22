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
    public Material ghostMaterial; 

    // --- DATA ---
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
        public bool isTrigger; // <-- NEW
    }

    // --- LOGIC ---
    public void ReconstructMap()
    {
        string path = Path.Combine(Application.dataPath, fileName);
        if (!File.Exists(path)) { Debug.LogError("File not found: " + path); return; }

        string json = File.ReadAllText(path);
        MapData data = JsonUtility.FromJson<MapData>(json);

        // Clean up old children
        while (transform.childCount > 0) DestroyImmediate(transform.GetChild(0).gameObject);

        foreach (ColliderEntry entry in data.colliders)
        {
            GameObject ghostObj = null;

            if (entry.type == "Box") ghostObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            else if (entry.type == "Sphere") ghostObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            else if (entry.type == "Capsule") ghostObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);

            if (ghostObj != null)
            {
                ghostObj.transform.SetParent(transform);
                ghostObj.name = $"{entry.type} [{entry.tag}]";
                ghostObj.transform.position = entry.position;
                ghostObj.transform.rotation = entry.rotation;
                ghostObj.transform.localScale = entry.scale;

                // 1. SET LAYER
                int layerIndex = LayerMask.NameToLayer(entry.layer);
                if (layerIndex >= 0) ghostObj.layer = layerIndex;

                // 2. SET TAG (Safely)
                try {
                    if (!string.IsNullOrEmpty(entry.tag)) ghostObj.tag = entry.tag;
                } catch {
                    Debug.LogWarning($"Tag '{entry.tag}' missing in Project Settings.");
                }

                // 3. SET TRIGGER
                Collider col = ghostObj.GetComponent<Collider>();
                if (col != null) col.isTrigger = entry.isTrigger;

                // 4. MATERIAL
                if (ghostMaterial != null) ghostObj.GetComponent<Renderer>().material = ghostMaterial;
            }
        }
        Debug.Log($"<b>[MapImporter]</b> Reconstructed {data.colliders.Count} objects!");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MapImporter))]
public class MapImporterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MapImporter script = (MapImporter)target;
        if (GUILayout.Button("Reconstruct from JSON")) script.ReconstructMap();
    }
}
#endif