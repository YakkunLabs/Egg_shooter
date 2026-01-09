using UnityEngine;

public class PlayerSkinLoader : MonoBehaviour
{
    [Header("Setup")]
    // Drag the actual player's MeshRenderer here
    public MeshRenderer playerMesh; 
    
    // IMPORTANT: This list MUST match the Main Menu list exactly!
    public Material[] allSkins; 

    void Start()
    {
        // 1. Read saved choice
        int chosenIndex = PlayerPrefs.GetInt("SelectedSkin", 0);

        // 2. Apply material
        if (playerMesh != null && allSkins.Length > chosenIndex)
        {
            playerMesh.material = allSkins[chosenIndex];
        }
    }
}