using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Weapon Auto-Upgrader - Automatically converts old GunSystem to new AdvancedGunSystem
/// This tool finds all weapons in the scene and upgrades them automatically
/// </summary>
public class WeaponAutoUpgrader : EditorWindow
{
    private List<GameObject> foundWeapons = new List<GameObject>();
    private Vector2 scrollPosition;

    [MenuItem("Tools/Weapon Auto-Upgrader")]
    public static void ShowWindow()
    {
        GetWindow<WeaponAutoUpgrader>("Weapon Auto-Upgrader");
    }

    private void OnGUI()
    {
        GUILayout.Label("🔧 WEAPON AUTO-UPGRADER", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "This tool will:\n" +
            "1. Find all GameObjects with GunSystem component\n" +
            "2. Replace GunSystem with AdvancedGunSystem\n" +
            "3. Preserve all references (camera, attack point, etc.)\n" +
            "4. Assign the appropriate WeaponData asset\n\n" +
            "⚠️ Make sure you've run the Weapon Setup Wizard first!",
            MessageType.Info
        );

        GUILayout.Space(10);

        if (GUILayout.Button("🔍 Scan for Weapons", GUILayout.Height(30)))
        {
            ScanForWeapons();
        }

        GUILayout.Space(10);

        if (foundWeapons.Count > 0)
        {
            GUILayout.Label($"Found {foundWeapons.Count} weapon(s):", EditorStyles.boldLabel);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
            
            foreach (var weapon in foundWeapons)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(weapon, typeof(GameObject), true);
                
                GunSystem gunSystem = weapon.GetComponent<GunSystem>();
                if (gunSystem != null)
                {
                    string weaponType = gunSystem.isSniper ? "Sniper" : "Unknown";
                    GUILayout.Label($"Type: {weaponType}", GUILayout.Width(100));
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();

            GUILayout.Space(10);

            if (GUILayout.Button("⚡ AUTO-UPGRADE ALL WEAPONS", GUILayout.Height(40)))
            {
                if (EditorUtility.DisplayDialog(
                    "Confirm Upgrade",
                    $"This will upgrade {foundWeapons.Count} weapon(s).\n\n" +
                    "The old GunSystem component will be removed and replaced with AdvancedGunSystem.\n\n" +
                    "This action can be undone with Ctrl+Z.\n\n" +
                    "Continue?",
                    "Yes, Upgrade",
                    "Cancel"))
                {
                    UpgradeAllWeapons();
                }
            }

            GUILayout.Space(5);

            if (GUILayout.Button("Clear Results", GUILayout.Height(25)))
            {
                foundWeapons.Clear();
            }
        }
        else
        {
            EditorGUILayout.HelpBox(
                "No weapons found. Click 'Scan for Weapons' to search the scene.",
                MessageType.Info
            );
        }

        GUILayout.Space(20);

        GUILayout.Label("Manual Upgrade Instructions:", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "If auto-upgrade doesn't work, manually:\n\n" +
            "1. Select weapon GameObject\n" +
            "2. Remove 'GunSystem' component\n" +
            "3. Add 'AdvancedGunSystem' component\n" +
            "4. Drag WeaponData asset to 'Weapon Data' field\n" +
            "5. Assign all references (same as before)",
            MessageType.Info
        );
    }

    private void ScanForWeapons()
    {
        foundWeapons.Clear();

        // Find all GunSystem components in the scene
        GunSystem[] gunSystems = FindObjectsByType<GunSystem>(FindObjectsSortMode.None);
        
        foreach (var gunSystem in gunSystems)
        {
            foundWeapons.Add(gunSystem.gameObject);
        }

        Debug.Log($"<color=cyan>🔍 Found {foundWeapons.Count} weapon(s) with GunSystem component</color>");
    }

    private void UpgradeAllWeapons()
    {
        int upgraded = 0;
        int failed = 0;

        foreach (var weaponObj in foundWeapons)
        {
            if (UpgradeWeapon(weaponObj))
            {
                upgraded++;
            }
            else
            {
                failed++;
            }
        }

        EditorUtility.DisplayDialog(
            "Upgrade Complete",
            $"✅ Successfully upgraded: {upgraded}\n" +
            $"❌ Failed: {failed}\n\n" +
            "Check the console for details.",
            "OK"
        );

        foundWeapons.Clear();
    }

    private bool UpgradeWeapon(GameObject weaponObj)
    {
        Undo.RegisterCompleteObjectUndo(weaponObj, "Upgrade Weapon");

        GunSystem oldSystem = weaponObj.GetComponent<GunSystem>();
        if (oldSystem == null)
        {
            Debug.LogWarning($"⚠️ {weaponObj.name}: No GunSystem found");
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

        // Assign appropriate WeaponData
        string weaponDataPath = DetermineWeaponDataPath(weaponObj.name, isSniper);
        WeaponData weaponData = AssetDatabase.LoadAssetAtPath<WeaponData>(weaponDataPath);

        if (weaponData != null)
        {
            newSystem.weaponData = weaponData;
            Debug.Log($"<color=green>✅ {weaponObj.name}: Upgraded successfully with {weaponData.weaponName} data</color>");
        }
        else
        {
            Debug.LogWarning($"<color=yellow>⚠️ {weaponObj.name}: Upgraded but WeaponData not found at {weaponDataPath}. Please assign manually.</color>");
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

        // Default to assault rifle
        return basePath + "AssaultRifle.asset";
    }
}
