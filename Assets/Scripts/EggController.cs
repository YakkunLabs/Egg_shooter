using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EggController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 12f;
    // Removed runSpeed
    public float jumpHeight = 2.5f;
    public float gravity = -19.62f;
    public AudioSource audioSource;

    [Header("Look Settings")]
    public Transform playerCamera;
    public float mouseSensitivity = 100f;
    public float lookXLimit = 85f;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;

    [Header("Status")]
    public bool canMove = true;

    [Header("Ladder Settings")]
    public bool isClimbing = false;
    public float climbSpeed = 6f;
    public GameObject climbMessageUI; 
    private bool canClimb = false;

    public bool serverAuthoritative = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        if (climbMessageUI != null) climbMessageUI.SetActive(false);

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
        {
            UnlockCursor();
            canMove = false;
            return;
        }

        if (MobileInputManager.Instance != null && MobileInputManager.Instance.IsMobileMode)
            UnlockCursor();
        else
            LockCursor();
    }

    void Update()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu") return;

        // --- 1. LADDER INTERACTION ---
        // Toggle climbing when pressing Shift near a ladder
        if (canClimb && Input.GetKeyDown(KeyCode.LeftShift))
        {
            isClimbing = !isClimbing;
            
            // Hide text if climbing, Show if waiting
            if (climbMessageUI != null) 
            {
                climbMessageUI.SetActive(!isClimbing);
            }
        }

        // --- 2. CALCULATE MOVEMENT ---
        
        if (isClimbing)
        {
            // === CLIMBING LOGIC ===
            float verticalInput = MobileInputManager.Instance.GetVertical(); 
            moveDirection = new Vector3(0, verticalInput * climbSpeed, 0);

            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                moveDirection += transform.forward * 2f; 
                isClimbing = false;
                
                if (climbMessageUI != null && canClimb) climbMessageUI.SetActive(true);
            }
        }
        else
        {
            // === NORMAL WALKING LOGIC (No Sprint) ===
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            // Always use walkSpeed (No Shift check here)
            float curSpeedX = canMove ? walkSpeed * MobileInputManager.Instance.GetVertical() : 0;
            float curSpeedY = canMove ? walkSpeed * MobileInputManager.Instance.GetHorizontal() : 0;

            float movementDirectionY = moveDirection.y; 
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            // Jumping
            if ((Input.GetButton("Jump") || MobileInputManager.Instance.jumpPressed) && canMove && characterController.isGrounded)
            {
                moveDirection.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                if (audioSource != null) audioSource.Play();
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }

            // Gravity
            if (!characterController.isGrounded)
            {
                moveDirection.y += gravity * Time.deltaTime;
            }
        }

        // --- 3. APPLY MOVEMENT ---
        if (!serverAuthoritative)
        {
            characterController.Move(moveDirection * Time.deltaTime);
        }

        // --- 4. LOOK AROUND ---
        if (canMove)
        {
            rotationX += -MobileInputManager.Instance.GetLookY() * mouseSensitivity * Time.deltaTime;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, MobileInputManager.Instance.GetLookX() * mouseSensitivity * Time.deltaTime, 0);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            canClimb = true;
            if (climbMessageUI != null && !isClimbing) climbMessageUI.SetActive(true); 
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            canClimb = false;
            isClimbing = false; 
            if (climbMessageUI != null) climbMessageUI.SetActive(false);
        }
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}