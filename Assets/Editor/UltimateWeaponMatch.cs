using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class UltimateWeaponMatch : EditorWindow
{
    [MenuItem("Tools/🧪 ULTIMATE WEAPON MATCH (Fix SMG & Duplicates)")]
    public static void Run()
    {
        string sceneName = "SimpleNaturePack_Demo";
        string scenePath = FindScenePath(sceneName);
        if (string.IsNullOrEmpty(scenePath)) return;
        Scene scene = EditorSceneManager.OpenScene(scenePath);

        GameObject player = GameObject.FindGameObjectWithTag("Player") ?? GameObject.Find("Player");
        if (player == null) return;

        // Default holder
        Transform holder = player.transform.Find("WeaponHolder") ?? player.transform;

        // 0. CLEANUP DUPLICATE WEAPON HOLDERS (CRITICAL FIX)
        Transform[] allChildren = player.GetComponentsInChildren<Transform>(true);
        Transform correctHolder = null;
        
        // First, find the "real" holder (one with the rifle)
        foreach (Transform t in allChildren)
        {
            if (t.name == "WeaponHolder")
            {
                if (FindChild(t, "blaster-p") != null || FindChild(t, "Pistal") != null)
                {
                    correctHolder = t;
                    break;
                }
            }
        }
        
        // If we found a good one, destroy bad ones safely
        if (correctHolder != null)
        {
            System.Collections.Generic.List<GameObject> toDestroy = new System.Collections.Generic.List<GameObject>();
            
            foreach (Transform t in allChildren)
            {
                // Check if valid reference first
                if (t != null && t.gameObject != null && t.name == "WeaponHolder" && t != correctHolder)
                {
                    toDestroy.Add(t.gameObject);
                }
            }
            
            foreach(GameObject g in toDestroy)
            {
                 if (g != null)
                 {
                    Debug.Log($"🔥 Destroying Ghost WeaponHolder: {g.name}");
                    DestroyImmediate(g);
                 }
            }
            holder = correctHolder;
        }

        // 1. FIND THE REFERENCE (RIFLE)
        // User confirmed rifle name is "blaster-p"
        GameObject rifle = FindChild(holder, "blaster-p") 
                        ?? FindChild(holder, "Assault Rifle") 
                        ?? FindChild(holder, "Rifle");

        if (rifle == null)
        {
            // Search deeper/broader if still not found
            Transform[] childrenSearch = holder.GetComponentsInChildren<Transform>(true);
            foreach(Transform t in childrenSearch) 
            {
                if (t.name.ToLower().Contains("blaster") && t.name.ToLower().Contains("p"))
                {
                    rifle = t.gameObject;
                    break;
                }
            }
        }

        if (rifle == null)
        {
            EditorUtility.DisplayDialog("Error", "Rifle (blaster-r) not found! Cannot copy settings.", "OK");
            return;
        }

        // 2. FIND OR CREATE SMG & ROCKET
        GameObject smg = EnsureWeapon(holder, "SMG_Weapon", "Assets/FBX format/blaster-n.fbx");
        GameObject rocket = EnsureWeapon(holder, "RocketLauncher_Weapon", "Assets/BigRookGames/_AssetPacks/Stylized Weapon Pack/Stylized Rocket Launcher/Prefabs/Rocket Launcher.prefab");

        // 3. COPY RIFLE SETTINGS TO SMG (EXACT CLONE)
        AdvancedGunSystem rifleSys = rifle.GetComponent<AdvancedGunSystem>();
        AdvancedGunSystem smgSys = smg.GetComponent<AdvancedGunSystem>();
        
        if (rifleSys && smgSys)
        {
            // COPY EXACT TRANSFORM FROM RIFLE
            // Based on blaster-p: Position(-0.00, 0.05, 0.624), Rotation(0, 180, 0), Scale(1.5, 1.5, 1.5)
            smg.transform.localPosition = rifle.transform.localPosition;
            smg.transform.localRotation = rifle.transform.localRotation;
            smg.transform.localScale = rifle.transform.localScale;
            
            Debug.Log($"✅ SMG Transform copied from Rifle: Pos={rifle.transform.localPosition}, Rot={rifle.transform.localRotation.eulerAngles}, Scale={rifle.transform.localScale}");
            
            // COPY BULLET PREFAB (Bullet_Root from rifle)
            smgSys.bulletPrefab = rifleSys.bulletPrefab;
            
            // COPY MUZZLE FLASH (Exact reference, not clone)
            smgSys.muzzleFlash = rifleSys.muzzleFlash;
            
            // COPY AUDIO SOURCE
            smgSys.audioSource = rifleSys.audioSource;
            
            // COPY ATTACK POINT
            smgSys.attackPoint = rifleSys.attackPoint;
            
            // COPY FPS CAMERA
            smgSys.fpsCamera = rifleSys.fpsCamera;

            // COPY WEAPON DATA SETTINGS
            if (rifleSys.weaponData != null && smgSys.weaponData != null)
            {
                smgSys.weaponData.shootSound = rifleSys.weaponData.shootSound;
                smgSys.weaponData.reloadSound = rifleSys.weaponData.reloadSound;
                smgSys.weaponData.bulletTracerPrefab = rifleSys.weaponData.bulletTracerPrefab;
                smgSys.weaponData.muzzleFlashPrefab = rifleSys.weaponData.muzzleFlashPrefab;
                
                // SMG SPECIFIC STATS (Keep these different from rifle)
                smgSys.weaponData.fireMode = FireMode.Automatic;
                smgSys.weaponData.roundsPerMinute = 750;
                smgSys.weaponData.magazineSize = 30;
                smgSys.weaponData.reserveAmmo = 120;
                EditorUtility.SetDirty(smgSys.weaponData);
            }
            
            Debug.Log("✅ SMG fully configured to match Rifle specs");
        }

        // 5. KILL DUPLICATE WEAPONS SAFELY
        // Scan children, keep only the OFFICIAL ones
        System.Collections.Generic.List<GameObject> roguesToDestroy = new System.Collections.Generic.List<GameObject>();
        
        foreach (Transform child in holder)
        {
            if (child == null) continue;
            GameObject c = child.gameObject;
            
            // SKIP THE OFFICIAL WEAPONS
            if (c == smg || c == rocket || c == rifle) continue;

            // If it looks like an SMG/Blaster but isn't the assigned one
            if (c.name.Contains("blaster") || c.name.Contains("SMG"))
            {
                roguesToDestroy.Add(c);
            }
            // If it looks like a Rocket but isn't the assigned one
            else if (c.name.Contains("Rocket") || c.name.Contains("Launcher"))
            {
                 roguesToDestroy.Add(c);
            }
        }
        
        foreach(GameObject rogue in roguesToDestroy)
        {
            if (rogue != null)
            {
                Debug.Log($"🔥 Destroying Rogue Duplicate: {rogue.name}");
                DestroyImmediate(rogue);
            }
        }
        
        // 6. UPDATE LOADOUT MANAGER
        LoadoutManager lm = player.GetComponent<LoadoutManager>();
        SerializedObject so = new SerializedObject(lm);
        SerializedProperty arr = so.FindProperty("weapons");
        arr.arraySize = 5;
        arr.GetArrayElementAtIndex(1).objectReferenceValue = rifle;
        arr.GetArrayElementAtIndex(3).objectReferenceValue = smg;
        arr.GetArrayElementAtIndex(4).objectReferenceValue = rocket;
        so.ApplyModifiedProperties();

        // 7. DISABLE ALL (Prevent overlap start)
        rifle.SetActive(false);
        smg.SetActive(false);
        rocket.SetActive(false);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        
        EditorUtility.DisplayDialog("Matched!", 
            "Weapons Matched & Duplicates Killed!\n\n" +
            "• SMG now uses RIFLE Bullets/Trails 100%\n" +
            "• Rogue Duplicate SMGs Destroyed\n" +
            "• SMG Positioned & Rotated Correctly", "PLAY");
    }

    private static GameObject FindChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains(name)) return child.gameObject;
        }
        return null;
    }

    private static GameObject EnsureWeapon(Transform parent, string name, string prefabPath)
    {
        GameObject existing = FindChild(parent, name);
        if (existing != null) return existing;

        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab)
        {
            GameObject viz = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            viz.transform.SetParent(go.transform, false);
            viz.name = "Visuals";
        }
        go.AddComponent<AdvancedGunSystem>();
        return go;
    }

    private static string FindScenePath(string sceneName)
    {
        string path = $"Assets/Scenes/{sceneName}.unity";
        if (System.IO.File.Exists(path)) return path;
        string[] guids = AssetDatabase.FindAssets($"t:Scene {sceneName}");
        if (guids.Length > 0) return AssetDatabase.GUIDToAssetPath(guids[0]);
        return null;
    }
}
