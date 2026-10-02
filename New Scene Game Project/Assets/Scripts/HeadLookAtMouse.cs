using UnityEngine;

public class HeadLookAtMouse : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject head;
    public static Quaternion aimAngle;

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 mousePos = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);
        float angleRad = Mathf.Atan2(mousePos.y - head.transform.position.y, mousePos.x - head.transform.position.x);
        float angleDeg = (180 / Mathf.PI) * angleRad - 90;

        aimAngle = Quaternion.Euler(0f, 0f, angleDeg);
        head.transform.rotation = aimAngle;
    }
}
