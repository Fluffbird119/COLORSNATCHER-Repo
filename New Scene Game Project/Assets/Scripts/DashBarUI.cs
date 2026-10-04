using UnityEngine;
using UnityEngine.UI;

public class DashBarUI : MonoBehaviour
{
    private float curCooldown1;
    private float curCooldown2;
    private float maxCooldown;

    [SerializeField] private Image dashBarLeft;
    [SerializeField] private Image dashBarRight;

    public void UpdateDashVisual()
    {
        curCooldown1 = PlayerPowerUps.Instance.getDash1CurCooldown();
        curCooldown2 = PlayerPowerUps.Instance.getDash2CurCooldown();
        maxCooldown = PlayerPowerUps.Instance.getDashMaxCooldown();

        dashBarLeft.fillAmount = curCooldown1 / maxCooldown;
        dashBarRight.fillAmount = curCooldown2 / maxCooldown;
    }
}
