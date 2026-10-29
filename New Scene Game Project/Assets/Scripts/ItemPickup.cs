using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private float strength = 0;
    public static event Action<float> OnHealthPickup;
    public static event Action<float> OnRagePickup;
    public static event Action<float> OnDashPickup;
    private List<Action<float>> events = new() {OnHealthPickup, OnRagePickup, OnDashPickup};
    private int eventIndex = 0;
    private float rotateSpeed = 150f;
    private float splootSpeed = 3f;
    private Vector3 direction;
    private SpriteRenderer spriteRenderer;
    private float lifetime = 10f;
    private static List<GameObject> itemPickups = new List<GameObject>();
    [SerializeField] private Sprite[] sprites;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        direction = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f)) * new Vector2(0, 1);
        gameObject.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f));
        itemPickups.Add(gameObject);
        DialogueSystem.DialogueStateChange += DestroyOnDialogue;
        StartCoroutine(SplootOut());
        Destroy(gameObject, lifetime);
    }

    private IEnumerator SplootOut()
    {
        float timeSplooting = 2;
        while (timeSplooting > 0)
        {
            timeSplooting -= Time.deltaTime;
            gameObject.transform.Rotate(new Vector3(0, 0, timeSplooting * rotateSpeed * Time.deltaTime));
            gameObject.transform.position += direction * timeSplooting * splootSpeed * Time.deltaTime;
            yield return null;
        }
        StartCoroutine(AttemptMerge());
    }

    private IEnumerator AttemptMerge()
    {
        int i = 0;
        while (i < itemPickups.Count)
        {
            if (itemPickups[i] != gameObject && Vector3.Distance(itemPickups[i].transform.position, transform.position) < 5)
            {
                if (itemPickups[i].GetComponent<ItemPickup>().eventIndex == eventIndex)
                {
                    transform.localScale *= 1.2f;
                    strength += itemPickups[i].GetComponent<ItemPickup>().strength;
                    Destroy(itemPickups[i]);
                    Debug.Log("Merged Item Pickups");
                    yield break;
                }
            }
            ++i;
            yield return null;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            events[eventIndex]?.Invoke(strength);
            Destroy(gameObject);
        }
    }

    public void CreateRagePickup(float value)
    {
        eventIndex = 1;
        strength = value;
        transform.localScale *= 1 + ((value - 1) / 10);
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            spriteRenderer.sprite = sprites[UnityEngine.Random.Range(0, 3)];
        }
        else
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.red;
            spriteRenderer.sprite = sprites[UnityEngine.Random.Range(0, 3)];
        }
    }

    public void CreateHealthPickup(float value)
    {
        eventIndex = 0;
        strength = value;
        transform.localScale *= 1 + ((value - 1) / 10);
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprites[UnityEngine.Random.Range(0, 3)];
        }
        else
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprites[UnityEngine.Random.Range(0, 3)];
        }
    }

    public void CreateDashPickup(float value)
    {
        eventIndex = 2;
        strength = value;
        transform.localScale *= 1 + ((value - 1) / 10);
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.yellow;
            spriteRenderer.sprite = sprites[UnityEngine.Random.Range(0, 3)];
        }
        else
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.yellow;
            spriteRenderer.sprite = sprites[UnityEngine.Random.Range(0, 3)];
        }
    }

    private void DestroyOnDialogue(bool state)
    {
        if (state)
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        itemPickups.Remove(gameObject);
        DialogueSystem.DialogueStateChange -= DestroyOnDialogue;
    }
}
