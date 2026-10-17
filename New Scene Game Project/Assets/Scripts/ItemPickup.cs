using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private float strength = 0;
    public static event Action<float> OnHealthPickup;
    public static event Action<float> OnRagePickup;
    private List<Action<float>> events = new() {OnHealthPickup, OnRagePickup};
    private int eventIndex = 0;

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
