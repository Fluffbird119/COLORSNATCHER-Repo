using UnityEngine;
using System.Collections;

public class PlayerPowerUps : MonoBehaviour
{
    public static PlayerPowerUps Instance { get; private set; }

    [Header("NonPowerUp Variables")]
    [SerializeField] private Animator tentacleAnim;
    [SerializeField] private Animator EyeAnim;

    [SerializeField] SpriteRenderer head;
    [SerializeField] SpriteRenderer eye;
    [SerializeField] SpriteRenderer attackTentacle;
    [SerializeField] SpriteRenderer legTentacles;

    [SerializeField] private GameObject player;

    [Header("Rage Variables")]
    #region Rage Variables
    [SerializeField] GameObject RageBarObject;
    [SerializeField] RageBarUI rageBarUIScript;

    public bool beatenRed = false; // bool for now cuz lazy can make it better latter

    private float oldSpeed = 15f;

    [SerializeField] float rage = 0f; 
    [SerializeField] float maxRage; // kill 20 eneimes to get 20 rage and able to use powerup
    [SerializeField] float rageDuration;
    private bool isRaged = false;
    #endregion

    [Header("Dash Variables")]
    #region Dash variables
    [SerializeField] GameObject DashBarObject;
    [SerializeField] private PlayerMovement movementScript;
    [SerializeField] private DashBarUI dashBarUIScript;

    public bool beatenYellow = false;

    [SerializeField] private float curDashCooldown;
    [SerializeField] private float maxDashCooldown;

    [SerializeField] private float dashMoveSpeed;
    [SerializeField] private float dashDuration;
    // private int numDashes = 0;
    [SerializeField] private int maxDashes;
    private bool canDash = true;
    private bool canQueueDash = true;
    private bool dashQueued = false;
    private Coroutine currDashRoutine;
    [SerializeField] private bool isDashing = false;
    #endregion

    [Header("Vine Attack Variables")]
    [SerializeField] GameObject VineBarObject;
    [SerializeField] GameObject VineBallPrefab;
    [SerializeField] Transform vineBallStartingPoint;
    [SerializeField] private float fireForce = 20f;

    public bool beatenGreen;

    [SerializeField] private float curVineCooldown;
    [SerializeField] private float maxVineCooldown;

    private VineBarUI vineBarUI;
    [SerializeField] private GameObject dashParticlePrefab;
    private ParticleSystem particleSys;


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
        vineBarUI = VineBarObject.GetComponent<VineBarUI>();

        rageBarUIScript.UpdateRageVisual();

        RageBarObject.SetActive(false);
        DashBarObject.SetActive(false);
        VineBarObject.SetActive(false);

        particleSys = dashParticlePrefab.GetComponent<ParticleSystem>();
    }

    void Update()
    {
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
        if (beatenRed)
        {
            RageBarObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Q) && beatenRed && rage >= maxRage)
            {
                RagePowerUpEffect();
            }
        } 

        if (beatenYellow)
        {
            DashBarObject.SetActive(true);

            RechargeDash();

            if (Input.GetKeyDown(KeyCode.LeftShift) && beatenYellow && curDashCooldown >= maxDashCooldown / maxDashes)
            {
                Dash();
            }
        }

        if (beatenGreen)
        {
            VineBarObject.SetActive(true);

            RechargeVine();
            
            if (Input.GetKeyDown(KeyCode.E) && beatenGreen && curVineCooldown >= maxVineCooldown)
            {
                ShootVineBall();
            }
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
        rage = 0;
        rageBarUIScript.UpdateRageVisual();
        isRaged = true;

        oldSpeed = PlayerMovement.Instance.GetSpeed();
        PlayerMovement.Instance.SetSpeed(oldSpeed * 1.5f);

        // turn head and eye and tentacle red
        head.color = Color.red;
        eye.color = Color.red;
        attackTentacle.color = Color.red;
        legTentacles.color = Color.red;

        // increase animation speed of tentacle and speed of eye to slow down start lag. also just removes start lag now
        tentacleAnim.speed = 3f;
        EyeAnim.speed = 2f;
        PlayerAttack.Instance.removeIdleToBlinkET();

        // wait for duration of buff then turn back to normal
        StartCoroutine(rageTimer());
    }

    private IEnumerator rageTimer()
    {
        yield return new WaitForSeconds(rageDuration);

        head.color = Color.white;
        eye.color = Color.white;
        attackTentacle.color = Color.white;
        legTentacles.color = Color.white;
        isRaged = false;

        tentacleAnim.speed = 2f;
        EyeAnim.speed = 2f;
        PlayerAttack.Instance.addIdleToBlinkET();

        PlayerMovement.Instance.SetSpeed(oldSpeed);
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
        // set cooldown back to zero
        curDashCooldown -= maxDashCooldown / maxDashes;

        // turn head and eye and tentacle red
        head.color = Color.yellow;
        eye.color = Color.yellow;
        attackTentacle.color = Color.yellow;
        legTentacles.color = Color.yellow;

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
        if (curDashCooldown < maxDashCooldown)
        {
            curDashCooldown += 1f * Time.deltaTime;
            curDashCooldown = Mathf.Clamp(curDashCooldown, 0f, maxDashCooldown); // Keep within bound

            dashBarUIScript.UpdateDashVisual();
        }
    }

    private IEnumerator StartDash()
    {
        while (dashQueued) {
            if (dashParticlePrefab != null && particleSys != null) {
                particleSys.Play();
            }

            dashQueued = false;
            PlayerMovement.Instance.SetSpeed(dashMoveSpeed);
            player.layer = 6;

            yield return new WaitForSeconds((dashDuration / maxDashes) - 0.05f);

            canQueueDash = true;

            yield return new WaitForSeconds(0.05f);
        }

        PlayerMovement.Instance.SetSpeed(dashMoveSpeed / 2f);
        player.layer = 9;

        canDash = true;

        yield return new WaitForSeconds(dashDuration / 4f);

        if (isRaged)
        {
            PlayerMovement.Instance.SetSpeed(15f * 1.5f);
        }
        else {
            PlayerMovement.Instance.SetSpeed(15f); // hard coded value cuz thats just what the player move speed is as of rn
        }

        head.color = Color.white;
        eye.color = Color.white;
        attackTentacle.color = Color.white;
        legTentacles.color = Color.white;
        isDashing = false;
        player.layer = 7;
        particleSys.Stop();
    }

    public bool getIsDashing()
    {
        return isDashing;
    }

    public void setIsDashing(bool value)
    {
        isDashing = value;
    }

    public float getDashCurCooldown()
    {
        return curDashCooldown;
    }
    
    public float getDashMaxCooldown()
    {
        return maxDashCooldown;
    }
    #endregion

    #region Vine Ball Attack
    private void RechargeVine()
    {
        if (curVineCooldown < maxVineCooldown)
        {
            curVineCooldown += 1f * Time.deltaTime;
            curVineCooldown = Mathf.Clamp(curVineCooldown, 0f, maxVineCooldown);

            vineBarUI.UpdateVineVisual();
        }
    }

    private void ShootVineBall()
    {
        curVineCooldown = 0;

        GameObject vineBall = Instantiate(VineBallPrefab, vineBallStartingPoint.position, vineBallStartingPoint.rotation);
        vineBall.GetComponent<Rigidbody2D>().AddForce(vineBallStartingPoint.up * fireForce, ForceMode2D.Impulse);
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
