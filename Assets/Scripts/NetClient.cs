using Capnp;
using CapnpGen;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

// ✅ Avoid WeaponType name collisions
using WeaponTypeCp = CapnpGen.WeaponType;
using WeaponSlotCp = CapnpGen.WeaponSlot;

public class NetClient : MonoBehaviour
{
    [Header("Visuals")]
    public GameObject bulletPrefab;

    [Header("Effects")]
    public GameObject muzzleFlashPrefab;
    public AudioClip shootSound;

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

    // NEW: cache spawns from snapshot (spawnId -> available/weapon)
    readonly Dictionary<ushort, WeaponSpawnState.READER> _spawns = new();

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

                        // IMPORTANT:
                        // Secondary weapon selection should map to CapnpGen.WeaponType values:
                        // 0=none, 1=pistol, 2=rifle, 3=smg, 4=shotgun, 5=sniper (based on your schema)
                        int savedSecondaryWeapon = PlayerPrefs.GetInt("SelectedWeapon", 0);

                        if (log) Debug.Log($"[NetClient] Sending SelectLoadout -> Skin: {savedSkin}, SecondaryWeapon: {savedSecondaryWeapon}");

                        SendSelectLoadout(savedSkin, (WeaponTypeCp)savedSecondaryWeapon);

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
        ApplySpawns(snap);   // NEW
        ApplyEvents(snap);
    }

    // ---------------- SNAPSHOT: PLAYERS ----------------

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

                // Enable weapon controller only for local player
                PlayerWeaponController wc = go.GetComponent<PlayerWeaponController>();
                if (wc != null)
                    wc.enabled = isLocal;

                go.name = $"Player_{id}";
                go.transform.localScale = new Vector3(capsuleRadius * 2f, capsuleHeight / 2f, capsuleRadius * 2f);
                _players[id] = go;

                if (isLocal)
                {
                    // Disable any other main camera that isn't our player camera
                    if (Camera.main != null && Camera.main.transform.root != go.transform)
                        Camera.main.gameObject.SetActive(false);
                }
            }

            // 2) Movement
            go.transform.position = new Vector3(p.X, p.Y, p.Z);
            if (id != myPlayerId)
            {
                go.transform.rotation = Quaternion.Euler(0f, p.Yaw * Mathf.Rad2Deg, 0f);
            }

            // 3) Visuals from NEW schema (primary+secondary+equipped)
            NetworkPlayerSetup visualSetup = go.GetComponent<NetworkPlayerSetup>();
            if (visualSetup != null)
            {
                // NEW fields:
                // p.Primary (WeaponSlotState)
                // p.Secondary (WeaponSlotState)
                // p.EquippedSlot (WeaponSlot)
                // p.IsReloading
                // p.SkinId
                int primWeapon = (int)p.Primary.Weapon;
                int primAmmo = (int)p.Primary.AmmoInMag;
                int primReserve = (int)p.Primary.ReserveAmmo;

                int secWeapon = (int)p.Secondary.Weapon;
                int secAmmo = (int)p.Secondary.AmmoInMag;
                int secReserve = (int)p.Secondary.ReserveAmmo;

                int equippedSlot = (int)p.EquippedSlot;
                bool isReloading = p.IsReloading;
                int skinIndex = (int)p.SkinId;

                // Call updated method if available:
                // UpdateVisuals(int primW,int primA,int primR,int secW,int secA,int secR,int equippedSlot,bool reload,int skin)
                var t = visualSetup.GetType();
                var mNew = t.GetMethod("UpdateVisuals", new[] {
                    typeof(int), typeof(int), typeof(int),
                    typeof(int), typeof(int), typeof(int),
                    typeof(int), typeof(bool), typeof(int)
                });

                if (mNew != null)
                {
                    mNew.Invoke(visualSetup, new object[] {
                        primWeapon, primAmmo, primReserve,
                        secWeapon, secAmmo, secReserve,
                        equippedSlot, isReloading, skinIndex
                    });
                }
                else
                {
                    // Fallback to your old UpdateVisuals(weapon, ammo, reserve, reloading, skin)
                    int equippedWeapon = (equippedSlot == (int)WeaponSlotCp.primary) ? primWeapon : secWeapon;
                    int equippedAmmo = (equippedSlot == (int)WeaponSlotCp.primary) ? primAmmo : secAmmo;
                    int equippedReserve = (equippedSlot == (int)WeaponSlotCp.primary) ? primReserve : secReserve;

                    visualSetup.UpdateVisuals(equippedWeapon, equippedAmmo, equippedReserve, isReloading, skinIndex);
                }

                if (id == myPlayerId && log)
                {
                    Debug.Log($"[NetClient] Snapshot Loadout prim={primWeapon}({primAmmo}/{primReserve}) " +
                              $"sec={secWeapon}({secAmmo}/{secReserve}) equippedSlot={equippedSlot} reload={isReloading} skin={skinIndex}");
                }
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

    // ---------------- SNAPSHOT: SPAWNS ----------------

    void ApplySpawns(Snapshot.READER snap)
    {
        // If your generated code uses a different property name, rename here.
        // We expect: snap.Spawns : IReadOnlyList<WeaponSpawnState.READER>
        if (snap.Spawns == null) return;

        foreach (var s in snap.Spawns)
        {
            _spawns[(ushort)s.SpawnId] = s;

            // If you have spawn visuals in the scene, update them by spawnId here.
            // Example:
            // WeaponSpawnViewRegistry.SetState((ushort)s.SpawnId, s.Available, (int)s.Weapon);
        }
    }

    // ---------------- SNAPSHOT: EVENTS ----------------

    void ApplyEvents(Snapshot.READER snap)
    {
        if (snap.Events == null) return;

        foreach (var e in snap.Events)
        {
            if (e.which == ServerEvent.WHICH.ShotFired)
            {
                var s = e.ShotFired;
                ulong shooterId = s.ShooterId;

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
                    float pitchDeg = -1f * s.Pitch * Mathf.Rad2Deg; // keep your invert fix
                    Quaternion shotRot = Quaternion.Euler(pitchDeg, yawDeg, 0f);

                    if (bulletPrefab != null)
                        Instantiate(bulletPrefab, spawnPos, shotRot);
                }
            }
        }
    }

    // ---------------- TX ----------------

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

        var mb = MessageBuilder.Create();
        var root = mb.BuildRoot<ClientMsg.WRITER>();

        root.which = ClientMsg.WHICH.Input;
        root.Input.Sequence = 1;
        root.Input.DtMs = 16;

        root.Input.W = false;
        root.Input.A = false;
        root.Input.S = false;
        root.Input.D = false;
        root.Input.Run = false;

        root.Input.JumpPressed = false;
        root.Input.FaceYaw = 0.0f;

        root.Input.AimYaw = 0.0f;
        root.Input.AimPitch = 0.0f;

        root.Input.ShootPressed = false;
        root.Input.ReloadPressed = false;

        // NEW fields in schema
        root.Input.InteractPressed = false;
        root.Input.SwitchWeaponPressed = false;

        SendClientMsg(mb);

        if (log) Debug.Log("[NetClient] Sent TEST input frame");
    }

    // ✅ NEW: One-time config message (skin + initial secondary weapon)
    public void SendSelectLoadout(int skinId, WeaponTypeCp secondaryWeapon)
    {
        if (_stream == null || !_stream.CanWrite) return;
        if (myPlayerId == 0) return;

        var mb = MessageBuilder.Create();
        var root = mb.BuildRoot<ClientMsg.WRITER>();

        root.which = ClientMsg.WHICH.SelectLoadout;

        // server ignores this and trusts TCP connection, but we set it anyway (matches schema)
        root.SelectLoadout.PlayerId = myPlayerId;

        root.SelectLoadout.SkinId = (ushort)skinId;
        root.SelectLoadout.SecondaryWeapon = secondaryWeapon;

        SendClientMsg(mb);

        if (log) Debug.Log($"[NetClient] Sent SelectLoadout skin={skinId} secondary={secondaryWeapon}");
    }

    public void SendInput(
        bool w, bool a, bool s, bool d,
        bool run, bool jumpPressed, float faceYaw,
        float aimYaw, float aimPitch,
        bool shootPressed, bool reloadPressed,
        bool interactPressed, bool switchWeaponPressed,
        ushort dtMs
    )
    {
        if (!isGameStarted)
        {
            w = a = s = d = false;
            run = jumpPressed = shootPressed = reloadPressed = false;
            interactPressed = false;
            switchWeaponPressed = false;
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

        // NEW
        root.Input.InteractPressed = interactPressed;
        root.Input.SwitchWeaponPressed = switchWeaponPressed;

        SendClientMsg(mb);
    }

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
                return t;
        }
        return null;
    }
}
