using UnityEngine;
using UnityEngine.UI;

public class RageBarUI : MonoBehaviour
{
    private float rage;
    private float maxRage;

    [SerializeField] private Image rageBar;
    [SerializeField] private Image rageBarHolder;

    private Color ogHolderColor;

    private void Awake()
    {
        ogHolderColor = rageBarHolder.color;
    }

    public void UpdateRageVisual()
    {
        rage = PlayerPowerUps.Instance.getRage();
        maxRage = PlayerPowerUps.Instance.getMaxRage();

        rageBar.fillAmount = rage / maxRage;

        if (rageBar.fillAmount == 1)
        {
            rageBarHolder.color = new Color(255, 0, 0, 255);
        }
        else
        {
            rageBarHolder.color = ogHolderColor;
        }
    }
}
