using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Events;

public class MenuButtonRepairer : EditorWindow
{
    [MenuItem("Tools/🔧 REPAIR ALL MENU BUTTONS")]
    public static void RepairAllButtons()
    {
        string scenePath = FindScenePath("MainMenu");
        if (string.IsNullOrEmpty(scenePath))
        {
            EditorUtility.DisplayDialog("Error", "MainMenu scene not found!", "OK");
            return;
        }

        Scene scene = EditorSceneManager.OpenScene(scenePath);
        MainMenu menu = FindAnyObjectByType<MainMenu>();
        
        if (menu == null)
        {
            EditorUtility.DisplayDialog("Error", "MainMenu script not found!", "OK");
            return;
        }

        int repaired = 0;

        // Define button mappings
        string[] buttonNames = { "Pistol", "Rifle", "Snipper", "Sniper", "SMG", "Rocket" };
        int[] weaponIndices = { 0, 1, 2, 2, 3, 4 }; // Snipper and Sniper both map to index 2

        for (int i = 0; i < buttonNames.Length; i++)
        {
            Button btn = FindButtonWithText(buttonNames[i]);
            if (btn != null)
            {
                RepairButton(btn, buttonNames[i], weaponIndices[i], menu);
                repaired++;
            }
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        EditorUtility.DisplayDialog("Success!", 
            $"Repaired {repaired} button(s)!\n\n" +
            "All weapon buttons should now work correctly.", "OK");
    }

    private static void RepairButton(Button btn, string name, int weaponIndex, MainMenu menuScript)
    {
        // Clear ALL existing listeners
        while (btn.onClick.GetPersistentEventCount() > 0)
        {
            UnityEventTools.RemovePersistentListener(btn.onClick, 0);
        }

        // Add the correct listener
        UnityEventTools.AddIntPersistentListener(btn.onClick, menuScript.SelectWeapon, weaponIndex);
        
        EditorUtility.SetDirty(btn);
        
        Debug.Log($"<color=green>✅ Repaired button: {name} -> SelectWeapon({weaponIndex})</color>");
    }

    private static Button FindButtonWithText(string textContent)
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (var btn in buttons)
        {
            Text legacy = btn.GetComponentInChildren<Text>();
            if (legacy != null && legacy.text.IndexOf(textContent, System.StringComparison.OrdinalIgnoreCase) >= 0)
                return btn;

            TextMeshProUGUI tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null && tmp.text.IndexOf(textContent, System.StringComparison.OrdinalIgnoreCase) >= 0)
                return btn;
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
