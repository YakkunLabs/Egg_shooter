using Capnp;
using CapnpGen;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

#if UNITY_WEBGL && !UNITY_EDITOR
using NativeWebSocket;
using WS = NativeWebSocket.WebSocket;
#endif

// ✅ Avoid WeaponType name collisions
using WeaponTypeCp = CapnpGen.WeaponType;
using WeaponSlotCp = CapnpGen.WeaponSlot;
using UnityEngine.SocialPlatforms.Impl;
using Unity.VisualScripting;

public class NetClient : MonoBehaviour
{
    [Header("Visuals")]
    public GameObject bulletPrefab;

    [Header("Effects")]
    public GameObject muzzleFlashPrefab;
    public AudioClip shootSound;

    [Header("UI")]
    public TMPro.TextMeshProUGUI playerCountText;

    public bool isGameStarted = false;

    [Header("TCP (Editor/Standalone)")]
    public string host = "127.0.0.1";
    public int port = 9001;

    [Header("WSS (WebGL/Browser)")]
    [Tooltip("Example: wss://your-domain.com/ws  (this should be your WS->TCP proxy endpoint)")]
    public string wssUrl = "wss://your-domain.com/ws";

    [Header("Capsules")]
    public float capsuleHeight = 2f;
    public float capsuleRadius = 0.5f;

    [Header("Debug")]
    public bool log = true;

    uint _sequence = 0;

    readonly ConcurrentQueue<Action> _mainThread = new();
    readonly Dictionary<ulong, GameObject> _players = new();
    readonly Dictionary<ulong, string> _playerNames = new();

    // cache spawns from snapshot (spawnId -> state)
    readonly Dictionary<ushort, WeaponSpawnState.READER> _spawns = new();

    public ulong myPlayerId { get; private set; } = 0;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    INetTransport _net;

    void Start() => Connect();
    void OnDestroy() => Shutdown();

    void Update()
    {
        _net?.Pump();

        while (_mainThread.TryDequeue(out var a))
            a();
    }

    // ---------------- CONNECT ----------------

    public void Connect()
    {
        Shutdown();

#if UNITY_WEBGL && !UNITY_EDITOR
        _net = new WebSocketTransport(wssUrl, log);
#else
        _net = new TcpTransport(host, port, log);
#endif

        _net.OnMessage += OnTransportMessage;
        _net.OnDisconnected += () =>
        {
            if (log) Debug.Log("[NetClient] Disconnected");
        };
        _net.OnError += (err) =>
        {
            Debug.LogError($"[NetClient] Net error: {err}");
        };

        _net.Connect();
    }

    public void Shutdown()
    {
        if (_net != null)
        {
            _net.OnMessage -= OnTransportMessage;
            _net.Dispose();
            _net = null;
        }
    }

    // ---------------- RX DISPATCH ----------------

    void OnTransportMessage(byte[] payload)
    {
        try
        {
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
                    int savedSecondaryWeapon = PlayerPrefs.GetInt("SelectedWeapon", 0);

                    if (log) Debug.Log($"[NetClient] Sending SelectLoadout -> Skin: {savedSkin}, SecondaryWeapon: {savedSecondaryWeapon}");

                    SendSelectLoadout(savedSkin, (WeaponTypeCp)savedSecondaryWeapon);
                    SendTestInput();
                });
            }
            else if (msg.which == ServerMsg.WHICH.Snapshot)
            {
                var snap = msg.Snapshot;
                _mainThread.Enqueue(() => ApplySnapshot(snap));
            }
            // ✅ SAVE NAME WHEN PLAYER JOINS
