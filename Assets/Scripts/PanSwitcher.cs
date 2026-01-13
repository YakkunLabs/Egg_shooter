using UnityEngine;

public class PanSwitcher : MonoBehaviour
{
    [Header("References")]
    public GameObject panObject;  // Drag your Pan model here
    public GameObject[] allGuns;  // Drag your Rifle, Sniper, Pistol, etc. here

    private GameObject lastActiveGun; // To remember what we held before pressing P
    private bool isHoldingPan = false;

    void Start()
    {
        // Ensure Pan is hidden at start
        if(panObject != null) 
            panObject.SetActive(false);
    }

    void Update()
    {
        // Toggle Pan with 'P'
        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleMelee();
        }
    }

    void ToggleMelee()
    {
        isHoldingPan = !isHoldingPan;

        if (isHoldingPan)
        {
            // --- SWITCHING TO PAN ---
            
            // 1. Find which gun is currently on, save it, then hide it
            foreach (GameObject gun in allGuns)
            {
                if (gun.activeSelf)
                {
                    lastActiveGun = gun;
                    gun.SetActive(false);
                }
            }

            // 2. Show the Pan
            panObject.SetActive(true);
        }
        else
        {
            // --- SWITCHING BACK TO GUN ---

            // 1. Hide the Pan
            panObject.SetActive(false);

            // 2. Bring back the gun we had (or default to the first one)
            if (lastActiveGun != null)
            {
                lastActiveGun.SetActive(true);
            }
            else if (allGuns.Length > 0)
            {
                // Safety net: if we forgot the last gun, just show the first one
                allGuns[0].SetActive(true);
            }
        }
    }
}