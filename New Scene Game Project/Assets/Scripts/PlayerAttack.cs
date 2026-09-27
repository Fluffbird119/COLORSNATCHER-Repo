using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Animator tentacleAnim;
    [SerializeField] private Animator eyeAnim;

    [SerializeField] private GameObject tentacleObject;

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private GameObject eyeTentacle;

    // this collider is the one that is updated each frame for the tentacle
    [SerializeField] private PolygonCollider2D polygonCollider;

    // this collider is turned on during the few frames when tentacle is swinging across screen
    [SerializeField] private PolygonCollider2D tentacleSwipeHitbox;

    private Sprite curSprite;
    private readonly List<Vector2> physicsShapePath = new();

    private void Awake()
    {
        PolygonCollider2D[] colliders = eyeTentacle.GetComponents<PolygonCollider2D>();

        polygonCollider = colliders[0];
        tentacleSwipeHitbox = colliders[1];

        tentacleSwipeHitbox.enabled = false;
    }
    private void Start()
    {
        tentacleObject.SetActive(false);
        tentacleSwipeHitbox.enabled = false;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            eyeAnim.SetTrigger("Blink Eye");
        }
    }


    // all stuff for updating the polygon collider right
    void LateUpdate()
    {
        // Only update if the sprite frame has changed
        if (spriteRenderer.sprite != curSprite && polygonCollider.enabled)
        {
            curSprite = spriteRenderer.sprite;
            UpdateColliderShape();
        }
    }

    void UpdateColliderShape()
    {
        int shapeCount = curSprite.GetPhysicsShapeCount();
        polygonCollider.pathCount = shapeCount;

        for (int i = 0; i < shapeCount; i++)
        {
            curSprite.GetPhysicsShape(i, physicsShapePath);
            polygonCollider.SetPath(i, physicsShapePath);
        }
    }

    // the blink eye animation should call this 
    public void PauseEye()
    {
        eyeAnim.speed = 0;
        tentacleObject.SetActive(true);
        tentacleAnim.SetTrigger("Tentacle Attack");
    }

    // the end of the tentecle animatiion should call this
    public void ResumeEye()
    {
        tentacleObject.SetActive(false);
        eyeAnim.speed = 2f;
        AnimatorStateInfo stateInfo = eyeAnim.GetCurrentAnimatorStateInfo(0);
        eyeAnim.Play(stateInfo.shortNameHash, 0, stateInfo.normalizedTime + 0.1f);
    }

    public void TurnOnSwipeHitbox()
    {
        polygonCollider.enabled = false;
        tentacleSwipeHitbox.enabled = true;
    }

    public void TurnOffSwipeHitbox()
    {
        tentacleSwipeHitbox.enabled = false;
        polygonCollider.enabled = true;
    }
}
