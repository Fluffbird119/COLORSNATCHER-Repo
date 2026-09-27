using System.Collections;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private PlayerPowerUps playerPowerUpScript;

    public float speed = 3f;
    public float stoppingDistance = 0f;
    public int maxHealth = 5;
    private int currentHealth;
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;
    private bool isKnockedBack = false;
    public int damage = 1;
    public bool proceduralSpeed = false;
    private Transform playerTransform;
    private Rigidbody2D rb;
    private float lastSpeedBoostTime = 0f;
    public bool reincarnates = false;
    private bool reincarnated = false;
    private bool isInDeathState = false;

    [SerializeField] private PlayerHealth healthScript;

    // change this to change how fix are roatated
    public float spriteAngleOffset = -90f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerPowerUpScript = player.GetComponent<PlayerPowerUps>();
        }
        lastSpeedBoostTime = Time.time;
    }

    void FixedUpdate()
    {
        if (isKnockedBack || isInDeathState) return;
        if (proceduralSpeed)
        {
            if (Time.time - lastSpeedBoostTime >= 5f)
            {
                speed *= 2f;
                transform.localScale *= 1.25f;
                lastSpeedBoostTime = Time.time;
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
                    speed * Time.fixedDeltaTime
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
        }
        if (collision.CompareTag("Player"))
        {
            healthScript.ChangeHealth(-1);
        }
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        currentHealth -= 1;
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
        isKnockedBack = false;
    }

    private void Die()
    {
        if (reincarnates && !reincarnated)
        {
            StartCoroutine(Reincarnate());
            return;
        }
        playerPowerUpScript.IncreaseRage(1f);
        GameManager.Instance.killsThisWave++;
        Destroy(gameObject);
    }

    private IEnumerator Reincarnate()
    {
        isInDeathState = true;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        RigidbodyType2D originalBodyType = rb.bodyType;
        rb.bodyType = RigidbodyType2D.Kinematic;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.gray;
            yield return new WaitForSeconds(5f);
            reincarnated = true;
            currentHealth = maxHealth;
            isInDeathState = false;
            rb.bodyType = originalBodyType;
            spriteRenderer.color = originalColor;
        }
        else
        {
            yield return new WaitForSeconds(5f);
            reincarnated = true;
            currentHealth = maxHealth;
            isInDeathState = false;
            rb.bodyType = originalBodyType;
        }
    }
}