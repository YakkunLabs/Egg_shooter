using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MoveBackgroundToBack : EditorWindow
{
    [MenuItem("Tools/📐 Move Background Image to Back")]
    public static void MoveBackground()
    {
        string scenePath = FindScenePath("MainMenu");
        if (string.IsNullOrEmpty(scenePath)) return;

        Scene scene = EditorSceneManager.OpenScene(scenePath);
        
        // Find the Canvas
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        
        foreach (Canvas canvas in canvases)
        {
            // Get all children of the canvas
            Transform canvasTransform = canvas.transform;
            
            // Look through all direct children
            for (int i = 0; i < canvasTransform.childCount; i++)
            {
                Transform child = canvasTransform.GetChild(i);
                string childName = child.name.ToLower();
                
                // Check if this looks like a background
                if (childName.Contains("background") || 
                    childName.Contains("bg") || 
                    childName.Contains("image") && child.GetComponent<Image>() != null)
                {
                    // Check if it's a large image
                    RectTransform rect = child.GetComponent<RectTransform>();
                    if (rect != null)
                    {
                        float width = rect.rect.width;
                        float height = rect.rect.height;
                        
                        // If it's large (likely full-screen background)
                        if (width > 500 || height > 500)
                        {
                            // Move to position 0 (first in hierarchy = renders first = behind everything)
                            child.SetSiblingIndex(0);
                            
                            // Disable raycast so it doesn't block clicks
                            Image img = child.GetComponent<Image>();
                            if (img != null) img.raycastTarget = false;
                            
                            RawImage rawImg = child.GetComponent<RawImage>();
                            if (rawImg != null) rawImg.raycastTarget = false;
                            
                            Debug.Log($"<color=green>✅ Moved '{child.name}' to back (size: {width}x{height})</color>");
                        }
                    }
                }
            }
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        EditorUtility.DisplayDialog("Done", 
            "Background image moved to back.\n\nPress Play to test.", "OK");
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
