using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Advanced Gun System - Supports multiple fire modes and realistic weapon mechanics
/// Compatible with WeaponData ScriptableObject for easy configuration
/// </summary>
public class AdvancedGunSystem : MonoBehaviour
{
    [Header("Weapon Configuration")]
    public WeaponData weaponData;

    [Header("Runtime Stats")]
    private int currentAmmo;
    private int reserveAmmo;
    private int maxReserveAmmo;
    private FireMode currentFireMode;
    private bool isReloading = false;
    private bool readyToShoot = true;
    private int burstShotsRemaining = 0;
    private Vector3 initialPosition;

    [Header("Scope & Zoom")]
    private bool isScoped = false;
    private float normalFOV = 60f;
    private float currentFOV;

    [Header("References")]
    public Camera fpsCamera;
    public Transform attackPoint;
    public GameObject bulletPrefab;
    public AudioSource audioSource;

    [Header("Graphics")]
    public ParticleSystem muzzleFlash;
    public GameObject scopeOverlay;
    public GameObject weaponModel;

    [Header("UI")]
    public TextMeshProUGUI text_ammo;
    public TextMeshProUGUI text_fireMode;

    [Header("Scroll Zoom Settings")]
    public float maxZoom = 60f;
    public float minZoom = 10f;
    public float zoomSpeed = 30f;

    void Start()
    {
        if (weaponData == null)
        {
            Debug.LogError("WeaponData is not assigned! Please assign a weapon configuration.");
            return;
        }

        // Auto-recover camera if missing
        if (fpsCamera == null) 
        {
            fpsCamera = Camera.main;
            if (fpsCamera != null) Debug.Log("Auto-assigned Main Camera to weapon.");
        }

        // Initialize ammo
        currentAmmo = weaponData.magazineSize;
        reserveAmmo = weaponData.reserveAmmo;
        maxReserveAmmo = weaponData.reserveAmmo;
        currentFireMode = weaponData.fireMode;
        initialPosition = transform.localPosition;

        // Initialize camera settings
        if (fpsCamera != null)
        {
            normalFOV = fpsCamera.fieldOfView;
            currentFOV = normalFOV;
            fpsCamera.fieldOfView = currentFOV;
        }

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        UpdateUI();
    }

    void OnEnable()
    {
        isReloading = false;
        readyToShoot = true;
        isScoped = false;
        if (weaponModel != null) weaponModel.SetActive(true);
        if (scopeOverlay != null) scopeOverlay.SetActive(false);
        UpdateUI();
    }

    void Update()
    {
        // 1. DISABLE IN MAIN MENU
        // Prevents guns from shooting when clicking UI buttons
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu") return;

        if (PauseManager.isPaused) return;

        HandleInput();
        HandleScrollZoom();
        UpdateUI();
    }

