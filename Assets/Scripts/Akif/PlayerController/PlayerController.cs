using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 15f;
    public float rotationDamping = 0.025f;
    private Vector3 velocity;

    [Header("Jump Settings")]
    public float jumpHeight = 2f;
    private bool isGrounded;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;

    private float coyoteTimeCounter = 0f;
    private float jumpBufferCounter = 0f;

    [Header("Dash Settings")]
    public float dashSpeed = 40f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.0f;
    public AnimationCurve dashSpeedCurve = AnimationCurve.Linear(0, 1, 1, 0.5f);

    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection = Vector3.zero;


    [Header("Gravity Settings")]
    public float normalGravity = -20f;
    public float gravity = -20f;

    public float risingGravityMultiplier = 1f;
    public float glidingGravityMultiplier = 0.75f;
    public float fallingGravityMultiplier = 2f;


    [Header("Input Smoothing Settings")]
    public float accelerationTime = 0.2f;
    public float decelerationTime = 0.2f;
    public AnimationCurve inputCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private Vector2 currentSmoothedInput = Vector2.zero;

    private float progressX = 0f;
    private float progressZ = 0f;

    private int targetX = 0;
    private int targetZ = 0;

    private CharacterController controller;
    private InputManager m_inputManager;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        m_inputManager = InputManager.Instance;
    }

    void Update()
    {
        GroundCheck();

        HandleInputSmoothing();
        HandleMovement();

        CheckJumpInputBuffer();
        HandleJump();

        HandleDashInput();
        HandleDash();

        ApplyGravity();
        HandleRotation();
    }


    void GroundCheck()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }


    void HandleInputSmoothing()
    {
        targetX = Mathf.RoundToInt(m_inputManager.GetMoveInput().x);
        targetZ = Mathf.RoundToInt(m_inputManager.GetMoveInput().y);

        progressX = UpdateProgress(progressX, targetX != 0, accelerationTime, decelerationTime);
        progressZ = UpdateProgress(progressZ, targetZ != 0, accelerationTime, decelerationTime);

        float targetSmoothX = inputCurve.Evaluate(progressX) * targetX;
        float targetSmoothZ = inputCurve.Evaluate(progressZ) * targetZ;

        Vector2 targetInput = new Vector2(targetSmoothX, targetSmoothZ);
        //currentSmoothedInput = Vector2.MoveTowards(currentSmoothedInput, targetInput, Time.deltaTime / Mathf.Max(0.0001f, decelerationTime));
        currentSmoothedInput = Vector2.Lerp(currentSmoothedInput, targetInput, Time.deltaTime / decelerationTime);

    }

    void HandleMovement()
    {
        Vector3 move = new Vector3(currentSmoothedInput.x, 0, currentSmoothedInput.y);
        if (move.magnitude > 1f) move.Normalize();

        controller.Move(move * moveSpeed * Time.deltaTime);

    }

    void CheckJumpInputBuffer()
    {
        if (m_inputManager.GetJumpInput())
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }


    void HandleJump()
    {
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * normalGravity);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }
    }

    void HandleDashInput()
    {
        dashCooldownTimer -= Time.deltaTime;

        if (isDashing) return;

        if (m_inputManager.GetDashInput() && dashCooldownTimer <= 0f)
        {
            StartDash();
        }
    }

    void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        Vector3 moveInput = new Vector3(currentSmoothedInput.x, 0, currentSmoothedInput.y);
        if (moveInput.magnitude > 0.1f)
        {
            dashDirection = moveInput.normalized;
        }
        else
        {
            dashDirection = transform.forward;
        }

        velocity.y = 0;
    }

    void HandleDash()
    {
        if (!isDashing) return;

        dashTimer -= Time.deltaTime;

        float dashProgress = 1f - (dashTimer / dashDuration);
        float curveValue = dashSpeedCurve.Evaluate(dashProgress);

        Vector3 dashMove = dashDirection * dashSpeed * curveValue;

        controller.Move(dashMove * Time.deltaTime);

        if (dashTimer <= 0f)
        {
            isDashing = false;
        }
    }


    void ApplyGravity()
    {
        if (isDashing) return;

        if (velocity.y > 2f)
        {
            gravity = normalGravity * risingGravityMultiplier;
        }
        else if (velocity.y < -2f)
        {
            gravity = normalGravity * fallingGravityMultiplier;
        }
        else
        {
            gravity = normalGravity * glidingGravityMultiplier;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleRotation()
    {
        Vector3 move = new Vector3(currentSmoothedInput.x, 0, currentSmoothedInput.y);
        if (move != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationDamping);
        }
    }

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

    #region Public Methods
    public bool IsDashing()
    {
        return isDashing;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public bool IsJumping()
    {
        return !isGrounded && Mathf.Abs(velocity.y) > 0.01f;
    }
    #endregion
}
