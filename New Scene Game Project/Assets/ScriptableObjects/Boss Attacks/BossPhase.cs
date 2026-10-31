using UnityEngine;

[CreateAssetMenu(fileName = "BossPhase", menuName = "Scriptable Objects/BossPhase")]
public class BossPhase : ScriptableObject
{
    public Sprite sprite;
    public Animator animation;
    public bool[] animStates;
    public MonoBehaviour idleBehavior; // what the boss does when not attacking
    public PolygonCollider2D hitbox;
    public float health;
    public GenericAttack[] genericAttacks;
    public int[] genericAttackAnimLinker; // the values held point to the associated anim state boolean
    public ComboAttack[] comboAttacks;
    public int[] comboAttackAnimLinker; // the values held point to the associated anim state boolean
    public float cooldown;
    public int stamina;
    public int staminaRegen;
    public bool isCustomPhase;
    public MonoBehaviour customPhaseScript;
    public bool phaseDone = false;
}