else if (msg.which == ServerMsg.WHICH.PlayerJoined)
            {
                var joined = msg.PlayerJoined;
                var p = joined.Player;

                // 1. Save Name to Dictionary (For Name Tags)
                lock (_playerNames)
                {
                    _playerNames[p.PlayerId] = p.Name;
                }
                
                Debug.Log($"[NetClient] PlayerJoined -> id={p.PlayerId}, name='{p.Name}'");

                // 2. Trigger UI Notification (For Join Panel)
                _mainThread.Enqueue(() => 
                {
                    if (JoinNotification.Instance != null)
                    {
                        JoinNotification.Instance.ShowMessage(p.Name);
                    }
                    else
                    {
                        // If this prints, check if NotificationManager is ACTIVE in the Hierarchy!
                        Debug.LogError("❌ JoinNotification.Instance is NULL!"); 
                    }
                });
            }
            // ✅ SAVE ALL NAMES FROM ROSTER
            else if (msg.which == ServerMsg.WHICH.Roster)
            {
                var roster = msg.Roster;
                lock (_playerNames)
                {
                    foreach (var p in roster.Players)
                    {
                        _playerNames[p.PlayerId] = p.Name;
                    }
                }
                Debug.Log($"[NetClient] Roster received. Saved {_playerNames.Count} names.");
            }
            else if (msg.which == ServerMsg.WHICH.ScoreUpdate)
            {
                var score = msg.ScoreUpdate.Score;
                Debug.Log($"[NetClient] Score: {score}");
            }
            else if (msg.which == ServerMsg.WHICH.MatchEnded)
            {
                var scores = msg.MatchEnded.Scores;
                foreach (var score in scores)
                {
                    Debug.Log($"[NetClient] Player : {score.PlayerId}  Score: {score.Score}");
                }
            }
            else if (msg.which == ServerMsg.WHICH.PlayerJoined)
            {
                var joined = msg.PlayerJoined;
                var p = joined.Player;

                Debug.Log($"[NetClient] PlayerJoined -> id={p.PlayerId}, name='{p.Name}', skin={p.SkinId}");
            }
            else if (msg.which == ServerMsg.WHICH.Roster)
            {
                var roster = msg.Roster;
                Debug.Log($"[NetClient] Roster received. Player count = {roster.Players.Count}");

                foreach (var p in roster.Players)
                {
                    Debug.Log($"[NetClient] Roster Player -> id={p.PlayerId}, name='{p.Name}', skin={p.SkinId}");
                }
            }
            else if (msg.which == ServerMsg.WHICH.LobbyInfo)
            {
                var info = msg.LobbyInfo;
                Debug.Log($"[NetClient] Players in lobby: {info.PlayerCount}");
            }
            else if (msg.which == ServerMsg.WHICH.ServerFull)
            {
                uint max = msg.ServerFull.MaxPlayers;

                Debug.Log($"[NetClient] Server full. Max players = {max}");

                ShowPopup($"Server is full.\nMax players: {max}");
                Disconnect();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[NetClient] Parse message error: {e}");
        }
    }

    void Disconnect()
    {
        try
        {
            if (_net != null)
            {
                _net.OnMessage -= OnTransportMessage;
                _net.Dispose();          // closes TCP or WSS safely
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[NetClient] Disconnect error: {e.Message}");
        }
        finally
        {
            _net = null;
            myPlayerId = 0;
            isGameStarted = false;

            Debug.Log("[NetClient] Disconnected");
        }
    }


    void ShowPopup(string message)
    {
        Debug.Log($"[POPUP] {message}");
    }

    string GenerateRandomName(int length = 10)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        System.Random rng = new System.Random();

        char[] buffer = new char[length];
        for (int i = 0; i < length; i++)
            buffer[i] = chars[rng.Next(chars.Length)];

        return new string(buffer);
    }

    void ApplySnapshot(Snapshot.READER snap)
    {
        ApplyPlayers(snap);
        ApplySpawns(snap);
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

            if (!_players.TryGetValue(id, out var go) || go == null)
            {
                bool isLocal = (id == myPlayerId);
                go = isLocal ? Instantiate(playerPrefab) : Instantiate(enemyPrefab);

                PlayerWeaponController wc = go.GetComponent<PlayerWeaponController>();
                if (wc != null) wc.enabled = isLocal;

                go.name = $"Player_{id}";
                go.transform.localScale = new Vector3(capsuleRadius * 2f, capsuleHeight / 2f, capsuleRadius * 2f);
                _players[id] = go;

                if (isLocal)
                {
                    if (Camera.main != null && Camera.main.transform.root != go.transform)
                        Camera.main.gameObject.SetActive(false);
                }
            }

            go.transform.position = new Vector3(p.X, p.Y, p.Z);
            if (id != myPlayerId)
                go.transform.rotation = Quaternion.Euler(0f, p.Yaw * Mathf.Rad2Deg, 0f);

            NetworkPlayerSetup visualSetup = go.GetComponent<NetworkPlayerSetup>();
            if (visualSetup != null)
            {
                // ✅ GET NAME FROM DICTIONARY
                string displayName = $"Player {id}"; // Default
                lock (_playerNames)
                {
                    if (_playerNames.ContainsKey(id))
                    {
                        displayName = _playerNames[id];
                    }
                }

                // Send Name to Setup Script
                visualSetup.SetName(displayName);
                
                int primWeapon = (int)p.Primary.Weapon;
                int primAmmo = (int)p.Primary.AmmoInMag;
                int primReserve = (int)p.Primary.ReserveAmmo;

                int secWeapon = (int)p.Secondary.Weapon;
                int secAmmo = (int)p.Secondary.AmmoInMag;
                int secReserve = (int)p.Secondary.ReserveAmmo;

                int equippedSlot = (int)p.EquippedSlot;
                bool isReloading = p.IsReloading;
                int skinIndex = (int)p.SkinId;

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
                    int equippedWeapon = (equippedSlot == (int)WeaponSlotCp.primary) ? primWeapon : secWeapon;
                    int equippedAmmo = (equippedSlot == (int)WeaponSlotCp.primary) ? primAmmo : secAmmo;
                    int equippedReserve = (equippedSlot == (int)WeaponSlotCp.primary) ? primReserve : secReserve;

                    visualSetup.UpdateVisuals(equippedWeapon, equippedAmmo, equippedReserve, isReloading, skinIndex);
                }
            }

            int serverHealth = (int)p.Health;
            PlayerHealth hpScript = go.GetComponent<PlayerHealth>();
            if (hpScript != null) hpScript.UpdateHealthFromServer(serverHealth);
        }

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

        if (playerCountText != null)
        {
            // _players.Count gives the number of active 3D models in the game
            playerCountText.text = $"Players: {_players.Count}";
        }
    }

    // ---------------- SNAPSHOT: SPAWNS ----------------

    void ApplySpawns(Snapshot.READER snap)
    {
        if (snap.Spawns == null) return;
        foreach (var s in snap.Spawns)
            _spawns[(ushort)s.SpawnId] = s;
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
                    float pitchDeg = -1f * s.Pitch * Mathf.Rad2Deg;
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
        if (_net == null || !_net.IsConnected) return;

        byte[] payload;
        using (var ms = new MemoryStream())
        {
            var pump = new FramePump(ms);
            pump.Send(mb.Frame);
            payload = ms.ToArray();
        }

        _net.Send(payload);
    }

    void SendTestInput()
    {
        if (_net == null || !_net.IsConnected) return;
        if (myPlayerId == 0) return;

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

        root.Input.InteractPressed = false;
        root.Input.SwitchWeaponPressed = false;

        SendClientMsg(mb);
        if (log) Debug.Log("[NetClient] Sent TEST input frame");
    }

    public void SendSelectLoadout(int skinId, WeaponTypeCp secondaryWeapon)
    {
        if (_net == null || !_net.IsConnected) return;
        if (myPlayerId == 0) return;

        var mb = MessageBuilder.Create();
        var root = mb.BuildRoot<ClientMsg.WRITER>();

        root.which = ClientMsg.WHICH.SelectLoadout;
        root.SelectLoadout.PlayerId = myPlayerId;

        root.SelectLoadout.SkinId = (ushort)skinId;
        root.SelectLoadout.SecondaryWeapon = secondaryWeapon;

        // Generate random name
        string randomName = GenerateRandomName(10);
        root.SelectLoadout.PlayerName = randomName;

        SendClientMsg(mb);

        if (log) Debug.Log($"[NetClient] Sent SelectLoadout skin={skinId} secondary={secondaryWeapon} name={randomName}");
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

        if (_net == null || !_net.IsConnected) return;
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
        root.Input.AimPitch = -aimPitch;

        root.Input.ShootPressed = shootPressed;
        root.Input.ReloadPressed = reloadPressed;

        root.Input.InteractPressed = interactPressed;
        root.Input.SwitchWeaponPressed = switchWeaponPressed;

        SendClientMsg(mb);
    }

    Transform FindGunMuzzle(GameObject player)
    {
        Transform[] allChildren = player.GetComponentsInChildren<Transform>();
        foreach (Transform t in allChildren)
            if (t.name == "Attack point" && t.gameObject.activeInHierarchy)
                return t;

        return null;
    }

    // ============================================================
    // Transport Abstraction
    // ============================================================

    interface INetTransport : IDisposable
    {
        bool IsConnected { get; }
        event Action<byte[]> OnMessage;
        event Action OnDisconnected;
        event Action<string> OnError;

        void Connect();
        void Send(byte[] payload);

        // ✅ per-frame pump (safe on all platforms)
        void Pump();
    }

    // ---------------- TCP transport (Editor/Standalone) ----------------
    class TcpTransport : INetTransport
    {
        public bool IsConnected => _running && _tcp != null && _tcp.Connected;

        public event Action<byte[]> OnMessage;
        public event Action OnDisconnected;
        public event Action<string> OnError;

        readonly string _host;
        readonly int _port;
        readonly bool _log;

        TcpClient _tcp;
        NetworkStream _stream;
        Thread _rxThread;
        volatile bool _running;

        public TcpTransport(string host, int port, bool log)
        {
            _host = host;
            _port = port;
            _log = log;
        }

        public void Connect()
        {
            try
            {
                _tcp = new TcpClient();
                _tcp.NoDelay = true;
                _tcp.Connect(_host, _port);
                _stream = _tcp.GetStream();

                _running = true;
                _rxThread = new Thread(RxLoop) { IsBackground = true };
                _rxThread.Start();

                if (_log) Debug.Log($"[NetClient] TCP connected {_host}:{_port}");
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.ToString());
            }
        }

        public void Send(byte[] payload)
        {
            if (_stream == null || !_stream.CanWrite) return;
            try
            {
                WriteFrameBigEndian(_stream, payload);
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.ToString());
            }
        }

        public void Pump()
        {
            // TCP uses a background thread -> nothing to do each frame
        }

        void RxLoop()
        {
            try
            {
                while (_running)
                {
                    byte[] payload = ReadFrameBigEndian(_stream);
                    if (payload == null) break;
                    OnMessage?.Invoke(payload);
                }
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.ToString());
            }

            _running = false;
            OnDisconnected?.Invoke();
        }

        public void Dispose()
        {
            _running = false;
            try { _stream?.Close(); } catch { }
            try { _tcp?.Close(); } catch { }
            try { if (_rxThread != null && _rxThread.IsAlive) _rxThread.Join(200); } catch { }
        }

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
    }

    // ---------------- WebSocket transport (WebGL) ----------------
