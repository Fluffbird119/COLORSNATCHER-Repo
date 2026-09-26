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
            // start eye animation and pause after 3 frames

            // play tentacle animation and turn on hitbox for that after keyframe index 2 and turn off hitbox on keyframe index 10

            // give tentacle hitbox
            tentacleObject.SetActive(true);
            tentacleObject.AddComponent<PolygonCollider2D>();
            tentacleAnim.SetTrigger("Tentacle Attack");

            // unpause eye animation then allow player to attack again

            // remove it so next frame will get a new accurate hitbox
            Destroy(tentacleObject.GetComponent<PolygonCollider2D>());
        }
    }
}
