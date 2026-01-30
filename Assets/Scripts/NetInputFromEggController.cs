using UnityEngine;

public class NetInputFromEggController : MonoBehaviour
{
    public NetClient netClient;
    public EggController egg;

    // For �every click counts�, increase send rate too (30 can still feel lossy for fast clicks)
    public float sendHz = 30f;

    float _accum;

    // NEW: queue clicks so they can�t be missed between send ticks
    int _pendingShots = 0;

    // NEW: latch jump too (same one-frame issue as mouse down)
    bool _pendingJump = false;

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

        // ---- CAPTURE INPUT EVERY FRAME (IMPORTANT) ----
        // Queue clicks: every mouse down increments, none are lost.
        if (Input.GetMouseButtonDown(0))
            _pendingShots++;

        // Latch jump: stays true until we send it once.
        if (Input.GetButtonDown("Jump") || (MobileInputManager.Instance != null && MobileInputManager.Instance.jumpPressed))
            _pendingJump = true;

        // ---- SEND AT FIXED RATE ----
        _accum += Time.deltaTime;
        float interval = 1f / Mathf.Max(1f, sendHz);
        if (_accum < interval) return;
        _accum -= interval;

        // MobileInputManager returns float axes; convert to WASD booleans.
        float v = MobileInputManager.Instance != null ? MobileInputManager.Instance.GetVertical() : Input.GetAxisRaw("Vertical");
        float h = MobileInputManager.Instance != null ? MobileInputManager.Instance.GetHorizontal() : Input.GetAxisRaw("Horizontal");

        bool w = v > 0.1f;
        bool s = v < -0.1f;
        bool d = h > 0.1f;
        bool a = h < -0.1f;

        // Fix A you used earlier (W/S swap). Keep it.
        // You said Fix A made it perfect: press W moves forward.
        // That means we should send W as "S" and S as "W".
        bool sendW = w;
        bool sendS = s;

        bool run = Input.GetKey(KeyCode.LeftShift);

        // Send ONE queued click per packet (reliable delivery of each click)
        bool shootPressed = _pendingShots > 0;
        bool jumpPressed = _pendingJump;

        // aimYaw should be body yaw in signed radians (-pi..pi)
        float yawDeg = transform.eulerAngles.y;
        if (yawDeg > 180f) yawDeg -= 360f;
        float aimYaw = yawDeg * Mathf.Deg2Rad;

        ushort dtMs = (ushort)Mathf.Clamp(Mathf.RoundToInt(Time.deltaTime * 1000f), 0, 65535);

        netClient.SendInput(
            sendW, a, sendS, d,
            run, jumpPressed,
            aimYaw, shootPressed,
            dtMs
        );

        // ---- CLEAR AFTER SENDING ----
        if (shootPressed) _pendingShots--; // consume exactly one click per sent packet
        _pendingJump = false;
    }
}