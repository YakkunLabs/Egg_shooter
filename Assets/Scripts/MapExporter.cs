using UnityEngine;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapExporter : MonoBehaviour
{
    [Header("Export Settings")]
    public string fileName = "map_data.json";
    public bool exportTags = true;
    public bool exportLayers = true;

    // --- DATA STRUCTURES (The Schema) ---
    [System.Serializable]
    public class MapData
    {
        public List<ColliderEntry> colliders = new List<ColliderEntry>();
    }

    [System.Serializable]
    public class ColliderEntry
    {
        public string type;       // "Box", "Sphere", "Capsule"
        public Vector3 position;  // World Space
        public Quaternion rotation; // World Space
        public Vector3 scale;     // World Dimensions (Total Size)
        public string tag;        // Optional
        public string layer;      // Optional
    }

    // --- EXPORT LOGIC ---
    public void ExportMap()
    {
        MapData data = new MapData();

        // 1. Find all colliders attached to this object or its children
        Collider[] allColliders = GetComponentsInChildren<Collider>();

        foreach (Collider col in allColliders)
        {
            // Skip triggers if you only want physical walls (optional)
            // if (col.isTrigger) continue; 

            ColliderEntry entry = new ColliderEntry();
            
            // Common Data
            entry.tag = exportTags ? col.gameObject.tag : "";
            entry.layer = exportLayers ? LayerMask.LayerToName(col.gameObject.layer) : "";

            // Calculate World Position/Rotation correcting for offsets
            // Note: We use the Transform's rotation, but Position must include the collider's center offset.
            Transform t = col.transform;

            if (col is BoxCollider box)
            {
                entry.type = "Box";
                // World Position = Transform Position + (Rotated Offset)
                entry.position = t.TransformPoint(box.center); 
                entry.rotation = t.rotation;
                // World Scale = Local Size * Global Scale
                entry.scale = Vector3.Scale(box.size, t.lossyScale); 
            }
            else if (col is SphereCollider sphere)
            {
                entry.type = "Sphere";
                entry.position = t.TransformPoint(sphere.center);
                entry.rotation = t.rotation;
                // Sphere radius scales by the largest axis of the transform
                float maxScale = Mathf.Max(t.lossyScale.x, Mathf.Max(t.lossyScale.y, t.lossyScale.z));
                float worldRadius = sphere.radius * maxScale;
                entry.scale = new Vector3(worldRadius, worldRadius, worldRadius);
            }
            else if (col is CapsuleCollider cap)
            {
                entry.type = "Capsule";
                entry.position = t.TransformPoint(cap.center);
                entry.rotation = t.rotation;
                
                // Capsules are tricky because Unity handles scaling weirdly. 
                // We will export the raw dimensions for the target engine to calculate.
                // Assuming Y-axis height (Unity default)
                float heightScale = t.lossyScale.y;
                float radiusScale = Mathf.Max(t.lossyScale.x, t.lossyScale.z);
                
                entry.scale = new Vector3(cap.radius * radiusScale, cap.height * heightScale, 0); 
                // X = World Radius, Y = World Height
            }
            else
            {
                // Skip MeshColliders (Too heavy/complex for simple export)
                continue;
            }

            data.colliders.Add(entry);
        }

        // 2. Convert to JSON
        string json = JsonUtility.ToJson(data, true);

        // 3. Save to File
        string path = Path.Combine(Application.dataPath, fileName);
        File.WriteAllText(path, json);

        Debug.Log($"<b>[MapExporter]</b> Exported {data.colliders.Count} colliders to: {path}");
    }
}

// --- EDITOR BUTTON ---
#if UNITY_EDITOR
[CustomEditor(typeof(MapExporter))]
public class MapExporterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapExporter script = (MapExporter)target;
        if (GUILayout.Button("Export Map JSON"))
        {
            script.ExportMap();
        }
    }
}
#endif