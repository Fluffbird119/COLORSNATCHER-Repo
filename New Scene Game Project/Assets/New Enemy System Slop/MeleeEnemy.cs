using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MeleeEnemy", menuName = "Scriptable Objects/MeleeEnemy")]
public class MeleeEnemy : ScriptableObject
{
    public float Speed;
    public float StoppingDistance;
    public float MaxHealth;
    public float KnockbackForce;
    public float KnockbackDuration;
    public float RageDamageBuff;
    public float StunDamageBuff;
    public bool ProceduralSpeed;
    public float DistanceSpeedMod;
    public bool Reincarnates;
    public float SpriteAngleOffset;
    public GameObject HitParticlePrefab;
    public Sprite[] BodyParts;
    public bool IsTeleporter;
    public Animator Animator;
    public float EnemyLightRadius;
    public float EnemyLightFallOff;
    public Color EnemyLightColor;

    // hitbox stuff only works for one body part cuz fukc you
    public List<Vector2[]> PhysicsPaths = new List<Vector2[]>();
    private static readonly List<Vector2> pathBuffer = new List<Vector2>();

    private void OnValidate()
    {
        // clean list in between updates in the scriptable object
        PhysicsPaths.Clear();

        if (BodyParts[0] == null)
        {
            return;
        }

        // get total count of shapes in the sprite
        int shapeCount = BodyParts[0].GetPhysicsShapeCount();

        // for each shape get its shape and add it to the list
        for (int i = 0; i < shapeCount; i++)
        {
            pathBuffer.Clear();
            BodyParts[0].GetPhysicsShape(i, pathBuffer);
            PhysicsPaths.Add(pathBuffer.ToArray());
        }
    }
}
