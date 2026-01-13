using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class RocketBeautifier : EditorWindow
{
    [MenuItem("Tools/🚀 BEAUTIFY ROCKET (Apply Texture)")]
    public static void BeautifyRocket()
    {
        string scenePath = FindScenePath("MainMenu");
        if (string.IsNullOrEmpty(scenePath)) return;

        Scene scene = EditorSceneManager.OpenScene(scenePath);
        
        // Find the Rocket Launcher
        GameObject rocket = FindRocketInScene();
        if (rocket == null)
        {
            EditorUtility.DisplayDialog("Error", "Rocket Launcher not found!", "OK");
            return;
        }

        // 1. Find a reference weapon that works (Pistol) to steal its Shader
        GameObject pistol = GameObject.Find("Pistol"); // Try exact name
        if (pistol == null) pistol = GameObject.Find("Pistal"); // Common typo in this project
        
        Shader targetShader = Shader.Find("Standard"); // Fallback
        if (pistol != null)
        {
            Renderer pistolRend = pistol.GetComponentInChildren<Renderer>();
            if (pistolRend != null && pistolRend.sharedMaterial != null)
            {
                targetShader = pistolRend.sharedMaterial.shader;
                Debug.Log($"<color=cyan>✅ Stealing Shader '{targetShader.name}' from Pistol</color>");
            }
        }
        else
        {
             // Try searching for any blaster
             GameObject blaster = GameObject.Find("blaster-n");
             if (blaster != null)
             {
                 Renderer bRend = blaster.GetComponentInChildren<Renderer>();
                 if (bRend != null && bRend.sharedMaterial != null)
                 {
                     targetShader = bRend.sharedMaterial.shader;
                     Debug.Log($"<color=cyan>✅ Stealing Shader '{targetShader.name}' from Blaster</color>");
                 }
             }
        }

        // 2. Load Texture
        string texPath = "Assets/BigRookGames/_AssetPacks/Stylized Weapon Pack/Stylized Rocket Launcher/Textures/rocket_launcher_Albedo.png";
        Texture2D albedo = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);

        if (albedo == null)
        {
            // Second attempt: Search by name
            string[] results = AssetDatabase.FindAssets("rocket_launcher_Albedo t:Texture2D");
            if (results.Length > 0)
            {
                texPath = AssetDatabase.GUIDToAssetPath(results[0]);
                albedo = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
            }
        }

        if (albedo == null)
        {
            EditorUtility.DisplayDialog("Error", "Texture NOT found!", "OK");
            return;
        }

        // 3. Apply to ALL Renderers using the Safe Shader
        Renderer[] renderers = rocket.GetComponentsInChildren<Renderer>(true);
        foreach (var rend in renderers)
        {
            Material[] mats = rend.sharedMaterials;
            for (int i = 0; i < mats.Length; i++)
            {
                // Create material with the WORKING shader
                Material newMat = new Material(targetShader); 
                newMat.name = $"Rocket_Fixed_{i}";
                
                // Assign texture to common property names
                if (newMat.HasProperty("_MainTex")) newMat.SetTexture("_MainTex", albedo);
                if (newMat.HasProperty("_BaseMap")) newMat.SetTexture("_BaseMap", albedo); // URP
                if (newMat.HasProperty("_BaseColorMap")) newMat.SetTexture("_BaseColorMap", albedo); // HDRP
                
                newMat.color = Color.white;
                
                mats[i] = newMat;
            }
            rend.materials = mats;
            Debug.Log($"<color=green>✅ Applied Texture with shader: {targetShader.name}</color>");
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        EditorUtility.DisplayDialog("Done", "Rocket Launcher texture applied and beautified! 🚀✨", "Cool");
    }

    private static GameObject FindRocketInScene()
    {
        // Search children of WeaponParent typically
        GameObject[] all = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject go in all)
        {
            if (go.scene.name == null) continue;
            if (go.transform.parent == null) continue;
            
            if (go.name.Contains("Rocket") && go.name.Contains("Launcher"))
            {
                return go;
            }
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
