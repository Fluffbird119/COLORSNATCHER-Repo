using UnityEngine;
using System.Collections;

public class VineBallScript : MonoBehaviour
{
    [SerializeField] private GameObject vineStunAOEPrefab;
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
