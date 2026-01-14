using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Weapon Setup Quick Start - ONE-CLICK AUTOMATED SETUP
/// </summary>
public class WeaponSetupQuickStart : EditorWindow
{
    private bool setupComplete = false;
    private string statusMessage = "";
    private MessageType statusType = MessageType.Info;
    private Vector2 scrollPosition;

    // MAIN MENU ITEM - Runs setup immediately without showing window
    [MenuItem("Tools/🎯 Weapon Setup Quick Start")]
    public static void RunSetupImmediately()
    {
        Debug.Log("<color=cyan>═══════════════════════════════════════════════════════</color>");
        Debug.Log("<color=yellow>🚀 STARTING ONE-CLICK AUTOMATED WEAPON SETUP...</color>");
        Debug.Log("<color=cyan>═══════════════════════════════════════════════════════</color>");

        try
        {
            // STEP 1: Create Weapon Configurations
            Debug.Log("\n<color=cyan>📋 STEP 1: Creating Weapon Configurations...</color>");
            CreateWeaponConfigurationsStatic();

            // STEP 2: Find and Upgrade Weapons
            Debug.Log("\n<color=cyan>🔧 STEP 2: Finding and Upgrading Weapons...</color>");
            List<GameObject> weapons = FindWeaponsStatic();
            int upgraded = UpgradeWeaponsStatic(weapons);

            // STEP 3: Complete!
            Debug.Log("\n<color=green>✅ SETUP COMPLETE!</color>");
            Debug.Log("<color=cyan>═══════════════════════════════════════════════════════</color>");

            // Show completion dialog
            EditorUtility.DisplayDialog(
                "Setup Complete! ✅",
                $"Weapon setup completed successfully!\n\n" +
                $"✅ Created 3 weapon configurations\n" +
                $"✅ Found {weapons.Count} weapon(s) in scene\n" +
                $"✅ Upgraded {upgraded} weapon(s)\n\n" +
                "WEAPON SPECS:\n" +
                "• Pistol: 12 rounds, semi-auto\n" +
                "• AR: 30 rounds, auto/burst/semi\n" +
                "• Sniper: 6 rounds, bolt-action\n\n" +
                "Press Play to test your weapons!",
                "OK"
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError($"<color=red>❌ Setup failed: {e.Message}</color>");
            Debug.LogError($"<color=red>Stack trace: {e.StackTrace}</color>");

            EditorUtility.DisplayDialog(
                "Setup Failed",
                $"An error occurred during setup:\n\n{e.Message}\n\n" +
                "Check the console for details.",
                "OK"
            );
        }
    }

    // Optional: Show window for manual control
    [MenuItem("Tools/Weapon Setup (Advanced)")]
    public static void ShowWindow()
    {
        WeaponSetupQuickStart window = GetWindow<WeaponSetupQuickStart>("Weapon Setup Quick Start");
        window.minSize = new Vector2(500, 600);
    }

    private void OnGUI()
    {
        GUILayout.Label("🎯 WEAPON SETUP QUICK START", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (!setupComplete)
        {
            EditorGUILayout.HelpBox(
                "🚀 ONE-CLICK AUTOMATED SETUP\n\n" +
                "This will automatically:\n" +
                "1. Create weapon configurations (Pistol, AR, Sniper)\n" +
                "2. Find all weapons in your scene\n" +
                "3. Upgrade them to the new system\n" +
                "4. Assign correct weapon data\n" +
                "5. Preserve all references\n\n" +
                "⚡ EVERYTHING HAPPENS AUTOMATICALLY!\n" +
                "No manual work required!",
                MessageType.Info
            );

            GUILayout.Space(20);

            // BIG ONE-CLICK BUTTON
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("⚡ ONE-CLICK SETUP - DO EVERYTHING AUTOMATICALLY ⚡", GUILayout.Height(60)))
            {
                RunCompleteSetup();
            }
            GUI.backgroundColor = Color.white;

            GUILayout.Space(20);

            EditorGUILayout.HelpBox(
                "🎮 WHAT YOU'LL GET:\n\n" +
                "• Pistol: Semi-auto, 12 rounds, 1.5s reload\n" +
                "• Assault Rifle: Auto/Burst/Semi, 30 rounds, 2.5s reload\n" +
                "• Sniper: Bolt-action, 5 rounds, 3.5s reload, scope\n\n" +
                "All with realistic damage, fire rates, and mechanics!",
                MessageType.Info
            );

            GUILayout.Space(10);

            GUILayout.Label("📖 MANUAL OPTIONS:", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Open Weapon Setup Wizard (Manual)", GUILayout.Height(25)))
            {
                WeaponSetupWizard.ShowWindow();
            }

            if (GUILayout.Button("Open Weapon Auto-Upgrader (Manual)", GUILayout.Height(25)))
            {
                WeaponAutoUpgrader.ShowWindow();
            }
        }
        else
        {
            // SETUP COMPLETE SCREEN
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.HelpBox(statusMessage, statusType);

            GUILayout.Space(20);

            EditorGUILayout.HelpBox(
                "🎯 CONTROLS:\n\n" +
                "Fire: Left Mouse Button\n" +
                "Aim/Scope: Right Mouse Button (hold)\n" +
                "Reload: R\n" +
                "Toggle Fire Mode: B (Assault Rifle only)\n" +
                "Zoom: Mouse Scroll Wheel",
                MessageType.Info
            );

            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "🔫 WEAPON SPECS:\n\n" +
                "PISTOL:\n" +
                "• Semi-Auto | 35 damage | 12 rounds | 1.5s reload\n\n" +
                "ASSAULT RIFLE:\n" +
                "• Auto/Burst/Semi | 25 damage | 30 rounds | 2.5s reload\n" +
                "• Press B to switch fire modes\n\n" +
                "SNIPER RIFLE:\n" +
                "• Bolt-Action | 100 damage | 5 rounds | 3.5s reload\n" +
                "• Hold Right-Click for scope | Scroll to zoom",
                MessageType.Info
            );

            EditorGUILayout.EndScrollView();

            GUILayout.Space(20);

            GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button("▶️ PRESS PLAY TO TEST WEAPONS", GUILayout.Height(50)))
            {
                EditorApplication.isPlaying = true;
            }
            GUI.backgroundColor = Color.white;

            GUILayout.Space(10);

            if (GUILayout.Button("🔄 Run Setup Again", GUILayout.Height(30)))
            {
                setupComplete = false;
                statusMessage = "";
            }

            GUILayout.Space(5);

            if (GUILayout.Button("📂 Open Weapon Data Folder", GUILayout.Height(30)))
            {
                EditorUtility.RevealInFinder("Assets/WeaponData");
            }

            GUILayout.Space(5);

            if (GUILayout.Button("📄 Open Setup Guide", GUILayout.Height(30)))
            {
                string path = Path.Combine(Application.dataPath, "..", "WEAPON_SETUP_GUIDE.md");
                if (File.Exists(path))
                {
                    Application.OpenURL("file:///" + path);
                }
            }
        }
    }

