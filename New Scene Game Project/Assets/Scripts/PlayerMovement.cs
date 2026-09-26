using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Stuff")]
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rb;

    [Header("Animation Stuff")]
    [SerializeField] Animator playerAnimation;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        rb.linearVelocity = new Vector2(moveHorizontal, moveVertical).normalized * speed;

        if (rb.linearVelocity.magnitude > 0)
        {
            playerAnimation.SetBool("Player Walking", true);
        }
        else
        {
            playerAnimation.SetBool("Player Walking", false);
        }
    }
}
