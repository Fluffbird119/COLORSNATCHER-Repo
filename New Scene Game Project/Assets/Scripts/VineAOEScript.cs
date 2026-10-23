using UnityEngine;
using System;
using System.Collections;

public class VineAOEScript : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(VineAOELastOnGround());
    }

    private IEnumerator VineAOELastOnGround()
    {
        yield return new WaitForSeconds(5f);

        gameObject.transform.position = new Vector2(1000, 0);
        gameObject.SetActive(false);
    }
}
