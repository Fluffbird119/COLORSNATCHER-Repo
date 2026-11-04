using System.Collections;
using System;
using UnityEngine;

public class BossAttackController : MonoBehaviour
{
    [SerializeField] private GenericAttack attackScriptableObject;
    private Sprite sprite;
    private bool compoundSprite;
    private float spriteHeight;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool preAtkAnim; // attack has a wind up/telegraph animation
    private bool atkAnim; // attack has an animation
    private bool postAtkAnim; // attack has an end animation
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
    private Vector2[] currentPoints = new Vector2[2]; // not in the scriptable object
    private Vector2[] startOffsets = new Vector2[2]; // not in the scriptable object
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
    public static event Action<float> OnAttackLanding; // not in the scriptable object
    public static event Action<GameObject, bool> OnAttackCompletion; // not in the scriptable object

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("player"))
        {
            OnAttackLanding?.Invoke(damage);
            if (endOnHit)
            {
                OnAttackCompletion?.Invoke(gameObject, true);
                StopCoroutine(attackRoutine);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("player"))
        {
            OnAttackLanding?.Invoke(damage);
            if (endOnHit)
            {
                OnAttackCompletion?.Invoke(gameObject, true);
                StopCoroutine(attackRoutine);
            }
        }
    }

    private void ProccessScriptableObject(Vector2 arenaOffset) // update this to consider the offset of the arena
    {
        isCustomAttack = attackScriptableObject.isCustomAttack;
        if (!isCustomAttack)
        {
            sprite = attackScriptableObject.sprite;
            compoundSprite = attackScriptableObject.compoundSprite;
            spriteHeight = attackScriptableObject.spriteHeight;
            spriteRenderer.sprite = sprite;

            animator = attackScriptableObject.animator;
            preAtkAnim = attackScriptableObject.preAtkAnim;
            atkAnim = attackScriptableObject.atkAnim;
            postAtkAnim = attackScriptableObject.postAtkAnim;

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
                    startOffsets[0] = startPoints[0];
                    startPoints[0] = startPoints[0] + caster;
                    startPoints[1] = startPoints[1] + startPoints[0];
                }
                else
                {
                    Vector2 caster = startTransform.position - shootPointTransform.position; // calculates and casts the offset
                    startPoints[0] += arenaOffset;
                    startPoints[1] += arenaOffset;
                    repositionScript.Reposition(startTransform, startPoints[0] + caster, transformToTrack);
                }

                if (dynamicEnd)
                {
                    Vector2 caster = shootPointTransform.position;
                    startOffsets[1] = endPoints[0];
                    endPoints[0] = endPoints[0] + caster;
                    endPoints[1] = endPoints[1] + endPoints[0];
                }
                else
                {
                    endPoints[0] += arenaOffset;
                    endPoints[1] += arenaOffset;
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
                    startPoints[0] += arenaOffset;
                    repositionScript.Reposition(startTransform, startPoints[0] + caster, transformToTrack);
                }

                if (dynamicEnd)
                {
                    Vector2 caster = shootPointTransform.position;
                    endPoints[0] = endPoints[0] + caster;
                }
                else
                {
                    endPoints[0] += arenaOffset;
                }

                if (transformToTrack)
                {
                    endPoints[0] = transformToTrack.position;
                }
            }

            currentPoints[0] = startPoints[0];
            currentPoints[1] = startPoints[1];
        }
        else
        {
            customScript = attackScriptableObject.customScript;
        }
        attackDone = false;
    } // end ProccessScriptableObject()

    public void StartAttack(Vector2 arenaOffset) // update this to consider the offset of the arena
    {
        // the phase controller should handle unpooling
        ProccessScriptableObject(arenaOffset); // this just reads in the info from the ScriptableObject, and updates the initial positions

        attackRoutine = StartCoroutine(RunAttack());
    } // end StartAttack()

    private IEnumerator RunAttack()
    {
        animator.SetBool(preAtkAnimHash, true);
        animator.SetBool(atkAnimHash, false);
        animator.SetBool(postAtkAnimHash, false);

        Coroutine telegraphRoutine = StartCoroutine(TelegraphAttack());
        yield return new WaitForSeconds(startDelay);
        StopCoroutine(telegraphRoutine);
        currentPoints[0] = startPoints[0];
        currentPoints[0] = startPoints[0];

        animator.SetBool(preAtkAnimHash, false);
        animator.SetBool(atkAnimHash, true);
        animator.SetBool(postAtkAnimHash, false);

        float timeInAttack = 0f;
        while ((timeInAttack += Time.deltaTime) < attackDuration)
        {
            if (endPoints[1] != null) // handle laser things
            {
                Vector2 caster;
                if (transformToTrack)
                {
                    caster = transformToTrack.position;

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

                if (startFollowsShooter)
                {
                    caster = shootPointTransform.position;
                    endPoints[0] = caster + startOffsets[1];
                }

                float distanceToEnd0 = Vector2.Distance(currentPoints[0], endPoints[0]);
                //float distanceToEnd1 = Vector2.Distance(currentPoints[1], endPoints[1]);
                float distanceFromStart0 = Vector2.Distance(currentPoints[0], startPoints[0]);
                //float distanceFromStart1 = Vector2.Distance(currentPoints[1], startPoints[1]);
                float totalDistance0 = distanceToEnd0 + distanceFromStart0;
                //float totalDistance1 = distanceToEnd1 + distanceFromStart1;

                int checkpointIndex;
                if (!checkpointsByTime)
                {
                    checkpointIndex = Mathf.FloorToInt(Mathf.Clamp01(distanceFromStart0 / totalDistance0) * (pathSpeedCheckpoints.Length - 1));
                }
                else
                {
                    checkpointIndex = Mathf.FloorToInt(Mathf.Clamp01(timeInAttack / attackDuration) * (pathSpeedCheckpoints.Length - 1));
                }
                // update currentPoints to the next position based on current position, end points, curveDepth etc.

                //update rendering and such based on current points
            }
            else // handle projectile things
            {
                if (transformToTrack)
                {
                    Vector2 caster = transformToTrack.position;

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

                // update currentPoints to the next position based on current position, end points, curveDepth etc.

                transform.position = currentPoints[0];
            }

            yield return null;
        }

        attackDone = true;
        OnAttackCompletion?.Invoke(gameObject, false);
        // the phase controller should handle repooling
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
                        caster = shootPointTransform.position;
                        startPoints[0] = caster + startOffsets[0];
                        endPoints[0] = caster + startOffsets[1];
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