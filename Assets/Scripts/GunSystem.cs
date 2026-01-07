using UnityEngine;
using TMPro; // Standard Unity Text Library

public class GunSystem : MonoBehaviour
{
    // --- Stats ---
    [Header("Gun Stats")]
    public int damage = 10;
    public float timeBetweenShooting = 0.1f;
    public float spread = 0f;
    public float range = 100f; // (Note: Range is now controlled by Bullet lifetime, but we can keep this)
    public float reloadTime = 1.5f;
    public int magazineSize = 30;
    public int bulletsLeft;
    
    // --- Bools ---
    bool readyToShoot;
    bool reloading;

    // --- References ---
    [Header("References")]
    public Camera fpsCamera;
    public Transform attackPoint; // This is the Fire Point (Tip of gun)
    public GameObject bulletPrefab; // NEW: Drag your "Projectile" prefab here
    public AudioSource audioSource; 

    // --- Graphics ---
    [Header("Graphics")]
    public GameObject impactEffect; // (Optional: You can move this logic to the bullet script later)
    public ParticleSystem muzzleFlash; 

    // --- UI ---
    [Header("UI")]
    public TextMeshProUGUI text_ammo; 

    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>(); 
    }

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        // Check Pause
        if (PauseManager.isPaused) return;

        MyInput();
        
        // Update UI
        if(text_ammo != null)
            text_ammo.SetText(bulletsLeft + " / " + magazineSize);
    }

    private void MyInput()
    {
        if (Input.GetButton("Fire1") && readyToShoot && !reloading && bulletsLeft > 0)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        // 1. Play Sound
        if (audioSource != null)
        {
            audioSource.Play(); 
        }

        // 2. Play Muzzle Flash
        if(muzzleFlash != null)
            muzzleFlash.Play();

        // 3. SPAWN THE BULLET (Replaces Raycast)
        // We spawn it at 'attackPoint' (tip of gun) and give it the gun's rotation
        Instantiate(bulletPrefab, attackPoint.position, attackPoint.rotation);

        // 4. Decrease Ammo
        bulletsLeft--;

        // 5. Reset Shot
        Invoke("ResetShot", timeBetweenShooting);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}