using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Weapon Setup Wizard - Automatically creates weapon configurations
/// This tool creates realistic weapon presets for Pistol, Assault Rifle, and Sniper Rifle
/// </summary>
public class WeaponSetupWizard : EditorWindow
{
    private string weaponDataPath = "Assets/WeaponData";
    private bool setupComplete = false;

    [MenuItem("Tools/Weapon Setup Wizard")]
    public static void ShowWindow()
    {
        GetWindow<WeaponSetupWizard>("Weapon Setup Wizard");
    }

    private void OnGUI()
    {
        GUILayout.Label("🔫 WEAPON SETUP WIZARD", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "This wizard will automatically create realistic weapon configurations for:\n\n" +
            "• Pistol (Semi-Auto, 12 rounds, fast reload)\n" +
            "• Assault Rifle (Full-Auto, 30 rounds, medium reload)\n" +
            "• Sniper Rifle (Bolt-Action, 5 rounds, slow reload, scope)",
            MessageType.Info
        );

        GUILayout.Space(10);

        GUILayout.Label("Weapon Data Folder:", EditorStyles.boldLabel);
        weaponDataPath = EditorGUILayout.TextField("Path:", weaponDataPath);

        GUILayout.Space(10);

        if (setupComplete)
        {
            EditorGUILayout.HelpBox(
                "✅ Weapon setup complete!\n\n" +
                "Weapon configurations created at: " + weaponDataPath + "\n\n" +
                "Next Steps:\n" +
                "1. Find your weapon GameObjects in the scene\n" +
                "2. Replace 'GunSystem' with 'AdvancedGunSystem' component\n" +
                "3. Drag the appropriate WeaponData asset to the component\n" +
                "4. Assign references (camera, attack point, etc.)\n" +
                "5. Press Play and test!",
                MessageType.Info
            );

            if (GUILayout.Button("Open Weapon Data Folder", GUILayout.Height(30)))
            {
                EditorUtility.RevealInFinder(weaponDataPath);
            }

            GUILayout.Space(5);

            if (GUILayout.Button("Setup Another Set", GUILayout.Height(30)))
            {
                setupComplete = false;
            }
        }
        else
        {
            if (GUILayout.Button("🚀 CREATE WEAPON CONFIGURATIONS", GUILayout.Height(40)))
            {
                CreateWeaponConfigurations();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Show Weapon Stats Reference", GUILayout.Height(30)))
            {
                ShowWeaponStatsReference();
            }
        }
    }

    private void CreateWeaponConfigurations()
    {
        // Create folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder(weaponDataPath))
        {
            string parentFolder = Path.GetDirectoryName(weaponDataPath);
            string folderName = Path.GetFileName(weaponDataPath);
            
            if (!AssetDatabase.IsValidFolder(parentFolder))
            {
                AssetDatabase.CreateFolder("Assets", folderName);
            }
            else
            {
                AssetDatabase.CreateFolder(parentFolder, folderName);
            }
        }

        Debug.Log("<color=cyan>🔫 Creating weapon configurations...</color>");

        // Create Pistol
        CreatePistol();

        // Create Assault Rifle
        CreateAssaultRifle();

        // Create Sniper Rifle
        CreateSniperRifle();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        setupComplete = true;

        Debug.Log("<color=green>✅ All weapon configurations created successfully!</color>");
    }

    private void CreatePistol()
    {
        WeaponData pistol = ScriptableObject.CreateInstance<WeaponData>();

        // Identity
        pistol.weaponName = "Pistol";
        pistol.weaponType = WeaponType.Pistol;

        // Fire Mode
        pistol.fireMode = FireMode.SemiAutomatic;
        pistol.canToggleFireMode = false;

        // Damage
        pistol.damage = 35; // High damage per shot
        pistol.headshotMultiplier = 2.5f;

        // Fire Rate (realistic pistol RPM)
        pistol.roundsPerMinute = 300f; // Semi-auto, player-limited

        // Magazine & Ammo
        pistol.magazineSize = 12;
        pistol.reserveAmmo = 60; // 5 extra magazines
        pistol.infiniteAmmo = false;

        // Reload
        pistol.reloadTime = 1.5f; // Fast reload
        pistol.reloadFullMagazine = true;

        // Accuracy
        pistol.spread = 1.5f; // Moderate spread
        pistol.aimSpreadMultiplier = 0.4f;
        pistol.recoilAmount = 2.0f; // High recoil per shot

        // Range
        pistol.effectiveRange = 30f;
        pistol.maxRange = 100f;

        // Scope
        pistol.hasScope = false;

        string path = Path.Combine(weaponDataPath, "Pistol.asset");
        AssetDatabase.CreateAsset(pistol, path);
        Debug.Log($"<color=green>✅ Created: {pistol.weaponName}</color>");
    }

    private void CreateAssaultRifle()
    {
        WeaponData ar = ScriptableObject.CreateInstance<WeaponData>();

        // Identity
        ar.weaponName = "Assault Rifle";
        ar.weaponType = WeaponType.AssaultRifle;

        // Fire Mode
        ar.fireMode = FireMode.Automatic;
        ar.canToggleFireMode = true;
        ar.availableFireModes = new FireMode[] { 
            FireMode.Automatic, 
            FireMode.Burst, 
            FireMode.SemiAutomatic 
        };

        // Burst settings
        ar.burstCount = 3;
        ar.burstDelay = 0.1f;

        // Damage
        ar.damage = 25; // Balanced damage
        ar.headshotMultiplier = 2.0f;

        // Fire Rate (realistic AR RPM: 600-900)
        ar.roundsPerMinute = 700f;

        // Magazine & Ammo
        ar.magazineSize = 30;
        ar.reserveAmmo = 150; // 5 extra magazines
        ar.infiniteAmmo = false;

        // Reload
        ar.reloadTime = 2.5f; // Standard reload
        ar.reloadFullMagazine = true;

        // Accuracy
        ar.spread = 0.8f; // Good accuracy
        ar.aimSpreadMultiplier = 0.3f;
        ar.recoilAmount = 1.2f; // Moderate recoil

        // Range
        ar.effectiveRange = 100f;
        ar.maxRange = 300f;

        // Scope
        ar.hasScope = false; // Can add red dot sight later

        string path = Path.Combine(weaponDataPath, "AssaultRifle.asset");
        AssetDatabase.CreateAsset(ar, path);
        Debug.Log($"<color=green>✅ Created: {ar.weaponName}</color>");
    }

    private void CreateSniperRifle()
    {
        WeaponData sniper = ScriptableObject.CreateInstance<WeaponData>();

        // Identity
        sniper.weaponName = "Sniper Rifle";
        sniper.weaponType = WeaponType.SniperRifle;

        // Fire Mode
        sniper.fireMode = FireMode.BoltAction;
        sniper.canToggleFireMode = false;

        // Damage
        sniper.damage = 100; // One-shot kill potential
        sniper.headshotMultiplier = 3.0f; // Instant kill on headshot

        // Fire Rate (bolt-action, slow)
        sniper.roundsPerMinute = 40f; // Very slow, deliberate shots

        // Magazine & Ammo
        sniper.magazineSize = 6; // 6 bullets per magazine (realistic)
        sniper.reserveAmmo = 30; // 5 extra magazines (6x5=30)
        sniper.infiniteAmmo = false;

        // Reload
        sniper.reloadTime = 3.5f; // Slow reload
        sniper.reloadFullMagazine = true;

        // Accuracy
        sniper.spread = 0.1f; // Extremely accurate
        sniper.aimSpreadMultiplier = 0.05f; // Nearly perfect when scoped
        sniper.recoilAmount = 3.5f; // High recoil, but slow fire rate

        // Range
        sniper.effectiveRange = 300f;
        sniper.maxRange = 500f;

        // Scope
        sniper.hasScope = true;
        sniper.scopedFOV = 15f;
        sniper.scopeZoomSpeed = 10f;

        string path = Path.Combine(weaponDataPath, "SniperRifle.asset");
        AssetDatabase.CreateAsset(sniper, path);
        Debug.Log($"<color=green>✅ Created: {sniper.weaponName} (6 rounds, bolt-action)</color>");
    }

    private void ShowWeaponStatsReference()
    {
        string reference = 
            "═══════════════════════════════════════════════════════\n" +
            "                  WEAPON STATS REFERENCE\n" +
            "═══════════════════════════════════════════════════════\n\n" +
            
            "🔫 PISTOL\n" +
            "  • Type: Semi-Automatic\n" +
            "  • Damage: 35 per shot\n" +
            "  • Fire Rate: 300 RPM\n" +
            "  • Magazine: 12 rounds\n" +
            "  • Reserve: 60 rounds (5 mags)\n" +
            "  • Reload: 1.5 seconds\n" +
            "  • Range: 30m effective, 100m max\n" +
            "  • Spread: 1.5 (moderate)\n" +
            "  • Recoil: 2.0 (high per shot)\n" +
            "  • Special: Fast reload, high damage\n\n" +
            
            "🔫 ASSAULT RIFLE\n" +
            "  • Type: Automatic / Burst / Semi\n" +
            "  • Damage: 25 per shot\n" +
            "  • Fire Rate: 700 RPM\n" +
            "  • Magazine: 30 rounds\n" +
            "  • Reserve: 150 rounds (5 mags)\n" +
            "  • Reload: 2.5 seconds\n" +
            "  • Range: 100m effective, 300m max\n" +
            "  • Spread: 0.8 (good accuracy)\n" +
            "  • Recoil: 1.2 (moderate)\n" +
            "  • Burst: 3 rounds, 0.1s delay\n" +
            "  • Special: Versatile, switchable fire modes (Press B)\n\n" +
            
            "🔫 SNIPER RIFLE\n" +
            "  • Type: Bolt-Action\n" +
            "  • Damage: 100 per shot\n" +
            "  • Fire Rate: 40 RPM (very slow)\n" +
            "  • Magazine: 5 rounds\n" +
            "  • Reserve: 25 rounds (5 mags)\n" +
            "  • Reload: 3.5 seconds\n" +
            "  • Range: 300m effective, 500m max\n" +
            "  • Spread: 0.1 (extremely accurate)\n" +
            "  • Recoil: 3.5 (high, but slow fire)\n" +
            "  • Scope: Yes (15° FOV, adjustable)\n" +
            "  • Headshot Multiplier: 3.0x (instant kill)\n" +
            "  • Special: One-shot potential, scope zoom\n\n" +
            
            "═══════════════════════════════════════════════════════\n" +
            "CONTROLS:\n" +
            "  • Fire: Left Mouse Button\n" +
            "  • Aim/Scope: Right Mouse Button (hold)\n" +
            "  • Reload: R\n" +
            "  • Toggle Fire Mode: B (AR only)\n" +
            "  • Scroll Zoom: Mouse Wheel\n" +
            "═══════════════════════════════════════════════════════";

        Debug.Log(reference);
        EditorUtility.DisplayDialog("Weapon Stats Reference", reference, "OK");
    }
}
