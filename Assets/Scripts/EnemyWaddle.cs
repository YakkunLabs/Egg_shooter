using UnityEngine;
using UnityEngine.AI; // Needed for NavMeshAgent

public class EnemyWaddle : MonoBehaviour
{
    [Header("Settings")]
    public Transform eggModel;      // Drag the Child Mesh (Visuals) here
    public float waddleSpeed = 10f; // How fast it rocks
    public float waddleAmount = 10f;// How far it leans (Angle)

    private NavMeshAgent agent;
    private Vector3 startRotation;

    void Start()
    {
        // 1. Get the NavMeshAgent from this object
        agent = GetComponent<NavMeshAgent>();

        // 2. Remember the model's starting rotation (so we don't break your 90-degree fix)
        if (eggModel != null)
        {
            startRotation = eggModel.localEulerAngles;
        }
        else
        {
            Debug.LogError("Please assign the 'Egg Model' in the Inspector!");
        }
    }

    void Update()
    {
        if (eggModel == null || agent == null) return;

        // 3. Check if the enemy is moving
        if (agent.velocity.magnitude > 0.1f)
        {
            // Calculate the "Rocking" angle using a Sine wave (Up and Down numbers)
            float wobble = Mathf.Sin(Time.time * waddleSpeed) * waddleAmount;

            // Apply the wobble to the Z-Axis (Left/Right Tilt)
            // We add 'wobble' to the startRotation to keep the original alignment
            eggModel.localEulerAngles = startRotation + new Vector3(0f, 0f, wobble);
        }
        else
        {
            // 4. If stopped, smoothly return to normal upright position
            eggModel.localEulerAngles = Vector3.Lerp(eggModel.localEulerAngles, startRotation, Time.deltaTime * 5f);
        }
    }
}