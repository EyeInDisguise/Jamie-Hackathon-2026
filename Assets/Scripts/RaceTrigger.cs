using UnityEngine;

public class RaceTrigger : MonoBehaviour
{
    [SerializeField] private SpeedrunTimer timer;
    [SerializeField] private bool startsRace;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore everything except the Player
        if (!other.CompareTag("Player")) return;

        Debug.Log("Player entered race trigger");

        if (startsRace)
        {
           // Debug.Log("Starting timer");
            timer.StartTimer();
        }
        else
        {
           // Debug.Log("Stopping timer");
            timer.StopTimer();
        }
    }
}