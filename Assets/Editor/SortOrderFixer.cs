using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SortOrderFixer : EditorWindow
{
    [MenuItem("Tools/🔢 FIX SORTING ORDER (0 vs 100)")]
    public static void ApplyUserFix()
    {
        string scenePath = FindScenePath("MainMenu");
        if (string.IsNullOrEmpty(scenePath)) return;

        Scene scene = EditorSceneManager.OpenScene(scenePath);

        // 1. Fix BackgroundCanvas
        GameObject bgObj = GameObject.Find("BackgroundCanvas");
        if (bgObj != null)
        {
            Canvas bgCanvas = bgObj.GetComponent<Canvas>();
            if (bgCanvas == null) bgCanvas = bgObj.AddComponent<Canvas>();
            
            // Set to ScreenSpaceCamera so it renders BEHIND 3D objects
            // If we use Overlay, it covers 3D objects irrespective of sort order usually.
            // But let's try to set the plane distance far away if using Camera.
            bgCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            bgCanvas.worldCamera = Camera.main;
            bgCanvas.planeDistance = 1000f; // Push it way back
            bgCanvas.sortingOrder = -100;    // Absolute bottom
            
            Debug.Log($"<color=green>✅ Set BackgroundCanvas to SortOrder -100 (Back)</color>");
        }
        else
        {
            Debug.LogError("Could not find 'BackgroundCanvas'");
        }

        // 2. Fix UI Canvas
        GameObject uiObj = GameObject.Find("Canvas");
        if (uiObj != null)
        {
            Canvas uiCanvas = uiObj.GetComponent<Canvas>();
            if (uiCanvas == null) uiCanvas = uiObj.AddComponent<Canvas>();
            
            uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            uiCanvas.sortingOrder = 100; // Top
            
            Debug.Log($"<color=green>✅ Set UI Canvas to SortOrder 100 (Front)</color>");
        }
        else
        {
            Debug.LogError("Could not find 'Canvas'");
        }
        
        // 3. Ensure EventSystem exists
        if (FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
             new GameObject("EventSystem").AddComponent<UnityEngine.EventSystems.EventSystem>()
                .gameObject.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        EditorUtility.DisplayDialog("Done", "Sorting Orders Updated!", "Great");
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
