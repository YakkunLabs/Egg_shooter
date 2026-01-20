using UnityEngine;

public class DynamicMusic : MonoBehaviour
{
    [Header("Music Tracks")]
    public AudioClip calmMusic;
    public AudioClip battleMusic;

    [Header("Settings")]
    public float detectionRadius = 15f; // How close enemies must be to trigger battle music
    public LayerMask enemyLayer;        // Which objects count as "Enemies"
    public float fadeSpeed = 0.5f;      // How fast the music switches

    [Header("Audio Sources (Auto-Assigned)")]
    public AudioSource calmSource;
    public AudioSource battleSource;

    private bool isBattleState = false;

    void Start()
    {
        // 1. Setup Calm Source
        calmSource = gameObject.AddComponent<AudioSource>();
        calmSource.clip = calmMusic;
        calmSource.loop = true;
        calmSource.volume = 1f; // Start fully on
        calmSource.Play();

        // 2. Setup Battle Source
        battleSource = gameObject.AddComponent<AudioSource>();
        battleSource.clip = battleMusic;
        battleSource.loop = true;
        battleSource.volume = 0f; // Start silent
        battleSource.Play();
    }

    void Update()
    {
        CheckForEnemies();
        HandleCrossfade();
    }

    void CheckForEnemies()
    {
        // Check if there are any colliders on the "Enemy" layer inside our radius
        // We check every frame, but Physics.CheckSphere is very fast/cheap
        isBattleState = Physics.CheckSphere(transform.position, detectionRadius, enemyLayer);
    }

    void HandleCrossfade()
    {
        if (isBattleState)
        {
            // --- BATTLE MODE ---
            // Fade Calm DOWN, Battle UP
            calmSource.volume = Mathf.MoveTowards(calmSource.volume, 0f, fadeSpeed * Time.deltaTime);
            battleSource.volume = Mathf.MoveTowards(battleSource.volume, 1f, fadeSpeed * Time.deltaTime);
        }
        else
        {
            // --- CALM MODE ---
            // Fade Calm UP, Battle DOWN
            calmSource.volume = Mathf.MoveTowards(calmSource.volume, 1f, fadeSpeed * Time.deltaTime);
            battleSource.volume = Mathf.MoveTowards(battleSource.volume, 0f, fadeSpeed * Time.deltaTime);
        }
    }

    // Visualize the detection range in the Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}