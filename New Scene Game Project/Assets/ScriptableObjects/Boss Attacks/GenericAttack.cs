using UnityEngine;

[CreateAssetMenu(fileName = "GenericAttack", menuName = "Scriptable Objects/GenericAttack")]
public class GenericAttack : ScriptableObject
{
    public Sprite sprite;
    public Animator animation;
    public bool preAtkAnim; // attack wind up/telegraph state flag
    public bool atkAnim; // attack animation state flag
    public bool postAtkAnim; // attack end animation state flag
    public PolygonCollider2D hitbox;
    public float damage;
    public Vector2[] startPoints = new Vector2[2]; // use 1 point for projectiles
    public Vector2[] endPoints = new Vector2[2]; // use 1 point for projectiles
    public Transform transformToTrack;
    public float trackStrength;
    public float trackDuration;
    public float attackDuration;
    public float pathCurveDepth; // makes it curve from start to end. the path will move clockwise for positive depth
    public float[] pathSpeedCheckpoints; // divides the distance of the path by the number of items
    public bool isCustomAttack;
    public MonoBehaviour customScript;
    public bool attackDone = false;
}
