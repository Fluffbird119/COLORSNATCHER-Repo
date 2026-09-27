using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    [SerializeField] PlayerAttack attackScript;
    [SerializeField] HealthBarUI healthBarUIScript;
    [SerializeField] GameObject youDiedMessageAndButton;
    [SerializeField] GameObject player;
    [SerializeField] Transform playerTransform;

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
        health += amount;

        if (health > maxHealth)
        {
            health = maxHealth;
        }

        healthBarUIScript.ChangeHealthVisual();
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

    public float GetHealth()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }
}
