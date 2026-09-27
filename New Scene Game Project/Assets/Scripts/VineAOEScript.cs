using UnityEngine;
using System.Collections;

public class VineAOEScript : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(VineAOELastOnGround());
    }
    private IEnumerator VineAOELastOnGround()
    {
        yield return new WaitForSeconds(5f);

        transform.position = new Vector2(1000, 1000);

        yield return new WaitForFixedUpdate();
        Destroy(gameObject);
    }
}
