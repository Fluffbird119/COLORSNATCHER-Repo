using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Animator tentacleAnim;
    [SerializeField] private Animator eyeAnim;

    [SerializeField] private GameObject tentacleObject;

    private bool isAttacking = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            eyeAnim.SetTrigger("Blink Eye");
            isAttacking = true;
        }

        if (isAttacking)
        {
            tentacleObject.AddComponent<PolygonCollider2D>();
        }

        Destroy(tentacleObject.GetComponent<PolygonCollider2D>());
    }

    // the blink eye animation should call this 
    public void PauseEye()
    {
        Debug.Log("pause eye");
        eyeAnim.speed = 0;
        tentacleObject.SetActive(true);
        tentacleAnim.SetTrigger("Tentacle Attack");

    }

    // the end of the tentecle animatiion should call this
    public void ResumeEye()
    {
        Debug.Log("resume eye");
        tentacleObject.SetActive(false);
        isAttacking = false;
        eyeAnim.speed = 0.5f;
        AnimatorStateInfo stateInfo = eyeAnim.GetCurrentAnimatorStateInfo(0);
        eyeAnim.Play(stateInfo.shortNameHash, 0, stateInfo.normalizedTime + 0.01f);
    }
}
