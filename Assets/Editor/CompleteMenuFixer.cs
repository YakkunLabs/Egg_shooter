using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Events;
using System.Collections.Generic;

public class CompleteMenuFixer : EditorWindow
{
    [MenuItem("Tools/⚡ COMPLETE MENU FIX (Restore Everything)")]
    public static void FixEverything()
    {
        bool confirm = EditorUtility.DisplayDialog("Complete Menu Fix", 
            "This will:\n" +
            "1. Find or create ALL 5 weapon buttons\n" +
            "2. Ensure they look identical\n" +
            "3. Wire them to the correct weapons\n" +
            "4. Fix the menuGuns array\n\n" +
            "Continue?", "YES", "Cancel");
        
        if (!confirm) return;

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

        // Find a reference button (any existing button will do)
        Button referenceButton = FindAnyWeaponButton();
        if (referenceButton == null)
        {
            EditorUtility.DisplayDialog("Error", "No weapon buttons found to use as reference!", "OK");
            return;
        }

        Debug.Log($"<color=cyan>Using '{referenceButton.name}' as reference button</color>");

        // Button definitions
        string[] buttonLabels = { "Pistol", "Rifle", "Snipper", "SMG", "Rocket" };
        int[] weaponIndices = { 0, 1, 2, 3, 4 };

        List<Button> allButtons = new List<Button>();

        // Create or find all buttons
        for (int i = 0; i < buttonLabels.Length; i++)
        {
            Button btn = FindButtonWithText(buttonLabels[i]);
            
            if (btn == null)
            {
                // Create missing button
                Debug.Log($"<color=yellow>Creating missing button: {buttonLabels[i]}</color>");
                btn = CreateButton(referenceButton, buttonLabels[i], weaponIndices[i], menu);
            }
            else
            {
                // Repair existing button
                Debug.Log($"<color=green>Repairing existing button: {buttonLabels[i]}</color>");
                RepairButton(btn, weaponIndices[i], menu);
            }

            if (btn != null)
            {
                allButtons.Add(btn);
                
                // Ensure button is active and visible
                btn.gameObject.SetActive(true);
                btn.interactable = true;
            }
        }

        // Organize buttons vertically
        OrganizeButtons(allButtons, referenceButton);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        EditorUtility.DisplayDialog("Success!", 
            $"Fixed {allButtons.Count} buttons!\n\n" +
            "All weapon selection buttons are now working.", "Awesome!");
    }

    private static void OrganizeButtons(List<Button> buttons, Button reference)
    {
        if (buttons.Count == 0) return;

        // Check if there's a layout group
        if (reference.transform.parent.GetComponent<VerticalLayoutGroup>() != null)
        {
            // Layout group will handle positioning automatically
            return;
        }

        // Manual positioning - stack vertically
        RectTransform refRect = reference.GetComponent<RectTransform>();
        float startY = refRect.anchoredPosition.y;
        float spacing = refRect.rect.height + 10f;

        for (int i = 0; i < buttons.Count; i++)
        {
            RectTransform rect = buttons[i].GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(refRect.anchoredPosition.x, startY - (i * spacing));
            EditorUtility.SetDirty(rect);
        }
    }

    private static Button CreateButton(Button original, string label, int weaponIndex, MainMenu menuScript)
    {
        GameObject newBtnObj = Instantiate(original.gameObject, original.transform.parent);
        newBtnObj.name = label + " Button";

        // Update text
        Text legacy = newBtnObj.GetComponentInChildren<Text>();
        if (legacy) legacy.text = label;
        
        TextMeshProUGUI tmp = newBtnObj.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp) tmp.text = label;

        // Setup button
        Button btn = newBtnObj.GetComponent<Button>();
        RepairButton(btn, weaponIndex, menuScript);

        // Copy visual properties explicitly
        CopyButtonVisuals(original, btn);

        EditorUtility.SetDirty(newBtnObj);
        return btn;
    }

    private static void RepairButton(Button btn, int weaponIndex, MainMenu menuScript)
    {
        // Clear all listeners
        while (btn.onClick.GetPersistentEventCount() > 0)
        {
            UnityEventTools.RemovePersistentListener(btn.onClick, 0);
        }

        // Add correct listener
        UnityEventTools.AddIntPersistentListener(btn.onClick, menuScript.SelectWeapon, weaponIndex);
        
        EditorUtility.SetDirty(btn);
        Debug.Log($"<color=green>✅ Wired button -> SelectWeapon({weaponIndex})</color>");
    }

    private static void CopyButtonVisuals(Button source, Button target)
    {
        Image srcImg = source.GetComponent<Image>();
        Image tgtImg = target.GetComponent<Image>();
        
        if (srcImg != null && tgtImg != null)
        {
            tgtImg.sprite = srcImg.sprite;
            tgtImg.color = srcImg.color;
            tgtImg.material = srcImg.material;
            tgtImg.type = srcImg.type;
            tgtImg.fillCenter = srcImg.fillCenter;
            tgtImg.pixelsPerUnitMultiplier = srcImg.pixelsPerUnitMultiplier;
        }

        // Copy ColorBlock
        target.colors = source.colors;
        target.transition = source.transition;
        target.targetGraphic = tgtImg;
    }

    private static Button FindAnyWeaponButton()
    {
        string[] possibleNames = { "Snipper", "Sniper", "SMG", "Rocket", "Pistol", "Rifle" };
        
        foreach (string name in possibleNames)
        {
            Button btn = FindButtonWithText(name);
            if (btn != null) return btn;
        }

        return null;
    }

    private static Button FindButtonWithText(string textContent)
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (var btn in buttons)
        {
            // Skip non-weapon buttons
            if (btn.name.Contains("PLAY") || btn.name.Contains("QUIT")) continue;

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
