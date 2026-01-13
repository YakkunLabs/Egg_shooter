using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

public class FixWeaponsComplete : EditorWindow
{
    [MenuItem("Tools/🛠️ FIX & POLISH WEAPONS (One Click)")]
    public static void Run()
    {
        // 1. Open Scene
        string sceneName = "SimpleNaturePack_Demo";
        string scenePath = FindScenePath(sceneName);
        if (string.IsNullOrEmpty(scenePath))
        {
            EditorUtility.DisplayDialog("Error", $"Scene '{sceneName}' not found!", "OK");
            return;
        }
        Scene scene = EditorSceneManager.OpenScene(scenePath);

        // 2. Find Player & WeaponHolder
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) player = GameObject.Find("Player");
        if (player == null) player = FindAnyObjectByType<CharacterController>()?.gameObject;

        if (player == null)
        {
            EditorUtility.DisplayDialog("Error", "Could not find Player in scene!", "OK");
            return;
        }

        Transform holder = player.transform.Find("WeaponHolder");
        if (holder == null)
        {
            // Create if missing
            GameObject h = new GameObject("WeaponHolder");
            h.transform.SetParent(player.transform, false);
            h.transform.localPosition = new Vector3(0, 1.5f, 0); // Rough camera height
            holder = h.transform;
        }

        // 3. Find Reference Rifle (to steal settings)
        GameObject rifle = FindChildRecursively(holder, "Assault Rifle");
        if (rifle == null) rifle = FindChildRecursively(holder, "Rifle");
        
        // Capture Rifle Settings
        Vector3 refPos = new Vector3(0.35f, -0.35f, 0.6f); // Default good pos
        Quaternion refRot = Quaternion.identity;
        ParticleSystem refFlash = null;
        GameObject refBullet = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Bullet.prefab");
        AudioClip refShoot = null;
        AudioClip refReload = null;

        if (rifle != null)
        {
            refPos = rifle.transform.localPosition;
            refRot = rifle.transform.localRotation;
            
            var ags = rifle.GetComponent<AdvancedGunSystem>();
            if (ags != null)
            {
                refFlash = ags.muzzleFlash;
                refBullet = ags.bulletPrefab;
                if (ags.weaponData != null)
                {
                    refShoot = ags.weaponData.shootSound;
                    refReload = ags.weaponData.reloadSound;
                }
            }
        }

        // 4. SETUP SMG
        GameObject smg = VerifyWeapon(holder, "SMG_Weapon", "Assets/FBX format/blaster-n.fbx");
        // Apply Rifle Transform
        smg.transform.localPosition = refPos;
        smg.transform.localRotation = refRot;
        smg.transform.localScale = Vector3.one; 
        
        WeaponData smgData = GetOrCreateData("SMG_Data", WeaponType.SMG, FireMode.Automatic, 30, 150, 20, 600, 0);
        if (refShoot) smgData.shootSound = refShoot; // Copy Sound
        if (refReload) smgData.reloadSound = refReload;
        
        SetupAGS(smg, smgData, refBullet, refFlash);

        // 5. SETUP ROCKET
        GameObject rocket = VerifyWeapon(holder, "RocketLauncher_Weapon", "Assets/BigRookGames/_AssetPacks/Stylized Weapon Pack/Stylized Rocket Launcher/Prefabs/Rocket Launcher.prefab");
        // Rocket specific trans
        rocket.transform.localPosition = new Vector3(refPos.x, refPos.y - 0.2f, refPos.z);
        rocket.transform.localRotation = Quaternion.Euler(0, 180, 0); // Fix backward rocket
        
        WeaponData rktData = GetOrCreateData("Rocket_Data", WeaponType.RocketLauncher, FireMode.BoltAction, 1, 3, 80, 60, 6f);
        SetupAGS(rocket, rktData, refBullet, refFlash); // Share flash for now
        FixPinkRocket(rocket);

