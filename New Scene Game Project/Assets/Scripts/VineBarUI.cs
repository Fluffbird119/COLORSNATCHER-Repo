using UnityEngine;

public class VineBarUI : MonoBehaviour
{
    private float curCooldown;
    private float maxCooldown;
    public float width;
    public float height;

    [SerializeField] private RectTransform vineBar;

    public void UpdateVineVisual()
    {
        curCooldown = PlayerPowerUps.Instance.getVineCurCooldown();
        maxCooldown = PlayerPowerUps.Instance.getVineMaxCooldown();

        float newHeight = (curCooldown / maxCooldown) * height;

        vineBar.sizeDelta = new Vector2(width, newHeight);
    }
}
