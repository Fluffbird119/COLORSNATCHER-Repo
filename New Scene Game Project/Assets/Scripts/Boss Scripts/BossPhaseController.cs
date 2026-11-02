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
    private BossAttackController[] genericAttacks;
    private int[] genericAttackAnimLinker; // the values held point to the associated anim state boolean
    private BossComboController[] comboAttacks;
    private int[] comboAttackAnimLinker; // the values held point to the associated anim state boolean
    private Object currentAttack;
    private float cooldown;
    private int stamina;
    private int staminaRegen;
    private bool isCustomPhase;
    private MonoBehaviour customPhaseScript;
    public bool phaseDone {get; private set;} = false;
}
