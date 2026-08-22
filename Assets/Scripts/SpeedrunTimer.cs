using UnityEngine;
using TMPro;

public class SpeedrunTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    
    // For the Leaderboard
    [SerializeField] private LeaderboardManager leaderboardManager;

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
        // Reset the run and begin counting
        elapsedTime = 0f;
        timerRunning = true;

        //Debug.Log("SpeedrunTimer successfully started");
    }

    public void StopTimer()
    {
        timerRunning = false;
        
        // Show the name-entry screen after finishing
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