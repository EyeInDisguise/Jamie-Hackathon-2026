using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FixedAspectRatio : MonoBehaviour
{
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        UpdateAspectRatio();
    }

    private void Update()
    {
        // Browser window can change size
        UpdateAspectRatio();
    }

    private void UpdateAspectRatio()
    {
        // Keep the game view at 16:9
        const float targetAspect = 16f / 9f;

        float windowAspect =
            (float)Screen.width / Screen.height;

        float scaleHeight =
            windowAspect / targetAspect;

        // Window is narrower than 16:9
        if (scaleHeight < 1f)
        {
            Rect rect = cam.rect;

            rect.width = 1f;
            rect.height = scaleHeight;

            rect.x = 0f;
            rect.y = (1f - scaleHeight) / 2f;

            cam.rect = rect;
        }

        // Window is wider than 16:9
        else
        {
            float scaleWidth = 1f / scaleHeight;

            Rect rect = cam.rect;

            rect.width = scaleWidth;
            rect.height = 1f;

            rect.x = (1f - scaleWidth) / 2f;
            rect.y = 0f;

            cam.rect = rect;
        }
    }
}