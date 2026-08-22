using UnityEngine;

public class MovingHazard : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float moveDistance = 5f;
        
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Freeze the object's movement while time stop is active
        if (player.IsTimeStopped) return;
        
        // Pingpong just makes the value go up and down between 0 and moveDistance
        float movement = Mathf.PingPong(Time.time * moveSpeed, moveDistance);
        // offset from orig position instead of always adding movement
        transform.position = startPosition + Vector3.up * movement;
    }
}
