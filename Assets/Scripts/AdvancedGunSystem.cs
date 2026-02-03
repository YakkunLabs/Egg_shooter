using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Advanced Gun System - Updated for Network Client Architecture
/// </summary>
public class AdvancedGunSystem : MonoBehaviour
{
    [Header("Weapon Configuration")]
    public WeaponData weaponData;

    [Header("Runtime Stats")]
    public int currentAmmo;
    public int reserveAmmo;
    private int maxReserveAmmo;
    private FireMode currentFireMode;
    public bool isReloading = false;
    public bool readyToShoot = true;
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
    public Image reloadIndicator;

    [Header("Scroll Zoom Settings")]
    public float maxZoom = 60f;
    public float minZoom = 10f;
    public float zoomSpeed = 30f;

    public float CurrentYaw { get; private set; }
    public float CurrentPitch { get; private set; }

    void Start()
    {
        // 1. AUTO-FIND TEXT
        if (text_ammo == null)
        {
            GameObject uiObj = GameObject.Find("AmmoText");
            if (uiObj != null) text_ammo = uiObj.GetComponent<TextMeshProUGUI>();
        }

        if (text_fireMode == null)
        {
            GameObject uiObj = GameObject.Find("FireModeText");
            if (uiObj != null) text_fireMode = uiObj.GetComponent<TextMeshProUGUI>();
        }

        // 2. AUTO-FIND RELOAD INDICATOR (Robust Deep Search)
        if (reloadIndicator == null)
        {
            // Try standard search first
            GameObject directObj = GameObject.Find("ReloadIndicator");
            if (directObj != null)
            {
                reloadIndicator = directObj.GetComponent<Image>();
            }
            else
            {
                // Fallback: Search deeply inside the Canvas (includes disabled objects)
                Canvas canvas = FindFirstObjectByType<Canvas>();
                if (canvas != null)
                {
                    // "true" means include inactive (hidden) objects
                    Image[] allImages = canvas.GetComponentsInChildren<Image>(true);
                    foreach (Image img in allImages)
                    {
                        if (img.name == "ReloadIndicator")
                        {
                            reloadIndicator = img;
                            break; // Found it!
                        }
                    }
                }
            }
        }

        // 3. FORCE SETTINGS & HIDE
        if (reloadIndicator != null)
        {
            // Ensure the image settings are correct via code
            reloadIndicator.type = Image.Type.Filled; 
            reloadIndicator.fillMethod = Image.FillMethod.Radial360;
            reloadIndicator.gameObject.SetActive(false); 
        }
        else
        {
            Debug.LogError("❌ UI ERROR: Could not find 'ReloadIndicator'. Ensure it is named exactly that and is a child of the Canvas.");
        }

        // 4. SETUP WEAPON
        if (weaponData == null)
        {
            Debug.LogError("⛔ WeaponData is missing!");
            return;
        }

        if (fpsCamera == null) fpsCamera = Camera.main;
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        currentAmmo = weaponData.magazineSize;
        reserveAmmo = weaponData.reserveAmmo;
        maxReserveAmmo = weaponData.reserveAmmo;
        currentFireMode = weaponData.fireMode;
        initialPosition = transform.localPosition;

        if (fpsCamera != null)
        {
            normalFOV = fpsCamera.fieldOfView;
            currentFOV = normalFOV;
        }
        
        // Force Time to run (Fixes stuck timers)
        Time.timeScale = 1f;

        UpdateUI();
    }
    
    void OnEnable()
    {
        isReloading = false;
        readyToShoot = true;
        isScoped = false;
        if (weaponModel != null) weaponModel.SetActive(true);
        if (scopeOverlay != null) scopeOverlay.SetActive(false);
        if (reloadIndicator != null) reloadIndicator.gameObject.SetActive(false);
        UpdateUI();
    }

    void Update()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu") return;
        if (PauseManager.isPaused) return;

        HandleInput();
        HandleScrollZoom();
        UpdateUI();
    }

    // --- NEW: PUBLIC METHODS FOR INPUT SCRIPT ---
    
    // Called by NetInputFromEggController
    public void AttemptToShoot()
    {
        // 1. Check conditions locally
        if (isReloading) return;
        
        if (currentAmmo <= 0) 
        {
             PlayEmptySound();
             return;
        }
        
        if (!readyToShoot) return;

        // 2. Fire!
        Shoot(); 
    }

    // Called by NetInputFromEggController
