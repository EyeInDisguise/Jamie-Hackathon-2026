using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RunResetManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform restartPoint;

    [Header("Run")]
    [SerializeField] private SpeedrunTimer speedrunTimer;

    [Header("Finish UI")]
    [SerializeField] private GameObject finishPanel;
    [SerializeField] private TMP_InputField nameInput;

    [Header("Camera")]
    [SerializeField] private PlayerCamera playerCamera;
    
    [Header("Ghost")]
    [SerializeField] private GhostPlayback ghostPlayback;

    public void ResetRun()
    {
        // Hide the finish screen
        if (finishPanel != null)
        {
            finishPanel.SetActive(false);
        }

        // Clear the previous player's name
        if (nameInput != null)
        {
            nameInput.text = "";
        }

        // Reset timer to 00:00.000
        if (speedrunTimer != null)
        {
            speedrunTimer.ResetTimer();
        }

        // Put the player back before the start trigger
        if (playerController != null && restartPoint != null)
        {
            playerController.ResetForNewRun(restartPoint.position);
        }

        // Return camera to normal zoom
        if (playerCamera != null)
        {
            playerCamera.ResetZoom();
        }

        // Stop Submit button staying selected
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
        
        // Stop the previous ghost playback
        if (ghostPlayback != null)
        {
            ghostPlayback.StopPlayback();
        }
    }
}
