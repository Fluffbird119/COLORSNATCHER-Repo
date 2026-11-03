using UnityEngine;

[CreateAssetMenu(fileName = "ComboAttack", menuName = "Scriptable Objects/ComboAttack")]
public class ComboAttack : ScriptableObject
{
    public bool usesComboAttacks;
    public GenericAttack[] genericAttacks;
    public ComboAttack[] comboAttacks;
    public float delay;
    public bool preserveAttackEndPos; // used for chaining attacks, bursts at the end of an attack, etc
    public bool shuffled;
    public int numAttacksPerCombo; // useful for shuffling. it can choose a few from a pool of attacks
}
