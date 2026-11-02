using System.Collections;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BossAttackController : MonoBehaviour
{
    [SerializeField] private GenericAttack attackScriptableObject;
    private Sprite sprite;
    private bool compoundSprite;
    private float spriteHeight;
    private SpriteRenderer spriteRenderer;
    private Animator animation;
    /*private bool preAtkAnim; // attack wind up/telegraph state flag
    private bool atkAnim; // attack animation state flag
    private bool postAtkAnim; // attack end animation state flag*/
    private int preAtkAnimHash = Animator.StringToHash("preAtkAnim"); // not in the scriptable object
    private int atkAnimHash = Animator.StringToHash("atkAnim"); // not in the scriptable object
    private int postAtkAnimHash = Animator.StringToHash("postAtkAnim"); // not in the scriptable object
    private PolygonCollider2D hitbox;
    private float damage;
    private bool endOnHit;
    private Vector2[] startPoints = new Vector2[2]; // use 1 point for projectiles
    private bool dynamicStart;
    private bool trackStart;
    private Vector2[] endPoints = new Vector2[2]; // use 1 point for projectiles
    private bool dynamicEnd;
    private Vector2[] currentPoints = new Vector2[2];
    private RepositionInterface repositionScript;
    private Transform startTransform;
    private Transform shootPointTransform;
    private bool startFollowsShooter;
    private Transform transformToTrack;
    private bool stopAtTarget;
    private float trackStrength;
    private float trackDuration;
    private bool telegraphs;
    private Color telegraphColor;
    private float attackDuration;
    private float startDelay;
    private float pathCurveDepth; // makes it curve from start to end. the path will move clockwise for positive depth
    private float[] pathSpeedCheckpoints; // divides the distance of the path by the number of items
    private bool checkpointsByTime;
    private bool isCustomAttack;
    private MonoBehaviour customScript;
    private Coroutine attackRoutine; // not in the scriptable object
    public bool attackDone {get; private set;} = false; // not in the scriptable object

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void ProccessScriptableObject() // update this to consider the offset of the arena
    {
        isCustomAttack = attackScriptableObject.isCustomAttack;
        if (!isCustomAttack)
        {
            sprite = attackScriptableObject.sprite;
            compoundSprite = attackScriptableObject.compoundSprite;
            spriteHeight = attackScriptableObject.spriteHeight;
            spriteRenderer.sprite = sprite;

            animation = attackScriptableObject.animation;
            /*preAtkAnim = attackScriptableObject.preAtkAnim;
            atkAnim = attackScriptableObject.atkAnim;
            postAtkAnim = attackScriptableObject.postAtkAnim;*/

            hitbox = attackScriptableObject.hitbox;
            damage = attackScriptableObject.damage;
            endOnHit = attackScriptableObject.endOnHit;

            startPoints = attackScriptableObject.startPoints;
            dynamicStart = attackScriptableObject.dynamicStart;
            trackStart = attackScriptableObject.trackStart;
            endPoints = attackScriptableObject.endPoints;
            dynamicEnd = attackScriptableObject.dynamicEnd;
            repositionScript = attackScriptableObject.repositionScript;

            startTransform = attackScriptableObject.startTransform;
            shootPointTransform = attackScriptableObject.shootPointTransform;
            startFollowsShooter = attackScriptableObject.startFollowsShooter;
            transformToTrack = attackScriptableObject.transformToTrack;
            stopAtTarget = attackScriptableObject.stopAtTarget;
            trackStrength = attackScriptableObject.trackStrength;
            trackDuration = attackScriptableObject.trackDuration;

            telegraphs = attackScriptableObject.telegraphs;
            telegraphColor = attackScriptableObject.telegraphColor;

            attackDuration = attackScriptableObject.attackDuration;
            startDelay = attackScriptableObject.startDelay;
            pathCurveDepth = attackScriptableObject.pathCurveDepth;
            pathSpeedCheckpoints = attackScriptableObject.pathSpeedCheckpoints;
            checkpointsByTime = attackScriptableObject.checkpointsByTime;

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
    } // end ProccessScriptableObject()

    public void StartAttack() // update this to consider the offset of the arena
    {
        // unpool
        ProccessScriptableObject(); // this just reads in the info from the ScriptableObject, and updates the initial positions

        attackRoutine = StartCoroutine(RunAttack());
    } // end StartAttack()

    private IEnumerator RunAttack()
    {
        animation.SetBool(preAtkAnimHash, true);
        animation.SetBool(atkAnimHash, false);
        animation.SetBool(postAtkAnimHash, false);

        Coroutine telegraphRoutine = StartCoroutine(TelegraphAttack());
        yield return new WaitForSeconds(startDelay);
        StopCoroutine(telegraphRoutine);

        animation.SetBool(preAtkAnimHash, false);
        animation.SetBool(atkAnimHash, true);
        animation.SetBool(postAtkAnimHash, false);

        float timeInAttack = 0f;
        while ((timeInAttack += Time.deltaTime) < attackDuration)
        {
            if (endPoints[1] != null) // handle laser things
            {
                if (transformToTrack)
                {
                    
                }
            }
            else // handle projectile things
            {
                
            }
            if (transformToTrack)
            {
                Vector2 caster = transformToTrack.position;
                
                if (endPoints[1] != null) // laser tracking
                {
                    Vector2 realVsTargetOffset = caster - endPoints[1];
                    if (realVsTargetOffset.magnitude < trackStrength)
                    {
                        endPoints[1] = realVsTargetOffset.normalized * trackStrength;
                    }
                    else
                    {
                        endPoints[1] = caster;
                    }
                }
                else // projectile tracking
                {
                    Vector2 realVsTargetOffset = caster - endPoints[0];
                    if (realVsTargetOffset.magnitude < trackStrength)
                    {
                        endPoints[0] = realVsTargetOffset.normalized * trackStrength;
                    }
                    else
                    {
                        endPoints[0] = caster;
                    }
                }
            }

            // determine next position based on current position, end points, curveDepth etc.

            yield return null;
        }

        attackDone = true;
        yield break;
        // repool at the end of this coroutine
    } // end RunAttack()

    private IEnumerator TelegraphAttack()
    {
        if (!telegraphs)
        {
            yield break;
        }

        while (true) // runs until the coroutine is ended
        {
            if (startPoints[1] != null) // laser type attack
            {
                if (transformToTrack) // laser tracking stuff
                {
                    Vector2 caster = transformToTrack.position;

                    Vector2 realVsTargetOffset = caster - startPoints[1];
                    if (realVsTargetOffset.magnitude < trackStrength)
                    {
                        if (trackStart)
                        {
                            startPoints[1] = realVsTargetOffset.normalized * trackStrength;
                        }
                        endPoints[1] = realVsTargetOffset.normalized * trackStrength;
                    }
                    else
                    {
                        if (trackStart)
                        {
                            startPoints[1] = caster;
                        }
                        endPoints[1] = caster;
                    }

                    if (startFollowsShooter)
                    {
                        startPoints[0] = shootPointTransform.position;
                        endPoints[0] = startPoints[0];
                    }
                }
                else
                {
                    // do static laser stuff. only consider the start points
                }

                // actually render the laser telegraph
            }
            else
            {
                // do projectile stuff. shouldn't need to consider tracking
            }
        }
    } // end TelegraphAttack()
}