using UnityEngine;

public class BossComboController : MonoBehaviour
{
    private bool usesComboAttacks;
    private BossAttackController[] genericAttacks;
    private BossComboController[] comboAttacks;
    private float delay;
    private bool shuffled;
    private int numAttacksPerCombo;
    public bool comboDone {get; private set;} = false;
}
