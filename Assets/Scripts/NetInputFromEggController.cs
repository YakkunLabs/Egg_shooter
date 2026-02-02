using UnityEngine;

public class NetInputFromEggController : MonoBehaviour
{
    public NetClient netClient;
    public EggController egg;

    public float sendHz = 30f;
    float _accum;

    // --- QUEUED INPUTS (Latch these so they aren't missed) ---
    int _pendingShots = 0;
    bool _pendingJump = false;
    bool _pendingReload = false; 

    void Start()
    {
        if (netClient == null) netClient = FindFirstObjectByType<NetClient>();
        if (egg == null) egg = GetComponent<EggController>();
    }

    void Update()
    {
        if (netClient == null || egg == null) return;
        if (netClient.myPlayerId == 0) return;
        if (!egg.canMove) return;

        // ---------------------------------------------------------------------
        // 1. CAPTURE INPUT (Every Frame - Before Network Check)
        // ---------------------------------------------------------------------

        // A. Gun Status Check (Client-Side Gatekeeping)
        bool gunReady = true;
        AdvancedGunSystem myGun = GetComponentInChildren<AdvancedGunSystem>();
        
        if (myGun != null)
        {
            if (myGun.isReloading || myGun.currentAmmo <= 0 || !myGun.readyToShoot)
            {
                gunReady = false;
            }
        }

        // B. Queue Inputs & TRIGGER LOCAL GUN (Instant Feedback)

        // --- SHOOTING ---
        if (gunReady && Input.GetMouseButtonDown(0))
        {
            _pendingShots++;        // 1. Queue for Network
            myGun.AttemptToShoot(); // 2. FIRE LOCALLY (Sound/Ammo/Flash)
        }

        // --- JUMPING ---
        if (Input.GetButtonDown("Jump") || (MobileInputManager.Instance != null && MobileInputManager.Instance.jumpPressed))
            _pendingJump = true;

        // --- RELOADING ---
        if (Input.GetKeyDown(KeyCode.R))
        {
            _pendingReload = true;  // 1. Queue for Network
            if (myGun != null) myGun.AttemptToReload(); // 2. RELOAD LOCALLY
        }

        // ---------------------------------------------------------------------
        // 2. NETWORK SEND (Fixed Rate)
        // ---------------------------------------------------------------------
        _accum += Time.deltaTime;
        float interval = 1f / Mathf.Max(1f, sendHz);
        if (_accum < interval) return;
        _accum -= interval;

        // --- MOVEMENT ---
        float v = MobileInputManager.Instance != null ? MobileInputManager.Instance.GetVertical() : Input.GetAxisRaw("Vertical");
        float h = MobileInputManager.Instance != null ? MobileInputManager.Instance.GetHorizontal() : Input.GetAxisRaw("Horizontal");

        bool w = v > 0.1f;
        bool s = v < -0.1f;
        bool d = h > 0.1f;
        bool a = h < -0.1f;

        bool sendW = w;
        bool sendS = s;
        bool run = Input.GetKey(KeyCode.LeftShift);

        // --- RETRIEVE QUEUED INPUTS ---
        bool shootPressed = _pendingShots > 0;
        bool jumpPressed = _pendingJump;
        bool reloadPressed = _pendingReload; 

        // --- CALCULATE ANGLES (Use Camera for accuracy) ---
        
        // 1. Body Yaw (FaceYaw)
        float bodyYawDeg = transform.eulerAngles.y;
        if (bodyYawDeg > 180f) bodyYawDeg -= 360f;
        float faceYaw = bodyYawDeg * Mathf.Deg2Rad;

        // 2. Aim Pitch (Vertical) & Aim Yaw (Horizontal)
        float aimYaw = 0;
        float aimPitch = 0;

        // Try to find the camera to get the REAL aiming direction
        Camera cam = GetComponentInChildren<Camera>();
        if (cam == null) cam = Camera.main;

        if (cam != null)
        {
            // Pitch (X axis): -90 (Up) to 90 (Down)
            float pitchDeg = cam.transform.eulerAngles.x;
            if (pitchDeg > 180f) pitchDeg -= 360f;
            aimPitch = pitchDeg * Mathf.Deg2Rad;

            // Yaw (Y axis): usually same as body, but camera is truth
            float yawDeg = cam.transform.eulerAngles.y;
            if (yawDeg > 180f) yawDeg -= 360f;
            aimYaw = yawDeg * Mathf.Deg2Rad;
        }
        else
        {
            // Fallback if no camera found
            aimYaw = faceYaw;
        }

        // --- SEND ---
        ushort dtMs = (ushort)Mathf.Clamp(Mathf.RoundToInt(Time.deltaTime * 1000f), 0, 65535);

        netClient.SendInput(
            sendW, a, sendS, d,
            run, jumpPressed, faceYaw,
            aimYaw, aimPitch, shootPressed, reloadPressed,
            dtMs
        );

        // --- RESET QUEUES ---
        if (shootPressed) _pendingShots--;
        _pendingJump = false;
        _pendingReload = false; 
    }
}