using UnityEngine;
using System.Collections;

public class PlayerPowerUps : MonoBehaviour
{
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
    [SerializeField] RageBarUI rageBarUIScript;

    public bool beatenRed = false; // bool for now cuz lazy can make it better latter
    [SerializeField] float rage = 0f; 
    [SerializeField] float maxRage; // kill 20 eneimes to get 20 rage and able to use powerup
    [SerializeField] float rageDuration;
    #endregion

    [Header("Dash Variables")]
    #region Dash variables
    [SerializeField] private PlayerMovement movementScript;
    [SerializeField] private DashBarUI dashBarUIScript;

    public bool beatenYellow = false;

    [SerializeField] private float curDashCooldown;
    [SerializeField] private float maxDashCooldown;

    [SerializeField] private float dashMoveSpeed;
    [SerializeField] private float dashDuration;
    #endregion

    void Start()
    {
        rageBarUIScript.UpdateRageVisual();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && beatenRed && rage >= maxRage)
        {
            RagePowerUpEffect();
        } // i know this fucking else if is gonna fuck me over
        else if (Input.GetKeyDown(KeyCode.Alpha2) && beatenYellow && curDashCooldown >= maxDashCooldown)
        {
            Debug.Log("bojpaijjgapijg");
        }
    }

    #region Rage PowerUp Methods
    public void IncreaseRage(float amount)
    {
        rage += amount;

        if (rage > maxRage)
        {
            rage = maxRage;
        }

        rageBarUIScript.UpdateRageVisual();
    }

    void RagePowerUpEffect()
    {
        rage = 0;
        rageBarUIScript.UpdateRageVisual();

        // turn head and eye and tentacle red
        head.color = Color.red;
        eye.color = Color.red;
        attackTentacle.color = Color.red;
        legTentacles.color = Color.red;

        // increase animation speed of tentacle and speed of eye to slow down start lag
        tentacleAnim.speed = 1.5f;
        EyeAnim.speed = 2f;

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

        tentacleAnim.speed = 0.5f;
        EyeAnim.speed = 0.5f;
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
    
    #endregion


}
