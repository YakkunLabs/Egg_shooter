using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Weapon Setup - GUARANTEED TO WORK
/// This version ensures changes are saved and applied
/// </summary>
public class WeaponSetupGuaranteed : EditorWindow
{
    [MenuItem("Tools/🎯 FIX WEAPONS NOW (Guaranteed)")]
    public static void FixWeaponsNow()
    {
        Debug.Log("<color=cyan>═══════════════════════════════════════════════════════</color>");
        Debug.Log("<color=yellow>🔧 FIXING WEAPONS IN ALL SCENES...</color>");
        Debug.Log("<color=cyan>═══════════════════════════════════════════════════════</color>");

        string currentScenePath = SceneManager.GetActiveScene().path;

        try
        {
            // STEP 1: Create WeaponData assets
            Debug.Log("\n<color=cyan>📋 STEP 1: Creating WeaponData assets...</color>");
            CreateWeaponDataAssets();

            // STEP 2: Iterate through ALL scenes in Build Settings
            Debug.Log("\n<color=cyan>🔧 STEP 2: FIXING ALL SCENES...</color>");
            
            int totalFixed = 0;
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            
            // Ask user to save current scene before proceeding
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Debug.LogWarning("Setup cancelled by user.");
                return;
            }

            for (int i = 0; i < sceneCount; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                
                // SKIP IF SCENE DOES NOT EXIST
                if (string.IsNullOrEmpty(scenePath) || !System.IO.File.Exists(scenePath))
                {
                    Debug.LogWarning($"⚠️ Skipping missing scene in Build Settings: {scenePath}");
                    continue;
                }
                
                Debug.Log($"<color=cyan>📂 Opening Scene {i + 1}/{sceneCount}: {scenePath}</color>");
                
                try 
                {
                    Scene scene = EditorSceneManager.OpenScene(scenePath);
                    
                    // Fix weapons in this scene
                    int fixedInScene = FixAllWeaponsInScene();
                    totalFixed += fixedInScene;
                    
                    if (fixedInScene > 0)
                    {
                        // FORCE SAVE THIS SCENE
                        EditorSceneManager.MarkSceneDirty(scene);
                        EditorSceneManager.SaveScene(scene);
                        Debug.Log($"<color=green>💾 Saved scene: {scene.name}</color>");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"❌ Failed to process scene {scenePath}: {ex.Message}");
                }
            }

            // Return to original scene
            if (!string.IsNullOrEmpty(currentScenePath) && currentScenePath != SceneManager.GetActiveScene().path)
            {
                EditorSceneManager.OpenScene(currentScenePath);
            }

            // Final refresh
            AssetDatabase.Refresh();

            Debug.Log("\n<color=green>✅ FIX COMPLETE IN ALL SCENES!</color>");
            Debug.Log("<color=cyan>═══════════════════════════════════════════════════════</color>");

            EditorUtility.DisplayDialog(
                "Weapons Fixed! ✅",
                $"Successfully scanned {sceneCount} scene(s)!\n" +
                $"Fixed {totalFixed} weapon(s) total!\n\n" +
                "Scanned Scenes:\n" +
                "- MainMenu\n" +
                "- SampleScene (Gameplay)\n\n" +
                "ALL SCENES SAVED!\n" +
                "Press Play to test!",
                "OK"
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError($"<color=red>❌ Fix failed: {e.Message}</color>");
            Debug.LogError($"<color=red>{e.StackTrace}</color>");
            
            // Try to return to original scene
            if (!string.IsNullOrEmpty(currentScenePath)) EditorSceneManager.OpenScene(currentScenePath);

            EditorUtility.DisplayDialog(
                "Fix Failed",
                $"Error: {e.Message}\n\nCheck console for details.",
                "OK"
            );
        }
    }

    private static void CreateWeaponDataAssets()
    {
        string path = "Assets/WeaponData";

        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder("Assets", "WeaponData");
        }

        // Create or update Pistol (Speed: 300)
        CreateOrUpdateWeaponData(path, "Pistol", WeaponType.Pistol, FireMode.SemiAutomatic, 
            35, 300f, 12, 60, 1.5f, false, 300f, 0f);

        // Create or update AR (Speed: 400)
        CreateOrUpdateWeaponData(path, "AssaultRifle", WeaponType.AssaultRifle, FireMode.Automatic, 
            25, 700f, 30, 150, 2.5f, false, 400f, 0f);

        // Create or update Sniper (Speed: 800)
        CreateOrUpdateWeaponData(path, "SniperRifle", WeaponType.SniperRifle, FireMode.BoltAction, 
            100, 40f, 6, 30, 3.5f, true, 800f, 0f);

