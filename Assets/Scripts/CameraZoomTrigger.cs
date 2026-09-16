using UnityEngine;

public class CameraZoomTrigger : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private PlayerCamera playerCamera;

    [Header("Zoom")]
    [SerializeField] private float zoomedOutSize = 8f;

    private void Awake()
    {
        // Find the camera automatically if it wasn't assigned
        if (playerCamera == null)
        {
            playerCamera = FindAnyObjectByType<PlayerCamera>();
        }

        if (playerCamera == null)
        {
            Debug.LogError("CameraZoomTrigger could not find PlayerCamera!", this);
            enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerCamera.SetZoom(zoomedOutSize);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerCamera.ResetZoom();
    }
}
