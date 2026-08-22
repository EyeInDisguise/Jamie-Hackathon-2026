using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private Transform visual;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform visualPivot;

    [Header("Movement Stuff")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 16f;

    [Header("Jump Stuff")]
    [SerializeField] private float coyoteTime = 0.08f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.05f;

    [Header("Gravity")]
    [SerializeField] private float fallGravityMultiplier = 2f;

    [Header("Ac/De-eleration")]
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 60f;

    [Header("FallSpeed")]
    [SerializeField] private float maxFallSpeed = 20f;

    [Header("Dashing Stuff")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;

    private bool isDashing;
    private bool canDash = true;

    [Header("Wall Detection")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance = 0.1f;
    [SerializeField] private LayerMask wallLayer;

    private bool isTouchingWall;

    [Header("Wall Slide")]
    [SerializeField] private float wallSlideSpeed = 1f;

    private bool isWallSliding;

    [Header("Wall Jump")]
    [SerializeField] private float wallJumpHorizontalForce = 10f;
    [SerializeField] private float wallJumpVerticalForce = 16f;

    [Header("Gravity Stuff")]
    [SerializeField] private Transform ceilingCheck;

    private bool gravityFlipped;

    [Header("Time Stop")]
    [SerializeField] private float timeStopDuration = 3f;

    private bool isTimeStopped;

    [Header("Ability UI")]
    [SerializeField] private AbilityUI abilityUI;

    // Other scripts can check if time has stopped
    public bool IsTimeStopped => isTimeStopped;

    // Components
    private Rigidbody2D rb;

    // Input System
    private PlayerControls controls;

    // Movement state
    private float horizontalInput;
    private bool isGrounded;
    private bool wasGrounded;
    private float velocityXSmoothing;

    // Jump
    private bool jumpInputHeld;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool isJumping;

    // Stores current ability
    private int currentAbility = 0;

    // Wall detection
    private bool wallLeft;
    private bool wallRight;

    private void Awake()
    {
        // Initialises all the components
        rb = GetComponent<Rigidbody2D>();
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Player.Enable();

        // Jump
        controls.Player.Jump.performed += OnJumpPerformed;
        controls.Player.Jump.canceled += OnJumpCanceled;
    }

    private void OnDisable()
    {
        // Jump
        controls.Player.Jump.performed -= OnJumpPerformed;
        controls.Player.Jump.canceled -= OnJumpCanceled;

        controls.Player.Disable();
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        // For Ability 2 Wall Jumps
        // If on ability 2 and touching wall and not on ground
        if (currentAbility == 2 && isTouchingWall && !isGrounded)
        {
            float horizontalDirection = wallLeft ? 1f : -1f;

            // Takes into account flipped gravity
            float verticalDirection = gravityFlipped ? -1f : 1f;

            rb.linearVelocity = new Vector2(
                horizontalDirection * wallJumpHorizontalForce,
                verticalDirection * wallJumpVerticalForce
            );

            animator.SetTrigger("WallJump");

            // Play wall jump sound
            AudioManager.Instance?.PlayWallJump();

            isJumping = true;
            jumpBufferCounter = 0f;

            return;
        }

        jumpBufferCounter = jumpBufferTime;
    }

    private void OnJumpCanceled(InputAction.CallbackContext ctx)
    {
        float verticalVelocity =
            gravityFlipped ? -rb.linearVelocity.y : rb.linearVelocity.y;

        if (verticalVelocity > 0f)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * 0.5f
            );
        }
    }

    void Update()
    {
        horizontalInput = controls.Player.Move.ReadValue<float>();

        CheckGroundStatus();

        // Check if player just landed
        if (!wasGrounded && isGrounded)
        {
            animator.SetTrigger("Land");

            // Play landing sound once
            AudioManager.Instance?.PlayLand();
        }

        // Vertical velocity relative to gravity
        float verticalVelocity =
            gravityFlipped ? -rb.linearVelocity.y : rb.linearVelocity.y;

        // Reset dash when touching ground
        if (isGrounded)
        {
            canDash = true;
        }

        // Detect when landed
        if (isGrounded && verticalVelocity <= 0f)
        {
            isJumping = false;
        }

        // Coyote time
        if (isGrounded && !isJumping)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Jump buffer
        if (jumpBufferCounter > 0f)
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Normal jump
        if (
            jumpBufferCounter > 0f &&
            coyoteTimeCounter > 0f &&
            !isJumping
        )
        {
            // Jumping from ceiling pushes player away
            float jumpDirection = gravityFlipped ? -1f : 1f;

            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce * jumpDirection
            );

            // Play jump sound
            AudioManager.Instance?.PlayJump();

            isJumping = true;
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }

        // Flip the player depending on direction
        // Also accounts for flipped gravity
        if (horizontalInput > 0f)
        {
            spriteRenderer.flipX = gravityFlipped;
        }
        else if (horizontalInput < 0f)
        {
            spriteRenderer.flipX = !gravityFlipped;
        }

        // Wall jump
        CheckWallStatus();

        // Animation speed also accounts for momentum
        float animationSpeed = Mathf.Max(
            Mathf.Abs(horizontalInput),
            Mathf.Abs(rb.linearVelocity.x) / moveSpeed
        );

        animator.SetFloat("Speed", animationSpeed);

        animator.SetBool("Jumping", isJumping);
        animator.SetFloat("VelocityY", verticalVelocity);
        animator.SetBool("Grounded", isGrounded);
        animator.SetBool("WallSliding", isWallSliding);

        // Ability selection
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            SetAbility(1);
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            SetAbility(2);
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            SetAbility(3);
        }

        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            SetAbility(4);
        }

        // Dash
        if (
            currentAbility == 1 &&
            Keyboard.current.leftShiftKey.wasPressedThisFrame &&
            canDash &&
            !isDashing
        )
        {
            StartCoroutine(Dash());
        }

        // Gravity ability
        if (
            currentAbility == 3 &&
            Keyboard.current.leftShiftKey.wasPressedThisFrame
        )
        {
            FlipGravity();
        }

        // Time Stop ability
        if (
            currentAbility == 4 &&
            Keyboard.current.leftShiftKey.wasPressedThisFrame &&
            !isTimeStopped
        )
        {
            StartCoroutine(TimeStop());
        }
    }

    private void FixedUpdate()
    {
        // Don't let normal movement overwrite a dash
        if (isDashing)
        {
            return;
        }

        // Horizontal acceleration/deceleration
        float targetSpeed = horizontalInput * moveSpeed;

        float rate =
            horizontalInput != 0
                ? acceleration
                : deceleration;

        float newXVelocity = Mathf.MoveTowards(
            rb.linearVelocity.x,
            targetSpeed,
            rate * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector2(
            newXVelocity,
            rb.linearVelocity.y
        );

        // Vertical velocity relative to current gravity direction
        // Negative means falling even when upside down
        float verticalVelocity =
            gravityFlipped ? -rb.linearVelocity.y : rb.linearVelocity.y;

        // Wall sliding
        isWallSliding =
            currentAbility == 2 &&
            isTouchingWall &&
            !isGrounded &&
            verticalVelocity < 0f;

        if (isWallSliding)
        {
            float slideDirection = gravityFlipped ? 1f : -1f;

            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                slideDirection * wallSlideSpeed
            );
        }
        else
        {
            // Faster falling
            if (verticalVelocity < 0f)
            {
                float gravityDirection =
                    gravityFlipped ? 1f : -1f;

                rb.linearVelocity +=
                    Vector2.up *
                    (
                        gravityDirection *
                        Mathf.Abs(Physics2D.gravity.y) *
                        (fallGravityMultiplier - 1f) *
                        Time.fixedDeltaTime
                    );
            }

            // Recalculate since gravity changed velocity
            verticalVelocity =
                gravityFlipped
                    ? -rb.linearVelocity.y
                    : rb.linearVelocity.y;

            // Max fall speed
            if (verticalVelocity < -maxFallSpeed)
            {
                float fallDirection =
                    gravityFlipped ? 1f : -1f;

                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    fallDirection * maxFallSpeed
                );
            }
        }
    }

    private void CheckGroundStatus()
    {
        wasGrounded = isGrounded;

        // Use ceiling check when gravity is flipped
        Transform activeCheck =
            gravityFlipped ? ceilingCheck : groundCheck;

        isGrounded = Physics2D.OverlapCircle(
            activeCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        // Play dash sound once
        AudioManager.Instance?.PlayDash();

        // Temporarily disable gravity
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // Calculate dash direction
        Vector2 dashDirection =
            controls.Player.DashDirection.ReadValue<Vector2>();

        if (dashDirection == Vector2.zero)
        {
            dashDirection =
                spriteRenderer.flipX
                    ? Vector2.left
                    : Vector2.right;
        }

        // Keep diagonal dash speed the same
        dashDirection.Normalize();

        rb.linearVelocity = dashDirection * dashSpeed;

        bool sidewaysDash =
            Mathf.Abs(dashDirection.x) > 0.1f;

        // Choose animation depending on dash direction
        if (sidewaysDash)
        {
            animator.Play("Dash", 0, 0f);
        }
        else if (dashDirection.y > 0f)
        {
            animator.Play("JumpRise", 0, 0f);
        }
        else if (dashDirection.y < 0f)
        {
            animator.Play("JumpFall", 0, 0f);
        }

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;

        animator.SetBool("Dashing", false);
    }

    private void CheckWallStatus()
    {
        wallLeft = Physics2D.Raycast(
            wallCheck.position,
            Vector2.left,
            wallCheckDistance,
            groundLayer
        );

        wallRight = Physics2D.Raycast(
            wallCheck.position,
            Vector2.right,
            wallCheckDistance,
            groundLayer
        );

        isTouchingWall = wallLeft || wallRight;
    }

    // Gravity ability
    private void FlipGravity()
    {
        gravityFlipped = !gravityFlipped;

        rb.gravityScale *= -1f;

        visualPivot.localRotation =
            gravityFlipped
                ? Quaternion.Euler(0f, 0f, 180f)
                : Quaternion.identity;

        // Play gravity flip sound
        AudioManager.Instance?.PlayGravityFlip();
    }

    // Time Stop ability
    private IEnumerator TimeStop()
    {
        isTimeStopped = true;

        // Play time stop sound
        AudioManager.Instance?.PlayTimeStop();

        yield return new WaitForSeconds(timeStopDuration);

        isTimeStopped = false;
    }

    private void SetAbility(int ability)
    {
        currentAbility = ability;

        // Update HUD
        abilityUI.SetAbility(currentAbility);

        // Play RFID / ability select sound
        AudioManager.Instance?.PlayAbilitySelect();
    }
}