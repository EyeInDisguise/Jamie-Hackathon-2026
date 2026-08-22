using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private GameObject finishPanel;
    [SerializeField] private TMP_Text finalTimeText;
    [SerializeField] private TMP_InputField nameInput;
    //Leaderboard
    [SerializeField] private TMP_Text leaderboardText;
    
    [SerializeField] private GhostRecorder ghostRecorder;
    [SerializeField] private GhostPlayback ghostPlayback;
    private class ScoreEntry
    {
        // gives each leaderboard entry 2 pieces of info
        public string name;
        public float time;

        public ScoreEntry(string name, float time)
        {
            this.name = name;
            this.time = time;
        }
    }
    
    private List<ScoreEntry> scores = new List<ScoreEntry>();
    private bool scoreSubmitted;
    
    private float finishedTime;

    
    private void Start()
    {
        LoadLeaderboard();
        UpdateLeaderboardText();
    }
    
    private void LoadLeaderboard()
    {
        scores.Clear();

        int scoreCount = PlayerPrefs.GetInt("ScoreCount", 0);

        for (int i = 0; i < scoreCount; i++)
        {
            string playerName = PlayerPrefs.GetString("PlayerName" + i);
            float playerTime = PlayerPrefs.GetFloat("PlayerTime" + i);

            scores.Add(new ScoreEntry(playerName, playerTime));
        }
    }
    
    public void ShowFinishScreen(float time)
    {
        scoreSubmitted = false;
        Debug.Log("2. SHOW FINISH SCREEN");

        finishedTime = time;

        finishPanel.SetActive(true);

        Debug.Log("3. PANEL ACTIVE: " + finishPanel.activeSelf);

        finalTimeText.text = $"TIME: {FormatTime(time)}";

        nameInput.text = "";
        nameInput.Select();
        nameInput.ActivateInputField();
    }

    public void SubmitScore()
    {
        if (scoreSubmitted) return;

        string playerName = nameInput.text.Trim();

        if (playerName.Length == 0)
        {
            Debug.Log("Enter a name first");
            return;
        }

        // Check against the current #1 BEFORE inserting this score
        bool isNewBest = scores.Count == 0 || finishedTime < scores[0].time;

        scores.Add(new ScoreEntry(playerName, finishedTime));

        scores.Sort((a, b) => a.time.CompareTo(b.time));

        if (scores.Count > 5)
        {
            scores.RemoveRange(5, scores.Count - 5);
        }

        SaveLeaderboard();
        UpdateLeaderboardText();

        // Only #1 becomes the ghost
        if (isNewBest)
        {
            ghostPlayback.SaveBestGhost(ghostRecorder.Frames);
            Debug.Log(playerName + " is the new ghost!");
        }

        scoreSubmitted = true;
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        float seconds = time % 60f;

        return $"{minutes:00}:{seconds:00.000}";
    }
    
    private void SaveLeaderboard()
    {
        PlayerPrefs.SetInt("ScoreCount", scores.Count);

        for (int i = 0; i < scores.Count; i++)
        {
            PlayerPrefs.SetString("PlayerName" + i, scores[i].name);
            PlayerPrefs.SetFloat("PlayerTime" + i, scores[i].time);
        }

        PlayerPrefs.Save();
    }
    
    private void UpdateLeaderboardText()
    {
        leaderboardText.text = "LEADERBOARD\n\n";

        for (int i = 0; i < scores.Count; i++)
        {
            leaderboardText.text +=
                $"{i + 1}. {scores[i].name}    {FormatTime(scores[i].time)}\n";
        }

        // Fill unused positions
        for (int i = scores.Count; i < 5; i++)
        {
            leaderboardText.text += $"{i + 1}. ---\n";
        }
    }
    
    public void ResetLeaderboard()
    {
        // Clear leaderboard in memory
        scores.Clear();

        // Clear saved leaderboard data
        PlayerPrefs.DeleteKey("ScoreCount");

        for (int i = 0; i < 5; i++)
        {
            PlayerPrefs.DeleteKey("PlayerName" + i);
            PlayerPrefs.DeleteKey("PlayerTime" + i);
        }

        PlayerPrefs.Save();

        UpdateLeaderboardText();

        Debug.Log("Leaderboard reset");
    }

    void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetLeaderboard();
        }
    }
        
}