using UnityEngine;
using UnityEngine.UI;

public class VineBarUI : MonoBehaviour
{
    private float curCooldown;
    private float maxCooldown;

    [SerializeField] private Image vineBar;
    [SerializeField] private Image vineBarHolder;

    private Color ogHolderColor;

    private void Awake()
    {
        ogHolderColor = vineBarHolder.color;
    }

    public void UpdateVineVisual()
    {
        curCooldown = PlayerPowerUps.Instance.getVineCurCooldown();
        maxCooldown = PlayerPowerUps.Instance.getVineMaxCooldown();

        vineBar.fillAmount = curCooldown / maxCooldown;

        if (vineBar.fillAmount == 1)
        {
            vineBarHolder.color = new Color32(65, 255, 25, 255);
        }
        else
        {
            vineBarHolder.color = ogHolderColor;
        }
    }
}
