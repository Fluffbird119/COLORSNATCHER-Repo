using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private PlayerPowerUps playerPowerUpScript;

    public float speed = 3f;
    private float oldSpeed;
    public float stoppingDistance = 0f;
    public int maxHealth = 5;
    private float currentHealth;
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;
    private bool isKnockedBack = false;
    public float rageDamageBuff = 1.5f;
    public bool proceduralSpeed = false;
    private Transform playerTransform;
    private Rigidbody2D rb;
    private float lastSpeedBoostTime = 0f;
    private int numSpeedBoosts = 0;
    [SerializeField] private float distanceSpeedMod = 1f;
    public bool reincarnates = false;
    private bool reincarnated = false;
    private bool isInDeathState = false;

    [SerializeField] private PlayerHealth healthScript;

    [SerializeField] private bool isStunned;

    public float spriteAngleOffset = -90f;

    public GameObject hitParticlePrefab;
    [SerializeField] private SpriteRenderer[] bodyParts;
    [SerializeField] private Animator animator;
    private float animSpeed;
    [SerializeField] private Light2D spotLight;
    private float lightRadius;
    private float lightFallOff;
    private Color lightColor;
    [SerializeField] private Color vineColor;

    void Start()
    {
        oldSpeed = speed;
        isStunned = false;
        animSpeed = animator.speed;

        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        GameObject player = GameObject.FindWithTag("Player");
        lightRadius = spotLight.pointLightOuterRadius;
        lightFallOff = spotLight.falloffIntensity;
        lightColor = spotLight.color;
        if (player != null)
        {
            playerTransform = player.transform;
            playerPowerUpScript = player.GetComponent<PlayerPowerUps>();
        }
        lastSpeedBoostTime = Time.time;
    }

    void FixedUpdate()
    {
        if (isKnockedBack || isInDeathState || isStunned) return;
        if (proceduralSpeed)
        {
            if (Time.time - lastSpeedBoostTime >= 2f + (0.25f * numSpeedBoosts))
            {
                if (numSpeedBoosts < 3)
                {
                    currentHealth += 1;
                }
                speed *= 2f;
                transform.localScale *= 1.25f;
                lastSpeedBoostTime = Time.time + Random.Range(-0.2f, 0.2f);
                numSpeedBoosts++;
            }
        }
        if (playerTransform != null)
        {
            Vector2 direction = (Vector2)playerTransform.position - rb.position;

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            rb.rotation = targetAngle + spriteAngleOffset;

            float distance = Vector2.Distance(rb.position, playerTransform.position);
            if (distance > stoppingDistance)
            {
                Vector2 newPosition = Vector2.MoveTowards(
                    rb.position,
                    playerTransform.position,
                    speed * ((distance > 50f) ? distanceSpeedMod : 1f) * Time.fixedDeltaTime
                );
                rb.MovePosition(newPosition);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isInDeathState) return;
        if (collision.CompareTag("Tentacle"))
        {
            Vector2 knockbackDir = (transform.position - collision.transform.position).normalized;
            TakeDamage(1, knockbackDir);
            CameraShake.Instance.Shake(0.3f, 0.5f, CameraShake.ShakeAxis.Horizontal);

            AudioManager.Instance.HitNoise();
        }
        if (collision.CompareTag("VineAOE"))
        {
            StunEnemy();
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("VineAOE"))
        {
            StunEnemy();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("VineAOE"))
        {
            UnStunEnemy();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.layer != LayerMask.NameToLayer("iframes") && !isInDeathState && !isStunned && collision.gameObject.layer != LayerMask.NameToLayer("Dashing"))
        {
            PlayerHealth.Instance.ChangeHealth(-1);
            CameraShake.Instance.Shake(0.3f, 0.5f, CameraShake.ShakeAxis.Vertical);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.layer != LayerMask.NameToLayer("iframes") && !isInDeathState && !isStunned && !(collision.gameObject.layer == LayerMask.NameToLayer("Dashing")))
        {
            PlayerHealth.Instance.ChangeHealth(-1);
            CameraShake.Instance.Shake(0.3f, 0.5f, CameraShake.ShakeAxis.Vertical);
        }
    }
    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        if (PlayerPowerUps.Instance.getIsRaged())
        {
            currentHealth -= damage * rageDamageBuff;
        }
        else {
            currentHealth -= damage;
        }
        StartCoroutine(ApplyKnockback(knockbackDirection));
        SpawnHitParticles();
        StartCoroutine(HitFlash());
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator HitFlash()
    {
        spotLight.pointLightOuterRadius *= 1.2f;
        spotLight.falloffIntensity -= 0.15f;
        yield return new WaitForSeconds(0.2f);
        spotLight.pointLightOuterRadius = lightRadius;
        spotLight.falloffIntensity = lightFallOff;
    }

    private void SpawnHitParticles()
    {
        if (hitParticlePrefab == null) return;
        GameObject particles = Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
        ParticleSystem ps = particles.GetComponent<ParticleSystem>();
        float lifetime = ps != null ? ps.main.duration + ps.main.startLifetime.constantMax : 2f;
        Destroy(particles, lifetime);
    }

    private IEnumerator ApplyKnockback(Vector2 direction)
    {
        isKnockedBack = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
        isKnockedBack = false;
    }

    private void Die()
    {
        if (reincarnates && !reincarnated)
        {
            PlayerPowerUps.Instance.IncreaseRage(0.5f);
            StartCoroutine(Reincarnate());
            return;
        }
        PlayerPowerUps.Instance.IncreaseRage(1f);
        GameManager.Instance.killsThisWave++;
        EnemyManager.Instance.RemoveEnemy(gameObject);
        Destroy(gameObject);
    }

    private IEnumerator Reincarnate()
    {
        isInDeathState = true;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        RigidbodyType2D originalBodyType = rb.bodyType;
        rb.bodyType = RigidbodyType2D.Kinematic;
        gameObject.layer = LayerMask.NameToLayer("NoCollide");

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.gray;
            spriteRenderer.sortingLayerName = "DeadEnemy";
            animator.speed = 0;
            yield return new WaitForSeconds(5f);
            animator.speed = animSpeed;
            gameObject.layer = LayerMask.NameToLayer("Default");
            reincarnated = true;
            currentHealth = (int)(maxHealth * 1.5f);
            isInDeathState = false;
            rb.bodyType = originalBodyType;
            spriteRenderer.color = originalColor;
            spriteRenderer.sortingLayerName = "Enemy";
            speed *= 3f;
            knockbackDuration /= 3f;
        }
        else
        {
            yield return new WaitForSeconds(5f);
            gameObject.layer = LayerMask.NameToLayer("Default");
            reincarnated = true;
            currentHealth = (int)(maxHealth * 1.5f);
            isInDeathState = false;
            rb.bodyType = originalBodyType;
            speed *= 2.5f;
            knockbackDuration /= 3f;
        }
    }

    private void StunEnemy()
    {
        isStunned = true;
        speed = 0;
        spotLight.color = vineColor;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        animator.speed = 0;
        foreach (SpriteRenderer renderer in bodyParts)
        {
            renderer.color = Color.green;
        }
    }
    private void UnStunEnemy()
    {
        isStunned = false;
        speed = oldSpeed;
        rb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
        animator.speed = animSpeed;
        spotLight.color = lightColor;
        foreach (SpriteRenderer renderer in bodyParts)
        {
            renderer.color = Color.white;
        }
    }
}