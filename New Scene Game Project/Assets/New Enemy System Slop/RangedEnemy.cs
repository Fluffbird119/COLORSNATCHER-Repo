using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RangedEnemy", menuName = "Scriptable Objects/RangedEnemy")]
public class RangedEnemy : ScriptableObject
{
    public ProjectileScriptableObject RangedEnemyBullet;
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
    public bool IsWalker;
    public float DistanceSpeedMod;
    public bool AimIncreaser;
    public float MoveSpeed;
    public GameObject HitParticlePrefab;
    public Sprite[] BodyParts;
    public RuntimeAnimatorController Animator;
    public Color LightColor = new Color(0, 0, 0, 255);
    public float LightIntensity;
    public float LightInnerRadius;
    public float LightOuterRadius;
    public float LightFallOff;
    public Color32 vineColor = new Color32(12, 182, 0, 255);

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
