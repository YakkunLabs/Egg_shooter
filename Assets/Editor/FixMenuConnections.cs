using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class FixMenuConnections : EditorWindow
{
    [MenuItem("Tools/🔗 FIX MENU WEAPON CONNECTIONS")]
    public static void FixConnections()
    {
        string scenePath = FindScenePath("MainMenu");
        if (string.IsNullOrEmpty(scenePath)) return;

        Scene scene = EditorSceneManager.OpenScene(scenePath);
        MainMenu menu = FindAnyObjectByType<MainMenu>();

        if (menu == null)
        {
            EditorUtility.DisplayDialog("Error", "MainMenu script not found!", "OK");
            return;
        }

        // Find the specific weapon objects in the hierarchy
        // They should be children of the holder.
        
        // We need to find 5 specific weapons
        GameObject pistol = FindWeaponInScene("Pistol"); // Often named "Pistal" or "Pistol"
        if (pistol == null) pistol = FindWeaponInScene("Pistal");

        GameObject rifle = FindWeaponInScene("Rifle"); 
        if (rifle == null) rifle = FindWeaponInScene("Assault");

        GameObject sniper = FindWeaponInScene("Sniper");
        if (sniper == null) sniper = FindWeaponInScene("Snipper"); // User used "Snipper" in buttons

        GameObject smg = FindWeaponInScene("blaster"); // blaster-n
        
        GameObject rocket = FindWeaponInScene("Rocket");

        // Verify we found them
        if (pistol == null || rifle == null || sniper == null || smg == null || rocket == null)
        {
            string missing = "";
            if (pistol == null) missing += "Pistol ";
            if (rifle == null) missing += "Rifle ";
            if (sniper == null) missing += "Sniper ";
            if (smg == null) missing += "SMG ";
            if (rocket == null) missing += "Rocket ";
            
            EditorUtility.DisplayDialog("Error", $"Could not find these weapons in the scene:\n{missing}\n\nCheck naming!", "OK");
            return;
        }

        // Use SerializedObject to update the array reliably
        SerializedObject so = new SerializedObject(menu);
        SerializedProperty gunsProp = so.FindProperty("menuGuns");

        gunsProp.ClearArray();
        gunsProp.arraySize = 5;

        gunsProp.GetArrayElementAtIndex(0).objectReferenceValue = pistol;
        gunsProp.GetArrayElementAtIndex(1).objectReferenceValue = rifle;
        gunsProp.GetArrayElementAtIndex(2).objectReferenceValue = sniper;
        gunsProp.GetArrayElementAtIndex(3).objectReferenceValue = smg;
        gunsProp.GetArrayElementAtIndex(4).objectReferenceValue = rocket;

        so.ApplyModifiedProperties();

        // Ensure purely visual state is reset (hide all)
        pistol.SetActive(false);
        rifle.SetActive(false);
        sniper.SetActive(false);
        smg.SetActive(false);
        rocket.SetActive(false);

        // Maybe activate pistol by default?
        pistol.SetActive(true);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        EditorUtility.DisplayDialog("Success", "Weapons re-connected to MainMenu!\n\nOrder:\n0: Pistol\n1: Rifle\n2: Sniper\n3: SMG\n4: Rocket", "Awesome");
    }

    private static GameObject FindWeaponInScene(string partialName)
    {
        // Search ALL objects in scene to be thorough
        GameObject[] all = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject go in all)
        {
            // Must be in scene
            if (go.scene.name == null) continue;
            
            // Should be a child of something (WeaponHolder), not a root object usually
            if (go.transform.parent == null) continue;

            if (go.name.IndexOf(partialName, System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                // Avoid "WeaponHolder" if searching for "Weapon"
                if (go.name == "WeaponHolder") continue;
                return go;
            }
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
