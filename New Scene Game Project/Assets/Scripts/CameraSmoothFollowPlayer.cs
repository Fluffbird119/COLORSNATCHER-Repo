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

    private float leftWallOffset;
    private float rightWallOffset;
    private float topWallOffset;
    private float bottomWallOffset;

    private float minX; 
    private float maxX;
    private float minY;
    private float maxY;

    private Camera mainCam;

    public static float centerEdgeDistance;

    private void Start()
    {
        mainCam = GetComponent<Camera>();

        leftWallOffset = (float)(leftWall.transform.position.x + 0.5);
        rightWallOffset = (float)(rightWall.transform.position.x - 0.5);
        topWallOffset = (float)(topWall.transform.position.y - 0.5);
        bottomWallOffset = (float)(bottomWall.transform.position.y + 0.5);
    }

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        float mainCamHeight = mainCam.orthographicSize;
        float mainCamWidth = mainCamHeight * mainCam.aspect;

        minX = leftWallOffset + mainCamWidth; // the .5 offset is cuz objects x and y transform are based on the center so offset by half the length in this case .5
        maxX = rightWallOffset - mainCamWidth;
        maxY = topWallOffset - mainCamHeight;
        minY = bottomWallOffset + mainCamHeight;
        float halfHeight = mainCamHeight / 2;
        float halfWidth = mainCamWidth / 2;
        centerEdgeDistance = Mathf.Sqrt((halfHeight * halfHeight) + (halfWidth * halfWidth));


        // Calculate where the camera wants to go
        Vector3 targetPosition = player.position + offset;

        Vector3 smoothPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        float clampedX = Mathf.Clamp(smoothPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(smoothPosition.y, minY, maxY);

        transform.position = new Vector3(clampedX, clampedY, smoothPosition.z);
    }
}
