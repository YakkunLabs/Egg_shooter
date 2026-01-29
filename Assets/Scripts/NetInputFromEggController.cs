using UnityEngine;

public class NetInputFromEggController : MonoBehaviour
{
    public NetClient netClient;
    public EggController egg;
    public float sendHz = 30f;

    float _accum;

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

        // If you previously needed Fix A for axis mismatch, apply it here too:
        // swap W and S
        bool sendW = w;
        bool sendS = s;

        bool run = Input.GetKey(KeyCode.LeftShift); // if you want run later
        bool jumpPressed = Input.GetButtonDown("Jump") || (MobileInputManager.Instance != null && MobileInputManager.Instance.jumpPressed);

        // aimYaw should be body yaw in signed radians (-pi..pi)
        float yawDeg = transform.eulerAngles.y;
        if (yawDeg > 180f) yawDeg -= 360f;
        float aimYaw = yawDeg * Mathf.Deg2Rad;

        bool shootPressed = Input.GetMouseButtonDown(0);

        ushort dtMs = (ushort)Mathf.Clamp(Mathf.RoundToInt(Time.deltaTime * 1000f), 0, 65535);

        netClient.SendInput(
            sendW, a, sendS, d,
            run, jumpPressed,
            aimYaw, shootPressed,
            dtMs
        );
    }
}
