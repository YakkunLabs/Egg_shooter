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
    public bool exportTriggers = true;

    [Header("ProBuilder / Land Fix")]
    public bool convertMeshesToBoxes = true;

    // --- DATA STRUCTURES ---
    [System.Serializable]
    public class MapData
    {
        public List<SpawnEntry> spawnPoints = new List<SpawnEntry>(); 
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
        public bool isTrigger;    
    }

    [System.Serializable]
    public class SpawnEntry
    {
        public string name;      // Will be "Spawn_1", "Spawn_2" etc.
        public string tag;       // NEW: Will be "spawn_point"
        public Vector3 position;
        public float yaw; 
    }

    // --- LOGIC ---
    public void ExportMap()
    {
        MapData data = new MapData();

        // ---------------------------------------------------------
        // PART 1: EXPORT SPAWN POINTS
        // ---------------------------------------------------------
        GameObject[] spawns = GameObject.FindGameObjectsWithTag("spawn_point");
        
        foreach (GameObject sp in spawns)
        {
            SpawnEntry s = new SpawnEntry();
            s.name = sp.name;               // Reads the object name (Rename them in Hierarchy!)
            s.tag = sp.tag;                 // Exports "spawn_point"
            s.position = sp.transform.position;
            s.yaw = sp.transform.eulerAngles.y; 
            data.spawnPoints.Add(s);
        }

        Debug.Log($"Found {data.spawnPoints.Count} Spawn Points.");

        // ---------------------------------------------------------
        // PART 2: EXPORT COLLIDERS
        // ---------------------------------------------------------
        Collider[] allColliders = GetComponentsInChildren<Collider>();

        foreach (Collider col in allColliders)
        {
            if (col.isTrigger && !exportTriggers) continue;

            ColliderEntry entry = new ColliderEntry();
            entry.tag = exportTags ? col.gameObject.tag : "Untagged";
            entry.layer = exportLayers ? LayerMask.LayerToName(col.gameObject.layer) : "Default";
            entry.isTrigger = col.isTrigger;

            Transform t = col.transform;

            if (col is BoxCollider box)
            {
                entry.type = "Box";
                entry.position = t.TransformPoint(box.center);
                entry.rotation = t.rotation;
                entry.scale = Vector3.Scale(box.size, t.lossyScale);
                data.colliders.Add(entry);
            }
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
            else if (col is MeshCollider meshCol)
            {
                if (convertMeshesToBoxes)
                {
                    entry.type = "Box";
                    entry.position = meshCol.bounds.center; 
                    entry.rotation = Quaternion.identity; 
                    entry.scale = meshCol.bounds.size;      
                    data.colliders.Add(entry);
                }
            }
        }

        // ---------------------------------------------------------
        // PART 3: SAVE FILE
        // ---------------------------------------------------------
        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.dataPath, fileName);
        File.WriteAllText(path, json);

        Debug.Log($"<b>[MapExporter]</b> Exported {data.colliders.Count} colliders and {data.spawnPoints.Count} spawns to: {path}");
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