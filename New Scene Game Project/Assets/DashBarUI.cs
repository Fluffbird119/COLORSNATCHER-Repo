using UnityEngine;

public class DashBarUI : MonoBehaviour
{
    [SerializeField] PlayerPowerUps playerPowerUpsScript;

    private float curCooldown;
    private float maxCooldown;
    public float Width;
    public float Height;

    [SerializeField] private RectTransform dashBar;
}
