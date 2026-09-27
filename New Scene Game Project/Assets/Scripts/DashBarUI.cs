using UnityEngine;

public class DashBarUI : MonoBehaviour
{
    [SerializeField] PlayerPowerUps playerPowerUpsScript;

    private float curCooldown;
    private float maxCooldown;
    public float Width;
    public float Height;

    [SerializeField] private RectTransform dashBar;

    public void UpdateDashVisual()
    {
        curCooldown = playerPowerUpsScript.getDashCurCooldown();
        maxCooldown = playerPowerUpsScript.getDashMaxCooldown();

        float newHeight = (curCooldown / maxCooldown) * Height;

        dashBar.sizeDelta = new Vector2(Width, newHeight);
    }
}