public void AttemptToReload()
    {
        if (isReloading) return;

        // 1. Check constraints
        if (currentAmmo >= weaponData.magazineSize) 
        {
            Debug.Log("Fail: Mag Full");
            return;
        }

        if (reserveAmmo <= 0 && !weaponData.infiniteAmmo) 
        {
            Debug.Log("Fail: No Reserve Ammo");
            return;
        }

        // 2. FORCE INSTANT RELOAD (Bypassing Coroutine for testing)
        Debug.Log("Forcing Instant Reload...");

        // int ammoNeeded = weaponData.magazineSize - currentAmmo;
        
        // // Take what we can from reserve
        // int ammoToTake = weaponData.infiniteAmmo ? ammoNeeded : Mathf.Min(ammoNeeded, reserveAmmo);

        // currentAmmo += ammoToTake;
        // if (!weaponData.infiniteAmmo) reserveAmmo -= ammoToTake;

        // Debug.Log($"Reloaded {ammoToTake} bullets. Current: {currentAmmo}, Reserve: {reserveAmmo}");

        // 3. Update UI
        // UpdateUI();

        StartCoroutine(Reload());
    }
    // --------------------------------------------

    private void HandleInput()
    {
        // Fire Mode Toggle (Local Only)
        if (weaponData.canToggleFireMode && Input.GetKeyDown(KeyCode.B))
        {
            ToggleFireMode();
        }

        // --- DISABLED INTERNAL SHOOTING ---
        // We commented this out because 'NetInputFromEggController' now handles 
        // calling AttemptToShoot(). If we leave this, you will shoot twice!
        /*
        bool pcFire = !MobileInputManager.Instance.IsMobileMode && 
                     (currentFireMode == FireMode.Automatic ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1"));
        
        bool mobileFire = MobileInputManager.Instance.shootPressed;

        if ((pcFire || mobileFire) && readyToShoot && !isReloading && currentAmmo > 0)
        {
            Shoot();
        }

        if (Input.GetButtonDown("Fire1") && currentAmmo <= 0 && !isReloading)
        {
            PlayEmptySound();
        }
        */

        // --- DISABLED INTERNAL RELOADING ---
        // Handled by NetInputFromEggController calling AttemptToReload()
        /*
        if ((Input.GetKeyDown(KeyCode.R) || MobileInputManager.Instance.reloadPressed) && currentAmmo < weaponData.magazineSize && !isReloading)
        {
            if (reserveAmmo > 0 || weaponData.infiniteAmmo)
            {
                StartCoroutine(Reload());
            }
        }
        */

        // Sniper Scope Logic (Visuals stay local)
        bool pcAim = !MobileInputManager.Instance.IsMobileMode && (Input.GetButton("Fire2") || Input.GetKey(KeyCode.LeftShift));
        bool mobileAim = MobileInputManager.Instance.scopePressed;
        bool isAiming = pcAim || mobileAim;

        if (isAiming)
        {
            if (weaponData.hasScope)
            {
                if (!isScoped) StartCoroutine(OnScoped());
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, weaponData.aimPosition, Time.deltaTime * weaponData.aimSpeed);
                fpsCamera.fieldOfView = Mathf.Lerp(fpsCamera.fieldOfView, weaponData.aimZoom, Time.deltaTime * weaponData.aimSpeed);
            }
        }
        else
        {
            if (weaponData.hasScope && isScoped)
            {
                OnUnscoped();
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * weaponData.aimSpeed);
                fpsCamera.fieldOfView = Mathf.Lerp(fpsCamera.fieldOfView, normalFOV, Time.deltaTime * weaponData.aimSpeed);
            }
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

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
        if (fpsCamera == null) fpsCamera = Camera.main;
        if (fpsCamera == null) return;

        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        UpdateAim(ray);

        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, weaponData.maxRange))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(weaponData.maxRange);
        }

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

        if (audioSource != null && weaponData.shootSound != null)
        {
            audioSource.PlayOneShot(weaponData.shootSound);
        }

        if (muzzleFlash != null && !isScoped)
        {
            muzzleFlash.Play();
        }

        if (bulletPrefab != null)
        {
            Vector3 spawnPos = attackPoint.position + (attackPoint.forward * 0.5f);
            
            GameObject currentBullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            
            currentBullet.transform.forward = directionWithSpread;

            Projectile projectile = currentBullet.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.damage = weaponData.damage;
                projectile.speed = weaponData.bulletSpeed > 0 ? weaponData.bulletSpeed : 100f; 
                projectile.explosionRadius = weaponData.explosionRadius; 
            }
            
            Collider bulletCollider = currentBullet.GetComponent<Collider>();
            Collider playerCollider = GameObject.FindWithTag("Player")?.GetComponent<Collider>();
            if (bulletCollider != null && playerCollider != null)
            {
                Physics.IgnoreCollision(bulletCollider, playerCollider);
            }
        }

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

        // 1. SHOW LOADING UI
        if (reloadIndicator != null)
        {
            reloadIndicator.gameObject.SetActive(true);
            reloadIndicator.fillAmount = 0f;
        }

        if (weaponData.hasScope && isScoped)
        {
            OnUnscoped();
        }

        // 2. PLAY SOUND
        if (audioSource != null && weaponData.reloadSound != null)
        {
            audioSource.pitch = 1f; // Reset pitch
            audioSource.PlayOneShot(weaponData.reloadSound);
        }

        if (weaponData.reloadFullMagazine)
        {
            // --- STANDARD RELOAD ---
            float elapsedTime = 3.5f;
            float duration = weaponData.reloadTime;

            // SAFETY CHECK: If duration is 0, force it to 2 seconds so UI works
            if (duration <= 0.1f) duration = 2.0f; 

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                
                if (reloadIndicator != null)
                    reloadIndicator.fillAmount = elapsedTime / duration;

                yield return null; 
            }
            // -----------------------------
            

            int ammoNeeded = weaponData.magazineSize - currentAmmo;
            int ammoToReload = weaponData.infiniteAmmo ? ammoNeeded : Mathf.Min(ammoNeeded, reserveAmmo);

            currentAmmo += ammoToReload;
            if (!weaponData.infiniteAmmo) reserveAmmo -= ammoToReload;
        }
        else
        {
            // --- SHOTGUN RELOAD ---
            while (currentAmmo < weaponData.magazineSize && (reserveAmmo > 0 || weaponData.infiniteAmmo))
            {
                float elapsedTime = 0f;
                float duration = weaponData.reloadPerBulletTime;

                // SAFETY CHECK
                if (duration <= 0.05f) duration = 0.5f;

                while (elapsedTime < duration)
                {
                    elapsedTime += Time.deltaTime;
                    if (reloadIndicator != null) reloadIndicator.fillAmount = elapsedTime / duration;
                    yield return null;
                }

                currentAmmo++;
                if (!weaponData.infiniteAmmo) reserveAmmo--;

                UpdateUI(); 

                if (Input.GetButtonDown("Fire1")) break;
            }
        }

        // 3. HIDE LOADING UI
        if (reloadIndicator != null)
        {
            reloadIndicator.gameObject.SetActive(false);
        }

        isReloading = false;
        UpdateUI(); 
    }
    // Changed to Public for the Input Script
    public void ResetShot()
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

    public void RefillAmmo()
    {
        reserveAmmo = maxReserveAmmo;
        if (audioSource != null && weaponData.reloadSound != null)
        {
            audioSource.PlayOneShot(weaponData.reloadSound);
        }
        UpdateUI(); 
        Debug.Log("Ammo Refilled to Max: " + reserveAmmo);
    }

    void UpdateAim(Ray camera_ray)
    {
        Ray ray = camera_ray;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out var hit, weaponData.maxRange))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(weaponData.maxRange);

        Vector3 dir = (targetPoint - attackPoint.position).normalized;

        CurrentYaw = Mathf.Atan2(dir.x, dir.z);
        CurrentPitch = Mathf.Asin(dir.y);
    }
}