using TMPro;
using UnityEngine;

public class LeaderboardSubmitRestart : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private LeaderboardManager leaderboardManager;
    [SerializeField] private RunResetManager runResetManager;

    public void SubmitAndRestart()
    {
        // Don't reset if no name was entered
        if (nameInput == null || string.IsNullOrWhiteSpace(nameInput.text))
        {
            return;
        }

        // Use your existing leaderboard submission
        leaderboardManager.SubmitScore();

        // Then reset the run
        runResetManager.ResetRun();
    }
}