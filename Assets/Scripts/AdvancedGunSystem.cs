using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Advanced Gun System - Robust Reload & Diagnostic Version
/// </summary>
public class AdvancedGunSystem : MonoBehaviour
{
    [Header("Weapon Configuration")]
    public WeaponData weaponData;

    [Header("Runtime Stats")]
    public int currentAmmo;
    public int reserveAmmo;
    public bool isReloading = false;
    public bool readyToShoot = true;
    private FireMode currentFireMode;
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
    public GameObject weaponModel;
    public ParticleSystem muzzleFlash;
    public GameObject scopeOverlay;

    [Header("UI")]
    public TextMeshProUGUI text_ammo;
    public TextMeshProUGUI text_fireMode;
    public Image reloadIndicator;
    public TextMeshProUGUI text_reloading;

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

        // 2. AUTO-FIND RELOAD INDICATOR (Deep Search)
        if (reloadIndicator == null)
        {
            Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            foreach (Canvas c in canvases)
            {
                Image[] imgs = c.GetComponentsInChildren<Image>(true);
                foreach (Image img in imgs)
                {
                    if (img.name == "ReloadIndicator")
                    {
                        reloadIndicator = img;
                        break;
                    }
                }
                if (reloadIndicator != null) break;
            }
        }

        if (text_reloading == null)
        {
            // Try to find it as a child of the indicator first (Best/Fastest way)
            if (reloadIndicator != null)
            {
                text_reloading = reloadIndicator.GetComponentInChildren<TextMeshProUGUI>(true);
            }

            // If still null, search the whole world (Backup plan)
            if (text_reloading == null)
            {
                Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
                foreach (Canvas c in canvases)
                {
                    TextMeshProUGUI[] txts = c.GetComponentsInChildren<TextMeshProUGUI>(true);
                    foreach (TextMeshProUGUI txt in txts)
                    {
                        if (txt.name == "ReloadingText")
                        {
                            text_reloading = txt;
                            goto TextFound; // Break out of all loops
                        }
                    }
                }
            }
        }
        TextFound: // Label to jump to

        // 3. FORCE SETTINGS & HIDE
        if (reloadIndicator != null)
        {
            reloadIndicator.type = Image.Type.Filled; 
            reloadIndicator.fillMethod = Image.FillMethod.Radial360;
            reloadIndicator.gameObject.SetActive(false); 
        }
        else
        {
            Debug.LogError("❌ UI ERROR: Could not find 'ReloadIndicator'. Ensure it is named exactly that.");
        }
        if (text_reloading != null)
        {
            text_reloading.gameObject.SetActive(false);
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
        currentFireMode = weaponData.fireMode;
        initialPosition = transform.localPosition;

        if (fpsCamera != null)
        {
            normalFOV = fpsCamera.fieldOfView;
            currentFOV = normalFOV;
        }
        
        // Force Time to run
        if (Time.timeScale == 0) Time.timeScale = 1f;

        UpdateUI();
    }

    // --- DIAGNOSTIC: CATCH DISABLES ---
    void OnDisable()
    {
        if (isReloading)
        {
            // If you see this error, another script (like NetInput) turned this object off!
            Debug.LogError("⛔ RELOAD CRASHED: The Gun GameObject was disabled/turned off during reload!");
            isReloading = false; 
            if (reloadIndicator != null) reloadIndicator.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu") return;
        if (PauseManager.isPaused) return;

        HandleInput();
        HandleScrollZoom();
        UpdateUI();
    }
    
    // --- PUBLIC METHODS ---

    public void AttemptToShoot()
    {
        if (isReloading) return;
        if (currentAmmo <= 0) 
        {
             PlayEmptySound();
             return;
        }
        if (!readyToShoot) return;
        Shoot(); 
    }

    public void AttemptToReload()
    {
        if (isReloading) return;

        // 1. Check constraints
        if (currentAmmo >= weaponData.magazineSize) return; // Full
        if (reserveAmmo <= 0 && !weaponData.infiniteAmmo) return; // Empty

        // 2. START RELOAD
        StartCoroutine(Reload());
    }

    // ----------------------

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("--- RELOAD STARTED ---");

        // 1. SHOW LOADING UI
        if (reloadIndicator != null)
        {
            reloadIndicator.gameObject.SetActive(true);
            reloadIndicator.fillAmount = 0f;
        }
        if (text_reloading != null)
        {
            text_reloading.gameObject.SetActive(true);
        }

        if (weaponData.hasScope && isScoped) OnUnscoped();

        // 2. PLAY SOUND
        if (audioSource != null && weaponData.reloadSound != null)
        {
            audioSource.pitch = 1f; 
            audioSource.PlayOneShot(weaponData.reloadSound);
        }

        // 3. ANIMATION LOOP
        if (weaponData.reloadFullMagazine)
        {
            float elapsedTime = 0f; 
            float duration = weaponData.reloadTime;
            
            if (duration <= 0.1f) duration = 2.0f; 

            Debug.Log($"Timer Counting... Duration: {duration}");

            while (elapsedTime < duration)
            {
                // FIX: Use unscaledDeltaTime to ignore TimeScale freezes
                elapsedTime += Time.unscaledDeltaTime; 
                
                if (reloadIndicator != null)
                    reloadIndicator.fillAmount = elapsedTime / duration;

                yield return null; 
            }

            Debug.Log("Timer Finished! Refilling Ammo...");

            // Apply Ammo
            int needed = weaponData.magazineSize - currentAmmo;
            int take = weaponData.infiniteAmmo ? needed : Mathf.Min(needed, reserveAmmo);
            currentAmmo += take;
            if (!weaponData.infiniteAmmo) reserveAmmo -= take;
            
            Debug.Log($"Reload Success. Added {take} bullets.");
        }
        else
        {
            // Shotgun Logic
            while (currentAmmo < weaponData.magazineSize && (reserveAmmo > 0 || weaponData.infiniteAmmo))
            {
                float elapsedTime = 0f;
                float duration = weaponData.reloadPerBulletTime;
                if (duration <= 0.05f) duration = 0.5f;

                while (elapsedTime < duration)
                {
                    elapsedTime += Time.unscaledDeltaTime; // FIX HERE TOO
                    if (reloadIndicator != null) reloadIndicator.fillAmount = elapsedTime / duration;
                    yield return null;
                }
                currentAmmo++;
                if (!weaponData.infiniteAmmo) reserveAmmo--;
                UpdateUI();
                if (Input.GetButtonDown("Fire1")) break;
            }
        }

        // 4. HIDE UI
        if (reloadIndicator != null)
        {
            reloadIndicator.gameObject.SetActive(false);
        }
        if (text_reloading != null) text_reloading.gameObject.SetActive(false);

        isReloading = false;
        UpdateUI();
        Debug.Log("--- RELOAD COMPLETE ---");
    }

