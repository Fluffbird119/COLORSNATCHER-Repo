using UnityEngine;
using System.Collections;

public class VineBallScript : MonoBehaviour
{
    [SerializeField] private GameObject vineStunAOEPrefab;
    [SerializeField] private float arenaMinBoundsX = -70f;
    [SerializeField] private float arenaMaxBoundsX = 70f;
    [SerializeField] private float arenaMinBoundsY = -55f;
    [SerializeField] private float arenaMaxBoundsY = 35f;
    private void Update()
    {
        if (transform.position.x > arenaMaxBoundsX || transform.position.y > arenaMaxBoundsY || transform.position.x < arenaMinBoundsX || transform.position.y < arenaMinBoundsY)
        {
            Vector2 hitPoint = transform.position;

            Instantiate(vineStunAOEPrefab, hitPoint, Quaternion.identity);

            Destroy(gameObject);
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
    }    
}
