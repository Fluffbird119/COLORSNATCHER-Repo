using JetBrains.Annotations;
using UnityEngine;

public class BossAttackController : MonoBehaviour
{
    private Sprite sprite;
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
}
