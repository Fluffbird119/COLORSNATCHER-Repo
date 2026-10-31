using UnityEngine;

[CreateAssetMenu(fileName = "Boss", menuName = "Scriptable Objects/Boss")]
public class Boss : ScriptableObject
{
    public BossPhase[] phases;
    // smth for boss health bar
    public float health; // this purely effects the health bar, phase health is what matters
    public bool bossDone = false;
}
