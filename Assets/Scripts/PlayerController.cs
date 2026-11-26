using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private Transform visual;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform visualPivot;

    [Header("Movement Stuff")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Jump Stuff")]
    [SerializeField] private float coyoteTime = 0.08f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.05f;

    [Header("Gravity")]
    [SerializeField] private float fallGravityMultiplier = 1.46f;

    [Header("Ac/De-eleration")]
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 60f;

    [Header("FallSpeed")]
    [SerializeField] private float maxFallSpeed = 20f;

    [Header("Dashing Stuff")]
    [SerializeField] private float dashSpeed = 12.67f;
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
    [SerializeField] private float wallJumpHorizontalForce = 11f;
    [SerializeField] private float wallJumpVerticalForce = 13f;

    [Header("Gravity Stuff")]
    [SerializeField] private Transform ceilingCheck;    

    private bool gravityFlipped;

    [Header("Time Stop")]
    [SerializeField] private float timeStopDuration = 3f;

    private bool isTimeStopped;

    [Header("Ability UI")]
    [SerializeField] private AbilityUI abilityUI;

    [Header("Crush Death")]
    [SerializeField] private RunResetManager runResetManager;
    [SerializeField] private float minCrushSpeed = 0.05f;
    [SerializeField, Range(-1f, 0f)] private float opposingSurfaceDot = -0.75f;

    // Other scripts can check if time has stopped
    public bool IsTimeStopped => isTimeStopped;

    // Components
    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private float normalGravityScale;
    private Vector2 initialSpawnPosition;
    private readonly ContactPoint2D[] crushContacts = new ContactPoint2D[16];
    private float nextAllowedCrushResetTime;

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
        playerCollider = GetComponent<Collider2D>();
        initialSpawnPosition = rb.position;

        // Remember normal gravity so we can restore it after a run
        normalGravityScale = Mathf.Abs(rb.gravityScale);

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

    private void Update()
    {
        horizontalInput = controls.Player.Move.ReadValue<float>();

        CheckGroundStatus();

        // Check if player just landed
        if (!wasGrounded && isGrounded)
        {
            animator.SetTrigger("Land");

            AudioManager.Instance?.PlayLand();
        }

        float verticalVelocity =
            gravityFlipped ? -rb.linearVelocity.y : rb.linearVelocity.y;

        // Reset dash when touching ground
        if (isGrounded)
        {
            canDash = true;
        }

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
            float jumpDirection = gravityFlipped ? -1f : 1f;

            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce * jumpDirection
            );

            AudioManager.Instance?.PlayJump();

            isJumping = true;
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }

        // Flip the player depending on direction
        if (horizontalInput > 0f)
        {
            spriteRenderer.flipX = false;
        }
        else if (horizontalInput < 0f)
        {
            spriteRenderer.flipX = true;
        }

        CheckWallStatus();

        float animationSpeed = Mathf.Max(
            Mathf.Abs(horizontalInput),
            Mathf.Abs(rb.linearVelocity.x) / moveSpeed
        );

        animator.SetFloat("Speed", animationSpeed);

        animator.SetBool("Jumping", isJumping);
        animator.SetFloat("VelocityY", verticalVelocity);
        animator.SetBool("Grounded", isGrounded);
        animator.SetBool("WallSliding", isWallSliding);

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        // Ability selection
        if (keyboard.digit1Key.wasPressedThisFrame)
        {
            SetAbility(1);
        }

        if (keyboard.digit2Key.wasPressedThisFrame)
        {
            SetAbility(2);
        }

        if (keyboard.digit3Key.wasPressedThisFrame)
        {
            SetAbility(3);
        }

        if (keyboard.digit4Key.wasPressedThisFrame)
        {
            SetAbility(4);
        }

        // Dash
        if (
            currentAbility == 1 &&
            keyboard.leftShiftKey.wasPressedThisFrame &&
            canDash &&
            !isDashing
        )
        {
            StartCoroutine(Dash());
        }

        // Gravity ability
        if (
            currentAbility == 3 &&
            keyboard.leftShiftKey.wasPressedThisFrame
        )
        {
            FlipGravity();
        }

        // Time Stop ability
        if (
            currentAbility == 4 &&
            keyboard.leftShiftKey.wasPressedThisFrame &&
            !isTimeStopped
        )
        {
            StartCoroutine(TimeStop());
        }
    }

    private void FixedUpdate()
    {
        CheckForCrushDeath();

        // Don't let normal movement overwrite a dash
        if (isDashing)
        {
            return;
        }

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

    private void CheckForCrushDeath()
    {
        if (Time.unscaledTime < nextAllowedCrushResetTime)
        {
            return;
        }

        int contactCount = playerCollider.GetContacts(crushContacts);

        for (int i = 0; i < contactCount; i++)
        {
            ContactPoint2D crusherContact = crushContacts[i];
            Collider2D crusherCollider = GetOtherCollider(crusherContact);

            if (crusherCollider == null)
            {
                continue;
            }

            MovingHazard crusher = crusherCollider.GetComponentInParent<MovingHazard>();

            if (crusher == null)
            {
                continue;
            }

            Vector2 crusherNormal = GetNormalTowardPlayer(crusherContact);

            // The hazard must be moving into the player, not away or sideways.
            if (Vector2.Dot(crusher.Velocity, crusherNormal) < minCrushSpeed)
            {
                continue;
            }

            for (int j = 0; j < contactCount; j++)
            {
                if (j == i)
                {
                    continue;
                }

                ContactPoint2D blockingContact = crushContacts[j];
                Collider2D blockingCollider = GetOtherCollider(blockingContact);

                if (blockingCollider == null ||
                    blockingCollider.GetComponentInParent<MovingHazard>() == crusher)
                {
                    continue;
                }

                Vector2 blockingNormal = GetNormalTowardPlayer(blockingContact);

                // Opposing contact means there is nowhere left for the player to go.
                if (Vector2.Dot(crusherNormal, blockingNormal) <= opposingSurfaceDot)
                {
                    ResetAfterCrush();
                    return;
                }
            }
        }
    }

    private Collider2D GetOtherCollider(ContactPoint2D contact)
    {
        return contact.collider == playerCollider
            ? contact.otherCollider
            : contact.collider;
    }

    private Vector2 GetNormalTowardPlayer(ContactPoint2D contact)
    {
        Vector2 normal = contact.normal;
        Vector2 towardPlayer = (Vector2)playerCollider.bounds.center - contact.point;

        if (Vector2.Dot(normal, towardPlayer) < 0f)
        {
            normal = -normal;
        }

        return normal;
    }

    private void ResetAfterCrush()
    {
        // Avoid a second reset while the old physics contacts are being cleared.
        nextAllowedCrushResetTime = Time.unscaledTime + 0.25f;

        if (runResetManager == null)
        {
            runResetManager = FindAnyObjectByType<RunResetManager>();
        }

        if (runResetManager != null)
        {
            runResetManager.ResetRun();
        }
        else
        {
            // The tutorial has no run manager, so return to its original spawn.
            ResetForNewRun(initialSpawnPosition);
        }
    }

    private void CheckGroundStatus()
    {
        wasGrounded = isGrounded;

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

        AudioManager.Instance?.PlayDash();

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        Vector2 dashDirection =
            controls.Player.DashDirection.ReadValue<Vector2>();

        if (dashDirection == Vector2.zero)
        {
            dashDirection =
                spriteRenderer.flipX
                    ? Vector2.left
                    : Vector2.right;
        }

        dashDirection.Normalize();

        rb.linearVelocity = dashDirection * dashSpeed;

        bool sidewaysDash =
            Mathf.Abs(dashDirection.x) > 0.1f;

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

        // Mirror vertically (not rotate), so the player keeps facing the same way
        visualPivot.localScale =
            gravityFlipped
                ? new Vector3(1f, -1f, 1f)
                : Vector3.one;

        AudioManager.Instance?.PlayGravityFlip();
    }

    // Time Stop ability
    private IEnumerator TimeStop()
    {
        isTimeStopped = true;

        AudioManager.Instance?.PlayTimeStop();

        yield return new WaitForSeconds(timeStopDuration);

        isTimeStopped = false;
    }

    private void SetAbility(int ability)
    {
        currentAbility = ability;

        abilityUI.SetAbility(currentAbility);

        AudioManager.Instance?.PlayAbilitySelect();
    }

    // Freeze for when player is finished
    public void Freeze()
    {
        // Stop a dash or time stop that's still running
        StopAllCoroutines();

        // Remove momentum
        rb.linearVelocity = Vector2.zero;

        // Stop gravity and physics from moving the body
        rb.bodyType = RigidbodyType2D.Kinematic;
        
        // Turn off this script: stops Update, FixedUpdate and input
        enabled = false;
    }
    
    // Reset the player without reloading the scene
    public void ResetForNewRun(Vector3 startPosition)
    {
        // Stop dash/time stop from the previous run
        StopAllCoroutines();

        // Unfreeze physics so gravity works again
        rb.bodyType = RigidbodyType2D.Dynamic;

        // Teleport back and remove momentum
        rb.position = startPosition;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Reset gravity
        gravityFlipped = false;
        rb.gravityScale = normalGravityScale;
        visualPivot.localScale = Vector3.one;
        spriteRenderer.flipX = false;

        // Reset ability/movement state
        isDashing = false;
        canDash = true;
        isTimeStopped = false;
        isWallSliding = false;
        isTouchingWall = false;
        wallLeft = false;
        wallRight = false;

        isJumping = false;
        horizontalInput = 0f;

        jumpBufferCounter = 0f;
        coyoteTimeCounter = 0f;

        currentAbility = 0;

        isGrounded = false;
        wasGrounded = false;

        // Reset ability HUD
        if (abilityUI != null)
        {
            abilityUI.SetAbility(0);
        }

        // Reset animation
        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }
        
        // Unfreeze the player so they can move again
        enabled = true;
    }
}
