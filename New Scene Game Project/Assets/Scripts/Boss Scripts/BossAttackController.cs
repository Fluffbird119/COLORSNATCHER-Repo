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
    private Vector2[] endPoints = new Vector2[2]; // use 1 point for projectiles
    private bool dynamicEnd;
    private Transform startTransform;
    private Transform shootPointTransform;
    private Transform transformToTrack;
    private float trackStrength;
    private float trackDuration;
    private bool telegraphs;
    private Color telegraphColor;
    private float attackDuration;
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
            animation = attackScriptableObject.animation;
            preAtkAnim = attackScriptableObject.preAtkAnim;
            atkAnim = attackScriptableObject.atkAnim;
            postAtkAnim = attackScriptableObject.postAtkAnim;
            hitbox = attackScriptableObject.hitbox;
            damage = attackScriptableObject.damage;
            startPoints = attackScriptableObject.startPoints;
            dynamicStart = attackScriptableObject.dynamicStart;
            endPoints = attackScriptableObject.endPoints;
            dynamicEnd = attackScriptableObject.dynamicEnd;
            startTransform = attackScriptableObject.startTransform;
            shootPointTransform = attackScriptableObject.shootPointTransform;
            transformToTrack = attackScriptableObject.transformToTrack;
            trackStrength = attackScriptableObject.trackStrength;
            trackDuration = attackScriptableObject.trackDuration;
            telegraphs = attackScriptableObject.telegraphs;
            telegraphColor = attackScriptableObject.telegraphColor;
            attackDuration = attackScriptableObject.attackDuration;
            pathCurveDepth = attackScriptableObject.pathCurveDepth;
            pathSpeedCheckpoints = attackScriptableObject.pathSpeedCheckpoints;

            if (dynamicStart)
            {
                startPoints[0] = shootPointTransform.position;
                if (startPoints[1] != null)
                {
                    if (transformToTrack != null)
                    {
                        startPoints[1] = transformToTrack.position;
                    }
                }
            }
            else
            {
                // send the startTransform to the correct position, rember shootPoint offset
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
