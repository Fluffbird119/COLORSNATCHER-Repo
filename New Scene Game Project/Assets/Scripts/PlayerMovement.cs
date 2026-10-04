using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    [Header("Player Stuff")]
    public float speed;
    [SerializeField] private Rigidbody2D rb;

    [Header("Animation Stuff")]
    [SerializeField] Animator playerAnimation;

    Vector2 input;

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            // If a duplicate exists, destroy this GameObject to enforce the single instance
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Update()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

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

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    public float GetSpeed()
    {
        return speed;
    }
}
