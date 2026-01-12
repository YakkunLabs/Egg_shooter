using UnityEngine;
using TMPro;
using System.Collections;

public class GunSystem : MonoBehaviour
{
    // --- Gun Type Settings ---
    [Header("Gun Type Settings")]
    public bool isSniper = false;
    public float scopedFOV = 15f; 
    private float normalFOV = 60f;

    // --- Stats ---
    [Header("Gun Stats")]
    public int damage = 10;
    public float timeBetweenShooting = 0.1f;
    public float spread = 0f;
    public float reloadTime = 1.5f;
    public int magazineSize = 30;
    public int bulletsLeft;

    // --- Scroll Zoom Settings ---
    [Header("Scroll Zoom Settings")]
    public float maxZoom = 60f; 
    public float minZoom = 10f; // Lowered this so you can zoom deep
    public float zoomSpeed = 30f; 
    private float currentFOV;

    // --- Bools ---
    bool readyToShoot;
    bool reloading;
    private bool isScoped = false; 

    // --- References ---
    [Header("References")]
    public Camera fpsCamera;
    public Transform attackPoint;
    public GameObject bulletPrefab;
    public AudioSource audioSource; 
    public AudioClip reloadSound;

    // --- Graphics & Scope ---
    [Header("Graphics")]
    public GameObject impactEffect; 
    public ParticleSystem muzzleFlash;
    public GameObject scopeOverlay; 
    public GameObject weaponModel; 

    // --- UI ---
    [Header("UI")]
    public TextMeshProUGUI text_ammo; 

    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>(); 

        if (fpsCamera != null)
        {
            currentFOV = maxZoom;
            normalFOV = maxZoom;
            fpsCamera.fieldOfView = currentFOV;
        }
    }

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        if (PauseManager.isPaused) return;

        MyInput();

        // 1. ALWAYS Allow Scroll Zoom (Removed the !isSniper check)
        HandleScrollZoom();
        
        if(text_ammo != null)
            text_ammo.SetText(bulletsLeft + " / " + magazineSize);
    }

    private void HandleScrollZoom()
    {
        if (fpsCamera == null) return;

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            // If we are scoped, we change the 'scopedFOV' variable so the zoom updates live
            if (isScoped)
            {
                scopedFOV -= scrollInput * zoomSpeed * 5f;
                scopedFOV = Mathf.Clamp(scopedFOV, 1f, 30f); // Limit scope zoom
                fpsCamera.fieldOfView = scopedFOV;
            }
            else
            {
                // Normal Zoom
                currentFOV -= scrollInput * zoomSpeed * 10f;
                currentFOV = Mathf.Clamp(currentFOV, minZoom, maxZoom);
                fpsCamera.fieldOfView = currentFOV;
            }
        }
    }

    private void MyInput()
    {
        // Shooting
        if (Input.GetButton("Fire1") && readyToShoot && !reloading && bulletsLeft > 0)
        {
            Shoot();
        }

        // Reloading
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
        }

        // --- NEW HOLD-TO-AIM SNIPER LOGIC ---
        if (isSniper)
        {
            // If holding Right Click -> Scope ON
            if (Input.GetButton("Fire2") && !isScoped)
            {
                StartCoroutine(OnScoped());
            }
            // If released Right Click -> Scope OFF
            else if (!Input.GetButton("Fire2") && isScoped)
            {
                OnUnscoped();
            }
        }
    }

    IEnumerator OnScoped()
    {
        isScoped = true; // Mark as true immediately so we don't start this loop again

        // Optional: Play a "Zoom In" sound here
        
        yield return new WaitForSeconds(0.15f);

        if(scopeOverlay != null) scopeOverlay.SetActive(true);
        if(weaponModel != null) weaponModel.SetActive(false); // Hide Gun
        
        if(fpsCamera != null) fpsCamera.fieldOfView = scopedFOV;
    }

    void OnUnscoped()
    {
        isScoped = false;

        if(scopeOverlay != null) scopeOverlay.SetActive(false);
        if(weaponModel != null) weaponModel.SetActive(true); // Show Gun
        
        if(fpsCamera != null) fpsCamera.fieldOfView = currentFOV; // Return to previous scroll level
    }

    private void Shoot()
    {
        readyToShoot = false;

        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); 
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(75);
        }

        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        if (audioSource != null) audioSource.Play();

        // 2. FIX: Only play muzzle flash if the gun is actually visible!
        // (If we are scoped, the gun is hidden, so muzzle flash won't work anyway)
        if (muzzleFlash != null && !isScoped) 
            muzzleFlash.Play();

        GameObject currentBullet = Instantiate(bulletPrefab, attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithoutSpread.normalized;

        bulletsLeft--;
        Invoke("ResetShot", timeBetweenShooting);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        if (isSniper && isScoped)
        {
            OnUnscoped();
        }

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