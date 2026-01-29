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
                // Your Rust read_frame uses BIG-ENDIAN u32 length:
                byte[] payload = ReadFrameBigEndian(_stream);
                if (payload == null) break;

                // Decode payload using the same approach as your working example
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
        // ---- players (what you already do) ----
        ApplyPlayers(snap);

        // ---- events (NEW) ----
        ApplyEvents(snap);
    }

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

                if (isLocal)
                    go = Instantiate(playerPrefab);
                else
                    go = Instantiate(enemyPrefab);

                go.name = $"Player_{id}";
                go.transform.localScale = new Vector3(capsuleRadius * 2f, capsuleHeight / 2f, capsuleRadius * 2f);
                _players[id] = go;
            }

            go.transform.position = new Vector3(p.X, p.Y, p.Z);
            if (id != myPlayerId)
            {
                go.transform.rotation = Quaternion.Euler(0f, p.Yaw * Mathf.Rad2Deg, 0f);
            }
        }

        // Remove players not present anymore
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
            // Your generated ServerEvent has enum WHICH { ShotFired=0, Noop=1 }
            if (e.which == ServerEvent.WHICH.ShotFired)
            {
                var s = e.ShotFired;

                // Example: just log for now
                Debug.Log($"[EVENT] ShotFired shooter={s.ShooterId} pos=({s.X},{s.Y},{s.Z}) yaw={s.Yaw}");

                // Later: spawn muzzle flash / tracer / sound
                // You can also find shooter object by name "Player_<id>"
            }
        }
    }


    // ---------------- TX (optional later) ----------------
    // Here’s the correct writer matching your Rust write_frame (BIG-ENDIAN length).
    // Call this when you start sending ClientMsg.
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

    // ---------------- FRAMING (BIG-ENDIAN u32) ----------------

    static byte[] ReadFrameBigEndian(NetworkStream s)
    {
        byte[] lenBytes = ReadExact(s, 4);
        if (lenBytes == null) return null;

        // BIG-ENDIAN length
        int len = (lenBytes[0] << 24) | (lenBytes[1] << 16) | (lenBytes[2] << 8) | (lenBytes[3]);
        if (len <= 0 || len > 20_000_000) throw new Exception($"Invalid frame len={len}");

        return ReadExact(s, len);
    }

    static void WriteFrameBigEndian(NetworkStream s, byte[] payload)
    {
        int len = payload.Length;

        // BIG-ENDIAN length (matches Rust to_be_bytes)
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

        root.Input.PlayerId = myPlayerId;
        root.Input.Sequence = 1;          // first input
        root.Input.DtMs = 16;             // 16ms

        // All keys false; yaw 0; shoot false
        root.Input.W = false;
        root.Input.A = false;
        root.Input.S = false;
        root.Input.D = false;
        root.Input.Run = false;
        root.Input.JumpPressed = false;
        root.Input.AimYaw = 0.0f;
        root.Input.ShootPressed = false;

        // Serialize capnp to payload bytes (same as your Tileman example)
        byte[] payload;
        using (var ms = new MemoryStream())
        {
            var pump = new FramePump(ms);
            pump.Send(mb.Frame);
            payload = ms.ToArray();
        }

        Debug.Log($"[NetClient] Sending TEST input frame: payloadBytes={payload.Length}");

        // Send as BIG-ENDIAN framed payload to match Rust write_frame
        WriteFrameBigEndian(_stream, payload);
    }


    public void SendInput(
    bool w, bool a, bool s, bool d,
    bool run, bool jumpPressed,
    float aimYaw, bool shootPressed,
    ushort dtMs
)
    {
        if (_stream == null || !_stream.CanWrite) return;
        if (myPlayerId == 0) return;

        var mb = MessageBuilder.Create();
        var root = mb.BuildRoot<ClientMsg.WRITER>();

        root.Input.PlayerId = myPlayerId;
        root.Input.Sequence = ++_sequence;
        root.Input.DtMs = dtMs;

        root.Input.W = w;
        root.Input.A = a;
        root.Input.S = s;
        root.Input.D = d;

        root.Input.Run = run;
        root.Input.JumpPressed = jumpPressed;
        root.Input.AimYaw = aimYaw;
        root.Input.ShootPressed = shootPressed;

        // Serialize capnp → bytes
        byte[] payload;
        using (var ms = new MemoryStream())
        {
            var pump = new FramePump(ms);
            pump.Send(mb.Frame);
            payload = ms.ToArray();
        }

        // BIG-ENDIAN length prefix (matches Rust)
        WriteFrameBigEndian(_stream, payload);
    }


}


