using UnityEngine;
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
    // private int numDashes = 0;
    private bool canDash = true;
    private bool canQueueDash = true;
    private bool dashQueued = false;
    private Coroutine currDashRoutine;
    private bool isDashing = false;

    [SerializeField] private GameObject dashParticlePrefab;
    private ParticleSystem particleSys;
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

    private VineBarUI vineBarUI;
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
        playerRB = player.GetComponent<Rigidbody2D>();

        oldSpeed = PlayerMovement.Instance.GetSpeed();
        playerLightRadius = playerLight.pointLightOuterRadius;
        playerLightFallOff = playerLight.falloffIntensity;
    }

    void Update()
    {
        /*
        if (!isRaged && head.color == Color.red)
        {
            head.color = Color.white;
            eye.color = Color.white;
            attackTentacle.color = Color.white;
            legTentacles.color = Color.white;

            tentacleAnim.speed = 2f;
            EyeAnim.speed = 2f;
            PlayerAttack.Instance.addIdleToBlinkET();

            PlayerMovement.Instance.SetSpeed(oldSpeed);
        }
        else if (!isDashing && head.color == Color.yellow)
        {
            head.color = Color.white;
            eye.color = Color.white;
            attackTentacle.color = Color.white;
            legTentacles.color = Color.white;

            PlayerMovement.Instance.SetSpeed(oldSpeed);
            player.layer = 7;
        }
        */
        // wow look how hard it is to make debug keybinds only in editor derek
        #if UNITY_EDITOR 
        if (Input.GetKeyDown(KeyCode.F))
        {
            EnemyManager.Instance.DeleteEnemies();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            rage = maxRage;
            rageBarUIScript.UpdateRageVisual();
            curDash1Cooldown = maxIndividualDashCooldown;
            curDash2Cooldown = maxIndividualDashCooldown;
            curVineCooldown = maxVineCooldown;
        }
        #endif

        isStunned = PlayerMovement.Instance.GetIsStunned();

        if (beatenRed)
        {
            RageBarObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Q) && beatenRed && rage >= maxRage && !isStunned)
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

        if (isRaged)
        {
            rage -= rageDrainRate * Time.deltaTime;
            rageBarUIScript.UpdateRageVisual();
        }
    }

#region Rage PowerUp Methods
    public void IncreaseRage(float amount)
    {
        if (beatenRed)
        {
            rage += amount;

            if (rage > maxRage)
            {
                rage = maxRage;
            }

            rageBarUIScript.UpdateRageVisual();
        }
    }

    void RagePowerUpEffect()
    {
        isRaged = true;

        if (!isDashing) {
            PlayerMovement.Instance.SetSpeed(oldSpeed * 1.5f);
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
        PlayerAttack.Instance.removeIdleToBlinkET();

        // wait for duration of buff then turn back to normal
        StartCoroutine(RageTimer());
    }

    private IEnumerator RageTimer()
    {
        yield return new WaitUntil(() => rage <= 0);

        isRaged = false;
        rage = 0;
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
        }

        tentacleAnim.speed = 1f;
        EyeAnim.speed = 1f;
        PlayerAttack.Instance.addIdleToBlinkET();

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
            Debug.Log("Queued Dash");
            dashQueued = true;
        }
    }

    private void RechargeDash()
    {
        if (curDash1Cooldown < maxIndividualDashCooldown)
        {
            curDash1Cooldown += Time.deltaTime;
            curDash1Cooldown = Mathf.Clamp(curDash1Cooldown, 0f, maxIndividualDashCooldown);
        }
        else if (curDash2Cooldown < maxIndividualDashCooldown)
        {
            curDash2Cooldown += Time.deltaTime;
            curDash2Cooldown = Mathf.Clamp(curDash2Cooldown, 0f, maxIndividualDashCooldown);
        }

        dashBarUIScript.UpdateDashVisual();
    }

    private IEnumerator StartDash()
    {
        while (dashQueued) {
            // set the collider to be a trigger
            if (isRaged)
            {
                // activate dash burst
            }
            if (dashParticlePrefab != null && particleSys != null) {
                particleSys.Play();
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
            playerLight.falloffIntensity -= 0.25f;

            dashQueued = false;
            PlayerMovement.Instance.SetSpeed(dashMoveSpeed);
            player.layer = 6;

            yield return new WaitForSeconds(dashDuration - 0.05f);

            canQueueDash = true;

            yield return new WaitForSeconds(0.05f);
        }

        PlayerMovement.Instance.SetSpeed(dashMoveSpeed / 2f);
        player.layer = 9;

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
            playerLight.falloffIntensity += 0.25f;
        }

        isDashing = false;
        player.layer = 7;
        particleSys.Stop();
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
#endregion

#region Goop Ball Attack
    private void RechargeVine()
    {
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

        GameObject vineBall = Instantiate(VineBallPrefab, vineBallStartingPoint.position, HeadLookAtMouse.aimAngle);
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
#endregion
}
