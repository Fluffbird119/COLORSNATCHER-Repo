using UnityEngine;
using UnityEngine.UI;

public class RageBarUI : MonoBehaviour
{
    private float rage;
    private float maxRage;

    [SerializeField] private Image rageBar;

    public void UpdateRageVisual()
    {
        rage = PlayerPowerUps.Instance.getRage();
        maxRage = PlayerPowerUps.Instance.getMaxRage();

        rageBar.fillAmount = rage / maxRage;
    }
}