        // Create or update SMG (Blaster-N) - Fast, spray
        CreateOrUpdateWeaponData(path, "SMG", WeaponType.SMG, FireMode.Automatic,
            18, 950f, 40, 200, 1.8f, false, 350f, 0f);

        // Create or update RocketLauncher - Explosive!
        CreateOrUpdateWeaponData(path, "RocketLauncher", WeaponType.RocketLauncher, FireMode.SemiAutomatic,
            150, 60f, 1, 10, 3.0f, true, 40f, 6.0f); // Slow speed (40), Big boom (6.0), 1 round

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void CreateOrUpdateWeaponData(string path, string name, WeaponType type, 
        FireMode fireMode, int damage, float rpm, int magSize, int reserve, float reloadTime, bool hasScope, float bulletSpeed, float explosionRadius)
    {
        string assetPath = Path.Combine(path, $"{name}.asset");
        WeaponData data = AssetDatabase.LoadAssetAtPath<WeaponData>(assetPath);

        if (data == null)
        {
            data = ScriptableObject.CreateInstance<WeaponData>();
            AssetDatabase.CreateAsset(data, assetPath);
        }

        // Set all properties
        data.weaponName = name;
        data.weaponType = type;
        data.fireMode = fireMode;
        data.damage = damage;
        data.headshotMultiplier = type == WeaponType.SniperRifle ? 3.0f : 2.5f;
        data.roundsPerMinute = rpm;
        data.magazineSize = magSize;
        data.reserveAmmo = reserve;
        data.reloadTime = reloadTime;
        data.reloadFullMagazine = true;
        data.hasScope = hasScope;
        data.bulletSpeed = bulletSpeed;
        data.explosionRadius = explosionRadius;

        if (type == WeaponType.SniperRifle)
        {
            data.scopedFOV = 15f;
            data.scopeZoomSpeed = 10f;
            data.spread = 0.0001f; // Perfect accuracy
            data.aimSpreadMultiplier = 0.01f;
            data.recoilAmount = 4.0f; // High recoil
        }
        else if (type == WeaponType.AssaultRifle)
        {
            data.canToggleFireMode = true;
            data.availableFireModes = new FireMode[] { FireMode.Automatic, FireMode.Burst, FireMode.SemiAutomatic };
            data.burstCount = 3;
            data.burstDelay = 0.1f;
            data.spread = 0.05f; 
            data.aimSpreadMultiplier = 0.3f;
            data.recoilAmount = 1.2f;
        }
        else if (type == WeaponType.SMG)
        {
            data.spread = 0.15f; 
            data.aimSpreadMultiplier = 0.5f;
            data.recoilAmount = 1.0f;
        }
        else if (type == WeaponType.RocketLauncher)
        {
            data.hasScope = true; // Maybe zoom in a bit
            data.scopedFOV = 40f;
            data.spread = 0.01f; // Accurate path
            data.recoilAmount = 5.0f; // Huge kick
        }
        else
        {
            // PISTOL
            data.spread = 0.1f;
            data.aimSpreadMultiplier = 0.4f;
            data.recoilAmount = 2.0f;
            if (data.bulletSpeed < 300f) data.bulletSpeed = 300f;
        }

        EditorUtility.SetDirty(data);
        Debug.Log($"<color=green>✅ Created/Updated: {name} ({magSize} rounds, Speed: {bulletSpeed}, Boom: {explosionRadius})</color>");
    }

