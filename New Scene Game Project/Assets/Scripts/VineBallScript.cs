using UnityEngine;
using System.Collections;

public class VineBallScript : MonoBehaviour
{
    [SerializeField] private GameObject vineStunAOEPrefab;
    [SerializeField] private float rotationSpeed = 5f;
    private Transform sprite;
    void Start()
    {
        sprite = transform.GetChild(1);
    }
    private void Update()
    {
        sprite.Rotate(new Vector3(0, 0, rotationSpeed));
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
