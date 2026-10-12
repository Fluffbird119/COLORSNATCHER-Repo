using UnityEngine;

public class CameraSmoothFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float smoothTime;
    [SerializeField] private Vector3 offset;

    [SerializeField] private GameObject leftWall;
    [SerializeField] private GameObject rightWall;
    [SerializeField] private GameObject topWall;
    [SerializeField] private GameObject bottomWall;

    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    private Camera mainCam;

    private void Start()
    {
        mainCam = GetComponent<Camera>();

        float mainCamHeight = mainCam.orthographicSize;
        float mainCamWidth = mainCamHeight * mainCam.aspect;

        minX = (float)(leftWall.transform.position.x + 0.5) + mainCamWidth; // the .5 offset is cuz objects x and y transform are based on the center so offset by half the length in this case .5
        maxX = (float)(rightWall.transform.position.x - 0.5) - mainCamWidth;
        maxY = (float)(topWall.transform.position.y - 0.5) - mainCamHeight;
        minY = (float)(bottomWall.transform.position.y + 0.5) + mainCamHeight;
    }

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        // Calculate where the camera wants to go
        Vector3 targetPosition = player.position + offset;

        Vector3 smoothPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        float clampedX = Mathf.Clamp(smoothPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(smoothPosition.y, minY, maxY);

        transform.position = new Vector3(clampedX, clampedY, smoothPosition.z);
    }
}