    private void HandleInput()
    {
        if (weaponData.canToggleFireMode && Input.GetKeyDown(KeyCode.B))
        {
            ToggleFireMode();
        }

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
            case FireMode.BoltAction:
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
        }
    }

    private void FireBullet()
    {
        if (fpsCamera == null) fpsCamera = Camera.main;
        if (fpsCamera == null) return;

        // 1. DETERMINE PELLET COUNT
        // If Shotgun, fire 8 pellets. Otherwise, fire 1.
        int pelletCount = (weaponData.weaponType == WeaponType.Shotgun) ? 8 : 1;

        // 2. PLAY SOUND & FLASH ONCE
        if (audioSource != null && weaponData.shootSound != null)
            audioSource.PlayOneShot(weaponData.shootSound);

        if (muzzleFlash != null && !isScoped)
            muzzleFlash.Play();

        // 3. CALCULATE TARGET POINT (Raycast center of screen)
        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        UpdateAim(ray);

        RaycastHit hit;
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, weaponData.maxRange))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(weaponData.maxRange);

        // 4. LOOP TO SPAWN BULLETS
        for (int i = 0; i < pelletCount; i++)
        {
            float currentSpread = isScoped ? 
                weaponData.spread * weaponData.aimSpreadMultiplier : 
                weaponData.spread;

            // Calculate direction with RANDOM SPREAD for each pellet
            Vector3 directionWithSpread = targetPoint - attackPoint.position;
            directionWithSpread += new Vector3(
                Random.Range(-currentSpread, currentSpread),
                Random.Range(-currentSpread, currentSpread),
                Random.Range(-currentSpread, currentSpread)
            );
            directionWithSpread.Normalize();

            if (bulletPrefab != null)
            {
                Vector3 spawnPos = attackPoint.position + (attackPoint.forward * 0.5f);
                GameObject currentBullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
                currentBullet.transform.forward = directionWithSpread;

                Projectile projectile = currentBullet.GetComponent<Projectile>();
                if (projectile != null)
                {
                    // For shotguns, you might want to divide damage, or keep it per-pellet
                    // If damage is 100 and you fire 8 pellets, that's 800 damage total! 
                    // OPTIONAL: projectile.damage = weaponData.damage / pelletCount;
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
        }

        // 5. DECREASE AMMO ONCE (Per trigger pull, not per pellet)
        currentAmmo--;
        UpdateUI();
        ApplyRecoil();
    }

    private void ApplyRecoil()
    {
        if (fpsCamera != null)
        {
           float r = weaponData.recoilAmount;
           Vector3 recoilRotation = new Vector3(-r, Random.Range(-r/3f, r/3f), 0);
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
            audioSource.PlayOneShot(weaponData.emptySound);
    }

    private void UpdateUI()
    {
        if (text_ammo != null)
            text_ammo.SetText(weaponData.infiniteAmmo ? $"{currentAmmo} / ∞" : $"{currentAmmo} / {reserveAmmo}");
        if (text_fireMode != null)
            text_fireMode.SetText(currentFireMode.ToString());
    }

    public void RefillAmmo()
    {
        reserveAmmo = weaponData.reserveAmmo;
        if (audioSource != null && weaponData.reloadSound != null)
            audioSource.PlayOneShot(weaponData.reloadSound);
        UpdateUI(); 
        Debug.Log("Ammo Refilled to Max: " + reserveAmmo);
    }

    void UpdateAim(Ray camera_ray)
    {
        Vector3 targetPoint;
        if (Physics.Raycast(camera_ray, out var hit, weaponData.maxRange))
            targetPoint = hit.point;
        else
            targetPoint = camera_ray.GetPoint(weaponData.maxRange);

        Vector3 dir = (targetPoint - attackPoint.position).normalized;
        CurrentYaw = Mathf.Atan2(dir.x, dir.z);
        CurrentPitch = Mathf.Asin(dir.y);
    }
}