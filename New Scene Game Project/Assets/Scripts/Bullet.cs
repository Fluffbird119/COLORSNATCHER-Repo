using UnityEditor.Analytics;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 10f;

    private Rigidbody2D rb;
    private bool parried = false;

    private float damage = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
            rb.linearVelocity =  playerTransform.rotation * new Vector2(0, 1) * rb.linearVelocity.magnitude;
            gameObject.transform.position = playerTransform.position + playerTransform.rotation * new Vector2(0, 1) * Vector2.Distance(gameObject.transform.position, playerTransform.position);
            parried = true;
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
            PlayerHealth.Instance.ChangeHealth(-damage);
        }
    }
}