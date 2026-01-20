using UnityEngine;

/// <summary>
/// Central Manager for handling Mobile Inputs vs PC Inputs
/// </summary>
public class MobileInputManager : MonoBehaviour
{
    public static MobileInputManager Instance { get; private set; }

    [Header("Mobile Controls References")]
    public VirtualJoystick joystick;
    public TouchField touchField;
    public GameObject mobileCanvas; // The entire UI layer for buttons

    // Button states
    public bool shootPressed = false;
    public bool jumpPressed = false;
    public bool reloadPressed = false;
    public bool scopePressed = false;

    public bool IsMobileMode { get; private set; }

    private void Awake()
    {
        // Singleton Setup - Simple Replace
        // If a new one loads (because we re-entered the scene), let it become the new Instance.
        Instance = this; 
    }

    private void Start()
    {
        // Auto-detect Platform
        CheckPlatform();
    }

    [Header("Debug")]
    public bool forceEnableMobile = false; // Check this in Inspector to FORCE controls to show

    private void CheckPlatform()
    {
        bool isMobile = Application.isMobilePlatform;

        // Check WebGL specific conditions
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // If it identifies as a Desktop OS, assume PC (even if it has touch like a Surface)
            // Telegram Desktop identifies as Windows/Mac usually.
            bool isDesktop = SystemInfo.operatingSystem.Contains("Windows") || 
                             SystemInfo.operatingSystem.Contains("Mac") || 
                             SystemInfo.operatingSystem.Contains("Linux");

            if (!isDesktop && Input.touchSupported) 
            {
                isMobile = true;
            }
        }

        // 3. User Override
        if (forceEnableMobile) isMobile = true;

        IsMobileMode = isMobile;

        #if UNITY_EDITOR
        // Ensure we can test in editor with Remote
        if (UnityEditor.EditorApplication.isRemoteConnected) isMobile = true;
        #endif

        if (mobileCanvas != null)
        {
            mobileCanvas.SetActive(isMobile);
            
            Canvas c = mobileCanvas.GetComponent<Canvas>();
            if(c != null) c.sortingOrder = 999;
        }
    }

    // --- PUBLIC METHODS FOR OTHER SCRIPTS ---

    public float GetHorizontal()
    {
        float pcInput = Input.GetAxis("Horizontal");
        float mobileInput = (joystick != null) ? joystick.InputDirection.x : 0;
        
        return Mathf.Abs(mobileInput) > 0.1f ? mobileInput : pcInput;
    }

    public float GetVertical()
    {
        float pcInput = Input.GetAxis("Vertical");
        float mobileInput = (joystick != null) ? joystick.InputDirection.z : 0; // Joystick 'z' translates to forward/back

        return Mathf.Abs(mobileInput) > 0.1f ? mobileInput : pcInput;
    }

    public float GetLookX()
    {
        float pcInput = Input.GetAxis("Mouse X");
        float mobileInput = (touchField != null) ? touchField.TouchDist.x * 0.2f : 0; // 0.2f is sensitivity dampener

        return Mathf.Abs(mobileInput) > 0.01f ? mobileInput : pcInput;
    }

    public float GetLookY()
    {
        float pcInput = Input.GetAxis("Mouse Y");
        float mobileInput = (touchField != null) ? touchField.TouchDist.y * 0.2f : 0;

        return Mathf.Abs(mobileInput) > 0.01f ? mobileInput : pcInput;
    }

    // --- BUTTON HANDLERS (Connect these to UI Events) ---

    public void OnShootDown() { shootPressed = true; }
    public void OnShootUp() { shootPressed = false; }

    public void OnJumpDown() { jumpPressed = true; }
    public void OnJumpUp() { jumpPressed = false; } // Immediate release for jump

    public void OnReloadPress() { reloadPressed = true; Invoke("ResetReload", 0.1f); }
    private void ResetReload() { reloadPressed = false; }

    public void OnScopeToggle() { scopePressed = !scopePressed; }
}
