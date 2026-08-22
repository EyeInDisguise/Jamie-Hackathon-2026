using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody2D playerRb;

    [Header("Offset")]
    [SerializeField] private Vector2 offset = new Vector2(0f, 1f);

    [Header("Dead Zone")]
    [SerializeField] private float horizontalDeadZone = 0.8f;
    [SerializeField] private float verticalDeadZone = 0.5f;

    [Header("Look Ahead")]
    [SerializeField] private float lookAheadDistance = 1.2f;
    [SerializeField] private float lookAheadSpeed = 6f;

    private float currentLookAhead;

    private void LateUpdate()
    {
        if (player == null) return;

        // Look ahead in the direction the player is moving
        float targetLookAhead = 0f;

        if (playerRb != null && Mathf.Abs(playerRb.linearVelocity.x) > 0.1f)
        {
            targetLookAhead =
                Mathf.Sign(playerRb.linearVelocity.x) * lookAheadDistance;
        }

        // Smooth the look ahead movement
        currentLookAhead = Mathf.MoveTowards(
            currentLookAhead,
            targetLookAhead,
            lookAheadSpeed * Time.deltaTime
        );

        float targetX = player.position.x + offset.x + currentLookAhead;
        float targetY = player.position.y + offset.y;

        Vector3 newPosition = transform.position;

        // Move the camera when the player leaves the horizontal dead zone
        if (targetX > newPosition.x + horizontalDeadZone)
        {
            newPosition.x = targetX - horizontalDeadZone;
        }
        else if (targetX < newPosition.x - horizontalDeadZone)
        {
            newPosition.x = targetX + horizontalDeadZone;
        }

        // Move the camera when the player leaves the vertical dead zone
        if (targetY > newPosition.y + verticalDeadZone)
        {
            newPosition.y = targetY - verticalDeadZone;
        }
        else if (targetY < newPosition.y - verticalDeadZone)
        {
            newPosition.y = targetY + verticalDeadZone;
        }

        // Keep the same Z position
        newPosition.z = transform.position.z;

        transform.position = newPosition;
    }
}