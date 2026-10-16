using UnityEngine;
using System.Collections;

public class VineBallScript : MonoBehaviour
{
    [SerializeField] private GameObject vineStunAOEPrefab;
    [SerializeField] private float rotationSpeed = 5f;
    private Transform sprite;
    private Rigidbody2D rb;
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
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 hitPoint = collision.GetContact(0).point;

            Instantiate(vineStunAOEPrefab, hitPoint, Quaternion.identity);

            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("BoundaryWall"))
        {
            Vector2 hitPoint = transform.position;

            Instantiate(vineStunAOEPrefab, hitPoint, Quaternion.identity);

            Destroy(gameObject);
        }
    }    
}
