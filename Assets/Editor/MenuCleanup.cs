using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class MenuCleanup : EditorWindow
{
    [MenuItem("Tools/🧹 CLEANUP DUPLICATE UI")]
    public static void CleanupDuplicates()
    {
        string scenePath = FindScenePath("MainMenu");
        if (string.IsNullOrEmpty(scenePath))
        {
            EditorUtility.DisplayDialog("Error", "MainMenu scene not found!", "OK");
            return;
        }

        Scene scene = EditorSceneManager.OpenScene(scenePath);
        
        int removed = 0;

        // 1. Remove duplicate standalone text objects
        removed += RemoveDuplicateTexts();

        // 2. Remove duplicate buttons (smart version - keeps the most visible one)
        removed += RemoveDuplicateButtonsSmart();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        EditorUtility.DisplayDialog("Cleanup Complete!", 
            $"Removed {removed} duplicate object(s).\n\n" +
            "Your menu should now be clean!", "OK");
    }

    private static int RemoveDuplicateTexts()
    {
        int removed = 0;
        Text[] allTexts = FindObjectsByType<Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        TextMeshProUGUI[] allTMPs = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        string[] weaponNames = { "Blaster", "Rifle", "Pistol", "Snipper", "Sniper", "SMG", "Rocket" };

        foreach (Text txt in allTexts)
        {
            if (txt == null) continue;
            if (txt.GetComponentInParent<Button>() != null) continue;

            foreach (string weaponName in weaponNames)
            {
                if (txt.text.IndexOf(weaponName, System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Debug.Log($"<color=red>Removing duplicate Text: {txt.gameObject.name} ('{txt.text}')</color>");
                    DestroyImmediate(txt.gameObject);
                    removed++;
                    break;
                }
            }
        }

        foreach (TextMeshProUGUI tmp in allTMPs)
        {
            if (tmp == null) continue;
            if (tmp.GetComponentInParent<Button>() != null) continue;

            foreach (string weaponName in weaponNames)
            {
                if (tmp.text.IndexOf(weaponName, System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Debug.Log($"<color=red>Removing duplicate TMP: {tmp.gameObject.name} ('{tmp.text}')</color>");
                    DestroyImmediate(tmp.gameObject);
                    removed++;
                    break;
                }
            }
        }

        return removed;
    }

    private static int RemoveDuplicateButtonsSmart()
    {
        int removed = 0;
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        // Group buttons by their text
        Dictionary<string, List<Button>> buttonGroups = new Dictionary<string, List<Button>>();

        foreach (Button btn in allButtons)
        {
            if (btn == null) continue;

            // Skip non-weapon buttons
            if (btn.name.Contains("PLAY") || btn.name.Contains("QUIT") || 
                btn.name.Contains("Play") || btn.name.Contains("Quit"))
                continue;

            string buttonText = GetButtonText(btn);
            if (string.IsNullOrEmpty(buttonText)) continue;

            string normalizedText = buttonText.Trim().ToLower();

            if (!buttonGroups.ContainsKey(normalizedText))
            {
                buttonGroups[normalizedText] = new List<Button>();
            }
            buttonGroups[normalizedText].Add(btn);
        }

        // For each group, keep the best button and remove the rest
        foreach (var group in buttonGroups)
        {
            if (group.Value.Count <= 1) continue; // No duplicates

            // Sort by priority:
            // 1. Active buttons first
            // 2. Then by Y position (higher Y = lower on screen = better positioned)
            var sorted = group.Value.OrderByDescending(b => b.gameObject.activeInHierarchy ? 1 : 0)
                                     .ThenByDescending(b => GetYPosition(b))
                                     .ToList();

            // Keep the first (best) button
            Button keepButton = sorted[0];
            Debug.Log($"<color=green>✅ Keeping button: {keepButton.name} ('{group.Key}') at Y={GetYPosition(keepButton)}</color>");

            // Remove all others
            for (int i = 1; i < sorted.Count; i++)
            {
                Debug.Log($"<color=red>❌ Removing duplicate button: {sorted[i].name} ('{group.Key}') at Y={GetYPosition(sorted[i])}</color>");
                DestroyImmediate(sorted[i].gameObject);
                removed++;
            }
        }

        return removed;
    }

    private static float GetYPosition(Button btn)
    {
        RectTransform rect = btn.GetComponent<RectTransform>();
        if (rect != null)
        {
            return rect.anchoredPosition.y;
        }
        return btn.transform.position.y;
    }

    private static string GetButtonText(Button btn)
    {
        Text legacy = btn.GetComponentInChildren<Text>();
        if (legacy != null) return legacy.text;

        TextMeshProUGUI tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) return tmp.text;

        return "";
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
