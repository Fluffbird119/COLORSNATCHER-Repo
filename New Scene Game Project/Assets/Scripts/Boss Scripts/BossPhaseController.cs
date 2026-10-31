using UnityEngine;

public class BossPhaseController : MonoBehaviour
{
    private Sprite sprite;
    private Animator animation;
    private bool[] animStates;
    private MonoBehaviour idleBehavior; // what the boss does when not attacking
    private PolygonCollider2D hitbox;
    private float health;
    private BossAttackController[] genericAttacks;
    private int[] genericAttackAnimLinker; // the values held point to the associated anim state boolean
    private BossComboController[] comboAttacks;
    private int[] comboAttackAnimLinker; // the values held point to the associated anim state boolean
    private float cooldown;
    private int stamina;
    private int staminaRegen;
    private bool isCustomPhase;
    private MonoBehaviour customPhaseScript;
    public bool phaseDone {get; private set;} = false;
}
