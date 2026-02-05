// using UnityEngine;
// using UnityEditor;
// using UnityEditor.SceneManagement;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;
// using TMPro;
// using UnityEditor.Events;
// using System.Linq;
// using System.Collections.Generic;

// public class TotalMenuRebuild : EditorWindow
// {
//     [MenuItem("Tools/🏗️ TOTAL MENU REBUILD (From Scratch)")]
//     public static void RebuildMenu()
//     {
//         bool confirm = EditorUtility.DisplayDialog("Total Rebuild", 
//             "This will DELETE and RESPAWN all menu buttons.\n" +
//             "This fixes duplicate text and alignment issues.\n\n" +
//             "Continue?", "CLEAN & REBUILD", "Cancel");
        
//         if (!confirm) return;

//         string scenePath = FindScenePath("MainMenu");
//         if (string.IsNullOrEmpty(scenePath)) return;

//         Scene scene = EditorSceneManager.OpenScene(scenePath);
//         MainMenu menu = FindAnyObjectByType<MainMenu>();
//         Canvas canvas = FindAnyObjectByType<Canvas>();

//         if (menu == null || canvas == null)
//         {
//             Debug.LogError("Missing MainMenu script or Canvas!");
//             return;
//         }

//         // --- STEP 0: CLEANUP ---
//         // Destroy existing buttons to prevent duplicates/stacking
//         string[] targetNames = { "White Skin", "Army Skin", "Pistol", "Rifle", "Snipper", "SMG", "Rocket" };
        
//         List<GameObject> toDestroy = new List<GameObject>();
        
//         // Find buttons
//         var allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
//         foreach(var btn in allButtons)
//         {
//             if (btn == null) continue;
//             string bName = btn.name;
//             bool isTarget = targetNames.Any(t => bName.Contains(t));
//             // explicit checks
//             if (bName.Contains("Play") || bName.Contains("Quit")) isTarget = false;
            
//             if (isTarget) toDestroy.Add(btn.gameObject);
//         }

//         // Find stray text
//         var allText = FindObjectsByType<Text>(FindObjectsSortMode.None);
//         foreach(var txt in allText)
//         {
//              if (txt == null) continue;
//              string tContent = txt.text;
//              bool isTarget = targetNames.Any(t => tContent.Equals(t)); // Exact match for content
             
//              // If this text is NOT part of a button we already marked for death, mark it too.
//              // But if it IS part of a marked button, it will die anyway.
//              // If it's a floating text labeled "Rocket", kill it.
//              if (isTarget && txt.transform.parent.GetComponent<Button>() == null)
//              {
//                  toDestroy.Add(txt.gameObject);
//              }
//         }

//         foreach(var obj in toDestroy)
//         {
//             if (obj != null) DestroyImmediate(obj);
//         }


//         // Load Font
//         Font gameFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/LilitaOne-Regular.ttf");
//         if (gameFont == null) gameFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

//         // --- STEP 1: SKIN BUTTONS (LEFT) ---
//         CreateButton(canvas.transform, "White Skin", new Vector2(-450, 60), 200, 50, gameFont, () => {
//             if (menu != null) UnityEventTools.AddIntPersistentListener(GetLastButton().onClick, menu.SelectSkin, 0);
//         });
        
//         CreateButton(canvas.transform, "Army Skin", new Vector2(-450, 0), 200, 50, gameFont, () => {
//              if (menu != null) UnityEventTools.AddIntPersistentListener(GetLastButton().onClick, menu.SelectSkin, 1);
//         });

//         // --- STEP 2: WEAPON BUTTONS (RIGHT) ---
//         string[] weapons = { "Pistol", "Rifle", "Snipper", "SMG", "Rocket" };
        
//         float startY = 60; 
//         float spacing = 60; 
//         float xPos = 450; 

//         for(int i=0; i<weapons.Length; i++)
//         {
//             int index = i; 
//             CreateButton(canvas.transform, weapons[i], new Vector2(xPos, startY - (i * spacing)), 200, 50, gameFont, () => {
//                 if (menu != null) UnityEventTools.AddIntPersistentListener(GetLastButton().onClick, menu.SelectWeapon, index);
//             });
//         }

//         EditorSceneManager.MarkSceneDirty(scene);
//         EditorSceneManager.SaveScene(scene);
//         EditorUtility.DisplayDialog("Done", "Menu Cleaned & Rebuilt! Duplicate text should be gone.", "OK");
//     }

//     private static Button lastCreatedButton;

//     private static Button GetLastButton() => lastCreatedButton;

//     private static void CreateButton(Transform parent, string name, Vector2 pos, float w, float h, Font font, System.Action onCreated)
//     {
//         // Create New
//         GameObject btnObj = new GameObject(name + " Button", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
//         btnObj.transform.SetParent(parent, false);

//         // Position
//         RectTransform rect = btnObj.GetComponent<RectTransform>();
//         if (rect != null)
//         {
//             rect.anchorMin = new Vector2(0.5f, 0.5f); 
//             rect.anchorMax = new Vector2(0.5f, 0.5f); 
//             rect.pivot = new Vector2(0.5f, 0.5f);     
//             rect.anchoredPosition = pos;
//             rect.sizeDelta = new Vector2(w, h);
//         }

//         // Visuals
//         Image img = btnObj.GetComponent<Image>();
//         if (img == null) img = btnObj.AddComponent<Image>();
//         img.color = Color.white; 

//         // Text - Create FRESH
//         GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
//         textObj.transform.SetParent(btnObj.transform, false);
//         Text txt = textObj.GetComponent<Text>();
        
//         txt.text = name;
//         txt.font = font;
//         txt.alignment = TextAnchor.MiddleCenter;
//         txt.color = Color.black;
//         txt.fontSize = 28;
            
//         RectTransform textRect = txt.GetComponent<RectTransform>();
//         if (textRect != null)
//         {
//             textRect.anchorMin = Vector2.zero;
//             textRect.anchorMax = Vector2.one;
//             textRect.offsetMin = Vector2.zero;
//             textRect.offsetMax = Vector2.zero;
//         }

//         // Logic
//         lastCreatedButton = btnObj.GetComponent<Button>();
//         if (lastCreatedButton == null) lastCreatedButton = btnObj.AddComponent<Button>();
            
//         onCreated?.Invoke();
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
