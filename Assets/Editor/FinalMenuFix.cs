// using UnityEngine;
// using UnityEditor;
// using UnityEditor.SceneManagement;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;
// using TMPro;
// using UnityEditor.Events;
// using System.Collections.Generic;

// public class FinalMenuFix : EditorWindow
// {
//     [MenuItem("Tools/✨ FINAL MENU FIX (Complete Reset)")]
//     public static void FinalFix()
//     {
//         bool confirm = EditorUtility.DisplayDialog("Final Menu Fix", 
//             "This will completely rebuild the weapon buttons:\n\n" +
//             "1. Remove ALL weapon buttons\n" +
//             "2. Create fresh buttons for all 5 weapons\n" +
//             "3. Position them correctly\n" +
//             "4. Wire them up properly\n" +
//             "5. Preserve skin selection buttons\n\n" +
//             "Continue?", "YES - FIX IT", "Cancel");
        
//         if (!confirm) return;

//         string scenePath = FindScenePath("MainMenu");
//         if (string.IsNullOrEmpty(scenePath))
//         {
//             EditorUtility.DisplayDialog("Error", "MainMenu scene not found!", "OK");
//             return;
//         }

//         Scene scene = EditorSceneManager.OpenScene(scenePath);
//         MainMenu menu = FindAnyObjectByType<MainMenu>();
        
//         if (menu == null)
//         {
//             EditorUtility.DisplayDialog("Error", "MainMenu script not found!", "OK");
//             return;
//         }

//         // Step 1: Find ANY existing weapon button to use as template
//         Button template = FindWeaponButtonTemplate();
//         if (template == null)
//         {
//             EditorUtility.DisplayDialog("Error", "No weapon buttons found to use as template!", "OK");
//             return;
//         }

//         Transform buttonParent = template.transform.parent;
//         RectTransform templateRect = template.GetComponent<RectTransform>();
        
//         // SAVE template properties BEFORE doing anything
//         Vector2 templatePos = templateRect.anchoredPosition;
//         float buttonHeight = templateRect.rect.height;
//         float spacing = 10f;

//         // Step 2: Collect ONLY weapon buttons to delete (preserve skin buttons)
//         List<Button> toDelete = new List<Button>();
//         Button[] allButtons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
//         string[] weaponKeywords = { "pistol", "rifle", "snipper", "sniper", "smg", "rocket" };
        
//         foreach (Button btn in allButtons)
//         {
//             if (btn == null) continue;
            
//             // Skip UI buttons
//             if (btn.name.Contains("PLAY") || btn.name.Contains("QUIT") || 
//                 btn.name.Contains("Play") || btn.name.Contains("Quit")) continue;
            
//             // Skip skin buttons
//             string btnName = btn.name.ToLower();
//             string btnText = GetButtonText(btn).ToLower();
//             if (btnName.Contains("skin") || btnText.Contains("skin")) 
//             {
//                 Debug.Log($"<color=cyan>Preserving skin button: {btn.name}</color>");
//                 continue;
//             }
            
//             // Check if it's a weapon button
//             bool isWeaponButton = false;
//             foreach (string keyword in weaponKeywords)
//             {
//                 if (btnName.Contains(keyword) || btnText.Contains(keyword))
//                 {
//                     isWeaponButton = true;
//                     break;
//                 }
//             }
            
//             if (isWeaponButton)
//             {
//                 toDelete.Add(btn);
//             }
//         }

//         // Step 3: Create all 5 NEW buttons FIRST (while template still exists)
//         string[] labels = { "Pistol", "Rifle", "Snipper", "SMG", "Rocket" };
//         int[] indices = { 0, 1, 2, 3, 4 };

//         List<GameObject> newButtons = new List<GameObject>();

//         for (int i = 0; i < labels.Length; i++)
//         {
//             GameObject btnObj = Instantiate(template.gameObject, buttonParent);
//             btnObj.name = labels[i] + " Button";
//             btnObj.SetActive(true);

//             // Position
//             RectTransform rect = btnObj.GetComponent<RectTransform>();
//             rect.anchoredPosition = new Vector2(templatePos.x, templatePos.y - (i * (buttonHeight + spacing)));

//             // Update text
//             Text legacy = btnObj.GetComponentInChildren<Text>();
//             if (legacy) legacy.text = labels[i];
            
//             TextMeshProUGUI tmp = btnObj.GetComponentInChildren<TextMeshProUGUI>();
//             if (tmp) tmp.text = labels[i];

//             // Wire up button
//             Button btn = btnObj.GetComponent<Button>();
//             while (btn.onClick.GetPersistentEventCount() > 0)
//             {
//                 UnityEventTools.RemovePersistentListener(btn.onClick, 0);
//             }
//             UnityEventTools.AddIntPersistentListener(btn.onClick, menu.SelectWeapon, indices[i]);

//             EditorUtility.SetDirty(btnObj);
//             newButtons.Add(btnObj);
//             Debug.Log($"<color=green>✅ Created: {labels[i]} -> SelectWeapon({indices[i]})</color>");
//         }

//         // Step 4: NOW delete all old WEAPON buttons (not skin buttons)
//         Debug.Log("<color=yellow>Removing old weapon buttons...</color>");
//         foreach (Button btn in toDelete)
//         {
//             if (btn != null && btn.gameObject != null)
//             {
//                 Debug.Log($"Removing: {btn.name}");
//                 DestroyImmediate(btn.gameObject);
//             }
//         }

//         EditorSceneManager.MarkSceneDirty(scene);
//         EditorSceneManager.SaveScene(scene);

//         EditorUtility.DisplayDialog("Success!", 
//             "Menu completely rebuilt!\n\n" +
//             "You now have:\n" +
//             "• 5 weapon buttons (Pistol, Rifle, Snipper, SMG, Rocket)\n" +
//             "• Skin selection buttons preserved", "Awesome!");
//     }

//     private static Button FindWeaponButtonTemplate()
//     {
//         Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        
//         // Look for weapon buttons specifically
//         string[] weaponKeywords = { "pistol", "rifle", "snipper", "sniper", "smg", "rocket" };
        
//         foreach (var btn in buttons)
//         {
//             if (btn.name.Contains("PLAY") || btn.name.Contains("QUIT")) continue;
            
//             string btnName = btn.name.ToLower();
//             string btnText = GetButtonText(btn).ToLower();
            
//             // Skip skin buttons
//             if (btnName.Contains("skin") || btnText.Contains("skin")) continue;
            
//             // Check if it's a weapon button
//             foreach (string keyword in weaponKeywords)
//             {
//                 if (btnName.Contains(keyword) || btnText.Contains(keyword))
//                 {
//                     return btn;
//                 }
//             }
//         }
        
//         return null;
//     }

//     private static string GetButtonText(Button btn)
//     {
//         Text legacy = btn.GetComponentInChildren<Text>();
//         if (legacy != null) return legacy.text;

//         TextMeshProUGUI tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
//         if (tmp != null) return tmp.text;

//         return "";
//     }

//     private static string FindScenePath(string sceneName)
//     {
//         string path = $"Assets/Scenes/{sceneName}.unity";
//         if (System.IO.File.Exists(path)) return path;

//         string[] guids = AssetDatabase.FindAssets($"t:Scene {sceneName}");
//         if (guids.Length > 0) return AssetDatabase.GUIDToAssetPath(guids[0]);

//         return null;
//     }
// }
