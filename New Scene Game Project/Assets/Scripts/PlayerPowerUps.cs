using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class PlayerPowerUps : MonoBehaviour
{
    public static PlayerPowerUps Instance { get; private set; }

    [Header("NonPowerUp Variables")]
    public Animator tentacleAnim;
    public Animator EyeAnim;

    public SpriteRenderer head;
    public SpriteRenderer eye;
    public SpriteRenderer attackTentacle;
    public SpriteRenderer legTentacles;

    private bool isStunned; // grabs the isStunned value from PlayerMovement this variable is only changed in PlayerMovement

    [SerializeField] private GameObject player;
    private Rigidbody2D playerRB;
    [SerializeField] private Light2D playerLight;
    private float playerLightRadius;
    private float playerLightFallOff;
    private bool cooldownsPaused;

    [Header("Rage Variables")]
    #region Rage Variables
    public bool beatenRed = false; // bool for now cuz lazy can make it better latter
    [SerializeField] GameObject RageBarObject;
    [SerializeField] RageBarUI rageBarUIScript;

    private float oldSpeed = 15f;

    [SerializeField] float rage = 0f; 
    [SerializeField] float maxRage; // kill 20 eneimes to get 20 rage and able to use powerup (not true its 15)
    [SerializeField] float rageDuration;
    private bool isRaged = false;
    private float rageDrainRate;
    private float rageStache;
    public static event Action<bool> RageStateChanged;
    #endregion

    [Header("Dash Variables")]
    #region Dash variables 
    public bool beatenYellow = false;
    [SerializeField] GameObject DashBarObject;
    [SerializeField] private PlayerMovement movementScript;
    [SerializeField] private DashBarUI dashBarUIScript;

    [SerializeField] private float curDash1Cooldown;
    [SerializeField] private float curDash2Cooldown; // right lung dash should be used first before dash1
    [SerializeField] private float maxIndividualDashCooldown; // max dash cooldown for ONE!!!! DASH

    [SerializeField] private float dashMoveSpeed;
    [SerializeField] private float dashDuration;
    private float currDashDuration;
    [SerializeField] private Collider2D dashTrigger;
    // private int numDashes = 0;
    private bool canDash = true;
    private bool canQueueDash = true;
    private bool dashQueued = false;
    private Coroutine currDashRoutine;
    private bool isDashing = false;

    [SerializeField] private GameObject dashParticlePrefab;
    [SerializeField] private GameObject rageDashParticlePrefab;
    [SerializeField] private GameObject gunkDashParticlePrefab;
    private ParticleSystem particleSys;
    private ParticleSystem rageParticleSys;
    private ParticleSystem gunkParticleSys;
    #endregion

    [Header("Vine Attack Variables")]
    #region Vine Variables
    public bool beatenGreen;
    [SerializeField] GameObject VineBarObject;
    [SerializeField] GameObject VineBallPrefab;
    [SerializeField] Transform vineBallStartingPoint;
    [SerializeField] private float fireForce = 20f;

    [SerializeField] private float curVineCooldown;
    [SerializeField] private float maxVineCooldown;
    private GameObject vineBall;
    private VineBarUI vineBarUI;
    public bool gunked;
    #endregion


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

        DialogueSystem.DialogueStateChange += PauseCooldowns;
        EnemyScript.DashTriggerEnemyHit += AddDash;
        //Bullet.DashTriggerBulletHit += AddDash;
        ShooterScript.DashTriggerShooterHit += AddDash;
        ItemPickup.OnRagePickup += IncreaseRage;
        ItemPickup.OnDashPickup += IncreaseDash;
    }

    void Start()
    {
        rageDrainRate = maxRage / rageDuration;

        vineBarUI = VineBarObject.GetComponent<VineBarUI>();

        rageBarUIScript.UpdateRageVisual();

        RageBarObject.SetActive(false);
        DashBarObject.SetActive(false);
        VineBarObject.SetActive(false);

        particleSys = dashParticlePrefab.GetComponent<ParticleSystem>();
        rageParticleSys = rageDashParticlePrefab.GetComponent<ParticleSystem>();
        gunkParticleSys = gunkDashParticlePrefab.GetComponent<ParticleSystem>();

        playerRB = player.GetComponent<Rigidbody2D>();

        oldSpeed = PlayerMovement.Instance.GetSpeed();
        playerLightRadius = playerLight.pointLightOuterRadius;
        playerLightFallOff = playerLight.falloffIntensity;

        vineBall = Instantiate(VineBallPrefab, new Vector3(1000, 0, 0), HeadLookAtMouse.aimAngle);
        vineBall.SetActive(false);
    }

    void Update()
    {
        // wow look how hard it is to make debug keybinds only in editor derek
        #if UNITY_EDITOR 
        if (Input.GetKeyDown(KeyCode.F))
        {
            EnemyManager.Instance.DeleteEnemies();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            beatenRed = true;
            beatenYellow = true;
            beatenGreen = true;
            rage = maxRage;
            rageBarUIScript.UpdateRageVisual();
            curDash1Cooldown = maxIndividualDashCooldown;
            curDash2Cooldown = maxIndividualDashCooldown;
            dashBarUIScript.UpdateDashVisual();
            curVineCooldown = maxVineCooldown;
            vineBarUI.UpdateVineVisual();
        }
        #endif

        isStunned = PlayerMovement.Instance.GetIsStunned();

        if (beatenRed)
        {
            RageBarObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Q) && beatenRed && !isRaged && rage >= maxRage && !isStunned)
            {
                RagePowerUpEffect();
            }
        } 

        if (beatenYellow)
        {
            DashBarObject.SetActive(true);

            RechargeDash();

            if (Input.GetKeyDown(KeyCode.LeftShift) && beatenYellow && curDash1Cooldown >= maxIndividualDashCooldown && !isStunned)
            {
                Dash();
            }
            if (isDashing && particleSys.isEmitting && playerRB.linearVelocity.magnitude <= 0.1f)
            {
                particleSys.Stop();
            }
            else if (isDashing && !particleSys.isEmitting && playerRB.linearVelocity.magnitude >= 0.5f)
            {
                particleSys.Play();
            }
        }

        if (beatenGreen)
        {
            VineBarObject.SetActive(true);

            RechargeVine();
            
            if (Input.GetKeyDown(KeyCode.E) && beatenGreen && curVineCooldown >= maxVineCooldown && !isStunned)
            {
                ShootVineBall();
            }
        }

        if (isRaged && !cooldownsPaused)
        {
            rage -= rageDrainRate * Time.deltaTime;
            rageBarUIScript.UpdateRageVisual();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("VineAOE"))
        {
            gunked = isDashing;
            if (isDashing)
            {
                if (gunkDashParticlePrefab != null && gunkParticleSys != null && gunked)
                {   
                    gunkParticleSys.Play();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (gunked && collision.CompareTag("VineAOE"))
        {
            StartCoroutine(GunkDuration());
        }
    }

#region Rage PowerUp Methods
    public void IncreaseRage(float amount)
    {
        if (beatenRed)
        {
            if (isRaged)
            {
                rageStache += amount;
                rage += 15 * amount / (rageStache + 14);
            }
            else
            {
                rage += amount;
                if (rage > maxRage)
                {
                    rage = maxRage;
                }
            }

            rageBarUIScript.UpdateRageVisual();
        }
    }

    void RagePowerUpEffect()
    {
        isRaged = true;
        RageStateChanged?.Invoke(true);

        if (!isDashing) {
            PlayerMovement.Instance.SetSpeed(oldSpeed * 1.5f);
        }
        else
        {
            dashTrigger.enabled = true;
            if (rageDashParticlePrefab != null && rageParticleSys != null && isRaged)
            {
                rageParticleSys.Play();
            }
        }

        // turn head and eye and tentacle red
        head.color = Color.red;
        eye.color = Color.red;
        attackTentacle.color = Color.red;
        legTentacles.color = Color.red;
        playerLight.color = Color.red;
        playerLight.pointLightOuterRadius *= 1.2f;
        playerLight.falloffIntensity -= 0.25f;

        // increase animation speed of tentacle and speed of eye to slow down start lag. also just removes start lag now
        tentacleAnim.speed = 3f;
        EyeAnim.speed = 2f;

        // wait for duration of buff then turn back to normal
        StartCoroutine(RageTimer());
    }

    private IEnumerator RageTimer()
    {
        yield return new WaitUntil(() => rage <= 0);

        isRaged = false;
        RageStateChanged?.Invoke(false);
        rage = Mathf.Floor(Mathf.Log(2 * rageStache + 1, 2));
        if (rage >= maxRage)
        {
            rage = maxRage;
        }
        rageStache = 0f;
        rageBarUIScript.UpdateRageVisual();

        if (!isDashing) {
            head.color = Color.white;
            eye.color = Color.white;
            attackTentacle.color = Color.white;
            legTentacles.color = Color.white;
            playerLight.color = Color.white;
            playerLight.pointLightOuterRadius = playerLightRadius;
            playerLight.falloffIntensity = playerLightFallOff;
        }
        else
        {
            // rage ends in the middle of a dash
            playerLight.pointLightOuterRadius /= 1.2f;
            playerLight.falloffIntensity += 0.25f;
            dashTrigger.enabled = false;
            if (rageDashParticlePrefab != null && rageParticleSys != null && rageParticleSys.isPlaying)
            {
                rageParticleSys.Stop();
            }
        }

        tentacleAnim.speed = 1f;
        EyeAnim.speed = 1f;

        if (!isDashing)
        {
            PlayerMovement.Instance.SetSpeed(oldSpeed);
        }
    }

    public bool getIsRaged()
    {
        return isRaged;
    }

    public void setIsRaged(bool value)
    {
        isRaged = value;
        RageStateChanged?.Invoke(value);
    }

    public float getRage()
    {
        return rage;
    }

    public float getMaxRage()
    {
        return maxRage;
    }
#endregion

#region Lightning Dash PowerUp Methods
    private void Dash()
    {
        // set movespeed really high for dashDuration
        if (canDash)
        {
            if (currDashRoutine != null)
            {
                StopCoroutine(currDashRoutine);
                particleSys.Stop();
            }
            
            canDash = false;
            canQueueDash = false;
            dashQueued = true;
            isDashing = true;
            currDashRoutine = StartCoroutine(StartDash());
        }
        else if (canQueueDash)
        {
            //Debug.Log("Queued Dash");
            dashQueued = true;
        }
    }

    private void RechargeDash()
    {
        if (cooldownsPaused)
        {
            return;
        }

        if (curDash1Cooldown < maxIndividualDashCooldown)
        {
            curDash1Cooldown += Time.deltaTime * (isRaged ? 1.5f : 1f);
            curDash1Cooldown = Mathf.Clamp(curDash1Cooldown, 0f, maxIndividualDashCooldown);
        }
        else if (curDash2Cooldown < maxIndividualDashCooldown)
        {
            curDash2Cooldown += Time.deltaTime * (isRaged ? 1.5f : 1f);
            curDash2Cooldown = Mathf.Clamp(curDash2Cooldown, 0f, maxIndividualDashCooldown);
        }

        dashBarUIScript.UpdateDashVisual();
    }

    private void IncreaseDash(float amount)
    {
        if (curDash1Cooldown < maxIndividualDashCooldown)
        {
            curDash1Cooldown += amount;
            amount = curDash1Cooldown - maxIndividualDashCooldown;
            curDash1Cooldown = Mathf.Clamp(curDash1Cooldown, 0f, maxIndividualDashCooldown);
        }
        if (curDash2Cooldown < maxIndividualDashCooldown && amount > 0)
        {
            curDash1Cooldown += amount;
            curDash2Cooldown = Mathf.Clamp(curDash2Cooldown, 0f, maxIndividualDashCooldown);
        }
    }

    private IEnumerator StartDash()
    {
        while (dashQueued) {
            dashTrigger.enabled = true;
            currDashDuration = dashDuration * (isRaged ? 1.5f : 1f);
            if (isRaged)
            {
                // activate dash burst
            }

            if (dashParticlePrefab != null && particleSys != null) {
                particleSys.Play();
            }
            if (rageDashParticlePrefab != null && rageParticleSys != null && isRaged)
            {
                rageParticleSys.Play();
            }
            if (gunkDashParticlePrefab != null && gunkParticleSys != null && gunked)
            {
                gunkParticleSys.Play();
            }

            // set cooldown back to zero
            curDash1Cooldown = curDash2Cooldown;
            curDash2Cooldown = 0;

            // turn head and eye and tentacle yellow
            head.color = Color.yellow;
            eye.color = Color.yellow;
            attackTentacle.color = Color.yellow;
            legTentacles.color = Color.yellow;
            playerLight.color = Color.yellow;
            playerLight.pointLightOuterRadius *= 1.2f;

            dashQueued = false;
            PlayerMovement.Instance.SetSpeed(dashMoveSpeed * (isRaged ? 1.2f : 1f));
            player.layer = 6;

            float timeInDash = 0f;
            while (timeInDash < currDashDuration - 0.05f)
            {
                timeInDash += Time.deltaTime;
                yield return null;
            }

            canQueueDash = true;

            yield return new WaitForSeconds(0.05f);
        }

        PlayerMovement.Instance.SetSpeed(dashMoveSpeed / 2f);
        player.layer = 9;
        dashTrigger.enabled = false;

        canDash = true;

        yield return new WaitForSeconds(dashDuration / 4f);

        if (isRaged)
        {
            PlayerMovement.Instance.SetSpeed(oldSpeed * 1.5f);
        }
        else {
            PlayerMovement.Instance.SetSpeed(oldSpeed);
        }

        // setting player back to red if raged or back to white if not raged when the dash ends
        if (!isRaged) {
            head.color = Color.white;
            eye.color = Color.white;
            attackTentacle.color = Color.white;
            legTentacles.color = Color.white;
            playerLight.color = Color.white;
            playerLight.pointLightOuterRadius = playerLightRadius;
            playerLight.falloffIntensity = playerLightFallOff;
        }
        else
        {
            head.color = Color.red;
            eye.color = Color.red;
            attackTentacle.color = Color.red;
            legTentacles.color = Color.red;
            playerLight.color = Color.red;
            playerLight.pointLightOuterRadius /= 1.2f;
        }

        isDashing = false;
        player.layer = 7;
        particleSys.Stop();
        rageParticleSys.Stop();
        gunkParticleSys.Stop();
        currDashRoutine = null;
    }

    public bool getIsDashing()
    {
        return isDashing;
    }

    public void setIsDashing(bool value)
    {
        isDashing = value;
    }

    public float getDash1CurCooldown()
    {
        return curDash1Cooldown;
    }
    public float getDash2CurCooldown()
    {
        return curDash2Cooldown;
    }

    public float getDashMaxCooldown()
    {
        return maxIndividualDashCooldown;
    }

    private void AddDash()
    {
        if (currDashDuration < (isRaged ? 3 : 2) * dashDuration)
        currDashDuration += dashDuration / 3f;
    }
#endregion

#region Goop Ball Attack
    private void RechargeVine()
    {
        if (cooldownsPaused)
        {
            return;
        }
        
        if (curVineCooldown < maxVineCooldown)
        {
            curVineCooldown += Time.deltaTime;
            curVineCooldown = Mathf.Clamp(curVineCooldown, 0f, maxVineCooldown);

            vineBarUI.UpdateVineVisual();
        }
    }

    private void ShootVineBall()
    {
        curVineCooldown = 0;

        vineBall.SetActive(true);
        vineBall.transform.position = vineBallStartingPoint.position;
        vineBall.GetComponent<Rigidbody2D>().AddForce(vineBallStartingPoint.up * fireForce, ForceMode2D.Impulse);
        ParticleSystem vineParticles = vineBall.transform.GetChild(0).GetComponent<ParticleSystem>();
        if (vineParticles != null)
        {
            float radiansZ = HeadLookAtMouse.aimAngle.eulerAngles.z * Mathf.Deg2Rad;
            ParticleSystem.MainModule vinePartMain = vineParticles.main;
            vinePartMain.startRotation = new ParticleSystem.MinMaxCurve(-radiansZ);
        }
    }

    public float getVineCurCooldown()
    {
        return curVineCooldown;
    }

    public float getVineMaxCooldown()
    {
        return maxVineCooldown;
    }

    private IEnumerator GunkDuration()
    {
        if (!gunked)
        {
            yield break;
        }
        yield return new WaitForSeconds(1f);
        gunked = false;
        if (isDashing)
        {
            if (gunkDashParticlePrefab != null && gunkParticleSys != null)
            {   
                gunkParticleSys.Stop();
            }
        }
    }
#endregion

#region General Methods
    private void PauseCooldowns(bool state)
    {
        cooldownsPaused = state;
    }

    void OnDestroy()
    {
        DialogueSystem.DialogueStateChange -= PauseCooldowns;
        EnemyScript.DashTriggerEnemyHit -= AddDash;
        //Bullet.DashTriggerBulletHit -= AddDash;
        ShooterScript.DashTriggerShooterHit -= AddDash;
        ItemPickup.OnRagePickup -= IncreaseRage;
        ItemPickup.OnDashPickup -= IncreaseDash;
    }
#endregion
}
