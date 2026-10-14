using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

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
    [SerializeField] Volume proccessingVolume;
    private VolumeProfile postProccessing;
    private Bloom bloomOverride;
    [SerializeField] private float baseBloomIntensity;
    private Vignette vignetteOverride;
    [SerializeField] private float baseVignetteIntensity;
    private Coroutine currVigRoutine;
    private Coroutine curriFrameRoutine;
    private float timeSinceDamage;

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
        postProccessing = proccessingVolume.profile;
        if (postProccessing.TryGet(out bloomOverride))
        {
           bloomOverride.intensity.overrideState = true;
        }

        if (postProccessing.TryGet(out vignetteOverride))
        {
            vignetteOverride.intensity.overrideState = true;
            vignetteOverride.intensity.Override(0f); 
        }

        youDiedMessageAndButton.SetActive(false);
        StartCoroutine(HealOverTime());
    }

    private void Update()
    {
        if (health <= 0)
        {
            GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");

            foreach (GameObject bullet in bullets)
            {
                Destroy(bullet);
            }

            Debug.Log("Ded");

            health = 0;
            GameManager.Instance.StopCurrWaveRoutine();
            EnemyManager.Instance.DeleteEnemies();
            AudioManager.Instance.StopAll();
            StopAllCoroutines();

            attackScript.ResumeEye();
            player.layer = 9;

            youDiedMessageAndButton.SetActive(true);
            PlayerPowerUps.Instance.setIsRaged(false);
            PlayerPowerUps.Instance.setIsDashing(false);
            GameManager.Instance.killsThisWave = 0;
            PlayerManager.Instance.SetPlayerActive(false);
        }
    }

    // adds passed in amount to health
    public void ChangeHealth(float amount)
    {
        if (amount < 0)
        {
            if (curriFrameRoutine != null)
            {
                StopCoroutine(curriFrameRoutine);
            }
            curriFrameRoutine = StartCoroutine(iFrames());
            if (currVigRoutine != null)
            {
                StopCoroutine(currVigRoutine);
            }
            currVigRoutine = StartCoroutine(HitVignette());
            AudioManager.Instance.PlayerHitNoise();
            PlayerPowerUps.Instance.IncreaseRage(1f);
            timeSinceDamage = 0f;
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
            if (curriFrameRoutine != null)
            {
                StopCoroutine(curriFrameRoutine);
            }
            curriFrameRoutine = StartCoroutine(iFrames());
            if (currVigRoutine != null)
            {
                StopCoroutine(currVigRoutine);
            }
            currVigRoutine = StartCoroutine(HitVignette());
            AudioManager.Instance.PlayerHitNoise();
            PlayerPowerUps.Instance.IncreaseRage(1f);
            timeSinceDamage = 0f;
        }

        health += maxHealth * sign * percentage;

        if (health > maxHealth)
        {
            health = maxHealth;
        }

        healthBarUIScript.ChangeHealthVisual();
    }

    // i frames for 0.3 seconds after player takes damage
    private IEnumerator iFrames()
    {
        player.layer = 9;

        yield return new WaitForSeconds(0.3f);

        player.layer = 7;
        curriFrameRoutine = null;
    }

    private IEnumerator HitVignette()
    {
        if (bloomOverride == null || vignetteOverride == null)
        {
            yield break;
        }

        float duration = 1f;
        float elapsedTime = 0f;

        bloomOverride.intensity.Override(baseBloomIntensity * 10f);
        vignetteOverride.intensity.Override(baseVignetteIntensity);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            bloomOverride.intensity.Override(Mathf.Lerp(baseBloomIntensity * 10f, baseBloomIntensity, t));
            vignetteOverride.intensity.Override(Mathf.Lerp(baseVignetteIntensity, 0f, t));
            
            yield return null;
        }

        vignetteOverride.intensity.Override(0f);
        bloomOverride.intensity.Override(baseBloomIntensity);
        currVigRoutine = null;
    }

    public void ResetPlayerStuff()
    {
        ChangeHealth(maxHealth);
        PlayerManager.Instance.SetPlayerActive(true);
        playerTransform.position = new Vector2(0, 0);
        player.layer = 7;
        vignetteOverride.intensity.Override(0f);
        bloomOverride.intensity.Override(baseBloomIntensity);

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
            timeSinceDamage += Time.deltaTime;
            if (timeSinceDamage >= 15)
            {
                ChangeHealth(2);
                yield return new WaitForSeconds(1f);
            }
            else if (timeSinceDamage >= 10)
            {
                ChangeHealth(1);
                yield return new WaitForSeconds(1f);
            }
            yield return null;
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
