using UnityEngine;
using TMPro; // Standard Unity Text Library

public class GunSystem : MonoBehaviour
{
    // --- Stats ---
    [Header("Gun Stats")]
    public int damage = 10;
    public float timeBetweenShooting = 0.1f;
    public float spread = 0f;
    public float range = 100f;
    public float reloadTime = 1.5f;
    public int magazineSize = 30;
    public int bulletsLeft;
    
    // --- Bools ---
    bool readyToShoot;
    bool reloading;

    // --- References ---
    [Header("References")]
    public Camera fpsCamera;
    public Transform attackPoint; // Where the bullet visual starts (tip of gun)
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy; // To make sure we only hit valid things

    // --- Graphics ---
    [Header("Graphics")]
    public GameObject impactEffect; // The hole/spark when you hit a wall
    public ParticleSystem muzzleFlash; // (Optional) Fire at tip of gun

    // --- UI ---
    [Header("UI")]
    public TextMeshProUGUI text_ammo; // Drag your UI Text here

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        MyInput();
        
        // Update UI Text
        if(text_ammo != null)
            text_ammo.SetText(bulletsLeft + " / " + magazineSize);
    }

    private void MyInput()
    {
        // 0 = Left Click
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

        // 1. Raycast Logic (The math behind the shot)
        // Calculate direction with spread (optional, kept 0 for now)
        float x = Screen.width / 2;
        float y = Screen.height / 2;
        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        // Shoot the ray
        if (Physics.Raycast(ray, out rayHit, range, whatIsEnemy))
        {
            Debug.Log("Hit: " + rayHit.collider.name); // Check Console to see what you hit

            // Check if the object we hit has the "Target" script
            Target target = rayHit.transform.GetComponent<Target>();
            
            // If it DOES have the script, deal damage
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            // Instantiate Impact Effect (Bullet hole/sparks)
            if(impactEffect != null)
                Instantiate(impactEffect, rayHit.point, Quaternion.LookRotation(rayHit.normal));
        }

        // 2. Visuals
        if(muzzleFlash != null)
            muzzleFlash.Play();

        bulletsLeft--;

        // 3. Reset Shot (Fire Rate)
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