using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Events;

public class MainMenuExpansion : EditorWindow
{
    [MenuItem("Tools/🚀 ONE-CLICK EXTENSION (Add Weapons)")]
    public static void ExpandGame()
    {
        bool confirm = EditorUtility.DisplayDialog("Expand Game?", 
            "This will automatically:\n" +
            "1. Add SMG and Rocket Launcher to Main Menu\n" +
            "2. Create Buttons for them\n" +
            "3. Add them to the Game Scene (Loadout)\n" +
            "4. Setup all physics/scripts\n\n" +
            "BACKUP YOUR PROJECT FIRST!", "DO IT!", "Cancel");
        
        if (!confirm) return;

        string smgPath = "Assets/FBX format/blaster-n.fbx";
        string rocketPath = "Assets/BigRookGames/_AssetPacks/Stylized Weapon Pack/Stylized Rocket Launcher/Prefabs/Rocket Launcher.prefab";

        ProcessMainMenu(smgPath, rocketPath);
        ProcessGameScene(smgPath, rocketPath);
        
        Debug.Log("<color=green>Running Weapon Setup Fixer...</color>");
        WeaponSetupGuaranteed.FixWeaponsNow();

        EditorUtility.DisplayDialog("Success!", "Expansion Complete! SMG and Rocket Launcher added!", "Cool");
    }

