using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    [SerializeField] PlayerAttack attackScript;
    [SerializeField] HealthBarUI healthBarUIScript;
    [SerializeField] GameObject youDiedMessageAndButton;
    [SerializeField] GameObject player;
    [SerializeField] Transform playerTransform;

    [SerializeField] private float timeInBetweenHeals;
    [SerializeField] private Light2D playerLight;
    private float playerLightRadius;
    private float playerLightFallOff;

    public float health;
    public float maxHealth;

    private float originalSpeed; // for resetting player values on player death

    private void Awake()
    {
        health = maxHealth;

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
        originalSpeed = PlayerMovement.Instance.GetSpeed();
        playerLightRadius = playerLight.pointLightOuterRadius;
        playerLightFallOff = playerLight.falloffIntensity;

        youDiedMessageAndButton.SetActive(false);
        
        StartCoroutine(HealOverTime());
    }

    private void Update()
    {
        if (health <= 0)
        {
            /*GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject enemy in enemyObjects)
            {
                Destroy(enemy);
            }*/

            GameManager.Instance.StopCurrWaveRoutine();
            EnemyManager.Instance.DeleteEnemies();
            AudioManager.Instance.StopAll();

            attackScript.ResumeEye();

            youDiedMessageAndButton.SetActive(true);
            PlayerPowerUps.Instance.setIsRaged(false);
            PlayerPowerUps.Instance.setIsDashing(false);
            GameManager.Instance.killsThisWave = 0;
            player.SetActive(false);
        }
    }

    // adds passed in amount to health
    public void ChangeHealth(float amount)
    {
        if (amount < 0)
        {
            StartCoroutine(iFrames());
            PlayerPowerUps.Instance.IncreaseRage(1f);
        }

        health += amount;

        if (health > maxHealth)
        {
            health = maxHealth;
        }

        healthBarUIScript.ChangeHealthVisual();
    }

    // overload for percentage based healing
    public void ChangeHealth(int sign, float percentage)
    {
        if (sign < 0)
        {
            StartCoroutine(iFrames());
        }

        health += maxHealth * sign * percentage;

        if (health > maxHealth)
        {
            health = maxHealth;
        }

        healthBarUIScript.ChangeHealthVisual();
    }

    // i frames for 0.1 seconds after player takes damage
    private IEnumerator iFrames()
    {
        player.layer = 9;

        yield return new WaitForSeconds(0.3f);

        player.layer = 7;
    }

    public void ResetPlayerStuff()
    {
        ChangeHealth(maxHealth);
        playerTransform.position = new Vector2(0, 0);
        player.SetActive(true);
        player.layer = 7;

        PlayerPowerUps.Instance.head.color = Color.white;
        PlayerPowerUps.Instance.eye.color = Color.white;
        PlayerPowerUps.Instance.attackTentacle.color = Color.white;
        PlayerPowerUps.Instance.legTentacles.color = Color.white;
        playerLight.color = Color.white;
        playerLight.pointLightOuterRadius = playerLightRadius;
        playerLight.falloffIntensity = playerLightFallOff;

        PlayerMovement.Instance.SetSpeed(originalSpeed);

        PlayerPowerUps.Instance.tentacleAnim.speed = 1f;
        PlayerPowerUps.Instance.EyeAnim.speed = 1f;
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
        else if (GameManager.Instance.waveNumber == 4)
        {
            StartCoroutine(GameManager.Instance.WaveFour());
        }
        else if (GameManager.Instance.waveNumber == 5)
        {
            StartCoroutine(GameManager.Instance.WaveFive());
        }
        else if (GameManager.Instance.waveNumber == 6)
        {
            StartCoroutine(GameManager.Instance.WaveSix());
        }
        else if (GameManager.Instance.waveNumber == 7)
        {
            StartCoroutine(GameManager.Instance.WaveSeven());
        }
        else if (GameManager.Instance.waveNumber == 8)
        {
            StartCoroutine(GameManager.Instance.WaveEight());
        }
        else if (GameManager.Instance.waveNumber == 9)
        {
            StartCoroutine(GameManager.Instance.WaveNine());
        }
        else if (GameManager.Instance.waveNumber == 10)
        {
            StartCoroutine(GameManager.Instance.WaveTen());
        }
        else if (GameManager.Instance.waveNumber == 11)
        {
            StartCoroutine(GameManager.Instance.WaveEleven());
        }
        else if (GameManager.Instance.waveNumber == 12)
        {
            StartCoroutine(GameManager.Instance.WaveTwelve());
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
