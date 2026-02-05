using Capnp;
using CapnpGen;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class NetClient : MonoBehaviour
{
    [Header("Visuals")]
    public GameObject bulletPrefab; // Drag your NetBullet here!
    [Header("Effects")]
    public GameObject muzzleFlashPrefab; // Drag a particle effect here
    public AudioClip shootSound;         // Drag a "Bang.wav" here

    public bool isGameStarted = false;

    [Header("TCP")]
    public string host = "127.0.0.1";
    public int port = 9001;

    [Header("Capsules")]
    public float capsuleHeight = 2f;
    public float capsuleRadius = 0.5f;

    [Header("Debug")]
    public bool log = true;

    TcpClient _tcp;
    NetworkStream _stream;
    Thread _rxThread;
    volatile bool _running;

    uint _sequence = 0;

    readonly ConcurrentQueue<Action> _mainThread = new();
    readonly Dictionary<ulong, GameObject> _players = new();

    public ulong myPlayerId { get; private set; } = 0;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    void Start() => Connect();
    void OnDestroy() => Shutdown();

    void Update()
    {
        while (_mainThread.TryDequeue(out var a)) a();
    }

    // ---------------- CONNECT ----------------

    public void Connect()
    {
        try
        {
            _tcp = new TcpClient();
            _tcp.NoDelay = true;
            _tcp.Connect(host, port);
            _stream = _tcp.GetStream();

            _running = true;
            _rxThread = new Thread(RxLoop) { IsBackground = true };
            _rxThread.Start();

            if (log) Debug.Log($"[NetClient] Connected {host}:{port}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[NetClient] Connect failed: {e}");
        }
    }

    public void Shutdown()
    {
        _running = false;
        try { _stream?.Close(); } catch { }
        try { _tcp?.Close(); } catch { }
        try { if (_rxThread != null && _rxThread.IsAlive) _rxThread.Join(200); } catch { }
    }

    // ---------------- RX LOOP ----------------

    void RxLoop()
    {
        try
        {
            while (_running)
            {
                // Rust read_frame uses BIG-ENDIAN u32 length:
                byte[] payload = ReadFrameBigEndian(_stream);
                if (payload == null) break;

                using var ms = new MemoryStream(payload, writable: false);
                var segments = Framing.ReadSegments(ms);
                var state = DeserializerState.CreateRoot(segments);

                var msg = new ServerMsg.READER(state);

                if (msg.which == ServerMsg.WHICH.AssignId)
                {
                    var a = msg.AssignId;
                    _mainThread.Enqueue(() =>
                    {
                        myPlayerId = a.PlayerId;
                        if (log) Debug.Log($"[NetClient] Assigned playerId={myPlayerId}");

                        int savedSkin = PlayerPrefs.GetInt("SelectedSkin", 0);
                        int savedWeapon = PlayerPrefs.GetInt("SelectedWeapon", 0);

                        Debug.Log($"[NetClient] Sending Config -> Skin: {savedSkin}, Weapon: {savedWeapon}");
                        
                        SendSelectSkin(savedSkin);
                        // SendSelectWeapon(savedWeapon);

                        // int selectedWeapon = PlayerPrefs.GetInt("SelectedWeapon", 0);

                        // optional: send a first empty input so server sees activity
                        
                        SendTestInput();
});
                }
                else if (msg.which == ServerMsg.WHICH.Snapshot)
                {
                    var snap = msg.Snapshot;
                    _mainThread.Enqueue(() => ApplySnapshot(snap));
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[NetClient] RxLoop error: {e}");
        }

        _running = false;
        if (log) Debug.Log("[NetClient] Disconnected");
    }

    void ApplySnapshot(Snapshot.READER snap)
    {
        ApplyPlayers(snap);
        ApplyEvents(snap);
    }

    void ApplyPlayers(Snapshot.READER snap)
    {
        var alive = new HashSet<ulong>();

        foreach (var p in snap.Players)
        {
            var id = p.PlayerId;
            alive.Add(id);

            // 1) Spawn
            if (!_players.TryGetValue(id, out var go) || go == null)
            {
                bool isLocal = (id == myPlayerId);
                go = isLocal ? Instantiate(playerPrefab) : Instantiate(enemyPrefab);

                PlayerWeaponController wc = go.GetComponent<PlayerWeaponController>();
                    if (wc != null)
                    {
                        wc.enabled = isLocal; // Enable for me, disable for enemies
                    }

                go.name = $"Player_{id}";
                go.transform.localScale = new Vector3(capsuleRadius * 2f, capsuleHeight / 2f, capsuleRadius * 2f);
                _players[id] = go;

                if (isLocal)
                {
                    // Find the temporary camera we made and turn it off
                    // GameObject lobbyCam = GameObject.Find("LobbyCamera");
                    // if (lobbyCam != null) lobbyCam.SetActive(false);

                    // Also try to find any camera tagged "MainCamera" that isn't ours
                    if (Camera.main != null && Camera.main.transform.root != go.transform)
                    {
                        Camera.main.gameObject.SetActive(false);
                    }
                }
            }


            // 2) Movement
            go.transform.position = new Vector3(p.X, p.Y, p.Z);
            if (id != myPlayerId)
            {
                go.transform.rotation = Quaternion.Euler(0f, p.Yaw * Mathf.Rad2Deg, 0f);
            }

            // 3) Visuals from schema (Weapon + Ammo)
            NetworkPlayerSetup visualSetup = go.GetComponent<NetworkPlayerSetup>();
            if (visualSetup != null)
            {
                int weaponIndex = (int)p.Weapon; // WeaponType: none/pistol/rifle/...
                int ammoInMag = (int)p.AmmoInMag;
                int reserveAmmo = (int)p.ReserveAmmo;
                bool isReloading = p.IsReloading;
                int skinIndex = (int)p.SkinId;

                // Use reflection so you can have either:
                // UpdateVisuals(int weapon, int ammo)
                // OR UpdateVisuals(int weapon, int ammo, int reserveAmmo, bool isReloading)
                // var t = visualSetup.GetType();
                // var m4 = t.GetMethod("UpdateVisuals", new[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(int) });
                // if (m4 != null)
                // {
                //     m4.Invoke(visualSetup, new object[] { weaponIndex, ammoInMag, reserveAmmo, isReloading, skinIndex });
                // }
                // else
                // {
                //     var m2 = t.GetMethod("UpdateVisuals", new[] { typeof(int), typeof(int), typeof(int) });
                //     if (m2 != null)
                //         m2.Invoke(visualSetup, new object[] { weaponIndex, ammoInMag, skinIndex });
                // }
                if (id == myPlayerId) Debug.Log($"[NetClient] Server sent Snapshot with weapon: {weaponIndex}, ammo: {ammoInMag}, reserve: {reserveAmmo}, reloading: {isReloading}, skin: {skinIndex}");

                // if (id == myPlayerId) Debug.Log($"[NetClient] Server sent Snapshot with Skin: {skinIndex}");

                visualSetup.UpdateVisuals(weaponIndex, ammoInMag, reserveAmmo, isReloading, skinIndex);
            }

            // 4) Health sync
            int serverHealth = (int)p.Health;
            PlayerHealth hpScript = go.GetComponent<PlayerHealth>();
            if (hpScript != null)
            {
                hpScript.UpdateHealthFromServer(serverHealth);
            }
        }

        // 5) Cleanup disconnected players
        var toRemove = new List<ulong>();
        foreach (var kv in _players)
        {
            if (!alive.Contains(kv.Key))
            {
                if (kv.Value != null) Destroy(kv.Value);
                toRemove.Add(kv.Key);
            }
        }
        foreach (var id in toRemove) _players.Remove(id);
    }

    void ApplyEvents(Snapshot.READER snap)
    {
        if (snap.Events == null) return;

        foreach (var e in snap.Events)
        {
            if (e.which == ServerEvent.WHICH.ShotFired)
            {
                var s = e.ShotFired;
                ulong shooterId = s.ShooterId;

                // Ignore me (local client already played effects instantly)
                if (shooterId == myPlayerId) continue;

                if (_players.TryGetValue(shooterId, out GameObject shooterObj))
                {
                    Transform muzzle = FindGunMuzzle(shooterObj);
                    Vector3 spawnPos = (muzzle != null) ? muzzle.position : shooterObj.transform.position;
                    Quaternion spawnRot = (muzzle != null) ? muzzle.rotation : shooterObj.transform.rotation;

                    if (shootSound != null)
                        AudioSource.PlayClipAtPoint(shootSound, spawnPos);

                    if (muzzleFlashPrefab != null)
                    {
                        GameObject flash = Instantiate(muzzleFlashPrefab, spawnPos, spawnRot);
                        Destroy(flash, 0.5f);
                    }

                    float yawDeg = s.Yaw * Mathf.Rad2Deg;
                    float pitchDeg = -1 * s.Pitch * Mathf.Rad2Deg;
                    Quaternion shotRot = Quaternion.Euler(pitchDeg, yawDeg, 0f);

                    if (bulletPrefab != null)
                        Instantiate(bulletPrefab, spawnPos, shotRot);
                }
            }
        }
    }

    // ---------------- TX ----------------

    // Generic sender (frame pump + big-endian length prefix)
    public void SendClientMsg(MessageBuilder mb)
    {
        if (_stream == null || !_stream.CanWrite) return;

        byte[] payload;
        using (var ms = new MemoryStream())
        {
            var pump = new FramePump(ms);
            pump.Send(mb.Frame);
            payload = ms.ToArray();
        }

        WriteFrameBigEndian(_stream, payload);
    }

    void SendTestInput()
    {
        if (_stream == null || !_stream.CanWrite)
        {
            Debug.LogWarning("[NetClient] SendTestInput: stream not writable");
            return;
        }
        if (myPlayerId == 0)
        {
            Debug.LogWarning("[NetClient] SendTestInput: myPlayerId is 0");
            return;
        }

        // Build ClientMsg { input: ClientInput }
        var mb = MessageBuilder.Create();
        var root = mb.BuildRoot<ClientMsg.WRITER>();

        // NOTE: New schema -> ClientInput has NO PlayerId.
        // Server uses your TCP connection to identify you.
        root.which = ClientMsg.WHICH.Input;
        root.Input.Sequence = 1;
        root.Input.DtMs = 16;

        root.Input.W = false;
        root.Input.A = false;
        root.Input.S = false;
        root.Input.D = false;
        root.Input.Run = false;
        root.Input.JumpPressed = false;
        root.Input.AimYaw = 0.0f;
        root.Input.ShootPressed = false;
        root.Input.ReloadPressed = false;

        byte[] payload;
        using (var ms = new MemoryStream())
        {
            var pump = new FramePump(ms);
            pump.Send(mb.Frame);
            payload = ms.ToArray();
        }

        Debug.Log($"[NetClient] Sending TEST input frame: payloadBytes={payload.Length}");
        WriteFrameBigEndian(_stream, payload);
    }

    void SendInitialClientMessage(CapnpGen.WeaponType pickedWeapon)
    {
        if (_stream == null || !_stream.CanWrite) return;
        if (myPlayerId == 0) return;

        var mb = MessageBuilder.Create();
        var root = mb.BuildRoot<ClientMsg.WRITER>();

        // Use the NEW union variant instead of input
        root.which = ClientMsg.WHICH.SelectSkin;
        root.SelectSkin.PlayerId = myPlayerId;      // keep ONLY if your schema has PlayerId here
        //root.SelectSkin.Weapon = pickedWeapon;

        // frame pump + big endian length prefix (same as your SendClientMsg)
        byte[] payload;
        using (var ms = new MemoryStream())
        {
            var pump = new FramePump(ms);
            pump.Send(mb.Frame);
            payload = ms.ToArray();
        }

        WriteFrameBigEndian(_stream, payload);

        if (log) Debug.Log($"[NetClient] Sent initial SelectWeapon: {pickedWeapon}");
    }

    public void SendInput(
        bool w, bool a, bool s, bool d,
        bool run, bool jumpPressed, float faceYaw,
        float aimYaw, float aimPitch, bool shootPressed, bool reloadPressed,
        ushort dtMs
    )
    {
        if (!isGameStarted) 
        {
            // Send zeros/false so the server sees you standing still
            w = a = s = d = false;
            run = jumpPressed = shootPressed = reloadPressed = false;
        }

        if (_stream == null || !_stream.CanWrite) return;
        if (myPlayerId == 0) return;

        var mb = MessageBuilder.Create();
        var root = mb.BuildRoot<ClientMsg.WRITER>();

        root.which = ClientMsg.WHICH.Input;
        root.Input.Sequence = ++_sequence;
        root.Input.DtMs = dtMs;

        root.Input.W = w;
        root.Input.A = a;
        root.Input.S = s;
        root.Input.D = d;

        root.Input.Run = run;
        root.Input.JumpPressed = jumpPressed;
        root.Input.FaceYaw = faceYaw;
        root.Input.AimYaw = aimYaw;
        root.Input.AimPitch = aimPitch;
        root.Input.ShootPressed = shootPressed;
        root.Input.ReloadPressed = reloadPressed;

        byte[] payload;
        using (var ms = new MemoryStream())
        {
            var pump = new FramePump(ms);
            pump.Send(mb.Frame);
            payload = ms.ToArray();
        }

        WriteFrameBigEndian(_stream, payload);
    }

    // Weapon selection is now a union variant: ClientMsg.selectWeapon
    public void SendSelectSkin(int skinid)
    {
        //skinid = 1;
        if (_stream == null || !_stream.CanWrite) return;
        if (myPlayerId == 0) return;

        var mb = MessageBuilder.Create();
        var root = mb.BuildRoot<ClientMsg.WRITER>();

        root.which = ClientMsg.WHICH.SelectSkin;
        root.SelectSkin.PlayerId = myPlayerId;
        root.SelectSkin.SkinId = (ushort)skinid;

        byte[] payload;
        using (var ms = new MemoryStream())
        {
            var pump = new FramePump(ms);
            pump.Send(mb.Frame);
            payload = ms.ToArray();
        }

        WriteFrameBigEndian(_stream, payload);
    }

    // Call this to tell the server which gun we want
    // public void SendSelectWeapon(int weaponId)
    // {
    //     if (_stream == null || !_stream.CanWrite) return;
    //     if (myPlayerId == 0) return;

    //     var mb = MessageBuilder.Create();
    //     var root = mb.BuildRoot<ClientMsg.WRITER>();

    //     // IMPORTANT: You need a "SelectWeapon" message in your Schema!
    //     // If you don't have one, you might need to reuse an existing field or add it.
    //     // Assuming you added: SelectWeapon @(some_id) :group { playerId @0 :UInt64; weaponId @1 :UInt16; }
        
    //     root.which = ClientMsg.WHICH.SelectWeapon; 
    //     root.SelectWeapon.PlayerId = myPlayerId;
    //     root.SelectWeapon.WeaponId = (ushort)weaponId;

    //     byte[] payload;
    //     using (var ms = new MemoryStream())
    //     {
    //         var pump = new FramePump(ms);
    //         pump.Send(mb.Frame);
    //         payload = ms.ToArray();
    //     }

    //     WriteFrameBigEndian(_stream, payload);
    //     if (log) Debug.Log($"[NetClient] Sent Weapon Request: {weaponId}");
    // }

    // ---------------- FRAMING (BIG-ENDIAN u32) ----------------

    static byte[] ReadFrameBigEndian(NetworkStream s)
    {
        byte[] lenBytes = ReadExact(s, 4);
        if (lenBytes == null) return null;

        int len = (lenBytes[0] << 24) | (lenBytes[1] << 16) | (lenBytes[2] << 8) | (lenBytes[3]);
        if (len <= 0 || len > 20_000_000) throw new Exception($"Invalid frame len={len}");

        return ReadExact(s, len);
    }

    static void WriteFrameBigEndian(NetworkStream s, byte[] payload)
    {
        int len = payload.Length;

        byte[] lenBytes = new byte[4];
        lenBytes[0] = (byte)((len >> 24) & 0xFF);
        lenBytes[1] = (byte)((len >> 16) & 0xFF);
        lenBytes[2] = (byte)((len >> 8) & 0xFF);
        lenBytes[3] = (byte)(len & 0xFF);

        s.Write(lenBytes, 0, 4);
        s.Write(payload, 0, payload.Length);
        s.Flush();
    }

    static byte[] ReadExact(NetworkStream s, int n)
    {
        byte[] buf = new byte[n];
        int off = 0;
        while (off < n)
        {
            int r = s.Read(buf, off, n - off);
            if (r <= 0) return null;
            off += r;
        }
        return buf;
    }

    Transform FindGunMuzzle(GameObject player)
    {
        Transform[] allChildren = player.GetComponentsInChildren<Transform>();
        foreach (Transform t in allChildren)
        {
            if (t.name == "Attack point" && t.gameObject.activeInHierarchy)
            {
                return t;
            }
        }
        return null;
    }
}
