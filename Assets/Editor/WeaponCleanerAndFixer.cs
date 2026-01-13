using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class WeaponCleanerAndFixer : EditorWindow
{
    [MenuItem("Tools/🧹 CLEAN DUPLICATE WEAPONS & FIX ROCKET")]
    public static void CleanAndFix()
    {
        string scenePath = FindScenePath("MainMenu");
        if (string.IsNullOrEmpty(scenePath)) return;

        Scene scene = EditorSceneManager.OpenScene(scenePath);
        MainMenu menu = FindAnyObjectByType<MainMenu>();

        if (menu == null)
        {
            Debug.LogError("MainMenu script not found!");
            return;
        }

        // --- STEP 1: CLEAN DUPLICATES ---
        CleanDuplicates(menu, "Rocket Launcher");
        CleanDuplicates(menu, "blaster-n");

        // --- STEP 2: FIX ROCKET POSITION & TEXTURE ---
        FixRocket(menu);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        EditorUtility.DisplayDialog("Cleaned & Fixed", "Removed duplicates and fixed Rocket Launcher!", "Great");
    }

    private static void CleanDuplicates(MainMenu menu, string partialName)
    {
        // specific list of weapons in the menu script array
        List<GameObject> keptWeapons = new List<GameObject>();
        
        // Find all children in the weapon holder (assuming they are children of the first weapon's parent)
        if (menu.menuGuns.Length > 0 && menu.menuGuns[0] != null)
        {
            Transform holder = menu.menuGuns[0].transform.parent;
            List<GameObject> duplicates = new List<GameObject>();

            // Collect all objects matching the name
            for (int i = 0; i < holder.childCount; i++)
            {
                Transform child = holder.GetChild(i);
                if (child.name.Contains(partialName))
                {
                    duplicates.Add(child.gameObject);
                }
            }

            // If we have duplicates, keep only ONE (the last one usually has the latest fixes, or the first one?)
            // Let's keep the LAST created one (highest index) as it's likely the newest spawn
            if (duplicates.Count > 1)
            {
                // Sort by sibling index just to be sure
                duplicates = duplicates.OrderBy(d => d.transform.GetSiblingIndex()).ToList();
                
                // Keep the LAST one
                GameObject toKeep = duplicates[duplicates.Count - 1];
                
                Debug.Log($"<color=yellow>Found {duplicates.Count} copies of {partialName}. Keeping {toKeep.name} (Index {toKeep.transform.GetSiblingIndex()})</color>");

                for (int i = 0; i < duplicates.Count - 1; i++) // Destroy all except last
                {
                    GameObject dup = duplicates[i];
                    Debug.Log($"<color=red>Destroying duplicate: {dup.name}</color>");
                    DestroyImmediate(dup);
                }
            }
        }
    }

    private static void FixRocket(MainMenu menu)
    {
        GameObject rocket = null;
        
        // Re-find the rocket after cleanup
        Transform holder = menu.menuGuns[0].transform.parent;
        for(int i=0; i<holder.childCount; i++) {
            if(holder.GetChild(i).name.Contains("Rocket Launcher")) {
                rocket = holder.GetChild(i).gameObject;
                break;
            }
        }

        if (rocket == null) return;

        // Apply Position/Rotation/Scale
        rocket.transform.localPosition = new Vector3(0.14f, -0.43f, 0.14f);
        rocket.transform.localRotation = Quaternion.Euler(-92.12f, 256.98f, -0.85f);
        rocket.transform.localScale = new Vector3(0.93f, 1.55f, 1.32f);

        // Apply Texture
        string texturePath = "Assets/BigRookGames/_AssetPacks/Stylized Weapon Pack/Stylized Rocket Launcher/Textures/rocket_launcher_Albedo.png";
        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        
        if (tex != null)
        {
            Renderer[] renderers = rocket.GetComponentsInChildren<Renderer>(true);
            foreach (var rend in renderers)
            {
                if (rend.sharedMaterial != null)
                {
                    rend.sharedMaterial.mainTexture = tex; // Use sharedMaterial to ensure it sticks
                }
            }
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
