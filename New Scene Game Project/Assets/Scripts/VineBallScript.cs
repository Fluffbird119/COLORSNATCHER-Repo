using UnityEngine;
using System.Collections;

public class VineBallScript : MonoBehaviour
{
    [SerializeField] private GameObject vineStunAOEPrefab;
    private GameObject VineStunAOE;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 hitPoint = collision.GetContact(0).point;

            VineStunAOE = Instantiate(vineStunAOEPrefab, hitPoint, Quaternion.identity);

            StartCoroutine(VineAOELastOnGround());

            Destroy(gameObject);
        }
    }

    private IEnumerator VineAOELastOnGround()
    {
        yield return new WaitForSeconds(5f);

        Destroy(VineStunAOE);
    }
}
