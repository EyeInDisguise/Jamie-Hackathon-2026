using UnityEngine;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private GameObject finishPanel;
    [SerializeField] private TMP_Text finalTimeText;
    [SerializeField] private TMP_InputField nameInput;

    private float finishedTime;

    public void ShowFinishScreen(float time)
    {
        Debug.Log("2. SHOW FINISH SCREEN");

        finishedTime = time;

        finishPanel.SetActive(true);

        Debug.Log("3. PANEL ACTIVE: " + finishPanel.activeSelf);

        finalTimeText.text = $"TIME: {FormatTime(time)}";

        nameInput.Select();
        nameInput.ActivateInputField();
    }

    public void SubmitScore()
    {
        string playerName = nameInput.text.Trim();

        // Don't allow blank 
        if (playerName.Length == 0) return;

        Debug.Log($"{playerName}: {finishedTime}");

        // Still need to save :D
        finishPanel.SetActive(false);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        float seconds = time % 60f;

        return $"{minutes:00}:{seconds:00.000}";
    }
}