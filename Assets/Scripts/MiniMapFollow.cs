using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    [Header("Settings")]
    public float height = 30f; // How high the camera is
    public bool rotateWithPlayer = false; // Set True if you want the map to spin

    private Transform playerTarget;
    private NetClient netClient;

    void Start()
    {
        netClient = FindFirstObjectByType<NetClient>();
    }

    void LateUpdate()
    {
        // 1. If we don't have a target, try to find the Local Player
        if (playerTarget == null)
        {
            FindLocalPlayer();
            return;
        }

        // 2. Follow the Player (X and Z only, keep Y fixed)
        Vector3 newPos = playerTarget.position;
        newPos.y = height;
        transform.position = newPos;

        // 3. (Optional) Rotate with player
        if (rotateWithPlayer)
        {
            Vector3 rot = transform.eulerAngles;
            rot.y = playerTarget.eulerAngles.y;
            transform.eulerAngles = rot;
        }
    }

    void FindLocalPlayer()
    {
        if (netClient == null) return;
        
        // Wait until we have a valid ID
        if (netClient.myPlayerId == 0) return;

        // Find the object named "Player_ID"
        GameObject playerObj = GameObject.Find($"Player_{netClient.myPlayerId}");
        
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }
}