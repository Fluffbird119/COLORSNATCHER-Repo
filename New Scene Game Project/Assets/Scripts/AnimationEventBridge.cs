using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AnimationEventBridge : MonoBehaviour
{
    // i hate unity
    // this whole thing is needed cuz animtaion event thingy cant see past its own fucking object liek HELLO!>!? use your fucking eyes stupuid shit im telling you where ot look like its right there ufuckwitt

    [SerializeField] private PlayerAttack playerAttackScript;
    /*[SerializeField] private GameObject tentacleLight;
    [SerializeField] private Transform tentacleTransform;*/

    public void CallPauseEye()
    {
        playerAttackScript.PauseEye();
    }

    // Call this from the Tentacle's animation event
    public void CallResumeEye()
    {
        playerAttackScript.ResumeEye();
    }

    public void CallTurnOnSwipeHitbox()
    {
        playerAttackScript.TurnOnSwipeHitbox();
    }

    public void CallTurnOffSwipeHitbox()
    {
        playerAttackScript.TurnOffSwipeHitbox();
    }
    
    /*public void ActivateLight()
    {
        tentacleLight.SetActive(true);
    }

    public void DeactivateLight()
    {
        tentacleLight.SetActive(false);
    }

    public void LightPos1()
    {
        tentacleLight.transform.position = tentacleTransform.position + (HeadLookAtMouse.aimAngle * new Vector3(0f, 1f, 0f));
    }

    public void LightPos2()
    {
        tentacleLight.transform.position = tentacleTransform.position + (HeadLookAtMouse.aimAngle * new Vector3(0f, 2f, 0f));
    }

    public void LightPos3()
    {
        tentacleLight.transform.position = tentacleTransform.position + (HeadLookAtMouse.aimAngle * new Vector3(0f, 3f, 0f));
    }*/
}
