using System.Collections;
using UnityEngine;

public class ShooterScript : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootingRange = 8f;
    public float fireRate = 1.5f;
    public float bulletSpeed = 30f;
    public int damage = 1;
    [Range(0f, 1f)]
    public float accuracy = 0.8f;
    public int maxHealth = 3;
    private int currentHealth;
    public float knockbackForce = 3f;
    public float knockbackDuration = 0.15f;
    private bool isKnockedBack = false;
    private Transform playerTransform;
    private Rigidbody2D rb;
    private float nextFireTime = 0f;
    public bool isWalker = false;
    public float moveSpeed = 2f;
    public bool isStunned = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
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

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance <= shootingRange)
        {
            rb.linearVelocity = Vector2.zero;
            RotateTowardsPlayer();

            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + (1f / fireRate);
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
        Vector2 fireDirection = (playerTransform.position - firePoint.position).normalized;
        float maxSpreadAngle = (1f - accuracy) * 45f;
        float randomAngle = Random.Range(-maxSpreadAngle, maxSpreadAngle);
        Quaternion spreadRotation = Quaternion.Euler(0, 0, randomAngle);
        Vector2 finalDirection = spreadRotation * fireDirection;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = finalDirection * bulletSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tentacle"))
        {
            Vector2 knockbackDir = (transform.position - collision.transform.position).normalized;
            TakeDamage(1, knockbackDir);
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

    public void TakeDamage(int damageAmount, Vector2 knockbackDirection)
    {
        currentHealth -= damageAmount;
        StartCoroutine(ApplyKnockback(knockbackDirection));

        if (currentHealth <= 0)
        {
            Die();
        }
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