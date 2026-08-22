using UnityEngine;

public class BouncyBall : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerController player;

    [Header("Starting Movement")]
    [SerializeField] private Vector2 startingVelocity = new Vector2(5f, 8f);

    [Header("Bounce")]
    [SerializeField] private float minimumSpeed = 5f;
    [SerializeField] private float maximumSpeed = 15f;

    private Rigidbody2D rb;

    private Vector2 savedVelocity;
    private float savedAngularVelocity;

    private bool wasTimeStopped;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rb.linearVelocity = startingVelocity;
    }

    private void FixedUpdate()
    {
        // Time stop has just started
        if (player.IsTimeStopped && !wasTimeStopped)
        {
            // Remember exactly how the ball was moving
            savedVelocity = rb.linearVelocity;
            savedAngularVelocity = rb.angularVelocity;

            // Freeze the physics body
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;

            wasTimeStopped = true;
            return;
        }

        // Stay frozen
        if (player.IsTimeStopped)
        {
            return;
        }

        // Time has just resumed
        if (wasTimeStopped)
        {
            rb.simulated = true;

            // Continue from exactly where it left off
            rb.linearVelocity = savedVelocity;
            rb.angularVelocity = savedAngularVelocity;

            wasTimeStopped = false;
        }

        float speed = rb.linearVelocity.magnitude;

        if (speed > 0.1f && speed < minimumSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * minimumSpeed;
        }

        if (speed > maximumSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maximumSpeed;
        }
    }
}