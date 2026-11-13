using System.Collections;
using System;
using UnityEngine;
using System.IO;
using Unity.Mathematics;

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
    private int numHits;
    private float hitCooldown;
    private float currHitCooldown = 0f; // not in the scriptable object
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
    private bool trackingAttack;
    private Transform transformToTrack;
    private bool stopAtTarget;
    private float trackStrength;
    private float trackDuration;
    private bool telegraphs;
    private Color telegraphColor;
    private float attackDuration;
    private float startDelay;
    private float endDelay;
    private float pathCurveDepth; // makes it curve from start to end. the path will move clockwise for positive depth
    private float curveTime;
    private float[] pathSpeedCheckpoints; // divides the distance of the path by the number of items
    private bool checkpointsByTime;
    private bool speedByDistance;
    private bool isCustomAttack;
    private CustomAttackInterface customScript;
    private Coroutine attackRoutine; // not in the scriptable object
    private bool flagToEnd = false; // not in the scriptable object
    public bool attackDone {get; private set;} = false; // not in the scriptable object
    public static event Action<float> OnAttackLanding; // not in the scriptable object
    public static event Action<GameObject, bool> OnAttackCompletion; // not in the scriptable object

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hitbox = gameObject.GetComponent<PolygonCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currHitCooldown <= 0 && collision.gameObject.CompareTag("player"))
        {
            OnAttackLanding?.Invoke(damage);
            if (numHits > 0)
            {
                --numHits;
            }
            if (numHits == 0)
            {
                StartCoroutine(RunAttackEnd());
                OnAttackCompletion?.Invoke(gameObject, true);
                StopCoroutine(attackRoutine);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (currHitCooldown <= 0 && collision.CompareTag("player"))
        {
            OnAttackLanding?.Invoke(damage);
            if (numHits > 0)
            {
                --numHits;
            }
            if (numHits == 0)
            {
                StartCoroutine(RunAttackEnd());
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

            hitbox.pathCount = attackScriptableObject.hitbox.pathCount;
            for (int i = 0; i < attackScriptableObject.hitbox.pathCount; i++)
            {
                hitbox.SetPath(i, attackScriptableObject.hitbox.GetPath(i));
            }
            hitbox.isTrigger = attackScriptableObject.hitbox.isTrigger;
            hitbox.sharedMaterial = attackScriptableObject.hitbox.sharedMaterial;
            hitbox.offset = attackScriptableObject.hitbox.offset;
            hitbox.usedByEffector = attackScriptableObject.hitbox.usedByEffector;
            hitbox.compositeOperation = attackScriptableObject.hitbox.compositeOperation;

            damage = attackScriptableObject.damage;
            numHits = attackScriptableObject.numHits;
            hitCooldown = attackScriptableObject.hitCooldown;

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
            trackingAttack = attackScriptableObject.trackingAttack;
            stopAtTarget = attackScriptableObject.stopAtTarget;
            trackStrength = attackScriptableObject.trackStrength;
            trackDuration = attackScriptableObject.trackDuration;

            telegraphs = attackScriptableObject.telegraphs;
            telegraphColor = attackScriptableObject.telegraphColor;

            attackDuration = attackScriptableObject.attackDuration;
            startDelay = attackScriptableObject.startDelay;
            endDelay = attackScriptableObject.endDelay;
            pathCurveDepth = attackScriptableObject.pathCurveDepth;
            curveTime = attackScriptableObject.curveTime;
            pathSpeedCheckpoints = attackScriptableObject.pathSpeedCheckpoints;
            speedByDistance = attackScriptableObject.speedByDistance;
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

                if (trackingAttack)
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

                if (trackingAttack)
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
        flagToEnd = false;
    } // end ProccessScriptableObject()

    public void StartAttack(Vector2 arenaOffset) // update this to consider the offset of the arena
    {
        // TODO the phase controller/combo controller should handle unpooling
        ProccessScriptableObject(arenaOffset); // this just reads in the info from the ScriptableObject, and updates the initial positions

        if (isCustomAttack)
        {
            attackRoutine = StartCoroutine(customScript.RunAttack()); // still tracks the coroutine to enable interruptions
        }
        else
        {
            attackRoutine = StartCoroutine(RunAttack());
        }
    } // end StartAttack()

    private IEnumerator RunAttack()
    {
        animator.SetBool(preAtkAnimHash, preAtkAnim);
        animator.SetBool(atkAnimHash, false);
        animator.SetBool(postAtkAnimHash, false);

        Coroutine telegraphRoutine = StartCoroutine(TelegraphAttack());
        yield return new WaitForSeconds(startDelay);
        StopCoroutine(telegraphRoutine);
        currentPoints[0] = startPoints[0];
        currentPoints[0] = startPoints[0];

        hitbox.enabled = true;

        animator.SetBool(preAtkAnimHash, false);
        animator.SetBool(atkAnimHash, atkAnim);
        animator.SetBool(postAtkAnimHash, false);

        float timeInAttack = 0f;
        while ((timeInAttack += Time.deltaTime) < attackDuration)
        {
            if (flagToEnd)
            {
                StartCoroutine(RunAttackEnd());
                OnAttackCompletion?.Invoke(gameObject, false);
                yield break;
            }

            if (currHitCooldown > 0)
            {
                currHitCooldown -= Time.deltaTime;
            }
            if (trackDuration > 0)
            {
                trackDuration -= Time.deltaTime;
            }
            if (curveTime > 0)
            {
                curveTime -= Time.deltaTime;
            }

            if (endPoints[1] != null) // handle laser things
            {
                Vector2 caster;
                if (trackDuration > 0 && trackingAttack)
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
                float distanceToEnd1 = Vector2.Distance(currentPoints[1], endPoints[1]);
                float distanceFromStart0 = Vector2.Distance(currentPoints[0], startPoints[0]);
                float distanceFromStart1 = Vector2.Distance(currentPoints[1], startPoints[1]);
                float totalDistance0 = distanceToEnd0 + distanceFromStart0;
                float totalDistance1 = distanceToEnd1 + distanceFromStart1;

                int checkpointIndex;
                if (!checkpointsByTime)
                {
                    checkpointIndex = Mathf.FloorToInt(Mathf.Clamp01(distanceFromStart0 / totalDistance0) * (pathSpeedCheckpoints.Length - 1));
                }
                else
                {
                    checkpointIndex = Mathf.FloorToInt(Mathf.Clamp01(timeInAttack / attackDuration) * (pathSpeedCheckpoints.Length - 1));
                }

                Vector2[] targetPoints = new Vector2[2]
                {
                    endPoints[0],
                    endPoints[1]
                }; // defaults to the end points until path curve is determined

                if (pathCurveDepth != 0 && curveTime > 0) // determines path curve
                {
                    Vector2[] offsets = new Vector2[2]
                    {
                        endPoints[0] - startPoints[0],
                        endPoints[1] - startPoints[1]
                    };

                    float[] halfMagnitudes = new float[2]
                    {
                        offsets[0].magnitude / 2,
                        offsets[1].magnitude / 2
                    };

                    Vector2[] midPoints = new Vector2[2]
                    {
                        offsets[0].normalized * halfMagnitudes[0] + startPoints[0],
                        offsets[1].normalized * halfMagnitudes[1] + startPoints[1]
                    };

                    int depthSign = pathCurveDepth < 0 ? -1 : 1;

                    Vector2[] centerDirections = new Vector2[2]
                    {
                        new Vector2(depthSign * offsets[0].y, -depthSign * offsets[0].x).normalized,
                        new Vector2(depthSign * offsets[1].y, -depthSign * offsets[1].x).normalized
                    };

                    Vector2[] circleCenters = new Vector2[2]
                    {
                        midPoints[0] + (pathCurveDepth - (Mathf.Pow(halfMagnitudes[0], 2) / pathCurveDepth)) / 2 * centerDirections[0],
                        midPoints[1] + (pathCurveDepth - (Mathf.Pow(halfMagnitudes[1], 2) / pathCurveDepth)) / 2 * centerDirections[1]
                    };

                    float[] radiuses = new float[2]
                    {
                        Vector2.Distance(endPoints[0], circleCenters[0]),
                        Vector2.Distance(endPoints[1], circleCenters[1])
                    };

                    Vector2[] currPointOffsets = new Vector2[2]
                    {
                        currentPoints[0] - circleCenters[0],
                        currentPoints[1] - circleCenters[1]
                    };

                    float[] currAngles = new float[2]
                    {
                        Mathf.Atan2(currPointOffsets[0].y, currPointOffsets[0].x),
                        Mathf.Atan2(currPointOffsets[1].y, currPointOffsets[1].x)
                    };

                    if (!speedByDistance) // speed is not based on distance if it's tracking, since distance can change
                    {
                        currAngles[0] += depthSign * pathSpeedCheckpoints[checkpointIndex] / (2 * radiuses[0]);
                        currAngles[1] += depthSign * pathSpeedCheckpoints[checkpointIndex] / (2 * radiuses[1]);
                    }
                    else
                    {
                        currAngles[0] += depthSign * totalDistance0 * pathSpeedCheckpoints[checkpointIndex] / (2 * radiuses[0]);
                        currAngles[1] += depthSign * totalDistance1 * pathSpeedCheckpoints[checkpointIndex] / (2 * radiuses[1]);
                    }

                    targetPoints[0] = radiuses[0] * (Quaternion.Euler(0, 0, currAngles[0]) * new Vector2(0, 1));
                    targetPoints[1] = radiuses[1] * (Quaternion.Euler(0, 0, currAngles[1]) * new Vector2(0, 1));

                    // TODO figure out detection and handling for overshooting the end point. or don't. idc
                }

                if (!speedByDistance) // handles speed that is not based on distance
                {
                    currentPoints[0] = (targetPoints[0] - currentPoints[0]).normalized * pathSpeedCheckpoints[checkpointIndex];
                    currentPoints[1] = (targetPoints[1] - currentPoints[1]).normalized * pathSpeedCheckpoints[checkpointIndex];
                }
                else
                {
                    currentPoints[0] = pathSpeedCheckpoints[checkpointIndex] * totalDistance0 * (targetPoints[0] - currentPoints[0]).normalized;
                    currentPoints[1] = pathSpeedCheckpoints[checkpointIndex] * totalDistance1 * (targetPoints[1] - currentPoints[1]).normalized;
                }

                if (startFollowsShooter)
                {
                    currentPoints[0] = endPoints[0]; // endPoints overrides target points here
                }

                // TODO update rendering and such based on current points. rember the stopsAtTarget bool for spriting
            }
            else // handle projectile things
            {   
                if (trackDuration > 0 && trackingAttack)
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

                float distanceToEnd = Vector2.Distance(currentPoints[0], endPoints[0]);
                float distanceFromStart = Vector2.Distance(currentPoints[0], startPoints[0]);
                float totalDistance = distanceToEnd + distanceFromStart;

                int checkpointIndex;
                if (!checkpointsByTime)
                {
                    checkpointIndex = Mathf.FloorToInt(Mathf.Clamp01(distanceFromStart / totalDistance) * (pathSpeedCheckpoints.Length - 1));
                }
                else
                {
                    checkpointIndex = Mathf.FloorToInt(Mathf.Clamp01(timeInAttack / attackDuration) * (pathSpeedCheckpoints.Length - 1));
                }

                Vector2 targetPoint = endPoints[0];

                if (pathCurveDepth != 0 && curveTime > 0) // determines path curve
                {
                    Vector2 offset = endPoints[0] - startPoints[0];
                    float halfMagnitude = offset.magnitude / 2;
                    Vector2 midPoint = offset.normalized * halfMagnitude + startPoints[0];

                    int depthSign = pathCurveDepth < 0 ? -1 : 1;

                    Vector2 centerDirection = new Vector2(depthSign * offset.y, -depthSign * offset.x).normalized;
                    Vector2 circleCenter = midPoint + (pathCurveDepth - (Mathf.Pow(halfMagnitude, 2) / pathCurveDepth)) / 2 * centerDirection;
                    float radius = Vector2.Distance(endPoints[0], circleCenter);

                    Vector2 currPointOffset = currentPoints[0] - circleCenter;
                    float currAngle = Mathf.Atan2(currPointOffset.y, currPointOffset.x);

                    if (!speedByDistance) // speed is not based on distance if it's tracking, since distance can change
                    {
                        currAngle += depthSign * pathSpeedCheckpoints[checkpointIndex] / (2 * radius);
                    }
                    else
                    {
                        currAngle += depthSign * totalDistance * pathSpeedCheckpoints[checkpointIndex] / (2 * radius);
                    }

                    targetPoint = radius * (Quaternion.Euler(0, 0, currAngle) * new Vector2(0, 1));

                    // TODO figure out detection and handling for overshooting the end point. or don't. idc
                }

                if (!speedByDistance)
                {
                    Vector2 targetOffset = targetPoint - currentPoints[0];
                    if (targetOffset.magnitude > (endPoints[0] - currentPoints[0]).magnitude)
                    {
                        flagToEnd = true;
                    }
                    currentPoints[0] = targetOffset.normalized * pathSpeedCheckpoints[checkpointIndex];
                }
                else
                {
                    Vector2 targetOffset = targetPoint - currentPoints[0];
                    if (targetOffset.magnitude > (endPoints[0] - currentPoints[0]).magnitude)
                    {
                        flagToEnd = true;
                    }
                    currentPoints[0] = pathSpeedCheckpoints[checkpointIndex] * totalDistance * targetOffset.normalized;
                }

                transform.position = currentPoints[0];
                // TODO update rendering and such based on current point
            }

            yield return null;
        }

        OnAttackCompletion?.Invoke(gameObject, false);
        // TODO the phase controller/combo controller should handle repooling
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
                if (trackingAttack) // laser tracking stuff
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
                    if (startFollowsShooter)
                    {
                        Vector2 caster = shootPointTransform.position;
                        startPoints[0] = caster + startOffsets[0];
                        endPoints[0] = caster + startOffsets[1];
                    }
                }

                // TODO actually render the laser telegraph. only use start points
            }
            else
            {
                if (trackStart && trackingAttack)
                {
                    Vector2 caster = transformToTrack.position;

                    Vector2 realVsTargetOffset = caster - startPoints[1];
                    if (realVsTargetOffset.magnitude < trackStrength)
                    {
                        endPoints[0] = realVsTargetOffset.normalized * trackStrength;
                    }
                    else
                    {
                        endPoints[0] = caster;
                    }

                    if (startFollowsShooter)
                    {
                        caster = shootPointTransform.position;
                        startPoints[0] = caster + startOffsets[0];
                    }
                }
                else
                {
                    if (startFollowsShooter)
                    {
                        Vector2 caster = shootPointTransform.position;
                        startPoints[0] = caster + startOffsets[0];
                    }
                }

                // TODO actually render projectile telegraph. use start and end point
            }
        }
    } // end TelegraphAttack()

    private IEnumerator RunAttackEnd()
    {
        attackDone = true;

        animator.SetBool(preAtkAnimHash, false);
        animator.SetBool(atkAnimHash, false);
        animator.SetBool(postAtkAnimHash, postAtkAnim);

        hitbox.enabled = false;
        yield return new WaitForSeconds(endDelay);

        animator.SetBool(preAtkAnimHash, false);
        animator.SetBool(atkAnimHash, false);
        animator.SetBool(postAtkAnimHash, false);
    } // end RunAttackEnd()
}