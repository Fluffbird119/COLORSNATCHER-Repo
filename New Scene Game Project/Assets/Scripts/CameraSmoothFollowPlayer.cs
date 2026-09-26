using UnityEngine;

public class CameraSmoothFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float smoothTime;
    [SerializeField] private Vector3 offset;

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (player != null)
        {
            // Calculate where the camera wants to go
            Vector3 targetPosition = player.position + offset;

            // Smoothly move the camera to that target position
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}
