using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EggController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 12f;
    public float jumpHeight = 2.5f;
    public float gravity = -19.62f; // Higher gravity feels snappier for FPS

    [Header("Look Settings")]
    public Transform playerCamera; // Drag your Main Camera here
    public float mouseSensitivity = 100f;
    public float lookXLimit = 85f; // Prevents breaking your neck looking up/down

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;

    [Header("Status")]
    public bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock cursor to center of screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. Calculate Movement (WASD)
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Check if shift is held for running (optional)
        float curSpeedX = canMove ? walkSpeed * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? walkSpeed * Input.GetAxis("Horizontal") : 0;

        float movementDirectionY = moveDirection.y; // Preserve vertical velocity (gravity)
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // 2. Jumping Logic
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // 3. Apply Gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y += gravity * Time.deltaTime;
        }

        // 4. Move the Controller
        characterController.Move(moveDirection * Time.deltaTime);

        // 5. Camera Rotation (Looking Around)
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

            // Rotate Camera up/down
            playerCamera.localRotation = Quaternion.Euler(rotationX, 0, 0);
            
            // Rotate Player Body left/right
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime, 0);
        }
    }
}