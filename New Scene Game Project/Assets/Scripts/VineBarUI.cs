using UnityEngine;
using UnityEngine.UI;

public class VineBarUI : MonoBehaviour
{
    private float curCooldown;
    private float maxCooldown;

    [SerializeField] private Image vineBar;

    public void UpdateVineVisual()
    {
        curCooldown = PlayerPowerUps.Instance.getVineCurCooldown();
        maxCooldown = PlayerPowerUps.Instance.getVineMaxCooldown();

        vineBar.fillAmount = curCooldown / maxCooldown;
    }
}
