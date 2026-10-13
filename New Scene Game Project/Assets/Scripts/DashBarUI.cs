using UnityEngine;
using UnityEngine.UI;

public class DashBarUI : MonoBehaviour
{
    private float curCooldown1;
    private float curCooldown2;
    private float maxCooldown;

    [SerializeField] private Image dashBarLeft;
    [SerializeField] private Image dashBarRight;

    [SerializeField] private Image dashBarHolder;

    private Color ogHolderColor;

    private void Awake()
    {
        ogHolderColor = dashBarHolder.color;
    }

    public void UpdateDashVisual()
    {
        curCooldown1 = PlayerPowerUps.Instance.getDash1CurCooldown();
        curCooldown2 = PlayerPowerUps.Instance.getDash2CurCooldown();
        maxCooldown = PlayerPowerUps.Instance.getDashMaxCooldown();

        dashBarLeft.fillAmount = curCooldown1 / maxCooldown;
        dashBarRight.fillAmount = curCooldown2 / maxCooldown;

        // make holder yellow to show that they can dash but theyll also be able to wait and stack up two dashes
        if (dashBarLeft.fillAmount == 1)
        {
            dashBarHolder.color = new Color32(255, 236, 0, 255);
        }
        else
        {
            dashBarHolder.color = ogHolderColor;
        }
    }
}
