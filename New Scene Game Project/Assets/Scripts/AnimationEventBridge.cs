using UnityEngine;

public class AnimationEventBridge : MonoBehaviour
{
    // i hate unity
    // this whole thing is needed cuz animtaion event thingy cant see past its own fucking object liek HELLO!>!? use your fucking eyes stupuid shit im telling you where ot look like its right there ufuckwitt

    [SerializeField] private PlayerAttack playerAttackScript;
    public void CallPauseEye()
    {
        playerAttackScript.PauseEye();
    }

    // Call this from the Tentacle's animation event
    public void CallResumeEye()
    {
        playerAttackScript.ResumeEye();
    }
}
