using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Editor Tool to AUTOMATICALLY create the Mobile UI 
/// because doing it manually is annoying.
/// </summary>
public class MobileSetupTool : EditorWindow
{
    [MenuItem("Tools/Setup Mobile Controls")]
    public static void ShowWindow()
    {
        GetWindow<MobileSetupTool>("Mobile Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Mobile Controls Generator", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("GENERATE MOBILE UI", GUILayout.Height(50)))
        {
            CreateMobileUI();
        }
    }

    private void CreateMobileUI()
    {
        // 1. Check/Create EventSystem
        GameObject eventSystemChecker = GameObject.FindObjectOfType<EventSystem>()?.gameObject;
        if (eventSystemChecker == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            Undo.RegisterCreatedObjectUndo(eventSystem, "Create EventSystem");
            eventSystem.AddComponent<EventSystem>();

            // SMART INPUT MODULE SETUP
            // Try to add the New Input System module first
            bool addedNewSystem = false;
            try
            {
                // We use reflection so this code compiles even if you don't have the package
                System.Type inputType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
                if (inputType != null)
                {
                    eventSystem.AddComponent(inputType);
                    addedNewSystem = true;
                    Debug.Log("Added New InputSystemUIInputModule");
                }
            }
            catch { }

            // Fallback to Old System if New one failed/missing
            if (!addedNewSystem)
            {
                eventSystem.AddComponent<StandaloneInputModule>();
                Debug.Log("Added Standard StandaloneInputModule");
            }
        }

        // 2. Create Canvas
        GameObject canvasObj = new GameObject("MobileControls");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        // 3. Create Manager Singleton
        GameObject managerObj = new GameObject("MobileManager");
        MobileInputManager manager = managerObj.AddComponent<MobileInputManager>();
        manager.mobileCanvas = canvasObj;

        // --- LEFT SIDE: JOYSTICK ---
        GameObject joystickBg = CreateImage(canvasObj.transform, "JoystickBackground", AnchorPresets.BottomLeft);
        RectTransform joyRect = joystickBg.GetComponent<RectTransform>();
        joyRect.anchoredPosition = new Vector2(300, 300);
        joyRect.sizeDelta = new Vector2(300, 300);
        joystickBg.GetComponent<Image>().color = new Color(1, 1, 1, 0.3f); // Semi-transparent white
        
        // Add Joystick Handle
        GameObject handle = CreateImage(joystickBg.transform, "JoystickHandle", AnchorPresets.MiddleCenter);
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(100, 100);
        handle.GetComponent<Image>().color = new Color(1, 1, 1, 0.8f);

        // Add Script
        VirtualJoystick joystickScript = joystickBg.AddComponent<VirtualJoystick>();
        joystickScript.bgImage = joystickBg.GetComponent<Image>();
        joystickScript.joystickImg = handle.GetComponent<Image>();
        
        // Link to Manager
        manager.joystick = joystickScript;

        // --- RIGHT SIDE: TOUCH FIELD (Aiming) ---
        GameObject touchFieldObj = CreateImage(canvasObj.transform, "TouchField", AnchorPresets.StretchRight);
        touchFieldObj.GetComponent<Image>().color = new Color(0, 0, 0, 0); // Invisible
        TouchField touchScript = touchFieldObj.AddComponent<TouchField>();
        
        // Link to Manager
        manager.touchField = touchScript;

        // --- BUTTONS ---
        
        // SHOOT (Big button right)
        CreateButton(canvasObj.transform, "Btn_Shoot", new Vector2(-250, 300), new Vector2(250, 250), "SHOOT", Color.red, (trigger) => {
            AddEvent(trigger, EventTriggerType.PointerDown, (data) => manager.OnShootDown());
            AddEvent(trigger, EventTriggerType.PointerUp, (data) => manager.OnShootUp());
        });

        // JUMP (Smaller button)
        CreateButton(canvasObj.transform, "Btn_Jump", new Vector2(-550, 200), new Vector2(180, 180), "JUMP", Color.green, (trigger) => {
            AddEvent(trigger, EventTriggerType.PointerDown, (data) => manager.OnJumpDown());
            AddEvent(trigger, EventTriggerType.PointerUp, (data) => manager.OnJumpUp());
        });

        // RELOAD (Smaller top)
        CreateButton(canvasObj.transform, "Btn_Reload", new Vector2(-450, 500), new Vector2(150, 150), "RELOAD", Color.yellow, (trigger) => {
            AddEvent(trigger, EventTriggerType.PointerDown, (data) => manager.OnReloadPress());
        });

        // SCOPE (Side)
        CreateButton(canvasObj.transform, "Btn_Scope", new Vector2(-200, 600), new Vector2(150, 150), "SCOPE", Color.cyan, (trigger) => {
            AddEvent(trigger, EventTriggerType.PointerDown, (data) => manager.OnScopeToggle());
        });

        Debug.Log("<color=green>MOBILE CONTROLS GENERATED SUCCESSFULLY!</color>");
        
        // Select the manager so user sees it
        Selection.activeGameObject = managerObj;

        // ENABLE SAVING: Mark scene as dirty so Unity asks to save
        EditorUtility.SetDirty(managerObj);
        EditorUtility.SetDirty(canvasObj);
        if (!Application.isPlaying) 
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }
    }

    // --- HELPERS ---

    private GameObject CreateImage(Transform parent, string name, AnchorPresets preset)
    {
        GameObject obj = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(obj, "Create Mobile UI"); // Add Undo support
        obj.transform.SetParent(parent, false);
        Image img = obj.AddComponent<Image>();
        img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd"); // Default circle look
        SetAnchor(obj.GetComponent<RectTransform>(), preset);
        return obj;
    }

    private void CreateButton(Transform parent, string name, Vector2 pos, Vector2 size, string label, Color color, System.Action<EventTrigger> setupEvents)
    {
        GameObject btnObj = CreateImage(parent, name, AnchorPresets.BottomRight);
        btnObj.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.5f);
        RectTransform rect = btnObj.GetComponent<RectTransform>();
        rect.anchoredPosition = pos;
        rect.sizeDelta = size;

        // Add Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 32;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.rectTransform.anchorMin = Vector2.zero;
        tmp.rectTransform.anchorMax = Vector2.one;
        tmp.rectTransform.offsetMin = Vector2.zero;
        tmp.rectTransform.offsetMax = Vector2.zero;

        // Add Event Trigger for custom events
        EventTrigger trigger = btnObj.AddComponent<EventTrigger>();
        setupEvents(trigger);
    }

    private void AddEvent(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    private void SetAnchor(RectTransform rect, AnchorPresets preset)
    {
        switch (preset)
        {
            case AnchorPresets.BottomLeft:
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.zero;
                break;
            case AnchorPresets.BottomRight:
                rect.anchorMin = new Vector2(1, 0);
                rect.anchorMax = new Vector2(1, 0);
                break;
            case AnchorPresets.StretchRight:
                rect.anchorMin = new Vector2(0.5f, 0); // Start from middle
                rect.anchorMax = Vector2.one;          // Stretch to top-right
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                break;
            case AnchorPresets.MiddleCenter:
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                break;
        }
    }

    enum AnchorPresets { BottomLeft, BottomRight, StretchRight, MiddleCenter }
}
