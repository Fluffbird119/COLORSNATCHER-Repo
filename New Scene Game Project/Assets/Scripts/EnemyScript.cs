using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float speed = 3f;
    public float stoppingDistance = 0f;

    private Transform playerTransform;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void FixedUpdate()
    {
        if (playerTransform != null)
        {
            float distance = Vector2.Distance(rb.position, playerTransform.position);
            if (distance > stoppingDistance)
            {
                Vector2 newPosition = Vector2.MoveTowards(
                    rb.position,
                    playerTransform.position,
                    speed * Time.fixedDeltaTime
                );

                rb.MovePosition(newPosition);
            }
        }
    }
}