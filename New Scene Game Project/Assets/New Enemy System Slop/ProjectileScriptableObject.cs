using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileScriptableObject", menuName = "Scriptable Objects/ProjectileScriptableObject")]
public class ProjectileScriptableObject : ScriptableObject
{
    public float Velocity;
    public float Damage;
    public float Lifetime;
    public float BulletColliderRadius = 0.55f; // probably will always be .55 for fairness for player
    public Sprite BulletSprite;
    public float SpriteAngleOffset;
    public Vector3 ProjectileScale = new Vector3(1, 1, 1);
}