using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SimpleTelegraphRenderer : MonoBehaviour
{
    private LineRenderer line;
    private bool isActive = false;
    private Vector2[] positions = new Vector2[2];

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = true;
    }

    void Update()
    {
        if (isActive)
        {
            line.SetPosition(0, positions[0]);
            line.SetPosition(1, positions[1]);
        }
    }

    public void SetActive(bool value)
    {
        isActive = value;
        if (value)
        {
            line.positionCount = 2;
        }
        else
        {
            line.positionCount = 0;
        }
    }

    public void SetColor(Color color)
    {
        line.startColor = color;
        line.endColor = color;
    }

    public void SetPositions(Vector2 a, Vector2 b)
    {
        positions[0] = a;
        positions[1] = b;
    }
}
