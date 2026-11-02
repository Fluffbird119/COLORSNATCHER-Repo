using System.Collections;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BossAttackController : MonoBehaviour
{
    [SerializeField] private GenericAttack attackScriptableObject;
    private Sprite sprite;
    private SpriteRenderer spriteRenderer;
    private Animator animation;
    private bool preAtkAnim; // attack wind up/telegraph state flag
    private bool atkAnim; // attack animation state flag
    private bool postAtkAnim; // attack end animation state flag
    private PolygonCollider2D hitbox;
    private float damage;
    private Vector2[] startPoints = new Vector2[2]; // use 1 point for projectiles
    private bool dynamicStart;
    private bool trackStart;
    private Vector2[] endPoints = new Vector2[2]; // use 1 point for projectiles
    private bool dynamicEnd;
    private RepositionInterface repositionScript;
    private Transform startTransform;
    private Transform shootPointTransform;
    private Transform transformToTrack;
    private float trackStrength;
    private float trackDuration;
    private bool telegraphs;
    private Color telegraphColor;
    private float attackDuration;
    private float startDelay;
    private float pathCurveDepth; // makes it curve from start to end. the path will move clockwise for positive depth
    private float[] pathSpeedCheckpoints; // divides the distance of the path by the number of items
    private bool isCustomAttack;
    private MonoBehaviour customScript;
    public bool attackDone {get; private set;} = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void ProccessScriptableObject()
    {
        isCustomAttack = attackScriptableObject.isCustomAttack;
        if (!isCustomAttack)
        {
            sprite = attackScriptableObject.sprite;
            spriteRenderer.sprite = sprite;

            animation = attackScriptableObject.animation;
            preAtkAnim = attackScriptableObject.preAtkAnim;
            atkAnim = attackScriptableObject.atkAnim;
            postAtkAnim = attackScriptableObject.postAtkAnim;

            hitbox = attackScriptableObject.hitbox;
            damage = attackScriptableObject.damage;

            startPoints = attackScriptableObject.startPoints;
            dynamicStart = attackScriptableObject.dynamicStart;
            trackStart = attackScriptableObject.trackStart;
            endPoints = attackScriptableObject.endPoints;
            dynamicEnd = attackScriptableObject.dynamicEnd;
            repositionScript = attackScriptableObject.repositionScript;

            startTransform = attackScriptableObject.startTransform;
            shootPointTransform = attackScriptableObject.shootPointTransform;
            transformToTrack = attackScriptableObject.transformToTrack;
            trackStrength = attackScriptableObject.trackStrength;
            trackDuration = attackScriptableObject.trackDuration;

            telegraphs = attackScriptableObject.telegraphs;
            telegraphColor = attackScriptableObject.telegraphColor;

            attackDuration = attackScriptableObject.attackDuration;
            startDelay = attackScriptableObject.startDelay;
            pathCurveDepth = attackScriptableObject.pathCurveDepth;
            pathSpeedCheckpoints = attackScriptableObject.pathSpeedCheckpoints;

            if (startPoints[1] != null) // the attack is a laser (2 start points)
            {
                if (endPoints[1] == null)
                {
                    Debug.LogError("2 start points but 1 end point");
                }

                if (dynamicStart)
                {
                    Vector2 caster = shootPointTransform.position;
                    startPoints[0] = startPoints[0] + caster;
                    startPoints[1] = startPoints[1] + caster;
                }
                else
                {
                    Vector2 caster = startTransform.position - shootPointTransform.position; // calculates and casts the offset
                    repositionScript.Reposition(startTransform, startPoints[0] + caster, transformToTrack);
                }

                if (dynamicEnd)
                {
                    Vector2 caster = shootPointTransform.position;
                    endPoints[0] = endPoints[0] + caster;
                    endPoints[1] = endPoints[1] + caster;
                }

                if (transformToTrack)
                {
                    if (trackStart)
                    {
                        startPoints[1] = transformToTrack.position;
                    }
                    endPoints[1] = transformToTrack.position;
                }
            }
            else
            {
                if (endPoints[1] != null)
                {
                    Debug.LogError("1 start point but 2 end points");
                }

                if (dynamicStart)
                {
                    Vector2 caster = shootPointTransform.position;
                    startPoints[0] = startPoints[0] + caster;
                }
                else
                {
                    Vector2 caster = startTransform.position - shootPointTransform.position; // calculates and casts the offset
                    repositionScript.Reposition(startTransform, startPoints[0] + caster, transformToTrack);
                }

                if (dynamicEnd)
                {
                    Vector2 caster = shootPointTransform.position;
                    endPoints[0] = endPoints[0] + caster;
                }

                if (transformToTrack)
                {
                    endPoints[0] = transformToTrack.position;
                }
            }
        }
        else
        {
            customScript = attackScriptableObject.customScript;
        }
        attackDone = false;
    }

    public void StartAttack()
    {
        // unpool
        ProccessScriptableObject();

        StartCoroutine(RunAttack());
    }

    private IEnumerator RunAttack()
    {
        yield break;
        // repool at the end of this coroutine
    }
}
