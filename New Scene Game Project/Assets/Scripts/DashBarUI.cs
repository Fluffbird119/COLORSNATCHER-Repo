using UnityEngine;
using UnityEngine.UI;

public class DashBarUI : MonoBehaviour
{
    private float curCooldown;
    private float maxCooldown;

    [SerializeField] private Image dashBarLeft;
    [SerializeField] private Image dashBarRight;

    public void UpdateDashVisual()
    {
        curCooldown = PlayerPowerUps.Instance.getDashCurCooldown();
        maxCooldown = PlayerPowerUps.Instance.getDashMaxCooldown();

        if (curCooldown <= (maxCooldown / 2))
        {
            dashBarLeft.fillAmount = (curCooldown / (maxCooldown / 2)); // the / 2 is so it scales correctly for the left bar
        }
        else
        {
            dashBarRight.fillAmount = (curCooldown - maxCooldown / 2) / (maxCooldown - (maxCooldown / 2));
        }
    }
}
