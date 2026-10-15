using UnityEditor.Analytics;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 10f;

    private Rigidbody2D rb;
    private bool parried = false;

    private float damage = 1;
    private float baseSpeed;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Vector3 playerColliderOffset = new Vector3(0.06916094f, -1.707474f, 0);

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        baseSpeed = rb.linearVelocity.magnitude;
        Destroy(gameObject, lifetime);
    }

    public void SetSize(float sizeMod)
    {
        transform.localScale *= sizeMod;
    }

    public void SetDamage(float damageValue)
    {
        damage = damageValue;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tentacle") && !parried && PlayerPowerUps.Instance.getIsRaged())
        {
            Transform playerTransform = collision.gameObject.transform.parent;
            rb.linearVelocity =  playerTransform.rotation * new Vector2(0, 1) * baseSpeed * 1.5f;
            Vector3 playerPos = playerTransform.position + playerColliderOffset;
            gameObject.transform.position = playerPos + playerTransform.rotation * new Vector2(0, 1) * (Vector2.Distance(gameObject.transform.position, playerPos) + 0.1f);
            parried = true;
            spriteRenderer.color = Color.red;
        }
        else if (collision.CompareTag("Enemy") && parried)
        {
            if (collision.gameObject.GetComponent<EnemyScript>() != null)
            {
                collision.gameObject.GetComponent<EnemyScript>().TakeDamage(2, rb.linearVelocity.normalized);
            }
            else if (collision.gameObject.GetComponent<ShooterScript>() != null)
            {
                collision.gameObject.GetComponent<ShooterScript>().TakeDamage(2, rb.linearVelocity.normalized);
                CameraShake.Instance.Shake(0.2f, 0.7f, CameraShake.ShakeAxis.Horizontal);
            }
            
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
            if (collision.gameObject.layer != LayerMask.NameToLayer("iframes")) {
                if (!parried) {
                    PlayerHealth.Instance.ChangeHealth(-damage);
                }
                else
                {
                    PlayerHealth.Instance.ChangeHealth(-1, 1);
                    AudioManager.Instance.LaughTrack();
                }
            }
        }
        else if (collision.CompareTag("VineAOE") && !parried)
        {
            SlowBullet();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("VineAOE") && !parried)
        {
            SlowBullet();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("VineAOE") && !parried)
        {
            UnSlowBullet();
        }
    }

    private void SlowBullet()
    {
        rb.linearVelocity = baseSpeed / 2f * rb.linearVelocity.normalized;
        spriteRenderer.color = Color.green;
    }

    private void UnSlowBullet()
    {
        rb.linearVelocity = baseSpeed* rb.linearVelocity.normalized;
        spriteRenderer.color = Color.white;
    }
}