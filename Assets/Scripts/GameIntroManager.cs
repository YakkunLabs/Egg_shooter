using UnityEngine;
using TMPro; // Make sure you use TextMeshPro
using System.Collections;

public class GameIntroManager : MonoBehaviour
{
    [Header("References")]
    public NetClient netClient;
    public TextMeshProUGUI countdownText; // Drag your UI Text here
    public GameObject lobbyCamera;        // Drag the "LobbyCamera" here

    [Header("Settings")]
    public int countdownTime = 3;

    void Start()
    {
        // 1. Lock the game initially
        if (netClient != null) netClient.isGameStarted = false;

        // 2. Ensure Lobby Camera is ON (if you want to see the map before spawning)
        if (lobbyCamera != null) lobbyCamera.SetActive(true);

        // 3. Start the routine
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        // Wait until we are actually connected and assigned an ID
        while (netClient == null || netClient.myPlayerId == 0)
        {
            countdownText.text = "CONNECTING...";
            yield return null;
        }

        // Optional: Wait for the player object to spawn?
        // yield return new WaitForSeconds(1f);

        int timeLeft = countdownTime;

        while (timeLeft > 0)
        {
            countdownText.text = timeLeft.ToString();
            // Play a beep sound here if you want
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }

        // GO!
        countdownText.text = "GO!";
        
        // 4. Unlock the Input
        if (netClient != null) netClient.isGameStarted = true;

        // 5. Hide the Lobby Camera (So Player Camera takes over)
        if (lobbyCamera != null) lobbyCamera.SetActive(false);

        // Hide text after 1 second
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);
    }
}