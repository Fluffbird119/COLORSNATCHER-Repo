using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Animator tentacleAnim;
    [SerializeField] private Animator eyeAnim;

    [SerializeField] private GameObject tentacleObject;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PolygonCollider2D polygonCollider;
    private Sprite curSprite;
    private readonly List<Vector2> physicsShapePath = new();

    private void Start()
    {
        tentacleObject.SetActive(false);
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
        if (spriteRenderer.sprite != curSprite)
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
        Debug.Log("pause eye");
        eyeAnim.speed = 0;
        tentacleObject.SetActive(true);
        tentacleAnim.SetTrigger("Tentacle Attack");

    }

    // the end of the tentecle animatiion should call this
    public void ResumeEye()
    {
        Debug.Log("resume eye");
        tentacleObject.SetActive(false);
        eyeAnim.speed = 0.5f;
        AnimatorStateInfo stateInfo = eyeAnim.GetCurrentAnimatorStateInfo(0);
        eyeAnim.Play(stateInfo.shortNameHash, 0, stateInfo.normalizedTime + 0.01f);
    }
}
