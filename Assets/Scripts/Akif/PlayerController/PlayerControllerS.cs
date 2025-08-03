using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerS : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 15f;
    public float rotationDamping = 0.025f;

    [Header("Jump Settings")]
    public float jumpHeight = 2f;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;

    [Header("Dash Settings")]
    public float dashSpeed = 40f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.0f;
    public AnimationCurve dashSpeedCurve = AnimationCurve.Linear(0, 1, 1, 0.5f);

    [Header("Attack Settings")]
    public float attackDuration = 0.5f;
    public float attackDmg = 10f;

    [Header("Gravity Settings")]
    public float normalGravity = -20f;
    public float risingGravityMultiplier = 1f;
    public float glidingGravityMultiplier = 0.75f;
    public float fallingGravityMultiplier = 2f;
    private float gravity = -20f;

    [Header("Input Smoothing Settings")]
    public float accelerationTime = 0.2f;
    public float decelerationTime = 0.2f;
    public AnimationCurve inputCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("References")]
    public Animator animator;

    [HideInInspector] public CharacterController controller;
    [HideInInspector] public InputManager inputManager;
    [HideInInspector] public PlayerStateMachine stateMachine;

    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public bool isGrounded;

    // Input smoothing
    [HideInInspector] public Vector2 currentSmoothedInput = Vector2.zero;
    [HideInInspector] public int targetX, targetZ;
    [HideInInspector] public float progressX, progressZ;

    // Jump helpers
    [HideInInspector] public float coyoteTimeCounter;
    [HideInInspector] public float jumpBufferCounter;

    // Dash helpers
    [HideInInspector] public bool isDashing;
    [HideInInspector] public float dashTimer;
    [HideInInspector] public float dashCooldownTimer;
    [HideInInspector] public Vector3 dashDirection;

    [Header("Health Settings")]
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private int maxHealth = 100;
    private float currentHealth;
    private bool isDead = false;
    private bool isHit = false;

    void Awake()
    {
        healthBar = FindObjectOfType<HealthBar>();
        controller = GetComponent<CharacterController>();

        if (!animator)
            animator = GetComponentInChildren<Animator>();

        stateMachine = new PlayerStateMachine();
    }

    void Start()
    {
        healthBar.SetMaxHealth(maxHealth);
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);

        inputManager = InputManager.Instance;

        stateMachine.Initialize(new IdleState(this, stateMachine));
    }

    void Update()
    {
        if (isDead || isHit) return;

        stateMachine.CurrentState?.HandleInput();
        stateMachine.CurrentState?.LogicUpdate();
    }

    void FixedUpdate()
    {
        stateMachine.CurrentState?.PhysicsUpdate();
    }

    public void TakeDamage(float damage)
    {
        if (isDead || isHit || isDashing) return;

        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        else if (currentHealth > maxHealth) 
        {
            currentHealth = maxHealth;
        }

        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(HandleHit());
        }
    }

    private IEnumerator HandleHit()
    {
        isHit = true;
        animator.SetTrigger("getHit");

        yield return new WaitForSeconds(0.4f); // "getHit" animasyonu s�resi kadar

        yield return new WaitForSeconds(0.6f);
        isHit = false;
    }

    public void Die()
    {
        isDead = true;
        animator.SetTrigger("die");
        controller.enabled = false;
        GetComponent<Collider>().enabled = false;
        new WaitForSeconds(1f);
        SceneManager.LoadScene("GameOver");
        //this.enabled = false;
    }

    public void GoToPosition(Transform newPos)
    {
        controller.enabled = false; 
        transform.position = newPos.position;
        transform.rotation = newPos.rotation;
        controller.enabled = true; 
    }

    public void GroundCheck()
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
        animator.SetBool("isGroundedBool", isGrounded);
    }


    public void HandleInputSmoothing()
    {
        targetX = Mathf.RoundToInt(inputManager.GetMoveInput().x);
        targetZ = Mathf.RoundToInt(inputManager.GetMoveInput().y);

        progressX = UpdateProgress(progressX, targetX != 0, accelerationTime, decelerationTime);
        progressZ = UpdateProgress(progressZ, targetZ != 0, accelerationTime, decelerationTime);

        float targetSmoothX = inputCurve.Evaluate(progressX) * targetX;
        float targetSmoothZ = inputCurve.Evaluate(progressZ) * targetZ;

        Vector2 targetInput = new Vector2(targetSmoothX, targetSmoothZ);
        //currentSmoothedInput = Vector2.MoveTowards(currentSmoothedInput, targetInput, Time.deltaTime / Mathf.Max(0.0001f, decelerationTime));
        currentSmoothedInput = Vector2.Lerp(currentSmoothedInput, targetInput, Time.deltaTime / decelerationTime);

    }

    public void HandleMovement()
    {
        Vector3 move = new Vector3(currentSmoothedInput.x, 0, currentSmoothedInput.y);
        if (move.magnitude > 1f) move.Normalize();

        controller.Move(move * moveSpeed * Time.deltaTime);
        animator.SetFloat("speedFloat", controller.velocity.magnitude);

    }

    public void CheckJumpInputBuffer()
    {
        if (inputManager.GetJumpInput())
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }


    public void HandleJump()
    {
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            animator.SetTrigger("jumpTrigger");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * normalGravity);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }
    }

    public void HandleDashInput()
    {
        dashCooldownTimer -= Time.deltaTime;

        if (isDashing) return;

        if (inputManager.GetDashInput() && dashCooldownTimer <= 0f)
        {
            StartDash();
        }
    }

    public void StartDash()
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

        animator.SetTrigger("dashTrigger");
    }

    public void HandleDash()
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
            animator.SetBool("isDashingBool", false);
        }
    }


    public void ApplyGravity()
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

    public void HandleRotation()
    {
        Vector3 move = new Vector3(currentSmoothedInput.x, 0, currentSmoothedInput.y);
        if (move != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationDamping);
        }
    }

    public float UpdateProgress(float progress, bool accelerating, float accelTime, float decelTime)
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

    public void HandleAttack()
    {
        animator.SetTrigger("attackTrigger");

    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Enemy"))
    //    {
    //        other.GetComponent<EnemyController>().TakeDamage(attackDmg);
    //    }
    //}

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
