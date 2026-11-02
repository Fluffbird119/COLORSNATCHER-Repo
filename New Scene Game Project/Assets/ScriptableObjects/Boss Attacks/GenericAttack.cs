using UnityEngine;

[CreateAssetMenu(fileName = "GenericAttack", menuName = "Scriptable Objects/GenericAttack")]
public class GenericAttack : ScriptableObject
{
    public Sprite sprite;
    public bool compoundSprite; // useful for lasers. You can create a smaller sprite that acts as a section of the laser so it can dynamically resize. smaller sprites create less deformation
    public float spriteHeight;
    public Animator animation; // animations MUST have boolean flags labeled "preAtkAnim", "atkAnim", and "postAtkAnim"
    /*public bool preAtkAnim; // attack wind up/telegraph state flag
    public bool atkAnim; // attack animation state flag
    public bool postAtkAnim; // attack end animation state flag*/
    public PolygonCollider2D hitbox;
    public float damage;
    public bool endOnHit; // generally true for projectiles, false for lasers
    public Vector2[] startPoints = new Vector2[2]; // use 1 point (2nd is null) for projectiles.  if dynamicEnd, these will serve as offsets from the shootPointTransform
    public bool dynamicStart; // if false, moves the attacker to the set start positions
    public bool trackStart;
    public Vector2[] endPoints = new Vector2[2]; // use 1 point for projectiles. if dynamicEnd, these will serve as offsets from the shootPointTransform
    public bool dynamicEnd; // should typically be true if dynamicStart
    public RepositionInterface repositionScript; // use this if dynamicStart is false
    public Transform startTransform;
    public Transform shootPointTransform;
    public bool startFollowsShooter; // useful for lasers
    public Transform transformToTrack; // if this is not null, the attack will target this
    public bool stopAtTarget; // use for lasers if you don't want it to go past the player
    public float trackStrength;
    public float trackDuration;
    public bool telegraphs;
    public Color telegraphColor;
    public float attackDuration;
    public float startDelay; // telegraphs during this delay, if that is enabled
    public float pathCurveDepth; // makes it curve from start to end. the path will move clockwise for positive depth. generally don't use this with tracking attacks
    public float[] pathSpeedCheckpoints; // divides the distance of the path by the number of items
    public bool checkpointsByTime; // if this is set to true then attack duration is divided by the number of items instead
    public bool isCustomAttack;
    public MonoBehaviour customScript;
}
