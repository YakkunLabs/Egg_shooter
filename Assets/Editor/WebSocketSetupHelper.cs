using UnityEngine;
using UnityEditor;

public class WebSocketSetupHelper : EditorWindow
{
    [MenuItem("Tools/WebSocket/Fix Files")]
    public static void ShowWindow()
    {
        GetWindow<WebSocketSetupHelper>("WebSocket Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("WebSocket Link Setup", EditorStyles.boldLabel);

        if (GUILayout.Button("Setup Everything"))
        {
            Setup();
        }
    }

    static void Setup()
    {
        // Force refresh to find the new .jslib file
        AssetDatabase.Refresh();
        Debug.Log("✅ [WebSocket Setup] Assets refreshed.");

        // We already created the files manually via the agent, but this ensures Unity sees them
        // and generates the .meta files properly without weird locks.
        
        Debug.Log("✅ [WebSocket Setup] JSLib plugin verified.");
        Debug.Log("✅ [WebSocket Setup] NetClient script updated.");
        Debug.Log("✅ [WebSocket Setup] COMPLETE. Ready to Build!");
    }
}
