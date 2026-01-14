using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

public class GameWeaponSetup : EditorWindow
{
    [MenuItem("Tools/🎮 SETUP GAME WEAPONS (SMG & Rocket)")]
    public static void ShowWindow()
    {
        SetupGameWeapons();
    }

    public static void SetupGameWeapons()
    {
        // 1. Open Game Scene
        string sceneName = "SimpleNaturePack_Demo";
        string scenePath = FindScenePath(sceneName);
        if (string.IsNullOrEmpty(scenePath))
        {
            EditorUtility.DisplayDialog("Error", $"Scene '{sceneName}' not found!", "OK");
            return;
        }
        Scene scene = EditorSceneManager.OpenScene(scenePath);

        // 2. Find Player and Loadout
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) player = GameObject.Find("Player");
        
        if (player == null)
        {
            EditorUtility.DisplayDialog("Error", "Player object not found in scene!", "OK");
            return;
        }

        LoadoutManager loadout = player.GetComponent<LoadoutManager>();
        if (loadout == null) loadout = player.AddComponent<LoadoutManager>();

        Transform weaponHolder = player.transform.Find("WeaponHolder");
        if (weaponHolder == null)
        {
            // Try to find ANY child that looks like a camera or holder
            weaponHolder = player.GetComponentInChildren<Camera>()?.transform;
            if (weaponHolder == null) weaponHolder = player.transform;
        }

        // 3. Ensure Loadout Array Size
        SerializedObject so = new SerializedObject(loadout);
        SerializedProperty weaponsProp = so.FindProperty("weapons");
        weaponsProp.arraySize = 5; // Pistol, Rifle, Sniper, SMG, Rocket
        so.ApplyModifiedProperties();

        // 4. Create/Configure SMG
        GameObject smgObj = SetupWeaponObject(weaponHolder, "SMG_Weapon", "Assets/FBX format/blaster-n.fbx");
        WeaponData smgData = CreateWeaponAsset("SMG_Data", WeaponType.SMG, FireMode.Automatic, 30, 150, 20f, 600f, 0f);
        ConfigureGunSystem(smgObj, smgData);
        
        // SMG Transform Correction (blaster-n needs rotation)
        smgObj.transform.localPosition = new Vector3(0.5f, -0.4f, 0.7f);
        smgObj.transform.localRotation = Quaternion.Euler(0, -90, 0); 
        
        // Assign to Loadout Index 3
        weaponsProp.GetArrayElementAtIndex(3).objectReferenceValue = smgObj;

        // 5. Create/Configure Rocket Launcher
        GameObject rocketObj = SetupWeaponObject(weaponHolder, "RocketLauncher_Weapon", "Assets/BigRookGames/_AssetPacks/Stylized Weapon Pack/Stylized Rocket Launcher/Prefabs/Rocket Launcher.prefab");
        // Rocket: 1 mag, 3 total -> Reserve = 2. Explosive Radius = 5f.
        WeaponData rocketData = CreateWeaponAsset("Rocket_Data", WeaponType.RocketLauncher, FireMode.BoltAction, 1, 2, 80f, 60f, 6f); 
        ConfigureGunSystem(rocketObj, rocketData);
        
        // Rocket Transform Correction
        rocketObj.transform.localPosition = new Vector3(0.4f, -0.4f, 0.5f);
        rocketObj.transform.localRotation = Quaternion.Euler(0, 0, 0);
        
        // Fix Pink Texture on Rocket
        FixPinkRocket(rocketObj);

        // Assign to Loadout Index 4
        weaponsProp.GetArrayElementAtIndex(4).objectReferenceValue = rocketObj;
        
        // 6. Ensure Bullet Prefab
        GameObject bulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Bullet.prefab");
        if (bulletPrefab != null)
        {
            if (smgObj.GetComponent<AdvancedGunSystem>()) smgObj.GetComponent<AdvancedGunSystem>().bulletPrefab = bulletPrefab;
            if (rocketObj.GetComponent<AdvancedGunSystem>()) rocketObj.GetComponent<AdvancedGunSystem>().bulletPrefab = bulletPrefab;
        }

