using UnityEngine;

[CreateAssetMenu(fileName = "RangedEnemy", menuName = "Scriptable Objects/RangedEnemy")]
public class RangedEnemy : ScriptableObject
{
    public GameObject Bullet;
    public Transform FirePoint;
    public float ShootingRange;
    public float FireRate;
    public float DistanceRateIncrease;
    public float DistanceAccuracyIncrease;
    public float DistanceMovePredict;
    public float Bulletspeed;
    public float BulletDamage;

}
