using UnityEngine;
using System;

public class BossComboController : MonoBehaviour
{
    private bool usesComboAttacks;
    private GenericAttack[] genericAttacks;
    private ComboAttack[] comboAttacks;
    private float delay;
    private bool preserveAttackEndPos;
    private bool isChain;
    private bool shuffled;
    private int numAttacksPerCombo;
    private GameObject currentAttack;
    public bool comboDone {get; private set;} = false;
    public static event Action<ScriptableObject> OnRunningAttack;
}
