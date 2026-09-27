using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    [SerializeField] PlayerAttack attackScript;
    [SerializeField] HealthBarUI healthBarUIScript;
    [SerializeField] GameObject youDiedMessageAndButton;
    [SerializeField] GameObject player;
    [SerializeField] Transform playerTransform;

    [SerializeField] private float timeInBetweenHeals;

    public float health;
    public float maxHealth;

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            // If a duplicate exists, destroy this GameObject to enforce the single instance
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        youDiedMessageAndButton.SetActive(false);
        ChangeHealth(maxHealth);

        StartCoroutine(HealOverTime());
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

            attackScript.ResumeEye();

            youDiedMessageAndButton.SetActive(true);
            player.SetActive(false);
        }
    }

    // adds passed in amount to health
    public void ChangeHealth(float amount)
    {
        if (amount < 0)
        {
            StartCoroutine(iFrames());
        }

        health += amount;

        if (health > maxHealth)
        {
            health = maxHealth;
        }

        healthBarUIScript.ChangeHealthVisual();
    }

    // i frames for 0.1 seconds after player takes damage
    private IEnumerator iFrames()
    {
        player.layer = 6;

        yield return new WaitForSeconds(0.1f);

        player.layer = 7;
    }

    public void ResetPlayerStuff()
    {
        ChangeHealth(maxHealth);
        playerTransform.position = new Vector2(0, 0);
        player.SetActive(true);
    }

    public void ResetWaveStuff()
    {
        youDiedMessageAndButton.SetActive(false);

        if (GameManager.Instance.waveNumber == 1)
        {
            StartCoroutine(GameManager.Instance.WaveOne());
        }
        else if (GameManager.Instance.waveNumber == 2)
        {
            StartCoroutine(GameManager.Instance.WaveTwo());
        }
        else if (GameManager.Instance.waveNumber == 3)
        {
            StartCoroutine(GameManager.Instance.WaveThree());
        }
    }

    private IEnumerator HealOverTime()
    {
        // yay we love while true loops
        while (true)
        {
            yield return new WaitForSeconds(timeInBetweenHeals);
            ChangeHealth(1);
        }
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
