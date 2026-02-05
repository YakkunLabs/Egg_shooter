// using UnityEngine;
// using UnityEditor;
// using UnityEditor.SceneManagement;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;
// using TMPro;
// using UnityEditor.Events;
// using System.Collections.Generic;
// using System.Linq;

// public class UltimateMenuRepair : EditorWindow
// {
//     [MenuItem("Tools/👑 ULTIMATE MENU REPAIR (Buttons & Weapons)")]
//     public static void RepairAll()
//     {
//         string scenePath = FindScenePath("MainMenu");
//         if (string.IsNullOrEmpty(scenePath)) return;

//         Scene scene = EditorSceneManager.OpenScene(scenePath);
//         MainMenu menu = FindAnyObjectByType<MainMenu>();
//         Canvas canvas = FindAnyObjectByType<Canvas>();

//         if (menu == null || canvas == null)
//         {
//             EditorUtility.DisplayDialog("Error", "MainMenu script or Canvas not found!", "OK");
//             return;
//         }

//         // --- PART 1: FIX WEAPON MAPPING ---
//         SerializedObject so = new SerializedObject(menu);
//         SerializedProperty gunsProp = so.FindProperty("menuGuns");
        
//         gunsProp.ClearArray();
//         gunsProp.arraySize = 5;

//         // Find specific models requested by User
//         GameObject pistolModel = FindWeaponModel("Pistal") ?? FindWeaponModel("pistol");
//         GameObject rifleModel  = FindWeaponModel("blaster-r") ?? FindWeaponModel("Rifle");
//         GameObject sniperModel = FindWeaponModel("blaster-e"); // User Req: Sniper = blaster-e
//         GameObject smgModel    = FindWeaponModel("blaster-n"); // User Req: SMG = blaster-n
//         GameObject rocketModel = FindWeaponModel("Rocket Launcher") ?? FindWeaponModel("Rocket");

//         gunsProp.GetArrayElementAtIndex(0).objectReferenceValue = pistolModel;
//         gunsProp.GetArrayElementAtIndex(1).objectReferenceValue = rifleModel;
//         gunsProp.GetArrayElementAtIndex(2).objectReferenceValue = sniperModel;
//         gunsProp.GetArrayElementAtIndex(3).objectReferenceValue = smgModel;
//         gunsProp.GetArrayElementAtIndex(4).objectReferenceValue = rocketModel;

//         so.ApplyModifiedProperties();

//         // --- PART 2: DESTROY OLD BUTTONS (Clean Slate) ---
//         var allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
//         List<string> targetNames = new List<string> { "Pistol", "Rifle", "Snipper", "Sniper", "SMG", "Rocket" };
        
//         foreach(var btn in allButtons)
//         {
//             if (btn == null) continue;
            
//             // Check button name OR text content
//             string bName = btn.name;
//             string bText = GetButtonText(btn);

//             // Skip Skin buttons and Play/Quit
//             if (bName.Contains("Skin") || bText.Contains("Skin")) continue;
//             if (bName.Contains("Play") || bName.Contains("Quit")) continue;

//             // If it matches a weapon name, DESTROY IT
//             bool isTarget = targetNames.Any(t => bName.Contains(t) || bText.Contains(t));
//             if (isTarget)
//             {
//                 DestroyImmediate(btn.gameObject);
//             }
//         }

//         // --- PART 3: CREATE FRESH BUTTONS ---
//         string[] btnLabels = { "Pistol", "Rifle", "Snipper", "SMG", "Rocket" };
        
//         Font gameFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/LilitaOne-Regular.ttf");
//         if (gameFont == null) gameFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

//         // Find the proper parent by looking for existing buttons (Play/Quit)
//         Transform buttonParent = canvas.transform;
//         Button playBtn = FindButtonWithText("PLAY") ?? FindButtonWithText("Play");
//         if (playBtn != null)
//         {
//             buttonParent = playBtn.transform.parent;
//             Debug.Log($"✅ Using button parent: {buttonParent.name}");
//         }

//         float startY = 60;
//         float spacing = 60;
//         float xPos = 450;

//         for (int i = 0; i < btnLabels.Length; i++)
//         {
//             int index = i;
//             string label = btnLabels[i];
            
//             CreateBtn(buttonParent, label, new Vector2(xPos, startY - (i * spacing)), gameFont, () => {
//                 if(menu != null) UnityEventTools.AddIntPersistentListener(GetLastButton().onClick, menu.SelectWeapon, index);
//             });
//         }

