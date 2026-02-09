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
        public List<playerPoint> playerPoints = new List<playerPoint>(); 
        public List<ColliderEntry> colliders = new List<ColliderEntry>();
    }
    
    [System.Serializable]
    public class WeaponPoint
    {
        public string name;
        public string tag;
        public Vector3 position;
        public float yaw;
        public string player; // extracted from object name (e.g., "Rifle")
    }

    [System.Serializable]
    public class playerPoint
    {
        public string name;
        public string tag;
        public Vector3 position;
        public float yaw;
        public string player;
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
        public string name;      
        public string tag;       
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
            s.name = sp.name;               
            s.tag = sp.tag;                 
            s.position = sp.transform.position;
            s.yaw = sp.transform.eulerAngles.y; 
            data.spawnPoints.Add(s);
        }

        Debug.Log($"Found {data.spawnPoints.Count} Spawn Points.");

        // ---------------------------------------------------------
        // PART 2: EXPORT PLAYER POINTS (NEW)
        // ---------------------------------------------------------
        GameObject[] playerPoints = GameObject.FindGameObjectsWithTag("player_point");

        foreach (GameObject wp in playerPoints)
        {
            playerPoint w = new playerPoint();
            w.name = wp.name;
            w.tag = wp.tag;
            // Use the parent position (center of the crate area)
            w.position = wp.transform.position;
            w.yaw = wp.transform.eulerAngles.y;
            
           
            string rawName = wp.name.Replace("(Clone)", "").Replace("Spawn", "").Trim();
           
            w.player = System.Text.RegularExpressions.Regex.Replace(rawName, @"[\d-]", "").Trim();

            data.playerPoints.Add(w);
        }

        Debug.Log($"Found {data.playerPoints.Count} Player Points.");

        // ---------------------------------------------------------
        // PART 3: EXPORT COLLIDERS
        // ---------------------------------------------------------
        Collider[] allColliders = GetComponentsInChildren<Collider>();

        foreach (Collider col in allColliders)
        {
            if (col.isTrigger && !exportTriggers) continue;
            
            // Skip the weapon crates/triggers themselves if they are tagged
            if (col.CompareTag("weapon_point") || col.CompareTag("spawn_point")) continue;

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
        // PART 4: SAVE FILE
        // ---------------------------------------------------------
        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.dataPath, fileName);
        File.WriteAllText(path, json);

        Debug.Log($"<b>[MapExporter]</b> Exported {data.colliders.Count} colliders, {data.spawnPoints.Count} spawns, and {data.playerPoints.Count} player points to: {path}");
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