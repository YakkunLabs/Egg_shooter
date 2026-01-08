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
    public AudioClip reloadSound;

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

        // --- NEW ACCURACY LOGIC ---
        // 1. Find exactly what the Crosshair is looking at
        // (0.5, 0.5) is the exact center of the screen
        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); 
        RaycastHit hit;

        Vector3 targetPoint;

        // 2. Check if the ray hits anything (Enemy, Wall, Floor)
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point; // Aim at the hit point
        }
        else
        {
            targetPoint = ray.GetPoint(75); // Hit nothing? Aim at a point far away in the air
        }

        // 3. Calculate direction: Target Position - Gun Barrel Position
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;
        // ---------------------------

        // 4. Play Sound
        if (audioSource != null)
        {
            audioSource.Play();
        }

        // 5. Play Muzzle Flash
        if (muzzleFlash != null)
            muzzleFlash.Play();

        // 6. SPAWN THE BULLET
        // We spawn it at 'attackPoint' (tip of gun) but use Quaternion.identity (no rotation yet)
        GameObject currentBullet = Instantiate(bulletPrefab, attackPoint.position, Quaternion.identity);

        // 7. ROTATE BULLET TO FACE TARGET
        // This is the magic line that makes it fly towards the crosshair
        currentBullet.transform.forward = directionWithoutSpread.normalized;

        // 8. Decrease Ammo & Reset
        bulletsLeft--;
        Invoke("ResetShot", timeBetweenShooting);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        // 1. Play the Sound
        if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}