        // 6. UPDATE LOADOUT
        LoadoutManager lm = player.GetComponent<LoadoutManager>();
        if (lm == null) lm = player.AddComponent<LoadoutManager>();
        
        SerializedObject so = new SerializedObject(lm);
        SerializedProperty arr = so.FindProperty("weapons");
        arr.arraySize = 5;
        arr.GetArrayElementAtIndex(3).objectReferenceValue = smg;
        arr.GetArrayElementAtIndex(4).objectReferenceValue = rocket;
        so.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        
        EditorUtility.DisplayDialog("Fixed!", "Weapons Fixed, Polished, and Ready!", "PLAY");
    }

    private static GameObject VerifyWeapon(Transform parent, string name, string prefabPath)
    {
        Transform t = parent.Find(name);
        if (t == null)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            
            // Visuals
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab)
            {
                GameObject viz = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                viz.transform.SetParent(go.transform, false);
                viz.name = "Visuals";
                
                // Rotation fix for blaster-n if needed (usually visual child needs rotation, not parent)
                if (name.Contains("SMG")) viz.transform.localRotation = Quaternion.Euler(0, -90, 0);
            }
            return go;
        }
        return t.gameObject;
    }

    private static void SetupAGS(GameObject go, WeaponData data, GameObject bullet, ParticleSystem flash)
    {
        AdvancedGunSystem ags = go.GetComponent<AdvancedGunSystem>();
        if (ags == null) ags = go.AddComponent<AdvancedGunSystem>();
        
        ags.weaponData = data;
        ags.bulletPrefab = bullet;
        ags.attackPoint = go.transform.Find("Visuals") ?? go.transform;
        
        // Clone Flash
        if (flash != null)
        {
            Transform old = go.transform.Find("VFX_Flash");
            if (old) DestroyImmediate(old.gameObject);
            
            ParticleSystem newFlash = Instantiate(flash, go.transform);
            newFlash.name = "VFX_Flash";
            newFlash.transform.localPosition = new Vector3(0, 0.1f, 0.6f);
            ags.muzzleFlash = newFlash;
        }
    }

    private static WeaponData GetOrCreateData(string name, WeaponType type, FireMode mode, int mag, int total, int dmg, float rpm, float radius)
    {
        string path = $"Assets/Resources/WeaponData/{name}.asset";
        WeaponData d = AssetDatabase.LoadAssetAtPath<WeaponData>(path);
        if (d == null)
        {
            if (!Directory.Exists("Assets/Resources/WeaponData")) Directory.CreateDirectory("Assets/Resources/WeaponData");
            d = ScriptableObject.CreateInstance<WeaponData>();
            AssetDatabase.CreateAsset(d, path);
        }
        
        d.weaponName = name;
        d.weaponType = type;
        d.fireMode = mode;
        d.magazineSize = mag;
        d.reserveAmmo = total - mag;
        d.damage = dmg;
        d.roundsPerMinute = rpm;
        d.explosionRadius = radius;
        d.canToggleFireMode = false;
        
        EditorUtility.SetDirty(d);
        return d;
    }

    private static void FixPinkRocket(GameObject go)
    {
        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/BigRookGames/_AssetPacks/Stylized Weapon Pack/Stylized Rocket Launcher/Textures/rocket_launcher_Albedo.png");
        if (tex == null) return;
        
        Shader s = Shader.Find("Standard");
        Renderer[] rs = go.GetComponentsInChildren<Renderer>(true);
        foreach(var r in rs)
        {
            Material[] mats = r.sharedMaterials;
            for(int i=0; i<mats.Length; i++)
            {
                Material m = new Material(s);
                m.mainTexture = tex;
                if(m.HasProperty("_BaseMap")) m.SetTexture("_BaseMap", tex);
                mats[i] = m;
            }
            r.materials = mats;
        }
    }

    private static GameObject FindChildRecursively(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains(name)) return child.gameObject;
            GameObject f = FindChildRecursively(child, name);
            if (f != null) return f;
        }
        return null;
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