    private static void ProcessMainMenu(string smgPath, string rocketPath)
    {
        string scenePath = FindScenePath("MainMenu");
        if (string.IsNullOrEmpty(scenePath)) return;

        Scene scene = EditorSceneManager.OpenScene(scenePath);
        MainMenu menu = FindAnyObjectByType<MainMenu>();
        
        if (menu == null)
        {
            Debug.LogError("MainMenu script not found!");
            return;
        }

        // Spawn Weapons using the FIRST weapon as reference
        if (menu.menuGuns.Length == 0)
        {
            Debug.LogError("No weapons in menuGuns array!");
            return;
        }

        GameObject refWeapon = menu.menuGuns[0];
        GameObject smg = SpawnWeapon(smgPath, "blaster-n", refWeapon.transform);
        GameObject rocket = SpawnWeapon(rocketPath, "Rocket Launcher", refWeapon.transform);

        // Use SerializedObject to properly modify the array
        SerializedObject so = new SerializedObject(menu);
        SerializedProperty propGuns = so.FindProperty("menuGuns");
        
        AddToSerializedArray(propGuns, smg);
        AddToSerializedArray(propGuns, rocket);
        
        so.ApplyModifiedProperties();

        // Create Buttons
        Button sniperBtn = FindButtonWithText("Snipper") ?? FindButtonWithText("Sniper");
        
        if (sniperBtn != null)
        {
            CreateButton(sniperBtn, "SMG", 3, menu);
            CreateButton(sniperBtn, "Rocket", 4, menu);
        }
        else
        {
            Debug.LogWarning("Sniper button not found - buttons not created");
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static void ProcessGameScene(string smgPath, string rocketPath)
    {
        string scenePath = FindScenePath("SampleScene");
        if (string.IsNullOrEmpty(scenePath)) return;

        Scene scene = EditorSceneManager.OpenScene(scenePath);
        LoadoutManager loadout = FindAnyObjectByType<LoadoutManager>();
        
        if (loadout == null)
        {
            Debug.LogError("LoadoutManager not found!");
            return;
        }

        if (loadout.weapons.Length == 0)
        {
            Debug.LogError("No weapons in LoadoutManager!");
            return;
        }

        GameObject refWeapon = loadout.weapons[0];
        GameObject smg = SpawnWeapon(smgPath, "blaster-n", refWeapon.transform);
        GameObject rocket = SpawnWeapon(rocketPath, "Rocket Launcher", refWeapon.transform);

        SerializedObject so = new SerializedObject(loadout);
        SerializedProperty propWeapons = so.FindProperty("weapons");
        
        AddToSerializedArray(propWeapons, smg);
        AddToSerializedArray(propWeapons, rocket);
        
        so.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static void AddToSerializedArray(SerializedProperty arrayProp, GameObject item)
    {
        // Check if already exists
        for (int i = 0; i < arrayProp.arraySize; i++)
        {
            if (arrayProp.GetArrayElementAtIndex(i).objectReferenceValue == item)
                return; // Already added
        }

        int newIndex = arrayProp.arraySize;
        arrayProp.InsertArrayElementAtIndex(newIndex);
        arrayProp.GetArrayElementAtIndex(newIndex).objectReferenceValue = item;
    }

    private static GameObject SpawnWeapon(string path, string name, Transform referenceTransform)
    {
        // Check if already exists
        Transform existing = referenceTransform.parent.Find(name);
        if (existing != null)
        {
            Debug.Log($"Weapon '{name}' already exists, reusing it.");
            return existing.gameObject;
        }

        Object prefab = AssetDatabase.LoadAssetAtPath<Object>(path);
        if (prefab == null)
        {
            Debug.LogError($"Asset not found: {path}");
            return null;
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.name = name;
        instance.transform.SetParent(referenceTransform.parent);
        instance.transform.localPosition = referenceTransform.localPosition;
        instance.transform.localRotation = referenceTransform.localRotation;
        instance.transform.localScale = referenceTransform.localScale;
        instance.SetActive(false);
        
        return instance;
    }

    private static Button FindButtonWithText(string textContent)
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach(var btn in buttons)
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

    private static void CreateButton(Button original, string newLabel, int weaponIndex, MainMenu menuScript)
    {
        // Check if button already exists
        if (FindButtonWithText(newLabel) != null)
        {
            Debug.Log($"Button '{newLabel}' already exists");
            return;
        }

        GameObject newBtnObj = Instantiate(original.gameObject, original.transform.parent);
        newBtnObj.name = newLabel + " Button";
        
        // Positioning
        RectTransform rect = newBtnObj.GetComponent<RectTransform>();
        if (original.transform.parent.GetComponent<VerticalLayoutGroup>() == null)
        {
            float offset = (weaponIndex - 2) * (rect.rect.height + 10f);
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y - offset);
        }

        // Update Text
        Text legacy = newBtnObj.GetComponentInChildren<Text>();
        if (legacy) legacy.text = newLabel;
        
        TextMeshProUGUI tmp = newBtnObj.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp) tmp.text = newLabel;

        // Fix Button Click Event
        Button btn = newBtnObj.GetComponent<Button>();
        
        // Clear ALL persistent listeners
        while (btn.onClick.GetPersistentEventCount() > 0)
        {
            UnityEventTools.RemovePersistentListener(btn.onClick, 0);
        }
        
        // Add new listener
        UnityEventTools.AddIntPersistentListener(btn.onClick, menuScript.SelectWeapon, weaponIndex);
        
        // Ensure visual properties are preserved (they should be from Instantiate, but let's be explicit)
        Image btnImage = btn.GetComponent<Image>();
        Image origImage = original.GetComponent<Image>();
        if (btnImage != null && origImage != null)
        {
            btnImage.sprite = origImage.sprite;
            btnImage.color = origImage.color;
            btnImage.material = origImage.material;
            btnImage.type = origImage.type;
        }

        EditorUtility.SetDirty(newBtnObj);
    }

    private static string FindScenePath(string sceneName)
    {
        string path = $"Assets/Scenes/{sceneName}.unity";
        if (System.IO.File.Exists(path)) return path;

        string[] guids = AssetDatabase.FindAssets($"t:Scene {sceneName}");
        if (guids.Length > 0) return AssetDatabase.GUIDToAssetPath(guids[0]);

        Debug.LogError($"Scene '{sceneName}' not found!");
        return null;
    }
}