//         // --- PART 4: ENSURE INTERACTIVITY (EventSystem & Raycasters) ---
//         if (FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
//         {
//              GameObject eventSystem = new GameObject("EventSystem");
//              eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
//              eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
//              Debug.Log("✅ Created missing EventSystem");
//         }

//         if (canvas.GetComponent<GraphicRaycaster>() == null)
//         {
//             canvas.gameObject.AddComponent<GraphicRaycaster>();
//             Debug.Log("✅ Added missing GraphicRaycaster to Canvas");
//         }

//         // DON'T change Canvas mode - keep original settings to preserve other UI elements

//         EditorSceneManager.MarkSceneDirty(scene);
//         EditorSceneManager.SaveScene(scene);

//         EditorUtility.DisplayDialog("Complete", 
//             "Menu Repaired & CLICKABLE!\n\n" +
//             "• Fixed Buttons & Weapons\n" +
//             "• Added EventSystem (Required for clicking)\n" +
//             "• Fixed Text blocking clicks", "PLAY NOW");
//     }

//     private static string GetButtonText(Button btn)
//     {
//         Text t = btn.GetComponentInChildren<Text>();
//         if (t) return t.text;
//         TextMeshProUGUI tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
//         if (tmp) return tmp.text;
//         return "";
//     }

//     private static Button FindButtonWithText(string text)
//     {
//         Button[] allBtns = FindObjectsByType<Button>(FindObjectsSortMode.None);
//         foreach (var btn in allBtns)
//         {
//             string btnText = GetButtonText(btn);
//             if (btnText.IndexOf(text, System.StringComparison.OrdinalIgnoreCase) >= 0)
//                 return btn;
//         }
//         return null;
//     }

//     private static GameObject FindWeaponModel(string name)
//     {
//         GameObject[] all = Resources.FindObjectsOfTypeAll<GameObject>();
//         foreach (GameObject go in all)
//         {
//             if (go.scene.name == null) continue;
//             if (go.transform.parent == null) continue;
//             if (go.name.IndexOf(name, System.StringComparison.OrdinalIgnoreCase) >= 0) return go;
//         }
//         return null;
//     }

//     private static Button lastCreatedButton;
//     private static Button GetLastButton() => lastCreatedButton;

//     private static void CreateBtn(Transform parent, string name, Vector2 pos, Font font, System.Action onCreated)
//     {
//         GameObject btnObj = new GameObject(name + " Button", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
//         btnObj.transform.SetParent(parent, false);
//         btnObj.layer = 5; // UI Layer - CRITICAL for raycasting

//         // Setup Transform
//         RectTransform rect = btnObj.GetComponent<RectTransform>();
//         rect.anchorMin = new Vector2(0.5f, 0.5f);
//         rect.anchorMax = new Vector2(0.5f, 0.5f);
//         rect.pivot = new Vector2(0.5f, 0.5f);
//         rect.anchoredPosition = pos;
//         rect.sizeDelta = new Vector2(200, 50);

//         // Setup Image
//         Image img = btnObj.GetComponent<Image>();
//         img.color = Color.white;
//         img.raycastTarget = true; // Essential for clicking

//         // Setup Text
//         GameObject tObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
//         tObj.transform.SetParent(btnObj.transform, false);
//         tObj.layer = 5; // UI Layer for text too
//         Text txt = tObj.GetComponent<Text>();
        
//         txt.text = name;
//         txt.font = font;
//         txt.fontSize = 28;
//         txt.alignment = TextAnchor.MiddleCenter;
//         txt.color = Color.black;
//         txt.raycastTarget = false; // CRITICAL: Don't let text block the button click
        
//         RectTransform trect = txt.GetComponent<RectTransform>();
//         trect.anchorMin = Vector2.zero;
//         trect.anchorMax = Vector2.one;
//         trect.offsetMin = Vector2.zero;
//         trect.offsetMax = Vector2.zero;

//         lastCreatedButton = btnObj.GetComponent<Button>();
        
//         // Add default transition for visual feedback
//         ColorBlock colors = lastCreatedButton.colors;
//         colors.normalColor = Color.white;
//         colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
//         colors.pressedColor = new Color(0.7f, 0.7f, 0.7f);
//         lastCreatedButton.colors = colors;

//         onCreated?.Invoke();
        
//         Debug.Log($"✅ Created button: {name} on layer {btnObj.layer}");
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
