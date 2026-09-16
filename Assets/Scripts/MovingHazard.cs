using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingHazard : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float moveDistance = 5f;
        
    private Rigidbody2D rb;
    private Vector2 startPosition;
    private float movementTime;

    public Vector2 Velocity { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        startPosition = rb.position;
    }

    private void FixedUpdate()
    {
        // Freeze the object's movement while time stop is active
        if (player != null && player.IsTimeStopped)
        {
            Velocity = Vector2.zero;
            return;
        }

        // Will resume exactly where it stopped rather than in front
        movementTime += Time.fixedDeltaTime;

        // Pingpong just makes the value go up and down between 0 and moveDistance
        float movement = Mathf.PingPong(movementTime * moveSpeed, moveDistance);
        // offset from orig position instead of always adding movement
        Vector2 targetPosition = startPosition + Vector2.up * movement;

        // Store the crusher's real movement so the player can tell whether it is
        // closing a gap or merely touching them.
        Velocity = (targetPosition - rb.position) / Time.fixedDeltaTime;

        // Move through the 2D physics system so contacts stay reliable.
        rb.MovePosition(targetPosition);
    }
}
