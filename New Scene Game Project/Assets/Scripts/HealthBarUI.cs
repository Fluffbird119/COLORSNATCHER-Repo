using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] PlayerHealth healthScript;

    private float health;
    private float maxHealth;
    public float width;
    public float height;

    [SerializeField] private RectTransform healthBar;


    public void ChangeHealthVisual()
    {
        health = healthScript.GetHealth();
        maxHealth = healthScript.GetMaxHealth();

        float newWidth = (health / maxHealth) * width;

        healthBar.sizeDelta = new Vector2(newWidth, height);
    }
}
