using UnityEngine;

public class HeadLookAtMouse : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 mousePos = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);
        float angleRad = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x);
        float angleDed = (180 / Mathf.PI) * angleRad - 90; // offset by 90 degs

        transform.rotation = Quaternion.Euler(0f, 0f, angleDed);
    }
}
