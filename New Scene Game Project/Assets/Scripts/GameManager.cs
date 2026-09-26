using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int waveOneCount = 10;
    public float minBounds = -100f;
    public float maxBounds = 100f;
    public int killsThisWave = 0;

    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        StartCoroutine(DialogueSystem.instance.StartCutscene(1));
        while (DialogueSystem.instance.isCutscenePlaying)
        {
            yield return null;
        }
        CreateWave(1);
        while(killsThisWave < waveOneCount)
        {
            yield return null;
        }
        StartCoroutine(DialogueSystem.instance.StartCutscene(2));
    }

    public void CreateWave(int waveNumber)
    {
        if(waveNumber == 1)
        {
            for (int i = 0; i < waveOneCount; i++)
            {
                float randomX = Random.Range(minBounds, maxBounds);
                float randomY = Random.Range(minBounds, maxBounds);
                Vector2 spawnPosition = new Vector2(randomX, randomY);
                Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }
}