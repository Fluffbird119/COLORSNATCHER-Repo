using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private float strength = 0;
    public static event Action<float> OnHealthPickup;
    public static event Action<float> OnRagePickup;
    private List<Action<float>> events = new() {OnHealthPickup, OnRagePickup};
    private int eventIndex = 0;
    private float rotateSpeed = 3f;
    private float splootSpeed = 2f;
    private Vector3 direction;

    void Start()
    {
        direction = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f)) * new Vector2(0, 1);
        StartCoroutine(SplootOut()):
    }

    private IEnumerator SplootOut()
    {
        float timeSplooting = 2;
        while (timeSplooting > 0)
        {
            timeSplooting -= Time.deltaTime;
            gameObject.transform.rotation = Quaternion.Euler(0, 0, gameObject.transform.rotation.z + timeSplooting * rotateSpeed);
            gameObject.transform.position = gameObject.transform.position + direction * timeSplooting * splootSpeed;
            yield return null;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            events[eventIndex]?.Invoke(strength);
        }
    }

    public void CreateRagePickup(float value)
    {
        eventIndex = 1;
        strength = value;
    }

    public void CreateHealthPickup(float value)
    {
        eventIndex = 0;
        strength = value;
    }
}
