using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [Header("Movement Stuff")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 16f;
    
    [Header("Jump Stuff")]
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.2f;
    
    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    
    
    // Components
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    // Input System
    private PlayerControls controls;

    // Movement state
    private float horizontalInput;
    private bool isGrounded;
    private bool wasGrounded;
    private float velocityXSmoothing;

    // Jump state
    private bool jumpInputHeld;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool isJumping;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        controls = new PlayerControls();
    }
    
    private void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Jump.performed += OnJumpPerformed;
    }

    private void OnDisable()
    {
        controls.Player.Jump.performed -= OnJumpPerformed;
        controls.Player.Disable();
    }
    private void OnJumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        jumpBufferCounter = jumpBufferTime;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    void Update()
    {
        horizontalInput = controls.Player.Move.ReadValue<float>();
        
        CheckGroundStatus();
        //Debug
        Debug.Log(isGrounded);

    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(
            horizontalInput * moveSpeed,
            rb.linearVelocity.y
        );
    }
    
    private void CheckGroundStatus()
    {
        wasGrounded = isGrounded;

        // Just checks if player is grounded
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer);
    }
}
