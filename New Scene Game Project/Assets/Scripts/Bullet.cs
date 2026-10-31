using System.Collections;
using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private bool parried = false;

    private float damage = 1;
    private float baseSpeed;
    private float curLifetime;
    private float totalLifetime;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D bulletRB;
    [SerializeField] private CircleCollider2D bulletCollider;
    private Vector3 playerColliderOffset = new Vector3(0.06916094f, -1.707474f, 0);
    private bool parryImmunityWindow = false;
    public static event Action DashTriggerBulletHit;

    /*
    void Start()
    {   
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

    public void SetSpeed(float speedValue)
    {
        rb.linearVelocity = rb.linearVelocity.normalized * speedValue;
        baseSpeed = speedValue;
    }
    
    // Always call this when firing a bullet! add whatever you want to this tho
    public void Fire(Vector2 direction, float speed)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * speed;
        baseSpeed = speed;
    }
    */

    private void Update()
    {
        if (curLifetime > 0)
        {
            curLifetime -= 1f * Time.deltaTime;
        }
        else
        {
            ProjectileManager.Instance.ProjectilePool.Release(gameObject);
        }
    }

    public void PopulateBulletValues(ProjectileScriptableObject projectileScriptableObject, Transform firePoint, Vector2 direction)
    {
        damage = projectileScriptableObject.Damage;
        baseSpeed = projectileScriptableObject.Velocity;
        gameObject.transform.localScale = projectileScriptableObject.ProjectileScale;
        totalLifetime = projectileScriptableObject.Lifetime;
        curLifetime = totalLifetime;

        // set sprite to proper one
        spriteRenderer.sprite = projectileScriptableObject.BulletSprite;

        // set hitbox size (should probably always be .55 but i dunno)
        bulletCollider.radius = projectileScriptableObject.BulletColliderRadius;

        gameObject.transform.position = firePoint.position;

        // rotate bullet
        Vector2 finalDirection = Quaternion.Euler(0, 0, projectileScriptableObject.SpriteAngleOffset) * direction;
        bulletRB.rotation = Mathf.Atan2(finalDirection.y, finalDirection.x) * Mathf.Rad2Deg;

        // give bullet its speed and in the direction
        bulletRB.linearVelocity = finalDirection * projectileScriptableObject.Velocity;
    }

    public void RePoolObject()
    {
        bulletRB.linearVelocity = Vector3.zero;
        bulletRB.angularVelocity = 0f;

        ProjectileManager.Instance.ProjectilePool.Release(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tentacle") && !parried && PlayerPowerUps.Instance.getIsRaged())
        {
            Transform playerTransform = collision.gameObject.transform.parent;
            bulletRB.linearVelocity = playerTransform.rotation * new Vector2(0, 1) * baseSpeed * 1.5f;
            Vector3 playerPos = playerTransform.position + playerColliderOffset;
            gameObject.transform.position = playerPos + playerTransform.rotation * new Vector2(0, 1) * (Vector2.Distance(gameObject.transform.position, playerPos) + 0.1f);
            parried = true;
            curLifetime = totalLifetime; // reset lifetime when its parried
            spriteRenderer.color = Color.red;
            StartCoroutine(RunParryImmunity());
        }
        else if (collision.CompareTag("Enemy") && parried)
        {
            if (collision.gameObject.GetComponent<EnemyScript>() != null)
            {
                collision.gameObject.GetComponent<EnemyScript>().TakeDamage(2, bulletRB.linearVelocity.normalized);
            }
            else if (collision.gameObject.GetComponent<ShooterScript>() != null)
            {
                collision.gameObject.GetComponent<ShooterScript>().TakeDamage(2, bulletRB.linearVelocity.normalized);
                CameraShake.Instance.Shake(0.2f, 0.7f, CameraShake.ShakeAxis.Horizontal);
            }

            RePoolObject();
        }
        else if (collision.CompareTag("Player"))
        {
            if (collision.isTrigger)
            {
                DashTriggerBulletHit?.Invoke();
            }
            else
            {
                RePoolObject();
                if (collision.gameObject.layer != LayerMask.NameToLayer("iframes"))
                {
                    if (!parried)
                    {
                        PlayerHealth.Instance.ChangeHealth(-damage);
                    }
                    /*else if (!parryImmunityWindow)
                    {
                        PlayerHealth.Instance.ChangeHealth(-1, 1);
                        AudioManager.Instance.LaughTrack();
                    }*/
                }
            }
        }
        else if (collision.CompareTag("VineAOE") && !parried)
        {
            SlowBullet();
        }
        else if (collision.CompareTag("BoundaryWall"))
        {
            RePoolObject();
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
        bulletRB.linearVelocity = baseSpeed / 2f * bulletRB.linearVelocity.normalized;
        spriteRenderer.color = Color.green;
    }

    private void UnSlowBullet()
    {
        bulletRB.linearVelocity = baseSpeed * bulletRB.linearVelocity.normalized;
        spriteRenderer.color = Color.white;
    }

    private IEnumerator RunParryImmunity()
    {
        parryImmunityWindow = true;
        yield return new WaitForSeconds(0.2f);
        parryImmunityWindow = false;
    }
}