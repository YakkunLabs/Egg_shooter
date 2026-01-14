using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class FinalGameFixer : EditorWindow
{
    [MenuItem("Tools/👑 FINAL FIX (Loadout & SMG Polish)")]
    public static void Run()
    {
        // 1. Open Game Scene
        string sceneName = "SimpleNaturePack_Demo";
        string scenePath = FindScenePath(sceneName);
        if (string.IsNullOrEmpty(scenePath)) return;
        Scene scene = EditorSceneManager.OpenScene(scenePath);

        // 2. Find Player & WeaponHolder
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) player = GameObject.Find("Player");
        
        LoadoutManager lm = player.GetComponent<LoadoutManager>();
        if (lm == null) lm = player.AddComponent<LoadoutManager>();

        Transform holder = player.transform.Find("WeaponHolder");
        if (holder == null) holder = player.transform; // Fallback

        // 3. IDENTIFY ALL WEAPONS
        // We need to find them or create placeholders if missing to ensure array alignment
        GameObject[] weapons = new GameObject[5];
        
        weapons[0] = FindChild(holder, "Pistol") ?? FindChild(holder, "Pistal");               // 0: Pistol
        weapons[1] = FindChild(holder, "Assault Rifle") ?? FindChild(holder, "Rifle");        // 1: Rifle
        weapons[2] = FindChild(holder, "Sniper") ?? FindChild(holder, "Sniper Rifle");        // 2: Sniper
        weapons[3] = FindChild(holder, "SMG_Weapon");                                         // 3: SMG
        weapons[4] = FindChild(holder, "RocketLauncher_Weapon");                              // 4: Rocket

        // 4. FIX LOADOUT MANAGER (The "Double Weapon" Fix)
        SerializedObject so = new SerializedObject(lm);
        SerializedProperty arr = so.FindProperty("weapons");
        arr.arraySize = 5;
        
        for (int i = 0; i < 5; i++)
        {
            if (weapons[i] != null)
            {
                arr.GetArrayElementAtIndex(i).objectReferenceValue = weapons[i];
                // CRITICAL: Turn them OFF so they don't appear together
                weapons[i].SetActive(false); 
            }
        }
        so.ApplyModifiedProperties();
        Debug.Log("✅ LoadoutManager updated & all weapons disabled by default.");

        // 5. POLISH SMG (Make it like Rifle)
        GameObject rifle = weapons[1];
        GameObject smg = weapons[3];

        if (rifle != null && smg != null)
        {
            // Copy Position/Rotation
            smg.transform.localPosition = rifle.transform.localPosition;
            smg.transform.localRotation = rifle.transform.localRotation;
            smg.transform.localScale = rifle.transform.localScale;

            // Copy Stats & VFX
            AdvancedGunSystem rifleSys = rifle.GetComponent<AdvancedGunSystem>();
            AdvancedGunSystem smgSys = smg.GetComponent<AdvancedGunSystem>();
            
            if (rifleSys != null && smgSys != null)
            {
                smgSys.bulletPrefab = rifleSys.bulletPrefab; // Same bullets
                smgSys.muzzleFlash = CloneFlash(rifleSys.muzzleFlash, smg.transform); // Same flash
                
                if (smgSys.weaponData != null && rifleSys.weaponData != null)
                {
                    smgSys.weaponData.shootSound = rifleSys.weaponData.shootSound;
                    smgSys.weaponData.reloadSound = rifleSys.weaponData.reloadSound;
                    smgSys.weaponData.bulletTracerPrefab = rifleSys.weaponData.bulletTracerPrefab;
                    
                    // SMG Specific Stats (Faster fire, lower damage)
                    smgSys.weaponData.fireMode = FireMode.Automatic;
                    smgSys.weaponData.roundsPerMinute = 800; // Fast!
                    EditorUtility.SetDirty(smgSys.weaponData);
                }
                
                // Align AttackPoint
                Transform existingAP = smg.transform.Find("AttackPoint");
                if (existingAP == null) 
                {
                    existingAP = new GameObject("AttackPoint").transform;
                    existingAP.SetParent(smg.transform, false);
                }
                existingAP.localPosition = new Vector3(0, 0.15f, 0.7f); // Barrel tip
                smgSys.attackPoint = existingAP;
            }
            Debug.Log("✨ SMG Polished to match Rifle specs.");
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        
        EditorUtility.DisplayDialog("Complete", 
            "Final Fix Applied!\n\n" +
            "• Fixed Overlapping Weapons (All disabled by default)\n" +
            "• SMG Visuals & Feel matched to Rifle\n" +
            "• Rocket Left Unique\n" +
            "• Loadout Correctly Configured", "PLAY NOW");
    }

    private static GameObject FindChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains(name)) return child.gameObject;
        }
        return null;
    }

    private static ParticleSystem CloneFlash(ParticleSystem original, Transform parent)
    {
        if (original == null) return null;
        Transform old = parent.Find("VFX_Flash_Polished");
        if (old) DestroyImmediate(old.gameObject);

        ParticleSystem newFlash = Instantiate(original, parent);
        newFlash.name = "VFX_Flash_Polished";
        newFlash.transform.localPosition = new Vector3(0, 0.1f, 0.6f);
        return newFlash;
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
