using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RangedEnemy", menuName = "Scriptable Objects/RangedEnemy")]
public class RangedEnemy : ScriptableObject
{
    public ProjectileScriptableObject RangedEnemyBullet;
    public Transform FirePoint;
    public float ShootingRange;
    public float FireRate;
    public float DistanceRateIncrease;
    public float DistanceAccuracyIncrease;
    public float DistanceMovePredict;
    public float RageDamageBuff;
    public float StunDamageBuff;
    [Range(0f, 1f)]
    public float Accuracy;
    public int MaxHealth;
    public float KnockbackForce;
    public float KnockbackDuration;
    public float IsWalker;
    public float DistanceSpeedMod;
    public bool AimIncreaser;
    public float MoveSpeed;
    public GameObject HitParticlePrefab;
    public Sprite[] BodyParts;
    public Animator Animator;
    public float LightRadius;
    public float LightFallOff;
    public Color LightColor;

    // hitbox stuff only works for one body part cuz fukc you
    public List<Vector2[]> PhysicsPaths = new List<Vector2[]>();
    private static readonly List<Vector2> pathBuffer = new List<Vector2>();

    private void OnValidate()
    {
        // as of me making this the firepoint is 0,0,0 for every ranged enemy so im just setting it to that
        FirePoint.localPosition = Vector3.zero;
        FirePoint.rotation = Quaternion.identity;
        FirePoint.localScale = Vector3.one;

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
