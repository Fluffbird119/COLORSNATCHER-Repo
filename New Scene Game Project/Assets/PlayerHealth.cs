using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] HealthBarUI healthBarUIScript;
    [SerializeField] GameObject youDiedMessageAndButton;

    public float health;
    public float maxHealth;

    private void Start()
    {
        youDiedMessageAndButton.SetActive(false);
        ChangeHealth(maxHealth);
    }

    private void Update()
    {
        if (health <= 0)
        {
            GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject enemy in enemyObjects)
            {
                Destroy(enemy);
            }
        }

        // turn on your died dialouge and set active on player to false
        youDiedMessageAndButton.SetActive(true);
        // then return player to 0,0 and call the start wave coroutnine
    }

    // adds passed in amount to health
    public void ChangeHealth(float amount)
    {
        health += amount;

        if (health > maxHealth)
        {
            health = maxHealth;
        }

        healthBarUIScript.ChangeHealthVisual();
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }
}
