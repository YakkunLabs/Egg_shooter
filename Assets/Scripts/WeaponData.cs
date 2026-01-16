using UnityEngine;

/// <summary>
/// Weapon Data ScriptableObject - Stores all weapon statistics
/// This allows easy configuration of different weapon types
/// </summary>
[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Identity")]
    public string weaponName = "Weapon";
    public WeaponType weaponType = WeaponType.AssaultRifle;

    [Header("Fire Mode")]
    public FireMode fireMode = FireMode.Automatic;
    public bool canToggleFireMode = false;
    public FireMode[] availableFireModes;

    [Header("Damage Stats")]
    public int damage = 25;
    public float headshotMultiplier = 2.0f;

    [Header("Fire Rate")]
    [Tooltip("Rounds per minute (RPM)")]
    public float roundsPerMinute = 600f;
    public float bulletSpeed = 100f; // Default speed

    
    [Tooltip("Calculated automatically from RPM")]
    public float timeBetweenShots => 60f / roundsPerMinute;

    [Header("Burst Fire Settings (if applicable)")]
    public int burstCount = 3;
    public float burstDelay = 0.1f;

    [Header("Magazine & Ammo")]
    public int magazineSize = 30;
    public int reserveAmmo = 120;
    public bool infiniteAmmo = false;

    [Header("Reload")]
    public float reloadTime = 2.5f;
    public bool reloadFullMagazine = true;
    [Tooltip("If false, reload one bullet at a time (like shotgun)")]
    public float reloadPerBulletTime = 0.5f;

    [Header("Accuracy")]
    [Range(0f, 10f)]
    public float spread = 0.5f;
    [Range(0f, 10f)]
    public float aimSpreadMultiplier = 0.3f; // Spread when aiming
    [Range(0f, 5f)]
    public float recoilAmount = 1.0f;

    [Header("Range")]
    public float effectiveRange = 100f;
    public float maxRange = 200f;

    [Header("Sniper Scope (if applicable)")]
    public bool hasScope = false;
    public float scopedFOV = 15f;
    public float scopeZoomSpeed = 10f;

    [Header("Aiming / Iron Sights")]
    public Vector3 aimPosition; // Where the gun moves when aiming
    public float aimZoom = 40f; // FOV when aiming (Default is 60)
    public float aimSpeed = 10f; // How fast it snaps to the eye

    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip emptySound;

    [Header("Explosive Stats")]
    public float explosionRadius = 0f; // 0 = no explosion

    [Header("Visual Effects")]
    public GameObject muzzleFlashPrefab;
    public GameObject bulletTracerPrefab;
    public GameObject impactEffectPrefab;
}

public enum WeaponType
{
    Pistol,
    AssaultRifle,
    SniperRifle,
    Shotgun,
    SMG,
    LMG,
    RocketLauncher
}

public enum FireMode
{
    SemiAutomatic,  // One shot per click
    Automatic,      // Hold to shoot continuously
    Burst,          // 3-round burst per click
    BoltAction      // Manual reload after each shot (sniper)
}
