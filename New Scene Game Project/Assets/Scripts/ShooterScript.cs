using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShooterScript : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootingRange = 8f;
    public float fireRate = 1.5f;
    [SerializeField] private float distanceRateIncrease = 3f;
    [SerializeField] private float distanceAccuracyIncrease = 5f;
    [SerializeField] private float distanceMovePredict = 0f;
    public float bulletSpeed = 30f;
    [SerializeField] private float bulletDamage;
    [SerializeField] private float bulletSizeMod;
    public float rageDamageBuff = 1.5f;
    [Range(0f, 1f)]
    public float accuracy = 0.8f;
    public int maxHealth = 3;
    private float currentHealth;
    public float knockbackForce = 3f;
    public float knockbackDuration = 0.15f;
    private bool isKnockedBack = false;
    private Transform playerTransform;
    private Rigidbody2D playerRB;
    private Rigidbody2D rb;
    private float nextFireTime = 0f;
    private float reducedNextFireTime = 0f;
    public bool isWalker = false;
    [SerializeField] private float distanceSpeedMod;
    private float buffDistance;
    public bool aimIncreaser = false;
    public float moveSpeed = 2f;
    private float oldSpeed;
    public bool isStunned = false;
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
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        nextFireTime = Time.time + Random.Range(0.3f, 1f);
        reducedNextFireTime = Time.time + 3f;

        GameObject player = GameObject.FindWithTag("Player");
        lightRadius = spotLight.pointLightOuterRadius;
        lightFallOff = spotLight.falloffIntensity;
        lightColor = spotLight.color;
        if (player != null)
        {
            playerTransform = player.transform;
            playerRB = player.GetComponent<Rigidbody2D>();
        }

        oldSpeed = moveSpeed;
        animSpeed = animator.speed;
        buffDistance = CameraSmoothFollowPlayer.centerEdgeDistance + 15f;
        CameraSmoothFollowPlayer.ScreenSizeChange += UpdateBuffDistance;
    }

    void Update()
    {
        if (playerTransform == null || isKnockedBack) return;

        if (isStunned)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (aimIncreaser && accuracy < 1f)
        {
            accuracy += Time.deltaTime / 200f;
            if (accuracy > 1f)
            {
                accuracy = 1f;
            }
        }

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance <= shootingRange)
        {
            if (!isWalker)
            {
                rb.linearVelocity = Vector2.zero;
            }
            else
            {
                moveSpeed /= 2;
                MoveTowardsPlayer();
                moveSpeed *= 2;
            }
            RotateTowardsPlayer();

            if ((Time.time >= nextFireTime) || (distance > buffDistance && Time.time >= reducedNextFireTime))
            {
                Shoot();
                nextFireTime = Time.time + (1f / fireRate);
                reducedNextFireTime = Time.time + (1f / (fireRate * distanceRateIncrease));
            }
        }
        else if (isWalker)
        {
            MoveTowardsPlayer();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void MoveTowardsPlayer()
    {
        float distance = Vector2.Distance(playerTransform.position, transform.position);
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed * ((distance > buffDistance) ? distanceSpeedMod : 1f);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void RotateTowardsPlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void Shoot()
    {
        if (isStunned)
        {
            return;
        }

        if (bulletPrefab == null || firePoint == null) return;
        Vector2 playerPosition = playerTransform.position;
        Vector2 firePointPosition = firePoint.position;
        float distance = Vector2.Distance(playerPosition, firePointPosition);
        Vector2 fireDirection = (playerPosition + (distance > buffDistance ? (distanceMovePredict * playerRB.linearVelocity) : new Vector2(0, 0)) - firePointPosition).normalized;
        float maxSpreadAngle;
        if (Vector2.Distance(transform.position, playerTransform.position) > buffDistance)
        {
            maxSpreadAngle = Mathf.Max(1f - (accuracy * distanceAccuracyIncrease), 0) * 45f;
        }
        else
        {
            maxSpreadAngle = (1f - accuracy) * 45f;
        }
        float randomAngle = Random.Range(-maxSpreadAngle, maxSpreadAngle);
        Quaternion spreadRotation = Quaternion.Euler(0, 0, randomAngle);
        Vector2 finalDirection = spreadRotation * fireDirection;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = finalDirection * bulletSpeed;
        }
        bulletScript.SetSize(bulletSizeMod);
        bulletScript.SetDamage(bulletDamage);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tentacle"))
        {
            Vector2 knockbackDir = (transform.position - collision.transform.position).normalized;
            TakeDamage(1, knockbackDir);

            AudioManager.Instance.HitNoise();
        }
        if (collision.CompareTag("VineAOE"))
        {
            StunEnemy();
        }
        if (collision.CompareTag("Player") && collision.gameObject.layer == LayerMask.NameToLayer("Dashing"))
        {
            if (PlayerPowerUps.Instance.getIsRaged()) {
                TakeDamage(1, new Vector2(0, 0));
                CameraShake.Instance.Shake(0.2f, 0.3f, CameraShake.ShakeAxis.Horizontal);
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("VineAOE"))
        {
            StunEnemy();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.layer != LayerMask.NameToLayer("iframes") && !isStunned && isWalker && collision.gameObject.layer != LayerMask.NameToLayer("Dashing"))
        {
            PlayerHealth.Instance.ChangeHealth(-1);
            CameraShake.Instance.Shake(0.3f, 0.5f, CameraShake.ShakeAxis.Vertical);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.layer != LayerMask.NameToLayer("iframes") && !isStunned && isWalker && collision.gameObject.layer != LayerMask.NameToLayer("Dashing"))
        {
            PlayerHealth.Instance.ChangeHealth(-1);
            CameraShake.Instance.Shake(0.3f, 0.5f, CameraShake.ShakeAxis.Vertical);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("VineAOE"))
        {
            UnStunEnemy();
        }
    }

    public void TakeDamage(int damageAmount, Vector2 knockbackDirection)
    {
        if (PlayerPowerUps.Instance.getIsRaged())
        {
            currentHealth -= damageAmount * rageDamageBuff;
        }
        else {
            currentHealth -= damageAmount;
        }
        
        StartCoroutine(ApplyKnockback(knockbackDirection));
        SpawnHitParticles();
        StartCoroutine(HitFlash());
        AudioManager.Instance.HitNoise();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator HitFlash()
    {
        spotLight.pointLightOuterRadius *= 1.2f;
        spotLight.falloffIntensity -= 0.15f;
        yield return new WaitForSeconds(0.1f);
        spotLight.pointLightOuterRadius = lightRadius;
        spotLight.falloffIntensity = lightFallOff;
    }

    private void SpawnHitParticles()
    {
        if (hitParticlePrefab == null) return;
        GameObject particles = Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
        ParticleSystem ps = particles.GetComponentInChildren<ParticleSystem>();
        float lifetime = 2f;
        if (ps != null)
        {
            ps.Play();
            lifetime = ps.main.duration + ps.main.startLifetime.constantMax;
        }
        Destroy(particles, lifetime);
    }

    private IEnumerator ApplyKnockback(Vector2 direction)
    {
        isKnockedBack = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
        rb.linearVelocity = Vector2.zero;
        isKnockedBack = false;
    }

    private void Die()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.killsThisWave++;
        }
        PlayerPowerUps.Instance.IncreaseRage(1f);
        EnemyManager.Instance.RemoveEnemy(gameObject);
        Destroy(gameObject);
    }
    private void StunEnemy()
    {
        isStunned = true;
        moveSpeed = 0;
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
        moveSpeed = oldSpeed;
        rb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
        animator.speed = animSpeed;
        spotLight.color = lightColor;
        nextFireTime = Time.time + (1f / fireRate);
        reducedNextFireTime = Time.time + (1f / (fireRate * distanceRateIncrease));
        foreach (SpriteRenderer renderer in bodyParts)
        {
            renderer.color = Color.white;
        }
    }

    private void UpdateBuffDistance(float centerEdgeDistance)
    {
        buffDistance = centerEdgeDistance + 15f;
    }

    void OnDestroy()
    {
        CameraSmoothFollowPlayer.ScreenSizeChange -= UpdateBuffDistance;
    }
}