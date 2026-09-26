using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public float Health;
    public float MaxHealth;
    public float Width;
    public float Height;


    [SerializeField] private RectTransform healthBar;

    public void SetMaxHealth(float newMaxHeath)
    {
        MaxHealth = newMaxHeath;
    }


    // adds passed value to current health so pass in negatives for damage and positives for healing
    public void ChangeHealth(float health)
    {
        Health += health;
        float newWidth = (Health / MaxHealth) * Width;

        healthBar.sizeDelta = new Vector2(newWidth, Height);
    }
}
