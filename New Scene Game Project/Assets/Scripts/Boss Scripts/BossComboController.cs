using UnityEngine;
using System;

public class BossComboController : MonoBehaviour
{
    private ComboAttack comboScriptableObject; // not in the scriptable object (obviously)
    private bool usesComboAttacks;
    private GenericAttack[] genericAttacks;
    private ComboAttack[] comboAttacks;
    private float delay;
    private bool preserveAttackEndPos;
    private bool isChain;
    private bool useLeader;
    private bool shuffled;
    private int numAttacksPerCombo;
    private Rect arena;
    private GameObject currentAttack; // not in the scriptable object
    public bool comboDone {get; private set;} = false; // not in the scriptable object
    public static event Action<ScriptableObject> OnRunningAttack; // not in the scriptable object
    public static event Action<GameObject, bool> OnComboCompletion; // not in the scriptable object

    private void ProccessScriptableObject(Vector2 arenaOffset)
    {
        usesComboAttacks = comboScriptableObject.usesComboAttacks;

        genericAttacks = comboScriptableObject.genericAttacks;
        comboAttacks = comboScriptableObject.comboAttacks;

        delay = comboScriptableObject.delay;
        preserveAttackEndPos = comboScriptableObject.preserveAttackEndPos;
        isChain = comboScriptableObject.isChain;
        useLeader = comboScriptableObject.useLeader;

        shuffled = comboScriptableObject.shuffled;
        numAttacksPerCombo = comboScriptableObject.numAttacksPerCombo;

        comboDone = false;
    }

    public void StartCombo()
    {
        
    }

    public void RunCombo()
    {
        
    }
}
