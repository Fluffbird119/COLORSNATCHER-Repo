using UnityEngine;

public class CircleDrawer : MonoBehaviour
{
    public int segments = 50;
    public float radius = 1f;
    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = false;
        DrawCircle();
    }

    void DrawCircle()
    {
        line.positionCount = segments;
        
        for (int i = 0; i < segments; i++)
        {
            float progress = (float)i / segments;
            float angle = progress * 2 * Mathf.PI;

            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            line.SetPosition(i, new Vector3(x, y, 0));
        }
    }
}
