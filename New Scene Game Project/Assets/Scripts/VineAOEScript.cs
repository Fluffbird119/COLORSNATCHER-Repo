using UnityEngine;
using System;
using System.Collections;

public class VineAOEScript : MonoBehaviour
{
    public float vineLastOnGround = 5f;
    private void OnEnable()
    {
        StartCoroutine(VineAOELastOnGround());
    }

    private IEnumerator VineAOELastOnGround()
    {
        yield return new WaitForSeconds(vineLastOnGround);

        gameObject.transform.position = new Vector2(1000, 0);
        gameObject.SetActive(false);
    }
}