        so.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        EditorUtility.DisplayDialog("Success", 
            "Game Weapons Configured!\n\n" +
            "• SMG: 30 Mag, 150 Total, Auto\n" +
            "• Rocket: 1 Mag, 3 Total, Explosive\n" +
            "• WeaponData assets created in Assets/Resources/WeaponData\n" +
            "• Pink textures fixed", "PLAY NOW");
    }

    private static GameObject SetupWeaponObject(Transform parent, string name, string assetPath)
    {
        Transform existing = parent.Find(name);
        if (existing != null) return existing.gameObject;

        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);

        GameObject visuals = null;
        if (assetPath.EndsWith(".prefab"))
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab) visuals = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        }
        else
        {
            GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (model) visuals = (GameObject)PrefabUtility.InstantiatePrefab(model);
        }

        if (visuals != null)
        {
            visuals.transform.SetParent(go.transform, false);
            visuals.name = "Visuals";
        }
        
        // Add AudioSource
        if (go.GetComponent<AudioSource>() == null) go.AddComponent<AudioSource>();

        return go;
    }

    private static WeaponData CreateWeaponAsset(string name, WeaponType type, FireMode mode, int mag, int total, float damage, float rpm, float explosiveRadius)
    {
        string path = "Assets/Resources/WeaponData";
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        
        string assetPath = $"{path}/{name}.asset";
        WeaponData data = AssetDatabase.LoadAssetAtPath<WeaponData>(assetPath);
        
        if (data == null)
        {
            data = ScriptableObject.CreateInstance<WeaponData>();
            AssetDatabase.CreateAsset(data, assetPath);
        }

        data.weaponName = name.Replace("_Data", "");
        data.weaponType = type;
        data.fireMode = mode;
        data.magazineSize = mag;
        data.reserveAmmo = total - mag; // Total provided by user, subtract mag size for reserve
        if (data.reserveAmmo < 0) data.reserveAmmo = 0;
        
        data.damage = (int)damage;
        data.roundsPerMinute = rpm;
        data.explosionRadius = explosiveRadius;
        
        // Defaults
        data.reloadTime = 2.0f;
        data.spread = 1.0f;
        data.recoilAmount = 1.0f;
        
        EditorUtility.SetDirty(data);
        return data;
    }

    private static void ConfigureGunSystem(GameObject go, WeaponData data)
    {
        // Remove old scripts
        MonoBehaviour[] oldScripts = go.GetComponents<MonoBehaviour>();
        foreach (var script in oldScripts)
        {
            if (script.GetType().Name == "GunSystem") DestroyImmediate(script);
        }

        AdvancedGunSystem ags = go.GetComponent<AdvancedGunSystem>();
        if (ags == null) ags = go.AddComponent<AdvancedGunSystem>();
        
        ags.weaponData = data;
        ags.attackPoint = go.transform.Find("Visuals") ?? go.transform;
    }

    private static void FixPinkRocket(GameObject rocket)
    {
         string texPath = "Assets/BigRookGames/_AssetPacks/Stylized Weapon Pack/Stylized Rocket Launcher/Textures/rocket_launcher_Albedo.png";
         Texture2D albedo = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
         
         if (albedo == null) return;

         // Steal Shader from Pistol (common working shader)
         Shader shader = Shader.Find("Standard");
         GameObject pistol = GameObject.Find("Pistol");
         if (pistol != null)
         {
             Renderer pr = pistol.GetComponentInChildren<Renderer>();
             if (pr && pr.sharedMaterial) shader = pr.sharedMaterial.shader;
         }

         Renderer[] renderers = rocket.GetComponentsInChildren<Renderer>(true);
         foreach(var r in renderers)
         {
             Material[] mats = r.sharedMaterials;
             for(int i=0; i<mats.Length; i++)
             {
                 Material m = new Material(shader);
                 m.mainTexture = albedo;
                 if (m.HasProperty("_BaseMap")) m.SetTexture("_BaseMap", albedo);
                 m.name = "Fixed_Rocket_Mat";
                 mats[i] = m;
             }
             r.materials = mats;
         }
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
