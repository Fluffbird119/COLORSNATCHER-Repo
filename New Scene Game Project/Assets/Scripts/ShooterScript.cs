using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

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
    public bool aimIncreaser = false;
    public float moveSpeed = 2f;
    public bool isStunned = false;
    public GameObject hitParticlePrefab;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        nextFireTime = Time.time + Random.Range(0.3f, 1f);
        reducedNextFireTime = Time.time + 3f;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerRB = player.GetComponent<Rigidbody2D>();
        }
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

            if ((Time.time >= nextFireTime) || (Vector2.Distance(playerTransform.position, transform.position) > 60f && Time.time >= reducedNextFireTime))
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
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
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
        Vector2 fireDirection = (playerPosition + (distance > 60f ? (distanceMovePredict * playerRB.linearVelocity) : new Vector2(0, 0)) - firePointPosition).normalized;
        float maxSpreadAngle;
        if (Vector2.Distance(transform.position, playerTransform.position) > 50f)
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isStunned && isWalker)
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

        if (currentHealth <= 0)
        {
            Die();
        }
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
        Destroy(gameObject);
    }
    private void StunEnemy()
    {
        isStunned = true;
    }
    private void UnStunEnemy()
    {
        isStunned = false;
    }
}