using UnityEngine;

[CreateAssetMenu(fileName = "Boss", menuName = "Scriptable Objects/Boss")]
public class Boss : ScriptableObject
{
    public BossPhase[] phases;
    public bool bossDone = false;
}