#if UNITY_WEBGL && !UNITY_EDITOR
    class WebSocketTransport : INetTransport
    {
        public bool IsConnected => _ws != null && _ws.State == WebSocketState.Open;

        public event Action<byte[]> OnMessage;
        public event Action OnDisconnected;
        public event Action<string> OnError;

        readonly string _url;
        readonly bool _log;
        WS _ws;

        public WebSocketTransport(string url, bool log)
        {
            _url = url;
            _log = log;
        }

        public async void Connect()
        {
            try
            {
                _ws = new WS(_url);

                _ws.OnOpen += () =>
                {
                    if (_log) Debug.Log($"[NetClient] WSS connected {_url}");
                };

                _ws.OnError += (e) => OnError?.Invoke(e);

                _ws.OnClose += (code) =>
                {
                    if (_log) Debug.Log($"[NetClient] WSS closed code={code}");
                    OnDisconnected?.Invoke();
                };

                _ws.OnMessage += (bytes) =>
                {
                    OnMessage?.Invoke(bytes);
                };

                await _ws.Connect();
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.ToString());
            }
        }

        public async void Send(byte[] payload)
        {
            if (!IsConnected) return;
            try
            {
                await _ws.Send(payload);
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.ToString());
            }
        }

        public void Pump()
        {
            // In WebGL, do NOT call DispatchMessageQueue()
            // WebSocket callbacks are driven by the browser event loop.
        }

        public async void Dispose()
        {
            try
            {
                if (_ws != null)
                {
                    await _ws.Close();
                    _ws = null;
                }
            }
            catch { }
        }
    }
#else
    // Stub for non-WebGL builds (so the file compiles everywhere even without NativeWebSocket)
    class WebSocketTransport : INetTransport
    {
        public bool IsConnected => false;

        public event Action<byte[]> OnMessage;
        public event Action OnDisconnected;
        public event Action<string> OnError;

        public WebSocketTransport(string url, bool log) { }

        public void Connect() => OnError?.Invoke("WebSocketTransport is only used in WebGL builds.");
        public void Send(byte[] payload) { }
        public void Pump() { }
        public void Dispose() { }
    }
#endif


}


