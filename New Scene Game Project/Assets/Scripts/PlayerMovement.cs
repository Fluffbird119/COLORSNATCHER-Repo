using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Stuff")]
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rb;

    [Header("Animation Stuff")]
    [SerializeField] Animator playerAnimation;

    Vector2 input;

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        input = new Vector2(moveHorizontal, moveVertical);

        rb.linearVelocity = input.normalized * speed;

        if (rb.linearVelocity.magnitude > 0)
        {
            playerAnimation.SetBool("Player Walking", true);
        }
        else
        {
            playerAnimation.SetBool("Player Walking", false);
        }
    }

    public void setSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}
