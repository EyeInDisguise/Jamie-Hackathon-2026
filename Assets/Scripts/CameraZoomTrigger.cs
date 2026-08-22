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
            playerCamera = FindFirstObjectByType<PlayerCamera>();
        }

        if (playerCamera == null)
        {
            Debug.LogError("CameraZoomTrigger could not find PlayerCamera!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Something entered zoom trigger: " + other.name);

        if (!other.CompareTag("Player"))
        {
            return;
        }

        Debug.Log("Player entered zoom trigger");

        playerCamera.SetZoom(zoomedOutSize);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Something left zoom trigger: " + other.name);

        if (!other.CompareTag("Player"))
        {
            return;
        }

        Debug.Log("Player left zoom trigger");

        playerCamera.ResetZoom();
    }
}