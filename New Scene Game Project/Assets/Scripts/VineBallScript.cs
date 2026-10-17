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
    GameObject splat;

    void Start()
    {
        sprite = transform.GetChild(1);
        rb = gameObject.GetComponent<Rigidbody2D>();

        splat = Instantiate(vineStunAOEPrefab, new Vector3(1000, 0, 0), Quaternion.identity);
        splat.SetActive(false);
    }
    private void Update()
    {
        sprite.Rotate(new Vector3(0, 0, rotationSpeed));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tentacle"))
        {
            rb.linearVelocity *= 1.2f;
            rb.linearVelocity = HeadLookAtMouse.aimAngle * new Vector2(0, 1) * rb.linearVelocity.magnitude;
            ++hits;
            AudioManager.Instance.HitNoise();
        }
        if (collision.CompareTag("Player")) // this hitbox that this trigger will hit is only active when player is dashing
        {
            rb.linearVelocity *= 1.5f;
            hits += 2;
            AudioManager.Instance.HitNoise();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 hitPoint = collision.GetContact(0).point;

            splat.SetActive(true);
            splat.transform.position = hitPoint;

            gameObject.transform.position = new Vector3(1000, 0, 0);
            gameObject.SetActive(false);
        }
        else if (collision.gameObject.CompareTag("BoundaryWall"))
        {
            Vector2 hitPoint = transform.position;

            splat.SetActive(true);
            splat.transform.position = hitPoint;

            gameObject.transform.position = new Vector3(1000, 0, 0);
            gameObject.SetActive(false);
        }
        if (splat != null)
        {
            splat.transform.localScale = new Vector3(1, 1, 1) * (baseStartSizeMod + (hits / 4f));
        }
    }    
}
