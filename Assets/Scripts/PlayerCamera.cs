using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector2 offset = new Vector2(0f, 1f);

    private void LateUpdate()
    {
        if (player == null) return;

        transform.position = new Vector3(
            player.position.x + offset.x,
            player.position.y + offset.y,
            transform.position.z
        );
    }
}