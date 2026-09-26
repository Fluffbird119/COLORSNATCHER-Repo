using UnityEngine;

public class RageBarUI : MonoBehaviour
{
    [SerializeField] PlayerPowerUps playerPowerUpsScript;

    private float rage;
    private float maxRage;
    public float Width;
    public float Height;

    [SerializeField] private RectTransform rageBar;

    public void UpdateRageVisual()
    {
        rage = playerPowerUpsScript.getRage();
        maxRage = playerPowerUpsScript.getMaxRage();

        float newHeight = (rage / maxRage) * Height;

        rageBar.sizeDelta = new Vector2(Width, newHeight);
    }
}
