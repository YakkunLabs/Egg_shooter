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

    [Header("ProBuilder / Land Fix")]
    [Tooltip("If true, it treats complex MeshColliders as simple Boxes based on their size. Great for floors/walls.")]
    public bool convertMeshesToBoxes = true;

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
    }

    // --- LOGIC ---
    public void ExportMap()
    {
        MapData data = new MapData();
        Collider[] allColliders = GetComponentsInChildren<Collider>();

        foreach (Collider col in allColliders)
        {
            // Skip Triggers if you only want solid walls
            if (col.isTrigger) continue;

            ColliderEntry entry = new ColliderEntry();
            entry.tag = exportTags ? col.gameObject.tag : "";
            entry.layer = exportLayers ? LayerMask.LayerToName(col.gameObject.layer) : "";
            
            Transform t = col.transform;

            // 1. HANDLE BOXES
            if (col is BoxCollider box)
            {
                entry.type = "Box";
                entry.position = t.TransformPoint(box.center);
                entry.rotation = t.rotation;
                entry.scale = Vector3.Scale(box.size, t.lossyScale);
                data.colliders.Add(entry);
            }
            // 2. HANDLE SPHERES
            else if (col is SphereCollider sphere)
            {
                entry.type = "Sphere";
                entry.position = t.TransformPoint(sphere.center);
                entry.rotation = t.rotation;
                float maxScale = Mathf.Max(t.lossyScale.x, Mathf.Max(t.lossyScale.y, t.lossyScale.z));
                float worldRadius = sphere.radius * maxScale;
                entry.scale = new Vector3(worldRadius, worldRadius, worldRadius);
                data.colliders.Add(entry);
            }
            // 3. HANDLE CAPSULES
            else if (col is CapsuleCollider cap)
            {
                entry.type = "Capsule";
                entry.position = t.TransformPoint(cap.center);
                entry.rotation = t.rotation;
                float heightScale = t.lossyScale.y;
                float radiusScale = Mathf.Max(t.lossyScale.x, t.lossyScale.z);
                entry.scale = new Vector3(cap.radius * radiusScale, cap.height * heightScale, 0); 
                data.colliders.Add(entry);
            }
            // 4. HANDLE MESH COLLIDERS (ProBuilder / Land)
            else if (col is MeshCollider meshCol)
            {
                if (convertMeshesToBoxes)
                {
                    // THE FIX: Measure the bounds and pretend it's a Box
                    entry.type = "Box";
                    entry.position = meshCol.bounds.center; // World Center
                    entry.rotation = Quaternion.identity;   // AABB (Axis Aligned) - Cannot rotate "Bounds" easily
                    entry.scale = meshCol.bounds.size;      // World Size
                    
                    // Note: This creates an "Axis Aligned" box. 
                    // If your wall is rotated 45 degrees, this might look weird (large box).
                    // Ideally, for rotated ProBuilder objects, you should use real BoxColliders.
                    
                    data.colliders.Add(entry);
                }
                else if (meshCol.convex)
                {
                    // Only export if strict Convex is enabled
                    // (Writing Convex export logic is complex, usually implies generic mesh data)
                    Debug.LogWarning($"Skipping Convex Mesh: {col.name}. (Complex mesh export not implemented yet)");
                }
            }
        }

        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.dataPath, fileName);
        File.WriteAllText(path, json);

        Debug.Log($"<b>[MapExporter]</b> Exported {data.colliders.Count} colliders to: {path}");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MapExporter))]
public class MapExporterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MapExporter script = (MapExporter)target;
        if (GUILayout.Button("Export Map JSON")) script.ExportMap();
    }
}
#endif