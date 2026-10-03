using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    private float health;
    private float maxHealth;

    [SerializeField] private Image healthBar;

    public void ChangeHealthVisual()
    {
        health = PlayerHealth.Instance.GetHealth();
        maxHealth = PlayerHealth.Instance.GetMaxHealth();

        healthBar.fillAmount = health / maxHealth;
    }
}
