using UnityEngine;

[CreateAssetMenu(fileName = "ComboAttack", menuName = "Scriptable Objects/ComboAttack")]
public class ComboAttack : ScriptableObject
{
    public bool usesComboAttacks;
    public GenericAttack[] genericAttacks;
    public ComboAttack[] comboAttacks;
    public float delay;
    public bool shuffled;
    public int numAttacksPerCombo;
    public bool comboDone = false;
}
