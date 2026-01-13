using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FixUILayering : EditorWindow
{
    [MenuItem("Tools/🎨 FIX UI LAYERING (Move Background Behind)")]
    public static void FixLayering()
    {
        string scenePath = FindScenePath("MainMenu");
        if (string.IsNullOrEmpty(scenePath)) return;

        Scene scene = EditorSceneManager.OpenScene(scenePath);
        Canvas canvas = FindAnyObjectByType<Canvas>();

        if (canvas == null)
        {
            EditorUtility.DisplayDialog("Error", "Canvas not found!", "OK");
            return;
        }

        // Find all Image components in the Canvas
        Image[] allImages = canvas.GetComponentsInChildren<Image>(true);
        
        Debug.Log($"Found {allImages.Length} images in Canvas");

        // Look for background images (usually large, full-screen images)
        foreach (var img in allImages)
        {
            RectTransform rect = img.GetComponent<RectTransform>();
            
            // Check if this is likely a background (large size, no button component)
            bool isLargeImage = rect.rect.width > 800 || rect.rect.height > 600;
            bool hasNoButton = img.GetComponent<Button>() == null;
            bool hasBackgroundInName = img.name.ToLower().Contains("background") || 
                                       img.name.ToLower().Contains("bg") ||
                                       img.name.ToLower().Contains("canvas");
            
            if ((isLargeImage && hasNoButton) || hasBackgroundInName)
            {
                // Move to the FIRST position in hierarchy (renders first = behind)
                img.transform.SetAsFirstSibling();
                
                // Ensure it doesn't block raycasts
                img.raycastTarget = false;
                
                Debug.Log($"<color=green>✅ Moved '{img.name}' to back (size: {rect.rect.width}x{rect.rect.height})</color>");
            }
        }

        // Also check for raw images
        RawImage[] rawImages = canvas.GetComponentsInChildren<RawImage>(true);
        foreach (var raw in rawImages)
        {
            RectTransform rect = raw.GetComponent<RectTransform>();
            bool isLarge = rect.rect.width > 800 || rect.rect.height > 600;
            
            if (isLarge)
            {
                raw.transform.SetAsFirstSibling();
                raw.raycastTarget = false;
                Debug.Log($"<color=green>✅ Moved RawImage '{raw.name}' to back</color>");
            }
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        EditorUtility.DisplayDialog("Fixed!", 
            "Background images moved behind UI elements.\n\n" +
            "Check the Console for details.", "OK");
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
