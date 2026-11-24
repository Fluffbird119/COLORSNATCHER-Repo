using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ComboAttack", menuName = "Scriptable Objects/ComboAttack")]
public class ComboAttack : ScriptableObject
{
    public List<GenericAttack> genericAttacks;
    public List<ComboAttack> comboAttacks;
    public float delay;
    public bool preserveAttackEndPos; // used for chaining attacks, bursts at the end of an attack, etc
    public bool isChain; // if an attack is ended prematurely, the rest of the attacks will not run
    public bool useLeader; // positions will be based on the position of the first attack fired, chaining only considers the leader
    public bool shuffled;
    public bool repeatable;
    public int numAttacksPerCombo; // useful for shuffling. it can choose a few from a pool of attacks
    public GameObject arena;
}
