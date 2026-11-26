using UnityEngine;

[CreateAssetMenu(fileName = "BossPhase", menuName = "Scriptable Objects/BossPhase")]
public class BossPhase : ScriptableObject
{
    public Sprite sprite;
    public Animator animator;
    public AnimatorControllerParameter[] animStates;
    public MonoBehaviour idleBehavior; // what the boss does when not attacking
    public MonoBehaviour attackBehavior; // additional behaviors during an attack. this serves as the default behavior, use the OnRunningAttack events to get more dynamic
    public Rect arenaBounds; // the phase controller will make an arena GameObject
    public PolygonCollider2D hitbox;
    public float health;
    [Range(0f, 1f)]
    public float defense; // percent damage decrease
    public bool healthDrains; // causes health to drain over time
    public float drainRate; // rate at which health drains
    public GenericAttack[] genericAttacks;
    public ComboAttack[] comboAttacks;
    public int[] attackAnimLinker; // the values represent indeces in the animStates array
    public int[] staminaCost;  // size should be combined size of generic and combo attacks
    public bool shuffled;
    public float cooldown;
    public int stamina;
    public float staminaRegenTime; // time it takes for stamina to come back after depletion
    public bool isCustomPhase;
    public MonoBehaviour customPhaseScript;
}
