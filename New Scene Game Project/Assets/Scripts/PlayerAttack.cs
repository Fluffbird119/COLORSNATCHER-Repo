using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Animator tentacleAnim;
    [SerializeField] private Animator eyeAnim;

    [SerializeField] private GameObject tentacleObject;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            tentacleObject.SetActive(true);
            eyeAnim.SetTrigger("Blink Eye");
        }
    }

    // the blink eye animation should call this 
    public void PauseEye()
    {
        eyeAnim.speed = 0;
        tentacleAnim.SetTrigger("Tentacle Attack");
    }

    // the end of the tentecle animatiion should call this
    public void ResumeEye()
    {
        eyeAnim.speed = 0.5f;
        tentacleObject.SetActive(false);
    }
}