    private void HandleInput()
    {
        // Fire Mode Toggle
        if (weaponData.canToggleFireMode && Input.GetKeyDown(KeyCode.B))
        {
            ToggleFireMode();
        }

        // Shooting
        // PC uses Mouse ONLY (checks !IsMobileMode to avoid touch-fire)
        // Mobile uses Button ONLY
        bool pcFire = !MobileInputManager.Instance.IsMobileMode && 
                     (currentFireMode == FireMode.Automatic ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1"));
        
        bool mobileFire = MobileInputManager.Instance.shootPressed;

        if ((pcFire || mobileFire) && readyToShoot && !isReloading && currentAmmo > 0)
        {
            Shoot();
        }

        // Empty click sound
        if (Input.GetButtonDown("Fire1") && currentAmmo <= 0 && !isReloading)
        {
            PlayEmptySound();
        }

        // Reloading
        if ((Input.GetKeyDown(KeyCode.R) || MobileInputManager.Instance.reloadPressed) && currentAmmo < weaponData.magazineSize && !isReloading)
        {
            if (reserveAmmo > 0 || weaponData.infiniteAmmo)
            {
                StartCoroutine(Reload());
            }
        }

        // Sniper Scope Logic
        // PC: Right Click ("Fire2") - Only if !IsMobileMode
        // Mobile: Scope Button Only
        bool pcAim = !MobileInputManager.Instance.IsMobileMode && (Input.GetButton("Fire2") || Input.GetKey(KeyCode.LeftShift));
        bool mobileAim = MobileInputManager.Instance.scopePressed;

        bool isAiming = pcAim || mobileAim;

        if (isAiming)
        {
            // A. SNIPER LOGIC (Scope Overlay)
            if (weaponData.hasScope)
            {
                if (!isScoped) StartCoroutine(OnScoped());
            }
            // B. STANDARD GUN LOGIC (Iron Sights)
            else
            {
                // Move Gun to Center
                transform.localPosition = Vector3.Lerp(transform.localPosition, weaponData.aimPosition, Time.deltaTime * weaponData.aimSpeed);
                // Zoom Camera
                fpsCamera.fieldOfView = Mathf.Lerp(fpsCamera.fieldOfView, weaponData.aimZoom, Time.deltaTime * weaponData.aimSpeed);
            }
        }
        else
        {
            // STOP AIMING
            if (weaponData.hasScope && isScoped)
            {
                OnUnscoped();
            }
            else
            {
                // Return Gun to Hip
                transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * weaponData.aimSpeed);
                // Reset Camera
                fpsCamera.fieldOfView = Mathf.Lerp(fpsCamera.fieldOfView, normalFOV, Time.deltaTime * weaponData.aimSpeed);
            }
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        // Fire mode logic
        switch (currentFireMode)
        {
            case FireMode.SemiAutomatic:
                FireBullet();
                Invoke(nameof(ResetShot), weaponData.timeBetweenShots);
                break;

            case FireMode.Automatic:
                FireBullet();
                Invoke(nameof(ResetShot), weaponData.timeBetweenShots);
                break;

            case FireMode.Burst:
                burstShotsRemaining = weaponData.burstCount;
                StartCoroutine(BurstFire());
                break;

            case FireMode.BoltAction:
                FireBullet();
                Invoke(nameof(ResetShot), weaponData.timeBetweenShots);
                break;
        }
    }

    private void FireBullet()
    {
        // Safety check
        if (fpsCamera == null) fpsCamera = Camera.main;
        if (fpsCamera == null) return; // Can't shoot without camera

        // Calculate target point
        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        // Ignore Player layer (usually layer 3, 6, 7 etc) - Use a mask if needed, for now hit everything
        if (Physics.Raycast(ray, out hit, weaponData.maxRange))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(weaponData.maxRange);
        }

        // Apply spread
        float currentSpread = isScoped ? 
            weaponData.spread * weaponData.aimSpreadMultiplier : 
            weaponData.spread;

        Vector3 directionWithSpread = targetPoint - attackPoint.position;
        directionWithSpread += new Vector3(
            Random.Range(-currentSpread, currentSpread),
            Random.Range(-currentSpread, currentSpread),
            Random.Range(-currentSpread, currentSpread)
        );
        directionWithSpread.Normalize();

        // Play sound
        if (audioSource != null && weaponData.shootSound != null)
        {
            audioSource.PlayOneShot(weaponData.shootSound);
        }

        // Muzzle flash (only if not scoped)
        if (muzzleFlash != null && !isScoped)
        {
            muzzleFlash.Play();
        }

        // Spawn bullet - Offset slightly to avoid clipping
        if (bulletPrefab != null)
        {
            // Move spawn point forward by 0.5 units if not scoped (or less if scoped)
            Vector3 spawnPos = attackPoint.position + (attackPoint.forward * 0.5f);
            
            GameObject currentBullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            
            // Apply direction
            currentBullet.transform.forward = directionWithSpread;

            // Set bullet properties
            Projectile projectile = currentBullet.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.damage = weaponData.damage;
                projectile.speed = weaponData.bulletSpeed > 0 ? weaponData.bulletSpeed : 100f; 
                projectile.explosionRadius = weaponData.explosionRadius; // Set explosion radius
            }
            
            // Ignore collision with Player to prevent self-damage
            Collider bulletCollider = currentBullet.GetComponent<Collider>();
            Collider playerCollider = GameObject.FindWithTag("Player")?.GetComponent<Collider>(); // Try to find player
            if (bulletCollider != null && playerCollider != null)
            {
                Physics.IgnoreCollision(bulletCollider, playerCollider);
            }
        }

