using UnityEngine;
using System.Collections;

public class VineBallScript : MonoBehaviour
{
    [SerializeField] private GameObject vineStunAOEPrefab;
    [SerializeField] private float rotationSpeed = 5f;
    private Transform sprite;
    private Rigidbody2D rb;
    private int hits = 0;
    [SerializeField] private float baseStartSizeMod = 0.6f;
    void Start()
    {
        sprite = transform.GetChild(1);
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        sprite.Rotate(new Vector3(0, 0, rotationSpeed));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tentacle"))
        {
            rb.linearVelocity *= 1.1f;
            rb.linearVelocity = HeadLookAtMouse.aimAngle * new Vector2(0, 1) * rb.linearVelocity.magnitude;
            ++hits;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject splat = null;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 hitPoint = collision.GetContact(0).point;

            splat = Instantiate(vineStunAOEPrefab, hitPoint, Quaternion.identity);

            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("BoundaryWall"))
        {
            Vector2 hitPoint = transform.position;

            splat = Instantiate(vineStunAOEPrefab, hitPoint, Quaternion.identity);

            Destroy(gameObject);
        }
        if (splat != null)
        {
            splat.transform.localScale *= baseStartSizeMod + (hits / 5);
        }
    }    
}
