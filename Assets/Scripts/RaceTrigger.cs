using UnityEngine;

public class RaceTrigger : MonoBehaviour
{
    [SerializeField] private SpeedrunTimer timer;
    [SerializeField] private bool startsRace;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Something entered: " + other.name);

        if (!other.CompareTag("Player"))
        {
            Debug.Log("Not tagged Player");
            return;
        }

        Debug.Log("Player entered trigger");

        if (startsRace)
        {
            timer.StartTimer();
        }
        else
        {
            timer.StopTimer();
        }
    }
}