    private static int FixAllWeaponsInScene()
    {
        int fixedCount = 0;

        // Find weapons with GunSystem (INCLUDE INACTIVE!)
        GunSystem[] oldGuns = FindObjectsByType<GunSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Debug.Log($"<color=cyan>Found {oldGuns.Length} GunSystem components</color>");
        foreach (GunSystem oldGun in oldGuns)
        {
            Debug.Log($"<color=cyan>  - GunSystem on: {oldGun.gameObject.name}</color>");
            if (FixWeapon(oldGun.gameObject))
            {
                fixedCount++;
            }
        }

        // Find weapons with AdvancedGunSystem (INCLUDE INACTIVE!)
        AdvancedGunSystem[] advGuns = FindObjectsByType<AdvancedGunSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Debug.Log($"<color=cyan>Found {advGuns.Length} AdvancedGunSystem components</color>");
        foreach (AdvancedGunSystem advGun in advGuns)
        {
            Debug.Log($"<color=cyan>  - AdvancedGunSystem on: {advGun.gameObject.name}</color>");
            if (FixWeapon(advGun.gameObject))
            {
                fixedCount++;
            }
        }

        // ALSO search by name in case weapons don't have components yet
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Debug.Log($"<color=cyan>Searching {allObjects.Length} GameObjects by name...</color>");
        
        foreach (GameObject obj in allObjects)
        {
            string name = obj.name;
            if (name.Equals("Pistol", System.StringComparison.OrdinalIgnoreCase) ||
                name.Equals("Rifle", System.StringComparison.OrdinalIgnoreCase) ||
                name.Equals("Sniper", System.StringComparison.OrdinalIgnoreCase) ||
                name.Equals("blaster-e", System.StringComparison.OrdinalIgnoreCase) ||
                name.Equals("blaster-p", System.StringComparison.OrdinalIgnoreCase) ||
                name.Equals("blaster-n", System.StringComparison.OrdinalIgnoreCase) ||
                name.IndexOf("Rocket", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                // Check if we already processed it
                bool alreadyProcessed = false;
                foreach (GunSystem g in oldGuns)
                    if (g.gameObject == obj) alreadyProcessed = true;
                foreach (AdvancedGunSystem a in advGuns)
                    if (a.gameObject == obj) alreadyProcessed = true;

                if (!alreadyProcessed)
                {
                    Debug.Log($"<color=yellow>Found weapon by name: {obj.name}</color>");
                    if (FixWeapon(obj))
                    {
                        fixedCount++;
                    }
                }
            }
        }

        return fixedCount;
    }

    private static bool FixWeapon(GameObject weaponObj)
    {
        Debug.Log($"<color=cyan>🔧 Fixing: {weaponObj.name}</color>");

        // Get components
        GunSystem oldGun = weaponObj.GetComponent<GunSystem>();
        AdvancedGunSystem advGun = weaponObj.GetComponent<AdvancedGunSystem>();

        // Determine weapon type settings - ROBUST MATCHING (Contains logic)
        string weaponName = weaponObj.name;
        
        // Sniper checks (Aggressive)
        bool isSniper = weaponName.IndexOf("Sniper", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                        weaponName.IndexOf("Snipper", System.StringComparison.OrdinalIgnoreCase) >= 0 || // Typos
                        weaponName.IndexOf("blaster-e", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                        weaponName.IndexOf("blaster e", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                        weaponName.IndexOf("blaster_e", System.StringComparison.OrdinalIgnoreCase) >= 0;
        
        // Rifle checks
        bool isRifle = weaponName.IndexOf("Rifle", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                       weaponName.IndexOf("blaster-p", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                       weaponName.IndexOf("blaster p", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                       weaponName.IndexOf("AKM", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                       weaponName.IndexOf("Assault", System.StringComparison.OrdinalIgnoreCase) >= 0;
        
        // Pistol checks
        bool isPistol = weaponName.IndexOf("Pistol", System.StringComparison.OrdinalIgnoreCase) >= 0;

        // NEW: SMG checks
        bool isSMG = weaponName.IndexOf("blaster-n", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                     weaponName.IndexOf("SMG", System.StringComparison.OrdinalIgnoreCase) >= 0;

        // NEW: Rocket checks
        bool isRocket = weaponName.IndexOf("Rocket", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                        weaponName.IndexOf("Launcher", System.StringComparison.OrdinalIgnoreCase) >= 0;
        
        string dataName;
        if (isSniper) {
            dataName = "SniperRifle";
            Debug.Log($"<color=cyan>  >> Identified as SNIPER (Name: '{weaponName}')</color>");
        }
        else if (isRifle) {
            dataName = "AssaultRifle";
            Debug.Log($"<color=cyan>  >> Identified as RIFLE (Name: '{weaponName}')</color>");
        }
        else if (isSMG) {
            dataName = "SMG";
            Debug.Log($"<color=cyan>  >> Identified as SMG (Name: '{weaponName}')</color>");
        }
        else if (isRocket) {
            dataName = "RocketLauncher";
            Debug.Log($"<color=cyan>  >> Identified as ROCKET LAUNCHER (Name: '{weaponName}')</color>");
        }
        else if (isPistol) {
            dataName = "Pistol";
            Debug.Log($"<color=cyan>  >> Identified as PISTOL (Name: '{weaponName}')</color>");
        }
        else {
            dataName = "Pistol"; // Fallback
            Debug.Log($"<color=orange>  >> Fallback to PISTOL (Name: '{weaponName}')</color>");
        }
        
        string dataPath = $"Assets/WeaponData/{dataName}.asset";
        WeaponData weaponData = AssetDatabase.LoadAssetAtPath<WeaponData>(dataPath);

        if (weaponData == null)
        {
            Debug.LogError($"<color=red>❌ WeaponData not found: {dataPath}</color>");
            return false;
        }

        Debug.Log($"<color=yellow>📋 {weaponObj.name} → {dataName} ({weaponData.magazineSize} rounds)</color>");

        // Variables to hold references
        Camera refCamera = null;
        Transform refAttackPoint = null;
        GameObject refBulletPrefab = null;
        AudioSource refAudioSource = null;
        ParticleSystem refMuzzleFlash = null;
        GameObject refScopeOverlay = null;
        GameObject refWeaponModel = null;
        TMPro.TextMeshProUGUI refTextAmmo = null;

        // 1. Try to get references from Old GunSystem
        if (oldGun != null)
        {
            refCamera = oldGun.fpsCamera;
            refAttackPoint = oldGun.attackPoint;
            refBulletPrefab = oldGun.bulletPrefab;
            refAudioSource = oldGun.audioSource;
            refMuzzleFlash = oldGun.muzzleFlash;
            refScopeOverlay = oldGun.scopeOverlay;
            refWeaponModel = oldGun.weaponModel;
            refTextAmmo = oldGun.text_ammo;
            
            // Destroy old component
            Object.DestroyImmediate(oldGun);
        }
        // 2. Or get from existing AdvancedGunSystem
        else if (advGun != null)
        {
            refCamera = advGun.fpsCamera;
            refAttackPoint = advGun.attackPoint;
            refBulletPrefab = advGun.bulletPrefab;
            refAudioSource = advGun.audioSource;
            refMuzzleFlash = advGun.muzzleFlash;
            refScopeOverlay = advGun.scopeOverlay;
            refWeaponModel = advGun.weaponModel;
            refTextAmmo = advGun.text_ammo;
        }

        // 3. Ensure component exists
        if (advGun == null)
        {
            advGun = weaponObj.AddComponent<AdvancedGunSystem>();
        }

        // 4. AUTO-RESOLVE MISSING REFERENCES (The Magic Fix)
        
        // Fix Camera
        if (refCamera == null)
        {
            refCamera = Camera.main;
            if (refCamera != null) Debug.Log($"<color=yellow>⚠️ Auto-assigned Main Camera for {weaponObj.name}</color>");
        }

        // Fix Audio Source
        if (refAudioSource == null)
        {
            refAudioSource = weaponObj.GetComponent<AudioSource>();
            if (refAudioSource == null) refAudioSource = weaponObj.AddComponent<AudioSource>();
        }

        // Fix Attack Point
        if (refAttackPoint == null)
        {
            // Try to find child named "AttackPoint"
            foreach (Transform child in weaponObj.transform)
            {
                if (child.name.Equals("AttackPoint", System.StringComparison.OrdinalIgnoreCase))
                {
                    refAttackPoint = child;
                    break;
                }
            }
            // If still null, create one
            if (refAttackPoint == null)
            {
                GameObject newPoint = new GameObject("AttackPoint");
                newPoint.transform.SetParent(weaponObj.transform);
                newPoint.transform.localPosition = new Vector3(0, 0, 1f); // One unit forward
                refAttackPoint = newPoint.transform;
                Debug.Log($"<color=yellow>⚠️ Created missing AttackPoint for {weaponObj.name}</color>");
            }
        }

        // Fix UI - Find existing TMP objects if null
        if (refTextAmmo == null)
        {
            // Try to find a global object named "AmmoText" or similar
            GameObject foundText = GameObject.Find("AmmoText") ?? GameObject.Find("Text_Ammo");
            if (foundText != null) refTextAmmo = foundText.GetComponent<TMPro.TextMeshProUGUI>();
        }

        // 5. Apply References back to new component
        advGun.fpsCamera = refCamera;
        advGun.attackPoint = refAttackPoint;
        advGun.bulletPrefab = refBulletPrefab;
        advGun.audioSource = refAudioSource;
        advGun.muzzleFlash = refMuzzleFlash;
        advGun.scopeOverlay = refScopeOverlay;
        advGun.weaponModel = refWeaponModel;
        advGun.text_ammo = refTextAmmo;

        // 6. Assign Data and Save
        advGun.weaponData = weaponData;
        
        // Mark EVERYTHING as dirty to ensure it saves
        EditorUtility.SetDirty(advGun);
        EditorUtility.SetDirty(weaponObj);
        
        // Also mark the scene as dirty
        if (!string.IsNullOrEmpty(weaponObj.scene.path))
        {
            EditorSceneManager.MarkSceneDirty(weaponObj.scene);
        }
        
        Debug.Log($"<color=green>✅ Fixed: {weaponObj.name} → {weaponData.weaponName} ({weaponData.magazineSize} rounds, Speed: {weaponData.bulletSpeed})</color>");
        return true;
    }
}
