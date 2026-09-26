using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 5f;
    public PlayerHealth healthScript;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
            healthScript.ChangeHealth(-1);
        }
    }
}