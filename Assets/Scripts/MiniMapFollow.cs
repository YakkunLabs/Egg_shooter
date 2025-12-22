using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public Transform player;

    void LateUpdate()
    {
        if (player != null)
        {
            // Update position: Follow the player's X and Z, but keep our own Y (height)
            Vector3 newPosition = player.position;
            newPosition.y = transform.position.y; 
            transform.position = newPosition;

            // OPTIONAL: If you want the map to rotate with you, uncomment this line:
            // transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
        }
    }
}