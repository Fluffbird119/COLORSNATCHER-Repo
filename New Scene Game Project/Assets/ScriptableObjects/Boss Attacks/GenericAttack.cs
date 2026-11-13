using UnityEngine;

[CreateAssetMenu(fileName = "GenericAttack", menuName = "Scriptable Objects/GenericAttack")]
public class GenericAttack : ScriptableObject
{
    public Sprite sprite;
    public bool compoundSprite; // useful for lasers. You can create a smaller sprite that acts as a section of the laser so it can dynamically resize. smaller sprites create less deformation
    public float spriteHeight;
    public Animator animator; // animations MUST have boolean flags labeled "preAtkAnim", "atkAnim", and "postAtkAnim"
    public bool preAtkAnim; // attack has a wind up/telegraph animation
    public bool atkAnim; // attack has an animation
    public bool postAtkAnim; // attack has an end animation
    public PolygonCollider2D hitbox;
    public float damage;
    public int numHits; // number of times it can hit the player before ending. set to -1 to make it permanent
    public float hitCooldown; // delay before it can hit the player again
    public Vector2[] startPoints = new Vector2[2]; // use 1 point (2nd is null) for projectiles.  if dynamicEnd, these will serve as offsets from the shootPointTransform
    public bool dynamicStart; // if false, moves the attacker to the set start positions
    public bool trackStart;
    public Vector2[] endPoints = new Vector2[2]; // use 1 point for projectiles. if dynamicEnd, these will serve as offsets from the shootPointTransform
    public bool dynamicEnd; // should typically be true if dynamicStart
    public RepositionInterface repositionScript; // use this if dynamicStart is false
    public Transform startTransform;
    public Transform shootPointTransform;
    public bool startFollowsShooter; // useful for lasers
    public bool trackingAttack;
    public Transform transformToTrack; // if this is not null, the attack will target this
    public bool stopAtTarget; // use for lasers if you don't want it to go past the player. use for non tracking projectiles to not need to worry about attack duration
    public float trackStrength;
    public float trackDuration;
    public bool telegraphs;
    public bool usesSimpleTelegraph; // useful for testing/basic telegraphs
    public Color telegraphColor; // used for simple telegraph
    public float attackDuration;
    public float startDelay; // telegraphs during this delay, if that is enabled
    public float endDelay; // runs an ending animation if there is one. the hitbox will be disabled during this time
    public float pathCurveDepth; // makes it curve from start to end. the path will move clockwise for positive depth. generally don't use this with tracking attacks
    public float curveTime;
    public float[] pathSpeedCheckpoints; // divides the distance of the path by the number of items
    public bool checkpointsByTime; // if this is set to true then attack duration is divided by the number of items instead
    public bool speedByDistance; // typically do not do this for tracking attacks
    public bool isCustomAttack;
    public CustomAttackInterface customScript;
}
