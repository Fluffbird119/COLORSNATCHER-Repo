using UnityEngine;

public class BossController : MonoBehaviour
{
    private BossPhaseController[] phases;
    // smth for boss health bar
    private float health; // this purely effects the health bar, phase health is what matters
    public bool bossDone {get; private set;} = false;
}
