using UnityEngine;

[CreateAssetMenu(fileName = "ComboAttack", menuName = "Scriptable Objects/ComboAttack")]
public class ComboAttack : ScriptableObject
{
    public bool usesComboAttacks;
    public BossAttackController[] genericAttacks;
    public BossComboController[] comboAttacks;
    public float delay;
    public bool shuffled;
    public int numAttacksPerCombo;
}
