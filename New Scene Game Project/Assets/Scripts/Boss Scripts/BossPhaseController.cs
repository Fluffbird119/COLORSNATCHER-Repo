using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseController : MonoBehaviour
{
    private BossPhase phaseScriptableObject; // not in the scriptable object (obviously)
    private Sprite sprite;
    private Animator animator;
    private AnimatorControllerParameter[] animStates;
    private MonoBehaviour idleBehavior; // TODO make an interface
    private MonoBehaviour attackBehavior; // TODO make an interface
    private GameObject arena;
    private PolygonCollider2D hitbox;
    private float health;
    private float defense;
    private float tempDefense; // not in the scriptable object. Serves as a way for defense to be dynamic
    private Coroutine defenseRoutine; // don't mind this
    private bool healthDrains;
    private float drainRate;
    private GenericAttack[] genericAttacks;
    private ComboAttack[] comboAttacks;
    private int[] attackAnimLinker;
    private int[] staminaCost;
    private bool shuffled;
    private float cooldown; // starts when the current attack ends
    private int stamina;
    private int currStamina; // not in the scriptable object
    private float staminaRegenTime;
    private bool isCustomPhase;
    private MonoBehaviour customPhaseScript; // TODO make an interface for this
    private List<GameObject> activeAttacks; // tracks the active attacks. Not in the scriptable object
    private Coroutine phaseRoutine;
    public bool phaseDone {get; private set;} = false;
    public static event Action<ScriptableObject> OnRunningAttack;
    public static event Action<ScriptableObject> OnPhaseCompletion;

    private void ProcessScriptableObject()
    {
        if (!isCustomPhase)
        {
            sprite = phaseScriptableObject.sprite;
            animator = phaseScriptableObject.animator;
            animStates = phaseScriptableObject.animStates;

            idleBehavior = phaseScriptableObject.idleBehavior;
            attackBehavior = phaseScriptableObject.attackBehavior;

            // TODO make an arena GameObject using arenaBounds from the scriptable object

            hitbox = phaseScriptableObject.hitbox;
            health = phaseScriptableObject.health;
            defense = phaseScriptableObject.defense;
            healthDrains = phaseScriptableObject.healthDrains;
            drainRate = phaseScriptableObject.drainRate;

            genericAttacks = phaseScriptableObject.genericAttacks;
            comboAttacks = phaseScriptableObject.comboAttacks;
            attackAnimLinker = phaseScriptableObject.attackAnimLinker;

            staminaCost = phaseScriptableObject.staminaCost;
            shuffled = phaseScriptableObject.shuffled;
            cooldown = phaseScriptableObject.cooldown;
            stamina = phaseScriptableObject.stamina;
            staminaRegenTime = phaseScriptableObject.staminaRegenTime;

            tempDefense = 1 - defense;
            currStamina = stamina;
        }
        else
        {
            // TODO custom phase things like event listeners
        }
    } // end ProcessScriptableObject()

    public void StartPhase(BossPhase phaseInfo)
    {
        phaseScriptableObject = phaseInfo;
        
        ProcessScriptableObject();

        if (isCustomPhase)
        {
            // TODO implement custom phase starting
        }
        else
        {
            phaseRoutine = StartCoroutine(RunPhase());
        }
    } // end StartPhase()

    public void EndPhase()
    {
        // TODO implement custom phase ending
        StopAllCoroutines();

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
    } // end EndPhase()

    private void EndCustomPhase()
    {
        // TODO custom phase stuff
    } // end EndCustomPhase()

    private IEnumerator RunPhase()
    {
        yield break;
    } // end RunPhase()

    public void TakeDamage(float amount)
    {
        health -= amount * tempDefense;

        if (health <= 0)
        {
            EndPhase();
        }
    }

    public void SetDefense(float value, float duration)
    {
        tempDefense = 1 - value;
        if (defenseRoutine != null)
        {
            StopCoroutine(defenseRoutine);
        }
        defenseRoutine = StartCoroutine(DefenseTimer(duration));
    }

    public void ResetDefense()
    {
        if (defenseRoutine != null)
        {
            StopCoroutine(defenseRoutine);
        }
        tempDefense = 1 - defense;
    }

    private IEnumerator DefenseTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        tempDefense = 1 - defense;
    }

    private IEnumerator StaminaRegenClock()
    {
        yield return new WaitForSeconds(staminaRegenTime);
        currStamina = stamina;
    }

    public void ForceRunAttack(ScriptableObject attack, bool interrupt)
    {
        // replaces the currentAttack with this attack. interrupting it if that is marked as true
    }
}