        // Consume ammo
        currentAmmo--;
        UpdateUI();
        ApplyRecoil();
    }

    private void ApplyRecoil()
    {
        if (fpsCamera != null)
        {
           float recoil = weaponData.recoilAmount;
           Vector3 recoilRotation = new Vector3(-recoil, Random.Range(-recoil/3f, recoil/3f), 0);
           fpsCamera.transform.localEulerAngles += recoilRotation;
        }
    }

    private IEnumerator BurstFire()
    {
        while (burstShotsRemaining > 0 && currentAmmo > 0)
        {
            FireBullet();
            burstShotsRemaining--;
            yield return new WaitForSeconds(weaponData.burstDelay);
        }

        Invoke(nameof(ResetShot), weaponData.timeBetweenShots);
    }

    private IEnumerator Reload()
    {
        isReloading = true;

        // Exit scope if scoped
        if (weaponData.hasScope && isScoped)
        {
            OnUnscoped();
        }

        // Play reload sound
        if (audioSource != null && weaponData.reloadSound != null)
        {
            audioSource.PlayOneShot(weaponData.reloadSound);
        }

        if (weaponData.reloadFullMagazine)
        {
            // Standard reload (full magazine at once)
            yield return new WaitForSeconds(weaponData.reloadTime);

            int ammoNeeded = weaponData.magazineSize - currentAmmo;
            int ammoToReload = weaponData.infiniteAmmo ? ammoNeeded : Mathf.Min(ammoNeeded, reserveAmmo);

            currentAmmo += ammoToReload;
            if (!weaponData.infiniteAmmo)
            {
                reserveAmmo -= ammoToReload;
            }
        }
        else
        {
            // Reload one bullet at a time (shotgun style)
            while (currentAmmo < weaponData.magazineSize && (reserveAmmo > 0 || weaponData.infiniteAmmo))
            {
                yield return new WaitForSeconds(weaponData.reloadPerBulletTime);
                currentAmmo++;
                if (!weaponData.infiniteAmmo)
                {
                    reserveAmmo--;
                }

                // Allow interrupting reload
                if (Input.GetButtonDown("Fire1"))
                {
                    break;
                }
            }
        }

        isReloading = false;
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void ToggleFireMode()
    {
        if (weaponData.availableFireModes == null || weaponData.availableFireModes.Length <= 1)
            return;

        int currentIndex = System.Array.IndexOf(weaponData.availableFireModes, currentFireMode);
        currentIndex = (currentIndex + 1) % weaponData.availableFireModes.Length;
        currentFireMode = weaponData.availableFireModes[currentIndex];

        Debug.Log($"Fire Mode: {currentFireMode}");
    }

    private IEnumerator OnScoped()
    {
        isScoped = true;
        yield return new WaitForSeconds(0.15f);

        if (scopeOverlay != null) scopeOverlay.SetActive(true);
        if (weaponModel != null) weaponModel.SetActive(false);
        if (fpsCamera != null) fpsCamera.fieldOfView = weaponData.scopedFOV;
    }

    private void OnUnscoped()
    {
        isScoped = false;

        if (scopeOverlay != null) scopeOverlay.SetActive(false);
        if (weaponModel != null) weaponModel.SetActive(true);
        if (fpsCamera != null) fpsCamera.fieldOfView = currentFOV;
    }

    private void HandleScrollZoom()
    {
        if (fpsCamera == null) return;

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            if (isScoped && weaponData.hasScope)
            {
                float scopedFOV = weaponData.scopedFOV;
                scopedFOV -= scrollInput * weaponData.scopeZoomSpeed * 5f;
                scopedFOV = Mathf.Clamp(scopedFOV, 1f, 30f);
                weaponData.scopedFOV = scopedFOV;
                fpsCamera.fieldOfView = scopedFOV;
            }
            else
            {
                currentFOV -= scrollInput * zoomSpeed * 10f;
                currentFOV = Mathf.Clamp(currentFOV, minZoom, maxZoom);
                fpsCamera.fieldOfView = currentFOV;
            }
        }
    }

    private void PlayEmptySound()
    {
        if (audioSource != null && weaponData.emptySound != null)
        {
            audioSource.PlayOneShot(weaponData.emptySound);
        }
    }

    private void UpdateUI()
    {
        if (text_ammo != null)
        {
            if (weaponData.infiniteAmmo)
            {
                text_ammo.SetText($"{currentAmmo} / ∞");
            }
            else
            {
                text_ammo.SetText($"{currentAmmo} / {reserveAmmo}");
            }
        }

        if (text_fireMode != null)
        {
            text_fireMode.SetText(currentFireMode.ToString());
        }
    }

    // --- NEW REFILL FUNCTION ---
    public void RefillAmmo()
    {
        // Simply reset the reserve to the maximum allowed
        reserveAmmo = maxReserveAmmo;

        // Optional: Play pickup sound
        if (audioSource != null && weaponData.reloadSound != null)
        {
            audioSource.PlayOneShot(weaponData.reloadSound);
        }

        UpdateUI(); 
        Debug.Log("Ammo Refilled to Max: " + reserveAmmo);
    }
}
