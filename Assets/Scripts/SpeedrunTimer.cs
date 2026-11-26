using UnityEngine;
using TMPro;

public class SpeedrunTimer : MonoBehaviour
{
    // Ready -> Running -> Finished. Reset returns to Ready
    private enum TimerState
    {
        Ready,
        Running,
        Finished
    }
    
    [SerializeField] private TMP_Text timerText;

    // For the Leaderboard
    [SerializeField] private LeaderboardManager leaderboardManager;
    [SerializeField] private GhostRecorder ghostRecorder;
    [SerializeField] private GhostPlayback ghostPlayback;

    // Frozen when the run finishes
    [SerializeField] private PlayerController playerController;

    private float elapsedTime;
    private TimerState currentState;
    
    private void Update()
    {
        // Only update the timer if it's in the Running state
        if (currentState != TimerState.Running) return;
        
        elapsedTime += Time.deltaTime;
        UpdateTimerUI();
    }

    private void SetTimerState(TimerState state)
    {
        currentState = state;
    }

    public void StartTimer()
    {
        // Only start the timer if it's in the Ready state
        if (currentState != TimerState.Ready) return;
        SetTimerState(TimerState.Running);
        elapsedTime = 0f;

        // Play the current #1 ghost
        ghostPlayback.StartPlayback();

        // Record the challenger
        ghostRecorder.StartRecording();
    }

    public void StopTimer()
    {
        // Only stop the timer if it's in the Running state
        if (currentState != TimerState.Running) return;
        SetTimerState(TimerState.Finished);

        // Freeze the player
        playerController.Freeze();
        
        // Stop recording when the player crosses the finish
        ghostRecorder.StopRecording();

        // Play finish sound
        AudioManager.Instance?.PlayFinish();

        leaderboardManager.ShowFinishScreen(elapsedTime);
    }

    // Reset without starting another run
    public void ResetTimer()
    {  
        // Resetting the timer returns it to the Ready state
        SetTimerState(TimerState.Ready);
        elapsedTime = 0f;

        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        float seconds = elapsedTime % 60f;

        // This makes it look like for example 01:23.456
        timerText.text = $"{minutes:00}:{seconds:00.000}";
    }
}