    private void RunCompleteSetup()
    {
        Debug.Log("<color=cyan>═══════════════════════════════════════════════════════</color>");
        Debug.Log("<color=yellow>🚀 STARTING ONE-CLICK AUTOMATED WEAPON SETUP...</color>");
        Debug.Log("<color=cyan>═══════════════════════════════════════════════════════</color>");

        try
        {
            // STEP 1: Create Weapon Configurations
            Debug.Log("\n<color=cyan>📋 STEP 1: Creating Weapon Configurations...</color>");
            CreateWeaponConfigurations();

            // STEP 2: Find and Upgrade Weapons
            Debug.Log("\n<color=cyan>🔧 STEP 2: Finding and Upgrading Weapons...</color>");
            List<GameObject> weapons = FindWeapons();
            int upgraded = UpgradeWeapons(weapons);

            // STEP 3: Complete!
            Debug.Log("\n<color=green>✅ SETUP COMPLETE!</color>");
            Debug.Log("<color=cyan>═══════════════════════════════════════════════════════</color>");

            setupComplete = true;
            statusType = MessageType.Info;
            statusMessage = 
                "✅ SETUP COMPLETE!\n\n" +
                $"✅ Created 3 weapon configurations\n" +
                $"✅ Found {weapons.Count} weapon(s) in scene\n" +
                $"✅ Upgraded {upgraded} weapon(s) successfully\n\n" +
                "🎮 YOUR WEAPONS ARE READY!\n\n" +
                "Next Steps:\n" +
                "1. Press the 'PRESS PLAY TO TEST WEAPONS' button below\n" +
                "2. Test each weapon (Pistol, AR, Sniper)\n" +
                "3. Try different fire modes on AR (Press B)\n" +
                "4. Test sniper scope (Hold Right-Click)\n\n" +
                "All weapon stats can be customized in Assets/WeaponData/";

            EditorUtility.DisplayDialog(
                "Setup Complete! ✅",
                $"Weapon setup completed successfully!\n\n" +
                $"✅ Created 3 weapon configurations\n" +
                $"✅ Upgraded {upgraded} weapon(s)\n\n" +
                "Press Play to test your weapons!",
                "OK"
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError($"<color=red>❌ Setup failed: {e.Message}</color>");
            setupComplete = true;
            statusType = MessageType.Error;
            statusMessage = 
                "❌ SETUP FAILED\n\n" +
                $"Error: {e.Message}\n\n" +
                "Please try:\n" +
                "1. Click 'Run Setup Again'\n" +
                "2. Or use manual setup tools\n" +
                "3. Check console for details";

            EditorUtility.DisplayDialog(
                "Setup Failed",
                $"An error occurred during setup:\n\n{e.Message}\n\n" +
                "Check the console for details.",
                "OK"
            );
        }
    }

    // ========== STATIC METHODS FOR IMMEDIATE SETUP ==========

    private static void CreateWeaponConfigurationsStatic()
    {
        string weaponDataPath = "Assets/WeaponData";

        // Create folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder(weaponDataPath))
        {
            AssetDatabase.CreateFolder("Assets", "WeaponData");
            Debug.Log($"<color=green>✅ Created folder: {weaponDataPath}</color>");
        }

        // Create Pistol
        CreatePistolStatic(weaponDataPath);

        // Create Assault Rifle
        CreateAssaultRifleStatic(weaponDataPath);

        // Create Sniper Rifle
        CreateSniperRifleStatic(weaponDataPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("<color=green>✅ All weapon configurations created successfully!</color>");
    }

    private static void CreatePistolStatic(string path)
    {
        WeaponData pistol = ScriptableObject.CreateInstance<WeaponData>();
        pistol.weaponName = "Pistol";
        pistol.weaponType = WeaponType.Pistol;
        pistol.fireMode = FireMode.SemiAutomatic;
        pistol.canToggleFireMode = false;
        pistol.damage = 35;
        pistol.headshotMultiplier = 2.5f;
        pistol.roundsPerMinute = 300f;
        pistol.magazineSize = 12;
        pistol.reserveAmmo = 60;
        pistol.infiniteAmmo = false;
        pistol.reloadTime = 1.5f;
        pistol.reloadFullMagazine = true;
        pistol.spread = 1.5f;
        pistol.aimSpreadMultiplier = 0.4f;
        pistol.recoilAmount = 2.0f;
        pistol.effectiveRange = 30f;
        pistol.maxRange = 100f;
        pistol.hasScope = false;

        string assetPath = Path.Combine(path, "Pistol.asset");
        AssetDatabase.CreateAsset(pistol, assetPath);
        Debug.Log($"<color=green>✅ Created: Pistol</color>");
    }

    private static void CreateAssaultRifleStatic(string path)
    {
        WeaponData ar = ScriptableObject.CreateInstance<WeaponData>();
        ar.weaponName = "Assault Rifle";
        ar.weaponType = WeaponType.AssaultRifle;
        ar.fireMode = FireMode.Automatic;
        ar.canToggleFireMode = true;
        ar.availableFireModes = new FireMode[] { FireMode.Automatic, FireMode.Burst, FireMode.SemiAutomatic };
        ar.burstCount = 3;
        ar.burstDelay = 0.1f;
        ar.damage = 25;
        ar.headshotMultiplier = 2.0f;
        ar.roundsPerMinute = 700f;
        ar.magazineSize = 30;
        ar.reserveAmmo = 150;
        ar.infiniteAmmo = false;
        ar.reloadTime = 2.5f;
        ar.reloadFullMagazine = true;
        ar.spread = 0.8f;
        ar.aimSpreadMultiplier = 0.3f;
        ar.recoilAmount = 1.2f;
        ar.effectiveRange = 100f;
        ar.maxRange = 300f;
        ar.hasScope = false;

        string assetPath = Path.Combine(path, "AssaultRifle.asset");
        AssetDatabase.CreateAsset(ar, assetPath);
        Debug.Log($"<color=green>✅ Created: Assault Rifle</color>");
    }

    private static void CreateSniperRifleStatic(string path)
    {
        WeaponData sniper = ScriptableObject.CreateInstance<WeaponData>();
        sniper.weaponName = "Sniper Rifle";
        sniper.weaponType = WeaponType.SniperRifle;
        sniper.fireMode = FireMode.BoltAction;
        sniper.canToggleFireMode = false;
        sniper.damage = 100;
        sniper.headshotMultiplier = 3.0f;
        sniper.roundsPerMinute = 40f;
        sniper.magazineSize = 6;
        sniper.reserveAmmo = 30;
        sniper.infiniteAmmo = false;
        sniper.reloadTime = 3.5f;
        sniper.reloadFullMagazine = true;
        sniper.spread = 0.1f;
        sniper.aimSpreadMultiplier = 0.05f;
        sniper.recoilAmount = 3.5f;
        sniper.effectiveRange = 300f;
        sniper.maxRange = 500f;
        sniper.hasScope = true;
        sniper.scopedFOV = 15f;
        sniper.scopeZoomSpeed = 10f;

        string assetPath = Path.Combine(path, "SniperRifle.asset");
        AssetDatabase.CreateAsset(sniper, assetPath);
        Debug.Log($"<color=green>✅ Created: Sniper Rifle (6 rounds, bolt-action)</color>");
    }

    private static List<GameObject> FindWeaponsStatic()
    {
        List<GameObject> foundWeapons = new List<GameObject>();
        
        // Find weapons with old GunSystem
        GunSystem[] gunSystems = FindObjectsByType<GunSystem>(FindObjectsSortMode.None);
        foreach (var gunSystem in gunSystems)
        {
            if (!foundWeapons.Contains(gunSystem.gameObject))
            {
                foundWeapons.Add(gunSystem.gameObject);
            }
        }

        // Find weapons with AdvancedGunSystem
        AdvancedGunSystem[] advancedSystems = FindObjectsByType<AdvancedGunSystem>(FindObjectsSortMode.None);
        foreach (var advancedSystem in advancedSystems)
        {
            if (!foundWeapons.Contains(advancedSystem.gameObject))
            {
                foundWeapons.Add(advancedSystem.gameObject);
            }
        }

        Debug.Log($"<color=cyan>🔍 Found {foundWeapons.Count} weapon(s)</color>");
        return foundWeapons;
    }

    private static int UpgradeWeaponsStatic(List<GameObject> weapons)
    {
        int upgraded = 0;

        foreach (var weaponObj in weapons)
        {
            if (UpgradeWeaponStatic(weaponObj))
            {
                upgraded++;
            }
        }

        Debug.Log($"<color=green>✅ Successfully upgraded {upgraded} weapon(s)</color>");
        return upgraded;
    }

    private static bool UpgradeWeaponStatic(GameObject weaponObj)
    {
        Undo.RegisterCompleteObjectUndo(weaponObj, "Upgrade Weapon");

        // Check if weapon already has AdvancedGunSystem
        AdvancedGunSystem existingAdvanced = weaponObj.GetComponent<AdvancedGunSystem>();
        GunSystem oldSystem = weaponObj.GetComponent<GunSystem>();

        if (existingAdvanced != null)
        {
            // Weapon already has AdvancedGunSystem, just assign WeaponData
            Debug.Log($"<color=yellow>⚠️ {weaponObj.name}: Already has AdvancedGunSystem, assigning WeaponData...</color>");
            
            bool isSniperWeapon = weaponObj.name.ToLower().Contains("sniper");
            string dataPath = DetermineWeaponDataPathStatic(weaponObj.name, isSniperWeapon);
            WeaponData data = AssetDatabase.LoadAssetAtPath<WeaponData>(dataPath);

            if (data != null)
            {
                existingAdvanced.weaponData = data;
                EditorUtility.SetDirty(weaponObj);
                Debug.Log($"<color=green>✅ {weaponObj.name}: Assigned {data.weaponName} data</color>");
                return true;
            }
            else
            {
                Debug.LogError($"<color=red>❌ {weaponObj.name}: WeaponData not found at {dataPath}</color>");
                return false;
            }
        }

        if (oldSystem == null)
        {
            Debug.LogWarning($"<color=yellow>⚠️ {weaponObj.name}: No GunSystem or AdvancedGunSystem found</color>");
            return false;
        }

        // Store references from old system
        bool isSniper = oldSystem.isSniper;
        Camera fpsCamera = oldSystem.fpsCamera;
        Transform attackPoint = oldSystem.attackPoint;
        GameObject bulletPrefab = oldSystem.bulletPrefab;
        AudioSource audioSource = oldSystem.audioSource;
        ParticleSystem muzzleFlash = oldSystem.muzzleFlash;
        GameObject scopeOverlay = oldSystem.scopeOverlay;
        GameObject weaponModel = oldSystem.weaponModel;
        TMPro.TextMeshProUGUI text_ammo = oldSystem.text_ammo;

        // Remove old component
        Object.DestroyImmediate(oldSystem);

        // Add new component
        AdvancedGunSystem newSystem = weaponObj.AddComponent<AdvancedGunSystem>();

        // Restore references
        newSystem.fpsCamera = fpsCamera;
        newSystem.attackPoint = attackPoint;
        newSystem.bulletPrefab = bulletPrefab;
        newSystem.audioSource = audioSource;
        newSystem.muzzleFlash = muzzleFlash;
        newSystem.scopeOverlay = scopeOverlay;
        newSystem.weaponModel = weaponModel;
        newSystem.text_ammo = text_ammo;

        // Assign WeaponData
        string weaponDataPath = DetermineWeaponDataPathStatic(weaponObj.name, isSniper);
        WeaponData weaponData = AssetDatabase.LoadAssetAtPath<WeaponData>(weaponDataPath);

        if (weaponData != null)
        {
            newSystem.weaponData = weaponData;
            Debug.Log($"<color=green>✅ {weaponObj.name}: Upgraded with {weaponData.weaponName} data</color>");
        }
        else
        {
            Debug.LogError($"<color=red>❌ {weaponObj.name}: WeaponData not found at {weaponDataPath}</color>");
        }

        EditorUtility.SetDirty(weaponObj);
        return true;
    }

    private static string DetermineWeaponDataPathStatic(string objectName, bool isSniper)
    {
        string basePath = "Assets/WeaponData/";
        string name = objectName.ToLower();

        if (isSniper || name.Contains("sniper"))
        {
            return basePath + "SniperRifle.asset";
        }
        else if (name.Contains("rifle") || name.Contains("ar") || name.Contains("assault"))
        {
            return basePath + "AssaultRifle.asset";
        }
        else if (name.Contains("pistol") || name.Contains("handgun"))
        {
            return basePath + "Pistol.asset";
        }

        return basePath + "AssaultRifle.asset";
    }

    // ========== INSTANCE METHODS FOR WINDOW (OPTIONAL) ==========

    private void CreateWeaponConfigurations()
    {
        string weaponDataPath = "Assets/WeaponData";

        // Create folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder(weaponDataPath))
        {
            AssetDatabase.CreateFolder("Assets", "WeaponData");
            Debug.Log($"<color=green>✅ Created folder: {weaponDataPath}</color>");
        }

        // Create Pistol
        CreatePistol(weaponDataPath);

        // Create Assault Rifle
        CreateAssaultRifle(weaponDataPath);

        // Create Sniper Rifle
        CreateSniperRifle(weaponDataPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("<color=green>✅ All weapon configurations created successfully!</color>");
    }

    private void CreatePistol(string path)
    {
        WeaponData pistol = ScriptableObject.CreateInstance<WeaponData>();
        pistol.weaponName = "Pistol";
        pistol.weaponType = WeaponType.Pistol;
        pistol.fireMode = FireMode.SemiAutomatic;
        pistol.canToggleFireMode = false;
        pistol.damage = 35;
        pistol.headshotMultiplier = 2.5f;
        pistol.roundsPerMinute = 300f;
        pistol.magazineSize = 12;
        pistol.reserveAmmo = 60;
        pistol.infiniteAmmo = false;
        pistol.reloadTime = 1.5f;
        pistol.reloadFullMagazine = true;
        pistol.spread = 1.5f;
        pistol.aimSpreadMultiplier = 0.4f;
        pistol.recoilAmount = 2.0f;
        pistol.effectiveRange = 30f;
        pistol.maxRange = 100f;
        pistol.hasScope = false;

        string assetPath = Path.Combine(path, "Pistol.asset");
        AssetDatabase.CreateAsset(pistol, assetPath);
        Debug.Log($"<color=green>✅ Created: Pistol</color>");
    }

    private void CreateAssaultRifle(string path)
    {
        WeaponData ar = ScriptableObject.CreateInstance<WeaponData>();
        ar.weaponName = "Assault Rifle";
        ar.weaponType = WeaponType.AssaultRifle;
        ar.fireMode = FireMode.Automatic;
        ar.canToggleFireMode = true;
        ar.availableFireModes = new FireMode[] { FireMode.Automatic, FireMode.Burst, FireMode.SemiAutomatic };
        ar.burstCount = 3;
        ar.burstDelay = 0.1f;
        ar.damage = 25;
        ar.headshotMultiplier = 2.0f;
        ar.roundsPerMinute = 700f;
        ar.magazineSize = 30;
        ar.reserveAmmo = 150;
        ar.infiniteAmmo = false;
        ar.reloadTime = 2.5f;
        ar.reloadFullMagazine = true;
        ar.spread = 0.8f;
        ar.aimSpreadMultiplier = 0.3f;
        ar.recoilAmount = 1.2f;
        ar.effectiveRange = 100f;
        ar.maxRange = 300f;
        ar.hasScope = false;

        string assetPath = Path.Combine(path, "AssaultRifle.asset");
        AssetDatabase.CreateAsset(ar, assetPath);
        Debug.Log($"<color=green>✅ Created: Assault Rifle</color>");
    }

    private void CreateSniperRifle(string path)
    {
        WeaponData sniper = ScriptableObject.CreateInstance<WeaponData>();
        sniper.weaponName = "Sniper Rifle";
        sniper.weaponType = WeaponType.SniperRifle;
        sniper.fireMode = FireMode.BoltAction;
        sniper.canToggleFireMode = false;
        sniper.damage = 100;
        sniper.headshotMultiplier = 3.0f;
        sniper.roundsPerMinute = 40f; // Slow fire rate for bolt-action
        sniper.magazineSize = 6; // 6 bullets per magazine (realistic)
        sniper.reserveAmmo = 30; // 5 extra magazines (6x5=30)
        sniper.infiniteAmmo = false;
        sniper.reloadTime = 3.5f; // Slow reload time
        sniper.reloadFullMagazine = true;
        sniper.spread = 0.1f;
        sniper.aimSpreadMultiplier = 0.05f;
        sniper.recoilAmount = 3.5f;
        sniper.effectiveRange = 300f;
        sniper.maxRange = 500f;
        sniper.hasScope = true;
        sniper.scopedFOV = 15f;
        sniper.scopeZoomSpeed = 10f;

        string assetPath = Path.Combine(path, "SniperRifle.asset");
        AssetDatabase.CreateAsset(sniper, assetPath);
        Debug.Log($"<color=green>✅ Created: Sniper Rifle (6 rounds, bolt-action)</color>");
    }

    private List<GameObject> FindWeapons()
    {
        List<GameObject> foundWeapons = new List<GameObject>();
        
        // Find weapons with old GunSystem
        GunSystem[] gunSystems = FindObjectsByType<GunSystem>(FindObjectsSortMode.None);
        foreach (var gunSystem in gunSystems)
        {
            if (!foundWeapons.Contains(gunSystem.gameObject))
            {
                foundWeapons.Add(gunSystem.gameObject);
            }
        }

        // Find weapons with AdvancedGunSystem (might need WeaponData assigned)
        AdvancedGunSystem[] advancedSystems = FindObjectsByType<AdvancedGunSystem>(FindObjectsSortMode.None);
        foreach (var advancedSystem in advancedSystems)
        {
            if (!foundWeapons.Contains(advancedSystem.gameObject))
            {
                foundWeapons.Add(advancedSystem.gameObject);
            }
        }

        Debug.Log($"<color=cyan>🔍 Found {foundWeapons.Count} weapon(s) (GunSystem + AdvancedGunSystem)</color>");
        return foundWeapons;
    }

    private int UpgradeWeapons(List<GameObject> weapons)
    {
        int upgraded = 0;

        foreach (var weaponObj in weapons)
        {
            if (UpgradeWeapon(weaponObj))
            {
                upgraded++;
            }
        }

        Debug.Log($"<color=green>✅ Successfully upgraded {upgraded} weapon(s)</color>");
        return upgraded;
    }

    private bool UpgradeWeapon(GameObject weaponObj)
    {
        Undo.RegisterCompleteObjectUndo(weaponObj, "Upgrade Weapon");

        // Check if weapon already has AdvancedGunSystem
        AdvancedGunSystem existingAdvanced = weaponObj.GetComponent<AdvancedGunSystem>();
        GunSystem oldSystem = weaponObj.GetComponent<GunSystem>();

        if (existingAdvanced != null)
        {
            // Weapon already has AdvancedGunSystem, just assign WeaponData
            Debug.Log($"<color=yellow>⚠️ {weaponObj.name}: Already has AdvancedGunSystem, assigning WeaponData...</color>");
            
            // Determine weapon type from name or existing settings
            bool isSniperWeapon = weaponObj.name.ToLower().Contains("sniper");
            string dataPath = DetermineWeaponDataPath(weaponObj.name, isSniperWeapon);
            WeaponData data = AssetDatabase.LoadAssetAtPath<WeaponData>(dataPath);

            if (data != null)
            {
                existingAdvanced.weaponData = data;
                EditorUtility.SetDirty(weaponObj);
                Debug.Log($"<color=green>✅ {weaponObj.name}: Assigned {data.weaponName} data</color>");
                return true;
            }
            else
            {
                Debug.LogError($"<color=red>❌ {weaponObj.name}: WeaponData not found at {dataPath}</color>");
                return false;
            }
        }

        if (oldSystem == null)
        {
            Debug.LogWarning($"<color=yellow>⚠️ {weaponObj.name}: No GunSystem or AdvancedGunSystem found</color>");
            return false;
        }

        // Store references from old system
        bool isSniper = oldSystem.isSniper;
        Camera fpsCamera = oldSystem.fpsCamera;
        Transform attackPoint = oldSystem.attackPoint;
        GameObject bulletPrefab = oldSystem.bulletPrefab;
        AudioSource audioSource = oldSystem.audioSource;
        ParticleSystem muzzleFlash = oldSystem.muzzleFlash;
        GameObject scopeOverlay = oldSystem.scopeOverlay;
        GameObject weaponModel = oldSystem.weaponModel;
        TMPro.TextMeshProUGUI text_ammo = oldSystem.text_ammo;

        // Remove old component
        DestroyImmediate(oldSystem);

        // Add new component
        AdvancedGunSystem newSystem = weaponObj.AddComponent<AdvancedGunSystem>();

        // Restore references
        newSystem.fpsCamera = fpsCamera;
        newSystem.attackPoint = attackPoint;
        newSystem.bulletPrefab = bulletPrefab;
        newSystem.audioSource = audioSource;
        newSystem.muzzleFlash = muzzleFlash;
        newSystem.scopeOverlay = scopeOverlay;
        newSystem.weaponModel = weaponModel;
        newSystem.text_ammo = text_ammo;

        // Assign WeaponData
        string weaponDataPath = DetermineWeaponDataPath(weaponObj.name, isSniper);
        WeaponData weaponData = AssetDatabase.LoadAssetAtPath<WeaponData>(weaponDataPath);

        if (weaponData != null)
        {
            newSystem.weaponData = weaponData;
            Debug.Log($"<color=green>✅ {weaponObj.name}: Upgraded with {weaponData.weaponName} data</color>");
        }
        else
        {
            Debug.LogError($"<color=red>❌ {weaponObj.name}: WeaponData not found at {weaponDataPath}. Please assign manually!</color>");
        }

        EditorUtility.SetDirty(weaponObj);
        return true;
    }

    private string DetermineWeaponDataPath(string objectName, bool isSniper)
    {
        string basePath = "Assets/WeaponData/";
        string name = objectName.ToLower();

        if (isSniper || name.Contains("sniper"))
        {
            return basePath + "SniperRifle.asset";
        }
        else if (name.Contains("rifle") || name.Contains("ar") || name.Contains("assault"))
        {
            return basePath + "AssaultRifle.asset";
        }
        else if (name.Contains("pistol") || name.Contains("handgun"))
        {
            return basePath + "Pistol.asset";
        }

        return basePath + "AssaultRifle.asset";
    }
}
