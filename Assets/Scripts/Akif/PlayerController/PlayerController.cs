using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 15f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f * 3;
    public float rotationDamping = 0.025f;

    [Header("Smoothing Settings")]
    public float accelerationTime = 0.2f;
    public float decelerationTime = 0.2f;
    public AnimationCurve inputCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private InputManager m_inputManager;

    // Progression state per axis
    private float progressX = 0f;
    private float progressZ = 0f;

    private int targetX = 0;
    private int targetZ = 0;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        m_inputManager = InputManager.Instance;
    }

    void Update()
    {
        // Ground check
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Raw input from InputManager
        Vector2 moveInput = m_inputManager.GetMoveInput();
        targetX = Mathf.RoundToInt(Mathf.Clamp(moveInput.x, -1f, 1f));
        targetZ = Mathf.RoundToInt(Mathf.Clamp(moveInput.y, -1f, 1f));

        // Update progression values
        progressX = UpdateProgress(progressX, targetX != 0, accelerationTime, decelerationTime);
        progressZ = UpdateProgress(progressZ, targetZ != 0, accelerationTime, decelerationTime);

        // Evaluate AnimationCurve
        float smoothX = inputCurve.Evaluate(progressX) * targetX;
        float smoothZ = inputCurve.Evaluate(progressZ) * targetZ;

        // Movement vector
        Vector3 move = new Vector3(smoothX, 0, smoothZ);
        if (move.magnitude > 1f) move.Normalize();

        controller.Move(move * moveSpeed * Time.deltaTime);

        // Jump
        if (m_inputManager.GetJumpInput() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Rotation
        if (move != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationDamping);
        }
    }

    /// <summary>
    /// Updates the progression value (0..1) for input smoothing.
    /// </summary>
    float UpdateProgress(float progress, bool accelerating, float accelTime, float decelTime)
    {
        if (accelerating)
        {
            progress += Time.deltaTime / Mathf.Max(0.0001f, accelTime);
        }
        else
        {
            progress -= Time.deltaTime / Mathf.Max(0.0001f, decelTime);
        }
        return Mathf.Clamp01(progress);
    }
}
