using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class RocketPositionFixer : EditorWindow
{
    [MenuItem("Tools/🚀 FIX ROCKET LAUNCHER POSITION")]
    public static void FixRocketPosition()
    {
        string scenePath = FindScenePath("MainMenu");
        if (string.IsNullOrEmpty(scenePath))
        {
            EditorUtility.DisplayDialog("Error", "MainMenu scene not found!", "OK");
            return;
        }

        Scene scene = EditorSceneManager.OpenScene(scenePath);
        MainMenu menu = FindAnyObjectByType<MainMenu>();

        if (menu == null || menu.menuGuns == null || menu.menuGuns.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "MainMenu or menuGuns not found!", "OK");
            return;
        }

        // Find Rocket Launcher and a reference weapon (Pistol)
        GameObject rocket = null;
        GameObject pistol = null;

        foreach (var gun in menu.menuGuns)
        {
            if (gun == null) continue;
            
            string gunName = gun.name.ToLower();
            if (gunName.Contains("rocket") || gunName.Contains("launcher"))
            {
                rocket = gun;
            }
            if (gunName.Contains("pistol"))
            {
                pistol = gun;
            }
        }

        if (rocket == null)
        {
            EditorUtility.DisplayDialog("Error", "Rocket Launcher not found in menuGuns array!", "OK");
            return;
        }

        if (pistol == null && menu.menuGuns.Length > 0)
        {
            // Use first weapon as reference if pistol not found
            pistol = menu.menuGuns[0];
        }

        // Apply EXACT user values
        if (rocket != null)
        {
            rocket.transform.localPosition = new Vector3(0.14f, -0.43f, 0.14f);
            rocket.transform.localRotation = Quaternion.Euler(-92.12f, 256.98f, -0.85f);
            rocket.transform.localScale = new Vector3(0.93f, 1.55f, 1.32f);
            
            // --- TEXTURE FIX ---
            string texturePath = "Assets/BigRookGames/_AssetPacks/Stylized Weapon Pack/Stylized Rocket Launcher/Textures/rocket_launcher_Albedo.png";
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            
            if (tex != null)
            {
                Renderer[] renderers = rocket.GetComponentsInChildren<Renderer>(true);
                foreach (var rend in renderers)
                {
                    if (rend.sharedMaterial != null)
                    {
                        // Create a clone to avoid changing the asset on disk for everyone if that's not desired, 
                        // OR just set it if we want to fix the asset. 
                        // Let's set it on the instance material to be safe and immediate for this scene.
                        rend.material.mainTexture = tex;
                        Debug.Log($"<color=green>✅ Applied Texture: {tex.name} to {rend.name}</color>");
                    }
                }
            }
            else
            {
                Debug.LogError($"Could not find texture at {texturePath}");
            }

            Debug.Log($"<color=green>✅ ROCKET PERFECTLY POSITIONED & TEXTURED!</color>");
            Debug.Log($"Pos: {rocket.transform.localPosition}");
            Debug.Log($"Rot: {rocket.transform.localRotation.eulerAngles}");
            Debug.Log($"Scale: {rocket.transform.localScale}");
        }

        EditorUtility.SetDirty(rocket.transform);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        EditorUtility.DisplayDialog("Success!", 
            "Rocket Launcher position has been adjusted!\n\n" +
            "Press Play to see the result.\n" +
            "If it still needs adjustment, you can manually tweak it in the Scene view.", "OK");
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
