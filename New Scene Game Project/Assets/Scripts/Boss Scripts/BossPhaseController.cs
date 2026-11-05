using System;
using UnityEngine;

public class BossPhaseController : MonoBehaviour
{
    private Sprite sprite;
    private Animator animator;
    private AnimatorControllerParameter[] animStates;
    private MonoBehaviour idleBehavior; // what the boss does when not attacking
    private MonoBehaviour attackBehavior;
    private Rect arenaBounds;
    private PolygonCollider2D hitbox;
    private float health;
    private GenericAttack[] genericAttacks;
    private int[] genericAttackAnimLinker; // the values held point to the associated anim state boolean
    private ComboAttack[] comboAttacks;
    private int[] comboAttackAnimLinker; // the values held point to the associated anim state boolean
    private int[] staminaCost;
    private bool shuffled;
    //public ScriptableObject currentAttack; // tracks the current attack, useful for custom scripts
    private float cooldown; // starts when the current attack ends
    private int stamina;
    private int currStamina;
    private int staminaRegen;
    private bool isCustomPhase;
    private MonoBehaviour customPhaseScript;
    public bool phaseDone {get; private set;} = false;
    public static event Action<ScriptableObject> OnRunningAttack;

    public void ForceRunAttack(ScriptableObject attack, bool interrupt)
    {
        // replaces the currentAttack with this attack. interrupting it if that is marked as true
    }
}
