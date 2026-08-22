using UnityEngine;
using TMPro;

public class SpeedrunTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    
    // For the Leaderboard
    [SerializeField] private LeaderboardManager leaderboardManager;
    [SerializeField] private GhostRecorder ghostRecorder;
    private float elapsedTime;
    private bool timerRunning;

    private void Update()
    {
        // Only count time while the run is active
        if (!timerRunning) return;

        elapsedTime += Time.deltaTime;
        UpdateTimerUI();
    }

    public void StartTimer()
    {
        elapsedTime = 0f;
        timerRunning = true;

        // Start recording the player's movement
        ghostRecorder.StartRecording();
    }

    public void StopTimer()
    {
        timerRunning = false;

        // Stop recording when the player crosses the finish
        ghostRecorder.StopRecording();

        leaderboardManager.ShowFinishScreen(elapsedTime);
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        float seconds = elapsedTime % 60f;

        // This makes it look like for example 01:23.456
        timerText.text = $"{minutes:00}:{seconds:00.000}";
    }
}