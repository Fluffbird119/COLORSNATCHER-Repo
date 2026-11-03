using UnityEngine;

public class BossComboController : MonoBehaviour
{
    private bool usesComboAttacks;
    private GenericAttack[] genericAttacks;
    private ComboAttack[] comboAttacks;
    private float delay;
    private bool preserveAttackEndPos;
    private bool shuffled;
    private int numAttacksPerCombo;
    public bool comboDone {get; private set;} = false;
}
