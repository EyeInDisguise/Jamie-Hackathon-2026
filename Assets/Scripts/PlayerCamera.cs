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

    [Header("Camera Zoom")]
    [SerializeField] private float zoomSpeed = 5f;

    private Camera cam;

    private float currentLookAhead;

    private float normalCameraSize;
    private float targetCameraSize;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        if (cam == null)
        {
           // Debug.LogError("PlayerCamera needs to be on the Main Camera!");
            return;
        }

        normalCameraSize = cam.orthographicSize;
        targetCameraSize = normalCameraSize;

      //  Debug.Log("Normal camera size: " + normalCameraSize);
    }

    private void LateUpdate()
    {
        if (player == null || cam == null)
        {
            return;
        }

        // Look ahead in the direction the player is moving
        float targetLookAhead = 0f;

        if (playerRb != null && Mathf.Abs(playerRb.linearVelocity.x) > 0.1f)
        {
            targetLookAhead =
                Mathf.Sign(playerRb.linearVelocity.x) * lookAheadDistance;
        }

        currentLookAhead = Mathf.MoveTowards(
            currentLookAhead,
            targetLookAhead,
            lookAheadSpeed * Time.deltaTime
        );

        float targetX =
            player.position.x + offset.x + currentLookAhead;

        float targetY =
            player.position.y + offset.y;

        Vector3 newPosition = transform.position;

        // Horizontal dead zone
        if (targetX > newPosition.x + horizontalDeadZone)
        {
            newPosition.x = targetX - horizontalDeadZone;
        }
        else if (targetX < newPosition.x - horizontalDeadZone)
        {
            newPosition.x = targetX + horizontalDeadZone;
        }

        // Vertical dead zone
        if (targetY > newPosition.y + verticalDeadZone)
        {
            newPosition.y = targetY - verticalDeadZone;
        }
        else if (targetY < newPosition.y - verticalDeadZone)
        {
            newPosition.y = targetY + verticalDeadZone;
        }

        newPosition.z = transform.position.z;

        transform.position = newPosition;

        // Smooth camera zoom
        cam.orthographicSize = Mathf.MoveTowards(
            cam.orthographicSize,
            targetCameraSize,
            zoomSpeed * Time.deltaTime
        );
    }

    public void SetZoom(float newSize)
    {
    //    Debug.Log("Camera zoom requested: " + newSize);

        targetCameraSize = newSize;
    }

    public void ResetZoom()
    {
      //  Debug.Log("Camera zoom reset: " + normalCameraSize);

        targetCameraSize = normalCameraSize;
    }
}