using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BossComboController : MonoBehaviour
{
    private ComboAttack comboScriptableObject; // not in the scriptable object (obviously)
    private List<GenericAttack> genericAttacks;
    private List<ComboAttack> comboAttacks;
    private float delay;
    private bool preserveAttackEndPos;
    private Vector2 attackEndPos; // not in the scriptable object
    private bool isChain;
    private bool useLeader;
    private bool shuffled;
    private bool repeatable;
    private int numAttacksPerCombo;
    private GameObject arena;
    private int attackIndex; // not in the scriptable object
    private GameObject currentAttack; // not in the scriptable object
    private List<GameObject> activeAttacks; // not in the scriptable object
    private Coroutine comboRoutine; // not in the scriptable object
    public bool comboDone {get; private set;} = false; // not in the scriptable object
    public static event Action<ScriptableObject, GameObject> OnRunningAttack; // not in the scriptable object
    public static event Action<GameObject, bool> OnComboCompletion; // not in the scriptable object

    private void ProccessScriptableObject()
    {
        genericAttacks = comboScriptableObject.genericAttacks;
        comboAttacks = comboScriptableObject.comboAttacks;

        delay = comboScriptableObject.delay;
        preserveAttackEndPos = comboScriptableObject.preserveAttackEndPos;
        isChain = comboScriptableObject.isChain;
        useLeader = comboScriptableObject.useLeader;

        shuffled = comboScriptableObject.shuffled;
        repeatable = comboScriptableObject.repeatable;
        numAttacksPerCombo = comboScriptableObject.numAttacksPerCombo;

        arena = comboScriptableObject.arena;

        attackEndPos = new Vector2(0, 0);
        attackIndex = shuffled ? UnityEngine.Random.Range(0, genericAttacks.Count + comboAttacks.Count) : 0;
        currentAttack = null;
        comboRoutine = null;
        comboDone = false;
    }

    public void StartCombo(ComboAttack comboInfo)
    {
        comboScriptableObject = comboInfo;
        ProccessScriptableObject();

        BossAttackController.OnAttackCompletion += AttackEnded;

        comboRoutine = StartCoroutine(RunCombo());
    }

    public void EndCombo(bool isPremature) // ending prematurely will typically not do anything
    {
        if (comboRoutine != null)
        {
            StopCoroutine(comboRoutine);
        }

        BossAttackController.OnAttackCompletion -= AttackEnded;
        if (isPremature)
        {
            foreach (GameObject attack in activeAttacks)
            {
                if (attack.TryGetComponent<BossAttackController>(out var attackScript))
                {
                    attackScript.EndAttack(true);
                }
                else
                {
                    if (attack.TryGetComponent<BossComboController>(out var comboScript))
                    {
                        comboScript.EndCombo(true);
                    }
                }
            }
        }

        OnComboCompletion?.Invoke(gameObject, isPremature);
    }

    private IEnumerator RunCombo()
    {
        for (int i = 0; i < numAttacksPerCombo; ++i)
        {
            if (attackIndex >= genericAttacks.Count)
            {
                BossComboController comboController = null;
                // TODO unpool combo, set comboController to its script. also add it to activeAttacks and current attack (when applicable)

                comboController.StartCombo(comboAttacks[attackIndex - genericAttacks.Count]);
            }
            else
            {
                BossAttackController attackController = null;
                // TODO unpool attack, set attackController to its script. also add it to activeAttacks and current attack (when applicable)
                attackController.StartAttack(genericAttacks[attackIndex], arena.transform.position, preserveAttackEndPos || useLeader, currentAttack == null ? attackEndPos : currentAttack.transform.position);
            }

            if (shuffled)
            {
                if (!repeatable)
                {
                    if (attackIndex >= genericAttacks.Count)
                    {
                        comboAttacks.RemoveAt(attackIndex);
                    }
                    else
                    {
                        genericAttacks.RemoveAt(attackIndex);
                    }
                }

                attackIndex = UnityEngine.Random.Range(0, genericAttacks.Count + comboAttacks.Count);
            }
            else
            {
                ++attackIndex;
            }

            yield return new WaitForSeconds(delay);
        }
        while (activeAttacks.Count > 0)
        {
            yield return null;
        }
        EndCombo(false);
    }

    private void AttackEnded(GameObject attackObject, bool isPremature)
    {
        activeAttacks.Remove(attackObject);
        if (attackObject != currentAttack)
        {
            return;
        }

        if (isPremature && isChain)
        {
            EndCombo(true);
        }

        attackEndPos = currentAttack.transform.position;
        currentAttack = null;
    }
}